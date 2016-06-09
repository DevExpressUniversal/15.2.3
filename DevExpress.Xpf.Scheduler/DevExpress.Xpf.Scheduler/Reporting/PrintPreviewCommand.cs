#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
using System.Windows.Input;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Reporting;
using DevExpress.Xpf.Printing;
using System.Windows;
using DevExpress.Utils;
#if !SL
using DevExpress.Xpf.Scheduler.Reporting.Native;
#endif
namespace DevExpress.Xpf.Scheduler.Reporting {
#if !SL
	public class SchedulerReportConfigurator {
		public virtual void Configure(ISchedulerPrintingSettings settings) {
			if (settings == null)
				return;
			ISchedulerReport report = settings.ReportInstance;
			if (report == null)
				return;
			string templatePath = settings.GetReportTemplatePath();
			if (!String.IsNullOrEmpty(templatePath))
				report.LoadLayout(templatePath);
			SchedulerPrintAdapter adapter = GetSchedulerAdapter(settings.SchedulerPrintAdapter);
			if (adapter != null)
				report.SchedulerAdapter = adapter;
			report.PrintingSystemBase.ClearContent();
		}
		protected SchedulerPrintAdapter GetSchedulerAdapter(DXSchedulerPrintAdapter adapter) {
			return adapter != null ? adapter.SchedulerAdapter : null;
		}
	}
#else
	public class ShowPrintPreviewEventArgs : EventArgs {
		public ShowPrintPreviewEventArgs(ISchedulerPrintingSettings printingSettings) {
			PrintingSettings = printingSettings;
		}
		public ISchedulerPrintingSettings PrintingSettings { get; private set; }
	}	
#endif
}
