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
		static List<Module> Create_DXMap_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				new WpfModule(demo,
					name: "PhotoGallery",
					displayName: @"Photo Gallery",
					group: "Real Life Solutions",
					type: "MapDemo.PhotoGallery",
					shortDescription: @"In this demo you can see the Photo Gallery based on DXMap control.",
					description: @"
                        <Paragraph>
                        This demo illustrates the flexibility of the DevExpress WPF Map Control. The Photo Gallery allows you to locate a city within the map and then navigate to a collection of images associated with the selected city.
                        </Paragraph>
                        <Paragraph>
                        As you explore the images, you can see how we use map elements to describe each city in detail.
                        </Paragraph>",
					allowSwitchingThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "SalesDashboard",
					displayName: @"Sales Dashboard",
					group: "Real Life Solutions",
					type: "MapDemo.SalesDashboard",
					shortDescription: @"In this demo you can see a Sales dashboard created based on DXMap, DXCharts and DXGauges controls.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to create a dashboard using the Map control included in the DXMap Suite.
                        </Paragraph>
                        <Paragraph>
                        By default, this dashboard provides information about the previous month's sales made by five shops.  This dashboard also demonstrates the location of each shop on the map.
                        </Paragraph>
                        <Paragraph>
                        To get information about individual shop sales from the previous month, click a shop icon on the map. After that you will see the shop contact information on the tooltip and shop statistics on the chart.
                        </Paragraph>
                        <Paragraph>
                        Note that the gauge needle displays current total sales made by an individual shop, while gauge markers show minimum and maximum sales values made by all shops.  It helps you to assess the current total sales ranking of an individual shop among other shops.
                        </Paragraph>
                        <Paragraph>
                        Use scroll buttons to move the map in any direction and the zoom slider to see any map region in greater detail. Note that it is also possible to zoom in or out using the middle mouse button.
                        </Paragraph>",
					allowSwitchingThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "Navigator",
					displayName: @"Navigator",
					group: "Real Life Solutions",
					type: "MapDemo.Navigator",
					shortDescription: @"In this demo you can see a navigation system based on the DXMap control.",
					description: @"
                        <Paragraph>
                        This demo shows how to create a typical navigator with the help of the DXMap control. This is possible thanks to the DXMap support for Microsoft Bing GeoCode, Bing Route and Bing Search services.
                        </Paragraph>
                        <Paragraph>
                        In this demo, you can locate any place on a map (by clicking it or using the Search panel), see the detailed information about this place or specify a route consisting of two or more locations.
                        </Paragraph>
                        <Paragraph>
                        In addition, this demo can imitate the movement on a route. You can control the speed of a moving point via the Time Scale slider.
                        </Paragraph>
                        <Paragraph>
                        To start navigation, follow the instructions displayed in this demo.
                        </Paragraph>",
					allowSwitchingThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "WorldWeather",
					displayName: @"World Weather",
					group: "Real Life Solutions",
					type: "MapDemo.WorldWeather",
					shortDescription: @"This demo illustrates the capability of a map to display weather data obtained from various weather services.",
					description: @"
                        <Paragraph>
                        A map control can display weather data obtained from various weather services.
                        </Paragraph>
                        <Paragraph>
                        In this demo you can see the current weather in the world’s largest cities.
                        </Paragraph>
                        <Paragraph>
                        To see the weather forecast on a graph for a specific city, click the city’s label on a map. This invokes the chart showing potential changes in temperature for a selected city.
                        </Paragraph>
                        <Paragraph>
                        NOTE. The weather data for this demo is provided by the OpenWeatherMap service (http://www.openweathermap.org).
                        </Paragraph>",
					allowSwitchingThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "BingMapsProvider",
					displayName: @"Bing Maps Provider",
					group: "Geo Data Sources",
					type: "MapDemo.BingMapsProvider",
					shortDescription: @"This is a typical map provided by the Bing Maps source.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to load a map using the Bing Maps data provider.
                        </Paragraph>
                        <Paragraph>
                        To move the map in any direction, you can use either a drag operation with the mouse or the scroll buttons at the top left.
                        </Paragraph>
                        <Paragraph>
                        To zoom in the map for more detail or to zoom out for a wider view, you can use the scroll wheel on the mouse or the zoom slider.
                        </Paragraph>
                        <Paragraph>
                        NOTE: Image tiles for this demo are provided by the Bing Maps website at http://www.bing.com/maps/. When using Bing Maps, you must read and understand terms of use. Read the Bing Maps Licensing and Pricing Information at http://www.microsoft.com/maps/product/licensing.aspx.
                        </Paragraph>
                        <Paragraph>
                        By default, the map displays an area. To see a<LineBreak/>road/street map or a hybrid view, use the options below.
                        </Paragraph>",
					allowSwitchingThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V142
				),
				new WpfModule(demo,
					name: "OpenStreetMapProvider",
					displayName: @"OpenStreetMap Provider",
					group: "Geo Data Sources",
					type: "MapDemo.OpenStreetMapProvider",
					shortDescription: @"This is a typical map provided by the OpenStreetMap source.",
					description: @"
                        <Paragraph>
                        This demo illustrates use of map images from the OpenStreetMap data provider.
                        </Paragraph>
                        <Paragraph>
                        Use scroll buttons to move the map in any direction and the zoom slider to see any map region in greater detail. Note that it is also possible to zoom in or out using the middle mouse button.
                        </Paragraph>
                        <Paragraph>
                        NOTE: Image tiles for this demo are provided by the OpenStreetMap website at<LineBreak/>http://www.openstreetmap.org. When using OpenStreetMap data, you must read and understand terms of use (http://wiki.openstreetmap.org/wiki/Legal_FAQ) and the Tile Usage Policy (http://wiki.openstreetmap.org/wiki/Tile_usage_policy).
                        </Paragraph>
                        <Paragraph>
                        By default, the map displays a road/street view. Use the options below to see a hybrid view of the map.
                        </Paragraph>",
					allowSwitchingThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V151
				),
				new WpfModule(demo,
					name: "MultipleLayers",
					displayName: @"Multiple Layers",
					group: "Features",
					type: "MapDemo.MultipleLayers",
					shortDescription: @"This demo shows how to simultaneously draw several layers on a single map.",
					description: @"
                        <Paragraph>
                        This demo illustrates the use of multiple layers.
                        </Paragraph>
                        <Paragraph>
                        Map layers are intended to display map images or various vector elements. For example, in this demo one layer is used to show reliefs provided by the Bing Maps data source, while another layer displays a smaller area as a road map using the OpenStreetMap data provider.
                        </Paragraph>
                        <Paragraph>
                        Note that when one layer is shown above another, the DXMap control can clip appropriate layers using shape contours of any complexity. For example, in this demo the road layer has been clipped using a complex polygon.
                        </Paragraph>
                        <Paragraph>
                        In addition, it is possible to specify minimum and maximum zoom levels at which a layer should be displayed. For example, the layer that shows the interior road map becomes invisible when the minimum zoom level is reached.
                        </Paragraph>",
					allowSwitchingThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "MapElements",
					displayName: @"Map Elements",
					group: "Features",
					type: "MapDemo.MapElements",
					shortDescription: @"The map control supports the drawing of various graphics elements on it (e.g., ellipse, polygon, polyline, etc.).",
					description: @"
                        <Paragraph>
                        This demo illustrates the three different map elements that can be displayed on the map: a polyline (in this demo, the flight path for an airplane is a polyline), a polygon (here, the airplane icons on the map) and a custom element used to draw an ellipse (here, the small circles indicating the initial and final airplane destinations).
                        </Paragraph>
                        <Paragraph>
                        Click an airplane icon on the map to get information about the airplane, its flight number, other flight data, and to see the flight path.
                        </Paragraph>
                        <Paragraph>
                        You can also change the time scale using the slider below.
                        </Paragraph>",
					allowSwitchingThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V152
				),
				new WpfModule(demo,
					name: "ImportExport",
					displayName: @"Import Export",
					group: "Features",
					type: "MapDemo.ImportExport",
					shortDescription: @"The DXMap Suite provides support for the loading of vector elements from Shapefiles.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to generate a map from shape elements stored in a Shapefile. This file contains information on shape contours, as well as additional information.
                        </Paragraph>
                        <Paragraph>
                        Note that the DXMap control supports automatic highlighting of the currently selected/hovered shape. To see how this works, move the mouse pointer so that it hovers over any country - and you will see that its region has become highlighted. In addition, the hit-testing feature provides the capability to invoke a tooltip with some information about this country.
                        </Paragraph>
                        <Paragraph>
                        NOTE: Shapefiles for this demo were obtained from the Natural Earth web resource<LineBreak/>(http://www.naturalearthdata.com)
                        </Paragraph>",
					allowSwitchingThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V152
				),
				new WpfModule(demo,
					name: "Colorizer",
					displayName: @"Colorizer",
					group: "Features",
					type: "MapDemo.Colorizer",
					shortDescription: @"This demo illustrates a map control's capability to automatically color shapes depending on supplementary data.",
					description: @"
                        <Paragraph>
                        This demo shows how to create GDP, Population, and Political maps using the map colorizer.  This colorizer fills each country shape with a color depending on geographic data.
                        </Paragraph>
                        <Paragraph>
                        Note that GDP and Population map kinds have a map legend. This legend contains a color scale accompanied by labels, which help interpret colors on a map.
                        </Paragraph>
                        <Paragraph>
                        In addition, this demo shows automatic highlighting of the currently hovered shape. To see how it works, move the mouse pointer over any shape and see that its contour has become highlighted.
                        </Paragraph>
                        <Paragraph>
                        You can also click any shape on a map and see a tooltip with additional information about this country.
                        </Paragraph>
                        <Paragraph>
                        NOTE: The Shapefile for this demo was obtained from the Natural Earth web resource (http://www.naturalearthdata.com).
                        </Paragraph>",
					allowSwitchingThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "DataBinding",
					displayName: @"Data Binding",
					group: "Features",
					type: "MapDemo.DataBinding",
					shortDescription: @"This demo illustrates the data binding feature of a map control.",
					description: @"
                        <Paragraph>
                        The map control supports binding to vector data from various data sources.
                        </Paragraph>
                        <Paragraph>
                        In this demo you can see wrecked ship images generated automatically after a map control has been bound to the XML data source. The XML file contains name and coordinates for each wrecked ship.
                        </Paragraph>
                        <Paragraph>
                        To see the ship description on a tooltip, click the corresponding icon on a map.
                        </Paragraph>",
					allowSwitchingThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V142
				),
				new WpfModule(demo,
					name: "BubbleCharts",
					displayName: @"Bubble Charts",
					group: "Real Life Solutions",
					type: "MapDemo.BubbleCharts",
					shortDescription: @"This demo illustrates the use of Bubble charts within a map.",
					description: @"
                        <Paragraph>
                        In this demo, the map control displays earthquake occurrences around the world (by year).The magnitude of individual earthquakes are visualized using Bubble charts and colors are used to specify the year of occurrence.
                        </Paragraph>
                        <Paragraph>
                        As you can see, because Bubble size is proportional to the magnitude, it simplifies the comparison of quake events.
                        </Paragraph>
                        <Paragraph>
                        Press the Options button to customize the contents of the Map. You can control the year, magnitude and marker type used for earthquake information displayed on the map.
                        </Paragraph>",
					allowSwitchingThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.V142
				),
				new WpfModule(demo,
					name: "EnergyStatistics",
					displayName: @"Energy Statistics",
					group: "Real Life Solutions",
					type: "MapDemo.EnergyStatistics",
					shortDescription: @"This demo illustrates the use of Pie charts within a map. ",
					description: @"
                        <Paragraph>
                        The DevExpress WPF Map Control can display chart data within it. This demo illustrates the use of Pie charts and displays energy statistics in Europe (by country).
                        </Paragraph>
                        <Paragraph>
                        The Pie is segmented by energy sectors. Pie chart size displays total energy produced or imported.
                        </Paragraph>",
					allowSwitchingThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.V142
				),
				new WpfModule(demo,
					name: "DayAndNight",
					displayName: @"Day and Night",
					group: "Features",
					type: "MapDemo.DayAndNight",
					shortDescription: @"This demo illustrates the use of geographical map projections.",
					description: @"
                        <Paragraph>
                        The DevExpress Map Control can display geographical data using any number of unique map projections. In this demo, you can view day/night across the planet using different map projections.
                        </Paragraph>
                        <Paragraph>
                        To change the current map projection, date/time and map interactivity speed, press the Options button then select an appropriate value from list on the right.
                        </Paragraph>",
					allowSwitchingThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.V142
				),
				new WpfModule(demo,
					name: "HotelPlans",
					displayName: @"Hotel Plans",
					group: "Features",
					type: "MapDemo.HotelPlans",
					shortDescription: @"This demo illustrates the use of different coordinate systems",
					description: @"
                        <Paragraph>
                        The Map control supports the display of both Geographical and Cartesian maps.
                        </Paragraph>
                        <Paragraph>
                        Geographical maps often show geospatial data using geographic coordinates (latitude and longitude), while Cartesian maps use X and Y coordinates to draw building plans or any other Cartesian data. This demo demonstrates the use of both map modes.
                        </Paragraph>
                        <Paragraph>
                        To view a hotel plan, select the hotel's pushpin.
                        </Paragraph>",
					allowSwitchingThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.V151
				),
				new WpfModule(demo,
					name: "Clustering",
					displayName: @"Clustering",
					group: "Features",
					type: "MapDemo.Clustering",
					shortDescription: @"The Map Control supports the vector data aggregation using one of the map item clusterers",
					description: @"<Paragraph> 
                    The Map Control supports the vector data aggregation using one of the map item clusterers.
                    </Paragraph>
                    <Paragraph> 
                    The clusterers allow you to display custom cluster representatives instead of the default and group tree map items by specified values before clustering. The clustering helps to improve the data clarity and optimize the hardware resources utilization. </Paragraph>
                    <Paragraph> 
                    This demo demonstrates the use of the clusterers.
                    </Paragraph>
                    <Paragraph> 
                    In this demo the Ballarat Trees data set has been used (obtained from https://data.gov.au/).
                    </Paragraph>",
					allowSwitchingThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.V152
				)
			};
		}
	}
}
