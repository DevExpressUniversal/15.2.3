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

using System.Resources;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
using DevExpress.XtraGauges.Base;
using DevExpress.XtraGauges.Core;
using DevExpress.XtraGauges.Presets;
namespace DevExpress.XtraGauges.Presets.Localization {
	#region enum GaugesPresetsStringId
	public enum GaugesPresetsStringId {
		ThemeDefault,
		ThemeWhite,
		ThemeDarkNight,
		ThemeDeepFire,
		ThemeIceColdZone,
		ThemeGothicMat,
		ThemeShiningDark,
		ThemeAfricaSunset,
		ThemeMechanical,
		ThemeSilverBlur,
		ThemePureDark,
		ThemeCleanWhite,
		ThemeSportCar,
		ThemeMilitary,
		ThemeRetro,
		ThemeDisco,
		ThemeClever,
		ThemeCosmic,
		ThemeSmart,
		ThemeProgressive,
		ThemeEco,
		ThemeMagicLight,
		ThemeiStyle,
		ThemeFuture,
		ThemeYellowSubmarine,
		ThemeClassic,
		ThemeRed,
		ThemeFlatLight,
		ThemeFlatDark,
		ThemeIgnis,
		ThemeHaze,
		ShapeFull,
		ShapeHalf,
		ShapeQuarterLeft,
		ShapeQuarterRight,
		ShapeThreeFourth,
		ShapeWide,
		ShapeHorizontal,
		ShapeVertical,
		StyleChooserFilterTheme,
		StyleChooserFilterShape,
	}
	#endregion
	public class GaugesPresetsLocalizer : XtraLocalizer<GaugesPresetsStringId> {
		#region static
		static GaugesPresetsLocalizer() {
			SetActiveLocalizerProvider(
					new DefaultActiveLocalizerProvider<GaugesPresetsStringId>(CreateDefaultLocalizer())
				);
		}
		public static XtraLocalizer<GaugesPresetsStringId> CreateDefaultLocalizer() {
			return new GaugesPresetsResXLocalizer();
		}
		public static string GetString(GaugesPresetsStringId id) {
			return Active.GetLocalizedString(id);
		}
		public new static XtraLocalizer<GaugesPresetsStringId> Active {
			get { return XtraLocalizer<GaugesPresetsStringId>.Active; }
			set { XtraLocalizer<GaugesPresetsStringId>.Active = value; }
		}
		#endregion static
		public override XtraLocalizer<GaugesPresetsStringId> CreateResXLocalizer() {
			return new GaugesPresetsResXLocalizer();
		}
		protected override void PopulateStringTable() {
			#region AddString
			AddString(GaugesPresetsStringId.ThemeDefault, "Default");
			AddString(GaugesPresetsStringId.ThemeWhite, "White");
			AddString(GaugesPresetsStringId.ThemeDarkNight, "Dark Night");
			AddString(GaugesPresetsStringId.ThemeDeepFire, "Deep Fire");
			AddString(GaugesPresetsStringId.ThemeIceColdZone, "Ice-Cold Zone");
			AddString(GaugesPresetsStringId.ThemeGothicMat, "Gothic Mat");
			AddString(GaugesPresetsStringId.ThemeShiningDark, "Shining Dark");
			AddString(GaugesPresetsStringId.ThemeAfricaSunset, "Africa Sunset");
			AddString(GaugesPresetsStringId.ThemeMechanical, "Mechanical");
			AddString(GaugesPresetsStringId.ThemeSilverBlur, "Silver Blur");
			AddString(GaugesPresetsStringId.ThemePureDark, "Pure Dark");
			AddString(GaugesPresetsStringId.ThemeCleanWhite, "Clean White");
			AddString(GaugesPresetsStringId.ThemeSportCar, "Sport Car");
			AddString(GaugesPresetsStringId.ThemeMilitary, "Military");
			AddString(GaugesPresetsStringId.ThemeRetro, "Retro");
			AddString(GaugesPresetsStringId.ThemeDisco, "Disco");
			AddString(GaugesPresetsStringId.ThemeClever, "Clever");
			AddString(GaugesPresetsStringId.ThemeCosmic, "Cosmic");
			AddString(GaugesPresetsStringId.ThemeSmart, "Smart");
			AddString(GaugesPresetsStringId.ThemeProgressive, "Progressive");
			AddString(GaugesPresetsStringId.ThemeEco, "Eco");
			AddString(GaugesPresetsStringId.ThemeMagicLight, "Magic Light");
			AddString(GaugesPresetsStringId.ThemeiStyle, "iStyle");
			AddString(GaugesPresetsStringId.ThemeFuture, "Future");
			AddString(GaugesPresetsStringId.ThemeYellowSubmarine, "Yellow Submarine");
			AddString(GaugesPresetsStringId.ThemeClassic, "Classic");
			AddString(GaugesPresetsStringId.ThemeRed, "Red");
			AddString(GaugesPresetsStringId.ThemeFlatLight, "Flat Light");
			AddString(GaugesPresetsStringId.ThemeFlatDark, "Flat Dark");
			AddString(GaugesPresetsStringId.ThemeIgnis, "Ignis");
			AddString(GaugesPresetsStringId.ThemeHaze, "Haze");
			AddString(GaugesPresetsStringId.ShapeFull, "Full");
			AddString(GaugesPresetsStringId.ShapeHalf, "Half");
			AddString(GaugesPresetsStringId.ShapeQuarterLeft, "QuarterLeft");
			AddString(GaugesPresetsStringId.ShapeQuarterRight, "QuarterRight");
			AddString(GaugesPresetsStringId.ShapeThreeFourth, "ThreeFourth");
			AddString(GaugesPresetsStringId.ShapeWide, "Wide");
			AddString(GaugesPresetsStringId.ShapeHorizontal, "Horizontal");
			AddString(GaugesPresetsStringId.ShapeVertical, "Vertical");
			AddString(GaugesPresetsStringId.StyleChooserFilterTheme, "Theme");
			AddString(GaugesPresetsStringId.StyleChooserFilterShape, "Shape");
			#endregion AddString
		}
	}
	public class GaugesPresetsResXLocalizer : XtraResXLocalizer<GaugesPresetsStringId> {
		public GaugesPresetsResXLocalizer()
			: base(new GaugesPresetsLocalizer()) {
		}
		protected override ResourceManager CreateResourceManagerCore() {
			return new ResourceManager("DevExpress.XtraGauges.Presets.LocalizationRes", typeof(GaugesPresetsResXLocalizer).Assembly);
		}
	}
}
