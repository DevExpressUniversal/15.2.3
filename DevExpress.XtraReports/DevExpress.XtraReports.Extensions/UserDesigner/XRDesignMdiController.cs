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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.DataAccess.Wizard;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.Utils.Design;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Design.Commands;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner.Native;
using DevExpress.XtraReports.Wizards3;
using DevExpress.XtraReports.Wizards3.Builder;
using DevExpress.DataAccess.UI.Native.Sql;
using DevExpress.DataAccess.UI.Wizard;
namespace DevExpress.XtraReports.UserDesigner {
	[Designer("DevExpress.XtraReports.Design.XRDesignMdiControllerDesigner, " + AssemblyInfo.SRAssemblyReportsDesignFull, typeof(IDesigner))]
	[ToolboxBitmap(typeof(LocalResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "XRDesignMdiController.bmp")]
	[ToolboxItem(false)]
	[InitAssemblyResolver]
	public class XRDesignMdiController : Component, IServiceContainer, INestedServiceProvider {
		#region static
		static bool IsRootComponent(IComponent component) {
			if(component == null || component.Site == null)
				return false;
			IDesignerHost designerHost = (IDesignerHost)component.Site.GetService(typeof(IDesignerHost));
			return (designerHost.RootComponent == component);
		}
		#endregion
		#region inner classes
		class MdiCommandHandler : ICommandHandler {
			readonly XRDesignMdiController mdiController;
			public MdiCommandHandler(XRDesignMdiController xrDesignMdiController) {
				this.mdiController = xrDesignMdiController;
			}
			#region ICommandHandler Members
			public void HandleCommand(ReportCommand command, object[] args) {
				switch(command) {
					case ReportCommand.Exit:
						if(mdiController.Form != null)
							mdiController.Form.Close();
						break;
					case ReportCommand.Close:
						mdiController.CloseActivePanel();
						break;
					case ReportCommand.NewReport:
						mdiController.CreateNewReport();
						break;
					case ReportCommand.NewReportWizard:
						mdiController.CreateNewReportWizard();
						break;
					case ReportCommand.OpenFile:
						UserLookAndFeel lookAndFeel = mdiController.Form is ISupportLookAndFeel ? ((ISupportLookAndFeel)mdiController.Form).LookAndFeel : UserLookAndFeel.Default;
						new TryCatchHelper(lookAndFeel, mdiController.Form).ExecuteAction(() => mdiController.OpenReport());
						break;
					case ReportCommand.SaveAll:
						mdiController.SaveAllReports();
						mdiController.InvalidateDesigners();
						if(mdiController.ActiveDesignPanel != null)
							mdiController.ActiveDesignPanel.Activate();
						break;
					case ReportCommand.MdiCascade:
						this.mdiController.Form.LayoutMdi(MdiLayout.Cascade);
						break;
					case ReportCommand.MdiTileHorizontal:
						this.mdiController.Form.LayoutMdi(MdiLayout.TileHorizontal);
						break;
					case ReportCommand.MdiTileVertical:
						this.mdiController.Form.LayoutMdi(MdiLayout.TileVertical);
						break;
					case ReportCommand.ShowTabbedInterface:
						if(this.mdiController.XtraTabbedMdiManager != null) {
							this.mdiController.XtraTabbedMdiManager.SetView(ViewType.Tabbed);
							this.mdiController.UpdateCommandStatus();
						}
						break;
					case ReportCommand.ShowWindowInterface:
						if(this.mdiController.XtraTabbedMdiManager != null) {
							this.mdiController.XtraTabbedMdiManager.SetView(ViewType.NativeMdi);
							this.mdiController.UpdateCommandStatus();
						}
						break;
					case ReportCommand.Closing:
						if(this.mdiController.SaveChangedReport(args) == DialogResult.Cancel && args != null && args.Length > 1 && args[1] is CancelEventArgs)
							((CancelEventArgs)args[1]).Cancel = true;
						else
							this.mdiController.InvalidateDesigners();
						break;
					case ReportCommand.OpenSubreport:
						((IWindowsService)mdiController.mdiServices).EditSubreport((XRSubreport)args[0]);
						break;
					default:
						break;
				}
			}
			public bool CanHandleCommand(ReportCommand command, ref bool useNextHandler) {
				bool canHandle = CanHandleCommandCore(command);
				if(canHandle)
					useNextHandler = false;
				return canHandle;
			}
			bool CanHandleCommandCore(ReportCommand command) {
				switch(command) {
					case ReportCommand.Exit:
						return mdiController.Form != null;
					case ReportCommand.NewReport:
					case ReportCommand.NewReportWizard:
					case ReportCommand.OpenFile:
					case ReportCommand.Closing:
					case ReportCommand.ShowTabbedInterface:
					case ReportCommand.ShowWindowInterface:
					case ReportCommand.OpenSubreport:
						return true;
					case ReportCommand.SaveAll:
					case ReportCommand.Close:
						return this.mdiController.MdiChildrenCount >= 1;
					case ReportCommand.MdiCascade:
					case ReportCommand.MdiTileHorizontal:
					case ReportCommand.MdiTileVertical:
						return this.mdiController.MdiChildrenCount >= 1 &&
							this.mdiController.XtraTabbedMdiManager != null &&
							this.mdiController.XtraTabbedMdiManager.View != null &&
							this.mdiController.XtraTabbedMdiManager.View.Type == ViewType.NativeMdi;
					default:
						return false;
				}
			}
			#endregion
		}
		class RibbonPagesHelper {
			XRDesignRibbonController xrDesignRibbonController;
			Dictionary<IXRDesignPanelContainer, DevExpress.XtraBars.Ribbon.RibbonPage> pages = new Dictionary<IXRDesignPanelContainer, DevExpress.XtraBars.Ribbon.RibbonPage>();
			IXRDesignPanelContainer xrDesignPanelContainer;
			public RibbonPagesHelper(XRDesignRibbonController xrDesignRibbonController) {
				this.xrDesignRibbonController = xrDesignRibbonController;
			}
			public void OnMdiChildActivate(IXRDesignPanelContainer activeMdiChild) {
				if(activeMdiChild == null)
					return;
				if(xrDesignPanelContainer == null)
					xrDesignPanelContainer = activeMdiChild;
				DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl = xrDesignRibbonController.RibbonControl;
				pages[xrDesignPanelContainer] = ribbonControl.SelectedPage;
				DevExpress.XtraBars.Ribbon.RibbonPage savedPage;
				if(pages.TryGetValue(activeMdiChild, out savedPage))
					ribbonControl.SelectedPage = savedPage;
				else
					if(!(ribbonControl.SelectedPage is XRDesignRibbonPage)) {
						XRDesignRibbonPage designerPage = ribbonControl.Pages.OfType<XRDesignRibbonPage>().FirstOrDefault();
						if(designerPage != null)
							ribbonControl.SelectedPage = designerPage;
					}
				xrDesignPanelContainer = activeMdiChild;
			}
		}
		#endregion
		Form form;
		ContainerControl containerControl;
		Form containerControlForm;
		XRDesignPanel activeDesignPanel;
		XRDesignPanelListenersCollection designPanelListenersCollection;
		RibbonPagesHelper ribbonPagesHelper;
		MdiCommandHandler mdiCommandHandler;
		XRTabbedMdiManager xtraTabbedMdiManager;
		delegate void XRDesignPanelHandler(XRDesignPanel xrDesignPanel);
		int mdiChildrenCount;
		MdiServices mdiServices;
		ServiceContainer serviceContainer;
		Container components;
		ActivationService activationService;
		SqlWizardSettings sqlWizardSettings = new SqlWizardSettings();
		static SqlWizardSettings sqlWizardSettingsDefault = new SqlWizardSettings();
		internal IXRDesignPanelContainer ActiveDesignPanelContainer {
			get { return XtraTabbedMdiManager != null ? XtraTabbedMdiManager.ActiveContainer : (Form != null ? Form.ActiveMdiChild as IXRDesignPanelContainer : null); }
		}
		int MdiChildrenCount {
			get { return XtraTabbedMdiManager != null ? XtraTabbedMdiManager.FormsCount : (Form != null ? Form.MdiChildren.Length : 0); }
		}
		IEnumerable<Control> MdiChildren {
			get { return XtraTabbedMdiManager != null ? XtraTabbedMdiManager.GetContainers() : (Form != null ? Array.ConvertAll(Form.MdiChildren, form => (Control)form) : new Control[] { }); }
		}
		Form MdiParent {
			get { return XtraTabbedMdiManager != null ? XtraTabbedMdiManager.MdiParent : null; }
		}
		internal UserLookAndFeel LookAndFeel {
			get {
				return (xtraTabbedMdiManager != null && xtraTabbedMdiManager.BarAndDockingController != null)
					? xtraTabbedMdiManager.BarAndDockingController.LookAndFeel
					: (this is ISupportLookAndFeel)
						? ((ISupportLookAndFeel)this).LookAndFeel
						: null;
			}
		}
		[
		Category("Behavior"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		]
		public SqlWizardSettings SqlWizardSettings {
			get { return sqlWizardSettings; }
		}
		bool ShouldSerializeSqlWizardSettings() {
			return !sqlWizardSettings.Equals(sqlWizardSettingsDefault);
		}
		[
		Browsable(false),
		DefaultValue(null),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)
		]
		public XRTabbedMdiManager XtraTabbedMdiManager {
			get {
				return xtraTabbedMdiManager;
			}
			set {
				if(xtraTabbedMdiManager != value) 
					SetMdiManager(value);
			}
		}
		void ForceMdiManager() {
			if(xtraTabbedMdiManager == null) {
				SetMdiManager(new XRTabbedMdiManager(components));
				xtraTabbedMdiManager.MdiParent = Form;
				xtraTabbedMdiManager.ContainerControl = ContainerControl;
			}
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Browsable(false),
		]
		public XRDesignPanelListenersCollection DesignPanelListeners {
			get { return designPanelListenersCollection; }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Browsable(false),
		]
		public Form Form {
			get { return form; }
			set {
				if(form != value) {
					if(form != null)
						UnsubscribeForm();
					form = value;
					if(form != null)
						SubscribeForm();
				}
			}
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Browsable(false),
		]
		public ContainerControl ContainerControl {
			get { return containerControl; }
			set {
				if(containerControl != value) {
					if(containerControl != null)
						UnsubscribeContainerControl();
					containerControl = value;
					if(containerControl != null)
						SubscribeContainerControl();
				}
			}
		}
		[
		Browsable(false),
		]
		public XRDesignPanel ActiveDesignPanel {
			get { return activeDesignPanel; }
		}
		[Category("Behavior")]
		public event DesignerLoadedEventHandler DesignPanelLoaded;
		[Category("Behavior")]
		public event EventHandler<DocumentEventArgs> AnyDocumentActivated;
		[Category("Behavior")]
		public event EventHandler<ValidateSqlEventArgs> ValidateCustomSql;
		void RaiseValidateCustomSql(ValidateSqlEventArgs e) {
			if(ValidateCustomSql != null)
				ValidateCustomSql(this, e);
		}
		public XRDesignMdiController() {
			InitializeComponent();
		}
		public XRDesignMdiController(IContainer container) {
			DevExpress.Utils.Guard.ArgumentNotNull(container, "container");
			container.Add(this);
			InitializeComponent();
		}
		void InitializeComponent() {
			components = new System.ComponentModel.Container();
			activationService = new ActivationService();
			designPanelListenersCollection = new XRDesignPanelListenersCollection(this);
			serviceContainer = new ServiceContainer();
			mdiCommandHandler = new MdiCommandHandler(this);
			mdiServices = new MdiServices(this);
			ReportCommandServiceBase service = new ReportCommandServiceBase();
			service.AddCommandHandler(mdiCommandHandler);
			serviceContainer.AddService(typeof(ReportCommandServiceBase), service);
			serviceContainer.AddService(typeof(ActivationService), activationService);
			serviceContainer.AddService(typeof(ISqlWizardOptionsProvider), new SqlWizardOptionsProvider(() => SqlWizardSettings.ToSqlWizardOptions()));
			serviceContainer.AddService(typeof(ICustomQueryValidator), new XRCustomQueryValidator(RaiseValidateCustomSql));
		}
		void CreateRibbonPageHelper() {
			if(ribbonPagesHelper == null && !DesignMode)
				foreach(XRDesignPanelListener listener in designPanelListenersCollection) {
					if(listener.DesignControl is XRDesignRibbonController)
						ribbonPagesHelper = new RibbonPagesHelper((XRDesignRibbonController)listener.DesignControl);
				}
		}
		void xtraTabbedMdiManager_AnyDocumentActivated(object sender, DocumentEventArgs e) {
			MdiChildActivate(ActiveDesignPanelContainer);
			if(AnyDocumentActivated != null)
				AnyDocumentActivated(this, e);
		}
		void Form_MdiChildActivate(object sender, EventArgs e) {
			if(XtraTabbedMdiManager == null)
				MdiChildActivate(ActiveDesignPanelContainer);
		}
		void MdiChildActivate(IXRDesignPanelContainer designPanelContainer) {
			if(designPanelContainer == null)
				return;
			if(activeDesignPanel == designPanelContainer.DesignPanel) {
				AssignDesignPanelToListeners(designPanelContainer.DesignPanel);
				UpdateCommandStatus();
				return;
			}
			if(activeDesignPanel != null)
				activeDesignPanel.Deactivate();
			AssignDesignPanelToListeners(designPanelContainer.DesignPanel);
			if(!designPanelContainer.DesignPanel.ReportDesigner.IsActive)
				designPanelContainer.DesignPanel.Activate();
			SaveActiveDesignPanel(designPanelContainer);
			UpdateCommandStatus();
			if(ribbonPagesHelper != null)
				ribbonPagesHelper.OnMdiChildActivate(designPanelContainer);
		}
		void SaveActiveDesignPanel(IXRDesignPanelContainer container) {
			activeDesignPanel = container.DesignPanel;
			XRDesignPanelForm form = container as XRDesignPanelForm;
			if(form != null) {
				form.FormClosed -= new FormClosedEventHandler(ActiveDesignPanelForm_FormClosed);
				form.FormClosed += new FormClosedEventHandler(ActiveDesignPanelForm_FormClosed);
			}
		}
		void ActiveDesignPanelForm_FormClosed(object sender, FormClosedEventArgs e) {
			XRDesignPanelForm form = (XRDesignPanelForm)sender;
			if(activeDesignPanel == form.DesignPanel)
				activeDesignPanel = null;
			form.FormClosed -= new FormClosedEventHandler(ActiveDesignPanelForm_FormClosed);
		}
		void UpdateCommandStatus() {
			if(this.ActiveDesignPanel == null)
				return;
			MenuCommandHandler menuCommandHandler = this.ActiveDesignPanel.GetService(typeof(MenuCommandHandler)) as MenuCommandHandler;
			if(menuCommandHandler != null)
				menuCommandHandler.UpdateCommandStatus();
		}
		public void OpenReport() {
			string result = ReportStorageServiceInteractive.GetNewUrl();
			if(!string.IsNullOrEmpty(result))
				OpenReport(result);
		}
		public void OpenReport(XtraReport report) {
			if(report == null)
				throw new ArgumentException("report");
			IXRDesignPanelContainer container = GetContainerBy(report);
			if(container != null && XtraTabbedMdiManager != null) {
				XtraTabbedMdiManager.Activate(container);
				return;
			}
			CreateNewMdiForm(delegate(XRDesignPanel xrDesignPanel) {
				xrDesignPanel.OpenReport(report);
			});
		}
		IXRDesignPanelContainer GetContainerBy(XtraReport report) {
			return ProcessForms(container => container.Report == report);
		}
		internal IXRDesignPanelContainer GetContainerBy(string fileName) {
			return ProcessForms(container => !string.IsNullOrEmpty(fileName) && string.Equals(fileName, container.DesignPanel.FileName));
		}
		IXRDesignPanelContainer ProcessForms(Predicate<IXRDesignPanelContainer> predicate) {
			foreach(Control control in MdiChildren) {
				IXRDesignPanelContainer container = control as IXRDesignPanelContainer;
				if(container != null && predicate(container))
					return container;
			}
			return null;
		}
		public void OpenReport(string fileName) {
			IXRDesignPanelContainer container = GetContainerBy(fileName);
			if(container != null && XtraTabbedMdiManager != null) {
				XtraTabbedMdiManager.Activate(container);
				return;
			}
			CreateNewMdiForm(delegate(XRDesignPanel xrDesignPanel) {
				xrDesignPanel.OpenReport(fileName);
			});
		}
		public void CreateNewReport() {
			OpenReport(CreateReport());
		}
		public void CreateNewReportWizard() {
			OpenReport(new XtraReport());
			IDesignerHost host = ActiveDesignPanel.GetService(typeof(IDesignerHost)) as IDesignerHost;
			IConnectionStorageService storageService = (IConnectionStorageService)GetService(typeof(IConnectionStorageService));
			XtraReportModel model = new XtraReportModelUI(storageService).CreateReportModel(host);
			if(model == null) return;
			ActiveDesignPanel.BeginInvoke(new MethodInvoker(() => {
				DataComponentCreator.SaveConnectionIfShould(model, storageService);
				UndoEngine undoEngine = host.GetService(typeof(UndoEngine)) as UndoEngine;
				IWizardCustomizationService serv = host.GetService(typeof(IWizardCustomizationService)) as IWizardCustomizationService;
				object dataSource; string dataMember;
				serv.CreateDataSourceSafely(model, out dataSource, out dataMember);
				undoEngine.ExecuteWithoutUndo(() => serv.CreateReportSafely(host, model, dataSource, dataMember));
			}));
		}
		internal XtraReport CreateReport() {
			ReportTypeService reportTypeService = ((IServiceProvider)this).GetService(typeof(ReportTypeService)) as ReportTypeService;
			XtraReport report = reportTypeService != null ? reportTypeService.GetDefaultReport() : XtraReport.GetDefaultReport();
			report.Name = string.Format("Report{0}", GetNextMdiChildNumber());
			return report;
		}
		int GetNextMdiChildNumber() {
			return ++mdiChildrenCount;
		}
		void CreateNewMdiForm(XRDesignPanelHandler xrDesignPanelHandler) {
			ForceMdiManager();
			IXRDesignPanelContainer xrDesignPanelContainer = CreateDesignPanelContainer();
			if(LookAndFeel != null)
				xrDesignPanelContainer.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			xrDesignPanelContainer.DesignPanel.DesignerHostLoaded += new DesignerLoadedEventHandler(DesignPanel_DesignerHostLoaded);
			AssignDesignPanelToListeners(xrDesignPanelContainer.DesignPanel);
			try {
				xrDesignPanelHandler(xrDesignPanelContainer.DesignPanel);
			} catch {
				xrDesignPanelContainer.DesignPanel.DesignerHostLoaded -= new DesignerLoadedEventHandler(DesignPanel_DesignerHostLoaded);
				xrDesignPanelContainer.Dispose();
				throw;
			}
			xrDesignPanelContainer.SynchText();
			IComponentChangeService componentChangeService = xrDesignPanelContainer.DesignPanel.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(componentChangeService != null)
				componentChangeService.ComponentChanged += new ComponentChangedEventHandler(componentChangeService_ComponentChanged);
			mdiServices.AddServices(xrDesignPanelContainer.DesignPanel);
			if(this.Form != null) {
				XRDesignPanelForm xrDesignPanelForm = xrDesignPanelContainer as XRDesignPanelForm;
				xrDesignPanelForm.Icon = Form.Icon;
				xrDesignPanelForm.MdiParent = Form;
				xrDesignPanelForm.WindowState = FormWindowState.Maximized;
			}
			xrDesignPanelContainer.ShowActive();
			MdiChildActivate(xrDesignPanelContainer);
		}
		private IXRDesignPanelContainer CreateDesignPanelContainer() {
			if(this.Form != null)
				return CreateDesignPanelForm();
			return XRDesignPanelContainerControl.Create(this, XtraTabbedMdiManager);
		}
		protected virtual XRDesignPanelForm CreateDesignPanelForm() {
			return new XRDesignPanelForm(this);
		}
		void DesignPanel_DesignerHostLoaded(object sender, DesignerLoadedEventArgs e) {
			CreateRibbonPageHelper();
			((XRDesignPanel)sender).AddService(typeof(INestedServiceProvider), this);
			if(DesignPanelLoaded != null)
				DesignPanelLoaded(sender, e);
		}
		void componentChangeService_ComponentChanged(object sender, ComponentChangedEventArgs e) {
			if(IsRootComponent(e.Component as IComponent) && e.Member != null && e.Member.Name == "DisplayName") {
				ActiveDesignPanelContainer.SynchText();
			}
		}
		void InvalidateDesigners() {
			ProcessForms(delegate(IXRDesignPanelContainer container) {
				container.DesignPanel.InvalidateDesignSurface();
				return false;
			});
		}
		void SaveAllReports() {
			ProcessForms(delegate(IXRDesignPanelContainer container) {
				container.DesignPanel.SaveReport();
				return false;
			});
			ProcessForms(delegate(IXRDesignPanelContainer container) {
				container.DesignPanel.EnsureReportSaved();
				return false;
			});
		}
		DialogResult SaveChangedReport(object[] args) {
			IXRDesignPanelContainer designPanelContainer = FindDesignPanelContainer(args);
			if(designPanelContainer == null)
				return DialogResult.None;
			DialogResult dialogResult = DialogResult.None;
			if(designPanelContainer.DesignPanel.ReportState == ReportState.Changed) {
				dialogResult = ShowSaveChangedReportDialog(designPanelContainer);
				if(dialogResult == DialogResult.Yes) {
					designPanelContainer.DesignPanel.SaveReportUsingCommand();
					if(designPanelContainer.DesignPanel.ReportState == ReportState.Changed)
						dialogResult = DialogResult.Cancel;
				}
			}
			return dialogResult;
		}
#if DEBUGTEST
		public static Func<DialogResult> Test_GetSaveChangedReportResult;
#endif
		static DialogResult ShowSaveChangedReportDialog(IXRDesignPanelContainer designPanelContainer) {
#if DEBUGTEST
			if(Test_GetSaveChangedReportResult != null)
				return Test_GetSaveChangedReportResult();
#endif
			string text = string.Format(ReportLocalizer.GetString(ReportStringId.UD_Msg_MdiReportChanged), designPanelContainer.ReportDisplayName);
			return XtraMessageBox.Show(designPanelContainer.DesignPanel.LookAndFeel, text, ReportLocalizer.GetString(ReportStringId.UD_ReportDesigner),
				MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
		}
		IXRDesignPanelContainer FindDesignPanelContainer(object[] args) {
			if(args == null)
				return null;
			return args.Length >= 1 ? args[0] as IXRDesignPanelContainer : null;
		}
		void AssignDesignPanelToListeners(XRDesignPanel xrDesignPanel) {
			foreach(XRDesignPanelListener designPanelListener in designPanelListenersCollection) {
				if(XtraTabbedMdiManager.MenuManager == null && designPanelListener.DesignControl is IDXMenuManager) {
					XtraTabbedMdiManager.MenuManager = (IDXMenuManager)designPanelListener.DesignControl;
				}
				if(designPanelListener.DesignControl != null)
					designPanelListener.DesignControl.XRDesignPanel = xrDesignPanel;
			}
		}
		void form_FormClosing(object sender, FormClosingEventArgs e) {
			if(!e.Cancel) {
				CancelEventArgs cancelArgs = new CancelEventArgs();
				foreach(Control ctrl in MdiChildren) {
					DoClosing(ctrl as IXRDesignPanelContainer, cancelArgs);
					e.Cancel |= cancelArgs.Cancel; 
				}
			}
		}
		void View_DocumentClosing(object sender, DocumentCancelEventArgs e) {
			DoClosing(e.Document.Control as IXRDesignPanelContainer, e);
		}
		void DoClosing(IXRDesignPanelContainer container, CancelEventArgs e) {
			if(container == null)
				return;
			container.DesignPanel.ExecuteClosing(new object[] { container, e });
		}
		void SetMdiManager(XRTabbedMdiManager value) {
			if(xtraTabbedMdiManager != null)
				UnsubscribeTabbedMdiManager();
			xtraTabbedMdiManager = value;
			if(xtraTabbedMdiManager != null) {
				if(xtraTabbedMdiManager.View == null)
					xtraTabbedMdiManager.SetView(ViewType.Tabbed);
				SubscribeTabbedMdiManager();
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(form != null) {
					UnsubscribeForm();
					form = null;
					ribbonPagesHelper = null;
				}
				if(xtraTabbedMdiManager != null) {
					UnsubscribeTabbedMdiManager();
					xtraTabbedMdiManager = null;
				}
				if(components != null) {
					components.Dispose();
					components = null;
				}
				serviceContainer.Dispose();
			}
			base.Dispose(disposing);
		}
		void SubscribeForm() {
			form.MdiChildActivate += Form_MdiChildActivate;
			form.FormClosing += form_FormClosing;
		}
		void UnsubscribeForm() {
			form.MdiChildActivate -= Form_MdiChildActivate;
			form.FormClosing -= form_FormClosing;
		}
		void SubscribeTabbedMdiManager() {
			xtraTabbedMdiManager.AnyDocumentActivated += xtraTabbedMdiManager_AnyDocumentActivated;
			if(xtraTabbedMdiManager.View != null)
				xtraTabbedMdiManager.View.DocumentClosing += View_DocumentClosing;
		}
		void UnsubscribeTabbedMdiManager() {
			xtraTabbedMdiManager.AnyDocumentActivated -= xtraTabbedMdiManager_AnyDocumentActivated;
			if(xtraTabbedMdiManager.View != null)
				xtraTabbedMdiManager.View.DocumentClosing -= View_DocumentClosing;
		}
		void SubscribeContainerControl() {
			containerControlForm = containerControl.FindForm();
			containerControl.ParentChanged += containerControl_ParentChanged;
			if(containerControlForm != null)
				containerControlForm.FormClosing += form_FormClosing;
		}
		void UnsubscribeContainerControl() {
			containerControl.ParentChanged -= containerControl_ParentChanged;
			if(containerControlForm != null) {
				containerControlForm.FormClosing -= form_FormClosing;
				containerControlForm = null;
			}
		}
		void containerControl_ParentChanged(object sender, EventArgs e) {
			if(containerControlForm != null)
				containerControlForm.FormClosing -= form_FormClosing;
			containerControlForm = containerControl.FindForm();
			if(containerControlForm != null)
				containerControlForm.FormClosing += form_FormClosing;
		}
		#region report commands
		public void AddCommandHandler(ICommandHandler handler) {
			ReportCommandServiceBase servBase = serviceContainer.GetService(typeof(ReportCommandServiceBase)) as ReportCommandServiceBase;
			if(servBase != null)
				servBase.AddCommandHandler(handler);
		}
		public void RemoveCommandHandler(ICommandHandler handler) {
			ReportCommandServiceBase servBase = serviceContainer.GetService(typeof(ReportCommandServiceBase)) as ReportCommandServiceBase;
			if(servBase != null)
				servBase.RemoveCommandHandler(handler);
		}
		public void SetCommandVisibility(ReportCommand command, CommandVisibility visibility) {
			ReportCommandServiceBase servBase = serviceContainer.GetService(typeof(ReportCommandServiceBase)) as ReportCommandServiceBase;
			if(servBase != null)
				servBase.SetCommandVisibility(command, visibility);
		}
		public void SetCommandVisibility(ReportCommand[] commands, CommandVisibility visibility) {
			ReportCommandServiceBase servBase = serviceContainer.GetService(typeof(ReportCommandServiceBase)) as ReportCommandServiceBase;
			if(servBase != null)
				servBase.SetCommandVisibility(commands, visibility);
		}
		public CommandVisibility GetCommandVisibility(ReportCommand command) {
			ReportCommandServiceBase servBase = serviceContainer.GetService(typeof(ReportCommandServiceBase)) as ReportCommandServiceBase;
			return servBase != null ? servBase.GetCommandVisibility(command) : CommandVisibility.None;
		}
		#endregion
		#region IServiceContainer Members
		public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote) {
			serviceContainer.AddService(serviceType, callback, promote);
		}
		public void AddService(Type serviceType, ServiceCreatorCallback callback) {
			serviceContainer.AddService(serviceType, callback);
		}
		public void AddService(Type serviceType, object serviceInstance, bool promote) {
			serviceContainer.AddService(serviceType, serviceInstance, promote);
		}
		public void AddService(Type serviceType, object serviceInstance) {
			serviceContainer.AddService(serviceType, serviceInstance);
		}
		public void RemoveService(Type serviceType, bool promote) {
			serviceContainer.RemoveService(serviceType, promote);
		}
		public void RemoveService(Type serviceType) {
			serviceContainer.RemoveService(serviceType);
		}
		object IServiceProvider.GetService(Type serviceType) {
			return serviceContainer.GetService(serviceType);
		}
		#endregion
		protected override object GetService(Type service) {
			return ((IServiceContainer)this).GetService(service);
		}
		internal void CloseActivePanel() {
			if(XtraTabbedMdiManager != null)
				XtraTabbedMdiManager.View.Controller.Close(XtraTabbedMdiManager.View.ActiveDocument);
			else if(Form != null)
				Form.ActiveMdiChild.Close();
		}
	}
}
namespace DevExpress.XtraReports.UserDesigner.Native {
	using System.IO;
	public interface IWindowsService {
		void EditSubreport(XRSubreport subreport);
		bool ShouldSaveSubreport(XRSubreport subreport);
		string GetReportSourceDisplayName(XRSubreport subreport);
	}
	public class ActivationService {
		public int ReportCount { get; set; }
		public DevExpress.XtraReports.Design.ReportDesigner ActiveDesigner { get; set; }
	}
	class MdiServices : IWindowsService {
		readonly XRDesignMdiController mdiController;
		readonly Dictionary<XRSubreport, IXRDesignPanelContainer> containerDictionary = new Dictionary<XRSubreport, IXRDesignPanelContainer>();
		public MdiServices(XRDesignMdiController mdiController) {
			this.mdiController = mdiController;
		}
		bool IWindowsService.ShouldSaveSubreport(XRSubreport subreport) {
			if(subreport == null)
				return false;
			IXRDesignPanelContainer container = GetContainer(subreport);
			return container != null ? container.DesignPanel.ReportState == ReportState.Changed : false;
		}
		string IWindowsService.GetReportSourceDisplayName(XRSubreport subreport) {
			if(subreport == null)
				return string.Empty;
			IXRDesignPanelContainer container = GetContainer(subreport);
			return container != null ? container.ReportDisplayName : string.Empty;
		}
		void IWindowsService.EditSubreport(XRSubreport subreport) {
			if(subreport == null)
				return;
			IXRDesignPanelContainer container = GetContainer(subreport);
			if(container == null) {
				XtraReport report = CreateReport(subreport.ReportSource, subreport.ReportSourceUrl);
				if(report == null)
					return;
				mdiController.OpenReport(report);
				container = mdiController.ActiveDesignPanelContainer;
				if(container != null) {
					containerDictionary[subreport] = container;
					string fileName = subreport.ReportSourceUrl;
					if(ReportStorageService.IsValidUrl(fileName))
						container.DesignPanel.FileName = fileName;
					container.DesignPanel.FileNameChanged += new EventHandler(panel_FileNameChanged);
				}
			} else {
				if(mdiController.XtraTabbedMdiManager != null)
					mdiController.XtraTabbedMdiManager.Activate(container);
			}
		}
		IXRDesignPanelContainer GetContainer(XRSubreport subreport) {
			IXRDesignPanelContainer container = mdiController.GetContainerBy(subreport.ReportSourceUrl);
			if(container == null)
				containerDictionary.TryGetValue(subreport, out container);
			return container != null && !container.IsDisposed ? container : null;
		}
		XtraReport CreateReport(XtraReport reportSource, string reportSourceUrl) {
			byte[] buffer = ReportStorageService.IsValidUrl(reportSourceUrl) ? ReportStorageService.GetData(reportSourceUrl) : new byte[] { };
			if(buffer.Length > 0 && reportSource == null) {
				using(Stream stream = new MemoryStream(buffer)) {
					return XtraReport.FromStream(stream, true);
				}
			} else if(buffer.Length > 0 && reportSource != null) {
				using(Stream stream = new MemoryStream(buffer)) {
					XtraReport result = (XtraReport)Activator.CreateInstance(reportSource.GetType());
					result.LoadLayout(stream);
					return result;
				}
			} else if(buffer.Length == 0 && reportSource != null) {
				XtraReport result = (XtraReport)Activator.CreateInstance(reportSource.GetType());
				CopyReportLayout(reportSource, result);
				return result;
			} else {
				if(string.IsNullOrEmpty(reportSourceUrl))
					return mdiController.CreateReport();
				else {
					IDesignerHost designerHost = mdiController.ActiveDesignPanel.GetService<IDesignerHost>();
					DialogResult dialogResult = XtraMessageBox.Show(DevExpress.LookAndFeel.DesignService.DesignLookAndFeelHelper.GetLookAndFeel(designerHost),
						ReportLocalizer.GetString(ReportStringId.UD_Msg_ReportSourceUrlNotFound),
						ReportLocalizer.GetString(ReportStringId.UD_ReportDesigner),
						 MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
					return dialogResult == DialogResult.Yes ? mdiController.CreateReport() : null;
				}
			}
		}
		void panel_FileNameChanged(object sender, EventArgs e) {
			SynchReportSourceUrl();
		}
		void SynchReportSourceUrl() {
			Dictionary<XRSubreport, IXRDesignPanelContainer>.Enumerator enumerator = containerDictionary.GetEnumerator();
			while(enumerator.MoveNext()) {
				SetReportSourceUrl(enumerator.Current.Key, enumerator.Current.Value.DesignPanel.FileName);
			}
		}
		static void SetReportSourceUrl(XRSubreport subreport, string value) {
			if(subreport.ReportSourceUrl != value) {
				PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(XRSubreport))["ReportSourceUrl"];
				System.Diagnostics.Debug.Assert(property != null);
				property.SetValue(subreport, value);
			}
		}
		static void CopyReportLayout(XtraReport source, XtraReport dest) {
			using(Stream stream = new MemoryStream()) {
				SaveReportLayout(source, stream);
				dest.LoadLayout(stream);
			}
		}
		static void SaveReportLayout(XtraReport report, Stream stream) {
			ISite site = report.Site;
			try {
				report.Site = null;
				report.SaveLayout(stream);
			} finally {
				report.Site = site;
			}
		}
		public void AddServices(XRDesignPanel panel) {
			panel.AddService(typeof(IWindowsService), this);
		}
	}
}
