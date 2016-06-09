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
using DevExpress.Utils.Commands;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region ChartCommandGroupBase (abstract class)
	public abstract class ChartCommandGroupBase : SpreadsheetCommandGroup {
		protected ChartCommandGroupBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, InnerControl.DocumentModel.DocumentCapabilities.Charts, !InnerControl.IsInplaceEditorActive);
			state.Visible = true;
			List<Chart> selectedCharts = ModifyChartCommandBase.GetSelectedCharts(ActiveSheet);
			if (selectedCharts.Count <= 0) {
				state.Enabled = false;
				return;
			}
			foreach (Chart chart in selectedCharts) {
				if (!CanModifyChart(chart)) {
					state.Enabled = false;
					return;
				}
			}
			state.Enabled = true;
		}
		protected abstract bool CanModifyChart(Chart chart);
	}
	#endregion
	#region ChartAxesCommandGroup
	public class ChartAxesCommandGroup : ChartCommandGroupBase {
		public ChartAxesCommandGroup(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartAxesCommandGroup; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartAxesCommandGroupDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartAxesCommandGroup; } }
		public override string ImageName { get { return "ChartAxesGroup"; } }
		#endregion
		protected override bool CanModifyChart(Chart chart) {
			if (chart.PrimaryAxes.Count <= 0)
				return false;
			return true;
		}
	}
	#endregion
	#region ChartPrimaryHorizontalAxisCommandGroup
	public class ChartPrimaryHorizontalAxisCommandGroup : ChartCommandGroupBase {
		public ChartPrimaryHorizontalAxisCommandGroup(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisCommandGroup; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisCommandGroupDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartPrimaryHorizontalAxisCommandGroup; } }
		public override string ImageName { get { return "ChartHorizontalAxis_Default"; } }
		#endregion
		protected override bool CanModifyChart(Chart chart) {
			if (chart.PrimaryAxes.Count <= 0)
				return false;
			foreach (IChartView view in chart.Views)
				if (view is RadarChartView)
					return false;
			return ChartModifyPrimaryAxisCommandBase.GetAxis(chart, true) != null;
		}
	}
	#endregion
	#region ChartPrimaryVerticalAxisCommandGroup
	public class ChartPrimaryVerticalAxisCommandGroup : ChartCommandGroupBase {
		public ChartPrimaryVerticalAxisCommandGroup(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisCommandGroup; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisCommandGroupDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartPrimaryVerticalAxisCommandGroup; } }
		public override string ImageName { get { return "ChartVerticalAxis_Default"; } }
		#endregion
		protected override bool CanModifyChart(Chart chart) {
			if (chart.PrimaryAxes.Count <= 0)
				return false;
			return ChartModifyPrimaryAxisCommandBase.GetAxis(chart, false) != null;
		}
	}
	#endregion
	#region ChartModifyPrimaryAxisCommandBase (abstract class)
	public abstract class ChartModifyPrimaryAxisCommandBase : ModifyChartCommandBase {
		protected ChartModifyPrimaryAxisCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected abstract bool IsHorizontalAxis { get; }
		protected abstract AxisDataType AxisType { get; }
		public static AxisBase GetAxis(Chart chart, bool isHorizontal) {
			foreach (AxisBase axis in chart.PrimaryAxes) {
				if (axis.AxisType != AxisDataType.Series) {
					if (isHorizontal) {
						if (axis.Position == AxisPosition.Top || axis.Position == AxisPosition.Bottom)
							return axis;
					}
					else {
						if (axis.Position == AxisPosition.Left || axis.Position == AxisPosition.Right)
							return axis;
					}
				}
			}
			return null;
		}
		protected internal AxisBase GetAxis(Chart chart) {
			return GetAxis(chart, IsHorizontalAxis);
		}
		protected override bool CanModifyChart(Chart chart) {
			if (chart.PrimaryAxes.Count <= 0)
				return false;
			return GetAxis(chart) != null;
		}
		protected override bool ShouldHideCommand(Chart chart) {
			AxisBase axis = GetAxis(chart);
			if (axis == null)
				return false;
			return axis.AxisType != this.AxisType;
		}
	}
	#endregion
	#region ChartPrimaryAxisScaleCommandBase (abstract class)
	public abstract class ChartPrimaryAxisScaleCommandBase : ChartModifyPrimaryAxisCommandBase {
		protected ChartPrimaryAxisScaleCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		protected override AxisDataType AxisType { get { return AxisDataType.Value; } }
		protected abstract DisplayUnitType UnitType { get; }
		#endregion
		protected override bool IsChecked(Chart chart) {
			ValueAxis axis = GetAxis(chart) as ValueAxis;
			return axis != null && !axis.Delete && axis.Scaling.Orientation == AxisOrientation.MinMax && axis.TickLabelPos != TickLabelPosition.None && axis.DisplayUnit.UnitType == UnitType && !axis.Scaling.LogScale;
		}
		protected override void ModifyChart(Chart chart) {
			AxisBase axis = GetAxis(chart);
			axis.Scaling.Orientation = AxisOrientation.MinMax;
			axis.Scaling.LogScale = false;
			axis.TickLabelPos = TickLabelPosition.NextTo;
			axis.Delete = false;
			ValueAxis valueAxis = axis as ValueAxis;
			if (valueAxis != null) {
				valueAxis.DisplayUnit.UnitType = UnitType;
				valueAxis.DisplayUnit.ShowLabel = true;
			}
		}
	}
	#endregion
	#region ChartHidePrimaryHorizontalAxisCommand
	public class ChartHidePrimaryHorizontalAxisCommand : ChartModifyPrimaryAxisCommandBase {
		public ChartHidePrimaryHorizontalAxisCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartHidePrimaryHorizontalAxisCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartHidePrimaryHorizontalAxisCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartHidePrimaryHorizontalAxis; } }
		public override string ImageName { get { return "ChartHorizontalAxis_None"; } }
		protected override bool IsHorizontalAxis { get { return true; } }
		protected override AxisDataType AxisType { get { return AxisDataType.Agrument; } } 
		#endregion
		protected override bool IsChecked(Chart chart) {
			return GetAxis(chart).Delete == true;
		}
		protected override void ModifyChart(Chart chart) {
			GetAxis(chart).Delete = true;
		}
		protected override bool ShouldHideCommand(Chart chart) {
			return false;
		}
	}
	#endregion
	#region ChartPrimaryHorizontalAxisLeftToRightCommand
	public class ChartPrimaryHorizontalAxisLeftToRightCommand : ChartModifyPrimaryAxisCommandBase {
		public ChartPrimaryHorizontalAxisLeftToRightCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisLeftToRightCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisLeftToRightCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartPrimaryHorizontalAxisLeftToRight; } }
		public override string ImageName { get { return "ChartHorizontalAxis_LeftToRight"; } }
		protected override bool IsHorizontalAxis { get { return true; } }
		protected override AxisDataType AxisType { get { return AxisDataType.Agrument; } }
		#endregion
		protected override bool IsChecked(Chart chart) {
			AxisBase axis = GetAxis(chart);
			return !axis.Delete && axis.Scaling.Orientation == AxisOrientation.MinMax && axis.TickLabelPos != TickLabelPosition.None && !axis.Scaling.LogScale;
		}
		protected override void ModifyChart(Chart chart) {
			AxisBase axis = GetAxis(chart);
			axis.Scaling.Orientation = AxisOrientation.MinMax;
			axis.TickLabelPos = TickLabelPosition.NextTo;
			axis.Scaling.LogScale = false;
			axis.Delete = false;
		}
	}
	#endregion
	#region ChartPrimaryHorizontalAxisRightToLeftCommand
	public class ChartPrimaryHorizontalAxisRightToLeftCommand : ChartModifyPrimaryAxisCommandBase {
		public ChartPrimaryHorizontalAxisRightToLeftCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisRightToLeftCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisRightToLeftCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartPrimaryHorizontalAxisRightToLeft; } }
		public override string ImageName { get { return "ChartHorizontalAxis_RightToLeft"; } }
		protected override bool IsHorizontalAxis { get { return true; } }
		protected override AxisDataType AxisType { get { return AxisDataType.Agrument; } }
		#endregion
		protected override bool IsChecked(Chart chart) {
			AxisBase axis = GetAxis(chart);
			return !axis.Delete && axis.Scaling.Orientation == AxisOrientation.MaxMin && axis.TickLabelPos != TickLabelPosition.None && !axis.Scaling.LogScale;
		}
		protected override void ModifyChart(Chart chart) {
			AxisBase axis = GetAxis(chart);
			axis.Scaling.Orientation = AxisOrientation.MaxMin;
			axis.TickLabelPos = TickLabelPosition.NextTo;
			axis.Scaling.LogScale = false;
			axis.Delete = false;
		}
	}
	#endregion
	#region ChartPrimaryHorizontalAxisHideLabelsCommand
	public class ChartPrimaryHorizontalAxisHideLabelsCommand : ChartModifyPrimaryAxisCommandBase {
		public ChartPrimaryHorizontalAxisHideLabelsCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisHideLabelsCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisHideLabelsCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartPrimaryHorizontalAxisHideLabels; } }
		public override string ImageName { get { return "ChartHorizontalAxis_WithoutLabeling"; } }
		protected override bool IsHorizontalAxis { get { return true; } }
		protected override AxisDataType AxisType { get { return AxisDataType.Agrument; } }
		#endregion
		protected override bool IsChecked(Chart chart) {
			AxisBase axis = GetAxis(chart);
			return !axis.Delete && axis.TickLabelPos == TickLabelPosition.None && !axis.Scaling.LogScale;
		}
		protected override void ModifyChart(Chart chart) {
			AxisBase axis = GetAxis(chart);
			axis.TickLabelPos = TickLabelPosition.None;
			axis.Scaling.LogScale = false;
			axis.Delete = false;
		}
	}
	#endregion
	#region ChartPrimaryHorizontalAxisDefaultCommand
	public class ChartPrimaryHorizontalAxisDefaultCommand : ChartModifyPrimaryAxisCommandBase {
		public ChartPrimaryHorizontalAxisDefaultCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisDefaultCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisDefaultCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartPrimaryHorizontalAxisDefault; } }
		public override string ImageName { get { return "ChartHorizontalAxis_Default"; } }
		protected override bool IsHorizontalAxis { get { return true; } }
		protected override AxisDataType AxisType { get { return AxisDataType.Value; } }
		#endregion
		protected override bool IsChecked(Chart chart) {
			AxisBase axis = GetAxis(chart);
			if (!(!axis.Delete && axis.Scaling.Orientation == AxisOrientation.MinMax && axis.TickLabelPos != TickLabelPosition.None))
				return false;
			ValueAxis valueAxis = axis as ValueAxis;
			if (valueAxis == null)
				return true;
			return valueAxis.DisplayUnit.UnitType == DisplayUnitType.None && !valueAxis.DisplayUnit.ShowLabel && !axis.Scaling.LogScale;
		}
		protected override void ModifyChart(Chart chart) {
			AxisBase axis = GetAxis(chart);
			axis.Scaling.Orientation = AxisOrientation.MinMax;
			axis.TickLabelPos = TickLabelPosition.NextTo;
			axis.Delete = false;
			axis.Scaling.LogScale = false;
			ValueAxis valueAxis = axis as ValueAxis;
			if (valueAxis != null) {
				valueAxis.DisplayUnit.UnitType = DisplayUnitType.None;
				valueAxis.DisplayUnit.ShowLabel = false;
			}
		}
	}
	#endregion
	#region ChartPrimaryHorizontalAxisScaleLogarithmCommand
	public class ChartPrimaryHorizontalAxisScaleLogarithmCommand : ChartModifyPrimaryAxisCommandBase {
		public ChartPrimaryHorizontalAxisScaleLogarithmCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisScaleLogarithmCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisScaleLogarithmCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartPrimaryHorizontalAxisScaleLogarithm; } }
		public override string ImageName { get { return "ChartHorizontalAxis_LogScale"; } }
		protected override bool IsHorizontalAxis { get { return true; } }
		protected override AxisDataType AxisType { get { return AxisDataType.Value; } }
		#endregion
		protected override bool IsChecked(Chart chart) {
			AxisBase axis = GetAxis(chart);
			return !axis.Delete && axis.Scaling.Orientation == AxisOrientation.MinMax && axis.TickLabelPos != TickLabelPosition.None && IsChecked(axis.Scaling);
		}
		protected override void ModifyChart(Chart chart) {
			AxisBase axis = GetAxis(chart);
			axis.Scaling.Orientation = AxisOrientation.MinMax;
			ModifyChartScaling(axis.Scaling);
			axis.TickLabelPos = TickLabelPosition.NextTo;
			axis.Delete = false;
		}
		protected virtual bool IsChecked(ScalingOptions scaling) {
			return scaling.LogScale == true && scaling.LogBase == 10;
		}
		protected virtual void ModifyChartScaling(ScalingOptions scaling) {
			scaling.LogScale = true;
			scaling.LogBase = 10;
		}
	}
	#endregion
	#region ChartPrimaryHorizontalAxisScaleThousandsCommand
	public class ChartPrimaryHorizontalAxisScaleThousandsCommand : ChartPrimaryAxisScaleCommandBase {
		public ChartPrimaryHorizontalAxisScaleThousandsCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisScaleThousandsCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisScaleThousandsCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartPrimaryHorizontalAxisScaleThousands; } }
		public override string ImageName { get { return "ChartHorizontalAxis_Thousands"; } }
		protected override bool IsHorizontalAxis { get { return true; } }
		protected override DisplayUnitType UnitType { get { return DisplayUnitType.Thousands; } }
		#endregion
	}
	#endregion
	#region ChartPrimaryHorizontalAxisScaleMillionsCommand
	public class ChartPrimaryHorizontalAxisScaleMillionsCommand : ChartPrimaryAxisScaleCommandBase {
		public ChartPrimaryHorizontalAxisScaleMillionsCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisScaleMillionsCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisScaleMillionsCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartPrimaryHorizontalAxisScaleMillions; } }
		public override string ImageName { get { return "ChartHorizontalAxis_Millions"; } }
		protected override bool IsHorizontalAxis { get { return true; } }
		protected override DisplayUnitType UnitType { get { return DisplayUnitType.Millions; } }
		#endregion
	}
	#endregion
	#region ChartPrimaryHorizontalAxisScaleBillionsCommand
	public class ChartPrimaryHorizontalAxisScaleBillionsCommand : ChartPrimaryAxisScaleCommandBase {
		public ChartPrimaryHorizontalAxisScaleBillionsCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisScaleBillionsCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisScaleBillionsCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartPrimaryHorizontalAxisScaleBillions; } }
		public override string ImageName { get { return "ChartHorizontalAxis_Billions"; } }
		protected override bool IsHorizontalAxis { get { return true; } }
		protected override DisplayUnitType UnitType { get { return DisplayUnitType.Billions; } }
		#endregion
	}
	#endregion
	#region ChartHidePrimaryVerticalAxisCommand
	public class ChartHidePrimaryVerticalAxisCommand : ChartHidePrimaryHorizontalAxisCommand {
		public ChartHidePrimaryVerticalAxisCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartHidePrimaryVerticalAxisCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartHidePrimaryVerticalAxisCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartHidePrimaryVerticalAxis; } }
		public override string ImageName { get { return "ChartVerticalAxis_None"; } }
		protected override bool IsHorizontalAxis { get { return false; } }
		#endregion
	}
	#endregion
	#region ChartPrimaryVerticalAxisLeftToRightCommand
	public class ChartPrimaryVerticalAxisLeftToRightCommand : ChartPrimaryHorizontalAxisLeftToRightCommand {
		public ChartPrimaryVerticalAxisLeftToRightCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisLeftToRightCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisLeftToRightCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartPrimaryVerticalAxisLeftToRight; } }
		public override string ImageName { get { return "ChartVerticalAxis_BottomUp"; } }
		protected override bool IsHorizontalAxis { get { return false; } }
		#endregion
	}
	#endregion
	#region ChartPrimaryVerticalAxisRightToLeftCommand
	public class ChartPrimaryVerticalAxisRightToLeftCommand : ChartPrimaryHorizontalAxisRightToLeftCommand {
		public ChartPrimaryVerticalAxisRightToLeftCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisRightToLeftCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisRightToLeftCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartPrimaryVerticalAxisRightToLeft; } }
		public override string ImageName { get { return "ChartVerticalAxis_TopDown"; } }
		protected override bool IsHorizontalAxis { get { return false; } }
		#endregion
	}
	#endregion
	#region ChartPrimaryVerticalAxisHideLabelsCommand
	public class ChartPrimaryVerticalAxisHideLabelsCommand : ChartPrimaryHorizontalAxisHideLabelsCommand {
		public ChartPrimaryVerticalAxisHideLabelsCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisHideLabelsCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisHideLabelsCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartPrimaryVerticalAxisHideLabels; } }
		public override string ImageName { get { return "ChartVerticallAxis_WithoutLabeling"; } }
		protected override bool IsHorizontalAxis { get { return false; } }
		#endregion
	}
	#endregion
	#region ChartPrimaryVerticalAxisDefaultCommand
	public class ChartPrimaryVerticalAxisDefaultCommand : ChartPrimaryHorizontalAxisDefaultCommand {
		public ChartPrimaryVerticalAxisDefaultCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisDefaultCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisDefaultCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartPrimaryVerticalAxisDefault; } }
		public override string ImageName { get { return "ChartVerticalAxis_Default"; } }
		protected override bool IsHorizontalAxis { get { return false; } }
		#endregion
	}
	#endregion
	#region ChartPrimaryVerticalAxisScaleLogarithmCommand
	public class ChartPrimaryVerticalAxisScaleLogarithmCommand : ChartPrimaryHorizontalAxisScaleLogarithmCommand {
		public ChartPrimaryVerticalAxisScaleLogarithmCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisScaleLogarithmCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisScaleLogarithmCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartPrimaryVerticalAxisScaleLogarithm; } }
		public override string ImageName { get { return "ChartVerticalAxis_LogScale"; } }
		protected override bool IsHorizontalAxis { get { return false; } }
		#endregion
	}
	#endregion
	#region ChartPrimaryVerticalAxisScaleThousandsCommand
	public class ChartPrimaryVerticalAxisScaleThousandsCommand : ChartPrimaryHorizontalAxisScaleThousandsCommand {
		public ChartPrimaryVerticalAxisScaleThousandsCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisScaleThousandsCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisScaleThousandsCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartPrimaryVerticalAxisScaleThousands; } }
		public override string ImageName { get { return "ChartVerticalAxis_Thousands"; } }
		protected override bool IsHorizontalAxis { get { return false; } }
		#endregion
	}
	#endregion
	#region ChartPrimaryVerticalAxisScaleMillionsCommand
	public class ChartPrimaryVerticalAxisScaleMillionsCommand : ChartPrimaryHorizontalAxisScaleMillionsCommand {
		public ChartPrimaryVerticalAxisScaleMillionsCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisScaleMillionsCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisScaleMillionsCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartPrimaryVerticalAxisScaleMillions; } }
		public override string ImageName { get { return "ChartVerticalAxis_Millions"; } }
		protected override bool IsHorizontalAxis { get { return false; } }
		#endregion
	}
	#endregion
	#region ChartPrimaryVerticalAxisScaleBillionsCommand
	public class ChartPrimaryVerticalAxisScaleBillionsCommand : ChartPrimaryHorizontalAxisScaleBillionsCommand {
		public ChartPrimaryVerticalAxisScaleBillionsCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisScaleBillionsCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisScaleBillionsCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartPrimaryVerticalAxisScaleBillions; } }
		public override string ImageName { get { return "ChartVerticalAxis_Billions"; } }
		protected override bool IsHorizontalAxis { get { return false; } }
		#endregion
	}
	#endregion
}
