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

using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Controls.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
namespace DevExpress.Xpf.Controls {
	public class WizardControl : FooteredSelectorBase<WizardControl, WizardItem> {
		#region Properties
		public static readonly DependencyProperty IsButtonBackProperty = DependencyProperty.RegisterAttached("IsButtonBack", typeof(bool), typeof(WizardControl),
			new PropertyMetadata(false, (d, e) => OnIsWizardButtonChanged(d, (bool)e.OldValue, (bool)e.NewValue, WizardButtonKind.Back)));
		public static readonly DependencyProperty IsButtonNextProperty = DependencyProperty.RegisterAttached("IsButtonNext", typeof(bool), typeof(WizardControl),
			new PropertyMetadata(false, (d, e) => OnIsWizardButtonChanged(d, (bool)e.OldValue, (bool)e.NewValue, WizardButtonKind.Next)));
		public static readonly DependencyProperty IsButtonNavigateToProperty = DependencyProperty.RegisterAttached("IsButtonNavigateTo", typeof(string), typeof(WizardControl),
			new PropertyMetadata(string.Empty, (d, e) => OnIsWizardButtonChanged(d, !string.IsNullOrEmpty((string)e.OldValue), !string.IsNullOrEmpty((string)e.NewValue), WizardButtonKind.NavigateTo)));
		public static readonly DependencyProperty IsButtonCancelProperty = DependencyProperty.RegisterAttached("IsButtonCancel", typeof(bool), typeof(WizardControl),
			new PropertyMetadata(false, (d, e) => OnIsWizardButtonChanged(d, (bool)e.OldValue, (bool)e.NewValue, WizardButtonKind.Cancel)));
		public static readonly DependencyProperty IsButtonFinishProperty = DependencyProperty.RegisterAttached("IsButtonFinish", typeof(bool), typeof(WizardControl),
			new PropertyMetadata(false, (d, e) => OnIsWizardButtonChanged(d, (bool)e.OldValue, (bool)e.NewValue, WizardButtonKind.Finish)));
		public static bool GetIsButtonBack(DependencyObject obj) { return (bool)obj.GetValue(IsButtonBackProperty); }
		public static void SetIsButtonBack(DependencyObject obj, bool value) { obj.SetValue(IsButtonBackProperty, value); }
		public static bool GetIsButtonNext(DependencyObject obj) { return (bool)obj.GetValue(IsButtonNextProperty); }
		public static void SetIsButtonNext(DependencyObject obj, bool value) { obj.SetValue(IsButtonNextProperty, value); }
		public static string GetIsButtonNavigateTo(DependencyObject obj) { return (string)obj.GetValue(IsButtonNavigateToProperty); }
		public static void SetIsButtonNavigateTo(DependencyObject obj, string value) { obj.SetValue(IsButtonNavigateToProperty, value); }
		public static bool GetIsButtonCancel(DependencyObject obj) { return (bool)obj.GetValue(IsButtonCancelProperty); }
		public static void SetIsButtonCancel(DependencyObject obj, bool value) { obj.SetValue(IsButtonCancelProperty, value); }
		public static bool GetIsButtonFinish(DependencyObject obj) { return (bool)obj.GetValue(IsButtonFinishProperty); }
		public static void SetIsButtonFinish(DependencyObject obj, bool value) { obj.SetValue(IsButtonFinishProperty, value); }
		static void OnIsWizardButtonChanged(DependencyObject sender, bool oldValue, bool newValue, WizardButtonKind buttonType) {
			if(!newValue) {
				var b = Interaction.GetBehaviors(sender).OfType<WizardButtonBehavior>().ToList();
				b.ForEach(x => Interaction.GetBehaviors(sender).Remove(x));
			} else {
				WizardButtonBehavior b = new WizardButtonBehavior() { ButtonType = buttonType };
				Interaction.GetBehaviors(sender).Add(b);
			}
		}
		public static readonly DependencyProperty ContentCacheModeProperty = DependencyProperty.Register("ContentCacheMode", typeof(TabContentCacheMode), typeof(WizardControl), new PropertyMetadata(TabContentCacheMode.None));
		public static readonly DependencyProperty ContentHostTemplateProperty = DependencyProperty.Register("ContentHostTemplate", typeof(DataTemplate), typeof(WizardControl),
			new FrameworkPropertyMetadata(null, (d, e) => ((WizardControl)d).OnLogicalElementTemplateChanged(ContentHostProperty, ContentHostPropertyKey, (DataTemplate)e.NewValue)));
		static readonly DependencyPropertyKey ContentHostPropertyKey = DependencyProperty.RegisterReadOnly("ContentHost", typeof(object), typeof(WizardControl), null);
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		public static readonly DependencyProperty ContentHostProperty = ContentHostPropertyKey.DependencyProperty;
		public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent("SelectionChanged", RoutingStrategy.Bubble, typeof(SelectorSelectionChangedEventHandler), typeof(WizardControl));
		public static readonly RoutedEvent SelectionChangingEvent = EventManager.RegisterRoutedEvent("SelectionChanging", RoutingStrategy.Bubble, typeof(SelectorSelectionChangingEventHandler), typeof(WizardControl));
		public TabContentCacheMode ContentCacheMode { get { return (TabContentCacheMode)GetValue(ContentCacheModeProperty); } set { SetValue(ContentCacheModeProperty, value); } }
		public DataTemplate ContentHostTemplate { get { return (DataTemplate)GetValue(ContentHostTemplateProperty); } set { SetValue(ContentHostTemplateProperty, value); } }
		public event SelectorSelectionChangedEventHandler SelectionChanged { add { AddHandler(SelectionChangedEvent, value); } remove { RemoveHandler(SelectionChangedEvent, value); } }
		public event SelectorSelectionChangingEventHandler SelectionChanging { add { AddHandler(SelectionChangingEvent, value); } remove { RemoveHandler(SelectionChangingEvent, value); } }
		#endregion
		static WizardControl() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(WizardControl), new FrameworkPropertyMetadata(typeof(WizardControl)));
		}
		public WizardControl() {
		}
		protected override WizardItem CreateContainer() {
			return new WizardItem();
		}
		internal WizardService OwnerService { get; set; }
		public void Back() {
			if(!CanBack()) return;
			SelectPrevItem(); 
		}
		public bool CanBack() { return CanSelectPrevItem(); }
		public void Next() {
			if(!CanNext()) return;
			SelectNextItem(); 
		}
		public bool CanNext() { return CanSelectNextItem(); }
		public void Finish() {
			if(!CanFinish()) return;
			OwnerService.Do(x => x.SetResult(WizardResult.Finish));
			OwnerService.Do(x => Window.GetWindow(this).Close()); 
		}
		public bool CanFinish() { return true; }
		public void Cancel() {
			if(!CanCancel()) return;
			OwnerService.Do(x => x.SetResult(WizardResult.Cancel));
			OwnerService.Do(x => Window.GetWindow(this).Close()); 
		}
		public bool CanCancel() { return true; }
		public void NavigateTo(object id) {
			if(!CanNavigateTo(id)) return;
			var item = GetItemBasedOnId(id);
			SetCurrentValue(SelectedItemProperty, item);
		}
		public bool CanNavigateTo(object id) {
			var item = GetItemBasedOnId(id);
			if(item == null || SelectedItem == item) return false;
			return true;
		}
		object GetItemBasedOnId(object id) {
			if(Items.Contains(id)) return id;
			WizardItem container = null;
			if(id is WizardItem)
				container = (WizardItem)id;
			if(id is string)
				container = GetContainers().FirstOrDefault(x => x.Name == (string)id);
			if(container == null)
				container = GetContainers().FirstOrDefault(x => x.DataContext == id);
			if(container == null)
				container = GetContainers().FirstOrDefault(x => x.Content == id);
			if(container != null)
				return Items.Contains(container) ? container : ItemContainerGenerator.ItemFromContainer(container);
			return null;
		}
		protected override void RaiseSelectionChanged(int oldIndex, int newIndex, object oldItem, object newItem) {
			base.RaiseSelectionChanged(oldIndex, newIndex, oldItem, newItem);
			SelectorSelectionChangedEventArgs args = new SelectorSelectionChangedEventArgs(SelectionChangedEvent, this,
				new List<object>() { oldItem }, new List<int>() { oldIndex },
				new List<object>() { newItem }, new List<int>() { newIndex });
			RaiseEvent(args);
		}
		protected override void RaiseSelectionChanging(int oldIndex, int newIndex, object oldItem, object newItem, CancelEventArgs e) {
			base.RaiseSelectionChanging(oldIndex, newIndex, oldItem, newItem, e);
			SelectorSelectionChangingEventArgs args = new SelectorSelectionChangingEventArgs(SelectionChangingEvent, this,
				new List<object>() { oldItem }, new List<int>() { oldIndex },
				new List<object>() { newItem }, new List<int>() { newIndex });
			args.Cancel = e.Cancel;
			RaiseEvent(args);
			e.Cancel = args.Cancel;
		}
		protected override void OnItemsSourceChanged(System.Collections.IEnumerable oldValue, System.Collections.IEnumerable newValue) {
			base.OnItemsSourceChanged(oldValue, newValue);
		}
		protected override void OnItemContainerGeneratorStatusChanged(object sender, System.EventArgs e) {
			base.OnItemContainerGeneratorStatusChanged(sender, e);
		}
	}
	public class WizardItem : FooteredSelectorItemBase<WizardControl, WizardItem> {
		static WizardItem() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(WizardItem), new FrameworkPropertyMetadata(typeof(WizardItem)));
		}
	}
}
namespace DevExpress.Xpf.Controls.Native {
	public class WizardControlInvisiblePanel : Panel {
		protected override Size MeasureOverride(Size availableSize) {
			foreach(FrameworkElement child in Children)
				child.Measure(SizeHelper.Infinite);
			return new Size();
		}
		protected override Size ArrangeOverride(Size finalSize) {
			return finalSize;
		}
	}
}
