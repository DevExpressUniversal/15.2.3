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

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Layout.Core;
namespace DevExpress.Xpf.Docking.VisualElements {
	public class GroupPanel : psvGrid {
		#region static
		public static readonly DependencyProperty OrientationProperty;
		public static readonly DependencyProperty LastChildFillProperty;
		static GroupPanel() {
			var dProp = new DependencyPropertyRegistrator<GroupPanel>();
			dProp.Register("Orientation", ref OrientationProperty, Orientation.Horizontal,
				(dObj, e) => ((GroupPanel)dObj).OnOrientatonChanged((Orientation)e.NewValue));
			dProp.Register("LastChildFill", ref LastChildFillProperty, true,
				(dObj, e) => ((GroupPanel)dObj).OnLastChildFillChanged((bool)e.NewValue));
		}
		#endregion static
		public GroupPanel() {
			CheckChildrenParity = true;
		}
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		public bool LastChildFill {
			get { return (bool)GetValue(LastChildFillProperty); }
			set { SetValue(LastChildFillProperty, value); }
		}
		protected virtual void OnOrientatonChanged(Orientation orientation) {
			RebuildLayout();
		}
		protected virtual void OnLastChildFillChanged(bool value) {
			RebuildLayout();
		}
		protected void ClearParameters() {
			RowDefinitions.Clear();
			ColumnDefinitions.Clear();
			isValidLayout = false;
		}
		bool postponeRebuild;
		internal bool CheckChildrenParity { get; set; }
		protected virtual bool SkipRebuildLayout() {
			return IsDisposing || !IsLoadedComplete || (CheckChildrenParity && Children.Count % 2 == 0) || RowDefinitions.IsReadOnly || ColumnDefinitions.IsReadOnly;
		}
		bool isValidLayout;
		void QueueInvlidateMeasure() {
			Dispatcher.BeginInvoke(new System.Action(() => { InvalidateMeasure(); }));
		}
		void CorrectDefectiveMeasure(double constraint) {
			if(deffectiveMeasureCount > 0) deffectiveMeasureCount = 0;
			else {
				deffectiveMeasureCount++;
				QueueInvlidateMeasure();
				return;
			}
			double stars = 0;
			double pixels = 0;
			foreach(UIElement child in InternalChildren) {
				BaseLayoutItem item = GetLayoutItem(child);
				if(item == null) continue;
				DefinitionBase definition = DefinitionsHelper.GetDefinition(item);
				if(definition == null) continue;
				GridLength length = definition.GetLength();
				if(length.IsStar) {
					stars += length.Value;
					pixels += definition.GetActualLength();
				}
			}
			if(pixels == 0d) return;
			bool shouldRemeasure = false;
			foreach(UIElement child in InternalChildren) {
				BaseLayoutItem item = GetLayoutItem(child);
				if(item == null) continue;
				DefinitionBase definition = DefinitionsHelper.GetDefinition(item);
				if(definition == null) continue;
				GridLength length = definition.GetLength();
				if(length.IsStar) {
					var actualLength = definition.GetActualLength();
					var minValue = IsHorizontal ? item.ActualMinSize.Width : item.ActualMinSize.Height;
					var oldValue = (GridLength)item.GetValue(Orientation == Orientation.Horizontal ? BaseLayoutItem.ItemWidthProperty : BaseLayoutItem.ItemHeightProperty);
					if(actualLength <= minValue || !oldValue.IsStar) continue;
					var newValue = new GridLength(actualLength * stars / pixels, GridUnitType.Star);
					if(!object.Equals(oldValue, newValue) && newValue != DefinitionsHelper.ZeroStarLength) {
						item.SetValue(Orientation == Orientation.Horizontal ? BaseLayoutItem.ItemWidthProperty : BaseLayoutItem.ItemHeightProperty, newValue);
						shouldRemeasure = true;
					}
				}
			}
			if(shouldRemeasure) QueueInvlidateMeasure();
		}
		double GetLength(Size s, bool IsHorizontal) {
			return IsHorizontal ? s.Width : s.Height;
		}
		double deffectiveMeasureCount = 0;
		bool IsHorizontal { get { return Orientation == System.Windows.Controls.Orientation.Horizontal; } }
		protected override Size MeasureOverride(Size constraint) {
			if(!isValidLayout) return new Size();
			Size baseSize = base.MeasureOverride(constraint);
			double constraintLength = GetLength(constraint, IsHorizontal);
			double baseLength = GetLength(baseSize, IsHorizontal);
			if(baseLength - constraintLength > 1 && constraintLength > 0)
				CorrectDefectiveMeasure(constraintLength);
			return baseSize;
		}
		protected override Size ArrangeOverride(Size arrangeSize) {
			if(!isValidLayout) return new Size();
			return base.ArrangeOverride(arrangeSize);
		}
		DocumentGroup documentHost;
		DocumentGroup DocumentHost { get { return documentHost; } }
		DocumentGroup GetDocumentHost(LayoutGroup ownerGroup) {
			DocumentGroup documentHost = null;
			if(ownerGroup != null) {
				var documentGroups = ownerGroup.GetNestedPanels().OfType<DocumentGroup>();
				documentHost = documentGroups.FirstOrDefault(x => x.HasNotCollapsedItems);
				if(documentHost == null) {
					documentHost = documentGroups.FirstOrDefault(x => x.ShowWhenEmpty);
					if(documentHost == null) {
						documentHost = documentGroups.FirstOrDefault();
					}
				}
			}
			return documentHost;
		}
		LayoutGroup GetOwnerGroup() {
			return DockLayoutManager.GetLayoutItem(this) as LayoutGroup;
		}
		protected internal virtual void RebuildLayout() {
			try {
				if(SkipRebuildLayout()) {
					postponeRebuild = true;
					return;
				}
				postponeRebuild = false;
				ClearParameters();
				int index = 0;
				bool horz = Orientation == Orientation.Horizontal;
				bool splitterNeeded = false;
				documentHost = GetDocumentHost(GetOwnerGroup());
				foreach(UIElement element in Children) {
					BaseLayoutItem item = GetLayoutItem(element) ?? (element as Splitter).With(x => x.LayoutGroup);
					if(item == null) return;
					if(element is Splitter) {
						BaseLayoutItem nextItem = GetLayoutItem(Children[index + 1]);
						if(!CanSkipItem(nextItem) && splitterNeeded) {
							splitterNeeded = false;
							element.Visibility = Visibility.Visible;
						}
						else {
							element.Visibility = Visibility.Collapsed;
						}
						((Splitter)element).InitSplitThumb(horz);
						DependencyProperty interval = IntervalHelper.GetTargetProperty(
								GetLayoutItem(Children[index - 1]), GetLayoutItem(Children[index + 1])
							);
						DoubleToGridLengthConverter converter = new DoubleToGridLengthConverter();
						if(horz) {
							ColumnDefinition column = new ColumnDefinition() { Width = new GridLength() };
							if(element.Visibility == System.Windows.Visibility.Visible)
								BindingHelper.SetBinding(column, ColumnDefinition.WidthProperty, item, interval, converter);
							DefinitionsHelper.SetDefinition(element, column);
							ColumnDefinitions.Add(column);
						}
						else {
							RowDefinition row = new RowDefinition() { Height = new GridLength() };
							if(element.Visibility == System.Windows.Visibility.Visible)
								BindingHelper.SetBinding(row, RowDefinition.HeightProperty, item, interval, converter);
							DefinitionsHelper.SetDefinition(element, row);
							RowDefinitions.Add(row);
						}
					}
					else {
						if(item is LayoutControlItem)
							item.ClearValue(LayoutControlItem.DesiredSizeInternalProperty);
						bool isItemVisible = !CanSkipItem(item);
						if(isItemVisible)
							splitterNeeded = true;
						if(horz) {
							ColumnDefinition column = new ColumnDefinition() { Width = DefinitionsHelper.ZeroLength };
							if(isItemVisible) {
								BindingHelper.SetBinding(column, ColumnDefinition.WidthProperty, item, "ItemWidth");
								column.SetMinSize(item.ActualMinSize);
								column.SetMaxSize(item.ActualMaxSize);
							}
							DefinitionsHelper.SetDefinition(item, column);
							ColumnDefinitions.Add(column);
						}
						else {
							RowDefinition row = new RowDefinition() { Height = DefinitionsHelper.ZeroLength };
							if(isItemVisible) {
								BindingHelper.SetBinding(row, RowDefinition.HeightProperty, item, "ItemHeight");
								row.SetMinSize(item.ActualMinSize);
								row.SetMaxSize(item.ActualMaxSize);
							}
							DefinitionsHelper.SetDefinition(item, row);
							RowDefinitions.Add(row);
						}
					}
					element.SetValue(horz ? Grid.ColumnProperty : Grid.RowProperty, index++);
				}
				UpdateDefinitionsForSingleChild();
				isValidLayout = true;
			}
			finally {
				documentHost = null;
			}
		}
		void UpdateDefinitionsForSingleChild() {
			if(LastChildFill) {
				if(RowDefinitions.Count == 1 && !DefinitionsHelper.IsZero(RowDefinitions[0])) RowDefinitions[0].ClearValue(RowDefinition.HeightProperty);
				if(ColumnDefinitions.Count == 1 && !DefinitionsHelper.IsZero(ColumnDefinitions[0])) ColumnDefinitions[0].ClearValue(ColumnDefinition.WidthProperty);
			}
		}
		static BaseLayoutItem GetLayoutItem(DependencyObject element) {
			BaseLayoutItem item = (element as MultiTemplateControl).Return(x => x.LayoutItem, () => null);
			return item ?? element as BaseLayoutItem;
		}
		bool CanSkipItem(BaseLayoutItem item) {
			return item.Visibility == Visibility.Collapsed || CanSkipGroup(item as LayoutGroup);
		}
		bool CanSkipGroup(LayoutGroup group) {
			if(group == null) return false;
			if(group is FloatGroup) return false;
			DocumentGroup dGroup = group as DocumentGroup;
			if(dGroup != null && dGroup.Parent != null) {
				return !(dGroup.HasNotCollapsedItems || dGroup == DocumentHost);
			}
			return !group.HasNotCollapsedItems && group.GroupBorderStyle == GroupBorderStyle.NoBorder && !group.GetIsDocumentHost();
		}
		protected override void OnLoaded() {
			base.OnLoaded();
			if(postponeRebuild) RebuildLayout();
		}
		protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved) {
			base.OnVisualChildrenChanged(visualAdded, visualRemoved);
			if(visualRemoved != null) {
				DependencyObject dObj = visualRemoved as Splitter ?? (GetLayoutItem(visualRemoved) ?? visualRemoved);
				DefinitionsHelper.SetDefinition(dObj, null);
			}
		}
		protected override UIElementCollection CreateUIElementCollection(FrameworkElement logicalParent) {
			return new UIElementCollection(this, logicalParent);
		}
	}
	[TemplatePart(Name = "PART_Content", Type = typeof(GroupPaneContentPresenter))]
	public class GroupPane : psvContentControl {
		#region static
		public static readonly DependencyProperty NoBorderTemplateProperty;
		public static readonly DependencyProperty GroupTemplateProperty;
		public static readonly DependencyProperty GroupBoxTemplateProperty;
		public static readonly DependencyProperty TabbedTemplateProperty;
		static GroupPane() {
			var dProp = new DependencyPropertyRegistrator<GroupPane>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("NoBorderTemplate", ref NoBorderTemplateProperty, (DataTemplate)null);
			dProp.Register("GroupTemplate", ref GroupTemplateProperty, (DataTemplate)null);
			dProp.Register("GroupBoxTemplate", ref GroupBoxTemplateProperty, (DataTemplate)null);
			dProp.Register("TabbedTemplate", ref TabbedTemplateProperty, (DataTemplate)null);
		}
		#endregion static
		public GroupPane() {
			FocusVisualStyle = null;
			IsTabStop = false;
		}
		protected override void OnDispose() {
			if(PartContent != null) {
				PartContent.Dispose();
				PartContent = null;
			}
			if(PartLayoutItemsControl != null) {
				PartLayoutItemsControl.Dispose();
				PartLayoutItemsControl = null;
			}
			ClearValue(NoBorderTemplateProperty);
			ClearValue(GroupTemplateProperty);
			ClearValue(NoBorderTemplateProperty);
			ClearValue(TabbedTemplateProperty);
			base.OnDispose();
		}
		public DataTemplate NoBorderTemplate {
			get { return (DataTemplate)GetValue(NoBorderTemplateProperty); }
			set { SetValue(NoBorderTemplateProperty, value); }
		}
		public DataTemplate GroupTemplate {
			get { return (DataTemplate)GetValue(GroupTemplateProperty); }
			set { SetValue(GroupTemplateProperty, value); }
		}
		public DataTemplate GroupBoxTemplate {
			get { return (DataTemplate)GetValue(GroupBoxTemplateProperty); }
			set { SetValue(GroupBoxTemplateProperty, value); }
		}
		public DataTemplate TabbedTemplate {
			get { return (DataTemplate)GetValue(TabbedTemplateProperty); }
			set { SetValue(TabbedTemplateProperty, value); }
		}
		public GroupPaneContentPresenter PartContent { get; private set; }
		public LayoutItemsControl PartLayoutItemsControl { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if(PartContent != null && !LayoutItemsHelper.IsTemplateChild(PartContent, this))
				PartContent.Dispose();
			PartContent = GetTemplateChild("PART_Content") as GroupPaneContentPresenter;
			if(PartContent != null)
				PartContent.EnsureOwner(this);
		}
		protected override void EnsureContentElementCore(DependencyObject element) {
			base.EnsureContentElementCore(element);
			ClearLayoutItemsControlBindings(PartLayoutItemsControl);
			PartLayoutItemsControl = element as LayoutItemsControl;
			CreateLayoutItemsControlBindings(PartLayoutItemsControl);
		}
		void CreateLayoutItemsControlBindings(LayoutItemsControl itemsControl) {
			if(itemsControl != null) {
				BindingHelper.SetBinding(itemsControl, LayoutItemsControl.OrientationProperty, LayoutItem, "Orientation");
				BindingHelper.SetBinding(itemsControl, LayoutItemsControl.ItemsSourceProperty, LayoutItem, "ItemsInternal");
				BindingHelper.SetBinding(itemsControl, LayoutItemsControl.LastChildFillProperty, LayoutItem, "LastChildFill");
			}
		}
		void ClearLayoutItemsControlBindings(LayoutItemsControl itemsControl) {
			if(itemsControl != null) {
				itemsControl.ClearValue(LayoutItemsControl.OrientationProperty);
				psvItemsControl.Clear(itemsControl);
			}
		}
		protected override void OnLayoutItemChanged(BaseLayoutItem item, BaseLayoutItem oldItem) {
			base.OnLayoutItemChanged(item, oldItem);
			if(oldItem != null)
				ClearLayoutItemsControlBindings(PartLayoutItemsControl);
			if(item != null)
				CreateLayoutItemsControlBindings(PartLayoutItemsControl);
		}
	}
	public class GroupPaneContentPresenter : BasePanePresenter<GroupPane, LayoutGroup> {
		#region static
		public static readonly DependencyProperty GroupBorderStyleProperty;
		static GroupPaneContentPresenter() {
			var dProp = new DependencyPropertyRegistrator<GroupPaneContentPresenter>();
			dProp.Register("GroupBorderStyle", ref GroupBorderStyleProperty, GroupBorderStyle.NoBorder,
				(dObj, e) => ((GroupPaneContentPresenter)dObj).OnStylePropertyChanged());
		}
		#endregion static
		public GroupBorderStyle GroupBorderStyle {
			get { return (GroupBorderStyle)GetValue(GroupBorderStyleProperty); }
			set { SetValue(GroupBorderStyleProperty, value); }
		}
		protected override bool CanSelectTemplate(LayoutGroup group) {
			return group.ActualGroupTemplateSelector != null;
		}
		protected override DataTemplate SelectTemplateCore(LayoutGroup group) {
			return group.ActualGroupTemplateSelector.SelectTemplate(group, this);
		}
		protected psvItemsControl PartLayoutItemsControl { get; private set; }
		protected BaseGroupContentControl PartGroupContentControl { get; private set; }
		protected override void OnDispose() {
			if(PartLayoutItemsControl != null) {
				PartLayoutItemsControl.Dispose();
				PartLayoutItemsControl = null;
			}
			if(PartGroupContentControl != null) {
				PartGroupContentControl.Dispose();
				PartGroupContentControl = null;
			}
			base.OnDispose();
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if(PartLayoutItemsControl != null && !LayoutItemsHelper.IsTemplateChild(PartLayoutItemsControl, this))
				PartLayoutItemsControl.Dispose();
			PartLayoutItemsControl = LayoutItemsHelper.GetTemplateChild<psvItemsControl>(this);
			if(PartGroupContentControl != null && !LayoutItemsHelper.IsTemplateChild(PartGroupContentControl, this))
				PartGroupContentControl.Dispose();
			PartGroupContentControl = LayoutItemsHelper.GetTemplateChild<BaseGroupContentControl>(this); ;
			if(PartGroupContentControl != null)
				PartGroupContentControl.Loaded += PartGroupContentControl_Loaded;
		}
		void PartGroupContentControl_Loaded(object sender, RoutedEventArgs e) {
			((BaseGroupContentControl)sender).Loaded -= PartGroupContentControl_Loaded;
			if(IsDisposing) return;
			PartLayoutItemsControl = LayoutItemsHelper.GetTemplateChild<psvItemsControl>(this); ;
		}
		protected override LayoutGroup ConvertToLogicalItem(object content) {
			return LayoutItemData.ConvertToBaseLayoutItem(content) as LayoutGroup ?? base.ConvertToLogicalItem(content);
		}
	}
}
