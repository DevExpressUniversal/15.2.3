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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Snap.Core.Native.Templates;
using DevExpress.Snap.Native.LayoutUI;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Mouse;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office.Drawing;
using DevExpress.Snap.Core;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.Snap.Core.Native;
using DevExpress.XtraRichEdit.Tables.Native;
namespace DevExpress.Snap.Native.HoverDecorators {
	public interface ITableDropZoneVisitor : IHotZoneVisitor {
		void Visit(DropFieldTableHotZoneBase hotZone);
	}
	public enum InsertColumnType { InsertToRight, InsertToLeft }
	public class SNListInfo {
		Field field;
		string dataSource;
		string dataMember;
		bool isNested;
		public SNListInfo(Field field, string dataSource, string dataMember, bool isNested) {
			this.field = field;
			this.dataSource = dataSource;
			this.dataMember = dataMember;
			this.isNested = isNested;
		}
		public Field Field { get { return field; } }
		public string DataSource { get { return dataSource; } }
		public string DataMember { get { return dataMember; } }
		public bool IsNested { get { return isNested; } }
	}
	public abstract class DropFieldTableHotZoneBase : HotZone {
		bool isAllowed;
		bool isNested;
		string dataSource;
		string dataMember;
		Field field;
		protected DropFieldTableHotZoneBase(Rectangle renderBounds, SNListInfo listInfo) {
			Bounds = renderBounds;
			this.dataSource = listInfo.DataSource;
			this.dataMember = listInfo.DataMember;
			this.field = listInfo.Field;
			this.isNested = listInfo.IsNested;
		}
		protected internal bool IsAllowed { get { return isAllowed; } }
		protected internal bool IsNested { get { return isNested; } }
		protected internal string DataSource { get { return dataSource; } }
		protected internal string DataMember { get { return dataMember; } }
		protected internal Field Field { get { return field; } }
		public override RichEditCursor Cursor { get { return RichEditCursors.Default; } }
		public override void Activate(RichEditMouseHandler handler, RichEditHitTestResult result) { }
		public override bool BeforeActivate(RichEditMouseHandler handler, RichEditHitTestResult result) {
			return false;
		}
		public override bool HitTest(Point point, float dpi, float zoomFactor) {
			return false;
		}
		protected override void AcceptCore(IHotZoneVisitor visitor) {
			Accept((ITableDropZoneVisitor)visitor);
		}
		void Accept(ITableDropZoneVisitor visitor) {
			visitor.Visit(this);
		}
		public void CheckIsAllowed(DraggedFieldDataInfo[] draggedFields) {
			this.isAllowed = CheckIsAllowedCore(draggedFields);
		}
		protected virtual bool CheckIsAllowedCore(DraggedFieldDataInfo[] draggedFields) {
			int length = draggedFields.Length;
			for (int i = 0; i < length; i++) {
				DraggedFieldDataInfo info = draggedFields[i];
				if (info.IsParameter)
					continue;
				if (String.Compare(DataSource, info.DataSource, true) != 0)
					return false;
				if (String.Compare(DataMember, info.DataMember, true) != 0)
					return false;
			}
			return true;
		}
		public void OnDragDrop(DragEventArgs e) {
			if (Object.ReferenceEquals(Field, null)) return;
			SNDataInfo[] dataInfo = BeforeDragDrop(e.Data);
			if (Object.ReferenceEquals(dataInfo, null) || dataInfo.Length == 0) return;
			OnDragDropCore(dataInfo);
			AfterDragDrop();
		}
		protected virtual SNDataInfo[] BeforeDragDrop(IDataObject data) {
			return SnapDataHelper.GetDataInfo(data);
		}
		protected abstract void OnDragDropCore(SNDataInfo[] dataInfo);
		protected abstract void AfterDragDrop();
	}
	public class DropFieldTableHotZone : DropFieldTableHotZoneBase {
		InsertColumnType insertColumnType;
		TableCell owner;
		public DropFieldTableHotZone(Rectangle renderBounds, SNListInfo listInfo, TableCell owner, InsertColumnType insertColumnType)
			: base(renderBounds, listInfo) {
				this.owner = owner;
				this.insertColumnType = insertColumnType;
		}
		protected internal TableCell Owner { get { return owner; } }
		protected internal InsertColumnType InsertColumnType { get { return insertColumnType; } }
		protected internal PieceTable PieceTable { get { return owner.PieceTable; } }
		protected internal SnapDocumentModel DocumentModel { get { return (SnapDocumentModel)owner.DocumentModel; } }
		protected override SNDataInfo[] BeforeDragDrop(IDataObject data) {
			SNDataInfo[] dataInfo = base.BeforeDragDrop(data);
			SnapDocumentModel model = DocumentModel;
			int targetColumnIndex = Owner.IndexInRow;
			if (InsertColumnType == HoverDecorators.InsertColumnType.InsertToRight)
				targetColumnIndex++;
			BeforeInsertSnListColumnsEventArgs e = new BeforeInsertSnListColumnsEventArgs(Field, targetColumnIndex, dataInfo);
			model.RaiseBeforeInsertSnListColumns(e);
			return e.DataFields;
		}
		protected override void OnDragDropCore(SNDataInfo[] dataInfo) {
			SnapDocumentModel model = DocumentModel;
			model.BeginUpdate();
			try {
				InsertTableColumns(dataInfo.Length);
				InsertDataInfoTemplates(dataInfo);
				PieceTable.FieldUpdater.UpdateFieldAndNestedFields(Field);
			}
			finally {
				model.EndUpdate();
			}
		}
		protected override void AfterDragDrop() {
			SnapDocumentModel model = DocumentModel;
			PieceTable pieceTable = model.MainPieceTable;
			Field modelField = pieceTable.FindFieldByRunIndex(pieceTable.Paragraphs[Owner.Table.StartParagraphIndex].FirstRunIndex);
			model.RaiseAfterInsertSnListColumns(new AfterInsertSnListColumnsEventArgs(modelField));
		}
		void InsertTableColumns(int count) {
			PieceTable pieceTable = PieceTable;
			Action<TableCell, bool> insertColumn;
			if (InsertColumnType == HoverDecorators.InsertColumnType.InsertToLeft)
				insertColumn = pieceTable.InsertColumnToTheLeft;
			else
				insertColumn = pieceTable.InsertColumnToTheRight;
			for (int i = 0; i < count; i++)
				insertColumn(Owner, true);
		}
		void InsertDataInfoTemplates(SNDataInfo[] dataInfo) {
			bool insertedToRight = InsertColumnType == HoverDecorators.InsertColumnType.InsertToRight;
			int count = dataInfo.Length;
			TableCell targetCell = Owner;
			if (insertedToRight)
				targetCell = targetCell.Next;
			else
				for (int i = 0; i < count; i++)
					targetCell = targetCell.Previous;
			for (int i = 0; i < count; i++) {
				InsertColumnTemplateUpdater rowUpdater = new InsertColumnTemplateUpdater(targetCell, dataInfo[i], insertedToRight);
				rowUpdater.UpdateTemplates();
				targetCell = targetCell.Next;
			}
		}
	}
	public class DropFieldTableMasterDetailHotZone : DropFieldTableHotZoneBase {
		TableRow owner;
		public DropFieldTableMasterDetailHotZone(Rectangle renderBounds, SNListInfo listInfo, TableRow owner)
			: base(renderBounds, listInfo) {
				this.owner = owner;
		}
		protected internal TableRow Owner { get { return owner; } }
		protected internal SnapPieceTable PieceTable { get { return (SnapPieceTable)owner.PieceTable; } }
		protected internal SnapDocumentModel DocumentModel { get { return (SnapDocumentModel)owner.DocumentModel; } }
		protected override SNDataInfo[] BeforeDragDrop(IDataObject data) {
			SNDataInfo[] dataInfo = base.BeforeDragDrop(data);
			SnapDocumentModel model = DocumentModel;
			BeforeInsertSnListDetailEventArgs e = new BeforeInsertSnListDetailEventArgs(Field, dataInfo);
			model.RaiseBeforeInsertSnListDetail(e);
			return e.DataFields;
		}
		protected override void OnDragDropCore(SNDataInfo[] dataInfo) {
			SnapDocumentModel model = DocumentModel;
			model.BeginUpdate();
			try {
				InsertTableRow();
				InsertDataInfoTemplates(dataInfo);
				PieceTable.FieldUpdater.UpdateFieldAndNestedFields(Field);
			}
			finally {
				model.EndUpdate();
			}
		}
		void InsertTableRow() {
			SnapPieceTable pieceTable = PieceTable;
			pieceTable.InsertTableRowBelow(Owner, false);
			TableStructureBySelectionCalculator calculator = new TableStructureBySelectionCalculator(pieceTable);
			SelectedCellsCollection selectedCells = calculator.Calculate(Owner.Next.FirstCell, Owner.Next.LastCell);
			pieceTable.MergeCells(selectedCells);
		}
		void InsertDataInfoTemplates(SNDataInfo[] dataInfo) {
			InsertDetailListTemplateUpdater updater = new InsertDetailListTemplateUpdater(Owner.Next.FirstCell, dataInfo);
			updater.UpdateTemplates();
		}
		protected override void AfterDragDrop() {
			SnapDocumentModel model = DocumentModel;
			PieceTable pieceTable = model.MainPieceTable;
			Field modelField = pieceTable.FindFieldByRunIndex(pieceTable.Paragraphs[Owner.Table.StartParagraphIndex].FirstRunIndex);
			model.RaiseAfterInsertSnListDetail(new AfterInsertSnListDetailEventArgs(modelField));
		}
		protected override bool CheckIsAllowedCore(DraggedFieldDataInfo[] draggedFields) {
			int length = draggedFields.Length;
			for (int i = 0; i < length; i++) {
				DraggedFieldDataInfo draggedField = draggedFields[i];
				if (String.IsNullOrEmpty(draggedField.DataMember))
					return false;
				if (String.Compare(draggedField.DataSource, DataSource, true) != 0)
					return false;
				string parentDataMember = string.Empty;
				int parentPathLength = draggedField.EscapedDataPaths.Length - 2;
				if (parentPathLength > 0) {
					string[] parentDataPaths = new string[parentPathLength];
					Array.Copy(draggedField.EscapedDataPaths, parentDataPaths, parentPathLength);
					parentDataMember = string.Join(".", parentDataPaths);
				}
				if (String.Compare(parentDataMember, DataMember, true) != 0)
					return false;
			}
			return true;
		}
	}
	public class DragExternalContentTableHotZonePainter : ITableViewInfoDecorator, ITableDropZoneVisitor {
		readonly DragExternalContentTableViewInfoController tableViewInfoController;
		readonly DropFieldTableHotZonePainter hotZonePainter;
		public DragExternalContentTableHotZonePainter(DragExternalContentTableViewInfoController tableViewInfoController, Painter painter) {
			this.tableViewInfoController = tableViewInfoController;
			this.hotZonePainter = new DropFieldTableHotZonePainter(painter, tableViewInfoController.CreateConverter());
		}
		protected DragExternalContentTableViewInfoController TableViewInfoController { get { return tableViewInfoController; } }
		protected DropFieldTableHotZonePainter HotZonePainter { get { return hotZonePainter; } }
		public void Decorate() {
			IHotZone hotZone = TableViewInfoController.VisibleHotZone;
			if (hotZone != null)
				hotZone.Accept(this);
		}
		void ITableDropZoneVisitor.Visit(DropFieldTableHotZoneBase hotZone) {
			HotZonePainter.DrawHotZone(hotZone);
		}
		public void Dispose() {
			HotZonePainter.Dispose();
		}
	}
}
