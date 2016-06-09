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
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms;
using DevExpress.Data.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.Design.Commands;
using DevExpress.XtraReports.Extensions;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Reporting.Templates;
namespace DevExpress.XtraScheduler.Reporting.Design {
   #region _SchedulerReportDesigner
	[CLSCompliant(false),
   ToolboxItemFilter(AttributeSR.SchedulerToolboxItem),
   ToolboxItemFilter(AttributeSR.SchedulerToolboxItemFilter)
   ]
	public class SchedulerReportDesigner_ : _ReportDesigner {
		DesignerVerbCollection verbs;
		public SchedulerReportDesigner_() {
		}
		public new XtraSchedulerReport RootReport { get { return (XtraSchedulerReport)base.RootReport; } }
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			HideMenuCommands();
			ReplaceDragDropService();
		}
		protected override void OnResetToolbox() {
			base.OnResetToolbox();
			string category = AssemblyInfo.DXTabSchedulerReporting;
			IToolboxService toolboxSvc = GetService(typeof(IToolboxService)) as IToolboxService;
			ToolboxItemCollection items = toolboxSvc.GetToolboxItems(category);
			foreach (ToolboxItem item in items)
				toolboxSvc.RemoveToolboxItem(item);
			ToolboxItem[] schedulerReportingToolboxItems = SchedulerReportToolboxService.SchedulerReportingToolboxItems;
			foreach (ToolboxItem toolboxItem in schedulerReportingToolboxItems)
				toolboxSvc.AddToolboxItem(toolboxItem, category);
		}
		protected override bool CanAddToolboxItem(ToolboxItem toolboxItem) {
			Type[] xrControlTypes = SchedulerReportToolboxService.FilteredXRControlTypes; 
			for (int i = 0; i < xrControlTypes.Length; i++) {
				if (xrControlTypes[i].FullName == toolboxItem.TypeName)
					return false;
			}
			return base.CanAddToolboxItem(toolboxItem);
		}
		protected virtual void ReplaceDragDropService() {
			IDragDropService service = (IDragDropService)fDesignerHost.GetService(typeof(IDragDropService));
			if (service != null) {
				fDesignerHost.RemoveService(typeof(IDragDropService));
			}
			fDesignerHost.AddService(typeof(IDragDropService), new SchedulerReportDragDropService(fDesignerHost));
		}
		public override DesignerVerbCollection Verbs {
			get {
				if (verbs == null) {
					verbs = base.Verbs;
					DesignerVerbCollection validBaseVerbs = GetValidBaseVerbs(verbs);
					verbs.Clear();
					verbs.AddRange(validBaseVerbs);
					verbs.Insert(0, new DesignerVerb("Load Report Template...", new EventHandler(OnLoadReportTemplate)));
					verbs.AddRange(new DesignerVerb[] {
						new DesignerVerb(DevExpress.XtraReports.Design.DesignSR.Verb_Import, new EventHandler(OnImport)),
						new XRDesignerVerb(DevExpress.XtraReports.Design.DesignSR.Verb_Save, new EventHandler(OnSave), ReportCommand.None),
						new DesignerVerb(DevExpress.XtraReports.Design.DesignSR.Verb_About, new EventHandler(OnReportAbout))});
					DevExpress.Utils.Design.DXSmartTagsHelper.CreateDefaultVerbs(this, verbs);
				}
				return verbs;
			}
		}
		private DesignerVerbCollection GetValidBaseVerbs(DesignerVerbCollection verbs) {
			DesignerVerbCollection result = new DesignerVerbCollection();
			for (int i = 0; i < verbs.Count; i++) {
				DesignerVerb item = verbs[i];
				if (item.Text == "Reset Toolbox") {
					result.Add(item);
					continue;
				}
				XRDesignerVerb xrVerb = item as XRDesignerVerb;
				if(xrVerb != null && xrVerb.CommandID == CommandIDReportCommandConverter.GetCommandID(ReportCommand.VerbEditBands)) {
					result.Add(verbs[i]);
				}
			}
			return result;
		}
		private void OnImport(object sender, EventArgs e) {
			string fileName = RootReport.GetType().Name;
			using (OpenFileDialog fileDialog = new OpenFileDialog()) {
				InitFileDialog(fileDialog, fileName);
				if (fileDialog.ShowDialog() == DialogResult.OK)
					ImportFromRepx(fileDialog.FileName);
			}
		}
		protected void ImportFromRepx(string fileName) {
			DevExpress.XtraReports.Import.RepxConverter converter = new DevExpress.XtraReports.Import.RepxConverter();
			try {
				DevExpress.XtraPrinting.Native.CursorStorage.SetCursor(Cursors.WaitCursor);
				converter.Convert(fileName, RootReport);
			}
			catch (Exception e) {
				XtraMessageBox.Show(e.Message);
			}
			finally {
				DevExpress.XtraPrinting.Native.CursorStorage.RestoreCursor();
			}
		}
		protected void InitFileDialog(FileDialog fileDialog, string fileName) {
			fileDialog.Filter = GetFileDialogFilter();
			try {
				if (fileName != null && fileName.Length > 0) {
					string s = Path.GetDirectoryName(fileName);
					if (s.Length > 0)
						fileDialog.InitialDirectory = s;
				}
			}
			catch { }
		}
		protected internal string GetFileDialogFilter() {
			string defaultExt = XtraSchedulerReport.DefaultReportTemplateExt;
			return String.Format("Scheduler Report Files (*{0})|*{1}|" + "All Files (*.*)|*.*", defaultExt, defaultExt);
		}
		private void OnReportAbout(object sender, EventArgs e) {
			try {
				SchedulerControl.About();
			}
			catch { }
		}
		private void OnSave(object sender, EventArgs e) {
			try {
				SaveFileDialog fileDialog = DevExpress.XtraReports.UserDesigner.XRDesignPanel.CreateSaveFileDialog(RootReport, "");
				fileDialog.Filter = GetFileDialogFilter();
				if (fileDialog.ShowDialog() == DialogResult.OK)
					RootReport.SaveLayout(fileDialog.FileName);
				fileDialog.Dispose();
			}
			catch {
			}
		}
		void OnLoadReportTemplate(object sender, EventArgs e) {
			LoadReportTemplate();
		}
		protected override ITemplateProvider GetTemplateProvider() {
			return SchedulerDXTemplateProvider.CreateInstance();
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
		protected override IToolShell GetReportTool(IServiceProvider srvProvider) {
			IToolShell reportToolShell = new ToolShell();
			reportToolShell.AddToolItem(new ReportExplorerToolItem(srvProvider, "Report Explorer"));
			reportToolShell.AddToolItem(new ReportMenu(srvProvider));
			return reportToolShell;
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
		protected override string GetToolboxSelectedCategory() {
			return string.Format(SchedulerLocalizer.GetString(SchedulerStringId.VS_SchedulerReportsToolboxCategoryName), AssemblyInfo.VersionShort);
		}
		protected override void OnComponentAdded(object source, ComponentEventArgs e) {
			base.OnComponentAdded(source, e);
			SchedulerPrintAdapter adapter = e.Component as SchedulerPrintAdapter;
			if (adapter != null && RootReport.SchedulerAdapter == null) {
				RootReport.SchedulerAdapter = adapter;
			}
		}
		public override bool GetToolSupported(ToolboxItem tool) {
			if (tool == null)
				return base.GetToolSupported(tool);
			return IsSchedulerReportToolboxItems(tool.TypeName, tool.Version) || base.GetToolSupported(tool);
		}
		protected bool IsSchedulerReportToolboxItems(string typeName, string version) {
			return (typeName.StartsWith(AttributeSR.SchedulerToolboxItem) && version == AssemblyInfo.Version);
		}
	}
	#endregion
}
