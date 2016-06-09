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
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Fields;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Export.OpenDocument {
	#region OpenDocumentFieldExporterBase (abstract class)
	public abstract class OpenDocumentFieldExporterBase {
		readonly FieldsExporter exporter;
		protected OpenDocumentFieldExporterBase(FieldsExporter exporter) {
			this.exporter = exporter;
		}
		public PieceTable PieceTable { get { return WriteHelper.Exporter.PieceTable; } }
		public ExportHelper WriteHelper { get { return exporter.WriteHelper; } }
		public List<RunIndex> InnerRuns { get { return FieldsExporter.InnerRuns; } }
		public FieldsExporter FieldsExporter { get { return exporter; } }
		public abstract void ExportCodeStartRun(FieldCodeStartRun run);
		public virtual void ExportFieldCodeEndRunCore() {
		}
		public virtual void ExportFieldResultEndRunCore() {
			TextRunCollection runs = FieldsExporter.Exporter.PieceTable.Runs;
			foreach (RunIndex index in InnerRuns)
				ExportFieldResultRun(runs[index]);
			WriteHelper.WriteEndElement();
			WriteHelper.WriteEndElement();
		}
		void ExportFieldResultRun(TextRunBase run) {
			TextRun textRun = run as TextRun;
			if (textRun != null) {
				WriteHelper.WriteTextRunContent(textRun);
				return;
			}
			InlinePictureRun pictureRun = run as InlinePictureRun;
			if (pictureRun != null) {
				exporter.Exporter.ExportInlinePictureRunCore(pictureRun);
				return;
			}
			FloatingObjectAnchorRun anchorRun = run as FloatingObjectAnchorRun;
			if (anchorRun != null) {
				FieldsExporter.SaveInnerRuns();
				exporter.Exporter.ExportFloatingObjectAnchorRunCore(anchorRun);
				FieldsExporter.RestoreInnerRuns();
				return;
			}
		}
	}
	#endregion
	#region PageFieldExporter
	public class PageFieldExporter : OpenDocumentFieldExporterBase {
		public PageFieldExporter(FieldsExporter exporter)
			: base(exporter) {
		}
		public override void ExportCodeStartRun(FieldCodeStartRun run) {
			WriteHelper.WritePageFieldStart();
			try {
				WriteHelper.WritePageFieldAttributes();
			} finally {
			}
		}
	}
	#endregion
	#region PageRefFieldExporter
	public class PageRefFieldExporter : OpenDocumentFieldExporterBase {
		public PageRefFieldExporter(FieldsExporter exporter)
			: base(exporter) {
		}
		public override void ExportCodeStartRun(FieldCodeStartRun run) {
			RunIndex index = run.GetRunIndex();
			string targetRef = GetTargetBookmarkRef(index);
			WriteHelper.WritePageRefFieldStart();
			WriteHelper.WritePageRefFieldAttributes(targetRef);
		}
		string GetTargetBookmarkRef(RunIndex index) {
			Field field = PieceTable.FindFieldByRunIndex(index);
			StringBuilder sb = new StringBuilder();
			for (RunIndex codeIndex = field.Code.Start; codeIndex < field.Code.End; codeIndex++) {
				sb.Append(WriteHelper.Exporter.PieceTable.GetRunText(codeIndex));
			}
			string code = sb.ToString();
			string[] splitCode = code.Trim().Split(' ');
			string targetRef = string.Empty;
			if (splitCode.Length > 3)
				targetRef = splitCode[2];
			return targetRef;
		}
	}
	#endregion
	#region TOCFieldExporter
	public class TOCFieldExporter : OpenDocumentFieldExporterBase {
		public TOCFieldExporter(FieldsExporter exporter)
			: base(exporter) {
		}
		public override void ExportCodeStartRun(FieldCodeStartRun run) {
			WriteHelper.WriteTextStart();
		}
	}
	#endregion
	#region NumPagesFieldExporter
	public class NumPagesFieldExporter : OpenDocumentFieldExporterBase {
		public NumPagesFieldExporter(FieldsExporter exporter)
			: base(exporter) {
		}
		public override void ExportCodeStartRun(FieldCodeStartRun run) {
			WriteHelper.WritePageCountFieldStart();
			WriteHelper.WritePageCountFieldAttributes();
		}
	}
	#endregion
	#region HyperlinkFieldExporter
	public class HyperlinkFieldExporter : OpenDocumentFieldExporterBase {
		HyperlinkInfo info;
		public HyperlinkFieldExporter(FieldsExporter exporter)
			: base(exporter) {
			this.info = HyperlinkInfo.Empty;
		}
		HyperlinkInfo GetHyperlinkInfo(FieldCodeStartRun run) {
			try {
				RunIndex index = run.GetRunIndex();
				Field field = WriteHelper.Exporter.PieceTable.FindFieldByRunIndex(index);
				return WriteHelper.Exporter.PieceTable.GetHyperlinkInfo(field);
			} catch {
				return HyperlinkInfo.Empty;
			}
		}
		public override void ExportCodeStartRun(FieldCodeStartRun run) {
			info = GetHyperlinkInfo(run);
			WriteHelper.WriteHyperlinkStart();
			try {
				WriteHelper.WriteHyperlinkAttributes(info);
			} finally {
			}
		}
	}
	#endregion
	#region OpenDocumentExpressionFieldExporter
	public class OpenDocumentExpressionFieldExporter : OpenDocumentFieldExporterBase {
		bool shouldCloseParagraph;
		public OpenDocumentExpressionFieldExporter(FieldsExporter exporter)
			: base(exporter) {
		}
		internal bool ShouldCloseParagraph { get { return shouldCloseParagraph; } set { shouldCloseParagraph = value; } }
		public override void ExportCodeStartRun(FieldCodeStartRun run) {
			WriteHelper.WriteFieldExpressionStart();
			WriteHelper.WriteFieldAttributes();
		}
		public override void ExportFieldCodeEndRunCore() {
			string fieldFormula = string.Empty;
			foreach (RunIndex index in InnerRuns) {
				TextRun run = FieldsExporter.Exporter.PieceTable.Runs[index] as TextRun;
				if (run != null)
					fieldFormula += FieldsExporter.GetRunText(run);
			}
			fieldFormula = XmlTextHelper.DeleteIllegalXmlCharacters(fieldFormula);
			WriteHelper.WriteFieldFormula(fieldFormula);
		}
	}
	#endregion
	#region OpenDocumentFieldExportersCollection
	public class OpenDocumentFieldExportersCollection : Dictionary<string, OpenDocumentFieldExporterBase> {
	}
	#endregion
	#region FieldsExporter
	public class FieldsExporter {
		#region Fields
		OpenDocumentFieldExportersCollection fieldExporters;
		List<RunIndex> innerRuns = new List<RunIndex>();
		string innerRunsText;
		readonly ExportHelper writeHelper;
		int nestedFieldsCounter;
		List<RunIndex> originalInnerRuns;
		int originalNestedFieldsCounter;
		OpenDocumentFieldExporterBase currentfieldExporter;
		#endregion
		#region Properties
		public bool FieldClosed { get { return NestedFieldsCounter == 0; } }
		protected internal bool IsRootField { get { return NestedFieldsCounter == 1; } }
		protected internal List<RunIndex> InnerRuns { get { return innerRuns; } }
		protected internal string InnerRunsText { get { return innerRunsText; } set { innerRunsText = value; } }
		protected internal int NestedFieldsCounter { get { return nestedFieldsCounter; } set { nestedFieldsCounter = value; } }
		protected internal ExportHelper WriteHelper { get { return writeHelper; } }
		protected internal OpenDocumentTextExporter Exporter { get { return WriteHelper.Exporter; } }
		public OpenDocumentFieldExporterBase CurrentfieldExporter {
			get { return currentfieldExporter; }
			internal set { currentfieldExporter = value; }
		}
		#endregion
		public FieldsExporter(ExportHelper writeHelper) {
			Guard.ArgumentNotNull(writeHelper, "writeHelper");
			this.writeHelper = writeHelper;
			this.fieldExporters = CreateOpenDocumentFieldsExportersCollection();
			this.currentfieldExporter = new OpenDocumentExpressionFieldExporter(this);
		}
		OpenDocumentFieldExportersCollection CreateOpenDocumentFieldsExportersCollection() {
			OpenDocumentFieldExportersCollection result = new OpenDocumentFieldExportersCollection();
			result.Add(HyperlinkField.FieldType, new HyperlinkFieldExporter(this));
			result.Add(PageField.FieldType, new PageFieldExporter(this));
			result.Add(NumPagesField.FieldType, new NumPagesFieldExporter(this));
			result.Add(PageRefField.FieldType, new PageRefFieldExporter(this));
			result.Add(TocField.FieldType, new TOCFieldExporter(this));
			return result;
		}
		public virtual void ExportTextRun(TextRun run) {
			InnerRunsText += GetRunText(run);
			InnerRuns.Add(run.GetRunIndex());
		}
		public virtual void ExportInlinePictureRun(InlinePictureRun run) {
			InnerRuns.Add(run.GetRunIndex());
		}
		public virtual void ExportFloatingObjectAnchorRun(FloatingObjectAnchorRun run) {
			InnerRuns.Add(run.GetRunIndex());
		}
		protected internal virtual void SaveInnerRuns() {
			originalInnerRuns = innerRuns;
			innerRuns = new List<RunIndex>();
			originalNestedFieldsCounter = NestedFieldsCounter;
			nestedFieldsCounter = 0;
		}
		protected internal virtual void RestoreInnerRuns() {
			Debug.Assert(originalInnerRuns != null);
			innerRuns = originalInnerRuns;
			originalInnerRuns = null;
			nestedFieldsCounter = originalNestedFieldsCounter;
			originalNestedFieldsCounter = 0;
		}
		internal virtual string GetRunText(TextRun run) {
			string plainText = run.GetPlainText(Exporter.PieceTable.TextBuffer);
			StringBuilder sb = new StringBuilder();
			int count = plainText.Length;
			for (int i = 0; i < count; i++) {
				char character = plainText[i];
				if (!ShouldSkipCharacter(character))
					sb.Append(character);
			}
			return sb.ToString();
		}
		internal virtual bool ShouldSkipCharacter(char ch) {
			return ch == Characters.LineBreak
				|| ch == Characters.PageBreak
				|| ch == Characters.ColumnBreak
				|| ch == Characters.TabMark;
		}
		public virtual void ExportCodeStartRun(FieldCodeStartRun run) {
			NestedFieldsCounter++;
			if (IsRootField) {
				InitializeCurrentFieldExporter(run);
				ExportCodeStartRunCore(run);
			}
		}
		protected virtual internal void InitializeCurrentFieldExporter(FieldCodeStartRun run) {
			RunIndex index = run.GetRunIndex();
			Field field = WriteHelper.Exporter.PieceTable.FindFieldByRunIndex(index);
			Token exporterPieceTableGetFieldToken = Exporter.PieceTable.GetFieldToken(field);
			string fieldToken = exporterPieceTableGetFieldToken.Value;
			if (fieldExporters.ContainsKey(fieldToken))
				this.currentfieldExporter = fieldExporters[fieldToken];
			else
				this.currentfieldExporter = new OpenDocumentExpressionFieldExporter(this);
		}
		#region Field
		public virtual void ExportFieldCodeEndRun() {
			if (IsRootField) {
				ExportFieldCodeEndRunCore();
				ClearInnerRuns();
			}
		}
		public virtual void ExportFieldResultEndRun() {
			if (this.FieldClosed)
				return;
			if (IsRootField) {
				ExportFieldResultEndRunCore();
				ClearInnerRuns();
			}
			NestedFieldsCounter--;
		}
		void ClearInnerRuns() {
			InnerRuns.Clear();
			InnerRunsText = String.Empty;
		}
		protected internal virtual void ExportCodeStartRunCore(FieldCodeStartRun run) {
			WriteHelper.WriteTextRunStart();
			if (ShouldExportFieldRunProperties()) {
				RunIndex runIndex = run.GetRunIndex();
				RunIndex firstRunIndexFromFieldResult = runIndex + 1;
				Field field = run.Paragraph.PieceTable.GetHyperlinkField(runIndex);
				if (field != null)
					firstRunIndexFromFieldResult = field.Result.Start;
				TextRun resultRun = Exporter.PieceTable.Runs[firstRunIndexFromFieldResult] as TextRun;
				if (resultRun != null) {
					string styleName = Exporter.GetTextRunStyleName(resultRun);
					WriteHelper.WriteTextRunAttributes(styleName);
				}
			}
			CurrentfieldExporter.ExportCodeStartRun(run);
		}
		protected virtual bool ShouldExportFieldRunProperties() {
			return true;
		}
		protected internal virtual void ExportFieldCodeEndRunCore() {
			CurrentfieldExporter.ExportFieldCodeEndRunCore();
		}
		protected internal virtual void ExportFieldResultEndRunCore() {
			CurrentfieldExporter.ExportFieldResultEndRunCore();
		}
		#endregion
		public void CloseFieldOnParagraphClose() {
			ExportFieldResultEndRun();
		}
	}
	#endregion
	#region FieldRegistrator
	public class FieldRegistrator {
		#region Fields
		int nestedFieldsCount;
		FieldCodeStartRun startRun;
		List<TextRun> innerTextRuns;
		CharacterStyleInfoTable characterAutoStyles;
		Dictionary<FieldCodeStartRun, string> fieldStyles;
		#endregion
		public FieldRegistrator(CharacterStyleInfoTable characterAutoStyles) {
			Guard.ArgumentNotNull(characterAutoStyles, "characterAutoStyles");
			this.characterAutoStyles = characterAutoStyles;
			innerTextRuns = new List<TextRun>();
			fieldStyles = new Dictionary<FieldCodeStartRun, string>();
		}
		#region Properties
		public Dictionary<FieldCodeStartRun, string> FieldStyles { get { return fieldStyles; } }
		protected internal int NestedFieldsCount { get { return nestedFieldsCount; } set { nestedFieldsCount = value; } }
		protected internal FieldCodeStartRun StartRun { get { return startRun; } set { startRun = value; } }
		protected internal List<TextRun> InnerTextRuns { get { return innerTextRuns; } }
		protected internal CharacterStyleInfoTable CharacterAutoStyles { get { return characterAutoStyles; } }
		protected internal bool IsRootField { get { return NestedFieldsCount == 1; } }
		#endregion
		private void Reset() {
			NestedFieldsCount = 0;
			startRun = null;
			ClearInnerTextRuns();
		}
		public void RegisterCodeStartRun(FieldCodeStartRun run) {
			NestedFieldsCount++;
			if (IsRootField)
				StartRun = run;
		}
		public void RegisterCodeEndRun() {
			if (IsRootField)
				ClearInnerTextRuns();
		}
		internal void ClearInnerTextRuns() {
			InnerTextRuns.Clear();
		}
		internal void RegisterFieldStyle() {
			if (InnerTextRuns.Count > 0 && StartRun != null) {
				TextRun run = InnerTextRuns[0];
				string styleName = NameResolver.GetTextRunStyleName(CharacterAutoStyles, run);
				if (!fieldStyles.ContainsKey(StartRun))
					fieldStyles.Add(StartRun, styleName);
			}
		}
		public void RegisterEndResultRun() {
			Debug.Assert(NestedFieldsCount > 0);
			if (IsRootField) {
				RegisterFieldStyle();
				Reset();
			} else
				NestedFieldsCount--;
		}
		public void RegisterTextRun(TextRun run) {
			if (NestedFieldsCount >= 0)
				InnerTextRuns.Add(run);
		}
	}
	#endregion
}
