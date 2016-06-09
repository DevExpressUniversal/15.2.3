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
using System.Text;
using DevExpress.XtraExport.Xlsx;
using DevExpress.Utils;
using DevExpress.Export.Xl;
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraExport.Xls {
	#region XfExtType
	public static class XfExtType {
		public const short ForegroundColor = 0x0004;
		public const short BackgroundColor = 0x0005;
		public const short GradientFill = 0x0006;
		public const short TopBorderColor = 0x0007;
		public const short BottomBorderColor = 0x0008;
		public const short LeftBorderColor = 0x0009;
		public const short RightBorderColor = 0x000a;
		public const short DiagonalBorderColor = 0x000b;
		public const short TextColor = 0x000d;
		public const short FontScheme = 0x000e;
		public const short IndentLevel = 0x000f;
	}
	#endregion
	#region XfPropType
	public static class XfPropType {
		public const short FillPattern = 0x0000;
		public const short ForegroundColor = 0x0001;
		public const short BackgroundColor = 0x0002;
		public const short GradientFill = 0x0003;
		public const short GradientStop = 0x0004;
		public const short TextColor = 0x0005;
		public const short TopBorder = 0x0006;
		public const short BottomBorder = 0x0007;
		public const short LeftBorder = 0x0008;
		public const short RightBorder = 0x0009;
		public const short DiagonalBorder = 0x000a;
		public const short VerticalBorder = 0x000b;
		public const short HorizontalBorder = 0x000c;
		public const short DiagonalUpBorder = 0x000d;
		public const short DiagonalDownBorder = 0x000e;
		public const short HorizontalAlign = 0x000f;
		public const short VerticalAlign = 0x0010;
		public const short TextRotation = 0x0011;
		public const short AbsoluteTextIndent = 0x0012;
		public const short ReadingOrder = 0x0013;
		public const short Wrapped = 0x0014;
		public const short JustifyDistributed = 0x0015;
		public const short ShrinkToFit = 0x0016;
		public const short Merged = 0x0017;
		public const short FontName = 0x0018;
		public const short Bold = 0x0019;
		public const short Underline = 0x001a;
		public const short Script = 0x001b;
		public const short Italic = 0x001c;
		public const short StrikeThrough = 0x001d;
		public const short Outline = 0x001e;
		public const short Shadow = 0x001f;
		public const short Condensed = 0x0020;
		public const short Extended = 0x0021;
		public const short Charset = 0x0022;
		public const short FontFamily = 0x0023;
		public const short FontSize = 0x0024;
		public const short FontScheme = 0x0025;
		public const short NumberFormat = 0x0026;
		public const short NumberFormatId = 0x0029;
		public const short IndentLevel = 0x002a;
		public const short Locked = 0x002b;
		public const short Hidden = 0x002c;
	}
	#endregion
	#region XfPropBase
	public abstract class XfPropBase {
		const short headerSize = 4;
		public short TypeCode { get; private set; }
		protected XfPropBase(short typeCode) {
			TypeCode = typeCode;
		}
		public void Write(BinaryWriter writer) {
			writer.Write(TypeCode);
			short size = (short)GetSize();
			writer.Write(size);
			WriteCore(writer);
		}
		public int GetSize() {
			return GetSizeCore() + headerSize;
		}
		protected abstract void WriteCore(BinaryWriter writer);
		protected abstract int GetSizeCore();
	}
	#endregion
	#region XfPropColor
	public class XfPropColor : XfPropBase {
		public XlColor Color { get; private set; }
		public XfPropColor(short typeCode, XlColor color)
			: base(typeCode) {
			Color = color;
		}
		protected override void WriteCore(BinaryWriter writer) {
			byte colorType = 4; 
			byte colorIndex = 0;
			if(Color.ColorType == XlColorType.Auto)
				colorType = 0;
			else if(Color.ColorType == XlColorType.Rgb)
				colorType = 2;
			else if(Color.ColorType == XlColorType.Theme) {
				colorType = 3;
				colorIndex = (byte)Color.ThemeColor;
			}
			byte bitwiseField = (byte)(((byte)colorType << 1) | 0x01);
			writer.Write(bitwiseField);
			writer.Write(colorIndex);
			writer.Write((short)(Color.Tint * 32767));
			Color argb = Color.Rgb;
			writer.Write((byte)argb.R);
			writer.Write((byte)argb.G);
			writer.Write((byte)argb.B);
			writer.Write((byte)0xff);
		}
		protected override int GetSizeCore() {
			return 8;
		}
	}
	#endregion
	#region XfPropBorder
	public class XfPropBorder : XfPropColor {
		public XlBorderLineStyle LineStyle { get; private set; }
		public XfPropBorder(short typeCode, XlColor color, XlBorderLineStyle lineStyle)
			: base(typeCode, color) {
			LineStyle = lineStyle;
		}
		protected override void WriteCore(BinaryWriter writer) {
			base.WriteCore(writer);
			writer.Write((ushort)LineStyle);
		}
		protected override int GetSizeCore() {
			return base.GetSizeCore() + 2;
		}
	}
	#endregion
	#region XfPropBool
	public class XfPropBool : XfPropBase {
		public bool Value { get; private set; }
		public XfPropBool(short typeCode, bool value)
			: base(typeCode) {
			Value = value;
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write(Convert.ToByte(Value));
		}
		protected override int GetSizeCore() {
			return 1;
		}
	}
	#endregion
	#region XfPropByte
	public class XfPropByte : XfPropBase {
		public byte Value { get; private set; }
		public XfPropByte(short typeCode, byte value)
			: base(typeCode) {
			Value = value;
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write(Value);
		}
		protected override int GetSizeCore() {
			return 1;
		}
	}
	#endregion
	#region XfPropUInt16
	public class XfPropUInt16 : XfPropBase {
		public int Value { get; private set; }
		public XfPropUInt16(short typeCode, int value)
			: base(typeCode) {
			Value = value;
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((ushort)Value);
		}
		protected override int GetSizeCore() {
			return 2;
		}
	}
	#endregion
	#region XfPropInt32
	public class XfPropInt32 : XfPropBase {
		public int Value { get; private set; }
		public XfPropInt32(short typeCode, int value)
			: base(typeCode) {
			Value = value;
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write(Value);
		}
		protected override int GetSizeCore() {
			return 4;
		}
	}
	#endregion
	#region XfPropString
	public class XfPropString : XfPropBase {
		LPWideString stringValue = new LPWideString();
		public XfPropString(short typeCode, string value) 
			: base(typeCode) {
			Value = value;
		}
		public string Value {
			get { return stringValue.Value; }
			set { stringValue.Value = value; }
		}
		protected override void WriteCore(BinaryWriter writer) {
			this.stringValue.Write(writer);
		}
		protected override int GetSizeCore() {
			return this.stringValue.Length;
		}
	}
	#endregion
	#region XfPropFullColor
	public class XfPropFullColor : XfPropColor {
		public XfPropFullColor(short typeCode, XlColor color)
			: base(typeCode, color) {
		}
		protected override void WriteCore(BinaryWriter writer) {
			ushort colorType = 4; 
			if(Color.ColorType == XlColorType.Auto)
				colorType = 0;
			else if(Color.ColorType == XlColorType.Rgb)
				colorType = 2;
			else if(Color.ColorType == XlColorType.Theme) {
				colorType = 3;
			}
			writer.Write(colorType);
			writer.Write((short)(Color.Tint * 32767));
			switch(colorType) {
				case 2:
					Color argb = Color.Rgb;
					writer.Write((byte)argb.R);
					writer.Write((byte)argb.G);
					writer.Write((byte)argb.B);
					writer.Write((byte)0xff);
					break;
				case 3:
					writer.Write((int)Color.ThemeColor);
					break;
				default:
					writer.Write((int)0);
					break;
			}
			writer.Write((long)0); 
		}
		protected override int GetSizeCore() {
			return 16;
		}
	}
	#endregion
	#region XfProperties
	public class XfProperties : IEnumerable<XfPropBase> {
		readonly List<XfPropBase> items = new List<XfPropBase>();
		#region Properties
		public int Count { get { return items.Count; } }
		public XfPropBase this[int index] {
			get { return this.items[index]; }
		}
		#endregion
		public void Add(XfPropBase item) {
			Guard.ArgumentNotNull(item, "item");
			this.items.Add(item);
		}
		public void Clear() {
			this.items.Clear();
		}
		public void Write(BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			int count = items.Count;
			writer.Write((ushort)0); 
			writer.Write((ushort)count);
			for(int i = 0; i < count; i++)
				items[i].Write(writer);
		}
		public int GetSize() {
			int result = 4;
			int count = items.Count;
			for(int i = 0; i < count; i++)
				result += items[i].GetSize();
			return result;
		}
		#region IEnumerable<XfPropBase> Members
		public IEnumerator<XfPropBase> GetEnumerator() {
			return ((IEnumerable<XfPropBase>)this.items).GetEnumerator();
		}
		#endregion
		#region IEnumerable Members
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return ((System.Collections.IEnumerable)this.items).GetEnumerator();
		}
		#endregion
	}
	#endregion
}
