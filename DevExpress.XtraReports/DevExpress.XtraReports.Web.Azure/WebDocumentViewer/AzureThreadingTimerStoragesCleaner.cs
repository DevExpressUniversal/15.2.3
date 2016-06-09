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
using DevExpress.XtraReports.Web.WebDocumentViewer.Native.Services;
namespace DevExpress.XtraReports.Web.Azure.WebDocumentViewer {
	public class AzureThreadingTimerStoragesCleaner : IStoragesCleaner {
		static ILoggingService Logger {
			get { return DefaultLoggingService.Instance; }
		}
		readonly object syncRoot = new object();
		readonly StoragesCleanerSettings settings;
		readonly IAzureEntityStorageManager entityStorageManager;
		readonly IReportManagementService reportManagenemtService;
		bool started;
		Timer timer;
		public AzureThreadingTimerStoragesCleaner(StoragesCleanerSettings settings, IAzureEntityStorageManager entityStorageManager, IReportManagementService reportManagenemtService) {
			this.entityStorageManager = entityStorageManager;
			this.settings = settings;
			this.reportManagenemtService = reportManagenemtService;
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
			CleanReportsLocaly();
			CleanRemoteStorage();
		}
		void CleanReportsLocaly() {
			try {
				reportManagenemtService.Clean(settings.EntityTimeToLife);
			} catch(Exception e) {
				LogCleanException(e);
			}
		}
		void CleanRemoteStorage() {
			try {
				entityStorageManager.Clean(settings.EntityTimeToLife);
			} catch(Exception e) {
				LogCleanException(e);
			}
		}
		void LogCleanException(Exception e) {
			Logger.Error("Cleaner - Unhandled exception occurred during a clean operation: " + e);
		}
	}
}
