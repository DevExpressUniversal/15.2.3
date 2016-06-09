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
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Model {
	#region ConditionalTableStyleFormattingTypes
	[Flags]
	public enum ConditionalTableStyleFormattingTypes {
		WholeTable = 4096,
		FirstRow = 2048, 
		LastRow = 1024, 
		FirstColumn = 512, 
		LastColumn = 256, 
		OddColumnBanding = 128, 
		EvenColumnBanding = 64, 
		OddRowBanding = 32, 
		EvenRowBanding = 16, 
		TopRightCell = 8, 
		TopLeftCell = 4, 
		BottomRightCell = 2, 
		BottomLeftCell = 1, 
	}
	#endregion
	#region MergingState
	public enum MergingState {
		None,
		Continue,
		Restart
	}
	#endregion
	#region ShadingPattern
	public enum ShadingPattern {
		Clear,
		DiagCross,
		DiagStripe,
		HorzCross,
		HorzStripe,
		Nil,
		Pct5,
		Pct10,
		Pct12,
		Pct15,
		Pct20,
		Pct25,
		Pct30,
		Pct35,
		Pct37,
		Pct40,
		Pct45,
		Pct50,
		Pct55,
		Pct60,
		Pct62,
		Pct65,
		Pct70,
		Pct75,
		Pct80,
		Pct85,
		Pct87,
		Pct90,
		Pct95,
		ReverseDiagStripe,
		Solid,
		ThinDiagCross,
		ThinDiagStripe,
		ThinHorzCross,
		ThinHorzStripe,
		ThinReverseDiagStripe,
		ThinVertStripe,
		VertStripe
	}
	#endregion
	#region TableCellProperty
	public enum TableCellProperty {
		PreferredWidth,
		HideCellMark,
		FitText,
		CellMargins,
		TextDirection,
		VerticalAlignment,
		ColumnSpan,
		HorizontalMerging,
		VerticalMerging,
		CellConditionalFormatting,
		Borers,
		BackgroundColor, 
		Shading
	}
	#endregion
	#region TableCellGeneralSettingsInfo
	public class TableCellGeneralSettingsInfo : ICloneable<TableCellGeneralSettingsInfo>, ISupportsCopyFrom<TableCellGeneralSettingsInfo>, ISupportsSizeOf {
		#region Fields
		bool hideCellMark; 
		bool noWrap;
		bool fitText;
		TextDirection textDirection;
		VerticalAlignment verticalAlignment;
		int columnSpan = 1;
		MergingState horizontalMerging; 
		MergingState verticalMerging;
		ConditionalTableStyleFormattingTypes cellConditionalFormatting;
		Color backgroundColor;
		Color foregroundColor;
		ShadingPattern shadingPattern;
#if THEMES_EDIT
		Shading shading;
#endif
		#endregion
		#region Properties
		public bool HideCellMark { get { return hideCellMark; } set { hideCellMark = value; } }
		public bool NoWrap { get { return noWrap; } set { noWrap = value; } }
		public bool FitText { get { return fitText; } set { fitText = value; } }
		public TextDirection TextDirection { get { return textDirection; } set { textDirection = value; } }
		public VerticalAlignment VerticalAlignment { get { return verticalAlignment; } set { verticalAlignment = value; } }
		public int ColumnSpan { get { return columnSpan; } set { columnSpan = value; } }
		public MergingState HorizontalMerging { get { return horizontalMerging; } set { horizontalMerging = value; } }
		public MergingState VerticalMerging { get { return verticalMerging; } set { verticalMerging = value; } }
		public ConditionalTableStyleFormattingTypes CellConditionalFormatting { get { return cellConditionalFormatting; } set { cellConditionalFormatting = value; } }
		public Color BackgroundColor { get { return backgroundColor; } set { backgroundColor = value; } }
		public Color ForegroundColor { get { return foregroundColor; } set { foregroundColor = value; } }
		public ShadingPattern ShadingPattern { get { return shadingPattern; } set { shadingPattern = value; } }
#if THEMES_EDIT        
		public Shading Shading { get { return shading; } set { shading = value; } }
#endif
		#endregion
		public TableCellGeneralSettingsInfo Clone() {
			TableCellGeneralSettingsInfo info = new TableCellGeneralSettingsInfo();
			info.CopyFrom(this);
			return info;
		}
		public void CopyFrom(TableCellGeneralSettingsInfo info) {
			HideCellMark = info.HideCellMark;
			NoWrap = info.NoWrap;
			FitText = info.FitText;
			TextDirection = info.TextDirection;
			VerticalAlignment = info.VerticalAlignment;
			ColumnSpan = info.ColumnSpan;
			HorizontalMerging = info.HorizontalMerging;
			VerticalMerging = info.VerticalMerging;
			CellConditionalFormatting = info.CellConditionalFormatting;
			BackgroundColor = info.BackgroundColor;
			ForegroundColor = info.ForegroundColor;
			ShadingPattern = info.ShadingPattern;
#if THEMES_EDIT
			Shading = info.Shading;
#endif
		}
		public override bool Equals(object obj) {
			TableCellGeneralSettingsInfo info = obj as TableCellGeneralSettingsInfo;
			if (info == null)
				return false;
			return HideCellMark == info.HideCellMark &&
				   NoWrap == info.NoWrap &&
				   FitText == info.FitText &&
				   TextDirection == info.TextDirection &&
				   VerticalAlignment == info.VerticalAlignment &&
				   ColumnSpan == info.ColumnSpan &&
				   HorizontalMerging == info.HorizontalMerging &&
				   VerticalMerging == info.VerticalMerging &&
				   CellConditionalFormatting == info.CellConditionalFormatting &&
#if THEMES_EDIT
				   Shading.Equals(info.Shading) &&
#endif
				   BackgroundColor == info.BackgroundColor &&
				   ForegroundColor == info.ForegroundColor &&
				   ShadingPattern == info.ShadingPattern;
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
	#region TableCellGeneralSettingsInfoCache
	public class TableCellGeneralSettingsInfoCache : UniqueItemsCache<TableCellGeneralSettingsInfo> {
		public TableCellGeneralSettingsInfoCache(DocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override TableCellGeneralSettingsInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			TableCellGeneralSettingsInfo info = new TableCellGeneralSettingsInfo();
			info.TextDirection = TextDirection.LeftToRightTopToBottom;
			info.VerticalAlignment = VerticalAlignment.Top;
			info.ColumnSpan = 1;
			info.HorizontalMerging = MergingState.None;
			info.VerticalMerging = MergingState.None;
			info.CellConditionalFormatting = ConditionalTableStyleFormattingTypes.WholeTable;
			info.BackgroundColor = DXColor.Empty;
			info.ForegroundColor = DXColor.Empty;
			info.ShadingPattern = ShadingPattern.Clear;
#if THEMES_EDIT
			info.Shading = Shading.Create();
#endif
			return info;
		}
	}
	#endregion
	#region TableCellGeneralSettings
	public class TableCellGeneralSettings : RichEditIndexBasedObject<TableCellGeneralSettingsInfo> {
		readonly IPropertiesContainer owner;
		public TableCellGeneralSettings(PieceTable pieceTable, IPropertiesContainer owner)
			: base(pieceTable) {
			Guard.ArgumentNotNull(owner, "owner");
			this.owner = owner;
		}
		#region Properties
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
		DocumentModelChangeActions SetHideCellMark(TableCellGeneralSettingsInfo info, bool value) {
			info.HideCellMark = value;
			return TableCellChangeActionsCalculator.CalculateChangeActions(TableCellChangeType.HideCellMark);
		}
		#endregion
		#region NoWrap
		public bool NoWrap {
			get { return Info.NoWrap; }
			set {
				BeginChanging(Properties.NoWrap);
				if (value == Info.NoWrap) {
					EndChanging();
					return;
				}
				SetPropertyValue(SetNoWrap, value);
				EndChanging();
			}
		}
		DocumentModelChangeActions SetNoWrap(TableCellGeneralSettingsInfo info, bool value) {
			info.NoWrap = value;
			return TableCellChangeActionsCalculator.CalculateChangeActions(TableCellChangeType.NoWrap);
		}
		#endregion
		#region FitText
		public bool FitText {
			get { return Info.FitText; }
			set {
				BeginChanging(Properties.FitText);
				if (value == Info.FitText) {
					EndChanging();
					return;
				}
				SetPropertyValue(SetFitText, value);
				EndChanging();
			}
		}
		DocumentModelChangeActions SetFitText(TableCellGeneralSettingsInfo info, bool value) {
			info.FitText = value;
			return TableCellChangeActionsCalculator.CalculateChangeActions(TableCellChangeType.FitText);
		}
		#endregion
		#region TextDirection
		public TextDirection TextDirection {
			get { return Info.TextDirection; }
			set {
				BeginChanging(Properties.TextDirection);
				if (value == Info.TextDirection) {
					EndChanging();
					return;
				}
				SetPropertyValue(SetTextDirection, value);
				EndChanging();
			}
		}
		DocumentModelChangeActions SetTextDirection(TableCellGeneralSettingsInfo info, TextDirection value) {
			info.TextDirection = value;
			return TableCellChangeActionsCalculator.CalculateChangeActions(TableCellChangeType.TextDirection);
		}
		#endregion
		#region VerticalAlignment
		public VerticalAlignment VerticalAlignment {
			get { return Info.VerticalAlignment; }
			set {
				BeginChanging(Properties.VerticalAlignment);
				if (value == Info.VerticalAlignment) {
					EndChanging();
					return;
				}
				SetPropertyValue(SetVerticalAlignment, value);
				EndChanging();
			}
		}
		DocumentModelChangeActions SetVerticalAlignment(TableCellGeneralSettingsInfo info, VerticalAlignment value) {
			info.VerticalAlignment = value;
			return TableCellChangeActionsCalculator.CalculateChangeActions(TableCellChangeType.VerticalAlignment);
		}
		#endregion
		#region ColumnSpan
		public int ColumnSpan {
			get { return Info.ColumnSpan; }
			set {
				if (value <= 0)
					Exceptions.ThrowArgumentException("ColumnSpan", value);
				BeginChanging(Properties.ColumnSpan);
				if (value == Info.ColumnSpan) {
					EndChanging();
					return;
				}
				SetPropertyValue(SetColumnSpan, value);
				EndChanging();
			}
		}
		DocumentModelChangeActions SetColumnSpan(TableCellGeneralSettingsInfo info, int value) {
			info.ColumnSpan = value;
			return TableCellChangeActionsCalculator.CalculateChangeActions(TableCellChangeType.ColumnSpan);
		}
		#endregion
		#region VerticalMerging
		public MergingState VerticalMerging {
			get { return Info.VerticalMerging; }
			set {
				BeginChanging(Properties.VerticalMerging);
				if (value == Info.VerticalMerging) {
					EndChanging();
					return;
				}
				SetPropertyValue(SetVerticalMerging, value);
				EndChanging();
			}
		}
		DocumentModelChangeActions SetVerticalMerging(TableCellGeneralSettingsInfo info, MergingState value) {
			info.VerticalMerging = value;
			return TableCellChangeActionsCalculator.CalculateChangeActions(TableCellChangeType.VerticalMerging);
		}
		#endregion
		#region CellConditionalFormatting
		public ConditionalTableStyleFormattingTypes CellConditionalFormatting {
			get { return Info.CellConditionalFormatting; }
			set {
				BeginChanging(Properties.CellConditionalFormatting);
				if (value == Info.CellConditionalFormatting) {
					EndChanging();
					return;
				}
				SetPropertyValue(SetCellConditionalFormatting, value);
				EndChanging();
			}
		}
		DocumentModelChangeActions SetCellConditionalFormatting(TableCellGeneralSettingsInfo info, ConditionalTableStyleFormattingTypes value) {
			info.CellConditionalFormatting = value;
			return TableCellChangeActionsCalculator.CalculateChangeActions(TableCellChangeType.ConditionalFormatting);
		}
		#endregion
		#region BackgroundColor
		public Color BackgroundColor {
			get { return Info.BackgroundColor; }
			set {
				BeginChanging(Properties.BackgroundColor);
				if (value == Info.BackgroundColor) {
					EndChanging();
					return;
				}
				SetPropertyValue(SetBackgroundColor, value);
				EndChanging();
			}
		}
		DocumentModelChangeActions SetBackgroundColor(TableCellGeneralSettingsInfo info, Color value) {
			info.BackgroundColor = value;
			return TableCellChangeActionsCalculator.CalculateChangeActions(TableCellChangeType.BackgroundColor);
		}
		#endregion
		#region ForegroundColor
		public Color ForegroundColor {
			get { return Info.ForegroundColor; }
			set {
				BeginChanging(Properties.ForegroundColor);
				if (value == Info.ForegroundColor) {
					EndChanging();
					return;
				}
				SetPropertyValue(SetForegroundColor, value);
				EndChanging();
			}
		}
		DocumentModelChangeActions SetForegroundColor(TableCellGeneralSettingsInfo info, Color value) {
			info.ForegroundColor = value;
			return TableCellChangeActionsCalculator.CalculateChangeActions(TableCellChangeType.ForegroundColor);
		}
		#endregion
		#region ShadingPattern
		public ShadingPattern ShadingPattern {
			get { return Info.ShadingPattern; }
			set {
				BeginChanging(Properties.ShadingPattern);
				if (value == Info.ShadingPattern) {
					EndChanging();
					return;
				}
				SetPropertyValue(SetShadingPattern, value);
				EndChanging();
			}
		}
		DocumentModelChangeActions SetShadingPattern(TableCellGeneralSettingsInfo info, ShadingPattern value) {
			info.ShadingPattern = value;
			return TableCellChangeActionsCalculator.CalculateChangeActions(TableCellChangeType.ShadingPattern);
		}
		#endregion
#if THEMES_EDIT
		#region Shading
		public Shading Shading {
			get { return Info.Shading; }
			set {
				BeginChanging(Properties.Shading);
				if (value == Info.Shading) {
					EndChanging();
					return;
				}
				SetPropertyValue(SetShading, value);
				EndChanging();
			}
		}
		DocumentModelChangeActions SetShading(TableCellGeneralSettingsInfo info, Shading value) {
			info.Shading = value;
			return TableCellChangeActionsCalculator.CalculateChangeActions(TableCellChangeType.Shading);
		}
		#endregion
#endif
		internal IPropertiesContainer Owner { get { return owner; } }
		#endregion
		protected internal override UniqueItemsCache<TableCellGeneralSettingsInfo> GetCache(DocumentModel documentModel) {
			return documentModel.Cache.TableCellGeneralSettingsInfoCache;
		}
		public void CopyFrom(TableCellGeneralSettings newSettings) {
			Owner.BeginPropertiesUpdate();
			try {
				BeginUpdate();
				try {
					this.HideCellMark = newSettings.HideCellMark;
					this.NoWrap = newSettings.NoWrap;
					this.FitText = newSettings.FitText;
					this.TextDirection = newSettings.TextDirection;
					this.VerticalAlignment = newSettings.VerticalAlignment;
					this.ColumnSpan = newSettings.ColumnSpan;
					this.VerticalMerging = newSettings.VerticalMerging;
					this.CellConditionalFormatting = newSettings.CellConditionalFormatting;
					this.BackgroundColor = newSettings.BackgroundColor;
					this.ForegroundColor = newSettings.ForegroundColor;
					this.ShadingPattern = newSettings.ShadingPattern;
#if THEMES_EDIT
					this.Shading = newSettings.Shading;
#endif
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
			return TableCellChangeActionsCalculator.CalculateChangeActions(TableCellChangeType.BatchUpdate);
		}
		protected override void ApplyChanges(DocumentModelChangeActions changeActions) {
			Owner.ApplyChanges(changeActions);
		}
		protected internal override void RaiseObtainAffectedRange(ObtainAffectedRangeEventArgs args) {
			Owner.RaiseObtainAffectedRange(args);
		}
	}
	#endregion
	#region TableCellChangeType
	public enum TableCellChangeType {
		None = 0,
		HideCellMark,
		NoWrap,
		FitText,
		TextDirection,
		VerticalAlignment,
		ColumnSpan,
		HorizontalMerging,
		VerticalMerging,
		ConditionalFormatting,
		BackgroundColor,
		ForegroundColor,
		ShadingPattern,
		Shading,
		BatchUpdate
	}
	#endregion
	#region TableCellChangeActionsCalculator
	public static class TableCellChangeActionsCalculator {
		internal class TableCellChangeActionsTable : Dictionary<TableCellChangeType, DocumentModelChangeActions> {
		}
		internal static readonly TableCellChangeActionsTable tableCellChangeActionsTable = CreateTableCellChangeActionsTable();
		internal static TableCellChangeActionsTable CreateTableCellChangeActionsTable() {
			TableCellChangeActionsTable table = new TableCellChangeActionsTable();
			table.Add(TableCellChangeType.None, DocumentModelChangeActions.None);
			table.Add(TableCellChangeType.HideCellMark, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout);
			table.Add(TableCellChangeType.NoWrap, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ForceResetHorizontalRuler | DocumentModelChangeActions.ForceResetVerticalRuler);
			table.Add(TableCellChangeType.FitText, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ForceResetHorizontalRuler | DocumentModelChangeActions.ForceResetVerticalRuler);
			table.Add(TableCellChangeType.TextDirection, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout);
			table.Add(TableCellChangeType.VerticalAlignment, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(TableCellChangeType.ColumnSpan, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ForceResetHorizontalRuler | DocumentModelChangeActions.ForceResetVerticalRuler);
			table.Add(TableCellChangeType.HorizontalMerging, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ForceResetHorizontalRuler | DocumentModelChangeActions.ForceResetVerticalRuler);
			table.Add(TableCellChangeType.VerticalMerging, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ForceResetHorizontalRuler | DocumentModelChangeActions.ForceResetVerticalRuler);
			table.Add(TableCellChangeType.ConditionalFormatting, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ForceResetHorizontalRuler | DocumentModelChangeActions.ForceResetVerticalRuler);
			table.Add(TableCellChangeType.BackgroundColor, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetSecondaryLayout); 
			table.Add(TableCellChangeType.ForegroundColor, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetSecondaryLayout); 
			table.Add(TableCellChangeType.ShadingPattern, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetSecondaryLayout); 
			table.Add(TableCellChangeType.Shading, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetSecondaryLayout); 
			table.Add(TableCellChangeType.BatchUpdate, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ForceResetHorizontalRuler | DocumentModelChangeActions.ForceResetVerticalRuler);
			return table;
		}
		public static DocumentModelChangeActions CalculateChangeActions(TableCellChangeType change) {
			return tableCellChangeActionsTable[change];
		}
	}
	#endregion
	#region TableCellPropertiesOptions
	public class TableCellPropertiesOptions : ICloneable<TableCellPropertiesOptions>, ISupportsCopyFrom<TableCellPropertiesOptions>, ISupportsSizeOf {
		#region Mask
		public enum Mask {
			UseNone = 0x00000000,
			UsePreferredWidth = 0x00000001,
			UseHideCellMark = 0x00000002,
			UseNoWrap = 0x00000004,
			UseFitText = 0x00000008,
			UseLeftMargin = 0x00000010,
			UseRightMargin = 0x00000020,
			UseTopMargin = 0x00000040,
			UseBottomMargin = 0x00000080,			
			UseTextDirection = 0x00000100,
			UseVerticalAlignment = 0x00000200,
			UseCellConditionalFormatting = 0x00000800,
			UseLeftBorder = 0x00001000,
			UseRightBorder = 0x00002000,
			UseTopBorder = 0x00004000,
			UseBottomBorder = 0x00008000,
			UseInsideHorizontalBorder = 0x00010000,
			UseInsideVerticalBorder = 0x000020000,
			UseTopLeftDiagonalBorder = 0x00040000,
			UseTopRightDiagonalBorder = 0x00080000,
			UseBackgroundColor = 0x00100000,
			UseForegroundColor = 0x00200000,
			UseShading = 0x00400000,
			UseAll = 0x7FFFFFFF
		}
		#endregion
		Mask val;
		public TableCellPropertiesOptions() {
			val = Mask.UseNone;
		}
		internal TableCellPropertiesOptions(Mask val) {
			this.val = val;
		}
		#region Properties
		public bool UseLeftBorder { get { return GetVal(Mask.UseLeftBorder); } set { SetVal(Mask.UseLeftBorder, value); } }
		public bool UseRightBorder { get { return GetVal(Mask.UseRightBorder); } set { SetVal(Mask.UseRightBorder, value); } }
		public bool UseTopBorder { get { return GetVal(Mask.UseTopBorder); } set { SetVal(Mask.UseTopBorder, value); } }
		public bool UseBottomBorder { get { return GetVal(Mask.UseBottomBorder); } set { SetVal(Mask.UseBottomBorder, value); } }
		public bool UseInsideHorizontalBorder { get { return GetVal(Mask.UseInsideHorizontalBorder); } set { SetVal(Mask.UseInsideHorizontalBorder, value); } }
		public bool UseInsideVerticalBorder { get { return GetVal(Mask.UseInsideVerticalBorder); } set { SetVal(Mask.UseInsideVerticalBorder, value); } }
		public bool UseTopLeftDiagonalBorder { get { return GetVal(Mask.UseTopLeftDiagonalBorder); } set { SetVal(Mask.UseTopLeftDiagonalBorder, value); } }
		public bool UseTopRightDiagonalBorder { get { return GetVal(Mask.UseTopRightDiagonalBorder); } set { SetVal(Mask.UseTopRightDiagonalBorder, value); } }
		public bool UsePreferredWidth { get { return GetVal(Mask.UsePreferredWidth); } set { SetVal(Mask.UsePreferredWidth, value); } }
		public bool UseHideCellMark { get { return GetVal(Mask.UseHideCellMark); } set { SetVal(Mask.UseHideCellMark, value); } }
		public bool UseNoWrap { get { return GetVal(Mask.UseNoWrap); } set { SetVal(Mask.UseNoWrap, value); } }
		public bool UseFitText { get { return GetVal(Mask.UseFitText); } set { SetVal(Mask.UseFitText, value); } }
		public bool UseLeftMargin { get { return GetVal(Mask.UseLeftMargin); } set { SetVal(Mask.UseLeftMargin, value); } }
		public bool UseRightMargin { get { return GetVal(Mask.UseRightMargin); } set { SetVal(Mask.UseRightMargin, value); } }
		public bool UseTopMargin { get { return GetVal(Mask.UseTopMargin); } set { SetVal(Mask.UseTopMargin, value); } }
		public bool UseBottomMargin { get { return GetVal(Mask.UseBottomMargin); } set { SetVal(Mask.UseBottomMargin, value); } }
		public bool UseTextDirection { get { return GetVal(Mask.UseTextDirection); } set { SetVal(Mask.UseTextDirection, value); } }
		public bool UseVerticalAlignment { get { return GetVal(Mask.UseVerticalAlignment); } set { SetVal(Mask.UseVerticalAlignment, value); } }		
		public bool UseCellConditionalFormatting { get { return GetVal(Mask.UseCellConditionalFormatting); } set { SetVal(Mask.UseCellConditionalFormatting, value); } }
		public bool UseBackgroundColor { get { return GetVal(Mask.UseBackgroundColor); } set { SetVal(Mask.UseBackgroundColor, value); } }
		public bool UseForegroundColor { get { return GetVal(Mask.UseForegroundColor); } set { SetVal(Mask.UseForegroundColor, value); } }
		public bool UseShading { get { return GetVal(Mask.UseShading); } set { SetVal(Mask.UseShading, value); } }
		internal Mask Value { get { return val; } set { val = value; } }
		#endregion
		public TableCellPropertiesOptions Clone() {
			TableCellPropertiesOptions clone = new TableCellPropertiesOptions();
			clone.CopyFrom(this);
			return clone;
		}
		public void CopyFrom(TableCellPropertiesOptions options) {
			this.val = options.val;
		}
		public override bool Equals(object obj) {
			TableCellPropertiesOptions opts = obj as TableCellPropertiesOptions;
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
		internal static bool GetOptionsUseCellConditionalFormatting(TableCellPropertiesOptions options) { return options.UseCellConditionalFormatting; }
		internal static void SetOptionsUseCellConditionalFormatting(TableCellPropertiesOptions options, bool value) { options.UseCellConditionalFormatting = value; }
		internal static bool GetOptionsUseTopMargin(TableCellPropertiesOptions options) { return options.UseTopMargin; }
		internal static void SetOptionsUseTopMargin(TableCellPropertiesOptions options, bool value) { options.UseTopMargin= value; }
		internal static bool GetOptionsUseBottomMargin(TableCellPropertiesOptions options) { return options.UseBottomMargin; }
		internal static void SetOptionsUseBottomMargin(TableCellPropertiesOptions options, bool value) { options.UseBottomMargin = value; }
		internal static bool GetOptionsUseLeftMargin(TableCellPropertiesOptions options) { return options.UseLeftMargin; }
		internal static void SetOptionsUseLeftMargin(TableCellPropertiesOptions options, bool value) { options.UseLeftMargin = value; }
		internal static bool GetOptionsUseRightMargin(TableCellPropertiesOptions options) { return options.UseRightMargin; }
		internal static void SetOptionsUseRightMargin(TableCellPropertiesOptions options, bool value) { options.UseRightMargin = value; }
		internal static bool GetOptionsUseFitText(TableCellPropertiesOptions options) { return options.UseFitText; }
		internal static void SetOptionsUseFitText(TableCellPropertiesOptions options, bool value) { options.UseFitText = value; }
		internal static bool GetOptionsUseHideCellMark(TableCellPropertiesOptions options) { return options.UseHideCellMark; }
		internal static void SetOptionsUseHideCellMark(TableCellPropertiesOptions options, bool value) { options.UseHideCellMark = value; }
		internal static bool GetOptionsUseNoWrap(TableCellPropertiesOptions options) { return options.UseNoWrap; }
		internal static void SetOptionsUseNoWrap(TableCellPropertiesOptions options, bool value) { options.UseNoWrap = value; }
		internal static bool GetOptionsUsePreferredWidth(TableCellPropertiesOptions options) { return options.UsePreferredWidth; }
		internal static void SetOptionsUsePreferredWidth(TableCellPropertiesOptions options, bool value) { options.UsePreferredWidth = value; }
		internal static bool GetOptionsUseTextDirection(TableCellPropertiesOptions options) { return options.UseTextDirection; }
		internal static void SetOptionsUseTextDirection(TableCellPropertiesOptions options, bool value) { options.UseTextDirection = value; }
		internal static bool GetOptionsUseVerticalAlignment(TableCellPropertiesOptions options) { return options.UseVerticalAlignment; }
		internal static void SetOptionsUseVerticalAlignment(TableCellPropertiesOptions options, bool value) { options.UseVerticalAlignment = value; }
		internal static bool GetOptionsUseVerticalMerging(TableCellPropertiesOptions options) { return true; }
		internal static void SetOptionsUseVerticalMerging(TableCellPropertiesOptions options, bool value) {  }
		internal static bool GetOptionsUseColumnSpan(TableCellPropertiesOptions options) { return true; }
		internal static void SetOptionsUseColumnSpan(TableCellPropertiesOptions options, bool value) {  }
		internal static void SetOptionsUseBackgroundColor(TableCellPropertiesOptions options, bool value) { options.UseBackgroundColor = value; }
		internal static bool GetOptionsUseBackgroundColor(TableCellPropertiesOptions options) { return options.UseBackgroundColor; }
		internal static void SetOptionsUseForegroundColor(TableCellPropertiesOptions options, bool value) { options.UseForegroundColor = value; }
		internal static bool GetOptionsUseForegroundColor(TableCellPropertiesOptions options) { return options.UseForegroundColor; }
		internal static void SetOptionsUseShading(TableCellPropertiesOptions options, bool value) { options.UseShading = value; }
		internal static bool GetOptionsUseShading(TableCellPropertiesOptions options) { return options.UseShading; }
		internal static bool GetOptionsUseLeftBorder(TableCellPropertiesOptions options) { return options.UseLeftBorder; }
		internal static void SetOptionsUseLeftBorder(TableCellPropertiesOptions options, bool value) { options.UseLeftBorder = value; }
		internal static bool GetOptionsUseRightBorder(TableCellPropertiesOptions options) { return options.UseRightBorder; }
		internal static void SetOptionsUseRightBorder(TableCellPropertiesOptions options, bool value) { options.UseRightBorder = value; }
		internal static bool GetOptionsUseTopBorder(TableCellPropertiesOptions options) { return options.UseTopBorder; }
		internal static void SetOptionsUseTopBorder(TableCellPropertiesOptions options, bool value) { options.UseTopBorder = value; }
		internal static bool GetOptionsUseBottomBorder(TableCellPropertiesOptions options) { return options.UseBottomBorder; }
		internal static void SetOptionsUseBottomBorder(TableCellPropertiesOptions options, bool value) { options.UseBottomBorder = value; }
		internal static bool GetOptionsUseInsideVerticalBorder(TableCellPropertiesOptions options) { return options.UseInsideVerticalBorder; }
		internal static void SetOptionsUseInsideVerticalBorder(TableCellPropertiesOptions options, bool value) { options.UseInsideVerticalBorder = value; }
		internal static bool GetOptionsUseInsideHorizontalBorder(TableCellPropertiesOptions options) { return options.UseInsideHorizontalBorder; }
		internal static void SetOptionsUseInsideHorizontalBorder(TableCellPropertiesOptions options, bool value) { options.UseInsideHorizontalBorder = value; }
		internal static bool GetOptionsUseTopLeftDiagonalBorder(TableCellPropertiesOptions options) { return options.UseTopLeftDiagonalBorder; }
		internal static void SetOptionsUseTopLeftDiagonalBorder(TableCellPropertiesOptions options, bool value) { options.UseTopLeftDiagonalBorder = value; }
		internal static bool GetOptionsUseTopRightDiagonalBorder(TableCellPropertiesOptions options) { return options.UseTopRightDiagonalBorder; }
		internal static void SetOptionsUseTopRightDiagonalBorder(TableCellPropertiesOptions options, bool value) { options.UseTopRightDiagonalBorder = value; }
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
		#endregion
	}
	#endregion
	#region TableCellPropertiesOptionsCache
	public class TableCellPropertiesOptionsCache : UniqueItemsCache<TableCellPropertiesOptions> {
		internal const int EmptyCellPropertiesOptionsItem = 0;
		internal const int RootCellPropertiesOptionsItem = 1;
		public TableCellPropertiesOptionsCache(DocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override void InitItems(IDocumentModelUnitConverter unitConverter) {
			base.InitItems(unitConverter);
			AddRootStyleOptions();
		}
		protected override TableCellPropertiesOptions CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new TableCellPropertiesOptions();
		}
		void AddRootStyleOptions() {
			AppendItem(new TableCellPropertiesOptions(TableCellPropertiesOptions.Mask.UseAll));
		}
	}
	#endregion
	#region TableCellProperties
	public class TableCellProperties : PropertiesBase<TableCellPropertiesOptions>, ICellPropertiesContainer, ICellMarginsContainer {
		public class TableCellPropertiesAccessorTable : PropertiesDictionary<BoolPropertyAccessor<TableCellPropertiesOptions>> {
		}
		#region Fields
		static readonly TableCellPropertiesAccessorTable accessorTable = CreateAccessorTable();
		readonly PreferredWidth preferredWidth;
		readonly ICellPropertiesOwner owner;
		readonly CellMargins cellMargins;
		readonly TableCellBorders borders;
		readonly TableCellGeneralSettings generalSettings;
		#endregion
		public TableCellProperties(PieceTable pieceTable, ICellPropertiesOwner owner)
			: base(pieceTable) {
			Guard.ArgumentNotNull(owner, "owner");
			this.preferredWidth = new PreferredWidth(pieceTable, this);
			this.cellMargins = new CellMargins(pieceTable, this);
			this.borders = new TableCellBorders(pieceTable, this);
			this.generalSettings = new TableCellGeneralSettings(pieceTable, this);
			this.owner = owner;
		}
		protected override PropertiesDictionary<BoolPropertyAccessor<TableCellPropertiesOptions>> AccessorTable { get { return accessorTable; } }
		#region Properties
		public PreferredWidth PreferredWidth { get { return preferredWidth; } }
		public bool UsePreferredWidth { get { return Info.UsePreferredWidth; } }
		public CellMargins CellMargins { get { return cellMargins; } }
		public bool UseLeftMargin { get { return Info.UseLeftMargin; } }
		public bool UseRightMargin { get { return Info.UseRightMargin; } }
		public bool UseTopMargin { get { return Info.UseTopMargin; } }
		public bool UseBottomMargin { get { return Info.UseBottomMargin; } }
		public TableCellBorders Borders { get { return borders; } }
		public bool HideCellMark { get { return GeneralSettings.HideCellMark; } set { GeneralSettings.HideCellMark = value; } }
		public bool UseHideCellMark { get { return Info.UseHideCellMark; } }
		public bool NoWrap { get { return GeneralSettings.NoWrap; } set { GeneralSettings.NoWrap = value; } }
		public bool UseNoWrap { get { return Info.UseNoWrap; } }
		public bool FitText { get { return GeneralSettings.FitText; } set { GeneralSettings.FitText = value; } }
		public bool UseFitText { get { return Info.UseFitText; } }
		public Color BackgroundColor { get { return GeneralSettings.BackgroundColor; } set { GeneralSettings.BackgroundColor = value; } }
		public Color ForegroundColor { get { return GeneralSettings.ForegroundColor; } set { GeneralSettings.ForegroundColor = value; } }
		public ShadingPattern ShadingPattern { get { return GeneralSettings.ShadingPattern; } set { GeneralSettings.ShadingPattern = value; } }
#if THEMES_EDIT
		public Shading Shading { get { return GeneralSettings.Shading; } set { GeneralSettings.Shading = value; } }
#endif
		public bool UseBackgroundColor { get { return Info.UseBackgroundColor; } }
		public bool UseForegroundColor { get { return Info.UseForegroundColor; } }
		public bool UseShading { get { return Info.UseShading; } }
		public TextDirection TextDirection { get { return GeneralSettings.TextDirection; } set { GeneralSettings.TextDirection = value; } }
		public bool UseTextDirection { get { return Info.UseTextDirection; } }
		public VerticalAlignment VerticalAlignment { get { return GeneralSettings.VerticalAlignment; } set { GeneralSettings.VerticalAlignment = value; } }
		public bool UseVerticalAlignment { get { return Info.UseVerticalAlignment; } }
		public int ColumnSpan { get { return GeneralSettings.ColumnSpan; } set { GeneralSettings.ColumnSpan = value; } }
		public MergingState VerticalMerging { get { return GeneralSettings.VerticalMerging; } set { GeneralSettings.VerticalMerging = value; } }
		public ConditionalTableStyleFormattingTypes CellConditionalFormatting { get { return GeneralSettings.CellConditionalFormatting; } set { GeneralSettings.CellConditionalFormatting = value; } }
		public bool UseCellConditionalFormatting { get { return Info.UseCellConditionalFormatting; } }
		public TableCellPropertiesOptions.Mask UseValue { get { return Info.Value; } set { Info.Value = value; } }
		internal TableCellGeneralSettings GeneralSettings { get { return generalSettings; } }
		#endregion
		#region Events
		EventHandler propertiesChanged;
		public event EventHandler PropertiesChanged {
			add { propertiesChanged += value; }
			remove { propertiesChanged -= value; }
		}
		#endregion
		protected internal override UniqueItemsCache<TableCellPropertiesOptions> GetCache(DocumentModel documentModel) {
			return documentModel.Cache.TableCellPropertiesOptionsCache;
		}
		internal void ResetUse(TableCellPropertiesOptions.Mask mask) {
			TableCellPropertiesOptions newOptions = new TableCellPropertiesOptions(this.Info.Value & (~mask));
			ReplaceInfo(newOptions, GetBatchUpdateChangeActions());
		}
		public bool GetUse(TableCellPropertiesOptions.Mask mask) {
			return Info.GetVal(mask);
		}
		public void CopyFrom(TableCellProperties properties) {
			IPropertiesContainer container = this as IPropertiesContainer;
			DocumentModel.BeginUpdate();
			container.BeginPropertiesUpdate();
			try {
				if (Object.ReferenceEquals(this.DocumentModel, properties.DocumentModel)) {
					this.GeneralSettings.CopyFrom(properties.GeneralSettings);
					this.Borders.CopyFrom(properties.Borders); 
					this.CellMargins.CopyFrom(properties.CellMargins);
					this.PreferredWidth.CopyFrom(properties.PreferredWidth);
					TableCellPropertiesOptions info = this.GetInfoForModification();
					info.CopyFrom(properties.Info);
					ReplaceInfo(info, GetBatchUpdateChangeActions());
				}
				else {
					this.GeneralSettings.CopyFrom(properties.GeneralSettings.Info);
					this.Borders.CopyFrom(properties.Borders);  
					this.CellMargins.CopyFrom(properties.CellMargins);
					this.PreferredWidth.CopyFrom(properties.PreferredWidth.Info);
					this.Info.CopyFrom(properties.Info);
				}
			}
			finally {
				container.EndPropertiesUpdate();
				DocumentModel.EndUpdate();
			}
		}
		internal void CopyFrom(MergedTableCellProperties properties) {
			IPropertiesContainer container = this as IPropertiesContainer;
			container.BeginPropertiesUpdate();
			try {
				this.GeneralSettings.CopyFrom(properties.Info.GeneralSettings);				
				this.Borders.CopyFrom(properties.Info.Borders);
				this.CellMargins.CopyFrom(properties.Info.CellMargins);
				this.PreferredWidth.CopyFrom(properties.Info.PreferredWidth);				
				this.Info.CopyFrom(properties.Options);				
			}
			finally {
				container.EndPropertiesUpdate();
			}
		}
		public void Reset() {
			DocumentModel.History.BeginTransaction();
			try {
				CopyFrom(DocumentModel.DefaultTableCellProperties);
				ResetAllUse();
			}
			finally {
				DocumentModel.History.EndTransaction();
			}
		}
		internal void ResetAllUse() {
			ReplaceInfo(GetCache(DocumentModel)[TableCellPropertiesOptionsCache.EmptyCellPropertiesOptionsItem], GetBatchUpdateChangeActions());
		}		
		void RaisePropertiesChanged() {
			if (propertiesChanged != null)
				propertiesChanged(this, EventArgs.Empty);
		}
		internal void Merge(TableCellProperties properties) {
			IPropertiesContainer container = this as IPropertiesContainer;
			container.BeginPropertiesUpdate();
			try {
				Borders.Merge(properties.Borders);
				if (!UseCellConditionalFormatting && properties.UseCellConditionalFormatting)
					CellConditionalFormatting = properties.CellConditionalFormatting;
				CellMargins.Merge(properties.CellMargins);
				if (!UseBackgroundColor && properties.UseBackgroundColor)
					BackgroundColor = properties.BackgroundColor;
				if (!UseForegroundColor && properties.UseForegroundColor)
					ForegroundColor = properties.ForegroundColor;
				if (!UseShading && properties.UseShading) {
					ShadingPattern = properties.ShadingPattern;
#if THEMES_EDIT
					Shading = properties.Shading;
#endif
				}
				if (!UseFitText && properties.UseFitText)
					FitText = properties.FitText;
				if (!UseHideCellMark && properties.UseHideCellMark)
					HideCellMark = properties.HideCellMark;				
				if (!UseNoWrap && properties.UseNoWrap)
					NoWrap = properties.NoWrap;
				if (!UsePreferredWidth && properties.UsePreferredWidth)
					PreferredWidth.CopyFrom(properties.PreferredWidth);
				if (!UseTextDirection && properties.UseTextDirection)
					TextDirection = properties.TextDirection;
				if (!UseVerticalAlignment && properties.UseVerticalAlignment)
					VerticalAlignment = properties.VerticalAlignment;
			}
			finally {
				container.EndPropertiesUpdate();
			}
		}
		static TableCellPropertiesAccessorTable CreateAccessorTable() {
			TableCellPropertiesAccessorTable result = new TableCellPropertiesAccessorTable();
			result.Add(Properties.CellConditionalFormatting, new BoolPropertyAccessor<TableCellPropertiesOptions>(TableCellPropertiesOptions.GetOptionsUseCellConditionalFormatting, TableCellPropertiesOptions.SetOptionsUseCellConditionalFormatting));
			result.Add(Properties.PreferredWidth, new BoolPropertyAccessor<TableCellPropertiesOptions>(TableCellPropertiesOptions.GetOptionsUsePreferredWidth, TableCellPropertiesOptions.SetOptionsUsePreferredWidth));
			result.Add(Properties.HideCellMark, new BoolPropertyAccessor<TableCellPropertiesOptions>(TableCellPropertiesOptions.GetOptionsUseHideCellMark, TableCellPropertiesOptions.SetOptionsUseHideCellMark));
			result.Add(Properties.NoWrap, new BoolPropertyAccessor<TableCellPropertiesOptions>(TableCellPropertiesOptions.GetOptionsUseNoWrap, TableCellPropertiesOptions.SetOptionsUseNoWrap));
			result.Add(Properties.FitText, new BoolPropertyAccessor<TableCellPropertiesOptions>(TableCellPropertiesOptions.GetOptionsUseFitText, TableCellPropertiesOptions.SetOptionsUseFitText));
			result.Add(Properties.TopMargin, new BoolPropertyAccessor<TableCellPropertiesOptions>(TableCellPropertiesOptions.GetOptionsUseTopMargin, TableCellPropertiesOptions.SetOptionsUseTopMargin));
			result.Add(Properties.BottomMargin, new BoolPropertyAccessor<TableCellPropertiesOptions>(TableCellPropertiesOptions.GetOptionsUseBottomMargin, TableCellPropertiesOptions.SetOptionsUseBottomMargin));
			result.Add(Properties.LeftMargin, new BoolPropertyAccessor<TableCellPropertiesOptions>(TableCellPropertiesOptions.GetOptionsUseLeftMargin, TableCellPropertiesOptions.SetOptionsUseLeftMargin));
			result.Add(Properties.RightMargin, new BoolPropertyAccessor<TableCellPropertiesOptions>(TableCellPropertiesOptions.GetOptionsUseRightMargin, TableCellPropertiesOptions.SetOptionsUseRightMargin));
			result.Add(Properties.TextDirection, new BoolPropertyAccessor<TableCellPropertiesOptions>(TableCellPropertiesOptions.GetOptionsUseTextDirection, TableCellPropertiesOptions.SetOptionsUseTextDirection));
			result.Add(Properties.VerticalAlignment, new BoolPropertyAccessor<TableCellPropertiesOptions>(TableCellPropertiesOptions.GetOptionsUseVerticalAlignment, TableCellPropertiesOptions.SetOptionsUseVerticalAlignment));
			result.Add(Properties.VerticalMerging, new BoolPropertyAccessor<TableCellPropertiesOptions>(TableCellPropertiesOptions.GetOptionsUseVerticalMerging, TableCellPropertiesOptions.SetOptionsUseVerticalMerging));			
			result.Add(Properties.ColumnSpan, new BoolPropertyAccessor<TableCellPropertiesOptions>(TableCellPropertiesOptions.GetOptionsUseColumnSpan, TableCellPropertiesOptions.SetOptionsUseColumnSpan));
			result.Add(Properties.BackgroundColor, new BoolPropertyAccessor<TableCellPropertiesOptions>(TableCellPropertiesOptions.GetOptionsUseBackgroundColor, TableCellPropertiesOptions.SetOptionsUseBackgroundColor));
			result.Add(Properties.ForegroundColor, new BoolPropertyAccessor<TableCellPropertiesOptions>(TableCellPropertiesOptions.GetOptionsUseForegroundColor, TableCellPropertiesOptions.SetOptionsUseForegroundColor));
			result.Add(Properties.ShadingPattern, new BoolPropertyAccessor<TableCellPropertiesOptions>(TableCellPropertiesOptions.GetOptionsUseShading, TableCellPropertiesOptions.SetOptionsUseShading));
#if THEMES_EDIT            
			result.Add(Properties.Shading, new BoolPropertyAccessor<TableCellPropertiesOptions>(TableCellPropertiesOptions.GetOptionsUseShading, TableCellPropertiesOptions.SetOptionsUseShading));
#endif
			result.Add(Properties.LeftBorder, new BoolPropertyAccessor<TableCellPropertiesOptions>(TableCellPropertiesOptions.GetOptionsUseLeftBorder, TableCellPropertiesOptions.SetOptionsUseLeftBorder));
			result.Add(Properties.RightBorder, new BoolPropertyAccessor<TableCellPropertiesOptions>(TableCellPropertiesOptions.GetOptionsUseRightBorder, TableCellPropertiesOptions.SetOptionsUseRightBorder));
			result.Add(Properties.TopBorder, new BoolPropertyAccessor<TableCellPropertiesOptions>(TableCellPropertiesOptions.GetOptionsUseTopBorder, TableCellPropertiesOptions.SetOptionsUseTopBorder));
			result.Add(Properties.BottomBorder, new BoolPropertyAccessor<TableCellPropertiesOptions>(TableCellPropertiesOptions.GetOptionsUseBottomBorder, TableCellPropertiesOptions.SetOptionsUseBottomBorder));
			result.Add(Properties.InsideHorizontalBorder, new BoolPropertyAccessor<TableCellPropertiesOptions>(TableCellPropertiesOptions.GetOptionsUseInsideHorizontalBorder, TableCellPropertiesOptions.SetOptionsUseInsideHorizontalBorder));
			result.Add(Properties.InsideVerticalBorder, new BoolPropertyAccessor<TableCellPropertiesOptions>(TableCellPropertiesOptions.GetOptionsUseInsideVerticalBorder, TableCellPropertiesOptions.SetOptionsUseInsideVerticalBorder));
			result.Add(Properties.TopLeftDiagonalBorder, new BoolPropertyAccessor<TableCellPropertiesOptions>(TableCellPropertiesOptions.GetOptionsUseTopLeftDiagonalBorder, TableCellPropertiesOptions.SetOptionsUseTopLeftDiagonalBorder));
			result.Add(Properties.TopRightDiagonalBorder, new BoolPropertyAccessor<TableCellPropertiesOptions>(TableCellPropertiesOptions.GetOptionsUseTopRightDiagonalBorder, TableCellPropertiesOptions.SetOptionsUseTopRightDiagonalBorder));
			return result;
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected override IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateIndexChangedHistoryItem() {
			return owner.CreateCellPropertiesChangedHistoryItem(this);
		}
	}
	#endregion
	#region CombinedCellBordersInfo
	public class CombinedCellBordersInfo : ICloneable<CombinedCellBordersInfo>, ISupportsCopyFrom<CombinedCellBordersInfo> {
		readonly BorderInfo bottomBorder;
		readonly BorderInfo leftBorder;
		readonly BorderInfo rightBorder;
		readonly BorderInfo topBorder;
		readonly BorderInfo insideHorizontalBorder;
		readonly BorderInfo insideVerticalBorder;
		readonly BorderInfo topLeftDiagonalBorder;
		readonly BorderInfo topRightDiagonalBorder;
		public CombinedCellBordersInfo(TableCellBorders tableBorders) {
			this.bottomBorder = tableBorders.BottomBorder.Info;
			this.leftBorder = tableBorders.LeftBorder.Info;
			this.rightBorder = tableBorders.RightBorder.Info;
			this.topBorder = tableBorders.TopBorder.Info;
			this.insideHorizontalBorder = tableBorders.InsideHorizontalBorder.Info;
			this.insideVerticalBorder = tableBorders.InsideVerticalBorder.Info;
			this.topLeftDiagonalBorder = tableBorders.TopLeftDiagonalBorder.Info;
			this.topRightDiagonalBorder = tableBorders.TopRightDiagonalBorder.Info;
		}
		internal CombinedCellBordersInfo() {
			this.bottomBorder = new BorderInfo();
			this.leftBorder = new BorderInfo();
			this.rightBorder = new BorderInfo();
			this.topBorder = new BorderInfo();
			this.insideHorizontalBorder = new BorderInfo();
			this.insideVerticalBorder = new BorderInfo();
			this.topLeftDiagonalBorder = new BorderInfo();
			this.topRightDiagonalBorder = new BorderInfo();
		}
		public BorderInfo BottomBorder { get { return bottomBorder; } }
		public BorderInfo LeftBorder { get { return leftBorder; } }
		public BorderInfo RightBorder { get { return rightBorder; } }
		public BorderInfo TopBorder { get { return topBorder; } }
		public BorderInfo InsideHorizontalBorder { get { return insideHorizontalBorder; } }
		public BorderInfo InsideVerticalBorder { get { return insideVerticalBorder; } }
		public BorderInfo TopLeftDiagonalBorder { get { return topLeftDiagonalBorder; } }
		public BorderInfo TopRightDiagonalBorder { get { return topRightDiagonalBorder; } }
		public void CopyFrom(CombinedCellBordersInfo info) {
			this.bottomBorder.CopyFrom(info.BottomBorder);
			this.leftBorder.CopyFrom(info.LeftBorder);
			this.rightBorder.CopyFrom(info.RightBorder);
			this.topBorder.CopyFrom(info.TopBorder);
			this.insideHorizontalBorder.CopyFrom(info.InsideHorizontalBorder);
			this.insideVerticalBorder.CopyFrom(info.InsideVerticalBorder);
			this.topLeftDiagonalBorder.CopyFrom(info.TopLeftDiagonalBorder);
			this.topRightDiagonalBorder.CopyFrom(info.TopRightDiagonalBorder);
		}
		public CombinedCellBordersInfo Clone() {
			CombinedCellBordersInfo clone = new CombinedCellBordersInfo();
			clone.CopyFrom(this);
			return clone;
		}
	}
	#endregion
	#region CombinedCellPropertiesInfo
	public class CombinedCellPropertiesInfo : ICloneable<CombinedCellPropertiesInfo>, ISupportsCopyFrom<CombinedCellPropertiesInfo> {
		readonly WidthUnitInfo preferredWidth;
		readonly CombinedCellMarginsInfo cellMargins;
		readonly CombinedCellBordersInfo borders;
		readonly TableCellGeneralSettingsInfo generalSettings;
		public CombinedCellPropertiesInfo(TableCellProperties cellProperties) {
			this.preferredWidth = cellProperties.PreferredWidth.Info;
			this.cellMargins = new CombinedCellMarginsInfo(cellProperties.CellMargins);
			this.borders = new CombinedCellBordersInfo(cellProperties.Borders);
			this.generalSettings = cellProperties.GeneralSettings.Info;
		}
		internal CombinedCellPropertiesInfo() {
			this.preferredWidth = new WidthUnitInfo();
			this.cellMargins = new CombinedCellMarginsInfo();
			this.borders = new CombinedCellBordersInfo();
			this.generalSettings = new TableCellGeneralSettingsInfo();
		}
		public WidthUnitInfo PreferredWidth { get { return preferredWidth; } }
		public CombinedCellMarginsInfo CellMargins { get { return cellMargins; } }
		public CombinedCellBordersInfo Borders { get { return borders; } }
		public TableCellGeneralSettingsInfo GeneralSettings { get { return generalSettings; } }
		public CombinedCellPropertiesInfo Clone() {
			CombinedCellPropertiesInfo clone = new CombinedCellPropertiesInfo();
			clone.CopyFrom(this);
			return clone;
		}
		public void CopyFrom(CombinedCellPropertiesInfo value) {
			PreferredWidth.CopyFrom(value.PreferredWidth);
			CellMargins.CopyFrom(value.CellMargins);
			Borders.CopyFrom(value.Borders);
			GeneralSettings.CopyFrom(value.GeneralSettings);
		}
	}
	#endregion
	#region MergedTableCellProperties
	public class MergedTableCellProperties : MergedProperties<CombinedCellPropertiesInfo, TableCellPropertiesOptions> {
		public MergedTableCellProperties(CombinedCellPropertiesInfo info, TableCellPropertiesOptions options)
			: base(info, options) {
		}
	}
	#endregion
	#region TableCellPropertiesMerger
	public class TableCellPropertiesMerger : PropertiesMergerBase<CombinedCellPropertiesInfo, TableCellPropertiesOptions, MergedTableCellProperties> {
		public TableCellPropertiesMerger(TableCellProperties initialProperties)
			: base(new MergedTableCellProperties(new CombinedCellPropertiesInfo(initialProperties), initialProperties.Info)) {
		}
		public TableCellPropertiesMerger(MergedTableCellProperties initialProperties)
			: base(new MergedTableCellProperties(initialProperties.Info, initialProperties.Options)) {
		}
		public void Merge(TableCellProperties properties) {
			MergeCore(new CombinedCellPropertiesInfo(properties), properties.Info);
		}
		protected internal override void MergeCore(CombinedCellPropertiesInfo info, TableCellPropertiesOptions options) {
			if (!OwnOptions.UseBottomBorder && options.UseBottomBorder) {
				OwnInfo.Borders.BottomBorder.CopyFrom(info.Borders.BottomBorder);
				OwnOptions.UseBottomBorder = true;
			}
			if (!OwnOptions.UseTopBorder && options.UseTopBorder) {
				OwnInfo.Borders.TopBorder.CopyFrom(info.Borders.TopBorder);
				OwnOptions.UseTopBorder = true;
			}
			if (!OwnOptions.UseLeftBorder && options.UseLeftBorder) {
				OwnInfo.Borders.LeftBorder.CopyFrom(info.Borders.LeftBorder);
				OwnOptions.UseLeftBorder = true;
			}
			if (!OwnOptions.UseRightBorder && options.UseRightBorder) {
				OwnInfo.Borders.RightBorder.CopyFrom(info.Borders.RightBorder);
				OwnOptions.UseRightBorder = true;
			}
			if (!OwnOptions.UseInsideHorizontalBorder && options.UseInsideHorizontalBorder) {
				OwnInfo.Borders.InsideHorizontalBorder.CopyFrom(info.Borders.InsideHorizontalBorder);
				OwnOptions.UseInsideHorizontalBorder = true;
			}
			if (!OwnOptions.UseInsideVerticalBorder && options.UseInsideVerticalBorder) {
				OwnInfo.Borders.InsideVerticalBorder.CopyFrom(info.Borders.InsideVerticalBorder);
				OwnOptions.UseInsideVerticalBorder = true;
			}
			if (!OwnOptions.UseTopLeftDiagonalBorder && options.UseTopLeftDiagonalBorder) {
				OwnInfo.Borders.TopLeftDiagonalBorder.CopyFrom(info.Borders.TopLeftDiagonalBorder);
				OwnOptions.UseTopLeftDiagonalBorder = true;
			}
			if (!OwnOptions.UseTopRightDiagonalBorder && options.UseTopRightDiagonalBorder) {
				OwnInfo.Borders.TopRightDiagonalBorder.CopyFrom(info.Borders.TopRightDiagonalBorder);
				OwnOptions.UseTopRightDiagonalBorder = true;
			}
			if (!OwnOptions.UseTopMargin && options.UseTopMargin) {
				OwnInfo.CellMargins.Top.CopyFrom(info.CellMargins.Top);
				OwnOptions.UseTopMargin= true;
			}
			if (!OwnOptions.UseLeftMargin && options.UseLeftMargin) {
				OwnInfo.CellMargins.Left.CopyFrom(info.CellMargins.Left);
				OwnOptions.UseLeftMargin = true;
			}
			if (!OwnOptions.UseRightMargin && options.UseRightMargin) {
				OwnInfo.CellMargins.Right.CopyFrom(info.CellMargins.Right);
				OwnOptions.UseRightMargin = true;
			}
			if (!OwnOptions.UseBottomMargin && options.UseBottomMargin) {
				OwnInfo.CellMargins.Bottom.CopyFrom(info.CellMargins.Bottom);
				OwnOptions.UseBottomMargin = true;
			}
			if (!OwnOptions.UseBackgroundColor && options.UseBackgroundColor) {
				OwnInfo.GeneralSettings.BackgroundColor = info.GeneralSettings.BackgroundColor;
				OwnOptions.UseBackgroundColor = true;
			}
			if (!OwnOptions.UseForegroundColor && options.UseForegroundColor) {
				OwnInfo.GeneralSettings.ForegroundColor = info.GeneralSettings.ForegroundColor;
				OwnOptions.UseForegroundColor = true;
			}
			if (!OwnOptions.UseShading && options.UseShading) {
				OwnInfo.GeneralSettings.ShadingPattern = info.GeneralSettings.ShadingPattern;
#if THEMES_EDIT
				OwnInfo.GeneralSettings.Shading = info.GeneralSettings.Shading;
#endif
				OwnOptions.UseShading = true;
			}
			if (!OwnOptions.UseCellConditionalFormatting && options.UseCellConditionalFormatting) {
				OwnInfo.GeneralSettings.CellConditionalFormatting = info.GeneralSettings.CellConditionalFormatting;
				OwnOptions.UseCellConditionalFormatting = true;
			}
			if (!OwnOptions.UseFitText && options.UseFitText) {
				OwnInfo.GeneralSettings.FitText = info.GeneralSettings.FitText;
				OwnOptions.UseFitText = true;
			}
			if (!OwnOptions.UseHideCellMark && options.UseHideCellMark) {
				OwnInfo.GeneralSettings.HideCellMark = info.GeneralSettings.HideCellMark;
				OwnOptions.UseHideCellMark = true;
			}
			if (!OwnOptions.UseNoWrap && options.UseNoWrap) {
				OwnInfo.GeneralSettings.NoWrap = info.GeneralSettings.NoWrap;
				OwnOptions.UseNoWrap = true;
			}
			if (!OwnOptions.UseTextDirection && options.UseTextDirection) {
				OwnInfo.GeneralSettings.TextDirection = info.GeneralSettings.TextDirection;
				OwnOptions.UseTextDirection = true;
			}
			if (!OwnOptions.UseVerticalAlignment && options.UseVerticalAlignment) {
				OwnInfo.GeneralSettings.VerticalAlignment = info.GeneralSettings.VerticalAlignment;
				OwnOptions.UseVerticalAlignment = true;
			}
			if (!OwnOptions.UsePreferredWidth && options.UsePreferredWidth) {
				OwnInfo.PreferredWidth.CopyFrom(info.PreferredWidth);
				OwnOptions.UsePreferredWidth = true;
			}
		}
	}
	#endregion
}
