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
using System.Drawing;
using System.ComponentModel;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Preview;
using DevExpress.XtraPrinting.Preview.Native;
using DevExpress.XtraReports.UserDesigner.Native;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraReports.Design;
using DevExpress.XtraBars.Docking;
using System.Collections;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.Design.Commands;
using System.ComponentModel.Design;
using System.Reflection;
using DevExpress.Utils.Menu;
namespace DevExpress.XtraReports.UserDesigner {
	static class RibbonExtensions {
		public static RibbonControl GetActualRibbon(this RibbonControl ribbon) {
			return ribbon.MergeOwner != null ? GetActualRibbon(ribbon.MergeOwner) : ribbon;
		}
		public static RibbonPage GetPage(this RibbonControl ribbon, Predicate<RibbonPage> predicate) {
			return GetPageFromCategoryCollection(ribbon.MergedCategories, predicate) ?? GetPageFromCategoryCollection(ribbon.PageCategories, predicate) ??
				GetPageFromPageCollection(ribbon.MergedPages, predicate) ?? GetPageFromPageCollection(ribbon.Pages, predicate);
		}
		static RibbonPage GetPageFromCategoryCollection(RibbonPageCategoryCollection categoryCollection, Predicate<RibbonPage> predicate) {
			foreach(RibbonPageCategory category in categoryCollection) {
				RibbonPage result = GetPageFromCategory(category, predicate);
				if(result != null) return result;
			}
			return null;
		}
		static RibbonPage GetPageFromCategory(RibbonPageCategory category, Predicate<RibbonPage> predicate) {
			return GetPageFromPageCollection(category.MergedPages, predicate) ?? GetPageFromPageCollection(category.Pages, predicate);
		}
		static RibbonPage GetPageFromPageCollection(RibbonPageCollection pageCollection, Predicate<RibbonPage> predicate) {
			foreach(RibbonPage page in pageCollection) {
				if(predicate(page))
					return page;
			}
			return null;
		}
		public static int GetPageIndex(this RibbonPageCollection pages, Predicate<RibbonPage> predicate) {
			for(int i = 0; i < pages.Count; i++)
				if(predicate(pages[i]))
					return i;
			return -1;
		}
	}
	public class XRDesignRibbonPage : RibbonPage {
		public XRDesignRibbonPage() {
		}
		protected override RibbonPage CreatePage() {
			return new XRDesignRibbonPage();
		}
	}
	public class XRHtmlRibbonPage : RibbonPage {
		public XRHtmlRibbonPage() {
		}
		protected override RibbonPage CreatePage() {
			return new XRHtmlRibbonPage();
		}
	}
	public class XRToolboxRibbonPage : RibbonPage {
		public XRToolboxRibbonPage() {
		}
		protected override RibbonPage CreatePage() {
			return new XRToolboxRibbonPage();
		}
	}
	public class XRToolboxPageCategory : RibbonPageCategory {
		public XRToolboxPageCategory() {
		}
		protected override RibbonPageCategory CreateRibbonPageCategory() {
			return new XRToolboxPageCategory();
		}
	}
	public class XRDesignBarButtonGroup : DevExpress.XtraBars.BarButtonGroup {
	}
	public enum XRDesignRibbonPageGroupKind {
		Report,
		Edit,
		Font,
		Alignment,
		SizeAndLayout,
		Zoom,
		View,
		Scripts,
		HtmlNavigation
	}
	public class XRDesignRibbonPageGroup : RibbonPageGroup, ControllerRibbonPageGroupKind<XRDesignRibbonPageGroupKind> {
		XRDesignRibbonPageGroupKind kind;
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public XRDesignRibbonPageGroupKind Kind {
			get { return kind; } 
			set { kind = value; } 
		}
	}
	[
	Designer("DevExpress.XtraReports.Design.XRDesignRibbonControllerDesigner," + AssemblyInfo.SRAssemblyReportsDesignFull, typeof(System.ComponentModel.Design.IDesigner)),
	ToolboxBitmap(typeof(LocalResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "XRDesignRibbonController.bmp"),
	Description("Allows you create an End-User Report Designer using a RibbonControl."),
	ToolboxItem(false),
	]
	public class XRDesignRibbonController : RibbonControllerBase, IDesignControl, IDesignPanelListener, IWeakServiceProvider, IDXMenuManager {
		class XRPrintRibbonController : PrintRibbonController {
			public DesignPreviewRibbonItemsLogic RibbonItemsLogic {
				get { return (DesignPreviewRibbonItemsLogic)PrintItemsLogic; }
			}
			public XRPrintRibbonController(object contextSpecifier)
				: base(contextSpecifier) {
			}
			protected override RibbonPreviewItemsLogic CreateItemsLogic() {
				return new DesignPreviewRibbonItemsLogic(RibbonControl.Manager, this.ContextSpecifier);
			}
		}
		class DesignPreviewRibbonItemsLogic : RibbonPreviewItemsLogic {
			public DesignPreviewRibbonItemsLogic(RibbonBarManager manager, object contextSpecifier)
				: base(manager, contextSpecifier) {
			}
			protected override void InitializeProgressReflector() {
				if(GetBarItemByStatusPanelID(Manager, StatusPanelID.Progress) != null)
					base.InitializeProgressReflector();
			}
			public void EnableCommand(bool value, params PrintingSystemCommand[] commands) {
				CommandSet.EnableCommand(value, commands); 
			}
		}
		XRDesignPanel designPanel;
		XRDesignRibbonItemsLogic designItemsLogic;
		XRPrintRibbonController printRibbonController;
		XRDesignDockManager dockManager;
		bool scriptsVisibilty;
#if !SL
	[DevExpressXtraReportsExtensionsLocalizedDescription("XRDesignRibbonControllerXRDesignPanel")]
#endif
		public XRDesignPanel XRDesignPanel {
			get { return designItemsLogic != null ? designItemsLogic.XRDesignPanel : designPanel; }
			set {
				if(designPanel != null)
					UnSubscribeDesignPanelEvents();
				designPanel = value;
				if(designItemsLogic != null)
					designItemsLogic.XRDesignPanel = value;
				if(designPanel != null)
					SubscribeDesignPanelEvents();
			}
		}
#if !SL
	[DevExpressXtraReportsExtensionsLocalizedDescription("XRDesignRibbonControllerRibbonControl")]
#endif
		public override RibbonControl RibbonControl {
			get { return base.RibbonControl; }
			set {
				if(RibbonControl != null) {
					RibbonControl actualRibbon = RibbonControl.GetActualRibbon();
					actualRibbon.SelectedPageChanging -= new RibbonPageChangingEventHandler(RibbonControl_SelectedPageChanging);
					actualRibbon.SelectedPageChanged -= new EventHandler(RibbonControl_SelectedPageChanged);
					actualRibbon.Merge -= new RibbonMergeEventHandler(RibbonControl_Merge);
				}
				base.RibbonControl = value;
				if(value != null) {
					printRibbonController.RibbonControl = RibbonControl;
					RibbonControl actualRibbon = RibbonControl.GetActualRibbon();
					actualRibbon.SelectedPageChanging += new RibbonPageChangingEventHandler(RibbonControl_SelectedPageChanging);
					actualRibbon.SelectedPageChanged += new EventHandler(RibbonControl_SelectedPageChanged);
					actualRibbon.Merge += new RibbonMergeEventHandler(RibbonControl_Merge);
					RibbonControl.Manager.DockManager = dockManager;
				}
			}
		}
		[Browsable(false)]
		public XRDesignDockManager XRDesignDockManager {
			get { return RibbonControl != null ? RibbonControl.Manager.DockManager as XRDesignDockManager : dockManager; }
			set { 
				dockManager = value;
				if(RibbonControl != null)
					RibbonControl.Manager.DockManager = dockManager; 
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RibbonImageCollection ImageCollection {
			get { return printRibbonController.ImageCollection; }
		}
		bool ShouldSerializeImageCollection() {
			return ImageCollection.Count > 0;
		}
		void ResetImageCollection() {
			ImageCollection.Clear();
		}
		internal PrintRibbonController PrintRibbonController { 
			get { return printRibbonController; }
		}
		public XRDesignRibbonController() {
			printRibbonController = new XRPrintRibbonController(this);
		}
		public XRDesignRibbonController(IContainer container) {
			Guard.ArgumentNotNull(container, "container");
			container.Add(this);
			printRibbonController = new XRPrintRibbonController(this);
		}
		internal void SetBarItemLocked(BarItem item, bool locked) {
			printRibbonController.RibbonItemsLogic.SetItemLocked(item, locked);
		}
		internal BarStaticItem GetBarItemBy(StatusPanelID id) {
			return printRibbonController.RibbonItemsLogic.GetPanelByID(id);
		}
		internal BarItem GetBarItemBy(PrintingSystemCommand command) {
			return printRibbonController.RibbonItemsLogic.GetBarItemByCommand(command);
		}
		internal void EnableCommand(PrintingSystemCommand command, bool enabled) {
			printRibbonController.RibbonItemsLogic.EnableCommand(enabled, command);
			printRibbonController.UpdateCommands();
		}
		protected internal override Dictionary<string, Image> GetImagesFromAssembly() {
			return RibbonControllerHelper.JoinImageDictionaries(XRDesignRibbonControllerConfigurator.GetImagesFromAssembly(), PrintRibbonControllerConfigurator.GetImagesFromAssembly());
		}
		protected internal override void ConfigureRibbonController(RibbonControl ribbonControl, RibbonStatusBar ribbonStatusBar, Dictionary<string, Image> images) {
			(new XRDesignRibbonControllerConfigurator(ribbonControl, ribbonStatusBar, images)).Configure(null);
			printRibbonController.RibbonStatusBar = ribbonStatusBar;
			printRibbonController.ConfigureRibbonController(ribbonControl, ribbonStatusBar, images);
			MovePreviewPage();
		}
		void MovePreviewPage() {
			int designPageIndex = RibbonControl.Pages.GetPageIndex(item => item is XRDesignRibbonPage);
			int previewPageIndex = RibbonControl.Pages.GetPageIndex(item => item is PrintPreviewRibbonPage);
			if(designPageIndex == -1 || previewPageIndex == -1) return;
			RibbonPage previewPage = RibbonControl.Pages[previewPageIndex];
			RibbonControl.Pages.RemoveAt(previewPageIndex);
			RibbonControl.Pages.Insert(designPageIndex + 1, previewPage);
		}
		public override void BeginInit() {
			printRibbonController.BeginInit();
		}
		public override void EndInit() {
			if(designItemsLogic == null && RibbonControl != null) {
				designItemsLogic = new XRDesignRibbonItemsLogic(RibbonControl.Manager, this);
				designItemsLogic.XRDesignPanel = designPanel;
				designItemsLogic.EndInit();
				XRDesignRibbonControllerConfigurator.LocalizeStrings(RibbonControl);
				printRibbonController.EndInit();
				RibbonControl.ForceInitialize();
				if(XRDesignDockManager != null)
					SetTemporaryDockPanelsVisibility(
						RibbonControl.GetActualRibbon().SelectedPage ??
						(RibbonControl.GetActualRibbon().Pages.Count > 0 ? RibbonControl.GetActualRibbon().Pages[0] : null));
			}
		}
		[
		Obsolete("The RegisterCommandHandler method is obsolete now. Use the XRDesignMdiController.AddCommandHandler method instead."),
		]
		public void RegisterCommandHandler(ICommandHandler commandHandler) {
		}
		[
		Obsolete("The UnregisterCommandHandler method is obsolete now. Use the XRDesignMdiController.RemoveCommandHandler method instead."),
		]
		public void UnregisterCommandHandler(ICommandHandler commandHandler) {
		}
		protected override ICollection<IDisposable> GetObjectsToDispose() {
			List<IDisposable> objectsToDispose = new List<IDisposable>();
			foreach(RibbonPage page in RibbonControl.Pages) {
				if(page is XRDesignRibbonPage || page is XRHtmlRibbonPage)
					objectsToDispose.Add(page);
			}
			foreach(RibbonPageCategory category in RibbonControl.PageCategories) {
				if(category is XRToolboxPageCategory) {
					foreach(RibbonPage page in category.Pages)
						if(page is XRToolboxRibbonPage)
							objectsToDispose.Add(page);
					if((category.Pages.Count == 0) || (category.Pages.Count == 1 && category.Pages[0] is XRToolboxRibbonPage)) objectsToDispose.Add(category);
				}
			}
			foreach(DevExpress.XtraBars.BarItem item in RibbonControl.Items) {
				if(item is ISupportReportCommand || item is XRDesignBarButtonGroup || item is BarDockPanelsListItem)
					objectsToDispose.Add(item);
			}
			AddEditBarItemToDestroy(objectsToDispose, XRDesignRibbonControllerConfigurator.GetFontNameEditor(RibbonControl.Manager));
			AddEditBarItemToDestroy(objectsToDispose, XRDesignRibbonControllerConfigurator.GetFontSizeEditor(RibbonControl.Manager));
			return objectsToDispose;
		}
		protected override void Dispose(bool disposing) {
			try {
				if(disposing) {
					if(designPanel != null)
						UnSubscribeDesignPanelEvents();
					if(RibbonControl != null) {
						RibbonControl actualRibbon = RibbonControl.GetActualRibbon();
						actualRibbon.SelectedPageChanging -= new RibbonPageChangingEventHandler(RibbonControl_SelectedPageChanging);
						actualRibbon.SelectedPageChanged -= new EventHandler(RibbonControl_SelectedPageChanged);
						actualRibbon.Merge -= new RibbonMergeEventHandler(RibbonControl_Merge);
					}
					printRibbonController.Dispose();
				}
			} finally {
				base.Dispose(disposing);
			}
		}
		void SubscribeDesignPanelEvents() {
			designPanel.Activated += new EventHandler(designPanel_Activated);
			designPanel.DesignerHostLoading += new EventHandler(designPanel_DesignerHostLoading);
			designPanel.SelectedTabIndexChanged += new EventHandler(designPanel_SelectedTabIndexChanged);
		}
		void designPanel_Activated(object sender, EventArgs e) {
			UpdateScriptsButton(designPanel.SelectedTabIndex);
		}
		void designPanel_DesignerHostLoading(object sender, EventArgs e) {
			designPanel.AddService(typeof(TabControlLogicCreator), new TabControlRibbonLogicCreator(this));
		}
		void UnSubscribeDesignPanelEvents() {
			designPanel.Activated -= new EventHandler(designPanel_Activated);
			designPanel.DesignerHostLoading -= new EventHandler(designPanel_DesignerHostLoading);
			designPanel.SelectedTabIndexChanged -= new EventHandler(designPanel_SelectedTabIndexChanged);
		}
		static void AddEditBarItemToDestroy(List<IDisposable> objectsToDispose, BarEditItem item) {
			if(item != null) {
				objectsToDispose.Add(item);
				objectsToDispose.Add(item.Edit);
			}
		}
		void designPanel_SelectedTabIndexChanged(object sender, EventArgs e) {
			UpdateScriptsButton(designPanel.SelectedTabIndex);
		}
		void RibbonControl_SelectedPageChanging(object sender, RibbonPageChangingEventArgs e) {
			if(designPanel == null || designItemsLogic == null)
				return;
			ReportTabControl tabControl = designPanel.GetService(typeof(ReportTabControl)) as ReportTabControl;
			if(tabControl == null)
				return;
			if(e.Page is XRDesignRibbonPage) {
				ReportCommand currentReportCommand = scriptsVisibilty ? ReportCommand.ShowScriptsTab : ReportCommand.ShowDesignerTab;
				designPanel.ExecCommand(currentReportCommand);
				e.Cancel = tabControl.SelectedPageCommand != currentReportCommand;
			} else if(e.Page is XRHtmlRibbonPage) {
				designPanel.ExecCommand(ReportCommand.ShowHTMLViewTab);
				e.Cancel = tabControl.SelectedPageCommand != ReportCommand.ShowHTMLViewTab;
			} else if(e.Page is PrintPreviewRibbonPage && ((ISupportContextSpecifier)e.Page).HasSameContext(this)) {
				designPanel.ExecCommand(ReportCommand.ShowPreviewTab);
				e.Cancel = tabControl.SelectedPageCommand != ReportCommand.ShowPreviewTab;
			}
		}
		void RibbonControl_Merge(object sender, RibbonMergeEventArgs e) {
			RibbonControl actualRibbon = e.MergeOwner.GetActualRibbon();
			actualRibbon.SelectedPageChanging -= new RibbonPageChangingEventHandler(RibbonControl_SelectedPageChanging);
			actualRibbon.SelectedPageChanged -= new EventHandler(RibbonControl_SelectedPageChanged);
			actualRibbon.SelectedPageChanging += new RibbonPageChangingEventHandler(RibbonControl_SelectedPageChanging);
			actualRibbon.SelectedPageChanged += new EventHandler(RibbonControl_SelectedPageChanged);
		}
		void RibbonControl_SelectedPageChanged(object sender, EventArgs e) {
			if(XRDesignDockManager != null && RibbonControl != null)
				SetTemporaryDockPanelsVisibility(RibbonControl.GetActualRibbon().SelectedPage);
			UpdateRibbonElementsVisibility();
		}
		public void SetTemporaryDockPanelsVisibility(RibbonPage selectedRibbonPage) {
			bool visible = (selectedRibbonPage is XRDesignRibbonPage) || (selectedRibbonPage is XRToolboxRibbonPage);
			XRDesignDockManager.SetTemporaryDockPanelsVisibility(visible);
		}
		void UpdateRibbonElementsVisibility() {
			if(designPanel == null || RibbonControl == null) return;
			RibbonControl actualRibbon = RibbonControl.GetActualRibbon();
			RibbonPage page = actualRibbon.GetPage(item =>item is XRToolboxRibbonPage);
			if(page == null) return;
			if(page.Groups.Count > 0) {
				page.Category.Visible = (actualRibbon.SelectedPage is XRDesignRibbonPage) || (actualRibbon.SelectedPage is XRToolboxRibbonPage);
				page.Visible = page.Category.Visible;
			}	   
		}
		void UpdateScriptsButton(int selectedTabIndex) {
			if(this.designItemsLogic == null)
				return;
			if(selectedTabIndex == TabIndices.Scripts) {
				scriptsVisibilty = true;
				BarItem[] items = designItemsLogic.GetBarItemsByReportCommand(ReportCommand.ShowScriptsTab);
				foreach(ScriptsCommandBarItem item in EnumScriptItems(items))
					item.UpdateCommand(ReportCommand.ShowDesignerTab);
			} else if(selectedTabIndex == TabIndices.Designer) {
				scriptsVisibilty = false;
				BarItem[] items = designItemsLogic.GetBarItemsByReportCommand(ReportCommand.ShowDesignerTab);
				foreach(ScriptsCommandBarItem item in EnumScriptItems(items))
					item.UpdateCommand(ReportCommand.ShowScriptsTab);
			}
		}
		IEnumerable<ScriptsCommandBarItem> EnumScriptItems(BarItem[] items) {
			if(items != null) {
				foreach(BarItem item in items)
					if(item is ScriptsCommandBarItem)
						yield return (ScriptsCommandBarItem)item;
			}
		}
		IServiceProvider serviceProvider;
		IServiceProvider IWeakServiceProvider.ServiceProvider {
			get { return serviceProvider; }
			set {
				serviceProvider = value;
				if(designItemsLogic != null && this.Site == null)
					designItemsLogic.UpdateBarItems();
			}
		}
		object IServiceProvider.GetService(Type serviceType) {
			return serviceProvider != null ? serviceProvider.GetService(serviceType) : null;
		}
		IDXMenuManager IDXMenuManager.Clone(System.Windows.Forms.Form newForm) {
			if(RibbonControl != null)
				return ((IDXMenuManager)RibbonControl.Manager).Clone(newForm);
			return null;
		}
		void IDXMenuManager.DisposeManager() {
			((IDXMenuManager)RibbonControl.Manager).DisposeManager();
		}
		void IDXMenuManager.ShowPopupMenu(DXPopupMenu menu, System.Windows.Forms.Control control, Point pos) {
			((IDXMenuManager)RibbonControl.Manager).ShowPopupMenu(menu, control, pos);
		}
	}
}
namespace DevExpress.XtraReports.UserDesigner.Native {
	public static class XRDesignRibbonControllerHelper {
		public static PrintRibbonController GetPrintRibbonController(XRDesignRibbonController designRibbonController) {
			return designRibbonController.PrintRibbonController;
		}
	}
}
