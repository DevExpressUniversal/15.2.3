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
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.XtraNavBar.Utils;
using DevExpress.XtraNavBar.ViewInfo;
namespace DevExpress.XtraNavBar.Design {
	public class NavBarComponentDesigner : BaseComponentDesigner {
		protected override bool AllowEditInherited { get { return false; } }
	}
	public class NavBarItemDesigner : NavBarComponentDesigner {
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new SmartDesignerActionList(this, Component));
		}
	}
	public class NavBarGroupDesigner : NavBarComponentDesigner {
		protected override bool AllowInheritanceWrapper { get { return true; } }
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new NavBarGroupActionList(this));
		}
	}
	public class NavBarGroupActionList : DesignerActionList {
		NavBarGroupDesigner designer;
		public NavBarGroupActionList(NavBarGroupDesigner designer)
			: base(designer.Component) {
			this.designer = designer;
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			res.Add(new DesignerActionHeaderItem("Appearance", "Appearance"));
			res.Add(new DesignerActionPropertyItem("Caption", "Caption", "Appearance"));
			res.Add(new DesignerActionPropertyItem("SuperTip", "Super tip", "Appearance"));
			res.Add(new DesignerActionPropertyItem("GroupStyle", "Group Style", "Appearance"));
			res.Add(new DesignerActionPropertyItem("Expanded", "Expanded", "Appearance"));
			res.Add(new DesignerActionHeaderItem("Image", "Image"));
			res.Add(new DesignerActionPropertyItem("SmallImage", "Small Image", "Image"));
			res.Add(new DesignerActionPropertyItem("LargeImage", "Large Image", "Image"));
			res.Add(new DesignerActionPropertyItem("SmallImageIndex", "Small Image Index", "Image"));
			res.Add(new DesignerActionPropertyItem("LargeImageIndex", "Large Image Index", "Image"));
			res.Add(new DesignerActionPropertyItem("AllowGlyphSkinning", "Allow Glyph Skinning", "Image"));
			res.Add(new DesignerActionHeaderItem("Actions"));
			if(Group.GroupStyle != NavBarGroupStyle.ControlContainer) {
				res.Add(new DesignerActionMethodItem(this, "AddItem", "Add Item", "Actions"));
			}
			res.Add(new DesignerActionMethodItem(this, "AddGroup", "Add Group", "Actions"));
			return res;
		}
		protected void SetPropertyValue(string property, object value) {
			EditorContextHelper.SetPropertyValue(Designer, Component, property, value);
		}
		public NavBarGroupDesigner Designer { get { return designer; } }
		public NavBarGroup Group { get { return (NavBarGroup)Component; } }
		[DefaultValue("")]
		public string Caption {
			get { return Group.Caption; }
			set { SetPropertyValue("Caption", value); }
		}
		public bool Expanded {
			get { return Group.Expanded; }
			set { SetPropertyValue("Expanded", value); }
		}
		[RefreshProperties(RefreshProperties.All)]
		public NavBarGroupStyle GroupStyle {
			get { return Group.GroupStyle; }
			set {
				SetPropertyValue("GroupStyle", value);
				EditorContextHelperEx.RefreshSmartPanel(Component);
			}
		}
		public void AddItem() {
			NavBarItemLink link = Group.AddItem();
			if(!Group.Expanded) {
				EditorContextHelper.SetPropertyValue(Component.Site, Group, "Expanded", true);
			}
			if(link != null && Group.NavBar != null) {
				NavBarViewInfo vi = Group.NavBar.GetViewInfo();
				vi.MakeLinkVisible(link);
			}
		}
		public void AddGroup() {
			NavBarControl navbar = Group.NavBar;
			if(navbar == null)
				return;
			IDesignerHost host = GetService(typeof(IDesignerHost)) as IDesignerHost;
			if(host == null)
				return;
			ISmartTagCommandsImp commandImp = host.GetDesigner(Group.NavBar) as ISmartTagCommandsImp;
			if(commandImp == null)
				return;
			commandImp.AddGroup();
		}
		[Editor("DevExpress.XtraEditors.Design.ToolTipContainerUITypeEditor, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(UITypeEditor))]
		public SuperToolTip SuperTip {
			get { return Group.SuperTip; }
			set { SetPropertyValue("SuperTip", value); }
		}
		[Editor(typeof(DXImageEditor), typeof(UITypeEditor))]
		public Image SmallImage {
			get { return Group.SmallImage; }
			set { SetPropertyValue("SmallImage", value); }
		}
		[Editor(typeof(DXImageEditor), typeof(UITypeEditor))]
		public Image LargeImage {
			get { return Group.LargeImage; }
			set { SetPropertyValue("LargeImage", value); }
		}
		public int SmallImageIndex {
			get { return Group.SmallImageIndex; }
			set { SetPropertyValue("SmallImageIndex", value); }
		}
		public int LargeImageIndex {
			get { return Group.LargeImageIndex; }
			set { SetPropertyValue("LargeImageIndex", value); }
		}
		public DefaultBoolean AllowGlyphSkinning {
			get { return Group.AllowGlyphSkinning; }
			set { SetPropertyValue("AllowGlyphSkinning", value); }
		}
		[RefreshProperties(RefreshProperties.All)]
		public virtual void EditMask() {
		}
	}
	public class NavBarGroupControlContainerDesigner : ParentControlDesigner {
		protected override void OnPaintAdornments(PaintEventArgs e) {
			base.OnPaintAdornments(e);
		}
		public override SelectionRules SelectionRules {
			get {
				if(GroupContainer == null || !GroupContainer.Visible) return SelectionRules.None;
				SelectionRules rules = SelectionRules.Visible;
				NavBarViewInfo vinfo = NavBarControlDesigner.GetViewInfo(NavBar);
				if(vinfo != null && vinfo.IsExplorerBar) rules |= SelectionRules.BottomSizeable;
				return rules;
			}
		}
		public NavBarGroupControlContainer GroupContainer { get { return Control as NavBarGroupControlContainer; } }
		public NavBarControl NavBar {
			get {
				return GroupContainer == null || GroupContainer.OwnerGroup == null ? null : GroupContainer.OwnerGroup.NavBar;
			}
		}
		protected override bool DrawGrid {
			get {
				if(base.DrawGrid) return true; 
				return true;
			}
		}
	}
	public class NavBarControlDesigner : BaseParentControlDesigner, ISmartTagCommandsImp {
		NavBarEditorForm editor;
#if DXWhidbey
		protected override bool AllowHookDebugMode { get { return true; } }
#endif
		public NavBarControlDesigner() {
			editor = null;
		}
		protected override bool AllowInheritanceWrapper { get { return true; } }
		protected override bool UseVerbsAsActionList { get { return true; } }
		protected override bool AllowEditInherited { get { return false; } }
		IDesignerHost host;
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			this.host = GetService(typeof(IDesignerHost)) as IDesignerHost;
			LoaderPatcherService.InstallService(host);
			bool allowBonusSkins = SkinHelper.InitSkins(component.Site);
			if(allowBonusSkins && component is NavBarControl) PopulateDesignTimeViews(component as NavBarControl);
		}
		protected override void Dispose(bool disposing) {
			LoaderPatcherService.UnInstallService(host);
			this.host = null;
			if(disposing) {
				Editor = null;
			}
			base.Dispose(disposing);
		}
		protected override void PostFilterProperties(IDictionary properties) {
			base.PostFilterProperties(properties);
			DXPropertyDescriptor.ConvertDescriptors(properties, null);
		}
		public static void PopulateDesignTimeViews(NavBarControl navBar) {
			RegisterAvailableNavBarViewsCore(navBar);
			PopulateDesignTimeViewsCore(navBar);
		}
		protected static void RegisterAvailableNavBarViewsCore(NavBarControl navBar) {
			MethodInfo mi = typeof(NavBarControl).GetMethod("RegisterAvailableNavBarViews", BindingFlags.NonPublic | BindingFlags.Instance);
			if(mi != null) mi.Invoke(navBar, null);
		}
		protected static void PopulateDesignTimeViewsCore(NavBarControl navBar) {
			MethodInfo mi = typeof(NavBarControl).GetMethod("PopulateDesignTimeViews", BindingFlags.NonPublic | BindingFlags.Instance);
			if(mi != null) mi.Invoke(navBar, null);
		}
		protected override bool DrawGrid { get { return false; } }
		protected override bool EnableDragRect { get { return false; } }
		public override bool CanParent(Control control) {
			return control is NavBarGroupControlContainer;
		}
		protected override void OnDragEnter(DragEventArgs de) {
			de.Effect = DragDropEffects.None;
		}
		protected override void OnDragOver(DragEventArgs de) {
			de.Effect = DragDropEffects.None;
		}
		protected override IComponent[] CreateToolCore(ToolboxItem tool, int x, int y, int width, int height, bool hasLocation, bool hasSize) {
			Type ownerType = GetOwnerType(tool);
			if(ownerType != null && typeof(Component).IsAssignableFrom(ownerType) && !typeof(Control).IsAssignableFrom(ownerType)) {
				return base.CreateToolCore(tool, x, y, width, height, hasLocation, hasSize);
			}
			return null;
		}
		protected Type GetOwnerType(ToolboxItem tool) {
			Type res = null;
			try {
				res = tool.GetType(DesignerHost);
			}
			catch { }
			return res;
		}
		protected virtual bool GetHitTestCore(Point client) {
			ExplorerBarNavBarViewInfo vi = NavBar.GetViewInfo() as ExplorerBarNavBarViewInfo;
			if(vi != null && vi.ScrollBarRectangle.Contains(client)) return true;
			if(!Rectangle.Inflate(NavBar.ClientRectangle, -3, -3).Contains(client)) return false;
			NavBarHitInfo hInfo = NavBar.CalcHitInfo(client);
			if(hInfo.InGroupCaption || hInfo.InLink || hInfo.InGroupButton || hInfo.HitTest == NavBarHitTest.UpButton
				|| hInfo.HitTest == NavBarHitTest.DownButton || hInfo.HitTest == NavBarHitTest.NavigationPaneOverflowPanelButton) return true;
			return false;
		}
		protected override bool GetHitTest(Point point) {
			bool res = base.GetHitTest(point);
			if(!AllowDesigner || DebuggingState) return res;
			if(NavBar == null || res) return res;
			Point client = NavBar.PointToClient(point);
			return GetHitTestCore(client);
		}
		protected NavBarEditorForm Editor {
			get { return editor; }
			set {
				if(Editor == value) return;
				if(Editor != null) Editor.Dispose();
				editor = value;
			}
		}
		protected NavBarControl NavBar { get { return Control as NavBarControl; } }
		IDesignerHost DesignerHost { get { return GetService(typeof(IDesignerHost)) as IDesignerHost; } }
		public static NavBarViewInfo GetViewInfo(NavBarControl navBar) {
			if(navBar == null) return null;
			PropertyInfo pi = navBar.GetType().GetProperty("ViewInfo", BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic);
			if(pi != null) return pi.GetValue(navBar, null) as NavBarViewInfo;
			return null;
		}
		DesignerActionListCollection smartTagCore = null;
		public override DesignerActionListCollection ActionLists {
			get {
				if(smartTagCore == null) {
					smartTagCore = new DesignerActionListCollection();
					smartTagCore.Add(new NavBarDesignerActionList(Component, this));
					DXSmartTagsHelper.CreateDefaultLinks(this, smartTagCore);
				}
				return smartTagCore;
			}
		}
#if DXWhidbey
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			OnInitialize();
		}
#else
		public override void OnSetComponentDefaults() {
			base.OnSetComponentDefaults();
			OnInitialize();
		}
#endif
		protected virtual void OnInitialize() {
			if(NavBar == null) return;
			NavBar.Groups.Add().Expanded = true;
		}
		public override ICollection AssociatedComponents {
			get {
				if(NavBar == null) return base.AssociatedComponents;
				ArrayList controls = new ArrayList();
				controls.AddRange(NavBar.Items);
				controls.AddRange(NavBar.Groups);
				AddBase(controls);
				return controls;
			}
		}
		void AddBase(ArrayList controls) {
			foreach(object obj in base.AssociatedComponents) {
				if(controls.Contains(obj)) continue;
				controls.Add(obj);
			}
		}
		#region ISmartTagCommandsImp
		public void About() {
			NavBarControl.About();
		}
		public void RunDesigner() {
			if(NavBar == null) return;
			Editor = new NavBarEditorForm();
			editor.InitEditingObject(NavBar);
			Editor.ShowDialog();
			Editor = null;
		}
		public void AddGroup() {
			if(NavBar == null || DesignerHost == null) return;
			NavBar.Groups.Add();
		}
		#endregion
		protected override bool CanUseComponentSmartTags {
			get { return true; }
		}
	}
	public interface ISmartTagCommandsImp {
		void About();
		void RunDesigner();
		void AddGroup();
	}
	public class NavBarDesignerActionList : DesignerActionList {
		ISmartTagCommandsImp imp;
		public NavBarDesignerActionList(IComponent component, ISmartTagCommandsImp imp)
			: base(component) {
			this.imp = imp;
		}
		public void About() {
			imp.About();
		}
		public void RunDesigner() {
			imp.RunDesigner();
		}
		public void AddGroup() {
			imp.AddGroup();
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			res.Add(new DesignerActionMethodItem(this, "AddGroup", "Add Group", true));
			res.Add(new DesignerActionPropertyItem("Dock", "Choose Dock Style"));
			res.Add(new DesignerActionPropertyItem("PaintStyleKind", "Paint Style"));
			res.Add(new DesignerActionMethodItem(this, "RunDesigner", "Run Designer", true));
			res.Add(new DesignerActionMethodItem(this, "About", "About", true));
			return res;
		}
		public NavBarViewKind PaintStyleKind {
			get { return Control.PaintStyleKind; }
			set {
				EditorContextHelper.SetPropertyValue(Component.Site, Control, "PaintStyleKind", value);
			}
		}
		public DockStyle Dock {
			get {
				if(Control == null) return DockStyle.None;
				return Control.Dock;
			}
			set {
				EditorContextHelper.SetPropertyValue(Component.Site, Control, "Dock", value);
			}
		}
		NavBarControl Control { get { return Component as NavBarControl; } }
	}
	public class NavBarControlGroupsCollectionEditor : DXCollectionEditorBase {
		public NavBarControlGroupsCollectionEditor(Type type) : base(type) { }
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			groups = new List<NavBarGroup>();
			return base.EditValue(context, provider, value);
		}
		protected override bool AllowLiveUpdates {
			get { return true; }
		}
		protected override bool StandardCollectionEditorRemoveBehavior {
			get { return true; }
		}
		List<NavBarGroup> groups;
		protected override void StandardCollectionEditorLiveUpdateRemoving(DevExpress.Utils.Design.Internal.CollectionChangedEventArgs e) {
			NavBarGroup group = e.Item as NavBarGroup;
			if(group != null) {
				groups.Add(group);
				if(group.ControlContainer != null && group.NavBar != null)
					group.NavBar.Controls.Remove(group.ControlContainer);
			}
			base.StandardCollectionEditorLiveUpdateRemoving(e);
		}
		protected override DXCollectionEditorBase.DXCollectionEditorBaseForm CreateCollectionForm() {
			return new CollectionEditorForm(this);
		}
		void RestoreNavBar() {
			if(groups == null) return;
			foreach(var group in groups) {
				group.NavBar.Groups.Add(group);
				if(group.ControlContainer != null)
					group.NavBar.Controls.Add(group.ControlContainer);
			}
		}
		class CollectionEditorForm : DXCollectionEditorBase.DXCollectionEditorBaseForm {
			NavBarControlGroupsCollectionEditor editor;
			public CollectionEditorForm(NavBarControlGroupsCollectionEditor editor)
				: base(editor) {
				this.editor = editor;
			}
			protected override void OnClosing(CancelEventArgs e) {
				base.OnClosing(e);
				if(this.DialogResult == System.Windows.Forms.DialogResult.Cancel)
					editor.RestoreNavBar();
			}
		}
	}
	public class NavBarControlItemsCollectionEditor : DXCollectionEditorBase {
		public NavBarControlItemsCollectionEditor(Type type) : base(type) { }
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			return base.EditValue(context, provider, value);
		}
		protected override bool AllowLiveUpdates {
			get { return true; }
		}
		protected override bool StandardCollectionEditorRemoveBehavior {
			get { return true; }
		}
	}
}
