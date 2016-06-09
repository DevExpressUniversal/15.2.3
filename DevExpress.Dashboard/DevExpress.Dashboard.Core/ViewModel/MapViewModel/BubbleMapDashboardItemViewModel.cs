#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

namespace DevExpress.DashboardCommon.ViewModel {
	public class BubbleMapDashboardItemViewModel : GeoPointMapDashboardItemViewModelBase {
		public override GeoPointMapKind MapKind { get { return GeoPointMapKind.BubbleMap; } }
		public string WeightName { get; set; }
		public string ColorName { get; set; }
		public string WeightId { get; set; }
		public string ColorId { get; set; }
		public MapColorizerViewModel Colorizer { get; set; }
		public MapLegendViewModel ColorLegend { get; set; }
		public WeightedLegendViewModel WeightedLegend { get; set; }
		public BubbleMapDashboardItemViewModel()
			: base() {
		}
		public BubbleMapDashboardItemViewModel(BubbleMapDashboardItem dashboardItem)
			: base(dashboardItem) {
			Measure weight = dashboardItem.Weight;
			if(weight != null) {
				WeightName = weight.DisplayName;
				WeightId = weight.ActualId;
			}
			Measure color = dashboardItem.Color;
			if(color != null) {
				ColorName = color.DisplayName;
				ColorId = color.ActualId;
			}
			Colorizer = new MapColorizerViewModel(dashboardItem.ColorPalette, dashboardItem.ColorScale);
			ColorLegend = new MapLegendViewModel(dashboardItem.Legend);
			WeightedLegend = new WeightedLegendViewModel(dashboardItem.WeightedLegend);
		}
		public override bool ShouldUpdateData(GeoPointMapDashboardItemViewModelBase viewModel) {
			if(base.ShouldUpdateData(viewModel))
				return true;
			BubbleMapDashboardItemViewModel bubbleViewModel = viewModel as BubbleMapDashboardItemViewModel;
			if(bubbleViewModel == null)
				return true;
			return WeightId != bubbleViewModel.WeightId || ColorId != bubbleViewModel.ColorId || !object.Equals(Colorizer, bubbleViewModel.Colorizer);
		}
		public override bool ShouldUpdateLegends(GeoPointMapDashboardItemViewModelBase viewModel) {
			BubbleMapDashboardItemViewModel bubbleViewModel = viewModel as BubbleMapDashboardItemViewModel;
			if(bubbleViewModel == null)
				return true;
			return !object.Equals(ColorLegend, bubbleViewModel.ColorLegend) || !object.Equals(WeightedLegend, bubbleViewModel.WeightedLegend);
		}
	}
}
