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

using DevExpress.Office;
using DevExpress.Services;
using DevExpress.Snap.API.Native;
using DevExpress.Snap.Core.API;
using DevExpress.Snap.Core.Native.Options;
using DevExpress.Snap.Core.Native.Services;
using DevExpress.Snap.Core.Options;
using DevExpress.Snap.Core.Services;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.Snap.Core.Native {
	public class SnapMailMergeHelper : MailMergeHelperBase {
		SnapMailMergeStartedEventArgs startedArgs;
		public SnapMailMergeHelper(SnapDocumentModel sourceModel, SnapMailMergeExportOptions options)
			: base(sourceModel, options) {
		}
		public SnapMailMergeHelper(SnapDocumentModel sourceModel, SnapMailMergeExportOptions options, IProgressIndicationService progressIndication)
			: base(sourceModel, options, progressIndication) {
		}
		public new SnapDocumentModel SourceModel { get { return (SnapDocumentModel)base.SourceModel; } }
		RecordSeparator RecordSeparator { get { return ((SnapMailMergeExportOptions)Options).RecordSeparator; } }
		protected override string OperationDescription { get { return startedArgs.OperationDescription; } }
		public bool IsCancellationRequested {
			get {
				SnapMailMergeProgressIndication progressIndication = ProgressIndication as SnapMailMergeProgressIndication;
				if (progressIndication == null)
					return false;
				return progressIndication.IsCancellationRequested;
			}
		}
		protected override void BeforeRecordInserted(DocumentModel targetModel, bool firstRecord) {
			if (firstRecord)
				return;
			PieceTable target = targetModel.MainPieceTable;
			DocumentLogPosition end = target.DocumentEndLogPosition;
			if (RecordSeparator == RecordSeparator.Custom) {
				target.InsertParagraph(end, true);
				SnapNativeDocument document = ((SnapMailMergeExportOptions)Options).CustomSeparator as SnapNativeDocument;
				if (document != null) {
					SnapPieceTable customSeparator = document.PieceTable;
					UpdateFields(customSeparator.DocumentModel);
					CopyFromIntermediateResult(customSeparator, target);
					if (((SnapMailMergeExportOptions)Options).StartEachRecordFromNewParagraph)
						target.InsertParagraph(target.DocumentEndLogPosition);
				}
			}
			else if (RecordSeparator == RecordSeparator.None)
				target.InsertParagraph(end);
			else if (RecordSeparator == RecordSeparator.PageBreak) {
				target.InsertParagraph(end);
				target.InsertText(end + 1, new string(Characters.PageBreak, 1), true);
			}
			else if (NewSectionInserted()) {
				target.InsertParagraph(end, true);
				targetModel.InsertSection(end + 1, true);
			}
			else if (RecordSeparator == RecordSeparator.Paragraph) {
				target.InsertParagraph(end, true);
				target.InsertParagraph(end, true);
			}
		}
		protected override void PrepareRecordData(DocumentModel recordModel) {
			SnapDocumentModel model = (SnapDocumentModel)recordModel;
			model.InheritDataServicesForMailMerge(SourceModel);
			model.DataSources.MailMergeMode = true;
			model.DataSources.MailMergeDataSourceName = ((IDataDispatcherOptions)Options).DataSourceName;
		}
		protected internal override void PrepareRecordResultCore(DocumentModel intermediateResult, IMailMergeDataService fieldDataService) {
			SnapDocumentModel model = (SnapDocumentModel)intermediateResult;
			IDataDispatcherOptions options = (IDataDispatcherOptions)Options;
			DataSourceInfo info = model.DataSourceDispatcher.GetInfo(options.DataSourceName);
			if (info == null)
				if (options.DataSourceName != null) {
					model.BeginUpdateDataSource();
					model.DataSources.Add(options.DataSourceName, options.DataSource);
					model.EndUpdateDataSource();
				}
				else {
					info = model.RegisterDataSource(options.DataSource);
					IDataSourceContainer dataContainer = options as IDataSourceContainer;
					if (dataContainer != null)
						dataContainer.RegisterDataSource(info.DataSourceName, info.DataSource);
					options.DataSourceName = info.DataSourceName;
				}
			int currentRecordIndex = fieldDataService.GetCurrentRecordIndex();
			model.SetRootDataContext(info, options, currentRecordIndex);
			base.PrepareRecordResultCore(intermediateResult, fieldDataService);
		}
		protected override void AfterRecordInserted(DocumentModel intermediateResult, DocumentModel targetModel) {
			base.AfterRecordInserted(intermediateResult, targetModel);
			if (NewSectionInserted() && targetModel.Sections.Count > 1)
				targetModel.Sections.Last.GeneralSettings.StartType = CalcSectionStartType();
		}
		protected internal override bool MailMergeRecord(DocumentModel targetModel, IMailMergeDataService fieldDataService, IFieldCalculatorService fieldCalculatorService, Office.Services.IUriStreamService uriStreamService, bool firstRecord) {
			if (!base.MailMergeRecord(targetModel, fieldDataService, fieldCalculatorService, uriStreamService, firstRecord))
				return false;
			return !IsCancellationRequested;
		}
		protected override DocumentModel CreateIntermediateResultModel(DocumentModel targetModel) {
			DocumentModel result = base.CreateIntermediateResultModel(targetModel);
			result.DocumentImportOptions.CopyFrom(targetModel.DocumentImportOptions);
			return result;
		}
		protected override DocumentModelPosition CalculateTargetPosition(PieceTable target) {
			if (RecordSeparator == RecordSeparator.PageBreak || RecordSeparator == RecordSeparator.Custom)
				return DocumentModelPosition.FromDocumentEnd(target);
			return base.CalculateTargetPosition(target);
		}
		SectionStartType CalcSectionStartType() {
			switch (RecordSeparator) {
				case RecordSeparator.SectionNextPage: return SectionStartType.NextPage;
				case RecordSeparator.SectionEvenPage: return SectionStartType.EvenPage;
				case RecordSeparator.SectionOddPage: return SectionStartType.OddPage;
				default: return SectionStartType.Continuous;
			}
		}
		protected internal override bool NewSectionInserted() {
			return RecordSeparator == RecordSeparator.SectionEvenPage ||
				RecordSeparator == RecordSeparator.SectionNextPage ||
				RecordSeparator == RecordSeparator.SectionOddPage;
		}
		protected internal override bool RaiseMailMergeStarted(IMailMergeDataService fieldDataService, DocumentModel targetModel) {
			this.startedArgs = new SnapMailMergeStartedEventArgs((SnapDocumentModel)targetModel);
			return SourceModel.RaiseSnapMailMergeStarted(startedArgs);
		}
		protected internal override bool RaiseMailMergeRecordStarted(IMailMergeDataService fieldDataService, DocumentModel targetModel, DocumentModel recordModel) {
			SnapMailMergeRecordStartedEventArgs recordStartedArgs = new SnapMailMergeRecordStartedEventArgs((SnapDocumentModel)targetModel, (SnapDocumentModel)recordModel);
			recordStartedArgs.RecordIndex = fieldDataService.GetCurrentRecordIndex();
			return SourceModel.RaiseSnapMailMergeRecordStarted(recordStartedArgs);
		}
		protected internal override bool RaiseMailMergeRecordFinished(IMailMergeDataService fieldDataService, DocumentModel targetModel, DocumentModel recordModel) {
			SnapMailMergeRecordFinishedEventArgs recordFinishedArgs = new SnapMailMergeRecordFinishedEventArgs((SnapDocumentModel)targetModel, (SnapDocumentModel)recordModel);
			recordFinishedArgs.RecordIndex = fieldDataService.GetCurrentRecordIndex();
			return SourceModel.RaiseSnapMailMergeRecordFinished(recordFinishedArgs);
		}
		protected internal override void RaiseMailMergeFinished(IMailMergeDataService fieldDataService, DocumentModel targetModel) {
			SnapMailMergeFinishedEventArgs finishedArgs = new SnapMailMergeFinishedEventArgs((SnapDocumentModel)targetModel);
			SourceModel.RaiseSnapMailMergeFinished(finishedArgs);
		}
	}
}
