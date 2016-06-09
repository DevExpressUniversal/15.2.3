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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Utils;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Map {
	public delegate void LegendItemCreatingEventHandler(object sender, LegendItemCreatingEventArgs e);
	public class LegendItemCreatingEventArgs : EventArgs {
		readonly MapLegendBase legend;
		readonly MapLegendItemBase item;
		int index;
		public MapLegendBase Legend { get { return legend; } }
		public MapLegendItemBase Item { get { return item; } }
		public int Index { get { return index; } }
		public LegendItemCreatingEventArgs(MapLegendBase legend, int index, MapLegendItemBase item) {
			Guard.ArgumentNotNull(legend, "legend");
			Guard.ArgumentNotNull(item, "item");
			this.legend = legend;
			this.item = item;
			this.index = index;
		}
	}
	public class LegendCollection : MapDependencyObjectCollection<MapLegendBase> {
		protected MapControl Map { get { return (MapControl)Owner; } }
		public LegendCollection(MapControl map) {
			((IOwnedElement)this).Owner = map;
		}
		protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			base.OnCollectionChanged(e);
			Map.LegendsCollectionChanged();
		}
		internal void DetachLayer(LayerBase layerToDetach) {
			foreach (MapLegendBase legend in this) {
				ItemsLayerLegend layerLegend = legend as ItemsLayerLegend;
				if (layerLegend != null) {
					if (layerToDetach == null || Object.Equals(layerToDetach, layerLegend.Layer))
						layerLegend.Layer = null;
				}
			}
		}
		internal void DetachLayers(IList layerCollection) {
			if (layerCollection == null)
				DetachLayer(null);
			else {
				foreach (LayerBase layer in layerCollection)
					DetachLayer(layer);
			}
		}
#if DEBUGTEST
		internal IList<MapLegendBase> GetLegendsByLayer(LayerBase layer) {
			List<MapLegendBase> result = new List<MapLegendBase>();
			foreach (MapLegendBase legend in this) {
				ItemsLayerLegend mapLegend = legend as ItemsLayerLegend;
				if (mapLegend != null && object.Equals(mapLegend.Layer, layer))
					result.Add(mapLegend);
			}
			return result;
		}
#endif
	}
	public enum LegendAlignment {
		TopLeft,
		TopCenter,
		TopRight,
		MiddleLeft,
		MiddleRight,
		BottomLeft,
		BottomCenter,
		BottomRight
	}
	[TemplatePart(Name = "PART_ItemsControl", Type = typeof(ItemsControl))]
	public abstract class MapLegendBase : MapElement, IOverlayInfo, INotifyPropertyChanged {
		public static readonly DependencyProperty HeaderProperty = DependencyPropertyManager.Register("Header",
			typeof(string), typeof(MapLegendBase), new PropertyMetadata(null));
		public static readonly DependencyProperty DescriptionProperty = DependencyPropertyManager.Register("Description",
			typeof(string), typeof(MapLegendBase), new PropertyMetadata(null));
		public static readonly DependencyProperty RangeStopsFormatProperty = DependencyPropertyManager.Register("RangeStopsFormat",
			typeof(string), typeof(MapLegendBase), new PropertyMetadata(null, RangeStopsFormatPropertyChanged));
		public static readonly DependencyProperty AlignmentProperty = DependencyPropertyManager.Register("Alignment",
			typeof(LegendAlignment), typeof(MapLegendBase), new PropertyMetadata(LegendAlignment.TopRight, AlignmentPropertyChanged));
		static void RangeStopsFormatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapLegendBase legend = d as MapLegendBase;
			if (legend != null)
				legend.UpdateItems();
		}
		static void AlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapLegendBase legend = d as MapLegendBase;
			if (legend != null)
				legend.UpdateMapOverlays();
		}
		protected static void CustomItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapLegendBase legend = d as MapLegendBase;
			if (legend != null)
				legend.ActualItems.CustomItemsChanged(e.OldValue, e.NewValue);
		}
		[Category(Categories.Appearance)]
		public string RangeStopsFormat {
			get { return (string)GetValue(RangeStopsFormatProperty); }
			set { SetValue(RangeStopsFormatProperty, value); }
		}
		[Category(Categories.Appearance)]
		public string Header {
			get { return (string)GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}
		[Category(Categories.Appearance)]
		public string Description {
			get { return (string)GetValue(DescriptionProperty); }
			set { SetValue(DescriptionProperty, value); }
		}
		[Category(Categories.Layout)]
		public LegendAlignment Alignment {
			get { return (LegendAlignment)GetValue(AlignmentProperty); }
			set { SetValue(AlignmentProperty, value); }
		}
		new HorizontalAlignment HorizontalAlignment { get { return base.HorizontalAlignment; } set { base.HorizontalAlignment = value; } }
		new VerticalAlignment VerticalAlignment { get { return base.VerticalAlignment; } set { base.VerticalAlignment = value; } }
		readonly MapLegendItemCollection innerItems;
		readonly ActualLegendItemCollection actualItems;
		readonly MapOverlayLayout layout = new MapOverlayLayout();
		protected internal MapControl Map { get { return Owner as MapControl; } }
		protected internal MapLegendItemCollection InnerItems { get { return innerItems; } }
		protected internal ActualLegendItemCollection ActualItems { get { return actualItems; } }
		protected internal virtual bool ReverseItems { get { return false; } }
		protected internal virtual IEnumerable<MapLegendItemBase> CustomItemsInternal { get { return new MapLegendItemBase[0]; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsItemsPresent { get { return ActualItems.Count > 0; } }
		protected MapLegendBase() {
			this.actualItems = new ActualLegendItemCollection(this);
			this.innerItems = new MapLegendItemCollection();
		}
		#region IOverlayInfo implementation
		MapOverlayLayout IOverlayInfo.Layout { get { return layout; } }
		Control IOverlayInfo.GetPresentationControl() {
			return this;
		}
		HorizontalAlignment IOverlayInfo.HorizontalAlignment { get { return CalculateHorizontalAlignment(); } }
		VerticalAlignment IOverlayInfo.VerticalAlignment { get { return CalculateVerticalAlignment(); } }
		void IOverlayInfo.OnAlignmentUpdated() {
			UpdateMapOverlays();
		}
		VerticalAlignment CalculateVerticalAlignment() {
			switch (Alignment) {
				case LegendAlignment.TopCenter:
				case LegendAlignment.TopLeft:
				case LegendAlignment.TopRight:
					return VerticalAlignment.Top;
				case LegendAlignment.BottomCenter:
				case LegendAlignment.BottomLeft:
				case LegendAlignment.BottomRight:
					return VerticalAlignment.Bottom;
				case LegendAlignment.MiddleLeft:
				case LegendAlignment.MiddleRight:
					return VerticalAlignment.Center;
			}
			return VerticalAlignment.Stretch;
		}
		HorizontalAlignment CalculateHorizontalAlignment() {
			switch (Alignment) {
				case LegendAlignment.TopLeft:
				case LegendAlignment.BottomLeft:
				case LegendAlignment.MiddleLeft:
					return HorizontalAlignment.Left;
				case LegendAlignment.TopRight:
				case LegendAlignment.BottomRight:
				case LegendAlignment.MiddleRight:
					return HorizontalAlignment.Right;
				case LegendAlignment.TopCenter:
				case LegendAlignment.BottomCenter:
					return HorizontalAlignment.Center;
			}
			return HorizontalAlignment.Stretch;
		}
		#endregion
		void UpdateMapOverlays() {
			if (Map != null)
				Map.InvalidateOverlays();
		}
		protected internal virtual void UpdateItems() {
		}
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
			OverlayLayoutUpdater.LayotPropertyChanged(e.Property, this);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			ItemsControl legendItemsControl = GetTemplateChild("PART_ItemsControl") as ItemsControl;
			if (legendItemsControl != null)
				legendItemsControl.ItemsSource = actualItems;
		}
		PropertyChangedEventHandler propertyChanged;
		#region INotifyPropertyChanged
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged {
			add {
				propertyChanged += value;
			}
			remove {
				propertyChanged -= value;
			}
		}
		#endregion
		protected void RaisePropertyChanged(string name) {
			if(propertyChanged != null)
				propertyChanged(this, new PropertyChangedEventArgs(name));
		}
	}
	public abstract class DataProviderLegend : MapLegendBase {
		protected abstract internal ILegendDataProvider DataProvider { get; }
		void RaiseItemCreating(MapLegendItemBase item, int index) {
			if (Map != null)
				Map.RaiseLegendItemCreating(new LegendItemCreatingEventArgs(this, index, item));
		}
		protected void ClearItems() {
			InnerItems.Clear();
		}
		protected internal virtual void PopulateItems() {
			ClearItems();
			if (DataProvider == null) return;
			IList<MapLegendItemBase> items = DataProvider.CreateItems(this);
			AddInnerItems(items);
		}
		protected virtual void AddInnerItems(IList<MapLegendItemBase> items) {
			for (int i = 0; i < items.Count; i++) {
				MapLegendItemBase item = items[i];
				InnerItems.Add(item);
				RaiseItemCreating(item, i);
			}
		}
		protected internal override void UpdateItems() {
			PopulateItems();
			ActualItems.Update();
			RaisePropertyChanged("IsItemsPresent");
			base.UpdateItems();
		}
	}
	public abstract class ItemsLayerLegend : DataProviderLegend {
		public static readonly DependencyProperty LayerProperty = DependencyPropertyManager.Register("Layer",
			typeof(VectorLayer), typeof(ItemsLayerLegend), new PropertyMetadata(null, LayerPropertyChanged));
		static void LayerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ItemsLayerLegend legend = d as ItemsLayerLegend;
			if (legend != null) {
				legend.UpdateItems();
				LayerBase layer = e.NewValue as LayerBase;
				if (layer != null)
					legend.Visibility = layer.Visibility;
			}
		}
		[Category(Categories.Data)]
		public VectorLayer Layer {
			get { return (VectorLayer)GetValue(LayerProperty); }
			set { SetValue(LayerProperty, value); }
		}
		protected internal override ILegendDataProvider DataProvider { get { return Layer as ILegendDataProvider; } }
	}
}
