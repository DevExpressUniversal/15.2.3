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
using DevExpress.XtraMap.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Threading;
namespace DevExpress.XtraMap {
	public abstract class MapClustererBase : IClusterer, IDisposable {
		const int DefaultStep = 50;
		const int DefaultItemMaxSize = 50;
		const int DefaultItemMinSize = 10;
		readonly object locker = new object();
		int itemMaxSize = DefaultItemMaxSize;
		int itemMinSize = DefaultItemMinSize;
		int stepInPixels = DefaultStep;
		MapItemCollection items;
		Dictionary<int, MapItemCollection> ClusteringCache = new Dictionary<int, MapItemCollection>();
		IClustererGroupProvider groupProvider;
		IClusterItemFactory clusterItemFactory;
		Thread ClusteringThread = null;
		ClustererThreadParam CurrentState = null;
		bool isBusy = false;
		bool disposed = false;
		bool ignoreThreadResult = false;
		IClusterItemFactory ClusterItemFactory {
			get { return clusterItemFactory; }
			set {
				if (clusterItemFactory == value) return;
				clusterItemFactory = value;
				Clusterize();
			}
		}
		protected internal virtual bool IsBusy { get { return isBusy; } set { isBusy = value; } }
		protected IClusterItemFactory ActualClusterItemFactory { get { return ClusterItemFactory != null ? ClusterItemFactory : DefaultClusterItemFactory.Instance; } }
		protected MapItemsLayerBase Layer { get { return Adapter != null ? Adapter.GetLayer() : null; } }
		protected IMapDataAdapter Adapter { get; set; }
		protected MapItemCollection Items {
			get { return items; }
			set {
				items = value;
				NotifyDataChanged();
			}
		}
		protected bool CanClusterize { get { return Layer != null && Layer.View != null; } }
		protected internal class ClustererThreadParam {
			public IEnumerable<MapItem> SourceItems { get; set; }
			public int Zoom { get; set; }
		}
		[Category(SRCategoryNames.Data), DefaultValue(DefaultStep)]
		public int StepInPixels {
			get { return stepInPixels; }
			set {
				if(stepInPixels == value)
					return;
				stepInPixels = value;
				OnPropertyChanged();
			}
		}
		[Category(SRCategoryNames.Data), DefaultValue(null),
		TypeConverter("DevExpress.XtraMap.Design.ExpandableNoneStringSupportedTypeConverter," + AssemblyInfo.SRAssemblyMapDesign),
		Editor("DevExpress.XtraMap.Design.MapClustererGroupProviderEditor," + AssemblyInfo.SRAssemblyMapDesign, typeof(UITypeEditor)),
		RefreshProperties(RefreshProperties.All)]
		public IClustererGroupProvider GroupProvider {
			get { return groupProvider; }
			set {
				if (groupProvider == value)
					return;
				if (groupProvider != null)
					groupProvider.Changed -= OnGroupProviderChanged;
				groupProvider = value;
				if (groupProvider != null)
					groupProvider.Changed += OnGroupProviderChanged;
				OnPropertyChanged();
			}
		}
		[Category(SRCategoryNames.Layout), DefaultValue(DefaultItemMinSize)]
		public int ItemMinSize {
			get { return itemMinSize; }
			set {
				int newMinSize = ValidateMinSize(value);
				if(newMinSize == itemMinSize)
					return;
				itemMinSize = newMinSize;
				OnPropertyChanged();
			}
		}
		[Category(SRCategoryNames.Layout), DefaultValue(DefaultItemMaxSize)]
		public int ItemMaxSize {
			get { return itemMaxSize; }
			set {
				int newMaxSize = ValidateMaxSize(value);
				if(newMaxSize == itemMaxSize)
					return;
				itemMaxSize = newMaxSize;
				OnPropertyChanged();
			}
		}
		public event ClusteringEventHandler Clustering;
		public event ClusteredEventHandler Clustered;
		#region IClusterer
		MapItemCollection IClusterer.Items {
			get { return Items; }
		}
		void IClusterer.SetOwner(IMapDataAdapter owner) {
			if (Adapter == owner)
				return;
			Adapter = owner;
			Clusterize();
		}
		void IClusterer.Clusterize(IEnumerable<MapItem> sourceItems, MapViewport viewport, bool sourceChanged) {
			Clusterize(sourceItems, viewport, sourceChanged);
		}
		bool IClusterer.IsBusy { get { return IsBusy; } }
		#endregion      
		~MapClustererBase() {
			Dispose(false);
		}
		void NotifyDataChanged() {
			if(Adapter != null)
				Adapter.OnClustered();
		}
		void OnGroupProviderChanged(object sender, EventArgs e) {
			OnPropertyChanged();
		}
		void OnPropertyChanged() {
			Clusterize();
		}
		void Clusterize() {
			if(CanClusterize)
				Clusterize(Adapter.SourceItems, Layer.GetViewport(), true);
		}
		int ValidateMinSize(int value) {
			return Math.Max(Math.Min(value, ItemMaxSize), 0);
		}
		int ValidateMaxSize(int value) {
			return Math.Max(value, ItemMinSize);
		}
		MapItemCollection CreateClusteringResult(IEnumerable<MapItem> items, IClusterCalculator calculator, double step) {
			MapItemCollection results = new MapItemCollection(Adapter);
			if (GroupProvider == null)
				ProcessResult(results, calculator.CalculateGroups(items.OfType<IClusterable>(), step));
			else {
				Dictionary<object, IEnumerable<MapItem>> groups = GroupProvider.GetGroups(items);
				foreach (IList<MapItem> group in groups.Values)
					ProcessResult(results, calculator.CalculateGroups(group.OfType<IClusterable>(), step));
			}
			return results;
		}
		void ProcessResult(MapItemCollection items, ClusterCalculatorResult result) {
			OnResultProcessing(result);
			items.AddRange(result.Items.OfType<MapItem>().ToList());
		}
		void OnResultProcessing(ClusterCalculatorResult result) {
			foreach (IClusterItem item in result.Items) {
				float delta = item.ClusteredItems.Count / (float)result.MaxItemsCountInGroup;
				double size = MathUtils.MinMax(ItemMaxSize * delta, ItemMinSize, ItemMaxSize);
				if(item.ClusteredItems.Count > 0)
					item.ApplySize(size);
				if (GroupProvider != null)
					GroupProvider.OnClusterCreated(item as MapItem);
			}
		}
		void Clusterize(IEnumerable<MapItem> sourceItems, MapViewport viewport, bool sourceChanged) {
			int zoom = (int)viewport.ZoomLevel;
			this.ignoreThreadResult = false;
			if (sourceChanged)
				ClusteringCache.Clear();
			RaiseClusteringEvent();
			if(ClusteringCache.ContainsKey(zoom)) {
				this.ignoreThreadResult = true;
				MapItemCollection result = ClusteringCache[zoom];
				RaiseClusteredEvent(result);
				Items = result;  
			}
			else
				StartClusterizeThread(sourceItems, zoom);
		}
		protected virtual void RaiseClusteredEvent(IList<MapItem> items) {
			if(Clustered != null)
				RaiseEventAsync(()=> Clustered(this, new ClusteredEventArgs(items)));
		}
		protected virtual void RaiseClusteringEvent() {
			if(Clustering != null)
				RaiseEventAsync(()=> Clustering(this, new EventArgs()));
		}
		protected virtual void RaiseEventAsync(Action action) {
			LayerBase layer = Layer;
			if(layer != null && layer.Map != null)
			MapUtils.BeginInvokeAction(layer.Map.ExternalInvoker, action);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposed) return;
			lock (locker) {
				disposed = true;
			}
		}
		protected internal virtual void StartClusterizeThread(IEnumerable<MapItem> sourceItems, int zoom) {
			CurrentState = new ClustererThreadParam() { SourceItems = sourceItems, Zoom = zoom };
			if (ClusteringThread == null || ClusteringThread.ThreadState != ThreadState.Running) {
				ClusteringThread = new Thread(DoClusterize);
				ClusteringThread.Start(CurrentState);
			}
		}
		protected internal void DoClusterize(object state) {
			ClustererThreadParam param = state as ClustererThreadParam;
			if(param == null) return;
			try {
				lock (locker) {
					IsBusy = true;
					IClusterCalculator calculator = CreateClusterCalculator();
					double step = CalculateStep();
					MapItemCollection result = CreateClusteringResult(param.SourceItems, calculator, step);
					ClusteringCache[param.Zoom] = result;
					if(!this.ignoreThreadResult) {
						RaiseClusteredEvent(result);
						Items = result;
					}
				}
				if (param != CurrentState)
					DoClusterize(CurrentState);
			}
			catch {
			}
			finally {
				IsBusy = false;
			}
		}
		protected internal virtual double CalculateStep() {
			MapUnitConverter converter = Layer != null ? Layer.UnitConverter : EmptyUnitConverter.Instance;;
			const double angleKoef = 0.707107;
			MapUnit p1 = converter.ScreenPointToMapUnit(new MapPoint(), false);
			MapUnit p2 = converter.ScreenPointToMapUnit(new MapPoint(StepInPixels * angleKoef, StepInPixels * angleKoef), false);
			return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
		}
		protected abstract IClusterCalculator CreateClusterCalculator();
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		public void SetClusterItemFactory(IClusterItemFactory clusterItemFactory) {
			ClusterItemFactory = clusterItemFactory;
		}
	} 
	public class MarkerClusterer : MapClustererBase {
		protected override IClusterCalculator CreateClusterCalculator() {
			return new MarkerClusterCalculator(new MapClusterFactoryAdapter(ActualClusterItemFactory));
		}
		public override string ToString() {
			return "(MarkerClusterer)";
		}
	}
	public class DistanceBasedClusterer : MapClustererBase {
		protected override IClusterCalculator CreateClusterCalculator() {
			CoordObjectFactory pointFactory = Layer != null ? Layer.UnitConverter.PointFactory : GeoPointFactory.Instance;
			return new DistanceBasedClusterCalculator(new MapClusterFactoryAdapter(ActualClusterItemFactory), pointFactory);
		}
		public override string ToString() {
			return "(DistanceBasedClusterer)";
		}
	}
}
