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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using DevExpress.Xpf.Map.Native;
namespace DevExpress.Xpf.Map.Native {
	public interface ISupportVirtualizationData {
		bool IsPreprocessingData { get; }
	}
	public class ClusteredObservableCollection<TClusterKey, TValue> : ICollection<TValue>, INotifyCollectionChanged, INotifyPropertyChanged where TClusterKey : class {
		class Enumerator : IEnumerator<TValue> {
			readonly List<Cluster> clusters;
			int clusterIndex;
			int listIndex;
			Cluster cluster;
			TValue currentValue;
			public Enumerator(List<Cluster> clusters) {
				this.clusters = clusters;
				Reset();
			}
			#region IEnumerator
			object IEnumerator.Current {
				get {
					return currentValue;
				}
			}
			bool IEnumerator.MoveNext() {
				if (clusters.Count == 0)
					return false;
				if ((clusterIndex == -1) || (listIndex >= (cluster.Values.Count - 1)))
					if (!NextCluster())
						return false;
				listIndex++;
				currentValue = cluster.Values[listIndex];
				return true;
			}
			void IEnumerator.Reset() {
				this.Reset();
			}
			#endregion
			#region IEnumerator<T>
			TValue IEnumerator<TValue>.Current {
				get {
					return currentValue;
				}
			}
			#endregion
			#region IDisposable
			void IDisposable.Dispose() { }
			#endregion
			bool NextCluster() {
				listIndex = -1;
				do {
					clusterIndex++;
					if (clusterIndex >= clusters.Count)
						return false;
					cluster = clusters.ElementAt(clusterIndex);
				} while (cluster.Values.Count == 0);
				return true;
			}
			void Reset() {
				this.clusterIndex = -1;
				this.listIndex = -1;
				this.cluster = null;
			}
		}
		public class Cluster {
			readonly TClusterKey key;
			readonly List<TValue> values;
			public TClusterKey Key {
				get {
					return key;
				}
			}
			public List<TValue> Values {
				get {
					return values;
				}
			}
			public Cluster(TClusterKey key) {
				this.key = key;
				this.values = new List<TValue>();
			}
			public override string ToString() {
				return "Cluster (Key = " + ((key == null) ? "null" : key.GetType().Name + ":" + key.GetHashCode()) + ", Count = " + Values.Count.ToString() + ")";
			}
		}
		const string CountProperty = "Count";
		const string IndexerProperty = "Item[]";
		NotifyCollectionChangedEventHandler collectionChanged;
		PropertyChangedEventHandler propertyChanged;
		TClusterKey defaultCluster;
		List<Cluster> clusters;
		public List<Cluster> Clusters {
			get {
				return clusters;
			}
		}
		public TClusterKey DefaultCluster {
			get {
				return defaultCluster;
			}
		}
		public int Count {
			get {
				int count = 0;
				foreach (var cluster in clusters)
					count += cluster.Values.Count;
				return count;
			}
		}
		public ClusteredObservableCollection(TClusterKey defaultCluster) {
			this.defaultCluster = defaultCluster;
			this.clusters = new List<Cluster>();
		}
		#region ICollection
		int ICollection<TValue>.Count {
			get {
				return this.Count;
			}
		}
		bool ICollection<TValue>.IsReadOnly {
			get {
				return false;
			}
		}
		void ICollection<TValue>.Add(TValue item) {
			this.AddItem(DefaultCluster, item);
		}
		void ICollection<TValue>.Clear() {
			this.Clear();
		}
		bool ICollection<TValue>.Contains(TValue item) {
			foreach (var cluster in clusters) {
				if (cluster.Values.Contains(item))
					return true;
			}
			return false;
		}
		bool ICollection<TValue>.Remove(TValue item) {
			return this.RemoveItem(item);
		}
		void ICollection<TValue>.CopyTo(TValue[] array, int arrayIndex) {
			for (int i = arrayIndex; i < array.Length; i++)
				AddItem(DefaultCluster, array[i]);
		}
		#endregion
		#region IEnumerable<T>
		IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() {
			return CreateEnumerator();
		}
		#endregion
		#region IEnumerable
		IEnumerator IEnumerable.GetEnumerator() {
			return CreateEnumerator();
		}
		#endregion
		#region INotifyCollectionChanged
		event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged {
			add {
				collectionChanged += value;
			}
			remove {
				collectionChanged -= value;
			}
		}
		#endregion
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
		void RaisePropertyChanged(string name) {
			if (propertyChanged != null)
				propertyChanged(this, new PropertyChangedEventArgs(name));
		}
		void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e) {
			if (collectionChanged != null)
				collectionChanged(this, e);
		}
		void RaiseCollectionChanged(NotifyCollectionChangedAction action, object item, int index) {
			RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));
		}
		void RaiseCollectionReset() {
			RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}
		Enumerator CreateEnumerator() {
			return new Enumerator(clusters);
		}
		int GetAbsoluteIndex(Cluster searchCluster) {
			int index = 0;
			foreach (var cluster in clusters) {
				if (cluster == searchCluster)
					break;
				index += cluster.Values.Count;
			}
			return index;
		}
		protected bool RemoveItem(Cluster cluster, TValue item) {
			int index = RemoveItemCore(cluster, item);
			if (index >= 0) {
				int absoluteIndex = GetAbsoluteIndex(cluster) + index;
				RaisePropertyChanged(CountProperty);
				RaisePropertyChanged(IndexerProperty);
				RaiseCollectionChanged(NotifyCollectionChangedAction.Remove, item, absoluteIndex);
				if (cluster.Values.Count == 0)
					clusters.Remove(cluster);
			}
			return false;
		}
		protected void AddItem(Cluster cluster, TValue item) {
			if (AddItemCore(cluster, item)) {
				int absoluteIndex = GetAbsoluteIndex(cluster);
				RaisePropertyChanged(CountProperty);
				RaisePropertyChanged(IndexerProperty);
				RaiseCollectionChanged(NotifyCollectionChangedAction.Add, item, absoluteIndex + cluster.Values.Count - 1);
			}
		}
		protected int RemoveItemCore(Cluster cluster, TValue item) {
			int index = cluster.Values.IndexOf(item);
			if (index >= 0) {
				cluster.Values.RemoveAt(index);
				return index;
			}
			return -1;
		}
		protected bool AddItemCore(Cluster cluster, TValue item) {
			cluster.Values.Add(item);
			return true;
		}
		protected virtual bool ClearCore() {
			clusters.Clear();
			return true;
		}
		public Cluster GetCluster(TClusterKey clusterKey) {
			Cluster foundedCluster = null;
			foreach (var cluster in clusters) {
				if (cluster.Key == clusterKey) {
					foundedCluster = cluster;
					break;
				}
			}
			if (foundedCluster == null) {
				foundedCluster = new Cluster(clusterKey);
				clusters.Add(foundedCluster);
			}
			return foundedCluster;
		}
		public void AddItem(TValue item) {
			AddItem(DefaultCluster, item);
		}
		public void AddItem(TClusterKey key, TValue item) {
			AddItem(GetCluster(key), item);
		}
		public void Clear() {
			if (ClearCore()) {
				RaisePropertyChanged(CountProperty);
				RaisePropertyChanged(IndexerProperty);
				RaiseCollectionReset();
			}
		}
		public bool RemoveItem(TValue item) {
			bool result = false;
			var readonlyClusters = clusters.ToArray();
			foreach (var cluster in readonlyClusters)
				result |= RemoveItem(cluster, item);
			return result;
		}
		public bool RemoveItem(TClusterKey key, TValue item) {
			return RemoveItem(GetCluster(key), item);
		}
	}
	public class MapItemPropertyChangedEventArgs : PropertyChangedEventArgs {
		readonly object oldValue;
		readonly object newValue;
		public object OldValue {
			get {
				return oldValue;
			}
		}
		public object NewValue {
			get {
				return newValue;
			}
		}
		public MapItemPropertyChangedEventArgs(string name, object oldValue, object newValue)
			: base(name) {
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
	}
	public class VirtualMapItemCollection : ClusteredObservableCollection<ControlTemplate, MapItemInfo> {
		List<MapItemInfo> indexer;
		public VirtualMapItemCollection()
			: base(null) {
			this.indexer = new List<MapItemInfo>();
		}
		ControlTemplate GetMapItemTemplate(MapItem item) {
			if (item != null)
				return item.ItemTemplate;
			return null;
		}
		MapItemInfo GetInfoInCluster(Cluster cluster, MapItem item) {
			foreach (var info in cluster.Values)
				if (info.MapItem == item)
					return info;
			return null;
		}
		void RemoveFromIndex(MapItemInfo item) {
			indexer.RemoveAt(item.IndexInCollection);
			for (int i = item.IndexInCollection; i < indexer.Count; i++)
				indexer[i].IndexInCollection--;
			MapItemInfo.UpdateCollectionLength(indexer.Count);
		}
		void AddToIndex(MapItemInfo item, int index) {
			item.IndexInCollection = index;
			indexer.Insert(index, item);
			for (int i = index + 1; i < indexer.Count; i++)
				indexer[i].IndexInCollection++;
			MapItemInfo.UpdateCollectionLength(indexer.Count);
		}
		void OnMapItemPropertyChanged(object sender, PropertyChangedEventArgs e) {
			MapItemPropertyChangedEventArgs eventArgs = e as MapItemPropertyChangedEventArgs;
			MapItem item = sender as MapItem;
			if ((e.PropertyName == "ItemTemplate") && (item != null) && (eventArgs != null)) {
				Cluster oldCluster = GetCluster((ControlTemplate)eventArgs.OldValue);
				MapItemInfo info = GetInfoInCluster(oldCluster, item);
				if (info != null) {
					info.MapItem = null;
					item.Info = null;
					UnsubscribeMapItem(item);
				}
				PushMapItem(item, info.IndexInCollection);
			}
		}
		void SubscribeMapItem(MapItem item) {
			if (item != null)
				item.PropertyChanged += OnMapItemPropertyChanged;
		}
		void UnsubscribeMapItem(MapItem item) {
			if (item != null)
				item.PropertyChanged -= OnMapItemPropertyChanged;
		}
		void RemoveItemInfo(MapItemInfo itemInfo, MapItem item) {
			itemInfo.MapItem = null;
			if (item != null) {
				item.Container = null;
				item.Info = null;
				UnsubscribeMapItem(item);
			}
		}
		public void PushMapItem(MapItem item) {
			PushMapItem(item, indexer.Count);
		}
		public void PushMapItem(MapItem item, int index) {
			Cluster cluster = GetCluster(GetMapItemTemplate(item));
			MapItemInfo freeInfo = null;
			foreach(var info in cluster.Values) {
				if(info.IsFree) {
					freeInfo = info;
					break;
				}
			}
			bool addToCollection = (freeInfo == null);
			if(addToCollection)
				freeInfo = item.CreateInfo();
			item.Info = freeInfo;
			freeInfo.MapItem = item;
			SubscribeMapItem(item);
			if (addToCollection)
				AddItem(cluster, freeInfo);
			else {
				item.Container = freeInfo.Container;
				if(item.Container != null)
					((MapItemPresenter)item.Container).Template = item.ItemTemplate;
			}
			AddToIndex(freeInfo, index);
		}
		public void RemoveMapItem(MapItem item) {
			Cluster cluster = GetCluster(GetMapItemTemplate(item));
			MapItemInfo foundedInfo = GetInfoInCluster(cluster, item);
			if (foundedInfo != null) {
				RemoveItemInfo(foundedInfo, item);
				RemoveFromIndex(foundedInfo);
			}
		}
		public void ClearClusters() {
			ClearCore();
			Clusters.Clear();
		}
		protected override bool ClearCore() {
			foreach (Cluster cluster in Clusters) {
				foreach (MapItemInfo info in cluster.Values) {
					RemoveItemInfo(info, info.MapItem);
				}
			}
			indexer.Clear();
			return false;
		}
	}
}
