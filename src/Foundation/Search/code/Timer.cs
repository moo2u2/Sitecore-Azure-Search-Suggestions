using System;
using System.Diagnostics;

namespace Sitecore.HabitatHome.Foundation.Search
{
    // From Sitecore.XA.Foundation.Search.Timer
    public class Timer : IDisposable
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public long Msec => _stopwatch.ElapsedMilliseconds;

        public Timer()
        {
            _stopwatch.Start();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            _stopwatch?.Stop();
        }
    }
}