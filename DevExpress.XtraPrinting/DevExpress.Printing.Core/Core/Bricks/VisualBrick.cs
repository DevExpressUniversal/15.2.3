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

using DevExpress.XtraPrinting.Native;
using System;
using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using System.IO;
using System.Collections.Generic;
using DevExpress.Utils.StoredObjects;
#if SL
using System.Windows.Media;
using DevExpress.Xpf.Drawing;
using DevExpress.Xpf.Windows.Forms;
using DevExpress.Xpf.Drawing.Drawing2D;
#else
using DevExpress.XtraPrinting.BrickExporters;
using System.Drawing.Drawing2D;
#endif
namespace DevExpress.XtraPrinting {
#if DEBUGTEST
	[System.Diagnostics.DebuggerDisplay(@"\{{GetType().FullName,nq}, Text = {Text}, Rect = {Rect}}")]
#endif
#if !SL
	[BrickExporter(typeof(VisualBrickExporter))]
#endif 
	public class VisualBrick : Brick, IVisualBrick, IXtraPartlyDeserializable {
		#region static
		protected static bool ToBoolean(DevExpress.Utils.DefaultBoolean value, bool defaultValue) {
			return value == DevExpress.Utils.DefaultBoolean.True ? true :
				value == DevExpress.Utils.DefaultBoolean.False ? false :
				defaultValue;
		}
		internal static BrickStyle GetAreaStyle(StylesContainer styles, BrickStyle style, RectangleF area, RectangleF baseBounds) {
			BrickStyle brickSyle = BrickStyleHelper.ChangeSides(style, GetAreaBorderSides(style.Sides, area, baseBounds));
			return styles.GetStyle(brickSyle);
		}
		protected static BorderSide GetAreaBorderSides(BorderSide sides, RectangleF area, RectangleF baseBounds) {
			if(area.Left != baseBounds.Left)
				sides &= ~BorderSide.Left;
			if(area.Top != baseBounds.Top)
				sides &= ~BorderSide.Top;
			if(area.Right != baseBounds.Right)
				sides &= ~BorderSide.Right;
			if(area.Bottom != baseBounds.Bottom)
				sides &= ~BorderSide.Bottom;
			return sides;
		}
		#endregion
		BrickStyle fStyle;
		IBrickOwner brickOwner = NullBrickOwner.Instance;
		[XtraSerializableProperty]
		[DefaultValue(false)]
		public bool UseTextAsDefaultHint {
			get {
				return flags[bitUseTextAsDefaultHint];
			}
			set {
				flags[bitUseTextAsDefaultHint] = value;
			}
		}
		public override string Hint {
			get {
				int index = GetDataIndex(BrickAttachedProperties.Hint.Index);
				return index >= 0 ? GetDataValue<string>(index) :
					UseTextAsDefaultHint ? Text :
					string.Empty;
			}
			set {
				base.Hint = value;
			}
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("VisualBrickAnchorName"),
#endif
		XtraSerializableProperty,
		DefaultValue(""),
		]
		public string AnchorName {
			get { return GetValue(BrickAttachedProperties.AnchorName, string.Empty); }
			set { SetAttachedValue(BrickAttachedProperties.AnchorName, value ?? string.Empty, string.Empty); }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("VisualBrickTarget"),
#endif
		XtraSerializableProperty,
		DefaultValue(""),
		]
		public string Target {
			get { return GetValue(BrickAttachedProperties.Target, string.Empty); }
			set { SetAttachedValue(BrickAttachedProperties.Target, value ?? string.Empty, string.Empty); }
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("VisualBrickNavigationPair")]
#endif
		public BrickPagePair NavigationPair {
			get { return GetValue(BrickAttachedProperties.NavigationPair, BrickPagePair.Empty); }
			set { SetAttachedValue(BrickAttachedProperties.NavigationPair, value, BrickPagePair.Empty); }
		}
		#region serialization
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("VisualBrickNavigationPageIndex"),
#endif
		DefaultValue(BrickPagePair.UndefinedPageIndex),
		XtraSerializableProperty
		]
		public int NavigationPageIndex { get { return NavigationPair.PageIndex; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("VisualBrickNavigationBrickIndices"),
#endif
		DefaultValue(""),
		XtraSerializableProperty,
		]
		public string NavigationBrickIndices { get { return NavigationPair.Indices; } }
		#endregion
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("VisualBrickBookmarkInfo")]
#endif
		public BookmarkInfo BookmarkInfo {
			get { return GetValue(BrickAttachedProperties.BookmarkInfo, BrickOwner.EmptyBookmarkInfo); }
			set { SetAttachedValue(BrickAttachedProperties.BookmarkInfo, value, BrickOwner.EmptyBookmarkInfo); }
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("VisualBrickBrickOwner")]
#endif
		public IBrickOwner BrickOwner { get { return brickOwner; } protected set { brickOwner = value; } }
		internal bool HasCrossReference { get { return !string.IsNullOrEmpty(GetActualUrl()) && Target == "_self"; } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("VisualBrickSeparableHorz")]
#endif
		public override bool SeparableHorz { get { return BrickOwner.IsSeparableHorz(base.SeparableHorz); } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("VisualBrickSeparableVert")]
#endif
		public override bool SeparableVert { get { return BrickOwner.IsSeparableVert(base.SeparableVert); } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("VisualBrickPrintingSystem")]
#endif
		public override PrintingSystemBase PrintingSystem { get { return (PrintingSystemBase)BrickPSHelper.GetPS(Style); } set { BrickPSHelper.SetPS(Style, value); } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("VisualBrickTextValue")]
#endif
		public virtual object TextValue { get { return null; } set { } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("VisualBrickTextValueFormatString")]
#endif
		public virtual string TextValueFormatString { get { return null; } set { } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("VisualBrickXlsxFormatString")]
#endif
		public virtual string XlsxFormatString { get { return null; } set { } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("VisualBrickStyle"),
#endif
		XtraSerializableProperty(XtraSerializationVisibility.Content, true, false, false, 0, XtraSerializationFlags.Cached),
		]
		public BrickStyle Style {
			get {
				return fStyle; 
			}
			set {
				if(value == null)
					throw new ArgumentNullException("style");
				BrickPSHelper.SetPS(value, PrintingSystem);
				if(PrintingSystem == null)
					fStyle = value;
				else
					fStyle = PrintingSystem.Styles.GetStyle(value);
			}
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("VisualBrickSides")]
#endif
		public BorderSide Sides {
			get { return Style.Sides; }
			set {
				if(Sides != value)
					Style = BrickStyleHelper.ChangeSides(Style, value);
			}
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("VisualBrickBorderWidth")]
#endif
		public float BorderWidth {
			get { return Style.BorderWidth; }
			set {
				if(BorderWidth != value)
					Style = BrickStyleHelper.ChangeBorderWidth(Style, value);
			}
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("VisualBrickBorderDashStyle")]
#endif
		public BorderDashStyle BorderDashStyle {
			get { return Style.BorderDashStyle; }
			set {
				if(BorderDashStyle != value)
					Style = BrickStyleHelper.ChangeBorderDashStyle(Style, value);
			}
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("VisualBrickBorderColor")]
#endif
		public Color BorderColor {
			get { return Style.BorderColor; }
			set {
				if(BorderColor != value)
					Style = BrickStyleHelper.ChangeBorderColor(Style, value);
			}
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("VisualBrickBackColor")]
#endif
		public Color BackColor {
			get { return Style.BackColor; }
			set {
				if(BackColor != value)
					Style = BrickStyleHelper.ChangeBackColor(Style, value);
			}
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("VisualBrickPadding")]
#endif
		public PaddingInfo Padding {
			get { return Style.Padding; }
			set {
				if(Padding != value)
					Style = BrickStyleHelper.ChangePadding(Style, value);
			}
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("VisualBrickBorderStyle")]
#endif
		public BrickBorderStyle BorderStyle {
			get { return Style.BorderStyle; }
			set {
				if(BorderStyle != value)
					Style = BrickStyleHelper.ChangeBorderStyle(Style, value);
			}
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("VisualBrickText")]
#endif
		public virtual string Text { get { return String.Empty; } set { } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("VisualBrickBrickType")]
#endif
		public override string BrickType { get { return BrickTypes.Visual; } }
		internal bool CanGrow { get { return flags[bitCanGrow]; } set { flags[bitCanGrow] = value; } }
		internal bool CanShrink { get { return flags[bitCanShrink]; } set { flags[bitCanShrink] = value; } }
		internal bool NeedAdjust { get { return CanGrow || CanShrink; } }
		public VisualBrick(BrickStyle style)
			: this(style, NullBrickOwner.Instance) {
		}
		public VisualBrick()
			: this(new BrickStyle(BrickGraphics.InternalBrickStyle)) {
		}
		public VisualBrick(BorderSide sides, float borderWidth, Color borderColor, Color backColor)
			: this(new BrickStyle(sides, borderWidth, borderColor, backColor, BrickGraphics.InternalBrickStyle.ForeColor, BrickGraphics.InternalBrickStyle.Font, BrickGraphics.InternalBrickStyle.StringFormat)) {
		}
		public VisualBrick(IBrickOwner brickOwner)
			: this(new BrickStyle(BrickGraphics.InternalBrickStyle), brickOwner) {
		}
		internal VisualBrick(VisualBrick brick)
			: base(brick) {
			fStyle = brick.Style;
			brickOwner = brick.brickOwner;
			CanGrow = brick.CanGrow;
			CanShrink = brick.CanShrink;
		}
		VisualBrick(BrickStyle style, IBrickOwner brickOwner) {
			fStyle = style;
			this.brickOwner = brickOwner;
			CanGrow = false;
			CanShrink = false;
		}
		public float GetScaleFactor(IPrintingSystemContext context) {
			if(HasModifier(BrickModifier.MarginalHeader, BrickModifier.MarginalFooter))
				return 1;
			return context.DrawingPage != null ? context.DrawingPage.ScaleFactor : context.PrintingSystem.Document.ScaleFactor;
		}
		public virtual RectangleF GetClientRectangle(RectangleF rect, float dpi) {
			return Style.DeflateBorderWidth(rect, dpi);
		}
		protected override void StoreValues(BinaryWriter writer, IRepositoryProvider provider) {
			base.StoreValues(writer, provider);
			writer.Write(provider.StoreObject<BrickStyle>(fStyle));
			writer.Write(provider.StoreObject<IBrickOwner>(brickOwner));
		}
		protected override void RestoreValues(BinaryReader reader, IRepositoryProvider provider) {
			base.RestoreValues(reader, provider);
			fStyle = provider.RestoreObject<BrickStyle>(reader.ReadInt64(), null);
			brickOwner = provider.RestoreObject<IBrickOwner>(reader.ReadInt64(), null);
		}
		protected internal override void Scale(double scaleFactor) {
			base.Scale(scaleFactor);
			Style = Style.Scale((float)scaleFactor);
		}
		protected internal override bool AfterPrintOnPage(IList<int> indices, int pageIndex, int pageCount, Action<BrickBase> callback) {
			if(BrickOwner.RaiseAfterPrintOnPage(this, pageIndex, pageCount)) {
				base.AfterPrintOnPage(indices, pageIndex, pageCount, callback);
				return true;
			}
			return false;
		}
		protected override void OnSetPrintingSystem(bool cacheStyle) {
			base.OnSetPrintingSystem(cacheStyle);
			if(fStyle != null && cacheStyle)
				fStyle = PrintingSystem.Styles.GetStyle(fStyle);
		}
		public override float ValidatePageRight(float pageRight, RectangleF rect) {
			return (pageRight < rect.Left || pageRight > rect.Right) ? pageRight :
				!SeparableHorz ? rect.Left : ValidatePageRightInternal(pageRight, rect);
		}
		protected virtual float ValidatePageRightInternal(float pageRight, RectangleF rect) {
			return rect.Left;
		}
		internal void SetBoundsWidth(float width, float dpi) {
			base.Width = GraphicsUnitConverter.Convert(width, dpi, GraphicsDpi.Document);
		}
		internal void SetBoundsHeight(float height, float dpi) {
			base.Height = GraphicsUnitConverter.Convert(height, dpi, GraphicsDpi.Document);
		}
		internal void SetBoundsY(float y, float dpi) {
			base.Y = GraphicsUnitConverter.Convert(y, dpi, GraphicsDpi.Document);
		}
		internal RectangleF GetBounds(float dpi) {
			return GraphicsUnitConverter.Convert(InitialRect, GraphicsDpi.Document, dpi);
		}
		void IXtraPartlyDeserializable.Deserialize(object rootObject, IXtraPropertyCollection properties) {
			NavigationPair = ((Document)rootObject).CreateBrickPagePair(properties["NavigationPageIndex"], properties["NavigationBrickIndices"]);
		}
		protected override object CreateContentPropertyValue(XtraItemEventArgs e) {
			if(e.Item.Name == PrintingSystemSerializationNames.Style)
				return BrickFactory.CreateBrickStyle(e);
			return base.CreateContentPropertyValue(e);
		}
		public virtual DevExpress.XtraPrinting.Native.LayoutAdjustment.ILayoutData CreateLayoutData(float dpi) {
			return new DevExpress.XtraPrinting.Native.LayoutAdjustment.VisualBrickLayoutData(this, dpi);
		}
		public override object Clone() {
			return new VisualBrick(this);
		}
		protected override bool ShouldSerializeCore(string propertyName) {
			if(propertyName == PrintingSystemSerializationNames.UseTextAsDefaultHint)
				return UseTextAsDefaultHint != false;
			if(propertyName == PrintingSystemSerializationNames.Hint && UseTextAsDefaultHint == true)
				return GetDataIndex(BrickAttachedProperties.Hint.Index) >= 0;
			return base.ShouldSerializeCore(propertyName);
		}
		internal static float[] GetDashPattern(DashStyle style) {
			switch(style) {
				case DashStyle.Dash:
					return new float[] { 3, 3 };
				case DashStyle.DashDot:
					return new float[] { 5, 3, 1, 3 };
				case DashStyle.DashDotDot:
					return new float[] { 5, 3, 1, 3, 1, 3 };
				case DashStyle.Dot:
					return new float[] { 1, 2 };
			}
			return new float[] { };
		}
		internal static float[] GetDashPattern(BorderDashStyle dashStyle) {
			return GetDashPattern(ConvertDashStyle(dashStyle));
		}
		internal static DashStyle ConvertDashStyle(BorderDashStyle dashStyle) {
			switch(dashStyle) {
				case BorderDashStyle.Dash:
					return DashStyle.Dash;
				case BorderDashStyle.Dot:
					return DashStyle.Dot;
				case BorderDashStyle.DashDot:
					return DashStyle.DashDot;
				case BorderDashStyle.DashDotDot:
					return DashStyle.DashDotDot;
				default:
					return DashStyle.Solid;
			}
		}
	}
}
namespace DevExpress.XtraPrinting.Native {
	public static class VisualBrickHelper {
#if !SL
		public static void DrawBrick(VisualBrick brick, IGraphics gr, RectangleF rect, RectangleF parentRect) {
			BrickBaseExporter.GetExporter(gr, brick).Draw(gr, rect, parentRect);
		}
#endif
		public static float GetTabInterval(Font font, StringFormat sf, GraphicsUnit unit, Measurer measurer) {
			return measurer.MeasureString("Q", font, 0, sf, unit).Width * 8.0f;
		}
		public static void InitializeBrick(Brick brick, PrintingSystemBase ps, RectangleF rect) {
			brick.Initialize(ps, rect);
		}
		public static RectangleF GetBrickInitialRect(Brick brick) {
			return brick.InitialRect;
		}
		public static void SetBrickInitialRect(Brick brick, RectangleF value) {
			brick.InitialRect = value;
		}
		public static void SetBrickBoundsHeight(VisualBrick brick, float height, float dpi) {
			brick.SetBoundsHeight(height, dpi);
		}
		public static void SetBrickBoundsWidth(VisualBrick brick, float width, float dpi) {
			brick.SetBoundsWidth(width, dpi);
		}
		public static void SetBrickBoundsY(VisualBrick brick, float y, float dpi) {
			brick.SetBoundsY(y, dpi);
		}
		public static RectangleF GetBrickBounds(VisualBrick brick, float dpi) {
			return brick.GetBounds(dpi);
		}
		public static void SetBrickBounds(VisualBrick brick, RectangleF bounds, float dpi) {
			brick.SetBounds(bounds, dpi);
		}
		public static bool GetCanGrow(VisualBrick brick) {
			return brick.CanGrow;
		}
		public static void SetCanGrow(VisualBrick brick, bool value) {
			brick.CanGrow = value;
		}
		public static bool GetCanShrink(VisualBrick brick) {
			return brick.CanShrink;
		}
		public static void SetCanShrink(VisualBrick brick, bool value) {
			brick.CanShrink = value;
		}
		public static bool GetCanOverflow(VisualBrick brick) {
			return brick.CanOverflow;
		}
		public static void SetCanOverflow(VisualBrick brick, bool value) {
			brick.CanOverflow = value;
		}
	}
}
