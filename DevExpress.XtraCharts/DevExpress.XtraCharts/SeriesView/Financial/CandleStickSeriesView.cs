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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(CandleStickSeriesViewTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]	
	public class CandleStickSeriesView : FinancialSeriesViewBase {
		protected internal override string StringId { get { return ChartLocalizer.GetString(ChartStringId.SvnCandleStick); } }
		public CandleStickSeriesView() : base() {
		}
		void RenderPoint(IRenderer renderer, Color color, RectangleF stockLowRect, RectangleF stockHighRect, RectangleF[] bodyRects) {
			renderer.FillRectangle(stockLowRect, color);
			renderer.FillRectangle(stockHighRect, color);
			foreach (RectangleF rect in bodyRects)
				renderer.FillRectangle(rect, color);
		}
		protected override DrawOptions CreateSeriesDrawOptionsInternal() {
			return new CandleStickDrawOptions(this);
		}
		protected internal override PointOptions CreatePointOptions() {
			return new StockPointOptions();
		}
		protected override ChartElement CreateObjectForClone() {
			return new CandleStickSeriesView();
		}
		protected override FinancialSeriesPointLayout CreateSeriesPointLayout(RefinedPointData pointData, int lineThickness) {
			return new CandleStickSeriesPointLayout(pointData, lineThickness);
		}
		protected override void RenderFinancial(IRenderer renderer, FinancialSeriesPointLayout financialPointLayout, Color color) {
			CandleStickSeriesPointLayout candleLayout = (CandleStickSeriesPointLayout)financialPointLayout;
			RenderPoint(renderer, color, candleLayout.StockLowRect, candleLayout.StockHighRect, candleLayout.SplitBodyRect());
			if (financialPointLayout.PointData != null) {
				SelectionState pointSelectionState = financialPointLayout.PointData.SelectionState;
				if (pointSelectionState != SelectionState.Normal) {
					SeriesHitTestState state = Series.HitState;
					renderer.FillRectangle(candleLayout.StockLowRect, state.HatchStyle, state.GetHatchColorLight(pointSelectionState));
					renderer.FillRectangle(candleLayout.StockHighRect, state.HatchStyle, state.GetHatchColorLight(pointSelectionState));
					RectangleF[] rects = candleLayout.SplitBodyRect();
					foreach (RectangleF rect in rects)
						renderer.FillRectangle(rect, state.HatchStyle, state.GetHatchColorLight(pointSelectionState));
				}
			}
		}
		protected internal override GraphicsCommand CreateShadowGraphicsCommand(Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
			return null;
		}
		protected internal override void RenderShadow(IRenderer renderer, Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
			if (pointLayout == null)
				return;
			CandleStickSeriesPointLayout candleLayout = (CandleStickSeriesPointLayout)pointLayout;
			FinancialDrawOptions financialDrawOptions = (FinancialDrawOptions)drawOptions;
			int shadowSize = financialDrawOptions.Shadow.GetActualSize(-1);
			financialDrawOptions.Shadow.Render(renderer, candleLayout.StockLowRect, shadowSize);
			financialDrawOptions.Shadow.Render(renderer, candleLayout.StockHighRect, shadowSize);
			RectangleF[] rects = candleLayout.SplitBodyRect();
			foreach (RectangleF rect in rects)
				financialDrawOptions.Shadow.Render(renderer, rect, shadowSize);
		}
		protected internal override HighlightedPointLayout CalculateHighlightedPointLayout(XYDiagramMappingBase diagramMapping, RefinedPoint refinedPoint, ISeriesView seriesView, DrawOptions drawOptions) {
			IFinancialPoint pointInfo = refinedPoint;
			FinancialDrawOptions financialDrawOptions = drawOptions as FinancialDrawOptions;
			if (pointInfo == null || financialDrawOptions == null)
				return null;
			CandleStickSeriesPointLayout pointLayout = new CandleStickSeriesPointLayout(null, financialDrawOptions.LineThickness);
			pointLayout.Calculate(diagramMapping, pointInfo.Argument,
				pointInfo.Low, pointInfo.High, pointInfo.Open, pointInfo.Close, financialDrawOptions);
			return new HighlightedCandleStickPointLayout(drawOptions.Color, pointLayout.StockLowRect, pointLayout.StockHighRect, pointLayout.SplitBodyRect());
		}
		protected internal override void RenderHighlightedPoint(IRenderer renderer, HighlightedPointLayout pointLayout) {
			HighlightedCandleStickPointLayout candlePointLayout = pointLayout as HighlightedCandleStickPointLayout;
			if (candlePointLayout != null) {
				RectangleF stockLowRect = new RectangleF(candlePointLayout.StockLowRect.X - 1, candlePointLayout.StockLowRect.Y - 1,
					candlePointLayout.StockLowRect.Width + 2, candlePointLayout.StockLowRect.Height + 2);
				RectangleF stockHighRect = new RectangleF(candlePointLayout.StockHighRect.X - 1, candlePointLayout.StockHighRect.Y - 1,
					candlePointLayout.StockHighRect.Width + 2, candlePointLayout.StockHighRect.Height + 2);
				RectangleF[] rects = candlePointLayout.BodyRects;
				RectangleF[] bodyRects = new RectangleF[rects.Length];
				for (int i = 0; i < rects.Length; i++)
					bodyRects[i] = new RectangleF(rects[i].X - 1, rects[i].Y - 1, rects[i].Width + 2, rects[i].Height + 2);
				RenderPoint(renderer, candlePointLayout.Color, stockLowRect, stockHighRect, bodyRects);
			}
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public class CandleStickSeriesPointLayout : FinancialSeriesPointLayout {
		RectangleF bodyRect;
		RectangleF stockLowRect;
		RectangleF stockHighRect;
		bool filled;
		public RectangleF StockLowRect { get { return stockLowRect; } }
		public RectangleF StockHighRect { get { return stockHighRect; } }
		public CandleStickSeriesPointLayout(RefinedPointData pointData, int lineThickness) : base(pointData, lineThickness) {
		}
		RectangleF CalcBodyRect(MatrixTransform matrixTransform) {
			DiagramPoint point1 = matrixTransform.Apply(new DiagramPoint(Low.X - NearCorrection - LevelLineLengthOpen, Open.Y));
			DiagramPoint point2 = matrixTransform.Apply(new DiagramPoint(Low.X + FarCorrection + LevelLineLengthClose, Close.Y));
			return MathUtils.MakeRectangle((PointF)point1, (PointF)point2, 1.0f);
		}
		RectangleF CalcStockLowRect(MatrixTransform matrixTransform) {
			int y = Math.Min(Open.Y, Close.Y);
			DiagramPoint point1 = matrixTransform.Apply(new DiagramPoint(Low.X - NearCorrection, Low.Y));
			DiagramPoint point2 = matrixTransform.Apply(new DiagramPoint(Low.X + FarCorrection, y));
			return MathUtils.MakeRectangle((PointF)point1, (PointF)point2, 1.0f);
		}
		RectangleF CalcStockHighRect(MatrixTransform matrixTransform) {
			int y = Math.Max(Open.Y, Close.Y);
			DiagramPoint point1 = matrixTransform.Apply(new DiagramPoint(High.X - NearCorrection, High.Y));
			DiagramPoint point2 = matrixTransform.Apply(new DiagramPoint(High.X + FarCorrection, y));
			return MathUtils.MakeRectangle((PointF)point1, (PointF)point2, 1.0f);
		}
		protected override void CalculateInternal(MatrixTransform matrixTransform) {
			this.bodyRect = CalcBodyRect(matrixTransform);
			this.stockLowRect = CalcStockLowRect(matrixTransform);
			this.stockHighRect = CalcStockHighRect(matrixTransform);
			this.filled = Open.Y > Close.Y;
		}
		internal RectangleF[] SplitBodyRect() {
			if(filled || LineThickness * 2 >= bodyRect.Height || LineThickness * 2 >= bodyRect.Width)
				return new RectangleF[] { bodyRect };
			float bottomCoord =  bodyRect.Location.Y + LineThickness;
			RectangleF top = new RectangleF(bodyRect.Location.X, bodyRect.Location.Y, bodyRect.Width, LineThickness);
			RectangleF left = new RectangleF(bodyRect.Location.X, bottomCoord, LineThickness, bodyRect.Height - 2 * LineThickness);
			RectangleF bottom = new RectangleF(bodyRect.Location.X, bottomCoord + bodyRect.Height - 2 * LineThickness, bodyRect.Width, LineThickness);
			RectangleF right = new RectangleF(bodyRect.Location.X + bodyRect.Width - LineThickness, bottomCoord, LineThickness, left.Height);
			return new RectangleF[] { top, left, bottom, right };
		}
		public override HitRegionContainer CalculateHitRegion() {
			HitRegionContainer hitRegion = base.CalculateHitRegion();
			hitRegion.Union(new HitRegion(GraphicUtils.InflateRect(bodyRect, 1, 1)));
			hitRegion.Union(new HitRegion(GraphicUtils.InflateRect(stockLowRect, 1, 1)));
			hitRegion.Union(new HitRegion(GraphicUtils.InflateRect(stockHighRect, 1, 1)));
			return hitRegion;
		}
	}
}
