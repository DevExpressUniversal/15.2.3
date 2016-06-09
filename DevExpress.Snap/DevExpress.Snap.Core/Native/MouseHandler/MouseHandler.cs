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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Snap.Core.Native.Services;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Mouse;
using DevExpress.XtraRichEdit;
using DevExpress.Snap.Core.Fields;
#if !SL
using PlatformIndependentMouseEventArgs = System.Windows.Forms.MouseEventArgs;
using System.Windows.Forms;
#else
using PlatformIndependentMouseEventArgs = DevExpress.Data.MouseEventArgs;
using DevExpress.Data;
#endif
namespace DevExpress.Snap.Core.Native.MouseHandler {
	public class SnapMouseHandler : RichEditMouseHandler {
		HierarchicalController<Field, PieceTable> fieldHierarchicalController;
		public SnapMouseHandler(IRichEditControl control)
			: base(control) {
			this.fieldHierarchicalController = CreateFieldHierarchicalController();
			SubscribeFieldHierarchicalControllerEvents();
		}				
		public HierarchicalController<Field, PieceTable> FieldHierarchicalController { get { return fieldHierarchicalController; } }		
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (fieldHierarchicalController != null)
					UnsubscribeFieldHierarchicalControllerEvents();
				fieldHierarchicalController = null;
			}
			base.Dispose(disposing);			
		}		
		protected virtual HierarchicalController<Field, PieceTable> CreateFieldHierarchicalController() {
			return new HierarchicalController<Field, PieceTable>();
		}
		protected virtual void SubscribeFieldHierarchicalControllerEvents() {
			FieldHierarchicalController.BeforeChangeCurrentItem += BeforeChangeCurrentItem;
			FieldHierarchicalController.AfterChangeCurrentItem += AfterChangeCurrentItem;
		}
		protected virtual void UnsubscribeFieldHierarchicalControllerEvents() {
			FieldHierarchicalController.BeforeChangeCurrentItem -= BeforeChangeCurrentItem;
			FieldHierarchicalController.AfterChangeCurrentItem -= AfterChangeCurrentItem;
		}
		protected internal override DefaultMouseHandlerState CreateDefaultState() {
			return new SnapDefaultMouseHandlerState(this);
		}
		protected internal override RichEditRectangularObjectResizeMouseHandlerState CreateRectangularObjectResizeState(RectangularObjectHotZone hotZone, RichEditHitTestResult result) {
			return new SnapRectangularObjectResizeMouseHandlerState(this, hotZone, result);
		}
		protected virtual void AfterChangeCurrentItem(object sender, EventArgs e) {
			Field newCurrentItem = FieldHierarchicalController.CurrentItem;
			if (newCurrentItem != null)
				OnCurrentFieldChanged(FieldHierarchicalController.CurrentUserData, newCurrentItem);
		}
		protected virtual void BeforeChangeCurrentItem(object sender, EventArgs e) {
			Field currentItem = FieldHierarchicalController.CurrentItem;
			if (currentItem != null)
				OnCurrentFieldChanging(FieldHierarchicalController.CurrentUserData, currentItem);
		}
		protected internal virtual void OnCurrentFieldChanged(PieceTable pieceTable, Field field) {
			((InnerSnapControl)this.Control.InnerControl).OnCurrentFieldChanged(pieceTable, field);
		}
		protected virtual void OnCurrentFieldChanging(PieceTable pieceTable, Field field) {
			((InnerSnapControl)this.Control.InnerControl).OnCurrentFieldChanging(pieceTable, field);
		}
		protected override void HandleMouseUp(PlatformIndependentMouseEventArgs e) {
			base.HandleMouseUp(e);
			RichEditHitTestResult hitTestResult = CalculateHitTest(new Point(e.X, e.Y));
			if(hitTestResult == null)
				return;
			SnapPieceTable pieceTable = (SnapPieceTable)hitTestResult.PieceTable;
			Field field = FieldsHelper.FindFieldByHitTestResult(hitTestResult);
			if(field == null)
				return;
			FieldHierarchicalController.ChangeCurrentItem(field, pieceTable);
		}
		protected internal override DragContentMouseHandlerStateCalculator CreateDragContentMouseHandlerStateCalculator() {
			return new SnapDragContentMouseHandlerStateCalculator(Control.InnerControl.Owner);
		}
		protected internal override DragFloatingObjectMouseHandlerStateCalculator CreateDragFloatingObjectMouseHandlerStateCalculator() {
			return new SnapDragFloatingObjectMouseHandlerStateCalculator(Control.InnerControl.Owner);
		}
		public override void SwitchStateCore(MouseHandlerState newState, Point mousePosition) {
			base.SwitchStateCore(newState, mousePosition);
			UpdateTableViewInfoController(newState, mousePosition);
		}
		protected virtual void UpdateTableViewInfoController(MouseHandlerState newState, Point mousePosition) {
			ITableViewInfoControllerService controllerService = Control.InnerControl.DocumentModel.GetService<ITableViewInfoControllerService>();
			RichEditView activeView = Control.InnerControl.ActiveView;
			if (controllerService == null || activeView == null)
				return;
			activeView.TableController = controllerService.Get(newState, activeView, mousePosition);
		}
	}
	public class SnapDragContentMouseHandlerStateCalculator : DragContentMouseHandlerStateCalculator {
		readonly IInnerRichEditControlOwner owner;
		bool currentBookmarkChanged;
		EnteredBookmarkInfo startDragBookmark;
		public SnapDragContentMouseHandlerStateCalculator(IInnerRichEditControlOwner owner) {
			this.owner = owner;			
		}
		public EnteredBookmarkInfo StartDragBookmark { get { return startDragBookmark; } set { startDragBookmark = value; } }
		public override bool CanDropContentTo(RichEditHitTestResult hitTestResult, PieceTable pieceTable) {
			currentBookmarkChanged = false;
			if (!base.CanDropContentTo(hitTestResult, pieceTable))
				return false;
			IDropFieldTarget target = this.owner.InnerControl.ActiveView.TableController as IDropFieldTarget;
			if (target != null && target.VisibleHotZone != null)
				return true;
			SnapPieceTable snapPieceTable = (SnapPieceTable)pieceTable;
			SnapDocumentModel documentModel = snapPieceTable.DocumentModel;
			if (StartDragBookmark != null && !IsHitTestPositionOnBookmark(hitTestResult, StartDragBookmark.Bookmark))
				return false;
			EnteredBookmarkInfo modifiedBookmark = GetModifiedBookmark(documentModel);
			DocumentLogPosition pos = pieceTable.GetRunLogPosition(hitTestResult.Box.StartPos.RunIndex);
			if (modifiedBookmark != null && !IsHitTestPositionOnBookmark(hitTestResult, modifiedBookmark.Bookmark))
				return false;
			currentBookmarkChanged = documentModel.SelectionInfo.CheckCurrentSnapBookmark(true, false, pos, pos, snapPieceTable);
			return true;
		}
		public override void OnInternalDragStart() {
			base.OnInternalDragStart();
			SnapDocumentModel documentModel = (SnapDocumentModel)owner.InnerControl.DocumentModel;
			Stack<EnteredBookmarkInfo> enteredBookmarks = documentModel.EnteredBookmarks;
			if (enteredBookmarks.Count == 0)
				StartDragBookmark = null;
			else
				StartDragBookmark = enteredBookmarks.Peek();
		}
		bool IsHitTestPositionOnBookmark(RichEditHitTestResult hitTestResult, SnapBookmark bookmark) {
			RunIndex enteredBookmarkStart = bookmark.Interval.NormalizedStart.RunIndex;
			RunIndex enteredBookmarkEnd = bookmark.Interval.NormalizedEnd.RunIndex;
			RunIndex hitTestRunIndex = hitTestResult.Box.StartPos.RunIndex;
			return hitTestRunIndex >= enteredBookmarkStart && hitTestRunIndex <= enteredBookmarkEnd;
		}
		EnteredBookmarkInfo GetModifiedBookmark(SnapDocumentModel documentModel) {
			Stack<EnteredBookmarkInfo> enteredBookmarks = documentModel.EnteredBookmarks;
			foreach (EnteredBookmarkInfo info in enteredBookmarks)
				if (info.Modified)
					return info;
			return null;
		}
		public override DocumentModelPosition UpdateDocumentModelPosition(DocumentModelPosition pos) {
			SnapPieceTable pieceTable = (SnapPieceTable)pos.PieceTable;
			Field field = pieceTable.FindNonTemplateFieldByRunIndex(pos.RunIndex);
			IDropFieldsService srv = pieceTable.DocumentModel.GetService<IDropFieldsService>();
			if (srv != null && srv.CanDropContentOnField(pieceTable, field))
				return pos;
			DocumentLogPosition fieldStart = field.IsCodeView ? pieceTable.GetRunLogPosition(field.FirstRunIndex) 
				: pieceTable.GetRunLogPosition(field.Result.Start);
			DocumentLogPosition fieldEnd = field.IsCodeView ? pieceTable.GetRunLogPosition(field.Result.Start) 
				: pieceTable.GetRunLogPosition(field.LastRunIndex);
			RunIndex runIndex = (fieldEnd - pos.LogPosition > pos.LogPosition - fieldStart) ? field.FirstRunIndex : field.LastRunIndex + 1;
			return DocumentModelPosition.FromRunStart(pieceTable, runIndex);
		}
		public override void UpdateVisualState() {
			if (currentBookmarkChanged && owner != null)
				owner.Redraw();
		}
	}
	public class SnapDragFloatingObjectMouseHandlerStateCalculator : DragFloatingObjectMouseHandlerStateCalculator {
		readonly IInnerRichEditControlOwner owner;
		readonly EnteredBookmarkInfo startDragBookmark;
		public SnapDragFloatingObjectMouseHandlerStateCalculator(IInnerRichEditControlOwner owner) {
			this.owner = owner;
			Stack<EnteredBookmarkInfo> enteredBookmarks = DocumentModel.EnteredBookmarks;
			if(enteredBookmarks.Count == 0)
				this.startDragBookmark = null;
			else
				this.startDragBookmark = enteredBookmarks.Peek();
		}
		SnapDocumentModel DocumentModel { get { return (SnapDocumentModel)owner.InnerControl.DocumentModel; } }
		EnteredBookmarkInfo StartDragBookmark { get { return startDragBookmark; } }
		public override bool CanDropTo(RichEditHitTestResult point) {
			if(!base.CanDropTo(point))
				return false;
			if(StartDragBookmark == null)
				return true;
			return IsHitTestPositionOnBookmark(point, StartDragBookmark.Bookmark);
		}
		bool IsHitTestPositionOnBookmark(RichEditHitTestResult hitTestResult, SnapBookmark bookmark) {
			DocumentLayout documentLayout = hitTestResult.DocumentLayout;
			int pageIndex = hitTestResult.Page.PageIndex;
			DocumentModelPosition bmStart = bookmark.Interval.NormalizedStart;
			DocumentModelPosition bmEnd = bookmark.Interval.NormalizedEnd;
			DocumentLayoutPosition bmStartLayoutPosition = documentLayout.CreateLayoutPosition(hitTestResult.PieceTable, bmStart.LogPosition, pageIndex);
			DocumentLayoutPosition bmEndLayoutPosition = documentLayout.CreateLayoutPosition(hitTestResult.PieceTable, bmEnd.LogPosition-1, pageIndex);
			if(!bmStartLayoutPosition.Update(documentLayout.Pages, DocumentLayoutDetailsLevel.Box) || !bmEndLayoutPosition.Update(documentLayout.Pages, DocumentLayoutDetailsLevel.Box))
				return false;
			if(pageIndex < bmStartLayoutPosition.Page.PageIndex || pageIndex > bmEndLayoutPosition.Page.PageIndex)
				return false;
			if(hitTestResult.Column.StartPos.RunIndex > bmStart.RunIndex && hitTestResult.Column.EndPos.RunIndex < bmEnd.RunIndex)
				return true;
			int y = hitTestResult.LogicalPoint.Y + LogicalPointOffset.Y;
			if(Object.ReferenceEquals(hitTestResult.Column, bmStartLayoutPosition.Column)) {
				if(Object.ReferenceEquals(hitTestResult.Column, bmEndLayoutPosition.Column))
					return y > bmStartLayoutPosition.Box.Bounds.Top && y < bmEndLayoutPosition.Box.Bounds.Bottom;
				return y > bmStartLayoutPosition.Box.Bounds.Top;
			}
			return y < bmEndLayoutPosition.Box.Bounds.Bottom;
		}
	}
}
