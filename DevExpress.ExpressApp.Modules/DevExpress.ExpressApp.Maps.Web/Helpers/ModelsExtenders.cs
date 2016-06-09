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
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.DC;
namespace DevExpress.ExpressApp.Maps.Web.Helpers {
	public interface IModelMaps : IModelNode {
		IModelMapSettings MapSettings { get; }
		IModelVectorMapSettings VectorMapSettings { get; }
	}
	public interface IModelMapSettings : IModelNode {
		[Category("Geometry")]
		[DefaultValue(MapsAspNetModule.defaultMapWidth)]
		int Width { get; set; }
		[Category("Geometry")]
		[DefaultValue(MapsAspNetModule.defaultMapHeight)]
		int Height { get; set; }
		[Category("Appearance")]
		[DefaultValue(MapsAspNetModule.defaultMapControlsEnabled)]
		bool ControlsEnabled { get; set; }
		[Category("Appearance")]
		[DefaultValue(MapsAspNetModule.defaultMapMarkersTooltipsEnabled)]
		bool MarkersTooltipsEnabled { get; set; }
		[Category("Appearance")]
		[DefaultValue(MapsAspNetModule.defaultMapProvider)]
		MapProvider Provider { get; set; }
		[Category("Appearance")]
		[DefaultValue(MapsAspNetModule.defaultMapType)]
		MapType Type { get; set; }
		[Category("Geometry")]
		[DefaultValue(MapsAspNetModule.defaultMapZoomLevel)]
		int ZoomLevel { get; set; }
		[Category("Geometry")]
		[DefaultValue(MapsAspNetModule.defaultMapCenterLatitude)]
		double CenterLatitude { get; set; }
		[Category("Geometry")]
		[DefaultValue(MapsAspNetModule.defaultMapCenterLongitude)]
		double CenterLongitude { get; set; }
		[Category("Routes")]
		Color RouteColor { get; set; }
		[Category("Routes")]
		[DefaultValue(MapsAspNetModule.defaultRouteWeight)]
		int RouteWeight { get; set; }
		[Category("Routes")]
		[DefaultValue(MapsAspNetModule.defaultRouteMode)]
		RouteMode RouteMode { get; set; }
		[Category("Routes")]
		[DefaultValue(MapsAspNetModule.defaultRouteOpacity)]
		double RouteOpacity { get; set; }
	}
	public interface IModelVectorMapSettings : IModelNode {
		[Category("Geometry")]
		[DefaultValue(MapsAspNetModule.defaultVectorMapWidth)]
		int Width { get; set; }
		[Category("Geometry")]
		[DefaultValue(MapsAspNetModule.defaultVectorMapHeight)]
		int Height { get; set; }
		[Category("Geometry")]
		[DefaultValue(MapsAspNetModule.defaultVectorMapCenterLatitude)]
		double CenterLatitude { get; set; }
		[Category("Geometry")]
		[DefaultValue(MapsAspNetModule.defaultVectorMapCenterLongitude)]
		double CenterLongitude { get; set; }
		[Category("Appearance")]
		[DefaultValue(MapsAspNetModule.defaultVectorMapType)]
		VectorMapType Type { get; set; }
		[Category("Appearance")]
		[DefaultValue(MapsAspNetModule.defaultVectorMapControlsEnabled)]
		bool ControlsEnabled { get; set; }
		[Category("Appearance")]
		[DefaultValue(MapsAspNetModule.defaultVectorMapAreasTitlesEnabled)]
		bool AreasTitlesEnabled { get; set; }
		[Category("Appearance")]
		[DefaultValue(MapsAspNetModule.defaultVectorMapMarkersTitlesEnabled)]
		bool MarkersTitlesEnabled { get; set; }
		[Category("Geometry")]
		[DefaultValue(MapsAspNetModule.defaultVectorMapZoomFactor)]
		double ZoomFactor { get; set; }
		[Category("Bounds")]
		[DefaultValue(MapsAspNetModule.defaultVectorMapMinLongitude)]
		double MinLongitude { get; set; }
		[Category("Bounds")]
		[DefaultValue(MapsAspNetModule.defaultVectorMapMaxLatitude)]
		double MaxLatitude { get; set; }
		[Category("Bounds")]
		[DefaultValue(MapsAspNetModule.defaultVectorMapMaxLongitude)]
		double MaxLongitude { get; set; }
		[Category("Bounds")]
		[DefaultValue(MapsAspNetModule.defaultVectorMapMinLatitude)]
		double MinLatitude { get; set; }
		[Category("Bubble markers")]
		[DefaultValue(MapsAspNetModule.defaultVectorMapBubbleMarkerMinSize)]
		int BubbleMarkerMinSize { get; set; }
		[Category("Bubble markers")]
		[DefaultValue(MapsAspNetModule.defaultVectorMapBubbleMarkerMaxSize)]
		int BubbleMarkerMaxSize { get; set; }
		[Category("Pie markers")]
		[DefaultValue(MapsAspNetModule.defaultVectorMapPieMarkerSize)]
		int PieMarkerSize { get; set; }
		[Category("Areas")]
		[DefaultValue(MapsAspNetModule.defaultVectorMapAreaValue)]
		float DefaultAreaValue { get; set; }
		[Category("Appearance")]
		[DefaultValue(MapsAspNetModule.defaultVectorMapPalette)]
		VectorMapPalette Palette { get; set; }
		[Category("Appearance")]
		Color BackgroundColor { get; set; }
		[Category("Legend")]
		IModelIntervalItems IntervalItems { get; }
		[Category("Legend")]
		[DefaultValue(MapsAspNetModule.defaultVectorMapLegendEnabled)]
		bool LegendEnabled { get; set; }
		[Category("Legend")]
		[DefaultValue(MapsAspNetModule.defaultVectorMapLegendType)]
		VectorMapLegendType LegendType { get; set; }
	}
	public interface IModelIntervalItems : IModelNode, IModelList<IModelIntervalItem> {
	}
	public interface IModelIntervalItem : IModelNode {
		float Value { get; set; }
		string Title { get; set; }
	}
	[DomainLogic(typeof(IModelVectorMapSettings))]
	public class ModelVectorMapSettingsLogic {
		public static void BeforeSet_Type(IModelVectorMapSettings settings, object value) {
			VectorMapType type = (VectorMapType)value;
			switch(type) {
				case VectorMapType.World:
					settings.MinLatitude = 0;
					settings.MinLongitude = 0;
					settings.MaxLatitude = 0;
					settings.MaxLongitude = 0;
					settings.ZoomFactor = 1.25;
					settings.CenterLatitude = 0;
					settings.CenterLongitude = 0;
					break;
				case VectorMapType.USA:
					settings.MinLongitude = -133.5;
					settings.MinLatitude = 30;
					settings.MaxLongitude = -83.5;
					settings.MaxLatitude = 70;
					settings.ZoomFactor = 1.4;
					settings.CenterLatitude = 40;
					settings.CenterLongitude = -95;
					break;
				case VectorMapType.Canada:
					settings.MinLongitude = -133.5;
					settings.MinLatitude = 10;
					settings.MaxLongitude = -83.5;
					settings.MaxLatitude = 89;
					settings.ZoomFactor = 1.0;
					settings.CenterLatitude = 45;
					settings.CenterLongitude = -95;
					break;
				case VectorMapType.Europe:
					settings.MinLongitude = -25.9;
					settings.MinLatitude = 36.6;
					settings.MaxLongitude = 49.6;
					settings.MaxLatitude = 81.5;
					settings.ZoomFactor = 1;
					settings.CenterLatitude = 55.5;
					settings.CenterLongitude = 16;
					break;
				case VectorMapType.Eurasia:
					settings.MinLongitude = -25.9;
					settings.MinLatitude = 6.2;
					settings.MaxLongitude = 179.0;
					settings.MaxLatitude = 77.5;
					settings.ZoomFactor = 1.3;
					settings.CenterLatitude = 55.5;
					settings.CenterLongitude = 86;
					break;
				case VectorMapType.Africa:
					settings.MinLongitude = -20.7;
					settings.MinLatitude = -38.4;
					settings.MaxLongitude = 53.2;
					settings.MaxLatitude = 39.6;
					settings.ZoomFactor = 1.0;
					settings.CenterLatitude = 0;
					settings.CenterLongitude = 30;
					break;
			}
		}
	}
}
