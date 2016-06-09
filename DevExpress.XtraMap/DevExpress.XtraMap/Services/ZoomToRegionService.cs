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

using DevExpress.Map;
using DevExpress.Utils;
using DevExpress.XtraMap.Native;
using System.Drawing;
using System.Diagnostics.CodeAnalysis;
namespace DevExpress.XtraMap.Services {
	public interface IZoomToRegionService {
		CoordPoint RegionLeftTop { get; }
		CoordPoint RegionRightBottom { get; }
		CoordPoint CenterPoint { get; }
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		void ZoomToRegion(CoordPoint leftTop, CoordPoint rightBottom, CoordPoint centerPoint, double zoomPadding = 0);
	}
	public class ZoomToRegionService : IZoomToRegionService {
		InnerMap map;
		protected bool IsLayersVisible { get { return map.OperationHelper.IsLayersVisible(); } }
		protected InnerMap Map { get { return map; } }
		public ZoomToRegionService(InnerMap map) {
			Guard.ArgumentNotNull(map, "map");
			this.map = map;
		}
		#region IZoomToRegionService Members
		CoordPoint IZoomToRegionService.RegionLeftTop { get { return GetRegionLeftTopInGeoPoint(); } }
		CoordPoint IZoomToRegionService.RegionRightBottom { get { return GetRegionRightBottomInGeoPoint(); } }
		CoordPoint IZoomToRegionService.CenterPoint { get { return map.CenterPoint; } }
		void IZoomToRegionService.ZoomToRegion(CoordPoint leftTop, CoordPoint rightBottom, CoordPoint centerPoint, double zoomPadding) {
			ZoomToRegion(leftTop, rightBottom, centerPoint, zoomPadding);
		}
		internal CoordPoint GetRegionLeftTopInGeoPoint() {
			if (!IsLayersVisible)
				return null;
			return Map.GetViewLeftTopInCoordPoint();
		}
		internal CoordPoint GetRegionRightBottomInGeoPoint() {
			if(!IsLayersVisible)
				return null;
			return Map.GetViewRightBottomInCoordPoint();
		}
		internal void ZoomToRegion(CoordPoint leftTop, CoordPoint rightBottom, CoordPoint centerPoint, double zoomPadding) {
			if(!map.OperationHelper.CanZoom())
				return;
			double newZoomLevel = Map.CalculateRegionZoom(leftTop, rightBottom, map.ZoomLevel, zoomPadding);
			Point anchorCenter = RectUtils.GetCenter(map.ContentRectangle);
			map.Zoom(newZoomLevel, new MapPoint(anchorCenter.X, anchorCenter.Y));
			map.CenterPoint = centerPoint;
		}
		#endregion
	}
}
