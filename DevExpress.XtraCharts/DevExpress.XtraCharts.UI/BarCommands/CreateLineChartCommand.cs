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
namespace DevExpress.XtraCharts.Commands {
	public class CreateLineChartCommand : CreateLineChartCommandBase {
		public CreateLineChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateLineChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateLineChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateLineChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateLineChart; } }
		protected override ViewType ChartViewType { get { return ViewType.Line; } }
	}
	public class CreateLine3DChartCommand : CreateLineChartCommandBase {
		public CreateLine3DChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateLine3DChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateLine3DChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateLine3DChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateLine3DChart; } }
		protected override ViewType ChartViewType { get { return ViewType.Line3D; } }
	}
	public class CreateFullStackedLineChartCommand : CreateLineChartCommandBase {
		public CreateFullStackedLineChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateFullStackedLineChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateFullStackedLineChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateFullStackedLineChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateFullStackedLineChart; } }
		protected override ViewType ChartViewType { get { return ViewType.FullStackedLine; } }
	}
	public class CreateFullStackedLine3DChartCommand : CreateLineChartCommandBase {
		public CreateFullStackedLine3DChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateFullStackedLine3DChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateFullStackedLine3DChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateFullStackedLine3DChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateFullStackedLine3DChart; } }
		protected override ViewType ChartViewType { get { return ViewType.FullStackedLine3D; } }
	}
	public class CreateScatterLineChartCommand : CreateLineChartCommandBase {
		public CreateScatterLineChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateScatterLineChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateScatterLineChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateScatterLineChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateScatterLineChart; } }
		protected override ViewType ChartViewType { get { return ViewType.ScatterLine; } }
	}
	public class CreateSplineChartCommand : CreateLineChartCommandBase {
		public CreateSplineChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateSplineChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateSplineChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateSplineChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateSplineChart; } }
		protected override ViewType ChartViewType { get { return ViewType.Spline; } }
	}
	public class CreateSpline3DChartCommand : CreateLineChartCommandBase {
		public CreateSpline3DChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateSpline3DChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateSpline3DChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateSpline3DChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateSpline3DChart; } }
		protected override ViewType ChartViewType { get { return ViewType.Spline3D; } }
	}
	public class CreateStackedLineChartCommand : CreateLineChartCommandBase {
		public CreateStackedLineChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateStackedLineChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateStackedLineChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateStackedLineChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateStackedLineChart; } }
		protected override ViewType ChartViewType { get { return ViewType.StackedLine; } }
	}
	public class CreateStackedLine3DChartCommand : CreateLineChartCommandBase {
		public CreateStackedLine3DChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateStackedLine3DChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateStackedLine3DChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateStackedLine3DChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateStackedLine3DChart; } }
		protected override ViewType ChartViewType { get { return ViewType.StackedLine3D; } }
	}
	public class CreateStepLineChartCommand : CreateLineChartCommandBase {
		public CreateStepLineChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateStepLineChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateStepLineChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateStepLineChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateStepLineChart; } }
		protected override ViewType ChartViewType { get { return ViewType.StepLine; } }
	}
	public class CreateStepLine3DChartCommand : CreateLineChartCommandBase {
		public CreateStepLine3DChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateStepLine3DChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateStepLine3DChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateStepLine3DChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateStepLine3DChart; } }
		protected override ViewType ChartViewType { get { return ViewType.StepLine3D; } }
	}
}
