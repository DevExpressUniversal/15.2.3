#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Text;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Maps.Web.Helpers;
namespace DevExpress.ExpressApp.Maps.Web {
	public class MapViewerJSGenerator {
		protected IObjectInfoHelper objectInfoHelper;
		protected JavaScriptSerializer serializer;
		protected MapSettings mapSettings;
		protected string localVarName;
		protected StringBuilder scriptContainer;
		private void AppendVariable(string name, string value) {
			if(!string.IsNullOrEmpty(value)) {
				scriptContainer.AppendLine(string.Format("{0}.{1} = {2};", localVarName, name, value));
			}
		}
		protected string GenerateMapOptions() {
			JSMapSettings jsMapSettings = new JSMapSettings() {
				provider = mapSettings.Provider.ToString().ToLower(),
				type = mapSettings.Type.ToString().ToLower(),
				zoom = mapSettings.ZoomLevel,
				height = mapSettings.Height,
				width = mapSettings.Width,
				controls = mapSettings.IsControlsEnabled,
				autoAdjust = true
			};
			bool isCoordinatsSetted = mapSettings.Center.lng != 0 || mapSettings.Center.lat != 0;
			if(isCoordinatsSetted) {
				jsMapSettings.autoAdjust = false;
				jsMapSettings.center = mapSettings.Center;
			}
			string options = serializer.Serialize(jsMapSettings);
			return options;
		}
		protected string GenerateApiKeysString() {
			bool isGoogleApiKeyIsSet = !string.IsNullOrEmpty(mapSettings.GoogleApiKey);
			bool isGoogleStaticApiKeyIsSet = !string.IsNullOrEmpty(mapSettings.GoogleStaticApiKey);
			bool isBingApiKeyIsSet = !string.IsNullOrEmpty(mapSettings.BingApiKey);
			string apiKeysString = "";
			if(isGoogleApiKeyIsSet || isGoogleStaticApiKeyIsSet || isBingApiKeyIsSet) {
				JSApiKeys apiKeys = new JSApiKeys();
				apiKeys.google = isGoogleApiKeyIsSet ? mapSettings.GoogleApiKey : null;
				apiKeys.googleStatic = isGoogleStaticApiKeyIsSet ? mapSettings.GoogleStaticApiKey : null;
				apiKeys.bing = isBingApiKeyIsSet ? mapSettings.BingApiKey : null;
				apiKeysString = serializer.Serialize(apiKeys);
			}
			return apiKeysString;
		}
		protected string GenerateMarkersDataString() {
			List<JSMarkerData> jsMarkersList = new List<JSMarkerData>();
			if(mapSettings.Markers != null && mapSettings.Markers.Count() > 0) {
				foreach(IMapsMarker marker in mapSettings.Markers) {
					JSMarkerData jsMarkerData = new JSMarkerData() {
						title = marker.Title,
						latitude = marker.Latitude,
						longitude = marker.Longitude,
						id = objectInfoHelper.GetHandle(marker)
					};
					jsMarkersList.Add(jsMarkerData);
				}
			}
			string markers = serializer.Serialize(jsMarkersList);
			return markers;
		}
		protected string GenerateRoutesString() {
			string routesString = "";
			if(mapSettings.isRouteEnabled) {
				RouteSettings routeSettings = mapSettings.RouteSettings;
				JSRouteSettings jsRouteSettings = new JSRouteSettings() {
					weight = routeSettings.Weight,
					color = ColorTranslator.ToHtml(routeSettings.Color),
					opacity = routeSettings.Opacity,
					mode = routeSettings.Mode.ToString().ToLower(),
					locations = new List<MapPoint>()
				};
				if(mapSettings.Markers != null && mapSettings.Markers.Count() > 0) {
					foreach(IMapsMarker marker in mapSettings.Markers) {
						jsRouteSettings.locations.Add(new MapPoint(marker.Latitude, marker.Longitude));
					}
				}
				routesString = serializer.Serialize(jsRouteSettings);
			}
			return routesString;
		}
		protected string GenerateLocalizationStrings() {
			JSLocalizedStrings jsLocalizedStrings = new JSLocalizedStrings() {
				showDetails = mapSettings.LocalizedShowDetailsString
			};
			string localizedStrings = serializer.Serialize(jsLocalizedStrings);
			return localizedStrings;
		}
		public MapViewerJSGenerator(string localVarName, MapSettings mapSettings, IObjectInfoHelper objectInfoHelper) {
			this.localVarName = localVarName;
			this.mapSettings = mapSettings;
			this.objectInfoHelper = objectInfoHelper;
			serializer = new JavaScriptSerializer();
			serializer.RegisterConverters(new JavaScriptConverter[] { new NullValuesConverter() });
		}
		public string GenerateScript() {
			scriptContainer = new StringBuilder();
			AppendVariable("mapOptions", GenerateMapOptions());
			AppendVariable("apiKeys", GenerateApiKeysString());
			AppendVariable("markers", GenerateMarkersDataString());
			AppendVariable("route", GenerateRoutesString());
			AppendVariable("localizedStrings", GenerateLocalizationStrings());
			AppendVariable("showDetails", mapSettings.IsShowDetailsEnabled.ToString().ToLower());
			AppendVariable("enableMarkersTooltips", mapSettings.IsMarkersTooltipsEnabled.ToString().ToLower());
			return scriptContainer.ToString();
		}
	}
}
