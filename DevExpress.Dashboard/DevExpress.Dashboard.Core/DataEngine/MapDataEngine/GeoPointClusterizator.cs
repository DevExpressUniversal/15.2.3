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
using System.Drawing;
using System.Linq;
using DevExpress.DashboardCommon.Data;
using DevExpress.DashboardCommon.DataProcessing;
using DevExpress.DashboardCommon.Service;
using DevExpress.DataAccess.Native.Data;
using DevExpress.Map;
using DevExpress.Map.Native;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.DashboardCommon.Native {
	public interface IGeoPointClusterizator {
		bool Initialized { get; }
		bool HasClusteredResult { get; }
		void UpdateMapInfo(MapClusterizationRequestInfo info);
		void UpdateSelection(IList<GeoPointEx> selection);
		ClusterizedMapGeoPointData CreateClusteredData(bool isOlap, HierarchicalDataParams data, InternalMapDataMembersContainer clientDataDataMembers);
		IList<GeoPointEx> GetClusteredData(IList<GeoPointEx> points);
		GeoPointEx GetCluster(GeoPointEx point);
		void ClearResults();
	}
	public class GeoPointClusterizator : IGeoPointClusterizator {
		static double cellValue = 30;
		static double algorithmRestriction = 1.5;
		static double zoomFactorStep = 0.5;
		public static double CellValue { get { return cellValue; } set { cellValue = value; } }
		public static double AlgorithmRestriction { get { return algorithmRestriction; } set { algorithmRestriction = value; } }
		public static double ZoomFactorStep { get { return zoomFactorStep; } set { zoomFactorStep = value; } }
		protected int PointCountIndex {  get { return pointCountIndex; } }
		public static Size GetFullScreenSize(Size clientSize, IMapUnit leftTopMapUnit, IMapUnit rightBottomMapUnit) {
			MapRectCore viewportRect = new MapRectCore(leftTopMapUnit.X, leftTopMapUnit.Y, rightBottomMapUnit.X - leftTopMapUnit.X, rightBottomMapUnit.Y - leftTopMapUnit.Y);
			int fullScreenWidth = (int)(clientSize.Width / viewportRect.Width);
			int fullScreenHeight = (int)(clientSize.Height / viewportRect.Height);
			int fullScreenSize = Math.Min(fullScreenWidth, fullScreenHeight);
			if(ZoomFactorStep > 0) {
				double dZoomLevel = Math.Log((double)fullScreenSize / 256, 2);
				int iZoomLevel = (int)dZoomLevel;
				double correctZoomLevel = ZoomFactorStep > 1 ? (iZoomLevel / ZoomFactorStep) * ZoomFactorStep : (int)((dZoomLevel - iZoomLevel) / ZoomFactorStep) * ZoomFactorStep + iZoomLevel;
				fullScreenSize = (int)(256 * Math.Pow(2, correctZoomLevel));
			}
			return new Size(fullScreenSize, fullScreenSize);
		}
		readonly MapCoordinateSystemCore coordinateSystem = new MapGeoCoordinateSystemCore(new GeoPointFactory());
		readonly List<GeoPointEx> unclusteredPoints = new List<GeoPointEx>();
		readonly HashSet<GeoPointEx> selection = new HashSet<GeoPointEx>();
		HashSet<GeoPointEx> calculatedPoints = new HashSet<GeoPointEx>();
		Dictionary<GeoPointEx, GeoPointEx> clusteredResult;
		MapViewportState viewport;
		MapViewportState currentViewPort;
		Size fullClientSize;
		List<MapPointCore> clusters;
		int latitudeIndex = -1;
		int longitudeIndex = -1;
		int pointCountIndex = -1;
		bool IGeoPointClusterizator.Initialized { get { return Initialized; } }
		bool IGeoPointClusterizator.HasClusteredResult { get { return clusteredResult != null; } }
		protected virtual bool Initialized { get { return viewport != null && !fullClientSize.IsEmpty; } }
		protected List<MapPointCore> Clusters { get { return clusters; } }
		protected HashSet<GeoPointEx> Selection { get { return selection; } }
		void IGeoPointClusterizator.UpdateMapInfo(MapClusterizationRequestInfo info) {
			if(info == null)
				return;
			this.viewport = info.Viewport;
			this.fullClientSize = GetFullScreenSize(info.ClientSize);
		}
		void IGeoPointClusterizator.UpdateSelection(IList<GeoPointEx> selectedPoints) {
			selection.Clear();
			if(selectedPoints != null) {
				foreach(GeoPointEx point in selectedPoints)
					selection.Add(point);
			}
		}
		ClusterizedMapGeoPointData IGeoPointClusterizator.CreateClusteredData(bool isOlap, HierarchicalDataParams hData, InternalMapDataMembersContainer clientDataDataMembers) {
			StorageSlice slice = hData.Storage.Where((f) => f.KeyColumns.Count() == hData.Storage.Select<StorageSlice, int>((d) => d.KeyColumns.Count()).Max()).SingleOrDefault();
			OrderedDictionary<StorageColumn, string> resultColumns = new OrderedDictionary<StorageColumn, string>();
			List<Type> resultTypes = new List<Type>();
			Dictionary<GeoPointEx, List<object>> olapUniqueCoordinates = new Dictionary<GeoPointEx, List<object>>();
			List<object[]> filteredData = new List<object[]>();
			if(slice != null) {
				int currIndex = 0;
				latitudeIndex = currIndex++;
				StorageColumn latitudeColumn = slice.KeyColumns.First((c) => c.Name == clientDataDataMembers.Latitude.Id);
				resultColumns.Add(latitudeColumn, clientDataDataMembers.Latitude.DataMember);
				resultTypes.Add(clientDataDataMembers.Latitude.DataType);
				longitudeIndex = currIndex++;
				StorageColumn longitudeColumn = slice.KeyColumns.First((c) => c.Name == clientDataDataMembers.Longitude.Id);
				resultTypes.Add(clientDataDataMembers.Longitude.DataType);
				resultColumns.Add(longitudeColumn, clientDataDataMembers.Longitude.DataMember);
				if(clientDataDataMembers.PieArgument != null) {
					StorageColumn pieArgumentColumn = slice.KeyColumns.First((c) => c.Name == clientDataDataMembers.PieArgument.Id);
					resultColumns.Add(pieArgumentColumn, clientDataDataMembers.PieArgument.DataMember);
					resultTypes.Add(clientDataDataMembers.PieArgument.DataType);
					currIndex++;
				}
				foreach(InternalMapDataMemberContainer pair in clientDataDataMembers.Dimensions) {
					resultColumns.Add(slice.KeyColumns.First((c) => c.Name == pair.Id), pair.DataMember);
					resultTypes.Add(pair.DataType);
					currIndex++;
				}
				foreach(InternalMapDataMemberContainer pair in clientDataDataMembers.Measures) {
					StorageColumn mColumn = slice.MeasureColumns.FirstOrDefault((c) => c.Name == pair.Id);
					resultColumns.Add(mColumn, pair.DataMember);
					resultTypes.Add(pair.DataType);
					currIndex++;
				}
				if(!string.IsNullOrEmpty(clientDataDataMembers.PointsCount.Id))
					pointCountIndex = currIndex++;
				else
					pointCountIndex = -1;
				currentViewPort = CorrectFilterViewport();
				if(isOlap) {
					foreach(StorageRow row in slice) {
						if(!IsCoordinatesInViewPort(Helper.ConvertToDouble(hData.Storage.GetOLAPValue(row[latitudeColumn])), Helper.ConvertToDouble(hData.Storage.GetOLAPValue(row[longitudeColumn]))))
							continue;
						object[] dataRow = new object[currIndex];
						for(int i = 0; i < resultColumns.Count; i++) {
							StorageColumn col = resultColumns.Keys[i];
							dataRow[i] = col.IsKey && (col == latitudeColumn || col == longitudeColumn) ? hData.Storage.GetOLAPValue(row[col]) : row[col].MaterializedValue;
						}
						filteredData.Add(dataRow);
						GeoPointEx point = new GeoPointEx(Helper.ConvertToDouble(hData.Storage.GetOLAPValue(row[latitudeColumn])), Helper.ConvertToDouble(hData.Storage.GetOLAPValue(row[longitudeColumn])));
						olapUniqueCoordinates[point] = new List<object> { row[latitudeColumn].MaterializedValue, row[longitudeColumn].MaterializedValue };
					}
				} else {
					foreach(StorageRow row in slice) {
						if(!IsCoordinatesInViewPort(Helper.ConvertToDouble(row[latitudeColumn].MaterializedValue), Helper.ConvertToDouble(row[longitudeColumn].MaterializedValue)))
							continue;
						object[] dataRow = new object[currIndex];
						for(int i = 0; i < resultColumns.Count; i++) {
							StorageColumn col = resultColumns.Keys[i];
							dataRow[i] = row[col].MaterializedValue;
						}
						filteredData.Add(dataRow);
					}
				}
				ClusterCore(filteredData);
			}
			PropertiesRepository props = new PropertiesRepository();
			for(int i = 0; i < resultColumns.Count; i++) {
				string name =  resultColumns.Values[i];
				props.Add(new DashboardItemDataArrayPropertyDescriptor(i, resultTypes[i], name, name));
			}
			if(pointCountIndex >= 0)
				props.Add(new DashboardItemDataArrayPropertyDescriptor(pointCountIndex, clientDataDataMembers.PointsCount.DataType, clientDataDataMembers.PointsCount.DataMember, clientDataDataMembers.PointsCount.DataMember));
			return new ClusterizedMapGeoPointData(filteredData, olapUniqueCoordinates, props);
		}
		protected void InitializeIndexes(int latitudeIndex, int longitudeIndex, int pointCountIndex, List<int> tooltipDimensionIndices) {
			this.latitudeIndex = latitudeIndex;
			this.longitudeIndex = longitudeIndex;
			this.pointCountIndex = pointCountIndex;
		}
		protected virtual void ClusterCore(List<object[]> filteredData) {
			List<List<GeoPointEx>> pointsList = PreparePoints(filteredData);
			calculatedPoints.Clear();
			unclusteredPoints.Clear();
			clusteredResult = new Dictionary<GeoPointEx, GeoPointEx>();
			foreach(List<GeoPointEx> points in pointsList)
				ClusterPoints(points);
			for(int i = 0; i < filteredData.Count; i++) {
				object[] row = (object[])filteredData[i];
				GeoPointEx point = new GeoPointEx(Helper.ConvertToDouble(row[latitudeIndex]), Helper.ConvertToDouble(row[longitudeIndex]));
				GeoPointEx cluster = GetCluster(point);
				row[latitudeIndex] = cluster.Latitude;
				row[longitudeIndex] = cluster.Longitude;
				if(pointCountIndex >= 0)
					row[pointCountIndex] = GetCountValue(point);
			}
		}
		IList<GeoPointEx> IGeoPointClusterizator.GetClusteredData(IList<GeoPointEx> points) {
			return clusteredResult != null ? clusteredResult.Where(row => points.Contains(row.Value)).Select(row => row.Key).ToList() : points;  
		}
		GeoPointEx IGeoPointClusterizator.GetCluster(GeoPointEx point) {
			return GetCluster(point);
		}
		protected GeoPointEx GetCluster(GeoPointEx point) {
			GeoPointEx cluster = null;
			if(clusteredResult != null)
				clusteredResult.TryGetValue(point, out cluster);
			return cluster;
		}
		int GetCountValue(GeoPointEx geoPoint) {
			if(!calculatedPoints.Contains(geoPoint)) {
				calculatedPoints.Add(geoPoint);
				return 1;
			}
			return 0;
		}
		List<List<GeoPointEx>> PreparePoints(IEnumerable<object[]> filterData) {
			HashSet<GeoPointEx> points = new HashSet<GeoPointEx>();
			HashSet<GeoPointEx> selectedPoints = new HashSet<GeoPointEx>();
			foreach(object[] row in filterData) {
				GeoPointEx point = new GeoPointEx(Helper.ConvertToDouble(row[latitudeIndex]), Helper.ConvertToDouble(row[longitudeIndex]));
				HashSet<GeoPointEx> pointHash = selection.Contains(point) ? selectedPoints : points;
				if(!pointHash.Contains(point))
					pointHash.Add(point);
			}
			List<List<GeoPointEx>> res = new List<List<GeoPointEx>>();
			if(points.Count > 0)
				res.Add(new List<GeoPointEx>(points));
			if(selectedPoints.Count > 0)
				res.Add(new List<GeoPointEx>(selectedPoints));
			return res;
		}
		void IGeoPointClusterizator.ClearResults() {
			clusters = null;
			clusteredResult = null;
			unclusteredPoints.Clear();
			calculatedPoints.Clear();
		}
		protected virtual bool IsCoordinatesInViewPort(double latitude, double longitude) {
			return currentViewPort.IsCoordinatesInViewPort(latitude, longitude);
		}
		void ClusterPoints(IList<GeoPointEx> data) {
			IList<IMapUnit> mapUnitData = data.Select(point => GeoPointToMapUnit(point)).ToList();
			IList<List<int>> clusters = ClusterMapUnitData(mapUnitData);
			Dictionary<GeoPointEx, List<int>> clusteredData = new Dictionary<GeoPointEx, List<int>>();
			for(int i = 0; i < clusters.Count; i++) {
				IMapUnit clusteredMapUnit = new MapUnitCore(clusters[i].Sum(l => mapUnitData[l].X) / clusters[i].Count, clusters[i].Sum(k => mapUnitData[k].Y) / clusters[i].Count);
				clusteredData[MapUnitToGeoPoint(clusteredMapUnit)] = clusters[i];
				if(clusters[i].Count == 1)
					unclusteredPoints.Add(data[clusters[i][0]]);
			}
			GeoPointEx[] geoClusters = new GeoPointEx[data.Count];
			foreach(var record in clusteredData) {
				foreach(int index in record.Value)
					geoClusters[index] = record.Key;
			}
			for(int i = 0; i < data.Count; i++)
				clusteredResult.Add(data[i], geoClusters[i]);
		}
		MapViewportState CorrectFilterViewport() {
			if(viewport == null)
				return null;
			IMapUnit leftTopMapUnit = GeoPointToMapUnit(new GeoPointEx(viewport.TopLatitude, viewport.LeftLongitude));
			IMapUnit rightBottomMapUnit = GeoPointToMapUnit(new GeoPointEx(viewport.BottomLatitude, viewport.RightLongitude));
			MapRectCore fullMapRect = new MapRectCore(0, 0, 1, 1);
			MapPointCore leftTopMapPoint = coordinateSystem.MapUnitToScreenPoint(leftTopMapUnit, fullMapRect, new MapSizeCore(fullClientSize.Width, fullClientSize.Height));
			MapPointCore rightBottomMapPoint = coordinateSystem.MapUnitToScreenPoint(rightBottomMapUnit, fullMapRect, new MapSizeCore(fullClientSize.Width, fullClientSize.Height));
			double width = 2 * CellValue * AlgorithmRestriction;
			double height = 2 * CellValue * AlgorithmRestriction;
			leftTopMapPoint.X = leftTopMapPoint.X > width ? leftTopMapPoint.X - width : 0;
			leftTopMapPoint.Y = leftTopMapPoint.Y > height ? leftTopMapPoint.Y - height : 0;
			rightBottomMapPoint.X += width;
			rightBottomMapPoint.Y += height;
			IMapUnit correctLeftTopMapUnit = coordinateSystem.ScreenPointToMapUnit(leftTopMapPoint, fullMapRect, new MapSizeCore(fullClientSize.Width, fullClientSize.Height));
			IMapUnit correctRightBottomMapPoint = coordinateSystem.ScreenPointToMapUnit(rightBottomMapPoint, fullMapRect, new MapSizeCore(fullClientSize.Width, fullClientSize.Height));
			GeoPointEx correctLeftTop = MapUnitToGeoPoint(correctLeftTopMapUnit);
			GeoPointEx correctRightBottom = MapUnitToGeoPoint(correctRightBottomMapPoint);
			return new MapViewportState { LeftLongitude = correctLeftTop.Longitude, TopLatitude = correctLeftTop.Latitude, RightLongitude = correctRightBottom.Longitude, BottomLatitude = correctRightBottom.Latitude };
		}
		Size GetFullScreenSize(Size clientSize) {
			IMapUnit leftTopMapUnit = GeoPointToMapUnit(new GeoPointEx(viewport.TopLatitude, viewport.LeftLongitude));
			IMapUnit rightBottomMapUnit = GeoPointToMapUnit(new GeoPointEx(viewport.BottomLatitude, viewport.RightLongitude));
			return GetFullScreenSize(clientSize, leftTopMapUnit, rightBottomMapUnit);
		}
		IList<List<int>> ClusterMapUnitData(IList<IMapUnit> data) {
			List<MapPointCore> points = data.Select(row => coordinateSystem.MapUnitToScreenPoint(row, new MapRectCore(0, 0, 1, 1), new MapSizeCore(fullClientSize.Width, fullClientSize.Height))).ToList();
			return ClusterMapPoints(points);
		}
		internal IList<List<int>> ClusterMapPoints(IList<MapPointCore> points) {
			Dictionary<MapPointCore, List<int>> cells = new Dictionary<MapPointCore, List<int>>();
			Dictionary<MapPointCore, MapPointCore> cellPoints = new Dictionary<MapPointCore, MapPointCore>();
			double restr = CellValue;
			for(int i = 0; i < points.Count; i++) {
				int cellX = (int)(points[i].X / restr);
				int cellY = (int)(points[i].Y / restr);
				MapPointCore cell = new MapPointCore(cellX, cellY);
				if(cells.ContainsKey(cell)) {
					double x = (cellPoints[cell].X * cells[cell].Count + points[i].X) / (cells[cell].Count + 1);
					double y = (cellPoints[cell].Y * cells[cell].Count + points[i].Y) / (cells[cell].Count + 1);
					cellPoints[cell] = new MapPointCore(x, y);
					cells[cell].Add(i);
				} else {
					cellPoints.Add(cell, new MapPointCore(points[i].X, points[i].Y));
					cells.Add(cell, new List<int> { i });
				}
			}
			return ClusterMapPointCells(cells.ToDictionary(d => cellPoints[d.Key], d => d.Value));
		}
		internal IList<List<int>> ClusterMapPointCells(Dictionary<MapPointCore, List<int>> points) {
			clusters = points.Keys.ToList();
			IList<List<int>> clusterRes = ClusterMapPointCells(clusters, AlgorithmRestriction * AlgorithmRestriction * CellValue * CellValue);
			return clusterRes.Select(row => points.Values.Where((d, i) => row.Contains(i)).SelectMany(x => x).ToList()).ToList();
		}
		internal IList<List<int>> ClusterMapPointCells(IList<MapPointCore> points, double restriction) {
			List<List<double>> distanceMatr = new List<List<double>>();
			for(int i = 0; i < points.Count - 1; i++) {
				List<double> list = new List<double>();
				for(int j = i + 1; j < points.Count; j++)
					list.Add(Distance(points[i], points[j]));
				distanceMatr.Add(list);
			}
			List<List<int>> res = points.Select((p, i) => new List<int> { i }).ToList();
			while(true) {
				if(!Clustered(points, restriction, distanceMatr, res))
					return res;
			}
		}
		bool Clustered(IList<MapPointCore> points, double restriction, List<List<double>> distanceMatr, List<List<int>> res) {
			Point nearest = FindNearestPoints(restriction, distanceMatr);
			int fi = nearest.X;
			int fj = nearest.Y;
			bool clustered = fi != -1 && fj != -1;
			if(clustered) {
				MapPointCore newPoint = new MapPointCore((points[fi].X * res[fi].Count + points[fj].X * res[fj].Count) / (res[fi].Count + res[fj].Count),
					(points[fi].Y * res[fi].Count + points[fj].Y * res[fj].Count) / (res[fi].Count + res[fj].Count));
				points.RemoveAt(fj);
				points[fi] = newPoint;
				res[fi].AddRange(res[fj]);
				res.RemoveAt(fj);
				for(int i = 0; i < fj; i++)
					distanceMatr[i].RemoveAt(fj - 1 - i);
				if(fj < distanceMatr.Count)
					distanceMatr.RemoveAt(fj);
				else
					distanceMatr.RemoveAt(distanceMatr.Count - 1);
				for(int i = 0; i < points.Count; i++) {
					if(i == fi)
						continue;
					double dist = Distance(points[i], points[fi]);
					if(i < fi)
						distanceMatr[i][fi - i - 1] = dist;
					else
						distanceMatr[fi][i - fi - 1] = dist;
				}
			}
			return clustered;
		}
		Point FindNearestPoints(double restriction, List<List<double>> distanceMatr) {
			Point nearest = new Point(-1, -1);
			double min = restriction;
			for(int i = 0; i < distanceMatr.Count; i++) {
				for(int j = 0; j < distanceMatr[i].Count; j++) {
					if(distanceMatr[i][j] < min) {
						min = distanceMatr[i][j];
						nearest.X = i;
						nearest.Y = i + j + 1;
					}
				}
			}
			return nearest;
		}
		protected virtual double Distance(MapPointCore p1, MapPointCore p2) {
			return Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2);
		}
		IMapUnit GeoPointToMapUnit(GeoPointEx point) {
			return coordinateSystem.CoordPointToMapUnit(point);
		}
		GeoPointEx MapUnitToGeoPoint(IMapUnit unit) {
			CoordPoint coordPoint = coordinateSystem.MapUnitToCoordPoint(unit);
			return new GeoPointEx(coordPoint.GetY(), coordPoint.GetX());
		}
	}
	public class MapClusterizationRequestInfo {
		public MapViewportState Viewport { get; set; }
		public Size ClientSize { get; set; }
	}
}
