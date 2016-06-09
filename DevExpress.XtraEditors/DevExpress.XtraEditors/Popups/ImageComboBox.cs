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
using System.Linq;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.NativeBricks;
using DevExpress.Utils.Text;
using DevExpress.XtraEditors.ToolboxIcons;
namespace DevExpress.XtraEditors.Repository {
	[Obsolete(ObsoleteText.SRObsoletePickImageRepository)]
	public class RepositoryItemPickImage : RepositoryItemImageComboBox {
	}
	public class RepositoryItemImageComboBoxCore : RepositoryItemComboBox {
		protected new ComboBoxItemCollection Items { get { return base.Items; } }
	}
	public class RepositoryItemImageComboBox : RepositoryItemImageComboBoxCore {
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(ObsoleteText.SRObsoletePropertiesText)]
		public new RepositoryItemImageComboBox Properties { get { return this; } }
		[Browsable(false)]
		public override string EditorTypeName { get { return "ImageComboBoxEdit"; } }
		HorzAlignment fGlyphAlignment;
		object smallImages, largeImages;
		public RepositoryItemImageComboBox() {
			this.fGlyphAlignment = HorzAlignment.Near;
			this.smallImages = this.largeImages = null;
			this.fTextEditStyle = TextEditStyles.DisableTextEditor;
		}
		protected Image GetBrickImage(ImageComboBoxEditViewInfo icVi, PrintCellHelperInfo info, int imageIndex) {
			MultiKey key = new MultiKey(new object[] { this.ButtonsStyle, GlyphAlignment, LargeImages, SmallImages, imageIndex, icVi.Bounds.Size, info.EditValue, this.AutoHeight, this.BorderStyle, this.Enabled, this.EditorTypeName });
			Image img = GetCachedPrintImage(key, info.PS);
			if(img != null) return img;
			using(BitmapGraphicsHelper gHelper = new BitmapGraphicsHelper(icVi.ImageSize.Width, icVi.ImageSize.Height)) {
				DevExpress.Utils.ImageCollection.DrawImageListImage(new GraphicsCache(gHelper.Graphics), icVi.Images, imageIndex, new Rectangle(0, 0, icVi.ImageSize.Width, icVi.ImageSize.Height), icVi.State != ObjectState.Disabled);
				return AddImageToPrintCache(key, gHelper.MemSafeBitmap, info.PS);
			}
		}
		protected override ExportMode GetExportMode() {
			if(ExportMode == ExportMode.Default) return ExportMode.DisplayText;
			return ExportMode;
		}
		protected override BrickStyle CreateBrickStyle(PrintCellHelperInfo info, string type) {
			BrickStyle style = base.CreateBrickStyle(info, type);
			if(type != "panel") {
				style.Sides = DevExpress.XtraPrinting.BorderSide.None;
			}
			else {
				SetupTextBrickStyleProperties(info, style);
			}
			if(type == "image") style.Padding = new PaddingInfo(0, 0, 0, 0);
			return style;
		}
		public override IVisualBrick GetBrick(PrintCellHelperInfo info) {
			const int ImageIndent = 5;
			ImageComboBoxEditViewInfo icVi = PreparePrintViewInfo(info, true) as ImageComboBoxEditViewInfo;
			IPanelBrick panelBrick = new XETextPanelBrick(CreateBrickStyle(info, "panel"));
			SetCommonBrickProperties(panelBrick, info);
			RectangleF textRect, imageRect;
			Rectangle content = icVi.ContentRect;
			textRect = imageRect = content;
			imageRect.Size = icVi.ImageSize;
			imageRect.Y += (content.Height > imageRect.Height) ? (content.Height - imageRect.Height) / 2 : 0;
			switch(icVi.GlyphAlignment) {
				case HorzAlignment.Near:
				case HorzAlignment.Default:
					textRect.X = imageRect.Right + ImageIndent;
					textRect.Width = content.Right - textRect.X;
					break;
				case HorzAlignment.Center:
					textRect = Rectangle.Empty;
					imageRect.X += (content.Width > imageRect.Width) ? (content.Width - imageRect.Width) / 2 : 0;
					break;
				case HorzAlignment.Far:
					textRect.Width = textRect.Width - (imageRect.Width + ImageIndent);
					if(textRect.Width > 0) {
						imageRect.X = textRect.Right + ImageIndent;
					}
					else {
						imageRect = Rectangle.Empty;
					}
					break;
			}
			int imageIndex = Items.GetItem(info.EditValue) != null ? Items.GetItem(info.EditValue).ImageIndex : -1;
			if(imageIndex >= 0 && icVi.Images != null && !imageRect.IsEmpty) {
				IImageBrick imageBrick = CreateImageBrick(info, CreateBrickStyle(info, "image"));
				imageBrick.Image = GetBrickImage(icVi, info, imageIndex);
				imageBrick.Rect = imageRect;
				panelBrick.Bricks.Add(imageBrick);
			}
			else {
				imageRect = Rectangle.Empty;
			}
			if(icVi.GlyphAlignment != HorzAlignment.Center && textRect != Rectangle.Empty && TextEditStyle != TextEditStyles.HideTextEditor) {
				ITextBrick textBrick = CreateTextBrick(info);
				if(!imageRect.IsEmpty)
					textBrick.Rect = textRect;
				else
					textBrick.Rect = content;
				panelBrick.Bricks.Add(textBrick);
			}
			return panelBrick;
		}
		public override void Assign(RepositoryItem item) {
			RepositoryItemImageComboBox source = item as RepositoryItemImageComboBox;
			BeginUpdate();
			try {
				base.Assign(item);
				if(source == null) return;
				this.fGlyphAlignment = source.GlyphAlignment;
				this.largeImages = source.LargeImages;
				this.smallImages = source.SmallImages;
				this.fTextEditStyle = source.TextEditStyle;
			}
			finally {
				EndUpdate();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemImageComboBoxGlyphAlignment"),
#endif
 DefaultValue(HorzAlignment.Near), Localizable(true)]
		public virtual HorzAlignment GlyphAlignment {
			get { return fGlyphAlignment; }
			set {
				if(GlyphAlignment == value) return;
				fGlyphAlignment = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Appearance),
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemImageComboBoxSmallImages"),
#endif
		TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter)),
		DefaultValue(null), SmartTagProperty("Small Images", "", SmartTagActionType.RefreshBoundsAfterExecute)
		]
		public virtual object SmallImages {
			get { return smallImages; }
			set {
				if(SmallImages == value) return;
				smallImages = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemImageComboBoxLargeImages"),
#endif
		TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter)),
		DefaultValue(null), SmartTagProperty("Large Images", "", SmartTagActionType.RefreshBoundsAfterExecute)]
		public object LargeImages {
			get { return largeImages; }
			set {
				if(LargeImages == value) return;
				largeImages = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemImageComboBoxTextEditStyle"),
#endif
 DefaultValue(TextEditStyles.DisableTextEditor)]
		public override TextEditStyles TextEditStyle {
			get { return base.TextEditStyle; }
			set {
				if(value == TextEditStyles.Standard) value = TextEditStyles.DisableTextEditor;
				base.TextEditStyle = value;
			}
		}
		[Browsable(false)]
		public override HorzAlignment DefaultAlignment { get { return HorzAlignment.Near; } }
		[Localizable(true), DXCategory(CategoryName.Data), RefreshProperties(RefreshProperties.All), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemImageComboBoxItems"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Editor("System.ComponentModel.Design.CollectionEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor))]
		public virtual new ImageComboBoxItemCollection Items { get { return base.Items as ImageComboBoxItemCollection; } }
		protected override ComboBoxItemCollection CreateItemCollection() {
			return new ImageComboBoxItemCollection(this);
		}
		public override string GetDisplayText(FormatInfo format, object editValue) {
			ImageComboBoxItem pi = Items.GetItem(editValue);
			if(pi == null) return base.GetDisplayText(format, editValue);
			return GetNormalizedText(format.GetDisplayText(pi.Description));
		}
		public override BaseEditorGroupRowPainter CreateGroupPainter() {
			return new ImageComboBoxEditorGroupRowPainter();
		}
		[Browsable(false)]
		public override bool RequireDisplayTextSorting { get { return true; } }
		public override bool IsFilterLookUp { get { return true; } }
	}
}
namespace DevExpress.XtraEditors {
	[Obsolete(ObsoleteText.SRObsoletePickImage), ToolboxItem(false), DefaultBindingPropertyEx("EditValue")]
	public class PickImage : ImageComboBoxEdit {
	}
	[Designer("DevExpress.XtraEditors.Design.ImageComboBoxEditDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign),
	 Description("Displays a drop-down list of items that can represent values of any type. Supports images for list items."),
	 ToolboxTabName(AssemblyInfo.DXTabCommon), SmartTagAction(typeof(ImageComboBoxEditActions), "EditItems", "Edit items", SmartTagActionType.CloseAfterExecute), SmartTagFilter(typeof(ImageComboBoxEditFilter)),
	ToolboxBitmap(typeof(ToolboxIconsRootNS), "ImageComboBoxEdit")
	]
	public class ImageComboBoxEdit : ComboBoxEdit {
		[Browsable(false)]
		public override string EditorTypeName { get { return "ImageComboBoxEdit"; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ImageComboBoxEditProperties"),
#endif
 DXCategory(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SmartTagSearchNestedProperties]
		public new RepositoryItemImageComboBox Properties { get { return base.Properties as RepositoryItemImageComboBox; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), Obsolete(ObsoleteText.SRObsoletePickValue)]
		public virtual object Value { get { return EditValue; } set { EditValue = value; } }
		protected override PopupBaseForm CreatePopupForm() {
			return new PopupImageComboBoxEditListBoxForm(this);
		}
		[Bindable(false), Browsable(false)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override object SelectedItem {
			get { return Properties.Items.GetItem(EditValue); }
			set {
				if(value == null) {
					EditValue = null;
					return;
				}
				if(Properties.Items.Contains(value)) {
					ImageComboBoxItem pi = value as ImageComboBoxItem;
					if(pi != null)
						EditValue = pi.Value;
				}
			}
		}
		[Browsable(false)]
		public override object EditValue {
			get { return base.EditValue; }
			set {
				if(IsLoading) {
					base.EditValue = value;
					return;
				}
				ImageComboBoxItem pi = value as ImageComboBoxItem;
				if(pi != null) {
					base.EditValue = pi.Value;
				}
				else {
					base.EditValue = value;
				}
			}
		}
		protected internal virtual void ItemsClearComplete(int prevIndex) {
		}
		public void SelectItemByDescription(string val) {
			ImageComboBoxItem ret = Properties.Items.OfType<ImageComboBoxItem>().FirstOrDefault(item => item.Description == val);
			if(ret != null)
				EditValue = ret.Value;
		}
	}
}
namespace DevExpress.XtraEditors.ViewInfo {
	public class ImageComboBoxEditViewInfo : ComboBoxViewInfo {
		int imageIndex;
		public ImageComboBoxEditViewInfo(RepositoryItem item)
			: base(item) {
		}
		protected override void Assign(BaseControlViewInfo info) {
			base.Assign(info);
			ImageComboBoxEditViewInfo be = info as ImageComboBoxEditViewInfo;
			if(be == null) return;
			this.imageIndex = be.imageIndex;
		}
		public override void Reset() {
			this.imageIndex = -1;
			base.Reset();
		}
		public override int ImageIndex { get { return imageIndex; } }
		public override object Images { get { return Item.SmallImages == null ? Item.LargeImages : Item.SmallImages; } }
		public new RepositoryItemImageComboBox Item { get { return base.Item as RepositoryItemImageComboBox; } }
		public override TextGlyphDrawModeEnum GlyphDrawMode {
			get {
				if(Images == null) return TextGlyphDrawModeEnum.Text;
				if(GlyphAlignment == HorzAlignment.Center) return TextGlyphDrawModeEnum.Glyph;
				return TextGlyphDrawModeEnum.TextGlyph;
			}
		}
		public override void UpdateEditValue() {
			if(OwnerEdit != null && OwnerEdit.IsPopupOpen) {
				ImageComboBoxItem pi = OwnerEdit.PopupForm.ResultValue as ImageComboBoxItem;
				if(pi != null) EditValue = pi.Value;
				else EditValue = OwnerEdit.EditValue;
				return;
			}
			base.UpdateEditValue();
		}
		protected internal override bool IsShowNullValuePrompt() {
			if(!base.IsShowNullValuePrompt()) return false;
			return Item.Items.GetItem(EditValue) == null;
		}
		protected override void OnEditValueChanged() {
			ImageComboBoxItem item = Item.Items.GetItem(EditValue);
			if(item != null) {
				this.imageIndex = item.ImageIndex;
				this.fDisplayText = item.Description;
			}
			else {
				this.imageIndex = -1;
				if(IsShowNullValuePrompt())
					this.fDisplayText = Item.NullValuePrompt;
				else
					this.fDisplayText = Item.NullText;
			}
		}
		public override HorzAlignment GlyphAlignment {
			get {
				HorzAlignment align = Item.GlyphAlignment;
				if(align == HorzAlignment.Default) align = HorzAlignment.Near;
				return align;
			}
		}
	}
}
namespace DevExpress.XtraEditors.Drawing {
	public class ImageComboBoxEditPainter : ButtonEditPainter {
	}
	public class ImageComboBoxEditorGroupRowPainter : BaseEditorGroupRowPainter {
		protected virtual ImageComboBoxItem GetItem(EditorGroupRowArgs e) {
			RepositoryItemImageComboBox pcombo = e.Properties as RepositoryItemImageComboBox;
			return pcombo.Items.GetItem(e.EditValue);
		}
		public virtual object GetImages(ObjectInfoArgs e) {
			RepositoryItemImageComboBox pcombo = ((EditorGroupRowArgs)e).Properties as RepositoryItemImageComboBox;
			if(pcombo == null) return null;
			if(pcombo.SmallImages == null) return pcombo.LargeImages;
			return pcombo.SmallImages;
		}
		protected Size GetImageSize(EditorGroupRowArgs e, ImageComboBoxItem item) {
			RepositoryItemImageComboBox pcombo = e.Properties as RepositoryItemImageComboBox;
			if(item == null) return Size.Empty;
			return ImageCollection.IsImageListImageExists(GetImages(e), item.ImageIndex) ? ImageCollection.GetImageListSize(GetImages(e)) : Size.Empty;
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			Rectangle res = base.CalcObjectMinBounds(e);
			string start, end;
			ParseText((EditorGroupRowArgs)e, out start, out end);
			if(end.Length == 0) return res;
			int height = ImageCollection.GetImageListSize(GetImages(e)).Height;
			if(height != 0) height += 2;
			res.Height = Math.Max(res.Height, height);
			return res;
		}
		const int ImageIndent = 4;
		public override void DrawObject(ObjectInfoArgs e) {
			EditorGroupRowArgs ee = (EditorGroupRowArgs)e;
			ImageComboBoxItem item = GetItem(ee);
			Size imageSize = GetImageSize(ee, item);
			string start, end;
			ParseText(ee, out start, out end);
			if(end.Length == 0 || imageSize.IsEmpty) {
				base.DrawObject(e);
				return;
			}
			Size startSize = StringPainter.Default.Calculate(e.Graphics, GetStyle(e), start, 0).Bounds.Size;
			Rectangle text = e.Bounds, image = e.Bounds;
			text.Width = startSize.Width;
			if(ee.RightToLeft) text.X = e.Bounds.Right - text.Width;
			StringPainter.Default.DrawString(e.Cache, GetStyle(e), start, text);
			image.Size = imageSize;
			image.X = text.Right + ImageIndent;
			image.Y += (e.Bounds.Height - imageSize.Height) / 2;
			if(ee.RightToLeft) {
				image.X = text.Left - ImageIndent - image.Width;
				text.X = image.X - ImageIndent;
			}
			else {
				text.X = image.Right + ImageIndent;
			}
			if(BaseEditPainter.CheckRectangle(ee.Bounds, image)) {
				e.Cache.Paint.DrawImage(e, GetImages(e), item.ImageIndex, image, true);
			}
			else {
				if(ee.RightToLeft)
					text.X = image.X + image.Width;
				else
					text.X = image.X - ImageIndent;
			}
			text.Width = ee.Bounds.Right - text.X;
			if(ee.RightToLeft) {
				text.Width = text.X - ee.Bounds.Left;
				text.X = ee.Bounds.Left;
			}
			EditorGroupRowArgs info = new EditorGroupRowArgs(text, ee.Appearance, ee.Properties, ee.EditValue, end);
			info.RightToLeft = ee.RightToLeft;
			info.Cache = ee.Cache;
			info.AllowHtmlDraw = ee.AllowHtmlDraw;
			base.DrawObject(info);
		}
		protected virtual void ParseText(EditorGroupRowArgs e, out string start, out string end) {
			end = string.Empty;
			start = e.Text;
			int index = e.Text.IndexOf(ImageText);
			if(index == -1) return;
			start = e.Text.Substring(0, index);
			end = e.Text.Substring(index + ImageText.Length);
			if(end.Length == 0) end = " ";
		}
	}
}
namespace DevExpress.XtraEditors.Popup {
	public class PopupImageComboBoxEditListBoxForm : PopupListBoxForm {
		public PopupImageComboBoxEditListBoxForm(ComboBoxEdit ownerEdit)
			: base(ownerEdit) {
		}
		protected override PopupListBox CreateListBox() {
			return new PopupImageComboBoxEditListBox(this);
		}
	}
	[ToolboxItem(false)]
	public class PopupImageComboBoxEditListBox : PopupListBox {
		internal static int TextGlyphIndent = 4;
		public PopupImageComboBoxEditListBox(PopupListBoxForm ownerForm)
			: base(ownerForm) {
			ItemHeight = Math.Max(ViewInfo.CalcItemMinHeight(), ItemHeight);
		}
		[Browsable(false)]
		public new ImageComboBoxEdit OwnerEdit { get { return base.OwnerEdit as ImageComboBoxEdit; } }
		[DXCategory(CategoryName.Properties)]
		public new RepositoryItemImageComboBox Properties { get { return base.Properties as RepositoryItemImageComboBox; } }
		[DXCategory(CategoryName.Appearance)]
		public virtual object Images {
			get { return Properties.LargeImages == null ? Properties.SmallImages : Properties.LargeImages; }
		}
		protected override BaseStyleControlViewInfo CreateViewInfo() {
			return new PopupImageComboBoxEditListBoxViewInfo(this);
		}
		public override int CalcItemWidth(GraphicsInfo gInfo, object item) {
			int w = base.CalcItemWidth(gInfo, item);
			w += ImageCollection.GetImageListSize(Images).Width;
			return w + TextGlyphIndent;
		}
		protected override BaseControlPainter CreatePainter() {
			return new PopupImageListBoxPainter();
		}
		[DXCategory(CategoryName.Behavior)] 
		public virtual HorzAlignment GlyphAlignment {
			get {
				if(Properties.GlyphAlignment == HorzAlignment.Far) return HorzAlignment.Far;
				return HorzAlignment.Near;
			}
		}
	}
	public class PopupImageListBoxItemPainter : ComboBoxItemPainter {
		protected override Rectangle CalcItemTextRectangle(ListBoxItemObjectInfoArgs e) {
			return e.TextRect;
		}
	}
	public class PopupImageListBoxSkinItemPainter : ComboBoxSkinItemPainter {
		protected override Rectangle CalcItemTextRectangle(ListBoxItemObjectInfoArgs e) {
			return Rectangle.Inflate(e.TextRect, -2, 0);
		}
	}
	public class PopupImageListBoxPainter : BaseListBoxPainter {
		protected override void DrawItemCore(ControlGraphicsInfoArgs info, BaseListBoxViewInfo.ItemInfo itemInfo, ListBoxDrawItemEventArgs e) {
			PopupImageComboBoxEditListBoxViewInfo viewInfo = info.ViewInfo as PopupImageComboBoxEditListBoxViewInfo;
			ImageListBoxViewInfo.ImageItemInfo item = itemInfo as ImageListBoxViewInfo.ImageItemInfo;
			base.DrawItemCore(info, itemInfo, e);
			if(AllowDrawItemImage(viewInfo, item)) {
				GraphicsInfoArgs ginfo = new GraphicsInfoArgs(new GraphicsCache(e.Graphics), Rectangle.Empty);
				DrawItemImageCore(ginfo, viewInfo, item);
			}
		}
		protected virtual bool AllowDrawItemImage(PopupImageComboBoxEditListBoxViewInfo viewInfo, ImageListBoxViewInfo.ImageItemInfo item) {
			return item != null && !item.ImageRect.IsEmpty && item.ImageIndex > -1;
		}
		protected virtual void DrawItemImageCore(GraphicsInfoArgs ginfo, PopupImageComboBoxEditListBoxViewInfo viewInfo, ImageListBoxViewInfo.ImageItemInfo item) {
			Utils.Paint.XPaint.Graphics.DrawImage(ginfo, viewInfo.OwnerControl.Images, item.ImageIndex, item.ImageRect, true);
		}
	}
	public class PopupImageComboBoxEditListBoxViewInfo : PopupListBoxViewInfo {
		public PopupImageComboBoxEditListBoxViewInfo(PopupImageComboBoxEditListBox owner) : base(owner) { }
		protected internal new PopupImageComboBoxEditListBox OwnerControl { get { return base.OwnerControl as PopupImageComboBoxEditListBox; } }
		protected internal override int CalcItemMinHeight() {
			int h = base.CalcItemMinHeight();
			if(OwnerControl.Images != null) {
				h = Math.Max(ImageCollection.GetImageListSize(OwnerControl.Images).Height + 2, h);
			}
			return h;
		}
		protected override BaseListBoxViewInfo.ItemInfo CreateItemInfo(Rectangle bounds, object item, string text, int index) {
			ImageListBoxViewInfo.ImageItemInfo itemInfo = CreateImageInfoInstance(bounds, item, text, index);
			ImageComboBoxItem pi = OwnerControl.GetItem(index) as ImageComboBoxItem;
			if(pi != null && ImageCollection.IsImageListImageExists(OwnerControl.Images, pi.ImageIndex))
				itemInfo.ImageIndex = pi.ImageIndex;
			CalcItemRects(bounds, itemInfo);
			return itemInfo;
		}
		protected virtual ImageListBoxViewInfo.ImageItemInfo CreateImageInfoInstance(Rectangle bounds, object item, string text, int index) {
			return new ImageListBoxViewInfo.ImageItemInfo(OwnerControl, bounds, item, text, index);
		}
		void CalcItemRects(Rectangle bounds, ImageListBoxViewInfo.ImageItemInfo itemInfo) {
			Rectangle textRect = Rectangle.Inflate(bounds, -2, -1);
			Rectangle glyphRect = Rectangle.Empty;
			if(!ImageCollection.GetImageListSize(OwnerControl.Images).IsEmpty) {
				glyphRect.Size = ImageCollection.GetImageListSize(OwnerControl.Images);
				glyphRect.Y = textRect.Y + (textRect.Height - glyphRect.Size.Height) / 2;
				glyphRect.X = textRect.X;
				bool rightToLeft = OwnerControl.ViewInfo.RightToLeft;
				if((OwnerControl.GlyphAlignment == HorzAlignment.Near && !rightToLeft) ||
					(OwnerControl.GlyphAlignment == HorzAlignment.Far && rightToLeft)) {
					textRect.X += (glyphRect.Size.Width + PopupImageComboBoxEditListBox.TextGlyphIndent);
				}
				else {
					glyphRect.X = textRect.Right - glyphRect.Size.Width;
				}
				textRect.Width -= (glyphRect.Size.Width + PopupImageComboBoxEditListBox.TextGlyphIndent);
			}
			itemInfo.ImageRect = glyphRect;
			itemInfo.TextRect = textRect;
		}
		protected override BaseListBoxItemPainter CreateItemPainter() {
			if(IsSkinnedHighlightingEnabled)
				return new PopupImageListBoxSkinItemPainter();
			return new PopupImageListBoxItemPainter();
		}
	}
}
namespace DevExpress.XtraEditors.Controls {
	[TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter)),
	 Editor("DevExpress.XtraEditors.Design.ImageComboBoxItemCollectionEditor, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(System.Drawing.Design.UITypeEditor))]
	public class ImageComboBoxItemCollection : ComboBoxItemCollection, System.Collections.Generic.IEnumerable<ImageComboBoxItem> {
		Hashtable valueIndexes;
		public ImageComboBoxItemCollection(RepositoryItemImageComboBox properties)
			: base(properties) {
			this.valueIndexes = new Hashtable();
		}
#if !SL
	[DevExpressXtraEditorsLocalizedDescription("ImageComboBoxItemCollectionItem")]
#endif
		public new ImageComboBoxItem this[int index] {
			get { return base[index] as ImageComboBoxItem; }
			set {
				if(value == null) return;
				base[index] = value;
			}
		}
		protected override IComparer CreateComparer() {
			return new ListComaprer();
		}
		class ListComaprer : IComparer {
			IComparer comparer = new DevExpress.Data.ValueComparer();
			int IComparer.Compare(object a, object b) {
				ImageComboBoxItem i1 = a as ImageComboBoxItem, i2 = b as ImageComboBoxItem;
				if(i1 == null)
					return i2 == null ? 0 : -1;
				if(i2 == null) return 1;
				return comparer.Compare(i1.Description, i2.Description);
			}
		}
		protected new RepositoryItemImageComboBox Properties { get { return base.Properties as RepositoryItemImageComboBox; } }
		public int Add(ImageComboBoxItem item) {
			return base.Add(item);
		}
		public void AddRange(ImageComboBoxItem[] items) {
			base.AddRange(items);
		}
		public void AddEnum(Type enumType, bool addEnumeratorIntegerValues) {
			BeginUpdate();
			try {
				int imageIndex = 0;
				Array values = EnumDisplayTextHelper.GetEnumValues(enumType);
				foreach(object obj in values) {
					if(ValueIndexes.ContainsKey(obj)) continue;
					object value = EnumDisplayTextHelper.GetEnumValue(addEnumeratorIntegerValues, obj, enumType);
					this.Add(new ImageComboBoxItem(EnumDisplayTextHelper.GetDisplayText(obj), value, imageIndex++));
				}
			}
			finally { EndUpdate(); }
		}
		public void AddEnum(Type enumType) {
			AddEnum(enumType, false);
		}
		public void AddEnum<TEnum>() {
			AddEnum<TEnum>(null);
		}
		public void AddEnum<TEnum>(Converter<TEnum, string> displayTextConverter) {
			if(displayTextConverter == null)
				displayTextConverter = (v) => EnumDisplayTextHelper.GetDisplayText(v);
			BeginUpdate();
			try {
				int imageIndex = 0;
				var values = EnumDisplayTextHelper.GetEnumValues<TEnum>();
				foreach(TEnum value in values) {
					if(ValueIndexes.ContainsKey(value)) continue;
					base.Add(new ImageComboBoxItem(displayTextConverter(value), value, imageIndex++));
				}
			}
			finally { EndUpdate(); }
		}
		public virtual ImageComboBoxItem GetItem(object val) {
			if(Count == 0) return null;
			ImageComboBoxItem pi;
			if(val != null) {
				pi = ValueIndexes[val] as ImageComboBoxItem;
				if(pi != null) return pi;
				pi = GetPickItem(val);
				if(pi != null)
					ValueIndexes[val] = pi;
			}
			else {
				pi = GetPickItem(val);
			}
			return pi;
		}
		public override string GetItemDescription(object item) {
			ImageComboBoxItem pi = item as ImageComboBoxItem;
			return pi == null ? "" : pi.Description;
		}
		protected virtual ImageComboBoxItem GetPickItem(object val) {
			for(int n = Count - 1; n >= 0; n--) {
				ImageComboBoxItem pi = this[n];
				object piVal = pi.Value;
				if(piVal == null && val == null) return pi;
				if(piVal == val) return pi;
				if(piVal == null) return null;
				try {
					if(piVal.Equals(val)) return pi;
				}
				catch {
				}
			}
			return null;
		}
		protected virtual Hashtable ValueIndexes { get { return valueIndexes; } }
		protected override object CloneItem(object item) {
			ImageComboBoxItem pi = new ImageComboBoxItem();
			pi.Assign(item as ImageComboBoxItem);
			return pi;
		}
		protected override object ExtractItem(object item) {
			return item;
		}
		protected override bool CanAddItem(object item) { return item is ImageComboBoxItem; }
		protected override void OnInsertComplete(int index, object item) {
			ImageComboBoxItem pi = item as ImageComboBoxItem;
			pi.Changed += new EventHandler(OnItemChanged);
			pi.Properties = Properties;
			if(pi.Value != null)
				ValueIndexes[pi.Value] = pi;
			base.OnInsertComplete(index, item);
		}
		protected override void OnRemoveComplete(int index, object item) {
			ImageComboBoxItem pi = item as ImageComboBoxItem;
			pi.Changed -= new EventHandler(OnItemChanged);
			pi.Properties = null;
			if(pi.Value != null)
				ValueIndexes.Remove(pi.Value);
			base.OnRemoveComplete(index, item);
		}
		protected override void OnSetComplete(int index, object oldValue, object newValue) {
			ImageComboBoxItem piOld = oldValue as ImageComboBoxItem, piNew = newValue as ImageComboBoxItem;
			piOld.Changed -= new EventHandler(OnItemChanged);
			piOld.Properties = null;
			piNew.Changed += new EventHandler(OnItemChanged);
			piNew.Properties = Properties;
			if(piOld.Value != null) {
				ValueIndexes.Remove(piOld.Value);
			}
			base.OnSetComplete(index, oldValue, newValue);
		}
		int prevIndex = -1;
		protected override void OnClear() {
			if(Count == 0) return;
			if(Properties != null && Properties.OwnerEdit != null)
				prevIndex = Properties.OwnerEdit.SelectedIndex;
			BeginUpdate();
			try {
				for(int n = Count - 1; n >= 0; n--) {
					RemoveAt(n);
				}
			}
			finally {
				EndUpdate();
			}
			Indexes.Clear();
			ValueIndexes.Clear();
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			if(prevIndex != -1) {
				ImageComboBoxEdit edit = Properties.OwnerEdit as ImageComboBoxEdit;
				if(edit != null)
					edit.ItemsClearComplete(prevIndex);
				prevIndex = -1;
			}
		}
		protected virtual void OnItemChanged(object sender, EventArgs e) {
			ValueIndexes.Clear();
			ImageComboBoxItem pi = sender as ImageComboBoxItem;
			if(pi.Value != null) ValueIndexes[pi.Value] = pi.Description;
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, sender));
		}
		#region IEnumerable<ImageComboBoxItem>
		System.Collections.Generic.IEnumerator<ImageComboBoxItem> System.Collections.Generic.IEnumerable<ImageComboBoxItem>.GetEnumerator() {
			foreach(ImageComboBoxItem item in InnerList) yield return item;
		}
		#endregion
	}
	[Obsolete(ObsoleteText.SRObsoletePickImageItem)]
	public class PickImageItem : ImageComboBoxItem {
		public PickImageItem()
			: this("", null) {
		}
		public PickImageItem(string description, object value, int imageIndex)
			: base(description, value, imageIndex) {
		}
		public PickImageItem(string description, object value) : this(description, value, -1) { }
		public PickImageItem(object value, int imageIndex) : this(null, value, imageIndex) { }
		public PickImageItem(string description, int imageIndex) : this(description, null, imageIndex) { }
		public PickImageItem(int imageIndex) : this(null, null, imageIndex) { }
		public PickImageItem(object value) : this(null, value, -1) { }
		public PickImageItem(string description) : this(description, null, -1) { }
	}
	[TypeConverter("DevExpress.XtraEditors.Design.ImageComboBoxItemTypeConverter, " + AssemblyInfo.SRAssemblyEditorsDesign)]
	public class ImageComboBoxItem : ComboBoxItem, ICaptionSupport {
		string description;
		int imageIndex;
		protected internal event EventHandler Changed;
		RepositoryItemImageComboBox properties;
		public ImageComboBoxItem()
			: this("", null) {
		}
		public ImageComboBoxItem([Localizable(true)] string description, object value, int imageIndex)
			: base(value) {
			this.description = description == null ? "" : description;
			this.imageIndex = imageIndex;
			this.properties = null;
		}
		public ImageComboBoxItem(string description, object value) : this(description, value, -1) { }
		public ImageComboBoxItem(object value, int imageIndex) : this(string.Format("{0}", value), value, imageIndex) { }
		public ImageComboBoxItem(string description, int imageIndex) : this(description, description, imageIndex) { }
		public ImageComboBoxItem(int imageIndex) : this(null, imageIndex, imageIndex) { }
		public ImageComboBoxItem(object value) : this(value, -1) { }
		public ImageComboBoxItem(string description) : this(description, description, -1) { }
		string ICaptionSupport.Caption { get { return Description; } }
		protected internal RepositoryItemImageComboBox Properties { get { return properties; } set { properties = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object Images {
			get {
				if(Properties == null) return null;
				return Properties.LargeImages == null ? Properties.SmallImages : Properties.LargeImages;
			}
		}
		public virtual void Assign(ImageComboBoxItem pi) {
			if(pi == null) return;
			Description = pi.Description;
			Value = pi.Value;
			ImageIndex = pi.ImageIndex;
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ImageComboBoxItemImageIndex"),
#endif
 DefaultValue(-1), Editor(typeof(DevExpress.Utils.Design.ImageIndexesEditor), typeof(System.Drawing.Design.UITypeEditor)), ImageList("Images")]
		public int ImageIndex {
			get { return imageIndex; }
			set {
				if(ImageIndex == value) return;
				imageIndex = value;
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ImageComboBoxItemDescription"),
#endif
 DefaultValue("")]
		public string Description {
			get { return description; }
			set {
				if(Description == value) return;
				description = value;
				OnChanged();
			}
		}
		public override string ToString() {
			if(Description != "") return Description;
			return base.ToString();
		}
		protected override void OnChanged() {
			if(Changed != null) Changed(this, EventArgs.Empty);
		}
	}
}
