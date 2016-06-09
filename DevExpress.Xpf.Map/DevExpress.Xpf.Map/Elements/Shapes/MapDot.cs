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
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
namespace DevExpress.Xpf.Map {
	public enum MapDotShapeKind {
		Rectangle,
		Circle
	}
	public class MapDot : MapShape, ISupportCoordLocation, IPointCore , IClusterItem, IClusterable {
		const double DefaultSize = 3.0;
		static ControlTemplate templateEllipse;
		static ControlTemplate templateRectangle;
		IList<IClusterable> clusteredItems = new List<IClusterable>();
		static MapDot() {
			XamlHelper.SetLocalNamespace(CommonUtils.localNamespace);
			templateEllipse = XamlHelper.GetControlTemplate("<Ellipse x:Name=\"PART_Shape\"/>");
			templateRectangle = XamlHelper.GetControlTemplate("<Rectangle x:Name=\"PART_Shape\"/>"); 
		}
		public static readonly DependencyProperty LocationProperty = DependencyPropertyManager.Register("Location",
		   typeof(CoordPoint), typeof(MapDot), new PropertyMetadata(new GeoPoint(0, 0), LayoutPropertyChanged, CoerceLocation));
		public static readonly DependencyProperty SizeProperty = DependencyPropertyManager.Register("Size",
		  typeof(double), typeof(MapDot), new PropertyMetadata(DefaultSize, LayoutPropertyChanged));
		public static readonly DependencyProperty ShapeKindProperty = DependencyPropertyManager.Register("ShapeKind",
		  typeof(MapDotShapeKind), typeof(MapDot), new PropertyMetadata(MapDotShapeKind.Circle, ShapeKindPropertyChanged));
		static void ShapeKindPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapDot dot = d as MapDot;
			if (dot != null)
				dot.OnShapeKindPropertyChanged((MapDotShapeKind)e.OldValue, (MapDotShapeKind)e.NewValue);
		}
		static object CoerceLocation(DependencyObject d, object baseValue) {
			if (baseValue == null)
				return new GeoPoint(0, 0);
			return baseValue;
		}
		[Category(Categories.Layout), TypeConverter(typeof(GeoPointConverter))]
		public CoordPoint Location {
			get { return (CoordPoint)GetValue(LocationProperty); }
			set { SetValue(LocationProperty, value); }
		}
		[Category(Categories.Layout)]
		public double Size {
			get { return (double)GetValue(SizeProperty); }
			set { SetValue(SizeProperty, value); }
		}
		[Category(Categories.Appearance)]
		public MapDotShapeKind ShapeKind {
			get { return (MapDotShapeKind)GetValue(ShapeKindProperty); }
			set { SetValue(ShapeKindProperty, value); }
		}
		protected internal override ControlTemplate ItemTemplate {
			get {
				return GetTemplate(ShapeKind);	
			}
		}
		public MapDot() { }
		ControlTemplate GetTemplate(MapDotShapeKind shapeKind) {
			return (shapeKind == MapDotShapeKind.Circle) ? templateEllipse : templateRectangle;
		}
		void OnShapeKindPropertyChanged(MapDotShapeKind oldValue, MapDotShapeKind newValue) {
			NotifyPropertyChanged(this, new MapItemPropertyChangedEventArgs("ItemTemplate", GetTemplate(oldValue), GetTemplate(newValue)));
		}
		protected override void CalculateLayout() {
			Layout.SizeInPixels = new Size(Size, Size);
			Point centerScreenPoint = Layer.MapUnitToScreenZeroOffset(Layout.LocationInMapUnits);
			Layout.LocationInPixels = new Point(centerScreenPoint.X - Size / 2, centerScreenPoint.Y - Size / 2);
		}
		protected override void CalculateLayoutInMapUnits() {
			if (Layer == null)
				return;
			Layout.LocationInMapUnits = CoordinateSystem.CoordPointToMapUnit(Location);
			Layout.SizeInMapUnits = new Size(0, 0);
		}
		protected internal override CoordBounds CalculateBounds() {
			return new CoordBounds(Location, Location);
		}
		protected internal override CoordPoint GetCenterCore() {
			return Location;
		}
		protected internal override Rect CalculateTitleLayout(Size titleSize, VectorLayerBase layer) {
			Point centerPoint = layer.MapUnitToScreenZeroOffset(Layout.LocationInMapUnits);
			centerPoint = new Point(centerPoint.X - titleSize.Width / 2, centerPoint.Y - titleSize.Height / 2);
			return new Rect(centerPoint, titleSize);
		}
		protected internal override IList<CoordPoint> GetItemPoints() {
			return new CoordPoint[] { Location };
		}
		protected override MapDependencyObject CreateObject() {
			return new MapDot();
		}
		#region IClusterItem, IClusterable implementation
		IClusterItem IClusterable.CreateInstance() {
			return new MapDot();
		}
		IMapUnit IClusterItemCore.GetUnitLocation() {
			return Layout.LocationInMapUnits;
		}
		object IClusterItemCore.Owner {
			get {
				return ((IOwnedElement)this).Owner;
			}
			set {
				SetOwnerInternal(value);
				ApplyTitleOptions();
			}
		}
		void IClusterItem.ApplySize(double size) {
			Size = size;
		}
		IList<IClusterable> IClusterItem.ClusteredItems { get { return clusteredItems; } set { clusteredItems = value; } }
		string IClusterItem.Text { 
			get {
				return Title.Text;
			}
			set {
			   Title.Text = value;
			} 
		}
		#endregion
	}
}
