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
	#region ChartTitleCommandGroup
	public class ChartTitleCommandGroup : ChartCommandGroupBase {
		public ChartTitleCommandGroup(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartTitleCommandGroup; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartTitleCommandGroupDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartTitleCommandGroup; } }
		public override string ImageName { get { return "ChartTitleAbove"; } }
		#endregion
		protected override bool CanModifyChart(Chart chart) {
			return true;
		}
	}
	#endregion
	#region ChartModifyChartTitleCommandBase (abstract class)
	public abstract class ChartModifyChartTitleCommandBase : ModifyChartCommandBase {
		protected ChartModifyChartTitleCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected override bool CanModifyChart(Chart chart) {
			return true;
		}
		protected override bool ShouldHideCommand(Chart chart) {
			return false;
		}
	}
	#endregion
	#region ChartTitleNoneCommand
	public class ChartTitleNoneCommand : ChartModifyChartTitleCommandBase {
		public ChartTitleNoneCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartTitleNoneCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartTitleNoneCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartTitleNone; } }
		public override string ImageName { get { return "ChartTitleNone"; } }
		#endregion
		protected override bool IsChecked(Chart chart) {
			return !chart.Title.Visible;
		}
		protected override void ModifyChart(Chart chart) {
			chart.AutoTitleDeleted = true;
			chart.Title.Text = ChartText.Empty;
		}
	}
	#endregion
	#region ChartTitleAboveCommand
	public class ChartTitleAboveCommand : ChartModifyChartTitleCommandBase {
		public ChartTitleAboveCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartTitleAboveCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartTitleAboveCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartTitleAbove; } }
		public override string ImageName { get { return "ChartTitleAbove"; } }
		#endregion
		protected override bool IsChecked(Chart chart) {
			return chart.Title.Visible && !chart.Title.Overlay && !chart.AutoTitleDeleted;
		}
		protected override void ModifyChart(Chart chart) {
			chart.AutoTitleDeleted = false;
			if (chart.Title.Text.TextType == ChartTextType.None)
				chart.Title.Text = ChartText.Auto;
			chart.Title.Overlay = false;
		}
	}
	#endregion
	#region ChartTitleCenteredOverlayCommand
	public class ChartTitleCenteredOverlayCommand : ChartModifyChartTitleCommandBase {
		public ChartTitleCenteredOverlayCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartTitleCenteredOverlayCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartTitleCenteredOverlayCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartTitleCenteredOverlay; } }
		public override string ImageName { get { return "ChartTitleCenteredOverlay"; } }
		#endregion
		protected override bool IsChecked(Chart chart) {
			return chart.Title.Visible && chart.Title.Overlay && !chart.AutoTitleDeleted;
		}
		protected override void ModifyChart(Chart chart) {
			chart.AutoTitleDeleted = false;
			if (chart.Title.Text.TextType == ChartTextType.None)
				chart.Title.Text = ChartText.Auto;
			chart.Title.Overlay = true;
		}
	}
	#endregion
}
