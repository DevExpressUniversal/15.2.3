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
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Utils.Commands;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region FormatRemoveHyperlinksCommand
	public class FormatRemoveHyperlinksCommand : ClearSelectedCellsCommand {
		public FormatRemoveHyperlinksCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.FormatRemoveHyperlinks; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FormatRemoveHyperlinks; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FormatRemoveHyperlinksDescription; } }
		protected override bool UseOfficeImage { get { return true; } }
		public override string ImageName { get { return "Delete_Hyperlink"; } }
		#endregion
		protected internal override void ModifyRange(CellRange range) {
			ActiveSheet.ClearHyperlinks(range, true);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentCapability.Default, !InnerControl.IsInplaceEditorActive && IsHyperlinkSelected());
			ApplyActiveSheetProtection(state);
		}
		protected internal virtual bool IsHyperlinkSelected() {
			IList<CellRange> selectedRanges = ActiveSheet.Selection.SelectedRanges;
			foreach (CellRange range in selectedRanges) {
				if (IsHyperlinkSelectedCore(range))
					return true;
			}
			return false;
		}
		protected internal virtual bool IsHyperlinkSelectedCore(CellRange selectionRange) {
			ModelHyperlinkCollection hyperlinks = ActiveSheet.Hyperlinks;
			int count = hyperlinks.Count;
			for (int i = 0; i < count; i++) {
				ModelHyperlink hyperlink = hyperlinks[i];
				CellRange hyperlinkRange = hyperlink.Range;
				if (selectionRange.Includes(hyperlinkRange) || hyperlinkRange.Includes(selectionRange))
					return true;
			}
			return false;
		}
	}
	#endregion
}
