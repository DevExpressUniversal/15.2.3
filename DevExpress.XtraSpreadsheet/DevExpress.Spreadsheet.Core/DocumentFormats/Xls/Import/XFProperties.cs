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
using System.IO;
using System.Reflection;
using System.Text;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.Export.Xl;
using DevExpress.XtraExport.Xls;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
using DevExpress.Office.Model;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XFPropUnknown
	public class XFPropUnknown : XFPropertyBase {
		protected override void ReadCore(XlsReader reader) {
			if(DataSize > 0)
				reader.ReadBytes(DataSize);
		}
		protected override void WriteCore(BinaryWriter writer) {
		}
		protected override short GetSizeCore() {
			return 0;
		}
	}
	#endregion
	#region XFPropFillPattern
	public class XFPropFillPattern : XFPropertyBase {
		public XlPatternType FillPatternType { get; set; }
		protected override void ReadCore(XlsReader reader) {
			FillPatternType = (XlPatternType)reader.ReadByte();
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((byte)FillPatternType);
		}
		protected override short GetSizeCore() {
			return 1;
		}
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			contentReceiver.SetFillPatternType(FillPatternType);
		}
	}
	#endregion
	#region XFPropColor
	public class XFPropForegroundColor : XFPropColorBase {
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			contentReceiver.SetForegroundColor(ColorInfo);
		}
	}
	public class XFPropBackgroundColor : XFPropColorBase {
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			contentReceiver.SetBackgroundColor(ColorInfo);
		}
	}
	public class XFPropTextColor : XFPropColorBase {
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			contentReceiver.SetTextColor(ColorInfo);
		}
	}
	#endregion
	#region XFPropGradient
	public class XFGradient {
		public const int Size = 44;
		GradientFillInfo info = new GradientFillInfo();
		public static XFGradient FromStream(XlsReader reader) {
			XFGradient result = new XFGradient();
			result.Read(reader);
			return result;
		}
		protected internal GradientFillInfo Info { get { return info; } set { info = value; } }
		protected void Read(XlsReader reader) {
			info.Type = Convert.ToBoolean(reader.ReadUInt32())? ModelGradientFillType.Path : ModelGradientFillType.Linear;
			info.Degree = reader.ReadDouble();
			info.Left = (float)reader.ReadDouble();
			info.Right = (float)reader.ReadDouble();
			info.Top = (float)reader.ReadDouble();
			info.Bottom = (float)reader.ReadDouble();
		}
		public void Write(BinaryWriter writer) {
			writer.Write(Convert.ToUInt32(info.Type == ModelGradientFillType.Path));
			writer.Write(info.Degree);
			writer.Write((double)info.Left);
			writer.Write((double)info.Right);
			writer.Write((double)info.Top);
			writer.Write((double)info.Bottom);
		}
	}
	public class XFPropGradient : XFPropertyBase {
		XFGradient gradient = new XFGradient();
		public XFGradient Gradient {
			get { return gradient; }
			set {
				Guard.ArgumentNotNull(value, "Gradient value");
				gradient = value;
			}
		}
		protected override void ReadCore(XlsReader reader) {
			this.gradient = XFGradient.FromStream(reader);
		}
		protected override void WriteCore(BinaryWriter writer) {
			this.gradient.Write(writer);
		}
		protected override short GetSizeCore() {
			return (short)XFGradient.Size;
		}
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			contentReceiver.SetXFGradient(gradient.Info);
		}
	}
	public class XFPropGradientStop : XFPropColorBase {
		public double Position { get; set; }
		protected override void ReadCore(XlsReader reader) {
			reader.ReadUInt16(); 
			Position = reader.ReadDouble();
			base.ReadCore(reader);
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((ushort)0); 
			writer.Write(Position);
			base.WriteCore(writer);
		}
		protected override short GetSizeCore() {
			return (short)(base.GetSizeCore() + 10);
		}
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			contentReceiver.SetXFGradientStop(new XFGradStop(Position, ColorInfo));
		}
	}
	#endregion
	#region XFPropBorder
	public class XFPropTopBorder : XFPropBorderBase {
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			if(HasColorValue)
				contentReceiver.SetTopBorderColor(ColorInfo);
			contentReceiver.SetTopBorderLineStyle(LineStyle);
		}
	}
	public class XFPropBottomBorder : XFPropBorderBase {
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			if(HasColorValue)
				contentReceiver.SetBottomBorderColor(ColorInfo);
			contentReceiver.SetBottomBorderLineStyle(LineStyle);
		}
	}
	public class XFPropLeftBorder : XFPropBorderBase {
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			if(HasColorValue)
				contentReceiver.SetLeftBorderColor(ColorInfo);
			contentReceiver.SetLeftBorderLineStyle(LineStyle);
		}
	}
	public class XFPropRightBorder : XFPropBorderBase {
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			if(HasColorValue)
				contentReceiver.SetRightBorderColor(ColorInfo);
			contentReceiver.SetRightBorderLineStyle(LineStyle);
		}
	}
	public class XFPropDiagonalBorder : XFPropBorderBase {
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			if(HasColorValue)
				contentReceiver.SetDiagonalBorderColor(ColorInfo);
			contentReceiver.SetDiagonalBorderLineStyle(LineStyle);
		}
	}
	public class XFPropVerticalBorder : XFPropBorderBase {
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			if(HasColorValue)
				contentReceiver.SetVerticalBorderColor(ColorInfo);
			contentReceiver.SetVerticalBorderLineStyle(LineStyle);
		}
	}
	public class XFPropHorizontalBorder : XFPropBorderBase {
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			if(HasColorValue)
				contentReceiver.SetHorizontalBorderColor(ColorInfo);
			contentReceiver.SetHorizontalBorderLineStyle(LineStyle);
		}
	}
	#endregion
	#region XFPropBool
	public class XFPropDiagonalUpBorder : XFPropBoolBase {
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			contentReceiver.SetDiagonalUpBorder(Value);
		}
	}
	public class XFPropDiagonalDownBorder : XFPropBoolBase {
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			contentReceiver.SetDiagonalDownBorder(Value);
		}
	}
	public class XFPropTextWrapped : XFPropBoolBase {
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			contentReceiver.SetTextWrapped(Value);
		}
	}
	public class XFPropTextJustifyDistributed : XFPropBoolBase {
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			contentReceiver.SetTextJustifyDistributed(Value);
		}
	}
	public class XFPropShrinkToFit : XFPropBoolBase {
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			contentReceiver.SetShrinkToFit(Value);
		}
	}
	public class XFPropCellMerged : XFPropBoolBase {
	}
	public class XFPropTextItalic : XFPropBoolBase {
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			contentReceiver.SetTextItalic(Value);
		}
	}
	public class XFPropTextStrikethrough : XFPropBoolBase {
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			contentReceiver.SetTextStrikethrough(Value);
		}
	}
	public class XFPropHasOutlineStyle : XFPropBoolBase {
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			contentReceiver.SetHasOutlineStyle(Value);
		}
	}
	public class XFPropHasShadowStyle : XFPropBoolBase {
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			contentReceiver.SetHasShadowStyle(Value);
		}
	}
	public class XFPropTextCondensed : XFPropBoolBase {
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			contentReceiver.SetTextCondensed(Value);
		}
	}
	public class XFPropTextExtended : XFPropBoolBase {
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			contentReceiver.SetTextExtended(Value);
		}
	}
	public class XFPropLockedProtection : XFPropBoolBase {
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			contentReceiver.SetLockedProtection(Value);
		}
	}
	public class XFPropHiddenProtection : XFPropBoolBase {
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			contentReceiver.SetHiddenProtection(Value);
		}
	}
	#endregion
	#region XFPropAlignment / Text rotation / Indentation / Reading order
	public class XFPropHorizontalAlignment : XFPropertyBase {
		XlHorizontalAlignment value;
		bool hasValue;
		public XlHorizontalAlignment Value {
			get { return value; }
			set {
				this.value = value;
				this.hasValue = true;
			}
		}
		public bool HasValue { get { return hasValue; } }
		protected override void ReadCore(XlsReader reader) {
			byte value = reader.ReadByte();
			if(value != 0xff)
				Value = (XlHorizontalAlignment)value;
		}
		protected override void WriteCore(BinaryWriter writer) {
			if(HasValue)
				writer.Write((byte)Value);
			else
				writer.Write((byte)0xff);
		}
		protected override short GetSizeCore() {
			return 1;
		}
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			if(HasValue)
				contentReceiver.SetHorizontalAlignment(Value);
		}
	}
	public class XFPropVerticalAlignment : XFPropertyBase {
		public XlVerticalAlignment Value { get; set; }
		protected override void ReadCore(XlsReader reader) {
			Value = (XlVerticalAlignment)reader.ReadByte();
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((byte)Value);
		}
		protected override short GetSizeCore() {
			return 1;
		}
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			contentReceiver.SetVerticalAlignment(Value);
		}
	}
	public class XFPropTextRotation : XFPropByteBase {
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			contentReceiver.SetTextRotation(Value);
		}
	}
	public class XFPropIndentation : XFPropUShortBase {
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			contentReceiver.SetIndent(Value);
		}
	}
	public class XFPropReadingOrder : XFPropertyBase {
		public XlReadingOrder Value { get; set; }
		protected override void ReadCore(XlsReader reader) {
			Value = (XlReadingOrder)reader.ReadByte();
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((byte)Value);
		}
		protected override short GetSizeCore() {
			return 1;
		}
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			contentReceiver.SetReadingOrder(Value);
		}
	}
	#endregion
	#region XFPropFont
	public class XFPropFontName : XFPropertyBase {
		LPWideString fontName = new LPWideString();
		public string Value {
			get { return fontName.Value; }
			set {
				if(!string.IsNullOrEmpty(value) && value.Length > 32)
					throw new ArgumentException("Font name length exceed 32 characters");
				fontName.Value = value;
			}
		}
		protected override void ReadCore(XlsReader reader) {
			this.fontName = LPWideString.FromStream(reader);
		}
		protected override void WriteCore(BinaryWriter writer) {
			this.fontName.Write(writer);
		}
		protected override short GetSizeCore() {
			return (short)this.fontName.Length;
		}
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			if(!string.IsNullOrEmpty(Value))
				contentReceiver.SetFontName(Value);
		}
	}
	public class XFPropFontBold : XFPropBoolBase {
		protected override void ReadCore(XlsReader reader) {
			ushort value = reader.ReadUInt16();
			Value = value == 0x02bc;
		}
		protected override void WriteCore(BinaryWriter writer) {
			if(Value)
				writer.Write((ushort)0x02bc);
			else
				writer.Write((ushort)0x0190);
		}
		protected override short GetSizeCore() {
			return 2;
		}
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			contentReceiver.SetTextBold(Value);
		}
	}
	public class XFPropFontUnderline : XFPropertyBase {
		public XlUnderlineType Underline { get; set; }
		protected override void ReadCore(XlsReader reader) {
			Underline = (XlUnderlineType)reader.ReadUInt16();
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((ushort)Underline);
		}
		protected override short GetSizeCore() {
			return 2;
		}
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			contentReceiver.SetFontUnderline(Underline);
		}
	}
	public class XFPropFontScript : XFPropertyBase {
		public XlScriptType Script { get; set; }
		protected override void ReadCore(XlsReader reader) {
			Script = (XlScriptType)reader.ReadUInt16();
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((ushort)Script);
		}
		protected override short GetSizeCore() {
			return 2;
		}
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			contentReceiver.SetFontScript(Script);
		}
	}
	public class XFPropCharset : XFPropByteBase {
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			contentReceiver.SetCharset(Value);
		}
	}
	public class XFPropFontFamily : XFPropByteBase {
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			contentReceiver.SetFontFamily(Value);
		}
	}
	public class XFPropTextSizeInTwips : XFPropertyBase {
		public int Value { get; set; }
		protected override void ReadCore(XlsReader reader) {
			Value = reader.ReadInt32();
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write(Value);
		}
		protected override short GetSizeCore() {
			return 4;
		}
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			contentReceiver.SetTextSizeInTwips(Value);
		}
	}
	public class XFPropFontScheme : XFPropertyBase {
		XlFontSchemeStyles value;
		bool hasValue;
		public XlFontSchemeStyles Value {
			get { return value; }
			set {
				this.value = value;
				this.hasValue = true;
			}
		}
		public bool HasValue { get { return hasValue; } }
		protected override void ReadCore(XlsReader reader) {
			byte scheme = reader.ReadByte();
			if(scheme != 0xff)
				Value = ByteToFontScheme(scheme);
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write(FontSchemeToByte(Value, HasValue));
		}
		protected override short GetSizeCore() {
			return 1;
		}
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			if(HasValue)
				contentReceiver.SetFontScheme(Value);
		}
	}
	#endregion
	#region XFPropNumberFormat
	public class XFPropNumberFormat : XFPropertyBase {
		LPWideString formatValue = new LPWideString();
		public string Value {
			get { return formatValue.Value; }
			set { formatValue.Value = value; }
		}
		protected override void ReadCore(XlsReader reader) {
			this.formatValue = LPWideString.FromStream(reader);
		}
		protected override void WriteCore(BinaryWriter writer) {
			this.formatValue.Write(writer);
		}
		protected override short GetSizeCore() {
			return (short)this.formatValue.Length;
		}
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			contentReceiver.SetNumberFormatCode(Value);
		}
	}
	public class XFPropNumberFormatId : XFPropUShortBase {
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			contentReceiver.SetNumberFormatId(Value);
		}
	}
	#endregion
	#region XFPropRelativeIndentation
	public class XFPropRelativeIndentation : XFPropertyBase {
		public int Value { get; set; }
		protected override void ReadCore(XlsReader reader) {
			Value = reader.ReadInt16();
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((short)Value);
		}
		protected override short GetSizeCore() {
			return 2;
		}
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			contentReceiver.SetRelativeIndent(Value);
		}
	}
	#endregion
	#region XFExtPropColors
	public class XFExtPropForegroundColor : XFExtPropFullColorBase {
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			contentReceiver.SetForegroundColor(ColorInfo);
		}
	}
	public class XFExtPropBackgroundColor : XFExtPropFullColorBase {
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			contentReceiver.SetBackgroundColor(ColorInfo);
		}
	}
	public class XFExtPropTopBorderColor : XFExtPropFullColorBase {
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			contentReceiver.SetTopBorderColor(ColorInfo);
		}
	}
	public class XFExtPropBottomBorderColor : XFExtPropFullColorBase {
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			contentReceiver.SetBottomBorderColor(ColorInfo);
		}
	}
	public class XFExtPropLeftBorderColor : XFExtPropFullColorBase {
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			contentReceiver.SetLeftBorderColor(ColorInfo);
		}
	}
	public class XFExtPropRightBorderColor : XFExtPropFullColorBase {
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			contentReceiver.SetRightBorderColor(ColorInfo);
		}
	}
	public class XFExtPropDiagonalBorderColor : XFExtPropFullColorBase {
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			contentReceiver.SetDiagonalBorderColor(ColorInfo);
		}
	}
	public class XFExtPropTextColor : XFExtPropFullColorBase {
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			contentReceiver.SetTextColor(ColorInfo);
		}
	}
	#endregion
	#region XFExtPropGradient
	public class XFGradStop {
		public const int Size = 22;
		ColorModelInfo colorInfo = new ColorModelInfo();
		public XFGradStop() {
		}
		public XFGradStop(double position, ColorModelInfo colorInfo) {
			Position = position;
			ColorInfo = colorInfo;
		}
		public ColorModelInfo ColorInfo {
			get { return colorInfo; }
			set {
				Guard.ArgumentNotNull(value, "ColorInfo value");
				this.colorInfo = value;
			}
		}
		public double Position { get; set; }
		public static XFGradStop FromStream(XlsReader reader) {
			XFGradStop result = new XFGradStop();
			result.Read(reader);
			return result;
		}
		protected void Read(XlsReader reader) {
			XColorType colorType = (XColorType)reader.ReadUInt16();
			int colorValue = reader.ReadInt32();
			Position = reader.ReadDouble();
			double tint = reader.ReadDouble();
			switch(colorType) {
				case XColorType.Auto:
					colorInfo.Auto = true;
					break;
				case XColorType.Indexed:
					colorInfo.ColorIndex = colorValue;
					break;
				case XColorType.Rgb:
					byte[] argb = BitConverter.GetBytes(colorValue);
					colorInfo.Rgb = DXColor.FromArgb(argb[3], argb[0], argb[1], argb[2]);
					break;
				case XColorType.Themed:
					colorInfo.Theme = new ThemeColorIndex(colorValue);
					colorInfo.Tint = tint;
					break;
			}
		}
		public void Write(BinaryWriter writer) {
			XColorType colorType = XColorType.Ninched;
			int colorValue = 0;
			double tint = 0.0;
			if(colorInfo.ColorType == ColorType.Auto)
				colorType = XColorType.Auto;
			else if(colorInfo.ColorType == ColorType.Index) {
				colorType = XColorType.Indexed;
				colorValue = colorInfo.ColorIndex;
			}
			else if(colorInfo.ColorType == ColorType.Rgb) {
				colorType = XColorType.Rgb;
				Color argb = colorInfo.Rgb;
				colorValue = (int)((uint)argb.R | (((uint)argb.G) << 8) | (((uint)argb.B) << 16) | (((uint)argb.A) << 24));
			}
			else if(colorInfo.ColorType == ColorType.Theme) {
				colorType = XColorType.Themed;
				colorValue = colorInfo.Theme.ToInt();
				tint = colorInfo.Tint;
			}
			writer.Write((ushort)colorType);
			writer.Write(colorValue);
			writer.Write(Position);
			writer.Write(tint);
		}
	}
	public class XFExtPropGradient : XFExtPropertyBase {
		#region Fields
		XFGradient gradient = new XFGradient();
		List<XFGradStop> stops = new List<XFGradStop>();
		#endregion
		#region Properties
		public XFGradient Gradient {
			get { return gradient; }
			set {
				Guard.ArgumentNotNull(value, "Gradient value");
				gradient = value;
			}
		}
		public List<XFGradStop> Stops { get { return stops; } }
		#endregion
		protected override void ReadCore(XlsReader reader) {
			gradient = XFGradient.FromStream(reader);
			int count = reader.ReadInt32();
			for (int i = 0; i < count; i++) 
				this.stops.Add(XFGradStop.FromStream(reader));
		}
		protected override void WriteCore(BinaryWriter writer) {
			gradient.Write(writer);
			int count = stops.Count;
			writer.Write(count);
			for (int i = 0; i < count; i++) 
				stops[i].Write(writer);
		}
		protected override short GetSizeCore() {
			return (short)(XFGradient.Size + 4 + XFGradStop.Size * stops.Count);
		}
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			contentReceiver.SetXFGradient(gradient.Info, stops);
		}
	}
	#endregion
	#region XFExtPropFontScheme
	public class XFExtPropFontScheme : XFExtPropertyBase {
		XlFontSchemeStyles value;
		bool hasValue;
		public XlFontSchemeStyles Value {
			get { return value; }
			set {
				this.value = value;
				this.hasValue = true;
			}
		}
		public bool HasValue { get { return hasValue; } }
		protected override void ReadCore(XlsReader reader) {
			byte scheme = reader.ReadByte();
			if(scheme != 0xff)
				Value = ByteToFontScheme(scheme);
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write(FontSchemeToByte(Value, HasValue));
		}
		protected override short GetSizeCore() {
			return 1;
		}
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			if(HasValue)
				contentReceiver.SetFontScheme(Value);
		}
	}
	#endregion
	#region XFExtPropIndentLevel
	public class XFExtPropIndentLevel : XFExtPropertyBase {
		int value;
		public int Value {
			get { return value; }
			set {
				ValueChecker.CheckValue(value, 0, 250);
				this.value = value;
			} 
		}
		protected override void ReadCore(XlsReader reader) {
			Value = reader.ReadUInt16();
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((ushort)Value);
		}
		protected override short GetSizeCore() {
			return 2;
		}
		public override void ApplyContent(IXFContentReceiver contentReceiver) {
			contentReceiver.SetIndent(Value);
		}
	}
	#endregion
	#region XFPropertyFactory, XFExtPropertyFactory
	public static class XFPropertyFactory {
		class XFPropFactoryImpl : XFPropFactoryBase {
			protected override void InitializeFactory() {
				AddProduct(0x7fff, typeof(XFPropUnknown));
				AddProduct(0x0000, typeof(XFPropFillPattern));
				AddProduct(0x0001, typeof(XFPropForegroundColor));
				AddProduct(0x0002, typeof(XFPropBackgroundColor));
				AddProduct(0x0003, typeof(XFPropGradient));
				AddProduct(0x0004, typeof(XFPropGradientStop));
				AddProduct(0x0005, typeof(XFPropTextColor));
				AddProduct(0x0006, typeof(XFPropTopBorder));
				AddProduct(0x0007, typeof(XFPropBottomBorder));
				AddProduct(0x0008, typeof(XFPropLeftBorder));
				AddProduct(0x0009, typeof(XFPropRightBorder));
				AddProduct(0x000a, typeof(XFPropDiagonalBorder));
				AddProduct(0x000b, typeof(XFPropVerticalBorder));
				AddProduct(0x000c, typeof(XFPropHorizontalBorder));
				AddProduct(0x000d, typeof(XFPropDiagonalUpBorder));
				AddProduct(0x000e, typeof(XFPropDiagonalDownBorder));
				AddProduct(0x000f, typeof(XFPropHorizontalAlignment));
				AddProduct(0x0010, typeof(XFPropVerticalAlignment));
				AddProduct(0x0011, typeof(XFPropTextRotation));
				AddProduct(0x0012, typeof(XFPropIndentation));
				AddProduct(0x0013, typeof(XFPropReadingOrder));
				AddProduct(0x0014, typeof(XFPropTextWrapped));
				AddProduct(0x0015, typeof(XFPropTextJustifyDistributed));
				AddProduct(0x0016, typeof(XFPropShrinkToFit));
				AddProduct(0x0017, typeof(XFPropCellMerged));
				AddProduct(0x0018, typeof(XFPropFontName));
				AddProduct(0x0019, typeof(XFPropFontBold));
				AddProduct(0x001a, typeof(XFPropFontUnderline));
				AddProduct(0x001b, typeof(XFPropFontScript));
				AddProduct(0x001c, typeof(XFPropTextItalic));
				AddProduct(0x001d, typeof(XFPropTextStrikethrough));
				AddProduct(0x001e, typeof(XFPropHasOutlineStyle));
				AddProduct(0x001f, typeof(XFPropHasShadowStyle));
				AddProduct(0x0020, typeof(XFPropTextCondensed));
				AddProduct(0x0021, typeof(XFPropTextExtended));
				AddProduct(0x0022, typeof(XFPropCharset));
				AddProduct(0x0023, typeof(XFPropFontFamily));
				AddProduct(0x0024, typeof(XFPropTextSizeInTwips));
				AddProduct(0x0025, typeof(XFPropFontScheme));
				AddProduct(0x0026, typeof(XFPropNumberFormat));
				AddProduct(0x0029, typeof(XFPropNumberFormatId));
				AddProduct(0x002a, typeof(XFPropRelativeIndentation));
				AddProduct(0x002b, typeof(XFPropLockedProtection));
				AddProduct(0x002c, typeof(XFPropHiddenProtection));
			}
		}
		static XFPropFactoryImpl impl = new XFPropFactoryImpl();
		public static short GetTypeCodeByType(Type propType) {
			return impl.GetTypeCodeByType(propType);
		}
		public static IXFProperty CreateProperty(short typeCode) {
			return impl.CreateProperty(typeCode);
		}
		public static IXFProperty CreateProperty(XlsReader reader) {
			return impl.CreateProperty(reader);
		}
		public static IXFProperty CreateProperty(Type propType) {
			return impl.CreateProperty(propType);
		}
	}
	public static class XFExtPropertyFactory {
		class XFExtPropFactoryImpl : XFPropFactoryBase {
			protected override void InitializeFactory() {
				AddProduct(0x7fff, typeof(XFPropUnknown));
				AddProduct(0x0004, typeof(XFExtPropForegroundColor));
				AddProduct(0x0005, typeof(XFExtPropBackgroundColor));
				AddProduct(0x0006, typeof(XFExtPropGradient));
				AddProduct(0x0007, typeof(XFExtPropTopBorderColor));
				AddProduct(0x0008, typeof(XFExtPropBottomBorderColor));
				AddProduct(0x0009, typeof(XFExtPropLeftBorderColor));
				AddProduct(0x000a, typeof(XFExtPropRightBorderColor));
				AddProduct(0x000b, typeof(XFExtPropDiagonalBorderColor));
				AddProduct(0x000d, typeof(XFExtPropTextColor));
				AddProduct(0x000e, typeof(XFExtPropFontScheme));
				AddProduct(0x000f, typeof(XFExtPropIndentLevel));
			}
		}
		static XFExtPropFactoryImpl impl = new XFExtPropFactoryImpl();
		public static short GetTypeCodeByType(Type propType) {
			return impl.GetTypeCodeByType(propType);
		}
		public static IXFProperty CreateProperty(short typeCode) {
			return impl.CreateProperty(typeCode);
		}
		public static IXFProperty CreateProperty(XlsReader reader) {
			return impl.CreateProperty(reader);
		}
		public static IXFProperty CreateProperty(Type propType) {
			return impl.CreateProperty(propType);
		}
	}
	#endregion
	#region XFProperties, XFExtProperties, DifferentialFormatProperties, DifferentialFormatExtProperties
	public class XFProperties : XFPropertiesBase {
		protected override IXFProperty CreateProperty(XlsReader reader) {
			return XFPropertyFactory.CreateProperty(reader);
		}
	}
	public class XFExtProperties : XFPropertiesBase {
		protected override IXFProperty CreateProperty(XlsReader reader) {
			return XFExtPropertyFactory.CreateProperty(reader);
		}
	}
	#endregion
}
