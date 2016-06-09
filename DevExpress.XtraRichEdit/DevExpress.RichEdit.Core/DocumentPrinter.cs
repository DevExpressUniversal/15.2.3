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
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Collections.Specialized;
#if SL
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Xpf.Core.Native;
using DevExpress.XtraRichEdit.Forms;
#else
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Forms;
#endif
namespace DevExpress.XtraRichEdit.Layout.Export {
	#region PlainTextDocumentLayoutExporter
	public class PlainTextDocumentLayoutExporter : DocumentLayoutExporter {
		readonly StringCollection strings = new StringCollection();
		readonly StringBuilder builder = new StringBuilder();
		readonly bool newLineOnEachRow;
		public PlainTextDocumentLayoutExporter(DocumentModel documentModel, bool newLineOnEachRow)
			: base(documentModel) {
			this.newLineOnEachRow = newLineOnEachRow;
		}
		public override DevExpress.Office.Drawing.Painter Painter {
			get { throw new NotImplementedException(); }
		}
		public string GetResultingText() {
			StringBuilder resultBuilder = new StringBuilder();
			int count = strings.Count;
			for (int i = 0; i < count; i++) {
				resultBuilder.Append(strings[i]);
				if (i < count - 1)
					resultBuilder.Append("\r\n");
			}
			return resultBuilder.ToString();
		}
		void MarkNewLine() {
			strings.Add(builder.ToString());
			builder.Length = 0;
		}
		#region IDocumentLayoutExporter implementation
		protected internal override void ExportRowCore() {
			base.ExportRowCore();
			if (newLineOnEachRow)
				MarkNewLine();
		}
		public override void ExportTextBox(TextBox box) {
			AddBox(box);
		}
		public override void ExportSpaceBox(Box box) {
			AddBox(box);
		}
		public override void ExportTabSpaceBox(TabSpaceBox box) {
			AddBox(box);
		}
		public override void ExportParagraphMarkBox(ParagraphMarkBox box) {
			if (!newLineOnEachRow)
				MarkNewLine();
		}
		public override void ExportSectionMarkBox(SectionMarkBox box) {
			if (!newLineOnEachRow)
				MarkNewLine();
		}
		void AddBox(Box box) {
			builder.Append(GetBoxText(box));
		}
		public override void ExportCustomRunBox(CustomRunBox box) {
		}
		public override void ExportSeparatorBox(SeparatorBox box) {
		}
		public override void ExportDataContainerRunBox(DataContainerRunBox box) {
		}
		#endregion
	}
	#endregion
	#region DocumentPrinterFormattingController
	public class DocumentPrinterFormattingController : DocumentFormattingController {
		public DocumentPrinterFormattingController(DocumentLayout documentLayout, PieceTable pieceTable)
			: base(documentLayout, pieceTable, null, null) {
		}
		public Point ColumnLocation {
			get {
				PrintColumnController columnController = (PrintColumnController)ColumnController;
				return columnController.ColumnLocation;
			}
			set {
				if (value == ColumnLocation)
					return;
				PrintColumnController columnController = (PrintColumnController)ColumnController;
				columnController.ColumnLocation = value;
				Reset(false);
			}
		}
		public Size ColumnSize {
			get {
				PrintColumnController columnController = (PrintColumnController)ColumnController;
				return columnController.Size;
			}
			set {
				if (value == ColumnSize)
					return;
				PrintColumnController columnController = (PrintColumnController)ColumnController;
				columnController.Size = value;
				Reset(false);
			}
		}
		protected internal override PageController CreatePageController() {
			return new PrintPageController(DocumentLayout);
		}
		protected internal override ColumnController CreateColumnController() {
			return new PrintColumnController(PageAreaController);
		}
		protected internal override PageAreaController CreatePageAreaController() {
			return new PrintPageAreController(PageController);
		}
		protected internal override RowsController CreateRowController() {
			return new RowsController(PieceTable, ColumnController, DocumentModel.LayoutOptions.PrintLayoutView.MatchHorizontalTableIndentsToTextEdge);
		}
	}
	#endregion
	#region TextBoxPrinterFormattingController
	public class TextBoxDocumentPrinterFormattingController : DocumentPrinterFormattingController {
		public TextBoxDocumentPrinterFormattingController(DocumentLayout documentLayout, PieceTable pieceTable)
			: base(documentLayout, pieceTable) {
		}
		protected internal override PageController CreatePageController() {
			return new TextBoxPrintPageController(DocumentLayout, PieceTable);
		}
	}
	#endregion
	#region CommentPrinterFormattingController
	public class CommentPrinterFormattingController : DocumentPrinterFormattingController {
		public CommentPrinterFormattingController(DocumentLayout documentLayout, PieceTable pieceTable)
			: base(documentLayout, pieceTable) {
		}
		protected internal override RowsController CreateRowController() {
			return new CommentRowsController(PieceTable, ColumnController, DocumentModel.LayoutOptions.PrintLayoutView.MatchHorizontalTableIndentsToTextEdge);
		}
		protected internal override PageController CreatePageController() {
			return new CommentPrintPageController(DocumentLayout, PieceTable);
		}
	}
	#endregion
	#region PrintPageController
	public class PrintPageController : PageController {
		public PrintPageController(DocumentLayout documentLayout)
			: base(documentLayout, null, null) {
		}
		protected internal override PageBoundsCalculator CreatePageBoundsCalculator() {
			return new PageBoundsCalculator(DocumentLayout.DocumentModel.ToDocumentLayoutUnitConverter);
		}
		protected internal override BoxHitTestCalculator CreateHitTestCalculator(RichEditHitTestRequest request, RichEditHitTestResult result) {
			return new PrintLayoutViewBoxHitTestCalculator(request, result);
		}
		public override CompleteFormattingResult CompleteCurrentPageFormatting() {
			if (Pages.Count > 0)
				return base.CompleteCurrentPageFormatting();
			return CompleteFormattingResult.Success;
		}
		public override Page GetNextPage(bool keepFloatingObjects) {
			if (Pages.Count > 0)
				return Pages.Last;
			return base.GetNextPage(keepFloatingObjects);
		}
	}
	#endregion
	#region TextBoxPrintPageController
	public class TextBoxPrintPageController : PrintPageController {
		readonly PieceTable pieceTable;
		public TextBoxPrintPageController(DocumentLayout documentLayout, PieceTable pieceTable)
			: base(documentLayout) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
		}
		public override PieceTable PieceTable { get { return pieceTable; } }
		internal override bool AppendFloatingObjectsToPage(Page page) {
			return true;
		}
	}
	#endregion
	#region CommentPrintPageController
	public class CommentPrintPageController : PrintPageController {
		readonly PieceTable pieceTable;
		public CommentPrintPageController(DocumentLayout documentLayout, PieceTable pieceTable)
			: base(documentLayout) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
		}
		public override PieceTable PieceTable { get { return pieceTable; } }
	}
	#endregion
	public class PrintPageAreController : PageAreaController {
		public PrintPageAreController(PageController pageController)
			: base(pageController) {
		}
		public override CompleteFormattingResult CompleteCurrentAreaFormatting() {
			if (PageController.Pages.Count > 0 && Areas.Count > 0)
				return CompleteFormattingResult.Success;
			return base.CompleteCurrentAreaFormatting();
		}
		public override PageArea GetNextPageArea(bool keepFloatingObjects) {
			if (PageController.Pages.Count > 0 && Areas.Count > 0)
				return Areas.Last;
			return base.GetNextPageArea(keepFloatingObjects);
		}
	}
	#region PrintColumnController
	public class PrintColumnController : ColumnController {
		#region Fields
		Point columnLocation;
		Size size;
		#endregion
		public PrintColumnController(PageAreaController pageAreaController)
			: base(pageAreaController) {
		}
		#region Properties
		public Point ColumnLocation { get { return columnLocation; } set { columnLocation = value; } }
		public Size Size { get { return size; } set { size = value; } }
		#endregion
		protected internal override Column GetNextColumnCore(Column column) {
			Column newColumn = new Column();
			if (column == null)
				newColumn.Bounds = new Rectangle(columnLocation.X, columnLocation.Y, size.Width, size.Height);
			else
				newColumn.Bounds = new Rectangle(columnLocation.X, column.Rows.Last.Bounds.Bottom, size.Width, size.Height);
			return newColumn;
		}
		protected internal override Rectangle CalculateColumnBoundsCore(int columnIndex) {
			return ColumnsBounds[columnIndex];
		}
		public override Rectangle GetCurrentPageBounds(Page currentPage, Column currentColumn) {
			Rectangle pageBounds = base.GetCurrentPageBounds(currentPage, currentColumn);
			return Rectangle.Union(pageBounds, currentColumn.Bounds);
		}
		public override Rectangle GetCurrentPageClientBounds(Page currentPage, Column currentColumn) {
			Rectangle pageClientBounds = base.GetCurrentPageClientBounds(currentPage, currentColumn);
			return Rectangle.Union(pageClientBounds, currentColumn.Bounds);
		}
	}
	#endregion
	#region DocumentPrinterController (abstract class)
	public abstract class DocumentPrinterController {
		public abstract Graphics BeginFormat(DocumentModel documentModel);
		public abstract void EndFormat();
	}
	#endregion
	#region PlatformDocumentPrinterController
	public class PlatformDocumentPrinterController : DocumentPrinterController {
#if SL || DXPORTABLE
		public override Graphics BeginFormat(DocumentModel documentModel) {
			return null;
		}
		public override void EndFormat() {
		}
#else
		Graphics graphics;
		GraphicsToLayoutUnitsModifier modifier;
		public override Graphics BeginFormat(DocumentModel documentModel) {
			this.graphics = GraphicsHelper.CreateGraphicsWithoutAspCheck();
			this.modifier = new GraphicsToLayoutUnitsModifier(graphics, documentModel.LayoutUnitConverter);
			return graphics;
		}
		public override void EndFormat() {
			modifier.Dispose();
			graphics.Dispose();
		}
#endif
	}
	#endregion
	#region DocumentPrinter (abstract class)
	public abstract class DocumentPrinter : IDisposable {
		#region Fields
		DocumentLayout documentLayout;
		readonly DocumentModel documentModel;
		DocumentFormattingController controller;
		ParagraphIndex frameParagraphIndex;
		#endregion
		protected DocumentPrinter(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
			this.frameParagraphIndex = new ParagraphIndex(-1);
		}
		#region Properties
		public PageCollection Pages { get { return controller == null ? null : controller.PageController.Pages; } }
		public DocumentModel DocumentModel { get { return documentModel; } }
		public virtual PieceTable PieceTable { get { return documentModel.MainPieceTable; } }
		public DocumentLayout DocumentLayout { get { return documentLayout; } }
		protected virtual Page PageNumberSource { get { return null; } }
		protected internal DocumentFormattingController Controller { get { return controller; } }
		public ParagraphIndex FrameParagraphIndex { get { return frameParagraphIndex; } set { frameParagraphIndex = value; } }
		#endregion
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				UnsubscribeFromFormattingControllerEvents();
			}
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		public void Format() {
			DocumentPrinterController printerController = CreateDocumentPrinterController();
			Graphics gr = printerController.BeginFormat(DocumentModel);
			try {
				Format(gr);
			}
			finally {
				printerController.EndFormat();
			}
		}
		protected void Format(Graphics gr) {
			Format(gr, int.MaxValue);
		}
		protected int Format(Graphics gr, int maxHeight) {
			BoxMeasurer measurer = CreateMeasurer(gr);
			this.documentLayout = CreateDocumentLayout(measurer);
			this.controller = CreateDocumentFormattingController(documentLayout);
			SubscribeToFormattingControllerEvents();
			int actualHeight = PerformPrimaryFormat(maxHeight);
			PerformFinalFormat();
			return Math.Min(actualHeight, maxHeight);
		}
		internal virtual void SubscribeToFormattingControllerEvents() { }
		internal virtual void UnsubscribeFromFormattingControllerEvents() { }
		protected internal virtual DocumentLayout CreateDocumentLayout(BoxMeasurer measurer) {
			return new DocumentLayout(DocumentModel, new ExplicitBoxMeasurerProvider(measurer));
		}
		protected internal abstract DocumentFormattingController CreateDocumentFormattingController(DocumentLayout documentLayout);
		protected internal abstract DocumentPrinterController CreateDocumentPrinterController();
		protected internal abstract BoxMeasurer CreateMeasurer(Graphics gr);
		protected internal virtual int PerformPrimaryFormat(int maxHeight) {
			Page page = new Page(PageNumberSource);
			page.PageOrdinal = 1;
			if (this.FrameParagraphIndex != new ParagraphIndex(-1))
				controller.RowsController.FrameParagraphIndex = this.FrameParagraphIndex;
			SimplePieceTablePrimaryFormatter formatter = new SimplePieceTablePrimaryFormatter(PieceTable, documentLayout.Measurer, controller.RowsController, PieceTable.VisibleTextFilter, page);
			int actualBottom = formatter.Format(maxHeight, controller);
			if (controller.RowsController.FrameParagraphIndex != new ParagraphIndex(-1))
				FrameParagraphIndex = controller.RowsController.FrameParagraphIndex;
			return actualBottom;
		}
		protected virtual void PerformFinalFormat() {
			ParagraphFinalFormatter finalFormatter = new ParagraphFinalFormatter(documentLayout, CommentPadding.GetDefaultCommentPadding(documentLayout.DocumentModel));
			finalFormatter.PieceTable = PieceTable;
			finalFormatter.BookmarkCalculator.ExportToPdf = true;
			PerformFinalFormatCore(finalFormatter);
		}
		protected internal virtual void PerformFinalFormatCore(ParagraphFinalFormatter finalFormatter) {
			finalFormatter.FormatPages(Pages);
		}
		protected virtual void Export(IDocumentLayoutExporter exporter) {
			Pages.ExportTo(exporter);
		}
	}
	#endregion
	public class LayoutDocumentPrinter : BrickDocumentPrinter {
		public LayoutDocumentPrinter(DocumentModel documentModel, bool useGdiPlus)
			: base(documentModel, useGdiPlus) {
		}
		#region PageFormattingComplete
		PageFormattingCompleteEventHandler onPageFormattingComplete;
		public event PageFormattingCompleteEventHandler PageFormattingComplete { add { onPageFormattingComplete += value; } remove { onPageFormattingComplete -= value; } }
		protected internal virtual void RaisePageFormattingComplete(PageFormattingCompleteEventArgs args) {
			if (onPageFormattingComplete != null)
				onPageFormattingComplete(this, args);
		}
		#endregion
		internal override void SubscribeToFormattingControllerEvents() {
			this.Controller.PageFormattingComplete += OnPageFormattingComplete;
		}
		internal override void UnsubscribeFromFormattingControllerEvents() {
			this.Controller.PageFormattingComplete -= OnPageFormattingComplete;
		}
		void OnPageFormattingComplete(object sender, PageFormattingCompleteEventArgs e) {
			RaisePageFormattingComplete(e);
		}
	}
	#region SimpleDocumentPrinter (abstract class)
	public abstract class SimpleDocumentPrinter : DocumentPrinter {
		Point columnLocation;
		Size columnSize;
		protected SimpleDocumentPrinter(DocumentModel documentModel)
			: base(documentModel) {
		}
		public ColumnCollection Columns { get { return Controller == null ? null : Controller.ColumnController.Columns; } }
		public Size ColumnSize { get { return columnSize; } set { columnSize = value; } }
		public Point ColumnLocation { get { return columnLocation; } set { columnLocation = value; } }
		protected internal override DocumentFormattingController CreateDocumentFormattingController(DocumentLayout documentLayout) {
			DocumentPrinterFormattingController result = CreateDocumentPrinterFormattingController(documentLayout, PieceTable);
			result.ColumnLocation = ColumnLocation;
			result.ColumnSize = ColumnSize;
			return result;
		}
		protected internal virtual DocumentPrinterFormattingController CreateDocumentPrinterFormattingController(DocumentLayout documentLayout, PieceTable pieceTable) {
			return new DocumentPrinterFormattingController(documentLayout, pieceTable);
		}
		public int GetEffectiveHeight() {
			int columnCount = Columns.Count;
			Column column = Columns[columnCount - 1];
			int rowsCount = column.Rows.Count;
			Row lastRow = column.Rows[rowsCount - 1];
			if (columnCount == 1 && rowsCount == 1 && lastRow.Boxes.Count == 1 && lastRow.Boxes[0] is ParagraphMarkBox)
				return 0;
			int floatingObjectsBottom = CalculateFloatingObjectsBottom(Pages[0]);
			return Math.Max(floatingObjectsBottom, GetRowBottom(lastRow));
		}
		protected virtual int GetRowBottom(Row row) {
			return row.Bounds.Bottom;
		}
		int CalculateFloatingObjectsBottom(Page page) {
			int result = 0;
			result = Math.Max(CalculateFloatingObjectsBottom(page.BackgroundFloatingObjects), result);
			result = Math.Max(CalculateFloatingObjectsBottom(page.FloatingObjects), result);
			result = Math.Max(CalculateFloatingObjectsBottom(page.ForegroundFloatingObjects), result);
			return result;
		}
		int CalculateFloatingObjectsBottom(List<FloatingObjectBox> floatingObjects) {
			int result = 0;
			int count = floatingObjects.Count;
			for (int i = 0; i < count; i++)
				result = Math.Max(floatingObjects[i].ExtendedBounds.Bottom, result);
			return result;
		}
		protected override void Export(IDocumentLayoutExporter exporter) {
			ColumnCollection columns = Columns;
			if (columns != null)
				columns.ExportTo(exporter);
		}
		protected internal override void PerformFinalFormatCore(ParagraphFinalFormatter finalFormatter) {
			ColumnCollection columns = Columns;
			if (columns != null)
				columns.ForEach(finalFormatter.FormatColumn);
		}
	}
	#endregion
	#region ExtensionCommentInfo
	public struct ExtensionCommentInfo {
		Comment comment;
		int pageOrdinal;
		public ExtensionCommentInfo(Comment comment, int pageOrdinal) {
			this.comment = comment;
			this.pageOrdinal = pageOrdinal;
		}
		public Comment Comment { get { return comment; } set { comment = value; } }
		public int PageOrdinal { get { return pageOrdinal; } set { pageOrdinal = value; } }
	}
	#endregion
	#region RichEditPrinterBase (abstract class)
	public abstract class RichEditPrinterBase {
		readonly DocumentModel documentModel;
		readonly InnerRichEditDocumentServer server;
		Dictionary<int, ExtensionCommentInfo> extensionCommentId = new Dictionary<int, ExtensionCommentInfo>();
		protected RichEditPrinterBase(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
		}
		protected RichEditPrinterBase(InnerRichEditDocumentServer server)
			: this(server.DocumentModel) {
			Guard.ArgumentNotNull(server, "server");
			this.server = server;
		}
		protected internal DocumentModel DocumentModel { get { return documentModel; } }
		protected internal InnerRichEditDocumentServer Server { get { return server; } }
		protected Dictionary<int, ExtensionCommentInfo> ExtensionCommentId { get { return extensionCommentId; } set { extensionCommentId = value; } }
		protected internal virtual DocumentLayout CalculatePrintDocumentLayout() {
			BeginDocumentRendering();
			try {
				DocumentModel targetDocumentModel = DocumentModel.CreateDocumentModelForExport(InitializeEmptyDocumentModel);
				targetDocumentModel.EnsureImageLoadComplete();
				targetDocumentModel.LayoutUnit = Office.DocumentLayoutUnit.Document;
				using (DocumentPrinter printer = CreateDocumentPrinter(targetDocumentModel)) {
					printer.Format();
					return printer.DocumentLayout;
				}
			}
			finally {
				EndDocumentRendering();
			}
		}
		protected internal virtual DocumentLayout CalculatePrintCommentDocumentLayout(DocumentLayout documentLayout, int position) {
			BeginDocumentRendering();
			try {
				ExtensionCommentId = FillExtensionCommentId(documentLayout);
				DocumentModel targetDocumentModel = CreateCommentDocumentModelForExport(documentLayout, position);
				targetDocumentModel.EnsureImageLoadComplete();
				targetDocumentModel.LayoutUnit = Office.DocumentLayoutUnit.Document;
				using (DocumentPrinter printer = CreateDocumentPrinter(targetDocumentModel)) {
					printer.Format();
					return printer.DocumentLayout;
				}
			}
			finally {
				EndDocumentRendering();
			}
		}
		Dictionary<int, ExtensionCommentInfo> FillExtensionCommentId(DocumentLayout documentLayout) {
			Dictionary<int, ExtensionCommentInfo> result = new Dictionary<int, ExtensionCommentInfo>();
			int commentIndex = 0;
			int pagesCount = documentLayout.Pages.Count;
			if (pagesCount > 0) {
				for (int i = 0; i < pagesCount; i++) {
					Page page = documentLayout.Pages[i];
					int commentsCount = page.Comments.Count;
					if (commentsCount > 0) {
						for (int j = 0; j < commentsCount; j++) {
							CommentViewInfo commentViewInfo = page.Comments[j];
							if (commentViewInfo.IsWholeContentVisible) {
								commentIndex += 1;
								result.Add(commentIndex, new ExtensionCommentInfo(commentViewInfo.Comment, page.PageOrdinal));
							}
						}
					}
				}
			}
			return result;
		}
		DocumentModel CreateCommentDocumentModelForExport(DocumentLayout documentLayout, int position) {
			CommentIdProvider provider = new CommentIdProvider();
			DocumentModel targetModel = documentLayout.DocumentModel.CreateNew();
			DocumentPrinterCommentCreator creator = new DocumentPrinterCommentCreator(provider, targetModel);
			targetModel.BeginSetContent();
			creator.CreateCommentHeadingParagraphStyle();
			creator.ChangeCommentHeadingParagraphStyle(position);
			PieceTable targetPieceTable = targetModel.MainPieceTable;
			DocumentLogPosition logPosition = targetPieceTable.DocumentStartLogPosition;
			if (ExtensionCommentId != null) {
				int commentIndex = 0;
				int count = ExtensionCommentId.Count;
				for (int j = 0; j < count; j++) {
					commentIndex = j + 1;
					ExtensionCommentInfo extensionCommentInfo = GetCommentId(ExtensionCommentId, commentIndex);
					Comment sourceComment = extensionCommentInfo.Comment;
					int pageOrdinal = extensionCommentInfo.PageOrdinal;
					logPosition = creator.CopyComment(sourceComment, j, logPosition, pageOrdinal, 0);
				}
			}
			targetModel.EndSetContent(DocumentModelChangeType.LoadNewDocument, false, new FieldUpdateOnLoadOptions(false, false));
			return targetModel;
		}
		ExtensionCommentInfo GetCommentId(Dictionary<int, ExtensionCommentInfo> extensionCommentId, int commentIndex) {
			ExtensionCommentInfo result;
			if (extensionCommentId.TryGetValue(commentIndex, out result))
				return result;
			return new ExtensionCommentInfo(null, 0);
		}
		protected internal virtual void BeginDocumentRendering() {
		}
		protected internal virtual void EndDocumentRendering() {
		}
		protected internal virtual void InitializeEmptyDocumentModel(DocumentModel documentModel) {
		}
		protected internal abstract DocumentPrinter CreateDocumentPrinter(DocumentModel documentModel);
	}
	#endregion
}
