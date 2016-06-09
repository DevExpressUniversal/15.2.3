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
namespace DevExpress.XtraRichEdit.Model {
	#region IDocumentModelStructureChangedListener
	public interface IDocumentModelStructureChangedListener {
		void OnParagraphInserted(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, TableCell cell, bool isParagraphMerged, ParagraphIndex actualParagraphIndex, int historyNotificationId);
		void OnParagraphRemoved(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId);
		void OnParagraphMerged(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId);
		void OnRunInserted(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId);
		void OnRunRemoved(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int length, int historyNotificationId);
		void OnRunSplit(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int splitOffset);
		void OnRunJoined(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex joinedRunIndex, int splitOffset, int tailRunLength);
		void OnRunMerged(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength);
		void OnRunUnmerged(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength);
		void OnFieldRemoved(PieceTable pieceTable, int fieldIndex);
		void OnFieldInserted(PieceTable pieceTable, int fieldIndex);
		void OnBeginMultipleRunSplit(PieceTable pieceTable);
		void OnEndMultipleRunSplit(PieceTable pieceTable);
	}
	#endregion
	#region DocumentModelStructureChangedNotifier (helper class)
	public static class DocumentModelStructureChangedNotifier {
		public static void NotifyParagraphInserted(IDocumentModelStructureChangedListener listener, PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, TableCell cell, bool isParagraphMerged, ParagraphIndex actualParagraphIndex, int historyNotificationId) {
			listener.OnParagraphInserted(pieceTable, sectionIndex, paragraphIndex, runIndex, cell, isParagraphMerged, actualParagraphIndex, historyNotificationId);
		}
		public static void NotifyParagraphRemoved(IDocumentModelStructureChangedListener listener, PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			listener.OnParagraphRemoved(pieceTable, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
		}
		public static void NotifyParagraphMerged(IDocumentModelStructureChangedListener listener, PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			listener.OnParagraphMerged(pieceTable, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
		}
		public static void NotifyRunInserted(IDocumentModelStructureChangedListener listener, PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId) {
			listener.OnRunInserted(pieceTable, paragraphIndex, newRunIndex, length, historyNotificationId);
		}
		public static void NotifyRunRemoved(IDocumentModelStructureChangedListener listener, PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int length, int historyNotificationId) {
			listener.OnRunRemoved(pieceTable, paragraphIndex, runIndex, length, historyNotificationId);
		}
		public static void NotifyRunSplit(IDocumentModelStructureChangedListener listener, PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int splitOffset) {
			listener.OnRunSplit(pieceTable, paragraphIndex, runIndex, splitOffset);
		}
		public static void NotifyRunJoined(IDocumentModelStructureChangedListener listener, PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex joinedRunIndex, int splitOffset, int tailRunLength) {
			listener.OnRunJoined(pieceTable, paragraphIndex, joinedRunIndex, splitOffset, tailRunLength);
		}
		public static void NotifyRunMerged(IDocumentModelStructureChangedListener listener, PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			listener.OnRunMerged(pieceTable, paragraphIndex, runIndex, deltaRunLength);
		}
		public static void NotifyRunUnmerged(IDocumentModelStructureChangedListener listener, PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			listener.OnRunUnmerged(pieceTable, paragraphIndex, runIndex, deltaRunLength);
		}
		public static void NotifyFieldRemoved(IDocumentModelStructureChangedListener listener, PieceTable pieceTable, int fieldIndex) {
			listener.OnFieldRemoved(pieceTable, fieldIndex);
		}
		public static void NotifyFieldInserted(IDocumentModelStructureChangedListener listener, PieceTable pieceTable, int fieldIndex) {
			listener.OnFieldInserted(pieceTable, fieldIndex);
		}
		public static void NotifyBeginMultipleRunSplit(IDocumentModelStructureChangedListener listener, PieceTable pieceTable) {
			listener.OnBeginMultipleRunSplit(pieceTable);
		}
		public static void NotifyEndMultipleRunSplit(IDocumentModelStructureChangedListener listener, PieceTable pieceTable) {
			listener.OnEndMultipleRunSplit(pieceTable);
		}
	}
	#endregion
}
