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
using DevExpress.Office;
using DevExpress.XtraRichEdit.Tables.Native;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraRichEdit.Layout.Engine {
	#region DraftViewDocumentFormattingController
	public class DraftViewDocumentFormattingController : DocumentFormattingController {
		public DraftViewDocumentFormattingController(DocumentLayout documentLayout, PieceTable pieceTable)
			: base(documentLayout, pieceTable, null, null) {
		}
		protected internal override PageController CreatePageController() {
			return new DraftViewPageController(DocumentLayout);
		}
		protected internal override ColumnController CreateColumnController() {
			return new DraftViewColumnController(PageAreaController);
		} 
		 protected internal override RowsController CreateRowController() {
			 return new DraftViewRowsController(PieceTable, ColumnController, DocumentModel.LayoutOptions.DraftView.MatchHorizontalTableIndentsToTextEdge);
		 }
	}
	#endregion
	#region DraftViewRowsController
	public class DraftViewRowsController : SimpleViewRowsControllerBase {
		public DraftViewRowsController(PieceTable pieceTable, ColumnController columnController, bool matchHorizontalTableIndentsToTextEdge)
			: base(pieceTable, columnController, matchHorizontalTableIndentsToTextEdge) {
		}
		protected internal override LineNumberingRestart GetEffectiveLineNumberingRestartType(Section section) {
			return GetEffectiveLineNumberingRestartTypeBase(section);
		}
		public override TableGridCalculator CreateTableGridCalculator(DocumentModel documentModel, TableWidthsCalculator widthsCalculator, int maxTableWidth) {
			return new TableGridCalculator(documentModel, widthsCalculator, maxTableWidth, DocumentModel.LayoutOptions.DraftView.AllowTablesToExtendIntoMargins, false);
		}
	}
	#endregion
	#region DraftViewPageController
	public class DraftViewPageController : NonPrintViewPageControllerBase {
		public DraftViewPageController(DocumentLayout documentLayout)
			: base(documentLayout) {
		}
		protected internal override void FinalizePagePrimaryFormatting(Page page, bool documentEnded) {
			PageArea lastPageArea = page.Areas.Last;
			ColumnCollection columns = lastPageArea.Columns;
			int count = columns.Count;
			int height = 0;
			for (int i = 0; i < count; i++) {
				height = Math.Max(height, GetColumnBottom(columns[i]));
			}
			Rectangle newPageBounds = page.Bounds;
			newPageBounds.Height = height;
			page.Bounds = newPageBounds;
			Rectangle newPageClientBounds = page.ClientBounds;
			newPageClientBounds.Height = height;
			page.ClientBounds = newPageClientBounds;
			Rectangle newAreaBounds = lastPageArea.Bounds;
			newAreaBounds.Height = Math.Max(newAreaBounds.Height, height);
			lastPageArea.Bounds = newAreaBounds;
			base.FinalizePagePrimaryFormatting(page, documentEnded);
		}
		protected internal override PageBoundsCalculator CreatePageBoundsCalculator() {
			return new DraftPageBoundsCalculator(DocumentLayout.DocumentModel.ToDocumentLayoutUnitConverter);
		}
		protected internal override BoxHitTestCalculator CreateHitTestCalculator(RichEditHitTestRequest request, RichEditHitTestResult result) {
			return new DraftViewBoxHitTestCalculator(request, result);
		}
	}
	#endregion
	#region DraftViewColumnController
	public class DraftViewColumnController : ColumnController {
		public DraftViewColumnController(PageAreaController pageAreaController)
			: base(pageAreaController) {
		}
		public override bool ShouldZeroSpacingBeforeWhenMoveRowToNextColumn { get { return false; } }
		protected internal override Rectangle CalculateColumnBoundsCore(int columnIndex) {
			return ColumnsBounds[0];
		}
		protected internal override ColumnsBoundsCalculator CreateColumnBoundsCalculator() {
			return new DraftViewColumnsBoundsCalculator(PageAreaController.PageController.DocumentLayout.DocumentModel.ToDocumentLayoutUnitConverter);
		}
	}
	#endregion
	#region DraftViewColumnsBoundsCalculator
	public class DraftViewColumnsBoundsCalculator : ColumnsBoundsCalculator {
		public DraftViewColumnsBoundsCalculator(DocumentModelUnitToLayoutUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected internal override void PopulateColumnsBounds(List<Rectangle> result, Rectangle bounds, ColumnInfoCollection columnInfoCollection) {
			Rectangle columnBounds = bounds;
			columnBounds.Width = UnitConverter.ToLayoutUnits(columnInfoCollection[0].Width);
			result.Add(columnBounds);
		}
		protected internal override void PopulateEqualWidthColumnsBoundsCore(List<Rectangle> result, Rectangle[] columnRects, int spaceBetweenColumns) {
			result.Add(columnRects[0]);
		}
	}
	#endregion
	#region DraftPageBoundsCalculator
	public class DraftPageBoundsCalculator : PageBoundsCalculator {
		public DraftPageBoundsCalculator(DocumentModelUnitToLayoutUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected internal override Rectangle CalculatePageBounds(Section section) {
			SectionPage page = section.Page;
			SectionMargins margins = section.Margins;
			return CalculatePageClientBoundsCore(page.Width, page.Height, margins.Left, margins.Top, 0, margins.Bottom);
		}
		protected internal override Rectangle CalculatePageClientBoundsCore(int pageWidth, int pageHeight, int marginLeft, int marginTop, int marginRight, int marginBottom) {
			int width = UnitConverter.ToLayoutUnits(pageWidth);
			int height = UnitConverter.ToLayoutUnits(pageHeight);
			int left = UnitConverter.ToLayoutUnits(marginLeft);
			int top = UnitConverter.ToLayoutUnits(marginTop);
			int right = UnitConverter.ToLayoutUnits(marginRight);
			int bottom = UnitConverter.ToLayoutUnits(marginBottom);
			return Rectangle.FromLTRB(0, 0, width - right - left, height - bottom - top);
		}
	}
	#endregion
}
