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
using System.Linq;
using DevExpress.Map.Kml.Model;
using DevExpress.Map.Native;
using System.Net;
#if DXPORTABLE
using PlatformColor = DevExpress.Compatibility.System.Drawing;
#else
using PlatformColor = System.Drawing;
#endif
namespace DevExpress.Map.Kml {
	public abstract class KmlFileLoaderCore<TItem> : MapFormatLoaderCore<TItem> {
		List<TItem> items;
		Root root;
		IKmlStyleProvider styleProvider;
		public Root Root { get { return root; } }
		protected KmlFileLoaderCore(MapLoaderFactoryCore<TItem> factory)
			: base(factory) {
				this.items = new List<TItem>();
		}
#region MapLoaderCore
		public override List<TItem> Items { get { return items; } }
#endregion
		internal List<TItem> LoadItems(Stream stream) {
			List<TItem> result = new List<TItem>();
			KmlImporter importer = new KmlImporter() { WorkingFolder = WorkingFolder };
			this.styleProvider = importer;
			importer.Import(stream);
			this.root = importer.Root;
			result.AddRange(CreateItems());
			return result;
		}
		protected virtual IEnumerable<TItem> CreateItems() {
			List<TItem> result = new List<TItem>();
			if (Root == null)
				return result;
			PlacemarkList placemarks = Root.Placemarks;
			int count = placemarks.Count;
			for (int i = 0; i < count; i++) {
				Placemark placemark = placemarks[i];
				Style style = styleProvider.ResolveStyle(placemark);
				TItem[] items = CreateMapItems(placemark, style);
				SetItemsAttributes(items, placemark);
				ApplyItemsStyle(items, style);
				result.AddRange(items);
			}
			return result;
		}
		void SetItemsAttributes(IEnumerable<TItem> items, Placemark placemark) {
			foreach (TItem item in items) {
				IMapItemCore mapItem = (IMapItemCore)item;
				mapItem.AddAttribute(new MapItemAttributeCore() { Name = KmlTokens.Name, Type = typeof(string), Value = placemark.Name });
				mapItem.AddAttribute(new MapItemAttributeCore() { Name = KmlTokens.Address, Type = typeof(string), Value = placemark.Address });
				mapItem.AddAttribute(new MapItemAttributeCore() { Name = KmlTokens.PhoneNumber, Type = typeof(string), Value = placemark.PhoneNumber });
				mapItem.AddAttribute(new MapItemAttributeCore() { Name = KmlTokens.Description, Type = typeof(string), Value = placemark.Description });
			}
		}
		TItem[] CreateMapItems(Placemark placemark, Style style) {
			if (placemark == null) return new TItem[0];
			Geometry geometry = placemark.Geometry;
			MultiGeometry multiGeometry = geometry as MultiGeometry;
			if (multiGeometry != null)
				return CreateMapItemsFromMultiGeometry(placemark, style, multiGeometry);
			else {
				TItem mapItem = CreateMapItemFromGeometry(placemark, style, geometry);
				if (mapItem != null) 
					return new TItem[] { mapItem };
			}
			return new TItem[0];
		}
		TItem[] CreateMapItemsFromMultiGeometry(Placemark placemark, Style style, MultiGeometry multiGeometry) {
			int count = multiGeometry.GeometryList.Count;
			List<TItem> result = new List<TItem>();
			TItem pathItem = Factory.CreatePath();
			IPathCore path = pathItem as IPathCore;
			for (int i = 0; i < count; i++) {
				MultiGeometry innerMultiGeometry = multiGeometry.GeometryList[i] as MultiGeometry;
				if(innerMultiGeometry != null) {
					TItem[] innerItems = CreateMapItemsFromMultiGeometry(placemark, style, innerMultiGeometry);
					result.AddRange(innerItems);
					continue;
				}
				Point point = multiGeometry.GeometryList[i] as Point;
				if(point != null && point.Coordinates.Count > 0) {
					result.Add(CreatePin(placemark, style, point));
					continue;
				}
				CreatePathSegmentFromGeometry(path, multiGeometry.GeometryList[i]);
			}
			if(path.SegmentCount > 0)
				result.Add(pathItem);
			return result.ToArray();
		}
		TItem CreateMapItemFromGeometry(Placemark placemark, Style style, Geometry geometry) {
			Point point = geometry as Point;
			if (point != null && point.Coordinates.Count > 0) {
				return CreatePin(placemark, style, point);
			}
			LineString lineStr = geometry as LineString;
			if (lineStr != null) {
				if (lineStr.Coordinates != null) {
					TItem polyline = Factory.CreatePolyline();
					IPointContainerCore container = (IPointContainerCore)polyline;
					AddPointsToContainer(lineStr.Coordinates, container);
					return polyline;
				}
			}
			LinearRing linearRing = geometry as LinearRing;
			if (linearRing != null) {
				if (linearRing.Coordinates != null) {
					TItem polyline = Factory.CreatePolyline();
					IPointContainerCore container = (IPointContainerCore)polyline;
					AddPointsToContainer(lineStr.Coordinates, container);
					return polyline;
				}
			}
			Polygon polygon = geometry as Polygon;
			if (polygon != null) {
				TItem pathItem = Factory.CreatePath();
				IPathCore path = pathItem as IPathCore;
				IPathSegmentCore segment = path.CreateSegment();
				if(polygon.OuterBoundaryIs != null) {
					LinearRing outerRing = polygon.OuterBoundaryIs.LinearRing;
					if(outerRing != null) {
						AddPointsToContainer(outerRing.Coordinates, segment);
						foreach(InnerBoundary innerBoundary in polygon.InnerBoundaryIs) {
							LinearRing ring = innerBoundary.LinearRing;
							if(ring != null) {
								IPointContainerCore innerContour = segment.CreateInnerContour();
								if(innerContour != null)
									AddPointsToContainer(ring.Coordinates, innerContour);
							}
						}
					}
				}
				return pathItem;
			}
			return default(TItem);
		}
		TItem CreatePin(Placemark placemark, Style style, Point point) {
			LatLonPoint pt = point.Coordinates[0];
			ImageTextInfoCore info = new ImageTextInfoCore();
			info.Text = placemark.Name;
			IconStyle iconStyle = GetIconStyle(style);
			if(iconStyle != null) {
				info.ImageUri = iconStyle.Icon != null ? iconStyle.Icon.Href : null;
				info.ImageTransform = CreateKmlImageTransform(iconStyle.Scale, iconStyle.HotSpot, info.ImageUri == null);
				info.ImageScale = iconStyle.Scale;
			}
			LabelStyle labelStyle = GetLabelStyle(style);
			if (labelStyle != null) {
				info.TextColor = labelStyle.Color;
				info.TextScale = labelStyle.Scale;
			}
			TItem imageText = Factory.CreateImageAndText(CoordFactory.CreatePoint(pt.Longitude, pt.Latitude), info);
			return imageText;
		}
		void CreatePathSegmentFromGeometry(IPathCore path, Geometry geometry) {
			LineString lineStr = geometry as LineString;
			if(lineStr != null && lineStr.Coordinates != null) {
				IPathSegmentCore segment = path.CreateSegment();
				segment.IsClosed = false;
				segment.IsFilled = false;
				AddPointsToContainer(lineStr.Coordinates, segment);
			}
			LinearRing linearRing = geometry as LinearRing;
			if(linearRing != null && linearRing.Coordinates != null) {
				IPathSegmentCore segment = path.CreateSegment();
				segment.IsClosed = true;
				segment.IsFilled = false;
				AddPointsToContainer(lineStr.Coordinates, segment);
			}
			Polygon polygon = geometry as Polygon;
			if(polygon != null && polygon.OuterBoundaryIs != null) {
				LinearRing outerRing = polygon.OuterBoundaryIs.LinearRing;
				if(outerRing != null) {
					IPathSegmentCore segment = path.CreateSegment();
					AddPointsToContainer(outerRing.Coordinates, segment);
					foreach(InnerBoundary innerBoundary in polygon.InnerBoundaryIs) {
						LinearRing ring = innerBoundary.LinearRing;
						if(ring != null) {
							IPointContainerCore innerContour = segment.CreateInnerContour();
							if(innerContour != null)
								AddPointsToContainer(ring.Coordinates, innerContour);
						}
					}
				}
			}
		}
		IImageTransform CreateKmlImageTransform(double scale, HotSpot hotSpot, bool useDefaultImage) {
			return new KmlImageTransform() { Scale = scale, HotSpot = hotSpot, UseDefaultImage = useDefaultImage };
		}
		void AddPointsToContainer(LatLonPointCollection points, IPointContainerCore container) {
			int count = points.Count;
			for(int i = 0; i < count; i++) {
				LatLonPoint point = points[i];
				container.AddPoint(CoordFactory.CreatePoint(point.Longitude, point.Latitude));
			} 
		}
		void ApplyItemsStyle(TItem[] items, Style style) {
			if (items == null || style == null)
				return;
			int count = items.Length;
			for (int i = 0; i < count; i++)
				ApplyItemStyle(items[i], style);
		}
		void ApplyItemStyle(TItem item, Style style) {
			ISupportStyleCore itemStyle = item as ISupportStyleCore;
			if (itemStyle == null || style == null)
				return;
			bool strokeEnabled = true;
			PolyStyle polyStyle = style.PolyStyle;
			if (polyStyle != null) {
				if (polyStyle.Fill)
					itemStyle.SetFill(CalculateActualColor(polyStyle.Color, polyStyle.ColorMode));
				strokeEnabled = polyStyle.Outline;
			}
			LineStyle lineStyle = style.LineStyle;
			if (lineStyle != null && strokeEnabled) {
				itemStyle.SetStroke(CalculateActualColor(lineStyle.Color, lineStyle.ColorMode));
				itemStyle.SetStrokeWidth(Convert.ToDouble(lineStyle.Width)); 
			}
		}
		PlatformColor.Color CalculateActualColor(ColorABGR color, ColorMode mode) {
			ColorABGR c = (mode == ColorMode.Random) ? styleProvider.CalculateRandomColor(color) : color;
			return ConvertToColor(c);
		}
		PlatformColor.Color ConvertToColor(ColorABGR color) {
			return DevExpress.Utils.DXColor.FromArgb(color.A, color.R, color.G, color.B);
		}
		IconStyle GetIconStyle(Style style) {
			IconStyle iconStyle = style != null ? style.IconStyle : null;
			return iconStyle ?? IconStyle.Default;
		}
		LabelStyle GetLabelStyle(Style style) {
			LabelStyle iconStyle = style != null ? style.LabelStyle : null;
			return iconStyle ?? LabelStyle.Default;
		}
		protected override void LoadStream(Stream stream) {
			items = LoadItems(stream);
		}
	}
	public class MapItemAttributeCore : IMapItemAttribute {
		public string Name { get; set; }
		public Type Type { get; set; }
		public object Value { get; set; }
	}
	public class KmlImageTransform : IImageTransform {
		protected virtual double DefaultHotSpotX { get { return 0.29; } }
		protected virtual double DefaultHotSpotY { get { return 0.0; } }
		public HotSpot HotSpot { get; set; }
		public double Scale { get; set; }
		public bool UseDefaultImage { get; set; }
		double GetActualImageScale(double imageWidth, double imageHeight, double imageScale) {
			return (imageWidth > 0 && imageHeight > 0) ? 32.0 / (double)Math.Min(imageWidth, imageHeight) * imageScale : 1.0;
		}
		public void CalcImageOrigin(double imageWidth, double imageHeight, out double originX, out double originY) {
			originX = 0; originY = 0;
			double scale = GetActualImageScale(imageWidth, imageHeight, Scale);
			originX = GetHorizontalOrigin(imageWidth * scale);
			originY = GetVerticalOrigin(imageHeight * scale);
		}
		double GetHorizontalOrigin(double imageWidth) {
			double imageHotSpot = HotSpot != null ? HotSpot.X : UseDefaultImage ? DefaultHotSpotX : HotSpot.DefaultX;
			Unit units = HotSpot != null ? HotSpot.XUnits : HotSpot.DefaultUnit;
			switch (units) {
				case Unit.Pixels:
					return imageHotSpot / imageWidth;
				case Unit.InsetPixels:
					return  - (imageHotSpot / imageWidth);
				case Unit.Fraction:
				default:
					return imageHotSpot;
			}
		}
		double GetVerticalOrigin(double imageHeight) {
			double imageHotSpot = HotSpot != null ? HotSpot.Y : UseDefaultImage ? DefaultHotSpotY : HotSpot.DefaultY;
			Unit units = HotSpot != null ? HotSpot.YUnits : HotSpot.DefaultUnit;
			switch (units) {
				case Unit.Pixels:
					return 1.0 - imageHotSpot / imageHeight;
				case Unit.InsetPixels:
					return 1 + imageHotSpot / imageHeight;
				case Unit.Fraction:
				default:
					return (1.0 - imageHotSpot);
			}
		}
	}
}
