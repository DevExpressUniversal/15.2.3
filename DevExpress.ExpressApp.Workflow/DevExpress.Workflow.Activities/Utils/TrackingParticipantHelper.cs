#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities.Tracking;
using System.Globalization;
namespace DevExpress.Workflow.Utils {
	public static class TrackingParticipantHelper {
		public static string GetInfo(WorkflowInstanceRecord workflowInstanceRecord) {
			return String.Format(CultureInfo.InvariantCulture, "Workflow instance state: {0}\r\n", workflowInstanceRecord.State);
		}
		public static string GetInfo(ActivityStateRecord activityStateRecord) {
			IDictionary<String, object> arguments = activityStateRecord.Arguments;
			StringBuilder args = new StringBuilder();
			if(arguments.Count > 0) {
				args.AppendLine("\r\n\tArguments:");
				foreach(KeyValuePair<string, object> argument in arguments) {
					args.AppendLine(String.Format("\r\n\t\tName: {0} Value: {1}", argument.Key, argument.Value));
				}
			}
			IDictionary<String, object> variables = activityStateRecord.Variables;
			StringBuilder vars = new StringBuilder();
			if(variables.Count > 0) {
				vars.AppendLine("\r\n\tVariables:");
				foreach(KeyValuePair<string, object> variable in variables) {
					vars.AppendLine(String.Format("\r\n\t\tName: {0} Value: {1}", variable.Key, variable.Value));
				}
			}
			return String.Format(CultureInfo.InvariantCulture, "Activity DisplayName: {0} : ActivityInstanceState: {1}{2}{3}\r\n", activityStateRecord.Activity.Name, activityStateRecord.State, ((arguments.Count > 0) ? args.ToString() : String.Empty), ((variables.Count > 0) ? vars.ToString() : String.Empty));
		}
		public static string GetInfo(CustomTrackingRecord customTrackingRecord) {
			StringBuilder userData = new StringBuilder();
			foreach(string data in customTrackingRecord.Data.Keys) {
				userData.AppendLine(String.Format(CultureInfo.InvariantCulture, "\r\n\t\t {0} : {1}", data, customTrackingRecord.Data[data]));
			}
			return String.Format(CultureInfo.InvariantCulture, "\tUser Data: {0}\r\n", ((customTrackingRecord.Data.Count > 0) ? userData.ToString() : String.Empty));
		}
		public static string GetInfo(FaultPropagationRecord faultPropagationRecord) {
			StringBuilder faultData = new StringBuilder();
			faultData.AppendLine(String.Format(CultureInfo.InvariantCulture, "\r\nFault info:"));
			if(faultPropagationRecord.FaultSource != null) {
				faultData.AppendLine(String.Format(CultureInfo.InvariantCulture, "\r\nActivity: {0}", faultPropagationRecord.FaultSource.Name));
			}
			if(faultPropagationRecord.Fault != null) {
				faultData.AppendLine(String.Format(CultureInfo.InvariantCulture, "\r\nException Message: {0}", faultPropagationRecord.Fault.Message));
				faultData.AppendLine(String.Format(CultureInfo.InvariantCulture, "\r\nStack Trace: {0}", faultPropagationRecord.Fault.StackTrace));
			}
			if(faultPropagationRecord.Annotations.Count > 0) {
				faultData.AppendLine(String.Format(CultureInfo.InvariantCulture, "\r\nAnnotations:"));
				foreach(string data in faultPropagationRecord.Annotations.Keys) {
					faultData.AppendLine(String.Format(CultureInfo.InvariantCulture, "\r\n\t {0} : {1}", data, faultPropagationRecord.Annotations[data]));
				}
			}
			return String.Format(CultureInfo.InvariantCulture, "{0}\r\n", faultData.ToString());
		}
	}
}
