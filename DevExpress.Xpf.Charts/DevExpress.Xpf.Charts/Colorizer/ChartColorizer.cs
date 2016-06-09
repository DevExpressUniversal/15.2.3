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

using System.Windows;
using System.Windows.Media;
using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public interface ISupportPalette {
		Palette GetPalette();
	}
	public interface ILegendItemsProvider {
		bool ShowInLegend { get; }
		List<ColorizerLegendItem> GetLegendItems(Palette palette);
	}
	public abstract class ChartColorizerBase : ChartDependencyObject {
		public abstract Color? GetPointColor(object argument, object[] values, object colorKey, Palette palette);
		public virtual Color? GetAggregatedPointColor(object argument, List<object> values, List<SeriesPoint> points, Palette palette) {
			return null;
		}
	}
	public abstract class ChartPaletteColorizerBase : ChartColorizerBase, ISupportPalette {
		public static readonly DependencyProperty PaletteProperty = DependencyPropertyManager.Register("Palette", typeof(Palette), typeof(ChartPaletteColorizerBase));
		[
		Category(Categories.Presentation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)]
		public Palette Palette {
			get { return (Palette)GetValue(PaletteProperty); }
			set { SetValue(PaletteProperty, value); }
		}
		Palette ISupportPalette.GetPalette() {
			return Palette;
		}
	}
	public abstract class LegendSupportColorizerBase : ChartPaletteColorizerBase, ILegendItemsProvider, IPatternHolder {
		public static readonly DependencyProperty LegendTextPatternProperty = DependencyPropertyManager.Register("LegendTextPattern", typeof(string), typeof(LegendSupportColorizerBase));
		public static readonly DependencyProperty ShowInLegendProperty = DependencyPropertyManager.Register("ShowInLegend", typeof(bool), typeof(LegendSupportColorizerBase), new PropertyMetadata(true));
		[Category(Categories.Presentation),
		XtraSerializableProperty]
		public string LegendTextPattern {
			get { return (string)GetValue(LegendTextPatternProperty); }
			set { SetValue(LegendTextPatternProperty, value); }
		}
		[Category(Categories.Behavior),
		XtraSerializableProperty]
		public bool ShowInLegend {
			get { return (bool)GetValue(ShowInLegendProperty); }
			set { SetValue(ShowInLegendProperty, value); }
		}
		protected abstract PatternDataProvider GetPatternDataProvider(PatternConstants patternConstant);
		protected abstract void UpdateLegendItems(List<ColorizerLegendItem> legendItems, Palette palette);
		protected string CreateLegendItemText(params object[] contexts) {
			PatternParser patternParser = new PatternParser(LegendTextPattern, this);
			patternParser.SetContext(contexts);
			return patternParser.GetText();
		}
		#region ILegendItemsProvider
		bool ILegendItemsProvider.ShowInLegend { get { return ShowInLegend; } }
		List<ColorizerLegendItem> ILegendItemsProvider.GetLegendItems(Palette palette) {
			List<ColorizerLegendItem> legendItems = new List<ColorizerLegendItem>();
			UpdateLegendItems(legendItems, palette);
			return legendItems;
		}
		#endregion
		#region IPatternHolder
		PatternDataProvider IPatternHolder.GetDataProvider(PatternConstants patternConstant) {
			return GetPatternDataProvider(patternConstant);
		}
		string IPatternHolder.PointPattern { get { return LegendTextPattern; } }
		#endregion
	}
	public class ColorizerLegendItem {
		readonly Color color;
		readonly string text;
		public Color Color { get { return color; } }
		public string Text { get { return text; } }
		public ColorizerLegendItem(Color color, string text) {
			this.color = color;
			this.text = text;
		}
	}
}
