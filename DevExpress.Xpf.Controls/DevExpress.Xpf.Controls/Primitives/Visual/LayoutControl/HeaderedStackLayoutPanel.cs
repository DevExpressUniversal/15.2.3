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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Controls.Primitives {
	public interface IHeaderedStackPanelModel : IStackPanelModel {
		bool ShowGroupHeaders { get; }
	}
	public interface IHeaderedStackLayoutPanel : IStackLayoutPanel, IHeaderedStackPanelModel {
	}
	[StyleTypedProperty(Property = "GroupHeaderStyle", StyleTargetType = typeof(StackPanelGroupHeader))]
	public class HeaderedStackLayoutPanel : StackLayoutPanel, IHeaderedStackLayoutPanel, ILogicalOwner {
		public static readonly Brush DefaultBackground = new SolidColorBrush(Colors.Transparent);
		public const double DefaultGroupHeaderSpace = 5;
		public new const double DefaultItemSpacing = 10;
		public static readonly new Thickness DefaultPadding = new Thickness();
		#region Dependency Properties
		public static readonly DependencyProperty ShowGroupHeadersProperty =
			DependencyProperty.Register("ShowGroupHeaders", typeof(bool), typeof(HeaderedStackLayoutPanel), new PropertyMetadata(true, OnShowGroupHeadersChanged));
		public static readonly DependencyProperty GroupHeaderProperty =
			DependencyProperty.RegisterAttached("GroupHeader", typeof(object), typeof(HeaderedStackLayoutPanel),
				new PropertyMetadata(OnAttachedPropertyChanged));
		public static readonly DependencyProperty GroupHeaderSpaceProperty =
			DependencyProperty.Register("GroupHeaderSpace", typeof(double), typeof(HeaderedStackLayoutPanel),
				new PropertyMetadata(DefaultGroupHeaderSpace, (o, e) => ((HeaderedStackLayoutPanel)o).OnGroupHeaderSpaceChanged()));
		public static readonly DependencyProperty GroupHeaderStyleProperty =
			DependencyProperty.Register("GroupHeaderStyle", typeof(Style), typeof(HeaderedStackLayoutPanel),
				new PropertyMetadata((o, e) => ((HeaderedStackLayoutPanel)o).GroupHeaders.ItemStyle = (Style)e.NewValue));
		public static readonly DependencyProperty GroupHeaderTemplateProperty =
			DependencyProperty.Register("GroupHeaderTemplate", typeof(DataTemplate), typeof(HeaderedStackLayoutPanel),
				new PropertyMetadata((o, e) => ((HeaderedStackLayoutPanel)o).GroupHeaders.ItemContentTemplate = (DataTemplate)e.NewValue));
		public static object GetGroupHeader(DependencyObject element) {
			return element.GetValue(GroupHeaderProperty);
		}
		public static void SetGroupHeader(DependencyObject element, object value) {
			element.SetValue(GroupHeaderProperty, value);
		}
		private static void OnShowGroupHeadersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((HeaderedStackLayoutPanel)d).OnShowGroupHeadersChanged((bool)e.OldValue, (bool)e.NewValue);
		}
		#endregion Dependency Properties
#if !SILVERLIGHT
		static HeaderedStackLayoutPanel() {
			BackgroundProperty.OverrideMetadata(typeof(HeaderedStackLayoutPanel), new FrameworkPropertyMetadata(DefaultBackground));
			ItemSpacingProperty.OverrideMetadata(typeof(HeaderedStackLayoutPanel), new PropertyMetadata(DefaultItemSpacing));
			PaddingProperty.OverrideMetadata(typeof(HeaderedStackLayoutPanel), new PropertyMetadata(DefaultPadding));
		}
#endif
		internal HeaderedStackLayoutPanel() {
			GroupHeaders = CreateGroupHeaders();
		}
		public bool ShowGroupHeaders {
			get { return (bool)GetValue(ShowGroupHeadersProperty); }
			set { SetValue(ShowGroupHeadersProperty, value); }
		}
		public double GroupHeaderSpace {
			get { return (double)GetValue(GroupHeaderSpaceProperty); }
			set { SetValue(GroupHeaderSpaceProperty, value); }
		}
		public Style GroupHeaderStyle {
			get { return (Style)GetValue(GroupHeaderStyleProperty); }
			set { SetValue(GroupHeaderStyleProperty, value); }
		}
		public DataTemplate GroupHeaderTemplate {
			get { return (DataTemplate)GetValue(GroupHeaderTemplateProperty); }
			set { SetValue(GroupHeaderTemplateProperty, value); }
		}
		#region Children
		protected override bool IsTempChild(UIElement child) {
			return base.IsTempChild(child) || GroupHeaders.IsItem(child);
		}
		#endregion Children
		#region Layout
		protected override Size OnArrange(Rect bounds) {
			GroupHeaders.MarkItemsAsUnused();
			Size result = base.OnArrange(bounds);
			GroupHeaders.DeleteUnusedItems();
			return result;
		}
		protected virtual StackPanelGroupHeaders CreateGroupHeaders() {
			return new StackPanelGroupHeaders(this);
		}
		protected override LayoutPanelProviderBase CreateLayoutProvider() {
			return new HeaderedStackLayoutProvider(this);
		}
		protected override LayoutPanelParametersBase CreateLayoutProviderParameters() {
			return new HeaderedStackLayoutPanelParameters(ItemSpacing, AllowItemClipping, AllowItemAlignment, GroupHeaderSpace, GroupHeaders);
		}
		#endregion Layout
		protected override PanelControllerBase CreateController() {
			return new HeaderedStackLayoutPanelController(this);
		}
		protected override void OnAttachedPropertyChanged(FrameworkElement child, DependencyProperty property, object oldValue, object newValue) {
			base.OnAttachedPropertyChanged(child, property, oldValue, newValue);
			if(property == GroupHeaderProperty)
				OnGroupHeaderChanged(child);
		}
		protected virtual void OnGroupHeaderChanged(FrameworkElement child) {
			InvalidateArrange();
		}
		protected virtual void OnGroupHeaderSpaceChanged() {
			InvalidateArrange();
		}
		protected virtual void OnShowGroupHeadersChanged(bool oldValue, bool newValue) {
			InvalidateArrange();
		}
		protected StackPanelGroupHeaders GroupHeaders { get; private set; }
		#region ILogicalOwner Members
		void ILogicalOwner.AddChild(object child) {
			AddLogicalChild(child);
		}
		void ILogicalOwner.RemoveChild(object child) {
			RemoveLogicalChild(child);
		}
		#endregion
		#region IHeaderedStackPanelModel Members
		bool IHeaderedStackPanelModel.ShowGroupHeaders {
			get { return IsItemsHost ? false : ShowGroupHeaders; }
		}
		#endregion
	}
	public class StackPanelGroupHeader : ContentControlBase {
		public StackPanelGroupHeader() {
			DefaultStyleKey = typeof(StackPanelGroupHeader);
		}
		protected override bool IsContentInLogicalTree { get { return false; } }
	}
	public class StackPanelGroupHeaders : ElementPool<StackPanelGroupHeader> {
		private DataTemplate _ItemContentTemplate;
		public StackPanelGroupHeaders(Panel container) : base(container) { }
		public DataTemplate ItemContentTemplate {
			get { return _ItemContentTemplate; }
			set {
				if(ItemContentTemplate == value)
					return;
				_ItemContentTemplate = value;
				OnItemContentTemplateChanged();
			}
		}
		protected override StackPanelGroupHeader CreateItem() {
			StackPanelGroupHeader result = CreateItemCore();
			if(ItemContentTemplate != null)
				UpdateItemContentTemplate(result);
			return result;
		}
		protected virtual StackPanelGroupHeader CreateItemCore() {
			return base.CreateItem();
		}
		protected virtual void OnItemContentTemplateChanged() {
			Items.ForEach(UpdateItemContentTemplate);
		}
		private void UpdateItemContentTemplate(StackPanelGroupHeader item) {
			item.SetValueIfNotDefault(ContentControlBase.ContentTemplateProperty, ItemContentTemplate);
		}
		public new StackPanelGroupHeader Add() {
			var result = base.Add();
			if(!Container.Children.Contains(result)) Container.Children.Add(result);
			return result;
		}
		public new void DeleteUnusedItems() {
			for(int i = Items.Count - 1; i >= 0; i--) {
				StackPanelGroupHeader item = Items[i];
				if(!IsItemUsed(item)) {
					((ILogicalOwner)Container).RemoveChild(item);
				}
			}
			base.DeleteUnusedItems();
		}
		protected override bool IsItemInTree(StackPanelGroupHeader item) {
			return base.IsItemInTree(item) || Container.Children.Contains(item);
		}
	}
	public class HeaderedStackLayoutPanelParameters : StackLayoutPanelParameters {
		public HeaderedStackLayoutPanelParameters(double itemSpace, bool allowItemClipping, bool allowItemAlignment, double groupHeaderSpace, StackPanelGroupHeaders groupHeaders) :
			base(itemSpace, allowItemClipping, allowItemAlignment) {
			GroupHeaders = groupHeaders;
			GroupHeaderSpace = groupHeaderSpace;
		}
		public StackPanelGroupHeaders GroupHeaders { get; private set; }
		public double GroupHeaderSpace { get; private set; }
	}
	public class HeaderedStackLayoutProvider : StackLayoutPanelProvider {
		new IHeaderedStackPanelModel Model { get { return base.Model as IHeaderedStackPanelModel; } }
		public HeaderedStackLayoutProvider(IHeaderedStackPanelModel model) : base(model) { }
		public virtual bool ShowGroupHeaders {
			get { return Model.ShowGroupHeaders; }
		}
		protected override Size OnArrange(FrameworkElements items, Rect bounds, Rect viewPortBounds) {
			Size result = base.OnArrange(items, bounds, viewPortBounds);
			if(ShowGroupHeaders)
				AddGroupHeaders();
			return result;
		}
		protected void AddGroupHeader(object headerSource, Rect headerAreaBounds) {
			StackPanelGroupHeader groupHeader = Parameters.GroupHeaders.Add();
			groupHeader.SetBinding(StackPanelGroupHeader.ContentProperty,
				new Binding { Source = headerSource, Path = new PropertyPath(HeaderedStackLayoutPanel.GroupHeaderProperty), Mode = BindingMode.TwoWay });
#if !SILVERLIGHT
			groupHeader.InvalidateParentsOfModifiedChildren();
#endif
			groupHeader.Measure(headerAreaBounds.Size());
			groupHeader.Arrange(GetGroupHeaderBounds(headerAreaBounds, groupHeader.DesiredSize));
		}
		protected virtual void AddGroupHeaders() {
			for(int i = 0; i < LayoutItems.Count; i++) {
				FrameworkElement child = LayoutItems[i];
				object header = HeaderedStackLayoutPanel.GetGroupHeader(child);
				if(header != null) {
					AddGroupHeader(child, GetGroupHeaderAreaBounds(i));
				}
			}
		}
		protected virtual Rect GetGroupBounds(int groupFirstLayerInfoIndex) {
			Rect result = ArrangeInfos[groupFirstLayerInfoIndex].ArrangeBounds;
			for(int i = groupFirstLayerInfoIndex + 1; i < LayoutItems.Count; i++) {
				object header = HeaderedStackLayoutPanel.GetGroupHeader(LayoutItems[i]);
				if(header == null) {
					var arrangeInfo = ArrangeInfos[i];
					result.Union(arrangeInfo.ArrangeBounds);
				}
				else
					break;
			}
			return result;
		}
		protected virtual Rect GetGroupHeaderAreaBounds(int groupFirstLayerInfoIndex) {
			Rect result = GetGroupBounds(groupFirstLayerInfoIndex);
			result.Y -= Parameters.GroupHeaderSpace;
			result.Height = double.PositiveInfinity;
			return result;
		}
		protected virtual Rect GetGroupHeaderBounds(Rect headerAreaBounds, Size headerSize) {
			Rect result = headerAreaBounds;
			result.Y -= headerSize.Height;
			result.Height = headerSize.Height;
			return result;
		}
		protected new HeaderedStackLayoutPanelParameters Parameters { get { return base.Parameters as HeaderedStackLayoutPanelParameters; } }
	}
	public class HeaderedStackLayoutPanelController : StackLayoutPanelController {
		public HeaderedStackLayoutPanelController(IStackLayoutPanel control) : base(control) { }
		public new IHeaderedStackLayoutPanel ILayoutControl { get { return base.ILayoutControl as IHeaderedStackLayoutPanel; } }
		#region Drag&Drop
		protected override DragAndDropController CreateItemDragAndDropController(Point startDragPoint, FrameworkElement dragControl) {
			return new HeaderedStackLayoutPanelDragAndDropControllerBase(this, startDragPoint, dragControl);
		}
		#endregion Drag&Drop
	}
	public class HeaderedStackLayoutPanelDragAndDropControllerBase : StackLayoutPanelDragAndDropControllerBase {
		public HeaderedStackLayoutPanelDragAndDropControllerBase(Controller controller, Point startDragPoint, FrameworkElement dragControl) :
			base(controller, startDragPoint, dragControl) { }
		public override void EndDragAndDrop(bool accept) {
			if(accept)
				HeaderedStackLayoutPanel.SetGroupHeader(DragControl, HeaderedStackLayoutPanel.GetGroupHeader(DragControlPlaceHolder));
			else
				RestoreGroupHeaderOriginalValues();
			base.EndDragAndDrop(accept);
		}
		protected override FrameworkElement CreateDragControlPlaceHolder() {
			return base.CreateDragControlPlaceHolder();
		}
		protected override void InitDragControlPlaceHolder() {
			base.InitDragControlPlaceHolder();
			HeaderedStackLayoutPanel.SetGroupHeader(DragControlPlaceHolder, HeaderedStackLayoutPanel.GetGroupHeader(DragControl));
		}
		protected void MoveGroupHeaderAndStoreOriginalValues(UIElement from, UIElement to) {
			if(GroupHeaderOriginalValues == null)
				GroupHeaderOriginalValues = new Dictionary<UIElement, object>();
			object groupHeader = HeaderedStackLayoutPanel.GetGroupHeader(from);
			if(!GroupHeaderOriginalValues.ContainsKey(from))
				GroupHeaderOriginalValues.Add(from, groupHeader);
			HeaderedStackLayoutPanel.SetGroupHeader(from, DependencyProperty.UnsetValue);
			if(to != null) {
				if(!GroupHeaderOriginalValues.ContainsKey(to))
					GroupHeaderOriginalValues.Add(to, HeaderedStackLayoutPanel.GetGroupHeader(to));
				HeaderedStackLayoutPanel.SetGroupHeader(to, groupHeader);
			}
		}
		protected void RestoreGroupHeaderOriginalValues() {
			if(GroupHeaderOriginalValues == null)
				return;
			foreach(KeyValuePair<UIElement, object> originalValue in GroupHeaderOriginalValues)
				HeaderedStackLayoutPanel.SetGroupHeader(originalValue.Key, originalValue.Value);
		}
		protected new HeaderedStackLayoutPanelController Controller { get { return (HeaderedStackLayoutPanelController)base.Controller; } }
		private Dictionary<UIElement, object> GroupHeaderOriginalValues { get; set; }
	}
}
