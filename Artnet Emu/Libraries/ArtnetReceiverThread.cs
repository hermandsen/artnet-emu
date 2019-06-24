using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using ArtnetEmu.Model.Interfaces;

namespace ArtnetEmu.Libraries
{
    public class ArtnetReceiverThread
    {
        private Thread ServerThread;
        private ArtnetReceiver server;
        private IPreloader[] preloaders;
        protected System.Timers.Timer TimeoutTimer;
        public event ThreadStateChangedEvent OnThreadStateChanged;
        public ArtnetReceiverThread(ArtnetReceiver server)
        {
            this.server = server;
            this.server.OnThreadStateChanged += ThreadStateChangedEventHandler;
        }
        public void Start(params IPreloader[] preloaders)
        {
            this.preloaders = preloaders;
            ServerThread = new Thread(new ThreadStart(StartThread));
            ServerThread.Start();
        }
        public void Stop(int timeout = 3000)
        {
            server.Stop();
            TimeoutTimer = new System.Timers.Timer();
            TimeoutTimer.Elapsed += TimerElapsedEventHandler;
            TimeoutTimer.AutoReset = false;
            TimeoutTimer.Interval = timeout;
            TimeoutTimer.Enabled = true;
        }
        private void TimerElapsedEventHandler(object sender, ElapsedEventArgs e)
        {
            var t = (System.Timers.Timer)sender;
            t.Enabled = false;
            if (ServerThread != null)
            {
                server.Terminate();
                ServerThread.Abort();
                InvokeOnThreadStateChanged(ArtnetServerState.Terminated, null);
                ServerThread = null;
            }
            t.Elapsed -= TimerElapsedEventHandler;
            t.Dispose();
            TimeoutTimer = null;
            GC.Collect();
        }
        private void StartThread()
        {
            try
            {
                foreach (var preloader in preloaders)
                {
                    preloader.Preload();
                }
                preloaders = null;
                server.Start();
            }
            catch (Exception e)
            {
                InvokeOnThreadStateChanged(ArtnetServerState.Terminated, e.Message);
            }
        }
        private void ThreadStateChangedEventHandler(ArtnetServerState state, string message)
        {
            if (state == ArtnetServerState.Terminated && TimeoutTimer != null)
            {
                TimeoutTimer.Enabled = false;
                TimeoutTimer.Dispose();
                TimeoutTimer = null;
            }
            InvokeOnThreadStateChanged(state, message);
        }
        private void InvokeOnThreadStateChanged(ArtnetServerState state, string message)
        {
            OnThreadStateChanged?.Invoke(state, message);
        }
    }
}
