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
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.Snap.Core.Export;
using DevExpress.Office.Utils;
namespace DevExpress.Snap.Core.Native {
	public class SnapBookmark : BookmarkBase, IFieldContextOwner {
		SnapBookmark parent;
		public SnapBookmark(SnapPieceTable pieceTable, IFieldContext dataContext, DocumentLogPosition start, DocumentLogPosition end, SnapTemplateInterval templateInterval)
			: base(pieceTable, start, end) {
			Guard.ArgumentNotNull(templateInterval, "templateInterval");
			CanExpand = true;
			FieldContext = dataContext;
			TemplateInterval = templateInterval;
		}
		public new SnapPieceTable PieceTable { get { return (SnapPieceTable)base.PieceTable; } }
		public SnapTemplateInterval TemplateInterval { get; private set; }
		public IFieldContext FieldContext { get; private set; }
		public SnapBookmark Parent {
			get { return parent; }
			set {
				if (Object.ReferenceEquals(this, value))
					Exceptions.ThrowArgumentException("Parent", value);
				this.parent = value;
			}
		}
		IFieldContextOwner IFieldContextOwner.Parent {
			get { return Parent; }
		}
		public SnapBookmark HeaderBookmark { get; set; }
		public SnapBookmark FooterBookmark { get; set; }
		public bool Deleted { get; set; }
		public override void Visit(IDocumentIntervalVisitor visitor) {
			((ISnapBookmarkVisitor)visitor).Visit(this);
		}
		protected internal override void Delete(int index) {
			PieceTable.DeleteSnapBookmarkCore(index);
		}
		#region IDocumentModelStructureChangedListener overrides
		protected internal override void OnFieldInserted(int fieldIndex) {
		}
		protected internal override void OnFieldRemoved(int fieldIndex) {
		}
		protected internal override void OnParagraphInserted(SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, TableCell cell, bool isParagraphMerged, ParagraphIndex actualParagraphIndex, int historyNotificationId) {
		}
		protected internal override void OnParagraphMerged(SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
		}
		protected internal override void OnParagraphRemoved(SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
		}
		protected internal override void OnRunInserted(ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId) {
		}
		protected internal override void OnRunJoined(ParagraphIndex paragraphIndex, RunIndex joinedRunIndex, int splitOffset, int tailRunLength) {
		}
		protected internal override void OnRunMerged(ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
		}
		protected internal override void OnRunRemoved(ParagraphIndex paragraphIndex, RunIndex runIndex, int length, int historyNotificationId) {
		}
		protected internal override void OnRunSplit(ParagraphIndex paragraphIndex, RunIndex runIndex, int splitOffset) {
		}
		protected internal override void OnRunUnmerged(ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
		}
		#endregion
		public virtual void RegisterInterval(DocumentModelPositionManager documentModelPositionManager) {
			((ReferencedBookmarkRunInfo)Interval).RegisterInterval(documentModelPositionManager);
			TemplateInterval.RegisterInterval(documentModelPositionManager);
		}
		public virtual void AttachInterval(DocumentModelPositionManager documentModelPositionManager) {
			((ReferencedBookmarkRunInfo)Interval).AttachInterval(documentModelPositionManager);
			TemplateInterval.AttachInterval(documentModelPositionManager);
		}
		public virtual void DetachInterval(DocumentModelPositionManager documentModelPositionManager) {
			((ReferencedBookmarkRunInfo)Interval).DetachInterval(documentModelPositionManager);
			TemplateInterval.DetachInterval(documentModelPositionManager);
		}
		protected override RunInfo CreateRunInfo(PieceTable pieceTable) {
			return new ReferencedBookmarkRunInfo(pieceTable);
		}
	}
	#region FieldRunIndexComparable
	public class SnapBookmarkAndLogPositionComparable : IComparable<SnapBookmark> {
		readonly DocumentLogPosition position;
		public SnapBookmarkAndLogPositionComparable(DocumentLogPosition position) {
			this.position = position;
		}
		#region IComparable<SnapBookmark> Members
		public int CompareTo(SnapBookmark bookmark) {
			if (bookmark.NormalizedEnd < position)
				return -1;
			if (bookmark.NormalizedEnd > position)
				return 1;
			return 0;
		}
		#endregion
	}
	#endregion
}
