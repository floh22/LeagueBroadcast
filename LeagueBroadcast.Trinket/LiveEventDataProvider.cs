using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LeagueBroadcast.Trinket
{
    public class LiveEventDataProvider : IDisposable
    {
        private bool _run = true;
        private Socket? soc;

        public bool Connected => IsAlive();
        public EventHandler<LiveEventArgs>? OnLiveEvent { get; set; }
        public EventHandler<string>? OnConnectionError { get; set; }
        public EventHandler? OnConnect { get; set; }
        public EventHandler? OnDisconnect { get; set; }

        private static LiveEventDataProvider? _instance;

        public static LiveEventDataProvider Instance
        {
            get
            {
                if (_instance is null)
                {
                    _instance = new();
                }

                return _instance;
            }
        }


    public LiveEventDataProvider()
        {
            OnConnectionError += (s, e) => soc?.Dispose();
        }

        public void Connect()
        {
            new Task(async () =>
            {
                soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    soc.Connect("127.0.0.1", 34243);
                }
                catch (Exception e)
                {
                    OnConnectionError?.Invoke(this, e.Message);
                    soc = null;
                    return;
                }

                OnConnect?.Invoke(this, EventArgs.Empty);
                _run = true;
                while (_run)
                {
                    if (soc.Available > 0)
                    {
                        int size = soc.Available;
                        string responseContent = "";
                        byte[] bytes = new byte[size];
                        try
                        {
                            _ = soc.Receive(bytes, 0, size, SocketFlags.None);
                            responseContent = Encoding.UTF8.GetString(bytes);
                        }
                        catch
                        {
                            return;
                        }

                        char[] chars = responseContent.ToCharArray();

                        int openBrackets = 0;
                        int startOfEvent = 0;
                        for (int i = 0; i < chars.Length; i++)
                        {
                            char c = chars[i];
                            if (c == '{')
                            {
                                openBrackets++;
                                continue;
                            }
                            if (c == '}')
                            {
                                openBrackets--;
                                if (openBrackets == 0)
                                {
                                    int length = i - startOfEvent + 1;
                                    if (length != 0)
                                    {
                                        char[] e = new char[length];
                                        Array.Copy(chars, startOfEvent, e, 0, length);
                                        LiveEventArgs response = JsonSerializer.Deserialize<LiveEventArgs>(new string(e))!;
                                        startOfEvent = i + 1;
                                        OnLiveEvent?.Invoke(this, response);
                                    }
                                }
                            }
                        }
                    }
                    await Task.Delay(5);
                }

                soc.Dispose();
                soc = null;

                OnDisconnect?.Invoke(this, EventArgs.Empty);
            }).Start();
        }

        public void Disconnect()
        {
            _run = false;
        }

        private bool IsAlive()
        {
            return soc != null && !((soc.Poll(1000, SelectMode.SelectRead) && (soc.Available == 0)) || !soc.Connected);
        }

        public void Dispose()
        {
            ((IDisposable?)soc)?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
