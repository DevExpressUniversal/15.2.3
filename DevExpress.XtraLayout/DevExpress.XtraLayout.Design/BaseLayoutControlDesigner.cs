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
using System.Reflection;
using System.Collections;
using System.Drawing;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.Utils.Design;
using System.Windows.Forms.Design.Behavior;
namespace DevExpress.XtraLayout.DesignTime {
	public abstract class BaseLayoutControlDesigner : BaseParentControlDesigner {
		IToolboxService toolboxService;
		IComponentChangeService componentChangeService;
		IDesignerHost designerHost;
		ISelectionService selectionService;
		IToolboxService toolBoxService;
		IMenuCommandService menuCommandService;
		protected DesignerVerbCollection defaultVerbs;
		public BaseLayoutControlDesigner() {
			defaultVerbs = new DesignerVerbCollection(
				new DesignerVerb[] {
					new DesignerVerb("About", new EventHandler(OnAboutClick)),
					new DesignerVerb("Show Customization", new EventHandler(OnShowCustomizationClick)),
				}
			);
		}
#if DXWhidbey
		protected override bool AllowHookDebugMode { get { return true; } }
#endif
		public override DesignerVerbCollection DXVerbs {
			get {
				if(!AllowDesigner) return null;
				return defaultVerbs;
			}
		}
		protected void OnAboutClick(object sender, EventArgs e) {
			LayoutControl.About();
		}
		protected void OnShowCustomizationClick(object sender, EventArgs e) {
			ShowCustomization(true);
		}
		protected abstract void SelectedComponentsChanged(object sender, EventArgs ea);
		protected abstract void SelectedComponentsChanging(object sender, EventArgs ea);
		public ISelectionService SelectionService {
			get {
				if (selectionService == null) {
					selectionService = (ISelectionService)GetService(typeof(ISelectionService));
					if (selectionService != null) selectionService.SelectionChanged += new EventHandler(SelectedComponentsChanged);
					if (selectionService != null) selectionService.SelectionChanging += new EventHandler(SelectedComponentsChanging);
				}
				return selectionService;
			}
		}
		public void RefreshSelectionManager(){
			Type scd = typeof(ScrollableControlDesigner);
			Type selectionManagerType = null;
			FieldInfo fieldInfo = scd.GetField("selManager", BindingFlags.Instance | BindingFlags.NonPublic);
			if (fieldInfo != null) { selectionManagerType = fieldInfo.FieldType; }
			if (selectionManagerType != null) {
				object selManagerInstance = GetService(selectionManagerType);
				if (selManagerInstance != null) {
					selectionManagerType.InvokeMember("Refresh", BindingFlags.InvokeMethod, null, selManagerInstance, new object[] { });
					Component.Invalidate();
					Component.Update();
				}
			}
		}
		protected virtual void UnSubscribeEvents() {
			if (selectionService != null) {
				selectionService.SelectionChanged -= new EventHandler(SelectedComponentsChanged);
				selectionService.SelectionChanging -= new EventHandler(SelectedComponentsChanging);
			}
		}
		protected IToolboxService ToolBoxService {
			get {
				if (toolBoxService == null) {
					toolBoxService = (IToolboxService)GetService(typeof(IToolboxService));
				}
				return toolBoxService;
			}
		}
		protected IMenuCommandService MenuCommandService {
			get {
				if (menuCommandService == null) {
					menuCommandService = (IMenuCommandService)GetService(typeof(IMenuCommandService));
				}
				return menuCommandService;
			}
		}
		protected IComponentChangeService ComponentChangeService {
			get {
				if (componentChangeService == null) {
					componentChangeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
				}
				return componentChangeService;
			}
		}
		public IDesignerHost DesignerHost {
			get {
				if (designerHost == null) designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
				return designerHost;
			}
		}
		protected internal IToolboxService ToolboxService {
			get {
				if (toolboxService == null) toolboxService = (IToolboxService)GetService(typeof(IToolboxService));
				return toolboxService;
			}
		}
		public new LayoutControl Component {
			get {
				return (LayoutControl)base.Component;
			}
		}
		public virtual ArrayList SelectedComponents {
			get {
				return new ArrayList(SelectionService.GetSelectedComponents());
			}
		}
		protected virtual void OnAbout(object sender, EventArgs e) {
			LayoutControl.About();
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			if (AllowDesigner)
				list.Add(new LayoutControlDesignerActionList(this));
			base.RegisterActionLists(list);
		}
		protected override bool DrawGrid {
			get { return false; }
		}
		public bool CanShowCustomization() {
			return Component != null && (Component.CustomizationForm == null || !Component.CustomizationForm.Visible);
		}
		public void ShowCustomization(bool value) {
			if(Component != null) {
				if(value) Component.ShowCustomizationForm();
				else Component.HideCustomizationForm();
			}
		}
	}
	public class LayoutControlDesignerActionList : DesignerActionList {
		BaseLayoutControlDesigner designer;
		public LayoutControlDesignerActionList(BaseLayoutControlDesigner designer)
			: base(designer.Component) {
			this.designer = designer;
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			if (designer == null || designer.Component == null) return res;
			res.Add(new DesignerActionPropertyItem("Dock", "Choose DockStyle"));
			res.Add(new DesignerActionPropertyItem("ShowCustomization", "Show Customization"));
			return res;
		}
		public LayoutControl Control { get { return designer.Component; } }
		public DockStyle Dock {
			get {
				if (Control == null) return DockStyle.None;
				return Control.Dock;
			}
			set {
				DevExpress.Utils.Design.EditorContextHelper.SetPropertyValue(designer, Control, "Dock", value);
			}
		}
		public bool ShowCustomization {
			get { return !designer.CanShowCustomization(); }
			set { designer.ShowCustomization(value); }
		}
	}
}
