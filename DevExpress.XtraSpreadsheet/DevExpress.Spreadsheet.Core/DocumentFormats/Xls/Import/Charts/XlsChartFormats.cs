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
using DevExpress.Data.Export;
using DevExpress.Export.Xl;
using DevExpress.XtraExport.Xls;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
using DevExpress.Office.Drawing;
using DevExpress.Office.Model;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsChartLineStyle
	public enum XlsChartLineStyle {
		Solid = 0,
		Dash = 1,
		Dot = 2,
		DashDot = 3,
		DashDotDot = 4,
		None = 5,
		DarkGray = 6,
		MediumGray = 7,
		LightGray = 8
	}
	#endregion
	#region XlsChartLineThickness
	public enum XlsChartLineThickness {
		Hairline = 0xffff,
		Narrow = 0x0000,
		Medium = 0x0001,
		Wide = 0x0002
	}
	#endregion
	#region IXlsChartLineFormat
	public interface IXlsChartLineFormat {
		bool Apply { get; set; }
		bool Auto { get; set; }
		bool AxisVisible { get; set; }
		XlsChartLineStyle LineStyle { get; set; }
		XlsChartLineThickness Thickness { get; set; }
		bool AutoColor { get; set; }
		Color LineColor { get; set; }
		int LineColorIndex { get; set; }
	}
	#endregion
	#region XlsChartLineFormat
	public class XlsChartLineFormat : IXlsChartLineFormat {
		public XlsChartLineFormat() {
			Auto = true;
			AxisVisible = true;
			AutoColor = true;
		}
		#region IXlsChartLineFormat Members
		public bool Apply { get; set; }
		public bool Auto { get; set; }
		public bool AxisVisible { get; set; }
		public XlsChartLineStyle LineStyle { get; set; }
		public XlsChartLineThickness Thickness { get; set; }
		public bool AutoColor { get; set; }
		public Color LineColor { get; set; }
		public int LineColorIndex { get; set; }
		#endregion
		public void SetupShapeProperties(ShapeProperties properties) {
			if (Auto)
				return;
			DocumentModel documentModel = properties.DocumentModel;
			Outline outline = properties.Outline;
			switch (LineStyle) {
				case XlsChartLineStyle.None:
					outline.Fill = DrawingFill.None;
					break;
				case XlsChartLineStyle.Solid:
				case XlsChartLineStyle.Dot:
				case XlsChartLineStyle.Dash:
				case XlsChartLineStyle.DashDot:
				case XlsChartLineStyle.DashDotDot:
					DrawingSolidFill solidFill = new DrawingSolidFill(documentModel);
					solidFill.Color.OriginalColor.Rgb = AutoColor ? documentModel.StyleSheet.Palette.DefaultChartForegroundColor : LineColor;
					outline.Fill = solidFill;
					outline.BeginUpdate();
					try {
						outline.Dashing = CalcOutlineDashing();
						outline.Width = CalcOutlineWidth(documentModel);
					}
					finally {
						outline.EndUpdate();
					}
					break;
				case XlsChartLineStyle.LightGray:
				case XlsChartLineStyle.MediumGray:
				case XlsChartLineStyle.DarkGray:
					DrawingPatternFill patternFill = new DrawingPatternFill(documentModel);
					patternFill.PatternType = CalcPatternType();
					patternFill.ForegroundColor.OriginalColor.Rgb = AutoColor ? documentModel.StyleSheet.Palette.DefaultChartForegroundColor : LineColor;
					patternFill.BackgroundColor.OriginalColor.Rgb = AutoColor ? documentModel.StyleSheet.Palette.DefaultChartBackgroundColor : DXColor.White;
					outline.Fill = patternFill;
					outline.BeginUpdate();
					try {
						outline.Dashing = OutlineDashing.Solid;
						outline.Width = CalcOutlineWidth(documentModel);
					}
					finally {
						outline.EndUpdate();
					}
					break;
			}
		}
		#region Utils
		int CalcOutlineWidth(DocumentModel documentModel) {
			switch (Thickness) {
				case XlsChartLineThickness.Narrow: return documentModel.UnitConverter.PointsToModelUnits(1);
				case XlsChartLineThickness.Medium: return documentModel.UnitConverter.PointsToModelUnits(2);
				case XlsChartLineThickness.Wide: return documentModel.UnitConverter.PointsToModelUnits(3);
			}
			return (int)documentModel.UnitConverter.PointsToModelUnitsF(0.25f);
		}
		OutlineDashing CalcOutlineDashing() {
			switch (LineStyle) {
				case XlsChartLineStyle.Dot: return OutlineDashing.Dot;
				case XlsChartLineStyle.Dash: return OutlineDashing.Dash;
				case XlsChartLineStyle.DashDot: return OutlineDashing.LongDashDot;
				case XlsChartLineStyle.DashDotDot: return OutlineDashing.LongDashDotDot;
			}
			return OutlineDashing.Solid;
		}
		DrawingPatternType CalcPatternType() {
			switch (LineStyle) {
				case XlsChartLineStyle.LightGray: return DrawingPatternType.Percent25;
				case XlsChartLineStyle.DarkGray: return DrawingPatternType.Percent75;
			}
			return DrawingPatternType.Percent50;
		}
		#endregion
		#region ShapePropsCheckSum
		public void CalcSpCheckSum(MsoCrc32Compute crc32) {
			if (!Apply)
				return;
			crc32.Add((byte)LineColorIndex);
			if (LineStyle == XlsChartLineStyle.None)
				crc32.Add((byte)0xff); 
			else
				crc32.Add((byte)LineStyle);
			crc32.Add((byte)((short)Thickness + 1));
			crc32.Add((byte)(Auto ? 1 : 0));
			crc32.Add(LineColor.R);
			crc32.Add(LineColor.G);
			crc32.Add(LineColor.B);
			crc32.Add((byte)0);
		}
		#endregion
	}
	#endregion
	#region XlsChartLineFormatContainer
	public abstract class XlsChartLineFormatContainer : IXlsChartLineFormat, IXlsChartShapeFormatContainer {
		#region Fields
		const int formatsCount = 4;
		XlsChartLineFormat[] lineFormats;
		int lineFormatIndex;
		XlsChartShapeFormatCollection shapeFormats = new XlsChartShapeFormatCollection();
		#endregion
		protected XlsChartLineFormatContainer() {
			lineFormats = new XlsChartLineFormat[formatsCount];
			for (int i = 0; i < formatsCount; i++)
				lineFormats[i] = new XlsChartLineFormat();
			lineFormatIndex = 0;
		}
		#region Properties
		protected internal XlsChartLineFormat[] LineFormats { get { return lineFormats; } }
		protected internal int LineFormatIndex {
			get { return lineFormatIndex; }
			set {
				ValueChecker.CheckValue(value, 0, formatsCount - 1, "LineFormatIndex");
				lineFormatIndex = value;
			}
		}
		protected internal XlsChartShapeFormatCollection ShapeFormats { get { return shapeFormats; } }
		#endregion
		#region IXlsChartLineFormat Members
		bool IXlsChartLineFormat.Apply {
			get { return lineFormats[lineFormatIndex].Apply; }
			set { lineFormats[lineFormatIndex].Apply = value; }
		}
		bool IXlsChartLineFormat.Auto {
			get { return lineFormats[lineFormatIndex].Auto; }
			set { lineFormats[lineFormatIndex].Auto = value; }
		}
		bool IXlsChartLineFormat.AxisVisible {
			get { return lineFormats[lineFormatIndex].AxisVisible; }
			set { lineFormats[lineFormatIndex].AxisVisible = value; }
		}
		XlsChartLineStyle IXlsChartLineFormat.LineStyle {
			get { return lineFormats[lineFormatIndex].LineStyle; }
			set { lineFormats[lineFormatIndex].LineStyle = value; }
		}
		XlsChartLineThickness IXlsChartLineFormat.Thickness {
			get { return lineFormats[lineFormatIndex].Thickness; }
			set { lineFormats[lineFormatIndex].Thickness = value; }
		}
		bool IXlsChartLineFormat.AutoColor {
			get { return lineFormats[lineFormatIndex].AutoColor; }
			set { lineFormats[lineFormatIndex].AutoColor = value; }
		}
		Color IXlsChartLineFormat.LineColor {
			get { return lineFormats[lineFormatIndex].LineColor; }
			set { lineFormats[lineFormatIndex].LineColor = value; }
		}
		int IXlsChartLineFormat.LineColorIndex {
			get { return lineFormats[lineFormatIndex].LineColorIndex; }
			set { lineFormats[lineFormatIndex].LineColorIndex = value; }
		}
		#endregion
		#region IXlsChartShapePropertiesContainer Members
		void IXlsChartShapeFormatContainer.Add(XlsChartShapeFormat properties) {
			int index = GetIndexByObjContext(properties.ObjContext);
			if(IsValidSpCheckSum(index, properties.CheckSum))
				this.shapeFormats.Add(properties);
		}
		#endregion
		protected abstract int GetIndexByObjContext(int objContext);
		protected virtual bool IsValidSpCheckSum(int index, int checkSum) {
			return CalcSpCheckSum(index) == checkSum;
		}
		protected virtual int CalcSpCheckSum(int index) {
			MsoCrc32Compute crc32 = new MsoCrc32Compute();
			LineFormats[index].CalcSpCheckSum(crc32);
			return crc32.CrcValue;
		}
	}
	#endregion
	#region XlsChartFillType
	public enum XlsChartFillType {
		None = 0x0000,
		Solid = 0x0001,
		MediumGray = 0x0002,
		DarkGray = 0x0003,
		LightGray = 0x0004,
		Horizontal = 0x0005,
		Vertical = 0x0006,
		DownwardDiagonal = 0x0007,
		UpwardDiagonal = 0x0008,
		Grid = 0x0009,
		Trellis = 0x000a,
		LightHorizontal = 0x000b,
		LightVertical = 0x000c,
		LightDown = 0x000d,
		LightUp = 0x000e,
		LightGrid = 0x000f,
		LightTrellis = 0x0010,
		Gray125 = 0x0011,
		Gray0625 = 0x0012
	}
	#endregion
	#region IXlsChartAreaFormat
	public interface IXlsChartAreaFormat {
		bool Apply { get; set; }
		Color ForegroundColor { get; set; }
		Color BackgroundColor { get; set; }
		XlsChartFillType FillType { get; set; }
		bool AutoColor { get; set; }
		bool InvertIfNegative { get; set; }
	}
	#endregion
	#region XlsChartAreaFormat
	public class XlsChartAreaFormat : IXlsChartAreaFormat, ISupportsCopyFrom<XlsChartAreaFormat>, ICloneable<XlsChartAreaFormat> {
		public XlsChartAreaFormat() {
			AutoColor = true;
			FillType = XlsChartFillType.Solid;
		}
		#region IXlsChartAreaFormat Members
		public bool Apply { get; set; }
		public Color ForegroundColor { get; set; }
		public Color BackgroundColor { get; set; }
		public XlsChartFillType FillType { get; set; }
		public bool AutoColor { get; set; }
		public bool InvertIfNegative { get; set; }
		#endregion
		public void SetupShapeProperties(ShapeProperties properties) {
			if (AutoColor && FillType == XlsChartFillType.Solid)
				return;
			DocumentModel documentModel = properties.DocumentModel;
			if (FillType == XlsChartFillType.None)
				properties.Fill = DrawingFill.None;
			else {
				DrawingSolidFill solidFill = new DrawingSolidFill(documentModel);
				solidFill.Color.OriginalColor.Rgb = ForegroundColor;
				properties.Fill = solidFill;
			}
		}
		public void CalcSpCheckSum(MsoCrc32Compute crc32) {
			if (!Apply || AutoColor)
				return;
			crc32.Add(ForegroundColor.R);
			crc32.Add(ForegroundColor.G);
			crc32.Add(ForegroundColor.B);
			crc32.Add((byte)0);
			crc32.Add(BackgroundColor.R);
			crc32.Add(BackgroundColor.G);
			crc32.Add(BackgroundColor.B);
			crc32.Add((byte)0);
			crc32.Add((byte)FillType);
		}
		#region ISupportsCopyFrom<XlsChartAreaFormat> Members
		public void CopyFrom(XlsChartAreaFormat value) {
			Guard.ArgumentNotNull(value, "value");
			this.Apply = value.Apply;
			this.ForegroundColor = value.ForegroundColor;
			this.BackgroundColor = value.BackgroundColor;
			this.FillType = value.FillType;
			this.AutoColor = value.AutoColor;
			this.InvertIfNegative = value.InvertIfNegative;
		}
		#endregion
		#region ICloneable<XlsChartAreaFormat> Members
		public XlsChartAreaFormat Clone() {
			XlsChartAreaFormat result = new XlsChartAreaFormat();
			result.CopyFrom(this);
			return result;
		}
		#endregion
	}
	#endregion
	#region IXlsChartGraphicFormat
	public interface IXlsChartGraphicFormat {
		bool Apply { get; set; }
		OfficeArtProperties ArtProperties { get; set; }
		OfficeArtTertiaryProperties ArtTertiaryProperties { get; set; }
	}
	#endregion
	#region XlsChartGraphicFormat
	public class XlsChartGraphicFormat : IXlsChartGraphicFormat {
		#region PatternTable (static)
		class PatternTypeMD4 {
			byte[] md4;
			public PatternTypeMD4(DrawingPatternType patternType, byte[] md4) {
				PatternType = patternType;
				this.md4 = md4;
			}
			public DrawingPatternType PatternType { get; private set; }
			public bool IsEqualMD4(byte[] data, int start) {
				for (int i = 0; i < md4.Length; i++)
					if (md4[i] != data[i + start])
						return false;
				return true;
			}
		}
		static List<PatternTypeMD4> patternTable = CreatePatternTable();
		static List<PatternTypeMD4> CreatePatternTable() {
			List<PatternTypeMD4> result = new List<PatternTypeMD4>();
			result.Add(new PatternTypeMD4(DrawingPatternType.Percent5, new byte[] { 0xE4, 0x42, 0xEF, 0x5D, 0x22, 0xB2, 0xE3, 0x82, 0x49, 0x3C, 0x39, 0x6B, 0x02, 0xBB, 0xDF, 0x5F }));
			result.Add(new PatternTypeMD4(DrawingPatternType.Percent10, new byte[] { 0x99, 0x33, 0x78, 0xFC, 0x21, 0x74, 0x56, 0xC3, 0x24, 0xCE, 0x01, 0xA0, 0x8E, 0x64, 0xEC, 0xBB }));
			result.Add(new PatternTypeMD4(DrawingPatternType.Percent20, new byte[] { 0x04, 0x66, 0x07, 0x15, 0x4C, 0x34, 0x12, 0xC2, 0xB3, 0x40, 0x31, 0xD1, 0xF2, 0xE4, 0x81, 0x00 }));
			result.Add(new PatternTypeMD4(DrawingPatternType.Percent25, new byte[] { 0x91, 0xB4, 0xE6, 0x77, 0x12, 0x86, 0x17, 0xBD, 0x71, 0xD6, 0x31, 0x0E, 0x17, 0x76, 0xD0, 0x26 }));
			result.Add(new PatternTypeMD4(DrawingPatternType.Percent30, new byte[] { 0xDD, 0x0E, 0xE2, 0xDD, 0xDF, 0xBF, 0xDF, 0x9D, 0x58, 0x6F, 0xF1, 0xE7, 0xF2, 0x0E, 0xB9, 0x98 }));
			result.Add(new PatternTypeMD4(DrawingPatternType.Percent40, new byte[] { 0xFF, 0xB5, 0x90, 0x12, 0x31, 0xA1, 0x12, 0x95, 0x1B, 0xEB, 0xDA, 0x01, 0xE1, 0xA4, 0x75, 0x26 }));
			result.Add(new PatternTypeMD4(DrawingPatternType.Percent50, new byte[] { 0x72, 0xE8, 0x25, 0xA2, 0x4B, 0xE5, 0x50, 0x1D, 0x29, 0x73, 0x70, 0x3A, 0x22, 0x1C, 0x4D, 0xC3 }));
			result.Add(new PatternTypeMD4(DrawingPatternType.Percent60, new byte[] { 0xA7, 0xCE, 0x67, 0x5C, 0xD3, 0x4F, 0xF0, 0x79, 0x0A, 0x9D, 0xB4, 0xAD, 0xF7, 0x45, 0x6B, 0x91 }));
			result.Add(new PatternTypeMD4(DrawingPatternType.Percent70, new byte[] { 0xEB, 0xB6, 0x02, 0xB2, 0x2F, 0xEA, 0x69, 0x67, 0x8A, 0x7E, 0xAA, 0x68, 0x51, 0x98, 0xB7, 0x59 }));
			result.Add(new PatternTypeMD4(DrawingPatternType.Percent75, new byte[] { 0x78, 0x51, 0x80, 0x46, 0xFE, 0x6F, 0xE2, 0xE9, 0xD7, 0xE2, 0xF5, 0x4A, 0x6C, 0x2F, 0xEE, 0x49 }));
			result.Add(new PatternTypeMD4(DrawingPatternType.Percent80, new byte[] { 0x37, 0xCE, 0x2E, 0x0C, 0x69, 0x8F, 0xD2, 0xBE, 0xC5, 0xDF, 0x3F, 0xFB, 0x4F, 0x6B, 0xCD, 0xB9 }));
			result.Add(new PatternTypeMD4(DrawingPatternType.Percent90, new byte[] { 0xC7, 0xF1, 0xB3, 0xA9, 0x1E, 0xE8, 0x11, 0xA2, 0xBA, 0x91, 0x2B, 0xDD, 0x07, 0xB8, 0x2E, 0xAA }));
			result.Add(new PatternTypeMD4(DrawingPatternType.DarkDownwardDiagonal, new byte[] { 0x2E, 0x48, 0x79, 0x5D, 0xBE, 0x1F, 0x5F, 0x95, 0x7B, 0x60, 0x8E, 0x40, 0xA4, 0xED, 0xD6, 0x55 }));
			result.Add(new PatternTypeMD4(DrawingPatternType.DarkHorizontal, new byte[] { 0x4D, 0x2D, 0xC3, 0x2A, 0xF4, 0x1B, 0xBC, 0x63, 0x9B, 0x68, 0xE7, 0x48, 0xC3, 0x48, 0x9F, 0xE2 }));
			result.Add(new PatternTypeMD4(DrawingPatternType.DarkUpwardDiagonal, new byte[] { 0x35, 0xAA, 0x4F, 0x63, 0xEF, 0x7D, 0xAE, 0xB5, 0xD1, 0x83, 0xC7, 0x25, 0x90, 0x34, 0xE5, 0x01 }));
			result.Add(new PatternTypeMD4(DrawingPatternType.DarkVertical, new byte[] { 0x8A, 0x36, 0xDD, 0xA2, 0x04, 0xA0, 0x01, 0xD5, 0xF0, 0x48, 0x73, 0xD4, 0xA0, 0xD0, 0x17, 0xA3 }));
			result.Add(new PatternTypeMD4(DrawingPatternType.DashedDownwardDiagonal, new byte[] { 0x24, 0x63, 0x4F, 0xFD, 0x51, 0x4D, 0x7A, 0x3E, 0x1F, 0x92, 0x0E, 0xF0, 0x3A, 0xAD, 0xCF, 0x8C }));
			result.Add(new PatternTypeMD4(DrawingPatternType.DashedHorizontal, new byte[] { 0x63, 0x8E, 0xC1, 0x65, 0x59, 0x66, 0x28, 0x48, 0x49, 0x21, 0x1B, 0x3E, 0xAD, 0xA1, 0x65, 0x40 }));
			result.Add(new PatternTypeMD4(DrawingPatternType.DashedUpwardDiagonal, new byte[] { 0xF8, 0x53, 0x9A, 0xED, 0xF0, 0xF8, 0xAC, 0x75, 0x89, 0x1E, 0xED, 0x9F, 0xA3, 0x4E, 0x5C, 0xD6 }));
			result.Add(new PatternTypeMD4(DrawingPatternType.DashedVertical, new byte[] { 0x19, 0xCD, 0xA0, 0x12, 0xDE, 0xEF, 0xDC, 0x13, 0x81, 0x0E, 0x4E, 0xF0, 0x0C, 0x1A, 0x18, 0xB7 }));
			result.Add(new PatternTypeMD4(DrawingPatternType.LightDownwardDiagonal, new byte[] { 0xDC, 0x25, 0xAD, 0x6F, 0xA8, 0x89, 0x4F, 0xFB, 0xC1, 0x79, 0xAD, 0xFD, 0x79, 0x9E, 0x40, 0xB4 }));
			result.Add(new PatternTypeMD4(DrawingPatternType.LightHorizontal, new byte[] { 0x96, 0x03, 0x6B, 0x0E, 0x91, 0xD6, 0x57, 0x37, 0x84, 0x7B, 0xD4, 0x79, 0x33, 0xE5, 0x54, 0x7F }));
			result.Add(new PatternTypeMD4(DrawingPatternType.LightUpwardDiagonal, new byte[] { 0xA6, 0x39, 0x43, 0x5F, 0x69, 0x79, 0x02, 0x9D, 0x75, 0xEB, 0x13, 0xD1, 0x15, 0x55, 0xFF, 0xEE }));
			result.Add(new PatternTypeMD4(DrawingPatternType.LightVertical, new byte[] { 0xA4, 0xF0, 0x0B, 0x9E, 0xC9, 0x31, 0x86, 0xAA, 0x3D, 0x3C, 0x2A, 0xB6, 0x49, 0xD8, 0x9C, 0x55 }));
			result.Add(new PatternTypeMD4(DrawingPatternType.WideDownwardDiagonal, new byte[] { 0x7C, 0x6B, 0xCD, 0x61, 0x40, 0xE4, 0x4F, 0xF6, 0x2B, 0x06, 0xEF, 0x38, 0xF8, 0xC5, 0x6B, 0x94 }));
			result.Add(new PatternTypeMD4(DrawingPatternType.WideUpwardDiagonal, new byte[] { 0x24, 0xC4, 0x45, 0x32, 0x04, 0xF8, 0xB3, 0x66, 0x59, 0x5F, 0x2C, 0xE4, 0x49, 0x96, 0x21, 0x29 }));
			result.Add(new PatternTypeMD4(DrawingPatternType.LargeCheckerBoard, new byte[] { 0xE4, 0x4A, 0x8B, 0x4A, 0x44, 0x54, 0xC1, 0x26, 0x2B, 0x3E, 0xF1, 0xA0, 0x4C, 0x80, 0x42, 0xEB }));
			result.Add(new PatternTypeMD4(DrawingPatternType.LargeConfetti, new byte[] { 0xC9, 0x79, 0x75, 0xBD, 0x80, 0xFF, 0x1B, 0x49, 0x98, 0xD3, 0x98, 0x71, 0x94, 0x10, 0xCC, 0x01 }));
			result.Add(new PatternTypeMD4(DrawingPatternType.LargeGrid, new byte[] { 0xC1, 0x72, 0x73, 0xE0, 0xE5, 0x76, 0xC2, 0xD1, 0xFC, 0x97, 0x2A, 0x96, 0x09, 0x22, 0xDB, 0x9A }));
			result.Add(new PatternTypeMD4(DrawingPatternType.SmallCheckerBoard, new byte[] { 0xDF, 0x80, 0x4D, 0xEE, 0x27, 0x24, 0xF9, 0x2C, 0x7F, 0x2C, 0xF1, 0xD2, 0xCC, 0x47, 0x8C, 0x8C }));
			result.Add(new PatternTypeMD4(DrawingPatternType.SmallConfetti, new byte[] { 0xC7, 0xCF, 0xE6, 0xFF, 0x48, 0x5F, 0xF6, 0xA4, 0x13, 0x92, 0xC5, 0xFD, 0x08, 0x2D, 0xEE, 0x6E }));
			result.Add(new PatternTypeMD4(DrawingPatternType.SmallGrid, new byte[] { 0x7C, 0x98, 0x54, 0xBD, 0x17, 0x3E, 0xA0, 0xE8, 0xF3, 0xF8, 0xFD, 0xCC, 0x7F, 0x8C, 0xBA, 0xE6 }));
			result.Add(new PatternTypeMD4(DrawingPatternType.NarrowHorizontal, new byte[] { 0xEF, 0x8D, 0xC0, 0x6D, 0x3E, 0x80, 0x5F, 0xEF, 0x9B, 0xEC, 0xE0, 0x7A, 0x36, 0x3B, 0x2B, 0xDC }));
			result.Add(new PatternTypeMD4(DrawingPatternType.NarrowVertical, new byte[] { 0x0C, 0xAE, 0x68, 0xE8, 0x1F, 0xCB, 0x3E, 0xF9, 0xC8, 0xC1, 0x22, 0xBA, 0x66, 0x5B, 0x67, 0xD2 }));
			result.Add(new PatternTypeMD4(DrawingPatternType.DottedDiamond, new byte[] { 0x42, 0xA6, 0x8C, 0x09, 0x80, 0xE7, 0x8C, 0xA0, 0xB8, 0x9D, 0x97, 0x4B, 0xE8, 0x5E, 0x5C, 0xDD }));
			result.Add(new PatternTypeMD4(DrawingPatternType.DottedGrid, new byte[] { 0xA9, 0xF1, 0xB4, 0x0C, 0xC0, 0xE7, 0x7E, 0x1B, 0x31, 0x3E, 0x75, 0x50, 0xD4, 0xC1, 0xBD, 0x2C }));
			result.Add(new PatternTypeMD4(DrawingPatternType.HorizontalBrick, new byte[] { 0x25, 0x33, 0xC3, 0xA0, 0xC6, 0x52, 0xFB, 0x96, 0x48, 0x4D, 0x92, 0x3B, 0xE8, 0xE3, 0x22, 0x86 }));
			result.Add(new PatternTypeMD4(DrawingPatternType.DiagonalBrick, new byte[] { 0x8B, 0x22, 0x81, 0x2C, 0x1A, 0x5A, 0x5B, 0x9E, 0xF9, 0x9F, 0x03, 0xC8, 0x44, 0x71, 0x30, 0x8B }));
			result.Add(new PatternTypeMD4(DrawingPatternType.Divot, new byte[] { 0xE9, 0xD5, 0x31, 0xBF, 0xEA, 0x0F, 0xA0, 0x26, 0x53, 0x9D, 0x9F, 0xCF, 0xF0, 0xC9, 0x15, 0xDB }));
			result.Add(new PatternTypeMD4(DrawingPatternType.Plaid, new byte[] { 0x74, 0x17, 0xD8, 0x91, 0x36, 0x06, 0xC4, 0xDB, 0x4A, 0xC6, 0x22, 0x2B, 0x7A, 0x63, 0x61, 0xBE }));
			result.Add(new PatternTypeMD4(DrawingPatternType.Shingle, new byte[] { 0xDB, 0x50, 0xDD, 0x30, 0x25, 0xC5, 0xB8, 0x30, 0xEA, 0xB7, 0xA2, 0xC3, 0x6F, 0x95, 0x27, 0x25 }));
			result.Add(new PatternTypeMD4(DrawingPatternType.OpenDiamond, new byte[] { 0x4C, 0xD2, 0x08, 0xB5, 0xB4, 0x61, 0xBC, 0x5D, 0x30, 0x87, 0x97, 0x7C, 0xA5, 0xD1, 0xE7, 0x72 }));
			result.Add(new PatternTypeMD4(DrawingPatternType.SolidDiamond, new byte[] { 0x3D, 0x6D, 0xBD, 0x0F, 0x52, 0x67, 0xDF, 0xDC, 0x39, 0x05, 0x92, 0xFD, 0x77, 0x4A, 0x75, 0xB3 }));
			result.Add(new PatternTypeMD4(DrawingPatternType.Sphere, new byte[] { 0x32, 0x66, 0xB8, 0x58, 0x47, 0x32, 0xF9, 0x11, 0x2E, 0x4E, 0xE5, 0xE6, 0xB0, 0xBF, 0xA9, 0xFF }));
			result.Add(new PatternTypeMD4(DrawingPatternType.Trellis, new byte[] { 0xD4, 0xED, 0xE5, 0x0F, 0xEE, 0xF4, 0x2E, 0x5B, 0x5D, 0x2D, 0x20, 0x3D, 0xAA, 0x27, 0xA5, 0x3E }));
			result.Add(new PatternTypeMD4(DrawingPatternType.Wave, new byte[] { 0xC5, 0x53, 0xA1, 0x13, 0x4C, 0xBB, 0x0D, 0x01, 0xB1, 0xB1, 0xA2, 0xEE, 0x19, 0x45, 0xC1, 0xB5 }));
			result.Add(new PatternTypeMD4(DrawingPatternType.Weave, new byte[] { 0xBD, 0x05, 0xF4, 0xD4, 0x91, 0xAB, 0xF6, 0x5D, 0x06, 0x3E, 0xC2, 0xD4, 0xD1, 0x5D, 0x46, 0x01 }));
			result.Add(new PatternTypeMD4(DrawingPatternType.ZigZag, new byte[] { 0x60, 0xA2, 0xE3, 0x6E, 0xFD, 0x96, 0x78, 0xCC, 0x18, 0xD6, 0xBC, 0x66, 0x08, 0x97, 0x61, 0xEB }));
			return result;
		}
		#endregion
		#region Fields
		OfficeArtProperties artProperties;
		OfficeArtTertiaryProperties artTertiaryProperties;
		#endregion
		#region IXlsChartGraphicFormat Members
		public bool Apply { get; set; }
		public OfficeArtProperties ArtProperties {
			get {
				if (artProperties == null)
					artProperties = new OfficeArtProperties();
				return artProperties;
			}
			set { artProperties = value; }
		}
		public OfficeArtTertiaryProperties ArtTertiaryProperties {
			get {
				if (artTertiaryProperties == null)
					artTertiaryProperties = new OfficeArtTertiaryProperties();
				return artTertiaryProperties;
			}
			set { artTertiaryProperties = value; }
		}
		#endregion
		#region SetupShapeProperties
		public void SetupShapeProperties(ShapeProperties properties) {
			OfficeFillType fillType = OfficeFillType.Solid;
			OfficeDrawingFillType fillTypeProp = ArtProperties.GetPropertyByType(typeof(OfficeDrawingFillType)) as OfficeDrawingFillType;
			if(fillTypeProp != null)
				fillType = fillTypeProp.FillType;
			switch(fillType) {
				case OfficeFillType.Solid:
					SetupShapeSolidFill(properties);
					break;
				case OfficeFillType.Pattern:
					SetupShapePatternFill(properties);
					break;
				case OfficeFillType.Texture:
				case OfficeFillType.Picture:
					SetupShapePictureFill(properties, fillType);
					break;
				case OfficeFillType.Shade:
				case OfficeFillType.ShadeCenter:
				case OfficeFillType.ShadeScale:
				case OfficeFillType.ShadeShape:
				case OfficeFillType.ShadeTile:
					SetupShapeGradientFill(properties, fillType);
					break;
				case OfficeFillType.Background:
					properties.Fill = DrawingFill.Group;
					break;
			}
		}
		void SetupShapeSolidFill(ShapeProperties properties) {
			DocumentModel documentModel = properties.DocumentModel;
			DrawingSolidFill fill = new DrawingSolidFill(documentModel);
			SetupFillColor(fill.Color, documentModel);
			double opacity = GetFillOpacity(typeof(DrawingFillOpacity));
			SetupFillOpacity(fill.Color, opacity);
			properties.Fill = fill;
		}
		void SetupShapePatternFill(ShapeProperties properties) {
			DocumentModel documentModel = properties.DocumentModel;
			DrawingPatternFill fill = new DrawingPatternFill(documentModel);
			SetupColor(fill.ForegroundColor, GetFillColor(typeof(DrawingFillColor)), documentModel);
			SetupColor(fill.BackgroundColor, GetFillColor(typeof(DrawingFillBackColor)), documentModel);
			SetupFillOpacity(fill.ForegroundColor, GetFillOpacity(typeof(DrawingFillOpacity)));
			SetupFillOpacity(fill.BackgroundColor, GetFillOpacity(typeof(DrawingFillBackOpacity)));
			fill.PatternType = GetDrawingPatternType();
			properties.Fill = fill;
		}
		void SetupShapePictureFill(ShapeProperties properties, OfficeFillType fillType) {
		}
		void SetupShapeGradientFill(ShapeProperties properties, OfficeFillType fillType) {
			DocumentModel documentModel = properties.DocumentModel;
			DrawingGradientFill fill = new DrawingGradientFill(documentModel);
			switch (fillType) {
				case OfficeFillType.ShadeShape:
					fill.GradientType = GradientType.Shape;
					fill.FillRect = new RectangleOffset(50000, 50000, 50000, 50000);
					break;
				case OfficeFillType.ShadeCenter:
					fill.GradientType = GradientType.Rectangle;
					SetupFillRect(fill);
					break;
				default:
					fill.GradientType = GradientType.Linear;
					break;
			}
			SetupGradientStops(fill, documentModel);
			SetupGradientAngle(fill);
			properties.Fill = fill;
		}
		#region Color
		void SetupColor(IDrawingColor drawingColor, OfficeColorRecord officeColor, DocumentModel documentModel) {
			if (officeColor == null || officeColor.IsDefault)
				drawingColor.Rgb = DXColor.FromArgb(0xff, 0xff, 0xff);
			else if (officeColor.SystemColorUsed)
				SetupSystemColor(drawingColor, officeColor, documentModel);
			else if (officeColor.ColorSchemeUsed)
				drawingColor.Rgb = ColorModelInfo.Create(officeColor.ColorSchemeIndex).ToRgb(documentModel.StyleSheet.Palette, documentModel.OfficeTheme.Colors);
			else
				drawingColor.Rgb = officeColor.Color;
		}
		void SetupSystemColor(IDrawingColor drawingColor, OfficeColorRecord officeColor, DocumentModel documentModel) {
			if (officeColor.ColorUse == OfficeColorUse.None) {
				SystemColorValues systemColor = SystemColorValues.Empty;
				if (Enum.IsDefined(typeof(SystemColorValues), officeColor.SystemColorIndex))
					systemColor = (SystemColorValues)officeColor.SystemColorIndex;
				drawingColor.System = systemColor;
			}
			else
				SetupSpecialColor(drawingColor, officeColor, documentModel);
		}
		void SetupSpecialColor(IDrawingColor drawingColor, OfficeColorRecord officeColor, DocumentModel documentModel) {
			if (officeColor.ColorUse == OfficeColorUse.UseFillColor)
				SetupRgbColor(drawingColor, GetFillColor(typeof(DrawingFillColor)), documentModel);
			else if (officeColor.ColorUse == OfficeColorUse.UseFillBackColor)
				SetupRgbColor(drawingColor, GetFillColor(typeof(DrawingFillBackColor)), documentModel);
			else if (officeColor.ColorUse == OfficeColorUse.UseFillOrLineColor) {
				OfficeColorRecord colorRecord = GetFillColor(typeof(DrawingFillColor));
				if (colorRecord != null)
					SetupRgbColor(drawingColor, colorRecord, documentModel);
				else
					SetupRgbColor(drawingColor, GetFillColor(typeof(DrawingLineColor)), documentModel);
			}
			else if (officeColor.ColorUse == OfficeColorUse.UseLineOrFillColor) {
				OfficeColorRecord colorRecord = GetFillColor(typeof(DrawingLineColor));
				if (colorRecord != null)
					SetupRgbColor(drawingColor, colorRecord, documentModel);
				else
					SetupRgbColor(drawingColor, GetFillColor(typeof(DrawingFillColor)), documentModel);
			}
			else if (officeColor.ColorUse == OfficeColorUse.UseLineColor)
				SetupRgbColor(drawingColor, GetFillColor(typeof(DrawingLineColor)), documentModel);
			else
				SetupRgbColor(drawingColor, null, documentModel);
			if (officeColor.Transform == OfficeColorTransform.Darken && officeColor.TransformValue < 0xff) {
				drawingColor.Transforms.Add(new GammaColorTransform());
				drawingColor.Transforms.Add(new ShadeColorTransform(officeColor.TransformValue * DrawingValueConstants.ThousandthOfPercentage / 0xff));
				drawingColor.Transforms.Add(new InverseGammaColorTransform());
			}
			if (officeColor.Transform == OfficeColorTransform.Lighten && officeColor.TransformValue < 0xff) {
				drawingColor.Transforms.Add(new GammaColorTransform());
				drawingColor.Transforms.Add(new TintColorTransform(officeColor.TransformValue * DrawingValueConstants.ThousandthOfPercentage / 0xff));
				drawingColor.Transforms.Add(new InverseGammaColorTransform());
			}
		}
		void SetupRgbColor(IDrawingColor drawingColor, OfficeColorRecord officeColor, DocumentModel documentModel) {
			if (officeColor == null || officeColor.IsDefault || officeColor.SystemColorUsed)
				drawingColor.Rgb = DXColor.FromArgb(0xff, 0xff, 0xff);
			else if (officeColor.ColorSchemeUsed)
				drawingColor.Rgb = ColorModelInfo.Create(officeColor.ColorSchemeIndex).ToRgb(documentModel.StyleSheet.Palette, documentModel.OfficeTheme.Colors);
			else
				drawingColor.Rgb = officeColor.Color;
		}
		void SetupFillColor(IDrawingColor drawingColor, DocumentModel documentModel) {
			DrawingFillColor propColor = ArtProperties.GetPropertyByType(typeof(DrawingFillColor)) as DrawingFillColor;
			SetupColor(drawingColor, propColor == null ? null : propColor.ColorRecord, documentModel);
		}
		OfficeColorRecord GetFillColor(Type propType) {
			OfficeDrawingColorPropertyBase propColor = ArtProperties.GetPropertyByType(propType) as OfficeDrawingColorPropertyBase;
			if (propColor != null)
				return propColor.ColorRecord;
			return null;
		}
		#endregion
		#region Opacity
		void SetupFillOpacity(IDrawingColor drawingColor, double opacity) {
			if (opacity < 1.0) {
				if (opacity < 0.0)
					opacity = 0.0;
				drawingColor.Transforms.Add(new AlphaColorTransform((int)(opacity * DrawingValueConstants.ThousandthOfPercentage)));
			}
		}
		double GetFillOpacity(Type propType) {
			OfficeDrawingFixedPointPropertyBase propOpacity = ArtProperties.GetPropertyByType(propType) as OfficeDrawingFixedPointPropertyBase;
			if (propOpacity != null)
				return propOpacity.Value;
			return 1.0;
		}
		#endregion
		#region Pattern
		DrawingPatternType GetDrawingPatternType() {
			DrawingFillBlip prop = ArtProperties.GetPropertyByType(typeof(DrawingFillBlip)) as DrawingFillBlip;
			if (prop != null && prop.ComplexData.Length > 24)
				return GetDrawingPatternType(prop.ComplexData);
			return DrawingPatternType.Percent50;
		}
		DrawingPatternType GetDrawingPatternType(byte[] data) {
			foreach (PatternTypeMD4 item in patternTable) {
				if (item.IsEqualMD4(data, 8))
					return item.PatternType;
			}
			return DrawingPatternType.Percent50;
		}
		#endregion
		#region Gradient
		void SetupGradientStops(DrawingGradientFill gradientFill, DocumentModel documentModel) {
			DrawingFillShadeColors prop = ArtProperties.GetPropertyByType(typeof(DrawingFillShadeColors)) as DrawingFillShadeColors;
			List<OfficeShadeColor> shadeColors = new List<OfficeShadeColor>();
			List<double> shadeOpacities = new List<double>();
			if (prop != null && prop.ComplexData.Length > 0)
				GetShadeColors(shadeColors, shadeOpacities, prop.ComplexData);
			else
				GetShadeColors(shadeColors, shadeOpacities);
			for (int i = 0; i < shadeColors.Count; i++) {
				DrawingGradientStop gradientStop = new DrawingGradientStop(documentModel);
				SetupColor(gradientStop.Color, shadeColors[i].ColorRecord, documentModel);
				gradientStop.Position = (int)(shadeColors[i].Position * DrawingValueConstants.ThousandthOfPercentage);
				SetupFillOpacity(gradientStop.Color, shadeOpacities[i]);
				gradientFill.GradientStops.Add(gradientStop);
			}
		}
		void GetShadeColors(List<OfficeShadeColor> shadeColors, List<double> shadeOpacities, byte[] data) {
			int count;
			using (MemoryStream ms = new MemoryStream(data, false)) {
				using (BinaryReader reader = new BinaryReader(ms)) {
					count = reader.ReadUInt16();
					reader.ReadUInt16(); 
					reader.ReadUInt16(); 
					for (int i = 0; i < count; i++)
						shadeColors.Add(OfficeShadeColor.FromStream(reader));
				}
			}
			double firstOpacity = GetFillOpacity(typeof(DrawingFillOpacity));
			double lastOpacity = GetFillOpacity(typeof(DrawingFillBackOpacity));
			double firstPosition = shadeColors[0].Position;
			double lastPosition = shadeColors[count - 1].Position;
			double k = (lastPosition == firstPosition) ? 0.0 : ((lastOpacity - firstOpacity) / (lastPosition - firstPosition));
			for (int i = 0; i < count; i++) {
				double opacity = firstOpacity + k * (shadeColors[i].Position - firstPosition);
				shadeOpacities.Add(opacity);
			}
		}
		void GetShadeColors(List<OfficeShadeColor> shadeColors, List<double> shadeOpacities) {
			int focus = 100;
			DrawingFillFocus propFocus = ArtProperties.GetPropertyByType(typeof(DrawingFillFocus)) as DrawingFillFocus;
			if (propFocus != null)
				focus = propFocus.Value;
			double fillOpacity = GetFillOpacity(typeof(DrawingFillOpacity));
			double fillBackOpacity = GetFillOpacity(typeof(DrawingFillBackOpacity));
			if (focus <= -100 || focus >= 100) {
				shadeColors.Add(new OfficeShadeColor() { ColorRecord = GetFillColor(typeof(DrawingFillColor)), Position = 0.0 });
				shadeColors.Add(new OfficeShadeColor() { ColorRecord = GetFillColor(typeof(DrawingFillBackColor)), Position = 1.0 });
				shadeOpacities.Add(fillOpacity);
				shadeOpacities.Add(fillBackOpacity);
			}
			else if (focus < 0) {
				shadeColors.Add(new OfficeShadeColor() { ColorRecord = GetFillColor(typeof(DrawingFillColor)), Position = 0.0 });
				shadeColors.Add(new OfficeShadeColor() { ColorRecord = GetFillColor(typeof(DrawingFillBackColor)), Position = -focus / 100.0 });
				shadeColors.Add(new OfficeShadeColor() { ColorRecord = GetFillColor(typeof(DrawingFillColor)), Position = 1.0 });
				shadeOpacities.Add(fillOpacity);
				shadeOpacities.Add(fillBackOpacity);
				shadeOpacities.Add(fillOpacity);
			}
			else if (focus > 0) {
				shadeColors.Add(new OfficeShadeColor() { ColorRecord = GetFillColor(typeof(DrawingFillBackColor)), Position = 0.0 });
				shadeColors.Add(new OfficeShadeColor() { ColorRecord = GetFillColor(typeof(DrawingFillColor)), Position = focus / 100.0 });
				shadeColors.Add(new OfficeShadeColor() { ColorRecord = GetFillColor(typeof(DrawingFillBackColor)), Position = 1.0 });
				shadeOpacities.Add(fillBackOpacity);
				shadeOpacities.Add(fillOpacity);
				shadeOpacities.Add(fillBackOpacity);
			}
			else { 
				shadeColors.Add(new OfficeShadeColor() { ColorRecord = GetFillColor(typeof(DrawingFillBackColor)), Position = 0.0 });
				shadeColors.Add(new OfficeShadeColor() { ColorRecord = GetFillColor(typeof(DrawingFillColor)), Position = 1.0 });
				shadeOpacities.Add(fillBackOpacity);
				shadeOpacities.Add(fillOpacity);
			}
		}
		void SetupGradientAngle(DrawingGradientFill gradientFill) {
			double angle = 0.0;
			DrawingFillAngle prop = ArtProperties.GetPropertyByType(typeof(DrawingFillAngle)) as DrawingFillAngle;
			if (prop != null)
				angle = prop.Value;
			if (angle >= 0)
				angle = 90.0 - angle;
			else
				angle = -(angle + 90.0);
			if (angle < 0.0)
				angle += 360.0;
			if (angle > 360.0)
				angle -= 360.0;
			gradientFill.Angle = (int)(angle * DrawingValueConstants.OnePositiveFixedAngle);
		}
		void SetupFillRect(DrawingGradientFill fill) {
			double left = GetRelativePosition(typeof(DrawingFillToLeft));
			double top = GetRelativePosition(typeof(DrawingFillToTop));
			double right = 1.0 - GetRelativePosition(typeof(DrawingFillToRight));
			double bottom = 1.0 - GetRelativePosition(typeof(DrawingFillToBottom));
			int leftOffset = (int)(left * DrawingValueConstants.ThousandthOfPercentage);
			int topOffset = (int)(top * DrawingValueConstants.ThousandthOfPercentage);
			int rightOffset = (int)(right * DrawingValueConstants.ThousandthOfPercentage);
			int bottomOffset = (int)(bottom * DrawingValueConstants.ThousandthOfPercentage);
			fill.FillRect = new RectangleOffset(bottomOffset, leftOffset, rightOffset, topOffset);
		}
		double GetRelativePosition(Type propType) {
			OfficeDrawingFixedPointPropertyBase prop = ArtProperties.GetPropertyByType(propType) as OfficeDrawingFixedPointPropertyBase;
			if (prop != null)
				return prop.Value;
			return 0.0;
		}
		#endregion
		#endregion
		#region CalcSpCheckSum
		public void CalcSpCheckSum(MsoCrc32Compute crc32) {
			OfficeArtPropertiesCheckSumHelper helper = new OfficeArtPropertiesCheckSumHelper(crc32, ArtProperties);
			helper.CalcIntPropertyCheckSum(0x0180, typeof(OfficeDrawingFillType), (int)OfficeFillType.Solid);
			helper.CalcColorPropertyCheckSum(0x0181, typeof(DrawingFillColor), 0x00ffffff);
			helper.CalcFixedPointCheckSum(0x0182, typeof(DrawingFillOpacity), 0x00010000);
			helper.CalcColorPropertyCheckSum(0x0183, typeof(DrawingFillBackColor), 0x00ffffff);
			helper.CalcFixedPointCheckSum(0x0184, typeof(DrawingFillBackOpacity), 0x00010000);
			helper.CalcColorPropertyCheckSum(0x0185, typeof(DrawingFillBWColor), 0x20000000);
			helper.CalcComplexPropertyCheckSum(0x0186, typeof(DrawingFillBlip), 8, 16);
			helper.CalcComplexPropertyCheckSum(0x0187, typeof(DrawingFillBlipName));
			helper.CalcIntPropertyCheckSum(0x0188, typeof(DrawingFillBlipFlags), 0);
			helper.CalcIntPropertyCheckSum(0x0189, typeof(DrawingFillWidth), 0);
			helper.CalcIntPropertyCheckSum(0x018a, typeof(DrawingFillHeight), 0);
			helper.CalcFixedPointCheckSum(0x018b, typeof(DrawingFillAngle), 0);
			helper.CalcIntPropertyCheckSum(0x018c, typeof(DrawingFillFocus), 0);
			helper.CalcFixedPointCheckSum(0x018d, typeof(DrawingFillToLeft), 0);
			helper.CalcFixedPointCheckSum(0x018e, typeof(DrawingFillToTop), 0);
			helper.CalcFixedPointCheckSum(0x018f, typeof(DrawingFillToRight), 0);
			helper.CalcFixedPointCheckSum(0x0190, typeof(DrawingFillToBottom), 0);
			helper.CalcIntPropertyCheckSum(0x0191, typeof(DrawingFillRectLeft), 0);
			helper.CalcIntPropertyCheckSum(0x0192, typeof(DrawingFillRectTop), 0);
			helper.CalcIntPropertyCheckSum(0x0193, typeof(DrawingFillRectRight), 0);
			helper.CalcIntPropertyCheckSum(0x0194, typeof(DrawingFillRectBottom), 0);
			helper.CalcIntPropertyCheckSum(0x0195, typeof(DrawingFillDzType), 0);
			helper.CalcIntPropertyCheckSum(0x0196, typeof(DrawingFillShadePreset), 0);
			helper.CalcComplexPropertyCheckSum(0x0197, typeof(DrawingFillShadeColors));
			helper.CalcFixedPointCheckSum(0x0198, typeof(DrawingFillOriginX), 0);
			helper.CalcFixedPointCheckSum(0x0199, typeof(DrawingFillOriginY), 0);
			helper.CalcFixedPointCheckSum(0x019a, typeof(DrawingFillShapeOriginX), 0);
			helper.CalcFixedPointCheckSum(0x019b, typeof(DrawingFillShapeOriginY), 0);
			helper.CalcIntPropertyCheckSum(0x019c, typeof(DrawingFillShadeType), 0);
			helper = new OfficeArtPropertiesCheckSumHelper(crc32, ArtTertiaryProperties);
			helper.CalcColorPropertyCheckSum(0x019e, typeof(DrawingFillColorExt), -1);
			helper.CalcIntPropertyCheckSum(0x019f, typeof(DrawingFillReserved415), -1);
			helper.CalcIntPropertyCheckSum(0x01a0, typeof(DrawingFillTintShade), 0x20000000);
			helper.CalcIntPropertyCheckSum(0x01a1, typeof(DrawingFillReserved417), 0);
			helper.CalcColorPropertyCheckSum(0x01a2, typeof(DrawingFillBackColorExt), -1);
			helper.CalcIntPropertyCheckSum(0x01a3, typeof(DrawingFillReserved419), -1);
			helper.CalcIntPropertyCheckSum(0x01a4, typeof(DrawingFillBackTintShade), 0x20000000);
			helper.CalcIntPropertyCheckSum(0x01a5, typeof(DrawingFillReserved421), 0);
			helper.CalcIntPropertyCheckSum(0x01a6, typeof(DrawingFillReserved422), -1);
			helper.CalcIntPropertyCheckSum(0x01a7, typeof(DrawingFillReserved423), -1);
			CalcFillStyleBooleanPropertiesCheckSum(crc32);
		}
		void CalcFillStyleBooleanPropertiesCheckSum(MsoCrc32Compute crc32) {
			DrawingFillStyleBooleanProperties prop = ArtProperties.GetPropertyByType(typeof(DrawingFillStyleBooleanProperties)) as DrawingFillStyleBooleanProperties;
			if (prop != null) {
				crc32.Add((int)0x01bb);
				if (prop.UseFilled)
					crc32.Add((int)(prop.Filled ? 1 : 0));
				else
					crc32.Add((int)1);
				crc32.Add((int)0x01bd);
				if (prop.UseFillShape)
					crc32.Add((int)(prop.FillShape ? 1 : 0));
				else
					crc32.Add((int)1);
				crc32.Add((int)0x01be);
				if (prop.UseFillUseRect)
					crc32.Add((int)(prop.FillUseRect ? 1 : 0));
				else
					crc32.Add((int)0);
			}
			else {
				crc32.Add((int)0x01bb);
				crc32.Add((int)1);
				crc32.Add((int)0x01bd);
				crc32.Add((int)1);
				crc32.Add((int)0x01be);
				crc32.Add((int)0);
			}
		}
		#endregion
	}
	#endregion
	#region OfficeArtPropertiesCheckSumHelper
	class OfficeArtPropertiesCheckSumHelper {
		MsoCrc32Compute crc32;
		OfficeArtPropertiesBase properties;
		public OfficeArtPropertiesCheckSumHelper(MsoCrc32Compute crc32, OfficeArtPropertiesBase properties) {
			this.crc32 = crc32;
			this.properties = properties;
		}
		public void CalcIntPropertyCheckSum(int typeCode, Type propType, int defaultValue) {
			crc32.Add(typeCode);
			OfficeDrawingIntPropertyBase prop = OfficeArtPropertiesHelper.GetPropertyByType(properties, propType) as OfficeDrawingIntPropertyBase;
			if (prop != null)
				crc32.Add(prop.Value);
			else
				crc32.Add(defaultValue);
		}
		public void CalcFixedPointCheckSum(int typeCode, Type propType, int defaultValue) {
			crc32.Add(typeCode);
			OfficeDrawingFixedPointPropertyBase prop = OfficeArtPropertiesHelper.GetPropertyByType(properties, propType) as OfficeDrawingFixedPointPropertyBase;
			if (prop != null) {
				FixedPoint helper = new FixedPoint();
				helper.Value = prop.Value;
				crc32.Add(helper.GetBytes());
			}
			else
				crc32.Add(defaultValue);
		}
		public void CalcColorPropertyCheckSum(int typeCode, Type propType, int defaultValue) {
			crc32.Add(typeCode);
			OfficeDrawingColorPropertyBase prop = OfficeArtPropertiesHelper.GetPropertyByType(properties, propType) as OfficeDrawingColorPropertyBase;
			if (prop != null)
				crc32.Add(prop.ColorRecord.GetBytes());
			else
				crc32.Add(defaultValue);
		}
		public void CalcComplexPropertyCheckSum(int typeCode, Type propType) {
			crc32.Add(typeCode);
			OfficeDrawingIntPropertyBase prop = OfficeArtPropertiesHelper.GetPropertyByType(properties, propType) as OfficeDrawingIntPropertyBase;
			if (prop != null) {
				crc32.Add(prop.Value);
				if (prop.Value > 0)
					crc32.Add(prop.ComplexData);
			}
			else
				crc32.Add((int)0);
		}
		public void CalcComplexPropertyCheckSum(int typeCode, Type propType, int start, int count) {
			crc32.Add(typeCode);
			OfficeDrawingIntPropertyBase prop = OfficeArtPropertiesHelper.GetPropertyByType(properties, propType) as OfficeDrawingIntPropertyBase;
			if (prop != null) {
				crc32.Add(prop.Value);
				if (prop.Value > (start + count))
					crc32.Add(prop.ComplexData, start, count);
			}
			else
				crc32.Add((int)0);
		}
	}
	#endregion
	#region IXlsChartMarkerFormat
	public interface IXlsChartMarkerFormat {
		bool Apply { get; set; }
		Color ForegroundColor { get; set; }
		Color BackgroundColor { get; set; }
		MarkerStyle MarkerSymbol { get; set; }
		int MarkerSize { get; set; }
		bool ShowInterior { get; set; }
		bool ShowBorder { get; set; }
	}
	#endregion
	#region XlsChartMarkerFormat
	public class XlsChartMarkerFormat : IXlsChartMarkerFormat {
		public XlsChartMarkerFormat() {
			MarkerSymbol = MarkerStyle.Auto;
		}
		#region IXlsChartMarkerFormat Members
		public bool Apply { get; set; }
		public Color ForegroundColor { get; set; }
		public Color BackgroundColor { get; set; }
		public MarkerStyle MarkerSymbol { get; set; }
		public int MarkerSize { get; set; }
		public bool ShowInterior { get; set; }
		public bool ShowBorder { get; set; }
		#endregion
		public void SetupMarker(Marker marker, XlsChartShapeFormat shapeFormat) {
			if (Apply) {
				marker.Symbol = MarkerSymbol;
				marker.Size = MarkerSize;
				ShapeProperties shapeProperties = marker.ShapeProperties;
				if (shapeFormat != null)
					shapeFormat.SetupShapeProperties(shapeProperties);
				else {
					DocumentModel documentModel = marker.DocumentModel;
					if (ShowInterior) {
						DrawingSolidFill solidFill = new DrawingSolidFill(documentModel);
						solidFill.Color.OriginalColor.Rgb = BackgroundColor;
						shapeProperties.Fill = solidFill;
					}
					else
						shapeProperties.Fill = DrawingFill.None;
					Outline outline = shapeProperties.Outline;
					if(ShowBorder) {
						DrawingSolidFill solidFill = new DrawingSolidFill(documentModel);
						solidFill.Color.OriginalColor.Rgb = ForegroundColor;
						outline.Fill = solidFill;
					}
					else
						outline.Fill = DrawingFill.None;
				}
			}
		}
	}
	#endregion
	#region XlsChartSeriesFormat
	public class XlsChartSeriesFormat {
		#region Properties
		public bool Apply { get; set; }
		public bool SmoothLine { get; set; }
		public bool Bubbles3D { get; set; }
		public bool MarkerShadow { get; set; }
		#endregion
	}
	#endregion
	#region XlsChartDataLabelFormat
	public class XlsChartDataLabelFormat {
		#region Properties
		public bool Apply { get; set; }
		public bool ShowValue { get; set; }
		public bool ShowPercent { get; set; }
		public bool ShowLabelAndPercent { get; set; }
		public bool ShowLabel { get; set; }
		public bool ShowBubbleSizes { get; set; }
		public bool ShowSeriesName { get; set; }
		#endregion
		public void SetupDataLabel(DataLabelBase dataLabel) {
			if (!Apply)
				return;
			dataLabel.BeginUpdate();
			try {
				dataLabel.ShowCategoryName = ShowLabel || ShowLabelAndPercent;
				dataLabel.ShowPercent = ShowPercent || ShowLabelAndPercent;
				dataLabel.ShowSeriesName = ShowSeriesName;
				dataLabel.ShowValue = ShowValue;
				dataLabel.ShowBubbleSize = ShowBubbleSizes;
			}
			finally {
				dataLabel.EndUpdate();
			}
		}
		public void SetupDataLabelForAreaOrFilledRadar(DataLabelBase dataLabel) {
			if (!Apply)
				return;
			dataLabel.BeginUpdate();
			try {
				dataLabel.ShowSeriesName = ShowLabel;
				dataLabel.ShowValue = ShowValue;
			}
			finally {
				dataLabel.EndUpdate();
			}
		}
	}
	#endregion
	#region IXlsChartShapeFormatContainer
	public interface IXlsChartShapeFormatContainer {
		void Add(XlsChartShapeFormat properties);
	}
	#endregion
	#region XlsChartShapeFormat
	public class XlsChartShapeFormat {
		#region Fields
		byte[] content = new byte[0];
		#endregion
		#region Properties
		public int ObjContext { get; set; }
		public int CheckSum { get; set; }
		public byte[] Content {
			get { return content; }
			set {
				if (value == null)
					content = new byte[0];
				else
					content = value;
			}
		}
		#endregion
		public static XlsChartShapeFormat FromStream(BinaryReader reader) {
			XlsChartShapeFormat result = new XlsChartShapeFormat();
			result.Read(reader);
			return result;
		}
		protected void Read(BinaryReader reader) {
			ObjContext = reader.ReadUInt16();
			reader.ReadUInt16(); 
			CheckSum = reader.ReadInt32();
			int contentLength = reader.ReadInt32();
			if (contentLength > 0)
				Content = reader.ReadBytes(contentLength);
			else
				Content = null;
		}
		public void Write(BinaryWriter writer) {
			writer.Write((ushort)ObjContext);
			writer.Write((ushort)0); 
			writer.Write(CheckSum);
			int contentLength = Content.Length;
			writer.Write(contentLength);
			if (contentLength > 0)
				writer.Write(Content);
		}
		public void SetupShapeProperties(ShapeProperties properties) {
			properties.ResetToStyle();
			int contentLength = Content.Length;
			if (contentLength > 0) {
				using (MemoryStream contentStream = new MemoryStream(Content, false)) {
					using (XlsChartShapePropStreamImporter importer = new XlsChartShapePropStreamImporter(properties)) {
						importer.Import(contentStream);
					}
				}
				if (properties.Fill.FillType == DrawingFillType.Picture)
					properties.Fill = DrawingFill.Automatic;
			}
		}
	}
	#endregion
	#region XlsChartShapeFormatCollection
	public class XlsChartShapeFormatCollection : List<XlsChartShapeFormat> {
		public XlsChartShapeFormatCollection()
			: base() {
		}
		public XlsChartShapeFormat FindBy(int objContext) {
			foreach (XlsChartShapeFormat item in this)
				if (item.ObjContext == objContext)
					return item;
			return null;
		}
	}
	#endregion
	#region IXlsChartDataFormatContainer
	public interface IXlsChartDataFormatContainer {
		void Add(XlsChartDataFormat dataFormat);
	}
	#endregion
	#region XlsChartDataFormat
	public class XlsChartDataFormat : IXlsChartBuilder, IXlsChartLineFormat, IXlsChartAreaFormat, IXlsChartGraphicFormat, IXlsChartMarkerFormat, IXlsChartExtPropertyVisitor, IXlsChartShapeFormatContainer {
		#region Fields
		XlsChartLineFormat lineFormat = new XlsChartLineFormat();
		XlsChartAreaFormat areaFormat = new XlsChartAreaFormat();
		XlsChartGraphicFormat graphicFormat = new XlsChartGraphicFormat();
		XlsChartMarkerFormat markerFormat = new XlsChartMarkerFormat();
		XlsChartDataLabelFormat dataLabelFormat = new XlsChartDataLabelFormat();
		int pointIndex = XlsChartDefs.PointIndexOfSeries;
		BarShape shape = BarShape.Auto;
		int explosion = -1;
		XlsChartSeriesFormat seriesFormat = new XlsChartSeriesFormat();
		XlsChartShapeFormatCollection shapeFormats = new XlsChartShapeFormatCollection();
		#endregion
		#region Properties
		public IXlsChartDataFormatContainer Container { get; set; }
		public XlsChartLineFormat LineFormat { get { return lineFormat; } }
		public XlsChartAreaFormat AreaFormat { get { return areaFormat; } }
		public XlsChartGraphicFormat GraphicFormat { get { return graphicFormat; } }
		public XlsChartMarkerFormat MarkerFormat { get { return markerFormat; } }
		public XlsChartDataLabelFormat DataLabelFormat { get { return dataLabelFormat; } }
		public int PointIndex { get { return pointIndex; } set { pointIndex = value; } }
		public bool IsSeriesDataFormat { get { return pointIndex == XlsChartDefs.PointIndexOfSeries; } }
		public bool IsPointDataFormat { get { return pointIndex >= 0 && pointIndex <= XlsChartDefs.MaxPointIndex; } }
		public int SeriesIndex { get; set; }
		public int SeriesOrder { get; set; }
		public BarShape Shape { get { return shape; } set { shape = value; } }
		public bool HasExplosion { get { return explosion >= 0; } }
		public int Explosion { get { return explosion; } set { explosion = value; } }
		public XlsChartSeriesFormat SeriesFormat { get { return seriesFormat; } }
		public XlsChartShapeFormatCollection ShapeFormats { get { return shapeFormats; } }
		#endregion
		#region IXlsChartBuilder Members
		public void Execute(XlsContentBuilder contentBuilder) {
			if (Container != null)
				Container.Add(this);
		}
		#endregion
		#region IXlsChartLineFormat Members
		bool IXlsChartLineFormat.Apply {
			get { return lineFormat.Apply; }
			set { lineFormat.Apply = value; }
		}
		bool IXlsChartLineFormat.Auto {
			get { return lineFormat.Auto; }
			set { lineFormat.Auto = value; }
		}
		bool IXlsChartLineFormat.AxisVisible {
			get { return lineFormat.AxisVisible; }
			set { lineFormat.AxisVisible = value; }
		}
		XlsChartLineStyle IXlsChartLineFormat.LineStyle {
			get { return lineFormat.LineStyle; }
			set { lineFormat.LineStyle = value; }
		}
		XlsChartLineThickness IXlsChartLineFormat.Thickness {
			get { return lineFormat.Thickness; }
			set { lineFormat.Thickness = value; }
		}
		bool IXlsChartLineFormat.AutoColor {
			get { return lineFormat.AutoColor; }
			set { lineFormat.AutoColor = value; }
		}
		Color IXlsChartLineFormat.LineColor {
			get { return lineFormat.LineColor; }
			set { lineFormat.LineColor = value; }
		}
		int IXlsChartLineFormat.LineColorIndex {
			get { return lineFormat.LineColorIndex; }
			set { lineFormat.LineColorIndex = value; }
		}
		#endregion
		#region IXlsChartAreaFormat Members
		bool IXlsChartAreaFormat.Apply {
			get { return areaFormat.Apply; }
			set { areaFormat.Apply = value; }
		}
		Color IXlsChartAreaFormat.ForegroundColor {
			get { return areaFormat.ForegroundColor; }
			set { areaFormat.ForegroundColor = value; }
		}
		Color IXlsChartAreaFormat.BackgroundColor {
			get { return areaFormat.BackgroundColor; }
			set { areaFormat.BackgroundColor = value; }
		}
		XlsChartFillType IXlsChartAreaFormat.FillType {
			get { return areaFormat.FillType; }
			set { areaFormat.FillType = value; }
		}
		bool IXlsChartAreaFormat.AutoColor {
			get { return areaFormat.AutoColor; }
			set { areaFormat.AutoColor = value; }
		}
		bool IXlsChartAreaFormat.InvertIfNegative {
			get { return areaFormat.InvertIfNegative; }
			set { areaFormat.InvertIfNegative = value; }
		}
		#endregion
		#region IXlsChartGraphicFormat Members
		bool IXlsChartGraphicFormat.Apply {
			get { return graphicFormat.Apply; }
			set { graphicFormat.Apply = value; }
		}
		OfficeArtProperties IXlsChartGraphicFormat.ArtProperties {
			get { return graphicFormat.ArtProperties; }
			set { graphicFormat.ArtProperties = value; }
		}
		OfficeArtTertiaryProperties IXlsChartGraphicFormat.ArtTertiaryProperties {
			get { return graphicFormat.ArtTertiaryProperties; }
			set { graphicFormat.ArtTertiaryProperties = value; }
		}
		#endregion
		#region IXlsChartMarkerFormat Members
		bool IXlsChartMarkerFormat.Apply {
			get { return markerFormat.Apply; }
			set { markerFormat.Apply = value; }
		}
		Color IXlsChartMarkerFormat.ForegroundColor {
			get { return markerFormat.ForegroundColor; }
			set { markerFormat.ForegroundColor = value; }
		}
		Color IXlsChartMarkerFormat.BackgroundColor {
			get { return markerFormat.BackgroundColor; }
			set { markerFormat.BackgroundColor = value; }
		}
		MarkerStyle IXlsChartMarkerFormat.MarkerSymbol {
			get { return markerFormat.MarkerSymbol; }
			set { markerFormat.MarkerSymbol = value; }
		}
		bool IXlsChartMarkerFormat.ShowInterior {
			get { return markerFormat.ShowInterior; }
			set { markerFormat.ShowInterior = value; }
		}
		bool IXlsChartMarkerFormat.ShowBorder {
			get { return markerFormat.ShowBorder; }
			set { markerFormat.ShowBorder = value; }
		}
		int IXlsChartMarkerFormat.MarkerSize {
			get { return markerFormat.MarkerSize; }
			set { markerFormat.MarkerSize = value; }
		}
		#endregion
		#region IXlsChartExtPropertyVisitor Members
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropScaleMax item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropScaleMin item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropLogBase item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropStyle item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropThemeOverride item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropColorMapOverride item) { }
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
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropCultureCode item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropSymbol item) {
			switch (item.Value) {
				case 0x0023: MarkerFormat.MarkerSymbol = MarkerStyle.None; break;
				case 0x0024: MarkerFormat.MarkerSymbol = MarkerStyle.Diamond; break;
				case 0x0025: MarkerFormat.MarkerSymbol = MarkerStyle.Square; break;
				case 0x0026: MarkerFormat.MarkerSymbol = MarkerStyle.Triangle; break;
				case 0x0027: MarkerFormat.MarkerSymbol = MarkerStyle.X; break;
				case 0x0028: MarkerFormat.MarkerSymbol = MarkerStyle.Star; break;
				case 0x0029: MarkerFormat.MarkerSymbol = MarkerStyle.Dot; break;
				case 0x002a: MarkerFormat.MarkerSymbol = MarkerStyle.Dash; break;
				case 0x002b: MarkerFormat.MarkerSymbol = MarkerStyle.Circle; break;
				case 0x002c: MarkerFormat.MarkerSymbol = MarkerStyle.Plus; break;
				case 0x002d: MarkerFormat.MarkerSymbol = MarkerStyle.Auto; break;
			}
		}
		#endregion
		#region IXlsChartShapePropertiesContainer Members
		void IXlsChartShapeFormatContainer.Add(XlsChartShapeFormat properties) {
			if (IsValidSpCheckSum(properties.ObjContext, properties.CheckSum))
				ShapeFormats.Add(properties);
		}
		bool IsValidSpCheckSum(int objContext, int checkSum) {
			MsoCrc32Compute crc32 = new MsoCrc32Compute();
			if (objContext == XlsChartDefs.DataContext) {
				if (GraphicFormat.Apply)
					return true; 
				LineFormat.CalcSpCheckSum(crc32);
				if (GraphicFormat.Apply && AreaFormat.Apply && !AreaFormat.AutoColor)
					GraphicFormat.CalcSpCheckSum(crc32);
				else
					AreaFormat.CalcSpCheckSum(crc32);
			}
			else if (objContext == XlsChartDefs.MarkerContext) {
				XlsChartAreaFormat format = AreaFormat.Clone();
				format.Apply &= MarkerFormat.Apply; 
				format.ForegroundColor = MarkerFormat.ForegroundColor;
				format.BackgroundColor = MarkerFormat.BackgroundColor;
				format.CalcSpCheckSum(crc32);
			}
			return crc32.CrcValue == checkSum;
		}
		#endregion
		#region SetupSeries
		public void SetupSeries(ISeries series) {
			if (IsSeriesDataFormat) {
				SetupShapeProperties(((SeriesBase)series).ShapeProperties, FillApplicable(series));
				SetupSeriesBarShape(series as BarSeries);
				SetupSeriesExplosion(series as PieSeries);
				SetupSeriesBubble3D(series as BubbleSeries);
				SetupSeriesSmooth(series as SeriesWithMarkerAndSmooth);
				SetupSeriesMarker(series as ISeriesWithMarker);
			}
			if (IsPointDataFormat) 
				SetupSeriesDataPoint(series as SeriesWithDataLabelsAndPoints);
		}
		void SetupSeriesBarShape(BarSeries series) {
			if (series == null || !series.View.Is3DView)
				return;
			series.Shape = Shape;
		}
		void SetupSeriesExplosion(PieSeries series) {
			if (series != null && HasExplosion)
				series.Explosion = Explosion;
		}
		void SetupSeriesBubble3D(BubbleSeries series) {
			if (series == null)
				return;
			series.Bubble3D = GetActualBubbles3D(series);
		}
		void SetupSeriesSmooth(SeriesWithMarkerAndSmooth series) {
			if (series == null)
				return;
			if (SeriesFormat.Apply)
				series.Smooth = SeriesFormat.SmoothLine;
			else {
				LineChartView view = series.View as LineChartView;
				series.Smooth = (view != null) ? view.Smooth : false;
			}
		}
		void SetupSeriesMarker(ISeriesWithMarker series) {
			if (series == null)
				return;
			MarkerFormat.SetupMarker(series.Marker, ShapeFormats.FindBy(XlsChartDefs.MarkerContext));
		}
		void SetupSeriesDataPoint(SeriesWithDataLabelsAndPoints series) {
			if (series == null)
				return;
			DataPoint point = series.DataPoints.CreateDataPoint(PointIndex);
			point.Bubble3D = GetActualBubbles3D(series);
			if (HasExplosion) {
				point.HasExplosion = true;
				point.Explosion = Explosion;
			}
			MarkerFormat.SetupMarker(point.Marker, ShapeFormats.FindBy(XlsChartDefs.MarkerContext));
			SetupShapeProperties(point.ShapeProperties, FillApplicable(series));
			if (IsFakeDataPoint(series, point))
				series.DataPoints.RemoveByIndex(PointIndex);
		}
		bool GetActualBubbles3D(ISeries series) {
			if (SeriesFormat.Apply)
				return SeriesFormat.Bubbles3D;
			BubbleChartView view = series.View as BubbleChartView;
			if (view != null)
				return view.Bubble3D;
			return false;
		}
		bool IsFakeDataPoint(SeriesWithDataLabelsAndPoints series, DataPoint dataPoint) {
			return IsSameBubble3D(series as BubbleSeries, dataPoint) &&
				IsSameInvertIfNegative(series as ISupportsInvertIfNegative, dataPoint) &&
				IsSameExplosion(series as PieSeries, dataPoint) &&
				IsSameMarker(series as ISeriesWithMarker, dataPoint) &&
				dataPoint.ShapeProperties.IsAutomatic;
		}
		bool IsSameBubble3D(BubbleSeries series, DataPoint dataPoint) {
			if (series == null)
				return true;
			return series.Bubble3D == dataPoint.Bubble3D;
		}
		bool IsSameInvertIfNegative(ISupportsInvertIfNegative series, DataPoint dataPoint) {
			if (series == null)
				return true;
			return series.InvertIfNegative == dataPoint.InvertIfNegative;
		}
		bool IsSameExplosion(PieSeries series, DataPoint dataPoint) {
			if (series == null || !dataPoint.HasExplosion)
				return true;
			return series.Explosion == dataPoint.Explosion;
		}
		bool IsSameMarker(ISeriesWithMarker series, DataPoint dataPoint) {
			if (series == null)
				return true;
			return series.Marker.Symbol == dataPoint.Marker.Symbol &&
				series.Marker.Size == dataPoint.Marker.Size && 
				dataPoint.Marker.ShapeProperties.IsAutomatic;
		}
		protected internal void SetupSeriesDataLabels(SeriesWithDataLabelsAndPoints series) {
			if (!DataLabelFormat.Apply)
				return;
			if (IsSeriesDataFormat)
				SetupDataLabels(series);
			else if (IsPointDataFormat) {
				DataLabel label = new DataLabel(series.Parent, PointIndex);
				DataLabelFormat.SetupDataLabel(label);
				series.DataLabels.Labels.Add(label);
			}
			series.DataLabels.Apply = true;
		}
		protected internal void SetupDataLabels(SeriesWithDataLabelsAndPoints series) {
			DataLabels dataLabels = series.DataLabels;
			if (IsAreaOrFilledRadar(series.View))
				DataLabelFormat.SetupDataLabelForAreaOrFilledRadar(dataLabels);
			else
				DataLabelFormat.SetupDataLabel(dataLabels);
		}
		void SetupShapeProperties(ShapeProperties shapeProperties, bool fillApplicable) {
			XlsChartShapeFormat shapeFormat = ShapeFormats.FindBy(XlsChartDefs.DataContext);
			if (shapeFormat != null)
				shapeFormat.SetupShapeProperties(shapeProperties);
			else {
				LineFormat.SetupShapeProperties(shapeProperties);
				if(fillApplicable)
					AreaFormat.SetupShapeProperties(shapeProperties);
			}
		}
		bool FillApplicable(ISeries series) {
			if (series.SeriesType == ChartSeriesType.Radar)
				return false;
			if (series.SeriesType == ChartSeriesType.Scatter)
				return false;
			if (series.SeriesType == ChartSeriesType.Surface)
				return false;
			if (series.SeriesType == ChartSeriesType.Line && !series.View.Is3DView)
				return false;
			return true;
		}
		bool IsAreaOrFilledRadar(IChartView view) {
			ChartViewType viewType = view.ViewType;
			if (viewType == ChartViewType.Radar) {
				RadarChartView radarView = view as RadarChartView;
				return radarView.RadarStyle == RadarChartStyle.Filled;
			}
			return viewType == ChartViewType.Area || viewType == ChartViewType.Area3D;
		}
		#endregion
		#region SetupView
		public void SetupView(IChartView view) {
			SetupViewBarShape(view as Bar3DChartView);
			SetupViewBubble3D(view as BubbleChartView);
			SetupViewSmoothAndMarker(view as LineChartView);
		}
		void SetupViewBarShape(Bar3DChartView view) {
			if (view != null)
				view.Shape = Shape;
		}
		void SetupViewBubble3D(BubbleChartView view) {
			if (view != null && SeriesFormat.Apply)
				view.Bubble3D = SeriesFormat.Bubbles3D;
		}
		void SetupViewSmoothAndMarker(LineChartView view) {
			if (view != null) {
				if(SeriesFormat.Apply)
					view.Smooth = SeriesFormat.SmoothLine;
				if (MarkerFormat.Apply)
					view.ShowMarker = MarkerFormat.MarkerSymbol != MarkerStyle.None;
			}
		}
		#endregion
	}
	#endregion
	#region IXlsChartFontX
	public interface IXlsChartFontX {
		bool Apply { get; set; }
		int Index { get; set; }
	}
	#endregion
	#region XlsChartFontX
	public class XlsChartFontX : IXlsChartFontX {
		#region IXlsChartFontX Members
		public bool Apply { get; set; }
		public int Index { get; set; }
		#endregion
		protected internal void SetupParagraphDefaults(XlsContentBuilder contentBuilder, DrawingTextCharacterProperties defaultProperties) {
			DocumentModel documentModel = contentBuilder.DocumentModel;
			int fontInfoIndex = contentBuilder.StyleSheet.GetFontInfoIndex(Index);
			RunFontInfo fontInfo = documentModel.Cache.FontInfoCache[fontInfoIndex];
			XlsCharacterPropertiesHelper.SetupCharacterPropertiesWithoutFill(defaultProperties, fontInfo);
		}
	}
	#endregion
	#region IXlsChartTextFormatContainer
	public interface IXlsChartTextFormatContainer {
		void Add(XlsChartTextFormat properties);
	}
	#endregion
	#region XlsChartTextFormat
	public class XlsChartTextFormat {
		#region Fields
		byte[] content = new byte[0];
		#endregion
		#region Properties
		public int CheckSum { get; set; }
		public byte[] Content {
			get { return content; }
			set {
				if (value == null)
					content = new byte[0];
				else
					content = value;
			}
		}
		#endregion
		public static XlsChartTextFormat FromStream(BinaryReader reader) {
			XlsChartTextFormat result = new XlsChartTextFormat();
			result.Read(reader);
			return result;
		}
		protected void Read(BinaryReader reader) {
			CheckSum = reader.ReadInt32();
			int contentLength = reader.ReadInt32();
			if (contentLength > 0)
				Content = reader.ReadBytes(contentLength);
			else
				Content = null;
		}
		public void Write(BinaryWriter writer) {
			writer.Write(CheckSum);
			int contentLength = Content.Length;
			writer.Write(contentLength);
			if (contentLength > 0)
				writer.Write(Content);
		}
		public void SetupTextProperties(TextProperties properties) {
			properties.ResetToStyle();
			int contentLength = Content.Length;
			if (contentLength > 0) {
				using (MemoryStream contentStream = new MemoryStream(Content, false)) {
					using (XlsChartTextPropStreamImporter importer = new XlsChartTextPropStreamImporter(properties)) {
						importer.Import(contentStream);
					}
				}
			}
		}
		public bool IsValidCheckSum(XlsContentBuilder contentBuilder, XlsChartTextBuilder textBuilder) {
			textBuilder = GetTextBuilder(contentBuilder, textBuilder);
			if (textBuilder == null)
				return true;
			return IsValidCheckSum(contentBuilder, textBuilder, textBuilder.FontX);
		}
		public bool IsValidCheckSum(XlsContentBuilder contentBuilder, XlsChartTextBuilder textBuilder, XlsChartFontX fontX) {
			textBuilder = GetTextBuilder(contentBuilder, textBuilder);
			if (textBuilder == null)
				return true;
			if (fontX.Apply && fontX.Index >= contentBuilder.StyleSheet.Fonts.Count)
				return true;
			MsoCrc32Compute crc32 = new MsoCrc32Compute();
			using (MemoryStream ms = new MemoryStream()) {
				using (BinaryWriter writer = new BinaryWriter(ms)) {
					if (fontX.Apply) {
						XlsFontInfo fontInfo = contentBuilder.StyleSheet.Fonts[fontX.Index];
						byte[] nameBytes = XLStringEncoder.GetBytes(fontInfo.Name, true);
						writer.Write(nameBytes); 
						writer.Write((int)(fontInfo.Size * XlsCommandFont.FontCoeff)); 
						ushort bitwiseField = 0;
						if (fontInfo.Boldness > XlsCommandFont.DefaultNormal)
							bitwiseField |= 0x0001;
						if (fontInfo.Italic)
							bitwiseField |= 0x0002;
						if (fontInfo.Underline != XlUnderlineType.None)
							bitwiseField |= 0x0004;
						if (fontInfo.Outline)
							bitwiseField |= 0x0008;
						if (fontInfo.Shadow)
							bitwiseField |= 0x0010;
						if (fontInfo.Condense)
							bitwiseField |= 0x0020;
						if (fontInfo.Extend)
							bitwiseField |= 0x0040;
						if (fontInfo.StrikeThrough)
							bitwiseField |= 0x0080;
						bitwiseField |= 0x0100; 
						int pixelHeight = (int)contentBuilder.DocumentModel.LayoutUnitConverter.LayoutUnitsToPixelsF(
							contentBuilder.DocumentModel.LayoutUnitConverter.PointsToLayoutUnitsF((float)fontInfo.Size),
							DocumentModel.Dpi);
						if (pixelHeight < 6)
							bitwiseField |= 0x0200;
						writer.Write(bitwiseField);
						writer.Write((short)fontInfo.Boldness);
						writer.Write((short)fontInfo.Script);
						writer.Write((byte)fontInfo.Underline);
						writer.Write((byte)fontInfo.FontFamily);
						writer.Write((byte)fontInfo.Charset);
						writer.Write((byte)0); 
						XlsLongRGB.Write(writer, contentBuilder.DocumentModel.StyleSheet.Palette[fontInfo.FontColorIndex]);
					}
					writer.Write((int)(textBuilder.IsTransparent ? 0x010d : 0x020d));
					writer.Write((byte)textBuilder.TextRotation);
					writer.Write((int)XlsCommandChartText.DrawingTextAlignmentTypeToHorizontalText(textBuilder.HorizontalAlignment));
					writer.Write((int)XlsCommandChartText.DrawingTextAnchoringTypeToVerticalText(textBuilder.VerticalAlignment));
					writer.Write((byte)textBuilder.TextReadingOrder);
					byte[] data = ms.ToArray();
					crc32.Add(data);
				}
			}
			return crc32.CrcValue == CheckSum;
		}
		XlsChartTextBuilder GetTextBuilder(XlsContentBuilder contentBuilder, XlsChartTextBuilder textBuilder) {
			if (textBuilder == null)
				textBuilder = ((XlsChartRootBuilder)contentBuilder.ChartRootBuilder).DefaultText.FindBy(2);
			return textBuilder;
		}
	}
	#endregion
}
