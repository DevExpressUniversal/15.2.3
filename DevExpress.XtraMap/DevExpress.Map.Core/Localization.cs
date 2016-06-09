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
using System.Resources;
using DevExpress.Utils;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
using System.Collections.Generic;
namespace DevExpress.Map.Localization {
	#region MapStringId
	public enum MapStringId {
		InvalidBingKeyMessage,
		LatitudeNegativeChar,
		LatitudePositiveChar,
		LongitudeNegativeChar,
		LongitudePositiveChar,
		MsgIncorrectStringFormat,
		PrintSizeModeNormal,
		PrintSizeModeStretch,
		PrintSizeModeZoom,
		PrintMiniMap,
		PrintNavPanel,
		PrintSizeModeTitle,
		PrintItemsTitle,
		SearchPanelNullText,
		SearchPanelShowMoreItems,
		SearchPanelShowOnlyItem,
		Kilometer,
		KilpmeterAbbr,
		Meter,
		MeterAbbr,
		Centimeter,
		CentimeterAbbr,
		Millimeter,
		MillimeterAbbr,
		Mile,
		MileAbbr,
		Furlong,
		FurlongAbbr,
		Yard,
		YardAbbr,
		Foot,
		FootAbbr,
		Inch,
		InchAbbr,
		NauticalMile,
		NauticalMileAbbr
	}
	#endregion
	public class MapResLocalizer : XtraResXLocalizer<MapStringId> {
		public MapResLocalizer()
			: base(new MapLocalizer()) {
		}
		protected override ResourceManager CreateResourceManagerCore() {
#if DXPORTABLE
			return new ResourceManager("DevExpress.Map.Core.LocalizationRes", typeof(MapResLocalizer).GetAssembly());
#else
			return new ResourceManager("DevExpress.Map.LocalizationRes", typeof(MapResLocalizer).GetAssembly());
#endif
		}
		protected override bool DiffersFromCurrentCulture() {
			return false;
		}
		internal static string GetDefaultString(MapStringId id) {
			string result = "";
			switch (id) {
				case MapStringId.InvalidBingKeyMessage: return "The specified Bing Maps key is invalid." + Environment.NewLine + "To create a developer account, refer to" + Environment.NewLine + "http://www.microsoft.com/maps/developers";
				case MapStringId.LatitudeNegativeChar: return "S";
				case MapStringId.LatitudePositiveChar: return "N";
				case MapStringId.LongitudeNegativeChar: return "W";
				case MapStringId.LongitudePositiveChar: return "E";
				case MapStringId.MsgIncorrectStringFormat: return "The specified string format is incorrect.";
				case MapStringId.PrintSizeModeNormal: return "Normal (a map is printed with the size identical to that shown on the form)";
				case MapStringId.PrintSizeModeStretch: return "Stretch (a map is stretched or shrunk to fit the page on which it is printed)";
				case MapStringId.PrintSizeModeZoom: return "Zoom (a map is resized proportionally (without clipping), so that it best fits the page on which it is printed)";
				case MapStringId.PrintMiniMap: return "Mini map";
				case MapStringId.PrintNavPanel: return "Navigation panel";
				case MapStringId.PrintSizeModeTitle: return "Choose how a map should be resized when being printed:";
				case MapStringId.PrintItemsTitle: return "Choose additional items to be printed:";
				case MapStringId.SearchPanelNullText: return "Enter search location";
				case MapStringId.SearchPanelShowMoreItems: return "Show others...";
				case MapStringId.SearchPanelShowOnlyItem: return "Show best result...";
				case MapStringId.Kilometer: return "Kilometer";
				case MapStringId.KilpmeterAbbr: return "km";
				case MapStringId.Meter: return "Meter";
				case MapStringId.MeterAbbr: return "m";
				case MapStringId.Centimeter: return "Centimeter";
				case MapStringId.CentimeterAbbr: return "cm";
				case MapStringId.Millimeter: return "Millimeter";
				case MapStringId.MillimeterAbbr: return "mm";
				case MapStringId.Mile: return "Mile";
				case MapStringId.MileAbbr: return "ml";
				case MapStringId.Furlong: return "Furlong";
				case MapStringId.FurlongAbbr: return "fur";
				case MapStringId.Yard: return "Yard";
				case MapStringId.YardAbbr: return "yd";
				case MapStringId.Foot: return "Foot";
				case MapStringId.FootAbbr: return "ft";
				case MapStringId.Inch: return "Inch";
				case MapStringId.InchAbbr: return "in";
				case MapStringId.NauticalMile: return "Nautical mile";
				case MapStringId.NauticalMileAbbr: return "nmi";
			}
			return result;
		}
	}
	public class MapLocalizer : XtraLocalizer<MapStringId> {
		static MapLocalizer() {
			SetActiveLocalizerProvider(new MapLocalizerProvider(CreateDefaultLocalizer()));
			ForceLocalization();
		}
		public static new XtraLocalizer<MapStringId> Active {
			get { return XtraLocalizer<MapStringId>.Active; }
			set {
				XtraLocalizer<MapStringId>.Active = value;
				ForceLocalization();
			}
		}
		public override XtraLocalizer<MapStringId> CreateResXLocalizer() {
			return new MapResLocalizer();
		}
		public static MapResLocalizer CreateDefaultLocalizer() {
			return new MapResLocalizer();
		}
		static void ForceLocalization() {
			foreach (MapStringId stringId in (MapStringId[])Enum.GetValues(typeof(MapStringId)))
				Active.GetLocalizedString(stringId);
		}
		public static string GetString(MapStringId id) {
			return MapLocalizer.Active.GetLocalizedString(id);
		}
		protected override void PopulateStringTable() {
			foreach (MapStringId stringId in (MapStringId[])Enum.GetValues(typeof(MapStringId)))
				AddString(stringId, MapResLocalizer.GetDefaultString(stringId));
		}
	}
	public class MapLocalizerProvider : DefaultActiveLocalizerProvider<MapStringId> {
		static XtraLocalizer<MapStringId> mapLocalizer;
		public MapLocalizerProvider(XtraLocalizer<MapStringId> defaultLocalizer)
			: base(defaultLocalizer) {
		}
		protected override XtraLocalizer<MapStringId> GetActiveLocalizerCore() {
			return mapLocalizer;
		}
		protected override void SetActiveLocalizerCore(XtraLocalizer<MapStringId> localizer) {
			mapLocalizer = localizer;
		}
	}
}
