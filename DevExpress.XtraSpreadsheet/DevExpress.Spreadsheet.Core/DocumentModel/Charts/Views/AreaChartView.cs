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
namespace DevExpress.XtraSpreadsheet.Model {
	#region AreaChartView
	public class AreaChartView : ChartViewWithGroupingAndDropLines {
		public AreaChartView(IChart parent)
			: base(parent) {
		}
		#region IChartView Members
		public override ChartViewType ViewType { get { return ChartViewType.Area; } }
		public override ChartType ChartType {
			get {
				if (Grouping == ChartGrouping.Standard)
					return ChartType.Area;
				if (Grouping == ChartGrouping.Stacked)
					return ChartType.AreaStacked;
				return ChartType.AreaFullStacked;
			}
		}
		public override AxisGroupType AxesType { get { return AxisGroupType.CategoryValue; } }
		public override IChartView CloneTo(IChart parent) {
			AreaChartView result = new AreaChartView(parent);
			result.CopyFrom(this);
			return result;
		}
		protected internal override IChartView Duplicate() {
			AreaChartView result = new AreaChartView(Parent);
			result.CopyFromWithoutSeries(this);
			return result;
		}
		public override void Visit(IChartViewVisitor visitor) {
			visitor.Visit(this);
		}
		public override ISeries CreateSeriesInstance() {
			return new AreaSeries(this);
		}
		#endregion
	}
	#endregion
	#region Area3DChartView
	public class Area3DChartView : ChartViewWithGroupingAndDropLines, IChartViewWithGapDepth {
		#region Fields
		int gapDepth = 150;
		#endregion
		public Area3DChartView(IChart parent)
			: base(parent) {
		}
		#region Properties
		#region GapDepth
		public int GapDepth {
			get { return gapDepth; }
			set {
				ValueChecker.CheckValue(value, 0, 500, "GapDepth");
				if (gapDepth == value)
					return;
				SetGapDepth(value);
			}
		}
		void SetGapDepth(int value) {
			ChartViewGapDepthPropertyChangedHistoryItem historyItem = new ChartViewGapDepthPropertyChangedHistoryItem(DocumentModel, this, gapDepth, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		public void SetGapDepthCore(int value) {
			this.gapDepth = value;
			Parent.Invalidate();
		}
		#endregion
		#endregion
		#region IChartView Members
		public override ChartViewType ViewType { get { return ChartViewType.Area3D; } }
		public override ChartType ChartType {
			get {
				if (Grouping == ChartGrouping.Standard)
					return ChartType.Area3D;
				if (Grouping == ChartGrouping.Stacked)
					return ChartType.Area3DStacked;
				return ChartType.Area3DFullStacked;
			}
		}
		public override bool Is3DView { get { return true; } }
		public override AxisGroupType AxesType { get { return Grouping == ChartGrouping.Standard ? AxisGroupType.CategoryValueSeries : AxisGroupType.CategoryValue; } }
		public override IChartView CloneTo(IChart parent) {
			Area3DChartView result = new Area3DChartView(parent);
			result.CopyFrom(this);
			return result;
		}
		public override void Visit(IChartViewVisitor visitor) {
			visitor.Visit(this);
		}
		public override ISeries CreateSeriesInstance() {
			return new AreaSeries(this);
		}
		#endregion
		protected override void CopyFrom(IChartView value, bool copySeries) {
			base.CopyFrom(value, copySeries);
			IChartViewWithGapDepth view = value as IChartViewWithGapDepth;
			if (view != null)
				CopyFromCore(view);
		}
		void CopyFromCore(IChartViewWithGapDepth value) {
			GapDepth = value.GapDepth;
		}
	}
	#endregion
}
