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

using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
using System;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Commands {
	class ModifyPivotTableStylesCommand : ModifyPivotTableCommandBase {
		public ModifyPivotTableStylesCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public string StyleName { get; set; }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ModifyPivotTableStyles; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			IValueBasedCommandUIState<string> valueBasedState = state as IValueBasedCommandUIState<string>;
			if (valueBasedState == null)
				return;
			StyleName = valueBasedState.Value;
			base.ForceExecute(state);
		}
		protected internal override void ExecuteCore() {
			DocumentModel.ActiveSheet.ApplyStyleToActiveTableBases(StyleName, true);
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			return new DefaultValueBasedCommandUIState<string>();
		}
		protected override void ModifyPivotTable(PivotTable table) {
			table.StyleInfo.StyleName = StyleName;
		}
		protected override void ModifyState(PivotTable table, ICommandUIState state) {
			state.Checked = IsChecked(table);
			IValueBasedCommandUIState<string> valueBasedState = state as IValueBasedCommandUIState<string>;
			if (valueBasedState != null) {
				if (table.StyleInfo.Style == null)
					valueBasedState.Value = String.Empty;
				else
					valueBasedState.Value = table.StyleInfo.Style.Name.Name;
			}
		}
		protected override bool IsChecked(PivotTable table) {
			if (table.StyleInfo.Style == null)
				return false;
			return table.StyleInfo.Style.Name.IsEquals(StyleName);
		}
	}
}
