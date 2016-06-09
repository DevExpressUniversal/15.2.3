﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.Utils;
using System.Diagnostics;
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	#region ModifyChartStyleCommand
	public class ModifyChartStyleCommand : ModifyChartCommandBase {
		public ModifyChartStyleCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public int StyleId { get; set; }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ModifyChartStyle; } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			IValueBasedCommandUIState<int> valueBasedState = state as IValueBasedCommandUIState<int>;
			if (valueBasedState == null)
				return;
			this.StyleId = valueBasedState.Value;
			base.ForceExecute(state);
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			return new DefaultValueBasedCommandUIState<int>();
		}
		protected override bool IsChecked(Chart chart) {
			return chart.Style == this.StyleId;
		}
		protected override bool ShouldHideCommand(Chart chart) {
			return false;
		}
		protected override bool CanModifyChart(Chart chart) {
			return true;
		}
		protected override void ModifyChart(Chart chart) {
			chart.Style = StyleId;
			chart.ResetToStyle();
		}
	}
	#endregion
}
