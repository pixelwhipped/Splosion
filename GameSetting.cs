using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.Storage;

namespace Splosion
{

    public class ScoreData
    {
        public int HighScore;
        public int GameScore;
        public int Level;
        public int BestScore;
        public int RushHighScore;
    }

    [DataContract]
    public class GameSettings
    {
     public ScoreData Data
        {
            get
            {
                if (_data != null) return _data;
                _result.Wait();
                if (_result.Result != null)
                {
                    _data = _result.Result;
                }
                else
                {
                    ResetToDefault();
                }
                return _data;
            }
        }

        [DataMember] public ScoreData _data;

        private readonly Task<ScoreData> _result;

        public Splosion Game;

        public GameSettings(Splosion game)
        {
            Game = game;
            if (Utilities.DoesFileExistAsync(ApplicationData.Current.RoamingFolder, "Data"))
            {
                _result = Load<ScoreData>(ApplicationData.Current.RoamingFolder, "Data");
            }
            else
            {
                _data = new ScoreData();
                Save();
            }
        }

        public async void Save()
        {
            await Save<ScoreData>(ApplicationData.Current.RoamingFolder, "Data", Data);            
        }

        public void ResetToDefault()
        {
            _data = new ScoreData();
            Save();
        }



        public static async Task Save<T>(StorageFolder folder, string fileName, object instance)
        {
            var newFile = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            var newFileStream = await newFile.OpenStreamForWriteAsync();
            var ser = new DataContractSerializer(typeof (T));
            ser.WriteObject(newFileStream, instance);
            newFileStream.Dispose();
        }

        public static async Task<T> Load<T>(StorageFolder folder, string fileName)
        {
            try
            {
                var newFile = await folder.GetFileAsync(fileName);
                var newFileStream = await newFile.OpenStreamForReadAsync();
                var ser = new DataContractSerializer(typeof (T));
                var b = (T) ser.ReadObject(newFileStream);
                newFileStream.Dispose();
                return b;
            }
            catch
            {
                return default(T);
            }
        }
    }
}
