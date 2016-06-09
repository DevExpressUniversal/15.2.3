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
using System.Activities;
using System.ComponentModel;
namespace DevExpress.Workflow.Utils {
	public class TrackingEventArgs : EventArgs {
		public TrackingEventArgs(TrackingRecord trackingRecord, TimeSpan timeout) {
			this.Record = trackingRecord;
			this.Timeout = timeout;
		}
		public TrackingRecord Record { get; set; }
		public TimeSpan Timeout { get; set; }
	}
	public class TrackingParticipantBase : TrackingParticipant {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use the 'UserCancelledExceptionMessage' constant instead.")]
		public const string UserCancelledExecptionMessage = "User cancelled.";
		public const string UserCancelledExceptionMessage = "User cancelled.";
		protected override void Track(TrackingRecord record, TimeSpan timeout) {
			try {
				RaiseOnTrackReceived(record, timeout);
			}
			catch(Exception e) {
				if(e.Message == UserCancelledExceptionMessage) {
					throw;
				}
				else {
					DevExpress.Persistent.Base.Tracing.Tracer.LogError(e);
				}
			}
		}
		protected void RaiseOnTrackReceived(TrackingRecord record, TimeSpan timeout) {
#if DebugTest
			System.Diagnostics.Debug.WriteLine(String.Format("Tracking Record Received: {0} with timeout: {1} seconds.", record, timeout.TotalSeconds));
#endif
			if(TrackReceived != null) {
				TrackReceived(this, new TrackingEventArgs(record, timeout));
			}
		}
		public event EventHandler<TrackingEventArgs> TrackReceived;
	}
}
