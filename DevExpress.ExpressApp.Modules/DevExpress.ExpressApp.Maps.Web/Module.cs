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
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using DevExpress.Utils;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Maps.Web.Helpers;
[assembly: WebResource(DevExpress.ExpressApp.Maps.Web.MapsAspNetModule.MapScriptResourceName, "text/javascript")]
[assembly: WebResource(DevExpress.ExpressApp.Maps.Web.MapsAspNetModule.VectorMapScriptResourceName, "text/javascript")]
[assembly: WebResource(DevExpress.ExpressApp.Maps.Web.MapsAspNetModule.WorldMapScriptResourceName, "text/javascript")]
[assembly: WebResource(DevExpress.ExpressApp.Maps.Web.MapsAspNetModule.UsaMapScriptResourceName, "text/javascript")]
[assembly: WebResource(DevExpress.ExpressApp.Maps.Web.MapsAspNetModule.CanadaMapScriptResourceName, "text/javascript")]
[assembly: WebResource(DevExpress.ExpressApp.Maps.Web.MapsAspNetModule.EuropeMapScriptResourceName, "text/javascript")]
[assembly: WebResource(DevExpress.ExpressApp.Maps.Web.MapsAspNetModule.EurasiaMapScriptResourceName, "text/javascript")]
[assembly: WebResource(DevExpress.ExpressApp.Maps.Web.MapsAspNetModule.AfricaMapScriptResourceName, "text/javascript")]
namespace DevExpress.ExpressApp.Maps.Web {
	[DXToolboxItem(true)]
	[ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
	[Description("Provides Maps functionality in XAF applications.")]
	[ToolboxBitmap(typeof(MapsAspNetModule), "Resources.Toolbox_Module_Maps_Web.ico")]
	[ToolboxItemFilter("Xaf.Platform.Web")]
	public sealed class MapsAspNetModule : ModuleBase {
		internal const string VectorMapScriptResourceName = "DevExpress.ExpressApp.Maps.Web.Scripts.vectorMaps.js";
		internal const string MapScriptResourceName = "DevExpress.ExpressApp.Maps.Web.Scripts.maps.js";
		internal const string WorldMapScriptResourceName = "DevExpress.ExpressApp.Maps.Web.Scripts.world.js";
		internal const string UsaMapScriptResourceName = "DevExpress.ExpressApp.Maps.Web.Scripts.usa.js";
		internal const string CanadaMapScriptResourceName = "DevExpress.ExpressApp.Maps.Web.Scripts.canada.js";
		internal const string EuropeMapScriptResourceName = "DevExpress.ExpressApp.Maps.Web.Scripts.europe.js";
		internal const string EurasiaMapScriptResourceName = "DevExpress.ExpressApp.Maps.Web.Scripts.eurasia.js";
		internal const string AfricaMapScriptResourceName = "DevExpress.ExpressApp.Maps.Web.Scripts.africa.js";
		internal const int defaultMapWidth = 0;
		internal const int defaultMapHeight = 0;
		internal const bool defaultMapControlsEnabled = true;
		internal const bool defaultMapMarkersTooltipsEnabled = true;
		internal const MapProvider defaultMapProvider = MapProvider.Google;
		internal const MapType defaultMapType = MapType.RoadMap;
		internal const int defaultMapZoomLevel = 10;
		internal const double defaultMapCenterLatitude = 0.0;
		internal const double defaultMapCenterLongitude = 0.0;
		internal const int defaultRouteWeight = 6;
		internal const RouteMode defaultRouteMode = RouteMode.Driving;
		internal const double defaultRouteOpacity = 0.5;
		internal const VectorMapType defaultVectorMapType = VectorMapType.World;
		internal const int defaultVectorMapWidth = 0;
		internal const int defaultVectorMapHeight = 0;
		internal const int defaultVectorMapBubbleMarkerMinSize = 15;
		internal const int defaultVectorMapBubbleMarkerMaxSize = 40;
		internal const int defaultVectorMapPieMarkerSize = 30;
		internal const bool defaultVectorMapControlsEnabled = true;
		internal const bool defaultVectorMapLegendEnabled = true;
		internal const bool defaultVectorMapAreasTitlesEnabled = false;
		internal const bool defaultVectorMapMarkersTitlesEnabled = true;
		internal const VectorMapLegendType defaultVectorMapLegendType = VectorMapLegendType.AreasColors;
		internal const double defaultVectorMapZoomFactor = 1.25;
		internal const double defaultVectorMapCenterLatitude = 0;
		internal const double defaultVectorMapCenterLongitude = 0;
		internal const double defaultVectorMapMinLongitude = 0;
		internal const double defaultVectorMapMinLatitude = 0;
		internal const double defaultVectorMapMaxLongitude = 0;
		internal const double defaultVectorMapMaxLatitude = 0;
		internal const float defaultVectorMapAreaValue = 0;
		internal const VectorMapPalette defaultVectorMapPalette = VectorMapPalette.Default;
		private ClientLibrariesLocations clientLibrariesLocation = ClientLibrariesLocations.Embedded;
		private string googleApiKey = "";
		private string googleStaticApiKey = "";
		private string bingApiKey = "";
#if !SL
	[DevExpressExpressAppMapsWebLocalizedDescription("MapsAspNetModuleClientLibrariesLocation")]
#endif
		[DefaultValue(ClientLibrariesLocations.Embedded)]
		public ClientLibrariesLocations ClientLibrariesLocation {
			get { return clientLibrariesLocation; }
			set { clientLibrariesLocation = value; }
		}
#if !SL
	[DevExpressExpressAppMapsWebLocalizedDescription("MapsAspNetModuleGoogleApiKey")]
#endif
		[DefaultValue("")]
		public string GoogleApiKey {
			get { return googleApiKey; }
			set { googleApiKey = value; }
		}
#if !SL
	[DevExpressExpressAppMapsWebLocalizedDescription("MapsAspNetModuleGoogleStaticApiKey")]
#endif
		[DefaultValue("")]
		public string GoogleStaticApiKey {
			get { return googleStaticApiKey; }
			set { googleStaticApiKey = value; }
		}
#if !SL
	[DevExpressExpressAppMapsWebLocalizedDescription("MapsAspNetModuleBingApiKey")]
#endif
		[DefaultValue("")]
		public string BingApiKey {
			get { return bingApiKey; }
			set { bingApiKey = value; }
		}
		protected override IEnumerable<Type> GetRegularTypes() {
			return new Type[]{
				typeof(IModelMaps),
				typeof(IModelMapSettings),
				typeof(IModelVectorMapSettings),
				typeof(IModelIntervalItems),
				typeof(IModelIntervalItem),
				typeof(ModelVectorMapSettingsLogic)
			};
		}
		protected override IEnumerable<Type> GetDeclaredControllerTypes() {
			return new Type[] {};
		}
		protected override IEnumerable<Type> GetDeclaredExportedTypes() {
			return Type.EmptyTypes;
		}
		protected override void RegisterEditorDescriptors(List<EditorDescriptor> editorDescriptors) {
			string mapsPropertyEditorAliasName = "MapsProperyEditor";
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration(mapsPropertyEditorAliasName, typeof(IMapsMarker), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(mapsPropertyEditorAliasName, typeof(IMapsMarker), typeof(WebMapsPropertyEditor), true)));
			string mapsListEditorAliasName = "MapsListEditor";
			editorDescriptors.Add(new ListEditorDescriptor(new AliasRegistration(mapsListEditorAliasName, typeof(IMapsMarker), true)));
			editorDescriptors.Add(new ListEditorDescriptor(new EditorTypeRegistration(mapsListEditorAliasName, typeof(IMapsMarker), typeof(WebMapsListEditor), true)));
			string vectorMapsListEditorAliasName = "VectorMapsListEditor";
			editorDescriptors.Add(new ListEditorDescriptor(new AliasRegistration(vectorMapsListEditorAliasName, typeof(IAreaInfo), true)));
			editorDescriptors.Add(new ListEditorDescriptor(new EditorTypeRegistration(vectorMapsListEditorAliasName, typeof(IAreaInfo), typeof(WebVectorMapsListEditor), true)));
			editorDescriptors.Add(new ListEditorDescriptor(new AliasRegistration(vectorMapsListEditorAliasName, typeof(IVectorMapsMarker), true)));
			editorDescriptors.Add(new ListEditorDescriptor(new EditorTypeRegistration(vectorMapsListEditorAliasName, typeof(IVectorMapsMarker), typeof(WebVectorMapsListEditor), true)));
			editorDescriptors.Add(new ListEditorDescriptor(new AliasRegistration(vectorMapsListEditorAliasName, typeof(IVectorMapsPieMarker), true)));
			editorDescriptors.Add(new ListEditorDescriptor(new EditorTypeRegistration(vectorMapsListEditorAliasName, typeof(IVectorMapsPieMarker), typeof(WebVectorMapsListEditor), true)));
		}
		public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
			base.ExtendModelInterfaces(extenders);
			extenders.Add<IModelListView, IModelMaps>();
			extenders.Add<IModelPropertyEditor, IModelMaps>();
			extenders.Add<IModelColumn, IModelMaps>();
		}
		public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
			base.AddGeneratorUpdaters(updaters);
			updaters.Add(new ListViewDataAccessModeGeneratorUpdater(typeof(WebMapsListEditor)));
			updaters.Add(new ListViewDataAccessModeGeneratorUpdater(typeof(WebMapsPropertyEditor)));
			updaters.Add(new ListViewDataAccessModeGeneratorUpdater(typeof(WebVectorMapsListEditor)));
		}
		private static MapsAspNetModule FindInstance(XafApplication application) {
			MapsAspNetModule module = null;
			if(application != null) {
				module = (MapsAspNetModule)application.Modules.FindModule(typeof(MapsAspNetModule));
			}
			return module;
		}
		public static void SetEmbeddingClientLibraries(XafApplication application) {
			bool embedRequiredClientLibraries = true;
			MapsAspNetModule module = FindInstance(application);
			if(module != null)
				embedRequiredClientLibraries = module.ClientLibrariesLocation == ClientLibrariesLocations.Embedded;
			DevExpress.Web.ASPxWebControl.GlobalEmbedRequiredClientLibraries = embedRequiredClientLibraries;
		}
		public static MapSettings MapSettingsFromModel(IModelMaps model, XafApplication application) {
			IModelMapSettings mapsModelSettings = model.MapSettings;
			MapSettings mapSettings = new MapSettings();
			mapSettings.Provider = mapsModelSettings.Provider;
			mapSettings.Height = mapsModelSettings.Height;
			mapSettings.Width = mapsModelSettings.Width;
			mapSettings.IsMarkersTooltipsEnabled = mapsModelSettings.MarkersTooltipsEnabled;
			mapSettings.Type = mapsModelSettings.Type;
			mapSettings.ZoomLevel = mapsModelSettings.ZoomLevel;
			mapSettings.Center = new MapPoint(mapsModelSettings.CenterLatitude, mapsModelSettings.CenterLongitude);
			mapSettings.IsControlsEnabled = mapsModelSettings.ControlsEnabled;
			RouteSettings routeSettings = new RouteSettings();
			routeSettings.Enabled = false;
			routeSettings.Color = mapsModelSettings.RouteColor;
			routeSettings.Mode = mapsModelSettings.RouteMode;
			routeSettings.Weight = mapsModelSettings.RouteWeight;
			routeSettings.Opacity = mapsModelSettings.RouteOpacity;
			mapSettings.RouteSettings = routeSettings;
			mapSettings.LocalizedShowDetailsString = CaptionHelper.GetLocalizedText("Messages", "ShowDetailsMapMessage");
			MapsAspNetModule module = FindInstance(application);
			if(module != null) {
				mapSettings.GoogleApiKey = module.GoogleApiKey;
				mapSettings.GoogleStaticApiKey = module.GoogleStaticApiKey;
				mapSettings.BingApiKey = module.BingApiKey;
			}
			return mapSettings;
		}
		public static VectorMapSettings VectorMapSettingsFromModel(IModelMaps model) {
			IModelVectorMapSettings vectorMapsModelSettings = model.VectorMapSettings;
			VectorMapSettings vectorMapSettings = new VectorMapSettings();
			vectorMapSettings.IsControlsEnabled = vectorMapsModelSettings.ControlsEnabled;
			vectorMapSettings.IsAreasTitlesEnabled = vectorMapsModelSettings.AreasTitlesEnabled;
			vectorMapSettings.IsMarkersTitlesEnabled = vectorMapsModelSettings.MarkersTitlesEnabled;
			vectorMapSettings.Height = vectorMapsModelSettings.Height;
			vectorMapSettings.Width = vectorMapsModelSettings.Width;
			vectorMapSettings.ZoomFactor = vectorMapsModelSettings.ZoomFactor;
			vectorMapSettings.BackgroundColor = (vectorMapsModelSettings.BackgroundColor == default(Color)) ? Color.PaleTurquoise : vectorMapsModelSettings.BackgroundColor;
			vectorMapSettings.Center = new MapPoint(vectorMapsModelSettings.CenterLatitude, vectorMapsModelSettings.CenterLongitude);
			vectorMapSettings.Type = vectorMapsModelSettings.Type;
			vectorMapSettings.DefaultAreaValue = vectorMapsModelSettings.DefaultAreaValue;
			vectorMapSettings.Palette = vectorMapsModelSettings.Palette;
			vectorMapSettings.BubbleMarkerMaxSize = vectorMapsModelSettings.BubbleMarkerMaxSize;
			vectorMapSettings.BubbleMarkerMinSize = vectorMapsModelSettings.BubbleMarkerMinSize;
			vectorMapSettings.PieMarkerSize = vectorMapsModelSettings.PieMarkerSize;
			BoundsSettings boundsSettings = vectorMapSettings.Bounds;
			boundsSettings.MinLatitude = vectorMapsModelSettings.MinLatitude;
			boundsSettings.MaxLatitude = vectorMapsModelSettings.MaxLatitude;
			boundsSettings.MinLongitude = vectorMapsModelSettings.MinLongitude;
			boundsSettings.MaxLongitude = vectorMapsModelSettings.MaxLongitude;
			LegendSettings legendSettings = vectorMapSettings.Legend;
			legendSettings.Enabled = vectorMapsModelSettings.LegendEnabled;
			legendSettings.Type = vectorMapsModelSettings.LegendType;
			foreach (IModelIntervalItem intervalItem in vectorMapsModelSettings.IntervalItems) {
				LegendItem item = new LegendItem { Title = intervalItem.Title, Value = intervalItem.Value };
				legendSettings.Items.Add(item);
			}
			return vectorMapSettings;
		}
	}
	public enum MapProvider {
		Google, Bing
	};
	public enum MapType {
		RoadMap, Satellite, Hybrid
	};
	public enum RouteMode {
		Driving, Walking
	};
	public enum VectorMapType {
		World, USA, Canada, Europe, Eurasia, Africa
	};
	public enum VectorMapLegendType {
		AreasColors, MarkersSizes, MarkersColors
	};
	public enum VectorMapPalette {
		Default, SoftPastel, HarmonyLight, Pastel, Bright, Soft, Ocean, Vintage, Violet
	};
	public enum ClientLibrariesLocations {
		Embedded,
		Custom
	}
}
