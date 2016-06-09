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

using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.Native;
using DevExpress.Utils.Commands;
namespace DevExpress.DashboardWin.Commands {
	public class WeightedLegendChangeTypeCommand : DashboardItemCommand<GeoPointMapDashboardItemBase> {
		public override DashboardCommandId Id { get { return DashboardCommandId.WeightedLegendChangeType; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandChangeWeightedLegendTypeCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandChangeWeightedLegendTypeDescription; } }
		public override string ImageName { get { return "InlineWeightLegend"; } }
		public WeightedLegendChangeTypeCommand(DashboardDesigner control)
			: base(control) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = true;
		}
		protected override void ExecuteInternal(ICommandUIState state) {
		}
	}
	public abstract class ChangeWeightedLegendCommandBase : DashboardItemInteractionCommand<MapDashboardItem> {
		protected abstract WeightedLegendType LegendType { get; }
		protected ChangeWeightedLegendCommandBase(DashboardDesigner control)
			: base(control) {
		}
		protected override bool CheckDashboardItem(MapDashboardItem item) {
			WeightedLegend legend = item.WeightedLegend;
			return legend != null ? legend.Type == LegendType && legend.Visible : false;
		}
		protected override IHistoryItem CreateHistoryItem(MapDashboardItem dashboardItem, bool enabled) {
			WeightedLegend legend = dashboardItem.WeightedLegend;
			return legend != null ? new WeightedLegendChangeTypeHistoryItem(dashboardItem, legend, LegendType) : null;
		}
	}
	public class WeightedLegendLinearTypeCommand : ChangeWeightedLegendCommandBase {
		public override DashboardCommandId Id { get { return DashboardCommandId.WeightedLegendLinearType; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandWeightedLegendLinearTypeCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandWeightedLegendLinearTypeDescription; } }
		public override string ImageName { get { return "InlineWeightLegend"; } }
		protected override WeightedLegendType LegendType { get { return WeightedLegendType.Linear; } }
		public WeightedLegendLinearTypeCommand(DashboardDesigner control) 
			: base(control) {
		}
	}
	public class WeightedLegendNestedTypeCommand : ChangeWeightedLegendCommandBase {
		public override DashboardCommandId Id { get { return DashboardCommandId.WeightedLegendNestedType; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandWeightedLegendNestedTypeCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandWeightedLegendNestedTypeDescription; } }
		public override string ImageName { get { return "NestedWeightLegend"; } }
		protected override WeightedLegendType LegendType { get { return WeightedLegendType.Nested; } }
		public WeightedLegendNestedTypeCommand(DashboardDesigner control)
			: base(control) {
		}
	}
	public class WeightedLegendNoneCommand : DashboardItemInteractionCommand<MapDashboardItem> {
		public override DashboardCommandId Id { get { return DashboardCommandId.WeightedLegendNoneType; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandWeightedLegendNoneTypeCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandWeightedLegendNoneTypeDescription; } }
		public override string ImageName { get { return "NoneWeightLegend"; } }
		public WeightedLegendNoneCommand(DashboardDesigner control)
			: base(control) {
		}
		protected override bool CheckDashboardItem(MapDashboardItem item) {
			WeightedLegend legend = item.WeightedLegend;
			return legend != null ? !legend.Visible : false;
		}
		protected override IHistoryItem CreateHistoryItem(MapDashboardItem dashboardItem, bool enabled) {
			WeightedLegend legend = dashboardItem.WeightedLegend;
			return legend != null ? new DisableWeightedLegendHistoryItem(dashboardItem, legend) : null;
		}
	}
}
