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
namespace DevExpress.DemoData.Model {
	public static partial class Repository {
		static List<Module> Create_XtraMap_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				new SimpleModule(demo,
					name: "About",
					displayName: @"DevExpress XtraMap %MarketingVersion%",
					group: "About",
					type: "DevExpress.XtraMap.Demos.About",
					description: @"",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "BingServices",
					displayName: @"Bing Services",
					group: "Features",
					type: "DevExpress.XtraMap.Demos.BingServices",
					description: @"This demo shows the map control that supports Microsoft Bing Geocode, Bing Route and Bing Search services.  In this demo, you can locate any place on a map (by clicking it or typing a place name in the Search panel), see detailed information about this place (by hovering over a pushpin) or specify a route consisting of two or more places (by clicking two or more pushpins on a map). If the Bing Search service finds alternative places for a request, the ""Show others…"" option will appear in search results.  To go back to the best search result for a place, click ""Show best result…"" in the Search panel.  In addition, this demo can imitate the movement on a route. To start navigation, follow the instructions displayed in this demo.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "SalesDashboard",
					displayName: @"Sales Dashboard",
					group: "Features",
					type: "DevExpress.XtraMap.Demos.SalesDashboard",
					description: @"This demo illustrates how to create a dashboard using the Map control. By default, this dashboard provides information about the previous month's sales made by five shops.  This dashboard also demonstrates the location of each shop on the map. To get information about individual shop sales from the previous month, click a shop icon on the map. After that you will see the shop contact information on the tooltip and shop statistics on the chart. Note that the gauge needle displays current total sales made by an individual shop, while gauge markers show minimum and maximum sales values made by all shops.  It helps you to assess the current total sales ranking of an individual shop among other shops.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "WorldWeather",
					displayName: @"World Weather",
					group: "Features",
					type: "DevExpress.XtraMap.Demos.WorldWeather",
					description: @"A map control can display weather data obtained from various weather services. In this demo you can see the current weather in the world's largest cities. To see the weather forecast on a graph for a specific city, click the city's label on a map. This invokes the chart showing potential changes in temperature for a selected city. NOTE. The weather data for this demo is provided by the OpenWeatherMap service (http://www.openweathermap.org).",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "MapElements",
					displayName: @"Map Elements",
					group: "Features",
					type: "DevExpress.XtraMap.Demos.MapElements",
					description: @"This demo illustrates the three different map elements that can be displayed on the map: a polyline (in this demo, the flight path for an airplane is a polyline), a polygon (here, the airplane icons on the map) and a custom element used to draw an ellipse (here, the small circles indicating the initial and final airplane destinations). Click an airplane icon on the map to get information about the airplane, its flight number, other flight data, and to see the flight path. You can also change the time scale using the slider below.",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V152
				),
				new SimpleModule(demo,
					name: "ImportExport",
					displayName: @"Import Export",
					group: "Features",
					type: "DevExpress.XtraMap.Demos.ImportExport",
					description: @"This demo illustrates how to generate a map from shape elements stored in a Shapefile. This file contains information on shape contours, as well as additional information (e.g. country names, GDP, population and other values). Note that the Map Control supports automatic highlighting of the currently selected/ hovered shape. Use the mouse pointer to select or hover over any country on a map and see related information about this country in a tooltip.",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V152
				),
				new SimpleModule(demo,
					name: "BingMapDataProviders",
					displayName: @"Bing Map Data Providers",
					group: "Features",
					type: "DevExpress.XtraMap.Demos.BingMapDataProviders",
					description: @"This demo illustrates how to load a map using Bing Maps data provider. Use the corresponding Map Kind option to select which map should be displayed: Area (photos of the Earth's surface), Road (street view) or Hybrid (combination of both images and schemes).",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V151
				),
				new SimpleModule(demo,
					name: "OSMDataProviders",
					displayName: @"OpenStreetMap Data Providers",
					group: "Features",
					type: "DevExpress.XtraMap.Demos.OSMDataProviders",
					description: @"This demo illustrates how to load a map using OpenStreetMap data provider.",
					addedIn: KnownDXVersion.V151,
					updatedIn: KnownDXVersion.V151
				),
				new SimpleModule(demo,
					name: "DataSource",
					displayName: @"Data Source",
					group: "Features",
					type: "DevExpress.XtraMap.Demos.DataSource",
					description: @"The map control supports binding to vector data from various data sources. In this demo you can see wrecked ship images generated automatically after a map control has been bound to the XML data source. The XML file contains name and coordinates for each wrecked ship. To see the ship description on a tooltip, hover over the corresponding icon on a map.",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V142
				),
				new SimpleModule(demo,
					name: "OlympicMedals",
					displayName: @"Olympic Medals",
					group: "Features",
					type: "DevExpress.XtraMap.Demos.OlympicMedals",
					description: @"This demo shows the count of Sochi 2014 Olympic medals by countries. You can see the proportion of medals that each country won in a tooltip by hovering over the selected Pie chart on a map. The pies are sized depending on the medal count and colored in each medals type. To select a pie, click the required pie item on the map or select a country in the grid control on the left of the demo. In addition you can change the legend size type (e.g., to Inline Size Legend) or remove the Color legend using the demo options.",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V142
				),
				new SimpleModule(demo,
					name: "BubbleCharts",
					displayName: @"Bubble Charts",
					group: "Features",
					type: "DevExpress.XtraMap.Demos.BubbleCharts",
					description: @"In this demo, the map control shows earthquakes around the world by years. Magnitudes are displayed on a map using the Bubble chart. The bubble size is proportional to the magnitude, and it simplifies the process of comparing quake events. In addition, the Bubble chart is colored on a map depending on the earthquake history period. To see a specific history period (e.g. 1970-1979), click the corresponding item in the Years Filter. To change the magnitude range on a map, use the Magnitude Filter slider. You can customize the marker type in the Marker Type drop-down list.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "PhotoGallery",
					displayName: @"Photo Gallery",
					group: "Features",
					type: "DevExpress.XtraMap.Demos.PhotoGallery",
					description: @"This demo illustrates the flexibility of the DevExpress WinForms Map Control. The Photo Gallery allows you to locate a city within the map and then navigate to a collection of images associated with the selected city. As you explore the images, you can see how we use map elements to describe each city in detail.",
					addedIn: KnownDXVersion.V142
				),
				new SimpleModule(demo,
					name: "HotelPlans",
					displayName: @"Hotel Plans",
					group: "Features",
					type: "DevExpress.XtraMap.Demos.HotelPlans",
					description: @"The Map control supports the display of both Geographical and Cartesian maps. Geographical maps often show geospatial data using geographic coordinates (latitude and longitude), while Cartesian maps use X and Y coordinates to draw building plans or any other Cartesian data. This demo demonstrates the use of both map modes. To view a hotel plan, select the hotel's pushpin.",
					addedIn: KnownDXVersion.V142
				),
				new SimpleModule(demo,
					name: "DayAndNight",
					displayName: @"Day and Night",
					group: "Features",
					type: "DevExpress.XtraMap.Demos.DayAndNight",
					description: @"The DevExpress Map Control can display geographical data using any number of unique map projections. In this demo, you can view day/night across the planet using different map projections. To change the current map projection, select the desired value from the list on the right. To change date/time and see its effects within the map, use the data selector within the Ribbon bar.",
					addedIn: KnownDXVersion.V142
				),
				  new SimpleModule(demo,
					name: "Clustering",
					displayName: @"Clustering",
					group: "Features",
					type: "DevExpress.XtraMap.Demos.Clustering",
					description: @"The Map Control supports the vector data aggregation using one of the map item clusterers. The clusterers allow you to display custom cluster representatives instead of the default and group tree map items by specified values before clustering. The clustering helps to improve the data clarity and optimize the hardware resources utilization. This demo demonstrates the use of the clusterers. In this demo the Ballarat Trees data set has been used (obtained from https://data.gov.au/).",
					addedIn: KnownDXVersion.V152
				),
				new SimpleModule(demo,
					name: "FlagsGame",
					displayName: @"Flags Game",
					group: "Features",
					type: "DevExpress.XtraMap.Demos.FlagsGame",
					description: @"This demo demonstrates you the Flags game implemented using the Map Control Overlays feature. Overlays allow you to display the semitransparent images and text over the map. The flag images have been obtained from Wikipedia.",
					addedIn: KnownDXVersion.V152
				)
			};
		}
	}
}
