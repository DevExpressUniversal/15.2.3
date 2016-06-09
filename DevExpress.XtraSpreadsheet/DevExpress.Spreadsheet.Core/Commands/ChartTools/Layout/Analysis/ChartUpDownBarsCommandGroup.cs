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
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region ChartUpDownBarsCommandGroup
	public class ChartUpDownBarsCommandGroup : ChartCommandGroupBase {
		public ChartUpDownBarsCommandGroup(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartUpDownBarsCommandGroup; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartUpDownBarsCommandGroupDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartUpDownBarsCommandGroup; } }
		public override string ImageName { get { return "ChartUpDownBars"; } }
		#endregion
		protected override bool CanModifyChart(Chart chart) {
			if (chart.Views.Count <= 0)
				return false;
			foreach (IChartView view in chart.Views)
				if (!CanModifyView(view))
					return false;
			return true;
		}
		protected virtual bool CanModifyView(IChartView view) {
			return view is ISupportsUpDownBars;
		}
	}
	#endregion
	#region ChartModifyViewCommandBase (abstract class)
	public abstract class ChartModifyViewCommandBase : ModifyChartCommandBase {
		protected ChartModifyViewCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected override bool CanModifyChart(Chart chart) {
			if (chart.Views.Count <= 0)
				return false;
			foreach (IChartView view in chart.Views)
				if (!CanModifyView(view))
					return false;
			return true;
		}
		protected override void ModifyChart(Chart chart) {
			foreach (IChartView view in chart.Views)
				ModifyView(view);
		}
		protected override bool ShouldHideCommand(Chart chart) {
			return false;
		}
		protected override bool IsChecked(Chart chart) {
			return false;
		}
		protected abstract bool CanModifyView(IChartView view);
		protected abstract void ModifyView(IChartView view);
	}
	#endregion
	#region ChartModifyUpDownBarsCommandBase (abstract class)
	public abstract class ChartModifyUpDownBarsCommandBase : ChartModifyViewCommandBase {
		protected ChartModifyUpDownBarsCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected abstract bool ShowUpDownBars { get; }
		protected override bool CanModifyView(IChartView view) {
			return view is ISupportsUpDownBars;
		}
		protected override void ModifyView(IChartView view) {
			ISupportsUpDownBars upDownBars = view as ISupportsUpDownBars;
			if (upDownBars == null)
				return;
			upDownBars.ShowUpDownBars = ShowUpDownBars;
		}
	}
	#endregion
	#region ChartHideUpDownBarsCommand
	public class ChartHideUpDownBarsCommand : ChartModifyUpDownBarsCommandBase {
		public ChartHideUpDownBarsCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartHideUpDownBarsCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartHideUpDownBarsCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartHideUpDownBars; } }
		public override string ImageName { get { return "ChartUpDownBars_None"; } }
		protected override bool ShowUpDownBars { get { return true; } }
		#endregion
	}
	#endregion
	#region ChartShowUpDownBarsCommand
	public class ChartShowUpDownBarsCommand : ChartModifyUpDownBarsCommandBase {
		public ChartShowUpDownBarsCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartShowUpDownBarsCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartShowUpDownBarsCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartShowUpDownBars; } }
		public override string ImageName { get { return "ChartUpDownBars"; } }
		protected override bool ShowUpDownBars { get { return true; } }
		#endregion
	}
	#endregion
}
