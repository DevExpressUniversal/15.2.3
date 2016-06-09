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
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Collections;
using DevExpress.XtraEditors;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.ViewInfo;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Win;
using DevExpress.Utils;
using DevExpress.Utils.ToolTip.ViewInfo;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Skins;
using System.Runtime.Serialization;
using DevExpress.Utils.Text.Internal;
using DevExpress.Utils.Design;
using System.ComponentModel.Design.Serialization;
namespace DevExpress.Utils {
	[TypeConverter(typeof(DevExpress.Utils.Design.UniversalTypeConverterEx))]
	public abstract class BaseToolTipObject {
		BaseToolTipItemViewInfo viewInfo;
		AppearanceObject appearance;
		Rectangle bounds;
		int maxWidth, lockUpdate = 0;
		public BaseToolTipObject() {
			this.viewInfo = null;
			this.appearance = null;
			this.maxWidth = DefaultMaxWidth;
			this.bounds = DefaultBounds;
		}
		public virtual void Dispose() {
		}
		protected internal void Assign(BaseToolTipItem item) {
			BeginUpdate();
			try {
				AssignCore(item);
			}
			finally {
				CancelUpdate();
			}
		}
		protected virtual void BeginUpdate() { this.lockUpdate++; }
		protected virtual void EndUpdate() {
			if(--this.lockUpdate == 0) AdjustSize();
		}
		protected void CancelUpdate() { this.lockUpdate--; }
		protected bool IsLockUpdate { get { return lockUpdate != 0; } }
		protected virtual void AssignCore(BaseToolTipItem item) {
			if(item == null) return;
			MaxWidth = item.MaxWidth;
			Appearance.Assign(item.Appearance);
		}
		protected internal abstract BaseToolTipItemViewInfo CreateViewInfo();
		protected internal virtual ObjectPainter CreatePainter() { return new BaseToolTipItemPainter(); }
		protected internal AppearanceObject CreateAppearanceObject() { return new AppearanceObject(); }
		protected internal virtual BaseToolTipItemViewInfo ViewInfo {
			get {
				if(viewInfo == null) viewInfo = CreateViewInfo();
				return viewInfo;
			}
		}
		protected internal virtual ObjectPainter Painter { get { return CreatePainter(); } }
		bool ShouldSerializeAppearance() { return Appearance.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject Appearance {
			get {
				if(appearance == null) appearance = CreateAppearanceObject();
				return appearance;
			}
		}
		protected void ResetMaxWidth() { MaxWidth = DefaultMaxWidth; }
		protected bool ShouldSerializeMaxWidth() { return MaxWidth != DefaultMaxWidth; }
		public virtual int MaxWidth {
			get { return maxWidth; }
			set {
				value = Math.Max(0, value);
				if(MaxWidth == value) return;
				maxWidth = value;
				AdjustSize();
			}
		}
		protected internal virtual void AdjustSize() { AdjustSize(Size.Empty); }
		protected internal abstract void AdjustSize(Size customSize);
		protected internal Rectangle Bounds { get { return bounds; } }
		protected internal virtual int DefaultMaxWidth { get { return 286; } }
		protected internal virtual Rectangle DefaultBounds { get { return new Rectangle(0, 0, DefaultMaxWidth, 10); } }
		protected internal void SetBounds(Rectangle bounds) { this.bounds = bounds; }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool IsEmpty {
			get {
				return true;
			}
		}
		protected internal virtual ToolTipController Controller { get { return null; } }
	}
	public abstract class BaseToolTipItem : BaseToolTipObject {
		SuperToolTip container;
		public BaseToolTipItem() { }
		protected internal UserLookAndFeel LookAndFeel { 
			get {
				if(container == null) return UserLookAndFeel.Default;
				return Container.LookAndFeel;
			} 
		}
		protected internal override ToolTipController Controller {
			get {
				return Container == null ? null : Container.Controller;
			}
		}
		protected internal virtual bool HasBigImage { get { return false; } }
		protected internal abstract BaseToolTipItem CreateInstance();
		protected internal virtual BaseToolTipItem Clone() {
			BaseToolTipItem res = CreateInstance();
			res.Assign(this);
			return res;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SuperToolTip Container { get { return container; } }
		protected internal void SetContainer(SuperToolTip container) {
			this.container = container;
		}
		protected internal override void AdjustSize(Size customSize) {
			if(Container == null) return;
			SetBounds(new Rectangle(Bounds.Location, ViewInfo.CalcActualContentSize()));
		}
	}
	public class ToolTipTypeConverter : UniversalTypeConverterEx {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if(sourceType.Equals(typeof(byte[])))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if(destinationType.Equals(typeof(byte[])))
				return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
			if(value is byte[]) {
				BinaryTypeConverter conv = new BinaryTypeConverter();
				return conv.ConvertFrom(context, culture, value);
			}
			return base.ConvertFrom(context, culture, value);
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			if(destinationType.Equals(typeof(byte[]))) {
				BinaryTypeConverter conv = new BinaryTypeConverter();
				return conv.ConvertTo(value, destinationType);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	[Serializable, TypeConverter(typeof(ToolTipTypeConverter))]
	public class ToolTipTitleItem : ToolTipItem {
		public ToolTipTitleItem() : base() { }
		public ToolTipTitleItem(SerializationInfo info, StreamingContext context) : base(info, context) { }
		protected internal override BaseToolTipItemViewInfo CreateViewInfo() { return new ToolTipTitleItemViewInfo(this); }
		protected internal override BaseToolTipItem CreateInstance() { return new ToolTipTitleItem(); }
	}
	public enum ToolTipImageAlignment { Default, Left, Right };
	[Serializable, TypeConverter(typeof(ToolTipTypeConverter))]
	public class ToolTipItem : BaseToolTipItem, ISerializable {
		public static int DefaultImageToTextDistance = 14;
		object images;
		int imageIndex;
		Icon icon;
		string text;
		int imageToTextDistance;
		int leftIndent = 0;
		ToolTipImageAlignment imageAlign;
		DefaultBoolean allowHtmlText;
		bool ownerAllowHtmlText;
		public ToolTipItem(SerializationInfo info, StreamingContext context) : this() {
			Deserialize(info, context);
		}
		public ToolTipItem() {
			this.images = null;
			this.imageIndex = -1;
			this.leftIndent = 0;
			this.text = string.Empty;
			this.imageAlign = ToolTipImageAlignment.Default;
			this.imageToTextDistance = DefaultImageToTextDistance;
			this.allowHtmlText = DefaultBoolean.Default;
			this.ownerAllowHtmlText = false;
		}
		protected internal override BaseToolTipItem CreateInstance() { return new ToolTipItem(); }
		protected override void AssignCore(BaseToolTipItem item) {
			base.AssignCore(item);
			ToolTipItem titem = item as ToolTipItem;
			if(item == null) return;
			this.text = titem.Text;
			this.icon = titem.Icon;
			this.images = titem.Images;
			this.imageIndex = titem.ImageIndex;
			this.imageAlign = titem.ImageAlign;
			this.imageToTextDistance = titem.ImageToTextDistance;
			this.leftIndent = titem.LeftIndent;
			this.allowHtmlText = titem.allowHtmlText;
			this.ownerAllowHtmlText = titem.ownerAllowHtmlText;
			MaxWidth = item.MaxWidth;
			Appearance.Assign(item.Appearance);
		}
		protected internal override bool HasBigImage {
			get {
				return ViewInfo.ImageBounds.Height > 16;
			}
		}
		protected internal override ObjectPainter CreatePainter() { return new ToolTipItemPainter(); }
		protected internal override BaseToolTipItemViewInfo CreateViewInfo() { return new ToolTipItemViewInfo(this); }
		protected void ResetImageToTextDistance() { ImageToTextDistance = DefaultImageToTextDistance; }
		protected bool ShouldSerializeImageToTextDistance() { return ImageToTextDistance != DefaultImageToTextDistance; }
		public int ImageToTextDistance {
			get { return imageToTextDistance; }
			set {
				value = Math.Max(0, value);
				imageToTextDistance = value;
				AdjustSize();
			}
		}
		[DefaultValue(0)]
		public int LeftIndent {
			get { return leftIndent; }
			set { leftIndent = Math.Max(value, 0); }
		}
		[DefaultValue(false), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool OwnerAllowHtmlText {
			get { return ownerAllowHtmlText; }
			protected internal set {
				if (OwnerAllowHtmlText == value) return;
				ownerAllowHtmlText = value;
				AdjustSize();
			}
		}
		[DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean AllowHtmlText {
			get { return allowHtmlText; }
			set {
				if (AllowHtmlText == value) return;
				allowHtmlText = value;
				AdjustSize();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int MaxWidth {
			get {
				if(Container != null) return Container.MaxWidth - Container.GetPaddingSize().Width; 
				return base.MaxWidth;
			}
			set { base.MaxWidth = value; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public Font Font {
			get { return Appearance.Font; }
			set { 
				Appearance.Font = value;
				AdjustSize();
			}
		}
		[DefaultValue(null),
		TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter))]
		public object Images {
			get { return images; }
			set { images = value; }
		}
		[DefaultValue(null)]
		public Icon Icon {
			get { return icon; }
			set {
				icon = value;
				AdjustSize();
			}
		}
		[DefaultValue(-1),
	   Editor(typeof(DevExpress.Utils.Design.ImageIndexesEditor), typeof(System.Drawing.Design.UITypeEditor)),
	   ImageList("ImageList"), Localizable(true)]
		public int ImageIndex {
			get { return imageIndex; }
			set {
				if(value < 0) value = -1;
				if(ImageIndex == value) return;
				imageIndex = value;
				AdjustSize();
			}
		}
		[DefaultValue(null), Localizable(true), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(System.Drawing.Design.UITypeEditor))]
		public Image Image {
			get { return Appearance.Image; }
			set {
				if(value == Appearance.Image) return;
				Appearance.Image = value;
				AdjustSize();
			}
		}
		[DefaultValue(""), Localizable(true)]
		public string Text {
			get { return text; }
			set {
				if(value == null) value = string.Empty;
				if(Text == value) return;
				text = value;
				AdjustSize();
			}
		}
		[DefaultValue(ToolTipImageAlignment.Default)]
		public ToolTipImageAlignment ImageAlign {
			get { return imageAlign; }
			set { 
				imageAlign = value;
				AdjustSize();
			}
		}
		public override bool IsEmpty {
			get {
				if(!base.IsEmpty) return false;
				if(Text != string.Empty || ImageCollection.IsImageExists(Image, Images, ImageIndex) || Icon != null) return false;
				return true;
			}
		}
		protected internal new ToolTipItemViewInfo ViewInfo { get { return base.ViewInfo as ToolTipItemViewInfo; } }
		protected internal new ToolTipItemPainter Painter { get { return base.Painter as ToolTipItemPainter; } }
		#region ISerializable Members
		[System.Security.SecurityCritical]
		protected virtual void Serialize(SerializationInfo info, StreamingContext context) {
			info.AddValue("AllowHtmlText", this.AllowHtmlText);
			info.AddValue("Icon", this.Icon, typeof(Icon));
			info.AddValue("Image", this.Image, typeof(Image));
			info.AddValue("ImageAlign", this.ImageAlign);
			info.AddValue("ImageIndex", this.ImageIndex);
			info.AddValue("ImageToTextDistance", this.ImageToTextDistance);
			info.AddValue("LeftIndent", this.LeftIndent);
			info.AddValue("MaxWidth", this.MaxWidth);
			info.AddValue("OwnerAllowHtmlText", this.OwnerAllowHtmlText);
			info.AddValue("Text", this.Text);
		}
		protected virtual void Deserialize(SerializationInfo info, StreamingContext context) {
			SerializationInfoEnumerator enumerator = info.GetEnumerator();
			while(enumerator.MoveNext()) {
				SerializationEntry current = enumerator.Current;
				switch(current.Name) {
					case "AllowHtmlText":
						AllowHtmlText = (DefaultBoolean)info.GetValue(current.Name, typeof(DefaultBoolean));
						break;
					case "Icon":
						Icon = (Icon)info.GetValue(current.Name, typeof(Icon));
						break;
					case "Image":
						Image = (Image)info.GetValue(current.Name, typeof(Image));
						break;
					case "ImageAlign":
						ImageAlign = (ToolTipImageAlignment)info.GetValue(current.Name, typeof(ToolTipImageAlignment));
						break;
					case "ImageIndex":
						ImageIndex = info.GetInt32(current.Name);
						break;
					case "ImageToTextDistance":
						ImageToTextDistance = info.GetInt32(current.Name);
						break;
					case "LeftIndent":
						LeftIndent = info.GetInt32(current.Name);
						break;
					case "MaxWidth":
						MaxWidth = info.GetInt32(current.Name);
						break;
					case "OwnerAllowHtmlText":
						OwnerAllowHtmlText = info.GetBoolean(current.Name);
						break;
					case "Text":
						Text = info.GetString(current.Name);
						break;
				}
			}
		}
		[System.Security.SecurityCritical]
		void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context) {
			Serialize(si, context);
		}
		#endregion
	}
	[Serializable, TypeConverter(typeof(ToolTipTypeConverter))]
	public class ToolTipSeparatorItem : BaseToolTipItem, ISerializable {
		public ToolTipSeparatorItem() {
		}
		public ToolTipSeparatorItem(SerializationInfo info, StreamingContext context) : this() {
		}
		protected internal override BaseToolTipItemViewInfo CreateViewInfo() { return new ToolTipSeparatorItemViewInfo(this) as BaseToolTipItemViewInfo; }
		protected internal override ObjectPainter CreatePainter() {
			if(LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) return new SkinToolTipSeparatorItemPainter(LookAndFeel);
			return new ToolTipSeparatorItemPainter();
		}
		protected internal new ToolTipSeparatorItemViewInfo ViewInfo { get { return base.ViewInfo as ToolTipSeparatorItemViewInfo; } }
		protected internal new ToolTipSeparatorItemPainter Painter { get { return base.Painter as ToolTipSeparatorItemPainter; } }
		protected internal override BaseToolTipItem CreateInstance() { return new ToolTipSeparatorItem(); }
		#region ISerializable Members
		[System.Security.SecurityCritical]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) {
		}
		#endregion
	}
	public class ToolTipItemCollection : CollectionBase {
		SuperToolTip container;
		public ToolTipItemCollection(SuperToolTip container) {
			this.container = container;
		}
		protected SuperToolTip Container { get { return container; } }
		public BaseToolTipItem this[int index] { get { return List[index] as BaseToolTipItem; } }
		public virtual ToolTipItem Add(string text) {
			ToolTipItem item = new ToolTipItem();
			item.Text = text;
			Add(item);
			return item;
		}
		public virtual ToolTipSeparatorItem AddSeparator() {
			ToolTipSeparatorItem item = new ToolTipSeparatorItem();
			Add(item);
			return item;
		}
		public virtual ToolTipTitleItem AddTitle(string text) {
			ToolTipTitleItem title = new ToolTipTitleItem();
			title.Text = text;
			Add(title);
			return title;
		}
		public int Add(BaseToolTipItem item) {
			if(List.Contains(item)) return List.IndexOf(item);
			return List.Add(item);
		}
		public bool Contains(BaseToolTipItem item) { return List.Contains(item); }
		protected override void OnInsertComplete(int index, object value) {
			BaseToolTipItem item = (BaseToolTipItem)value;
			item.SetContainer(Container);
			base.OnInsertComplete(index, value);
		}
	}
	internal enum SuperToolTipItemType { Item, Title, Separator }
	[Serializable, TypeConverter(typeof(ToolTipTypeConverter))]
	public class SuperToolTip : BaseToolTipObject, ICloneable, ISerializable {
		public const int DefaultDistanceBetweenItems = 6;
		ToolTipItemCollection items;
		UserLookAndFeel lookAndFeel;
		Padding padding;
		int distanceBetweenItems;
		DefaultBoolean allowHtmlText;
		bool ownerAllowHtmlText;
		public SuperToolTip() {
			this.items = new ToolTipItemCollection(this);
			this.padding = DefaultPadding;
			this.distanceBetweenItems = DefaultDistanceBetweenItems;
			this.allowHtmlText = DefaultBoolean.Default;
			this.ownerAllowHtmlText = false;
		}
		ToolTipController controller;
		protected internal void SetController(ToolTipController controller) { this.controller = controller; }
		protected internal override ToolTipController Controller {
			get { return controller; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool RightToLeft { get; set; }
		public SuperToolTip(SerializationInfo info, StreamingContext context) : this() {
			Deserialize(info, context);
		}
		[System.Security.SecurityCritical]
		private void Serialize(SerializationInfo info, StreamingContext context) {
			info.AddValue("AllowHtmlText", this.AllowHtmlText, typeof(DefaultBoolean));
			info.AddValue("FixedToolTipWidth", this.FixedTooltipWidth);
			info.AddValue("ItemsCount", this.Items.Count);
			for(int i = 0; i < this.Items.Count; i++) {
				info.AddValue("itemType" + i, GetItemType(Items[i]), typeof(SuperToolTipItemType));
				info.AddValue("item" + i, this.Items[i], this.Items[i].GetType());
			}
		}
		SuperToolTipItemType GetItemType(BaseToolTipItem item) {
			if(item is ToolTipSeparatorItem) return SuperToolTipItemType.Separator;
			if(item is ToolTipTitleItem) return SuperToolTipItemType.Title;
			return SuperToolTipItemType.Item;
		}
		protected virtual void Deserialize(SerializationInfo info, StreamingContext context) {
			SerializationInfoEnumerator enumerator = info.GetEnumerator();
			int itemsCount = 0;
			while (enumerator.MoveNext()) {
				SerializationEntry current = enumerator.Current;
				switch(current.Name) { 
					case "AllowHtmlText":
						AllowHtmlText = (DefaultBoolean)info.GetValue(current.Name, typeof(DefaultBoolean));
						break;
					case "FixedToolTipWidth":
						FixedTooltipWidth = info.GetBoolean(current.Name);
						break;
					case "ItemsCount":
						itemsCount = info.GetInt32(current.Name);
						break;
				}
			}
			for(int i = 0; i < itemsCount; i++) {
				SuperToolTipItemType itemType = (SuperToolTipItemType)info.GetValue("itemType" + i, typeof(SuperToolTipItemType));
				Items.Add((BaseToolTipItem)info.GetValue("item" + i, GetItemType(itemType)));
			}
		}
		Type GetItemType(SuperToolTipItemType itemType) {
			if(itemType == SuperToolTipItemType.Item)
				return typeof(ToolTipItem);
			if(itemType == SuperToolTipItemType.Separator)
				return typeof(ToolTipSeparatorItem);
			return typeof(ToolTipTitleItem);
		}
		bool ShouldSerializePadding() { return Padding.All != 0; }
#if !SL
	[DevExpressUtilsLocalizedDescription("SuperToolTipPadding")]
#endif
		public Padding Padding { get { return padding; } set { padding = value; } }
		protected internal Padding DefaultPadding { get { return new Padding(0, 0, 0, 0); } }
		public override string ToString() {
			if(IsEmpty) return string.Empty;
			SuperToolTipSetupArgs setup = new SuperToolTipSetupArgs(this);
			string res = string.Empty;
			if(setup.Title.Text != "") res += setup.Title.Text;
			if(setup.Contents.Text != "") res += (res == "" ? "" : "/") + setup.Contents.Text;
			if(setup.Footer.Text != "") res += (res == "" ? "" : "/") + setup.Footer.Text;
			return res;
		}
		public virtual object Clone() {
			SuperToolTip res = new SuperToolTip();
			res.Assign(this);
			return res;
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("SuperToolTipOwnerAllowHtmlText"),
#endif
 DefaultValue(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool OwnerAllowHtmlText {
			get { return ownerAllowHtmlText; }
			protected internal set {
				if (OwnerAllowHtmlText == value) return;
				ownerAllowHtmlText = value;
				OnOwnerAllowHtmlTextChanged();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceObject Appearance {
			get {
				return base.Appearance;
			}
		}
		protected virtual void OnOwnerAllowHtmlTextChanged() {
			foreach (BaseToolTipItem item in Items) {
				ToolTipItem titem = item as ToolTipItem;
				if (titem == null) continue;
				titem.OwnerAllowHtmlText = AllowHtmlText == DefaultBoolean.Default? OwnerAllowHtmlText: AllowHtmlText == DefaultBoolean.True;
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("SuperToolTipAllowHtmlText"),
#endif
 DefaultValue(DefaultBoolean.Default)]
		public virtual DefaultBoolean AllowHtmlText {
			get { return allowHtmlText; }
			set {
				if(AllowHtmlText == value) return;
				allowHtmlText = value;
				OnOwnerAllowHtmlTextChanged();
			}
		}
		bool fixedTooltipWidth = false;
		[
#if !SL
	DevExpressUtilsLocalizedDescription("SuperToolTipFixedTooltipWidth"),
#endif
 DefaultValue(false)]
		public virtual bool FixedTooltipWidth {
			get { return fixedTooltipWidth; }
			set { fixedTooltipWidth = value; }
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("SuperToolTipMaxWidth")]
#endif
		public override int MaxWidth {
			get {
				if(FixedTooltipWidth) {
					foreach(BaseToolTipItem item in Items) {
						if(item.HasBigImage) return 318;
					}
					return 210;
				}
				return base.MaxWidth;
			}
			set {
				base.MaxWidth = Math.Max(10, value);
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("SuperToolTipDistanceBetweenItems"),
#endif
		DefaultValue(DefaultDistanceBetweenItems)]
		public int DistanceBetweenItems {
			get { return distanceBetweenItems; }
			set {
				distanceBetweenItems = Math.Max(0, value);
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(false)]
		public ToolTipItemCollection Items { get { return items; } }
		public void Assign(SuperToolTip source) {
			if(source == null) return;
			Items.Clear();
			this.OwnerAllowHtmlText = source.OwnerAllowHtmlText;
			foreach(BaseToolTipItem item in source.Items) {
				Items.Add(item.Clone());
			}
			this.Appearance.Assign(source.Appearance);
			this.DistanceBetweenItems = source.DistanceBetweenItems;
			this.FixedTooltipWidth = source.FixedTooltipWidth;
			this.AllowHtmlText = source.AllowHtmlText;
			this.Padding = source.Padding;
			this.MaxWidth = source.MaxWidth;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public UserLookAndFeel LookAndFeel { 
			get {
				if(lookAndFeel != null) return lookAndFeel;
				return UserLookAndFeel.Default; 
			}
			set {
				lookAndFeel = value;
			}
		}
		protected internal Rectangle ContentBounds { get { return ViewInfo.ContentBounds; } }
#if !SL
	[DevExpressUtilsLocalizedDescription("SuperToolTipIsEmpty")]
#endif
		public override bool IsEmpty {
			get {
				if(base.IsEmpty && Items.Count == 0) return true;
				foreach(BaseToolTipItem item in Items) {
					ToolTipItem titem = item as ToolTipItem;
					if(titem != null && !titem.IsEmpty) return false;
				}
				return true;
			}
		}
		protected internal override BaseToolTipItemViewInfo CreateViewInfo() { return new ToolTipContainerViewInfo(this) as BaseToolTipItemViewInfo; }
		protected internal override ObjectPainter CreatePainter() { return new ToolTipContainerPainter(); }
		protected internal new ToolTipContainerViewInfo ViewInfo { get { return base.ViewInfo as ToolTipContainerViewInfo; } }
		protected internal override int DefaultMaxWidth { get { return 316; } }
		protected internal override void AdjustSize(Size customSize) {
			SetBounds(new Rectangle(Bounds.Location, customSize != Size.Empty? customSize:ViewInfo.CalcSize()));
			ViewInfo.CalcViewInfo();
		}
		protected internal Size GetPaddingSize() {
#if DXWhidbey
			return Padding.Size;
#else
			return Size.Empty;
#endif
		}
		protected internal virtual void OnPaint(object sender, PaintEventArgs e) {
			GraphicsCache cache = new GraphicsCache(e.Graphics);
			try {
				Draw(cache);
			}
			finally { cache.Dispose(); }
		}
		protected internal virtual void Draw(GraphicsCache cache) {
			ToolTipContainerInfoArgs infoArgs = new ToolTipContainerInfoArgs(cache, ViewInfo);
			Painter.DrawObject(infoArgs);
		}
		public virtual void Setup(SuperToolTipSetupArgs info) {
			int leftIndent = 0;
			Items.Clear();
			OwnerAllowHtmlText = info.OwnerAllowHtmlText;
			AllowHtmlText = info.AllowHtmlText;
			if(!info.Title.IsEmpty) {
				Items.Add(info.Title);
				leftIndent = 6;
			}
			if(!info.Contents.IsEmpty || !info.Footer.IsEmpty) {
				Items.Add(info.Contents);
				if((Items[Items.Count - 1] as ToolTipItem).LeftIndent == 0)
					(Items[Items.Count-1] as ToolTipItem).LeftIndent = leftIndent;
			}
			if(!info.Footer.IsEmpty) {
				if(info.ShowFooterSeparator) Items.AddSeparator();
				Items.Add(info.Footer);
				if((Items[Items.Count - 1] as ToolTipItem).LeftIndent == 0)
					(Items[Items.Count - 1] as ToolTipItem).LeftIndent = leftIndent;
			}
		}
		#region ISerializable Members
		[System.Security.SecurityCritical]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) {
			Serialize(info, context);
		}
		#endregion
	}
	[ToolboxItem(false)]
	public class SuperToolTipPreviewControl : Control {
		SuperToolTip container;
		public SuperToolTipPreviewControl() : this(null) { }
		public SuperToolTipPreviewControl(SuperToolTip container) {
			this.container = container;
			SetStyle(ControlStyles.Opaque | ControlConstants.DoubleBuffer, true);
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public SuperToolTip ToolContainer { get { return container; } set { container = value; } }
		protected override void OnPaint(PaintEventArgs e) {
			if(this.container == null) {
				base.OnPaint(e);
				return;
			}
			e.Graphics.FillRectangle(Brushes.White, ClientRectangle);
			this.container.AdjustSize();
			this.container.OnPaint(this, e);
		}
	}
	[ToolboxItem(false)]
	public class SuperToolTipWindow : ToolTipControllerBaseWindow {
		SuperToolTip container;
		public SuperToolTipWindow(SuperToolTip container, bool dropShadow) : base(dropShadow) {
			this.container = container;
		}
		protected override StringBlock GetLinkByMouse(Point pt) {
			foreach(BaseToolTipItem item in TipContainer.Items) {
				ToolTipItemViewInfo vi = item.ViewInfo as ToolTipItemViewInfo;
				if(vi == null)
					continue;
				StringBlock block = vi.TextInfo.GetLinkByPoint(pt);
				if(block != null)
					return block;
			}
			return null;
		}
		protected override bool AllowHtmlText {
			get {
				foreach(BaseToolTipItem item in TipContainer.Items) {
					ToolTipItemViewInfo vi = item.ViewInfo as ToolTipItemViewInfo;
					if(vi != null && vi.GetAllowHtmlText())
						return true;
				}
				return false;
			}
		}
		protected internal override bool HasHyperlink {
			get {
				foreach(BaseToolTipItem item in TipContainer.Items) {
					ToolTipItemViewInfo vi = item.ViewInfo as ToolTipItemViewInfo;
					if(vi != null && vi.TextInfo.HasHyperlink)
						return true;
				}
				return false;
			}
		}
		protected SuperToolTip TipContainer { get { return container; } }
		protected override void OnPaint(PaintEventArgs e) {
			TipContainer.OnPaint(this, e);
			RaisePaintEvent(this, e);
		}
		public override void DrawBackground(GraphicsCache cache, AppearanceObject apperance) {
			TipContainer.Draw(cache);
		}
		protected internal override void UpdateRegion() {
			if(TipContainer.LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) {
				SkinElement se = CommonSkins.GetSkin(TipContainer.LookAndFeel)[CommonSkins.SkinToolTipWindow];
				if(se != null) {
					int cornerRadius = se.Properties.GetInteger(CommonSkins.SkinToolTipWindowCornerRadius);
					if(cornerRadius != 0) {
						Region = NativeMethods.CreateRoundRegion(new Rectangle(Point.Empty, Size), cornerRadius);
						return;
					}
				}
			}
			Region = null;
		}
	}
	public class SuperToolTipSetupArgs {
		ToolTipTitleItem title;
		ToolTipItem contents;
		bool showFooterSeparator;
		ToolTipTitleItem footer;
		DefaultBoolean allowHtmlText;
		bool ownerAllowHtmlText;
		public SuperToolTipSetupArgs() {
			this.title = new ToolTipTitleItem();
			this.contents = new ToolTipItem();
			this.showFooterSeparator = false;
			this.footer = new ToolTipTitleItem();
			this.allowHtmlText = DefaultBoolean.Default;
			this.ownerAllowHtmlText = false;
		}
		public bool OwnerAllowHtmlText { 
			get { return ownerAllowHtmlText; } 
			protected internal set {
				if (OwnerAllowHtmlText == value) return;
				ownerAllowHtmlText = value;
				OnOwnerAllowHtmlTextChanged();
			} 
		}
		protected virtual void OnOwnerAllowHtmlTextChanged() {
			if(Title != null) Title.OwnerAllowHtmlText = GetOwnerAllowHtmlText();
			if(Contents != null) Contents.OwnerAllowHtmlText = GetOwnerAllowHtmlText();
			if(Footer != null) Footer.OwnerAllowHtmlText = GetOwnerAllowHtmlText();
		}
		protected internal bool GetOwnerAllowHtmlText() {
			if(AllowHtmlText != DefaultBoolean.Default) return AllowHtmlText == DefaultBoolean.True;
			return OwnerAllowHtmlText;
		}
		public DefaultBoolean AllowHtmlText { 
			get { return allowHtmlText; } 
			set {
				if(AllowHtmlText == value) return;
				allowHtmlText = value;
				OnOwnerAllowHtmlTextChanged();
			} 
		}
		public ToolTipTitleItem Title { get { return title; } }
		public ToolTipItem Contents { get { return contents; } }
		public bool ShowFooterSeparator { get { return showFooterSeparator; } set { showFooterSeparator = value; }  }
		public ToolTipTitleItem Footer { get { return footer; } }
		public SuperToolTipSetupArgs(SuperToolTip container) : this() {
			if(container.Items.Count == 0) return;
			bool showSeparator = false;
			int index = 0;
			this.allowHtmlText = container.AllowHtmlText;
			if(container.Items[index] is ToolTipTitleItem) {
				Title.Assign(container.Items[index++]);
			}
			if(index >= container.Items.Count) return;
			if(container.Items[index] is ToolTipItem) {
				Contents.Assign(container.Items[index++]);
			}
			if(index >= container.Items.Count) return;
			if(container.Items[index] is ToolTipSeparatorItem) {
				showSeparator = true;
				index++;
			}
			if(index >= container.Items.Count) return;
			if(container.Items[index] is ToolTipItem) {
				Footer.Assign(container.Items[index++]);
				ShowFooterSeparator = showSeparator;
			}
		}
	}
}
