using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;

namespace EmbeddedDbDotNetTest
{
	class TestTimer
	{
		private readonly long pFrequency;
		private long pStartTime;
		private long pStopTime;
		private double pTotalTime = 0;

		public TestTimer(string name)
		{
			Name = name;
            if(QueryPerformanceFrequency(out pFrequency) == false)
            {
                // high-performance counter not supported
                throw new Win32Exception();
            }
		}

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);

        public void Start()
        {
            // lets do the waiting threads there work
            Thread.Sleep(0);
			QueryPerformanceCounter(out pStartTime);
        }

        public double Stop()
        {
			QueryPerformanceCounter(out pStopTime);
        	double time = ((double) (pStopTime - pStartTime))/pFrequency;
			pTotalTime += time;
			return time;
        }

		public string Name
		{
			get; private set;
		}

		public long TotalTimeMs
        {
            get
            {
				return (long)(pTotalTime*1000);
            }
        }
    }
}
