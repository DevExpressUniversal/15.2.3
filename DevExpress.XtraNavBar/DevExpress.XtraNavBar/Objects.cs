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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Serializing;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.XtraNavBar.ViewInfo;
using DevExpress.Utils.Drawing;
using DevExpress.XtraNavBar.Forms;
using System.Drawing.Design;
using DevExpress.Utils.Text;
namespace DevExpress.XtraNavBar {
	public interface ICollectionItem {
		Collection Collection { get ; }
		string ItemName { get; }
		void SetCollection(Collection newCollection);
		event EventHandler ItemChanged;
	}
	[DesignTimeVisible(false), ToolboxItem(false)]
	public class ComponentCollectionItem : Component, ICollectionItem {
		static readonly object itemChangedEvent = new object();
		string name;
		Collection collection;
		public ComponentCollectionItem() {
			collection = null;
			name = "";
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				Collection oldCollection = collection;
				collection = null;
				if(oldCollection != null) oldCollection.Remove(this);
			}
			base.Dispose(disposing);
		}
		[Browsable(false), XtraSerializableProperty()]
		public string Name {
			get { 
				if(Site != null) return Site.Name;
				return name;
			}
			set {
				if(value == null) return;
				name = value;
			}
		}
		string ICollectionItem.ItemName { get { return Name; } }
		Collection ICollectionItem.Collection { get { return collection; } }
		void ICollectionItem.SetCollection(Collection newCollection) {
			collection = newCollection;
		}
		protected virtual void RaiseItemChanged() {
			EventHandler handler = this.Events[itemChangedEvent] as EventHandler;
			if(handler != null) handler(this, EventArgs.Empty);
		}
#if !SL
	[DevExpressXtraNavBarLocalizedDescription("ComponentCollectionItemItemChanged")]
#endif
		public event EventHandler ItemChanged { 
			add { Events.AddHandler(itemChangedEvent, value); }
			remove { Events.RemoveHandler(itemChangedEvent, value); }
		}
	}
	public class NavElementConverter : ComponentConverter {
		public NavElementConverter(Type type) : base(type) { }
	}
	[DesignTimeVisible(false), ToolboxItem(false), TypeConverter(typeof(NavElementConverter)),
	Designer("DevExpress.XtraNavBar.Design.NavBarComponentDesigner, " + AssemblyInfo.SRAssemblyNavBarDesign)]
	public class NavElement : ComponentCollectionItem, ICaptionSupport, IAppearanceOwner, IXtraSerializableLayoutEx, IDXImageUriClient {
		string caption;
		NavBarControl navBar;
		int smallImageIndex, largeImageIndex;
		Image smallImage, largeImage;
		string hint;
		AppearanceObject appearance, appearancePressed, appearanceHotTracked;
		Size smallImageSize, largeImageSize;
		bool visible;
		object tag;
		DefaultBoolean allowGlyphSkinning;
		DefaultBoolean allowHtmlString;
		DxImageUri imageUri;
		const int AppearanceId = 1;
		const int AppearanceHotTrackedId = 2;
		const int AppearancePressedId = 3;
		public NavElement() {
			tag = null;
			hint = string.Empty;
			visible = true;
			navBar = null;
			caption = DefaultCaption;
			largeImageIndex = smallImageIndex = -1;
			largeImageSize = smallImageSize = DefaultImageSize;
			smallImage = largeImage = null;
			this.appearance = CreateAppearance("Default");
			this.appearancePressed = CreateAppearance("Pressed");
			this.appearanceHotTracked = CreateAppearance("HotTracked");
			this.allowGlyphSkinning = DefaultBoolean.Default;
			this.allowHtmlString = DefaultBoolean.Default;
			this.imageUri = CreateImageUriInstance();
			this.imageUri.Changed += ImageUriChanged;
		}
		protected internal virtual bool ShouldSerializeImageUri() {
			return ImageUri.ShouldSerialize();
		}
		protected internal virtual void ResetImageUri() {
			ImageUri.Reset();
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavElementImageUri"),
#endif
 Category("Appearance"), TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual DxImageUri ImageUri {
			get { return imageUri; }
			set {
				if(value == null || ImageUri.Equals(value)) return;
				DxImageUri prev = ImageUri;
				this.imageUri = value;
				OnImageUriChanged(prev, value);
			}
		}
		private void OnImageUriChanged(DxImageUri prev, DxImageUri next) {
			if(prev != null) {
				prev.Changed -= ImageUriChanged;
			}
			if(next != null) {
				next.Changed += ImageUriChanged;
				next.SetClient(this);
			}
			RaiseItemChanged();
		}
		protected void ImageUriChanged(object sender, EventArgs e) {
			RaiseItemChanged();
		}
		protected virtual DxImageUri CreateImageUriInstance() {
			return new DxImageUri();
		}
		public static readonly Size DefaultImageSize = new Size(0, 0);
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(this.imageUri != null) {
					this.imageUri.Changed -= ImageUriChanged;
					this.imageUri.Dispose();
				}
				this.imageUri = null;
				navBar = null;
				DestroyAppearances();
			}
			base.Dispose(disposing);
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavElementAllowGlyphSkinning"),
#endif
 Category("Appearance"), DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(), SmartTagProperty("Allow Glyph Skinning", "Image", 120)]
		public DefaultBoolean AllowGlyphSkinning {
			get { return allowGlyphSkinning; }
			set {
				if(AllowGlyphSkinning == value)
					return;
				allowGlyphSkinning = value;
				RaiseItemChanged();
			}
		}
		[Category("Behavior"), DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public DefaultBoolean AllowHtmlString {
			get { return allowHtmlString; }
			set {
				if(AllowHtmlString == value)
				return;
				allowHtmlString = value;
				RaiseItemChanged();
			}
		}
		protected internal virtual bool GetAllowGlyphSkinning() {
			if(AllowGlyphSkinning == DefaultBoolean.Default)
				return NavBar == null ? false : NavBar.AllowGlyphSkinning;
			return AllowGlyphSkinning == DefaultBoolean.True;
		}
		protected internal virtual bool GetAllowHtmlString() {
			if (AllowHtmlString == DefaultBoolean.Default)
				return NavBar == null ? false : NavBar.AllowHtmlString;
			return AllowHtmlString == DefaultBoolean.True;
		}
		bool IAppearanceOwner.IsLoading { get { return IsLoading; } }
		protected AppearanceObject CreateAppearance(string name) { return CreateAppearance(name, null); }
		protected AppearanceObject CreateAppearance(string name, AppearanceObject parentAppearance) {
			AppearanceObject res = new AppearanceObject(this, parentAppearance, name);
			res.Changed += new EventHandler(OnAppearanceChanged);
			return res;
		}
		protected void DestroyAppearance(AppearanceObject appearance) {
			if(appearance == null) return;
			appearance.Changed -= new EventHandler(OnAppearanceChanged);
			appearance.Dispose();
		}
		protected virtual void DestroyAppearances() {
			DestroyAppearance(this.appearance);
			DestroyAppearance(this.appearancePressed);
			DestroyAppearance(this.appearanceHotTracked);
		}
		protected void FireChanged() {
			if(!DesignMode || IsLoading) return;
			System.ComponentModel.Design.IComponentChangeService srv = GetService(typeof(System.ComponentModel.Design.IComponentChangeService)) as System.ComponentModel.Design.IComponentChangeService;
			if(srv == null) return;
			srv.OnComponentChanged(this, null, null, null);
		}
		protected override void RaiseItemChanged() {
			FireChanged();
			base.RaiseItemChanged();
		}
		protected internal bool IsLoading { 
			get { return NavBar == null || NavBar.IsLoading; }
		}
		string ICaptionSupport.Caption { get { return Name; } }
		public override string ToString() { return string.Format("{0} {1}", Name,  Caption); }
		bool ShouldSerializeAppearance() { return Appearance.ShouldSerialize(); }
		void ResetAppearance() { Appearance.Reset(); }
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavElementAppearance"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), XtraSerializablePropertyId(AppearanceId)]
		public virtual AppearanceObject Appearance { get { return appearance; } }
		bool ShouldSerializeAppearanceHotTracked() { return AppearanceHotTracked.ShouldSerialize(); }
		void ResetAppearanceHotTracked() { AppearanceHotTracked.Reset(); }
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavElementAppearanceHotTracked"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), XtraSerializablePropertyId(AppearanceHotTrackedId)]
		public virtual AppearanceObject AppearanceHotTracked { get { return appearanceHotTracked; } }
		bool ShouldSerializeAppearancePressed() { return AppearancePressed.ShouldSerialize(); }
		void ResetAppearancePressed() { AppearancePressed.Reset(); }
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavElementAppearancePressed"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), XtraSerializablePropertyId(AppearancePressedId)]
		public virtual AppearanceObject AppearancePressed { get { return appearancePressed; } }
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavElementHint"),
#endif
 DefaultValue(""), Category("Appearance"), Localizable(true)]
		public virtual string Hint {
			get { return hint; }
			set { if(value == null) value = string.Empty;
				hint = value;
			}
		}
		protected virtual void OnVisibleChanged() { }
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavElementVisible"),
#endif
 DefaultValue(true), Category("Appearance"), XtraSerializableProperty()]
		public bool Visible {
			get { return visible; }
			set {
				if(Visible == value) return;
				visible = value;
				RaiseItemChanged();
				OnVisibleChanged();
			}
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavElementTag"),
#endif
 XtraSerializableProperty(), Category("Data"), DefaultValue(null),
	   Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))
		]
		public virtual object Tag {
			get { return tag; }
			set {
				tag = value;
			}
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavElementSmallImageIndex"),
#endif
 DefaultValue(-1), Category("Appearance"), Editor(typeof(ImageIndexesEditor), typeof(UITypeEditor)),
		ImageList("SmallImages"), XtraSerializableProperty, Localizable(true), SmartTagProperty("Small Image Index", "Image", 20, SmartTagActionType.RefreshAfterExecute)
		]
		public virtual int SmallImageIndex { 
			get { return smallImageIndex; }
			set {
				if(SmallImageIndex == value) return;
				smallImageIndex = value;
				RaiseItemChanged();
			}
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavElementLargeImageIndex"),
#endif
 DefaultValue(-1), Category("Appearance"), Editor(typeof(ImageIndexesEditor), typeof(UITypeEditor)),
		ImageList("LargeImages"), XtraSerializableProperty, Localizable(true), SmartTagProperty("Large Image Index", "Image", 30, SmartTagActionType.RefreshAfterExecute)
		]
		public virtual int LargeImageIndex { 
			get { return largeImageIndex; }
			set {
				if(LargeImageIndex == value) return;
				largeImageIndex = value;
				RaiseItemChanged();
			}
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavElementSmallImage"),
#endif
 Category("Appearance"), DefaultValue(null), Localizable(true), SmartTagProperty("Small Image", "Image", 0, SmartTagActionType.RefreshAfterExecute), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public virtual Image SmallImage { 
			get { return smallImage; }
			set {
				if(SmallImage == value) return;
				smallImage = ConvertImage(value);
				RaiseItemChanged();
			}
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavElementLargeImage"),
#endif
 Category("Appearance"), DefaultValue(null), Localizable(true), SmartTagProperty("Large Image", "Image", 10, SmartTagActionType.RefreshAfterExecute), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public virtual Image LargeImage { 
			get { return largeImage; }
			set {
				if(LargeImage == value) return;
				largeImage = ConvertImage(value);
				RaiseItemChanged();
			}
		}
		protected internal Image GetActualSmallImage() {
			if(ImageUri != null && ImageUri.HasImage) return ImageUri.GetImage();
			return SmallImage;
		}
		protected internal Image GetActualLargeImage() {
			if(ImageUri != null && ImageUri.HasLargeImage) return ImageUri.GetLargeImage();
			return LargeImage;
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavElementSmallImageSize"),
#endif
 Category("Appearance")]
		public virtual Size SmallImageSize {
			get { return smallImageSize; }
			set {
				if(smallImageSize == value) return;
				smallImageSize = value;
				RaiseItemChanged();
			}
		}
		bool ShouldSerializeSmallImageSize() {
			return SmallImageSize != DefaultImageSize;
		}
		void ResetSmallImageSize() {
			SmallImageSize = DefaultImageSize;
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavElementLargeImageSize"),
#endif
 Category("Appearance")]
		public virtual Size LargeImageSize {
			get { return largeImageSize; }
			set {
				if(largeImageSize == value) return;
				largeImageSize = value;
				RaiseItemChanged();
			}
		}
		bool ShouldSerializeLargeImageSize() {
			return LargeImageSize != DefaultImageSize;
		}
		void ResetLargeImageSize() {
			LargeImageSize = DefaultImageSize;
		}
		protected internal bool ShouldUseLargeImageSize {
			get {
				if(ShouldUseSmallImageSize) return false;
				return LargeImageSize.Width != 0 && LargeImageSize.Height != 0 && GetActualLargeImage() != null;
			}
		}
		protected internal bool ShouldUseSmallImageSize {
			get {
				return GetActualSmallImage() != null && SmallImageSize.Width != 0 && SmallImageSize.Height != 0;
			}
		}
		public Size GetPrefferedImageSize(Size r) {
			if(ShouldUseSmallImageSize)
				return new Size(Math.Min(SmallImageSize.Width, r.Width), Math.Min(SmallImageSize.Height, r.Height));
			if(ShouldUseLargeImageSize)
				return new Size(Math.Min(LargeImageSize.Width, r.Width), Math.Min(LargeImageSize.Height, r.Height));
			return r;
		}
		protected internal virtual bool IsDesignMode { get { return DesignMode; } }
		protected internal bool IsVisible { get { return Visible || DesignMode; } }
		protected virtual Image ConvertImage(Image value) {
			return value;
		}
		[Browsable(false)]
		public object SmallImages {
			get { 
				if(NavBar == null) return null;
				return NavBar.SmallImages;
			}
		}
		[Browsable(false)]
		public object LargeImages {
			get { 
				if(NavBar == null) return null;
				return NavBar.LargeImages;
			}
		}
		internal Image GetImageCore(object list, int imageIndex) {
			return ImageCollection.GetImageListImage(list, imageIndex);
		}
		internal bool IsImageExists(object list, int imageIndex) {
			return ImageCollection.IsImageListImageExists(list, imageIndex);
		}
		protected internal virtual string DefaultCaption { get { return "Element"; } }
		internal bool ShouldSerializeCaption() { return DefaultCaption != Caption; }
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavElementCaption"),
#endif
 Category("Appearance"), XtraSerializableProperty(), Localizable(true), SmartTagProperty("Caption", "Appearance", 0, SmartTagActionType.RefreshAfterExecute)]
		public virtual string Caption {
			get { return caption; }
			set {
				if(Caption == value) return;
				caption = value;
				RaiseItemChanged();
			}
		}
		[Browsable(false)]
		public NavBarControl NavBar { get { return navBar; } }
		internal void SetNavBarCore(NavBarControl newValue) {
			this.navBar = newValue;
		}
		protected virtual void OnAppearanceChanged(object sender, EventArgs e) {
			RaiseItemChanged();
		}
		SuperToolTip superTip;
		internal bool ShouldSerializeSuperTip() { return SuperTip != null && !SuperTip.IsEmpty; }
		[Localizable(true), Category("Tooltip"), 
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavElementSuperTip"),
#endif
 SmartTagProperty("Super Tip", "Appearance", 30),
		Editor("DevExpress.XtraEditors.Design.ToolTipContainerUITypeEditor, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(UITypeEditor))
		]
		public virtual SuperToolTip SuperTip {
			get { return superTip; }
			set { superTip = value; }
		}
		public void ResetSuperTip() { SuperTip = null; }
		#region IXtraSerializableLayoutEx
		bool IXtraSerializableLayoutEx.AllowProperty(OptionsLayoutBase options, string propertyName, int id) {
			return OnAllowSerializationProperty(options, propertyName, id);
		}
		void IXtraSerializableLayoutEx.ResetProperties(OptionsLayoutBase options) {
			OnResetSerializationProperties(options);
		}
		#endregion
		protected virtual bool OnAllowSerializationProperty(OptionsLayoutBase options, string propertyName, int id) {
			if(id == AppearanceId || id == AppearanceHotTrackedId || id == AppearancePressedId) {
				return NavBar.OptionsLayout.StoreAppearance;
			}
			return true;
		}
		protected virtual void OnResetSerializationProperties(OptionsLayoutBase options) {
			if(!NavBar.OptionsLayout.StoreAppearance) 
				return;
			Appearance.Reset();
			AppearanceHotTracked.Reset();
			AppearancePressed.Reset();
		}
		bool IDXImageUriClient.IsDesignMode {
			get { return DesignMode; }
		}
		DevExpress.LookAndFeel.UserLookAndFeel IDXImageUriClient.LookAndFeel {
			get {
				if(NavBar == null) return null;
				return NavBar.LookAndFeel;
			}
		}
		void IDXImageUriClient.SetGlyphSkinningValue(bool value) {
			AllowGlyphSkinning = (value ? DefaultBoolean.True : DefaultBoolean.False);
		}
		bool IDXImageUriClient.SupportsGlyphSkinning {
			get { return true; }
		}
		bool IDXImageUriClient.SupportsLookAndFeel {
			get { return true; }
		}
	}
	[TypeConverter("DevExpress.XtraNavBar.Design.NavBarItemLinkTypeConverter, " + AssemblyInfo.SRAssemblyNavBarDesign)]
	public class NavBarItemLink : ICollectionItem, IDisposable {
		NavLinkCollection collection;
		NavBarGroup group;
		event EventHandler itemChanged, visibleChanged;
		bool visible;
		NavBarItem item;
		bool allowAutoSelectCore;
		public NavBarItemLink(NavBarItem item) {
			if(item == null) throw new ArgumentException("item can't be null");
			this.group = null;
			this.collection = null;
			this.item = item;
			this.visible = Item.Visible;
			this.allowAutoSelectCore = Item.AllowAutoSelect;
		}
#if !SL
	[DevExpressXtraNavBarLocalizedDescription("NavBarItemLinkCaption")]
#endif
		[SearchColumn]
		public string Caption {
			get { return Item.Caption; }
		}
		public bool AllowHtmlString {
			get { return Item.GetAllowHtmlString(); }
		}
#if !SL
	[DevExpressXtraNavBarLocalizedDescription("NavBarItemLinkVisible")]
#endif
		public bool Visible {
			get {
				if(Item.IsDesignMode) return true;
				return visible && Item.IsVisible; 
			}
			set {
				if(Visible == value) return;
				visible = value;
				RaiseVisibleChanged();
			}
		}
		[XtraSerializableProperty, Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool VisibleCore {
			get { return visible; }
			set { visible = value; }
		}
		[Browsable(false)]
		public virtual Image GetImage() {
			if(Group == null) return null;
			if(Group.GetLinksUseSmallImage()) {
				if(Item.GetActualSmallImage() != null) return Item.GetActualSmallImage();
				return Group.GetImageCore(Item.SmallImages, Item.SmallImageIndex);
			}
			if(Item.GetActualLargeImage() != null) return Item.GetActualLargeImage();
			return Group.GetImageCore(Item.LargeImages, Item.LargeImageIndex);
		}
		[Browsable(false)]
		public virtual Size GetImageSize() {
			if(Group.GetLinksUseSmallImage()) {
				if(Item.GetActualSmallImage() != null) return Item.GetActualSmallImage().Size;
				if(Item.SmallImages != null) {
					if(NavBar.SharedImageCollectionImageSizeMode == SharedImageCollectionImageSizeMode.UseImageSize) {
						Size size = GetListImageSizeCore(Item.SmallImages, Item.SmallImageIndex);
						if(size != Size.Empty) return size;
					}
					return ImageCollection.GetImageListSize(item.SmallImages);
				}
				return new Size(8, 8);
			} 
			if(Item.GetActualLargeImage() != null) return Item.GetActualLargeImage().Size;
			if(Item.LargeImages != null) {
				if(NavBar.SharedImageCollectionImageSizeMode == SharedImageCollectionImageSizeMode.UseImageSize) {
					Size size = GetListImageSizeCore(Item.LargeImages, Item.LargeImageIndex);
					if(size != Size.Empty) return size;
				}
				return ImageCollection.GetImageListSize(item.LargeImages);
			}
			return new Size(16, 16);
		}
		protected virtual Size GetListImageSizeCore(object images, int index) {
			if(index == -1) return Size.Empty;
			Image image = ImageCollection.GetImageListImage(images, index);
			return image != null ? image.Size : Size.Empty;
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarItemLinkAllowAutoSelect"),
#endif
 DefaultValue(true)]
		public bool AllowAutoSelect { 
			get { return allowAutoSelectCore && Item.AllowAutoSelect; } 
			set { allowAutoSelectCore = value; }
		}
#if !SL
	[DevExpressXtraNavBarLocalizedDescription("NavBarItemLinkGroup")]
#endif
		public NavBarGroup Group { get { return group; } } 
#if !SL
	[DevExpressXtraNavBarLocalizedDescription("NavBarItemLinkItem")]
#endif
		public NavBarItem Item { get { return item; } }
#if !SL
	[DevExpressXtraNavBarLocalizedDescription("NavBarItemLinkNavBar")]
#endif
		public NavBarControl NavBar { 
			get {
				if(Item != null && Item.NavBar != null) return Item.NavBar;
				if(Group != null && Group.NavBar != null) return Group.NavBar;
				return null;
			} 
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool Enabled {
			get { return Item.Enabled; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual ObjectState State {
			get {
				ObjectState state = ObjectState.Normal;
				if(this == NavBar.SelectedLink) state |= ObjectState.Selected;
				if(this == NavBar.PressedLink) state |= ObjectState.Pressed;
				if(this == NavBar.HotTrackedLink) state |= ObjectState.Hot;
				return state;
			}
		}
		public virtual void Dispose() {
			if(Group != null) 
				Group.ItemLinks.Remove(this);
		}
		[Browsable(false), XtraSerializableProperty()]
		public string ItemName { 
			get { return Item.Name; }
			set { Item.Name = value; }
		}
		string ICollectionItem.ItemName { get { return Item.Name; } }
		Collection ICollectionItem.Collection { get { return collection; } }
		void ICollectionItem.SetCollection(Collection newCollection) {
			collection = newCollection as NavLinkCollection;
		}
		internal void SetNavGroupCore(NavBarGroup group) {
			this.group = group;
		}
		protected virtual void RaiseItemChanged() {
			if(itemChanged != null) itemChanged(this, EventArgs.Empty);
		}
		protected virtual void RaiseVisibleChanged() {
			if(visibleChanged != null) visibleChanged(this, EventArgs.Empty);
		}
#if !SL
	[DevExpressXtraNavBarLocalizedDescription("NavBarItemLinkVisibleChanged")]
#endif
		public event EventHandler VisibleChanged {
			add { visibleChanged += value; }
			remove { visibleChanged -= value; }
		}
#if !SL
	[DevExpressXtraNavBarLocalizedDescription("NavBarItemLinkItemChanged")]
#endif
		public event EventHandler ItemChanged { 
			add { itemChanged += value; }
			remove { itemChanged -= value; }
		}
		public void PerformClick() {
			if(!Enabled || NavBar == null) return;
			NavBar.RaiseLinkClicked(this);
		}
	}
	public class NavPaneMinimizedGroupFormShowingEventArgs : EventArgs {
		public NavPaneMinimizedGroupFormShowingEventArgs(NavBarControl navBar, NavPaneForm navPaneForm) {
			NavPaneForm = navPaneForm;
			NavBar = navBar;
		}
		public NavBarControl NavBar { get; private set; }
		public NavPaneForm NavPaneForm { get; private set; }
	}
	public class NavBarGroupEventArgs : EventArgs {
		NavBarGroup group;
		public NavBarGroupEventArgs(NavBarGroup group) {
			this.group = group;
		}
		public NavBarGroup Group { get { return group; } }
	}
	public class NavBarGroupCancelEventArgs : NavBarGroupEventArgs {
		public NavBarGroupCancelEventArgs(NavBarGroup group) : base(group) {
			Cancel = false;
		}
		public bool Cancel { get; set; }
	}
	public class NavBarLinkEventArgs : EventArgs {
		NavBarItemLink link;
		public NavBarLinkEventArgs(NavBarItemLink link) {
			this.link = link;
		}
		public NavBarItemLink Link { get { return link; } }
	}
	public class NavBarCustomHintEventArgs {
		NavBarHintInfo hintInfo;
		public NavBarCustomHintEventArgs(NavBarHintInfo hintInfo) {
			this.hintInfo = hintInfo;
		}
		public NavBarHintInfo HintInfo { get { return hintInfo; } }
		public NavBarItemLink Link { get { return HintInfo.HintObject as NavBarItemLink; } }
		public NavBarGroup Group { get { return HintInfo.HintObject as NavBarGroup; } }
	}
	public class NavBarGetHintEventArgs : NavBarCustomHintEventArgs {
		public NavBarGetHintEventArgs(NavBarHintInfo hintInfo) : base(hintInfo) { }
		public AppearanceObject Appearance {
			get { return HintInfo.Appearance; }
		}
		public string Hint {
			get { return HintInfo.Text; }
			set { HintInfo.Text = value; }
		}
	}
	public class NavBarCalcHintSizeEventArgs : NavBarCustomHintEventArgs {
		Size size;
		public NavBarCalcHintSizeEventArgs(NavBarHintInfo hintInfo, Size size) : base(hintInfo) {
			this.size = size;
		}
		public Size Size { get { return size; } set { size = value; } }
	}
	public class NavBarCustomDrawHintEventArgs : NavBarCustomHintEventArgs {
		PaintEventArgs paintArgs;
		Rectangle bounds;
		bool handled;
		public NavBarCustomDrawHintEventArgs(NavBarHintInfo hintInfo, PaintEventArgs args, Rectangle bounds) : base(hintInfo) {
			this.paintArgs = args;
			this.bounds = bounds;
			this.handled = false;
		}
		public AppearanceObject Appearance { get { return HintInfo.Appearance; } }
		public string Hint { get { return HintInfo.Text; } }
		public PaintEventArgs PaintArgs { get { return paintArgs; } }
		public Rectangle Bounds { get { return bounds; } }
		public bool Handled { 
			get { return handled; }
			set {
				handled = value;
			}
		}
	}
	public class NavPaneOptionsCanEditGroupFontEventArgs : EventArgs {
		public NavPaneOptionsCanEditGroupFontEventArgs(NavBarGroup group, bool result) {
			this.Group = group;
			this.Result = result;
		}
		public bool Result { get; set; }
		public NavBarGroup Group { get; private set; }
	}
	public class NavPaneOptionsApplyGroupFontEventArgs : EventArgs {
		public NavPaneOptionsApplyGroupFontEventArgs(NavBarGroup group, Font font) {
			this.Group = group;
			this.Font = font;
		}
		public Font Font { get; private set; }
		public NavBarGroup Group { get; private set; }
	}
	public class NavPaneOptionsResetEventArgs : EventArgs {
		public NavPaneOptionsResetEventArgs(NavBarControl navBar) {
			this.NavBar = navBar;
		}
		public NavBarControl NavBar { get; private set; }
	}
	public delegate void NavBarCustomDrawHintEventHandler(object sender, NavBarCustomDrawHintEventArgs e);
	public delegate void NavBarCalcHintSizeEventHandler(object sender, NavBarCalcHintSizeEventArgs e);
	public delegate void NavBarGetHintEventHandler(object sender, NavBarGetHintEventArgs e);
	public class NavBarCalcGroupClientHeightEventArgs : NavBarGroupEventArgs {
		int height;
		public NavBarCalcGroupClientHeightEventArgs(NavBarGroup group, int height) : base(group) {
			this.height = height;
		}
		public int Height { 
			get { return height; }
			set { height = value; }
		}
	}
	public delegate void NavBarCalcGroupClientHeightEventHandler(object sender, NavBarCalcGroupClientHeightEventArgs e);
	public delegate void NavBarGroupEventHandler(object sender, NavBarGroupEventArgs e);
	public delegate void NavBarGroupCancelEventHandler(object sender, NavBarGroupCancelEventArgs e);
	public delegate void NavBarLinkEventHandler(object sender, NavBarLinkEventArgs e);
}
