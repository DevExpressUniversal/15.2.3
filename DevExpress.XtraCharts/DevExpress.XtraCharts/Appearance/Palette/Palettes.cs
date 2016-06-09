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

using DevExpress.XtraCharts.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
namespace DevExpress.XtraCharts {
	public static class Palettes {
		#region Nested Classes
		class PaletteInfo {
			Palette palette;
			public ChartStringId Id { get; private set; }
			public string FileName { get; private set; }
			public PaletteInfo(ChartStringId id, string fileName) {
				Id = id;
				FileName = fileName;
			}
			public virtual Palette GetPalette() {
				if (palette == null)
					palette = Palette.CreatePredefinedPalette(Id, resourcePrefix + FileName);
				return palette;
			}
		}
		class DefaultPaletteInfo : PaletteInfo {
			public DefaultPaletteInfo() : base(ChartStringId.PltDefault, string.Empty) { }
			public override Palette GetPalette() {
				return Default;
			}
		}
		#endregion
		internal const string resourcePrefix = "DevExpress.XtraCharts.Appearance.Palettes.";
		public static DefaultPalette Default = new DefaultPalette();
		static Dictionary<string, PaletteInfo> paletts = new Dictionary<string, PaletteInfo>() {
			{ "Default", new DefaultPaletteInfo() },
			{ "Apex", new PaletteInfo(ChartStringId.PltApex, "Apex.xml") },
			{ "Aspect", new PaletteInfo(ChartStringId.PltAspect, "Aspect.xml") },
			{ "Black And White", new PaletteInfo(ChartStringId.PltBlackAndWhite, "BlackAndWhite.xml") },
			{ "Blue", new PaletteInfo(ChartStringId.PltBlue, "Blue.xml") },
			{ "Blue Green", new PaletteInfo(ChartStringId.PltBlueGreen, "BlueGreen.xml") },
			{ "Blue Warm", new PaletteInfo(ChartStringId.PltBlueWarm, "BlueWarm.xml") },
			{ "Chameleon", new PaletteInfo(ChartStringId.PltChameleon, "Chameleon.xml") },
			{ "Civic", new PaletteInfo(ChartStringId.PltCivic, "Civic.xml") },
			{ "Concourse", new PaletteInfo(ChartStringId.PltConcourse, "Concourse.xml") },
			{ "Equity", new PaletteInfo(ChartStringId.PltEquity, "Equity.xml") },
			{ "Flow", new PaletteInfo(ChartStringId.PltFlow, "Flow.xml") },
			{ "Foundry", new PaletteInfo(ChartStringId.PltFoundry, "Foundry.xml") },
			{ "Grayscale", new PaletteInfo(ChartStringId.PltGrayscale, "Grayscale.xml") },
			{ "Green", new PaletteInfo(ChartStringId.PltGreen, "Green.xml") },
			{ "Green Yellow", new PaletteInfo(ChartStringId.PltGreenYellow, "GreenYellow.xml") },
			{ "In A Fog", new PaletteInfo(ChartStringId.PltInAFog, "InAFog.xml") },
			{ "Marquee", new PaletteInfo(ChartStringId.PltMarquee, "Marquee.xml") },
			{ "Median", new PaletteInfo(ChartStringId.PltMedian, "Median.xml") },
			{ "Metro", new PaletteInfo(ChartStringId.PltMetro, "Metro.xml") },
			{ "Mixed", new PaletteInfo(ChartStringId.PltMixed, "Mixed.xml") },
			{ "Module", new PaletteInfo(ChartStringId.PltModule, "Module.xml") },
			{ "Nature Colors", new PaletteInfo(ChartStringId.PltNatureColors, "NatureColors.xml") },
			{ "Northern Lights", new PaletteInfo(ChartStringId.PltNorthernLights, "NorthernLights.xml") },
			{ "Office", new PaletteInfo(ChartStringId.PltOffice, "Office.xml") },
			{ "Office 2013", new PaletteInfo(ChartStringId.PltOffice2013, "Office2013.xml") },
			{ "Opulent", new PaletteInfo(ChartStringId.PltOpulent, "Opulent.xml") },
			{ "Orange", new PaletteInfo(ChartStringId.PltOrange, "Orange.xml") },
			{ "Orange Red", new PaletteInfo(ChartStringId.PltOrangeRed, "OrangeRed.xml") },
			{ "Oriel", new PaletteInfo(ChartStringId.PltOriel, "Oriel.xml") },
			{ "Origin", new PaletteInfo(ChartStringId.PltOrigin, "Origin.xml") },
			{ "Paper", new PaletteInfo(ChartStringId.PltPaper, "Paper.xml") },
			{ "Pastel Kit", new PaletteInfo(ChartStringId.PltPastelKit, "PastelKit.xml") },
			{ "Red", new PaletteInfo(ChartStringId.PltRed, "Red.xml") },
			{ "Red Orange", new PaletteInfo(ChartStringId.PltRedOrange, "RedOrange.xml") },
			{ "Red Violet", new PaletteInfo(ChartStringId.PltRedViolet, "RedViolet.xml") },
			{ "Slipstream", new PaletteInfo(ChartStringId.PltSlipstream, "Slipstream.xml") },
			{ "Solstice", new PaletteInfo(ChartStringId.PltSolstice, "Solstice.xml") },
			{ "Technic", new PaletteInfo(ChartStringId.PltTechnic, "Technic.xml") },
			{ "Terracotta Pie", new PaletteInfo(ChartStringId.PltTerracottaPie, "TerracottaPie.xml") },
			{ "The Trees", new PaletteInfo(ChartStringId.PltTheTrees, "TheTrees.xml") },
			{ "Trek", new PaletteInfo(ChartStringId.PltTrek, "Trek.xml") },
			{ "Urban", new PaletteInfo(ChartStringId.PltUrban, "Urban.xml") },
			{ "Verve", new PaletteInfo(ChartStringId.PltVerve, "Verve.xml") },
			{ "Violet", new PaletteInfo(ChartStringId.PltViolet, "Violet.xml") },
			{ "Violet II", new PaletteInfo(ChartStringId.PltVioletII, "VioletII.xml") },
			{ "Yellow", new PaletteInfo(ChartStringId.PltYellow, "Yellow.xml") },
			{ "Yellow Orange", new PaletteInfo(ChartStringId.PltYellowOrange, "YellowOrange.xml") }
		};
		public static Palette Apex { get { return paletts["Apex"].GetPalette(); } }
		public static Palette Aspect { get { return paletts["Aspect"].GetPalette(); } }
		public static Palette BlackAndWhite { get { return paletts["Black And White"].GetPalette(); } }
		public static Palette Blue { get { return paletts["Blue"].GetPalette(); } }
		public static Palette BlueGreen { get { return paletts["Blue Green"].GetPalette(); } }
		public static Palette BlueWarm { get { return paletts["Blue Warm"].GetPalette(); } }
		public static Palette Chameleon { get { return paletts["Chameleon"].GetPalette(); } }
		public static Palette Civic { get { return paletts["Civic"].GetPalette(); } }
		public static Palette Concourse { get { return paletts["Concourse"].GetPalette(); } }
		public static Palette Equity { get { return paletts["Equity"].GetPalette(); } }
		public static Palette Flow { get { return paletts["Flow"].GetPalette(); } }
		public static Palette Foundry { get { return paletts["Foundry"].GetPalette(); } }
		public static Palette Grayscale { get { return paletts["Grayscale"].GetPalette(); } }
		public static Palette Green { get { return paletts["Green"].GetPalette(); } }
		public static Palette GreenYellow { get { return paletts["Green Yellow"].GetPalette(); } }
		public static Palette InAFog { get { return paletts["In A Fog"].GetPalette(); } }
		public static Palette Marquee { get { return paletts["Marquee"].GetPalette(); } }
		public static Palette Median { get { return paletts["Median"].GetPalette(); } }
		public static Palette Metro { get { return paletts["Metro"].GetPalette(); } }
		public static Palette Mixed { get { return paletts["Mixed"].GetPalette(); } }
		public static Palette Module { get { return paletts["Module"].GetPalette(); } }
		public static Palette NatureColors { get { return paletts["Nature Colors"].GetPalette(); } }
		public static Palette NorthernLights { get { return paletts["Northern Lights"].GetPalette(); } }
		public static Palette Office { get { return paletts["Office"].GetPalette(); } }
		public static Palette Office2013 { get { return paletts["Office 2013"].GetPalette(); } }
		public static Palette Opulent { get { return paletts["Opulent"].GetPalette(); } }
		public static Palette Orange { get { return paletts["Orange"].GetPalette(); } }
		public static Palette OrangeRed { get { return paletts["Orange Red"].GetPalette(); } }
		public static Palette Oriel { get { return paletts["Oriel"].GetPalette(); } }
		public static Palette Origin { get { return paletts["Origin"].GetPalette(); } }
		public static Palette Paper { get { return paletts["Paper"].GetPalette(); } }
		public static Palette PastelKit { get { return paletts["Pastel Kit"].GetPalette(); } }
		public static Palette Red { get { return paletts["Red"].GetPalette(); } }
		public static Palette RedOrange { get { return paletts["Red Orange"].GetPalette(); } }
		public static Palette RedViolet { get { return paletts["Red Violet"].GetPalette(); } }
		public static Palette Slipstream { get { return paletts["Slipstream"].GetPalette(); } }
		public static Palette Solstice { get { return paletts["Solstice"].GetPalette(); } }
		public static Palette Technic { get { return paletts["Technic"].GetPalette(); } }
		public static Palette TerracottaPie { get { return paletts["Terracotta Pie"].GetPalette(); } }
		public static Palette TheTrees { get { return paletts["The Trees"].GetPalette(); } }
		public static Palette Trek { get { return paletts["Trek"].GetPalette(); } }
		public static Palette Urban { get { return paletts["Urban"].GetPalette(); } }
		public static Palette Verve { get { return paletts["Verve"].GetPalette(); } }
		public static Palette Violet { get { return paletts["Violet"].GetPalette(); } }
		public static Palette VioletII { get { return paletts["Violet II"].GetPalette(); } }
		public static Palette Yellow { get { return paletts["Yellow"].GetPalette(); } }
		public static Palette YellowOrange { get { return paletts["Yellow Orange"].GetPalette(); } }
		public static string[] GetNames() {
			List<string> values = new List<string>();
			foreach (PaletteInfo info in paletts.Values)
				values.Add(ChartLocalizer.GetString(info.Id));
			return values.ToArray();
		}
		public static Palette GetPalette(string name) {
			PaletteInfo paletteInfo;
			if (paletts.TryGetValue(name, out paletteInfo))
				return paletteInfo.GetPalette();
			foreach (PaletteInfo info in paletts.Values) {
				if (ChartLocalizer.GetString(info.Id) == name || ChartLocalizer.GetDefaultString(info.Id) == name)
					return info.GetPalette();
			}
			return null;
		}
	}
}
