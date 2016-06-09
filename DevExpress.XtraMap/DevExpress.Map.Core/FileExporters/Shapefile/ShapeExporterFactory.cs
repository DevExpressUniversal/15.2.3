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

using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
namespace DevExpress.Map.Native {
	public static class ShapeExporterFactory {
		class SinglePointContainer : IPointContainerCore {
			CoordPoint point;
			public SinglePointContainer(CoordPoint point) {
				this.point = point;
			}
			#region IPointContainerCore implementation
			int IPointContainerCore.PointCount { get { return 1; } }
			void IPointContainerCore.LockPoints() {
			}
			void IPointContainerCore.AddPoint(CoordPoint point) {
			}
			void IPointContainerCore.UnlockPoints() {
			}
			CoordPoint IPointContainerCore.GetPoint(int index) {
				return point;
			}
			#endregion
		}
		static void FillPointProperties(IPointRecord record, IPointCore point) {
			CoordPoint location = point.Location;
			record.RecreatePoint(location.GetX(), location.GetY());
		}
		static void FillPointsArrayProperties(IPointsArrayRecord record, IPointContainerCore pointContainer) {
			record.PartCount = 1;
			record.Parts = new int[] { 0 };
			record.PointCount = pointContainer.PointCount;
			record.RecreatePointsArray(pointContainer.PointCount);
			for(int i = 0; i < record.PointCount; i++) {
				CoordPoint point = pointContainer.GetPoint(i);
				record.RecreatePointValue(i, point.GetX(), point.GetY());
			}
		}
		static void FillPolygonProperties(IPolygonRecord record, IPathCore path) {
			record.PartCount = path.SegmentCount;
			ICollection<int> parts = new List<int>();
			IList<ShpPoint> points = new List<ShpPoint>();
			for(int i = 0; i < path.SegmentCount; i++) {
				parts.Add(points.Count);
				IPathSegmentCore segment = path.GetSegment(i);
				record.PartCount += segment.InnerContourCount;
				for(int j = 0; j < segment.PointCount; j++) {
					CoordPoint pt = segment.GetPoint(j);
					points.Add(new ShpPoint() { X = pt.GetX(), Y = pt.GetY() });
				}
				for(int j = 0; j < segment.InnerContourCount; j++) {
					parts.Add(points.Count);
					IPointContainerCore innerContour = segment.GetInnerCountour(j);
					for(int k = 0; k < innerContour.PointCount; k++) {
						CoordPoint pt = innerContour.GetPoint(k);
						points.Add(new ShpPoint() { X = pt.GetX(), Y = pt.GetY() });
					}
				}
			}
			record.Parts = parts.ToArray();
			record.PointCount = points.Count;
			record.RecreatePointsArray(points.Count);
			for(int i = 0; i < points.Count; i++)
				record.RecreatePointValue(i, points[i].X, points[i].Y);
		}
		static IList<T> GetPointRecords<T>(IEnumerable<IMapItemCore> items, Func<int, T> createRecord) where T : IPointRecord {
			List<T> result = new List<T>();
			int i = 0;
			foreach(IMapItemCore item in items) {
				IPointCore point = item as IPointCore;
				if(point != null) {
					T record = createRecord(++i);
					FillPointProperties(record, point);
					result.Add(record);
				}
			}
			return result;
		}
		static IList<T> GetPolylineRecords<T>(IEnumerable<IMapItemCore> items, Func<int, T> createRecord) where T : IPolylineRecord {
			List<T> result = new List<T>();
			int i = 0;
			foreach(IMapItemCore item in items) {
				IPointContainerCore pointContainer = item as IPointContainerCore;
				if(pointContainer == null)
					continue;
				T record = createRecord(++i);
				FillPointsArrayProperties(record, pointContainer);
				result.Add(record);
			}
			return result;
		}
		static IList<T> CreatePolygonRecords<T>(IEnumerable<IMapItemCore> items, Func<int, T> createRecord) where T : IPolygonRecord {
			List<T> result = new List<T>();
			int i = 0;
			foreach(IMapItemCore item in items) {
				IPolygonCore polygon = item as IPolygonCore;
				IPathCore path = item as IPathCore;
				if(polygon == null && path == null)
					continue;
				T record = createRecord(++i);
				if(polygon != null)
					FillPointsArrayProperties(record, polygon);
				else
					FillPolygonProperties(record, path);
				result.Add(record);
			}
			return result;
		}
		static IList<T> GetMultipointRecords<T>(IEnumerable<IMapItemCore> items, Func<int, T> createRecord) where T : IMultipointRecord {
			List<T> result = new List<T>();
			int i = 0;
			foreach(IMapItemCore item in items) {
				IPointCore point = item as IPointCore;
				if(point != null) {
					T record = createRecord(++i);
					FillPointsArrayProperties(record, new SinglePointContainer(point.Location));
					result.Add(record);
				}
			}
			return result;
		}
		public static IList<PointShpRecord> PopulatePoints(IEnumerable<IMapItemCore> items) {
			return GetPointRecords<PointShpRecord>(items, recordNumber => new PointShpRecord(recordNumber));
		}
		public static IList<PolyLineShpRecord> PopulatePolylines(IEnumerable<IMapItemCore> items) {
			return GetPolylineRecords<PolyLineShpRecord>(items, recordNumber => new PolyLineShpRecord(recordNumber));
		}
		public static IList<PolygonShpRecord> PopulatePolygons(IEnumerable<IMapItemCore> items) {
			return CreatePolygonRecords<PolygonShpRecord>(items, recordNumber => new PolygonShpRecord(recordNumber));
		}
		public static IList<MultiPointShpRecord> PopulateMultipoints(IEnumerable<IMapItemCore> items) {
			return GetMultipointRecords<MultiPointShpRecord>(items, recordNumber => new MultiPointShpRecord(recordNumber));
		}
		public static IList<PointZShpRecord> PopulatePointsZ(IEnumerable<IMapItemCore> items) {
			return GetPointRecords<PointZShpRecord>(items, recordNumber => new PointZShpRecord(recordNumber));
		}
		public static IList<PolyLineZShpRecord> PopulatePolylinesZ(IEnumerable<IMapItemCore> items) {
			return GetPolylineRecords<PolyLineZShpRecord>(items, recordNumber => new PolyLineZShpRecord(recordNumber));
		}
		public static IList<PolygonZShpRecord> PopulatePolygonsZ(IEnumerable<IMapItemCore> items) {
			return CreatePolygonRecords<PolygonZShpRecord>(items, recordNumber => new PolygonZShpRecord(recordNumber));
		}
		public static IList<MultiPointZShpRecord> PopulateMultipointsZ(IEnumerable<IMapItemCore> items) {
			return GetMultipointRecords<MultiPointZShpRecord>(items, recordNumber => new MultiPointZShpRecord(recordNumber));
		}
		public static IList<PointMShpRecord> PopulatePointsM(IEnumerable<IMapItemCore> items) {
			return GetPointRecords<PointMShpRecord>(items, recordNumber => new PointMShpRecord(recordNumber));
		}
		public static IList<PolyLineMShpRecord> PopulatePolylinesM(IEnumerable<IMapItemCore> items) {
			return GetPolylineRecords<PolyLineMShpRecord>(items, recordNumber => new PolyLineMShpRecord(recordNumber));
		}
		public static IList<PolygonMShpRecord> PopulatePolygonsM(IEnumerable<IMapItemCore> items) {
			return CreatePolygonRecords<PolygonMShpRecord>(items, recordNumber => new PolygonMShpRecord(recordNumber));
		}
		public static IList<MultiPointMShpRecord> PopulateMultipointsM(IEnumerable<IMapItemCore> items) {
			return GetMultipointRecords<MultiPointMShpRecord>(items, recordNumber => new MultiPointMShpRecord(recordNumber));
		}		
		public static ShapeExporterCoreBase CreateCoreExporter(ShpRecordTypes recordType) {
			switch(recordType) {
				case ShpRecordTypes.Point: return new ShpPointExporter();
				case ShpRecordTypes.PolyLine: return new ShpPolylineExporter();
				case ShpRecordTypes.Polygon: return new ShpPolygonExporter();
				case ShpRecordTypes.MultiPoint: return new ShpMultipointExporter();
				case ShpRecordTypes.PointZ: return new ShpPointZExporter();
				case ShpRecordTypes.PolyLineZ: return new ShpPolylineZExporter();
				case ShpRecordTypes.PolygonZ: return new ShpPolygonZExporter();
				case ShpRecordTypes.MultiPointZ: return new ShpMultipointZExporter();
				case ShpRecordTypes.PointM: return new ShpPointMExporter();
				case ShpRecordTypes.PolyLineM: return new ShpPolylineMExporter();
				case ShpRecordTypes.PolygonM: return new ShpPolygonMExporter();
				case ShpRecordTypes.MultiPointM: return new ShpMultipointMExporter();
			}
			throw new ArgumentException(string.Format("{0} record format is not supported for export", recordType));
		}
	}
}
