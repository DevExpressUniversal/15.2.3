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
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardCommon.ViewModel {
	public enum GeoPointMapKind { GeoPoint, BubbleMap, PieMap }
	public abstract class GeoPointMapDashboardItemViewModelBase : MapDashboardItemViewModel {
		public abstract GeoPointMapKind MapKind { get; }
		public string LatitudeDataId { get; set; }
		public string LongitudeDataId { get; set; }
		public string PointsCountDataId { get; set; }
		public bool EnableClustering { get; set; }
		public IList<TooltipDataItemViewModel> TooltipDimensions { get; set; }
		protected GeoPointMapDashboardItemViewModelBase()
			: base() {
		}
		protected GeoPointMapDashboardItemViewModelBase(GeoPointMapDashboardItemBase dashboardItem)
			: base(dashboardItem, dashboardItem.CreateTooltipMeasureViewModel()) {
			ContentDescription = new ContentDescriptionViewModel(dashboardItem.ElementContainer);
			MapItems = PrepareMapItems(dashboardItem.MapItems);
			EnableClustering = dashboardItem.EnableClustering;
			if(dashboardItem.IsMapReady) {
				LatitudeDataId = dashboardItem.Latitude.ActualId;
				LongitudeDataId = dashboardItem.Longitude.ActualId;
			}
			if(EnableClustering)
				PointsCountDataId = GeoPointMapDashboardItemBase.PointsCountDataId;
			TooltipDimensions = dashboardItem.CreateTooltipDimensionViewModel();
		}
		public bool ShouldUpdateGeometry(GeoPointMapDashboardItemViewModelBase viewModel) {
			if(viewModel == null)
				return true;
			if(ShapeTitleAttributeName != viewModel.ShapeTitleAttributeName)
				return true;
			return !Helper.DataEquals(MapItems, viewModel.MapItems);
		}
		public virtual bool ShouldUpdateData(GeoPointMapDashboardItemViewModelBase viewModel) {
			if(viewModel == null)
				return true;
			return LatitudeDataId != viewModel.LatitudeDataId || LongitudeDataId != viewModel.LongitudeDataId || PointsCountDataId != viewModel.PointsCountDataId;
		}
		public virtual bool ShouldUpdateLegends(GeoPointMapDashboardItemViewModelBase viewModel) {
			return false;
		}
	}
}
