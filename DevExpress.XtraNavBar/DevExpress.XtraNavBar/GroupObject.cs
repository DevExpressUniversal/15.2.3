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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.XtraNavBar.ViewInfo;
using DevExpress.XtraNavBar;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.XtraNavBar.Forms;
using System.Drawing.Design;
using DevExpress.Utils.Navigation;
using DevExpress.XtraEditors;
using DevExpress.LookAndFeel;
namespace DevExpress.XtraNavBar {
	public enum NavBarImage { Default, Small, Large };
	public enum NavBarGroupStyle { Default, SmallIconsText, LargeIconsText, SmallIconsList, LargeIconsList, ControlContainer };
	[
	DesignTimeVisible(false), ToolboxItem(false),
	SmartTagSupport(typeof(NavBarGroupDesignTimeBoundsProvider), SmartTagSupportAttribute.SmartTagCreationMode.UseComponentDesigner),
	Designer("DevExpress.XtraNavBar.Design.NavBarGroupDesigner, " + AssemblyInfo.SRAssemblyNavBarDesign, typeof(IDesigner))
	]
	public class NavBarGroup : NavElement, INavigationItem {
		NavBarGroupControlContainer controlContainer;
		Control collapsedNavPaneContentControl;
		Brush textureBackgroundBrush;
		string styleBackgroundName;
		NavBarImage groupCaptionUseImage;
		bool expanded, useSmallImage;
		int topLinkIndex;
		internal int selectedLinkIndex;
		int groupClientHeight;
		NavBarDragDrop dragDropFlags;
		Image backgroundImage;
		NavLinkCollection itemLinks;
		NavReadOnlyLinkCollection visibleItemLinks;
		NavBarGroupStyle groupStyle;
		bool navigationPaneVisible;
		AppearanceObject appearanceBackground;
		public NavBarGroup(string caption) : this() {
			Caption = caption;
		}
		public NavBarGroup() {
			this.appearanceBackground = CreateAppearance("Background");
			this.navigationPaneVisible = true;
			this.groupCaptionUseImage = NavBarImage.Default;
			this.controlContainer = null;
			this.collapsedNavPaneContentControl = null;
			this.groupStyle = NavBarGroupStyle.Default;
			this.groupClientHeight = -1;
			this.styleBackgroundName = string.Empty;
			this.textureBackgroundBrush = null;
			this.dragDropFlags = NavBarDragDrop.Default;
			this.topLinkIndex = 0;
			this.selectedLinkIndex = -1;
			this.useSmallImage = this.expanded = false;
			this.visibleItemLinks = new NavReadOnlyLinkCollection();
			this.itemLinks = new NavLinkCollection(this);
			this.itemLinks.CollectionChanged += new CollectionChangeEventHandler(OnItemLinksCollectionChanged);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				DestroyContainer();
				if(CollapsedNavPaneContentControl != null && NavBar != null && NavBar.ContainsControl(CollapsedNavPaneContentControl))
					CollapsedNavPaneContentControl.Dispose();
				this.collapsedNavPaneContentControl = null;
				if(this.ItemLinks != null) {
					itemLinks.Clear();
					itemLinks.CollectionChanged -= new CollectionChangeEventHandler(OnItemLinksCollectionChanged);
				}
			}
			base.Dispose(disposing);
		}
		protected override void DestroyAppearances() {
			DestroyAppearance(this.appearanceBackground);
			base.DestroyAppearances();
		}
		NavBarItemLink newSelectedLink;
		bool setSelectedLink;
		protected internal NavBarItemLink NewSelectedLink { get { return newSelectedLink; } set { newSelectedLink = value; } }
		protected internal bool DelayedSetSelectedLink { get { return setSelectedLink; } set { setSelectedLink = value; } }
		protected virtual void OnItemLinksCollectionChanged(object sender, CollectionChangeEventArgs e) {
			NavBarItemLink link = e.Element as NavBarItemLink;
			NewSelectedLink = null;
			DelayedSetSelectedLink = false;
			switch(e.Action) {
				case CollectionChangeAction.Add : 
					link.Item.Links.AddLink(link);
					link.SetNavGroupCore(this);
					if(link.NavBar != null && link.NavBar.Items.IndexOf(link.Item) < 0)
						link.NavBar.Items.Add(link.Item);
					link.ItemChanged += new EventHandler(OnLinkChanged);
					link.VisibleChanged += new EventHandler(OnLinkVisibleChanged);
					NewSelectedLink = link;
					DelayedSetSelectedLink = true;
					break;
				case CollectionChangeAction.Remove:
					link.Item.Links.RemoveLink(link);
					link.SetNavGroupCore(null);
					link.ItemChanged -= new EventHandler(OnLinkChanged);
					link.VisibleChanged -= new EventHandler(OnLinkVisibleChanged);
					NewSelectedLink  = null;
					DelayedSetSelectedLink = true;
					break;
			}
			RebuildVisibleLinks();
			if(DelayedSetSelectedLink && NavBar != null && !NavBar.IsLoading && !NavBar.IsLockLayout) {
				if(NewSelectedLink == null) 
					SelectedLinkIndex = -1;
				else
					SelectedLink = NewSelectedLink;
				DelayedSetSelectedLink = false;
			}
			RaiseItemChanged();
			if(NavBar != null)
				NavBar.UpdateNavBarOnLinksChanged();
		}
		DefaultBoolean showIcons = DefaultBoolean.Default;
		[DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean ShowIcons {
			get { return showIcons; }
			set {
				if(ShowIcons == value)
					return;
				showIcons = value;
				RaiseItemChanged();
			}
		}
		protected internal bool GetShowIcons() {
			if(ShowIcons == DefaultBoolean.Default)
				return NavBar == null ? true : NavBar.ShowIcons != DefaultBoolean.False;
			return ShowIcons == DefaultBoolean.True;
		}
		protected virtual void OnLinkChanged(object sender, EventArgs e) {
			if(Expanded)
				RaiseItemChanged();
		}
		protected virtual void OnLinkVisibleChanged(object sender, EventArgs e) {
			RebuildVisibleLinks();
			RaiseItemChanged();
		}
		internal void RebuildVisibleLinksCore() {
			RebuildVisibleLinks();			
		}
		internal void RebuildVisibleLinksCore(DevExpress.Data.Filtering.CriteriaOperator filterCriteria) {
			RebuildVisibleLinks();
			((ISupportSearchDataAdapter)VisibleItemLinks).FilterCriteria = filterCriteria;
			Visible = VisibleItemLinks.Count != 0 || Object.Equals(filterCriteria, null);
		}
		protected internal virtual void OnLoaded() {
		}
		protected virtual void RebuildVisibleLinks() {
			VisibleItemLinks.ClearLinks();
			foreach(NavBarItemLink link in ItemLinks) {
				if(!link.Visible) continue;
				VisibleItemLinks.AddLink(link);
			}
		}
		bool ShouldSerializeAppearanceBackground() { return AppearanceBackground.ShouldSerialize(); }
		void ResetAppearanceBackground() { AppearanceBackground.Reset(); }
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarGroupAppearanceBackground"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject AppearanceBackground { get { return appearanceBackground; } }
		[XtraSerializableProperty(), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual string ControlContainerName {
			get { return ControlContainer == null ? string.Empty : ControlContainer.Name;	}
			set {
				if(value == null) value = string.Empty;
				if(ControlContainerName == value) return;
				if(NavBar == null) return;
				ControlContainer = FindContainer(value);
			}
		}
		protected override void OnVisibleChanged() {
			if(NavBar != null && NavBar.ActiveGroup != null && !NavBar.ActiveGroup.IsVisible)
				NavBar.ActiveGroup = NavBar.FindVisibleGroup();
			UpdateNavigationClientItems();
		}
		void UpdateNavigationClientItems() {
			if(NavBar != null)
				NavBar.RaiseNavigationBarClientItemsSourceChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
		}
		[DefaultValue(null), Browsable(false)]
		public virtual NavBarGroupControlContainer ControlContainer {
			get { return controlContainer; }
			set {
				if(ControlContainer == value) return;
				DestroyContainer();
				controlContainer = value;
				if(ControlContainer != null) {
					InitContainer(ControlContainer);
					this.groupStyle = NavBarGroupStyle.ControlContainer;
				} else {
					if(GroupStyle == NavBarGroupStyle.ControlContainer) this.groupStyle = NavBarGroupStyle.Default;
				}
				RaiseItemChanged();
			}
		}
		[Category("Appearance"), 
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarGroupCollapsedNavPaneContentControl"),
#endif
 DefaultValue(null), TypeConverter("DevExpress.XtraNavBar.Design.CollapsedNavPaneContentControlTypeConverter, " + AssemblyInfo.SRAssemblyNavBarDesign)]
		public virtual Control CollapsedNavPaneContentControl {
			get { return collapsedNavPaneContentControl; }
			set {
				if(CollapsedNavPaneContentControl == value) return;
				if(CollapsedNavPaneContentControl != null) NavBar.OnDestroyContentControl(CollapsedNavPaneContentControl);
				collapsedNavPaneContentControl = value;
				if(value != null && NavBar != null) NavBar.OnInitContentControl(value);
				RaiseItemChanged();
			}
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarGroupNavigationPaneVisible"),
#endif
 Category("Appearance"), DefaultValue(true), XtraSerializableProperty()]
		public bool NavigationPaneVisible {
			get { return navigationPaneVisible; }
			set {
				if(NavigationPaneVisible == value) return;
				navigationPaneVisible = value;
				RaiseItemChanged();
			}
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarGroupGroupStyle"),
#endif
 Category("Appearance"), DefaultValue(NavBarGroupStyle.Default), XtraSerializableProperty()]
		public virtual NavBarGroupStyle GroupStyle {
			get { return groupStyle; }
			set {
				if(GroupStyle == value) return;
				OnGroupStyleChanging(GroupStyle, value);
			}
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarGroupDragDropFlags"),
#endif
 Category("Behavior"), DefaultValue(NavBarDragDrop.Default),
		Editor(typeof(DevExpress.Utils.Editors.AttributesEditor), typeof(System.Drawing.Design.UITypeEditor)), XtraSerializableProperty()]
		public virtual NavBarDragDrop DragDropFlags {
			get { return dragDropFlags; }
			set {
				dragDropFlags = value;
			}
		}
		public virtual NavBarDragDrop GetDragDropFlags() {
			if((DragDropFlags & NavBarDragDrop.Default) != 0) return NavBar.GetDragDropFlags();
			return DragDropFlags;
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarGroupGroupClientHeight"),
#endif
 Category("Appearance"), DefaultValue(-1), XtraSerializableProperty()]
		public virtual int GroupClientHeight { 
			get { return groupClientHeight; }
			set {
				if(value < 0) value = -1;
				if(GroupClientHeight == value) return;
				groupClientHeight = value;
				RaiseItemChanged();
			}
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarGroupSelectedLinkIndex"),
#endif
 Category("Appearance"), DefaultValue(-1), XtraSerializableProperty()]
		public virtual int SelectedLinkIndex {
			get { 
				if(NavBar == null) return selectedLinkIndex;
				if(!NavBar.GetAllowSelectedLink()) return -1;
				return selectedLinkIndex; }
			set {
				if(value < 0) value = -1;
				int prevSelected = SelectedLinkIndex;
				if(prevSelected == value) return;
				if(NavBar == null || NavBar.IsLoading) selectedLinkIndex = value;
				else {
					selectedLinkIndex = NavBar.SelectItemLink(this, value);
					if(prevSelected != selectedLinkIndex) {
						NavBar.RaiseSelectedLinkChanged(new NavBarSelectedLinkChangedEventArgs(this, SelectedLink));
					}
					if(SelectedLink != null)
						NavBar.AddToGroupSelectedItems(this, SelectedLink);
					NavBar.ViewInfo.OnSelectedLinkChanged(this);
				}
				RaiseItemChanged();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual NavBarItemLink SelectedLink {
			get {
				if(SelectedLinkIndex == -1 || SelectedLinkIndex >= VisibleItemLinks.Count) return null;
				return VisibleItemLinks[SelectedLinkIndex];
			}
			set {
				NavBarItemLink prev = SelectedLink;
				if(prev == value) return;
				if(value == null) SelectedLinkIndex = -1;
				else {
					if(value.Group != this) return;
					SelectedLinkIndex = VisibleItemLinks.IndexOf(value);
				}
			}
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarGroupBackgroundImage"),
#endif
 Category("Appearance"), DefaultValue(null), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public virtual Image BackgroundImage {
			get { return backgroundImage; }
			set {
				if(BackgroundImage == value) return;
				backgroundImage = value;
				UpdateBrush();
				RaiseItemChanged();
			}
		}
		[Browsable(false)]
		public virtual Brush TextureBackgroundBrush {
			get { 
				if(textureBackgroundBrush == null) return NavBar.GroupTextureBackgroundBrush;
				return textureBackgroundBrush; 
			}
		}
		protected virtual void UpdateBrush() {
			if(BackgroundImage == null) textureBackgroundBrush = null;
			else textureBackgroundBrush = new TextureBrush(BackgroundImage);
		}
		[Obsolete("You should use GroupStyle property"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool LinksUseSmallImage {
			get { return GroupStyle == NavBarGroupStyle.SmallIconsList || GroupStyle == NavBarGroupStyle.SmallIconsText; } 
			set {
				if(LinksUseSmallImage == value) return;
				if(!value) {
					GroupStyle = NavBarGroupStyle.Default;
				} else {
					GroupStyle = NavBarGroupStyle.SmallIconsText;
				}
			}
		}
		[Obsolete("You should use GroupStyle property"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool ShowAsIconsView {
			get { return GroupStyle == NavBarGroupStyle.LargeIconsList || GroupStyle == NavBarGroupStyle.SmallIconsList; } 
			set {
				if(ShowAsIconsView == value) return;
				if(!value) {
					GroupStyle = NavBarGroupStyle.Default;
				} else {
					GroupStyle = NavBarGroupStyle.SmallIconsList;
				}
			}
		}
		public NavBarItemLink AddItem() { return InsertItem(ItemLinks.Count); }
		public virtual NavBarItemLink InsertItem(int position) {
			if(NavBar == null) return null;
			NavBarItem item = NavBar.Items.Add();
			return ItemLinks.Insert(position, item) as NavBarItemLink;
		}
		public virtual bool GetShowAsIconsView() {
			if(!NavBar.AllowShowAsIconsView) return false;
			return GetGroupStyle() == NavBarGroupStyle.LargeIconsList || GetGroupStyle() == NavBarGroupStyle.SmallIconsList;
		}
		public virtual bool GetLinksUseSmallImage() {
			if(NavBar.AllowOnlySmallImages) return true;
			return GetGroupStyle() == NavBarGroupStyle.SmallIconsList || GetGroupStyle() == NavBarGroupStyle.SmallIconsText;
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarGroupGroupCaptionUseImage"),
#endif
 Category("Appearance"), DefaultValue(NavBarImage.Default), XtraSerializableProperty()]
		public virtual NavBarImage GroupCaptionUseImage {
			get { return groupCaptionUseImage; }
			set {
				if(GroupCaptionUseImage == value) return;
				groupCaptionUseImage = value;
				RaiseItemChanged();
			}
		}
		[Obsolete("You should use GroupCaptionUseImage property"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool UseSmallImage {
			get { return GroupCaptionUseImage == NavBarImage.Small; } 
			set {
				if(UseSmallImage == value) return;
				GroupCaptionUseImage = value ? NavBarImage.Small : NavBarImage.Large;
			}
		}
		protected internal NavBarGroupStyle GetGroupStyle() {
			NavBarGroupStyle groupStyle = GroupStyle;
			if(groupStyle == NavBarGroupStyle.Default) groupStyle = GetDefaultGroupStyle();
			return groupStyle;
		}
		protected virtual NavBarGroupStyle GetDefaultGroupStyle() {
			if(NavBar != null) return NavBar.ViewInfo.GetDefaultGroupStyle();
			return NavBarGroupStyle.LargeIconsText;
		}
		public virtual Image GetImage() {
			return GetGroupCaptionUseImage() == NavBarImage.Small ? GetSmallImage() : GetLargeImage();
		}
		protected internal virtual Image GetSmallImage() {
			if(GetActualSmallImage() != null) return GetActualSmallImage();
			return GetImageCore(SmallImages, SmallImageIndex);
		}
		protected internal virtual Image GetLargeImage() {
			if(GetActualLargeImage() != null) return GetActualLargeImage();
			return GetImageCore(LargeImages, LargeImageIndex);
		}
		protected internal virtual Size GetSmallImageSize() {
			if(GetActualSmallImage() != null) return GetActualSmallImage().Size;
			if(NavBar.SharedImageCollectionImageSizeMode == SharedImageCollectionImageSizeMode.UseImageSize) {
				Image image = GetSmallImage();
				if(image != null) return image.Size;
			}
			return ImageCollection.GetImageListSize(SmallImages);
		}
		protected internal virtual Size GetLargeImageSize() {
			if(GetActualLargeImage() != null) return GetActualLargeImage().Size;
			if(NavBar.SharedImageCollectionImageSizeMode == SharedImageCollectionImageSizeMode.UseImageSize) {
				Image image = GetLargeImage();
				if(image != null) return image.Size;
			}
			return ImageCollection.GetImageListSize(LargeImages);
		}
		NavBarImage GetGroupCaptionUseImage() {
			NavBarImage useImage = GroupCaptionUseImage;
			if(useImage == NavBarImage.Default) {
				if(GetActualSmallImage() != null || IsImageExists(SmallImages, SmallImageIndex)) 
					useImage = NavBarImage.Small;
				else
					useImage = NavBarImage.Large;
			}
			return useImage;
		}
		public virtual Size GetImageSize() {
			return GetGroupCaptionUseImage() == NavBarImage.Small ? GetSmallImageSize() : GetLargeImageSize();
		}
		public virtual Size GetPrefferedImageSize() {
			if(ShouldUseSmallImageSize) return SmallImageSize;
			if(ShouldUseLargeImageSize) return LargeImageSize;
			return GetImageSize();
		}				  
		[Browsable(false)]
		public NavGroupCollection Collection { get { return ((ICollectionItem)this).Collection as NavGroupCollection; } }
		internal object XtraCreateItemLinksItem(XtraItemEventArgs e) {
			DevExpress.Utils.Serializing.Helpers.XtraPropertyInfo pi = e.Item.ChildProperties["ItemName"];
			if(pi == null) return null;
			NavBarItem item = NavBar.Items[pi.Value.ToString()];
			if(item == null) return null;
			NavBarItemLink link = ItemLinks.Add(item);
			link.VisibleCore = bool.Parse((string)e.Item.ChildProperties["VisibleCore"].Value);
			return null;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), 
		XtraSerializableProperty(true, false, true)
#if DXWhidbey
		,InheritableCollection
#endif
		]
		public NavLinkCollection ItemLinks {
			get { return itemLinks; }
		}
		[Browsable(false)]
		public NavReadOnlyLinkCollection VisibleItemLinks {
			get { return visibleItemLinks; }
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarGroupTopVisibleLinkIndex"),
#endif
 Category("Appearance"), DefaultValue(0), XtraSerializableProperty()]
		public virtual int TopVisibleLinkIndex {
			get { return topLinkIndex; }
			set {
				if(value < 0) value = 0;
				if(TopVisibleLinkIndex == value) return;
				topLinkIndex = value;
				RaiseItemChanged();
			}
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarGroupExpanded"),
#endif
 Category("Appearance"), DefaultValue(false), XtraSerializableProperty()]
		public virtual bool Expanded {
			get { 
				if(NavBar == null) return expanded;
				return NavBar.IsGroupExpanded(this);
			}
			set {
				if(NavBar == null || NavBar.IsLoading) {
					expanded = value;
					return;
				}
				if(Expanded == value) return;
				if(NavBar.ExpandGroupCore(this, value) == Expanded) return;
				expanded = value;
				RaiseGroupExpandedChanged();
				RaiseItemChanged();
			}
		}
		internal bool ExpandedCore { get { return expanded; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual ObjectState State { get { return StateCore & (~ObjectState.Selected); } }
		protected internal virtual ObjectState StateCore {
			get {
				if(NavBar == null) return ObjectState.Normal;
				ObjectState res = ObjectState.Normal;
				if(this == NavBar.PressedGroup) res = ObjectState.Pressed;
				else
					if(this == NavBar.HotTrackedGroup) res = ObjectState.Hot;
				if(this == NavBar.ActiveGroup && !NavBar.ViewInfo.IsExplorerBar ) res |= ObjectState.Selected;
				return res;
			}
		}
		protected internal virtual void RaiseGroupExpanding(NavBarGroupCancelEventArgs e) {
			if(NavBar == null) return;
			if(Expanded)
				NavBar.RaiseGroupCollapsing(e);
			else
				NavBar.RaiseGroupExpanding(e);
		}
		protected internal virtual void RaiseGroupExpandedChanged() {
			if(NavBar == null)
				return;
			if(Expanded) 
				NavBar.RaiseGroupExpanded(new NavBarGroupEventArgs(this));
			else 
				NavBar.RaiseGroupCollapsed(new NavBarGroupEventArgs(this));
		}
		protected internal NavBarItemLink GetItemLinkByCaption(string str){
			if(VisibleItemLinks.Count == 0) return null;
			foreach (NavBarItemLink link in VisibleItemLinks){
				if(link.Caption == str)
					return link;
			}
			return VisibleItemLinks[0];
		}
		protected internal NavBarItemLink GetFirstItemLink() {
			if(VisibleItemLinks.Count == 0) return null;
			return VisibleItemLinks[0];
		}
		protected internal AppearanceObject GetHeaderAppearance() {
			if(NavBar == null) return Appearance;
			ObjectState state = StateCore;
			AppearanceObject res = GetHeaderAppearanceByState(state);
			return res;
		}
		protected internal AppearanceObject GetHeaderAppearanceByState(ObjectState state) {
			AppearanceObject res = new AppearanceObject();
			AppearanceObject[] combine;
			AppearanceObject selected = (state & ObjectState.Selected) != 0 ? NavBar.PaintAppearance.GroupHeaderActive : null;
			switch(state & (~ObjectState.Selected)) {
				case ObjectState.Hot: combine = new AppearanceObject[] { AppearanceHotTracked, selected, NavBar.PaintAppearance.GroupHeaderHotTracked, Appearance, NavBar.PaintAppearance.GroupHeader }; break;
				case ObjectState.Pressed: combine = new AppearanceObject[] { AppearancePressed, selected, NavBar.PaintAppearance.GroupHeaderPressed, Appearance, NavBar.PaintAppearance.GroupHeader }; break;
				default:
					combine = new AppearanceObject[] { selected, Appearance, NavBar.PaintAppearance.GroupHeader }; break;
			}
			AppearanceHelper.Combine(res, combine);
			return res;
		}
		protected virtual void OnGroupStyleChanging(NavBarGroupStyle oldStyle, NavBarGroupStyle newStyle) {
			this.groupStyle = newStyle;
			if(IsLoading) return;
			if(oldStyle == NavBarGroupStyle.ControlContainer) {
				DestroyContainer();
				if(NavBar.IsDesignMode) this.groupClientHeight = -1;
			}
			if(newStyle == NavBarGroupStyle.ControlContainer) {
				if(NavBar.IsDesignMode) {
					this.groupClientHeight = 80;
					Expanded = true;
				}
				InitContainer(null);
			}
			RaiseItemChanged();
		}
		protected virtual void DestroyContainer() {
			if(ControlContainer == null) return;
			ControlContainer.SetOwnerGroup(null);
			ControlContainer.Dispose();
			this.controlContainer = null;
		}
		protected virtual void InitContainer(NavBarGroupControlContainer container) {
			if(container == null) container = CreateControlContainer();
			this.controlContainer = container;
			ControlContainer.SetOwnerGroup(this);
			ControlContainer.Visible = false;
			if(NavBar == null || IsLoading) return;
			NavBar.Controls.Add(ControlContainer);
		}
		protected virtual NavBarGroupControlContainer CreateControlContainer() {
			IDesignerHost host = NavBar.InternalGetService(typeof(IDesignerHost)) as IDesignerHost;
			if(host == null) return new NavBarGroupControlContainer();
			return host.CreateComponent(typeof(NavBarGroupControlContainer)) as NavBarGroupControlContainer;
		}
		protected NavBarGroupControlContainer FindContainer(string name) {
			if(name == string.Empty || NavBar == null) return null;
			foreach(Control ctrl in NavBar.Controls) {
				if(ctrl.Name == name) {
					NavBarGroupControlContainerWrapper wrapper = ctrl as NavBarGroupControlContainerWrapper;
					if(wrapper != null) return wrapper.ControlContainer;
					return ctrl as NavBarGroupControlContainer;
				}
			}
			return null;
		}
		protected internal virtual void UpdateControlContainer(Rectangle clientBounds) {
			if(ControlContainer == null) return;
			if(clientBounds.IsEmpty) SetControlContainerVisibility(false);
			else {
				SetControlContainerBounds(clientBounds);
				SetControlContainerVisibility(true);
			}
		}
		protected virtual bool DesignModeCore { get { return DesignMode; } } 
		protected virtual void SetControlContainerBounds(Rectangle bounds) {
			if(ControlContainer == null) return;
			if(DesignModeCore) {
				ControlContainer.SetBounds(bounds);
				return;
			}
			Point location = Point.Empty;
			Size containerSize = bounds.Size;
			NavBarGroupControlContainerWrapper wrapper = ControlContainer.Parent as NavBarGroupControlContainerWrapper;
			if(wrapper == null) {
				wrapper = new NavBarGroupControlContainerWrapper(controlContainer);
			}
			if(bounds.Bottom > NavBar.ViewInfo.Client.Bottom) {
				bounds.Height = NavBar.ViewInfo.Client.Bottom - bounds.Top;
			}
			if(bounds.Top < NavBar.ViewInfo.Client.Top) {
				int delta = NavBar.ViewInfo.Client.Top - bounds.Top;
				bounds.Y = NavBar.ViewInfo.Client.Top;
				bounds.Height -= delta;
				location.Y -= delta;
			}
			controlContainer.SetBounds(new Rectangle(location, containerSize));
			wrapper.Bounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, Math.Max(0, bounds.Height));
		}
		protected virtual void SetControlContainerVisibility(bool visible) {
			if(!CanUpdateControlContainerVisibility(visible)) return;
			ControlContainer.Visible = visible;
			if(ControlContainer.Parent is NavBarGroupControlContainerWrapper)
				ControlContainer.Parent.Visible = visible;
		}
		protected virtual bool CanUpdateControlContainerVisibility(bool visible) {
			if(ControlContainer == null) return false;
			if(visible || NavBar == null || NavBar.NavPaneForm == null) return true;
			return !NavBar.NavPaneForm.Visible;
		}
		private static readonly object calcGroupClientHeight = new object();
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarGroupCalcGroupClientHeight"),
#endif
 Category("NavBar")]
		public event NavBarCalcGroupClientHeightEventHandler CalcGroupClientHeight {
			add { Events.AddHandler(calcGroupClientHeight, value); }
			remove { Events.RemoveHandler(calcGroupClientHeight, value); }
		}
		protected internal virtual void RaiseCalcGroupClientHeight(NavBarCalcGroupClientHeightEventArgs e) {
			NavBarCalcGroupClientHeightEventHandler handler = (NavBarCalcGroupClientHeightEventHandler)this.Events[calcGroupClientHeight];
			if(handler != null) handler(this, e);
		}
		#region INavigationItem Members
		Image INavigationItem.Image {
			get { return GetImage(); }
		}
		string INavigationItem.Name {
			get { return Name; }
		}
		string INavigationItem.Text {
			get { return Caption; }
		}
		#endregion
	}
	[ToolboxItem(false)]
	public class NavBarGroupControlContainerWrapper : Control {
		NavBarGroupControlContainer controlContainer;
		public NavBarGroupControlContainerWrapper(NavBarGroupControlContainer controlContainer) {
			this.TabStop = false;
			this.Visible = false;
			Control oldParent = controlContainer.Parent;
			this.controlContainer = controlContainer;
			Controls.Add(ControlContainer);
			ControlContainer.Disposed += new EventHandler(OnControlContainer_Disposed);
			SetStyle(ControlStyles.UserMouse | ControlConstants.DoubleBuffer | ControlStyles.SupportsTransparentBackColor | ControlStyles.ResizeRedraw, true);
			BackColor = Color.Transparent;
			if(oldParent != null) {
				oldParent.Controls.Add(this);
				this.UseWaitCursor = oldParent.UseWaitCursor;
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				NavBarGroupControlContainer oldContainer = ControlContainer;
				this.controlContainer = null;
				if(oldContainer != null) oldContainer.Dispose();
			}
			base.Dispose(disposing);
		}
		protected override void OnMove(EventArgs e) {
			base.OnMove(e);
			if(!ControlContainer.OwnerGroup.NavBar.ViewInfo.inUpdateGroupBounds)
				Refresh();
		}
		protected void OnControlContainer_Disposed(object sender, EventArgs e) {
			Dispose();
		}
		public NavBarGroupControlContainer ControlContainer { 
			get { return controlContainer; }
		}
	}
	[ToolboxItem(false), Designer("DevExpress.XtraNavBar.Design.NavBarGroupControlContainerDesigner, " + AssemblyInfo.SRAssemblyNavBarDesign, typeof(IDesigner))]
	public class NavBarGroupControlContainer : XtraScrollableControl, ITransparentBackgroundManager, ITransparentBackgroundManagerEx {
		NavBarGroup ownerGroup;
		internal bool useTransparentColor = false;
		public NavBarGroupControlContainer() {
			this.TabStop = false;
			SetStyle(ControlStyles.Opaque | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserMouse | ControlConstants.DoubleBuffer | ControlStyles.ResizeRedraw, true);
			base.BackColor = SystemColors.Control; 
			this.ownerGroup = null;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(OwnerGroup != null) {
					NavBarGroup group = OwnerGroup;
					this.ownerGroup = null;
					group.GroupStyle = NavBarGroupStyle.Default;
				}
			}
			base.Dispose(disposing);
		}
		DevExpress.Accessibility.BaseAccessible dxAccessible;
		protected internal virtual DevExpress.Accessibility.BaseAccessible DXAccessible { 
			get {
				if(dxAccessible == null) dxAccessible = CreateAccessibleInstance();
				return dxAccessible;
			}
		}
		protected virtual DevExpress.Accessibility.BaseAccessible CreateAccessibleInstance() {
			return new DevExpress.Accessibility.StandardAccessibleEx(this, base.CreateAccessibilityInstance()); 
		}
		protected override AccessibleObject CreateAccessibilityInstance() {
			return DXAccessible.Accessible;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new AnchorStyles Anchor { get { return base.Anchor; } set { base.Anchor = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new Point Location {
			get { return base.Location; }
			set { base.Location = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), EditorBrowsable(EditorBrowsableState.Never)]
		public new Size Size {
			get { return base.Size; }
			set { 
				SuspendLayout(); 
				try {
					SetBounds(new Rectangle(Location, value));				
				} finally {
					ResumeLayout(true);
				}
			}
		}
		[Browsable(false)]
		public override bool AutoScroll {
			get { return base.AutoScroll; }
			set { base.AutoScroll = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool Visible {
			get { return base.Visible; }
			set { base.Visible = value; }
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarGroupControlContainerTabStop"),
#endif
 DefaultValue(false)]
		public new bool TabStop { 
			get { return base.TabStop; } 
			set { base.TabStop = value; }
		}
#if !SL
	[DevExpressXtraNavBarLocalizedDescription("NavBarGroupControlContainerOwnerGroup")]
#endif
		public NavBarGroup OwnerGroup { get { return ownerGroup; } }
		protected internal void SetOwnerGroup(NavBarGroup ownerGroup) {
			this.ownerGroup = ownerGroup;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override DockStyle Dock { 
			get { return base.Dock; }
			set { base.Dock = DockStyle.None; }
		}
		protected override void OnMove(EventArgs e) {
			base.OnMove(e);
			if(!NavBar.ViewInfo.inUpdateGroupBounds)
				Refresh();
		}
		protected NavBarViewInfo ViewInfo {
			get { return NavBar == null ? null : NavBar.ViewInfo; }
		}
		protected NavBarControl NavBar { get { return OwnerGroup == null ? null : OwnerGroup.NavBar; } }
		protected override void OnPaint(PaintEventArgs e) {
			bool inForm = Parent != null && Parent is NavPaneForm;
			NavGroupInfoArgs groupInfo;
			if(NavBar != null) NavBar.CheckViewInfo();
			if( !inForm ) groupInfo = ViewInfo == null ? null : ViewInfo.GetGroupInfo(OwnerGroup);
			else groupInfo = NavBar.NavPaneForm.ExpandedGroupInfo;
			if(groupInfo == null) {
				base.OnPaint(e);
				return;
			}
			BaseNavGroupPainter groupPainter = NavBar.GroupPainter;
			GraphicsCache cache = new GraphicsCache(e);
			groupInfo.Cache = cache;
			if(!inForm) {
				bool oldFlag = cache.ClipInfo.RequireAPIClipping;
				cache.ClipInfo.RequireAPIClipping = false;
				cache.ClipInfo.MaximumBounds = new Rectangle(cache.ClipInfo.MaximumBounds.X, 0, cache.ClipInfo.MaximumBounds.Width, groupInfo.ClientInfo.ClientBounds.Height);
				cache.ClipInfo.SetClip(cache.ClipInfo.MaximumBounds);
				e.Graphics.TranslateTransform(-groupInfo.ClientInfo.ClientInnerBounds.X, -groupInfo.ClientInfo.ClientInnerBounds.Y);
				groupPainter.ClientPainter.DrawObject(groupInfo.ClientInfo);
				cache.ClipInfo.RequireAPIClipping = oldFlag;
			}
			else { 
				e.Graphics.TranslateTransform(-NavBar.NavPaneForm.ViewInfo.ContentBounds.X, -NavBar.NavPaneForm.ViewInfo.ContentBounds.Y);
				NavBar.NavPaneForm.Painter.DrawBackground(NavBar.NavPaneForm.ViewInfo.GetInfoArgs(cache));
			}
			e.Graphics.ResetTransform();
			groupInfo.Cache = null;
			cache.Dispose();
		}
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
			height = Math.Max(20, height);
			base.SetBoundsCore(x, y, width, height, specified);
		}
		protected override void OnPaintBackground(PaintEventArgs e) {
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override Color BackColor {
			get { 
				if(useTransparentColor) return Color.Transparent;
				if(ViewInfo != null )
					return ViewInfo.PaintAppearance.GroupBackground.BackColor;
				return base.BackColor;
			}
			set {
			}
		}
		bool internalSizing = false;
		protected internal void SetBounds(Rectangle bounds) {
			if(this.internalSizing)
				return;
			this.internalSizing = true;
			try {
				Bounds = bounds;
			}
			finally {
				this.internalSizing = false;
			}
		}
		protected override void OnResize(EventArgs e) {
			bool res = this.internalSizing;
			base.OnResize(e);
			if(res) {
				this.internalSizing = res;
				return;
			}
			if(ShouldAdjustGroupHeight) {
				OwnerGroup.GroupClientHeight = NavBar.GroupPainter.ClientPainter.CalcGroupClientHeightByClientSize(Size);
			}
		}
		protected virtual bool ShouldAdjustGroupHeight {
			get {
				if(DesignMode) return true;
				if(InsideDocking()) return false;
				return OwnerGroup != null && IsHandleCreated;
			}
		}
		protected bool InsideDocking() { 
			if(NavBar == null) return false;
			Control parent = NavBar.Parent;
			while(parent != null) {
				if(string.Equals(parent.GetType().Name, "DockPanel", StringComparison.Ordinal))
					return true;
				parent = parent.Parent;
			}
			return false;
		}
		Color ITransparentBackgroundManager.GetForeColor(object childObject) {
			return Color.Empty;
		}
		Color ITransparentBackgroundManager.GetForeColor(Control childControl) {
			if(NavBar != null) return NavBar.PaintAppearance.Item.ForeColor;
			return Color.Transparent;
		}
		Color ITransparentBackgroundManagerEx.GetEmptyBackColor(Control childControl) {
			return Color.Transparent;
		}
	}
}
