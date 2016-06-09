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
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using SendOrPostCallback = System.Threading.SendOrPostCallback;
namespace DevExpress.Map.BingServices {
	[DataContract(Name = "TravelMode", Namespace = BingServicesNamespaces.RouteNamespace)]
	public enum TravelMode : int {
		[EnumMember]
		Driving = 0,
		[EnumMember]
		Walking = 1,
	}
	[DataContract(Name = "RouteOptimization", Namespace = BingServicesNamespaces.RouteNamespace)]
	public enum RouteOptimization : int {
		[EnumMember]
		MinimizeTime = 0,
		[EnumMember]
		MinimizeDistance = 1,
	}
	[DataContract(Name = "RoutePathType", Namespace = BingServicesNamespaces.RouteNamespace)]
	public enum RoutePathType : int {
		[EnumMember]
		None = 0,
		[EnumMember]
		Points = 1,
	}
	[DataContract(Name = "TrafficUsage", Namespace = BingServicesNamespaces.RouteNamespace)]
	public enum TrafficUsage : int {
		[EnumMember]
		None = 0,
		[EnumMember]
		TrafficBasedTime = 1,
		[EnumMember]
		TrafficBasedRouteAndTime = 2,
	}
	[DataContract(Name = "ManeuverType", Namespace = BingServicesNamespaces.RouteNamespace)]
	public enum ManeuverType : int {
		[EnumMember]
		None = 0,
		[EnumMember]
		Unknown = 1,
		[EnumMember]
		DepartStart = 2,
		[EnumMember]
		DepartIntermediateStop = 3,
		[EnumMember]
		DepartIntermediateStopReturning = 4,
		[EnumMember]
		ArriveFinish = 5,
		[EnumMember]
		ArriveIntermediateStop = 6,
		[EnumMember]
		TurnLeft = 7,
		[EnumMember]
		TurnRight = 8,
		[EnumMember]
		TurnBack = 9,
		[EnumMember]
		UTurn = 10,
		[EnumMember]
		TurnToStayLeft = 11,
		[EnumMember]
		TurnToStayRight = 12,
		[EnumMember]
		BearLeft = 13,
		[EnumMember]
		BearRight = 14,
		[EnumMember]
		KeepToStayLeft = 15,
		[EnumMember]
		KeepToStayRight = 16,
		[EnumMember]
		KeepToStayStraight = 17,
		[EnumMember]
		KeepLeft = 18,
		[EnumMember]
		KeepRight = 19,
		[EnumMember]
		KeepStraight = 20,
		[EnumMember]
		Take = 21,
		[EnumMember]
		TakeRampLeft = 22,
		[EnumMember]
		TakeRampRight = 23,
		[EnumMember]
		TakeRampStraight = 24,
		[EnumMember]
		KeepOnrampLeft = 25,
		[EnumMember]
		KeepOnrampRight = 26,
		[EnumMember]
		KeepOnrampStraight = 27,
		[EnumMember]
		Merge = 28,
		[EnumMember]
		Continue = 29,
		[EnumMember]
		RoadNameChange = 30,
		[EnumMember]
		EnterRoundabout = 31,
		[EnumMember]
		ExitRoundabout = 32,
		[EnumMember]
		TurnRightThenTurnRight = 33,
		[EnumMember]
		TurnRightThenTurnLeft = 34,
		[EnumMember]
		TurnRightThenBearRight = 35,
		[EnumMember]
		TurnRightThenBearLeft = 36,
		[EnumMember]
		TurnLeftThenTurnLeft = 37,
		[EnumMember]
		TurnLeftThenTurnRight = 38,
		[EnumMember]
		TurnLeftThenBearLeft = 39,
		[EnumMember]
		TurnLeftThenBearRight = 40,
		[EnumMember]
		BearRightThenTurnRight = 41,
		[EnumMember]
		BearRightThenTurnLeft = 42,
		[EnumMember]
		BearRightThenBearRight = 43,
		[EnumMember]
		BearRightThenBearLeft = 44,
		[EnumMember]
		BearLeftThenTurnLeft = 45,
		[EnumMember]
		BearLeftThenTurnRight = 46,
		[EnumMember]
		BearLeftThenBearRight = 47,
		[EnumMember]
		BearLeftThenBearLeft = 48,
		[EnumMember]
		RampThenHighwayRight = 49,
		[EnumMember]
		RampThenHighwayLeft = 50,
		[EnumMember]
		RampToHighwayStraight = 51,
		[EnumMember]
		EnterThenExitRoundabout = 52,
		[EnumMember]
		BearThenMerge = 53,
		[EnumMember]
		TurnThenMerge = 54,
		[EnumMember]
		BearThenKeep = 55,
		[EnumMember]
		Transfer = 56,
		[EnumMember]
		Wait = 57,
		[EnumMember]
		TakeTransit = 58,
		[EnumMember]
		Walk = 59,
	}
	[DataContract(Name = "ItineraryItemHintType", Namespace = BingServicesNamespaces.RouteNamespace)]
	public enum ItineraryItemHintType : int {
		[EnumMember]
		PreviousIntersection = 0,
		[EnumMember]
		NextIntersection = 1,
		[EnumMember]
		Landmark = 2,
	}
	[DataContract(Name = "ItineraryWarningSeverity", Namespace = BingServicesNamespaces.RouteNamespace)]
	public enum ItineraryWarningSeverity : int {
		[EnumMember]
		None = 0,
		[EnumMember]
		LowImpact = 1,
		[EnumMember]
		Minor = 2,
		[EnumMember]
		Moderate = 3,
		[EnumMember]
		Serious = 4,
	}
	[DataContract(Name = "ItineraryWarningType", Namespace = BingServicesNamespaces.RouteNamespace)]
	public enum ItineraryWarningType : int {
		[EnumMember]
		Accident = 0,
		[EnumMember]
		AdminDivisionChange = 1,
		[EnumMember]
		BlockedRoad = 2,
		[EnumMember]
		CheckTimetable = 3,
		[EnumMember]
		Congestion = 4,
		[EnumMember]
		CountryChange = 5,
		[EnumMember]
		DisabledVehicle = 6,
		[EnumMember]
		GateAccess = 7,
		[EnumMember]
		GetOffTransit = 8,
		[EnumMember]
		GetOnTransit = 9,
		[EnumMember]
		IllegalUTurn = 10,
		[EnumMember]
		MassTransit = 11,
		[EnumMember]
		Miscellaneous = 12,
		[EnumMember]
		NoIncident = 13,
		[EnumMember]
		None = 14,
		[EnumMember]
		Other = 15,
		[EnumMember]
		OtherNews = 16,
		[EnumMember]
		OtherTrafficIncidents = 17,
		[EnumMember]
		PlannedEvent = 18,
		[EnumMember]
		PrivateRoad = 19,
		[EnumMember]
		RestrictedTurn = 20,
		[EnumMember]
		RoadClosures = 21,
		[EnumMember]
		RoadHazard = 22,
		[EnumMember]
		ScheduledConstruction = 23,
		[EnumMember]
		SeasonalClosures = 24,
		[EnumMember]
		Tollbooth = 25,
		[EnumMember]
		TollRoad = 26,
		[EnumMember]
		TrafficFlow = 27,
		[EnumMember]
		UnpavedRoad = 28,
		[EnumMember]
		UnscheduledConstruction = 29,
		[EnumMember]
		Weather = 30,
	}
	[DataContract(Name = "MajorRoutesRequest", Namespace = BingServicesNamespaces.RouteNamespace)]
	public class MajorRoutesRequest : RequestBase {
		Waypoint destination;
		MajorRoutesOptions options;
		[DataMember]
		public Waypoint Destination {
			get { return destination; }
			set {
				if((object.ReferenceEquals(destination, value) != true)) {
					destination = value;
					RaisePropertyChanged("Destination");
				}
			}
		}
		[DataMember]
		public MajorRoutesOptions Options {
			get { return this.options; }
			set {
				if((object.ReferenceEquals(options, value) != true)) {
					options = value;
					RaisePropertyChanged("Options");
				}
			}
		}
	}
	[DataContract(Name = "RouteRequest", Namespace = BingServicesNamespaces.RouteNamespace)]
	public class RouteRequest : RequestBase {
		RouteOptions options;
		Waypoint[] waypoints;
		[DataMember]
		public RouteOptions Options {
			get { return options; }
			set {
				if((object.ReferenceEquals(options, value) != true)) {
					options = value;
					RaisePropertyChanged("Options");
				}
			}
		}
		[DataMember]
		public Waypoint[] Waypoints {
			get { return waypoints; }
			set {
				if((object.ReferenceEquals(waypoints, value) != true)) {
					waypoints = value;
					RaisePropertyChanged("Waypoints");
				}
			}
		}
	}
	[DataContract(Name = "RouteOptions", Namespace = BingServicesNamespaces.RouteNamespace)]
	[KnownType(typeof(MajorRoutesOptions))]
	public class RouteOptions : INotifyPropertyChanged {
		TravelMode mode;
		RouteOptimization optimization;
		RoutePathType routePathType;
		TrafficUsage trafficUsage;
		[DataMember]
		public TravelMode Mode {
			get { return mode; }
			set {
				if((mode.Equals(value) != true)) {
					mode = value;
					RaisePropertyChanged("Mode");
				}
			}
		}
		[DataMember]
		public RouteOptimization Optimization {
			get { return optimization; }
			set {
				if((optimization.Equals(value) != true)) {
					optimization = value;
					RaisePropertyChanged("Optimization");
				}
			}
		}
		[DataMember]
		public RoutePathType RoutePathType {
			get { return routePathType; }
			set {
				if((routePathType.Equals(value) != true)) {
					routePathType = value;
					RaisePropertyChanged("RoutePathType");
				}
			}
		}
		[DataMember]
		public TrafficUsage TrafficUsage {
			get { return trafficUsage; }
			set {
				if((trafficUsage.Equals(value) != true)) {
					trafficUsage = value;
					RaisePropertyChanged("TrafficUsage");
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
	[DataContract(Name = "Waypoint", Namespace = BingServicesNamespaces.RouteNamespace)]
	public class Waypoint : INotifyPropertyChanged {
		string description;
		Location location;
		[DataMember]
		public string Description {
			get { return description; }
			set {
				if((object.ReferenceEquals(description, value) != true)) {
					description = value;
					RaisePropertyChanged("Description");
				}
			}
		}
		[DataMember]
		public Location Location {
			get { return location; }
			set {
				if((object.ReferenceEquals(location, value) != true)) {
					location = value;
					RaisePropertyChanged("Location");
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
	[DataContract(Name = "MajorRoutesOptions", Namespace = BingServicesNamespaces.RouteNamespace)]
	public class MajorRoutesOptions : RouteOptions {
		bool returnRoutes;
		[DataMember]
		public bool ReturnRoutes {
			get { return returnRoutes; }
			set {
				if((returnRoutes.Equals(value) != true)) {
					returnRoutes = value;
					RaisePropertyChanged("ReturnRoutes");
				}
			}
		}
	}
	[DataContract(Name = "MajorRoutesResponse", Namespace = BingServicesNamespaces.RouteNamespace)]
	public class MajorRoutesResponse : ResponseBase {
		RouteResult[] routes;
		Waypoint[] startingPoints;
		[DataMember]
		public RouteResult[] Routes {
			get { return routes; }
			set {
				if((object.ReferenceEquals(routes, value) != true)) {
					routes = value;
					RaisePropertyChanged("Routes");
				}
			}
		}
		[DataMember]
		public Waypoint[] StartingPoints {
			get { return startingPoints; }
			set {
				if((object.ReferenceEquals(startingPoints, value) != true)) {
					startingPoints = value;
					RaisePropertyChanged("StartingPoints");
				}
			}
		}
	}
	[DataContract(Name = "RouteResponse", Namespace = BingServicesNamespaces.RouteNamespace)]
	public class RouteResponse : ResponseBase {
		RouteResult result;
		[DataMember]
		public RouteResult Result {
			get { return result; }
			set {
				if((object.ReferenceEquals(result, value) != true)) {
					result = value;
					RaisePropertyChanged("Result");
				}
			}
		}
	}
	[DataContract(Name = "RouteResult", Namespace = BingServicesNamespaces.RouteNamespace)]
	public class RouteResult : INotifyPropertyChanged {
		RouteLeg[] legs;
		RoutePath routePath;
		RouteSummary summary;
		[DataMember]
		public RouteLeg[] Legs {
			get { return legs; }
			set {
				if((object.ReferenceEquals(legs, value) != true)) {
					legs = value;
					RaisePropertyChanged("Legs");
				}
			}
		}
		[DataMember]
		public RoutePath RoutePath {
			get { return routePath; }
			set {
				if((object.ReferenceEquals(routePath, value) != true)) {
					routePath = value;
					RaisePropertyChanged("RoutePath");
				}
			}
		}
		[DataMember]
		public RouteSummary Summary {
			get { return summary; }
			set {
				if((object.ReferenceEquals(summary, value) != true)) {
					summary = value;
					RaisePropertyChanged("Summary");
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
	[DataContract(Name = "RoutePath", Namespace = BingServicesNamespaces.RouteNamespace)]
	public class RoutePath : INotifyPropertyChanged {
		Location[] points;
		[DataMember]
		public Location[] Points {
			get { return points; }
			set {
				if((object.ReferenceEquals(points, value) != true)) {
					points = value;
					RaisePropertyChanged("Points");
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
	[DataContract(Name = "RouteSummary", Namespace = BingServicesNamespaces.RouteNamespace)]
	public class RouteSummary : INotifyPropertyChanged {
		Rectangle boundingRectangle;
		double distance;
		long timeInSeconds;
		[DataMember]
		public Rectangle BoundingRectangle {
			get { return boundingRectangle; }
			set {
				if((object.ReferenceEquals(boundingRectangle, value) != true)) {
					boundingRectangle = value;
					RaisePropertyChanged("BoundingRectangle");
				}
			}
		}
		[DataMember]
		public double Distance {
			get { return distance; }
			set {
				if((distance.Equals(value) != true)) {
					distance = value;
					RaisePropertyChanged("Distance");
				}
			}
		}
		[DataMember]
		public long TimeInSeconds {
			get { return this.timeInSeconds; }
			set {
				if((timeInSeconds.Equals(value) != true)) {
					timeInSeconds = value;
					RaisePropertyChanged("TimeInSeconds");
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
	[DataContract(Name = "RouteLeg", Namespace = BingServicesNamespaces.RouteNamespace)]
	public class RouteLeg : INotifyPropertyChanged {
		Location actualEnd;
		Location actualStart;
		ItineraryItem[] itinerary;
		RouteSummary summary;
		[DataMember]
		public Location ActualEnd {
			get { return actualEnd; }
			set {
				if((object.ReferenceEquals(actualEnd, value) != true)) {
					actualEnd = value;
					RaisePropertyChanged("ActualEnd");
				}
			}
		}
		[DataMember]
		public Location ActualStart {
			get { return actualStart; }
			set {
				if((object.ReferenceEquals(actualStart, value) != true)) {
					actualStart = value;
					RaisePropertyChanged("ActualStart");
				}
			}
		}
		[DataMember]
		public ItineraryItem[] Itinerary {
			get { return itinerary; }
			set {
				if((object.ReferenceEquals(itinerary, value) != true)) {
					itinerary = value;
					RaisePropertyChanged("Itinerary");
				}
			}
		}
		[DataMember]
		public RouteSummary Summary {
			get { return summary; }
			set {
				if((object.ReferenceEquals(summary, value) != true)) {
					summary = value;
					RaisePropertyChanged("Summary");
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
	[DataContract(Name = "ItineraryItem", Namespace = BingServicesNamespaces.RouteNamespace)]
	public class ItineraryItem : INotifyPropertyChanged {
		string compassDirection;
		ItineraryItemHint[] hints;
		Location location;
		ManeuverType maneuverType;
		RouteSummary summary;
		string text;
		ItineraryItemWarning[] warnings;
		[DataMember]
		public string CompassDirection {
			get { return compassDirection; }
			set {
				if((object.ReferenceEquals(compassDirection, value) != true)) {
					compassDirection = value;
					RaisePropertyChanged("CompassDirection");
				}
			}
		}
		[DataMember]
		public ItineraryItemHint[] Hints {
			get { return hints; }
			set {
				if((object.ReferenceEquals(hints, value) != true)) {
					hints = value;
					RaisePropertyChanged("Hints");
				}
			}
		}
		[DataMember]
		public Location Location {
			get { return location; }
			set {
				if((object.ReferenceEquals(location, value) != true)) {
					location = value;
					RaisePropertyChanged("Location");
				}
			}
		}
		[DataMember]
		public ManeuverType ManeuverType {
			get { return maneuverType; }
			set {
				if((maneuverType.Equals(value) != true)) {
					maneuverType = value;
					RaisePropertyChanged("ManeuverType");
				}
			}
		}
		[DataMember]
		public RouteSummary Summary {
			get { return summary; }
			set {
				if((object.ReferenceEquals(summary, value) != true)) {
					summary = value;
					RaisePropertyChanged("Summary");
				}
			}
		}
		[DataMember]
		public string Text {
			get { return text; }
			set {
				if((object.ReferenceEquals(text, value) != true)) {
					text = value;
					RaisePropertyChanged("Text");
				}
			}
		}
		[DataMember]
		public ItineraryItemWarning[] Warnings {
			get { return warnings; }
			set {
				if((object.ReferenceEquals(warnings, value) != true)) {
					warnings = value;
					RaisePropertyChanged("Warnings");
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
	[DataContract(Name = "ItineraryItemHint", Namespace = BingServicesNamespaces.RouteNamespace)]
	public class ItineraryItemHint : INotifyPropertyChanged {
		ItineraryItemHintType hintType;
		string text;
		[DataMember]
		public ItineraryItemHintType HintType {
			get { return hintType; }
			set {
				if((hintType.Equals(value) != true)) {
					hintType = value;
					RaisePropertyChanged("HintType");
				}
			}
		}
		[DataMember]
		public string Text {
			get { return text; }
			set {
				if((object.ReferenceEquals(text, value) != true)) {
					text = value;
					RaisePropertyChanged("Text");
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
	[DataContract(Name = "ItineraryItemWarning", Namespace = BingServicesNamespaces.RouteNamespace)]
	public class ItineraryItemWarning : INotifyPropertyChanged {
		ItineraryWarningSeverity severity;
		string text;
		ItineraryWarningType warningType;
		[DataMember]
		public ItineraryWarningSeverity Severity {
			get { return severity; }
			set {
				if((severity.Equals(value) != true)) {
					severity = value;
					RaisePropertyChanged("Severity");
				}
			}
		}
		[DataMember]
		public string Text {
			get { return text; }
			set {
				if((object.ReferenceEquals(text, value) != true)) {
					text = value;
					RaisePropertyChanged("Text");
				}
			}
		}
		[DataMember]
		public ItineraryWarningType WarningType {
			get { return warningType; }
			set {
				if((warningType.Equals(value) != true)) {
					warningType = value;
					RaisePropertyChanged("WarningType");
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
	[ServiceContract(Namespace = BingServicesNamespaces.RouteContractNamespace, ConfigurationName = "BingRoute.IRouteService")]
	public interface IRouteService {
		[OperationContractAttribute(AsyncPattern = true, Action = "http://dev.virtualearth.net/webservices/v1/route/contracts/IRouteService/CalculateRoute",
			ReplyAction = "http://dev.virtualearth.net/webservices/v1/route/contracts/IRouteService/CalculateRouteResponse")]
		[FaultContractAttribute(typeof(ResponseSummary), Action = "http://dev.virtualearth.net/webservices/v1/route/contracts/IRouteService/CalculateRouteResponseSummaryFault",
			Name = "ResponseSummary", Namespace = BingServicesNamespaces.CommonNamespace)]
		IAsyncResult BeginCalculateRoute(RouteRequest request, AsyncCallback callback, object asyncState);
		RouteResponse EndCalculateRoute(IAsyncResult result);
		[OperationContractAttribute(AsyncPattern = true, Action = "http://dev.virtualearth.net/webservices/v1/route/contracts/IRouteService/CalculateRoutesFromMajorRoads",
			ReplyAction = "http://dev.virtualearth.net/webservices/v1/route/contracts/IRouteService/CalculateRoutesFromMajorRoadsResponse")]
		[FaultContractAttribute(typeof(ResponseSummary), Action = "http://dev.virtualearth.net/webservices/v1/route/contracts/IRouteService/CalculateRoutesFromMajorRoadsResponseSummaryFault",
			Name = "ResponseSummary", Namespace = BingServicesNamespaces.CommonNamespace)]
		IAsyncResult BeginCalculateRoutesFromMajorRoads(MajorRoutesRequest request, AsyncCallback callback, object asyncState);
		MajorRoutesResponse EndCalculateRoutesFromMajorRoads(IAsyncResult result);
	}
	public class CalculateRouteCompletedEventArgs : AsyncCompletedEventArgs {
		object[] results;
		public CalculateRouteCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) :
			base(exception, cancelled, userState) {
			this.results = results;
		}
		public RouteResponse Result {
			get {
				base.RaiseExceptionIfNecessary();
				return (RouteResponse)results[0];
			}
		}
		public bool HasError {
			get {
				return Error != null;
			}
		}
	}
	public class CalculateRoutesFromMajorRoadsCompletedEventArgs : AsyncCompletedEventArgs {
		object[] results;
		public CalculateRoutesFromMajorRoadsCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) :
			base(exception, cancelled, userState) {
			this.results = results;
		}
		public MajorRoutesResponse Result {
			get {
				base.RaiseExceptionIfNecessary();
				return ((MajorRoutesResponse)(results[0]));
			}
		}
	}
	public class RouteServiceClient : ClientBase<IRouteService>, IRouteService {
		BeginOperationDelegate onBeginCalculateRouteDelegate;
		EndOperationDelegate onEndCalculateRouteDelegate;
		SendOrPostCallback onCalculateRouteCompletedDelegate;
		BeginOperationDelegate onBeginCalculateRoutesFromMajorRoadsDelegate;
		EndOperationDelegate onEndCalculateRoutesFromMajorRoadsDelegate;
		SendOrPostCallback onCalculateRoutesFromMajorRoadsCompletedDelegate;
		BeginOperationDelegate onBeginOpenDelegate;
		EndOperationDelegate onEndOpenDelegate;
		SendOrPostCallback onOpenCompletedDelegate;
		BeginOperationDelegate onBeginCloseDelegate;
		EndOperationDelegate onEndCloseDelegate;
		SendOrPostCallback onCloseCompletedDelegate;
		public RouteServiceClient() {
		}
		public RouteServiceClient(string endpointConfigurationName) :
			base(endpointConfigurationName) {
		}
		public RouteServiceClient(string endpointConfigurationName, string remoteAddress) :
			base(endpointConfigurationName, remoteAddress) {
		}
		public RouteServiceClient(string endpointConfigurationName, EndpointAddress remoteAddress) :
			base(endpointConfigurationName, remoteAddress) {
		}
		public RouteServiceClient(Binding binding, EndpointAddress remoteAddress) :
			base(binding, remoteAddress) {
		}
		public event EventHandler<CalculateRouteCompletedEventArgs> CalculateRouteCompleted;
		public event EventHandler<CalculateRoutesFromMajorRoadsCompletedEventArgs> CalculateRoutesFromMajorRoadsCompleted;
		public event EventHandler<AsyncCompletedEventArgs> OpenCompleted;
		public event EventHandler<AsyncCompletedEventArgs> CloseCompleted;
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		IAsyncResult IRouteService.BeginCalculateRoute(RouteRequest request, AsyncCallback callback, object asyncState) {
			IAsyncResult result;
			try {
				result = base.Channel.BeginCalculateRoute(request, callback, asyncState);
			}
			catch {
				result = null;
			}
			return result;
		}
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		RouteResponse IRouteService.EndCalculateRoute(IAsyncResult result) {
			return base.Channel.EndCalculateRoute(result);
		}
		IAsyncResult OnBeginCalculateRoute(object[] inValues, AsyncCallback callback, object asyncState) {
			RouteRequest request = (RouteRequest)inValues[0];
			return ((IRouteService)this).BeginCalculateRoute(request, callback, asyncState);
		}
		object[] OnEndCalculateRoute(IAsyncResult result) {
			RouteResponse retVal = ((IRouteService)this).EndCalculateRoute(result);
			return new object[] { retVal };
		}
		void OnCalculateRouteCompleted(object state) {
			if((this.CalculateRouteCompleted != null)) {
				InvokeAsyncCompletedEventArgs e = (InvokeAsyncCompletedEventArgs)state;
				this.CalculateRouteCompleted(this, new CalculateRouteCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
			}
		}
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		IAsyncResult IRouteService.BeginCalculateRoutesFromMajorRoads(MajorRoutesRequest request, AsyncCallback callback, object asyncState) {
			return base.Channel.BeginCalculateRoutesFromMajorRoads(request, callback, asyncState);
		}
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		MajorRoutesResponse IRouteService.EndCalculateRoutesFromMajorRoads(IAsyncResult result) {
			return base.Channel.EndCalculateRoutesFromMajorRoads(result);
		}
		IAsyncResult OnBeginCalculateRoutesFromMajorRoads(object[] inValues, AsyncCallback callback, object asyncState) {
			MajorRoutesRequest request = (MajorRoutesRequest)inValues[0];
			return ((IRouteService)this).BeginCalculateRoutesFromMajorRoads(request, callback, asyncState);
		}
		object[] OnEndCalculateRoutesFromMajorRoads(IAsyncResult result) {
			MajorRoutesResponse retVal = ((IRouteService)this).EndCalculateRoutesFromMajorRoads(result);
			return new object[] { retVal };
		}
		void OnCalculateRoutesFromMajorRoadsCompleted(object state) {
			if(this.CalculateRoutesFromMajorRoadsCompleted != null) {
				InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)state);
				this.CalculateRoutesFromMajorRoadsCompleted(this, new CalculateRoutesFromMajorRoadsCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
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
			if(this.OpenCompleted != null) {
				InvokeAsyncCompletedEventArgs e = (InvokeAsyncCompletedEventArgs)state;
				this.OpenCompleted(this, new AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
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
			if(this.CloseCompleted != null) {
				InvokeAsyncCompletedEventArgs e = (InvokeAsyncCompletedEventArgs)state;
				this.CloseCompleted(this, new AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
			}
		}
		public void CalculateRouteAsync(RouteRequest request) {
			this.CalculateRouteAsync(request, null);
		}
		public void CalculateRouteAsync(RouteRequest request, object userState) {
			if(this.onBeginCalculateRouteDelegate == null)
				this.onBeginCalculateRouteDelegate = new BeginOperationDelegate(this.OnBeginCalculateRoute);
			if(this.onEndCalculateRouteDelegate == null)
				this.onEndCalculateRouteDelegate = new EndOperationDelegate(this.OnEndCalculateRoute);
			if(this.onCalculateRouteCompletedDelegate == null)
				this.onCalculateRouteCompletedDelegate = new SendOrPostCallback(this.OnCalculateRouteCompleted);
			base.InvokeAsync(this.onBeginCalculateRouteDelegate, new object[] { request },
				this.onEndCalculateRouteDelegate, this.onCalculateRouteCompletedDelegate, userState);
		}
		public void CalculateRoutesFromMajorRoadsAsync(MajorRoutesRequest request) {
			this.CalculateRoutesFromMajorRoadsAsync(request, null);
		}
		public void CalculateRoutesFromMajorRoadsAsync(MajorRoutesRequest request, object userState) {
			if(this.onBeginCalculateRoutesFromMajorRoadsDelegate == null)
				this.onBeginCalculateRoutesFromMajorRoadsDelegate = new BeginOperationDelegate(this.OnBeginCalculateRoutesFromMajorRoads);
			if(this.onEndCalculateRoutesFromMajorRoadsDelegate == null)
				this.onEndCalculateRoutesFromMajorRoadsDelegate = new EndOperationDelegate(this.OnEndCalculateRoutesFromMajorRoads);
			if(this.onCalculateRoutesFromMajorRoadsCompletedDelegate == null)
				this.onCalculateRoutesFromMajorRoadsCompletedDelegate = new SendOrPostCallback(this.OnCalculateRoutesFromMajorRoadsCompleted);
			base.InvokeAsync(this.onBeginCalculateRoutesFromMajorRoadsDelegate, new object[] { request },
				this.onEndCalculateRoutesFromMajorRoadsDelegate, this.onCalculateRoutesFromMajorRoadsCompletedDelegate, userState);
		}
		public void OpenAsync() {
			this.OpenAsync(null);
		}
		public void OpenAsync(object userState) {
			if(this.onBeginOpenDelegate == null)
				this.onBeginOpenDelegate = new BeginOperationDelegate(this.OnBeginOpen);
			if(this.onEndOpenDelegate == null)
				this.onEndOpenDelegate = new EndOperationDelegate(this.OnEndOpen);
			if(this.onOpenCompletedDelegate == null)
				this.onOpenCompletedDelegate = new SendOrPostCallback(this.OnOpenCompleted);
			base.InvokeAsync(this.onBeginOpenDelegate, null, this.onEndOpenDelegate, this.onOpenCompletedDelegate, userState);
		}
		public void CloseAsync() {
			this.CloseAsync(null);
		}
		public void CloseAsync(object userState) {
			if(this.onBeginCloseDelegate == null)
				this.onBeginCloseDelegate = new BeginOperationDelegate(this.OnBeginClose);
			if(this.onEndCloseDelegate == null)
				this.onEndCloseDelegate = new EndOperationDelegate(this.OnEndClose);
			if(this.onCloseCompletedDelegate == null)
				this.onCloseCompletedDelegate = new SendOrPostCallback(this.OnCloseCompleted);
			base.InvokeAsync(this.onBeginCloseDelegate, null, this.onEndCloseDelegate, this.onCloseCompletedDelegate, userState);
		}
	}
}
