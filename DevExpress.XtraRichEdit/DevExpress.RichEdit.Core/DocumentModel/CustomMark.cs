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
	public class CustomMark : DocumentModelPositionAnchor {
		readonly object userData;
		public CustomMark(DocumentModelPosition pos, object userData) : base(pos) {
			this.userData = userData;
		}
		public object UserData { get { return userData; } }
	}
	public class CustomMarkCollection : List<CustomMark>, IDocumentModelStructureChangedListener {		 
		public CustomMarkCollection() {			
		}
		#region IDocumentModelStructureChangedListener Members
		void IDocumentModelStructureChangedListener.OnParagraphInserted(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, TableCell cell, bool isParagraphMerged, ParagraphIndex actualParagraphIndex, int historyNotificationId) {
			OnParagraphInserted(pieceTable, sectionIndex, paragraphIndex, runIndex, cell, isParagraphMerged, actualParagraphIndex, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnParagraphRemoved(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			OnParagraphRemoved(pieceTable, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnParagraphMerged(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			OnParagraphMerged(pieceTable, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnRunInserted(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId) {
			OnRunInserted(pieceTable, paragraphIndex, newRunIndex, length, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnRunRemoved(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int length, int historyNotificationId) {
			OnRunRemoved(pieceTable, paragraphIndex, runIndex, length, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnBeginMultipleRunSplit(PieceTable pieceTable) {
		}
		void IDocumentModelStructureChangedListener.OnEndMultipleRunSplit(PieceTable pieceTable) {
		}
		void IDocumentModelStructureChangedListener.OnRunSplit(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int splitOffset) {
			OnRunSplit(pieceTable, paragraphIndex, runIndex, splitOffset);
		}
		void IDocumentModelStructureChangedListener.OnRunJoined(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex joinedRunIndex, int splitOffset, int tailRunLength) {
			OnRunJoined(pieceTable, paragraphIndex, joinedRunIndex, splitOffset, tailRunLength);
		}
		void IDocumentModelStructureChangedListener.OnRunMerged(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			OnRunMerged(pieceTable, paragraphIndex, runIndex, deltaRunLength);
		}
		void IDocumentModelStructureChangedListener.OnRunUnmerged(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			OnRunUnmerged(pieceTable, paragraphIndex, runIndex, deltaRunLength);
		}
		void IDocumentModelStructureChangedListener.OnFieldRemoved(PieceTable pieceTable, int fieldIndex) {
			OnFieldRemoved(pieceTable, fieldIndex);	
		}
		void IDocumentModelStructureChangedListener.OnFieldInserted(PieceTable pieceTable, int fieldIndex) {
			OnFieldInserted(pieceTable, fieldIndex);
		}
		#endregion
		protected virtual void OnParagraphInserted(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, TableCell cell, bool isParagraphMerged, ParagraphIndex actualParagraphIndex, int historyNotificationId) {
			Action<DocumentModelPositionAnchor> anchorAction = delegate(DocumentModelPositionAnchor anchor) {
				DocumentModelStructureChangedNotifier.NotifyParagraphInserted(anchor, pieceTable, sectionIndex, paragraphIndex, runIndex, cell, isParagraphMerged, actualParagraphIndex, historyNotificationId);
			};
			UpdateAnchors(anchorAction);
		}
		protected virtual void OnParagraphRemoved(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			Action<DocumentModelPositionAnchor> anchorAction = delegate(DocumentModelPositionAnchor anchor) {
				DocumentModelStructureChangedNotifier.NotifyParagraphRemoved(anchor, pieceTable, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
			};
			UpdateAnchors(anchorAction);
		}
		protected virtual void OnParagraphMerged(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			Action<DocumentModelPositionAnchor> anchorAction = delegate(DocumentModelPositionAnchor anchor) {
				DocumentModelStructureChangedNotifier.NotifyParagraphMerged(anchor, pieceTable, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
			};
			UpdateAnchors(anchorAction);
		}
		protected virtual void OnRunInserted(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId) {
			Action<DocumentModelPositionAnchor> anchorAction = delegate(DocumentModelPositionAnchor anchor) {
				DocumentModelStructureChangedNotifier.NotifyRunInserted(anchor, pieceTable, paragraphIndex, newRunIndex, length, historyNotificationId);
			};
			UpdateAnchors(anchorAction);
		}
		protected virtual void OnRunRemoved(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int length, int historyNotificationId) {
			Action<DocumentModelPositionAnchor> anchorAction = delegate(DocumentModelPositionAnchor anchor) {
				DocumentModelStructureChangedNotifier.NotifyRunRemoved(anchor, pieceTable, paragraphIndex, runIndex, length, historyNotificationId);
			};
			UpdateAnchors(anchorAction);
		}
		protected virtual void OnRunSplit(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int splitOffset) {
			Action<DocumentModelPositionAnchor> anchorAction = delegate(DocumentModelPositionAnchor anchor) {
				DocumentModelStructureChangedNotifier.NotifyRunSplit(anchor, pieceTable, paragraphIndex, runIndex, splitOffset);
			};
			UpdateAnchors(anchorAction);
		}
		protected virtual void OnRunJoined(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex joinedRunIndex, int splitOffset, int tailRunLength) {
			Action<DocumentModelPositionAnchor> anchorAction = delegate(DocumentModelPositionAnchor anchor) {
				DocumentModelStructureChangedNotifier.NotifyRunJoined(anchor, pieceTable, paragraphIndex, joinedRunIndex, splitOffset, tailRunLength);
			};
			UpdateAnchors(anchorAction);
		}
		protected virtual void OnRunMerged(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			Action<DocumentModelPositionAnchor> anchorAction = delegate(DocumentModelPositionAnchor anchor) {
				DocumentModelStructureChangedNotifier.NotifyRunMerged(anchor, pieceTable, paragraphIndex, runIndex, deltaRunLength);
			};
			UpdateAnchors(anchorAction);
		}
		protected virtual void OnRunUnmerged(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			Action<DocumentModelPositionAnchor> anchorAction = delegate(DocumentModelPositionAnchor anchor) {
				DocumentModelStructureChangedNotifier.NotifyRunUnmerged(anchor, pieceTable, paragraphIndex, runIndex, deltaRunLength);
			};
			UpdateAnchors(anchorAction);
		}
		protected virtual void OnFieldRemoved(PieceTable pieceTable, int fieldIndex) {
			Action<DocumentModelPositionAnchor> anchorAction = delegate(DocumentModelPositionAnchor anchor) {
				DocumentModelStructureChangedNotifier.NotifyFieldRemoved(anchor, pieceTable, fieldIndex);
			};
			UpdateAnchors(anchorAction);
		}
		protected virtual void OnFieldInserted(PieceTable pieceTable, int fieldIndex) {
			Action<DocumentModelPositionAnchor> anchorAction = delegate(DocumentModelPositionAnchor anchor) {
				DocumentModelStructureChangedNotifier.NotifyFieldInserted(anchor, pieceTable, fieldIndex);
			};
			UpdateAnchors(anchorAction);
		}
		protected internal virtual void UpdateAnchors(Action<DocumentModelPositionAnchor> anchorAction) {			
			for (int i = Count - 1; i >= 0; i--) {
				CustomMark customMark = this[i];
				anchorAction(customMark);
			}
		}
	}
}
