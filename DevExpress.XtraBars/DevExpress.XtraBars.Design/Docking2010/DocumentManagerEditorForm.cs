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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using DevExpress.Utils.Design;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraBars.Docking2010.Views;
namespace DevExpress.XtraBars.Design {
	[ToolboxItem(false)]
	public partial class DocumentManagerEditorForm : BaseDesignerForm {
		public const string DocumentManagerSettings = "Software\\Developer Express\\Designer\\DocumentManager\\";
		BaseDocumentManagerDesigner emptyDesigner;
		BaseDocumentManagerDesigner defaultDesigner;
		IDictionary<ViewType, BaseDocumentManagerDesigner> designers;
		public DocumentManagerEditorForm() {
			InitializeComponent();
			this.emptyDesigner = new EmptyViewDesigner();
			emptyDesigner.Init();
			this.defaultDesigner = new DefaultDesigner();
			defaultDesigner.Init();
			designers = new Dictionary<ViewType, BaseDocumentManagerDesigner>();
			designers.Add(ViewType.Tabbed, new TabbedViewDesigner());
			designers.Add(ViewType.NativeMdi, new NativeMdiViewDesigner());
			designers.Add(ViewType.WindowsUI, new WindowsUIViewDesigner());
			designers.Add(ViewType.Widget, new WidgetViewDesigner());
			ProductInfo = new DevExpress.Utils.About.ProductInfo("DocumentManager", typeof(DocumentManager), DevExpress.Utils.About.ProductKind.DXperienceWin, DevExpress.Utils.About.ProductInfoStage.Registered);
		}
		protected override BaseDesigner ActiveDesigner {
			get {
				if(DocumentManagerInfo != null && DocumentManagerInfo.SelectedObject is DocumentManager) 
					return defaultDesigner;
				if(CurrentView == null)
					return emptyDesigner;
				return designers[CurrentView.Type];
			}
		}
		protected override void OnXF_RefreshWizard(object sender, DevExpress.XtraEditors.Designer.Utils.RefreshWizardEventArgs e) {
			if(e.Condition == "ChangedView") {
				RefreshDesigner(false);
			}
		}
		protected void RefreshDesigner(bool updateActiveFrame) {
			InitNavBar(updateActiveFrame);
			EnabledMainItems();
		}
		protected void EnabledMainItems() {
			for(int i = 0; i < NavBar.Items.Count; i++) {
				DesignerItem item = NavBar.Items[i].Tag as DesignerItem;
				if(item != null && !string.IsNullOrEmpty(item.Caption) && item.Caption != "Views")
					NavBar.Items[i].Enabled = CurrentView != null;
			}
		}
		public virtual void InitDocumentManager(DocumentManager manager) {
			InitDocumentManager(manager, string.Empty, null);
		}
		EditingDocumentManagerInfo documentManagerInfoCore;
		public virtual void InitDocumentManager(DocumentManager manager, string itemLinkCaption, BaseView view) {
			Initialize();
			this.documentManagerInfoCore = new EditingDocumentManagerInfo(manager);
			if(view != null) DocumentManagerInfo.SelectedObject = view;
			InitEditingObject(manager, itemLinkCaption);
			EnabledMainItems();
		}
		protected override void OnClosed(EventArgs e) {
			base.OnClosed(e);
		}
		protected override void InitFrame(string caption, Bitmap bitmap) {
			XF.InitFrame(CurrentView, DocumentManagerInfo, caption, bitmap);
		}
		public virtual EditingDocumentManagerInfo DocumentManagerInfo { 
			get { return documentManagerInfoCore; } 
		}
		protected string CurrentViewName {
			get {
				if(CurrentView == null) return "";
				else return CurrentView.Type.ToString();
			}
		}
		protected DocumentManager EditingDocumentManager { 
			get { return DocumentManagerInfo == null ? null : DocumentManagerInfo.EditingDocumentManager; } 
		}
		protected BaseView CurrentView {
			get { return DocumentManagerInfo == null ? null : DocumentManagerInfo.SelectedView; }
		}
		#region Windows Form Designer generated code
		#endregion
		void eActiveView_EditValueChanged(object sender, System.EventArgs e) {
			if(DocumentManagerInfo == null) return;
			RefreshDesigner(true);
		}
		protected override string RegistryStorePath { 
			get { return DocumentManagerSettings; } 
		}
		protected override Type ResolveType(string type) {
			return typeof(DocumentManagerEditorForm).Assembly.GetType(type) ?? base.ResolveType(type);
		}
	}
	public class EditingDocumentManagerInfo : ISelectionService {
		object selectedObject;
		DocumentManager editingObject;
		public EditingDocumentManagerInfo(DocumentManager editingObject) {
			this.editingObject = editingObject;
			this.selectedObject = EditingDocumentManager == null ? null : EditingDocumentManager.View;
		}
		public virtual DocumentManager EditingDocumentManager {
			get { return editingObject; }
		}
		public virtual BaseView SelectedView {
			get { return SelectedObject as BaseView; }
		}
		public virtual object SelectedObject {
			get { return selectedObject; }
			set {
				if(SelectedObject == value) return;
				if(SelectionChanging != null) SelectionChanging(this, EventArgs.Empty);
				this.selectedObject = value;
				if(SelectionChanged != null) SelectionChanged(this, EventArgs.Empty);
			}
		}
		bool ISelectionService.GetComponentSelected(object component) { 
			return component == SelectedObject; 
		}
		ICollection ISelectionService.GetSelectedComponents() {
			if(SelectedObject == null) return null;
			return new object[] { SelectedObject };
		}
		object ISelectionService.PrimarySelection {
			get { return SelectedObject; }
		}
		int ISelectionService.SelectionCount { get { return SelectedObject == null ? 0 : 1; } }
		void ISelectionService.SetSelectedComponents(ICollection components, SelectionTypes types) {
			((ISelectionService)this).SetSelectedComponents(components);
		}
		void ISelectionService.SetSelectedComponents(ICollection components) {
			if(components == null || components.Count == 0) SelectedObject = null;
			else {
				foreach(object sel in components) {
					SelectedObject = sel;
					break;
				}
			}
		}
		public event EventHandler SelectionChanged, SelectionChanging;
	}
}
