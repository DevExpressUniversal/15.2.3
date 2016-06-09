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
		static List<Module> Create_DXGauges_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				new WpfModule(demo,
					name: "CarDashboard",
					displayName: @"Car Dashboard",
					group: "Dashboards",
					type: "GaugesDemo.CarDashboard",
					shortDescription: @"This is a typical car dashboard, created with DXGauges controls.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to create a typical car dashboard using controls shipped with the DXGauges Suite. This dashboard consists of the following elements:
                        </Paragraph>
                        <Paragraph>
                        <InlineUIContainer> <StackPanel Margin=""30,0,0,0""> <TextBlock>- left circular gauge that shows the current car speed;</TextBlock> <TextBlock>- right circular gauge that displays the current rate of rotation;</TextBlock> <TextBlock TextWrapping=""Wrap"">- two half-circular gauges in the center that represent the current engine temperature and fuel rate.</TextBlock> </StackPanel> </InlineUIContainer> <LineBreak/>
                        </Paragraph>
                        <Paragraph>
                        In this demo, you can press the Accelerate button and hold it to see how different gauges emulate real-life car movement. To slow down the car, click the Brake button and continue to hold it until the car stops.
                        </Paragraph>",
					allowSwitchingThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "CircularScales",
					displayName: @"Circular Scales",
					group: "Circular Gauge",
					type: "GaugesDemo.CircularScales",
					shortDescription: @"This demo illustrates three Circular scales showing current time at different locations.",
					description: @"
                        <Paragraph>
                        This demo shows three Circular scales. These scales use the built-in RedClock model (to look like real-life clocks) and show the current local time in three different locations over the globe: New York, London and Moscow.
                        </Paragraph>
                        <Paragraph>
                        In this demo you can show or hide clock labels, use a custom template for them (to display Roman numerals) and show or hide major or minor tickmarks.
                        </Paragraph>
                        <Paragraph>
                        This demo also demonstrates the capability used to add any custom element into the Circular scale. In this demo, a text (containing a city name) and an image (displaying a city icon) were added to each scale as custom elements.
                        </Paragraph>",
					allowSwitchingThemes: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "CircularIndicators",
					displayName: @"Circular Indicators",
					group: "Circular Gauge",
					type: "GaugesDemo.CircularIndicators",
					shortDescription: @"A Circular scale can contain such indicators as needles, markers and range bars.",
					description: @"
                        <Paragraph>
                        This demo illustrates three different value indicators that can be displayed on a Circular scale: a needle (which looks like an airplane in this demo), a marker (a small white spot moving from left to right) and a range bar (a blue strip that indicates how far the marker is from its central position).
                        </Paragraph>
                        <Paragraph>
                        Using this demo's options, you can enable or disable animation for each value indicator.
                        </Paragraph>",
					allowSwitchingThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "CircularRanges",
					displayName: @"Circular Ranges",
					group: "Circular Gauge",
					type: "GaugesDemo.CircularRanges",
					shortDescription: @"A Circular scale can be split into multiple ranges and trigger events for them.",
					description: @"
                        <Paragraph>
                        This demo illustrates a typical weather gauge with multiple ranges used in it. For example, two ranges in the center (blue and yellow) are intended to indicate negative and positive temperature values; and three ranges on a peripheral scale (green-yellow-red) illustrate whether the current pressure is low or high.
                        </Paragraph>
                        <Paragraph>
                        On the options pane of this demo, there are several track bars that allow you to modify current pressure and temperature values, as well as change the start and end limits of pressure range bars and see what effect all these actions produce for a weather gauge.
                        </Paragraph>
                        <Paragraph>
                        This demo uses the State Indicator control introduced in v2011 vol 2. This control is placed into the scale center and displays an icon corresponding to the current weather state. This is done thanks to the IndicatorEnter and IndicatorLeave events, provided by the RangeBase class.
                        </Paragraph>",
					allowSwitchingThemes: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "CircularModels",
					displayName: @"Circular Models",
					group: "Circular Gauge",
					type: "GaugesDemo.CircularModels",
					shortDescription: @"In this demo you can see all Circular models included into the DXGauges Suite.",
					description: @"
                        <Paragraph>
                        This demo shows a typical Circular gauge that constantly changes values of its indicators (a needle, a marker and a range bar). This is useful for seeing how different gauge elements look using different appearance settings.
                        </Paragraph>
                        <Paragraph>
                        To change the current model to another one, use the list displayed on the Options pane of this demo.
                        </Paragraph>",
					allowSwitchingThemes: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "LinearScales",
					displayName: @"Linear Scales",
					group: "Linear Gauge",
					type: "GaugesDemo.LinearScales",
					shortDescription: @"This demo illustrates multiple Linear scales emulating an equalizer.",
					description: @"
                        <Paragraph>
                        The gauge in this demo shows 10 linear scales with vertical orientation to emulate a real-life equalizer.
                        </Paragraph>
                        <Paragraph>
                        In this demo you can show or hide equalizer labels and show or hide major or minor tickmarks.
                        </Paragraph>",
					allowSwitchingThemes: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "LinearIndicators",
					displayName: @"Linear Indicators",
					group: "Linear Gauge",
					type: "GaugesDemo.LinearIndicators",
					shortDescription: @"A Linear scale can contain such indicators as level bars, markers and range bars.",
					description: @"
                        <Paragraph>
                        This demo shows a statistical report with five different measured quantities. Each measured quantity has its own value, which is represented by corresponding value indicators.
                        </Paragraph>
                        <Paragraph>
                        Range Bars are used in this demo by default, but you can replace them with either Markers or Level Bars using the Options pane on the left. Also you can choose which values to display on labels: absolute or percents.
                        </Paragraph>",
					allowSwitchingThemes: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "LinearRanges",
					displayName: @"Linear Ranges",
					group: "Linear Gauge",
					type: "GaugesDemo.LinearRanges",
					shortDescription: @"A Linear scale can be split into multiple ranges and trigger events for them.",
					description: @"
                        <Paragraph>
                        This demo illustrates a typical weather gauge with multiple ranges used in it. For example, two ranges on the left (blue and yellow) are intended to indicate negative and positive temperature values; and three ranges on the right (green-yellow-red) illustrate whether the current pressure is low or high.
                        </Paragraph>
                        <Paragraph>
                        On the options pane of this demo, there are several track bars that allow you to modify current pressure and temperature values, as well as change the start and end limits of pressure range bars and see what effect all these actions produce on a weather gauge.
                        </Paragraph>
                        <Paragraph>
                        This demo uses the State Indicator control introduced in v2011 vol 2. This control is placed into the scale center and displays an icon corresponding to the current weather state. This is done thanks to the IndicatorEnter and IndicatorLeave events, provided by the RangeBase class.
                        </Paragraph>",
					allowSwitchingThemes: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "LinearModels",
					displayName: @"Linear Models",
					group: "Linear Gauge",
					type: "GaugesDemo.LinearModels",
					shortDescription: @"In this demo you can see all Linear models included into the DXGauges Suite.",
					description: @"
                        <Paragraph>
                        This demo shows two typical Linear gauges that constantly change their values. This is useful for seeing how different gauge elements look using different appearance settings.
                        </Paragraph>
                        <Paragraph>
                        To change the current model to another one, use the list displayed on the Options pane of this demo.
                        </Paragraph>",
					allowSwitchingThemes: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "ViewTypes",
					displayName: @"View Types",
					group: "Digital Gauge",
					type: "GaugesDemo.ViewTypes",
					shortDescription: @"In this demo you can see all Digital view types included into the DXGauges Suite.",
					description: @"
                        <Paragraph>
                        This demo shows different views supported by the Digital Gauge control. These views include 7 Segments, 14 Segments, 5x8 Matrix and 8x14 Matrix types - all of which make a digital gauge look like real LED devices.
                        </Paragraph>
                        <Paragraph>
                        Note that the Digital Gauge control supports the animation of its symbols. The current demo illustrates a Blinking animation type.
                        </Paragraph>",
					allowSwitchingThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "DigitalModels",
					displayName: @"Digital Models",
					group: "Digital Gauge",
					type: "GaugesDemo.DigitalModels",
					shortDescription: @"In this demo you can see all Digital models included into the DXGauges Suite.",
					description: @"
                        <Paragraph>
                        This demo illustrates two typical Digital gauges, each of which contains a text moving from right to left. This is done thanks to the Creeping Line built-in animation, and is useful for seeing how different gauge elements look using different appearance settings.
                        </Paragraph>
                        <Paragraph>
                        To change the current model to another one, use the list displayed on the Options pane of this demo.
                        </Paragraph>",
					allowSwitchingThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "CustomSymbolMapping",
					displayName: @"Custom Symbol Mapping",
					group: "Digital Gauge",
					type: "GaugesDemo.CustomSymbolMapping",
					shortDescription: @"The Digital Gauge control can display any custom symbol thanks to the Symbol Mapping feature.",
					description: @"
                        <Paragraph>
                        This demo shows different types of traffic lights as an example of Custom Symbol Mapping, which allows you to display custom symbols (e.g. characters or images). To do this, it is necessary to specify which segments or matrix cells should be used to represent each custom symbol.
                        </Paragraph>",
					allowSwitchingThemes: false,
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "StateIndicatorModels",
					displayName: @"State Indicator Models",
					group: "State Indicator",
					type: "GaugesDemo.StateIndicatorModels",
					shortDescription: @"In this demo you can see all StateIndicator models included into the DXGauges Suite.",
					description: @"
                        <Paragraph>
                        This demo illustrates StateIndicator models that are available in the DXGauges Suite. Each model contains a set of predefined images representing different states of the same object (e.g. lamp, arrow, etc.).
                        </Paragraph>
                        <Paragraph>
                        To see all states provided by each model, switch them using the slider.
                        </Paragraph>",
					allowSwitchingThemes: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "IntegrationWithAnalogGauges",
					displayName: @"Integration with Analog Gauges",
					group: "State Indicator",
					type: "GaugesDemo.IntegrationWithAnalogGauges",
					shortDescription: @"This demo illustrates integration of State Indicators with Circular and Linear Gauges.",
					description: @"
                        <Paragraph>
                        In this demo you can see how to integrate State Indicators with analog gauges (Circular and Linear) that are shipped with the DXGauges Suite. This is done via the AnalogGaugeControl.ValueIndicator attached property, which allows you to bind a State Indicator to any value indicator (e.g., to a needle of a Circular gauge).
                        </Paragraph>
                        <Paragraph>
                        For example, three ranges (green-yellow-red) on the left are used together with a state indicator to show whether the pressure is low, average or high; and three ranges (green-yellow-red) with a state indicator on the right do the same with current temperature values.
                        </Paragraph>",
					allowSwitchingThemes: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "IndicatorAnimation",
					displayName: @"Indicator Animation",
					group: "Features",
					type: "GaugesDemo.IndicatorAnimation",
					shortDescription: @"All indicators in DXGauges can animate changing of their values.",
					description: @"
                        <Paragraph>
                        This demo illustrates three circular gauges with needles that constantly change their values. When a needle is moving from the current value to a new one, it uses the Easing function provided by the currently enabled animation. You can switch the current Easing function to another one using the Options pane of this demo.
                        </Paragraph>
                        <Paragraph>
                        You can also change animation duration among demo options and see how needles' animation is changed.
                        </Paragraph>",
					allowSwitchingThemes: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "SymbolAnimation",
					displayName: @"Symbol Animation",
					group: "Features",
					type: "GaugesDemo.SymbolAnimation",
					shortDescription: @"This demo illustrates animation types, supported by the Digital Gauge control.",
					description: @"
                        <Paragraph>
                        The Digital Gauge control provides two built-in animation effects:
                        </Paragraph>
                        <Paragraph>
                        <Bold>•</Bold> A <Bold>Creeping Line</Bold> animation that is useful for showing text moving from right to left or vice versa. You can see this animation on the upper panel of a car audio receiver, which imitates a LED display that shows the current radio station.
                        </Paragraph>
                        <Paragraph>
                        Note that you can change the animation direction on the options pane of this demo.
                        </Paragraph>
                        <Paragraph>
                        <Bold>•</Bold> A <Bold>Blinking</Bold> animation that is useful for attracting attention to some text. You can see this animation on the lower panel of a car audio receiver, which imitates a digital clock.
                        </Paragraph>
                        <Paragraph>
                        To change the receiver mode from CD to Radio, and vice versa, use the ""SRC"" button on the receiver panel.
                        </Paragraph>
                        <Paragraph>
                        When the CD mode is chosen, the blinking animation starts to imitate the reading process showing the ""READING"" text on the panel. Then, the playing process is demonstrated on the panel using the creeping line animation.
                        </Paragraph>
                        <Paragraph>
                        In the CD mode, you can change the music composition using the  ""<"", "">"", ""<<"", "">>"" buttons on the receiver panel.
                        </Paragraph>",
					allowSwitchingThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "Interactivity",
					displayName: @"Interactivity",
					group: "Features",
					type: "GaugesDemo.Interactivity",
					shortDescription: @"Knob-like gauges provide realistic interactivity for your end-users.",
					description: @"
                        <Paragraph>
                        This demo illustrates a digital oscilloscope with 5 independent knob-like gauges. You can use the mouse pointer to click any knob and move its current value to another position to see how this changes the oscilloscope's output.
                        </Paragraph>",
					allowSwitchingThemes: false,
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				)
			};
		}
	}
}
