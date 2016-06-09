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
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model.History;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Model {
	#region ITableStyle
	public interface ITableStyle {
		TableProperties TableProperties { get; }
		TableRowProperties TableRowProperties { get; }
		TableCellProperties TableCellProperties { get; }
		CharacterProperties CharacterProperties { get; }
		ParagraphProperties ParagraphProperties { get; }
		TableStyle Parent { get; }
	}
	#endregion
	#region ITableCellStyle
	public interface ITableCellStyle {
		TableCellProperties TableCellProperties { get; }
		CharacterProperties CharacterProperties { get; }
		ParagraphProperties ParagraphProperties { get; }
		TableCellStyle Parent { get; }
	}
	#endregion
	#region TableConditionalStyle
	public class TableConditionalStyle : ICellPropertiesOwner, ICharacterPropertiesContainer, IParagraphPropertiesContainer, ITableStyle {
		#region Fields
		readonly ConditionalTableStyleFormattingTypes conditionType;
		readonly TableStyle owner;
		TableProperties tableProperties;
		TableRowProperties tableRowProperties;
		TableCellProperties tableCellProperties;
		ParagraphProperties paragraphProperties;
		TabProperties tabs;
		CharacterProperties characterProperties;
		#endregion
		public TableConditionalStyle(TableStyle owner, ConditionalTableStyleFormattingTypes conditionType) {
			Guard.ArgumentNotNull(owner, "owner");
			this.owner = owner;
			this.conditionType = conditionType;
		}
		#region Properties
		public DocumentModel DocumentModel { get { return owner.DocumentModel; } }
		public PieceTable PieceTable { get { return DocumentModel.MainPieceTable; } }
		public ConditionalTableStyleFormattingTypes ConditionType { get { return conditionType; } }
		#region ITableStyle properties
		public TableProperties TableProperties {
			get {
				if (conditionType == ConditionalTableStyleFormattingTypes.WholeTable)
					return owner.TableProperties;
				if (tableProperties == null)
					tableProperties = new TableProperties(DocumentModel.MainPieceTable);
				return tableProperties;
			}
		}
		public TableRowProperties TableRowProperties {
			get {
				if (tableRowProperties == null)
					tableRowProperties = new TableRowProperties(DocumentModel.MainPieceTable);
				return tableRowProperties;
			}
		}
		public TableCellProperties TableCellProperties {
			get {
				if (conditionType == ConditionalTableStyleFormattingTypes.WholeTable)
					return owner.TableCellProperties;
				if (tableCellProperties == null)
					tableCellProperties = new TableCellProperties(DocumentModel.MainPieceTable, this);
				return tableCellProperties;
			}
		}
		public CharacterProperties CharacterProperties {
			get {
				if (conditionType == ConditionalTableStyleFormattingTypes.WholeTable)
					return owner.CharacterProperties;
				if (characterProperties == null)
					characterProperties = new CharacterProperties(this);
				return characterProperties;
			}
		}
		public ParagraphProperties ParagraphProperties {
			get {
				if (conditionType == ConditionalTableStyleFormattingTypes.WholeTable)
					return owner.ParagraphProperties;
				if (paragraphProperties == null)
					paragraphProperties = new ParagraphProperties(this);
				return paragraphProperties;
			}
		}
		public TableStyle Parent { get { return owner.Parent; } set { } }
		#endregion
		internal TabProperties Tabs {
			get {
				if (tabs == null)
					tabs = new TabProperties(this);
				return tabs;
			}
		}
		#endregion
		public virtual TableProperties GetTableProperties(TablePropertiesOptions.Mask mask) {
			if (tableProperties != null && tableProperties.GetUse(mask))
				return TableProperties;
			return null;
		}
		public virtual TableRowProperties GetTableRowProperties(TableRowPropertiesOptions.Mask mask) {
			if (tableRowProperties != null && tableRowProperties.GetUse(mask))
				return TableRowProperties;
			return null;
		}
		public virtual TableCellProperties GetTableCellProperties(TableCellPropertiesOptions.Mask mask) {
			if (tableCellProperties != null && tableCellProperties.GetUse(mask))
				return TableCellProperties;
			return null;
		}
		public void CopyFrom(TableConditionalStyle condition) {
			this.TableProperties.CopyFrom(condition.TableProperties);
			this.TableRowProperties.CopyFrom(condition.TableRowProperties);
			this.TableCellProperties.CopyFrom(condition.TableCellProperties);
			if (Object.ReferenceEquals(DocumentModel, condition.DocumentModel)) {
				this.ParagraphProperties.CopyFrom(condition.ParagraphProperties);
				this.CharacterProperties.CopyFrom(condition.CharacterProperties);
			}
			else {
				this.ParagraphProperties.CopyFrom(condition.ParagraphProperties.Info);
				this.CharacterProperties.CopyFrom(condition.CharacterProperties.Info);
			}
		}
		#region ICellPropertiesOwner Members
		IndexChangedHistoryItemCore<DocumentModelChangeActions> ICellPropertiesOwner.CreateCellPropertiesChangedHistoryItem(TableCellProperties properties) {
			Debug.Assert(properties == TableCellProperties);
			return new IndexChangedHistoryItem<DocumentModelChangeActions>(DocumentModel.MainPieceTable, properties);
		}
		#endregion
		IndexChangedHistoryItemCore<DocumentModelChangeActions> ICharacterPropertiesContainer.CreateCharacterPropertiesChangedHistoryItem() {
			return new IndexChangedHistoryItem<DocumentModelChangeActions>(DocumentModel.MainPieceTable, CharacterProperties);
		}
		void ICharacterPropertiesContainer.OnCharacterPropertiesChanged() {
			DocumentModel.ResetDocumentFormattingCaches(ResetFormattingCacheType.Character);
		}
		IndexChangedHistoryItemCore<DocumentModelChangeActions> IParagraphPropertiesContainer.CreateParagraphPropertiesChangedHistoryItem() {
			return new IndexChangedHistoryItem<DocumentModelChangeActions>(DocumentModel.MainPieceTable, ParagraphProperties);
		}
		void IParagraphPropertiesContainer.OnParagraphPropertiesChanged() {
			DocumentModel.ResetDocumentFormattingCaches(ResetFormattingCacheType.Paragraph);
		}
		public MergedCharacterProperties GetMergedCharacterProperties() {
			if (owner.Deleted)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedStyleError);
			CharacterPropertiesMerger merger = new CharacterPropertiesMerger(new MergedCharacterProperties(new CharacterFormattingInfo(), new CharacterFormattingOptions()));
			merger.Merge(CharacterProperties);
			if (Parent != null)
				merger.Merge(Parent.GetMergedCharacterProperties(conditionType));
			return merger.MergedProperties;
		}
		public virtual MergedCharacterProperties GetMergedWithDefaultCharacterProperties() {
			CharacterPropertiesMerger merger = new CharacterPropertiesMerger(new MergedCharacterProperties(new CharacterFormattingInfo(), new CharacterFormattingOptions()));
			merger.Merge(CharacterProperties);
			merger.Merge(DocumentModel.DefaultCharacterProperties);
			return merger.MergedProperties;
		}
		public virtual MergedParagraphProperties GetMergedParagraphProperties() {
			if (owner.Deleted)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedStyleError);
			ParagraphPropertiesMerger merger = new ParagraphPropertiesMerger(new MergedParagraphProperties(new ParagraphFormattingInfo(), new ParagraphFormattingOptions()));
			merger.Merge(ParagraphProperties);
			if (Parent != null)
				merger.Merge(Parent.GetMergedParagraphProperties(conditionType));
			return merger.MergedProperties;
		}
		public virtual MergedParagraphProperties GetMergedWithDefaultParagraphProperties() {
			ParagraphPropertiesMerger merger = new ParagraphPropertiesMerger(new MergedParagraphProperties(new ParagraphFormattingInfo(), new ParagraphFormattingOptions()));
			merger.Merge(ParagraphProperties);
			merger.Merge(DocumentModel.DefaultParagraphProperties);
			return merger.MergedProperties;
		}
		public virtual MergedTableProperties GetMergedTableProperties() {
			if (owner.Deleted)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedStyleError);
			TablePropertiesMerger merger = new TablePropertiesMerger(TableProperties);
			if (Parent != null)
				merger.Merge(Parent.GetMergedTableProperties());
			return merger.MergedProperties;
		}
		public virtual MergedTableRowProperties GetMergedTableRowProperties() {
			if (owner.Deleted)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedStyleError);
			TableRowPropertiesMerger merger = new TableRowPropertiesMerger(TableRowProperties);
			if (Parent != null)
				merger.Merge(Parent.GetMergedTableRowProperties(conditionType));
			return merger.MergedProperties;
		}
		public virtual MergedTableCellProperties GetMergedTableCellProperties() {
			if (owner.Deleted)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedStyleError);
			TableCellPropertiesMerger merger = new TableCellPropertiesMerger(TableCellProperties);
			if (Parent != null)
				merger.Merge(Parent.GetMergedTableCellProperties(conditionType));
			return merger.MergedProperties;
		}
	}
	#endregion
	#region TableConditionalStyleProperties
	public class TableConditionalStyleProperties {
		static readonly ConditionalTableStyleFormattingTypes[] styleTypes;
		public static ConditionalTableStyleFormattingTypes[] StyleTypes { get { return styleTypes; } }
		static TableConditionalStyleProperties() {
			List<ConditionalTableStyleFormattingTypes> styleTypeList = new List<ConditionalTableStyleFormattingTypes>();
#if!SL
			foreach (ConditionalTableStyleFormattingTypes styleType in Enum.GetValues(typeof(ConditionalTableStyleFormattingTypes))) {
				styleTypeList.Add(styleType);
			}
#else
			styleTypeList.Add(ConditionalTableStyleFormattingTypes.FirstRow);
			styleTypeList.Add(ConditionalTableStyleFormattingTypes.LastRow);
			styleTypeList.Add(ConditionalTableStyleFormattingTypes.FirstColumn);
			styleTypeList.Add(ConditionalTableStyleFormattingTypes.LastColumn);
			styleTypeList.Add(ConditionalTableStyleFormattingTypes.OddColumnBanding);
			styleTypeList.Add(ConditionalTableStyleFormattingTypes.EvenColumnBanding);
			styleTypeList.Add(ConditionalTableStyleFormattingTypes.OddRowBanding);
			styleTypeList.Add(ConditionalTableStyleFormattingTypes.EvenRowBanding);
			styleTypeList.Add(ConditionalTableStyleFormattingTypes.TopRightCell);
			styleTypeList.Add(ConditionalTableStyleFormattingTypes.TopLeftCell);
			styleTypeList.Add(ConditionalTableStyleFormattingTypes.BottomRightCell);
			styleTypeList.Add(ConditionalTableStyleFormattingTypes.BottomLeftCell);
			styleTypeList.Add(ConditionalTableStyleFormattingTypes.WholeTable);
#endif
			styleTypes = styleTypeList.ToArray();
		}
		readonly Dictionary<ConditionalTableStyleFormattingTypes, TableConditionalStyle> items;
		readonly TableStyle owner;
		public TableConditionalStyleProperties(TableStyle owner) {
			this.items = new Dictionary<ConditionalTableStyleFormattingTypes, TableConditionalStyle>();
			this.owner = owner;
			InitializeItems(DocumentModel);
		}
		public DocumentModel DocumentModel { get { return owner.DocumentModel; } }
		public TableConditionalStyle this[ConditionalTableStyleFormattingTypes condition] { get { return items[condition]; } }
		internal Dictionary<ConditionalTableStyleFormattingTypes, TableConditionalStyle> Items { get { return items; } }
		public TableStyle Owner { get { return owner; } }
		public TableConditionalStyle GetStyleSafe(ConditionalTableStyleFormattingTypes condition) {
			TableConditionalStyle result = this[condition];
			if (result != null)
				return result;
			result = new TableConditionalStyle(Owner, condition);
			Items[condition] = result;
			return result;
		}
		public void AddStyle(TableConditionalStyle style) {
			ConditionalTableStyleFormattingTypes styleType = style.ConditionType;
			TableConditionalStyle oldStyle = Items[styleType];
			if (Items[styleType] != null) {
				oldStyle.CharacterProperties.ObtainAffectedRange -= owner.OnObtainAffectedRange;
				oldStyle.ParagraphProperties.ObtainAffectedRange -= owner.OnObtainAffectedRange;
				oldStyle.TableCellProperties.ObtainAffectedRange -= owner.OnObtainAffectedRange;
				oldStyle.TableProperties.ObtainAffectedRange -= owner.OnObtainAffectedRange;
				oldStyle.TableRowProperties.ObtainAffectedRange -= owner.OnObtainAffectedRange;
			}
			style.CharacterProperties.ObtainAffectedRange += owner.OnObtainAffectedRange;
			style.ParagraphProperties.ObtainAffectedRange += owner.OnObtainAffectedRange;
			style.TableCellProperties.ObtainAffectedRange += owner.OnObtainAffectedRange;
			style.TableProperties.ObtainAffectedRange += owner.OnObtainAffectedRange;
			style.TableRowProperties.ObtainAffectedRange += owner.OnObtainAffectedRange;
			Items[styleType] = style;
			TableProperties ownerTableProperties = Owner.TableProperties;
			bool isColumnBinding = styleType == ConditionalTableStyleFormattingTypes.EvenColumnBanding || styleType == ConditionalTableStyleFormattingTypes.OddColumnBanding;
			if (isColumnBinding && !ownerTableProperties.UseTableStyleColBandSize)
				ownerTableProperties.TableStyleColBandSize = 1;
			bool isRowBinding = styleType == ConditionalTableStyleFormattingTypes.EvenRowBanding || styleType == ConditionalTableStyleFormattingTypes.OddRowBanding;
			if (isRowBinding && !ownerTableProperties.UseTableStyleRowBandSize)
				ownerTableProperties.TableStyleRowBandSize = 1;
			TableCollection tables = style.DocumentModel.ActivePieceTable.Tables;
			for (int i = 0; i < tables.Count; i++) {
				TableConditionalFormattingController controller = new TableConditionalFormattingController(tables[i]);
				controller.ResetCachedProperties(0);
			}
		}
		public void CopyFrom(TableConditionalStyleProperties conditionalProperties) {
			foreach (ConditionalTableStyleFormattingTypes condition in StyleTypes) {
				CopyConditionalStyle(condition, conditionalProperties);
			}
		}
		void CopyConditionalStyle(ConditionalTableStyleFormattingTypes condition, TableConditionalStyleProperties conditionalProperties) {
			TableConditionalStyle sourceStyle = conditionalProperties[condition];
			if (sourceStyle != null) {
				TableConditionalStyle targetStyle = GetStyleSafe(condition);
				targetStyle.CopyFrom(sourceStyle);
			}
			else
				Items[condition] = null;
		}
		void InitializeItems(DocumentModel documentModel) {
			foreach (ConditionalTableStyleFormattingTypes condition in StyleTypes) {
				AddConditionalStyle(documentModel, condition);
			}
		}
		void AddConditionalStyle(DocumentModel documentModel, ConditionalTableStyleFormattingTypes condition) {
			Items.Add(condition, null);
		}
		public bool ContainsStyle(ConditionalTableStyleFormattingTypes conditionalTableStyleFormattingType) {
			return Items[conditionalTableStyleFormattingType] != null;
		}
		public bool HasNonNullStyle {
			get {
				foreach (ConditionalTableStyleFormattingTypes condition in StyleTypes)
					if (Items[condition] != null)
						return true;
				return false;
			}
		}
	}
	#endregion
	#region TableStyle
	public class TableStyle : ParagraphPropertiesBasedStyle<TableStyle>, ICharacterPropertiesContainer, ICellPropertiesOwner, ITableStyle {
		#region Fields
		TableProperties tableProperties;
		TableRowProperties tableRowProperties;
		TableCellProperties tableCellProperties;
		CharacterProperties characterProperties;
		TableConditionalStyleProperties conditionalStyleProperties;
		#endregion
		public TableStyle(DocumentModel documentModel)
			: this(documentModel, null) {
		}
		public TableStyle(DocumentModel documentModel, TableStyle parent)
			: this(documentModel, parent, String.Empty) {
		}
		public TableStyle(DocumentModel documentModel, TableStyle parent, string styleName)
			: base(documentModel, parent, styleName) {
			this.characterProperties = new CharacterProperties(this);
			SubscribeCharacterPropertiesEvents();
		}
		#region Properties
		public override StyleType Type { get { return StyleType.TableStyle; } }
		public bool HasConditionalStyleProperties { get { return conditionalStyleProperties != null && conditionalStyleProperties.HasNonNullStyle; } }
		public bool HasRowBandingStyleProperties {
			get {
				return conditionalStyleProperties != null &&
					(conditionalStyleProperties.ContainsStyle(ConditionalTableStyleFormattingTypes.OddRowBanding) || conditionalStyleProperties.ContainsStyle(ConditionalTableStyleFormattingTypes.EvenRowBanding));
			}
		}
		public bool HasColumnBandingStyleProperties {
			get {
				return conditionalStyleProperties != null &&
					(conditionalStyleProperties.ContainsStyle(ConditionalTableStyleFormattingTypes.OddColumnBanding) || conditionalStyleProperties.ContainsStyle(ConditionalTableStyleFormattingTypes.EvenColumnBanding));
			}
		}
		public TableConditionalStyleProperties ConditionalStyleProperties {
			get {
				if (conditionalStyleProperties == null)
					conditionalStyleProperties = new TableConditionalStyleProperties(this);
				return conditionalStyleProperties;
			}
		}
		#region ITableStyle properties
		public TableProperties TableProperties {
			get {
				if (tableProperties == null) {
					tableProperties = new TableProperties(DocumentModel.MainPieceTable);
					SubscribeTablePropertiesPropertiesEvents();
				}
				return tableProperties;
			}
		}
		public TableRowProperties TableRowProperties {
			get {
				if (tableRowProperties == null) {
					tableRowProperties = new TableRowProperties(DocumentModel.MainPieceTable);
					SubscribeTableRowPropertiesEvents();
				}
				return tableRowProperties;
			}
		}
		public TableCellProperties TableCellProperties {
			get {
				if (tableCellProperties == null) {
					tableCellProperties = new TableCellProperties(DocumentModel.MainPieceTable, this);
					SubscribeTableCellPropertiesEvents();
				}
				return tableCellProperties;
			}
		}
		public CharacterProperties CharacterProperties { get { return characterProperties; } }
		#endregion
		#endregion
		protected internal virtual void SubscribeCharacterPropertiesEvents() {
			CharacterProperties.ObtainAffectedRange += OnObtainAffectedRange;
		}
		protected internal virtual void SubscribeTablePropertiesPropertiesEvents() {
			tableProperties.ObtainAffectedRange += OnObtainAffectedRange;
		}
		protected internal virtual void SubscribeTableRowPropertiesEvents() {
			tableRowProperties.ObtainAffectedRange += OnObtainAffectedRange;
		}
		protected internal virtual void SubscribeTableCellPropertiesEvents() {
			tableCellProperties.ObtainAffectedRange += OnObtainAffectedRange;
		}
		protected internal virtual void OnObtainAffectedRange(object sender, ObtainAffectedRangeEventArgs e) {
			e.Start = RunIndex.Zero;
			e.End = RunIndex.MaxValue;
		}
		public void MergeConditionalProperties(CharacterPropertiesMerger merger, ConditionalRowType rowType, ConditionalColumnType columnType) {
			if (conditionalStyleProperties == null)
				return;
			Dictionary<ConditionalTableStyleFormattingTypes, TableConditionalStyle> conditionalStyles = conditionalStyleProperties.Items;
			TableConditionalStyle conditionalStyle = null;
			if (columnType == ConditionalColumnType.FirstColumn) {
				if (rowType == ConditionalRowType.FirstRow) {
					if (conditionalStyles.TryGetValue(ConditionalTableStyleFormattingTypes.TopLeftCell, out conditionalStyle) && conditionalStyle != null)
						merger.Merge(conditionalStyle.CharacterProperties);
				}
				else if (rowType == ConditionalRowType.LastRow) {
					if (conditionalStyles.TryGetValue(ConditionalTableStyleFormattingTypes.BottomLeftCell, out conditionalStyle) && conditionalStyle != null)
						merger.Merge(conditionalStyle.CharacterProperties);
				}
			}
			else if (columnType == ConditionalColumnType.LastColumn) {
				if (rowType == ConditionalRowType.FirstRow) {
					if (conditionalStyles.TryGetValue(ConditionalTableStyleFormattingTypes.TopRightCell, out conditionalStyle) && conditionalStyle != null)
						merger.Merge(conditionalStyle.CharacterProperties);
				}
				else if (rowType == ConditionalRowType.LastRow) {
					if (conditionalStyles.TryGetValue(ConditionalTableStyleFormattingTypes.BottomRightCell, out conditionalStyle) && conditionalStyle != null)
						merger.Merge(conditionalStyle.CharacterProperties);
				}
			}
			if (rowType == ConditionalRowType.FirstRow) {
				if (conditionalStyles.TryGetValue(ConditionalTableStyleFormattingTypes.FirstRow, out conditionalStyle) && conditionalStyle != null)
					merger.Merge(conditionalStyle.CharacterProperties);
			}
			else if (rowType == ConditionalRowType.LastRow) {
				if (conditionalStyles.TryGetValue(ConditionalTableStyleFormattingTypes.LastRow, out conditionalStyle) && conditionalStyle != null)
					merger.Merge(conditionalStyle.CharacterProperties);
			}
			if (columnType == ConditionalColumnType.FirstColumn) {
				if (conditionalStyles.TryGetValue(ConditionalTableStyleFormattingTypes.FirstColumn, out conditionalStyle) && conditionalStyle != null)
					merger.Merge(conditionalStyle.CharacterProperties);
			}
			else if (columnType == ConditionalColumnType.LastColumn) {
				if (conditionalStyles.TryGetValue(ConditionalTableStyleFormattingTypes.LastColumn, out conditionalStyle) && conditionalStyle != null)
					merger.Merge(conditionalStyle.CharacterProperties);
			}
			if (columnType == ConditionalColumnType.EvenColumnBand) {
				if (conditionalStyles.TryGetValue(ConditionalTableStyleFormattingTypes.EvenColumnBanding, out conditionalStyle) && conditionalStyle != null)
					merger.Merge(conditionalStyle.CharacterProperties);
			}
			else if (columnType == ConditionalColumnType.OddColumnBand) {
				if (conditionalStyles.TryGetValue(ConditionalTableStyleFormattingTypes.OddColumnBanding, out conditionalStyle) && conditionalStyle != null)
					merger.Merge(conditionalStyle.CharacterProperties);
			}
			if (rowType == ConditionalRowType.EvenRowBand) {
				if (conditionalStyles.TryGetValue(ConditionalTableStyleFormattingTypes.EvenRowBanding, out conditionalStyle) && conditionalStyle != null)
					merger.Merge(conditionalStyle.CharacterProperties);
			}
			else if (rowType == ConditionalRowType.OddRowBand) {
				if (conditionalStyles.TryGetValue(ConditionalTableStyleFormattingTypes.OddRowBanding, out conditionalStyle) && conditionalStyle != null)
					merger.Merge(conditionalStyle.CharacterProperties);
			}
			if (conditionalStyles.TryGetValue(ConditionalTableStyleFormattingTypes.WholeTable, out conditionalStyle) && conditionalStyle != null)
				merger.Merge(conditionalStyle.CharacterProperties);
		}
		public delegate bool MaskPrediacate(TableConditionalStyle conditionalStyle);
		void TryGetConditionalStyle(Dictionary<ConditionalTableStyleFormattingTypes, TableConditionalStyle> conditionalStyles, ConditionalTableStyleFormattingTypes styleType, out TableConditionalStyle result, MaskPrediacate maskPredicate) {
			conditionalStyles.TryGetValue(styleType, out result);
			if ((result == null || !maskPredicate(result)) && Parent != null)
				Parent.TryGetConditionalStyle(Parent.ConditionalStyleProperties.Items, styleType, out result, maskPredicate);
		}
		public TableConditionalStyle GetConditionalPropertiesSource(ConditionalRowType rowType, ConditionalColumnType columnType, TablePropertiesOptions.Mask mask, bool isBorderCell, out bool insideBorder) {
			insideBorder = !isBorderCell;
			if(object.ReferenceEquals(this.conditionalStyleProperties, null))
				return null;
			Dictionary<ConditionalTableStyleFormattingTypes, TableConditionalStyle> conditionalStyles = this.conditionalStyleProperties.Items;
			TableConditionalStyle result = null;
			if(columnType == ConditionalColumnType.FirstColumn) {
				if(rowType == ConditionalRowType.FirstRow) {
					TryGetConditionalStyle(conditionalStyles, ConditionalTableStyleFormattingTypes.TopLeftCell, out result, mask);
					if(!object.ReferenceEquals(result, null)) {
						insideBorder = false;
						return result;
					}
				}
				else if(rowType == ConditionalRowType.LastRow) {
					TryGetConditionalStyle(conditionalStyles, ConditionalTableStyleFormattingTypes.BottomLeftCell, out result, mask);
					if(!object.ReferenceEquals(result, null)) {
						insideBorder = false;
						return result;
					}
				}
			}
			else if(columnType == ConditionalColumnType.LastColumn) {
				if(rowType == ConditionalRowType.FirstRow) {
					TryGetConditionalStyle(conditionalStyles, ConditionalTableStyleFormattingTypes.TopRightCell, out result, mask);
					if(!object.ReferenceEquals(result, null)) {
						insideBorder = false;
						return result;
					}
				}
				else if(rowType == ConditionalRowType.LastRow) {
					TryGetConditionalStyle(conditionalStyles, ConditionalTableStyleFormattingTypes.BottomRightCell, out result, mask);
					if(!object.ReferenceEquals(result, null)) {
						insideBorder = false;
						return result;
					}
				}
			}
			bool rowConditionBorder = isBorderCell || ((mask & (TablePropertiesOptions.Mask.UseBottomBorder | TablePropertiesOptions.Mask.UseTopBorder)) != 0);
			bool columnConditionBorder = isBorderCell || ((mask & (TablePropertiesOptions.Mask.UseLeftBorder | TablePropertiesOptions.Mask.UseRightBorder)) != 0);			
			TablePropertiesOptions.Mask rowMask = Table.OuterOrInside(mask, rowConditionBorder);
			TablePropertiesOptions.Mask columnMask = Table.OuterOrInside(mask, columnConditionBorder);
			if(rowType == ConditionalRowType.FirstRow) {
				TryGetConditionalStyle(conditionalStyles, ConditionalTableStyleFormattingTypes.FirstRow, out result, rowMask);
				if(!object.ReferenceEquals(result, null)) {
					insideBorder = !rowConditionBorder;
					return result;
				}
			}
			else if(rowType == ConditionalRowType.LastRow) {
				TryGetConditionalStyle(conditionalStyles, ConditionalTableStyleFormattingTypes.LastRow, out result, rowMask);
				if(!object.ReferenceEquals(result, null)) {
					insideBorder = !rowConditionBorder;
					return result;
				}
			}			
			if(columnType == ConditionalColumnType.FirstColumn) {
				TryGetConditionalStyle(conditionalStyles, ConditionalTableStyleFormattingTypes.FirstColumn, out result, columnMask);
				if(!object.ReferenceEquals(result, null)) {
					insideBorder = !columnConditionBorder;
					return result;
				}
			}
			else if(columnType == ConditionalColumnType.LastColumn) {
				TryGetConditionalStyle(conditionalStyles, ConditionalTableStyleFormattingTypes.LastColumn, out result, columnMask);
				if(!object.ReferenceEquals(result, null)) {
					insideBorder = !columnConditionBorder;
					return result;
				}
			}
			else if(columnType == ConditionalColumnType.EvenColumnBand) {
				TryGetConditionalStyle(conditionalStyles, ConditionalTableStyleFormattingTypes.EvenColumnBanding, out result, columnMask);
				if(!object.ReferenceEquals(result, null)) {
					insideBorder = !columnConditionBorder;
					return result;
				}
			}
			else if(columnType == ConditionalColumnType.OddColumnBand) {
				TryGetConditionalStyle(conditionalStyles, ConditionalTableStyleFormattingTypes.OddColumnBanding, out result, columnMask);
				if(!object.ReferenceEquals(result, null)) {
					insideBorder = !columnConditionBorder;
					return result;
				}
			}
			if(rowType == ConditionalRowType.EvenRowBand) {
				TryGetConditionalStyle(conditionalStyles, ConditionalTableStyleFormattingTypes.EvenRowBanding, out result, rowMask);
				if(!object.ReferenceEquals(result, null)) {
					insideBorder = !rowConditionBorder;
					return result;
				}
			}
			else if(rowType == ConditionalRowType.OddRowBand) {
				TryGetConditionalStyle(conditionalStyles, ConditionalTableStyleFormattingTypes.OddRowBanding, out result, rowMask);
				if(!object.ReferenceEquals(result, null)) {
					insideBorder = !rowConditionBorder;
					return result;
				}
			}
			TryGetConditionalStyle(conditionalStyles, ConditionalTableStyleFormattingTypes.WholeTable, out result, Table.OuterOrInside(mask, isBorderCell));
			return result;
		}
		void TryGetConditionalStyle(Dictionary<ConditionalTableStyleFormattingTypes, TableConditionalStyle> conditionalStyles, ConditionalTableStyleFormattingTypes styleType, out TableConditionalStyle result, TablePropertiesOptions.Mask mask) {
			TryGetConditionalStyle(conditionalStyles, styleType, out result, style => style.TableProperties.GetUse(mask));
		}
		public TableConditionalStyle GetConditionalPropertiesSource(ConditionalRowType rowType, ConditionalColumnType columnType, MaskPrediacate maskPredicate) {
			if (conditionalStyleProperties == null)
				return null;
			Dictionary<ConditionalTableStyleFormattingTypes, TableConditionalStyle> conditionalStyles = conditionalStyleProperties.Items;
			TableConditionalStyle result = null;
			if (columnType == ConditionalColumnType.FirstColumn) {
				if (rowType == ConditionalRowType.FirstRow) {
					TryGetConditionalStyle(conditionalStyles, ConditionalTableStyleFormattingTypes.TopLeftCell, out result, maskPredicate);
					if (result != null)
						return result;
				}
				else if (rowType == ConditionalRowType.LastRow) {
					TryGetConditionalStyle(conditionalStyles, ConditionalTableStyleFormattingTypes.BottomLeftCell, out result, maskPredicate);
					if (result != null)
						return result;
				}
			}
			else if (columnType == ConditionalColumnType.LastColumn) {
				if (rowType == ConditionalRowType.FirstRow) {
					TryGetConditionalStyle(conditionalStyles, ConditionalTableStyleFormattingTypes.TopRightCell, out result, maskPredicate);
					if (result != null)
						return result;
				}
				else if (rowType == ConditionalRowType.LastRow) {
					TryGetConditionalStyle(conditionalStyles, ConditionalTableStyleFormattingTypes.BottomRightCell, out result, maskPredicate);
					if (result != null)
						return result;
				}
			}
			if (rowType == ConditionalRowType.FirstRow) {
				TryGetConditionalStyle(conditionalStyles, ConditionalTableStyleFormattingTypes.FirstRow, out result, maskPredicate);
				if (result != null)
					return result;
			}
			else if (rowType == ConditionalRowType.LastRow) {
				TryGetConditionalStyle(conditionalStyles, ConditionalTableStyleFormattingTypes.LastRow, out result, maskPredicate);
				if (result != null)
					return result;
			}
			if (columnType == ConditionalColumnType.FirstColumn) {
				TryGetConditionalStyle(conditionalStyles, ConditionalTableStyleFormattingTypes.FirstColumn, out result, maskPredicate);
				if (result != null)
					return result;
			}
			else if (columnType == ConditionalColumnType.LastColumn) {
				TryGetConditionalStyle(conditionalStyles, ConditionalTableStyleFormattingTypes.LastColumn, out result, maskPredicate);
				if (result != null)
					return result;
			}
			if (columnType == ConditionalColumnType.EvenColumnBand) {
				TryGetConditionalStyle(conditionalStyles, ConditionalTableStyleFormattingTypes.EvenColumnBanding, out result, maskPredicate);
				if (result != null)
					return result;
			}
			else if (columnType == ConditionalColumnType.OddColumnBand) {
				TryGetConditionalStyle(conditionalStyles, ConditionalTableStyleFormattingTypes.OddColumnBanding, out result, maskPredicate);
				if (result != null)
					return result;
			}
			if (rowType == ConditionalRowType.EvenRowBand) {
				TryGetConditionalStyle(conditionalStyles, ConditionalTableStyleFormattingTypes.EvenRowBanding, out result, maskPredicate);
				if (result != null)
					return result;
			}
			else if (rowType == ConditionalRowType.OddRowBand) {
				TryGetConditionalStyle(conditionalStyles, ConditionalTableStyleFormattingTypes.OddRowBanding, out result, maskPredicate);
				if (result != null)
					return result;
			}
			TryGetConditionalStyle(conditionalStyles, ConditionalTableStyleFormattingTypes.WholeTable, out result, maskPredicate);
			if (result != null)
				return result;
			return null;
		}
		public ConditionalTableStyleFormattingTypes GetConditionalPropertiesMask(ConditionalRowType rowType, ConditionalColumnType columnType) {
			if (conditionalStyleProperties == null)
				return 0;
			ConditionalTableStyleFormattingTypes result = 0;
			Dictionary<ConditionalTableStyleFormattingTypes, TableConditionalStyle> conditionalStyles = conditionalStyleProperties.Items;
			TableConditionalStyle condStyle;
			if (columnType == ConditionalColumnType.FirstColumn) {
				if (rowType == ConditionalRowType.FirstRow) {
					if (conditionalStyles.TryGetValue(ConditionalTableStyleFormattingTypes.TopLeftCell, out condStyle) && condStyle != null)
						result |= ConditionalTableStyleFormattingTypes.TopLeftCell;
				}
				else if (rowType == ConditionalRowType.LastRow) {
					if (conditionalStyles.TryGetValue(ConditionalTableStyleFormattingTypes.BottomLeftCell, out condStyle) && condStyle != null)
						result |= ConditionalTableStyleFormattingTypes.BottomLeftCell;
				}
			}
			else if (columnType == ConditionalColumnType.LastColumn) {
				if (rowType == ConditionalRowType.FirstRow) {
					if (conditionalStyles.TryGetValue(ConditionalTableStyleFormattingTypes.TopRightCell, out condStyle) && condStyle != null)
						result |= ConditionalTableStyleFormattingTypes.TopRightCell;
				}
				else if (rowType == ConditionalRowType.LastRow) {
					if (conditionalStyles.TryGetValue(ConditionalTableStyleFormattingTypes.BottomRightCell, out condStyle) && condStyle != null)
						result |= ConditionalTableStyleFormattingTypes.BottomRightCell;
				}
			}
			if (rowType == ConditionalRowType.FirstRow) {
				if (conditionalStyles.TryGetValue(ConditionalTableStyleFormattingTypes.FirstRow, out condStyle) && condStyle != null)
					result |= ConditionalTableStyleFormattingTypes.FirstRow;
			}
			else if (rowType == ConditionalRowType.LastRow) {
				if (conditionalStyles.TryGetValue(ConditionalTableStyleFormattingTypes.LastRow, out condStyle) && condStyle != null)
					result |= ConditionalTableStyleFormattingTypes.LastRow;
			}
			if (columnType == ConditionalColumnType.FirstColumn) {
				if (conditionalStyles.TryGetValue(ConditionalTableStyleFormattingTypes.FirstColumn, out condStyle) && condStyle != null)
					result |= ConditionalTableStyleFormattingTypes.FirstColumn;
			}
			else if (columnType == ConditionalColumnType.LastColumn) {
				if (conditionalStyles.TryGetValue(ConditionalTableStyleFormattingTypes.LastColumn, out condStyle) && condStyle != null)
					result |= ConditionalTableStyleFormattingTypes.LastColumn;
			}
			if (columnType == ConditionalColumnType.EvenColumnBand) {
				if (conditionalStyles.TryGetValue(ConditionalTableStyleFormattingTypes.EvenColumnBanding, out condStyle) && condStyle != null)
					result |= ConditionalTableStyleFormattingTypes.EvenColumnBanding;
			}
			else if (columnType == ConditionalColumnType.OddColumnBand) {
				if (conditionalStyles.TryGetValue(ConditionalTableStyleFormattingTypes.OddColumnBanding, out condStyle) && condStyle != null)
					result |= ConditionalTableStyleFormattingTypes.OddColumnBanding;
			}
			if (rowType == ConditionalRowType.EvenRowBand) {
				if (conditionalStyles.TryGetValue(ConditionalTableStyleFormattingTypes.EvenRowBanding, out condStyle) && condStyle != null)
					result |= ConditionalTableStyleFormattingTypes.EvenRowBanding;
			}
			else if (rowType == ConditionalRowType.OddRowBand) {
				if (conditionalStyles.TryGetValue(ConditionalTableStyleFormattingTypes.OddRowBanding, out condStyle) && condStyle != null)
					result |= ConditionalTableStyleFormattingTypes.OddRowBanding;
			}
			if (conditionalStyles.TryGetValue(ConditionalTableStyleFormattingTypes.WholeTable, out condStyle) && condStyle != null)
				result |= ConditionalTableStyleFormattingTypes.WholeTable;
			return result;
		}
		public TableProperties GetTableProperties(TablePropertiesOptions.Mask mask, ConditionalRowType rowType, ConditionalColumnType columnType) {
			bool insideBorder;
			return GetTableProperties(mask, rowType, columnType, true, out insideBorder);
		}
		public virtual TableProperties GetTableProperties(TablePropertiesOptions.Mask mask, ConditionalRowType rowType, ConditionalColumnType columnType, bool isBorderCell, out bool insideBorder) {
			if (Deleted)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedStyleError);
			TableConditionalStyle conditionalStyle = GetConditionalPropertiesSource(rowType, columnType, mask, isBorderCell, out insideBorder);
			if(conditionalStyle != null)
				return conditionalStyle.TableProperties;
			insideBorder = !isBorderCell;
			if(tableProperties != null && tableProperties.GetUse(Table.OuterOrInside(mask, isBorderCell)))
				return TableProperties;
			return Parent != null ? Parent.GetTableProperties(mask, rowType, columnType, isBorderCell, out insideBorder) : null;
		}
		public virtual MergedCharacterProperties GetMergedCharacterProperties(ConditionalRowType rowType, ConditionalColumnType columnType) {
			if (Deleted)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedStyleError);
			CharacterPropertiesMerger merger = new CharacterPropertiesMerger(new MergedCharacterProperties(new CharacterFormattingInfo(), new CharacterFormattingOptions()));
			MergeConditionalProperties(merger, rowType, columnType);
			merger.Merge(CharacterProperties);
			if (Parent != null)
				merger.Merge(Parent.GetMergedCharacterProperties(rowType, columnType));
			return merger.MergedProperties;
		}
		public virtual MergedCharacterProperties GetMergedCharacterProperties() {
			return GetMergedCharacterProperties(ConditionalRowType.Normal, ConditionalColumnType.Normal);
		}
		public virtual MergedCharacterProperties GetMergedWithDefaultCharacterProperties(ConditionalRowType rowType, ConditionalColumnType columnType) {
			if (Deleted)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedStyleError);
			CharacterPropertiesMerger merger = new CharacterPropertiesMerger(new MergedCharacterProperties(new CharacterFormattingInfo(), new CharacterFormattingOptions()));
			MergeConditionalProperties(merger, rowType, columnType);
			merger.Merge(CharacterProperties);
			merger.Merge(DocumentModel.DefaultCharacterProperties);
			return merger.MergedProperties;
		}
		public virtual MergedTableProperties GetMergedTableProperties() {
			if (Deleted)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedStyleError);
			TablePropertiesMerger merger = new TablePropertiesMerger(TableProperties);
			if (Parent != null)
				merger.Merge(Parent.GetMergedTableProperties());
			return merger.MergedProperties;
		}
		public virtual MergedTableProperties GetMergedWithDefaultTableProperties() {
			if (Deleted)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedStyleError);
			TablePropertiesMerger merger = new TablePropertiesMerger(GetMergedTableProperties());
			merger.Merge(DocumentModel.DefaultTableProperties);
			return merger.MergedProperties;
		}
		public virtual MergedTableRowProperties GetMergedWithDefaultTableRowProperties() {
			if (Deleted)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedStyleError);
			TableRowPropertiesMerger merger = new TableRowPropertiesMerger(GetMergedTableRowProperties());
			merger.Merge(DocumentModel.DefaultTableRowProperties);
			return merger.MergedProperties;
		}
		public virtual MergedTableCellProperties GetMergedWithDefaultTableCellProperties() {
			if (Deleted)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedStyleError);
			TableCellPropertiesMerger merger = new TableCellPropertiesMerger(GetMergedTableCellProperties());
			merger.Merge(DocumentModel.DefaultTableCellProperties);
			return merger.MergedProperties;
		}
		public virtual TableCellProperties GetTableCellProperties(TableCellPropertiesOptions.Mask mask, ConditionalRowType rowType, ConditionalColumnType columnType) {
			if (Deleted)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedStyleError);
			TableConditionalStyle conditionalStyle = GetConditionalPropertiesSource(rowType, columnType, style => style.TableCellProperties.GetUse(mask));
			if (conditionalStyle != null)
				return conditionalStyle.TableCellProperties;
			if (tableCellProperties != null && tableCellProperties.GetUse(mask))
				return tableCellProperties;
			return Parent != null ? Parent.GetTableCellProperties(mask, rowType, columnType) : null;
		}
		public virtual CharacterProperties GetCharacterProperties(CharacterFormattingOptions.Mask mask, ConditionalRowType rowType, ConditionalColumnType columnType) {
			if (Deleted)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedStyleError);
			TableConditionalStyle conditionalStyle = GetConditionalPropertiesSource(rowType, columnType, style => style.CharacterProperties.UseVal(mask));
			if (conditionalStyle != null)
				return conditionalStyle.CharacterProperties;
			if (characterProperties != null && characterProperties.UseVal(mask))
				return characterProperties;
			return Parent != null ? Parent.GetCharacterProperties(mask, rowType, columnType) : null;
		}
		public virtual ParagraphProperties GetParagraphProperties(ParagraphFormattingOptions.Mask mask, ConditionalRowType rowType, ConditionalColumnType columnType) {
			if (Deleted)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedStyleError);
			TableConditionalStyle conditionalStyle = GetConditionalPropertiesSource(rowType, columnType, style => style.ParagraphProperties.UseVal(mask));
			if (conditionalStyle != null)
				return conditionalStyle.ParagraphProperties;
			if (ParagraphProperties != null && ParagraphProperties.UseVal(mask))
				return ParagraphProperties;
			return Parent != null ? Parent.GetParagraphProperties(mask, rowType, columnType) : null;
		}
		public virtual MergedParagraphProperties GetMergedParagraphProperties(ConditionalRowType rowType, ConditionalColumnType columnType) {
			if (Deleted)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedStyleError);
			ParagraphPropertiesMerger merger = new ParagraphPropertiesMerger(new MergedParagraphProperties(new ParagraphFormattingInfo(), new ParagraphFormattingOptions()));
			GetConditionalPropertiesSource(rowType, columnType, style => { merger.Merge(style.ParagraphProperties); return false; });
			if (ParagraphProperties != null)
				merger.Merge(ParagraphProperties);
			if (Parent != null)
				merger.Merge(Parent.GetMergedParagraphProperties(rowType, columnType));
			return merger.MergedProperties;
		}
		public override MergedParagraphProperties GetMergedParagraphProperties() {
			return GetMergedParagraphProperties(ConditionalRowType.Normal, ConditionalColumnType.Normal);
		}
		public virtual TableRowProperties GetTableRowProperties(TableRowPropertiesOptions.Mask mask, ConditionalRowType rowType) {
			if (Deleted)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedStyleError);
			TableConditionalStyle conditionalStyle = GetConditionalPropertiesSource(rowType, ConditionalColumnType.Normal, style => style.TableRowProperties.GetUse(mask));
			if (conditionalStyle != null)
				return conditionalStyle.TableRowProperties;
			if (tableRowProperties != null && tableRowProperties.GetUse(mask))
				return tableRowProperties;
			return Parent != null ? Parent.GetTableRowProperties(mask, rowType) : null;
		}
		protected override void MergePropertiesWithParent() {
			base.MergePropertiesWithParent();
			TableProperties.Merge(Parent.TableProperties);
			TableRowProperties.Merge(Parent.TableRowProperties);
			TableCellProperties.Merge(Parent.TableCellProperties);
			CharacterProperties.Merge(Parent.CharacterProperties);
		}
		protected internal override void CopyProperties(TableStyle source) {
			base.CopyProperties(source);
			DevExpress.XtraRichEdit.Model.History.TableConditionalFormattingController.ResetTablesCachedProperties(DocumentModel);
			TableProperties.CopyFrom(source.TableProperties);
			ConditionalStyleProperties.CopyFrom(source.ConditionalStyleProperties);
			TableRowProperties.CopyFrom(source.TableRowProperties);
			TableCellProperties.CopyFrom(source.TableCellProperties);
			CharacterProperties.CopyFrom(source.CharacterProperties.Info);
		}
		public override int Copy(DocumentModel targetModel) {
			TableStyleCollection tableStyles = targetModel.TableStyles;
			for (int i = 0; i < tableStyles.Count; i++) {
				if (this.StyleName == tableStyles[i].StyleName)
					return i;
			}
			return targetModel.TableStyles.AddNewStyle(CopyTo(targetModel));
		}
		protected internal virtual TableStyle CopyTo(DocumentModel targetModel) {
			TableStyle style = new TableStyle(targetModel);
			style.StyleName = this.StyleName;
			style.CopyProperties(this);
			if (HasConditionalStyleProperties)
				style.ConditionalStyleProperties.CopyFrom(this.ConditionalStyleProperties);
			if (Parent != null)
				style.Parent = targetModel.TableStyles[Parent.Copy(targetModel)];
			ApplyPropertiesDiff(style);
			return style;
		}
		protected internal void ApplyPropertiesDiff(TableStyle style) {
			ParagraphProperties.ApplyPropertiesDiff(style.ParagraphProperties, style.GetMergedWithDefaultParagraphProperties().Info, this.GetMergedWithDefaultParagraphProperties().Info);
			CharacterProperties.ApplyPropertiesDiff(style.CharacterProperties, style.GetMergedWithDefaultCharacterProperties(ConditionalRowType.Normal, ConditionalColumnType.Normal).Info, this.GetMergedWithDefaultCharacterProperties(ConditionalRowType.Normal, ConditionalColumnType.Normal).Info);
		}
		#region ICharacterPropertiesContainer Members
		public IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateCharacterPropertiesChangedHistoryItem() {
			return new IndexChangedHistoryItem<DocumentModelChangeActions>(DocumentModel.MainPieceTable, CharacterProperties);
		}
		public void OnCharacterPropertiesChanged() {
			DocumentModel.ResetDocumentFormattingCaches(ResetFormattingCacheType.Character);
		}
		#endregion
		#region ICellPropertiesOwner Members
		IndexChangedHistoryItemCore<DocumentModelChangeActions> ICellPropertiesOwner.CreateCellPropertiesChangedHistoryItem(TableCellProperties properties) {
			Debug.Assert(properties == TableCellProperties);
			return new IndexChangedHistoryItem<DocumentModelChangeActions>(properties.PieceTable, properties);
		}
		#endregion
		public override void ResetCachedIndices(ResetFormattingCacheType resetFormattingCacheType) {
		}
		protected internal bool HasColumnStyle(ConditionalColumnType conditionalColumnType) {
			if (conditionalStyleProperties != null) {
				if (conditionalColumnType == ConditionalColumnType.FirstColumn)
					return conditionalStyleProperties.ContainsStyle(ConditionalTableStyleFormattingTypes.FirstColumn)
						|| conditionalStyleProperties.ContainsStyle(ConditionalTableStyleFormattingTypes.TopLeftCell)
						|| conditionalStyleProperties.ContainsStyle(ConditionalTableStyleFormattingTypes.BottomLeftCell);
				else if (conditionalColumnType == ConditionalColumnType.LastColumn)
					return conditionalStyleProperties.ContainsStyle(ConditionalTableStyleFormattingTypes.LastColumn)
						|| conditionalStyleProperties.ContainsStyle(ConditionalTableStyleFormattingTypes.TopRightCell)
						|| conditionalStyleProperties.ContainsStyle(ConditionalTableStyleFormattingTypes.BottomRightCell);
			}
			return Parent != null ? Parent.HasColumnStyle(conditionalColumnType) : false;
		}
		protected internal bool HasRowStyle(ConditionalRowType conditionalRowType) {
			if (conditionalStyleProperties != null) {
				if (conditionalRowType == ConditionalRowType.FirstRow)
					return conditionalStyleProperties.ContainsStyle(ConditionalTableStyleFormattingTypes.FirstRow)
						|| conditionalStyleProperties.ContainsStyle(ConditionalTableStyleFormattingTypes.TopLeftCell)
						|| conditionalStyleProperties.ContainsStyle(ConditionalTableStyleFormattingTypes.TopRightCell);
				else if (conditionalRowType == ConditionalRowType.LastRow)
					return conditionalStyleProperties.ContainsStyle(ConditionalTableStyleFormattingTypes.LastRow)
						|| conditionalStyleProperties.ContainsStyle(ConditionalTableStyleFormattingTypes.BottomLeftCell)
						|| conditionalStyleProperties.ContainsStyle(ConditionalTableStyleFormattingTypes.BottomRightCell);
			}
			return Parent != null ? Parent.HasRowStyle(conditionalRowType) : false;
		}
		internal MergedCharacterProperties GetMergedCharacterProperties(ConditionalTableStyleFormattingTypes conditionType) {
			if (conditionalStyleProperties != null && this.conditionalStyleProperties.ContainsStyle(conditionType))
				return this.conditionalStyleProperties.Items[conditionType].GetMergedCharacterProperties();
			if (Parent != null)
				return Parent.GetMergedCharacterProperties(conditionType);
			return new MergedCharacterProperties(new CharacterFormattingInfo(), new CharacterFormattingOptions());
		}
		internal MergedParagraphProperties GetMergedParagraphProperties(ConditionalTableStyleFormattingTypes conditionType) {
			if (conditionalStyleProperties != null && this.conditionalStyleProperties.ContainsStyle(conditionType))
				return this.conditionalStyleProperties.Items[conditionType].GetMergedParagraphProperties();
			if (Parent != null)
				return Parent.GetMergedParagraphProperties(conditionType);
			return new MergedParagraphProperties(new ParagraphFormattingInfo(), new ParagraphFormattingOptions());
		}
		public virtual MergedTableRowProperties GetMergedTableRowProperties() {
			return GetMergedTableRowProperties(ConditionalRowType.Normal);
		}
		internal MergedTableRowProperties GetMergedTableRowProperties(ConditionalRowType rowType) {
			if (Deleted)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedStyleError);
			TableRowPropertiesMerger merger = new TableRowPropertiesMerger(TableRowProperties);
			GetConditionalPropertiesSource(rowType, ConditionalColumnType.Normal, style => { merger.Merge(style.TableRowProperties); return false; });
			if (Parent != null)
				merger.Merge(Parent.GetMergedTableRowProperties(rowType));
			return merger.MergedProperties;
		}
		internal MergedTableRowProperties GetMergedTableRowProperties(ConditionalTableStyleFormattingTypes conditionType) {
			if (conditionalStyleProperties != null && this.conditionalStyleProperties.ContainsStyle(conditionType))
				return this.conditionalStyleProperties.Items[conditionType].GetMergedTableRowProperties();
			if (Parent != null)
				return Parent.GetMergedTableRowProperties(conditionType);
			return new MergedTableRowProperties(new CombinedTableRowPropertiesInfo(), new TableRowPropertiesOptions());
		}
		public virtual MergedTableCellProperties GetMergedTableCellProperties() {
			return GetMergedTableCellProperties(ConditionalRowType.Normal, ConditionalColumnType.Normal);
		}
		internal MergedTableCellProperties GetMergedTableCellProperties(ConditionalRowType rowType, ConditionalColumnType colType) {
			if (Deleted)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedStyleError);
			TableCellPropertiesMerger merger = new TableCellPropertiesMerger(TableCellProperties);
			GetConditionalPropertiesSource(rowType, colType, style => { merger.Merge(style.TableCellProperties); return false; });
			if (Parent != null)
				merger.Merge(Parent.GetMergedTableCellProperties(rowType, colType));
			return merger.MergedProperties;
		}
		internal MergedTableCellProperties GetMergedTableCellProperties(ConditionalTableStyleFormattingTypes conditionType) {
			if (conditionalStyleProperties != null && this.conditionalStyleProperties.ContainsStyle(conditionType))
				return this.conditionalStyleProperties.Items[conditionType].GetMergedTableCellProperties();
			if (Parent != null)
				return Parent.GetMergedTableCellProperties(conditionType);
			return new MergedTableCellProperties(new CombinedCellPropertiesInfo(), new TableCellPropertiesOptions());
		}
	}
	#endregion
	#region TableStyleCollection
	public class TableStyleCollection : StyleCollectionBase<TableStyle> {
		public const int DefaultTableStyleIndex = 0;
		const int defaultLeftMargin = 108;
		const int defaultRightMargin = 108;
		const int defaultBorderWidth = 10;
		const BorderLineStyle defaultBorderLineStyle = BorderLineStyle.Single;
		public static readonly string TableSimpleStyleName = "Table Simple 1";
		public static readonly string DefaultTableStyleName = "Normal Table";
		public TableStyleCollection(DocumentModel documentModel, bool changeDefaultTableStyle)
			: base(documentModel) {
			Items.Add(CreateNormalTableStyle(changeDefaultTableStyle));
			Items.Add(CreateTableSimpleStyle());
		}
		protected internal virtual TableStyle CreateNormalTableStyle(bool changeDefaultTableStyle) {
			TableStyle style = new TableStyle(DocumentModel, null, DefaultTableStyleName);
			if (changeDefaultTableStyle) {
				style.TableProperties.BeginInit();
				try {
					SetDefaultMargin(style.TableProperties.CellMargins.Left, defaultLeftMargin);
					SetDefaultMargin(style.TableProperties.CellMargins.Top, 0);
					SetDefaultMargin(style.TableProperties.CellMargins.Right, defaultRightMargin);
					SetDefaultMargin(style.TableProperties.CellMargins.Bottom, 0);
					TablePropertiesOptions options = style.TableProperties.GetInfoForModification();
					options.UseLeftMargin = true;
					options.UseTopMargin = true;
					options.UseRightMargin = true;
					options.UseBottomMargin = true;
					style.TableProperties.ReplaceInfo(options, DocumentModelChangeActions.None);
				}
				finally {
					style.TableProperties.EndInit();
				}
			}
			return style;
		}
		protected internal virtual TableStyle CreateTableSimpleStyle() {
			TableStyle style = new TableStyle(DocumentModel, this[DefaultTableStyleIndex], TableSimpleStyleName);
			style.TableProperties.BeginInit();
			try {
				SetDefaultMargin(style.TableProperties.CellMargins.Left, defaultLeftMargin);
				SetDefaultMargin(style.TableProperties.CellMargins.Right, defaultRightMargin);
				SetDefaultBorder(style.TableProperties.Borders.LeftBorder, defaultBorderLineStyle, defaultBorderWidth);
				SetDefaultBorder(style.TableProperties.Borders.RightBorder, defaultBorderLineStyle, defaultBorderWidth);
				SetDefaultBorder(style.TableProperties.Borders.TopBorder, defaultBorderLineStyle, defaultBorderWidth);
				SetDefaultBorder(style.TableProperties.Borders.BottomBorder, defaultBorderLineStyle, defaultBorderWidth);
				SetDefaultBorder(style.TableProperties.Borders.InsideHorizontalBorder, defaultBorderLineStyle, defaultBorderWidth);
				SetDefaultBorder(style.TableProperties.Borders.InsideVerticalBorder, defaultBorderLineStyle, defaultBorderWidth);
				TablePropertiesOptions options = style.TableProperties.GetInfoForModification();
				options.UseLeftMargin = true;
				options.UseRightMargin = true;
				options.UseTopMargin = true;
				options.UseBottomMargin = true;
				options.UseLeftBorder = true;
				options.UseRightBorder = true;
				options.UseTopBorder = true;
				options.UseBottomBorder = true;
				options.UseInsideHorizontalBorder = true;
				options.UseInsideVerticalBorder = true;
				style.TableProperties.ReplaceInfo(options, DocumentModelChangeActions.None);
			}
			finally {
				style.TableProperties.EndInit();
			}
			return style;
		}
		protected internal override TableStyle CreateDefaultItem() {
			return null;
		}
		void SetDefaultBorder(BorderBase border, BorderLineStyle style, int widthInTwips) {
			border.BeginInit();
			try {
				border.Style = BorderLineStyle.Single;
				border.Width = DocumentModel.UnitConverter.TwipsToModelUnits(widthInTwips);
			}
			finally {
				border.EndInit();
			}
		}
		void SetDefaultMargin(WidthUnit margin, int valueInTwips) {
			margin.BeginInit();
			try {
				margin.Type = WidthUnitType.ModelUnits;
				margin.Value = DocumentModel.UnitConverter.TwipsToModelUnits(valueInTwips);
			}
			finally {
				margin.EndInit();
			}
		}
		protected override void NotifyPieceTableStyleDeleting(PieceTable pieceTable, TableStyle style) {
			TableCollection tables = pieceTable.Tables;
			int count = tables.Count;
			for (int i = 0; i < count; i++) {
				if (tables[i].TableStyle == style)
					tables[i].StyleIndex = this.DefaultItemIndex;
			}
		}
	}
	#endregion
	#region TableConditions
	internal class TableConditions {
		TableConditions() {
		}
		public const int Count = 12;
	}
	#endregion
	public class TableCellStyle : ParagraphPropertiesBasedStyle<TableCellStyle>, ICharacterPropertiesContainer, ICellPropertiesOwner, ITableCellStyle {
		#region Fields
		TableCellProperties tableCellProperties;
		CharacterProperties characterProperties;
		#endregion
		public TableCellStyle(DocumentModel documentModel)
			: this(documentModel, null) {
		}
		public TableCellStyle(DocumentModel documentModel, TableCellStyle parent)
			: this(documentModel, parent, String.Empty) {
		}
		public TableCellStyle(DocumentModel documentModel, TableCellStyle parent, string styleName)
			: base(documentModel, parent, styleName) {
			this.characterProperties = new CharacterProperties(this);
			SubscribeCharacterPropertiesEvents();
		}
		#region Properties
		public override StyleType Type { get { return StyleType.TableCellStyle; } }
		#region ITableStyle properties
		public TableCellProperties TableCellProperties {
			get {
				if (tableCellProperties == null) {
					tableCellProperties = new TableCellProperties(DocumentModel.MainPieceTable, this);
					SubscribeTableCellPropertiesEvents();
				}
				return tableCellProperties;
			}
		}
		public CharacterProperties CharacterProperties { get { return characterProperties; } }
		#endregion
		#endregion
		protected internal virtual void SubscribeCharacterPropertiesEvents() {
			CharacterProperties.ObtainAffectedRange += OnObtainAffectedRange;
		}
		protected internal virtual void SubscribeTableCellPropertiesEvents() {
			tableCellProperties.ObtainAffectedRange += OnObtainAffectedRange;
		}
		protected internal virtual void OnObtainAffectedRange(object sender, ObtainAffectedRangeEventArgs e) {
			e.Start = RunIndex.Zero;
			e.End = RunIndex.MaxValue;
		}
		public virtual MergedCharacterProperties GetMergedCharacterProperties(ConditionalRowType rowType, ConditionalColumnType columnType) {
			if (Deleted)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedStyleError);
			CharacterPropertiesMerger merger = new CharacterPropertiesMerger(new MergedCharacterProperties(new CharacterFormattingInfo(), new CharacterFormattingOptions()));
			merger.Merge(CharacterProperties);
			if (Parent != null)
				merger.Merge(Parent.GetMergedCharacterProperties(rowType, columnType));
			return merger.MergedProperties;
		}
		public virtual MergedCharacterProperties GetMergedWithDefaultCharacterProperties(ConditionalRowType rowType, ConditionalColumnType columnType) {
			if (Deleted)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedStyleError);
			CharacterPropertiesMerger merger = new CharacterPropertiesMerger(new MergedCharacterProperties(new CharacterFormattingInfo(), new CharacterFormattingOptions()));
			merger.Merge(CharacterProperties);
			merger.Merge(DocumentModel.DefaultCharacterProperties);
			return merger.MergedProperties;
		}
		public virtual MergedTableCellProperties GetMergedWithDefaultTableCellProperties() {
			if (Deleted)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedStyleError);
			TableCellPropertiesMerger merger = new TableCellPropertiesMerger(GetMergedTableCellProperties());
			merger.Merge(DocumentModel.DefaultTableCellProperties);
			return merger.MergedProperties;
		}
		public virtual TableCellProperties GetTableCellProperties(TableCellPropertiesOptions.Mask mask) {
			if (Deleted)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedStyleError);
			if (tableCellProperties != null && tableCellProperties.GetUse(mask))
				return tableCellProperties;
			return Parent != null ? Parent.GetTableCellProperties(mask) : null;
		}
		public virtual CharacterProperties GetCharacterProperties(CharacterFormattingOptions.Mask mask, ConditionalRowType rowType, ConditionalColumnType columnType) {
			if (Deleted)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedStyleError);
			if (characterProperties != null && characterProperties.UseVal(mask))
				return characterProperties;
			return Parent != null ? Parent.GetCharacterProperties(mask, rowType, columnType) : null;
		}
		public virtual ParagraphProperties GetParagraphProperties(ParagraphFormattingOptions.Mask mask, ConditionalRowType rowType, ConditionalColumnType columnType) {
			if (Deleted)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedStyleError);
			if (ParagraphProperties != null && ParagraphProperties.UseVal(mask))
				return ParagraphProperties;
			return Parent != null ? Parent.GetParagraphProperties(mask, rowType, columnType) : null;
		}
		public virtual MergedParagraphProperties GetMergedParagraphProperties(ConditionalRowType rowType, ConditionalColumnType columnType) {
			if (Deleted)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedStyleError);
			ParagraphPropertiesMerger merger = new ParagraphPropertiesMerger(new MergedParagraphProperties(new ParagraphFormattingInfo(), new ParagraphFormattingOptions()));
			if (ParagraphProperties != null)
				merger.Merge(ParagraphProperties);
			if (Parent != null)
				merger.Merge(Parent.GetMergedParagraphProperties(rowType, columnType));
			return merger.MergedProperties;
		}
		public override MergedParagraphProperties GetMergedParagraphProperties() {
			return GetMergedParagraphProperties(ConditionalRowType.Normal, ConditionalColumnType.Normal);
		}
		protected override void MergePropertiesWithParent() {
			base.MergePropertiesWithParent();
			TableCellProperties.Merge(Parent.TableCellProperties);
			CharacterProperties.Merge(Parent.CharacterProperties);
		}
		protected internal override void CopyProperties(TableCellStyle source) {
			base.CopyProperties(source);
			TableCellProperties.CopyFrom(source.TableCellProperties);
			CharacterProperties.CopyFrom(source.CharacterProperties.Info);
		}
		public override int Copy(DocumentModel targetModel) {
			TableCellStyleCollection tableCellStyles = targetModel.TableCellStyles;
			for (int i = 0; i < tableCellStyles.Count; i++) {
				if (this.StyleName == tableCellStyles[i].StyleName)
					return i;
			}
			return targetModel.TableCellStyles.AddNewStyle(CopyTo(targetModel));
		}
		protected internal virtual TableCellStyle CopyTo(DocumentModel targetModel) {
			TableCellStyle style = new TableCellStyle(targetModel);
			style.CopyProperties(this);
			style.StyleName = this.StyleName;
			if (Parent != null)
				style.Parent = targetModel.TableCellStyles[Parent.Copy(targetModel)];
			ApplyPropertiesDiff(style);
			return style;
		}
		protected internal void ApplyPropertiesDiff(TableCellStyle style) {
			ParagraphProperties.ApplyPropertiesDiff(style.ParagraphProperties, style.GetMergedWithDefaultParagraphProperties().Info, this.GetMergedWithDefaultParagraphProperties().Info);
			CharacterProperties.ApplyPropertiesDiff(style.CharacterProperties, style.GetMergedWithDefaultCharacterProperties(ConditionalRowType.Normal, ConditionalColumnType.Normal).Info, this.GetMergedWithDefaultCharacterProperties(ConditionalRowType.Normal, ConditionalColumnType.Normal).Info);
		}
		#region ICharacterPropertiesContainer Members
		public IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateCharacterPropertiesChangedHistoryItem() {
			return new IndexChangedHistoryItem<DocumentModelChangeActions>(DocumentModel.MainPieceTable, CharacterProperties);
		}
		public void OnCharacterPropertiesChanged() {
			DocumentModel.ResetDocumentFormattingCaches(ResetFormattingCacheType.Character);
		}
		#endregion
		#region ICellPropertiesOwner Members
		IndexChangedHistoryItemCore<DocumentModelChangeActions> ICellPropertiesOwner.CreateCellPropertiesChangedHistoryItem(TableCellProperties properties) {
			Debug.Assert(properties == TableCellProperties);
			return new IndexChangedHistoryItem<DocumentModelChangeActions>(properties.PieceTable, properties);
		}
		#endregion
		public override void ResetCachedIndices(ResetFormattingCacheType resetFormattingCacheType) {
		}
		internal MergedTableRowProperties GetMergedTableRowProperties(ConditionalTableStyleFormattingTypes conditionType) {
			if (Parent != null)
				return Parent.GetMergedTableRowProperties(conditionType);
			return new MergedTableRowProperties(new CombinedTableRowPropertiesInfo(), new TableRowPropertiesOptions());
		}
		public virtual MergedTableCellProperties GetMergedTableCellProperties() {
			return GetMergedTableCellProperties(ConditionalRowType.Normal, ConditionalColumnType.Normal);
		}
		internal MergedTableCellProperties GetMergedTableCellProperties(ConditionalRowType rowType, ConditionalColumnType colType) {
			if (Deleted)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedStyleError);
			TableCellPropertiesMerger merger = new TableCellPropertiesMerger(TableCellProperties);
			if (Parent != null)
				merger.Merge(Parent.GetMergedTableCellProperties(rowType, colType));
			return merger.MergedProperties;
		}
		internal MergedTableCellProperties GetMergedTableCellProperties(ConditionalTableStyleFormattingTypes conditionType) {
			if (Parent != null)
				return Parent.GetMergedTableCellProperties(conditionType);
			return new MergedTableCellProperties(new CombinedCellPropertiesInfo(), new TableCellPropertiesOptions());
		}
	}
	public class TableCellStyleCollection : StyleCollectionBase<TableCellStyle> {
		public const int DefaultTableCellStyleIndex = 0;
		const int defaultLeftMargin = 108;
		const int defaultRightMargin = 108;
		const int defaultBorderWidth = 10;
		const BorderLineStyle defaultBorderLineStyle = BorderLineStyle.Single;
		public static readonly string TableCellSimpleStyleName = "Table Cell Simple 1";
		public static readonly string DefaultTableCellStyleName = "Normal Table Cell";
		public TableCellStyleCollection(DocumentModel documentModel, bool changeDefaultTableCellStyle)
			: base(documentModel) {
			Items.Add(CreateNormalTableCellStyle(changeDefaultTableCellStyle));
			Items.Add(CreateTableSimpleStyle());
		}
		protected internal virtual TableCellStyle CreateNormalTableCellStyle(bool changeDefaultTableCellStyle) {
			TableCellStyle style = new TableCellStyle(DocumentModel, null, DefaultTableCellStyleName);
			if (changeDefaultTableCellStyle) {
				style.TableCellProperties.BeginInit();
				try {
				}
				finally {
					style.TableCellProperties.EndInit();
				}
			}
			return style;
		}
		protected internal virtual TableCellStyle CreateTableSimpleStyle() {
			TableCellStyle style = new TableCellStyle(DocumentModel, this[DefaultTableCellStyleIndex], TableCellSimpleStyleName);
			style.TableCellProperties.BeginInit();
			try {
				SetDefaultMargin(style.TableCellProperties.CellMargins.Left, defaultLeftMargin);
				SetDefaultMargin(style.TableCellProperties.CellMargins.Right, defaultRightMargin);
				SetDefaultBorder(style.TableCellProperties.Borders.LeftBorder, defaultBorderLineStyle, defaultBorderWidth);
				SetDefaultBorder(style.TableCellProperties.Borders.RightBorder, defaultBorderLineStyle, defaultBorderWidth);
				SetDefaultBorder(style.TableCellProperties.Borders.TopBorder, defaultBorderLineStyle, defaultBorderWidth);
				SetDefaultBorder(style.TableCellProperties.Borders.BottomBorder, defaultBorderLineStyle, defaultBorderWidth);
				SetDefaultBorder(style.TableCellProperties.Borders.InsideHorizontalBorder, defaultBorderLineStyle, defaultBorderWidth);
				SetDefaultBorder(style.TableCellProperties.Borders.InsideVerticalBorder, defaultBorderLineStyle, defaultBorderWidth);
				TableCellPropertiesOptions options = style.TableCellProperties.GetInfoForModification();
				options.UseLeftMargin = true;
				options.UseRightMargin = true;
				options.UseTopMargin = true;
				options.UseBottomMargin = true;
				options.UseLeftBorder = true;
				options.UseRightBorder = true;
				options.UseTopBorder = true;
				options.UseBottomBorder = true;
				options.UseInsideHorizontalBorder = true;
				options.UseInsideVerticalBorder = true;
				style.TableCellProperties.ReplaceInfo(options, DocumentModelChangeActions.None);
			}
			finally {
				style.TableCellProperties.EndInit();
			}
			return style;
		}
		protected internal override TableCellStyle CreateDefaultItem() {
			return null;
		}
		void SetDefaultBorder(BorderBase border, BorderLineStyle style, int widthInTwips) {
			border.BeginInit();
			try {
				border.Style = BorderLineStyle.Single;
				border.Width = DocumentModel.UnitConverter.TwipsToModelUnits(widthInTwips);
			}
			finally {
				border.EndInit();
			}
		}
		void SetDefaultMargin(WidthUnit margin, int valueInTwips) {
			margin.BeginInit();
			try {
				margin.Type = WidthUnitType.ModelUnits;
				margin.Value = DocumentModel.UnitConverter.TwipsToModelUnits(valueInTwips);
			}
			finally {
				margin.EndInit();
			}
		}
		protected override void NotifyPieceTableStyleDeleting(PieceTable pieceTable, TableCellStyle style) {
			TableCollection tables = pieceTable.Tables;
			int count = tables.Count;
			TableCellProcessorDelegate setDefaultStyle = delegate(TableCell cell) {
				if (cell.TableCellStyle == style)
					cell.StyleIndex = this.DefaultItemIndex;
			};
			for (int i = 0; i < count; i++)
				tables[i].ForEachCell(setDefaultStyle);
		}
	}
}
