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
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Utils;
namespace DevExpress.XtraRichEdit.Model {
	#region TableRowAlignment
	public enum TableRowAlignment {
		Both,
		Center,
		Distribute,
		Left,
		NumTab, 
		Right
	}
	#endregion
	#region TableLayoutType
	public enum TableLayoutType {
		Fixed,
		Autofit
	}
	#endregion
	#region TableAutoFitBehaviorType
	public enum TableAutoFitBehaviorType {
		FixedColumnWidth,
		AutoFitToContents,
		AutoFitToWindow
	}
	#endregion
	#region TableLookTypes
	[Flags]
	public enum TableLookTypes {
		None = 0x0000,
		ApplyFirstRow = 0x0020,
		ApplyLastRow = 0x0040,
		ApplyFirstColumn = 0x0080,
		ApplyLastColumn = 0x0100,
		DoNotApplyRowBanding = 0x0200,
		DoNotApplyColumnBanding = 0x0400
	}
	#endregion
	#region HeightUnitType
	public enum HeightUnitType {
		Minimum,
		Auto,
		Exact
	}
	#endregion
	#region HeightUnitInfo
	public class HeightUnitInfo : ICloneable<HeightUnitInfo>, ISupportsCopyFrom<HeightUnitInfo>, ISupportsSizeOf {
		#region Fields
		HeightUnitType type;
		int val;
		#endregion
		public HeightUnitInfo() {
		}
		public HeightUnitInfo(int value, HeightUnitType type) {
			this.val = value;
			this.type = type;
		}
		#region Properties
		public HeightUnitType Type { get { return type; } set { type = value; } }
		public int Value { get { return val; } set { val = value; } }
		#endregion
		public void CopyFrom(HeightUnitInfo info) {
			Type = info.Type;
			Value = info.Value;
		}
		public HeightUnitInfo Clone() {
			HeightUnitInfo info = new HeightUnitInfo();
			info.CopyFrom(this);
			return info;
		}
		public override bool Equals(object obj) {
			HeightUnitInfo info = obj as HeightUnitInfo;
			if (info == null)
				return false;
			return Type == info.Type && Value == info.Value;
		}
		public override int GetHashCode() {
			return ((int)Type << 3) | Value;
		}
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
		#endregion
	}
	#endregion
	#region HeightUnitInfoCache
	public class HeightUnitInfoCache : UniqueItemsCache<HeightUnitInfo> {
		public HeightUnitInfoCache(DocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override HeightUnitInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			HeightUnitInfo result = new HeightUnitInfo();
			result.Type = HeightUnitType.Auto;
			result.Value = 0;
			return result;
		}
	}
	#endregion
	public class HeightUnit : RichEditIndexBasedObject<HeightUnitInfo> {
		readonly IPropertiesContainer owner;
		public HeightUnit(PieceTable pieceTable, IPropertiesContainer owner)
			: base(pieceTable) {
			Guard.ArgumentNotNull(owner, "owner");
			this.owner = owner;
		}
		#region Properties
		#region Type
		public HeightUnitType Type {
			get { return Info.Type; }
			set {
				if (value == Info.Type) {
					NotifyFakeAssign();
					return;
				}
				SetPropertyValue(SetTypeCore, value);
			}
		}
		DocumentModelChangeActions SetTypeCore(HeightUnitInfo unit, HeightUnitType value) {
			unit.Type = value;
			return GetBatchUpdateChangeActions();
		}
		#endregion
		#region Value
		public int Value {
			get { return Info.Value; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("Value", value);
				if (value == Info.Value) {
					NotifyFakeAssign();
					return;
				}
				SetPropertyValue(SetValueCore, value);
			}
		}
		DocumentModelChangeActions SetValueCore(HeightUnitInfo unit, int value) {
			unit.Value = value;
			return GetBatchUpdateChangeActions();
		}
		#endregion
		internal IPropertiesContainer Owner { get { return owner; } }
		#endregion
		protected internal override UniqueItemsCache<HeightUnitInfo> GetCache(DocumentModel documentModel) {
			return documentModel.Cache.HeightUnitInfoCache;
		}
		protected override void OnBeginAssign() {
			base.OnBeginAssign();
		}
		protected override void OnEndAssign() {
			Owner.EndChanging();
			base.OnEndAssign();
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected internal override void RaiseObtainAffectedRange(ObtainAffectedRangeEventArgs args) {
			if (Owner != null)
				Owner.RaiseObtainAffectedRange(args);
		}
		protected override void ApplyChanges(DocumentModelChangeActions changeActions) {
			if (Owner != null)
				Owner.ApplyChanges(changeActions);
			else
				base.ApplyChanges(changeActions);
		}
	}
	public class RowHeight : HeightUnit {
		public RowHeight(PieceTable pieceTable, IPropertiesContainer owner)
			: base(pieceTable, owner) {
		}
		protected override void OnBeginAssign() {
			base.OnBeginAssign();
			Owner.BeginChanging(Properties.RowHeight);
		}
		protected override void OnEndAssign() {
			base.OnEndAssign();
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetRuler | DocumentModelChangeActions.RaiseContentChanged | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetRuler;
		}
	}
	#region WidthBefore
	public class WidthBefore : WidthUnit {
		public WidthBefore(PieceTable pieceTable, IPropertiesContainer owner)
			: base(pieceTable, owner) {
		}
		protected override void OnBeginAssign() {
			base.OnBeginAssign();
			Owner.BeginChanging(Properties.WidthBefore);
		}
		protected override void OnEndAssign() {
			base.OnEndAssign();
		}
		protected override DocumentModelChangeActions SetTypeCore(WidthUnitInfo unit, WidthUnitType value) {
			return base.SetTypeCore(unit, value) | DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetRuler | DocumentModelChangeActions.RaiseContentChanged | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler;
		}
		protected override DocumentModelChangeActions SetValueCore(WidthUnitInfo unit, int value) {
			return base.SetValueCore(unit, value) | DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetRuler | DocumentModelChangeActions.RaiseContentChanged | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler;
		}
	}
	#endregion
	#region WidthAfter
	public class WidthAfter : WidthUnit {
		public WidthAfter(PieceTable pieceTable, IPropertiesContainer owner)
			: base(pieceTable, owner) {
		}
		protected override void OnBeginAssign() {
			base.OnBeginAssign();
			Owner.BeginChanging(Properties.WidthAfter);
		}
		protected override void OnEndAssign() {
			base.OnEndAssign();
		}
		protected override DocumentModelChangeActions SetTypeCore(WidthUnitInfo unit, WidthUnitType value) {
			return base.SetTypeCore(unit, value) | DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetRuler | DocumentModelChangeActions.RaiseContentChanged | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler;
		}
		protected override DocumentModelChangeActions SetValueCore(WidthUnitInfo unit, int value) {
			return base.SetValueCore(unit, value) | DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetRuler | DocumentModelChangeActions.RaiseContentChanged | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler;
		}
	}
	#endregion
	#region TableRowGeneralSettingsInfo
	public class TableRowGeneralSettingsInfo : ICloneable<TableRowGeneralSettingsInfo>, ISupportsCopyFrom<TableRowGeneralSettingsInfo>, ISupportsSizeOf {
		#region Fields
		bool cantSplit;
		bool hideCellMark;
		bool header;
		int gridBefore;
		int gridAfter;
		TableRowAlignment tableRowAlignment;
		#endregion
		#region Properties
		public bool CantSplit { get { return cantSplit; } set { cantSplit = value; } }
		public bool HideCellMark { get { return hideCellMark; } set { hideCellMark = value; } }
		public bool Header { get { return header; } set { header = value; } }
		public int GridBefore { get { return gridBefore; } set { gridBefore = value; } }
		public int GridAfter { get { return gridAfter; } set { gridAfter = value; } }
		public TableRowAlignment TableRowAlignment { get { return tableRowAlignment; } set { tableRowAlignment = value; } }
		#endregion
		public void CopyFrom(TableRowGeneralSettingsInfo info) {
			CantSplit = info.CantSplit;
			HideCellMark = info.HideCellMark;
			Header = info.Header;
			GridBefore = info.GridBefore;
			GridAfter = info.GridAfter;
			TableRowAlignment = info.TableRowAlignment;
		}
		public TableRowGeneralSettingsInfo Clone() {
			TableRowGeneralSettingsInfo info = new TableRowGeneralSettingsInfo();
			info.CopyFrom(this);
			return info;
		}
		public override bool Equals(object obj) {
			TableRowGeneralSettingsInfo info = obj as TableRowGeneralSettingsInfo;
			if (info == null)
				return false;
			return CantSplit == info.CantSplit &&
				   HideCellMark == info.HideCellMark &&
				   Header == info.Header &&
				   GridBefore == info.GridBefore &&
				   GridAfter == info.GridAfter &&
				   TableRowAlignment == info.TableRowAlignment; 
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
		#endregion
	}
	#endregion
	#region TableRowGeneralSettingsInfoCache
	public class TableRowGeneralSettingsInfoCache : UniqueItemsCache<TableRowGeneralSettingsInfo> {
		public TableRowGeneralSettingsInfoCache(DocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override TableRowGeneralSettingsInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			TableRowGeneralSettingsInfo info = new TableRowGeneralSettingsInfo();
			info.TableRowAlignment = TableRowAlignment.Left;
			return info;
		}
	}
	#endregion
	#region TableRowFormattingInfo
	public class TableRowGeneralSettings : RichEditIndexBasedObject<TableRowGeneralSettingsInfo> {
		readonly IPropertiesContainer owner;
		public TableRowGeneralSettings(PieceTable pieceTable, IPropertiesContainer owner)
			: base(pieceTable) {
			Guard.ArgumentNotNull(owner, "owner");
			this.owner = owner;
		}
		#region Properties
		#region Header
		public bool Header {
			get { return Info.Header; }
			set {
				BeginChanging(Properties.Header);
				if (value == Info.Header) {
					EndChanging();
					return;
				}
				SetPropertyValue(SetHeader, value);
				EndChanging();
			}
		}
		DocumentModelChangeActions SetHeader(TableRowGeneralSettingsInfo info, bool value) {
			info.Header = value;
			return TableRowChangeActionsCalculator.CalculateChangeActions(TableRowChangeType.Header);
		}
		#endregion
		#region HideCellMark
		public bool HideCellMark {
			get { return Info.HideCellMark; }
			set {
				BeginChanging(Properties.HideCellMark);
				if (value == Info.HideCellMark) {
					EndChanging();
					return;
				}
				SetPropertyValue(SetHideCellMark, value);
				EndChanging();
			}
		}
		DocumentModelChangeActions SetHideCellMark(TableRowGeneralSettingsInfo info, bool value) {
			info.HideCellMark = value;
			return TableRowChangeActionsCalculator.CalculateChangeActions(TableRowChangeType.HideCellMark);
		}
		#endregion
		#region CantSplit
		public bool CantSplit {
			get { return Info.CantSplit; }
			set {
				BeginChanging(Properties.CantSplit);
				if (value == Info.CantSplit) {
					EndChanging();
					return;
				}
				SetPropertyValue(SetCantSplit, value);
				EndChanging();
			}
		}
		DocumentModelChangeActions SetCantSplit(TableRowGeneralSettingsInfo info, bool value) {
			info.CantSplit = value;
			return TableRowChangeActionsCalculator.CalculateChangeActions(TableRowChangeType.CantSplit);
		}
		#endregion
		#region TableRowAlignment
		public TableRowAlignment TableRowAlignment {
			get { return Info.TableRowAlignment; }
			set {
				BeginChanging(Properties.TableRowAlignment);
				if (value == Info.TableRowAlignment) {
					EndChanging();
					return;
				}
				SetPropertyValue(SetTableRowAlignment, value);
				EndChanging();
			}
		}
		DocumentModelChangeActions SetTableRowAlignment(TableRowGeneralSettingsInfo info, TableRowAlignment value) {
			info.TableRowAlignment = value;
			return TableRowChangeActionsCalculator.CalculateChangeActions(TableRowChangeType.TableRowAlignment);
		}
		#endregion
		#region GridAfter
		public int GridAfter {
			get { return Info.GridAfter; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("GridBefore", value);
				BeginChanging(Properties.GridAfter);
				if (value == Info.GridAfter) {
					EndChanging();
					return;
				}
				SetPropertyValue(SetGridAfter, value);
				EndChanging();
			}
		}
		DocumentModelChangeActions SetGridAfter(TableRowGeneralSettingsInfo info, int value) {
			info.GridAfter = value;
			return TableRowChangeActionsCalculator.CalculateChangeActions(TableRowChangeType.GridAfter);
		}
		#endregion
		#region GridBefore
		public int GridBefore {
			get { return Info.GridBefore; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("GridBefore", value);
				BeginChanging(Properties.GridBefore);
				if (value == Info.GridBefore) {
					EndChanging();
					return;
				}
				SetPropertyValue(SetGridBefore, value);
				EndChanging();
			}
		}
		DocumentModelChangeActions SetGridBefore(TableRowGeneralSettingsInfo info, int value) {
			info.GridBefore = value;
			return TableRowChangeActionsCalculator.CalculateChangeActions(TableRowChangeType.GridBefore);
		}
		#endregion
		internal IPropertiesContainer Owner { get { return owner; } }
		#endregion
		protected internal override UniqueItemsCache<TableRowGeneralSettingsInfo> GetCache(DocumentModel documentModel) {
			return documentModel.Cache.TableRowGeneralSettingsInfoCache;
		}
		public void CopyFrom(TableRowGeneralSettings settings) {
			Owner.BeginPropertiesUpdate();
			try {
				BeginUpdate();
				try {
					CantSplit = settings.CantSplit;
					HideCellMark = settings.HideCellMark;
					Header = settings.Header;
					GridBefore = settings.GridBefore;
					GridAfter = settings.GridAfter;
					TableRowAlignment = settings.TableRowAlignment;
				}
				finally {
					EndUpdate();
				}
			}
			finally {
				Owner.EndPropertiesUpdate();
			}
		}
		protected virtual void BeginChanging(Properties changedProperty) {
			Owner.BeginChanging(changedProperty);
		}
		protected virtual void EndChanging() {
			Owner.EndChanging();
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return TableRowChangeActionsCalculator.CalculateChangeActions(TableRowChangeType.BatchUpdate);
		}
		protected internal override void RaiseObtainAffectedRange(ObtainAffectedRangeEventArgs args) {
			Owner.RaiseObtainAffectedRange(args);
		}
		protected override void ApplyChanges(DocumentModelChangeActions changeActions) {
			Owner.ApplyChanges(changeActions);
		}
	}
	#endregion
	#region TableRowChangeType
	public enum TableRowChangeType {
		None = 0,
		Header,
		HideCellMark,
		CantSplit,
		TableRowAlignment,
		TableRowConditionalFormatting,
		GridAfter,
		GridBefore,
		BatchUpdate
	}
	#endregion
	#region TableRowChangeActionsCalculator
	public static class TableRowChangeActionsCalculator {
		internal class TableRowChangeActionsTable : Dictionary<TableRowChangeType, DocumentModelChangeActions> {
		}
		internal static TableRowChangeActionsTable tableRowChangeActionsTable = CreateTableRowChangeActionsTable();
		internal static TableRowChangeActionsTable CreateTableRowChangeActionsTable() {
			TableRowChangeActionsTable table = new TableRowChangeActionsTable();
			table.Add(TableRowChangeType.None, DocumentModelChangeActions.None);
			table.Add(TableRowChangeType.Header, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout);
			table.Add(TableRowChangeType.HideCellMark, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout);
			table.Add(TableRowChangeType.CantSplit, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout);
			table.Add(TableRowChangeType.TableRowAlignment, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(TableRowChangeType.TableRowConditionalFormatting, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout);
			table.Add(TableRowChangeType.GridAfter, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(TableRowChangeType.GridBefore, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(TableRowChangeType.BatchUpdate, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			return table;
		}
		public static DocumentModelChangeActions CalculateChangeActions(TableRowChangeType change) {
			return tableRowChangeActionsTable[change];
		}
	}
	#endregion
	#region TableRowPropertiesOptions
	public class TableRowPropertiesOptions : ICloneable<TableRowPropertiesOptions>, ISupportsCopyFrom<TableRowPropertiesOptions>, ISupportsSizeOf {
		#region Mask
		public enum Mask {
			UseNone = 0x00000000,
			UseHeight = 0x00000001,
			UseCantSplit = 0x00000002,
			UseHideCellMark = 0x00000004,
			UseGridBefore = 0x00000008,
			UseGridAfter = 0x00000010,
			UseWidthBefore = 0x00000020,
			UseWidthAfter = 0x00000040,
			UseCellSpacing = 0x00000080,
			UseTableRowAlignment = 0x00000100,
			UseHeader = 0x00000400,
			UseAll = 0x7FFFFFFF
		}
		#endregion
		Mask val;
		public TableRowPropertiesOptions() {
			this.val = Mask.UseNone;
		}
		internal TableRowPropertiesOptions(Mask val) {
			this.val = val;
		}
		#region Properties
		public bool UseHeight { get { return GetVal(Mask.UseHeight); } set { SetVal(Mask.UseHeight, value); } }
		public bool UseCantSplit { get { return GetVal(Mask.UseCantSplit); } set { SetVal(Mask.UseCantSplit, value); } }
		public bool UseHideCellMark { get { return GetVal(Mask.UseHideCellMark); } set { SetVal(Mask.UseHideCellMark, value); } }
		public bool UseHeader { get { return GetVal(Mask.UseHeader); } set { SetVal(Mask.UseHeader, value); } }
		public bool UseGridBefore { get { return GetVal(Mask.UseGridBefore); } set { SetVal(Mask.UseGridBefore, value); } }
		public bool UseGridAfter { get { return GetVal(Mask.UseGridAfter); } set { SetVal(Mask.UseGridAfter, value); } }
		public bool UseWidthBefore { get { return GetVal(Mask.UseWidthBefore); } set { SetVal(Mask.UseWidthBefore, value); } }
		public bool UseWidthAfter { get { return GetVal(Mask.UseWidthAfter); } set { SetVal(Mask.UseWidthAfter, value); } }
		public bool UseCellSpacing { get { return GetVal(Mask.UseCellSpacing); } set { SetVal(Mask.UseCellSpacing, value); } }
		public bool UseTableRowAlignment { get { return GetVal(Mask.UseTableRowAlignment); } set { SetVal(Mask.UseTableRowAlignment, value); } }
		internal Mask Value { get { return val; } set { val = value; } }
		#endregion
		public TableRowPropertiesOptions Clone() {
			TableRowPropertiesOptions clone = new TableRowPropertiesOptions();
			clone.CopyFrom(this);
			return clone;
		}
		public void CopyFrom(TableRowPropertiesOptions options) {
			this.val = options.val;
		}
		public override bool Equals(object obj) {
			TableRowPropertiesOptions opts = obj as TableRowPropertiesOptions;
			if (opts == null)
				return false;
			return Value == opts.Value;
		}
		public override int GetHashCode() {
			return (int)Value;
		}
		#region GetVal/SetVal helpers
		public bool GetVal(Mask mask) {
			return (val & mask) != 0;
		}
		void SetVal(Mask mask, bool bitVal) {
			if (bitVal)
				val |= mask;
			else
				val &= ~mask;
		}
		#endregion
		#region Options accessors
		internal static bool GetUseCantSplit(TableRowPropertiesOptions options) { return options.UseCantSplit; }
		internal static void SetUseCantSplit(TableRowPropertiesOptions options, bool value) { options.UseCantSplit = value; }
		internal static bool GetUseCellSpacing(TableRowPropertiesOptions options) { return options.UseCellSpacing; }
		internal static void SetUseCellSpacing(TableRowPropertiesOptions options, bool value) { options.UseCellSpacing = value; }
		internal static bool GetUseGridAfter(TableRowPropertiesOptions options) { return options.UseGridAfter; }
		internal static void SetUseGridAfter(TableRowPropertiesOptions options, bool value) { options.UseGridAfter = value; }
		internal static bool GetUseGridBefore(TableRowPropertiesOptions options) { return options.UseGridBefore; }
		internal static void SetUseGridBefore(TableRowPropertiesOptions options, bool value) { options.UseGridBefore = value; }
		internal static bool GetUseHeader(TableRowPropertiesOptions options) { return options.UseHeader; }
		internal static void SetUseHeader(TableRowPropertiesOptions options, bool value) { options.UseHeader = value; }
		internal static bool GetUseHeight(TableRowPropertiesOptions options) { return options.UseHeight; }
		internal static void SetUseHeight(TableRowPropertiesOptions options, bool value) { options.UseHeight = value; }
		internal static bool GetUseHideCellMark(TableRowPropertiesOptions options) { return options.UseHideCellMark; }
		internal static void SetUseHideCellMark(TableRowPropertiesOptions options, bool value) { options.UseHideCellMark = value; }
		internal static bool GetUseTableRowAlignment(TableRowPropertiesOptions options) { return options.UseTableRowAlignment; }
		internal static void SetUseTableRowAlignment(TableRowPropertiesOptions options, bool value) { options.UseTableRowAlignment = value; }
		internal static bool GetUseWidthAfter(TableRowPropertiesOptions options) { return options.UseWidthAfter; }
		internal static void SetUseWidthAfter(TableRowPropertiesOptions options, bool value) { options.UseWidthAfter = value; }
		internal static bool GetUseWidthBefore(TableRowPropertiesOptions options) { return options.UseWidthBefore; }
		internal static void SetUseWidthBefore(TableRowPropertiesOptions options, bool value) { options.UseWidthBefore = value; }
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
		#endregion
	}
	#endregion
	#region TableRowFormattingOptionsCache
	public class TableRowPropertiesOptionsCache : UniqueItemsCache<TableRowPropertiesOptions> {
		internal const int EmptyRowPropertiesOptionsItem = 0;
		internal const int RootRowPropertiesOptionsItem = 1;
		public TableRowPropertiesOptionsCache(DocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override void InitItems(IDocumentModelUnitConverter unitConverter) {
			base.InitItems(unitConverter);
			AddRootStyleOptions();
		}
		protected override TableRowPropertiesOptions CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new TableRowPropertiesOptions();
		}
		void AddRootStyleOptions() {
			AppendItem(new TableRowPropertiesOptions(TableRowPropertiesOptions.Mask.UseAll));
		}
	}
	#endregion
	#region TableRowProperties
	public class TableRowProperties : PropertiesBase<TableRowPropertiesOptions> {
		public class TableRowPropertiesAccessorTable : PropertiesDictionary<BoolPropertyAccessor<TableRowPropertiesOptions>> {
		}
		#region Fields
		static TableRowPropertiesAccessorTable accessorTable = CreateAccessorTable();
		HeightUnit height;
		WidthUnit widthBefore;
		WidthUnit widthAfter;
		WidthUnit cellSpacing;
		TableRowGeneralSettings generalSettings;
		#endregion
		public TableRowProperties(PieceTable pieceTable)
			: base(pieceTable) {
			this.height = new RowHeight(pieceTable, this);
			this.widthBefore = new WidthBefore(pieceTable, this);
			this.widthAfter = new WidthAfter(pieceTable, this);
			this.cellSpacing = new CellSpacing(pieceTable, this);
			this.generalSettings = new TableRowGeneralSettings(pieceTable, this);
		}
		#region Properties
		public HeightUnit Height { get { return height; } }
		public bool UseHeight { get { return Info.UseHeight; } }
		public WidthUnit WidthBefore { get { return widthBefore; } }
		public bool UseWidthBefore { get { return Info.UseWidthBefore; } }
		public WidthUnit WidthAfter { get { return widthAfter; } }
		public bool UseWidthAfter { get { return Info.UseWidthAfter; } }
		public WidthUnit CellSpacing { get { return cellSpacing; } }
		public bool UseCellSpacing { get { return Info.UseCellSpacing; } }
		public bool Header { get { return GeneralSettings.Header; } set { GeneralSettings.Header = value; } }
		public bool UseHeader { get { return Info.UseHeader; } }
		public bool HideCellMark { get { return GeneralSettings.HideCellMark; } set { GeneralSettings.HideCellMark = value; } }
		public bool UseHideCellMark { get { return Info.UseHideCellMark; } }
		public bool CantSplit { get { return GeneralSettings.CantSplit; } set { GeneralSettings.CantSplit = value; } }
		public bool UseCantSplit { get { return Info.UseCantSplit; } }
		public TableRowAlignment TableRowAlignment { get { return GeneralSettings.TableRowAlignment; } set { GeneralSettings.TableRowAlignment = value; } }
		public bool UseTableRowAlignment { get { return Info.UseTableRowAlignment; } }
		public int GridAfter { get { return GeneralSettings.GridAfter; } set { GeneralSettings.GridAfter = value; } }
		public bool UseGridAfter { get { return Info.UseGridAfter; } }
		public int GridBefore { get { return GeneralSettings.GridBefore; } set { GeneralSettings.GridBefore = value; } }
		public bool UseGridBefore { get { return Info.UseGridBefore; } }
		public TableRowPropertiesOptions.Mask UseValue { get { return Info.Value; } internal set { Info.Value = value; } }
		protected override PropertiesDictionary<BoolPropertyAccessor<TableRowPropertiesOptions>> AccessorTable { get { return accessorTable; } }
		internal TableRowGeneralSettings GeneralSettings { get { return generalSettings; } }
		#endregion
		protected internal override UniqueItemsCache<TableRowPropertiesOptions> GetCache(DocumentModel documentModel) {
			return documentModel.Cache.TableRowPropertiesOptionsCache;
		}
		public void CopyFrom(TableRowProperties properties) {
			IPropertiesContainer container = this as IPropertiesContainer;
			container.BeginPropertiesUpdate();
			try {
				if (Object.ReferenceEquals(this.DocumentModel, properties.DocumentModel)) {
					Height.CopyFrom(properties.Height);
					WidthBefore.CopyFrom(properties.WidthBefore);
					WidthAfter.CopyFrom(properties.WidthAfter);
					CellSpacing.CopyFrom(properties.CellSpacing);
					GeneralSettings.CopyFrom(properties.GeneralSettings);
					TableRowPropertiesOptions info = this.GetInfoForModification();
					info.CopyFrom(properties.Info);
					ReplaceInfo(info, GetBatchUpdateChangeActions());
				}
				else {
					Height.CopyFrom(properties.Height.Info);
					WidthBefore.CopyFrom(properties.WidthBefore.Info);
					WidthAfter.CopyFrom(properties.WidthAfter.Info);
					CellSpacing.CopyFrom(properties.CellSpacing.Info);
					GeneralSettings.CopyFrom(properties.GeneralSettings.Info);
					this.Info.CopyFrom(properties.Info);
				}
			}
			finally {
				container.EndPropertiesUpdate();
			}
		}
		internal void ResetUse(TableRowPropertiesOptions.Mask mask) {
			TableRowPropertiesOptions newOptions = new TableRowPropertiesOptions(this.Info.Value & (~mask));
			ReplaceInfo(newOptions, GetBatchUpdateChangeActions());
		}
		public bool GetUse(TableRowPropertiesOptions.Mask mask) {
			return Info.GetVal(mask);
		}
		public void Reset() {
			DocumentModel.History.BeginTransaction();
			try {
				CopyFrom(DocumentModel.DefaultTableRowProperties);
				ResetAllUse();
			}
			finally {
				DocumentModel.History.EndTransaction();
			}
		}
		internal void ResetAllUse() {
			ReplaceInfo(GetCache(DocumentModel)[TableRowPropertiesOptionsCache.EmptyRowPropertiesOptionsItem], GetBatchUpdateChangeActions());
		}
		internal void Merge(TableRowProperties properties) {
			IPropertiesContainer container = this as IPropertiesContainer;
			container.BeginPropertiesUpdate();
			try {
				if (!UseCantSplit && properties.UseCantSplit)
					CantSplit = properties.CantSplit;
				if (!UseCellSpacing && properties.UseCellSpacing)
					CellSpacing.CopyFrom(properties.CellSpacing);
				if (!UseGridAfter && properties.UseGridAfter)
					GridAfter = properties.GridAfter;
				if (!UseGridBefore && properties.UseGridBefore)
					GridBefore = properties.GridBefore;
				if (!UseHeader && properties.UseHeader)
					Header = properties.Header;
				if (!UseHeight && properties.UseHeight)
					Height.CopyFrom(properties.Height);
				if (!UseHideCellMark && properties.HideCellMark)
					HideCellMark = properties.HideCellMark;
				if (!UseTableRowAlignment && properties.UseTableRowAlignment)
					TableRowAlignment = properties.TableRowAlignment;
				if (!UseWidthBefore && properties.UseWidthBefore)
					WidthBefore.CopyFrom(properties.WidthBefore);
				if (!UseWidthAfter && properties.UseWidthAfter)
					WidthAfter.CopyFrom(properties.WidthAfter);
			}
			finally {
				container.EndPropertiesUpdate();
			}
		}
		static TableRowPropertiesAccessorTable CreateAccessorTable() {
			TableRowPropertiesAccessorTable result = new TableRowPropertiesAccessorTable();
			result.Add(Properties.CantSplit, new BoolPropertyAccessor<TableRowPropertiesOptions>(TableRowPropertiesOptions.GetUseCantSplit, TableRowPropertiesOptions.SetUseCantSplit));
			result.Add(Properties.CellSpacing, new BoolPropertyAccessor<TableRowPropertiesOptions>(TableRowPropertiesOptions.GetUseCellSpacing, TableRowPropertiesOptions.SetUseCellSpacing));
			result.Add(Properties.GridAfter, new BoolPropertyAccessor<TableRowPropertiesOptions>(TableRowPropertiesOptions.GetUseGridAfter, TableRowPropertiesOptions.SetUseGridAfter));
			result.Add(Properties.GridBefore, new BoolPropertyAccessor<TableRowPropertiesOptions>(TableRowPropertiesOptions.GetUseGridBefore, TableRowPropertiesOptions.SetUseGridBefore));
			result.Add(Properties.Header, new BoolPropertyAccessor<TableRowPropertiesOptions>(TableRowPropertiesOptions.GetUseHeader, TableRowPropertiesOptions.SetUseHeader));
			result.Add(Properties.RowHeight, new BoolPropertyAccessor<TableRowPropertiesOptions>(TableRowPropertiesOptions.GetUseHeight, TableRowPropertiesOptions.SetUseHeight));
			result.Add(Properties.HideCellMark, new BoolPropertyAccessor<TableRowPropertiesOptions>(TableRowPropertiesOptions.GetUseHideCellMark, TableRowPropertiesOptions.SetUseHideCellMark));
			result.Add(Properties.TableRowAlignment, new BoolPropertyAccessor<TableRowPropertiesOptions>(TableRowPropertiesOptions.GetUseTableRowAlignment, TableRowPropertiesOptions.SetUseTableRowAlignment));
			result.Add(Properties.WidthAfter, new BoolPropertyAccessor<TableRowPropertiesOptions>(TableRowPropertiesOptions.GetUseWidthAfter, TableRowPropertiesOptions.SetUseWidthAfter));
			result.Add(Properties.WidthBefore, new BoolPropertyAccessor<TableRowPropertiesOptions>(TableRowPropertiesOptions.GetUseWidthBefore, TableRowPropertiesOptions.SetUseWidthBefore));
			return result;
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
	}
	#endregion
	#region CombinedTableRowPropertiesInfo
	public class CombinedTableRowPropertiesInfo : ICloneable<CombinedTableRowPropertiesInfo>, ISupportsCopyFrom<CombinedTableRowPropertiesInfo> {
		readonly HeightUnitInfo height;
		readonly WidthUnitInfo widthBefore;
		readonly WidthUnitInfo widthAfter;
		readonly WidthUnitInfo cellSpacing;
		readonly TableRowGeneralSettingsInfo generalSettings;
		public CombinedTableRowPropertiesInfo(TableRowProperties rowProperties) {
			this.height = rowProperties.Height.Info;
			this.widthBefore = rowProperties.WidthBefore.Info;
			this.widthAfter = rowProperties.WidthAfter.Info;
			this.cellSpacing = rowProperties.CellSpacing.Info;
			this.generalSettings = rowProperties.GeneralSettings.Info;
		}
		internal CombinedTableRowPropertiesInfo() {
			this.height = new HeightUnitInfo();
			this.widthBefore = new WidthUnitInfo();
			this.widthAfter = new WidthUnitInfo();
			this.cellSpacing = new WidthUnitInfo();
			this.generalSettings = new TableRowGeneralSettingsInfo();
		}
		public HeightUnitInfo Height { get { return height; } }
		public WidthUnitInfo WidthBefore { get { return widthBefore; } }
		public WidthUnitInfo WidthAfter { get { return widthAfter; } }
		public WidthUnitInfo CellSpacing { get { return cellSpacing; } }
		public TableRowGeneralSettingsInfo GeneralSettings { get { return generalSettings; } }
		public CombinedTableRowPropertiesInfo Clone() {
			CombinedTableRowPropertiesInfo clone = new CombinedTableRowPropertiesInfo();
			clone.CopyFrom(this);
			return clone;
		}
		public void CopyFrom(CombinedTableRowPropertiesInfo value) {
			Height.CopyFrom(value.Height);
			WidthBefore.CopyFrom(value.WidthBefore);
			WidthAfter.CopyFrom(value.WidthAfter);
			CellSpacing.CopyFrom(value.CellSpacing);
			GeneralSettings.CopyFrom(value.GeneralSettings);
		}
	}
	#endregion
	#region MergedTableRowProperties
	public class MergedTableRowProperties : MergedProperties<CombinedTableRowPropertiesInfo, TableRowPropertiesOptions> {
		public MergedTableRowProperties(CombinedTableRowPropertiesInfo info, TableRowPropertiesOptions options)
			: base(info, options) {
		}
	}
	#endregion
	#region TableRowPropertiesMerger
	public class TableRowPropertiesMerger : PropertiesMergerBase<CombinedTableRowPropertiesInfo, TableRowPropertiesOptions, MergedTableRowProperties> {
		public TableRowPropertiesMerger(TableRowProperties initialProperties)
			: base(new MergedTableRowProperties(new CombinedTableRowPropertiesInfo(initialProperties), initialProperties.Info)) {
		}
		public TableRowPropertiesMerger(MergedTableRowProperties initialProperties)
			: base(new MergedTableRowProperties(initialProperties.Info, initialProperties.Options)) {
		}
		public void Merge(TableRowProperties properties) {
			MergeCore(new CombinedTableRowPropertiesInfo(properties), properties.Info);
		}
		protected internal override void MergeCore(CombinedTableRowPropertiesInfo info, TableRowPropertiesOptions options) {
			if (!OwnOptions.UseCellSpacing && options.UseCellSpacing) {
				OwnInfo.CellSpacing.CopyFrom(info.CellSpacing);
				OwnOptions.UseCellSpacing = true;
			}
			if (!OwnOptions.UseHeight && options.UseHeight) {
				OwnInfo.Height.CopyFrom(info.Height);
				OwnOptions.UseHeight = true;
			}
			if (!OwnOptions.UseWidthBefore && options.UseWidthBefore) {
				OwnInfo.WidthBefore.CopyFrom(info.WidthBefore);
				OwnOptions.UseWidthBefore = true;
			}
			if (!OwnOptions.UseWidthAfter && options.UseWidthAfter) {
				OwnInfo.WidthAfter.CopyFrom(info.WidthAfter);
				OwnOptions.UseWidthAfter = true;
			}
			if (!OwnOptions.UseCantSplit && options.UseCantSplit) {
				OwnInfo.GeneralSettings.CantSplit = info.GeneralSettings.CantSplit;
				OwnOptions.UseCantSplit = true;
			}
			if (!OwnOptions.UseGridAfter && options.UseGridAfter) {
				OwnInfo.GeneralSettings.GridAfter = info.GeneralSettings.GridAfter;
				OwnOptions.UseGridAfter = true;
			}
			if (!OwnOptions.UseGridBefore && options.UseGridBefore) {
				OwnInfo.GeneralSettings.GridBefore = info.GeneralSettings.GridBefore;
				OwnOptions.UseGridBefore = true;
			}
			if (!OwnOptions.UseHeader && options.UseHeader) {
				OwnInfo.GeneralSettings.Header = info.GeneralSettings.Header;
				OwnOptions.UseHeader = true;
			}
			if (!OwnOptions.UseHideCellMark && options.UseHideCellMark) {
				OwnInfo.GeneralSettings.HideCellMark = info.GeneralSettings.HideCellMark;
				OwnOptions.UseHideCellMark = true;
			}
			if (!OwnOptions.UseTableRowAlignment && options.UseTableRowAlignment) {
				OwnInfo.GeneralSettings.TableRowAlignment = info.GeneralSettings.TableRowAlignment;
				OwnOptions.UseTableRowAlignment = true;
			}
		}
	}
	#endregion
}
