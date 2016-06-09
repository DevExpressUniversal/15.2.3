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

using System.Collections.Generic;
namespace DevExpress.DashboardCommon.ViewModel {
	public class PieMapDashboardItemViewModel : GeoPointMapDashboardItemViewModelBase {
		readonly List<string> values = new List<string>();
		public override GeoPointMapKind MapKind { get { return GeoPointMapKind.PieMap; } }
		public string ArgumentDataId { get; set; }
		public IList<string> Values { get { return values; } }
		public MapLegendViewModel ColorLegend { get; set; }
		public WeightedLegendViewModel WeightedLegend { get; set; }
		public bool IsWeighted { get; set; }
		public PieMapDashboardItemViewModel()
			: base() {
		}
		public PieMapDashboardItemViewModel(PieMapDashboardItem dashboardItem)
			: base(dashboardItem) {
			Dimension argument = dashboardItem.Argument;
			if(argument != null) {
				ArgumentDataId = argument.ActualId;
				if (dashboardItem.SelectedValue != null)
					values.Add(dashboardItem.SelectedValue.ActualId);
			}
			else {
				for(int i = 0; i < dashboardItem.Values.Count; i++) {
					values.Add(dashboardItem.Values[i].ActualId);
				}
			}
			ColorLegend = new MapLegendViewModel(dashboardItem.Legend);
			WeightedLegend = new WeightedLegendViewModel(dashboardItem.WeightedLegend);
			IsWeighted = dashboardItem.IsWeighted && Values.Count > 0;
		}
		public override bool ShouldUpdateData(GeoPointMapDashboardItemViewModelBase viewModel) {
			if(base.ShouldUpdateData(viewModel))
				return true;
			PieMapDashboardItemViewModel pieViewModel = viewModel as PieMapDashboardItemViewModel;
			if(pieViewModel == null)
				return true;
			if(IsWeighted != pieViewModel.IsWeighted || ArgumentDataId != pieViewModel.ArgumentDataId || Values.Count != pieViewModel.Values.Count)
				return true;
			for(int i = 0; i < Values.Count; i++) {
				if(Values[i] != pieViewModel.Values[i])
					return true;
			}
			return false;
		}
		public override bool ShouldUpdateLegends(GeoPointMapDashboardItemViewModelBase viewModel) {
			PieMapDashboardItemViewModel pieViewModel = viewModel as PieMapDashboardItemViewModel;
			if(pieViewModel == null)
				return true;
			return IsWeighted != pieViewModel.IsWeighted || !object.Equals(ColorLegend, pieViewModel.ColorLegend) || !object.Equals(WeightedLegend, pieViewModel.WeightedLegend);
		}
	}
}
