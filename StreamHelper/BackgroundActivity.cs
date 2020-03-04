using Humanlights.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamHelper
{
    public class BackgroundActivity
    {
        public static List<BackgroundActivity> All = new List<BackgroundActivity> ();

        public Action Action;
        public float Seconds;
        public bool Repeat;
        public float ElapsedTime { get { return ( float ) Timer.Interval; } }

        private System.Timers.Timer Timer = new System.Timers.Timer ();

        public static BackgroundActivity Create ( Action action, float seconds, bool repeat )
        {
            var activity = new BackgroundActivity ( action, seconds, repeat );

            All.Add ( activity );
            return activity;
        }

        public BackgroundActivity () { }
        public BackgroundActivity ( Action action, float seconds, bool repeat )
        {
            Action = action;
            Seconds = seconds;
            Repeat = repeat;
        }

        public void Run ()
        {
            var Miliseconds = Seconds.Clamp ( 0.001f, Seconds ) * 1000;
            Timer.AutoReset = Repeat;
            Timer.Elapsed += delegate { if ( !Repeat ) All.Remove ( this ); Action?.Invoke (); };
            Timer.Interval = Miliseconds;
            Timer.Start ();
        }

        public void Stop ()
        {
            Timer.Stop ();
        }
    }
}