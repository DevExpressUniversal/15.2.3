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
using System.Collections;
using System.Collections.Specialized;
using DevExpress.Xpf.Utils;
using System.Windows.Media;
#if SILVERLIGHT
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
#endif
namespace DevExpress.Xpf.Core {
	public abstract class ItemsControlBase : Control, IWeakEventListener {
		static readonly ControlTemplate DefaultItemsPanel;
		public static readonly DependencyProperty ItemsSourceProperty;
		public static readonly DependencyProperty ItemsPanelProperty;
		public static readonly DependencyProperty ContainerStyleProperty;
		static readonly DependencyPropertyKey PanelPropertyKey;
		public static readonly DependencyProperty PanelProperty;
		public static readonly DependencyProperty ItemTemplateProperty;
		static ItemsControlBase() {
			DefaultItemsPanel = XamlHelper.GetControlTemplate("<StackPanel/>");
			Type ownerType = typeof(ItemsControlBase);
			ItemsSourceProperty = DependencyPropertyManager.Register("ItemsSource", typeof(IEnumerable), ownerType, new PropertyMetadata(null, (d, e) => ((ItemsControlBase)d).OnItemsSourceChanged(e)));
			ItemsPanelProperty = DependencyPropertyManager.Register("ItemsPanel", typeof(ControlTemplate), ownerType, new PropertyMetadata(DefaultItemsPanel, (d, e) => ((ItemsControlBase)d).OnItemsPanelChanged(), (d, baseValue) => baseValue ?? DefaultItemsPanel));
			ContainerStyleProperty = DependencyPropertyManager.Register("ContainerStyle", typeof(Style), ownerType, new PropertyMetadata(null, (d, e) => ((ItemsControlBase)d).InvalidateMeasure()));
			PanelPropertyKey = DependencyPropertyManager.RegisterReadOnly("Panel", typeof(Panel), ownerType, new PropertyMetadata(null));
			PanelProperty = PanelPropertyKey.DependencyProperty;
			ItemTemplateProperty = DependencyPropertyManager.Register("ItemTemplate", typeof(DataTemplate), ownerType, new PropertyMetadata(null, (d, e) => ((CachedItemsControl)d).OnItemTemplateChanged()));
		}
		public ItemsControlBase() {
			UpdateTemplate();
		}
		public DataTemplate ItemTemplate {
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}
		public IEnumerable ItemsSource {
			get { return (IEnumerable)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}
		public ControlTemplate ItemsPanel {
			get { return (ControlTemplate)GetValue(ItemsPanelProperty); }
			set { SetValue(ItemsPanelProperty, value); }
		}
		public Style ContainerStyle {
			get { return (Style)GetValue(ContainerStyleProperty); }
			set { SetValue(ContainerStyleProperty, value); }
		}
		protected virtual FrameworkElement CreateChild(object item) {
			return new ContentPresenter() {
				ContentTemplate = ItemTemplate,
				Content = item
			};
		}
		public Panel Panel {
			get { return (Panel)GetValue(PanelProperty); }
			internal set { this.SetValue(PanelPropertyKey, value); }
		}
		protected override Size MeasureOverride(Size constraint) {
			ValidateVisualTree();
			return base.MeasureOverride(constraint);
		}
		protected abstract void ValidateVisualTree();
		void OnItemsPanelChanged() {
			UpdateTemplate();
		}
		void UpdateTemplate() {
			Template = ItemsPanel;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			Panel = VisualTreeHelper.GetChildrenCount(this) > 0 ? VisualTreeHelper.GetChild(this, 0) as Panel : null;
		}
		protected virtual void OnItemsSourceChanged(DependencyPropertyChangedEventArgs e) {
			INotifyCollectionChanged oldValue = e.OldValue as INotifyCollectionChanged;
			INotifyCollectionChanged newValue = e.NewValue as INotifyCollectionChanged;
			if(oldValue != null)
				CollectionChangedEventManager.RemoveListener(oldValue, this);
			if(newValue != null)
				CollectionChangedEventManager.AddListener(newValue, this);
			OnCollectionChanged();
		}
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			if(managerType == typeof(CollectionChangedEventManager)) {
				OnCollectionChanged();
				return true;
			}
			return false;
		}
		protected virtual void OnCollectionChanged() {
			InvalidateMeasure();
			if(Panel != null)
				Panel.InvalidateMeasure();
		}
		protected virtual void OnItemTemplateChanged() { 
		}
	}
}
