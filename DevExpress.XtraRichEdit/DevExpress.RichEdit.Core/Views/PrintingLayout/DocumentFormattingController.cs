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
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Compatibility.System.Drawing;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Layout.Engine {
	#region PrintLayoutViewDocumentFormattingController
	public class PrintLayoutViewDocumentFormattingController : DocumentFormattingController {
		public PrintLayoutViewDocumentFormattingController(DocumentLayout documentLayout, PieceTable pieceTable)
			: base(documentLayout, pieceTable, null, null) {
		}
		protected internal override PageController CreatePageController() {
			return new PrintLayoutViewPageController(DocumentLayout);
		}
		protected internal override ColumnController CreateColumnController() {
			return new PrintLayoutViewColumnController(PageAreaController);
		}
		protected internal override PageAreaController CreatePageAreaController() {
			return new PrintLayoutViewPageAreaController(PageController);
		}
		protected internal override RowsController CreateRowController() {
			return new RowsController(PieceTable, ColumnController, DocumentModel.LayoutOptions.PrintLayoutView.MatchHorizontalTableIndentsToTextEdge);
		}
	}
	#endregion
	#region PrintLayoutViewPageController
	public class PrintLayoutViewPageController : PageController {
		public PrintLayoutViewPageController(DocumentLayout documentLayout)
			: base(documentLayout, null, null) {
		}
		protected internal override PageBoundsCalculator CreatePageBoundsCalculator() {
			return new PageBoundsCalculator(DocumentLayout.DocumentModel.ToDocumentLayoutUnitConverter);
		}
		protected internal override void FormatHeader(Page page, bool firstPageOfSection) {
			PageHeaderFormatter formatter = new PageHeaderFormatter(DocumentLayout, FloatingObjectsLayout, ParagraphFramesLayout, page, CurrentSectionIndex);
			formatter.Format(firstPageOfSection);
		}
		protected internal override void FormatFooter(Page page, bool firstPageOfSection) {
			PageFooterFormatter formatter = new PageFooterFormatter(DocumentLayout, FloatingObjectsLayout, ParagraphFramesLayout, page, CurrentSectionIndex);
			formatter.Format(firstPageOfSection);
		}
		protected internal override void ApplyExistingHeaderAreaBounds(Page page) {
			PageHeaderFormatter formatter = new PageHeaderFormatter(DocumentLayout, FloatingObjectsLayout, ParagraphFramesLayout, page, CurrentSectionIndex);
			formatter.ApplyExistingAreaBounds();
		}
		protected internal override void ApplyExistingFooterAreaBounds(Page page) {
			PageFooterFormatter formatter = new PageFooterFormatter(DocumentLayout, FloatingObjectsLayout, ParagraphFramesLayout, page, CurrentSectionIndex);
			formatter.ApplyExistingAreaBounds();
		}
		protected internal override BoxHitTestCalculator CreateHitTestCalculator(RichEditHitTestRequest request, RichEditHitTestResult result) {
			return new PrintLayoutViewBoxHitTestCalculator(request, result);
		}
	}
	#endregion
	#region PrintLayoutViewColumnController
	public class PrintLayoutViewColumnController : ColumnController {
		public PrintLayoutViewColumnController(PageAreaController pageAreaController)
			: base(pageAreaController) {
		}
		protected internal override Rectangle CalculateColumnBoundsCore(int columnIndex) {
			return ColumnsBounds[columnIndex];
		}
	}
	#endregion
	#region PrintLayoutViewPageAreaController
	public class PrintLayoutViewPageAreaController : PageAreaController {
		public PrintLayoutViewPageAreaController(PageController pageController)
			: base(pageController) {
		}
		public override void Reset(Section section) {
			SwitchToDefaultState();
			base.Reset(section);
		}
		public override void RestartFormattingFromTheStartOfSection(Section section, int currentAreaIndex) {
			SwitchToDefaultState();
			base.RestartFormattingFromTheStartOfSection(section, currentAreaIndex);
		}
		public override void RestartFormattingFromTheMiddleOfSection(Section section, int currentAreaIndex) {
			SwitchToDefaultState();
			base.RestartFormattingFromTheMiddleOfSection(section, currentAreaIndex);
		}
		protected internal virtual void SwitchToDefaultState() {
			PageAreaControllerState nextState = CreateDefaultState(0);
			SwitchToState(nextState);
		}
		protected internal override PageAreaControllerState CreateDefaultState(int currentAreaIndex) {
			return new PrintLayoutViewDefaultPageAreaControllerState(this, currentAreaIndex);
		}
	}
	#endregion
	#region PrintLayoutViewDefaultPageAreaControllerState
	public class PrintLayoutViewDefaultPageAreaControllerState : DefaultPageAreaControllerState {
		public PrintLayoutViewDefaultPageAreaControllerState(PrintLayoutViewPageAreaController owner, int nextAreaIndex)
			: base(owner, nextAreaIndex) {
		}
		protected internal override void ApplyContinuousSectionStart(Section section) {
			base.ApplyContinuousSectionStart(section);
		}
	}
	#endregion
	#region FinishContinuousSectionStartPageAreaControllerState
	public class FinishContinuousSectionStartPageAreaControllerState : PageAreaControllerState {
		readonly Rectangle areaBounds;
		int getNextPageAreaCount;
		public FinishContinuousSectionStartPageAreaControllerState(PageAreaController owner, Rectangle areaBounds)
			: base(owner) {
			this.areaBounds = areaBounds;
			CreateCurrentAreaBounds();
		}
		public override CompleteFormattingResult CompleteCurrentAreaFormatting() {
			if (Owner.PageController.Pages.Last == null)
				return Owner.PageController.CompleteCurrentPageFormatting();
			return CompleteFormattingResult.Success;
		}
		public override PageArea GetNextPageArea(bool keepFloatingObjects) {
			if (Owner.PageController.Pages.Last == null)
				Owner.PageController.GetNextPage(keepFloatingObjects);
			Debug.Assert(getNextPageAreaCount == 0);
			getNextPageAreaCount++;
			CreateCurrentAreaBounds();
			PageArea newPageArea = GetNextPageAreaCore();
			Areas.Add(newPageArea);
			return newPageArea;
		}
		protected internal override void ApplySectionStart(Section section, int currentColumnsCount) {
			if (getNextPageAreaCount == 0)
				return;
			PageAreaControllerState nextState = Owner.CreateDefaultState(Areas.Count - 1);
			Owner.SwitchToState(nextState);
			nextState.CreateCurrentAreaBounds();
		}
		protected internal override Rectangle CreateCurrentAreaBoundsCore() {
			return areaBounds;
		}
	}
	#endregion
}
