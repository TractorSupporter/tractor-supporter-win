using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TractorSupporter.Tests.UI;

public static class Retry
{
    /// <summary>
    /// Retries the provided action until it succeeds or the timeout is reached.
    /// </summary>
    /// <typeparam name="T">The type of the result returned by the action.</typeparam>
    /// <param name="action">The function to execute.</param>
    /// <param name="timeout">The maximum time to wait before throwing an exception.</param>
    /// <param name="retryInterval">The interval between retries (default: 100ms).</param>
    /// <param name="shouldRetryOn">A predicate to determine whether to retry on a given exception.</param>
    /// <param name="onRetry">An action to execute on each retry attempt (e.g., logging).</param>
    /// <returns>The result of the successful action.</returns>
    public static T For<T>(
        Func<T> action,
        TimeSpan timeout,
        TimeSpan? retryInterval = null,
        Func<Exception, bool> shouldRetryOn = null,
        Action<int, Exception> onRetry = null)
    {
        var startTime = DateTime.UtcNow;
        var interval = retryInterval ?? TimeSpan.FromMilliseconds(100);
        var attempt = 0;
        Exception lastException = null;

        while (DateTime.UtcNow - startTime < timeout)
        {
            try
            {
                // Attempt to execute the action
                return action();
            }
            catch (Exception ex)
            {
                attempt++;
                lastException = ex;

                // Check if we should retry on this exception
                if (shouldRetryOn != null && !shouldRetryOn(ex))
                {
                    throw;
                }

                // Log or handle the retry attempt
                onRetry?.Invoke(attempt, ex);

                // Wait before retrying
                Thread.Sleep(interval);
            }
        }

        throw new TimeoutException($"The operation timed out after {timeout.TotalSeconds} seconds.", lastException);
    }
} 
