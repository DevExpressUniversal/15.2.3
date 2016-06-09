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
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	#region usings
	using DevExpress.Spreadsheet;
	using DevExpress.Utils;
	using DevExpress.Export.Xl;
	using DevExpress.Compatibility.System.Drawing;
	#endregion
	#region NativeTableStylePropertiesBase (abstract class)
	abstract partial class NativeTableStyleElementBase {
		readonly NativeTableStyle nativeTableStyle;
		readonly TableStyleElementType type;
		public NativeTableStyleElementBase(NativeTableStyle nativeTableStyle, TableStyleElementType type) {
			Guard.ArgumentNotNull(nativeTableStyle, "nativeTableStyle");
			this.nativeTableStyle = nativeTableStyle;
			this.type = type;
		}
		protected NativeTableStyle NativeTableStyle { get { return nativeTableStyle; } }
		protected TableStyleElementType TableStyleElementType { get { return type; } }
		protected Model.TableStyle ModelTableStyle { get { return nativeTableStyle.ModelTableStyle; } }
		protected int ModelElementIndex { get { return (int)type; } }
		protected Model.ITableStyleElementFormat ModelTableStyleElementFormat { get { return ModelTableStyle.GetTableStyleElementFormat(ModelElementIndex); } }
		protected virtual void CheckValidOperation() { 
			if(nativeTableStyle.BuiltIn)
				throw new InvalidOperationException();
		}
	}
	#endregion
	#region NativeTableStyleBorders
	partial class NativeTableStyleBorders : NativeTableStyleElementBase, Borders {
		public NativeTableStyleBorders(NativeTableStyle nativeTableStyle, TableStyleElementType type)
			: base(nativeTableStyle, type) {
		}
		Model.IBorderInfo BorderInfo { get { return ModelTableStyleElementFormat.Border; } }
		#region Borders Members
		public Border LeftBorder { get { return new NativeTableStyleLeftBorderPart(BorderInfo, NativeTableStyle.BuiltIn); } }
		public Border RightBorder { get { return new NativeTableStyleRightBorderPart(BorderInfo, NativeTableStyle.BuiltIn); } }
		public Border TopBorder { get { return new NativeTableStyleTopBorderPart(BorderInfo, NativeTableStyle.BuiltIn); } }
		public Border BottomBorder { get { return new NativeTableStyleBottomBorderPart(BorderInfo, NativeTableStyle.BuiltIn); } }
		public Border InsideHorizontalBorders { get { return new NativeTableStyleInsideHorizontalBorderPart(BorderInfo, NativeTableStyle.BuiltIn); } }
		public Border InsideVerticalBorders { get { return new NativeTableStyleInsideVerticalBorderPart(BorderInfo, NativeTableStyle.BuiltIn); } }
		public Color DiagonalBorderColor {
			get { return BorderInfo.DiagonalColor; }
			set {
				CheckValidOperation();
				BorderInfo.DiagonalColor = value; }
		}
		public BorderLineStyle DiagonalBorderLineStyle {
			get {
				if (DiagonalBorderType == Spreadsheet.DiagonalBorderType.Down)
					return (BorderLineStyle)BorderInfo.DiagonalDownLineStyle;
				return (BorderLineStyle)BorderInfo.DiagonalUpLineStyle;
			}
			set {
				CheckValidOperation();
				ModelTableStyle.BeginUpdate();
				try {
					if (DiagonalBorderType == Spreadsheet.DiagonalBorderType.None)
						DiagonalBorderType = Spreadsheet.DiagonalBorderType.UpAndDown;
					if (DiagonalBorderType == Spreadsheet.DiagonalBorderType.UpAndDown || DiagonalBorderType == Spreadsheet.DiagonalBorderType.Down)
						BorderInfo.DiagonalDownLineStyle = (XlBorderLineStyle)value;
					if (DiagonalBorderType == Spreadsheet.DiagonalBorderType.UpAndDown || DiagonalBorderType == Spreadsheet.DiagonalBorderType.Up)
						BorderInfo.DiagonalUpLineStyle = (XlBorderLineStyle)value;
				} finally {
					ModelTableStyle.EndUpdate();
				}
			}
		}
		public DiagonalBorderType DiagonalBorderType {
			get {
				bool up = BorderInfo.DiagonalUpLineStyle != XlBorderLineStyle.None;
				bool down = BorderInfo.DiagonalDownLineStyle != XlBorderLineStyle.None;
				if (up && !down)
					return DiagonalBorderType.Up;
				if (up && down)
					return DiagonalBorderType.UpAndDown;
				if (!up && down)
					return DiagonalBorderType.Down;
				return DiagonalBorderType.None;
			}
			set {
				CheckValidOperation();
				ModelTableStyle.BeginUpdate();
				try {
					if (value == Spreadsheet.DiagonalBorderType.None) {
						BorderInfo.DiagonalUpLineStyle = XlBorderLineStyle.None;
						BorderInfo.DiagonalDownLineStyle = XlBorderLineStyle.None;
					}
					if (value == Spreadsheet.DiagonalBorderType.UpAndDown) {
						if (BorderInfo.DiagonalDownLineStyle == XlBorderLineStyle.None)
							BorderInfo.DiagonalDownLineStyle = XlBorderLineStyle.Thin;
						if (BorderInfo.DiagonalUpLineStyle == XlBorderLineStyle.None)
							BorderInfo.DiagonalUpLineStyle = XlBorderLineStyle.Thin;
					}
					if (value == Spreadsheet.DiagonalBorderType.Down) {
						if (BorderInfo.DiagonalDownLineStyle == XlBorderLineStyle.None)
							BorderInfo.DiagonalDownLineStyle = XlBorderLineStyle.Thin;
						BorderInfo.DiagonalUpLineStyle = XlBorderLineStyle.None;
					}
					if (value == Spreadsheet.DiagonalBorderType.Up) {
						if (BorderInfo.DiagonalUpLineStyle == XlBorderLineStyle.None)
							BorderInfo.DiagonalUpLineStyle = XlBorderLineStyle.Thin;
						BorderInfo.DiagonalDownLineStyle = XlBorderLineStyle.None;
					}
				} finally {
					ModelTableStyle.EndUpdate();
				}
			}
		}
		public void SetOutsideBorders(Color color, BorderLineStyle style) {
			CheckValidOperation();
			ModelTableStyle.BeginUpdate();
			try {
				BorderInfo.TopColor = color;
				BorderInfo.TopLineStyle = (XlBorderLineStyle)style;
				BorderInfo.BottomColor = color;
				BorderInfo.BottomLineStyle = (XlBorderLineStyle)style;
				BorderInfo.LeftColor = color;
				BorderInfo.LeftLineStyle = (XlBorderLineStyle)style;
				BorderInfo.RightColor = color;
				BorderInfo.RightLineStyle = (XlBorderLineStyle)style;
			} finally {
				ModelTableStyle.EndUpdate();
			}
		}
		public void SetDiagonalBorders(Color color, BorderLineStyle style, DiagonalBorderType diagonalBorderType) {
			CheckValidOperation();
			if (diagonalBorderType == DiagonalBorderType.None)
				return;
			ModelTableStyle.BeginUpdate();
			try {
				BorderInfo.DiagonalColor = color;
				if (diagonalBorderType == DiagonalBorderType.UpAndDown) {
					BorderInfo.DiagonalUpLineStyle = (XlBorderLineStyle)style;
					BorderInfo.DiagonalDownLineStyle = (XlBorderLineStyle)style;
				}
				if (diagonalBorderType == DiagonalBorderType.Up) {
					BorderInfo.DiagonalUpLineStyle = (XlBorderLineStyle)style;
					BorderInfo.DiagonalDownLineStyle = XlBorderLineStyle.None;
				}
				if (diagonalBorderType == DiagonalBorderType.Down) {
					BorderInfo.DiagonalUpLineStyle = XlBorderLineStyle.None;
					BorderInfo.DiagonalDownLineStyle = (XlBorderLineStyle)style;
				}
			} finally {
				ModelTableStyle.EndUpdate();
			}
		}
		public void SetAllBorders(Color color, BorderLineStyle style) {
			CheckValidOperation();
			ModelTableStyle.BeginUpdate();
			try {
				BorderInfo.TopColor = color;
				BorderInfo.TopLineStyle = (XlBorderLineStyle)style;
				BorderInfo.BottomColor = color;
				BorderInfo.BottomLineStyle = (XlBorderLineStyle)style;
				BorderInfo.LeftColor = color;
				BorderInfo.LeftLineStyle = (XlBorderLineStyle)style;
				BorderInfo.RightColor = color;
				BorderInfo.RightLineStyle = (XlBorderLineStyle)style;
				BorderInfo.HorizontalColor = color;
				BorderInfo.HorizontalLineStyle = (XlBorderLineStyle)style;
				BorderInfo.VerticalColor = color;
				BorderInfo.VerticalLineStyle = (XlBorderLineStyle)style;
			} finally {
				ModelTableStyle.EndUpdate();
			}
		}
		public void RemoveBorders() {
			CheckValidOperation();
			ModelTableStyle.ClearBorders(ModelElementIndex);
		}
		#endregion
	}
	abstract partial class NativeTableStyleBorderPart : Border {
		Model.IBorderInfo innerBorderInfo;
		bool isBuiltInTableStyle;
		protected NativeTableStyleBorderPart(Model.IBorderInfo innerBorderInfo, bool isBuiltInTableStyle) {
			this.innerBorderInfo = innerBorderInfo;
			this.isBuiltInTableStyle = isBuiltInTableStyle;
		}
		protected Model.IBorderInfo InnerBorderInfo { get { return innerBorderInfo; } }
		#region Border Members
		public Color Color { 
			get { return GetColor(); } 
			set {
				CheckValidOperation();
				SetColor(value); 
			} 
		}
		public BorderLineStyle LineStyle { 
			get { return GetLineStyle(); } 
			set {
				CheckValidOperation();
				SetLineStyle(value); 
			} 
		}
		#endregion
		protected abstract Color GetColor();
		protected abstract void SetColor(Color value);
		protected abstract BorderLineStyle GetLineStyle();
		protected abstract void SetLineStyle(BorderLineStyle value);
		void CheckValidOperation() {
			if (isBuiltInTableStyle)
				throw new InvalidOperationException();
		}
	}
	partial class NativeTableStyleLeftBorderPart : NativeTableStyleBorderPart {
		public NativeTableStyleLeftBorderPart(Model.IBorderInfo innerBorderInfo, bool isBuiltInTableStyle)
			: base(innerBorderInfo, isBuiltInTableStyle) {
		}
		protected override Color GetColor() {
			return InnerBorderInfo.LeftColor;
		}
		protected override void SetColor(Color value) {
			InnerBorderInfo.LeftColor = value;
		}
		protected override BorderLineStyle GetLineStyle() {
			return (BorderLineStyle)InnerBorderInfo.LeftLineStyle;
		}
		protected override void SetLineStyle(BorderLineStyle value) {
			InnerBorderInfo.LeftLineStyle = (XlBorderLineStyle)value;
		}
	}
	partial class NativeTableStyleRightBorderPart : NativeTableStyleBorderPart {
		public NativeTableStyleRightBorderPart(Model.IBorderInfo innerBorderInfo, bool isBuiltInTableStyle)
			: base(innerBorderInfo, isBuiltInTableStyle) {
		}
		protected override Color GetColor() {
			return InnerBorderInfo.RightColor;
		}
		protected override void SetColor(Color value) {
			InnerBorderInfo.RightColor = value;
		}
		protected override BorderLineStyle GetLineStyle() {
			return (BorderLineStyle)InnerBorderInfo.RightLineStyle;
		}
		protected override void SetLineStyle(BorderLineStyle value) {
			InnerBorderInfo.RightLineStyle = (XlBorderLineStyle)value;
		}
	}
	partial class NativeTableStyleTopBorderPart : NativeTableStyleBorderPart {
		public NativeTableStyleTopBorderPart(Model.IBorderInfo innerBorderInfo, bool isBuiltInTableStyle)
			: base(innerBorderInfo, isBuiltInTableStyle) {
		}
		protected override Color GetColor() {
			return InnerBorderInfo.TopColor;
		}
		protected override void SetColor(Color value) {
			InnerBorderInfo.TopColor = value;
		}
		protected override BorderLineStyle GetLineStyle() {
			return (BorderLineStyle)InnerBorderInfo.TopLineStyle;
		}
		protected override void SetLineStyle(BorderLineStyle value) {
			InnerBorderInfo.TopLineStyle = (XlBorderLineStyle)value;
		}
	}
	partial class NativeTableStyleBottomBorderPart : NativeTableStyleBorderPart {
		public NativeTableStyleBottomBorderPart(Model.IBorderInfo innerBorderInfo, bool isBuiltInTableStyle)
			: base(innerBorderInfo, isBuiltInTableStyle) {
		}
		protected override Color GetColor() {
			return InnerBorderInfo.BottomColor;
		}
		protected override void SetColor(Color value) {
			InnerBorderInfo.BottomColor = value;
		}
		protected override BorderLineStyle GetLineStyle() {
			return (BorderLineStyle)InnerBorderInfo.BottomLineStyle;
		}
		protected override void SetLineStyle(BorderLineStyle value) {
			InnerBorderInfo.BottomLineStyle = (XlBorderLineStyle)value;
		}
	}
	partial class NativeTableStyleInsideHorizontalBorderPart : NativeTableStyleBorderPart {
		public NativeTableStyleInsideHorizontalBorderPart(Model.IBorderInfo innerBorderInfo, bool isBuiltInTableStyle)
			: base(innerBorderInfo, isBuiltInTableStyle) {
		}
		protected override Color GetColor() {
			return InnerBorderInfo.HorizontalColor;
		}
		protected override void SetColor(Color value) {
			InnerBorderInfo.HorizontalColor = value;
		}
		protected override BorderLineStyle GetLineStyle() {
			return (BorderLineStyle)InnerBorderInfo.HorizontalLineStyle;
		}
		protected override void SetLineStyle(BorderLineStyle value) {
			InnerBorderInfo.HorizontalLineStyle = (XlBorderLineStyle)value;
		}
	}
	partial class NativeTableStyleInsideVerticalBorderPart : NativeTableStyleBorderPart {
		public NativeTableStyleInsideVerticalBorderPart(Model.IBorderInfo innerBorderInfo, bool isBuiltInTableStyle)
			: base(innerBorderInfo, isBuiltInTableStyle) {
		}
		protected override Color GetColor() {
			return InnerBorderInfo.VerticalColor;
		}
		protected override void SetColor(Color value) {
			InnerBorderInfo.VerticalColor = value;
		}
		protected override BorderLineStyle GetLineStyle() {
			return (BorderLineStyle)InnerBorderInfo.VerticalLineStyle;
		}
		protected override void SetLineStyle(BorderLineStyle value) {
			InnerBorderInfo.VerticalLineStyle = (XlBorderLineStyle)value;
		}
	}
	#endregion
	#region NativeTableStyleFont
	partial class NativeTableStyleFont : NativeTableStyleElementBase, SpreadsheetFont {
		public NativeTableStyleFont(NativeTableStyle nativeTableStyle, TableStyleElementType type)
			: base(nativeTableStyle, type) {
		}
		Model.IRunFontInfo FontInfo { get { return ModelTableStyleElementFormat.Font; } }
		#region Font Members
		public SpreadsheetFontStyle FontStyle {
			get {
				bool bold = FontInfo.Bold;
				bool italic = FontInfo.Italic;
				if (bold && italic)
					return DevExpress.Spreadsheet.SpreadsheetFontStyle.BoldItalic;
				if (bold && !italic)
					return DevExpress.Spreadsheet.SpreadsheetFontStyle.Bold;
				if (!bold && italic)
					return DevExpress.Spreadsheet.SpreadsheetFontStyle.Italic;
				return DevExpress.Spreadsheet.SpreadsheetFontStyle.Regular;
			}
			set {
				CheckValidOperation();
				ModelTableStyle.BeginUpdate();
				try {
					switch (value) {
						case DevExpress.Spreadsheet.SpreadsheetFontStyle.Bold:
							FontInfo.Bold = true;
							FontInfo.Italic = false;
						break;
						case DevExpress.Spreadsheet.SpreadsheetFontStyle.BoldItalic:
							FontInfo.Bold = true;
							FontInfo.Italic = true;
						break;
						case DevExpress.Spreadsheet.SpreadsheetFontStyle.Italic:
							FontInfo.Bold = false;
							FontInfo.Italic = true;
						break;
						default:
							FontInfo.Bold = false;
							FontInfo.Italic = false;
						break;
					}
				} finally {
					ModelTableStyle.EndUpdate();
				}
			}
		}
		public bool Bold { 
			get { return FontInfo.Bold; } 
			set {
				CheckValidOperation();
				FontInfo.Bold = value; 
			} 
		}
		public bool Italic { 
			get { return FontInfo.Italic; } 
			set {
				CheckValidOperation();
				FontInfo.Italic = value; 
			} 
		}
		public string Name { 
			get { return FontInfo.Name; } 
			set {
				CheckValidOperation();
				FontInfo.Name = value; 
			} 
		}
		public ScriptType Script {
			get { return (ScriptType)FontInfo.Script; }
			set {
				CheckValidOperation();
				FontInfo.Script = (XlScriptType)value; }
		}
		public double Size { 
			get { return FontInfo.Size; } 
			set {
				CheckValidOperation();
				FontInfo.Size = value; 
			} 
		}
		public UnderlineType UnderlineType {
			get {
				return (UnderlineType)FontInfo.Underline;
			}
			set {
				CheckValidOperation();
				FontInfo.Underline = (XlUnderlineType)(value);
			}
		}
		public bool Strikethrough { 
			get { return FontInfo.StrikeThrough; } 
			set {
				CheckValidOperation();
				FontInfo.StrikeThrough = value; 
			} 
		}
		public Color Color { 
			get { return FontInfo.Color; } 
			set {
				CheckValidOperation();
				FontInfo.Color = value; 
			} 
		}
		#endregion
		public void Clear() {
			CheckValidOperation();
			ModelTableStyle.ClearFont(ModelElementIndex);
		}
	}
	#endregion
	#region NativeTableStyleFill
	partial class NativeTableStyleFill : NativeTableStyleElementBase, Fill, IDifferentialFormatGradientFillAccessor {
		NativeDifferentialFormatGradientFill gradientFill; 
		public NativeTableStyleFill(NativeTableStyle nativeTableStyle, TableStyleElementType type)
			: base(nativeTableStyle, type) {
			this.gradientFill = new NativeDifferentialFormatGradientFill(this);
		}
		Model.IFillInfo FillInfo { get { return ModelTableStyleElementFormat.Fill; } }
		#region Fill Members
		public Color BackgroundColor { 
			get { return FillInfo.BackColor; } 
			set {
				CheckValidOperation();
				FillInfo.BackColor = value; 
			} 
		}
		public Color PatternColor { 
			get { return FillInfo.ForeColor; } 
			set {
				CheckValidOperation();
				FillInfo.ForeColor = value; 
			} 
		}
		public PatternType PatternType { 
			get { return (PatternType)FillInfo.PatternType; } 
			set {
				CheckValidOperation();
				FillInfo.PatternType = (XlPatternType)value; 
			} 
		}
		public FillType FillType {
			get { return (FillType)FillInfo.FillType; }
			set { FillInfo.FillType = (Model.ModelFillType)value; }
		}
		public GradientFill Gradient { get { return gradientFill; } }
		#endregion
		#region IGradientFillAccessor Members
		Model.IGradientFillInfo IDifferentialFormatGradientFillAccessor.ReadWriteInfo { get { return FillInfo.GradientFill; } }
		#endregion
		public void Clear() {
			CheckValidOperation();
			ModelTableStyle.ClearFill(ModelElementIndex);
		}
	}
	#endregion
}
