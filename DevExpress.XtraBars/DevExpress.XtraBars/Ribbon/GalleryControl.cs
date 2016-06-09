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
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars.Localization;
using DevExpress.XtraBars.Ribbon.Gallery;
using DevExpress.XtraBars.Ribbon.Helpers;
using DevExpress.XtraBars.Ribbon.ViewInfo;
namespace DevExpress.XtraBars.Ribbon {
	[SmartTagSupport(typeof(GalleryItemGroupDesignTimeBoundsProvider), SmartTagSupportAttribute.SmartTagCreationMode.Auto)]
	public class GalleryObjectDescriptor : IComponent, IDisposable, ICustomTypeDescriptor, IDXObjectWrapper, IChildVisualElement {
		private static readonly object eventDisposed = new object();
		private EventHandlerList events = new EventHandlerList();
		object item;
		private ISite site;
		IComponent parent;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
		public event EventHandler Disposed {
			add { events.AddHandler(eventDisposed, value); }
			remove { events.RemoveHandler(eventDisposed, value); }
		}
		public GalleryObjectDescriptor(object item, IComponent parent, RibbonGalleryBarItemLink link) {
			this.item = item;
			this.parent = parent;
			GalleryLink = link;
		}
		public void Dispose() { 
			Dispose(true);
		}
		protected virtual void Dispose(bool disposed) {
			if(disposed) {
				lock(this) {
					if((this.site != null) && (this.site.Container != null)) {
						this.site.Container.Remove(this);
					}
					if(this.events != null) {
						EventHandler handler = (EventHandler)this.events[eventDisposed];
						if(handler != null) {
							handler(this, EventArgs.Empty);
						}
					}
				}
			}
		}
		[RefreshProperties(RefreshProperties.All)]
		public virtual object Item { 
			get { return item; } 
			set { item = value; }
		}
		public GalleryItem GalleryItem { get { return Item as GalleryItem; } }
		public GalleryItemGroup GalleryGroup { get { return Item as GalleryItemGroup; } }
		public BaseGallery Gallery {
			get {
				if(GalleryItem != null)
					return GalleryItem.Gallery;
				return GalleryGroup.Gallery;
			}
		}
		public InRibbonGallery InRibbonGallery { get { return Gallery as InRibbonGallery; } }
		public StandaloneGallery StandaloneGallery { get { return Gallery as StandaloneGallery; } }
		public RibbonGalleryBarItemLink GalleryLink {
			get;
			internal set;
		}
		AttributeCollection ICustomTypeDescriptor.GetAttributes() { return TypeDescriptor.GetAttributes(Item, true); }
		string ICustomTypeDescriptor.GetClassName() { return TypeDescriptor.GetClassName(Item, true); }
		string ICustomTypeDescriptor.GetComponentName() { return ""; }
		TypeConverter ICustomTypeDescriptor.GetConverter() { return TypeDescriptor.GetConverter(Item, true); }
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() { return TypeDescriptor.GetDefaultEvent(Item, true); }
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() { return TypeDescriptor.GetDefaultProperty(Item, true); }
		object ICustomTypeDescriptor.GetEditor(Type editorBaseType) { return TypeDescriptor.GetEditor(Item, editorBaseType, true); }
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents() { return EventDescriptorCollection.Empty; }
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(System.Attribute[] attributes) { return EventDescriptorCollection.Empty; }
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() { return TypeDescriptor.GetProperties(Item, true); }
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(System.Attribute[] attributes) { return TypeDescriptor.GetProperties(Item, attributes, true); }
		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor p) { return Item; }
		object IDXObjectWrapper.SourceObject { get { return Item; } }
		public IComponent Parent { get { return this.parent; } }
		ISite IComponent.Site {
			get {
				InRibbonGallery gallery = Parent as InRibbonGallery;
				InDropDownGallery galleryDropDown = Parent as InDropDownGallery;
				if(this.site == null) {
					if(Parent.Site != null)
						this.site = new GalleryObjectSite(this, Parent.Site.Container);
					else if(gallery != null)
						this.site = new GalleryObjectSite(this, gallery.Ribbon.Container);
					else if(galleryDropDown != null)
						this.site = new GalleryObjectSite(this, galleryDropDown.GalleryDropDown.Container);
				}
				return this.site;
			}
			set {
			}
		}
		#region IChildVisualElement
		IComponent IChildVisualElement.Owner {
			get { return this.parent; }
		}
		#endregion
	}
	public class GalleryObjectSite : ISite, IServiceProvider {
		private IComponent component;
		private IContainer container;
		bool disposed = false;
		public GalleryObjectSite(IComponent component, IContainer container) {
			this.component = component;
			this.container = container;
		}
		object IServiceProvider.GetService(Type type) { return null; }
		IComponent ISite.Component { get { return this.component; } }
		IContainer ISite.Container { get { return this.container; } }
		bool ISite.DesignMode { get { return true; } }
		string ISite.Name { get { return this.component.GetType().FullName; } set { } }
		public bool Disposed {
			get { return disposed; }
			set { disposed = value; }
		}
	}
	[
	TypeConverter(typeof(UniversalTypeConverterEx)),
	SmartTagAction(typeof(GalleryItemDesignTimeActionsProvider), "AddGroup", "Add Group"),
	SmartTagAction(typeof(GalleryItemDesignTimeActionsProvider), "AddItem", "Add Item"),
	SmartTagAction(typeof(GalleryItemDesignTimeActionsProvider), "RemoveItem", "Remove Item", SmartTagActionType.CloseAfterExecute)
	]
	public class GalleryItem : ICloneable, DevExpress.Utils.MVVM.ISupportCommandBinding, IAppearanceOwner {
		GalleryItemGroup group;
		Image hoverImage;
		Image image;
		int imageIndex;
		int hoverImageIndex;
		string caption;
		string description;
		bool itemChecked;
		bool visible;
		bool enabled;
		object tag;
		string hint;
		SuperToolTip superTip;
		GalleryItem originItem;
		ImageLoadInfo imageInfo;
		StateAppearances appearanceCaption, appearanceDescription;
		protected internal ImageLoadInfo ImageInfo {
			get {
				if(imageInfo == null) {
					imageInfo = CreateImageLoadInfo();
				}
				return imageInfo;
			}
			set { imageInfo = value; }
		}
		bool ShouldSerializeAppearanceCaption() { return AppearanceCaption.ShouldSerialize(); }
		void ResetAppearanceCaption() { AppearanceCaption.Reset(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public StateAppearances AppearanceCaption {
			get {
				if(appearanceCaption == null)
					appearanceCaption = CreateAppearance();
				return appearanceCaption; 
			}
		}
		protected virtual StateAppearances CreateAppearance() {
			StateAppearances res = new StateAppearances(this);
			res.Changed += new EventHandler(OnApperanceChanged);
			return res;
		}
		private void OnApperanceChanged(object sender, EventArgs e) {
			if(Gallery != null)
				Gallery.UpdateGallery();
		}
		bool ShouldSerializeAppearanceDescription() { return AppearanceDescription.ShouldSerialize(); }
		void ResetAppearanceDescription() { AppearanceDescription.Reset(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public StateAppearances AppearanceDescription {
			get {
				if(appearanceDescription == null)
					appearanceDescription = CreateAppearance();
				return appearanceDescription; 
			}
		}
		protected ImageLoadInfo CreateImageLoadInfo() {
			if(Gallery == null || GalleryGroup == null) return null;
			int index = GalleryGroup.Items.IndexOf(this);
			Size desiredSize = Gallery.OptionsImageLoad.DesiredThumbnailSize;
			desiredSize = desiredSize == Size.Empty ? Gallery.ImageSize : desiredSize;
			ImageContentAnimationType animationType = Gallery.OptionsImageLoad.AnimationType;
			ImageLayoutMode mode = Gallery.ItemImageLayout;
			if(mode == ImageLayoutMode.Default) mode = ImageLayoutMode.MiddleCenter;
			return new GalleryImageLoadInfo(index, this, animationType, mode, Gallery.ImageSize, desiredSize);
		}
		object fValue;
		public GalleryItem(GalleryItem source) : this(source.Image, source.HoverImage, source.Caption, source.Description, source.ImageIndex, source.HoverImageIndex, source.tag, source.Hint, source.itemChecked) { }
		public GalleryItem(Image image, Image hoverImage, string caption, string description, int imageIndex, int hoverImageIndex, object tag, string hint, bool visible, bool itemChecked)
			: this(image, hoverImage, caption, description, imageIndex, hoverImageIndex, tag, hint, itemChecked) {
			this.visible = visible;
		}
		public GalleryItem(Image image, Image hoverImage, string caption, string description, int imageIndex, int hoverImageIndex, object tag, string hint, bool itemChecked) : 
		this(image, hoverImage, caption, description, imageIndex, hoverImageIndex, tag, hint){
			this.itemChecked = itemChecked;
		}
		public GalleryItem(Image image, Image hoverImage, string caption, string description, int imageIndex, int hoverImageIndex, object tag, string hint) : this(image, caption, description) {
			this.hint = hint;
			this.tag = tag;
			this.hoverImage = hoverImage;
			this.ImageIndex = imageIndex;
			this.HoverImageIndex = hoverImageIndex;
		}
		public GalleryItem(Image image, string caption, string description) : this() {
			this.image = image;
			this.caption = caption;
			this.description = description;
		}
		public GalleryItem() {
			this.image = null;
			this.hint = this.caption = string.Empty;
			this.description = string.Empty;
			this.hoverImage = null;
			this.group = null;
			this.itemChecked = false;
			this.imageIndex = -1;
			this.hoverImageIndex = -1;
			this.visible = true;
			this.enabled = true;
			this.lockRefresh = false;
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("GalleryItemValue"),
#endif
 Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter)), SmartTagProperty("Value", "", 1),
		]
		public virtual object Value {
			get { return fValue; }
			set {
				if(Value == value)
					return;
				fValue = value;
				OnPropertiesChanged("Value");
			}
		}
		public bool Focus() {
			if(Gallery == null)
				return false;
			Gallery.FocusedItem = this;
			return Gallery.FocusedItem == this;
		}
		protected internal virtual bool ShouldSerializeValue() {
			return Value != null;
		}
		protected internal virtual void ResetValue() {
			Value = null;
		}
		[Browsable(false)]
		public GalleryItem OriginItem { get { return originItem; } }
		internal void SetOriginItem(GalleryItem item) { this.originItem = item; }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public GalleryItemGroup GalleryGroup {  get { return group; } }
		internal void SetGroup(GalleryItemGroup group) {
			this.group = group; 
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BaseGallery Gallery { 
			get {
				if(GalleryGroup == null) return null;
				return GalleryGroup.Gallery; 
			}
		}
		public void MakeVisible() {
			if(Gallery != null) Gallery.MakeVisible(this);
		}
		protected internal virtual void LayoutChanged() {
			if(Gallery != null) Gallery.LayoutChanged();
		}
		protected internal virtual void RefreshGallery() {
			if(Gallery != null) Gallery.RefreshGallery();
		}
		protected internal virtual void UpdateGallery() {
			if(Gallery != null) Gallery.UpdateGallery();
		}
		bool lockRefresh;
		protected internal void LockRefresh() {
			this.lockRefresh = true;
		}
		protected internal void UnlockRefresh() {
			this.lockRefresh = false;
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("GalleryItemImage"),
#endif
 Category("Appearance"), SmartTagProperty("Image", "Image", 0, SmartTagActionType.RefreshAfterExecute), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public Image Image {
			get { return image; }
			set {
				if(image == value) return;
				image = value;
				OnPropertiesChanged();
			}
		}
		protected internal virtual bool ShouldSerializeImage() {
			return Image != null;
		}
		protected internal virtual void ResetImage() {
			Image = null;
		}
		protected virtual void OnPropertiesChanged(string propName) {
			OnPropertiesChanged(propName, true);
		}
		protected virtual void OnPropertiesChanged(string propName, bool updateLayout) {
			FireGalleryChanged();
			if(Gallery != null)
				Gallery.OnItemPropertiesChanged(propName, updateLayout);
			if(updateLayout)
				LayoutChanged();
			else
				UpdateVisual();
		}
		protected virtual void UpdateVisual() {
			if(Gallery != null)
				Gallery.UpdateVisual();
		}
		protected virtual void OnPropertiesChanged() {
			if(lockRefresh) return;
			OnPropertiesChanged("");
		}
		protected virtual void FireGalleryChanged() {
			if(Gallery != null)
				Gallery.FireGalleryChanged();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), 
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object Images {
			get {
				if(Gallery == null) return null;
				return Gallery.Images;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), 
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object HoverImages {
			get {
				if(Gallery == null) return null;
				return Gallery.HoverImages;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("GalleryItemHoverImage"),
#endif
 Category("Appearance"), DefaultValue(null), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor)), SmartTagProperty("Hover Image", "Image", 10, SmartTagActionType.RefreshAfterExecute)]
		public Image HoverImage {
			get { return hoverImage; }
			set {
				if(HoverImage == value)
					return;
				hoverImage = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("GalleryItemCaption"),
#endif
 Category("Appearance"), Localizable(true), SmartTagProperty("Caption", "Appearance", 0, SmartTagActionType.RefreshAfterExecute), SearchColumn]
		public virtual string Caption {
			get { return caption; }
			set {
				if(value == null) value = string.Empty;
				if(caption == value) return;
				caption = value;
				OnPropertiesChanged("Caption");
			}
		}
		protected internal virtual bool ShouldSerializeCaption() {
			return !string.IsNullOrEmpty(Caption);
		}
		protected internal virtual void ResetCaption() {
			Caption = string.Empty;
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("GalleryItemDescription"),
#endif
 Category("Appearance"), Localizable(true), Editor(ControlConstants.MultilineStringEditor, typeof(UITypeEditor)), SmartTagProperty("Description", "Appearance", 10, SmartTagActionType.RefreshAfterExecute)]
		public string Description {
			get { return description; }
			set {
				if(value == null) value = string.Empty;
				if(description == value) return;
				description = value;
				OnPropertiesChanged();
			}
		}
		protected internal virtual bool ShouldSerializeDescription() {
			return !string.IsNullOrEmpty(Description);
		}
		protected internal virtual void ResetDescription() {
			Description = string.Empty;
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("GalleryItemHint"),
#endif
  Localizable(true)]
		public string Hint {
			get { return hint; }
			set {
				if(value == null) 
					value = string.Empty;
				if(Hint == value)
					return;
				hint = value;
				OnPropertiesChanged();
			}
		}
		protected internal virtual bool ShouldSerializeHint() {
			return !String.IsNullOrEmpty(Hint);
		}
		protected internal virtual void ResetHint() {
			Hint = String.Empty;
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("GalleryItemImageIndex"),
#endif
 Category("Appearance"), Editor(typeof(ImageIndexesEditor), typeof(UITypeEditor)), ImageList("Images"), DefaultValue(-1), SmartTagProperty("Image Index", "Image", 20, SmartTagActionType.RefreshAfterExecute)]
		public int ImageIndex {
			get { return imageIndex; }
			set {
				if(value < -1) value = -1;
				if(Gallery != null && value >= ImageCollection.GetImageListImageCount(Gallery.Images)) value = -1;
				if(ImageIndex == value) return;
				imageIndex = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("GalleryItemHoverImageIndex"),
#endif
 Category("Appearance"), Editor(typeof(ImageIndexesEditor), typeof(UITypeEditor)), ImageList("HoverImages"), DefaultValue(-1), SmartTagProperty("Hover Image Index", "Image", 40, SmartTagActionType.RefreshAfterExecute)]
		public int HoverImageIndex {
			get { return hoverImageIndex; }
			set {
				if(value < -1) value = -1;
				if(Gallery != null && value >= ImageCollection.GetImageListImageCount(Gallery.HoverImages)) value = -1;
				if(HoverImageIndex == value) return;
				hoverImageIndex = value;
				RefreshGallery();
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("GalleryItemChecked"),
#endif
 Category("Behavior"), DefaultValue(false), SmartTagProperty("Checked", "Appearance", 60)]
		public bool Checked {
			get { return itemChecked; }
			set {
				if(itemChecked == value) return;
				itemChecked = value;
				if(Gallery != null) 
					Gallery.OnCheckedChanged(this);
				OnPropertiesChanged("Checked", false);
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("GalleryItemVisible"),
#endif
		System.ComponentModel.Category("Behavior"), DefaultValue(true)]
		public bool Visible {
			get { return visible; }
			set {
				if(visible == value) return;
				visible = value;
				if(Gallery != null) 
					Gallery.OnItemVisibleChanged(this);
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("GalleryItemEnabled"),
#endif
 System.ComponentModel.Category("Behavior"), DefaultValue(true)]
		public bool Enabled {
			get { return enabled; }
			set {
				if(enabled == value) return;
				enabled = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("GalleryItemTag"),
#endif
 DefaultValue(null), Category("Data"), Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(UITypeEditor)),
		TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter)), SmartTagProperty("Tag", "", 0)]
		public object Tag {
			get { return tag; }
			set {
				if(Tag == value)
					return;
				tag = value;
				OnPropertiesChanged();
			}
		}
		bool ShouldSerializeSuperTip() { return SuperTip != null && !SuperTip.IsEmpty; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("GalleryItemSuperTip"),
#endif
		Editor("DevExpress.XtraEditors.Design.ToolTipContainerUITypeEditor, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(System.Drawing.Design.UITypeEditor)),
		Category("Appearance"), Localizable(true), SmartTagProperty("Super Tip", "Appearance", 20)
		]
		public SuperToolTip SuperTip {
			get { return superTip; }
			set {
				if(SuperTip == value)
					return;
				superTip = value;
				OnPropertiesChanged();
			}
		}
		public void ResetSuperTip() { SuperTip = null; }
		protected virtual SuperToolTip GetSuperTip() {
			if(SuperTip != null && !SuperTip.IsEmpty) return SuperTip;
			if(Hint == "") return null;
			SuperToolTip res = new SuperToolTip();
			SuperToolTipSetupArgs args = new SuperToolTipSetupArgs();
			args.Contents.Text = Hint;
			res.Setup(args);
			return res;
		}
		protected internal virtual ToolTipControlInfo GetHoverTooltip(ToolTipControlInfo info, RibbonHitInfo hi) {
			if(Gallery == null) return info;
			GalleryItemImagePopupForm popupForm = Gallery.FindForm(hi.GalleryItemInfo);
			if(popupForm == null || !popupForm.Active) return null;
			info.ToolTipLocation = ToolTipLocation.BottomCenter;
			info.ToolTipPosition = new Point(popupForm.Left + popupForm.Width / 2, popupForm.Bottom + 6 - DevExpress.Utils.Win.CursorInfo.CursorBounds.Height);
			return info;
		}
		protected internal virtual ToolTipControlInfo GetToolTipInfo(RibbonHitInfo hi, Point point) {
			ToolTipControlInfo info = new ToolTipControlInfo();
			info.SuperTip = GetSuperTip();
			info.Text = Hint;
			info.Object = this;
			info.IconType = ToolTipIconType.None;
			info.ToolTipLocation = ToolTipLocation.BottomRight;
			info.HideHintOnMouseMove = false;
			info.Interval = 800;
			return info;
		}
		public virtual void Assign(GalleryItem galleryItem) {
			this.caption = galleryItem.caption;
			this.description = galleryItem.description;
			this.hint = galleryItem.Hint;
			this.hoverImage = galleryItem.HoverImage;
			this.hoverImageIndex = galleryItem.HoverImageIndex;
			this.image = galleryItem.Image;
			this.imageIndex = galleryItem.ImageIndex;
			this.itemChecked = galleryItem.Checked;
			if(galleryItem.SuperTip != null)
			this.superTip = (SuperToolTip)galleryItem.SuperTip.Clone();
			this.tag = galleryItem.Tag;
			this.visible = galleryItem.Visible;
			this.Value = galleryItem.Value;
			this.enabled = galleryItem.Enabled;
		}
		#region ICloneable Members
		protected virtual GalleryItem CreateItem() { return new GalleryItem(); }
		protected virtual GalleryItem CloneCore() {
			GalleryItem item = CreateItem();
			item.SetOriginItem(this);
			item.Assign(this);
			return item;
		}
		public object Clone() {
			return CloneCore();
		}
		#endregion
		internal void SetCheckedInternal(bool isChecked) {
			itemChecked = isChecked;
		}
		protected internal void RaiseItemClickEvent(RibbonGalleryBarItemLink link) {
			if(ItemClick != null)
				ItemClick.Invoke(this, new GalleryItemClickEventArgs(link, Gallery, this));
		}
		public event GalleryItemClickEventHandler ItemClick;
		#region Commands
		public IDisposable BindCommand(object command, Func<object> queryCommandParameter = null) {
			return DevExpress.Utils.MVVM.CommandHelper.Bind(this,
				(galleryItem, execute) => galleryItem.ItemClick += (s, e) => execute(),
				(galleryItem, canExecute) => galleryItem.Enabled = canExecute(),
				command, queryCommandParameter);
		}
		public IDisposable BindCommand(System.Linq.Expressions.Expression<Action> commandSelector, object source, Func<object> queryCommandParameter = null) {
			return DevExpress.Utils.MVVM.CommandHelper.Bind(this,
				(galleryItem, execute) => galleryItem.ItemClick += (s, e) => execute(),
				(galleryItem, canExecute) => galleryItem.Enabled = canExecute(),
				commandSelector, source, queryCommandParameter);
		}
		public IDisposable BindCommand<T>(System.Linq.Expressions.Expression<Action<T>> commandSelector, object source, Func<T> queryCommandParameter = null) {
			return DevExpress.Utils.MVVM.CommandHelper.Bind(this,
				(galleryItem, execute) => galleryItem.ItemClick += (s, e) => execute(),
				(galleryItem, canExecute) => galleryItem.Enabled = canExecute(),
				commandSelector, source, () => (queryCommandParameter != null) ? queryCommandParameter() : default(T));
		}
		#endregion Commands
		bool IAppearanceOwner.IsLoading {
			get { return false; }
		}
	}
	public class GalleryItemCollection : CollectionBase, IEnumerable<GalleryItem>, ISupportSearchDataAdapter {
		GalleryItemGroup group;
		SearchDataAdapter adapter;
		public GalleryItemCollection(GalleryItemGroup group) {
			this.group = group;
			this.adapter = new SearchDataAdapter<GalleryItem>();
			this.adapter.SetDataSource(List);
		}
		public GalleryItemCollection() { }
#if !SL
	[DevExpressXtraBarsLocalizedDescription("GalleryItemCollectionGroup")]
#endif
		public GalleryItemGroup Group { get { return group; } set { group = value; } }
		public int Add(GalleryItem item) {
			return List.Add(item);
		}
		public void AddRange(GalleryItem[] items) {
			foreach(GalleryItem item in items) {
				Add(item);
			}
		}
		public void Insert(int index, GalleryItem item) {
			List.Insert(index, item);
		}
		public void Remove(GalleryItem item) {
			List.Remove(item);
		}
		public int IndexOf(GalleryItem item) {
			int index = List.IndexOf(item);
			return adapter.GetControllerRow(index);
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("GalleryItemCollectionItem")]
#endif
		public GalleryItem this[int index] { get { return adapter.GetValueAtIndex(string.Empty, index) as GalleryItem; } }
		void RefreshGallery() {
			if(Group == null || Group.Gallery == null) return;
			Group.Gallery.RefreshGallery();
		}
		void LayoutChanged() {
			if(Group != null && Group.Gallery != null)
				Group.Gallery.LayoutChanged();
		}
		protected virtual void OnCollectionChanged() {
			if(Group != null && Group.Gallery != null)
				Group.Gallery.OnItemCollectionChanged(this);
			LayoutChanged();
		}
		protected override void OnInsertComplete(int index, object value) {
			GalleryItem item = (GalleryItem)value;
			base.OnInsertComplete(index, value);
			item.SetGroup(Group);
			OnCollectionChanged();
		}
		protected override void OnRemoveComplete(int index, object value) {
			base.OnRemoveComplete(index, value);
			GalleryItem item = (GalleryItem)value;
			item.SetGroup(null);
			OnCollectionChanged();
		}
		protected override void OnClear() {
			base.OnClear();
			foreach(GalleryItem item in this) {
				item.SetGroup(null);
			}
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			OnCollectionChanged();
		}
		protected override void OnSetComplete(int index, object oldValue, object newValue) {
			base.OnSetComplete(index, oldValue, newValue);
			OnCollectionChanged();
		}
		public bool Contains(GalleryItem item) { return List.Contains(item); }
		public new int Count { get { return adapter.VisibleCount; } }
		#region IEnumerable
		IEnumerator<GalleryItem> IEnumerable<GalleryItem>.GetEnumerator() {
			foreach(GalleryItem item in List) {
				yield return item;
			}
		}
		#endregion
		#region ISupportSearchDataAdapter Members
		bool ISupportSearchDataAdapter.AdapterEnabled {
			get { return true; }
			set { }
		}
		Data.Filtering.CriteriaOperator ISupportSearchDataAdapter.FilterCriteria {
			get { return adapter.FilterCriteria; }
			set { adapter.FilterCriteria = value; }
		}
		int ISupportSearchDataAdapter.GetSourceIndex(int filteredIndex) {
			object val = adapter.GetValueAtIndex(string.Empty, filteredIndex);
			return List.IndexOf(val);
		}
		int ISupportSearchDataAdapter.GetVisibleIndex(int index) { return index; }
		int ISupportSearchDataAdapter.VisibleCount { get { return adapter.VisibleCount; } }
		#endregion
	}
	public enum GalleryItemGroupCaptionAlignment { Left, Center, Right, Stretch }
	[
	TypeConverter(typeof(UniversalTypeConverterEx)),
	SmartTagAction(typeof(GalleryItemGroupDesignTimeActionsProvider), "AddGroup", "Add Group"),
	SmartTagAction(typeof(GalleryItemGroupDesignTimeActionsProvider), "AddItem", "Add Item"),
	SmartTagAction(typeof(GalleryItemGroupDesignTimeActionsProvider), "RemoveGroup", "Remove Group", SmartTagActionType.CloseAfterExecute),
	]
	public class GalleryItemGroup : IDisposable, ICloneable {
		GalleryItemGroupCollection collection;
		GalleryItemCollection items;
		string caption;
		bool visible;
		string name = string.Empty;
		object tag;
		Control captionControl;
		GalleryItemGroupCaptionAlignment captionAlignment;
		Size captionControlSize;
		public GalleryItemGroup() {
			this.items = new GalleryItemCollection(this);
			this.caption = string.Empty;
			this.visible = true;
			this.captionControlSize = Size.Empty;
			this.captionControl = null;
			this.captionAlignment = GalleryItemGroupCaptionAlignment.Left;
		}
		public GalleryItemGroup(GalleryItemGroup sourceGroup) : this() {
			Assign(sourceGroup);
		}
		public virtual void Assign(GalleryItemGroup sourceGroup) {
			this.visible = sourceGroup.Visible;
			this.caption = sourceGroup.Caption;
			this.captionAlignment = sourceGroup.CaptionAlignment;
			this.captionControlSize = sourceGroup.CaptionControlSize;
			this.tag = sourceGroup.Tag;
			for(int i = 0; i < sourceGroup.Items.Count; i++) {
				Items.Add((GalleryItem)sourceGroup.Items[i].Clone());
			}
		}
		public virtual void Dispose() {
			if(Collection != null) Collection.Remove(this);
			this.collection = null;
			Items.Clear();
			Dispose(true);
		}
		protected internal virtual void Dispose(bool disposing) {
		}
		protected virtual void OnPropertiesChanged() {
			if(Gallery != null)
				Gallery.FireGalleryChanged();
			LayoutChanged();
		}
		bool ShouldSerializeCaptionControl() { return CaptionControl != null; }
		void ResetCaptionContentControl() { CaptionControl = null; }
		public Control CaptionControl {
			get { return captionControl; }
			set {
				if(CaptionControl == value)
					return;
				if(!IsGroupCaptionControlUsageAllowed)
					return;
				Control prevControl = CaptionControl;
				captionControl = value;
				OnCaptionControlChanged(prevControl, CaptionControl);
			}
		}
		protected virtual bool IsGroupCaptionControlUsageAllowed {
			get { return Gallery == null || Gallery.IsGroupCaptionControlAllowed; }
		}
		bool ShouldSerializeCaptionControlSize() { return CaptionControlSize != Size.Empty; }
		void ResetCaptionControlSize() { CaptionControlSize = Size.Empty; }
		public Size CaptionControlSize {
			get { return captionControlSize; }
			set {
				if(CaptionControlSize == value)
					return;
				captionControlSize = value;
				OnCaptionControlSizeChanged();
			}
		}
		protected virtual void OnCaptionControlSizeChanged() {
			OnPropertiesChanged();
		}
		protected virtual void OnCaptionControlChanged(Control prevControl, Control newControl) {
			if(CaptionControlSize == Size.Empty && CaptionControl != null)
				CaptionControlSize = CaptionControl.Size;
			else if(CaptionControl != null) {
				CaptionControl.Size = CaptionControlSize;
			}
			if(Gallery != null)
				Gallery.OnGalleryGroupCaptionControlChanged(this, prevControl, newControl);
			OnPropertiesChanged();
		}
		bool ShouldSerializeCaptionAlignment() { return CaptionAlignment != GalleryItemGroupCaptionAlignment.Left; }
		void ResetCaptionAlignment() { CaptionAlignment = GalleryItemGroupCaptionAlignment.Left; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("GalleryItemGroupCaptionAlignment"),
#endif
 Category("Appearance"), SmartTagProperty("Caption Alignment", "Appearance")]
		public GalleryItemGroupCaptionAlignment CaptionAlignment {
			get { return captionAlignment; }
			set {
				if(CaptionAlignment == value)
					return;
				captionAlignment = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("GalleryItemGroupTag"),
#endif
 DefaultValue(null), Category("Data"),
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))]
		public object Tag {
			get { return tag; }
			set {
				if(Tag == value)
					return;
				tag = value;
				OnPropertiesChanged();
			}
		}
		internal void ReplaceCollection(GalleryItemGroupCollection newCollection) {
			if(newCollection != this.collection) {
				if(this.collection != null) collection.InternalRemove(this);
				this.collection = newCollection;
			}
		}
		protected GalleryItemGroupCollection Collection { get { return collection; } }
		internal void SetCollection(GalleryItemGroupCollection collection) { 
			this.collection = collection;
			OnCollectionChanged();
		}
		protected virtual void OnCollectionChanged() {
			if(Gallery != null)
				Gallery.OnGalleryGroupCaptionControlChanged(this, null, CaptionControl);
			else
				if(CaptionControl != null && CaptionControl.Parent != null)
					CaptionControl.Parent.Controls.Remove(CaptionControl);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BaseGallery Gallery { get { return Collection == null ? null : Collection.Gallery; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(false)]
		public GalleryItemCollection Items { get { return items; } }
		protected internal virtual void RefreshGallery() { if(Gallery != null) Gallery.RefreshGallery(); }
		protected internal virtual void LayoutChanged() { if(Gallery != null) Gallery.LayoutChanged(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("GalleryItemGroupCaption"),
#endif
 Category("Appearance"), Localizable(true), SmartTagProperty("Caption", "Appearance", SmartTagActionType.RefreshAfterExecute)]
		public string Caption {
			get { return caption; }
			set {
				if(caption == value) return;
				caption = value;
				OnPropertiesChanged();
			}
		}
		protected internal virtual bool ShouldSerializeCaption() {
			return !String.IsNullOrEmpty(Caption);
		}
		protected internal virtual void ResetCaption() {
			Caption = String.Empty;
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("GalleryItemGroupVisible"),
#endif
		System.ComponentModel.Category("Behavior"), DefaultValue(true)]
		public bool Visible {
			get { return visible; }
			set {
				if(visible == value) return;
				visible = value;
				OnVisibleChanged();
			}
		}
		protected virtual void OnVisibleChanged() {
			if(CaptionControl != null)
				CaptionControl.Visible = Visible;
			OnPropertiesChanged();
		}
		#region ICloneable Members
		protected virtual GalleryItemGroup CreateGroup() { return new GalleryItemGroup(); }
		protected virtual GalleryItemGroup CloneCore() {
			GalleryItemGroup group = CreateGroup();
			group.Assign(this);
			return group;
		}
		public object Clone() {
			return CloneCore();
		}
		#endregion
		public List<GalleryItem> GetCheckedItems() {
			List<GalleryItem> res = new List<GalleryItem>();
			foreach(GalleryItem item in Items) {
				if(item.Checked)
					res.Add(item);
			}
			return res;
		}
		internal void SetCaptionControlCore(object p) {
			this.captionControl = null;
		}
		public GalleryItem GetItemByValue(object value) {
			foreach(GalleryItem item in Items) {
				if(item.Value != null && item.Value.Equals(value))
					return item;
			}
			return null;
		}
		public bool HasVisibleItems() {
			foreach(GalleryItem item in Items) {
				if(item.Visible)
					return true;
			}
			return false;
		}
	}
	public class GalleryItemGroupCollection : CollectionBase, IEnumerable<GalleryItemGroup> { 
		BaseGallery gallery;
		public GalleryItemGroupCollection() { }
		public GalleryItemGroupCollection(BaseGallery gallery) {
			this.gallery = gallery;
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("GalleryItemGroupCollectionGallery")]
#endif
		public BaseGallery Gallery { get { return gallery; } set { gallery = value; } }
		public int Add(GalleryItemGroup item) {
			if(List.Contains(item)) return IndexOf(item);
			return List.Add(item);
		}
		public void AddRange(GalleryItemGroup[] groups) {
			foreach(GalleryItemGroup item in groups) {
				Add(item);
			}
		}
		public void Insert(int index, GalleryItemGroup group) { 
			List.Insert(index, group);
		}
		public void Remove(GalleryItemGroup group) { 
			List.Remove(group);
		}
		internal void EnsureGroupVisible() {
			foreach(GalleryItemGroup group in this) {
				if(group.Visible) return;
			}
			if(Count > 0) this[0].Visible = true;
		}
		internal void InternalRemove(GalleryItemGroup group) { if(InnerList.Contains(group)) InnerList.Remove(group); }
		public bool Contains(GalleryItemGroup group) { return List.Contains(group); }
#if !SL
	[DevExpressXtraBarsLocalizedDescription("GalleryItemGroupCollectionItem")]
#endif
public GalleryItemGroup this[int index] { get { return List[index] as GalleryItemGroup; } }
		public int IndexOf(GalleryItemGroup group) { return List.IndexOf(group); }
		protected virtual void RefreshGallery() {
			if(Gallery != null) Gallery.RefreshGallery();
		}
		protected virtual void LayoutChanged() {
			if(Gallery != null) Gallery.LayoutChanged();
		}
		protected internal bool GetGroupsVisibility() {
			foreach(GalleryItemGroup group in this) {
				if(!group.Visible) return false;
			}
			return true;
		}
		protected virtual void OnCollectionChanged() {
			if(Gallery != null)
				Gallery.OnGroupCollectionChanged(this);
			LayoutChanged();
		}
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			GalleryItemGroup group = (GalleryItemGroup)value;
			group.SetCollection(this);
			OnCollectionChanged();
		}
		protected override void OnRemoveComplete(int index, object value) {
			base.OnRemoveComplete(index, value);
			GalleryItemGroup group = (GalleryItemGroup)value;
			group.SetCollection(null);
			OnCollectionChanged();
		}
		protected override void OnClear() {
			base.OnClear();
			foreach(GalleryItemGroup group in this) {
				group.SetCollection(null);
			}
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			OnCollectionChanged();
		}
		protected override void OnSetComplete(int index, object oldValue, object newValue) {
			base.OnSetComplete(index, oldValue, newValue);
			OnCollectionChanged();
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("GalleryItemGroupCollectionVisibleGroupsCount")]
#endif
		public int VisibleGroupsCount {
			get {
				int count = 0;
				for(int i = 0; i < Count; i++) {
					if(this[i].Visible) count++;
				}
				return count;
			}
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("GalleryItemGroupCollectionVisibleGroups")]
#endif
		public ArrayList VisibleGroups {
			get {
				ArrayList list = new ArrayList();
				for(int n = 0; n < Count; n++) {
					if(this[n].Visible) list.Add(this[n]);
				}
				return list;
			}
		}
		#region IEnumerable
		IEnumerator<GalleryItemGroup> IEnumerable<GalleryItemGroup>.GetEnumerator() {
			foreach(GalleryItemGroup group in List) yield return group;
		}
		#endregion
	}
	public enum ItemImageLocation { Default, Left, Right, Top, Bottom }
	public enum GallerySizeMode { Default, None, Vertical, Both }
	public class GalleryItemEventArgs : EventArgs {
		RibbonGalleryBarItemLink inRibbonGalleryLink;
		BaseGallery gallery;
		GalleryItem item;
		public GalleryItemEventArgs(RibbonGalleryBarItemLink inRibbonGalleryLink, BaseGallery gallery, GalleryItem item) :
				this(gallery, item) {
			this.inRibbonGalleryLink = inRibbonGalleryLink;
		}
		public GalleryItemEventArgs(BaseGallery gallery, GalleryItem item) {
			this.gallery = gallery;
			this.item = item;
			this.inRibbonGalleryLink = null;
		}
		public BaseGallery Gallery { get { return gallery; } }
		public GalleryItem Item { get { return item; } }
		public RibbonGalleryBarItemLink InRibbonGalleryLink { get { return inRibbonGalleryLink; } }
	}
	public class GalleryItemClickEventArgs : GalleryItemEventArgs {
		public GalleryItemClickEventArgs(RibbonGalleryBarItemLink inRibbonGalleryLink, BaseGallery gallery, GalleryItem item) : base(inRibbonGalleryLink, gallery, item) { }   
	}
	public class GalleryItemCustomDrawEventArgs : GalleryItemEventArgs {
		object itemInfo;
		Rectangle bounds;
		GraphicsCache cache;
		bool handled = false;
		public GalleryItemCustomDrawEventArgs(GraphicsCache cache, BaseGallery gallery, object itemInfo, Rectangle bounds)
			: base(gallery, ((GalleryItemViewInfo)itemInfo).Item) {
			this.cache = cache;
			this.itemInfo = itemInfo;
			this.bounds = bounds;
		}
		public object ItemInfo { get { return itemInfo; } }
		public Rectangle Bounds { get { return bounds; } }
		public bool Handled { get { return handled; } set { handled = value; } }
		public GraphicsCache Cache { get { return cache; } }
	}
	public class InplaceGalleryEventArgs : EventArgs {
		InDropDownGallery popupGallery;
		RibbonGalleryBarItem item;
		public InplaceGalleryEventArgs(RibbonGalleryBarItem item, InDropDownGallery popupGallery) {
			this.item = item;
			this.popupGallery = popupGallery;
		}
		public InDropDownGallery PopupGallery { get { return popupGallery; } }
		public RibbonGalleryBarItem Item { get { return item; } }
	}
	public delegate void GalleryItemCustomDrawEventHandler(object sender, GalleryItemCustomDrawEventArgs e);
	public delegate void GalleryItemClickEventHandler(object sender, GalleryItemClickEventArgs e);
	public delegate void GalleryItemEventHandler(object sender, GalleryItemEventArgs e);
	public delegate void GallerySelectionEventHandler(object sender, GallerySelectionEventArgs e);
	public delegate void InplaceGalleryEventHandler(object sender, InplaceGalleryEventArgs e);
	public delegate void GalleryFilterMenuEventHandler(object sender, GalleryFilterMenuEventArgs e);
	public delegate void GalleryFilterMenuItemClickEventHandler(object sender, GalleryFilterMenuClickEventArgs e);
	public class GallerySelectionEventArgs : EventArgs {
		BaseGallery gallery;
		List<GalleryItem> selection;
		public GallerySelectionEventArgs(BaseGallery gallery, List<GalleryItem> selection) {
			this.gallery = gallery;
			this.selection = selection;
		}
		public BaseGallery Gallery { get { return gallery; } }
		public List<GalleryItem> Selection { get { return selection; } }
	}
	public class GalleryFilterMenuEventArgs : EventArgs {
		ImageGalleryFilterMenu filterMenu;
		public GalleryFilterMenuEventArgs(ImageGalleryFilterMenu filterMenu) {
			this.filterMenu = filterMenu;		
		}
		public ImageGalleryFilterMenu FilterMenu { get { return filterMenu; } }
		public StandaloneGallery Gallery { get { return FilterMenu.Gallery; } }
	}
	public class GalleryFilterMenuClickEventArgs : EventArgs {
		ImageGalleryFilterMenu filterMenu;
		DXMenuCheckItem item;
		public GalleryFilterMenuClickEventArgs(ImageGalleryFilterMenu filterMenu, DXMenuCheckItem item) {
			this.filterMenu = filterMenu;
			this.item = item;
		}
		public DXMenuCheckItem Item { get { return item; } }
		public ImageGalleryFilterMenu FilterMenu { get { return filterMenu; } }
		public StandaloneGallery Gallery { get { return FilterMenu.Gallery; } }
	}
	[ToolboxItem(false)]
	public class ImageGalleryFilterMenu : DXPopupMenu {
		StandaloneGallery gallery;
		public ImageGalleryFilterMenu(StandaloneGallery gallery)
			: base() {
			this.gallery = gallery;
			CreateMenuItems();
		}
		public StandaloneGallery Gallery { get { return gallery; } }
		protected internal virtual void CreateMenuItems() {
			if(Gallery.Groups.Count > 1) {
				foreach(GalleryItemGroup group in Gallery.Groups) {
					DXMenuCheckItem checkItem = new DXMenuCheckItem();
					checkItem.CloseMenuOnClick = false;
					checkItem.Checked = group.Visible;
					checkItem.Caption = group.Caption;
					checkItem.Tag = group;
					checkItem.Click += new EventHandler(OnItemClick);
					Items.Add(checkItem);
				}
				DXMenuCheckItem allItems = new DXMenuCheckItem();
				allItems.CloseMenuOnClick = false;
				allItems.BeginGroup = true;
				allItems.Checked = Gallery.Groups.GetGroupsVisibility();
				allItems.Caption = BarLocalizer.Active.GetLocalizedString(BarString.RibbonGalleryFilter);
				allItems.Tag = Gallery;
				allItems.Click += new EventHandler(OnItemClick);
				Items.Add(allItems);
			}
			Gallery.RaiseFilterMenuPopup(this);
		}
		void SetGroupsVisibility(bool visible) {
			foreach(GalleryItemGroup group in Gallery.Groups) {
				group.Visible = visible;
			}
		}
		void OnItemClick(object sender, EventArgs e) {
			Gallery.RaiseFilterMenuItemClick(this, ((DXMenuCheckItem)sender));
			Gallery.BeginUpdate();
			try {
				GalleryItemGroup group = ((DXMenuCheckItem)sender).Tag as GalleryItemGroup;
				if(group != null) group.Visible = ((DXMenuCheckItem)sender).Checked;
				if(((DXMenuCheckItem)sender).Tag == Gallery) SetGroupsVisibility(((DXMenuCheckItem)sender).Checked);
				Gallery.Groups.EnsureGroupVisible();
			}
			finally {
				Gallery.EndUpdate();
			}
		}
	}
}
