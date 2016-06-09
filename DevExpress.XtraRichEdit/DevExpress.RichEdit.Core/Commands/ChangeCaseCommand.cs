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
using System.ComponentModel;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit;
using DevExpress.Office.Utils;
namespace DevExpress.XtraRichEdit.Commands {
	#region ChangeCaseCommandBase (abstract class)
	public abstract class ChangeCaseCommandBase : ChangeCharacterPropertiesCommandBase {
		protected ChangeCaseCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected internal override DocumentModelChangeActions ChangeProperty(DocumentModelPosition start, DocumentModelPosition end, ICommandUIState state) {
			RunChangeCaseModifierBase modifier = CreateModifier();
			return ChangeCharacterFormatting(start.LogPosition, end.LogPosition, modifier);
		}
		protected internal override void ChangeInputPositionCharacterFormatting(RunPropertyModifierBase modifier) {
		}
		protected internal abstract RunChangeCaseModifierBase CreateModifier();
	}
	#endregion
	#region ChangeTextCasePlaceholderCommand
	public class ChangeTextCasePlaceholderCommand : ChangeCaseCommandBase, IPlaceholderCommand {
		public ChangeTextCasePlaceholderCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeTextCase; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeTextCaseDescription; } }
		public override RichEditCommandId Id { get { return RichEditCommandId.ChangeTextCasePlaceholder; } }
		public override string ImageName { get { return "ChangeTextCase"; } }
		public override void ForceExecute(ICommandUIState state) {
		}
		protected internal override RunChangeCaseModifierBase CreateModifier() {
			return null;
		}
	}
	#endregion
	#region MakeTextUpperCaseCommand
	public class MakeTextUpperCaseCommand : ChangeCaseCommandBase {
		public MakeTextUpperCaseCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("MakeTextUpperCaseCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_MakeTextUpperCase; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("MakeTextUpperCaseCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_MakeTextUpperCaseDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("MakeTextUpperCaseCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.MakeTextUpperCase; } }
		protected internal override RunChangeCaseModifierBase CreateModifier() {
			return new RunMakeUpperCaseModifier();
		}
	}
	#endregion
	#region MakeTextLowerCaseCommand
	public class MakeTextLowerCaseCommand : ChangeCaseCommandBase {
		public MakeTextLowerCaseCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("MakeTextLowerCaseCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_MakeTextLowerCase; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("MakeTextLowerCaseCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_MakeTextLowerCaseDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("MakeTextLowerCaseCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.MakeTextLowerCase; } }
		protected internal override RunChangeCaseModifierBase CreateModifier() {
			return new RunMakeLowerCaseModifier();
		}
	}
	#endregion
	#region ToggleTextCaseCommand
	public class ToggleTextCaseCommand : ChangeCaseCommandBase {
		public ToggleTextCaseCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleTextCaseCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleTextCase; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleTextCaseCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleTextCaseDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleTextCaseCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ToggleTextCase; } }
		protected internal override RunChangeCaseModifierBase CreateModifier() {
			return new RunToggleCaseModifier();
		}
	}
	#endregion
	#region CapitalizeEachWordTextCaseCommand
	public class CapitalizeEachWordCaseCommand : ChangeCaseCommandBase {
		public CapitalizeEachWordCaseCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("CapitalizeEachWordCaseCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_CapitalizeEachWordTextCase; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("CapitalizeEachWordCaseCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_CapitalizeEachWordTextCaseDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("CapitalizeEachWordCaseCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.CapitalizeEachWordTextCase; } }
		protected internal override RunChangeCaseModifierBase CreateModifier() {
			return new RunCapitalizeEachWordCaseModifier();
		}
	}
	#endregion
	public class ChangeCaseCommand : ChangeCaseCommandBase {
		public ChangeCaseCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeCaseCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeTextCase; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeCaseCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeTextCaseDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeCaseCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ChangeCase; } }
		protected internal override RunChangeCaseModifierBase CreateModifier() {
			List<SelectionItem> selectionItems = GetSelectionItems();
			if (selectionItems.Count == 0)
				return new RunMakeLowerCaseModifier();
			SelectionItem item = selectionItems[0];
			DocumentModelPosition start = CalculateStartPosition(item, true);
			DocumentModelPosition end = CalculateEndPosition(item, true);
			RunInfo rinfo = ActivePieceTable.ObtainAffectedRunInfo(start.LogPosition, end.LogPosition - start.LogPosition);
			if (rinfo.Start.RunIndex < new RunIndex(0) || rinfo.End.RunIndex < rinfo.Start.RunIndex)
				return new RunMakeLowerCaseModifier();
			ParagraphIndex endParagraphIndex = rinfo.End.ParagraphIndex;
			bool? prevCharIsUpper = null;
			ChunkedStringBuilder textBuffer = ActivePieceTable.TextBuffer;
			for (ParagraphIndex pIndex = rinfo.Start.ParagraphIndex; pIndex <= endParagraphIndex; pIndex++) {
				Paragraph paragraph = ActivePieceTable.Paragraphs[pIndex];
				RunIndex lastRunIndex = Algorithms.Min(rinfo.End.RunIndex, paragraph.LastRunIndex);
				RunIndex firstRunIndex = Algorithms.Max(rinfo.Start.RunIndex, paragraph.FirstRunIndex);
				for (RunIndex rIndex = firstRunIndex; rIndex <= lastRunIndex; rIndex++) {
					if (!ActivePieceTable.VisibleTextFilter.IsRunVisible(rIndex))
						continue;
					TextRunBase run = ActivePieceTable.Runs[rIndex];
					for (int chIndex = run.StartIndex + run.Length - 1; chIndex >= run.StartIndex; chIndex--) {
						char ch = textBuffer[chIndex];
						if (!char.IsLetter(ch))
							continue;
						if (!prevCharIsUpper.HasValue)
							prevCharIsUpper = char.IsUpper(ch);
						if (prevCharIsUpper.Value != char.IsUpper(ch))
							return new RunMakeUpperCaseModifier();
					}
				}
			}
			if (!prevCharIsUpper.HasValue)
				return new RunMakeLowerCaseModifier();
			if (prevCharIsUpper.Value)
				return new RunMakeLowerCaseModifier();
			return new RunCapitalizeEachWordCaseModifier();
		}
	}
}
