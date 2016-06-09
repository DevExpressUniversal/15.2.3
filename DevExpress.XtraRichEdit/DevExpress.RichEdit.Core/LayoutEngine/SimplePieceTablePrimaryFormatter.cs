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
namespace DevExpress.XtraRichEdit.Layout.Engine {
	public class SimplePieceTablePrimaryFormatter {
		#region Fields
		readonly PieceTable pieceTable;
		readonly RowsController rowsController;
		readonly Page page;
		#endregion
		public SimplePieceTablePrimaryFormatter(PieceTable pieceTable, BoxMeasurer measurer, RowsController rowsController, IVisibleTextFilter visibleTextFilter, Page page) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			Guard.ArgumentNotNull(measurer, "measurer");
			Guard.ArgumentNotNull(rowsController, "rowsController");
			Guard.ArgumentNotNull(visibleTextFilter, "visibleTextFilter");
			Guard.ArgumentNotNull(page, "page");
			this.pieceTable = pieceTable;
			this.rowsController = rowsController;
			this.page = page;
		}
		#region Properties
		public PieceTable PieceTable { get { return pieceTable; } }
		public DocumentModel DocumentModel { get { return pieceTable.DocumentModel; } }
		public Page Page { get { return page; } }
		#endregion
		public virtual int Format(int maxHeight, DocumentFormattingController controller) {
			using (DocumentFormatter documentFormatter = new DocumentFormatter(controller)) {
				if (this.rowsController.FrameParagraphIndex != new ParagraphIndex(-1))
					documentFormatter.ParagraphIndex = rowsController.FrameParagraphIndex;
				documentFormatter.ParagraphFormatter.PageNumberSource = Page.PageNumberSource;
				documentFormatter.ParagraphFormatter.MaxHeight = maxHeight;
				FormattingProcessResult result;
				do {
					result = documentFormatter.FormatNextRow();
					if (documentFormatter.ParagraphFormatter.ForceFormattingComplete)
						break;
				}
				while (result.FormattingProcess != FormattingProcess.Finish);
				if (this.rowsController.FrameParagraphIndex != new ParagraphIndex(-1))
					rowsController.FrameParagraphIndex = documentFormatter.ParagraphIndex;
				PageArea area = controller.PageController.Pages.Last.Areas.Last;
				if (area != null)
					if (area.Columns.Last.Rows.Count == 0)
						area.Columns.RemoveAt(area.Columns.Count - 1);
				if (rowsController.CurrentRow.Boxes.Count == 0)
					return rowsController.CurrentRow.Bounds.Top;
				else
					return rowsController.CurrentRow.Bounds.Bottom;
			}
		}
	}
}
