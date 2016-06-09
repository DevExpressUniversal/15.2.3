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
using DevExpress.Utils;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraScheduler.Reporting.UserDesigner;
namespace DevExpress.XtraScheduler.Reporting {
	using DevExpress.LookAndFeel;
	using System.ComponentModel.Design;
	using DevExpress.XtraScheduler.Reporting.UI;
	public static class XtraSchedulerReportExtensions {
		static IReportDesignTool GetDesignTool(XtraSchedulerReport report) {
			IReportDesignTool tool = ((IServiceProvider)report).GetService(typeof(IReportDesignTool)) as IReportDesignTool;
			if(tool == null) {
				tool = new SchedulerReportDesignTool(report);
				IServiceContainer serv = ((IServiceProvider)report).GetService(typeof(IServiceContainer)) as IServiceContainer;
				serv.AddService(typeof(IReportDesignTool), tool);
			}
			return tool;
		}
		public static void ShowDesigner(this XtraSchedulerReport report) {
			GetDesignTool(report).ShowDesigner();
		}
		public static void ShowDesignerDialog(this XtraSchedulerReport report) {
			GetDesignTool(report).ShowDesignerDialog();
		}
		public static void ShowDesigner(this XtraSchedulerReport report, UserLookAndFeel lookAndFeel) {
			GetDesignTool(report).ShowDesigner(lookAndFeel);
		}
		public static void ShowDesignerDialog(this XtraSchedulerReport report, UserLookAndFeel lookAndFeel) {
			GetDesignTool(report).ShowDesignerDialog(lookAndFeel);
		}
		public static void ShowDesigner(this XtraSchedulerReport report, UserLookAndFeel lookAndFeel, DesignDockPanelType hiddenPanels) {
			GetDesignTool(report).ShowDesigner(lookAndFeel, hiddenPanels);
		}
		public static void ShowDesignerDialog(this XtraSchedulerReport report, UserLookAndFeel lookAndFeel, DesignDockPanelType hiddenPanels) {
			GetDesignTool(report).ShowDesignerDialog(lookAndFeel, hiddenPanels);
		}
		public static void ShowRibbonDesigner(this XtraSchedulerReport report) {
			GetDesignTool(report).ShowRibbonDesigner();
		}
		public static void ShowRibbonDesignerDialog(this XtraSchedulerReport report) {
			GetDesignTool(report).ShowRibbonDesignerDialog();
		}
		public static void ShowRibbonDesigner(this XtraSchedulerReport report, UserLookAndFeel lookAndFeel) {
			GetDesignTool(report).ShowRibbonDesigner(lookAndFeel);
		}
		public static void ShowRibbonDesignerDialog(this XtraSchedulerReport report, UserLookAndFeel lookAndFeel) {
			GetDesignTool(report).ShowRibbonDesignerDialog(lookAndFeel);
		}
		public static void ShowRibbonDesigner(this XtraSchedulerReport report, UserLookAndFeel lookAndFeel, DesignDockPanelType hiddenPanels) {
			GetDesignTool(report).ShowRibbonDesigner(lookAndFeel, hiddenPanels);
		}
		public static void ShowRibbonDesignerDialog(this XtraSchedulerReport report, UserLookAndFeel lookAndFeel, DesignDockPanelType hiddenPanels) {
			GetDesignTool(report).ShowRibbonDesignerDialog(lookAndFeel, hiddenPanels);
		}
	}
}
namespace DevExpress.XtraScheduler.Reporting.UI {
	public class SchedulerReportDesignTool : ReportDesignTool {
		public SchedulerReportDesignTool(DevExpress.XtraScheduler.Reporting.XtraSchedulerReport report)
			: base(report) {
			Guard.ArgumentNotNull(report, "report");
		}
		protected override IDesignForm CreateDesignForm() {
			return new ReportDesignForm();
		}
	}
}
