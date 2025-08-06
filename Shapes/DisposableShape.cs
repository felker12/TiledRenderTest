using System;

namespace TiledRenderTest.Shapes
{
    public abstract class DisposableShape : IDisposable
    {
        private bool disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected abstract void DisposeManaged();

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    DisposeManaged();
                }
                disposed = true;
            }
        }
    }
}
