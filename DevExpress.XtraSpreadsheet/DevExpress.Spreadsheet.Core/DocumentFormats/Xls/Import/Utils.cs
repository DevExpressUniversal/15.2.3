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
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Export.Xls;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.Export.Xl;
using DevExpress.XtraExport.Xls;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsCellRangeFactory
	public static class XlsCellRangeFactory {
		public static CellRangeBase CreateCellRange(IWorksheet sheet, CellPosition topLeft, CellPosition bottomRight) {
			if(topLeft.Column <= 0 && bottomRight.Column >= XlsDefs.MaxColumnCount - 1)
				return CellIntervalRange.CreateRowInterval(sheet, topLeft.Row, topLeft.RowType, bottomRight.Row, bottomRight.RowType);
			if(topLeft.Row <= 0 && bottomRight.Row >= XlsDefs.MaxRowCount - 1)
				return CellIntervalRange.CreateColumnInterval(sheet, topLeft.Column, topLeft.ColumnType, bottomRight.Column, bottomRight.ColumnType);
			return new CellRange(sheet, topLeft, bottomRight);
		}
		public static CellRangeInfo CreateCellRangeInfo(CellRangeBase range) {
			if(range.RangeType == CellRangeType.SingleRange) {
				return new CellRangeInfo(range.TopLeft, range.BottomRight);
			}
			else if(range.RangeType == CellRangeType.IntervalRange) {
				CellIntervalRange intervalRange = range as CellIntervalRange;
				if(intervalRange.IsColumnInterval)
					return new CellRangeInfo(intervalRange.TopLeft, new CellPosition(intervalRange.BottomRight.Column, XlsDefs.MaxRowCount - 1));
				return new CellRangeInfo(intervalRange.TopLeft, new CellPosition(XlsDefs.MaxColumnCount - 1, intervalRange.BottomRight.Row));
			}
			return null;
		}
		public static CellRangeInfo CreateTruncatedCellRangeInfo(CellRangeBase range) {
			if(range.RangeType == CellRangeType.SingleRange) {
				CellPosition bottomRight = new CellPosition(
					Math.Min(XlsDefs.MaxColumnCount - 1, range.BottomRight.Column), 
					Math.Min(XlsDefs.MaxRowCount - 1, range.BottomRight.Row));
				return new CellRangeInfo(range.TopLeft, bottomRight);
			}
			else if(range.RangeType == CellRangeType.IntervalRange) {
				CellIntervalRange intervalRange = range as CellIntervalRange;
				if(intervalRange.IsColumnInterval)
					return new CellRangeInfo(intervalRange.TopLeft, new CellPosition(Math.Min(XlsDefs.MaxColumnCount - 1, intervalRange.BottomRight.Column), XlsDefs.MaxRowCount - 1));
				return new CellRangeInfo(intervalRange.TopLeft, new CellPosition(XlsDefs.MaxColumnCount - 1, Math.Min(XlsDefs.MaxRowCount - 1, intervalRange.BottomRight.Row)));
			}
			return null;
		}
	}
	#endregion
	#region Xnum
	public class Xnum {
		const int fixedSize = 8;
		public double Value { get; set; }
		public int Length { get { return fixedSize; } }
		public void Read(BinaryReader reader) {
			Value = reader.ReadDouble();
		}
		public void Write(BinaryWriter writer) {
			writer.Write(Value);
		}
	}
	#endregion
	#region XlsLongRGB
	public static class XlsLongRGB {
		public static Color FromStream(XlsReader reader) {
			int colorValue = reader.ReadInt32();
			int red = colorValue & 0x0000ff;
			int green = (colorValue & 0x00ff00) >> 8;
			int blue = (colorValue & 0xff0000) >> 16;
			return DXColor.FromArgb(0xff, red, green, blue);
		}
		public static void Write(BinaryWriter writer, Color color) {
			int colorValue = ArgbToValue(color.R, color.G, color.B);
			writer.Write(colorValue);
		}
		static int ArgbToValue(byte red, byte green, byte blue) {
			return (int)red + ((int)green << 8) + ((int)blue << 16);
		}
	}
	#endregion
	#region XlsTextAlignHelper
	public static class XlsTextAlignHelper {
		public static XlHorizontalAlignment HorizontalTextToModelAlign(int align) {
			switch (align) {
				case 1: return XlHorizontalAlignment.Left;
				case 2: return XlHorizontalAlignment.Center;
				case 3: return XlHorizontalAlignment.Right;
				case 4: return XlHorizontalAlignment.Justify;
				case 7: return XlHorizontalAlignment.Distributed;
			}
			return XlHorizontalAlignment.Left;
		}
		public static XlVerticalAlignment VerticalTextToModelAlign(int align) {
			switch (align) {
				case 1: return XlVerticalAlignment.Top;
				case 2: return XlVerticalAlignment.Center;
				case 3: return XlVerticalAlignment.Bottom;
				case 4: return XlVerticalAlignment.Justify;
				case 7: return XlVerticalAlignment.Distributed;
			}
			return XlVerticalAlignment.Top;
		}
		public static int ModelAlignToHorizontalText(XlHorizontalAlignment align) {
			switch (align) {
				case XlHorizontalAlignment.Left: return 1;
				case XlHorizontalAlignment.Center: return 2;
				case XlHorizontalAlignment.Right: return 3;
				case XlHorizontalAlignment.Justify: return 4;
				case XlHorizontalAlignment.Distributed: return 7;
			}
			return 1; 
		}
		public static int ModelAlignToVerticalText(XlVerticalAlignment align) {
			switch (align) {
				case XlVerticalAlignment.Top: return 1;
				case XlVerticalAlignment.Center: return 2;
				case XlVerticalAlignment.Bottom: return 3;
				case XlVerticalAlignment.Justify: return 4;
				case XlVerticalAlignment.Distributed: return 7;
			}
			return 1; 
		}
	}
	#endregion
}
