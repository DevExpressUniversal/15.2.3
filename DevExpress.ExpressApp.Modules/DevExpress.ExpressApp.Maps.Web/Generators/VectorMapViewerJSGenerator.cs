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
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Maps.Web.Helpers;
using System.Text;
namespace DevExpress.ExpressApp.Maps.Web {
	public class VectorMapViewerJSGenerator {
		public static readonly string paramsDelimiter = ", ";
		protected IObjectInfoHelper objectInfoHelper;
		protected JavaScriptSerializer serializer;
		protected VectorMapSettings vectorMapSettings;
		protected string localVarName;
		protected string variableName;
		protected StringBuilder scriptContainer;
		protected string PaletteToString(VectorMapPalette palette) {
			string paletteName = "";
			switch(palette) {
				case VectorMapPalette.Default:
					paletteName = "Default";					
					break;
				case VectorMapPalette.Bright:
					paletteName = "Bright";
					break;
				case VectorMapPalette.HarmonyLight:
					paletteName = "Harmony Light";
					break;
				case VectorMapPalette.Ocean:
					paletteName = "Ocean";
					break;
				case VectorMapPalette.Pastel:
					paletteName = "Pastel";
					break;
				case VectorMapPalette.Soft:
					paletteName = "Soft";
					break;
				case VectorMapPalette.SoftPastel:
					paletteName = "Soft Pastel";
					break;
				case VectorMapPalette.Vintage:
					paletteName = "Vintage";
					break;
				case VectorMapPalette.Violet:
					paletteName = "Violet";
					break;
				default:
					paletteName = "Default";
					break;
			}
			return paletteName;
		}
		private void AppendVariable(string name, string value) {
			if(!string.IsNullOrEmpty(value)) {
				scriptContainer.AppendLine(string.Format("{0}.{1} = {2};", localVarName, name, value));
			}
		}
		private void AppendQuotedSubVariable(string name, string value) {
			AppendSubVariable(name, "'" + value + "'");
		}
		private void AppendSubVariable(string name, string value) {
			if(!string.IsNullOrEmpty(value)) {
				scriptContainer.AppendLine(string.Format("{0}.{1}[\"{2}\"] = {3};", localVarName, variableName, name, value));
			}
		}
		protected string GenerateAreasTooltips() {
			var areasTooltips = new Dictionary<string, string>();
			foreach(IAreaInfo area in vectorMapSettings.Areas) {
				areasTooltips[area.Title] = area.Tooltip;
			}
			return serializer.Serialize(areasTooltips);
		}
		protected string GenerateAreasIDs() {
			var areasOids = new Dictionary<string, string>();
			foreach(IAreaInfo area in vectorMapSettings.Areas) {
				areasOids[area.Title] = objectInfoHelper.GetHandle(area);
			}
			return serializer.Serialize(areasOids);
		}
		protected string GenerateAreasValues() {
			var areasValues = new Dictionary<string, float>();
			foreach(IAreaInfo area in vectorMapSettings.Areas) {
				areasValues[area.Title] = area.Value;
			}
			return serializer.Serialize(areasValues);
		}
		protected string GenerateMarkersData() {
			List<JSVectorMapMarker> jsMarkersList = new List<JSVectorMapMarker>();
			if(vectorMapSettings.Markers != null && vectorMapSettings.Markers.Count > 0) {
				foreach(IVectorMapsMarker marker in vectorMapSettings.Markers) {
					JSVectorMapMarker jsMarkerData = new JSVectorMapMarker() {
						text = marker.Title,
						coordinates = new List<double> { marker.Longitude, marker.Latitude },
						value = marker.Value,
						attributes = new JSVectorMapMarkerAttributes { id = objectInfoHelper.GetHandle(marker), tooltip = marker.Tooltip }
					};
					jsMarkersList.Add(jsMarkerData);
				}
			}
			if(vectorMapSettings.PieMarkers != null && vectorMapSettings.PieMarkers.Count > 0) {
				foreach(IVectorMapsPieMarker pieMarker in vectorMapSettings.PieMarkers) {
					JSVectorMapMarker jsMarkerData = new JSVectorMapMarker() {
						text = pieMarker.Title,
						coordinates = new List<double> { pieMarker.Longitude, pieMarker.Latitude },
						attributes = new JSVectorMapMarkerAttributes { id = objectInfoHelper.GetHandle(pieMarker), tooltip = pieMarker.Tooltip }
					};
					jsMarkerData.values = pieMarker.Values.Select(item => item).ToList();
					jsMarkersList.Add(jsMarkerData);
				}
			}
			return serializer.Serialize(jsMarkersList);
		}
		protected string GenerateMarkerType() {
			VectorMapMarkerType markerType = VectorMapMarkerType.Bubble;
			if(vectorMapSettings.PieMarkers != null && vectorMapSettings.PieMarkers.Count > 0)
					markerType = VectorMapMarkerType.Pie;
			return markerType.ToString().ToLower();
		}
		protected string GenerateSettingsObject() {
			JSVectorMapSettings jsVectorMapSettings = new JSVectorMapSettings() {
				zoomFactor = vectorMapSettings.ZoomFactor,
				height = vectorMapSettings.Height,
				width = vectorMapSettings.Width,
			};
			bool isCoordinatsSetted = vectorMapSettings.Center.lng != 0 || vectorMapSettings.Center.lat != 0;
			if(isCoordinatsSetted) {
				jsVectorMapSettings.center = new List<double>() { vectorMapSettings.Center.lng, vectorMapSettings.Center.lat };
			}
			bool isBoundsSetted = vectorMapSettings.Bounds != null &&
								 (vectorMapSettings.Bounds.MaxLatitude != 0 ||
								  vectorMapSettings.Bounds.MinLatitude != 0 ||
								  vectorMapSettings.Bounds.MaxLongitude != 0 ||
								  vectorMapSettings.Bounds.MinLongitude != 0);
			if(isBoundsSetted) {
				jsVectorMapSettings.bounds = new List<double>() {
					vectorMapSettings.Bounds.MinLongitude,
					vectorMapSettings.Bounds.MaxLatitude,
					vectorMapSettings.Bounds.MaxLongitude,
					vectorMapSettings.Bounds.MinLatitude };
			}
			return serializer.Serialize(jsVectorMapSettings);
		}
		protected string GenerateLegendItems() {
			var legendItems = new Dictionary<string, float>();
			foreach(LegendItem legendItem in vectorMapSettings.Legend.Items) {
				legendItems[legendItem.Title] = legendItem.Value;
			}
			return serializer.Serialize(legendItems);
		}
		protected string GenerateLegendType() {
			string legendTypeString = "";
			switch(vectorMapSettings.Legend.Type) {
				case VectorMapLegendType.AreasColors:
					legendTypeString = "areaColorGroups";
					break;
				case VectorMapLegendType.MarkersSizes:
					legendTypeString = "markerSizeGroups";
					break;
				case VectorMapLegendType.MarkersColors:
					legendTypeString = "markerColorGroups";
					break;
				default:
					legendTypeString = "areaColorGroups";
					break;
			}
			return legendTypeString;
		}
		protected string GenerateMapResourceSettings() {
			return "DevExpressMap_" + vectorMapSettings.Type.ToString().ToLower();
		}
		public VectorMapViewerJSGenerator(string localVarName, VectorMapSettings vectorMapSettings, IObjectInfoHelper objectInfoHelper) {
			this.localVarName = localVarName;
			this.vectorMapSettings = vectorMapSettings;
			this.objectInfoHelper = objectInfoHelper;
			serializer = new JavaScriptSerializer();
			serializer.RegisterConverters(new JavaScriptConverter[] { new NullValuesConverter() });
		}
		public string GenerateScript() {
			scriptContainer = new StringBuilder();
			string optionsVarName = "vectorMapSettings";
			AppendVariable(optionsVarName, GenerateSettingsObject());
			variableName = optionsVarName;
			AppendQuotedSubVariable("palette", PaletteToString(vectorMapSettings.Palette));
			AppendSubVariable("defaultAreaValue", vectorMapSettings.DefaultAreaValue.ToString());
			AppendQuotedSubVariable("backgroundColor", ColorTranslator.ToHtml(vectorMapSettings.BackgroundColor));
			AppendSubVariable("areasTitlesEnabled", vectorMapSettings.IsAreasTitlesEnabled.ToString().ToLower());
			AppendSubVariable("markersTitlesEnabled", vectorMapSettings.IsMarkersTitlesEnabled.ToString().ToLower());
			AppendSubVariable("controlsEnabled", vectorMapSettings.IsControlsEnabled.ToString().ToLower());
			AppendSubVariable("areasIds", GenerateAreasIDs());
			AppendSubVariable("areasValues", GenerateAreasValues());
			AppendSubVariable("areasTooltips", GenerateAreasTooltips());
			AppendSubVariable("controlsEnabled", vectorMapSettings.IsControlsEnabled.ToString().ToLower());
			AppendSubVariable("markers", GenerateMarkersData());
			AppendQuotedSubVariable("markersType", GenerateMarkerType());
			AppendSubVariable("markerMinSize", vectorMapSettings.BubbleMarkerMinSize.ToString());
			AppendSubVariable("markerMaxSize", vectorMapSettings.BubbleMarkerMaxSize.ToString());
			AppendSubVariable("pieMarkerSize", vectorMapSettings.PieMarkerSize.ToString());
			AppendSubVariable("legendEnabled", vectorMapSettings.Legend.Enabled.ToString().ToLower());
			AppendQuotedSubVariable("legendType", GenerateLegendType());
			AppendSubVariable("legendItems", GenerateLegendItems());
			AppendSubVariable("mapResource", GenerateMapResourceSettings());
			return scriptContainer.ToString();
		}
	}
}
