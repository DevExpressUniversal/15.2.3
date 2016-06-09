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
using DevExpress.XtraRichEdit;
using DevExpress.Snap.Core.Native;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.Utils;
namespace DevExpress.Snap.Core.Commands {
	public class SnapDeleteCoreCommand : DeleteCoreCommand {
		public SnapDeleteCoreCommand(IRichEditControl control)
			: base(control) {
		}
		protected internal override bool ValidateSelectionRanges(SelectionRangeCollection sorted) {
			return new SnapSelectionRangesValidator((SnapPieceTable)ActivePieceTable).ValidateSelectionRanges(sorted);
		}
	}
	public class SnapSelectionRangesValidator {
		readonly SnapPieceTable pieceTable;
		public SnapSelectionRangesValidator(SnapPieceTable pieceTable) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
		}
		public SnapPieceTable PieceTable { get { return pieceTable; } }
		public bool ValidateSelectionRanges(SelectionRangeCollection sorted) {
			SnapBookmarkController controller = new SnapBookmarkController(PieceTable);
			SnapBookmark prevStart = null;
			SnapBookmark prevEnd = null;
			for (int i = 0; i < sorted.Count; i++) {
				SelectionRange range = sorted[i];
				SnapBookmark start = controller.FindInnermostTemplateBookmarkByPosition(range.Start);
				SnapBookmark end = controller.FindInnermostTemplateBookmarkByPosition(range.End);
				if (!Object.ReferenceEquals(start, end))
					return false;
				if ((!Object.ReferenceEquals(start, prevStart) || !Object.ReferenceEquals(end, prevEnd)) && i > 0)
					return false;
				if (start != null) {
					int selectionLength = Math.Max(1, Math.Abs(range.Length));
					if (selectionLength >= start.Length)
						return false;
				}
				prevStart = start;
				prevEnd = end;
			}
			return true;
		}
	}
}
