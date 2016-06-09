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

using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using DevExpress.Charts.Native;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile),
	]
	public enum StockType {
		Both,
		Open,
		Close
	}
	[
	TypeConverter(typeof(StockSeriesViewTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]	
	public class StockSeriesView : FinancialSeriesViewBase {
		const StockType DefaultStockType = StockType.Both;
		StockType stockType = DefaultStockType;
		protected internal override string StringId { get { return ChartLocalizer.GetString(ChartStringId.SvnStock); } }
		protected internal override string DefaultPointToolTipPattern {
			get {
				bool ShowOpen = ShowOpenClose == StockType.Both || ShowOpenClose == StockType.Open;
				bool ShowClose = ShowOpenClose == StockType.Both || ShowOpenClose == StockType.Close;
				string argumentPattern = "{A" + GetDefaultArgumentFormat() + "}";
				string valuePattern = "\nHigh: {HV" + GetDefaultFormat(Series.ValueScaleType) + "}\nLow: {LV" + GetDefaultFormat(Series.ValueScaleType) + "}" +
					(ShowOpen ? "\nOpen: {OV" + GetDefaultFormat(Series.ValueScaleType) + "}" : "") + (ShowClose ? "\nClose: {CV" + GetDefaultFormat(Series.ValueScaleType) + "}" : "");
				return argumentPattern + valuePattern;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("StockSeriesViewShowOpenClose"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.StockSeriesView.ShowOpenClose"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public StockType ShowOpenClose { 
			get { return this.stockType; } 
			set {
				SendNotification(new ElementWillChangeNotification(this));
				this.stockType = value;
				RaiseControlChanged();
			}
		}
		public StockSeriesView() : base() {
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if(propertyName == "ShowOpenClose")
				return ShouldSerializeShowOpenClose();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeShowOpenClose() {
			return ShowOpenClose != DefaultStockType;
		}
		void ResetShowOpenClose() {
			ShowOpenClose = DefaultStockType;
		}
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				ShouldSerializeShowOpenClose();
		}
		#endregion
		void RenderPoint(IRenderer renderer, Color color, RectangleF stockRect, RectangleF openRect, RectangleF closeRect) {
			renderer.FillRectangle(stockRect, color);
			renderer.FillRectangle(openRect, color);
			renderer.FillRectangle(closeRect, color);
		}
		protected override DrawOptions CreateSeriesDrawOptionsInternal() {
			return new StockDrawOptions(this);
		}
		protected override ChartElement CreateObjectForClone() {
			return new StockSeriesView();
		}
		protected override FinancialSeriesPointLayout CreateSeriesPointLayout(RefinedPointData pointData, int lineThickness) {
			return new StockSeriesPointLayout(pointData, stockType, lineThickness);		
		}
		protected override void RenderFinancial(IRenderer renderer, FinancialSeriesPointLayout financialPointLayout, Color color) {
			StockSeriesPointLayout stockLayout = (StockSeriesPointLayout)financialPointLayout;
			RenderPoint(renderer, color, stockLayout.StockRect, stockLayout.OpenRect, stockLayout.CloseRect);
			if (financialPointLayout.PointData != null) {
				SelectionState pointSelectionState = financialPointLayout.PointData.SelectionState;
				if (pointSelectionState != SelectionState.Normal) {
					SeriesHitTestState state = Series.HitState;
					renderer.FillRectangle(stockLayout.StockRect, state.HatchStyle, state.GetHatchColorLight(pointSelectionState), color);
					renderer.FillRectangle(stockLayout.OpenRect, state.HatchStyle, state.GetHatchColorLight(pointSelectionState), color);
					renderer.FillRectangle(stockLayout.CloseRect, state.HatchStyle, state.GetHatchColorLight(pointSelectionState), color);
				}
			}
		}
		protected internal override GraphicsCommand CreateShadowGraphicsCommand(Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
			return null;
		}
		protected internal override void RenderShadow(IRenderer renderer, Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
			if (pointLayout == null)
				return;
			StockSeriesPointLayout stockLayout = (StockSeriesPointLayout)pointLayout;
			FinancialDrawOptions financialDrawOptions = (FinancialDrawOptions)drawOptions;
			int shadowSize = financialDrawOptions.Shadow.GetActualSize(-1);
			financialDrawOptions.Shadow.Render(renderer, stockLayout.StockRect, shadowSize);
			financialDrawOptions.Shadow.Render(renderer, stockLayout.OpenRect, shadowSize);
			financialDrawOptions.Shadow.Render(renderer, stockLayout.CloseRect, shadowSize);
		}
		protected internal override void RenderHighlightedPoint(IRenderer renderer, HighlightedPointLayout pointLayout) {
			HighlightedStockPointLayout stockPointLayout = pointLayout as HighlightedStockPointLayout;
			if (stockPointLayout != null) {
				RectangleF stockRect = new RectangleF(stockPointLayout.StockRect.X - 1, stockPointLayout.StockRect.Y - 1,
					stockPointLayout.StockRect.Width + 2, stockPointLayout.StockRect.Height + 2);
				RectangleF openRect = new RectangleF(stockPointLayout.OpenRect.X - 1, stockPointLayout.OpenRect.Y - 1,
					stockPointLayout.OpenRect.Width + 2, stockPointLayout.OpenRect.Height + 2);
				RectangleF closeRect = new RectangleF(stockPointLayout.CloseRect.X - 1, stockPointLayout.CloseRect.Y - 1,
					stockPointLayout.CloseRect.Width + 2, stockPointLayout.CloseRect.Height + 2);
				RenderPoint(renderer, stockPointLayout.Color, stockRect, openRect, closeRect);
			}
		}
		protected internal override PointOptions CreatePointOptions() {
			return new StockPointOptions();
		}
		protected internal override HighlightedPointLayout CalculateHighlightedPointLayout(XYDiagramMappingBase diagramMapping, RefinedPoint refinedPoint, ISeriesView seriesView, DrawOptions drawOptions) {
			IFinancialPoint pointInfo = refinedPoint;
			FinancialDrawOptions financialDrawOptions = drawOptions as FinancialDrawOptions;
			if (pointInfo == null || financialDrawOptions == null)
				return null;
			StockSeriesPointLayout pointLayout = new StockSeriesPointLayout(null, stockType, financialDrawOptions.LineThickness);
			pointLayout.Calculate(diagramMapping, pointInfo.Argument,
				pointInfo.Low, pointInfo.High, pointInfo.Open, pointInfo.Close, financialDrawOptions);
			return new HighlightedStockPointLayout(drawOptions.Color, pointLayout.StockRect, pointLayout.OpenRect, pointLayout.CloseRect);
		}
		protected internal override ToolTipPointDataToStringConverter CreateToolTipValueToStringConverter() {
			bool showOpen = ShowOpenClose == StockType.Both || ShowOpenClose == StockType.Open;
			bool showClose = ShowOpenClose == StockType.Both || ShowOpenClose == StockType.Close;
			return new ToolTipFinancialValueToStringConverter(Series, showOpen, showClose);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			StockSeriesView view = (StockSeriesView)obj;
			return view.stockType == this.stockType;
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			StockSeriesView view = obj as StockSeriesView;
			if (view == null)
				return;
			this.stockType = view.stockType;
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public class StockSeriesPointLayout : FinancialSeriesPointLayout {
		StockType stockType;
		RectangleF stockRect;
		RectangleF openRect;
		RectangleF closeRect;
		public RectangleF StockRect { get { return stockRect; } }
		public RectangleF OpenRect { get { return openRect; } }
		public RectangleF CloseRect { get { return closeRect; } }
		public StockSeriesPointLayout(RefinedPointData pointData, StockType stockType, int lineThickness) : base(pointData, lineThickness) {
			this.stockType = stockType;
		}
		RectangleF CalcStock(MatrixTransform matrixTransform) {
			DiagramPoint point1 = matrixTransform.Apply(new DiagramPoint(Low.X - NearCorrection, High.Y));
			DiagramPoint point2 = matrixTransform.Apply(new DiagramPoint(Low.X + FarCorrection, Low.Y));
			return MathUtils.MakeRectangle((PointF)point1, (PointF)point2, 1.0f);
		}
		RectangleF CalcOpen(MatrixTransform matrixTransform) {
			if(stockType == StockType.Close)
				return RectangleF.Empty;
			DiagramPoint point1 = matrixTransform.Apply(new DiagramPoint(Low.X - NearCorrection - LevelLineLengthOpen, Open.Y + NearCorrection));
			DiagramPoint point2 = matrixTransform.Apply(new DiagramPoint(Low.X + FarCorrection, Open.Y - FarCorrection));
			return MathUtils.MakeRectangle((PointF)point1, (PointF)point2, 1.0f);
		}
		RectangleF CalcClose(MatrixTransform matrixTransform) {
			if(stockType == StockType.Open)
				return RectangleF.Empty;
			DiagramPoint point1 = matrixTransform.Apply(new DiagramPoint(Low.X - NearCorrection, Close.Y + NearCorrection));
			DiagramPoint point2 = matrixTransform.Apply(new DiagramPoint(Low.X + FarCorrection + LevelLineLengthClose, Close.Y - FarCorrection));
			return MathUtils.MakeRectangle((PointF)point1, (PointF)point2, 1.0f);
		}
		protected override void CalculateInternal(MatrixTransform matrixTransform) {
			this.stockRect = CalcStock(matrixTransform);
			this.openRect = CalcOpen(matrixTransform);
			this.closeRect = CalcClose(matrixTransform);
		}
		public override HitRegionContainer CalculateHitRegion() {
			HitRegionContainer hitRegion = base.CalculateHitRegion();
			hitRegion.Union(new HitRegion(GraphicUtils.InflateRect(stockRect, 1, 1)));
			hitRegion.Union(new HitRegion(GraphicUtils.InflateRect(openRect, 1, 1)));
			hitRegion.Union(new HitRegion(GraphicUtils.InflateRect(closeRect, 1, 1)));
			return hitRegion;
		}
	}
}
