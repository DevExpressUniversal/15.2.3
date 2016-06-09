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
using System;
using System.Windows;
namespace DevExpress.Xpf.Map.Native {
	public static class BingServicesUtils {
		public static RequestResultCode GetRequestResultCode(ResponseStatusCode statusCode) {
			return (RequestResultCode)(Enum.Parse(typeof(RequestResultCode), statusCode.ToString(), true));
		}
		public static GeoPoint ConvertLocationToGeoPoint(Location location) {
			return new GeoPoint(location.Latitude, location.Longitude);
		}
		public static MapShape ConvertBingShapeToMapShape(ShapeBase bingShape, LayerBase layer) {
			if(bingShape is Circle) {
				Circle bingCircle = bingShape as Circle;
				MapEllipse mapEllipse = new MapEllipse();
				GeoPoint anchorPoint = new GeoPoint(bingCircle.Center.Latitude, bingCircle.Center.Longitude);
				Size ellipseSize = layer.ActualProjection.KilometersToGeoSize(anchorPoint, new Size(bingCircle.Radius, bingCircle.Radius));
				anchorPoint = new GeoPoint(anchorPoint.Latitude + ellipseSize.Height, anchorPoint.Longitude - ellipseSize.Width);
				mapEllipse.Location = anchorPoint;
				mapEllipse.Width = bingCircle.Radius * 2;
				mapEllipse.Height = bingCircle.Radius * 2;
				return mapEllipse;
			}
			if(bingShape is Rectangle) {
				Rectangle bingRectangle = bingShape as Rectangle;
				MapPolygon mapPolygon = new MapPolygon();
				mapPolygon.Points.Add(BingServicesUtils.ConvertLocationToGeoPoint(bingRectangle.Northeast));
				mapPolygon.Points.Add(new GeoPoint(bingRectangle.Southwest.Latitude, bingRectangle.Northeast.Longitude));
				mapPolygon.Points.Add(BingServicesUtils.ConvertLocationToGeoPoint(bingRectangle.Southwest));
				mapPolygon.Points.Add(new GeoPoint(bingRectangle.Northeast.Latitude, bingRectangle.Southwest.Longitude));
				return mapPolygon;
			}
			if(bingShape is Polygon) {
				Polygon bingPolygon = bingShape as Polygon;
				MapPolygon mapPolygon = new MapPolygon();
				foreach(Location vertice in bingPolygon.Vertices)
					mapPolygon.Points.Add(BingServicesUtils.ConvertLocationToGeoPoint(vertice));
			}
			return null;
		}
		public static DistanceUnit ConvertDistanceMeasureUnitToDistanceUnit(DistanceMeasureUnit distanceMeasureUnit) {
			return (DistanceUnit)(Enum.Parse(typeof(DistanceUnit), distanceMeasureUnit.ToString(), true));
		}
		public static BingManeuverType ConvertManeuverTypeToBingManeuverType(ManeuverType maneuver) {
			return (BingManeuverType)(Enum.Parse(typeof(BingManeuverType), maneuver.ToString(), true));
		}
		public static BingItineraryWarningType ConvertItineraryWarningTypeToBingItineraryWarningType(ItineraryWarningType warning) {
			return (BingItineraryWarningType)(Enum.Parse(typeof(BingItineraryWarningType), warning.ToString(), true));
		}
		public static AddressBase ConvertToBingAddress(Address address) {
			return address != null ? new BingAddress() {
				AddressLine = address.AddressLine,
				AdminDistrict = address.AdminDistrict,
				CountryRegion = address.CountryRegion,
				FormattedAddress = address.FormattedAddress,
				PostalCode = address.PostalCode,
				Locality = address.Locality,
				District = address.District,
				PostalTown = address.PostalTown
			} : null;
		}
	}
}
