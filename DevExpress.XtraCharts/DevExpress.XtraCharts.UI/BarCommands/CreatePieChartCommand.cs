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
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts.Commands {
	public class CreatePieChartCommand : CreateSimpleDiagramChartCommandBase {
		public CreatePieChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreatePieChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreatePieChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreatePieChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreatePieChart; } }
		protected override ViewType ChartViewType { get { return ViewType.Pie; } }
	}
	public class CreatePie3DChartCommand : CreateSimpleDiagramChartCommandBase {
		public CreatePie3DChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreatePie3DChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreatePie3DChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreatePie3DChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreatePie3DChart; } }
		protected override ViewType ChartViewType { get { return ViewType.Pie3D; } }
	}
	public class CreateDoughnutChartCommand : CreateSimpleDiagramChartCommandBase {
		public CreateDoughnutChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateDoughnutChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateDoughnutChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateDoughnutChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateDoughnutChart; } }
		protected override ViewType ChartViewType { get { return ViewType.Doughnut; } }
	}
	public class CreateNestedDoughnutChartCommand : CreateSimpleDiagramChartCommandBase {
		public CreateNestedDoughnutChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateNestedDoughnutChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateNestedDoughnutChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateNestedDoughnutChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateNestedDoughnutChart; } }
		protected override ViewType ChartViewType { get { return ViewType.NestedDoughnut; } }
		protected override void CreateFakeChart() {
			Series series1 = new Series("", ChartViewType);
			Series series2 = new Series("", ChartViewType);
			Chart.Series.Add(series1);
			Chart.Series.Add(series2);
		}
	}
	public class CreateDoughnut3DChartCommand : CreateSimpleDiagramChartCommandBase {
		public CreateDoughnut3DChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateDoughnut3DChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateDoughnut3DChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateDoughnut3DChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateDoughnut3DChart; } }
		protected override ViewType ChartViewType { get { return ViewType.Doughnut3D; } }
	}
}
