#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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
using System.Threading;
using DevExpress.XtraReports.Web.Native.ClientControls.Services;
namespace DevExpress.XtraReports.Web.WebDocumentViewer.Native.Services {
	public sealed class ThreadingTimerStoragesCleaner : IStoragesCleaner, IDisposable {
		static ILoggingService Logger {
			get { return DefaultLoggingService.Instance; }
		}
		readonly object syncRoot = new object();
		readonly StoragesCleanerSettings settings;
		readonly IReportManagementService reportManagementService;
		readonly IDocumentStorage documentStorage;
		bool started;
		Timer timer;
		public ThreadingTimerStoragesCleaner(StoragesCleanerSettings settings, IReportManagementService reportManagementService, IDocumentStorage documentStorage) {
			this.settings = settings;
			this.reportManagementService = reportManagementService;
			this.documentStorage = documentStorage;
		}
		public void SafeStart() {
			if(!started) {
				lock(syncRoot) {
					if(!started) {
						StartCore();
						started = true;
					}
				}
			}
		}
		void StartCore() {
			timer = new Timer(_ => Clean(), null, settings.DueTime, settings.Period);
		}
		void Clean() {
			CleanCore("Documents", documentStorage.Clean);
			CleanCore("Reports", reportManagementService.Clean);
		}
		void CleanCore(string containerName, Action<TimeSpan> cleanMethod) {
			try {
				cleanMethod(settings.EntityTimeToLife);
			} catch(Exception e) {
				Logger.Error(string.Format("Cleaner - Cannot clean the {0} container: {1}", containerName, e));
			}
		}
		public void Dispose() {
			timer.Dispose();
		}
	}
}
