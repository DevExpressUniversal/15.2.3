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
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Utils;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraRichEdit.SyntaxEdit;
using DevExpress.XtraRichEdit.SyntaxEdit.Services.Implementation;
using DevExpress.Utils.Commands;
using DevExpress.Services.Internal;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office;
namespace DevExpress.XtraReports.Design {
	public interface IScriptSource {
		ScriptLanguage ScriptLanguage { get; set; }
	}
	[ToolboxItem(false)] 
	public class SyntaxEditor : SyntaxEditControl, IScriptEditor, ISupportInitialize {
		#region inner classes
		public class ScriptEditorRichEditCommandFactoryService : RichEditCommandFactoryServiceWrapper {
			readonly Action0 scriptValidator;
			readonly IRichEditControl control;
			public ScriptEditorRichEditCommandFactoryService(IRichEditControl control, IRichEditCommandFactoryService service, Action0 scriptValidator)
				: base(service) {
				Guard.ArgumentNotNull(control, "control");
				this.control = control;
				this.scriptValidator = scriptValidator;
			}
			public override RichEditCommand CreateCommand(RichEditCommandId commandId) {
				if(commandId == RichEditCommandId.FileSave)
					return new ValidateScriptCommand(control, scriptValidator);
				else if(commandId == RichEditCommandId.ShiftTabKey)
					return new ScriptShiftTabKeyCommand(control);
				else if(commandId == RichEditCommandId.TabKey) {
					return new CustomInsertTabCommand(control);
				} else
					return base.CreateCommand(commandId);
			}
		}
		public class ValidateScriptCommand : SaveDocumentCommand {
			readonly Action0 scriptValidator;
			public ValidateScriptCommand(IRichEditControl control, Action0 scriptValidator)
				: base(control) {
				this.scriptValidator = scriptValidator;
			}
			public override void Execute() {
				scriptValidator();
			}
		}
		public class ScriptShiftTabKeyCommand : ShiftTabKeyCommand {
			public ScriptShiftTabKeyCommand(IRichEditControl control)
				: base(control) {
			}
			protected override void ExecuteCore() {
				ChangeIndent();
			}
			protected override DecrementIndentByTheTabCommand CreateDecrementIndentByTheTabCommand() {
				return new ScriptDecrementIndentByTheTabCommand(Control);
			}
		}
		public class CustomInsertTabCommand : InsertTabCommand {
			public CustomInsertTabCommand(IRichEditControl control)
				: base(control) {
			}
			protected override void ForceExecuteCore(ICommandUIState state) {
				if(DocumentModel.Selection.Length > 0)
					InsertTabsForAllRows();
				else
					base.ForceExecuteCore(state);
			}
			void InsertTabsForAllRows() {
				CustomInsertTabCoreCommand command = new CustomInsertTabCoreCommand(Control);
				command.Execute();
			}
		}
		public class ScriptDecrementIndentByTheTabCommand : DecrementIndentByTheTabCommand {
			public ScriptDecrementIndentByTheTabCommand(IRichEditControl control)
				: base(control) {
			}
			protected override void DecrementParagraphIndent() {
				DecrementParagraphIndentCommand command = CreateDecrementParagraphIndentCommand();
				command.ForceExecute(CreateDefaultCommandUIState());
			}
			protected override DecrementParagraphIndentCommand CreateDecrementParagraphIndentCommand() {
				return new ScriptDecrementParagraphIndentCommand(Control);
			}
		}
		public class ScriptDecrementParagraphIndentCommand : DecrementParagraphIndentCommand {
			public ScriptDecrementParagraphIndentCommand(IRichEditControl control)
				: base(control) {
			}
			protected override void ExecuteCore() {
				ScriptDeletePrevTabCommand command = new ScriptDeletePrevTabCommand(Control);
				command.Execute();
			}
		}
		public class CustomInsertTabCoreCommand : DevExpress.XtraRichEdit.Commands.Internal.InsertTabCoreCommand {
			public CustomInsertTabCoreCommand(IRichEditControl control)
				: base(control) {
			}
			protected override void InsertTextCore() {
				string insertedText = GetInsertedText();
				if(String.IsNullOrEmpty(insertedText))
					return;
				DevExpress.XtraRichEdit.Model.ParagraphList paragraphs = DocumentModel.Selection.GetSelectedParagraphs();
				for(int i = 0; i < paragraphs.Count; i++) {
					DevExpress.XtraRichEdit.Model.DocumentLogPosition insertPosition = paragraphs[new DevExpress.XtraRichEdit.Model.ParagraphIndex(i)].LogPosition;
					ActivePieceTable.InsertText(insertPosition, insertedText, GetForceVisible());
					OnTextInserted(insertPosition, insertedText.Length);
				}
			}
		}
		public class ScriptDeletePrevTabCommand : DeleteBackCommand {
			int oldSelectionLength = 0;
			public ScriptDeletePrevTabCommand(IRichEditControl control)
				: base(control) {
			}
			protected override void CreateCommands() {
				if(!IsPrevCharacterTab())
					return;
				oldSelectionLength = DocumentModel.Selection.Length;
				if(oldSelectionLength > 0)
					DocumentModel.Selection.End = DocumentModel.Selection.Start;
				Commands.Add(new SelectFieldPrevToCaretCommand(Control));
				Commands.Add(DocumentModel.CommandsCreationStrategy.CreateDeleteBackCoreCommand(Control));
			}
			public override void Execute() {
				base.Execute();
				if(oldSelectionLength > 0)
					DocumentModel.Selection.End += oldSelectionLength;
			}
			protected virtual bool IsPrevCharacterTab() {
				if(DocumentModel.Selection.NormalizedStart == DocumentLogPosition.Zero)
					return false;
				DocumentModelPosition currentPos = PositionConverter.ToDocumentModelPosition(DocumentModel.Selection.PieceTable, DocumentModel.Selection.NormalizedStart);
				DocumentModelPosition prevPos = DocumentModelPosition.MoveBackward(currentPos);
				TextRunBase run = DocumentModel.Selection.PieceTable.Runs[prevPos.RunIndex];
				string res = run.GetText(DocumentModel.Selection.PieceTable.TextBuffer, prevPos.RunOffset, prevPos.RunOffset);
				return String.Equals(res, Characters.TabMark.ToString());
			}
		}
		public class ScriptEditorCommandUIStateManagerService : SyntaxEditorCommandUIStateManagerService {
			protected override bool IsCommandAllowed(RichEditCommandId id) {
				if (id == RichEditCommandId.FileSave)
					return false;
				return base.IsCommandAllowed(id);
			}
		}
		#endregion
		int initCount;
		public SyntaxEditor(ISyntaxColors syntaxColors, Action0 scriptValidator)
			: base(syntaxColors) {
			ReplaceService<IRichEditCommandFactoryService>(new ScriptEditorRichEditCommandFactoryService(this, GetService<IRichEditCommandFactoryService>(), scriptValidator));
			ReplaceService<ICommandUIStateManagerService>(new ScriptEditorCommandUIStateManagerService());
		}
		void IScriptEditor.HighlightErrors(CompilerErrorCollection errors) {
			ShowErrorsCore(errors);
		}
		int IScriptEditor.LinesCount {
			get { return Document.Paragraphs.Count; }
		}
		void IScriptEditor.AppendText(string text) {
			Document.InsertText(Document.Range.End, text);
		}
		public void BeginInit() {
			initCount++;
		}
		public void EndInit() {
			initCount--;
		}
		public bool IsInitializing {
			get { return initCount > 0; }
		}
	}
}
