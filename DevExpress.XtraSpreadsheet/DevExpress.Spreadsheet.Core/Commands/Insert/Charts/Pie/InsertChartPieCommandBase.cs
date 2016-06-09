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
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Commands.Internal;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region InsertChartPieCommandBase (abstract class)
	public abstract class InsertChartPieCommandBase : InsertChartCommandBase {
		protected InsertChartPieCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected virtual int ExplosionPercent { get { return 0; } }
		protected override ChartLayoutModifier Preset { get { return ChartPiePresets.Instance.DefaultModifier; } }
		protected override ChartViewType ViewType { get { return ChartViewType.Pie; } }
		protected internal override IChartView CreateChartView(IChart parent) {
			ChartViewWithVaryColors view = CreateChartViewCore(parent);
			view.VaryColors = true;
			return view;
		}
		protected abstract ChartViewWithVaryColors CreateChartViewCore(IChart parent);
		protected override void CreateAxes(Chart chart) {
		}
		protected internal override SeriesBase CreateSeriesCore(IChartView view) {
			PieSeries series = new PieSeries(view);
			series.Explosion = ExplosionPercent;
			return series;
		}
		protected bool AreSeriesCompatible(SeriesCollection seriesCollection) {
			int count = seriesCollection.Count;
			for (int i = 0; i < count; i++) {
				PieSeries series = seriesCollection[i] as PieSeries;
				if (series == null)
					return false;
				if ((series.Explosion != 0) != (ExplosionPercent != 0))
					return false;
			}
			return true;
		}
	}
	#endregion
}
