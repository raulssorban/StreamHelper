using Humanlights.ConsoleHelper;

namespace StreamHelper
{
    class Program
    {
        static void Main ( string [] args )
        {
            var console = new ConsoleHandler ();
            var streamManager = StreamManager.Deserialize ();
            streamManager.Install ();

            console.Install ();
            console.Run ();
        }
    }
}
