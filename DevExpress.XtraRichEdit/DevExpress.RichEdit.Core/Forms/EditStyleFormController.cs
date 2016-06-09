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
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Model;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Internal;
using System.Collections.Generic;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Forms {
	public class EditStyleFormControllerParameters : FormControllerParameters {
		#region Fields
		ParagraphStyle paragraphSourceStyle;
		ParagraphStyle targetParagraphStyle;
		CharacterStyle characterSourceStyle;
		CharacterStyle targetCharacterStyle;
		readonly IRichEditControl editStyleControl;
		readonly ParagraphIndex paragraphIndex;
		bool isParagraphStyle;
		#endregion
		public EditStyleFormControllerParameters(IRichEditControl control, ParagraphStyle sourceStyle, ParagraphIndex paragraphIndex)
			: base(control) {
			Guard.ArgumentNotNull(sourceStyle, "sourceStyle");
			this.paragraphSourceStyle = sourceStyle;
			this.targetParagraphStyle = new ParagraphStyle(paragraphSourceStyle.DocumentModel);
			this.targetCharacterStyle = new CharacterStyle(paragraphSourceStyle.DocumentModel);
			this.editStyleControl = control;
			this.paragraphIndex = paragraphIndex;
			isParagraphStyle = true;
		}
		public EditStyleFormControllerParameters(IRichEditControl control, CharacterStyle sourceStyle, ParagraphIndex paragraphIndex)
			: base(control) {
			Guard.ArgumentNotNull(sourceStyle, "sourceStyle");
			this.characterSourceStyle = sourceStyle;
			this.targetCharacterStyle = new CharacterStyle(characterSourceStyle.DocumentModel);
			this.targetParagraphStyle = new ParagraphStyle(characterSourceStyle.DocumentModel);
			this.editStyleControl = control;
			this.paragraphIndex = paragraphIndex;
			isParagraphStyle = false;
		}
		#region Properties
		internal bool IsParagraphStyle { get { return isParagraphStyle; } }
		internal ParagraphIndex ParagraphIndex { get { return paragraphIndex; } }
		internal ParagraphStyle ParagraphSourceStyle {
			get { return paragraphSourceStyle; }
			set {
				paragraphSourceStyle = value;
				isParagraphStyle = true;
			}
		}
		internal CharacterStyle CharacterSourceStyle {
			get { return characterSourceStyle; }
			set {
				characterSourceStyle = value;
				isParagraphStyle = false;
			}
		}
		internal ParagraphStyle TargetParagraphStyle { get { return targetParagraphStyle; } set { targetParagraphStyle = value; } }
		internal CharacterStyle TargetCharacterStyle { get { return targetCharacterStyle; } set { targetCharacterStyle = value; } }
		internal IRichEditControl EditStyleControl { get { return editStyleControl; } }
		#endregion
	}
	public class EditStyleFormController : FormController {
		#region Fields
		const int MaxTempParagraphSymbols = 256;
		const int EmptyParagraphRepeatCount = 21;
		const int FollowingParagraphRepeatCount = 25;
		const int PreviousParagraphRepeatCount = 10;
		readonly EditStyleFormControllerParameters parameters;
		readonly ParagraphIndex paragraphIndex;
		ParagraphStyle intermediateParagraphStyle;
		CharacterStyle intermediateCharacterStyle;
		readonly IRichEditControl control;
		readonly DocumentModel model;
		#endregion
		public EditStyleFormController(IRichEditControl previewStyleControl, EditStyleFormControllerParameters parameters) {
			Guard.ArgumentNotNull(parameters, "parameters");
			this.parameters = parameters;
			this.control = parameters.EditStyleControl;
			this.paragraphIndex = parameters.ParagraphIndex;
			this.model = parameters.Control.InnerControl.DocumentModel;
			DocumentModel previewModel = previewStyleControl.InnerDocumentServer.DocumentModel;
			previewModel.BeginUpdate();
			try {
				CopyCharacterStyles(previewModel);
				CopyParagraphStyles(previewModel);
			}
			finally {
				previewModel.EndUpdate();
			}
			if (IsParagraphStyle)
				this.intermediateParagraphStyle = previewModel.ParagraphStyles.GetStyleByName(ParagraphSourceStyle.StyleName);
			else
				this.intermediateCharacterStyle = previewModel.CharacterStyles.GetStyleByName(CharacterSourceStyle.StyleName);
			previewModel.DefaultCharacterProperties.CopyFrom(model.DefaultCharacterProperties.Info);
			previewModel.DefaultParagraphProperties.CopyFrom(model.DefaultParagraphProperties.Info);
		}
		#region Properties
		public IRichEditControl Control { get { return control; } }
		public DocumentModel Model { get { return model; } }
		public string StyleName {
			get { return IsParagraphStyle ? IntermediateParagraphStyle.LocalizedStyleName : IntermediateCharacterStyle.LocalizedStyleName; }
			set {
				if (IsParagraphStyle)
					IntermediateParagraphStyle.StyleName = value;
				else
					IntermediateCharacterStyle.StyleName = value;
			}
		}
		public EditStyleFormControllerParameters Parameters { get { return parameters; } }
		public bool IsParagraphStyle { get { return parameters.IsParagraphStyle; } }
		public ParagraphIndex ParagraphIndex { get { return paragraphIndex; } }
		public ParagraphStyle ParagraphSourceStyle { get { return parameters.ParagraphSourceStyle; } }
		public ParagraphStyle IntermediateParagraphStyle { get { return intermediateParagraphStyle; } set { intermediateParagraphStyle = value; } }
		private ParagraphProperties ParagraphProperties { get { return IntermediateParagraphStyle.ParagraphProperties; } }
		public CharacterStyle CharacterSourceStyle { get { return parameters.CharacterSourceStyle; } }
		public CharacterStyle IntermediateCharacterStyle { get { return intermediateCharacterStyle; } set { intermediateCharacterStyle = value; } }
		public CharacterProperties CharacterProperties { get { return IsParagraphStyle ? IntermediateParagraphStyle.CharacterProperties : IntermediateCharacterStyle.CharacterProperties; } }
		private DocumentModelUnitConverter UnitConverter { get { return Control.InnerControl.DocumentModel.UnitConverter; } }
		#endregion
		void CopyCharacterStyles(DocumentModel previewModel) {
			foreach (CharacterStyle style in model.CharacterStyles) {
				if (style.Deleted)
					continue;
				CharacterStyle targetStyle = previewModel.CharacterStyles.GetStyleByName(style.StyleName);
				if (targetStyle != null)
					style.CopyTo(targetStyle);
				else
					style.Copy(previewModel);
			}
		}
		void CopyParagraphStyles(DocumentModel previewModel) {
			foreach (ParagraphStyle style in model.ParagraphStyles) {
				if (style.Deleted)
					continue;
				ParagraphStyle targetStyle = previewModel.ParagraphStyles.GetStyleByName(style.StyleName);
				if (targetStyle != null)
					style.CopyTo(targetStyle);
				else
					style.Copy(previewModel);
			}
		}
		private void CopyParagraphStyle(ParagraphStyle targetStyle, ParagraphStyle style) {
			style.CopyTo(targetStyle);
		}
		private void CopyCharacterStyle(CharacterStyle targetStyle, CharacterStyle style) {
			style.CopyTo(targetStyle);
		}
		public override void ApplyChanges() {
			if (IsParagraphStyle)
				CopyParagraphStyle(ParagraphSourceStyle, IntermediateParagraphStyle);
			else
				CopyCharacterStyle(CharacterSourceStyle, IntermediateCharacterStyle);
		}
		public ParagraphFormattingInfo GetParentParagraphProperties() {
			if (IntermediateParagraphStyle.Parent != null)
				return IntermediateParagraphStyle.Parent.GetMergedWithDefaultParagraphProperties().Info;
			return Control.InnerControl.DocumentModel.DefaultParagraphProperties.Info.Info;
		}
		public CharacterFormattingInfo GetParentCharacterProperties() {
			if (IsParagraphStyle) {
				if (IntermediateParagraphStyle.Parent != null)
					return IntermediateParagraphStyle.Parent.GetMergedWithDefaultCharacterProperties().Info;
			}
			else {
				if (IntermediateCharacterStyle.Parent != null)
					return IntermediateCharacterStyle.Parent.GetMergedWithDefaultCharacterProperties().Info;
			}
			return Control.InnerControl.DocumentModel.DefaultCharacterProperties.Info.Info;
		}
		public void ChangeCurrentValue<T>(CharacterPropertyAccessorBase<T> accesor, T isChecked) {
			accesor.Reset(CharacterProperties);
			CharacterFormattingInfo parent = GetParentCharacterProperties();
			if (parent == null || !isChecked.Equals(accesor.GetValue(parent)))
				accesor.SetValue(CharacterProperties, isChecked);
		}
		public void ChangeCurrentValue<T>(ParagraphPropertyAccessorBase<T> accesor, T isChecked) {
			accesor.Reset(ParagraphProperties);
			ParagraphFormattingInfo parent = GetParentParagraphProperties();
			if (parent == null || !isChecked.Equals(accesor.GetValue(parent)))
				accesor.SetValue(ParagraphProperties, isChecked);
		}
		protected internal string FillParagraphText(int count, XtraRichEditStringId id) {
			System.Text.StringBuilder text = new System.Text.StringBuilder();
			for (int i = 0; i < count; i++)
				text.Append(XtraRichEditLocalizer.GetString(id)); 
			return text.ToString();
		}
		public string GetParagraphText(Paragraph sourceParagraph, int paragraphLength) {
			PieceTable source = control.InnerControl.DocumentModel.ActivePieceTable;
			DocumentModelPosition startPosition = PositionConverter.ToDocumentModelPosition(source, sourceParagraph.LogPosition);
			DocumentModelPosition endPosition = PositionConverter.ToDocumentModelPosition(source, sourceParagraph.LogPosition + paragraphLength - 1);
			return source.GetPlainText(startPosition, endPosition);
		}
		protected internal void SetActiveParagraph(PieceTable target, Paragraph sourceParagraph) {
			if (sourceParagraph.LogPosition != sourceParagraph.EndLogPosition) {
				int paragraphLength = sourceParagraph.Length < MaxTempParagraphSymbols ? sourceParagraph.Length : MaxTempParagraphSymbols;
				string paragraphText = GetParagraphText(sourceParagraph, paragraphLength);
				if (!String.IsNullOrEmpty(paragraphText)) {
					target.InsertPlainText(new DocumentLogPosition(0), paragraphText);
					return;
				}
			}
			string activeParagraphText = String.Empty;
			activeParagraphText += FillParagraphText(EmptyParagraphRepeatCount, XtraRichEditStringId.Caption_CurrentParagraphText);
			target.InsertPlainText(new DocumentLogPosition(0), activeParagraphText);
		}
		private static void ApplyCharacterStyleToParagraph(ParagraphIndex index, PieceTable target, int styleIndex, int cStyleIndex) {
			target.Paragraphs[index].ParagraphStyleIndex = styleIndex;
			RunIndex start = target.Paragraphs[index].FirstRunIndex;
			RunIndex end = target.Paragraphs[index].LastRunIndex;
			for (RunIndex i = start; i < end; i++) {
				target.Runs[i].CharacterStyleIndex = cStyleIndex;
			}
		}
		private string GetAvalableStyleName(string currentName) {
			if (IsValidName(currentName))
				return currentName;
			int stylesCount = Model.CharacterStyles.Count + Model.ParagraphStyles.Count;
			for (int i = 1; i < stylesCount + 1; i++) {
				if (IsValidName(currentName + i))
					return currentName + i;
			}
			return currentName + stylesCount + 1;
		}
		protected internal void ApplyStyleToParagraphs(IRichEditControl richEditControl) {
			DocumentModel model = richEditControl.InnerControl.DocumentModel;
			PieceTable target = model.ActivePieceTable;
			ParagraphStyle tempParagraphStyle = new ParagraphStyle(model);
			tempParagraphStyle.StyleName = GetAvalableStyleName("_TempParagraphStyle");
			tempParagraphStyle.CharacterProperties.FontName = "Times New Roman";
			tempParagraphStyle.CharacterProperties.DoubleFontSize = 20;
			tempParagraphStyle.CharacterProperties.ForeColor = DXColor.FromArgb(191, 191, 191);
			int styleIndex = tempParagraphStyle.Copy(model);
			CharacterStyle defaulCharacterStyle = new CharacterStyle(model);
			defaulCharacterStyle.StyleName = GetAvalableStyleName("_DefaulCharacterStyle");
			int cStyleIndex = defaulCharacterStyle.Copy(model);
			ApplyCharacterStyleToParagraph(new ParagraphIndex(0), target, styleIndex, cStyleIndex);
			ApplyCharacterStyleToParagraph(new ParagraphIndex(2), target, styleIndex, cStyleIndex);
		}
		public void ChangePreviewControlCurrentStyle(IRichEditControl richEditControl) {
			DocumentModel tableModel = richEditControl.InnerDocumentServer.DocumentModel;
			Paragraph paragraph = tableModel.ActivePieceTable.Paragraphs[new ParagraphIndex(1)];
			tableModel.BeginUpdate();
			try {
				if (IsParagraphStyle) {
					ApplyParagraphStyle(paragraph, tableModel.ParagraphStyles.GetStyleIndexByName(IntermediateParagraphStyle.StyleName));
					ApplyCharacterStyle(paragraph, tableModel.CharacterStyles.Count - 1);
				}
				else {
					ApplyParagraphStyle(paragraph, tableModel.ParagraphStyles.Count - 1);
					ApplyCharacterStyle(paragraph, tableModel.CharacterStyles.GetStyleIndexByName(IntermediateCharacterStyle.StyleName));
				}
			}
			finally {
				tableModel.EndUpdate();
			}
		}
		void ApplyParagraphStyle(Paragraph paragraph, int styleIndex) {
			paragraph.ParagraphStyleIndex = styleIndex;
		}
		void ApplyCharacterStyle(Paragraph paragraph, int styleIndex) {
			PieceTable pieceTable = paragraph.PieceTable;
			if (paragraph.Length > 0)
				pieceTable.ApplyCharacterStyle(paragraph.LogPosition, paragraph.Length, styleIndex);
		}
		public void FillTempRichEdit(IRichEditControl richEditControl) {
			DocumentModel sourceModel = Control.InnerControl.DocumentModel;
			PieceTable sourcePieceTable = sourceModel.ActivePieceTable;
			DocumentModel tableModel = richEditControl.InnerDocumentServer.DocumentModel;
			PieceTable targetPieceTable = tableModel.ActivePieceTable;
			Paragraph sourceParagraph = sourcePieceTable.Paragraphs[ParagraphIndex];
			string secondParagraphText = "\n";
			secondParagraphText += FillParagraphText(FollowingParagraphRepeatCount, XtraRichEditStringId.Caption_FollowingParagraphText);
			targetPieceTable.InsertPlainText(new DocumentLogPosition(0), secondParagraphText);
			SetActiveParagraph(targetPieceTable, sourceParagraph);
			string firstParagraphText = String.Empty;
			firstParagraphText += FillParagraphText(PreviousParagraphRepeatCount, XtraRichEditStringId.Caption_PreviousParagraphText);
			firstParagraphText += "\n";
			targetPieceTable.InsertPlainText(new DocumentLogPosition(0), firstParagraphText);
			ApplyStyleToParagraphs(richEditControl);
			ParagraphStyle defaultParagraphStyle = new ParagraphStyle(tableModel);
			defaultParagraphStyle.StyleName = GetAvalableStyleName("_DefaultParagraphStyle");
			defaultParagraphStyle.Copy(tableModel);
			DevExpress.XtraRichEdit.API.Native.Document document = richEditControl.Document;
			document.CaretPosition = document.CreatePosition(0);
		}
		protected internal void CopyCharacterPropertiesFromMerged(MergedCharacterProperties mergedProperties) {
			CharacterFormattingInfo mergedCharacterProperties = mergedProperties.Info;
			CharacterFormattingInfo characterProperties = IsParagraphStyle ? GetIntermediateMergedCharacterProperties(IntermediateParagraphStyle) : GetIntermediateMergedCharacterProperties(IntermediateCharacterStyle);
			if (characterProperties.FontName != mergedCharacterProperties.FontName)
				ChangeCurrentValue<string>(new FontNameAccessor(), mergedCharacterProperties.FontName);
			if (characterProperties.FontBold != mergedCharacterProperties.FontBold)
				ChangeCurrentValue<bool>(new FontBoldAccessor(), mergedCharacterProperties.FontBold);
			if (characterProperties.FontItalic != mergedCharacterProperties.FontItalic)
				ChangeCurrentValue<bool>(new FontItalicAccessor(), mergedCharacterProperties.FontItalic);
			if (characterProperties.DoubleFontSize != mergedCharacterProperties.DoubleFontSize)
				ChangeCurrentValue<int>(new DoubleFontSizeAccessor(), mergedCharacterProperties.DoubleFontSize);
			if (characterProperties.ForeColor != mergedCharacterProperties.ForeColor)
				ChangeCurrentValue<Color>(new ForeColorAccessor(), mergedCharacterProperties.ForeColor);
			if (CharacterProperties.FontUnderlineType != mergedCharacterProperties.FontUnderlineType)
				ChangeCurrentValue<UnderlineType>(new FontUnderlineTypeAccessor(), mergedCharacterProperties.FontUnderlineType);
			if (characterProperties.UnderlineColor != mergedCharacterProperties.UnderlineColor)
				ChangeCurrentValue<Color>(new UnderlineColorAccessor(), mergedCharacterProperties.UnderlineColor);
			if (characterProperties.FontStrikeoutType != mergedCharacterProperties.FontStrikeoutType)
				ChangeCurrentValue<StrikeoutType>(new FontStrikeoutTypeAccessor(), mergedCharacterProperties.FontStrikeoutType);
			if (characterProperties.UnderlineWordsOnly != mergedCharacterProperties.UnderlineWordsOnly)
				ChangeCurrentValue<bool>(new UnderlineWordsOnlyAccessor(), mergedCharacterProperties.UnderlineWordsOnly);
			if (characterProperties.Script != mergedCharacterProperties.Script)
				ChangeCurrentValue<CharacterFormattingScript>(new ScriptAccessor(), mergedCharacterProperties.Script);
			if (characterProperties.AllCaps != mergedCharacterProperties.AllCaps)
				ChangeCurrentValue<bool>(new AllCapsAccessor(), mergedCharacterProperties.AllCaps);
			if (characterProperties.Hidden != mergedCharacterProperties.Hidden)
				ChangeCurrentValue<bool>(new HiddenAccessor(), mergedCharacterProperties.Hidden);
		}
		protected internal void CopyParagraphPropertiesFromMerged(MergedParagraphProperties mergedProperties) {
			ParagraphFormattingInfo mergedParagraphProperties = mergedProperties.Info;
			ParagraphFormattingInfo paragraphProperties = GetIntermediateMergedParagraphProperties(IntermediateParagraphStyle);
			if (paragraphProperties.Alignment != mergedParagraphProperties.Alignment)
				ChangeCurrentValue<ParagraphAlignment>(new AlignmentAccessor(), mergedParagraphProperties.Alignment);
			if (paragraphProperties.OutlineLevel != mergedParagraphProperties.OutlineLevel)
				ChangeCurrentValue<int>(new OutlineLevelAccessor(), mergedParagraphProperties.OutlineLevel);
			if (paragraphProperties.LeftIndent != mergedParagraphProperties.LeftIndent)
				ChangeCurrentValue<int>(new LeftIndentAccessor(), mergedParagraphProperties.LeftIndent);
			if (paragraphProperties.RightIndent != mergedParagraphProperties.RightIndent)
				ChangeCurrentValue<int>(new RightIndentAccessor(), mergedParagraphProperties.RightIndent);
			if (paragraphProperties.FirstLineIndentType != mergedParagraphProperties.FirstLineIndentType)
				ParagraphProperties.FirstLineIndentType = mergedParagraphProperties.FirstLineIndentType;
			if (paragraphProperties.FirstLineIndent != mergedParagraphProperties.FirstLineIndent)
				ChangeCurrentValue<int>(new FirstLineIndentAccessor(), mergedParagraphProperties.FirstLineIndent);
			if (paragraphProperties.SpacingBefore != mergedParagraphProperties.SpacingBefore)
				ChangeCurrentValue<int>(new SpacingBeforeAccessor(), mergedParagraphProperties.SpacingBefore);
			if (paragraphProperties.SpacingAfter != mergedParagraphProperties.SpacingAfter)
				ChangeCurrentValue<int>(new SpacingAfterAccessor(), mergedParagraphProperties.SpacingAfter);
			if (paragraphProperties.LineSpacingType != mergedParagraphProperties.LineSpacingType)
				ParagraphProperties.LineSpacingType = mergedParagraphProperties.LineSpacingType;
			if (paragraphProperties.LineSpacing != mergedParagraphProperties.LineSpacing)
				ChangeCurrentValue<float>(new LineSpacingAccessor(), mergedParagraphProperties.LineSpacing);
			if (paragraphProperties.BeforeAutoSpacing != mergedParagraphProperties.BeforeAutoSpacing)
				ChangeCurrentValue<bool>(new BeforeAutoSpacingAccessor(), mergedParagraphProperties.BeforeAutoSpacing);
			if (paragraphProperties.AfterAutoSpacing != mergedParagraphProperties.AfterAutoSpacing)
				ChangeCurrentValue<bool>(new AfterAutoSpacingAccessor(), mergedParagraphProperties.AfterAutoSpacing);
			if (paragraphProperties.ContextualSpacing != mergedParagraphProperties.ContextualSpacing)
				ChangeCurrentValue<bool>(new ContextualSpacingAccessor(), mergedParagraphProperties.ContextualSpacing);
			if (paragraphProperties.KeepLinesTogether != mergedParagraphProperties.KeepLinesTogether)
				ChangeCurrentValue<bool>(new KeepLinesTogetherAccessor(), mergedParagraphProperties.KeepLinesTogether);
			if (paragraphProperties.KeepWithNext != mergedParagraphProperties.KeepWithNext)
				ChangeCurrentValue<bool>(new KeepWithNextAccessor(), mergedParagraphProperties.KeepWithNext);
			if (paragraphProperties.PageBreakBefore != mergedParagraphProperties.PageBreakBefore)
				ChangeCurrentValue<bool>(new PageBreakBeforeAccessor(), mergedParagraphProperties.PageBreakBefore);
			if (paragraphProperties.SuppressLineNumbers != mergedParagraphProperties.SuppressLineNumbers)
				ChangeCurrentValue<bool>(new SuppressLineNumbersAccessor(), mergedParagraphProperties.SuppressLineNumbers);
			if (paragraphProperties.SuppressHyphenation != mergedParagraphProperties.SuppressHyphenation)
				ChangeCurrentValue<bool>(new SuppressHyphenationAccessor(), mergedParagraphProperties.SuppressHyphenation);
			if (paragraphProperties.WidowOrphanControl != mergedParagraphProperties.WidowOrphanControl)
				ChangeCurrentValue<bool>(new WidowOrphanControlAccessor(), mergedParagraphProperties.WidowOrphanControl);
			if (paragraphProperties.BackColor != mergedParagraphProperties.BackColor)
				ChangeCurrentValue<Color>(new BackColorAccessor(), mergedParagraphProperties.BackColor);
		}
		protected internal void IncreaseSpacing() {
			int newValue = UnitConverter.PointsToModelUnits(6);
			ParagraphProperties.SpacingBefore += newValue;
			ParagraphProperties.SpacingAfter += newValue;
		}
		protected internal void DecreaseSpacing() {
			int newValue = UnitConverter.PointsToModelUnits(6);
			if (ParagraphProperties.SpacingBefore < newValue)
				ParagraphProperties.SpacingBefore = 0;
			else
				ParagraphProperties.SpacingBefore -= newValue;
			if (ParagraphProperties.SpacingAfter < newValue)
				ParagraphProperties.SpacingAfter = 0;
			else
				ParagraphProperties.SpacingAfter -= newValue;
		}
		protected internal void DecreaseIndent() {
			int newValue = (int)UnitConverter.CentimetersToModelUnitsF(1.27f);
			if (ParagraphProperties.LeftIndent < newValue) {
				ParagraphProperties.LeftIndent = 0;
			}
			else
				ParagraphProperties.LeftIndent -= newValue;
		}
		protected internal void IncreaseIndent() {
			int newValue = (int)UnitConverter.CentimetersToModelUnitsF(1.27f);
			ParagraphProperties.LeftIndent += newValue;
		}
		public CharacterFormattingInfo GetIntermediateMergedCharacterProperties(ParagraphStyle style) {
			MergedCharacterProperties properties = style.GetMergedCharacterProperties();
			CharacterPropertiesMerger merger = new CharacterPropertiesMerger(properties);
			merger.Merge(style.DocumentModel.DefaultCharacterProperties);
			return merger.MergedProperties.Info;
		}
		public CharacterFormattingInfo GetIntermediateMergedCharacterProperties(CharacterStyle style) {
			MergedCharacterProperties properties = style.GetMergedCharacterProperties();
			CharacterPropertiesMerger merger = new CharacterPropertiesMerger(properties);
			merger.Merge(style.DocumentModel.DefaultCharacterProperties);
			return merger.MergedProperties.Info;
		}
		public ParagraphFormattingInfo GetIntermediateMergedParagraphProperties(ParagraphStyle style) {
			MergedParagraphProperties properties = style.GetMergedParagraphProperties();
			ParagraphPropertiesMerger merger = new ParagraphPropertiesMerger(properties);
			merger.Merge(style.DocumentModel.DefaultParagraphProperties);
			return merger.MergedProperties.Info;
		}
		#region IsValidName
		protected internal bool IsValidName(string name) {
			DocumentModel model = Control.InnerControl.DocumentModel;
			if (IsParagraphStyle)
				return EditStyleHelper.IsValidStyleName<ParagraphStyle>(name, ParagraphSourceStyle, model.ParagraphStyles);
			else
				return EditStyleHelper.IsValidStyleName<CharacterStyle>(name, CharacterSourceStyle, model.CharacterStyles);
		}
		#endregion
	}
}
