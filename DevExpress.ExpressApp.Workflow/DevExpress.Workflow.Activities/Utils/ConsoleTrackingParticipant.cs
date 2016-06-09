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
using System.Activities.Tracking;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
namespace DevExpress.Workflow.Utils {
	public class ConsoleTrackingParticipant : TrackingParticipantBase {
		private const String participantName = "ConsoleTrackingParticipant";
		protected override void Track(TrackingRecord record, TimeSpan timeout) {
			base.Track(record, timeout);
			Console.WriteLine(String.Format(CultureInfo.InvariantCulture, "{0} emitted trackRecord: {1}  Level: {2}, RecordNumber: {3}", participantName, record.GetType().FullName, record.Level, record.RecordNumber));
			WorkflowInstanceRecord workflowInstanceRecord = record as WorkflowInstanceRecord;
			if (workflowInstanceRecord != null) { 
				Console.WriteLine(TrackingParticipantHelper.GetInfo(workflowInstanceRecord));
			}
			ActivityStateRecord activityStateRecord = record as ActivityStateRecord;
			if (activityStateRecord != null) {
				Console.WriteLine(TrackingParticipantHelper.GetInfo(activityStateRecord));
			}
			CustomTrackingRecord customTrackingRecord = record as CustomTrackingRecord;
			if ((customTrackingRecord != null) && (customTrackingRecord.Data.Count > 0)) {
				Console.WriteLine(TrackingParticipantHelper.GetInfo(customTrackingRecord));
			}
			FaultPropagationRecord faultPropagationRecord = record as FaultPropagationRecord;
			if(faultPropagationRecord != null) {
				Console.WriteLine(TrackingParticipantHelper.GetInfo(faultPropagationRecord));
			}
			Console.WriteLine();
		}
	}
}
