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
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Map.Native {
	public class RequestResultItem {
		readonly MapItem[] items;
		public MapItem[] Items { get { return this.items; } }
		public RequestResultItem(MapItem[] items) {
			this.items = (MapItem[])items.Clone();
		}
	}
}
namespace DevExpress.Xpf.Map {
	[NonCategorized]
	public class LayerMouseEventArgs : EventArgs {
		GeoPoint geoPoint;
		Point screenPoint;
		public GeoPoint GeoPoint {
			get { return geoPoint; }
			set { this.geoPoint = value; }
		}
		public Point ScreenPoint {
			get { return screenPoint; }
			set { this.screenPoint = value; }
		}
		public LayerMouseEventArgs(Point screenPoint, GeoPoint geoPoint) {
			this.screenPoint = screenPoint;
			this.geoPoint = geoPoint;
		}
	}
	[ContentProperty("DataProvider")]
	public class InformationLayer : VectorLayerBase {
		public static readonly DependencyProperty DataProviderProperty = DependencyPropertyManager.Register("DataProvider",
			typeof(InformationDataProviderBase), typeof(InformationLayer), new PropertyMetadata(null, DataProviderPropertyChanged));
		[Category(Categories.Data)]
		public InformationDataProviderBase DataProvider {
			get { return (InformationDataProviderBase)GetValue(DataProviderProperty); }
			set { SetValue(DataProviderProperty, value); }
		}
		Queue<RequestResultItem> resultsQueue;
		MapSearchPanel searchPanel;
		readonly MapItemStorage dataAdapter;
		internal event EventHandler<LayerMouseEventArgs> MouseLeftClick;
		protected override MapDataAdapterBase DataAdapter { get { return dataAdapter; } }
		static void DataProviderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			InformationLayer layer = d as InformationLayer;
			if (layer != null) {
				layer.DataProviderChanged(e.NewValue as InformationDataProviderBase, e.OldValue as InformationDataProviderBase);
			}
		}
		public InformationLayer() {
			resultsQueue = new Queue<RequestResultItem>();
			dataAdapter = new MapItemStorage();
			DefaultStyleKey = typeof(InformationLayer);
			CommonUtils.SetItemOwner(dataAdapter, this);
		}
		void DataProviderChanged(InformationDataProviderBase provider, InformationDataProviderBase oldProvider) {
			if (oldProvider != provider) {
				if (oldProvider != null) {
					if (oldProvider.IsBusy)
						oldProvider.Cancel();
					oldProvider.RequestCompleted -= RequestCompleted;
					((IOwnedElement)oldProvider).Owner = null;
				}
				if (provider != null) {
					provider.RequestCompleted += RequestCompleted;
					((IOwnedElement)provider).Owner = this;
				}
				UpdateSearchPanelVisibility();
			}
		}
		void RequestCompleted(object sender, RequestCompletedEventArgs e) {
			if (!e.Cancelled && (e.Error == null)) {
				EnqueueRequestResultItems(e.Items);
				foreach (MapItem item in e.Items)
					PlaceItem(item);
			}
			UpdateControls(e);
		}
		void EnqueueRequestResultItems(MapItem[] items) {
			if (DataProvider.MaxVisibleResultCountInternal > 0) {
				resultsQueue.Enqueue(new RequestResultItem(items));
				if (resultsQueue.Count > DataProvider.MaxVisibleResultCountInternal)
					RemoveResultItems(resultsQueue.Dequeue());
			}
		}
		void RemoveResultItems(RequestResultItem resultItem) {
			foreach (MapItem item in resultItem.Items)
				RemoveItem(item);
		}
		void RaiseMouseLeftClick(Point point) {
			if (MouseLeftClick != null) {
				LayerMouseEventArgs args = new LayerMouseEventArgs(point, ScreenToGeoPoint(point));
				MouseLeftClick(this, args);
			}
		}
		void SearchPanel_SearchStringChanged(object sender, EventArgs e) {
			BingSearchDataProvider searchProvider = DataProvider as BingSearchDataProvider;
			if (searchProvider != null && searchProvider.ShowSearchPanel && searchPanel != null && !String.IsNullOrEmpty(searchPanel.SearchString)) {
				if (searchProvider.IsBusy)
					searchProvider.Cancel();
				searchProvider.SearchFromSearchPanel(searchPanel.SearchString);
				searchPanel.IsBusy = true;
				searchPanel.ShowAlternateResultsButton(false);
			}
		}
		void SearchPanel_SelectedResultChanged(object sender, SelectionChangedEventArgs e) {
			BingSearchDataProvider searchProvider = DataProvider as BingSearchDataProvider;
			if (searchProvider != null && searchProvider.ShowSearchPanel && searchPanel != null)
				foreach (MapItem item in DataItems) {
					MapPushpin pushpin = item as MapPushpin;
					if (pushpin != null) {
						LocationInformation locationInfo = pushpin.Information as LocationInformation;
						if (Map != null && locationInfo != null && locationInfo.Address != null && locationInfo.Address.FormattedAddress == searchPanel.SelectedResult)
							Map.SetCenterPoint(pushpin.Location);
					}
				}
		}
		void SearchPanel_DropDownButtonClick(object sender, RoutedEventArgs e) {
			BingSearchDataProvider searchProvider = DataProvider as BingSearchDataProvider;
			if (searchPanel != null && searchProvider != null)
				if (searchPanel.AreAlternateRequestsListed)
					searchProvider.GenerateSearchResults();
				else
					searchProvider.GenerateAlternateSearchRequests();
		}
		void UpdateControls(RequestCompletedEventArgs e) {
			BingSearchDataProvider searchProvider = DataProvider as BingSearchDataProvider;
			if (searchPanel != null && searchProvider != null) {
				searchPanel.SearchResults.Clear();
				List<string> addresses = e.UserState as List<string>;
				if (!e.Cancelled && e.Error == null && addresses != null)
					foreach (string address in addresses)
						searchPanel.SearchResults.Add(address);
				searchPanel.IsBusy = false;
				if (searchProvider != null)
					searchPanel.ShowAlternateResultsButton(searchProvider.HasAlternateResults);
			}
		}
		protected override void OnLeftMouseClick(Point point) {
			RaiseMouseLeftClick(point);
		}
		internal void UpdateSearchPanelVisibility() {
			if (searchPanel != null) {
				BingSearchDataProvider searchProvider = DataProvider as BingSearchDataProvider;
				if (searchProvider != null && searchProvider.ShowSearchPanel)
					searchPanel.Visibility = Visibility.Visible;
				else {
					searchPanel.Reset();
					searchPanel.Visibility = Visibility.Collapsed;
				}
			}
		}
		internal void PlaceItem(MapItem item) {
			if (item != null)
				dataAdapter.Items.Add(item);
		}
		internal void RemoveItem(MapItem item) {
			if (item != null)
				dataAdapter.Items.Remove(item);
		}
		internal void VerifyResultCount() {
			int removeCount = resultsQueue.Count - DataProvider.MaxVisibleResultCountInternal;
			while (removeCount > 0) {
				RemoveResultItems(resultsQueue.Dequeue());
				removeCount--;
			}
		}
		public void ClearResults() {
			int resultsCount = resultsQueue.Count;
			for (int i = 0; i < resultsCount; i++)
				RemoveResultItems(resultsQueue.Dequeue());
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			searchPanel = LayoutHelper.FindElementByName(this, "PART_SearchPanel") as MapSearchPanel;
			if (searchPanel != null) {
				searchPanel.SearchStringChanged += SearchPanel_SearchStringChanged;
				searchPanel.SelectedResultChanged += new SelectionChangedEventHandler(SearchPanel_SelectedResultChanged);
				searchPanel.DropDownButtonClick += new RoutedEventHandler(SearchPanel_DropDownButtonClick);
			}
			UpdateSearchPanelVisibility();
		}
	}
}
