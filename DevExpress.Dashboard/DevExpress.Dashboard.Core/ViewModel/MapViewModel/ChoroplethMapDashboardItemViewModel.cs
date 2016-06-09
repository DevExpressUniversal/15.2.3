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

using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardCommon.ViewModel {
	public class ChoroplethMapDashboardItemViewModel : MapDashboardItemViewModel {
		public string AttributeDimensionId { get; set; }
		public ChoroplethColorizerViewModel ChoroplethColorizer { get; set; }
		public string ToolTipAttributeName { get; set; }
		public MapLegendViewModel Legend { get; set; }
		protected override List<string> FilteredAttributes {
			get {
				List<string> filteredAttributes = base.FilteredAttributes;
				string attributeName = ChoroplethColorizer != null ? ChoroplethColorizer.AttributeName : null;
				if(attributeName != null)
					filteredAttributes.Add(attributeName);
				if(ToolTipAttributeName != null)
					filteredAttributes.Add(ToolTipAttributeName);
				return filteredAttributes.Distinct().ToList();
			}
		}
		public ChoroplethMapDashboardItemViewModel()
			: base() {
		}
		public ChoroplethMapDashboardItemViewModel(ChoroplethMapDashboardItem dashboardItem, IList<TooltipDataItemViewModel> tooptipMeasuresViewModel)
			: base(dashboardItem, tooptipMeasuresViewModel) {
			AttributeDimensionId = dashboardItem.AttributeDimension != null ? dashboardItem.AttributeDimension.ActualId : null;
			ToolTipAttributeName = dashboardItem.TooltipAttributeName != null ? dashboardItem.TooltipAttributeName : dashboardItem.AttributeName;
			if(dashboardItem.IsMapReady) {
				ChoroplethMap activeMap = dashboardItem.ActiveMap;
				if(activeMap != null) {
					ValueMap valueLayer = activeMap as ValueMap;
					if(valueLayer != null)
						ChoroplethColorizer = new MapValueColorizerViewModel(dashboardItem, valueLayer);
					DeltaMap deltaLayer = activeMap as DeltaMap;
					if(deltaLayer != null)
						ChoroplethColorizer = new MapDeltaColorizerViewModel(dashboardItem, deltaLayer);
				}
			}
			MapItems = PrepareMapItems(dashboardItem.MapItems);
			Legend = new MapLegendViewModel(dashboardItem.Legend);
		}
		public bool ShouldUpdateGeometry(ChoroplethMapDashboardItemViewModel viewModel) {
			if(viewModel == null)
				return true;
			if(ShapeTitleAttributeName != viewModel.ShapeTitleAttributeName)
				return true;
			if(ChoroplethColorizer == null && viewModel.ChoroplethColorizer != null || ChoroplethColorizer != null && ChoroplethColorizer.ShouldUpdateGeometry(viewModel.ChoroplethColorizer))
				return true;
			if(ToolTipAttributeName != viewModel.ToolTipAttributeName)
				return true;
			return !Helper.DataEquals(MapItems, viewModel.MapItems);
		}
		public bool ShouldUpdateLegend(ChoroplethMapDashboardItemViewModel viewModel) {
			if(viewModel == null)
				return Legend != null && Legend.Visible;
			return !Object.Equals(Legend, viewModel.Legend) ||
				ChoroplethColorizer != null && ChoroplethColorizer.ShouldUpdateLegend(viewModel.ChoroplethColorizer) || ChoroplethColorizer == null && viewModel.ChoroplethColorizer != null;
		}
	}
}
