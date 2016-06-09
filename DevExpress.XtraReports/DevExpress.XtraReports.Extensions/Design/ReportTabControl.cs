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
using System.Security;
using System.Windows.Forms;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.XtraReports.UI;
using System.ComponentModel.Design;
using DevExpress.XtraPrinting.Export.Web;
using System.Collections;
using DevExpress.XtraPrinting;
using System.IO;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Serialization;
using DevExpress.XtraReports.Design.Commands;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraPrinting.Preview;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.XtraBars;
using System.Reflection;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.LookAndFeel.Helpers;
using System.Runtime.InteropServices;
using DevExpress.XtraReports.Native.Parameters;
using DevExpress.XtraPrinting.Preview.Native;
using System.Collections.Generic;
using System.Drawing.Printing;
using DevExpress.DataAccess;
using DevExpress.XtraPrinting.Drawing;
using DevExpress.XtraReports.UserDesigner.Native;
using DevExpress.Utils.Design;
using DevExpress.DataAccess.Native;
using DevExpress.Entity.ProjectModel;
using DevExpress.Data.Utils;
namespace DevExpress.XtraReports.Design
{
	public class ReportTabControl : XtraTabControl, ISupportLookAndFeel {
		#region inner classes
		class DesignPrintControl : DevExpress.XtraPrinting.Control.PrintControl {
			protected override void WndProc(ref Message m) {
				if(m.Msg != DevExpress.XtraPrinting.Native.Win32.WM_CONTEXTMENU)
					base.WndProc(ref m);
			}
		}
		class DesignPrintingSystemExtender : PrintingSystemExtenderWin {
			IPrintControl printControl;
			public override ProgressReflector ActiveProgressReflector {
				get {
					return printControl.ProgressReflector;
				}
			}
			public DesignPrintingSystemExtender(PrintingSystemBase printingSystem, IPrintControl printControl)
				: base(printingSystem) {
				this.printControl = printControl;
			}
		}
		#endregion
		#region static
		static string CreateTempDirecory(string basePath, string directoryName) {
			string path = Path.Combine(basePath, directoryName);
			if(!Directory.Exists(path))
				Directory.CreateDirectory(path);
			return path;
		}
		#endregion
		#region ISupportLookAndFeel
		bool ISupportLookAndFeel.IgnoreChildren { get { return false; }
		}
		[
		Category("Appearance"), 
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), 
		]
		public UserLookAndFeel LookAndFeel { get { return userLookAndFeel; }
		}
		#endregion //ISupportLookAndFeel
		#region fields&properties
		protected XtraReport report;
		XtraReport previewReport;
		IServiceProvider serviceProvider;
		string htmlDocumentPath;
		ReportDesigner reportDesigner;
		DevExpress.XtraPrinting.Control.PrintControl printControl;
		Panel printControlPlaceHolder;
		WebBrowser browser;
		ScriptControl scriptEditor;
		UserLookAndFeel userLookAndFeel;
		ReportFrame designerView;
		PanelControl bottomMarginPanel;
		ReportCommandService reportCommandService;
		bool showPrintPreviewBar = true;
		bool showTabButtons = true;
		IDesignerHost host;
		bool exceptionWasShown;
		TabControlLogic tabControlLogic;
		public XtraReport PreviewReport {
			get { return previewReport; }
		}
		public DevExpress.XtraPrinting.Control.PrintControl PreviewControl {
			get { return printControl; }
		}
#if DEBUGTEST
		public BarManager BarManager {
			get { return Logic.BarManager; }
		}
		public void ActivatePage(int pageIndex) {
			OnPageActivate(fTabPages[pageIndex], EventArgs.Empty);
		}
		bool buildPagesInBackground = true;
		public void ShowPreviewTab(bool buildPagesInBackground) {
			bool store = this.buildPagesInBackground;
			try {
				this.buildPagesInBackground = buildPagesInBackground;
				this.SelectedIndex = TabIndices.Preview;
			} finally {
				this.buildPagesInBackground = store;
			}
		}
		public void ShowDesignerTab() {
			this.SelectedIndex = TabIndices.Designer;
		}
		public BarStaticItem DesignZoomItem {
			get { return Logic.DesignZoomItem; }
		}
#endif
		protected string HtmlDocumentPath {
			get {
				if(htmlDocumentPath == null)
					htmlDocumentPath = GetPath();
				return htmlDocumentPath;
			}
		}
		public PrintingSystemBase PrintingSystem {
			get { return previewReport != null ? previewReport.PrintingSystem : null; }
		}
		public bool ShowPrintPreviewBar { get { return showPrintPreviewBar; } set { showPrintPreviewBar = value; }
		}
		public bool ShowTabButtons { get { return showTabButtons; } set { showTabButtons = value; } }
		internal TabControlLogic Logic {
			get {
				if(tabControlLogic == null) {
					TabControlLogicCreator creator = serviceProvider.GetService(typeof(TabControlLogicCreator)) as TabControlLogicCreator;
					if(creator != null)
						tabControlLogic = creator.CreateInstance(this, serviceProvider);
				}
				if(tabControlLogic == null)
					tabControlLogic = new TabControlBarLogic(this, serviceProvider);
				return tabControlLogic;
			}
		}
		public override Color ForeColor {
			get {
				if(LookAndFeel == null || LookAndFeel.ActiveStyle != ActiveLookAndFeelStyle.Skin)
					return base.ForeColor;
				DevExpress.Skins.SkinElement element = DevExpress.Skins.CommonSkins.GetSkin(LookAndFeel)[DevExpress.Skins.CommonSkins.SkinForm];
				if(element == null)
					return base.ForeColor;
				return element.Color.GetForeColor();
			}
			set {
				base.ForeColor = value;
			}
		}
		#endregion
		#region events
		public event EventHandler PreviewReportCreated;
		void RaisePreviewReportCreated() {
			if(PreviewReportCreated != null)
				PreviewReportCreated(this, EventArgs.Empty);
		}
		#endregion
		public ReportTabControl(ReportDesigner reportDesigner) {
			this.reportDesigner = reportDesigner;
			report = reportDesigner.Component as XtraReport;
			serviceProvider = report.Site;
			host = (IDesignerHost)serviceProvider.GetService(typeof(IDesignerHost));
			host.Activated += new EventHandler(host_Activated);
			userLookAndFeel = new ControlUserLookAndFeel(this);
			ILookAndFeelService serv = host.GetService(typeof(ILookAndFeelService)) as ILookAndFeelService;
			serv.InitializeRootLookAndFeel(userLookAndFeel);
			userLookAndFeel.StyleChanged += new EventHandler(userLookAndFeel_StyleChanged);
			SuspendLayout();
			bottomMarginPanel = new PanelControl();
			bottomMarginPanel.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			bottomMarginPanel.BorderStyle = BorderStyles.NoBorder;
			bottomMarginPanel.Height = GetPanelBottmIndent();
			bottomMarginPanel.Dock = DockStyle.Bottom;
			Logic.Initialize(serviceProvider, reportDesigner.IsEUD);
			printControlPlaceHolder = new Panel();
			designerView = reportDesigner.ReportFrame;
			designerView.LookAndFeel.ParentLookAndFeel = userLookAndFeel;
			Logic.AddPage(designerView, ReportStringId.RepTabCtl_Designer, ReportCommand.ShowDesignerTab, false);
			Logic.AddPage(printControlPlaceHolder, ReportStringId.RepTabCtl_Preview, ReportCommand.ShowPreviewTab, true);
			AddBrowserPage();
			AddScriptsPage();
			UpdatePages();
			SetImages();
			Controls.Add(bottomMarginPanel);
			ResumeLayout();
			SelectedIndex = TabIndices.Designer;
			this.SelectedTabIndexChanged += new EventHandler(OnSelectedTabIndexChanged);
			UpdateVisibility();
			reportCommandService = serviceProvider.GetService(typeof(ReportCommandService)) as ReportCommandService;
			if(reportCommandService != null)
				reportCommandService.CommandChanged += new ReportCommandEventHandler(OnReportCommandChanged);
		}
		void host_Activated(object sender, EventArgs e) {
			UpdateVisibility();
		}
		public void HtmlBackward() {
			if(browser != null)
				browser.GoBack();
		}
		public void HtmlForward() {
			if(browser != null)
				browser.GoForward();
		}
		public void HtmlHome() {
			if(browser != null)
				browser.GoHome();
		}
		public void HtmlRefresh() {
			if(browser != null)
				browser.Refresh();
		}
		public void HtmlFind() {
			if(browser != null)
				browser.ShowFindDialog();
		}
		protected virtual void AddBrowserPage() {
			browser = new WebBrowser();
			browser.Dock = DockStyle.Fill;
			Logic.AddPage(browser, ReportStringId.RepTabCtl_HtmlView, ReportCommand.ShowHTMLViewTab, false);
		}
		public string SelectScript(ITypeDescriptorContext context, string eventName, bool createNewProcedureName) {
			if(scriptEditor != null)
				return scriptEditor.SetSelectedScript(((XRScriptsBase)context.Instance).Component as IScriptable , eventName, createNewProcedureName);
			return string.Empty;
		}
		public string[] GetCompatibleMethodsNames(IScriptable scriptContainer, string eventName) {
			if(scriptEditor != null)
				return scriptEditor.GetCompatibleMethodsNames(scriptContainer, eventName);
			return new string[] { };
		}
		protected virtual void AddScriptsPage() {
			scriptEditor = CreateScriptControl();
			scriptEditor.Dock = DockStyle.Fill;
			Logic.AddPage(scriptEditor, ReportStringId.RepTabCtl_Scripts, ReportCommand.ShowScriptsTab, true);
		}
		protected virtual ScriptControl CreateScriptControl() {
			return new ScriptControl(report, serviceProvider, userLookAndFeel, Logic.BarManager);
		}
		public void UpdateStatusValue(string status) {
			Logic.UpdateStatus(status);
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			UpdateView();
			Logic.Activate();
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			if(IsHandleCreated) 
				Logic.Activate();
		}
		void EnsurePrintControl() {
			if(printControl != null)
				return;
			printControl = CreatePrintControl();
			printControl.Dock = DockStyle.Fill;
			printControl.SelectedPageIndex = -1;
			printControl.LookAndFeel.ParentLookAndFeel = userLookAndFeel;
			printControlPlaceHolder.Controls.Add(printControl);
			Logic.OnPrintControlCreated();
		}
		protected virtual DevExpress.XtraPrinting.Control.PrintControl CreatePrintControl() {
			return new DesignPrintControl();
		}
		void SetImages() {
			DevExpress.Utils.ImageCollection tabImages = ImageCollectionHelper.CreateImageCollectionFromResources(LocalResFinder.GetFullName("Images.TabControlButtons.png"), LocalResFinder.Assembly);
			for(int i = 0; i < fTabPages.Count; i ++)
				fTabPages[i].Image = tabImages.Images[i];
			tabImages.Dispose();
		}
		void userLookAndFeel_StyleChanged(object sender, EventArgs e) {
			this.bottomMarginPanel.Height = GetPanelBottmIndent();
			UpdateView();
		}
		int GetPanelBottmIndent() {
			return ReportPaintStyles.GetPaintStyle(this.LookAndFeel).GetDesignPanelBottomIndent(this.LookAndFeel);
		}
		void UpdateView() {
			IBandViewInfoService svc = (IBandViewInfoService)serviceProvider.GetService(typeof(IBandViewInfoService));
			if(svc != null) svc.UpdateView();
		}
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				DeleteTempFiles();
				DisposePreviewReport();
				this.SelectedTabIndexChanged -= new EventHandler(OnSelectedTabIndexChanged);
				host.Activated -= new EventHandler(host_Activated);
				if(userLookAndFeel != null) {
					userLookAndFeel.StyleChanged -= new EventHandler(userLookAndFeel_StyleChanged);
					userLookAndFeel.Dispose();
					userLookAndFeel = null;
				}
				if(reportCommandService != null) {
					reportCommandService.CommandChanged -= new ReportCommandEventHandler(OnReportCommandChanged);
					reportCommandService = null;
				}
				if(browser != null) {
					browser.Dispose();
					browser = null;
				}
				reportDesigner = null;
			}
			base.Dispose( disposing );
			if(disposing) {
				if(tabControlLogic != null) {
					tabControlLogic.Dispose();
					tabControlLogic = null;
				}
			}
		}
		protected virtual void DisposePreviewReport() {
			if(previewReport != null) {
				printControl.PrintingSystem = null;
				previewReport.AfterPrint -= new EventHandler(previewReport_AfterPrint);
				RestoreConnection();
				previewReport.ParametersRequestSubmit -= previewReport_ParametersRequestSubmit;
				previewReport.PrintingSystem.CreateDocumentException -= new DevExpress.XtraPrinting.ExceptionEventHandler(PrintingSystem_CreateDocumentException);
				previewReport.Dispose();
				previewReport = null;
			}
		}
		private void DeleteTempFiles() {
			try {
				if(htmlDocumentPath == null || File.Exists(htmlDocumentPath) == false)
					return;
				File.Delete(htmlDocumentPath);
				string path = Path.GetDirectoryName(htmlDocumentPath);
				string s = Path.Combine(path, Path.GetFileNameWithoutExtension(htmlDocumentPath) + "_files"); 
				if( Directory.Exists(s) )
					Directory.Delete(s, true);
				string[] files = Directory.GetFiles(path);
				if(files == null || files.Length == 0)
					Directory.Delete(path);
			} catch {}
		}
		string GetPath() {
			string path1 = CreateTempDirecory(Path.GetTempPath(), Guid.NewGuid().ToString("B", null));
			if(!Directory.Exists(path1))
				Directory.CreateDirectory(path1);
			string[] items = host.RootComponentClassName.Split('.');
			System.Diagnostics.Debug.Assert(items.Length > 0 && items[items.Length - 1].Length > 0);
			string path2 = String.Format("{0}.html", items[items.Length - 1]);
			return Path.Combine(path1, path2);
		}
		void OnSelectedTabIndexChanged(object sender, EventArgs e) {
			if(SelectedPageControl == null)
				return;
			UpdateVisibility();
			ClearHtmlBrowser();
			if(SelectedPageCommand == ReportCommand.ShowDesignerTab) {
				HandleDesignerTab(PrintingSystem);
				DisposePreviewReport();
				return;
			}
			if(SelectedPageCommand == ReportCommand.ShowScriptsTab) {
				HandleScriptsTab();
				DisposePreviewReport();
				return;
			}
			AdjustCommands();
			Cursor.Current = Cursors.AppStarting;
			try {
				exceptionWasShown = false;
				EnsurePrintControl();
				if(SelectedPageCommand == ReportCommand.ShowPreviewTab) {
					BeforeCreateDocument();
					PatchConnection();
					PerformReportActions(
						CreateDocument,
						SubscrAfterPrint,
						() => {
							ReportTool.SyncPrintControl(previewReport, printControl);
							printControl.SelectedPageIndex = 0;
							printControl.SetFocus();
						}
				   );
				} else if(SelectedPageCommand == ReportCommand.ShowHTMLViewTab) {
					BeforeCreateDocument();
					PrintingSystem.Extend().RemoveViewer(printControl);
					PatchConnection();
					PerformReportActions(
						CreateDocument,
						SubscrAfterPrint
					);
				}
			} catch(Exception ex) {
				DevExpress.XtraPrinting.Tracer.TraceError(NativeSR.TraceSource, ex);
				StopPageBuilding();
				ShowException(ex);
				RestoreConnection();
			} finally {
				Cursor.Current = Cursors.Default;
			}
		}
		void PatchConnection() {
			IConnectionStringsService svc = (IConnectionStringsService)serviceProvider.GetService(typeof(IConnectionStringsService));
			if(svc != null)
				svc.PatchConnection();
		}
		void RestoreConnection() {
			IConnectionStringsService svc = (IConnectionStringsService)serviceProvider.GetService(typeof(IConnectionStringsService));
			if(svc != null)
				svc.RestoreConnection();
		}
		void AdjustCommands() {
			MenuCommandHandler menuCommandHandler = (MenuCommandHandler)serviceProvider.GetService(typeof(MenuCommandHandler));
			if(menuCommandHandler != null) {
				menuCommandHandler.DisableCommands();
				menuCommandHandler.LockCommands();
				menuCommandHandler.UnlockCommands(
					UICommands.NewReport,
					UICommands.OpenFile,
					UICommands.SaveFile,
					UICommands.SaveFileAs,
					UICommands.SaveAll,
					UICommands.Close,
					UICommands.Exit,
					UICommands.Closing,
					HtmlCommands.Backward,
					HtmlCommands.Forward,
					HtmlCommands.Home,
					HtmlCommands.Refresh,
					HtmlCommands.Find
					);
				menuCommandHandler.UpdateCommandStatus();
			}
		}
		void HandleScriptsTab() {
			if(scriptEditor != null) {
				scriptEditor.AdjustCommands();
				scriptEditor.Text = report.ScriptsSource;
			}
		}
		void HandleDesignerTab(PrintingSystemBase printingSystem) {
			MenuCommandHandler menuCommandHandler = (MenuCommandHandler)serviceProvider.GetService(typeof(MenuCommandHandler));
			if(menuCommandHandler != null) {
				menuCommandHandler.UnlockCommands();
				menuCommandHandler.UpdateCommandStatus();
			}
			if(printingSystem != null && !printingSystem.Document.IsEmpty) {
				SyncPageSettings(printingSystem.PageSettings);
				SyncWatermark(printingSystem.Watermark);
				SyncPageColor(printingSystem.Graph.PageBackColor);
			}
		}
		void previewReport_AfterPrint(object sender, EventArgs e) {
			((XtraReport)sender).AfterPrint -= new EventHandler(previewReport_AfterPrint);
			RestoreConnection();
			if(!((XtraReport)sender).IsDisposed && SelectedPageCommand == ReportCommand.ShowHTMLViewTab) {
				UpdateBrowserView(HtmlDocumentPath);
				Logic.OnBrowserUpdated();
			}
		}
		internal void UpdateVisibility() {
			if(SelectedPageCommand == ReportCommand.ShowDesignerTab)
				Logic.OnDesignerVisible();
			else if(SelectedPageCommand == ReportCommand.ShowScriptsTab)
				Logic.OnScriptsVisible();
			else if(SelectedPageCommand == ReportCommand.ShowPreviewTab)
				Logic.OnPreviewVisible();
			else
				Logic.OnBrowserVisible();
		}
		private void SyncPageColor(Color pageBackColor) {
			if(!report.PageColor.Equals(pageBackColor)) {
				IComponentChangeService svc = (IComponentChangeService)host.GetService(typeof(IComponentChangeService));
				DesignerTransaction transaction = host.CreateTransaction(DesignSR.PageSettings);
				try {
					XRControlDesignerBase.RaiseComponentChanging(svc, report, "PageColor");
					report.PageColor = pageBackColor;
					XRControlDesignerBase.RaiseComponentChanged(svc, report);
				} catch {
					transaction.Cancel();
				} finally {
					transaction.Commit();
				}
			}
		}
		protected virtual void SyncWatermark(Watermark watermark) {
			if(!report.Watermark.Equals(watermark) ) 
				reportDesigner.CopyWatermark(watermark, report.Watermark);
		}
		private void SyncPageSettings(XtraPageSettingsBase pageSettings) {
			if (report.DefaultPrinterSettingsUsing.AllSettingsUsed || PageSettingsEqual(report, pageSettings))
				return;
			IComponentChangeService svc = (IComponentChangeService)host.GetService(typeof(IComponentChangeService));
			DesignerTransaction transaction = host.CreateTransaction(DesignSR.PageSettings);
			try {
				XRControlDesignerBase.RaiseComponentChanging(svc, report, "Margins");
				XRControlDesignerBase.RaiseComponentChanging(svc, report, "Landscape");
				XRControlDesignerBase.RaiseComponentChanging(svc, report, "PaperKind");
				((IReport)report).UpdatePageSettings(pageSettings, pageSettings.PaperName);
				XRControlDesignerBase.RaiseComponentChanged(svc, report);
			} catch {
				transaction.Cancel();
			} finally {
				transaction.Commit();
			}		
		}
		static bool PageSettingsEqual(XtraReport report, DevExpress.XtraPrinting.XtraPageSettingsBase sourceSettings) {
			Margins margins = XRConvert.ConvertMargins(sourceSettings.Margins, GraphicsDpi.HundredthsOfAnInch, report.Dpi);
			return report.Margins.Equals(margins) &&
				report.PaperKind == sourceSettings.PaperKind &&
				report.PaperName == sourceSettings.PaperName &&
				report.Landscape == sourceSettings.Landscape;
		}
		private void BeforeCreateDocument() {
			new DesignerHostExtensions(host).CommitInplaceEditors();
			DisposePreviewReport();
			previewReport = CloneReport();
			RaisePreviewReportCreated();
			DesignPrintingSystemExtender extender = new DesignPrintingSystemExtender(previewReport.PrintingSystem, printControl);
			IPrintingSystemExtender sourceExtender = this.report.PrintingSystem.Extender as IPrintingSystemExtender;
			if(sourceExtender != null)
				extender.CommandSet.CopyFrom(sourceExtender.CommandSet);
			previewReport.PrintingSystem.Extender = extender;
			previewReport.PrintingSystem.CreateDocumentException += new DevExpress.XtraPrinting.ExceptionEventHandler(PrintingSystem_CreateDocumentException);
			previewReport.PrintingSystem.SetCommandVisibility(PrintingSystemCommand.ClosePreview, DevExpress.XtraPrinting.CommandVisibility.None);
			previewReport.PrintingSystem.SetCommandVisibility(PrintingSystemCommand.Open, DevExpress.XtraPrinting.CommandVisibility.None);
			printControl.PrintingSystem = previewReport.PrintingSystem;
			previewReport.ParametersRequestSubmit += previewReport_ParametersRequestSubmit;
			if(previewReport.ExportOptions.Html.ExportMode == HtmlExportMode.DifferentFiles)
				previewReport.ExportOptions.Html.ExportMode = HtmlExportMode.SingleFilePageByPage;
			report.DataContext.Clear();
		}
		void previewReport_ParametersRequestSubmit(object sender, Parameters.ParametersRequestEventArgs e) {
			exceptionWasShown = false;
		}
		void PerformReportActions(params Action[] actions) {
			foreach(Action action in actions)
				if(previewReport != null) action();
		}
		protected virtual void CreateDocument() {
			previewReport.ValidateScripts(new string[] { new DesignerHostExtensions(host).RootReference });
#if DEBUGTEST
			previewReport.CreateDocument(this.buildPagesInBackground);
#else
			previewReport.CreateDocument(true);
#endif
		}
		void SubscrAfterPrint() {
			if(previewReport.Document.IsCreated)
				previewReport_AfterPrint(previewReport, EventArgs.Empty);
			else
				previewReport.AfterPrint += new EventHandler(previewReport_AfterPrint);
		}
		void PrintingSystem_CreateDocumentException(object sender, DevExpress.XtraPrinting.ExceptionEventArgs args) {
			StopPageBuilding();
			if(args.Handled)
				return;
			args.Handled = true;
			ShowException(args.Exception);
		}
		protected virtual XtraReport CloneReport() {
			IComponentSource compSource = XRSerializer.ComponentSource;
			try {
				if(!DesignToolHelper.IsEndUserDesigner(report.Site))
					XRSerializer.ComponentSource = new DesignComponentSource(report.Site);
				XtraReport clone = CloneReportCore();
				IApplicationPathService service = host.GetService(typeof(IApplicationPathService)) as IApplicationPathService;
				if(service != null)
					clone.PrintingSystem.AddService(typeof(IApplicationPathService), service);
				IConnectionProviderService connectionService = host.GetService(typeof(IConnectionProviderService)) as IConnectionProviderService;
				if(connectionService != null)
					clone.PrintingSystem.AddService(typeof(IConnectionProviderService), connectionService);
				ISolutionTypesProvider typesProvider = host.GetService(typeof(ISolutionTypesProvider)) as ISolutionTypesProvider;
				if(typesProvider != null)
					clone.PrintingSystem.AddService(typeof(ISolutionTypesProvider), typesProvider);
				clone.CopyDataProperties(report);
				return clone;
			} finally {
				XRSerializer.ComponentSource = compSource;
			}
		}
		protected XtraReport CloneReportCore() {
			report.SaveComponents += report_SaveComponents;
			try {
				XtraReport clone = report.CloneReport();
				clone.IterateReportsRecursive(delegate(XtraReportBase item) {
					item.ReportPrintOptions.ActivateDesignTimeProperties(true);
				});
				return clone;
			} finally {
				report.SaveComponents -= report_SaveComponents;
			}
		}
		void report_SaveComponents(object sender, SaveComponentsEventArgs e) {
			for(int i = e.Components.Count - 1; i >= 0; i--) {
				if(e.Components[i] is DataComponentBase)
					e.Components.RemoveAt(i);
			}
		}
		private void UpdateBrowserView(string path) {
			try {
				if(browser != null && !string.IsNullOrEmpty(path)) {
					previewReport.ExportToHtml(path);
					browser.Navigate(path);
				}
			} catch(Exception ex) {
				DevExpress.XtraPrinting.Tracer.TraceError(NativeSR.TraceSource, ex);
				StopPageBuilding();
				ShowException(ex);
			}
		}
		void ClearHtmlBrowser() {
			if(browser!= null && browser.Document != null)
				browser.DocumentText = "<html></html>";
		}
		protected override void OnPageActivate(object sender, EventArgs e) {
			IMenuCommandService menuCommandService = (IMenuCommandService)host.GetService(typeof(IMenuCommandService));
			if(menuCommandService != null)
				menuCommandService.GlobalInvoke(CommandIDReportCommandConverter.GetCommandID(((TabPage)sender).Command));
		}
		void OnReportCommandChanged(object sender, ReportCommandEventArgs e) {
			UpdatePages();
		}
		void UpdatePages() {
			if(reportCommandService == null) return;
			for(int i = 0; i < Pages.Count; i ++) {
				Pages[i].Visible = showTabButtons && (reportCommandService.GetCommandVisibility(Pages[i].Command) & UserDesigner.CommandVisibility.Toolbar) > 0;
			}
		}
		void ShowException(Exception ex) {
			if(!exceptionWasShown) {
				exceptionWasShown = true;
#if DEBUGTEST
				DevExpress.XtraReports.Tests.DesignTests.ReportMessageBoxForTests reportMessageBox = serviceProvider.GetService(typeof(DevExpress.XtraReports.Tests.DesignTests.ReportMessageBoxForTests)) as DevExpress.XtraReports.Tests.DesignTests.ReportMessageBoxForTests;
				if(reportMessageBox != null)
					reportMessageBox.ShowException(ex);
				else
#endif
					NotificationService.ShowException<XtraReport>(LookAndFeel, FindForm(), ex);
			}
		}
		void StopPageBuilding() {
			if(PrintingSystem != null) {
				PrintingSystem.Document.StopPageBuilding();
				PrintingSystem.ProgressReflector.Reset();
			}
		}
	}
	[ToolboxItem(false)]
	public class DesignBarStaticItem : BarStaticItem {
		public bool Visible { get { return Visibility == BarItemVisibility.Always; } 
			set {
				if(value)
					Visibility = BarItemVisibility.Always;
				else
					Visibility = BarItemVisibility.Never;
			}
		}
	}
	[ToolboxItem(false)]
	public class BarZoomItem : DesignBarStaticItem {
		ZoomService zoomService;
		public BarZoomItem(IServiceProvider provider) {
			AutoSize = BarStaticItemSize.None;
			Width = 110;
			zoomService = ZoomService.GetInstance(provider);
			UpdateCaption();
			zoomService.ZoomChanged += new EventHandler(OnZoomChanged);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(zoomService != null) {
					zoomService.ZoomChanged -= new EventHandler(OnZoomChanged);
					zoomService = null;
				}
			}
			base.Dispose(disposing);
		}
		void OnZoomChanged(object sender, EventArgs e) {
			UpdateCaption();
		}
		void UpdateCaption() {
			Caption = string.Format(ReportLocalizer.GetString(ReportStringId.UD_Capt_ZoomFactor), zoomService.ZoomFactorInPercents);
		}
	}
	[ToolboxItem(false)]
	public class BarStatusItem : DesignBarStaticItem {
		public BarStatusItem() {
			AutoSize = BarStaticItemSize.Spring;
		}
		public void UpdateStatus(string status) {
			Caption = status;
		}
	}
	[ToolboxItem(false)]
	public class WebBrowser : System.Windows.Forms.WebBrowser {
		enum MiscCommandTarget { Find = 1, ViewSource, Options }
		static Guid CGID_ShellDocView = new Guid("ED016940-BD5B-11CF-BA4E-00C04FD70816");
		#region Interface definition
		[StructLayout(LayoutKind.Sequential)]
		struct OLECMD {
			[MarshalAs(UnmanagedType.U4)]
			public int cmdID;
			[MarshalAs(UnmanagedType.U4)]
			public int cmdf;
		}
		[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("B722BCCB-4E68-101B-A2BC-00AA00404770"), ComVisible(true)]
		interface IOleCommandTarget {
			[return: MarshalAs(UnmanagedType.I4)]
			[PreserveSig]
			int QueryStatus(ref Guid pguidCmdGroup, int cCmds, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] OLECMD[] prgCmds, [In, Out] IntPtr pCmdText);
			[return: MarshalAs(UnmanagedType.I4)]
			[PreserveSig]
			int Exec(ref Guid pguidCmdGroup, int nCmdID, int nCmdexecopt, [In, MarshalAs(UnmanagedType.LPArray)] object[] pvaIn, object pvaOut);
		}
		#endregion
		public void ShowFindDialog() { 
			object o = new object();
			((IOleCommandTarget)Document.DomDocument).Exec(ref CGID_ShellDocView, (int)MiscCommandTarget.Find, 0, null, o);
		}
	}	
}
namespace DevExpress.XtraReports.Native {
	using DevExpress.XtraReports.Design;
	public class DesignerHostExtensions {
		IDesignerHost designerHost;
		public DesignerHostExtensions(IDesignerHost designerHost) {
			this.designerHost = designerHost;
		}
		public void CommitInplaceEditors() {
			if(designerHost == null) return;
			ISelectionService service = designerHost.GetService(typeof(ISelectionService)) as ISelectionService;
			if(service == null) return;
			foreach(IComponent c in service.GetSelectedComponents()) {
				XRTextControlBaseDesigner designer = designerHost.GetDesigner(c) as XRTextControlBaseDesigner;
				if(designer != null) designer.CommitInplaceEditor();
			}
		}
		public string RootReference {
			get {
				try {
					Type type = designerHost.GetType(designerHost.RootComponentClassName);
					return type != null ? type.Assembly.Location : string.Empty;
				} catch {
					return string.Empty;
				}
			}
		}
		public System.CodeDom.Compiler.CompilerErrorCollection ValidateReportScripts() {
			if(designerHost != null) {
				XtraReport report = designerHost.RootComponent as XtraReport;
				if(report != null && !report.ReportIsLoading && !string.IsNullOrEmpty(report.ScriptsSource)) {
					return report.ValidateScripts(new string[] { RootReference });
				}
			}
			return null;
		}
	}
}
