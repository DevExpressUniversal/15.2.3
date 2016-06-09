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
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Services.Implementation;
using DevExpress.Office.NumberConverters;
using DevExpress.Utils;
namespace DevExpress.XtraRichEdit.Fields {
	public abstract class FieldResultFormatting {
		readonly string numericFormatting;
		readonly string[] generalFormatting;
		protected FieldResultFormatting(string numericFormatting, string[] generalFormatting) {
			this.numericFormatting = numericFormatting;
			this.generalFormatting = generalFormatting;
		}
		public abstract bool RecalculateOnSecondaryFormatting { get; }
		protected string NumericFormatting { get { return numericFormatting; } }
		protected virtual string[] GeneralFormatting { get { return generalFormatting; } }
		public string GetValue(ParagraphBoxFormatter formatter, DocumentModel documentModel) {
			int intValue = GetValueCore(formatter, documentModel);
			try {
				NumericFieldFormatter numericFieldFormatter = new NumericFieldFormatter();
				string value = numericFieldFormatter.Format((double)intValue, NumericFormatting, NumericFormatting != null);
				if (GeneralFormatting != null) {
					int count = GeneralFormatting.Length;
					GeneralFieldFormatter generalFieldFormatter = new GeneralFieldFormatter();
					bool hasExplicitFormatting = false;
					for (int i = 0; i < count; i++) {
						string generalFormattingSwitch = GeneralFormatting[i];
						string generalFormattingSwitchUpperCase = generalFormattingSwitch.ToUpper(CultureInfo.InvariantCulture);
						if (generalFormattingSwitchUpperCase == CalculatedFieldBase.MergeFormatKeyword || generalFormattingSwitchUpperCase == CalculatedFieldBase.MergeFormatInetKeyword || generalFormattingSwitchUpperCase == CalculatedFieldBase.CharFormatKeyword)
							continue;
						value = generalFieldFormatter.Format(value, value, generalFormattingSwitch);
						hasExplicitFormatting = true;
					}
					if (!hasExplicitFormatting && numericFormatting == null)
						value = ApplyImplicitFormatting(formatter, value, intValue);
				}
				return value;
			}
			catch (Exception e) {
				if (documentModel.FieldOptions.ThrowExceptionOnInvalidFormatSwitch)
					throw e;
				return intValue.ToString();
			}
		}
		protected virtual string ApplyImplicitFormatting(ParagraphBoxFormatter formatter, string value, int intValue) {
			return value;
		}
		protected abstract int GetValueCore(ParagraphBoxFormatter formatter, DocumentModel documentModel);
	}
	public class PageFieldResultFormatting : FieldResultFormatting {
		static readonly string[] emptyGenericFormatting = new string[] {};
		public PageFieldResultFormatting(string numericFormatting, string[] generalFormatting)
			: base(numericFormatting, generalFormatting) {
		}
		public override bool RecalculateOnSecondaryFormatting { get { return false; } }
		protected override string[] GeneralFormatting {
			get {
				string[] result = base.GeneralFormatting;
				return result == null ? emptyGenericFormatting : result;
			}
		}
		protected override int GetValueCore(ParagraphBoxFormatter formatter, DocumentModel documentModel) {			
			int result = GetPageOrdinal(formatter);
			return result;
		}
		private static SectionPageNumbering GetPageNumbering(ParagraphBoxFormatter formatter) {
			return formatter.RowsController.ColumnController.PageAreaController.PageController.CurrentSection.PageNumbering;			
		}
		protected override string ApplyImplicitFormatting(ParagraphBoxFormatter formatter, string value, int intValue) {
			SectionPageNumbering pageNumbering = GetPageNumbering(formatter);
			OrdinalBasedNumberConverter converter = OrdinalBasedNumberConverter.CreateConverter(pageNumbering.NumberingFormat, LanguageId.English);
			return converter.ConvertNumber(intValue);
		}
		protected internal virtual int GetPageOrdinal(ParagraphBoxFormatter formatter) {
			if (formatter.PageNumberSource != null)
				return formatter.PageNumberSource.PageOrdinal;
			else
				return formatter.RowsController.ColumnController.PageAreaController.PageController.Pages.Count;
		}
	}
	public partial class PageField : CalculatedFieldBase {		
		#region FieldInitialization
		#region static
		public static readonly string FieldType = "PAGE";
		static readonly Dictionary<string, bool> switchesWithArgument = CreateSwitchesWithArgument();
		public static CalculatedFieldBase Create() {
			return new PageField();
		}
		#endregion
		protected override Dictionary<string, bool> SwitchesWithArguments { get { return switchesWithArgument; } }
		protected override string FieldTypeName { get { return FieldType; } }
		public override void Initialize(PieceTable pieceTable, InstructionCollection instructions) {
			base.Initialize(pieceTable, instructions);
		}
		#endregion        
		public override CalculatedFieldValue GetCalculatedValueCore(PieceTable sourcePieceTable, MailMergeDataMode mailMergeDataMode, Field documentField) {
			DocumentLogPosition logPosition = DocumentModelPosition.FromRunStart(sourcePieceTable, documentField.FirstRunIndex).LogPosition;
			int pageIndex = GetPageIndex(sourcePieceTable, logPosition);
			if (pageIndex >= 0)
				return new CalculatedFieldValue(pageIndex.ToString());
			DocumentModel targetModel = sourcePieceTable.DocumentModel.GetFieldResultModel();
			targetModel.BeginUpdate();
			try {
				targetModel.MainPieceTable.InsertLayoutDependentTextRun(new ParagraphIndex(0), new DocumentLogPosition(0), new PageFieldResultFormatting(NumericFormatting, GeneralFormatting));
			}
			finally {
				targetModel.EndUpdate();
			}
			return new CalculatedFieldValue(targetModel);
		}
		int GetPageIndex(PieceTable pieceTable, DocumentLogPosition logPosition) {
			IDocumentLayoutService documentLayoutService = pieceTable.DocumentModel.GetService<IDocumentLayoutService>();
			if (pieceTable.IsMain && documentLayoutService != null) {
				DocumentLayout documentLayout = documentLayoutService.CalculateDocumentLayout();
				return GetPageIndex(pieceTable, documentLayout, logPosition);
			}
			return -1;
		}
		int GetPageIndex(PieceTable pieceTable, DocumentLayout documentLayout, DocumentLogPosition logPosition) {
			DocumentLayoutPosition layoutPosition = documentLayout.CreateLayoutPosition(pieceTable, logPosition, -1);
			layoutPosition.Update(documentLayout.Pages, DocumentLayoutDetailsLevel.Page);
			if (layoutPosition.IsValid(DocumentLayoutDetailsLevel.Page))
				return layoutPosition.Page.PageOrdinal;
			return -1;
		}
		protected internal override UpdateFieldOperationType GetAllowedUpdateFieldTypes(FieldUpdateOnLoadOptions options) {
			return UpdateFieldOperationType.Copy | UpdateFieldOperationType.Load | UpdateFieldOperationType.Normal | UpdateFieldOperationType.PasteFromIE | UpdateFieldOperationType.CreateModelForExport;
		}
		public override void ApplyFieldFormatting(CalculatedFieldValue value, MailMergeCustomSeparators customSeparators) {
		}
	}
}
