using Humanlights;
using Humanlights.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamHelper.Commands
{
    [Factory ( Name = "manager" )]
    class Manager : CommandLibrary
    {
        [Command ( Subname = "add", Help = "Appends a file to the stream background worker." )]
        public static void AddStreamedFile ()
        {
            var loopingTime = Choice ( "Looping time" ).ToFloat ();
            var filePath = Choice ( "File path" );

            StreamManager.Singleton.AddFile ( filePath, loopingTime );
        }

        [Command ( Subname = "remove", Help = "Removes a file that's currently in the stream background worker." )]
        public static void Remove ()
        {
            StreamManager.Singleton.RemoveFile ( Choice ( "File name" ) );
        }

        [Command ( Subname = "randommode", Help = "Randomly picks an index out of the lines of a file." )]
        public static void RandomMode ()
        {
            StreamManager.Singleton.RandomMode = Parameter.ToBool ();
        }

        [Command ( Subname = "save", Help = "Saves currently loaded/appended listable files to a config." )]
        public static void Save ()
        {
            StreamManager.Singleton.Serialize ();
        }
    }
}
