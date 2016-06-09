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
	public class CreateCylinderBar3DChartCommand : CreateCylinderBar3DChartCommandBase {
		public CreateCylinderBar3DChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateCylinderBar3DChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateCylinderBar3DChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateCylinderBar3DChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateCylinderBar3DChart; } }
		protected override ViewType ChartViewType { get { return ViewType.Bar3D; } }
	}
	public class CreateCylinderFullStackedBar3DChartCommand : CreateCylinderBar3DChartCommandBase {
		public CreateCylinderFullStackedBar3DChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateCylinderFullStackedBar3DChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateCylinderFullStackedBar3DChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateCylinderFullStackedBar3DChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateCylinderFullStackedBar3DChart; } }
		protected override ViewType ChartViewType { get { return ViewType.FullStackedBar3D; } }
	}
	public class CreateCylinderSideBySideFullStackedBar3DChartCommand : CreateCylinderBar3DChartCommandBase {
		public CreateCylinderSideBySideFullStackedBar3DChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateCylinderSideBySideFullStackedBar3DChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateCylinderSideBySideFullStackedBar3DChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateCylinderSideBySideFullStackedBar3DChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateCylinderSideBySideFullStackedBar3DChart; } }
		protected override ViewType ChartViewType { get { return ViewType.SideBySideFullStackedBar3D; } }
	}
	public class CreateCylinderSideBySideStackedBar3DChartCommand : CreateCylinderBar3DChartCommandBase {
		public CreateCylinderSideBySideStackedBar3DChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateCylinderSideBySideStackedBar3DChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateCylinderSideBySideStackedBar3DChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateCylinderSideBySideStackedBar3DChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateCylinderSideBySideStackedBar3DChart; } }
		protected override ViewType ChartViewType { get { return ViewType.SideBySideStackedBar3D; } }
	}
	public class CreateCylinderStackedBar3DChartCommand : CreateCylinderBar3DChartCommandBase {
		public CreateCylinderStackedBar3DChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateCylinderStackedBar3DChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateCylinderStackedBar3DChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateCylinderStackedBar3DChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateCylinderStackedBar3DChart; } }
		protected override ViewType ChartViewType { get { return ViewType.StackedBar3D; } }
	}
	public class CreateCylinderManhattanBarChartCommand : CreateCylinderBar3DChartCommandBase {
		public CreateCylinderManhattanBarChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateCylinderManhattanBarChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateCylinderManhattanBarChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateCylinderManhattanBarChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateCylinderManhattanBarChart; } }
		protected override ViewType ChartViewType { get { return ViewType.ManhattanBar; } }
	}
}
