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
using System.ComponentModel;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.UserDesigner;
using System.ComponentModel.Design;
using System.Windows.Forms;
using DevExpress.XtraReports.UI;
using System.Drawing.Design;
using DevExpress.XtraReports.Design.Commands;
using DevExpress.XtraScheduler.Native;
namespace DevExpress.XtraScheduler.Reporting.Design {
	#region DesignSR
	public static class DesignSR {
		public const string View = "View";
		public const string HorizontalHeaders = "HorizontalHeaders";
		public const string VerticalHeaders = "VerticalHeaders";
		public const string TimeCells = "TimeCells";
		public const string VerticalLayoutType = "VerticalLayoutType";
		public const string TimeScale = "TimeScale";
		public const string VisibleTimeStart = "VisibleTimeStart";
		public const string VisibleTimeEnd = "VisibleTimeEnd";
		public const string ShowWorkTimeOnly = "ShowWorkTimeOnly";
		public const string CompressWeekend = "CompressWeekend";
		public const string VisibleWeekDays = "VisibleWeekDays";
		public const string PrintInColumn = "PrintInColumn";
		public const string PrintContentMode = "PrintContentMode";
		public const string FormatType = "FormatType";
		public const string AutoFormat = "AutoFormat";
		public const string FormatString = "FormatString";
		public const string TopCornerIndent = "TopCornerIndent";
		public const string CalculatedFields = "CalculatedFields";
		public const string DataAdapter = "DataAdapter";
		public const string DataSource = "DataSource";
		public const string DataMember = "DataMember";
		public const string FilterString = "FilterString";
		public const string XmlDataPath = "XmlDataPath";
		public const string DataSourceSchema = "DataSourceSchema";
	}
	#endregion	
	#region SchedulerReportDesigner
	[ToolboxItemFilter(DevExpress.XtraReports.Design.AttributeSR.SchedulerToolboxItem)]
	public class SchedulerReportDesigner : ReportDesigner {
		public SchedulerReportDesigner() {
		}
		public new XtraSchedulerReport RootReport { get { return (XtraSchedulerReport)base.RootReport; } }
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			ReplaceToolboxService();
			AddReportTypeService();
			HideMenuCommands();
			SubscribeHostEvents();
			ReplaceDragDropService();
		}
		protected override void ActivateMenuCommands() {
			ReplaceMenuCommandHandler();
			base.ActivateMenuCommands();
		}
		protected virtual void AddReportTypeService() {
			fDesignerHost.AddService(typeof(ReportTypeService), new SchedulerReportTypeService());
		}
		protected virtual void ReplaceDragDropService() {
			IDragDropService service = (IDragDropService)fDesignerHost.GetService(typeof(IDragDropService));
			if (service != null)
				fDesignerHost.RemoveService(typeof(IDragDropService));
			fDesignerHost.AddService(typeof(IDragDropService), new SchedulerReportDragDropService(fDesignerHost));
		}
		protected virtual void SubscribeHostEvents() {
			if (fDesignerHost != null)
				fDesignerHost.LoadComplete += new EventHandler(fDesignerHost_LoadComplete);
		}
		protected virtual void UnsubscribeHostEvents() {
			if (fDesignerHost != null)
				fDesignerHost.LoadComplete -= new EventHandler(fDesignerHost_LoadComplete);
		}
		void fDesignerHost_LoadComplete(object sender, EventArgs e) {
			XRDesignPanel designPanel = (XRDesignPanel)fDesignerHost.GetService(typeof(XRDesignPanel));
			if (designPanel != null) {
				HideDesignerCommands(designPanel);
				ReplaceCommandHandlers(designPanel);
			}
			UnsubscribeHostEvents();
		}
		protected virtual void HideDesignerCommands(XRDesignPanel designPanel) {
			designPanel.SetCommandVisibility(ReportCommand.AddNewDataSource, CommandVisibility.None);
			designPanel.SetCommandVisibility(ReportCommand.InsertDetailReport, CommandVisibility.None);
			designPanel.SetCommandVisibility(ReportCommand.InsertGroupHeaderBand, CommandVisibility.None);
			designPanel.SetCommandVisibility(ReportCommand.InsertGroupFooterBand, CommandVisibility.None);
			designPanel.SetCommandVisibility(ReportCommand.VerbReportWizard, CommandVisibility.None);
		}
		protected virtual void ReplaceCommandHandlers(XRDesignPanel designPanel) {
			designPanel.AddCommandHandler(new NewReportCommandHandler(designPanel));
		}
		protected override string[] GetFilteredProperties() {
			return new string[] { 
				DesignSR.DataSource, 
				DesignSR.DataAdapter, 
				DesignSR.DataMember, 
				DesignSR.CalculatedFields,
				DesignSR.DataSourceSchema, 
				DesignSR.FilterString, 
				DesignSR.XmlDataPath 
			};
		}
		private void ReplaceToolboxService() {
			IToolboxService toolboxService = (IToolboxService)fDesignerHost.GetService(typeof(IToolboxService));
			if (toolboxService != null && fDesignerHost != null) {
				fDesignerHost.RemoveService(typeof(IToolboxService));
				SchedulerReportToolboxService schedulerToolboxService = CreateSchedulerToolboxService();
				fDesignerHost.AddService(typeof(IToolboxService), schedulerToolboxService);
			}
		}
		protected virtual SchedulerReportToolboxService CreateSchedulerToolboxService() {
			return new SchedulerReportToolboxService();
		}
		protected virtual void HideMenuCommands() {
			IMenuCommandService menuCommandService = (IMenuCommandService)fDesignerHost.GetService(typeof(IMenuCommandService));
			if (menuCommandService != null) {
				RemoveMenuCommand(menuCommandService, ReportCommands.InsertDetailReport);
				RemoveMenuCommand(menuCommandService, BandCommands.InsertGroupHeaderBand);
				RemoveMenuCommand(menuCommandService, BandCommands.InsertGroupFooterBand);
			}
		}
		protected void RemoveMenuCommand(IMenuCommandService cmdService, CommandID cmdID) {
			MenuCommand menuCmd = cmdService.FindCommand(cmdID);
			if (menuCmd != null)
				cmdService.RemoveCommand(menuCmd);
		}
		protected override void OnComponentAdded(object source, ComponentEventArgs e) {
			base.OnComponentAdded(source, e);
			SchedulerPrintAdapter adapter = e.Component as SchedulerPrintAdapter;
			if (adapter != null && RootReport.SchedulerAdapter == null) {
				RootReport.SchedulerAdapter = adapter;
			}
		}
		protected virtual void ReplaceMenuCommandHandler() {
			MenuCommandHandler commandHandler = (MenuCommandHandler)fDesignerHost.GetService(typeof(MenuCommandHandler));
			if (commandHandler != null && fDesignerHost != null) {
				fDesignerHost.RemoveService(typeof(MenuCommandHandler));
				SchedulerMenuCommandHandler schedulerHandler = CreateSchedulerMenuCommandHandler();
				fDesignerHost.AddService(typeof(MenuCommandHandler), schedulerHandler);
			}
		}
		protected virtual SchedulerMenuCommandHandler CreateSchedulerMenuCommandHandler() {
			return new SchedulerMenuCommandHandler(fDesignerHost);
		}
	}
	#endregion
	public class SchedulerMenuCommandHandler : MenuCommandHandler {
		public SchedulerMenuCommandHandler(IDesignerHost designerHost) : base(designerHost) {
		}
		protected override bool IsUnsupportedControlSelected(object obj) {
			bool baseResult = base.IsUnsupportedControlSelected(obj);
			if (baseResult) {
				if (obj is ReportViewBase || obj is SchedulerPrintAdapter)
					return false;
			}
			return baseResult;
		}
	}
	public class SchedulerReportTypeService : ReportTypeService {
		public Type GetType(Type reportType) {
			Type validBaseType = typeof(XtraSchedulerReport);
			return validBaseType.IsAssignableFrom(reportType) ? reportType : validBaseType;
		}
		public XtraReport GetDefaultReport() {
			XtraSchedulerReport report = new XtraSchedulerReport();
			DetailBand detail = new DetailBand();
			detail.Name = "Detail";
			report.Bands.Add(detail);
			return report;
		}
	}
	public class SchedulerReportDragDropService : DragDropService {
		public SchedulerReportDragDropService(IDesignerHost designerHost)
			: base(designerHost) {
		}
		protected override ControlDragHandler CreateControlDragHandler() {
			return new SchedulerReportControlDragHandler(DesignerHost);
		}
		protected override TbxItemDragHandler CreateToolboxItemDragHandler() {
			return new SchedulerReportTbxItemDragHandler(DesignerHost);
		}
	}
	public class SchedulerReportControlDragHandler : ControlDragHandler {
		public SchedulerReportControlDragHandler(IDesignerHost host)
			: base(host) {
		}
		protected override DragDropEffects CalculateDragDropEffect(DragDataObject dragData, XRControl parent) {
			bool isDetailBand = parent.Band is DetailBand;
			if (ContainsSchedulerControl(dragData.Controls) && !isDetailBand)
				return DragDropEffects.None;
			return base.CalculateDragDropEffect(dragData, parent);
		}
		internal virtual bool ContainsSchedulerControl(XRControl[] controls) {
			int count = controls.Length;
			for (int i = 0; i < count; i++) {
				if (controls[i] is ReportViewControlBase)
					return true;
			}
			return false;
		}
	}
	public class SchedulerReportTbxItemDragHandler : TbxItemDragHandler {
		public SchedulerReportTbxItemDragHandler(IDesignerHost host)
			: base(host) {
		}
		protected override bool IsContainerDropTargetLocked(XRControl target) {
			bool isDetailBand = target.Band is DetailBand;
			bool isSchedulerControl = this.XRControl is ReportViewControlBase;
			if (isSchedulerControl && !isDetailBand)
				return true;
			return base.IsContainerDropTargetLocked(target);
		}
	}
	#region NewReportCommandHandler
	public class NewReportCommandHandler : ICommandHandler {
		XRDesignPanel designPanel;
		public NewReportCommandHandler(XRDesignPanel designPanel) {
			if (designPanel == null)
				Exceptions.ThrowArgumentNullException("designPanel");
			this.designPanel = designPanel;
		}
		#region ICommandHandler Members
		public bool CanHandleCommand(ReportCommand command, ref bool useNextHandler) {
			if (command == ReportCommand.NewReport) {
				useNextHandler = false;
				return true;
			}
			return false;
		}
		public void HandleCommand(ReportCommand command, object[] args) {
			CreateNewReport();
		}
		protected virtual void CreateNewReport() {
			designPanel.OpenReport(GetDefaultReport());
		}
		protected virtual XtraSchedulerReport GetDefaultReport() {
			XtraSchedulerReport report = new XtraSchedulerReport();
			DetailBand detail = new DetailBand();
			detail.Name = "Detail";
			report.Bands.Add(detail);
			return report;
		}
		#endregion
	}
	#endregion
}
