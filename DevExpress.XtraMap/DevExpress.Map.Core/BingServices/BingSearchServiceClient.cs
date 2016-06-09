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
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Windows;
using System.Xml.Linq;
using SendOrPostCallback = System.Threading.SendOrPostCallback;
namespace DevExpress.Map.BingServices {
	[DataContract(Name = "LogicalOperator", Namespace = BingServicesNamespaces.CommonNamespace)]
	public enum LogicalOperator : int {
		[EnumMember]
		And = 0,
		[EnumMember]
		Or = 1,
	}
	[DataContract(Name = "ListingType", Namespace = BingServicesNamespaces.SearchNamespace)]
	public enum ListingType : int {
		[EnumMember]
		Business = 0,
		[EnumMember]
		CommunityContent = 1,
		[EnumMember]
		Person = 2,
		[EnumMember]
		Unknown = 3,
	}
	[DataContract(Name = "SortOrder", Namespace = BingServicesNamespaces.SearchNamespace)]
	public enum SortOrder : int {
		[EnumMember]
		Relevance = 0,
		[EnumMember]
		Distance = 1,
		[EnumMember]
		Rating = 2,
		[EnumMember]
		Popularity = 3,
	}
	[DataContract(Name = "CompareOperator", Namespace = BingServicesNamespaces.CommonNamespace)]
	public enum CompareOperator : int {
		[EnumMember]
		Equals = 0,
		[EnumMember]
		GreaterThan = 1,
		[EnumMember]
		GreaterThanOrEquals = 2,
		[EnumMember]
		LessThan = 3,
		[EnumMember]
		LessThanOrEquals = 4,
	}
	public class SearchCompletedEventArgs : AsyncCompletedEventArgs {
		object[] results;
		public SearchResponse Result {
			get {
				try {
					base.RaiseExceptionIfNecessary();
				} catch {
					return null;
				}
				return ((SearchResponse)(this.results[0]));
			}
		}
		public SearchCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) :
			base(exception, cancelled, userState) {
			this.results = results;
		}
	}
	[ServiceContract(Namespace = BingServicesNamespaces.SearchContractNamespace, ConfigurationName = "BingSearch.ISearchService")]
	public interface ISearchService {
		[OperationContract(Action = "http://dev.virtualearth.net/webservices/v1/search/contracts/ISearchService/Search",
			ReplyAction = "http://dev.virtualearth.net/webservices/v1/search/contracts/ISearchService/SearchResponse")]
		[FaultContract(typeof(ResponseSummary), Action = "http://dev.virtualearth.net/webservices/v1/search/contracts/ISearchService/SearchResponseSummaryFault",
			Name = "ResponseSummary", Namespace = "http://dev.virtualearth.net/webservices/v1/common")]
		SearchResponse Search(SearchRequest request);
		[OperationContract(AsyncPattern = true, Action = "http://dev.virtualearth.net/webservices/v1/search/contracts/ISearchService/Search",
			ReplyAction = "http://dev.virtualearth.net/webservices/v1/search/contracts/ISearchService/SearchResponse")]
		IAsyncResult BeginSearch(SearchRequest request, AsyncCallback callback, object asyncState);
		SearchResponse EndSearch(IAsyncResult result);
	}
	[DataContract(Name = "SearchResponse", Namespace = BingServicesNamespaces.SearchNamespace)]
	public class SearchResponse : ResponseBase {
		QuerySuggestion querySuggestion;
		ObservableCollection<SearchResultSet> resultSets;
		[DataMember]
		public QuerySuggestion QuerySuggestion {
			get { return querySuggestion; }
			set {
				if((object.ReferenceEquals(querySuggestion, value) != true)) {
					querySuggestion = value;
					RaisePropertyChanged("QuerySuggestion");
				}
			}
		}
		[DataMember]
		public ObservableCollection<SearchResultSet> ResultSets {
			get { return resultSets; }
			set {
				if((object.ReferenceEquals(resultSets, value) != true)) {
					resultSets = value;
					RaisePropertyChanged("ResultSets");
				}
			}
		}
	}
	[DataContract(Name = "SearchRequest", Namespace = BingServicesNamespaces.SearchNamespace)]
	public class SearchRequest : RequestBase {
		string query;
		SearchOptions searchOptions;
		StructuredSearchQuery structuredQuery;
		[DataMember]
		public string Query {
			get { return this.query; }
			set {
				if((object.ReferenceEquals(this.query, value) != true)) {
					this.query = value;
					this.RaisePropertyChanged("Query");
				}
			}
		}
		[DataMember]
		public SearchOptions SearchOptions {
			get { return this.searchOptions; }
			set {
				if((object.ReferenceEquals(this.searchOptions, value) != true)) {
					this.searchOptions = value;
					this.RaisePropertyChanged("SearchOptions");
				}
			}
		}
		[DataMember]
		public StructuredSearchQuery StructuredQuery {
			get { return this.structuredQuery; }
			set {
				if((object.ReferenceEquals(this.structuredQuery, value) != true)) {
					this.structuredQuery = value;
					this.RaisePropertyChanged("StructuredQuery");
				}
			}
		}
	}
	[DataContract(Name = "SearchOptions", Namespace = BingServicesNamespaces.SearchNamespace)]
	public class SearchOptions : INotifyPropertyChanged {
		Nullable<bool> autocorrectQuery;
		Nullable<int> count;
		FilterExpressionBase filters;
		ListingType listingType;
		bool parseOnly;
		Nullable<double> radius;
		SortOrder sortOrder;
		int startingIndex;
		[DataMember]
		public Nullable<bool> AutocorrectQuery {
			get { return this.autocorrectQuery; }
			set {
				if((this.autocorrectQuery.Equals(value) != true)) {
					this.autocorrectQuery = value;
					this.RaisePropertyChanged("AutocorrectQuery");
				}
			}
		}
		[DataMember]
		public Nullable<int> Count {
			get { return this.count; }
			set {
				if((this.count.Equals(value) != true)) {
					this.count = value;
					this.RaisePropertyChanged("Count");
				}
			}
		}
		[DataMember]
		public FilterExpressionBase Filters {
			get { return this.filters; }
			set {
				if((object.ReferenceEquals(this.filters, value) != true)) {
					this.filters = value;
					this.RaisePropertyChanged("Filters");
				}
			}
		}
		[DataMember]
		public ListingType ListingType {
			get { return this.listingType; }
			set {
				if((this.listingType.Equals(value) != true)) {
					this.listingType = value;
					this.RaisePropertyChanged("ListingType");
				}
			}
		}
		[DataMember]
		public bool ParseOnly {
			get { return this.parseOnly; }
			set {
				if((this.parseOnly.Equals(value) != true)) {
					this.parseOnly = value;
					this.RaisePropertyChanged("ParseOnly");
				}
			}
		}
		[DataMember]
		public Nullable<double> Radius {
			get { return this.radius; }
			set {
				if((this.radius.Equals(value) != true)) {
					this.radius = value;
					this.RaisePropertyChanged("Radius");
				}
			}
		}
		[DataMember]
		public SortOrder SortOrder {
			get { return this.sortOrder; }
			set {
				if((this.sortOrder.Equals(value) != true)) {
					this.sortOrder = value;
					this.RaisePropertyChanged("SortOrder");
				}
			}
		}
		[DataMember]
		public int StartingIndex {
			get { return this.startingIndex; }
			set {
				if((this.startingIndex.Equals(value) != true)) {
					this.startingIndex = value;
					this.RaisePropertyChanged("StartingIndex");
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if((propertyChanged != null))
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
	[DataContract(Name = "AvailableFilter", Namespace = BingServicesNamespaces.SearchNamespace)]
	[KnownType(typeof(ValueListFilter))]
	[KnownType(typeof(RangeFilter))]
	public class AvailableFilter : INotifyPropertyChanged {
		int propertyId;
		string propertyName;
		[DataMember]
		public int PropertyId {
			get { return this.propertyId; }
			set {
				if((this.propertyId.Equals(value) != true)) {
					this.propertyId = value;
					this.RaisePropertyChanged("PropertyId");
				}
			}
		}
		[DataMember]
		public string PropertyName {
			get { return this.propertyName; }
			set {
				if((object.ReferenceEquals(this.propertyName, value) != true)) {
					this.propertyName = value;
					this.RaisePropertyChanged("PropertyName");
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if((propertyChanged != null))
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
	[DataContract(Name = "ValueListFilter", Namespace = BingServicesNamespaces.SearchNamespace)]
	[KnownType(typeof(SearchRequest))]
	[KnownType(typeof(SearchOptions))]
	[KnownType(typeof(ListingType))]
	[KnownType(typeof(SortOrder))]
	[KnownType(typeof(StructuredSearchQuery))]
	[KnownType(typeof(SearchResponse))]
	[KnownType(typeof(QuerySuggestion))]
	[KnownType(typeof(ObservableCollection<SearchResultSet>))]
	[KnownType(typeof(SearchResultSet))]
	[KnownType(typeof(ObservableCollection<SearchRegion>))]
	[KnownType(typeof(SearchRegion))]
	[KnownType(typeof(ObservableCollection<AvailableFilter>))]
	[KnownType(typeof(AvailableFilter))]
	[KnownType(typeof(Neighborhood))]
	[KnownType(typeof(FilterValue))]
	[KnownType(typeof(Category))]
	[KnownType(typeof(RangeFilter))]
	[KnownType(typeof(ObservableCollection<CategoryCount>))]
	[KnownType(typeof(CategoryCount))]
	[KnownType(typeof(Parse))]
	[KnownType(typeof(ObservableCollection<SearchResultBase>))]
	[KnownType(typeof(SearchResultBase))]
	[KnownType(typeof(LocationData))]
	[KnownType(typeof(CommunityContentSearchResult))]
	[KnownType(typeof(BusinessSearchResult))]
	[KnownType(typeof(ObservableCollection<Category>))]
	[KnownType(typeof(CategorySpecificPropertySet))]
	[KnownType(typeof(ObservableCollection<Neighborhood>))]
	[KnownType(typeof(PersonSearchResult))]
	[KnownType(typeof(ObservableCollection<object>))]
	[KnownType(typeof(ObservableCollection<string>))]
	[KnownType(typeof(Dictionary<Neighborhood, int>))]
	[KnownType(typeof(Dictionary<string, object>))]
	[KnownType(typeof(Dictionary<int, CategorySpecificPropertySet>))]
	[KnownType(typeof(RequestBase))]
	[KnownType(typeof(Credentials))]
	[KnownType(typeof(ExecutionOptions))]
	[KnownType(typeof(UserProfile))]
	[KnownType(typeof(Heading))]
	[KnownType(typeof(UserLocation))]
	[KnownType(typeof(Location))]
	[KnownType(typeof(GeocodeLocation))]
	[KnownType(typeof(Confidence))]
	[KnownType(typeof(DeviceType))]
	[KnownType(typeof(DistanceUnit))]
	[KnownType(typeof(ShapeBase))]
	[KnownType(typeof(Rectangle))]
	[KnownType(typeof(Polygon))]
	[KnownType(typeof(ObservableCollection<Location>))]
	[KnownType(typeof(Circle))]
	[KnownType(typeof(SizeOfint))]
	[KnownType(typeof(FilterExpressionBase))]
	[KnownType(typeof(LogicalOperator))]
	[KnownType(typeof(FilterExpression))]
	[KnownType(typeof(CompareOperator))]
	[KnownType(typeof(FilterExpressionClause))]
	[KnownType(typeof(ObservableCollection<FilterExpressionBase>))]
	[KnownType(typeof(ResponseBase))]
	[KnownType(typeof(ResponseSummary))]
	[KnownType(typeof(AuthenticationResultCode))]
	[KnownType(typeof(ResponseStatusCode))]
	[KnownType(typeof(GeocodeResult))]
	[KnownType(typeof(Address))]
	[KnownType(typeof(ObservableCollection<GeocodeLocation>))]
	public class ValueListFilter : AvailableFilter {
		ObservableCollection<object> values;
		[DataMember]
		public ObservableCollection<object> Values {
			get { return this.values; }
			set {
				if((object.ReferenceEquals(this.values, value) != true)) {
					this.values = value;
					this.RaisePropertyChanged("Values");
				}
			}
		}
	}
	[DataContract(Name = "FilterExpression", Namespace = BingServicesNamespaces.CommonNamespace)]
	[KnownType(typeof(SearchRequest))]
	[KnownType(typeof(SearchOptions))]
	[KnownType(typeof(ListingType))]
	[KnownType(typeof(SortOrder))]
	[KnownType(typeof(StructuredSearchQuery))]
	[KnownType(typeof(SearchResponse))]
	[KnownType(typeof(QuerySuggestion))]
	[KnownType(typeof(ObservableCollection<SearchResultSet>))]
	[KnownType(typeof(SearchResultSet))]
	[KnownType(typeof(ObservableCollection<SearchRegion>))]
	[KnownType(typeof(SearchRegion))]
	[KnownType(typeof(ObservableCollection<AvailableFilter>))]
	[KnownType(typeof(AvailableFilter))]
	[KnownType(typeof(ValueListFilter))]
	[KnownType(typeof(Neighborhood))]
	[KnownType(typeof(FilterValue))]
	[KnownType(typeof(Category))]
	[KnownType(typeof(RangeFilter))]
	[KnownType(typeof(ObservableCollection<CategoryCount>))]
	[KnownType(typeof(CategoryCount))]
	[KnownType(typeof(Parse))]
	[KnownType(typeof(ObservableCollection<SearchResultBase>))]
	[KnownType(typeof(SearchResultBase))]
	[KnownType(typeof(LocationData))]
	[KnownType(typeof(CommunityContentSearchResult))]
	[KnownType(typeof(BusinessSearchResult))]
	[KnownType(typeof(ObservableCollection<Category>))]
	[KnownType(typeof(CategorySpecificPropertySet))]
	[KnownType(typeof(ObservableCollection<Neighborhood>))]
	[KnownType(typeof(PersonSearchResult))]
	[KnownType(typeof(ObservableCollection<object>))]
	[KnownType(typeof(ObservableCollection<string>))]
	[KnownType(typeof(Dictionary<Neighborhood, int>))]
	[KnownType(typeof(Dictionary<string, object>))]
	[KnownType(typeof(Dictionary<int, CategorySpecificPropertySet>))]
	[KnownType(typeof(RequestBase))]
	[KnownType(typeof(Credentials))]
	[KnownType(typeof(ExecutionOptions))]
	[KnownType(typeof(UserProfile))]
	[KnownType(typeof(Heading))]
	[KnownType(typeof(UserLocation))]
	[KnownType(typeof(Location))]
	[KnownType(typeof(GeocodeLocation))]
	[KnownType(typeof(Confidence))]
	[KnownType(typeof(DeviceType))]
	[KnownType(typeof(DistanceUnit))]
	[KnownType(typeof(ShapeBase))]
	[KnownType(typeof(Rectangle))]
	[KnownType(typeof(Polygon))]
	[KnownType(typeof(ObservableCollection<Location>))]
	[KnownType(typeof(Circle))]
	[KnownType(typeof(SizeOfint))]
	[KnownType(typeof(FilterExpressionBase))]
	[KnownType(typeof(LogicalOperator))]
	[KnownType(typeof(CompareOperator))]
	[KnownType(typeof(FilterExpressionClause))]
	[KnownType(typeof(ObservableCollection<FilterExpressionBase>))]
	[KnownType(typeof(ResponseBase))]
	[KnownType(typeof(ResponseSummary))]
	[KnownType(typeof(AuthenticationResultCode))]
	[KnownType(typeof(ResponseStatusCode))]
	[KnownType(typeof(GeocodeResult))]
	[KnownType(typeof(Address))]
	[KnownType(typeof(ObservableCollection<GeocodeLocation>))]
	public class FilterExpression : FilterExpressionBase {
		CompareOperator compareOperator;
		object filterValue;
		int propertyId;
		[DataMember]
		public CompareOperator CompareOperator {
			get { return this.compareOperator; }
			set {
				if((this.compareOperator.Equals(value) != true)) {
					this.compareOperator = value;
					this.RaisePropertyChanged("CompareOperator");
				}
			}
		}
		[DataMember]
		public object FilterValue {
			get { return this.filterValue; }
			set {
				if((object.ReferenceEquals(this.filterValue, value) != true)) {
					this.filterValue = value;
					this.RaisePropertyChanged("FilterValue");
				}
			}
		}
		[DataMember]
		public int PropertyId {
			get { return this.propertyId; }
			set {
				if((this.propertyId.Equals(value) != true)) {
					this.propertyId = value;
					this.RaisePropertyChanged("PropertyId");
				}
			}
		}
	}
	[DataContract(Name = "SearchResultBase", Namespace = BingServicesNamespaces.SearchNamespace)]
	[KnownType(typeof(CommunityContentSearchResult))]
	[KnownType(typeof(BusinessSearchResult))]
	[KnownType(typeof(PersonSearchResult))]
	public class SearchResultBase : INotifyPropertyChanged {
		double distance;
		string id;
		LocationData locationData;
		string name;
		[DataMember]
		public double Distance {
			get { return this.distance; }
			set {
				if((this.distance.Equals(value) != true)) {
					this.distance = value;
					this.RaisePropertyChanged("Distance");
				}
			}
		}
		[DataMember]
		public string Id {
			get { return this.id; }
			set {
				if((object.ReferenceEquals(this.id, value) != true)) {
					this.id = value;
					this.RaisePropertyChanged("Id");
				}
			}
		}
		[DataMember]
		public LocationData LocationData {
			get { return this.locationData; }
			set {
				if((object.ReferenceEquals(this.locationData, value) != true)) {
					this.locationData = value;
					this.RaisePropertyChanged("LocationData");
				}
			}
		}
		[DataMember]
		public string Name {
			get { return this.name; }
			set {
				if((object.ReferenceEquals(this.name, value) != true)) {
					this.name = value;
					this.RaisePropertyChanged("Name");
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if((propertyChanged != null))
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
	[DataContract(Name = "LocationData", Namespace = BingServicesNamespaces.SearchNamespace)]
	public class LocationData : INotifyPropertyChanged {
		Confidence confidence;
		ObservableCollection<GeocodeLocation> locations;
		ObservableCollection<string> matchCodes;
		[DataMember]
		public Confidence Confidence {
			get { return this.confidence; }
			set {
				if((this.confidence.Equals(value) != true)) {
					this.confidence = value;
					this.RaisePropertyChanged("Confidence");
				}
			}
		}
		[DataMember]
		public ObservableCollection<GeocodeLocation> Locations {
			get { return this.locations; }
			set {
				if((object.ReferenceEquals(this.locations, value) != true)) {
					this.locations = value;
					this.RaisePropertyChanged("Locations");
				}
			}
		}
		[DataMember]
		public ObservableCollection<string> MatchCodes {
			get { return this.matchCodes; }
			set {
				if((object.ReferenceEquals(this.matchCodes, value) != true)) {
					this.matchCodes = value;
					this.RaisePropertyChanged("MatchCodes");
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if((propertyChanged != null))
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
	[DataContract(Name = "CommunityContentSearchResult", Namespace = BingServicesNamespaces.SearchNamespace)]
	public class CommunityContentSearchResult : SearchResultBase {
		Dictionary<string, object> additionalProperties;
		string description;
		[DataMember]
		public Dictionary<string, object> AdditionalProperties {
			get { return this.additionalProperties; }
			set {
				if((object.ReferenceEquals(this.additionalProperties, value) != true)) {
					this.additionalProperties = value;
					this.RaisePropertyChanged("AdditionalProperties");
				}
			}
		}
		[DataMember]
		public string Description {
			get { return this.description; }
			set {
				if((object.ReferenceEquals(this.description, value) != true)) {
					this.description = value;
					this.RaisePropertyChanged("Description");
				}
			}
		}
	}
	[DataContract(Name = "BusinessSearchResult", Namespace = BingServicesNamespaces.SearchNamespace)]
	public class BusinessSearchResult : SearchResultBase {
		Dictionary<string, object> additionalProperties;
		Address address;
		ObservableCollection<Category> categories;
		Dictionary<int, CategorySpecificPropertySet> categorySpecificProperties;
		ObservableCollection<Neighborhood> neighborhoods;
		string phoneNumber;
		int ratingCount;
		int reviewCount;
		double userRating;
		Uri website;
		[DataMember]
		public Dictionary<string, object> AdditionalProperties {
			get { return this.additionalProperties; }
			set {
				if((object.ReferenceEquals(this.additionalProperties, value) != true)) {
					this.additionalProperties = value;
					this.RaisePropertyChanged("AdditionalProperties");
				}
			}
		}
		[DataMember]
		public Address Address {
			get { return this.address; }
			set {
				if((object.ReferenceEquals(this.address, value) != true)) {
					this.address = value;
					this.RaisePropertyChanged("Address");
				}
			}
		}
		[DataMember]
		public ObservableCollection<Category> Categories {
			get { return this.categories; }
			set {
				if((object.ReferenceEquals(this.categories, value) != true)) {
					this.categories = value;
					this.RaisePropertyChanged("Categories");
				}
			}
		}
		[DataMember]
		public Dictionary<int, CategorySpecificPropertySet> CategorySpecificProperties {
			get { return this.categorySpecificProperties; }
			set {
				if((object.ReferenceEquals(this.categorySpecificProperties, value) != true)) {
					this.categorySpecificProperties = value;
					this.RaisePropertyChanged("CategorySpecificProperties");
				}
			}
		}
		[DataMember]
		public ObservableCollection<Neighborhood> Neighborhoods {
			get { return this.neighborhoods; }
			set {
				if((object.ReferenceEquals(this.neighborhoods, value) != true)) {
					this.neighborhoods = value;
					this.RaisePropertyChanged("Neighborhoods");
				}
			}
		}
		[DataMember]
		public string PhoneNumber {
			get { return this.phoneNumber; }
			set {
				if((object.ReferenceEquals(this.phoneNumber, value) != true)) {
					this.phoneNumber = value;
					this.RaisePropertyChanged("PhoneNumber");
				}
			}
		}
		[DataMember]
		public int RatingCount {
			get { return this.ratingCount; }
			set {
				if((this.ratingCount.Equals(value) != true)) {
					this.ratingCount = value;
					this.RaisePropertyChanged("RatingCount");
				}
			}
		}
		[DataMember]
		public int ReviewCount {
			get { return this.reviewCount; }
			set {
				if((this.reviewCount.Equals(value) != true)) {
					this.reviewCount = value;
					this.RaisePropertyChanged("ReviewCount");
				}
			}
		}
		[DataMember]
		public double UserRating {
			get { return this.userRating; }
			set {
				if((this.userRating.Equals(value) != true)) {
					this.userRating = value;
					this.RaisePropertyChanged("UserRating");
				}
			}
		}
		[DataMember]
		public Uri Website {
			get { return this.website; }
			set {
				if((object.ReferenceEquals(this.website, value) != true)) {
					this.website = value;
					this.RaisePropertyChanged("Website");
				}
			}
		}
	}
	[DataContract(Name = "CategorySpecificPropertySet", Namespace = BingServicesNamespaces.SearchNamespace)]
	public class CategorySpecificPropertySet : INotifyPropertyChanged {
		string categoryName;
		Dictionary<string, object> properties;
		[DataMember]
		public string CategoryName {
			get { return this.categoryName; }
			set {
				if((object.ReferenceEquals(this.categoryName, value) != true)) {
					this.categoryName = value;
					this.RaisePropertyChanged("CategoryName");
				}
			}
		}
		[DataMember]
		public Dictionary<string, object> Properties {
			get { return this.properties; }
			set {
				if((object.ReferenceEquals(this.properties, value) != true)) {
					this.properties = value;
					this.RaisePropertyChanged("Properties");
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if((propertyChanged != null))
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
	[DataContract(Name = "PersonSearchResult", Namespace = BingServicesNamespaces.SearchNamespace)]
	public class PersonSearchResult : SearchResultBase {
		Address address;
		string phoneNumber;
		[DataMember]
		public Address Address {
			get { return this.address; }
			set {
				if((object.ReferenceEquals(this.address, value) != true)) {
					this.address = value;
					this.RaisePropertyChanged("Address");
				}
			}
		}
		[DataMember]
		public string PhoneNumber {
			get { return this.phoneNumber; }
			set {
				if((object.ReferenceEquals(this.phoneNumber, value) != true)) {
					this.phoneNumber = value;
					this.RaisePropertyChanged("PhoneNumber");
				}
			}
		}
	}
	[DataContract(Name = "Parse", Namespace = BingServicesNamespaces.SearchNamespace)]
	public class Parse : INotifyPropertyChanged {
		Address address;
		string keyword;
		string landmark;
		string locationSeparator;
		[DataMember]
		public Address Address {
			get { return this.address; }
			set {
				if((object.ReferenceEquals(this.address, value) != true)) {
					this.address = value;
					this.RaisePropertyChanged("Address");
				}
			}
		}
		[DataMember]
		public string Keyword {
			get { return this.keyword; }
			set {
				if((object.ReferenceEquals(this.keyword, value) != true)) {
					this.keyword = value;
					this.RaisePropertyChanged("Keyword");
				}
			}
		}
		[DataMember]
		public string Landmark {
			get { return this.landmark; }
			set {
				if((object.ReferenceEquals(this.landmark, value) != true)) {
					this.landmark = value;
					this.RaisePropertyChanged("Landmark");
				}
			}
		}
		[DataMember]
		public string LocationSeparator {
			get { return this.locationSeparator; }
			set {
				if((object.ReferenceEquals(this.locationSeparator, value) != true)) {
					this.locationSeparator = value;
					this.RaisePropertyChanged("LocationSeparator");
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if((propertyChanged != null))
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
	[DataContract(Name = "RangeFilter", Namespace = BingServicesNamespaces.SearchNamespace)]
	[KnownType(typeof(SearchRequest))]
	[KnownType(typeof(SearchOptions))]
	[KnownType(typeof(ListingType))]
	[KnownType(typeof(SortOrder))]
	[KnownType(typeof(StructuredSearchQuery))]
	[KnownType(typeof(SearchResponse))]
	[KnownType(typeof(QuerySuggestion))]
	[KnownType(typeof(ObservableCollection<SearchResultSet>))]
	[KnownType(typeof(SearchResultSet))]
	[KnownType(typeof(ObservableCollection<SearchRegion>))]
	[KnownType(typeof(SearchRegion))]
	[KnownType(typeof(ObservableCollection<AvailableFilter>))]
	[KnownType(typeof(AvailableFilter))]
	[KnownType(typeof(ValueListFilter))]
	[KnownType(typeof(Neighborhood))]
	[KnownType(typeof(FilterValue))]
	[KnownType(typeof(Category))]
	[KnownType(typeof(ObservableCollection<CategoryCount>))]
	[KnownType(typeof(CategoryCount))]
	[KnownType(typeof(Parse))]
	[KnownType(typeof(ObservableCollection<SearchResultBase>))]
	[KnownType(typeof(SearchResultBase))]
	[KnownType(typeof(LocationData))]
	[KnownType(typeof(CommunityContentSearchResult))]
	[KnownType(typeof(BusinessSearchResult))]
	[KnownType(typeof(ObservableCollection<Category>))]
	[KnownType(typeof(CategorySpecificPropertySet))]
	[KnownType(typeof(ObservableCollection<Neighborhood>))]
	[KnownType(typeof(PersonSearchResult))]
	[KnownType(typeof(ObservableCollection<object>))]
	[KnownType(typeof(ObservableCollection<string>))]
	[KnownType(typeof(Dictionary<Neighborhood, int>))]
	[KnownType(typeof(Dictionary<string, object>))]
	[KnownType(typeof(Dictionary<int, CategorySpecificPropertySet>))]
	[KnownType(typeof(RequestBase))]
	[KnownType(typeof(Credentials))]
	[KnownType(typeof(ExecutionOptions))]
	[KnownType(typeof(UserProfile))]
	[KnownType(typeof(Heading))]
	[KnownType(typeof(UserLocation))]
	[KnownType(typeof(Location))]
	[KnownType(typeof(GeocodeLocation))]
	[KnownType(typeof(Confidence))]
	[KnownType(typeof(DeviceType))]
	[KnownType(typeof(DistanceUnit))]
	[KnownType(typeof(ShapeBase))]
	[KnownType(typeof(Rectangle))]
	[KnownType(typeof(Polygon))]
	[KnownType(typeof(ObservableCollection<Location>))]
	[KnownType(typeof(Circle))]
	[KnownType(typeof(SizeOfint))]
	[KnownType(typeof(FilterExpressionBase))]
	[KnownType(typeof(LogicalOperator))]
	[KnownType(typeof(FilterExpression))]
	[KnownType(typeof(CompareOperator))]
	[KnownType(typeof(FilterExpressionClause))]
	[KnownType(typeof(ObservableCollection<FilterExpressionBase>))]
	[KnownType(typeof(ResponseBase))]
	[KnownType(typeof(ResponseSummary))]
	[KnownType(typeof(AuthenticationResultCode))]
	[KnownType(typeof(ResponseStatusCode))]
	[KnownType(typeof(GeocodeResult))]
	[KnownType(typeof(Address))]
	[KnownType(typeof(ObservableCollection<GeocodeLocation>))]
	public class RangeFilter : AvailableFilter {
		object maximumValue;
		object minimumValue;
		[DataMember]
		public object MaximumValue {
			get { return this.maximumValue; }
			set {
				if((object.ReferenceEquals(this.maximumValue, value) != true)) {
					this.maximumValue = value;
					this.RaisePropertyChanged("MaximumValue");
				}
			}
		}
		[DataMember]
		public object MinimumValue {
			get { return this.minimumValue; }
			set {
				if((object.ReferenceEquals(this.minimumValue, value) != true)) {
					this.minimumValue = value;
					this.RaisePropertyChanged("MinimumValue");
				}
			}
		}
	}
	[DataContract(Name = "CategoryCount", Namespace = BingServicesNamespaces.SearchNamespace)]
	public class CategoryCount : INotifyPropertyChanged {
		Category category;
		int count;
		Dictionary<Neighborhood, int> countByNeighborhood;
		[DataMember]
		public Category Category {
			get { return this.category; }
			set {
				if((object.ReferenceEquals(this.category, value) != true)) {
					this.category = value;
					this.RaisePropertyChanged("Category");
				}
			}
		}
		[DataMember]
		public int Count {
			get { return this.count; }
			set {
				if((this.count.Equals(value) != true)) {
					this.count = value;
					this.RaisePropertyChanged("Count");
				}
			}
		}
		[DataMember]
		public Dictionary<Neighborhood, int> CountByNeighborhood {
			get { return this.countByNeighborhood; }
			set {
				if((object.ReferenceEquals(this.countByNeighborhood, value) != true)) {
					this.countByNeighborhood = value;
					this.RaisePropertyChanged("CountByNeighborhood");
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if((propertyChanged != null))
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
	[DataContract(Name = "Category", Namespace = BingServicesNamespaces.SearchNamespace)]
	public class Category : INotifyPropertyChanged {
		int id;
		string name;
		[DataMember]
		public int Id {
			get { return this.id; }
			set {
				if((this.id.Equals(value) != true)) {
					this.id = value;
					this.RaisePropertyChanged("Id");
				}
			}
		}
		[DataMember]
		public string Name {
			get { return this.name; }
			set {
				if((object.ReferenceEquals(this.name, value) != true)) {
					this.name = value;
					this.RaisePropertyChanged("Name");
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if((propertyChanged != null))
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
	[DataContract(Name = "FilterValue", Namespace = BingServicesNamespaces.SearchNamespace)]
	public class FilterValue : INotifyPropertyChanged {
		int id;
		string name;
		[DataMember]
		public int Id {
			get { return this.id; }
			set {
				if((this.id.Equals(value) != true)) {
					this.id = value;
					this.RaisePropertyChanged("Id");
				}
			}
		}
		[DataMember]
		public string Name {
			get { return this.name; }
			set {
				if((object.ReferenceEquals(this.name, value) != true)) {
					this.name = value;
					this.RaisePropertyChanged("Name");
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if((propertyChanged != null))
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
	[DataContract(Name = "Neighborhood", Namespace = BingServicesNamespaces.SearchNamespace)]
	public class Neighborhood : INotifyPropertyChanged {
		string city;
		int id;
		string name;
		[DataMember]
		public string City {
			get { return this.city; }
			set {
				if((object.ReferenceEquals(this.city, value) != true)) {
					this.city = value;
					this.RaisePropertyChanged("City");
				}
			}
		}
		[DataMember]
		public int Id {
			get { return this.id; }
			set {
				if((this.id.Equals(value) != true)) {
					this.id = value;
					this.RaisePropertyChanged("Id");
				}
			}
		}
		[DataMember]
		public string Name {
			get { return this.name; }
			set {
				if((object.ReferenceEquals(this.name, value) != true)) {
					this.name = value;
					this.RaisePropertyChanged("Name");
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if((propertyChanged != null))
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
	[DataContract(Name = "FilterExpressionBase", Namespace = BingServicesNamespaces.CommonNamespace)]
	[KnownType(typeof(FilterExpression))]
	[KnownType(typeof(FilterExpressionClause))]
	public class FilterExpressionBase : INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if((propertyChanged != null))
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
	[DataContract(Name = "FilterExpressionClause", Namespace = BingServicesNamespaces.CommonNamespace)]
	public class FilterExpressionClause : FilterExpressionBase {
		ObservableCollection<FilterExpressionBase> expressions;
		LogicalOperator logicalOperator;
		[DataMember]
		public ObservableCollection<FilterExpressionBase> Expressions {
			get { return this.expressions; }
			set {
				if((object.ReferenceEquals(this.expressions, value) != true)) {
					this.expressions = value;
					this.RaisePropertyChanged("Expressions");
				}
			}
		}
		[DataMember]
		public LogicalOperator LogicalOperator {
			get { return this.logicalOperator; }
			set {
				if((this.logicalOperator.Equals(value) != true)) {
					this.logicalOperator = value;
					this.RaisePropertyChanged("LogicalOperator");
				}
			}
		}
	}
	[DataContract(Name = "QuerySuggestion", Namespace = BingServicesNamespaces.SearchNamespace)]
	public class QuerySuggestion : INotifyPropertyChanged {
		string query;
		StructuredSearchQuery structuredQuery;
		[DataMember]
		public string Query {
			get { return this.query; }
			set {
				if((object.ReferenceEquals(this.query, value) != true)) {
					this.query = value;
					this.RaisePropertyChanged("Query");
				}
			}
		}
		[DataMember]
		public StructuredSearchQuery StructuredQuery {
			get { return this.structuredQuery; }
			set {
				if((object.ReferenceEquals(this.structuredQuery, value) != true)) {
					this.structuredQuery = value;
					this.RaisePropertyChanged("StructuredQuery");
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if((propertyChanged != null))
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
	[DataContract(Name = "SearchResultSet", Namespace = BingServicesNamespaces.SearchNamespace)]
	public class SearchResultSet : INotifyPropertyChanged {
		ObservableCollection<SearchRegion> alternateSearchRegions;
		ObservableCollection<AvailableFilter> availableFilters;
		ObservableCollection<CategoryCount> categoryCounts;
		int estimatedMatches;
		string listingType;
		Parse parse;
		Category queryCategory;
		bool queryCorrected;
		ObservableCollection<SearchResultBase> results;
		SearchRegion searchRegion;
		[DataMember]
		public ObservableCollection<SearchRegion> AlternateSearchRegions {
			get { return this.alternateSearchRegions; }
			set {
				if((object.ReferenceEquals(this.alternateSearchRegions, value) != true)) {
					this.alternateSearchRegions = value;
					this.RaisePropertyChanged("AlternateSearchRegions");
				}
			}
		}
		[DataMember]
		public ObservableCollection<AvailableFilter> AvailableFilters {
			get { return this.availableFilters; }
			set {
				if((object.ReferenceEquals(this.availableFilters, value) != true)) {
					this.availableFilters = value;
					this.RaisePropertyChanged("AvailableFilters");
				}
			}
		}
		[DataMember]
		public ObservableCollection<CategoryCount> CategoryCounts {
			get { return this.categoryCounts; }
			set {
				if((object.ReferenceEquals(this.categoryCounts, value) != true)) {
					this.categoryCounts = value;
					this.RaisePropertyChanged("CategoryCounts");
				}
			}
		}
		[DataMember]
		public int EstimatedMatches {
			get { return this.estimatedMatches; }
			set {
				if((this.estimatedMatches.Equals(value) != true)) {
					this.estimatedMatches = value;
					this.RaisePropertyChanged("EstimatedMatches");
				}
			}
		}
		[DataMember]
		public string ListingType {
			get { return this.listingType; }
			set {
				if((object.ReferenceEquals(this.listingType, value) != true)) {
					this.listingType = value;
					this.RaisePropertyChanged("ListingType");
				}
			}
		}
		[DataMember]
		public Parse Parse {
			get { return this.parse; }
			set {
				if((object.ReferenceEquals(this.parse, value) != true)) {
					this.parse = value;
					this.RaisePropertyChanged("Parse");
				}
			}
		}
		[DataMember]
		public Category QueryCategory {
			get { return this.queryCategory; }
			set {
				if((object.ReferenceEquals(this.queryCategory, value) != true)) {
					this.queryCategory = value;
					this.RaisePropertyChanged("QueryCategory");
				}
			}
		}
		[DataMember]
		public bool QueryCorrected {
			get { return this.queryCorrected; }
			set {
				if((this.queryCorrected.Equals(value) != true)) {
					this.queryCorrected = value;
					this.RaisePropertyChanged("QueryCorrected");
				}
			}
		}
		[DataMember]
		public ObservableCollection<SearchResultBase> Results {
			get { return this.results; }
			set {
				if((object.ReferenceEquals(this.results, value) != true)) {
					this.results = value;
					this.RaisePropertyChanged("Results");
				}
			}
		}
		[DataMember]
		public SearchRegion SearchRegion {
			get { return this.searchRegion; }
			set {
				if((object.ReferenceEquals(this.searchRegion, value) != true)) {
					this.searchRegion = value;
					this.RaisePropertyChanged("SearchRegion");
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if((propertyChanged != null))
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
	[DataContract(Name = "StructuredSearchQuery", Namespace = BingServicesNamespaces.SearchNamespace)]
	public class StructuredSearchQuery : INotifyPropertyChanged {
		string keyword;
		string location;
		[DataMember]
		public string Keyword {
			get { return this.keyword; }
			set {
				if((object.ReferenceEquals(this.keyword, value) != true)) {
					this.keyword = value;
					this.RaisePropertyChanged("Keyword");
				}
			}
		}
		[DataMember]
		public string Location {
			get { return this.location; }
			set {
				if((object.ReferenceEquals(this.location, value) != true)) {
					this.location = value;
					this.RaisePropertyChanged("Location");
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if((propertyChanged != null))
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
	[DataContract(Name = "SearchRegion", Namespace = BingServicesNamespaces.SearchNamespace)]
	public class SearchRegion : INotifyPropertyChanged {
		ShapeBase boundingArea;
		GeocodeResult geocodeLocation;
		string source;
		[DataMember]
		public ShapeBase BoundingArea {
			get { return this.boundingArea; }
			set {
				if((object.ReferenceEquals(this.boundingArea, value) != true)) {
					this.boundingArea = value;
					this.RaisePropertyChanged("BoundingArea");
				}
			}
		}
		[DataMember]
		public GeocodeResult GeocodeLocation {
			get { return this.geocodeLocation; }
			set {
				if((object.ReferenceEquals(this.geocodeLocation, value) != true)) {
					this.geocodeLocation = value;
					this.RaisePropertyChanged("GeocodeLocation");
				}
			}
		}
		[DataMember]
		public string Source {
			get { return this.source; }
			set {
				if((object.ReferenceEquals(this.source, value) != true)) {
					this.source = value;
					this.RaisePropertyChanged("Source");
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if((propertyChanged != null))
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
	public class SearchServiceClient : ClientBase<ISearchService>, ISearchService {
		BeginOperationDelegate onBeginSearchDelegate;
		EndOperationDelegate onEndSearchDelegate;
		SendOrPostCallback onSearchCompletedDelegate;
		public SearchServiceClient(Binding binding, EndpointAddress remoteAddress) :
			base(binding, remoteAddress) {
		}
		public event EventHandler<SearchCompletedEventArgs> SearchCompleted;
		public event EventHandler<AsyncCompletedEventArgs> OpenCompleted;
		public event EventHandler<AsyncCompletedEventArgs> CloseCompleted;
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		IAsyncResult ISearchService.BeginSearch(SearchRequest request, AsyncCallback callback, object asyncState) {
			return base.Channel.BeginSearch(request, callback, asyncState);
		}
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		SearchResponse ISearchService.EndSearch(IAsyncResult result) {
			return base.Channel.EndSearch(result);
		}
		IAsyncResult OnBeginSearch(object[] inValues, AsyncCallback callback, object asyncState) {
			SearchRequest request = ((SearchRequest)(inValues[0]));
			return ((ISearchService)(this)).BeginSearch(request, callback, asyncState);
		}
		object[] OnEndSearch(IAsyncResult result) {
			SearchResponse retVal = ((ISearchService)(this)).EndSearch(result);
			return new object[] { retVal };
		}
		void OnSearchCompleted(object state) {
			if((this.SearchCompleted != null)) {
				InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
				this.SearchCompleted(this, new SearchCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
			}
		}
		[SuppressMessage("Microsoft.Performance", "CA1811:Avoid uncalled private code")]
		IAsyncResult OnBeginOpen(AsyncCallback callback, object asyncState) {
			return ((ICommunicationObject)(this)).BeginOpen(callback, asyncState);
		}
		[SuppressMessage("Microsoft.Performance", "CA1811:Avoid uncalled private code")]
		object[] OnEndOpen(IAsyncResult result) {
			((ICommunicationObject)(this)).EndOpen(result);
			return null;
		}
		[SuppressMessage("Microsoft.Performance", "CA1811:Avoid uncalled private code")]
		void OnOpenCompleted(object state) {
			if((this.OpenCompleted != null)) {
				InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
				this.OpenCompleted(this, new AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
			}
		}
		[SuppressMessage("Microsoft.Performance", "CA1811:Avoid uncalled private code")]
		IAsyncResult OnBeginClose(AsyncCallback callback, object asyncState) {
			return ((ICommunicationObject)(this)).BeginClose(callback, asyncState);
		}
		[SuppressMessage("Microsoft.Performance", "CA1811:Avoid uncalled private code")]
		object[] OnEndClose(IAsyncResult result) {
			((ICommunicationObject)(this)).EndClose(result);
			return null;
		}
		[SuppressMessage("Microsoft.Performance", "CA1811:Avoid uncalled private code")]
		void OnCloseCompleted(object state) {
			if((this.CloseCompleted != null)) {
				InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
				this.CloseCompleted(this, new AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
			}
		}
		public void SearchAsync(SearchRequest request) {
			this.SearchAsync(request, null);
		}
		public void SearchAsync(SearchRequest request, object userState) {
			if((this.onBeginSearchDelegate == null))
				this.onBeginSearchDelegate = new BeginOperationDelegate(this.OnBeginSearch);
			if((this.onEndSearchDelegate == null))
				this.onEndSearchDelegate = new EndOperationDelegate(this.OnEndSearch);
			if((this.onSearchCompletedDelegate == null))
				this.onSearchCompletedDelegate = new SendOrPostCallback(this.OnSearchCompleted);
			base.InvokeAsync(this.onBeginSearchDelegate, new object[] { request },
				this.onEndSearchDelegate, this.onSearchCompletedDelegate, userState);
		}
		public SearchResponse Search(SearchRequest request) {
			return base.Channel.Search(request);
		}
	}
}
namespace DevExpress.Map.Native {
	public class BingSearchServiceInfo : BingCommonServiceInfo {
		public BingSearchServiceInfo(XDocument xDocument)
			: base(xDocument) {
		}
	}
}
