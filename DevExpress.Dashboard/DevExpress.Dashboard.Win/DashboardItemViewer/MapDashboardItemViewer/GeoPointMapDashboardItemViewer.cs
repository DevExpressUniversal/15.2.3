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

using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.XtraMap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public abstract class GeoPointMapDashboardItemViewerBase : MapDashboardItemViewer {
		internal GeoPointMapDashboardItemViewControlBase GeoPointViewControlBase { get { return (GeoPointMapDashboardItemViewControlBase)MapViewControl; } }
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Mobility", "CA1601")]
		protected GeoPointMapDashboardItemViewerBase() {
			GeoPointViewControlBase.ClusteredViewportChanged += GeoPointViewControl_ClusteredViewportChanged;
		}
		protected override bool CancelSelection(MapItem item) {
			return GeoPointViewControlBase.IsShapeFileLayer(item.Layer);
		}
		internal override void RaiseViewportChangedTimerTick() {
			base.RaiseViewportChangedTimerTick();
			ProcessDataRequest();
		}
		protected override void SizeChangedAction() {
			ProcessDataRequest();
		}
		void GeoPointViewControl_ClusteredViewportChanged(object sender, EventArgs e) {
			ProcessDataRequest();
		}
		void ProcessDataRequest() {
			if(MapControl.Layers.Count > 0 && GeoPointViewControlBase.EnableClustering) {
				MapViewportState viewportState = MapControl.GetViewportState();
				MapPoint topLeftPoint = MapControl.CoordPointToScreenPoint(new GeoPoint(viewportState.TopLatitude, viewportState.LeftLongitude));
				MapPoint bottomRightPoint = MapControl.CoordPointToScreenPoint(new GeoPoint(viewportState.BottomLatitude, viewportState.RightLongitude));				
				Hashtable viewport = new Hashtable();
				viewport.Add("LeftLongitude", viewportState.LeftLongitude);
				viewport.Add("TopLatitude", viewportState.TopLatitude);
				viewport.Add("RightLongitude", viewportState.RightLongitude);
				viewport.Add("BottomLatitude", viewportState.BottomLatitude);
				Hashtable clientSize = new Hashtable();
				clientSize.Add("width", (int)(bottomRightPoint.X - topLeftPoint.X));
				clientSize.Add("height", (int)(bottomRightPoint.Y - topLeftPoint.Y));
				Hashtable itemClientState = new Hashtable();
				itemClientState.Add("viewport", viewport);
				itemClientState.Add("clientSize", clientSize);
				Hashtable clientState = new Hashtable();
				clientState.Add(DashboardItemName, itemClientState);
				ServiceClient.RequestData(DashboardItemName, clientState);
			}
		}
		protected override DataPointInfo GetDataPointInfo(Point location, bool onlyTargetAxes) {
			DataPointInfo dataPoint = new DataPointInfo();
			MapHitInfo hitInfo = MapControl.CalcHitInfo(location);
			IList values = GetItemValues(hitInfo);
			if(values != null) {
				dataPoint.DimensionValues.Add(DashboardDataAxisNames.DefaultAxis, values);
				dataPoint.Measures.AddRange(GetMeasureIds(hitInfo));
				return dataPoint;
			}
			return null;
		}
		internal override bool ClearClientViewportState() {
			bool cleared = base.ClearClientViewportState();
			if(cleared)
				ProcessDataRequest();
			return cleared;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) 
				GeoPointViewControlBase.ClusteredViewportChanged -= GeoPointViewControl_ClusteredViewportChanged;
			base.Dispose(disposing);
		}
		protected abstract IList<string> GetMeasureIds(MapHitInfo hitInfo);
		protected abstract IList GetItemValues(MapHitInfo hitInfo);
	}
	[DXToolboxItem(false)]
	public class GeoPointMapDashboardItemViewer : GeoPointMapDashboardItemViewerBase {
		GeoPointMapDashboardItemViewControl GeoPointViewControl { get { return (GeoPointMapDashboardItemViewControl)GeoPointViewControlBase; } }
		protected override void Update(MapDashboardItemViewModel viewModel, MultiDimensionalData data, bool dataChanged) {
			GeoPointViewControl.Update((GeoPointMapDashboardItemViewModel)viewModel, data, dataChanged);
		}
		protected override MapDashboardItemViewControl CreateViewControl(DashboardMapControl mapControl) {
			return new GeoPointMapDashboardItemViewControl(mapControl);
		}
		protected override IList<string> GetMeasureIds(MapHitInfo hitInfo) {
			List<string> measureIds = new List<string>();
			if(hitInfo.InMapCallout || hitInfo.InMapBubble) {
				GeoPointMapDashboardItemViewModel geoPointMapModel = ViewModel as GeoPointMapDashboardItemViewModel;
				if(geoPointMapModel != null) 
					measureIds.Add(geoPointMapModel.ValueId);
			}
			return measureIds;
		}
		protected override IList GetItemValues(MapHitInfo hitInfo) {
			MapItem mapItem = null;
			if(hitInfo.InMapCallout)
				mapItem = hitInfo.MapCallout;
			if(hitInfo.InMapBubble)
				mapItem = hitInfo.MapBubble;
			if(mapItem != null)
				return new[] { mapItem.Attributes["LatitudeSelection"].Value, mapItem.Attributes["LongitudeSelection"].Value };
			return null;
		}
	}
	[DXToolboxItem(false)]
	public class BubbleMapDashboardItemViewer : GeoPointMapDashboardItemViewerBase {
		BubbleMapDashboardItemViewControl BubbleViewControl { get { return (BubbleMapDashboardItemViewControl)GeoPointViewControlBase; } }
		protected override void Update(MapDashboardItemViewModel viewModel, MultiDimensionalData data, bool dataChanged) {
			BubbleViewControl.Update((BubbleMapDashboardItemViewModel)viewModel, data, dataChanged);
		}
		protected override MapDashboardItemViewControl CreateViewControl(DashboardMapControl mapControl) {
			return new BubbleMapDashboardItemViewControl(mapControl);
		}
		protected override IList<string> GetMeasureIds(MapHitInfo hitInfo) {
			List<string> measureIds = new List<string>();
			if(hitInfo.InMapBubble) {
				BubbleMapDashboardItemViewModel bubbleMapModel = ViewModel as BubbleMapDashboardItemViewModel;
				if(bubbleMapModel != null) {
					measureIds.Add(bubbleMapModel.WeightId);
					measureIds.Add(bubbleMapModel.ColorId);
				}
			}
			return measureIds;
		}
		protected override IList GetItemValues(MapHitInfo hitInfo) {
			if(hitInfo.InMapBubble) {
				MapItem mapItem = hitInfo.MapBubble;
				return new[] { mapItem.Attributes["LatitudeSelection"].Value, mapItem.Attributes["LongitudeSelection"].Value };
			}
			return null;
		}
	}
	[DXToolboxItem(false)]
	public class PieMapDashboardItemViewer : GeoPointMapDashboardItemViewerBase {
		PieMapDashboardItemViewControl PieViewControl { get { return (PieMapDashboardItemViewControl)GeoPointViewControlBase; } }
		protected override void Update(MapDashboardItemViewModel viewModel, MultiDimensionalData data, bool dataChanged) {
			PieViewControl.Update((PieMapDashboardItemViewModel)viewModel, data, dataChanged);
		}
		protected override MapDashboardItemViewControl CreateViewControl(DashboardMapControl mapControl) {
			return new PieMapDashboardItemViewControl(mapControl);
		}
		protected override IList<string> GetMeasureIds(MapHitInfo hitInfo) {
			List<string> measureIds = new List<string>();
			if(hitInfo.InMapPie) {
				PieMapDashboardItemViewModel pieMapModel = ViewModel as PieMapDashboardItemViewModel;
				if(pieMapModel != null) {
					if(pieMapModel.ArgumentDataId == null && pieMapModel.Values.Count > 0) 
						measureIds.Add(pieMapModel.Values[0]);
					else {
						foreach(string valueId in pieMapModel.Values)
							measureIds.Add(valueId);
					}
				}
			}
			return measureIds;
		}
		protected override IList GetItemValues(MapHitInfo hitInfo) {
			if(hitInfo.InMapPie) {
				MapItem mapItem = hitInfo.MapPie;
				return new[] { ((IList)mapItem.Attributes["LatitudeSelection"].Value)[0], ((IList)mapItem.Attributes["LongitudeSelection"].Value)[0] };
			}
			return null;
		}
	}
}
