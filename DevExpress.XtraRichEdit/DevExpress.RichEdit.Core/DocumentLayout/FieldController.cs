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
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Utils;
namespace DevExpress.XtraRichEdit.Layout {
	public class FieldController {
		public Field FindFieldByHitTestResult(RichEditHitTestResult hitTestResult) {
			Guard.ArgumentNotNull(hitTestResult, "hitTestResult");
			if (hitTestResult.DetailsLevel < DocumentLayoutDetailsLevel.Box)
				return null;
			return hitTestResult.PieceTable.FindFieldByRunIndex(hitTestResult.Box.StartPos.RunIndex);
		}
		public Field FindFieldBySelection<T>(Selection selection, out T parsedInfo) where T : CalculatedFieldBase {
			parsedInfo = null;
			Field field = FindFieldBySelection(selection);
			if (field == null)
				return null;
			return FindParentFieldByType<T>(selection.PieceTable, field, out parsedInfo);
		}
		public Field FindParentFieldByType<T>(PieceTable pieceTable, Field field, out T parsedInfo) where T : CalculatedFieldBase {
			parsedInfo = null;
			FieldCalculatorService parser = pieceTable.CreateFieldCalculatorService();
			while (field != null) {
				parsedInfo = parser.ParseField(pieceTable, field) as T;
				if (parsedInfo != null)
					return field;
				field = field.Parent;
			}
			return null;
		}
		public Field FindFieldBySelection(Selection selection) {
			PieceTable pieceTable = selection.PieceTable;
			DocumentModelPosition start = PositionConverter.ToDocumentModelPosition(pieceTable, Algorithms.Min(selection.NormalizedStart, pieceTable.DocumentEndLogPosition));
			Field startField = selection.PieceTable.FindFieldByRunIndex(start.RunIndex);
			if (startField == null)
				return null;
			DocumentModelPosition end = PositionConverter.ToDocumentModelPosition(pieceTable, Algorithms.Min(selection.NormalizedEnd, pieceTable.DocumentEndLogPosition));
			if (end.RunOffset == 0 && end.RunIndex == startField.LastRunIndex + 1)
				return startField;
			Field endField = selection.PieceTable.FindFieldByRunIndex(end.RunIndex);
			if (startField == endField)
				return startField;
			else
				return null;
		}
		public Field FindTopmostFieldBySelection(Selection selection) {
			PieceTable pieceTable = selection.PieceTable;
			DocumentModelPosition start = PositionConverter.ToDocumentModelPosition(pieceTable, Algorithms.Min(selection.NormalizedStart, pieceTable.DocumentEndLogPosition));
			Field startField = selection.PieceTable.FindFieldByRunIndex(start.RunIndex);
			if (startField == null)
				return null;
			DocumentModelPosition end = PositionConverter.ToDocumentModelPosition(pieceTable, Algorithms.Min(selection.NormalizedEnd, pieceTable.DocumentEndLogPosition));
			if (end.RunOffset == 0 && end.RunIndex == startField.LastRunIndex + 1)
				return startField;
			Field endField = selection.PieceTable.FindFieldByRunIndex(end.RunIndex);
			if (endField == null)
				return null;
			if (startField.ContainsField(endField))
				return startField;
			if (endField.ContainsField(startField))
				return endField;
			if (startField == endField)
				return startField;
			return null;
		}
	}
	public class SelectionFieldHelper {
		Field field;
		public SelectionFieldHelper(Selection selection) {
			FieldController controller = new FieldController();
			this.field = controller.FindFieldBySelection(selection);
		}
		protected Field Field { get { return field; } }
		public bool ShouldCreateRectangularObjectSelection() {
			if (Field == null)
				return false;
			return Field.Result.End - Field.Result.Start == 1;
		}
		public RunIndex GetFieldResultStart() {
			return Field.Result.Start;
		}
	}
}
