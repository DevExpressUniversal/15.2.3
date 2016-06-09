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
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Documents;
using DevExpress.Data;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Data;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Validation;
using DevExpress.Xpf.Grid.LookUp.Native;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Xpf.Utils;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.Utils;
using DevExpress.Core;
#if !SL
using System.Windows.Media.Animation;
using DevExpress.XtraPrinting.DataNodes;
using DevExpress.Data.Utils.ServiceModel;
using DevExpress.Xpf.Printing.BrickCollection;
using DevExpress.Xpf.Grid.Printing;
#else
using Storyboard = System.String;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using DependencyPropertyChangedEventHandler = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventHandler;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using RoutedEventHandler = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventHandler;
#endif
namespace DevExpress.Xpf.Grid {
	[SelectedItemsSourceBrowsableAttribute]
	public abstract partial class GridViewBase : GridDataViewBase {
		#region dependency properties fields
		public static readonly DependencyProperty ShowGroupedColumnsProperty;
		public static readonly DependencyProperty ShowGroupPanelProperty;
		static readonly DependencyPropertyKey IsGroupPanelVisiblePropertyKey;
		public static readonly DependencyProperty IsGroupPanelVisibleProperty;
		protected static readonly DependencyPropertyKey IsGroupPanelTextVisiblePropertyKey;
		public static readonly DependencyProperty IsGroupPanelTextVisibleProperty;
		static readonly DependencyPropertyKey VisibleColumnsPropertyKey;
		public static readonly DependencyProperty VisibleColumnsProperty;
		public static readonly DependencyProperty AllowGroupingProperty;
		public static readonly DependencyProperty UseAnimationWhenExpandingProperty;
		public static readonly DependencyProperty GroupRowStyleProperty;
		public static readonly DependencyProperty GroupValueContentStyleProperty;
		public static readonly DependencyProperty GroupSummaryContentStyleProperty;
		public static readonly DependencyProperty DefaultGroupSummaryItemTemplateProperty;
		public static readonly DependencyProperty GroupSummaryItemTemplateProperty;
		public static readonly DependencyProperty GroupSummaryItemTemplateSelectorProperty;
		static readonly DependencyPropertyKey ActualGroupSummaryItemTemplateSelectorPropertyKey;
		public static readonly DependencyProperty ActualGroupSummaryItemTemplateSelectorProperty;
		public static readonly DependencyProperty GroupValueTemplateProperty;
		public static readonly DependencyProperty GroupValueTemplateSelectorProperty;
		public static readonly DependencyProperty GroupRowTemplateProperty;
		public static readonly DependencyProperty GroupRowTemplateSelectorProperty;
		static readonly DependencyPropertyKey ActualGroupRowTemplateSelectorPropertyKey;
		public static readonly DependencyProperty ActualGroupRowTemplateSelectorProperty;
		public static readonly DependencyProperty IsGroupPanelMenuEnabledProperty;
		public static readonly DependencyProperty IsGroupFooterMenuEnabledProperty;
		public static readonly DependencyProperty FocusedColumnProperty;
		public static readonly DependencyProperty AllowDateTimeGroupIntervalMenuProperty;
		public static readonly DependencyProperty ImmediateUpdateRowPositionProperty;
		public static readonly DependencyProperty ExpandStoryboardProperty;
		public static readonly DependencyProperty CollapseStoryboardProperty;
		public static readonly RoutedEvent RowUpdatedEvent;
		public static readonly RoutedEvent RowCanceledEvent;
		public static readonly RoutedEvent CellValueChangedEvent;
		public static readonly RoutedEvent CellValueChangingEvent;
		public static readonly RoutedEvent InvalidRowExceptionEvent;
		public static readonly RoutedEvent SelectionChangedEvent;
		public static readonly RoutedEvent CopyingToClipboardEvent;
		public static readonly RoutedEvent ShowingEditorEvent;
		public static readonly RoutedEvent ShownEditorEvent;
		public static readonly RoutedEvent HiddenEditorEvent;
		static readonly DependencyPropertyKey ActualShowCheckBoxSelectorInGroupRowPropertyKey;
		public static readonly DependencyProperty ActualShowCheckBoxSelectorInGroupRowProperty;
		public static readonly DependencyProperty IsGroupRowMenuEnabledProperty;
		#region Printing
		public static readonly DependencyProperty PrintGroupRowTemplateProperty;
		public static readonly DependencyProperty PrintGroupRowStyleProperty;
		public static readonly DependencyProperty PrintAllGroupsProperty;
		#endregion
		#endregion
		#region cctor
		static GridViewBase() {
			Type ownerType = typeof(GridViewBase);
			GridViewSelectionControlWrapper.Register();
#if !SL
			ClipboardCopyMaxRowCountInServerModeProperty = DependencyPropertyManager.Register("ClipboardCopyMaxRowCountInServerMode", typeof(int), ownerType, new FrameworkPropertyMetadata(1000));
#endif
			ShowGroupedColumnsProperty = DependencyPropertyManager.Register("ShowGroupedColumns", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, (d, e) => ((GridViewBase)d).RebuildVisibleColumns()));
			ShowGroupPanelProperty = DependencyPropertyManager.Register("ShowGroupPanel", typeof(bool), ownerType, new PropertyMetadata(true, (d, e) => ((GridViewBase)d).UpdateMasterDetailViewProperties()));
			IsGroupPanelVisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsGroupPanelVisible", typeof(bool), ownerType, new PropertyMetadata(true));
			IsGroupPanelVisibleProperty = IsGroupPanelVisiblePropertyKey.DependencyProperty;
			IsGroupPanelTextVisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsGroupPanelTextVisible", typeof(bool), ownerType, new PropertyMetadata(true));
			IsGroupPanelTextVisibleProperty = IsGroupPanelTextVisiblePropertyKey.DependencyProperty;
			VisibleColumnsPropertyKey = DependencyPropertyManager.RegisterReadOnly("VisibleColumns", typeof(IList<GridColumn>), ownerType, new FrameworkPropertyMetadata(null));
			VisibleColumnsProperty = VisibleColumnsPropertyKey.DependencyProperty;
			AllowGroupingProperty = DependencyPropertyManager.Register("AllowGrouping", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, OnUpdateColumnsViewInfo));
			RowUpdatedEvent = EventManager.RegisterRoutedEvent("RowUpdated", RoutingStrategy.Direct, typeof(RowEventHandler), ownerType);
			RowCanceledEvent = EventManager.RegisterRoutedEvent("RowCanceled", RoutingStrategy.Direct, typeof(RowEventHandler), ownerType);
			CellValueChangedEvent = EventManager.RegisterRoutedEvent("CellValueChanged", RoutingStrategy.Direct, typeof(CellValueChangedEventHandler), ownerType);
			CellValueChangingEvent = EventManager.RegisterRoutedEvent("CellValueChanging", RoutingStrategy.Direct, typeof(CellValueChangedEventHandler), ownerType);
			InvalidRowExceptionEvent = EventManager.RegisterRoutedEvent("InvalidRowException", RoutingStrategy.Direct, typeof(InvalidRowExceptionEventHandler), ownerType);
			SelectionChangedEvent = EventManager.RegisterRoutedEvent("SelectionChanged", RoutingStrategy.Direct, typeof(GridSelectionChangedEventHandler), ownerType);
			CopyingToClipboardEvent = EventManager.RegisterRoutedEvent("CopyingToClipboard", RoutingStrategy.Direct, typeof(CopyingToClipboardEventHandler), ownerType);
			UseAnimationWhenExpandingProperty = DependencyPropertyManager.Register("UseAnimationWhenExpanding", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			FocusedColumnProperty = DependencyPropertyManager.Register("FocusedColumn", typeof(GridColumn), ownerType, new FrameworkPropertyMetadata(null, OnFocusedColumnChanged, (d, e) => ((GridViewBase)d).CoerceFocusedColumn((ColumnBase)e)));
			DefaultGroupSummaryItemTemplateProperty = DependencyPropertyManager.Register("DefaultGroupSummaryItemTemplate", typeof(DataTemplate), ownerType, new FrameworkPropertyMetadata(null));
			GroupSummaryItemTemplateProperty = DependencyPropertyManager.Register("GroupSummaryItemTemplate", typeof(DataTemplate), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((GridViewBase)d).UpdateActualGroupSummaryItemTemplateSelector()));
			GroupSummaryItemTemplateSelectorProperty = DependencyPropertyManager.Register("GroupSummaryItemTemplateSelector", typeof(DataTemplateSelector), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((GridViewBase)d).UpdateActualGroupSummaryItemTemplateSelector()));
			ActualGroupSummaryItemTemplateSelectorPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualGroupSummaryItemTemplateSelector", typeof(DataTemplateSelector), ownerType, new FrameworkPropertyMetadata(null));
			ActualGroupSummaryItemTemplateSelectorProperty = ActualGroupSummaryItemTemplateSelectorPropertyKey.DependencyProperty;
			GroupValueTemplateProperty = DependencyPropertyManager.Register("GroupValueTemplate", typeof(DataTemplate), ownerType, new PropertyMetadata(null, (d, e) => ((GridViewBase)d).UpdateColumnsActualGroupValueTemplateSelector()));
			GroupValueTemplateSelectorProperty = DependencyPropertyManager.Register("GroupValueTemplateSelector", typeof(DataTemplateSelector), ownerType, new PropertyMetadata(null, (d, e) => ((GridViewBase)d).UpdateColumnsActualGroupValueTemplateSelector()));
			GroupRowTemplateProperty = DependencyPropertyManager.Register("GroupRowTemplate", typeof(DataTemplate), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((GridViewBase)d).UpdateActualGroupRowTemplateSelector()));
			GroupRowTemplateSelectorProperty = DependencyPropertyManager.Register("GroupRowTemplateSelector", typeof(DataTemplateSelector), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((GridViewBase)d).UpdateActualGroupRowTemplateSelector()));
			ActualGroupRowTemplateSelectorPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualGroupRowTemplateSelector", typeof(DataTemplateSelector), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((GridViewBase)d).OnActualGroupRowTemplateSelectorChanged()));
			ActualGroupRowTemplateSelectorProperty = ActualGroupRowTemplateSelectorPropertyKey.DependencyProperty;
			GroupRowStyleProperty = DependencyPropertyManager.Register("GroupRowStyle", typeof(Style), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((GridViewBase)d).OnGroupRowStyleChanged()));
			GroupValueContentStyleProperty = DependencyPropertyManager.Register("GroupValueContentStyle", typeof(Style), ownerType, new PropertyMetadata(null, (d, e) => ((GridViewBase)d).OnGroupValueContentStyleChanged()));
			GroupSummaryContentStyleProperty = DependencyPropertyManager.Register("GroupSummaryContentStyle", typeof(Style), ownerType, new PropertyMetadata(null, (d, e) => ((GridViewBase)d).OnGroupSummaryContentStyleChanged()));
			IsGroupPanelMenuEnabledProperty = DependencyPropertyManager.Register("IsGroupPanelMenuEnabled", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			IsGroupFooterMenuEnabledProperty = DependencyPropertyManager.Register("IsGroupFooterMenuEnabled", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			IsGroupRowMenuEnabledProperty = DependencyPropertyManager.Register("IsGroupRowMenuEnabled", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			AllowDateTimeGroupIntervalMenuProperty = DependencyPropertyManager.Register("AllowDateTimeGroupIntervalMenu", typeof(bool), ownerType, new PropertyMetadata(true));
			ImmediateUpdateRowPositionProperty = DependencyPropertyManager.Register("ImmediateUpdateRowPosition", typeof(bool), typeof(GridViewBase), new PropertyMetadata(true, (d, e) => ((GridViewBase)d).RefreshImmediateUpdateRowPositionProperty()));
			ExpandStoryboardProperty = DependencyPropertyManager.RegisterAttached("ExpandStoryboard", typeof(Storyboard), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ExpandHelper.SetExpandStoryboard(d, (Storyboard)e.NewValue)));
			CollapseStoryboardProperty = DependencyPropertyManager.RegisterAttached("CollapseStoryboard", typeof(Storyboard), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ExpandHelper.SetCollapseStoryboard(d, (Storyboard)e.NewValue)));
			ShowingEditorEvent = EventManager.RegisterRoutedEvent("ShowingEditor", RoutingStrategy.Direct, typeof(ShowingEditorEventHandler), ownerType);
			ShownEditorEvent = EventManager.RegisterRoutedEvent("ShownEditor", RoutingStrategy.Direct, typeof(EditorEventHandler), ownerType);
			HiddenEditorEvent = EventManager.RegisterRoutedEvent("HiddenEditor", RoutingStrategy.Direct, typeof(EditorEventHandler), ownerType);
			RegisterClassCommandBindings();
			ActualShowCheckBoxSelectorInGroupRowPropertyKey = DependencyProperty.RegisterReadOnly("ActualShowCheckBoxSelectorInGroupRow", typeof(bool), ownerType, new PropertyMetadata(false, (d, e) => ((GridViewBase)d).OnActualShowCheckBoxSelectorInGroupRowChanged()));
			ActualShowCheckBoxSelectorInGroupRowProperty = ActualShowCheckBoxSelectorInGroupRowPropertyKey.DependencyProperty;
			#region Printing
			PrintGroupRowTemplateProperty = DependencyProperty.Register("PrintGroupRowTemplate", typeof(DataTemplate), ownerType, new UIPropertyMetadata(null));
			PrintGroupRowStyleProperty = DependencyProperty.Register("PrintGroupRowStyle", typeof(Style), ownerType, new FrameworkPropertyMetadata(null));
			PrintAllGroupsProperty = DependencyProperty.Register("PrintAllGroups", typeof(bool), ownerType, new UIPropertyMetadata(true));
			#endregion
		}
		protected static SimpleBridgeList<GridColumn, ColumnBase> ConvertToGridColumnsList(IList<ColumnBase> columns) {
			return new SimpleBridgeList<GridColumn, ColumnBase>(columns);
		}
		static partial void RegisterClassCommandBindings();
		#endregion
		#region dependency properties accessors
		[Obsolete("Use the DataControlBase.CurrentColumn property instead"), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), CloneDetailMode(CloneDetailMode.Skip)]
		public GridColumn FocusedColumn {
			get { return (GridColumn)GetValue(FocusedColumnProperty); }
			set { SetValue(FocusedColumnProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("GridViewBaseUseAnimationWhenExpanding"),
#endif
 Category(Categories.OptionsBehavior), XtraSerializableProperty]
		public bool UseAnimationWhenExpanding {
			get { return (bool)GetValue(UseAnimationWhenExpandingProperty); }
			set { SetValue(UseAnimationWhenExpandingProperty, value); }
		}
		[Category("Behavior")]
		public event RowEventHandler RowUpdated {
			add { AddHandler(RowUpdatedEvent, value); }
			remove { RemoveHandler(RowUpdatedEvent, value); }
		}
		[Category("Behavior")]
		public event RowEventHandler RowCanceled {
			add { AddHandler(RowCanceledEvent, value); }
			remove { RemoveHandler(RowCanceledEvent, value); }
		}
		[Category("Behavior")]
		public event CellValueChangedEventHandler CellValueChanged {
			add { AddHandler(CellValueChangedEvent, value); }
			remove { RemoveHandler(CellValueChangedEvent, value); }
		}
		[Category("Behavior")]
		public event CellValueChangedEventHandler CellValueChanging {
			add { AddHandler(CellValueChangingEvent, value); }
			remove { RemoveHandler(CellValueChangingEvent, value); }
		}
		[Category("Behavior")]
		public event InvalidRowExceptionEventHandler InvalidRowException {
			add { AddHandler(InvalidRowExceptionEvent, value); }
			remove { RemoveHandler(InvalidRowExceptionEvent, value); }
		}
		[Obsolete("Use the GridControl.SelectionChanged event instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public event GridSelectionChangedEventHandler SelectionChanged {
			add { AddHandler(SelectionChangedEvent, value); }
			remove { RemoveHandler(SelectionChangedEvent, value); }
		}
		[Obsolete("Use the GridControl.CopyingToClipboard event instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Category("Behavior")]
		public event CopyingToClipboardEventHandler CopyingToClipboard {
			add { AddHandler(CopyingToClipboardEvent, value); }
			remove { RemoveHandler(CopyingToClipboardEvent, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("GridViewBaseAllowGrouping"),
#endif
 Category(Categories.OptionsBehavior), XtraSerializableProperty]
		public bool AllowGrouping {
			get { return (bool)GetValue(AllowGroupingProperty); }
			set { SetValue(AllowGroupingProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("GridViewBaseAllowDateTimeGroupIntervalMenu"),
#endif
 Category(Categories.OptionsBehavior), XtraSerializableProperty]
		public bool AllowDateTimeGroupIntervalMenu {
			get { return (bool)GetValue(AllowDateTimeGroupIntervalMenuProperty); }
			set { SetValue(AllowDateTimeGroupIntervalMenuProperty, value); }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridViewBaseVisibleColumns")]
#endif
		public IList<GridColumn> VisibleColumns {
			get { return (IList<GridColumn>)GetValue(VisibleColumnsProperty); }
			protected set { this.SetValue(VisibleColumnsPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("GridViewBaseShowGroupPanel"),
#endif
 Category(Categories.OptionsView), XtraSerializableProperty, GridUIProperty]
		public bool ShowGroupPanel {
			get { return (bool)GetValue(ShowGroupPanelProperty); }
			set { SetValue(ShowGroupPanelProperty, value); }
		}
		public bool IsGroupPanelVisible {
			get { return (bool)GetValue(IsGroupPanelVisibleProperty); }
			private set { this.SetValue(IsGroupPanelVisiblePropertyKey, value); }
		}
		public bool IsGroupPanelTextVisible {
			get { return (bool)GetValue(IsGroupPanelTextVisibleProperty); }
			private set { this.SetValue(IsGroupPanelTextVisiblePropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("GridViewBaseShowGroupedColumns"),
#endif
 Category(Categories.OptionsView), XtraSerializableProperty]
		public bool ShowGroupedColumns {
			get { return (bool)base.GetValue(ShowGroupedColumnsProperty); }
			set { SetValue(ShowGroupedColumnsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("GridViewBaseGroupValueContentStyle"),
#endif
 Category(Categories.Appearance)]
		public Style GroupValueContentStyle {
			get { return (Style)GetValue(GroupValueContentStyleProperty); }
			set { SetValue(GroupValueContentStyleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("GridViewBaseGroupSummaryContentStyle"),
#endif
 Category(Categories.Appearance)]
		public Style GroupSummaryContentStyle {
			get { return (Style)GetValue(GroupSummaryContentStyleProperty); }
			set { SetValue(GroupSummaryContentStyleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("GridViewBaseGroupRowStyle"),
#endif
 Category(Categories.Appearance)]
		public Style GroupRowStyle {
			get { return (Style)GetValue(GroupRowStyleProperty); }
			set { SetValue(GroupRowStyleProperty, value); }
		}
		[Browsable(false), Category(Categories.Appearance)]
		public DataTemplate DefaultGroupSummaryItemTemplate {
			get { return (DataTemplate)GetValue(DefaultGroupSummaryItemTemplateProperty); }
			set { SetValue(DefaultGroupSummaryItemTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("GridViewBaseGroupSummaryItemTemplate"),
#endif
 Category(Categories.Appearance)]
		public DataTemplate GroupSummaryItemTemplate {
			get { return (DataTemplate)GetValue(GroupSummaryItemTemplateProperty); }
			set { SetValue(GroupSummaryItemTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("GridViewBaseGroupSummaryItemTemplateSelector"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Category(Categories.Appearance)]
		public DataTemplateSelector GroupSummaryItemTemplateSelector {
			get { return (DataTemplateSelector)GetValue(GroupSummaryItemTemplateSelectorProperty); }
			set { SetValue(GroupSummaryItemTemplateSelectorProperty, value); }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridViewBaseActualGroupSummaryItemTemplateSelector")]
#endif
		public DataTemplateSelector ActualGroupSummaryItemTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ActualGroupSummaryItemTemplateSelectorProperty); }
			private set { this.SetValue(ActualGroupSummaryItemTemplateSelectorPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("GridViewBaseGroupValueTemplate"),
#endif
 Category(Categories.Appearance)]
		public DataTemplate GroupValueTemplate {
			get { return (DataTemplate)GetValue(GroupValueTemplateProperty); }
			set { SetValue(GroupValueTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("GridViewBaseGroupValueTemplateSelector"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Category(Categories.Appearance)]
		public DataTemplateSelector GroupValueTemplateSelector {
			get { return (DataTemplateSelector)GetValue(GroupValueTemplateSelectorProperty); }
			set { SetValue(GroupValueTemplateSelectorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("GridViewBaseGroupRowTemplate"),
#endif
 Category(Categories.Appearance)]
		public DataTemplate GroupRowTemplate {
			get { return (DataTemplate)GetValue(GroupRowTemplateProperty); }
			set { SetValue(GroupRowTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("GridViewBaseGroupRowTemplateSelector"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Category(Categories.Appearance)]
		public DataTemplateSelector GroupRowTemplateSelector {
			get { return (DataTemplateSelector)GetValue(GroupRowTemplateSelectorProperty); }
			set { SetValue(GroupRowTemplateSelectorProperty, value); }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridViewBaseActualGroupRowTemplateSelector")]
#endif
		public DataTemplateSelector ActualGroupRowTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ActualGroupRowTemplateSelectorProperty); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("GridViewBaseIsGroupPanelMenuEnabled"),
#endif
 Category(Categories.OptionsBehavior), XtraSerializableProperty]
		public bool IsGroupPanelMenuEnabled {
			get { return (bool)GetValue(IsGroupPanelMenuEnabledProperty); }
			set { SetValue(IsGroupPanelMenuEnabledProperty, value); }
		}
		[ XtraSerializableProperty, Category(Categories.OptionsBehavior)]
		public bool IsGroupRowMenuEnabled {
			get { return (bool)GetValue(IsGroupRowMenuEnabledProperty); }
			set { SetValue(IsGroupRowMenuEnabledProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("GridViewBaseIsGroupFooterMenuEnabled"),
#endif
 Category(Categories.OptionsBehavior), XtraSerializableProperty]
		public bool IsGroupFooterMenuEnabled {
			get { return (bool)GetValue(IsGroupFooterMenuEnabledProperty); }
			set { SetValue(IsGroupFooterMenuEnabledProperty, value); }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridViewBaseImmediateUpdateRowPosition")]
#endif
		public bool ImmediateUpdateRowPosition {
			get { return (bool)GetValue(ImmediateUpdateRowPositionProperty); }
			set { SetValue(ImmediateUpdateRowPositionProperty, value); }
		}
		public Storyboard ExpandStoryboard {
			get { return (Storyboard)GetValue(ExpandStoryboardProperty); }
			set { SetValue(ExpandStoryboardProperty, value); }
		}
		public Storyboard CollapseStoryboard {
			get { return (Storyboard)GetValue(CollapseStoryboardProperty); }
			set { SetValue(CollapseStoryboardProperty, value); }
		}
#if !SL
		[Browsable(false)]
		public bool ShouldSerializeVisibleColumns(System.Windows.Markup.XamlDesignerSerializationManager manager) {
			return false;
		}
#endif
		[Category("Behavior")]
		public event EditorEventHandler ShownEditor {
			add { AddHandler(ShownEditorEvent, value); }
			remove { RemoveHandler(ShownEditorEvent, value); }
		}
		[Category("Behavior")]
		public event ShowingEditorEventHandler ShowingEditor {
			add { AddHandler(ShowingEditorEvent, value); }
			remove { RemoveHandler(ShowingEditorEvent, value); }
		}
		[Category("Behavior")]
		public event EditorEventHandler HiddenEditor {
			add { AddHandler(HiddenEditorEvent, value); }
			remove { RemoveHandler(HiddenEditorEvent, value); }
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public bool ActualShowCheckBoxSelectorInGroupRow {
			get { return (bool)GetValue(ActualShowCheckBoxSelectorInGroupRowProperty); }
			internal set { SetValue(ActualShowCheckBoxSelectorInGroupRowPropertyKey, value); }
		}
		#region Printing
		[ Category(Categories.AppearancePrint)]
		public DataTemplate PrintGroupRowTemplate {
			get { return (DataTemplate)GetValue(PrintGroupRowTemplateProperty); }
			set { SetValue(PrintGroupRowTemplateProperty, value); }
		}
		[ Category(Categories.AppearancePrint)]
		public Style PrintGroupRowStyle {
			get { return (Style)GetValue(PrintGroupRowStyleProperty); }
			set { SetValue(PrintGroupRowStyleProperty, value); }
		}
		[ Category(Categories.OptionsPrint), XtraSerializableProperty]
		public bool PrintAllGroups {
			get { return (bool)GetValue(PrintAllGroupsProperty); }
			set { SetValue(PrintAllGroupsProperty, value); }
		}
		#endregion
		#endregion
		#region Grid-Specific Stuff
		internal override bool AllowGroupingCore { get { return AllowGrouping; } }
		internal override ActualTemplateSelectorWrapper ActualGroupValueTemplateSelectorCore { get { return actualGroupValueTemplateSelector; } }
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridViewBaseGridMenu")]
#endif
		public GridPopupMenu GridMenu { get { return (GridPopupMenu)DataControlMenu; } }
		Lazy<BarManagerMenuController> groupPanelMenuControllerValue;
		Lazy<BarManagerMenuController> groupFooterMenuControllerValue;
		Lazy<BarManagerMenuController> groupRowMenuControllerValue;
		internal BarManagerMenuController GroupRowMenuController { get { return groupRowMenuControllerValue.Value; } }
		internal BarManagerMenuController GroupPanelMenuController { get { return groupPanelMenuControllerValue.Value; } }
		internal BarManagerMenuController GroupFooterMenuController { get { return groupFooterMenuControllerValue.Value; } }
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("GridViewBaseGroupPanelMenuCustomizations"),
#endif
 Browsable(false)]
		public BarManagerActionCollection GroupPanelMenuCustomizations { get { return GroupPanelMenuController.ActionContainer.Actions; } }
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("GridViewBaseGroupFooterMenuCustomizations"),
#endif
 Browsable(false)]
		public BarManagerActionCollection GroupFooterMenuCustomizations { get { return GroupFooterMenuController.ActionContainer.Actions; } }
		[ Browsable(false)]
		public BarManagerActionCollection GroupRowMenuCustomizations { get { return GroupRowMenuController.ActionContainer.Actions; } }
		internal IDesignTimeGridAdorner DesignTimeGridAdorner { get { return Grid != null ? Grid.DesignTimeGridAdorner : EmptyDesignTimeGridAdorner.Instance; } }
		internal override int GroupCount { get { return Grid.GroupCount; } }
#if DEBUGTEST
		protected internal GridDataProvider DataProvider { get { return (GridDataProvider)DataProviderBase; } }
#endif
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridViewBaseGroupedColumns")]
#endif
		public IList<GridColumn> GroupedColumns { get { return Grid.GroupedColumns; } }
		internal FrameworkElement GroupPanel { get; set; }
		new protected internal ClipboardController ClipboardController { get { return base.ClipboardController as ClipboardController; } }
		protected override RowsClipboardController CreateClipboardController() {
			return new ClipboardController(this);
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridViewBaseGrid")]
#endif
		public GridControl Grid { get { return (GridControl)DataControl; } }
		protected internal virtual GridColumnCollection Columns { get { return (GridColumnCollection)ColumnsCore; } }
		internal protected GridSortInfoCollection SortInfo { get { return Grid.SortInfo; } }
		SelectionAnchor globalSelectionAnchor = null;
		internal SelectionAnchor GlobalSelectionAnchor {
			get { return ((GridViewBase)RootView).globalSelectionAnchor; }
			private set { ((GridViewBase)RootView).globalSelectionAnchor = value; }
		}
		SelectionActionBase globalSelectionAction = null;
		internal SelectionActionBase GlobalSelectionAction {
			get { return ((GridViewBase)RootView).globalSelectionAction; }
			private set { ((GridViewBase)RootView).globalSelectionAction = value; }
		}
		public event GridRowValidationEventHandler ValidateRow;
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridViewBaseGridViewCommands")]
#endif
		public GridViewCommandsBase GridViewCommands { get { return (GridViewCommandsBase)Commands; } }
		internal GridViewBase EventTargetGridView { get { return (GridViewBase)EventTargetView; } }
		#endregion
		internal GridViewBase(MasterNodeContainer masterRootNode, MasterRowsContainer masterRootDataItem, DataControlDetailDescriptor detailDescriptor)
			: base(masterRootNode, masterRootDataItem, detailDescriptor) {
			UpdateColumnsActualGroupValueTemplateSelector();
			groupPanelMenuControllerValue = CreateMenuControllerLazyValue();
			groupFooterMenuControllerValue = CreateMenuControllerLazyValue();
			groupRowMenuControllerValue = CreateMenuControllerLazyValue();
			UpdateViewInfo();
		}
		#region view info
		void UpdateViewInfo() {
		}
		#endregion
		internal override IColumnCollection CreateEmptyColumnCollection() {
			return new GridColumnCollection(null);
		}
		protected override bool ChangeVisibleRowExpandCore(int rowHandle) {
			if(!DataProviderBase.IsGroupRow(Grid.GetRowVisibleIndexByHandle(rowHandle)))
				return false;
			ChangeGroupExpandedWithAnimation(rowHandle, Grid.IsRecursiveExpand);
			return true;
		}
		internal override bool IsDataRowNodeExpanded(DataRowNode rowNode) {
			GroupNode groupRowNode = rowNode as GroupNode;
			if(groupRowNode == null) return false;
			return groupRowNode.IsExpanded;
		}
		internal override bool IsExpandableRowFocused() {
			return IsGroupRowFocused();
		}
		internal override FrameworkElement CreateRowElement(RowData rowData) {
			return new GridRow();
		}
		internal override DependencyProperty GetFocusedColumnProperty() {
			return FocusedColumnProperty;
		}
		protected override void SetVisibleColumns(IList<ColumnBase> columns) {
			VisibleColumns = ConvertToGridColumnsList(columns);
		}
		internal override DataController GetDataControllerForUnboundColumnsCore() {
			return Grid != null ? Grid.DataController : null;
		}
		internal override bool CanCopyRows() {
			if(ActualClipboardCopyAllowed &&
			  (NavigationStyle != GridViewNavigationStyle.None) &&
			  (!FocusedView.IsInvalidFocusedRowHandle || SelectionStrategy.GetGlobalSelectedRowCount() > 0) &&
			  (ActiveEditor == null)) {
				return true;
			}
			return false;
		}
		protected internal override void SetSummariesIgnoreNullValues(bool value) {
			if(Grid == null)
				return;
			Grid.DataController.SummariesIgnoreNullValues = value;
		}
		protected internal override void OnDataReset() {
			base.OnDataReset();
			RefreshImmediateUpdateRowPositionProperty();
		}
		internal void RefreshImmediateUpdateRowPositionProperty() {
			if(Grid != null && Grid.DataController != null)
				Grid.DataController.ImmediateUpdateRowPosition = ImmediateUpdateRowPosition;
		}
		protected override void GroupColumn(string fieldName, int index, ColumnSortOrder sortOrder) {
			SortInfo.GroupByColumn(fieldName, index, sortOrder);
		}
		protected override void UngroupColumn(string fieldName) {
			SortInfo.UngroupByColumn(fieldName);
		}
		internal override void UpdateMasterDetailViewProperties() {
			base.UpdateMasterDetailViewProperties();
			UpdateIsGroupPanelVisible();
			UpdateIsGroupPanelTextVisible();
		}
		void UpdateIsGroupPanelTextVisible() {
			bool isAnyGridGrouped = false;
			UpdateAllOriginationViews(view => {
				if(view.DataControl != null)
					isAnyGridGrouped |= ((GridControl)view.DataControl).IsGrouped;
			});
			((GridViewBase)RootView).IsGroupPanelTextVisible = !isAnyGridGrouped;
		}
		void UpdateIsGroupPanelVisible() {
			bool isAnyGroupPanelVisible = false;
			UpdateAllOriginationViews(view => isAnyGroupPanelVisible |= ((GridViewBase)view).ShowGroupPanel);
			((GridViewBase)RootView).IsGroupPanelVisible = isAnyGroupPanelVisible;
		}
		#region events
		public event GridCellValidationEventHandler ValidateCell;
		protected internal override void RaiseValidateCell(GridRowValidationEventArgs e) {
			if(ValidateCell != null)
				ValidateCell(this, (GridCellValidationEventArgs)e);
		}
		internal override void RaiseHiddenEditor(int rowHandle, ColumnBase column, IBaseEdit editCore) {
			RaiseHiddenEditor(new EditorEventArgs(this, rowHandle, (GridColumn)column, editCore) { RoutedEvent = TableView.HiddenEditorEvent });
		}
		protected internal virtual void RaiseHiddenEditor(EditorEventArgs e) {
			RaiseEventInOriginationView(e);
		}
		internal override void RaiseCellValueChanging(int rowHandle, ColumnBase column, object value, object oldValue) {
			RaiseCellValueChanging(new CellValueChangedEventArgs(TableView.CellValueChangingEvent, this, rowHandle, (GridColumn)column, value, oldValue));
		}
		protected internal virtual void RaiseCellValueChanging(CellValueChangedEventArgs e) {
			RaiseEventInOriginationView(e);
		}
		internal override void RaiseCellValueChanged(int rowHandle, ColumnBase column, object newValue, object oldValue) {
			RaiseCellValueChanged(new CellValueChangedEventArgs(TableView.CellValueChangedEvent, this, rowHandle, (GridColumn)column, newValue, oldValue));
		}
		protected internal virtual void RaiseCellValueChanged(CellValueChangedEventArgs e) {
			RaiseEventInOriginationView(e);
		}
		internal override bool RaiseShowingEditor(int rowHanlde, ColumnBase columnBase) {
			ShowingEditorEventArgs e = new ShowingEditorEventArgs(this, rowHanlde, (GridColumn)columnBase);
			RaiseShowingEditor(e);
			return !e.Cancel;
		}
		protected internal virtual void RaiseShowingEditor(ShowingEditorEventArgs e) {
			RaiseEventInOriginationView(e);
		}
		internal override void RaiseShownEditor(int rowHandle, ColumnBase column, IBaseEdit editCore) {
			RaiseShownEditor(new EditorEventArgs(this, rowHandle, (GridColumn)column, editCore));
		}
		protected internal virtual void RaiseShownEditor(EditorEventArgs e) {
			RaiseEventInOriginationView(e);
		}
		internal override RowValidationError CreateCellValidationError(object errorContent, Exception exception, ErrorType errorType, int rowHandle, ColumnBase column) {
			return new GridCellValidationError(errorContent, exception, errorType, rowHandle, (GridColumn)column);
		}
		internal override GridRowValidationEventArgs CreateCellValidationEventArgs(object source, object value, int rowHandle, ColumnBase column) {
			return new GridCellValidationEventArgs(source, value, rowHandle, this, (GridColumn)column);
		}
		internal override BaseValidationError CreateCellValidationError(object errorContent, ErrorType errorType, int rowHandle, ColumnBase column) {
			return new GridCellValidationError(errorContent, null, errorType, rowHandle, (GridColumn)column);
		}
		internal override BaseValidationError CreateRowValidationError(object errorContent, ErrorType errorType, int rowHandle) {
			return new GridRowValidationError(errorContent, null, errorType, rowHandle);
		}
		#endregion
		#region Grid-Specific Stuff
		internal override string RaiseCustomDisplayText(int? rowHandle, int? listSourceIndex, ColumnBase column, object value, string displayText) {
			return Grid.RaiseCustomDisplayText(rowHandle, listSourceIndex, column, value, displayText);
		}
		internal override bool? RaiseCustomDisplayText(int? rowHandle, int? listSourceIndex, ColumnBase column, object value, string originalDisplayText, out string displayText) {
			return Grid.RaiseCustomDisplayText(rowHandle, listSourceIndex, column, value, originalDisplayText, out displayText);
		}
		internal override DataControlPopupMenu CreatePopupMenu() {
			return new GridPopupMenu(this);
		}
		protected override DataIteratorBase CreateDataIterator() {
			return new GridDataIterator(this);
		}
		protected internal virtual void SetWaitIndicator() {
			if(!IsColumnFilterOpened || IsColumnFilterLoaded)
				IsWaitIndicatorVisible = true;		  
		}
		protected internal virtual void ClearWaitIndicator() {
			IsWaitIndicatorVisible = false;
		}
		protected internal override void BeforeMoveColumnToChooser(BaseColumn column, HeaderPresenterType sourceType) {
			GridColumn gridColumn = column as GridColumn;
			if(gridColumn == null) return;
			if(sourceType == HeaderPresenterType.GroupPanel || gridColumn.IsGrouped)
				SortInfo.UngroupByColumn(gridColumn.FieldName);
		}
		protected override void NotifyDesignTimeAdornerOnColumnMoved(HeaderPresenterType moveFrom, HeaderPresenterType moveTo) {
			if(IsGroupMoveAction(moveFrom, moveTo))
				DesignTimeGridAdorner.OnColumnMovedGroup();
			else
				base.NotifyDesignTimeAdornerOnColumnMoved(moveFrom, moveTo);
		}
		protected override internal void OnDataChanged(bool rebuildVisibleColumns) {
			DataProviderBase.AutoExpandAllGroups = Grid.AutoExpandAllGroups;
			base.OnDataChanged(rebuildVisibleColumns);
		}
		internal override void OnSummaryDataChanged() {
			base.OnSummaryDataChanged();
			UpdateGroupSummary();
		}
		private static bool IsGroupMoveAction(HeaderPresenterType moveFrom, HeaderPresenterType moveTo) {
			return moveFrom == HeaderPresenterType.GroupPanel || moveTo == HeaderPresenterType.GroupPanel;
		}
		internal bool IsGroupRowFocused() {
			return Grid.IsGroupRowHandle(FocusedRowHandle);
		}
		protected internal override void UpdateGroupSummary() {
			UpdateRowData((rowData) => rowData.UpdateGroupSummaryData());
		}
		void UpdateActualGroupSummaryItemTemplateSelector() {
			ActualGroupSummaryItemTemplateSelector = new ActualTemplateSelectorWrapper(GroupSummaryItemTemplateSelector, GroupSummaryItemTemplate);
			UpdateGroupSummaryTemplates();
		}
		protected void UpdateGroupSummaryTemplates() {
			UpdateRowData(rowData => rowData.UpdateClientSummary());
		}
		ActualTemplateSelectorWrapper actualGroupValueTemplateSelector;
		void UpdateColumnsActualGroupValueTemplateSelector() {
			actualGroupValueTemplateSelector = new ActualTemplateSelectorWrapper(GroupValueTemplateSelector, GroupValueTemplate);
			if(Grid == null)
				return;
			foreach(GridColumn column in Grid.Columns) {
				column.UpdateActualGroupValueTemplateSelector();
			}
		}
		void UpdateActualGroupRowTemplateSelector() {
			UpdateActualTemplateSelector(ActualGroupRowTemplateSelectorPropertyKey, GroupRowTemplateSelector, GroupRowTemplate);
		}
		protected internal virtual GroupRowData CreateGroupRowDataCore(DataTreeBuilder treeBuilder) {
			return new GroupRowData(treeBuilder);
		}
		internal override bool IsColumnVisibleInHeaders(BaseColumn col) {
			GridColumn gridCol = (GridColumn)col;
			return !gridCol.IsGrouped || gridCol.ShowGroupedColumn.GetValue(ShowGroupedColumns) || AllowPartialGroupingCore;
		}
		protected override AutomationPeer GetAutomationPeer(DependencyObject obj) {
			if(!CanGetAutomationPeer(obj)) return null;
			AutomationPeer peer = base.GetAutomationPeer(obj);
			if(peer == null && (obj is GroupGridRowPresenter || obj is GroupRowControl)) {
				return Grid.AutomationPeer.GetRowPeer(FocusedRowHandle);
			}
			return peer;
		}
		internal bool IsGroupRow(int visibleIndex, int level) {
			return DataProviderBase.IsGroupRow(GetRowParentIndex(visibleIndex, level));
		}
		public virtual void DeleteRow(int rowHandle) {
			DeleteRowCore(rowHandle);
		}
		internal override bool IsInvisibleGroupRow(RowNode node) {
			return (node is GroupNode && !((GroupNode)node).IsRowVisible);
		}
		internal override bool CanStartDragSingleColumn() {
			return ShowGroupedColumns;
		}
		protected internal abstract FrameworkElement CreateGroupControl(GroupRowData rowData);
		internal override void SetFocusedRectangleOnGroupRow() {
			SetFocusedRectangleOnRowData(GetGroupRowFocusedRectangleTemplate());
		}
		protected override void HandleGroupMoveAction(ColumnBase source, int newVisibleIndex, HeaderPresenterType moveFrom, HeaderPresenterType moveTo) {
			if(IsGroupMoveAction(moveFrom, moveTo)) {
				SortInfo.OnGroupColumnMove(source.FieldName, newVisibleIndex, moveFrom == HeaderPresenterType.GroupPanel, moveTo == HeaderPresenterType.GroupPanel);
			}
		}
		#endregion
		#region events
		protected internal override bool RaiseCopyingToClipboard(CopyingToClipboardEventArgsBase e) {
			e.RoutedEvent = GridViewBase.CopyingToClipboardEvent;
			RaiseEventInOriginationView(e);
			return e.Handled;
		}
		protected internal override void RaiseSelectionChanged(GridSelectionChangedEventArgs e) {
			e.RoutedEvent = GridViewBase.SelectionChangedEvent;
			RaiseEventInOriginationView(e);
		}
		protected virtual void RaiseInvalidRowException(InvalidRowExceptionEventArgs e) {
			RaiseEventInOriginationView(e);
		}
		protected internal virtual void RaiseValidateRow(GridRowValidationEventArgs e) {
			EventTargetGridView.RaiseValidateRowCore(e);
		}
		void RaiseValidateRowCore(GridRowValidationEventArgs e) {
			if(ValidateRow != null)
				ValidateRow(this, e);
		}
		protected internal virtual void RaiseRowUpdated(RowEventArgs e) {
			RaiseEventInOriginationView(e);
		}
		protected internal virtual void RaiseRowCanceled(RowEventArgs e) {
			RaiseEventInOriginationView(e);
		}
		#endregion
		#region GroupDisplayText
		protected internal override object GetGroupDisplayValue(int rowHandle) {
			object originalValue = Grid.GetGroupRowValue(rowHandle);
			int rowLevel = Grid.GetRowLevelByRowHandle(rowHandle);
			string fieldName = GetSortInfoBySortLevel(rowLevel).FieldName;
			string s;
			DevExpress.Xpf.Grid.Helpers.GridDataColumnSortInfo info = Grid.SortData.GetSortInfo(DataProviderBase.Columns[fieldName]);
			object val = originalValue;
			if(info != null)
				val = info.UpdateGroupDisplayValue(originalValue);
			s = GetGroupDisplayText(rowHandle, val, Columns[fieldName], info == null ? null : info.GetColumnGroupFormatString());
			if(info != null)
				s = info.GetGroupDisplayText(val, s);
			return Grid.RaiseCustomDisplayText(rowHandle, null, Grid.Columns[fieldName], originalValue, s);
		}
		protected internal override string GetGroupRowDisplayText(int rowHandle) {
			GridColumn gridColumn = (GridColumn)GetColumnBySortLevel(Grid.GetRowLevelByRowHandle(rowHandle));
			if(gridColumn != null) {
				return String.Format(GetLocalizedString(GridControlStringId.GridGroupRowDisplayTextFormat), gridColumn.HeaderCaption, (string)GetGroupDisplayValue(rowHandle));
			}
			return string.Empty;
		}
		string GetGroupDisplayText(int rowHandle, object value, GridColumn column, string formatString) {
			if(column == null) return string.Empty;
			object result = value;
			if(string.IsNullOrEmpty(formatString))
				result = GetDisplayObject(result, column);
			else {
				if(column.GetSortMode() != DevExpress.XtraGrid.ColumnSortMode.Value)
					result = GetDisplayObject(result, column, false);
				result = FormatStringConverter.GetFormattedValue(formatString, result, CultureInfo.CurrentCulture);
			}
			return Grid.RaiseCustomGroupDisplayText(rowHandle, column, value, result.ToString());
		}
		#endregion
		#region validation
		protected internal virtual void OnPostRowException(ControllerRowExceptionEventArgs e) {
			InvalidRowExceptionEventArgs eventArgs = new InvalidRowExceptionEventArgs(this, e.RowHandle, e.Exception.Message, GetLocalizedString(GridControlStringId.ErrorWindowTitle), e.Exception, ExceptionMode.DisplayError);
			RaiseInvalidRowException(eventArgs);
			HandleInvalidRowExceptionEventArgs(e, eventArgs);
		}
		#endregion
		#region commands
		public void ShowGroupSummaryEditor() {
			GridGroupSummaryHelper helper = new GridGroupSummaryHelper(this);
			helper.ShowSummaryEditor();
		}
		internal bool CanShowGroupSummaryEditor() {
			return true;
		}
		internal bool IsGroupRow(DependencyObject obj) {
			RowHandle rowHandle = GetRowHandle(obj);
			if(rowHandle == null) return false;
			return Grid.IsGroupRowHandle(rowHandle.Value);
		}
		public virtual void MoveParentGroupRow() {
			MoveParentRow();
		}
		public bool CollapseFocusedRow() {
			return CollapseFocusedRowCore();
		}
		public bool ExpandFocusedRow() {
			return ExpandFocusedRowCore();
		}
		internal bool CanMoveGroupParentRow() {
			return Grid != null && HasParentRow(DataProviderBase.CurrentIndex);
		}
		internal bool GetIsGrouped() {
			if(!IsRootView && !HasClonedExpandedDetails) 
				return false;
			return GetIsGroupedCore();
		}
		internal bool GetIsGroupedCore() {
			return Grid != null && Grid.IsGrouped;
		}
		internal void ChangeGroupExpanded(object commandParameter) {
				OnChangeGroupExpanded(commandParameter);
		}
		void OnChangeGroupExpanded(object commandParameter) {
			if(commandParameter is int) {
				ChangeGroupExpandedWithAnimation((int)commandParameter, ((GridControl)DataControl).IsRecursiveExpand);
			}
		}
		internal void ChangeGroupExpandedWithAnimation(int rowHandle, bool recursive) {
			if(RootDataPresenter == null)
				return;
			if(CommitEditing() && DataControl.IsValidRowHandleCore(rowHandle) && Nodes.ContainsKey(rowHandle) && GetRowElementByRowHandle(rowHandle) != null) {
				GroupNode groupNode = (GroupNode)Nodes[rowHandle];
				if(UseAnimationWhenExpanding && !ActualAllowCellMerge) {
					RootDataPresenter.EnqueueContinousAction(new ExpandRowWithAnimationAction(RootDataPresenter, groupNode, recursive));
				} else {
					if(groupNode.IsExpanded)
						Grid.CollapseGroupRowWithEvents(groupNode.RowHandle.Value, recursive);
					else
						Grid.ExpandGroupRowWithEvents(groupNode.RowHandle.Value, recursive);
				}
				RootDataPresenter.InvalidateMeasure();
			}
		}
		internal void ExpandAllGroups(object commandParameter) {
			Grid.ExpandAllGroups();
		}
		internal void CollapseAllGroups(object commandParameter) {
			Grid.CollapseAllGroups();
		}
		internal void ClearGrouping() {
			Grid.ClearGrouping();
		}
		internal bool CanExpandCollapseAll(object commandParameter) {
			return GetIsGrouped();
		}
		internal bool CanClearGrouping() {
			return AllowGrouping && GetIsGroupedCore();
		}
		#endregion
		#region MultiSelection
		[Obsolete("Use the DataControlBase.BeginSelection method instead"), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public void BeginSelection() {
			BeginSelectionCore();
		}
		[Obsolete("Use the DataControlBase.EndSelection method instead"), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public void EndSelection() {
			EndSelectionCore();
		}
		[Obsolete("Use the DataControlBase.SelectItem method instead"), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public void SelectRow(int rowHandle) {
			SelectRowCore(rowHandle);
		}
		[Obsolete("Use the DataControlBase.UnselectItem method instead"), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public void UnselectRow(int rowHandle) {
			UnselectRowCore(rowHandle);
		}
		[Obsolete("Use the DataControlBase.UnselectAll method instead"), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public void ClearSelection() {
			ClearSelectionCore();
		}
		[Obsolete("Use the DataControlBase.SelectRange method instead"), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public void SelectRange(int startRowHandle, int endRowHandle) {
			SelectRangeCore(startRowHandle, endRowHandle);
		}
		protected internal override GridSelectionChangedEventArgs CreateSelectionChangedEventArgs(DevExpress.Data.SelectionChangedEventArgs e) {
			return new GridSelectionChangedEventArgs(this, e.Action, e.ControllerRow);
		}
		#endregion
		protected internal override void ResetHeadersChildrenCache() {
			if(Grid.AutomationPeer != null) Grid.AutomationPeer.ResetHeadersChildrenCache();
		}
		internal virtual void PerformUpdateGroupSummaryDataAction(Action action) { }
		protected override void UpdateGridControlColumnProvider() {
			if(DataControl != null) {
				GridControlColumnProvider columnProvider = new GridControlColumnProvider() { FilterByColumnsMode = FilterByColumnsMode.Default, AllowColumnsHighlighting = false };
				GridControlColumnProvider.SetColumnProvider(DataControl, columnProvider);
			}
		}
		protected internal virtual string GetGroupSummaryText(ColumnBase column, int rowHandle, bool groupFooter) {
			StringBuilder textBuilder = new StringBuilder();
			foreach(GridSummaryItem summaryItem in column.GroupSummariesCore) {
				object summaryValue = null;
				if(!DataProviderBase.TryGetGroupSummaryValue(rowHandle, summaryItem, out summaryValue)) continue;
				if(groupFooter && summaryItem.ShowInGroupColumnFooter != column.FieldName || !groupFooter && summaryItem.ShowInGroupColumnFooter == column.FieldName) continue;
				if(textBuilder.Length > 0) {
					if(groupFooter)
						textBuilder.Append(Environment.NewLine);
					else
						textBuilder.Append(GridControlLocalizer.GetString(GridControlStringId.SummaryItemsSeparator));
				}
				ColumnBase summaryColumn = DataControl.ColumnsCore[summaryItem.FieldName];
				string text = summaryItem.GetGroupColumnDisplayText(System.Globalization.CultureInfo.CurrentCulture, GridColumn.GetSummaryDisplayName(summaryColumn, summaryItem), summaryValue, GroupRowData.GetColumnDisplayFormat(column));
				textBuilder.Append(text);
			}
			return textBuilder.ToString();
		}
		internal bool? AreAllItemsSelected(int groupRowHandle) {
			return SelectionStrategy.GetAllItemsSelected(groupRowHandle);
		}
		internal void SelectRowRecursively(int groupRowHandle) {
			SelectionStrategy.SelectRowRecursively(groupRowHandle);
		}
		internal void UnselectRowRecursively(int groupRowHandle) {
			SelectionStrategy.UnselectRowRecursively(groupRowHandle);
		}
		internal virtual bool IsExpandButton(IDataViewHitInfo hitInfo) { return false; }
		internal void SetSelectionAnchor() {
			if(RootView.FocusedView != this)
				return;
			int selectionAnchorRowHandle = ViewBehavior.GetValueForSelectionAnchorRowHandle(FocusedRowHandle);
			GlobalSelectionAnchor = new SelectionAnchor(this, selectionAnchorRowHandle);
		}
		internal void SetSelectionAction(SelectionActionBase action) {
			GlobalSelectionAction = action;
		}
		internal void ExecuteSelectionAction() {
			if(RootView.FocusedView != this)
				return;
			if(GlobalSelectionAction != null)
				GlobalSelectionAction.Execute();
			GlobalSelectionAction = null;
		}
		void OnActualGroupRowTemplateSelectorChanged() {
			UpdateRowData(rowData => rowData.UpdateClientGroupRowTemplateSelector());
		}
		void OnGroupRowStyleChanged() {
			ViewBehavior.UpdateRowData(rowData => rowData.UpdateClientGroupRowStyle());
		}
		void OnActualShowCheckBoxSelectorInGroupRowChanged() {
			ViewBehavior.UpdateRowData(rowData => rowData.UpdateClientCheckBoxSelector());
		}
		void OnGroupValueContentStyleChanged() {
			ValidateGroupContentStyle(GroupValueContentStyle, GridTableViewBehaviorBase.GroupValueContentStyleInOptimizedModeError);
		}
		void OnGroupSummaryContentStyleChanged() {
			ValidateGroupContentStyle(GroupSummaryContentStyle, GridTableViewBehaviorBase.GroupSummaryContentStyleInOptimizedModeError);
		}
		void ValidateGroupContentStyle(Style style, string errorMessage) {
			if(style != null && !(style is DefaultStyle) && IsGroupRowOptimized && !DisableOptimizedModeVerification && !IsDesignTime)
				throw new InvalidOperationException(errorMessage);
		}
		protected virtual bool IsGroupRowOptimized { get { return false; } }
		#region Printing
		protected override bool GetCanCreateRootNodeAsync() {
			return DataProviderBase.IsAsyncServerMode;
		}
		EventHandler<ScalarOperationCompletedEventArgs<IRootDataNode>> createRootNodeCompletedHandler;
		protected override void AddCreateRootNodeCompletedEvent(EventHandler<ScalarOperationCompletedEventArgs<IRootDataNode>> eventHandler) {
			createRootNodeCompletedHandler += eventHandler;
		}
		protected override void RemoveCreateRootNodeCompletedEvent(EventHandler<ScalarOperationCompletedEventArgs<IRootDataNode>> eventHandler) {
			createRootNodeCompletedHandler -= eventHandler;
		}
		protected override IVisualTreeWalker GetCustomVisualTreeWalker() {
			return null;
		}
		internal ItemsGenerationStrategyBase CreateItemsGenerationStrategy() {
			if(DataProviderBase.IsServerMode) {
				return new ItemsGenerationServerModeStrategy(this);
			}
			if(DataProviderBase.IsAsyncServerMode) {
				return new ItemsGenerationAsyncServerModeStrategy(this);
			}
			if(PrintSelectedRowsOnly) {
				return new ItemsGenerationSelectedRowsStrategy(this);
			}
			if(PrintAllGroups)
				return new ItemsGenerationPrintAllGroupsStrategy(this);
			return new ItemsGenerationSimpleStrategy(this);
		}
		protected internal virtual void RaiseCreateRootNodeCompleted(IRootDataNode rootNode) {
			if(createRootNodeCompletedHandler != null) {
				createRootNodeCompletedHandler(this, new ScalarOperationCompletedEventArgs<IRootDataNode>(rootNode, null, false, null));
			}
		}
		#endregion
	}
	public class GridViewSelectionControlWrapper : SelectionControlWrapper {
		public static void Register() {
			SelectionControlWrapper.Wrappers.Add(typeof(GridViewBase), typeof(GridViewSelectionControlWrapper));
		}
		GridViewBase View { get; set; }
		public GridViewSelectionControlWrapper(GridViewBase view) {
			View = view;
		}
		public override void SubscribeSelectionChanged(Action<IList, IList> a) {
			View.DataControl.SelectedItems = View.GetValue(SelectionAttachedBehavior.SelectedItemsSourceProperty) as IList;
		}
		public override void UnsubscribeSelectionChanged() {
			View.DataControl.SelectedItems = null;
		}
		public override IList GetSelectedItems() { return null; }
		public override void ClearSelection() { }
		public override void SelectItem(object item) { }
		public override void UnselectItem(object item) { }
	}
}
