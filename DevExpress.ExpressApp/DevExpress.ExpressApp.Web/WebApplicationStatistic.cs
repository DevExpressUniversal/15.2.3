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
using DevExpress.Persistent.Base;
using System.Text;
using System.Collections.Generic;
using System.Configuration;
using System.ComponentModel;
namespace DevExpress.ExpressApp.Web {
	public class WebApplicationStatistic {
		public const string WebApplicationStatisticForceFullGCCollectKey = "WebApplicationStatisticForceFullGCCollect";
		public const string WebApplicationStatisticLogDetailsKey = "WebApplicationStatisticLogDetails";
		public readonly static List<ApplicationStatisticObject> Applications = new List<ApplicationStatisticObject>();
		[DefaultValue(false)]
		public static bool ForceCallGetTotalMemory = false;
		public static void WriteTraceInfoIfNeed() {
			bool canWriteWebApplicationStatistic = StringHelper.ParseToBool(ConfigurationManager.AppSettings[WebApplicationStatisticLogDetailsKey], false);
			if(!canWriteWebApplicationStatistic) {
				return;
			}
			int finalizedCount = 0;
			int disposedCount = 0;
			int totalCount = 0;
			string appState = "";
			lock(WebApplicationStatistic.Applications) {
				totalCount = Applications.Count;
				StringBuilder builder = new StringBuilder();
				builder.Append("SessionID");
				builder.Append('\t');
				builder.Append("CreatedOn");
				builder.Append('\t');
				builder.Append("LastAccessOn");
				builder.Append('\t');
				builder.Append("DisposedOn");
				builder.Append('\t');
				builder.Append("FinalizedOn");
				builder.Append('\t');
				builder.Append("UserName");
				builder.Append("\r\n");
				foreach(ApplicationStatisticObject statObject in WebApplicationStatistic.Applications) {
					if(statObject.IsFinalized)
						finalizedCount++;
					if(statObject.IsDisposed)
						disposedCount++;
					builder.Append(statObject.SessionId);
					builder.Append('\t');
					builder.Append(statObject.CreatedOn);
					builder.Append('\t');
					builder.Append(statObject.AccessedOn);
					builder.Append('\t');
					builder.Append(statObject.DisposedOn);
					builder.Append('\t');
					builder.Append(statObject.FinalizedOn);
					builder.Append('\t');
					builder.Append(statObject.UserName);
					builder.Append("\r\n");
				}
				appState = builder.ToString();
				Tracing.Tracer.LogValue("TotalApplications", totalCount);
				Tracing.Tracer.LogValue("DisposedApplications", disposedCount);
				Tracing.Tracer.LogValue("FinalizedApplications", finalizedCount);
				bool forceFullGCCollect = StringHelper.ParseToBool(ConfigurationManager.AppSettings[WebApplicationStatisticForceFullGCCollectKey], false);
				if(ForceCallGetTotalMemory) {
					GC.GetTotalMemory(true);
					GC.GetTotalMemory(true);
					Tracing.Tracer.LogValue("GC.GetTotalMemory(" + forceFullGCCollect + ") (Mb)", GC.GetTotalMemory(true) / (1024 * 1024));
				}
				if(StringHelper.ParseToBool(ConfigurationManager.AppSettings[WebApplicationStatisticLogDetailsKey], false)) {
					Tracing.Tracer.LogValue("Details", "\n" + appState);
				}
			}
		}
	}
	public class ApplicationStatisticObject {
		protected DateTime createdOn = DateTime.Now;
		private DateTime accessedOn = DateTime.MinValue;
		private DateTime disposedOn = DateTime.MinValue;
		private DateTime finalizedOn = DateTime.MinValue;
		private string sessionId = "N/A";
		private string userName = "N/A";
		public ApplicationStatisticObject() { }
		public void UpdateAccessedOn() {
			accessedOn = DateTime.Now;
		}
		public void SetSessionId(string sessionId) {
			this.sessionId = sessionId;
		}
		public void SetFinalized() {
			finalizedOn = DateTime.Now;
		}
		public void SetDisposed() {
			disposedOn = DateTime.Now;
		}
		public DateTime CreatedOn {
			get { return createdOn; }
		}
		public DateTime AccessedOn {
			get { return accessedOn; }
		}
		public bool IsDisposed {
			get { return DisposedOn != DateTime.MinValue; }
		}
		public DateTime DisposedOn {
			get { return disposedOn; }
		}
		public DateTime FinalizedOn {
			get { return finalizedOn; }
		}
		public bool IsFinalized {
			get { return FinalizedOn != DateTime.MinValue; }
		}
		public string SessionId {
			get { return sessionId; }
		}
		public string UserName {
			get { return userName; }
			set { userName = value; }
		}
	}
}
