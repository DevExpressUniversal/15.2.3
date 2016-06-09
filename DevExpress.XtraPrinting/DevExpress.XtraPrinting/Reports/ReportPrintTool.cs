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
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraPrinting.Preview;
using DevExpress.XtraPrinting.Control;
using DevExpress.LookAndFeel;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.Parameters;
using System.Windows.Forms;
using DevExpress.XtraReports.Native;
using DevExpress.XtraPrinting.Native;
using System.Drawing;
using DevExpress.XtraEditors;
using System.ComponentModel;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors.Repository;
using DevExpress.Data.Helpers;
using DevExpress.Data.Browsing;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Native.DrillDown;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraPrinting.Reports.Native;
namespace DevExpress.XtraReports.UI {
	public static class XtraReportExtensions {
		static IReportPrintTool GetPrintTool(IReport report) {
			if(report.PrintTool == null)
				report.AssignPrintTool(new ReportPrintTool(report));
			return report.PrintTool;
		}
		public static void AssignPrintTool(this IReport report, ReportPrintTool printTool) {
			report.PrintTool = printTool;
		}
		public static void ShowPreview(this IReport report) {
			GetPrintTool(report).ShowPreview();
		}
		public static void ShowPreviewDialog(this IReport report) {
			GetPrintTool(report).ShowPreviewDialog();
		}
		public static void ShowPreview(this IReport report, UserLookAndFeel lookAndFeel) {
			GetPrintTool(report).ShowPreview(lookAndFeel);
		}
		public static void ShowPreviewDialog(this IReport report, UserLookAndFeel lookAndFeel) {
			GetPrintTool(report).ShowPreviewDialog(lookAndFeel);
		}
		public static void ClosePreview(this IReport report) {
			GetPrintTool(report).ClosePreview();
		}
		public static void ShowRibbonPreview(this IReport report) {
			GetPrintTool(report).ShowRibbonPreview();
		}
		public static void ShowRibbonPreviewDialog(this IReport report) {
			GetPrintTool(report).ShowRibbonPreviewDialog();
		}
		public static void ShowRibbonPreview(this IReport report, UserLookAndFeel lookAndFeel) {
			GetPrintTool(report).ShowRibbonPreview(lookAndFeel);
		}
		public static void ShowRibbonPreviewDialog(this IReport report, UserLookAndFeel lookAndFeel) {
			GetPrintTool(report).ShowRibbonPreviewDialog(lookAndFeel);
		}
		public static void CloseRibbonPreview(this IReport report) {
			GetPrintTool(report).CloseRibbonPreview();
		}
		public static void Print(this IReport report) {
			GetPrintTool(report).Print();
		}
		public static void Print(this IReport report, string printerName) {
			GetPrintTool(report).Print(printerName);
		}
		public static DialogResult PrintDialog(this IReport report) {
			return GetPrintTool(report).PrintDialog() == true ? DialogResult.OK : DialogResult.Cancel;
		}
		public static DialogResult ShowPageSetupDialog(this IReport report) {
			return GetPrintTool(report).ShowPageSetupDialog(null) == true ? DialogResult.OK : DialogResult.Cancel;
		}
	}
	public class ReportPrintTool : PrintTool, IReportPrintTool {
		IReport report;
		UserLookAndFeel savedLookAndFeel;
		List<ParameterInfo> parametersInfo = new List<ParameterInfo>();
		bool autoShowParametersPanel = true;
		Nullable<bool> approveParametersResult;
		public bool AutoShowParametersPanel {
			get { return autoShowParametersPanel; }
			set { autoShowParametersPanel = value; }
		}
#if !SL
	[DevExpressXtraPrintingLocalizedDescription("ReportPrintToolReport")]
#endif
		public IReport Report {
			get { return report; }
		}
		public ReportPrintTool(IReport report) : base(report.PrintingSystemBase) {
			if(report == null)
				throw new ArgumentNullException();
			this.report = report;
			this.report.PrintTool = this;
			report.PrintingSystemBase.AfterChange += OnPrintingSystemChanged;
		}
		void OnPrintingSystemChanged(object sender, XtraPrinting.ChangeEventArgs e) {
			if(e.EventName != DevExpress.XtraPrinting.SR.BrickClick)
				return;
			VisualBrick brick = e.ValueOf(DevExpress.XtraPrinting.SR.Brick) as VisualBrick;
			if(brick == null || !(brick.Value is DrillDownKey))
				return;
			DrillDownKey key = (DrillDownKey)brick.Value;
			IDrillDownServiceBase serv = ((IServiceProvider)report).GetService(typeof(IDrillDownServiceBase)) as IDrillDownServiceBase;
			bool expanded;
			if(serv != null && serv.Keys.TryGetValue(key, out expanded)) {
				serv.Keys[key] = !expanded;
				RefreshDocument(serv);
			}
		}
		void RefreshDocument(IDrillDownServiceBase serv) {
			report.PrintingSystem.AfterChange -= OnPrintingSystemChanged;
			report.ReleasePrintingSystem();
			report.PrintingSystemBase.AfterChange += OnPrintingSystemChanged;
			report.PrintingSystemBase.PageSettings.Assign(PrintingSystem.PageSettings.Data);
			report.PrintingSystemBase.PageSettings.IsPresetted = true;
			report.PrintingSystemBase.AfterBuildPages -= OnAfterBuildPages;
			report.PrintingSystemBase.AfterBuildPages += OnAfterBuildPages;
			serv.IsDrillDowning = true;
			try {
				approveParametersResult = true;
				report.CreateDocument(true);
			} finally {
				serv.IsDrillDowning = false;
				approveParametersResult = null;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual void OnAfterBuildPages(object sender, EventArgs e) {
			((PrintingSystemBase)sender).AfterBuildPages -= OnAfterBuildPages;
			PrintingSystemBase prevPrintingSystem = PrintingSystem;
			IPrintingSystemExtender prevExtender = prevPrintingSystem.Extend();
			if(ControlIsValid(prevExtender.ActiveViewer)) {
				var updateStrategy = report.PrintingSystemBase.GetService<IUpdateDrillDownReportStrategy>() ?? new UpdateDrillDownReportStrategy();
				updateStrategy.Update(report, prevPrintingSystem);
				PrintingSystem = report.PrintingSystemBase;
				IPrintingSystemExtender extender = PrintingSystem.Extend();
				extender.CommandSet.CopyFrom(prevExtender.CommandSet);
				prevExtender.ActiveViewer.PrintingSystem = PrintingSystem;
				prevPrintingSystem.Dispose();
				System.Diagnostics.Debug.Assert(prevExtender.ActiveViewer == null);
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(report != null && report.PrintTool == this) {
					report.PrintingSystemBase.AfterChange -= OnPrintingSystemChanged;
					report.PrintTool = null;
				}
				report = null;
				savedLookAndFeel = null;
			}
			base.Dispose(disposing);
		}
		bool IReportPrintTool.ApproveParameters(Parameter[] parameters, bool requestParameters) {
			if(approveParametersResult.HasValue) 
				return approveParametersResult.Value;
			UpdateParametersInfo(parameters);
			ParametersRequestService.OnBeforeShow(parametersInfo, report);
			IPrintingSystemExtender extender = PrintingSystem.Extend();
			bool result = false;
			string  errorMessage;
			if(!CascadingParametersService.ValidateFilterStrings(parametersInfo.Select(x => x.Parameter), out errorMessage)) {
				NotificationService.ShowMessage<PrintingSystemBase>(this.savedLookAndFeel, null, errorMessage, PreviewStringId.Msg_ErrorTitle.GetString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			if(ControlIsValid(extender.ActiveViewer)) {
				ParametersPanelBuilder parametersBuilder = new ParametersPanelBuilder(this.report, parametersInfo);
				parametersBuilder.BuildParametersPanel(extender.ActiveViewer, AutoShowParametersPanel);
			} else if(requestParameters && parametersInfo.Count > 0) {
				ParametersFormBuilder parametersBuilder = new ParametersFormBuilder(this.report, parametersInfo);
				result = DialogResult.OK == parametersBuilder.ShowDialog(savedLookAndFeel);
			} else
				result = true;
			SetPrintingCommandVisibility(extender, PSCommandHelper.ExportCommands, DevExpress.XtraPrinting.CommandVisibility.All);
			SetPrintingCommandVisibility(extender, PSCommandHelper.SendCommands, DevExpress.XtraPrinting.CommandVisibility.All);
			return result;
		}
		void SetPrintingCommandVisibility(IPrintingSystemExtender extender, PrintingSystemCommand[] commands, DevExpress.XtraPrinting.CommandVisibility visibility) {
			foreach(PrintingSystemCommand command in commands) {
				if(IsCommandLowPriority(extender.CommandSet, command))
					extender.SetCommandVisibility(new PrintingSystemCommand[] { command }, visibility, Priority.High);
			}
		}
		static bool IsCommandLowPriority(CommandSet commandSet, PrintingSystemCommand command) {
			CommandSetItem item = commandSet[command];
			if(item != null)
				return item.IsLowPriority;
			return false;
		}
		static bool ControlIsValid(Control control) {
			return control != null && !control.IsDisposed;
		}
		void UpdateParametersInfo(Parameter[] parameters) {
			parametersInfo.Clear();
			foreach(Parameter parameter in parameters) {
				parametersInfo.Add(new ParameterInfo(parameter, CreateInstance));
			}
			report.RaiseParametersRequestBeforeShow(parametersInfo);
			for(int i = parametersInfo.Count - 1; i >= 0; i--) {
				if(!parametersInfo[i].Parameter.Visible)
					parametersInfo.RemoveAt(i);
			}
			UpdateEditorsDataSource(parametersInfo);
		}
		void CreateIfEmpty(bool buildPagesInBackground) {
			if(PrintingSystem.Document.IsCreating && buildPagesInBackground)
				return;
			if(PrintingSystem.Document.IsCreating && !buildPagesInBackground)
				PrintingSystem.ClearContent();
			if(PrintingSystem.Document.IsEmpty)
				report.CreateDocument(buildPagesInBackground);
		}
		void CreateIfEmptyForPreview(bool buildPagesInBackground) {
			if(DocumentIsEmptyAndNotCreating(report.PrintingSystemBase.Document)) {
				report.CreateDocument(buildPagesInBackground);
			} else {
				List<Parameter> parameters = new List<Parameter>();
				report.CollectParameters(parameters, p => true);
				((IReportPrintTool)this).ApproveParameters(parameters.ToArray(), report.RequestParameters);
			}
		}
		bool DocumentIsEmptyAndNotCreating(Document document) {
			return document.IsEmpty && !document.IsCreating;
		}
		protected override void BeforeShowPreview(XtraForm form, UserLookAndFeel lookAndFeel) {
			savedLookAndFeel = lookAndFeel;
			ReportTool.SyncPrintControl(report, ((IPrintPreviewForm)form).PrintControl);
			CreateIfEmptyForPreview(true);
		}
		protected override void BeforePrint() {
			CreateIfEmpty(false);
		}
#if DEBUGTEST
		public void ShowPreview(bool buildPagesInBackground) {
			CreateIfEmptyForPreview(buildPagesInBackground);
			ShowPreview();
		}
#endif
		List<ParameterInfo> IReportPrintTool.ParametersInfo { get { return parametersInfo; } }
		void IReportPrintTool.ShowPreview(object lookAndFeel) {
			ShowPreview((UserLookAndFeel)lookAndFeel);
		}
		void IReportPrintTool.ShowPreviewDialog(object lookAndFeel) {
			ShowPreviewDialog((UserLookAndFeel)lookAndFeel);
		}
		void IReportPrintTool.ShowRibbonPreview(object lookAndFeel) {
			ShowRibbonPreview((UserLookAndFeel)lookAndFeel);
		}
		void IReportPrintTool.ShowRibbonPreviewDialog(object lookAndFeel) {
			ShowRibbonPreviewDialog((UserLookAndFeel)lookAndFeel);
		}
		bool? IReportPrintTool.ShowPageSetupDialog(object lookAndFeel) {
			return ShowPageSetup();
		}
		public override bool? ShowPageSetup() {
			try {
				report.ApplyPageSettings(PrintingSystem.PageSettings);
				if(base.ShowPageSetup() == true) {
					report.UpdatePageSettings(PrintingSystem.PageSettings, PrintingSystem.PageSettings.PaperName);
					return true;
				}
			} catch { }
			return false;
		}
		Control CreateInstance(Parameter parameter) {
			RepositoryItem item = GetRepositoryItem(parameter);
			BaseEdit editor = item.CreateEditor();
			editor.Properties.Assign(item);
			return editor;
		}
		RepositoryItem GetRepositoryItem(Parameter parameter) {
			string value;
			report.Extensions.TryGetValue(DataEditorService.Guid, out value);
			EditingContext context = new EditingContext(value, report);
			RepositoryItem item = DataEditorService.GetRepositoryItem(parameter.Type, parameter, context);
			if(item != null) return item;
			LookUpSettings lookUpSettings = parameter.LookUpSettings;
			if(lookUpSettings != null && parameter.MultiValue)
				return CreateMultiValueLookUpItem(GetLookUpValues(lookUpSettings), parameter.Type);
			if(lookUpSettings != null)
				return CreateLookUpItem(GetLookUpValues(lookUpSettings));
			if(parameter.MultiValue)
				return DataEditorService.GetMultiValueRepositoryItem(parameter.Type);
			return DataEditorService.GetRepositoryItem(parameter.Type);
		}
		IList<LookUpValue> GetLookUpValues(LookUpSettings lookUpSettings) {
			DataContext dataContext = ((IServiceProvider)Report).GetService(typeof(DataContext)) as DataContext;
			return LookUpHelper.GetLookUpValues(lookUpSettings, dataContext);
		}
		static RepositoryItem CreateLookUpItem(IList<LookUpValue> lookUpValues) {
			RepositoryItemLookUpEdit lookUpItem = new RepositoryItemLookUpEdit() { 
				TextEditStyle = TextEditStyles.DisableTextEditor,
				ShowHeader = false, 
				NullText = string.Empty,
				DisplayMember = LookUpValue.DescriptionPropertyName,
				ValueMember = LookUpValue.ValuePropertyName,
			};
			lookUpItem.Columns.Add(new LookUpColumnInfo(LookUpValue.DescriptionPropertyName, string.Empty));
			if(lookUpValues != null && lookUpValues.Count > 0)
				lookUpItem.DataSource = lookUpValues;
			return lookUpItem;
		}
		static RepositoryItem CreateMultiValueLookUpItem(IList<LookUpValue> lookUpValues, Type valueType) {
			RepositoryItemMultiValueComboBoxEdit lookUpItem = new RepositoryItemMultiValueComboBoxEdit() {
				EditValueType = EditValueTypeCollection.List,
				ForceUpdateEditValue = DevExpress.Utils.DefaultBoolean.True,
				NullText = string.Empty,
				ValueType = valueType,
				DisplayMember = LookUpValue.DescriptionPropertyName,
				ValueMember = LookUpValue.ValuePropertyName
			};
			if(lookUpValues != null && lookUpValues.Count > 0)
				lookUpItem.DataSource = lookUpValues;
			return lookUpItem;
		}
		void UpdateEditorsDataSource(List<ParameterInfo> parametersInfo) {
			for(int i = 0; i < parametersInfo.Count; i++) {
				DynamicListLookUpSettings lookUpSettings = parametersInfo[i].Parameter.LookUpSettings as DynamicListLookUpSettings;
				if(lookUpSettings == null)
					continue;
				BaseEdit editor = parametersInfo[i].GetEditor(false) as BaseEdit;
				if(editor == null || editor.Properties == null)
					continue;
				LookUpEditUpdaterBase updater = LookUpEditUpdaterBase.CreateInstance(editor);
				if(updater == null)
					continue;
				updater.TryUpdateEditor(() => GetLookUpValues(lookUpSettings), true);
			}
		}
	}
}
