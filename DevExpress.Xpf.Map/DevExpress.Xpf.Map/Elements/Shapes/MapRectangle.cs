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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Map;
using DevExpress.Map.Native;
namespace DevExpress.Xpf.Map {
	public class MapRectangle : MapShape, IRectangleCore, IPolygonCore {
		static ControlTemplate templateRectangle;
		IList<CoordPoint> innerVertices;
		CoordPoint center;
		static MapRectangle() {
			XamlHelper.SetLocalNamespace(CommonUtils.localNamespace);
			templateRectangle = XamlHelper.GetControlTemplate("<Rectangle x:Name=\"PART_Shape\"/>");
		}
		public static MapRectangle CreateByCenter(MapCoordinateSystem coordSystem, CoordPoint center, double width, double height) {
			MapRectangle result = new MapRectangle() { Height = height, Width = width };
			result.center = center;
			result.Location = BoundingRectItemHelper.CalculateLocation(coordSystem.CoordSystemCore, center, width, height);
			return result;
		}
		public static readonly DependencyProperty LocationProperty = DependencyPropertyManager.Register("Location",
		   typeof(CoordPoint), typeof(MapRectangle), new PropertyMetadata(new GeoPoint(0, 0), LayoutPropertyChanged, CoerceLocation));
		public static readonly DependencyProperty HeightProperty = DependencyPropertyManager.Register("Height",
		  typeof(double), typeof(MapRectangle), new PropertyMetadata(0.0, LayoutPropertyChanged));
		public static readonly DependencyProperty WidthProperty = DependencyPropertyManager.Register("Width",
		  typeof(double), typeof(MapRectangle), new PropertyMetadata(0.0, LayoutPropertyChanged));
		public static readonly DependencyProperty RadiusXProperty = DependencyPropertyManager.Register("RadiusX",
		  typeof(double), typeof(MapRectangle), new PropertyMetadata(0.0, AppearancePropertyChanged));
		public static readonly DependencyProperty RadiusYProperty = DependencyPropertyManager.Register("RadiusY",
		  typeof(double), typeof(MapRectangle), new PropertyMetadata(0.0, AppearancePropertyChanged));
		static object CoerceLocation(DependencyObject d, object baseValue) {
			if (baseValue == null)
				return new GeoPoint(0, 0);
			return baseValue;
		}
		protected internal override ControlTemplate ItemTemplate {
			get {
				return templateRectangle;
			}
		}
		internal IList<CoordPoint> InnerVertices {
			get {
				if (innerVertices == null)
					innerVertices = GetVertices();
				return innerVertices;
			}
		}
		[Category(Categories.Layout), TypeConverter(typeof(GeoPointConverter))]
		public CoordPoint Location {
			get { return (CoordPoint)GetValue(LocationProperty); }
			set { SetValue(LocationProperty, value); }
		}
		[Category(Categories.Layout)]
		public double Height {
			get { return (double)GetValue(HeightProperty); }
			set { SetValue(HeightProperty, value); }
		}
		[Category(Categories.Layout)]
		public double Width {
			get { return (double)GetValue(WidthProperty); }
			set { SetValue(WidthProperty, value); }
		}
		[Category(Categories.Appearance)]
		public double RadiusX {
			get { return (double)GetValue(RadiusXProperty); }
			set { SetValue(RadiusXProperty, value); }
		}
		[Category(Categories.Appearance)]
		public double RadiusY {
			get { return (double)GetValue(RadiusYProperty); }
			set { SetValue(RadiusYProperty, value); }
		}
		#region IPolygonCore implementation
		CoordPoint IPointContainerCore.GetPoint(int index) {
			return InnerVertices[index];
		}
		int IPointContainerCore.PointCount { get { return InnerVertices.Count; } }
		CoordBounds IPolygonCore.GetBounds() {
			return CalculateBounds();
		}
		void IPointContainerCore.AddPoint(CoordPoint point) {
		}
		void IPointContainerCore.LockPoints() {
		}
		void IPointContainerCore.UnlockPoints() {
		}
		#endregion
		public MapRectangle() { }
		void ResetVertices() {
			this.innerVertices = null;
		}
		CoordPoint CaclculateNativeCenter() {
			Size size = CoordinateSystem.KilometersToGeoSize(Location, new Size(Width / 2.0, Height / 2.0));
			return CoordinateSystem.PointFactory.CreatePoint(Location.GetX() + size.Width, Location.GetY() - size.Height);
		}
		internal List<CoordPoint> GetVertices() {
			Size offset = CoordinateSystem.KilometersToGeoSize(Location, new Size(Width, Height));
			CoordPoint leftTop = Location;
			CoordPoint rightTop = CoordinateSystem.PointFactory.CreatePoint(Location.GetX() + offset.Width, Location.GetY());
			CoordPoint rightBottom = CoordinateSystem.PointFactory.CreatePoint(Location.GetX() + offset.Width, Location.GetY() - offset.Height);
			CoordPoint leftBottom = CoordinateSystem.PointFactory.CreatePoint(Location.GetX(), Location.GetY() - offset.Height);
			return new List<CoordPoint>() { leftTop, rightTop, rightBottom, leftBottom, leftTop };
		}
		protected internal override void ApplyAppearance() {
			base.ApplyAppearance();
			Rectangle rectangle = Shape as Rectangle;
			if (rectangle != null) {
				rectangle.RadiusX = RadiusX;
				rectangle.RadiusY = RadiusY;
			}
		}
		protected internal override CoordBounds CalculateBounds() {
			Size size = CoordinateSystem.KilometersToGeoSize(Location, new Size(Width, Height));
			return new CoordBounds(Location, Location.Offset(size.Width, -size.Height));
		}
		protected internal override IList<CoordPoint> GetItemPoints() {
			CoordBounds bounds = CalculateBounds();
			return new List<CoordPoint> { CoordinateSystem.PointFactory.CreatePoint(bounds.X1, bounds.Y1), CoordinateSystem.PointFactory.CreatePoint(bounds.X2, bounds.Y2) };
		}
		protected override MapDependencyObject CreateObject() {
			return new MapRectangle();
		}
		protected override void CalculateLayoutInMapUnits() {
			ResetVertices();
			Layout.Location = Location;
			Layout.LocationInMapUnits = CoordinateSystem.CoordPointToMapUnit(Location);
			Layout.Size = CoordinateSystem.KilometersToGeoSize(Layout.Location, new Size(Width, Height));
			MapUnit rightBottom = CoordinateSystem.CoordPointToMapUnit(CoordinateSystem.PointFactory.CreatePoint(Location.GetX() + Layout.Size.Width, Location.GetY() - Layout.Size.Height));
			Layout.SizeInMapUnits = new Size(rightBottom.X - Layout.LocationInMapUnits.X, rightBottom.Y - Layout.LocationInMapUnits.Y);
		}
		protected internal override CoordPoint GetCenterCore() {
			return center ?? CaclculateNativeCenter();
		}
	}
}
