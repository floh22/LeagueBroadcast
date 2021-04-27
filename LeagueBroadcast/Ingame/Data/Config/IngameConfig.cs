using LeagueBroadcast.Common.Data.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcast.Ingame.Data.Config
{
    class IngameConfig : JSONConfig
    {
        public override string Name => "Ingame";

        public override string FileVersion { get => _fileVersion; set => _fileVersion = value; }

        public GameConstantValues GameConstants;


        public override string GETCurrentVersion() => "1.0";

        public override string GETDefaultString()
        {
            throw new NotImplementedException();
        }

        private IngameConfig CreateDefault()
        {
            return new() { };
        }

        public override string GETJson()
        {
            throw new NotImplementedException();
        }

        public override void RevertToDefault()
        {
            throw new NotImplementedException();
        }

        public override void UpdateConfigVersion(string oldVersion, dynamic oldValues)
        {
            throw new NotImplementedException();
        }

        public override void UpdateValues(string readValues)
        {
            throw new NotImplementedException();
        }

        public class GameConstantValues
        {
            
        }
    }
}
