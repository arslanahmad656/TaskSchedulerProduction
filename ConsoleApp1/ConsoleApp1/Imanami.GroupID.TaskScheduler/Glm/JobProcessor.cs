using Imanami.Data.ServiceClient;
using Imanami.GroupID.DataTransferObjects.DataContracts.Services.Scheduling;
using log4net;
using log4net.Attributes;
using PostSharp.Aspects;
using PostSharp.Aspects.Internals;
using PostSharp.ImplementationDetails_bda91a0d;
using PostSharp.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Imanami.GroupID.TaskScheduler.Glm
{
	public class JobProcessor
	{
		private static ILog logger;

		static JobProcessor()
		{
			<>z__a_e.Initialize();
			Imanami.GroupID.TaskScheduler.Glm.JobProcessor.logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		}

		public JobProcessor()
		{
		}

		public void ProcessJob(long jobId)
		{
			Arguments<long> argument = new Arguments<long>()
			{
				Arg0 = jobId
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(this, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886338863104L),
				Method = <>z__a_e._1
			};
			<>z__a_e.a78.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						this.ProcessJob((new ServicesSchedulingServiceClient(true)).GetScheduledJob(jobId));
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_e.a78.OnException(methodExecutionArg);
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
					<>z__a_e.a78.OnExit(methodExecutionArg);
				}
			}
		}

		public void ProcessJob(TaskScheduling task)
		{
			Arguments<TaskScheduling> argument = new Arguments<TaskScheduling>()
			{
				Arg0 = task
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(this, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886338863103L),
				Method = <>z__a_e._3
			};
			<>z__a_e.a79.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						(new ExpiredGroupsProcessor()).DeleteTheExpiredGroupsWhichAreDueForDeletion();
						ExpiringGroupsProcessor expiringGroupsProcessor = new ExpiringGroupsProcessor();
						expiringGroupsProcessor.ExpireTheGroupsWhichAreDueForExpiry();
						EligibleGroupsForLifeExtensionProcessor eligibleGroupsForLifeExtensionProcessor = new EligibleGroupsForLifeExtensionProcessor();
						eligibleGroupsForLifeExtensionProcessor.ExtendEligibleGroupsLife();
						if (eligibleGroupsForLifeExtensionProcessor.ExtendedGroups.Count > 0)
						{
							expiringGroupsProcessor.ExtendedGroups = eligibleGroupsForLifeExtensionProcessor.ExtendedGroups;
						}
						expiringGroupsProcessor.SendNotificationToExpiringGroups();
						Imanami.GroupID.TaskScheduler.Glm.JobProcessor.logger.InfoFormat("Processed job.", Array.Empty<object>());
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_e.a79.OnException(methodExecutionArg);
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
					<>z__a_e.a79.OnExit(methodExecutionArg);
				}
			}
		}
	}
}