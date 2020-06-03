using Imanami.GroupID.DataTransferObjects.DataContracts;
using System;
using System.Runtime.CompilerServices;

namespace Imanami.GroupID.TaskScheduler
{
	internal static class ActionResultHelper
	{
		public static string ActionResultToString(this ActionResult result)
		{
			return string.Format("Status: {0}, Message: {1}, Name: {2}, Data: {3}, Type: {4}, Result: {5}", new object[] { result.get_Status(), result.get_Message(), result.get_Name(), result.get_Data(), result.get_Type(), result.get_IdentityStoreObject() });
		}
	}
}