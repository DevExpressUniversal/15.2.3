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
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Native;
using System.ComponentModel;
namespace DevExpress.XtraRichEdit.Commands {
	#region ToggleShowWhitespaceCommand
	public class ToggleShowWhitespaceCommand : RichEditMenuItemSimpleCommand {
		public ToggleShowWhitespaceCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleShowWhitespaceCommandId")]
#endif
public override RichEditCommandId Id { get { return RichEditCommandId.ToggleShowWhitespace; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleShowWhitespaceCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleWhitespace; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleShowWhitespaceCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleWhitespaceDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleShowWhitespaceCommandImageName")]
#endif
		public override string ImageName { get { return "ShowHidden"; } }
		#endregion
		protected internal override void ExecuteCore() {
			Control.BeginUpdate();
			try {
				DocumentModel.BeginUpdate();
				try {
					DocumentModel.FormattingMarkVisibilityOptions.ShowHiddenText = !DocumentModel.FormattingMarkVisibilityOptions.ShowHiddenText;
					EnsureSelectionVisible();
				}
				finally {
					DocumentModel.EndUpdate();
				}
			}
			finally {
				Control.EndUpdate();
			}
		}
		protected virtual void EnsureSelectionVisible() {
			RunInfo selectionInterval = DocumentModel.Selection.Interval;
			DocumentModelPosition start = selectionInterval.Start;
			DocumentModelPosition end = selectionInterval.End;
			DocumentLogPosition startLogPosition = start.LogPosition;
			DocumentLogPosition endLogPosition = end.LogPosition;
			IVisibleTextFilter textFilter = ActivePieceTable.NavigationVisibleTextFilter;
			if (!textFilter.IsRunVisible(start.RunIndex))
				startLogPosition = textFilter.GetVisibleLogPosition(start);
			if (!textFilter.IsRunVisible(end.RunIndex))
				endLogPosition = textFilter.GetVisibleLogPosition(end);
			DocumentModel.Selection.Start = startLogPosition;
			DocumentModel.Selection.End = endLogPosition;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = true;
			state.Visible = true;
			state.Checked = DocumentModel.FormattingMarkVisibilityOptions.ShowHiddenText;
		}
	}
	#endregion
}
