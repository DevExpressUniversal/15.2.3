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

using DevExpress.Utils.Controls;
using DevExpress.Utils.Serializing;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxPivotGrid.Data;
using DevExpress.WebUtils;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
namespace DevExpress.Web.ASPxPivotGrid {
	public enum PivotDataHeadersDisplayMode { Default, Popup };
	public enum PivotScrollingMode { Standard, Virtual };
	public class PivotGridWebOptionsView : PivotGridOptionsViewBase {
		const ScrollBarMode VerticalScrollBarModeDefault = ScrollBarMode.Hidden;
		const ScrollBarMode HorizontalScrollBarModeDefault = ScrollBarMode.Hidden;
		const PivotScrollingMode VerticalScrollingModeDefault = PivotScrollingMode.Standard;
		const PivotScrollingMode HorizontalScrollingModeDefault = PivotScrollingMode.Standard;
		bool showContextMenus;
		PivotDataHeadersDisplayMode dataHeadersDisplayMode;
		int dataHeadersPopupMaxColumnCount;
		int dataHeadersPopupMinCount;
		bool showHorizontalScrollBar;
		bool enableFilterControlPopupMenuScrolling;
		bool enableContextMenuScrolling;
		public PivotGridWebOptionsView(EventHandler optionsChanged, IViewBagOwner viewBagOwner, string objectPath)
			: base(optionsChanged, viewBagOwner, objectPath) {
			this.showContextMenus = true;
			this.dataHeadersDisplayMode = PivotDataHeadersDisplayMode.Default;
			this.dataHeadersPopupMinCount = 3;
			this.dataHeadersPopupMaxColumnCount = -1;
			this.showHorizontalScrollBar = false;
			this.enableFilterControlPopupMenuScrolling = false;
			this.enableContextMenuScrolling = false;
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridWebOptionsViewDataHeadersDisplayMode"),
#endif
		DefaultValue(PivotDataHeadersDisplayMode.Default), XtraSerializableProperty(), NotifyParentProperty(true)]
		public PivotDataHeadersDisplayMode DataHeadersDisplayMode {
			get { return dataHeadersDisplayMode; }
			set {
				if(value == dataHeadersDisplayMode) return;
				dataHeadersDisplayMode = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridWebOptionsViewDataHeadersPopupMinCount"),
#endif
		DefaultValue(3), XtraSerializableProperty(), NotifyParentProperty(true)]
		public int DataHeadersPopupMinCount {
			get { return dataHeadersPopupMinCount; }
			set {
				if(value == dataHeadersPopupMinCount) return;
				if(value < 0) value = 0;
				dataHeadersPopupMinCount = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridWebOptionsViewDataHeadersPopupMaxColumnCount"),
#endif
		DefaultValue(-1), XtraSerializableProperty(), NotifyParentProperty(true)]
		public int DataHeadersPopupMaxColumnCount {
			get { return dataHeadersPopupMaxColumnCount; }
			set {
				if(value == dataHeadersPopupMaxColumnCount) return;
				dataHeadersPopupMaxColumnCount = value;
				OnOptionsChanged();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public new bool ShowHorzLines {
			get { return base.ShowHorzLines; }
			set { base.ShowHorzLines = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public new bool ShowVertLines {
			get { return base.ShowVertLines; }
			set { base.ShowVertLines = value; }
		}
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridWebOptionsViewShowContextMenus")]
#endif
		[NotifyParentProperty(true), DefaultValue(true), XtraSerializableProperty]
		public bool ShowContextMenus {
			get { return showContextMenus; }
			set { showContextMenus = value; }
		}
		[
		Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never), XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new int RowTreeWidth {
			get { return base.RowTreeWidth; }
			set { base.RowTreeWidth = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public new bool DrawFocusedCellRect {
			get { return base.DrawFocusedCellRect; }
			set { base.DrawFocusedCellRect = value; }
		}
		[
		Obsolete("This property is obsolete now. Use the PivotGridWebOptionsView.HorizontalScrollBarMode property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridWebOptionsViewShowHorizontalScrollBar"),
#endif
		NotifyParentProperty(true), DefaultValue(false), XtraSerializableProperty()
		]
		public bool ShowHorizontalScrollBar {
			get { return showHorizontalScrollBar; }
			set {
				if(value == showHorizontalScrollBar) return;
				showHorizontalScrollBar = value;
				OnOptionsChanged();
			}
		}
		internal bool ShowHorizontalScrollBarInternal {
			get {
#pragma warning disable 0618 //warning: obsolete
				return ShowHorizontalScrollBar;
#pragma warning restore 0618
			}
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridWebOptionsViewHorizontalScrollBarMode"),
#endif
		DefaultValue(HorizontalScrollBarModeDefault), NotifyParentProperty(true)
		]
		public ScrollBarMode HorizontalScrollBarMode {
			get { return GetViewBagProperty<ScrollBarMode>("HorizontalScrollBarMode", HorizontalScrollBarModeDefault); }
			set {
				if(HorizontalScrollBarMode == value)
					return;
				SetViewBagProperty<ScrollBarMode>("HorizontalScrollBarMode", HorizontalScrollBarModeDefault, value);
				OnOptionsChanged();
			}
		}
		internal ScrollBarMode ActualHorizontalScrollBarMode {
			get { return ShowHorizontalScrollBarInternal ? ScrollBarMode.Auto : HorizontalScrollBarMode; }
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridWebOptionsViewVerticalScrollBarMode"),
#endif
		DefaultValue(VerticalScrollBarModeDefault), NotifyParentProperty(true)
		]
		public ScrollBarMode VerticalScrollBarMode {
			get { return GetViewBagProperty<ScrollBarMode>("VerticalScrollBarMode", VerticalScrollBarModeDefault); }
			set {
				if(VerticalScrollBarMode == value)
					return;
				SetViewBagProperty<ScrollBarMode>("VerticalScrollBarMode", VerticalScrollBarModeDefault, value);
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridWebOptionsViewVerticalScrollingMode"),
#endif
		DefaultValue(VerticalScrollingModeDefault), NotifyParentProperty(true)
		]
		public PivotScrollingMode VerticalScrollingMode {
			get { return GetViewBagProperty<PivotScrollingMode>("VerticalScrollingMode", VerticalScrollingModeDefault); }
			set {
				if(VerticalScrollingMode == value)
					return;
				SetViewBagProperty<PivotScrollingMode>("VerticalScrollingMode", VerticalScrollingModeDefault, value);
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridWebOptionsViewHorizontalScrollingMode"),
#endif
		DefaultValue(HorizontalScrollingModeDefault), NotifyParentProperty(true)
		]
		public PivotScrollingMode HorizontalScrollingMode {
			get { return GetViewBagProperty<PivotScrollingMode>("HorizontalScrollingMode", HorizontalScrollingModeDefault); }
			set {
				if(HorizontalScrollingMode == value)
					return;
				SetViewBagProperty<PivotScrollingMode>("HorizontalScrollingMode", HorizontalScrollingModeDefault, value);
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridWebOptionsViewEnableFilterControlPopupMenuScrolling"),
#endif
		DefaultValue(false), NotifyParentProperty(true)]
		public bool EnableFilterControlPopupMenuScrolling {
			get { return enableFilterControlPopupMenuScrolling; }
			set {
				if(EnableFilterControlPopupMenuScrolling == value)
					return;
				enableFilterControlPopupMenuScrolling = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridWebOptionsViewEnableContextMenuScrolling"),
#endif
		DefaultValue(false), NotifyParentProperty(true)]
		public bool EnableContextMenuScrolling {
			get { return enableContextMenuScrolling; }
			set {
				if(EnableContextMenuScrolling == value)
					return;
				enableContextMenuScrolling = value;
				OnOptionsChanged();
			}
		}
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			PivotGridWebOptionsView optionsView = options as PivotGridWebOptionsView;
			if(optionsView != null) {
				EnableContextMenuScrolling = optionsView.EnableContextMenuScrolling;
				EnableFilterControlPopupMenuScrolling = optionsView.EnableFilterControlPopupMenuScrolling;
				HorizontalScrollBarMode = optionsView.HorizontalScrollBarMode;
				HorizontalScrollingMode = optionsView.HorizontalScrollingMode;
				VerticalScrollBarMode = optionsView.VerticalScrollBarMode;
				VerticalScrollingMode = optionsView.VerticalScrollingMode;
			}
		}
	}
	public class PivotGridWebOptionsCustomization : PivotGridOptionsCustomization {
		#region OptionsChanged
		public enum OptionsCustomizationChangedReason { Unknown, AllowCustomizationWindowResizing };
		public class OptionsCustomizationChangedEventArgs : EventArgs {
			OptionsCustomizationChangedReason reason;
			public OptionsCustomizationChangedEventArgs(OptionsCustomizationChangedReason reason) {
				this.reason = reason;
			}
			public OptionsCustomizationChangedReason Reason { get { return reason; } }
		}
		public delegate void OptionsCustomizationChangedEventHandler(PivotGridWebOptionsCustomization sender, OptionsCustomizationChangedEventArgs e);
		OptionsCustomizationChangedEventHandler webOptionsChanged;
		protected OptionsCustomizationChangedEventHandler WebOptionsChanged { get { return webOptionsChanged; } }
		protected void Changed(OptionsCustomizationChangedReason reason) {
			if(WebOptionsChanged != null) WebOptionsChanged(this, new OptionsCustomizationChangedEventArgs(reason));
		}
		protected void Changed() {
			if(WebOptionsChanged != null) WebOptionsChanged(this, new OptionsCustomizationChangedEventArgs(OptionsCustomizationChangedReason.Unknown));
		}
		#endregion
		bool allowCustomizationWindowResizing;
		int customizationWindowWidth, customizationWindowHeight, customizationExcelWindowWidth, customizationExcelWindowHeight;
		Unit filterPopupWindowWidth, filterPopupWindowHeight, filterPopupWindowMinWidth, filterPopupWindowMinHeight;
		ResizingMode filterPopupWindowResizeMode;
		public PivotGridWebOptionsCustomization(OptionsCustomizationChangedEventHandler webOptionsChanged, EventHandler optionsChanged, IViewBagOwner viewBagOwner, string objectPath)
			: base(optionsChanged, viewBagOwner, objectPath) {
			this.webOptionsChanged = webOptionsChanged;
			customizationWindowWidth = 150;
			customizationWindowHeight = 170;
			customizationExcelWindowWidth = 410;
			customizationExcelWindowHeight = 500;
			allowCustomizationWindowResizing = true;
			filterPopupWindowWidth = Unit.Empty;
			filterPopupWindowHeight = Unit.Empty;
			filterPopupWindowMinWidth = Unit.Pixel(190);
			filterPopupWindowMinHeight = Unit.Empty;
			filterPopupWindowResizeMode = ResizingMode.Live;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public new AllowHideFieldsType AllowHideFields {
			get { return base.AllowHideFields; }
			set { base.AllowHideFields = value; }
		}
		[NotifyParentProperty(true), DefaultValue(true), XtraSerializableProperty]
		public bool AllowCustomizationWindowResizing {
			get { return allowCustomizationWindowResizing; }
			set {
				if(allowCustomizationWindowResizing == value) return;
				allowCustomizationWindowResizing = value;
				Changed(OptionsCustomizationChangedReason.AllowCustomizationWindowResizing);
			}
		}
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridWebOptionsCustomizationCustomizationWindowWidth")]
#endif
		[NotifyParentProperty(true), DefaultValue(150), XtraSerializableProperty]
		public int CustomizationWindowWidth {
			get { return customizationWindowWidth; }
			set { customizationWindowWidth = value; }
		}
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridWebOptionsCustomizationCustomizationWindowHeight")]
#endif
		[NotifyParentProperty(true), DefaultValue(170), XtraSerializableProperty]
		public int CustomizationWindowHeight {
			get { return customizationWindowHeight; }
			set { customizationWindowHeight = value; }
		}
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridWebOptionsCustomizationCustomizationExcelWindowWidth")]
#endif
		[NotifyParentProperty(true), DefaultValue(410), XtraSerializableProperty]
		public int CustomizationExcelWindowWidth {
			get { return customizationExcelWindowWidth; }
			set { customizationExcelWindowWidth = value; }
		}
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridWebOptionsCustomizationCustomizationExcelWindowHeight")]
#endif
		[NotifyParentProperty(true), DefaultValue(500), XtraSerializableProperty]
		public int CustomizationExcelWindowHeight {
			get { return customizationExcelWindowHeight; }
			set { customizationExcelWindowHeight = value; }
		}
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridWebOptionsCustomizationFilterPopupWindowWidth")]
#endif
		[NotifyParentProperty(true), DefaultValue(typeof(Unit), ""), XtraSerializableProperty]		
		public Unit FilterPopupWindowWidth {
			get { return filterPopupWindowWidth; }
			set { filterPopupWindowWidth = value; }
		}
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridWebOptionsCustomizationFilterPopupWindowHeight")]
#endif
		[NotifyParentProperty(true), DefaultValue(typeof(Unit), ""), XtraSerializableProperty]
		public Unit FilterPopupWindowHeight {
			get { return filterPopupWindowHeight; }
			set { filterPopupWindowHeight = value; }
		}
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridWebOptionsCustomizationFilterPopupWindowMinWidth")]
#endif
		[NotifyParentProperty(true), DefaultValue(typeof(Unit), "190"), XtraSerializableProperty]
		public Unit FilterPopupWindowMinWidth {
			get { return filterPopupWindowMinWidth; }
			set { filterPopupWindowMinWidth = value; }
		}
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridWebOptionsCustomizationFilterPopupWindowMinHeight")]
#endif
		[NotifyParentProperty(true), DefaultValue(typeof(Unit), ""), XtraSerializableProperty]
		public Unit FilterPopupWindowMinHeight {
			get { return filterPopupWindowMinHeight; }
			set { filterPopupWindowMinHeight = value; }
		}
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridWebOptionsCustomizationFilterPopupWindowResizeMode")]
#endif
		[NotifyParentProperty(true), DefaultValue(ResizingMode.Live), XtraSerializableProperty]
		public ResizingMode FilterPopupWindowResizeMode {
			get { return filterPopupWindowResizeMode; }
			set { filterPopupWindowResizeMode = value; }
		}
	}
	public class PivotGridWebOptionsDataField : PivotGridOptionsDataField {
		public PivotGridWebOptionsDataField(PivotGridData data)
			: base(data) {
		}		
		public PivotGridWebOptionsDataField(PivotGridData data, IViewBagOwner owner, string objectPath)
			: base(data, owner, objectPath) {
		}
		public PivotGridWebOptionsDataField(PivotGridData data, EventHandler optionsChanged, IViewBagOwner owner, string objectPath)
			: base(data, optionsChanged, owner, objectPath) {
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public new int RowHeaderWidth {
			get { return base.RowHeaderWidth; }
			set { base.RowHeaderWidth = value; }
		}
	}
	public class PivotGridWebOptionsData : PivotGridOptionsData {
		public PivotGridWebOptionsData(PivotGridData data, EventHandler optionsChanged)
			: base(data, optionsChanged) { }
		DefaultBoolean lockDataRefreshOnCustomCallback = DefaultBoolean.Default;
		internal bool HasLockDataRefreshOnCustomCallbackValue() {
			return lockDataRefreshOnCustomCallback != DefaultBoolean.Default;
		}
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridWebOptionsDataLockDataRefreshOnCustomCallback")]
#endif
		[NotifyParentProperty(true), DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(), AutoFormatDisable()]
		public DefaultBoolean LockDataRefreshOnCustomCallback {
			get { return lockDataRefreshOnCustomCallback; }
			set { lockDataRefreshOnCustomCallback = value; }
		}
	}
	public class PivotGridWebOptionsLoadingPanel : PivotGridOptionsBase {
		ASPxPivotGrid owner;
		public PivotGridWebOptionsLoadingPanel(ASPxPivotGrid owner)
			: base(null, null, string.Empty) {
			this.owner = owner;
		}
		protected ASPxPivotGrid Owner { get { return owner; } }
		protected virtual SettingsLoadingPanel Settings { get { return Owner.SettingsLoadingPanel; } }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridWebOptionsLoadingPanelImage"),
#endif
		Category("LoadingPanel"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable()
		]
		public ImageProperties Image { get { return ImageInternal; } }
		protected virtual ImageProperties ImageInternal { get { return Owner.LoadingPanelImage; } }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridWebOptionsLoadingPanelImagePosition"),
#endif
		Category("LoadingPanel"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.Attribute),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		XtraSerializableProperty(XtraSerializationVisibility.Visible), AutoFormatEnable(),
		DefaultValue(ImagePosition.Left)
		]
		public ImagePosition ImagePosition {
			get { return Settings.ImagePosition; }
			set { Settings.ImagePosition = value; }
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridWebOptionsLoadingPanelStyle"),
#endif
		Category("LoadingPanel"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable()
		]
		public LoadingPanelStyle Style { get { return StyleInternal; } }
		protected virtual LoadingPanelStyle StyleInternal { get { return Owner.LoadingPanelStyle; } }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridWebOptionsLoadingPanelText"),
#endif
		Category("LoadingPanel"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.Attribute),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		XtraSerializableProperty(),
		DefaultValue(StringResources.LoadingPanelText), Localizable(true), AutoFormatEnable()
		]
		public string Text { 
			get { return Settings.Text; }
			set { Settings.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridWebOptionsLoadingPanelEnabled"),
#endif
		Category("LoadingPanel"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.Attribute),
		XtraSerializableProperty(),
		DefaultValue(true), AutoFormatEnable()
		]
		public bool Enabled { 
			get { return Settings.Enabled; }
			set { Settings.Enabled = value; }
		}
		public override void Assign(BaseOptions source) {
			base.Assign(source);
			PivotGridWebOptionsLoadingPanel options = source as PivotGridWebOptionsLoadingPanel;
			if (options != null) {
				Image.CopyFrom(options.Image);
				Style.CopyFrom(options.Style);
			}
		}
	}
	public enum PagerAlign { Left, Center, Right, Justify };	
	public class PivotGridWebOptionsPager : PagerSettingsEx {
		const int DefaultPageIndex = 0;
		const int DefaultColumnPageIndex = 0;
		const bool DefaultAlwaysShowPager = false;
		public enum OptionsPagerChangedReason { Unknown, PagerAlign, RowsPerPage, ColumnsPerPage, PageIndex, ColumnPageIndex, AlwaysShowPager };
		public class OptionsPagerChangedEventArgs : EventArgs {
			OptionsPagerChangedReason reason;
			public OptionsPagerChangedReason Reason { get { return reason; } }
			public OptionsPagerChangedEventArgs(OptionsPagerChangedReason reason) {
				this.reason = reason;
			}
		}
		public delegate void OptionsPagerChangedEventHandler(PivotGridWebOptionsPager sender, OptionsPagerChangedEventArgs e);
		OptionsPagerChangedEventHandler optionsChanged;
		protected OptionsPagerChangedEventHandler OptionsChanged { get { return optionsChanged; } }
		public PivotGridWebOptionsPager(OptionsPagerChangedEventHandler optionsChanged, IViewBagOwner viewBagOwner, string objectPath)
			: base() {
			this.optionsChanged = optionsChanged;
		}
		protected void Changed(OptionsPagerChangedReason reason) {
			if(OptionsChanged != null) OptionsChanged(this, new OptionsPagerChangedEventArgs(reason));
		}
		protected override void Changed() {
			if(OptionsChanged != null) OptionsChanged(this, new OptionsPagerChangedEventArgs(OptionsPagerChangedReason.Unknown));
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridWebOptionsPagerPagerAlign"),
#endif
		DefaultValue(PagerAlign.Left), XtraSerializableProperty, NotifyParentProperty(true),
		AutoFormatEnable()]
		public PagerAlign PagerAlign {
			get { return (PagerAlign)GetEnumProperty("PagerAlign", PagerAlign.Left); }
			set {
				if(PagerAlign != value) {
					SetEnumProperty("PagerAlign", PagerAlign.Left, value);
					Changed(OptionsPagerChangedReason.PagerAlign);
				}
			}
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridWebOptionsPagerRowsPerPage"),
#endif
		DefaultValue(10), XtraSerializableProperty, NotifyParentProperty(true), AutoFormatEnable()]
		public int RowsPerPage {
			get { return GetIntProperty("RowsPerPage", 10); }
			set {
				if(RowsPerPage != value && value >= 0) {
					SetIntProperty("RowsPerPage", 10, value);
					Changed(OptionsPagerChangedReason.RowsPerPage);
				}
			}
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridWebOptionsPagerColumnsPerPage"),
#endif
		DefaultValue(10), XtraSerializableProperty, NotifyParentProperty(true), AutoFormatEnable()]
		public int ColumnsPerPage {
			get { return GetIntProperty("ColumnsPerPage", 10); }
			set {
				if(ColumnsPerPage != value && value >= 0) {
					SetIntProperty("ColumnsPerPage", 10, value);
					Changed(OptionsPagerChangedReason.ColumnsPerPage);
				}
			}
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridWebOptionsPagerPageIndex"),
#endif
		DefaultValue(DefaultPageIndex), XtraSerializableProperty, NotifyParentProperty(true), AutoFormatDisable()]
		public int PageIndex {
			get { return GetIntProperty("PageIndex", 0); }
			set {
				if(PageIndex != value && value >= -1) {
					SetIntProperty("PageIndex", 0, value);
					Changed(OptionsPagerChangedReason.PageIndex);
				}
			}
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridWebOptionsPagerColumnPageIndex"),
#endif
		DefaultValue(DefaultColumnPageIndex), XtraSerializableProperty, NotifyParentProperty(true), AutoFormatDisable()]
		public int ColumnPageIndex {
			get { return GetIntProperty("ColumnPageIndex", DefaultColumnPageIndex); }
			set {
				if(ColumnPageIndex != value && value >= -1) {
					SetIntProperty("ColumnPageIndex", 0, value);
					Changed(OptionsPagerChangedReason.ColumnPageIndex);
				}
			}
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridWebOptionsPagerAlwaysShowPager"),
#endif
		DefaultValue(DefaultAlwaysShowPager), XtraSerializableProperty, NotifyParentProperty(true), AutoFormatDisable()]
		public bool AlwaysShowPager {
			get { return GetBoolProperty("AlwaysShowPager", DefaultAlwaysShowPager); }
			set {
				if(AlwaysShowPager != value) {
					SetBoolProperty("AlwaysShowPager", DefaultAlwaysShowPager, value);
					Changed(OptionsPagerChangedReason.AlwaysShowPager);
				}
			}
		}
		[Browsable(false)]
		public bool HasPager(bool isColumn) {
			return !isColumn && Visible && (AlwaysShowPager || RowsPerPage > 0);
		}
		internal void Reset() {
			PageIndex = 0;
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			PivotGridWebOptionsPager options = source as PivotGridWebOptionsPager;
			if (options != null) {
				PagerAlign = options.PagerAlign;
				RowsPerPage = options.RowsPerPage;
				PageIndex = options.PageIndex;
				AlwaysShowPager = options.AlwaysShowPager;
				ColumnsPerPage = options.ColumnsPerPage;
				ColumnPageIndex = options.ColumnPageIndex;
			}
		}
		protected internal bool IsPageSizeVisible() {
			return PageSizeItemSettings.Visible;
		}
	}
	public class PivotGridWebFieldOptions : PivotGridFieldOptions {
		public PivotGridWebFieldOptions(PivotOptionsChangedEventHandler optionsChanged, IViewBagOwner viewBagOwner, string objectPath)
			: base(optionsChanged, viewBagOwner, objectPath) { }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool AllowRunTimeSummaryChange { get { return false; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool ShowSummaryTypeName { get { return false; } }
	}
	public class PivotGridWebOptionsChartDataSource : PivotGridOptionsChartDataSourceBase {
		bool currentPageOnly = true;
		public PivotGridWebOptionsChartDataSource(PivotGridWebData data) : base(data) { }
		public override void Assign(BaseOptions options) {
			try {
				options.BeginUpdate();
				base.Assign(options);
				PivotGridWebOptionsChartDataSource opts = options as PivotGridWebOptionsChartDataSource;
				if(opts == null)
					return;
				CurrentPageOnly = opts.CurrentPageOnly; 
			} finally {
				options.EndUpdate();
			}
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridWebOptionsChartDataSourceCurrentPageOnly"),
#endif
		PersistenceMode(PersistenceMode.Attribute), NotifyParentProperty(true), XtraSerializableProperty(),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), DefaultValue(true)
		]
		public bool CurrentPageOnly { get { return currentPageOnly; } set { currentPageOnly = value; } }
	}
	public class PivotGridWebOptionsLayout : PivotGridOptionsLayout {
		static PivotGridWebOptionsLayout defaultLayout;
		static PivotGridWebOptionsLayout fullLayout;
		static PivotGridWebOptionsLayout callbackStateFullLayout;
		static PivotGridWebOptionsLayout callbackStateLayout;
		public static PivotGridWebOptionsLayout DefaultLayout {
			get {
				if(defaultLayout == null) {
					defaultLayout = new PivotGridWebOptionsLayout();
					defaultLayout.StoreAppearance = true;
					defaultLayout.Columns.AddNewColumns = false;
					defaultLayout.Columns.RemoveOldColumns = false;
					defaultLayout.Columns.StoreAppearance = true;
				}
				return defaultLayout;
			}
		}
		public static new PivotGridWebOptionsLayout FullLayout {
			get {
				if(fullLayout == null) {
					fullLayout = new PivotGridWebOptionsLayout();
					fullLayout.StoreClientSideEvents = true;
				}
				return fullLayout;
			}
		}
		internal static PivotGridWebOptionsLayout CallbackStateFullLayout {
			get {
				if(callbackStateFullLayout == null) {
					callbackStateFullLayout = new PivotGridWebOptionsLayout();
					PrepareCallbackStateOptions(callbackStateFullLayout);
					callbackStateFullLayout.StoreClientSideEvents = true;
				}
				return callbackStateFullLayout;
			}
		}
		internal static PivotGridWebOptionsLayout CallbackStateLayout {
			get {
				if(callbackStateLayout == null) {
					callbackStateLayout = new PivotGridWebOptionsLayout();
					PrepareCallbackStateOptions(callbackStateLayout);
				}
				return callbackStateLayout;
			}
		}
		 static void PrepareCallbackStateOptions(PivotGridWebOptionsLayout options) {
			 if(DevExpress.Data.Helpers.SecurityHelper.IsPartialTrust) 
				 options.StoreViewStateOptions = false;
		}
		bool storeClientSideEvents;
		PivotGridResetOptions resetOptions;
		const PivotGridResetOptions ResetOptionsDefault = PivotGridResetOptions.OptionsChartDataSource | PivotGridResetOptions.OptionsDataField | PivotGridResetOptions.OptionsData |
													  PivotGridResetOptions.OptionsCustomization | PivotGridResetOptions.OptionsLoadingPanel;
		public PivotGridWebOptionsLayout() {
			this.storeClientSideEvents = false;
			this.StoreViewStateOptions = true;
			this.resetOptions = ResetOptionsDefault;
		}
		[
		DefaultValue(false), XtraSerializableProperty,
		NotifyParentProperty(true), AutoFormatDisable
		]
		public bool StoreClientSideEvents {
			get { return storeClientSideEvents; }
			set { storeClientSideEvents = value; }
		}
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			BeginUpdate();
			try {
				PivotGridWebOptionsLayout opt = options as PivotGridWebOptionsLayout;
				if(opt == null) return;
				this.storeClientSideEvents = opt.storeClientSideEvents;
				this.ResetOptions = opt.ResetOptions;
				this.StoreViewStateOptions = opt.StoreViewStateOptions;
			} finally {
				EndUpdate();
			}
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridWebOptionsLayoutResetOptions"),
#endif
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Web.ASPxPivotGrid.PivotGridWebOptionsLayout.ResetOptions"),
		XtraSerializableProperty, NotifyParentProperty(true),
		Editor("DevExpress.Utils.Editors.AttributesEditor, " + AssemblyInfo.SRAssemblyUtils, typeof(System.Drawing.Design.UITypeEditor)),
		DefaultValue(ResetOptionsDefault)
		]
		public new PivotGridResetOptions ResetOptions {
			get { return resetOptions; }
			set { resetOptions = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool StoreLayoutOptions {
			get { return false; }
			set { }
		}
		internal bool StoreViewStateOptions { get; set; }
	}
	public class PivotGridWebOptionsBehavior : PivotGridOptionsBehaviorBase {
		public PivotGridWebOptionsBehavior(EventHandler optionsChanged)
			: base(optionsChanged) { }
		[
		Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new CopyMultiSelectionMode ClipboardCopyMultiSelectionMode {
			get { return base.ClipboardCopyMultiSelectionMode; }
			set { base.ClipboardCopyMultiSelectionMode = value; }
		}
		[
		Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new bool CopyToClipboardWithFieldValues {
			get { return base.CopyToClipboardWithFieldValues; }
			set { base.CopyToClipboardWithFieldValues = value; }
		}
		[
		Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new CopyCollapsedValuesMode ClipboardCopyCollapsedValuesMode {
			get { return base.ClipboardCopyCollapsedValuesMode; }
			set { base.ClipboardCopyCollapsedValuesMode = value; }
		}
		[
		Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new int LoadingPanelDelay {
			get { return base.LoadingPanelDelay; }
			set { base.LoadingPanelDelay = value; }
		}
		[
		Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new bool UseAsyncMode {
			get { return base.UseAsyncMode; }
			set { base.UseAsyncMode = value; }
		}
	}
	public class PivotGridWebOptionsFilter : PivotGridOptionsFilterBase {
		bool showHiddenItems;
		bool nativeCheckBoxes;
		public PivotGridWebOptionsFilter(PivotOptionsFilterEventHandler optionsChanged, IViewBagOwner viewBagOwner, string projectPath)
			: base(optionsChanged, viewBagOwner, projectPath) {
			showHiddenItems = false;
			nativeCheckBoxes = true;
		}
		[DefaultValue(PivotGroupFilterMode.Tree), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable()]
		public new PivotGroupFilterMode GroupFilterMode {
			get { return base.GroupFilterMode; }
			set { base.GroupFilterMode = value; }
		}
		[DefaultValue(false), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable()]
		public new bool ShowOnlyAvailableItems {
			get { return base.ShowOnlyAvailableItems; }
			set { base.ShowOnlyAvailableItems = value; }
		}
		[DefaultValue(false), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable()]
		public bool ShowHiddenItems {
			get { return showHiddenItems; }
			set { showHiddenItems = value; }
		}
		[DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable()]
		public bool NativeCheckBoxes {
			get { return nativeCheckBoxes; }
			set { nativeCheckBoxes = value; }
		}
	}
	public class LayoutIds {
		public const int LayoutIdAppearance = 1;
		public const int LayoutIdData = 2;
		public const int LayoutIdLayout = 3;
		public const int LayoutIdOptionsView = 4;
		public const int LayoutIdColumns = 5;
		public const int ClientSideEvents = 6;
		public const int LayoutIdViewState = 7;
	}
	[Flags]
	public enum PivotGridResetOptions {
		None = 0,
		OptionsBehavior = 1,
		OptionsChartDataSource = 2,
		OptionsCustomization = 4,
		OptionsData = 8,
		OptionsDataField = 16,
		OptionsFilterPopup = 32,
		OptionsOLAP = 256,
		OptionsLoadingPanel = 2048,
		OptionsPager = 4096,
		All = OptionsBehavior | OptionsChartDataSource | OptionsCustomization |
				OptionsData | OptionsDataField | OptionsFilterPopup |
				OptionsOLAP | OptionsLoadingPanel | OptionsPager
	}
}
