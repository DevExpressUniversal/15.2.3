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

using System.Reflection;
using DevExpress.Xpf.Core.Design;
using DevExpress.Xpf.Utils;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.PropertyEditing;
namespace DevExpress.Xpf.Gauges.Design {
	internal class RegisterHelper {
		public static void PrepareAttributeTable(AttributeTableBuilder builder) {
			builder.AddCustomAttributes(typeof(CircularGaugeControl), CircularGaugeControl.ModelProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(CircularDefaultModel), typeof(CircularCleanWhiteModel), typeof(CircularCosmicModel), typeof(CircularSmartModel), typeof(CircularRedClockModel),
					typeof(CircularProgressiveModel), typeof(CircularEcoModel), typeof(CircularFutureModel), typeof(CircularClassicModel), typeof(CircularIStyleModel),
					typeof(CircularYellowSubmarineModel), typeof(CircularMagicLightModel), typeof(CircularFlatLightModel), typeof(CircularFlatDarkModel)));
			builder.AddCustomAttributes(typeof(LinearGaugeControl), LinearGaugeControl.ModelProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(LinearDefaultModel), typeof(LinearCleanWhiteModel), typeof(LinearCosmicModel), typeof(LinearSmartModel),
					typeof(LinearProgressiveModel), typeof(LinearEcoModel), typeof(LinearFutureModel), typeof(LinearClassicModel), typeof(LinearIStyleModel),
					typeof(LinearYellowSubmarineModel), typeof(LinearMagicLightModel), typeof(LinearRedThermometerModel), typeof(LinearFlatLightModel), typeof(LinearFlatDarkModel)));
			builder.AddCustomAttributes(typeof(Scale), ArcScale.TickmarksPresentationProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(DefaultTickmarksPresentation), typeof(CleanWhiteTickmarksPresentation), typeof(CosmicTickmarksPresentation),
					typeof(SmartTickmarksPresentation), typeof(ProgressiveTickmarksPresentation), typeof(EcoTickmarksPresentation),
					typeof(FutureTickmarksPresentation), typeof(ClassicTickmarksPresentation), typeof(IStyleTickmarksPresentation),
					typeof(YellowSubmarineTickmarksPresentation), typeof(MagicLightTickmarksPresentation), typeof(FlatLightTickmarksPresentation),
					typeof(FlatDarkTickmarksPresentation), typeof(CustomTickmarksPresentation)));
			builder.AddCustomAttributes(typeof(Scale), ArcScale.LabelPresentationProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(DefaultScaleLabelPresentation), typeof(CleanWhiteScaleLabelPresentation), typeof(CosmicScaleLabelPresentation),
					typeof(SmartScaleLabelPresentation), typeof(ProgressiveScaleLabelPresentation), typeof(EcoScaleLabelPresentation),
					typeof(FutureScaleLabelPresentation), typeof(ClassicScaleLabelPresentation), typeof(IStyleScaleLabelPresentation),
					typeof(YellowSubmarineScaleLabelPresentation), typeof(MagicLightScaleLabelPresentation), typeof(FlatLightScaleLabelPresentation),
					typeof(FlatDarkScaleLabelPresentation), typeof(CustomScaleLabelPresentation)));
			builder.AddCustomAttributes(typeof(ArcScale), ArcScale.SpindleCapPresentationProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(DefaultSpindleCapPresentation), typeof(CleanWhiteSpindleCapPresentation), typeof(CosmicSpindleCapPresentation),
					typeof(SmartSpindleCapPresentation), typeof(RedClockSpindleCapPresentation), typeof(ProgressiveSpindleCapPresentation),
					typeof(EcoSpindleCapPresentation), typeof(FutureSpindleCapPresentation), typeof(ClassicSpindleCapPresentation),
					typeof(IStyleSpindleCapPresentation), typeof(YellowSubmarineSpindleCapPresentation), typeof(MagicLightSpindleCapPresentation),
					typeof(FlatLightSpindleCapPresentation), typeof(FlatDarkSpindleCapPresentation), typeof(CustomSpindleCapPresentation)));
			builder.AddCustomAttributes(typeof(ArcScale), ArcScale.LinePresentationProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(DefaultArcScaleLinePresentation), typeof(CleanWhiteArcScaleLinePresentation), typeof(CosmicArcScaleLinePresentation),
					typeof(SmartArcScaleLinePresentation), typeof(ProgressiveArcScaleLinePresentation), typeof(EcoArcScaleLinePresentation),
					typeof(ClassicArcScaleLinePresentation), typeof(IStyleArcScaleLinePresentation), typeof(FlatLightArcScaleLinePresentation),
					typeof(FlatDarkArcScaleLinePresentation), typeof(CustomArcScaleLinePresentation)));
			builder.AddCustomAttributes(typeof(LinearScale), LinearScale.LinePresentationProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(DefaultLinearScaleLinePresentation), typeof(CleanWhiteLinearScaleLinePresentation), typeof(CosmicLinearScaleLinePresentation),
					typeof(SmartLinearScaleLinePresentation), typeof(ProgressiveLinearScaleLinePresentation), typeof(EcoLinearScaleLinePresentation),
					typeof(FutureLinearScaleLinePresentation), typeof(ClassicLinearScaleLinePresentation), typeof(IStyleLinearScaleLinePresentation),
					typeof(MagicLightLinearScaleLinePresentation), typeof(RedThermometerLinearScaleLinePresentation), typeof(FlatLightLinearScaleLinePresentation),
					typeof(FlatDarkLinearScaleLinePresentation), typeof(CustomLinearScaleLinePresentation)));
			builder.AddCustomAttributes(typeof(ArcScaleNeedle), ArcScaleNeedle.PresentationProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(DefaultArcScaleNeedlePresentation), typeof(CleanWhiteArcScaleNeedlePresentation), typeof(CosmicArcScaleNeedlePresentation),
					typeof(SmartArcScaleNeedlePresentation), typeof(RedClockSecondArcScaleNeedlePresentation), typeof(ProgressiveArcScaleNeedlePresentation),
					typeof(EcoArcScaleNeedlePresentation), typeof(FutureArcScaleNeedlePresentation), typeof(ClassicArcScaleNeedlePresentation),
					typeof(IStyleArcScaleNeedlePresentation), typeof(YellowSubmarineArcScaleNeedlePresentation), typeof(MagicLightArcScaleNeedlePresentation),
					typeof(FlatLightArcScaleNeedlePresentation), typeof(FlatDarkArcScaleNeedlePresentation), typeof(CustomArcScaleNeedlePresentation)));
			builder.AddCustomAttributes(typeof(ArcScaleMarker), ArcScaleMarker.PresentationProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(DefaultArcScaleMarkerPresentation), typeof(CleanWhiteArcScaleMarkerPresentation), typeof(CosmicArcScaleMarkerPresentation),
					typeof(SmartArcScaleMarkerPresentation), typeof(ProgressiveArcScaleMarkerPresentation), typeof(EcoArcScaleMarkerPresentation),
					typeof(FutureArcScaleMarkerPresentation), typeof(ClassicArcScaleMarkerPresentation), typeof(IStyleArcScaleMarkerPresentation),
					typeof(YellowSubmarineArcScaleMarkerPresentation), typeof(MagicLightArcScaleMarkerPresentation), typeof(FlatLightArcScaleMarkerPresentation),
					typeof(FlatDarkArcScaleMarkerPresentation), typeof(CustomArcScaleMarkerPresentation)));
			builder.AddCustomAttributes(typeof(LinearScaleMarker), LinearScaleMarker.PresentationProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(DefaultLinearScaleMarkerPresentation), typeof(CleanWhiteLinearScaleMarkerPresentation), typeof(CosmicLinearScaleMarkerPresentation),
					typeof(SmartLinearScaleMarkerPresentation), typeof(ProgressiveLinearScaleMarkerPresentation), typeof(EcoLinearScaleMarkerPresentation),
					typeof(FutureLinearScaleMarkerPresentation), typeof(ClassicLinearScaleMarkerPresentation), typeof(IStyleLinearScaleMarkerPresentation),
					typeof(YellowSubmarineLinearScaleMarkerPresentation), typeof(MagicLightLinearScaleMarkerPresentation), typeof(RedThermometerLinearScaleMarkerPresentation),
					typeof(FlatLightLinearScaleMarkerPresentation), typeof(FlatDarkLinearScaleMarkerPresentation), typeof(CustomLinearScaleMarkerPresentation)));
			builder.AddCustomAttributes(typeof(ArcScaleRangeBar), ArcScaleRangeBar.PresentationProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(DefaultArcScaleRangeBarPresentation), typeof(CustomArcScaleRangeBarPresentation)));
			builder.AddCustomAttributes(typeof(LinearScaleRangeBar), LinearScaleRangeBar.PresentationProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(DefaultLinearScaleRangeBarPresentation), typeof(CustomLinearScaleRangeBarPresentation)));
			builder.AddCustomAttributes(typeof(CircularGaugeLayer), CircularGaugeLayer.PresentationProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(DefaultCircularGaugeBackgroundLayerPresentation), typeof(CleanWhiteCircularGaugeBackgroundLayerPresentation),
					typeof(CosmicCircularGaugeBackgroundLayerPresentation), typeof(SmartCircularGaugeBackgroundLayerPresentation),
					typeof(ProgressiveCircularGaugeBackgroundLayerPresentation), typeof(EcoCircularGaugeBackgroundLayerPresentation),
					typeof(FutureCircularGaugeBackgroundLayerPresentation), typeof(ClassicCircularGaugeBackgroundLayerPresentation),
					typeof(IStyleCircularGaugeBackgroundLayerPresentation), typeof(YellowSubmarineCircularGaugeBackgroundLayerPresentation),
					typeof(MagicLightCircularGaugeBackgroundLayerPresentation), typeof(RedClockCircularGaugeBackgroundLayerPresentation),
					typeof(FlatLightCircularGaugeBackgroundLayerPresentation), typeof(FlatDarkCircularGaugeBackgroundLayerPresentation),
					typeof(CustomCircularGaugeLayerPresentation)));
			builder.AddCustomAttributes(typeof(LinearGaugeLayer), LinearGaugeLayer.PresentationProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(DefaultLinearGaugeBackgroundLayerPresentation), typeof(CleanWhiteLinearGaugeBackgroundLayerPresentation),
					typeof(CosmicLinearGaugeBackgroundLayerPresentation), typeof(SmartLinearGaugeBackgroundLayerPresentation),
					typeof(ProgressiveLinearGaugeBackgroundLayerPresentation), typeof(EcoLinearGaugeBackgroundLayerPresentation),
					typeof(FutureLinearGaugeBackgroundLayerPresentation), typeof(ClassicLinearGaugeBackgroundLayerPresentation),
					typeof(IStyleLinearGaugeBackgroundLayerPresentation), typeof(YellowSubmarineLinearGaugeBackgroundLayerPresentation),
					typeof(MagicLightLinearGaugeBackgroundLayerPresentation), typeof(RedThermometerLinearGaugeBackgroundLayerPresentation),
					typeof(FlatLightLinearGaugeBackgroundLayerPresentation), typeof(FlatDarkLinearGaugeBackgroundLayerPresentation),
					typeof(CustomLinearGaugeLayerPresentation)));
			builder.AddCustomAttributes(typeof(ArcScaleLayer), ArcScaleLayer.PresentationProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(DefaultArcScaleBackgroundLayerPresentation), typeof(DefaultHalfTopArcScaleBackgroundLayerPresentation),
					typeof(DefaultQuarterTopLeftArcScaleBackgroundLayerPresentation), typeof(DefaultQuarterTopRightArcScaleBackgroundLayerPresentation),
					typeof(DefaultThreeQuartersArcScaleBackgroundLayerPresentation),
					typeof(CleanWhiteArcScaleBackgroundLayerPresentation), typeof(CleanWhiteHalfTopArcScaleBackgroundLayerPresentation),
					typeof(CleanWhiteQuarterTopLeftArcScaleBackgroundLayerPresentation), typeof(CleanWhiteQuarterTopRightArcScaleBackgroundLayerPresentation),
					typeof(CleanWhiteThreeQuartersArcScaleBackgroundLayerPresentation),
					typeof(CosmicArcScaleBackgroundLayerPresentation), typeof(CosmicHalfTopArcScaleBackgroundLayerPresentation),
					typeof(CosmicQuarterTopLeftArcScaleBackgroundLayerPresentation), typeof(CosmicQuarterTopRightArcScaleBackgroundLayerPresentation),
					typeof(CosmicThreeQuartersArcScaleBackgroundLayerPresentation),
					typeof(SmartArcScaleBackgroundLayerPresentation), typeof(SmartHalfTopArcScaleBackgroundLayerPresentation),
					typeof(SmartQuarterTopLeftArcScaleBackgroundLayerPresentation), typeof(SmartQuarterTopRightArcScaleBackgroundLayerPresentation),
					typeof(SmartThreeQuartersArcScaleBackgroundLayerPresentation),
					typeof(RedClockArcScaleBackgroundLayerPresentation), typeof(RedClockHalfTopArcScaleBackgroundLayerPresentation),
					typeof(RedClockQuarterTopLeftArcScaleBackgroundLayerPresentation), typeof(RedClockQuarterTopRightArcScaleBackgroundLayerPresentation),
					typeof(RedClockThreeQuartersArcScaleBackgroundLayerPresentation),
					typeof(ProgressiveArcScaleBackgroundLayerPresentation), typeof(ProgressiveHalfTopArcScaleBackgroundLayerPresentation),
					typeof(ProgressiveQuarterTopLeftArcScaleBackgroundLayerPresentation), typeof(ProgressiveQuarterTopRightArcScaleBackgroundLayerPresentation),
					typeof(ProgressiveThreeQuartersArcScaleBackgroundLayerPresentation),
					typeof(EcoArcScaleBackgroundLayerPresentation), typeof(EcoHalfTopArcScaleBackgroundLayerPresentation),
					typeof(EcoQuarterTopLeftArcScaleBackgroundLayerPresentation), typeof(EcoQuarterTopRightArcScaleBackgroundLayerPresentation),
					typeof(EcoThreeQuartersArcScaleBackgroundLayerPresentation),
					typeof(FutureArcScaleBackgroundLayerPresentation), typeof(FutureHalfTopArcScaleBackgroundLayerPresentation),
					typeof(FutureQuarterTopLeftArcScaleBackgroundLayerPresentation), typeof(FutureQuarterTopRightArcScaleBackgroundLayerPresentation),
					typeof(FutureThreeQuartersArcScaleBackgroundLayerPresentation),
					typeof(ClassicArcScaleBackgroundLayerPresentation), typeof(ClassicHalfTopArcScaleBackgroundLayerPresentation),
					typeof(ClassicQuarterTopLeftArcScaleBackgroundLayerPresentation), typeof(ClassicQuarterTopRightArcScaleBackgroundLayerPresentation),
					typeof(ClassicThreeQuartersArcScaleBackgroundLayerPresentation),
					typeof(IStyleArcScaleBackgroundLayerPresentation), typeof(IStyleHalfTopArcScaleBackgroundLayerPresentation),
					typeof(IStyleQuarterTopLeftArcScaleBackgroundLayerPresentation), typeof(IStyleQuarterTopRightArcScaleBackgroundLayerPresentation),
					typeof(IStyleThreeQuartersArcScaleBackgroundLayerPresentation),
					typeof(YellowSubmarineArcScaleBackgroundLayerPresentation), typeof(YellowSubmarineHalfTopArcScaleBackgroundLayerPresentation),
					typeof(YellowSubmarineQuarterTopLeftArcScaleBackgroundLayerPresentation), typeof(YellowSubmarineQuarterTopRightArcScaleBackgroundLayerPresentation),
					typeof(YellowSubmarineThreeQuartersArcScaleBackgroundLayerPresentation),
					typeof(ProgressiveArcScaleForegroundLayerPresentation), typeof(ProgressiveHalfTopArcScaleForegroundLayerPresentation),
					typeof(ProgressiveQuarterTopLeftArcScaleForegroundLayerPresentation), typeof(ProgressiveQuarterTopRightArcScaleForegroundLayerPresentation),
					typeof(ProgressiveThreeQuartersArcScaleForegroundLayerPresentation),
					typeof(FutureArcScaleForegroundLayerPresentation), typeof(FutureHalfTopArcScaleForegroundLayerPresentation),
					typeof(FutureQuarterTopLeftArcScaleForegroundLayerPresentation), typeof(FutureQuarterTopRightArcScaleForegroundLayerPresentation),
					typeof(FutureThreeQuartersArcScaleForegroundLayerPresentation),
					typeof(MagicLightArcScaleForegroundLayerPresentation), typeof(MagicLightHalfTopArcScaleForegroundLayerPresentation),
					typeof(MagicLightQuarterTopLeftArcScaleForegroundLayerPresentation), typeof(MagicLightQuarterTopRightArcScaleForegroundLayerPresentation),
					typeof(MagicLightThreeQuartersArcScaleForegroundLayerPresentation),
					typeof(FlatLightArcScaleBackgroundLayerPresentation), typeof(FlatLightHalfTopArcScaleBackgroundLayerPresentation),
					typeof(FlatLightQuarterTopLeftArcScaleBackgroundLayerPresentation), typeof(FlatLightQuarterTopRightArcScaleBackgroundLayerPresentation),
					typeof(FlatLightThreeQuartersArcScaleBackgroundLayerPresentation),
					typeof(FlatDarkArcScaleBackgroundLayerPresentation), typeof(FlatDarkHalfTopArcScaleBackgroundLayerPresentation),
					typeof(FlatDarkQuarterTopLeftArcScaleBackgroundLayerPresentation), typeof(FlatDarkQuarterTopRightArcScaleBackgroundLayerPresentation),
					typeof(FlatDarkThreeQuartersArcScaleBackgroundLayerPresentation),
					typeof(CustomArcScaleLayerPresentation)));
			builder.AddCustomAttributes(typeof(LinearScaleLayer), LinearScaleLayer.PresentationProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(DefaultLinearScaleBackgroundLayerPresentation), typeof(CleanWhiteLinearScaleBackgroundLayerPresentation),
					typeof(CosmicLinearScaleBackgroundLayerPresentation), typeof(SmartLinearScaleBackgroundLayerPresentation),
					typeof(ProgressiveLinearScaleBackgroundLayerPresentation), typeof(EcoLinearScaleBackgroundLayerPresentation),
					typeof(FutureLinearScaleBackgroundLayerPresentation), typeof(ClassicLinearScaleBackgroundLayerPresentation),
					typeof(IStyleLinearScaleBackgroundLayerPresentation), typeof(YellowSubmarineLinearScaleBackgroundLayerPresentation),
					typeof(MagicLightLinearScaleBackgroundLayerPresentation), typeof(RedThermometerLinearScaleBackgroundLayerPresentation),
					typeof(ClassicLinearScaleForegroundLayerPresentation), typeof(ProgressiveLinearScaleForegroundLayerPresentation),
					typeof(FlatLightLinearScaleBackgroundLayerPresentation), typeof(FlatDarkLinearScaleBackgroundLayerPresentation),
					typeof(CustomLinearScaleLayerPresentation)));
			builder.AddCustomAttributes(typeof(ArcScaleRange), ArcScaleRange.PresentationProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(DefaultArcScaleRangePresentation), typeof(CustomArcScaleRangePresentation)));
			builder.AddCustomAttributes(typeof(LinearScaleRange), LinearScaleRange.PresentationProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(DefaultLinearScaleRangePresentation), typeof(CustomLinearScaleRangePresentation)));
			builder.AddCustomAttributes(typeof(LinearScaleLevelBar), LinearScaleLevelBar.PresentationProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(DefaultLinearScaleLevelBarPresentation), typeof(CleanWhiteLinearScaleLevelBarPresentation), typeof(CosmicLinearScaleLevelBarPresentation),
					typeof(SmartLinearScaleLevelBarPresentation), typeof(ProgressiveLinearScaleLevelBarPresentation), typeof(EcoLinearScaleLevelBarPresentation),
					typeof(FutureLinearScaleLevelBarPresentation), typeof(ClassicLinearScaleLevelBarPresentation), typeof(IStyleLinearScaleLevelBarPresentation),
					typeof(YellowSubmarineLinearScaleLevelBarPresentation), typeof(MagicLightLinearScaleLevelBarPresentation), typeof(RedThermometerLinearScaleLevelBarPresentation),
					typeof(FlatLightLinearScaleLevelBarPresentation), typeof(FlatDarkLinearScaleLevelBarPresentation), typeof(CustomLinearScaleLevelBarPresentation)));
			builder.AddCustomAttributes(typeof(DigitalGaugeControl), DigitalGaugeControl.SymbolViewProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(SevenSegmentsView), typeof(FourteenSegmentsView),
					typeof(MatrixView5x8), typeof(MatrixView8x14)));
			builder.AddCustomAttributes(typeof(DigitalGaugeControl), DigitalGaugeControl.ModelProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(DigitalDefaultModel), typeof(DigitalCleanWhiteModel), typeof(DigitalCosmicModel), typeof(DigitalSmartModel), typeof(DigitalRedClockModel),
					typeof(DigitalProgressiveModel), typeof(DigitalEcoModel), typeof(DigitalFutureModel), typeof(DigitalClassicModel), typeof(DigitalIStyleModel),
					typeof(DigitalYellowSubmarineModel), typeof(DigitalMagicLightModel)));
			builder.AddCustomAttributes(typeof(SevenSegmentsView), SevenSegmentsView.PresentationProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(DefaultSevenSegmentsPresentation),
					typeof(CustomSevenSegmentsPresentation)));
			builder.AddCustomAttributes(typeof(FourteenSegmentsView), FourteenSegmentsView.PresentationProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(DefaultFourteenSegmentsPresentation),
					typeof(CustomFourteenSegmentsPresentation)));
			builder.AddCustomAttributes(typeof(MatrixView5x8), MatrixView5x8.PresentationProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(DefaultMatrix5x8Presentation),
					typeof(CustomMatrix5x8Presentation)));
			builder.AddCustomAttributes(typeof(MatrixView8x14), MatrixView8x14.PresentationProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(DefaultMatrix8x14Presentation),
					typeof(CustomMatrix8x14Presentation)));
			builder.AddCustomAttributes(typeof(DigitalGaugeLayer), DigitalGaugeLayer.PresentationProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(DefaultDigitalGaugeBackgroundLayerPresentation), typeof(CleanWhiteDigitalGaugeBackgroundLayerPresentation),
					typeof(CosmicDigitalGaugeBackgroundLayerPresentation), typeof(SmartDigitalGaugeBackgroundLayerPresentation),
					typeof(ProgressiveDigitalGaugeBackgroundLayerPresentation), typeof(EcoDigitalGaugeBackgroundLayerPresentation),
					typeof(FutureDigitalGaugeBackgroundLayerPresentation), typeof(ClassicDigitalGaugeBackgroundLayerPresentation),
					typeof(IStyleDigitalGaugeBackgroundLayerPresentation), typeof(YellowSubmarineDigitalGaugeBackgroundLayerPresentation),
					typeof(MagicLightDigitalGaugeBackgroundLayerPresentation), typeof(RedClockDigitalGaugeBackgroundLayerPresentation),
					typeof(CustomDigitalGaugeLayerPresentation)));
			builder.AddCustomAttributes(typeof(SymbolViewBase), SymbolViewBase.AnimationProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(CreepingLineAnimation),
					typeof(BlinkingAnimation)));
			builder.AddCustomAttributes(typeof(StateIndicatorControl), StateIndicatorControl.ModelProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(EmptyStateIndicatorModel), typeof(TrafficLightsStateIndicatorModel),
					typeof(LampStateIndicatorModel), typeof(SmileStateIndicatorModel), typeof(ArrowStateIndicatorModel)));
			builder.AddCustomAttributes(typeof(State), State.PresentationProperty.GetName(),
				new NewItemTypesAttribute(
					typeof(TrafficLightsOffStatePresentation), typeof(TrafficLightsRedStatePresentation), typeof(TrafficLightsYellowStatePresentation),
					typeof(TrafficLightsGreenStatePresentation), typeof(TrafficLightsDefaultStatePresentation),
					typeof(LampOffStatePresentation), typeof(LampRedStatePresentation), typeof(LampYellowStatePresentation),
					typeof(LampGreenStatePresentation), typeof(LampDefaultStatePresentation),
					typeof(SmileHappyStatePresentation), typeof(SmileGladStatePresentation), typeof(SmileIndifferentStatePresentation),
					typeof(SmileSadStatePresentation), typeof(SmileDefaultStatePresentation),
					typeof(ArrowUpStatePresentation), typeof(ArrowDownStatePresentation), typeof(ArrowLeftStatePresentation),
					typeof(ArrowRightStatePresentation), typeof(ArrowLeftUpStatePresentation), typeof(ArrowRightUpStatePresentation),
					typeof(ArrowRightDownStatePresentation), typeof(ArrowLeftDownStatePresentation), typeof(ArrowDefaultStatePresentation),
					typeof(DefaultStatePresentation), typeof(CustomStatePresentation)));
		}
	}
	internal class RegisterMetadata : MetadataProviderBase {
		protected override void PrepareAttributeTable(AttributeTableBuilder builder) {
			RegisterHelper.PrepareAttributeTable(builder);
		}
		protected override Assembly RuntimeAssembly { get { return typeof(CircularGaugeControl).Assembly; } }
		protected override string ToolboxCategoryPath { get { return AssemblyInfo.DXTabNameData; } }
	}
}
