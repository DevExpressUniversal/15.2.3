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
using DevExpress.XtraRichEdit.Localization;
using System.ComponentModel;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Commands {
	#region ShowTableOptionsFormCommand
	public class ShowTableOptionsFormCommand : RichEditMenuItemSimpleCommand {
		public ShowTableOptionsFormCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowTableOptionsFormCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ShowTableOptionsForm; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowTableOptionsFormCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ShowTableOptionsForm; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowTableOptionsFormCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ShowTableOptionsFormDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowTableOptionsFormCommandImageName")]
#endif
		public override string ImageName { get { return "TableCellMargins"; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowTableOptionsFormCommandShowsModalDialog")]
#endif
		public override bool ShowsModalDialog { get { return true; } }
		#endregion
		protected internal override void ExecuteCore() {
			Table table = DocumentModel.Selection.SelectedCells.FirstSelectedCell.Table;
			Control.ShowTableOptionsForm(table);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = DocumentModel.Selection.IsWholeSelectionInOneTable();
			ApplyCommandRestrictionOnEditableControl(state, DocumentModel.DocumentCapabilities.Tables, state.Enabled);
			ApplyDocumentProtectionToSelectedParagraphs(state);
		}
	}
	#endregion
}
