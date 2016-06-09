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
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Map;
namespace DevExpress.XtraMap.Native {
	public abstract class MapNotification { }
	public class ZoomChangedNotification : MapNotification {
		public double ZoomLevel { get; set; }
	}
	public class CenterPointChangedNotification : MapNotification {
		public CoordPoint CenterPoint { get; set; }
	}
	public class MiniMapController : IMapNotificationObserver {
		readonly MiniMap miniMap;
		InnerMap map;
		public InnerMap Map { get { return map; } }
		public MiniMap MiniMap { get { return miniMap; } }
		public MiniMapController(MiniMap miniMap) {
			Guard.ArgumentNotNull(miniMap, "miniMap");
			this.miniMap = miniMap;
		}
		public void SetMap(InnerMap map) {
			this.map = map;
		}
		public void UpdateActualZoomLevel(double zoomLevel) {
			MiniMap.ActualZoomLevel = CalculateActualZoomLevel(zoomLevel);
		}
		public void UpdateActualCenterPoint(CoordPoint centerPoint) {
			MiniMap.ActualCenterPoint = MiniMap.Behavior.Center != null ? MiniMap.Behavior.Center : centerPoint; 
		}
		void IMapNotificationObserver.HandleChanged(MapNotification info) {
			CenterPointChangedNotification centerChanged = info as CenterPointChangedNotification;
			UpdateActualCenterPoint(centerChanged != null ? centerChanged.CenterPoint : Map.CenterPoint);
			ZoomChangedNotification zoomChanged = info as ZoomChangedNotification;
			if (zoomChanged != null)
				UpdateActualZoomLevel(zoomChanged.ZoomLevel);
			MiniMap.UpdateViewportRect();
		}
		internal void UpdateMapCenterPoint(Point point) {
			CoordPoint centerPoint = ScreenToMiniMapClient(point);
			Map.CenterPoint = centerPoint;
		}
		internal void Shift(Point offset) {
			MapPoint point = MiniMap.UnitConverter.CoordPointToScreenPoint(Map.CenterPoint);
			Map.CenterPoint = MiniMap.UnitConverter.ScreenPointToCoordPoint(new MapPoint(point.X + offset.X, point.Y + offset.Y), false);
			Application.DoEvents();
		}
		protected internal CoordPoint ScreenToMiniMapClient(Point point) {
			MapPoint mapPoint = new MapPoint(point.X, point.Y);
			return MiniMap.UnitConverter.ScreenPointToCoordPoint(mapPoint);
		}
		protected internal double CalculateActualZoomLevel(double value) {
			double zoomLevel = MiniMap.Behavior.CalculateZoomLevel(value);
			return Map != null ? Map.ValidateMinMaxZoomLevel(zoomLevel) : zoomLevel;
		}
	}
}
