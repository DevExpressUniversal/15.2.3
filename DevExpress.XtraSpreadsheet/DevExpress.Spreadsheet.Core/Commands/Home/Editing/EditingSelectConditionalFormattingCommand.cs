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
using DevExpress.Utils.Commands;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.Office.Services.Implementation;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region EditingSelectConditionalFormattingCommand
	public class EditingSelectConditionalFormattingCommand : EditingSelectSpecificRangesCommand {
		public EditingSelectConditionalFormattingCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Fields
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.EditingSelectConditionalFormatting; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_EditingSelectConditionalFormatting; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_EditingSelectConditionalFormattingDescription; } }
		#endregion
		protected override IList<CellRangeBase> GetRanges() {
			List<CellRangeBase> result = new List<CellRangeBase>();
			foreach (ConditionalFormatting formatting in ActiveSheet.ConditionalFormattings)
				result.Add(formatting.CellRange.Clone());
			return result;
		}
	}
	#endregion
	#region EditingSelectSpecificRangesCommand (abstract class)
	public abstract class EditingSelectSpecificRangesCommand : SpreadsheetSelectionCommand {
		protected EditingSelectSpecificRangesCommand(ISpreadsheetControl control)
			: base(control) {
		}
		protected override bool ExpandsSelection { get { return false; } }
		protected internal override bool ChangeSelection() {
			IList<CellRangeBase> ranges = GetRanges();
			if (ranges == null || ranges.Count <= 0) {
				IThreadSyncService service = InnerControl.GetService<IThreadSyncService>();
				if (service != null)
					service.EnqueueInvokeInUIThread(new Action(delegate { Control.ShowWarningMessage(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_NoCellsWereFound)); }));
				return false;
			}
			Selection.SetSelectedRanges(ranges, true);
			return true;
		}
		protected abstract IList<CellRangeBase> GetRanges();
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentCapability.Default, !InnerControl.IsAnyInplaceEditorActive);
		}
	}
	#endregion
}
