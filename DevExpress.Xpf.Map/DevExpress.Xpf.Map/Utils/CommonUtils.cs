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
using DevExpress.Map.Native;
using DevExpress.Xpf.Core.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
namespace DevExpress.Xpf.Map.Native {
	public static class CommonUtils {
		public const string localNamespace = "clr-namespace:DevExpress.Xpf.Map;assembly=" + AssemblyInfo.SRAssemblyDXMap;
		static MapCoordinateSystem svgDefaultCoordinateSystem;
		public static MapCoordinateSystem SvgDefaultCoordinateSystem {
			get {
				if(svgDefaultCoordinateSystem == null)
					svgDefaultCoordinateSystem = new CartesianMapCoordinateSystem();
				return svgDefaultCoordinateSystem;
			}
		}
		public static Panel GetChildPanel(ItemsControl itemsControl) {
			return LayoutHelper.FindElement(itemsControl, element => element is Panel) as Panel;
		}
		public static void SubscribePropertyChangedWeakEvent(INotifyPropertyChanged oldSource, INotifyPropertyChanged newSource, IWeakEventListener listener) {
			if (listener != null) {
				if (oldSource != null)
					PropertyChangedWeakEventManager.RemoveListener(oldSource, listener);
				if (newSource != null)
					PropertyChangedWeakEventManager.AddListener(newSource, listener);
			}
		}
		public static DoubleCollection CloneDoubleCollection(DoubleCollection collection) {
			DoubleCollection collectionClone = new DoubleCollection();
			if (collection != null)
				foreach (double value in collection)
					collectionClone.Add(value);
			return collectionClone;
		}
		public static string GetUserFriendlyEnumString(object value) {
			string strValue = value.ToString();
			if (string.IsNullOrWhiteSpace(strValue))
				return "";
			StringBuilder result = new StringBuilder();
			foreach (char c in strValue)
				if (char.IsUpper(c) && (result.Length != 0))
					result.Append(" " + char.ToLower(c));
				else
					result.Append(c);
			return result.ToString();
		}
		public static void SetOwnerForValues(object oldValue, object newValue, object owner) {
			SetItemOwner(oldValue, null);
			SetItemOwner(newValue, owner);
		}
		public static void SetItemOwner(object item, object owner) {
			IOwnedElement ownedElement = item as IOwnedElement;
			if (ownedElement != null)
				ownedElement.Owner = owner;
		}
		public static bool IsEmptyStream(Stream stream) {
			return stream == null || Object.Equals(stream, Stream.Null) || stream.Length == 0;
		}
	}
	public static class DebugHelper {
		public static void Fail(string message) {
			Debug.Assert(false, message);
		}
	}
	public class NonTestablePropertyAttribute : Attribute {
		public NonTestablePropertyAttribute() : base() { }
	}
	public static class CoordPointHelper {
		public static CoordBounds SelectItemBounds(MapItem item) {
			return SelectItemsBounds(new List<MapItem>() { item });
		}
		public static CoordBounds SelectLayersItemsBounds(IEnumerable<LayerBase> layers) {
			CoordBounds bounds = CoordBounds.Empty;
			foreach (LayerBase layer in layers) {
				VectorLayerBase itemsLayer = layer as VectorLayerBase;
				if (itemsLayer != null && itemsLayer.IsVisible) {
					CoordBounds layerItemsBounds = SelectItemsBounds(itemsLayer.DataItems);
					bounds = CoordBounds.Union(bounds, layerItemsBounds);
				}
			}
			return bounds;
		}
		public static CoordBounds SelectItemsBounds(IEnumerable<MapItem> items) {
			IList<CoordPoint> points = items.SelectMany<MapItem, CoordPoint>(item => { 
				IList<CoordPoint> itemPoints = item.GetItemPoints();
				return itemPoints == null ? new CoordPoint[0] : itemPoints;
				}).ToList();
			if(points.Count > 0) {
				double y1 = points.Max(p => p.GetY());
				double y2 = points.Min(p => p.GetY());
				double x1 = points.Min(p => p.GetX());
				double x2 = points.Max(p => p.GetX());
				return new CoordBounds(x1, y1, x2, y2);
			}
			return CoordBounds.Empty;
		}
	}
	public class MapPathSegmentHelper {
		readonly PathSegmentHelperCore segmentHelperCore;
		public MapPathSegmentHelper(MapPath path) {
			this.segmentHelperCore = new PathSegmentHelperCore(path, path.CoordinateSystem.PointFactory);
		}
		internal void UpdatePointFactory(CoordObjectFactory pointFactory) {
			segmentHelperCore.UpdatePointFactory(pointFactory);
		}
		public CoordPoint GetMaxSegmentCenter() {
			return segmentHelperCore.GetMaxSegmentCenter();
		}
		public Rect GetMaxSegmentBounds() {
			MapPolyLineSegment segment = (MapPolyLineSegment)segmentHelperCore.MaxSegment;
			return segment != null ? segment.Bounds : Rect.Empty;
		}
		public void Reset() {
			segmentHelperCore.Reset();
		}
	}
	public static class OrthodromeHelper {
		public static IList<IList<CoordPoint>> CalculateLine(CoordPoint point1, CoordPoint point2) {
			OrthodromeCalculator calculator = new OrthodromeCalculator(GeoPointFactory.Instance);
			return calculator.CalculateLine(point1, point2);
		}
		public static IList<IList<CoordPoint>> CalculateLine(IList<CoordPoint> points) {
			OrthodromeCalculator calculator = new OrthodromeCalculator(GeoPointFactory.Instance);
			List<List<CoordPoint>> result = new List<List<CoordPoint>>();
			result.Add(new List<CoordPoint>());
			for(int i = 0; i < points.Count - 1; i++) {
				IList<IList<CoordPoint>> segmentPoints = calculator.CalculateLine(points[i], points[i + 1]);
				result[result.Count - 1].AddRange(segmentPoints[0]);
				if(segmentPoints.Count > 1) {
					result.Add(new List<CoordPoint>());
					result[result.Count - 1].AddRange(segmentPoints[1]);
				}
			}
			return result.Cast<IList<CoordPoint>>().ToList();
		}
	}
}
namespace DevExpress.Xpf.Map {
	public class PushpinTooltipMenuDropAlignmentToLayoutTransformConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (targetType == typeof(Transform)) {
				if ((value is bool) && ((bool)value))
					return new ScaleTransform() { ScaleX = -1.0, ScaleY = 1.0 };
			}
			return null;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class PushpinTooltipMenuDropAlignmentToHorizontalAlignmentConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (targetType == typeof(HorizontalAlignment)) {
				if ((value is bool) && ((bool)value))
					return HorizontalAlignment.Right;
			}
			return HorizontalAlignment.Left;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class PushpinTooltipMenuDropAlignmentToHorizontalOffsetConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (targetType == typeof(double)) {
				if ((value is bool) && ((bool)value))
					return 21;
			}
			return -21;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class PushpinTooltipMenuDropAlignmentToMarginConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (targetType == typeof(Thickness)) {
				if ((value is bool) && ((bool)value))
					return new Thickness(0, 0, 5, 0);
			}
			return new Thickness(51, 0, 0, 0);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class SystemParametersWrapper {
		public bool MenuDropAlignment {
			get { return SystemParameters.MenuDropAlignment; }
		}
	}
}
