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
using System.Diagnostics;
using System.Reflection;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraRichEdit.Design.Internal;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.Office.Drawing;
using DevExpress.Office.Layout;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
namespace DevExpress.XtraRichEdit.Design {
	[
	UserRepositoryItem("RegisterUnderlineStyleEdit"),
	System.Runtime.InteropServices.ComVisible(false)
	]
	public class RepositoryItemUnderlineStyle : RepositoryItemImageComboBox {
		internal static string InternalEditorTypeName { get { return typeof(UnderlineStyleEdit).Name; } }
		static RepositoryItemUnderlineStyle() { RegisterUnderlineStyleEdit(); }
		public static void RegisterUnderlineStyleEdit() {
			Bitmap img = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraRichEdit.Bitmaps256.RichTextEdit.bmp", Assembly.GetExecutingAssembly());
			img.MakeTransparent(Color.Magenta);
			EditorClassInfo editorInfo = new EditorClassInfo(typeof(UnderlineStyleEdit).Name, typeof(UnderlineStyleEdit), typeof(RepositoryItemUnderlineStyle), typeof(DevExpress.XtraEditors.ViewInfo.ImageComboBoxEditViewInfo), new UnderlineStylePainter(), true, img);
			editorInfo.AllowInplaceEditing = ShowInContainerDesigner.OnlyInBars;
			EditorRegistrationInfo.Default.Editors.Add(editorInfo);
		}
		UnderlineRepository underlineRepository;
		public RepositoryItemUnderlineStyle() {
			this.underlineRepository = null;
			InitItems();
		}
		public override string EditorTypeName { get { return InternalEditorTypeName; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImageComboBoxItemCollection Items { get { return base.Items; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public UnderlineRepository UnderlineRepository {
			get { return underlineRepository; }
			set {
				if (underlineRepository == value)
					return;
				underlineRepository = value;
				InitItems();
			}
		}
		protected void InitItems() {
			BeginUpdate();
			try {
				PopulateItems();
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual void PopulateItems() {
			Items.Clear();
			if (UnderlineRepository == null)
				return;
			AddItem(UnderlineType.None);
			AddItem(UnderlineType.Single);
			AddItem(UnderlineType.ThickSingle);
			AddItem(UnderlineType.Double);
			AddItem(UnderlineType.Dotted);
			AddItem(UnderlineType.ThickDotted);
			AddItem(UnderlineType.Dashed);
			AddItem(UnderlineType.ThickDashed);
			AddItem(UnderlineType.LongDashed);
			AddItem(UnderlineType.ThickLongDashed);
			AddItem(UnderlineType.DashDotted);
			AddItem(UnderlineType.ThickDashDotted);
			AddItem(UnderlineType.DashDotDotted);
			AddItem(UnderlineType.ThickDashDotDotted);
			AddItem(UnderlineType.Wave);
			AddItem(UnderlineType.HeavyWave);
			AddItem(UnderlineType.DoubleWave);
		}
		void AddItem(UnderlineType underlineType) {
			Underline underline = UnderlineRepository.GetPatternLineByType(underlineType);
			if (underline != null)
				Items.Add(new ImageComboBoxItem(underline.ToString(), underline, -1));
		}
	}
	[DXToolboxItem(false)]
	public partial class UnderlineStyleEdit : ImageComboBoxEdit {
		static UnderlineStyleEdit() {
			RepositoryItemUnderlineStyle.RegisterUnderlineStyleEdit();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public UnderlineType? UnderlineType {
			get {
				if (IsNullValue(EditValue))
					return null;
				return ((Underline)EditValue).Id;
			}
			set {
				if (value == null) {
					base.EditValue = null;
					return;
				}
				base.EditValue = Properties.UnderlineRepository.GetPatternLineByType(value.Value);
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemUnderlineStyle Properties { get { return base.Properties as RepositoryItemUnderlineStyle; } }
		public override string EditorTypeName { get { return GetType().Name; } }
		protected override PopupBaseForm CreatePopupForm() {
			return new UnderlineStyleEditPopupImageComboBoxEditListBoxForm(this);
		}
	}
}
namespace DevExpress.XtraRichEdit.Design.Internal {
	#region ScreenCharacterLinePainterParameters
	public class ScreenCharacterLinePainterParameters : PatternLinePainterParameters {
		protected internal override float PixelsToUnits(float value, float dpi) {
			return value;
		}
	}
	#endregion
	public class ScreenCharacterLinePainter : RichEditPatternLinePainter {
		readonly static PatternLinePainterParametersTable parametersTable = new PatternLinePainterParametersTable();
		readonly static Graphics pixelGraphics = CreatePixelGraphics();
		static PatternLinePainterParameters parameters;
		public ScreenCharacterLinePainter(Painter painter, DocumentLayoutUnitConverter unitConverter)
			: base(painter, unitConverter) {
		}
		protected override Graphics PixelGraphics { get { return pixelGraphics; } }
		protected override PatternLinePainterParameters Parameters { get { return parameters; } }
		protected override PatternLinePainterParametersTable ParametersTable { get { return parametersTable; } }
		protected internal override void PrepareParameters() {
			if (parameters == null) {
				parameters = new ScreenCharacterLinePainterParameters();
				InitializeParameters(parameters);
			}
		}
		protected override void InitializeParameters(PatternLinePainterParameters parameters) {
			parameters.Initialize(PixelGraphics.DpiX);
		}
		protected override float PixelsToUnits(float val, float dpi) {
			return val;
		}
		protected override float UnitsToPixels(float val, float dpi) {
			return val;
		}
	}
	public class UnderlineStylePainter : ImageComboBoxEditPainter {
		protected override void DrawText(ControlGraphicsInfoArgs info) {
			TextEditViewInfo viewInfo = info.ViewInfo as TextEditViewInfo;
			if (viewInfo == null)
				return;
			Underline value = viewInfo.EditValue as Underline;
			if (value == null)
				return;
			if (value.Id == UnderlineType.None)
				base.DrawText(info);
			else
				UnderlinePainterHelper.DrawUnderlineItem(value, info.Cache, info.Bounds, viewInfo.PaintAppearance);
		}
	}
	public class ValueListBoxItemObjectInfoArgs : ListBoxItemObjectInfoArgs {
		ComboBoxItem itemValue;
		public ValueListBoxItemObjectInfoArgs(BaseListBoxViewInfo viewInfo, GraphicsCache cache, Rectangle bounds)
			: base(viewInfo, cache, bounds) {
		}
		public ComboBoxItem ItemValue { get { return itemValue; } }
		public override void AssignFromItemInfo(BaseListBoxViewInfo.ItemInfo item) {
			base.AssignFromItemInfo(item);
			this.itemValue = item.Item as ComboBoxItem;
		}
	}
	public delegate void DrawItemTextHandler(ListBoxItemObjectInfoArgs e);
	public static class UnderlinePainterHelper {
		const int HorizontalPadding = 4;
		static int UnderlineThickness = 1;
		static int UnderlineBoxHeight = 3;
		public static void DrawUnderlineItem(ListBoxItemObjectInfoArgs e, DrawItemTextHandler handler) {
			ValueListBoxItemObjectInfoArgs args = e as ValueListBoxItemObjectInfoArgs;
			if (args == null)
				return;
			Underline value = args.ItemValue.Value as Underline;
			if (value == null)
				return;
			if (value.Id == UnderlineType.None)
				handler(e);
			else
				DrawUnderlineItem(value, e.Cache, e.Bounds, e.PaintAppearance);
		}
		public static void DrawUnderlineItem(Underline underline, GraphicsCache cache, Rectangle bounds, AppearanceObject appearance) {
			DrawUnderlineItemCore(underline, cache, bounds, appearance.ForeColor, UnderlineThickness, UnderlineBoxHeight);
		}
		internal static void DrawUnderlineItemCore(Underline underline, GraphicsCache cache, Rectangle bounds, Color foreColor, int thickness, int underlineBoxHeight) {
			Graphics graphics = cache.Graphics;
			RectangleF oldClip = graphics.ClipBounds;
			using (GdiPlusPainter painter = new GdiPlusPainter(cache)) {
				Rectangle clipRect = CalcClipBounds(bounds);
				painter.Graphics.SetClip(clipRect);
				Rectangle actualUnderlineBounds = CalculateCenteredUnderlineBounds(underline, bounds, thickness);
				ScreenCharacterLinePainter linePainter = new ScreenCharacterLinePainter(painter, new EmptyDocumentLayoutUnitConverter());
				underline.Draw(linePainter, actualUnderlineBounds, foreColor);
			}
			graphics.SetClip(oldClip);
		}
		static Rectangle CalculateCenteredUnderlineBounds(Underline underline, Rectangle bounds, int thickness) {
			Rectangle rect = bounds;
			rect.Height = thickness;
			Rectangle result = underline.CalcLineBounds(rect, thickness);
			int dy = (bounds.Y - result.Y) + (bounds.Height - result.Height) / 2;
			result.Y += dy;
			return result;
		}
		internal static Rectangle CalcClipBounds(Rectangle bounds) {
			return Rectangle.FromLTRB(bounds.Left + HorizontalPadding, bounds.Top, bounds.Right - HorizontalPadding, bounds.Bottom);
		}
	}
	public class UnderlineStyleEditPopupImageListBoxSkinItemPainter : PopupImageListBoxSkinItemPainter {
		protected override void DrawItemText(ListBoxItemObjectInfoArgs e) {
			UnderlinePainterHelper.DrawUnderlineItem(e, base.DrawItemText);
		}
	}
	public class UnderlineStyleEditPopupImageListBoxItemPainter : PopupImageListBoxItemPainter {
		protected override void DrawItemText(ListBoxItemObjectInfoArgs e) {
			UnderlinePainterHelper.DrawUnderlineItem(e, base.DrawItemText);
		}
	}
	public class UnderlineStyleEditPopupImageComboBoxEditListBoxViewInfo : PopupImageComboBoxEditListBoxViewInfo {
		public UnderlineStyleEditPopupImageComboBoxEditListBoxViewInfo(PopupImageComboBoxEditListBox owner)
			: base(owner) {
		}
		public new ListBoxItemObjectInfoArgs ListBoxItemInfoArgs { get { return base.ListBoxItemInfoArgs; } }
		protected override BaseListBoxItemPainter CreateItemPainter() {
			if (IsSkinnedHighlightingEnabled)
				return new UnderlineStyleEditPopupImageListBoxSkinItemPainter();
			return new UnderlineStyleEditPopupImageListBoxItemPainter();
		}
		protected override ListBoxItemObjectInfoArgs CreateListBoxItemInfoArgs() {
			return new ValueListBoxItemObjectInfoArgs(this, null, Rectangle.Empty);
		}
	}
	public class UnderlineStyleEditPopupImageComboBoxEditListBox : PopupImageComboBoxEditListBox {
		public UnderlineStyleEditPopupImageComboBoxEditListBox(PopupListBoxForm ownerForm)
			: base(ownerForm) {
		}
		protected override DevExpress.XtraEditors.ViewInfo.BaseStyleControlViewInfo CreateViewInfo() {
			return new UnderlineStyleEditPopupImageComboBoxEditListBoxViewInfo(this);
		}
		public new UnderlineStyleEditPopupImageComboBoxEditListBoxViewInfo ViewInfo { get { return (UnderlineStyleEditPopupImageComboBoxEditListBoxViewInfo)base.ViewInfo; } }
	}
	public class UnderlineStyleEditPopupImageComboBoxEditListBoxForm : PopupImageComboBoxEditListBoxForm {
		public UnderlineStyleEditPopupImageComboBoxEditListBoxForm(ComboBoxEdit ownerEdit)
			: base(ownerEdit) {
		}
		protected override PopupListBox CreateListBox() {
			return new UnderlineStyleEditPopupImageComboBoxEditListBox(this);
		}
		public override int CalcMinimumComboWidth() {
			return OwnerEdit.Width;
		}
	}
}
