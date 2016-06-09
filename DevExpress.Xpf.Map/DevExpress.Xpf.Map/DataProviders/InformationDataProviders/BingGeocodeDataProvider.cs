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
#if UseSoap
using DevExpress.Map.BingServices;
#endif
using DevExpress.Map.Native;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Reflection;
namespace DevExpress.Xpf.Map.Native {
	public class GeocodeRequestContext {
		readonly MapPushpin marker;
		readonly GeoPoint location;
		readonly InformationLayer layer;
		readonly object userState;
		public MapPushpin Marker { get { return marker; } }
		public GeoPoint Location { get { return location; } }
		public InformationLayer Layer { get { return layer; } }
		public object UserState { get { return userState; } }
		public GeocodeRequestContext(InformationLayer layer, MapPushpin marker, GeoPoint location) : this(layer, marker, location, null) {
		}
		public GeocodeRequestContext(InformationLayer layer, MapPushpin marker, GeoPoint location, object userState) {
			this.layer = layer;
			this.marker = marker;
			this.location = location;
			this.userState = userState;
		}
		public void PlaceMarker() {
			if ((Layer != null) && (Marker != null))
				Layer.PlaceItem(Marker);
		}
		public void RemoveMarker() {
			if ((Layer != null) && (Marker != null))
				Layer.RemoveItem(Marker);
		}
	}
}
namespace DevExpress.Xpf.Map {
	public class GeocodeRequestResult : RequestResultBase {
		readonly LocationInformation[] locations;
		public LocationInformation[] Locations { get { return locations; } }
		public GeocodeRequestResult(RequestResultCode resultCode, string faultReason, LocationInformation[] locations) : base(resultCode, faultReason) {
			this.locations = locations;
		}
	}
	public class LocationInformationReceivedEventArgs : AsyncCompletedEventArgs {
		readonly GeocodeRequestResult result;
		public GeocodeRequestResult Result { get { return result; } }
		public LocationInformationReceivedEventArgs(GeocodeRequestResult result, Exception error, bool cancelled, object userState) : base(error, cancelled, userState) {
			this.result = result;
		}
	}
	public delegate void LocationInformationReceivedEventHandler(object sender, LocationInformationReceivedEventArgs e);
	public class BingGeocodeDataProvider : InformationDataProviderBase {
#if !UseSoap
		const string RESTuri = "http://dev.virtualearth.net/REST/v1/Locations";
#endif
		public static readonly DependencyProperty BingKeyProperty = DependencyPropertyManager.Register("BingKey",
			typeof(string), typeof(BingGeocodeDataProvider), new PropertyMetadata(null, BingKeyChanged));
		public static readonly DependencyProperty ProcessMouseEventsProperty = DependencyPropertyManager.Register("ProcessMouseEvents",
			typeof(bool), typeof(BingGeocodeDataProvider), new PropertyMetadata(true, ProcessMouseEventsPropertyChanged));
		public static readonly DependencyProperty MaxVisibleResultCountProperty = DependencyPropertyManager.Register("MaxVisibleResultCount",
			typeof(int), typeof(BingGeocodeDataProvider), new PropertyMetadata(3, MaxVisibleResultCountPropertyChanged));
#if UseSoap
		GeocodeServiceClient geocodeService;
#else
		RESTClient client;
		GeocodeRequestContext requestContext;
#endif
		bool isKeyRestricted = false;
		string ActualBingKey { get { return isKeyRestricted ? string.Empty : BingKey; } }
		public override bool IsBusy { get { return false; } }
		protected internal override int MaxVisibleResultCountInternal { get { return MaxVisibleResultCount; } }
		[Category(Categories.Behavior)]
		public string BingKey {
			get { return (string)GetValue(BingKeyProperty); }
			set { SetValue(BingKeyProperty, value); }
		}
		[Category(Categories.Behavior)]
		public bool ProcessMouseEvents {
			get { return (bool)GetValue(ProcessMouseEventsProperty); }
			set { SetValue(ProcessMouseEventsProperty, value); }
		}
		[Category(Categories.Behavior)]
		public int MaxVisibleResultCount {
			get { return (int)GetValue(MaxVisibleResultCountProperty); }
			set { SetValue(MaxVisibleResultCountProperty, value); }
		}
		public event LocationInformationReceivedEventHandler LocationInformationReceived;
		static void BingKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			BingGeocodeDataProvider geocodeProvider = d as BingGeocodeDataProvider;
			if(geocodeProvider != null) {
				Assembly asm = Assembly.GetEntryAssembly();
				geocodeProvider.isKeyRestricted = DXBingKeyVerifier.IsKeyRestricted(geocodeProvider.BingKey, asm != null ? asm.FullName : string.Empty);
			}
			NotifyPropertyChanged(d, e);
		}
		static void ProcessMouseEventsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			BingGeocodeDataProvider geocodeProvider = d as BingGeocodeDataProvider;
			if (geocodeProvider != null)
				geocodeProvider.ProcessMouseEventsChanged();
		}
		static void MaxVisibleResultCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			BingGeocodeDataProvider geocodeProvider = d as BingGeocodeDataProvider;
			if (geocodeProvider != null)
				geocodeProvider.MaxVisibleResultCountChanged();
		}
		static BingGeocodeDataProvider() {
			DXBingKeyVerifier.RegisterPlatform("Xpf");
		}		
		public BingGeocodeDataProvider() {
		}
#if UseSoap
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
			geocodeService = new GeocodeServiceClient(binding, address);
			geocodeService.ReverseGeocodeCompleted += OnReverseGeocodeCompleted;
		}
#else
		string GetLocation(GeoPoint point) {
			return point.Latitude.ToString(CultureInfo.InvariantCulture) + "," + point.Longitude.ToString(CultureInfo.InvariantCulture);
		}
		void InitGeocodeService(GeoPoint location) {
			if(client != null)
				UnsubscribeEvents();
			this.client = new RESTClient(WebServiceHelper.CorrectScheme(RESTuri));
			client.Parameters.Add(GetLocation(location));
			client.Arguments.Add("includeNeighborhood", "1");
			client.Arguments.Add("include", "ciso2");
			client.Arguments.Add("o", "xml");
			client.Arguments.Add("c", ActualCultureName);
			client.Arguments.Add("key", ActualBingKey);
			SubscribeClientEvents();
		}
		void SubscribeClientEvents() {
			client.Response += OnGeocodeClientResponse;
			client.Error += OnGeocodeClientError;
		}
		void UnsubscribeEvents() {
			client.Error -= OnGeocodeClientError;
			client.Response -= OnGeocodeClientResponse;
		}
		void OnGeocodeClientResponse(object sender, RESTClientResponseEventArgs e) {
			BingGeocodeServiceInfo info = new BingGeocodeServiceInfo(e.XDocument);
			OnReverseGeocodeCompleted(info, new AsyncCompletedEventArgs(null, false, null));
		}
		void OnGeocodeClientError(object sender, AsyncCompletedEventArgs e) {
			OnReverseGeocodeCompleted(null, e);
		}
#endif
		Brush CreateDefaultTraceStroke() {
			GradientStopCollection fadeBrushStops = new GradientStopCollection()
			{
				new GradientStop() { Color = Color.FromArgb(0, 0, 255, 255), Offset = 0.0 },
				new GradientStop() { Color = Color.FromArgb(30, 0, 255, 255), Offset = 0.3 },
				new GradientStop() { Color = Color.FromArgb(255, 0, 255, 255), Offset = 1.0 }
			};
			RadialGradientBrush fadeBrush = new RadialGradientBrush();
			fadeBrush.GradientStops = fadeBrushStops;
			fadeBrush.GradientOrigin = new Point(0, 0);
			fadeBrush.Center = new Point(0, 0);
			fadeBrush.RadiusX = 1;
			fadeBrush.RadiusY = 1;
			return fadeBrush;
		}
		StrokeStyle CreateDefaultTraceStrokeStyle() {
			StrokeStyle strokeStyle = new StrokeStyle();
			strokeStyle.DashArray = new DoubleCollection()
			{
				0.05,
				2
			};
			strokeStyle.DashCap = PenLineCap.Round;
			strokeStyle.StartLineCap = PenLineCap.Round;
			strokeStyle.EndLineCap = PenLineCap.Round;
			strokeStyle.Thickness = 5;
			return strokeStyle;
		}
		MapPushpin CreatePushpin(GeoPoint requestLocation, GeoPoint actualLocation, object content) {
			MapPushpin pushpin = new MapPushpin();
			pushpin.Location = requestLocation;
			pushpin.Information = content;
			pushpin.TraceDepth = 1;
			pushpin.TraceStroke = CreateDefaultTraceStroke();
			pushpin.TraceStrokeStyle = CreateDefaultTraceStrokeStyle();
			pushpin.LocationChangedAnimation = new PushpinLocationAnimation() { EasingFunction = new SineEase() { EasingMode = EasingMode.EaseOut } };
			pushpin.Location = actualLocation;
			return pushpin;
		}
		void ProcessMouseEventsChanged() {
			if (Layer != null) {
				if (ProcessMouseEvents)
					Layer.MouseLeftClick += LayerMouseLeftClick;
				else
					Layer.MouseLeftClick -= LayerMouseLeftClick;
			}
		}
		void MaxVisibleResultCountChanged() {
			if (Layer != null)
				Layer.VerifyResultCount();
		}
		void RaiseLocationInformationRecieved(LocationInformationReceivedEventArgs args) {
			if (LocationInformationReceived != null)
				LocationInformationReceived(this, args);
		}
		void LayerMouseLeftClick(object sender, LayerMouseEventArgs args) {
			if ((Layer != null) && ProcessMouseEvents) {
				GeocodeRequestContext requestContext = null;
				if (GenerateLayerItems) {
					MapPushpin clickMarker = new MapPushpin();
					clickMarker.State = MapPushpinState.Busy;
					clickMarker.Location = args.GeoPoint;
					RaiseLayerItemsGenerating(new LayerItemsGeneratingEventArgs(new MapItem[] { clickMarker }, null, false, null));
					requestContext = new GeocodeRequestContext(Layer, clickMarker, args.GeoPoint);
					requestContext.PlaceMarker();
				} else
					requestContext = new GeocodeRequestContext(Layer, null, args.GeoPoint);
				RequestLocationInformation(requestContext, CultureName);
			}  
		}
		void RequestLocationInformation(GeocodeRequestContext context, string culture) {
#if UseSoap
			if (HasRequestCompletedSubscribers || (LocationInformationReceived != null)) {
				if(geocodeService == null)
					InitGeocodeService();
				ReverseGeocodeRequest request = new ReverseGeocodeRequest();
				request.Credentials = new Credentials() { ApplicationId = ActualBingKey };
				request.Culture = string.IsNullOrEmpty(culture) ? Thread.CurrentThread.CurrentUICulture.ToString() : culture;
				request.Location = new Location()
				{
					Longitude = context.Location.Longitude,
					Latitude = context.Location.Latitude,
					Altitude = 0
				};
				request.ExecutionOptions = new ExecutionOptions() { SuppressFaults = false };
				geocodeService.ReverseGeocodeAsync(request, context);
			}
#else
			if(HasRequestCompletedSubscribers || (LocationInformationReceived != null)) {
				SetRequestContext(context);
				InitGeocodeService(context.Location);
				client.BeginRequest();
			}
#endif
		}
#if UseSoap
		protected void OnReverseGeocodeCompleted(object sender, ReverseGeocodeCompletedEventArgs e) {
			GeocodeRequestContext layerContext = e.UserState as GeocodeRequestContext;
			layerContext.RemoveMarker();
			List<LocationInformation> infoLocations = new List<LocationInformation>();
			if (e.Result != null) {
				foreach (GeocodeResult result in e.Result.Results) {
					if (result.Locations.Count > 0) {
						GeoPoint pushpinLocation = new GeoPoint(result.Locations[0].Latitude, result.Locations[0].Longitude);
						infoLocations.Add(new LocationInformation(pushpinLocation, result.EntityType, result.DisplayName, BingServicesUtils.ConvertToBingAddress(result.Address)));
					}
				}
			}
			GeocodeRequestResult requestResult;
			if (e.Result != null)
				requestResult = new GeocodeRequestResult(BingServicesUtils.GetRequestResultCode(e.Result.ResponseSummary.StatusCode), e.Result.ResponseSummary.FaultReason, infoLocations.ToArray());
			else
				requestResult = new GeocodeRequestResult(RequestResultCode.Timeout, "Request timeout", infoLocations.ToArray());
			RaiseLocationInformationRecieved(new LocationInformationReceivedEventArgs(requestResult, e.Error, e.Cancelled, layerContext.UserState));
			List<MapItem> mapItems = new List<MapItem>();
			if (GenerateLayerItems) {
				foreach (LocationInformation locationInfo in infoLocations)
					mapItems.Add(CreatePushpin(layerContext.Location, locationInfo.Location, locationInfo));
				RaiseLayerItemsGenerating(new LayerItemsGeneratingEventArgs(mapItems.ToArray(), e.Error, e.Cancelled, layerContext.UserState));
			}
			RaiseRequestComplete(new RequestCompletedEventArgs(mapItems.ToArray(), e.Error, e.Cancelled, layerContext.UserState));
		}
#else
		protected void SetRequestContext(GeocodeRequestContext requestContext) {
			this.requestContext = requestContext;
		}
		protected virtual void OnReverseGeocodeCompleted(BingGeocodeServiceInfo info, AsyncCompletedEventArgs args) {
			requestContext.RemoveMarker();
			bool success = info != null && info.IsValidResponse;
			IList<LocationData> locationDatas;
			if(success)
				locationDatas = info.Locations;
			else
				locationDatas = new LocationData[0];
			List<LocationInformation> infoLocations = new List<LocationInformation>();
			foreach(LocationData locationData in locationDatas) {
				GeoPoint markerLocation = new GeoPoint(locationData.Point.Latitude, locationData.Point.Longitude);
				infoLocations.Add(new LocationInformation(markerLocation, locationData.EntityType, locationData.Name, locationData.Address.FormattedAddress));
			}
			GeocodeRequestResult requestResult = new GeocodeRequestResult(success ? RequestResultCode.Success : RequestResultCode.Timeout, args.Error != null ? args.Error.ToString() : null, infoLocations.ToArray());
			RaiseLocationInformationRecieved(new LocationInformationReceivedEventArgs(requestResult, args.Error, args.Cancelled, requestContext.UserState));
			List<MapItem> mapItems = new List<MapItem>();
			if(GenerateLayerItems) {
			foreach(LocationInformation locationInfo in infoLocations)
				mapItems.Add(CreatePushpin(requestContext.Location, locationInfo.Location, locationInfo));
				RaiseLayerItemsGenerating(new LayerItemsGeneratingEventArgs(mapItems.ToArray(), args.Error, args.Cancelled, requestContext.UserState));
			}
			RaiseRequestComplete(new RequestCompletedEventArgs(mapItems.ToArray(), args.Error, args.Cancelled, requestContext.UserState));
		}
#endif
		protected override MapDependencyObject CreateObject() {
			return new BingGeocodeDataProvider();
		}
		protected override void InfromationLayerChanged(InformationLayer oldLayer, InformationLayer newLayer) {
			if (ProcessMouseEvents) {
				if (oldLayer != null)
					oldLayer.MouseLeftClick -= LayerMouseLeftClick;
				if (newLayer != null)
					newLayer.MouseLeftClick += LayerMouseLeftClick;
			}
			base.InfromationLayerChanged(oldLayer, newLayer);
		}
		public void RequestLocationInformation(GeoPoint location, object userState) {
			RequestLocationInformation(location, CultureName, userState);
		}
		public void RequestLocationInformation(GeoPoint location, string culture, object userState) {
			RequestLocationInformation(new GeocodeRequestContext(Layer, null, location, userState), culture);	 
		}
		public override void Cancel() {
#if UseSoap
			if(geocodeService != null) {
				geocodeService.ReverseGeocodeCompleted -= OnReverseGeocodeCompleted;
				geocodeService.CloseAsync();
				geocodeService = null;
			}
#else
			if(client != null) {
				UnsubscribeEvents();
				client.AbortRequest();
			}
#endif
		}
	}
}
