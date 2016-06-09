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

using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Native;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
namespace DevExpress.XtraCharts {
	public abstract class ChartPaletteColorizerBase : ChartColorizerBase, IPaletteProvider, IPaletteRepositoryProvider, ILegendItemProvider {
		const bool DefaultShowInLegend = true;
		Palette palette;
		PaletteRepository paletteRepository;
		string paletteName;
		bool showInLegend = DefaultShowInLegend;
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ChartPaletteColorizerBasePalette"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PaletteColorizerBase.Palette"),
		TypeConverter(typeof(CollectionTypeConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		]
		public Palette Palette {
			get { return palette; }
			set {
				if (palette != value) {
					RaisePropertyChanging();
					palette = value;
					RaisePropertyChanged("Palette");
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ChartPaletteColorizerBasePaletteName"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PaletteColorizerBase.PaletteName"),
		Editor("DevExpress.XtraCharts.Design.ColorizerPaletteTypeEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		TypeConverter("DevExpress.XtraCharts.Design.PaletteTypeConverter," + AssemblyInfo.SRAssemblyCharts),
		]
		public string PaletteName {
			get { return paletteName; }
			set {
				if (paletteName == value)
					return;
				if (value == DefaultPalette.DefaultPaletteName)
					if (paletteRepository != null)
						palette = paletteRepository.GetPaletteByName(value);
				RaisePropertyChanging();
				paletteName = value;
				RaisePropertyChanged("Palette");
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ChartPaletteColorizerBaseShowInLegend"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PaletteColorizerBase.ShowInLegend"),
		TypeConverter(typeof(BooleanTypeConverter)),
		Category(Categories.Behavior),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty]
		public bool ShowInLegend {
			get { return showInLegend; }
			set {
				if (value != showInLegend) {
					RaisePropertyChanging();
					showInLegend = value;
					RaisePropertyChanged("ShowInLegend");
				}
			}
		}
		#region IPaletteProvider
		Palette IPaletteProvider.GetPalette() { return palette; }
		string IPaletteProvider.GetPaletteName() { return paletteName; }
		PaletteRepository IPaletteRepositoryProvider.GetPaletteRepository() {
			return paletteRepository;
		}
		void IPaletteRepositoryProvider.SetPaletteRepository(PaletteRepository value) {
			this.paletteRepository = value;
		}
		#endregion
		#region ILegendItemProvider
		List<LegendItem> ILegendItemProvider.GetLegendItems(Palette palette, bool legendTextVisible, Color legendTextColor, Font legendFont, bool legendMarkerVisible, Size legendMarkerSize) {
			return GetLegendItems(palette, legendTextVisible, legendTextColor, legendFont, legendMarkerVisible, legendMarkerSize);
		}
		bool ILegendItemProvider.ShowInLegend {
			get { return showInLegend; }
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "ShowInLegend":
					return ShouldSerializeShowInLegend();
				case "PaletteName":
					return ShouldSerializePaletteName();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeShowInLegend() {
			return showInLegend != DefaultShowInLegend;
		}
		void ResetShowInLegend() {
			ShowInLegend = DefaultShowInLegend;
		}
		bool ShouldSerializePaletteName() {
			return !string.IsNullOrEmpty(paletteName);
		}
		void ResetPaletteName() {
			PaletteName = null;
		}
		bool ShouldSerialize() {
			return ShouldSerializeShowInLegend();
		}
		#endregion
		protected virtual List<LegendItem> GetLegendItems(Palette palette, bool legendTextVisible, Color legendTextColor, Font legendFont, bool legendMarkerVisible, Size legendMarkerSize) {
			List<LegendItem> items = new List<LegendItem>();
			return items;
		}
	}
}
