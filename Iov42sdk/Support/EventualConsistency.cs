using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Iov42sdk.Support
{
    internal class EventualConsistency
    {
        private static readonly TimeSpan ConsistencyDelay = TimeSpan.FromMilliseconds(500);

        private DateTime _lastWriteLimit = DateTime.MinValue;
        private DateTime _start;
        private bool _writePerformed;

        public void StartWriteOperation()
        {
            _start = DateTime.UtcNow;
            _writePerformed = true;
        }

        public void EndWriteOperation()
        {
            var now = DateTime.UtcNow;
            // Save the best estimate when persistence will be complete - crudely scaled
            // based on how long the last write operation took
            var totalMilliseconds = now.Subtract(_start).TotalMilliseconds;
            Debug.WriteLine(totalMilliseconds);
            _lastWriteLimit = now.AddMilliseconds(totalMilliseconds);
        }

        public async Task ReadOperation()
        {
            // Allow eventual consistency of persistence to kick in
            if (_lastWriteLimit <= DateTime.UtcNow || !_writePerformed) 
                return;
            _writePerformed = false;
            await Task.Delay(ConsistencyDelay);
            _lastWriteLimit = DateTime.MinValue;
        }
    }
}