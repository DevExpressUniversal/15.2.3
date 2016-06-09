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
	public class CreateStockChartCommand : CreateFinancialChartCommandBase {
		public CreateStockChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateStockChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateStockChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateStockChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateStockChart; } }
		protected override ViewType ChartViewType { get { return ViewType.Stock; } }
	}
	public class CreateCandleStickChartCommand : CreateFinancialChartCommandBase {
		public CreateCandleStickChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateCandleStickChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateCandleStickChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateCandleStickChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateCandleStickChart; } }
		protected override ViewType ChartViewType { get { return ViewType.CandleStick; } }
	}
}
