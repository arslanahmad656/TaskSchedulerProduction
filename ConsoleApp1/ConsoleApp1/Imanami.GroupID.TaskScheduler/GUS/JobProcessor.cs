using log4net;
using System;
using System.Reflection;

namespace Imanami.GroupID.TaskScheduler.GUS
{
	public class JobProcessor
	{
		private static ILog logger;

		static JobProcessor()
		{
			Imanami.GroupID.TaskScheduler.GUS.JobProcessor.logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		}

		public JobProcessor()
		{
		}

		public void ProcessGroupUsage()
		{
			(new GroupsProcessor()).ProcessGroupUsage();
		}
	}
}