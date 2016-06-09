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
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Native;
using DevExpress.XtraRichEdit.Commands;
using System.Collections.Generic;
namespace DevExpress.XtraRichEdit.Forms {
	public delegate bool Predicate();
	#region BookmarkFormControllerParameters
	public class BookmarkFormControllerParameters : FormControllerParameters {
		internal BookmarkFormControllerParameters(IRichEditControl control)
			: base(control) {
		}
	}
	#endregion
	#region BookmarkFormController
	public class BookmarkFormController : FormController {
		readonly IRichEditControl control;
		public BookmarkFormController(BookmarkFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.control = controllerParameters.Control;
		}
		#region Properties
		public IRichEditControl Control { get { return control; } }
		protected PieceTable PieceTable { get { return Control.InnerControl.DocumentModel.Selection.PieceTable; } }
		#endregion
		protected internal virtual List<Bookmark> GetBookmarks(bool includeHiddenBookmarks) {
			return Control.InnerControl.DocumentModel.GetBookmarks(includeHiddenBookmarks);
		}
		public override void ApplyChanges() {
		}
		public IList<Bookmark> GetBookmarksSortedByName() {
			return GetBookmarksSortedByName(true);
		}
		public IList<Bookmark> GetBookmarksSortedByName(bool includeHiddenBookmarks) {
			List<Bookmark> result = GetBookmarks(includeHiddenBookmarks);
			result.Sort(new BookmarkNameComparer());
			return result;
		}
		public IList<Bookmark> GetBookmarksSortedByLocation() {
			return GetBookmarksSortedByLocation(true);
		}
		public IList<Bookmark> GetBookmarksSortedByLocation(bool includeHiddenBookmarks) {
			return GetBookmarks(includeHiddenBookmarks);
		}
		public virtual void CreateBookmark(string name, Predicate recreate) {
			List<Bookmark> bookmarks = GetBookmarks(true);
			Bookmark bookmark = FindBookmarkByName(bookmarks, name);
			if (bookmark != null) {
				if (!recreate())
					return;
				DeleteBookmark(bookmark);
			}
			CreateBookmarkCore(name);
		}
		protected internal virtual Bookmark FindBookmarkByName(List<Bookmark> bookmarks, string name) {
			int count = bookmarks.Count;
			for (int i = 0; i < count; i++) {
				if (bookmarks[i].Name == name)
					return bookmarks[i];
			}
			return null;
		}
		protected internal virtual bool ValidateName(string name) {
			return Bookmark.IsNameValid(name);
		}
		protected internal virtual void CreateBookmarkCore(string name) {
			CreateBookmarkCommand command = new CreateBookmarkCommand(Control, name.Trim());
			command.Execute();
		}
		public virtual void DeleteBookmark(Bookmark bookmark) {
			DeleteBookmarkCommand command = new DeleteBookmarkCommand(Control, bookmark);
			command.Execute();
		}
		public virtual void SelectBookmark(Bookmark bookmark) {
			if (!CanSelectBoomark(bookmark))
				return;
			SelectBookmarkCommand command = new SelectBookmarkCommand(Control, bookmark);
			command.Execute();
		}
		public virtual bool CanSelectBoomark(Bookmark bookmark) {
			return Object.ReferenceEquals(PieceTable, bookmark.PieceTable);
		}
		public virtual Bookmark GetCurrentBookmark() {
			Bookmark result = null;
			DocumentLogPosition logPosition = Control.InnerControl.ActiveView.CaretPosition.LogPosition;
			VisitableDocumentIntervalBoundaryIterator iterator = new VisitableDocumentIntervalBoundaryIterator(PieceTable);
			VisitableDocumentIntervalBoundaryCollection boundaries = iterator.Boundaries;
			int count = boundaries.Count;
			for (int i = 0; i < count; i++) {
				VisitableDocumentIntervalBoundary boundary = boundaries[i];
				if (logPosition > boundary.Position.LogPosition) {
					if (boundary.Order == BookmarkBoundaryOrder.Start && logPosition <= boundary.VisitableInterval.End)
						result = (Bookmark)boundary.VisitableInterval;
					continue;
				}
				if (boundary.Order == BookmarkBoundaryOrder.Start) {
					if (result != null)
						return result;
					return (Bookmark)(i > 0 ? boundaries[i - 1].VisitableInterval : boundaries[count - 1].VisitableInterval);
				}
			}
			return (Bookmark)((result != null || count == 0) ? result : boundaries[count - 1].VisitableInterval); 
		}
	}
	#endregion
}
