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
using System.ComponentModel;
using System.Windows;
using DevExpress.Map.Native;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Map;
namespace DevExpress.Xpf.Map {
	public abstract class MapChartItemBase : MapShapeBase, ISupportCoordLocation, IMapChartItem {
		public static readonly DependencyProperty LocationProperty = DependencyPropertyManager.Register("Location",
			typeof(CoordPoint), typeof(MapChartItemBase), new PropertyMetadata(new GeoPoint(0, 0), LayoutPropertyChanged, CoerceLocation));
		public static readonly DependencyProperty SizeProperty = DependencyPropertyManager.Register("Size",
			typeof(double), typeof(MapChartItemBase), new PropertyMetadata(20.0, SizePropertyChanged));
		public static readonly DependencyProperty ItemIdProperty = DependencyPropertyManager.Register("ItemId",
			typeof(object), typeof(MapChartItemBase), new PropertyMetadata(null, LayoutPropertyChanged));
		static void SizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapChartItemBase chart = d as MapChartItemBase;
			if (chart != null)
				chart.OnSizeChanged();
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
		[Category(Categories.Data)]
		public object ItemId {
			get { return (object)GetValue(ItemIdProperty); }
			set { SetValue(ItemIdProperty, value); }
		}
		protected override IMapItemStyleProvider StyleProvider { get { return (MapChartItemInfo)Info; } }
		protected abstract double ItemValue { get; set; }
		#region IMapChartItem implementation
		double IMapChartItem.ValueSizeInPixels {
			get { return Size; }
			set {
				if (Size != value)
					Size = value;
			}
		}
		#endregion
		#region IMapChartDataItem implementation
		object IMapChartDataItem.Argument {
			get { return ItemId; }
			set { ItemId = value; }
		}
		double IMapChartDataItem.Value {
			get { return ItemValue; }
			set { ItemValue = value; }
		}
		#endregion
		protected virtual void OnSizeChanged() {
			UpdateLayout();
		}
		protected override void OnItemInfoChanged() {
			ApplyAppearance();
			UpdateVisibility();
		}
		protected override void CalculateLayoutInMapUnits() {
			Layout.LocationInMapUnits = CoordinateSystem.CoordPointToMapUnit(Location);
			Layout.SizeInMapUnits = new Size(0, 0);
		}
		protected override void CalculateLayout() {
			Layout.SizeInPixels = new Size(Size, Size);
			Point centerScreenPoint = Layer.MapUnitToScreenZeroOffset(Layout.LocationInMapUnits);
			Layout.LocationInPixels = new Point(centerScreenPoint.X - Size / 2, centerScreenPoint.Y - Size / 2);
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
		protected internal override MapItemInfo CreateInfo() {
			return new MapChartItemInfo(this);
		}
	}
}
