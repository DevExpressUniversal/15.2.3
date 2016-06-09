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
	#region ChartGridlinesCommandGroup
	public class ChartGridlinesCommandGroup : ChartCommandGroupBase {
		public ChartGridlinesCommandGroup(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartGridlinesCommandGroup; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartGridlinesCommandGroupDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartGridlinesCommandGroup; } }
		public override string ImageName { get { return "ChartGridlines"; } }
		#endregion
		protected override bool CanModifyChart(Chart chart) {
			if (chart.PrimaryAxes.Count <= 0)
				return false;
			return true;
		}
	}
	#endregion
	#region ChartPrimaryHorizontalGridlinesCommandGroup
	public class ChartPrimaryHorizontalGridlinesCommandGroup : ChartCommandGroupBase {
		public ChartPrimaryHorizontalGridlinesCommandGroup(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalGridlinesCommandGroup; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalGridlinesCommandGroupDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartPrimaryHorizontalGridlinesCommandGroup; } }
		public override string ImageName { get { return "ChartGridlinesHorizontal_Major"; } }
		#endregion
		protected override bool CanModifyChart(Chart chart) {
			if (chart.PrimaryAxes.Count <= 0)
				return false;
			return ChartModifyPrimaryAxisCommandBase.GetAxis(chart, true) != null;
		}
	}
	#endregion
	#region ChartPrimaryVerticalGridlinesCommandGroup
	public class ChartPrimaryVerticalGridlinesCommandGroup : ChartCommandGroupBase {
		public ChartPrimaryVerticalGridlinesCommandGroup(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalGridlinesCommandGroup; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalGridlinesCommandGroupDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartPrimaryVerticalGridlinesCommandGroup; } }
		public override string ImageName { get { return "ChartGridlinesVertical_Major"; } }
		#endregion
		protected override bool CanModifyChart(Chart chart) {
			if (chart.PrimaryAxes.Count <= 0)
				return false;
			return ChartModifyPrimaryAxisCommandBase.GetAxis(chart, false) != null;
		}
	}
	#endregion
	#region ChartModifyPrimaryGridlinesCommandBase (abstract class)
	public abstract class ChartModifyPrimaryGridlinesCommandBase : ChartModifyPrimaryAxisCommandBase {
		protected ChartModifyPrimaryGridlinesCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected override AxisDataType AxisType { get { return AxisDataType.Agrument; } } 
		protected abstract bool ShowMajor { get; }
		protected abstract bool ShowMinor { get; }
		protected override bool ShouldHideCommand(Chart chart) {
			return false;
		}
		protected override bool IsChecked(Chart chart) {
			AxisBase axis = GetAxis(chart);
			return axis.ShowMajorGridlines == ShowMajor && axis.ShowMinorGridlines == ShowMinor;
		}
		protected override void ModifyChart(Chart chart) {
			AxisBase axis = GetAxis(chart);
			axis.ShowMajorGridlines = ShowMajor;
			axis.ShowMinorGridlines = ShowMinor;
		}
	}
	#endregion
	#region ChartPrimaryHorizontalGridlinesNoneCommand
	public class ChartPrimaryHorizontalGridlinesNoneCommand : ChartModifyPrimaryGridlinesCommandBase {
		public ChartPrimaryHorizontalGridlinesNoneCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalGridlinesNoneCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalGridlinesNoneCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartPrimaryHorizontalGridlinesNone; } }
		public override string ImageName { get { return "ChartGridlinesHorizontal_None"; } }
		protected override bool IsHorizontalAxis { get { return false; } }
		protected override bool ShowMajor { get { return false; } }
		protected override bool ShowMinor { get { return false; } }
		#endregion
	}
	#endregion
	#region ChartPrimaryHorizontalGridlinesMajorCommand
	public class ChartPrimaryHorizontalGridlinesMajorCommand : ChartModifyPrimaryGridlinesCommandBase {
		public ChartPrimaryHorizontalGridlinesMajorCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalGridlinesMajorCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalGridlinesMajorCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartPrimaryHorizontalGridlinesMajor; } }
		public override string ImageName { get { return "ChartGridlinesHorizontal_Major"; } }
		protected override bool IsHorizontalAxis { get { return false; } }
		protected override bool ShowMajor { get { return true; } }
		protected override bool ShowMinor { get { return false; } }
		#endregion
	}
	#endregion
	#region ChartPrimaryHorizontalGridlinesMinorCommand
	public class ChartPrimaryHorizontalGridlinesMinorCommand : ChartModifyPrimaryGridlinesCommandBase {
		public ChartPrimaryHorizontalGridlinesMinorCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalGridlinesMinorCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalGridlinesMinorCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartPrimaryHorizontalGridlinesMinor; } }
		public override string ImageName { get { return "ChartGridlinesHorizontal_Minor"; } }
		protected override bool IsHorizontalAxis { get { return false; } }
		protected override bool ShowMajor { get { return false; } }
		protected override bool ShowMinor { get { return true; } }
		#endregion
	}
	#endregion
	#region ChartPrimaryHorizontalGridlinesMajorAndMinorCommand
	public class ChartPrimaryHorizontalGridlinesMajorAndMinorCommand : ChartModifyPrimaryGridlinesCommandBase {
		public ChartPrimaryHorizontalGridlinesMajorAndMinorCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalGridlinesMajorAndMinorCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalGridlinesMajorAndMinorCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartPrimaryHorizontalGridlinesMajorAndMinor; } }
		public override string ImageName { get { return "ChartGridlinesHorizontal_MajorMinor"; } }
		protected override bool IsHorizontalAxis { get { return false; } }
		protected override bool ShowMajor { get { return true; } }
		protected override bool ShowMinor { get { return true; } }
		#endregion
	}
	#endregion
	#region ChartPrimaryVerticalGridlinesNoneCommand
	public class ChartPrimaryVerticalGridlinesNoneCommand : ChartModifyPrimaryGridlinesCommandBase {
		public ChartPrimaryVerticalGridlinesNoneCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalGridlinesNoneCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalGridlinesNoneCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartPrimaryVerticalGridlinesNone; } }
		public override string ImageName { get { return "ChartGridlinesVertical_None"; } }
		protected override bool IsHorizontalAxis { get { return true; } }
		protected override bool ShowMajor { get { return false; } }
		protected override bool ShowMinor { get { return false; } }
		#endregion
	}
	#endregion
	#region ChartPrimaryVerticalGridlinesMajorCommand
	public class ChartPrimaryVerticalGridlinesMajorCommand : ChartModifyPrimaryGridlinesCommandBase {
		public ChartPrimaryVerticalGridlinesMajorCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalGridlinesMajorCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalGridlinesMajorCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartPrimaryVerticalGridlinesMajor; } }
		public override string ImageName { get { return "ChartGridlinesVertical_Major"; } }
		protected override bool IsHorizontalAxis { get { return true; } }
		protected override bool ShowMajor { get { return true; } }
		protected override bool ShowMinor { get { return false; } }
		#endregion
	}
	#endregion
	#region ChartPrimaryVerticalGridlinesMinorCommand
	public class ChartPrimaryVerticalGridlinesMinorCommand : ChartModifyPrimaryGridlinesCommandBase {
		public ChartPrimaryVerticalGridlinesMinorCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalGridlinesMinorCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalGridlinesMinorCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartPrimaryVerticalGridlinesMinor; } }
		public override string ImageName { get { return "ChartGridlinesVertical_Minor"; } }
		protected override bool IsHorizontalAxis { get { return true; } }
		protected override bool ShowMajor { get { return false; } }
		protected override bool ShowMinor { get { return true; } }
		#endregion
	}
	#endregion
	#region ChartPrimaryVerticalGridlinesMajorAndMinorCommand
	public class ChartPrimaryVerticalGridlinesMajorAndMinorCommand : ChartModifyPrimaryGridlinesCommandBase {
		public ChartPrimaryVerticalGridlinesMajorAndMinorCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalGridlinesMajorAndMinorCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalGridlinesMajorAndMinorCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartPrimaryVerticalGridlinesMajorAndMinor; } }
		public override string ImageName { get { return "ChartGridlinesVertical_MajorMinor"; } }
		protected override bool IsHorizontalAxis { get { return true; } }
		protected override bool ShowMajor { get { return true; } }
		protected override bool ShowMinor { get { return true; } }
		#endregion
	}
	#endregion
}
