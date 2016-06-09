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
	public class DoughnutChartView : ChartViewWithSlice {
		#region Fields
		int holeSize = 10;
		#endregion
		public DoughnutChartView(IChart parent)
			: base(parent) {
		}
		#region Properties
		#region HoleSize
		public int HoleSize {
			get { return holeSize; }
			set {
				ValueChecker.CheckValue(value, 10, 90, "HoleSize");
				if(holeSize == value)
					return;
				SetHoleSize(value);
			}
		}
		void SetHoleSize(int value) {
			DoughnutChartHoleSizePropertyChangedHistoryItem historyItem = new DoughnutChartHoleSizePropertyChangedHistoryItem(DocumentModel, this, holeSize, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetHoleSizeCore(int value) {
			this.holeSize = value;
			Parent.Invalidate();
		}
		#endregion
		#endregion
		#region IChartView Members
		public override ChartViewType ViewType { get { return ChartViewType.Doughnut; } }
		public override ChartType ChartType { get { return PieChartView.HasExplodedFirstSeries(Series) ? ChartType.DoughnutExploded : ChartType.Doughnut; } }
		public override AxisGroupType AxesType { get { return AxisGroupType.Empty; } }
		public override IChartView CloneTo(IChart parent) {
			DoughnutChartView result = new DoughnutChartView(parent);
			result.CopyFrom(this);
			return result;
		}
		public override void Visit(IChartViewVisitor visitor) {
			visitor.Visit(this);
		}
		public override ISeries CreateSeriesInstance() {
			return PieSeries.Create(this, Exploded);
		}
		#endregion
		protected override void CopyFrom(IChartView value, bool copySeries) {
			base.CopyFrom(value, copySeries);
			DoughnutChartView view = value as DoughnutChartView;
			if (view != null)
				CopyFromCore(view);
		}
		void CopyFromCore(DoughnutChartView value) {
			HoleSize = value.HoleSize;
		}
	}
}
