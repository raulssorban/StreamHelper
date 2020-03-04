using Humanlights.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamHelper
{
    //
    // This should work as a background worker.
    // Update configured text files and execute --.
    //
    public class StreamManager
    {
        public static StreamManager Singleton { get; private set; } = new StreamManager ();

        public List<ListingFile> ListFiles { get; set; } = new List<ListingFile> ();
        public bool RandomMode { get; set; }

        private Dictionary<string, BackgroundActivity> Queue { get; set; } = new Dictionary<string, BackgroundActivity> ();

        public void Serialize ()
        {
            OsEx.File.Create ( $"{AppDomain.CurrentDomain.BaseDirectory}config.ini", JsonConvert.SerializeObject ( this, Formatting.Indented ) );
        }
        public static StreamManager Deserialize ()
        {
            if ( !OsEx.File.Exists ( $"{AppDomain.CurrentDomain.BaseDirectory}config.ini" ) )
            {
                return new StreamManager ();
            }

            return JsonConvert.DeserializeObject<StreamManager> ( OsEx.File.ReadText ( $"{AppDomain.CurrentDomain.BaseDirectory}config.ini" ) );
        }

        public void Install ()
        {
            foreach ( var listFile in ListFiles )
            {
                PlayLoopForFile ( listFile );
                Console.Write ( $"Looping now: {listFile.FilePath} (every {listFile.LoopTime} {StringEx.Plural ( ( int ) listFile.LoopTime, "second", "seconds" )})." );
            }
        }

        public static string GetStreamFolder ()
        {
            var folder = $"{AppDomain.CurrentDomain.BaseDirectory}Streamed Files";
            OsEx.Folder.Create ( folder );

            return folder;
        }

        public void AddFile ( string filePath, float loopTime )
        {
            var file = new ListingFile ()
            {
                LoopTime = loopTime,
                FilePath = filePath
            };
            ListFiles.Add ( file );

            PlayLoopForFile ( file );
        }
        public void RemoveFile ( string filePath )
        {
            ListFiles.RemoveAll ( x => x.FilePath == filePath );

            var activity = Queue.FirstOrDefault ( x => x.Key == filePath );

            if ( activity.Value != null )
            {
                Console.WriteLine ( $"Stopped file loop: {filePath}" );
                activity.Value.Stop ();
                Queue.Remove ( activity.Key );
            }
        }

        public void PlayLoopForFile ( ListingFile file )
        {
            var index = 0;

            var activity = new BackgroundActivity ( () =>
            {
                var lines = OsEx.File.ReadTextLines ( file.FilePath );
                if ( index > lines.Length - 1 ) index = 0;

                var currentLine = lines [ RandomMode ? index = RandomEx.GetRandomInteger ( 0, lines.Length - 1 ) : index ];
                var fileName = Path.GetFileNameWithoutExtension ( file.FilePath );
                var fileExtension = Path.GetExtension ( file.FilePath );
                var streamedFile = $"{GetStreamFolder ()}\\{fileName}_stream{fileExtension}";

                OsEx.File.Create ( streamedFile, currentLine );

                if ( !RandomMode )
                {
                    index++;
                    if ( index > lines.Length - 1 ) index = 0;
                }

            }, file.LoopTime, true );
            activity.Run ();

            Queue.Add ( file.FilePath, activity );
        }

        [System.Serializable]
        public class ListingFile
        {
            public string FilePath { get; set; }
            public float LoopTime { get; set; } = 2.5f;
        }
    }
}