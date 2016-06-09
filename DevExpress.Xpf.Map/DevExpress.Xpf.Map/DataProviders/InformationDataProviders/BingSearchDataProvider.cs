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

#define UseSoap
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.ServiceModel;
using System.Windows;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
#if UseSoap
using DevExpress.Map.BingServices;
#endif
using DevExpress.Map.Native;
using System.Reflection;
namespace DevExpress.Xpf.Map {
	public class BingSearchOptions : MapDependencyObject {
		public static readonly DependencyProperty AutocorrectQueryProperty = DependencyPropertyManager.Register("AutocorrectQuery",
			typeof(bool), typeof(BingSearchOptions), new PropertyMetadata(true));
		public static readonly DependencyProperty ResultsCountProperty = DependencyPropertyManager.Register("ResultsCount",
			typeof(int), typeof(BingSearchOptions), new PropertyMetadata(10));
		public static readonly DependencyProperty SearchRadiusProperty = DependencyPropertyManager.Register("SearchRadius",
			typeof(double), typeof(BingSearchOptions), new PropertyMetadata(100.0));
		public static readonly DependencyProperty DistanceUnitProperty = DependencyPropertyManager.Register("DistanceUnit",
			typeof(DistanceMeasureUnit), typeof(BingSearchOptions), new PropertyMetadata(DistanceMeasureUnit.Kilometer));
		[Category(Categories.Behavior)]
		public bool AutocorrectQuery {
			get { return (bool)GetValue(AutocorrectQueryProperty); }
			set { SetValue(AutocorrectQueryProperty, value); }
		}
		[Category(Categories.Behavior)]
		public int ResultsCount {
			get { return (int)GetValue(ResultsCountProperty); }
			set { SetValue(ResultsCountProperty, value); }
		}
		[Category(Categories.Behavior)]
		public double SearchRadius {
			get { return (double)GetValue(SearchRadiusProperty); }
			set { SetValue(SearchRadiusProperty, value); }
		}
		[Category(Categories.Behavior)]
		public DistanceMeasureUnit DistanceUnit {
			get { return (DistanceMeasureUnit)GetValue(DistanceUnitProperty); }
			set { SetValue(DistanceUnitProperty, value); }
		}
		protected override MapDependencyObject CreateObject() {
			return new BingSearchOptions();
		}
	}
	public delegate void BingSearchCompletedEventHandler(object sender, BingSearchCompletedEventArgs e);
	public class BingSearchCompletedEventArgs : AsyncCompletedEventArgs {
		readonly SearchRequestResult requestResult;
		public SearchRequestResult RequestResult { get { return requestResult; } }
		public BingSearchCompletedEventArgs(SearchRequestResult requestResult, Exception error, bool cancelled, object userState)
			: base(error, cancelled, userState) {
				this.requestResult = requestResult;
		}
	}
	public class BingSearchDataProvider : InformationDataProviderBase {
#if !UseSoap
		const string RESTuri = "http://dev.virtualearth.net/REST/v1/Locations";
#endif
		public static readonly DependencyProperty SearchOptionsProperty = DependencyPropertyManager.Register("SearchOptions",
			typeof(BingSearchOptions), typeof(BingSearchDataProvider), new PropertyMetadata());
		public static readonly DependencyProperty BingKeyProperty = DependencyPropertyManager.Register("BingKey",
			typeof(string), typeof(BingSearchDataProvider), new PropertyMetadata(string.Empty, BingKeyChanged));
		public static readonly DependencyProperty ShowSearchPanelProperty = DependencyPropertyManager.Register("ShowSearchPanel",
			typeof(bool), typeof(BingSearchDataProvider), new PropertyMetadata(true, UseSearchPanelPropertyChanged));
		bool isKeyRestricted = false;
		[Category(Categories.Behavior)]
		public BingSearchOptions SearchOptions {
			get { return (BingSearchOptions)GetValue(SearchOptionsProperty); }
			set { SetValue(SearchOptionsProperty, value); }
		}
		[Category(Categories.Behavior)]
		public string BingKey {
			get { return (string)GetValue(BingKeyProperty); }
			set { SetValue(BingKeyProperty, value); }
		}
		[Category(Categories.Behavior)]
		public bool ShowSearchPanel {
			get { return (bool)GetValue(ShowSearchPanelProperty); }
			set { SetValue(ShowSearchPanelProperty, value); }
		}
		BingSearchOptions defaultSearchOptions = new BingSearchOptions();
#if UseSoap
		SearchServiceClient searchService;
#else
		RESTClient searchClient;
#endif
		bool isBusy;
		GeoPoint searchAnchorPoint = new GeoPoint();
		int startingIndex;
		bool generateResultsForSearchPanel;
		SearchRequestResult lastRequestResult;
		bool hasAlternateResuts;
		public event BingSearchCompletedEventHandler SearchCompleted;
		static void BingKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			BingSearchDataProvider provider = d as BingSearchDataProvider;
			if(provider != null) {
				Assembly asm = Assembly.GetEntryAssembly();
				provider.isKeyRestricted = DXBingKeyVerifier.IsKeyRestricted(provider.BingKey, asm != null ? asm.FullName : string.Empty);
			}
		}
		static void UseSearchPanelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			BingSearchDataProvider provider = d as BingSearchDataProvider;
			if (provider != null && provider.Layer != null)
				provider.Layer.UpdateSearchPanelVisibility();
		}
		BingSearchOptions ActualSearchOptions {
			get { return SearchOptions != null ? SearchOptions : defaultSearchOptions; }
		}
		string ActualBingKey { get { return isKeyRestricted ? string.Empty : BingKey; } }
		internal bool HasAlternateResults { get { return hasAlternateResuts; } }
		protected internal override int MaxVisibleResultCountInternal { get { return 1; } }
		public override bool IsBusy { get { return isBusy; } }
		static BingSearchDataProvider() {
			DXBingKeyVerifier.RegisterPlatform("Xpf");
		}
		public BingSearchDataProvider() {
			ResetSearchService();
		}
		void ResetSearchService() {
#if UseSoap
			if (searchService != null) {
				searchService.SearchCompleted -= OnSearchCompleted;
				searchService.SearchCompleted -= OnAlternateSearchCompleted;
				searchAnchorPoint = new GeoPoint();
				startingIndex = 0;
			}
			BasicHttpSecurityMode mode = WebServiceHelper.SecurityMode;
			BasicHttpBinding binding = new BasicHttpBinding(mode);
			binding.MaxBufferSize = 2147483647;
			binding.MaxReceivedMessageSize = 2147483647L;
			binding.OpenTimeout = new TimeSpan(0, 0, 20);
			binding.CloseTimeout = new TimeSpan(0, 0, 20);
			binding.ReceiveTimeout = new TimeSpan(0, 0, 20);
			binding.SendTimeout = new TimeSpan(0, 0, 20);
			EndpointAddress address = new EndpointAddress(WebServiceHelper.CurrentScheme + "://dev.virtualearth.net/webservices/v1/searchservice/searchservice.svc");
			searchService = new SearchServiceClient(binding, address);
			searchService.SearchCompleted += OnSearchCompleted;
			hasAlternateResuts = false;
#else
			if(searchClient != null) {
				UnsubscribeEvents();
				searchAnchorPoint = new GeoPoint();
				startingIndex = 0;
			}
			this.searchClient = new RESTClient(RESTuri);
			SubscribeEvents();
			hasAlternateResuts = false;
#endif
		}
#if !UseSoap
		void UnsubscribeEvents() {
		}
		void SubscribeEvents() {
		}
#endif
		void SearchInternal(string keyword, string location, int startingIndex, bool parseOnly, string culture) {
			lastRequestResult = null;
			hasAlternateResuts = false;
#if UseSoap
			SearchRequest request = new SearchRequest();
			request.Credentials = new Credentials();
			request.Credentials.ApplicationId = ActualBingKey;
			request.Culture = string.IsNullOrEmpty(culture) ? CultureInfo.InvariantCulture.Name : CultureName;
			request.ExecutionOptions = new ExecutionOptions() { SuppressFaults = true };
			if (String.IsNullOrEmpty(location))
				request.Query = keyword;
			else
				request.StructuredQuery = new StructuredSearchQuery() { Keyword = keyword, Location = location };
			SearchOptions options = new SearchOptions();
			options.AutocorrectQuery = ActualSearchOptions.AutocorrectQuery;
			options.Count = ActualSearchOptions.ResultsCount;
			if (!parseOnly)
				options.StartingIndex = startingIndex;
			options.ListingType = ListingType.Unknown;
			options.Radius = ActualSearchOptions.SearchRadius;
			options.ParseOnly = parseOnly;
			request.SearchOptions = options;
			UserProfile profile = new UserProfile();
			profile.CurrentLocation = new UserLocation() { Latitude = searchAnchorPoint.Latitude, Longitude = searchAnchorPoint.Longitude };
			profile.DistanceUnit = BingServicesUtils.ConvertDistanceMeasureUnitToDistanceUnit(ActualSearchOptions.DistanceUnit);
			request.UserProfile = profile;
			searchService.SearchAsync(request);
#else
			searchClient.Parameters.Add(keyword);
			searchClient.Arguments.Add("includeNeighborhood", "1");
			searchClient.Arguments.Add("include", "ciso2");
			searchClient.Arguments.Add("o", "xml");
			searchClient.Arguments.Add("c", culture);
			searchClient.Arguments.Add("key", ActualBingKey);
#endif
		}
#if UseSoap
		void OnSearchCompleted(object sender, SearchCompletedEventArgs e) {
			SearchResponse response = e.Result;
			RequestResultCode resultCode;
			string faultReason;
			if (response != null) {
				resultCode = BingServicesUtils.GetRequestResultCode(response.ResponseSummary.StatusCode);
				faultReason = response.ResponseSummary.FaultReason;
			}
			else {
				resultCode = RequestResultCode.Timeout;
				faultReason = "Request timeout";
			}
			if (resultCode == RequestResultCode.Success && response.ResultSets.Count > 0) {
				searchService.SearchCompleted -= OnSearchCompleted;
				searchService.SearchCompleted += OnAlternateSearchCompleted;
				string location = response.ResultSets[0].Parse.Address != null ? response.ResultSets[0].Parse.Address.FormattedAddress : "";
				SearchInternal(response.ResultSets[0].Parse.Keyword, location, startingIndex, false, CultureName);
			}
			else {
				SearchRequestResult failResult = new SearchRequestResult(resultCode, faultReason,
					new List<LocationInformation>(), null, new List<LocationInformation>(), 0, "", "");
				if (SearchCompleted != null)
					SearchCompleted(this, new BingSearchCompletedEventArgs(failResult, e.Error, e.Cancelled, e.UserState));
				SendResultsToLayer(failResult, e.Error, e.Cancelled, e.UserState);
				isBusy = false;
				startingIndex = 0;
			}
		}
#else
#endif
#if UseSoap
		void OnAlternateSearchCompleted(object sender, SearchCompletedEventArgs e) {
			SearchResponse response = e.Result;
			RequestResultCode resultCode;
			string faultReason;
			if (response != null) {
				resultCode = BingServicesUtils.GetRequestResultCode(response.ResponseSummary.StatusCode);
				faultReason = response.ResponseSummary.FaultReason;
			}
			else {
				resultCode = RequestResultCode.Timeout;
				faultReason = "Request timeout";
			}
			SearchRequestResult requestResult = null;
			if (resultCode == RequestResultCode.Success && response.ResultSets.Count > 0)
				requestResult = CreateRequestResult(response, response.ResultSets[0]);
			else
				requestResult = new SearchRequestResult(resultCode, faultReason, 
					new List<LocationInformation>(), null, new List<LocationInformation>(), 0, "", "");
			if (SearchCompleted != null)
				SearchCompleted(this, new BingSearchCompletedEventArgs(requestResult, e.Error, e.Cancelled, e.UserState));
			searchService.SearchCompleted -= OnAlternateSearchCompleted;
			searchService.SearchCompleted += OnSearchCompleted;
			SendResultsToLayer(requestResult, e.Error, e.Cancelled, e.UserState);
			isBusy = false;
			startingIndex = 0;
		}
#else
#endif
		void SendResultsToLayer(SearchRequestResult requestResult, Exception error, bool cancelled, object userState) {
			List<MapItem> mapItems = CreateMapItems(requestResult);
			List<string> searchAddresses = CreateSearchAddreses(requestResult);
			if (requestResult.ResultCode == RequestResultCode.Success && requestResult.AlternateSearchRegions.Count > 0 && ShowSearchPanel && generateResultsForSearchPanel) {
					lastRequestResult = requestResult;
					hasAlternateResuts = true;
			}
			if (GenerateLayerItems && mapItems.Count > 0)
				RaiseLayerItemsGenerating(new LayerItemsGeneratingEventArgs(mapItems.ToArray(), error, cancelled, userState));
			RaiseRequestComplete(new RequestCompletedEventArgs(mapItems.ToArray(), error, cancelled, searchAddresses));
		}
#if UseSoap
		SearchRequestResult CreateRequestResult(SearchResponse response, SearchResultSet resultSet) {
			LocationInformation searchRegion = null;
			List<LocationInformation> alternateSearchRegions = new List<LocationInformation>();
			List<LocationInformation> searchResults = new List<LocationInformation>();
			string keyword = "";
			string location = "";
			foreach (SearchResultBase resultBase in resultSet.Results) {
				DevExpress.Map.BingServices.Address addressInfo = null;
				if (resultBase is BusinessSearchResult && ((BusinessSearchResult)resultBase).Address != null)
					addressInfo = ((BusinessSearchResult)resultBase).Address;
				if (resultBase is PersonSearchResult && ((PersonSearchResult)resultBase).Address != null)
					addressInfo = ((PersonSearchResult)resultBase).Address;
				searchResults.Add(new LocationInformation(new GeoPoint(resultBase.LocationData.Locations[0].Latitude, resultBase.LocationData.Locations[0].Longitude),
					resultSet.ListingType, resultBase.Name, BingServicesUtils.ConvertToBingAddress(addressInfo)));
			}
			if (resultSet.QueryCorrected && (response.QuerySuggestion.Query != null || response.QuerySuggestion.StructuredQuery != null)) {
				keyword = response.QuerySuggestion.Query != null ? response.QuerySuggestion.Query : response.QuerySuggestion.StructuredQuery.Keyword;
				location = response.QuerySuggestion.StructuredQuery != null ? response.QuerySuggestion.StructuredQuery.Location : "";
			}
			else
				if (resultSet.Parse != null) {
					keyword = resultSet.Parse.Keyword;
					if (resultSet.Parse.Address != null)
						location = resultSet.Parse.Address.FormattedAddress;
				}
			if (resultSet.SearchRegion != null && resultSet.SearchRegion.GeocodeLocation != null) {
				searchRegion = new LocationInformation(BingServicesUtils.ConvertLocationToGeoPoint(resultSet.SearchRegion.GeocodeLocation.Locations[0]),
				resultSet.SearchRegion.GeocodeLocation.EntityType, resultSet.SearchRegion.GeocodeLocation.DisplayName, BingServicesUtils.ConvertToBingAddress(resultSet.SearchRegion.GeocodeLocation.Address));
			}
			foreach (SearchRegion alternateRegion in resultSet.AlternateSearchRegions)
				if (alternateRegion.GeocodeLocation != null) {
					alternateSearchRegions.Add(new LocationInformation(BingServicesUtils.ConvertLocationToGeoPoint(alternateRegion.GeocodeLocation.Locations[0]),
							alternateRegion.GeocodeLocation.EntityType, alternateRegion.GeocodeLocation.DisplayName, BingServicesUtils.ConvertToBingAddress(alternateRegion.GeocodeLocation.Address)));
				}
			return new SearchRequestResult(BingServicesUtils.GetRequestResultCode(response.ResponseSummary.StatusCode),
				response.ResponseSummary.FaultReason, searchResults, searchRegion, alternateSearchRegions, resultSet.EstimatedMatches, keyword, location);
		}
#else
#endif
		List<MapItem> CreateMapItems(SearchRequestResult requestResult) {
			List<MapItem> mapItems = new List<MapItem>();
			if (requestResult.ResultCode == RequestResultCode.Success) {
				foreach (LocationInformation resultInfo in requestResult.SearchResults)
						if (GenerateLayerItems)
							mapItems.Add(new MapPushpin() { Location = resultInfo.Location, Information = resultInfo });
				if (requestResult.SearchResults.Count == 0 && GenerateLayerItems && requestResult.SearchRegion != null)
					mapItems.Add(new MapPushpin() { Location = requestResult.SearchRegion.Location, Information = requestResult.SearchRegion });
			}
			return mapItems;
		}
		List<string> CreateSearchAddreses(SearchRequestResult requestResult) {
			List<string> searchAddresses = new List<string>();
			string keyword = String.IsNullOrEmpty(requestResult.Keyword) ? "" : requestResult.Keyword + "; ";
			if (requestResult.ResultCode == RequestResultCode.Success) {
				foreach (LocationInformation resultInfo in requestResult.SearchResults) {
					if (resultInfo != null && resultInfo.Address != null)
						if (!searchAddresses.Contains(resultInfo.Address.FormattedAddress) && ShowSearchPanel && generateResultsForSearchPanel)
							searchAddresses.Add(resultInfo.Address.FormattedAddress);
				}
				if (requestResult.SearchResults.Count == 0 && ShowSearchPanel && generateResultsForSearchPanel && requestResult.SearchRegion != null)
					searchAddresses.Add(keyword + requestResult.SearchRegion.Address);
			}
			return searchAddresses;
		}
		internal void SearchFromSearchPanel(string keyword) {
			generateResultsForSearchPanel = true;
			if (!isBusy) {
				isBusy = true;
				SearchInternal(keyword, null, 0, true, CultureName);
			}
		}
		internal void GenerateSearchResults() {
			List<MapItem> mapItems = CreateMapItems(lastRequestResult);
			List<string> searchAddresses = CreateSearchAddreses(lastRequestResult);
			if (GenerateLayerItems && mapItems.Count > 0)
				RaiseLayerItemsGenerating(new LayerItemsGeneratingEventArgs(mapItems.ToArray(), null, false, null));
			RaiseRequestComplete(new RequestCompletedEventArgs(mapItems.ToArray(), null, false, searchAddresses));
		}
		internal void GenerateAlternateSearchRequests() {
			List<MapItem> mapItems = CreateMapItems(lastRequestResult);
			List<string> alternateSearchRequests = CreateSearchAddreses(lastRequestResult);
			string keyword = String.IsNullOrEmpty(lastRequestResult.Keyword) ? "" : lastRequestResult.Keyword + "; ";
			foreach (LocationInformation locationInfo in lastRequestResult.AlternateSearchRegions){
				if (locationInfo != null && locationInfo.Address != null && !alternateSearchRequests.Contains(locationInfo.Address.FormattedAddress)) {
					if (ShowSearchPanel && generateResultsForSearchPanel)
						alternateSearchRequests.Add(keyword + locationInfo.Address);
					if (GenerateLayerItems)
						mapItems.Add(new MapPushpin() { Location = locationInfo.Location, Information = locationInfo });
				}
		}
			if (GenerateLayerItems && mapItems.Count > 0)
				RaiseLayerItemsGenerating(new LayerItemsGeneratingEventArgs(mapItems.ToArray(), null, false, null));
			RaiseRequestComplete(new RequestCompletedEventArgs(mapItems.ToArray(), null, false, alternateSearchRequests));
		}
		protected override MapDependencyObject CreateObject() {
			return new BingSearchDataProvider();
		}
		public void Search(string keyword) {
			Search(keyword, null);
		}
		public void Search(string keyword, int startingIndex) {
			this.startingIndex = startingIndex;
			Search(keyword);
		}
		public void Search(string keyword, string location) {
			Search(keyword, location, CultureName);
		}
		public void Search(string keyword, string location, string culture) {
			generateResultsForSearchPanel = false;
			if(!isBusy) {
				isBusy = true;
				if(Layer != null && Layer.Map != null)
					searchAnchorPoint = (GeoPoint)Layer.Map.CenterPoint;
				else
					searchAnchorPoint = new GeoPoint(0.0, 0.0);
				SearchInternal(keyword, location, 0, true, culture);
			}
		}
		public void Search(string keyword, string location, int startingIndex) {
			this.startingIndex = startingIndex;
			Search(keyword, location);
		}
		public void Search(string keyword, string location, GeoPoint searchAnchorPoint) {
			Search(keyword, location, searchAnchorPoint, CultureName);
		}
		public void Search(string keyword, string location, GeoPoint searchAnchorPoint, string culture) {
			generateResultsForSearchPanel = false;
			if(!isBusy) {
				isBusy = true;
				this.searchAnchorPoint = searchAnchorPoint;
				SearchInternal(keyword, location, 0, true, culture);
			}
		}
		public void Search(string keyword, string location, GeoPoint searchAnchorPoint, int startingIndex) {
			this.startingIndex = startingIndex;
			Search(keyword, location, searchAnchorPoint);
		}
		public override void Cancel() {
			ResetSearchService();
			isBusy = false;
		}
	}
	public class SearchRequestResult : RequestResultBase {
		readonly List<LocationInformation> searchResults;
		readonly List<LocationInformation> alternateSearchRegions;
		LocationInformation searchRegion;
		int estimatedMatches;
		string keyword;
		string location;
		public List<LocationInformation> SearchResults { get { return searchResults; } }
		public LocationInformation SearchRegion { get { return searchRegion; } }
		public List<LocationInformation> AlternateSearchRegions { get { return alternateSearchRegions; } }
		public int EstimatedMatches { get { return estimatedMatches; } }
		public string Keyword { get { return keyword; } }
		public string Location { get { return location; } }
		public SearchRequestResult(RequestResultCode statusCode, string faultReason, List<LocationInformation> searchResults, 
			LocationInformation searchRegion, List<LocationInformation> alternateSearchRegions,
			int estimatedMatches, string keyword, string location)
			: base(statusCode, faultReason) {
			this.searchResults = searchResults;
			this.searchRegion = searchRegion;
			this.alternateSearchRegions = alternateSearchRegions;
			this.estimatedMatches = estimatedMatches;
			this.keyword = keyword;
			this.location = location;
		}
	}
}
