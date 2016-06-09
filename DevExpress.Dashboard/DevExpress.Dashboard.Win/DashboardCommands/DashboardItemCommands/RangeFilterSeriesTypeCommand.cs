#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.Utils.Commands;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Native;
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Commands {
	public abstract class RangeFilterSeriesTypeCommand : DashboardItemCommand<RangeFilterDashboardItem> {
		protected abstract SimpleSeriesType SeriesType { get; }
		protected override string ImageResourcePrefix { get { return "DevExpress.DashboardWin.Images.ChartSeriesTypes"; } }
		protected RangeFilterSeriesTypeCommand(DashboardDesigner control)
			: base(control) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			RangeFilterDashboardItem rangeFilterDashboardItem = DashboardItem;
			if(rangeFilterDashboardItem != null) {
				ChartSimpleSeriesConverter simpleSeriesConverter = rangeFilterDashboardItem.DefaultSeriesConverter as ChartSimpleSeriesConverter;
				state.Checked = simpleSeriesConverter != null ? simpleSeriesConverter.SeriesType == SeriesType : false;
			}
			else
				state.Checked = false;
		}
		protected override void ExecuteInternal(ICommandUIState state) {
			RangeFilterDashboardItem rangeFilterDashboardItem = DashboardItem;
			if(rangeFilterDashboardItem != null) {
				RangeFilterSeriesTypeHistoryItem historyItem = new RangeFilterSeriesTypeHistoryItem(rangeFilterDashboardItem, SeriesType);
				historyItem.Redo(Control);
				Control.History.Add(historyItem);
			}
		}
	}
	public class RangeFilterSeriesTypeLineCommand : RangeFilterSeriesTypeCommand {
		protected override SimpleSeriesType SeriesType { get { return SimpleSeriesType.Line; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandRangeFilterSeriesTypeLineCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandRangeFilterSeriesTypeLineDescription; } }
		public override DashboardCommandId Id { get { return DashboardCommandId.RangeFilterSeriesTypeLine; } }
		public override string ImageName { get { return "Line"; } }
		public RangeFilterSeriesTypeLineCommand(DashboardDesigner designer)
			: base(designer) {
		}
	}
	public class RangeFilterSeriesTypeStackedLineCommand : RangeFilterSeriesTypeCommand {
		protected override SimpleSeriesType SeriesType { get { return SimpleSeriesType.StackedLine; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandRangeFilterSeriesTypeStackedLineCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandRangeFilterSeriesTypeStackedLineDescription; } }
		public override DashboardCommandId Id { get { return DashboardCommandId.RangeFilterSeriesTypeStackedLine; } }
		public override string ImageName { get { return "StackedLine"; } }
		public RangeFilterSeriesTypeStackedLineCommand(DashboardDesigner designer)
			: base(designer) {
		}
	}
	public class RangeFilterSeriesTypeFullStackedLineCommand : RangeFilterSeriesTypeCommand {
		protected override SimpleSeriesType SeriesType { get { return SimpleSeriesType.FullStackedLine; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandRangeFilterSeriesTypeFullStackedLineCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandRangeFilterSeriesTypeFullStackedLineDescription; } }
		public override DashboardCommandId Id { get { return DashboardCommandId.RangeFilterSeriesTypeFullStackedLine; } }
		public override string ImageName { get { return "StackedLine"; } }
		public RangeFilterSeriesTypeFullStackedLineCommand(DashboardDesigner designer)
			: base(designer) {
		}
	}
	public class RangeFilterSeriesTypeAreaCommand : RangeFilterSeriesTypeCommand {
		protected override SimpleSeriesType SeriesType { get { return SimpleSeriesType.Area; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandRangeFilterSeriesTypeAreaCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandRangeFilterSeriesTypeAreaDescription; } }
		public override DashboardCommandId Id { get { return DashboardCommandId.RangeFilterSeriesTypeArea; } }
		public override string ImageName { get { return "Area"; } }
		public RangeFilterSeriesTypeAreaCommand(DashboardDesigner designer)
			: base(designer) {
		}
	}
	public class RangeFilterSeriesTypeStackedAreaCommand : RangeFilterSeriesTypeCommand {
		protected override SimpleSeriesType SeriesType { get { return SimpleSeriesType.StackedArea; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandRangeFilterSeriesTypeStackedAreaCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandRangeFilterSeriesTypeStackedAreaDescription; } }
		public override DashboardCommandId Id { get { return DashboardCommandId.RangeFilterSeriesTypeStackedArea; } }
		public override string ImageName { get { return "StackedArea"; } }
		public RangeFilterSeriesTypeStackedAreaCommand(DashboardDesigner designer)
			: base(designer) {
		}
	}
	public class RangeFilterSeriesTypeFullStackedAreaCommand : RangeFilterSeriesTypeCommand {
		protected override SimpleSeriesType SeriesType { get { return SimpleSeriesType.FullStackedArea; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandRangeFilterSeriesTypeFullStackedAreaCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandRangeFilterSeriesTypeFullStackedAreaDescription; } }
		public override DashboardCommandId Id { get { return DashboardCommandId.RangeFilterSeriesTypeFullStackedArea; } }
		public override string ImageName { get { return "FullStackedArea"; } }
		public RangeFilterSeriesTypeFullStackedAreaCommand(DashboardDesigner designer)
			: base(designer) {
		}
	}
}
