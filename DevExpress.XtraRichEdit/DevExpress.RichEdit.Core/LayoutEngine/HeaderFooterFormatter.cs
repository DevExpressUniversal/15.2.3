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
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraRichEdit.Layout.Engine {
	#region PageHeaderFooterFormatterBase<TModelObject, TLayoutObject> (abstract class)
	public abstract class PageHeaderFooterFormatterBase<TModelObject, TLayoutObject>
		where TModelObject : ContentTypeBase
		where TLayoutObject : HeaderFooterPageAreaBase {
		#region Fields
		readonly DocumentLayout documentLayout;
		readonly FloatingObjectsLayout floatingObjectsLayout;
		readonly ParagraphFramesLayout paragraphFramesLayout;
		readonly Page page;
		readonly SectionIndex sectionIndex;
		readonly Section section;
		#endregion
		protected PageHeaderFooterFormatterBase(DocumentLayout documentLayout, FloatingObjectsLayout floatingObjectsLayout, ParagraphFramesLayout paragraphFramesLayout, Page page, SectionIndex sectionIndex) {
			Guard.ArgumentNotNull(documentLayout, "documentLayout");
			Guard.ArgumentNotNull(page, "page");
			Guard.ArgumentNotNull(floatingObjectsLayout, "floatingObjectsLayout");
			Guard.ArgumentNotNull(paragraphFramesLayout, "paragraphFramesLayout");
			this.documentLayout = documentLayout;
			this.floatingObjectsLayout = floatingObjectsLayout;
			this.paragraphFramesLayout = paragraphFramesLayout;
			this.page = page;
			this.sectionIndex = sectionIndex;
			this.section = documentLayout.DocumentModel.Sections[sectionIndex];
		}
		#region Properties
		public DocumentLayout DocumentLayout { get { return documentLayout; } }
		public Page Page { get { return page; } }
		public SectionIndex SectionIndex { get { return sectionIndex; } }
		public Section Section { get { return section; } }
		protected FloatingObjectsLayout FloatingObjectsLayout { get { return floatingObjectsLayout; } }
		protected ParagraphFramesLayout ParagraphFramesLayout { get { return paragraphFramesLayout; } }
		#endregion
		public virtual void Format(bool firstPageOfSection) {
			TModelObject pieceTable = GetActualModelObject(firstPageOfSection);
			if (pieceTable == null)
				return;
			TLayoutObject area = CreatePageArea(pieceTable);
			AppendPageAreaToPage(area);
			Rectangle availableAreaBounds = CalculateAvailableAreaBounds();
			Rectangle initialAreaBounds = availableAreaBounds;
			initialAreaBounds.Height = Int32.MaxValue / 4;
			area.Bounds = initialAreaBounds;
			int actualAreaBottom = FormatArea(pieceTable, area, initialAreaBounds);
			area.Bounds = availableAreaBounds;
			Rectangle newAreaBounds = CalculateFinalAreaBounds(area, actualAreaBottom);
			int deltaY = newAreaBounds.Y - area.Bounds.Y;
			area.MoveVertically(deltaY);
			MoveParagraphFramesVertically(pieceTable.PieceTable, deltaY);
			MoveFloatingObjectsVertically(pieceTable.PieceTable, deltaY, ShouldMoveFloatingObjectVertically);
			area.ContentBounds = newAreaBounds;
			newAreaBounds = AdjustNewAreaBounds(pieceTable.PieceTable, newAreaBounds);
			area.Bounds = newAreaBounds;
			area.Columns.First.Bounds = newAreaBounds;
			Rectangle oldPageClientBounds = page.ClientBounds;
			page.ClientBounds = CalculateFinalAdjustedPageClientBounds(area);
			Rectangle newPageClientBounds = page.ClientBounds;
			deltaY = CalculateDeltaAfterAdjustPageClientBounds(oldPageClientBounds, newPageClientBounds, area.Columns.First.Bounds);
			if (deltaY != 0)
				MoveFloatingObjectsVertically(pieceTable.PieceTable, deltaY, ShouldMoveFloatingObjectVerticallyAfterAdjustPageClientBounds);
		}
		private bool ShouldMoveFloatingObjectVerticallyAfterAdjustPageClientBounds(FloatingObjectBox box) {
			FloatingObjectAnchorRun run = box.GetFloatingObjectRun();
			FloatingObjectVerticalPositionType verticalPositionType = run.FloatingObjectProperties.VerticalPositionType;
			return verticalPositionType == FloatingObjectVerticalPositionType.Margin;
		}
		protected abstract int CalculateDeltaAfterAdjustPageClientBounds(Rectangle oldPageClientBounds, Rectangle newPageClientBounds, Rectangle columnBounds);
		public virtual void ApplyExistingAreaBounds() {
			TLayoutObject existingArea = GetExistingArea(page);
			if (existingArea != null)
				page.ClientBounds = CalculateFinalAdjustedPageClientBounds(existingArea);
		}
		protected abstract TLayoutObject GetExistingArea(Page page);
		protected virtual void MoveParagraphFramesVertically(PieceTable pieceTable, int deltaY) {
			int count = paragraphFramesLayout.Items.Count;
			for (int i = 0; i < count; i++) {
				ParagraphFrameBox box = paragraphFramesLayout.Items[i];
				if (box.PieceTable.IsFooter)
					box.MoveVertically(deltaY);
			}
		}
		protected virtual void MoveFloatingObjectsVertically(PieceTable pieceTable, int deltaY, Func<FloatingObjectBox, bool> shouldMovePredicate) {
			IList<FloatingObjectBox> objects = floatingObjectsLayout.GetFloatingObjects(pieceTable);
			int count = objects.Count;
			for (int i = 0; i < count; i++) {
				FloatingObjectBox box = objects[i];
				if (shouldMovePredicate(box))
					box.MoveVertically(deltaY);
			}
		}
		protected virtual bool ShouldMoveFloatingObjectVertically(FloatingObjectBox box) {
			FloatingObjectAnchorRun run = box.GetFloatingObjectRun();
			FloatingObjectVerticalPositionType verticalPositionType = run.FloatingObjectProperties.VerticalPositionType;
			return verticalPositionType == FloatingObjectVerticalPositionType.Line || verticalPositionType == FloatingObjectVerticalPositionType.Margin || verticalPositionType == FloatingObjectVerticalPositionType.Paragraph;
		}
		protected internal virtual int FormatArea(TModelObject contentType, TLayoutObject area, Rectangle availableAreaBounds) {
			BoxMeasurer measurer = DocumentLayout.Measurer;
			PieceTable oldPieceTable = measurer.PieceTable;
			try {
				PieceTable pieceTable = contentType.PieceTable;
				measurer.PieceTable = pieceTable;
				HeaderFooterDocumentFormattingController controller = new HeaderFooterDocumentFormattingController(DocumentLayout, floatingObjectsLayout, paragraphFramesLayout, pieceTable, area, SectionIndex, page);
				Page fakePage = controller.PageController.Pages.Last;
				fakePage.Bounds = page.Bounds;
				fakePage.ClientBounds = page.ClientBounds;
				SimplePieceTablePrimaryFormatter formatter = new SimplePieceTablePrimaryFormatter(pieceTable, measurer, controller.RowsController, oldPieceTable.VisibleTextFilter.Clone(pieceTable), Page);
				return formatter.Format(availableAreaBounds.Bottom, controller);
			}
			finally {
				measurer.PieceTable = oldPieceTable;
			}
		}
		protected int GetPageAreaBottom(PageArea area, int actualAreaBottom) {
			int initialBottom = area.Bounds.Bottom;
			if (actualAreaBottom > initialBottom) {
				Column column = area.Columns.First;
				if (column != null) {
					Row row = FindRow(column, initialBottom);
					if (row != null)
						return row.Bounds.Bottom;
				}
				return initialBottom;
			}
			else
				return actualAreaBottom;
		}
		Row FindRow(Column column, int y) {
			TableLayout.TableViewInfoCollection tables = column.InnerTables;
			if (tables != null && tables.Count > 0) {
				TableLayout.TableViewInfo table = FindTable(tables, y);
				if (table != null)
					return FindRow(table, y);
			}
			return FindRow(column.Rows, y);
		}
		TableLayout.TableViewInfo FindTable(TableLayout.TableViewInfoCollection tables, int y) {
			foreach (TableLayout.TableViewInfo table in tables) {
				int tableBottom = table.GetActualBottomPosition();
				if (table.Anchors.First.VerticalPosition <= y && tableBottom >= y)
					return table;
			}
			return null;
		}
		Row FindRow(TableLayout.TableViewInfo table, int y) {
			int tableRowIndex = Algorithms.BinarySearch(table.Rows, new TableRowAnchorComparable(y, table.Table.Rows.Last));
			if (tableRowIndex < 0)
				return null;
			TableLayout.TableRowViewInfoBase rowViewInfo = table.Rows[tableRowIndex];
			foreach (TableLayout.TableCellViewInfo cellViewInfo in rowViewInfo.Cells) {
				RowCollection tableCellRows = cellViewInfo.GetRows(table.Column);
				Row row = FindRow(tableCellRows, y);
				if (row != null)
					return row;
			}
			return null;
		}
		Row FindRow(RowCollection rows, int y) {
			int rowIndex = Algorithms.BinarySearch(rows, new BoxAndPointYComparable<Row>(new Point(0, y)));
			if (rowIndex >= 0)
				return rows[rowIndex];
			return null;
		}
		protected internal abstract TModelObject GetActualModelObject(bool firstPageOfSection);
		protected internal abstract TLayoutObject CreatePageArea(TModelObject pieceTable);
		protected internal abstract void AppendPageAreaToPage(TLayoutObject area);
		protected internal abstract Rectangle CalculateAvailableAreaBounds();
		protected internal abstract Rectangle CalculateFinalAreaBounds(TLayoutObject area, int actualAreaBottom);
		protected internal abstract Rectangle CalculateFinalAdjustedPageClientBounds(TLayoutObject area);
		protected internal abstract Rectangle AdjustNewAreaBounds(PieceTable pieceTable, Rectangle newAreaBounds);
	}
	#endregion
	#region PageHeaderFormatter
	public class PageHeaderFormatter : PageHeaderFooterFormatterBase<SectionHeader, HeaderPageArea> {
		public PageHeaderFormatter(DocumentLayout documentLayout, FloatingObjectsLayout floatingObjectsLayout, ParagraphFramesLayout paragraphFramesLayout, Page page, SectionIndex sectionIndex)
			: base(documentLayout, floatingObjectsLayout, paragraphFramesLayout, page, sectionIndex) {
		}
		protected override int CalculateDeltaAfterAdjustPageClientBounds(Rectangle oldPageClientBounds, Rectangle newPageClientBounds, Rectangle columnBounds) {
			return newPageClientBounds.Y - columnBounds.Y;
		}
		protected internal override SectionHeader GetActualModelObject(bool firstPageOfSection) {
			return Section.Headers.CalculateActualObject(firstPageOfSection, Page.IsEven);
		}
		protected override HeaderPageArea GetExistingArea(Page page) {
			return page.Header;
		}
		protected internal override HeaderPageArea CreatePageArea(SectionHeader pieceTable) {
			return new HeaderPageArea(pieceTable, Section);
		}
		protected internal override void AppendPageAreaToPage(HeaderPageArea area) {
			Page.Header = area;
		}
		protected internal override Rectangle CalculateAvailableAreaBounds() {
			Rectangle bounds = Page.ClientBounds;
			bounds.Y = DocumentLayout.DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(Section.Margins.HeaderOffset);
			bounds.Height = (int)(Page.Bounds.Height / 2);
			return bounds;
		}
		protected internal override Rectangle CalculateFinalAreaBounds(HeaderPageArea area, int actualAreaBottom) {
			Rectangle result = area.Bounds;
			int areaBottom = GetPageAreaBottom(area, actualAreaBottom);
			result.Height = Math.Max(0, areaBottom - area.Bounds.Y);
			return result;
		}
		protected internal override Rectangle CalculateFinalAdjustedPageClientBounds(HeaderPageArea area) {
			if (Section.Margins.Top < 0)
				return Page.ClientBounds;
			Rectangle result = Page.ClientBounds;
			int deltaClientTop = Math.Max(0, area.Bounds.Bottom - result.Top);
			result.Y += deltaClientTop;
			result.Height -= deltaClientTop;
			return result;
		}
		protected internal override Rectangle AdjustNewAreaBounds(PieceTable pieceTable, Rectangle newAreaBounds) {
			return newAreaBounds;
		}
	}
	#endregion
	#region PageFooterFormatter
	public class PageFooterFormatter : PageHeaderFooterFormatterBase<SectionFooter, FooterPageArea> {
		public PageFooterFormatter(DocumentLayout documentLayout, FloatingObjectsLayout floatingObjectsLayout, ParagraphFramesLayout paragraphFramesLayout, Page page, SectionIndex sectionIndex)
			: base(documentLayout, floatingObjectsLayout, paragraphFramesLayout, page, sectionIndex) {
		}
		protected override int CalculateDeltaAfterAdjustPageClientBounds(Rectangle oldPageClientBounds, Rectangle newPageClientBounds, Rectangle columnBounds) {
			return 0;
		}
		protected internal override SectionFooter GetActualModelObject(bool firstPageOfSection) {
			return Section.Footers.CalculateActualObject(firstPageOfSection, Page.IsEven);
		}
		protected override FooterPageArea GetExistingArea(Page page) {
			return page.Footer;
		}
		protected internal override FooterPageArea CreatePageArea(SectionFooter pieceTable) {
			return new FooterPageArea(pieceTable, Section);
		}
		protected internal override void AppendPageAreaToPage(FooterPageArea area) {
			Page.Footer = area;
		}
		protected internal override Rectangle CalculateAvailableAreaBounds() {
			Rectangle bounds = Page.ClientBounds;
			int maxHeight = (int)(Page.Bounds.Height / 2);
			bounds.Y = Page.Bounds.Height - maxHeight - DocumentLayout.DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(Section.Margins.FooterOffset);
			bounds.Height = maxHeight;
			return bounds;
		}
		protected internal override Rectangle CalculateFinalAreaBounds(FooterPageArea area, int actualAreaBottom) {
			Rectangle result = area.Bounds;
			int initialBottom = result.Bottom;
			int areaBottom = GetPageAreaBottom(area, actualAreaBottom);
			result.Height = areaBottom - result.Y;
			result.Y = initialBottom - result.Height;
			return result;
		}
		protected override void MoveFloatingObjectsVertically(PieceTable pieceTable, int deltaY, Func<FloatingObjectBox, bool> shouldMovePredicate) {
			int floatingObjectsBottom = CalculateFloatingObjectsBottomWithDelta(pieceTable, deltaY);
			int pageBottom = Page.Bounds.Bottom;
			if (floatingObjectsBottom > pageBottom)
				deltaY -= floatingObjectsBottom - pageBottom;
			base.MoveFloatingObjectsVertically(pieceTable, deltaY, shouldMovePredicate);
		}
		protected internal override Rectangle AdjustNewAreaBounds(PieceTable pieceTable, Rectangle newAreaBounds) {
			int floatingObjectsBottom = CalculateFloatingObjectsBottom(pieceTable, newAreaBounds.Top);
			int bottom = Math.Min(Page.Bounds.Bottom, Math.Max(newAreaBounds.Bottom, floatingObjectsBottom));
			newAreaBounds.Height = bottom - newAreaBounds.Top;
			return newAreaBounds;
		}
		int CalculateFloatingObjectsBottom(PieceTable pieceTable, int initialBottom) {
			return CalculateFloatingObjectsBottomWithDelta(pieceTable, initialBottom, 0);
		}
		int CalculateFloatingObjectsBottomWithDelta(PieceTable pieceTable, int initialBottom, int delta) {
			int result = initialBottom;
			IList<FloatingObjectBox> floatingObjects = FloatingObjectsLayout.GetFloatingObjects(pieceTable);
			int count = floatingObjects.Count;
			for (int i = 0; i < count; i++)
				if (ShouldMoveFloatingObjectVertically(floatingObjects[i]))
					result = Math.Max(result, floatingObjects[i].Bounds.Bottom + delta);
			return result;
		}
		int CalculateFloatingObjectsBottomWithDelta(PieceTable pieceTable, int delta) {
			return CalculateFloatingObjectsBottomWithDelta(pieceTable, int.MinValue, delta);
		}
		protected internal override Rectangle CalculateFinalAdjustedPageClientBounds(FooterPageArea area) {
			if (Section.Margins.Bottom < 0)
				return Page.ClientBounds;
			Rectangle result = Page.ClientBounds;
			result.Height = Math.Min(area.Bounds.Top, result.Bottom) - result.Top;
			return result;
		}
	}
	#endregion
}
