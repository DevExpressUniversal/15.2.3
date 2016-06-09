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
using System.IO;
using System.Text;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Export.Rtf;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Export;
using DevExpress.XtraRichEdit.Export.Html;
using DevExpress.Office.Services;
using DevExpress.Utils;
using System.ComponentModel;
using DevExpress.XtraRichEdit.API.Native.Implementation;
using ModelSelectionRange = DevExpress.XtraRichEdit.Model.SelectionRange;
using DevExpress.Office;
using DevExpress.Compatibility.System.Windows.Forms;
using System.Windows.Forms;
using DevExpress.Office.Utils;
using DevExpress.Office.Utils.Internal;
namespace DevExpress.XtraRichEdit.Commands {
	#region CopySelectionCommand
	public class CopySelectionCommand : RichEditCaretBasedCommand {
		readonly CopySelectionManager copySelectionManager;
		public CopySelectionCommand(IRichEditControl control)
			: base(control) {
			this.copySelectionManager = CreateCopySelectionManager();
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("CopySelectionCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.CopySelection; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("CopySelectionCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_CopySelection; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("CopySelectionCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_CopySelectionDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("CopySelectionCommandImageName")]
#endif
		public override string ImageName { get { return "Copy"; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("CopySelectionCommandFixLastParagraph")]
#endif
		public bool FixLastParagraph { get { return copySelectionManager.FixLastParagraph; } set { copySelectionManager.FixLastParagraph = value; } }
		protected internal DefaultPropertiesCopyOptions DefaultPropertiesCopyOptions { get { return copySelectionManager.DefaultPropertiesCopyOptions; } set { copySelectionManager.DefaultPropertiesCopyOptions = value; } }
		protected internal CopySelectionManager CopySelectionManager { get { return copySelectionManager; } }
		#endregion
		protected internal override void ExecuteCore() {
#if SL
			Control.UpdateUIFromBackgroundThread(delegate {
#endif
			Selection selection = DocumentModel.Selection;
			CopySelectionManager.KeepFieldCodeViewState = true;
			CopySelectionManager.CopyDocumentRange(ActivePieceTable, selection.GetSortedSelectionCollection());
#if SL
			});
#endif
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
#if !SL
			CheckExecutedAtUIThread();
#endif
			state.Checked = false;
			ApplyCommandsRestriction(state, Options.Behavior.Copy, DocumentModel.Selection.Length > 0);
		}
		protected internal virtual CopySelectionManager CreateCopySelectionManager() {
			return Control.InnerDocumentServer.CreateCopySelectionManager();
		}
	}
	#endregion
#if SILVERLIGHT
	#region CopyInternalSelectionCommand
	public class CopyInternalSelectionCommand : CopySelectionCommand {
		public CopyInternalSelectionCommand(IRichEditControl control)
			: base(control) {
		}
		protected internal override CopySelectionManager CreateCopySelectionManager() {
			return new CopyInternalSelectionManager(Control.InnerDocumentServer);
		}
	}
	#endregion
#endif
}
namespace DevExpress.XtraRichEdit.Commands.Internal {
	#region CopySelectionManager
	public class CopySelectionManager {
		readonly InnerRichEditDocumentServer documentServer;
		bool fixLastParagraph;
		bool allowCopyWholeFieldResult;
		bool keepFieldCodeViewState;
		DefaultPropertiesCopyOptions defaultPropertiesCopyOptions = DefaultPropertiesCopyOptions.Never;
		public CopySelectionManager(InnerRichEditDocumentServer documentServer) {
			Guard.ArgumentNotNull(documentServer, "documentServer");
			this.documentServer = documentServer;
		}
		public bool FixLastParagraph { get { return fixLastParagraph; } set { fixLastParagraph = value; } }
		public bool AllowCopyWholeFieldResult { get { return allowCopyWholeFieldResult; } set { allowCopyWholeFieldResult = value; } }
		public bool KeepFieldCodeViewState { get { return keepFieldCodeViewState; } set { keepFieldCodeViewState = value; } }
		protected internal DefaultPropertiesCopyOptions DefaultPropertiesCopyOptions { get { return defaultPropertiesCopyOptions; } set { defaultPropertiesCopyOptions = value; } }
		protected internal InnerRichEditDocumentServer DocumentServer { get { return documentServer; } }
		protected internal virtual void SubscribeTargetModelEvents(DocumentModel targetModel) {
			targetModel.BeforeExport += OnBeforeExport;
			targetModel.CalculateDocumentVariable += OnCalculateDocumentVariable;
		}
		protected internal virtual void UnsubscribeTargetModelEvents(DocumentModel targetModel) {
			targetModel.BeforeExport -= OnBeforeExport;
			targetModel.CalculateDocumentVariable -= OnCalculateDocumentVariable;
		}
		protected internal virtual void OnBeforeExport(object sender, BeforeExportEventArgs e) {
			DocumentServer.OnBeforeExport(sender, e);
		}
		protected internal virtual void OnCalculateDocumentVariable(object sender, CalculateDocumentVariableEventArgs e) {
			DocumentServer.OnCalculateDocumentVariable(sender, e);
		}
		protected internal virtual string GetRtfText(PieceTable pieceTable, SelectionRangeCollection selectionRanges) {
			return GetRtfText(pieceTable, selectionRanges, null, false, false);
		}
		protected internal virtual string GetRtfText(PieceTable pieceTable, SelectionRangeCollection selectionRanges, RtfDocumentExporterOptions options, bool forceRaiseBeforeExport, bool forceRaiseAfterExport) {
			try {
				using (DocumentModel target = CreateDocumentModel(ParagraphNumerationCopyOptions.CopyIfWholeSelected, FormattingCopyOptions.UseDestinationStyles, pieceTable, selectionRanges, true, null)) {
					SubscribeTargetModelEvents(target);
					try {
						bool lastParagraphRunSelected = IsLastParagraphRunSelected(pieceTable, selectionRanges);
						return target.InternalAPI.GetDocumentRtfContent(options, !lastParagraphRunSelected, KeepFieldCodeViewState, forceRaiseBeforeExport, forceRaiseAfterExport);
					}
					finally {
						UnsubscribeTargetModelEvents(target);
					}
				}
			}
			catch {
				return null;
			}
		}
		protected internal bool IsLastParagraphRunSelected(PieceTable pieceTable, SelectionRangeCollection selectionRanges) {
			DevExpress.XtraRichEdit.Model.SelectionRange lastRange = selectionRanges.Last;
			int selectionRangeIndex = selectionRanges.Count - 1;
			while(lastRange.Length == 0 && selectionRangeIndex > 0) {
				selectionRangeIndex--;
				lastRange = selectionRanges[selectionRangeIndex];
			}
			if(lastRange.Length == 0)
				return false;
			DocumentLogPosition lastLogPosition = Algorithms.Max(lastRange.Start, lastRange.End);			
			ParagraphIndex paragraphIndex = pieceTable.FindParagraphIndexCore(lastLogPosition);
			ParagraphCollection paragraphs = pieceTable.Paragraphs;
			if (paragraphIndex == new ParagraphIndex(~paragraphs.Count))
				return true;
			if(paragraphIndex < ParagraphIndex.Zero)
				return false;
			Paragraph paragraph = paragraphs[paragraphIndex];
			return lastLogPosition == paragraph.LogPosition || lastLogPosition == paragraph.EndLogPosition + 1;				
		}
		protected internal string GetSuppressStoreImageSizeCollection(PieceTable pieceTable, SelectionRangeCollection selectionCollection) {
			using (DocumentModel target = CreateDocumentModel(ParagraphNumerationCopyOptions.CopyIfWholeSelected, FormattingCopyOptions.UseDestinationStyles, pieceTable, selectionCollection)) {
				try {
					TextRunCollection runs = target.ActivePieceTable.Runs;
					StringBuilder suppressStoreImageScale = new StringBuilder();
					for (int i = 0; i < runs.Count; i++) {
						InlinePictureRun pictureRun = runs[new RunIndex(i)] as InlinePictureRun;
						if (pictureRun != null) {
							if (InternalOfficeImageHelper.GetSuppressStore(pictureRun.Image))
								suppressStoreImageScale.Append(String.Format("{0},{1};", pictureRun.ScaleX, pictureRun.ScaleY));
						}
					}
					return suppressStoreImageScale.ToString();
				}
				finally {
				}
			}
		}
		protected internal virtual string GetPlainText(PieceTable pieceTable, SelectionRangeCollection selectionCollection) {
			return GetPlainText(pieceTable, selectionCollection, null, null);
		}
		protected internal virtual string GetPlainText(PieceTable pieceTable, SelectionRangeCollection selectionCollection, PlainTextDocumentExporterOptions options, TextFragmentOptions getTextOptions) {
			try {
				using (DocumentModel target = CreateDocumentModel(ParagraphNumerationCopyOptions.CopyAlways, FormattingCopyOptions.UseDestinationStyles, pieceTable, selectionCollection, true, getTextOptions, true)) {
					target.ModelForExport = true;
					SubscribeTargetModelEvents(target);
					try {
						return target.InternalAPI.GetDocumentPlainTextContent(options);
					}
					finally {
						UnsubscribeTargetModelEvents(target);
					}
				}
			}
			catch {
				return null;
			}
		}
		protected internal virtual string GetHtmlText(PieceTable pieceTable, SelectionRangeCollection selectionCollection, IUriProvider provider, HtmlDocumentExporterOptions options) {
			using (DocumentModel target = CreateDocumentModel(ParagraphNumerationCopyOptions.CopyAlways, FormattingCopyOptions.UseDestinationStyles, pieceTable, selectionCollection)) {
				SubscribeTargetModelEvents(target);
				try {
					return target.InternalAPI.GetDocumentHtmlContent(provider, options);
				}
				finally {
					UnsubscribeTargetModelEvents(target);
				}
			}
		}
		protected internal virtual string GetMhtText(PieceTable pieceTable, SelectionRangeCollection selectionCollection) {
			return GetMhtText(pieceTable, selectionCollection, null);
		}
		protected internal virtual string GetMhtText(PieceTable pieceTable, SelectionRangeCollection selectionCollection, MhtDocumentExporterOptions options) {
			using (DocumentModel target = CreateDocumentModel(ParagraphNumerationCopyOptions.CopyAlways, FormattingCopyOptions.UseDestinationStyles, pieceTable, selectionCollection)) {
				SubscribeTargetModelEvents(target);
				try {
					return target.InternalAPI.GetDocumentMhtContent(options);
				}
				finally {
					UnsubscribeTargetModelEvents(target);
				}
			}
		}
		protected internal virtual byte[] GetOpenXmlBytes(PieceTable pieceTable, SelectionRangeCollection selectionCollection) {
			return GetOpenXmlBytes(pieceTable, selectionCollection, null);
		}
		protected internal virtual byte[] GetOpenXmlBytes(PieceTable pieceTable, SelectionRangeCollection selectionCollection, OpenXmlDocumentExporterOptions options) {
			using (DocumentModel target = CreateDocumentModel(ParagraphNumerationCopyOptions.CopyAlways, FormattingCopyOptions.UseDestinationStyles, pieceTable, selectionCollection)) {
				SubscribeTargetModelEvents(target);
				try {
					return target.InternalAPI.GetDocumentOpenXmlContent(options);
				}
				finally {
					UnsubscribeTargetModelEvents(target);
				}
			}
		}
		protected internal virtual string GetWordMLText(PieceTable pieceTable, SelectionRangeCollection selectionCollection) {
			return GetWordMLText(pieceTable, selectionCollection, null);
		}
		protected internal virtual string GetWordMLText(PieceTable pieceTable, SelectionRangeCollection selectionCollection, WordMLDocumentExporterOptions options) {
			using (DocumentModel target = CreateDocumentModel(ParagraphNumerationCopyOptions.CopyAlways, FormattingCopyOptions.UseDestinationStyles, pieceTable, selectionCollection)) {
				SubscribeTargetModelEvents(target);
				try {
					return target.InternalAPI.GetDocumentWordMLContent(options);
				}
				finally {
					UnsubscribeTargetModelEvents(target);
				}
			}
		}
		protected internal virtual MemoryStream GetSilverlightXamlStream(PieceTable pieceTable, SelectionRangeCollection selectionCollection) {
			try {
				MemoryStream result = new MemoryStream();
				byte[] bytes = Encoding.UTF8.GetBytes(GetSilverlightXamlText(pieceTable, selectionCollection));
				result.Write(bytes, 0, bytes.Length);
				return result;
			}
			catch {
				return null;
			}
		}
		protected internal virtual string GetSilverlightXamlText(PieceTable pieceTable, SelectionRangeCollection selectionCollection) {
			using (DocumentModel target = CreateDocumentModel(ParagraphNumerationCopyOptions.CopyAlways, FormattingCopyOptions.UseDestinationStyles, pieceTable, selectionCollection)) {
				SubscribeTargetModelEvents(target);
				try {
					XamlDocumentExporterOptions options = new XamlDocumentExporterOptions();
					options.SilverlightCompatible = true;
					return target.InternalAPI.GetDocumentXamlContent(options);
				}
				finally {
					UnsubscribeTargetModelEvents(target);
				}
			}
		}
		protected internal virtual DocumentModel CreateDocumentModel(ParagraphNumerationCopyOptions paragraphNumerationCopyOptions, FormattingCopyOptions formattingCopyOptions, PieceTable sourcePieceTable, SelectionRangeCollection selectionRanges) {
			return CreateDocumentModel(paragraphNumerationCopyOptions, formattingCopyOptions, sourcePieceTable, selectionRanges, false, null);
		}
		protected internal virtual DocumentModel CreateDocumentModel(ParagraphNumerationCopyOptions paragraphNumerationCopyOptions, FormattingCopyOptions formattingCopyOptions, PieceTable sourcePieceTable, SelectionRangeCollection selectionRanges, bool suppressFieldsUpdate, TextFragmentOptions getTextOptions) {
			return CreateDocumentModel(paragraphNumerationCopyOptions, formattingCopyOptions, sourcePieceTable, selectionRanges, suppressFieldsUpdate, getTextOptions, false);
		}
		protected internal virtual DocumentModel CreateDocumentModel(ParagraphNumerationCopyOptions paragraphNumerationCopyOptions, FormattingCopyOptions formattingCopyOptions, PieceTable sourcePieceTable, SelectionRangeCollection selectionRanges, bool suppressFieldsUpdate, TextFragmentOptions getTextOptions, bool modelForTextExport) {
			DocumentModel target = sourcePieceTable.DocumentModel.CreateNew(modelForTextExport);
			if (getTextOptions == null)
				getTextOptions = new TextFragmentOptions();
			if (getTextOptions.PreserveOriginalNumbering)
				target.MainPieceTable.PrecalculatedNumberingListTexts = new Dictionary<Paragraph, string>();
			if (!getTextOptions.AllowExtendingDocumentRange)
				selectionRanges = UpdateSelectionCollection(sourcePieceTable, selectionRanges);
			target.InternalAPI.ImporterFactory = sourcePieceTable.DocumentModel.InternalAPI.ImporterFactory;
			target.InternalAPI.ExporterFactory = sourcePieceTable.DocumentModel.InternalAPI.ExporterFactory;
			target.FieldOptions.UpdateFieldsOnPaste = sourcePieceTable.DocumentModel.FieldOptions.UpdateFieldsOnPaste;
			target.CalculateDocumentVariable += OnCalculateDocumentVariable;
			target.BeginUpdate();
			try {
				DocumentModelCopyOptions options = new DocumentModelCopyOptions(selectionRanges);
				options.ParagraphNumerationCopyOptions = paragraphNumerationCopyOptions;
				options.FormattingCopyOptions = formattingCopyOptions;
				options.DefaultPropertiesCopyOptions = DefaultPropertiesCopyOptions;
				DocumentModelCopyCommand copyCommand = sourcePieceTable.DocumentModel.CreateDocumentModelCopyCommand(sourcePieceTable, target, options);
				copyCommand.FixLastParagraph = FixLastParagraph;
				copyCommand.SuppressFieldsUpdate = suppressFieldsUpdate;
				copyCommand.AllowCopyWholeFieldResult = AllowCopyWholeFieldResult;
				copyCommand.Execute();
			} finally {
				target.EndUpdate();
			}
			target.CalculateDocumentVariable -= OnCalculateDocumentVariable;
			return target;
		}
		protected internal virtual SelectionRangeCollection UpdateSelectionCollection(PieceTable sourcePieceTable, SelectionRangeCollection selection) {
			AllowCopyWholeFieldResult = false;
			SelectionRangeCollection result = new SelectionRangeCollection();
			for (int i = 0; i < selection.Count; i++) {
				ModelSelectionRange range = selection[i];
				RunInfo info = sourcePieceTable.FindRunInfo(range.Start, range.Length);
				DocumentLogPosition start = range.Start;
				DocumentLogPosition end = range.End;
				Field field = sourcePieceTable.FindFieldByRunIndex(info.Start.RunIndex);
				if (ShouldExtendRange(info, field)) {
					start = Max(start, sourcePieceTable.GetRunLogPosition(field.Result.Start));
					end = Min(end, sourcePieceTable.GetRunLogPosition(field.Result.End));
					if (start < end)
						result.Add(new ModelSelectionRange(start, end - start));
					start = end + 1;
				}
				field = sourcePieceTable.FindFieldByRunIndex(info.End.RunIndex);
				if (ShouldExtendRange(info, field)) {
					end = sourcePieceTable.GetRunLogPosition(field.Code.Start);
					if (start >= end)
						continue;
					result.Add(new ModelSelectionRange(start, end - start));
					start = sourcePieceTable.GetRunLogPosition(field.Result.Start);
					end = range.End;
					if (start < end)
						result.Add(new ModelSelectionRange(start, end - start));
				}
				else
					result.Add(new ModelSelectionRange(start, range.End - start));
			}
			return result;
		}
		bool ShouldExtendRange(RunInfo info, Field field) {
			if (field == null)
				return false;
			return !FieldsOperation.IsFieldCodeTextAffectedOnly(info, field) &&
				!FieldsOperation.IsFieldResultTextAffectedOnly(info, field) &&
				!FieldsOperation.IsEntireFieldAffected(info, field);
		}
		DocumentLogPosition Min(DocumentLogPosition val1, DocumentLogPosition val2) {
			if (val1 < val2)
				return val1;
			return val2;
		}
		DocumentLogPosition Max(DocumentLogPosition val1, DocumentLogPosition val2) {
			if (val1 > val2)
				return val1;
			return val2;
		}
		protected internal virtual void CopyDocumentRange(PieceTable pieceTable, SelectionRangeCollection selectionCollection) {
			if (selectionCollection.Count <= 0 || selectionCollection[0].Length <= 0)
				return;
			IDataObject dataObject = new DataObject();
			DefaultPropertiesCopyOptions oldCopyOptions = DefaultPropertiesCopyOptions;
			try {
				SetDataObject(pieceTable, selectionCollection, dataObject);
			}
			finally {
				DefaultPropertiesCopyOptions = oldCopyOptions;					
			}
			try {
				OfficeClipboard.Clear();
				OfficeClipboard.SetDataObject(dataObject, true);
			}
			catch (Exception e) {
				DocumentServer.RaiseClipboardSetDataException(e);
			}
		}
		protected internal virtual void SetDataObject(PieceTable pieceTable, SelectionRangeCollection selectionCollection, IDataObject dataObject) {
			DefaultPropertiesCopyOptions = DefaultPropertiesCopyOptions.Always;
			RtfDocumentExporterOptions options = new RtfDocumentExporterOptions();
			options.ExportFinalParagraphMark = ExportFinalParagraphMark.Never;
			string rtfText = GetRtfText(pieceTable, selectionCollection, options, true, true);
			if (!String.IsNullOrEmpty(rtfText))
				dataObject.SetData(OfficeDataFormats.Rtf, rtfText);
			string plainText = GetPlainText(pieceTable, selectionCollection);
			if (!String.IsNullOrEmpty(plainText))
				dataObject.SetData(OfficeDataFormats.UnicodeText, plainText.Replace(Characters.NonBreakingSpace, Characters.Space));
			MemoryStream stream = GetSilverlightXamlStream(pieceTable, selectionCollection);
			if (stream != null)
				dataObject.SetData(OfficeDataFormats.SilverlightXaml, stream);
			string suppressStoreImageScale = GetSuppressStoreImageSizeCollection(pieceTable, selectionCollection);
			if (suppressStoreImageScale != null)
				dataObject.SetData(OfficeDataFormats.SuppressStoreImageSize, suppressStoreImageScale);
		}
		public void Copy(IRichEditControl control) {
		}
	}
	#endregion
#if SILVERLIGHT
	#region CopyInternalSelectionManager
	public class CopyInternalSelectionManager : CopySelectionManager {
		public CopyInternalSelectionManager(InnerRichEditDocumentServer documentServer) : base(documentServer) {
		}
		protected internal override void CopyDocumentRange(PieceTable pieceTable, SelectionRangeCollection selectionRanges) {
			if (selectionRanges.Count <= 0 || selectionRanges[0].Length <= 0)
				return;
			OfficeClipboard.rtfClipboard = GetRtfText(pieceTable, selectionRanges);
		}
	}
	#endregion
#endif
}
