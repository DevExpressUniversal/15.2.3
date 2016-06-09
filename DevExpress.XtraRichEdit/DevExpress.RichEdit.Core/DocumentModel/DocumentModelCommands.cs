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
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model.History;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.API.Internal;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Tables.Native;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.XtraRichEdit.Export.Rtf;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Office.History;
using DevExpress.XtraRichEdit.Drawing;
using LayoutUnit = System.Int32;
using ModelUnit = System.Int32;
using Debug = System.Diagnostics.Debug;
#if !SL
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraRichEdit.Model;
using System.Diagnostics;
#else
using System.Windows.Controls;
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Model {
	#region DocumentModelCommand (abstract class)
	public abstract class DocumentModelCommand {
		#region Fields
		readonly DocumentModel documentModel;
		HistoryTransaction transaction;
		#endregion
		protected DocumentModelCommand(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
		}
		#region Properties
		public DocumentModel DocumentModel { get { return documentModel; } }
		public PieceTable PieceTable { get { return documentModel.MainPieceTable; } }
		public HistoryTransaction Transaction { get { return transaction; } }
		#endregion
		public void Execute() {
			BeginExecute();
			ExecuteCore();
			EndExecute();
			DocumentModel.CheckIntegrity();
		}
		protected internal virtual void BeginExecute() {
			DocumentModel.BeginUpdate();
			this.transaction = new HistoryTransaction(DocumentModel.History);
			CalculateExecutionParameters();
			CalculateApplyChangesParameters();
		}
		protected internal virtual void EndExecute() {
			ApplyChanges();
			this.transaction.Dispose();
			DocumentModel.EndUpdate();
		}
		protected internal abstract void ExecuteCore();
		protected internal abstract void CalculateExecutionParameters();
		protected internal abstract void CalculateApplyChangesParameters();
		protected internal abstract void ApplyChanges();
	}
	#endregion
	#region DocumentModelInsertObjectCommand (abstract class)
	public abstract class DocumentModelInsertObjectCommand : DocumentModelCommand {
		#region Fields
		ParagraphIndex paragraphIndex;
		ParagraphIndex applyChangesParagraphIndex;
		#endregion
		protected DocumentModelInsertObjectCommand(DocumentModel documentModel)
			: base(documentModel) {
		}
		#region Properties
		public ParagraphIndex ParagraphIndex { get { return paragraphIndex; } }
		protected internal abstract DocumentModelChangeType ChangeType { get; }
		#endregion
		protected internal override void CalculateExecutionParameters() {
			this.paragraphIndex = CalculateInsertionParagraphIndex();
		}
		protected internal override void CalculateApplyChangesParameters() {
			SectionIndex sectionIndex = PieceTable.LookupSectionIndexByParagraphIndex(ParagraphIndex);
			Section section = DocumentModel.Sections[sectionIndex];
			if (section.LastParagraphIndex == ParagraphIndex && ParagraphIndex > ParagraphIndex.Zero)
				applyChangesParagraphIndex = ParagraphIndex - 1;
			else
				applyChangesParagraphIndex = ParagraphIndex;
			Paragraph paragraph = PieceTable.Paragraphs[applyChangesParagraphIndex];
			PieceTable.ApplyChanges(ChangeType, paragraph.FirstRunIndex, paragraph.LastRunIndex + 1); 
		}
		protected internal override void ApplyChanges() {
			Paragraph paragraph = PieceTable.Paragraphs[applyChangesParagraphIndex];
			PieceTable.ApplyChanges(ChangeType, paragraph.FirstRunIndex, paragraph.LastRunIndex);
		}
		protected internal abstract ParagraphIndex CalculateInsertionParagraphIndex();
	}
	#endregion
	#region DocumentModelInsertObjectAtLogPositionCommand (abstract class)
	public abstract class DocumentModelInsertObjectAtLogPositionCommand : DocumentModelInsertObjectCommand {
		readonly DocumentLogPosition logPosition;
		readonly bool forceVisible;
		protected DocumentModelInsertObjectAtLogPositionCommand(DocumentModel documentModel, DocumentLogPosition logPosition, bool forceVisible)
			: base(documentModel) {
			if (logPosition < DocumentLogPosition.Zero)
				Exceptions.ThrowArgumentException("logPosition", logPosition);
			this.logPosition = logPosition;
			this.forceVisible = forceVisible;
		}
		public DocumentLogPosition LogPosition { get { return logPosition; } }
		protected virtual bool ForceVisible { get { return forceVisible; } }
		protected internal override ParagraphIndex CalculateInsertionParagraphIndex() {
			return PieceTable.FindParagraphIndex(LogPosition);
		}
	}
	#endregion
	#region DocumentModelInsertObjectAtInputPositionCommand
	public abstract class DocumentModelInsertObjectAtInputPositionCommand : DocumentModelInsertObjectCommand {
		readonly InputPosition position;
		protected DocumentModelInsertObjectAtInputPositionCommand(DocumentModel documentModel, InputPosition position)
			: base(documentModel) {
			if (position.LogPosition < DocumentLogPosition.Zero)
				Exceptions.ThrowArgumentException("logPosition", position.LogPosition);
			this.position = position;
		}
		public InputPosition Position { get { return position; } }
		protected internal override ParagraphIndex CalculateInsertionParagraphIndex() {
			return Position.ParagraphIndex;
		}
	}
	#endregion
	#region PieceTableCommand (abstract class)
	public abstract class PieceTableCommand {
		#region Fields
		readonly PieceTable pieceTable;
		HistoryTransaction transaction;
		#endregion
		protected PieceTableCommand(PieceTable pieceTable) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
		}
		#region Properties
		public PieceTable PieceTable { get { return pieceTable; } }
		public DocumentModel DocumentModel { get { return pieceTable.DocumentModel; } }
		public HistoryTransaction Transaction { get { return transaction; } }
		public object Result { get; protected set; }
		#endregion
		public virtual void Execute() {
			BeginExecute();
			try {
				ExecuteCore();
			}
			finally {
				EndExecute();
			}
#if DEBUGTEST
			if (pieceTable == DocumentModel.MainPieceTable)
				DocumentModel.CheckIntegrity();
#endif
		}
		protected internal virtual void BeginExecute() {
			DocumentModel.BeginUpdate();
			this.transaction = new HistoryTransaction(DocumentModel.History);
			CalculateExecutionParameters();
			CalculateApplyChangesParameters();
		}
		protected internal virtual void EndExecute() {
			ApplyChanges();
			this.transaction.Dispose();
			DocumentModel.EndUpdate();
		}
		protected internal abstract void ExecuteCore();
		protected internal abstract void CalculateExecutionParameters();
		protected internal abstract void CalculateApplyChangesParameters();
		protected internal abstract void ApplyChanges();
	}
	#endregion
	#region PieceTableInsertObjectCommand (abstract class)
	public abstract class PieceTableInsertObjectCommand : PieceTableCommand {
		#region Fields
		ParagraphIndex paragraphIndex;
		ParagraphIndex applyChangesParagraphIndex;
		#endregion
		protected PieceTableInsertObjectCommand(PieceTable pieceTable)
			: base(pieceTable) {
		}
		#region Properties
		public ParagraphIndex ParagraphIndex { get { return paragraphIndex; } }
		protected internal abstract DocumentModelChangeType ChangeType { get; }
		#endregion
		protected internal override void CalculateExecutionParameters() {
			this.paragraphIndex = CalculateInsertionParagraphIndex();
		}
		protected internal override void CalculateApplyChangesParameters() {
			SectionIndex sectionIndex = PieceTable.LookupSectionIndexByParagraphIndex(ParagraphIndex);
			if (sectionIndex > new SectionIndex(0)) {
				Section section = DocumentModel.Sections[sectionIndex];
				if (section.LastParagraphIndex == ParagraphIndex && ParagraphIndex > ParagraphIndex.Zero)
					applyChangesParagraphIndex = ParagraphIndex - 1;
				else
					applyChangesParagraphIndex = ParagraphIndex;
			}
			else
				applyChangesParagraphIndex = ParagraphIndex;
			Paragraph firstParagraph = PieceTable.Paragraphs[applyChangesParagraphIndex];
			Paragraph lastParagraph = PieceTable.Paragraphs[ParagraphIndex];
			PieceTable.ApplyChanges(ChangeType, firstParagraph.FirstRunIndex, lastParagraph.LastRunIndex + 1); 
		}
		protected internal override void ApplyChanges() {
			Paragraph firstParagraph = PieceTable.Paragraphs[applyChangesParagraphIndex];
			Paragraph lastParagraph = PieceTable.Paragraphs[ParagraphIndex];
			PieceTable.ApplyChanges(ChangeType, firstParagraph.FirstRunIndex, lastParagraph.LastRunIndex);
		}
		protected internal abstract ParagraphIndex CalculateInsertionParagraphIndex();
	}
	#endregion
	#region PieceTableInsertObjectAtLogPositionCommand (abstract class)
	public abstract class PieceTableInsertObjectAtLogPositionCommand : PieceTableInsertObjectCommand {
		readonly DocumentLogPosition logPosition;
		readonly bool forceVisible;
		protected PieceTableInsertObjectAtLogPositionCommand(PieceTable pieceTable, DocumentLogPosition logPosition, bool forceVisible)
			: base(pieceTable) {
			if (logPosition < DocumentLogPosition.Zero)
				Exceptions.ThrowArgumentException("logPosition", logPosition);
			this.logPosition = logPosition;
			this.forceVisible = forceVisible;
		}
		protected bool ForceVisible { get { return forceVisible; } }
		public DocumentLogPosition LogPosition { get { return logPosition; } }
		protected internal override ParagraphIndex CalculateInsertionParagraphIndex() {
			return PieceTable.FindParagraphIndex(LogPosition);
		}
	}
	#endregion
	#region PieceTableInsertObjectAtInputPositionCommand (abstract class)
	public abstract class PieceTableInsertObjectAtInputPositionCommand : PieceTableInsertObjectCommand {
		readonly InputPosition position;
		readonly bool forceVisible;
		protected PieceTableInsertObjectAtInputPositionCommand(PieceTable pieceTable, InputPosition position, bool forceVisible)
			: base(pieceTable) {
			if (position.LogPosition < DocumentLogPosition.Zero)
				Exceptions.ThrowArgumentException("logPosition", position.LogPosition);
			this.position = position;
			this.forceVisible = forceVisible;
		}
		public InputPosition Position { get { return position; } }
		protected bool ForceVisible { get { return forceVisible; } }
		protected internal override ParagraphIndex CalculateInsertionParagraphIndex() {
			return Position.ParagraphIndex;
		}
	}
	#endregion
	#region PieceTableInsertTextAtLogPositionCommand
	public class PieceTableInsertTextAtLogPositionCommand : PieceTableInsertObjectAtLogPositionCommand {
		readonly string text;
		public PieceTableInsertTextAtLogPositionCommand(PieceTable pieceTable, DocumentLogPosition logPosition, string text, bool forceVisible)
			: base(pieceTable, logPosition, forceVisible) {
			if (String.IsNullOrEmpty(text))
				Exceptions.ThrowArgumentException("text", text);
			this.text = text;
		}
		public string Text { get { return text; } }
		protected internal override DocumentModelChangeType ChangeType { get { return DocumentModelChangeType.InsertText; } }
		protected internal override void ExecuteCore() {
			PieceTable.InsertTextCore(ParagraphIndex, LogPosition, text, ForceVisible);
		}
	}
	#endregion
	#region PieceTableInsertPlainTextAtLogPositionCommand
	public class PieceTableInsertPlainTextAtLogPositionCommand : PieceTableInsertTextAtLogPositionCommand {
		public PieceTableInsertPlainTextAtLogPositionCommand(PieceTable pieceTable, DocumentLogPosition logPosition, string text, bool forceVisible)
			: base(pieceTable, logPosition, text, forceVisible) {
		}
		protected internal override void ExecuteCore() {
			InsertPlainTextCore(LogPosition, Text);
		}
		protected internal virtual void InsertPlainTextCore(DocumentLogPosition position, string text) {
			int count = text.Length;
			int index = 0;
			TextRunBase runFormattingSource = null;
			while (index < count) {
				string line = GetNextLine(text, ref index);
				if (!String.IsNullOrEmpty(line)) {
					PieceTable.InsertText(position, line, ForceVisible);
					TextRunBase lastInsertedRun = PieceTable.LastInsertedRunInfo.Run;
					if (runFormattingSource != null)
						lastInsertedRun.ApplyFormatting(runFormattingSource.CharacterProperties.Info.Info, runFormattingSource.CharacterProperties.Info.Options, runFormattingSource.CharacterStyleIndex, ForceVisible);
					else
						runFormattingSource = lastInsertedRun;
					position += line.Length;
				}
				if (ShouldAddParagraph(text, ref index)) {
					PieceTable.InsertParagraph(position, ForceVisible);
					position++;
				}
			}
		}
		protected internal static string GetNextLine(string source, ref int index) {
			StringBuilder result = new StringBuilder();
			int count = source.Length;
			while (index < count && source[index] != '\r' && source[index] != '\n') {
				result.Append(source[index]);
				index++;
			}
			return result.ToString();
		}
		protected internal static bool ShouldAddParagraph(string source, ref int index) {
			int count = source.Length;
			if (index < count) {
				if (source[index] == '\r')
					index++;
				if (index < count && source[index] == '\n')
					index++;
				return true;
			}
			else
				return false;
		}
	}
	#endregion
	#region PieceTableInsertTextAtInputPositionCommand
	public class PieceTableInsertTextAtInputPositionCommand : PieceTableInsertObjectAtInputPositionCommand {
		readonly string text;
		public PieceTableInsertTextAtInputPositionCommand(PieceTable pieceTable, InputPosition inputPosition, string text, bool forceVisible)
			: base(pieceTable, inputPosition, forceVisible) {
			if (String.IsNullOrEmpty(text))
				Exceptions.ThrowArgumentException("text", text);
			this.text = text;
		}
		public string Text { get { return text; } }
		protected internal override DocumentModelChangeType ChangeType { get { return DocumentModelChangeType.InsertText; } }
		protected internal override void ExecuteCore() {
			PieceTable.InsertTextCore(Position, text, ForceVisible);
		}
	}
	#endregion
	#region PieceTableInsertPlainTextAtInputPositionCommand
	public class PieceTableInsertPlainTextAtInputPositionCommand : PieceTableInsertObjectAtInputPositionCommand {
		readonly string text;
		public PieceTableInsertPlainTextAtInputPositionCommand(PieceTable pieceTable, InputPosition inputPosition, string text, bool forceVisible)
			: base(pieceTable, inputPosition, forceVisible) {
			if (String.IsNullOrEmpty(text))
				Exceptions.ThrowArgumentException("text", text);
			this.text = text;
		}
		public string Text { get { return text; } }
		protected internal override DocumentModelChangeType ChangeType { get { return DocumentModelChangeType.InsertText; } }
		protected internal override void ExecuteCore() {
			InsertPlainTextCore(Position, Text);
		}
		protected internal virtual void InsertPlainTextCore(InputPosition position, string text) {
			int count = text.Length;
			int index = 0;
			while (index < count) {
				string line = PieceTableInsertPlainTextAtLogPositionCommand.GetNextLine(text, ref index);
				if (!String.IsNullOrEmpty(line))
					PieceTable.InsertTextCore(position, line);
				if (PieceTableInsertPlainTextAtLogPositionCommand.ShouldAddParagraph(text, ref index))
					PieceTable.InsertParagraphCore(position);
			}
		}
	}
	#endregion
	#region PieceTableInsertParagraphAtLogPositionCommand
	public class PieceTableInsertParagraphAtLogPositionCommand : PieceTableInsertObjectAtLogPositionCommand {
		public PieceTableInsertParagraphAtLogPositionCommand(PieceTable pieceTable, DocumentLogPosition logPosition, bool forceVisible)
			: base(pieceTable, logPosition, forceVisible) {
		}
		protected internal override DocumentModelChangeType ChangeType { get { return DocumentModelChangeType.InsertParagraph; } }
		protected internal override void ExecuteCore() {
			PieceTable.InsertParagraphCore(ParagraphIndex, LogPosition, ForceVisible);
			PieceTable.ApplyNumberingToInsertedParagraph(ParagraphIndex);
		}
	}
	#endregion
	#region PieceTableInsertParagraphAtLogPositionCommand
	public class PieceTableInsertParagraphAtInputPositionCommand : PieceTableInsertObjectAtLogPositionCommand {
		InputPosition inputPosition;
		public PieceTableInsertParagraphAtInputPositionCommand(PieceTable pieceTable, DocumentLogPosition logPosition, bool forceVisible, InputPosition inputPosition)
			: base(pieceTable, logPosition, forceVisible) {
			this.inputPosition = inputPosition;
		}
		protected internal override DocumentModelChangeType ChangeType { get { return DocumentModelChangeType.InsertParagraph; } }
		protected internal override void ExecuteCore() {
			RunIndex runIndex = PieceTable.InsertParagraphCore(ParagraphIndex, LogPosition, ForceVisible);
			PieceTable.Runs[runIndex].ApplyFormatting(inputPosition);
			PieceTable.ApplyNumberingToInsertedParagraph(ParagraphIndex);
		}
	}
	#endregion
	#region PieceTableInsertInlinePictureAtLogPositionCommand
	public class PieceTableInsertInlinePictureAtLogPositionCommand : PieceTableInsertObjectAtLogPositionCommand {
		#region Fields
		readonly OfficeImage image;
		readonly int scaleX;
		readonly int scaleY;
		#endregion
		public PieceTableInsertInlinePictureAtLogPositionCommand(PieceTable pieceTable, DocumentLogPosition logPosition, OfficeImage image, int scaleX, int scaleY, bool forceVisible)
			: base(pieceTable, logPosition, forceVisible) {
			Guard.ArgumentNotNull(image, "image");
			Guard.ArgumentPositive(scaleX, "scaleX");
			Guard.ArgumentPositive(scaleY, "scaleY");
			this.image = image;
			this.scaleX = scaleX;
			this.scaleY = scaleY;
		}
		#region Properties
		public OfficeImage Image { get { return image; } }
		public int ScaleX { get { return scaleX; } }
		public int ScaleY { get { return scaleY; } }
		protected internal override DocumentModelChangeType ChangeType { get { return DocumentModelChangeType.InsertInlinePicture; } }
		#endregion
		protected internal override void ExecuteCore() {
			Result = PieceTable.InsertInlineImageCore(ParagraphIndex, LogPosition, Image, ScaleX, ScaleY, false, ForceVisible);
		}
	}
	#endregion
	#region PieceTableInsertCustomRunAtLogPositionCommand
	public class PieceTableInsertCustomRunAtLogPositionCommand : PieceTableInsertObjectAtLogPositionCommand {
		#region Fields
		readonly ICustomRunObject customRunObject;
		#endregion
		public PieceTableInsertCustomRunAtLogPositionCommand(PieceTable pieceTable, DocumentLogPosition logPosition, ICustomRunObject customRunObject, bool forceVisible)
			: base(pieceTable, logPosition, forceVisible) {
			Guard.ArgumentNotNull(customRunObject, "customRunObject");
			this.customRunObject = customRunObject;
		}
		#region Properties
		public ICustomRunObject CustomRunObject { get { return customRunObject; } }
		protected internal override DocumentModelChangeType ChangeType { get { return DocumentModelChangeType.InsertInlineCustomObject; } }
		#endregion
		protected internal override void ExecuteCore() {
			PieceTable.InsertCustomRunCore(ParagraphIndex, LogPosition, CustomRunObject, ForceVisible);
		}
	}
	#endregion
	#region PieceTableInsertInlineCustomObjectAtLogPositionCommand
	public class PieceTableInsertInlineCustomObjectAtLogPositionCommand : PieceTableInsertObjectAtLogPositionCommand {
		#region Fields
		readonly IInlineCustomObject customObject;
		readonly int scaleX;
		readonly int scaleY;
		#endregion
		public PieceTableInsertInlineCustomObjectAtLogPositionCommand(PieceTable pieceTable, DocumentLogPosition logPosition, IInlineCustomObject customObject, int scaleX, int scaleY, bool forceVisible)
			: base(pieceTable, logPosition, forceVisible) {
			Guard.ArgumentNotNull(customObject, "customObject");
			Guard.ArgumentPositive(scaleX, "scaleX");
			Guard.ArgumentPositive(scaleY, "scaleY");
			this.customObject = customObject;
			this.scaleX = scaleX;
			this.scaleY = scaleY;
		}
		#region Properties
		public IInlineCustomObject CustomObject { get { return customObject; } }
		public int ScaleX { get { return scaleX; } }
		public int ScaleY { get { return scaleY; } }
		protected internal override DocumentModelChangeType ChangeType { get { return DocumentModelChangeType.InsertInlineCustomObject; } }
		#endregion
		protected internal override void ExecuteCore() {
			PieceTable.InsertInlineCustomObjectCore(ParagraphIndex, LogPosition, CustomObject, ScaleX, ScaleY, ForceVisible);
		}
	}
	#endregion
	#region PieceTableInsertFloatingObjectAnchorAtLogPositionCommand
	public class PieceTableInsertFloatingObjectAnchorAtLogPositionCommand : PieceTableInsertObjectAtLogPositionCommand {
		public PieceTableInsertFloatingObjectAnchorAtLogPositionCommand(PieceTable pieceTable, DocumentLogPosition logPosition, bool forceVisible)
			: base(pieceTable, logPosition, forceVisible) {
		}
		#region Properties
		protected internal override DocumentModelChangeType ChangeType { get { return DocumentModelChangeType.InsertFloatingObjectAnchor; } }
		#endregion
		protected internal override void ExecuteCore() {
			PieceTable.InsertFloatingObjectAnchorCore(ParagraphIndex, LogPosition, ForceVisible);
		}
	}
	#endregion
	#region PieceTableInsertContentConvertedToDocumentModelCommand
	public class PieceTableInsertContentConvertedToDocumentModelCommand : PieceTableInsertObjectAtLogPositionCommand {
		#region Fields
		readonly DocumentModel sourceModel;
		bool isMergingTableCell;
		InsertOptions insertOptions;
		bool suppressParentFieldsUpdate;
		bool suppressFieldsUpdate;
		bool copyLastParagraph;
		bool pasteFromIE;
		#endregion
		public PieceTableInsertContentConvertedToDocumentModelCommand(PieceTable targetPieceTable, DocumentModel sourceModel, DocumentLogPosition logPosition, InsertOptions insertOptions, bool forceVisible)
			: base(targetPieceTable, logPosition, forceVisible) {
			Guard.ArgumentNotNull(sourceModel, "sourceModel");
			this.sourceModel = sourceModel;
			this.insertOptions = insertOptions;
		}
		public PieceTableInsertContentConvertedToDocumentModelCommand(PieceTable targetPieceTable, DocumentModel sourceModel, DocumentLogPosition logPosition, bool forceVisible)
			: this(targetPieceTable, sourceModel, logPosition, InsertOptions.MatchDestinationFormatting, forceVisible) {
		}
		#region Properties
		protected internal override DocumentModelChangeType ChangeType { get { return DocumentModelChangeType.None; } }
		protected DocumentModel SourceModel { get { return sourceModel; } }
		protected internal bool IsMergingTableCell { get { return isMergingTableCell; } set { isMergingTableCell = value; } }
		public InsertOptions InsertOptions { get { return insertOptions; } }
		public bool SuppressParentFieldsUpdate { get { return suppressParentFieldsUpdate; } set { suppressParentFieldsUpdate = value; } }
		public bool SuppressFieldsUpdate { get { return suppressFieldsUpdate; } set { suppressFieldsUpdate = value; } }
		public bool CopyBetweenInternalModels { get; set; }
		public bool CopyLastParagraph { get { return copyLastParagraph; } set { copyLastParagraph = value; } }
		public bool PasteFromIE { get { return pasteFromIE; } set { pasteFromIE = value; } }
		public bool SuppressCopySectionProperties { get; set; }
		public bool RemoveLeadingPageBreak { get; set; } 
		#endregion
		protected internal override void ExecuteCore() {
			int length = GetDocumentLength(SourceModel);
			SourceModel.Selection.Start = DocumentLogPosition.Zero;
			if (SourceModel.MailMergeOptions.KeepLastParagraph)
				copyLastParagraph = true;
			SourceModel.Selection.End = new DocumentLogPosition(length - (copyLastParagraph ? 0 : 1));
			PieceTable targetPieceTable = PieceTable;
			DocumentModelCopyManager copyManager = targetPieceTable.GetCopyManager(SourceModel.MainPieceTable, InsertOptions);
			copyManager.IsInserted = true;
			copyManager.TargetPosition.LogPosition = LogPosition;
			copyManager.TargetPosition.ParagraphIndex = targetPieceTable.FindParagraphIndex(LogPosition);
			RunIndex runIndex;
			DocumentLogPosition rangeStartLogPosition = targetPieceTable.FindRunStartLogPosition(targetPieceTable.Paragraphs[copyManager.TargetPosition.ParagraphIndex], LogPosition, out runIndex);
			if (rangeStartLogPosition != LogPosition) {
				targetPieceTable.SplitTextRun(copyManager.TargetPosition.ParagraphIndex, runIndex, LogPosition - rangeStartLogPosition);
				rangeStartLogPosition = LogPosition;
				runIndex++;
			}
			copyManager.TargetPosition.RunStartLogPosition = rangeStartLogPosition;
			copyManager.TargetPosition.RunIndex = runIndex;
			copyManager.CopyAdditionalInfo(CopyBetweenInternalModels);
			CopyDocumentModelContent(copyManager, length);
		}
		void CopyDocumentModelContent(DocumentModelCopyManager copyManager, int length) {
			copyManager.TableCopyHelper.TargetStartParagraphIndex = copyManager.TargetPosition.ParagraphIndex;
			copyManager.TableCopyHelper.SuppressCopyTables = true;
			CopySectionOperation operation = SourceModel.CreateCopySectionOperation(copyManager);
			operation.SuppressParentFieldsUpdate = SuppressParentFieldsUpdate;
			operation.SuppressFieldsUpdate = SuppressFieldsUpdate;
			operation.IsMergingTableCell = IsMergingTableCell;
			operation.SuppressCopySectionProperties = SuppressCopySectionProperties;
			operation.RemoveLeadingPageBreak = RemoveLeadingPageBreak;
			if (pasteFromIE)
				operation.UpdateFieldOperationType = UpdateFieldOperationType.PasteFromIE;
			operation.Execute(DocumentLogPosition.Zero, length - (copyLastParagraph ? 0 : 1), true);
		}
		internal int GetDocumentLength(DocumentModel documentModel) {
			Paragraph lastParagraph = documentModel.MainPieceTable.Paragraphs.Last;
			return lastParagraph.LogPosition + lastParagraph.Length - DocumentLogPosition.Zero;
		}
	}
	public class DocumentModelCopyOptions {
		ParagraphNumerationCopyOptions paragraphNumerationCopyOptions = ParagraphNumerationCopyOptions.CopyAlways;
		DefaultPropertiesCopyOptions defaultPropertiesCopyOptions = DefaultPropertiesCopyOptions.Never;
		FormattingCopyOptions formattingCopyOptions = FormattingCopyOptions.UseDestinationStyles;
		SelectionRangeCollection selectionRanges;
		bool copyDocumentVariables;
		public DocumentModelCopyOptions(DocumentLogPosition from, int length) {
			this.selectionRanges = new SelectionRangeCollection();
			selectionRanges.Add(new SelectionRange(from, length));
		}
		public DocumentModelCopyOptions(SelectionRangeCollection selectionRanges) {
			Guard.ArgumentNotNull(selectionRanges, "SelectionRangeCollection");
			if (selectionRanges.Count == 0)
				Exceptions.ThrowArgumentException("SelectionRanges empty", selectionRanges);
			this.selectionRanges = selectionRanges;
		}
		public SelectionRangeCollection SelectionRanges { get { return selectionRanges; } }
		public DefaultPropertiesCopyOptions DefaultPropertiesCopyOptions { get { return defaultPropertiesCopyOptions; } set { defaultPropertiesCopyOptions = value; } }
		public bool CopyDocumentVariables { get { return copyDocumentVariables; } set { copyDocumentVariables = value; } }
		public ParagraphNumerationCopyOptions ParagraphNumerationCopyOptions {
			get { return paragraphNumerationCopyOptions; }
			set { paragraphNumerationCopyOptions = value; }
		}
		public FormattingCopyOptions FormattingCopyOptions { get { return formattingCopyOptions; } set { formattingCopyOptions = value; } }
		public DocumentLogPosition From {
			get { return selectionRanges[0].From; }
			set { selectionRanges[0].From = value; }
		}
		public int Length {
			get { return selectionRanges[0].Length; }
			set { selectionRanges[0].Length = value; }
		}
	}
	#region DocumentModelCopyCommand
	public class DocumentModelCopyCommand : PieceTableCommand {
		readonly DocumentModel targetModel;
		readonly DocumentModelCopyOptions options;
		bool fixLastParagraph;
		int tableCopyFromNestedLevel = -1;
		bool allowCopyWholeFieldResult;
		bool suppressFieldsUpdate;
		public DocumentModelCopyCommand(PieceTable sourcePieceTable, DocumentModel targetModel, DocumentModelCopyOptions options)
			: base(sourcePieceTable) {
			Guard.ArgumentNotNull(targetModel, "targetModel");
			Guard.ArgumentNotNull(options, "options");
			this.targetModel = targetModel;
			this.options = options;
			UpdateFieldOperationType = Model.UpdateFieldOperationType.Copy;
		}
		protected DocumentModel TargetModel { get { return targetModel; } }
		public bool FixLastParagraph { get { return fixLastParagraph; } set { fixLastParagraph = value; } }
		public int TableCopyFromNestedLevel { get { return tableCopyFromNestedLevel; } set { tableCopyFromNestedLevel = value; } }
		public bool AllowCopyWholeFieldResult { get { return allowCopyWholeFieldResult; } set { allowCopyWholeFieldResult = value; } }
		public bool SuppressFieldsUpdate { get { return suppressFieldsUpdate; } set { suppressFieldsUpdate = value; } }
		public bool RemoveLeadingPageBreak { get; set; } 
		public UpdateFieldOperationType UpdateFieldOperationType { get; set; }
		public bool UpdateIntervals { get; set; }
		protected internal override void ExecuteCore() {
			if (options.DefaultPropertiesCopyOptions == DefaultPropertiesCopyOptions.Always)
				ReplaceDefaultProperties(TargetModel, DocumentModel);
			ReplaceDefaultStyles(TargetModel, DocumentModel);
			if (options.CopyDocumentVariables)
				CopyDocumentVariables(TargetModel, DocumentModel);
			TargetModel.DeleteDefaultNumberingList(TargetModel.NumberingLists);
			DocumentModelCopyManager copyManager = new DocumentModelCopyManager(PieceTable, TargetModel.MainPieceTable, options.ParagraphNumerationCopyOptions, options.FormattingCopyOptions);
			copyManager.TableCopyHelper.CopyFromNestedLevel = TableCopyFromNestedLevel;
			CopySectionOperation operation = DocumentModel.CreateCopySectionOperation(copyManager);
			operation.ShouldCopyBookmarks = false;			
			operation.SuppressFieldsUpdate = SuppressFieldsUpdate;
			operation.AllowCopyWholeFieldResult = AllowCopyWholeFieldResult;
			operation.RemoveLeadingPageBreak = RemoveLeadingPageBreak;
			operation.UpdateFieldOperationType = UpdateFieldOperationType;
			SelectionRangeCollection selectionRanges = options.SelectionRanges;
			List<DocumentModelPosition> targetBookmarksPositions = new List<DocumentModelPosition>();
			TargetModel.ActivePieceTable.SuppressTableIntegrityCheck = true;
			for (int i = 0; i < selectionRanges.Count; i++) {
				SelectionRange currentRange = selectionRanges[i];
				targetBookmarksPositions.Add(copyManager.TargetPosition.Clone());
				operation.Execute(currentRange.From, currentRange.Length, false);
			}
			NormalizeTables(TargetModel.ActivePieceTable);
			TargetModel.ActivePieceTable.SuppressTableIntegrityCheck = false;
			for (int i = 0; i < selectionRanges.Count; i++) {
				SelectionRange currentRange = selectionRanges[i];
				CopyBookmarksOperation copyBookmarksOperation = CreateCopyBookmarkOperation(copyManager);
				copyBookmarksOperation.TargetBookmarksPosition = targetBookmarksPositions[i];
				copyBookmarksOperation.Execute(currentRange.From, currentRange.Length, false);
			}
			if (UpdateIntervals) {
				foreach (PieceTable pieceTable in TargetModel.GetPieceTables(true))
					pieceTable.UpdateIntervals();
			}
			bool oldValue = TargetModel.ForceNotifyStructureChanged;
			TargetModel.ForceNotifyStructureChanged = true;
			try {
				operation.AfterBookmarkCopied();
			}
			finally {
				TargetModel.ForceNotifyStructureChanged = oldValue;
			}
			if (FixLastParagraph)
				TargetModel.MainPieceTable.FixLastParagraph();
		}
		protected virtual CopyBookmarksOperation CreateCopyBookmarkOperation(DocumentModelCopyManager copyManager) {
			return new CopyBookmarksOperation(copyManager);
		}
		void NormalizeTables(PieceTable pieceTable) {
			TableCollection tables = pieceTable.Tables;
			int count = tables.Count;
			pieceTable.DocumentModel.BeginUpdate();
			try {
				for (int i = 0; i < count; i++) {
					Table currentTable = tables[i];
					currentTable.Normalize();
					currentTable.NormalizeCellColumnSpans();
					currentTable.NormalizeTableCellVerticalMerging();
					int maxCells = currentTable.FindTotalColumnsCountInTable();
					int rowsCount = currentTable.Rows.Count;
					for (int rowIndex = 0; rowIndex < rowsCount; rowIndex++) {
						TableRow row = currentTable.Rows[rowIndex];
						int cells = currentTable.GetTotalCellsInRowConsiderGrid(row);
						if (cells < maxCells) {
							int newCellsCount = maxCells - cells;
							row.LastCell.ColumnSpan += newCellsCount;
						}
					}
				}
			}
			finally {
				pieceTable.DocumentModel.EndUpdate();
			}
		}
		protected internal static void ReplaceDefaultProperties(DocumentModel targetModel, DocumentModel sourceModel) {
			targetModel.DefaultCharacterProperties.Info.CopyFrom(sourceModel.DefaultCharacterProperties.Info);
			targetModel.DefaultParagraphProperties.Info.CopyFrom(sourceModel.DefaultParagraphProperties.Info);
			targetModel.DefaultTableCellProperties.Info.CopyFrom(sourceModel.DefaultTableCellProperties.Info);
			targetModel.DefaultTableProperties.Info.CopyFrom(sourceModel.DefaultTableProperties.Info);
			targetModel.DefaultTableRowProperties.Info.CopyFrom(sourceModel.DefaultTableRowProperties.Info);
			targetModel.DocumentProperties.Info.CopyFrom(sourceModel.DocumentProperties.Info);
		}
		protected internal static void ReplaceDefaultStyles(DocumentModel targetModel, DocumentModel sourceModel) {
			ReplaceStylesCore(targetModel.CharacterStyles, sourceModel.CharacterStyles);
			ReplaceStylesCore(targetModel.ParagraphStyles, sourceModel.ParagraphStyles);
			ReplaceStylesCore(targetModel.TableStyles, sourceModel.TableStyles);
			ReplaceStylesCore(targetModel.TableCellStyles, sourceModel.TableCellStyles);
			ReplaceStylesCore(targetModel.NumberingListStyles, sourceModel.NumberingListStyles);
		}
		protected internal static void CopyStyles(DocumentModel targetModel, DocumentModel sourceModel) {
			CopyStyles(targetModel, sourceModel, false);
		}
		protected internal static void CopyStyles(DocumentModel targetModel, DocumentModel sourceModel, bool withId) {
			targetModel.BeginUpdate();
			CopyStylesCore(targetModel.CharacterStyles, sourceModel.CharacterStyles, withId);
			CopyStylesCore(targetModel.ParagraphStyles, sourceModel.ParagraphStyles, withId);
			CopyStylesCore(targetModel.TableStyles, sourceModel.TableStyles, withId);
			CopyStylesCore(targetModel.TableCellStyles, sourceModel.TableCellStyles, withId);
			CopyStylesCore(targetModel.NumberingListStyles, sourceModel.NumberingListStyles, withId);
			targetModel.EndUpdate();
		}
		void CopyDocumentVariables(DocumentModel targetModel, DocumentModel sourceModel) {
			Dictionary<string, object> sourceVariables = sourceModel.Variables.Items;
			DocumentVariableCollection targetVariables = targetModel.Variables;
			foreach (string key in sourceVariables.Keys)
				targetVariables.Add(key, sourceVariables[key]);
		}
		protected internal static void ReplaceStylesCore<T>(StyleCollectionBase<T> targetStyles, StyleCollectionBase<T> sourceStyles) where T : StyleBase<T> {
			int count = targetStyles.Count;
			for (int i = 0; i < count; i++) {
				T targetStyle = targetStyles[i];
				T sourceStyle = sourceStyles.GetStyleByName(targetStyle.StyleName);
				if (sourceStyle != null)
					targetStyle.CopyProperties(sourceStyle);
			}
		}
		protected internal static void CopyStylesCore<T>(StyleCollectionBase<T> targetStyles, StyleCollectionBase<T> sourceStyles, bool withId) where T : StyleBase<T> {
			foreach(T sourceStyle in sourceStyles) {
				string name = sourceStyle.StyleName;
				T targetStyle = targetStyles.GetStyleByName(name);
				if(targetStyle != null)
					targetStyle.CopyProperties(sourceStyle);
				else {
					int styleIndex = sourceStyle.Copy(targetStyles.DocumentModel);
					targetStyle = targetStyles[styleIndex];
				}
				if (withId)
					targetStyle.SetId(sourceStyle.Id);
			}
		}
		protected internal override void CalculateExecutionParameters() {
		}
		protected internal override void ApplyChanges() {
		}
		protected internal override void CalculateApplyChangesParameters() {
		}
	}
	#endregion
	#endregion
	#region PieceTableCreateFieldCommand
	public class PieceTableCreateFieldCommand : PieceTableInsertObjectAtLogPositionCommand {
		#region Fields
		readonly int length;
		Field insertedField;
		#endregion
		public PieceTableCreateFieldCommand(PieceTable pieceTable, DocumentLogPosition logPosition, int length, bool forceVisible)
			: base(pieceTable, logPosition, forceVisible) {
			Guard.ArgumentNonNegative(length, "length");
			this.length = length;
		}
		#region Properties
		public int Length { get { return length; } }
		public Field InsertedField { get { return insertedField; } }
		protected internal override DocumentModelChangeType ChangeType { get { return DocumentModelChangeType.InsertText; } }
		#endregion
		protected internal override void ExecuteCore() {
			DocumentLogPosition endLogPosition = Algorithms.Min(LogPosition + Length, PieceTable.DocumentEndLogPosition);
			ParagraphIndex endParagraphIndex = PieceTable.FindParagraphIndex(endLogPosition);
			DocumentModel.History.BeginTransaction();
			try {
				AddFieldHistoryItem item = new AddFieldHistoryItem(PieceTable);
				item.CodeStartRunIndex = PieceTable.InsertFieldCodeStartRunCore(ParagraphIndex, LogPosition, ForceVisible);
				item.CodeEndRunIndex = PieceTable.InsertFieldCodeEndRunCore(endParagraphIndex, endLogPosition + 1, ForceVisible);
				item.ResultEndRunIndex = PieceTable.InsertFieldResultEndRunCore(endParagraphIndex, endLogPosition + 2, ForceVisible);
				DocumentModel.History.Add(item);
				item.Execute();
				this.insertedField = PieceTable.Fields[item.InsertedFieldIndex];
			}
			finally {
				DocumentModel.History.EndTransaction();
			}
		}
	}
	#endregion
	#region PieceTableCreateFieldWithResultCommand
	public class PieceTableCreateFieldWithResultCommand : PieceTableInsertObjectCommand {
		#region Fields
		readonly DocumentLogPosition startCode;
		readonly DocumentLogPosition endCode;
		readonly int resultLength;
		Field insertedField;
		bool forceVisible;
		#endregion
		public PieceTableCreateFieldWithResultCommand(PieceTable pieceTable, DocumentLogPosition startCode, DocumentLogPosition endCode, int resultLength, bool forceVisible)
			: base(pieceTable) {
			this.startCode = startCode;
			this.endCode = endCode;
			this.resultLength = resultLength;
			this.forceVisible = forceVisible;
		}
		public Field InsertedField { get { return insertedField; } }
		protected internal override DocumentModelChangeType ChangeType { get { return DocumentModelChangeType.InsertText; } }
		protected internal override ParagraphIndex CalculateInsertionParagraphIndex() {
			return PieceTable.FindParagraphIndex(startCode);
		}
		protected internal override void ExecuteCore() {
			DocumentModel.History.BeginTransaction();
			try {
				AddFieldHistoryItem item = new AddFieldHistoryItem(PieceTable);
				item.CodeStartRunIndex = InsertStartCodeRun();
				DocumentLogPosition endCodePosition = Algorithms.Min(endCode + 1, PieceTable.DocumentEndLogPosition);
				item.CodeEndRunIndex = InsertEndCodeRun(endCodePosition);
				DocumentLogPosition endResultPosition = Algorithms.Min(endCodePosition + resultLength + 1, PieceTable.DocumentEndLogPosition);
				item.ResultEndRunIndex = InsertEndResultRun(endResultPosition);
				DocumentModel.History.Add(item);
				item.Execute();
				this.insertedField = PieceTable.Fields[item.InsertedFieldIndex];
			}
			finally {
				DocumentModel.History.EndTransaction();
			}
		}
		protected virtual RunIndex InsertStartCodeRun() {
			ParagraphIndex startCodeParagraphIndex = ParagraphIndex;
			return PieceTable.InsertFieldCodeStartRunCore(startCodeParagraphIndex, startCode, forceVisible);
		}
		protected virtual RunIndex InsertEndCodeRun(DocumentLogPosition endCodePosition) {
			ParagraphIndex endCodeParagraphIndex = PieceTable.FindParagraphIndex(endCodePosition);
			return PieceTable.InsertFieldCodeEndRunCore(endCodeParagraphIndex, endCodePosition, forceVisible);
		}
		protected virtual RunIndex InsertEndResultRun(DocumentLogPosition endResultPosition) {
			ParagraphIndex endResultParagraphIndex = PieceTable.FindParagraphIndex(endResultPosition);
			return PieceTable.InsertFieldResultEndRunCore(endResultParagraphIndex, endResultPosition, forceVisible);
		}
	}
	#endregion
	#region PieceTableInsertSeparatorAtLogPositionCommand
	public class PieceTableInsertSeparatorAtLogPositionCommand : PieceTableInsertObjectAtLogPositionCommand {
		public PieceTableInsertSeparatorAtLogPositionCommand(PieceTable pieceTable, DocumentLogPosition logPosition, bool forceVisible)
			: base(pieceTable, logPosition, forceVisible) {
		}
		protected internal override DocumentModelChangeType ChangeType { get { return DocumentModelChangeType.InsertText; } }
		protected internal override void ExecuteCore() {
			PieceTable.InsertSeparatorTextRunCore(ParagraphIndex, LogPosition);
		}
	}
	#endregion
	#region PieceTableInserDataContainerAtLogPositiopnCommand
	public class PieceTableInsertDataContainerAtLogPositionCommand : PieceTableInsertObjectAtLogPositionCommand {
		readonly IDataContainer dataContainer;
		public PieceTableInsertDataContainerAtLogPositionCommand(PieceTable pieceTable, DocumentLogPosition logPosition, IDataContainer dataContainer, bool forceVisible)
			: base(pieceTable, logPosition, forceVisible) {
			Guard.ArgumentNotNull(dataContainer, "dataContainer");
			this.dataContainer = dataContainer;
		}
		public IDataContainer DataContainer { get { return dataContainer; } }
		protected internal override DocumentModelChangeType ChangeType {
			get { return DocumentModelChangeType.InsertText; }
		}
		protected internal override void ExecuteCore() {
			PieceTable.InsertDataContainerRunCore(ParagraphIndex, LogPosition, DataContainer, ForceVisible);
		}
	}
	#endregion
	#region PieceTableDeleteFieldWithoutResult
	public class PieceTableDeleteFieldWithoutResultCommand : PieceTableCommand {
		readonly Field field;
		RunIndex runIndex;
		public PieceTableDeleteFieldWithoutResultCommand(PieceTable pieceTable, Field field)
			: base(pieceTable) {
			Guard.ArgumentNotNull(field, "field");
			this.field = field;
		}
		protected internal override void ExecuteCore() {
			PieceTable.RemoveField(field);
			int codeRunsCount = field.Code.End - field.Code.Start + 1;
			DocumentModel.UnsafeEditor.DeleteRuns(PieceTable, field.Code.Start, codeRunsCount);
			DocumentModel.UnsafeEditor.DeleteRuns(PieceTable, field.Result.End - codeRunsCount, 1);
		}
		protected internal override void CalculateExecutionParameters() {
		}
		protected internal override void CalculateApplyChangesParameters() {
			runIndex = field.Code.Start;
		}
		protected internal override void ApplyChanges() {
			PieceTable.ApplyChanges(DocumentModelChangeType.DeleteContent, runIndex, RunIndex.MaxValue);
		}
	}
	#endregion
	#region PieceTableDeleteTextCommand
	public class PieceTableDeleteTextCommand : PieceTableCommand {
		#region Fields
		readonly DocumentLogPosition logPosition;
		readonly int length;
		ParagraphIndex paragraphIndex;
		bool sectionBreakDeleted;
		bool documentLastParagraphSelected;
		bool leaveFieldIfResultIsRemoved;
		bool allowPartiallyDeletingField;
		bool forceRemoveInnerFields;
		bool backspacePressed;
		#endregion
		public PieceTableDeleteTextCommand(PieceTable pieceTable, DocumentLogPosition logPosition, int length)
			: base(pieceTable) {
			if (logPosition < DocumentLogPosition.Zero)
				Exceptions.ThrowArgumentException("logPosition", logPosition);
			Guard.ArgumentNonNegative(length, "length");
			this.logPosition = logPosition;
			this.length = length;
		}
		#region Properties
		public bool DocumentLastParagraphSelected { get { return documentLastParagraphSelected; } set { documentLastParagraphSelected = value; } }
		public bool LeaveFieldIfResultIsRemoved { get { return leaveFieldIfResultIsRemoved; } set { leaveFieldIfResultIsRemoved = value; } }
		public DocumentLogPosition LogPosition { get { return logPosition; } }
		public int Length { get { return length; } }
		public bool AllowPartiallyDeletingField { get { return allowPartiallyDeletingField; } set { allowPartiallyDeletingField = value; } }
		public bool ForceRemoveInnerFields { get { return forceRemoveInnerFields; } set { forceRemoveInnerFields = value; } }
		public bool BackspacePressed { get { return backspacePressed; } set { backspacePressed = value; } }
		#endregion
		protected internal override void CalculateExecutionParameters() {
		}
		protected internal override void ExecuteCore() {
			DeleteContentOperation operation = CreateDeleteContentOperation();
			operation.AllowPartiallyDeletingField = AllowPartiallyDeletingField;
			operation.LeaveFieldIfResultIsRemoved = LeaveFieldIfResultIsRemoved;
			operation.ForceRemoveInnerFields = ForceRemoveInnerFields;
			operation.BackspacePressed = BackspacePressed;
			this.sectionBreakDeleted = operation.Execute(LogPosition, Length, DocumentLastParagraphSelected);
		}
		protected virtual DeleteContentOperation CreateDeleteContentOperation() {
			return PieceTable.CreateDeleteContentOperation();
		}
		protected internal override void CalculateApplyChangesParameters() {
			this.paragraphIndex = PieceTable.FindParagraphIndex(LogPosition);
		}
		protected internal override void ApplyChanges() {
			if (sectionBreakDeleted) {
				SectionIndex sectionIndex = DocumentModel.FindSectionIndex(PieceTable.Paragraphs[paragraphIndex].LogPosition);
				paragraphIndex = DocumentModel.Sections[sectionIndex].FirstParagraphIndex;
			}
			paragraphIndex = Algorithms.Min(paragraphIndex, new ParagraphIndex(PieceTable.Paragraphs.Count - 1));
			RunIndex runIndex = PieceTable.Paragraphs[paragraphIndex].FirstRunIndex;
			PieceTable.ApplyChanges(DocumentModelChangeType.DeleteContent, runIndex, RunIndex.MaxValue);
		}
	}
	#endregion
	#region DocumentModelInsertSectionAtLogPositionCommand
	public class DocumentModelInsertSectionAtLogPositionCommand : DocumentModelInsertObjectAtLogPositionCommand {
		public DocumentModelInsertSectionAtLogPositionCommand(DocumentModel documentModel, DocumentLogPosition logPosition, bool forceVisible)
			: base(documentModel, logPosition, forceVisible) {
		}
		protected internal override DocumentModelChangeType ChangeType { get { return DocumentModelChangeType.InsertSection; } }
		protected internal override void ExecuteCore() {
			if (DocumentModel.DocumentCapabilities.SectionsAllowed)
				DocumentModel.SafeEditor.InsertSectionCore(ParagraphIndex, LogPosition, ForceVisible);
			else
				PieceTable.InsertParagraphCore(ParagraphIndex, LogPosition, ForceVisible);
			DocumentModel.MainPieceTable.ApplyNumberingToInsertedParagraph(ParagraphIndex);
		}
	}
	#endregion
	#region PieceTableTableBaseCommand (abstract class)
	public abstract class PieceTableTableBaseCommand : PieceTableCommand {
		protected PieceTableTableBaseCommand(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected internal override void CalculateExecutionParameters() {
		}
		protected internal override void CalculateApplyChangesParameters() {
		}
		protected internal override void ApplyChanges() {
		}
	}
	#endregion
	#region PieceTableTableDocumentServerOwnerCommand (abstract class)
	public abstract class PieceTableTableDocumentServerOwnerCommand : PieceTableTableBaseCommand {
		#region Fields
		IInnerRichEditDocumentServerOwner server;
		#endregion
		protected PieceTableTableDocumentServerOwnerCommand(PieceTable pieceTable, IInnerRichEditDocumentServerOwner server)
			: base(pieceTable) {
			Guard.ArgumentNotNull(server, "server");
			this.server = server;
		}
		#region Properties
		public IInnerRichEditDocumentServerOwner DocumentServer { get { return server; } }
		#endregion
	}
	#endregion
	#region PieceTableCreateEmptyTableCommand
	public class PieceTableCreateEmptyTableCommand : PieceTableTableBaseCommand {
		#region Fields
		Table insertedTable;
		TableCell sourceCell;
		#endregion
		public PieceTableCreateEmptyTableCommand(PieceTable pieceTable, TableCell sourceCell)
			: base(pieceTable) {
			this.sourceCell = sourceCell;
		}
		public Table NewTable { get { return insertedTable; } }
		protected internal override void ExecuteCore() {
			using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
				CreateEmptyTableHistoryItem item = new CreateEmptyTableHistoryItem(PieceTable, sourceCell);
				DocumentModel.History.Add(item);
				item.Execute();
				this.insertedTable = PieceTable.Tables[item.InsertedTableIndex];
				this.insertedTable.SuppressIntegrityCheck = true;
			}
		}
		public override void Execute() {
			base.Execute();
			this.insertedTable.SuppressIntegrityCheck = false;
		}
	}
	#endregion
	#region PieceTableCreateRowEmptyCommand
	public class PieceTableCreateRowEmptyCommand : PieceTableTableBaseCommand {
		#region Fields
		TableRow insertedTableRow;
		Table table;
		int index;
		bool suppressIntegrityCheck;
		#endregion
		public PieceTableCreateRowEmptyCommand(PieceTable pieceTable, Table table, int index)
			: base(pieceTable) {
			Guard.ArgumentNotNull(table, "table");
			Guard.ArgumentNonNegative(index, "row index");
			this.table = table;
			this.index = index;
		}
		public TableRow InsertedRow { get { return insertedTableRow; } }
		protected internal override void ExecuteCore() {
			using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
				InsertEmptyTableRowHistoryItem item = new InsertEmptyTableRowHistoryItem(PieceTable, table, index);
				DocumentModel.History.Add(item);
				item.Execute();
				this.insertedTableRow = table.Rows[item.InsertedRowIndex];
				this.suppressIntegrityCheck = table.SuppressIntegrityCheck;
				this.table.SuppressIntegrityCheck = true;
			}
		}
		public override void Execute() {
			base.Execute();
			this.table.SuppressIntegrityCheck = suppressIntegrityCheck;
		}
	}
	#endregion
	#region PieceTableCreateCellEmptyCommand
	public class PieceTableCreateCellEmptyCommand : PieceTableTableBaseCommand {
		#region Fields
		TableCell insertedTableCell;
		TableRow row;
		ParagraphIndex startParagraphIndex;
		ParagraphIndex endParagraphIndex;
		int insertedIndex;
		#endregion
		public PieceTableCreateCellEmptyCommand(PieceTable pieceTable, TableRow row, int insertedIndex, ParagraphIndex start, ParagraphIndex end)
			: base(pieceTable) {
			Guard.ArgumentNotNull(row, "tableRow");
			Guard.ArgumentNotNull(start, "start paragraphIndex");
			Guard.ArgumentNotNull(end, "end paragraphIndex");
			Guard.Equals(start >= ParagraphIndex.Zero, true);
			Guard.Equals(end >= ParagraphIndex.Zero, true);
			Guard.Equals(insertedIndex >= 0 && insertedIndex <= row.Cells.Count, true);
			this.row = row;
			this.startParagraphIndex = start;
			this.endParagraphIndex = end;
			this.insertedIndex = insertedIndex;
		}
		#region Properties
		public TableCell InsertedCell { get { return insertedTableCell; } }
		public ParagraphIndex StartParagraphIndex { get { return startParagraphIndex; } }
		public ParagraphIndex EndParagraphIndex { get { return endParagraphIndex; } }
		public TableRow Row { get { return row; } }
		public int InsertedIndex { get { return insertedIndex; } }
		#endregion
		protected internal override void ExecuteCore() {
			using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
				InsertEmptyTableCellHistoryItem item = new InsertEmptyTableCellHistoryItem(PieceTable, Row, InsertedIndex, StartParagraphIndex, EndParagraphIndex);
				DocumentModel.History.Add(item);
				item.Execute();
				this.insertedTableCell = item.InsertedCell;
			}
		}
	}
	#endregion
	#region PieceTableInsertTableRowCommand (abstract class)
	public abstract class PieceTableInsertTableRowCommand : PieceTableCommand {
		#region Fields
		ParagraphIndex newRowStartParagraphIndex;
		DocumentLogPosition positionToParagraphsInsert;
		int newRowIndex;
		readonly TableRow patternRow;
		readonly bool forceVisible;
		#endregion
		protected PieceTableInsertTableRowCommand(PieceTable pieceTable, TableRow patternRow, bool forceVisible)
			: base(pieceTable) {
			Guard.ArgumentNotNull(patternRow, "patternRow");
			this.patternRow = patternRow;
			this.forceVisible = forceVisible;
		}
		#region Properties
		public TableRow PatternRow { get { return patternRow; } }
		protected ParagraphIndex NewRowStartParagraphIndex { get { return newRowStartParagraphIndex; } set { newRowStartParagraphIndex = value; } }
		protected DocumentLogPosition PositionToParagraphsInsert { get { return positionToParagraphsInsert; } set { positionToParagraphsInsert = value; } }
		protected int NewRowIndex { get { return newRowIndex; } set { newRowIndex = value; } }
		#endregion
		protected internal override void ExecuteCore() {
			InsertParagraphs(PositionToParagraphsInsert, patternRow.Cells);
			TableRow row = PieceTable.CreateTableRowCore(patternRow.Table, NewRowIndex);
			row.Properties.CopyFrom(patternRow.Properties);
			int cellCount = patternRow.Cells.Count;
			PieceTable.ConvertParagraphsIntoTableRow(row, NewRowStartParagraphIndex, cellCount);
			for (int i = 0; i < cellCount; i++) {
				TableCell sourceCell = patternRow.Cells[i];
				TableCell targetCell = row.Cells[i];
				targetCell.StyleIndex = sourceCell.StyleIndex;
				CopyPropertiesFromPatternCell(sourceCell, targetCell);
				CopyCharacterAndParagraphFormattingFromPatternCell(sourceCell, targetCell);
				CorrentVerticalMerging(row, i, sourceCell, targetCell);
			}
			PieceTable.CheckIntegrity();
		}
		protected internal abstract void CorrentVerticalMerging(TableRow row, int i, TableCell sourceCell, TableCell targetCell);
		protected internal abstract void CopyPropertiesFromPatternCell(TableCell source, TableCell target);
		protected internal virtual void CopyCharacterAndParagraphFormattingFromPatternCell(TableCell source, TableCell target) {
			Paragraph patternParagraph = PieceTable.Paragraphs[source.StartParagraphIndex];
			Paragraph targetParagraph = PieceTable.Paragraphs[target.StartParagraphIndex];
			patternParagraph.CopyFrom(DocumentModel, targetParagraph);
			RunIndex runIndex = patternParagraph.FirstRunIndex;
			TextRunBase firstRunFromPatternParagraph = PieceTable.Runs[runIndex];
			if (firstRunFromPatternParagraph is SeparatorTextRun)
				firstRunFromPatternParagraph = PieceTable.Runs[runIndex + 1];
			ParagraphRun firstRunFromTargetParagraph = PieceTable.Runs[targetParagraph.FirstRunIndex] as ParagraphRun;
			if (firstRunFromPatternParagraph != null && firstRunFromTargetParagraph != null) {
				DocumentCapabilitiesOptions options = DocumentModel.DocumentCapabilities;
				if (options.CharacterFormattingAllowed)
					firstRunFromTargetParagraph.CharacterProperties.CopyFrom(firstRunFromPatternParagraph.CharacterProperties.Info);
				if (options.CharacterStyleAllowed)
					firstRunFromTargetParagraph.CharacterStyleIndex = firstRunFromPatternParagraph.CharacterStyle.Copy(DocumentModel);
			}
		}
		protected void InsertParagraphs(DocumentLogPosition logPosition, TableCellCollection cells) {
			int count = cells.Count;
			for (int i = 0; i < count; i++)
				PieceTable.InsertParagraph(logPosition, forceVisible);
			for (int i = 0; i < count; i++) {
				Paragraph sourceParagraph = PieceTable.Paragraphs[cells[i].StartParagraphIndex];
				Paragraph targetParagraph = PieceTable.Paragraphs[NewRowStartParagraphIndex + i];
				targetParagraph.InheritStyleAndFormattingFrom(sourceParagraph);
			}
			AfterParagraphsInserted();
		}
		protected internal override void CalculateApplyChangesParameters() {
		}
		protected internal override void ApplyChanges() {
		}
		protected abstract void AfterParagraphsInserted();
		protected abstract TableRow GetNextRow(TableRow row);
	}
	#endregion
	#region PieceTableInsertTableRowBelowCommand
	public class PieceTableInsertTableRowBelowCommand : PieceTableInsertTableRowCommand {
		public PieceTableInsertTableRowBelowCommand(PieceTable pieceTable, TableRow patternRow, bool forceVisible)
			: base(pieceTable, patternRow, forceVisible) {
		}
		protected internal override void CalculateExecutionParameters() {
			ParagraphIndex index = PatternRow.LastCell.EndParagraphIndex;
			Paragraph paragraph = PieceTable.Paragraphs[index];
			PositionToParagraphsInsert = paragraph.LogPosition + paragraph.Length - 1;
			NewRowStartParagraphIndex = index + 1;
			NewRowIndex = PatternRow.Table.Rows.IndexOf(PatternRow) + 1;
		}
		protected override void AfterParagraphsInserted() {
			ParagraphIndex index = PatternRow.LastCell.EndParagraphIndex;
			PieceTable.ChangeCellEndParagraphIndex(PatternRow.LastCell, index - PatternRow.Cells.Count);
		}
		protected override TableRow GetNextRow(TableRow row) {
			return row.Next;
		}
		protected internal override void CorrentVerticalMerging(TableRow createdRow, int i, TableCell sourceCell, TableCell targetCell) {
			if (sourceCell.VerticalMerging == MergingState.Continue) {
				TableRow nextRow = GetNextRow(createdRow);
				if (nextRow == null) 
					targetCell.VerticalMerging = MergingState.None;
				else {
					int sourceCellStartColumnIndex = sourceCell.GetStartColumnIndexConsiderRowGrid();
					int indexInNextRow = nextRow.Table.GetAbsoluteCellIndexInRow(nextRow, sourceCellStartColumnIndex, false);
					if (nextRow.Cells[indexInNextRow].VerticalMerging != MergingState.Continue)
						targetCell.VerticalMerging = MergingState.None;
				}
			}
			else if (sourceCell.VerticalMerging == MergingState.Restart) {
				targetCell.VerticalMerging = MergingState.Continue;
			}
		}
		protected internal override void CopyPropertiesFromPatternCell(TableCell sourceCell, TableCell targetCell) {
			TableCell patternCell = sourceCell;
			if (sourceCell.VerticalMerging == MergingState.Continue) {
				TableCell firstCellInVerticalMergeGroup = sourceCell.Table.GetFirstCellInVerticalMergingGroup(sourceCell);
				if (firstCellInVerticalMergeGroup != null)
					patternCell = firstCellInVerticalMergeGroup;
			}
			targetCell.Properties.CopyFrom(patternCell.Properties);
		}
	}
	#endregion
	#region PieceTableInsertTableRowAboveCommand
	public class PieceTableInsertTableRowAboveCommand : PieceTableInsertTableRowCommand {
		public PieceTableInsertTableRowAboveCommand(PieceTable pieceTable, TableRow patternRow, bool forceVisible)
			: base(pieceTable, patternRow, forceVisible) {
		}
		protected internal override void CalculateExecutionParameters() {
			ParagraphIndex index = PatternRow.FirstCell.StartParagraphIndex;
			Paragraph paragraph = PieceTable.Paragraphs[index];
			PositionToParagraphsInsert = paragraph.LogPosition;
			NewRowStartParagraphIndex = index;
			NewRowIndex = PatternRow.Table.Rows.IndexOf(PatternRow);
		}
		protected override void AfterParagraphsInserted() {
			ParagraphIndex index = PatternRow.FirstCell.StartParagraphIndex;
			TableCell mostNestedCell = PieceTable.Paragraphs[index].GetCell();
			PieceTable.ChangeCellStartParagraphIndex(PatternRow.FirstCell, index + PatternRow.Cells.Count);
			while (mostNestedCell.Table.ParentCell != null) {
				PieceTable.ChangeCellStartParagraphIndex(mostNestedCell, index + PatternRow.Cells.Count);
				mostNestedCell = mostNestedCell.Table.ParentCell;
			}
		}
		protected override TableRow GetNextRow(TableRow row) {
			return row.Previous;
		}
		protected internal override void CorrentVerticalMerging(TableRow row, int i, TableCell sourceCell, TableCell targetCell) {
			if (targetCell.Properties.VerticalMerging == MergingState.Restart) {
				targetCell.Properties.VerticalMerging = MergingState.None;
			}
		}
		protected internal override void CopyPropertiesFromPatternCell(TableCell sourceCell, TableCell targetCell) {
			targetCell.Properties.CopyFrom(sourceCell.Properties);
		}
	}
	#endregion
	#region PieceTableMergeTableCellsCommandBase (abstract class)
	public abstract class PieceTableMergeTableCellsCommandBase : PieceTableTableBaseCommand {
		#region Fields
		DocumentModel copyDocumentModel;
		DocumentModelCopyManager copyManager;
		TableCell cell;
		bool needDeleteNextTableCell = false;
		bool suppressNormalizeTableRows;
		#endregion
		protected PieceTableMergeTableCellsCommandBase(PieceTable pieceTable, TableCell cell)
			: base(pieceTable) {
			Guard.ArgumentNotNull(cell, "cell");
			this.cell = cell;
		}
		#region Properties
		protected internal PieceTable CopyPieceTable { get { return copyDocumentModel.ActivePieceTable; } }
		protected internal DocumentModelCopyManager CopyManager { get { return copyManager; } set { copyManager = value; } }
		protected internal bool NeedDeleteNextTableCell { get { return needDeleteNextTableCell; } set { needDeleteNextTableCell = value; } }
		protected internal TableCell PatternCell { get { return cell; } set { cell = value; } }
		protected internal bool SuppressNormalizeTableRows { get { return suppressNormalizeTableRows; } set { suppressNormalizeTableRows = value; } }
		#endregion
		protected internal override void ExecuteCore() {
			using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
				Table table = cell.Table;
				bool suppressIntegrityCheck = table.SuppressIntegrityCheck;
				table.SuppressIntegrityCheck = true;
				TableCell nextCell = CalculateNextCell();
				UpdateProperties(nextCell);
				SelectionRange selectionRangeForNextCell = CalculateSelectionRange(nextCell);
				bool isEmptyCell = IsEmptyCell(cell);
				bool isEmptyNextCell = IsEmptyCell(nextCell);
				CopyToCopyPieceTable(selectionRangeForNextCell);
				DeleteTableCellWithContent(nextCell, selectionRangeForNextCell);
				CopyToPieceTable();
				FixParagraphsInPatternCell(isEmptyCell, isEmptyNextCell);
				table.NormalizeCellColumnSpans();
				if (!suppressNormalizeTableRows)
					table.NormalizeRows();
				table.SuppressIntegrityCheck = suppressIntegrityCheck;
			}
		}
		protected internal abstract TableCell CalculateNextCell();
		protected internal abstract void UpdateProperties(TableCell nextCell);
		protected internal virtual SelectionRange CalculateSelectionRange(TableCell cell) {
			DocumentLogPosition startLogPosition = PieceTable.Paragraphs[cell.StartParagraphIndex].LogPosition;
			DocumentLogPosition endLogPosition = PieceTable.Paragraphs[cell.EndParagraphIndex].EndLogPosition;
			int length = endLogPosition - startLogPosition + 1;
			return new SelectionRange(startLogPosition, length);
		}
		protected internal virtual bool IsEmptyCell(TableCell cell) {
			Paragraph startParagraph = PieceTable.Paragraphs[cell.StartParagraphIndex];
			return cell.StartParagraphIndex == cell.EndParagraphIndex && startParagraph.Length == 1;
		}
		protected internal virtual void CopyToCopyPieceTable(SelectionRange copyingRange) {
			copyDocumentModel = PieceTable.DocumentModel.CreateNew();
			copyDocumentModel.IntermediateModel = true;
			copyDocumentModel.BeginUpdate();
			try {
				DocumentModelCopyOptions options = new DocumentModelCopyOptions(copyingRange.Start, copyingRange.Length);
				DocumentModelCopyCommand copyCommand = PieceTable.DocumentModel.CreateDocumentModelCopyCommand(PieceTable, copyDocumentModel, options);
				copyCommand.TableCopyFromNestedLevel = cell.Table.NestedLevel + 1;
				copyCommand.Execute();
			} finally {
				copyDocumentModel.EndUpdate();
			}
		}
		protected internal virtual void DeleteTableCellWithContent(TableCell nextCell, SelectionRange deletingRange) {
			Table table = nextCell.Table;
			if (NeedDeleteNextTableCell)
				PieceTable.DeleteTableCellWithNestedTables(table.Index, nextCell.RowIndex, nextCell.IndexInRow);
			PieceTable.DeleteContent(deletingRange.Start, deletingRange.Length, false);
		}
		protected internal virtual void CopyToPieceTable() {
			DocumentLogPosition position = PieceTable.Paragraphs[PatternCell.EndParagraphIndex].EndLogPosition;
			PieceTableInsertContentConvertedToDocumentModelCommand command = new PieceTableInsertContentConvertedToDocumentModelCommand(PieceTable, copyDocumentModel, position, false);
			command.IsMergingTableCell = true;
			command.CopyBetweenInternalModels = true;
			command.Execute();
		}
		protected internal virtual void FixParagraphsInPatternCell(bool needDeleteFirstParagraphInCell, bool needDeleteLastParagraphInCell) {
			if (!needDeleteFirstParagraphInCell && !needDeleteLastParagraphInCell)
				return;
			DeleteParagraphOperation operation = new DeleteParagraphOperation(PieceTable);
			operation.AllowedDeleteLastParagraphInTableCell = true;
			ParagraphCollection paragraphs = PieceTable.Paragraphs;
			if (needDeleteFirstParagraphInCell) {
				operation.Execute(paragraphs[PatternCell.StartParagraphIndex].LogPosition, 1, false);
				return;
			}
			if (needDeleteLastParagraphInCell)
				operation.Execute(paragraphs[PatternCell.EndParagraphIndex].LogPosition, 1, false);
		}
	}
	#endregion
	#region PieceTableMergeTwoTableCellsHorizontallyCommand
	public class PieceTableMergeTwoTableCellsHorizontallyCommand : PieceTableMergeTableCellsCommandBase {
		public PieceTableMergeTwoTableCellsHorizontallyCommand(PieceTable pieceTable, TableCell cell)
			: base(pieceTable, cell) {
		}
		protected internal override TableCell CalculateNextCell() {
			TableRow row = PatternCell.Row;
			int index = PatternCell.IndexInRow;
			int nextCellIndex = index + 1;
			TableCellCollection cells = row.Cells;
			if (nextCellIndex < cells.Count)
				return cells[nextCellIndex];
			else
				return null;
		}
		protected internal override void UpdateProperties(TableCell nextCell) {
			PatternCell.ColumnSpan += nextCell.ColumnSpan;
			PatternCell.PreferredWidth.Value += nextCell.PreferredWidth.Value;
		}
		protected internal override void DeleteTableCellWithContent(TableCell nextCell, SelectionRange deletingRange) {
			NeedDeleteNextTableCell = true;
			base.DeleteTableCellWithContent(nextCell, deletingRange);
		}
	}
	#endregion
	#region PieceTableMergeTwoTableCellsVerticallyCommand
	public class PieceTableMergeTwoTableCellsVerticallyCommand : PieceTableMergeTableCellsCommandBase {
		public PieceTableMergeTwoTableCellsVerticallyCommand(PieceTable pieceTable, TableCell cell)
			: base(pieceTable, cell) {
		}
		protected internal override TableCell CalculateNextCell() {
			Table table = PatternCell.Table;
			int columnIndex = PatternCell.GetStartColumnIndexConsiderRowGrid();
			return table.GetCell(PatternCell.RowIndex + 1, columnIndex);
		}
		protected internal override void UpdateProperties(TableCell nextCell) {
			PatternCell.VerticalMerging = MergingState.Restart;
			nextCell.VerticalMerging = MergingState.Continue;
		}
	}
	#endregion
	#region PieceTableMergeTableCellsHorizontallyCommand
	public class PieceTableMergeTableCellsHorizontallyCommand : PieceTableTableBaseCommand {
		#region Fields
		TableCell cell;
		int count;
		#endregion
		public PieceTableMergeTableCellsHorizontallyCommand(PieceTable pieceTable, TableCell cell, int count)
			: base(pieceTable) {
			Guard.ArgumentNotNull(cell, "cell");
			Guard.ArgumentPositive(count, "count");
			this.cell = cell;
			this.count = count;
		}
		protected internal virtual int CalculateCellIndex(TableRow row, int startCellIndex, int columnSpan) {
			TableCellCollection cells = row.Cells;
			int count = cells.Count;
			int result = startCellIndex;
			for (; result < count - 1; result++) {
				columnSpan -= cells[result].ColumnSpan;
				if (columnSpan <= 0)
					break;
			}
			return result;
		}
		protected internal override void ExecuteCore() {
			using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
				TableRow row = cell.Row;
				int startCellIndex = row.Cells.IndexOf(cell);
				int endCellIndex = CalculateCellIndex(row, startCellIndex, count) - 1;
				for (int i = endCellIndex; i >= startCellIndex; i--) {
					TableCell mergeCell = row.Cells[i];
					PieceTableMergeTwoTableCellsHorizontallyCommand command = new PieceTableMergeTwoTableCellsHorizontallyCommand(PieceTable, mergeCell);
					command.SuppressNormalizeTableRows = true;
					command.Execute();
				}
			}
		}
	}
	#endregion
	#region PieceTableMergeTableCellsVerticallyCommand
	public class PieceTableMergeTableCellsVerticallyCommand : PieceTableTableBaseCommand {
		#region Fields
		TableCell cell;
		int count;
		#endregion
		public PieceTableMergeTableCellsVerticallyCommand(PieceTable pieceTable, TableCell cell, int count)
			: base(pieceTable) {
			Guard.ArgumentNotNull(cell, "cell");
			Guard.ArgumentPositive(count, "count");
			this.cell = cell;
			this.count = count;
		}
		protected internal override void ExecuteCore() {
			using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
				Table table = cell.Table;
				int columnIndex = table.GetCellColumnIndexConsiderRowGrid(cell);
				int restartRowIndex = cell.RowIndex;
				int continionRowIndex = restartRowIndex + count - 2;
				for (int i = continionRowIndex; i >= restartRowIndex; i--) {
					TableCell mergeCell = table.GetCell(i, columnIndex);
					PieceTableMergeTwoTableCellsVerticallyCommand command = new PieceTableMergeTwoTableCellsVerticallyCommand(PieceTable, mergeCell);
					command.SuppressNormalizeTableRows = true;
					command.Execute();
				}
				table.NormalizeRows();
			}
		}
	}
	#endregion
	#region PieceTableDeleteTableCellWithNestedTablesCommand
	public class PieceTableDeleteTableCellWithNestedTablesCommand : PieceTableTableBaseCommand {
		int tableIndex;
		int rowIndex;
		int cellIndex;
		public PieceTableDeleteTableCellWithNestedTablesCommand(PieceTable pieceTable, int tableIndex, int rowIndex, int cellIndex)
			: base(pieceTable) {
			Guard.ArgumentNonNegative(tableIndex, "tableIndex");
			Guard.ArgumentNonNegative(rowIndex, "rowIndex");
			Guard.ArgumentNonNegative(cellIndex, "cellIndex");
			this.tableIndex = tableIndex;
			this.rowIndex = rowIndex;
			this.cellIndex = cellIndex;
		}
		protected internal override void ExecuteCore() {
			using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
				TableCell deletedCell = PieceTable.Tables[tableIndex].Rows[rowIndex].Cells[cellIndex];
				RunInfo runInfo = PieceTable.GetRunInfoByTableCell(deletedCell);
				PieceTable.DeleteSelectedTables(runInfo, false);
				PieceTable.DeleteEmptyTableCellCore(tableIndex, rowIndex, cellIndex);
			}
		}
	}
	#endregion
	#region PieceTableSplitTableCommand
	public class PieceTableSplitTableCommand : PieceTableTableBaseCommand {
		#region Fields
		DocumentModel copyDocumentModel;
		DocumentModelCopyManager copyManager;
		int tableIndex;
		int rowIndex;
		readonly bool forceVisible;
		#endregion
		public PieceTableSplitTableCommand(PieceTable pieceTable, int tableIndex, int rowIndex, bool forceVisible)
			: base(pieceTable) {
			Guard.ArgumentNonNegative(tableIndex, "tableIndex");
			Guard.ArgumentNonNegative(rowIndex, "rowIndex");
			this.tableIndex = tableIndex;
			this.rowIndex = rowIndex;
			this.forceVisible = forceVisible;
		}
		#region Properties
		protected internal PieceTable CopyPieceTable { get { return copyDocumentModel.ActivePieceTable; } }
		protected internal DocumentModelCopyManager CopyManager { get { return copyManager; } set { copyManager = value; } }
		protected internal Table Table { get { return PieceTable.Tables[tableIndex]; } }
		protected internal ParagraphCollection Paragraphs { get { return PieceTable.Paragraphs; } }
		#endregion
		protected internal override void ExecuteCore() {
			using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
				if (rowIndex == 0) {
					InsertParagraphBeforeTable();
				}
				else {
					SelectionRange selectionRange = CalculateSelectionRange();
					CopyToCopyPieceTable(selectionRange);
					DeleteContent(selectionRange);
					CopyToPieceTable();
				}
				Table.NormalizeCellColumnSpans();
				Table.NormalizeTableCellVerticalMerging();
			}
#if DEBUGTEST
			PieceTable.CheckIntegrity();
#endif
		}
		protected internal virtual void InsertParagraphBeforeTable() {
			TableCell firstCell = Table.Rows.First.Cells.First;
			ParagraphIndex startParagraphIndex = firstCell.StartParagraphIndex;
			DocumentLogPosition position = Paragraphs[startParagraphIndex].LogPosition;
			InsertParagraphWithDefaultProperties(position);
			TableCell nestedCell = PieceTable.Paragraphs[startParagraphIndex].GetCell();
			while (nestedCell != firstCell) {
				PieceTable.ChangeCellStartParagraphIndex(nestedCell, nestedCell.StartParagraphIndex + 1);
				nestedCell = PieceTable.Paragraphs[startParagraphIndex].GetCell();
			}
			PieceTable.ChangeCellStartParagraphIndex(firstCell, startParagraphIndex + 1);
		}
		protected internal virtual SelectionRange CalculateSelectionRange() {
			TableCell startCell = Table.Rows[rowIndex].FirstCell;
			TableCell endCell = Table.Rows.Last.LastCell;
			DocumentLogPosition startLogPosition = Paragraphs[startCell.StartParagraphIndex].LogPosition;
			DocumentLogPosition endLogPosition = Paragraphs[endCell.EndParagraphIndex].EndLogPosition;
			int length = endLogPosition - startLogPosition + 1;
			return new SelectionRange(startLogPosition, length);
		}
		protected internal virtual void CopyToCopyPieceTable(SelectionRange copyingRange) {
			copyDocumentModel = PieceTable.DocumentModel.CreateNew();
			copyDocumentModel.IntermediateModel = true;
			copyDocumentModel.BeginUpdate();
			try {
				DocumentModelCopyOptions options = new DocumentModelCopyOptions(copyingRange.Start, copyingRange.Length);
				DocumentModelCopyCommand copyCommand = PieceTable.DocumentModel.CreateDocumentModelCopyCommand(PieceTable, copyDocumentModel, options);
				copyCommand.Execute();
			}
			finally {
				copyDocumentModel.EndUpdate();
			}
		}
		protected internal virtual void DeleteContent(SelectionRange deletingRange) {
			TableRowCollection rows = Table.Rows;
			int lastRowIndex = rows.IndexOf(rows.Last);
			for (int i = lastRowIndex; i >= rowIndex; i--) {
				PieceTable.DeleteEmptyTableRowCore(tableIndex, i);
			}
			PieceTable.DeleteContent(deletingRange.Start, deletingRange.Length, false);
		}
		protected internal virtual void CopyToPieceTable() {
			TableRow row = Table.Rows[rowIndex - 1];
			DocumentLogPosition position = Paragraphs[row.LastCell.EndParagraphIndex].EndLogPosition;
			InsertParagraphWithDefaultProperties(position + 1);
			PieceTableInsertContentConvertedToDocumentModelCommand command = new PieceTableInsertContentConvertedToDocumentModelCommand(PieceTable, copyDocumentModel, position + 2, forceVisible);
			command.CopyBetweenInternalModels = true;
			command.Execute();
		}
		protected internal virtual void InsertParagraphWithDefaultProperties(DocumentLogPosition position) {
			PieceTable.InsertParagraph(position);
			ParagraphIndex insertedParagraphIndex = PieceTable.FindParagraphIndex(position);
			Paragraph insertedParagraph = Paragraphs[insertedParagraphIndex];
			insertedParagraph.ParagraphProperties.Reset();
			ParagraphStyleCollection paragraphStales = DocumentModel.ParagraphStyles;
			insertedParagraph.ParagraphStyleIndex = paragraphStales.IndexOf(paragraphStales.DefaultItem);
			TextRunBase run = PieceTable.Runs[insertedParagraph.FirstRunIndex];
			run.CharacterProperties.Reset();
		}
	}
	#endregion
	#region PieceTablePieceTableInsertColumnBase (abstract class)
	public abstract class PieceTablePieceTableInsertColumnBase : PieceTableTableBaseCommand {
		#region Fields
		readonly TableCell patternCell;
		readonly bool forceVisible;
		#endregion
		protected PieceTablePieceTableInsertColumnBase(PieceTable pieceTable, TableCell patternCell, bool forceVisible)
			: base(pieceTable) {
			this.patternCell = patternCell;
			this.forceVisible = forceVisible;
		}
		#region Properties
		public TableCell PatternCell { get { return patternCell; } }
		protected bool ForceVisible { get { return forceVisible; } }
		#endregion
		protected internal override void ExecuteCore() {
			using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
				int columnIndex = GetColumnIndex();
				Table table = PatternCell.Table;
				bool suppressIntegrityCheck = table.SuppressIntegrityCheck;
				table.SuppressIntegrityCheck = true;
				TableRowCollection rows = table.Rows;
				int rowsCount = rows.Count;
				for (int i = 0; i < rowsCount; i++) {
					TableRow currentRow = rows[i];
					TableCell currentCell = GetCurrentCell(columnIndex, currentRow);
					Modify(currentRow, currentCell);
				}
				table.SuppressIntegrityCheck = suppressIntegrityCheck;
				table.NormalizeTableGrid();
				table.NormalizeCellColumnSpans();
				NormalizeTableCellWidth(table);
			}
		}
		void NormalizeTableCellWidth(Table table) {
			const int maxWidth = 5000;
			int rowsCount = table.Rows.Count;
			for (int j = 0; j < rowsCount; j++) {
				TableRow currentRow = table.Rows[j];
				int cellsCount = currentRow.Cells.Count;
				int totalWidth = 0;
				for (int i = 0; i < cellsCount; i++) {
					TableCell cell = currentRow.Cells[i];
					if (cell.PreferredWidth.Type == WidthUnitType.FiftiethsOfPercent)
						totalWidth += cell.PreferredWidth.Value;
					else {
						totalWidth = 0;
						break;
					}
				}
				if (totalWidth <= maxWidth)
					continue;
				for (int i = 0; i < cellsCount; i++) {
					TableCell cell = currentRow.Cells[i];
					int newValue = cell.PreferredWidth.Value * maxWidth / totalWidth;
					cell.PreferredWidth.Value = newValue;
				}
			}
		}
		protected internal virtual void InsertColumnToTheLeft(TableRow currentRow, TableCell currentCell) {
			ParagraphIndex paragraphIndex = currentCell.StartParagraphIndex;
			DocumentLogPosition logPosition = PieceTable.Paragraphs[paragraphIndex].LogPosition;
			List<TableCell> cells = PieceTable.TableCellsManager.GetCellsByParagraphIndex(paragraphIndex, currentCell.Table.NestedLevel);
			PieceTable.InsertParagraph(logPosition, ForceVisible);
			ChangeStartParagraphIndexInCells(cells);
			TableCell newCell = InsertColumnCore(currentRow, currentCell.IndexInRow, paragraphIndex, paragraphIndex);
			newCell.Properties.CopyFrom(currentCell.Properties);
			newCell.StyleIndex = currentCell.StyleIndex;
			newCell.Properties.PreferredWidth.CopyFrom(PatternCell.PreferredWidth);
			newCell.ColumnSpan = 1;
			newCell.VerticalMerging = MergingState.None;
		}
		void ChangeStartParagraphIndexInCells(List<TableCell> cells) {
			int cellsCount = cells.Count;
			for (int i = 0; i < cellsCount; i++) {
				TableCell currentCell = cells[i];
				PieceTable.ChangeCellStartParagraphIndex(currentCell, currentCell.StartParagraphIndex + 1);
			}
		}
		protected internal virtual void InsertColumnToTheRight(TableRow currentRow, TableCell currentCell) {
			ParagraphIndex paragraphIndex = currentCell.EndParagraphIndex;
			DocumentLogPosition logPosition = PieceTable.Paragraphs[paragraphIndex].EndLogPosition;
			PieceTable.InsertParagraph(logPosition, ForceVisible);
			PieceTable.ChangeCellEndParagraphIndex(currentCell, currentCell.EndParagraphIndex - 1);
			paragraphIndex += 1;
			TableCell nextCell = currentCell.NextCellInRow;
			TableCell newCell = InsertColumnCore(currentRow, currentCell.IndexInRow + 1, paragraphIndex, paragraphIndex);
			newCell.StyleIndex = currentCell.StyleIndex;
			if (nextCell != null)
				newCell.Properties.CopyFrom(nextCell.Properties);
			else
				newCell.Properties.CopyFrom(currentCell.Properties);
			newCell.Properties.PreferredWidth.CopyFrom(PatternCell.PreferredWidth);
			newCell.ColumnSpan = 1;
			newCell.VerticalMerging = MergingState.None;
		}
		protected internal virtual TableCell InsertColumnCore(TableRow row, int insertedIndex, ParagraphIndex start, ParagraphIndex end) {
			TableCell newCell = PieceTable.CreateTableCellCore(row, insertedIndex, start, end);
			return newCell;
		}
		protected internal abstract int GetColumnIndex();
		protected internal abstract TableCell GetCurrentCell(int columnIndex, TableRow currentRow);
		protected internal abstract void Modify(TableRow currentRow, TableCell currentCell);
	}
	#endregion
	#region PieceTableInsertColumnToTheLeft
	public class PieceTableInsertColumnToTheLeft : PieceTablePieceTableInsertColumnBase {
		public PieceTableInsertColumnToTheLeft(PieceTable pieceTable, TableCell patternCell, bool forceVisible)
			: base(pieceTable, patternCell, forceVisible) {
		}
		protected internal override int GetColumnIndex() {
			return TableCellVerticalBorderCalculator.GetStartColumnIndex(PatternCell, false);
		}
		protected internal override TableCell GetCurrentCell(int columnIndex, TableRow currentRow) {
			return TableCellVerticalBorderCalculator.GetCellByColumnIndex(currentRow, columnIndex);
		}
		protected internal override void Modify(TableRow currentRow, TableCell currentCell) {
			if (currentCell == null)
				currentCell = currentRow.Cells.Last;
			InsertColumnToTheLeft(currentRow, currentCell);
		}
	}
	#endregion
	#region PieceTableInsertColumnToTheRight
	public class PieceTableInsertColumnToTheRight : PieceTablePieceTableInsertColumnBase {
		public PieceTableInsertColumnToTheRight(PieceTable pieceTable, TableCell patternCell, bool forceVisible)
			: base(pieceTable, patternCell, forceVisible) {
		}
		protected internal override int GetColumnIndex() {
			return TableCellVerticalBorderCalculator.GetStartColumnIndex(PatternCell, false) + PatternCell.ColumnSpan - 1;
		}
		protected internal override TableCell GetCurrentCell(int columnIndex, TableRow currentRow) {
			return TableCellVerticalBorderCalculator.GetCellByEndColumnIndex(currentRow, columnIndex);
		}
		protected internal override void Modify(TableRow currentRow, TableCell currentCell) {
			if (currentCell == null) {
				InsertColumnToTheLeft(currentRow, currentRow.Cells.First);
				return;
			}
			InsertColumnToTheRight(currentRow, currentCell);
		}
	}
	#endregion
	#region PieceTableDeleteTableColumns
	public class PieceTableDeleteTableColumnsCommand : PieceTableTableDocumentServerOwnerCommand {
		#region Fields
		SelectedCellsCollection selectedCells;
		int startColumnIndex;
		int endColumnIndex;
		#endregion
		public PieceTableDeleteTableColumnsCommand(PieceTable pieceTable, SelectedCellsCollection selectedCells, IInnerRichEditDocumentServerOwner server)
			: base(pieceTable, server) {
			Guard.ArgumentNotNull(selectedCells, "selectedCells");
			this.selectedCells = selectedCells;
		}
		#region Properties
		protected internal int StartColumnIndex { get { return startColumnIndex; } set { startColumnIndex = value; } }
		protected internal int EndColumnIndex { get { return endColumnIndex; } set { endColumnIndex = value; } }
		#endregion
		protected internal override void ExecuteCore() {
			using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
				Table table = selectedCells.NormalizedFirst.Table;
				if (selectedCells.IsSelectedEntireTable()) {
					table.PieceTable.DeleteTableWithContent(table);
					return;
				}
				TableRowCollection rows = table.Rows;
				int rowsCount = rows.Count;
				List<List<TableCell>> deletedCellsCollection = new List<List<TableCell>>();
				TableColumnWidthCalculator calculator = new TableColumnWidthCalculator(table, DocumentServer);
				TableWidthsContainer container = calculator.CalculateWidths();
				for (int i = rowsCount - 1; i >= 0; i--) {
					TableRow currentRow = rows[i];
					List<TableCell> cells = TableCellVerticalBorderCalculator.GetCellsByIntervalColumnIndex(currentRow, StartColumnIndex, EndColumnIndex);
					deletedCellsCollection.Add(cells);
				}
				for (int i = 0; i < rowsCount; i++) {
					List<TableCell> currentCells = deletedCellsCollection[i];   
					DeleteCellsWithContent(currentCells, container);
				}
				TableColumnKnownWidthCalculator newCalculator = new TableColumnKnownWidthCalculator(table, container);
				TableGridMerger merger = new TableGridMerger(new Table[] {table});  
				TableGrid tableGrid = new TableGrid(CalculateTableGridIntervals(newCalculator.GetTableGridColumns()));
				merger.MergeGrids(new TableGrid[] { tableGrid }, new int[] { table.Rows.Count });
				NormalizeCellVerticalMerging(table);
				table.NormalizeCellColumnSpans(); 
			}
		}		
		List<TableGridInterval> CalculateTableGridIntervals(TableGridColumnCollection tableGridColumnCollection) {
			List<TableGridInterval> result = new List<TableGridInterval>();
			int count = tableGridColumnCollection.Count;
			for (int i = 0; i < count; i++)
				result.Add(new TableGridInterval(tableGridColumnCollection[i].Width, 1, TableGridIntervalType.ModelUnit));	   
			return result;
		} 
		protected internal void DeleteCellsWithContent(List<TableCell> cells, TableWidthsContainer container) {
			int cellsCount = cells.Count;
			if (cellsCount == 0)
				return;
			if (IsSelectedEntireRow(cells)) {
				TableRow row = cells[0].Row;
				if (row.Table.Rows.Count == 1)
					PieceTable.DeleteTableWithContent(row.Table);
				else
					PieceTable.DeleteTableRowWithContent(row);
				return;
			}
			for (int i = cellsCount - 1; i >= 0; i--) {
				TableCell currentCell = cells[i];
				if (currentCell.VerticalMerging == MergingState.Continue && currentCell.IndexInRow == -1)
					continue;
				PieceTableDeleteTableCellWithContentKnownWidthsCommand command = new PieceTableDeleteTableCellWithContentKnownWidthsCommand(PieceTable, currentCell, DocumentServer, container);
				command.CanNormalizeCellVerticalMerging = false;
				command.UseDeltaBetweenColumnsUpdate = false;
				command.CollectVerticalSpanCells = false;
				command.Execute();
			}
		}
		protected internal bool IsSelectedEntireRow(List<TableCell> cells) {
			TableCell firstCell = cells[0];
			TableCell lastCell = cells[cells.Count - 1];
			TableRow row = firstCell.Row;
			return row.FirstCell == firstCell && row.LastCell == lastCell;
		}
		protected internal override void CalculateExecutionParameters() {
			TableCell firstCell = selectedCells.NormalizedFirst.NormalizedStartCell;
			TableCell lastCell = selectedCells.NormalizedFirst.NormalizedEndCell;
			StartColumnIndex = GetStartColumnIndex(firstCell);
			EndColumnIndex = GetEndColumnIndex(lastCell);
			int bottomRowIndex = selectedCells.GetBottomRowIndex();
			for (int i = selectedCells.GetTopRowIndex(); i <= bottomRowIndex; i++) {
				firstCell = selectedCells[i].NormalizedStartCell;
				lastCell = selectedCells[i].NormalizedEndCell;
				StartColumnIndex = Algorithms.Max(StartColumnIndex, GetStartColumnIndex(firstCell));
				EndColumnIndex = Algorithms.Min(EndColumnIndex, GetEndColumnIndex(lastCell));
			}
		}
		protected internal int GetStartColumnIndex(TableCell firstCell) {
			return TableCellVerticalBorderCalculator.GetStartColumnIndex(firstCell, false);
		}
		protected internal int GetEndColumnIndex(TableCell lastCell) {
			return TableCellVerticalBorderCalculator.GetStartColumnIndex(lastCell, false) + lastCell.ColumnSpan - 1;
		}
		protected internal virtual void NormalizeCellVerticalMerging(Table table) {
			TableRowCollection rows = table.Rows;
			int rowsCount = rows.Count - 1;
			for (int i = rowsCount; i >= 0; i--) {
				TableRow currentRow = rows[i];
				TableCellCollection cells = currentRow.Cells;
				int cellsCount = cells.Count;
				if (isAllCellsVerticalMergingContinue(cells))
					continue;
				for (int j = 0; j < cellsCount; j++) {
					TableCell currentCell = cells[j];
					MergingState verticalMerging = currentCell.VerticalMerging;
					if (verticalMerging == MergingState.None)
						continue;
					int columnIndex = currentCell.GetStartColumnIndexConsiderRowGrid();
					TableRow nextRow = currentRow.Next;
					if (verticalMerging == MergingState.Restart) {
						if (i == rowsCount) {
							currentCell.Properties.VerticalMerging = MergingState.None;
							continue;
						}
						TableCell nextCell = TableCellVerticalBorderCalculator.GetCellByStartColumnIndex(nextRow, columnIndex, false);
						if (nextCell == null || nextCell.VerticalMerging != MergingState.Continue)
							currentCell.Properties.VerticalMerging = MergingState.None;
					}
					else { 
						TableRow prevRow = currentRow.Previous;
						TableCell prevCell = prevRow != null ? TableCellVerticalBorderCalculator.GetCellByStartColumnIndex(prevRow, columnIndex, false) : null;
						TableCell nextCell = nextRow != null ? TableCellVerticalBorderCalculator.GetCellByStartColumnIndex(nextRow, columnIndex, false) : null;
						if ((prevCell == null || prevCell.VerticalMerging == MergingState.None) && (nextCell == null || nextCell.VerticalMerging != MergingState.Continue)) {
							currentCell.Properties.VerticalMerging = MergingState.None;
							continue;
						}
						if ((prevCell == null || prevCell.VerticalMerging == MergingState.None) && (nextCell != null && nextCell.VerticalMerging == MergingState.Continue))
							currentCell.Properties.VerticalMerging = MergingState.Restart;
					}
				}
			}
		}
		protected internal virtual bool isAllCellsVerticalMergingContinue(TableCellCollection cells) {
			int cellsCount = cells.Count;
			for (int i = 0; i < cellsCount; i++) {
				if (cells[i].VerticalMerging != MergingState.Continue)
					return false;
			}
			PieceTable.DeleteTableRowWithContent(cells.First.Row);
			return true;
		}
	}
	#endregion
	#region PieceTableDeleteTableCellWithShiftToTheUpCommand
	public class PieceTableDeleteTableCellsWithShiftToTheUpCommand : PieceTableTableBaseCommand {
		#region Fields
		SelectedCellsCollection selectedCells;
		#endregion
		public PieceTableDeleteTableCellsWithShiftToTheUpCommand(PieceTable pieceTable, SelectedCellsCollection selectedCells)
			: base(pieceTable) {
			Guard.ArgumentNotNull(selectedCells, "selectedCells");
			this.selectedCells = selectedCells;
		}
		protected internal override void ExecuteCore() {
			using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
				int selectedRowsCount = selectedCells.RowsCount;
				for (int i = selectedRowsCount - 1; i >= 0; i--) {
					SelectedCellsIntervalInRow currentSelectedCells = selectedCells[i];
					DeleteSelectedCells(currentSelectedCells);
				}
			}
		}
		protected internal void DeleteSelectedCells(SelectedCellsIntervalInRow selectedCells) {
			int leftCellIndex = selectedCells.NormalizedStartCellIndex;
			for (int i = selectedCells.NormalizedEndCellIndex; i >= leftCellIndex; i--) {
				TableCell deletedCell = selectedCells.Row.Cells[i];
				PieceTableDeleteTableCellWithShiftToTheUpCoreCommand command = new PieceTableDeleteTableCellWithShiftToTheUpCoreCommand(PieceTable, deletedCell);
				command.Execute();
			}
		}
	}
	#endregion
	#region PieceTableDeleteTableCellWithShiftToTheUpCoreCommand
	public class PieceTableDeleteTableCellWithShiftToTheUpCoreCommand : PieceTableTableBaseCommand {
		#region Fields
		TableCell cell;
		#endregion
		public PieceTableDeleteTableCellWithShiftToTheUpCoreCommand(PieceTable pieceTable, TableCell cell)
			: base(pieceTable) {
			this.cell = cell;
		}
		protected internal override void ExecuteCore() {
			Table table = cell.Table;
			int columnIndex = table.GetCellColumnIndexConsiderRowGrid(cell);
			int rowsCount = table.Rows.Count;
			for (int i = cell.RowIndex; i < rowsCount; i++) {
				TableCell deletedCell = table.GetCell(i, columnIndex);
				PieceTableDeleteOneTableCellWithShiftToTheUpCommand command = new PieceTableDeleteOneTableCellWithShiftToTheUpCommand(PieceTable, deletedCell);
				command.Execute();
			}
		}
	}
	#endregion
	#region PieceTableDeleteOneTableCellWithShiftToTheUpCommand
	public class PieceTableDeleteOneTableCellWithShiftToTheUpCommand : PieceTableMergeTwoTableCellsVerticallyCommand {
		#region Fields
		RunInfo runInfo;
		#endregion
		public PieceTableDeleteOneTableCellWithShiftToTheUpCommand(PieceTable pieceTable, TableCell cell)
			: base(pieceTable, cell) {
			this.runInfo = PieceTable.GetRunInfoByTableCell(PatternCell);
		}
		protected internal override void ExecuteCore() {
			if (PatternCell.Row.IsLastRowInTable)
				DeleteContentInCell();
			else
				base.ExecuteCore();
		}
		protected internal override void FixParagraphsInPatternCell(bool needDeleteFirstParagraphInCell, bool needDeleteLastParagraphInCell) {
			DeleteContentInCell();
		}
		protected internal virtual void DeleteContentInCell() {
			DocumentLogPosition startLogPosition = runInfo.Start.LogPosition;
			PieceTable.DeleteContent(startLogPosition, runInfo.End.LogPosition - startLogPosition + 1, false);
		}
		protected internal override void UpdateProperties(TableCell nextCell) {
		}
	}
	#endregion
	#region PieceTableDeleteTableCellWithContentCommand
	public class PieceTableDeleteTableCellWithContentCommand : PieceTableTableDocumentServerOwnerCommand {
		#region Fields
		TableCell deletedCell;
		bool canNormalizeCellVerticalMerging;
		bool useDeltaBetweenColumnsUpdate;
		bool collectVerticalSpanCells;
		#endregion
		public PieceTableDeleteTableCellWithContentCommand(PieceTable pieceTable, TableCell deletedCell, IInnerRichEditDocumentServerOwner server)
			: base(pieceTable, server) {
			this.deletedCell = deletedCell;
			this.canNormalizeCellVerticalMerging = true;
			this.useDeltaBetweenColumnsUpdate = true;
			this.collectVerticalSpanCells = true;
		}
		#region Properties
		public bool CanNormalizeCellVerticalMerging { get { return canNormalizeCellVerticalMerging; } set { canNormalizeCellVerticalMerging = value; } }
		public bool UseDeltaBetweenColumnsUpdate { get { return useDeltaBetweenColumnsUpdate; } set { useDeltaBetweenColumnsUpdate = value; } }
		public bool CollectVerticalSpanCells { get { return collectVerticalSpanCells; } set { collectVerticalSpanCells = value; } }
		#endregion
		protected internal override void ExecuteCore() {
			using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
				Table table = deletedCell.Table;
				bool suppressIntegrityCheck = table.SuppressIntegrityCheck;
				table.SuppressIntegrityCheck = true;
				int patternCellStartColumnIndex = deletedCell.GetStartColumnIndexConsiderRowGrid();
				List<TableCell> verticalSpanCells;
				if (CollectVerticalSpanCells)
					verticalSpanCells = TableCellVerticalBorderCalculator.GetVerticalSpanCells(deletedCell, patternCellStartColumnIndex, false);
				else
					verticalSpanCells = new List<TableCell>() { deletedCell };
				ITableColumnWidthCalculator calculator = CreateTableColumnWidthCalculator(table);
				NormalizeTableGridAfterDeleteCellHelper normalizeHelper = new NormalizeTableGridAfterDeleteCellHelper(table, verticalSpanCells, calculator.GetTableGridColumns(),  useDeltaBetweenColumnsUpdate);
				for (int i = verticalSpanCells.Count - 1; i >= 0; i--) {
					TableCell currentCell = verticalSpanCells[i];
					if (CanNormalizeCellVerticalMerging)
						normalizeHelper.NormalizeVerticalMerging(currentCell);
					PieceTable.DeleteTableCellWithNestedTables(table.Index, currentCell.RowIndex, currentCell.IndexInRow);
					RunInfo runInfo = PieceTable.GetRunInfoByTableCell(currentCell);
					DocumentLogPosition startLogPosition = runInfo.Start.LogPosition;
					PieceTable.DeleteContent(startLogPosition, runInfo.End.LogPosition - startLogPosition + 1, false, true);
				}
				table.SuppressIntegrityCheck = suppressIntegrityCheck;
				normalizeHelper.NormalizeColumnSpans();
				if (CanNormalizeCellVerticalMerging)
					table.NormalizeRows();
				table.NormalizeTableGrid();
				table.NormalizeCellColumnSpans(false);
				normalizeHelper.NormalizeWidthAfter(table);
			}
		}
		protected virtual ITableColumnWidthCalculator CreateTableColumnWidthCalculator(Table table) {
			return new TableColumnWidthCalculator(table, DocumentServer);
		}
	}
	#endregion
	#region PieceTableDeleteTableCellWithContentKnownWidthsCommand
	public class PieceTableDeleteTableCellWithContentKnownWidthsCommand : PieceTableDeleteTableCellWithContentCommand {
		TableWidthsContainer container;
		public PieceTableDeleteTableCellWithContentKnownWidthsCommand(PieceTable pieceTable, TableCell deletedCell, IInnerRichEditDocumentServerOwner server, TableWidthsContainer container)
			: base(pieceTable, deletedCell, server) {
				this.container = container; 
		}
		protected override ITableColumnWidthCalculator CreateTableColumnWidthCalculator(Table table) {
			return new TableColumnKnownWidthCalculator(table, container);
		}
	}
	#endregion
	#region PieceTableInsertTableCellsBase (abstract class)
	public abstract class PieceTableInsertTableCellsBase : PieceTableTableDocumentServerOwnerCommand {
		#region Fields
		TableCell patternCell;
		bool canNormalizeTable;
		bool canCopyProperties;
		bool canNormalizeVerticalMerging;
		readonly bool forceVisible;
		#endregion
		protected PieceTableInsertTableCellsBase(PieceTable pieceTable, TableCell patternCell, bool forceVisible, IInnerRichEditDocumentServerOwner server)
			: base(pieceTable, server) {
			Guard.ArgumentNotNull(patternCell, "patternCell");
			this.patternCell = patternCell;
			this.canNormalizeTable = true;
			this.canCopyProperties = true;
			this.canNormalizeVerticalMerging = true;
			this.forceVisible = forceVisible;
		}
		#region Properties
		public TableCell PatternCell { get { return patternCell; } }
		protected internal bool CanNormalizeTable { get { return canNormalizeTable; } set { canNormalizeTable = value; } }
		protected internal bool CanCopyProperties { get { return canCopyProperties; } set { canCopyProperties = value; } }
		protected internal bool CanNormalizeVerticalMerging { get { return canNormalizeVerticalMerging; } set { canNormalizeVerticalMerging = value; } }
		protected bool ForceVisible { get { return forceVisible; } }
		#endregion
		protected abstract TableCell Modify(TableCell cell);
		protected internal override void ExecuteCore() {
			using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
				Table table = PatternCell.Table;
				bool suppressIntegrityCheck = table.SuppressIntegrityCheck;
				table.SuppressIntegrityCheck = true;
				int patternCellStartColumnIndex = table.GetCellColumnIndexConsiderRowGrid(PatternCell);
				TableCell restartCell = table.GetFirstCellInVerticalMergingGroup(PatternCell);
				List<TableCell> cells = TableCellVerticalBorderCalculator.GetVerticalSpanCells(restartCell, patternCellStartColumnIndex, false);
				NormalizeTableGridAfterInsertCellHelper normalizeHelper = null;
				if (CanNormalizeTable) {
					TableColumnWidthCalculator calculator = new TableColumnWidthCalculator(table, DocumentServer);
					normalizeHelper = new NormalizeTableGridAfterInsertCellHelper(table, cells, calculator.GetTableGridColumns(), true);
				}
				for (int i = cells.Count - 1; i >= 0; i--) {
					TableCell currentCell = cells[i];
					if (CanNormalizeVerticalMerging)
						normalizeHelper.NormalizeVerticalMerging(currentCell);
					TableCell insertedCell = Modify(currentCell);
					if (CanCopyProperties) {
						insertedCell.Properties.CopyFrom(currentCell.Properties);
						insertedCell.StyleIndex = currentCell.StyleIndex;
					}
				}
				table.SuppressIntegrityCheck = suppressIntegrityCheck;
				if (CanNormalizeTable) {
					normalizeHelper.NormalizeColumnSpans();
					NormalizeTableGridAfter(table);
					table.NormalizeCellColumnSpans();
					normalizeHelper.NormalizeWidthAfter(table);
				}
			}
		}
		protected internal virtual void NormalizeTableGridAfter(Table table) {
			int minEndColumnIndex = int.MaxValue;
			TableRowCollection rows = table.Rows;
			int rowsCount = rows.Count;
			for (int i = 0; i < rowsCount; i++) {
				TableRow currentRow = rows[i];
				int currentEndColumnIndex = currentRow.LastCell.GetEndColumnIndexConsiderRowGrid() + currentRow.GridAfter;
				minEndColumnIndex = Algorithms.Min(minEndColumnIndex, currentEndColumnIndex);
			}
			for (int i = 0; i < rowsCount; i++) {
				TableRow currentRow = rows[i];
				int currentEndColumnIndex = currentRow.LastCell.GetEndColumnIndexConsiderRowGrid() + currentRow.GridAfter;
				currentRow.GridAfter -= currentEndColumnIndex - minEndColumnIndex;
			}
		}
	}
	#endregion
	#region PieceTableInsertTableCellToTheLeft
	public class PieceTableInsertTableCellToTheLeft : PieceTableInsertTableCellsBase {
		public PieceTableInsertTableCellToTheLeft(PieceTable pieceTable, TableCell patternCell, bool forceVisible, IInnerRichEditDocumentServerOwner server)
			: base(pieceTable, patternCell, forceVisible, server) {
		}
		protected override TableCell Modify(TableCell cell) {
			ParagraphIndex paragraphIndex = cell.StartParagraphIndex;
			DocumentLogPosition logPosition = PieceTable.Paragraphs[paragraphIndex].LogPosition;
			List<TableCell> cells = PieceTable.TableCellsManager.GetCellsByParagraphIndex(paragraphIndex, cell.Table.NestedLevel);
			PieceTable.InsertParagraph(logPosition, ForceVisible);
			ChangeStartParagraphIndexInCells(cells);
			return PieceTable.CreateTableCellCore(cell.Row, cell.IndexInRow, paragraphIndex, paragraphIndex);
		}
		void ChangeStartParagraphIndexInCells(List<TableCell> cells) {
			int cellsCount = cells.Count;
			for (int i = 0; i < cellsCount; i++) {
				TableCell currentCell = cells[i];
				PieceTable.ChangeCellStartParagraphIndex(currentCell, currentCell.StartParagraphIndex + 1);
			}
		}
	}
	#endregion
	#region PieceTableInsertTableCellToTheRight
	public class PieceTableInsertTableCellToTheRight : PieceTableInsertTableCellsBase {
		public PieceTableInsertTableCellToTheRight(PieceTable pieceTable, TableCell patternCell, bool forceVisible, IInnerRichEditDocumentServerOwner server)
			: base(pieceTable, patternCell, forceVisible, server) {
		}
		protected override TableCell Modify(TableCell cell) {
			ParagraphIndex paragraphIndex = cell.EndParagraphIndex;
			DocumentLogPosition logPosition = PieceTable.Paragraphs[paragraphIndex].EndLogPosition;
			PieceTable.InsertParagraph(logPosition, ForceVisible);
			PieceTable.ChangeCellEndParagraphIndex(cell, cell.EndParagraphIndex - 1);
			int insertedIndex = cell.IndexInRow + 1;
			return PieceTable.CreateTableCellCore(cell.Row, insertedIndex, paragraphIndex + 1, paragraphIndex + 1);
		}
	}
	#endregion
	#region PieceTableInsertTableCellWithShiftToTheDownCommand
	public class PieceTableInsertTableCellWithShiftToTheDownCommand : PieceTableTableDocumentServerOwnerCommand {
		#region Fields
		TableCell patternCell;
		readonly bool forceVisible;
		#endregion
		public PieceTableInsertTableCellWithShiftToTheDownCommand(PieceTable pieceTable, TableCell patternCell, bool forceVisible, IInnerRichEditDocumentServerOwner server)
			: base(pieceTable, server) {
			Guard.ArgumentNotNull(patternCell, "patternCell");
			this.patternCell = patternCell;
			this.forceVisible = forceVisible;
		}
		protected internal override void ExecuteCore() {
			using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
				TableRowCollection rows = patternCell.Table.Rows;
				int startRowIndex = patternCell.Row.IndexInTable;
				int endRowIndex = rows.Count - 1;
				int patternCellIndexInRow = patternCell.IndexInRow;
				for (int i = endRowIndex; i > startRowIndex; i--) {
					TableRow currentRow = rows[i];
					int cellsCountInCurrentRow = currentRow.Cells.Count;
					TableRow previousRow = currentRow.Previous;
					if (patternCellIndexInRow >= previousRow.Cells.Count)
						continue;
					if (patternCellIndexInRow >= cellsCountInCurrentRow)
						InsertTableCells(currentRow.LastCell, patternCellIndexInRow - cellsCountInCurrentRow + 1, previousRow.LastCell.PreferredWidth);
					TableCell currentCell = currentRow.Cells[patternCellIndexInRow];
					PieceTableInsertTableCellWithShiftToTheDownCoreCommand command = new PieceTableInsertTableCellWithShiftToTheDownCoreCommand(PieceTable, currentCell);
					command.Execute();
				}
				DeleteContentInPatternCell();
				patternCell.Table.NormalizeTableGrid();
				patternCell.Table.NormalizeCellColumnSpans();
			}
		}
		protected internal virtual void InsertTableCells(TableCell patternCell, int insertedCellsCount, PreferredWidth width) {
			int patternCellIndexInRow = patternCell.IndexInRow;
			TableCellCollection cellsInRow = patternCell.Row.Cells;
			for (int i = 0; i < insertedCellsCount; i++) {
				int indexLastCell = patternCellIndexInRow + i;
				PieceTableInsertTableCellToTheRight command = new PieceTableInsertTableCellToTheRight(PieceTable, cellsInRow[indexLastCell], forceVisible, DocumentServer);
				command.CanNormalizeTable = false;
				command.CanCopyProperties = false;
				command.Execute();
				cellsInRow[indexLastCell + 1].Properties.PreferredWidth.CopyFrom(width);
			}
		}
		protected internal virtual void DeleteContentInPatternCell() {
			RunInfo runInfo = PieceTable.GetRunInfoByTableCell(patternCell);
			DocumentLogPosition startLogPosition = runInfo.Start.LogPosition;
			PieceTable.DeleteContent(startLogPosition, runInfo.End.LogPosition - startLogPosition + 1, false);
		}
	}
	#endregion
	#region PieceTableInsertTableCellWithShiftToTheDownCoreCommand
	public class PieceTableInsertTableCellWithShiftToTheDownCoreCommand : PieceTableMergeTableCellsCommandBase {
		public PieceTableInsertTableCellWithShiftToTheDownCoreCommand(PieceTable pieceTable, TableCell cell)
			: base(pieceTable, cell) {
		}
		protected internal override TableCell CalculateNextCell() {
			return PatternCell.Table.Rows[PatternCell.Row.IndexInTable - 1].Cells[PatternCell.IndexInRow];
		}
		protected internal override void UpdateProperties(TableCell nextCell) {
			PatternCell.Properties.Borders.CopyFrom(nextCell.Properties.Borders); 
			PatternCell.Properties.BackgroundColor = nextCell.BackgroundColor; 
			PatternCell.Properties.VerticalAlignment = nextCell.VerticalAlignment; 
		}
		protected internal override void DeleteTableCellWithContent(TableCell nextCell, SelectionRange deletingRange) {
			SelectionRange selectionRangeForPatternCell = CalculateSelectionRange(PatternCell);
			PieceTable.DeleteContent(selectionRangeForPatternCell.Start, selectionRangeForPatternCell.Length, false);
		}
		protected internal override void FixParagraphsInPatternCell(bool needDeleteFirstParagraphInCell, bool needDeleteLastParagraphInCell) {
			base.FixParagraphsInPatternCell(true, false);
		}
	}
	#endregion
	#region PieceTableSplitTableCellsHorizontally
	public class PieceTableSplitTableCellsHorizontally : PieceTableTableDocumentServerOwnerCommand {
		#region Fields
		readonly TableCell patternCell;
		readonly int partsCount;
		readonly bool forceVisible;
		#endregion
		public PieceTableSplitTableCellsHorizontally(PieceTable pieceTable, TableCell patternCell, int partsCount, bool forceVisible, IInnerRichEditDocumentServerOwner server)
			: base(pieceTable, server) {
			this.patternCell = patternCell;
			this.partsCount = partsCount;
			this.forceVisible = forceVisible;
		}
		protected internal override void ExecuteCore() {
			using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
				int patternCellStartColumnIndex = patternCell.GetStartColumnIndexConsiderRowGrid();
				List<TableCell> verticalSpanCells = TableCellVerticalBorderCalculator.GetVerticalSpanCells(patternCell, patternCellStartColumnIndex, false);
				TableColumnWidthCalculator calculator = new TableColumnWidthCalculator(patternCell.Table, DocumentServer);
				NormalizeTableGridAfterSplitCellHelper normalizeHelper = new NormalizeTableGridAfterSplitCellHelper(patternCell.Table, verticalSpanCells, partsCount, calculator.GetTableGridColumns(), true);
				int oldWidthPatternCell = patternCell.PreferredWidth.Value;
				int lastCellIndex = verticalSpanCells.Count - 1;
				for (int i = lastCellIndex; i >= 0; i--) {
					verticalSpanCells[i].PreferredWidth.Value /= partsCount;
				}
				for (int i = 0; i < partsCount - 1; i++) {
					PieceTableInsertTableCellToTheRight command = new PieceTableInsertTableCellToTheRight(PieceTable, patternCell, forceVisible, DocumentServer);
					command.CanNormalizeTable = false;
					command.CanNormalizeVerticalMerging = false;
					command.ExecuteCore();
				}
				NormalizeTableCellsWidth(oldWidthPatternCell);
				normalizeHelper.NormalizeColumnSpans();
			}
		}
		protected internal void NormalizeTableCellsWidth(int oldWidth) {
			int widthAllNewCells = 0;
			int startIndex = patternCell.IndexInRow;
			int endIndex = startIndex + partsCount;
			TableCellCollection cells = patternCell.Row.Cells;
			for (int i = startIndex; i < endIndex; i++) {
				TableCell currentCell = cells[i];
				widthAllNewCells += currentCell.PreferredWidth.Value;
			}
			int delta = oldWidth - widthAllNewCells;
			Debug.Assert(startIndex + delta <= endIndex);
			endIndex = startIndex + delta;
			for (int i = startIndex; i < endIndex; i++) {
				TableCell currentCell = cells[i];
				currentCell.PreferredWidth.Value += 1;
			}
		}
	}
	#endregion
	#region PieceTableSplitTableCellsVertically
	public class PieceTableSplitTableCellsVertically : PieceTableTableBaseCommand {
		#region Fields
		TableCell patternCell;
		int partsCount;
		int columnsCount;
		readonly bool forceVisible;
		#endregion
		public PieceTableSplitTableCellsVertically(PieceTable pieceTable, TableCell patternCell, int partsCount, int columnsCount, bool forceVisible)
			: base(pieceTable) {
			Guard.ArgumentNotNull(patternCell, "patternCell");
			this.patternCell = patternCell;
			this.partsCount = partsCount;
			this.columnsCount = columnsCount;
			this.forceVisible = forceVisible;
		}
		protected internal override void ExecuteCore() {
			using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
				if (partsCount == 1)
					return;
				TableRow row = patternCell.Row;
				int normStartIndex = patternCell.IndexInRow;
				TableCellCollection cellsInRow = row.Cells;
				if (cellsInRow[normStartIndex].VerticalMerging == MergingState.Restart) {
					SplitMergedCellsVertically(cellsInRow);
					return;
				}
				InsertRows(row);
				int cellsCount = cellsInRow.Count;
				int endIndex = GetSelectedCellsEndIndex();
				for (int i = 0; i < cellsCount; i++) {
					if (i < normStartIndex || i > endIndex) {
						TableCell currentCell = cellsInRow[i];
						TableCell mergeCell = currentCell.Table.GetFirstCellInVerticalMergingGroup(currentCell);
						int delta = currentCell.RowIndex - mergeCell.RowIndex;
						PieceTable.MergeTableCellsVertically(mergeCell, partsCount + delta);
					}
				}
			}
		}
		protected internal virtual void InsertRows(TableRow row) {
			row.Height.Value /= partsCount;
			for (int i = 0; i < partsCount - 1; i++) {
				PieceTable.InsertTableRowBelow(row, forceVisible);
			}
		}
		protected internal virtual void SplitMergedCellsVertically(TableCellCollection cellsInRow) {
			int endIndex = GetSelectedCellsEndIndex();
			for (int i = patternCell.IndexInRow; i <= endIndex; i++) {
				SplitMergedCellsVerticallyCore(cellsInRow[i]);
			}
		}
		protected internal virtual int GetSelectedCellsEndIndex() {
			return patternCell.IndexInRow + (columnsCount - 1);
		}
		protected internal virtual void SplitMergedCellsVerticallyCore(TableCell restartCell) {
			int columnIndex = TableCellVerticalBorderCalculator.GetStartColumnIndex(restartCell, false);
			List<TableCell> mergedCells = TableCellVerticalBorderCalculator.GetVerticalSpanCells(restartCell, columnIndex, false);
			int mergedCellsCount = mergedCells.Count;
			if (mergedCellsCount == partsCount) {
				for (int i = 0; i < mergedCellsCount; i++) {
					mergedCells[i].VerticalMerging = MergingState.None;
				}
				return;
			}
			int totalRowsCount = mergedCellsCount / partsCount;
			for (int i = 0; i < mergedCellsCount; i++) {
				if (i % totalRowsCount == 0)
					mergedCells[i].VerticalMerging = MergingState.Restart;
			}
		}
	}
	#endregion
	#region SimpleTableWidthsCalculator
	public class SimpleTableWidthsCalculator : TableWidthsCalculatorBase {
		const int defaultPreferredWidthInTwips = 120;
		internal const int DefaultPercentBaseWidthInTwips = 5 * 1440;
		public SimpleTableWidthsCalculator(DocumentModelUnitToLayoutUnitConverter converter)
			: this(converter, converter.ToLayoutUnits(DefaultPercentBaseWidthInTwips)) {
		}
		public SimpleTableWidthsCalculator(DocumentModelUnitToLayoutUnitConverter converter, int percentBaseWidth)
			: base(converter, percentBaseWidth) {
		}
		protected override WidthsContentInfo CalculateParagraphWidths(Paragraph paragraph) {
			if (paragraph.BoxCollection.IsValid)
				return base.CalculateParagraphWidths(paragraph);
			return WidthsContentInfo.Empty;
		}
		protected override WidthsContentInfo CalculateCellContentWidthsCore(TableCell cell, int percentBaseWidth, bool simpleView) {
			WidthsContentInfo result = base.CalculateCellContentWidthsCore(cell, percentBaseWidth, simpleView);
			if (result != WidthsContentInfo.Empty)
				return result;
			LayoutUnit preferredWidth = 0;
			if (cell.PreferredWidth.Type == WidthUnitType.ModelUnits)
				preferredWidth = Converter.ToLayoutUnits(cell.PreferredWidth.Value);
			if (preferredWidth == 0)
				preferredWidth = Converter.ToLayoutUnits(defaultPreferredWidthInTwips) * cell.ColumnSpan;
			return new WidthsContentInfo(preferredWidth, preferredWidth);
		}
		public override bool CanUseCachedTableLayoutInfo(TableLayoutInfo tableLayoutInfo) {
			return false;
		}
		public override TableLayoutInfo CreateTableLayoutInfo(TableGrid tableGrid, LayoutUnit maxTableWidth, bool allowTablesToExtendIntoMargins, bool simpleView, LayoutUnit percentBaseWidth) {
			return null;
		}
	}
	#endregion
	#region JoinTableWidthsCalculator
	public class JoinTableWidthsCalculator : SimpleTableWidthsCalculator {
		public JoinTableWidthsCalculator(DocumentModelUnitToLayoutUnitConverter converter)
			: this(converter, converter.ToLayoutUnits(DefaultPercentBaseWidthInTwips)) {
		}
		public JoinTableWidthsCalculator(DocumentModelUnitToLayoutUnitConverter converter, int percentBaseWidth)
			: base(converter, percentBaseWidth) {
		}
		protected override LayoutUnit GetMaxWidth(WidthsContentInfo contentWidths, ModelUnit horizontalMargins, ModelUnit bordersWidth, ModelUnit spacing) {
			return contentWidths.MaxWidth;
		}
		protected override LayoutUnit GetMinWidth(WidthsContentInfo contentWidths, ModelUnit horizontalMargins, ModelUnit bordersWidth, ModelUnit spacing) {
			return contentWidths.MinWidth;
		}
	}
	#endregion
	#region PieceTableJoinSeveralTablesCommand
	public class PieceTableJoinSeveralTablesCommand : PieceTableTableBaseCommand {
		#region Fields
		Table[] tables;
		DocumentLogPosition joinPosition;
		#endregion
		public PieceTableJoinSeveralTablesCommand(PieceTable pieceTable, Table[] tables)
			: base(pieceTable) {
			Guard.ArgumentNotNull(tables, "tables");
			if (tables.Length < 2)
				Exceptions.ThrowArgumentException("tables.Length", tables.Length);
			this.tables = tables;
			ParagraphIndex firstTableLastEndParagraphIndex = tables[0].LastRow.LastCell.EndParagraphIndex;
			this.joinPosition = PieceTable.Paragraphs[firstTableLastEndParagraphIndex].EndLogPosition;
		}
		protected internal override void ExecuteCore() {
			int columnWidth = CalculateColumnWidth();
			int count = tables.Length;
			TableGrid[] tableGrids = new TableGrid[count];
			for (int tableIndex = 0; tableIndex < count; tableIndex++) {
				tableGrids[tableIndex] = GetTableGrid(tables[tableIndex], columnWidth);
			}
			using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
				int[] sourceTablesRowCount = new int[count];
				Table resultTable = tables[0];
				sourceTablesRowCount[0] = resultTable.Rows.Count;
				for (int tableIndex = 1; tableIndex < count; tableIndex++) {
					Table table = tables[tableIndex];
					sourceTablesRowCount[tableIndex] = table.Rows.Count;
					TableRowCollection rowsInBottomTable = table.Rows;
					int rowCountInBottomTable = rowsInBottomTable.Count;
					for (int i = 0; i < rowCountInBottomTable; i++) {
						TableRow currentBottomRow = table.Rows[i];
						PieceTable.MoveTableRowToOtherTable(resultTable, currentBottomRow);
					}
					PieceTable.DeleteTableFromTableCollection(table);
				}
				TableGridMerger merger = new TableGridMerger(tables);
				merger.MergeGrids(tableGrids, sourceTablesRowCount);
			}
		}
		int CalculateColumnWidth() {
			SectionIndex sectionIndex = DocumentModel.FindSectionIndex(joinPosition);
			Section section = DocumentModel.Sections[sectionIndex];
			DocumentModelTwipsToLayoutDocumentsConverter converter = new DocumentModelTwipsToLayoutDocumentsConverter();
			PageBoundsCalculator pageBoundsCalculator = new PageBoundsCalculator(converter);
			ColumnsBoundsCalculator columnsBoundsCalculator = new ColumnsBoundsCalculator(converter);
			return columnsBoundsCalculator.Calculate(section, pageBoundsCalculator.CalculatePageClientBounds(section))[0].Width;
		}
		TableGrid GetTableGrid(Table table, int percentWidthBase) {
			JoinTableWidthsCalculator widthsCalculator = new JoinTableWidthsCalculator(DocumentModel.ToDocumentLayoutUnitConverter, percentWidthBase);
			TableGridCalculator calculator = new TableGridCalculator(DocumentModel, widthsCalculator, percentWidthBase);
			return calculator.CalculateTableGrid(table, percentWidthBase);
		}
		protected internal virtual int CalculateColumnIndex(int position, List<int> columnWidthCollection, ref int correction) {
			int index = columnWidthCollection.IndexOf(position);
			if (index >= 0)
				return index;
			int columnWidthCount = columnWidthCollection.Count;
			for (int key = 0; key < columnWidthCount; key++) {
				int value = columnWidthCollection[key] - position;
				if (Math.Abs(value) < TableGridMerger.Delta) {
					correction += value;
					return key;
				}
			}
			Exceptions.ThrowInternalException();
			return -1;
		}
	}
	#endregion
	public struct PositionInfo {
		int position;
		int minLeft;
		int maxRight;
		public PositionInfo(int position, int minLeft, int maxRight) {
			this.position = position;
			this.minLeft = minLeft;
			this.maxRight = maxRight;
		}
		public int Position { get { return position; } set { position = value; } }
		public int MinLeft { get { return minLeft; } set { minLeft = value; } }
		public int MaxRight { get { return maxRight; } set { maxRight = value; } }
	}
	#region TableGridMerger
	public class TableGridMerger {
		#region Fields
		public const int Delta = 5;
		Table[] tables;
		#endregion
		public TableGridMerger(Table[] tables) {
			this.tables = tables;
		}
		public void MergeGrids(TableGrid[] tableGrids, int[] sourceTableRowsCount) {
			List<int> totalPosition = new List<int>();
			List<int[]> absolutePositions = CalculateMergedPositions(tables, tableGrids, totalPosition, sourceTableRowsCount);
			Table resultTable = tables[0];
			int count = tables.Length;
			int rowIndex = 0;
			for (int i = 0; i < count; i++) {
				MergedGridCore(resultTable, rowIndex, rowIndex + sourceTableRowsCount[i] - 1, absolutePositions[i], totalPosition);
				rowIndex += sourceTableRowsCount[i];
			}
			resultTable.NormalizeTableGrid();
			resultTable.NormalizeCellColumnSpans();
		}
		protected internal virtual List<int[]> CalculateMergedPositions(Table[] tables, TableGrid[] tableGrids, List<int> totalPositions, int[] sourceTableRowsCount) {
			List<PositionInfo[]> absolutePositions = new List<PositionInfo[]>(tableGrids.Length);
			int count = tableGrids.Length;
			for (int i = 0; i < count; i++) {
				absolutePositions.Add(CalculateAbsolutePositions(tables[i], tableGrids[i].Columns, sourceTableRowsCount[i]));
			}
			AdjustNearestPositions(absolutePositions, Delta, totalPositions);
			List<int[]> result = new List<int[]>(tableGrids.Length);
			for (int i = 0; i < count; i++)
				result.Add(GetPositions(absolutePositions[i]));
			return result;
		}
		int[] GetPositions(PositionInfo[] positionInfo) {
			int[] result = new int[positionInfo.Length];
			for (int i = 0; i < positionInfo.Length; i++)
				result[i] = positionInfo[i].Position;
			return result;
		}
		protected internal virtual PositionInfo[] CalculateAbsolutePositions(Table table, TableGridColumnCollection columns, int rowCount) {
			int columnCount = columns.Count;
			PositionInfo[] positions = new PositionInfo[columnCount + 1];
			int currentPosition = 0;
			positions[0] = new PositionInfo(0, 0, Int32.MaxValue);
			for (int i = 0; i < columnCount; i++) {
				currentPosition += columns[i].Width;
				positions[i + 1] = new PositionInfo(currentPosition, Int32.MinValue, Int32.MaxValue);
			}
			TableRowCollection rows = table.Rows;
			for (int i = 0; i < rowCount; i++) {
				TableRow row = rows[i];
				TableCellCollection cells = row.Cells;
				int startIndex = row.GridBefore;
				for (int j = 0; j < cells.Count; j++) {
					int endIndex = startIndex + cells[j].ColumnSpan;
					positions[startIndex].MaxRight = endIndex;
					positions[endIndex].MinLeft = startIndex;
					startIndex = endIndex;
				}
			}
			return positions;
		}
		void AdjustNearestPositions(List<PositionInfo[]> absolutePositions, int layoutDelta, List<int> totalPositions) {
			totalPositions.Add(0);
			int count = absolutePositions.Count;
			for (int i = 0; i < count; i++) {
				PositionInfo[] positions = absolutePositions[i];
				for (int j = 0; j < positions.Length; j++) {
					int pos = positions[j].Position;
					int index = totalPositions.BinarySearch(pos);
					if (index >= 0)
						continue;
					index = ~index;
					AdjustPosition(totalPositions, index, positions, j, layoutDelta);
				}
			}
		}
		void AdjustPosition(List<int> totalPositions, int totalPositionIndex, PositionInfo[] positions, int positionIndex, int layoutDelta) {
			PositionInfo pos = positions[positionIndex];
			if (AdjustRight(totalPositions, totalPositionIndex, positions, positionIndex, layoutDelta, pos))
				return;
			if (AdjustLeft(totalPositions, totalPositionIndex, positions, positionIndex, layoutDelta, pos))
				return;
			totalPositions.Insert(totalPositionIndex, pos.Position);
		}
		bool AdjustRight(List<int> totalPositions, int totalPositionIndex, PositionInfo[] positions, int positionIndex, int layoutDelta, PositionInfo pos) {
			if (totalPositionIndex >= totalPositions.Count)
				return false;
			int nextPosition = totalPositions[totalPositionIndex];
			if (nextPosition - pos.Position < layoutDelta && (positionIndex + 1 >= positions.Length || nextPosition < positions[positionIndex + 1].Position) && (pos.MaxRight >= positions.Length || nextPosition < positions[pos.MaxRight].Position)) {
				positions[positionIndex].Position = nextPosition;
				return true;
			}
			else
				return false;
		}
		bool AdjustLeft(List<int> totalPositions, int totalPositionIndex, PositionInfo[] positions, int positionIndex, int layoutDelta, PositionInfo pos) {
			if (totalPositionIndex <= 0)
				return false;
			int prevPosition = totalPositions[totalPositionIndex - 1];
			if (pos.Position - prevPosition < layoutDelta && (positionIndex == 0 || prevPosition >= positions[positionIndex - 1].Position) && (pos.MinLeft < 0 || prevPosition > positions[pos.MinLeft].Position)) {
				positions[positionIndex].Position = prevPosition;
				return true;
			}
			else
				return false;
		}
		protected internal virtual void MergedGridCore(Table table, int startRowIndex, int endRowIndex, int[] tablePositions, List<int> totalPositions) {
			for (int rowIndex = startRowIndex; rowIndex <= endRowIndex; rowIndex++) {
				TableRow row = table.Rows[rowIndex];
				int startIndex = 0;
				int rowGridBefore = row.GridBefore;
				if (rowGridBefore != 0) {
					int endIndex = startIndex + rowGridBefore;
					row.GridBefore = CalculateColumnSpan(totalPositions, tablePositions[0], tablePositions[endIndex]);
					startIndex = endIndex;
				}
				TableCellCollection cells = row.Cells;
				int cellsCount = row.Cells.Count;
				for (int cellIndex = 0; cellIndex < cellsCount; cellIndex++) {
					TableCell cell = cells[cellIndex];
					int endIndex = startIndex + cell.ColumnSpan;
					cell.ColumnSpan = CalculateColumnSpan(totalPositions, tablePositions[startIndex], tablePositions[endIndex]);
					startIndex = endIndex;
				}
				int rowGridAfter = row.GridAfter;
				if (rowGridAfter != 0) {
					int endIndex = startIndex + rowGridAfter;
					if (endIndex >= tablePositions.Length)
						endIndex = tablePositions.Length - 1;
					row.GridAfter = CalculateColumnSpan(totalPositions, tablePositions[startIndex], tablePositions[endIndex]);
				}
			}
		}
		int CalculateColumnSpan(List<int> totalPositions, int startPos, int endPos) {
			return totalPositions.BinarySearch(endPos) - totalPositions.BinarySearch(startPos);
		}
	}
	#endregion
	#region PieceTableJoinTablesCommand
	public class PieceTableJoinTablesCommand : PieceTableJoinSeveralTablesCommand {
		public PieceTableJoinTablesCommand(PieceTable pieceTable, Table topTable, Table bottomTable)
			: base(pieceTable, new Table[] { topTable, bottomTable }) {
			Guard.ArgumentNotNull(topTable, "sourceTable");
			Guard.ArgumentNotNull(bottomTable, "targetTable");
		}
	}
	#endregion
	public interface ITableColumnWidthCalculator {
		TableGridColumnCollection GetTableGridColumns();
	}
	public class TableColumnWidthCalculator : ITableColumnWidthCalculator {
		Table table;
		IInnerRichEditDocumentServerOwner server;
		public TableColumnWidthCalculator(Table table, IInnerRichEditDocumentServerOwner server) {
			Guard.ArgumentNotNull(table, "table");
			this.table = table;
			this.server = server;
		}
		public Table Table { get { return table; } }
		public DocumentModel DocumentModel { get { return Table.DocumentModel; } }
		public TableGridColumnCollection GetTableGridColumns() {
			int columnWidth = CalculateColumnWidth();
			MeasurementAndDrawingStrategy measurement = server.CreateMeasurementAndDrawingStrategy(DocumentModel);
			measurement.Initialize();
			TableGrid grid = GetTableGrid(Table,  columnWidth, measurement.Measurer);
			return grid.Columns;
		}
		int CalculateColumnWidth() {
			DocumentLogPosition position = DocumentModel.ActivePieceTable.Paragraphs[Table.StartParagraphIndex].LogPosition;
			SectionIndex sectionIndex = DocumentModel.FindSectionIndex(position);
			Section section = DocumentModel.Sections[sectionIndex];
			DocumentModelTwipsToLayoutDocumentsConverter converter = new DocumentModelTwipsToLayoutDocumentsConverter();
			PageBoundsCalculator pageBoundsCalculator = new PageBoundsCalculator(converter);
			ColumnsBoundsCalculator columnsBoundsCalculator = new ColumnsBoundsCalculator(converter);
			return columnsBoundsCalculator.Calculate(section, pageBoundsCalculator.CalculatePageClientBounds(section))[0].Width;
		}
		TableGrid GetTableGrid(Table table, int percentWidthBase, BoxMeasurer measurer) {
			PieceTable oldPieceTable = measurer.PieceTable;
			try {
				measurer.PieceTable = table.PieceTable;
				TableWidthsCalculator widthsCalculator = new TableWidthsCalculator(table.PieceTable, measurer, percentWidthBase);
				TableGridCalculator calculator = new TableGridCalculator(DocumentModel, widthsCalculator, percentWidthBase);
				return calculator.CalculateTableGrid(table, percentWidthBase);
			}
			finally {
				measurer.PieceTable = oldPieceTable;
			}
		}
		public TableWidthsContainer CalculateWidths() {
			Dictionary<TableCell, int> cellWidths = new Dictionary<TableCell, int>();
			Dictionary<TableRow, int> gridBeforeWidths = new Dictionary<TableRow, int>(); 
			TableGridColumnCollection gridColumns = GetTableGridColumns();
			TableRowCollection rows = table.Rows;
			int rowsCount = rows.Count;
			for (int i = 0; i < rowsCount; i++) {
				TableRow currentRow = rows[i];
				gridBeforeWidths.Add(currentRow, GetColumnsWidth(gridColumns, 0, currentRow.GridBefore-1));
				int firstCellIndex = currentRow.GridBefore;
				TableCellCollection cells = currentRow.Cells;
				int cellsCount = cells.Count;
				for (int j = 0; j < cellsCount; j++) {
					TableCell currentCell = cells[j];
					int lastCellIndex = firstCellIndex + currentCell.ColumnSpan - 1;
					cellWidths.Add(currentCell, GetColumnsWidth(gridColumns, firstCellIndex, lastCellIndex));
					firstCellIndex = lastCellIndex + 1;
				}
			}
			return new TableWidthsContainer(cellWidths, gridBeforeWidths);
		}
		int GetColumnsWidth(TableGridColumnCollection gridColumns, int firstIndex, int lastIndex) {
			int width = 0;
			for (int i = firstIndex; i <= lastIndex; i++)
				width += gridColumns[i].Width;
			return width;
		}
	}
	public class TableWidthsContainer {
		readonly Dictionary<TableCell, int> cellWidths;
		readonly Dictionary<TableRow, int> gridBeforeWidths;
		public TableWidthsContainer(Dictionary<TableCell, int> cellWidths, Dictionary<TableRow, int> gridBeforeWidths) {
			this.cellWidths = cellWidths;
			this.gridBeforeWidths = gridBeforeWidths;
		}
		public Dictionary<TableCell, int> CellWidths { get { return cellWidths; } }
		public Dictionary<TableRow, int> GridBeforeWidths { get { return gridBeforeWidths; } }
	}	
	public class TableColumnKnownWidthCalculator : ITableColumnWidthCalculator {
		Table table;
		Dictionary<TableCell, int> cellWidths;
		Dictionary<TableRow, int> gridBeforeWidths;
		public TableColumnKnownWidthCalculator(Table table, TableWidthsContainer container) {
			this.table = table;
			this.cellWidths = container.CellWidths;
			this.gridBeforeWidths = container.GridBeforeWidths;
		}
		public TableGridColumnCollection GetTableGridColumns() {			
			List<int> listWidths = CalculateWidths();
			return CreateColumnCollection(listWidths);			 
		}
		List<int> CalculateWidths() {
			List<int> listWidths = new List<int>();
			int width = 0;
			TableRowCollection rows = table.Rows;
			int rowsCount = rows.Count;
			for (int i = 0; i < rowsCount; i++) {
				int gridBefore = gridBeforeWidths[rows[i]];
				TableRow currentRow = rows[i];
				if (!listWidths.Contains(gridBefore))
					listWidths.Add(gridBefore);
				width = gridBefore;
				TableCellCollection cells = currentRow.Cells;
				int cellsCount = cells.Count;
				for (int j = 0; j < cellsCount; j++) {
					TableCell currentCell = cells[j];
					width += cellWidths[currentCell];
					if (!listWidths.Contains(width))
						listWidths.Add(width);
				}
			}
			listWidths.Sort();
			return listWidths;
		}
		TableGridColumnCollection CreateColumnCollection(List<int> listWidths) {
			TableGridColumnCollection columnCollection = new TableGridColumnCollection();
			int count = listWidths.Count;
			for (int i = 0; i < count-1; i++){
				int width = listWidths[i + 1] - listWidths[i];
				columnCollection.Add(new TableGridColumn(width, false));
			}
			return columnCollection; 
		}
	}
	#region NormalizeTableGridHelperBase (abstract class)
	public abstract class NormalizeTableGridHelperBase {
		#region Fields
		protected internal const int DeltaBetweenColumns = 4;
		int deltaBetweenColumnsUpdate;
		readonly Table table;
		List<TableCellParameters> patternCellsParameters;
		TableGridColumnCollection gridColumns;
		#endregion
		protected NormalizeTableGridHelperBase(Table table, List<TableCell> cells, TableGridColumnCollection gridColumns, bool useDeltaBetweenColumns) {
			Guard.ArgumentNotNull(cells, "deletedCells");
			Guard.ArgumentNotNull(table, "table");
			this.table = table;
			this.patternCellsParameters = CalculatePatternCellsParameters(cells);
			this.gridColumns = gridColumns;
			if (useDeltaBetweenColumns)
				deltaBetweenColumnsUpdate = DeltaBetweenColumns;
			else deltaBetweenColumnsUpdate = 0;
		}
		#region Properties
		public Table Table { get { return table; } }
		public List<TableCellParameters> PatternCellsParameters { get { return patternCellsParameters; } }
		public DocumentModel DocumentModel { get { return Table.DocumentModel; } }
		public TableGridColumnCollection GridColumns { get { return gridColumns; } }
		protected internal int DeltaBetweenColumnsUpdate { get { return deltaBetweenColumnsUpdate; } }
		#endregion
		protected internal virtual List<TableCellParameters> CalculatePatternCellsParameters(List<TableCell> cells) {
			List<TableCellParameters> result = new List<TableCellParameters>();
			int cellsCount = cells.Count;
			for (int i = 0; i < cellsCount; i++) {
				TableCell cell = cells[i];
				int startColumnIndex = cell.GetStartColumnIndexConsiderRowGrid();
				int endColumnIndex = cell.GetEndColumnIndexConsiderRowGrid(startColumnIndex);
				TableCellParameters parameters = new TableCellParameters(cell.IndexInRow, cell.RowIndex, startColumnIndex, endColumnIndex);
				result.Add(parameters);
			}
			return result;
		}
		protected internal void NormalizeColumnSpans() {
			List<int> rowContainsPatternCellIndexes = GetRowContainsPatternCellIndexes();
			List<DistanceCollection> distancesToNewCells = CalculateDistancesToNewCells();
			List<int> mergedDistances = MergeDistances(distancesToNewCells);
			SnapDistanceToColumnGrid(mergedDistances, distancesToNewCells);
			TableRowCollection rows = Table.Rows;
			int rowsCount = rows.Count;
			for (int i = 0; i < rowsCount; i++) {
				if (!rowContainsPatternCellIndexes.Contains(i))
					NormalizeColumnSpansCore(mergedDistances, rows[i]);
			}
			RecalculateGridIntervals(mergedDistances);
			NormalizeColumnSpansInRowContainingPatternCell(distancesToNewCells);
			table.ResetCachedLayoutInfo();
		}
		protected virtual void SnapDistanceToColumnGrid(List<int> mergedDistances, List<DistanceCollection> distancesToNewCells) {
		}
		protected internal virtual List<DistanceCollection> CalculateDistancesToNewCells() {
			List<DistanceCollection> result = new List<DistanceCollection>();
			int cellsCount = PatternCellsParameters.Count;
			for (int i = 0; i < cellsCount; i++) {
				result.Add(CalculateDistancesToNewCellsCore(PatternCellsParameters[i]));
			}
			return result;
		}
		protected internal virtual DistanceCollection CalculateDistancesToNewCellsCore(TableCellParameters cellParameters) {
			DistanceCollection result = new DistanceCollection();
			TableCellCollection cells = Table.Rows[cellParameters.RowIndex].Cells;
			int indexInRow = cellParameters.IndexInRow;
			if (indexInRow == cells.Count)
				return result;
			int patternCellWidth = 0;
			int distanceToCell = 0;
			int startColumnIndex = cellParameters.StartColumnIndex;
			int endColumnIndex = cellParameters.EndColumnIndex;
			TableCell cell = cells[GetIndex(indexInRow)];
			int columnIndex = GetColumnIndex(endColumnIndex, cell);
			int gridIntervalsCount = GridColumns.Count;
			for (int i = 0; i < gridIntervalsCount; i++) {
				int currentWidth = GridColumns[i].Width;
				distanceToCell += currentWidth;
				if (i >= startColumnIndex && i <= endColumnIndex)
					patternCellWidth += currentWidth;
				if (i >= startColumnIndex && columnIndex == i) {
					result.Add(CalculateDistance(distanceToCell, patternCellWidth));
					if (cell.IsLastCellInRow)
						break;
					cell = cell.Next;
					columnIndex += cell.ColumnSpan;
				}
			}
			return result;
		}
		protected internal virtual int GetColumnIndex(int index, TableCell cell) {
			return index;
		}
		protected internal virtual int CalculateDistance(int distanceToCell, int cellWidth) {
			return distanceToCell + cellWidth;
		}
		protected internal List<int> MergeDistances(List<DistanceCollection> distances) {
			List<int> result = new List<int>();
			int count = distances.Count;
			if (count == 0)
				return result;
			result.AddRange(distances[0]);
			for (int i = 1; i < count; i++) {
				DistanceCollection currentDistances = distances[i];
				int currentDistancesCount = currentDistances.Count;
				for (int j = 0; j < currentDistancesCount; j++) {
					int distance = currentDistances[j];
					if (!result.Contains(distance))
						result.Add(distance);
				}
			}
			result.Sort();
			return result;
		}
		protected internal virtual List<int> GetRowContainsPatternCellIndexes() {
			List<int> result = new List<int>();
			int count = PatternCellsParameters.Count;
			for (int i = 0; i < count; i++) {
				result.Add(PatternCellsParameters[i].RowIndex);
			}
			return result;
		}
		protected internal void NormalizeColumnSpansCore(List<int> distances, TableRow tableRow) {
			int tableRowGridBefore = tableRow.GridBefore;
			int tableRowGridAfter = tableRow.GridAfter;
			for (int i = distances.Count - 1; i >= 0; i--) {
				ColumnIndexParameters parameters = CalculateColumnIndexParameters(distances[i]);
				if (parameters.ColumnIndex < 0) {
					tableRow.GridAfter++;
					continue;
				}
				if (parameters.IsExactly)
					continue;
				int columnIndex = parameters.ColumnIndex;
				if (columnIndex < tableRowGridBefore) {
					tableRow.GridBefore++;
					continue;
				}
				if (columnIndex > GridColumns.Count - tableRowGridAfter - 1) {
					tableRow.GridAfter++;
					continue;
				}
				TableCell cell = TableCellVerticalBorderCalculator.GetCellByColumnIndex(tableRow, columnIndex);
				if(cell == null)
					continue;
				cell.ColumnSpan++;
			}
		}
		protected internal virtual int GetWidthConsiderUnitType(WidthUnit widthUnit) {
			int result = widthUnit.Value;
			if (widthUnit.Type == WidthUnitType.ModelUnits)
				result = Table.DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(result);
			return result;
		}
		protected internal void RecalculateGridIntervals(List<int> mergedDistances) {
			int gridIntervalsCount = GridColumns.Count;
			int totalLength = 0;
			for (int i = 0; i < gridIntervalsCount; i++) {
				TableGridColumn currentColumn = GridColumns[i];
				int currentColumnWidth = currentColumn.Width;
				totalLength += currentColumnWidth;
				int columnWidth = currentColumnWidth;
				int previousNewColumnWidth = 0;
				int distancesCount = mergedDistances.Count;
				for (int j = 0; j < distancesCount; j++) {
					int distance = mergedDistances[j];
					int delta = Math.Abs(totalLength - distance);
					int previousTotalWidth = totalLength - columnWidth;
					int deltaPrevious = Math.Abs(previousTotalWidth - distance);
					if (delta <= deltaBetweenColumnsUpdate || deltaPrevious <= deltaBetweenColumnsUpdate || distance > totalLength || distance < previousTotalWidth)
						continue;
					int newWidth = distance - previousTotalWidth - previousNewColumnWidth;
					currentColumn.Width = columnWidth - newWidth - previousNewColumnWidth;
					TableGridColumn newInterval = new TableGridColumn(newWidth, false);
					GridColumns.Insert(i, newInterval);
					i++;
					gridIntervalsCount++;
					previousNewColumnWidth += newWidth;
				}
			}
		}
		protected void NormalizeColumnSpansInRowContainingPatternCell(List<DistanceCollection> distances) {
			int count = PatternCellsParameters.Count;
			for (int i = 0; i < count; i++) {
				List<int> columnSpans = CalculateColumnSpansForNewCells(distances[i], PatternCellsParameters[i]);
				NormalizeColumnSpansInRowContainingPatternCellCore(columnSpans, PatternCellsParameters[i]);
			}
		}
		protected internal void NormalizeColumnSpansInRowContainingPatternCellCore(List<int> columnSpans, TableCellParameters cellParameters) {
			int startIndex = GetIndex(cellParameters.IndexInRow);
			TableCellCollection cells = Table.Rows[cellParameters.RowIndex].Cells;
			int spanCount = columnSpans.Count;
			for (int i = 0; i < spanCount; i++) {
				int columnSpan = columnSpans[i];
				cells[startIndex + i].ColumnSpan = columnSpan <= 0 ? 1 : columnSpan;
			}
		}
		protected internal virtual int GetIndex(int index) {
			return index;
		}
		protected internal virtual List<int> CalculateColumnSpansForNewCells(List<int> distances, TableCellParameters cellParameters) {
			List<int> result = new List<int>();
			int startColumnIndex = cellParameters.StartColumnIndex;
			int intervalsCount = distances.Count;
			for (int i = 0; i < intervalsCount; i++) {
				ColumnIndexParameters parameters = CalculateColumnIndexParameters(distances[i]);
				int columnSpan = parameters.ColumnIndex - startColumnIndex + 1;
				result.Add(columnSpan);
				startColumnIndex = parameters.ColumnIndex;
				if (parameters.IsExactly)
					startColumnIndex++;
			}
			return result;
		}
		protected internal ColumnIndexParameters  CalculateColumnIndexParameters(int distanceToCell) {
			ColumnIndexParameters result = new ColumnIndexParameters(-1, false);
			int gridIntervalsCount = GridColumns.Count;
			int totalLength = 0;
			int previousTotalLength = 0;
			for(int i = 0; i < gridIntervalsCount; i++) {
				totalLength += GridColumns[i].Width;
				int prveiousDelta = Math.Abs(distanceToCell - previousTotalLength);
				bool isExactly = distanceToCell == totalLength;
				bool isPrevousExactly = prveiousDelta <= deltaBetweenColumnsUpdate;
				if(distanceToCell <= totalLength) {
					int columnIndex = !isExactly && isPrevousExactly && previousTotalLength > 0 ? i - 1 : i;
					result.ColumnIndex = columnIndex;
					result.IsExactly = isExactly || (isPrevousExactly && previousTotalLength > 0);
					return result;
				}
				previousTotalLength = totalLength;
			}
			return result;
		}
		protected internal void NormalizeWidthAfter(Table table) {
			List<int> widthsCollection = new List<int>();
			int maxWidth = 0;
			TableRowCollection rows = table.Rows;
			int rowsCount = rows.Count;
			for (int i = 0; i < rowsCount; i++) {
				TableRow currentRow = rows[i];
				int width = GetActualWidthValue(currentRow.WidthBefore);
				TableCellCollection cells = currentRow.Cells;
				int cellsCount = cells.Count;
				for (int j = 0; j < cellsCount; j++) {
					TableCell currentCell = cells[j];
					width += GetActualWidthValue(currentCell.PreferredWidth);
				}
				maxWidth = Math.Max(maxWidth, width);
				widthsCollection.Add(width);
			}
			for (int i = 0; i < rowsCount; i++) {
				TableRow currentRow = rows[i];
				WidthUnit widthAfter = currentRow.WidthAfter;
				TableRowProperties currentRowProperties = currentRow.Properties;
				if (currentRow.GridAfter == 0) {
					if (widthAfter.Type != WidthUnitType.Nil || widthAfter.Value != 0) {
						ResetWidthAfter(currentRowProperties);
					}
					continue;
				}
				WidthUnitType unitType = rows.First.Cells.First.PreferredWidth.Type;
				if (unitType == WidthUnitType.Auto) {
					ResetWidthAfter(currentRowProperties);
					continue;
				}
				int newWidth = maxWidth - widthsCollection[i];
				Debug.Assert(newWidth > 0 || unitType == WidthUnitType.Nil);
				currentRowProperties.WidthAfter.CopyFrom(new WidthUnitInfo(unitType, newWidth));
			}
		}
		protected internal virtual int GetActualWidthValue(WidthUnit widthUnit) {
			WidthUnitType unitType = widthUnit.Type;
			return unitType == WidthUnitType.ModelUnits || unitType == WidthUnitType.FiftiethsOfPercent ? widthUnit.Value : 0;
		}
		protected internal virtual void ResetWidthAfter(TableRowProperties rowProperties) {
			rowProperties.WidthAfter.CopyFrom(new WidthUnitInfo(WidthUnitType.Nil, 0));
			rowProperties.ResetUse(TableRowPropertiesOptions.Mask.UseWidthAfter);
		}
		[System.Diagnostics.Conditional("DEBUG")]
		internal void CheckTableWidthTypeIntegrity(Table table) {
#if DEBUGTEST
#if !DXPORTABLE
			DevExpress.XtraRichEdit.Tests.DocumentModelIntegrityValidator.CheckTableWidthTypeIntegrity(table);
#endif
#endif
		}
		protected internal virtual void NormalizeVerticalMerging(TableCell cell) {
			int cellsCount = cell.Row.Cells.Count;
			for (int i = cell.IndexInRow + 1; i < cellsCount; i++) {
				TableCell currentCell = cell.Row.Cells[i];
				MergingState currentCellVerticalMerging = currentCell.VerticalMerging;
				if (currentCellVerticalMerging == MergingState.None)
					continue;
				TableCell firstCellInMergingGroup = currentCell.Table.GetFirstCellInVerticalMergingGroup(currentCell);
				int startColumnIndex = firstCellInMergingGroup.GetStartColumnIndexConsiderRowGrid();
				List<TableCell> verticalSpanCells = TableCellVerticalBorderCalculator.GetVerticalSpanCells(firstCellInMergingGroup, startColumnIndex, false);
				int verticalSpanCellsCount = verticalSpanCells.Count;
				Debug.Assert(verticalSpanCellsCount > 1);
				currentCell.Properties.VerticalMerging = MergingState.None;
				if (currentCellVerticalMerging == MergingState.Restart) {
					if (verticalSpanCellsCount > 2)
						verticalSpanCells[1].Properties.VerticalMerging = MergingState.Restart;
					else {
						verticalSpanCells[0].Properties.VerticalMerging = MergingState.None;
						verticalSpanCells[1].Properties.VerticalMerging = MergingState.None;
					}
				}
				else {
					int currentCellIndexInMergingGroup = verticalSpanCells.IndexOf(currentCell);
					if (currentCellIndexInMergingGroup == 1) {
						verticalSpanCells[0].Properties.VerticalMerging = MergingState.None;
					}
					if (verticalSpanCellsCount - 2 == currentCellIndexInMergingGroup)
						verticalSpanCells[verticalSpanCellsCount - 1].Properties.VerticalMerging = MergingState.None;
					else
						if (verticalSpanCellsCount - 1 != currentCellIndexInMergingGroup)
							verticalSpanCells[currentCellIndexInMergingGroup + 1].Properties.VerticalMerging = MergingState.Restart;
				}
			}
		}
	}
	#endregion
	#region NormalizeTableGridAfterSplitCellHelper
	public class NormalizeTableGridAfterSplitCellHelper : NormalizeTableGridHelperBase {
		#region Fields
		readonly int partsCount;
		#endregion
		public NormalizeTableGridAfterSplitCellHelper(Table table, List<TableCell> cells, int partsCount, TableGridColumnCollection gridColumns, bool useDeltaBetweenColumns)
			: base(table, cells, gridColumns, useDeltaBetweenColumns) {
			this.partsCount = partsCount;
		}
		protected internal override DistanceCollection CalculateDistancesToNewCellsCore(TableCellParameters cellParameters) {
			DistanceCollection result = new DistanceCollection();
			TableCellCollection cells = Table.Rows[cellParameters.RowIndex].Cells;
			if (cellParameters.IndexInRow == cells.Count)
				return result;
			int patternCellWidth = 0;
			int distanceToPatternCell = 0;
			int startColumnIndex = cellParameters.StartColumnIndex;
			int endColumnIndex = cellParameters.EndColumnIndex;
			for (int i = 0; i <= endColumnIndex; i++) {
				int currentWidth = GridColumns[i].Width;
				if (i < startColumnIndex)
					distanceToPatternCell += currentWidth;
				if (i >= startColumnIndex)
					patternCellWidth += currentWidth;
			}
			int partWidth = patternCellWidth / partsCount;
			int distanceToCell = distanceToPatternCell;
			for (int i = 0; i < partsCount - 1; i++) {
				distanceToCell += partWidth;
				result.Add(distanceToCell);
			}
			result.Add(distanceToPatternCell + patternCellWidth);
			return result;
		}
		protected override void SnapDistanceToColumnGrid(List<int> mergedDistances, List<DistanceCollection> distancesToNewCells) {
			int gridColumnsCount = GridColumns.Count;
			int distanceCount = mergedDistances.Count;
			int[] startIndices = new int[distanceCount];
			int gridColumnIndex = 0;
			int columnDistance = 0;
			while (gridColumnIndex < gridColumnsCount && columnDistance + GridColumns[gridColumnIndex].Width < mergedDistances[0]) {
				columnDistance += GridColumns[gridColumnIndex].Width;
				gridColumnIndex++;
			}
			int nextAllowedDistance = columnDistance + DeltaBetweenColumnsUpdate + 1;
			for (int i = 0; i < distanceCount; i++) {
				while (gridColumnIndex < gridColumnsCount && columnDistance < nextAllowedDistance) {
					columnDistance += GridColumns[gridColumnIndex].Width;
					gridColumnIndex++;
				}
				int distance = mergedDistances[i];
				int oldDistance = distance;
				if (distance < nextAllowedDistance)
					distance = nextAllowedDistance;
				if (gridColumnIndex < gridColumnsCount && Math.Abs(distance - columnDistance) <= DeltaBetweenColumnsUpdate)
					distance = columnDistance;
				if (oldDistance != distance) {
					ChangeDistance(distancesToNewCells, oldDistance, distance, startIndices);
					mergedDistances[i] = distance;
				}
				nextAllowedDistance = distance + DeltaBetweenColumnsUpdate + 1;
			}
		}
		void ChangeDistance(List<DistanceCollection> distancesToNewCells, int oldDistance, int newDistance, int[] startIndices) {
			int count = distancesToNewCells.Count;
			for (int i = 0; i < count; i++) {
				List<int> distances = distancesToNewCells[i];
				int index = distances.IndexOf(oldDistance, startIndices[i]);
				if (index >= 0) {
					distances[index] = newDistance;
					startIndices[i] = index + 1;
					return;
				}
			}
		}
	}
	#endregion
	#region NormalizeTableGridAfterInsertCellHelper
	public class NormalizeTableGridAfterInsertCellHelper : NormalizeTableGridHelperBase {
		public NormalizeTableGridAfterInsertCellHelper(Table table, List<TableCell> cells, TableGridColumnCollection gridColumns, bool useDeltaBetweenColumns)
			: base(table, cells, gridColumns, useDeltaBetweenColumns) {
		}
		protected internal override int GetIndex(int index) {
			return index + 1;
		}
		protected internal override List<int> CalculateColumnSpansForNewCells(List<int> distances, TableCellParameters cellParameters) {
			List<int> result = new List<int>();
			int intervalsCount = distances.Count;
			int startColumnIndex = cellParameters.EndColumnIndex + 1;
			for (int i = 0; i < intervalsCount; i++) {
				ColumnIndexParameters parameters = CalculateColumnIndexParameters(distances[i]);
				int columnSpan;
				if (parameters.ColumnIndex < 0 && startColumnIndex > 0)
					columnSpan = (GridColumns.Count + 1) - startColumnIndex;
				else
					columnSpan = parameters.ColumnIndex - (startColumnIndex) + 1;
				result.Add(columnSpan);
				startColumnIndex = parameters.ColumnIndex;
				if (parameters.IsExactly)
					startColumnIndex++;
			}
			return result;
		}
	}
	#endregion
	#region NormalizeTableGridAfterDeleteCellHelper
	public class NormalizeTableGridAfterDeleteCellHelper : NormalizeTableGridHelperBase {
		public NormalizeTableGridAfterDeleteCellHelper(Table table, List<TableCell> cells, TableGridColumnCollection gridColumns, bool useDeltaBetweenColumns)
			: base(table, cells, gridColumns, useDeltaBetweenColumns) {
		}
		protected internal override int GetColumnIndex(int index, TableCell cell) {
			return index + cell.ColumnSpan;
		}
		protected internal override int CalculateDistance(int distanceToCell, int cellWidth) {
			return distanceToCell - cellWidth;
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Model.History {
	#region AddFieldHistoryItem
	public class AddFieldHistoryItem : RichEditFieldHistoryItem {
		#region Fields
		int insertedFieldIndex = -1;		
		RunIndex codeStartRunIndex;
		RunIndex codeEndRunIndex;
		RunIndex resultEndRunIndex;
		#endregion
		public AddFieldHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		#region Properties
		public RunIndex CodeStartRunIndex { get { return codeStartRunIndex; } set { codeStartRunIndex = value; } }
		public RunIndex CodeEndRunIndex { get { return codeEndRunIndex; } set { codeEndRunIndex = value; } }
		public RunIndex ResultEndRunIndex { get { return resultEndRunIndex; } set { resultEndRunIndex = value; } }
		public int InsertedFieldIndex { get { return insertedFieldIndex; } }
		#endregion
		protected override void RedoCore() {
			Field field = new Field(PieceTable);
			field.IsCodeView = true;
			field.Code.SetInterval(codeStartRunIndex, codeEndRunIndex);
			field.Result.SetInterval(codeEndRunIndex + 1, ResultEndRunIndex);
			insertedFieldIndex = PieceTable.GetInsertIndex(field);
			FieldCollection fields = PieceTable.Fields;
			if (insertedFieldIndex >= 0) {
				field.Parent = fields[insertedFieldIndex];
			}
			else {
				insertedFieldIndex = ~insertedFieldIndex;
				if (insertedFieldIndex < fields.Count) {
					Field parentField = fields[insertedFieldIndex];
					while (parentField != null) {
						if (parentField.FirstRunIndex < field.FirstRunIndex && parentField.LastRunIndex > field.LastRunIndex)
							break;
						parentField = parentField.Parent;
					}
					field.Parent = parentField;
				}
			}
			SetAsParentForAllChildren(field);
			if (ChildFieldIndices.Length > 0)
				insertedFieldIndex = ChildFieldIndices[ChildFieldIndices.Length - 1] + 1;
			field.Index = insertedFieldIndex;
			fields.Insert(insertedFieldIndex, field);
			DocumentModel.ApplyChanges(PieceTable, DocumentModelChangeType.Fields, field.FirstRunIndex, field.LastRunIndex);
			DocumentModelStructureChangedNotifier.NotifyFieldInserted(PieceTable, PieceTable, insertedFieldIndex);
		}
		protected override void UndoCore() {
			FieldCollection fields = PieceTable.Fields;
			Field removedField = fields[insertedFieldIndex];
			fields.RemoveAt(insertedFieldIndex);
			RemoveParentFromAllChildren(removedField);
			DocumentModel.ApplyChanges(PieceTable, DocumentModelChangeType.Fields, RunIndex.Zero, RunIndex.Zero);
			DocumentModelStructureChangedNotifier.NotifyFieldRemoved(PieceTable, PieceTable, insertedFieldIndex);
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Internal {
	#region ColumnIndexParameters
	public struct ColumnIndexParameters {
		#region Fields
		int columnIndex;
		bool isExect;
		#endregion
		public ColumnIndexParameters(int columnIndex, bool isExact) {
			this.columnIndex = columnIndex;
			this.isExect = isExact;
		}
		#region Properties
		public int ColumnIndex { get { return columnIndex; } set { columnIndex = value; } }
		public bool IsExactly { get { return isExect; } set { isExect = value; } }
		#endregion
	}
	#endregion
	#region TableCellParameters
	public struct TableCellParameters {
		#region Fields
		int indexInRow;
		int rowIndex;
		int startColumnIndex;
		int endColumnIndex;
		#endregion
		public TableCellParameters(int indexInRow, int rowIndex, int startColumnIndex, int endColumnIndex) {
			this.indexInRow = indexInRow;
			this.rowIndex = rowIndex;
			this.startColumnIndex = startColumnIndex;
			this.endColumnIndex = endColumnIndex;
		}
		#region Properties
		internal int IndexInRow { get { return indexInRow; } }
		internal int RowIndex { get { return rowIndex; } }
		internal int StartColumnIndex { get { return startColumnIndex; } }
		internal int EndColumnIndex { get { return endColumnIndex; } }
		#endregion
	}
	#endregion
	#region DistanceCollection
	public class DistanceCollection : List<int> {
	}
	#endregion
}
