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

using DevExpress.XtraCharts.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using DevExpress.Utils.Design;
namespace DevExpress.XtraCharts.Designer.Native {
	public abstract class ChartColorizerBaseModel : DesignerChartElementModelBase {
		readonly ChartColorizerBase colorizer;
		protected internal ChartColorizerBase Colorizer { get { return colorizer; } }
		protected internal override object Element { get { return colorizer; } }
		protected internal override ChartElement ChartElement { get { return null; } }
		public ChartColorizerBaseModel(ChartColorizerBase colorizer, CommandManager commandManager)
			: base(commandManager) {
			this.colorizer = colorizer;
		}
	}
	[ModelOf(typeof(ColorObjectColorizer))]
	public class ColorObjectColorizerModel : ChartColorizerBaseModel {
		protected new ColorObjectColorizer Colorizer { get { return (ColorObjectColorizer)base.Colorizer; } }
		public ColorObjectColorizerModel(ColorObjectColorizer colorizer, CommandManager commandManager)
			: base(colorizer, commandManager) {
		}
		public override string ToString() {
			return Colorizer.ToString();
		}
	}
	public abstract class PaletteChartColorizerBaseModel : ChartColorizerBaseModel, IPaletteRepositoryProvider {
		protected new ChartPaletteColorizerBase Colorizer { get { return (ChartPaletteColorizerBase)base.Colorizer; } }
		[
		Editor("DevExpress.XtraCharts.Design.ColorizerPaletteTypeEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		TypeConverter("DevExpress.XtraCharts.Design.PaletteTypeConverter," + AssemblyInfo.SRAssemblyCharts)]
		public string PaletteName {
			get { return Colorizer.PaletteName; }
			set { SetProperty("PaletteName", value); }
		}
		[PropertyForOptions("Behavior"), TypeConverter(typeof(BooleanTypeConverter))]
		public bool ShowInLegend {
			get { return Colorizer.ShowInLegend; }
			set { SetProperty("ShowInLegend", value); }
		}
		public PaletteChartColorizerBaseModel(ChartPaletteColorizerBase colorizer, CommandManager commandManager)
			: base(colorizer, commandManager) {
		}
		#region IPaletteRepositoryProvider
		PaletteRepository IPaletteRepositoryProvider.GetPaletteRepository() {
			return ((IPaletteRepositoryProvider)Colorizer).GetPaletteRepository();
		}
		void IPaletteRepositoryProvider.SetPaletteRepository(PaletteRepository value) {
			((IPaletteRepositoryProvider)Colorizer).SetPaletteRepository(value);
		}
		#endregion
	}
	[ModelOf(typeof(KeyColorColorizer))]
	public class KeyColorColorizerModel : PaletteChartColorizerBaseModel {
		protected new KeyColorColorizer Colorizer { get { return (KeyColorColorizer)base.Colorizer; } }
		[
		PropertyForOptions,
		Editor(typeof(KeyColorColorizerLegendItemModelPatternEditor), typeof(UITypeEditor))
		]
		public string LegendItemPattern {
			get { return Colorizer.LegendItemPattern; }
			set { SetProperty("LegendItemPattern", value); }
		}
		[Editor(typeof(KeyCollectionModelTypeEditor), typeof(UITypeEditor))]
		public KeyCollection Keys { get { return Colorizer.Keys; } }
		public KeyColorColorizerModel(KeyColorColorizer colorizer, CommandManager commandManager)
			: base(colorizer, commandManager) {
		}
		public override string ToString() {
			return Colorizer.ToString();
		}
	}
	[ModelOf(typeof(RangeColorizer))]
	public class RangeColorizerModel : PaletteChartColorizerBaseModel {
		protected new RangeColorizer Colorizer { get { return (RangeColorizer)base.Colorizer; } }
		[PropertyForOptions]
		public DoubleCollection RangeStops { get { return Colorizer.RangeStops; } }
		[
		PropertyForOptions,
		Editor(typeof(RangeColorizerLegendItemModelPatternEditor), typeof(UITypeEditor))
		]
		public string LegendItemPattern {
			get { return Colorizer.LegendItemPattern; }
			set { SetProperty("LegendItemPattern", value); }
		}
		public RangeColorizerModel(RangeColorizer colorizer, CommandManager commandManager)
			: base(colorizer, commandManager) {
		}
		public override string ToString() {
			return Colorizer.ToString();
		}
	}
}
