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
using System.Text;
using DevExpress.XtraRichEdit.Model;
using System.Globalization;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Layout.Engine;
namespace DevExpress.XtraRichEdit.Fields {
	public class NumPagesFieldResultFormatting : FieldResultFormatting {
		public NumPagesFieldResultFormatting(string numericFormatting, string[] generalFormatting)
			: base(numericFormatting, generalFormatting) {			
		}
		public override bool RecalculateOnSecondaryFormatting { get { return true; } }
		protected override int GetValueCore(ParagraphBoxFormatter formatter, DocumentModel documentModel) {
			return documentModel.ExtendedDocumentProperties.Pages;
		}
	}
	public partial class NumPagesField : CalculatedFieldBase {
		#region FieldInitialization
		#region static
		public static readonly string FieldType = "NUMPAGES";
		static readonly Dictionary<string, bool> switchesWithArgument = CreateSwitchesWithArgument();
		public static CalculatedFieldBase Create() {
			return new NumPagesField();
		}
		#endregion
		protected override Dictionary<string, bool> SwitchesWithArguments { get { return switchesWithArgument; } }
		protected override string FieldTypeName { get { return FieldType; } }
		public override void Initialize(PieceTable pieceTable, InstructionCollection instructions) {
			base.Initialize(pieceTable, instructions);
		}
		#endregion
		public override CalculatedFieldValue GetCalculatedValueCore(PieceTable sourcePieceTable, MailMergeDataMode mailMergeDataMode, Field documentField) {
			DocumentModel targetModel = sourcePieceTable.DocumentModel.GetFieldResultModel();
			if (ShouldInsertLayoutDependentRun(sourcePieceTable)) {
				targetModel.BeginUpdate();
				try {
					targetModel.MainPieceTable.InsertLayoutDependentTextRun(new ParagraphIndex(0), new DocumentLogPosition(0), new NumPagesFieldResultFormatting(NumericFormatting, GeneralFormatting));
				}
				finally {
					targetModel.EndUpdate();
				}
				return new CalculatedFieldValue(targetModel);
			}
			else
				return new CalculatedFieldValue(sourcePieceTable.DocumentModel.ExtendedDocumentProperties.Pages);
		}
		private bool ShouldInsertLayoutDependentRun(PieceTable sourcePieceTable) {
			PieceTable topLevelPieceTable = GetTopLevelPieceTable(sourcePieceTable);
			return topLevelPieceTable.ContentType is SectionHeaderFooterBase;
		}
		PieceTable GetTopLevelPieceTable(PieceTable sourcePieceTable) {
			TextBoxContentType textBox = sourcePieceTable.ContentType as TextBoxContentType;
			if (textBox != null)
				return textBox.AnchorRun.PieceTable;
			return sourcePieceTable;
		}
		public override void ApplyFieldFormatting(CalculatedFieldValue value, MailMergeCustomSeparators customSeparators) {
			if (value.IsDocumentModelValue())
				return;
			else
				base.ApplyFieldFormatting(value, customSeparators);
		}
		protected internal override UpdateFieldOperationType GetAllowedUpdateFieldTypes(FieldUpdateOnLoadOptions options) {
			return UpdateFieldOperationType.Copy | UpdateFieldOperationType.Load | UpdateFieldOperationType.Normal | UpdateFieldOperationType.PasteFromIE | UpdateFieldOperationType.CreateModelForExport;
		}		
	}
}
