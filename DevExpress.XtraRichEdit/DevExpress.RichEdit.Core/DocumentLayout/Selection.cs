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
using DevExpress.Office.Layout;
using DevExpress.Office.Utils;
using DevExpress.Office.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Layout {
	#region Layouts
	public class SelectionLayout {
		readonly RichEditView view;
		int preferredPageIndex;
		List<DocumentSelectionLayout> documentSelectionLayouts;
		public SelectionLayout(RichEditView view, int preferredPageIndex) {
			Guard.ArgumentNotNull(view, "view");
			Guard.ArgumentNonNegative(preferredPageIndex, "preferredPageIndex");
			this.view = view;
			this.preferredPageIndex = preferredPageIndex;
			this.documentSelectionLayouts = null;
		}
		public int PreferredPageIndex {
			get { return preferredPageIndex; }
			set {
				Guard.ArgumentNonNegative(value, "preferredPageIndex");
				preferredPageIndex = value;
			}
		}
		public DocumentLayoutPosition StartLayoutPosition {
			get {
				DocumentLayoutPosition pos = FirstDocumentSelectionLayout.Start;
				pos.Update(DocumentLayout.Pages, DocumentLayoutDetailsLevel.Character);
				return pos;
			}
		}
		public DocumentLayoutPosition EndLayoutPosition {
			get {
				DocumentLayoutPosition pos = LastDocumentSelectionLayout.End;
				pos.Update(DocumentLayout.Pages, DocumentLayoutDetailsLevel.Character);
				return pos;
			}
		}
		protected DocumentSelectionLayout FirstDocumentSelectionLayout {
			get {
				if (documentSelectionLayouts == null)
					Update();
				return documentSelectionLayouts[0];
			}
		}
		public DocumentSelectionLayout LastDocumentSelectionLayout {
			get {
				if ((documentSelectionLayouts == null) || (documentSelectionLayouts.Count == 0))
					Update();
				return documentSelectionLayouts[documentSelectionLayouts.Count - 1];
			}
		}
		public RichEditView View { get { return view; } }
		protected DocumentLayout DocumentLayout { get { return View.DocumentLayout; } }
		public virtual HotZone CalculateHotZone(RichEditHitTestResult result, RichEditView view) {
			Guard.ArgumentNotNull(result, "result");
			Guard.ArgumentNotNull(view, "view");
			Update();
			HotZoneCollection hotZones = LastDocumentSelectionLayout.HotZones;
			int count = hotZones.Count;
			for (int i = 0; i < count; i++) {
				HotZone hotZone = hotZones[i];
				if (hotZone.HitTest(result.LogicalPoint, view.DocumentModel.UnitConverter.ScreenDpi, view.ZoomFactor))
					return hotZone;
			}
			return null;
		}
		protected internal virtual bool IsSelectionStartFromBeginRow() {
			if (StartLayoutPosition.Row.Boxes[0].StartPos.AreEqual(StartLayoutPosition.Character.StartPos))
				return true;
			return false;
		}
		public virtual void Invalidate() {
			if (this.documentSelectionLayouts == null)
				return;
			this.documentSelectionLayouts.Clear();
			this.documentSelectionLayouts = null;
		}
		public virtual void Update() {
			if (documentSelectionLayouts != null && documentSelectionLayouts.Count > 0 && LastDocumentSelectionLayout.UpdatedSuccessfully)
				return;
			Selection selection = DocumentLayout.DocumentModel.Selection;
			this.documentSelectionLayouts = new List<DocumentSelectionLayout>();
			for (int i = 0; i < selection.Items.Count; i++) {
				SelectionItem selectionItem = selection.Items[i];
				if (selectionItem.IsCovered) {
					Debug.Assert(selection.Items.Count > 1);
					continue;
				}
				if (!SelectionItemIsVisible(selectionItem))
					return;
				DocumentLayoutPosition start = CreateDocumentLayoutPosition(GetNormalisedStart(selectionItem));
				DocumentLayoutPosition end = CreateDocumentLayoutPosition(GetNormalisedEnd(selectionItem));
				start.LeftOffset = selectionItem.LeftOffset;
				end.RightOffset = selectionItem.RightOffset;
				DocumentSelectionLayout documentSelectionLayout = CreateDocumentSelectionLayout(start, end);
				this.documentSelectionLayouts.Add(documentSelectionLayout);
				if (selection.Length <= 0 && selection.Items.Count == 1 && selectionItem.LeftOffset == 0 && selectionItem.RightOffset == 0)
					documentSelectionLayout.ForceUpdateForEmptySelection();
				else
					documentSelectionLayout.Update();
			}
		}
		public virtual DocumentLogPosition GetNormalisedStart (SelectionItem selectionItem) {
			return selectionItem.NormalizedStart;
		}
		public virtual DocumentLogPosition GetNormalisedEnd(SelectionItem selectionItem) {
			return selectionItem.NormalizedEnd;
		}
		public virtual bool SelectionItemIsVisible(SelectionItem selectionItem) {
			return true;
		}
		public virtual DocumentLayoutPosition CreateDocumentLayoutPosition(DocumentLogPosition logPosition) {
			return new DocumentLayoutPosition(DocumentLayout, DocumentLayout.DocumentModel.Selection.PieceTable, logPosition);
		}
		protected virtual DocumentSelectionLayout CreateDocumentSelectionLayout(DocumentLayoutPosition start, DocumentLayoutPosition end) {
			return new DocumentSelectionLayout(this, start, end, View.DocumentModel.ActivePieceTable);
		}
		public bool HitTest(DocumentLogPosition logPosition, Point logicalPoint) {
			Update();
			foreach (DocumentSelectionLayout piece in this.documentSelectionLayouts)
				if (piece.HitTest(logPosition, logicalPoint))
					return true;
			return false;
		}
		public virtual PageSelectionLayoutsCollection GetPageSelection(Page page) {
			if (documentSelectionLayouts == null)
				Exceptions.ThrowInternalException();
			PageSelectionLayoutsCollection result = new PageSelectionLayoutsCollection();
			foreach (DocumentSelectionLayout layout in documentSelectionLayouts)
				layout.AddToPageSelection(page, result);
			return (result.Count == 0) ? null : result;
		}
	}
	public class HeaderFooterSelectionLayout : SelectionLayout {
		public HeaderFooterSelectionLayout(RichEditView view, int preferredPageIndex)
			: base(view, preferredPageIndex) { }
		public override DocumentLayoutPosition CreateDocumentLayoutPosition(DocumentLogPosition logPosition) {
			return new HeaderFooterDocumentLayoutPosition(DocumentLayout, DocumentLayout.DocumentModel.Selection.PieceTable, logPosition, PreferredPageIndex);
		}
	}
	public class TextBoxSelectionLayout : SelectionLayout {
		public TextBoxSelectionLayout(RichEditView view, int preferredPageIndex)
			: base(view, preferredPageIndex) { }
		public override DocumentLayoutPosition CreateDocumentLayoutPosition(DocumentLogPosition logPosition) {
			return new TextBoxDocumentLayoutPosition(DocumentLayout, (TextBoxContentType)DocumentLayout.DocumentModel.Selection.PieceTable.ContentType, logPosition, PreferredPageIndex);
		}
		protected override DocumentSelectionLayout CreateDocumentSelectionLayout(DocumentLayoutPosition start, DocumentLayoutPosition end) {
			return new TextBoxDocumentSelectionLayout(this, start, end, View.DocumentModel.ActivePieceTable);
		}
	}
	public class CommentSelectionLayout : SelectionLayout {
		readonly PieceTable pieceTable;
		readonly CommentViewInfo commentViewInfo;
		public CommentSelectionLayout(RichEditView view, int preferredPageIndex, PieceTable pieceTable)
			: base(view, preferredPageIndex) {
			this.pieceTable = pieceTable;
			this.commentViewInfo = FindCommentViewInfo();
		}
		protected internal PieceTable PieceTable { get { return pieceTable; } }
		protected internal CommentViewInfo CommentViewInfo { get { return commentViewInfo; } }
		public override DocumentLayoutPosition CreateDocumentLayoutPosition(DocumentLogPosition logPosition) {
			return new CommentDocumentLayoutPosition(DocumentLayout, (CommentContentType)DocumentLayout.DocumentModel.Selection.PieceTable.ContentType, logPosition, PreferredPageIndex);
		}
		public override DocumentLogPosition GetNormalisedStart(SelectionItem selectionItem) {
			return selectionItem.NormalizedStart;
		}
		public override DocumentLogPosition GetNormalisedEnd(SelectionItem selectionItem) {
			if ((CommentViewInfo != null) && (CommentViewInfo.LastVisiblePosition > DocumentLogPosition.Zero) && (CommentViewInfo.LastVisiblePosition < selectionItem.NormalizedEnd))
				return CommentViewInfo.LastVisiblePosition;
			return selectionItem.NormalizedEnd;
		}
		public override bool SelectionItemIsVisible(SelectionItem selectionItem) {
			return true;
		}
		CommentViewInfo FindCommentViewInfo() {
			if (View.PageViewInfos != null) {
				int count = View.PageViewInfos.Count;
				for (int i = 0; i < count; i++) {
					PageViewInfo pageViewInfo = View.PageViewInfos[i];
					CommentViewInfoHelper helper = new CommentViewInfoHelper();
					CommentViewInfo commentViewInfo = helper.FindCommentViewInfo(pageViewInfo.Page, this.PieceTable);
					if (commentViewInfo != null)
						return commentViewInfo;
				}
			}
			return null;
		}
	}
	#region TextBoxDocumentSelectionLayout
	public class TextBoxDocumentSelectionLayout : DocumentSelectionLayout {
		RectangularObjectSelectionLayout textBoxFrameSelection;
		public TextBoxDocumentSelectionLayout(SelectionLayout selectionLayout, DocumentLayoutPosition start, DocumentLayoutPosition end, PieceTable pieceTable)
			: base(selectionLayout, start, end, pieceTable) {
		}
		public override void AddToPageSelection(Page page, PageSelectionLayoutsCollection where) {
			base.AddToPageSelection(page, where);
			if (textBoxFrameSelection != null) {
				FloatingObjectBox box = (FloatingObjectBox)textBoxFrameSelection.Box;
				DocumentLayoutPosition pos = DocumentLayout.CreateLayoutPosition(box.PieceTable, textBoxFrameSelection.LogStart, 0);
				if (pos.Update(DocumentLayout.Pages, DocumentLayoutDetailsLevel.Page) && page == pos.Page)
					where.Add(textBoxFrameSelection);
			}
		}
		public override void ForceUpdateForEmptySelection() {
			this.textBoxFrameSelection = null;
			base.ForceUpdateForEmptySelection();
			TryToAddTextBoxFrameSelectionLayout(Start, End);
			UpdateNestedItems();
		}
		protected override bool UpdateCore() {
			this.textBoxFrameSelection = null;
			TryToAddTextBoxFrameSelectionLayout(Start, End);
			return base.UpdateCore();
		}
		protected virtual void TryToAddTextBoxFrameSelectionLayout(DocumentLayoutPosition start, DocumentLayoutPosition end) {
			TextBoxContentType textBoxContentType = DocumentModel.ActivePieceTable.ContentType as TextBoxContentType;
			if (textBoxContentType == null)
				return;
			FloatingObjectAnchorRun anchorRun = textBoxContentType.AnchorRun;
			DocumentLogPosition logPosition = anchorRun.PieceTable.GetRunLogPosition(anchorRun);
			PageCollection pages = DocumentLayout.Pages;
			int startBoxIndex = pages.IndexOf(GetBox(start));
			int endBoxIndex = pages.IndexOf(GetBox(end));
			for (int i = startBoxIndex; i <= endBoxIndex; i++) {
				FloatingObjectBox box = pages[i].FindFloatingObject(anchorRun);
				if (box == null)
					continue;
				ResizeableRectangularObjectSelectionLayout result = new ResizeableRectangularObjectSelectionLayout(View, box, logPosition, textBoxContentType.PieceTable);
				result.HitTestTransform = box.CreateBackwardTransformUnsafe();
				AddNestedItem(i, result);
				break;
			}
		}
	}
	#endregion
	#endregion
	public interface ISelectionPainter {
		void Draw(RectangularObjectSelectionLayout viewInfo);
		void Draw(RowSelectionLayoutBase viewInfo);
		void Draw(FloatingObjectAnchorSelectionLayout viewInfo);
	}
	#region LayoutItems
	public interface ISelectionLayoutItem {
		void Draw(ISelectionPainter selectionPainter);
		bool Update();
		bool HitTest(DocumentLogPosition logPosition, Point logicalPoint);
	}
	public abstract class RectangularObjectSelectionLayoutBase : ISelectionLayoutItem {
		readonly RichEditView view;
		readonly Box box;
		readonly DocumentLogPosition logStart;
		readonly DocumentLogPosition logEnd;
		readonly PieceTable pieceTable;
		private DocumentLayoutUnitConverter unitConverter;
		protected RectangularObjectSelectionLayoutBase(RichEditView view, Box box, DocumentLogPosition start, PieceTable pieceTable) {
			Guard.ArgumentNotNull(view, "view");
			Guard.ArgumentNotNull(box, "box");
			this.unitConverter = view.DocumentModel.LayoutUnitConverter;
			this.view = view;
			this.box = box;
			this.logStart = start;
			this.logEnd = start + 1;
			this.pieceTable = pieceTable;
		}
		public DocumentLogPosition LogStart { get { return logStart; } }
		public DocumentLogPosition LogEnd { get { return logEnd; } }
		public Box Box { get { return box; } }
		public RichEditView View { get { return view; } }
		public PieceTable PieceTable { get { return pieceTable; } }
		protected DocumentLayoutUnitConverter UnitConverter { get { return unitConverter; } }
		#region ISelectionLayoutItem Members
		public abstract void Draw(ISelectionPainter selectionPainter);
		public abstract bool Update();
		public abstract bool HitTest(DocumentLogPosition logPosition, Point logicalPoint);
		#endregion
	}
	public class RectangularObjectSelectionLayout : RectangularObjectSelectionLayoutBase {
		Matrix hitTestTransform;
		FloatingObjectAnchorSelectionLayout anchor;
		public RectangularObjectSelectionLayout(RichEditView view, Box box, DocumentLogPosition start, PieceTable pieceTable)
			: base(view, box, start, pieceTable) {
		}
		public Matrix HitTestTransform { get { return hitTestTransform; } set { hitTestTransform = value; } }
		protected FloatingObjectAnchorSelectionLayout Anchor { get { return anchor; } set { anchor = value; } }
		public virtual bool Resizeable { get { return false; } }
		#region ISelectionLayoutItem Members
		public override void Draw(ISelectionPainter selectionPainter) {
			selectionPainter.Draw(this);
			if (this.anchor != null)
				this.anchor.Draw(selectionPainter);
		}
		public override bool Update() {
			this.anchor = null;
			return true;
		}
		public override bool HitTest(DocumentLogPosition logPosition, Point logicalPoint) {
			throw new NotImplementedException();
		}
		#endregion
	}
	public class ResizeableRectangularObjectSelectionLayout : RectangularObjectSelectionLayout {
		readonly int hotZoneSize;
		const int defaultHotZoneSizeDocuments = 24;
		const int deltaRadiusForExtendedHotZone = 75;
		public ResizeableRectangularObjectSelectionLayout(RichEditView view, Box box, DocumentLogPosition start, PieceTable pieceTable)
			: base(view, box, start, pieceTable) {
			this.hotZoneSize = AlignSizeToPixel(UnitConverter.DocumentsToLayoutUnits(defaultHotZoneSizeDocuments)); 
		}
		public override bool Resizeable { get { return true; } }
		public override bool Update() {
			if (!base.Update())
				return false;
			Rectangle pictureBounds = Box.ActualSizeBounds;
			Anchor = TryCreateAnchorSelectionItem();
			if (Anchor != null)
				Anchor.Update();
			int delta = hotZoneSize * 3 - pictureBounds.Width;
			if (delta > 0) {
				pictureBounds.X -= delta / 2;
				pictureBounds.Width += delta;
			}
			delta = hotZoneSize * 3 - pictureBounds.Height;
			if (delta > 0) {
				pictureBounds.Y -= delta / 2;
				pictureBounds.Height += delta;
			}
			int largeHotZoneSize = AlignSizeToPixel(6 * hotZoneSize / 5); 
			IGestureStateIndicator gestureIndicator = View.Control.InnerControl as IGestureStateIndicator;
			if (Box is FloatingObjectBox) {
				RectangularObjectRotationHotZone rotationHotZone = new RectangularObjectRotationHotZone(Box, PieceTable, gestureIndicator);
				rotationHotZone.LineEnd = new Point(pictureBounds.Left + pictureBounds.Width / 2, pictureBounds.Top);
				AddHotZone(rotationHotZone, RectangleFromCenter(pictureBounds.Left + pictureBounds.Width / 2, pictureBounds.Top - 2 * largeHotZoneSize, largeHotZoneSize));
			}
			AddHotZone(new RectangularObjectTopLeftHotZone(Box, PieceTable, gestureIndicator), RectangleFromCenter(pictureBounds.Left, pictureBounds.Top, largeHotZoneSize));
			AddHotZone(new RectangularObjectTopRightHotZone(Box, PieceTable, gestureIndicator), RectangleFromCenter(pictureBounds.Right - 1, pictureBounds.Top, largeHotZoneSize));
			AddHotZone(new RectangularObjectBottomLeftHotZone(Box, PieceTable, gestureIndicator), RectangleFromCenter(pictureBounds.Left, pictureBounds.Bottom - 1, largeHotZoneSize));
			AddHotZone(new RectangularObjectBottomRightHotZone(Box, PieceTable, gestureIndicator), RectangleFromCenter(pictureBounds.Right - 1, pictureBounds.Bottom - 1, largeHotZoneSize));
			AddHotZone(new RectangularObjectTopMiddleHotZone(Box, PieceTable, gestureIndicator), RectangleFromCenter(pictureBounds.Left + pictureBounds.Width / 2, pictureBounds.Top, largeHotZoneSize));
			AddHotZone(new RectangularObjectBottomMiddleHotZone(Box, PieceTable, gestureIndicator), RectangleFromCenter(pictureBounds.Left + pictureBounds.Width / 2, pictureBounds.Bottom - 1, largeHotZoneSize));
			AddHotZone(new RectangularObjectMiddleLeftHotZone(Box, PieceTable, gestureIndicator), RectangleFromCenter(pictureBounds.Left, pictureBounds.Top + pictureBounds.Height / 2, largeHotZoneSize));
			AddHotZone(new RectangularObjectMiddleRightHotZone(Box, PieceTable, gestureIndicator), RectangleFromCenter(pictureBounds.Right - 1, pictureBounds.Top + pictureBounds.Height / 2, largeHotZoneSize));
			return true;
		}
		void AddHotZone(HotZone hotZone, Rectangle bounds) {
			DocumentSelectionLayout selectionLayout = View.SelectionLayout.LastDocumentSelectionLayout;
			HotZoneCollection hotZones = selectionLayout.HotZones;
			hotZone.Bounds = bounds;
			hotZone.ExtendedBounds = CalculateExtendedBounds(bounds);
			hotZone.HitTestTransform = HitTestTransform;
			hotZone.GestureStateIndicator = View.Control.InnerControl as IGestureStateIndicator;
			hotZones.Add(hotZone);
		}
		FloatingObjectAnchorSelectionLayout TryCreateAnchorSelectionItem() {
			FloatingObjectBox floatingObjectBox = Box as FloatingObjectBox;
			if (floatingObjectBox == null)
				return null;
			PieceTable anchorPieceTable = floatingObjectBox.PieceTable;
			DocumentLayoutPosition layoutPosition = View.DocumentLayout.CreateLayoutPosition(anchorPieceTable, LogStart, 0);
			if (!layoutPosition.Update(View.DocumentLayout.Pages, DocumentLayoutDetailsLevel.Row))
				return null;
			Row row = layoutPosition.Row;
			if (row == null)
				return null;
			FloatingObjectAnchorSelectionLayout result = new FloatingObjectAnchorSelectionLayout(View, Box, LogStart, anchorPieceTable);
			Rectangle bounds;
			if (row.Boxes.Count <= 0)
				bounds = row.Bounds;
			else
				bounds = row.Boxes.First.Bounds;
			bounds.Width = 30;
			bounds.Height = 30;
			result.AnchorBounds = bounds;
			return result;
		}
		Rectangle CalculateExtendedBounds(Rectangle r) {
			return new Rectangle(r.X - deltaRadiusForExtendedHotZone, r.Y - deltaRadiusForExtendedHotZone, r.Width + 2 * deltaRadiusForExtendedHotZone, r.Height + 2 * deltaRadiusForExtendedHotZone);
		}
		Rectangle RectangleFromCenter(int x, int y, int largeHotZoneSize) {
			int halfLargeHotZoneSize = largeHotZoneSize / 2;
			return new Rectangle(x - halfLargeHotZoneSize, y - halfLargeHotZoneSize, largeHotZoneSize, largeHotZoneSize);
		}
		int AlignSizeToPixel(int layoutSize) {
			int result = UnitConverter.LayoutUnitsToPixels(layoutSize);
			if ((result % 2) == 0)
				result++;
			return UnitConverter.PixelsToLayoutUnits(result);
		}
	}
	public class DocumentSelectionLayout : ISelectionLayoutItem {
		bool updatedSuccessfully;
		readonly HotZoneCollection hotZones;
		readonly Dictionary<int, List<ISelectionLayoutItem>> nestedItems;
		readonly SelectionLayout selectionLayout;
		readonly PieceTable pieceTable;
		readonly DocumentLayoutPosition start;
		readonly DocumentLayoutPosition end;
		readonly DocumentLayout documentLayout;
		public DocumentSelectionLayout(SelectionLayout selectionLayout, DocumentLayoutPosition start, DocumentLayoutPosition end, PieceTable pieceTable) {
			this.updatedSuccessfully = false;
			this.hotZones = new HotZoneCollection();
			this.nestedItems = new Dictionary<int, List<ISelectionLayoutItem>>();
			this.documentLayout = start.DocumentLayout;
			this.selectionLayout = selectionLayout;
			this.pieceTable = pieceTable;
			this.start = start;
			this.end = end;
		}
		public HotZoneCollection HotZones { get { return hotZones; } }
		public bool UpdatedSuccessfully { get { return updatedSuccessfully; } }
		protected Dictionary<int, List<ISelectionLayoutItem>> NestedItems { get { return nestedItems; } }
		SelectionLayout SelectionLayout { get { return selectionLayout; } }
		protected PieceTable PieceTable { get { return pieceTable; } }
		public DocumentLayoutPosition Start { get { return start; } }
		public DocumentLayoutPosition End { get { return end; } }
		protected DocumentLayout DocumentLayout { get { return documentLayout; } }
		protected RichEditView View { get { return selectionLayout.View; } }
		protected DocumentModel DocumentModel { get { return pieceTable.DocumentModel; } }
		public virtual void ForceUpdateForEmptySelection() {
			NestedItems.Clear();
			this.updatedSuccessfully = true;
		}
		public virtual void AddToPageSelection(Page page, PageSelectionLayoutsCollection target) {
			if (page == null)
				return;
			List<ISelectionLayoutItem> set;
			if (!NestedItems.TryGetValue(page.PageIndex, out set))
				return;
			target.AddRange(set);
		}
		Page FindPage(DocumentLogPosition logPosition) {
			DocumentLayoutPosition layoutPosition = new DocumentLayoutPosition(DocumentLayout, PieceTable, logPosition);
			layoutPosition.Update(DocumentLayout.Pages, DocumentLayoutDetailsLevel.Page);
			return layoutPosition.Page;
		}
		int FindPageIndex(DocumentLogPosition logPosition) {
			Page page = FindPage(logPosition);
			if (page == null)
				return -1;
			return page.PageIndex;
		}
		protected Page GetBox(DocumentLayoutPosition pos) {
			pos.Update(DocumentLayout.Pages, DocumentLayoutDetailsLevel.Page);
			return pos.Page;
		}
		protected virtual DocumentLayoutPosition CreateDocumentLayoutPosition(DocumentLogPosition logPosition) {
			return SelectionLayout.CreateDocumentLayoutPosition(logPosition);
		}
		protected void AddNestedItem(int pageIndex, ISelectionLayoutItem item) {
			if (!nestedItems.ContainsKey(pageIndex))
				nestedItems[pageIndex] = new List<ISelectionLayoutItem>();
			nestedItems[pageIndex].Add(item);
		}
		TableCellViewInfo FindCellViewInfo(TableCell modelCell, Column column) {
			if (column == null || modelCell == null)
				return null;
			foreach (TableViewInfo table in column.Tables) {
				if (table.Table == modelCell.Table) {
					foreach (var cell in table.Cells) {
						if (cell.Cell == modelCell) {
							return cell;
						}
					}
				}
			}
			return null;
		}
		#region ISelectionLayoutItem Members
		public void Draw(ISelectionPainter selectionPainter) {
			Update();
			foreach (List<ISelectionLayoutItem> pageLayout in this.nestedItems.Values)
				foreach (ISelectionLayoutItem item in pageLayout)
					item.Draw(selectionPainter);
		}
		public virtual bool Update() {
			HotZones.Clear();
			NestedItems.Clear();
			this.updatedSuccessfully = UpdateCore() && UpdateNestedItems();
			return UpdatedSuccessfully;
		}
		public bool HitTest(DocumentLogPosition logPosition, Point logicalPoint) {
			return logPosition >= Start.LogPosition && logPosition < End.LogPosition;
		}
		#endregion
		protected virtual bool UpdateCore() {
			if (TryCreatePictureSelection())
				return true;
			TryAddFloatingObjectsToSelection();
			DocumentLayoutPosition pos = Start;
			while (pos.LogPosition < End.LogPosition)
				if (!TryGetNextPosition(ref pos))
					return false;
			return true;
		}
		bool TryCreatePictureSelection() {
			RectangularObjectSelectionLayout pictureSelection = TryCreateRectangularObjectSelectionLayout(start, end);
			if (pictureSelection == null)
				return false;
			start.Update(DocumentLayout.Pages, DocumentLayoutDetailsLevel.Page);
			AddNestedItem(start.Page.PageIndex, pictureSelection);
			return true;
		}
		void TryAddFloatingObjectsToSelection() {
			RunInfo selectionRunsInfo = PieceTable.FindRunInfo(start.LogPosition, end.LogPosition - start.LogPosition);
			DocumentModelPosition runInfo = selectionRunsInfo.Start;
			while (runInfo.LogPosition < Algorithms.Min(end.LogPosition, PieceTable.DocumentEndLogPosition + 1)) {
				DocumentLayoutPosition anchorPos;
				RectangularObjectSelectionLayout floatSelection = TryCreateRectangularObjectSelectionLayoutByAnchorRun(runInfo, out anchorPos);
				if (floatSelection != null)
					AddNestedItem(anchorPos.Page.PageIndex, floatSelection);
				runInfo = PieceTable.FindRunInfo(runInfo.RunEndLogPosition + 1, 0).Start;
			}
		}
		bool TryGetNextPosition(ref DocumentLayoutPosition pos) {
			if (!pos.Update(DocumentLayout.Pages, DocumentLayoutDetailsLevel.Row, true)) {
				pos = CreateDocumentLayoutPosition(pos.LogPosition + 1);
				return true;
			}
			Paragraph par = PieceTable.FindParagraph(pos.LogPosition);
			TableCell cell = PieceTable.TableCellsManager.GetCell(par);
			if (cell != null && cell.VerticalMerging == MergingState.Continue) { 
				pos = CreateDocumentLayoutPosition(pos.LogPosition + 1);
				return true;
			}
			DocumentLogPosition realBoxStart = pos.Row.GetFirstPosition(PieceTable).LogPosition;
			if (realBoxStart > pos.LogPosition) { 
				pos = CreateDocumentLayoutPosition(realBoxStart);
				return true;
			}
			return TryGetNextPositionCore(par, cell, ref pos);
		}
		bool TryGetNextPositionCore(Paragraph par, TableCell cell, ref DocumentLayoutPosition pos) {
			DocumentLayoutPosition stop;
			DocumentLogPosition visibleEndLogPosition = End.LogPosition - 1;
			if (cell != null && (Start.LogPosition > PieceTable.Paragraphs[cell.StartParagraphIndex].LogPosition || End.LogPosition <= PieceTable.Paragraphs[cell.EndParagraphIndex].EndLogPosition)) {
				cell = null;
				DocumentLogPosition endLogPosition = End.LogPosition < PieceTable.DocumentEndLogPosition ? End.LogPosition : PieceTable.DocumentEndLogPosition;
				visibleEndLogPosition = PieceTable.VisibleTextFilter.GetPrevVisibleLogPosition(endLogPosition, true);
				stop = CreateDocumentLayoutPosition(visibleEndLogPosition);
			}
			else {
				if (cell != null) {
					TableCell parent = cell.Table.ParentCell;
					while (parent != null && (Start.LogPosition <= PieceTable.Paragraphs[parent.StartParagraphIndex].LogPosition || End.LogPosition > PieceTable.Paragraphs[parent.EndParagraphIndex].EndLogPosition)) {
						cell = parent;
						parent = cell.Table.ParentCell;
					}
					par = PieceTable.Paragraphs[cell.EndParagraphIndex];
				}
				if (par.EndLogPosition > End.LogPosition - 1) {
					visibleEndLogPosition = PieceTable.VisibleTextFilter.GetPrevVisibleLogPosition(End.LogPosition, true);
					stop = CreateDocumentLayoutPosition(visibleEndLogPosition);
				}
				else {
					Field field = PieceTable.FindFieldByRunIndex(par.LastRunIndex);
					if (field != null && field.IsCodeView && field.Result.Contains(par.LastRunIndex) && End.LogPosition != PieceTable.DocumentEndLogPosition + 1) {
						visibleEndLogPosition = PieceTable.VisibleTextFilter.GetPrevVisibleLogPosition(End.LogPosition, true);
						stop = CreateDocumentLayoutPosition(visibleEndLogPosition);
					}
					else
						stop = CreateDocumentLayoutPosition(par.EndLogPosition);
				}
			}
			if (!stop.Update(DocumentLayout.Pages, DocumentLayoutDetailsLevel.Row, true))
				return false;
			return TryGetNextPositionBySelectionLayoutItem(stop, visibleEndLogPosition, cell, ref pos);
		}
		bool TryGetNextPositionBySelectionLayoutItem(DocumentLayoutPosition stop, DocumentLogPosition visibleEndLogPosition, TableCell cell, ref DocumentLayoutPosition pos) {
			ISelectionLayoutItem item = null;
			if (cell == null) {
				if (!Object.ReferenceEquals(pos.Row, stop.Row)) {
					DocumentLayoutPosition fracture = CreateDocumentLayoutPosition(pos.Row.GetLastPosition(PieceTable).LogPosition);
					if (!fracture.Update(DocumentLayout.Pages, DocumentLayoutDetailsLevel.Row, true))
						return false;
					if (fracture.LogPosition < stop.LogPosition)
						stop = fracture;
				}
			}
			else {
				TableCellViewInfo info = FindCellViewInfo(cell, pos.Column);
				if (info != null) {
					if (!Object.ReferenceEquals(pos.Column, stop.Column)) {
						DocumentLayoutPosition fracture = CreateDocumentLayoutPosition(info.GetLastRow(pos.Column).GetLastPosition(PieceTable).LogPosition);
						if (!fracture.Update(DocumentLayout.Pages, DocumentLayoutDetailsLevel.Row, true))
							return false;
						if (fracture.LogPosition < stop.LogPosition)
							stop = fracture;
					}
					if (cell.Table.ParentCell != null && PieceTable.Paragraphs[cell.Table.ParentCell.StartParagraphIndex].LogPosition >= Start.LogPosition && PieceTable.Paragraphs[cell.Table.ParentCell.EndParagraphIndex].LogPosition < End.LogPosition) {
						pos = CreateDocumentLayoutPosition(PieceTable.Paragraphs[cell.Table.ParentCell.EndParagraphIndex].LogPosition + 1);
						return true;
					}
					else
						item = new TableCellSelectionLayout(info, pos.Page);
				}
			}
			return TryGetNextPositionBySelectionLayoutItemCore(item, stop, visibleEndLogPosition, ref pos);
		}
		bool TryGetNextPositionBySelectionLayoutItemCore(ISelectionLayoutItem item, DocumentLayoutPosition stop, DocumentLogPosition visibleEndLogPosition, ref DocumentLayoutPosition pos) {
			if (item == null) {
				if (Object.ReferenceEquals(start, pos) || stop.LogPosition >= visibleEndLogPosition) {
					if (!pos.Update(DocumentLayout.Pages, DocumentLayoutDetailsLevel.Character, true) || !stop.Update(DocumentLayout.Pages, DocumentLayoutDetailsLevel.Character, true))
						return false;
					item = new RowSelectionLayout(pos, stop, pos.Page);
				}
				else {
					if (!pos.Update(DocumentLayout.Pages, DocumentLayoutDetailsLevel.Row, true) || !stop.Update(DocumentLayout.Pages, DocumentLayoutDetailsLevel.Row, true))
						return false;
					item = new EntireRowSelectionLayout(pos, stop, pos.Page);
				}
			}
			AddNestedItem(pos.Page.PageIndex, item);
			DocumentLogPosition nextLogPosition = PieceTable.VisibleTextFilter.GetNextVisibleLogPosition(stop.LogPosition, true, true);
			pos = CreateDocumentLayoutPosition(nextLogPosition);
			return true;
		}
		#region Floating objects support
		FloatingObjectBox FindFloatingObject(List<FloatingObjectBox> floatingObjects, RunIndex index) {
			if (floatingObjects == null)
				return null;
			PieceTable pieceTable = DocumentLayout.DocumentModel.ActivePieceTable;
			foreach (FloatingObjectBox box in floatingObjects)
				if (box.StartPos.RunIndex == index && Object.ReferenceEquals(pieceTable, box.PieceTable))
					return box;
			return null;
		}
		FloatingObjectBox FindFloatingObject(Page page, RunIndex index) {
			FloatingObjectBox resultBox = FindFloatingObject(page.InnerBackgroundFloatingObjects, index);
			if (resultBox != null)
				return resultBox;
			resultBox = FindFloatingObject(page.InnerFloatingObjects, index);
			if (resultBox != null)
				return resultBox;
			return FindFloatingObject(page.InnerForegroundFloatingObjects, index);
		}
		RectangularObjectSelectionLayout TryCreateRectangularObjectSelectionLayout(DocumentLayoutPosition start, DocumentLayoutPosition end) {
			Selection selection = DocumentLayout.DocumentModel.Selection;
			ParagraphList selectedParagraphs = selection.GetSelectedParagraphs();
			if (selection.Length == 1 || (selectedParagraphs.Count > 0 && selectedParagraphs.First.FrameProperties != null && !selectedParagraphs.First.IsInCell()))
				return TryToCreateRectangularObjectSelectionLayoutForSinglePosition(start, end);
			FieldController controller = new FieldController();
			Field field = controller.FindFieldBySelection(selection);
			if (field == null)
				return null;
			return TryToCreateRectangularObjectSelectionLayoutByField(field);
		}
		RectangularObjectSelectionLayout TryToCreateRectangularObjectSelectionLayoutForSinglePosition(DocumentLayoutPosition start, DocumentLayoutPosition end) {
			Selection selection = DocumentLayout.DocumentModel.Selection;
			if (start.LogPosition > selection.NormalizedStart || selection.NormalizedStart > end.LogPosition)
				return null;
			DocumentLayoutPosition pos = CreateDocumentLayoutPosition(selection.NormalizedStart);
			pos.Update(DocumentLayout.Pages, DocumentLayoutDetailsLevel.Box);
			if (pos.IsValid(DocumentLayoutDetailsLevel.Page)) {
				FloatingObjectBox box = FindFloatingObject(pos.Page, selection.Interval.NormalizedStart.RunIndex);
				if (box != null && Object.ReferenceEquals(box.PieceTable, DocumentLayout.DocumentModel.ActivePieceTable)) {
					RectangularObjectSelectionLayout result = new ResizeableRectangularObjectSelectionLayout(View, box, selection.NormalizedStart, pieceTable);
					result.HitTestTransform = box.CreateBackwardTransformUnsafe();
					return result;
				}
			}
			return TryToCreateRectangularObjectSelectionLayoutByBox(pos, selection.NormalizedStart);
		}
		RectangularObjectSelectionLayout TryToCreateRectangularObjectSelectionLayoutByField(Field field) {
			if (field.Result.End - field.Result.Start != 1)
				return null;
			Selection selection = DocumentLayout.DocumentModel.Selection;
			DocumentLogPosition logPos = selection.PieceTable.GetRunLogPosition(field.Result.Start);
			DocumentLayoutPosition pos = CreateDocumentLayoutPosition(logPos);
			pos.Update(DocumentLayout.Pages, DocumentLayoutDetailsLevel.Box);
			return TryToCreateRectangularObjectSelectionLayoutByBox(pos, logPos);
		}
		RectangularObjectSelectionLayout TryToCreateRectangularObjectSelectionLayoutByBox(DocumentLayoutPosition layoutPos, DocumentLogPosition logPos) {
			if (layoutPos.IsValid(DocumentLayoutDetailsLevel.Box)) {
				InlinePictureBox inlinePicture = layoutPos.Box as InlinePictureBox;
				if (inlinePicture != null)
					return new ResizeableRectangularObjectSelectionLayout(View, inlinePicture, logPos, pieceTable);
				CustomRunBox customRunBox = layoutPos.Box as CustomRunBox;
				if (customRunBox != null) {
					IRectangularObject customRunObject = customRunBox.GetCustomRun(pieceTable).GetRectangularObject();
					if (customRunObject != null) {
						IResizeableObject resizeableObject = customRunObject as IResizeableObject;
						bool resizeable = (resizeableObject != null) && resizeableObject.Resizeable;
						return resizeable ?
							new ResizeableRectangularObjectSelectionLayout(View, customRunBox, logPos, pieceTable) :
							new RectangularObjectSelectionLayout(View, customRunBox, logPos, pieceTable);
					}
				}
			}
			return null;
		}
		RectangularObjectSelectionLayout TryToCreateRectangularObjectSelectionLayoutByAnchorRun(DocumentModelPosition runInfo) {
			DocumentLayoutPosition pos;
			return TryCreateRectangularObjectSelectionLayoutByAnchorRun(runInfo, out pos);
		}
		RectangularObjectSelectionLayout TryCreateRectangularObjectSelectionLayoutByAnchorRun(DocumentModelPosition runInfo, out DocumentLayoutPosition pos) {
			pos = null;
			Selection selection = DocumentLayout.DocumentModel.Selection;
			if (start.LogPosition > selection.NormalizedStart || selection.NormalizedStart > end.LogPosition)
				return null;
			if (!(PieceTable.Runs[runInfo.RunIndex] is FloatingObjectAnchorRun))
				return null;
			pos = CreateDocumentLayoutPosition(runInfo.RunStartLogPosition);
			pos.Update(DocumentLayout.Pages, DocumentLayoutDetailsLevel.Box);
			if (!pos.IsValid(DocumentLayoutDetailsLevel.Page))
				return null;
			FloatingObjectBox box = FindFloatingObject(pos.Page, runInfo.RunIndex);
			if (box != null && Object.ReferenceEquals(box.PieceTable, DocumentLayout.DocumentModel.ActivePieceTable)) {
				RectangularObjectSelectionLayout result = new RectangularObjectSelectionLayout(View, box, selection.NormalizedStart, pieceTable);
				result.HitTestTransform = box.CreateBackwardTransformUnsafe();
				return result;
			}
			return null;
		}
		protected bool UpdateNestedItems() {
			foreach (List<ISelectionLayoutItem> list in NestedItems.Values)
				foreach (ISelectionLayoutItem item in list)
					if (!item.Update())
						return false;
			return true;
		}
		#endregion
	}
	public abstract class RowSelectionLayoutBase : ISelectionLayoutItem {
		#region Fields
		Rectangle bounds;
		readonly Page page;
		#endregion
		protected RowSelectionLayoutBase(Page page) {
			this.page = page;
		}
		#region Properties
		protected internal Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		public Page Page { get { return page; } }
		#endregion
		#region ISelectionLayoutItem Members
		public virtual void Draw(ISelectionPainter selectionPainter) {
			selectionPainter.Draw(this);
		}
		public bool HitTest(DocumentLogPosition logPosition, Point logicalPoint) {
			return Bounds.Contains(logicalPoint);
		}
		public abstract bool Update();
		#endregion
	}
	public class EntireRowSelectionLayout : RowSelectionLayoutBase {
		#region Fields
		readonly DocumentLayoutPosition start;
		readonly DocumentLayoutPosition end;
		#endregion
		public EntireRowSelectionLayout(DocumentLayoutPosition start, DocumentLayoutPosition end, Page page)
			: base(page) {
			Guard.ArgumentNotNull(start, "start");
			Guard.ArgumentNotNull(end, "end");
			Debug.Assert(start.IsValid(DocumentLayoutDetailsLevel.Row));
			Debug.Assert(end.IsValid(DocumentLayoutDetailsLevel.Row));
			Debug.Assert(Object.ReferenceEquals(start.Row, end.Row));
			this.start = start;
			this.end = end;
		}
		#region Properties
		public DocumentLayoutPosition Start { get { return start; } }
		public DocumentLayoutPosition End { get { return end; } }
		#endregion
		public override bool Update() {
			BoxCollection rowBoxes = Start.Row.Boxes;
			Rectangle startCharacterBounds = rowBoxes.First.Bounds;
			Rectangle endCharacterBounds = rowBoxes.Last.Bounds;
			Rectangle rowBounds = Start.Row.Bounds;
			Bounds = new Rectangle(startCharacterBounds.Left, rowBounds.Top, endCharacterBounds.Right - startCharacterBounds.Left, rowBounds.Height);
			return true;
		}
	}
	public class RowSelectionLayout : RowSelectionLayoutBase {
		#region Fields
		readonly DocumentLayoutPosition start;
		readonly DocumentLayoutPosition end;
		#endregion
		public RowSelectionLayout(DocumentLayoutPosition start, DocumentLayoutPosition end, Page page)
			: base(page) {
			Guard.ArgumentNotNull(start, "start");
			Guard.ArgumentNotNull(end, "end");
			Debug.Assert(start.IsValid(DocumentLayoutDetailsLevel.Character));
			Debug.Assert(end.IsValid(DocumentLayoutDetailsLevel.Character));
			this.start = start;
			this.end = end;
		}
		#region Properties
		public DocumentLayoutPosition Start { get { return start; } }
		public DocumentLayoutPosition End { get { return end; } }
		#endregion
		public override bool Update() {
			Rectangle startCharacterBounds = Start.Character.Bounds;
			Rectangle endCharacterBounds = End.Character.Bounds;
			Bounds = new Rectangle(startCharacterBounds.Left, startCharacterBounds.Top, endCharacterBounds.Right - startCharacterBounds.Left, endCharacterBounds.Bottom - startCharacterBounds.Top);
			return true;
		}
	}
	public class TableCellSelectionLayout : RowSelectionLayoutBase {
		readonly TableCellViewInfo cell;
		public TableCellSelectionLayout(TableCellViewInfo cell, Page page)
			: base(page) {
			this.cell = cell;
		}
		TableCellViewInfo Cell { get { return cell; } }
		#region ISelectionLayoutItem Members
		public override bool Update() {
			Bounds = Cell.TableViewInfo.GetCellBounds(Cell);
			return true;
		}
		#endregion
	}
	public class FloatingObjectAnchorSelectionLayout : RectangularObjectSelectionLayoutBase {
		#region Fields
		Rectangle anchorBounds;
		#endregion
		public FloatingObjectAnchorSelectionLayout(RichEditView view, Box box, DocumentLogPosition start, PieceTable pieceTable) : base(view, box, start, pieceTable) { }
		#region Properties
		public Rectangle AnchorBounds { get { return anchorBounds; } set { anchorBounds = value; } }
		#endregion
		#region ISelectionLayoutItem Members
		public override void Draw(ISelectionPainter selectionPainter) {
			selectionPainter.Draw(this);
		}
		public override bool Update() {
			return true;
		}
		public override bool HitTest(DocumentLogPosition logPosition, Point logicalPoint) {
			return AnchorBounds.Contains(logicalPoint);
		}
		#endregion
	}
	#endregion
	#region LayoutItem Collections
	public class PageSelectionLayoutsCollection : List<ISelectionLayoutItem> { }
	#endregion
}
