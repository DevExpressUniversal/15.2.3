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
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ComponentModel;
using DevExpress.Utils;
using DevExpress.Map.BingServices;
using System.Threading;
using System.Xml.Linq;
namespace DevExpress.Map.BingServices {
	[DataContract(Name = "CoverageArea", Namespace = BingServicesNamespaces.ImageryNamespace)]
	public class CoverageArea : INotifyPropertyChanged {
		Rectangle boundingRectangleField;
		RangeOfint zoomRangeField;
		[DataMember]
		public Rectangle BoundingRectangle {
			get {
				return boundingRectangleField;
			}
			set {
				if((ReferenceEquals(boundingRectangleField, value) != true)) {
					boundingRectangleField = value;
					RaisePropertyChanged("BoundingRectangle");
				}
			}
		}
		[DataMember]
		public RangeOfint ZoomRange {
			get {
				return zoomRangeField;
			}
			set {
				if((ReferenceEquals(zoomRangeField, value) != true)) {
					zoomRangeField = value;
					RaisePropertyChanged("ZoomRange");
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
	[DataContract(Name = "ImageryProvider", Namespace = BingServicesNamespaces.ImageryNamespace)]
	public class ImageryProvider : INotifyPropertyChanged {
		string attributionField;
		NotificationCollection<CoverageArea> coverageAreasField;
		[DataMember]
		public string Attribution {
			get {
				return attributionField;
			}
			set {
				if((ReferenceEquals(attributionField, value) != true)) {
					attributionField = value;
					RaisePropertyChanged("Attribution");
				}
			}
		}
		[DataMember]
		public NotificationCollection<CoverageArea> CoverageAreas {
			get {
				return coverageAreasField;
			}
			set {
				if((ReferenceEquals(coverageAreasField, value) != true)) {
					coverageAreasField = value;
					RaisePropertyChanged("CoverageAreas");
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
	[DataContract(Name = "ImageryMetadataResult", Namespace = BingServicesNamespaces.ImageryNamespace)]
	[KnownType(typeof(ImageryMetadataBirdseyeResult))]
	public class ImageryMetadataResult : INotifyPropertyChanged {
		SizeOfint imageSizeField;
		string imageUriField;
		NotificationCollection<string> imageUriSubdomainsField;
		NotificationCollection<ImageryProvider> imageryProvidersField;
		RangeOfdateTime vintageField;
		RangeOfint zoomRangeField;
		[DataMember]
		public SizeOfint ImageSize {
			get {
				return imageSizeField;
			}
			set {
				if((ReferenceEquals(imageSizeField, value) != true)) {
					imageSizeField = value;
					RaisePropertyChanged("ImageSize");
				}
			}
		}
		[DataMember]
		public string ImageUri {
			get {
				return imageUriField;
			}
			set {
				if((ReferenceEquals(imageUriField, value) != true)) {
					imageUriField = value;
					RaisePropertyChanged("ImageUri");
				}
			}
		}
		[DataMember]
		public NotificationCollection<string> ImageUriSubdomains {
			get {
				return imageUriSubdomainsField;
			}
			set {
				if((ReferenceEquals(imageUriSubdomainsField, value) != true)) {
					imageUriSubdomainsField = value;
					RaisePropertyChanged("ImageUriSubdomains");
				}
			}
		}
		[DataMember]
		public NotificationCollection<ImageryProvider> ImageryProviders {
			get {
				return imageryProvidersField;
			}
			set {
				if((ReferenceEquals(imageryProvidersField, value) != true)) {
					imageryProvidersField = value;
					RaisePropertyChanged("ImageryProviders");
				}
			}
		}
		[DataMember]
		public RangeOfdateTime Vintage {
			get {
				return vintageField;
			}
			set {
				if((ReferenceEquals(vintageField, value) != true)) {
					vintageField = value;
					RaisePropertyChanged("Vintage");
				}
			}
		}
		[DataMember]
		public RangeOfint ZoomRange {
			get {
				return zoomRangeField;
			}
			set {
				if((ReferenceEquals(zoomRangeField, value) != true)) {
					zoomRangeField = value;
					RaisePropertyChanged("ZoomRange");
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
	[DataContract(Name = "MapUriRequest", Namespace = BingServicesNamespaces.ImageryNamespace)]
	public class MapUriRequest : RequestBase {
		Location centerField;
		Location majorRoutesDestinationField;
		MapUriOptions optionsField;
		NotificationCollection<Pushpin> pushpinsField;
		[DataMember]
		public Location Center {
			get {
				return centerField;
			}
			set {
				if((ReferenceEquals(centerField, value) != true)) {
					centerField = value;
					RaisePropertyChanged("Center");
				}
			}
		}
		[DataMember]
		public Location MajorRoutesDestination {
			get {
				return majorRoutesDestinationField;
			}
			set {
				if((ReferenceEquals(majorRoutesDestinationField, value) != true)) {
					majorRoutesDestinationField = value;
					RaisePropertyChanged("MajorRoutesDestination");
				}
			}
		}
		[DataMember]
		public MapUriOptions Options {
			get {
				return optionsField;
			}
			set {
				if((ReferenceEquals(optionsField, value) != true)) {
					optionsField = value;
					RaisePropertyChanged("Options");
				}
			}
		}
		[DataMember]
		public NotificationCollection<Pushpin> Pushpins {
			get {
				return pushpinsField;
			}
			set {
				if((ReferenceEquals(pushpinsField, value) != true)) {
					pushpinsField = value;
					RaisePropertyChanged("Pushpins");
				}
			}
		}
	}
	[DataContract(Name = "MapUriResponse", Namespace = BingServicesNamespaces.ImageryNamespace)]
	public class MapUriResponse : ResponseBase {
		string uriField;
		[DataMember]
		public string Uri {
			get {
				return uriField;
			}
			set {
				if((ReferenceEquals(uriField, value) != true)) {
					uriField = value;
					RaisePropertyChanged("Uri");
				}
			}
		}
	}
	[DataContract(Name = "MapUriOptions", Namespace = BingServicesNamespaces.ImageryNamespace)]
	public class MapUriOptions : INotifyPropertyChanged {
		NotificationCollection<string> displayLayersField;
		SizeOfint imageSizeField;
		ImageType imageTypeField;
		bool preventIconCollisionField;
		MapStyle styleField;
		UriScheme uriSchemeField;
		Nullable<int> zoomLevelField;
		[DataMember]
		public NotificationCollection<string> DisplayLayers {
			get {
				return displayLayersField;
			}
			set {
				if((ReferenceEquals(displayLayersField, value) != true)) {
					displayLayersField = value;
					RaisePropertyChanged("DisplayLayers");
				}
			}
		}
		[DataMember]
		public SizeOfint ImageSize {
			get {
				return imageSizeField;
			}
			set {
				if((ReferenceEquals(imageSizeField, value) != true)) {
					imageSizeField = value;
					RaisePropertyChanged("ImageSize");
				}
			}
		}
		[DataMember]
		public ImageType ImageType {
			get {
				return imageTypeField;
			}
			set {
				if((imageTypeField.Equals(value) != true)) {
					imageTypeField = value;
					RaisePropertyChanged("ImageType");
				}
			}
		}
		[DataMember]
		public bool PreventIconCollision {
			get {
				return preventIconCollisionField;
			}
			set {
				if((preventIconCollisionField.Equals(value) != true)) {
					preventIconCollisionField = value;
					RaisePropertyChanged("PreventIconCollision");
				}
			}
		}
		[DataMember]
		public MapStyle Style {
			get {
				return styleField;
			}
			set {
				if((styleField.Equals(value) != true)) {
					styleField = value;
					RaisePropertyChanged("Style");
				}
			}
		}
		[DataMember]
		public UriScheme UriScheme {
			get {
				return uriSchemeField;
			}
			set {
				if((uriSchemeField.Equals(value) != true)) {
					uriSchemeField = value;
					RaisePropertyChanged("UriScheme");
				}
			}
		}
		[DataMember]
		public Nullable<int> ZoomLevel {
			get {
				return zoomLevelField;
			}
			set {
				if((zoomLevelField.Equals(value) != true)) {
					zoomLevelField = value;
					RaisePropertyChanged("ZoomLevel");
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
	[DataContract(Name = "ImageryMetadataBirdseyeResult", Namespace = BingServicesNamespaces.ImageryNamespace)]
	public class ImageryMetadataBirdseyeResult : ImageryMetadataResult {
		Heading headingField;
		int tilesXField;
		int tilesYField;
		[DataMember]
		public Heading Heading {
			get {
				return headingField;
			}
			set {
				if((ReferenceEquals(headingField, value) != true)) {
					headingField = value;
					RaisePropertyChanged("Heading");
				}
			}
		}
		[DataMember]
		public int TilesX {
			get {
				return tilesXField;
			}
			set {
				if((tilesXField.Equals(value) != true)) {
					tilesXField = value;
					RaisePropertyChanged("TilesX");
				}
			}
		}
		[DataMember]
		public int TilesY {
			get {
				return tilesYField;
			}
			set {
				if((tilesYField.Equals(value) != true)) {
					tilesYField = value;
					RaisePropertyChanged("TilesY");
				}
			}
		}
	}
	[DataContract(Name = "ImageryMetadataResponse", Namespace = BingServicesNamespaces.ImageryNamespace)]
	public class ImageryMetadataResponse : ResponseBase {
		NotificationCollection<ImageryMetadataResult> resultsField;
		[DataMember]
		public NotificationCollection<ImageryMetadataResult> Results {
			get {
				return resultsField;
			}
			set {
				if((ReferenceEquals(resultsField, value) != true)) {
					resultsField = value;
					RaisePropertyChanged("Results");
				}
			}
		}
	}
	[DataContract(Name = "ImageryMetadataOptions", Namespace = BingServicesNamespaces.ImageryNamespace)]
	public class ImageryMetadataOptions : INotifyPropertyChanged {
		Heading headingField;
		Location locationField;
		bool returnImageryProvidersField;
		UriScheme uriSchemeField;
		Nullable<int> zoomLevelField;
		[DataMember]
		public Heading Heading {
			get {
				return headingField;
			}
			set {
				if((ReferenceEquals(headingField, value) != true)) {
					headingField = value;
					RaisePropertyChanged("Heading");
				}
			}
		}
		[DataMember]
		public Location Location {
			get {
				return locationField;
			}
			set {
				if((ReferenceEquals(locationField, value) != true)) {
					locationField = value;
					RaisePropertyChanged("Location");
				}
			}
		}
		[DataMember]
		public bool ReturnImageryProviders {
			get {
				return returnImageryProvidersField;
			}
			set {
				if((returnImageryProvidersField.Equals(value) != true)) {
					returnImageryProvidersField = value;
					RaisePropertyChanged("ReturnImageryProviders");
				}
			}
		}
		[DataMember]
		public UriScheme UriScheme {
			get {
				return uriSchemeField;
			}
			set {
				if((uriSchemeField.Equals(value) != true)) {
					uriSchemeField = value;
					RaisePropertyChanged("UriScheme");
				}
			}
		}
		[DataMember]
		public Nullable<int> ZoomLevel {
			get {
				return zoomLevelField;
			}
			set {
				if((zoomLevelField.Equals(value) != true)) {
					zoomLevelField = value;
					RaisePropertyChanged("ZoomLevel");
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			PropertyChangedEventHandler propertyChanged = PropertyChanged;
			if((propertyChanged != null))
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
	[DataContract(Name = "ImageryMetadataRequest", Namespace = BingServicesNamespaces.ImageryNamespace)]
	public class ImageryMetadataRequest : RequestBase {
		ImageryMetadataOptions optionsField;
		MapStyle styleField;
		[DataMemberAttribute()]
		public ImageryMetadataOptions Options {
			get {
				return optionsField;
			}
			set {
				if((ReferenceEquals(optionsField, value) != true)) {
					optionsField = value;
					RaisePropertyChanged("Options");
				}
			}
		}
		[DataMemberAttribute()]
		public MapStyle Style {
			get {
				return styleField;
			}
			set {
				if((styleField.Equals(value) != true)) {
					styleField = value;
					RaisePropertyChanged("Style");
				}
			}
		}
	}
	[ServiceContract(Namespace = BingServicesNamespaces.ImageryContractNamespace, ConfigurationName = "BingImageryService.IImageryService")]
	public interface IImageryService {
		[OperationContract(AsyncPattern = true,
						   Action = BingServicesNamespaces.ImageryContractNamespace + "/IImageryService/GetImageryMetadata",
						   ReplyAction = BingServicesNamespaces.ImageryContractNamespace + "/IImageryService/GetImageryMetadataResponse")]
		[FaultContract(typeof(ResponseSummary),
					   Action = BingServicesNamespaces.ImageryContractNamespace + "/IImageryService/GetImageryMetadataResponseSummaryFault",
					   Name = "ResponseSummary",
					   Namespace = BingServicesNamespaces.CommonNamespace)]
		IAsyncResult BeginGetImageryMetadata(ImageryMetadataRequest request, AsyncCallback callback, object asyncState);
		ImageryMetadataResponse EndGetImageryMetadata(IAsyncResult result);
		[OperationContract(AsyncPattern = true,
						   Action = BingServicesNamespaces.ImageryContractNamespace + "/IImageryService/GetMapUri",
						   ReplyAction = BingServicesNamespaces.ImageryContractNamespace + "/IImageryService/GetMapUriResponse")]
		[FaultContract(typeof(ResponseSummary), Action = BingServicesNamespaces.ImageryContractNamespace + "/IImageryService/GetMapUriResponseSummaryFault",
					   Name = "ResponseSummary",
					   Namespace = BingServicesNamespaces.CommonNamespace)]
		IAsyncResult BeginGetMapUri(MapUriRequest request, AsyncCallback callback, object asyncState);
		MapUriResponse EndGetMapUri(IAsyncResult result);
	}
	public class GetImageryMetadataCompletedEventArgs : AsyncCompletedEventArgs {
		object[] results;
		public GetImageryMetadataCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) :
			base(exception, cancelled, userState) {
			this.results = results;
		}
		public ImageryMetadataResponse Result {
			get {
				base.RaiseExceptionIfNecessary();
				return ((ImageryMetadataResponse)(results[0]));
			}
		}
	}
	public class GetMapUriCompletedEventArgs : AsyncCompletedEventArgs {
		object[] results;
		public GetMapUriCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) :
			base(exception, cancelled, userState) {
			this.results = results;
		}
		public MapUriResponse Result {
			get {
				base.RaiseExceptionIfNecessary();
				return ((MapUriResponse)(results[0]));
			}
		}
	}
	public class ImageryServiceClient : ClientBase<IImageryService>, IImageryService {
		BeginOperationDelegate onBeginGetImageryMetadataDelegate;
		EndOperationDelegate onEndGetImageryMetadataDelegate;
		SendOrPostCallback onGetImageryMetadataCompletedDelegate;
		BeginOperationDelegate onBeginGetMapUriDelegate;
		EndOperationDelegate onEndGetMapUriDelegate;
		SendOrPostCallback onGetMapUriCompletedDelegate;
		BeginOperationDelegate onBeginOpenDelegate;
		EndOperationDelegate onEndOpenDelegate;
		SendOrPostCallback onOpenCompletedDelegate;
		BeginOperationDelegate onBeginCloseDelegate;
		EndOperationDelegate onEndCloseDelegate;
		SendOrPostCallback onCloseCompletedDelegate;
		public ImageryServiceClient() {
		}
		public ImageryServiceClient(string endpointConfigurationName) :
			base(endpointConfigurationName) {
		}
		public ImageryServiceClient(string endpointConfigurationName, string remoteAddress) :
			base(endpointConfigurationName, remoteAddress) {
		}
		public ImageryServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
			base(endpointConfigurationName, remoteAddress) {
		}
		public ImageryServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
			base(binding, remoteAddress) {
		}
		public event EventHandler<GetImageryMetadataCompletedEventArgs> GetImageryMetadataCompleted;
		public event EventHandler<GetMapUriCompletedEventArgs> GetMapUriCompleted;
		public event EventHandler<AsyncCompletedEventArgs> OpenCompleted;
		public event EventHandler<AsyncCompletedEventArgs> CloseCompleted;
		#region IImageryService implementation
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		IAsyncResult IImageryService.BeginGetImageryMetadata(ImageryMetadataRequest request, AsyncCallback callback, object asyncState) {
			return base.Channel.BeginGetImageryMetadata(request, callback, asyncState);
		}
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		ImageryMetadataResponse IImageryService.EndGetImageryMetadata(IAsyncResult result) {
			return base.Channel.EndGetImageryMetadata(result);
		}
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		IAsyncResult IImageryService.BeginGetMapUri(MapUriRequest request, AsyncCallback callback, object asyncState) {
			return base.Channel.BeginGetMapUri(request, callback, asyncState);
		}
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		MapUriResponse IImageryService.EndGetMapUri(IAsyncResult result) {
			return base.Channel.EndGetMapUri(result);
		}
		#endregion
		IAsyncResult OnBeginGetImageryMetadata(object[] inValues, AsyncCallback callback, object asyncState) {
			ImageryMetadataRequest request = (ImageryMetadataRequest)inValues[0];
			return ((IImageryService)this).BeginGetImageryMetadata(request, callback, asyncState);
		}
		object[] OnEndGetImageryMetadata(IAsyncResult result) {
			ImageryMetadataResponse retVal = ((IImageryService)this).EndGetImageryMetadata(result);
			return new object[] { retVal };
		}
		void OnGetImageryMetadataCompleted(object state) {
			if(GetImageryMetadataCompleted != null) {
				InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)state);
				GetImageryMetadataCompleted(this, new GetImageryMetadataCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
			}
		}
		IAsyncResult OnBeginGetMapUri(object[] inValues, AsyncCallback callback, object asyncState) {
			MapUriRequest request = (MapUriRequest)inValues[0];
			return ((IImageryService)this).BeginGetMapUri(request, callback, asyncState);
		}
		object[] OnEndGetMapUri(IAsyncResult result) {
			MapUriResponse retVal = ((IImageryService)this).EndGetMapUri(result);
			return new object[] { retVal };
		}
		void OnGetMapUriCompleted(object state) {
			if(GetMapUriCompleted != null) {
				InvokeAsyncCompletedEventArgs e = (InvokeAsyncCompletedEventArgs)state;
				GetMapUriCompleted(this, new GetMapUriCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
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
			if(CloseCompleted != null) {
				InvokeAsyncCompletedEventArgs e = (InvokeAsyncCompletedEventArgs)state;
				CloseCompleted(this, new AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
			}
		}
		public void GetImageryMetadataAsync(ImageryMetadataRequest request) {
			GetImageryMetadataAsync(request, null);
		}
		public void GetImageryMetadataAsync(ImageryMetadataRequest request, object userState) {
			if(onBeginGetImageryMetadataDelegate == null)
				onBeginGetImageryMetadataDelegate = new BeginOperationDelegate(OnBeginGetImageryMetadata);
			if(onEndGetImageryMetadataDelegate == null)
				onEndGetImageryMetadataDelegate = new EndOperationDelegate(OnEndGetImageryMetadata);
			if(onGetImageryMetadataCompletedDelegate == null)
				onGetImageryMetadataCompletedDelegate = new SendOrPostCallback(OnGetImageryMetadataCompleted);
			base.InvokeAsync(onBeginGetImageryMetadataDelegate,
				new object[] { request }, onEndGetImageryMetadataDelegate, onGetImageryMetadataCompletedDelegate, userState);
		}
		public void GetMapUriAsync(MapUriRequest request) {
			GetMapUriAsync(request, null);
		}
		public void GetMapUriAsync(MapUriRequest request, object userState) {
			if(onBeginGetMapUriDelegate == null)
				onBeginGetMapUriDelegate = new BeginOperationDelegate(OnBeginGetMapUri);
			if(onEndGetMapUriDelegate == null)
				onEndGetMapUriDelegate = new EndOperationDelegate(OnEndGetMapUri);
			if(onGetMapUriCompletedDelegate == null)
				onGetMapUriCompletedDelegate = new SendOrPostCallback(OnGetMapUriCompleted);
			base.InvokeAsync(onBeginGetMapUriDelegate,
				new object[] { request }, onEndGetMapUriDelegate, onGetMapUriCompletedDelegate, userState);
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
			base.InvokeAsync(onBeginOpenDelegate, null, onEndOpenDelegate, onOpenCompletedDelegate, userState);
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
			base.InvokeAsync(onBeginCloseDelegate, null, onEndCloseDelegate, onCloseCompletedDelegate, userState);
		}
	}
}
namespace DevExpress.Map.Native {
	public enum BingMap {
		Road,
		Area,
		Hybrid
	}
	public class BingImageryServiceInfo {
		const string defaultUrlTemplate = @"http://t{subdomain}.tiles.virtualearth.net/tiles/{kind}{quadkey}.jpeg?g=000&mkt={culture}&shading=hill&stl=H&token={token}";
		const int defaultMaxZoomLevel = 20;
		const int defaultTileSize = 256;
		static string[] defaultSubdomains = new string[] { "0", "1", "2", "3" };
		public static string GetMapKindModificator(BingMap mapKind) {
			switch(mapKind) {
				case BingMap.Road: return "r";
				case BingMap.Area: return "a";
				case BingMap.Hybrid: return "h";
				default: goto case BingMap.Hybrid;
			}
		}
		readonly List<string> subdomains = new List<string>();
		readonly string urlTemplate;
		readonly int maxZoomLevel;
		readonly int tileHeight;
		readonly int tileWidth;
		readonly string key;
		readonly bool isDefaultInfo;
		public List<string> Subdomains { get { return subdomains; } }
		public string UrlTemplate { get { return urlTemplate; } }
		public int MaxZoomLevel { get { return maxZoomLevel; } }
		public int TileHeight { get { return tileHeight; } }
		public int TileWidth { get { return tileWidth; } }
		public int ImageHeight { get { return (int)Math.Pow(2.0, maxZoomLevel) * tileHeight; } }
		public int ImageWidth { get { return (int)Math.Pow(2.0, maxZoomLevel) * tileWidth; } }
		public string Key { get { return key; } }
		public bool IsDefaultInfo { get { return isDefaultInfo; } }
		BingImageryServiceInfo(string urlTemplate, string key, IEnumerable<string> subdomains, int maxZoomLevel, int tileWidth, int tileHeight) {
			this.urlTemplate = urlTemplate;
			this.key = key;
			this.maxZoomLevel = maxZoomLevel;
			this.tileHeight = tileHeight;
			this.tileWidth = tileWidth;
			foreach(string str in subdomains)
				this.subdomains.Add(str);
		}
		public BingImageryServiceInfo(BingMap kind, string key)
			: this(defaultUrlTemplate.Replace("{kind}", GetMapKindModificator(kind)), key, defaultSubdomains, defaultMaxZoomLevel, defaultTileSize, defaultTileSize) {
			isDefaultInfo = true;
		}
		public BingImageryServiceInfo(XDocument xDocument, string key) {
			this.key = key;
			string bingNamespace = "http://schemas.microsoft.com/search/local/ws/rest/v1";
			var response = xDocument.Element(XName.Get("Response", bingNamespace));
			var statusCode = response.Element(XName.Get("StatusCode", bingNamespace));
			if(statusCode.Value != "200")
				throw new Exception("Wrong status code");
			var imageryMD = response.Element(XName.Get("ResourceSets", bingNamespace)).
									 Element(XName.Get("ResourceSet", bingNamespace)).
									 Element(XName.Get("Resources", bingNamespace)).
									 Element(XName.Get("ImageryMetadata", bingNamespace));
			this.urlTemplate = imageryMD.Element(XName.Get("ImageUrl", bingNamespace)).Value;
			this.tileWidth = int.Parse(imageryMD.Element(XName.Get("ImageWidth", bingNamespace)).Value);
			this.tileHeight = int.Parse(imageryMD.Element(XName.Get("ImageHeight", bingNamespace)).Value);
			this.maxZoomLevel = int.Parse(imageryMD.Element(XName.Get("ZoomMax", bingNamespace)).Value);
			foreach(var subdomain in imageryMD.Element(XName.Get("ImageUrlSubdomains", bingNamespace)).Elements(XName.Get("string", bingNamespace)))
				this.subdomains.Add(subdomain.Value);
		}
	}
}
