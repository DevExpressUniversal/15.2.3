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

using System.Windows.Input;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Windows.Media;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Charts.Designer.Native {
	public abstract class ChartColorizerBasePropertyGridModel : NestedElementPropertyGridModelBase {
		public static ChartColorizerBasePropertyGridModel CreatePropertyGridModel(WpfChartModel chartModel, ChartColorizerBase colorizer, string propertyPath) {
			if (colorizer is ColorObjectColorizer)
				return new ColorObjectColorizerPropertyGridModel(chartModel, (ColorObjectColorizer)colorizer, propertyPath);
			if (colorizer is KeyColorColorizer)
				return new KeyColorColorizerPropertyGridModel(chartModel, (KeyColorColorizer)colorizer, propertyPath);
			if (colorizer is RangeColorizer)
				return new RangeColorizerPropertyGridModel(chartModel, (RangeColorizer)colorizer, propertyPath);
			return null;
		}
		protected internal ChartColorizerBase Colorizer { get { return colorizer; } }
		protected override ICommand SetObjectPropertyCommand { get { return setPropertyCommand; } }
		ChartColorizerBase colorizer;
		SetSeriesPropertyCommand setPropertyCommand;
		public ChartColorizerBasePropertyGridModel(ChartModelElement modelElement, ChartColorizerBase colorizer, string propertyPath)
			: base(modelElement, propertyPath) {
			this.colorizer = colorizer;
			this.setPropertyCommand = new SetSeriesPropertyCommand(ChartModel);
		}
		public abstract ChartColorizerBase CreateColorizer();
	}
	public abstract class ChartColorizerPropertyGridModel : ChartColorizerBasePropertyGridModel {
		new ChartPaletteColorizerBase Colorizer { get { return (ChartPaletteColorizerBase)base.Colorizer; } }
		[Category(Categories.Common), DefaultValue(null)]
		public Palette Palette {
			get { return Colorizer.Palette; }
			set { SetProperty("Palette", value); }
		}
		public ChartColorizerPropertyGridModel() : this(null, null, string.Empty) { }
		public ChartColorizerPropertyGridModel(ChartModelElement modelElement, ChartPaletteColorizerBase colorizer, string propertyPath)
			: base(modelElement, colorizer, propertyPath) {
		}
	}
	public class ColorObjectColorizerPropertyGridModel : ChartColorizerBasePropertyGridModel {
		new ColorObjectColorizer Colorizer { get { return (ColorObjectColorizer)base.Colorizer; } }
		public ColorObjectColorizerPropertyGridModel() : this(null, null, string.Empty) { }
		public ColorObjectColorizerPropertyGridModel(ChartModelElement modelElement, ColorObjectColorizer colorizer, string propertyPath)
			: base(modelElement, colorizer, propertyPath) {
		}
		public override ChartColorizerBase CreateColorizer() {
			return new ColorObjectColorizer();
		}
	}
	public class KeyColorColorizerPropertyGridModel : ChartColorizerPropertyGridModel {
		new KeyColorColorizer Colorizer { get { return (KeyColorColorizer)base.Colorizer; } }
		ColorizerKeysPropertyGridModelCollection keys;
		[Category(Categories.Common)]
		public ColorizerKeysPropertyGridModelCollection Keys { get { return keys; } }
		public KeyColorColorizerPropertyGridModel() : this(null, null, string.Empty) { }
		public KeyColorColorizerPropertyGridModel(WpfChartModel chartModel, KeyColorColorizer colorizer, string propertyPath)
			: base(chartModel, colorizer, propertyPath) {
			keys = new ColorizerKeysPropertyGridModelCollection(chartModel);
			UpdateInternal();
		}
		public override ChartColorizerBase CreateColorizer() {
			return new KeyColorColorizer();
		}
		protected override void UpdateInternal() {
			base.UpdateInternal();
			if (Colorizer != null)
				Keys.UpdateCollection(Colorizer.Keys);
		}
	}
	public class ColorizerKeysPropertyGridModelCollection : ObservableCollection<object> {
		WpfChartModel chartModel;
		bool allowExecuteCommand = true;
		public ColorizerKeysPropertyGridModelCollection(WpfChartModel chartModel) {
			this.chartModel = chartModel;
		}
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
			base.OnCollectionChanged(e);
			if (allowExecuteCommand) {
				switch (e.Action) {
					case NotifyCollectionChangedAction.Add:
						foreach (object newKey in e.NewItems) {
							AddKeyColorColorizerKeyCommand command = new AddKeyColorColorizerKeyCommand(chartModel);
							((ICommand)command).Execute(newKey);
						}
						break;
					case NotifyCollectionChangedAction.Remove:
						int removeIndex = e.OldStartingIndex;
						foreach (object oldKey in e.OldItems) {
							RemoveKeyColorColorizerKeyCommand command = new RemoveKeyColorColorizerKeyCommand(chartModel, removeIndex);
							((ICommand)command).Execute(oldKey);
							removeIndex++;
						}
						break;
					case NotifyCollectionChangedAction.Replace:
						int replaceIndex = e.NewStartingIndex;
						foreach (object newKey in e.NewItems) {
							ReplaceKeyColorColorizerKeyCommand command = new ReplaceKeyColorColorizerKeyCommand(chartModel, replaceIndex);
							((ICommand)command).Execute(newKey);
							replaceIndex++;
						}
						break;
					default:
						break;
				}
			}
		}
		public void UpdateCollection(IList<object> items) {
			allowExecuteCommand = false;
			ClearItems();
			if (items != null)
				foreach (object item in items)
					Add(item);
			allowExecuteCommand = true;
		}
	}
	public class RangeColorizerPropertyGridModel : ChartColorizerPropertyGridModel {
		new RangeColorizer Colorizer { get { return (RangeColorizer)base.Colorizer; } }
		ColorizerDoublePropertyGridModelCollection rangeStops;
		[Category(Categories.Common)]
		public ColorizerDoublePropertyGridModelCollection RangeStops { get { return rangeStops; } }
		public RangeColorizerPropertyGridModel() : this(null, null, string.Empty) { }
		public RangeColorizerPropertyGridModel(WpfChartModel chartModel, RangeColorizer colorizer, string propertyPath)
			: base(chartModel, colorizer, propertyPath) {
			rangeStops = new ColorizerDoublePropertyGridModelCollection(chartModel);
			UpdateInternal();
		}
		public override ChartColorizerBase CreateColorizer() {
			return new RangeColorizer();
		}
		protected override void UpdateInternal() {
			base.UpdateInternal();
			if (Colorizer != null)
				RangeStops.UpdateCollection(Colorizer.RangeStops);
		}
	}
	public class ColorizerDoublePropertyGridModelCollection : ObservableCollection<double> {
		WpfChartModel chartModel;
		bool allowExecuteCommand = true;
		public ColorizerDoublePropertyGridModelCollection(WpfChartModel chartModel) {
			this.chartModel = chartModel;
		}
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
			base.OnCollectionChanged(e);
			if (allowExecuteCommand) {
				switch (e.Action) {
					case NotifyCollectionChangedAction.Add:
						foreach (object newKey in e.NewItems) {
							AddRangeColorizerRangeStopCommand command = new AddRangeColorizerRangeStopCommand(chartModel);
							((ICommand)command).Execute(newKey);
						}
						break;
					case NotifyCollectionChangedAction.Remove:
						int removeIndex = e.OldStartingIndex;
						foreach (object oldKey in e.OldItems) {
							RemoveRangeColorizerRangeStopCommand command = new RemoveRangeColorizerRangeStopCommand(chartModel, removeIndex);
							((ICommand)command).Execute(oldKey);
							removeIndex++;
						}
						break;
					case NotifyCollectionChangedAction.Replace:
						int replaceIndex = e.NewStartingIndex;
						foreach (object newKey in e.NewItems) {
							ReplaceRangeColorizerRangeStopCommand command = new ReplaceRangeColorizerRangeStopCommand(chartModel, replaceIndex);
							((ICommand)command).Execute(newKey);
							replaceIndex++;
						}
						break;
					default:
						break;
				}
			}
		}
		public void UpdateCollection(DoubleCollection items) {
			allowExecuteCommand = false;
			ClearItems();
			if (items != null)
				foreach (double item in items)
					Add(item);
			allowExecuteCommand = true;
		}
	}
}
