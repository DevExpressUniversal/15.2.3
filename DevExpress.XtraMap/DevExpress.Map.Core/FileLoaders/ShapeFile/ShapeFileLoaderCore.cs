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
using System.IO;
using System.Net;
using System.Text;
using DevExpress.Utils;
namespace DevExpress.Map.Native {
	public abstract class ShapefileLoaderCore<TItem> : MapLoaderCore<TItem> {
		bool shpLoaded;
		bool dbfLoaded;
		Stream shpStream;
		Stream dbfStream;
		List<TItem> items;
		Shapefile shapeFile;
		CoordBounds boundingRect;
		Encoding defaultEncoding = DXEncoding.Default;
		protected override CoordBounds BoundingRect { get { return boundingRect; } }
		public Encoding DefaultEncoding {
			get { return defaultEncoding; }
			set {
				if (Object.Equals(value, defaultEncoding))
					return;
				defaultEncoding = value ?? DXEncoding.Default;
			}
		}
		protected ShapefileLoaderCore(MapLoaderFactoryCore<TItem> factory)
			: base(factory) {
			this.items = new List<TItem>();
		}
		void Load() {
			items = LoadItems(shpStream, dbfStream);
			shpStream.Dispose();
			shpStream = null;
			if (dbfStream != null) {
				dbfStream.Dispose();
				dbfStream = null;
			}
			RaiseItemsLoaded(Items);
			RaiseBoundsCalculated(BoundingRect);
		}
		internal List<TItem> LoadItems(Stream shpSource, Stream dbfSource) {
			shapeFile = new Shapefile(shpSource, dbfSource, DefaultEncoding);
			if ((shapeFile.ShpRecords != null) && (shapeFile.DbfRecords != null) && (shapeFile.ShpRecords.Count != shapeFile.DbfRecords.Count))
				throw new InconsistentDbfException();
			List<TItem> result = new List<TItem>();
			for (int i = 0; i < shapeFile.ShpRecords.Count; i++) {
				DbfRecord dbfRecord = shapeFile.DbfRecords != null ? shapeFile.DbfRecords[i] : null;
				IEnumerable<TItem> items = CreateItems(shapeFile.ShpRecords[i]);
				if (items != null) {
					if (dbfRecord != null)
						SetItemsAttributes(items, dbfRecord);
					result.AddRange(items);
				}
			}
			ShpHeader header = shapeFile.ShpHeader;
			CoordPoint pt1 = CreateConvertedPoint(header.Xmin, header.Ymin);
			CoordPoint pt2 = CreateConvertedPoint(header.Xmax, header.Ymax);
			boundingRect = new CoordBounds(pt1, pt2);
			ClearShapeData();
			return result;
		}
		void ClearShapeData() {
			if (shapeFile.ShpRecords != null)
				shapeFile.ShpRecords.Clear();
			if (shapeFile.DbfRecords != null)
				shapeFile.DbfRecords.Clear();
		}
		void SetItemsAttributes(IEnumerable<TItem> items, DbfRecord record) {
			foreach (TItem item in items)
				foreach (IMapItemAttribute attribute in record.Attributes)
					((IMapItemCore)item).AddAttribute(attribute);
		}
		IEnumerable<TItem> CreateItems(ShpRecord shpRecord) {
			List<TItem> result = CreateItemsAsPoint(shpRecord);
			if (result != null) return result;
			result = CreateItemsAsMultiPointM(shpRecord);
			if (result != null) return result;
			result = CreateItemsAsPolyLineM(shpRecord);
			if (result != null) return result;
			result = CreateItemsAsPolygonM(shpRecord);
			if (result != null) return result;
			result = CreateItemsAsMultiPoint(shpRecord);
			if (result != null) return result;
			result = CreateItemsAsPolyLine(shpRecord);
			if (result != null) return result;
			result = CreateItemsAsPolygon(shpRecord);
			if (result != null) return result;
			result = CreateItemsAsMultiPointZ(shpRecord);
			if (result != null) return result;
			result = CreateItemsAsPolyLineZ(shpRecord);
			if (result != null) return result;
			result = CreateItemsAsPolygonZ(shpRecord);
			if (result != null) return result;
			return new List<TItem>();
		}
		protected CoordPoint CreateConvertedPoint(ShpPoint point) {
			return CreateConvertedPoint(point.X, point.Y);
		}
		protected CoordPoint CreateConvertedPoint(ShpPointM point) {
			return CreateConvertedPoint(point.X, point.Y);
		}
		protected CoordPoint CreateConvertedPoint(ShpPointZ point) {
			return CreateConvertedPoint(point.X, point.Y);
		}
		List<TItem> CreateItemsAsPolygon(ShpRecord shpRecord) {
			PolygonShpRecord polygonRecord = shpRecord as PolygonShpRecord;
			if (polygonRecord != null) {
				List<TItem> result = new List<TItem>();
				TItem pathItem = Factory.CreatePath();
				IPathCore path = pathItem as IPathCore;
				for (int partIndex = 0; partIndex < polygonRecord.PartCount; partIndex++) {
					int end = ((partIndex + 1) == polygonRecord.PartCount) ? polygonRecord.Points.Length : polygonRecord.Parts[partIndex + 1];
					IPathSegmentCore segment = path.CreateSegment();
					segment.IsClosed = true;
					segment.IsFilled = true;
					segment.LockPoints();
					for (int pointIndex = polygonRecord.Parts[partIndex]; pointIndex < end; pointIndex++) {
						ShpPoint point = polygonRecord.Points[pointIndex];
						CoordPoint coordPoint = CreateConvertedPoint(point);
						segment.AddPoint(coordPoint);
					}
					segment.UnlockPoints();
				}
				result.Add(pathItem);
				return result;
			}
			return null;
		}
		List<TItem> CreateItemsAsPolyLine(ShpRecord shpRecord) {
			PolyLineShpRecord polyLineRecord = shpRecord as PolyLineShpRecord;
			if (polyLineRecord != null) {
				List<TItem> result = new List<TItem>();
				TItem pathItem = Factory.CreatePath();
				IPathCore path = pathItem as IPathCore;
				for (int partIndex = 0; partIndex < polyLineRecord.PartCount; partIndex++) {
					int end = ((partIndex + 1) == polyLineRecord.PartCount) ? polyLineRecord.Points.Length : polyLineRecord.Parts[partIndex + 1];
					IPathSegmentCore segment = path.CreateSegment();
					segment.IsClosed = false;
					segment.IsFilled = false;
					segment.LockPoints();
					for (int pointIndex = polyLineRecord.Parts[partIndex]; pointIndex < end; pointIndex++) {
						ShpPoint point = polyLineRecord.Points[pointIndex];
						CoordPoint geoPoint = CreateConvertedPoint(point);
						segment.AddPoint(geoPoint);
					}
					segment.UnlockPoints();
				}
				result.Add(pathItem);
				return result;
			}
			return null;
		}
		List<TItem> CreateItemsAsMultiPoint(ShpRecord shpRecord) {
			MultiPointShpRecord multiPointRecord = shpRecord as MultiPointShpRecord;
			if (multiPointRecord != null) {
				List<TItem> result = new List<TItem>();
				foreach (ShpPoint point in multiPointRecord.Points) {
					CoordPoint coordPoint = CreateConvertedPoint(point);
					result.Add(Factory.CreateDot(coordPoint));
				}
				return result;
			}
			return null;
		}
		List<TItem> CreateItemsAsPolygonM(ShpRecord shpRecord) {
			PolygonMShpRecord polygonMRecord = shpRecord as PolygonMShpRecord;
			if (polygonMRecord != null) {
				List<TItem> result = new List<TItem>();
				TItem pathItem = Factory.CreatePath();
				IPathCore path = pathItem as IPathCore;
				for (int partIndex = 0; partIndex < polygonMRecord.PartCount; partIndex++) {
					int end = ((partIndex + 1) == polygonMRecord.PartCount) ? polygonMRecord.Points.Length : polygonMRecord.Parts[partIndex + 1];
					IPathSegmentCore segment = path.CreateSegment();
					segment.IsClosed = true;
					segment.IsFilled = true;
					segment.LockPoints();
					for (int pointIndex = polygonMRecord.Parts[partIndex]; pointIndex < end; pointIndex++) {
						ShpPointM point = polygonMRecord.Points[pointIndex];
						CoordPoint geoPoint = CreateConvertedPoint(point);
						segment.AddPoint(geoPoint);
					}
					segment.UnlockPoints();
				}
				result.Add(pathItem);
				return result;
			}
			return null;
		}
		List<TItem> CreateItemsAsPolyLineM(ShpRecord shpRecord) {
			PolyLineMShpRecord plRecord = shpRecord as PolyLineMShpRecord;
			if (plRecord != null) {
				List<TItem> result = new List<TItem>();
				TItem pathItem = Factory.CreatePath();
				IPathCore path = pathItem as IPathCore;
				for (int partIndex = 0; partIndex < plRecord.PartCount; partIndex++) {
					int end = ((partIndex + 1) == plRecord.PartCount) ? plRecord.Points.Length : plRecord.Parts[partIndex + 1];
					IPathSegmentCore segment = path.CreateSegment();
					segment.IsClosed = false;
					segment.IsFilled = false;
					segment.LockPoints();
					for (int pointIndex = plRecord.Parts[partIndex]; pointIndex < end; pointIndex++) {
						ShpPointM point = plRecord.Points[pointIndex];
						CoordPoint geoPoint = CreateConvertedPoint(point);
						segment.AddPoint(geoPoint);
					}
					segment.UnlockPoints();
				}
				result.Add(pathItem);
				return result;
			}
			return null;
		}
		List<TItem> CreateItemsAsMultiPointM(ShpRecord shpRecord) {
			MultiPointMShpRecord mpRecord = shpRecord as MultiPointMShpRecord;
			if (mpRecord != null) {
				List<TItem> result = new List<TItem>();
				foreach (ShpPointM pointM in mpRecord.PointMs) {
					CoordPoint geoPoint = CreateConvertedPoint(pointM);
					result.Add(Factory.CreateDot(geoPoint));
				}
				return result;
			}
			return null;
		}
		List<TItem> CreateItemsAsPoint(ShpRecord shpRecord) {
			PointShpRecord pRecord = shpRecord as PointShpRecord;
			if (pRecord != null) {
				CoordPoint coordPoint = CreateConvertedPoint(pRecord.Point);
				return new List<TItem>() { Factory.CreateDot(coordPoint) };
			}
			return null;
		}
		List<TItem> CreateItemsAsMultiPointZ(ShpRecord shpRecord) {
			var mpRecord = shpRecord as MultiPointZShpRecord;
			if (mpRecord != null) {
				List<TItem> result = new List<TItem>();
				foreach (ShpPointZ pointZ in mpRecord.PointZs) {
					CoordPoint point = CreateConvertedPoint(pointZ);
					result.Add(Factory.CreateDot(point));
				}
				return result;
			}
			return null;
		}
		List<TItem> CreateItemsAsPolyLineZ(ShpRecord shpRecord) {
			var plRecord = shpRecord as PolyLineZShpRecord;
			if (plRecord != null) {
				List<TItem> result = new List<TItem>();
				TItem pathItem = Factory.CreatePath();
				IPathCore path = pathItem as IPathCore;
				for (int partIndex = 0; partIndex < plRecord.PartCount; partIndex++) {
					int end = ((partIndex + 1) == plRecord.PartCount) ? plRecord.Points.Length : plRecord.Parts[partIndex + 1];
					IPathSegmentCore segment = path.CreateSegment();
					segment.IsClosed = false;
					segment.IsFilled = false;
					segment.LockPoints();
					for (int pointIndex = plRecord.Parts[partIndex]; pointIndex < end; pointIndex++) {
						ShpPointZ point = plRecord.Points[pointIndex];
						CoordPoint coordPoint = CreateConvertedPoint(point);
						segment.AddPoint(coordPoint);
					}
					segment.UnlockPoints();
				}
				result.Add(pathItem);
				return result;
			}
			return null;
		}
		List<TItem> CreateItemsAsPolygonZ(ShpRecord shpRecord) {
			var polygonZRecord = shpRecord as PolygonZShpRecord;
			if (polygonZRecord != null) {
				List<TItem> result = new List<TItem>();
				TItem pathItem = Factory.CreatePath();
				IPathCore path = pathItem as IPathCore;
				for (int partIndex = 0; partIndex < polygonZRecord.PartCount; partIndex++) {
					int end = ((partIndex + 1) == polygonZRecord.PartCount) ? polygonZRecord.Points.Length : polygonZRecord.Parts[partIndex + 1];
					IPathSegmentCore segment = path.CreateSegment();
					segment.IsClosed = true;
					segment.IsFilled = true;
					segment.LockPoints();
					for (int pointIndex = polygonZRecord.Parts[partIndex]; pointIndex < end; pointIndex++) {
						ShpPointZ point = polygonZRecord.Points[pointIndex];
						CoordPoint geoPoint = CreateConvertedPoint(point);
						segment.AddPoint(geoPoint);
					}
					segment.UnlockPoints();
				}
				result.Add(pathItem);
				return result;
			}
			return null;
		}
		void DbfClientLoadCompleted(object sender, MapWebLoaderEventArgs e) {
			if (e.Error == null) {
				dbfLoaded = true;
				dbfStream = e.Stream;
				if (shpLoaded)
					Load();
			}
		}
		void ShpClientLoadCompleted(object sender, MapWebLoaderEventArgs e) {
			if (e.Error == null) {
				shpLoaded = true;
				shpStream = e.Stream;
				if (shpStream == null)
					throw new IncorrectUriException(e.UserState as Uri);
				if (dbfLoaded)
					Load();
			}
		}
		#region MapLoaderCore
		public override List<TItem> Items {
			get { return items; }
		}
		public void Load(Stream shpStream, Stream dbfStream) {
			items = LoadItems(shpStream, dbfStream);
			RaiseItemsLoaded(Items);
			RaiseBoundsCalculated(BoundingRect);
		}
		protected internal override void LoadFromResource(Uri uri, ResourceLoaderBehaviorBase resourceLoaderBehavior) {
			shpLoaded = true;
			dbfLoaded = true;
			shpStream = resourceLoaderBehavior.OpenResource(uri);
			if (shpStream == null)
				throw new IncorrectUriException(uri);
			dbfStream = resourceLoaderBehavior.OpenResource(CoreUtils.GetDbfUri(uri));
			Load();
		}
		protected internal override void LoadFromWeb(Uri uri) {
			shpLoaded = false;
			dbfLoaded = false;
			MapWebLoader shpClient = new MapWebLoader();
			shpClient.LoadComlete += ShpClientLoadCompleted;
			shpClient.ReadAsync(uri, uri);
			MapWebLoader dbfClient = new MapWebLoader();
			dbfClient.LoadComlete += DbfClientLoadCompleted;
			dbfClient.ReadAsync(CoreUtils.GetDbfUri(uri));
		}
		protected internal override void LoadFromFile(Uri uri) {
			shpStream = dbfStream = null;
			try {
				FileInfo fileInfo = new FileInfo(uri.LocalPath);
				shpStream = fileInfo.OpenRead();
				fileInfo = new FileInfo(CoreUtils.GetDbfUri(uri).LocalPath);
				if (fileInfo.Exists)
					dbfStream = fileInfo.OpenRead();
			}
			catch { }
			if (shpStream == null)
				throw new IncorrectUriException(uri);
			Load();
		}
		#endregion
	}
}
