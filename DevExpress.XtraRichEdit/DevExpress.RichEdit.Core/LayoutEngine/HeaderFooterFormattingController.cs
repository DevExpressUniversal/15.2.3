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
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Compatibility.System.Drawing;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Layout.Engine {
	#region HeaderFooterDocumentFormattingController
	public class HeaderFooterDocumentFormattingController : DocumentFormattingController {
		public HeaderFooterDocumentFormattingController(DocumentLayout documentLayout, FloatingObjectsLayout floatingObjectsLayout, ParagraphFramesLayout paragraphFramesLayout, PieceTable pieceTable, PageArea area, SectionIndex sectionIndex, Page pageNumberSource)
			: base(documentLayout, pieceTable, floatingObjectsLayout, paragraphFramesLayout) {
			Guard.ArgumentNotNull(area, "area");
			PageAreaController.Areas.Clear();
			PageAreaController.Areas.Add(area);
			HeaderFooterPageController controller = (HeaderFooterPageController)PageController;
			controller.PageNumberSource = pageNumberSource;
			controller.SetFixedSectionIndex(sectionIndex);
			Reset(false);
		}
		protected internal override PageController CreatePageController() {
			return new HeaderFooterPageController(DocumentLayout, FloatingObjectsLayout, ParagraphFramesLayout, PieceTable);
		}
		protected internal override PageAreaController CreatePageAreaController() {
			return new HeaderFooterPageAreaController(PageController);
		}
		protected internal override ColumnController CreateColumnController() {
			return new HeaderFooterColumnController(PageAreaController);
		}
		protected internal override RowsController CreateRowController() {
			return new HeaderFooterRowsController(PieceTable, ColumnController, DocumentModel.LayoutOptions.PrintLayoutView.MatchHorizontalTableIndentsToTextEdge);
		}
		protected internal override void NotifyDocumentFormattingComplete() {
		}
	}
	#endregion
	#region HeaderFooterPageController
	public class HeaderFooterPageController : PageController {
		readonly PageCollection pages;
		readonly PieceTable pieceTable;
		Page pageNumberSource;
		SectionIndex sectionIndex = new SectionIndex(0);
		public HeaderFooterPageController(DocumentLayout documentLayout, FloatingObjectsLayout floatingObjectsLayout, ParagraphFramesLayout paragraphFramesLayout, PieceTable pieceTable)
			: base(documentLayout, floatingObjectsLayout, paragraphFramesLayout) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pages = new PageCollection();
			this.pieceTable = pieceTable;			
			SetPageLastRunIndex(RunIndex.MaxValue - 1);
		}
		public override PageCollection Pages { get { return pages; } }
		public override PieceTable PieceTable { get { return pieceTable; } }
		public override Section CurrentSection { get { return DocumentLayout.DocumentModel.Sections[CurrentSectionIndex]; } }
		public override SectionIndex CurrentSectionIndex { get { return sectionIndex; } set { } }
		public virtual Page PageNumberSource { get { return pageNumberSource; } set { pageNumberSource = value; } }
		public void SetFixedSectionIndex(SectionIndex sectionIndex) {
			this.sectionIndex = sectionIndex;
		}
		public override void Reset(bool keepFloatingObjects) {
			base.Reset(keepFloatingObjects);
			Pages.Add(new Page(pageNumberSource)); 
		}
#if DEBUGTEST || DEBUG
		public override CompleteFormattingResult CompleteCurrentPageFormatting() {
			CompleteFormattingResult result = base.CompleteCurrentPageFormatting();
			Debug.Assert(result == CompleteFormattingResult.Success);
			return result;
		}
#endif
		public override Page GetNextPage(bool keepFloatingObjects) {
			return Pages[0];
		}
		protected internal override PageBoundsCalculator CreatePageBoundsCalculator() {
			return new HeaderFooterPageBoundsCalculator(this);
		}
		protected internal override BoxHitTestCalculator CreateHitTestCalculator(RichEditHitTestRequest request, RichEditHitTestResult result) {
			return new PrintLayoutViewBoxHitTestCalculator(request, result);
		}
		protected internal override void ClearFloatingObjectsLayout() {
		}
		protected internal override void ClearParagraphFramesLayout() {
		}
	}
	#endregion
	#region HeaderFooterPageBoundsCalculator
	public class HeaderFooterPageBoundsCalculator : PageBoundsCalculator {
		public HeaderFooterPageBoundsCalculator(HeaderFooterPageController controller)
			: base(controller.DocumentLayout.DocumentModel.ToDocumentLayoutUnitConverter) {
			Guard.ArgumentNotNull(controller, "controller");
		}
	}
	#endregion
	#region HeaderFooterPageAreaController
	public class HeaderFooterPageAreaController : PageAreaController {
		readonly PageAreaCollection areas;
		public HeaderFooterPageAreaController(PageController pageController)
			: base(pageController) {
			this.areas = new PageAreaCollection();
			this.areas.Add(new PageArea(PageController.PieceTable.ContentType, PageController.DocumentModel.Sections.First)); 
		}
		public override PageAreaCollection Areas { get { return areas; } }
		public override Rectangle CurrentAreaBounds { get { return Areas[0].Bounds; } }
		public override CompleteFormattingResult CompleteCurrentAreaFormatting() {
			return CompleteFormattingResult.Success;
		}
		public override PageArea GetNextPageArea(bool keepFloatingObjects) {
			return Areas[0];
		}
		public override void Reset(Section section) {
		}
		public override void BeginSectionFormatting(Section section, int currentColumnsCount) {
		}
		public override void RestartFormattingFromTheStartOfSection(Section section, int currentAreaIndex) {
		}
		public override void RestartFormattingFromTheMiddleOfSection(Section section, int currentAreaIndex) {
		}
		public override void ClearInvalidatedContent(FormatterPosition pos) {
		}
		protected internal override PageAreaControllerState CreateDefaultState(int currentAreaIndex) {
			return null;
		}
		public override void SwitchToState(PageAreaControllerState state) {
		}
	}
	#endregion
	#region HeaderFooterColumnController
	public class HeaderFooterColumnController : ColumnController {
		public HeaderFooterColumnController(PageAreaController pageAreaController)
			: base(pageAreaController) {
		}
		public override RunIndex PageLastRunIndex { get { return RunIndex.MaxValue; } }
		protected internal override Rectangle CalculateColumnBoundsCore(int columnIndex) {
			return ColumnsBounds[0];
		}
		protected internal override ColumnsBoundsCalculator CreateColumnBoundsCalculator() {
			return new HeaderFooterColumnsBoundsCalculator(PageAreaController.PageController.DocumentLayout.DocumentModel.ToDocumentLayoutUnitConverter);
		}
	}
	#endregion
	#region HeaderFooterColumnsBoundsCalculator
	public class HeaderFooterColumnsBoundsCalculator : ColumnsBoundsCalculator {
		public HeaderFooterColumnsBoundsCalculator(DocumentModelUnitToLayoutUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected internal override void PopulateColumnsBounds(List<Rectangle> result, Rectangle bounds, ColumnInfoCollection columnInfoCollection) {
			result.Add(bounds);
		}
		protected internal override void PopulateEqualWidthColumnsBounds(List<Rectangle> result, Rectangle bounds, int columnCount, int spaceBetweenColumns) {
			result.Add(bounds);
		}
	}
	#endregion
	#region HeaderFooterRowsController
	public class HeaderFooterRowsController : RowsController {
		public HeaderFooterRowsController(PieceTable pieceTable, ColumnController columnController, bool matchHorizontalTableIndentsToTextEdge)
			: base(pieceTable, columnController, matchHorizontalTableIndentsToTextEdge) {
		}
		protected internal override void RecreateHorizontalPositionController() {
		}
		protected internal override bool CanRestartDueFloatingObject() {
			return false;
		}
	}
	#endregion
}
