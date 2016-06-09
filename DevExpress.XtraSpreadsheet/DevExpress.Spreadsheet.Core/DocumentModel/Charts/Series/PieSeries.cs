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
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	public class PieSeries : SeriesWithDataLabelsAndPoints {
		public static PieSeries Create(IChartView view, bool exploded) {
			PieSeries result = new PieSeries(view);
			if (exploded)
				result.SetDefaultExplosion();
			return result;
		}
		#region Fields
		const int DefaultExplosion = 25;
		int explosion;
		#endregion
		public PieSeries(IChartView view)
			: base(view) {
		}
		#region Properties
		public int Explosion {
			get { return explosion; }
			set {
				Guard.ArgumentNonNegative(value, "Explosion");
				SetExplosion(value);
			}
		}
		void SetExplosion(int value) {
			PieSeriesExplosionPropertyChangedHistoryItem historyItem = new PieSeriesExplosionPropertyChangedHistoryItem(DocumentModel, this, explosion, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetExplosionCore(int value) {
			this.explosion = value;
			Parent.Invalidate();
		}
		#endregion
		#region ISeries
		public override ChartSeriesType SeriesType { get { return ChartSeriesType.Pie; } }
		public override ChartType ChartType { get { return View.ChartType; } }
		public override ISeries CloneTo(IChartView view) {
			PieSeries result = new PieSeries(view);
			result.CopyFrom(this);
			return result;
		}
		public override bool IsCompatible(IChartView view) {
			if(view == null)
				return false;
			return view.ViewType == ChartViewType.Doughnut || IsCompatibleCore(view);
		}
		bool IsCompatibleCore(IChartView view) {
			ChartViewType viewType = view.ViewType;
			return
				viewType == ChartViewType.OfPie ||
				viewType == ChartViewType.Pie ||
				viewType == ChartViewType.Pie3D;
		}
		public override void Visit(ISeriesVisitor visitor) {
			visitor.Visit(this);
		}
		#endregion
		public void SetDefaultExplosion() {
			Explosion = DefaultExplosion;
		}
		public override void CopyFrom(ISeries value) {
			base.CopyFrom(value);
			PieSeries series = value as PieSeries;
			if (series != null)
				CopyFromCore(series);
		}
		void CopyFromCore(PieSeries value) {
			Explosion = value.Explosion;
		}
		protected override bool CanRemoveOnResetToStyle(DataPoint dataPoint) {
			return !dataPoint.HasExplosion || (dataPoint.Explosion == Explosion);
		}
		protected override bool IsCompatibleLabelPosition(DataLabelPosition position) {
			if (position == DataLabelPosition.Default)
				return true;
			if (View.ViewType == ChartViewType.Doughnut)
				return position == DataLabelPosition.Center;
			if (IsCompatibleCore(View))
				return
					position == DataLabelPosition.BestFit ||
					position == DataLabelPosition.Center ||
					position == DataLabelPosition.InsideEnd ||				
					position == DataLabelPosition.OutsideEnd;
			return false;
		}
	}
}
