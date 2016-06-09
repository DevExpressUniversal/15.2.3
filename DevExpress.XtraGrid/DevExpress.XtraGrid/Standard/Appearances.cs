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
using System.ComponentModel;
using System.Collections;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Controls;
using DevExpress.XtraGrid.Views.Base;
using System.Drawing.Design;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using System.Drawing;
namespace DevExpress.XtraGrid.Views.Grid {
	public class GridViewAppearances : ColumnViewAppearances {
		public GridViewAppearances(BaseView view) : base(view) { }
		AppearanceObject headerPanel, groupPanel, 
			footerPanel, topNewRow, row, rowSeparator, groupRow, evenRow, oddRow, horzLine, vertLine, preview, focusedRow, focusedCell, 
			groupButton, detailTip, groupFooter, empty, selectedRow, hideSelectionRow, columnFilterButton, 
			columnFilterButtonActive, fixedLine, customizationFormHint;
		protected override void CreateAppearances() {
			base.CreateAppearances();
			this.headerPanel = CreateAppearance("HeaderPanel"); 
			this.groupPanel = CreateAppearance("GroupPanel"); 
			this.footerPanel = CreateAppearance("FooterPanel");
			this.row = CreateAppearance("Row"); 
			this.rowSeparator = CreateAppearance("RowSeparator"); 
			this.groupRow = CreateAppearance("GroupRow"); 
			this.topNewRow = CreateAppearance("TopNewRow"); 
			this.evenRow = CreateAppearance("EvenRow"); 
			this.oddRow = CreateAppearance("OddRow"); 
			this.horzLine = CreateAppearance("HorzLine"); 
			this.vertLine = CreateAppearance("VertLine"); 
			this.preview = CreateAppearance("Preview"); 
			this.focusedRow = CreateAppearance("FocusedRow"); 
			this.focusedCell = CreateAppearance("FocusedCell"); 
			this.groupButton = CreateAppearance("GroupButton"); 
			this.detailTip = CreateAppearance("DetailTip"); 
			this.groupFooter = CreateAppearance("GroupFooter"); 
			this.empty = CreateAppearance("Empty"); 
			this.selectedRow = CreateAppearance("SelectedRow"); 
			this.hideSelectionRow = CreateAppearance("HideSelectionRow"); 
			this.columnFilterButton = CreateAppearance("ColumnFilterButton"); 
			this.columnFilterButtonActive = CreateAppearance("ColumnFilterButtonActive"); 
			this.fixedLine = CreateAppearance("FixedLine");
			this.customizationFormHint = CreateAppearance("CustomizationFormHint");
		}
		void ResetHeaderPanel() { HeaderPanel.Reset(); }
		bool ShouldSerializeHeaderPanel() { return HeaderPanel.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewAppearancesHeaderPanel"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject HeaderPanel { get { return headerPanel; } }
		void ResetGroupPanel() { GroupPanel.Reset(); }
		bool ShouldSerializeGroupPanel() { return GroupPanel.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewAppearancesGroupPanel"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject GroupPanel { get { return groupPanel; } }
		void ResetFooterPanel() { FooterPanel.Reset(); }
		bool ShouldSerializeFooterPanel() { return FooterPanel.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewAppearancesFooterPanel"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject FooterPanel { get { return footerPanel; } }
		void ResetRow() { Row.Reset(); }
		bool ShouldSerializeRow() { return Row.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewAppearancesRow"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Row { get { return row; } }
		void ResetTopNewRow() { TopNewRow.Reset(); }
		bool ShouldSerializeTopNewRow() { return TopNewRow.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewAppearancesTopNewRow"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject TopNewRow { get { return topNewRow; } }
		void ResetRowSeparator() { RowSeparator.Reset(); }
		bool ShouldSerializeRowSeparator() { return RowSeparator.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewAppearancesRowSeparator"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject RowSeparator { get { return rowSeparator; } }
		void ResetGroupRow() { GroupRow.Reset(); }
		bool ShouldSerializeGroupRow() { return GroupRow.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewAppearancesGroupRow"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject GroupRow { get { return groupRow; } }
		void ResetEvenRow() { EvenRow.Reset(); }
		bool ShouldSerializeEvenRow() { return EvenRow.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewAppearancesEvenRow"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject EvenRow { get { return evenRow; } }
		void ResetOddRow() { OddRow.Reset(); }
		bool ShouldSerializeOddRow() { return OddRow.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewAppearancesOddRow"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject OddRow { get { return oddRow; } }
		void ResetHorzLine() { HorzLine.Reset(); }
		bool ShouldSerializeHorzLine() { return HorzLine.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewAppearancesHorzLine"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject HorzLine { get { return horzLine; } }
		void ResetVertLine() { VertLine.Reset(); }
		bool ShouldSerializeVertLine() { return VertLine.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewAppearancesVertLine"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject VertLine { get { return vertLine; } }
		void ResetPreview() { Preview.Reset(); }
		bool ShouldSerializePreview() { return Preview.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewAppearancesPreview"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Preview { get { return preview; } }
		void ResetFocusedRow() { FocusedRow.Reset(); }
		bool ShouldSerializeFocusedRow() { return FocusedRow.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewAppearancesFocusedRow"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject FocusedRow { get { return focusedRow; } }
		void ResetFocusedCell() { FocusedCell.Reset(); }
		bool ShouldSerializeFocusedCell() { return FocusedCell.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewAppearancesFocusedCell"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject FocusedCell { get { return focusedCell; } }
		void ResetGroupButton() { GroupButton.Reset(); }
		bool ShouldSerializeGroupButton() { return GroupButton.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewAppearancesGroupButton"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject GroupButton { get { return groupButton; } }
		void ResetDetailTip() { DetailTip.Reset(); }
		bool ShouldSerializeDetailTip() { return DetailTip.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewAppearancesDetailTip"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject DetailTip { get { return detailTip; } }
		void ResetGroupFooter() { GroupFooter.Reset(); }
		bool ShouldSerializeGroupFooter() { return GroupFooter.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewAppearancesGroupFooter"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject GroupFooter { get { return groupFooter; } }
		void ResetEmpty() { Empty.Reset(); }
		bool ShouldSerializeEmpty() { return Empty.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewAppearancesEmpty"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Empty { get { return empty; } }
		void ResetSelectedRow() { SelectedRow.Reset(); }
		bool ShouldSerializeSelectedRow() { return SelectedRow.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewAppearancesSelectedRow"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject SelectedRow { get { return selectedRow; } }
		void ResetHideSelectionRow() { HideSelectionRow.Reset(); }
		bool ShouldSerializeHideSelectionRow() { return HideSelectionRow.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewAppearancesHideSelectionRow"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject HideSelectionRow { get { return hideSelectionRow; } }
		void ResetColumnFilterButton() { ColumnFilterButton.Reset(); }
		bool ShouldSerializeColumnFilterButton() { return ColumnFilterButton.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewAppearancesColumnFilterButton"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ColumnFilterButton { get { return columnFilterButton; } }
		void ResetColumnFilterButtonActive() { ColumnFilterButtonActive.Reset(); }
		bool ShouldSerializeColumnFilterButtonActive() { return ColumnFilterButtonActive.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewAppearancesColumnFilterButtonActive"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ColumnFilterButtonActive { get { return columnFilterButtonActive; } }
		void ResetFixedLine() { FixedLine.Reset(); }
		bool ShouldSerializeFixedLine() { return FixedLine.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewAppearancesFixedLine"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject FixedLine { get { return fixedLine; } }
		void ResetCustomizationFormHint() { CustomizationFormHint.Reset(); }
		bool ShouldSerializeCustomizationFormHint() { return CustomizationFormHint.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewAppearancesCustomizationFormHint"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject CustomizationFormHint { get { return customizationFormHint; } }
		public AppearanceObject GetIndicatorAppearance() {
			AppearanceObject ret = HeaderPanel.Clone() as AppearanceObject;
			if(View.ElementsLookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) {
				SkinElement element = GridSkins.GetSkin(View.ElementsLookAndFeel)[GridSkins.SkinIndicator];
				if(element != null && element.Color.ForeColor != Color.Empty)
					ret.ForeColor = element.Color.ForeColor;
			}
			return ret;
		}
	}
	public class GridViewPrintAppearances : ColumnViewPrintAppearances {
		public GridViewPrintAppearances(BaseView view) : base(view) { }
		AppearanceObject headerPanel,  footerPanel, row, evenRow, oddRow, groupRow, lines, preview, groupFooter;
		protected override void CreateAppearances() {
			base.CreateAppearances();
			this.headerPanel = CreateAppearance("HeaderPanel"); 
			this.footerPanel = CreateAppearance("FooterPanel"); 
			this.row = CreateAppearance("Row"); 
			this.evenRow = CreateAppearance("EvenRow"); 
			this.oddRow = CreateAppearance("OddRow"); 
			this.groupRow = CreateAppearance("GroupRow"); 
			this.lines = CreateAppearance("Lines"); 
			this.preview = CreateAppearance("Preview"); 
			this.groupFooter = CreateAppearance("GroupFooter"); 
		}
		void ResetHeaderPanel() { HeaderPanel.Reset(); }
		bool ShouldSerializeHeaderPanel() { return HeaderPanel.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewPrintAppearancesHeaderPanel"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject HeaderPanel { get { return headerPanel; } }
		void ResetFooterPanel() { FooterPanel.Reset(); }
		bool ShouldSerializeFooterPanel() { return FooterPanel.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewPrintAppearancesFooterPanel"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject FooterPanel { get { return footerPanel; } }
		void ResetRow() { Row.Reset(); }
		bool ShouldSerializeRow() { return Row.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewPrintAppearancesRow"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Row { get { return row; } }
		void ResetOddRow() { OddRow.Reset(); }
		bool ShouldSerializeOddRow() { return OddRow.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewPrintAppearancesOddRow"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject OddRow { get { return oddRow; } }
		void ResetEvenRow() { EvenRow.Reset(); }
		bool ShouldSerializeEvenRow() { return EvenRow.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewPrintAppearancesEvenRow"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject EvenRow { get { return evenRow; } }
		void ResetGroupRow() { GroupRow.Reset(); }
		bool ShouldSerializeGroupRow() { return GroupRow.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewPrintAppearancesGroupRow"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject GroupRow { get { return groupRow; } }
		void ResetLines() { Lines.Reset(); }
		bool ShouldSerializeLines() { return Lines.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewPrintAppearancesLines"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Lines { get { return lines; } }
		void ResetPreview() { Preview.Reset(); }
		bool ShouldSerializePreview() { return Preview.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewPrintAppearancesPreview"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Preview { get { return preview; } }
		void ResetGroupFooter() { GroupFooter.Reset(); }
		bool ShouldSerializeGroupFooter() { return GroupFooter.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridViewPrintAppearancesGroupFooter"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject GroupFooter { get { return groupFooter; } }
	}
}
