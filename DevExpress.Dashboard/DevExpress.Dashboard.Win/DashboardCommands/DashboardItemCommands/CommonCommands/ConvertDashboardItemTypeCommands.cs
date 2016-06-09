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
using System.Collections.Generic;
using DevExpress.Utils.Commands;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Native;
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Commands {
	public class ConvertDashboardItemTypeCommand : DashboardItemCommand<DataDashboardItem> {
		public override DashboardCommandId Id { get { return DashboardCommandId.ConvertDashboardItemType; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandConvertDashboardItemTypeCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandConvertDashboardItemTypeDescription; } }
		public override string ImageName { get { return "ConvertDashboardItemType"; } }
		public ConvertDashboardItemTypeCommand(DashboardDesigner control)
			: base(control) {
		}
		protected override void ExecuteInternal(ICommandUIState state) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			DataDashboardItem dataDashboardItem = DashboardItem as DataDashboardItem;
			state.Visible = state.Enabled = dataDashboardItem != null;
		}
	}
	public abstract class ConvertToDataDashboardItemCommand<T> : DashboardItemCommand<T> where T : DataDashboardItem, new() {
		protected ConvertToDataDashboardItemCommand(DashboardDesigner control)
			: base(control) {
		}
		protected override void ExecuteInternal(ICommandUIState state) {
			DataDashboardItem item = Control.SelectedDashboardItem as DataDashboardItem;
			if(item != null) {
				ConvertDashboardItemTypeHistoryItem historyItem = new ConvertDashboardItemTypeHistoryItem(item, new T());
				historyItem.Redo(Control);
				Control.History.Add(historyItem);
			}
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Visible = state.Enabled = DashboardItem == null;
		}
	}
	public class ConvertGeoPointMapBaseCommandGroup : DashboardItemCommand<DataDashboardItem> {
		public override DashboardCommandId Id { get { return DashboardCommandId.ConvertGeoPointMapBase; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandConvertToGeoPointMapBaseCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandConvertToGeoPointMapBaseDescription; } }
		public override string ImageName { get { return "InsertGeoPointMap"; } }
		public ConvertGeoPointMapBaseCommandGroup(DashboardDesigner control)
			: base(control) {
		}
		protected override void ExecuteInternal(ICommandUIState state) {
		}
	}
	public class ConvertToPivotCommand : ConvertToDataDashboardItemCommand<PivotDashboardItem> {
		public override DashboardCommandId Id { get { return DashboardCommandId.ConvertToPivot; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandConvertToPivotCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandConvertToPivotDescription; } }
		public override string ImageName { get { return "InsertPivot"; } }
		public ConvertToPivotCommand(DashboardDesigner control)
			: base(control) {
		}
	}
	public class ConvertToChartCommand : ConvertToDataDashboardItemCommand<ChartDashboardItem> {
		public override DashboardCommandId Id { get { return DashboardCommandId.ConvertToChart; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandConvertToChartCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandConvertToChartDescription; } }
		public override string ImageName { get { return "InsertChart"; } }
		public ConvertToChartCommand(DashboardDesigner control)
			: base(control) {
		}
	}
	public class ConvertToScatterChartCommand : ConvertToDataDashboardItemCommand<ScatterChartDashboardItem> {
		public override DashboardCommandId Id { get { return DashboardCommandId.ConvertToScatterChart; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandConvertToScatterChartCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandConvertToScatterChartDescription; } }
		public override string ImageName { get { return "InsertScatterChart"; } }
		public ConvertToScatterChartCommand(DashboardDesigner control)
			: base(control) {
		}
	}
	public class ConvertToGridCommand : ConvertToDataDashboardItemCommand<GridDashboardItem> {
		public override DashboardCommandId Id { get { return DashboardCommandId.ConvertToGrid; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandConvertToGridCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandConvertToGridDescription; } }
		public override string ImageName { get { return "InsertGrid"; } }
		public ConvertToGridCommand(DashboardDesigner control)
			: base(control) {
		}
	}
	public class ConvertToPieCommand : ConvertToDataDashboardItemCommand<PieDashboardItem> {
		public override DashboardCommandId Id { get { return DashboardCommandId.ConvertToPie; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandConvertToPieCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandConvertToPieDescription; } }
		public override string ImageName { get { return "InsertPies"; } }
		public ConvertToPieCommand(DashboardDesigner control)
			: base(control) {
		}
	}
	public class ConvertToGaugeCommand : ConvertToDataDashboardItemCommand<GaugeDashboardItem> {
		public override DashboardCommandId Id { get { return DashboardCommandId.ConvertToGauge; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandConvertToGaugeCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandConvertToGaugeDescription; } }
		public override string ImageName { get { return "InsertGauges"; } }
		public ConvertToGaugeCommand(DashboardDesigner control)
			: base(control) {
		}
	}
	public class ConvertToCardCommand : ConvertToDataDashboardItemCommand<CardDashboardItem> {
		public override DashboardCommandId Id { get { return DashboardCommandId.ConvertToCard; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandConvertToCardCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandConvertToCardDescription; } }
		public override string ImageName { get { return "InsertCards"; } }
		public ConvertToCardCommand(DashboardDesigner control)
			: base(control) {
		}
	}
	public class ConvertToChoroplethMapCommand : ConvertToDataDashboardItemCommand<ChoroplethMapDashboardItem> {
		public override DashboardCommandId Id { get { return DashboardCommandId.ConvertToChoroplethMap; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandConvertToChoroplethMapCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandConvertToChoroplethMapDescription; } }
		public override string ImageName { get { return "InsertChoroplethMap"; } }
		public ConvertToChoroplethMapCommand(DashboardDesigner control)
			: base(control) {
		}
	}
	public class ConvertToGeoPointMapCommand : ConvertToDataDashboardItemCommand<GeoPointMapDashboardItem> {
		public override DashboardCommandId Id { get { return DashboardCommandId.ConvertToGeoPointMap; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandConvertToGeoPointMapCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandConvertToGeoPointMapDescription; } }
		public override string ImageName { get { return "InsertGeoPointMap"; } }
		public ConvertToGeoPointMapCommand(DashboardDesigner control)
			: base(control) {
		}
	}
	public class ConvertToBubbleMapCommand : ConvertToDataDashboardItemCommand<BubbleMapDashboardItem> {
		public override DashboardCommandId Id { get { return DashboardCommandId.ConvertToBubbleMap; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandConvertToBubbleMapCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandConvertToBubbleMapDescription; } }
		public override string ImageName { get { return "InsertBubbleMap"; } }
		public ConvertToBubbleMapCommand(DashboardDesigner control)
			: base(control) {
		}
	}
	public class ConvertToPieMapCommand : ConvertToDataDashboardItemCommand<PieMapDashboardItem> {
		public override DashboardCommandId Id { get { return DashboardCommandId.ConvertToPieMap; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandConvertToPieMapCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandConvertToPieMapDescription; } }
		public override string ImageName { get { return "InsertPieMap"; } }
		public ConvertToPieMapCommand(DashboardDesigner control)
			: base(control) {
		}
	}
	public class ConvertToRangeFilterCommand : ConvertToDataDashboardItemCommand<RangeFilterDashboardItem> {
		public override DashboardCommandId Id { get { return DashboardCommandId.ConvertToRangeFilter; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandConvertToRangeFilterCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandConvertToRangeFilterDescription; } }
		public override string ImageName { get { return "InsertRangeFilter"; } }
		public ConvertToRangeFilterCommand(DashboardDesigner control)
			: base(control) {
		}
	}
	public class ConvertToComboBoxCommand : ConvertToDataDashboardItemCommand<ComboBoxDashboardItem> {
		public override DashboardCommandId Id { get { return DashboardCommandId.ConvertToComboBox; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandConvertToComboBoxCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandConvertToComboBoxDescription; } }
		public override string ImageName { get { return "InsertComboBox"; } }
		public ConvertToComboBoxCommand(DashboardDesigner control)
			: base(control) {
		}
	}
	public class ConvertToListBoxCommand : ConvertToDataDashboardItemCommand<ListBoxDashboardItem> {
		public override DashboardCommandId Id { get { return DashboardCommandId.ConvertToListBox; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandConvertToListBoxCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandConvertToListBoxDescription; } }
		public override string ImageName { get { return "InsertListBox"; } }
		public ConvertToListBoxCommand(DashboardDesigner control)
			: base(control) {
		}
	}
	public class ConvertToTreeViewCommand : ConvertToDataDashboardItemCommand<TreeViewDashboardItem> {
		public override DashboardCommandId Id { get { return DashboardCommandId.ConvertToTreeView; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandConvertToTreeViewCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandConvertToTreeViewDescription; } }
		public override string ImageName { get { return "InsertTreeView"; } }
		public ConvertToTreeViewCommand(DashboardDesigner control)
			: base(control) {
		}
	}
}
