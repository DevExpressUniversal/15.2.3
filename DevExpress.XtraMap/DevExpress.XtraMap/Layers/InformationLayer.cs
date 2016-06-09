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
using DevExpress.Utils;
using DevExpress.XtraMap.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Diagnostics.CodeAnalysis;
namespace DevExpress.XtraMap {
	public class LayerMouseEventArgs : EventArgs {
		GeoPoint geoPoint;
		MapPoint screenPoint;
		public GeoPoint GeoPoint {
			get { return geoPoint; }
			set { this.geoPoint = value; }
		}
		public MapPoint ScreenPoint {
			get { return screenPoint; }
			set { this.screenPoint = value; }
		}
		public LayerMouseEventArgs(MapPoint screenPoint, GeoPoint geoPoint) {
			this.screenPoint = screenPoint;
			this.geoPoint = geoPoint;
		}
	}
	public class InformationLayer : MapItemsLayerBase, IMapClickHandler {
		InformationDataProviderBase dataProvider;
		Queue<RequestResultItem> resultsQueue;
		readonly object disposeLocker = new object();
		protected override int DefaultZIndex { get { return 50; } }
		protected internal override CoordBounds BoundingRect { get { return CoordBounds.Empty; } }
		protected internal new MapItemCollection DataItems { get { return (MapItemCollection)Data.Items; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("InformationLayerData"),
#endif
		Category(SRCategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TypeConverter("DevExpress.XtraMap.Design.ExpandableNoneStringSupportedTypeConverter," + AssemblyInfo.SRAssemblyMapDesign),
		RefreshProperties(RefreshProperties.All)
		]
		public MapItemStorage Data { get { return (MapItemStorage)DataAdapter; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("InformationLayerDataProvider"),
#endif
		DefaultValue(null), Category(SRCategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		TypeConverter("DevExpress.XtraMap.Design.ExpandableNoneStringSupportedTypeConverter," + AssemblyInfo.SRAssemblyMapDesign),
		Editor("DevExpress.XtraMap.Design.InformationDataProviderPickerEditor," + AssemblyInfo.SRAssemblyMapDesign, typeof(UITypeEditor)),
		RefreshProperties(RefreshProperties.All)
		]
		public InformationDataProviderBase DataProvider {
			get { return dataProvider; }
			set {
				if (Object.Equals(dataProvider, value))
					return;
				OnDataProviderChanging();
				this.dataProvider = value;
				OnDataProviderChanged();
				RaiseChanged();
			}
		}
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value"), SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value"), 
		Browsable(false), 
		EditorBrowsable(EditorBrowsableState.Never)]
		public new event DataLoadedEventHandler DataLoaded {  add { } remove { } }
		protected virtual void OnDataProviderChanging() {
			if(dataProvider != null) {
				UnsubscribeDataProviderEvents();
				((IOwnedElement)dataProvider).Owner = null;
			}
		}
		protected virtual void OnDataProviderChanged() {
			if(dataProvider != null) {
				((IOwnedElement)dataProvider).Owner = this;
				SubscribeDataProviderEvents();
			}
		}
#if !SL
	[DevExpressXtraMapLocalizedDescription("InformationLayerDataRequestCompleted")]
#endif
		public event EventHandler<RequestCompletedEventArgs> DataRequestCompleted;
		public InformationLayer() {
			resultsQueue = new Queue<RequestResultItem>();
		}
		protected override IMapDataAdapter CreateDataAdapter() {
			return new MapItemStorage();
		}
		void SubscribeDataProviderEvents() {
			if(dataProvider != null) {
				dataProvider.RequestCompleted += OnRequestCompleted;
				ISupportObjectChanged supportChanged = dataProvider as ISupportObjectChanged;
				if(supportChanged != null) supportChanged.Changed += OnProviderChangedEvents;
			}
		}
		void UnsubscribeDataProviderEvents() {
			if(dataProvider != null) {
				ISupportObjectChanged supportChanged = dataProvider as ISupportObjectChanged;
				if(supportChanged != null) supportChanged.Changed -= OnProviderChangedEvents;
				dataProvider.RequestCompleted -= OnRequestCompleted;
			}
		}
		void OnProviderChangedEvents(object sender, EventArgs e) {
			RaiseChanged();
		}
		void IMapClickHandler.OnPointClick(MapPoint point, CoordPoint coordPoint) {
			IMapClickHandler handler = DataProvider as IMapClickHandler;
			if(handler != null) {
				CoordPoint pt = UnitConverter.ScreenPointToCoordPoint(point);
				handler.OnPointClick(point, pt);
			}
		}
		protected override void DisposeOverride() {
			lock(disposeLocker) {
				if(dataProvider != null) {
					UnsubscribeDataProviderEvents();
					MapUtils.SetOwner(dataProvider, null);
					dataProvider = null;
				}
				if(resultsQueue != null) {
					resultsQueue.Clear();
					resultsQueue = null;
				}
				base.DisposeOverride();  
			}
		}
		protected override bool IsReadyForRender { get { return true; }  }
		void EnqueueRequestResultItems(MapItem[] items) {
			if(DataProvider.MaxVisibleResultCountInternal > 0) {
				resultsQueue.Enqueue(new RequestResultItem(items));
				if(resultsQueue.Count > DataProvider.MaxVisibleResultCountInternal)
					RemoveResultItems(resultsQueue.Dequeue());
			}
		}
		void RemoveResultItems(RequestResultItem resultItem) {
			DataItems.BeginUpdate();
			foreach(MapItem item in resultItem.Items)
				RemoveActualItem(item);
			DataItems.EndUpdate();
		} 
		void RaiseSeachRequestCompleted(RequestCompletedEventArgs e) {
			if(DataRequestCompleted != null)
				DataRequestCompleted(this, e);
		}
		internal void PlaceItem(MapItem item) {
			if((item != null) && !DataItems.Contains(item))
				DataItems.Add(item);
		}
		internal void RemoveActualItem(MapItem item) {
			if((item != null) && DataItems.Contains(item))
				DataItems.Remove(item);
		}
		void OnRequestCompleted(object sender, RequestCompletedEventArgs e) {
			lock(disposeLocker) {
				if(IsDisposed)
					return;
				if(!e.Cancelled && (e.Error == null)) {
					EnqueueRequestResultItems(e.Items);
					DataItems.AddRange(e.Items);
				}
				InvalidateRender();
				RaiseSeachRequestCompleted(e);
			}
		}
		protected internal bool StartSearch(string searchString) {
			if(string.IsNullOrEmpty(searchString))
				return false;
			BingSearchDataProvider searchProvider = DataProvider as BingSearchDataProvider;
			if(searchProvider != null && searchProvider.ShowSearchPanel) {
				searchProvider.Cancel();
				searchProvider.SearchByString(searchString);
				return true;
			}
			return false;
		}
		protected internal void VerifyResultCount() {
			int removeCount = resultsQueue.Count - DataProvider.MaxVisibleResultCountInternal;
			while(removeCount > 0) {
				RemoveResultItems(resultsQueue.Dequeue());
				removeCount--;
			}
		}
		public void ClearResults() {
			int resultsCount = resultsQueue.Count;
			for(int i = 0; i < resultsCount; i++)
				RemoveResultItems(resultsQueue.Dequeue());
		}
		public override string ToString() {
			return "(InformationLayer)";
		}
	}
}
namespace DevExpress.XtraMap.Native {
	public class RequestResultItem {
		readonly MapItem[] items;
		public MapItem[] Items { get { return this.items; } }
		public RequestResultItem(MapItem[] items) {
			this.items = (MapItem[])items.Clone();
		}
	}
}
