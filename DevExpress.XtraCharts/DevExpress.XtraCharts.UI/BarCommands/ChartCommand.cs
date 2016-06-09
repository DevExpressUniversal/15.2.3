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
using System.Runtime.InteropServices;
using DevExpress.Utils.Commands;
using DevExpress.Utils;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Localization;
using DevExpress.Utils.Localization;
using System.Reflection;
namespace DevExpress.XtraCharts.Commands {
	public abstract class ChartCommand : ControlCommand<IChartContainer, ChartCommandId, ChartStringId> {
		protected ChartCommand(IChartContainer control)
			: base(control) {
		}
		#region Properties
		public override ChartCommandId Id { get { return ChartCommandId.None; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdEmptyDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdEmptyMenuCaption; } }
		protected internal DataContainer DataContainer { get { return Chart.DataContainer; } }
		protected internal Chart Chart { get { return Control.Chart; } }
		protected override XtraLocalizer<ChartStringId> Localizer { get { return ChartLocalizer.Active; } }
		protected override string ImageResourcePrefix { get { return "DevExpress.XtraCharts.Images.Commands"; } }
		protected override Assembly ImageResourceAssembly { get { return Assembly.GetAssembly(typeof(DevExpress.XtraCharts.Native.Chart)); } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			NotifyBeginCommandExecution(state);
			try {
				ExecuteCore(state);
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		protected virtual void ExecuteCore(ICommandUIState state) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = Chart != null;
		}
	}
}
