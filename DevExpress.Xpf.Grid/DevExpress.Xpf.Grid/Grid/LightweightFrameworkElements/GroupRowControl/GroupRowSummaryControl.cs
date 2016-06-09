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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Grid.Native;
namespace DevExpress.Xpf.Grid {
	public class GroupRowDefaultSummaryControl : CachedItemsControl, ISupportLoadingAnimation {
		static GroupRowDefaultSummaryControl() {
			Type ownerType = typeof(GroupRowDefaultSummaryControl);
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
		}
		DataTemplateSelector itemTemplateSelectorCore;
		internal DataTemplateSelector ItemTemplateSelector {
			get { return itemTemplateSelectorCore; }
			set {
				if(itemTemplateSelectorCore != value) {
					itemTemplateSelectorCore = value;
					OnItemTemplateChanged();
				}
			}
		}
		bool UseDefaultItemTemplate { get { return itemTemplateSelectorCore == null; } }
		RowData RowData { get { return DataContext as RowData; } }
		protected override FrameworkElement CreateChild(object item) {
			if(UseDefaultItemTemplate)
				return new GroupRowDefaultSummaryItemControl();
			return new GroupSummaryContentPresenter { VerticalAlignment = System.Windows.VerticalAlignment.Center, ContentTemplateSelector = ItemTemplateSelector };
		}
		protected override void ValidateElement(FrameworkElement element, object item) {
			base.ValidateElement(element, item);
			((IDefaultGroupSummaryItem)element).ValueData = (GridGroupSummaryData)item;
		}
		LoadingAnimationHelper loadingAnimationHelper;
		LoadingAnimationHelper LoadingAnimationHelper {
			get {
				if(loadingAnimationHelper == null)
					loadingAnimationHelper = new LoadingAnimationHelper(this);
				return loadingAnimationHelper;
			}
		}
		internal void UpdateIsReady() {
			if(RowData != null)
				LoadingAnimationHelper.ApplyAnimation();
		}
		#region ISupportLoadingAnimation Members
		DataViewBase ISupportLoadingAnimation.DataView { get { return RowData.View; } }
		FrameworkElement ISupportLoadingAnimation.Element { get { return this; } }
		bool ISupportLoadingAnimation.IsGroupRow { get { return true; } }
		bool ISupportLoadingAnimation.IsReady { get { return RowData.IsReady; } }
		#endregion
	}
	class GroupRowDefaultSummaryItemController : GroupValuePresenterControllerBase<GridGroupSummaryData> {
		readonly GroupRowDefaultSummaryItemControl summaryItem;
		public GroupRowDefaultSummaryItemController(GroupRowDefaultSummaryItemControl summaryItem) {
			this.summaryItem = summaryItem;
		}
		protected override void UpdateText() {
			summaryItem.Text = CalcValueDataText();
		}
		string CalcValueDataText() {
			if(ValueData == null)
				return null;
			string text = ValueData.Text;
			if(ValueData.IsLast)
				return text;
			else
				return text + ", ";
		}
		protected override void SetClient(IGroupValueClient client) {
			if(ValueData != null)
				ValueData.SetGroupValueClient(client);
		}
	}
	public interface IDefaultGroupSummaryItem {
		GridGroupSummaryData ValueData { get; set; }
	}
	public class GroupRowDefaultSummaryItemControl : Control, IDefaultGroupSummaryItem {
		public string Text {
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(GroupRowDefaultSummaryItemControl), new PropertyMetadata(null));
		GroupRowDefaultSummaryItemController controller;
		public GroupRowDefaultSummaryItemControl() {
			controller = new GroupRowDefaultSummaryItemController(this);
		}
		static GroupRowDefaultSummaryItemControl() {
			Type ownerType = typeof(GroupRowDefaultSummaryItemControl);
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
			GridViewHitInfoBase.HitTestAcceptorProperty.OverrideMetadata(ownerType, new PropertyMetadata(new DevExpress.Xpf.Grid.HitTest.GroupSummaryTableViewHitTestAcceptor()));
		}
		#region IDefaultGroupSummaryItem Members
		GridGroupSummaryData IDefaultGroupSummaryItem.ValueData {
			get { return controller.ValueData; }
			set { controller.ValueData = value; }
		}
		#endregion
	}
	public class GroupRowAlignByColumnsSummaryControl : CachedItemsControl, ILayoutNotificationHelperOwner {
		readonly FixedStyle fixedStyle;
		bool actualUseDefaultItemTemplate = true;
		internal bool ActualUseDefaultItemTemplate { get { return actualUseDefaultItemTemplate; } }
		bool useDefaultItemTemplateCore = true;
		internal bool UseDefaultItemTemplate {
			get { return useDefaultItemTemplateCore; }
			set {
				if(useDefaultItemTemplateCore != value) {
					useDefaultItemTemplateCore = value;
					OnItemTemplateChanged();
				}
			}
		}
		BandsGroupSummaryAlignByColumnsPanel BandsPanel { get { return Panel as BandsGroupSummaryAlignByColumnsPanel; } }
		IList<BandBase> bandsCore;
		internal IList<BandBase> Bands {
			get { return bandsCore; }
			set {
				if(bandsCore != value) {
					bandsCore = value;
					OnBandsChanged();
				}
			}
		}
		double leftIndentCore;
		internal double LeftIndent {
			get { return leftIndentCore; }
			set {
				if(leftIndentCore != value) {
					leftIndentCore = value;
					OnLeftIndentChanged();
				}
			}
		}
		readonly LayoutNotificationHelper layoutNotificationHelper;
		protected override Size MeasureOverride(Size constraint) {
			layoutNotificationHelper.Subscribe();
			return base.MeasureOverride(constraint);
		}
		static readonly ControlTemplate OrdinarPanelTemplate;
		static readonly ControlTemplate BandsPanelTemplate;
		static readonly DataTemplate BandsItemTemplate;
		static readonly DataTemplate OrdinarItemTemplate;
		static GroupRowAlignByColumnsSummaryControl() {
			var ordinarPanelFactory = new FrameworkElementFactory(typeof(StackVisibleIndexPanel));
			ordinarPanelFactory.SetValue(OrderPanelBase.ArrangeAccordingToVisibleIndexProperty, true);
			ordinarPanelFactory.SetValue(OrderPanelBase.OrientationProperty, Orientation.Horizontal);
			OrdinarPanelTemplate = CreateTemplate(ordinarPanelFactory, () => new ControlTemplate(typeof(ItemsControlBase)));
			BandsPanelTemplate = CreateTemplate(new FrameworkElementFactory(typeof(BandsGroupSummaryAlignByColumnsPanel)), () => new ControlTemplate(typeof(ItemsControlBase)));
			var bandItemFactory = new FrameworkElementFactory(typeof(GroupBandSummaryControl));
			SetCustomItemTemplateBindings(bandItemFactory);
			bandItemFactory.SetBinding(GroupBandSummaryControl.HasTopElementProperty, new Binding("Column.HasTopElement"));
			bandItemFactory.SetBinding(StyleProperty, new Binding("View.GroupBandSummaryContentStyle"));
			BandsItemTemplate = CreateTemplate(bandItemFactory, () => new DataTemplate());
			var itemTemplateFactory = new FrameworkElementFactory(typeof(GroupColumnSummaryControl));
			SetCustomItemTemplateBindings(itemTemplateFactory);
			itemTemplateFactory.SetBinding(StyleProperty, new Binding("View.GroupColumnSummaryContentStyle"));
			OrdinarItemTemplate = CreateTemplate(itemTemplateFactory, () => new DefaultDataTemplate());
			ItemsPanelProperty.OverrideMetadata(typeof(GroupRowAlignByColumnsSummaryControl), new PropertyMetadata(OrdinarPanelTemplate));
			ItemTemplateProperty.OverrideMetadata(typeof(GroupRowAlignByColumnsSummaryControl), new PropertyMetadata(OrdinarItemTemplate));
		}
		static void SetCustomItemTemplateBindings(FrameworkElementFactory factory) {
			factory.SetBinding(GroupColumnSummaryControl.IsReadyProperty, new Binding("GroupRowData.IsReady"));
			factory.SetBinding(GroupColumnSummaryControl.IsGroupRowFocusedProperty, new Binding("GroupRowData.IsFocused"));
		}
		static T CreateTemplate<T>(FrameworkElementFactory factory, Func<T> creator) where T : FrameworkTemplate {
			T template = creator();
			template.VisualTree = factory;
			template.Seal();
			return template;
		}
		public GroupRowAlignByColumnsSummaryControl(FixedStyle fixedStyle) {
			this.fixedStyle = fixedStyle;
			layoutNotificationHelper = new LayoutNotificationHelper(this);
		}
		protected override FrameworkElement CreateChild(object item) {
			if(actualUseDefaultItemTemplate)
				return new GroupRowColumnSummaryControl();
			return base.CreateChild(item);
		}
		protected override void ValidateElement(FrameworkElement element, object item) {
			base.ValidateElement(element, item);
			if(actualUseDefaultItemTemplate) {
				var summaryControl = (GroupRowColumnSummaryControl)element;
				summaryControl.ColumnData = (GridGroupSummaryColumnData)item;
				summaryControl.SyncWithColumnData();
			}
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UpdateBandsPanel();
		}
		internal void UpdatePanel(bool hasBands) {
			if(hasBands) {
				ItemTemplate = BandsItemTemplate;
				Template = BandsPanelTemplate;
			} else {
				ItemTemplate = OrdinarItemTemplate;
				Template = OrdinarPanelTemplate;
			}
		}
		void UpdateBandsPanel() {
			if(BandsPanel != null) {
				BandsPanel.Fixed = fixedStyle;
				OnBandsChanged();
				OnLeftIndentChanged();
			}
		}
		void OnBandsChanged() {
			if(BandsPanel != null)
				BandsPanel.Bands = Bands;
		}
		void OnLeftIndentChanged() {
			if(BandsPanel != null)
				BandsPanel.LeftMargin = -LeftIndent;
		}
		protected override void OnItemTemplateChanged() {
			actualUseDefaultItemTemplate = UseDefaultItemTemplate && (ItemTemplate == null || ItemTemplate is DefaultDataTemplate);
			base.OnItemTemplateChanged();
		}
		#region ILayoutNotificationHelperOwner Members
		#region ILayoutNotificationHelperOwner Members
		DependencyObject ILayoutNotificationHelperOwner.NotificationManager {
			get {
				GroupRowData rowData = DataContext as GroupRowData;
				if(rowData != null) {
					DataViewBase view = rowData.View;
					if(view != null)
						return view.DataControl;
				}
				return null;
			}
		}
		#endregion
		#endregion
	}
	public interface IGroupRowColumnSummaryClient {
		void UpdateFocusState();
		void UpdateIsReady();
		void UpdateHasSummary();
		void UpdateSummaryValue();
	}
	public class GroupRowColumnSummaryControl : Control, IGroupRowColumnSummaryClient, ISupportLoadingAnimation {
		public static readonly DependencyProperty BorderBrushesProperty = DependencyProperty.Register("BorderBrushes", typeof(BrushSet), typeof(GroupRowColumnSummaryControl), new PropertyMetadata(null, (d, e) => ((GroupRowColumnSummaryControl)d).UpdateBorderBrush()));
		public BrushSet BorderBrushes {
			get { return (BrushSet)GetValue(BorderBrushesProperty); }
			set { SetValue(BorderBrushesProperty, value); }
		}
		TextBlock textBlock;
		Border border;
		static readonly Thickness normalThickness = new Thickness(0, 0, 1, 0);
		static readonly Thickness rightThickness = new Thickness(0);
		GridGroupSummaryColumnData columnDataCore;
		public GridGroupSummaryColumnData ColumnData {
			get {
				return columnDataCore;
			}
			internal set {
				if(columnDataCore != value) {
					columnDataCore = value;
					columnDataCore.SetColumnSummaryClient(this);
					SyncWithColumnData();
				}
			}
		}
		DataViewBase View { get { return ColumnData != null ? ColumnData.View : null; } }
		internal void SyncWithColumnData() {
			if(ColumnData == null)
				return;
			UpdateTextBlock();
			UpdateBorderThickness();
			UpdateBorderWidth();
			UpdateBorderBrush();
		}
		void UpdateBorderThickness() {
			if(border != null)
				border.BorderThickness = ColumnData.HasRightSibling ? normalThickness : rightThickness;
		}
		internal void UpdateBorderWidth() {
			if(border != null)
				border.Width = GroupSummaryLayoutCalculator.CalcWidth(ColumnData.Column, ColumnData.Column.ActualHeaderWidth, ColumnData.GroupRowData);
		}
		void UpdateBorderBrush() {
			if(border != null && BorderBrushes != null && View != null)
				border.BorderBrush = BorderBrushes.GetBrush((ColumnData.GroupRowData.IsFocused && View.IsKeyboardFocusWithin && View.FadeSelectionOnLostFocus) ? "Focused" : "Normal");
		}
		void UpdateTextBlock() {
			if(ColumnData.HasSummary) {
				if(textBlock == null && border != null) {
					textBlock = new TextBlock { TextAlignment = TextAlignment.Right, TextTrimming = TextTrimming.CharacterEllipsis };
					TextBlockService.SetAllowIsTextTrimmed(textBlock, true);
					TextBlockService.AddIsTextTrimmedChangedHandler(textBlock, OnIsTextTrimmedChanged);
					border.Child = textBlock;
				}
			} else {
				textBlock = null;
				if(border != null)
					border.Child = null;
			}
			UpdateTextBlockValue();
		}
		void UpdateTextBlockValue() {
			if(textBlock != null) {
				object value = ColumnData.Value;
				textBlock.Text = value != null ? value.ToString() : null;
			}
		}
		void OnIsTextTrimmedChanged(object o, RoutedEventArgs e) {
			textBlock.ToolTip = TextBlockService.GetIsTextTrimmed(textBlock) ? ColumnData.Value : DependencyProperty.UnsetValue;
		}
		static GroupRowColumnSummaryControl() {
			Type ownerType = typeof(GroupRowColumnSummaryControl);
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			border = (Border)GetTemplateChild("PART_Border");
			SyncWithColumnData();
		}
		LoadingAnimationHelper loadingAnimationHelper;
		LoadingAnimationHelper LoadingAnimationHelper {
			get {
				if(loadingAnimationHelper == null)
					loadingAnimationHelper = new LoadingAnimationHelper(this);
				return loadingAnimationHelper;
			}
		}
		#region IGroupRowColumnSummaryClient Members
		void IGroupRowColumnSummaryClient.UpdateFocusState() {
			UpdateBorderBrush();
		}
		void IGroupRowColumnSummaryClient.UpdateIsReady() {
			if(ColumnData != null)
				LoadingAnimationHelper.ApplyAnimation();
		}
		void IGroupRowColumnSummaryClient.UpdateHasSummary() {
			UpdateTextBlock();
		}
		void IGroupRowColumnSummaryClient.UpdateSummaryValue() {
			UpdateTextBlockValue();
		}
		#endregion
		#region ISupportLoadingAnimation Members
		DataViewBase ISupportLoadingAnimation.DataView { get { return ColumnData.View; } }
		FrameworkElement ISupportLoadingAnimation.Element { get { return textBlock; } }
		bool ISupportLoadingAnimation.IsGroupRow { get { return true; } }
		bool ISupportLoadingAnimation.IsReady { get { return ColumnData.GroupRowData.IsReady; } }
		#endregion
	}
	class SummaryAlignByColumnsController {
		#region inner classes
		abstract class GroupSummaryControlUpdaterBase {
			readonly Panel layoutPanel;
			readonly DataControlBase ownerControl;
			protected GroupRowAlignByColumnsSummaryControl SummaryControl { get; set; }
			protected ITableView View { get { return ownerControl.viewCore as ITableView; } }
			BandsLayoutBase BandsLayout { get { return ownerControl.BandsLayoutCore; } }
#if DEBUGTEST
			public GroupRowAlignByColumnsSummaryControl SummaryControlForTests { get { return SummaryControl; } }
#endif
			protected GroupSummaryControlUpdaterBase(Panel layoutPanel, DataControlBase ownerControl) {
				this.layoutPanel = layoutPanel;
				this.ownerControl = ownerControl;
			}
			public void UpdateData(GroupRowData data) {
				IList<GridGroupSummaryColumnData> columnData = GetFixedColumnData(data);
				if(columnData != null && columnData.Count > 0 && HasElements(ownerControl.viewCore as TableView)) {
					if(SummaryControl == null) {
						CreateElements();
						UpdateBands();
						UpdatePanel();
					}
					if (SummaryControl != null) SummaryControl.ItemsSource = columnData;
				}
				else
					Clear();
			}
			protected virtual void Clear() {
				Remove(SummaryControl);
				SummaryControl = null;
			}
			public void UpdateBands() {
				if(SummaryControl == null)
					return;
				IList<BandBase> bands = null;
				if(BandsLayout != null)
					bands = GetBands(BandsLayout);
				SummaryControl.Bands = bands;
			}
			public void UpdatePanel() {
				if(SummaryControl == null)
					return;
				SummaryControl.UpdatePanel(BandsLayout != null);
			}
			public void SetCanUseDefaultTemplate(bool canUse) {
				if(SummaryControl == null)
					return;
				SummaryControl.UseDefaultItemTemplate = canUse;
			}
			public void SetLeftIndent(double leftIndent) {
				if(SummaryControl == null)
					return;
				SummaryControl.LeftIndent = leftIndent;
			}
			protected void Add(FrameworkElement element) {
				if(element != null)
					layoutPanel.Children.Add(element);
			}
			protected void Remove(FrameworkElement element) {
				if(element != null)
					layoutPanel.Children.Remove(element);
			}
			protected abstract void CreateElements();
			protected abstract IList<BandBase> GetBands(BandsLayoutBase bandsLayout);
			protected GroupRowAlignByColumnsSummaryControl CreateSummaryItemsControl(int column, FixedStyle fixedStyle) {
				var itemsControl = new GroupRowAlignByColumnsSummaryControl(fixedStyle);
				Add(itemsControl);
				System.Windows.Controls.Grid.SetColumn(itemsControl, column);
				return itemsControl;
			}
			protected abstract IList<GridGroupSummaryColumnData> GetFixedColumnData(GroupRowData data);
			protected virtual bool HasElements(TableView view) {
				return false;
			}
		}
		class FixedNoneSummaryControlUpdater : GroupSummaryControlUpdaterBase {
			public FixedNoneSummaryControlUpdater(Panel layoutPanel, DataControlBase ownerControl) : base(layoutPanel, ownerControl) { }
			public void SetScrollingMargin(Thickness scrollingMargin) {
				if(SummaryControl != null)
					SummaryControl.Panel.Margin = scrollingMargin;
			}
			protected override void CreateElements() {
				GroupRowAlignByColumnsSummaryControl itemsControl = CreateSummaryItemsControl(2, FixedStyle.None);
				itemsControl.ClipToBounds = true;
				SummaryControl = itemsControl;
			}
			protected override IList<BandBase> GetBands(BandsLayoutBase bandsLayout) {
				return bandsLayout.FixedNoneVisibleBands;
			}
			protected override IList<GridGroupSummaryColumnData> GetFixedColumnData(GroupRowData data) {
				return data.FixedNoneGroupSummaryData;
			}
			protected override bool HasElements(TableView view) {
				return true;
			}
		}
		abstract class FixedSummaryControlUpdaterBase : GroupSummaryControlUpdaterBase {
			protected FixedSummaryControlUpdaterBase(Panel layoutPanel, DataControlBase ownerControl) : base(layoutPanel, ownerControl) { }
			internal protected GroupRowFixedLineSeparatorControl FixedLine { get; set; }
			protected void CreateFixedElements(int summaryColumn, int separatorColumn, FixedStyle fixedStyle) {
				SummaryControl = CreateSummaryItemsControl(summaryColumn, fixedStyle);
				FixedLine = CreateFixedLine(separatorColumn);
				UpdateFixedSeparatorWidth();
				UpdateFixedSeparatorShowVertialLines();
				UpdateFixedSeparatorVisibility();
				Add(FixedLine);
			}
			GroupRowFixedLineSeparatorControl CreateFixedLine(int column) {
				var line = new GroupRowFixedLineSeparatorControl(GetFixedColumns, GetBands);
				System.Windows.Controls.Grid.SetColumn(line, column);
				return line;
			}
			internal void UpdateFixedSeparatorWidth() {
				if(FixedLine != null)
					FixedLine.Width = View.FixedLineWidth;
			}
			internal void UpdateFixedSeparatorShowVertialLines() {
				if(FixedLine != null)
					FixedLine.ShowVerticalLines = View.ShowVerticalLines;
			}
			internal void UpdateFixedSeparatorVisibility() {
				if(FixedLine != null)
					FixedLine.UpdateVisibility(View.ViewBase.DataControl);
			}
			protected override void Clear() {
				base.Clear();
				Remove(FixedLine);
				FixedLine = null;
			}
			protected abstract IList<ColumnBase> GetFixedColumns(TableViewBehavior viewBehavior);
			internal void InvalidatePanel() {
				if(SummaryControl == null)
					return;
				SummaryControl.InvalidateMeasure();
				if(SummaryControl.Panel == null)
					return;
				SummaryControl.Panel.InvalidateMeasure();
			}
		}
		class FixedLeftSummaryControlUpdater : FixedSummaryControlUpdaterBase {
			public FixedLeftSummaryControlUpdater(Panel layoutPanel, DataControlBase ownerControl) : base(layoutPanel, ownerControl) { }
			protected override void CreateElements() {
				CreateFixedElements(0, 1, FixedStyle.Left);
			}
			protected override IList<BandBase> GetBands(BandsLayoutBase bandsLayout) {
				return bandsLayout.FixedLeftVisibleBands;
			}
			protected override IList<GridGroupSummaryColumnData> GetFixedColumnData(GroupRowData data) {
				return data.FixedLeftGroupSummaryData;
			}
			protected override IList<ColumnBase> GetFixedColumns(TableViewBehavior viewBehavior) {
				return viewBehavior.FixedLeftVisibleColumns;
			}
			protected override bool HasElements(TableView view) {
				return view != null && view.TableViewBehavior.HasFixedLeftElements;
			}
		}
		class FixedRightSummaryControlUpdater : FixedSummaryControlUpdaterBase {
			public FixedRightSummaryControlUpdater(Panel layoutPanel, DataControlBase ownerControl) : base(layoutPanel, ownerControl) { }
			protected override void CreateElements() {
				CreateFixedElements(4, 3, FixedStyle.Right);
			}
			protected override IList<BandBase> GetBands(BandsLayoutBase bandsLayout) {
				return bandsLayout.FixedRightVisibleBands;
			}
			protected override IList<GridGroupSummaryColumnData> GetFixedColumnData(GroupRowData data) {
				return data.FixedRightGroupSummaryData;
			}
			protected override IList<ColumnBase> GetFixedColumns(TableViewBehavior viewBehavior) {
				return viewBehavior.FixedRightVisibleColumns;
			}
			protected override bool HasElements(TableView view) {
				return view != null && view.TableViewBehavior.HasFixedRightElements;
			}
		}
		#endregion
#if DEBUGTEST
		internal GroupRowAlignByColumnsSummaryControl FixedNoneControlForTests { get { return fixedNoneUpdater.SummaryControlForTests; } }
		internal GroupRowAlignByColumnsSummaryControl FixedLeftControlForTests { get { return fixedLeftUpdater.SummaryControlForTests; } }
		internal GroupRowAlignByColumnsSummaryControl FixedRightControlForTests { get { return fixedRightUpdater.SummaryControlForTests; } }
		internal FrameworkElement FixedLeftSeparatorForTests { get { return fixedLeftUpdater.FixedLine; } }
		internal FrameworkElement FixedRightSeparatorForTests { get { return fixedRightUpdater.FixedLine; } }
#endif
		readonly FixedNoneSummaryControlUpdater fixedNoneUpdater;
		readonly FixedSummaryControlUpdaterBase fixedLeftUpdater;
		readonly FixedSummaryControlUpdaterBase fixedRightUpdater;
		internal System.Windows.Controls.Grid LayoutPanel { get; private set; }
		public SummaryAlignByColumnsController(DataControlBase ownerControl) {
			LayoutPanel = new System.Windows.Controls.Grid();
			LayoutPanel.HorizontalAlignment = HorizontalAlignment.Left;
			ColumnDefinitionCollection columnDefinitions = LayoutPanel.ColumnDefinitions;
			columnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
			columnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
			columnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
			columnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
			columnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
			fixedNoneUpdater = new FixedNoneSummaryControlUpdater(LayoutPanel, ownerControl);
			fixedLeftUpdater = new FixedLeftSummaryControlUpdater(LayoutPanel, ownerControl);
			fixedRightUpdater = new FixedRightSummaryControlUpdater(LayoutPanel, ownerControl);
		}
		internal void UpdateData(GroupRowData data) {
			DoUpdateAction(u => u.UpdateData(data));
		}
		internal void SetScrollingMargin(Thickness scrollingMargin) {
			fixedNoneUpdater.SetScrollingMargin(scrollingMargin);
		}
		internal void UpdateGroupColumnSummaryItemTemplate(bool useDefaultTemplate) {
			DoUpdateAction(u => u.SetCanUseDefaultTemplate(useDefaultTemplate));
		}
		internal void UpdatePanel() {
			DoUpdateAction(u => u.UpdatePanel());
		}
		internal void UpdateBands(FixedStyle fixedStyle) {
			DoUpdateAction(fixedStyle, u => u.UpdateBands());
		}
		internal void SetLeftIndent(double leftIndent) {
			DoUpdateAction(u => u.SetLeftIndent(leftIndent));
		}		
		internal void UpdateFixedSeparatorWidth() {
			DoFixedSummaryUpdateAction(u => u.UpdateFixedSeparatorWidth());
		}
		internal void UpdateFixedSeparatorShowVertialLines() {
			DoFixedSummaryUpdateAction(u => u.UpdateFixedSeparatorShowVertialLines());
		}
		internal void UpdateFixedSeparatorVisibility() {
			UpdateFixedLeftSeparatorVisibility();
			UpdateFixedRightSeparatorVisibility();
		}
		internal void UpdateFixedLeftSeparatorVisibility() {
			fixedLeftUpdater.UpdateFixedSeparatorVisibility();
		}
		internal void UpdateFixedRightSeparatorVisibility() {
			fixedRightUpdater.UpdateFixedSeparatorVisibility();
		}
		internal void InvalidateFixedLeft() {
			fixedLeftUpdater.InvalidatePanel();
		}
		void DoUpdateAction(FixedStyle fixedStyle, Action<GroupSummaryControlUpdaterBase> updateAction) {
			GroupSummaryControlUpdaterBase updater;
			switch(fixedStyle) {
				case FixedStyle.None:
					updater = fixedNoneUpdater;
					break;
				case FixedStyle.Left:
					updater = fixedLeftUpdater;
					break;
				case FixedStyle.Right:
					updater = fixedRightUpdater;
					break;
				default:
					throw new InvalidOperationException();
			}
			updateAction(updater);
		}
		void DoUpdateAction(Action<GroupSummaryControlUpdaterBase> updateAction) {
			updateAction(fixedNoneUpdater);
			updateAction(fixedLeftUpdater);
			updateAction(fixedRightUpdater);
		}
		void DoFixedSummaryUpdateAction(Action<FixedSummaryControlUpdaterBase> updateAction) {
			updateAction(fixedLeftUpdater);
			updateAction(fixedRightUpdater);
		}
	}
}
