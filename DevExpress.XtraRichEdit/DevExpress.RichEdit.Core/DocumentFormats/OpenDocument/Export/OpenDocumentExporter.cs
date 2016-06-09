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
using System.Xml;
using DevExpress.Utils;
using DevExpress.Utils.Zip;
using DevExpress.Office;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Export.OpenDocument;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Services.Implementation;
namespace DevExpress.XtraRichEdit.Export.OpenDocument {
	public partial class OpenDocumentTextExporter : XmlBasedDocumentModelExporter, IOpenDocumentTextExporter {
		#region Fields
		Stream outputStream;
		InternalZipArchive package;
		ExportHelper writeHelper;
		DocumentDataRegistrator dataRegistrator;
		int imageCounter;
		int sectionCounter;
		FieldsExporter fieldExporter;
		NumberingListExporter listExporter;
		readonly OpenDocumentExporterOptions options;
		Dictionary<OfficeImage, string> exportedImageTable;
		#endregion
		public OpenDocumentTextExporter(DocumentModel documentModel, OpenDocumentExporterOptions options)
			: base(documentModel) {
			Guard.ArgumentNotNull(options, "options");
			this.options = options;
		}
		#region Properties
		protected OpenDocumentExporterOptions Options { get { return options; } }
		protected internal Stream OutputStream { get { return outputStream; } }
		protected internal ExportHelper WriteHelper { get { return writeHelper; } }
		protected internal DocumentDataRegistrator DataRegistrator { get { return dataRegistrator; } }
		protected internal int ImageCounter { get { return imageCounter; } set { imageCounter = value; } }
		protected internal int SectionCounter { get { return sectionCounter; } set { sectionCounter = value; } }
		protected internal FieldsExporter FieldExporter { get { return fieldExporter; } }
		public NumberingListExporter ListExporter { get { return listExporter; } }
		public Dictionary<OfficeImage, string> ExportedImageTable { get { return exportedImageTable; } }
		protected internal override InternalZipArchive Package { get { return package; } }
		#endregion
		#region helper-methods
		protected internal string GetCharacterStyleNameByIndex(int index) {
			if (index < 0 || index >= DocumentModel.CharacterStyles.Count)
				return String.Empty;
			CharacterStyle style = DocumentModel.CharacterStyles[index];
			return style.StyleName;
		}
		protected internal string GetParagraphStyleNameByIndex(int index) {
			if (index < 0 || index >= DocumentModel.ParagraphStyles.Count)
				return String.Empty;
			ParagraphStyle style = DocumentModel.ParagraphStyles[index];
			return style.StyleName;
		}
		#endregion
		protected override DocumentModel PrepareModelForExport(DocumentModel documentModel) {
			DocumentModel model = documentModel.CreateDocumentModelForExport(emptyModel => { });
			IDocumentLayoutService service = documentModel.GetService<IDocumentLayoutService>();
			if(service != null)
				service.CreateService(model);
			FieldCalculatorService calculator = model.GetService<IFieldCalculatorService>() as FieldCalculatorService;
			if(object.ReferenceEquals(calculator, null))
				return model;
			PieceTable pieceTable = model.MainPieceTable;
			int i = 0;
			while(i < pieceTable.Fields.Count) {
				Field field = pieceTable.Fields[i];
				CalculatedFieldBase parsed = calculator.ParseField(pieceTable, field);
				if(IsFieldOfForbiddenType(parsed)) {
					RunInfo fieldInfo = pieceTable.GetFieldRunInfo(field);
					DocumentLogPosition startLogPosition = fieldInfo.NormalizedStart.LogPosition;					
					RunInfo resultInfo = pieceTable.GetFieldResultRunInfo(field);
					DocumentModel intermediate = new Model.DocumentModel(documentModel.DocumentFormatsDependencies) { IntermediateModel = true };
					intermediate.History.DisableHistory();
					PieceTable temp = intermediate.MainPieceTable;					
					model.BeginUpdate();					
					pieceTable.RemoveField(field);
					CopyHelper.CopyCore(pieceTable, temp, new DocumentLogInterval(resultInfo.NormalizedStart.LogPosition, resultInfo.NormalizedEnd.LogPosition - resultInfo.NormalizedStart.LogPosition + 1), DocumentLogPosition.Zero);
					pieceTable.DeleteContent(startLogPosition, fieldInfo.NormalizedEnd.LogPosition - startLogPosition + 1, false);					
					CopyHelper.CopyCore(temp, pieceTable, new DocumentLogInterval(temp.DocumentStartLogPosition, temp.DocumentEndLogPosition - temp.DocumentStartLogPosition), startLogPosition);
					model.EndUpdate();
					continue; 
				}
				i++;
			}
			return model;
		}
		bool IsFieldOfForbiddenType(CalculatedFieldBase field) {
			return field is TocField;
		}
		public virtual void Export(Stream outputStream) {
			if (outputStream == null)
				throw new InvalidOperationException();
			this.outputStream = outputStream;
			InitializeExport();
			base.Export();
		}
		protected internal virtual void InitializeExport() {
			this.imageCounter = 0;
			this.sectionCounter = 0;
			this.dataRegistrator = CreateDataRegistrator();
			this.writeHelper = CreateWriteExportHelper();
			this.fieldExporter = CreateFieldExporter();
			this.listExporter = CreateListExporter();
			this.exportedImageTable = new Dictionary<OfficeImage, string>();
		}
		protected internal override bool ShouldSplitRuns() {
			return true;
		}
		protected internal override void SplitRuns() {
			SplitTextRunsByBreaks();
			base.SplitRuns();
		}
		protected internal virtual void PerformPreExport() {
			DataRegistrator.ExportDocument();
		}
		protected internal override void ExportDocument() {
			PerformPreExport();
			using (InternalZipArchive documentPackage = new InternalZipArchive(outputStream)) {
				this.package = documentPackage;
				AddCompressedPackageContent(@"content.xml", ExportDocumentContent());
				AddCompressedPackageContent(@"styles.xml", ExportDocumentStyles());
				AddCompressedPackageContent(@"meta.xml", ExportDocumentMeta());
				AddCompressedPackageContent(@"META-INF/manifest.xml", ExportDocumentManifest());
			}
		}
		protected internal virtual void SplitTextRunsByBreaks() {
			DocumentLogPosition currentPos = new DocumentLogPosition(0);
			RunIndex currentRun = new RunIndex();
			while (currentRun < new RunIndex(PieceTable.Runs.Count) - 1) {
				currentPos = DocumentModelPosition.FromRunStart(PieceTable, currentRun).LogPosition;
				if (PieceTable.Runs[currentRun] is TextRun)
					SplitTextRunsByBreaksCore(currentPos, currentRun);
				currentRun++;
			}
		}
		void SplitTextRunsByBreaksCore(DocumentLogPosition currentPos, RunIndex currentRun) {
			string content = PieceTable.GetRunText(currentRun);
			char[] symbols = new char[2];
			symbols[0] = Characters.PageBreak;
			symbols[1] = Characters.ColumnBreak;
			int indexLineBreak = content.IndexOfAny(symbols);
			if ((indexLineBreak == content.Length - 1) && (PieceTable.Runs[currentRun + 1] is ParagraphRun))
				return;
			if (indexLineBreak != -1) {
				PieceTable.InsertParagraph(currentPos + indexLineBreak + 1);
			}
		}
		protected virtual ExportHelper CreateWriteExportHelper() {
			return new ExportHelper(this );
		}
		protected virtual DocumentDataRegistrator CreateDataRegistrator() {
			return new DocumentDataRegistrator(DocumentModel);
		}
		protected internal virtual FieldsExporter CreateFieldExporter() {
			return new FieldsExporter(WriteHelper);
		}
		protected internal virtual NumberingListExporter CreateListExporter() {
			return new NumberingListExporter(DataRegistrator.ListRegistrator, WriteHelper);
		}
		protected internal override void BeforeCreateXmlContent(XmlWriter writer) {
			base.BeforeCreateXmlContent(writer);
			WriteHelper.UpdateWriter(writer);
		}
		#region Document Manifest
		protected internal virtual CompressedStream ExportDocumentManifest() {
			return CreateCompressedXmlContent(GenerateDocumentManifest, new UTF8Encoding(false));
		}
		protected internal virtual void GenerateDocumentManifest(XmlWriter writer) {
			WriteHelper.WriteDocumentManifest(writer);
		}
		#endregion
		protected internal virtual CompressedStream ExportDocumentContent() {
			return CreateCompressedXmlContent(GenerateDocumentXmlContent);
		}
		protected internal virtual CompressedStream ExportDocumentMeta() {
			return CreateCompressedXmlContent(GenerateDocumentXmlMeta);
		}
		protected internal virtual void GenerateDocumentXmlContent(XmlWriter writer) {
			GenerateDocumentContent();
		}
		protected internal virtual void GenerateDocumentXmlMeta(XmlWriter writer) {
			WriteHelper.WriteDocumentContentStart();
			try {
				WriteHelper.WriteOfficeStartElement("meta");
				WriteHelper.WriteEndElement();
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		#region Document Content
		protected internal virtual void GenerateDocumentContent() {
			WriteHelper.WriteDocumentContentStart();
			try {
				ExportFontDeclarations();
				ExportContentAutoStyles();
				UpdateFields();
				WriteHelper.WriteBodyStart();
				try {
					WriteHelper.WriteTextStart();
					try {
						base.ExportDocument();
						ExportDocumentVariables();
					}
					finally {
						WriteHelper.WriteEndElement();
					}
				}
				finally {
					WriteHelper.WriteEndElement();
				}
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		#endregion
		void UpdateFields() {
			DocumentModel.BeginUpdate();
			try {
				PieceTable.FieldUpdater.UpdateFields(UpdateFieldOperationType.Normal);			   
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal virtual void ExportDocumentVariables() {
			if (DocumentModel.Variables.Count == 0)
				return;
			WriteHelper.WriteUserFieldDecls();
			try {
				ExportDocumentVariablesCore();
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		protected internal virtual void ExportDocumentVariablesCore() {
			DocumentVariableCollection variables = DocumentModel.Variables;
			foreach (string name in variables.GetVariableNames()) {
				WriteHelper.WriteUserFieldDecl();
				try {
					WriteHelper.WriteOfficeStringAttribute("value-type", "string");
					object value = variables[name];
					if(value == null || value == DocVariableValue.Current)
						value = String.Empty;
					WriteHelper.WriteOfficeStringAttribute("string-value", StringHelper.RemoveSpecialSymbols(value.ToString()));
					WriteHelper.WriteTextStringAttribute("name", name);
				}
				finally {
					WriteHelper.WriteEndElement();
				}
			}
		}
		#region Content Auto Styles
		protected internal virtual void ExportContentAutoStyles() {
			WriteHelper.WriteAutoStylesStart();
			try {
				ExportSectionStyles();
				ExportNumberingListsStyles();
				ExportCharacterAutoStyles(true);
				ExportPictureAutoStyles(true);
				ExportFloatingObjectAutoStyles(true);
				ExportParagraphAutoStyles(true);
				ExportTableAutoStyles(true);
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		#region Section Style
		protected internal virtual void ExportSectionStyles() {
			SectionCollection sections = DocumentModel.Sections;
			int count = sections.Count;
			for (int i = 0; i < count; i++) {
				Section section = sections[new SectionIndex(i)];
				if (ShouldExportSectionStyle(section)) {
					string styleName = NameResolver.CalculateSectionAutoStyleName(i);
					ExportSectionStyle(section, styleName);
				}
			}
		}
		bool ShouldExportSectionStyle(Section section) {
			return DocumentModel.DocumentCapabilities.SectionsAllowed;
		}
		protected internal virtual void ExportSectionStyle(Section section, string styleName) {
			WriteHelper.WriteStyleStart();
			try {
				WriteHelper.WriteSectionAutoStyleAttributes(styleName);
				ExportSectionStyleProperties(section);
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		protected internal virtual void ExportSectionStyleProperties(Section section) {
			WriteHelper.WriteSectionPropertiesStart();
			try {
				ExportSectionStyleColumns(section);
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		protected internal virtual void ExportSectionStyleColumns(Section section) {
			SectionColumns columns = section.Columns;
			if (!DocumentFormatsHelper.ShouldExportSectionColumns(columns, DocumentModel))
				return;
			WriteHelper.WriteColumnsStyleStart();
			try {
				WriteHelper.WriteColumnsStyleAttributes(columns);
				if (columns.DrawVerticalSeparator)
					ExportColumnSeparator();
				if (!columns.EqualWidthColumns)
					ExportNonUniformColumns(section);
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		protected internal virtual void ExportColumnSeparator() {
			WriteHelper.WriteColumnSeparatorStyleStart();
			try {
				WriteHelper.WriteColumnSeparatorAttributes();
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		protected internal virtual void ExportNonUniformColumns(Section section) {
			int sectionWidth = section.Page.Width - section.Margins.Right - section.Margins.Left;
			ColumnInfoCollection columnInfos = section.Columns.GetColumns();
			int count = columnInfos.Count;
			int prevColumnSpace = 0;
			for (int i = 0; i < count; i++) {
				ColumnInfo info = columnInfos[i];
				ExportColumn(info, prevColumnSpace, sectionWidth);
				prevColumnSpace = info.Space;
			}
		}
		protected internal virtual void ExportColumn(ColumnInfo column, int prevColumnSpace, int sectionWidth) {
			WriteHelper.WriteColumnStyleStart();
			try {
				float spaceBefore = (float)prevColumnSpace / 2;
				float spaceAfter = (float)column.Space / 2;
				float columnWidth = spaceAfter + spaceBefore + column.Width;
				int relWidth = (int)((columnWidth * WriteHelper.ColumnWeightBase) / sectionWidth);
				WriteHelper.WriteColumnStyleAttributes(relWidth, spaceBefore, spaceAfter);
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		#endregion
		#region Numbering List Auto Styles
		protected internal virtual void ExportNumberingListsStyles() {
			NumberingListCollection lists = DocumentModel.NumberingLists;
			int count = lists.Count;
			for (int i = 0; i < count; i++)
				ExportNumberingListStyle(lists[new NumberingListIndex(i)], i);
		}
		protected internal virtual void ExportNumberingListStyle(NumberingList list, int index) {
			WriteHelper.WriteListStyleStart();
			try {
				string styleName = NameResolver.CalculateNumberingListStyleName(index);
				WriteHelper.WriteListStyleAttributes(styleName);
				ExportNumberingListLevelStyle(list);
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		private void ExportNumberingListLevelStyle(NumberingList list) {
			ListLevelCollection<IOverrideListLevel> levels = list.Levels;
			int count = levels.Count;
			for (int i = 0; i < count; i++)
				ExportListLevelStyle(levels[i], i, NumberingListHelper.GetLevelType(list, i));
		}
		protected internal virtual void ExportListLevelStyle(IListLevel level, int index, NumberingType type) {
			if (type == NumberingType.Bullet)
				WriteHelper.WriteListLevelStyleBulletStart();
			else
				WriteHelper.WriteListLevelStyleNumberStart();
			try {
				string styleName = DataRegistrator.CharacterAutoStyles.GetStyleName(level.CharacterProperties);
				WriteHelper.WriteListLevelStyleCommonAttributes(index, styleName);
				if (type == NumberingType.Bullet)
					WriteHelper.WriteListLevelStyleBulletAttributes(level, index, styleName);
				else
					WriteHelper.WriteListLevelStyleNumberAttributes(level, index, styleName);
				ExportNumberingListLevelStyleProperties(level, index);
				if (type == NumberingType.Bullet)
					ExportNumberingListLevelTextProperties(level);
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		private void ExportNumberingListLevelTextProperties(IListLevel level) {
			WriteHelper.WriteStyleTextPropertiesStart();
			try {
				WriteHelper.WriteListLevelTextPropertiesAttributes(level.CharacterProperties.FontName);
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		private void ExportNumberingListLevelStyleProperties(IListLevel level, int index) {
			WriteHelper.WriteListLevelStylePropertiesStart();
			try {
				WriteHelper.WriteListLevelStylePropertiesAttributes(level, index);
				WriteHelper.WriteListLevelLabelAlignmentProperties(level);
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		#endregion
		protected internal virtual void ExportParagraphAutoStyles(bool isAutoStyleForMainDocument) {
			ParagraphStyleInfoTable styles = DataRegistrator.ParagraphAutoStyles;
			foreach (Paragraph key in styles.Keys) {
				ParagraphStyleInfo style = styles[key];
				if (style.IsStyleUsedInMainDocument == isAutoStyleForMainDocument)
					ExportParagraphAutoStyle(key, style);
			}
		}
		private void ExportParagraphAutoStyle(Paragraph paragraph, ParagraphStyleInfo info) {
			WriteHelper.WriteStyleStart();
			try {
				WriteHelper.WriteParagraphAutoStyleAttributes(paragraph, info);
				ExportAutoStyleParagraphProperties(paragraph, info.BreakAfterType);
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		protected virtual void ExportAutoStyleParagraphProperties(Paragraph paragraph, ParagraphBreakType paragraphBreakAfterType) {
			ExportAutoStyleParagraphPropertiesCore(paragraph, paragraphBreakAfterType);
			if (paragraph.IsEmpty)
				ExportParagraphRunProperties(paragraph);
		}
		void ExportAutoStyleParagraphPropertiesCore(Paragraph paragraph, ParagraphBreakType paragraphBreakAfterType) {
			bool shouldExportProperties = DataRegistrator.ShouldExportParagraphProperties(paragraph);
			bool shouldExportLineNumbering = DataRegistrator.ParagraphsLineNumber.ContainsKey(paragraph.Index)
				&& DataRegistrator.LineNumbersIncrement > 0;
			if (!shouldExportProperties && paragraphBreakAfterType == ParagraphBreakType.None && (!shouldExportLineNumbering))
				return;
			WriteHelper.WriteStyleParagraphPropertiesStart();
			try {
				WriteHelper.WriteStyleParagraphPropertiesAttributes(paragraph.ParagraphProperties, paragraphBreakAfterType);
				WriteHelper.WriteStyleParagraphPropertiesLineNumber(paragraph.ParagraphProperties, paragraph.Index, true);
				WriteHelper.WriteStyleParagraphPropertiesContent(paragraph.GetOwnTabs());
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		void ExportParagraphRunProperties(Paragraph paragraph) {
			CharacterProperties cp = paragraph.PieceTable.Runs[paragraph.LastRunIndex].CharacterProperties;
			if(cp.Info.Options.Value != CharacterFormattingOptions.Mask.UseNone) {
				WriteHelper.WriteStyleTextPropertiesStart();
				try {
					WriteHelper.WriteStyleTextPropertiesAttributes(cp);
				}
				finally {
					WriteHelper.WriteEndElement();
				}
			}
		}
		protected internal virtual void ExportCharacterAutoStyles(bool isAutoStyleForMainDocument) {
			CharacterStyleInfoTable styles = DataRegistrator.CharacterAutoStyles;
			foreach (StyleKey key in styles.Keys) {
				CharacterStyleInfo info = styles[key];
				if (info.IsStyleUsedInMainDocument == isAutoStyleForMainDocument)
					ExportCharacterAutoStyle(key, info);
			}
		}
		protected internal virtual void ExportCharacterAutoStyle(StyleKey styleKey, CharacterStyleInfo styleInfo) {
			WriteHelper.WriteStyleStart();
			try {
				string parentStyleName = GetCharacterStyleNameByIndex(styleKey.StyleIndex);
				WriteHelper.WriteCharacterAutoStyleAttributes(styleInfo.Name, parentStyleName);
				ExportStyleTextProperties(styleInfo.CharacterProperties);
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		protected internal virtual void ExportPictureAutoStyles(bool isAutoStyleForMainDocument) {
			PictureStyleInfoTable styles = DataRegistrator.PictureAutoStyles;
			foreach (StyleKey key in styles.Keys) {
				PictureStyleInfo info = styles[key];
				if (info.IsStyleUsedInMainDocument == isAutoStyleForMainDocument)
					ExportPictureAutoStyle(info.Name, info.PictureProperties);
			}
		}
		protected internal virtual void ExportFloatingObjectAutoStyles(bool isAutoStyleForMainDocument) {
			FloatingObjectStyleInfoTable styles = DataRegistrator.FloatingObjectAutoStyles;
			foreach (StyleKey key in styles.Keys) {
				FloatingObjectStyleInfo info = styles[key];
				if (info.IsStyleUsedInMainDocument == isAutoStyleForMainDocument)
					ExportFloatingObjectAutoStyle(info.Name, info);
			}
		}
		protected internal virtual void ExportFloatingObjectAutoStyle(string styleName, FloatingObjectStyleInfo info) {
			WriteHelper.WriteStyleStart();
			try {
				WriteHelper.WriteGraphicStyleContent(styleName);
				ExportGraphicStyleProperties(info);
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		protected internal virtual void ExportPictureAutoStyle(string styleName, InlinePictureProperties properties) {
			WriteHelper.WriteStyleStart();
			try {
				WriteHelper.WriteGraphicStyleContent(styleName);
				ExportGraphicStyleProperties(properties);
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		#endregion
		protected virtual void ExportGraphicStyleProperties(InlinePictureProperties properties) {
			WriteHelper.WriteGraphicStylePropertiesStart();
			try {
				WriteHelper.WriteInlinePictureFrameStyleContent(properties);
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		protected virtual void ExportGraphicStyleProperties(FloatingObjectStyleInfo info) {
			WriteHelper.WriteGraphicStylePropertiesStart();
			try {
				WriteHelper.WriteFloatingObjectFrameStyleContent(info.FloatingObjectProperties);
				WriteHelper.WriteShapeFrameStyleContent(info.Shape, info.TextBoxProperties != null);
				if (info.TextBoxProperties != null)
					WriteHelper.WriteTextBoxPropertiesFrameStyleContent(info.TextBoxProperties);
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		#region Styles.xml
		protected internal virtual CompressedStream ExportDocumentStyles() {
			return CreateCompressedXmlContent(GenerateStylesXmlContent);
		}
		protected internal virtual void GenerateStylesXmlContent(XmlWriter writer) {
			GenerateStylesContent();
		}
		protected internal virtual void GenerateStylesContent() {
			WriteHelper.WriteDocumentStylesStart();
			try {
				WriteHelper.WriteDocumentStylesAttributes();
				ExportFontDeclarations();
				ExportDocumentCommonStyles();
				ExportDocumentAutoStyles();
				ExportDocumentMasterStyles();
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		#region Styles.xml - Font Declaration
		protected internal virtual void ExportFontDeclarations() {
			WriteHelper.WriteFontDeclarationStart();
			try {
				List<string> fontNames = DataRegistrator.FontNames;
				for (int i = 0; i < fontNames.Count; i++) {
					WriteHelper.WriteFontDeclaration(fontNames[i]);
				}
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		#endregion
		#region Styles.xml - Common Styles
		protected internal virtual void ExportDocumentCommonStyles() {
			WriteHelper.WriteDocumentCommonStylesStart();
			try {
				ExportDefaultProperties();
				ExportDocumentCharacterStyles();
				ExportDocumentParagraphStyles();
				ExportLineNumberingConfiguration();
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		void ExportDefaultProperties() {
			ExportDefaultCharacterAndParagraphProperties();
			ExportDefaultTableProperties();
		}
		private void ExportDefaultTableProperties() {
			WriteHelper.WriteDefaultTableStyleStart();
			try {
				WriteHelper.WriteStyleTablePropertiesStart();
				try {
					WriteHelper.WriteStyleTablePropertiesAttributes(DocumentModel.DefaultTableProperties, Int32.MinValue);
				}
				finally {
					WriteHelper.WriteEndElement();
				}
				ExportStyleTextProperties(DocumentModel.DefaultCharacterProperties);
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		void ExportDefaultCharacterAndParagraphProperties() {
			WriteHelper.WriteDefaultParagraphStyleStart();
			try {
				WriteHelper.WriteStyleParagraphPropertiesStart();
				try {
					ExportDocumentStyleParagraphPropertiesCore(DocumentModel.DefaultParagraphProperties);
				}
				finally {
					WriteHelper.WriteEndElement();
				}
				ExportStyleTextProperties(DocumentModel.DefaultCharacterProperties);
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		protected internal virtual void ExportDocumentParagraphStyles() {
			ParagraphStyleCollection styles = DocumentModel.ParagraphStyles;
			int count = styles.Count;
			for (int i = 0; i < count; i++)
				ExportDocumentParagraphStyle(styles[i]);
		}
		protected internal virtual void ExportLineNumberingConfiguration() {
			if (!ShoudExportLineNumbering())
				return;
			WriteHelper.WriteLineNumberingStart();
			try {
				WriteHelper.WriteLineNumberingAttributes();
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		protected internal virtual bool ShoudExportLineNumbering() {
			return true;
		}
		protected internal virtual void ExportDocumentParagraphStyle(ParagraphStyle style) {
			if (style.Deleted)
				return;
			WriteHelper.WriteStyleStart();
			try {
				WriteHelper.WriteDocumentParagraphStyleAttributes(style);
				ExportDocumentStyleParagraphProperties(style);
				ExportStyleTextProperties(style.CharacterProperties);
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		protected internal virtual void ExportDocumentStyleParagraphProperties(ParagraphStyle style) {
			if (!DataRegistrator.ShouldExportParagraphProperties(style))
				return;
			WriteHelper.WriteStyleParagraphPropertiesStart();
			try {
				ExportDocumentStyleParagraphPropertiesCore(style.ParagraphProperties);
				WriteHelper.WriteStyleParagraphPropertiesContent(style.Tabs.GetTabs());				
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		protected internal virtual void ExportDocumentStyleParagraphPropertiesCore(ParagraphProperties properties) {
			WriteHelper.WriteStyleParagraphPropertiesAttributes(properties, ParagraphBreakType.None);
			WriteHelper.WriteStyleParagraphPropertiesLineNumber(properties, ParagraphIndex.Zero, false);
		}
		protected internal virtual void ExportDocumentCharacterStyles() {
			CharacterStyleCollection styles = DocumentModel.CharacterStyles;
			int count = styles.Count;
			for (int i = 0; i < count; i++)
				ExportDocumentCharacterStyle(styles[i]);
		}
		protected internal virtual void ExportDocumentCharacterStyle(CharacterStyle style) {
			if (style.Deleted)
				return;
			WriteHelper.WriteStyleStart();
			try {
				WriteHelper.WriteDocumentCharacterStyleAttributes(style);
				ExportStyleTextProperties(style.CharacterProperties);
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		protected internal virtual void ExportStyleTextProperties(CharacterProperties properties) {
			if (!DataRegistrator.ShouldExportCharacterProperties(properties))
				return;
			WriteHelper.WriteStyleTextPropertiesStart();
			try {
				WriteHelper.WriteStyleTextPropertiesAttributes(properties);
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		#endregion
		#region Styles.xml - Page Layout Styles
		protected internal virtual void ExportDocumentAutoStyles() {
			WriteHelper.WriteAutoStylesStart();
			try {
				ExportPageLayoutStyles();
				ExportNumberingListsStyles();
				ExportCharacterAutoStyles(false);
				ExportPictureAutoStyles(false);
				ExportFloatingObjectAutoStyles(false);
				ExportParagraphAutoStyles(false);
				ExportTableAutoStyles(false);
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		protected internal virtual void ExportPageLayoutStyles() {
			int count = DataRegistrator.SectionPages.Count;
			for (int i = 0; i < count; i++)
				ExportPageLayoutStyle(DataRegistrator.SectionPages[i], i);
		}
		protected virtual void ExportPageLayoutStyle(Section section, int pageIndex) {
			WriteHelper.WritePageLayoutStart();
			try {
				string styleName = NameResolver.CalculatePageLayoutName(pageIndex);
				WriteHelper.WritePageLayoutAttributes(styleName);
				ExportPageLayoutProperties(section);
				ExportPageLayoutHeaderStyle(section);
				ExportPageLayoutFooterStyle(section);
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		protected virtual void ExportPageLayoutProperties(Section section) {
			WriteHelper.WritePageLayoutPropertiesStart();
			try {
				WriteHelper.WritePageLayoutPropertiesAttributes(section);
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		protected virtual void ExportPageLayoutHeaderStyle(Section section) {
			if (!ShouldExportHeaders(section))
				return;
			WriteHelper.WriteHeaderStyleStart();
			try {
				ExportHeaderProperties(section);
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		protected internal virtual bool ShouldExportHeaders(Section section) {
			bool nonEmptyHeaders = section.InnerOddPageHeader != null
				|| section.InnerFirstPageHeader != null
				|| section.InnerEvenPageHeader != null;
			return nonEmptyHeaders && DocumentModel.DocumentCapabilities.HeadersFootersAllowed;
		}
		protected virtual void ExportHeaderProperties(Section section) {
			WriteHelper.WriteHeaderFooterPropertiesStart();
			try {
				WriteHelper.WriteHeaderPropertiesAttributes(section);
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		protected internal virtual void ExportPageLayoutFooterStyle(Section section) {
			if (!ShouldExportFooters(section))
				return;
			WriteHelper.WriteFooterStyleStart();
			try {
				ExportFooterProperties(section);
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		protected internal virtual bool ShouldExportFooters(Section section) {
			bool nonEmptyFooters = section.InnerOddPageFooter != null
				|| section.InnerFirstPageFooter != null
				|| section.InnerEvenPageFooter != null;
			return nonEmptyFooters && DocumentModel.DocumentCapabilities.HeadersFootersAllowed;
		}
		protected virtual void ExportFooterProperties(Section section) {
			WriteHelper.WriteHeaderFooterPropertiesStart();
			try {
				WriteHelper.WriteFooterPropertiesAttributes(section);
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		#endregion
		#region Styles.xml - MasterPage Styles
		protected internal virtual void ExportDocumentMasterStyles() {
			WriteHelper.WriteMasterStylesStart();
			try {
				ExportMasterPageStyles();
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		protected internal virtual void ExportMasterPageStyles() {
			int count = DataRegistrator.SectionPages.Count;
			for (int index = 0; index < count; index++) {
				string masterPageName = NameResolver.CalculateMasterPageName(index);
				string pageLayoutName = NameResolver.CalculatePageLayoutName(index);
				ExportMasterPageStyle(index, masterPageName, pageLayoutName);
				if (DataRegistrator.SectionPages[index].GeneralSettings.DifferentFirstPage
					&& DocumentModel.DocumentCapabilities.HeadersFootersAllowed) {
					string firstPageMasterPageName = NameResolver.CalculateMasterPageNameForFirstPageHeaderFooter(index);
					ExportMasterFirstPageStyle(index, firstPageMasterPageName, pageLayoutName, masterPageName);
				}
			}
		}
		public void ExportMasterPageStyle(int sectionPagesIndex, string masterPageName, string pageLayoutName) {
			WriteHelper.WriteMasterPageStart();
			try {
				WriteHelper.WriteMasterPageAttributes(masterPageName, pageLayoutName);
				ExportSectionHeaderFootersContent(sectionPagesIndex, masterPageName, pageLayoutName);
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		public void ExportMasterFirstPageStyle(int sectionPagesIndex, string masterPageName, string pageLayoutName, string nextMasterPageName) {
			WriteHelper.WriteMasterPageStart();
			try {
				WriteHelper.WriteMasterPageAttributes(masterPageName, pageLayoutName, nextMasterPageName);
				ExportSectionFirstPageHeaderFootersContent(sectionPagesIndex);
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		#region Export Headers and Footers Content
		protected internal virtual void ExportSectionFirstPageHeaderFootersContent(int sectionPagesIndex) {
			if (!DocumentModel.DocumentCapabilities.HeadersFootersAllowed)
				return;
			Section section = DataRegistrator.SectionPages[sectionPagesIndex];
			ExportHeaderFooter("header", section.FirstPageHeader);
			ExportHeaderFooter("footer", section.FirstPageFooter);
		}
		protected internal virtual void ExportSectionHeaderFootersContent(int sectionPagesIndex, string masterPageName, string pageLayoutName) {
			if (!DocumentModel.DocumentCapabilities.HeadersFootersAllowed)
				return;
			Section section = DataRegistrator.SectionPages[sectionPagesIndex];
			if (section.DocumentModel.DocumentProperties.DifferentOddAndEvenPages)
				ExportSectionHeaderFootersContentCore_Different(section);
			else
				ExportSectionHeaderFootersContentCore_Same(section);
		}
		void ExportSectionHeaderFootersContentCore_Same(Section section) {
			ExportHeaderFooter("header", section.OddPageHeader);
			ExportHeaderFooter("footer", section.OddPageFooter);
		}
		void ExportSectionHeaderFootersContentCore_Different(Section section) {
			ExportHeaderFooter("header", section.OddPageHeader);
			if (section.InnerOddPageHeader == null && section.InnerEvenPageHeader != null)
				ExportEmptyHeaderFooter("header");
			ExportHeaderFooter("header-left", section.EvenPageHeader);
			if (section.InnerEvenPageHeader == null && section.InnerOddPageHeader != null)
				ExportEmptyHeaderFooter("header-left");
			ExportHeaderFooter("footer", section.OddPageFooter);
			if (section.InnerOddPageFooter == null && section.InnerEvenPageFooter != null)
				ExportEmptyHeaderFooter("footer");
			ExportHeaderFooter("footer-left", section.EvenPageFooter);
			if (section.InnerEvenPageFooter == null && section.InnerOddPageFooter != null)
				ExportEmptyHeaderFooter("footer-left");
		}
		protected internal virtual void ExportHeaderFooter(string tag, SectionHeaderFooterBase header) {
			if (header == null)
				return;
			WriteHelper.WriteStyleStartElement(tag);
			try {
				PieceTable pieceTable = header.PieceTable;
				PerformExportPieceTable(pieceTable, ExportHeaderFooterContent);
				ListExporter.EndHeaderFooterExport(pieceTable, WriteHelper);
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		protected internal virtual void ExportHeaderFooterContent() {
			ExportParagraphs(PieceTable.Paragraphs.First.Index, PieceTable.Paragraphs.Last.Index);
		}
		void ExportEmptyHeaderFooter(string tag) {
			WriteHelper.WriteStyleStartElement(tag);
			try {
				WriteHelper.WriteParagraphStart(false);
				WriteHelper.WriteEndElement();
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		#endregion
		#endregion
		#endregion
		#region DocumentModelExporter methods
		#region TextRun
		protected internal override void ExportTextRun(TextRun run) {
			if (FieldExporter.FieldClosed)
				ExportTextRunCore(run);
			else
				FieldExporter.ExportTextRun(run);
		}
		protected internal virtual void ExportTextRunCore(TextRun run) {
			WriteHelper.WriteTextRunStart();
			try {
				string styleName = GetTextRunStyleName(run);
				WriteHelper.WriteTextRunAttributes(styleName);
				WriteHelper.WriteTextRunContent(run);
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		#endregion
		#region ExportParagraphRun
		protected internal override void ExportParagraphRun(ParagraphRun run) {
			base.ExportParagraphRun(run);
		}
		#endregion
		#region FloatingObjectAnchor
		void ExportFloatingObjectPicture(FloatingObjectAnchorRun run, PictureFloatingObjectContent content) {
			WriteHelper.WriteFloatingObjectAnchorFrameStart();
			try {
				WriteFloatingObjectPictureContent(content, run.Shape, run.FloatingObjectProperties, GenerteFloatingObjectName(run), GenerateImageID());
			}
			finally {
				ImageCounter++;
				WriteHelper.WriteEndElement();
			}
		}
		void ExportFloatingObjectTextBox(FloatingObjectAnchorRun run, TextBoxFloatingObjectContent content) {
			WriteHelper.WriteFloatingObjectAnchorFrameStart();
			try {
				WriteFloatingObjectTextBoxContent(content, run.Shape, run.FloatingObjectProperties, GenerteFloatingObjectName(run));
			}
			finally {
				ImageCounter++;
				WriteHelper.WriteEndElement();
			}
		}
		string GenerteFloatingObjectName(FloatingObjectAnchorRun run) {
			if (!String.IsNullOrEmpty(run.Name))
				return run.Name;
			else {
				string imageID = GenerateImageID();
				string name = (run.Content as PictureFloatingObjectContent != null ? "graphics" : "Text Box ");
				return NameResolver.CalculateFloatingObjectFrameName(name, imageID);
			}
		}
		protected internal override void ExportFloatingObjectAnchorRun(FloatingObjectAnchorRun run) {
			if (FieldExporter.FieldClosed)
				ExportFloatingObjectAnchorRunCore(run);
			else
				FieldExporter.ExportFloatingObjectAnchorRun(run);
		}
		protected internal virtual void ExportFloatingObjectAnchorRunCore(FloatingObjectAnchorRun run) {
			PictureFloatingObjectContent pictureContent = run.Content as PictureFloatingObjectContent;
			if (pictureContent != null)
				ExportFloatingObjectPicture(run, pictureContent);
			TextBoxFloatingObjectContent textBoxContent = run.Content as TextBoxFloatingObjectContent;
			if (textBoxContent != null)
				ExportFloatingObjectTextBox(run, textBoxContent);
		}
		#endregion
		#region InlinePicture
		protected internal override void ExportInlinePictureRun(InlinePictureRun run) {
			if (FieldExporter.FieldClosed)
				ExportInlinePictureRunCore(run);
			else
				FieldExporter.ExportInlinePictureRun(run);
		}
		protected internal virtual void ExportInlinePictureRunCore(InlinePictureRun run) {
			WriteHelper.WriteInlinePictureFrameStart();
			try {
				WriteInlinePictureFrameContent(run);
			}
			finally {
				ImageCounter++;
				WriteHelper.WriteEndElement();
			}
		}
		#endregion
		#region Paragraph
		protected internal override void ExportSection(Section section) {
			if (ShouldExportSectionStyle(section)) {
				WriteHelper.WriteSectionStart();
				try {
					ExportSectionAttributes(section);
					base.ExportSection(section);
					ListExporter.EndSectionExport(section, WriteHelper);
				}
				finally {
					SectionCounter++;
					WriteHelper.WriteEndElement();
				}
			}
			else {
				base.ExportSection(section);
				SectionCounter++;
			}
		}
		protected internal virtual void ExportSectionAttributes(Section section) {
			string styleName = NameResolver.CalculateSectionAutoStyleName(SectionCounter);
			string sectionName = NameResolver.CalculateSectionName(SectionCounter);
			bool isProtected = CalculateIsSectionProtected(section);
			WriteHelper.WriteSectionAttributes(styleName, sectionName, isProtected);
		}
		protected bool CalculateIsSectionProtected(Section section) {
			if (!DocumentModel.IsDocumentProtectionEnabled)
				return false;
			return !DataRegistrator.EditableSections.Contains(section);
		}
		protected internal override void ExportSectionHeadersFooters(Section section) {
		}
		protected internal override ParagraphIndex ExportParagraph(Paragraph paragraph) {
			if (ListExporter.ReadyForProcess) {
				ListExporter.ExportParagraph(paragraph);
			}
			return ExportParagraphCore(paragraph);
		}
		protected internal virtual ParagraphIndex ExportParagraphCore(Paragraph paragraph) {
			bool isParagraphHeading = IsParagraphHeading(paragraph);
			WriteHelper.WriteParagraphStart(isParagraphHeading);
			try {
				string styleName = GetParagraphAutoStyleName(paragraph);
				if (!String.IsNullOrEmpty(styleName) || isParagraphHeading) {
					WriteHelper.WriteParagraphAttributes(styleName, paragraph.ParagraphProperties.OutlineLevel);
				}
				return base.ExportParagraph(paragraph);
			}
			finally {
				if (!FieldExporter.FieldClosed) 
					FieldExporter.CloseFieldOnParagraphClose();
				WriteHelper.WriteEndElement();
			}
		}
		bool IsParagraphHeading(Paragraph paragraph) {
			int level = paragraph.ParagraphProperties.OutlineLevel;
			return WriteHelper.ShouldExportParagraphOutlineLevel(level);
		}
		internal virtual string GetParagraphAutoStyleName(Paragraph paragraph) {
			if (DataRegistrator.ParagraphAutoStyles.ContainsKey(paragraph)) {
				ParagraphStyleInfo info = DataRegistrator.ParagraphAutoStyles[paragraph];
				return info.Name;
			}
			return String.Empty;
		}
		#endregion
		#region Fields
		protected internal override void ExportFieldCodeStartRun(FieldCodeStartRun run) {
			FieldExporter.ExportCodeStartRun(run);
		}
		protected internal override void ExportFieldCodeEndRun(FieldCodeEndRun run) {
			FieldExporter.ExportFieldCodeEndRun();
		}
		protected internal override void ExportFieldResultEndRun(FieldResultEndRun run) {
			FieldExporter.ExportFieldResultEndRun();
		}
		#endregion
		#region Export Table
		protected internal override ParagraphIndex ExportTable(TableInfo tableInfo) {
			if (ListExporter.ReadyForProcess) {
				ListExporter.ExportTableStart(tableInfo.Table);
			}
			if (!FieldExporter.FieldClosed) {
			}
			return ExportTableCore(tableInfo);
		}
		ParagraphIndex ExportTableCore(TableInfo tableInfo) {
			WriteHelper.WriteTableStart();
			try {
				ExportTableAttributes(tableInfo);
				ExportTableColumns(tableInfo);
				return base.ExportTable(tableInfo);
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		protected internal virtual void ExportTableAttributes(TableInfo tableInfo) {
			WriteHelper.WriteTableAttributes(DataRegistrator.TableStyleInfoTable.GetName(tableInfo.Table));
		}
		protected internal virtual void ExportTableColumns(TableInfo tableInfo) {
			if (DataRegistrator.TableColumnsInfoTable.ContainsKey(tableInfo.Table)) {
				OpenDocumentTableColumnsInfo columns = DataRegistrator.TableColumnsInfoTable[tableInfo.Table];
				WriteHelper.WriteTableColumns(columns);
			}
		}
		protected internal override void ExportRow(TableRow row, TableInfo tableInfo) {
			WriteHelper.WriteTableRowStart();
			try {
				string rowStyleName = DataRegistrator.TableRowStyleInfoTable.GetName(row);
				WriteHelper.WriteTableRowAttributes(rowStyleName);
				ExportRowGridBeforeAfter(row.GridBefore);
				base.ExportRow(row, tableInfo);
				ExportRowGridBeforeAfter(row.GridAfter);
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		void ExportRowGridBeforeAfter(int value) {
			for (int gridIndex = value; gridIndex > 0; gridIndex--) {
				ExportEmptyTableCellForGridAfterBefore();
			}
		}
		void ExportEmptyTableCellForGridAfterBefore() {
			try {
				writeHelper.WriteTableCellStart();
				try {
					WriteHelper.WriteParagraphStart(false);
				}
				finally {
					WriteHelper.WriteEndElement();
				}
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		protected internal override void ExportCell(TableCell cell, TableInfo tableInfo) {
			if (cell.VerticalMerging == MergingState.Continue)
				ExportCoveredCell();
			else
				ExportCellCore(cell, tableInfo);
			if (ListExporter.ReadyForProcess) {
				ListExporter.ExportCellEnd(cell);
			}
			if (cell.ColumnSpan > 1) {
				ListExporter.BeforeExportCoveredCells(cell);
				for (int index = 0; index < cell.ColumnSpan - 1; index++) {
					ExportCoveredCell();
				}
			}
		}
		protected internal virtual void ExportCoveredCell() {
			WriteHelper.WriteCoveredTableCellStart();
			WriteHelper.WriteEndElement();
		}
		protected internal virtual void ExportCellCore(TableCell cell, TableInfo tableInfo) {
			WriteHelper.WriteTableCellStart();
			try {
				string cellStyleName = DataRegistrator.TableCellStyleInfoTable.GetName(cell);
				WriteHelper.WriteTableCellAttributes(cell, cellStyleName, tableInfo.GetCellRowSpan(cell));
				base.ExportCell(cell, tableInfo);
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		protected internal virtual void ExportTableAutoStyles(bool isAutoStyleInMainDocument) {
			TableStyleInfoTable styles = DataRegistrator.TableStyleInfoTable;
			foreach (Table table in styles.Keys) {
				TableStyleInfo style = styles[table];
				if (style.IsStyleUsedInMainDocument == isAutoStyleInMainDocument)
					ExportTableAutoStyle(table, style);
			}
		}
		protected internal virtual void ExportTableAutoStyle(Table table, TableStyleInfo tableStyleInfo) {
			WriteHelper.WriteStyleStart();
			try {
				WriteHelper.WriteStyleTableAttributes(tableStyleInfo);
				ExportAutoStyleTableProperties(table, tableStyleInfo);
			}
			finally {
				WriteHelper.WriteEndElement();
			}
			ExportColumnAutoStyles(table);
			ExportRowAutoStyles(table);
		}
		protected internal virtual void ExportAutoStyleTableProperties(Table table, TableStyleInfo tableStyleInfo) {
			WriteHelper.WriteStyleTablePropertiesStart();
			try {
				WriteHelper.WriteStyleTablePropertiesAttributes(table.TableProperties, tableStyleInfo.ParentWidth);
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		protected internal virtual void ExportColumnAutoStyles(Table table) {
			if (DataRegistrator.TableColumnsInfoTable.ContainsKey(table)) {
				OpenDocumentTableColumnsInfo columnsInfo = DataRegistrator.TableColumnsInfoTable[table];
				for (int index = 0; index < columnsInfo.Count; index++) {
					ExportColumnStyle(columnsInfo[index]);
				}
			}
		}
		protected internal virtual void ExportColumnStyle(OpenDocumentTableColumnInfo columnInfo) {
			WriteHelper.WriteStyleStart();
			try {
				WriteHelper.WriteStyleTableColumnAttributes(columnInfo.Name);
				WriteHelper.WriteStyleTableColumnPropertiesStart();
				try {
					WriteHelper.WriteStyleTableColumnPropertiesContent(columnInfo.Width, columnInfo.UseOtimalColumnWidth);
				}
				finally {
					WriteHelper.WriteEndElement();
				}
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		protected internal virtual void ExportRowAutoStyles(Table table) {
			int count = table.Rows.Count;
			for (int rowId = 0; rowId < count; rowId++) {
				TableRow row = table.Rows[rowId];
				ExportRowAutoStyleCore(row);
				ExportCellAutoStyles(row);
			}
		}
		protected internal virtual void ExportRowAutoStyleCore(TableRow row) {
			string styleName = DataRegistrator.TableRowStyleInfoTable.GetName(row);
			WriteHelper.WriteStyleStart();
			try {
				WriteHelper.WriteStyleTableRowAttributes(styleName);
				WriteHelper.WriteStyleTableRowPropertiesStart();
				try {
					WriteHelper.WriteStyleTableRowPropertiesAttributes(row.Properties);
				}
				finally {
					WriteHelper.WriteEndElement();
				}
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		protected internal virtual void ExportCellAutoStyles(TableRow row) {
			int count = row.Cells.Count;
			for (int cellId = 0; cellId < count; cellId++) {
				ExportCellAutoStyle(row, cellId);
			}
		}
		protected internal virtual void ExportCellAutoStyle(TableRow row, int cellId) {
			TableCell cell = row.Cells[cellId];
			String cellStyleName = DataRegistrator.TableCellStyleInfoTable.GetName(cell);
			ExportCellAutoStyleCore(cell, cellStyleName);
		}
		protected internal virtual void ExportCellAutoStyleCore(TableCell cell, String cellStyleName) {
			WriteHelper.WriteStyleStart();
			try {
				WriteHelper.WriteStyleTableCellAttributes(cellStyleName);
				WriteHelper.WriteStyleTableCellPropertiesStart();
				try {
					ExportTableCellBorders(cell);
					ExportTableCellBackgroundColor(cell);
					ExportTableCellPadding(cell);
					ExportTableCellVerticalAlign(cell);
				}
				finally {
					WriteHelper.WriteEndElement();
				}
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		#region Table Cell Auto Style - Borders
		internal virtual void ExportTableCellBorders(TableCell cell) {
			BorderBase bottomBorder = cell.GetActualBottomCellBorder();
			BorderBase topBorder = cell.GetActualTopCellBorder();
			BorderBase rightBorder = cell.GetActualRightCellBorder();
			BorderBase leftBorder = cell.GetActualLeftCellBorder();
			bool equals = leftBorder.Info == rightBorder.Info
				&& leftBorder.Info == bottomBorder.Info
				&& leftBorder.Info == topBorder.Info;
			if (equals)
				WriteHelper.WriteTableCellBorder(leftBorder);
			else
				WriteHelper.WriteCellBordersDifferent(cell);
		}
		#endregion
		#region ExportTableCellBackgroundColor
		protected internal virtual void ExportTableCellBackgroundColor(TableCell cell) {
			if (cell.BackgroundColor != DXColor.Empty)
				WriteHelper.WriteTableCellBackground(cell.BackgroundColor);
		}
		#endregion
		#region ExportTableCellVerticalAlign
		protected internal virtual void ExportTableCellVerticalAlign(TableCell cell) {
			if (cell.Properties.UseVerticalAlignment)
				WriteHelper.WriteTableCellVerticalAlign(cell.VerticalAlignment);
		}
		#endregion
		#region Table Cell Auto Style - Padding
		internal virtual void ExportTableCellPadding(TableCell cell) {
			WidthUnitInfo left = cell.GetActualLeftMargin().Info;
			bool equals = left == cell.GetActualRightMargin().Info
				&& left == cell.GetActualTopMargin().Info
				&& left == cell.GetActualBottomMargin().Info;
			if (equals)
				WriteHelper.WriteTableCellPadding(cell.GetActualLeftMargin());
			else
				WriteHelper.WriteTableCellPaddingsDifferent(cell);
		}
		#endregion
		#endregion
		#region Bookmarks
		protected internal override void ExportBookmarkStart(Bookmark bookmark) {
			if (FieldExporter.FieldClosed) {
				WriteHelper.WriteBookmarkStart();
				WriteHelper.WriteBookmarkName(bookmark);
				WriteHelper.WriteEndElement();
			}
		}
		protected internal override void ExportBookmarkEnd(Bookmark bookmark) {
			if (FieldExporter.FieldClosed) {
				WriteHelper.WriteBookmarkEnd();
				WriteHelper.WriteBookmarkName(bookmark);
				WriteHelper.WriteEndElement();
			}
		}
		#endregion
		#region Comments
		protected internal override void ExportCommentStart(Comment comment) {
			WriteHelper.WriteOfficeStartElement("annotation");
			try {
				string commentAuthor = comment.Author;
				if(!String.IsNullOrEmpty(commentAuthor))
					ExportCommentAuthor(commentAuthor);
				if (comment.Date > Comment.MinCommentDate) {
					DateTime commentDate = comment.Date;
					ExportCommentDate(commentDate.ToString("yyyy-MM-ddTHH:mm:ss"));
				}
				CommentContentType commentContent = comment.Content;
				if (!commentContent.PieceTable.IsEmpty)
					ExportCommentContent(commentContent);
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		void ExportCommentAuthor(string commentAuthor) {
			WriteHelper.WriteDcStartElement("creator");
			try {
				WriteHelper.WriteValue(commentAuthor);
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		void ExportCommentDate(string commentDate) {
			WriteHelper.WriteDcStartElement("date");
			try {
				WriteHelper.WriteValue(commentDate);
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		void ExportCommentContent(CommentContentType commentContent) {
			PerformExportPieceTable(commentContent.PieceTable, ExportCommentContent);
		}
		protected internal virtual void ExportCommentContent() {
			ExportParagraphs(PieceTable.Paragraphs.First.Index, PieceTable.Paragraphs.Last.Index);
		}
		#endregion
		#region FootNotes & EndNotes
		protected internal override void ExportFootNoteRun(FootNoteRun run) {
			if (PieceTable.IsMain)
				ExportFootNoteCore(run, "footnote");
		}
		protected internal override void ExportEndNoteRun(EndNoteRun run) {
			if (PieceTable.IsMain)
				ExportFootNoteCore(run, "endnote");
		}
		protected internal virtual void ExportFootNoteCore<T>(FootNoteRunBase<T> run, string type) where T : FootNoteBase<T> {
			WriteHelper.WriteTextStartElement("note");
			try {
				WriteHelper.WriteTextStringAttribute("note-class", type);
				WriteHelper.WriteTextStartElement("note-citation");
				WriteHelper.WriteEndElement();
				WriteHelper.WriteTextStartElement("note-body");
				try {
					PerformExportPieceTable(run.Note.PieceTable, ExportFootNoteEndNoteContent);
				}
				finally {
					WriteHelper.WriteEndElement();
				}
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		protected internal virtual void ExportFootNoteEndNoteContent() {
			ExportParagraphs(PieceTable.Paragraphs.First.Index, PieceTable.Paragraphs.Last.Index);
		}
		#endregion
		#endregion
		public virtual void WriteInlinePictureFrameContent(InlinePictureRun run) {
			string imageID = GenerateImageID();
			WriteHelper.WritePictureFrameAttributes(run, imageID);
			WriteHelper.WriteInlineImageStart();
			try {
				WriteHelper.WriteInlineImageAttributes(run.Image, imageID);
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		public virtual void WriteFloatingObjectPictureContent(PictureFloatingObjectContent content, Shape shape, FloatingObjectProperties floatingObjectProperties, string frameName, string imageID) {
			WriteHelper.WriteFloatingObjectFrameAttributes(floatingObjectProperties, shape, null, false, frameName);
			WriteHelper.WriteInlineImageStart();
			try {
				WriteHelper.WriteInlineImageAttributes(content.Image, imageID);
			}
			finally {
				WriteHelper.WriteEndElement();
			}
		}
		public virtual void WriteFloatingObjectTextBoxContent(TextBoxFloatingObjectContent content, Shape shape, FloatingObjectProperties floatingObjectProperties, string frameName) {
			WriteHelper.WriteFloatingObjectFrameAttributes(floatingObjectProperties, shape, content.TextBoxProperties, true, frameName);
			WriteHelper.WriteFloatingObjectAnchorTextBoxStart();
			try {
				PerformExportPieceTable(content.TextBox.PieceTable, ExportPieceTable);
			} finally {
				WriteHelper.WriteEndElement();
			}
		}
		internal static string WriteAnchorType(FloatingObjectProperties floatingObjectProperties) {
			return "paragraph";
		}
		protected internal virtual string GetTextRunStyleName(TextRun run) {
			return NameResolver.GetTextRunStyleName(DataRegistrator.CharacterAutoStyles, run);
		}
		protected internal virtual string GetPictureRunStyleName(InlinePictureRun run) {
			return DataRegistrator.PictureAutoStyles.GetStyleName(run.PictureProperties.Index);
		}
		protected internal virtual string GetFloatingObjectAnchorRunStyleName(FloatingObjectProperties floatingObjectProperties) {
			return DataRegistrator.FloatingObjectAutoStyles.GetStyleName(floatingObjectProperties.Index);
		}
		protected internal virtual string GetFieldStyleName(FieldCodeStartRun run) {
			Dictionary<FieldCodeStartRun, string> styles = DataRegistrator.FieldStyles;
			string styleName;
			if (styles.TryGetValue(run, out styleName))
				return styleName;
			return String.Empty;
		}
		protected internal string GenerateImageID() {
			return ImageCounter.ToString();
		}
	}
}
