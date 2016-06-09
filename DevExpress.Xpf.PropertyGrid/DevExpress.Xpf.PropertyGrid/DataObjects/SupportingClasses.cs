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

using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Xpf.Core.Internal;
namespace DevExpress.Xpf.PropertyGrid {
	public class RowDataBaseCollection : ObservableCollection<RowDataBase> {
		RowDataBase owner;
		public RowDataBase Owner {
			get { return owner; }
			set {
				if (value == owner) return;
				RowDataBase oldValue = owner;
				owner = value;
				OnOwnerChanged(oldValue);
			}
		}		
		public RowDataBaseCollection(RowDataBase owner) {
			Owner = owner;
		}
		protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			base.OnCollectionChanged(e);
			if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace || e.Action == NotifyCollectionChangedAction.Reset)
				ForceUpdateChildren(e.NewItems,
					rb => { rb.Level = Owner.Level + ((Owner.IsCategory || Owner.Handle.IsRoot) ? 0 : 1); },
					rb => { BindingOperations.SetBinding(rb, RowDataBase.ParentProperty, new Binding() { Source = Owner }); }
					);
		}
		protected virtual void OnOwnerChanged(RowDataBase oldValue) {
			ForceUpdateChildren(this,
					rb => { rb.Level = Owner.Level + ((Owner.IsCategory || Owner.Handle.IsRoot) ? 0 : 1); },
					rb => { BindingOperations.SetBinding(rb, RowDataBase.ParentProperty, new Binding() { Source = Owner }); }
					);
		}
		public virtual void ForceUpdateChildren(IList list, params Action<RowDataBase>[] updateActions) {
			list = list ?? this;
			if (Owner != null) {
				foreach (RowDataBase item in list) {
					foreach (var action in updateActions)
						action(item);
				}
			}
		}
		protected internal ReadOnlyCollection<RowDataBase> UnsortedItems { get; internal set; }
	}	
	public class PropertyWrapper : DependencyObject {
		public static object GetSourceBinding(DependencyObject obj) {
			return (object)obj.GetValue(SourceBindingProperty);
		}
		public static object GetTargetBinding(DependencyObject obj) {
			return (object)obj.GetValue(TargetBindingProperty);
		}
		public static void SetSourceBinding(DependencyObject obj, object value) {
			obj.SetValue(SourceBindingProperty, value);
		}
		public static void SetTargetBinding(DependencyObject obj, object value) {
			obj.SetValue(TargetBindingProperty, value);
		}
		public static readonly DependencyProperty TargetBindingProperty =
			DependencyProperty.RegisterAttached("TargetBinding", typeof(object), typeof(PropertyWrapper), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		public static readonly DependencyProperty SourceBindingProperty =
			DevExpress.Xpf.Utils.DependencyPropertyManager.RegisterAttached("SourceBinding", typeof(object), typeof(PropertyWrapper), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSourceBindingPropertyChanged)));
		protected static void OnSourceBindingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SetTargetBinding(d, e.NewValue);
		}
	}
	public class PGVirtualizingStackPanel : System.Windows.Controls.VirtualizingStackPanel {
		public static readonly DependencyProperty IsStandardPanelProperty;
		static readonly Action<VirtualizingStackPanel, bool> set_InRecyclingMode;
		static PGVirtualizingStackPanel() {
			IsStandardPanelProperty = DependencyPropertyManager.Register("IsStandardPanel", typeof(bool), typeof(PGVirtualizingStackPanel), new FrameworkPropertyMetadata(false, (d, e) => ((PGVirtualizingStackPanel)d).OnIsStandardPanelChanged((bool)e.OldValue)));
			set_InRecyclingMode = ReflectionHelper.CreateInstanceMethodHandler<VirtualizingStackPanel, Action<VirtualizingStackPanel, bool>>(null, "set_InRecyclingMode", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
		}
		public bool IsStandardPanel {
			get { return (bool)GetValue(IsStandardPanelProperty); }
			set { SetValue(IsStandardPanelProperty, value); }
		}
		public PGVirtualizingStackPanel() {
			CanHorizontallyScroll = false;
			SizeChanged += OnSizeChanged;
		}		
		void OnSizeChanged(object sender, SizeChangedEventArgs e) {
			UpdateViewportSize();
		}
		protected virtual void OnIsStandardPanelChanged(bool oldValue) {
			UpdateViewportSize();
		}
		public new void BringIndexIntoView(int index) {
			base.BringIndexIntoView(index);
		}
		private void UpdateViewportSize() {
			if (IsStandardPanel)
				PropertyGridHelper.SetViewportSize(this, RenderSize);
		}
		protected override void OnCleanUpVirtualizedItem(System.Windows.Controls.CleanUpVirtualizedItemEventArgs e) {
			RowControlBase rcb = e.UIElement as RowControlBase;
			if (rcb != null) {
				if (rcb.IsSelected)
					e.Cancel = true;
			}
			base.OnCleanUpVirtualizedItem(e);
		}
		protected override Size MeasureOverride(Size constraint) {
			ClearAutomationEventsHelper.ClearAutomationEvents();
			set_InRecyclingMode(this, ItemsControl.GetItemsOwner(this).Return(GetVirtualizationMode, () => VirtualizationMode.Standard) == VirtualizationMode.Recycling);
			return base.MeasureOverride(constraint);
		}
	}
	public class PrioritizedDataTemplateSelector : DataTemplateSelector {
		public DataTemplate DefaultTemplate { get; set; }
		public DataTemplateSelector DefaultTemplateSelector { get; set; }
		public DataTemplate CustomTemplate { get; set; }
		public DataTemplateSelector CustomTemplateSelector { get; set; }
		public override DataTemplate SelectTemplate(object item, DependencyObject container) {			
			if (CustomTemplate != null)
				return CustomTemplate;
			if(CustomTemplateSelector!=null)
				return CustomTemplateSelector.SelectTemplate(item, container);
			if(DefaultTemplate!=null)
				return DefaultTemplate;
			if(DefaultTemplateSelector!=null)
				return DefaultTemplateSelector.SelectTemplate(item, container);
			return base.SelectTemplate(item, container);
		}
	}
	public class PrioritizedStyleSelector : StyleSelector {
		public Style DefaultStyle { get; set; }
		public StyleSelector DefaultStyleSelector { get; set; }
		public Style CustomStyle { get; set; }
		public StyleSelector CustomStyleSelector { get; set; }
		public override Style SelectStyle(object item, DependencyObject container) {
			if(CustomStyle!=null)
				return CustomStyle;
			if(CustomStyleSelector!=null)
				return CustomStyleSelector.SelectStyle(item, container);
			if(DefaultStyle!=null)
				return DefaultStyle;
			if(DefaultStyleSelector!=null)
				return DefaultStyleSelector.SelectStyle(item, container);
			return base.SelectStyle(item, container);
		}
	}   
}
namespace DevExpress.Xpf.PropertyGrid.Internal {
	public class AggregateAction : IAggregateAction {
		Action<AggregateAction> action;
		public object Tag { get; set; }
		public AggregateAction(Action<AggregateAction> action, Func<AggregateAction, IAction, bool> canAggregateFunc) {
			this.action = action;
			this.CanAggregateDelegate = canAggregateFunc;
		}		
		protected Func<AggregateAction, IAction, bool> CanAggregateDelegate { get; set; }
		void IAction.Execute() {
			if (action != null)
				action(this);
		}
		bool IAggregateAction.CanAggregate(IAction action) {
			if (CanAggregateDelegate != null)
				return CanAggregateDelegate(this, action);
			return false;
		}
	}	
	public class FieldNameAction : AggregateAction {
		public FieldNameAction(Action<FieldNameAction> action, string fieldName)
			: base((param) => action((FieldNameAction)param), null) {
			FieldName = fieldName;
		}
		public string FieldName { get { return (string)Tag; } protected set { Tag = value; } }
		public Func<FieldNameAction, IAction, bool> CanAggregateFunc {
			get { return CanAggregateDelegate; }
			set { CanAggregateDelegate = new Func<AggregateAction, IAction, bool>((first, second) => value((FieldNameAction)first, second)); }
		}
	}
	public class DeferredAction : DevExpress.Xpf.Editors.IAction {
		Action action;
		Func<bool> canExecute;
		ImmediateActionsManager manager;
		public DeferredAction(Action action, Func<bool> canExecute, ImmediateActionsManager manager) {
			this.action = action;
			this.canExecute = canExecute ?? new Func<bool>(()=>true);
			this.manager = manager;
		}
		public void Execute() {
			if (!canExecute() && manager != null) {				
				manager.EnqueueAction(this);				
				return;
			}
			action();
		}
	}
}
