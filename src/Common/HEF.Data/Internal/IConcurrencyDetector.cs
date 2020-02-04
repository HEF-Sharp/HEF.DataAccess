using System;

namespace HEF.Data
{
    public interface IConcurrencyDetector
    {
        /// <summary>
        ///     Call to enter the critical section.
        /// </summary>
        /// <returns> A disposer that will exit the critical section when disposed. </returns>
        IDisposable EnterCriticalSection();
    }
}
