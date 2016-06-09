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

using System;
using System.Linq;
using DevExpress.Utils.Commands;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Bars;
using DevExpress.DashboardWin.Native;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardWin.Commands {
	public abstract class ChartLegendPositionCommandBase<T> : DashboardItemCommand<T> where T : DashboardItem {
		public override string ImageName { get { return "ChangeLegendPosition"; } }
		protected ChartLegendPositionCommandBase(DashboardDesigner control)
			: base(control) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			DashboardChartLegendPositionCommandUIState legendPositionGalleryState = state as DashboardChartLegendPositionCommandUIState;
			if(legendPositionGalleryState != null) {
				IChartDashboardItem chartDashboardItem = (IChartDashboardItem)DashboardItem;
				if(chartDashboardItem != null)
					legendPositionGalleryState.UpdateVisualState(chartDashboardItem.Legend);
			}
		}
		protected override void ExecuteInternal(ICommandUIState state) {
			DashboardChartLegendPositionCommandUIState legendPositionGalleryState = state as DashboardChartLegendPositionCommandUIState;
			if(legendPositionGalleryState != null) {
				IChartDashboardItem chartDashboardItem = (IChartDashboardItem)DashboardItem;
				if(chartDashboardItem != null) {
					ChartLegendPositionHistoryItem historyItem = null;
					ChartLegendInsidePositionGalleryItem insideItem = legendPositionGalleryState.SelectedChartLegendPositionGalleryItem as ChartLegendInsidePositionGalleryItem;
					if(insideItem != null)
						historyItem = new ChartLegendInsidePositionHistoryItem(chartDashboardItem, insideItem.Position);
					ChartLegendOutsidePositionGalleryItem outsideItem = legendPositionGalleryState.SelectedChartLegendPositionGalleryItem as ChartLegendOutsidePositionGalleryItem;
					if(outsideItem != null)
						historyItem = new ChartLegendOutsidePositionHistoryItem(chartDashboardItem, outsideItem.Position);
					if(historyItem != null) {
						historyItem.Redo(Control);
						Control.History.Add(historyItem);
					}
				}
			}
		}
	}
	public class ChartLegendPositionCommand : ChartLegendPositionCommandBase<ChartDashboardItem> {
		public override DashboardCommandId Id { get { return DashboardCommandId.ChartLegendPosition; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandChartLegendPositionCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandChartLegendPositionDescription; } }
		public ChartLegendPositionCommand(DashboardDesigner control)
			: base(control) {
		}
	}
	public class ScatterChartLegendPositionCommand : ChartLegendPositionCommandBase<ScatterChartDashboardItem> {
		public override DashboardCommandId Id { get { return DashboardCommandId.ScatterChartLegendPosition; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandScatterChartLegendPositionCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandScatterChartLegendPositionDescription; } }
		public ScatterChartLegendPositionCommand(DashboardDesigner control)
			: base(control) {
		}
	}
}
