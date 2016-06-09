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
		static List<Module> Create_XtraGauges_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				new SimpleModule(demo,
					name: "AnimationScale",
					displayName: @"Animation",
					group: "Features",
					type: "DevExpress.XtraGauges.Demos.AnimationScale",
					description: @"This demo illustrates how value indicators can be animated in XtraGauges. In this sample when a needle is moving from one value to another, it uses the specified Ease mode and function to animate its value.",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 2,
					isFeatured: true
				),
				new SimpleModule(demo,
					name: "GaugeStylesXPF",
					displayName: @"Modern Styles",
					group: "Features",
					type: "DevExpress.XtraGauges.Demos.GaugeStylesXPF",
					description: @"In addition to the set of basic and advanced gauge styles, now XtraGauges have 11 more new appearances! Note that these styles are available for all gauge types (Circular, Linear and Digital) and thus provide a consistent look and feel for your gauge dashboard.",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 3,
					isFeatured: true
				),
				new SimpleModule(demo,
					name: "LogarithmicScaleGaugeFeatures",
					displayName: @"Logarithmic Scale",
					group: "Features",
					type: "DevExpress.XtraGauges.Demos.LogarithmicScaleGaugeFeatures",
					description: @"This demo illustrates a logarithmic scale gauges (Circular and Linear). Practice customizing this scale.",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 1,
					isFeatured: true
				),
				new SimpleModule(demo,
					name: "ColorSchemeGauges",
					displayName: @"Color Scheme",
					group: "Features",
					type: "DevExpress.XtraGauges.Demos.ColorSchemeGauges",
					description: @"This demo illustrates how to easily repaint the DevExpress Gauge Control or any of its elements using Color Schemes.",
					addedIn: KnownDXVersion.V142,
					featuredPriority: 1,
					newUpdatedPriority: 0,
					isFeatured: true
				),
				new SimpleModule(demo,
					name: "About",
					displayName: @"DevExpress XtraGauges %MarketingVersion%",
					group: "About",
					type: "DevExpress.XtraGauges.Demos.About",
					description: @"",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "StyleManagerFeature",
					displayName: @"Style Manager",
					group: "Presets",
					type: "DevExpress.XtraGauges.Demos.StyleManagerFeature",
					description: @"This demo illustrates how end-users can customize the appearance of a Gauge control at runtime.",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 4,
					isFeatured: true,
					associatedFiles: new [] {
						@"\WinForms\CS\GaugesMainDemo\Modules\StyleManager.cs",
						@"\WinForms\VB\GaugesMainDemo\Modules\StyleManager.vb"
					}
				),
				new SimpleModule(demo,
					name: "NewPresets",
					displayName: @"New Presets",
					group: "Presets",
					type: "DevExpress.XtraGauges.Demos.NewPresets",
					description: @"This demo illustrates new presets that allow you to create various types of indicators, such as the Rating Control, Bullet Graphs, Equalizer, etc.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "GaugesNewStyles",
					displayName: @"Advanced Styles",
					group: "Features",
					type: "DevExpress.XtraGauges.Demos.GaugesNewStyles",
					description: @"The XtraGauges provides new 5 built-in styles. In this demo, you can switch between tab pages to see the corresponding styles.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\GaugesMainDemo\Modules\NewStyles.cs",
						@"\WinForms\VB\GaugesMainDemo\Modules\NewStyles.vb"
					}
				),
				new SimpleModule(demo,
					name: "GaugesStyles",
					displayName: @"Styles",
					group: "Features",
					type: "DevExpress.XtraGauges.Demos.GaugesStyles",
					description: @"The XtraGauges provides 10 built-in styles. In this demo, you can switch between tab pages to see the corresponding styles.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\GaugesMainDemo\Modules\Styles.cs",
						@"\WinForms\VB\GaugesMainDemo\Modules\Styles.vb"
					}
				),
				new SimpleModule(demo,
					name: "CustomDrawFeature",
					displayName: @"Custom Draw",
					group: "Features",
					type: "DevExpress.XtraGauges.Demos.CustomDrawFeature",
					description: @"This demo illustrates how to use the CustomDraw event to completely customize the appearance of a Gauge control.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\GaugesMainDemo\Modules\CustomDraw.cs",
						@"\WinForms\VB\GaugesMainDemo\Modules\CustomDraw.vb"
					}
				),
				new SimpleModule(demo,
					name: "ZoomFeature",
					displayName: @"Zoom",
					group: "Features",
					type: "DevExpress.XtraGauges.Demos.ZoomFeature",
					description: @"This demo illustrates the zooming feature that allows you to change the gauge's scale without loss of quality.",
					addedIn: KnownDXVersion.Before142,
					 associatedFiles: new [] {
						@"\WinForms\CS\GaugesMainDemo\Modules\Zoom.cs",
						@"\WinForms\VB\GaugesMainDemo\Modules\Zoom.vb"
					}
				),
				new SimpleModule(demo,
					name: "InteractionFeature",
					displayName: @"Event-Based Interaction",
					group: "Features",
					type: "DevExpress.XtraGauges.Demos.InteractionFeature",
					description: @"This demo illustrates how to make the control respond to end-user actions. In the demo, the control's MouseMove and MouseDown events are handled to allow an end-user to change Gauges' scale values.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\GaugesMainDemo\Modules\Interaction.cs",
						@"\WinForms\VB\GaugesMainDemo\Modules\Interaction.vb"
					}
				),
				new SimpleModule(demo,
					name: "AutoLayoutFeature",
					displayName: @"Auto Layout",
					group: "Features",
					type: "DevExpress.XtraGauges.Demos.AutoLayoutFeature",
					description: @"This demo illustrates the auto-layout feature. If you move a splitter or change the form's size, gauges will still be consistently arranged without overlapping.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\GaugesMainDemo\Modules\AutoLayout.cs",
						@"\WinForms\VB\GaugesMainDemo\Modules\AutoLayout.vb"
					}
				),
				new SimpleModule(demo,
					name: "Databinding",
					displayName: @"Data Binding",
					group: "Features",
					type: "DevExpress.XtraGauges.Demos.Databinding",
					description: @"This demo illustrates data-bound gauges. Scroll through the grid to see how the gauges' values change.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "CircularGaugeFeatures",
					displayName: @"Circular Gauge",
					group: "Gauges",
					type: "DevExpress.XtraGauges.Demos.CircularGaugeFeatures",
					description: @"This demo illustrates a circular gauge type. Practice customizing various display options, and immediately see the result in the preview pane.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "DigitalGaugeFeatures",
					displayName: @"Digital Gauge",
					group: "Gauges",
					type: "DevExpress.XtraGauges.Demos.DigitalGaugeFeatures",
					description: @"This demo illustrates a digital gauge type. See how the gauge displays various symbols using different display modes. Click the check box to enable the custom character mapping feature that allows you to paint specific characters in a custom manner.",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 5,
					isFeatured: true
				),
				new SimpleModule(demo,
					name: "DigitalGauges",
					displayName: @"Digital Gauge Display Modes",
					group: "Gauges",
					type: "DevExpress.XtraGauges.Demos.DigitalGauges",
					description: @"This demo illustrates the available display modes for digital gauges and shows which font symbols are supported in these modes.",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 7,
					isFeatured: true
				),
				new SimpleModule(demo,
					name: "LinearGaugeFeatures",
					displayName: @"Linear Gauge",
					group: "Gauges",
					type: "DevExpress.XtraGauges.Demos.LinearGaugeFeatures",
					description: @"This demo illustrates a linear gauge type. Practice customizing various display options, and immediately see the result in the preview pane.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "StateIndicatorGaugeFeatures",
					displayName: @"State Indicators",
					group: "Gauges",
					type: "DevExpress.XtraGauges.Demos.StateIndicatorGaugeFeatures",
					description: @"This demo illustrates State Indicator gauges. Gauges of this type have a list of predefined vector images associated with gauge states. You can use track bars to modify the current gauge state - the displayed image will be updated accordingly.",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V142,
					newUpdatedPriority: 1
				),
				 new SimpleModule(demo,
					name: "StateImageIndicatorWeatherStation",
					displayName: @"Weather Station",
					group: "Gauges",
					type: "DevExpress.XtraGauges.Demos.StateImageIndicatorWeatherStation",
					description: @"This demo illustrates gauges that use image state indicators - elements that store a set of predefined images changed according to the current gauge state. Gauges in this demo use data from http://openweathermap.org/ to display the current weather in different cities. Depending on the current weather (rainy, sunny, snowy, etc.), gauges display different icons within their image state indicators. Additionally, the color scheme applied to each gauge is changed depending on whether or not the temperature in the target city is currently above zero.",
					addedIn: KnownDXVersion.V142,
					isFeatured: true,
					featuredPriority: 0
				),
				new SimpleModule(demo,
					name: "DigitalClock",
					displayName: @"Digital Clock",
					group: "Gadgets",
					type: "DevExpress.XtraGauges.Demos.DigitalClock",
					description: @"This is a real-world example of using the digital gauge type. The gauge displays the current time.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "AnalogClock",
					displayName: @"Analog Clock",
					group: "Gadgets",
					type: "DevExpress.XtraGauges.Demos.AnalogClock",
					description: @"This is a real-world example of using the circular gauge type. The gauge displays the current time.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "TravelingLine",
					displayName: @"Radio Data System",
					group: "Gadgets",
					type: "DevExpress.XtraGauges.Demos.TravelingLine",
					description: @"This is a real-world example of using the digital gauge type. The gauge displays the creeping line. In this demo, you can specify the text, its speed and direction.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "WorldTime",
					displayName: @"World Time",
					group: "Gadgets",
					type: "DevExpress.XtraGauges.Demos.WorldTime",
					description: @"In this demo, gauges are used to show the current time in certain cities around the world. Each gauge contains three labels displaying a city name, country and an average currency exchange rate against US dollar. NOTE: This demo doesn't use real data.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "SysInfo",
					displayName: @"System Info Dashboard",
					group: "Gadgets",
					type: "DevExpress.XtraGauges.Demos.SysInfo",
					description: @"In this sample, gauges are used to create a dashboard indicating system resources. You can print this gauge via the main menu.",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 6,
					isFeatured: true
				)
			};
		}
	}
}
