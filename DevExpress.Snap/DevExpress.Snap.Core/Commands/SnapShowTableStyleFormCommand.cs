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
using System.Linq;
using System.Text;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.Snap.Core.Commands {
	#region SnapShowTableStyleFormCommand
	public class SnapShowTableStyleFormCommand : ShowTableStyleFormCommand {
		#region Fields
		TableCellStyle style;
		#endregion
		public SnapShowTableStyleFormCommand(IRichEditControl control)
			: this(control, null) {
		}
		public SnapShowTableStyleFormCommand(IRichEditControl control, TableCellStyle style)
			: base(control) {
			this.style = style;
		}
		public TableCellStyle CellStyle { get { return style; } set { style = value; } }
		public override void ForceExecute(ICommandUIState state) {
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
				if (CellStyle != null)
					ShowTableCellStyleForm(CellStyle);
				else
					FindStyleAndShowForm();
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		internal virtual void ShowTableCellStyleForm(TableCellStyle style) {
			((ISnapControl)Control).ShowTableCellStyleForm(style);
		}
		void FindStyleAndShowForm() {
			DocumentModel model = Control.InnerControl.DocumentModel;
			PieceTable pieceTable = model.ActivePieceTable;
			RunInfo interval = model.Selection.Interval;
			RunIndex startIndex = interval.NormalizedStart.RunIndex;
			RunIndex endIndex = interval.NormalizedEnd.RunIndex;
			TextRunBase firstRun = pieceTable.Runs[startIndex];
			ParagraphIndex firstIndex = firstRun.Paragraph.Index;
			TextRunBase lastRun = pieceTable.Runs[endIndex];
			ParagraphIndex lastIndex = lastRun.Paragraph.Index;
			TableCell sourceCell = pieceTable.Paragraphs[firstIndex].GetCell();
			if (sourceCell == null)
				return;
			for (ParagraphIndex i = firstIndex; i <= lastIndex; i++) {
				TableCell cell = pieceTable.Paragraphs[i].GetCell();
				if (cell != null) {
					if (cell.TableCellStyle != sourceCell.TableCellStyle)
						return;
				}
			}
			ShowTableCellStyleForm(sourceCell.TableCellStyle);
		}
	}
	#endregion
}
