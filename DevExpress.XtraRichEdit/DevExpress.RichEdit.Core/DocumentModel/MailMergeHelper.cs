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
using DevExpress.Office.Services;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.Services;
namespace DevExpress.XtraRichEdit.Model {
	public enum MergeMode {
		NewParagraph,
		NewSection,
		JoinTables,
	}
	public interface IDataSourceContainerOptions {
		object DataSource { get; set; }
		string DataMember { get; set; }
	}
	public interface IMailMergeOptions : IDataSourceContainerOptions {
		int FirstRecordIndex { get; set; }
		int LastRecordIndex { get; set; }
		bool CopyTemplateStyles { get; set; }
		bool HeaderFooterLinkToPrevious { get; set; }
	}
	public class MailMergeOptions : IMailMergeOptions {
		MergeMode mergeMode;
		int firstRecordIndex = 0;
		int lastRecordIndex = -1;
		object dataSource;
		string dataMember;
		bool copyTemplateStyles;
		bool headerFooterLinkToPrevious = true;
		public MergeMode MergeMode {
			get { return mergeMode; }
			set { mergeMode = value; }
		}
		public int FirstRecordIndex { get { return firstRecordIndex; } set { firstRecordIndex = value; } }
		public int LastRecordIndex { get { return lastRecordIndex; } set { lastRecordIndex = value; } }
		public bool CopyTemplateStyles { get { return copyTemplateStyles; } set { copyTemplateStyles = value; } }
		public bool HeaderFooterLinkToPrevious { get { return headerFooterLinkToPrevious; } set { headerFooterLinkToPrevious = value; } }
		public object DataSource {
			get { return dataSource; }
			set { dataSource = value; }
		}
		public string DataMember {
			get { return dataMember; }
			set { dataMember = value; }
		}
	}
	public class CalculateDocumentVariableEventRouter {
		readonly DocumentModel targetModel;
		public CalculateDocumentVariableEventRouter(DocumentModel targetModel) {
			Guard.ArgumentNotNull(targetModel, "targetModel");
			this.targetModel = targetModel;
		}
		public void OnCalculateDocumentVariable(object sender, CalculateDocumentVariableEventArgs e) {
			e.Value = targetModel.Variables.GetVariableValue(e.VariableName, e.Arguments, e.Field, e.UnhandledValue);
			DevExpress.XtraRichEdit.Internal.InternalRichEditDocumentServer server = DevExpress.XtraRichEdit.Internal.InternalRichEditDocumentServer.TryConvertInternalRichEditDocumentServer(e.Value);
			DocumentModel model = sender as DocumentModel;
			if (server != null && model != null)
				model.MailMergeOptions.KeepLastParagraph = server.DocumentModel.MailMergeOptions.KeepLastParagraph;
			e.Handled = true;
		}
	}
	public abstract class MailMergeHelperBase {
		readonly IMailMergeOptions options;
		readonly DocumentModel sourceModel;
		readonly IProgressIndicationService progressIndication;
		protected MailMergeHelperBase(DocumentModel sourceModel, IMailMergeOptions options)
			: this(sourceModel, options, null) {
		}
		protected MailMergeHelperBase(DocumentModel sourceModel, IMailMergeOptions options, IProgressIndicationService progressIndication) {
			Guard.ArgumentNotNull(sourceModel, "sourceModel");
			Guard.ArgumentNotNull(options, "options");
			this.sourceModel = sourceModel;
			this.options = options;
			this.progressIndication = progressIndication ?? new ProgressIndication(sourceModel);
		}
		protected virtual DocumentModel SourceModel { get { return sourceModel; } }
		protected virtual IMailMergeOptions Options { get { return options; } }
		protected IProgressIndicationService ProgressIndication { get { return progressIndication; } }
		protected abstract string OperationDescription { get; }
		public virtual void ExecuteMailMerge(DocumentModel targetModel) {
			IMailMergeDataService sourceFieldDataService = SourceModel.GetService<IMailMergeDataService>();
			if (!sourceFieldDataService.BoundMode && Options.DataSource == null)
				return;
			IMailMergeDataService fieldDataService = sourceFieldDataService.StartMailMerge(Options);
			if (fieldDataService == null)
				return;
			try {
				ExecuteMailMergeCore(targetModel, fieldDataService);
			}
			finally {
				fieldDataService.EndMailMerge(fieldDataService);
			}
		}
		protected internal virtual void ExecuteMailMergeCore(DocumentModel targetModel, IMailMergeDataService fieldDataService) {
			PrepareTargetModel(targetModel);
			if (!RaiseMailMergeStarted(fieldDataService, targetModel))
				return;
			int lastRecordIndex = Options.LastRecordIndex;
			if (lastRecordIndex < 0)
				lastRecordIndex = fieldDataService.RecordCount;
			ProgressIndication.Begin(OperationDescription, Options.FirstRecordIndex, lastRecordIndex, Options.FirstRecordIndex);
			IFieldCalculatorService fieldCalculatorService = SourceModel.GetService<IFieldCalculatorService>();
			IUriStreamService uriStreamService = SourceModel.GetService<IUriStreamService>();
			bool firstRecord = true;
			for (; ; ) {
				if (!MailMergeRecord(targetModel, fieldDataService, fieldCalculatorService, uriStreamService, firstRecord))
					break;
				firstRecord = false;
				if (!fieldDataService.MoveNextRecord())
					break;
				ProgressIndication.SetProgress(fieldDataService.GetCurrentRecordIndex());
			}
			targetModel.MainPieceTable.FixLastParagraph();
			Selection selection = targetModel.Selection;
			selection.Start = DocumentLogPosition.Zero;
			selection.End = DocumentLogPosition.Zero;
			ProgressIndication.End();
			RaiseMailMergeFinished(fieldDataService, targetModel);
		}
		void PrepareTargetModel(DocumentModel targetModel) {
			targetModel.DocumentProperties.CopyFrom(SourceModel.DocumentProperties.Info);
			DocumentModelCopyCommand.ReplaceDefaultProperties(targetModel, SourceModel);
			DocumentModelCopyCommand.ReplaceDefaultStyles(targetModel, SourceModel);
			if (Options.CopyTemplateStyles)
				CopyTemplateStyles(targetModel, SourceModel);
		}
		protected internal virtual bool MailMergeRecord(DocumentModel targetModel, IMailMergeDataService fieldDataService, IFieldCalculatorService fieldCalculatorService, IUriStreamService uriStreamService, bool firstRecord) {
			bool result = true;
			using (DocumentModel intermediateResult = CreateIntermediateResultModel(targetModel)) {
				intermediateResult.FieldOptions.CopyFrom(SourceModel.FieldOptions);
				CalculateDocumentVariableEventRouter eventRouter = new CalculateDocumentVariableEventRouter(targetModel);
				intermediateResult.CalculateDocumentVariable += eventRouter.OnCalculateDocumentVariable;
				intermediateResult.BeginSetContent();
				try {
					PieceTable mainPieceTable = SourceModel.MainPieceTable;
					DocumentLogPosition startLogPosition = mainPieceTable.DocumentStartLogPosition;
					DocumentLogPosition endLogPosition = mainPieceTable.DocumentEndLogPosition;
					DocumentModelCopyOptions options = new DocumentModelCopyOptions(startLogPosition, endLogPosition - startLogPosition + 1);
					options.CopyDocumentVariables = true;
					DocumentModelCopyCommand copyCommand = SourceModel.CreateDocumentModelCopyCommand(mainPieceTable, intermediateResult, options);
					copyCommand.FixLastParagraph = AllowFixLastParagraph(mainPieceTable);
					copyCommand.SuppressFieldsUpdate = true;
					copyCommand.Execute();
					if (!fieldDataService.BoundMode)
						Exceptions.ThrowInternalException();
					intermediateResult.DocumentSaveOptions.CurrentFileName = SourceModel.DocumentSaveOptions.CurrentFileName;
					ReplaceServices(intermediateResult, fieldDataService, fieldCalculatorService, uriStreamService);
				}
				finally {
					intermediateResult.EndSetContent(DocumentModelChangeType.LoadNewDocument, false, null);
					result = MailMergeRecordCore(targetModel, intermediateResult, fieldDataService, firstRecord);
					intermediateResult.CalculateDocumentVariable -= eventRouter.OnCalculateDocumentVariable;
				}
			}
			return result;
		}
		protected virtual DocumentModel CreateIntermediateResultModel(DocumentModel targetModel) {
			DocumentModel result = targetModel.CreateNew();
			result.IntermediateModel = true;
			result.ModelForExport = true;
			return result;
		}
		protected internal virtual bool MailMergeRecordCore(DocumentModel targetModel, DocumentModel intermediateResult, IMailMergeDataService fieldDataService, bool firstRecord) {
			bool result = PrepareRecordResult(targetModel, intermediateResult, fieldDataService);
			if (result)
				AppendRecordResult(targetModel, intermediateResult, fieldDataService, firstRecord);
			return result;
		}
		protected internal virtual void AppendRecordResult(DocumentModel targetModel, DocumentModel intermediateResult, IMailMergeDataService fieldDataService, bool firstRecord) {
			BeforeRecordInserted(targetModel, firstRecord);
			CopyFromIntermediateResult(intermediateResult.MainPieceTable, targetModel.MainPieceTable);
			AfterRecordInserted(intermediateResult, targetModel);
		}
		bool PrepareRecordResult(DocumentModel targetModel, DocumentModel intermediateResult, IMailMergeDataService fieldDataService) {
			PrepareRecordData(intermediateResult);
			if (!RaiseMailMergeRecordStarted(fieldDataService, targetModel, intermediateResult))
				return false;
			intermediateResult.MailMergeOptions.CustomSeparators.Assign(SourceModel.MailMergeOptions.CustomSeparators);
			PrepareRecordResultCore(intermediateResult, fieldDataService);
			if (!RaiseMailMergeRecordFinished(fieldDataService, targetModel, intermediateResult))
				return false;
			return true;
		}
		protected virtual void PrepareRecordData(DocumentModel recordModel) {
		}
		protected internal virtual void PrepareRecordResultCore(DocumentModel intermediateResult, IMailMergeDataService fieldDataService) {
			UpdateFields(intermediateResult);
		}
		protected virtual void BeforeRecordInserted(DocumentModel targetModel, bool firstRecord) {
		}
		protected virtual void AfterRecordInserted(DocumentModel intermediateResult, DocumentModel targetModel) {
			if (NewSectionInserted() && !Options.HeaderFooterLinkToPrevious)
				UpdateSectionHeadersFooters(intermediateResult.Sections.Last, targetModel.Sections.Last);
		}
		protected internal virtual bool NewSectionInserted() {
			return false;
		}
		protected void CopyFromIntermediateResult(PieceTable intermediateResult, PieceTable target) {
			DocumentModelCopyManager copyManager = new DocumentModelCopyManager(intermediateResult, target, ParagraphNumerationCopyOptions.CopyAlways);
			copyManager.TargetPosition.CopyFrom(CalculateTargetPosition(target));
			CopySectionOperation operation = intermediateResult.DocumentModel.CreateCopySectionOperation(copyManager);
			operation.FixLastParagraph = true;
			operation.Execute(intermediateResult.DocumentStartLogPosition, intermediateResult.DocumentEndLogPosition - intermediateResult.DocumentStartLogPosition + 1, false);
			ApplyLastSectionSize(target.DocumentModel, intermediateResult.DocumentModel);
		}
		protected virtual DocumentModelPosition CalculateTargetPosition(PieceTable target) {
			return DocumentModelPosition.FromDocumentEnd(target);
		}
		void ApplyLastSectionSize(DocumentModel targetModel, DocumentModel sourceModel) {
			targetModel.Sections.Last.CopyFromCore(sourceModel.Sections.Last);
		}
		protected internal abstract bool RaiseMailMergeStarted(IMailMergeDataService fieldDataService, DocumentModel targetModel);
		protected internal virtual bool RaiseMailMergeRecordStarted(IMailMergeDataService fieldDataService, DocumentModel targetModel, DocumentModel recordModel) {
			MailMergeRecordStartedEventArgs recordStartedArgs = new MailMergeRecordStartedEventArgs(targetModel, recordModel);
			recordStartedArgs.RecordIndex = fieldDataService.GetCurrentRecordIndex();
			return SourceModel.RaiseMailMergeRecordStarted(recordStartedArgs);
		}
		protected internal virtual bool RaiseMailMergeRecordFinished(IMailMergeDataService fieldDataService, DocumentModel targetModel, DocumentModel recordModel) {
			MailMergeRecordFinishedEventArgs recordFinishedArgs = new MailMergeRecordFinishedEventArgs(targetModel, recordModel);
			recordFinishedArgs.RecordIndex = fieldDataService.GetCurrentRecordIndex();
			return SourceModel.RaiseMailMergeRecordFinished(recordFinishedArgs);
		}
		protected internal virtual void RaiseMailMergeFinished(IMailMergeDataService fieldDataService, DocumentModel targetModel) {
			MailMergeFinishedEventArgs finishedArgs = new MailMergeFinishedEventArgs(targetModel);
			SourceModel.RaiseMailMergeFinished(finishedArgs);
		}
		protected virtual void CopyTemplateStyles(DocumentModel targetModel, DocumentModel SourceModel) {
			if (targetModel.DocumentCapabilities.CharacterStyleAllowed) {
				CharacterStyleCollection characterStyles = SourceModel.CharacterStyles;
				int count = characterStyles.Count;
				for (int i = 0; i < count; i++)
					characterStyles[i].Copy(targetModel);
			}
			if (targetModel.DocumentCapabilities.ParagraphStyleAllowed) {
				ParagraphStyleCollection paragraphStyles = SourceModel.ParagraphStyles;
				int count = paragraphStyles.Count;
				for (int i = 0; i < count; i++)
					paragraphStyles[i].Copy(targetModel);
			}
			if (targetModel.DocumentCapabilities.TableStyleAllowed) {
				TableStyleCollection tableStyles = SourceModel.TableStyles;
				int count = tableStyles.Count;
				for (int i = 0; i < count; i++)
					tableStyles[i].Copy(targetModel);
			}
		}
		protected internal virtual void ReplaceServices(DocumentModel documentModel, IMailMergeDataService mailMergeDataService, IFieldCalculatorService fieldCalculatorService, IUriStreamService uriStreamService) {
			documentModel.ReplaceService<IFieldDataService>(mailMergeDataService);
			documentModel.ReplaceService(mailMergeDataService);
			documentModel.ReplaceService(fieldCalculatorService);
			documentModel.ReplaceService(uriStreamService);
		}
		protected internal virtual void UpdateFields(DocumentModel documentModel) {
			documentModel.BeginUpdate();
			try {
				UpdateFieldsCore(documentModel);
			}
			finally {
				documentModel.EndUpdate();
			}
		}
		protected internal virtual void UpdateFieldsCore(DocumentModel documentModel) {
			List<PieceTable> pieceTables = documentModel.GetPieceTables(false);
			int count = pieceTables.Count;
			for (int i = 0; i < count; i++)
				UpdateFieldsCore(pieceTables[i]);
		}
		protected internal virtual void UpdateFieldsCore(PieceTable pieceTable) {
			FieldCollection fields = pieceTable.Fields;
			MailMergeFieldUpdater updater = new MailMergeFieldUpdater(pieceTable);
			int count = fields.Count;
			for (int i = 0; i < count; i++)
				updater.PrepareFieldUpdate(fields[i], UpdateFieldOperationType.Normal);
			updater.UpdateFields(UpdateFieldOperationType.Normal);
		}
		protected internal virtual bool AllowFixLastParagraph(PieceTable pieceTable) {
			return true;
		}
		protected void UpdateSectionHeadersFooters(Section intermediate, Section target) {
			UpdateSectionHeaderFooter(intermediate.InnerOddPageHeader, target, target.Headers, (t) => t.InnerOddPageHeader.PieceTable);
			UpdateSectionHeaderFooter(intermediate.InnerEvenPageHeader, target, target.Headers, (t) => t.InnerEvenPageHeader.PieceTable);
			UpdateSectionHeaderFooter(intermediate.InnerFirstPageHeader, target, target.Headers, (t) => t.InnerFirstPageHeader.PieceTable);
			UpdateSectionHeaderFooter(intermediate.InnerOddPageFooter, target, target.Footers, (t) => t.InnerOddPageFooter.PieceTable);
			UpdateSectionHeaderFooter(intermediate.InnerEvenPageFooter, target, target.Footers, (t) => t.InnerEvenPageFooter.PieceTable);
			UpdateSectionHeaderFooter(intermediate.InnerFirstPageFooter, target, target.Footers, (t) => t.InnerFirstPageFooter.PieceTable);
		}
		protected void UpdateSectionHeaderFooter(SectionHeaderFooterBase source, Section target, SectionHeadersFootersBase ownerList, Func<Section, PieceTable> targetProvider) {
			if (source == null)
				return;
			ownerList.Create(source.Type);
			CopyFromIntermediateResult(source.PieceTable, targetProvider(target));
		}
	}
	public class MailMergeHelper : MailMergeHelperBase {
		MailMergeStartedEventArgs startedArgs;
		public MailMergeHelper(DocumentModel sourceModel, MailMergeOptions options)
			: base(sourceModel, options) {
		}
		MergeMode MergeMode { get { return ((MailMergeOptions)Options).MergeMode; } }
		protected override string OperationDescription { get { return startedArgs.OperationDescription; } }
		protected internal override bool NewSectionInserted() {
			return MergeMode == MergeMode.NewSection;
		}
		protected override void BeforeRecordInserted(DocumentModel targetModel, bool firstRecord) {
			if (MergeMode == MergeMode.NewSection && !firstRecord) {
				targetModel.Sections.Last.GeneralSettings.StartType = SectionStartType.NextPage;
				targetModel.InsertSection(targetModel.MainPieceTable.DocumentEndLogPosition, true);
			}
		}
		protected internal override bool AllowFixLastParagraph(PieceTable pieceTable) {
			ParagraphCollection paragraphs = pieceTable.Paragraphs;
			if (MergeMode != MergeMode.JoinTables || paragraphs.Count == 1)
				return false;
			Paragraph lastParagraph = paragraphs.Last;
			bool isTableBeforeParagraph = paragraphs[lastParagraph.Index - 1].GetCell() != null;
			return lastParagraph.IsEmpty && isTableBeforeParagraph;
		}
		protected internal override bool RaiseMailMergeStarted(IMailMergeDataService fieldDataService, DocumentModel targetModel) {
			this.startedArgs = new MailMergeStartedEventArgs(targetModel);
			return SourceModel.RaiseMailMergeStarted(startedArgs);
		}
	}
}
