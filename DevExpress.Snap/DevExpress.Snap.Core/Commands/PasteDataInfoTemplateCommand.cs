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

using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Data.Browsing;
using DevExpress.Snap.Core.Native.Templates;
using DevExpress.Snap.Core.Native;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.Snap.Core.Fields;
using System;
using System.Collections.Generic;
using DevExpress.Snap.Core.Native.Data;
namespace DevExpress.Snap.Core.Commands {
	public class PasteDataInfoTemplateCommand : PasteContentConvertedToDocumentModelCommandBase {
		public PasteDataInfoTemplateCommand(IRichEditControl control)
			: base(control) {
		}
		public override DocumentFormat Format { get { return DocumentFormat.Undefined; } }
		protected internal override PieceTablePasteContentConvertedToDocumentModelCommandBase CreateInnerCommandCore() {
			return new PieceTablePasteDataInfoTemplateCommand(ActivePieceTable);
		}
		protected internal override void ModifyModel() {
			this.InnerCommand.CopyBetweenInternalModels = true;
			base.ModifyModel();
		}
	}
	public class PieceTablePasteDataInfoTemplateCommand : PieceTablePasteTextContentConvertedToDocumentModelCommandBase {
		public PieceTablePasteDataInfoTemplateCommand(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public override DocumentFormat Format { get { return DocumentFormat.Rtf; } }
		new SnapPieceTable PieceTable {
			get { return (SnapPieceTable)base.PieceTable; }
		}
		protected internal override ClipboardStringContent GetContent() {
			throw new NotImplementedException();
		}
		protected internal override string GetAdditionalContentString() {
			throw new NotImplementedException();
		}
		protected internal override void PopulateDocumentModelFromContentStringCore(DocumentModel documentModel, ClipboardStringContent content, string sizeCollection) {
			throw new NotImplementedException();
		}
		protected internal override void ExecuteCore() {
			SnapDocumentModel targetModel = (SnapDocumentModel)DocumentModel;
			DocumentLogPosition insertPosition = DocumentModel.Selection.End;
			SNDataInfo[] dataInfos = PasteSource.GetData(SnapDataFormats.SNDataInfoFullName) as SNDataInfo[];
			if(object.ReferenceEquals(dataInfos, null) || dataInfos.Length == 0)
				return;
			SnapBookmark bookmark = PieceTable.FindBookmarkByPosition(insertPosition);
			if(!object.ReferenceEquals(bookmark, null)) {
				var fieldContext = bookmark.FieldContext as Native.Data.Implementations.SingleListItemFieldContext;
				if(!object.ReferenceEquals(fieldContext, null)) {
					Field field = PieceTable.FindFieldByBookmarkTemplateInterval(bookmark);					
					SnapFieldCalculatorService calculator = new SnapFieldCalculatorService();
					SNListField parsedField = (SNListField)calculator.ParseField(PieceTable, field);
					IFieldDataAccessService dataAccessService = DocumentModel.GetService<IFieldDataAccessService>();
					FieldPathInfo pathInfo = dataAccessService.FieldPathService.FromString(parsedField.DataSourceName);
					FieldDataMemberInfoItem lastItem = pathInfo.DataMemberInfo.LastItem;
					string parent = object.ReferenceEquals(lastItem, null) ? string.Empty : lastItem.FieldName;
					foreach(SNDataInfo dataInfo in dataInfos) {
						DataMemberInfo dataMemberInfo = DataMemberInfo.Create(dataInfo.DataPaths);
						if(!string.Equals(dataMemberInfo.ParentDataMemberInfo.ColumnName, parent)) {
							PasteList(targetModel, insertPosition, dataInfos);
							return;
						}
					}
					PasteRecordData(targetModel, field, insertPosition, dataInfos);
					return;
				}
			}
			PasteList(targetModel, insertPosition, dataInfos);
		}
		protected virtual void PasteList(SnapDocumentModel targetModel, DocumentLogPosition insertPosition, SNDataInfo[] dataInfos) {
			BeforeInsertSnListEventArgs beforeE = new BeforeInsertSnListEventArgs(insertPosition, dataInfos);
			targetModel.RaiseBeforeInsertSnList(beforeE);
			insertPosition = beforeE.Position;
			PrepareMasterCellIfNeeded(insertPosition);
			DocumentModel templateModel = CreateTemplateDocumentModel(beforeE.DataFields, beforeE.InduceRelation);
			if(templateModel == null)
				return;
			templateModel.InheritDataServices(DocumentModel);
			List<SnapListFieldInfo> snListFields = TableCommandsHelper.InsertHeader((SnapPieceTable)templateModel.MainPieceTable, targetModel, insertPosition, dataInfos);
			UpdateTemplateFields(templateModel);
			targetModel.RaisePrepareSnList(new PrepareSnListEventArgs(templateModel));
			PasteDocumentModel(insertPosition, templateModel, false);
			SetSelection(snListFields, targetModel, insertPosition);
			targetModel.RaiseAfterInsertSnList(new AfterInsertSnListEventArgs(insertPosition, templateModel.MainPieceTable.DocumentEndLogPosition - templateModel.MainPieceTable.DocumentStartLogPosition + 1));
		}
		private void PrepareMasterCellIfNeeded(DocumentLogPosition insertPosition) {
			SnapBookmarkController bookmarkController = new SnapBookmarkController(PieceTable);
			SnapBookmark insertPosBookmark = bookmarkController.FindInnermostTemplateBookmarkByPosition(insertPosition);
			if(insertPosBookmark != null) {
				DocumentLogInterval docLogInterval = new DocumentLogInterval(insertPosBookmark.TemplateInterval.Start, insertPosBookmark.TemplateInterval.Length);
				TemplateController templateController = new TemplateController(PieceTable);
				Table tb;
				TemplateContentType templateContentType = templateController.GetTemplateType(docLogInterval, out tb);
				if(templateContentType == TemplateContentType.Table) {
					SnapObjectModelController objectModelController = new SnapObjectModelController(PieceTable);
					TableCell masterCell = objectModelController.FindCellByLogPosition(insertPosition);
					if(masterCell != null) {
						Paragraph startTableParagraph = PieceTable.Paragraphs[masterCell.StartParagraphIndex];
						Paragraph endTableParagraph = PieceTable.Paragraphs[masterCell.EndParagraphIndex];
						bool tableInsideBookmark = insertPosBookmark.Start <= startTableParagraph.LogPosition && insertPosBookmark.End >= endTableParagraph.LogPosition;
						if(!tableInsideBookmark)
							TableCellHelper.PrepareMasterCell(masterCell);
					}
				}
			}
		}
		protected virtual void PasteRecordData(SnapDocumentModel targetModel, Field field, DocumentLogPosition insertPosition, SNDataInfo[] dataInfos) {
			TableCell masterCell = new SnapObjectModelController(PieceTable).FindCellByLogPosition(insertPosition);
			int columnIndex = object.ReferenceEquals(masterCell, null) ? -1 : masterCell.IndexInRow;
			BeforeInsertSnListRecordDataEventArgs beforeE = new BeforeInsertSnListRecordDataEventArgs(field, columnIndex, dataInfos);
			targetModel.RaiseBeforeInsertSnListRecordData(beforeE);
			dataInfos = beforeE.DataFields;
			if(object.ReferenceEquals(dataInfos, null) || dataInfos.Length == 0)
				return;
			DocumentModel templateModel = CreateTemplateDocumentModel(dataInfos, true);
			if(templateModel == null)
				return;
			templateModel.InheritDataServices(DocumentModel);
			UpdateTemplateFields(templateModel);
			PasteDocumentModel(insertPosition, templateModel, false);
			targetModel.RaiseAfterInsertSnListRecordData(new AfterInsertSnListRecordDataEventArgs(insertPosition, templateModel.MainPieceTable.DocumentEndLogPosition - templateModel.MainPieceTable.DocumentStartLogPosition + 1));
		}
		void SetSelection(List<SnapListFieldInfo>  snListFields, SnapDocumentModel targetModel, DocumentLogPosition insertPosition) {
			if(snListFields.Count == 0) {
				SnapBookmark bookmark = PieceTable.FindBookmarkByPosition(DocumentModel.Selection.Start);
				if(bookmark != null)
					targetModel.SelectionInfo.ResetCurrentBookmark(true);
			}
			else {
				SnapPieceTable pieceTable = (SnapPieceTable)DocumentModel.Selection.PieceTable;
				SnapObjectModelController controller = new SnapObjectModelController(pieceTable);
				SnapFieldInfo info = controller.FindFieldInfoByLogPosition(insertPosition);
				if(info == null)
					return;
				SNListField listField = info.ParsedInfo as SNListField;
				if(listField == null)
					return;
				SnapBookmark bookmark = listField.GetFirstContentBookmark(info.PieceTable, info.Field);
				if(bookmark == null)
					return;
				if(bookmark.TemplateInterval.TemplateInfo.TemplateType == SnapTemplateIntervalType.ListHeader) {
					bookmark = controller.GetNextBookmark(bookmark);
					if(bookmark == null)
						return;
				}
				DocumentModelPosition bookmarkStart = bookmark.Interval.NormalizedStart;
				DocumentLogPosition position = bookmarkStart.LogPosition;
				RunIndex runIndex = bookmarkStart.RunIndex;
				while(pieceTable.Runs[runIndex] is SeparatorTextRun) {
					position += pieceTable.Runs[runIndex].Length;
					runIndex++;
				}
				targetModel.Selection.Start = position;
				targetModel.Selection.End = position;
				targetModel.Selection.SetStartCell(position);
			}
		}
		protected internal override PieceTableInsertContentConvertedToDocumentModelCommand CreatePasteDocumentModelCommand(DocumentLogPosition pos, DocumentModel source, bool suppressFieldsUpdate, bool pasteFromIE) {
			PieceTableInsertContentConvertedToDocumentModelCommand command = base.CreatePasteDocumentModelCommand(pos, source, suppressFieldsUpdate, pasteFromIE);
			command.SuppressCopySectionProperties = true;
			return command;
		}
		protected virtual void UpdateTemplateFields(DocumentModel templateModel) {			
			templateModel.UpdateFields(UpdateFieldOperationType.Normal, null);
		}
		protected virtual DocumentModel CreateTemplateDocumentModel(SNDataInfo[] dataInfoArray, bool induceRelation) {
			TemplateBuilder builder = CreateTemplateBuilder();
			builder.InduceRelation = induceRelation;
			return builder.CreateTemplateFromDraggedDataInfo((SnapPieceTable)DocumentModel.Selection.PieceTable, DocumentModel.Selection.End, dataInfoArray);
		}
		protected virtual TemplateBuilder CreateTemplateBuilder() {
			return ((SnapDocumentModel)DocumentModel).CreateTemplateBuilder();
		}
		protected internal override bool IsDataAvailable() {
			return PasteSource.ContainsData(SnapDataFormats.SNDataInfoFullName);
		}
	}
}
