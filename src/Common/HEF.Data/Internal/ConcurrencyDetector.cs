using System;
using System.Threading;

namespace HEF.Data
{
    public class ConcurrencyDetector : IConcurrencyDetector
    {
        private readonly IDisposable _disposer;
        private int _inCriticalSection;
        private static readonly AsyncLocal<bool> _threadHasLock = new AsyncLocal<bool>();
        private int _refCount;
        
        public ConcurrencyDetector() => _disposer = new Disposer(this);
                
        public virtual IDisposable EnterCriticalSection()
        {
            if (Interlocked.CompareExchange(ref _inCriticalSection, 1, 0) == 1)
            {
                if (!_threadHasLock.Value)
                {
                    throw new InvalidOperationException(
                        "A second operation started on this context before a previous operation completed");
                }
            }
            else
            {
                _threadHasLock.Value = true;
            }

            _refCount++;
            return _disposer;
        }

        private void ExitCriticalSection()
        {
            if (--_refCount == 0)
            {
                _threadHasLock.Value = false;
                _inCriticalSection = 0;
            }
        }

        private readonly struct Disposer : IDisposable
        {
            private readonly ConcurrencyDetector _concurrencyDetector;

            public Disposer(ConcurrencyDetector concurrencyDetector)
                => _concurrencyDetector = concurrencyDetector;

            public void Dispose() => _concurrencyDetector.ExitCriticalSection();
        }
    }
}
