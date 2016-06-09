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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.XtraPrinting;
namespace DevExpress.ExpressApp.Win.SystemModule {
	public sealed class PrintableControlEventArgs : EventArgs {
		public PrintableControlEventArgs(IPrintable iPrintable) {
			Printable = iPrintable;
		}
		public IPrintable Printable { get; set; }
	}
	public sealed class PrintableComponentLinkEventArgs : EventArgs {
		private PrintableComponentLink printableComponentLink;
		public PrintableComponentLinkEventArgs(PrintableComponentLink printableComponentLink) {
			this.printableComponentLink = printableComponentLink;
		}
		public PrintableComponentLink PrintableComponentLink {
			get { return printableComponentLink; }
			set { printableComponentLink = value; }
		}
	}
	public enum PrintingSettingsStorage { Application, View }
	public class CustomForceGridControlMethodsEventArgs : EventArgs {
		public CustomForceGridControlMethodsEventArgs(DevExpress.XtraGrid.GridControl grid, bool force) {
			this.GridControl = grid;
			this.Force = force;
		}
		public DevExpress.XtraGrid.GridControl GridControl { get; private set; }
		public bool Force { get; set; }
	}
	public class CustomShowPrintPreviewEventArgs : HandledEventArgs {
		public CustomShowPrintPreviewEventArgs(bool isRibbon) {
			this.IsRibbon = isRibbon;
		}
		public bool IsRibbon { get; set; }
	}
	public class PrintingController : ViewController {
		public const string PrintingSettingsNodeName = "PrintingSettings";
		private SimpleAction pageSetupAction;
		private SimpleAction printAction;
		private SimpleAction printPreviewAction;
		protected internal bool IsPrintingSettingsEmpty {
			get { return ((ModelNode)PrintingSettings).Master == null && !((ModelNode)PrintingSettings).HasModification; }
		}
		private PageSettings LoadPageSettings() {
			PageSettings pageSettings = new PageSettings();
			if(!IsPrintingSettingsEmpty ) {
				pageSettings.PaperSize = new PaperSize(PaperKind.Custom.ToString(), pageSettings.PaperSize.Width, pageSettings.PaperSize.Height);
				pageSettings.PaperSize.PaperName = PrintingSettings.PaperName;
				foreach(PaperSize size in pageSettings.PrinterSettings.PaperSizes) {
					if(size.Kind == PrintingSettings.PaperKind) {
						pageSettings.PaperSize = size;
						break;
					}
				}
				pageSettings.Landscape = PrintingSettings.Landscape;
				pageSettings.Margins = PrintingSettings.Margins;
			}
			return pageSettings;
		}
		private void SavePageSettings(PageSettings pageSettings) {
			PrintingSettings.PaperKind = pageSettings.PaperSize.Kind;
			PrintingSettings.PaperName = pageSettings.PaperSize.PaperName;
			PrintingSettings.Landscape = pageSettings.Landscape;
			PrintingSettings.Margins = pageSettings.Margins;
			if(PrintingSettings.PaperKind == PaperKind.Custom) {
				PrintingSettings.CustomPaperSize = new Size(pageSettings.PaperSize.Width, pageSettings.PaperSize.Height);
			}
		}
		private bool TryGetGridControl(View view, out DevExpress.XtraGrid.GridControl grid) {
			grid = null;
			ListView listView = view as ListView;
			if((listView != null) && (listView.Editor is DevExpress.ExpressApp.Win.Editors.GridListEditor)) {
				grid = ((DevExpress.ExpressApp.Win.Editors.GridListEditor)listView.Editor).Grid;
				return (grid != null);
			}
			return false;
		}
		private bool IsServerMode() {
			ListView listView = View as ListView;
			return (listView != null) && listView.CollectionSource.IsServerMode;
		}
		private void View_ControlsCreated(object sender, EventArgs e) {
			UpdateActionState();
		}
		private void pageSetupAction_OnExecute(Object sender, SimpleActionExecuteEventArgs args) {
			ExecutePageSetup();
		}
		private void printAction_OnExecute(Object sender, SimpleActionExecuteEventArgs args) {
			ExecutePrint();
		}
		private void printPreviewAction_OnExecute(Object sender, SimpleActionExecuteEventArgs args) {
			ExecutePrintPreview();
		}
		private SystemWindowsFormsModule SystemWindowsFormsModule {
			get {
				SystemWindowsFormsModule module = null;
				if(Application != null) {
					module = (SystemWindowsFormsModule)Application.Modules.FindModule(typeof(SystemWindowsFormsModule));
				}
				if(module == null) {
					Tracing.Tracer.LogWarning(GetType() + ": SystemWindowsFormsModule is null");
				}
				return module;
			}
		}
		protected virtual IModelPrintingSettings PrintingSettings {
			get {
				IModelOptionsPrintingSettings options = null;
				if(GetPrintingSettingsStorageMode() == PrintingSettingsStorage.View) {
					options = View.Model as IModelOptionsPrintingSettings;
				}
				else {
					options = GetApplicationModel().Options as IModelOptionsPrintingSettings;
				}
				if(options != null) {
					return options.PrintingSettings;
				}
				return null;
			}
		}
		protected IPrintable GetPrintable() {
			IPrintable printableControl = null;
			if(View.Control is IExportable) {
				printableControl = ((IExportable)(View.Control)).Printable;
			}
			else {
				printableControl = View.Control as IPrintable;
			}
			if(View is ListView) {
				IExportable exportable = ((ListView)View).Editor as IExportable;
				if(exportable != null) {
					printableControl = exportable.Printable;
				}
			}
			PrintableControlEventArgs args = new PrintableControlEventArgs(printableControl);
			OnCustomizePrintableControl(args);
			printableControl = args.Printable;
			return printableControl;
		}
		protected virtual IModelApplication GetApplicationModel() {
			return Application.Model;
		}
		protected virtual PrintingSettingsStorage GetPrintingSettingsStorageMode() {
			return (SystemWindowsFormsModule != null) ? SystemWindowsFormsModule.PrintingSettingsStorage : PrintingSettingsStorage.Application;
		}
		protected virtual void OnCustomizePrintableControl(PrintableControlEventArgs args) {
			if(CustomGetPrintableControl != null) {
				CustomGetPrintableControl(this, args);
			}
		}
		protected virtual void OnPrintingSettingsLoaded(PrintableComponentLinkEventArgs args) {
			if(PrintingSettingsLoaded != null) {
				PrintingSettingsLoaded(this, args);
			}
		}
		protected virtual void UpdateActionState() {
			foreach(ActionBase action in Actions) {
				action.Enabled.SetItemValue("View control is IPrintable", GetPrintable() != null);
			}
		}
		protected virtual void ExecutePageSetup() {
			PageSetupDialog dlg = new PageSetupDialog();
			dlg.PageSettings = LoadPageSettings();
			dlg.EnableMetric = RegionInfo.CurrentRegion.IsMetric;
			if(dlg.ShowDialog() == DialogResult.OK) {
				SavePageSettings(dlg.PageSettings);
			}
		}
		protected virtual bool OnCustomForceGridControlMethods(DevExpress.XtraGrid.GridControl grid, bool defaultForce) {
			CustomForceGridControlMethodsEventArgs args = new CustomForceGridControlMethodsEventArgs(grid, defaultForce);
			if(CustomForceGridControlMethods != null) {
				CustomForceGridControlMethods(this, args);
			}
			return args.Force;
		}
		protected virtual void ExecutePrint() {
			HandledEventArgs args = new HandledEventArgs();
			if(CustomPrint != null) {
				CustomPrint(this, args);
			}
			if(!args.Handled) {
				DevExpress.XtraGrid.GridControl grid;
				if(TryGetGridControl(View, out grid)) {
					bool force = OnCustomForceGridControlMethods(grid, IsServerMode());
					if(force) {
						grid.PrintDialog();
						return;
					}
				}
				PrintableComponentLink printableComponentLink = new PrintableComponentLink(new PrintingSystem());
				printableComponentLink.Component = GetPrintable();
				LoadPrintingSettings(printableComponentLink);
				if(printableComponentLink != null) {
					printableComponentLink.PrintDlg();
					SavePrintingSettings(printableComponentLink);
				}
			}
		}
		protected virtual void ExecutePrintPreview() {
			bool isRibbon = (Application.Model.Options is IModelOptionsWin)
				&& (((IModelOptionsWin)(Application.Model.Options)).FormStyle == DevExpress.XtraBars.Ribbon.RibbonFormStyle.Ribbon);
			CustomShowPrintPreviewEventArgs args = new CustomShowPrintPreviewEventArgs(isRibbon);
			if(CustomShowPrintPreview != null) {
				CustomShowPrintPreview(this, args);
			}
			if(!args.Handled) {
				DevExpress.XtraGrid.GridControl grid;
				if(TryGetGridControl(View, out grid)) {
					bool force = OnCustomForceGridControlMethods(grid, IsServerMode());
					if(force) {
						if(isRibbon) {
							grid.ShowRibbonPrintPreview();
						}
						else {
							grid.ShowPrintPreview();
						}
						return;
					}
				}
				PrintableComponentLink printableComponentLink = new PrintableComponentLink(new PrintingSystem());
				printableComponentLink.Component = GetPrintable();
				LoadPrintingSettings(printableComponentLink);
				if(isRibbon) {
					printableComponentLink.PrintingSystem.PreviewRibbonFormEx.RibbonControl.RibbonStyle = ((IModelOptionsWin)(Application.Model.Options)).RibbonOptions.RibbonControlStyle;
					printableComponentLink.ShowRibbonPreviewDialog(DevExpress.LookAndFeel.UserLookAndFeel.Default);
				}
				else {
					printableComponentLink.ShowPreviewDialog();
				}
				SavePrintingSettings(printableComponentLink);
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			View.ControlsCreated += new EventHandler(View_ControlsCreated);
		}
		protected override void OnDeactivated() {
			View.ControlsCreated -= new EventHandler(View_ControlsCreated);
			base.OnDeactivated();
		}
		public virtual void LoadPrintingSettings(PrintableComponentLink printableComponentLink) {
			Guard.ArgumentNotNull(printableComponentLink, "printableComponentLink");
			if(!IsPrintingSettingsEmpty ) {
				printableComponentLink.PaperKind = PrintingSettings.PaperKind;
				printableComponentLink.PaperName = PrintingSettings.PaperName;
				printableComponentLink.Landscape = PrintingSettings.Landscape;
				printableComponentLink.Margins = PrintingSettings.Margins;
				if(printableComponentLink.PaperKind == PaperKind.Custom) {
					printableComponentLink.CustomPaperSize = PrintingSettings.CustomPaperSize;
				}
				if(!string.IsNullOrEmpty(PrintingSettings.PageHeaderFooter)) {
					using(Stream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(PrintingSettings.PageHeaderFooter.ToCharArray()))) {
						printableComponentLink.RestorePageHeaderFooterFromStream(stream);
					}
				}
				else {
					printableComponentLink.PageHeaderFooter = new PageHeaderFooter();
				}
			}
			OnPrintingSettingsLoaded(new PrintableComponentLinkEventArgs(printableComponentLink));
		}
		public virtual void SavePrintingSettings(PrintableComponentLink printableComponentLink) {
			Guard.ArgumentNotNull(printableComponentLink, "printableComponentLink");
			PrintingSettings.PaperKind = printableComponentLink.PaperKind;
			PrintingSettings.PaperName = printableComponentLink.PaperName;
			PrintingSettings.Landscape = printableComponentLink.Landscape;
			PrintingSettings.Margins = printableComponentLink.Margins;
			if(printableComponentLink.PaperKind == PaperKind.Custom) {
				PrintingSettings.CustomPaperSize = printableComponentLink.CustomPaperSize;
			}
			using(Stream stream = new MemoryStream()) {
				printableComponentLink.SavePageHeaderFooterToStream(stream);
				stream.Position = 0;
				PrintingSettings.PageHeaderFooter = new StreamReader(stream).ReadToEnd();
			}
		}
		public PrintingController()
			: base() {
			this.pageSetupAction = new SimpleAction(this, "PageSetup", PredefinedCategory.Print);
			this.printAction = new SimpleAction(this, "Print", PredefinedCategory.Print);
			this.printPreviewAction = new SimpleAction(this, "PrintPreview", PredefinedCategory.Print);
			this.pageSetupAction.Caption = "Page Setup...";
			this.pageSetupAction.ToolTip = "Page Setup";
			this.pageSetupAction.ImageName = "MenuBar_PageSetup";
			this.pageSetupAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.pageSetupAction_OnExecute);
			this.pageSetupAction.TargetViewNesting = Nesting.Root;
			this.printAction.Caption = "Print...";
			this.printAction.Shortcut = "CtrlP";
			this.printAction.ToolTip = "Print Dialog";
			this.printAction.ImageName = "MenuBar_Print";
			this.printAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.printAction_OnExecute);
			this.printAction.TargetViewNesting = Nesting.Root;
			this.printPreviewAction.Caption = "Print Preview...";
			this.printPreviewAction.ToolTip = "Print Preview";
			this.printPreviewAction.ImageName = "MenuBar_PrintPreview";
			this.printPreviewAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.printPreviewAction_OnExecute);
			this.printPreviewAction.TargetViewNesting = Nesting.Any;
		}
		public SimpleAction PageSetupAction {
			get { return pageSetupAction; }
		}
		public SimpleAction PrintAction {
			get { return printAction; }
		}
		public SimpleAction PrintPreviewAction {
			get { return printPreviewAction; }
		}
		public IPrintable PrintableControl {
			get { return GetPrintable(); }
		}
		public event EventHandler<HandledEventArgs> CustomPrint;
		public event EventHandler<CustomShowPrintPreviewEventArgs> CustomShowPrintPreview;
		public event EventHandler<CustomForceGridControlMethodsEventArgs> CustomForceGridControlMethods;
		public event EventHandler<PrintableControlEventArgs> CustomGetPrintableControl;
		public event EventHandler<PrintableComponentLinkEventArgs> PrintingSettingsLoaded;
	}
#if !SL
	[DevExpressExpressAppWinLocalizedDescription("SystemModuleIModelPrintingSettings")]
#endif
	public interface IModelPrintingSettings : IModelNode {
		[
#if !SL
	DevExpressExpressAppWinLocalizedDescription("IModelPrintingSettingsPaperKind"),
#endif
 Category("Behavior")]
		[DefaultValue(PaperKind.Letter)]
		PaperKind PaperKind { get; set; }
		[DefaultValue("")]
		[
#if !SL
	DevExpressExpressAppWinLocalizedDescription("IModelPrintingSettingsPaperName"),
#endif
 Category("Behavior")]
		string PaperName { get; set; }
		[DefaultValue(false)]
		[
#if !SL
	DevExpressExpressAppWinLocalizedDescription("IModelPrintingSettingsLandscape"),
#endif
 Category("Behavior")]
		bool Landscape { get; set; }
		[
#if !SL
	DevExpressExpressAppWinLocalizedDescription("IModelPrintingSettingsMargins"),
#endif
 Category("Layout")]
		Margins Margins { get; set; }
		[
#if !SL
	DevExpressExpressAppWinLocalizedDescription("IModelPrintingSettingsCustomPaperSize"),
#endif
 Category("Layout")]
		Size CustomPaperSize { get; set; }
		[DefaultValue(""), Browsable(false)]
		[
#if !SL
	DevExpressExpressAppWinLocalizedDescription("IModelPrintingSettingsPageHeaderFooter"),
#endif
 Category("Behavior")]
		string PageHeaderFooter { get; set; }
	}
	public interface IModelOptionsPrintingSettings : IModelNode {
		[ModelBrowsable(typeof(PrintingSettingsVisibilityCalculator))]
		IModelPrintingSettings PrintingSettings { get; }
	}
	[DomainLogic(typeof(IModelPrintingSettings))]
	public static class ModelPrintingSettingsDomainLogic {
		public static Margins Get_Margins() {
			return new Margins(0, 0, 0, 0);
		}
	}
	public class PrintingSettingsVisibilityCalculator : IModelIsVisible {
		public bool IsVisible(IModelNode node, String propertyName) {
			PrintingSettingsStorage storage = PrintingSettingsStorage.Application;
			if(node.Application is IModelSources) {
				foreach(ModuleBase module in ((IModelSources)node.Application).Modules) {
					if(module is SystemWindowsFormsModule) {
						storage = ((SystemWindowsFormsModule)module).PrintingSettingsStorage;
					}
				}
			}
			if(node is IModelOptions) {
				return storage == PrintingSettingsStorage.Application;
			}
			return storage == PrintingSettingsStorage.View;
		}
	}
}
