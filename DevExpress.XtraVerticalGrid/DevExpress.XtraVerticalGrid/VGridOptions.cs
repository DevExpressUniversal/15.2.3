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
using DevExpress.XtraVerticalGrid.Utils;
using DevExpress.XtraVerticalGrid.Rows;
using DevExpress.Utils;
using DevExpress.XtraVerticalGrid.Localization;
namespace DevExpress.XtraVerticalGrid {
	public enum FindPanelVisibility { Default, Always, Never }
	public enum FindMode { Default, Always, FindClick };
	public class VGridOptionsLayout : OptionsLayoutGrid {
		internal const int AppearanceLayoutId = 1;
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		new public bool StoreAllOptions { get; set; }
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		new public bool StoreDataSettings { get; set; }
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		new public bool StoreVisualOptions { get; set; }
		protected override bool ShouldSerialize(IComponent owner) {
			return base.ShouldSerialize(owner);
		}
		protected override OptionsColumnLayout CreateOptionsColumn() {
			return new VGridOptionsColumnLayout();
		}
	}
	public class VGridOptionsColumnLayout : OptionsColumnLayout {
		public VGridOptionsColumnLayout() {
			base.AddNewColumns = false;
			base.RemoveOldColumns = false;
		}
		[NotifyParentProperty(true)]
		[XtraSerializableProperty]
		[DefaultValue(false)]
		new public bool AddNewColumns { get { return base.AddNewColumns; } set { base.AddNewColumns = value; } }
		[NotifyParentProperty(true)]
		[XtraSerializableProperty]
		[DefaultValue(false)]
		new public bool RemoveOldColumns { get { return base.RemoveOldColumns; } set { base.RemoveOldColumns = value; } }
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		new public bool StoreAllOptions { get; set; }
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		new  public bool StoreAppearance { get; set; }
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		new public bool StoreLayout { get; set; }
	}
	public class BaseVGridOptions : BaseOptions {
		protected BaseVGridOptions() {}
		internal event BaseOptionChangedEventHandler Changed {
			add { base.ChangedCore += value; }
			remove { base.ChangedCore -= value; }
		}
		protected void BooleanChanged(string name, bool value) {
			OnChanged(new BaseOptionChangedEventArgs(name, !value, value));
		}
		protected void DefaultBooleanChanged(string name, DefaultBoolean oldValue, DefaultBoolean newValue) {
			OnChanged(new BaseOptionChangedEventArgs(name, oldValue, newValue));
		}
		protected internal bool ShouldSerializeCore(IComponent owner) { return ShouldSerialize(owner); }
	}
	public class VGridOptionsHint : BaseVGridOptions {
		bool
			showCellHints,
			showRowHeaderHints;
		protected internal const string
			ShowCellHintsName = "ShowCellHints",
			ShowRowHeaderHintsName = "ShowRowHeaderHints";
		public VGridOptionsHint() {
			this.showCellHints = true;
			this.showRowHeaderHints = true;
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsHintShowCellHints"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowCellHints {
			get { return showCellHints; }
			set {
				if(ShowCellHints == value) return;
				bool prevValue = ShowCellHints;
				showCellHints = value;
				OnChanged(new BaseOptionChangedEventArgs(ShowCellHintsName, prevValue, ShowCellHints));
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsHintShowRowHeaderHints"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowRowHeaderHints {
			get { return showRowHeaderHints; }
			set {
				if(ShowRowHeaderHints == value) return;
				bool prevValue = ShowRowHeaderHints;
				showRowHeaderHints = value;
				OnChanged(new BaseOptionChangedEventArgs(ShowRowHeaderHintsName, prevValue, ShowRowHeaderHints));
			}
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				VGridOptionsHint opt = options as VGridOptionsHint;
				if(opt == null)
					return;
				this.showCellHints = opt.ShowCellHints;
				this.showRowHeaderHints = opt.ShowRowHeaderHints;
			} finally {
				EndUpdate();
			}
		}
	}
	public class VGridOptionsView : BaseVGridOptions {
		const int FixedLineWidthMin = 0;
		const int FixedLineWidthMax = 20;
		const int FixedLineWidthDefault = 2;
		public const int DefaultLevelIndent = -1;
		bool autoScaleBands, showButtons, showRows, showHorzLines, showVertLines,
			showFocusedFrame, showEmptyRowImage, fixRowHeaderPanelWidth, showRootCategories, allowGlyphSkinning, allowHtmlText;
		int fixedLineWidth;
		int minRowAutoHeight;
		int maxRowAutoHeight;
		int levelIndent;
		public VGridOptionsView() {
			this.showRows = this.showHorzLines = this.showVertLines =
				this.showButtons = this.showFocusedFrame = this.showRootCategories = true;
			this.autoScaleBands =  this.showEmptyRowImage = this.fixRowHeaderPanelWidth = false;
			this.fixedLineWidth = FixedLineWidthDefault;
			this.minRowAutoHeight = BaseRow.MinRowHeight;
			this.maxRowAutoHeight = BaseRow.MaxRowHeight;
			this.levelIndent = DefaultLevelIndent;
			this.AllowHtmlText = false;
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsViewShowRows"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowRows {
			get { return showRows; }
			set {
				if(ShowRows == value) return;
				showRows = value;
				BooleanChanged(ShowRowsName, ShowRows);
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsViewShowHorzLines"),
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
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsViewShowVertLines"),
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
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsViewShowButtons"),
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
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsViewShowFocusedFrame"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowFocusedFrame {
			get { return showFocusedFrame; }
			set {
				if(ShowFocusedFrame == value) return;
				showFocusedFrame = value;
				BooleanChanged(ShowFocusedFrameName, ShowFocusedFrame);
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsViewAutoScaleBands"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool AutoScaleBands {
			get { return autoScaleBands; }
			set {
				if(AutoScaleBands == value) return;
				autoScaleBands = value;
				BooleanChanged(AutoScaleBandsName, AutoScaleBands);
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsViewShowEmptyRowImage"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool ShowEmptyRowImage {
			get { return showEmptyRowImage; }
			set {
				if(ShowEmptyRowImage == value) return;
				showEmptyRowImage = value;
				BooleanChanged(ShowEmptyRowImageName, ShowEmptyRowImage);
			}
		}
#if !SL
	[DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsViewLevelIndent")]
#endif
		[DefaultValue(DefaultLevelIndent)]
		[XtraSerializableProperty()]
		public virtual int LevelIndent {
			get { return levelIndent; }
			set {
				if(levelIndent == value) return;
				int oldValue = levelIndent;
				levelIndent = value;
				OnChanged(LevelIndentName, oldValue, levelIndent);
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsViewFixRowHeaderPanelWidth"),
#endif
 DefaultValue(false),
		XtraSerializableProperty()]
		public virtual bool FixRowHeaderPanelWidth {
			get { return fixRowHeaderPanelWidth; }
			set {
				if(fixRowHeaderPanelWidth == value) return;
				fixRowHeaderPanelWidth = value;
				BooleanChanged(FixRowHeaderPanelWidthName, fixRowHeaderPanelWidth);
			} 
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsViewFixedLineWidth"),
#endif
 DefaultValue(FixedLineWidthDefault),
		XtraSerializableProperty()]
		public virtual int FixedLineWidth {
			get { return fixedLineWidth; }
			set {
				value = MathUtils.FitBounds(value, FixedLineWidthMin, FixedLineWidthMax);
				if(fixedLineWidth == value)
					return;
				int oldValue = fixedLineWidth;
				fixedLineWidth = value;
				OnChanged(FixedLineWidthName, oldValue, fixedLineWidth);
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsViewMinRowAutoHeight"),
#endif
 DefaultValue(BaseRow.MinRowHeight),
		XtraSerializableProperty()]
		public virtual int MinRowAutoHeight {
			get { return minRowAutoHeight; }
			set {
				value = MathUtils.FitBounds(value, BaseRow.MinRowHeight, int.MaxValue);
				if(minRowAutoHeight == value)
					return;
				int oldValue = minRowAutoHeight;
				minRowAutoHeight = value;
				OnChanged(MinRowAutoHeightName, oldValue, minRowAutoHeight);
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsViewMaxRowAutoHeight"),
#endif
 DefaultValue(BaseRow.MaxRowHeight),
		XtraSerializableProperty()]
		public virtual int MaxRowAutoHeight {
			get { return maxRowAutoHeight; }
			set {
				value = MathUtils.FitBounds(value, BaseRow.MaxRowHeight, int.MaxValue);
				if(maxRowAutoHeight == value)
					return;
				int oldValue = maxRowAutoHeight;
				maxRowAutoHeight = value;
				OnChanged(MaxRowAutoHeightName, oldValue, maxRowAutoHeight);
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsViewShowRootCategories"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowRootCategories {
			get { return showRootCategories; }
			set {
				if(ShowRootCategories == value) return;
				showRootCategories = value;
				BooleanChanged(ShowRootCategoriesName, ShowRootCategories);
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsViewAllowGlyphSkinning"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool AllowGlyphSkinning {
			get { return allowGlyphSkinning; }
			set {
				if(AllowGlyphSkinning == value) return;
				allowGlyphSkinning = value;
				BooleanChanged("AllowGlyphSkinning", AllowGlyphSkinning);
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsViewAllowHtmlText"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool AllowHtmlText {
			get { return allowHtmlText; }
			set {
				if(AllowHtmlText == value) return;
				allowHtmlText = value;
				BooleanChanged("AllowHtmlText", AllowHtmlText);
			}
		}
		protected internal const string ShowRowsName = "ShowRows";
		protected internal const string ShowHorzLinesName = "ShowHorzLines";
		protected internal const string ShowVertLinesName = "ShowVertLines";
		protected internal const string ShowButtonsName = "ShowButtons";
		protected internal const string ShowFocusedFrameName = "ShowFocusedFrame";
		protected internal const string AutoCalcPreviewLineCountName = "AutoCalcPreviewLineCount";
		protected internal const string AutoScaleBandsName = "AutoScaleBands";
		protected internal const string ShowSummaryFooterName = "ShowSummaryFooter";
		protected internal const string ShowRecordPreviewName = "ShowRecordPreview";
		protected internal const string ShowEmptyRowImageName = "ShowEmptyRowImage";
		protected internal const string FixRowHeaderPanelWidthName = "FixRowHeaderPanelWidth";
		protected internal const string ShowRootCategoriesName = "ShowRootCategories";
		protected internal const string LevelIndentName = "LevelIndent";
		protected internal const string FixedLineWidthName = "FixedLineWidth";
		protected internal const string MinRowAutoHeightName = "MinRowAutoHeight";
		protected internal const string MaxRowAutoHeightName = "MaxRowAutoHeight";
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				VGridOptionsView opt = options as VGridOptionsView;
				if(opt == null) return;
				this.showRows = opt.ShowRows;
				this.showHorzLines = opt.ShowHorzLines; 
				this.showVertLines = opt.ShowVertLines;
				this.showButtons = opt.ShowButtons; 
				this.showFocusedFrame = opt.ShowFocusedFrame; 
				this.autoScaleBands = opt.AutoScaleBands; 
				this.showEmptyRowImage = opt.ShowEmptyRowImage; 
				this.fixRowHeaderPanelWidth = opt.FixRowHeaderPanelWidth;
				this.showRootCategories = opt.ShowRootCategories;
				this.fixedLineWidth = opt.FixedLineWidth;
				this.minRowAutoHeight = opt.MinRowAutoHeight;
				this.maxRowAutoHeight = opt.MaxRowAutoHeight;
				this.allowGlyphSkinning = opt.AllowGlyphSkinning;
				this.allowHtmlText = opt.AllowHtmlText;
			}
			finally {
				EndUpdate();
			}
		}
	}
	public class PGridOptionsView : VGridOptionsView {
		const char DefaultPasswordChar = '*';
		protected internal const string PasswordCharName = "PasswordChar";
		char passwordChar = DefaultPasswordChar;
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("PGridOptionsViewPasswordChar"),
#endif
 DefaultValue(DefaultPasswordChar), XtraSerializableProperty()]
		public virtual char PasswordChar {
			get { return passwordChar; }
			set {
				if(PasswordChar == value)
					return;
				char oldValue = passwordChar;
				passwordChar = value;
				OnChanged(PasswordCharName, oldValue, PasswordChar);
			}
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				PGridOptionsView opt = options as PGridOptionsView;
				if(opt == null)
					return;
				this.PasswordChar = opt.PasswordChar;
			} finally {
				EndUpdate();
			}
		}
	}
	public class PGridOptionsBehavior : VGridOptionsBehavior {
		const PropertySort DefaultPropertySort = PropertySort.Alphabetical;
		protected internal const string AllowDynamicRowLoadingName = "AllowDynamicRowLoading";
		protected internal const string PropertySortName = "PropertySort";
		bool allowDynamicRowLoading = true;
		PropertySort propertySort;
		public PGridOptionsBehavior() {
			PropertySort = DefaultPropertySort;
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("PGridOptionsBehaviorAllowDynamicRowLoading"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowDynamicRowLoading {
			get { return allowDynamicRowLoading; }
			set {
				if(AllowDynamicRowLoading == value) return;
				allowDynamicRowLoading = value;
				BooleanChanged(AllowDynamicRowLoadingName, AllowDynamicRowLoading);
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("PGridOptionsBehaviorPropertySort"),
#endif
 DefaultValue(DefaultPropertySort), XtraSerializableProperty()]
		public virtual PropertySort PropertySort {
			get { return propertySort; }
			set {
				if(PropertySort == value) return;
				PropertySort oldValue = propertySort;
				propertySort = value;
				OnChanged(PropertySortName, oldValue, propertySort);
			}
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				PGridOptionsBehavior opt = options as PGridOptionsBehavior;
				if(opt == null) return;
				this.AllowDynamicRowLoading = opt.AllowDynamicRowLoading;
				this.PropertySort = opt.PropertySort;
			} finally {
				EndUpdate();
			}
		}
	}
	public class VGridOptionsBehavior : BaseVGridOptions {
		bool editable,
			autoSelectAllInEditor,
			smartExpand,
			recordsMouseWheel,
			resizeRowHeaders,
			resizeHeaderPanel,
			resizeRowValues,
			dragRowHeaders,
			preserveChildRows,
			useTabKey,
			useEnterAsTab,
			autoFocusNewRecord,
			showEditorOnMouseUp,
			useDefaultEditorsCollection,
			copyToClipboardWithRowHeaders;
		bool allowAnimatedScrolling;
		public VGridOptionsBehavior() {
			this.editable = this.autoSelectAllInEditor = this.smartExpand = this.resizeRowHeaders =
				this.resizeHeaderPanel = this.resizeRowValues = this.useTabKey = this.useDefaultEditorsCollection =
				this.copyToClipboardWithRowHeaders = true;
			this.recordsMouseWheel = this.dragRowHeaders = this.preserveChildRows = 
				this.useEnterAsTab = this.autoFocusNewRecord = this.showEditorOnMouseUp = false;
			this.allowAnimatedScrolling = true;
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsBehaviorAllowAnimatedScrolling"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowAnimatedScrolling {
			get { return allowAnimatedScrolling; }
			set {
				if (AllowAnimatedScrolling == value)
					return;
				allowAnimatedScrolling = value;
				BooleanChanged(AllowAnimatedScrollingName, AllowAnimatedScrolling);
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsBehaviorCopyToClipboardWithRowHeaders"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool CopyToClipboardWithRowHeaders {
			get { return copyToClipboardWithRowHeaders; }
			set {
				if(CopyToClipboardWithRowHeaders == value) return;
				copyToClipboardWithRowHeaders = value;
				BooleanChanged(CopyToClipboardWithRowHeadersName, CopyToClipboardWithRowHeaders);
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsBehaviorEditable"),
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
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsBehaviorAutoSelectAllInEditor"),
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
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsBehaviorSmartExpand"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool SmartExpand {
			get { return smartExpand; }
			set {
				if(SmartExpand == value) return;
				smartExpand = value;
				BooleanChanged(SmartExpandName, SmartExpand);
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsBehaviorResizeRowHeaders"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ResizeRowHeaders {
			get { return resizeRowHeaders; }
			set {
				if(ResizeRowHeaders == value) return;
				resizeRowHeaders = value;
				BooleanChanged(ResizeRowHeadersName, ResizeRowHeaders);
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsBehaviorResizeHeaderPanel"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ResizeHeaderPanel {
			get { return resizeHeaderPanel; }
			set {
				if(ResizeHeaderPanel == value) return;
				resizeHeaderPanel = value;
				BooleanChanged(ResizeHeaderPanelName, ResizeHeaderPanel);
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsBehaviorResizeRowValues"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ResizeRowValues {
			get { return resizeRowValues; }
			set {
				if(ResizeRowValues == value) return;
				resizeRowValues = value;
				BooleanChanged(ResizeRowValuesName, ResizeRowValues);
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsBehaviorUseTabKey"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool UseTabKey {
			get { return useTabKey; }
			set {
				if(UseTabKey == value) return;
				useTabKey = value;
				BooleanChanged(UseTabKeyName, UseTabKey);
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsBehaviorRecordsMouseWheel"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool RecordsMouseWheel {
			get { return recordsMouseWheel; }
			set {
				if(RecordsMouseWheel == value) return;
				recordsMouseWheel = value;
				BooleanChanged(RecordsMouseWheelName, RecordsMouseWheel);
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsBehaviorDragRowHeaders"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool DragRowHeaders {
			get { return dragRowHeaders; }
			set {
				if(DragRowHeaders == value) return;
				dragRowHeaders = value;
				BooleanChanged(DragRowHeadersName, DragRowHeaders);
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsBehaviorPreserveChildRows"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool PreserveChildRows {
			get { return preserveChildRows; }
			set {
				if(PreserveChildRows == value) return;
				preserveChildRows = value;
				BooleanChanged(PreserveChildRowsName, PreserveChildRows);
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsBehaviorUseEnterAsTab"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool UseEnterAsTab {
			get { return useEnterAsTab; }
			set {
				if(UseEnterAsTab == value) return;
				useEnterAsTab = value;
				BooleanChanged(UseEnterAsTabName, UseEnterAsTab);
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsBehaviorAutoFocusNewRecord"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool AutoFocusNewRecord {
			get { return autoFocusNewRecord; }
			set {
				if(AutoFocusNewRecord == value) return;
				autoFocusNewRecord = value;
				BooleanChanged(AutoFocusNewRecordName, AutoFocusNewRecord);
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsBehaviorShowEditorOnMouseUp"),
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
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsBehaviorUseDefaultEditorsCollection"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool UseDefaultEditorsCollection {
			get { return useDefaultEditorsCollection; }
			set {
				if(UseDefaultEditorsCollection == value) return;
				useDefaultEditorsCollection = value;
				BooleanChanged(UseDefaultEditorsCollectionName, UseDefaultEditorsCollection);
			}
		}
		protected internal const string EditableName = "Editable";
		protected internal const string  AutoSelectAllInEditorName = "AutoSelectAllInEditor";
		protected internal const string SmartExpandName = "SmartExpand";
		protected internal const string ResizeRowHeadersName = "ResizeRowHeaders";
		protected internal const string ResizeHeaderPanelName = "ResizeHeaderPanel";
		protected internal const string ResizeRowValuesName = "ResizeRowValues";
		protected internal const string UseTabKeyName = "UseTabKey";
		protected internal const string RecordsMouseWheelName = "RecordsMouseWheel";
		protected internal const string DragRowHeadersName = "DragRowHeaders";
		protected internal const string PreserveChildRowsName = "PreserveChildRows";
		protected internal const string UseEnterAsTabName = "UseEnterAsTab";
		protected internal const string AutoFocusNewRecordName = "AutoFocusNewRecord";
		protected internal const string ShowEditorOnMouseUpName = "ShowEditorOnMouseUp";
		protected internal const string UseDefaultEditorsCollectionName = "UseDefaultEditorsCollection";
		protected internal const string CopyToClipboardWithRowHeadersName = "CopyToClipboardWithRowHeaders";
		protected internal const string AllowAnimatedScrollingName = "AllowAnimatedScrolling";
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				VGridOptionsBehavior opt = options as VGridOptionsBehavior;
				if(opt == null) return;
				this.editable = opt.Editable;
				this.autoSelectAllInEditor = opt.AutoSelectAllInEditor;
				this.smartExpand = opt.SmartExpand;
				this.resizeRowHeaders = opt.ResizeRowHeaders;
				this.resizeHeaderPanel = opt.ResizeHeaderPanel;
				this.resizeRowValues = opt.ResizeRowValues;
				this.useTabKey = opt.UseTabKey;
				this.recordsMouseWheel = opt.RecordsMouseWheel; 
				this.dragRowHeaders = opt.DragRowHeaders;
				this.preserveChildRows = opt.PreserveChildRows; 
				this.useEnterAsTab = opt.UseEnterAsTab;
				this.autoFocusNewRecord = opt.AutoFocusNewRecord; 
				this.showEditorOnMouseUp = opt.ShowEditorOnMouseUp;
				this.useDefaultEditorsCollection = opt.UseDefaultEditorsCollection;
				this.copyToClipboardWithRowHeaders = opt.CopyToClipboardWithRowHeaders;
			}
			finally {
				EndUpdate();
			}
		}
	}
}
namespace DevExpress.XtraVerticalGrid.Rows {
	public class VGridOptionsRow : BaseVGridOptions {
		bool allowMove, allowSize, allowFocus, showInCustomizationForm, 
			allowMoveToCustomizationForm, dblClickExpanding;
		DefaultBoolean allowHtmlText;
		public VGridOptionsRow() {
			this.showInCustomizationForm = this.allowMove = this.allowSize = 
				this.allowFocus = this.allowMoveToCustomizationForm =
				this.dblClickExpanding = true;
			this.allowHtmlText = DefaultBoolean.Default;
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsRowAllowHtmlText"),
#endif
 DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public DefaultBoolean AllowHtmlText {
			get { return allowHtmlText; }
			set {
				if(AllowHtmlText == value) return;
				DefaultBoolean oldValue = AllowHtmlText;
				allowHtmlText = value;
				DefaultBooleanChanged(AllowHtmlTextName, oldValue, AllowHtmlText);
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsRowShowInCustomizationForm"),
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
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsRowAllowMove"),
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
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsRowAllowSize"),
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
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsRowAllowFocus"),
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
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsRowAllowMoveToCustomizationForm"),
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
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsRowDblClickExpanding"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool DblClickExpanding {
			get { return dblClickExpanding; }
			set {
				if(DblClickExpanding == value) return;
				dblClickExpanding = value;
				BooleanChanged(DblClickExpandingName, DblClickExpanding);
			}
		}
		protected internal const string AllowHtmlTextName = "AllowHtmlText"; 
		protected internal const string ShowInCustomizationFormName = "ShowInCustomizationForm";
		protected internal const string AllowMoveName = "AllowMove";
		protected internal const string AllowSizeName = "AllowSize";
		protected internal const string AllowFocusName = "AllowFocus";
		protected internal const string AllowMoveToCustomizationFormName = "AllowMoveToCustomizationForm";
		protected internal const string DblClickExpandingName = "DblClickExpanding";
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				VGridOptionsRow opt = options as VGridOptionsRow;
				if(opt == null) return;
				this.showInCustomizationForm = opt.ShowInCustomizationForm; 
				this.allowMove = opt.AllowMove; 
				this.allowSize = opt.AllowSize; 
				this.allowFocus = opt.AllowFocus; 
				this.allowMoveToCustomizationForm = opt.AllowMoveToCustomizationForm;
				this.dblClickExpanding = opt.DblClickExpanding; 
			}
			finally {
				EndUpdate();
			}
		}
	}
	public class PGridOptionsMenu : VGridOptionsMenu {
	}
	public class VGridOptionsMenu : BaseVGridOptions {
		bool enableContextMenu;
		[
		DefaultValue(false),
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsMenuEnableContextMenu"),
#endif
		XtraSerializableProperty()
		]
		public bool EnableContextMenu { get { return enableContextMenu; } set { enableContextMenu = value; } }
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				VGridOptionsMenu opt = options as VGridOptionsMenu;
				if(opt == null)
					return;
				this.enableContextMenu = opt.EnableContextMenu;
			} finally {
				EndUpdate();
			}
		}
	}
	public class VGridOptionsFind : BaseVGridOptions {
		protected internal const string VisibilityName = "Visibility";
		protected internal const string ShowCloseButtonName = "ShowCloseButton";
		protected internal const string ShowClearButtonName = "ShowClearButton";
		protected internal const string ShowFindButtonName = "ShowFindButton";
		protected internal const string FindDelayName = "FindDelay";
		protected internal const string FindNullPromptName = "FindNullPrompt";
		protected internal const string FindModeName = "FindMode";
		int findDelay;
		bool showCloseButton, clearFindOnClose, highlightFindResults, showClearButton, showFindButton;
		FindPanelVisibility visibility;
		FindMode findMode;
		string findFilterColumns;
		string findNullPrompt;
		public VGridOptionsFind() {
			this.highlightFindResults = true;
			this.clearFindOnClose = true;
			this.findDelay = 1000;
			this.showClearButton = this.showFindButton = this.showCloseButton = true;
			this.findFilterColumns = "*";
			this.findNullPrompt = defaultFindNullPrompt;
			this.visibility = FindPanelVisibility.Default;
		}
		[Category("Data")]
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsFindFindMode"),
#endif
 DefaultValue(FindMode.Default)]
		public FindMode FindMode {
			get { return findMode; }
			set {
				if (FindMode == value)
					return;
				FindMode prevValue = FindMode;
				findMode = value;
				OnChanged(new BaseOptionChangedEventArgs(FindModeName, prevValue, FindMode));
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsFindFindDelay"),
#endif
 DefaultValue(1000), XtraSerializableProperty()]
		public virtual int FindDelay {
			get { return findDelay; }
			set {
				if (value < 100)
					value = 100;
				if (FindDelay == value)
					return;
				int prevValue = FindDelay;
				findDelay = value;
				OnChanged(new BaseOptionChangedEventArgs(FindDelayName, prevValue, FindDelay));
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsFindVisibility"),
#endif
 DefaultValue(FindPanelVisibility.Default), XtraSerializableProperty()]
		public virtual FindPanelVisibility Visibility {
			get { return visibility; }
			set {
				if (Visibility == value)
					return;
				FindPanelVisibility previousValue = Visibility;
				visibility = value;
				OnChanged(new BaseOptionChangedEventArgs(VisibilityName, previousValue, Visibility));
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsFindHighlightFindResults"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool HighlightFindResults {
			get { return highlightFindResults; }
			set {
				if (HighlightFindResults == value)
					return;
				bool prevValue = HighlightFindResults;
				highlightFindResults = value;
				OnChanged(new BaseOptionChangedEventArgs("HighlightFindResults", prevValue, HighlightFindResults));
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsFindClearFindOnClose"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ClearFindOnClose {
			get { return clearFindOnClose; }
			set {
				if (ClearFindOnClose == value)
					return;
				bool prevValue = ClearFindOnClose;
				clearFindOnClose = value;
				OnChanged(new BaseOptionChangedEventArgs("ClearFindOnClose", prevValue, ClearFindOnClose));
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsFindShowCloseButton"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowCloseButton {
			get { return showCloseButton; }
			set {
				if (ShowCloseButton == value)
					return;
				bool prevValue = ShowCloseButton;
				showCloseButton = value;
				OnChanged(new BaseOptionChangedEventArgs(ShowClearButtonName, prevValue, ShowCloseButton));
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsFindShowFindButton"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowFindButton {
			get { return showFindButton; }
			set {
				if (ShowFindButton == value)
					return;
				bool prevValue = ShowFindButton;
				showFindButton = value;
				OnChanged(new BaseOptionChangedEventArgs(ShowFindButtonName, prevValue, ShowFindButton));
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsFindShowClearButton"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowClearButton {
			get { return showClearButton; }
			set {
				if (ShowClearButton == value)
					return;
				bool prevValue = ShowClearButton;
				showClearButton = value;
				OnChanged(new BaseOptionChangedEventArgs(ShowClearButtonName, prevValue, ShowClearButton));
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsFindFindFilterColumns"),
#endif
 DefaultValue("*"), XtraSerializableProperty()]
		public virtual string FindFilterColumns {
			get { return findFilterColumns; }
			set {
				if (value == null)
					value = "";
				if (FindFilterColumns == value)
					return;
				string prevValue = FindFilterColumns;
				findFilterColumns = value;
				OnChanged(new BaseOptionChangedEventArgs("FindFilterColumns", prevValue, FindFilterColumns));
			}
		}
		string defaultFindNullPrompt { get { return VGridLocalizer.Active.GetLocalizedString(VGridStringId.FindNullPrompt); } }
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsFindFindNullPrompt"),
#endif
 XtraSerializableProperty()]
		public virtual string FindNullPrompt {
			get { return findNullPrompt; }
			set {
				if (value == null)
					value = "";
				if (FindNullPrompt == value)
					return;
				string prevValue = FindNullPrompt;
				findNullPrompt = value;
				OnChanged(new BaseOptionChangedEventArgs(FindNullPromptName, prevValue, FindNullPrompt));
			}
		}
		bool ShouldSerializeFindNullPrompt() { return FindNullPrompt != defaultFindNullPrompt; }
		void ResetFindNullPrompt() { FindNullPrompt = defaultFindNullPrompt; }
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				VGridOptionsFind opt = options as VGridOptionsFind;
				if (opt == null)
					return;
				this.findNullPrompt = opt.FindNullPrompt;
				this.findMode = opt.FindMode;
				this.Visibility = opt.Visibility;
				this.showCloseButton = opt.ShowCloseButton;
				this.showFindButton = opt.ShowFindButton;
				this.showClearButton = opt.ShowClearButton;
				this.findFilterColumns = opt.FindFilterColumns;
				this.highlightFindResults = opt.HighlightFindResults;
				this.findDelay = opt.FindDelay;
			}
			finally {
				EndUpdate();
			}
		}
	}
	public class VGridOptionsSelectionAndFocus : BaseVGridOptions {
		protected internal const string EnableAppearanceFocusedRowName = "EnableAppearanceFocusedRow";
		bool enableAppearanceFocusedRow;
		const bool DefaultEnableAppearanceFocusedRow = true;
		public VGridOptionsSelectionAndFocus() {
			this.enableAppearanceFocusedRow = DefaultEnableAppearanceFocusedRow;
		}
#if !SL
	[DevExpressXtraVerticalGridLocalizedDescription("VGridOptionsSelectionAndFocusEnableAppearanceFocusedRow")]
#endif
		[DefaultValue(DefaultEnableAppearanceFocusedRow)]
		[XtraSerializableProperty()]
		public bool EnableAppearanceFocusedRow {
			get {
				 return enableAppearanceFocusedRow;
			}
			set {
				if (EnableAppearanceFocusedRow == value)
					return;
				bool prevValue = EnableAppearanceFocusedRow;
				enableAppearanceFocusedRow = value;
				OnChanged(new BaseOptionChangedEventArgs(EnableAppearanceFocusedRowName, prevValue, EnableAppearanceFocusedRow));
			}
		}
		bool ShouldSerializeEnableAppearanceFocusedRow() { return EnableAppearanceFocusedRow != DefaultEnableAppearanceFocusedRow; }
		void ResetEnableAppearanceFocusedRow() { EnableAppearanceFocusedRow = DefaultEnableAppearanceFocusedRow; }
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				VGridOptionsSelectionAndFocus opt = options as VGridOptionsSelectionAndFocus;
				if (opt == null)
					return;
				this.enableAppearanceFocusedRow = opt.enableAppearanceFocusedRow;
			}
			finally {
				EndUpdate();
			}
		}
	}
}
