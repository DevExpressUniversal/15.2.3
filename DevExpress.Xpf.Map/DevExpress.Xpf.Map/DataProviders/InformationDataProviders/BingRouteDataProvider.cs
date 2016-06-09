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
using System.Globalization;
using System.ServiceModel;
using System.Windows;
using System.Windows.Media;
using DevExpress.Map;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Map.BingServices;
using DevExpress.Map.Native;
using System.Reflection;
namespace DevExpress.Xpf.Map {
	public enum BingTravelMode {
		Driving,
		Walking
	}
	public enum BingRouteOptimization{
		MinimizeTime,
		MinimizeDistance
	}
	public enum BingManeuverType {
		None = 0,
		Unknown = 1,
		DepartStart = 2,
		DepartIntermediateStop = 3,
		DepartIntermediateStopReturning = 4,
		ArriveFinish = 5,
		ArriveIntermediateStop = 6,
		TurnLeft = 7,
		TurnRight = 8,
		TurnBack = 9,
		UTurn = 10,
		TurnToStayLeft = 11,
		TurnToStayRight = 12,
		BearLeft = 13,
		BearRight = 14,
		KeepToStayLeft = 15,
		KeepToStayRight = 16,
		KeepToStayStraight = 17,
		KeepLeft = 18,
		KeepRight = 19,
		KeepStraight = 20,
		Take = 21,
		TakeRampLeft = 22,
		TakeRampRight = 23,
		TakeRampStraight = 24,
		KeepOnrampLeft = 25,
		KeepOnrampRight = 26,
		KeepOnrampStraight = 27,
		Merge = 28,
		Continue = 29,
		RoadNameChange = 30,
		EnterRoundabout = 31,
		ExitRoundabout = 32,
		TurnRightThenTurnRight = 33,
		TurnRightThenTurnLeft = 34,
		TurnRightThenBearRight = 35,
		TurnRightThenBearLeft = 36,
		TurnLeftThenTurnLeft = 37,
		TurnLeftThenTurnRight = 38,
		TurnLeftThenBearLeft = 39,
		TurnLeftThenBearRight = 40,
		BearRightThenTurnRight = 41,
		BearRightThenTurnLeft = 42,
		BearRightThenBearRight = 43,
		BearRightThenBearLeft = 44,
		BearLeftThenTurnLeft = 45,
		BearLeftThenTurnRight = 46,
		BearLeftThenBearRight = 47,
		BearLeftThenBearLeft = 48,
		RampThenHighwayRight = 49,
		RampThenHighwayLeft = 50,
		RampToHighwayStraight = 51,
		EnterThenExitRoundabout = 52,
		BearThenMerge = 53,
		TurnThenMerge = 54,
		BearThenKeep = 55,
		Transfer = 56,
		Wait = 57,
		TakeTransit = 58,
		Walk = 59
	}
	public enum BingItineraryWarningType {
		Accident = 0,
		AdminDivisionChange = 1,
		BlockedRoad = 2,
		CheckTimetable = 3,
		Congestion = 4,
		CountryChange = 5,
		DisabledVehicle = 6,
		GateAccess = 7,
		GetOffTransit = 8,
		GetOnTransit = 9,
		IllegalUTurn = 10,
		MassTransit = 11,
		Miscellaneous = 12,
		NoIncident = 13,
		None = 14,
		Other = 15,
		OtherNews = 16,
		OtherTrafficIncidents = 17,
		PlannedEvent = 18,
		PrivateRoad = 19,
		RestrictedTurn = 20,
		RoadClosures = 21,
		RoadHazard = 22,
		ScheduledConstruction = 23,
		SeasonalClosures = 24,
		Tollbooth = 25,
		TollRoad = 26,
		TrafficFlow = 27,
		UnpavedRoad = 28,
		UnscheduledConstruction = 29,
		Weather = 30
	}
	public class BingRouteOptions : MapDependencyObject {
		public static readonly DependencyProperty ModeProperty = DependencyPropertyManager.Register("Mode",
			typeof(BingTravelMode), typeof(BingRouteOptions), new PropertyMetadata(BingTravelMode.Driving));
		public static readonly DependencyProperty RouteOptimizationProperty = DependencyPropertyManager.Register("RouteOptimization",
			typeof(BingRouteOptimization), typeof(BingRouteOptions), new PropertyMetadata(BingRouteOptimization.MinimizeDistance));
		public static readonly DependencyProperty DistanceUnitProperty = DependencyPropertyManager.Register("DistanceUnit",
			typeof(DistanceMeasureUnit), typeof(BingRouteOptions), new PropertyMetadata(DistanceMeasureUnit.Kilometer));
		[Category(Categories.Behavior)]
		public BingTravelMode Mode {
			get { return (BingTravelMode)GetValue(ModeProperty); }
			set { SetValue(ModeProperty, value); }
		}
		[Category(Categories.Behavior)]
		public BingRouteOptimization RouteOptimization {
			get { return (BingRouteOptimization)GetValue(RouteOptimizationProperty); }
			set { SetValue(RouteOptimizationProperty, value); }
		}
		[Category(Categories.Behavior)]
		public DistanceMeasureUnit DistanceUnit {
			get { return (DistanceMeasureUnit)GetValue(DistanceUnitProperty); }
			set { SetValue(DistanceUnitProperty, value); }
		}
		protected override MapDependencyObject CreateObject() {
			return new BingRouteOptions();
		}
	}
	public delegate void BingRouteCalculatedEventHandler(object sender, BingRouteCalculatedEventArgs e);
	public class BingRouteCalculatedEventArgs : AsyncCompletedEventArgs {
		readonly RouteCalculationResult calculationResult;
		public RouteCalculationResult CalculationResult { get { return calculationResult; } }
		public BingRouteCalculatedEventArgs(RouteCalculationResult calculationResult, Exception error, bool cancelled, object userState)
			: base(error, cancelled, userState) {
			this.calculationResult = calculationResult;
		}
	}
	public class RouteCalculationResult : RequestResultBase {
		List<BingRouteResult> routeResults;
		List<RouteWaypoint> startingPoints;
		public List<BingRouteResult> RouteResults { get { return routeResults; } }
		public List<RouteWaypoint> StartingPoints { get { return startingPoints; } }
		public RouteCalculationResult(RequestResultCode statusCode, string faultReason, List<BingRouteResult> routeResults, List<RouteWaypoint> startingPoints)
			: base(statusCode, faultReason) {
			this.routeResults = routeResults;
			this.startingPoints = startingPoints;
		}
	}
	public class RouteWaypoint {
		string description;
		GeoPoint location;
		public string Description { get { return description; } }
		public GeoPoint Location { get { return location; } }
		public RouteWaypoint(string description, GeoPoint location) {
			this.description = description;
			this.location = location;
		}
		public override string ToString() {
			return Description;
		}
	}
	public class BingItineraryItemWarning {
		string text;
		BingItineraryWarningType type;
		public string Text { get { return text; } }
		public BingItineraryWarningType Type { get { return type; } }
		public BingItineraryItemWarning(string text, BingItineraryWarningType type) {
			this.text = text;
			this.type = type;
		}
	}
	public class BingItineraryItem {
		GeoPoint location;
		BingManeuverType maneuver;
		string maneuverInstruction;
		List<BingItineraryItemWarning> warnings;
		double distance;
		TimeSpan time;
		public GeoPoint Location { get { return location; } }
		public BingManeuverType Maneuver { get { return maneuver; } }
		public string ManeuverInstruction { get { return maneuverInstruction; } }
		public List<BingItineraryItemWarning> Warnings { get { return warnings; } }
		public double Distance { get { return distance; } }
		public TimeSpan Time { get { return time; } }
		public BingItineraryItem(GeoPoint location, BingManeuverType maneuver, string maneuverInstruction, List<BingItineraryItemWarning> warnings, double distance, TimeSpan time) {
			this.location = location;
			this.maneuver = maneuver;
			this.maneuverInstruction = maneuverInstruction;
			this.warnings = warnings;
			this.distance = distance;
			this.time = time;
		}
	}
	public class BingRouteLeg {
		GeoPoint startPoint;
		GeoPoint endPoint;
		List<BingItineraryItem> itinerary;
		double distance;
		TimeSpan time;
		public GeoPoint StartPoint { get { return startPoint; } }
		public GeoPoint EndPoint { get { return endPoint; } }
		public List<BingItineraryItem> Itinerary { get { return itinerary; } }
		public double Distance { get { return distance; } }
		public TimeSpan Time { get { return time; } }
		public BingRouteLeg(GeoPoint startPoint, GeoPoint endPoint, List<BingItineraryItem> itinerary, double distance, TimeSpan time) {
			this.startPoint = startPoint;
			this.endPoint = endPoint;
			this.itinerary = itinerary;
			this.distance = distance;
			this.time = time;
		}
	}
	public class BingRouteResult {
		List<GeoPoint> routePath;
		double distance;
		TimeSpan time;
		List<BingRouteLeg> legs;
		public List<GeoPoint> RoutePath { get { return routePath; } }
		public double Distance { get { return distance; } }
		public TimeSpan Time { get { return time; } }
		public List<BingRouteLeg> Legs { get { return legs; } }
		public BingRouteResult(List<GeoPoint> routePath, double distance, TimeSpan time, List<BingRouteLeg> legs) {
			this.routePath = routePath;
			this.distance = distance;
			this.time = time;
			this.legs = legs;
		}
	}
	public class BingRouteDataProvider : InformationDataProviderBase {
		#region Dependency properties
		public static readonly DependencyProperty RouteOptionsProperty = DependencyPropertyManager.Register("RouteOptions",
			typeof(BingRouteOptions), typeof(BingRouteDataProvider), new PropertyMetadata());
		public static readonly DependencyProperty BingKeyProperty = DependencyPropertyManager.Register("BingKey",
			typeof(string), typeof(BingRouteDataProvider), new PropertyMetadata(string.Empty, BingKeyChanged));
		public static readonly DependencyProperty RouteStrokeProperty = DependencyPropertyManager.Register("RouteStroke",
			typeof(Brush), typeof(BingRouteDataProvider), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x8A, 0xFB, 0xFF)), NotifyPropertyChanged));
		public static readonly DependencyProperty RouteStrokeStyleProperty = DependencyPropertyManager.Register("RouteStrokeStyle",
			typeof(StrokeStyle), typeof(BingRouteDataProvider), new PropertyMetadata(new StrokeStyle() { Thickness = 3 }, NotifyPropertyChanged));
		#endregion
		bool isKeyRestricted = false;
		string ActualBingKey { get { return isKeyRestricted ? string.Empty : BingKey; } }
		[Category(Categories.Behavior)]
		public BingRouteOptions RouteOptions {
			get { return (BingRouteOptions)GetValue(RouteOptionsProperty); }
			set { SetValue(RouteOptionsProperty, value); }
		}
		[Category(Categories.Behavior)]
		public string BingKey {
			get { return (string)GetValue(BingKeyProperty); }
			set { SetValue(BingKeyProperty, value); }
		}
		[Category(Categories.Appearance)]
		public Brush RouteStroke {
			get { return (Brush)GetValue(RouteStrokeProperty); }
			set { SetValue(RouteStrokeProperty, value); }
		}
		[Category(Categories.Appearance)]
		public StrokeStyle RouteStrokeStyle {
			get { return (StrokeStyle)GetValue(RouteStrokeStyleProperty); }
			set { SetValue(RouteStrokeStyleProperty, value); }
		}
		BingRouteOptions defaultRouteOptions = new BingRouteOptions();
		RouteServiceClient routeService;
		bool isBusy;
		string startPointDescription;
		public event BingRouteCalculatedEventHandler RouteCalculated;
		BingRouteOptions ActualRouteOptions {
			get { return RouteOptions != null ? RouteOptions : defaultRouteOptions; }
		}
		protected internal override int MaxVisibleResultCountInternal { get { return 1; } }
		public override bool IsBusy { get { return isBusy; } }
		static void BingKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			BingRouteDataProvider routeDataProvider = d as BingRouteDataProvider;
			if(routeDataProvider != null) {
				Assembly asm = Assembly.GetEntryAssembly();
				routeDataProvider.isKeyRestricted = DXBingKeyVerifier.IsKeyRestricted(routeDataProvider.BingKey, asm != null ? asm.FullName : string.Empty);
			}
		}
		static BingRouteDataProvider() {
			DXBingKeyVerifier.RegisterPlatform("Xpf");
		}
		public BingRouteDataProvider() {
			ResetRouteService();
		}
		void ResetRouteService() {
			if (routeService != null) {
				routeService.CalculateRouteCompleted -= OnCalculateRouteCompleted;
				routeService.CalculateRoutesFromMajorRoadsCompleted -= OnCalculateRoutesFromMajorRoadsCompleted;
			}
			BasicHttpSecurityMode mode = WebServiceHelper.SecurityMode;
			BasicHttpBinding binding = new BasicHttpBinding(mode);
			binding.MaxBufferSize = 2147483647;
			binding.MaxReceivedMessageSize = 2147483647L;
			EndpointAddress address = new EndpointAddress(WebServiceHelper.CurrentScheme + "://dev.virtualearth.net/webservices/v1/routeservice/routeservice.svc");
			routeService = new RouteServiceClient(binding, address);
			routeService.CalculateRouteCompleted += OnCalculateRouteCompleted;
			routeService.CalculateRoutesFromMajorRoadsCompleted += OnCalculateRoutesFromMajorRoadsCompleted;
			startPointDescription = "";
		}
		void OnCalculateRouteCompleted(object sender, CalculateRouteCompletedEventArgs e) {
			isBusy = false;
			RouteResponse routeResponce;
			try {
				routeResponce = e.Result;
			}
			catch {
				routeResponce = null;
			}
			RouteCalculationResult calculationResult = (routeResponce != null) ? CreateRequestResult(routeResponce) : null;
			if (RouteCalculated != null)
				RouteCalculated(this, new BingRouteCalculatedEventArgs(calculationResult, e.Error, e.Cancelled, e.UserState));
			SendResultsToLayer(calculationResult, e.Error, e.Cancelled, e.UserState);
		}
		void OnCalculateRoutesFromMajorRoadsCompleted(object sender, CalculateRoutesFromMajorRoadsCompletedEventArgs e) {
			isBusy = false;
			RouteCalculationResult calculationResult = CreateRequestResultFromMajorResponce(e.Result);
			if (RouteCalculated != null)
				RouteCalculated(this, new BingRouteCalculatedEventArgs(calculationResult, e.Error, e.Cancelled, e.UserState));
			if (RouteCalculated != null)
				RouteCalculated(this, new BingRouteCalculatedEventArgs(calculationResult, e.Error, e.Cancelled, e.UserState));
			SendResultsToLayer(calculationResult, e.Error, e.Cancelled, e.UserState);
		}
		void SendResultsToLayer(RouteCalculationResult calculationResult, Exception error, bool cancelled, object userState) {
			List<MapItem> mapItems = CreateMapItems(calculationResult);
			if (GenerateLayerItems && mapItems.Count > 0)
				RaiseLayerItemsGenerating(new LayerItemsGeneratingEventArgs(mapItems.ToArray(), error, cancelled, userState));
			RaiseRequestComplete(new RequestCompletedEventArgs(mapItems.ToArray(), error, cancelled, userState));
		}
		List<MapItem> CreateMapItems(RouteCalculationResult calculationResult) {
			List<MapItem> mapItems = new List<MapItem>();
			if (GenerateLayerItems && calculationResult != null) {
				if (calculationResult.RouteResults != null) {
					foreach (BingRouteResult routeResult in calculationResult.RouteResults) {
						MapPolyline path = new MapPolyline();
						path.Stroke = RouteStroke;
						path.HighlightStroke = RouteStroke;
						path.StrokeStyle = RouteStrokeStyle;
						path.HighlightStrokeStyle = RouteStrokeStyle;
						foreach (GeoPoint point in routeResult.RoutePath)
							path.Points.Add(point);
						mapItems.Add(path);
					}
				}
				if (calculationResult.StartingPoints != null)
					foreach (RouteWaypoint startWaypoint in calculationResult.StartingPoints) {
						MapPushpin start = new MapPushpin();
						start.Location = startWaypoint.Location;
						start.Information = startWaypoint.Description;
						mapItems.Add(start);
					}
				if (calculationResult.RouteResults != null) {
					foreach (BingRouteResult routeResult in calculationResult.RouteResults) {
						if (routeResult.Legs != null)
							foreach (BingRouteLeg leg in routeResult.Legs)
								if (leg.EndPoint != null) {
									MapPushpin end = new MapPushpin();
									end.Location = leg.EndPoint;
									mapItems.Add(end);
								}
					}
				}
			}
			return mapItems;
		}
		RouteCalculationResult CreateRequestResult(RouteResponse routeResponce) {
			RequestResultCode resultCode = BingServicesUtils.GetRequestResultCode(routeResponce.ResponseSummary.StatusCode);
			List<BingRouteResult> routeResults = new List<BingRouteResult>();
			List<RouteWaypoint> startingPoints = new List<RouteWaypoint>();
			if (resultCode == RequestResultCode.Success && routeResponce.Result != null) {
				BingRouteResult result = CreateBingRouteResult(routeResponce.Result);
				routeResults.Add(result);
				GeoPoint startPointLocation = new GeoPoint();
				if(result.RoutePath.Count > 0)
					startPointLocation = result.RoutePath[0];
				startingPoints.Add(new RouteWaypoint(startPointDescription,
					startPointLocation));
			}
			return new RouteCalculationResult(resultCode, routeResponce.ResponseSummary.FaultReason, routeResults, startingPoints);
		}
		RouteCalculationResult CreateRequestResultFromMajorResponce(MajorRoutesResponse routeResponce) {
			RequestResultCode resultCode = BingServicesUtils.GetRequestResultCode(routeResponce.ResponseSummary.StatusCode);
			List<BingRouteResult> routeResults = new List<BingRouteResult>();
			List<RouteWaypoint> startingPoints = new List<RouteWaypoint>();
			if (resultCode == RequestResultCode.Success && routeResponce.Routes != null) {
				foreach (RouteResult result in routeResponce.Routes)
					routeResults.Add(CreateBingRouteResult(result));
				if (routeResponce.StartingPoints != null)
					foreach (Waypoint waypoint in routeResponce.StartingPoints)
						startingPoints.Add(new RouteWaypoint(waypoint.Description, BingServicesUtils.ConvertLocationToGeoPoint(waypoint.Location)));
			}
			else
				return new RouteCalculationResult(resultCode, routeResponce.ResponseSummary.FaultReason, routeResults, startingPoints);
			return new RouteCalculationResult(resultCode, routeResponce.ResponseSummary.FaultReason, routeResults, startingPoints);
		}
		BingRouteResult CreateBingRouteResult(RouteResult routeResult) {
			List<GeoPoint> routePath = new List<GeoPoint>();
			if (routeResult.RoutePath != null)
				foreach (Location point in routeResult.RoutePath.Points)
					routePath.Add(new GeoPoint(point.Latitude, point.Longitude));
			double distance = 0.0;
			TimeSpan time = TimeSpan.Zero;
			if (routeResult.Summary != null) {
				distance = routeResult.Summary.Distance;
				time = TimeSpan.FromSeconds(routeResult.Summary.TimeInSeconds);
			}
			List<BingRouteLeg> legs = new List<BingRouteLeg>();
			if (routeResult.Legs != null)
				foreach (RouteLeg leg in routeResult.Legs) {
					if (leg.Summary != null) {
						List<BingItineraryItem> itineraryItems = new List<BingItineraryItem>();
						if (leg.Itinerary != null)
							foreach (ItineraryItem itineraryItem in leg.Itinerary) {
								double itineraryDistance = 0.0;
								TimeSpan itineraryTime = new TimeSpan();
								if (itineraryItem.Summary != null) {
									itineraryDistance = itineraryItem.Summary.Distance;
									itineraryTime = TimeSpan.FromSeconds(itineraryItem.Summary.TimeInSeconds);
								}
								List<BingItineraryItemWarning> warnings = new List<BingItineraryItemWarning>();
								if (itineraryItem.Warnings != null)
									foreach (ItineraryItemWarning warning in itineraryItem.Warnings)
										warnings.Add(new BingItineraryItemWarning(warning.Text,
											BingServicesUtils.ConvertItineraryWarningTypeToBingItineraryWarningType(warning.WarningType)));
								itineraryItems.Add(new BingItineraryItem(
									BingServicesUtils.ConvertLocationToGeoPoint(itineraryItem.Location),
									BingServicesUtils.ConvertManeuverTypeToBingManeuverType(itineraryItem.ManeuverType),
									itineraryItem.Text,
									warnings,
									itineraryDistance,
									itineraryTime));
							}
						legs.Add(new BingRouteLeg(
							BingServicesUtils.ConvertLocationToGeoPoint(leg.ActualStart),
							BingServicesUtils.ConvertLocationToGeoPoint(leg.ActualEnd),
							itineraryItems,
							leg.Summary.Distance,
							TimeSpan.FromSeconds(leg.Summary.TimeInSeconds)));
					}
				}
			return new BingRouteResult(routePath, distance, time, legs);
		}
		RouteOptions SetRouteOptions(RouteOptions options) {
			options.RoutePathType = RoutePathType.Points;
			if (ActualRouteOptions.Mode == BingTravelMode.Driving)
				options.Mode = TravelMode.Driving;
			else
				options.Mode = TravelMode.Walking;
			if (ActualRouteOptions.RouteOptimization == BingRouteOptimization.MinimizeDistance)
				options.Optimization = RouteOptimization.MinimizeDistance;
			else
				options.Optimization = RouteOptimization.MinimizeTime;
			return options;
		}
		protected override MapDependencyObject CreateObject() {
			return new BingRouteDataProvider();
		}
		public void CalculateRoute(List<RouteWaypoint> waypoints) {
			CalculateRoute(waypoints, CultureName, null);
		}
		public void CalculateRoute(List<RouteWaypoint> waypoints, object userState) {
			CalculateRoute(waypoints, CultureName, userState);
		}
		public void CalculateRoute(List<RouteWaypoint> waypoints, string culture, object userState) {
			if (!isBusy) {
				isBusy = true;
				RouteRequest request = new RouteRequest();
				request.Credentials = new Credentials();
				request.Credentials.ApplicationId = ActualBingKey;
				request.Culture = string.IsNullOrEmpty(culture) ? CultureInfo.InvariantCulture.Name : culture;
				request.ExecutionOptions = new ExecutionOptions() { SuppressFaults = true };
				List<Waypoint> bingWaypoints = new List<Waypoint>();
				foreach (RouteWaypoint routePoint in waypoints) {
					Waypoint waypoint = new Waypoint();
					waypoint.Description = routePoint.Description;
					waypoint.Location = new Location();
					waypoint.Location.Latitude = routePoint.Location.Latitude;
					waypoint.Location.Longitude = routePoint.Location.Longitude;
					bingWaypoints.Add(waypoint);
				}
				request.Waypoints = bingWaypoints.ToArray();
				startPointDescription = "";
				if (waypoints.Count > 0)
					startPointDescription = waypoints[0].Description;
				RouteOptions options = new RouteOptions();
				options = SetRouteOptions(options);
				request.Options = options;
				UserProfile profile = new UserProfile();
				profile.DistanceUnit = BingServicesUtils.ConvertDistanceMeasureUnitToDistanceUnit(ActualRouteOptions.DistanceUnit);
				request.UserProfile = profile;
				routeService.CalculateRouteAsync(request, userState);
			}
		}
		public void CalculateRoutesFromMajorRoads(RouteWaypoint destination) {
			CalculateRoutesFromMajorRoads(destination, CultureName, null);
		}
		public void CalculateRoutesFromMajorRoads(RouteWaypoint destination, object userState) {
			CalculateRoutesFromMajorRoads(destination, CultureName, userState);
		}
		public void CalculateRoutesFromMajorRoads(RouteWaypoint destination, string culture, object userState) {
			if (!isBusy) {
				isBusy = true;
				MajorRoutesRequest request = new MajorRoutesRequest();
				request.Credentials = new Credentials();
				request.Credentials.ApplicationId = ActualBingKey;
				request.Culture = string.IsNullOrEmpty(culture) ? CultureInfo.InvariantCulture.Name : culture;
				request.ExecutionOptions = new ExecutionOptions() { SuppressFaults = true };
				Waypoint endPoint = new Waypoint();
				endPoint.Location = new Location();
				endPoint.Location.Latitude = destination.Location.Latitude;
				endPoint.Location.Longitude = destination.Location.Longitude;
				endPoint.Description = destination.Description;
				request.Destination = endPoint;
				MajorRoutesOptions options = new MajorRoutesOptions();
				options.ReturnRoutes = true;
				options = (MajorRoutesOptions)SetRouteOptions(options);
				request.Options = options;
				UserProfile profile = new UserProfile();
				request.UserProfile = profile;
				profile.DistanceUnit = BingServicesUtils.ConvertDistanceMeasureUnitToDistanceUnit(ActualRouteOptions.DistanceUnit);
				routeService.CalculateRoutesFromMajorRoadsAsync(request, userState);
			}
		}
		public override void Cancel() {
			ResetRouteService();
			isBusy = false;
		}
	}
}
