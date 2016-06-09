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
namespace DevExpress.XtraRichEdit.Model {
	#region DocumentModelChangeActions
	[Flags]
	public enum DocumentModelChangeActions {
		None = 0x00000000,
		Redraw = 0x00000001,
		ResetPrimaryLayout = 0x00000002,
		ResetAllPrimaryLayout = 0x00000004,
		ResetSecondaryLayout = 0x00000008,
		ResetSelectionLayout = 0x00000010,
		ResetCaretInputPositionFormatting = 0x00000020,
		RaiseSelectionChanged = 0x00000040,
		RaiseContentChanged = 0x00000080,
		ScrollToBeginOfDocument = 0x00000100,
		RaiseEmptyDocumentCreated = 0x00000200,
		RaiseDocumentLoaded = 0x00000400,
		RaiseModifiedChanged = 0x00000800,
		ResetUncheckedIntervals = 0x00002000,
		ForceResize = 0x00004000,
		ValidateSelectionInterval = 0x00008000,
		PerformActionsOnIdle = 0x00010000,
		SplitRunByCharset = 0x00020000,
		ResetRuler = 0x00040000,
		SuppressBindingsNotifications = 0x00080000,
		ActivePieceTableChanged = 0x00100000,
		RaiseDocumentProtectionChanged = 0x00200000,
		ForceResetHorizontalRuler = 0x00400000,
		ForceResetVerticalRuler = 0x00800000,
		ForceSyntaxHighlight = 0x01000000,
		ApplyAutoCorrect = 0x02000000,
		SuppressRaiseContentChangedCalculationByCurrentTransactionChanges = 0x04000000,
		Fields = 0x08000000,
		RaiseCommentContentChange = 0x10000000,
		DeleteComment = 0x20000000
	}
	#endregion
	#region DocumentModelChangeType
	public enum DocumentModelChangeType {
		None = 0,
		InsertParagraph,
		InsertText,
		InsertInlinePicture,
		InsertInlineCustomObject,
		InsertFloatingObjectAnchor,
		InsertSection,
		ModifySection,
		CreateEmptyDocument,
		LoadNewDocument,
		DeleteContent,
		JoinRun,
		SplitRun,
		ToggleFieldCodes,
		ToggleFieldLocked,
		Fields,
		CommentOptionsVisibilityChanged,
		CommentPropertiesChanged,
		CommentColorPropertiesChanged
	}
	#endregion
	#region DocumentModelChangeActionsCalculator
	public static class DocumentModelChangeActionsCalculator {
		internal class DocumentModelChangeActionsTable : Dictionary<DocumentModelChangeType, DocumentModelChangeActions> {
		}
		internal static DocumentModelChangeActionsTable documentModelChangeActionsTable = CreateDocumentModelChangeActionsTable();
		internal static DocumentModelChangeActionsTable CreateDocumentModelChangeActionsTable() {
			DocumentModelChangeActionsTable table = new DocumentModelChangeActionsTable();
			table.Add(DocumentModelChangeType.None, DocumentModelChangeActions.None);
			table.Add(DocumentModelChangeType.InsertParagraph, DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetCaretInputPositionFormatting | DocumentModelChangeActions.ResetRuler);
			table.Add(DocumentModelChangeType.InsertText, DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetCaretInputPositionFormatting | DocumentModelChangeActions.ResetRuler);
			table.Add(DocumentModelChangeType.InsertInlinePicture, DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetCaretInputPositionFormatting | DocumentModelChangeActions.ResetRuler);
			table.Add(DocumentModelChangeType.InsertInlineCustomObject, DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetCaretInputPositionFormatting | DocumentModelChangeActions.ResetRuler);
			table.Add(DocumentModelChangeType.InsertFloatingObjectAnchor, DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetCaretInputPositionFormatting | DocumentModelChangeActions.ResetRuler);
			table.Add(DocumentModelChangeType.InsertSection, DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetCaretInputPositionFormatting | DocumentModelChangeActions.ResetRuler);
			table.Add(DocumentModelChangeType.ModifySection, DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.Redraw | DocumentModelChangeActions.RaiseContentChanged | DocumentModelChangeActions.ForceResetHorizontalRuler | DocumentModelChangeActions.ForceResetVerticalRuler);
			table.Add(DocumentModelChangeType.CreateEmptyDocument, DocumentModelChangeActions.ResetAllPrimaryLayout | DocumentModelChangeActions.RaiseContentChanged | DocumentModelChangeActions.RaiseSelectionChanged | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ScrollToBeginOfDocument | DocumentModelChangeActions.RaiseEmptyDocumentCreated | DocumentModelChangeActions.ResetCaretInputPositionFormatting | DocumentModelChangeActions.ForceResetHorizontalRuler | DocumentModelChangeActions.ForceResetVerticalRuler | DocumentModelChangeActions.Fields);
			table.Add(DocumentModelChangeType.LoadNewDocument, DocumentModelChangeActions.ResetAllPrimaryLayout | DocumentModelChangeActions.RaiseContentChanged | DocumentModelChangeActions.RaiseSelectionChanged | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ScrollToBeginOfDocument | DocumentModelChangeActions.RaiseDocumentLoaded | DocumentModelChangeActions.ResetCaretInputPositionFormatting | DocumentModelChangeActions.ForceResetHorizontalRuler | DocumentModelChangeActions.ForceResetVerticalRuler | DocumentModelChangeActions.Fields);
			table.Add(DocumentModelChangeType.DeleteContent, DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.RaiseContentChanged | DocumentModelChangeActions.RaiseSelectionChanged | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetCaretInputPositionFormatting | DocumentModelChangeActions.ForceResetHorizontalRuler| DocumentModelChangeActions.ForceResetVerticalRuler);
			table.Add(DocumentModelChangeType.JoinRun, DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetCaretInputPositionFormatting);
			table.Add(DocumentModelChangeType.SplitRun, DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetCaretInputPositionFormatting);
			table.Add(DocumentModelChangeType.ToggleFieldCodes, DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetCaretInputPositionFormatting | DocumentModelChangeActions.ValidateSelectionInterval);
			table.Add(DocumentModelChangeType.ToggleFieldLocked, DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetCaretInputPositionFormatting);
			table.Add(DocumentModelChangeType.Fields, DocumentModelChangeActions.Fields);
			table.Add(DocumentModelChangeType.CommentOptionsVisibilityChanged, DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.Redraw);
			table.Add(DocumentModelChangeType.CommentPropertiesChanged, DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.Redraw);
			table.Add(DocumentModelChangeType.CommentColorPropertiesChanged, DocumentModelChangeActions.Redraw);
			return table;
		}
		public static DocumentModelChangeActions CalculateChangeActions(DocumentModelChangeType change) {
			return documentModelChangeActionsTable[change];
		}
	}
	#endregion
}
