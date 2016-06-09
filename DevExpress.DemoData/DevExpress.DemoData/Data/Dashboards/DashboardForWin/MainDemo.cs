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
		static List<Module> Create_DashboardForWin_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				new SimpleModule(demo,
					name: "About",
					displayName: @"DevExpress Dashboard %MarketingVersion%",
					group: "About",
					type: "DashboardMainDemo.Modules.About",
					description: @"",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "SalesOverview",
					displayName: @"Sales Overview",
					group: "Main",
					type: "DashboardMainDemo.Modules.SalesOverview",
					description: @"This dashboard shows statistics on sales of bicycles, related equipment and accessories.
The grid on the left shows sales breakdown by state. Gauges illustrate sales by the product category compared to target values. The chart visualizes sales through time by the product category.
Click one or several rows in the grid to view data for required states on the chart and gauges. To reset the selection, use the Clear Master Filter button in the grid caption.
You can also select a time interval in the Range Filter at the bottom to filter data by date. Alternatively, use radio buttons below the Range Filter to select predefined time intervals.
Use the Edit button to run the designer and edit the dashboard.",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V152,
					newUpdatedPriority: 1,
					associatedFiles: new [] {
						@"\Dashboard\CS\DashboardMainDemo\Modules\SalesOverview.cs",
						@"\Dashboard\VB\DashboardMainDemo\Modules\SalesOverview.vb"
					}
				),
				new SimpleModule(demo,
					name: "SalesPerformance",
					displayName: @"Sales Performance",
					group: "Main",
					type: "DashboardMainDemo.Modules.SalesPerformance",
					description: @"This dashboard displays sales performance parameters YTD.
The map indicates sales by state in different colors. Use the Values button in the map caption to switch between different values. You can also select one or several states in the map to view sales data for specific states. To reset the selection, use the Clear Master Filter button in the map caption.
Cards at the right show key sales metrics compared to the target. The grid displays sales by product and marks top 10/bottom 10 products. Moreover, Revenue values are colored depending on whether they greater or less than average.
The chart illustrates monthly sales for top five products.
Use the Edit button to run the designer and edit the dashboard.",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V152,
					newUpdatedPriority: 2,
					associatedFiles: new [] {
						@"\Dashboard\CS\DashboardMainDemo\Modules\SalesPerformance.cs",
						@"\Dashboard\VB\DashboardMainDemo\Modules\SalesPerformance.vb"
					}
				),
				new SimpleModule(demo,
					name: "SalesDetails",
					displayName: @"Sales Details",
					group: "Main",
					type: "DashboardMainDemo.Modules.SalesDetails",
					description: @"This dashboard provides an in-depth view of sales data.
Use list boxes on the left to view data for individual categories and states.
The cards on the right display sales by product. Click the desired card to see product sale details along with underlying data.
You can also select the value to display within cards. Use the Values button in the caption to switch between the Revenue, Units Sold, and Returns.
Use the Edit button to run the designer and edit the dashboard.",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V152,
					newUpdatedPriority: 3,
					associatedFiles: new [] {
						@"\Dashboard\CS\DashboardMainDemo\Modules\SalesDetails.cs",
						@"\Dashboard\VB\DashboardMainDemo\Modules\SalesDetails.vb"
					}
				),
				new SimpleModule(demo,
					name: "RevenueAnalysis",
					displayName: @"Revenue Analysis",
					group: "Main",
					type: "DashboardMainDemo.Modules.RevenueAnalysis",
					description: @"This dashboard shows revenue data.
The bar chart shows the revenue by year, while the pies show the revenue and number of units sold by the product category. 
The pivot table shows the revenue and number of units sold by product and state. Revenue data cells are colored using color ranges depending on they values while top states and the best category are marked.
You can click on chart bars and pie segments to filter data in the pivot table by year and category respectively. To clear the selection, click the Clear Master Filter button in the context menu of the chart and pie dashboard items.
Use the Edit button to run the designer and edit the dashboard.",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V151,
					associatedFiles: new [] {
						@"\Dashboard\CS\DashboardMainDemo\Modules\RevenueAnalysis.cs",
						@"\Dashboard\VB\DashboardMainDemo\Modules\RevenueAnalysis.vb"
					}
				),
				new SimpleModule(demo,
					name: "HumanResources",
					displayName: @"Human Resources",
					group: "Main",
					type: "DashboardMainDemo.Modules.HumanResources",
					description: @"This dashboard contains human resources department statistics.
The grid and cards on the left show staff turnover through time and staff turnover breakdown by department respectively. If the turnover rate exceeds 1%, the value gets marked with a warning - an orange circle. The chart at the top shows how the size of each department changes with time. Bar charts and pies show payroll structure and work absence reason statistics for top 10 employees and for the entire company.
In the grid, you can select years to filter data in pies, bar charts and cards. You can also double-click rows to drill-down to months and select the required month in the same manner. To drill up, click the Drill Up button in the grid dashboard item's caption.
You can also select cards to filter data in bar charts and pies by department. To reset the selection, use the Clear Master Filter button in the card dashboard item caption.
Use the Edit button to run the designer and edit the dashboard.",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V152,
					newUpdatedPriority: 4,
					associatedFiles: new [] {
						@"\Dashboard\CS\DashboardMainDemo\Modules\HumanResources.cs",
						@"\Dashboard\VB\DashboardMainDemo\Modules\HumanResources.vb"
					}
				),
				new SimpleModule(demo,
					name: "CustomerSupport",
					displayName: @"Customer Support",
					group: "Main",
					type: "DashboardMainDemo.Modules.CustomerSupport",
					description: @"This dashboard shows data from customer support service statistics.
Charts on the left show the number of opened issues through time and the number of processed issues by platform. The charts on the right show the average response time in the same manner.
In both bar charts, you can drill down from platforms to support engineers. To do this, click any bar, and the chart will show data for this platform by employee. To drill up, click the Drill Up button in the chart dashboard item caption.
You can also select a time interval in the Range Filter at the bottom to filter data by date.
Use the Edit button to run the designer and edit the dashboard.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Dashboard\CS\DashboardMainDemo\Modules\CustomerSupport.cs",
						@"\Dashboard\VB\DashboardMainDemo\Modules\CustomerSupport.vb"
					}
				),
				new SimpleModule(demo,
					name: "RevenueByIndustry",
					displayName: @"Revenue by Industry",
					group: "Main",
					type: "DashboardMainDemo.Modules.RevenueByIndustry",
					description: @"This dashboard displays revenue data by industry.
The list box on the left lists US industries, while the map indicates revenues for all industries based on geographical locations.
You can select list box values to view revenue distribution for specific industries.
The map supports clustering which is used to aggregate bubbles based on map size or zoom level. Click map bubbles/callouts to view the industries present in the selected region/city.
To reset selection, use the Clear Master Filter button.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Dashboard\CS\DashboardMainDemo\Modules\RevenueByIndustry.cs",
						@"\Dashboard\VB\DashboardMainDemo\Modules\RevenueByIndustry.vb"
					}
				),
				new SimpleModule(demo,
					name: "EnergyConsumption",
					displayName: @"Energy Consumption",
					group: "Main",
					type: "DashboardMainDemo.Modules.EnergyConsumption",
					description: @"This dashboard shows energy consumption by country.
The bubble map on the left indicates the energy production for each country via the bubble size, while the energy shortage is expressed via the bubble color.
You can select individual countries on the map to see more detailed information. The pie on the right shows energy consumption by sector for the selected country.
The chart displays energy consumption by sector over time. Use the Parameters button in the dashboard title to display energy statistics in different years.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Dashboard\CS\DashboardMainDemo\Modules\EnergyConsumption.cs",
						@"\Dashboard\VB\DashboardMainDemo\Modules\EnergyConsumption.vb"
					}
				),
				new SimpleModule(demo,
					name: "EnergyStatistics",
					displayName: @"Energy Statistics",
					group: "Main",
					type: "DashboardMainDemo.Modules.EnergyStatistics",
					description: @"This dashboard shows energy statistics by country.
The pie map on the left shows the energy production and import for european countries and indicates the contribution of each energy type to the total values. Use the Values button in the map caption to switch between the production and import.
You can select required countries to see detailed information. Cards show energy production compared to import for the selected countries. Select the energy type to see the domestic share and the variation of the energy production over time for each country in the grid. Note that the domestic share values are colored depending on their values.
To reset the selection, use Clear Master Filter buttons. Use the Parameters button in the dashboard title to display energy statistics in different years.",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V151,
					associatedFiles: new [] {
						@"\Dashboard\CS\DashboardMainDemo\Modules\EnergyStatistics.cs",
						@"\Dashboard\VB\DashboardMainDemo\Modules\EnergyStatistics.vb"
					}
				),
				new SimpleModule(demo,
					name: "WebsiteStatistics",
					displayName: @"Website Statistics",
					group: "Main",
					type: "DashboardMainDemo.Modules.WebsiteStatistics",
					description: @"This dashboard displays website statistics including traffic information and browser usage.
The dashboard contains two groups. The top group visualizes variations related to visitor count and allows you to filter this data by traffic sources using the tree view on the left.
The bottom group evaluates browser usage based upon browser version.",
					addedIn: KnownDXVersion.V142,
					associatedFiles: new [] {
						@"\Dashboard\CS\DashboardMainDemo\Modules\WebsiteStatistics.cs",
						@"\Dashboard\VB\DashboardMainDemo\Modules\WebsiteStatistics.vb"
					}
				),
				new SimpleModule(demo,
					name: "ChampionsLeagueStatistics",
					displayName: @"Champions League Statistics",
					group: "Main",
					type: "DashboardMainDemo.Modules.ChampionsLeagueStatistics",
					description: @"This dashboard displays the UEFA Champions League statistics by football clubs for three seasons.
The Scatter Chart visualizes league statistics for countries whose clubs participated at least in one season. Click the required bubble to see country statistics by season on the Bar Chart below. You can also double-click bubbles to drill down to detailed statistics by club. Besides, you can select bars on the Bar Chart below to filter the Scatter Chart by corresponding seasons.
The pivot table on the right shows goal difference statistics by clubs for all seasons. You can also use the Combo Box above to filter the pivot by countries.",
					addedIn: KnownDXVersion.V152,
					newUpdatedPriority: 0,
					associatedFiles: new [] {
						@"\Dashboard\CS\DashboardMainDemo\Modules\ChampionsLeagueStatistics.cs",
						@"\Dashboard\VB\DashboardMainDemo\Modules\ChampionsLeagueStatistics.vb"
					}
				)
			};
		}
	}
}
