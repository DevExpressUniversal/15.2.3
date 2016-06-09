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
	#region IXFContentReceiver
	public interface IXFContentReceiver {
		#region Fill
		void SetFillPatternType(XlPatternType patternType);
		void SetForegroundColor(ColorModelInfo colorInfo);
		void SetBackgroundColor(ColorModelInfo colorInfo);
		void SetXFGradient(GradientFillInfo info);
		void SetXFGradientStop(XFGradStop stop);
		void SetXFGradient(GradientFillInfo info, List<XFGradStop> stops);
		#endregion
		#region Border
		void SetTopBorderColor(ColorModelInfo colorInfo);
		void SetBottomBorderColor(ColorModelInfo colorInfo);
		void SetLeftBorderColor(ColorModelInfo colorInfo);
		void SetRightBorderColor(ColorModelInfo colorInfo);
		void SetDiagonalBorderColor(ColorModelInfo colorInfo);
		void SetVerticalBorderColor(ColorModelInfo colorInfo);
		void SetHorizontalBorderColor(ColorModelInfo colorInfo);
		void SetTopBorderLineStyle(XlBorderLineStyle lineStyle);
		void SetBottomBorderLineStyle(XlBorderLineStyle lineStyle);
		void SetLeftBorderLineStyle(XlBorderLineStyle lineStyle);
		void SetRightBorderLineStyle(XlBorderLineStyle lineStyle);
		void SetDiagonalBorderLineStyle(XlBorderLineStyle lineStyle);
		void SetVerticalBorderLineStyle(XlBorderLineStyle lineStyle);
		void SetHorizontalBorderLineStyle(XlBorderLineStyle lineStyle);
		void SetDiagonalUpBorder(bool flag);
		void SetDiagonalDownBorder(bool flag);
		#endregion
		#region Protection
		void SetLockedProtection(bool flag);
		void SetHiddenProtection(bool flag);
		#endregion
		#region Alignment
		void SetHorizontalAlignment(XlHorizontalAlignment alignment);
		void SetVerticalAlignment(XlVerticalAlignment alignment);
		void SetIndent(int indentation);
		void SetReadingOrder(XlReadingOrder readingOrder);
		void SetRelativeIndent(int relativeIndentation);
		void SetTextRotation(int rotation);
		void SetShrinkToFit(bool flag);
		void SetTextJustifyDistributed(bool flag);
		void SetTextWrapped(bool flag);
		#endregion
		#region Font
		void SetFontName(string fontName);
		void SetCharset(int charset);
		void SetFontFamily(int fontFamily);
		void SetFontUnderline(XlUnderlineType underlineType);
		void SetFontScheme(XlFontSchemeStyles fontScheme);
		void SetFontScript(XlScriptType scriptType);
		void SetTextColor(ColorModelInfo colorInfo);
		void SetTextSizeInTwips(int sizeInTwips);
		void SetTextBold(bool flag);
		void SetTextItalic(bool flag);
		void SetTextStrikethrough(bool flag);
		void SetTextCondensed(bool flag);
		void SetTextExtended(bool flag);
		void SetHasOutlineStyle(bool flag);
		void SetHasShadowStyle(bool flag);
		#endregion
		#region NumberFormat
		void SetNumberFormatCode(string numberFormatCode);
		void SetNumberFormatId(int numberFormatId);
		#endregion
	}
	#endregion
	#region IXFProperty
	public interface IXFProperty {
		void Read(XlsReader reader);
		void Write(BinaryWriter writer);
		int GetSize();
		void ApplyContent(IXFContentReceiver contentReceiver);
	}
	#endregion
	#region XFPropertyBase
	public abstract class XFPropertyBase : IXFProperty {
		#region Fields
		const short headerSize = 4;
		short dataSize;
		#endregion
		#region Properties
		protected internal short DataSize { get { return dataSize; } }
		#endregion
		#region IXFProperty Members
		public void Read(XlsReader reader) {
			this.dataSize = (short)(reader.ReadInt16() - headerSize);
			long initialPosition = reader.Position;
			long finalPosition = initialPosition + this.dataSize;
			ReadCore(reader);
			long actualPosition = reader.Position;
			if(actualPosition != finalPosition)
				throw new Exception(
					string.Format("Read failure: initial/final/actual positions = {0}/{1}/{2}, property={3}",
					initialPosition, finalPosition, actualPosition, this.GetType().ToString()));
		}
		public void Write(BinaryWriter writer) {
			short typeCode = GetTypeCode();
			writer.Write(typeCode);
			short size = (short)GetSize();
			writer.Write(size);
			WriteCore(writer);
		}
		public int GetSize() {
			return GetSizeCore() + headerSize;
		}
		public virtual void ApplyContent(IXFContentReceiver contentReceiver) {
		}
		#endregion
		protected virtual short GetTypeCode() {
			return XFPropertyFactory.GetTypeCodeByType(GetType());
		}
		protected abstract void ReadCore(XlsReader reader);
		protected abstract void WriteCore(BinaryWriter writer);
		protected abstract short GetSizeCore();
		protected internal XlFontSchemeStyles ByteToFontScheme(byte value) {
			if(value == 0x01)
				return XlFontSchemeStyles.Major;
			if(value == 0x02)
				return XlFontSchemeStyles.Minor;
			return XlFontSchemeStyles.None;
		}
		protected internal byte FontSchemeToByte(XlFontSchemeStyles value, bool hasValue) {
			if(hasValue) {
				switch(value) {
					case XlFontSchemeStyles.None:
						return 0x00;
					case XlFontSchemeStyles.Major:
						return 0x01;
					case XlFontSchemeStyles.Minor:
						return 0x02;
				}
			}
			return 0xff;
		}
	}
	#endregion
	#region XFPropColorBase
	public enum XColorType {
		Auto = 0,
		Indexed = 1,
		Rgb = 2,
		Themed = 3,
		Ninched = 4
	}
	public abstract class XFPropColorBase : XFPropertyBase {
		ColorModelInfo colorInfo = new ColorModelInfo();
		bool hasColorValue;
		protected bool HasColorValue { get { return hasColorValue; } }
		public ColorModelInfo ColorInfo {
			get { return colorInfo; }
			set {
				Guard.ArgumentNotNull(value, "ColorInfo value");
				this.colorInfo = value;
				this.hasColorValue = true;
			}
		}
		protected override void ReadCore(XlsReader reader) {
			XColorType colorType = (XColorType)(reader.ReadByte() >> 1);
			int colorIndex = reader.ReadByte();
			double tint = reader.ReadInt16() / 32767.0;
			int red = reader.ReadByte();
			int green = reader.ReadByte();
			int blue = reader.ReadByte();
			reader.ReadByte(); 
			switch (colorType) {
				case XColorType.Auto:
					colorInfo.Auto = true;
					break;
				case XColorType.Indexed:
					colorInfo.ColorIndex = colorIndex;
					break;
				case XColorType.Rgb:
					colorInfo.Rgb = DXColor.FromArgb(0xff, red, green, blue);
					break;
				case XColorType.Themed:
					colorInfo.Theme = new ThemeColorIndex(colorIndex);
					colorInfo.Tint = tint;
					break;
			}
			hasColorValue = true;
		}
		protected override void WriteCore(BinaryWriter writer) {
			XColorType colorType = XColorType.Ninched;
			byte colorIndex = 0;
			if(colorInfo.ColorType == ColorType.Auto)
				colorType = XColorType.Auto;
			else if(colorInfo.ColorType == ColorType.Index) {
				colorType = XColorType.Indexed;
				colorIndex = (byte)colorInfo.ColorIndex;
			}
			else if(colorInfo.ColorType == ColorType.Rgb)
				colorType = XColorType.Rgb;
			else if(colorInfo.ColorType == ColorType.Theme) {
				colorType = XColorType.Themed;
				colorIndex = (byte)colorInfo.Theme.ToInt();
			}
			byte bitwiseField = (byte)(((byte)colorType << 1) | 0x01);
			writer.Write(bitwiseField);
			writer.Write(colorIndex);
			writer.Write((short)(colorInfo.Tint * 32767));
			Color argb = colorInfo.Rgb;
			writer.Write((byte)argb.R);
			writer.Write((byte)argb.G);
			writer.Write((byte)argb.B);
			writer.Write((byte)0xff);
		}
		protected override short GetSizeCore() {
			return 8;
		}
	}
	#endregion
	#region XFPropBorderBase
	public abstract class XFPropBorderBase : XFPropColorBase {
		public XlBorderLineStyle LineStyle { get; set; }
		protected override void ReadCore(XlsReader reader) {
			base.ReadCore(reader);
			LineStyle = (XlBorderLineStyle)reader.ReadUInt16();
		}
		protected override void WriteCore(BinaryWriter writer) {
			base.WriteCore(writer);
			writer.Write((ushort)LineStyle);
		}
		protected override short GetSizeCore() {
			return (short)(base.GetSizeCore() + 2);
		}
	}
	#endregion
	#region XFPropBoolBase
	public abstract class XFPropBoolBase : XFPropertyBase {
		public bool Value { get; set; }
		protected override void ReadCore(XlsReader reader) {
			Value = Convert.ToBoolean(reader.ReadByte());
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write(Convert.ToByte(Value));
		}
		protected override short GetSizeCore() {
			return 1;
		}
	}
	#endregion
	#region XFPropByteBase
	public abstract class XFPropByteBase : XFPropertyBase {
		int value;
		public int Value {
			get { return value; }
			set {
				ValueChecker.CheckValue(value, 0, 255);
				this.value = value;
			}
		}
		protected override void ReadCore(XlsReader reader) {
			Value = reader.ReadByte();
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((byte)Value);
		}
		protected override short GetSizeCore() {
			return 1;
		}
	}
	#endregion
	#region XFPropUShortBase
	public abstract class XFPropUShortBase : XFPropertyBase {
		int value;
		public int Value {
			get { return value; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue);
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
	}
	#endregion
	#region XFExtPropertyBase
	public abstract class XFExtPropertyBase : XFPropertyBase {
		protected override short GetTypeCode() {
			return XFExtPropertyFactory.GetTypeCodeByType(GetType());
		}
	}
	#endregion
	#region XFExtPropFullColorBase
	public abstract class XFExtPropFullColorBase : XFExtPropertyBase {
		ColorModelInfo colorInfo = new ColorModelInfo();
		public ColorModelInfo ColorInfo {
			get { return colorInfo; }
			set {
				Guard.ArgumentNotNull(value, "ColorInfo value");
				this.colorInfo = value;
			}
		}
		protected override void ReadCore(XlsReader reader) {
			XColorType colorType = (XColorType)reader.ReadUInt16();
			double tint = reader.ReadInt16() / 32767.0;
			switch(colorType) {
				case XColorType.Auto:
					colorInfo.Auto = true;
					reader.ReadInt32();
					break;
				case XColorType.Indexed:
					colorInfo.ColorIndex = reader.ReadInt32();
					break;
				case XColorType.Rgb:
					int red = reader.ReadByte();
					int green = reader.ReadByte();
					int blue = reader.ReadByte();
					reader.ReadByte(); 
					colorInfo.Rgb = DXColor.FromArgb(0xff, red, green, blue);
					break;
				case XColorType.Themed:
					colorInfo.Theme = new ThemeColorIndex(reader.ReadInt32());
					colorInfo.Tint = tint;
					break;
				default:
					reader.ReadInt32();
					break;
			}
			reader.ReadUInt64(); 
		}
		protected override void WriteCore(BinaryWriter writer) {
			XColorType colorType = XColorType.Ninched;
			if(colorInfo.ColorType == ColorType.Auto)
				colorType = XColorType.Auto;
			else if(colorInfo.ColorType == ColorType.Index) {
				colorType = XColorType.Indexed;
			}
			else if(colorInfo.ColorType == ColorType.Rgb)
				colorType = XColorType.Rgb;
			else if(colorInfo.ColorType == ColorType.Theme) {
				colorType = XColorType.Themed;
			}
			writer.Write((ushort)colorType);
			writer.Write((short)(colorInfo.Tint * 32767));
			switch(colorType) {
				case XColorType.Indexed:
					writer.Write(colorInfo.ColorIndex);
					break;
				case XColorType.Rgb:
					Color argb = colorInfo.Rgb;
					writer.Write((byte)argb.R);
					writer.Write((byte)argb.G);
					writer.Write((byte)argb.B);
					writer.Write((byte)0xff);
					break;
				case XColorType.Themed:
					writer.Write(colorInfo.Theme.ToInt());
					break;
				default:
					writer.Write((int)0);
					break;
			}
			writer.Write((long)0); 
		}
		protected override short GetSizeCore() {
			return 16;
		}
	}
	#endregion
	#region XFPropFactoryBase
	abstract class XFPropFactoryBase {
		#region XFPropInfo
		internal class XFPropInfo {
			short typeCode;
			Type propType;
			public XFPropInfo(short typeCode, Type propType) {
				this.typeCode = typeCode;
				this.propType = propType;
			}
			public short TypeCode { get { return this.typeCode; } }
			public Type PropType { get { return this.propType; } }
		}
		#endregion
		List<XFPropInfo> infos;
		Dictionary<short, ConstructorInfo> propTypes;
		Dictionary<Type, short> typeCodes;
		protected XFPropFactoryBase() {
			infos = new List<XFPropInfo>();
			propTypes = new Dictionary<short, ConstructorInfo>();
			typeCodes = new Dictionary<Type, short>();
			InitializeFactory();
			for(int i = 0; i < infos.Count; i++) {
				propTypes.Add(infos[i].TypeCode, infos[i].PropType.GetConstructor(new Type[] { }));
				typeCodes.Add(infos[i].PropType, infos[i].TypeCode);
			}
		}
		protected abstract void InitializeFactory();
		protected void AddProduct(short typeCode, Type propType) {
			infos.Add(new XFPropInfo(typeCode, propType));
		}
		public short GetTypeCodeByType(Type propType) {
			short typeCode;
			if(!typeCodes.TryGetValue(propType, out typeCode))
				typeCode = 0x7fff;
			return typeCode;
		}
		public IXFProperty CreateProperty(short typeCode) {
			if(!propTypes.ContainsKey(typeCode))
				typeCode = 0x7fff;
			ConstructorInfo propConstructor = propTypes[typeCode];
			IXFProperty propInstance = propConstructor.Invoke(new object[] { }) as IXFProperty;
			return propInstance;
		}
		public IXFProperty CreateProperty(XlsReader reader) {
			short typeCode = reader.ReadInt16();
			if(!propTypes.ContainsKey(typeCode))
				typeCode = 0x7fff;
			ConstructorInfo propConstructor = propTypes[typeCode];
			IXFProperty propInstance = propConstructor.Invoke(new object[] { }) as IXFProperty;
			return propInstance;
		}
		public IXFProperty CreateProperty(Type propType) {
			return CreateProperty(GetTypeCodeByType(propType));
		}
	}
	#endregion
	#region XFPropertiesBase
	public abstract class XFPropertiesBase : IXFProperty, IEnumerable<IXFProperty> {
		readonly List<IXFProperty> items = new List<IXFProperty>();
		#region Properties
		public int Count { get { return items.Count; } }
		public IXFProperty this[int index] {
			get { return this.items[index]; }
			set { this.items[index] = value; }
		}
		#endregion
		public void Add(IXFProperty item) {
			Guard.ArgumentNotNull(item, "item");
			this.items.Add(item);
		}
		public void Clear() {
			this.items.Clear();
		}
		#region IXFProperty Members
		public void Read(XlsReader reader) {
			Guard.ArgumentNotNull(reader, "reader");
			Clear();
			reader.ReadUInt16(); 
			int count = reader.ReadUInt16();
			for(int i = 0; i < count; i++) {
				IXFProperty item = CreateProperty(reader);
				item.Read(reader);
				if(item is XFPropUnknown) continue;
				items.Add(item);
			}
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
		public void ApplyContent(IXFContentReceiver contentReceiver) {
			Guard.ArgumentNotNull(contentReceiver, "contentReceiver");
			int count = items.Count;
			for(int i = 0; i < count; i++)
				items[i].ApplyContent(contentReceiver);
		}
		#endregion
		#region IEnumerable<IXFProperty> Members
		public IEnumerator<IXFProperty> GetEnumerator() {
			return ((IEnumerable<IXFProperty>)this.items).GetEnumerator();
		}
		#endregion
		#region IEnumerable Members
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return ((System.Collections.IEnumerable)this.items).GetEnumerator();
		}
		#endregion
		protected abstract IXFProperty CreateProperty(XlsReader reader);
	}
	#endregion
}
