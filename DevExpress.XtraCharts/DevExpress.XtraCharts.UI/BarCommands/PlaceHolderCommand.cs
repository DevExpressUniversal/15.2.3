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
using System.Linq;
using System.Text;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
using DevExpress.Utils.Commands;
namespace DevExpress.XtraCharts.Commands {
	public abstract class PlaceHolderCommandBase : ChartCommand {
		public PlaceHolderCommandBase(IChartContainer control)
			: base(control) {
		}
		public override void ForceExecute(ICommandUIState state) {
			NotifyBeginCommandExecution(state);
			NotifyEndCommandExecution(state);
		}
	}
	public class CreateBarChartPlaceHolderCommand : PlaceHolderCommandBase {
		public CreateBarChartPlaceHolderCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateBarChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateBarChartPlaceHolderDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateBarChartPlaceHolderMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateBarChartPlaceHolder; } }
	}
	public class CreatePieChartPlaceHolderCommand : PlaceHolderCommandBase {
		public CreatePieChartPlaceHolderCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreatePieChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreatePieChartPlaceHolderDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreatePieChartPlaceHolderMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreatePieChartPlaceHolder; } }
	}
	public class CreateAreaChartPlaceHolderCommand : PlaceHolderCommandBase {
		public CreateAreaChartPlaceHolderCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateAreaChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateAreaChartPlaceHolderDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateAreaChartPlaceHolderMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateAreaChartPlaceHolder; } }
	}
	public class CreateLineChartPlaceHolderCommand : PlaceHolderCommandBase {
		public CreateLineChartPlaceHolderCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateLineChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateLineChartPlaceHolderDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateLineChartPlaceHolderMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateLineChartPlaceHolder; } }
	}
	public class CreateOtherSeriesTypesChartPlaceHolderCommand : PlaceHolderCommandBase {
		public CreateOtherSeriesTypesChartPlaceHolderCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateDoughnutChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateOtherSeriesTypesChartPlaceHolderDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateOtherSeriesTypesChartPlaceHolderMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateOtherSeriesTypesChartPlaceHolder; } }
	}
	public class CreateRotatedBarChartPlaceHolderCommand : PlaceHolderCommandBase {
		public CreateRotatedBarChartPlaceHolderCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateRotatedBarChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateRotatedBarChartPlaceHolderDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateRotatedBarChartPlaceHolderMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateRotatedBarChartPlaceHolder; } }
	}
	public class ChangePalettePlaceHolderCommand : PlaceHolderCommandBase {
		public ChangePalettePlaceHolderCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "ChangePalettePlaceHolder"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdChangePalettePlaceHolderDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdChangePalettePlaceHolderMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.ChangePalettePlaceHolder; } }
	}
	public class ChangeAppearancePlaceHolderCommand : PlaceHolderCommandBase {
		public ChangeAppearancePlaceHolderCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "ChangeAppearancePlaceHolder"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdChangeAppearancePlaceHolderDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdChangeAppearancePlaceHolderMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.ChangeAppearancePlaceHolder; } }
	}
	public class Column2DGroupPlaceHolderCommand : PlaceHolderCommandBase {
		public Column2DGroupPlaceHolderCommand(IChartContainer control)
			: base(control) {
		}
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdColumn2DGroupPlaceHolderMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.Column2DGroupPlaceHolder; } }
	}
	public class Column3DGroupPlaceHolderCommand : PlaceHolderCommandBase {
		public Column3DGroupPlaceHolderCommand(IChartContainer control)
			: base(control) {
		}
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdColumn3DGroupPlaceHolderMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.Column3DGroupPlaceHolder; } }
	}
	public class ColumnCylinderGroupPlaceHolderCommand : PlaceHolderCommandBase {
		public ColumnCylinderGroupPlaceHolderCommand(IChartContainer control)
			: base(control) {
		}
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdColumnCylinderGroupPlaceHolderMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.ColumnCylinderGroupPlaceHolder; } }
	}
	public class ColumnConeGroupPlaceHolderCommand : PlaceHolderCommandBase {
		public ColumnConeGroupPlaceHolderCommand(IChartContainer control)
			: base(control) {
		}
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdColumnConeGroupPlaceHolderMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.ColumnConeGroupPlaceHolder; } }
	}
	public class ColumnPyramidGroupPlaceHolderCommand : PlaceHolderCommandBase {
		public ColumnPyramidGroupPlaceHolderCommand(IChartContainer control)
			: base(control) {
		}
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdColumnPyramidGroupPlaceHolderMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.ColumnPyramidGroupPlaceHolder; } }
	}
	public class Line2DGroupPlaceHolderCommand : PlaceHolderCommandBase {
		public Line2DGroupPlaceHolderCommand(IChartContainer control)
			: base(control) {
		}
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdLine2DGroupPlaceHolderMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.Line2DGroupPlaceHolder; } }
	}
	public class Line3DGroupPlaceHolderCommand : PlaceHolderCommandBase {
		public Line3DGroupPlaceHolderCommand(IChartContainer control)
			: base(control) {
		}
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdLine3DGroupPlaceHolderMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.Line3DGroupPlaceHolder; } }
	}
	public class Pie2DGroupPlaceHolderCommand : PlaceHolderCommandBase {
		public Pie2DGroupPlaceHolderCommand(IChartContainer control)
			: base(control) {
		}
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdPie2DGroupPlaceHolderMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.Pie2DGroupPlaceHolder; } }
	}
	public class Pie3DGroupPlaceHolderCommand : PlaceHolderCommandBase {
		public Pie3DGroupPlaceHolderCommand(IChartContainer control)
			: base(control) {
		}
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdPie3DGroupPlaceHolderMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.Pie3DGroupPlaceHolder; } }
	}
	public class Bar2DGroupPlaceHolderCommand : PlaceHolderCommandBase {
		public Bar2DGroupPlaceHolderCommand(IChartContainer control)
			: base(control) {
		}
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdBar2DGroupPlaceHolderMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.Bar2DGroupPlaceHolder; } }
	}
	public class Area2DGroupPlaceHolderCommand : PlaceHolderCommandBase {
		public Area2DGroupPlaceHolderCommand(IChartContainer control)
			: base(control) {
		}
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdArea2DGroupPlaceHolderMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.Area2DGroupPlaceHolder; } }
	}
	public class Area3DGroupPlaceHolderCommand : PlaceHolderCommandBase {
		public Area3DGroupPlaceHolderCommand(IChartContainer control)
			: base(control) {
		}
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdArea3DGroupPlaceHolderMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.Area3DGroupPlaceHolder; } }
	}
	public class PointGroupPlaceHolderCommand : PlaceHolderCommandBase {
		public PointGroupPlaceHolderCommand(IChartContainer control)
			: base(control) {
		}
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdPointGroupPlaceHolderMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.PointGroupPlaceHolder; } }
	}
	public class FunnelGroupPlaceHolderCommand : PlaceHolderCommandBase {
		public FunnelGroupPlaceHolderCommand(IChartContainer control)
			: base(control) {
		}
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdFunnelGroupPlaceHolderMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.FunnelGroupPlaceHolder; } }
	}
	public class FinancialGroupPlaceHolderCommand : PlaceHolderCommandBase {
		public FinancialGroupPlaceHolderCommand(IChartContainer control)
			: base(control) {
		}
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdFinancialGroupPlaceHolderMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.FinancialGroupPlaceHolder; } }
	}
	public class RadarGroupPlaceHolderCommand : PlaceHolderCommandBase {
		public RadarGroupPlaceHolderCommand(IChartContainer control)
			: base(control) {
		}
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdRadarGroupPlaceHolderMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.RadarGroupPlaceHolder; } }
	}
	public class PolarGroupPlaceHolderCommand : PlaceHolderCommandBase {
		public PolarGroupPlaceHolderCommand(IChartContainer control)
			: base(control) {
		}
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdPolarGroupPlaceHolderMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.PolarGroupPlaceHolder; } }
	}
	public class RangeGroupPlaceHolderCommand : PlaceHolderCommandBase {
		public RangeGroupPlaceHolderCommand(IChartContainer control)
			: base(control) {
		}
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdRangeGroupPlaceHolderMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.RangeGroupPlaceHolder; } }
	}
	public class GanttGroupPlaceHolderCommand : PlaceHolderCommandBase {
		public GanttGroupPlaceHolderCommand(IChartContainer control)
			: base(control) {
		}
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdGanttGroupPlaceHolderMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.GanttGroupPlaceHolder; } }
	}
}
