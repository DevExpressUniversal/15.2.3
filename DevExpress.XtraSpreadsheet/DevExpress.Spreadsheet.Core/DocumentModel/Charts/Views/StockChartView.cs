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
using System.Globalization;
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.Office.Drawing;
namespace DevExpress.XtraSpreadsheet.Model {
	public class StockChartView : ChartViewWithDataLabels, ISupportsDropLines, ISupportsHiLowLines, ISupportsUpDownBars {
		#region Fields
		ShapeProperties dropLinesProperties;
		ShapeProperties hiLowLinesProperties;
		ChartUpDownBars upDownBars;
		#endregion
		public StockChartView(IChart parent)
			: base(parent) {
			this.dropLinesProperties = new ShapeProperties(DocumentModel) { Parent = parent };
			this.hiLowLinesProperties = new ShapeProperties(DocumentModel) { Parent = parent };
			this.upDownBars = new ChartUpDownBars(parent);
		}
		#region Properties
		#region ShowDropLines
		public bool ShowDropLines {
			get { return Info.ShowDropLines; }
			set {
				if (ShowDropLines == value)
					return;
				SetPropertyValue(SetShowDropLinesCore, value);
			}
		}
		DocumentModelChangeActions SetShowDropLinesCore(ChartViewInfo info, bool value) {
			info.ShowDropLines = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		public ShapeProperties DropLinesProperties { get { return dropLinesProperties; } }
		#region ShowHiLowLines
		public bool ShowHiLowLines {
			get { return Info.ShowHiLowLines; }
			set {
				if (ShowHiLowLines == value)
					return;
				SetPropertyValue(SetShowHiLowLinesCore, value);
			}
		}
		DocumentModelChangeActions SetShowHiLowLinesCore(ChartViewInfo info, bool value) {
			info.ShowHiLowLines = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		public ShapeProperties HiLowLinesProperties { get { return hiLowLinesProperties; } }
		#region ShowUpDownBars
		public bool ShowUpDownBars {
			get { return Info.ShowUpDownBars; }
			set {
				if (ShowUpDownBars == value)
					return;
				SetPropertyValue(SetShowUpDownBarsCore, value);
			}
		}
		DocumentModelChangeActions SetShowUpDownBarsCore(ChartViewInfo info, bool value) {
			info.ShowUpDownBars = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		public ChartUpDownBars UpDownBars { get { return upDownBars; } }
		#endregion
		#region IChartView Members
		public override ChartViewType ViewType { get { return ChartViewType.Stock; } }
		public override ChartType ChartType { get { return ShowUpDownBars ? ChartType.StockOpenHighLowClose : ChartType.StockHighLowClose; } }
		public override AxisGroupType AxesType { get { return AxisGroupType.CategoryValue; } }
		public override IChartView CloneTo(IChart parent) {
			StockChartView result = new StockChartView(parent);
			result.CopyFrom(this);
			return result;
		}
		public override void Visit(IChartViewVisitor visitor) {
			visitor.Visit(this);
		}
		public override DataLabelPosition DefaultDataLabelPosition {
			get { return DataLabelPosition.Right; }
		}
		public override ISeries CreateSeriesInstance() {
			LineSeries result = new LineSeries(this);
			if (ShowUpDownBars) {
				result.ShapeProperties.Outline.Fill = DrawingFill.None;
				result.Marker.Symbol = MarkerStyle.None;
			}
			return result;
		}
		#endregion
		protected override void CopyFrom(IChartView value, bool copySeries) {
			base.CopyFrom(value, copySeries);
			ISupportsHiLowLines viewWithHiLowLines = value as ISupportsHiLowLines;
			if (viewWithHiLowLines != null)
				hiLowLinesProperties.CopyFrom(viewWithHiLowLines.HiLowLinesProperties);
			ISupportsDropLines viewWithDropLines = value as ISupportsDropLines;
			if (viewWithDropLines != null)
				dropLinesProperties.CopyFrom(viewWithDropLines.DropLinesProperties);
			ISupportsUpDownBars viewWithUpDownBars = value as ISupportsUpDownBars;
			if (viewWithUpDownBars != null)
				upDownBars.CopyFrom(viewWithUpDownBars.UpDownBars);
		}
		public override void ResetToStyle() {
			base.ResetToStyle();
			DropLinesProperties.ResetToStyle();
			HiLowLinesProperties.ResetToStyle();
			UpDownBars.ResetToStyle();
		}
	}
}
