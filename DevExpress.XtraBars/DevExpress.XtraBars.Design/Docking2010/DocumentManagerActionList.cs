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

using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.Utils.Design;
namespace DevExpress.XtraBars.Design {
	public class DocumentManagerActionList : DesignerActionList {
		DocumentManagerDesigner designerCore;
		public DocumentManagerActionList(DocumentManagerDesigner designer)
			: base(designer.Component) {
			this.designerCore = designer;
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			if(Manager.ViewCollection.Count == 0) {
				res.Add(new DesignerActionMethodItem(this, "CreateView", "Create View", "View"));
				res.Add(new DesignerActionMethodItem(this, "RunDesigner", "Run Designer", "Designer"));
			}
			else {
				res.Add(new DesignerActionHeaderItem("Current View"));
				res.Add(new DesignerActionPropertyItem("View", "Choose", "Current View"));				
				if(Manager.View != null) {
					var type = Manager.View.Type;
					if(type != Docking2010.Views.ViewType.Tabbed)
						res.Add(new DesignerActionMethodItem(this, "ChangeTypeToTabbed", "Convert To TabbedView", "Current View"));
					if(type != Docking2010.Views.ViewType.NativeMdi)
						res.Add(new DesignerActionMethodItem(this, "ChangeTypeToNativeMdi", "Convert To NativeMdiView", "Current View"));
					if(type != Docking2010.Views.ViewType.WindowsUI)
						res.Add(new DesignerActionMethodItem(this, "ChangeTypeToWindowsUI", "Convert To WindowsUIView", "Current View"));
				if(type != Docking2010.Views.ViewType.Widget)
					res.Add(new DesignerActionMethodItem(this, "ChangeTypeToWidget", "Convert To WidgetView", "Current View"));
					if(Designer.DockManager == null)
						res.Add(new DesignerActionMethodItem(this, "AddDockManager", "Add DockManager", "CreateElements"));
					if(Designer.BarManager != null && Designer.CanAddWindowMenu())
						res.Add(new DesignerActionMethodItem(this, "AddWindowMenu", "Add Window Menu", "CreateElements"));
					if(type != Docking2010.Views.ViewType.NoDocuments)
						res.Add(new DesignerActionMethodItem(this, "RunDesigner", "Run Designer", "Designer", true));
				}
			}
			res.Add(new DesignerActionMethodItem(this, "About", "About", null, true));
			return res;
		}
		protected DocumentManagerDesigner Designer {
			get { return designerCore; }
		}
		protected Docking2010.DocumentManager Manager {
			get { return Component as Docking2010.DocumentManager; }
		}
		public void About() {
			Docking2010.DocumentManager.About();
		}
		public void RunDesigner() {
			Designer.RunDesigner();
		}
		[RefreshProperties(RefreshProperties.All)]
		public void AddDockManager() {
			Designer.AddDockManager();
		}
		[RefreshProperties(RefreshProperties.All)]
		public void AddWindowMenu() {
			Designer.AddWindowMenu();
		}
		[RefreshProperties(RefreshProperties.All)]
		public void CreateView() {
			AddNew();
		}
		[RefreshProperties(RefreshProperties.All)]
		public void AddNew() {
			AddNewCore(GetViewType(ViewType));
			EditorContextHelperEx.RefreshSmartPanel(Component);
		}
		[RefreshProperties(RefreshProperties.All)]
		public void ChangeTypeToTabbed() {
			ChangeTypeCore(Docking2010.Views.ViewType.Tabbed);
		}
		[RefreshProperties(RefreshProperties.All)]
		public void ChangeTypeToNativeMdi() {
			ChangeTypeCore(Docking2010.Views.ViewType.NativeMdi);
		}
		[RefreshProperties(RefreshProperties.All)]
		public void ChangeTypeToWindowsUI() {
			ChangeTypeCore(Docking2010.Views.ViewType.WindowsUI);
			System.Windows.Forms.Form mdiParent = Manager.MdiParent;
			if(mdiParent != null) {
				Manager.BeginUpdate();
				Manager.ContainerControl = mdiParent;
				mdiParent.IsMdiContainer = false;
				Manager.EndUpdate();
			}
		}
		[RefreshProperties(RefreshProperties.All)]
		public void ChangeTypeToWidget() {
			ChangeTypeCore(Docking2010.Views.ViewType.Widget);
		}
		void ChangeTypeCore(Docking2010.Views.ViewType type) {
			Docking2010.Views.BaseView view = Manager.View;
			AddNewCore(type);
			view.Dispose();
			EditorContextHelperEx.RefreshSmartPanel(Component);
		}
		protected void AddNewCore(Docking2010.Views.ViewType type) {
			Designer.CreateView(type);
		}
		[RefreshProperties(RefreshProperties.All)]
		public Docking2010.Views.BaseView View {
			get { return Manager.View; }
			set {
				EditorContextHelper.SetPropertyValue(Designer, Component, "View", value);
				EditorContextHelperEx.RefreshSmartPanel(Component);
			}
		}
		DesignerViewType typeCore;
		public DesignerViewType ViewType {
			get { return typeCore; }
			set { typeCore = value; }
		}
		public Docking2010.Views.ViewType GetViewType(DesignerViewType type) {
			return (Docking2010.Views.ViewType)((int)type);
		}
		public enum DesignerViewType {
			Tabbed, NativeMdi, WindowsUI,
			Widget
		}
	}
}
