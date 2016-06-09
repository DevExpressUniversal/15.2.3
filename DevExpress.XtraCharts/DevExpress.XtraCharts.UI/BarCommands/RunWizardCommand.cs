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
using DevExpress.Utils.Commands;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Wizard;
using DevExpress.XtraCharts.Designer;
namespace DevExpress.XtraCharts.Commands {
	public class RunWizardCommand : ChartCommand {
		public override string ImageName { get { return "RunWizard"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdRunWizardDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdRunWizardMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.RunWizard; } }
		public RunWizardCommand(IChartContainer control)
			: base(control) {
		}
		protected override void ExecuteCore(ICommandUIState state) {
			ChartWizard wizard = new ChartWizard(Control);
			wizard.ShowDialog();
		}
	}
	public class RunDesignerCommand : ChartCommand {
		public override string ImageName { get { return "RunDesigner"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdRunDesignerDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdRunDesignerMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.RunDesigner; } }
		public RunDesignerCommand(IChartContainer control)
			: base(control) {
		}
		protected override void ExecuteCore(ICommandUIState state) {
			ChartDesigner designer = new ChartDesigner(Control);
			designer.ShowDialog();
		}
	}
}
