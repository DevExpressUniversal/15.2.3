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
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils;
using System.Windows.Data;
using System.Collections.Generic;
using System.Windows.Shapes;
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Grid.HitTest;
#if !SILVERLIGHT
using System.ComponentModel;
using System.Windows.Markup;
#else
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
#endif
namespace DevExpress.Xpf.Grid {
	public abstract class GroupGridControl : ContentControl, IGroupRow {
		static GroupGridControl() {
			DataControlPopupMenu.GridMenuTypeProperty.OverrideMetadata(typeof(GroupGridControl), new FrameworkPropertyMetadata(GridMenuType.GroupRow));
		}
		protected GroupGridControl() {
			RowData.SetRowHandleBinding(this);
		}
		FrameworkElement HeaderContentPresenter { get; set; }
		internal RowsContainer LogicalItemsContainer { get { return ((GroupRowData)DataContext).RowsContainer; } }
		protected override void OnPreviewMouseRightButtonDown(System.Windows.Input.MouseButtonEventArgs e) {
			base.OnPreviewMouseRightButtonDown(e);
			AssignContextMenu();
		}
		protected GroupRowData GroupRowData { get { return DataContext as GroupRowData; } }
		protected DataViewBase View { get { return GroupRowData != null ? GroupRowData.View : null; } }
		internal void AssignContextMenu() {
			if(View != null)
				DevExpress.Xpf.Bars.BarManager.SetDXContextMenu(this, View.DataControlMenu);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			HeaderContentPresenter = GetTemplateChild("PART_HeaderContentPresenter") as FrameworkElement;
			if(HeaderContentPresenter != null) {
				ExpandHelper.SetRowContainer(HeaderContentPresenter, this);
				ExpandHelper.SetItemsContainer(HeaderContentPresenter, (FrameworkElement)GetTemplateChild("PART_ItemsContainer"));
				this.AddHandler(ExpandHelper.IsExpandedChangedEvent, new RoutedEventHandler((d, e) => OnIsExpandedChanged()));
			}
			RowData.ReassignCurrentRowData(this, HeaderContentPresenter);
			SetBinding(ExpandHelper.IsExpandedProperty, new Binding("IsRowExpanded"));
			OnIsExpandedChanged();
		}
		protected virtual void OnIsExpandedChanged() {
			LogicalItemsContainer.AnimationProgress = IsExpanded ? 1 : 0;
		}
		protected bool IsExpanded { get { return ExpandHelper.GetIsExpanded(this); } }
		protected override Size MeasureOverride(Size constraint) {
			return MeasurePixelSnapperHelper.MeasureOverride(base.MeasureOverride(constraint), SnapperType.Around);
		}
		#region IGroupRow Members
		FrameworkElement IGroupRow.RowElement {
			get { return HeaderContentPresenter; }
		}
		#endregion
	}
	public class GroupGridRow : GroupGridControl, IFixedGroupElement {
#if DEBUGTEST
		internal static double? DebugTestFixedHeight = null;
#endif
		public GroupGridRow() {
			this.SetDefaultStyleKey(typeof(GroupGridRow));
		}
		#region IFixedGroupElement Members
		FixedGroupElement fixedGroupElementCore = null;
		IFixedGroupElement FixedGroupElement {
			get {
				if(fixedGroupElementCore == null)
					fixedGroupElementCore = new FixedGroupElement(() => DataContext as GroupRowData);
				return fixedGroupElementCore;
			}
		}
		double IFixedGroupElement.GetLeftMargin(bool drawAdornerUnderWholeGroup) {
			return FixedGroupElement.GetLeftMargin(drawAdornerUnderWholeGroup);
		}
		double IFixedGroupElement.GetRightMargin(bool drawAdornerUnderWholeGroup) {
			return FixedGroupElement.GetRightMargin(drawAdornerUnderWholeGroup);
		}
		#endregion
	}
	public class GroupGridRowContent : ContentControl {
		public static readonly DependencyProperty CurrentHeightProperty =
							   DependencyPropertyManager.Register("CurrentHeight", typeof(double), typeof(GroupGridRowContent), new FrameworkPropertyMetadata(0.0));
		public double CurrentHeight {
			get { return (double)GetValue(CurrentHeightProperty); }
			set { SetValue(CurrentHeightProperty, value); }
		}
		public GroupGridRowContent() {
#if DEBUGTEST
			if(GroupGridRow.DebugTestFixedHeight.HasValue)
				Height = GroupGridRow.DebugTestFixedHeight.Value;
#endif
			this.SetDefaultStyleKey(typeof(GroupGridRowContent));
			SizeChanged += new SizeChangedEventHandler(GroupGridRowContent_SizeChanged);
		}
		void GroupGridRowContent_SizeChanged(object sender, SizeChangedEventArgs e) {
			CurrentHeight = e.NewSize.Height;
		}
	}
	public class GroupPanelControl : Control {
		public GroupPanelControl() {
			this.SetDefaultStyleKey(typeof(GroupPanelControl));
			GridControl.SetCurrentViewChangedListener(this, new GroupPanelInitializer());
			GridViewHitInfoBase.SetHitTestAcceptor(this, new GroupPanelTableViewHitTestAcceptor());
			GridPopupMenu.SetGridMenuType(this, GridMenuType.GroupPanel); 
		}
		public DataViewBase View {
			get { return (DataViewBase)GetValue(ViewProperty); }
			set { SetValue(ViewProperty, value); }
		}
		public static readonly DependencyProperty ViewProperty =
			DependencyPropertyManager.Register("View", typeof(DataViewBase), typeof(GroupPanelControl), new PropertyMetadata(null, (d, e) => ((GroupPanelControl)d).OnViewChanged()));
		public bool IsGrouped {
			get { return (bool)GetValue(IsGroupedProperty); }
			set { SetValue(IsGroupedProperty, value); }
		}
		public static readonly DependencyProperty IsGroupedProperty =
			DependencyPropertyManager.Register("IsGrouped", typeof(bool), typeof(GroupPanelControl), new PropertyMetadata(false, (d, e) => ((GroupPanelControl)d).UpdateGroupPanelDragText()));
		TextBlock groupPanelDragText;
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			groupPanelDragText = GetTemplateChild("PART_GroupPanelDragText") as TextBlock;
			UpdateGroupPanelDragText();
		}
		void UpdateGroupPanelDragText() {
			if(groupPanelDragText != null) {
				if((View != null) && (View.DataControl != null) && (View.DataControl.DetailDescriptorCore != null)) {
					groupPanelDragText.Visibility = Visibility.Collapsed;
					return;
				}
				groupPanelDragText.Visibility = IsGrouped ? Visibility.Collapsed : Visibility.Visible;
			}
		}
		void OnViewChanged() {
			DataControlBase.SetCurrentView(this, View);
		}
	}
	public interface IFixedGroupElement {
		double GetLeftMargin(bool drawAdornerUnderWholeGroup);
		double GetRightMargin(bool drawAdornerUnderWholeGroup);
	}
	class FixedGroupElement : IFixedGroupElement {
		readonly Func<GroupRowData> getRowDataFunc;
		#region IFixedGroupElement Members
		double IFixedGroupElement.GetLeftMargin(bool drawAdornerUnderWholeGroup) {
			GroupRowData data = getRowDataFunc();
			if(data == null)
				return 0;
			TableView view = (TableView)data.View;
			double leftIndent = 0;
			double tempIndent = 0;
			int parentGroupLevel = 0;
			GetDetailIndents(view, out leftIndent, out tempIndent, out parentGroupLevel);
			return (view.ActualShowIndicator ? view.IndicatorWidth : 0) + leftIndent + (parentGroupLevel + (view.UseGroupShadowIndent ? data.GroupLevel : 0) + (drawAdornerUnderWholeGroup ? 0 : 1)) * view.LeftGroupAreaIndent;
		}
		double IFixedGroupElement.GetRightMargin(bool drawAdornerUnderWholeGroup) {
			GroupRowData data = getRowDataFunc();
			if(data == null)
				return 0;
			TableView view = (TableView)data.View;
			int parentGroupLevel = 0;
			double tempIndent = 0;
			double rightIndent = 0;
			GetDetailIndents(view, out tempIndent, out rightIndent, out parentGroupLevel);
			return rightIndent;
		}
		#endregion
		public FixedGroupElement(Func<GroupRowData> getRowDataFunc) {
			this.getRowDataFunc = getRowDataFunc;
		}
		static void GetDetailIndents(TableView view, out double detailLeftIndent, out double detailRightIndent, out int detailParentGroupLevel) {
			double leftIndent = 0;
			double rightIndent = 0;
			int parentGroupLevel = 0;
			view.Grid.EnumerateThisAndParentDataControls(dataControl => {
				GridControl grid = dataControl as GridControl;
				if(grid == null)
					return;
				GridControl master = grid.GetMasterGrid();
				if(master != null)
					parentGroupLevel += master.View.GetRowData(grid.GetMasterRowHandle()).Level;
				TableView dataView = grid.View as TableView;
				if(dataView != view) {
					leftIndent += dataView.ActualDetailMargin.Left;
					rightIndent += dataView.ActualDetailMargin.Right;
				}
			});
			detailLeftIndent = leftIndent;
			detailRightIndent = rightIndent;
			detailParentGroupLevel = parentGroupLevel;
		}
		static void GetDetailLevels(TableView view, out int detailLevel, out int detailParentGroupLevel) {
			int level = -1;
			int parentGroupLevel = 0;
			view.Grid.EnumerateThisAndParentDataControls(dataControl => {
				level++;
				GridControl grid = dataControl as GridControl;
				if(grid == null)
					return;
				GridControl master = grid.GetMasterGrid();
				if(master != null)
					parentGroupLevel += master.View.GetRowData(grid.GetMasterRowHandle()).Level;
			});
			detailLevel = level;
			detailParentGroupLevel = parentGroupLevel;
		}
	}
	public class FixedGroupsAdorner : ContentControl {
		public IList<FrameworkElement> FixedElements {
			get { return (IList<FrameworkElement>)GetValue(FixedElementsProperty); }
			set { SetValue(FixedElementsProperty, value); }
		}
		public static readonly DependencyProperty FixedElementsProperty =
			DependencyPropertyManager.Register("FixedElements", typeof(IList<FrameworkElement>), typeof(FixedGroupsAdorner), new PropertyMetadata(null, (d, e) => ((FixedGroupsAdorner)d).OnChanged()));
		public FrameworkElement ParentControl {
			get { return (FrameworkElement)GetValue(ParentControlProperty); }
			set { SetValue(ParentControlProperty, value); }
		}
		public static readonly DependencyProperty ParentControlProperty =
			DependencyPropertyManager.Register("ParentControl", typeof(FrameworkElement), typeof(FixedGroupsAdorner), new PropertyMetadata(null, (d, e) => ((FixedGroupsAdorner)d).OnChanged()));
		public Brush AdornerBrush {
			get { return (Brush)GetValue(AdornerBrushProperty); }
			set { SetValue(AdornerBrushProperty, value); }
		}
		public static readonly DependencyProperty AdornerBrushProperty =
			DependencyPropertyManager.Register("AdornerBrush", typeof(Brush), typeof(FixedGroupsAdorner), new PropertyMetadata(null, (d, e) => ((FixedGroupsAdorner)d).OnChanged()));
		public double AdornerHeight {
			get { return (double)GetValue(AdornerHeightProperty); }
			set { SetValue(AdornerHeightProperty, value); }
		}
		public static readonly DependencyProperty AdornerHeightProperty =
			DependencyPropertyManager.Register("AdornerHeight", typeof(double), typeof(FixedGroupsAdorner), new PropertyMetadata(0d, (d, e) => ((FixedGroupsAdorner)d).OnChanged()));
		public bool DrawAdornerUnderWholeGroup {
			get { return (bool)GetValue(DrawAdornerUnderWholeGroupProperty); }
			set { SetValue(DrawAdornerUnderWholeGroupProperty, value); }
		}
		public static readonly DependencyProperty DrawAdornerUnderWholeGroupProperty =
			DependencyPropertyManager.Register("DrawAdornerUnderWholeGroup", typeof(bool), typeof(FixedGroupsAdorner), new PropertyMetadata(true, (d, e) => ((FixedGroupsAdorner)d).OnChanged()));
#if !SL
		[Browsable(false)]
		public bool ShouldSerializeFixedElements(XamlDesignerSerializationManager manager) {
			return false;
		}
#endif
		Panel Panel { get { return (Panel)Content; } }
		public FixedGroupsAdorner() {
			this.SetDefaultStyleKey(typeof(FixedGroupsAdorner));
			this.IsHitTestVisible = false;
			this.Content = new System.Windows.Controls.Grid() { IsHitTestVisible = false };
		}
		bool queued;
		void OnChanged() {
			if(queued)
				return;
			if(ParentControl == null)
				return;
			DataViewBase view = LayoutHelper.FindParentObject<DataViewBase>(ParentControl);
			if(view == null || view.DataPresenter == null)
				return;
			queued = true;
			view.EnqueueImmediateAction(() => {
				if(FixedElements == null || ParentControl == null)
					return;
				queued = false;
				Panel.Children.Clear();
				for(int i = 0; i < FixedElements.Count; i++) {
					Rect elementRect = LayoutHelper.GetRelativeElementRect(FixedElements[i], ParentControl);
					Path path = new Path() { Stroke = AdornerBrush, StrokeThickness = AdornerHeight, IsHitTestVisible = false };
					double lineCenter = elementRect.Bottom + AdornerHeight / 2;
					IFixedGroupElement fixedGroupElement = FixedElements[i] as IFixedGroupElement;
					double leftMargin = fixedGroupElement != null ? fixedGroupElement.GetLeftMargin(DrawAdornerUnderWholeGroup) : 0;
					double rightMargin = fixedGroupElement != null ? fixedGroupElement.GetRightMargin(DrawAdornerUnderWholeGroup) : 0;
					path.Data = new LineGeometry() { StartPoint = new Point(elementRect.Left + leftMargin, lineCenter), EndPoint = new Point(elementRect.Right - rightMargin, lineCenter) };
					path.Width = elementRect.Width;
					Panel.Children.Add(path);
				}
			});
		}
	}
}
