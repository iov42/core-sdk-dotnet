using System;
using System.Threading.Tasks;
using Iov42sdk.Support;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests.Support
{
    public class TestHelper
    {
        public static void AllNull(params object[] items)
        {
            foreach (var item in items) 
                Assert.IsNull(item);
        }

        public static async Task<ResponseResult<T>> CallAndRetry<T>(Func<Task<ResponseResult<T>>> getCall)
        {
            var count = 1;
            ResponseResult<T> result = null;
            while (count <= 3)
            {
                result = await getCall();
                // Retry due to eventual consistency
                if (result.Success)
                    return result;
                await Task.Delay(500);
                count++;
            }

            return result;
        }
    }
}