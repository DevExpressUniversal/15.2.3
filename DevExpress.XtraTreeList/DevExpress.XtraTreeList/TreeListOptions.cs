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
using DevExpress.Utils.Controls;
using DevExpress.Utils.Serializing;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraTreeList.Localization;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraExport.Helpers;
using DevExpress.Export;
namespace DevExpress.XtraTreeList {
	public class BaseTreeListOptions : BaseOptions {
		protected BaseTreeListOptions() {}
		internal event BaseOptionChangedEventHandler Changed {
			add { base.ChangedCore += value; }
			remove { base.ChangedCore -= value; }
		}
		protected void BooleanChanged(string name, bool value) {
			OnChanged(new BaseOptionChangedEventArgs(name, !value, value));
		}
		protected internal bool ShouldSerializeCore(IComponent owner) { return ShouldSerialize(owner); }
	}
	public class TreeListOptionsView : BaseTreeListOptions {
		bool autoCalcPreviewLineCount, autoWidth, showButtons, showColumns, showSummaryFooter,
			showRowFooterSummary, showHorzLines, showIndicator, showPreview, showRoot, showVertLines,
			showIndentAsRowStyle, enableAppearanceOddRow, enableAppearanceEvenRow, showCheckBoxes, expandButtonCentered, showAutoFilterRow, allowHtmlDrawHeaders, allowGlyphSkinning, showBands, allowBandColumnsMultiRow;
		ShowFilterPanelMode showFilterPanelMode;
		FilterButtonShowMode headerFilterButtonShowMode;
		TreeListAnimationType animationType;
		DrawFocusRectStyle focusRectStyle;
		DefaultBoolean columnHeaderAutoHeight, showBandsMode;
		bool showCaption;
		public TreeListOptionsView() {
			this.autoWidth = this.showColumns = this.showButtons = this.showHorzLines  = this.showVertLines =
				this.showIndicator = this.showRoot = this.expandButtonCentered = true;
			this.autoCalcPreviewLineCount = this.showSummaryFooter = this.showRowFooterSummary =
				this.showPreview = this.showIndentAsRowStyle = this.enableAppearanceOddRow =
				this.enableAppearanceEvenRow = this.showCheckBoxes = this.showAutoFilterRow = this.allowHtmlDrawHeaders = this.allowGlyphSkinning = this.showBands = this.allowBandColumnsMultiRow = this.showCaption = false;
			this.showFilterPanelMode = ShowFilterPanelMode.Default;
			this.headerFilterButtonShowMode = FilterButtonShowMode.Default;
			this.animationType = TreeListAnimationType.Default;
			this.focusRectStyle = DrawFocusRectStyle.CellFocus;
			this.columnHeaderAutoHeight = DefaultBoolean.Default;
			this.showBandsMode = DefaultBoolean.Default;
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsViewAutoWidth"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AutoWidth {
			get { return autoWidth; }
			set {
				if(AutoWidth == value) return;
				autoWidth = value;
				BooleanChanged(AutoWidthName, AutoWidth);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsViewShowColumns"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowColumns {
			get { return showColumns; }
			set {
				if(ShowColumns == value) return;
				showColumns = value;
				BooleanChanged(ShowColumnsName, ShowColumns);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsViewShowButtons"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowButtons {
			get { return showButtons; }
			set {
				if(ShowButtons == value) return;
				showButtons = value;
				BooleanChanged(ShowButtonsName, ShowButtons);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsViewShowHorzLines"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowHorzLines {
			get { return showHorzLines; }
			set {
				if(ShowHorzLines == value) return;
				showHorzLines = value;
				BooleanChanged(ShowHorzLinesName, ShowHorzLines);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsViewShowVertLines"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowVertLines {
			get { return showVertLines; }
			set {
				if(ShowVertLines == value) return;
				showVertLines = value;
				BooleanChanged(ShowVertLinesName, ShowVertLines);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsViewShowIndicator"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowIndicator {
			get { return showIndicator; }
			set {
				if(ShowIndicator == value) return;
				showIndicator = value;
				BooleanChanged(ShowIndicatorName, ShowIndicator);
			}
		}
		[Obsolete("Use the FocusRectStyle property instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool ShowFocusedFrame {
			get { return FocusRectStyle != DrawFocusRectStyle.None; }
			set { FocusRectStyle = (value ? DrawFocusRectStyle.CellFocus : DrawFocusRectStyle.None); }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsViewFocusRectStyle"),
#endif
 DefaultValue(DrawFocusRectStyle.CellFocus), XtraSerializableProperty()]
		public virtual DrawFocusRectStyle FocusRectStyle {
			get { return focusRectStyle; }
			set {
				if(FocusRectStyle == value) return;
				DrawFocusRectStyle prevValue = FocusRectStyle;
				focusRectStyle = value;
				OnChanged(new BaseOptionChangedEventArgs(FocusRectStyleName, prevValue, FocusRectStyle));
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsViewShowRoot"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowRoot {
			get { return showRoot; }
			set {
				if(ShowRoot == value) return;
				showRoot = value;
				BooleanChanged(ShowRootName, ShowRoot);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsViewAutoCalcPreviewLineCount"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool AutoCalcPreviewLineCount {
			get { return autoCalcPreviewLineCount; }
			set {
				if(AutoCalcPreviewLineCount == value) return;
				autoCalcPreviewLineCount = value;
				BooleanChanged(AutoCalcPreviewLineCountName, AutoCalcPreviewLineCount);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsViewShowSummaryFooter"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool ShowSummaryFooter {
			get { return showSummaryFooter; }
			set {
				if(ShowSummaryFooter == value) return;
				showSummaryFooter = value;
				BooleanChanged(ShowSummaryFooterName, ShowSummaryFooter);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsViewShowRowFooterSummary"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool ShowRowFooterSummary {
			get { return showRowFooterSummary; }
			set {
				if(ShowRowFooterSummary == value) return;
				showRowFooterSummary = value;
				BooleanChanged(ShowRowFooterSummaryName, ShowRowFooterSummary);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsViewShowPreview"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool ShowPreview {
			get { return showPreview; }
			set {
				if(ShowPreview == value) return;
				showPreview = value;
				BooleanChanged(ShowPreviewName, ShowPreview);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsViewShowIndentAsRowStyle"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool ShowIndentAsRowStyle {
			get { return showIndentAsRowStyle; }
			set {
				if(ShowIndentAsRowStyle == value) return;
				showIndentAsRowStyle = value;
				BooleanChanged(ShowIndentAsRowStyleName, ShowIndentAsRowStyle);
			}
		}
		[ DefaultValue(false), XtraSerializableProperty()]
		public virtual bool ShowCheckBoxes {
			get { return showCheckBoxes; }
			set {
				if(ShowCheckBoxes == value) return;
				showCheckBoxes = value;
				BooleanChanged(ShowCheckBoxesName, ShowCheckBoxes);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsViewEnableAppearanceOddRow"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool EnableAppearanceOddRow {
			get { return enableAppearanceOddRow; }
			set {
				if(EnableAppearanceOddRow == value) return;
				enableAppearanceOddRow = value;
				BooleanChanged(EnableAppearanceOddRowName, EnableAppearanceOddRow);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsViewEnableAppearanceEvenRow"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool EnableAppearanceEvenRow {
			get { return enableAppearanceEvenRow; }
			set {
				if(EnableAppearanceEvenRow == value) return;
				enableAppearanceEvenRow = value;
				BooleanChanged(EnableAppearanceEvenRowName, EnableAppearanceEvenRow);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsViewExpandButtonCentered"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ExpandButtonCentered {
			get { return expandButtonCentered; }
			set {
				if(ExpandButtonCentered == value) return;
				expandButtonCentered = value;
				BooleanChanged(ExpandButtonCenteredName, ExpandButtonCentered);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsViewShowAutoFilterRow"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool ShowAutoFilterRow {
			get { return showAutoFilterRow; }
			set {
				if(ShowAutoFilterRow == value) return;
				showAutoFilterRow = value;
				BooleanChanged(ShowAutoFilterRowName, ShowAutoFilterRow);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsViewShowFilterPanelMode"),
#endif
 DefaultValue(ShowFilterPanelMode.Default), XtraSerializableProperty()]
		public ShowFilterPanelMode ShowFilterPanelMode {
			get { return showFilterPanelMode; }
			set {
				if(ShowFilterPanelMode == value) return;
				ShowFilterPanelMode prevValue = ShowFilterPanelMode;
				showFilterPanelMode = value;
				OnChanged(new BaseOptionChangedEventArgs(ShowFilterPanelModeName, prevValue, ShowFilterPanelMode));
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsViewAllowHtmlDrawHeaders"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool AllowHtmlDrawHeaders {
			get { return allowHtmlDrawHeaders; }
			set {
				if(AllowHtmlDrawHeaders == value) return;
				bool prevValue = AllowHtmlDrawHeaders;
				allowHtmlDrawHeaders = value;
				BooleanChanged(AllowHtmlDrawHeadersName, AllowHtmlDrawHeaders);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsViewHeaderFilterButtonShowMode"),
#endif
 DefaultValue(FilterButtonShowMode.Default), XtraSerializableProperty()]
		public virtual FilterButtonShowMode HeaderFilterButtonShowMode {
			get { return headerFilterButtonShowMode; }
			set {
				if(HeaderFilterButtonShowMode == value) return;
				FilterButtonShowMode prevValue = HeaderFilterButtonShowMode;
				headerFilterButtonShowMode = value;
				OnChanged(new BaseOptionChangedEventArgs(HeaderFilterButtonShowModeName, prevValue, HeaderFilterButtonShowMode));
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsViewAllowGlyphSkinning"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool AllowGlyphSkinning {
			get { return allowGlyphSkinning; }
			set {
				if(AllowGlyphSkinning == value) return;
				allowGlyphSkinning = value;
				BooleanChanged(AllowGlyphSkinningName, AllowGlyphSkinning);
			}
		}
		[ DefaultValue(TreeListAnimationType.Default), XtraSerializableProperty()]
		public TreeListAnimationType AnimationType {
			get { return animationType; }
			set {
				if(AnimationType == value) return;
				object prevValue = AnimationType;
				animationType = value;
				OnChanged(new BaseOptionChangedEventArgs(AnimationTypeName, prevValue, AnimationType));
			}
		}
		[ DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public virtual DefaultBoolean ColumnHeaderAutoHeight {
			get { return columnHeaderAutoHeight; }
			set {
				if(ColumnHeaderAutoHeight == value) return;
				DefaultBoolean prevValue = ColumnHeaderAutoHeight;
				columnHeaderAutoHeight = value;
				OnChanged(new BaseOptionChangedEventArgs(ColumnHeaderAutoHeightName, prevValue, ColumnHeaderAutoHeight));
			}
		}
		[Obsolete("Use the ShowBandsMode property instead"), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShowBands {
			get { return ShowBandsMode == DefaultBoolean.True; }
			set { ShowBandsMode = value ? DefaultBoolean.True : DefaultBoolean.False; }
		}
		[ DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public virtual DefaultBoolean ShowBandsMode {
			get { return showBandsMode; }
			set {
				if(ShowBandsMode == value) return;
				DefaultBoolean prevValue = ShowBandsMode;
				showBandsMode = value;
				OnChanged(new BaseOptionChangedEventArgs(ShowBandsModeName, prevValue, ShowBandsMode));
			}
		}
		[ DefaultValue(false), XtraSerializableProperty()]
		public virtual bool AllowBandColumnsMultiRow {
			get { return allowBandColumnsMultiRow; }
			set {
				if(AllowBandColumnsMultiRow == value) return;
				allowBandColumnsMultiRow = value;
				BooleanChanged(AllowBandColumnsMultiRowName, AllowBandColumnsMultiRow);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsViewShowCaption"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool ShowCaption {
			get { return showCaption; }
			set {
				if(ShowCaption == value) return;
				showCaption = value;
				BooleanChanged(ShowCaptionName, ShowCaption);
			}
		}
		protected internal const string
			AutoWidthName = "AutoWidth",
			ShowColumnsName = "ShowColumns",
			ShowButtonsName = "ShowButtons",
			ShowHorzLinesName = "ShowHorzLines",
			ShowVertLinesName = "ShowVertLines",
			ShowIndicatorName = "ShowIndicator",
			ShowRootName = "ShowRoot",
			AutoCalcPreviewLineCountName = "AutoCalcPreviewLineCount",
			ShowSummaryFooterName = "ShowSummaryFooter",
			ShowRowFooterSummaryName = "ShowRowFooterSummary",
			ShowPreviewName = "ShowPreview",
			ShowIndentAsRowStyleName = "ShowIndentAsRowStyle",
			ShowCheckBoxesName = "ShowCheckBoxes",
			EnableAppearanceOddRowName = "EnableAppearanceOddRow",
			EnableAppearanceEvenRowName = "EnableAppearanceEvenRow",
			ExpandButtonCenteredName = "ExpandButtonCentered",
			ShowAutoFilterRowName = "ShowAutoFilterRow",
			ShowFilterPanelModeName = "ShowFilterPanelMode",
			AllowHtmlDrawHeadersName = "AllowHtmlDrawHeaders",
			HeaderFilterButtonShowModeName = "HeaderFilterButtonShowMode",
			AllowGlyphSkinningName = "AllowGlyphSkinning",
			AnimationTypeName = "AnimationType",
			FocusRectStyleName = "FocusRectStyle",
			ColumnHeaderAutoHeightName = "ColumnHeaderAutoHeight",
			ShowBandsModeName = "ShowBandsMode",
			AllowBandColumnsMultiRowName = "AllowBandColumnsMultiRow",
			ShowCaptionName = "ShowCaption";
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				TreeListOptionsView opt = options as TreeListOptionsView;
				if(opt == null) return;
				this.autoWidth = opt.AutoWidth;
				this.showColumns = opt.ShowColumns;
				this.showButtons = opt.ShowButtons;
				this.showHorzLines  = opt.ShowHorzLines;
				this.showVertLines = opt.ShowVertLines;
				this.showIndicator = opt.ShowIndicator;
				this.focusRectStyle = opt.FocusRectStyle;
				this.showRoot = opt.ShowRoot;
				this.showCheckBoxes = opt.ShowCheckBoxes;
				this.autoCalcPreviewLineCount = opt.AutoCalcPreviewLineCount;
				this.showSummaryFooter = opt.ShowSummaryFooter;
				this.showRowFooterSummary = opt.ShowRowFooterSummary;
				this.showPreview = opt.ShowPreview;
				this.expandButtonCentered = opt.ExpandButtonCentered;
				this.showIndentAsRowStyle = opt.ShowIndentAsRowStyle;
				this.enableAppearanceOddRow = opt.EnableAppearanceOddRow;
				this.enableAppearanceEvenRow = opt.EnableAppearanceEvenRow;
				this.allowHtmlDrawHeaders = opt.AllowHtmlDrawHeaders;
				this.allowGlyphSkinning = opt.AllowGlyphSkinning;
				this.animationType = opt.AnimationType;
				this.showAutoFilterRow = opt.ShowAutoFilterRow;
				this.columnHeaderAutoHeight = opt.ColumnHeaderAutoHeight;
				this.showBandsMode = opt.ShowBandsMode;
				this.allowBandColumnsMultiRow = opt.AllowBandColumnsMultiRow;
				this.showCaption = opt.ShowCaption;
			}
			finally {
				EndUpdate();
			}
		}
	}
	public class TreeListOptionsClipboard : ClipboardOptions {
		DefaultBoolean allowCopy, copyNodeHierarchy;
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsClipboardAllowCopy"),
#endif
 DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public virtual DefaultBoolean AllowCopy {
			get { return allowCopy; }
			set {
				if(AllowCopy == value) return;
				allowCopy = value;
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsClipboardCopyNodeHierarchy"),
#endif
 DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public virtual DefaultBoolean CopyNodeHierarchy {
			get { return copyNodeHierarchy; }
			set {
				if(CopyNodeHierarchy == value) return;
				copyNodeHierarchy = value;
			}
		}
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			TreeListOptionsClipboard op = options as TreeListOptionsClipboard;
			if(op == null) return;
			copyNodeHierarchy = op.CopyNodeHierarchy;
			allowCopy = op.AllowCopy;
		}
	}
	public class TreeListOptionsDragAndDrop : BaseTreeListOptions {
		bool expandNodeOnDragCore, canCloneNodesOnDrop, acceptOuterNodesCore;
		int dragNodesExpandDelayCore;		
		DropNodesMode dropNodesModeCore;
		DragNodesMode dragNodesModeCore;
		protected internal const string
			ExpandNodeOnDragName = "ExpandNodeOnDrag",
			DropNodesModeName = "DropNodesMode",
			DragNodesModeName = "DragNodesMode",
			DragNodesExpandDelayName = "DragExpandDelay",
			AcceptOuterNodesName = "AcceptOuterNodes",
			CanCloneNodesOnDropName = "CanCloneNodesOnDrop";
		public TreeListOptionsDragAndDrop() {
			expandNodeOnDragCore = true;
			dragNodesModeCore = XtraTreeList.DragNodesMode.None;
			dragNodesExpandDelayCore = 1000;
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsDragAndDropDropNodesMode"),
#endif
 DefaultValue(DropNodesMode.Default), XtraSerializableProperty()]
		public virtual DropNodesMode DropNodesMode {
			get { return dropNodesModeCore; }
			set {
				if(DropNodesMode == value) return;
				DropNodesMode oldValue = DropNodesMode;
				dropNodesModeCore = value;
				OnChanged(new BaseOptionChangedEventArgs(DropNodesModeName, oldValue, value));
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsDragAndDropDragNodesMode"),
#endif
 DefaultValue(DragNodesMode.None), XtraSerializableProperty()]
		public virtual DragNodesMode DragNodesMode {
			get { return dragNodesModeCore; }
			set {
				if(DragNodesMode == value) return;
				DragNodesMode oldValue = DragNodesMode;
				dragNodesModeCore = value;
				OnChanged(new BaseOptionChangedEventArgs(DragNodesModeName, oldValue, value));
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsDragAndDropCanCloneNodesOnDrop"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool CanCloneNodesOnDrop {
			get { return canCloneNodesOnDrop; }
			set {
				if(CanCloneNodesOnDrop == value) return;
				canCloneNodesOnDrop = value;
				BooleanChanged(CanCloneNodesOnDropName, CanCloneNodesOnDrop);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsDragAndDropDragNodesExpandDelay"),
#endif
 DefaultValue(1000), Localizable(true), XtraSerializableProperty()]
		public int DragNodesExpandDelay {
			get { return dragNodesExpandDelayCore; }
			set {
				if(value < 5) value = 5;
				if(value > 4000) value = 4000;
				OnChanged(new BaseOptionChangedEventArgs(DragNodesExpandDelayName, DragNodesExpandDelay, value));
				dragNodesExpandDelayCore = value;
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsDragAndDropExpandNodeOnDrag"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ExpandNodeOnDrag {
			get { return expandNodeOnDragCore; }
			set {
				if(ExpandNodeOnDrag == value) return;
				expandNodeOnDragCore = value;
				BooleanChanged(ExpandNodeOnDragName, ExpandNodeOnDrag);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsDragAndDropAcceptOuterNodes"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool AcceptOuterNodes {
			get { return acceptOuterNodesCore; }
			set {
				if(AcceptOuterNodes == value) return;
				acceptOuterNodesCore = value;
				BooleanChanged(AcceptOuterNodesName, AcceptOuterNodes);
			}
		}
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			TreeListOptionsDragAndDrop op = options as TreeListOptionsDragAndDrop;
			if(op == null) return;
			expandNodeOnDragCore = op.ExpandNodeOnDrag;
			dropNodesModeCore = op.DropNodesMode;
			dragNodesExpandDelayCore = op.DragNodesExpandDelay;
			dragNodesModeCore = op.DragNodesMode;
			acceptOuterNodesCore = op.AcceptOuterNodes;
			canCloneNodesOnDrop = op.CanCloneNodesOnDrop;
		}
	}
	public class TreeListOptionsBehavior : BaseTreeListOptions {
		bool editable, populateServiceColumns,
			showToolTips, resizeNodes, autoSelectAllInEditor,
			showEditorOnMouseUp, autoNodeHeight, autoChangeParent, closeEditorOnLostFocus, 
			keepSelectedOnClick, smartMouseHover, allowExpandOnDblClick,
			immediateEditor, autoPopulateColumns, allowIncrementalSearch, expandNodesOnIncrementalSearch, enableFiltering, allowIndeterminateCheckState, allowRecursiveNodeChecking, readOnly, expandNodesOnFiltering;		
		DefaultBoolean allowPixelScrolling;
		public TreeListOptionsBehavior(TreeList treeList) {
			this.TreeList = treeList;
			this.editable = this.resizeNodes =
				this.autoSelectAllInEditor = this.autoNodeHeight = this.closeEditorOnLostFocus =
				this.autoChangeParent = this.keepSelectedOnClick = this.smartMouseHover =
				this.showToolTips = this.allowExpandOnDblClick = this.immediateEditor =
				this.autoPopulateColumns = true;
			this.populateServiceColumns = this.showEditorOnMouseUp = this.expandNodesOnIncrementalSearch = this.allowRecursiveNodeChecking =
				this.allowIndeterminateCheckState = this.allowIncrementalSearch = this.enableFiltering = this.readOnly = this.expandNodesOnFiltering = false;
			this.allowPixelScrolling = DefaultBoolean.Default;
		}
		protected TreeList TreeList { get; private set; }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsBehaviorEditable"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool Editable {
			get { return editable; }
			set {
				if(Editable == value) return;
				editable = value;
				BooleanChanged(EditableName, Editable);
			}
		}
		[Obsolete("Use the OptionsNavigation.MoveOnEdit property instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool MoveOnEdit {
			get { return TreeList.OptionsNavigation.MoveOnEdit; }
			set { TreeList.OptionsNavigation.MoveOnEdit = value; }   
		}
		[Obsolete("Use the OptionsDragAndDrop.ExpandNodeOnDrag property instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool ExpandNodeOnDrag {
			get { return TreeList.OptionsDragAndDrop.ExpandNodeOnDrag; }
			set { TreeList.OptionsDragAndDrop.ExpandNodeOnDrag = value; }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsBehaviorResizeNodes"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ResizeNodes {
			get { return resizeNodes; }
			set {
				if(ResizeNodes == value) return;
				resizeNodes = value;
				BooleanChanged(ResizeNodesName, ResizeNodes);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsBehaviorAutoSelectAllInEditor"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AutoSelectAllInEditor {
			get { return autoSelectAllInEditor; }
			set {
				if(AutoSelectAllInEditor == value) return;
				autoSelectAllInEditor = value;
				BooleanChanged(AutoSelectAllInEditorName, AutoSelectAllInEditor);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsBehaviorAutoNodeHeight"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AutoNodeHeight {
			get { return autoNodeHeight; }
			set {
				if(AutoNodeHeight == value) return;
				autoNodeHeight = value;
				BooleanChanged(AutoNodeHeightName, AutoNodeHeight);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsBehaviorCloseEditorOnLostFocus"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool CloseEditorOnLostFocus {
			get { return closeEditorOnLostFocus; }
			set {
				if(CloseEditorOnLostFocus == value) return;
				closeEditorOnLostFocus = value;
				BooleanChanged(CloseEditorOnLostFocusName, CloseEditorOnLostFocus);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsBehaviorAutoChangeParent"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AutoChangeParent {
			get { return autoChangeParent; }
			set {
				if(AutoChangeParent == value) return;
				autoChangeParent = value;
				BooleanChanged(AutoChangeParentName, AutoChangeParent);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsBehaviorAutoPopulateColumns"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AutoPopulateColumns {
			get { return autoPopulateColumns; }
			set {
				if(AutoPopulateColumns == value) return;
				autoPopulateColumns = value;
				BooleanChanged(AutoPopulateColumnsName, AutoPopulateColumns);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsBehaviorKeepSelectedOnClick"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool KeepSelectedOnClick {
			get { return keepSelectedOnClick; }
			set {
				if(KeepSelectedOnClick == value) return;
				keepSelectedOnClick = value;
				BooleanChanged(KeepSelectedOnClickName, KeepSelectedOnClick);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsBehaviorSmartMouseHover"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool SmartMouseHover {
			get { return smartMouseHover; }
			set {
				if(SmartMouseHover == value) return;
				smartMouseHover = value;
				BooleanChanged(SmartMouseHoverName, SmartMouseHover);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsBehaviorShowToolTips"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowToolTips {
			get { return showToolTips; }
			set {
				if(ShowToolTips == value) return;
				showToolTips = value;
				BooleanChanged(ShowToolTipsName, ShowToolTips);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsBehaviorAllowExpandOnDblClick"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowExpandOnDblClick {
			get { return allowExpandOnDblClick; }
			set {
				if(AllowExpandOnDblClick == value) return;
				allowExpandOnDblClick = value;
				BooleanChanged(AllowExpandOnDblClickName, AllowExpandOnDblClick);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsBehaviorImmediateEditor"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ImmediateEditor {
			get { return immediateEditor; }
			set {
				if(ImmediateEditor == value) return;
				immediateEditor = value;
				BooleanChanged(ImmediateEditorName, ImmediateEditor);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsBehaviorPopulateServiceColumns"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool PopulateServiceColumns {
			get { return populateServiceColumns; }
			set {
				if(PopulateServiceColumns == value) return;
				populateServiceColumns = value;
				BooleanChanged(PopulateServiceColumnsName, PopulateServiceColumns);
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DefaultValue(false),
		Obsolete("Use the OptionsDragAndDrop.DragNodesMode property instead"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool DragNodes {
			get { return TreeList.OptionsDragAndDrop.DragNodesMode != DragNodesMode.None; }
			set {
				TreeList.OptionsDragAndDrop.DragNodesMode = value ? XtraTreeList.DragNodesMode.Single : XtraTreeList.DragNodesMode.None;
			}
		}		
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsBehaviorShowEditorOnMouseUp"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool ShowEditorOnMouseUp {
			get { return showEditorOnMouseUp; }
			set {
				if(ShowEditorOnMouseUp == value) return;
				showEditorOnMouseUp = value;
				BooleanChanged(ShowEditorOnMouseUpName, ShowEditorOnMouseUp);
			}
		}
		[Obsolete("Use the OptionsDragAndDrop.CanCloneNodesOnDrop property instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool CanCloneNodesOnDrop {
			get { return TreeList.OptionsDragAndDrop.CanCloneNodesOnDrop; }
			set { TreeList.OptionsDragAndDrop.CanCloneNodesOnDrop = value; }
		}
		[Obsolete("Use the OptionsNavigation.UseTabKey property instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool UseTabKey {
			get { return TreeList.OptionsNavigation.UseTabKey; }
			set { TreeList.OptionsNavigation.UseTabKey = value; }
		}
		[Obsolete("Use the OptionsNavigation.EnterMovesNextColumn property instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool EnterMovesNextColumn {
			get { return TreeList.OptionsNavigation.EnterMovesNextColumn; }
			set { TreeList.OptionsNavigation.EnterMovesNextColumn = value; }
		}
		[Obsolete("Use the OptionsNavigation.AutoMoveRowFocus property instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool AutoMoveRowFocus {
			get { return TreeList.OptionsNavigation.AutoMoveRowFocus; }
			set { TreeList.OptionsNavigation.AutoMoveRowFocus = value; }
		}
		[Obsolete("Use the OptionsNavigation.AutoFocusNewNode property instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool AutoFocusNewNode {
			get { return TreeList.OptionsNavigation.AutoFocusNewNode; }
			set { TreeList.OptionsNavigation.AutoFocusNewNode = value; }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsBehaviorAllowIncrementalSearch"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool AllowIncrementalSearch {
			get { return allowIncrementalSearch; }
			set {
				if(AllowIncrementalSearch == value) return;
				allowIncrementalSearch = value;
				BooleanChanged(AllowIncrementalSearchName, AllowIncrementalSearch);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsBehaviorExpandNodesOnIncrementalSearch"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool ExpandNodesOnIncrementalSearch {
			get { return expandNodesOnIncrementalSearch; }
			set {
				if(expandNodesOnIncrementalSearch == value) return;
				expandNodesOnIncrementalSearch = value;
				BooleanChanged(ExpandNodesOnIncrementalSearchName, ExpandNodesOnIncrementalSearch);
			}
		}
		[ DefaultValue(false), XtraSerializableProperty()]
		public virtual bool ExpandNodesOnFiltering {
			get { return expandNodesOnFiltering; }
			set {
				if(ExpandNodesOnFiltering == value) return;
				expandNodesOnFiltering = value;
				BooleanChanged(ExpandNodesOnFilteringName, ExpandNodesOnFiltering);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsBehaviorEnableFiltering"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool EnableFiltering {
			get { return enableFiltering; }
			set {
				if(enableFiltering == value) return;
				enableFiltering = value;
				BooleanChanged(EnableFilteringName, EnableFiltering);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsBehaviorAllowIndeterminateCheckState"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool AllowIndeterminateCheckState {
			get { return allowIndeterminateCheckState; }
			set {
				if(AllowIndeterminateCheckState == value) return;
				allowIndeterminateCheckState = value;
				BooleanChanged(AllowIndeterminateCheckStateName, AllowIndeterminateCheckState);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsBehaviorAllowRecursiveNodeChecking"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool AllowRecursiveNodeChecking {
			get { return allowRecursiveNodeChecking; }
			set {
				if(AllowRecursiveNodeChecking == value) return;
				allowRecursiveNodeChecking = value;
				BooleanChanged(AllowRecursiveNodeCheckingName, AllowRecursiveNodeChecking);
			}
		}
		[Obsolete("Use the OptionsClipboard.AllowCopy property instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool AllowCopyToClipboard {
			get { return TreeList.OptionsClipboard.AllowCopy != DefaultBoolean.False; ; }
			set { TreeList.OptionsClipboard.AllowCopy = value ? DefaultBoolean.True : DefaultBoolean.False; }
		}
		[Obsolete("Use the OptionsClipboard.CopyColumnHeaders property instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool CopyToClipboardWithColumnHeaders {
			get { return TreeList.OptionsClipboard.CopyColumnHeaders != DefaultBoolean.False; }
			set { TreeList.OptionsClipboard.CopyColumnHeaders = value ? DefaultBoolean.True : DefaultBoolean.False; }
		}
		[Obsolete("Use the OptionsClipboard.CopyNodeHierarchy property instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool CopyToClipboardWithNodeHierarchy {
			get { return TreeList.OptionsClipboard.CopyNodeHierarchy != DefaultBoolean.False; }
			set { TreeList.OptionsClipboard.CopyNodeHierarchy = value ? DefaultBoolean.True : DefaultBoolean.False; }
		}
		[Obsolete("Use the OptionsCustomization.AllowQuickHideColumns property instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool AllowQuickHideColumns {
			get { return TreeList.OptionsCustomization.AllowQuickHideColumns; }
			set { TreeList.OptionsCustomization.AllowQuickHideColumns = value; }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsBehaviorAllowPixelScrolling"),
#endif
 DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public virtual DefaultBoolean AllowPixelScrolling {
			get { return allowPixelScrolling; }
			set {
				if(AllowPixelScrolling == value) return;
				DefaultBoolean prevValue = AllowPixelScrolling;
				allowPixelScrolling = value;
				OnChanged(new BaseOptionChangedEventArgs(AllowPixelScrollingName, prevValue, AllowPixelScrolling));
			}
		}
		[ DefaultValue(false), XtraSerializableProperty()]
		public virtual bool ReadOnly {
			get { return readOnly; }
			set {
				if(ReadOnly == value) return;
				readOnly = value;
				BooleanChanged(ReadOnlyName, ReadOnly);
			}
		}
		protected internal const string
			EditableName = "Editable",
			ResizeNodesName = "ResizeNodes",
			AutoSelectAllInEditorName = "AutoSelectAllInEditor",
			AutoNodeHeightName = "AutoNodeHeight",
			CloseEditorOnLostFocusName = "CloseEditorOnLostFocus",
			AutoChangeParentName = "AutoChangeParent",
			AutoPopulateColumnsName = "AutoPopulateColumns",
			KeepSelectedOnClickName = "KeepSelectedOnClick",
			SmartMouseHoverName = "SmartMouseHover",
			ShowToolTipsName = "ShowToolTips",
			AllowExpandOnDblClickName = "AllowExpandOnDblClick",
			PopulateServiceColumnsName = "PopulateServiceColumns",			
			ShowEditorOnMouseUpName = "ShowEditorOnMouseUp",			
			ImmediateEditorName = "ImmediateEditor",
			AllowIncrementalSearchName = "AllowIncrementalSearch",
			ExpandNodesOnIncrementalSearchName = "ExpandNodesOnIncrementalSearch",
			AllowIndeterminateCheckStateName = "AllowIndeterminateCheckState",
			EnableFilteringName = "EnableFiltering",
			AllowRecursiveNodeCheckingName = "AllowRecursiveNodeChecking",
			AllowPixelScrollingName = "AllowPixelScrolling",
			ReadOnlyName = "ReadOnly",
			ExpandNodesOnFilteringName = "ExpandNodesOnFiltering";
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				TreeListOptionsBehavior opt = options as TreeListOptionsBehavior;
				if(opt == null) return;
				this.editable = opt.Editable;
				this.resizeNodes = opt.ResizeNodes;
				this.autoSelectAllInEditor = opt.AutoSelectAllInEditor;
				this.autoNodeHeight = opt.AutoNodeHeight;
				this.closeEditorOnLostFocus = opt.CloseEditorOnLostFocus;
				this.autoChangeParent = opt.AutoChangeParent;
				this.autoPopulateColumns = opt.AutoPopulateColumns;
				this.keepSelectedOnClick = opt.KeepSelectedOnClick;
				this.smartMouseHover = opt.SmartMouseHover;
				this.showToolTips = opt.ShowToolTips;
				this.populateServiceColumns = opt.PopulateServiceColumns;
				this.ShowEditorOnMouseUp = opt.ShowEditorOnMouseUp; 				
				this.allowExpandOnDblClick = opt.AllowExpandOnDblClick;
				this.allowIncrementalSearch = opt.allowIncrementalSearch;
				this.expandNodesOnIncrementalSearch = opt.expandNodesOnIncrementalSearch;
				this.allowRecursiveNodeChecking = opt.AllowRecursiveNodeChecking;
				this.readOnly = opt.ReadOnly;
				this.allowPixelScrolling = opt.AllowPixelScrolling;
				this.expandNodesOnFiltering = opt.ExpandNodesOnFiltering;
			}
			finally {
				EndUpdate();
			}
		}
	}
	public class TreeListOptionsPrint : BaseTreeListOptions {
		bool printAllNodes, printPageHeader, printReportFooter, printTree, printTreeButtons,
			printFilledTreeIndent, printImages, printPreview, printRowFooterSummary, printHorzLines,
			printVertLines, autoWidth, autoRowHeight, usePrintStyles, printBandHeader, showPrintExportProgress, allowCancelPrintExport;
		public TreeListOptionsPrint() {
			this.printPageHeader = this.printReportFooter = this.printTree = this.printTreeButtons =
				this.printImages = this.printHorzLines = this.printVertLines = this.autoWidth =
				this.autoRowHeight = this.usePrintStyles = this.printBandHeader = this.allowCancelPrintExport = true;
			this.printAllNodes = this.printFilledTreeIndent = this.printPreview = 
				this.printRowFooterSummary = this.showPrintExportProgress = false;
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsPrintPrintPageHeader"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool PrintPageHeader {
			get { return printPageHeader; }
			set {
				if(PrintPageHeader == value) return;
				printPageHeader = value;
				BooleanChanged(PrintPageHeaderName, PrintPageHeader);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsPrintPrintBandHeader"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool PrintBandHeader {
			get { return printBandHeader; }
			set {
				if(PrintBandHeader == value) return;
				printBandHeader = value;
				BooleanChanged(PrintBandHeaderName,PrintBandHeader);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsPrintPrintReportFooter"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool PrintReportFooter {
			get { return printReportFooter; }
			set {
				if(PrintReportFooter == value) return;
				printReportFooter = value;
				BooleanChanged(PrintReportFooterName, PrintReportFooter);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsPrintPrintTree"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool PrintTree {
			get { return printTree; }
			set {
				if(PrintTree == value) return;
				printTree = value;
				BooleanChanged(PrintTreeName, PrintTree);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsPrintPrintTreeButtons"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool PrintTreeButtons {
			get { return printTreeButtons; }
			set {
				if(PrintTreeButtons == value) return;
				printTreeButtons = value;
				BooleanChanged(PrintTreeButtonsName, PrintTreeButtons);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsPrintPrintImages"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool PrintImages {
			get { return printImages; }
			set {
				if(PrintImages == value) return;
				printImages = value;
				BooleanChanged(PrintImagesName, PrintImages);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsPrintPrintHorzLines"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool PrintHorzLines {
			get { return printHorzLines; }
			set {
				if(PrintHorzLines == value) return;
				printHorzLines = value;
				BooleanChanged(PrintHorzLinesName, PrintHorzLines);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsPrintPrintVertLines"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool PrintVertLines {
			get { return printVertLines; }
			set {
				if(PrintVertLines == value) return;
				printVertLines = value;
				BooleanChanged(PrintVertLinesName, PrintVertLines);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsPrintAutoWidth"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AutoWidth {
			get { return autoWidth; }
			set {
				if(AutoWidth == value) return;
				autoWidth = value;
				BooleanChanged(AutoWidthName, AutoWidth);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsPrintAutoRowHeight"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AutoRowHeight {
			get { return autoRowHeight; }
			set {
				if(AutoRowHeight == value) return;
				autoRowHeight = value;
				BooleanChanged(AutoRowHeightName, AutoRowHeight);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsPrintPrintAllNodes"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool PrintAllNodes {
			get { return printAllNodes; }
			set {
				if(PrintAllNodes == value) return;
				printAllNodes = value;
				BooleanChanged(PrintAllNodesName, PrintAllNodes);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsPrintPrintFilledTreeIndent"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool PrintFilledTreeIndent {
			get { return printFilledTreeIndent; }
			set {
				if(PrintFilledTreeIndent == value) return;
				printFilledTreeIndent = value;
				BooleanChanged(PrintFilledTreeIndentName, PrintFilledTreeIndent);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsPrintPrintPreview"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool PrintPreview {
			get { return printPreview; }
			set {
				if(PrintPreview == value) return;
				printPreview = value;
				BooleanChanged(PrintPreviewName, PrintPreview);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsPrintPrintRowFooterSummary"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool PrintRowFooterSummary {
			get { return printRowFooterSummary; }
			set {
				if(PrintRowFooterSummary == value) return;
				printRowFooterSummary = value;
				BooleanChanged(PrintRowFooterSummaryName, PrintRowFooterSummary);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsPrintUsePrintStyles"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool UsePrintStyles {
			get { return usePrintStyles; }
			set {
				if(UsePrintStyles == value) return;
				usePrintStyles = value;
				BooleanChanged(UsePrintStylesName, UsePrintStyles);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsPrintAllowCancelPrintExport"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowCancelPrintExport {
			get { return allowCancelPrintExport; }
			set {
				if(AllowCancelPrintExport == value) return;
				allowCancelPrintExport = value;
				BooleanChanged(AllowCancelPrintExportName, AllowCancelPrintExport);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsPrintShowPrintExportProgress"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool ShowPrintExportProgress {
			get { return showPrintExportProgress; }
			set {
				if(ShowPrintExportProgress == value) return;
				showPrintExportProgress = value;
				BooleanChanged(ShowPrintExportProgressName, ShowPrintExportProgress);
			}
		}
		protected internal const string
			PrintPageHeaderName = "PrintPageHeader",
			PrintReportFooterName = "PrintReportFooter",
			PrintTreeName = "PrintTree",
			PrintTreeButtonsName = "PrintTreeButtons",
			PrintImagesName = "PrintImages",
			PrintHorzLinesName = "PrintHorzLines",
			PrintVertLinesName = "PrintVertLines",
			AutoWidthName = "AutoWidth",
			AutoRowHeightName = "AutoRowHeight",
			PrintAllNodesName = "PrintAllNodes",
			PrintFilledTreeIndentName = "PrintFilledTreeIndent",
			PrintPreviewName = "PrintPreview",
			PrintRowFooterSummaryName = "PrintRowFooterSummary",
			UsePrintStylesName = "UsePrintStyles",
			PrintBandHeaderName = "PrintBandHeader",
			ShowPrintExportProgressName = "ShowPrintExportProgress",
			AllowCancelPrintExportName = "AllowCancelPrintExport";
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				TreeListOptionsPrint opt = options as TreeListOptionsPrint;
				if(opt == null) return;
				this.printPageHeader = opt.PrintPageHeader; 
				this.printReportFooter = opt.PrintReportFooter; 
				this.printTree = opt.PrintTree; 
				this.printTreeButtons = opt.PrintTreeButtons;
				this.printImages = opt.PrintImages; 
				this.printHorzLines = opt.PrintHorzLines; 
				this.printVertLines = opt.PrintVertLines; 
				this.autoWidth = opt.AutoWidth;
				this.autoRowHeight = opt.AutoRowHeight; 
				this.printAllNodes = opt.PrintAllNodes; 
				this.printFilledTreeIndent = opt.PrintFilledTreeIndent; 
				this.printPreview = opt.PrintPreview; 
				this.printRowFooterSummary = opt.PrintRowFooterSummary; 
				this.usePrintStyles = opt.UsePrintStyles;
				this.printBandHeader = opt.PrintBandHeader;
				this.showPrintExportProgress = opt.ShowPrintExportProgress;
				this.allowCancelPrintExport = opt.AllowCancelPrintExport;
			}
			finally {
				EndUpdate();
			}
		}
	}
	public class TreeListOptionsSelection : BaseTreeListOptions {
		bool invertSelection, enableAppearanceFocusedRow, enableAppearanceFocusedCell,
			multiSelect, useIndicatorForSelection, selectNodesOnRightClick;
		TreeListMultiSelectMode multiSelectMode;
		public TreeListOptionsSelection() {
			this.enableAppearanceFocusedRow = this.enableAppearanceFocusedCell = true;
			this.invertSelection = this.multiSelect = this.selectNodesOnRightClick = false;
			this.multiSelectMode = TreeListMultiSelectMode.RowSelect;
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsSelectionEnableAppearanceFocusedRow"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool EnableAppearanceFocusedRow {
			get { return enableAppearanceFocusedRow; }
			set {
				if(EnableAppearanceFocusedRow == value) return;
				enableAppearanceFocusedRow = value;
				BooleanChanged(EnableAppearanceFocusedRowName, EnableAppearanceFocusedRow);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsSelectionEnableAppearanceFocusedCell"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool EnableAppearanceFocusedCell {
			get { return enableAppearanceFocusedCell; }
			set {
				if(EnableAppearanceFocusedCell == value) return;
				enableAppearanceFocusedCell = value;
				BooleanChanged(EnableAppearanceFocusedCellName, EnableAppearanceFocusedCell);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsSelectionInvertSelection"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool InvertSelection {
			get { return invertSelection; }
			set {
				if(InvertSelection == value) return;
				invertSelection = value;
				BooleanChanged(InvertSelectionName, InvertSelection);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsSelectionMultiSelect"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool MultiSelect {
			get { return multiSelect; }
			set {
				if(MultiSelect == value) return;
				multiSelect = value;
				BooleanChanged(MultiSelectName, MultiSelect);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsSelectionUseIndicatorForSelection"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool UseIndicatorForSelection {
			get { return useIndicatorForSelection; }
			set {
				if(UseIndicatorForSelection == value) return;
				useIndicatorForSelection = value;
				BooleanChanged(UseIndicatorForSelectionName, UseIndicatorForSelection);
			}
		}
		[ DefaultValue(false), XtraSerializableProperty()]
		public virtual bool SelectNodesOnRightClick {
			get { return selectNodesOnRightClick; }
			set {
				if(SelectNodesOnRightClick == value) return;
				selectNodesOnRightClick = value;
				BooleanChanged(SelectNodesOnRightClickName, SelectNodesOnRightClick);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsSelectionMultiSelectMode"),
#endif
 DefaultValue(TreeListMultiSelectMode.RowSelect), XtraSerializableProperty()]
		public virtual TreeListMultiSelectMode MultiSelectMode {
			get { return multiSelectMode; }
			set {
				if(MultiSelectMode == value) return;
				TreeListMultiSelectMode prevValue = multiSelectMode;
				multiSelectMode = value;
				OnChanged(new BaseOptionChangedEventArgs(MultiSelectModeName, prevValue, MultiSelectMode));
			}
		}
		protected internal const string
			EnableAppearanceFocusedRowName = "EnableAppearanceFocusedRow",
			EnableAppearanceFocusedCellName = "EnableAppearanceFocusedCell",
			InvertSelectionName = "InvertSelection",
			MultiSelectName = "MultiSelect",
			UseIndicatorForSelectionName = "UseIndicatorForSelectionName",
			SelectNodesOnRightClickName = "SelectNodesOnRightClick",
			MultiSelectModeName = "MultiSelectMode";
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				TreeListOptionsSelection opt = options as TreeListOptionsSelection;
				if(opt == null) return;
				this.invertSelection = opt.InvertSelection;
				this.enableAppearanceFocusedRow = opt.EnableAppearanceFocusedRow;
				this.enableAppearanceFocusedCell = opt.EnableAppearanceFocusedCell;
				this.multiSelect = opt.MultiSelect;
				this.useIndicatorForSelection = opt.useIndicatorForSelection;
				this.selectNodesOnRightClick = opt.SelectNodesOnRightClick;
				this.multiSelectMode = opt.MultiSelectMode;
			}
			finally {
				EndUpdate();
			}
		}
		internal void SetMultiSelect(bool value) {
			this.multiSelect = value;
		}
	}
	public class TreeListOptionsMenu : BaseTreeListOptions {
		bool enableColumnMenu, enableFooterMenu, showAutoFilterRowItem, showConditionalFormattingItem;
		public TreeListOptionsMenu() {
			this.enableColumnMenu = this.enableFooterMenu = true;
			this.showAutoFilterRowItem = true;
			this.showConditionalFormattingItem = false;
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsMenuEnableColumnMenu"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool EnableColumnMenu {
			get { return enableColumnMenu; }
			set {
				if(EnableColumnMenu == value) return;
				enableColumnMenu = value;
				BooleanChanged(EnableColumnMenuName, EnableColumnMenu);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsMenuEnableFooterMenu"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool EnableFooterMenu {
			get { return enableFooterMenu; }
			set {
				if(EnableFooterMenu == value) return;
				enableFooterMenu = value;
				BooleanChanged(EnableFooterMenuName, EnableFooterMenu);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsMenuShowAutoFilterRowItem"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowAutoFilterRowItem {
			get { return showAutoFilterRowItem; }
			set {
				if(ShowAutoFilterRowItem == value) return;
				showAutoFilterRowItem = value;
				BooleanChanged(ShowAutoFilterRowItemName, ShowAutoFilterRowItem);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsMenuShowConditionalFormattingItem"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool ShowConditionalFormattingItem {
			get { return showConditionalFormattingItem; }
			set {
				if(ShowConditionalFormattingItem == value) return;
				showConditionalFormattingItem = value;
				BooleanChanged(ShowConditionalFormattingItemName, ShowConditionalFormattingItem);
			}
		}
		protected internal const string
			EnableColumnMenuName = "EnableColumnMenu",
			EnableFooterMenuName = "EnableFooterMenu",
			ShowAutoFilterRowItemName = "ShowAutoFilterRowItem",
			ShowConditionalFormattingItemName = "ShowConditionalFormattingItem";
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				TreeListOptionsMenu opt = options as TreeListOptionsMenu;
				if(opt == null) return;
				this.enableColumnMenu = opt.EnableColumnMenu;
				this.enableFooterMenu = opt.EnableFooterMenu;
				this.showAutoFilterRowItem = opt.ShowAutoFilterRowItem;
				this.showConditionalFormattingItem = opt.ShowConditionalFormattingItem;
			}
			finally {
				EndUpdate();
			}
		}
	}
	public class OptionsLayoutTreeList : OptionsLayoutBase {
		bool addNewColumns, removeOldColumns, storeAppearance;
		public OptionsLayoutTreeList() {
			this.addNewColumns = true;
			this.removeOldColumns = false;
			this.storeAppearance = false;
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("OptionsLayoutTreeListAddNewColumns"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool AddNewColumns {
			get { return addNewColumns; }
			set {
				if(AddNewColumns == value) return;
				bool prevValue = AddNewColumns;
				addNewColumns = value;
				OnChanged(new BaseOptionChangedEventArgs("AddNewColumns", prevValue, AddNewColumns));
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("OptionsLayoutTreeListRemoveOldColumns"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public bool RemoveOldColumns {
			get { return removeOldColumns; }
			set {
				if(RemoveOldColumns == value) return;
				bool prevValue = RemoveOldColumns;
				removeOldColumns = value;
				OnChanged(new BaseOptionChangedEventArgs("RemoveOldColumns", prevValue, RemoveOldColumns));
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("OptionsLayoutTreeListStoreAppearance"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public bool StoreAppearance {
			get { return storeAppearance; }
			set {
				if(StoreAppearance == value) return;
				bool prevValue = StoreAppearance;
				storeAppearance = value;
				OnChanged(new BaseOptionChangedEventArgs("StoreAppearance", prevValue, StoreAppearance));
			}
		}
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			OptionsLayoutTreeList opt = options as OptionsLayoutTreeList;
			if(opt == null) return;
			this.addNewColumns = opt.AddNewColumns;
			this.removeOldColumns = opt.RemoveOldColumns;
			this.storeAppearance = opt.StoreAppearance;
		}
	}
	public class TreeListOptionsFilter : BaseTreeListOptions {
		bool allowFilterEditor, allowMRUFilterList, allowColumnMRUFilterList, showAllValuesInFilterPopup, showAllValuesInCheckedFilterPopup;
		int mruFilterListPopupCount, columnFilterPopupRowCount, mruColumnFilterListCount;
		FilterEditorViewMode defaultFilterEditorView;
		FilterMode filterMode;
		public TreeListOptionsFilter() : base() {
			this.allowFilterEditor = allowMRUFilterList = allowColumnMRUFilterList = true;
			this.showAllValuesInFilterPopup = false;
			this.showAllValuesInCheckedFilterPopup = true;
			this.mruFilterListPopupCount = 7;
			this.columnFilterPopupRowCount = 20;
			this.mruColumnFilterListCount = 5;
			this.filterMode = FilterMode.Default;
			this.defaultFilterEditorView = FilterEditorViewMode.Visual;
		}
		[ DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowFilterEditor {
			get { return allowFilterEditor; }
			set {
				if(AllowFilterEditor == value) return;
				allowFilterEditor = value;
				BooleanChanged(AllowFilterEditorName, AllowFilterEditor);
			}
		}
		[ DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowMRUFilterList {
			get { return allowMRUFilterList; }
			set {
				if(AllowMRUFilterList == value) return;
				allowMRUFilterList = value;
				BooleanChanged(AllowMRUFilterListName, AllowMRUFilterList);
			}
		}
		[ DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowColumnMRUFilterList {
			get { return allowColumnMRUFilterList; }
			set {
				if(AllowColumnMRUFilterList == value) return;
				bool prevValue = AllowColumnMRUFilterList;
				allowColumnMRUFilterList = value;
				BooleanChanged(AllowColumnMRUFilterListName, AllowColumnMRUFilterList);
			}
		}
		[ DefaultValue(7), XtraSerializableProperty()]
		public virtual int MRUFilterListPopupCount {
			get { return mruFilterListPopupCount; }
			set {
				if(value < 0) value = 0;
				if(MRUFilterListPopupCount == value) return;
				int prevValue = MRUFilterListPopupCount;
				mruFilterListPopupCount = value;
				OnChanged(new BaseOptionChangedEventArgs(MRUFilterListPopupCountName, prevValue, MRUFilterListPopupCount));
			}
		}
		[ DefaultValue(20), XtraSerializableProperty()]
		public virtual int ColumnFilterPopupRowCount {
			get { return columnFilterPopupRowCount; }
			set {
				if(value < 4) value = 4;
				if(value > 100) value = 100;
				if(ColumnFilterPopupRowCount == value) return;
				int prevValue = ColumnFilterPopupRowCount;
				columnFilterPopupRowCount = value;
				OnChanged(new BaseOptionChangedEventArgs(ColumnFilterPopupRowCountName, prevValue, ColumnFilterPopupRowCount));
			}
		}
		[ DefaultValue(5), XtraSerializableProperty()]
		public virtual int MRUColumnFilterListCount {
			get { return mruColumnFilterListCount; }
			set {
				if(value < 0) value = 0;
				if(MRUColumnFilterListCount == value) return;
				int prevValue = MRUColumnFilterListCount;
				mruColumnFilterListCount = value;
				OnChanged(new BaseOptionChangedEventArgs(MRUColumnFilterListCountName, prevValue, MRUColumnFilterListCount));
			}
		}
		[ DefaultValue(false), XtraSerializableProperty()]
		public virtual bool ShowAllValuesInFilterPopup {
			get { return showAllValuesInFilterPopup; }
			set {
				if(ShowAllValuesInFilterPopup == value) return;
				showAllValuesInFilterPopup = value;
				BooleanChanged(ShowAllValuesInFilterPopupName, ShowAllValuesInFilterPopup);
			}
		}
		[ DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowAllValuesInCheckedFilterPopup {
			get { return showAllValuesInCheckedFilterPopup; }
			set {
				if(ShowAllValuesInCheckedFilterPopup == value) return;
				showAllValuesInCheckedFilterPopup = value;
				BooleanChanged(ShowAllValuesInCheckedFilterPopupName, ShowAllValuesInCheckedFilterPopup); 
			}
		}
		[ DefaultValue(FilterMode.Default), XtraSerializableProperty()]
		public virtual FilterMode FilterMode {
			get { return filterMode; }
			set {
				if(FilterMode == value) return;
				FilterMode prevValue = FilterMode;
				filterMode = value;
				OnChanged(new BaseOptionChangedEventArgs(FilterModeName, prevValue, FilterMode));
			}
		}
		[ DefaultValue(FilterEditorViewMode.Visual), XtraSerializableProperty()]
		public virtual FilterEditorViewMode DefaultFilterEditorView {
			get { return defaultFilterEditorView; }
			set {
				if(DefaultFilterEditorView == value) return;
				FilterEditorViewMode prevValue = DefaultFilterEditorView;
				defaultFilterEditorView = value;
				OnChanged(new BaseOptionChangedEventArgs(DefaultFilterEditorViewName, prevValue, DefaultFilterEditorView));
			}
		}
		protected internal const string
			AllowFilterEditorName = "AllowFilterEditor",
			MRUFilterListPopupCountName = "MRUFilterListPopupCount",
			AllowMRUFilterListName = "AllowMRUFilterList",
			ColumnFilterPopupRowCountName = "ColumnFilterPopupRowCount",
			AllowColumnMRUFilterListName = "AllowColumnMRUFilterList",
			MRUColumnFilterListCountName = "MRUColumnFilterListCount",
			ShowAllValuesInFilterPopupName = "ShowAllValuesInFilterPopup",
			ShowAllValuesInCheckedFilterPopupName = "ShowAllValuesInCheckedFilterPopup",
			FilterModeName = "FilterMode",
			DefaultFilterEditorViewName = "DefaultFilterEditorView";
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			TreeListOptionsFilter opt = options as TreeListOptionsFilter;
			if(opt == null) return;
			this.allowFilterEditor = opt.AllowFilterEditor;
			this.mruFilterListPopupCount = opt.MRUFilterListPopupCount;
			this.allowMRUFilterList = opt.AllowMRUFilterList;
			this.columnFilterPopupRowCount = opt.ColumnFilterPopupRowCount;
			this.allowColumnMRUFilterList = opt.AllowColumnMRUFilterList;
			this.mruColumnFilterListCount = opt.MRUColumnFilterListCount;
			this.showAllValuesInCheckedFilterPopup = opt.ShowAllValuesInCheckedFilterPopup;
			this.showAllValuesInFilterPopup = opt.ShowAllValuesInFilterPopup;
			this.filterMode = opt.FilterMode;
			this.defaultFilterEditorView = opt.DefaultFilterEditorView;
		}
	}
	public class TreeListOptionsCustomization : BaseTreeListOptions {
		bool allowChangeBandParent, allowChangeColumnParent;
		bool showBandsInCustomizationForm, allowBandResizing, allowBandMoving;
		bool allowColumnResizing, allowColumnMoving, allowQuickHideColumns, customizationFormSearchBoxVisible;
		public TreeListOptionsCustomization() : base() {
			this.allowChangeBandParent = this.allowChangeColumnParent = this.customizationFormSearchBoxVisible = false;
			this.showBandsInCustomizationForm = this.allowBandResizing =  this.allowBandMoving = true;
			this.allowColumnResizing = this.allowColumnMoving = this.allowQuickHideColumns = true;
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsCustomizationAllowChangeBandParent"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public bool AllowChangeBandParent {
			get { return allowChangeBandParent; }
			set {
				if(AllowChangeBandParent == value) return;
				allowChangeBandParent = value;
				BooleanChanged(AllowChangeBandParentName, AllowChangeBandParent);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsCustomizationAllowChangeColumnParent"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public bool AllowChangeColumnParent {
			get { return allowChangeColumnParent; }
			set {
				if(AllowChangeColumnParent == value) return;
				allowChangeColumnParent = value;
				BooleanChanged(AllowChangeColumnParentName, AllowChangeColumnParent);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsCustomizationAllowBandResizing"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool AllowBandResizing {
			get { return allowBandResizing; }
			set {
				if(AllowBandResizing == value) return;
				allowBandResizing = value;
				BooleanChanged(AllowBandResizingName, AllowBandResizing);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsCustomizationAllowBandMoving"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool AllowBandMoving {
			get { return allowBandMoving; }
			set {
				if(AllowBandMoving == value) return;
				allowBandMoving = value;
				BooleanChanged(AllowBandMovingName, AllowBandMoving);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsCustomizationShowBandsInCustomizationForm"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool ShowBandsInCustomizationForm {
			get { return showBandsInCustomizationForm; }
			set {
				if(ShowBandsInCustomizationForm == value) return;
				showBandsInCustomizationForm = value;
				BooleanChanged(ShowBandsInCustomizationFormName, ShowBandsInCustomizationForm);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsCustomizationAllowColumnResizing"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool AllowColumnResizing {
			get { return allowColumnResizing; }
			set {
				if(AllowColumnResizing == value) return;
				allowColumnResizing = value;
				BooleanChanged(AllowColumnResizingName, AllowColumnResizing);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsCustomizationAllowColumnMoving"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool AllowColumnMoving {
			get { return allowColumnMoving; }
			set {
				if(AllowColumnMoving == value) return;
				allowColumnMoving = value;
				BooleanChanged(AllowColumnMovingName, AllowColumnMoving);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsCustomizationAllowQuickHideColumns"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowQuickHideColumns {
			get { return allowQuickHideColumns; }
			set {
				if(AllowQuickHideColumns == value) return;
				allowQuickHideColumns = value;
				BooleanChanged(AllowQuickHideColumnsName, AllowQuickHideColumns);
			}
		}
		[ DefaultValue(false), XtraSerializableProperty()]
		public virtual bool CustomizationFormSearchBoxVisible {
			get { return customizationFormSearchBoxVisible; }
			set {
				if(CustomizationFormSearchBoxVisible == value) return;
				customizationFormSearchBoxVisible = value;
				BooleanChanged(CustomizationFormSearchBoxVisibleName, CustomizationFormSearchBoxVisible);
			}
		}
		protected internal const string
		   AllowChangeBandParentName = "AllowChangeBandParent",
		   AllowChangeColumnParentName = "AllowChangeColumnParent",
		   AllowBandResizingName = "AllowBandResizing",
		   AllowBandMovingName = "AllowBandMoving",
		   ShowBandsInCustomizationFormName = "ShowBandsInCustomizationForm",
		   AllowColumnResizingName = "AllowColumnResizing",
		   AllowColumnMovingName = "AllowColumnMoving",
		   AllowQuickHideColumnsName = "AllowQuickHideColumns",
		   CustomizationFormSearchBoxVisibleName = "CustomizationFormSearchBoxVisible";
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				TreeListOptionsCustomization opt = options as TreeListOptionsCustomization;
				if(opt == null) return;
				this.allowChangeBandParent = opt.AllowChangeBandParent;
				this.allowChangeColumnParent = opt.AllowChangeColumnParent;
				this.allowBandResizing = opt.AllowBandResizing;
				this.allowBandMoving = opt.AllowBandMoving;
				this.showBandsInCustomizationForm = opt.ShowBandsInCustomizationForm;
				this.allowColumnResizing = opt.AllowColumnResizing;
				this.allowColumnMoving = opt.AllowColumnMoving;
				this.allowQuickHideColumns = opt.AllowQuickHideColumns;
				this.customizationFormSearchBoxVisible = opt.CustomizationFormSearchBoxVisible;
			}
			finally {
				EndUpdate();
			}
		}
	}
	public class TreeListOptionsNavigation : BaseTreeListOptions {
		bool useTabKey, autoMoveRowFocus, enterMovesNextColumn, autoFocusNewNode, moveOnEdit;
		bool useBandsAdvHorzNavigation, useBandsAdvVertNavigation;
		public TreeListOptionsNavigation() {
			this.moveOnEdit = true;
			this.autoMoveRowFocus = this.enterMovesNextColumn = this.autoFocusNewNode = this.useTabKey = false;
			this.useBandsAdvHorzNavigation = this.useBandsAdvVertNavigation = true;
		}
		[ DefaultValue(true), XtraSerializableProperty()]
		public virtual bool MoveOnEdit {
			get { return moveOnEdit; }
			set {
				if(MoveOnEdit == value) return;
				moveOnEdit = value;
				BooleanChanged(MoveOnEditName, MoveOnEdit);
			}
		}
		[ DefaultValue(false), XtraSerializableProperty()]
		public virtual bool UseTabKey {
			get { return useTabKey; }
			set {
				if(UseTabKey == value) return;
				useTabKey = value;
				BooleanChanged(UseTabKeyName, UseTabKey);
			}
		}
		[ DefaultValue(false), XtraSerializableProperty()]
		public virtual bool EnterMovesNextColumn {
			get { return enterMovesNextColumn; }
			set {
				if(EnterMovesNextColumn == value) return;
				enterMovesNextColumn = value;
				BooleanChanged(EnterMovesNextColumnName, EnterMovesNextColumn);
			}
		}
		[ DefaultValue(false), XtraSerializableProperty()]
		public virtual bool AutoMoveRowFocus {
			get { return autoMoveRowFocus; }
			set {
				if(AutoMoveRowFocus == value) return;
				autoMoveRowFocus = value;
				BooleanChanged(AutoMoveRowFocusName, AutoMoveRowFocus);
			}
		}
		[ DefaultValue(false), XtraSerializableProperty()]
		public virtual bool AutoFocusNewNode {
			get { return autoFocusNewNode; }
			set {
				if(AutoFocusNewNode == value) return;
				autoFocusNewNode = value;
				BooleanChanged(AutoFocusNewNodeName, AutoFocusNewNode);
			}
		}
		[ DefaultValue(true), XtraSerializableProperty()]
		public virtual bool UseBandsAdvHorzNavigation {
			get { return useBandsAdvHorzNavigation; }
			set {
				if(UseBandsAdvHorzNavigation == value) return;
				useBandsAdvHorzNavigation = value;
				BooleanChanged(UseBandsAdvHorzNavigationName, UseBandsAdvHorzNavigation);
			}
		}
		[ DefaultValue(true), XtraSerializableProperty()]
		public virtual bool UseBandsAdvVertNavigation {
			get { return useBandsAdvVertNavigation; }
			set {
				if(UseBandsAdvVertNavigation == value) return;
				useBandsAdvVertNavigation = value;
				BooleanChanged(UseBandsAdvVertNavigationName, UseBandsAdvVertNavigation);
			}
		}
		protected internal const string
			MoveOnEditName = "MoveOnEdit",
			UseTabKeyName = "UseTabKey",
			EnterMovesNextColumnName = "EnterMovesNextColumn",
			AutoMoveRowFocusName = "AutoMoveRowFocus",
			AutoFocusNewNodeName = "AutoFocusNewNode",
			UseBandsAdvHorzNavigationName = "UseBandsAdvHorzNavigation",
			UseBandsAdvVertNavigationName = "UseBandsAdvVertNavigation";
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				TreeListOptionsNavigation opt = options as TreeListOptionsNavigation;
				if(opt == null) return;
				this.moveOnEdit = opt.MoveOnEdit;
				this.useTabKey = opt.UseTabKey;
				this.enterMovesNextColumn = opt.EnterMovesNextColumn;
				this.autoMoveRowFocus = opt.AutoMoveRowFocus;
				this.autoMoveRowFocus = opt.AutoFocusNewNode;
				this.useBandsAdvVertNavigation = opt.UseBandsAdvVertNavigation;
				this.useBandsAdvHorzNavigation = opt.UseBandsAdvHorzNavigation;
			}
			finally {
				EndUpdate();
			}
		}
	}
}
namespace DevExpress.XtraTreeList.Columns {
	public class TreeListOptionsColumn : BaseTreeListOptions {
		bool allowMove, allowSize, allowSort, readOnly, fixedWidth, allowFocus,
			showInCustomizationForm, allowMoveToCustomizationForm, allowEdit, showInExpressionEditor;
		DefaultBoolean printable;
		public TreeListOptionsColumn() {
			this.allowMove = this.showInCustomizationForm = this.allowSort = this.allowEdit =
				this.allowSize = this.allowFocus = this.allowMoveToCustomizationForm = this.showInExpressionEditor = true;
			this.readOnly = this.fixedWidth;
			this.printable = DefaultBoolean.Default;
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsColumnAllowMove"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowMove {
			get { return allowMove; }
			set {
				if(AllowMove == value) return;
				allowMove = value;
				BooleanChanged(AllowMoveName, AllowMove);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsColumnShowInCustomizationForm"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowInCustomizationForm {
			get { return showInCustomizationForm; }
			set {
				if(ShowInCustomizationForm == value) return;
				showInCustomizationForm = value;
				BooleanChanged(ShowInCustomizationFormName, ShowInCustomizationForm);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsColumnAllowSort"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowSort {
			get { return allowSort; }
			set {
				if(AllowSort == value) return;
				allowSort = value;
				BooleanChanged(AllowSortName, AllowSort);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsColumnAllowSize"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowSize {
			get { return allowSize; }
			set {
				if(AllowSize == value) return;
				allowSize = value;
				BooleanChanged(AllowSizeName, AllowSize);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsColumnAllowFocus"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowFocus {
			get { return allowFocus; }
			set {
				if(AllowFocus == value) return;
				allowFocus = value;
				BooleanChanged(AllowFocusName, AllowFocus);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsColumnAllowMoveToCustomizationForm"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowMoveToCustomizationForm {
			get { return allowMoveToCustomizationForm; }
			set {
				if(AllowMoveToCustomizationForm == value) return;
				allowMoveToCustomizationForm = value;
				BooleanChanged(AllowMoveToCustomizationFormName, AllowMoveToCustomizationForm);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsColumnReadOnly"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool ReadOnly {
			get { return readOnly; }
			set {
				if(ReadOnly == value) return;
				readOnly = value;
				BooleanChanged(ReadOnlyName, ReadOnly);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsColumnFixedWidth"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool FixedWidth {
			get { return fixedWidth; }
			set {
				if(FixedWidth == value) return;
				fixedWidth = value;
				BooleanChanged(FixedWidthName, FixedWidth);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsColumnAllowEdit"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowEdit {
			get { return allowEdit; }
			set {
				if(AllowEdit == value) return;
				allowEdit = value;
				BooleanChanged(AllowEditName, AllowEdit);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsColumnShowInExpressionEditor"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool ShowInExpressionEditor {
			get { return showInExpressionEditor; }
			set {
				if(ShowInExpressionEditor == value) return;
				showInExpressionEditor = value;
				BooleanChanged(ShowInExpressionEditorName, ShowInExpressionEditor);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsColumnPrintable"),
#endif
 DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public DefaultBoolean Printable {
			get { return printable; }
			set {
				if(Printable == value) return;
				DefaultBoolean prevValue = Printable;
				printable = value;
				OnChanged(new BaseOptionChangedEventArgs(PrintableName, prevValue, Printable));
			}
		}
		protected internal const string
			AllowMoveName = "AllowMove",
			ShowInCustomizationFormName = "ShowInCustomizationForm",
			AllowSortName = "AllowSort",
			AllowSizeName = "AllowSize",
			AllowFocusName = "AllowFocus",
			AllowMoveToCustomizationFormName = "AllowMoveToCustomizationForm",
			ReadOnlyName = "ReadOnly",
			FixedWidthName = "FixedWidth",
			AllowEditName = "AllowEdit",
			ShowInExpressionEditorName = "ShowInExpressionEditor",
			PrintableName = "Printable";
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				TreeListOptionsColumn opt = options as TreeListOptionsColumn;
				if(opt == null) return;
				this.allowMove = opt.AllowMove;
				this.showInCustomizationForm = opt.ShowInCustomizationForm;
				this.allowSort = opt.AllowSort;
				this.allowSize = opt.AllowSize;
				this.allowFocus = opt.AllowFocus;
				this.AllowMoveToCustomizationForm = opt.AllowMoveToCustomizationForm;
				this.readOnly = opt.ReadOnly;
				this.fixedWidth = opt.FixedWidth;
				this.allowEdit = opt.AllowEdit;
				this.showInExpressionEditor = opt.ShowInExpressionEditor;
				this.printable = opt.Printable;
			}
			finally {
				EndUpdate();
			}
		}
	}
	public enum AutoFilterCondition { Default, Like, Equals, Contains }
	public class TreeListOptionsColumnFilter : BaseTreeListOptions {
		bool allowAutoFilter, immediateUpdateAutoFilter, allowFilter, showEmptyDateFilter;
		AutoFilterCondition autoFilterCondition;
		FilterPopupMode filterPopupMode;
		DefaultBoolean immediateUpdatePopupDateFilter, showBlanksFilterItems;
		public TreeListOptionsColumnFilter() {
			this.autoFilterCondition = AutoFilterCondition.Default;
			this.allowAutoFilter = this.immediateUpdateAutoFilter = this.allowFilter = true;
			this.filterPopupMode = FilterPopupMode.Default;
			this.showEmptyDateFilter = false;
			this.immediateUpdatePopupDateFilter = DefaultBoolean.Default;
			this.showBlanksFilterItems = DefaultBoolean.Default;
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsColumnFilterAutoFilterCondition"),
#endif
 DefaultValue(AutoFilterCondition.Default), XtraSerializableProperty()]
		public virtual AutoFilterCondition AutoFilterCondition {
			get { return autoFilterCondition; }
			set {
				if(AutoFilterCondition == value) return;
				AutoFilterCondition prevValue = AutoFilterCondition;
				autoFilterCondition = value;
				OnChanged(new BaseOptionChangedEventArgs(AutoFilterConditionName, prevValue, AutoFilterCondition));
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsColumnFilterAllowFilter"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowFilter {
			get { return allowFilter; }
			set {
				if(AllowFilter == value) return;
				allowFilter = value;
				BooleanChanged(AllowFilterName, AllowFilter);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsColumnFilterAllowAutoFilter"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowAutoFilter {
			get { return allowAutoFilter; }
			set {
				if(AllowAutoFilter == value) return;
				allowAutoFilter = value;
				BooleanChanged(AllowAutoFilterName, AllowAutoFilter);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsColumnFilterImmediateUpdateAutoFilter"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ImmediateUpdateAutoFilter {
			get { return immediateUpdateAutoFilter; }
			set {
				if(ImmediateUpdateAutoFilter == value) return;
				bool prevValue = ImmediateUpdateAutoFilter;
				immediateUpdateAutoFilter = value;
				BooleanChanged(ImmediateUpdateAutoFilterName, ImmediateUpdateAutoFilter);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsColumnFilterFilterPopupMode"),
#endif
 DefaultValue(FilterPopupMode.Default), XtraSerializableProperty()]
		public virtual FilterPopupMode FilterPopupMode {
			get { return filterPopupMode; }
			set {
				if(FilterPopupMode == value) return;
				FilterPopupMode prevValue = FilterPopupMode;
				filterPopupMode = value;
				OnChanged(new BaseOptionChangedEventArgs(FilterPopupModeName, prevValue, FilterPopupMode));
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsColumnFilterShowEmptyDateFilter"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool ShowEmptyDateFilter {
			get { return showEmptyDateFilter; }
			set {
				if(ShowEmptyDateFilter == value) return;
				showEmptyDateFilter = value;
				BooleanChanged(ShowEmptyDateFilterName, ShowEmptyDateFilter);
			}
		}
		[ DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public virtual DefaultBoolean ImmediateUpdatePopupDateFilter {
			get { return immediateUpdatePopupDateFilter; }
			set {
				if(ImmediateUpdatePopupDateFilter == value) return;
				DefaultBoolean prevValue = ImmediateUpdatePopupDateFilter;
				immediateUpdatePopupDateFilter = value;
				OnChanged(new BaseOptionChangedEventArgs(ImmediateUpdatePopupDateFilterName, prevValue, ImmediateUpdatePopupDateFilter));
			}
		}
		[ DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public virtual DefaultBoolean ShowBlanksFilterItems {
			get { return showBlanksFilterItems; }
			set {
				if(ShowBlanksFilterItems == value) return;
				DefaultBoolean prevValue = ShowBlanksFilterItems;
				showBlanksFilterItems = value;
				OnChanged(new BaseOptionChangedEventArgs(ShowBlanksFilterItemsName, prevValue, ShowBlanksFilterItems));
			}
		}
		protected internal const string
			AutoFilterConditionName = "AutoFilterCondition",
			AllowAutoFilterName = "AllowAutoFilter",
			ImmediateUpdateAutoFilterName = "ImmediateUpdateAutoFilter",
			AllowFilterName = "AllowFilter",
			FilterPopupModeName = "FilterPopupMode",
			ShowEmptyDateFilterName = "ShowEmptyDateFilter",
			ImmediateUpdatePopupDateFilterName = "ImmediateUpdatePopupDateFilter",
			ShowBlanksFilterItemsName = "ShowBlanksFilterItems";
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				TreeListOptionsColumnFilter opt = options as TreeListOptionsColumnFilter;
				if(opt == null) return;
				this.autoFilterCondition = opt.AutoFilterCondition;
				this.allowAutoFilter = opt.AllowAutoFilter;
				this.immediateUpdateAutoFilter = opt.ImmediateUpdateAutoFilter;
				this.allowFilter = opt.AllowFilter;
				this.filterPopupMode = opt.FilterPopupMode;
				this.showEmptyDateFilter = opt.ShowEmptyDateFilter;
				this.immediateUpdateAutoFilter = opt.ImmediateUpdateAutoFilter;
				this.showBlanksFilterItems = opt.ShowBlanksFilterItems;
			}
			finally {
				EndUpdate();
			}
		}
	}
	public class TreeListOptionsFind : BaseTreeListOptions {
		const int DefaultFindDelay = 1000, MinFindDelay = 100;
		public const string DefaultFilterColumnsString = "*";
		string findFilterColumns, findNullPrompt;
		bool allowFindPanel, showClearButton, showFindButton, showCloseButton, clearFindOnClose, highlightFindResults, alwaysVisible;
		int findDelay;
		FindMode findMode;
		public TreeListOptionsFind() {
			this.allowFindPanel = this.alwaysVisible = false;
			this.showClearButton = this.showFindButton = this.showCloseButton = this.clearFindOnClose = this.highlightFindResults = true;
			this.findDelay = DefaultFindDelay;
			this.findFilterColumns = DefaultFilterColumnsString;
			this.findMode = FindMode.Default;
			this.findNullPrompt = DefaultFindNullPrompt;
		}
		string DefaultFindNullPrompt { get { return TreeListLocalizer.Active.GetLocalizedString(TreeListStringId.FindNullPrompt); } }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsFindAllowFindPanel"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool AllowFindPanel {
			get { return allowFindPanel; }
			set {
				if(AllowFindPanel == value) return;
				allowFindPanel = value;
				BooleanChanged(AllowFindPanelName, AllowFindPanel);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsFindShowClearButton"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowClearButton {
			get { return showClearButton; }
			set {
				if(ShowClearButton == value) return;
				showClearButton = value;
				BooleanChanged(ShowClearButtonName, ShowClearButton);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsFindShowFindButton"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowFindButton {
			get { return showFindButton; }
			set {
				if(ShowFindButton == value) return;
				showFindButton = value;
				BooleanChanged(ShowFindButtonName, ShowFindButton);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsFindShowCloseButton"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowCloseButton {
			get { return showCloseButton; }
			set {
				if(ShowCloseButton == value) return;
				showCloseButton = value;
				BooleanChanged(ShowCloseButtonName, ShowCloseButton);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsFindClearFindOnClose"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ClearFindOnClose {
			get { return clearFindOnClose; }
			set {
				if(ClearFindOnClose == value) return;
				clearFindOnClose = value;
				BooleanChanged(ClearFindOnCloseName, ClearFindOnClose);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsFindFindDelay"),
#endif
 DefaultValue(DefaultFindDelay), XtraSerializableProperty()]
		public virtual int FindDelay {
			get { return findDelay; }
			set {
				if(value < MinFindDelay) value = MinFindDelay;
				if(FindDelay == value) return;
				int prevValue = FindDelay;
				findDelay = value;
				OnChanged(new BaseOptionChangedEventArgs(FindDelayName, prevValue, FindDelay));
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsFindFindFilterColumns"),
#endif
 DefaultValue(DefaultFilterColumnsString), XtraSerializableProperty()]
		public virtual string FindFilterColumns {
			get { return findFilterColumns; }
			set {
				if(value == null) value = string.Empty;
				if(FindFilterColumns == value) return;
				string prevValue = FindFilterColumns;
				findFilterColumns = value;
				OnChanged(new BaseOptionChangedEventArgs(FindFilterColumnsName, prevValue, FindFilterColumns));
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsFindHighlightFindResults"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool HighlightFindResults {
			get { return highlightFindResults; }
			set {
				if(HighlightFindResults == value) return;
				highlightFindResults = value;
				BooleanChanged(HighlightFindResultsName, HighlightFindResults);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsFindAlwaysVisible"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool AlwaysVisible {
			get { return alwaysVisible; }
			set {
				if(AlwaysVisible == value) return;
				alwaysVisible = value;
				BooleanChanged(AlwaysVisibleName, AlwaysVisible);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsFindFindMode"),
#endif
 DefaultValue(FindMode.Default)]
		public FindMode FindMode {
			get { return findMode; }
			set {
				if(FindMode == value) return;
				FindMode prevValue = FindMode;
				findMode = value;
				OnChanged(new BaseOptionChangedEventArgs(FindModeName, prevValue, FindMode));
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsFindFindNullPrompt"),
#endif
 XtraSerializableProperty()]
		public string FindNullPrompt {
			get { return findNullPrompt; }
			set {
				if(value == null) value = "";
				if(FindNullPrompt == value) return;
				string prevValue = FindNullPrompt;
				findNullPrompt = value;
				OnChanged(new BaseOptionChangedEventArgs(FindNullPromptName, prevValue, FindNullPrompt));
			}
		}
		bool ShouldSerializeFindNullPrompt() { return FindNullPrompt != DefaultFindNullPrompt; }
		void ResetFindNullPrompt() { FindNullPrompt = DefaultFindNullPrompt; }
		protected internal const string
			FindDelayName = "FindDelay",
			AllowFindPanelName = "AllowFindPanel",
			ShowCloseButtonName = "ShowCloseButton",
			ShowClearButtonName = "ShowClearButton",
			ShowFindButtonName = "ShowFindButton",
			ClearFindOnCloseName = "ClearFindOnClose",
			FindFilterColumnsName = "FindFilterColumns",
			HighlightFindResultsName = "HighlightFindResults",
			AlwaysVisibleName = "AlwaysVisible",
			FindModeName = "FindMode",
			FindNullPromptName = "FindNullPrompt";
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				TreeListOptionsFind opt = options as TreeListOptionsFind;
				if(opt == null) return;
				this.allowFindPanel = opt.AllowFindPanel;
				this.showCloseButton = opt.ShowCloseButton;
				this.showFindButton = opt.ShowFindButton;
				this.showClearButton = opt.ShowClearButton;
				this.findDelay = opt.FindDelay;
				this.findFilterColumns = opt.FindFilterColumns;
				this.highlightFindResults = opt.HighlightFindResults;
				this.alwaysVisible = opt.AlwaysVisible;
				this.findMode = opt.FindMode;
				this.findNullPrompt = opt.FindNullPrompt;
			}
			finally {
				EndUpdate();
			}
		}
	}
	public class TreeListOptionsBand : BaseTreeListOptions {
		bool allowMove, allowSize, showInCustomizationForm, fixedWidth;
		public TreeListOptionsBand() {
			this.allowMove = this.allowSize = this.showInCustomizationForm = true;
			this.fixedWidth = false;
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsBandShowInCustomizationForm"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool ShowInCustomizationForm {
			get { return showInCustomizationForm; }
			set {
				if(ShowInCustomizationForm == value) return;
				showInCustomizationForm = value;
				BooleanChanged(ShowInCustomizationFormName, ShowInCustomizationForm);
			}
		}
		[ DefaultValue(true), XtraSerializableProperty()]
		public bool AllowSize {
			get { return allowSize; }
			set {
				if(AllowSize == value) return;
				allowSize = value;
				BooleanChanged(AllowSizeName, AllowSize);
			}
		}
		[ DefaultValue(true), XtraSerializableProperty()]
		public bool AllowMove {
			get { return allowMove; }
			set {
				if(AllowMove == value) return;
				allowMove = value;
				BooleanChanged(AllowMoveName, AllowMove);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListOptionsBandFixedWidth"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public bool FixedWidth {
			get { return fixedWidth; }
			set {
				if(FixedWidth == value) return;
				fixedWidth = value;
				BooleanChanged(FixedWidthName, FixedWidth);
			}
		}
		protected internal const string
		   ShowInCustomizationFormName = "ShowInCustomizationForm",
		   AllowSizeName = "AllowSize",
		   AllowMoveName = "AllowMove",
		   FixedWidthName = "FixedWidth";
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				TreeListOptionsBand opt = options as TreeListOptionsBand;
				if(opt == null) return;
				this.showInCustomizationForm = opt.ShowInCustomizationForm;
				this.allowSize = opt.AllowSize;
				this.allowMove = opt.AllowMove;
				this.fixedWidth = opt.FixedWidth;
			}
			finally {
				EndUpdate();
			}
		}
	}		
}
