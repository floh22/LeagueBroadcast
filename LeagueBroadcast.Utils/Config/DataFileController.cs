using LeagueBroadcast.Utils;
using LeagueBroadcast.Utils.Log;
using System.Text.Json;
using LeagueBroadcast.Common.Exceptions;
using LeagueBroadcast.Common.Config;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace LeagueBroadcast.Utils.Config
{
    public static class DataFileController
    {
        private static string? _dataDir;

        private static readonly Dictionary<string, object> files = InitDataList();
        private static readonly JsonSerializerOptions SerializationOptions = new()
        {
            WriteIndented = true,
            AllowTrailingCommas = true,
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString,
            ReadCommentHandling = JsonCommentHandling.Skip,
            IncludeFields = true
        };

        private static readonly FileSystemWatcher watcher = InitFileSystemWatcher();
        private static readonly HashSet<string> filesToWatch = new();
        private static readonly HashSet<string> waitingForRead = new();


        public static async Task<T> GetAsync<T>(string fileName) where T : IDataFile
        {
            var success = files.TryGetValue(fileName, out var config);
            if (!success)
            {
                config = await LoadAndGetAsync<T>(fileName);
            }
            return (T)config!;
        }

        private static async Task<object?> LoadAndGetAsync<T>(string fileName) where T : IDataFile
        {
            $"Attempting to retrieve unloaded config".Debug();
            if (await RegisterDataFileAsync(typeof(T), fileName) && files.TryGetValue(fileName, out var file))
                return file;
            throw new InvalidConfigException($"Unable to retrieve already loaded config");
        }



        public static async Task<bool> RegisterDataFileAsync<T>(string fileName) where T : IDataFile
        {
            return await RegisterDataFileAsync(typeof(T), fileName);
        }

        public static async Task<bool> RegisterDataFileAsync(Type fileType, string fileName)
        {
            if (!fileType.ImplementsInterface(typeof(IDataFile)))
            {
                throw new InvalidConfigException("Tried converting invalid object to DataFile. Config must implement IDataFile");
            }

            object? dataFileObj = Activator.CreateInstance(fileType) ?? throw new InvalidConfigException("Could not instantiate config file type");

            {
                object? fileObj = Activator.CreateInstance(fileType) ?? throw new InvalidConfigException("Could not instantiate config file type");
                IDataFile castFileObj = (IDataFile)fileObj;

                if (castFileObj.IsLoaded())
                {
                    $"Attempted to load {fileName} even though it has already been loaded".Debug();
                    return false;
                }

                $"Attempting to load {fileName}".Debug();
                var configDirContents = Directory.GetFiles(castFileObj.FilePath).Select(file => Path.GetFileName(file));

                if (configDirContents.Contains(fileName))
                {
                    //TODO Update this for non json files!
                    JsonElement readConfig = JsonSerializer.Deserialize<JsonElement>(await File.ReadAllTextAsync(Path.Combine(castFileObj.FilePath, fileName)));
                    fileObj = readConfig.ToObject(fileType, SerializationOptions);
                    //end json specific code

                    //Make sure config was read
                    if (fileObj is null)
                    {
                        $"Could not read {fileName}".Error();
                        return false;
                    }
                    castFileObj = (IDataFile)fileObj;

                    //Update config to latest file format
                    if (castFileObj.FileVersion < castFileObj.CurrentVersion)
                    {
                        $"{fileName} update detected".Debug();
                        castFileObj.CheckForUpdate();
                        castFileObj.FileVersion = castFileObj.CurrentVersion;
                    }

                    files.Add(fileName, fileObj);
                    $"{JsonSerializer.Serialize(readConfig, SerializationOptions)}".Debug();
                    $"{fileName} loaded".Info();
                    return true;
                }

                $"{fileName} not found. Generating default config".Info();
                castFileObj.RevertToDefault(fileName);
                castFileObj.FileVersion = castFileObj.CurrentVersion;
                $"{JsonSerializer.Serialize(fileObj, SerializationOptions)}".Debug();
                files.Add(fileName, fileObj);
                await WriteDataFileAsync(fileObj);
                $"{fileName} generated".Info();
                return true;
            }

        }

        public static async Task WriteDataFileAsync<T>(string fileName) where T : IDataFile
        {
            if (!files.ContainsKey(fileName))
            {
                $"Attempted to write an unloaded config!".Warn();
                return;
            }

            await WriteDataFileAsync(files[fileName]);
        }


        public static async Task WriteDataFileAsync(object fileObject)
        {
            IFileConfig castDataFile = (IFileConfig)fileObject;
            $"Writing {castDataFile.Name} to file".Debug();
            //TODO Update this for non json files!
            await File.WriteAllTextAsync(Path.Combine(castDataFile.FilePath, castDataFile.Name), JsonSerializer.Serialize(fileObject, SerializationOptions));
            $"{castDataFile.Name} saved".Debug();
        }


        #region LoadStatus
        public static bool IsLoaded(this IDataFile file)
        {
            return files.ContainsKey(file.Name);
        }
        #endregion



        private static Dictionary<string, object> InitDataList()
        {
            AppDomain.CurrentDomain.ProcessExit += OnAppExit;
            return new Dictionary<string, object>();
        }

        private static FileSystemWatcher InitFileSystemWatcher()
        {
            FileSystemWatcher watcher = new() { Filter = "*", NotifyFilter = NotifyFilters.LastWrite, Path = GetDataDirectory(), IncludeSubdirectories = true, EnableRaisingEvents = true };
            watcher.Error += OnError;
            watcher.Changed += OnChanged;
            $"Watching {GetDataDirectory()} for changes".Debug();
            return watcher;
        }

        public static string GetDataDirectory()
        {
            return _dataDir ?? CreateDataDirectory();

        }

        private static string CreateDataDirectory()
        {
            string workingDirectory = WorkingDirectory.GetDirectoryAsync().Result;

            string dataDir = Path.Combine(workingDirectory, "Data");
            Directory.CreateDirectory(dataDir);
            _dataDir = dataDir;
            return dataDir;
        }

        private static void OnAppExit(object? sender, EventArgs e)
        {
            "Saving all data files".Info();
            try
            {
                watcher.Dispose();
                files.ToList().ForEach(async pair =>
                {
                    await WriteDataFileAsync(pair.Value);
                });
                "Configs saved".Info();
            }
            catch
            {
                "Error saving all config files. Some files may be outdated".Warn();
            }

            Logger.CloseLoggers();
        }

        #region ConfigWatcher


        public static bool WatchFile(IDataFile file)
        {
            $"Watching {file.Name} for changes".Debug();
            return filesToWatch.Add(file.Name);
        }

        public static bool UnwatchFile(IDataFile file)
        {
            $"Stopped watching {file.Name} for changes".Debug();
            return filesToWatch.Remove(file.Name);
        }


        private static async void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.Name is null || waitingForRead.Contains(e.Name))
                return;
            waitingForRead.Add(e.Name);

            $"{e.Name} change detected".Debug();
            int attempts = 0;
            while (attempts < 10)
            {

                try
                {
                    await Task.Delay(500);
                    //await ReloadDataFileAsync(e.Name);
                    await Task.Delay(500);
                    waitingForRead.Remove(e.Name);
                    return;
                }
                catch (IOException)
                {
                    $"{e.Name} locked, cannot read!".Debug();
                    attempts++;
                }

            }
            $"Timed out reading updated data file {e.Name}".Warn();
        }

        private static void OnError(object sender, ErrorEventArgs e)
        {
            $"File Watch error: {e.GetException()}".Warn();
        }

        #endregion
    }
}
