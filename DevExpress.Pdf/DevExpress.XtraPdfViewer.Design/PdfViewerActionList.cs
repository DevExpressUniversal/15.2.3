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
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Design;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Commands.Design;
using DevExpress.XtraPdfViewer.Bars;
using DevExpress.XtraPdfViewer.Native;
using DevExpress.XtraPdfViewer.Localization;
namespace DevExpress.XtraPdfViewer.Design {
	public class PdfViewerActionList : DesignerActionList {
		static RibbonPage FindPageInternal(RibbonPageCollection ribbonPages, Type pageType) {
			foreach (RibbonPage page in ribbonPages)
				if (page.GetType() == pageType)
					return page;
			return null;
		}
		static RibbonPage FindPage(RibbonControl ribbonControl, Type pageType) {
			RibbonPage page = FindPageInternal(ribbonControl.Pages, pageType);
			if (page != null)
				return page;
			foreach (RibbonPageCategory pageCategory in ribbonControl.PageCategories) {
				page = FindPageInternal(pageCategory.Pages, pageType);
				if (page != null)
					return page;
			}
			return null;
		}
		readonly PdfViewer control;
		ISite site;
		IDesignerHost designerHost;
		IComponentChangeService componentChangeService;
		DesignerActionUIService designerActionUIService;
		IDesignerHost DesignerHost {
			get {
				EnsureServices();
				return designerHost;
			}
		}
		IComponentChangeService ComponentChangeService {
			get {
				EnsureServices();
				return componentChangeService;
			}
		}
		DesignerActionUIService DesignerActionUIService {
			get {
				EnsureServices();
				return designerActionUIService;
			}
		}
		public PdfViewerActionList(PdfViewer control) : base(control) {
			this.control = control;			
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection collection = new DesignerActionItemCollection();
			collection.Add(new DesignerActionMethodItem(this, "LoadPdf", XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.DesignerActionMethodLoadPdf), "File"));
			if (!String.IsNullOrEmpty(control.DocumentFilePath))
				collection.Add(new DesignerActionMethodItem(this, "UnloadPdf", XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.DesignerActionMethodUnloadPdf), "File"));
			if (control != null && control.Container != null && DesignerHost != null && ComponentChangeService != null) {
				collection.Add(new DesignerActionMethodItem(this, "CreateRibbon", XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.DesignerActionMethodCreateRibbon), "Bars"));
				collection.Add(new DesignerActionMethodItem(this, "CreateBars", XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.DesignerActionMethodCreateBars), "Bars"));
			}
			DesignerActionMethodItem item = control.Dock == DockStyle.Fill ?
				new DesignerActionMethodItem(this, "UndockInParentContainer", XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.DesignerActionMethodUndockInParentContainer), "Layout") :
				new DesignerActionMethodItem(this, "DockInParentContainer", XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.DesignerActionMethodDockInParentContainer), "Layout");
			collection.Add(item);
			return collection;
		}
		void EnsureServices() {
			if (site == null) {
				site = control.Site;
				if (site != null) {
					componentChangeService = site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
					designerHost = site.GetService(typeof(IDesignerHost)) as IDesignerHost;
					designerActionUIService = site.GetService(typeof(DesignerActionUIService)) as DesignerActionUIService;
				}
			}
		}
		void PerformChangingOperation(object component, Action action) {
			IComponentChangeService componentChangeService = ComponentChangeService;
			if (componentChangeService != null)
				componentChangeService.OnComponentChanging(component, null);		
			action();
			if (componentChangeService != null)
				componentChangeService.OnComponentChanged(component, null, null, null);		
		}
		void PerformPdfLoading(string path) {
			PerformChangingOperation(control, () => control.DocumentFilePath = path);
			DesignerActionUIService designerActionUIService = DesignerActionUIService;
			if (designerActionUIService != null)
				designerActionUIService.Refresh(control);
		}
		void LoadPdf() {  
			PerformPdfLoading(PdfFileChooser.ChooseFile(String.Empty, control.DocumentFilePath));
		}
		void UnloadPdf() {
			PerformPdfLoading(String.Empty);
		}
		T FindComponent<T>() where T : class {
			foreach (IComponent component in control.Container.Components) {
				T comp = component as T;
				if (comp != null)
					return comp;
			}
			return null;
		}
		void CreateBarInternal(Type componentType) {
			IDesignerHost designerHost = DesignerHost;
			if (designerHost != null)
				new PdfDesignTimeBarsGenerator(designerHost, control, componentType).AddNewBars(PdfBarsUtils.BarCreators, String.Empty, BarInsertMode.Add);
		}
		void CreateRibbon() {
			IDesignerHost designerHost = DesignerHost;
			if (designerHost != null) {
				string transactionName = XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.DesignerActionListCreateRibbonTransaction);
				using (DesignerTransaction transaction = designerHost.CreateTransaction(transactionName)) {
					Control parent = control.Parent;
					RibbonControl ribbon = FindComponent<RibbonControl>();
					if (ribbon == null) {
						ribbon = (RibbonControl)designerHost.CreateComponent(typeof(RibbonControl));
						BarAndDockingController controller = FindComponent<BarAndDockingController>();
						if (controller != null) {
							ISupportLookAndFeel lookAndFeelControl = control as ISupportLookAndFeel;
							if (lookAndFeelControl != null)
								PerformChangingOperation(controller, () => controller.LookAndFeel.ParentLookAndFeel = lookAndFeelControl.LookAndFeel);
							PerformChangingOperation(ribbon, () =>  ribbon.Controller = controller);
						}
						PdfBarsUtils.SetupRibbon(ribbon);  
						PerformChangingOperation(parent, () => parent.Controls.Add(ribbon));
					}
					RibbonForm ribbonForm = parent as RibbonForm;
					if (ribbonForm != null) 
						PerformChangingOperation(ribbonForm, () => ribbonForm.Ribbon = ribbon);
					CreateBarInternal(typeof(RibbonControl));
					RibbonPage page = FindPage(ribbon, typeof(PdfRibbonPage));
					if (page != null)
						ribbon.SelectedPage = page;
					transaction.Commit();
				 }
				 EditorContextHelperEx.RefreshSmartPanel(control);			
			}
		}
		void CreateBars() {
			string transactionName = XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.DesignerActionListCreateBarsTransaction);
			using (DesignerTransaction transaction = designerHost.CreateTransaction(transactionName)) {
				IContainer container = control.Container;
				BarManager barManager = FindComponent<BarManager>();
				if (barManager == null) {
					barManager = (BarManager)designerHost.CreateComponent(typeof(BarManager));
					PerformChangingOperation(container, () => container.Add(barManager));
				}
				PerformChangingOperation(barManager, () => barManager.Form = control.ParentForm);
				PerformChangingOperation(control, () => control.MenuManager = barManager);
				CreateBarInternal(typeof(BarManager));
				transaction.Commit();
			}
			EditorContextHelperEx.RefreshSmartPanel(control);			
		}
		void DockInParentContainer() {
			control.Dock = DockStyle.Fill;
			DesignerActionUIService designerActionUIService = DesignerActionUIService;
			if (designerActionUIService != null)
				designerActionUIService.Refresh(control);
		}
		void UndockInParentContainer() {
			control.Dock = DockStyle.None;
			DesignerActionUIService designerActionUIService = DesignerActionUIService;
			if (designerActionUIService != null)
				designerActionUIService.Refresh(control);
		}
	}
}
