using Imanami.Data.ServiceClient;
using Imanami.GroupID.DataTransferObjects.DataContracts;
using Imanami.GroupID.DataTransferObjects.DataContracts.Services.Scheduling;
using log4net;
using log4net.Attributes;
using PostSharp.Aspects;
using PostSharp.Aspects.Internals;
using PostSharp.ImplementationDetails_bda91a0d;
using PostSharp.Reflection;
using System;
using System.Reflection;

namespace Imanami.GroupID.TaskScheduler
{
	public class WorkflowApproverAccelerationProcessor
	{
		private static ILog logger;

		static WorkflowApproverAccelerationProcessor()
		{
			<>z__a_7.Initialize();
			WorkflowApproverAccelerationProcessor.logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		}

		public WorkflowApproverAccelerationProcessor()
		{
		}

		public void ProcessJob(TaskScheduling task)
		{
			Arguments<TaskScheduling> argument = new Arguments<TaskScheduling>()
			{
				Arg0 = task
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(this, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886339321856L),
				Method = <>z__a_7._1
			};
			<>z__a_7.a38.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						try
						{
							ActionResult actionResult = (new WorkflowManagerServiceClient(false)).ProcessApproverAcceleration(task.get_IdentityStoreId());
							WorkflowApproverAccelerationProcessor.logger.Debug(1023, 10334, string.Format("Workflow approver acceleration result: {0}. {1}", actionResult.get_Status(), actionResult.get_Message()), null, null);
						}
						catch (Exception exception1)
						{
							Exception exception = exception1;
							string str = string.Concat("Unable to accelerate the workflow approvers. ", exception.Message);
							WorkflowApproverAccelerationProcessor.logger.Error(1023, 10334, str, exception, null);
						}
					}
					catch (Exception exception2)
					{
						methodExecutionArg.Exception = exception2;
						<>z__a_7.a38.OnException(methodExecutionArg);
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
					<>z__a_7.a38.OnExit(methodExecutionArg);
				}
			}
		}
	}
}