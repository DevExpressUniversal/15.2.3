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
using System.Security;
using System.Windows.Controls;
using DevExpress.Xpf.Charts;
namespace DevExpress.Charts.Designer.Native {
	public abstract class LegendOptionsCommand : ChartCommandBase {
		protected WpfChartLegendModel LegendModel { get { return ChartModel.LegendModel; } }
		public LegendOptionsCommand(WpfChartModel model) : base(model) {
		}
		protected override bool CanExecute(object parameter) {
			return LegendModel != null;
		}
	}
	public class ChangeLegendPositionCommand : LegendOptionsCommand {
		readonly string imageName;
		readonly HorizontalPosition horizontalPosition;
		readonly VerticalPosition verticalPosition;
		public override string Caption { get { return null; } }
		public override string ImageName { get { return imageName; } }
		public HorizontalPosition HorizontalPosition { get { return horizontalPosition; } }
		public VerticalPosition VerticalPosition { get { return verticalPosition; } }
		public ChartControl ChartControl { get { return PreviewChart; } }
		public ChangeLegendPositionCommand(WpfChartModel chartModel, HorizontalPosition horizPos, VerticalPosition vertPos) : base(chartModel) {
			horizontalPosition = horizPos;
			verticalPosition = vertPos;
			imageName = GlyphUtils.GalleryItemImages + "LegendPositions/" + verticalPosition.ToString() + horizontalPosition.ToString();
		}
		protected override bool CanExecute(object parameter) {
			return ChartControl.Diagram != null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			LegendModel.VerticalPosition = ((LegendPosition)historyItem.OldValue).VerticalPosition;
			LegendModel.HorizontalPosition = ((LegendPosition)historyItem.OldValue).HorizontalPosition;
			return null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			LegendModel.VerticalPosition = ((LegendPosition)historyItem.NewValue).VerticalPosition;
			LegendModel.HorizontalPosition = ((LegendPosition)historyItem.NewValue).HorizontalPosition;
			return null;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if (!(parameter is bool) || !((bool)parameter))
				return null;
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter);
			CompositeHistoryItem compositHistoryItem = new CompositeHistoryItem();
			if (LegendModel == null) {
				CreateLegendCommand createLegendCommand = new CreateLegendCommand(ChartModel);
				CommandResult result = createLegendCommand.RuntimeExecute(parameter: null);
				if (result != null) {
					ChartModel.RecursivelyUpdateChildren();
					ChartModel.UpdateCommandsCanExecute();
					if (result.SelectedChartObject != null)
						ChartModel.SelectedObject = result.SelectedChartObject;
					compositHistoryItem.HistoryItems.Add(result.HistoryItem);
				}
			}
			HistoryItem historyItem = new HistoryItem(info, this, LegendModel.Legend, new LegendPosition(LegendModel.HorizontalPosition, LegendModel.VerticalPosition), new LegendPosition(horizontalPosition, verticalPosition));
			compositHistoryItem.HistoryItems.Add(historyItem);
			LegendModel.HorizontalPosition = horizontalPosition;
			LegendModel.VerticalPosition = verticalPosition;
			return new CommandResult(compositHistoryItem, LegendModel.Legend);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			chartControl.Legend.HorizontalPosition = this.horizontalPosition;
			chartControl.Legend.VerticalPosition = this.verticalPosition;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			chartModelItem.Properties["Legend"].Value.Properties["HorizontalPosition"].SetValue(this.horizontalPosition);
			chartModelItem.Properties["Legend"].Value.Properties["VerticalPosition"].SetValue(this.verticalPosition);
		}
	}
	public class ToggleLegendReverseItemsCommand : LegendOptionsCommand {
		public override string Caption { get { return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.LegendOptions_ReverseItems); } }
		public override string ImageName { get { return GlyphUtils.BarItemImages + "LegendOptions\\ReverseItems"; } }
		public ToggleLegendReverseItemsCommand(WpfChartModel chartModel) : base(chartModel) {
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			PreviewChart.Legend.ReverseItems = (bool)historyItem.OldValue;
			return null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			PreviewChart.Legend.ReverseItems = (bool)historyItem.NewValue;
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			chartControl.Legend.ReverseItems = (bool)historyItem.NewValue;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			chartModelItem.Properties["Legend"].Value.Properties["ReverseItems"].SetValue(historyItem.NewValue);
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if (parameter == null || !(parameter is bool))
				return null;
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter);
			Legend legend = PreviewChart.Legend;
			bool reverseItems = (bool)parameter;
			HistoryItem historyItem = new HistoryItem(info, this, PreviewChart.Legend, legend.ReverseItems, reverseItems);
			legend.ReverseItems = reverseItems;
			return new CommandResult(historyItem);
		}
	}
	public class ChangeLegendOrientationCommand : LegendOptionsCommand {
		readonly Orientation orientation;
		readonly string imageName;
		readonly string caption;
		readonly string description;
		public override string Caption { get { return caption; } }
		public override string Description { get { return description; } }
		public override string ImageName { get { return imageName; } }
		public Orientation Orientation { get { return orientation; } }
		public ChartControl ChartControl { get { return PreviewChart; } }
		public ChangeLegendOrientationCommand(WpfChartModel chartModel, Orientation orientation) : base(chartModel) {
			this.orientation = orientation;
			imageName = GlyphUtils.GalleryItemImages + "LegendOrientation/" + orientation.ToString();
			if (orientation == Orientation.Horizontal) {
				caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.LegendOptions_Orientation_Horizontal);
				description = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.LegendOptions_Orientation_HorizontalDescription);
			}
			else {
				caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.LegendOptions_Orientation_Vertical);
				description = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.LegendOptions_Orientation_VerticalDescription);
			}
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			PreviewChart.Legend.Orientation = (Orientation)historyItem.OldValue;
			return null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			PreviewChart.Legend.Orientation = (Orientation)historyItem.NewValue;
			return null;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if (parameter != null && (bool)parameter != true)
				return null;
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter);
			Legend legend = PreviewChart.Legend;
			HistoryItem historyItem = new HistoryItem(info, this, PreviewChart.Legend, legend.Orientation, orientation);
			legend.Orientation = orientation;
			return new CommandResult(historyItem);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			chartControl.Legend.Orientation = orientation;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			chartModelItem.Properties["Legend"].Value.Properties["Orientation"].SetValue(orientation);
		}
	}
	public class LegendFontFamilyCommand : FontFamilyCommand {
		public LegendFontFamilyCommand(WpfChartModel chartModel) : base(chartModel) {
		}
		protected override bool CanExecute(object parameter) {
			return ChartModel.SelectedModel is WpfChartLegendModel;
		}
		protected override Control GetFontHolder(ChartControl chartControl, HistoryItem historyItem) {
			return FontCommandUtils.GetLegendFontHolder(chartControl);
		}
		protected override IModelItem GetFontHolder(IModelItem chartModelItem, HistoryItem historyItem) {
			return FontCommandUtils.GetLegendFontHolder(chartModelItem);
		}
	}
	public class LegendFontSizeCommand : FontSizeCommand {
		public LegendFontSizeCommand(WpfChartModel chartModel) : base(chartModel) {
		}
		protected override bool CanExecute(object parameter) {
			return ChartModel.SelectedModel is WpfChartLegendModel;
		}
		protected override Control GetFontHolder(ChartControl chartControl, HistoryItem historyItem) {
			return FontCommandUtils.GetLegendFontHolder(chartControl);
		}
		protected override IModelItem GetFontHolder(IModelItem chartModelItem, HistoryItem historyItem) {
			return FontCommandUtils.GetLegendFontHolder(chartModelItem);
		}
	}
	public class LegendFontBoldCommand : FontBoldCommand {
		public LegendFontBoldCommand(WpfChartModel chartModel) : base(chartModel) {
		}
		protected override bool CanExecute(object parameter) {
			return ChartModel.SelectedModel is WpfChartLegendModel;
		}
		protected override Control GetFontHolder(ChartControl chartControl, HistoryItem historyItem) {
			return FontCommandUtils.GetLegendFontHolder(chartControl);
		}
		protected override IModelItem GetFontHolder(IModelItem chartModelItem, HistoryItem historyItem) {
			return FontCommandUtils.GetLegendFontHolder(chartModelItem);
		}
	}
	public class LegendFontItalicCommand : FontItalicCommand {
		public LegendFontItalicCommand(WpfChartModel chartModel) : base(chartModel) {
		}
		protected override bool CanExecute(object parameter) {
			return ChartModel.SelectedModel is WpfChartLegendModel;
		}
		protected override Control GetFontHolder(ChartControl chartControl, HistoryItem historyItem) {
			return FontCommandUtils.GetLegendFontHolder(chartControl);
		}
		protected override IModelItem GetFontHolder(IModelItem chartModelItem, HistoryItem historyItem) {
			return FontCommandUtils.GetLegendFontHolder(chartModelItem);
		}
	}
}
