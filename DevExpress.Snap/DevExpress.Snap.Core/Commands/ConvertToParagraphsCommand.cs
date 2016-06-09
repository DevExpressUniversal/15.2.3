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
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Snap.Core.Native;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Snap.Core.Native.Templates;
using DevExpress.Snap.Core.Commands;
namespace DevExpress.Snap.Core.Commands {
	[CommandLocalization(Localization.SnapStringId.ConvertToParagraphsCommand_MenuCaption, Localization.SnapStringId.ConvertToParagraphsCommand_Description)]
	public class ConvertToParagraphsCommand : EditListCommandBase {
		public ConvertToParagraphsCommand(IRichEditControl control)
			: base(control) {
		}
		protected override bool IsEnabled() {
			return base.IsEnabled() && HasTabledTemplates();
		}
		protected internal override void UpdateFieldCode(InstructionController controller1) {
			SnapPieceTable pieceTable = EditedFieldInfo.PieceTable;
			List<TemplateFieldInterval> templateIntervals = EditedFieldInfo.ParsedInfo.GetTemplateIntervals(pieceTable);
			int count = templateIntervals.Count;
			bool tableRemoved = false;			
			for (int i = 0; i < count; i++) {
				tableRemoved |= RemoveTable(pieceTable, templateIntervals[i].Interval);
			}
			if(tableRemoved)
				pieceTable.FieldUpdater.UpdateFieldAndNestedFields(EditedFieldInfo.Field);
		}
		bool HasTabledTemplates() {
			SnapPieceTable pieceTable = EditedFieldInfo.PieceTable;
			List<TemplateFieldInterval> templateIntervals = EditedFieldInfo.ParsedInfo.GetTemplateIntervals(pieceTable);
			for(int i = 0; i < templateIntervals.Count; i++) {
				Table table = GetTable(pieceTable, templateIntervals[i].Interval);
				if(table != null)
					return true;
			}
			return false;
		}
		Table GetTable(SnapPieceTable snapPieceTable, DocumentLogInterval sourceInterval) {
			DocumentLogInterval templateInterval = new TemplateController(snapPieceTable).GetActualTemplateInterval(sourceInterval);
			DocumentModelPosition start = PositionConverter.ToDocumentModelPosition(snapPieceTable, templateInterval.Start);
			DocumentModelPosition end = PositionConverter.ToDocumentModelPosition(snapPieceTable, templateInterval.Start + templateInterval.Length - 1);
			TableCell startCell = snapPieceTable.Paragraphs[start.ParagraphIndex].GetCell();
			TableCell endCell = snapPieceTable.Paragraphs[end.ParagraphIndex].GetCell();
			if(start == null || startCell == null || endCell == null)
				return null;
			Table table = startCell.Table;
			if(endCell.Table == table)
				return table;
			return null;
		}
		bool RemoveTable(SnapPieceTable pieceTable, DocumentLogInterval sourceInterval) {			
			DocumentLogInterval templateInterval = new TemplateController((SnapPieceTable)pieceTable).GetActualTemplateInterval(sourceInterval);
			Table table = GetTable(pieceTable, sourceInterval);
			if (table == null)
				return false;
			DocumentModelPosition start = PositionConverter.ToDocumentModelPosition(pieceTable, templateInterval.Start);
			DocumentModelPosition end = PositionConverter.ToDocumentModelPosition(pieceTable, templateInterval.Start + templateInterval.Length - 1);
			int styleIndex = pieceTable.DocumentModel.ParagraphStyles.GetStyleIndexByName(SnapDocumentModel.DefaultListStyleName + EditedFieldInfo.Field.GetLevel());
			if (styleIndex >= 0)
				for (ParagraphIndex i = start.ParagraphIndex; i <= end.ParagraphIndex; i++) {
					if (pieceTable.Paragraphs[i].ParagraphStyleIndex == 0 && pieceTable.Paragraphs[i].GetCell() != null && pieceTable.Paragraphs[i].GetCell().Table == table)
						pieceTable.Paragraphs[i].ParagraphStyleIndex = styleIndex;
				}
			pieceTable.DeleteTableCore(table);
			return true;
		}
		public override string ImageName {
			get {
				return "ConvertToParagraphs";
			}
		}
	}
}
