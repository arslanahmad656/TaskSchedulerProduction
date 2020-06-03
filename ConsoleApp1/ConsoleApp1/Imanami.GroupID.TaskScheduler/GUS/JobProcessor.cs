using log4net;
using log4net.Attributes;
using PostSharp.Aspects;
using PostSharp.ImplementationDetails_bda91a0d;
using PostSharp.Reflection;
using System;
using System.Reflection;

namespace Imanami.GroupID.TaskScheduler.GUS
{
	public class JobProcessor
	{
		private static ILog logger;

		static JobProcessor()
		{
			<>z__a_9.Initialize();
			Imanami.GroupID.TaskScheduler.GUS.JobProcessor.logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		}

		public JobProcessor()
		{
		}

		public void ProcessGroupUsage()
		{
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(this, Arguments.Empty)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886339190784L),
				Method = <>z__a_9._1
			};
			<>z__a_9.a41.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						(new GroupsProcessor()).ProcessGroupUsage();
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_9.a41.OnException(methodExecutionArg);
						switch (methodExecutionArg.FlowBehavior)
						{
							case FlowBehavior.Continue:
							{
								methodExecutionArg.Exception = null;
								break;
							}
							case FlowBehavior.Return:
							{
								methodExecutionArg.Exception = null;
								break;
							}
							case FlowBehavior.ThrowException:
							{
								throw methodExecutionArg.Exception;
							}
							default:
							{
								throw;
							}
						}
					}
				}
				finally
				{
					<>z__a_9.a41.OnExit(methodExecutionArg);
				}
			}
		}
	}
}