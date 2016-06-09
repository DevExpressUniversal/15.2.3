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
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Media3D;
using DevExpress.Xpf.Core.Design;
using DevExpress.Xpf.Utils;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.PropertyEditing;
namespace DevExpress.Xpf.Charts.Design {
	internal class RegisterHelper {
		static void RegisterDiagramTypes(AttributeTableBuilder builder, params Type[] diagramTypes) {
			builder.AddCustomAttributes(typeof(ChartControl), ChartControl.DiagramProperty.GetName(), new NewItemTypesAttribute(diagramTypes));
		}
		static void RegisterSeriesAndSeriesTemplateTypes(AttributeTableBuilder builder, Type diagramType, params Type[] seriesTypes) {
			builder.AddCustomAttributes(diagramType, Diagram.SeriesProperty.GetName(), new NewItemTypesAttribute(seriesTypes));
			builder.AddCustomAttributes(diagramType, Diagram.SeriesTemplateProperty.GetName(), new NewItemTypesAttribute(seriesTypes));
		}
		static void AddTransform3D(AttributeTableBuilder builder, Type ownerType, DependencyProperty property) {
			builder.AddCustomAttributes(ownerType, property.Name,
				new NewItemTypesAttribute(new Type[] { typeof(RotateTransform3D), typeof(ScaleTransform3D),
					typeof(TranslateTransform3D), typeof(MatrixTransform3D), typeof(Transform3DGroup)}));
		}
		static void AddMemberAttributes(AttributeTableBuilder attributeTableBuilderBuilder, Type type, string memberName, params Attribute[] attribs) {
			attributeTableBuilderBuilder.AddCallback(type, builder => builder.AddCustomAttributes(memberName, attribs));
		}
		public static readonly Type[] DiagramTypes = new Type[] {
				typeof(XYDiagram2D), 
				typeof(SimpleDiagram2D),
				typeof(RadarDiagram2D),
				typeof(PolarDiagram2D),
				typeof(XYDiagram3D),
				typeof(SimpleDiagram3D)
		};
		public static readonly Type[] XYDiagram2DSeriesTypes = new Type[]{
				typeof(BarSideBySideSeries2D),
				typeof(BarStackedSeries2D),
				typeof(BarFullStackedSeries2D),
				typeof(BarSideBySideStackedSeries2D),
				typeof(BarSideBySideFullStackedSeries2D), 
				typeof(PointSeries2D),
				typeof(LineSeries2D), 
				typeof(LineStackedSeries2D), 
				typeof(LineFullStackedSeries2D),
				typeof(LineStepSeries2D),
				typeof(LineScatterSeries2D),
				typeof(SplineSeries2D),
				typeof(SplineAreaSeries2D),
				typeof(SplineAreaStackedSeries2D),
				typeof(SplineAreaFullStackedSeries2D),
				typeof(AreaSeries2D),
				typeof(AreaStackedSeries2D),
				typeof(AreaFullStackedSeries2D),
				typeof(AreaStepSeries2D), 
				typeof(BubbleSeries2D), 
				typeof(RangeAreaSeries2D),
				typeof(RangeBarOverlappedSeries2D),
				typeof(RangeBarSideBySideSeries2D),
				typeof(StockSeries2D), 
				typeof(CandleStickSeries2D)
		};
		public static readonly Type[] RadarDiagram2DSeriesTypes = new Type[]{
				typeof(RadarPointSeries2D),
				typeof(RadarLineSeries2D),
				typeof(RadarLineScatterSeries2D),
				typeof(RadarAreaSeries2D)
		};
		public static readonly Type[] PolarDiagram2DSeriesTypes = new Type[]{
				typeof(PolarPointSeries2D),
				typeof(PolarLineSeries2D),
				typeof(PolarLineScatterSeries2D), 
				typeof(PolarAreaSeries2D)
		};
		public static readonly Type[] XYDiagram3DSeriesTypes = new Type[]{
				typeof(BarSeries3D),
				typeof(BarSideBySideSeries3D),
				typeof(PointSeries3D),
				typeof(BubbleSeries3D),
				typeof(AreaSeries3D),
				typeof(AreaStackedSeries3D),
				typeof(AreaFullStackedSeries3D)
		};
		public static void PrepareAttributeTable(AttributeTableBuilder builder) {
			RegisterDiagramTypes(builder, DiagramTypes  );
			RegisterSeriesAndSeriesTemplateTypes(builder, 
				typeof(XYDiagram2D), 
				XYDiagram2DSeriesTypes
			);
			RegisterSeriesAndSeriesTemplateTypes(builder,
				typeof(SimpleDiagram2D), 
				typeof(PieSeries2D),
				typeof(FunnelSeries2D),
				typeof(NestedDonutSeries2D)
			);
			RegisterSeriesAndSeriesTemplateTypes(builder, 
				typeof(RadarDiagram2D),
				RadarDiagram2DSeriesTypes
			);
			RegisterSeriesAndSeriesTemplateTypes(builder, 
				typeof(PolarDiagram2D),
				PolarDiagram2DSeriesTypes
			);
			builder.AddCustomAttributes(typeof(ChartControl), ChartControl.PaletteProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(ChameleonPalette),
					typeof(InAFogPalette),
					typeof(NatureColorsPalette),
					typeof(NorthernLightsPalette),
					typeof(PastelKitPalette), 
					typeof(TerracottaPiePalette),
					typeof(TheTreesPalette),
					typeof(OfficePalette), 
					typeof(DXChartsPalette), 
					typeof(CustomPalette),
					typeof(Office2013Palette),
					typeof(BlueWarmPalette),
					typeof(BluePalette),
					typeof(BlueIIPalette),
					typeof(BlueGreenPalette),
					typeof(GreenPalette),
					typeof(GreenYellowPalette),
					typeof(YellowPalette),
					typeof(YellowOrangePalette),
					typeof(OrangePalette),
					typeof(OrangeRedPalette),
					typeof(RedOrangePalette),
					typeof(RedPalette),
					typeof(RedVioletPalette),
					typeof(VioletPalette),
					typeof(VioletIIPalette),
					typeof(MarqueePalette),
					typeof(SlipstreamPalette)
				)
			);
			builder.AddCustomAttributes(typeof(XYSeries2D), XYSeries2D.IndicatorsProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(TrendLine),
					typeof(RegressionLine),
					typeof(FibonacciArcs),
					typeof(FibonacciFans),
					typeof(FibonacciRetracement),
					typeof(SimpleMovingAverage),
					typeof(WeightedMovingAverage),
					typeof(TriangularMovingAverage),
					typeof(ExponentialMovingAverage)
				)
			);
			builder.AddCustomAttributes(typeof(BarSeries2D), BarSeries2D.ModelProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(OutsetBar2DModel),
					typeof(GradientBar2DModel),
					typeof(BorderlessGradientBar2DModel),
					typeof(SimpleBar2DModel),
					typeof(BorderlessSimpleBar2DModel),
					typeof(FlatBar2DModel),
					typeof(FlatGlassBar2DModel),
					typeof(SteelColumnBar2DModel),
					typeof(TransparentBar2DModel),
					typeof(Quasi3DBar2DModel),
					typeof(GlassCylinderBar2DModel),
					typeof(CustomBar2DModel)
				)
			);
			builder.AddCustomAttributes(typeof(RangeBarSeries2D), RangeBarSeries2D.ModelProperty.GetName(),
				new NewItemTypesAttribute(
				   typeof(OutsetRangeBar2DModel),
				   typeof(GradientRangeBar2DModel),
				   typeof(BorderlessGradientRangeBar2DModel),
				   typeof(SimpleRangeBar2DModel),
				   typeof(BorderlessSimpleRangeBar2DModel),
				   typeof(FlatRangeBar2DModel),
				   typeof(FlatGlassRangeBar2DModel),
				   typeof(TransparentRangeBar2DModel),
				   typeof(CustomRangeBar2DModel)
				)
			);
			builder.AddCustomAttributes(typeof(AreaSeries2D), AreaSeries2D.MarkerModelProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(CircleMarker2DModel),
					typeof(CrossMarker2DModel), 
					typeof(DollarMarker2DModel),
					typeof(PolygonMarker2DModel),
					typeof(RingMarker2DModel), 
					typeof(SquareMarker2DModel),
					typeof(StarMarker2DModel), 
					typeof(TriangleMarker2DModel), 
					typeof(CustomMarker2DModel)
				)
			);
			builder.AddCustomAttributes(typeof(RangeAreaSeries2D), RangeAreaSeries2D.Marker1ModelProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(CircleMarker2DModel),
					typeof(CrossMarker2DModel), 
					typeof(DollarMarker2DModel),
					typeof(PolygonMarker2DModel),
					typeof(RingMarker2DModel),
					typeof(SquareMarker2DModel),
					typeof(StarMarker2DModel), 
					typeof(TriangleMarker2DModel), 
					typeof(CustomMarker2DModel)
				)
			);
			builder.AddCustomAttributes(typeof(RangeAreaSeries2D), RangeAreaSeries2D.Marker2ModelProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(CircleMarker2DModel),
					typeof(CrossMarker2DModel), 
					typeof(DollarMarker2DModel),
					typeof(PolygonMarker2DModel), 
					typeof(RingMarker2DModel), 
					typeof(SquareMarker2DModel),
					typeof(StarMarker2DModel), 
					typeof(TriangleMarker2DModel),
					typeof(CustomMarker2DModel)
				)
			);
			builder.AddCustomAttributes(typeof(RangeBarSeries2D), RangeBarSeries2D.MinMarkerModelProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(CircleMarker2DModel),
					typeof(CrossMarker2DModel),
					typeof(DollarMarker2DModel),
					typeof(PolygonMarker2DModel),
					typeof(RingMarker2DModel),
					typeof(SquareMarker2DModel),
					typeof(StarMarker2DModel),
					typeof(TriangleMarker2DModel),
					typeof(CustomMarker2DModel)
				)
			);
			builder.AddCustomAttributes(typeof(RangeBarSeries2D), RangeBarSeries2D.MaxMarkerModelProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(CircleMarker2DModel), 
					typeof(CrossMarker2DModel), 
					typeof(DollarMarker2DModel),
					typeof(PolygonMarker2DModel),
					typeof(RingMarker2DModel), 
					typeof(SquareMarker2DModel),
					typeof(StarMarker2DModel),
					typeof(TriangleMarker2DModel),
					typeof(CustomMarker2DModel)
				)
			);
			builder.AddCustomAttributes(typeof(LineSeries2D), LineSeries2D.MarkerModelProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(CircleMarker2DModel), 
					typeof(CrossMarker2DModel), 
					typeof(DollarMarker2DModel),
					typeof(PolygonMarker2DModel),
					typeof(RingMarker2DModel), 
					typeof(SquareMarker2DModel),
					typeof(StarMarker2DModel), 
					typeof(TriangleMarker2DModel), 
					typeof(CustomMarker2DModel)
				)
			);
			builder.AddCustomAttributes(typeof(PointSeries2D), PointSeries2D.MarkerModelProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(CircleMarker2DModel),
					typeof(CrossMarker2DModel),
					typeof(DollarMarker2DModel),
					typeof(PolygonMarker2DModel),
					typeof(RingMarker2DModel),
					typeof(SquareMarker2DModel),
					typeof(StarMarker2DModel), 
					typeof(TriangleMarker2DModel),
					typeof(CustomMarker2DModel)
				)
			);
			builder.AddCustomAttributes(typeof(CircularAreaSeries2D), CircularAreaSeries2D.MarkerModelProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(CircleMarker2DModel),
					typeof(CrossMarker2DModel), 
					typeof(DollarMarker2DModel),
					typeof(PolygonMarker2DModel),
					typeof(RingMarker2DModel),
					typeof(SquareMarker2DModel),
					typeof(StarMarker2DModel), 
					typeof(TriangleMarker2DModel),
					typeof(CustomMarker2DModel)
				)
			);
			builder.AddCustomAttributes(typeof(CircularLineSeries2D), CircularLineSeries2D.MarkerModelProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(CircleMarker2DModel), 
					typeof(CrossMarker2DModel), 
					typeof(DollarMarker2DModel),
					typeof(PolygonMarker2DModel),
					typeof(RingMarker2DModel), 
					typeof(SquareMarker2DModel),
					typeof(StarMarker2DModel), 
					typeof(TriangleMarker2DModel),
					typeof(CustomMarker2DModel)
				)
			);
			builder.AddCustomAttributes(typeof(BubbleSeries2D), BubbleSeries2D.MarkerModelProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(CircleMarker2DModel),
					typeof(CrossMarker2DModel),
					typeof(DollarMarker2DModel),
					typeof(PolygonMarker2DModel),
					typeof(RingMarker2DModel),
					typeof(SquareMarker2DModel),
					typeof(StarMarker2DModel), 
					typeof(TriangleMarker2DModel), 
					typeof(CustomMarker2DModel)
				)
			);
			builder.AddCustomAttributes(typeof(StockSeries2D), StockSeries2D.ModelProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(FlatStock2DModel),
					typeof(ThinStock2DModel),
					typeof(DropsStock2DModel),
					typeof(ArrowsStock2DModel), 
					typeof(CustomStock2DModel)
				)
			);
			builder.AddCustomAttributes(typeof(CandleStickSeries2D), CandleStickSeries2D.ModelProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(BorderCandleStick2DModel), 
					typeof(FlatCandleStick2DModel),
					typeof(ThinCandleStick2DModel),
					typeof(GlassCandleStick2DModel),
					typeof(SimpleCandleStick2DModel),
					typeof(CustomCandleStick2DModel)
				)
			);
			builder.AddCustomAttributes(typeof(PieSeries2D), PieSeries2D.ModelProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(SimplePie2DModel), 
					typeof(FlatPie2DModel),
					typeof(BorderlessFlatPie2DModel),
					typeof(GlarePie2DModel),
					typeof(GlassPie2DModel),
					typeof(GlossyPie2DModel),
					typeof(CupidPie2DModel),
					typeof(CustomPie2DModel)
				)
			);
			builder.AddCustomAttributes(typeof(BarSeries2DBase), BarSeries2DBase.PointAnimationProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(Bar2DGrowUpAnimation),
					typeof(Bar2DBounceAnimation),
					typeof(Bar2DSlideFromRightAnimation),
					typeof(Bar2DSlideFromLeftAnimation),
					typeof(Bar2DSlideFromBottomAnimation),
					typeof(Bar2DSlideFromTopAnimation),
					typeof(Bar2DWidenAnimation), 
					typeof(Bar2DDropInAnimation), 
					typeof(Bar2DFadeInAnimation)
				)
			);
			builder.AddCustomAttributes(typeof(PointSeries2D), PointSeries2D.PointAnimationProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(Marker2DWidenAnimation),
					typeof(Marker2DSlideFromTopAnimation), 
					typeof(Marker2DSlideFromBottomAnimation),
					typeof(Marker2DSlideFromLeftAnimation), 
					typeof(Marker2DSlideFromRightAnimation), 
					typeof(Marker2DSlideFromTopCenterAnimation),
					typeof(Marker2DSlideFromBottomCenterAnimation),
					typeof(Marker2DSlideFromLeftCenterAnimation),
					typeof(Marker2DSlideFromRightCenterAnimation),
					typeof(Marker2DSlideFromLeftTopCornerAnimation),
					typeof(Marker2DSlideFromRightTopCornerAnimation),
					typeof(Marker2DSlideFromRightBottomCornerAnimation),
					typeof(Marker2DSlideFromLeftBottomCornerAnimation), 
					typeof(Marker2DFadeInAnimation)
				)
			);
			builder.AddCustomAttributes(typeof(BubbleSeries2D), BubbleSeries2D.PointAnimationProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(Marker2DWidenAnimation), 
					typeof(Marker2DSlideFromTopAnimation),
					typeof(Marker2DSlideFromBottomAnimation),
					typeof(Marker2DSlideFromLeftAnimation),
					typeof(Marker2DSlideFromRightAnimation),
					typeof(Marker2DSlideFromTopCenterAnimation),
					typeof(Marker2DSlideFromBottomCenterAnimation), 
					typeof(Marker2DSlideFromLeftCenterAnimation), 
					typeof(Marker2DSlideFromRightCenterAnimation),
					typeof(Marker2DSlideFromLeftTopCornerAnimation),
					typeof(Marker2DSlideFromRightTopCornerAnimation), 
					typeof(Marker2DSlideFromRightBottomCornerAnimation),
					typeof(Marker2DSlideFromLeftBottomCornerAnimation), 
					typeof(Marker2DFadeInAnimation)
				)
			);
			builder.AddCustomAttributes(typeof(LineSeries2D), LineSeries2D.PointAnimationProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(Marker2DWidenAnimation),
					typeof(Marker2DSlideFromTopAnimation),
					typeof(Marker2DSlideFromBottomAnimation),
					typeof(Marker2DSlideFromLeftAnimation),
					typeof(Marker2DSlideFromRightAnimation),
					typeof(Marker2DSlideFromTopCenterAnimation),
					typeof(Marker2DSlideFromBottomCenterAnimation), 
					typeof(Marker2DSlideFromLeftCenterAnimation), 
					typeof(Marker2DSlideFromRightCenterAnimation),
					typeof(Marker2DSlideFromLeftTopCornerAnimation),
					typeof(Marker2DSlideFromRightTopCornerAnimation), 
					typeof(Marker2DSlideFromRightBottomCornerAnimation),
					typeof(Marker2DSlideFromLeftBottomCornerAnimation), 
					typeof(Marker2DFadeInAnimation)
				)
			);
			builder.AddCustomAttributes(typeof(LineSeries2D), LineSeries2D.SeriesAnimationProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(Line2DSlideFromLeftAnimation),
					typeof(Line2DSlideFromRightAnimation), 
					typeof(Line2DSlideFromTopAnimation),
					typeof(Line2DSlideFromBottomAnimation),
					typeof(Line2DUnwrapVerticallyAnimation),
					typeof(Line2DUnwrapHorizontallyAnimation),
					typeof(Line2DBlowUpAnimation), 
					typeof(Line2DStretchFromNearAnimation),
					typeof(Line2DStretchFromFarAnimation), 
					typeof(Line2DUnwindAnimation)
				)
			);
			builder.AddCustomAttributes(typeof(AreaSeries2D), AreaSeries2D.PointAnimationProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(Marker2DWidenAnimation), 
					typeof(Marker2DSlideFromTopAnimation),
					typeof(Marker2DSlideFromBottomAnimation),
					typeof(Marker2DSlideFromLeftAnimation),
					typeof(Marker2DSlideFromRightAnimation), 
					typeof(Marker2DSlideFromTopCenterAnimation),
					typeof(Marker2DSlideFromBottomCenterAnimation), 
					typeof(Marker2DSlideFromLeftCenterAnimation),
					typeof(Marker2DSlideFromRightCenterAnimation),
					typeof(Marker2DSlideFromLeftTopCornerAnimation),
					typeof(Marker2DSlideFromRightTopCornerAnimation),
					typeof(Marker2DSlideFromRightBottomCornerAnimation),
					typeof(Marker2DSlideFromLeftBottomCornerAnimation), 
					typeof(Marker2DFadeInAnimation)
				)
			);
			builder.AddCustomAttributes(typeof(AreaSeries2D), AreaSeries2D.SeriesAnimationProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(Area2DGrowUpAnimation),
					typeof(Area2DStretchFromNearAnimation),
					typeof(Area2DStretchFromFarAnimation),
					typeof(Area2DStretchOutAnimation),
					typeof(Area2DDropFromNearAnimation), 
					typeof(Area2DDropFromFarAnimation),
					typeof(Area2DUnwindAnimation)
				)
			);
			builder.AddCustomAttributes(typeof(RangeAreaSeries2D), RangeAreaSeries2D.PointAnimationProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(Marker2DWidenAnimation),
					typeof(Marker2DSlideFromTopAnimation),
					typeof(Marker2DSlideFromBottomAnimation),
					typeof(Marker2DSlideFromLeftAnimation),
					typeof(Marker2DSlideFromRightAnimation),
					typeof(Marker2DSlideFromTopCenterAnimation),
					typeof(Marker2DSlideFromBottomCenterAnimation),
					typeof(Marker2DSlideFromLeftCenterAnimation),
					typeof(Marker2DSlideFromRightCenterAnimation),
					typeof(Marker2DSlideFromLeftTopCornerAnimation),
					typeof(Marker2DSlideFromRightTopCornerAnimation),
					typeof(Marker2DSlideFromRightBottomCornerAnimation),
					typeof(Marker2DSlideFromLeftBottomCornerAnimation),
					typeof(Marker2DFadeInAnimation)
				)
			);
			builder.AddCustomAttributes(typeof(RangeAreaSeries2D), RangeAreaSeries2D.SeriesAnimationProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(Area2DGrowUpAnimation),
					typeof(Area2DStretchFromNearAnimation),
					typeof(Area2DStretchFromFarAnimation),
					typeof(Area2DStretchOutAnimation),
					typeof(Area2DDropFromNearAnimation), 
					typeof(Area2DDropFromFarAnimation),
					typeof(Area2DUnwindAnimation)
				)
			);
			builder.AddCustomAttributes(typeof(AreaStackedSeries2D), AreaStackedSeries2D.SeriesAnimationProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(Area2DGrowUpAnimation),
					typeof(Area2DStretchFromNearAnimation),
					typeof(Area2DStretchFromFarAnimation),
					typeof(Area2DStretchOutAnimation), 
					typeof(Area2DDropFromNearAnimation),
					typeof(Area2DDropFromFarAnimation),
					typeof(Area2DUnwindAnimation)
				)
			);
			builder.AddCustomAttributes(typeof(AreaStackedSeries2D), AreaStackedSeries2D.PointAnimationProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(AreaStacked2DFadeInAnimation)
				)
			);
			builder.AddCustomAttributes(typeof(FinancialSeries2D), FinancialSeries2D.PointAnimationProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(Stock2DSlideFromLeftAnimation),
					typeof(Stock2DSlideFromRightAnimation),
					typeof(Stock2DSlideFromTopAnimation),
					typeof(Stock2DSlideFromBottomAnimation),
					typeof(Stock2DExpandAnimation), 
					typeof(Stock2DFadeInAnimation)
				)
			);
			builder.AddCustomAttributes(typeof(CircularSeries2D), CircularSeries2D.PointAnimationProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(CircularMarkerWidenAnimation),
					typeof(CircularMarkerFadeInAnimation), 
					typeof(CircularMarkerSlideFromLeftCenterAnimation),
					typeof(CircularMarkerSlideFromRightCenterAnimation),
					typeof(CircularMarkerSlideFromTopCenterAnimation), 
					typeof(CircularMarkerSlideFromBottomCenterAnimation),
					typeof(CircularMarkerSlideFromCenterAnimation),
					typeof(CircularMarkerSlideToCenterAnimation)
				)
			);
			builder.AddCustomAttributes(typeof(CircularLineSeries2D), CircularLineSeries2D.SeriesAnimationProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(CircularLineZoomInAnimation),
					typeof(CircularLineSpinAnimation),
					typeof(CircularLineSpinZoomInAnimation), 
					typeof(CircularLineUnwindAnimation)
				)
			);
			builder.AddCustomAttributes(typeof(CircularAreaSeries2D), CircularAreaSeries2D.SeriesAnimationProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(CircularAreaZoomInAnimation),
					typeof(CircularAreaSpinAnimation),
					typeof(CircularAreaSpinZoomInAnimation), 
					typeof(CircularAreaUnwindAnimation)
				)
			);
			builder.AddCustomAttributes(typeof(PieSeries2D), PieSeries2D.PointAnimationProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(Pie2DGrowUpAnimation), 
					typeof(Pie2DPopUpAnimation), 
					typeof(Pie2DDropInAnimation),
					typeof(Pie2DFlyInAnimation),
					typeof(Pie2DWidenAnimation), 
					typeof(Pie2DBurstAnimation),
					typeof(Pie2DFadeInAnimation)
				)
			);
			builder.AddCustomAttributes(typeof(PieSeries2D), PieSeries2D.SeriesAnimationProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(Pie2DFanAnimation), 
					typeof(Pie2DFanZoomInAnimation),
					typeof(Pie2DSpinAnimation),
					typeof(Pie2DSpinZoomInAnimation), 
					typeof(Pie2DZoomInAnimation)
				)
			);
			builder.AddCustomAttributes(typeof(FunnelSeries2D), FunnelSeries2D.PointAnimationProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(Funnel2DWidenAnimation),
					typeof(Funnel2DGrowUpAnimation),
					typeof(Funnel2DSlideFromLeftAnimation),
					typeof(Funnel2DSlideFromRightAnimation),
					typeof(Funnel2DSlideFromTopAnimation),
					typeof(Funnel2DSlideFromBottomAnimation),
					typeof(Funnel2DFadeInAnimation),
					typeof(Funnel2DFlyInAnimation)
				)
			);
			AddMemberAttributes(builder, typeof(Pane), "GridLines", new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden));
			AddMemberAttributes(builder, typeof(Pane), "InterlaceControls", new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden));
			AddMemberAttributes(builder, typeof(Pane), "PaneItems", new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden));
			AddMemberAttributes(builder, typeof(Pane), "SeriesLabelItems", new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden));
			AddMemberAttributes(builder, typeof(Pane), "Pseudo3DPointPresentations", new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden));
			AddMemberAttributes(builder, typeof(XYDiagram2D), "Items", new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden));
			AddMemberAttributes(builder, typeof(TitleBase), "Content", new TypeConverterAttribute(typeof(StringConverter)));
			AddMemberAttributes(builder, typeof(CustomAxisLabel), "Content", new TypeConverterAttribute(typeof(StringConverter)));
			builder.RegisterAttachedPropertiesForChildren(typeof(AxisY2D), true, "GetAlwaysShowZeroLevel");
#pragma warning disable 0612, 0618
			builder.RegisterAttachedPropertiesForType(typeof(AxisY2D), typeof(AxisRange), "GetAlwaysShowZeroLevel");
			builder.RegisterAttachedPropertiesForType(typeof(CircularAxisY2D), typeof(AxisRange), "GetAlwaysShowZeroLevel");
#pragma warning restore 0612, 0618
			builder.RegisterAttachedPropertiesForType(typeof(AxisY2D), typeof(Range), "GetAlwaysShowZeroLevel");
			builder.RegisterAttachedPropertiesForType(typeof(CircularAxisY2D), typeof(Range), "GetAlwaysShowZeroLevel");
			builder.RegisterAttachedPropertiesForChildren(typeof(CircularAxisY2D), true, "GetAlwaysShowZeroLevel");			
			builder.RegisterAttachedPropertiesForChildren(typeof(XYDiagram2D), true, "GetSeriesPane", "GetSeriesAxisX", "GetSeriesAxisY");
			builder.RegisterAttachedPropertiesForChildren(typeof(RangeAreaSeries2D), true, "GetValue2", "GetDateTimeValue2", "GetLabelKind", "GetMinValueAngle", "GetMaxValueAngle");
			builder.RegisterAttachedPropertiesForType(typeof(RangeAreaSeries2D), typeof(SeriesPoint), "GetValue2", "GetDateTimeValue2");
			builder.RegisterAttachedPropertiesForType(typeof(RangeAreaSeries2D), typeof(SeriesLabel), "GetLabelKind", "GetMinValueAngle", "GetMaxValueAngle");
			builder.RegisterAttachedPropertiesForChildren(typeof(BarSideBySideSeries2D), true, "GetLabelPosition");
			builder.RegisterAttachedPropertiesForType(typeof(BarSideBySideSeries2D), typeof(SeriesLabel), "GetLabelPosition");
			builder.RegisterAttachedPropertiesForChildren(typeof(RangeBarSeries2D), true, "GetValue2", "GetDateTimeValue2", "GetLabelKind");
			builder.RegisterAttachedPropertiesForType(typeof(RangeBarSeries2D), typeof(SeriesPoint), "GetValue2", "GetDateTimeValue2");
			builder.RegisterAttachedPropertiesForType(typeof(RangeBarSeries2D), typeof(SeriesLabel), "GetLabelKind");
			builder.RegisterAttachedPropertiesForChildren(typeof(FinancialSeries2D), true, "GetLowValue", "GetHighValue", "GetOpenValue", "GetCloseValue", "GetValueToDisplay");
			builder.RegisterAttachedPropertiesForType(typeof(FinancialSeries2D), typeof(SeriesPoint), "GetLowValue", "GetHighValue", "GetOpenValue", "GetCloseValue");
			builder.RegisterAttachedPropertiesForChildren(typeof(LineFullStackedSeries2D), true, "GetPercentOptions");
			builder.RegisterAttachedPropertiesForChildren(typeof(PieSeries), true, "GetExplodedDistance", "GetLabelPosition", "GetPercentOptions");
			builder.RegisterAttachedPropertiesForType(typeof(PieSeries), typeof(SeriesPoint), "GetExplodedDistance");
			builder.RegisterAttachedPropertiesForType(typeof(PieSeries), typeof(SeriesLabel), "GetLabelPosition");
			builder.RegisterAttachedPropertiesForType(typeof(FunnelSeries2D), typeof(SeriesLabel), "GetLabelPosition");
			builder.RegisterAttachedPropertiesForChildren(typeof(BubbleSeries2D), true, "GetWeight", "GetValueToDisplay", "GetLabelPosition");
			builder.RegisterAttachedPropertiesForType(typeof(BubbleSeries2D), typeof(SeriesPoint), "GetWeight");
			builder.RegisterAttachedPropertiesForType(typeof(BubbleSeries2D), typeof(SeriesLabel), "GetLabelPosition");
			builder.RegisterAttachedPropertiesForChildren(typeof(MarkerSeries2D), true, "GetAngle");
			builder.RegisterAttachedPropertiesForType(typeof(MarkerSeries2D), typeof(SeriesLabel), "GetAngle");
			builder.RegisterAttachedPropertiesForChildren(typeof(CircularSeries2D), true, "GetAngle");
			builder.RegisterAttachedPropertiesForType(typeof(CircularSeries2D), typeof(SeriesLabel), "GetAngle");
			RegisterSeriesAndSeriesTemplateTypes(builder, typeof(XYDiagram3D),
														  XYDiagram3DSeriesTypes);
			RegisterSeriesAndSeriesTemplateTypes(builder, typeof(SimpleDiagram3D), typeof(PieSeries3D));
			AddTransform3D(builder, typeof(Diagram3D), XYDiagram3D.ContentTransformProperty);
			builder.AddCustomAttributes(typeof(Diagram), new FeatureAttribute(typeof(PerformRotationProvider)));
			builder.AddCustomAttributes(typeof(XYDiagram3D), XYDiagram3D.MaterialProperty.Name,
				new NewItemTypesAttribute(
					typeof(DiffuseMaterial), 
					typeof(EmissiveMaterial),
					typeof(SpecularMaterial), 
					typeof(MaterialGroup)
				)
			);
			builder.AddCustomAttributes(typeof(RotateTransform3D), RotateTransform3D.RotationProperty.Name,
				new NewItemTypesAttribute(
					typeof(AxisAngleRotation3D),
					typeof(QuaternionRotation3D)
				)
			);
			builder.AddCustomAttributes(typeof(BarSeries3D), BarSeries3D.ModelProperty.Name,
				new NewItemTypesAttribute(
					typeof(BoxBar3DModel),
					typeof(ConeBar3DModel),
					typeof(CylinderBar3DModel),
					typeof(HexagonBar3DModel), 
					typeof(PyramidBar3DModel),
					typeof(CustomBar3DModel)
				)
			);
			builder.AddCustomAttributes(typeof(PieSeries3D), PieSeries3D.ModelProperty.Name,
				new NewItemTypesAttribute(
					typeof(CirclePie3DModel),
					typeof(RectanglePie3DModel),
					typeof(PentagonPie3DModel),
					typeof(HexagonPie3DModel), 
					typeof(RoundedRectanglePie3DModel),
					typeof(SemiCirclePie3DModel),
					typeof(SemiRectanglePie3DModel), 
					typeof(SemiPentagonPie3DModel),
					typeof(SemiHexagonPie3DModel),
					typeof(SemiRoundedRectanglePie3DModel),
					typeof(CustomPie3DModel)
				)
			);
			builder.AddCustomAttributes(typeof(PointSeries3D), PointSeries3D.ModelProperty.Name,
				new NewItemTypesAttribute(
					typeof(CapsuleMarker3DModel),
					typeof(ConeMarker3DModel),
					typeof(CubeMarker3DModel),
					typeof(CylinderMarker3DModel), 
					typeof(HexagonMarker3DModel),
					typeof(PyramidMarker3DModel),
					typeof(RoundedCubeMarker3DModel),
					typeof(SphereMarker3DModel), 
					typeof(StarMarker3DModel)
				)
			);
			builder.AddCustomAttributes(typeof(BubbleSeries3D), BubbleSeries3D.ModelProperty.Name,
				new NewItemTypesAttribute(
					typeof(CapsuleMarker3DModel),
					typeof(ConeMarker3DModel),
					typeof(CubeMarker3DModel),
					typeof(CylinderMarker3DModel),
					typeof(HexagonMarker3DModel), 
					typeof(PyramidMarker3DModel),
					typeof(RoundedCubeMarker3DModel), 
					typeof(SphereMarker3DModel), 
					typeof(StarMarker3DModel)
				)
			);
			builder.RegisterAttachedPropertiesForChildren(typeof(MarkerSeries3D), true, "GetLabelPosition", "GetTransform");
			builder.RegisterAttachedPropertiesForType(typeof(MarkerSeries3D), typeof(SeriesLabel), "GetLabelPosition");
			builder.RegisterAttachedPropertiesForType(typeof(MarkerSeries3D), typeof(SeriesPoint), "GetTransform");
			builder.RegisterAttachedPropertiesForChildren(typeof(AxisY3D), true, "GetAlwaysShowZeroLevel");
#pragma warning disable 0612, 0618
			builder.RegisterAttachedPropertiesForType(typeof(AxisY3D), typeof(AxisRange), "GetAlwaysShowZeroLevel");
#pragma warning restore 0612, 0618
			builder.RegisterAttachedPropertiesForChildren(typeof(AreaFullStackedSeries3D), true, "GetPercentOptions");
			builder.RegisterAttachedPropertiesForChildren(typeof(BarSeries3D), true, "GetTransform");
			builder.RegisterAttachedPropertiesForType(typeof(BarSeries3D), typeof(SeriesPoint), "GetTransform");
			builder.RegisterAttachedPropertiesForChildren(typeof(BubbleSeries3D), true, "GetWeight", "GetValueToDisplay");
			builder.RegisterAttachedPropertiesForType(typeof(BubbleSeries3D), typeof(SeriesPoint), "GetWeight");
			builder.HideProperties(typeof(ChartControl), "QueryCursor");
			builder.AddCustomAttributes(typeof(ChartControl), new FeatureAttribute(typeof(ChartControlInitializer)));
		}
	}
	public class ChartControlInitializer : DefaultInitializer {
		public override void InitializeDefaults(ModelItem item) {
			ModelItem diagramItem = ModelFactory.CreateItem(item.Context, typeof(XYDiagram2D));
			ModelItem seriesItem = ModelFactory.CreateItem(item.Context, typeof(BarSideBySideSeries2D));
			seriesItem.Properties[Series.DisplayNameProperty.Name].SetValue("Series 1");
			diagramItem.Properties["Series"].Collection.Add(seriesItem);
			item.Properties[ChartControl.DiagramProperty.Name].SetValue(diagramItem);
			ModelItem legendItem = ModelFactory.CreateItem(item.Context, typeof(Legend));
			item.Properties[ChartControl.LegendProperty.Name].SetValue(legendItem);
			DevExpress.Xpf.Design.InitializerHelper.Initialize(item);
		}
	}
}
