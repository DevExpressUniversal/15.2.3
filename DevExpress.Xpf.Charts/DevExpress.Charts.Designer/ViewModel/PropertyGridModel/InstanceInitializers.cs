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
using System.Windows.Media;
using System.Reflection;
using System.Linq;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Charts;
namespace DevExpress.Charts.Designer.Native {
	public class AxisTitleItemInitializer : IInstanceInitializer {
		IEnumerable<TypeInfo> IInstanceInitializer.Types {
			get {
				return new List<TypeInfo>() {
					new TypeInfo(typeof(WpfChartAxisTitlePropertyGridModel), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.NewTitle)),
				};
			}
		}
		object IInstanceInitializer.CreateInstance(TypeInfo type) {
			return new WpfChartAxisTitlePropertyGridModel();
		}		
	}
	public class ConstantLineTitleItemInitializer : IInstanceInitializer {
		IEnumerable<TypeInfo> IInstanceInitializer.Types {
			get {
				return new List<TypeInfo>() {
					new TypeInfo(typeof(WpfChartConstantLineTitlePropertyGridModel), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.NewTitle)),
				};
			}
		}
		object IInstanceInitializer.CreateInstance(TypeInfo type) {
			return new WpfChartConstantLineTitlePropertyGridModel();
		}		
	}
	public class LineStyleItemInitializer : IInstanceInitializer {
		IEnumerable<TypeInfo> IInstanceInitializer.Types {
			get {
				return new List<TypeInfo>() {
					new TypeInfo(typeof(WpfChartLineStylePropertyGridModel), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.NewLineStyle)),
				};
			}
		}
		object IInstanceInitializer.CreateInstance(TypeInfo type) {
			return new WpfChartLineStylePropertyGridModel();
		}		
	}
	public class NavigationOptionsItemInitializer : IInstanceInitializer {
		IEnumerable<TypeInfo> IInstanceInitializer.Types {
			get {
				return new List<TypeInfo>() {
					new TypeInfo(typeof(WpfChartNavigationOptionsPropertyGridModel), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.NewNavigationOptions)),
				};
			}
		}
		object IInstanceInitializer.CreateInstance(TypeInfo type) {
			return new WpfChartNavigationOptionsPropertyGridModel();
		}		
	}
	public class XYNavigationOptionsItemInitializer : IInstanceInitializer {
		IEnumerable<TypeInfo> IInstanceInitializer.Types {
			get {
				return new List<TypeInfo>() {
					new TypeInfo(typeof(WpfChartXYNavigationOptionsPropertyGridModel), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.NewNavigationOptions)),
				};
			}
		}
		object IInstanceInitializer.CreateInstance(TypeInfo type) {
			return new WpfChartXYNavigationOptionsPropertyGridModel();
		}		
	}
	public class ToolTipPositionItemInitializer : IInstanceInitializer {
		IEnumerable<TypeInfo> IInstanceInitializer.Types {
			get {
				return new List<TypeInfo>() {
					new TypeInfo(typeof(WpfChartToolTipFreePositionPropertyGridModel), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.NewToolTipFreePosition)),
					new TypeInfo(typeof(WpfChartToolTipMousePositionPropertyGridModel), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.NewToolTipMousePosition)),
					new TypeInfo(typeof(WpfChartToolTipRelativePositionPropertyGridModel), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.NewToolTipRelativePosition)),
				};
			}
		}
		object IInstanceInitializer.CreateInstance(TypeInfo type) {
			return Activator.CreateInstance(type.Type);
		}		
	}
	public class CrosshairLabelPositionItemInitializer : IInstanceInitializer {
		IEnumerable<TypeInfo> IInstanceInitializer.Types {
			get {
				return new List<TypeInfo>() {
					new TypeInfo(typeof(WpfChartCrosshairFreePositionPropertyGridModel), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.NewCrosshairFreePosition)),
					new TypeInfo(typeof(WpfChartCrosshairMousePositionPropertyGridModel), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.NewCrosshairMousePosition)),
				};
			}
		}
		object IInstanceInitializer.CreateInstance(TypeInfo type) {
			return Activator.CreateInstance(type.Type);
		}		
	}
	public class ToolTipOptionsItemInitializer : IInstanceInitializer {
		IEnumerable<TypeInfo> IInstanceInitializer.Types {
			get {
				return new List<TypeInfo>() {
					new TypeInfo(typeof(WpfChartToolTipOptionsPropertyGridModel), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.NewToolTipOptions)),
				};
			}
		}
		object IInstanceInitializer.CreateInstance(TypeInfo type) {
			return new WpfChartToolTipOptionsPropertyGridModel();
		}		
	}
	public class ChartToolTipControllerItemInitializer : IInstanceInitializer {
		IEnumerable<TypeInfo> IInstanceInitializer.Types {
			get {
				return new List<TypeInfo>() {
					new TypeInfo(typeof(WpfChartToolTipControllerPropertyGridModel), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.NewChartToolTipController)),
				};
			}
		}
		object IInstanceInitializer.CreateInstance(TypeInfo type) {
			return new WpfChartToolTipControllerPropertyGridModel();
		}
	}
	public class CrosshairOptionsItemInitializer : IInstanceInitializer {
		IEnumerable<TypeInfo> IInstanceInitializer.Types {
			get {
				return new List<TypeInfo>() {
					new TypeInfo(typeof(WpfChartCrosshairOptionsPropertyGridModel), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.NewCrosshairOptions)),
				};
			}
		}
		object IInstanceInitializer.CreateInstance(TypeInfo type) {
			return new WpfChartCrosshairOptionsPropertyGridModel();
		}
	}
	public class CrosshairAxisLabelOptionsItemInitializer : IInstanceInitializer {
		IEnumerable<TypeInfo> IInstanceInitializer.Types {
			get {
				return new List<TypeInfo>() {
					new TypeInfo(typeof(WpfChartCrosshairAxisLabelOptionsPropertyGridModel), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.NewCrosshairAxisLabelOptions)),
				};
			}
		}
		object IInstanceInitializer.CreateInstance(TypeInfo type) {
			return new WpfChartCrosshairAxisLabelOptionsPropertyGridModel();
		}
	}
	public class AxisLabelItemInitializer : IInstanceInitializer {
		IEnumerable<TypeInfo> IInstanceInitializer.Types {
			get {
				return new List<TypeInfo>() {
					new TypeInfo(typeof(WpfChartAxisLabelPropertyGridModel), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.NewAxisLabel)),
				};
			}
		}
		object IInstanceInitializer.CreateInstance(TypeInfo type) {
			return new WpfChartAxisLabelPropertyGridModel();
		}
	}
	public class AxisRangeItemInitializer : IInstanceInitializer {
		IEnumerable<TypeInfo> IInstanceInitializer.Types {
			get {
				return new List<TypeInfo>() {
					new TypeInfo(typeof(WpfChartAxisRangePropertyGridModel), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.NewAxisRange)),
				};
			}
		}
		object IInstanceInitializer.CreateInstance(TypeInfo type) {
			return new WpfChartAxisRangePropertyGridModel();
		}
	}
	public class ResolveOverlappingOptionsItemInitializer : IInstanceInitializer {
		IEnumerable<TypeInfo> IInstanceInitializer.Types {
			get {
				return new List<TypeInfo>() {
					new TypeInfo(typeof(WpfChartResolveOverlappingOptionsPropertyGridModel), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.NewResolveOverlappingOptions)),
				};
			}
		}
		object IInstanceInitializer.CreateInstance(TypeInfo type) {
			return new WpfChartResolveOverlappingOptionsPropertyGridModel();
		}
	}
	public class ScrollBarOptionsItemInitializer : IInstanceInitializer {
		IEnumerable<TypeInfo> IInstanceInitializer.Types {
			get {
				return new List<TypeInfo>() {
					new TypeInfo(typeof(WpfChartScrollBarOptionsPropertyGridModel), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.NewScrollBarOptions)),
				};
			}
		}
		object IInstanceInitializer.CreateInstance(TypeInfo type) {
			return new WpfChartScrollBarOptionsPropertyGridModel();
		}
	}
	public class SeriesBorderItemInitializer : IInstanceInitializer {
		IEnumerable<TypeInfo> IInstanceInitializer.Types {
			get {
				return new List<TypeInfo>() {
					new TypeInfo(typeof(WpfChartSeriesBorderPropertyGridModel), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.NewSeriesBorder)),
				};
			}
		}
		object IInstanceInitializer.CreateInstance(TypeInfo type) {
			return new WpfChartSeriesBorderPropertyGridModel();
		}
	}
	public class ReductionStockOptionsItemInitializer : IInstanceInitializer {
		IEnumerable<TypeInfo> IInstanceInitializer.Types {
			get {
				return new List<TypeInfo>() {
					new TypeInfo(typeof(WpfChartReductionStockOptionsPropertyGridModel), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.NewReductionStockOptions)),
				};
			}
		}
		object IInstanceInitializer.CreateInstance(TypeInfo type) {
			return new WpfChartReductionStockOptionsPropertyGridModel();
		}
	}
	public class SeriesLabelItemInitializer : IInstanceInitializer {
		IEnumerable<TypeInfo> IInstanceInitializer.Types {
			get {
				return new List<TypeInfo>() {
					new TypeInfo(typeof(WpfChartSeriesLabelPropertyGridModel), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.NewSeriesLabel)),
				};
			}
		}
		object IInstanceInitializer.CreateInstance(TypeInfo type) {
			return new WpfChartSeriesLabelPropertyGridModel();
		}
	}
	public class DashStyleItemInitializer : IInstanceInitializer {
		IEnumerable<TypeInfo> IInstanceInitializer.Types {
			get {
				return new List<TypeInfo>() {
					new TypeInfo(typeof(DashStyle), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.DashStyle_Dash)),
					new TypeInfo(typeof(DashStyle), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.DashStyle_DashDot)),
					new TypeInfo(typeof(DashStyle), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.DashStyle_DashDotDot)),
					new TypeInfo(typeof(DashStyle), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.DashStyle_Dot)),
					new TypeInfo(typeof(DashStyle), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.DashStyle_Solid)),
				};
			}
		}
		object IInstanceInitializer.CreateInstance(TypeInfo type) {
			if (type.Name == ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.DashStyle_Dash))
				return DashStyles.Dash;
			if (type.Name == ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.DashStyle_DashDot))
				return DashStyles.DashDot;
			if (type.Name == ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.DashStyle_DashDotDot))
				return DashStyles.DashDotDot;
			if (type.Name == ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.DashStyle_Dot))
				return DashStyles.Dot;
			return DashStyles.Solid;
		}
	}
	public class Marker2DModelItemInitializer : IInstanceInitializer {
		IEnumerable<TypeInfo> IInstanceInitializer.Types {
			get {
				return new List<TypeInfo>() {
					new TypeInfo(typeof(CircleMarker2DModel), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Marker2DModelCircle)),
					new TypeInfo(typeof(CrossMarker2DModel), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Marker2DModelCross)),
					new TypeInfo(typeof(DollarMarker2DModel),  ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Marker2DModelDollar)),
					new TypeInfo(typeof(PolygonMarker2DModel),  ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Marker2DModelPolygon)),
					new TypeInfo(typeof(RingMarker2DModel),  ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Marker2DModelRing)),
					new TypeInfo(typeof(SquareMarker2DModel),  ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Marker2DModelSquare)),
					new TypeInfo(typeof(StarMarker2DModel),  ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Marker2DModelStar)),
					new TypeInfo(typeof(TriangleMarker2DModel),  ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Marker2DModelTriangle))
				};
			}
		}
		object IInstanceInitializer.CreateInstance(TypeInfo type) {
			return Activator.CreateInstance(type.Type);
		}
	}
	public class Marker2DAnimationItemInitializer : IInstanceInitializer {
		IEnumerable<TypeInfo> IInstanceInitializer.Types {
			get {
				return new List<TypeInfo>() {
					new TypeInfo(typeof(Marker2DWidenAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Marker2DAnimationWiden)),
					new TypeInfo(typeof(Marker2DSlideFromLeftAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Marker2DAnimationSlideFromLeft)),
					new TypeInfo(typeof(Marker2DSlideFromRightAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Marker2DAnimationSlideFromRight)),
					new TypeInfo(typeof(Marker2DSlideFromTopAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Marker2DAnimationSlideFromTop)),
					new TypeInfo(typeof(Marker2DSlideFromBottomAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Marker2DAnimationSlideFromBottom)),
					new TypeInfo(typeof(Marker2DSlideFromLeftCenterAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Marker2DAnimationSlideFromLeftCenter)),
					new TypeInfo(typeof(Marker2DSlideFromRightCenterAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Marker2DAnimationSlideFromRightCenter)),
					new TypeInfo(typeof(Marker2DSlideFromTopCenterAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Marker2DAnimationSlideFromTopCenter)),
					new TypeInfo(typeof(Marker2DSlideFromBottomCenterAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Marker2DAnimationSlideFromBottomCenter)),
					new TypeInfo(typeof(Marker2DSlideFromLeftTopCornerAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Marker2DAnimationSlideFromLeftTopCorner)),
					new TypeInfo(typeof(Marker2DSlideFromRightTopCornerAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Marker2DAnimationSlideFromRightTopCorner)),
					new TypeInfo(typeof(Marker2DSlideFromRightBottomCornerAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Marker2DAnimationSlideFromRightBottomCorner)),
					new TypeInfo(typeof(Marker2DSlideFromLeftBottomCornerAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Marker2DAnimationSlideFromLeftBottomCorner)),
					new TypeInfo(typeof(Marker2DFadeInAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Marker2DAnimationFadeIn))
				};
			}
		}
		object IInstanceInitializer.CreateInstance(TypeInfo type) {
			return Activator.CreateInstance(type.Type);
		}
	}
	public class Bar2DAnimationItemInitializer : IInstanceInitializer {
		IEnumerable<TypeInfo> IInstanceInitializer.Types {
			get {
				return new List<TypeInfo>() {
					new TypeInfo(typeof(Bar2DGrowUpAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Bar2DGrowUpAnimation)),
					new TypeInfo(typeof(Bar2DDropInAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Bar2DDropInAnimation)),
					new TypeInfo(typeof(Bar2DBounceAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Bar2DBounceAnimation)),
					new TypeInfo(typeof(Bar2DSlideFromLeftAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Bar2DSlideFromLeftAnimation)),
					new TypeInfo(typeof(Bar2DSlideFromRightAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Bar2DSlideFromRightAnimation)),
					new TypeInfo(typeof(Bar2DSlideFromTopAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Bar2DSlideFromTopAnimation)),
					new TypeInfo(typeof(Bar2DSlideFromBottomAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Bar2DSlideFromBottomAnimation)),
					new TypeInfo(typeof(Bar2DWidenAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Bar2DWidenAnimation)),
					new TypeInfo(typeof(Bar2DFadeInAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Bar2DFadeInAnimation)),
				};
			}
		}
		object IInstanceInitializer.CreateInstance(TypeInfo type) {
			return Activator.CreateInstance(type.Type);
		}
	}
	public class Area2DAnimationItemInitializer : IInstanceInitializer {
		IEnumerable<TypeInfo> IInstanceInitializer.Types {
			get {
				return new List<TypeInfo>() {
					new TypeInfo(typeof(Area2DGrowUpAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Area2DGrowUpAnimation)),
					new TypeInfo(typeof(Area2DStretchFromNearAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Area2DStretchFromNearAnimation)),
					new TypeInfo(typeof(Area2DStretchFromFarAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Area2DStretchFromFarAnimation)),
					new TypeInfo(typeof(Area2DStretchOutAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Area2DStretchOutAnimation)),
					new TypeInfo(typeof(Area2DDropFromNearAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Area2DDropFromNearAnimation)),
					new TypeInfo(typeof(Area2DDropFromFarAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Area2DDropFromFarAnimation)),
					new TypeInfo(typeof(Area2DUnwindAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Area2DUnwindAnimation)),
				};
			}
		}
		object IInstanceInitializer.CreateInstance(TypeInfo type) {
			return Activator.CreateInstance(type.Type);
		}
	}
	public class Line2DAnimationItemInitializer : IInstanceInitializer {
		IEnumerable<TypeInfo> IInstanceInitializer.Types {
			get {
				return new List<TypeInfo>() {
					new TypeInfo(typeof(Line2DSlideFromLeftAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Line2DSlideFromLeftAnimation)),
					new TypeInfo(typeof(Line2DSlideFromRightAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Line2DSlideFromRightAnimation)),
					new TypeInfo(typeof(Line2DSlideFromTopAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Line2DSlideFromTopAnimation)),
					new TypeInfo(typeof(Line2DSlideFromBottomAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Line2DSlideFromBottomAnimation)),
					new TypeInfo(typeof(Line2DUnwrapVerticallyAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Line2DUnwrapVerticallyAnimation)),
					new TypeInfo(typeof(Line2DUnwrapHorizontallyAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Line2DUnwrapHorizontallyAnimation)),
					new TypeInfo(typeof(Line2DBlowUpAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Line2DBlowUpAnimation)),
					new TypeInfo(typeof(Line2DStretchFromNearAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Line2DStretchFromNearAnimation)),
					new TypeInfo(typeof(Line2DStretchFromFarAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Line2DStretchFromFarAnimation)),
					new TypeInfo(typeof(Line2DUnwindAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Line2DUnwindAnimation)),
				};
			}
		}
		object IInstanceInitializer.CreateInstance(TypeInfo type) {
			return Activator.CreateInstance(type.Type);
		}
	}
	public class AreaStacked2DAnimationItemInitializer : IInstanceInitializer {
		IEnumerable<TypeInfo> IInstanceInitializer.Types {
			get {
				return new List<TypeInfo>() {
					new TypeInfo(typeof(AreaStacked2DFadeInAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AreaStacked2DFadeInAnimation)),
				};
			}
		}
		object IInstanceInitializer.CreateInstance(TypeInfo type) {
			return Activator.CreateInstance(type.Type);
		}
	}
	public class CircularMarkerAnimationItemInitializer : IInstanceInitializer {
		IEnumerable<TypeInfo> IInstanceInitializer.Types {
			get {
				return new List<TypeInfo>() {
					new TypeInfo(typeof(CircularMarkerWidenAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.CircularMarkerWidenAnimation)),
					new TypeInfo(typeof(CircularMarkerFadeInAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.CircularMarkerFadeInAnimation)),
					new TypeInfo(typeof(CircularMarkerSlideFromLeftCenterAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.CircularMarkerSlideFromLeftCenterAnimation)),
					new TypeInfo(typeof(CircularMarkerSlideFromRightCenterAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.CircularMarkerSlideFromRightCenterAnimation)),
					new TypeInfo(typeof(CircularMarkerSlideFromTopCenterAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.CircularMarkerSlideFromTopCenterAnimation)),
					new TypeInfo(typeof(CircularMarkerSlideFromBottomCenterAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.CircularMarkerSlideFromBottomCenterAnimation)),
					new TypeInfo(typeof(CircularMarkerSlideFromCenterAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.CircularMarkerSlideFromCenterAnimation)),
					new TypeInfo(typeof(CircularMarkerSlideToCenterAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.CircularMarkerSlideToCenterAnimation)),
				};
			}
		}
		object IInstanceInitializer.CreateInstance(TypeInfo type) {
			return Activator.CreateInstance(type.Type);
		}
	}
	public class CircularAreaAnimationItemInitializer : IInstanceInitializer {
		IEnumerable<TypeInfo> IInstanceInitializer.Types {
			get {
				return new List<TypeInfo>() {
					new TypeInfo(typeof(CircularAreaZoomInAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.CircularAreaZoomInAnimation)),
					new TypeInfo(typeof(CircularAreaSpinAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.CircularAreaSpinAnimation)),
					new TypeInfo(typeof(CircularAreaSpinZoomInAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.CircularAreaSpinZoomInAnimation)),
					new TypeInfo(typeof(CircularAreaUnwindAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.CircularAreaUnwindAnimation)),
				};
			}
		}
		object IInstanceInitializer.CreateInstance(TypeInfo type) {
			return Activator.CreateInstance(type.Type);
		}
	}
	public class CircularLineAnimationItemInitializer : IInstanceInitializer {
		IEnumerable<TypeInfo> IInstanceInitializer.Types {
			get {
				return new List<TypeInfo>() {
					new TypeInfo(typeof(CircularLineZoomInAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.CircularLineZoomInAnimation)),
					new TypeInfo(typeof(CircularLineSpinAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.CircularLineSpinAnimation)),
					new TypeInfo(typeof(CircularLineSpinZoomInAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.CircularLineSpinZoomInAnimation)),
					new TypeInfo(typeof(CircularLineUnwindAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.CircularLineUnwindAnimation)),
				};
			}
		}
		object IInstanceInitializer.CreateInstance(TypeInfo type) {
			return Activator.CreateInstance(type.Type);
		}
	}
	public class Stock2DAnimationItemInitializer : IInstanceInitializer {
		IEnumerable<TypeInfo> IInstanceInitializer.Types {
			get {
				return new List<TypeInfo>() {
					new TypeInfo(typeof(Stock2DSlideFromLeftAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Stock2DSlideFromLeftAnimation)),
					new TypeInfo(typeof(Stock2DSlideFromRightAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Stock2DSlideFromRightAnimation)),
					new TypeInfo(typeof(Stock2DSlideFromTopAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Stock2DSlideFromTopAnimation)),
					new TypeInfo(typeof(Stock2DSlideFromBottomAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Stock2DSlideFromBottomAnimation)),
					new TypeInfo(typeof(Stock2DExpandAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Stock2DExpandAnimation)),
					new TypeInfo(typeof(Stock2DFadeInAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Stock2DFadeInAnimation)),
				};
			}
		}
		object IInstanceInitializer.CreateInstance(TypeInfo type) {
			return Activator.CreateInstance(type.Type);
		}
	}
	public class Pie2DSeriesPointAnimationItemInitializer : IInstanceInitializer {
		IEnumerable<TypeInfo> IInstanceInitializer.Types {
			get {
				return new List<TypeInfo>() {
					new TypeInfo(typeof(Pie2DGrowUpAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Pie2DGrowUpAnimation)),
					new TypeInfo(typeof(Pie2DPopUpAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Pie2DPopUpAnimation)),
					new TypeInfo(typeof(Pie2DDropInAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Pie2DDropInAnimation)),
					new TypeInfo(typeof(Pie2DWidenAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Pie2DWidenAnimation)),
					new TypeInfo(typeof(Pie2DFlyInAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Pie2DFlyInAnimation)),
					new TypeInfo(typeof(Pie2DBurstAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Pie2DBurstAnimation)),
					new TypeInfo(typeof(Pie2DFadeInAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Pie2DFadeInAnimation)),
				};
			}
		}
		object IInstanceInitializer.CreateInstance(TypeInfo type) {
			return Activator.CreateInstance(type.Type);
		}
	}
	public class Pie2DSeriesAnimationItemInitializer : IInstanceInitializer {
		IEnumerable<TypeInfo> IInstanceInitializer.Types {
			get {
				return new List<TypeInfo>() {
					new TypeInfo(typeof(Pie2DZoomInAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Pie2DZoomInAnimation)),
					new TypeInfo(typeof(Pie2DFanAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Pie2DFanAnimation)),
					new TypeInfo(typeof(Pie2DFanZoomInAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Pie2DFanZoomInAnimation)),
					new TypeInfo(typeof(Pie2DSpinAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Pie2DSpinAnimation)),
					new TypeInfo(typeof(Pie2DSpinZoomInAnimation), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Pie2DSpinZoomInAnimation)),
				};
			}
		}
		object IInstanceInitializer.CreateInstance(TypeInfo type) {
			return Activator.CreateInstance(type.Type);
		}
	}
	public class NumericScaleOptionsItemInitializer : IInstanceInitializer {
		IEnumerable<TypeInfo> IInstanceInitializer.Types {
			get {
				return new List<TypeInfo>() {
					new TypeInfo(typeof(WpfChartAutomaticNumericScaleOptionsPropertyGridModel), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ScaleOptionsAutomatic)),
					new TypeInfo(typeof(WpfChartManualNumericScaleOptionsPropertyGridModel),  ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ScaleOptionsManual)),
					new TypeInfo(typeof(WpfChartContinuousNumericScaleOptionsPropertyGridModel),  ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ScaleOptionsContinuous)),
				};
			}
		}
		object IInstanceInitializer.CreateInstance(TypeInfo type) {
			return Activator.CreateInstance(type.Type);
		}
	}
	public class DateTimeScaleOptionsItemInitializer : IInstanceInitializer {
		IEnumerable<TypeInfo> IInstanceInitializer.Types {
			get {
				return new List<TypeInfo>() {
					new TypeInfo(typeof(WpfChartAutomaticDateTimeScaleOptionsPropertyGridModel),  ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ScaleOptionsAutomatic)),
					new TypeInfo(typeof(WpfChartManualDateTimeScaleOptionsPropertyGridModel),  ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ScaleOptionsManual)),
					new TypeInfo(typeof(WpfChartContinuousDateTimeScaleOptionsPropertyGridModel),  ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ScaleOptionsContinuous)),
				};
			}
		}
		object IInstanceInitializer.CreateInstance(TypeInfo type) {
			return Activator.CreateInstance(type.Type);
		}
	}
	public class ContinuousNumericScaleOptionsItemInitializer : IInstanceInitializer {
		IEnumerable<TypeInfo> IInstanceInitializer.Types {
			get {
				return new List<TypeInfo>() {
					new TypeInfo(typeof(WpfChartContinuousNumericScaleOptionsPropertyGridModel),  ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ScaleOptionsContinuous)),
				};
			}
		}
		object IInstanceInitializer.CreateInstance(TypeInfo type) {
			return new WpfChartContinuousNumericScaleOptionsPropertyGridModel();
		}
	}
	public class ContinuousDateTimeScaleOptionsItemInitializer : IInstanceInitializer {
		IEnumerable<TypeInfo> IInstanceInitializer.Types {
			get {
				return new List<TypeInfo>() {
					new TypeInfo(typeof(WpfChartContinuousDateTimeScaleOptionsPropertyGridModel),  ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ScaleOptionsContinuous)),
				};
			}
		}
		object IInstanceInitializer.CreateInstance(TypeInfo type) {
			return new WpfChartContinuousDateTimeScaleOptionsPropertyGridModel();
		}
	}
	public class ChartColorizerItemInitializer : IInstanceInitializer {
		IEnumerable<TypeInfo> IInstanceInitializer.Types {
			get {
				return new List<TypeInfo>() {
					new TypeInfo(typeof(ColorObjectColorizerPropertyGridModel), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ColorObjectColorizer)),
					new TypeInfo(typeof(KeyColorColorizerPropertyGridModel), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.KeyColorColorizer)),
					new TypeInfo(typeof(RangeColorizerPropertyGridModel), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.RangeColorizer)),
				};
			}
		}
		object IInstanceInitializer.CreateInstance(TypeInfo type) {
			return Activator.CreateInstance(type.Type);
		}
	}
	public class ColorizerKeyItemInitializer : IInstanceInitializer {
		IEnumerable<TypeInfo> IInstanceInitializer.Types {
			get {
				return new List<TypeInfo>() {
					new TypeInfo(typeof(string), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ColorizerKeyString)), 
					new TypeInfo(typeof(int), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ColorizerKeyInt)),
					new TypeInfo(typeof(double), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ColorizerKeyDouble)),
				};
			}
		}
		object IInstanceInitializer.CreateInstance(TypeInfo type) {
			if (type.Type == typeof(string))
				return "New String";
			if (type.Type == typeof(int))
				return 0;
			if (type.Type == typeof(double))
				return 0.0;
			return null;
		}
	}
	public class ColorizerPaletteItemInitializer : IInstanceInitializer {
		List<TypeInfo> types;
		public ColorizerPaletteItemInitializer() {
			types = new List<TypeInfo>();
			IEnumerable<Type> paletteTypes = Assembly.GetAssembly(typeof(ChartControl)).GetTypes().Where(x => (x.IsSubclassOf(typeof(PredefinedPalette)) && !x.IsAbstract));
			foreach (Type type in paletteTypes) {
				PredefinedPalette palette = (PredefinedPalette)Activator.CreateInstance(type);
				types.Add(new TypeInfo(type, palette.PaletteName));
			}
		}
		IEnumerable<TypeInfo> IInstanceInitializer.Types { get { return types; } }
		object IInstanceInitializer.CreateInstance(TypeInfo type) {
			return Activator.CreateInstance(type.Type);
		}
	}
}
