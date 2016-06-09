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
using DevExpress.Xpf.Map.Native;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
namespace DevExpress.Xpf.Map {
	public abstract class MapDataAdapterBase : MapDependencyObject, IOwnedElement, ISupportVirtualizationData {
		public static readonly DependencyProperty ClustererProperty = DependencyProperty.Register("Clusterer",
		   typeof(MapClustererBase), typeof(MapDataAdapterBase), new PropertyMetadata(null, ClustererPropertyChanged));
		[Category(Categories.Data)]
		public MapClustererBase Clusterer {
			get { return (MapClustererBase)GetValue(ClustererProperty); }
			set { SetValue(ClustererProperty, value); }
		}
		static void ClustererPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapDataAdapterBase adapter = d as MapDataAdapterBase;
			if(adapter != null && e.OldValue != e.NewValue) {
				CommonUtils.SetOwnerForValues(e.OldValue, e.NewValue, adapter);
				if(e.NewValue == null)
					adapter.OnClustered();
			}
		}
		object owner;
		protected virtual bool CanLoadData { get { return Layer != null && Layer.IsLoaded; } }
		protected internal VectorLayerBase Layer { get { return owner as VectorLayerBase; } }
		protected internal abstract MapVectorItemCollection ItemsCollection { get; }
		protected internal MapVectorItemCollection ActualItems { get { return Clusterer != null && Clusterer.Items != null ? Clusterer.Items : ItemsCollection; } }
		protected bool IsPreprocessingData { get { return Clusterer != null; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IEnumerable<MapItem> DisplayItems { get { return ItemsCollection; } }
		#region IOwnedElement implementation
		object IOwnedElement.Owner {
			get { return owner; }
			set {
				if (owner != value) {
					owner = value;
					OwnerChanged();
				}
			}
		}
		#endregion
		#region ISupportVirtualizationData implementation
		bool ISupportVirtualizationData.IsPreprocessingData {
			get { return IsPreprocessingData; }
		}
		#endregion
		void ClusterizeCore(MapViewport viewport, bool sourceChanged) {
			if(Clusterer != null)
				Clusterer.Clusterize(ItemsCollection, viewport, sourceChanged);
		}
		void Clusterize(bool sourceChanged) {
			if(Layer != null && Layer.View != null)
				ClusterizeCore(Layer.GetViewport(), sourceChanged);
		}
		protected virtual void OwnerChanged() {
			CommonUtils.SetItemOwner(ItemsCollection, Layer);
			LoadDataInternal();
		}
		protected abstract void LoadDataCore();
		protected internal abstract bool IsCSCompatibleTo(MapCoordinateSystem coordinateSystem);
		public abstract object GetItemSourceObject(MapItem item);
		public virtual void OnClustered() {
			if(Layer != null) {
				Layer.UpdateItemsSource(false);
				Layer.ColorizeItems();
				Layer.UpdateLegends();
			}
		}
		protected internal virtual void LoadDataInternal(bool publicCall) {
			if (CanLoadData || publicCall)
				LoadDataCore();
			if (CanLoadData)
				Clusterize(true);
		}
		protected internal void LoadDataInternal() {
			LoadDataInternal(false);
		}
		public void LoadData() {
			LoadDataInternal(true);
		}
		public void OnViewportUpdated(MapViewport viewport) {
			ClusterizeCore(viewport, false);
		}
	}
	public abstract class CoordinateSystemDataAdapterBase : MapDataAdapterBase {
		static protected internal SourceCoordinateSystem DefaultSourceCoordinateSystem { get { return new GeoSourceCoordinateSystem(); } }
		public static readonly DependencyProperty SourceCoordinateSystemProperty = DependencyProperty.Register("SourceCoordinateSystem",
			typeof(SourceCoordinateSystem), typeof(CoordinateSystemDataAdapterBase), new PropertyMetadata(DefaultSourceCoordinateSystem, CoordinateSystemChanged, CoerceCoordinateSystem));
		[Category(Categories.Data)]
		public SourceCoordinateSystem SourceCoordinateSystem {
			get { return (SourceCoordinateSystem)GetValue(SourceCoordinateSystemProperty); }
			set { SetValue(SourceCoordinateSystemProperty, value); }
		}
		static void CoordinateSystemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			CoordinateSystemDataAdapterBase dataAdapter = d as CoordinateSystemDataAdapterBase;
			if (dataAdapter != null)
				dataAdapter.UpdateCoordinateSystem();
		}
		static object CoerceCoordinateSystem(DependencyObject d, object baseValue) {
			if (baseValue == null)
				return DefaultSourceCoordinateSystem;
			return baseValue;
		}
		void UpdateCoordinateSystem() {
			if (Layer != null)
				Layer.CheckCompatibility();
			LoadDataInternal();
		}
		protected internal virtual SourceCoordinateSystem GetActualCoordinateSystem() {
			return SourceCoordinateSystem;
		}
		protected internal override bool IsCSCompatibleTo(MapCoordinateSystem coordinateSystem) {
			return coordinateSystem.PointType == SourceCoordinateSystem.GetSourcePointType();
		}
	}
}
