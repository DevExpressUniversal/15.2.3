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

using DevExpress.Web.Office.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public static class CommandFactory {
		static Dictionary<CommandType, Type> CommandsDictionary = new Dictionary<CommandType, Type>() {
			{ CommandType.Start, typeof(StartCommand) },
			{ CommandType.OpenDocument, typeof(OpenDocumentCommand) },
			{ CommandType.SaveAsDocument, typeof(SaveAsDocumentCommand) },
			{ CommandType.SaveDocument, typeof(SaveDocumentCommand) },
			{ CommandType.NewDocument, typeof(NewDocumentCommand) },
			{ CommandType.DelayedPrint, typeof(DelayedPrintCommand) },
			{ CommandType.FixedLengthText, typeof(FixedLengthTextCommand) },
			{ CommandType.InsertSimpleRun, typeof(InsertTextCommand) },
			{ CommandType.InsertInlinePicture, typeof(InsertInlinePictureCommand) },
			{ CommandType.InsertParagraph, typeof(InsertParagraphCommand) },
			{ CommandType.InsertSection, typeof(InsertSectionCommand) },
			{ CommandType.LoadInlinePictures, typeof(LoadInlinePicturesCommand) },
			{ CommandType.UpdateInlinePictures, typeof(UpdateInlinePicturesCommand)},
			{ CommandType.ChangeCharacterProperties, typeof(ChangeCharacterFormattingCommand) },
			{ CommandType.ChangeCharacterPropertiesUseValue, typeof(ChangeCharacterFormattingUseValueCommand) },
			{ CommandType.ChangeParagraphProperties, typeof(ChangeParagraphFormattingCommand) },
			{ CommandType.ChangeParagraphPropertiesUseValue, typeof(ChangeParagraphFormattingUseValueCommand) },
			{ CommandType.ChangeInlineObjectProperties, typeof(ChangeInlineObjectCommand) },
			{ CommandType.ApplyCharacterStyle, typeof(ApplyCharacterStyleCommand) },
			{ CommandType.ApplyParagraphStyle, typeof(ApplyParagraphStyleCommand) },
			{ CommandType.ChangeSectionProperties, typeof(ChangeSectionFormattingCommand) },
			{ CommandType.ChangeTextBuffer, typeof(ChangeTextBufferCommand) },
			{ CommandType.InsertTabToParagraph, typeof(InsertTabToParagraphWebCommand) },
			{ CommandType.DeleteTabAtParagraph, typeof(DeleteTabAtParagraphWebCommand) },
			{ CommandType.ChangeIOverrideListLevel, typeof(ChangeIOverrideListLevelCommand) },
			{ CommandType.ChangeListLevelCharacterProperties, typeof(ChangeListLevelCharacterPropertiesCommand) },
			{ CommandType.ChangeListLevelParagraphProperties, typeof(ChangeListLevelParagraphPropertiesCommand) },
			{ CommandType.ChangeListLevelProperties, typeof(ChangeListLevelPropertiesCommand) },
			{ CommandType.ApplyNumberingList, typeof(ApplyNumberingListCommand) },
			{ CommandType.DeleteAbstractNumberingList, typeof(DeleteAbstractNumberingListCommand) },
			{ CommandType.DeleteNumberingList, typeof(DeleteNumberingListCommand) },
			{ CommandType.AddAbstractNumberingList, typeof(AddAbstractNumberingListCommand) },
			{ CommandType.AddNumberingList, typeof(AddNumberingListCommand) },
			{ CommandType.CreateStyleLink, typeof(CreateStyleLinkCommand) },
			{ CommandType.DeleteStyleLink, typeof(DeleteStyleLinkCommand) },
			{ CommandType.FieldUpdate, typeof(FieldUpdateCommand) },
			{ CommandType.InsertField, typeof(InsertFieldCommand) },
			{ CommandType.DeleteField, typeof(DeleteFieldCommand) },
			{ CommandType.HyperlinkInfoChanged, typeof(HyperlinkInfoChangedCommand) },
			{ CommandType.ReloadDocument, typeof(ReloadDocumentCommand) },
			{ CommandType.ChangeDefaultTabWidth, typeof(ChangeDefaultTabWidthCommand) },
			{ CommandType.ChangePageColor, typeof(ChangePageColorCommand) },
			{ CommandType.ChangeDifferentOddAndEvenPages, typeof(ChangeDifferentOddAndEvenPagesCommand) },
			{ CommandType.DeleteRuns, typeof(DeleteRunsCommand) },
			{ CommandType.MergeParagraphs, typeof(MergeParagraphsCommand) },
			{ CommandType.MergeSections, typeof(MergeSectionsCommand) },
			{ CommandType.CreateHeader, typeof(CreateHeaderCommand) },
			{ CommandType.CreateFooter, typeof(CreateFooterCommand) },
			{ CommandType.ChangeHeaderIndex, typeof(ChangeHeaderIndexCommand) },
			{ CommandType.ChangeFooterIndex, typeof(ChangeFooterIndexCommand) },
			{ CommandType.SaveMergedDocument, typeof(SaveMergedDocumentCommand) },
			{ CommandType.CreateBookmark, typeof(CreateBookmarkCommand) },
			{ CommandType.DeleteBookmark, typeof(DeleteBookmarkCommand) },
			{ CommandType.CreateTable, typeof(CreateTableCommand) },
			{ CommandType.ChangeTable, typeof(ChangeTableCommand) },
			{ CommandType.ChangeTableProperty, typeof(ChangeTablePropertyCommand) },
			{ CommandType.ChangeTableRow, typeof(ChangeTableRowCommand) },
			{ CommandType.ChangeTableRowProperty, typeof(ChangeTableRowPropertyCommand) },
			{ CommandType.ChangeTableCell, typeof(ChangeTableCellCommand) },
			{ CommandType.ChangeTableCellProperty, typeof(ChangeTableCellPropertyCommand) },
			{ CommandType.RemoveTable, typeof(RemoveTableCommand) },
			{ CommandType.ShiftTableStartPosition, typeof(ShiftTableStartPositionCommand) },
			{ CommandType.SplitTableCellHorizontally, typeof(SplitTableCellHorizontallyCommand) },
			{ CommandType.MergeTableCellHorizontally, typeof(MergeTableCellHorizontallyCommand) },
			{ CommandType.InsertTableRow, typeof(InsertTableRowCommand) },
			{ CommandType.RemoveTableRow, typeof(RemoveTableRowCommand) },
			{ CommandType.InsertTableCell, typeof(InsertTableCellCommand) },
			{ CommandType.RemoveTableCell, typeof(RemoveTableCellCommand) },
			{ CommandType.ApplyTableStyle, typeof(ApplyTableStyleCommand) }
		};
		public static WebRichEditCommand Create(CommandManager commandManager, Hashtable parameters) {
			CommandType type = (CommandType)parameters["type"];
			Type commandType;
			if (CommandsDictionary.TryGetValue(type, out commandType))
				return Activator.CreateInstance(commandType, new object[] { commandManager, parameters }) as WebRichEditCommand;
			throw new Exception("Command Not Found");
		}
		public static DocumentHandlerResponse ExecuteCommands(RichEditWorkSession workSession, Guid clientGuid, ASPxRichEdit control, NameValueCollection parameters) {
			var manager = new CommandManager(workSession, clientGuid, control);
			return manager.ExecuteCommands(parameters);
		}
	}
}
