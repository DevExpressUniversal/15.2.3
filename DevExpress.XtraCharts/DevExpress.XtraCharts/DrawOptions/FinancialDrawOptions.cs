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

using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	public abstract class FinancialDrawOptions : DrawOptions {
		int lineThickness;
		double levelLineLength;
		ReductionStockOptions reductionOptions;
		Shadow shadow;
#if !SL
	[DevExpressXtraChartsLocalizedDescription("FinancialDrawOptionsLineThickness")]
#endif
		public int LineThickness { get { return this.lineThickness; } set { this.lineThickness = value; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("FinancialDrawOptionsLevelLineLength")]
#endif
		public double LevelLineLength { get { return this.levelLineLength; } set { this.levelLineLength = value; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("FinancialDrawOptionsReductionOptions")]
#endif
		public ReductionStockOptions ReductionOptions { get { return this.reductionOptions; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("FinancialDrawOptionsShadow")]
#endif
		public Shadow Shadow { get { return this.shadow; } }
		protected FinancialDrawOptions(FinancialSeriesViewBase view) : base(view) {
			this.lineThickness = view.LineThickness;
			this.levelLineLength = view.LevelLineLength;
			this.reductionOptions = (ReductionStockOptions)view.ReductionOptions.Clone();
			this.shadow = (Shadow)view.Shadow.Clone();
		}
		protected FinancialDrawOptions() {
		}
		protected override void DeepClone(object obj) {
			base.DeepClone(obj);
			FinancialDrawOptions drawOptions = obj as FinancialDrawOptions;
			if(drawOptions != null) {
				this.lineThickness = drawOptions.lineThickness;
				this.levelLineLength = drawOptions.levelLineLength;
				this.reductionOptions = (ReductionStockOptions)drawOptions.reductionOptions.Clone();
				this.shadow = (Shadow)drawOptions.shadow.Clone();
			}
		}
		protected internal override void InitializeSeriesPointDrawOptions(SeriesViewBase view, IRefinedSeries seriesInfo, int pointIndex) {
			base.InitializeSeriesPointDrawOptions(view, seriesInfo, pointIndex);
			if (ReductionOptions.Visible && pointIndex != 0) {
				ValueLevel valueLevel;
				switch (ReductionOptions.Level) {
					case StockLevel.Low:
						valueLevel = ValueLevel.Low;
						break;
					case StockLevel.High:
						valueLevel = ValueLevel.High;
						break;
					case StockLevel.Open:
						valueLevel = ValueLevel.Open;
						break;
					case StockLevel.Close:
						valueLevel = ValueLevel.Close;
						break;
					default:
						ChartDebug.Fail("Unknown stock level.");
						return;
				}
				RefinedPoint refinedPoint = seriesInfo.Points[pointIndex];
				if (refinedPoint != null && !refinedPoint.IsEmpty)
					while (--pointIndex >= 0) {
						RefinedPoint previousPointInfo = seriesInfo.Points[pointIndex];
						if (previousPointInfo != null && !previousPointInfo.IsEmpty) {
							if (refinedPoint.GetValue((ValueLevelInternal)valueLevel) < previousPointInfo.GetValue((ValueLevelInternal)valueLevel))
								Color = ReductionOptions.Color;
							return;
						}
					}
			}
		}		
	}
}
