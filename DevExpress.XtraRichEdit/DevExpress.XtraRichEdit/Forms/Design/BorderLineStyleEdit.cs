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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.Office;
using DevExpress.Office.Drawing;
using DevExpress.Office.Layout;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraRichEdit.Design.Internal;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Forms.Design;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Localization;
namespace DevExpress.XtraRichEdit.Forms.Design {
	#region RepositoryItemBorderLineStyle
	[
	UserRepositoryItem("RegisterBorderLineStyleEdit"),
	System.Runtime.InteropServices.ComVisible(false)
	]
	public class RepositoryItemBorderLineStyle : RepositoryItemImageComboBox, IBorderLineStyleEditor {
		internal static string InternalEditorTypeName { get { return typeof(BorderLineStyleEdit).Name; } }
		static RepositoryItemBorderLineStyle() { RegisterBorderLineStyleEdit(); }
		public static void RegisterBorderLineStyleEdit() {
			Bitmap img = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraRichEdit.Bitmaps256.RichTextEdit.bmp", Assembly.GetExecutingAssembly());
			img.MakeTransparent(Color.Magenta);
			EditorClassInfo editorInfo = new EditorClassInfo(typeof(BorderLineStyleEdit).Name, typeof(BorderLineStyleEdit), typeof(RepositoryItemBorderLineStyle), typeof(BorderLineStyleImageComboBoxEditViewInfo), new BorderLineStylePainter(), true, img);
			editorInfo.AllowInplaceEditing = ShowInContainerDesigner.OnlyInBars;
			EditorRegistrationInfo.Default.Editors.Add(editorInfo);
		}
		RichEditControl control;
		readonly BorderLineStyleEditorHelper lineStyleEditorHelper;
		public RepositoryItemBorderLineStyle() {
			lineStyleEditorHelper = new BorderLineStyleEditorHelper(this);
		}
		#region Properties
		public override string EditorTypeName { get { return InternalEditorTypeName; } }
		public DocumentModel DocumentModel { get { return Control != null ? Control.DocumentModel : null; } }
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
		#endregion
		protected internal virtual void SubscribeEvents() {
			Control.DocumentLoaded += OnDocumentRecreated;
			Control.EmptyDocumentCreated += OnDocumentRecreated;
			Control.DocumentModel.DocumentCleared += OnDocumentRecreated;
			Control.BeforeDispose += OnControlBeforeDispose;
		}
		protected internal virtual void UnsubscribeEvents() {
			Control.DocumentLoaded -= OnDocumentRecreated;
			Control.EmptyDocumentCreated -= OnDocumentRecreated;
			Control.DocumentModel.DocumentCleared -= OnDocumentRecreated;
			Control.BeforeDispose -= OnControlBeforeDispose;
		}
		protected virtual void OnControlBeforeDispose(object sender, EventArgs e) {
			this.Control = null;
		}
		protected virtual void OnControlChanged() {
			if (control == null)
				return;
			PopulateItems();
		}
		void PopulateItems() {
			lineStyleEditorHelper.PopulateItems(false);
		}
		void OnDocumentRecreated(object sender, EventArgs e) {
			if (!DesignMode)
				PopulateItems();
		}
		public override void BeginInit() {
			base.BeginInit();
			lineStyleEditorHelper.OnBeginInit();			
		}
		public override void EndInit() {
			lineStyleEditorHelper.OnEndInit(false);
			base.EndInit();
		}
		public override void Assign(RepositoryItem item) {
			base.Assign(item);
			RepositoryItemBorderLineStyle value = item as RepositoryItemBorderLineStyle;
			if (value != null)
				this.Control = value.Control;
		}
		#region IBorderLineStyleEditor implementation
		void IBorderLineStyleEditor.Clear() {
			Items.Clear();
		}
		int IBorderLineStyleEditor.ItemsCount {
			get { return Items.Count; }
		}
		void IBorderLineStyleEditor.AddItem(string description, BorderInfo borderInfo) {
			Items.Add(new ImageComboBoxItem(description, borderInfo, -1));
		}
		#endregion
	}
	#endregion
	#region BorderLineStyleEdit
	[DXToolboxItem(false)]
	public partial class BorderLineStyleEdit : ImageComboBoxEdit {
		static BorderLineStyleEdit() {
			RepositoryItemBorderLineStyle.RegisterBorderLineStyleEdit();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BorderLineStyle BorderLineStyle {
			get {
				if (IsNullValue(EditValue))
					return BorderLineStyle.None;
				return ((BorderInfo)EditValue).Style;
			}
			set {
				if (Control == null) {
					base.EditValue = null;
					return;
				}
				base.EditValue = Control.DocumentModel.TableBorderInfoRepository.GetItemByLineStyle(value);
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemBorderLineStyle Properties { get { return base.Properties as RepositoryItemBorderLineStyle; } }
		public override string EditorTypeName { get { return GetType().Name; } }
		public RichEditControl Control {
			get { return Properties != null ? Properties.Control : null; }
			set {
				if (Properties != null && !Object.ReferenceEquals(Properties.Control, value)) {
					Properties.Control = value;
					EditValue = value.DocumentModel.TableBorderInfoRepository.CurrentItem;
				}
			}
		}
		protected override PopupBaseForm CreatePopupForm() {
			return new BorderLineStyleEditPopupImageComboBoxEditListBoxForm(this);
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Design.Internal {
	public static class BorderLinePainterHelper {
		public static void DrawBorderLineItem(ListBoxItemObjectInfoArgs e, DrawItemTextHandler handler, DocumentModelUnitConverter unitConverter) {
			BorderInfo value = GetBorderInfo(e);
			if (value == null)
				return;
			if (value.Style == BorderLineStyle.None || value.Style == BorderLineStyle.Nil) {
				AppearanceObject appearance = e.PaintAppearance;
				e.PaintAppearance = e.ViewInfo.PaintAppearance;
				handler(e);
				e.PaintAppearance = appearance;
			}
			else
				DrawBorderLineItem(value, e.Cache, e.Bounds, e.PaintAppearance, unitConverter, false);
		}
		private static BorderInfo GetBorderInfo(ListBoxItemObjectInfoArgs e) {
			BorderLineStyleValueListBoxItemObjectInfoArgs args = e as BorderLineStyleValueListBoxItemObjectInfoArgs;
			if (args != null) 
				return args.BorderInfo;
			ValueListBoxItemObjectInfoArgs valueArgs = e as ValueListBoxItemObjectInfoArgs;
			if (valueArgs != null)
				return valueArgs.ItemValue.Value as BorderInfo;
			return null;
		}
		public static void DrawBorderLineItem(BorderInfo borderInfo, GraphicsCache cache, Rectangle bounds, AppearanceObject appearance, DocumentModelUnitConverter unitConverter, bool isSelected) {
			Color color;
			if (isSelected && appearance != null)
				color = appearance.ForeColor;
			else
				color = DXColor.IsTransparentOrEmpty(borderInfo.Color) && (appearance != null) ? appearance.ForeColor : borderInfo.Color;
			if (borderInfo.Style == BorderLineStyle.DoubleWave || borderInfo.Style == BorderLineStyle.Wave) {
				Underline underline = BorderInfoRepository.GetUnderlineByBorderLineStyle(borderInfo.Style);
				int thickness = Math.Max(1, unitConverter.ModelUnitsToPixels(borderInfo.Width));
				UnderlinePainterHelper.DrawUnderlineItemCore(underline, cache, bounds, color, thickness, thickness);
				return;
			}
			Graphics graphics = cache.Graphics;
			RectangleF oldClip = graphics.ClipBounds;
			System.Drawing.Drawing2D.Matrix oldTransform = graphics.Transform;
			int scaleFactor = 12;
			using (GdiPlusPainter painter = new GdiPlusPainter(cache)) {
				Rectangle clipRect = UnderlinePainterHelper.CalcClipBounds(bounds);
				painter.Graphics.SetClip(clipRect);				
				TableBorderPainter borderPainter = GetBorderTablePainter(borderInfo, painter, unitConverter, scaleFactor);
				if (borderPainter != null) {					
					graphics.TranslateTransform(0, (bounds.Top + bounds.Bottom) / 2f);
					graphics.ScaleTransform(1, 1f / scaleFactor);
					borderPainter.DrawHorizontalBorder(color, new PointF(bounds.Left, -borderPainter.Width / 2f), bounds.Width);					
				}
			}
			graphics.Transform = oldTransform;
			graphics.SetClip(oldClip);
		}
		internal static TableBorderPainter GetBorderTablePainter(BorderInfo borderInfo, Painter painter, DocumentModelUnitConverter unitConverter, int scaleFactor) {
			TableBorderCalculator borderCalculator = new TableBorderCalculator();
			float[] compoundArray = borderCalculator.GetDrawingCompoundArray(borderInfo.Style);
			if (compoundArray == null)
				compoundArray = borderCalculator.GetDrawingCompoundArray(BorderLineStyle.Single);
			if (compoundArray == null)
				return null;
			int thickness = Math.Max(1, unitConverter.ModelUnitsToPixels(borderInfo.Width * scaleFactor));
			int width = borderCalculator.GetActualWidth(borderInfo.Style,  thickness);
			ICharacterLinePainter horizontalLinePainter = new RichEditHorizontalPatternLinePainter(painter, new DocumentLayoutUnitPixelsConverter(unitConverter.ScreenDpiX, unitConverter.ScreenDpiY));
			ICharacterLinePainter verticalLinePainter = new RichEditVerticalPatternLinePainter(painter, new DocumentLayoutUnitPixelsConverter(unitConverter.ScreenDpiX, unitConverter.ScreenDpiY));
			GraphicsPainterWrapper painterWrapper = new GraphicsPainterWrapper(painter, horizontalLinePainter, verticalLinePainter);
			if (compoundArray.Length == 4)
				return new DevExpress.XtraRichEdit.Layout.DoubleBorderPainter(painterWrapper, compoundArray, width);
			else if (compoundArray.Length == 6)
				return new TripleBorderPainter(painterWrapper, compoundArray, width);
			else
				return new SingleBorderPainter(painterWrapper, width, GetTableBorderLine(borderInfo.Style));
		}
		internal static Underline GetTableBorderLine(BorderLineStyle borderLineStyle) {
			if (borderLineStyle == BorderLineStyle.Single)
				return null;
			return BorderInfoRepository.GetUnderlineByBorderLineStyle(borderLineStyle);
		}
	}
	public class BorderLineStylePainter : ImageComboBoxEditPainter {
		protected override void DrawText(ControlGraphicsInfoArgs info) {
			BorderLineStyleImageComboBoxEditViewInfo viewInfo = info.ViewInfo as BorderLineStyleImageComboBoxEditViewInfo;
			if (viewInfo == null)
				return;
			RichEditControl control = viewInfo.RichEditControl;
			if (control == null)
				return;
			BorderInfo value = viewInfo.EditValue as BorderInfo;
			if (value == null)
				return;
			if (value.Style == BorderLineStyle.None || value.Style == BorderLineStyle.Nil)
				base.DrawText(info);
			else
				BorderLinePainterHelper.DrawBorderLineItem(value, info.Cache, info.Bounds, viewInfo.PaintAppearance, viewInfo.RichEditControl.DocumentModel.UnitConverter, viewInfo.State == ObjectState.Selected);
		}
	}
	public class BorderLineStyleEditPopupImageListBoxSkinItemPainter : PopupImageListBoxSkinItemPainter {
		protected override void DrawItemText(ListBoxItemObjectInfoArgs e) {
			BorderLineStyleEditPopupImageComboBoxEditListBoxViewInfo viewInfo = e.ViewInfo as BorderLineStyleEditPopupImageComboBoxEditListBoxViewInfo;
			if (viewInfo == null || viewInfo.RichEditControl == null)
				return;
			BorderLinePainterHelper.DrawBorderLineItem(e, base.DrawItemText, viewInfo.RichEditControl.DocumentModel.UnitConverter);
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
	public class BorderLineStyleEditPopupImageListBoxItemPainter : PopupImageListBoxItemPainter {
		protected override void DrawItemText(ListBoxItemObjectInfoArgs e) {
			BorderLineStyleEditPopupImageComboBoxEditListBoxViewInfo viewInfo = e.ViewInfo as BorderLineStyleEditPopupImageComboBoxEditListBoxViewInfo;
			if (viewInfo == null || viewInfo.RichEditControl == null)
				return;
			BorderLinePainterHelper.DrawBorderLineItem(e, base.DrawItemText, viewInfo.RichEditControl.DocumentModel.UnitConverter);
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
	public class BorderLineStyleEditPopupImageComboBoxEditListBoxViewInfo : PopupImageComboBoxEditListBoxViewInfo {
		public BorderLineStyleEditPopupImageComboBoxEditListBoxViewInfo(PopupImageComboBoxEditListBox owner)
			: base(owner) {
		}
		public new ListBoxItemObjectInfoArgs ListBoxItemInfoArgs { get { return base.ListBoxItemInfoArgs; } }
		protected internal RichEditControl RichEditControl {
			get {
				if (OwnerControl == null)
					return null;
				BorderLineStyleEdit edit = OwnerControl.OwnerEdit as BorderLineStyleEdit;
				if (edit == null)
					return null;
				return edit.Control;
			}
		}
		protected override BaseListBoxItemPainter CreateItemPainter() {
			if (IsSkinnedHighlightingEnabled)
				return new BorderLineStyleEditPopupImageListBoxSkinItemPainter();
			return new BorderLineStyleEditPopupImageListBoxItemPainter();
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
	public class BorderLineStyleEditPopupImageComboBoxEditListBox : PopupImageComboBoxEditListBox {
		public BorderLineStyleEditPopupImageComboBoxEditListBox(PopupListBoxForm ownerForm)
			: base(ownerForm) {
		}
		protected override DevExpress.XtraEditors.ViewInfo.BaseStyleControlViewInfo CreateViewInfo() {
			return new BorderLineStyleEditPopupImageComboBoxEditListBoxViewInfo(this);
		}
		public new BorderLineStyleEditPopupImageComboBoxEditListBoxViewInfo ViewInfo { get { return (BorderLineStyleEditPopupImageComboBoxEditListBoxViewInfo)base.ViewInfo; } }
	}
	public class BorderLineStyleEditPopupImageComboBoxEditListBoxForm : PopupImageComboBoxEditListBoxForm {
		public BorderLineStyleEditPopupImageComboBoxEditListBoxForm(ComboBoxEdit ownerEdit)
			: base(ownerEdit) {
		}
		protected override PopupListBox CreateListBox() {
			return new BorderLineStyleEditPopupImageComboBoxEditListBox(this);
		}
		public override int CalcMinimumComboWidth() {
			return OwnerEdit.Width;
		}
	}
	public class BorderLineStyleImageComboBoxEditViewInfo : ImageComboBoxEditViewInfo {
		public BorderLineStyleImageComboBoxEditViewInfo(RepositoryItem item)
			: base(item) {
		}
		public RichEditControl RichEditControl {
			get {
				RepositoryItemBorderLineStyle owner = Owner as RepositoryItemBorderLineStyle;
				if (owner != null)
					return owner.Control;
				else
					return null;
			}
		}
	}
}
