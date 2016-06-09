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
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraRichEdit.Layout.Engine {
	#region ReadingLayoutViewDocumentFormattingController
	public class ReadingLayoutViewDocumentFormattingController : DocumentFormattingController {
		public ReadingLayoutViewDocumentFormattingController(DocumentLayout documentLayout, PieceTable pieceTable)
			: base(documentLayout, pieceTable, null, null) {
		}
		protected internal override PageController CreatePageController() {
			return new ReadingLayoutViewPageController(DocumentLayout);
		}
		protected internal override ColumnController CreateColumnController() {
			return new ReadingLayoutViewColumnController(PageAreaController);
		}
		protected internal override RowsController CreateRowController() {
			return new RowsController(PieceTable, ColumnController, DocumentModel.LayoutOptions.PrintLayoutView.MatchHorizontalTableIndentsToTextEdge);
		}
	}
	#endregion
	#region ReadingLayoutViewPageController
	public class ReadingLayoutViewPageController : PageController {
		public ReadingLayoutViewPageController(DocumentLayout documentLayout)
			: base(documentLayout, null, null) {
		}
		protected internal override PageBoundsCalculator CreatePageBoundsCalculator() {
			return new PageBoundsCalculator(DocumentLayout.DocumentModel.ToDocumentLayoutUnitConverter);
		}
		protected internal override BoxHitTestCalculator CreateHitTestCalculator(RichEditHitTestRequest request, RichEditHitTestResult result) {
			return new ReadingLayoutViewBoxHitTestCalculator(request, result);
		}
	}
	#endregion
	#region ReadingLayoutViewColumnController
	public class ReadingLayoutViewColumnController : ColumnController {
		public ReadingLayoutViewColumnController(PageAreaController pageAreaController)
			: base(pageAreaController) {
		}
		protected internal override Rectangle CalculateColumnBoundsCore(int columnIndex) {
			return ColumnsBounds[columnIndex];
		}
	}
	#endregion
}
