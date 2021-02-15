using System;
using System.Threading.Tasks;

namespace Iov42sdk.Support
{
    internal class EventualConsistency
    {
        private static readonly TimeSpan ConsistencyDelay = TimeSpan.FromMilliseconds(500);

        private DateTime _lastWriteLimit = DateTime.MinValue;
        private DateTime _start;

        public void StartWriteOperation()
        {
            _start = DateTime.UtcNow;
        }

        public void EndWriteOperation()
        {
            var now = DateTime.UtcNow;
            // Save the best estimate when persistence will be complete - crudely scaled
            // based on how long the last write operation took
            _lastWriteLimit = now.AddMilliseconds(now.Subtract(_start).Milliseconds * 2);
        }

        public async Task ReadOperation()
        {
            // Allow eventual consistency of persistence to kick in
            if (_lastWriteLimit <= DateTime.UtcNow) 
                return;
            await Task.Delay(ConsistencyDelay);
            _lastWriteLimit = DateTime.MinValue;
        }
    }
}