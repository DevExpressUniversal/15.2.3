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
using System.IO;
using System.Reflection;
using System.Threading;
using DevExpress.Map.Native;
using DevExpress.Map;
using System.Text;
using DevExpress.Utils;
namespace DevExpress.DashboardCommon.Native {
	public static class DashboardShapefileLoader {
		public static Dictionary<ShapefileArea, string> ChoroplethDefaultMapUrls = new Dictionary<ShapefileArea, string>();
		public static Dictionary<ShapefileArea, string> GeoPointDefaultMapUrls = new Dictionary<ShapefileArea, string>();
		static DashboardShapefileLoader() {
			ChoroplethDefaultMapUrls.Add(ShapefileArea.WorldCountries, "WorldCountries");
			ChoroplethDefaultMapUrls.Add(ShapefileArea.Asia, "Asia");
			ChoroplethDefaultMapUrls.Add(ShapefileArea.USA, "USA");
			ChoroplethDefaultMapUrls.Add(ShapefileArea.Europe, "Europe");
			ChoroplethDefaultMapUrls.Add(ShapefileArea.NorthAmerica, "North America");
			ChoroplethDefaultMapUrls.Add(ShapefileArea.SouthAmerica, "South America");
			ChoroplethDefaultMapUrls.Add(ShapefileArea.Africa, "Africa");
			ChoroplethDefaultMapUrls.Add(ShapefileArea.Canada, "Canada");
			GeoPointDefaultMapUrls.Add(ShapefileArea.WorldCountries, "WorldCountries GeoPoint");
			GeoPointDefaultMapUrls.Add(ShapefileArea.Asia, "Asia GeoPoint");
			GeoPointDefaultMapUrls.Add(ShapefileArea.USA, "USA GeoPoint");
			GeoPointDefaultMapUrls.Add(ShapefileArea.Europe, "Europe");
			GeoPointDefaultMapUrls.Add(ShapefileArea.NorthAmerica, "North America");
			GeoPointDefaultMapUrls.Add(ShapefileArea.SouthAmerica, "South America");
			GeoPointDefaultMapUrls.Add(ShapefileArea.Africa, "Africa");
			GeoPointDefaultMapUrls.Add(ShapefileArea.Canada, "Canada");
		}
		public static MapShapeItem[] Load(MapDashboardItem map) {
			if(map is ChoroplethMapDashboardItem)
				return Load(map, ChoroplethDefaultMapUrls);
			if(map is GeoPointMapDashboardItemBase)
				return Load(map, GeoPointDefaultMapUrls);
			return new MapShapeItem[0];
		}
		internal static string PrepareDefaultUrl(string url) {
			return string.Format("DevExpress.DashboardCommon.ShapeFiles.{0}.shp", url);
		}
		static MapShapeItem[] Load(MapDashboardItem map, Dictionary<ShapefileArea, string> defaultMapUrls) {
			MapShapeItem[] items = null;
			try {
				ShapeFileLoader loader = new ShapeFileLoader();
				loader.DefaultEncoding = Encoding.UTF8;
				AutoResetEvent waitHandle = new AutoResetEvent(false);
				loader.ItemsLoaded += (sender, e) => {
					items = e.Items.ToArray();
					waitHandle.Set();
				};
				if(map.IsDefault) {
					Assembly assembly = typeof(DashboardShapefileLoader).GetAssembly();
					string url = PrepareDefaultUrl(defaultMapUrls[map.Area]);
					Stream shpStream = assembly.GetManifestResourceStream(url);
					Stream dbfStream = assembly.GetManifestResourceStream(url.Replace(".shp", ".dbf"));
					loader.Load(shpStream, dbfStream);
				}
				else {
					CustomShapefile custom = map.CustomShapefile;
					CustomShapefileData data = custom.Data;
					if(data != null)
						loader.Load(PrepareStream(data.ShapeData), PrepareStream(data.AttributeData));
					else if(!string.IsNullOrEmpty(custom.Url))
						loader.Load(new Uri(custom.Url), false);
					else
						return new MapShapeItem[0];
				}
				waitHandle.WaitOne();
			}
			catch { }
			return items;
		}
		static Stream PrepareStream(byte[] data) {
			if(data == null)
				return null;
			Stream stream = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(stream);
			writer.Write(data);
			stream.Seek(0, SeekOrigin.Begin);
			return stream;
		}
	}
	class MapLoaderFactory : MapLoaderFactoryCore<MapShapeItem> {
		internal MapLoaderFactory() { }
		public override MapShapeItem CreateDot(CoordPoint point) {
			return new MapShapeDot(point.GetY(), point.GetX());
		}
		public override MapShapeItem CreateDot(CoordPoint point, double size) {
			return new MapShapeDot(point.GetY(), point.GetX());
		}
		public override MapShapeItem CreatePath() {
			return new MapShapePath();
		}
		public override MapShapeItem CreateImageAndText(CoordPoint point, ImageTextInfoCore info) {
			return null;
		}
		public override MapShapeItem CreatePolyline() {
			return new MapShapePolyline();
		}
		public override MapShapeItem CreateEllipse(CoordPoint location, double width, double height) {
			throw new NotSupportedException();
		}
		public override MapShapeItem CreateLine(CoordPoint point1, CoordPoint point2) {
			throw new NotSupportedException();
		}
		public override MapShapeItem CreatePolygon() {
			throw new NotSupportedException();
		}
		public override MapShapeItem CreateRectangle(CoordPoint location, double width, double height) {
			throw new NotSupportedException();
		}
	}
	class ShapeFileLoader : ShapefileLoaderCore<MapShapeItem> {
		internal ShapeFileLoader() : base(new MapLoaderFactory()) {
			CoordinateSystem = new GeoCoordSystemCore(GeoPointFactory.Instance);
		}
		protected override ResourceLoaderBehaviorBase GetResourceLoaderBehavior(Uri resourceUri) {
			return null;
		}
	}
}
