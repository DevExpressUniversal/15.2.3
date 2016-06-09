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

using System;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Commands.Internal;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region ChartLegendCommandGroup
	public class ChartLegendCommandGroup : ChartCommandGroupBase {
		public ChartLegendCommandGroup(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartLegendCommandGroup; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartLegendCommandGroupDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartLegendCommandGroup; } }
		public override string ImageName { get { return "ChartLegend_Top"; } }
		#endregion
		protected override bool CanModifyChart(Chart chart) {
			return true;
		}
	}
	#endregion
	#region ChartModifyChartLegendCommandBase (abstract class)
	public abstract class ChartModifyChartLegendCommandBase : ModifyChartCommandBase {
		protected ChartModifyChartLegendCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected abstract bool Overlay { get; }
		protected abstract LegendPosition Position { get; }
		protected override bool CanModifyChart(Chart chart) {
			return true;
		}
		protected override bool ShouldHideCommand(Chart chart) {
			return false;
		}
		protected override bool IsChecked(Chart chart) {
			Legend legend = chart.Legend;
			return legend.Visible && legend.Overlay == Overlay && legend.Position == Position;
		}
		protected override void ModifyChart(Chart chart) {
			Legend legend = chart.Legend;
			legend.Overlay = Overlay;
			legend.Position = Position;
			legend.Visible = true;
		}
	}
	#endregion
	#region ChartLegendNoneCommand
	public class ChartLegendNoneCommand : ChartModifyChartLegendCommandBase {
		public ChartLegendNoneCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartLegendNoneCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartLegendNoneCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartLegendNone; } }
		public override string ImageName { get { return "ChartLegend_None"; } }
		protected override bool Overlay { get { return false; } }
		protected override LegendPosition Position { get { return LegendPosition.Right; } }
		#endregion
		protected override bool IsChecked(Chart chart) {
			return !chart.Legend.Visible;
		}
		protected override void ModifyChart(Chart chart) {
			chart.Legend.Visible = false;
		}
	}
	#endregion
	#region ChartLegendAtRightCommand
	public class ChartLegendAtRightCommand : ChartModifyChartLegendCommandBase {
		public ChartLegendAtRightCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartLegendAtRightCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartLegendAtRightCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartLegendAtRight; } }
		public override string ImageName { get { return "ChartLegend_Right"; } }
		protected override bool Overlay { get { return false; } }
		protected override LegendPosition Position { get { return LegendPosition.Right; } }
		#endregion
	}
	#endregion
	#region ChartLegendAtTopCommand
	public class ChartLegendAtTopCommand : ChartModifyChartLegendCommandBase {
		public ChartLegendAtTopCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartLegendAtTopCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartLegendAtTopCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartLegendAtTop; } }
		public override string ImageName { get { return "ChartLegend_Top"; } }
		protected override bool Overlay { get { return false; } }
		protected override LegendPosition Position { get { return LegendPosition.Top; } }
		#endregion
	}
	#endregion
	#region ChartLegendAtLeftCommand
	public class ChartLegendAtLeftCommand : ChartModifyChartLegendCommandBase {
		public ChartLegendAtLeftCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartLegendAtLeftCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartLegendAtLeftCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartLegendAtLeft; } }
		public override string ImageName { get { return "ChartLegend_Left"; } }
		protected override bool Overlay { get { return false; } }
		protected override LegendPosition Position { get { return LegendPosition.Left; } }
		#endregion
	}
	#endregion
	#region ChartLegendAtBottomCommand
	public class ChartLegendAtBottomCommand : ChartModifyChartLegendCommandBase {
		public ChartLegendAtBottomCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartLegendAtBottomCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartLegendAtBottomCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartLegendAtBottom; } }
		public override string ImageName { get { return "ChartLegend_Bottom"; } }
		protected override bool Overlay { get { return false; } }
		protected override LegendPosition Position { get { return LegendPosition.Bottom; } }
		#endregion
	}
	#endregion
	#region ChartLegendOverlayAtRightCommand
	public class ChartLegendOverlayAtRightCommand : ChartModifyChartLegendCommandBase {
		public ChartLegendOverlayAtRightCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartLegendOverlayAtRightCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartLegendOverlayAtRightCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartLegendOverlayAtRight; } }
		public override string ImageName { get { return "ChartLegend_RightOverlay"; } }
		protected override bool Overlay { get { return true; } }
		protected override LegendPosition Position { get { return LegendPosition.Right; } }
		#endregion
	}
	#endregion
	#region ChartLegendOverlayAtLeftCommand
	public class ChartLegendOverlayAtLeftCommand : ChartModifyChartLegendCommandBase {
		public ChartLegendOverlayAtLeftCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartLegendOverlayAtLeftCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartLegendOverlayAtLeftCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartLegendOverlayAtLeft; } }
		public override string ImageName { get { return "ChartLegend_LeftOverlay"; } }
		protected override bool Overlay { get { return true; } }
		protected override LegendPosition Position { get { return LegendPosition.Left; } }
		#endregion
	}
	#endregion
}
