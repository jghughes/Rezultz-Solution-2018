using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetStd.Goodies.Mar2022
{
    // for the source of this see https://www.hanselman.com/blog/ComparingTwoTechniquesInNETAsynchronousCoordinationPrimitives.aspx

    public sealed class JghAsyncLock
    {
        private readonly SemaphoreSlim _mSemaphore = new(1, 1);
        private readonly Task<IDisposable> _mReleaser;

        public JghAsyncLock()
        {
            _mReleaser = Task.FromResult((IDisposable)new Releaser(this));
        }

        public Task<IDisposable> LockAsync()
        {
            var wait = _mSemaphore.WaitAsync();
            return wait.IsCompleted ?
                _mReleaser :
                wait.ContinueWith((_, state) => (IDisposable)state,
                    _mReleaser.Result, CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }

        private sealed class Releaser : IDisposable
        {
            private readonly JghAsyncLock _mToRelease;
            internal Releaser(JghAsyncLock toRelease) { _mToRelease = toRelease; }
            public void Dispose() { _mToRelease._mSemaphore.Release(); }
        }
    }
}
