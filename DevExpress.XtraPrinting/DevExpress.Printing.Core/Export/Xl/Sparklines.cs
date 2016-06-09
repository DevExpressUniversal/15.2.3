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
using System.Linq;
using System.Text;
using DevExpress.Utils;
namespace DevExpress.Export.Xl {
	#region XlSparklineType
	public enum XlSparklineType {
		Line,
		Column,
		WinLoss
	}
	#endregion
	#region XlSparklineAxisScaling
	public enum XlSparklineAxisScaling {
		Individual,
		Custom,
		Group
	}
	#endregion
	public enum XlDisplayBlanksAs {
		Zero,
		Span,
		Gap
	}
	#region XlSparkline
	public class XlSparkline {
		XlCellRange dataRange;
		XlCellRange location;
		public XlSparkline(XlCellRange dataRange, XlCellRange location) {
			DataRange = dataRange;
			Location = location;
		}
		public XlCellRange DataRange {
			get { return dataRange; }
			set {
				Guard.ArgumentNotNull(value, "DataRange");
				if(value.RowCount > 1 && value.ColumnCount > 1)
					throw new ArgumentException("Data range is invalid");
				dataRange = value;
			}
		}
		public XlCellRange Location {
			get { return location; }
			set {
				Guard.ArgumentNotNull(value, "Location");
				if(value.RowCount != 1 || value.ColumnCount != 1)
					throw new ArgumentException("Location is invalid");
				location = value;
				location.SheetName = string.Empty;
			}
		}
	}
	#endregion
	#region XlSparklineGroup
	public class XlSparklineGroup {
		readonly List<XlSparkline> sparklines = new List<XlSparkline>();
		XlColor colorSeries = XlColor.FromTheme(XlThemeColor.Accent1, -0.5);
		XlColor colorNegative = XlColor.FromTheme(XlThemeColor.Accent2, 0.0);
		XlColor colorAxis = XlColor.FromArgb(0x00, 0x00, 0x00);
		XlColor colorMarker = XlColor.FromTheme(XlThemeColor.Accent1, -0.5);
		XlColor colorFirst = XlColor.FromTheme(XlThemeColor.Accent1, 0.4);
		XlColor colorLast = XlColor.FromTheme(XlThemeColor.Accent1, 0.4);
		XlColor colorHigh = XlColor.FromTheme(XlThemeColor.Accent1, 0.0);
		XlColor colorLow = XlColor.FromTheme(XlThemeColor.Accent1, 0.0);
		double lineWeight = 0.75;
		public XlSparklineGroup() {
		}
		public XlSparklineGroup(XlCellRange dataRange, XlCellRange location) {
			Guard.ArgumentNotNull(dataRange, "dataRange");
			Guard.ArgumentNotNull(location, "location");
			if(location.RowCount > 1 && location.ColumnCount > 1)
				throw new ArgumentException("Location must be one column wide or one row tall");
			if(location.RowCount == 1 && location.ColumnCount == 1) { 
				if(dataRange.RowCount > 1 && dataRange.ColumnCount > 1)
					throw new ArgumentException("Data range must be one column wide or one row tall");
				this.sparklines.Add(new XlSparkline(dataRange, location));
			}
			else if(location.RowCount == 1) {
				if(dataRange.ColumnCount != location.ColumnCount)
					throw new ArgumentException("Data range must have same width as location");
				for(int i = 0; i < location.ColumnCount; i++)
					this.sparklines.Add(new XlSparkline(GetColumnSlice(dataRange, i), GetColumnSlice(location, i)));
			}
			else if(location.ColumnCount == 1) {
				if(dataRange.RowCount != location.RowCount)
					throw new ArgumentException("Data range must have same height as location");
				for(int i = 0; i < location.RowCount; i++)
					this.sparklines.Add(new XlSparkline(GetRowSlice(dataRange, i), GetRowSlice(location, i)));
			}
		}
		#region Properties
		public XlSparklineType SparklineType { get; set; }
		public IList<XlSparkline> Sparklines { get { return sparklines; } }
		public XlColor ColorSeries {
			get { return colorSeries; }
			set { colorSeries = value ?? XlColor.Empty; }
		}
		public XlColor ColorNegative {
			get { return colorNegative; }
			set { colorNegative = value ?? XlColor.Empty; }
		}
		public XlColor ColorAxis {
			get { return colorAxis; }
			set { colorAxis = value ?? XlColor.Empty; }
		}
		public XlColor ColorMarker {
			get { return colorMarker; }
			set { colorMarker = value ?? XlColor.Empty; }
		}
		public XlColor ColorFirst {
			get { return colorFirst; }
			set { colorFirst = value ?? XlColor.Empty; }
		}
		public XlColor ColorLast {
			get { return colorLast; }
			set { colorLast = value ?? XlColor.Empty; }
		}
		public XlColor ColorHigh {
			get { return colorHigh; }
			set { colorHigh = value ?? XlColor.Empty; }
		}
		public XlColor ColorLow {
			get { return colorLow; }
			set { colorLow = value ?? XlColor.Empty; }
		}
		public XlDisplayBlanksAs DisplayBlanksAs { get; set; }
		public double LineWeight {
			get { return lineWeight; }
			set {
				if(value < 0 || value > 1584)
					throw new ArgumentOutOfRangeException("LineWeight out of range 0...1584");
				lineWeight = value;
			}
		}
		public XlCellRange DateRange { get; set; }
		public XlSparklineAxisScaling MinScaling { get; set; }
		public XlSparklineAxisScaling MaxScaling { get; set; }
		public double ManualMax { get; set; }
		public double ManualMin { get; set; }
		public bool HighlightNegative { get; set; }
		public bool HighlightFirst { get; set; }
		public bool HighlightLast { get; set; }
		public bool HighlightHighest { get; set; }
		public bool HighlightLowest { get; set; }
		public bool DisplayXAxis { get; set; }
		public bool DisplayMarkers { get; set; }
		public bool DisplayHidden { get; set; }
		public bool RightToLeft { get; set; }
		#endregion
		XlCellRange GetColumnSlice(XlCellRange range, int index) {
			XlCellPosition topLeft = new XlCellPosition(range.FirstColumn + index, range.FirstRow, range.TopLeft.ColumnType, range.TopLeft.RowType);
			XlCellPosition bottomRight = new XlCellPosition(range.FirstColumn + index, range.LastRow, range.TopLeft.ColumnType, range.TopLeft.RowType);
			return new XlCellRange(range.SheetName, topLeft, bottomRight);
		}
		XlCellRange GetRowSlice(XlCellRange range, int index) {
			XlCellPosition topLeft = new XlCellPosition(range.FirstColumn, range.FirstRow + index, range.TopLeft.ColumnType, range.TopLeft.RowType);
			XlCellPosition bottomRight = new XlCellPosition(range.LastColumn, range.FirstRow + index, range.TopLeft.ColumnType, range.TopLeft.RowType);
			return new XlCellRange(range.SheetName, topLeft, bottomRight);
		}
	}
	#endregion
}
