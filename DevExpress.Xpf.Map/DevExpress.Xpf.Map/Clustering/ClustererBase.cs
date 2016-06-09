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
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Native;
using DevExpress.Xpf.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
namespace DevExpress.Xpf.Map {
	[NonCategorized]
	public class MapViewport {  
		public CoordPoint CenterPoint { get; set; }
		public double ZoomLevel { get; set; }
	}
	public class ClusteredEventArgs : EventArgs {
		readonly IEnumerable<MapItem> items;
		public IEnumerable<MapItem> Items { get { return items; } }
		internal ClusteredEventArgs(IEnumerable<MapItem> items) {
			this.items = items;
		}
	}
	public delegate void ClusteringEventHandler(object sender, EventArgs e);
	public delegate void ClusteredEventHandler(object sender, ClusteredEventArgs e);
	public abstract class MapClustererBase : MapDependencyObject, IOwnedElement {
		object owner;
		readonly VirtualMapItemCollection virtualMapItemCollection = new VirtualMapItemCollection();
		protected MapDataAdapterBase Adapter { get { return owner as MapDataAdapterBase; } }
		#region IOwnedElement implementation
		object IOwnedElement.Owner {
			get { return owner; }
			set {
				if(owner != value) {
					owner = value;
					OwnerChanged();
				}
			}
		}
		#endregion
		protected virtual void OwnerChanged() { }
		protected MapVectorItemCollection CreateItemsCollection() {
			MapVectorItemCollection collection = new MapVectorItemCollection();
			if(Adapter != null)
				CommonUtils.SetItemOwner(collection, Adapter.Layer);
			virtualMapItemCollection.Clear();
			collection.VirtualizingCollection = virtualMapItemCollection;
			return collection;
		}
		public abstract MapVectorItemCollection Items { get; }
		public abstract void Clusterize(MapVectorItemCollection sourceItems, MapViewport mapViewport, bool sourceChanged);
	}
	public abstract class MapClusterer : MapClustererBase, IMapItemSettingsListener {
		public static readonly DependencyProperty StepInPixelsProperty = DependencyPropertyManager.Register("StepInPixels",
		typeof(int), typeof(MapClusterer), new PropertyMetadata(DefaultStep, OnPropertyChanged));
		public static readonly DependencyProperty ItemMaxSizeProperty = DependencyPropertyManager.Register("ItemMaxSize",
		typeof(int), typeof(MapClusterer), new PropertyMetadata(DefaultItemMaxSize, OnItemMaxSizePropertyChanged, MaxSizeCoerce), new ValidateValueCallback(ValidateItemSize));
		public static readonly DependencyProperty ItemMinSizeProperty = DependencyPropertyManager.Register("ItemMinSize",
		typeof(int), typeof(MapClusterer), new PropertyMetadata(DefaultItemMinSize, OnItemMinSizePropertyChanged, MinSizeCoerce), new ValidateValueCallback(ValidateItemSize));
		public static readonly DependencyProperty GroupProviderProperty = DependencyPropertyManager.Register("GroupProvider",
		typeof(MapClustererGroupProviderBase), typeof(MapClusterer), new PropertyMetadata(null, OnPropertyChanged));
		public static readonly DependencyProperty ClusterSettingsProperty = DependencyPropertyManager.Register("ClusterSettings",
		   typeof(MapItemSettings), typeof(MapClusterer), new PropertyMetadata(null, ClusterSettingsPropertyChanged));
		static bool ValidateItemSize(object itemSize) {
			if((int)itemSize <= 0)
				throw new ArgumentException(DXMapStrings.MsgIncorrectItemSize);
			return true;
		}
		static void ItemSizeCoerce(MapClusterer clusterer, int minValue, int maxValue) {
			if(minValue > maxValue && clusterer.Layer != null && clusterer.Layer.IsLoaded)
				throw new ArgumentException(DXMapStrings.MsgIncorrectItemMinMaxSize);
		}
		static object MinSizeCoerce(DependencyObject d, object baseValue) {
			MapClusterer clusterer = d as MapClusterer;
			if(clusterer != null)
				ItemSizeCoerce(clusterer, (int)baseValue, clusterer.ItemMaxSize);
			return baseValue;
		}
		static object MaxSizeCoerce(DependencyObject d, object baseValue) {
			MapClusterer clusterer = d as MapClusterer;
			if(clusterer != null)
				ItemSizeCoerce(clusterer, clusterer.ItemMinSize, (int)baseValue);
			return baseValue;
		}
		[Category(Categories.Data)]
		public MapItemSettings ClusterSettings {
			get { return (MapItemSettings)GetValue(ClusterSettingsProperty); }
			set { SetValue(ClusterSettingsProperty, value); }
		}
		[Category(Categories.Data)]
		public int StepInPixels {
			get { return (int)GetValue(StepInPixelsProperty); }
			set { SetValue(StepInPixelsProperty, value); }
		}
		[Category(Categories.Data)]
		public int ItemMaxSize {
			get { return (int)GetValue(ItemMaxSizeProperty); }
			set { SetValue(ItemMaxSizeProperty, value); }
		}
		[Category(Categories.Data)]
		public int ItemMinSize {
			get { return (int)GetValue(ItemMinSizeProperty); }
			set { SetValue(ItemMinSizeProperty, value); }
		}
		[Category(Categories.Data)]
		public MapClustererGroupProviderBase GroupProvider {
			get { return (MapClustererGroupProviderBase)GetValue(GroupProviderProperty); }
			set { SetValue(GroupProviderProperty, value); }
		}
		public event ClusteringEventHandler Clustering;
		public event ClusteredEventHandler Clustered;
		static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapClusterer clusterer = d as MapClusterer;
			if(clusterer != null)
				clusterer.Clusterize();
		}
		static void OnItemMaxSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapClusterer clusterer = d as MapClusterer;
			if(clusterer != null) {
				clusterer.ItemMaxSize = Math.Max((int)e.NewValue, clusterer.ItemMinSize);
				clusterer.Clusterize();
			}
		}
		static void OnItemMinSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapClusterer clusterer = d as MapClusterer;
			if(clusterer != null) {
				clusterer.ItemMinSize = Math.Max(Math.Min((int)e.NewValue, clusterer.ItemMaxSize), 0);
				clusterer.Clusterize();
			}
		}
		protected static void ClusterSettingsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapClusterer clusterer = d as MapClusterer;
			if(clusterer != null) {
				CommonUtils.SetOwnerForValues(e.OldValue, e.NewValue, clusterer);
				clusterer.Clusterize();
			}
		}
		const int DefaultStep = 50;
		const int DefaultItemMaxSize = 50;
		const int DefaultItemMinSize = 10;
		Dictionary<int, MapVectorItemCollection> ClusteringCache = new Dictionary<int, MapVectorItemCollection>();
		MapVectorItemCollection items;
		protected VectorLayerBase Layer { get { return Adapter != null ? Adapter.Layer : null; } }
		protected bool CanClustered { get { return Layer != null; } }
		public override MapVectorItemCollection Items {
			get { return items; }
		}
		#region IMapItemSettingsListener implementation
		void IMapItemSettingsListener.OnPropertyChanged(DependencyProperty property, object value) {
			Clusterize();
		}
		void IMapItemSettingsListener.OnSourceChanged() {
			Clusterize();
		}
		#endregion
		void NotifyDataChanged() {
			Adapter.OnClustered();
		}
		void ApplyResults(ClusterCalculatorResult result) {
			foreach(IClusterItem item in result.Items) {
				float delta = item.ClusteredItems.Count / (float)result.MaxItemsCountInGroup;
				double size = MathUtils.MinMax(ItemMaxSize * delta, ItemMinSize, ItemMaxSize);
				if(item.ClusteredItems.Count>0)
					item.ApplySize(size);
				if(GroupProvider != null)
					GroupProvider.OnClusterCreated(item as MapItem);
			}
		}
		void Clusterize() {
			if(CanClustered)
				Clusterize(Adapter.ItemsCollection, Layer.GetViewport(), true);
		}
		void SetItems(MapVectorItemCollection results) {
			items = results;
			RaiseClustered(results);
			NotifyDataChanged();
		}
		MapVectorItemCollection ConvertToItemsCollection(IList<IClusterItem> list) {
			MapVectorItemCollection collection = CreateItemsCollection();
			return collection;
		}
		MapVectorItemCollection CreateResult(IEnumerable<MapItem> items, IClusterCalculator calculator, double step) {
			MapVectorItemCollection resItems = CreateItemsCollection();
			if(GroupProvider == null)
				ProcessResult(resItems, calculator.CalculateGroups(items.OfType<IClusterable>(), step));
			else {
				Dictionary<object, IEnumerable<MapItem>> groups = GroupProvider.GetGroups(items);
				foreach(IList<MapItem> group in groups.Values) {
					ProcessResult(resItems, calculator.CalculateGroups(group.OfType<IClusterable>(), step));
				}
			}
			return resItems;
		}
		void ProcessResult(MapVectorItemCollection resItems, ClusterCalculatorResult result) {
			ApplyResults(result);
			CopyItems(result.Items, resItems);
		}
		void CopyItems(IList<IClusterItem> source, MapVectorItemCollection dest) {
			dest.BeginUpdate(Adapter);
			foreach(MapItem item in source)
				dest.Add(item);
			dest.EndUpdate(true);
		}
		protected virtual void RaiseClustered(MapVectorItemCollection result) {
			if(Clustered != null)
				Clustered(this, new ClusteredEventArgs(result));
		}
		protected virtual void RaiseClustering() {
			if(Clustering != null)
				Clustering(this, new EventArgs());
		}
		protected internal double CalculateStep() {
			if(CanClustered) {
				VectorLayerBase layer = Adapter.Layer;
				const double angleKoef = 0.707107;
				MapUnit p1 = layer.ScreenPointToMapUnitInternal(new Point(), false, false);
				MapUnit p2 = layer.ScreenPointToMapUnitInternal(new Point(StepInPixels * angleKoef, StepInPixels * angleKoef), false, false);
				return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
			}
			return 0;
		}
		protected override void OwnerChanged() {
			Clusterize();
		}
		protected abstract IClusterCalculator CreateClusterCalculator();
		protected internal void ClusterizeCore(MapVectorItemCollection sourceItems, int zoom) {
			RaiseClustering();
			IClusterCalculator calculator = CreateClusterCalculator();
			double step = CalculateStep();
			MapVectorItemCollection result = CreateResult(sourceItems, calculator, step);
			ClusteringCache[zoom] = result;
			SetItems(ClusteringCache[zoom]);
		}
		public override void Clusterize(MapVectorItemCollection sourceItems, MapViewport mapViewport, bool sourceChanged) {
			if(sourceItems.Count == 0)
				return;
			int zoom = (int)mapViewport.ZoomLevel;
			if(sourceChanged)
				ClusteringCache.Clear();
			if(ClusteringCache.ContainsKey(zoom))
				SetItems(ClusteringCache[zoom]);
			else
				ClusterizeCore(sourceItems, zoom);
		}
	}
	public class MarkerClusterer : MapClusterer {
		protected override IClusterCalculator CreateClusterCalculator() {
			return new MarkerClusterCalculator(new MapClusterFactoryAdapter(ClusterSettings));
		}
		protected override MapDependencyObject CreateObject() {
			return new MarkerClusterer();
		}
	}
	public class DistanceBasedClusterer : MapClusterer {
		protected override IClusterCalculator CreateClusterCalculator() {
			CoordObjectFactory pointFactory = Layer != null ? Layer.ActualCoordinateSystem.PointFactory : GeoPointFactory.Instance;
			return new DistanceBasedClusterCalculator(new MapClusterFactoryAdapter(ClusterSettings), pointFactory);
		}
		protected override MapDependencyObject CreateObject() {
			return new DistanceBasedClusterer();
		}
	}
}
