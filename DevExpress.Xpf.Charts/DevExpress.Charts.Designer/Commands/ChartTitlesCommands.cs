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

extern alias Platform;
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Charts;
using System.Security;
namespace DevExpress.Charts.Designer.Native {
	public abstract class ChangeChartTitleCommandBase : ChartCommandBase {
		protected const string TitleIndexKey = "Title";
		protected WpfChartTitleModel TitleModel { get { return ChartModel.SelectedModel as WpfChartTitleModel; } }
		public ChangeChartTitleCommandBase(WpfChartModel chartModel) : base(chartModel) {
		}
		protected override bool CanExecute(object parameter) {
			return TitleModel != null;
		}
	}
	public class ChartTitlePositionCommand : ChangeChartTitleCommandBase {
		public class TitlePosition {
			public int TitleIndex { get; private set; }
			public Dock Dock { get; private set; }
			public HorizontalAlignment HorizontalAlignment { get; private set; }
			public VerticalAlignment VerticalAlignment { get; private set; }
			public TitlePosition(int titleIndex, Dock dock, HorizontalAlignment horizontalAlignment) : this(titleIndex, dock, horizontalAlignment, VerticalAlignment.Center) {
			}
			public TitlePosition(int titleIndex, Dock dock, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment) {
				TitleIndex = titleIndex;
				Dock = dock;
				HorizontalAlignment = horizontalAlignment;
				VerticalAlignment = verticalAlignment;
			}
			public void Apply(ChartControl chart) {
				Title title = chart.Titles[TitleIndex];
				title.Dock = Dock;
				title.HorizontalAlignment = HorizontalAlignment;
				title.VerticalAlignment = VerticalAlignment;
			}
		}
		readonly Dock dock;
		readonly HorizontalAlignment horizontalAlignment;
		public Dock Dock { get { return dock; } }
		public HorizontalAlignment HorizontalAlignment { get { return horizontalAlignment; } }
		public override string Caption { get { return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ChartTitleOptions_Position); } }
		public override string ImageName { get { return GlyphUtils.BarItemImages + "Elements\\AddTitle"; } }
		public ChartTitlePositionCommand(WpfChartModel chartModel, Dock dock, HorizontalAlignment horizontalAlignment) : base(chartModel) {
			this.dock = dock;
			this.horizontalAlignment = horizontalAlignment;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			WpfChartTitleModel titleModel = ChartModel.SelectedModel as WpfChartTitleModel;
			if (titleModel == null || !(parameter is bool) || !((bool)parameter))
				return null;
			int index = PreviewChart.Titles.IndexOf(titleModel.Title);
			TitlePosition oldPosition = new TitlePosition(index, titleModel.Title.Dock, titleModel.Title.HorizontalAlignment, titleModel.Title.VerticalAlignment);
			TitlePosition newPosition = new TitlePosition(index, dock, horizontalAlignment);
			newPosition.Apply(PreviewChart);
			HistoryItem historyItem = new HistoryItem(new ExecuteCommandInfo(parameter), this, PreviewChart, oldPosition, newPosition);
			return new CommandResult(historyItem, titleModel.Title);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			((TitlePosition)historyItem.NewValue).Apply(chartControl);
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			TitlePosition position = (TitlePosition)historyItem.NewValue;
			IModelItem title = chartModelItem.Properties["Titles"].Collection[position.TitleIndex];
			title.Properties["Dock"].SetValue(position.Dock);
			title.Properties["HorizontalAlignment"].SetValue(position.HorizontalAlignment);
			title.Properties["VerticalAlignment"].SetValue(position.VerticalAlignment);
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			((TitlePosition)historyItem.OldValue).Apply(PreviewChart);
			return null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			((TitlePosition)historyItem.NewValue).Apply(PreviewChart);
			return null;
		}
	}
	public class ChartTitleTextCommand : ChangeChartTitleCommandBase {
		public override string Caption { get { return null; } }
		public override string ImageName { get { return null; } }
		public ChartTitleTextCommand(WpfChartModel chartModel) : base(chartModel) {
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if (parameter == null)
				return null;
			string newValue = parameter.ToString();
			int index = ChartModel.Chart.Titles.IndexOf(TitleModel.Title);
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, new ElementIndexItem[] { new ElementIndexItem(TitleIndexKey, index) });
			HistoryItem historyItem = new HistoryItem(info, this, TitleModel.Title, TitleModel.Title.Content, newValue);
			CommandResult result = new CommandResult(historyItem);
			TitleModel.DisplayName = newValue;
			return result;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			Title target = (Title)historyItem.TargetObject;
			target.Content = historyItem.OldValue;
			return null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			Title target = (Title)historyItem.TargetObject;
			target.Content = historyItem.NewValue;
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			int index = historyItem.ExecuteCommandInfo.IndexByNameDictionary[TitleIndexKey];
			chartControl.Titles[index].Content = historyItem.NewValue;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			int index = historyItem.ExecuteCommandInfo.IndexByNameDictionary[TitleIndexKey];
			IModelItem title = chartModelItem.Properties["Titles"].Collection[index];
			title.Properties["Content"].SetValue(historyItem.NewValue);
		}
	}
	public class TitleFontFamilyCommand : FontFamilyCommand {
		public TitleFontFamilyCommand(WpfChartModel chartModel) : base(chartModel) {
		}
		protected override bool CanExecute(object parameter) {
			return ChartModel.SelectedModel is WpfChartTitleModel;
		}
		protected override ElementIndexItem[] GetPathIndexes() {
			return FontCommandUtils.GetTitlePathIndexes(ChartModel);
		}
		protected override Control GetFontHolder(ChartControl chartControl, HistoryItem historyItem) {
			return FontCommandUtils.GetTitleFontHolder(chartControl, historyItem);
		}
		protected override IModelItem GetFontHolder(IModelItem chartModelItem, HistoryItem historyItem) {
			return FontCommandUtils.GetTitleFontHolder(chartModelItem, historyItem);
		}
	}
	public class TitleFontSizeCommand : FontSizeCommand {
		public TitleFontSizeCommand(WpfChartModel chartModel)
			: base(chartModel) {
		}
		protected override bool CanExecute(object parameter) {
			return ChartModel.SelectedModel is WpfChartTitleModel;
		}
		protected override ElementIndexItem[] GetPathIndexes() {
			return FontCommandUtils.GetTitlePathIndexes(ChartModel);
		}
		protected override Control GetFontHolder(ChartControl chartControl, HistoryItem historyItem) {
			return FontCommandUtils.GetTitleFontHolder(chartControl, historyItem);
		}
		protected override IModelItem GetFontHolder(IModelItem chartModelItem, HistoryItem historyItem) {
			return FontCommandUtils.GetTitleFontHolder(chartModelItem, historyItem);
		}
	}
	public class TitleFontBoldCommand : FontBoldCommand {
		public TitleFontBoldCommand(WpfChartModel chartModel)
			: base(chartModel) {
		}
		protected override bool CanExecute(object parameter) {
			return ChartModel.SelectedModel is WpfChartTitleModel;
		}
		protected override ElementIndexItem[] GetPathIndexes() {
			return FontCommandUtils.GetTitlePathIndexes(ChartModel);
		}
		protected override Control GetFontHolder(ChartControl chartControl, HistoryItem historyItem) {
			return FontCommandUtils.GetTitleFontHolder(chartControl, historyItem);
		}
		protected override IModelItem GetFontHolder(IModelItem chartModelItem, HistoryItem historyItem) {
			return FontCommandUtils.GetTitleFontHolder(chartModelItem, historyItem);
		}
	}
	public class TitleFontItalicCommand : FontItalicCommand {
		public TitleFontItalicCommand(WpfChartModel chartModel)
			: base(chartModel) {
		}
		protected override bool CanExecute(object parameter) {
			return ChartModel.SelectedModel is WpfChartTitleModel;
		}
		protected override ElementIndexItem[] GetPathIndexes() {
			return FontCommandUtils.GetTitlePathIndexes(ChartModel);
		}
		protected override Control GetFontHolder(ChartControl chartControl, HistoryItem historyItem) {
			return FontCommandUtils.GetTitleFontHolder(chartControl, historyItem);
		}
		protected override IModelItem GetFontHolder(IModelItem chartModelItem, HistoryItem historyItem) {
			return FontCommandUtils.GetTitleFontHolder(chartModelItem, historyItem);
		}
	}
}
