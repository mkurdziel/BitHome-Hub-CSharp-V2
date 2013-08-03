using System;

namespace BitHome
{
	public interface IBitHomeService
	{
		bool Start();
		void Stop();
		void WaitFinishSaving();
	}
}

