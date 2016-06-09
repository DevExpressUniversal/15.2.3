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
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.Snap.Core.Fields;
using DevExpress.XtraRichEdit;
namespace DevExpress.Snap.Core.Native {
	public class SnapBackgroundFormatter : BackgroundFormatter {
		public SnapBackgroundFormatter(DocumentFormattingController controller, CommentPadding commentPadding)
			: base(controller, commentPadding) {
		}
		protected override ParagraphFinalFormatter CreateParagraphFinalFormatter(DocumentLayout documentLayout) {
			return new SnapParagraphFinalFormatter(documentLayout, CommentPadding);
		}
	}
	public class SnapParagraphFinalFormatter : ParagraphFinalFormatter {
		Dictionary<Field, CalculatedFieldBase> calculatedFieldsCache = new Dictionary<Field, CalculatedFieldBase>();
		public SnapParagraphFinalFormatter(DocumentLayout documentLayout, CommentPadding commentSkinPadding)
			: base(documentLayout, commentSkinPadding) {
		}
		public new SnapDocumentModel DocumentModel { get { return (SnapDocumentModel)base.DocumentModel; } }
		protected internal override void CalculateFieldsHighlight(Row row, Rectangle tightRowBounds) {
			row.ClearFieldHighlightAreas();
			RunIndex startRunIndex = row.Boxes.First.StartPos.RunIndex;
			RunIndex endRunIndex = row.Boxes.Last.EndPos.RunIndex;
			int firstFileldIndex = LookupFirstFieldIndexByRunIndex(row, startRunIndex);
			if (firstFileldIndex >= 0)
				CalculateFieldsHighlight(row, tightRowBounds, startRunIndex, endRunIndex, firstFileldIndex);
			ClearFieldsCache(endRunIndex);
		}
		void CalculateFieldsHighlight(Row row, Rectangle bounds, RunIndex startRunIndex, RunIndex endRunIndex, int startIndex) {
			FieldCollection fields = PieceTable.Fields;
			int count = fields.Count;
			for (int index = startIndex; index < count; index++) {
				int rootFieldIndex = GetHighlightedFieldIndex(index);
				if (rootFieldIndex < 0)
					continue;
				Field field = fields[rootFieldIndex];
				if (field.LastRunIndex < startRunIndex || field.FirstRunIndex > endRunIndex)
					break;
				CalculateFieldHighlightArea(row, bounds, startRunIndex, endRunIndex, field);
				index = rootFieldIndex;
			}
		}
		void ClearFieldsCache(RunIndex endRunIndex) {
			FieldCollection fields = PieceTable.Fields;
			if (fields.Count > 0 && fields.Last.LastRunIndex <= endRunIndex)
				calculatedFieldsCache.Clear();
		}
		void CalculateFieldHighlightArea(Row row, Rectangle tightRowBounds, RunIndex startRunIndex, RunIndex endRunIndex, Field field) {
			RunIndex start = Algorithms.Max(startRunIndex, field.FirstRunIndex);
			RunIndex end = Algorithms.Min(endRunIndex, field.LastRunIndex);
			Color color = DocumentModel.FieldOptions.HighlightColor;
			HighlightArea area = CreateFieldHighlightAreaCore(row, start, end, tightRowBounds, color);
			if (area != null)
				row.FieldHighlightAreas.Add(area);
		}
		int GetHighlightedFieldIndex(int index) {
			FieldCollection fields = PieceTable.Fields;
			int result = -1;
			for (; ; ) {
				Field field = fields[index];
				if (!ShouldHightlightField(field))
					break;
				result = index;
				if (field.Parent == null)
					break;
				index = FindParentFieldIndex(field, index);
			}
			return result;
		}
		int FindParentFieldIndex(Field child, int index) {
			do {
				index++;
			} while (!Object.ReferenceEquals(PieceTable.Fields[index], child.Parent));
			return index;
		}
		CalculatedFieldBase GetCalculatedField(Field field) {
			CalculatedFieldBase calculatedField;
			if (!calculatedFieldsCache.TryGetValue(field, out calculatedField)) {
				calculatedField = CalculateField(field);
				calculatedFieldsCache.Add(field, calculatedField);
			}
			return calculatedField;
		}
		bool ShouldHightlightField(Field field) {
			return DocumentModel.FieldOptions.HighlightMode == FieldsHighlightMode.Always ? ShouldHightlightField(GetCalculatedField(field)) : DocumentModel.HighlightedField == field;
		}
		bool ShouldHightlightField(CalculatedFieldBase calculatedField) {
			return !(calculatedField is SNListField);
		}
		CalculatedFieldBase CalculateField(Field field) {
			FieldCalculatorService service = new FieldCalculatorService();
			return service.ParseField(PieceTable, field);
		}
	}
}
