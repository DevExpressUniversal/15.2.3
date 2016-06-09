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
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Map.Native;
namespace DevExpress.Xpf.Map {
	[ContentProperty("Data")]
	public class VectorLayer : VectorLayerBase, IWeakEventListener, ILegendDataProvider {
		#region Dependency properties
		public static readonly DependencyProperty ShapeFillProperty = DependencyPropertyManager.Register("ShapeFill",
			typeof(Brush), typeof(VectorLayer), new PropertyMetadata(null, ShapeFillPropertyChanged));
		public static readonly DependencyProperty ShapeStrokeProperty = DependencyPropertyManager.Register("ShapeStroke",
			typeof(Brush), typeof(VectorLayer), new PropertyMetadata(null, ShapeStrokePropertyChanged));
		public static readonly DependencyProperty ShapeStrokeStyleProperty = DependencyPropertyManager.Register("ShapeStrokeStyle",
			typeof(StrokeStyle), typeof(VectorLayer), new PropertyMetadata(null, ShapeStrokeStylePropertyChanged));
		public static readonly DependencyProperty ShapeTitleOptionsProperty = DependencyPropertyManager.Register("ShapeTitleOptions",
			typeof(ShapeTitleOptions), typeof(VectorLayer), new PropertyMetadata(null, ShapeTitleOptionsPropertyChanged));
		public static readonly DependencyProperty HighlightShapeFillProperty = DependencyPropertyManager.Register("HighlightShapeFill",
			typeof(Brush), typeof(VectorLayer), new PropertyMetadata(null));
		public static readonly DependencyProperty HighlightShapeStrokeProperty = DependencyPropertyManager.Register("HighlightShapeStroke",
			typeof(Brush), typeof(VectorLayer), new PropertyMetadata(null));
		public static readonly DependencyProperty HighlightShapeStrokeStyleProperty = DependencyPropertyManager.Register("HighlightShapeStrokeStyle",
			typeof(StrokeStyle), typeof(VectorLayer), new PropertyMetadata(null));
		public static readonly DependencyProperty SelectedShapeFillProperty = DependencyPropertyManager.Register("SelectedShapeFill",
			typeof(Brush), typeof(VectorLayer), new PropertyMetadata(null, OnSelectedStyleChanged));
		public static readonly DependencyProperty SelectedShapeStrokeProperty = DependencyPropertyManager.Register("SelectedShapeStroke",
			typeof(Brush), typeof(VectorLayer), new PropertyMetadata(null, OnSelectedStyleChanged)); 
		public static readonly DependencyProperty SelectedShapeStrokeStyleProperty = DependencyPropertyManager.Register("SelectedShapeStrokeStyle",
			typeof(StrokeStyle), typeof(VectorLayer), new PropertyMetadata(null, ShapeStrokeStylePropertyChanged));
		public static readonly DependencyProperty DataProperty = DependencyPropertyManager.Register("Data",
			typeof(MapDataAdapterBase), typeof(VectorLayer), new PropertyMetadata(null, DataAdapterPropertyChanged));
		public static readonly DependencyProperty ColorizerProperty = DependencyPropertyManager.Register("Colorizer",
			typeof(MapColorizer), typeof(VectorLayer), new PropertyMetadata(null, ColorizerPropertyChanged));
		#endregion
		[Category(Categories.Data)]
		public MapDataAdapterBase Data {
			get { return (MapDataAdapterBase)GetValue(DataProperty); }
			set { SetValue(DataProperty, value); }
		}
		[Category(Categories.Appearance)]
		public Brush ShapeFill {
			get { return (Brush)GetValue(ShapeFillProperty); }
			set { SetValue(ShapeFillProperty, value); }
		}
		[Category(Categories.Appearance)]
		public Brush HighlightShapeFill {
			get { return (Brush)GetValue(HighlightShapeFillProperty); }
			set { SetValue(HighlightShapeFillProperty, value); }
		}
		[Category(Categories.Appearance)]
		public Brush ShapeStroke {
			get { return (Brush)GetValue(ShapeStrokeProperty); }
			set { SetValue(ShapeStrokeProperty, value); }
		}
		[Category(Categories.Appearance)]
		public StrokeStyle ShapeStrokeStyle {
			get { return (StrokeStyle)GetValue(ShapeStrokeStyleProperty); }
			set { SetValue(ShapeStrokeStyleProperty, value); }
		}
		[Category(Categories.Appearance)]
		public Brush HighlightShapeStroke {
			get { return (Brush)GetValue(HighlightShapeStrokeProperty); }
			set { SetValue(HighlightShapeStrokeProperty, value); }
		}
		[Category(Categories.Appearance)]
		public StrokeStyle HighlightShapeStrokeStyle {
			get { return (StrokeStyle)GetValue(HighlightShapeStrokeStyleProperty); }
			set { SetValue(HighlightShapeStrokeStyleProperty, value); }
		}
		[Category(Categories.Appearance)]
		public Brush SelectedShapeFill {
			get { return (Brush)GetValue(SelectedShapeFillProperty); }
			set { SetValue(SelectedShapeFillProperty, value); }
		}
		[Category(Categories.Appearance)]
		public Brush SelectedShapeStroke {
			get { return (Brush)GetValue(SelectedShapeStrokeProperty); }
			set { SetValue(SelectedShapeStrokeProperty, value); }
		}
		[Category(Categories.Appearance)]
		public StrokeStyle SelectedShapeStrokeStyle {
			get { return (StrokeStyle)GetValue(SelectedShapeStrokeStyleProperty); }
			set { SetValue(SelectedShapeStrokeStyleProperty, value); }
		}
		[Category(Categories.Presentation)]
		public ShapeTitleOptions ShapeTitleOptions {
			get { return (ShapeTitleOptions)GetValue(ShapeTitleOptionsProperty); }
			set { SetValue(ShapeTitleOptionsProperty, value); }
		}
		[Category(Categories.Appearance)]
		public new MapColorizer Colorizer {
			get { return (MapColorizer)GetValue(ColorizerProperty); }
			set { SetValue(ColorizerProperty, value); }
		}
		static void ShapeFillPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			VectorLayer layer = d as VectorLayer;
			if (layer != null)
				layer.ApplyShapeAppearance();
		}
		static void ShapeStrokePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			VectorLayer layer = d as VectorLayer;
			if (layer != null)
				layer.ApplyShapeAppearance();
		}
		static void ColorizerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			VectorLayerBase layer = d as VectorLayerBase;
			if(layer != null) {
				layer.Colorizer = (MapColorizer)e.NewValue;
			}
		}
		static void ShapeStrokeStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			VectorLayer layer = d as VectorLayer;
			if (layer != null) {
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as StrokeStyle, e.NewValue as StrokeStyle, layer);
				layer.ApplyShapeAppearance();
			}
		}
		static void ShapeTitleOptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			VectorLayer layer = d as VectorLayer;
			if (layer != null) {
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as ShapeTitleOptions, e.NewValue as ShapeTitleOptions, layer);
				layer.ApplyShapeTitleOptions();
			}
		}
		static void DataAdapterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			VectorLayer layer = d as VectorLayer;
			if (layer != null && e.OldValue != e.NewValue)  {
				CommonUtils.SetOwnerForValues(e.OldValue, e.NewValue, layer);
				layer.UpdateItemsSource(true);
				layer.UpdateLegends();
				layer.CheckCompatibility();
			}
		}
		static void OnSelectedStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			VectorLayer layer = d as VectorLayer;
			if (layer != null)
				layer.UpdateSelectedStyle();
		}
		CoordBounds boundingRect = CoordBounds.Empty;
		protected override MapDataAdapterBase DataAdapter { get { return Data; } }
		public VectorLayer() {
			DefaultStyleKey = typeof(VectorLayer);
		}
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			return PerformWeakEvent(managerType, sender, e);
		}
		bool PerformWeakEvent(Type managerType, object sender, EventArgs e) {
			bool success = false;
			if (managerType == typeof(PropertyChangedWeakEventManager)) {
				if ((sender is StrokeStyle)) {
					ApplyShapeAppearance();
					success = true;
				}
				else if ((sender is ShapeTitleOptions)) {
					ApplyShapeTitleOptions();
					success = true;
				}
			}
			return success;
		}
		void ApplyShapeTitleOptions() {
			foreach(MapItem item in DataItems) {
				MapShapeBase shape = item as MapShapeBase;
				if(shape != null)
					shape.ApplyTitleOptions();
			}
		}
		void UpdateSelectedStyle() {
			ApplyShapeAppearance();
		}
		CoordBounds CalculateItemsBoundingRect(MapVectorItemCollection mapItems) {
			CoordBounds bounds = CoordBounds.Empty;
			foreach (MapItem item in mapItems)
				bounds = CoordBounds.Union(bounds, item.CalculateBounds());
			return bounds;
		}
		protected internal override CoordBounds GetBoundingRect() {
			if (DataAdapter != null) {
				ShapefileDataAdapter shapefileDataAdapter = DataAdapter as ShapefileDataAdapter;
				if (shapefileDataAdapter != null)
					return shapefileDataAdapter.BoundingRect;
				else
					return CalculateItemsBoundingRect(DataAdapter.ItemsCollection);
			}
			return base.GetBoundingRect();
		}
		protected virtual IList<MapLegendItemBase> CreateLegendItems(MapLegendBase legend) {
			ILegendDataProvider provider = Data as ILegendDataProvider;
			if(provider != null && legend is SizeLegend)
				return provider.CreateItems(legend);
			provider = Colorizer as ILegendDataProvider;
			return provider != null ? provider.CreateItems(legend) : new List<MapLegendItemBase>();
		}
		#region ILegendDataProvider Members
		IList<MapLegendItemBase> ILegendDataProvider.CreateItems(MapLegendBase legend) {
			return CreateLegendItems(legend);
		}
		#endregion
	}
}
