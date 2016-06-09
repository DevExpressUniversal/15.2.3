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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Windows;
using System.Xml.Linq;
namespace DevExpress.Map.BingServices {
	public static class BingServicesNamespaces {
		public const string CommonNamespace = "http://dev.virtualearth.net/webservices/v1/common";
		public const string ImageryContractNamespace = "http://dev.virtualearth.net/webservices/v1/imagery/contracts";
		public const string GeocodeContractNamespace = "http://dev.virtualearth.net/webservices/v1/geocode/contracts";
		public const string SearchContractNamespace = "http://dev.virtualearth.net/webservices/v1/search/contracts";
		public const string RouteContractNamespace = "http://dev.virtualearth.net/webservices/v1/route/contracts";
		public const string ImageryNamespace = "http://dev.virtualearth.net/webservices/v1/imagery";
		public const string SearchNamespace = "http://dev.virtualearth.net/webservices/v1/search";
		public const string GeocodeNamespace = "http://dev.virtualearth.net/webservices/v1/geocode";
		public const string RouteNamespace = "http://dev.virtualearth.net/webservices/v1/route";
	}
	[DataContract(Name = "AuthenticationResultCode", Namespace = BingServicesNamespaces.CommonNamespace)]
	public enum AuthenticationResultCode : int {
		[EnumMember]
		None = 0,
		[EnumMember]
		NoCredentials = 1,
		[EnumMember]
		ValidCredentials = 2,
		[EnumMember]
		InvalidCredentials = 3,
		[EnumMember]
		CredentialsExpired = 4,
		[EnumMember]
		NotAuthorized = 7,
	}
	[DataContract(Name = "Confidence", Namespace = BingServicesNamespaces.CommonNamespace)]
	public enum Confidence : int {
		[EnumMember]
		High = 0,
		[EnumMember]
		Medium = 1,
		[EnumMember]
		Low = 2,
	}
	[DataContract(Name = "DeviceType", Namespace = BingServicesNamespaces.CommonNamespace)]
	public enum DeviceType : int {
		[EnumMember]
		Desktop = 0,
		[EnumMember]
		Mobile = 1,
	}
	[DataContract(Name = "DistanceUnit", Namespace = BingServicesNamespaces.CommonNamespace)]
	public enum DistanceUnit : int {
		[EnumMember]
		Kilometer = 0,
		[EnumMember]
		Mile = 1,
	}
	[DataContract(Name = "ImageType", Namespace = BingServicesNamespaces.CommonNamespace)]
	public enum ImageType : int {
		[EnumMember]
		Default = 0,
		[EnumMember]
		Png = 1,
		[EnumMember]
		Jpeg = 2,
		[EnumMember]
		Gif = 3,
	}
	[DataContract(Name = "MapStyle", Namespace = BingServicesNamespaces.CommonNamespace)]
	public enum MapStyle : int {
		[EnumMember]
		Road = 0,
		[EnumMember]
		Aerial = 1,
		[EnumMember]
		AerialWithLabels = 2,
		[EnumMember]
		Birdseye = 3,
		[EnumMember]
		BirdseyeWithLabels = 4,
		[EnumMember]
		Road_v0 = 5,
		[EnumMember]
		AerialWithLabels_v0 = 6,
		[EnumMember]
		BirdseyeWithLabels_v0 = 7,
		[EnumMember]
		Road_v1 = 8,
		[EnumMember]
		AerialWithLabels_v1 = 9,
		[EnumMember]
		BirdseyeWithLabels_v1 = 10,
	}
	[DataContract(Name = "ResponseStatusCode", Namespace = BingServicesNamespaces.CommonNamespace)]
	public enum ResponseStatusCode : int {
		[EnumMember()]
		Success = 0,
		[EnumMember()]
		BadRequest = 1,
		[EnumMember()]
		ServerError = 2,
	}
	[DataContract(Name = "UriScheme", Namespace = BingServicesNamespaces.CommonNamespace)]
	public enum UriScheme : int {
		[EnumMember()]
		Http = 0,
		[EnumMember()]
		Https = 1,
	}
	[DataContract(Name = "Location", Namespace = BingServicesNamespaces.CommonNamespace)]
	[KnownType(typeof(GeocodeLocation))]
	[KnownType(typeof(UserLocation))]
	public class Location : INotifyPropertyChanged {
		double altitude;
		double latitude;
		double longitude;
		[DataMember]
		public double Altitude {
			get {
				return altitude;
			}
			set {
				if(altitude != value) {
					altitude = value;
					RaisePropertyChanged("Altitude");
				}
			}
		}
		[DataMember]
		public double Latitude {
			get {
				return latitude;
			}
			set {
				if(latitude != value) {
					latitude = value;
					RaisePropertyChanged("Latitude");
				}
			}
		}
		[DataMember]
		public double Longitude {
			get {
				return longitude;
			}
			set {
				if(longitude != value) {
					longitude = value;
					RaisePropertyChanged("Longitude");
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			PropertyChangedEventHandler propertyChanged = PropertyChanged;
			if(propertyChanged != null) {
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	[DataContract(Name = "GeocodeLocation", Namespace = BingServicesNamespaces.CommonNamespace)]
	public class GeocodeLocation : Location {
		string calculationMethod;
		[DataMember]
		public string CalculationMethod {
			get {
				return calculationMethod;
			}
			set {
				if(calculationMethod != value) {
					calculationMethod = value;
					RaisePropertyChanged("CalculationMethod");
				}
			}
		}
	}
	[DataContract(Name = "UserLocation", Namespace = BingServicesNamespaces.CommonNamespace)]
	public class UserLocation : Location {
		Confidence confidence;
		[DataMemberAttribute()]
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
	}
	[DataContract(Name = "ShapeBase", Namespace = BingServicesNamespaces.CommonNamespace)]
	[KnownType(typeof(SearchPoint))]
	[KnownType(typeof(Circle))]
	[KnownType(typeof(Rectangle))]
	[KnownType(typeof(Polygon))]
	public class ShapeBase : INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			PropertyChangedEventHandler propertyChanged = PropertyChanged;
			if(propertyChanged != null) {
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	[DataContract(Name = "Circle", Namespace = BingServicesNamespaces.CommonNamespace)]
	public class Circle : ShapeBase {
		Location center;
		DistanceUnit distanceUnit;
		double radius;
		[DataMember]
		public Location Center {
			get {
				return center;
			}
			set {
				if(center != value) {
					center = value;
					RaisePropertyChanged("Center");
				}
			}
		}
		[DataMember]
		public DistanceUnit DistanceUnit {
			get {
				return distanceUnit;
			}
			set {
				if(distanceUnit != value) {
					distanceUnit = value;
					RaisePropertyChanged("DistanceUnit");
				}
			}
		}
		[DataMember]
		public double Radius {
			get {
				return radius;
			}
			set {
				if(radius != value) {
					radius = value;
					RaisePropertyChanged("Radius");
				}
			}
		}
	}
	[DataContract(Name = "Rectangle", Namespace = BingServicesNamespaces.CommonNamespace)]
	public class Rectangle : ShapeBase {
		Location northeast;
		Location southwest;
		[DataMember]
		public Location Northeast {
			get {
				return northeast;
			}
			set {
				if(northeast != value) {
					northeast = value;
					RaisePropertyChanged("Northeast");
				}
			}
		}
		[DataMember]
		public Location Southwest {
			get {
				return southwest;
			}
			set {
				if(southwest != value) {
					southwest = value;
					RaisePropertyChanged("Southwest");
				}
			}
		}
	}
	[DataContract(Name = "Polygon", Namespace = BingServicesNamespaces.CommonNamespace)]
	public class Polygon : ShapeBase {
		ObservableCollection<Location> vertices;
		[DataMember]
		public ObservableCollection<Location> Vertices {
			get {
				return vertices;
			}
			set {
				if(vertices != value) {
					vertices = value;
					RaisePropertyChanged("Vertices");
				}
			}
		}
	}
	[DataContract(Name = "SearchPoint", Namespace = BingServicesNamespaces.SearchNamespace)]
	public class SearchPoint : ShapeBase {
		Location point;
		[DataMember]
		public Location Point {
			get { return point; }
			set {
				if((ReferenceEquals(point, value) != true)) {
					point = value;
					RaisePropertyChanged("Point");
				}
			}
		}
	}
	[DataContract(Name = "RequestBase", Namespace = BingServicesNamespaces.CommonNamespace)]
	[KnownType(typeof(MapUriRequest))]
	[KnownType(typeof(ImageryMetadataRequest))]
	[KnownType(typeof(ReverseGeocodeRequest))]
	[KnownType(typeof(GeocodeRequest))]
	public class RequestBase : INotifyPropertyChanged {
		string culture;
		Credentials credentials;
		ExecutionOptions executionOptions;
		UserProfile userProfile;
		[DataMember]
		public Credentials Credentials {
			get {
				return credentials;
			}
			set {
				if(credentials != value) {
					credentials = value;
					RaisePropertyChanged("Credentials");
				}
			}
		}
		[DataMember]
		public string Culture {
			get {
				return culture;
			}
			set {
				if(culture != value) {
					culture = value;
					RaisePropertyChanged("Culture");
				}
			}
		}
		[DataMember]
		public ExecutionOptions ExecutionOptions {
			get {
				return executionOptions;
			}
			set {
				if(executionOptions != value) {
					executionOptions = value;
					RaisePropertyChanged("ExecutionOptions");
				}
			}
		}
		[DataMember]
		public UserProfile UserProfile {
			get {
				return userProfile;
			}
			set {
				if(userProfile != value) {
					userProfile = value;
					RaisePropertyChanged("UserProfile");
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			PropertyChangedEventHandler propertyChanged = PropertyChanged;
			if(propertyChanged != null)
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
	[DataContract(Name = "ResponseBase", Namespace = BingServicesNamespaces.CommonNamespace)]
	[KnownType(typeof(MapUriResponse))]
	[KnownType(typeof(ImageryMetadataResponse))]
	[KnownType(typeof(GeocodeResponse))]
	public class ResponseBase : INotifyPropertyChanged {
		Uri brandLogoUri;
		ResponseSummary responseSummary;
		[DataMember]
		public Uri BrandLogoUri {
			get {
				return brandLogoUri;
			}
			set {
				if(brandLogoUri != value) {
					brandLogoUri = value;
					RaisePropertyChanged("BrandLogoUri");
				}
			}
		}
		[DataMember]
		public ResponseSummary ResponseSummary {
			get {
				return responseSummary;
			}
			set {
				if(responseSummary != value) {
					responseSummary = value;
					RaisePropertyChanged("ResponseSummary");
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			PropertyChangedEventHandler propertyChanged = PropertyChanged;
			if(propertyChanged != null) {
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	[DataContract(Name = "ResponseSummary", Namespace = BingServicesNamespaces.CommonNamespace)]
	public class ResponseSummary : INotifyPropertyChanged {
		AuthenticationResultCode authenticationResultCode;
		string copyright;
		string faultReason;
		ResponseStatusCode statusCode;
		string traceId;
		[DataMember]
		public AuthenticationResultCode AuthenticationResultCode {
			get {
				return authenticationResultCode;
			}
			set {
				if(authenticationResultCode != value) {
					authenticationResultCode = value;
					RaisePropertyChanged("AuthenticationResultCode");
				}
			}
		}
		[DataMember]
		public string Copyright {
			get {
				return copyright;
			}
			set {
				if(copyright != value) {
					copyright = value;
					RaisePropertyChanged("Copyright");
				}
			}
		}
		[DataMember]
		public string FaultReason {
			get {
				return faultReason;
			}
			set {
				if(faultReason != value) {
					faultReason = value;
					RaisePropertyChanged("FaultReason");
				}
			}
		}
		[DataMember]
		public ResponseStatusCode StatusCode {
			get {
				return statusCode;
			}
			set {
				if(statusCode != value) {
					statusCode = value;
					RaisePropertyChanged("StatusCode");
				}
			}
		}
		[DataMember]
		public string TraceId {
			get {
				return traceId;
			}
			set {
				if(traceId != value) {
					traceId = value;
					RaisePropertyChanged("TraceId");
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			PropertyChangedEventHandler propertyChanged = PropertyChanged;
			if(propertyChanged != null) {
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	[DataContract(Name = "Credentials", Namespace = BingServicesNamespaces.CommonNamespace)]
	public class Credentials : INotifyPropertyChanged {
		string applicationId;
		string token;
		[DataMember]
		public string ApplicationId {
			get {
				return applicationId;
			}
			set {
				if(applicationId != value) {
					applicationId = value;
					RaisePropertyChanged("ApplicationId");
				}
			}
		}
		[DataMember]
		public string Token {
			get {
				return token;
			}
			set {
				if(token != value) {
					token = value;
					RaisePropertyChanged("Token");
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			PropertyChangedEventHandler propertyChanged = PropertyChanged;
			if(propertyChanged != null) {
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	[DataContract(Name = "ExecutionOptions", Namespace = BingServicesNamespaces.CommonNamespace)]
	public class ExecutionOptions : INotifyPropertyChanged {
		bool suppressFaults;
		[DataMember]
		public bool SuppressFaults {
			get {
				return suppressFaults;
			}
			set {
				if(suppressFaults != value) {
					suppressFaults = value;
					RaisePropertyChanged("SuppressFaults");
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			PropertyChangedEventHandler propertyChanged = PropertyChanged;
			if(propertyChanged != null)
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
	[DataContract(Name = "Heading", Namespace = BingServicesNamespaces.CommonNamespace)]
	public class Heading : INotifyPropertyChanged {
		double orientation;
		[DataMember]
		public double Orientation {
			get {
				return orientation;
			}
			set {
				if(orientation != value) {
					orientation = value;
					RaisePropertyChanged("Orientation");
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			PropertyChangedEventHandler propertyChanged = PropertyChanged;
			if(propertyChanged != null) {
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	[DataContract(Name = "SizeOfint", Namespace = BingServicesNamespaces.CommonNamespace)]
	public class SizeOfint : INotifyPropertyChanged {
		int height;
		int width;
		[DataMember]
		public int Height {
			get {
				return height;
			}
			set {
				if(height != value) {
					height = value;
					RaisePropertyChanged("Height");
				}
			}
		}
		[DataMember]
		public int Width {
			get {
				return width;
			}
			set {
				if(width != value) {
					width = value;
					RaisePropertyChanged("Width");
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			PropertyChangedEventHandler propertyChanged = PropertyChanged;
			if(propertyChanged != null) {
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	[DataContract(Name = "UserProfile", Namespace = BingServicesNamespaces.CommonNamespace)]
	public class UserProfile : INotifyPropertyChanged {
		Heading currentHeading;
		UserLocation currentLocation;
		DeviceType deviceType;
		DistanceUnit distanceUnit;
		string iPAddress;
		ShapeBase mapView;
		SizeOfint screenSize;
		[DataMember]
		public Heading CurrentHeading {
			get {
				return currentHeading;
			}
			set {
				if(currentHeading != value) {
					currentHeading = value;
					RaisePropertyChanged("CurrentHeading");
				}
			}
		}
		[DataMember]
		public UserLocation CurrentLocation {
			get {
				return currentLocation;
			}
			set {
				if(currentLocation != value) {
					currentLocation = value;
					RaisePropertyChanged("CurrentLocation");
				}
			}
		}
		[DataMember]
		public DeviceType DeviceType {
			get {
				return deviceType;
			}
			set {
				if(deviceType != value) {
					deviceType = value;
					RaisePropertyChanged("DeviceType");
				}
			}
		}
		[DataMember]
		public DistanceUnit DistanceUnit {
			get {
				return distanceUnit;
			}
			set {
				if(distanceUnit != value) {
					distanceUnit = value;
					RaisePropertyChanged("DistanceUnit");
				}
			}
		}
		[DataMember]
		public string IPAddress {
			get {
				return iPAddress;
			}
			set {
				if(iPAddress != value) {
					iPAddress = value;
					RaisePropertyChanged("IPAddress");
				}
			}
		}
		[DataMember]
		public ShapeBase MapView {
			get {
				return mapView;
			}
			set {
				if(mapView != value) {
					mapView = value;
					RaisePropertyChanged("MapView");
				}
			}
		}
		[DataMember]
		public SizeOfint ScreenSize {
			get {
				return screenSize;
			}
			set {
				if(screenSize != value) {
					screenSize = value;
					RaisePropertyChanged("ScreenSize");
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			PropertyChangedEventHandler propertyChanged = PropertyChanged;
			if(propertyChanged != null) {
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	[DataContract(Name = "Pushpin", Namespace = BingServicesNamespaces.CommonNamespace)]
	public class Pushpin : INotifyPropertyChanged {
		string iconStyle;
		string label;
		Location location;
		[DataMember]
		public string IconStyle {
			get {
				return iconStyle;
			}
			set {
				if(iconStyle != value) {
					iconStyle = value;
					RaisePropertyChanged("IconStyle");
				}
			}
		}
		[DataMember]
		public string Label {
			get {
				return label;
			}
			set {
				if(label != value) {
					label = value;
					RaisePropertyChanged("Label");
				}
			}
		}
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
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			PropertyChangedEventHandler propertyChanged = PropertyChanged;
			if(propertyChanged != null) {
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	[DataContract(Name = "RangeOfdateTime", Namespace = BingServicesNamespaces.CommonNamespace)]
	public class RangeOfdateTime : INotifyPropertyChanged {
		DateTime from;
		DateTime to;
		[DataMember]
		public DateTime From {
			get {
				return from;
			}
			set {
				if(from != value) {
					from = value;
					RaisePropertyChanged("From");
				}
			}
		}
		[DataMember]
		public DateTime To {
			get {
				return to;
			}
			set {
				if(to != value) {
					to = value;
					RaisePropertyChanged("To");
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			PropertyChangedEventHandler propertyChanged = PropertyChanged;
			if(propertyChanged != null) {
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	[DataContract(Name = "RangeOfint", Namespace = BingServicesNamespaces.CommonNamespace)]
	public class RangeOfint : INotifyPropertyChanged {
		int from;
		int to;
		[DataMember]
		public int From {
			get {
				return from;
			}
			set {
				if(from != value) {
					from = value;
					RaisePropertyChanged("From");
				}
			}
		}
		[DataMember]
		public int To {
			get {
				return to;
			}
			set {
				if(to != value) {
					to = value;
					RaisePropertyChanged("To");
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			PropertyChangedEventHandler propertyChanged = PropertyChanged;
			if(propertyChanged != null) {
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	[DataContract(Name = "Address", Namespace = BingServicesNamespaces.CommonNamespace)]
	public class Address : INotifyPropertyChanged {
		string addressLine;
		string adminDistrict;
		string countryRegion;
		string district;
		string formattedAddress;
		string locality;
		string postalCode;
		string postalTown;
		[DataMember]
		public string AddressLine {
			get {
				return addressLine;
			}
			set {
				if(addressLine != value) {
					addressLine = value;
					RaisePropertyChanged("AddressLine");
				}
			}
		}
		[DataMember]
		public string AdminDistrict {
			get {
				return adminDistrict;
			}
			set {
				if(adminDistrict != value) {
					adminDistrict = value;
					RaisePropertyChanged("AdminDistrict");
				}
			}
		}
		[DataMember]
		public string CountryRegion {
			get {
				return countryRegion;
			}
			set {
				if(countryRegion != value) {
					countryRegion = value;
					RaisePropertyChanged("CountryRegion");
				}
			}
		}
		[DataMember]
		public string District {
			get {
				return district;
			}
			set {
				if(district != value) {
					district = value;
					RaisePropertyChanged("District");
				}
			}
		}
		[DataMember]
		public string FormattedAddress {
			get {
				return formattedAddress;
			}
			set {
				if(formattedAddress != value) {
					formattedAddress = value;
					RaisePropertyChanged("FormattedAddress");
				}
			}
		}
		[DataMember]
		public string Locality {
			get {
				return locality;
			}
			set {
				if(locality != value) {
					locality = value;
					RaisePropertyChanged("Locality");
				}
			}
		}
		[DataMember]
		public string PostalCode {
			get {
				return postalCode;
			}
			set {
				if(postalCode != value) {
					postalCode = value;
					RaisePropertyChanged("PostalCode");
				}
			}
		}
		[DataMember]
		public string PostalTown {
			get {
				return postalTown;
			}
			set {
				if(postalTown != value) {
					postalTown = value;
					RaisePropertyChanged("PostalTown");
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
}
namespace DevExpress.Map.Native {
	public static class WebServiceHelper {
		public static string CurrentScheme { get { return Uri.UriSchemeHttp; } }
		public static string CorrectScheme(string uri) {
			return uri;
		}
		public static BasicHttpSecurityMode SecurityMode { get { return BasicHttpSecurityMode.None; } }
	}
	public class BingCommonServiceInfo {
		protected const string BingNamespace = "http://schemas.microsoft.com/search/local/ws/rest/v1";
		readonly bool isValidResponse;
		readonly XElement xResponse;
		protected XElement XResponse { get { return xResponse; } }
		public bool IsValidResponse { get { return isValidResponse; } }
		protected BingCommonServiceInfo(bool isValidResponse) {
			this.isValidResponse = isValidResponse;
		}
		public BingCommonServiceInfo(XDocument xDocument) {
			this.xResponse = xDocument.Element(XName.Get("Response", BingNamespace));
			string statusCode = GetAncestorElementValue(xResponse, "StatusCode");
			this.isValidResponse = statusCode == "200";
		}
		protected string GetAncestorElementValue(XElement xElement, string key) {
			XElement ancestor = xElement.Element(XName.Get(key, BingNamespace));
			return ancestor != null ? ancestor.Value : null;
		}
	}
}
