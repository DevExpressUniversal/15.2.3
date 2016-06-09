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
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Card;
using System.ComponentModel;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Controls;
using DevExpress.XtraGrid.Views;
using DevExpress.XtraGrid.Views.Layout.ViewInfo;
using DevExpress.XtraLayout;
using System.Drawing.Drawing2D;
using System.Drawing;
using DevExpress.XtraLayout.Utils;
namespace DevExpress.XtraGrid.Views.Layout {
	public class LayoutViewOptionsHeaderPanel : ViewBaseOptions {
		LayoutView viewCore = null;
		bool showSingleModeButtonCore;
		bool showRowModeButtonCore;
		bool showColumnModeButtonCore;
		bool showMultiRowModeButtonCore;
		bool showMultiColumnModeButtonCore;
		bool showCarouselModeButtonCore;
		bool showPanButtonCore;
		bool showCustomizeButtonCore;
		bool enableSingleModeButtonCore;
		bool enableRowModeButtonCore;
		bool enableColumnModeButtonCore;
		bool enableMultiRowModeButtonCore;
		bool enableMultiColumnModeButtonCore;
		bool enableCarouselModeButtonCore;
		bool enablePanButtonCore;
		bool enableCustomizeButtonCore;
		public LayoutViewOptionsHeaderPanel(LayoutView view) {
			this.viewCore = view;
			this.showSingleModeButtonCore = true;
			this.showRowModeButtonCore = true;
			this.showColumnModeButtonCore = true;
			this.showMultiRowModeButtonCore = true;
			this.showMultiColumnModeButtonCore = true;
			this.showCarouselModeButtonCore = true;
			this.showPanButtonCore = true;
			this.showCustomizeButtonCore = true;
			this.enableSingleModeButtonCore = true;
			this.enableRowModeButtonCore = true;
			this.enableColumnModeButtonCore = true;
			this.enableMultiRowModeButtonCore = true;
			this.enableMultiColumnModeButtonCore = true;
			this.enableCarouselModeButtonCore = true;
			this.enablePanButtonCore = true;
			this.enableCustomizeButtonCore = true;
		}
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			LayoutViewOptionsHeaderPanel source = options as LayoutViewOptionsHeaderPanel;
			this.showSingleModeButtonCore = source.showSingleModeButtonCore;
			this.showRowModeButtonCore = source.showRowModeButtonCore;
			this.showColumnModeButtonCore = source.showColumnModeButtonCore;
			this.showMultiRowModeButtonCore = source.showMultiRowModeButtonCore;
			this.showMultiColumnModeButtonCore = source.showMultiColumnModeButtonCore;
			this.showCarouselModeButtonCore = source.showCarouselModeButtonCore;
			this.showPanButtonCore = source.showPanButtonCore;
			this.showCustomizeButtonCore = source.showCustomizeButtonCore;
			this.enableSingleModeButtonCore = source.enableSingleModeButtonCore;
			this.enableRowModeButtonCore = source.enableRowModeButtonCore;
			this.enableColumnModeButtonCore = source.enableColumnModeButtonCore;
			this.enableMultiRowModeButtonCore = source.enableMultiRowModeButtonCore;
			this.enableMultiColumnModeButtonCore = source.enableMultiColumnModeButtonCore;
			this.enableCarouselModeButtonCore = source.enableCarouselModeButtonCore;
			this.enablePanButtonCore = source.enablePanButtonCore;
			this.enableCustomizeButtonCore = source.enableCustomizeButtonCore;
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsHeaderPanelShowSingleModeButton"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool ShowSingleModeButton {
			get { return showSingleModeButtonCore; }
			set {
				if(ShowSingleModeButton == value) return;
				bool prevValue = ShowSingleModeButton;
				showSingleModeButtonCore = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowSingleModeButton", prevValue, ShowSingleModeButton));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsHeaderPanelEnableSingleModeButton"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool EnableSingleModeButton {
			get { return enableSingleModeButtonCore; }
			set {
				if(EnableSingleModeButton == value) return;
				bool prevValue = EnableSingleModeButton;
				enableSingleModeButtonCore = value;
				OnChanged(new BaseOptionChangedEventArgs("EnableSingleModeButton", prevValue, EnableSingleModeButton));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsHeaderPanelShowRowModeButton"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool ShowRowModeButton {
			get { return showRowModeButtonCore; }
			set {
				if(ShowRowModeButton == value) return;
				bool prevValue = ShowRowModeButton;
				showRowModeButtonCore = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowRowModeButton", prevValue, ShowRowModeButton));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsHeaderPanelEnableRowModeButton"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool EnableRowModeButton {
			get { return enableRowModeButtonCore; }
			set {
				if(EnableRowModeButton == value) return;
				bool prevValue = EnableRowModeButton;
				enableRowModeButtonCore = value;
				OnChanged(new BaseOptionChangedEventArgs("EnableRowModeButton", prevValue, EnableRowModeButton));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsHeaderPanelShowColumnModeButton"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool ShowColumnModeButton {
			get { return showColumnModeButtonCore; }
			set {
				if(ShowColumnModeButton == value) return;
				bool prevValue = ShowColumnModeButton;
				showColumnModeButtonCore = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowColumnModeButton", prevValue, ShowColumnModeButton));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsHeaderPanelEnableColumnModeButton"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool EnableColumnModeButton {
			get { return enableColumnModeButtonCore; }
			set {
				if(EnableColumnModeButton == value) return;
				bool prevValue = EnableColumnModeButton;
				enableColumnModeButtonCore = value;
				OnChanged(new BaseOptionChangedEventArgs("EnableColumnModeButton", prevValue, EnableColumnModeButton));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsHeaderPanelShowMultiRowModeButton"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool ShowMultiRowModeButton {
			get { return showMultiRowModeButtonCore; }
			set {
				if(ShowMultiRowModeButton == value) return;
				bool prevValue = ShowMultiRowModeButton;
				showMultiRowModeButtonCore = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowMultiRowModeButton", prevValue, ShowMultiRowModeButton));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsHeaderPanelEnableMultiRowModeButton"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool EnableMultiRowModeButton {
			get { return enableMultiRowModeButtonCore; }
			set {
				if(EnableMultiRowModeButton == value) return;
				bool prevValue = EnableMultiRowModeButton;
				enableMultiRowModeButtonCore = value;
				OnChanged(new BaseOptionChangedEventArgs("EnableMultiRowModeButton", prevValue, EnableMultiRowModeButton));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsHeaderPanelShowMultiColumnModeButton"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool ShowMultiColumnModeButton {
			get { return showMultiColumnModeButtonCore; }
			set {
				if(ShowMultiColumnModeButton == value) return;
				bool prevValue = ShowMultiColumnModeButton;
				showMultiColumnModeButtonCore = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowMultiColumnModeButton", prevValue, ShowMultiColumnModeButton));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsHeaderPanelEnableMultiColumnModeButton"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool EnableMultiColumnModeButton {
			get { return enableMultiColumnModeButtonCore; }
			set {
				if(EnableMultiColumnModeButton == value) return;
				bool prevValue = EnableMultiColumnModeButton;
				enableMultiColumnModeButtonCore = value;
				OnChanged(new BaseOptionChangedEventArgs("EnableMultiColumnModeButton", prevValue, EnableMultiColumnModeButton));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsHeaderPanelShowCarouselModeButton"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool ShowCarouselModeButton {
			get { return showCarouselModeButtonCore; }
			set {
				if(ShowCarouselModeButton == value) return;
				bool prevValue = ShowCarouselModeButton;
				showCarouselModeButtonCore = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowCarouselModeButton", prevValue, ShowCarouselModeButton));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsHeaderPanelEnableCarouselModeButton"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool EnableCarouselModeButton {
			get { return enableCarouselModeButtonCore; }
			set {
				if(EnableCarouselModeButton == value) return;
				bool prevValue = EnableCarouselModeButton;
				enableCarouselModeButtonCore = value;
				OnChanged(new BaseOptionChangedEventArgs("EnableCarouselModeButton", prevValue, EnableCarouselModeButton));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsHeaderPanelShowPanButton"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool ShowPanButton {
			get { return showPanButtonCore; }
			set {
				if(ShowPanButton == value) return;
				bool prevValue = ShowPanButton;
				showPanButtonCore = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowPanButton", prevValue, ShowPanButton));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsHeaderPanelEnablePanButton"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool EnablePanButton {
			get { return enablePanButtonCore; }
			set {
				if(EnablePanButton == value) return;
				bool prevValue = EnablePanButton;
				enablePanButtonCore = value;
				OnChanged(new BaseOptionChangedEventArgs("EnablePanButton", prevValue, EnablePanButton));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsHeaderPanelShowCustomizeButton"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool ShowCustomizeButton {
			get { return showCustomizeButtonCore; }
			set {
				if(ShowCustomizeButton == value) return;
				bool prevValue = ShowCustomizeButton;
				showCustomizeButtonCore = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowCustomizeButton", prevValue, ShowCustomizeButton));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsHeaderPanelEnableCustomizeButton"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool EnableCustomizeButton {
			get { return enableCustomizeButtonCore; }
			set {
				if(EnableCustomizeButton == value) return;
				bool prevValue = EnableCustomizeButton;
				enableCustomizeButtonCore = value;
				OnChanged(new BaseOptionChangedEventArgs("EnableCustomizeButton", prevValue, EnableCustomizeButton));
			}
		}
	}
	public class LayoutViewOptionsCustomization : ViewBaseOptions {
		LayoutView viewCore = null;
		bool allowSortCore;
		bool allowFilterCore;
		bool useAdvancedRuntimeCustomizationCore;
		bool showGroupHiddenItemsCore;
		bool showGroupLayoutTreeViewCore;
		bool showGroupPropertyGridCore;
		bool showUndoRedoButtonsCore;
		bool showResetShrinkButtonsCore;
		bool showGroupCardIndentsCore;
		bool showGroupCardCaptionsCore;
		bool showSaveLoadLayoutButtonsCore;
		bool showGroupViewCore;
		bool showGroupLayoutCore;
		bool showGroupCardsCore;
		bool showGroupFieldsCore;
		public LayoutViewOptionsCustomization(LayoutView view) {
			this.viewCore = view;
			this.allowSortCore = true;
			this.allowFilterCore = true;
			this.useAdvancedRuntimeCustomizationCore = false;
			this.showGroupHiddenItemsCore = true;
			this.showGroupLayoutTreeViewCore = true;
			this.showGroupPropertyGridCore = true;
			this.showUndoRedoButtonsCore = true;
			this.showResetShrinkButtonsCore = true;
			this.showGroupCardIndentsCore = true;
			this.showGroupCardCaptionsCore = true;
			this.showSaveLoadLayoutButtonsCore = true;
			this.showGroupViewCore = true;
			this.showGroupLayoutCore = true;
			this.showGroupCardsCore = true;
			this.showGroupFieldsCore = true;
		}
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			LayoutViewOptionsCustomization source = options as LayoutViewOptionsCustomization;
			if(source == null) return;
			this.allowSortCore = source.allowSortCore;
			this.allowFilterCore = source.allowFilterCore;
			this.useAdvancedRuntimeCustomizationCore = source.useAdvancedRuntimeCustomizationCore;
			this.showGroupHiddenItemsCore = source.showGroupHiddenItemsCore;
			this.showGroupLayoutTreeViewCore = source.showGroupLayoutTreeViewCore;
			this.showGroupPropertyGridCore = source.showGroupPropertyGridCore;
			this.showUndoRedoButtonsCore = source.showUndoRedoButtonsCore;
			this.showResetShrinkButtonsCore = source.showResetShrinkButtonsCore;
			this.showGroupCardIndentsCore = source.showGroupCardIndentsCore;
			this.showGroupCardCaptionsCore = source.showGroupCardCaptionsCore;
			this.showSaveLoadLayoutButtonsCore = source.showSaveLoadLayoutButtonsCore;
			this.showGroupViewCore = source.showGroupViewCore;
			this.showGroupLayoutCore = source.showGroupLayoutCore;
			this.showGroupCardsCore = source.showGroupCardsCore;
			this.showGroupFieldsCore = source.showGroupFieldsCore;
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsCustomizationAllowSort"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool AllowSort {
			get { return allowSortCore; }
			set {
				if(AllowSort == value) return;
				bool prevValue = AllowSort;
				allowSortCore = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowSort", prevValue, AllowSort));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsCustomizationAllowFilter"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool AllowFilter {
			get { return allowFilterCore; }
			set {
				if(AllowFilter == value) return;
				bool prevValue = AllowFilter;
				allowFilterCore = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowFilter", prevValue, AllowFilter));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsCustomizationUseAdvancedRuntimeCustomization"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public bool UseAdvancedRuntimeCustomization {
			get { return useAdvancedRuntimeCustomizationCore; } 
			set {
				if(UseAdvancedRuntimeCustomization == value) return;
				bool prevValue = UseAdvancedRuntimeCustomization;
				useAdvancedRuntimeCustomizationCore = value;
				OnChanged(new BaseOptionChangedEventArgs("UseAdvancedRuntimeCustomization", prevValue, UseAdvancedRuntimeCustomization));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsCustomizationShowGroupLayoutTreeView"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool ShowGroupLayoutTreeView {
			get { return showGroupLayoutTreeViewCore; }
			set {
				if(ShowGroupLayoutTreeView == value) return;
				bool prevValue = ShowGroupLayoutTreeView;
				showGroupLayoutTreeViewCore = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowGroupLayoutTreeView", prevValue, ShowGroupLayoutTreeView));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsCustomizationShowGroupHiddenItems"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool ShowGroupHiddenItems {
			get { return showGroupHiddenItemsCore; }
			set {
				if(ShowGroupHiddenItems == value) return;
				bool prevValue = ShowGroupHiddenItems;
				showGroupHiddenItemsCore = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowGroupHiddenItems", prevValue, ShowGroupHiddenItems));
			}
		}
		protected internal bool ShowGroupPropertyGrid {
			get { return showGroupPropertyGridCore; }
			set {
				if(ShowGroupPropertyGrid == value) return;
				bool prevValue = ShowGroupPropertyGrid;
				showGroupPropertyGridCore = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowGroupPropertyGrid", prevValue, ShowGroupPropertyGrid));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsCustomizationShowResetShrinkButtons"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool ShowResetShrinkButtons {
			get { return showResetShrinkButtonsCore; }
			set {
				if(ShowResetShrinkButtons == value) return;
				bool prevValue = ShowResetShrinkButtons;
				showResetShrinkButtonsCore = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowResetShrinkButtons", prevValue, ShowResetShrinkButtons));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsCustomizationShowGroupCardIndents"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool ShowGroupCardIndents {
			get { return showGroupCardIndentsCore; }
			set {
				if(ShowGroupCardIndents == value) return;
				bool prevValue = ShowGroupCardIndents;
				showGroupCardIndentsCore = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowGroupCardIndents", prevValue, ShowGroupCardIndents));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsCustomizationShowGroupCardCaptions"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool ShowGroupCardCaptions {
			get { return showGroupCardCaptionsCore; }
			set {
				if(ShowGroupCardCaptions == value) return;
				bool prevValue = ShowGroupCardCaptions;
				showGroupCardCaptionsCore = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowGroupCardCaptions", prevValue, ShowGroupCardCaptions));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsCustomizationShowSaveLoadLayoutButtons"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool ShowSaveLoadLayoutButtons {
			get { return showSaveLoadLayoutButtonsCore; }
			set {
				if(ShowSaveLoadLayoutButtons == value) return;
				bool prevValue = ShowSaveLoadLayoutButtons;
				showSaveLoadLayoutButtonsCore = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowSaveLoadLayoutButtons", prevValue, ShowSaveLoadLayoutButtons));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsCustomizationShowGroupView"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool ShowGroupView {
			get { return showGroupViewCore; }
			set {
				if(ShowGroupView == value) return;
				bool prevValue = ShowGroupView;
				showGroupViewCore = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowGroupView", prevValue, ShowGroupView));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsCustomizationShowGroupLayout"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool ShowGroupLayout {
			get { return showGroupLayoutCore; }
			set {
				if(ShowGroupLayout == value) return;
				bool prevValue = ShowGroupLayout;
				showGroupLayoutCore = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowGroupLayout", prevValue, ShowGroupLayout));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsCustomizationShowGroupCards"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool ShowGroupCards {
			get { return showGroupCardsCore; }
			set {
				if(ShowGroupCards == value) return;
				bool prevValue = ShowGroupCards;
				showGroupCardsCore = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowGroupCards", prevValue, ShowGroupCards));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsCustomizationShowGroupFields"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool ShowGroupFields {
			get { return showGroupFieldsCore; }
			set {
				if(ShowGroupFields == value) return;
				bool prevValue = ShowGroupFields;
				showGroupFieldsCore = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowGroupFields", prevValue, ShowGroupFields));
			}
		}
	}
	public enum ScrollBarOrientation { Default, Horizontal, Vertical }
	public class LayoutViewOptionsMultiRecordMode : ViewBaseOptions {
		LayoutView viewCore = null;
		bool stretchCardToViewWidthCore;
		bool stretchCardToViewHeightCore;
		int maxCardRowsCore;
		int maxCardColumnsCore;
		ScrollBarOrientation multiRowScrollOrientationCore;
		ScrollBarOrientation multiColumnScrollOrientationCore;
		public LayoutViewOptionsMultiRecordMode(LayoutView view) {
			this.viewCore = view;
			this.stretchCardToViewWidthCore = false;
			this.stretchCardToViewHeightCore = false;
			this.maxCardRowsCore = 0;
			this.maxCardColumnsCore = 0;
			this.multiRowScrollOrientationCore = ScrollBarOrientation.Default;
			this.multiColumnScrollOrientationCore = ScrollBarOrientation.Default;
		}
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			LayoutViewOptionsMultiRecordMode source = options as LayoutViewOptionsMultiRecordMode;
			this.stretchCardToViewWidthCore = source.stretchCardToViewWidthCore;
			this.stretchCardToViewHeightCore = source.stretchCardToViewHeightCore;
			this.maxCardRowsCore = source.maxCardRowsCore;
			this.maxCardColumnsCore = source.maxCardColumnsCore;
			this.multiRowScrollOrientationCore = source.multiRowScrollOrientationCore;
			this.multiColumnScrollOrientationCore = source.multiColumnScrollOrientationCore;
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsMultiRecordModeMultiColumnScrollBarOrientation"),
#endif
 DefaultValue(ScrollBarOrientation.Default), XtraSerializableProperty()]
		public virtual ScrollBarOrientation MultiColumnScrollBarOrientation {
			get { return multiColumnScrollOrientationCore; }
			set {
				if(MultiColumnScrollBarOrientation == value) return;
				ScrollBarOrientation prevValue = MultiColumnScrollBarOrientation;
				multiColumnScrollOrientationCore = value;
				OnChanged(new BaseOptionChangedEventArgs("MultiColumnScrollBarOrientation", prevValue, MultiColumnScrollBarOrientation));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsMultiRecordModeMultiRowScrollBarOrientation"),
#endif
 DefaultValue(ScrollBarOrientation.Default), XtraSerializableProperty()]
		public virtual ScrollBarOrientation MultiRowScrollBarOrientation {
			get { return multiRowScrollOrientationCore; }
			set {
				if(MultiRowScrollBarOrientation == value) return;
				ScrollBarOrientation prevValue = MultiRowScrollBarOrientation;
				multiRowScrollOrientationCore = value;
				OnChanged(new BaseOptionChangedEventArgs("MultiRowScrollBarOrientation", prevValue, MultiRowScrollBarOrientation));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsMultiRecordModeStretchCardToViewWidth"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool StretchCardToViewWidth {
			get { return stretchCardToViewWidthCore; }
			set {
				if(StretchCardToViewWidth == value) return;
				bool prevValue = StretchCardToViewWidth;
				stretchCardToViewWidthCore = value;
				OnChanged(new BaseOptionChangedEventArgs("StretchCardToViewWidth", prevValue, StretchCardToViewWidth));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsMultiRecordModeStretchCardToViewHeight"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool StretchCardToViewHeight {
			get { return stretchCardToViewHeightCore; }
			set {
				if(StretchCardToViewHeight == value) return;
				bool prevValue = StretchCardToViewHeight;
				stretchCardToViewHeightCore = value;
				OnChanged(new BaseOptionChangedEventArgs("StretchCardToViewHeight", prevValue, StretchCardToViewHeight));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsMultiRecordModeMaxCardRows"),
#endif
 DefaultValue(0), XtraSerializableProperty()]
		public virtual int MaxCardRows {
			get { return maxCardRowsCore; }
			set {
				if(MaxCardRows == value) return;
				int prevValue = MaxCardRows;
				maxCardRowsCore = value;
				OnChanged(new BaseOptionChangedEventArgs("MaxCardRows", prevValue, MaxCardRows));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsMultiRecordModeMaxCardColumns"),
#endif
 DefaultValue(0), XtraSerializableProperty()]
		public virtual int MaxCardColumns {
			get { return maxCardColumnsCore; }
			set {
				if(MaxCardColumns == value) return;
				int prevValue = MaxCardColumns;
				maxCardColumnsCore = value;
				OnChanged(new BaseOptionChangedEventArgs("MaxCardColumns", prevValue, MaxCardColumns));
			}
		}
	}
	public class LayoutViewOptionsSingleRecordMode : ViewBaseOptions {
		LayoutView viewCore = null;
		CardsAlignment cardAlignmentCore;
		bool stretchCardToViewWidthCore;
		bool stretchCardToViewHeightCore;
		public LayoutViewOptionsSingleRecordMode(LayoutView view) {
			this.viewCore = view;
			this.stretchCardToViewWidthCore = false;
			this.stretchCardToViewHeightCore = false;
			this.cardAlignmentCore = CardsAlignment.Center;
		}
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			LayoutViewOptionsSingleRecordMode source = options as LayoutViewOptionsSingleRecordMode;
			this.stretchCardToViewWidthCore = source.stretchCardToViewWidthCore;
			this.stretchCardToViewHeightCore = source.stretchCardToViewHeightCore;
			this.cardAlignmentCore = source.cardAlignmentCore;
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsSingleRecordModeStretchCardToViewWidth"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool StretchCardToViewWidth {
			get { return stretchCardToViewWidthCore; }
			set {
				if(StretchCardToViewWidth == value) return;
				bool prevValue = StretchCardToViewWidth;
				stretchCardToViewWidthCore = value;
				OnChanged(new BaseOptionChangedEventArgs("StretchCardToViewWidth", prevValue, StretchCardToViewWidth));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsSingleRecordModeStretchCardToViewHeight"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool StretchCardToViewHeight {
			get { return stretchCardToViewHeightCore; }
			set {
				if(StretchCardToViewHeight == value) return;
				bool prevValue = StretchCardToViewHeight;
				stretchCardToViewHeightCore = value;
				OnChanged(new BaseOptionChangedEventArgs("StretchCardToViewHeight", prevValue, StretchCardToViewHeight));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsSingleRecordModeCardAlignment"),
#endif
 DefaultValue(CardsAlignment.Center), XtraSerializableProperty()]
		public virtual CardsAlignment CardAlignment {
			get { return cardAlignmentCore; }
			set {
				if(CardAlignment == value) return;
				CardsAlignment prevValue = CardAlignment;
				cardAlignmentCore = value;
				OnChanged(new BaseOptionChangedEventArgs("CardAlignment", prevValue, CardAlignment));
			}
		}
	}
	public class LayoutViewOptionsCarouselMode : ViewBaseOptions {
		LayoutView viewCore = null;
		int cardCountCore;
		int radiusCore;
		float pitchAngleCore;
		float bottomCardScaleCore;
		float bottomCardAlphaLevelCore;
		float bottomCardFadingCore;
		float rollAngleCore;
		int frameCountCore;
		int frameDelayCore;
		Point centerOffsetCore;
		InterpolationMode interpolationModeCore;
		bool stretchCardToViewWidthCore;
		bool stretchCardToViewHeightCore;
		public LayoutViewOptionsCarouselMode(LayoutView view) {
			this.viewCore = view;
			this.cardCountCore = 15;
			this.pitchAngleCore = (float)Math.PI / 2;
			this.radiusCore = 0;
			this.bottomCardScaleCore = 0.2f;
			this.bottomCardAlphaLevelCore = 0.0f;
			this.bottomCardFadingCore = 0.0f;
			this.rollAngleCore = (float)Math.PI;
			this.frameCountCore = 250;
			this.frameDelayCore = 10000;
			this.interpolationModeCore = InterpolationMode.Default;
			this.centerOffsetCore = Point.Empty;
			this.stretchCardToViewWidthCore = false;
			this.stretchCardToViewHeightCore = false;
		}
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			LayoutViewOptionsCarouselMode source = options as LayoutViewOptionsCarouselMode;
			this.cardCountCore = source.cardCountCore;
			this.pitchAngleCore = source.pitchAngleCore;
			this.radiusCore = source.radiusCore;
			this.bottomCardScaleCore = source.bottomCardScaleCore;
			this.bottomCardAlphaLevelCore = source.bottomCardAlphaLevelCore;
			this.bottomCardFadingCore = source.bottomCardFadingCore;
			this.rollAngleCore = source.rollAngleCore;
			this.frameCountCore = source.frameCountCore;
			this.frameDelayCore = source.frameDelayCore;
			this.interpolationModeCore = source.interpolationModeCore;
			this.stretchCardToViewWidthCore = source.stretchCardToViewWidthCore;
			this.stretchCardToViewHeightCore = source.stretchCardToViewHeightCore;
			this.centerOffsetCore = source.centerOffsetCore;
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsCarouselModeStretchCardToViewWidth"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool StretchCardToViewWidth {
			get { return stretchCardToViewWidthCore; }
			set {
				if(StretchCardToViewWidth == value) return;
				bool prevValue = StretchCardToViewWidth;
				stretchCardToViewWidthCore = value;
				OnChanged(new BaseOptionChangedEventArgs("StretchCardToViewWidth", prevValue, StretchCardToViewWidth));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsCarouselModeStretchCardToViewHeight"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool StretchCardToViewHeight {
			get { return stretchCardToViewHeightCore; }
			set {
				if(StretchCardToViewHeight == value) return;
				bool prevValue = StretchCardToViewHeight;
				stretchCardToViewHeightCore = value;
				OnChanged(new BaseOptionChangedEventArgs("StretchCardToViewHeight", prevValue, StretchCardToViewHeight));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsCarouselModeInterpolationMode"),
#endif
 DefaultValue(InterpolationMode.Default), XtraSerializableProperty()]
		public InterpolationMode InterpolationMode {
			get { return interpolationModeCore; }
			set {
				if(InterpolationMode == value) return;
				InterpolationMode prevValue = InterpolationMode;
				interpolationModeCore = value;
				OnChanged(new BaseOptionChangedEventArgs("InterpolationMode", prevValue, InterpolationMode));
			}
		}
		bool ShouldSerializeCenterOffset() { return CenterOffset != Point.Empty; }
		void ResetCenterOffset() {
			CenterOffset = Point.Empty;
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsCarouselModeCenterOffset"),
#endif
 XtraSerializableProperty()]
		public Point CenterOffset {
			get { return centerOffsetCore; }
			set {
				if(CenterOffset == value) return;
				Point prevValue = CenterOffset;
				centerOffsetCore = value;
				OnChanged(new BaseOptionChangedEventArgs("CenterOffset", prevValue, CenterOffset));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsCarouselModeFrameCount"),
#endif
 DefaultValue(250), XtraSerializableProperty()]
		public int FrameCount {
			get { return frameCountCore; }
			set {
				if(FrameCount == value) return;
				int prevValue = FrameCount;
				frameCountCore = value;
				OnChanged(new BaseOptionChangedEventArgs("FrameCount", prevValue, FrameCount));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsCarouselModeFrameDelay"),
#endif
 DefaultValue(10000), XtraSerializableProperty()]
		public int FrameDelay {
			get { return frameDelayCore; }
			set {
				if(FrameDelay == value) return;
				int prevValue = FrameDelay;
				frameDelayCore = value;
				OnChanged(new BaseOptionChangedEventArgs("FrameDelay", prevValue, FrameDelay));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsCarouselModeCardCount"),
#endif
 DefaultValue(15), XtraSerializableProperty()]
		public int CardCount {
			get { return cardCountCore; }
			set {
				if(CardCount == value) return;
				int prevValue = CardCount;
				cardCountCore = value;
				OnChanged(new BaseOptionChangedEventArgs("RecordCount", prevValue, CardCount));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsCarouselModeRollAngle"),
#endif
 DefaultValue((float)Math.PI), XtraSerializableProperty()]
		public float RollAngle {
			get { return rollAngleCore; }
			set {
				if(RollAngle == value) return;
				float prevValue = RollAngle;
				rollAngleCore = value;
				OnChanged(new BaseOptionChangedEventArgs("RollAngle", prevValue, RollAngle));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsCarouselModeRadius"),
#endif
 DefaultValue(0), XtraSerializableProperty()]
		public int Radius {
			get { return radiusCore; }
			set {
				if(Radius == value) return;
				int prevValue = Radius;
				radiusCore = value;
				OnChanged(new BaseOptionChangedEventArgs("Radius", prevValue, Radius));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsCarouselModePitchAngle"),
#endif
 DefaultValue((float)Math.PI / 2), XtraSerializableProperty()]
		public float PitchAngle {
			get { return pitchAngleCore; }
			set {
				if(PitchAngle == value) return;
				float prevValue = PitchAngle;
				pitchAngleCore = value;
				OnChanged(new BaseOptionChangedEventArgs("PitchAngle", prevValue, PitchAngle));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsCarouselModeBottomCardScale"),
#endif
 DefaultValue(0.2f), XtraSerializableProperty()]
		public float BottomCardScale {
			get { return bottomCardScaleCore; }
			set {
				if(BottomCardScale == value) return;
				float prevValue = BottomCardScale;
				bottomCardScaleCore = value;
				OnChanged(new BaseOptionChangedEventArgs("BottomCardScale", prevValue, BottomCardScale));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsCarouselModeBottomCardFading"),
#endif
 DefaultValue(0.0f), XtraSerializableProperty()]
		public float BottomCardFading {
			get { return bottomCardFadingCore; }
			set {
				if(BottomCardFading == value) return;
				float prevValue = BottomCardFading;
				bottomCardFadingCore = value;
				OnChanged(new BaseOptionChangedEventArgs("BottomCardFading", prevValue, BottomCardFading));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsCarouselModeBottomCardAlphaLevel"),
#endif
 DefaultValue(0.0f), XtraSerializableProperty()]
		public float BottomCardAlphaLevel {
			get { return bottomCardAlphaLevelCore; }
			set {
				if(BottomCardAlphaLevel == value) return;
				float prevValue = BottomCardAlphaLevel;
				bottomCardAlphaLevelCore = value;
				OnChanged(new BaseOptionChangedEventArgs("BottomCardAlphaLevel", prevValue, BottomCardAlphaLevel));
			}
		}
	}
	public enum FieldTextAlignMode { AlignGlobal, AlignInGroups, AutoSize, CustomSize }
	public class LayoutViewOptionsItemText : ViewBaseOptions {
		LayoutView viewCore = null;
		FieldTextAlignMode alignModeCore;
		int textToControlDistanceCore;
		public LayoutViewOptionsItemText(LayoutView view) {
			this.viewCore = view;
			this.alignModeCore = FieldTextAlignMode.AlignInGroups;
			this.textToControlDistanceCore = 5;
		}
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			LayoutViewOptionsItemText source = options as LayoutViewOptionsItemText;
			this.viewCore = source.viewCore;
			this.alignModeCore = source.alignModeCore;
			this.textToControlDistanceCore = source.textToControlDistanceCore;
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsItemTextAlignMode"),
#endif
 DefaultValue(FieldTextAlignMode.AlignInGroups), XtraSerializableProperty()]
		public FieldTextAlignMode AlignMode {
			get { return alignModeCore; }
			set {
				if(AlignMode == value) return;
				FieldTextAlignMode prevValue = AlignMode;
				alignModeCore = value;
				OnChanged(new BaseOptionChangedEventArgs("AlignMode", prevValue, AlignMode));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsItemTextTextToControlDistance"),
#endif
 DefaultValue(5), XtraSerializableProperty()]
		public int TextToControlDistance {
			get { return textToControlDistanceCore; }
			set {
				if(TextToControlDistance == value) return;
				int prevValue = TextToControlDistance;
				textToControlDistanceCore = value;
				OnChanged(new BaseOptionChangedEventArgs("TextToControlDistance", prevValue, TextToControlDistance));
			}
		}
		protected internal FieldTextAlignMode ConvertFrom(TextAlignMode mode) {
			switch(mode) {
				case TextAlignMode.AlignInGroups: return FieldTextAlignMode.AlignInGroups;
				case TextAlignMode.AlignInLayoutControl: return FieldTextAlignMode.AlignGlobal;
				case TextAlignMode.AutoSize: return FieldTextAlignMode.AutoSize;
				default: return FieldTextAlignMode.CustomSize;
			}
		}
		protected internal TextAlignMode ConvertTo(FieldTextAlignMode mode) {
			switch(mode) {
				case FieldTextAlignMode.AlignGlobal: return TextAlignMode.AlignInLayoutControl;
				case FieldTextAlignMode.AlignInGroups: return TextAlignMode.AlignInGroups;
				case FieldTextAlignMode.AutoSize: return TextAlignMode.AutoSize;
				default: return TextAlignMode.CustomSize;
			}
		}
	}
	public class LayoutViewOptionsBehavior : ColumnViewOptionsBehavior {
		bool allowExpandCollapseCore;
		bool allowPanCardsCore;
		bool allowRuntimeCustomizationCore;
		bool allowSwitchViewModesCore;
		bool autoFocusNewCardCore;
		bool autoFocusCardOnScrollingCore;
		bool useTabKeyCore;
		bool overrideLayoutControlRestoreBehaviorCore;
		ScrollVisibility scrollVisibilityCore;
		public LayoutViewOptionsBehavior(LayoutView view)
			: base(view) {
			this.allowExpandCollapseCore = true;
			this.allowPanCardsCore = true;
			this.allowRuntimeCustomizationCore = true;
			this.allowSwitchViewModesCore = true;
			this.autoFocusNewCardCore = false;
			this.autoFocusCardOnScrollingCore = false;
			this.scrollVisibilityCore = ScrollVisibility.Always;
			this.overrideLayoutControlRestoreBehaviorCore = false;
			this.useTabKeyCore = true;
			this.AllowMouseWheelSmoothScrolling = Utils.DefaultBoolean.Default;
		}
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			LayoutViewOptionsBehavior source = options as LayoutViewOptionsBehavior;
			if(source == null) return;
			this.allowExpandCollapseCore = source.allowExpandCollapseCore;
			this.allowPanCardsCore = source.allowPanCardsCore;
			this.allowRuntimeCustomizationCore = source.allowRuntimeCustomizationCore;
			this.allowSwitchViewModesCore = source.allowSwitchViewModesCore;
			this.autoFocusNewCardCore = source.autoFocusNewCardCore;
			this.autoFocusCardOnScrollingCore = source.autoFocusCardOnScrollingCore;
			this.scrollVisibilityCore = source.scrollVisibilityCore;
			this.useTabKeyCore = source.useTabKeyCore;
			this.overrideLayoutControlRestoreBehaviorCore = source.overrideLayoutControlRestoreBehaviorCore;
			this.AllowMouseWheelSmoothScrolling = source.AllowMouseWheelSmoothScrolling;
		}
		[Obsolete("You don't need OverrideLayoutControlRestoreBehavior property anymore.")]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool OverrideLayoutControlRestoreBehavior {
			get { return overrideLayoutControlRestoreBehaviorCore; }
			set { overrideLayoutControlRestoreBehaviorCore = true; }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsBehaviorUseTabKey"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool UseTabKey {
			get { return useTabKeyCore; }
			set {
				if(UseTabKey == value) return;
				bool prevValue = UseTabKey;
				useTabKeyCore = value;
				OnChanged(new BaseOptionChangedEventArgs("UseTabKey", prevValue, UseTabKey));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsBehaviorScrollVisibility"),
#endif
 DefaultValue(ScrollVisibility.Always), XtraSerializableProperty()]
		public virtual ScrollVisibility ScrollVisibility {
			get { return scrollVisibilityCore; }
			set {
				if(ScrollVisibility == value) return;
				ScrollVisibility prevValue = ScrollVisibility;
				scrollVisibilityCore = value;
				OnChanged(new BaseOptionChangedEventArgs("ScrollVisibility", prevValue, ScrollVisibility));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsBehaviorAutoFocusNewCard"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool AutoFocusNewCard {
			get { return autoFocusNewCardCore; }
			set {
				if(AutoFocusNewCard == value) return;
				bool prevValue = AutoFocusNewCard;
				autoFocusNewCardCore = value;
				OnChanged(new BaseOptionChangedEventArgs("AutoFocusNewCard", prevValue, AutoFocusNewCard));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsBehaviorAutoFocusCardOnScrolling"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool AutoFocusCardOnScrolling {
			get { return autoFocusCardOnScrollingCore; }
			set {
				if(AutoFocusCardOnScrolling == value) return;
				bool prevValue = AutoFocusCardOnScrolling;
				autoFocusCardOnScrollingCore = value;
				OnChanged(new BaseOptionChangedEventArgs("AutoFocusCardOnScroll", prevValue, AutoFocusCardOnScrolling));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsBehaviorAllowExpandCollapse"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowExpandCollapse {
			get { return allowExpandCollapseCore; }
			set {
				if(AllowExpandCollapse == value) return;
				bool prevValue = AllowExpandCollapse;
				allowExpandCollapseCore = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowExpandCollapse", prevValue, AllowExpandCollapse));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsBehaviorAllowPanCards"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowPanCards {
			get { return allowPanCardsCore; }
			set {
				if(AllowPanCards == value) return;
				bool prevValue = AllowPanCards;
				allowPanCardsCore = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowPanCards", prevValue, AllowPanCards));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsBehaviorAllowRuntimeCustomization"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowRuntimeCustomization {
			get { return allowRuntimeCustomizationCore; }
			set {
				if(AllowRuntimeCustomization == value) return;
				bool prevValue = AllowRuntimeCustomization;
				allowRuntimeCustomizationCore = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowRuntimeCustomization", prevValue, AllowRuntimeCustomization));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsBehaviorAllowSwitchViewModes"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowSwitchViewModes {
			get { return allowSwitchViewModesCore; }
			set {
				if(AllowSwitchViewModes == value) return;
				bool prevValue = AllowSwitchViewModes;
				allowSwitchViewModesCore = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowSwitchViewModes", prevValue, AllowSwitchViewModes));
			}
		}
		[DefaultValue(Utils.DefaultBoolean.Default), XtraSerializableProperty]
		[ Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Utils.DefaultBoolean AllowMouseWheelSmoothScrolling { get; set; }
	}
	public enum CardsAlignment { Near, Center, Far }
	public enum LayoutViewMode { SingleRecord, Row, Column, MultiRow, MultiColumn, Carousel }
	public enum LayoutCardArrangeRule { ShowWholeCards, AllowPartialCards }
	public enum SortFilterButtonShowMode {
		Default,
		InFieldCaption,
		InFieldValue,
		Nowhere
	}
	public enum SortFilterButtonLocation {
		Default,
		MiddleLeft, MiddleCenter, MiddleRight,
		TopCenter, TopLeft, TopRight,
		BottomCenter, BottomLeft, BottomRight
	}
	public class OptionsField : ViewBaseOptions {
		SortFilterButtonLocation buttonLocationCore;
		SortFilterButtonShowMode buttonShowModeCore;
		public OptionsField() {
			this.buttonLocationCore = SortFilterButtonLocation.Default;
			this.buttonShowModeCore = SortFilterButtonShowMode.Default;
		}
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			OptionsField source = options as OptionsField;
			if(options != null) {
				this.buttonLocationCore = source.buttonLocationCore;
				this.buttonShowModeCore = source.buttonShowModeCore;
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("OptionsFieldSortFilterButtonLocation"),
#endif
 DefaultValue(SortFilterButtonLocation.Default), XtraSerializableProperty()]
		public virtual SortFilterButtonLocation SortFilterButtonLocation {
			get { return buttonLocationCore; }
			set {
				if(SortFilterButtonLocation == value) return;
				SortFilterButtonLocation prevValue = SortFilterButtonLocation;
				this.buttonLocationCore = value;
				OnChanged(new BaseOptionChangedEventArgs("SortFilterButtonLocation", prevValue, SortFilterButtonLocation));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("OptionsFieldSortFilterButtonShowMode"),
#endif
 DefaultValue(SortFilterButtonShowMode.Default), XtraSerializableProperty()]
		public virtual SortFilterButtonShowMode SortFilterButtonShowMode {
			get { return buttonShowModeCore; }
			set {
				if(SortFilterButtonShowMode == value) return;
				SortFilterButtonShowMode prevValue = SortFilterButtonShowMode;
				this.buttonShowModeCore = value;
				OnChanged(new BaseOptionChangedEventArgs("SortFilterButtonShowMode", prevValue, SortFilterButtonShowMode));
			}
		}
	}
	public enum FocusRectStyle {
		Default,
		None
	}
	public class LayoutViewOptionsView : ColumnViewOptionsView {
		bool showFieldHints, showCardExpandButton;
		bool showHeaderPanel;
		bool showCardCaption;
		bool showCardBorderIfCaptionHidden;
		bool showCardLines;
		bool showCardFieldBorders;
		bool allowBorderColorBlendingCore;
		CardsAlignment cardsAlignment;
		LayoutViewMode viewMode;
		LayoutCardArrangeRule cardArrangeRule;
		int partialCardWrapThresholdCore;
		Utils.DefaultBoolean partialCardsSimpleScrollingCore;
		bool allowHotTrackFields;
		ContentAlignment contentAlignmentCore;
		FocusRectStyle focusRectStyleCore;
		LayoutView viewCore = null;
		int columnCount;
		public LayoutViewOptionsView(LayoutView view)
			: base() {
			this.viewCore = view;
			this.showCardExpandButton = true;
			this.showFieldHints = true;
			this.showHeaderPanel = true;
			this.showCardCaption = true;
			this.showCardLines = true;
			this.showCardFieldBorders = false;
			this.showCardBorderIfCaptionHidden = true;
			this.cardsAlignment = CardsAlignment.Center;
			this.viewMode = LayoutViewMode.SingleRecord;
			this.cardArrangeRule = LayoutCardArrangeRule.ShowWholeCards;
			this.partialCardWrapThresholdCore = 10;
			this.partialCardsSimpleScrollingCore = Utils.DefaultBoolean.Default;
			this.allowHotTrackFields = true;
			this.contentAlignmentCore = ContentAlignment.MiddleCenter;
			this.focusRectStyleCore = FocusRectStyle.Default;
			this.columnCount = 0;
		}
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			LayoutViewOptionsView source = options as LayoutViewOptionsView;
			if(source == null) return;
			this.showCardExpandButton = source.showCardExpandButton;
			this.showFieldHints = source.showFieldHints;
			this.showHeaderPanel = source.showHeaderPanel;
			this.showCardCaption = source.showCardCaption;
			this.showCardLines = source.showCardLines;
			this.showCardFieldBorders = source.showCardFieldBorders;
			this.showCardBorderIfCaptionHidden = source.showCardBorderIfCaptionHidden;
			this.cardsAlignment = source.cardsAlignment;
			this.viewMode = source.viewMode;
			this.cardArrangeRule = source.cardArrangeRule;
			this.partialCardWrapThresholdCore = source.partialCardWrapThresholdCore;
			this.partialCardsSimpleScrollingCore = source.partialCardsSimpleScrollingCore;
			this.allowHotTrackFields = source.allowHotTrackFields;
			this.contentAlignmentCore = source.contentAlignmentCore;
			this.focusRectStyleCore = source.focusRectStyleCore;
			this.columnCount = source.columnCount;
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsViewAllowHotTrackFields"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowHotTrackFields {
			get { return allowHotTrackFields; }
			set {
				if(AllowHotTrackFields == value) return;
				bool prevValue = AllowHotTrackFields;
				allowHotTrackFields = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowHotTrackFields", prevValue, AllowHotTrackFields));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsViewAllowBorderColorBlending"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool AllowBorderColorBlending {
			get { return allowBorderColorBlendingCore; }
			set {
				if(AllowBorderColorBlending == value) return;
				allowBorderColorBlendingCore = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowBorderColorBlending", !value, value));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsViewShowHeaderPanel"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowHeaderPanel {
			get { return showHeaderPanel; }
			set {
				if(ShowHeaderPanel == value) return;
				bool prevValue = ShowHeaderPanel;
				showHeaderPanel = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowHeaderPanel", prevValue, ShowHeaderPanel));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsViewCardArrangeRule"),
#endif
		DefaultValue(LayoutCardArrangeRule.ShowWholeCards), XtraSerializableProperty()]
		public virtual LayoutCardArrangeRule CardArrangeRule {
			get { return cardArrangeRule; }
			set {
				if(CardArrangeRule == value) return;
				LayoutCardArrangeRule prevValue = CardArrangeRule;
				cardArrangeRule = value;
				OnChanged(new BaseOptionChangedEventArgs("CardArrangeRule", prevValue, CardArrangeRule));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsViewPartialCardWrapThreshold"),
#endif
		DefaultValue(10), XtraSerializableProperty()]
		public virtual int PartialCardWrapThreshold {
			get { return partialCardWrapThresholdCore; }
			set {
				if(PartialCardWrapThreshold == value) return;
				int prevValue = PartialCardWrapThreshold;
				partialCardWrapThresholdCore = value;
				OnChanged(new BaseOptionChangedEventArgs("PartialCardWrapThreshold", prevValue, PartialCardWrapThreshold));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsViewPartialCardsSimpleScrolling"),
#endif
		DefaultValue(Utils.DefaultBoolean.Default), XtraSerializableProperty()]
		public virtual Utils.DefaultBoolean PartialCardsSimpleScrolling {
			get { return partialCardsSimpleScrollingCore; }
			set {
				if(PartialCardsSimpleScrolling == value) return;
				Utils.DefaultBoolean prevValue = PartialCardsSimpleScrolling;
				partialCardsSimpleScrollingCore = value;
				OnChanged(new BaseOptionChangedEventArgs("PartialCardsSimpleScrolling", prevValue, PartialCardsSimpleScrolling));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsViewShowCardCaption"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowCardCaption {
			get { return showCardCaption; }
			set {
				if(ShowCardCaption == value) return;
				bool prevValue = ShowCardCaption;
				showCardCaption = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowCardCaption", prevValue, ShowCardCaption));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsViewShowCardBorderIfCaptionHidden"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowCardBorderIfCaptionHidden {
			get { return showCardBorderIfCaptionHidden; }
			set {
				if(showCardBorderIfCaptionHidden == value) return;
				bool prevValue = ShowCardBorderIfCaptionHidden;
				showCardBorderIfCaptionHidden = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowCardBorderIfCaptionHidden", prevValue, ShowCardBorderIfCaptionHidden));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsViewShowCardLines"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowCardLines {
			get { return showCardLines; }
			set {
				if(ShowCardLines == value) return;
				bool prevValue = ShowCardLines;
				showCardLines = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowCardLines", prevValue, ShowCardLines));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsViewShowCardFieldBorders"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool ShowCardFieldBorders {
			get { return showCardFieldBorders; }
			set {
				if(ShowCardFieldBorders == value) return;
				bool prevValue = ShowCardFieldBorders;
				showCardFieldBorders = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowCardFieldBorders", prevValue, ShowCardFieldBorders));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsViewShowCardExpandButton"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowCardExpandButton {
			get { return showCardExpandButton; }
			set {
				if(ShowCardExpandButton == value) return;
				bool prevValue = ShowCardExpandButton;
				showCardExpandButton = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowCardExpandButton", prevValue, ShowCardExpandButton));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsViewCardsAlignment"),
#endif
 DefaultValue(CardsAlignment.Center), XtraSerializableProperty()]
		public virtual CardsAlignment CardsAlignment {
			get { return cardsAlignment; }
			set {
				if(CardsAlignment == value) return;
				CardsAlignment prevValue = CardsAlignment;
				cardsAlignment = value;
				OnChanged(new BaseOptionChangedEventArgs("CardsAlignment", prevValue, CardsAlignment));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsViewContentAlignment"),
#endif
 DefaultValue(ContentAlignment.MiddleCenter), XtraSerializableProperty()]
		public virtual ContentAlignment ContentAlignment {
			get { return contentAlignmentCore; }
			set {
				if(ContentAlignment == value) return;
				ContentAlignment prevValue = ContentAlignment;
				contentAlignmentCore = value;
				OnChanged(new BaseOptionChangedEventArgs("ContentAlignment", prevValue, CardsAlignment));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsViewFocusRectStyle"),
#endif
 DefaultValue(FocusRectStyle.Default), XtraSerializableProperty()]
		public virtual FocusRectStyle FocusRectStyle {
			get { return focusRectStyleCore; }
			set {
				if(FocusRectStyle == value) return;
				FocusRectStyle prevValue = FocusRectStyle;
				focusRectStyleCore = value;
				OnChanged(new BaseOptionChangedEventArgs("FocusRectStyle", prevValue, FocusRectStyle));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsViewViewMode"),
#endif
 DefaultValue(LayoutViewMode.SingleRecord), XtraSerializableProperty()]
		public virtual LayoutViewMode ViewMode {
			get { return viewMode; }
			set {
				if(ViewMode == value || !viewCore.OptionsBehavior.AllowSwitchViewModes) return;
				LayoutViewMode prevValue = ViewMode;
				viewMode = value;
				OnChanged(new BaseOptionChangedEventArgs("ViewMode", prevValue, ViewMode));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsViewShowFieldHints"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowFieldHints {
			get { return showFieldHints; }
			set {
				if(ShowFieldHints == value) return;
				bool prevValue = ShowFieldHints;
				showFieldHints = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowFieldHints", prevValue, ShowFieldHints));
			}
		}
		[ DefaultValue(0), XtraSerializableProperty()]
		public virtual int DefaultColumnCount {
			get { return columnCount; }
			set {
				if(DefaultColumnCount == value) return;
				int prevValue = DefaultColumnCount;
				columnCount = value;
				OnChanged(new BaseOptionChangedEventArgs("DefaultColumnCount", prevValue, DefaultColumnCount));
			}
		}
	}
	public enum LayoutViewPrintOptionsFlags {
		PrintFilterInfo,
		PrintCardCaption,
		PrintSelectedCardOnly,
		UsePrintStyles
	}
	public enum LayoutViewPrintMode {
		Default, Row, Column, MultiRow, MultiColumn
	}
	public class LayoutViewOptionsPrint : ViewPrintOptionsBase {
		bool printCardCaptionCore, printSelectedCardsOnlyCore, usePrintStylesCore, printFilterInfoCore;
		int maxCardRowsCore;
		int maxCardColumnsCore;
		LayoutViewPrintMode printModeCore;
		public LayoutViewOptionsPrint()
			: base() {
			this.printFilterInfoCore = false;
			this.printCardCaptionCore = true;
			this.printSelectedCardsOnlyCore = false;
			this.usePrintStylesCore = true;
			this.maxCardRowsCore = 0;
			this.maxCardColumnsCore = 0;
			this.printModeCore = LayoutViewPrintMode.Default;
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsPrintPrintMode"),
#endif
 DefaultValue(LayoutViewPrintMode.Default), XtraSerializableProperty()]
		public virtual LayoutViewPrintMode PrintMode {
			get { return printModeCore; }
			set {
				if(PrintMode == value) return;
				LayoutViewPrintMode prevValue = PrintMode;
				printModeCore = value;
				OnChanged(new BaseOptionChangedEventArgs("PrintMode", prevValue, PrintMode));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsPrintMaxCardRows"),
#endif
 DefaultValue(0), XtraSerializableProperty()]
		public virtual int MaxCardRows {
			get { return maxCardRowsCore; }
			set {
				if(MaxCardRows == value) return;
				int prevValue = MaxCardRows;
				maxCardRowsCore = value;
				OnChanged(new BaseOptionChangedEventArgs("MaxCardRows", prevValue, MaxCardRows));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsPrintMaxCardColumns"),
#endif
 DefaultValue(0), XtraSerializableProperty()]
		public virtual int MaxCardColumns {
			get { return maxCardColumnsCore; }
			set {
				if(MaxCardColumns == value) return;
				int prevValue = MaxCardColumns;
				maxCardColumnsCore = value;
				OnChanged(new BaseOptionChangedEventArgs("MaxCardColumns", prevValue, MaxCardColumns));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsPrintPrintCardCaption"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool PrintCardCaption {
			get { return printCardCaptionCore; }
			set {
				if(PrintCardCaption == value) return;
				bool prevValue = PrintCardCaption;
				printCardCaptionCore = value;
				OnChanged(new BaseOptionChangedEventArgs("PrintCardCaption", prevValue, PrintCardCaption));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsPrintPrintFilterInfo"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool PrintFilterInfo {
			get { return printFilterInfoCore; }
			set {
				if(PrintFilterInfo == value) return;
				bool prevValue = PrintFilterInfo;
				printFilterInfoCore = value;
				OnChanged(new BaseOptionChangedEventArgs("PrintFilterInfo", prevValue, PrintFilterInfo));
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Obsolete(ObsoleteText.SRCardOptionsPrint_PrintSelectedCardOnly)]
		public bool PrintSelectedCardOnly {
			get { return PrintSelectedCardsOnly; }
			set { PrintSelectedCardsOnly = value; }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsPrintPrintSelectedCardsOnly"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool PrintSelectedCardsOnly {
			get { return printSelectedCardsOnlyCore; }
			set {
				if(PrintSelectedCardsOnly == value) return;
				bool prevValue = PrintSelectedCardsOnly;
				printSelectedCardsOnlyCore = value;
				OnChanged(new BaseOptionChangedEventArgs("PrintSelectedCardsOnly", prevValue, PrintSelectedCardsOnly));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsPrintUsePrintStyles"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool UsePrintStyles {
			get { return usePrintStylesCore; }
			set {
				if(UsePrintStyles == value) return;
				bool prevValue = UsePrintStyles;
				usePrintStylesCore = value;
				OnChanged(new BaseOptionChangedEventArgs("UsePrintStyles", prevValue, UsePrintStyles));
			}
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				LayoutViewOptionsPrint source = options as LayoutViewOptionsPrint;
				if(source == null) return;
				this.printModeCore = source.printModeCore;
				this.printFilterInfoCore = source.printFilterInfoCore;
				this.printCardCaptionCore = source.printCardCaptionCore;
				this.printSelectedCardsOnlyCore = source.printSelectedCardsOnlyCore;
				this.usePrintStylesCore = source.usePrintStylesCore;
				this.maxCardRowsCore = source.maxCardRowsCore;
				this.maxCardColumnsCore = source.maxCardColumnsCore;
			}
			finally { EndUpdate(); }
		}
		internal void ConvertFromFlags(LayoutViewPrintOptionsFlags flags) {
			PrintFilterInfo = (flags & LayoutViewPrintOptionsFlags.PrintFilterInfo) != 0;
			PrintCardCaption = (flags & LayoutViewPrintOptionsFlags.PrintCardCaption) != 0;
			PrintSelectedCardsOnly = (flags & LayoutViewPrintOptionsFlags.PrintSelectedCardOnly) != 0;
			UsePrintStyles = (flags & LayoutViewPrintOptionsFlags.UsePrintStyles) != 0;
		}
		public override string ToString() {
			string mode = PrintMode.ToString();
			string flags = base.ToString();
			if(flags.Length == 0) return mode;
			return mode + ", " + flags;
		}
	}
}
