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

using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Model;
using System.Windows.Forms;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Snap.Core.Native;
using DevExpress.Snap.Core.Fields;
namespace DevExpress.Snap.Core.Commands {
	public class SnapDragCopyExternalContentCommand : DragCopyExternalContentCommand {
		public SnapDragCopyExternalContentCommand(IRichEditControl control, DocumentModelPosition targetPosition, IDataObject dataObject)
			: base(control, targetPosition, dataObject) {
		}
		protected internal override void SetSelection(Selection selection, DocumentLogPosition start) {
			SnapPieceTable pieceTable = (SnapPieceTable)selection.PieceTable;
			SnapBookmark bookmark = new SnapBookmarkController(pieceTable).FindInnermostTemplateBookmarkByPosition(selection.NormalizedStart);
			if (bookmark == null)
				return;
			DocumentLogPosition position = bookmark.NormalizedStart;
			pieceTable.DocumentModel.Selection.Start = position;
			pieceTable.DocumentModel.Selection.End = position;
		}
	}
}
