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

using DevExpress.Map.BingServices;
using DevExpress.Map.Native;
using DevExpress.Utils;
using DevExpress.XtraMap.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceModel;
using System.Threading;
namespace DevExpress.XtraMap {
	public delegate void BingSearchCompletedEventHandler(object sender, BingSearchCompletedEventArgs e);
	public class BingSearchCompletedEventArgs : AsyncCompletedEventArgs {
		readonly SearchRequestResult requestResult;
		public SearchRequestResult RequestResult { get { return requestResult; } }
		public BingSearchCompletedEventArgs(SearchRequestResult requestResult, Exception error, bool cancelled, object userState)
			: base(error, cancelled, userState) {
			this.requestResult = requestResult;
		}
	}
	public class BingSearchDataProvider : BingMapDataProviderBase, ISupportObjectChanged, IDisposable {
		static BingSearchOptions DefaultSearchOptions = new BingSearchOptions();
		bool showSearchPanel = true;
		BingSearchOptions searchOptions;
		GeoPoint searchAnchorPoint;
		int startingIndex;
		bool generateResultsForSearchPanel;
		SearchRequestResult lastRequestResult;
		bool hasAlternateResuts;
		SearchServiceClient searchService;
		bool disposed = false;
		BingSearchOptions ActualSearchOptions {
			get { return SearchOptions != null ? SearchOptions : DefaultSearchOptions; }
		}
		internal bool HasAlternateResults { get { return hasAlternateResuts; } }
		protected internal override int MaxVisibleResultCountInternal { get { return 1; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("BingSearchDataProviderShowSearchPanel"),
#endif
		Category(SRCategoryNames.Behavior), DefaultValue(true)]
		public bool ShowSearchPanel {
			get {
				return showSearchPanel;
			}
			set {
				showSearchPanel = value;
				RaiseChanged();
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("BingSearchDataProviderSearchOptions"),
#endif
		Category(SRCategoryNames.Behavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public BingSearchOptions SearchOptions { get { return searchOptions; } }
#if !SL
	[DevExpressXtraMapLocalizedDescription("BingSearchDataProviderSearchCompleted")]
#endif
		public event BingSearchCompletedEventHandler SearchCompleted;
		public BingSearchDataProvider() {
			this.searchOptions = new BingSearchOptions();
			ResetSearchService();
		}
		~BingSearchDataProvider() {
			Dispose(false);
		}
		#region ISupportObjectChanged implementation
		EventHandler onChanged;
		event EventHandler ISupportObjectChanged.Changed {
			add { onChanged += value; }
			remove { onChanged -= value; }
		}
		protected internal void RaiseChanged() {
			if (onChanged != null) onChanged(this, EventArgs.Empty);
		}
		#endregion
		void ResetSearchService() {
			if(searchService != null) {
				searchService.SearchCompleted -= OnSearchCompleted;
				searchService.SearchCompleted -= OnAlternateSearchCompleted;
				searchAnchorPoint = new GeoPoint(0.0, 0.0);
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
		}
		void OnAlternateSearchCompleted(object sender, SearchCompletedEventArgs e) {
			FinalizeSearch(e);
		}
		void FinalizeSearch(SearchCompletedEventArgs e) {
			SearchResponse response = e.Result;
			RequestResultCode resultCode;
			string faultReason;
			if(response != null) {
				resultCode = BingServicesUtils.GetRequestResultCode(response.ResponseSummary.StatusCode);
				faultReason = response.ResponseSummary.FaultReason;
			} else {
				resultCode = RequestResultCode.Timeout;
				faultReason = "Request timeout";
			}
			SearchRequestResult requestResult = null;
			if(resultCode == RequestResultCode.Success && response.ResultSets.Count > 0)
				requestResult = CreateRequestResult(response, response.ResultSets[0]);
			else
				requestResult = new SearchRequestResult(resultCode, faultReason,
					new List<LocationInformation>(), null, new List<LocationInformation>(), 0, "", "");
			if(SearchCompleted != null)
				SearchCompleted(this, new BingSearchCompletedEventArgs(requestResult, e.Error, e.Cancelled, e.UserState));
			searchService.SearchCompleted -= OnAlternateSearchCompleted;
			searchService.SearchCompleted += OnSearchCompleted;
			SendResultsToLayer(requestResult, e.Error, e.Cancelled, e.UserState);
			IsBusy = false;
			startingIndex = 0;
		}
		void OnSearchCompleted(object sender, SearchCompletedEventArgs e) {
			SearchResponse response = e.Result;
			RequestResultCode resultCode;
			string faultReason;
			if(response != null) {
				resultCode = BingServicesUtils.GetRequestResultCode(response.ResponseSummary.StatusCode);
				faultReason = response.ResponseSummary.FaultReason;
			} else {
				resultCode = RequestResultCode.Timeout;
				faultReason = "Request timeout";
			}
			if(resultCode == RequestResultCode.Success && response.ResultSets.Count > 0) {
				searchService.SearchCompleted -= OnSearchCompleted;
				Parse parse = response.ResultSets[0].Parse;
				string keyword = parse != null && parse.Address != null ? parse.Address.FormattedAddress : "";
				string location = parse != null ? parse.Keyword : string.Empty;
				if(!string.IsNullOrEmpty(keyword) || !string.IsNullOrEmpty(location)) {
					searchService.SearchCompleted += OnAlternateSearchCompleted;
					SearchInternal(keyword, parse != null ? parse.Keyword : "", startingIndex, false, true);
				} else
					FinalizeSearch(e);
			} else {
				SearchRequestResult failResult = new SearchRequestResult(resultCode, faultReason,
					new List<LocationInformation>(), null, new List<LocationInformation>(), 0, "", "");
				if(SearchCompleted != null)
					SearchCompleted(this, new BingSearchCompletedEventArgs(failResult, e.Error, e.Cancelled, e.UserState));
				SendResultsToLayer(failResult, e.Error, e.Cancelled, e.UserState);
				IsBusy = false;
				startingIndex = 0;
			}
		}
		SearchRequestResult CreateRequestResult(SearchResponse response, SearchResultSet resultSet) {
			LocationInformation searchRegion = null;
			List<LocationInformation> alternateSearchRegions = new List<LocationInformation>();
			List<LocationInformation> searchResults = new List<LocationInformation>();
			string keyword = "";
			string location = "";
			foreach(SearchResultBase resultBase in resultSet.Results) {
				DevExpress.Map.BingServices.Address addressInfo = null;
				BusinessSearchResult businessSearchResult = resultBase as BusinessSearchResult;
				if(businessSearchResult != null && businessSearchResult.Address != null)
					addressInfo = businessSearchResult.Address;
				PersonSearchResult personSearchResult = resultBase as PersonSearchResult;
				if(personSearchResult != null && personSearchResult.Address != null)
					addressInfo = personSearchResult.Address;
				searchResults.Add(new LocationInformation(new GeoPoint(resultBase.LocationData.Locations[0].Latitude, resultBase.LocationData.Locations[0].Longitude),
					resultSet.ListingType, resultBase.Name, ConvertToBingAddress(addressInfo)));
			}
			if(resultSet.QueryCorrected && (response.QuerySuggestion.Query != null || response.QuerySuggestion.StructuredQuery != null)) {
				keyword = response.QuerySuggestion.Query != null ? response.QuerySuggestion.Query : response.QuerySuggestion.StructuredQuery.Keyword;
				location = response.QuerySuggestion.StructuredQuery != null ? response.QuerySuggestion.StructuredQuery.Location : "";
			} else
				if(resultSet.Parse != null) {
					keyword = resultSet.Parse.Keyword;
					if(resultSet.Parse.Address != null)
						location = resultSet.Parse.Address.FormattedAddress;
				}
			if(resultSet.SearchRegion != null && resultSet.SearchRegion.GeocodeLocation != null) {
				searchRegion = new LocationInformation(BingServicesUtils.ConvertLocationToGeoPoint(resultSet.SearchRegion.GeocodeLocation.Locations[0]),
						resultSet.SearchRegion.GeocodeLocation.EntityType, resultSet.SearchRegion.GeocodeLocation.DisplayName, ConvertToBingAddress(resultSet.SearchRegion.GeocodeLocation.Address));
			}
			foreach(SearchRegion alternateRegion in resultSet.AlternateSearchRegions)
				if(alternateRegion.GeocodeLocation != null) {
					alternateSearchRegions.Add(new LocationInformation(BingServicesUtils.ConvertLocationToGeoPoint(alternateRegion.GeocodeLocation.Locations[0]),
							alternateRegion.GeocodeLocation.EntityType, alternateRegion.GeocodeLocation.DisplayName, ConvertToBingAddress(alternateRegion.GeocodeLocation.Address)));
				}
			return new SearchRequestResult(BingServicesUtils.GetRequestResultCode(response.ResponseSummary.StatusCode),
				response.ResponseSummary.FaultReason, searchResults, searchRegion, alternateSearchRegions, resultSet.EstimatedMatches, keyword, location);
		}
		void SetSearchAnchorPoint() {
			if(Layer != null && Layer.Map != null)
				searchAnchorPoint = (GeoPoint)Layer.Map.CenterPoint;
			else
				searchAnchorPoint = new GeoPoint(0.0, 0.0);
		}
		void SearchInternal(string keyword, string location, int startingIndex, bool parseOnly, bool searchPanelUsed) {
			lastRequestResult = null;
			hasAlternateResuts = false;
			SearchRequest request = CreateSearchRequest(keyword, location, startingIndex, parseOnly, searchPanelUsed);
			searchService.SearchAsync(request);
		}
		SearchRequest CreateSearchRequest(string keyword, string location, int startingIndex, bool parseOnly, bool searchPanelUsed) {
			SearchRequest request = new SearchRequest();
			request.Credentials = new Credentials();
			request.Credentials.ApplicationId = ActualBingKey;
			request.Culture = Thread.CurrentThread.CurrentUICulture.ToString();
			request.ExecutionOptions = new ExecutionOptions() { SuppressFaults = false };
			SetQuery(keyword, location, searchPanelUsed, request);
			SearchOptions options = new SearchOptions();
			options.AutocorrectQuery = ActualSearchOptions.AutocorrectQuery;
			options.Count = ActualSearchOptions.ResultsCount;
			if(!parseOnly)
				options.StartingIndex = startingIndex;
			options.ListingType = ListingType.Unknown;
			if(!searchPanelUsed)
				options.Radius = ActualSearchOptions.SearchRadius;
			options.ParseOnly = false;
			request.SearchOptions = options;
			UserProfile profile = new UserProfile();
			if(searchAnchorPoint != null && !searchPanelUsed)
				profile.CurrentLocation = new UserLocation() { Latitude = searchAnchorPoint.Latitude, Longitude = searchAnchorPoint.Longitude };
			profile.DistanceUnit = (DevExpress.Map.BingServices.DistanceUnit)ActualSearchOptions.DistanceUnit;
			if(!searchPanelUsed)
				profile.MapView = GetMapView();
			request.UserProfile = profile;
			return request;
		}
		void SetQuery(string keyword, string location, bool searchPanelUsed, SearchRequest request) {
			if(string.IsNullOrEmpty(location))
				request.Query = keyword;
			else
				request.StructuredQuery = GetStructuredQuery(keyword, location, searchPanelUsed);
		}
		StructuredSearchQuery GetStructuredQuery(string keyword, string location, bool searchPanelUsed) {
			if(searchPanelUsed)
				return new StructuredSearchQuery() { Keyword = location, Location = keyword };
			else
				return new StructuredSearchQuery() { Keyword = keyword, Location = location };
		}
		Rectangle GetMapView() {
			if(BingSearchOptions.IsDefault(ActualSearchOptions) || searchAnchorPoint == null)
				return null;
			MapSize delta = Layer.UnitConverter.MeasureUnitToCoordSize(searchAnchorPoint, new MapSize(ActualSearchOptions.SearchRadius, ActualSearchOptions.SearchRadius));
			GeoPoint northEast = new GeoPoint(searchAnchorPoint.Latitude + delta.Height, searchAnchorPoint.Longitude + delta.Width);
			GeoPoint southWest = new GeoPoint(searchAnchorPoint.Latitude - delta.Height, searchAnchorPoint.Longitude - delta.Width);
			return new Rectangle() {
				Northeast = new Location() { Latitude = northEast.Latitude, Longitude = northEast.Longitude }, 
				Southwest = new Location() { Latitude = southWest.Latitude, Longitude = southWest.Longitude } 
			};
		}
		List<MapItem> CreateMapItems(SearchRequestResult requestResult) {
			List<MapItem> mapItems = new List<MapItem>();
			if(requestResult.ResultCode == RequestResultCode.Success) {
				foreach(LocationInformation resultInfo in requestResult.SearchResults)
					if(GenerateLayerItems)
						mapItems.Add(CreatePushpin(resultInfo));
				if(requestResult.SearchResults.Count == 0 && GenerateLayerItems && requestResult.SearchRegion != null)
					mapItems.Add(CreatePushpin(requestResult.SearchRegion));
			}
			return mapItems;
		}
		MapPushpin CreatePushpin(LocationInformation info) {
			return new MapPushpin() { Location = info.Location, Information = info, ToolTipPattern = info.DisplayName };
		}
		List<LocationInformation> CreateSearchAddreses(SearchRequestResult requestResult) {
			List<LocationInformation> searchAddresses = new List<LocationInformation>();
			string keyword = String.IsNullOrEmpty(requestResult.Keyword) ? "" : requestResult.Keyword + "; ";
			if(requestResult.ResultCode == RequestResultCode.Success) {
				foreach(LocationInformation resultInfo in requestResult.SearchResults)
					if(ShowSearchPanel && generateResultsForSearchPanel)
						searchAddresses.Add(resultInfo);
				if(requestResult.SearchResults.Count == 0 && ShowSearchPanel && generateResultsForSearchPanel && requestResult.SearchRegion != null) {
					requestResult.SearchRegion.Address.FormattedAddress = keyword + requestResult.SearchRegion.Address.FormattedAddress;
					searchAddresses.Add(requestResult.SearchRegion);
				}
			}
			return searchAddresses;
		}
		protected virtual void Dispose(bool disposing){
			if (disposed)
				return;
			if (disposing)
				searchService.Close();
			disposed = true;
		}
		protected internal void SendResultsToLayer(SearchRequestResult requestResult, Exception error, bool cancelled, object userState) {
			List<MapItem> mapItems = CreateMapItems(requestResult);
			List<LocationInformation> searchAddresses = CreateSearchAddreses(requestResult);
			if(requestResult.ResultCode == RequestResultCode.Success && requestResult.AlternateSearchRegions.Count > 0 && ShowSearchPanel && generateResultsForSearchPanel) {
				lastRequestResult = requestResult;
				hasAlternateResuts = true;
			}
			if(GenerateLayerItems && mapItems.Count > 0)
				RaiseLayerItemsGenerating(new LayerItemsGeneratingEventArgs(mapItems.ToArray(), error, cancelled, userState));
			OnRequestComplete(new RequestCompletedEventArgs(mapItems.ToArray(), error, cancelled, searchAddresses));
		}
		protected internal override void Cancel() {
			if(IsBusy) {
				ResetSearchService();
				IsBusy = false;
			}
		}
		protected internal void SearchByString(string keyword) {
			generateResultsForSearchPanel = true;
			if (!IsBusy) {
				IsBusy = true;
				SetSearchAnchorPoint();
				SearchInternal(keyword, null, 0, true, true);
			}
		}
		internal void GenerateSearchResults() {
			List<MapItem> mapItems = CreateMapItems(lastRequestResult);
			List<LocationInformation> searchAddresses = CreateSearchAddreses(lastRequestResult);
			if (GenerateLayerItems && mapItems.Count > 0)
				RaiseLayerItemsGenerating(new LayerItemsGeneratingEventArgs(mapItems.ToArray(), null, false, null));
			OnRequestComplete(new RequestCompletedEventArgs(mapItems.ToArray(), null, false, searchAddresses));
		}
		internal void GenerateAlternateSearchRequests() {
			List<MapItem> mapItems = CreateMapItems(lastRequestResult);
			List<LocationInformation> alternateSearchRequests = CreateSearchAddreses(lastRequestResult);
			string keyword = String.IsNullOrEmpty(lastRequestResult.Keyword) ? "" : lastRequestResult.Keyword + "; ";
			foreach (LocationInformation locationInfo in lastRequestResult.AlternateSearchRegions) {
				if (ShowSearchPanel && generateResultsForSearchPanel) {
					locationInfo.Address.FormattedAddress = keyword + locationInfo.Address.FormattedAddress;
					alternateSearchRequests.Add(locationInfo);
				}
				if (GenerateLayerItems)
					mapItems.Add(CreatePushpin(locationInfo));
			}
			if (GenerateLayerItems && mapItems.Count > 0)
				RaiseLayerItemsGenerating(new LayerItemsGeneratingEventArgs(mapItems.ToArray(), null, false, null));
			OnRequestComplete(new RequestCompletedEventArgs(mapItems.ToArray(), null, false, alternateSearchRequests));
		}
		public void Search(string keyword) {
			Search(keyword, null);
		}
		public void Search(string keyword, int startingIndex) {
			this.startingIndex = startingIndex;
			Search(keyword);
		}
		public void Search(string keyword, string location) {
			generateResultsForSearchPanel = false;
			if(!IsBusy) {
				IsBusy = true;
				SetSearchAnchorPoint();
				SearchInternal(keyword, location, 0, true, false);
			}
		}
		public void Search(string keyword, string location, int startingIndex) {
			this.startingIndex = startingIndex;
			Search(keyword, location);
		}
		public void Search(string keyword, string location, GeoPoint searchAnchorPoint) {
			generateResultsForSearchPanel = false;
			if(!IsBusy) {
				IsBusy = true;
				this.searchAnchorPoint = searchAnchorPoint;
				SearchInternal(keyword, location, 0, true, false);
			}
		}
		public void Search(string keyword, string location, GeoPoint searchAnchorPoint, int startingIndex) {
			this.startingIndex = startingIndex;
			Search(keyword, location, searchAnchorPoint);
		}
		public void ClearResults() {
			Layer.ClearResults();
			lastRequestResult = null;
			this.hasAlternateResuts = false;
		}
		public void ClearSearchPanel() {
			Layer.Map.InteractionController.SearchPanelReset();
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		public override string ToString() {
			return "(BingSearchDataProvider)";
		}
	}
	public class LocationInformation {
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("LocationInformationLocation"),
#endif
		DefaultValue(null)]
		public GeoPoint Location { get; set; }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("LocationInformationEntityType"),
#endif
		DefaultValue(null)]
		public string EntityType { get; set; }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("LocationInformationDisplayName"),
#endif
		DefaultValue(null)]
		public string DisplayName { get; set; }
		[DefaultValue(null)]
		public AddressBase Address { get; set; }
		public LocationInformation() {
		}
		public LocationInformation(GeoPoint location, string entityType, string displayName, AddressBase address) {
			Location = location;
			EntityType = entityType;
			DisplayName = displayName;
			Address = address;
		}
		public override string ToString() {
			return Address != null ? Address.FormattedAddress : null;
		}
	}
	public class SearchRequestResult : RequestResultBase {
		readonly List<LocationInformation> searchResults;
		readonly List<LocationInformation> alternateSearchRegions;
		LocationInformation searchRegion;
		int estimatedMatches;
		string keyword;
		string location;
#if !SL
	[DevExpressXtraMapLocalizedDescription("SearchRequestResultSearchResults")]
#endif
		public List<LocationInformation> SearchResults { get { return searchResults; } }
#if !SL
	[DevExpressXtraMapLocalizedDescription("SearchRequestResultSearchRegion")]
#endif
		public LocationInformation SearchRegion { get { return searchRegion; } }
#if !SL
	[DevExpressXtraMapLocalizedDescription("SearchRequestResultAlternateSearchRegions")]
#endif
		public List<LocationInformation> AlternateSearchRegions { get { return alternateSearchRegions; } }
#if !SL
	[DevExpressXtraMapLocalizedDescription("SearchRequestResultEstimatedMatches")]
#endif
		public int EstimatedMatches { get { return estimatedMatches; } }
#if !SL
	[DevExpressXtraMapLocalizedDescription("SearchRequestResultKeyword")]
#endif
		public string Keyword { get { return keyword; } }
#if !SL
	[DevExpressXtraMapLocalizedDescription("SearchRequestResultLocation")]
#endif
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
	public class BingSearchOptions : MapNotificationOptions {
		const bool DefaultAutocorrectQuery = true;
		const int DefaultResultsCount = 10;
		const double DefaultSearchRadius = 100.0;
		const DistanceMeasureUnit DefaultDistanceUnit = DistanceMeasureUnit.Kilometer;
		bool autocorrectQuery = DefaultAutocorrectQuery;
		int resultsCount = DefaultResultsCount;
		double searchRadius = DefaultSearchRadius;
		DistanceMeasureUnit distanceUnit = DefaultDistanceUnit;
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("BingSearchOptionsAutocorrectQuery"),
#endif
		Category(SRCategoryNames.Behavior), DefaultValue(DefaultAutocorrectQuery)]
		public bool AutocorrectQuery {
			get { return autocorrectQuery; }
			set {
				if (autocorrectQuery == value)
					return;
				autocorrectQuery = value;
				OnChanged("AutocorrectQuery", !autocorrectQuery, autocorrectQuery);
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("BingSearchOptionsResultsCount"),
#endif
		Category(SRCategoryNames.Behavior), DefaultValue(DefaultResultsCount)]
		public int ResultsCount {
			get { return resultsCount; }
			set {
				if (resultsCount == value)
					return;
				int oldValue = resultsCount;
				resultsCount = value;
				OnChanged("ResultsCount", oldValue, resultsCount);
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("BingSearchOptionsSearchRadius"),
#endif
		Category(SRCategoryNames.Behavior), DefaultValue(DefaultSearchRadius)]
		public double SearchRadius {
			get { return searchRadius; }
			set {
				if (MathUtils.Compare(searchRadius, value))
					return;
				double oldValue = searchRadius;
				searchRadius = value;
				OnChanged("SearchRadius", oldValue, searchRadius);
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("BingSearchOptionsDistanceUnit"),
#endif
		Category(SRCategoryNames.Behavior), DefaultValue(DefaultDistanceUnit)]
		public DistanceMeasureUnit DistanceUnit {
			get { return distanceUnit; }
			set {
				if (distanceUnit == value)
					return;
				DistanceMeasureUnit oldValue = distanceUnit;
				distanceUnit = value;
				OnChanged("DistanceUnit", oldValue, distanceUnit);
			}
		}
		internal static bool IsDefault(BingSearchOptions options) {
			return options.AutocorrectQuery == DefaultAutocorrectQuery && options.DistanceUnit == DefaultDistanceUnit && options.ResultsCount == DefaultResultsCount && options.SearchRadius == DefaultSearchRadius;
		}
		protected internal override void ResetCore() {
			autocorrectQuery = DefaultAutocorrectQuery;
			resultsCount = DefaultResultsCount;
			searchRadius = DefaultSearchRadius;
			distanceUnit = DefaultDistanceUnit;
		}
	}
}
