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
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.Services.Internal;
namespace DevExpress.XtraRichEdit.SyntaxEdit.Services.Implementation {
	public class SyntaxEditorCommandUIStateManagerService : ICommandUIStateManagerService {
		static Dictionary<RichEditCommandId, bool> forbiddenCommands = CreateForbiddenCommandsList();
		public static Dictionary<RichEditCommandId, bool> CreateForbiddenCommandsList() {
			Dictionary<RichEditCommandId, bool> result = new Dictionary<RichEditCommandId, bool>();
			result.Add(RichEditCommandId.ToggleShowWhitespace, false);
			result.Add(RichEditCommandId.ToggleFontBold, false);
			result.Add(RichEditCommandId.ToggleFontItalic, false);
			result.Add(RichEditCommandId.ToggleFontUnderline, false);
			result.Add(RichEditCommandId.ToggleFontDoubleUnderline, false);
			result.Add(RichEditCommandId.ToggleFontStrikeout, false);
			result.Add(RichEditCommandId.ToggleFontDoubleStrikeout, false);
			result.Add(RichEditCommandId.IncreaseFontSize, false);
			result.Add(RichEditCommandId.DecreaseFontSize, false);
			result.Add(RichEditCommandId.IncrementFontSize, false);
			result.Add(RichEditCommandId.DecrementFontSize, false);
			result.Add(RichEditCommandId.ToggleFontSuperscript, false);
			result.Add(RichEditCommandId.ToggleFontSubscript, false);
			result.Add(RichEditCommandId.ShowFontForm, false);
			result.Add(RichEditCommandId.ShowParagraphForm, false);
			result.Add(RichEditCommandId.ToggleParagraphAlignmentLeft, false);
			result.Add(RichEditCommandId.ToggleParagraphAlignmentCenter, false);
			result.Add(RichEditCommandId.ToggleParagraphAlignmentRight, false);
			result.Add(RichEditCommandId.ToggleParagraphAlignmentJustify, false);
			result.Add(RichEditCommandId.ShowSymbolForm, false);
			result.Add(RichEditCommandId.FileNew, false);
			result.Add(RichEditCommandId.FileOpen, false);
			result.Add(RichEditCommandId.FileSaveAs, false);
			result.Add(RichEditCommandId.ZoomIn, false);
			result.Add(RichEditCommandId.ZoomOut, false);
			result.Add(RichEditCommandId.InsertPicture, false);
			result.Add(RichEditCommandId.Print, false);
			result.Add(RichEditCommandId.QuickPrint, false);
			result.Add(RichEditCommandId.PrintPreview, false);
			result.Add(RichEditCommandId.SwitchToDraftView, false);
			result.Add(RichEditCommandId.SwitchToPrintLayoutView, false);
			result.Add(RichEditCommandId.SwitchToSimpleView, false);
			result.Add(RichEditCommandId.InsertLineBreak, false);
			result.Add(RichEditCommandId.InsertPageBreak, false);
			result.Add(RichEditCommandId.InsertNonBreakingSpace, false);
			result.Add(RichEditCommandId.InsertColumnBreak, false);
			result.Add(RichEditCommandId.SetSingleParagraphSpacing, false);
			result.Add(RichEditCommandId.SetDoubleParagraphSpacing, false);
			result.Add(RichEditCommandId.SetSesquialteralParagraphSpacing, false);
			result.Add(RichEditCommandId.IncrementNumerationFromParagraph, false);
			result.Add(RichEditCommandId.DecrementNumerationFromParagraph, false);
			result.Add(RichEditCommandId.IncreaseIndent, false);
			result.Add(RichEditCommandId.DecreaseIndent, false);
			result.Add(RichEditCommandId.ToggleNumberingListItem, false);
			result.Add(RichEditCommandId.ToggleMultilevelListItem, false);
			result.Add(RichEditCommandId.ToggleBulletedListItem, false);
			result.Add(RichEditCommandId.ClearFormatting, false);
			result.Add(RichEditCommandId.CreateField, false);
			result.Add(RichEditCommandId.UpdateField, false);
			result.Add(RichEditCommandId.ToggleFieldCodes, false);
			result.Add(RichEditCommandId.ShowInsertMergeFieldForm, false);
			result.Add(RichEditCommandId.ToggleViewMergedData, false);
			result.Add(RichEditCommandId.ShowAllFieldCodes, false);
			result.Add(RichEditCommandId.ShowAllFieldResults, false);
			result.Add(RichEditCommandId.ShowNumberingListForm, false);
			result.Add(RichEditCommandId.ShowHyperlinkForm, false);
			return result;
		}
		public virtual void UpdateCommandUIState(Command command, ICommandUIState state) {
			if (state.Enabled)
				state.Enabled = IsRegisteredCommand(command);
		}
		protected virtual bool IsRegisteredCommand(Command command) {
			RichEditCommand richEditCommand = command as RichEditCommand;
			return richEditCommand != null && IsCommandAllowed(richEditCommand.Id);
		}
		protected virtual bool IsCommandAllowed(RichEditCommandId id) {
			return !forbiddenCommands.ContainsKey(id);
		}
	}
}
