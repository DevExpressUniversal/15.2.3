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
	 public class CreatePyramidBar3DChartCommand : CreatePyramidBar3DChartCommandBase {
		public CreatePyramidBar3DChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreatePyramidBar3DChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreatePyramidBar3DChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreatePyramidBar3DChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreatePyramidBar3DChart; } }
		protected override ViewType ChartViewType { get { return ViewType.Bar3D; } }
	}
	public class CreatePyramidFullStackedBar3DChartCommand : CreatePyramidBar3DChartCommandBase {
		public CreatePyramidFullStackedBar3DChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreatePyramidFullStackedBar3DChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreatePyramidFullStackedBar3DChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreatePyramidFullStackedBar3DChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreatePyramidFullStackedBar3DChart; } }
		protected override ViewType ChartViewType { get { return ViewType.FullStackedBar3D; } }
	}
	public class CreatePyramidSideBySideFullStackedBar3DChartCommand : CreatePyramidBar3DChartCommandBase {
		public CreatePyramidSideBySideFullStackedBar3DChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreatePyramidSideBySideFullStackedBar3DChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreatePyramidSideBySideFullStackedBar3DChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreatePyramidSideBySideFullStackedBar3DChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreatePyramidSideBySideFullStackedBar3DChart; } }
		protected override ViewType ChartViewType { get { return ViewType.SideBySideFullStackedBar3D; } }
	}
	public class CreatePyramidSideBySideStackedBar3DChartCommand : CreatePyramidBar3DChartCommandBase {
		public CreatePyramidSideBySideStackedBar3DChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreatePyramidSideBySideStackedBar3DChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreatePyramidSideBySideStackedBar3DChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreatePyramidSideBySideStackedBar3DChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreatePyramidSideBySideStackedBar3DChart; } }
		protected override ViewType ChartViewType { get { return ViewType.SideBySideStackedBar3D; } }
	}
	public class CreatePyramidStackedBar3DChartCommand : CreatePyramidBar3DChartCommandBase {
		public CreatePyramidStackedBar3DChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreatePyramidStackedBar3DChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreatePyramidStackedBar3DChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreatePyramidStackedBar3DChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreatePyramidStackedBar3DChart; } }
		protected override ViewType ChartViewType { get { return ViewType.StackedBar3D; } }
	}
	public class CreatePyramidManhattanBarChartCommand : CreatePyramidBar3DChartCommandBase {
		public CreatePyramidManhattanBarChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreatePyramidManhattanBarChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreatePyramidManhattanBarChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreatePyramidManhattanBarChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreatePyramidManhattanBarChart; } }
		protected override ViewType ChartViewType { get { return ViewType.ManhattanBar; } }
	}
}
