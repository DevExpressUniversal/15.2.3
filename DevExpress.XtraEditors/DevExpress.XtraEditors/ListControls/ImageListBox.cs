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
using System.Collections;
using System.Windows.Forms;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ListControls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Utils;
using DevExpress.XtraEditors.ToolboxIcons;
using System.Collections.Generic;
namespace DevExpress.XtraEditors.ViewInfo {
	public class ImageListBoxViewInfo : BaseListBoxViewInfo {
		public ImageListBoxViewInfo(BaseImageListBoxControl listBox) : base(listBox) { }
		public int FullImageWidth {
			get {
				if(ImageList == null) return 0;
				return ImageSize.Width + 2 * ImageIndent;
			}
		}
		protected internal override int CalcItemMinHeight() {
			return Math.Max(base.CalcItemMinHeight(), ImageSize.Height + 2);
		}
		protected override int CalcItemHeight(Graphics g, int itemIndex) {
			return Math.Max(base.CalcItemHeight(g, itemIndex), ImageSize.Height + 2);
		}
		public Size ImageSize { get { return ImageCollection.GetImageListSize(ImageList); } }
		public object ImageList { get { return OwnerControl.ImageList; } }
		protected static int ImageIndent { get { return 2; } }
		protected new BaseImageListBoxControl OwnerControl { get { return base.OwnerControl as BaseImageListBoxControl; } }
		protected override BaseListBoxViewInfo.ItemInfo CreateItemInfo(Rectangle bounds, object item, string text, int index) {
			Rectangle textRect = new Rectangle(bounds.Left + FullImageWidth, bounds.Top, bounds.Width - FullImageWidth, bounds.Height);
			if(GlyphAlignment == StringAlignment.Far) textRect.X -= FullImageWidth;
			ImageItemInfo itemInfo = new ImageItemInfo(OwnerControl, bounds, item, text, index);
			itemInfo.TextRect = GlyphAlignment == StringAlignment.Center ? Rectangle.Empty : textRect;
			itemInfo.ImageRect = CalcImageRect(bounds);
			itemInfo.ImageIndex = (index == -1 ? -1 : OwnerControl.GetItemImageIndex(index));
			if(!ImageCollection.IsImageListImageExists(ImageList, itemInfo.ImageIndex)) itemInfo.ImageIndex = -1;
			return itemInfo;
		}
		protected virtual Rectangle CalcImageRect(Rectangle itemBounds) {
			Rectangle rect = new Rectangle(new Point(itemBounds.Left + ImageIndent, itemBounds.Top + (itemBounds.Height - ImageSize.Height) / 2), ImageSize);
			if(GlyphAlignment == StringAlignment.Far) rect.X = itemBounds.Right - FullImageWidth + ImageIndent;
			else if(GlyphAlignment == StringAlignment.Center) rect.X = itemBounds.Left + (itemBounds.Width - FullImageWidth) / 2;
			return rect;
		}
		protected override Size CalcItemSize(int itemIndex) {
			return CalcItemSize(itemIndex, true);
		}
		protected override Size CalcItemSize(int itemIndex, bool cacheItemSize) {
			Size size = base.CalcItemSize(itemIndex, cacheItemSize);
			size.Height = Math.Max(size.Height, ImageSize.Height + 2);
			return size;
		}
		public override int CalcBestColumnWidth() {
			int result = (OwnerControl.GlyphAlignment == HorzAlignment.Center ? 0 : base.CalcBestColumnWidth());
			result += FullImageWidth;
			return result;
		}
		StringAlignment GlyphAlignment {
			get {
				if(OwnerControl.GlyphAlignment == HorzAlignment.Center) return StringAlignment.Center;
				if(OwnerControl.GlyphAlignment == HorzAlignment.Far) return (OwnerControl.IsRightToLeft ? StringAlignment.Near : StringAlignment.Far);
				return (OwnerControl.IsRightToLeft ? StringAlignment.Far : StringAlignment.Near);
			}
		}
		#region ImageItemInfo
		public class ImageItemInfo : ItemInfo {
			Rectangle imageRect;
			int imageIndex;
			public ImageItemInfo(BaseListBoxControl ownerControl, Rectangle rect, object item, string text, int index)
				: base(ownerControl, rect, item, text, index) {
				imageRect = Rectangle.Empty;
				imageIndex = -1;
			}
			public int ImageIndex { get { return imageIndex; } set { imageIndex = value; } }
			public Rectangle ImageRect { get { return imageRect; } set { imageRect = value; } }
		}
		#endregion
	}
}
namespace DevExpress.XtraEditors.Drawing {
	public class PainterImageListBox : BaseListBoxPainter {
		protected override void DrawItemCore(ControlGraphicsInfoArgs info, BaseListBoxViewInfo.ItemInfo itemInfo, ListBoxDrawItemEventArgs e) {
			base.DrawItemCore(info, itemInfo, e);
			ImageListBoxViewInfo.ImageItemInfo imageItemInfo = (ImageListBoxViewInfo.ImageItemInfo)itemInfo;
			if(imageItemInfo.ImageIndex != -1) {
				ImageListBoxViewInfo vi = info.ViewInfo as ImageListBoxViewInfo;
				info.Cache.Paint.DrawImage(info, vi.ImageList, imageItemInfo.ImageIndex, imageItemInfo.ImageRect, (vi.State & DevExpress.Utils.Drawing.ObjectState.Disabled) == 0); 
			}
		}
	}
}
namespace DevExpress.XtraEditors {
	[DevExpress.Utils.Design.SmartTagAction(typeof(BaseImageListBoxControlActions), "EditItems", "Edit items", DevExpress.Utils.Design.SmartTagActionType.CloseAfterExecute), DevExpress.Utils.Design.SmartTagFilter(typeof(BaseImageListBoxControlFilter))]
	public abstract class BaseImageListBoxControl : BaseListBoxControl {
		protected HorzAlignment fGlyphAlignment;
		#region Events
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseImageListBoxControlImageIndexMemberChanged"),
#endif
 DXCategory(CategoryName.PropertyChanged)]
		readonly object imageIndexMemberChanged = new object();
		public event EventHandler ImageIndexMemberChanged {
			add { Events.AddHandler(imageIndexMemberChanged, value); }
			remove { Events.RemoveHandler(imageIndexMemberChanged, value); }
		}
		#endregion
		protected BaseImageListBoxControl() {
			fGlyphAlignment = HorzAlignment.Near;
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseImageListBoxControlGlyphAlignment"),
#endif
 DefaultValue(HorzAlignment.Near), Localizable(true), DevExpress.Utils.Design.SmartTagProperty("Glyph Aligment", "")]
		public HorzAlignment GlyphAlignment {
			get { return fGlyphAlignment; }
			set {
				if(GlyphAlignment == value) return;
				fGlyphAlignment = value;
				LayoutChanged();
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseImageListBoxControlImageList"),
#endif
 DefaultValue(null),
		TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter))]
		public object ImageList {
			get { return ImageListCore; }
			set {
				if(ImageList == value) return;
				ImageListCore = value;
				if(ItemHeight != 0)
					ViewInfo.ItemHeight = Math.Max(ViewInfo.CalcItemMinHeight(), ItemHeight); 
				LayoutChanged();
			}
		}
		[DXCategory(CategoryName.Data), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseImageListBoxControlImageIndexMember"),
#endif
 DefaultValue(""),
		TypeConverter("DevExpress.XtraEditors.Design.DataMemberTypeConverter, " + AssemblyInfo.SRAssemblyEditorsDesign)]
		public virtual string ImageIndexMember {
			get { return ImageIndexMemberCore; }
			set {
				if(value == null) value = string.Empty;
				if(ImageIndexMember != value) {
					ImageIndexMemberCore = value;
					OnDataSourceChanged(this, EventArgs.Empty);
					RaiseImageIndexMemberChanged();
				}
			}
		}
		[Localizable(true), DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseImageListBoxControlItems"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageListBoxItemCollection Items { get { return ItemsCore as ImageListBoxItemCollection; } }
		protected abstract object ImageListCore { get; set; }
		protected abstract string ImageIndexMemberCore { get; set; }
		protected abstract int GetItemImageIndexCore(int index);
		protected internal new ImageListBoxViewInfo ViewInfo { get { return base.ViewInfo as ImageListBoxViewInfo; } }
		public int GetItemImageIndex(int index) {
			if(IsBoundMode) {
				if(ImageIndexMember == string.Empty) return -1;
				int imIndex = -1;
				try { imIndex = Convert.ToInt32(GetDataSourceValue(index, ImageIndexMember)); }
				catch { }
				return imIndex;
			}
			return GetItemImageIndexCore(index);
		}
		protected virtual void RaiseImageIndexMemberChanged() {
			EventHandler handler = (EventHandler)Events[imageIndexMemberChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
	}
	[DXToolboxItem(DXToolboxItemKind.Free), DefaultProperty("Items"),
	 Description("Displays a list of items with small images."),
	 ToolboxTabName(AssemblyInfo.DXTabCommon)
	]
	[DevExpress.Utils.Design.DataAccess.DataAccessMetadata("All", SupportedProcessingModes = "Simple", EnableDirectBinding = false)]
	[ImageListBoxControlCustomBindingPropertiesAttribute]
	[ToolboxBitmap(typeof(ToolboxIconsRootNS), "ImageListBoxControl")]
	public class ImageListBoxControl : BaseImageListBoxControl {
		private object imageList;
		private string imageIndexMember;
		public ImageListBoxControl()
			: base() {
			this.Items.ListChanged += new ListChangedEventHandler(OnCollectionChanged);
			this.Items.ImageListBox = this;
			this.imageList = null;
			this.imageIndexMember = string.Empty;
		}
		protected override ListBoxItemCollection CreateItemsCollection() {
			return new ImageListBoxItemCollection();
		}
		protected virtual void OnCollectionChanged(object sender, ListChangedEventArgs e) {
			if(!IsBoundMode) base.OnListChanged(sender, e);
		}
		protected override BaseStyleControlViewInfo CreateViewInfo() { return new ImageListBoxViewInfo(this); }
		protected override BaseControlPainter CreatePainter() { return new PainterImageListBox(); }
		protected override object ImageListCore { 
			get { return imageList; }
			set {
				if(ImageListCore == value) return;
				if(ImageListCore != null)
					UnsubscribeImageListEvents(ImageListCore);
				imageList = value;
				if(ImageListCore != null)
					SubscribeImageListEvents(ImageListCore);
			} 
		}
		protected virtual void SubscribeImageListEvents(object imageList) {
			ImageCollection imageCollection = imageList as ImageCollection;
			if(imageCollection != null)
				imageCollection.Changed += new EventHandler(OnImageCollectionChanged);
		}
		protected virtual void UnsubscribeImageListEvents(object imageList) {
			ImageCollection imageCollection = imageList as ImageCollection;
			if(imageCollection != null)
				imageCollection.Changed -= new EventHandler(OnImageCollectionChanged);
		}
		void OnImageCollectionChanged(object sender, EventArgs e) {
			LayoutChanged();
		}
		protected override string ImageIndexMemberCore { get { return imageIndexMember; } set { imageIndexMember = value; } }		
		protected override void DoSort() {
			new ListBoxItemSorter(Items, this).DoSort();
		}
		protected override int GetItemImageIndexCore(int index) {
			ImageListBoxItem item = GetItemCore(index) as ImageListBoxItem;
			if(item == null) return -1;
			return item.ImageIndex;
		}
		protected override string GetItemTextCore(int index) {
			if(index < 0 || index > ItemCount - 1) return string.Empty;
			string text = base.GetItemTextCore(index);
			if(IsBoundMode) return text;
			object item = GetItemCore(index);
			return (item != null ? item.ToString() : string.Empty);
		}
		protected override object GetItemValueCore(int index) {
			ImageListBoxItem item = GetItemCore(index) as ImageListBoxItem;
			if(item == null) return null;
			return item.Value;
		}
		protected override void SetItemValueCore(object itemValue, int index) {
			if(itemValue is ImageListBoxItem)
				base.SetItemValueCore(itemValue, index);
			else
				Items[index].Value = itemValue;
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing) 
				ImageListCore = null;
		}
		protected class ImageListBoxControlCustomBindingPropertiesAttribute : ListControlCustomBindingPropertiesAttribute {
			public ImageListBoxControlCustomBindingPropertiesAttribute()
				: base("ImageListBoxControl") {
			}
			public override System.Collections.Generic.IEnumerable<DevExpress.Utils.Design.DataAccess.ICustomBindingProperty> GetCustomBindingProperties() {
				return new DevExpress.Utils.Design.DataAccess.ICustomBindingProperty[] {
						new CustomBindingPropertyAttribute("DisplayMember", "Display Member", GetDisplayMemberDescription()),
						new CustomBindingPropertyAttribute("ValueMember", "Value Member", GetValueMemberDescription()),
						new CustomBindingPropertyAttribute("ImageIndexMember", "Image Index Member", GetImageIndexMemberDescription())
					};
			}
			protected virtual string GetImageIndexMemberDescription() {
				return string.Format("Gets or sets the name of the data field whose values supply image indexes for list box items of the {0}.", ControlName);
			}
		}
	}
}
namespace DevExpress.XtraEditors.Controls {
	public class ImageListBoxItemCollection : ListBoxItemCollection, IEnumerable<ImageListBoxItem> {
		BaseImageListBoxControl imageListBox;
		public ImageListBoxItemCollection() {
			imageListBox = null;
		}
		protected override ListBoxItem CreateItem(object value, string description) {
			return new ImageListBoxItem(value, description);
		}
#if !SL
	[DevExpressXtraEditorsLocalizedDescription("ImageListBoxItemCollectionItem")]
#endif
		public new ImageListBoxItem this[int index] {
			get { return List[index] as ImageListBoxItem; }
			set { List[index] = value; }
		}
		public override int IndexOf(object value) {
			if(value is ImageListBoxItem)
				return base.IndexOf(value);
			return IndexOfValue(value);
		}
		protected internal BaseImageListBoxControl ImageListBox { get { return imageListBox; } set { imageListBox = value; } }
		public void AddRange(ImageListBoxItem[] items) { base.AddRange(items); }
		public override int Add(object value) {
			if(value is ImageListBoxItem) return base.Add(value);
			return base.Add(new ImageListBoxItem(value));
		}
		public int Add(object value, int imageIndex) {
			return Add(new ImageListBoxItem(value, imageIndex));
		}
		protected override void Attach(object item) {
			((ImageListBoxItem)item).ItemChanged += new EventHandler(OnItemChanged);
			((ImageListBoxItem)item).ImageListBox = ImageListBox;
		}
		protected override void Detach(object item) {
			((ImageListBoxItem)item).ItemChanged -= new EventHandler(OnItemChanged);
			((ImageListBoxItem)item).ImageListBox = null;
		}
		protected virtual void OnItemChanged(object sender, EventArgs e) {
			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, this.IndexOf(sender as ListBoxItem), -1));
		}
		#region IEnumerable<ImageListBoxItem> Members
		IEnumerator<ImageListBoxItem> IEnumerable<ImageListBoxItem>.GetEnumerator() {
			foreach(ImageListBoxItem item in InnerList)
				yield return item;
		}
		#endregion
	}
	[TypeConverter("DevExpress.XtraEditors.Design.ImageListBoxItemTypeConverter, " + AssemblyInfo.SRAssemblyEditorsDesign)]
	public class ImageListBoxItem : ListBoxItem {
		int imageIndex;
		BaseImageListBoxControl imageListBox;
		string description;
		public ImageListBoxItem() : this(null) { }
		public ImageListBoxItem(object value) : this(value, -1) { }
		public ImageListBoxItem(object value, string description) : this(value, description, -1) { }
		public ImageListBoxItem(object value, string description, object tag) : this(value, description, -1, tag) { }
		public ImageListBoxItem(object value, int imageIndex) : this(value, string.Empty, imageIndex) { }
		public ImageListBoxItem(object value, string description, int imageIndex) : this(value, description, imageIndex, null) { }
		public ImageListBoxItem(object value, string description, int imageIndex, object tag)
			: base(value, tag) {
			this.description = description;
			this.imageIndex = imageIndex;
			this.imageListBox = null;
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ImageListBoxItemImageIndex"),
#endif
 DefaultValue(-1),
		Editor(typeof(DevExpress.Utils.Design.ImageIndexesEditor), typeof(System.Drawing.Design.UITypeEditor)),
		ImageList("Images")]
		public virtual int ImageIndex {
			get { return imageIndex; }
			set {
				if(ImageIndex == value) return;
				imageIndex = value;
				Changed();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object Images {
			get {
				if(ImageListBox == null) return null;
				return ImageListBox.ImageList;
			}
		}
		[ DefaultValue("")]
		public virtual string Description {
			get { return description; }
			set {
				if(Description != value) {
					description = value;
					Changed();
				}
			}
		}
		public override string ToString() {
			if(!string.IsNullOrEmpty(Description)) return Description;
			return base.ToString();
		}
		protected internal BaseImageListBoxControl ImageListBox { get { return imageListBox; } set { imageListBox = value; } }
	}
}
