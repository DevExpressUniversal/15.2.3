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

using System.CodeDom.Compiler;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System;
using System.Threading;
using System.Net;
using System.ServiceModel.Channels;
using System.Xml.Linq;
namespace DevExpress.Map.BingServices {
	[DataContract(Name = "FilterBase", Namespace = BingServicesNamespaces.GeocodeNamespace)]
	[KnownType(typeof(ConfidenceFilter))]
	public class FilterBase : INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if((propertyChanged != null)) {
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	[DataContract(Name = "ConfidenceFilter", Namespace = BingServicesNamespaces.GeocodeNamespace)]
	public class ConfidenceFilter : FilterBase {
		private Confidence minimumConfidence;
		[DataMember]
		public Confidence MinimumConfidence {
			get {
				return minimumConfidence;
			}
			set {
				if(minimumConfidence != value) {
					minimumConfidence = value;
					RaisePropertyChanged("MinimumConfidence");
				}
			}
		}
	}
	[DataContract(Name = "GeocodeOptions", Namespace = BingServicesNamespaces.GeocodeNamespace)]
	public class GeocodeOptions : INotifyPropertyChanged {
		Nullable<int> count;
		ObservableCollection<FilterBase> filters;
		[DataMember]
		public Nullable<int> Count {
			get {
				return count;
			}
			set {
				if(count != value) {
					count = value;
					RaisePropertyChanged("Count");
				}
			}
		}
		[DataMember]
		public ObservableCollection<FilterBase> Filters {
			get {
				return filters;
			}
			set {
				if(filters != value) {
					filters = value;
					this.RaisePropertyChanged("Filters");
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			PropertyChangedEventHandler propertyChanged = PropertyChanged;
			if((propertyChanged != null)) {
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	[DataContract(Name = "GeocodeResult", Namespace = BingServicesNamespaces.CommonNamespace)]
	public class GeocodeResult : INotifyPropertyChanged {
		string displayName;
		string entityType;
		Address address;
		Rectangle bestView;
		Confidence confidence;
		ObservableCollection<GeocodeLocation> locations;
		ObservableCollection<string> matchCodes;
		[DataMember]
		public Address Address {
			get {
				return address;
			}
			set {
				if(address != value) {
					address = value;
					RaisePropertyChanged("Address");
				}
			}
		}
		[DataMember]
		public Rectangle BestView {
			get {
				return bestView;
			}
			set {
				if(bestView != value) {
					bestView = value;
					RaisePropertyChanged("BestView");
				}
			}
		}
		[DataMember]
		public Confidence Confidence {
			get {
				return confidence;
			}
			set {
				if(confidence != value) {
					confidence = value;
					RaisePropertyChanged("Confidence");
				}
			}
		}
		[DataMember]
		public string DisplayName {
			get {
				return displayName;
			}
			set {
				if(displayName != value) {
					displayName = value;
					RaisePropertyChanged("DisplayName");
				}
			}
		}
		[DataMember]
		public string EntityType {
			get {
				return entityType;
			}
			set {
				if(entityType != value) {
					entityType = value;
					RaisePropertyChanged("EntityType");
				}
			}
		}
		[DataMember]
		public ObservableCollection<GeocodeLocation> Locations {
			get {
				return locations;
			}
			set {
				if(locations != value) {
					locations = value;
					RaisePropertyChanged("Locations");
				}
			}
		}
		[DataMember]
		public ObservableCollection<string> MatchCodes {
			get {
				return matchCodes;
			}
			set {
				if(matchCodes != value) {
					matchCodes = value;
					RaisePropertyChanged("MatchCodes");
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if((propertyChanged != null)) {
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	[DataContract(Name = "GeocodeRequest", Namespace = BingServicesNamespaces.GeocodeNamespace)]
	public class GeocodeRequest : RequestBase {
		string query;
		Address address;
		GeocodeOptions options;
		[DataMember]
		public Address Address {
			get {
				return address;
			}
			set {
				if(address != value) {
					address = value;
					RaisePropertyChanged("Address");
				}
			}
		}
		[DataMember]
		public GeocodeOptions Options {
			get {
				return options;
			}
			set {
				if(options != value) {
					options = value;
					RaisePropertyChanged("Options");
				}
			}
		}
		[DataMember]
		public string Query {
			get {
				return this.query;
			}
			set {
				if(query != value) {
					query = value;
					RaisePropertyChanged("Query");
				}
			}
		}
	}
	[DataContract(Name = "ReverseGeocodeRequest", Namespace = BingServicesNamespaces.GeocodeNamespace)]
	public class ReverseGeocodeRequest : RequestBase {
		Location location;
		[DataMember]
		public Location Location {
			get {
				return location;
			}
			set {
				if(location != value) {
					location = value;
					RaisePropertyChanged("Location");
				}
			}
		}
	}
	[DataContract(Name = "GeocodeResponse", Namespace = BingServicesNamespaces.GeocodeNamespace)]
	public class GeocodeResponse : ResponseBase {
		private ObservableCollection<GeocodeResult> results;
		[DataMember]
		public ObservableCollection<GeocodeResult> Results {
			get {
				return results;
			}
			set {
				if(results != value) {
					results = value;
					RaisePropertyChanged("Results");
				}
			}
		}
	}
	[ServiceContract(Namespace = BingServicesNamespaces.GeocodeContractNamespace, ConfigurationName = "GeoCode.IGeocodeService")]
	public interface IGeocodeService {
		[OperationContract(AsyncPattern = true,
						   Action = BingServicesNamespaces.GeocodeContractNamespace + "/IGeocodeService/Geocode",
						   ReplyAction = BingServicesNamespaces.GeocodeContractNamespace + "/IGeocodeService/GeocodeResponse")]
		[FaultContract(typeof(ResponseSummary),
					   Action = BingServicesNamespaces.GeocodeContractNamespace + "/IGeocodeService/GeocodeResponseSummaryFault",
					   Name = "ResponseSummary",
					   Namespace = BingServicesNamespaces.CommonNamespace)]
		IAsyncResult BeginGeocode(GeocodeRequest request, AsyncCallback callback, object asyncState);
		GeocodeResponse EndGeocode(IAsyncResult result);
		[OperationContract(AsyncPattern = true,
						   Action = BingServicesNamespaces.GeocodeContractNamespace + "/IGeocodeService/ReverseGeocode",
						   ReplyAction = BingServicesNamespaces.GeocodeContractNamespace + "/contracts/IGeocodeService/ReverseGeocodeResponse")]
		[FaultContract(typeof(ResponseSummary),
					   Action = BingServicesNamespaces.GeocodeContractNamespace + "/IGeocodeService/ReverseGeocodeResponseSummaryFault",
					   Name = "ResponseSummary",
					   Namespace = BingServicesNamespaces.CommonNamespace)]
		IAsyncResult BeginReverseGeocode(ReverseGeocodeRequest request, AsyncCallback callback, object asyncState);
		GeocodeResponse EndReverseGeocode(IAsyncResult result);
	}
	public interface IGeocodeServiceChannel : IGeocodeService, IClientChannel {
	}
	public class GeocodeCompletedEventArgs : AsyncCompletedEventArgs {
		object[] results;
		public GeocodeCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState)
			: base(exception, cancelled, userState) {
			this.results = results;
		}
		public GeocodeResponse Result {
			get {
				RaiseExceptionIfNecessary();
				return (GeocodeResponse)results[0];
			}
		}
	}
	public class ReverseGeocodeCompletedEventArgs : AsyncCompletedEventArgs {
		object[] results;
		public ReverseGeocodeCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState)
			: base(exception, cancelled, userState) {
			this.results = results;
		}
		public GeocodeResponse Result {
			get {
				try {
					base.RaiseExceptionIfNecessary();
				} catch {
					return null;
				}
				return (GeocodeResponse)results[0];
			}
		}
	}
	public class GeocodeServiceClient : ClientBase<IGeocodeService>, IGeocodeService {
		BeginOperationDelegate onBeginGeocodeDelegate;
		EndOperationDelegate onEndGeocodeDelegate;
		SendOrPostCallback onGeocodeCompletedDelegate;
		BeginOperationDelegate onBeginReverseGeocodeDelegate;
		EndOperationDelegate onEndReverseGeocodeDelegate;
		SendOrPostCallback onReverseGeocodeCompletedDelegate;
		BeginOperationDelegate onBeginOpenDelegate;
		EndOperationDelegate onEndOpenDelegate;
		SendOrPostCallback onOpenCompletedDelegate;
		BeginOperationDelegate onBeginCloseDelegate;
		EndOperationDelegate onEndCloseDelegate;
		SendOrPostCallback onCloseCompletedDelegate;
		public GeocodeServiceClient(Binding binding, EndpointAddress remoteAddress)
			: base(binding, remoteAddress) {
		}
		public event EventHandler<GeocodeCompletedEventArgs> GeocodeCompleted;
		public event EventHandler<ReverseGeocodeCompletedEventArgs> ReverseGeocodeCompleted;
		public event EventHandler<AsyncCompletedEventArgs> OpenCompleted;
		public event EventHandler<AsyncCompletedEventArgs> CloseCompleted;
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		IAsyncResult IGeocodeService.BeginGeocode(GeocodeRequest request, AsyncCallback callback, object asyncState) {
			return Channel.BeginGeocode(request, callback, asyncState);
		}
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		GeocodeResponse IGeocodeService.EndGeocode(IAsyncResult result) {
			return Channel.EndGeocode(result);
		}
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		IAsyncResult IGeocodeService.BeginReverseGeocode(ReverseGeocodeRequest request, AsyncCallback callback, object asyncState) {
			return Channel.BeginReverseGeocode(request, callback, asyncState);
		}
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		GeocodeResponse IGeocodeService.EndReverseGeocode(IAsyncResult result) {
			return Channel.EndReverseGeocode(result);
		}
		IAsyncResult OnBeginGeocode(object[] inValues, AsyncCallback callback, object asyncState) {
			GeocodeRequest request = (GeocodeRequest)inValues[0];
			return ((IGeocodeService)this).BeginGeocode(request, callback, asyncState);
		}
		object[] OnEndGeocode(IAsyncResult result) {
			GeocodeResponse retVal = ((IGeocodeService)this).EndGeocode(result);
			return new object[] { retVal };
		}
		void OnGeocodeCompleted(object state) {
			if(GeocodeCompleted != null) {
				InvokeAsyncCompletedEventArgs e = (InvokeAsyncCompletedEventArgs)state;
				GeocodeCompleted(this, new GeocodeCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
			}
		}
		IAsyncResult OnBeginReverseGeocode(object[] inValues, AsyncCallback callback, object asyncState) {
			ReverseGeocodeRequest request = (ReverseGeocodeRequest)inValues[0];
			return ((IGeocodeService)this).BeginReverseGeocode(request, callback, asyncState);
		}
		object[] OnEndReverseGeocode(IAsyncResult result) {
			GeocodeResponse retVal = ((IGeocodeService)this).EndReverseGeocode(result);
			return new object[] { retVal };
		}
		void OnReverseGeocodeCompleted(object state) {
			if(ReverseGeocodeCompleted != null) {
				InvokeAsyncCompletedEventArgs e = (InvokeAsyncCompletedEventArgs)state;
				ReverseGeocodeCompleted(this, new ReverseGeocodeCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
			}
		}
		IAsyncResult OnBeginOpen(object[] inValues, AsyncCallback callback, object asyncState) {
			return ((ICommunicationObject)this).BeginOpen(callback, asyncState);
		}
		object[] OnEndOpen(IAsyncResult result) {
			((ICommunicationObject)this).EndOpen(result);
			return null;
		}
		void OnOpenCompleted(object state) {
			if(OpenCompleted != null) {
				InvokeAsyncCompletedEventArgs e = (InvokeAsyncCompletedEventArgs)state;
				OpenCompleted(this, new AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
			}
		}
		IAsyncResult OnBeginClose(object[] inValues, AsyncCallback callback, object asyncState) {
			return ((ICommunicationObject)this).BeginClose(callback, asyncState);
		}
		object[] OnEndClose(IAsyncResult result) {
			((ICommunicationObject)this).EndClose(result);
			return null;
		}
		void OnCloseCompleted(object state) {
			if((CloseCompleted != null)) {
				InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
				CloseCompleted(this, new AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
			}
		}
		public void GeocodeAsync(GeocodeRequest request) {
			GeocodeAsync(request, null);
		}
		public void GeocodeAsync(GeocodeRequest request, object userState) {
			if(onBeginGeocodeDelegate == null)
				onBeginGeocodeDelegate = new BeginOperationDelegate(OnBeginGeocode);
			if(onEndGeocodeDelegate == null)
				onEndGeocodeDelegate = new EndOperationDelegate(OnEndGeocode);
			if(onGeocodeCompletedDelegate == null)
				onGeocodeCompletedDelegate = new SendOrPostCallback(OnGeocodeCompleted);
			InvokeAsync(onBeginGeocodeDelegate, new object[] { request }, onEndGeocodeDelegate, onGeocodeCompletedDelegate, userState);
		}
		public void ReverseGeocodeAsync(ReverseGeocodeRequest request) {
			ReverseGeocodeAsync(request, null);
		}
		public void ReverseGeocodeAsync(ReverseGeocodeRequest request, object userState) {
			if(onBeginReverseGeocodeDelegate == null)
				onBeginReverseGeocodeDelegate = new BeginOperationDelegate(OnBeginReverseGeocode);
			if(onEndReverseGeocodeDelegate == null)
				onEndReverseGeocodeDelegate = new EndOperationDelegate(OnEndReverseGeocode);
			if(onReverseGeocodeCompletedDelegate == null)
				onReverseGeocodeCompletedDelegate = new SendOrPostCallback(OnReverseGeocodeCompleted);
			InvokeAsync(onBeginReverseGeocodeDelegate, new object[] { request }, onEndReverseGeocodeDelegate, onReverseGeocodeCompletedDelegate, userState);
		}
		public void CloseAsync() {
			CloseAsync(null);
		}
		public void CloseAsync(object userState) {
			if(onBeginCloseDelegate == null)
				onBeginCloseDelegate = new BeginOperationDelegate(OnBeginClose);
			if(onEndCloseDelegate == null)
				onEndCloseDelegate = new EndOperationDelegate(OnEndClose);
			if(onCloseCompletedDelegate == null)
				onCloseCompletedDelegate = new SendOrPostCallback(OnCloseCompleted);
			InvokeAsync(onBeginCloseDelegate, null, onEndCloseDelegate, onCloseCompletedDelegate, userState);
		}
		public void OpenAsync() {
			OpenAsync(null);
		}
		public void OpenAsync(object userState) {
			if(onBeginOpenDelegate == null)
				onBeginOpenDelegate = new BeginOperationDelegate(OnBeginOpen);
			if(onEndOpenDelegate == null)
				onEndOpenDelegate = new EndOperationDelegate(OnEndOpen);
			if(onOpenCompletedDelegate == null)
				onOpenCompletedDelegate = new SendOrPostCallback(OnOpenCompleted);
			InvokeAsync(onBeginOpenDelegate, null, onEndOpenDelegate, onOpenCompletedDelegate, userState);
		}
	}
}
namespace DevExpress.Map.Native {
	public class Coordinates {
		public double Latitude { get; set; }
		public double Longitude { get; set; }
	}
	public class BoundingBox {
		public double SouthLatitude { get; set; }
		public double WestLongitude { get; set; }
		public double NorthLatitude { get; set; }
		public double EastLongitude { get; set; }
	}
	public class Address {
		public string AddressLine { get; set; }
		public string Locality { get; set; }
		public string Neighborhood { get; set; }
		public string AdminDistrict { get; set; }
		public string AdminDistrict2 { get; set; }
		public string FormattedAddress { get; set; }
		public string PostalCode { get; set; }
		public string CountryRegion { get; set; }
		public string CountryRegionIso2 { get; set; }
		public string Landmark { get; set; }
	}
	public enum Confidence {
		High,
		Medium,
		Low
	}
	public enum MatchCode {
		Good,
		Ambiguous,
		UpHierarchy
	}
	public enum CalculationMethod {
		Interpolation,
		InterpolationOffset,
		Parcel,
		Rooftop
	}
	public enum UsageType {
		Display,
		Route
	}
	public class GeocodePoint {
		public double Latitude { get; set; }
		public double Longitude { get; set; }
		public CalculationMethod CalculationMethod { get; set; }
		public ObservableCollection<UsageType> UsageTypes { get; set; }
		public GeocodePoint() {
			this.UsageTypes = new ObservableCollection<UsageType>();
		}
	}
	public class LocationData {
		public string Name { get; set; }
		public Coordinates Point { get; set; }
		public BoundingBox BoundingBox { get; set; }
		public string EntityType { get; set; }
		public Address Address { get; set; }
		public Confidence Confidence { get; set; }
		public MatchCode MatchCode { get; set; }
		public GeocodePoint GeocodePoint { get; set; }
	}
	public class BingGeocodeServiceInfo : BingCommonServiceInfo {
		readonly ObservableCollection<LocationData> locations = new ObservableCollection<LocationData>();
		public ObservableCollection<LocationData> Locations { get { return locations; } }
		public BingGeocodeServiceInfo(ObservableCollection<LocationData> locations)
			: base(true) {
			this.locations = locations;
		}
		public BingGeocodeServiceInfo(XDocument xDocument)
			: base(xDocument) {
			if(!IsValidResponse)
				return;
			XElement resources = XResponse.Element(XName.Get("ResourceSets", BingNamespace)).
										  Element(XName.Get("ResourceSet", BingNamespace)).
										  Element(XName.Get("Resources", BingNamespace));
			foreach(XElement location in resources.Elements(XName.Get("Location", BingNamespace)))
				locations.Add(GetLocationData(location));
		}
		LocationData GetLocationData(XElement xLocation) {
			LocationData locationData = new LocationData();
			locationData.Name = GetAncestorElementValue(xLocation, "Name");
			FillPoint(locationData, xLocation);
			FillBoundingBox(locationData, xLocation);
			locationData.EntityType = GetAncestorElementValue(xLocation, "EntityType");
			FillAddress(locationData, xLocation);
			FillConfidence(locationData, xLocation);
			FillMatchCode(locationData, xLocation);
			FillGeocodePoint(locationData, xLocation);
			return locationData;
		}
		void FillPoint(LocationData locationData, XElement xLocation) {
			XElement xPoint = xLocation.Element(XName.Get("Point", BingNamespace));
			string latitude = GetAncestorElementValue(xPoint, "Latitude");
			string longitude = GetAncestorElementValue(xPoint, "Longitude");
			if(latitude != null && longitude != null)
				locationData.Point = new Coordinates() { Latitude = Double.Parse(latitude), Longitude = Double.Parse(longitude) };
		}
		void FillBoundingBox(LocationData locationData, XElement xLocation) {
			XElement boundingBox = xLocation.Element(XName.Get("BoundingBox", BingNamespace));
			string southLatitude = GetAncestorElementValue(boundingBox, "SouthLatitude");
			string westLongitude = GetAncestorElementValue(boundingBox, "WestLongitude");
			string northLatitude = GetAncestorElementValue(boundingBox, "NorthLatitude");
			string eastLongitude = GetAncestorElementValue(boundingBox, "EastLongitude");
			if(southLatitude != null && westLongitude != null && northLatitude != null && eastLongitude != null)
				locationData.BoundingBox = new BoundingBox() {
					SouthLatitude = Double.Parse(southLatitude),
					WestLongitude = Double.Parse(westLongitude),
					NorthLatitude = Double.Parse(northLatitude),
					EastLongitude = Double.Parse(eastLongitude)
				};
		}
		void FillAddress(LocationData locationData, XElement xLocation) {
			XElement xAddress = xLocation.Element(XName.Get("Address", BingNamespace));
			locationData.Address = new Address() {
				AddressLine = GetAncestorElementValue(xAddress, "AddressLine"),
				Locality = GetAncestorElementValue(xAddress, "Locality"),
				Neighborhood = GetAncestorElementValue(xAddress, "Neighborhood"),
				AdminDistrict = GetAncestorElementValue(xAddress, "AdminDistrict"),
				AdminDistrict2 = GetAncestorElementValue(xAddress, "AdminDistrict2"),
				FormattedAddress = GetAncestorElementValue(xAddress, "FormattedAddress"),
				PostalCode = GetAncestorElementValue(xAddress, "PostalCode"),
				CountryRegion = GetAncestorElementValue(xAddress, "CountryRegion"),
				CountryRegionIso2 = GetAncestorElementValue(xAddress, "CountryRegionIso2"),
				Landmark = GetAncestorElementValue(xAddress, "Landmark")
			};
		}
		void FillConfidence(LocationData locationData, XElement xLocation) {
			string confidence = GetAncestorElementValue(xLocation, "Confidence");
			switch(confidence) {
				case "High": locationData.Confidence = Confidence.High; break;
				case "Medium": locationData.Confidence = Confidence.Medium; break;
				case "Low": locationData.Confidence = Confidence.Low; break;
			}
		}
		void FillMatchCode(LocationData locationData, XElement xLocation) {
			string matchCode = GetAncestorElementValue(xLocation, "MatchCode");
			switch(matchCode) {
				case "Good": locationData.MatchCode = MatchCode.Good; break;
				case "Ambiguous": locationData.MatchCode = MatchCode.Ambiguous; break;
				case "UpHierarchy": locationData.MatchCode = MatchCode.UpHierarchy; break;
			}
		}
		void FillGeocodePoint(LocationData locationData, XElement xLocation) {
			locationData.GeocodePoint = new GeocodePoint();
			XElement xGeoCodePoint = xLocation.Element(XName.Get("GeocodePoint", BingNamespace));
			string latitude = GetAncestorElementValue(xGeoCodePoint, "Latitude");
			if(latitude != null)
				locationData.GeocodePoint.Latitude = Double.Parse(latitude);
			string longitude = GetAncestorElementValue(xGeoCodePoint, "Longitude");
			if(longitude != null)
				locationData.GeocodePoint.Longitude = Double.Parse(longitude);
			FillCalculationMethod(locationData.GeocodePoint, xGeoCodePoint);
			FillUsageTypes(locationData.GeocodePoint, xGeoCodePoint);
		}
		void FillCalculationMethod(GeocodePoint geocodePoint, XElement xGeoCodePoint) {
			string calculationMethod = GetAncestorElementValue(xGeoCodePoint, "CalculationMethod");
			switch(calculationMethod) {
				case "Interpolation": geocodePoint.CalculationMethod = CalculationMethod.Interpolation; break;
				case "InterpolationOffset": geocodePoint.CalculationMethod = CalculationMethod.InterpolationOffset; break;
				case "Parcel": geocodePoint.CalculationMethod = CalculationMethod.Parcel; break;
				case "Rooftop": geocodePoint.CalculationMethod = CalculationMethod.Rooftop; break;
			}
		}
		void FillUsageTypes(GeocodePoint geocodePoint, XElement xGeoCodePoint) {
			geocodePoint.UsageTypes = new ObservableCollection<UsageType>();
			foreach(XElement usageType in xGeoCodePoint.Elements(XName.Get("UsageType", BingNamespace)))
				geocodePoint.UsageTypes.Add(GetUsageType(usageType.Value));
		}
		UsageType GetUsageType(string usageType) {
			switch(usageType) {
				case "Display": return UsageType.Display;
				case "Route":
				default: return UsageType.Route;
			}
		}
	}
}
