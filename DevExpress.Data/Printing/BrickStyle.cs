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

using System.Drawing;
using DevExpress.XtraPrinting.Native;
using System.ComponentModel;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Design;
using DevExpress.Data.Utils;
using DevExpress.Data;
using System;
using System.Reflection;
using DevExpress.Utils;
using System.Collections;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Utils.StoredObjects;
#if SL
using DevExpress.Xpf.Drawing;
using DevExpress.Xpf.Collections;
using System.Windows.Media;
#else
#endif
#if DXRESTRICTED
using DevExpress.Xpf.Collections;
using DevExpress.Compatibility.System;
using DevExpress.Compatibility.System.ComponentModel;
#endif
namespace DevExpress.XtraPrinting {
#if !SL && !DXPORTABLE
	[TypeConverter(typeof(EnumTypeConverter))]
#endif
	[ResourceFinder(typeof(ResFinder))]
	public enum TextAlignment {
		TopLeft = 1,
		TopCenter = 2,
		TopRight = 4,
		MiddleLeft = 16,
		MiddleCenter = 32,
		MiddleRight = 64,
		BottomLeft = 256,
		BottomCenter = 512,
		BottomRight = 1024,
		TopJustify = 2048,
		MiddleJustify = 4096,
		BottomJustify = 8096
	}
#if !DXPORTABLE
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1028")]
#endif
	public enum BrickBorderStyle : byte { 
		Inset = 1, 
		Outset = 2, 
		Center = 4 
	}
	public enum ExportTarget { Xls, Xlsx, Html, Mht, Pdf, Text, Rtf, Csv, Image }
#if !SL && !DXPORTABLE
	[TypeConverter(typeof(EnumTypeConverter))]
#endif
	[ResourceFinder(typeof(ResFinder))]
#if !DXPORTABLE
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1028")]
#endif
	public enum PageInfo : byte {
		None = 0,
		Number = 1,
		NumberOfTotal = 2,
		RomLowNumber = 4,
		RomHiNumber = 8,
		DateTime = 16,
		UserName = 32,
		Total = 64
	}
#if !SL && !DXPORTABLE
	[TypeConverter(typeof(DevExpress.XtraPrinting.Design.BordersConverter))]
#endif
	[Flags,	Serializable]
	[ResourceFinder(typeof(ResFinder))]
	public enum BorderSide {
		None = 0,
		Left = 1,
		Top = 2,
		Right = 4,
		Bottom = 8,
		All = 15 
	};
	[Flags]
	public enum StyleProperty {
		None = 0,
		BackColor = 1 << 0,
		ForeColor = 1 << 1,
		BorderColor = 1 << 2,
		Font = 1 << 3,
		BorderDashStyle = 1 << 4,
		Borders = 1 << 5,
		BorderWidth = 1 << 6,
		TextAlignment = 1 << 7,
		Padding = 1 << 8,
		All = BackColor | ForeColor | BorderColor | Font | BorderDashStyle | Borders | BorderWidth | TextAlignment | Padding,
	}
	public class BrickStyle : ICloneable, IDisposable, IStoredObject
	{
#region static
		static float AdjustBorderWidthToInflate(float borderWidth, BrickBorderStyle borderStyle) {
			return borderStyle == BrickBorderStyle.Inset ? 0f :
				borderStyle == BrickBorderStyle.Center ? borderWidth / 2f :
				borderWidth;
		}
		static float AdjustBorderWidthToDeflate(float borderWidth, BrickBorderStyle borderStyle) {
			return borderStyle == BrickBorderStyle.Inset ? borderWidth :
				borderStyle == BrickBorderStyle.Center ? borderWidth / 2f :
				0;
		}
		public static RectangleF DeflateBorderWidth(RectangleF rect, BorderSide sides, float borderWidth, BrickBorderStyle borderStyle) {
			return DeflateBorderWidth(rect, sides, AdjustBorderWidthToDeflate(borderWidth, borderStyle));
		}
		public static RectangleF DeflateBorderWidth(RectangleF rect, BorderSide sides, float borderWidth) {
			return InflateBorderWidth(rect, sides, -borderWidth);
		}
		public static RectangleF InflateBorderWidth(RectangleF rect, BorderSide sides, float borderWidth, BrickBorderStyle borderStyle) {
			return InflateBorderWidth(rect, sides, AdjustBorderWidthToInflate(borderWidth, borderStyle));
		}
		public static RectangleF InflateBorderWidth(RectangleF rect, BorderSide sides, float borderWidth) {
			if(borderWidth == 0f)
				return rect;
			if((sides & DevExpress.XtraPrinting.BorderSide.Left) > 0) {
				rect.X -= borderWidth;
				rect.Width += borderWidth;
			}
			if((sides & DevExpress.XtraPrinting.BorderSide.Top) > 0) {
				rect.Y -= borderWidth;
				rect.Height += borderWidth;
			}
			if((sides & DevExpress.XtraPrinting.BorderSide.Right) > 0) {
				rect.Width += borderWidth;
			}
			if((sides & DevExpress.XtraPrinting.BorderSide.Bottom) > 0) {
				rect.Height += borderWidth;
			}
			if(rect.Width < 0) rect.Width = 0;
			if(rect.Height < 0) rect.Height = 0;
			return rect;
		}
		static bool IsValidFont(Font font) {
			IntPtr nativeFont = (IntPtr)typeof(Font).GetField("nativeFont", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(font);
			return nativeFont != IntPtr.Zero;
		}
		static Font fDefaultFont;
#if !SL
	[DevExpressDataLocalizedDescription("BrickStyleDefaultFont")]
#endif
		public static Font DefaultFont {
			get { return fDefaultFont; }
		}
		static BrickStyle() {
#if SL
			fDefaultFont = new Font("GenericSerif", 9.75f);
#else
			fDefaultFont = new Font("Times New Roman", 9.75f);
#endif
		}
		[
#if !SL
	DevExpressDataLocalizedDescription("BrickStyleDefault"),
#endif
Obsolete("This property is now obsolete. You should use the CreateDefault method instead.")]
		public static BrickStyle Default {
			get { return CreateDefault(); }
		}
		public static BrickStyle CreateDefault() {
			return new BrickStyle(BorderSide.Left | BorderSide.Top | BorderSide.Right | BorderSide.Bottom, 1, DXColor.Black, DXColor.White, DXColor.Black, DefaultFont, new BrickStringFormat());
		}
		static BrickBorderStyle defaultBorderStyle = BrickBorderStyle.Center;
#endregion
		Color fBackColor;
		Color fBorderColor;
		BorderSide fSides;
		float fBorderWidth;
		BorderDashStyle fBorderDashStyle;
		Color fForeColor;
		Font fFont;
		PaddingInfo fPadding;
		TextAlignment fTextAlignment;
		float fTabInterval = float.NaN;
		StyleProperty setProperties;
		BrickStringFormat fStringFormat;
		BrickBorderStyle fBorderStyle = defaultBorderStyle;
		IPrintingSystem printingSystem;
		Font fontInPoints;
		bool disposed;
		public BrickStyle Scale(float ratio) {
			BrickStyle scaledStyle = (BrickStyle)Clone();
			scaledStyle.Font = new Font(Font.FontFamily, Font.Size * ratio, Font.Style, Font.Unit, Font.GdiCharSet, Font.GdiVerticalFont);
			scaledStyle.BorderWidth *= ratio;
			scaledStyle.Padding = Padding.Scale(ratio);
			ResetTabInterval();
			return scaledStyle;
		}
#region properties
		internal StyleProperty SetProperties { get { return setProperties; } }
		internal IPrintingSystem PrintingSystem { get { return printingSystem; } set { printingSystem = value; } }
		internal float[] CalculateTabStops(IMeasurer measurer) {
			if(!IsSetTabInterval)
				fTabInterval = measurer.MeasureString("Q", Font, 0, StringFormat.Value, GraphicsUnit.Document).Width * 8.0f;
			return new float[] { (float)fTabInterval };
		}
		internal Font FontInPoints {
			get {
				if(Font.Unit == GraphicsUnit.Point)
					return Font;
				if(fontInPoints == null)
					fontInPoints = CreateFontInPoints(Font);
				return fontInPoints;
			}
		}
		internal bool IsDisposed { get { return disposed; } }
		[
#if !SL
	DevExpressDataLocalizedDescription("BrickStyleBorderStyle"),
#endif
		XtraSerializableProperty
		]
		public virtual BrickBorderStyle BorderStyle {
			get { return fBorderStyle; }
			set { fBorderStyle = value; }
		}
		[
#if !SL
	DevExpressDataLocalizedDescription("BrickStylePadding"),
#endif
		XtraSerializableProperty
		]
		public virtual PaddingInfo Padding {
			get { return IsSetPadding ? fPadding : GetInitialPadding(); }
			set { SetPadding(value); }
		}
		[XtraSerializableProperty(XtraSerializationFlags.SuppressDefaultValue), 
#if !SL
	DevExpressDataLocalizedDescription("BrickStyleTextAlignment")
#else
	Description("")
#endif
]
		public virtual TextAlignment TextAlignment {
			get { return IsSetTextAlignment ? fTextAlignment : GetInitialTextAlignment(); }
			set { SetTextAlignment(value); }
		}
		[XtraSerializableProperty, 
#if !SL
	DevExpressDataLocalizedDescription("BrickStyleSides")
#else
	Description("")
#endif
]
		public virtual BorderSide Sides {
			get { return IsSetBorders ? fSides : GetInitialBorders(); }
			set { SetBorders(value); }
		}
#if !SL
	[DevExpressDataLocalizedDescription("BrickStyleBorderWidth")]
#endif
		public virtual float BorderWidth {
			get { return IsSetBorderWidth ? fBorderWidth : GetInitialBorderWidth(); }
			set { SetBorderWidth(value); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public float BorderWidthSerializable { get { return BorderWidth; } set { BorderWidth = value; } }
		[XtraSerializableProperty(XtraSerializationFlags.SuppressDefaultValue), 
#if !SL
	DevExpressDataLocalizedDescription("BrickStyleBorderColor")
#else
	Description("")
#endif
]
		public virtual Color BorderColor {
			get { return IsSetBorderColor ? fBorderColor : GetInitialBorderColor(); }
			set { SetBorderColor(value); }
		}
		[XtraSerializableProperty(XtraSerializationFlags.SuppressDefaultValue), 
#if !SL
	DevExpressDataLocalizedDescription("BrickStyleBackColor")
#else
	Description("")
#endif
]
		public virtual Color BackColor {
			get { return IsSetBackColor ? fBackColor : GetInitialBackColor(); }
			set { SetBackColor(value); }
		}
		[XtraSerializableProperty(XtraSerializationFlags.SuppressDefaultValue), 
#if !SL
	DevExpressDataLocalizedDescription("BrickStyleForeColor")
#else
	Description("")
#endif
]
		public virtual Color ForeColor {
			get { return IsSetForeColor ? fForeColor : GetInitialForeColor(); }
			set { SetForeColor(value); }
		}
		[XtraSerializableProperty, 
#if !SL
	DevExpressDataLocalizedDescription("BrickStyleFont")
#else
	Description("")
#endif
]
		public virtual Font Font {
			get { return IsSetFont ? fFont : GetInitialFont(); }
			set {
				if(value == null)
					ResetFont();
				else
					SetFont(value);
				InvalidateFontInPoints();
			}
		}
		[XtraSerializableProperty, 
#if !SL
	DevExpressDataLocalizedDescription("BrickStyleStringFormat")
#else
	Description("")
#endif
]
		public virtual BrickStringFormat StringFormat {
			get {
				if(fStringFormat == null)
					fStringFormat = new BrickStringFormat();
				return fStringFormat;
			}
			set {
				if(fStringFormat != value) {
					if(fStringFormat != null)
						fStringFormat.Dispose();
					SetStringFormat(value);
				}
			}
		}
		[XtraSerializableProperty, 
#if !SL
	DevExpressDataLocalizedDescription("BrickStyleBorderDashStyle")
#else
	Description("")
#endif
]
		public virtual BorderDashStyle BorderDashStyle {
			get { return IsSetBorderDashStyle ? (BorderDashStyle)fBorderDashStyle : GetInitialBorderDashStyle(); ; }
			set { SetBorderDashStyle(value); }
		}
		[Browsable(false)]
		public bool IsTransparent { 
			get { 
				return BackColor.A == 0 || BorderColor.A == 0; 
			} 
		}
		[Browsable(false)]
		public bool IsValid {
			get {
				return IsValidFont(this.Font);
			}
		}
		[Browsable(false)]
		public bool IsJustified {
			get {
				return TextAlignment >= TextAlignment.TopJustify;
			}
		}
#endregion
		public BrickStyle() {
		}
		void Initialize(BorderSide sides, float borderWidth, Color borderColor, BorderDashStyle borderDashStyle, BrickBorderStyle borderStyle,
			Color backColor, Color foreColor, Font font, BrickStringFormat sf, TextAlignment textAlignment, PaddingInfo padding) {
			SetBackColor(backColor);
			SetBorders(sides);
			SetBorderWidth(borderWidth);
			SetBorderColor(borderColor);
			SetBorderDashStyle(borderDashStyle);
			fBorderStyle = borderStyle;
			SetForeColor(foreColor);
			SetFont(font);
			SetStringFormat(sf);
			SetTextAlignment(textAlignment);
			SetPadding(padding);
		}
		public BrickStyle(float dpi) {
			SetPadding(new PaddingInfo(dpi));
		}
		public BrickStyle(BorderSide sides, float borderWidth, Color borderColor, Color backColor, Color foreColor, Font font, BrickStringFormat sf) {
			Initialize(sides, borderWidth, borderColor, default(BorderDashStyle), defaultBorderStyle, backColor, foreColor, font, sf, default(TextAlignment), default(PaddingInfo));
			ResetBorderDashStyle();
			ResetTextAlignment();
			ResetPadding();
		}
		public BrickStyle(BrickStyle src) {
			Initialize(src.Sides, src.BorderWidth, src.BorderColor, src.BorderDashStyle, src.BorderStyle, src.BackColor, src.ForeColor, src.Font, src.StringFormat, src.TextAlignment, src.Padding);
		}
		internal void CopyProperties(BrickStyle dest, StyleProperty properties) {
			if((properties & StyleProperty.BackColor) > 0)
				dest.SetBackColor(BackColor);
			if((properties & StyleProperty.BorderColor) > 0)
				dest.SetBorderColor(BorderColor);
			if((properties & StyleProperty.BorderDashStyle) > 0)
				dest.SetBorderDashStyle(BorderDashStyle);
			if((properties & StyleProperty.Borders) > 0)
				dest.SetBorders(Sides);
			if((properties & StyleProperty.BorderWidth) > 0)
				dest.SetBorderWidth(BorderWidth);
			if((properties & StyleProperty.Font) > 0)
				dest.SetFont(Font);
			if((properties & StyleProperty.ForeColor) > 0)
				dest.SetForeColor(ForeColor);
			if((properties & StyleProperty.Padding) > 0)
				dest.SetPadding(Padding);
			if((properties & StyleProperty.TextAlignment) > 0)
				dest.SetTextAlignment(TextAlignment);
		}
		internal void SetValue(StyleProperty property, object value) {
			switch(property) {
				case StyleProperty.BackColor:
					SetBackColor((Color)value);
					break;
				case StyleProperty.BorderColor:
					SetBorderColor((Color)value);
					break;
				case StyleProperty.BorderDashStyle:
					SetBorderDashStyle((BorderDashStyle)value);
					break;
				case StyleProperty.Borders:
					SetBorders((BorderSide)value);
					break;
				case StyleProperty.BorderWidth:
					SetBorderWidth((float)value);
					break;
				case StyleProperty.Font:
					SetFont((Font)value);
					break;
				case StyleProperty.ForeColor:
					SetForeColor((Color)value);
					break;
				case StyleProperty.Padding:
					SetPadding((PaddingInfo)value);
					break;
				case StyleProperty.TextAlignment:
					SetTextAlignment((TextAlignment)value);
					break;
				default:
					throw new ArgumentException("properties");
			}
		}
		internal object GetValue(StyleProperty property) {
			switch(property) {
				case StyleProperty.BackColor:
					return BackColor;
				case StyleProperty.BorderColor:
					return BorderColor;
				case StyleProperty.BorderDashStyle:
					return BorderDashStyle;
				case StyleProperty.Borders:
					return Sides;
				case StyleProperty.BorderWidth:
					return BorderWidth;
				case StyleProperty.Font:
					return Font;
				case StyleProperty.ForeColor:
					return ForeColor;
				case StyleProperty.Padding:
					return Padding;
				case StyleProperty.TextAlignment:
					return TextAlignment;
				default:
					throw new ArgumentException("properties");
			}
		}
		internal void Reset(StyleProperty property) {
			FlagReset(property);
		}
		internal static bool PropertyIsSet(BrickStyle style, StyleProperty property) {
			return style != null && style.IsSet(property);
		}
		internal bool IsSet(StyleProperty property) {
			return (SetProperties & property) > 0;
		}
		public virtual object Clone() {
			BrickStyle clone = CreateClone();
			clone.Initialize(fSides, fBorderWidth, fBorderColor, fBorderDashStyle, fBorderStyle, fBackColor, fForeColor, fFont, fStringFormat, fTextAlignment, fPadding);
			clone.setProperties = setProperties;
			return clone;
		}
		protected virtual BrickStyle CreateClone() {
			return new BrickStyle();
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		internal int GetHashCodeInternal() {
			return HashCodeHelper.CalcHashCode(
				(int)Sides,
				(int)BorderWidth,
				BorderColor.GetHashCode(),
				BackColor.GetHashCode(),
				ForeColor.GetHashCode(),
				(int)TextAlignment,
				Font.GetHashCode(),
				StringFormat.GetHashCode(),
				Padding.GetHashCode(),
				(int)BorderDashStyle,
				(int)fBorderStyle
				);
		}
		public override bool Equals(object obj) {
			if (this == obj)
				return true;
			DevExpress.XtraPrinting.BrickStyle style = obj as DevExpress.XtraPrinting.BrickStyle;
			return style != null &&
				Sides == style.Sides &&
				BorderWidth == style.BorderWidth &&
				BorderColor == style.BorderColor &&
				BackColor == style.BackColor &&
				ForeColor == style.ForeColor &&
				TextAlignment == style.TextAlignment &&
				Comparer.Equals(Font, style.Font) &&
				Comparer.Equals(fStringFormat, style.fStringFormat) &&
				Padding.Equals(style.Padding) &&
				fBorderStyle == style.fBorderStyle &&
				BorderDashStyle == style.BorderDashStyle &&
				GetType() == style.GetType() &&
				object.ReferenceEquals(PrintingSystem, style.PrintingSystem);
		}
		public RectangleF DeflateBorderWidth(RectangleF rect, float dpi) {
			return DeflateBorderWidth(rect, dpi, false);
		}
		public RectangleF InflateBorderWidth(RectangleF rect, float dpi) {
			return InflateBorderWidth(rect, dpi, false);
		}
		public RectangleF DeflateBorderWidth(RectangleF rect, float dpi, bool applyBorderStyle) {
			float borderWidth = applyBorderStyle ? BrickStyle.AdjustBorderWidthToDeflate(BorderWidth, fBorderStyle) : BorderWidth;
			return BrickStyle.DeflateBorderWidth(rect, Sides, GraphicsUnitConverter.Convert(borderWidth, GraphicsDpi.DeviceIndependentPixel, dpi));
		}
		public RectangleF InflateBorderWidth(RectangleF rect, float dpi, bool applyBorderStyle) {
			return InflateBorderWidth(rect, dpi, applyBorderStyle, Sides);
		}
		public RectangleF InflateBorderWidth(RectangleF rect, float dpi, bool applyBorderStyle, BorderSide sides) {
			float borderWidth = applyBorderStyle ? BrickStyle.AdjustBorderWidthToInflate(BorderWidth, fBorderStyle) : BorderWidth;
			return BrickStyle.InflateBorderWidth(rect, sides, GraphicsUnitConverter.Convert(borderWidth, GraphicsDpi.DeviceIndependentPixel, dpi));
		}
		public void SetAlignment(HorzAlignment horzAlignment, VertAlignment vertAlignment) {
			TextAlignment = TextAlignmentConverter.ToTextAlignment(horzAlignment, vertAlignment);
			StringFormat = StringFormat.ChangeAlignment(
				AlignmentConverter.HorzAlignmentToStringAlignment(horzAlignment),
				AlignmentConverter.VertAlignmentToStringAlignment(vertAlignment));
		}
#region Disposing
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~BrickStyle() {
			Dispose(false);
		}
		protected virtual void Dispose(bool disposing) {
			if(!disposed) {
				if(fStringFormat != null) {
					fStringFormat.Dispose();
					fStringFormat = null;
				}
				disposed = true;
			}
		}
#endregion
#region Get initial values
		protected virtual Color GetInitialBackColor() {
			return DXColor.Empty;
		}
		protected virtual Color GetInitialBorderColor() {
			return DXColor.Empty;
		}
		protected virtual BorderSide GetInitialBorders() {
			return BorderSide.None;
		}
		protected virtual BorderDashStyle GetInitialBorderDashStyle() {
			return BorderDashStyle.Solid;
		}
		protected virtual float GetInitialBorderWidth() {
			return 0;
		}
		protected virtual Font GetInitialFont() {
			return DefaultFont;
		}
		protected virtual Color GetInitialForeColor() {
			return DXColor.Empty;
		}
		protected virtual PaddingInfo GetInitialPadding() {
			return PaddingInfo.Empty;
		}
		protected virtual TextAlignment GetInitialTextAlignment() {
			return TextAlignment.TopLeft;
		}
#endregion
#region IsSet properties
		internal bool IsSetBackColor {
			get { return (setProperties & StyleProperty.BackColor) != 0; }
		}
		internal bool IsSetBorderColor {
			get { return (setProperties & StyleProperty.BorderColor) != 0; }
		}
		internal bool IsSetBorderDashStyle {
			get { return (setProperties & StyleProperty.BorderDashStyle) != 0; }
		}
		internal bool IsSetBorders {
			get { return (setProperties & StyleProperty.Borders) != 0; }
		}
		internal bool IsSetBorderWidth {
			get { return (setProperties & StyleProperty.BorderWidth) != 0; }
		}
		internal bool IsSetFont {
			get { return (setProperties & StyleProperty.Font) != 0; }
		}
		internal bool IsSetForeColor {
			get { return (setProperties & StyleProperty.ForeColor) != 0; }
		}
		internal bool IsSetPadding {
			get { return (setProperties & StyleProperty.Padding) != 0; }
		}
		internal bool IsSetTextAlignment {
			get { return (setProperties & StyleProperty.TextAlignment) != 0; }
		}
		bool IsSetTabInterval {
			get { return !float.IsNaN(fTabInterval); }
		}
#endregion
#region Reset functions
		public void ResetBackColor() {
			FlagReset(StyleProperty.BackColor);
		}
		void SetBackColor(Color value) {
			fBackColor = value;
			FlagSet(StyleProperty.BackColor);
		}
		public void ResetBorderColor() {
			FlagReset(StyleProperty.BorderColor);
		}
		void SetBorderColor(Color value) {
			fBorderColor = value;
			FlagSet(StyleProperty.BorderColor);
		}
		public void ResetBorders() {
			FlagReset(StyleProperty.Borders);
		}
		void SetBorders(BorderSide value) {
			fSides = value;
			FlagSet(StyleProperty.Borders);
		}
		public void ResetBorderWidth() {
			FlagReset(StyleProperty.BorderWidth);
		}
		void SetBorderWidth(float value) {
			fBorderWidth = value;
			FlagSet(StyleProperty.BorderWidth);
		}
		public void ResetBorderDashStyle() {
			FlagReset(StyleProperty.BorderDashStyle);
		}
		void SetBorderDashStyle(BorderDashStyle value) {
			fBorderDashStyle = value;
			FlagSet(StyleProperty.BorderDashStyle);
		}
		public void ResetFont() {
			FlagReset(StyleProperty.Font);
			fFont = null;
		}
		void SetFont(Font value) {
			fFont = value;
			ResetTabInterval();
			FlagSet(StyleProperty.Font);
		}
		void SetStringFormat(BrickStringFormat value) {
			fStringFormat = value != null ? (BrickStringFormat)value.Clone() : null;
			ResetTabInterval();
		}
		public void ResetForeColor() {
			FlagReset(StyleProperty.ForeColor);
		}
		void SetForeColor(Color value) {
			fForeColor = value;
			FlagSet(StyleProperty.ForeColor);
		}
		public void ResetPadding() {
			FlagReset(StyleProperty.Padding);
		}
		void SetPadding(PaddingInfo value) {
			fPadding = value;
			FlagSet(StyleProperty.Padding);
		}
		public void ResetTextAlignment() {
			FlagReset(StyleProperty.TextAlignment);
		}
		void SetTextAlignment(TextAlignment value) {
			fTextAlignment = value;
			FlagSet(StyleProperty.TextAlignment);
		}
		void ResetTabInterval() {
			fTabInterval = float.NaN;
		}
		void FlagSet(StyleProperty value) {
			setProperties = setProperties | value;
		}
		void FlagReset(StyleProperty value) {
			setProperties = setProperties & ~value;
		}
		static Font CreateFontInPoints(Font source) {
			return new Font(source.FontFamily,
				GraphicsUnitConverter.Convert(source.Size, source.Unit, GraphicsUnit.Point),
				source.Style, GraphicsUnit.Point, source.GdiCharSet);
		}
		void InvalidateFontInPoints() {
			fontInPoints = null;
		}
#endregion
		#region IStoredObject Members
		long id = StoredObjectExtentions.UndefinedId;
		long IStoredObject.Id {
			get { return id; }
			set { id = value; }
		}
		byte[] IStoredObject.Store(IRepositoryProvider provider) {
			throw new NotSupportedException();
		}
		void IStoredObject.Restore(IRepositoryProvider provider, byte[] store) {
			throw new NotSupportedException();
		}
		#endregion
	}
}
