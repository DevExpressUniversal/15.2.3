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
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.Printing;
using DevExpress.XtraTreeList.ViewInfo;
using DevExpress.Utils;
using System.ComponentModel;
namespace DevExpress.XtraTreeList {
	public class TreeListBaseAppearanceCollection : BaseAppearanceCollection {
		IAppearanceOwner treeList;
		protected TreeListBaseAppearanceCollection(IAppearanceOwner treeList) {
			this.treeList = treeList;
		}
		public override bool IsLoading { get { return treeList.IsLoading; } }
		protected override AppearanceObject CreateNullAppearance() { return null; }
	}
	public class TreeListAppearanceCollection : TreeListBaseAppearanceCollection {
		AppearanceObject headerPanel, bandPanel, headerPanelBackground, footerPanel, row, evenRow, oddRow, horzLine, 
			vertLine, preview, focusedRow, focusedCell, groupButton, treeLine,
			groupFooter, empty, selectedRow, hideSelectionRow, fixedLine, customizationFormHint,
			filterPanel, caption;
		public TreeListAppearanceCollection(IAppearanceOwner treeList) : base(treeList) {}
		protected override void CreateAppearances() {
			this.headerPanel = CreateAppearance(AppearanceName.HeaderPanel);
			this.bandPanel = CreateAppearance(AppearanceName.BandPanel);
			this.headerPanelBackground = CreateAppearance(AppearanceName.HeaderPanelBackground);
			this.footerPanel = CreateAppearance(AppearanceName.FooterPanel);
			this.row = CreateAppearance(AppearanceName.Row);
			this.evenRow = CreateAppearance(AppearanceName.EvenRow);
			this.oddRow = CreateAppearance(AppearanceName.OddRow);
			this.horzLine = CreateAppearance(AppearanceName.HorzLine);
			this.vertLine = CreateAppearance(AppearanceName.VertLine);
			this.preview = CreateAppearance(AppearanceName.Preview);
			this.focusedRow = CreateAppearance(AppearanceName.FocusedRow);
			this.focusedCell = CreateAppearance(AppearanceName.FocusedCell);
			this.groupButton = CreateAppearance(AppearanceName.GroupButton);
			this.treeLine = CreateAppearance(AppearanceName.TreeLine);
			this.groupFooter = CreateAppearance(AppearanceName.GroupFooter);
			this.empty = CreateAppearance(AppearanceName.Empty);
			this.selectedRow = CreateAppearance(AppearanceName.SelectedRow);
			this.hideSelectionRow = CreateAppearance(AppearanceName.HideSelectionRow);
			this.fixedLine = CreateAppearance(AppearanceName.FixedLine);
			this.customizationFormHint = CreateAppearance(AppearanceName.CustomizationFormHint);
			this.filterPanel = CreateAppearance(AppearanceName.FilterPanel);
			this.caption = CreateAppearance(AppearanceName.Caption);
		}
		void ResetHeaderPanel() { HeaderPanel.Reset(); }
		bool ShouldSerializeHeaderPanel() { return HeaderPanel.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListAppearanceCollectionHeaderPanel"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject HeaderPanel { get { return headerPanel; } }
		void ResetHeaderPanelBackground() { HeaderPanelBackground.Reset(); }
		bool ShouldSerializeHeaderPanelBackground() { return HeaderPanelBackground.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListAppearanceCollectionHeaderPanelBackground"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject HeaderPanelBackground { get { return headerPanelBackground; } }
		void ResetBandPanel() { BandPanel.Reset(); }
		bool ShouldSerializeBandPanel() { return BandPanel.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListAppearanceCollectionBandPanel"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject BandPanel { get { return bandPanel; } }
		void ResetFooterPanel() { FooterPanel.Reset(); }
		bool ShouldSerializeFooterPanel() { return FooterPanel.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListAppearanceCollectionFooterPanel"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject FooterPanel { get { return footerPanel; } }
		void ResetRow() { Row.Reset(); }
		bool ShouldSerializeRow() { return Row.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListAppearanceCollectionRow"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Row { get { return row; } }
		void ResetEvenRow() { EvenRow.Reset(); }
		bool ShouldSerializeEvenRow() { return EvenRow.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListAppearanceCollectionEvenRow"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject EvenRow { get { return evenRow; } }
		void ResetOddRow() { OddRow.Reset(); }
		bool ShouldSerializeOddRow() { return OddRow.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListAppearanceCollectionOddRow"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject OddRow { get { return oddRow; } }
		void ResetHorzLine() { HorzLine.Reset(); }
		bool ShouldSerializeHorzLine() { return HorzLine.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListAppearanceCollectionHorzLine"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject HorzLine { get { return horzLine; } }
		void ResetVertLine() { VertLine.Reset(); }
		bool ShouldSerializeVertLine() { return VertLine.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListAppearanceCollectionVertLine"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject VertLine { get { return vertLine; } }
		void ResetPreview() { Preview.Reset(); }
		bool ShouldSerializePreview() { return Preview.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListAppearanceCollectionPreview"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Preview { get { return preview; } }
		void ResetFocusedRow() { FocusedRow.Reset(); }
		bool ShouldSerializeFocusedRow() { return FocusedRow.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListAppearanceCollectionFocusedRow"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject FocusedRow { get { return focusedRow; } }
		void ResetFocusedCell() { FocusedCell.Reset(); }
		bool ShouldSerializeFocusedCell() { return FocusedCell.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListAppearanceCollectionFocusedCell"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject FocusedCell { get { return focusedCell; } }
		void ResetGroupButton() { GroupButton.Reset(); }
		bool ShouldSerializeGroupButton() { return GroupButton.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListAppearanceCollectionGroupButton"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject GroupButton { get { return groupButton; } }
		void ResetTreeLine() { TreeLine.Reset(); }
		bool ShouldSerializeTreeLine() { return TreeLine.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListAppearanceCollectionTreeLine"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject TreeLine { get { return treeLine; } }
		void ResetGroupFooter() { GroupFooter.Reset(); }
		bool ShouldSerializeGroupFooter() { return GroupFooter.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListAppearanceCollectionGroupFooter"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject GroupFooter { get { return groupFooter; } }
		void ResetEmpty() { Empty.Reset(); }
		bool ShouldSerializeEmpty() { return Empty.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListAppearanceCollectionEmpty"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Empty { get { return empty; } }
		void ResetSelectedRow() { SelectedRow.Reset(); }
		bool ShouldSerializeSelectedRow() { return SelectedRow.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListAppearanceCollectionSelectedRow"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject SelectedRow { get { return selectedRow; } }
		void ResetHideSelectionRow() { HideSelectionRow.Reset(); }
		bool ShouldSerializeHideSelectionRow() { return HideSelectionRow.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListAppearanceCollectionHideSelectionRow"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject HideSelectionRow { get { return hideSelectionRow; } }
		void ResetFixedLine() { FixedLine.Reset(); }
		bool ShouldSerializeFixedLine() { return FixedLine.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject FixedLine { get { return fixedLine; } }
		void ResetCustomizationFormHint() { CustomizationFormHint.Reset(); }
		bool ShouldSerializeCustomizationFormHint() { return CustomizationFormHint.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject CustomizationFormHint { get { return customizationFormHint; } }
		void ResetFilterPanel() { FilterPanel.Reset(); }
		bool ShouldSerializeFilterPanel() { return FilterPanel.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListAppearanceCollectionFilterPanel"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject FilterPanel { get { return filterPanel; } }
		void ResetCaption() { Caption.Reset(); }
		bool ShouldSerializeCaption() { return Caption.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListAppearanceCollectionCaption"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Caption { get { return caption; } }
	}
	public class TreeListPrintAppearanceCollection : TreeListBaseAppearanceCollection {
		AppearanceObject headerPanel, bandPanel, row, evenRow, oddRow, lines, preview, footerPanel, groupFooter, caption;
		public TreeListPrintAppearanceCollection(IAppearanceOwner treeList) : base(treeList) {}
		protected override void CreateAppearances() {
			this.headerPanel = CreateAppearance(TreeListPrinter.DefaultStyleNames[PrintStyleId.HeaderPanel]);
			this.bandPanel = CreateAppearance(TreeListPrinter.DefaultStyleNames[PrintStyleId.BandPanel]);
			this.row = CreateAppearance(TreeListPrinter.DefaultStyleNames[PrintStyleId.Row]);
			this.lines = CreateAppearance(TreeListPrinter.DefaultStyleNames[PrintStyleId.Lines]);
			this.preview = CreateAppearance(TreeListPrinter.DefaultStyleNames[PrintStyleId.Preview]);
			this.footerPanel = CreateAppearance(TreeListPrinter.DefaultStyleNames[PrintStyleId.FooterPanel]);
			this.groupFooter = CreateAppearance(TreeListPrinter.DefaultStyleNames[PrintStyleId.GroupFooter]);
			this.evenRow = CreateAppearance(TreeListPrinter.DefaultStyleNames[PrintStyleId.EvenRow]);
			this.oddRow = CreateAppearance(TreeListPrinter.DefaultStyleNames[PrintStyleId.OddRow]);
			this.caption = CreateAppearance(TreeListPrinter.DefaultStyleNames[PrintStyleId.Caption]);
		}
		protected override AppearanceObject CreateAppearanceInstance(AppearanceObject parent, string name) {
			return new AppearanceObjectPrint(this, parent, name);
		}
		void ResetHeaderPanel() { HeaderPanel.Reset(); }
		bool ShouldSerializeHeaderPanel() { return HeaderPanel.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListPrintAppearanceCollectionHeaderPanel"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject HeaderPanel { get { return headerPanel; } }
		void ResetBandPanel() { BandPanel.Reset(); }
		bool ShouldSerializeBandPanel() { return BandPanel.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListPrintAppearanceCollectionBandPanel"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject BandPanel { get { return bandPanel; } }
		void ResetRow() { Row.Reset(); }
		bool ShouldSerializeRow() { return Row.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListPrintAppearanceCollectionRow"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Row { get { return row; } }
		void ResetLines() { Lines.Reset(); }
		bool ShouldSerializeLines() { return Lines.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListPrintAppearanceCollectionLines"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Lines { get { return lines; } }
		void ResetPreview() { Preview.Reset(); }
		bool ShouldSerializePreview() { return Preview.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListPrintAppearanceCollectionPreview"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Preview { get { return preview; } }
		void ResetFooterPanel() { FooterPanel.Reset(); }
		bool ShouldSerializeFooterPanel() { return FooterPanel.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListPrintAppearanceCollectionFooterPanel"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject FooterPanel { get { return footerPanel; } }
		void ResetGroupFooter() { GroupFooter.Reset(); }
		bool ShouldSerializeGroupFooter() { return GroupFooter.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListPrintAppearanceCollectionGroupFooter"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject GroupFooter { get { return groupFooter; } }
		void ResetEvenRow() { EvenRow.Reset(); }
		bool ShouldSerializeEvenRow() { return EvenRow.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListPrintAppearanceCollectionEvenRow"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject EvenRow { get { return evenRow; } }
		void ResetOddRow() { OddRow.Reset(); }
		bool ShouldSerializeOddRow() { return OddRow.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListPrintAppearanceCollectionOddRow"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject OddRow { get { return oddRow; } }
		void ResetCaption() { Caption.Reset(); }
		bool ShouldSerializeCaption() { return Caption.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListPrintAppearanceCollectionCaption"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Caption { get { return caption; } }
	}
}
