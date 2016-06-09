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

using System.Collections.Generic;
namespace DevExpress.Map.Native {
	public abstract class SqlGeometryLoader<TItem> : MapLoaderCoreBase<TItem> {
		List<TItem> items;
		public override List<TItem> Items { get { return items; } }
		protected SqlGeometryLoader(MapLoaderFactoryCore<TItem> factory)
			: base(factory) {
			items = new List<TItem>();
		}
		void AddItems(IList<TItem> items, IList<IMapItemAttribute> attributes){
			if(items != null)
				CreateAttributes(items, attributes);
				Items.AddRange(items);
		}
		void CreateAttributes(IList<TItem> items, IList<IMapItemAttribute> attributes) {
			foreach(TItem item in items)
				foreach(IMapItemAttribute attribute in attributes)
					((IMapItemCore)item).AddAttribute(attribute);
		}
		IList<TItem> LoadAsPoint(string sqlData) {
			TItem item = CreateDot(sqlData);
			return item != null ? new List<TItem>() { item } : new List<TItem>();
		}
		IList<TItem> LoadAsMultiPoint(string sqlData) {
			IList<TItem> result = new List<TItem>();
			string[] coords = SqlDataRegex.SplitCoordinateList(sqlData);
			foreach(string coord in coords) {
				TItem item = CreateDot(coord);
				if(item != null)
					result.Add(item);
			}
			return result;
		}
		IList<TItem> LoadAsPolyline(string sqlData) {
			TItem item = CreatePolyline(sqlData);
			return item != null ? new List<TItem>() { item } : new List<TItem>();
		}
		IList<TItem> LoadAsPolygon(string sqlData) {
			TItem pathItem = CreatePolygon(sqlData);
			return pathItem != null ? new List<TItem>() { pathItem } : new List<TItem>();
		}
		IList<TItem> LoadAsGeometryCollection(string sqlData) {
			List<TItem> result = new List<TItem>();
			string geometries = SqlDataRegex.GetGeometriesContent(sqlData);
			List<string> elements = SqlDataRegex.GetGeometries(geometries);
			foreach(string element in elements) {
				IList<TItem> items = ParseItems(element);
				if(items.Count > 0)
					result.AddRange(items);
			}
			return result;
		}
		IList<TItem> LoadAsMultilinestring(string sqlData) {
			List<TItem> result = new List<TItem>();
			string content = SqlDataRegex.GetGeometryContent(sqlData);
			List<string> lines = SqlDataRegex.GetCoordinateList(content);
			foreach(string line in lines) {
				TItem item = CreatePolyline(line);
				if(item != null)
					result.Add(item);
			}
			return result;
		}
		IList<TItem> LoadAsMultiPolygon(string sqlData) {
			TItem pathItem = Factory.CreatePath();
			IPathCore path = pathItem as IPathCore;
			string content = SqlDataRegex.GetGeometryContent(sqlData);
			List<string> elements = SqlDataRegex.GetElementContent(content);
			foreach(string line in elements)
				CreatePolygonSegment(path, line);
			return path.SegmentCount > 0 ? new List<TItem>() { pathItem } : new List<TItem>();
		}
		TItem CreateDot(string sqlData) {
			double x, y, z, m;
			SqlDataRegex.ParseCoordinatesM(sqlData, out x, out y, out z, out m);
			if(double.IsNaN(x) || double.IsNaN(y))
				return default(TItem);
			CoordPoint coordPoint = CreateConvertedPoint(x, y);
			return double.IsNaN(m) ? Factory.CreateDot(coordPoint) : Factory.CreateDot(coordPoint, m);
		}
		TItem CreatePolyline(string sqlData) {
			TItem polyline = Factory.CreatePolyline();
			IPointContainerCore container = (IPointContainerCore)polyline;
			List<string> points = SqlDataRegex.GetCoordinateList(sqlData);
			if(points.Count > 0) {
				string[] coordinates = SqlDataRegex.SplitCoordinateList(points[0]);
				if(coordinates.Length > 0) {
					AddPointsToContainer(coordinates, container);
					return polyline;
				}
			}
			return default(TItem);
		}
		TItem CreatePolygon(string sqlData) {
			TItem pathItem = Factory.CreatePath();
			IPathCore path = pathItem as IPathCore;
			string content = SqlDataRegex.GetGeometryContent(sqlData);
			CreatePolygonSegment(path, content);
			return path.SegmentCount > 0 ? pathItem : default(TItem);
		}
		string ValidateInputLine(string line) {
			if(!string.IsNullOrEmpty(line))
				return line.ToUpper().TrimStart();
			return line;
		}
		void CreatePolygonSegment(IPathCore path, string sqlData) {
			List<string> coordGroups = SqlDataRegex.GetCoordinateList(sqlData);
			if(coordGroups.Count > 0) {
				IPathSegmentCore segment = path.CreateSegment();
				string[] coordinates = SqlDataRegex.SplitCoordinateList(coordGroups[0]);
				AddPointsToContainer(coordinates, segment);
				for(int i = 1; i < coordGroups.Count; i++) {
					string[] innerContourStr = SqlDataRegex.SplitCoordinateList(coordGroups[i]);
					if(innerContourStr.Length > 0) {
						IPointContainerCore innerContour = segment.CreateInnerContour();
						if(innerContour != null)
							AddPointsToContainer(innerContourStr, innerContour);
					}
				}
			}
		}
		void AddPointsToContainer(string[] coordinates, IPointContainerCore container) {
			foreach(string coordStr in coordinates) {
				double x, y;
				SqlDataRegex.ParseCoordinates(coordStr.Trim(), out x, out y);
				CoordPoint point = CreateConvertedPoint(x, y);
				container.AddPoint(point);
			} 
		}
		internal IList<TItem> ParseItems(string line) {
			IList<TItem> result = new List<TItem>();
			line = ValidateInputLine(line);
			switch(GetGeometryType(line)) {
				case SqlGeometryType.Point:
					return LoadAsPoint(line);
				case SqlGeometryType.Polygon:
					return LoadAsPolygon(line);
				case SqlGeometryType.MultiPoint:
					return LoadAsMultiPoint(line);
				case SqlGeometryType.LineString:
					return LoadAsPolyline(line);
				case SqlGeometryType.MultiPolygon:
					return LoadAsMultiPolygon(line);
				case SqlGeometryType.MultiLineString:
					return LoadAsMultilinestring(line);
				case SqlGeometryType.GeometryCollection:
					return LoadAsGeometryCollection(line);
			}
			return result;
		}
		internal SqlGeometryType GetGeometryType(string line) {
			if(string.IsNullOrEmpty(line))
				return SqlGeometryType.Unknown;
			if(line.StartsWith(SQLGeometryTypeNames.Point))
				return SqlGeometryType.Point;
			else if(line.StartsWith(SQLGeometryTypeNames.Polygon))
				return SqlGeometryType.Polygon;
			else if(line.StartsWith(SQLGeometryTypeNames.MultiPoint))
				return SqlGeometryType.MultiPoint;
			else if(line.StartsWith(SQLGeometryTypeNames.LineString))
				return SqlGeometryType.LineString;
			else if(line.StartsWith(SQLGeometryTypeNames.MultiPolygon))
				return SqlGeometryType.MultiPolygon;
			else if(line.StartsWith(SQLGeometryTypeNames.MultiLineString))
				return SqlGeometryType.MultiLineString;
			else if(line.StartsWith(SQLGeometryTypeNames.GeometryCollection))
				return SqlGeometryType.GeometryCollection;
			return SqlGeometryType.Unknown;
		}
		public void Load(IList<SQLGeometryItemCore> content, CoordSystemCore coordSystem) {
			CoordinateSystem = coordSystem;
			Items.Clear();
			foreach(SQLGeometryItemCore geometryItem in content)
				AddItems(ParseItems(geometryItem.GeometryString), geometryItem.Attributes);
			RaiseItemsLoaded(Items);
			RaiseBoundsCalculated(BoundingRect);
		}
	}
}
