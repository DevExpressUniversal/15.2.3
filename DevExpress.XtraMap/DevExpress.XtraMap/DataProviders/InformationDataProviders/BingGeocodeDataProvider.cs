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
using DevExpress.Map.BingServices;
using DevExpress.Map.Native;
using DevExpress.XtraMap.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceModel;
using System.Threading;
namespace DevExpress.XtraMap {
	public class GeocodeRequestResult : RequestResultBase {
		readonly LocationInformation[] locations;
#if !SL
	[DevExpressXtraMapLocalizedDescription("GeocodeRequestResultLocations")]
#endif
		public LocationInformation[] Locations { get { return locations; } }
		public GeocodeRequestResult(RequestResultCode resultCode, string faultReason, LocationInformation[] locations)
			: base(resultCode, faultReason) {
			this.locations = locations;
		}
	}
	public delegate void LocationInformationReceivedEventHandler(object sender, LocationInformationReceivedEventArgs e);
	public class LocationInformationReceivedEventArgs : AsyncCompletedEventArgs {
		readonly GeocodeRequestResult result;
		public GeocodeRequestResult Result { get { return result; } }
		public LocationInformationReceivedEventArgs(GeocodeRequestResult result, Exception error, bool cancelled, object userState)
			: base(error, cancelled, userState) {
			this.result = result;
		}
	}
	public class BingGeocodeDataProvider : BingMapDataProviderBase, IMapClickHandler {
		internal const bool DefaultProcessMouseEvents = true;
		internal const int DefaultMaxVisibleResultCount = 3;
		bool processMouseEvents = DefaultProcessMouseEvents;
		int maxVisibleResultCount = DefaultMaxVisibleResultCount;
		GeocodeServiceClient geocodeService;
		GeocodeServiceClient GeocodeService {
			get { return geocodeService; }
			set {
				if(geocodeService != null) {
					geocodeService.ReverseGeocodeCompleted -= OnReverseGeocodeCompleted;
					geocodeService.CloseAsync();
					geocodeService = null;
				}
				geocodeService = value;
				if(geocodeService != null)
					geocodeService.ReverseGeocodeCompleted += OnReverseGeocodeCompleted;
			} 
		}
		protected internal override int MaxVisibleResultCountInternal { get { return MaxVisibleResultCount; } }
#if !SL
	[DevExpressXtraMapLocalizedDescription("BingGeocodeDataProviderIsBusy")]
#endif
		public override bool IsBusy { get { return false; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("BingGeocodeDataProviderProcessMouseEvents"),
#endif
		Category(SRCategoryNames.Behavior), DefaultValue(DefaultProcessMouseEvents)]
		public bool ProcessMouseEvents {
			get { return processMouseEvents; }
			set {
				if (processMouseEvents == value)
					return;
				processMouseEvents = value;
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("BingGeocodeDataProviderMaxVisibleResultCount"),
#endif
		Category(SRCategoryNames.Behavior), DefaultValue(DefaultMaxVisibleResultCount)]
		public int MaxVisibleResultCount {
			get { return maxVisibleResultCount; }
			set {
				if (maxVisibleResultCount == value)
					return;
				maxVisibleResultCount = value;
				MaxVisibleResultCountChanged();
			}
		}
#if !SL
	[DevExpressXtraMapLocalizedDescription("BingGeocodeDataProviderLocationInformationReceived")]
#endif
		public event LocationInformationReceivedEventHandler LocationInformationReceived;
		public BingGeocodeDataProvider() {
		}
		void InitGeocodeService() {
			BasicHttpSecurityMode mode = WebServiceHelper.SecurityMode;
			BasicHttpBinding binding = new BasicHttpBinding(mode);
			binding.MaxBufferSize = 2147483647;
			binding.MaxReceivedMessageSize = 2147483647L;
			binding.OpenTimeout = new TimeSpan(0, 0, 20);
			binding.CloseTimeout = new TimeSpan(0, 0, 20);
			binding.ReceiveTimeout = new TimeSpan(0, 0, 20);
			binding.SendTimeout = new TimeSpan(0, 0, 20);
			EndpointAddress address = new EndpointAddress(WebServiceHelper.CurrentScheme + "://dev.virtualearth.net/webservices/v1/geocodeservice/geocodeservice.svc");
			GeocodeService = new GeocodeServiceClient(binding, address);
		}
		void MaxVisibleResultCountChanged() {
			if(Layer != null)
				Layer.VerifyResultCount();
		}
		void IMapClickHandler.OnPointClick(MapPoint point, CoordPoint coordPoint) {
			if(!ProcessMouseEvents)
				return;
			GeoPoint geoPoint = (GeoPoint)coordPoint;
			GeocodeRequestContext requestContext = null;
			if(GenerateLayerItems) {
				MapPushpin clickMarker = new MapPushpin();
				clickMarker.Location = coordPoint;
				RaiseLayerItemsGenerating(new LayerItemsGeneratingEventArgs(new MapItem[] { clickMarker }, null, false, null));
				requestContext = new GeocodeRequestContext(Layer, clickMarker, geoPoint);
				requestContext.PlaceMarker();
			} else
				requestContext = new GeocodeRequestContext(Layer, null, geoPoint);
			RequestLocationInformation(requestContext, Thread.CurrentThread.CurrentUICulture.ToString());
		}
		void RequestLocationInformation(GeocodeRequestContext context, string culture) {
			if(GeocodeService == null)
				InitGeocodeService();
			ReverseGeocodeRequest request = new ReverseGeocodeRequest();
			request.Credentials = new Credentials() { ApplicationId = ActualBingKey };
			request.Culture = string.IsNullOrEmpty(culture) ? Thread.CurrentThread.CurrentUICulture.ToString() : culture;
			request.Location = new Location() {
				Longitude = context.Location.Longitude,
				Latitude = context.Location.Latitude,
				Altitude = 0
			};
			request.ExecutionOptions = new ExecutionOptions() { SuppressFaults = false };
			GeocodeService.ReverseGeocodeAsync(request, context);
		}
		void RaiseLocationInformationRecieved(LocationInformationReceivedEventArgs args) {
			if(LocationInformationReceived != null)
				LocationInformationReceived(this, args);
		}
		MapPushpin CreatePushpin(GeoPoint requestLocation, GeoPoint actualLocation, object content) {
			MapPushpin pushpin = new MapPushpin();
			pushpin.Location = requestLocation;
			pushpin.Information = content;
			pushpin.ToolTipPattern = ((LocationInformation)content).DisplayName;
			pushpin.Location = actualLocation;
			return pushpin;
		}
		protected void OnReverseGeocodeCompleted(object sender, ReverseGeocodeCompletedEventArgs e) {
			GeocodeRequestContext layerContext = e.UserState as GeocodeRequestContext;
			layerContext.RemoveMarker();
			List<LocationInformation> infoLocations = new List<LocationInformation>();
			if(e.Result != null) {
				foreach(GeocodeResult result in e.Result.Results) {
					if(result.Locations.Count > 0) {
						GeoPoint pushpinLocation = new GeoPoint(result.Locations[0].Latitude, result.Locations[0].Longitude);
						infoLocations.Add(new LocationInformation(pushpinLocation, result.EntityType, result.DisplayName, ConvertToBingAddress(result.Address)));
					}
				}
			}
			GeocodeRequestResult requestResult;
			if(e.Result != null)
				requestResult = new GeocodeRequestResult(BingServicesUtils.GetRequestResultCode(e.Result.ResponseSummary.StatusCode), e.Result.ResponseSummary.FaultReason, infoLocations.ToArray());
			else
				requestResult = new GeocodeRequestResult(RequestResultCode.Timeout, "Request timeout", infoLocations.ToArray());
			RaiseLocationInformationRecieved(new LocationInformationReceivedEventArgs(requestResult, e.Error, e.Cancelled, layerContext.UserState));
			List<MapItem> mapItems = new List<MapItem>();
			if(GenerateLayerItems) {
				foreach(LocationInformation locationInfo in infoLocations)
					mapItems.Add(CreatePushpin(layerContext.Location, locationInfo.Location, locationInfo));
				RaiseLayerItemsGenerating(new LayerItemsGeneratingEventArgs(mapItems.ToArray(), e.Error, e.Cancelled, layerContext.UserState));
			}
			OnRequestComplete(new RequestCompletedEventArgs(mapItems.ToArray(), e.Error, e.Cancelled, layerContext.UserState));
		}
		protected internal override void Cancel() {
			GeocodeService = null;
		}
		public void RequestLocationInformation(GeoPoint location, object userState) {
			RequestLocationInformation(new GeocodeRequestContext(Layer, null, location, userState), Thread.CurrentThread.CurrentUICulture.ToString());
		}
		public void RequestLocationInformation(GeoPoint location, string culture, object userState) {
			RequestLocationInformation(new GeocodeRequestContext(Layer, null, location, userState), culture);
		}
		public override string ToString() {
			return "(BingGeocodeDataProvider)";
		}
	}
}
namespace DevExpress.XtraMap.Native {
	public class GeocodeRequestContext {
		readonly MapPushpin marker;
		readonly GeoPoint location;
		readonly InformationLayer layer;
		readonly object userState;
		public MapPushpin Marker { get { return marker; } }
		public GeoPoint Location { get { return location; } }
		public InformationLayer Layer { get { return layer; } }
		public object UserState { get { return userState; } }
		public GeocodeRequestContext(InformationLayer layer, MapPushpin marker, GeoPoint location)
			: this(layer, marker, location, null) {
		}
		public GeocodeRequestContext(InformationLayer layer, MapPushpin marker, GeoPoint location, object userState) {
			this.layer = layer;
			this.marker = marker;
			this.location = location;
			this.userState = userState;
		}
		public void PlaceMarker() {
			if((Layer != null) && (Marker != null))
				Layer.PlaceItem(Marker);
		}
		public void RemoveMarker() {
			if((Layer != null) && (Marker != null))
				Layer.RemoveActualItem(Marker);
		}
	}
}
