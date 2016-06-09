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
using System.Drawing;
using System.Reflection;
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.Utils.Drawing;
using DevExpress.XtraRichEdit.Design.Internal;
using DevExpress.Utils;
using System.Collections.Generic;
using DevExpress.XtraRichEdit.Forms.Design;
using System.Windows.Forms;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office;
using DevExpress.Office.Design.Internal;
namespace DevExpress.XtraRichEdit.Forms.Design {
	#region RepositoryItemBorderLineWeight
	[
	UserRepositoryItem("RegisterBorderLineWeightEdit"),
	System.Runtime.InteropServices.ComVisible(false)
	]
	public class RepositoryItemBorderLineWeight : RepositoryItemImageComboBox {
		 static string InternalEditorTypeName { get { return typeof(BorderLineWeightEdit).Name; } }
		static RepositoryItemBorderLineWeight() { RegisterBorderLineWeightEdit(); }
		public static void RegisterBorderLineWeightEdit() {
			Bitmap img = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraRichEdit.Bitmaps256.RichTextEdit.bmp", Assembly.GetExecutingAssembly());
			img.MakeTransparent(Color.Magenta);
			EditorClassInfo editorInfo = new EditorClassInfo(typeof(BorderLineWeightEdit).Name, typeof(BorderLineWeightEdit), typeof(RepositoryItemBorderLineWeight), typeof(BorderLineWeightImageComboBoxEditViewInfo), new BorderLineWeightPainter(), true, img);
			editorInfo.AllowInplaceEditing = ShowInContainerDesigner.OnlyInBars;
			EditorRegistrationInfo.Default.Editors.Add(editorInfo);
		}
		ImageList imageList;
		RichEditControl control;
		public RepositoryItemBorderLineWeight() {
			GlyphAlignment = HorzAlignment.Far;
			this.imageList = new ImageList();
			imageList.ColorDepth = ColorDepth.Depth24Bit;
			imageList.ImageSize = new Size(48, 16);
			imageList.TransparentColor = Color.Transparent;
			this.SmallImages = imageList;
			this.LargeImages = imageList;
		}
		#region Properties
		public override string EditorTypeName { get { return InternalEditorTypeName; } }
		#region Control
		public RichEditControl Control {
			get { return control; }
			set {
				if (control == value)
					return;
				if (control != null)
					UnsubscribeEvents();
				control = value;
				if (control != null) {
					OnControlChanged();
					SubscribeEvents();
				}
			}
		}
		#endregion
		#region Items
		[Localizable(false), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImageComboBoxItemCollection Items { get { return base.Items; } }
		protected internal override bool ShouldSerializeItems() {
			return false;
		}
		#endregion
		[DefaultValue(HorzAlignment.Far)]
		public override HorzAlignment GlyphAlignment { get { return base.GlyphAlignment; } set { base.GlyphAlignment = value; } }
		protected internal virtual BorderInfoRepository BorderInfoRepository {
			get {
				if (Control != null)
					return Control.DocumentModel.TableBorderInfoRepository;
				else
					return null;
			}
		}
		#endregion
		protected internal virtual void SubscribeEvents() {
			Control.DocumentLoaded += OnDocumentRecreated;
			Control.EmptyDocumentCreated += OnDocumentRecreated;
			Control.DocumentModel.DocumentCleared += OnDocumentRecreated;
			Control.BeforeDispose += OnControlBeforeDispose;
			BorderInfoRepository.UpdateUI += OnBorderInfoRepositoryUpdateUI;
		}
		protected internal virtual void UnsubscribeEvents() {
			Control.DocumentLoaded -= OnDocumentRecreated;
			Control.EmptyDocumentCreated -= OnDocumentRecreated;
			Control.DocumentModel.DocumentCleared -= OnDocumentRecreated;
			Control.BeforeDispose -= OnControlBeforeDispose;
			BorderInfoRepository.UpdateUI -= OnBorderInfoRepositoryUpdateUI;
		}
		protected virtual void OnControlBeforeDispose(object sender, EventArgs e) {
			this.Control = null;
		}
		protected virtual void OnControlChanged() {
			if (control == null)
				return;
			PopulateItems();
		}
		void OnDocumentRecreated(object sender, EventArgs e) {
			if (!DesignMode)
				PopulateItems();
		}
		protected virtual void OnBorderInfoRepositoryUpdateUI(object sender, EventArgs e) {
			GenerateImages();
		}
		public override void BeginInit() {
			base.BeginInit();
			Items.Clear();
		}
		public override void EndInit() {
			if (Items.Count <= 0)
				PopulateItems();
			base.EndInit();
		}
		protected void PopulateItems() {
			BeginUpdate();
			try {
				Items.Clear();
				if (Control != null)
					PopulateItemsCore();
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual void PopulateItemsCore() {
			AddItem(0.25f);
			AddItem(0.5f);
			AddItem(0.75f);
			AddItem(1.0f);
			AddItem(1.5f);
			AddItem(2.25f);
			AddItem(3.0f);
			AddItem(4.5f);
			AddItem(6.0f);
			GenerateImages();
		}
		protected void AddItem(float weightInPoints) {
			int imageIndex = Items.Count;
			float weight = Control.DocumentModel.UnitConverter.PointsToModelUnitsF(weightInPoints);
			int value = Math.Max(1, (int)Math.Round(weight));
			Items.Add(new ImageComboBoxItem(String.Format("{0}{1}", weightInPoints, UIUnit.GetTextAbbreviation(DocumentUnit.Point)), value, imageIndex));
		}
		public override void Assign(RepositoryItem item) {
			base.Assign(item);
			RepositoryItemBorderLineWeight value = item as RepositoryItemBorderLineWeight;
			if (value != null)
				this.Control = value.Control;
		}
		void GenerateImages() {
			BorderInfo borderInfo = BorderInfoRepository.CurrentItem;			
			imageList.Images.Clear();
			int count = Items.Count;
			for (int i = 0; i < count; i++) {
				Image image = GenerateItemImage(Items[i], borderInfo.Style, borderInfo.Color);
				imageList.Images.Add(image);
			}
		}
		Image GenerateItemImage(ImageComboBoxItem item, BorderLineStyle style, Color color) {
			Size size = imageList.ImageSize;
			Bitmap bitmap = new Bitmap(size.Width, size.Height);
			BorderInfo borderInfo = new BorderInfo();
			borderInfo.Width = (int)item.Value;
			borderInfo.Style = style;
			borderInfo.Color = color;
			using (GraphicsCache cache = new GraphicsCache(Graphics.FromImage(bitmap))) {
				cache.Graphics.Clear(Color.Transparent);
				Rectangle bounds = new Rectangle(Point.Empty, size);
				BorderLinePainterHelper.DrawBorderLineItem(borderInfo, cache, bounds, null, Control.DocumentModel.UnitConverter, false);
			}
			return bitmap;
		}
	}
	#endregion
	#region BorderLineWeightEdit
	[DXToolboxItem(false)]
	public partial class BorderLineWeightEdit : ImageComboBoxEdit {
		static BorderLineWeightEdit() {
			RepositoryItemBorderLineWeight.RegisterBorderLineWeightEdit();
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemBorderLineWeight Properties { get { return base.Properties as RepositoryItemBorderLineWeight; } }
		public override string EditorTypeName { get { return GetType().Name; } }
		public RichEditControl Control {
			get { return Properties != null ? Properties.Control : null; }
			set {
				if (Properties != null && !Object.ReferenceEquals(Properties.Control, value)) {
					Properties.Control = value;
				}
			}
		}
		protected override PopupBaseForm CreatePopupForm() {
			return new BorderLineWeightEditPopupImageComboBoxEditListBoxForm(this);
		}
	}
	#endregion
	#region RepositoryItemFloatingObjectOutlineWeight
	public class RepositoryItemFloatingObjectOutlineWeight : RepositoryItemBorderLineWeight {
		 static string InternalEditorTypeName { get { return typeof(FloatingObjectOutlineWeightEdit).Name; } }
		static RepositoryItemFloatingObjectOutlineWeight() { RegisterFloatingObjectOutlineWeightEdit(); }
		public static void RegisterFloatingObjectOutlineWeightEdit() {
			Bitmap img = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraRichEdit.Bitmaps256.RichTextEdit.bmp", Assembly.GetExecutingAssembly());
			img.MakeTransparent(Color.Magenta);
			EditorClassInfo editorInfo = new EditorClassInfo(typeof(FloatingObjectOutlineWeightEdit).Name, typeof(FloatingObjectOutlineWeightEdit), typeof(RepositoryItemFloatingObjectOutlineWeight), typeof(BorderLineWeightImageComboBoxEditViewInfo), new BorderLineWeightPainter(), true, img);
			editorInfo.AllowInplaceEditing = ShowInContainerDesigner.OnlyInBars;
			EditorRegistrationInfo.Default.Editors.Add(editorInfo);
		}
		protected internal override BorderInfoRepository BorderInfoRepository {
			get {
				if (Control != null)
					return Control.DocumentModel.FloatingObjectBorderInfoRepository;
				else
					return null;
			}
		}
		protected internal override void PopulateItemsCore() {
			AddItem(0.0f);
			base.PopulateItemsCore();
		}
	}
	#endregion
	#region FloatingObjectOutlineWeightEdit
	[DXToolboxItem(false)]
	public partial class FloatingObjectOutlineWeightEdit : BorderLineWeightEdit {
		static FloatingObjectOutlineWeightEdit() {
			RepositoryItemFloatingObjectOutlineWeight.RegisterFloatingObjectOutlineWeightEdit();
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemFloatingObjectOutlineWeight Properties { get { return base.Properties as RepositoryItemFloatingObjectOutlineWeight; } }
		protected override PopupBaseForm CreatePopupForm() {
			return new BorderLineWeightEditPopupImageComboBoxEditListBoxForm(this);
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Design.Internal {
	public class BorderLineWeightPainter : ImageComboBoxEditPainter {
	}
	public class BorderLineWeightEditPopupImageListBoxSkinItemPainter : PopupImageListBoxSkinItemPainter {
		protected override void DrawItemText(ListBoxItemObjectInfoArgs e) {
			AppearanceObject appearance = e.PaintAppearance;
			e.PaintAppearance = e.ViewInfo.PaintAppearance;
			base.DrawItemText(e);
			e.PaintAppearance = appearance;
		}
		protected override void DrawItemBarCore(ListBoxItemObjectInfoArgs e) {
			AppearanceObject appearance = e.PaintAppearance;
			e.PaintAppearance = e.ViewInfo.PaintAppearance;
			base.DrawItemBarCore(e);
			e.PaintAppearance = appearance;
			Rectangle selectionRectBounds = e.Bounds;
			selectionRectBounds.Width--;
			e.Cache.DrawRectangle(e.Cache.GetPen(e.PaintAppearance.BackColor), selectionRectBounds);
		}
	}
	public class BorderLineWeightEditPopupImageListBoxItemPainter : PopupImageListBoxItemPainter {
		protected override void DrawItemText(ListBoxItemObjectInfoArgs e) {
			AppearanceObject appearance = e.PaintAppearance;
			e.PaintAppearance = e.ViewInfo.PaintAppearance;
			base.DrawItemText(e);
			e.PaintAppearance = appearance;
		}
		protected override void DrawItemBar(ListBoxItemObjectInfoArgs e) {
			AppearanceObject appearance = e.PaintAppearance;
			e.PaintAppearance = e.ViewInfo.PaintAppearance;
			base.DrawItemBar(e);
			e.PaintAppearance = appearance;
			Rectangle selectionRectBounds = e.Bounds;
			selectionRectBounds.Width--;
			e.Cache.DrawRectangle(e.Cache.GetPen(e.PaintAppearance.BackColor), selectionRectBounds);
		}
	}
	public class BorderLineWeightEditPopupImageComboBoxEditListBoxViewInfo : PopupImageComboBoxEditListBoxViewInfo {
		public BorderLineWeightEditPopupImageComboBoxEditListBoxViewInfo(PopupImageComboBoxEditListBox owner)
			: base(owner) {
		}
		public new ListBoxItemObjectInfoArgs ListBoxItemInfoArgs { get { return base.ListBoxItemInfoArgs; } }
		protected internal RichEditControl RichEditControl {
			get {
				if (OwnerControl == null)
					return null;
				BorderLineWeightEdit edit = OwnerControl.OwnerEdit as BorderLineWeightEdit;
				if (edit == null)
					return null;
				return edit.Control;
			}
		}
		protected override BaseListBoxItemPainter CreateItemPainter() {
			if (IsSkinnedHighlightingEnabled)
				return new BorderLineWeightEditPopupImageListBoxSkinItemPainter();
			return new BorderLineWeightEditPopupImageListBoxItemPainter();
		}
		protected override ListBoxItemObjectInfoArgs CreateListBoxItemInfoArgs() {
			return new ValueListBoxItemObjectInfoArgs(this, null, Rectangle.Empty);
		}
		protected internal override int CalcItemMinHeight() {
			int height = 0;
			if (RichEditControl != null) {
				DocumentModelUnitConverter unitConverter = RichEditControl.DocumentModel.UnitConverter;
				height = unitConverter.ModelUnitsToPixels(unitConverter.PointsToModelUnits(20));
			}
			return Math.Max(base.CalcItemMinHeight(), height);
		}
	}
	public class BorderLineWeightEditPopupImageComboBoxEditListBox : PopupImageComboBoxEditListBox {
		public BorderLineWeightEditPopupImageComboBoxEditListBox(PopupListBoxForm ownerForm)
			: base(ownerForm) {
		}
		protected override DevExpress.XtraEditors.ViewInfo.BaseStyleControlViewInfo CreateViewInfo() {
			return new BorderLineWeightEditPopupImageComboBoxEditListBoxViewInfo(this);
		}
		public new BorderLineWeightEditPopupImageComboBoxEditListBoxViewInfo ViewInfo { get { return (BorderLineWeightEditPopupImageComboBoxEditListBoxViewInfo)base.ViewInfo; } }
	}
	public class BorderLineWeightEditPopupImageComboBoxEditListBoxForm : PopupImageComboBoxEditListBoxForm {
		public BorderLineWeightEditPopupImageComboBoxEditListBoxForm(ComboBoxEdit ownerEdit)
			: base(ownerEdit) {
		}
		protected override PopupListBox CreateListBox() {
			return new BorderLineWeightEditPopupImageComboBoxEditListBox(this);
		}
		public override int CalcMinimumComboWidth() {
			return OwnerEdit.Width;
		}
	}
	public class BorderLineWeightImageComboBoxEditViewInfo : ImageComboBoxEditViewInfo {
		public BorderLineWeightImageComboBoxEditViewInfo(RepositoryItem item)
			: base(item) {
		}
		public RichEditControl RichEditControl {
			get {
				RepositoryItemBorderLineWeight owner = Owner as RepositoryItemBorderLineWeight;
				if (owner != null)
					return owner.Control;
				else
					return null;
			}
		}
	}
}
