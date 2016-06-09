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
using DevExpress.XtraRichEdit.Model.History;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Forms {
	public abstract class TableStyleFormControllerParametersBase : FormControllerParameters {
		#region Fields
		bool currentStyleEnabled = true;
		#endregion
		protected TableStyleFormControllerParametersBase(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		internal bool CurrentStyleEnabled { get { return currentStyleEnabled; } set { currentStyleEnabled = value; } }
		#endregion
	}
	public class TableStyleFormControllerParameters : TableStyleFormControllerParametersBase {
		#region Fields
		TableStyle tableSourceStyle;
		#endregion
		public TableStyleFormControllerParameters(IRichEditControl control, TableStyle sourceStyle)
			: base(control) {
			Guard.ArgumentNotNull(sourceStyle, "style");
			this.tableSourceStyle = sourceStyle;
		}
		#region Properties
		internal TableStyle TableSourceStyle { get { return tableSourceStyle; } set { tableSourceStyle = value; } }
		#endregion
	}
	public abstract class TableStyleFormControllerBase : FormController {
		#region Fields
		static readonly List<string> tableContent = new List<string>() 
		{
			  " ", "Jan", "Feb", "Mar", "Total",
		   "East",   "7",   "7",   "5",	"19",
		   "West",   "6",   "4",   "7",	"17",
		  "South",   "8",   "7",   "9",	"24",
		  "Total",  "21",  "18",  "21",	"60" 
		};
		readonly FormControllerParameters parameters;
		readonly IRichEditControl control;
		readonly DocumentModel model;
		#endregion
		protected TableStyleFormControllerBase(IRichEditControl previewStyleControl, FormControllerParameters parameters) {
			Guard.ArgumentNotNull(parameters, "parameters");
			this.control = previewStyleControl;
			this.parameters = parameters;
			this.model = previewStyleControl.InnerDocumentServer.DocumentModel;
		}
		#region Properties
		public IRichEditControl Control { get { return control; } }
		public DocumentModel Model { get { return model; } }
		public FormControllerParameters Parameters { get { return parameters; } }
		public abstract int StyleIndex { get; }
		public abstract string StyleName { get; set; }
		public abstract CharacterProperties CharacterProperties { get; }
		public abstract ParagraphProperties ParagraphProperties { get; }
		public abstract TableCellProperties TableCellProperties { get; }
		public abstract TableProperties TableProperties { get; }
		public abstract ConditionalTableStyleFormattingTypes ConditionalStyleType { get; set; }
		public abstract TableConditionalStyleProperties ConditionalStyleProperties { get; }
		public abstract TabProperties Tabs { get; }
		public abstract ITableBorders CurrentBorders { get; }
		CharacterFormattingInfo CharacterProperiesInfo { get { return GetMergedCharacterProperties().Info; } }
		#endregion
		public abstract TabFormattingInfo GetTabInfo();
		public abstract MergedCharacterProperties GetMergedCharacterProperties();
		public abstract MergedParagraphProperties GetMergedParagraphProperties();
		public abstract CharacterFormattingInfo GetMergedWithDefaultCharacterProperties();
		public abstract CombinedCellPropertiesInfo GetMergedTableCellProperties();
		public abstract CharacterFormattingInfo GetParentCharacterProperties();
		public abstract ParagraphFormattingInfo GetParentParagraphProperties();
		public abstract ParagraphFormattingOptions GetParentParagraphPropertiesOptions();
		public abstract TableCellGeneralSettingsInfo GetParentTableCellProperties();
		public abstract TableCellPropertiesOptions GetParentTableCellPropertiesOptions();
		public abstract void AddStyle();
		public void ChangeConditionalCurrentValue<T>(CharacterPropertyAccessorBase<T> accesor, T isChecked) {
			AddStyle();
			accesor.Reset(CharacterProperties);
			CharacterFormattingInfo parent = GetParentCharacterProperties();
			if (parent == null || !isChecked.Equals(accesor.GetValue(parent)))
				accesor.SetValue(CharacterProperties, isChecked);
		}
		public void ChangeConditionalCurrentValue<T>(ParagraphPropertyAccessorBase<T> accesor, T isChecked) {
			AddStyle();
			accesor.Reset(ParagraphProperties);
			ParagraphFormattingInfo parent = GetParentParagraphProperties();
			ParagraphFormattingOptions parentOptions = GetParentParagraphPropertiesOptions();
			if (parent == null || !isChecked.Equals(accesor.GetValue(parent)) || !true.Equals(accesor.GetOptionsUseValue(parentOptions)))
				accesor.SetValue(ParagraphProperties, isChecked);
		}
		public void ChangeConditionalCurrentValue<T>(TableCellPropertyAccessorBase<T> accesor, T isChecked) {
			AddStyle();
			accesor.Reset(TableCellProperties);
			TableCellGeneralSettingsInfo parent = GetParentTableCellProperties();
			TableCellPropertiesOptions parentOptions = GetParentTableCellPropertiesOptions();
			if (parent == null || !isChecked.Equals(accesor.GetValue(parent)) || !true.Equals(accesor.GetOptionsUseValue(parentOptions)))
				accesor.SetValue(TableCellProperties, isChecked);
		}
		public void ChangeConditionalCurrentAlignmentValue(VerticalAlignment verticalAlignment, ParagraphAlignment paragraphAlignment) {
			ChangeConditionalCurrentValue<VerticalAlignment>(new VerticalAlignmentAccessor(), verticalAlignment);
			ChangeConditionalCurrentValue<ParagraphAlignment>(new AlignmentAccessor(), paragraphAlignment);
		}
		protected internal int GetColumnWidth(IRichEditControl richEditControl) {
			RichEditView activeView = richEditControl.InnerControl.ActiveView;
			DocumentModel model = richEditControl.InnerControl.DocumentModel;
			activeView.EnsureFormattingCompleteForSelection();
			DevExpress.XtraRichEdit.Layout.CaretPosition caretPosition = activeView.CaretPosition;
			DocumentModelUnitToLayoutUnitConverter unitConverter = model.ToDocumentLayoutUnitConverter;
			if (caretPosition.Update(DevExpress.XtraRichEdit.Layout.DocumentLayoutDetailsLevel.TableCell)) {
				DevExpress.XtraRichEdit.Layout.DocumentLayoutPosition layoutPosition = caretPosition.LayoutPosition;
				int width = layoutPosition.TableCell == null ? layoutPosition.Column.Bounds.Width : layoutPosition.TableCell.TextWidth;
				return unitConverter.ToModelUnits(width);
			}
			return 0;
		}
		protected Table InitializeTable(IRichEditControl richEditControl) {
			DocumentModel model = richEditControl.InnerDocumentServer.DocumentModel;
			PieceTable target = model.ActivePieceTable;
			Table tempTable;
			tempTable = target.InsertTable(model.Selection.End, 5, 5, TableAutoFitBehaviorType.FixedColumnWidth, Int32.MinValue, GetColumnWidth(richEditControl), true, false);
			DocumentLogPosition pos;
			for (int i = 0; i < 25; i++) {
				pos = DocumentModelPosition.FromParagraphStart(target, new ParagraphIndex(i)).LogPosition;
				target.InsertText(pos, tableContent[i]);
			}
			return tempTable;
		}
		protected abstract void SetCurrentStyle(Table tempTable);
		public void FillTempRichEdit(IRichEditControl richEditControl) {
			DocumentModel model = richEditControl.InnerDocumentServer.DocumentModel;
			PieceTable target = model.ActivePieceTable;
			Table tempTable;
			if (target.Tables.Count == 0)
				tempTable = InitializeTable(richEditControl);
			else
				tempTable = target.Tables[0];
			SetCurrentStyle(tempTable);
			DevExpress.XtraRichEdit.API.Native.Document document = richEditControl.Document;
			document.CaretPosition = document.CreatePosition(0);
			ConditionalTableStyleFormattingTypes allTypes = ConditionalTableStyleFormattingTypes.BottomLeftCell | ConditionalTableStyleFormattingTypes.BottomRightCell | ConditionalTableStyleFormattingTypes.EvenColumnBanding | ConditionalTableStyleFormattingTypes.EvenRowBanding | ConditionalTableStyleFormattingTypes.FirstColumn | ConditionalTableStyleFormattingTypes.FirstRow | ConditionalTableStyleFormattingTypes.LastColumn | ConditionalTableStyleFormattingTypes.LastRow | ConditionalTableStyleFormattingTypes.OddColumnBanding | ConditionalTableStyleFormattingTypes.OddRowBanding | ConditionalTableStyleFormattingTypes.TopLeftCell | ConditionalTableStyleFormattingTypes.TopRightCell | ConditionalTableStyleFormattingTypes.WholeTable;
			EditStyleHelper.ChangeConditionalType(tempTable, allTypes);
		}
		protected internal void CopyCharacterPropertiesFromMerged(MergedCharacterProperties mergedProperties) {
			CharacterFormattingInfo mergedCharacterProperties = mergedProperties.Info;
			CharacterFormattingInfo characterProperties = CharacterProperiesInfo;
			if (characterProperties.FontName != mergedCharacterProperties.FontName)
				ChangeConditionalCurrentValue<string>(new FontNameAccessor(), mergedCharacterProperties.FontName);
			if (characterProperties.FontBold != mergedCharacterProperties.FontBold)
				ChangeConditionalCurrentValue<bool>(new FontBoldAccessor(), mergedCharacterProperties.FontBold);
			if (characterProperties.FontItalic != mergedCharacterProperties.FontItalic)
				ChangeConditionalCurrentValue<bool>(new FontItalicAccessor(), mergedCharacterProperties.FontItalic);
			if (characterProperties.DoubleFontSize != mergedCharacterProperties.DoubleFontSize)
				ChangeConditionalCurrentValue<int>(new DoubleFontSizeAccessor(), mergedCharacterProperties.DoubleFontSize);
			if (characterProperties.ForeColor != mergedCharacterProperties.ForeColor)
				ChangeConditionalCurrentValue<Color>(new ForeColorAccessor(), mergedCharacterProperties.ForeColor);
			if (characterProperties.FontUnderlineType != mergedCharacterProperties.FontUnderlineType)
				ChangeConditionalCurrentValue<UnderlineType>(new FontUnderlineTypeAccessor(), mergedCharacterProperties.FontUnderlineType);
			if (characterProperties.UnderlineColor != mergedCharacterProperties.UnderlineColor)
				ChangeConditionalCurrentValue<Color>(new UnderlineColorAccessor(), mergedCharacterProperties.UnderlineColor);
			if (characterProperties.FontStrikeoutType != mergedCharacterProperties.FontStrikeoutType)
				ChangeConditionalCurrentValue<StrikeoutType>(new FontStrikeoutTypeAccessor(), mergedCharacterProperties.FontStrikeoutType);
			if (characterProperties.UnderlineWordsOnly != mergedCharacterProperties.UnderlineWordsOnly)
				ChangeConditionalCurrentValue<bool>(new UnderlineWordsOnlyAccessor(), mergedCharacterProperties.UnderlineWordsOnly);
			if (characterProperties.Script != mergedCharacterProperties.Script)
				ChangeConditionalCurrentValue<CharacterFormattingScript>(new ScriptAccessor(), mergedCharacterProperties.Script);
			if (characterProperties.AllCaps != mergedCharacterProperties.AllCaps)
				ChangeConditionalCurrentValue<bool>(new AllCapsAccessor(), mergedCharacterProperties.AllCaps);
			if (characterProperties.Hidden != mergedCharacterProperties.Hidden)
				ChangeConditionalCurrentValue<bool>(new HiddenAccessor(), mergedCharacterProperties.Hidden);
		}
		protected internal void CopyParagraphPropertiesFromMerged(MergedParagraphProperties mergedProperties) {
			ParagraphFormattingInfo mergedParagraphProperties = mergedProperties.Info;
			ParagraphFormattingInfo paragraphProperties = GetMergedParagraphProperties().Info;
			if (paragraphProperties.Alignment != mergedParagraphProperties.Alignment)
				ChangeConditionalCurrentValue<ParagraphAlignment>(new AlignmentAccessor(), mergedParagraphProperties.Alignment);
			if (paragraphProperties.OutlineLevel != mergedParagraphProperties.OutlineLevel)
				ChangeConditionalCurrentValue<int>(new OutlineLevelAccessor(), mergedParagraphProperties.OutlineLevel);
			if (paragraphProperties.LeftIndent != mergedParagraphProperties.LeftIndent)
				ChangeConditionalCurrentValue<int>(new LeftIndentAccessor(), mergedParagraphProperties.LeftIndent);
			if (paragraphProperties.RightIndent != mergedParagraphProperties.RightIndent)
				ChangeConditionalCurrentValue<int>(new RightIndentAccessor(), mergedParagraphProperties.RightIndent);
			if (paragraphProperties.FirstLineIndentType != mergedParagraphProperties.FirstLineIndentType)
				ParagraphProperties.FirstLineIndentType = mergedParagraphProperties.FirstLineIndentType;
			if (paragraphProperties.FirstLineIndent != mergedParagraphProperties.FirstLineIndent)
				ChangeConditionalCurrentValue<int>(new FirstLineIndentAccessor(), mergedParagraphProperties.FirstLineIndent);
			if (paragraphProperties.SpacingBefore != mergedParagraphProperties.SpacingBefore)
				ChangeConditionalCurrentValue<int>(new SpacingBeforeAccessor(), mergedParagraphProperties.SpacingBefore);
			if (paragraphProperties.SpacingAfter != mergedParagraphProperties.SpacingAfter)
				ChangeConditionalCurrentValue<int>(new SpacingAfterAccessor(), mergedParagraphProperties.SpacingAfter);
			if (paragraphProperties.LineSpacing != mergedParagraphProperties.LineSpacing)
				ChangeConditionalCurrentValue<float>(new LineSpacingAccessor(), mergedParagraphProperties.LineSpacing);
			if (paragraphProperties.LineSpacingType != mergedParagraphProperties.LineSpacingType)
				ParagraphProperties.LineSpacingType = mergedParagraphProperties.LineSpacingType;
			if (paragraphProperties.BeforeAutoSpacing != mergedParagraphProperties.BeforeAutoSpacing)
				ChangeConditionalCurrentValue<bool>(new BeforeAutoSpacingAccessor(), mergedParagraphProperties.BeforeAutoSpacing);
			if (paragraphProperties.AfterAutoSpacing != mergedParagraphProperties.AfterAutoSpacing)
				ChangeConditionalCurrentValue<bool>(new AfterAutoSpacingAccessor(), mergedParagraphProperties.AfterAutoSpacing);
			if (paragraphProperties.ContextualSpacing != mergedParagraphProperties.ContextualSpacing)
				ChangeConditionalCurrentValue<bool>(new ContextualSpacingAccessor(), mergedParagraphProperties.ContextualSpacing);
			if (paragraphProperties.KeepLinesTogether != mergedParagraphProperties.KeepLinesTogether)
				ChangeConditionalCurrentValue<bool>(new KeepLinesTogetherAccessor(), mergedParagraphProperties.KeepLinesTogether);
			if (paragraphProperties.KeepWithNext != mergedParagraphProperties.KeepWithNext)
				ChangeConditionalCurrentValue<bool>(new KeepWithNextAccessor(), mergedParagraphProperties.KeepWithNext);
			if (paragraphProperties.PageBreakBefore != mergedParagraphProperties.PageBreakBefore)
				ChangeConditionalCurrentValue<bool>(new PageBreakBeforeAccessor(), mergedParagraphProperties.PageBreakBefore);
			if (paragraphProperties.SuppressLineNumbers != mergedParagraphProperties.SuppressLineNumbers)
				ChangeConditionalCurrentValue<bool>(new SuppressLineNumbersAccessor(), mergedParagraphProperties.SuppressLineNumbers);
			if (paragraphProperties.SuppressHyphenation != mergedParagraphProperties.SuppressHyphenation)
				ChangeConditionalCurrentValue<bool>(new SuppressHyphenationAccessor(), mergedParagraphProperties.SuppressHyphenation);
			if (paragraphProperties.WidowOrphanControl != mergedParagraphProperties.WidowOrphanControl)
				ChangeConditionalCurrentValue<bool>(new WidowOrphanControlAccessor(), mergedParagraphProperties.WidowOrphanControl);
			if (paragraphProperties.BackColor != mergedParagraphProperties.BackColor)
				ChangeConditionalCurrentValue<Color>(new BackColorAccessor(), mergedParagraphProperties.BackColor);
		}
		public abstract bool IsValidName(string name);
	}
	public class TableStyleFormController : TableStyleFormControllerBase {
		#region Fields
		readonly TableStyle editedTableStyle;
		readonly int editedTableStyleIndex;
		#endregion
		public TableStyleFormController(IRichEditControl previewStyleControl, FormControllerParameters parameters)
			: base(previewStyleControl, parameters) {
			TableStyleCollection sourceTableStyles = parameters.Control.InnerControl.DocumentModel.TableStyles;
			DocumentModel previewModel = previewStyleControl.InnerDocumentServer.DocumentModel;
			TableCollection tables = previewModel.ActivePieceTable.Tables;
			for (int i = 0; i < tables.Count; i++) {
				previewModel.ActivePieceTable.DeleteTableCore(tables[i]);
			}
			while (previewModel.TableStyles.Count > 0)
				previewModel.TableStyles.RemoveLastStyle();
			foreach (TableStyle style in sourceTableStyles) {
				if (!style.Deleted) {
					previewModel.BeginUpdate();
					try {
						style.Copy(previewModel);
					}
					finally {
						previewModel.EndUpdate();
					}
				}
			}
			foreach (TableStyle style in previewModel.TableStyles) {
				TableStyle sourceStyle = sourceTableStyles.GetStyleByName(style.StyleName);
				style.ConditionalStyleProperties.CopyFrom(sourceStyle.ConditionalStyleProperties);
			}
			this.editedTableStyleIndex = ((TableStyleFormControllerParameters)parameters).TableSourceStyle.Copy(previewModel);
			this.editedTableStyle = previewModel.TableStyles[editedTableStyleIndex];
		}
		#region Properties
		public override string StyleName { get { return IntermediateTableStyle.LocalizedStyleName; } set { IntermediateTableStyle.StyleName = value; } }
		public override int StyleIndex { get { return editedTableStyleIndex; } }
		public TableStyle TableSourceStyle { get { return ((TableStyleFormControllerParameters)Parameters).TableSourceStyle; } }
		public TableStyle IntermediateTableStyle { get { return editedTableStyle; } }
		public override ITableBorders CurrentBorders { get { return ConditionalStyleType == ConditionalTableStyleFormattingTypes.WholeTable ? IntermediateTableStyle.TableProperties.Borders : (ITableBorders)TableCellProperties.Borders; } }
		public override CharacterProperties CharacterProperties { get { return CurrentConditionalStyle != null ? CurrentConditionalStyle.CharacterProperties : IntermediateTableStyle.CharacterProperties; } }
		public override ParagraphProperties ParagraphProperties { get { return CurrentConditionalStyle != null ? CurrentConditionalStyle.ParagraphProperties : IntermediateTableStyle.ParagraphProperties; } }
		public override TableCellProperties TableCellProperties { get { return CurrentConditionalStyle != null ? CurrentConditionalStyle.TableCellProperties : IntermediateTableStyle.TableCellProperties; } }
		public override TableProperties TableProperties { get { return CurrentConditionalStyle != null ? CurrentConditionalStyle.TableProperties : IntermediateTableStyle.TableProperties; } }
		public override ConditionalTableStyleFormattingTypes ConditionalStyleType { get; set; }
		public override TableConditionalStyleProperties ConditionalStyleProperties { get { return editedTableStyle.ConditionalStyleProperties; } }
		private TableConditionalStyle CurrentConditionalStyle { get { return ConditionalStyleProperties[ConditionalStyleType]; } }
		private TableStyle ConditionalStyleParent { get { return CurrentConditionalStyle.Parent; } }
		public override TabProperties Tabs { get { return IntermediateTableStyle.Tabs; } }
		#endregion
		public override TabFormattingInfo GetTabInfo() {
			return IntermediateTableStyle.GetTabs();
		}
		public override MergedCharacterProperties GetMergedCharacterProperties() {
			return CurrentConditionalStyle.GetMergedWithDefaultCharacterProperties();
		}
		public override MergedParagraphProperties GetMergedParagraphProperties() {
			return CurrentConditionalStyle.GetMergedWithDefaultParagraphProperties();
		}
		public override CharacterFormattingInfo GetMergedWithDefaultCharacterProperties() {
			if (CurrentConditionalStyle != null)
				return CurrentConditionalStyle.GetMergedWithDefaultCharacterProperties().Info;
			else
				return IntermediateTableStyle.GetMergedWithDefaultCharacterProperties(ConditionalRowType.Normal, ConditionalColumnType.Normal).Info;
		}
		public override CombinedCellPropertiesInfo GetMergedTableCellProperties() {
			if (CurrentConditionalStyle != null)
				return CurrentConditionalStyle.GetMergedTableCellProperties().Info;
			else
				return IntermediateTableStyle.GetMergedTableCellProperties().Info;
		}
		public override void ApplyChanges() {
			TableSourceStyle.BeginUpdate();
			try {
				TableSourceStyle.CopyProperties(IntermediateTableStyle);
				TableSourceStyle.ConditionalStyleProperties.CopyFrom(IntermediateTableStyle.ConditionalStyleProperties);
				TableSourceStyle.StyleName = IntermediateTableStyle.StyleName;
				if (IntermediateTableStyle.Parent != null)
					TableSourceStyle.Parent = TableSourceStyle.DocumentModel.TableStyles.GetStyleByName(IntermediateTableStyle.Parent.StyleName);
				else
					TableSourceStyle.Parent = null;
			}
			finally {
				TableSourceStyle.EndUpdate();
			}
		}
		public override CharacterFormattingInfo GetParentCharacterProperties() {
			return IntermediateTableStyle.Parent != null ? GetParentConditionalCharacterProperties() : null;
		}
		public override ParagraphFormattingInfo GetParentParagraphProperties() {
			return IntermediateTableStyle.Parent != null ? GetParentConditionalParagraphProperties() : null;
		}
		public override ParagraphFormattingOptions GetParentParagraphPropertiesOptions() {
			return IntermediateTableStyle.Parent != null ? GetParentConditionalParagraphPropertiesOptions() : null;
		}
		public override TableCellGeneralSettingsInfo GetParentTableCellProperties() {
			return IntermediateTableStyle.Parent != null ? GetParentConditionalTableCellProperties() : null;
		}
		public override TableCellPropertiesOptions GetParentTableCellPropertiesOptions() {
			return IntermediateTableStyle.Parent != null ? GetParentConditionalTableCellPropertiesOptions() : null;
		}
		public CharacterFormattingInfo GetParentConditionalCharacterProperties() {
			return ConditionalStyleParent != null ? ConditionalStyleParent.GetMergedCharacterProperties().Info : null;
		}
		public ParagraphFormattingInfo GetParentConditionalParagraphProperties() {
			return ConditionalStyleParent != null ? ConditionalStyleParent.GetMergedParagraphProperties().Info : null;
		}
		public ParagraphFormattingOptions GetParentConditionalParagraphPropertiesOptions() {
			return ConditionalStyleParent != null ? ConditionalStyleParent.GetMergedParagraphProperties().Options : null;
		}
		private TableCellGeneralSettingsInfo GetParentConditionalTableCellProperties() {
			return ConditionalStyleParent != null ? ConditionalStyleParent.GetMergedTableCellProperties().Info.GeneralSettings : null;
		}
		private TableCellPropertiesOptions GetParentConditionalTableCellPropertiesOptions() {
			return ConditionalStyleParent != null ? ConditionalStyleParent.GetMergedTableCellProperties().Options : null;
		}
		public void UnsubscribeConditionalStyleEvents() {
			if (CurrentConditionalStyle != null) {
				CurrentConditionalStyle.CharacterProperties.ObtainAffectedRange -= IntermediateTableStyle.OnObtainAffectedRange;
				CurrentConditionalStyle.ParagraphProperties.ObtainAffectedRange -= IntermediateTableStyle.OnObtainAffectedRange;
				CurrentConditionalStyle.TableCellProperties.ObtainAffectedRange -= IntermediateTableStyle.OnObtainAffectedRange;
				CurrentConditionalStyle.TableProperties.ObtainAffectedRange -= IntermediateTableStyle.OnObtainAffectedRange;
				CurrentConditionalStyle.TableRowProperties.ObtainAffectedRange -= IntermediateTableStyle.OnObtainAffectedRange;
			}
		}
		public void SubscribeConditionalStyleEvents() {
			if (CurrentConditionalStyle != null) {
				CurrentConditionalStyle.CharacterProperties.ObtainAffectedRange += IntermediateTableStyle.OnObtainAffectedRange;
				CurrentConditionalStyle.ParagraphProperties.ObtainAffectedRange += IntermediateTableStyle.OnObtainAffectedRange;
				CurrentConditionalStyle.TableCellProperties.ObtainAffectedRange += IntermediateTableStyle.OnObtainAffectedRange;
				CurrentConditionalStyle.TableProperties.ObtainAffectedRange += IntermediateTableStyle.OnObtainAffectedRange;
				CurrentConditionalStyle.TableRowProperties.ObtainAffectedRange += IntermediateTableStyle.OnObtainAffectedRange;
			}
		}
		protected override void SetCurrentStyle(Table tempTable) {
			tempTable.StyleIndex = StyleIndex;
		}
		public override void AddStyle() {
			if (CurrentConditionalStyle == null) {
				UnsubscribeConditionalStyleEvents();
				ConditionalStyleProperties.AddStyle(new TableConditionalStyle(IntermediateTableStyle, ConditionalStyleType));
				TableConditionalFormattingController controller = new TableConditionalFormattingController(Control.InnerDocumentServer.DocumentModel.MainPieceTable.Tables[0]);
				controller.ResetCachedProperties(0);
				SubscribeConditionalStyleEvents();
			}
		}
		public override bool IsValidName(string name) {
			DocumentModel model = TableSourceStyle.DocumentModel;
			return EditStyleHelper.IsValidStyleName<TableStyle>(name, TableSourceStyle, model.TableStyles);
		}
	}
}
