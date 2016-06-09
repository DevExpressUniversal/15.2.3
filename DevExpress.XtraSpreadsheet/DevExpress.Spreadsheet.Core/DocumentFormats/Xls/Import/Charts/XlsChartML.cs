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
using System.Globalization;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraExport.Xls;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsChartExtPropTypeCode
	public enum XlsChartExtPropTypeCode {
		ScaleMax = 0x00550003,
		ScaleMin = 0x00560003,
		LogBase = 0x00000003,
		Style = 0x00030004,
		ThemeOverride = 0x00330007,
		ColorMapOverride = 0x00340007,
		NoMultiLvlLbl = 0x002e0002,
		TickLabelSkip = 0x00510004,
		TickMarkSkip = 0x00520004,
		MajorUnit = 0x00530003,
		MinorUnit = 0x00540003,
		TickLabelPos = 0x005c0006,
		BaseTimeUnit = 0x005f0006,
		FormatCode = 0x00640005,
		MajorTimeUnit = 0x006a0006,
		MinorTimeUnit = 0x006b0006,
		ShowDLblsOverMax = 0x005b0002,
		BackWallThickness = 0x00350004,
		FloorThickness = 0x00360004,
		DispBlankAs = 0x00660006,
		StartSurface = 0x00590000,
		EndSurface = 0x00590001,
		ShapeProps = 0x001e0007,
		TextProps = 0x00200007,
		Overlay = 0x002f0002,
		PieCombo = 0x005e0002,
		RightAngAxOff = 0x00500002,
		Perspective = 0x004d0004,
		RotationX = 0x004e0004,
		RotationY = 0x004f0004,
		HeightPercent = 0x00650003,
		CultureCode = 0x00760005,
		Symbol = 0x00220006,
		Eof = 0x00000000
	}
	#endregion
	#region IXlsChartExtProperty
	public interface IXlsChartExtProperty {
		int GetSize();
		void Read(BinaryReader reader);
		void Write(BinaryWriter writer);
		void Visit(IXlsChartExtPropertyVisitor visitor);
	}
	#endregion
	#region IXlsChartExtPropertyVisitor
	public interface IXlsChartExtPropertyVisitor {
		void Visit(XlsChartExtPropScaleMax item);
		void Visit(XlsChartExtPropScaleMin item);
		void Visit(XlsChartExtPropLogBase item);
		void Visit(XlsChartExtPropStyle item);
		void Visit(XlsChartExtPropThemeOverride item);
		void Visit(XlsChartExtPropColorMapOverride item);
		void Visit(XlsChartExtPropNoMultiLvlLbl item);
		void Visit(XlsChartExtPropTickLabelSkip item);
		void Visit(XlsChartExtPropTickMarkSkip item);
		void Visit(XlsChartExtPropMajorUnit item);
		void Visit(XlsChartExtPropMinorUnit item);
		void Visit(XlsChartExtPropTickLabelPos item);
		void Visit(XlsChartExtPropBaseTimeUnit item);
		void Visit(XlsChartExtPropFormatCode item);
		void Visit(XlsChartExtPropMajorTimeUnit item);
		void Visit(XlsChartExtPropMinorTimeUnit item);
		void Visit(XlsChartExtPropShowDLblsOverMax item);
		void Visit(XlsChartExtPropBackWallThickness item);
		void Visit(XlsChartExtPropFloorThickness item);
		void Visit(XlsChartExtPropDispBlankAs item);
		void Visit(XlsChartExtPropStartSurface item);
		void Visit(XlsChartExtPropEndSurface item);
		void Visit(XlsChartExtPropShapeProps item);
		void Visit(XlsChartExtPropTextProps item);
		void Visit(XlsChartExtPropOverlay item);
		void Visit(XlsChartExtPropPieCombo item);
		void Visit(XlsChartExtPropRightAngAxOff item);
		void Visit(XlsChartExtPropPerspective item);
		void Visit(XlsChartExtPropRotationX item);
		void Visit(XlsChartExtPropRotationY item);
		void Visit(XlsChartExtPropHeightPercent item);
		void Visit(XlsChartExtPropCultureCode item);
		void Visit(XlsChartExtPropSymbol item);
	}
	#endregion
	#region XlsChartExtPropParent
	public enum XlsChartExtPropParent {
		ValueAxis = 0x0001,
		Chart = 0x0002,
		CategoryAxis = 0x0004,
		ChartSpace = 0x0005,
		Legend = 0x000f,
		DataFormat = 0x0013,
		PlotArea = 0x0016,
		AttachedLabel = 0x0019,
		View3D = 0x0037
	}
	#endregion
	#region Abstract properties
	public abstract class XlsChartExtPropBase : IXlsChartExtProperty {
		protected XlsChartExtPropBase(XlsChartExtPropTypeCode typeCode) {
			TypeCode = typeCode;
		}
		public XlsChartExtPropTypeCode TypeCode { get; private set; }
		#region IXlsChartExtProperty Members
		public abstract int GetSize();
		public abstract void Read(BinaryReader reader);
		public virtual void Write(BinaryWriter writer) {
			writer.Write((int)TypeCode);
		}
		public virtual void Visit(IXlsChartExtPropertyVisitor visitor) {
		}
		#endregion
	}
	public abstract class XlsChartExtPropEmpty : XlsChartExtPropBase {
		protected XlsChartExtPropEmpty(XlsChartExtPropTypeCode typeCode)
			: base(typeCode) {
		}
		public override int GetSize() { return 4; }
		public override void Read(BinaryReader reader) {
		}
	}
	#endregion
	#region Typed properties
	public class XlsChartExtPropBoolean : XlsChartExtPropBase {
		public XlsChartExtPropBoolean(XlsChartExtPropTypeCode typeCode)
			: base(typeCode) {
		}
		public bool Value { get; set; }
		public override int GetSize() { return 6; }
		public override void Read(BinaryReader reader) {
			Value = reader.ReadBoolean();
			reader.ReadByte(); 
		}
		public override void Write(BinaryWriter writer) {
			base.Write(writer);
			writer.Write(Value);
			writer.Write((byte)0);
		}
	}
	public class XlsChartExtPropShort : XlsChartExtPropBase {
		public XlsChartExtPropShort(XlsChartExtPropTypeCode typeCode)
			: base(typeCode) {
		}
		public int Value { get; set; }
		public override int GetSize() { return 6; }
		public override void Read(BinaryReader reader) {
			Value = reader.ReadUInt16();
		}
		public override void Write(BinaryWriter writer) {
			base.Write(writer);
			writer.Write((ushort)Value);
		}
	}
	public class XlsChartExtPropInt : XlsChartExtPropBase {
		public XlsChartExtPropInt(XlsChartExtPropTypeCode typeCode)
			: base(typeCode) {
		}
		public int Value { get; set; }
		public override int GetSize() { return 8; }
		public override void Read(BinaryReader reader) {
			Value = reader.ReadInt32();
		}
		public override void Write(BinaryWriter writer) {
			base.Write(writer);
			writer.Write(Value);
		}
	}
	public class XlsChartExtPropDouble : XlsChartExtPropBase {
		public XlsChartExtPropDouble(XlsChartExtPropTypeCode typeCode)
			: base(typeCode) {
		}
		public double Value { get; set; }
		public override int GetSize() { return 16; }
		public override void Read(BinaryReader reader) {
			reader.ReadUInt32(); 
			Value = reader.ReadDouble();
		}
		public override void Write(BinaryWriter writer) {
			base.Write(writer);
			writer.Write((int)0); 
			writer.Write(Value);
		}
	}
	public class XlsChartExtPropBlob : XlsChartExtPropBase {
		public XlsChartExtPropBlob(XlsChartExtPropTypeCode typeCode)
			: base(typeCode) {
		}
		public byte[] Data { get; set; }
		public override int GetSize() { return GetDataSize() + 8; }
		public override void Read(BinaryReader reader) {
			int dataSize = reader.ReadInt32();
			Data = reader.ReadBytes(dataSize);
		}
		public override void Write(BinaryWriter writer) {
			base.Write(writer);
			int dataSize = GetDataSize();
			writer.Write(dataSize);
			if (dataSize > 0)
				writer.Write(Data);
		}
		int GetDataSize() {
			return Data != null ? Data.Length : 0;
		}
	}
	public class XlsChartExtPropString : XlsChartExtPropBase {
		string value = string.Empty;
		public XlsChartExtPropString(XlsChartExtPropTypeCode typeCode)
			: base(typeCode) {
		}
		public string Value {
			get { return value; }
			set {
				if (string.IsNullOrEmpty(value))
					this.value = string.Empty;
				else
					this.value = value;
			}
		}
		public override int GetSize() { return value.Length * 2 + 8; }
		public override void Read(BinaryReader reader) {
			int charCount = reader.ReadInt32();
			byte[] buf = reader.ReadBytes(charCount * 2);
			this.value = XLStringEncoder.GetEncoding(true).GetString(buf, 0, buf.Length);
		}
		public override void Write(BinaryWriter writer) {
			base.Write(writer);
			int charCount = this.value.Length;
			writer.Write(charCount);
			if (charCount > 0) {
				byte[] buf = XLStringEncoder.GetEncoding(true).GetBytes(this.value);
				writer.Write(buf);
			}
		}
	}
	#endregion
	#region Concrete properties
	public class XlsChartExtPropScaleMax : XlsChartExtPropDouble {
		public XlsChartExtPropScaleMax() : base(XlsChartExtPropTypeCode.ScaleMax) { }
		public override void Visit(IXlsChartExtPropertyVisitor visitor) { visitor.Visit(this); }
	}
	public class XlsChartExtPropScaleMin : XlsChartExtPropDouble {
		public XlsChartExtPropScaleMin() : base(XlsChartExtPropTypeCode.ScaleMin) { }
		public override void Visit(IXlsChartExtPropertyVisitor visitor) { visitor.Visit(this); }
	}
	public class XlsChartExtPropLogBase : XlsChartExtPropDouble {
		public XlsChartExtPropLogBase() : base(XlsChartExtPropTypeCode.LogBase) { }
		public override void Visit(IXlsChartExtPropertyVisitor visitor) { visitor.Visit(this); }
	}
	public class XlsChartExtPropStyle : XlsChartExtPropInt {
		public XlsChartExtPropStyle() : base(XlsChartExtPropTypeCode.Style) { }
		public override void Visit(IXlsChartExtPropertyVisitor visitor) { visitor.Visit(this); }
	}
	public class XlsChartExtPropThemeOverride : XlsChartExtPropBlob {
		public XlsChartExtPropThemeOverride() : base(XlsChartExtPropTypeCode.ThemeOverride) { }
		public override void Visit(IXlsChartExtPropertyVisitor visitor) { visitor.Visit(this); }
	}
	public class XlsChartExtPropColorMapOverride : XlsChartExtPropBlob {
		public XlsChartExtPropColorMapOverride() : base(XlsChartExtPropTypeCode.ColorMapOverride) { }
		public override void Visit(IXlsChartExtPropertyVisitor visitor) { visitor.Visit(this); }
	}
	public class XlsChartExtPropNoMultiLvlLbl : XlsChartExtPropBoolean {
		public XlsChartExtPropNoMultiLvlLbl() : base(XlsChartExtPropTypeCode.NoMultiLvlLbl) { }
		public override void Visit(IXlsChartExtPropertyVisitor visitor) { visitor.Visit(this); }
	}
	public class XlsChartExtPropTickLabelSkip : XlsChartExtPropInt {
		public XlsChartExtPropTickLabelSkip() : base(XlsChartExtPropTypeCode.TickLabelSkip) { }
		public override void Visit(IXlsChartExtPropertyVisitor visitor) { visitor.Visit(this); }
	}
	public class XlsChartExtPropTickMarkSkip : XlsChartExtPropInt {
		public XlsChartExtPropTickMarkSkip() : base(XlsChartExtPropTypeCode.TickMarkSkip) { }
		public override void Visit(IXlsChartExtPropertyVisitor visitor) { visitor.Visit(this); }
	}
	public class XlsChartExtPropMajorUnit : XlsChartExtPropDouble {
		public XlsChartExtPropMajorUnit() : base(XlsChartExtPropTypeCode.MajorUnit) { }
		public override void Visit(IXlsChartExtPropertyVisitor visitor) { visitor.Visit(this); }
	}
	public class XlsChartExtPropMinorUnit : XlsChartExtPropDouble {
		public XlsChartExtPropMinorUnit() : base(XlsChartExtPropTypeCode.MinorUnit) { }
		public override void Visit(IXlsChartExtPropertyVisitor visitor) { visitor.Visit(this); }
	}
	public class XlsChartExtPropTickLabelPos : XlsChartExtPropShort {
		public XlsChartExtPropTickLabelPos() : base(XlsChartExtPropTypeCode.TickLabelPos) { }
		public override void Visit(IXlsChartExtPropertyVisitor visitor) { visitor.Visit(this); }
	}
	public class XlsChartExtPropBaseTimeUnit : XlsChartExtPropShort {
		public XlsChartExtPropBaseTimeUnit() : base(XlsChartExtPropTypeCode.BaseTimeUnit) { }
		public override void Visit(IXlsChartExtPropertyVisitor visitor) { visitor.Visit(this); }
	}
	public class XlsChartExtPropFormatCode : XlsChartExtPropString {
		public XlsChartExtPropFormatCode() : base(XlsChartExtPropTypeCode.FormatCode) { }
		public override void Visit(IXlsChartExtPropertyVisitor visitor) { visitor.Visit(this); }
	}
	public class XlsChartExtPropMajorTimeUnit : XlsChartExtPropShort {
		public XlsChartExtPropMajorTimeUnit() : base(XlsChartExtPropTypeCode.MajorTimeUnit) { }
		public override void Visit(IXlsChartExtPropertyVisitor visitor) { visitor.Visit(this); }
	}
	public class XlsChartExtPropMinorTimeUnit : XlsChartExtPropShort {
		public XlsChartExtPropMinorTimeUnit() : base(XlsChartExtPropTypeCode.MinorTimeUnit) { }
		public override void Visit(IXlsChartExtPropertyVisitor visitor) { visitor.Visit(this); }
	}
	public class XlsChartExtPropShowDLblsOverMax : XlsChartExtPropBoolean {
		public XlsChartExtPropShowDLblsOverMax() : base(XlsChartExtPropTypeCode.ShowDLblsOverMax) { }
		public override void Visit(IXlsChartExtPropertyVisitor visitor) { visitor.Visit(this); }
	}
	public class XlsChartExtPropBackWallThickness : XlsChartExtPropInt {
		public XlsChartExtPropBackWallThickness() : base(XlsChartExtPropTypeCode.BackWallThickness) { }
		public override void Visit(IXlsChartExtPropertyVisitor visitor) { visitor.Visit(this); }
	}
	public class XlsChartExtPropFloorThickness : XlsChartExtPropInt {
		public XlsChartExtPropFloorThickness() : base(XlsChartExtPropTypeCode.FloorThickness) { }
		public override void Visit(IXlsChartExtPropertyVisitor visitor) { visitor.Visit(this); }
	}
	public class XlsChartExtPropDispBlankAs : XlsChartExtPropShort {
		public XlsChartExtPropDispBlankAs() : base(XlsChartExtPropTypeCode.DispBlankAs) { }
		public override void Visit(IXlsChartExtPropertyVisitor visitor) { visitor.Visit(this); }
	}
	public class XlsChartExtPropStartSurface : XlsChartExtPropEmpty {
		public XlsChartExtPropStartSurface() : base(XlsChartExtPropTypeCode.StartSurface) { }
		public override void Visit(IXlsChartExtPropertyVisitor visitor) { visitor.Visit(this); }
	}
	public class XlsChartExtPropEndSurface : XlsChartExtPropEmpty {
		public XlsChartExtPropEndSurface() : base(XlsChartExtPropTypeCode.EndSurface) { }
		public override void Visit(IXlsChartExtPropertyVisitor visitor) { visitor.Visit(this); }
	}
	public class XlsChartExtPropShapeProps : XlsChartExtPropBlob {
		public XlsChartExtPropShapeProps() : base(XlsChartExtPropTypeCode.ShapeProps) { }
		public override void Visit(IXlsChartExtPropertyVisitor visitor) { visitor.Visit(this); }
	}
	public class XlsChartExtPropTextProps : XlsChartExtPropBlob {
		public XlsChartExtPropTextProps() : base(XlsChartExtPropTypeCode.TextProps) { }
		public override void Visit(IXlsChartExtPropertyVisitor visitor) { visitor.Visit(this); }
	}
	public class XlsChartExtPropOverlay : XlsChartExtPropBoolean {
		public XlsChartExtPropOverlay() : base(XlsChartExtPropTypeCode.Overlay) { }
		public override void Visit(IXlsChartExtPropertyVisitor visitor) { visitor.Visit(this); }
	}
	public class XlsChartExtPropPieCombo : XlsChartExtPropBoolean {
		public XlsChartExtPropPieCombo() : base(XlsChartExtPropTypeCode.PieCombo) { }
		public override void Visit(IXlsChartExtPropertyVisitor visitor) { visitor.Visit(this); }
	}
	public class XlsChartExtPropRightAngAxOff : XlsChartExtPropBoolean {
		public XlsChartExtPropRightAngAxOff() : base(XlsChartExtPropTypeCode.RightAngAxOff) { }
		public override void Visit(IXlsChartExtPropertyVisitor visitor) { visitor.Visit(this); }
	}
	public class XlsChartExtPropPerspective : XlsChartExtPropInt {
		public XlsChartExtPropPerspective() : base(XlsChartExtPropTypeCode.Perspective) { }
		public override void Visit(IXlsChartExtPropertyVisitor visitor) { visitor.Visit(this); }
	}
	public class XlsChartExtPropRotationX : XlsChartExtPropInt {
		public XlsChartExtPropRotationX() : base(XlsChartExtPropTypeCode.RotationX) { }
		public override void Visit(IXlsChartExtPropertyVisitor visitor) { visitor.Visit(this); }
	}
	public class XlsChartExtPropRotationY : XlsChartExtPropInt {
		public XlsChartExtPropRotationY() : base(XlsChartExtPropTypeCode.RotationY) { }
		public override void Visit(IXlsChartExtPropertyVisitor visitor) { visitor.Visit(this); }
	}
	public class XlsChartExtPropHeightPercent : XlsChartExtPropDouble {
		public XlsChartExtPropHeightPercent() : base(XlsChartExtPropTypeCode.HeightPercent) { }
		public override void Visit(IXlsChartExtPropertyVisitor visitor) { visitor.Visit(this); }
	}
	public class XlsChartExtPropCultureCode : XlsChartExtPropString {
		public XlsChartExtPropCultureCode() : base(XlsChartExtPropTypeCode.CultureCode) { }
		public override void Visit(IXlsChartExtPropertyVisitor visitor) { visitor.Visit(this); }
	}
	public class XlsChartExtPropSymbol : XlsChartExtPropShort {
		public XlsChartExtPropSymbol() : base(XlsChartExtPropTypeCode.Symbol) { }
		public override void Visit(IXlsChartExtPropertyVisitor visitor) { visitor.Visit(this); }
	}
	public class XlsChartExtPropEof : XlsChartExtPropEmpty {
		public XlsChartExtPropEof() : base(XlsChartExtPropTypeCode.Eof) { }
	}
	#endregion
	#region XlsChartExtProperties
	public class XlsChartExtProperties {
		#region Fields
		readonly List<IXlsChartExtProperty> items = new List<IXlsChartExtProperty>();
		#endregion
		#region Properties
		public XlsChartExtPropParent Parent { get; set; }
		public List<IXlsChartExtProperty> Items { get { return items; } }
		#endregion
		public void Read(BinaryReader reader, XlsContentBuilder contentBuilder) {
			int bytesToRead = reader.ReadInt32();
			int recordVersion = reader.ReadByte();
			if (recordVersion != 0)
				contentBuilder.ThrowInvalidFile("Invalid CrtMlFrt record version!");
			reader.ReadByte(); 
			Parent = (XlsChartExtPropParent)reader.ReadUInt16();
			bytesToRead -= 4;
			while (bytesToRead > 0) {
				IXlsChartExtProperty item = ReadItem(reader);
				if (item != null) {
					if (item is XlsChartExtPropEof)
						return;
					Items.Add(item);
					bytesToRead -= item.GetSize();
				}
			}
			reader.ReadInt32(); 
		}
		public void Write(BinaryWriter writer) {
			int size = GetItemsSize() + 4;
			writer.Write(size);
			writer.Write((ushort)0); 
			writer.Write((ushort)Parent);
			foreach (IXlsChartExtProperty item in items)
				item.Write(writer);
			writer.Write((int)0);
		}
		public void Visit(IXlsChartExtPropertyVisitor visitor) {
			foreach (IXlsChartExtProperty item in items)
				item.Visit(visitor);
		}
		#region Utils
		IXlsChartExtProperty ReadItem(BinaryReader reader) {
			XlsChartExtPropTypeCode typeCode = (XlsChartExtPropTypeCode)(reader.ReadUInt32() & 0xffff00ff);
			IXlsChartExtProperty item = CreateItem(typeCode);
			if (item != null)
				item.Read(reader);
			return item;
		}
		IXlsChartExtProperty CreateItem(XlsChartExtPropTypeCode typeCode) {
			IXlsChartExtProperty result = XlsChartExtPropFactory.CreateProperty(typeCode);
			if (result != null)
				return result;
			switch ((int)typeCode & 0x00ff) {
				case 0x02: return new XlsChartExtPropBoolean(typeCode);
				case 0x03: return new XlsChartExtPropDouble(typeCode);
				case 0x04: return new XlsChartExtPropInt(typeCode);
				case 0x05: return new XlsChartExtPropString(typeCode);
				case 0x06: return new XlsChartExtPropShort(typeCode);
				case 0x07: return new XlsChartExtPropBlob(typeCode);
			}
			return null;
		}
		int GetItemsSize() {
			int result = 0;
			foreach (IXlsChartExtProperty item in items)
				result += item.GetSize();
			return result;
		}
		#endregion
	}
	#endregion
	#region XlsChartExtPropFactory
	public static class XlsChartExtPropFactory {
		#region PropInfo
		internal class PropInfo {
			XlsChartExtPropTypeCode typeCode;
			Type propType;
			public PropInfo(XlsChartExtPropTypeCode typeCode, Type propType) {
				this.typeCode = typeCode;
				this.propType = propType;
			}
			public XlsChartExtPropTypeCode TypeCode { get { return this.typeCode; } }
			public Type PropType { get { return this.propType; } }
		}
		#endregion
		static Dictionary<XlsChartExtPropTypeCode, ConstructorInfo> propTypes;
		static XlsChartExtPropFactory() {
			propTypes = new Dictionary<XlsChartExtPropTypeCode, ConstructorInfo>();
			List<PropInfo> infos = new List<PropInfo>();
			infos.Add(new PropInfo(XlsChartExtPropTypeCode.ScaleMax, typeof(XlsChartExtPropScaleMax)));
			infos.Add(new PropInfo(XlsChartExtPropTypeCode.ScaleMin, typeof(XlsChartExtPropScaleMin)));
			infos.Add(new PropInfo(XlsChartExtPropTypeCode.LogBase, typeof(XlsChartExtPropLogBase)));
			infos.Add(new PropInfo(XlsChartExtPropTypeCode.Style, typeof(XlsChartExtPropStyle)));
			infos.Add(new PropInfo(XlsChartExtPropTypeCode.ThemeOverride, typeof(XlsChartExtPropThemeOverride)));
			infos.Add(new PropInfo(XlsChartExtPropTypeCode.ColorMapOverride, typeof(XlsChartExtPropColorMapOverride)));
			infos.Add(new PropInfo(XlsChartExtPropTypeCode.NoMultiLvlLbl, typeof(XlsChartExtPropNoMultiLvlLbl)));
			infos.Add(new PropInfo(XlsChartExtPropTypeCode.TickLabelSkip, typeof(XlsChartExtPropTickLabelSkip)));
			infos.Add(new PropInfo(XlsChartExtPropTypeCode.TickMarkSkip, typeof(XlsChartExtPropTickMarkSkip)));
			infos.Add(new PropInfo(XlsChartExtPropTypeCode.MajorUnit, typeof(XlsChartExtPropMajorUnit)));
			infos.Add(new PropInfo(XlsChartExtPropTypeCode.MinorUnit, typeof(XlsChartExtPropMinorUnit)));
			infos.Add(new PropInfo(XlsChartExtPropTypeCode.TickLabelPos, typeof(XlsChartExtPropTickLabelPos)));
			infos.Add(new PropInfo(XlsChartExtPropTypeCode.BaseTimeUnit, typeof(XlsChartExtPropBaseTimeUnit)));
			infos.Add(new PropInfo(XlsChartExtPropTypeCode.FormatCode, typeof(XlsChartExtPropFormatCode)));
			infos.Add(new PropInfo(XlsChartExtPropTypeCode.MajorTimeUnit, typeof(XlsChartExtPropMajorTimeUnit)));
			infos.Add(new PropInfo(XlsChartExtPropTypeCode.MinorTimeUnit, typeof(XlsChartExtPropMinorTimeUnit)));
			infos.Add(new PropInfo(XlsChartExtPropTypeCode.ShowDLblsOverMax, typeof(XlsChartExtPropShowDLblsOverMax)));
			infos.Add(new PropInfo(XlsChartExtPropTypeCode.BackWallThickness, typeof(XlsChartExtPropBackWallThickness)));
			infos.Add(new PropInfo(XlsChartExtPropTypeCode.FloorThickness, typeof(XlsChartExtPropFloorThickness)));
			infos.Add(new PropInfo(XlsChartExtPropTypeCode.DispBlankAs, typeof(XlsChartExtPropDispBlankAs)));
			infos.Add(new PropInfo(XlsChartExtPropTypeCode.StartSurface, typeof(XlsChartExtPropStartSurface)));
			infos.Add(new PropInfo(XlsChartExtPropTypeCode.EndSurface, typeof(XlsChartExtPropEndSurface)));
			infos.Add(new PropInfo(XlsChartExtPropTypeCode.ShapeProps, typeof(XlsChartExtPropShapeProps)));
			infos.Add(new PropInfo(XlsChartExtPropTypeCode.TextProps, typeof(XlsChartExtPropTextProps)));
			infos.Add(new PropInfo(XlsChartExtPropTypeCode.Overlay, typeof(XlsChartExtPropOverlay)));
			infos.Add(new PropInfo(XlsChartExtPropTypeCode.PieCombo, typeof(XlsChartExtPropPieCombo)));
			infos.Add(new PropInfo(XlsChartExtPropTypeCode.RightAngAxOff, typeof(XlsChartExtPropRightAngAxOff)));
			infos.Add(new PropInfo(XlsChartExtPropTypeCode.Perspective, typeof(XlsChartExtPropPerspective)));
			infos.Add(new PropInfo(XlsChartExtPropTypeCode.RotationX, typeof(XlsChartExtPropRotationX)));
			infos.Add(new PropInfo(XlsChartExtPropTypeCode.RotationY, typeof(XlsChartExtPropRotationY)));
			infos.Add(new PropInfo(XlsChartExtPropTypeCode.HeightPercent, typeof(XlsChartExtPropHeightPercent)));
			infos.Add(new PropInfo(XlsChartExtPropTypeCode.CultureCode, typeof(XlsChartExtPropCultureCode)));
			infos.Add(new PropInfo(XlsChartExtPropTypeCode.Symbol, typeof(XlsChartExtPropSymbol)));
			infos.Add(new PropInfo(XlsChartExtPropTypeCode.Eof, typeof(XlsChartExtPropEof)));
			for (int i = 0; i < infos.Count; i++) {
				propTypes.Add(infos[i].TypeCode, infos[i].PropType.GetConstructor(new Type[] { }));
			}
		}
		public static IXlsChartExtProperty CreateProperty(XlsChartExtPropTypeCode typeCode) {
			if (!propTypes.ContainsKey(typeCode))
				return null;
			ConstructorInfo propConstructor = propTypes[typeCode];
			return propConstructor.Invoke(new object[] { }) as IXlsChartExtProperty;
		}
	}
	#endregion
	#region XlsChartExtPropertyVisitor
	public class XlsChartExtPropertyVisitor : IXlsChartExtPropertyVisitor {
		#region Fields
		Chart chart;
		#endregion
		public XlsChartExtPropertyVisitor(Chart chart) {
			Guard.ArgumentNotNull(chart, "chart");
			this.chart = chart;
		}
		#region IXlsChartExtPropertyVisitor Members
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropScaleMax item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropScaleMin item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropLogBase item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropStyle item) {
			this.chart.Style = item.Value;
		}
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropThemeOverride item) {
		}
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropColorMapOverride item) {
		}
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropNoMultiLvlLbl item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropTickLabelSkip item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropTickMarkSkip item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropMajorUnit item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropMinorUnit item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropTickLabelPos item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropBaseTimeUnit item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropFormatCode item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropMajorTimeUnit item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropMinorTimeUnit item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropShowDLblsOverMax item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropBackWallThickness item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropFloorThickness item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropDispBlankAs item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropStartSurface item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropEndSurface item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropShapeProps item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropTextProps item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropOverlay item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropPieCombo item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropRightAngAxOff item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropPerspective item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropRotationX item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropRotationY item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropHeightPercent item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropCultureCode item) {
			try {
				this.chart.Culture = new CultureInfo(item.Value);
			}
			catch { }
		}
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropSymbol item) { }
		#endregion
	}
	#endregion
}
