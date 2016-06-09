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
using System.Windows.Media;
using System.Windows.Shapes;
using DevExpress.Xpf.Scheduler.Native;
using DevExpress.XtraScheduler;
using System.Collections.Specialized;
using DevExpress.Xpf.Utils;
using DevExpress.XtraScheduler.Drawing;
using DependencyPropertyHelper = DevExpress.Xpf.Core.Native.DependencyPropertyHelper;
#if SILVERLIGHT
using DevExpress.Xpf.Editors.Controls;
using DevExpress.Xpf.Core;
using Decorator =  DevExpress.Xpf.Core.WPFCompatibility.Decorator;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Scheduler.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
#else
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using DevExpress.XtraScheduler.Native;
#endif
namespace DevExpress.Xpf.Scheduler.Drawing {
	public interface ISupportSchedulerPanel {
		ISchedulerObservablePanel SchedulerPanel { get; }
		event EventHandler SchedulerPanelChanged;
	}
	#region ISchedulerItemsControlPropertiesCalculator
	public interface ISchedulerItemsControlAttachedPropertiesCalculator {
		void SetAttachedProperties(DependencyObject target, int index, int count);
	}
	#endregion
	#region SchedulerDayViewTimeCellAttachedPropertiesCalculator
	public class SchedulerDayViewTimeCellAttachedPropertiesCalculator : ISchedulerItemsControlAttachedPropertiesCalculator {
		#region ISchedulerItemsControlAttachedPropertiesCalculator Members
		public void SetAttachedProperties(DependencyObject target, int index, int count) {
			if(index == count - 1)
				SchedulerTimeCellControl.SetBottomBorderVisibility(target, Visibility.Collapsed);
		}
		#endregion
	}
	#endregion    
	#region SchedulerItemsControl
	public class SchedulerItemsControl : XPFItemsControl {
		#region ElementPosition
		public static readonly DependencyProperty ElementPositionProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterAttachedPropertyCore<SchedulerItemsControl, ElementPosition>("ElementPosition", ElementPosition.Standalone, FrameworkPropertyMetadataOptions.None, OnElementPositionPropertyChanged);
		public static void OnElementPositionPropertyChanged(DependencyObject element, DependencyPropertyChangedEventArgs e) {
			ISupportElementPosition elementPositionOwner = element as ISupportElementPosition;
			if (elementPositionOwner != null) {
				elementPositionOwner.OnRecalculatePostions((ElementPosition)e.OldValue, (ElementPosition)e.NewValue);
			}
		}
		public static ElementPosition GetElementPosition(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (ElementPosition)element.GetValue(ElementPositionProperty);
		}
		public static void SetElementPosition(DependencyObject element, ElementPosition value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(ElementPositionProperty, value);
		}
		#endregion
		Dictionary<object, DependencyObject> itemsDictionary = new Dictionary<object, DependencyObject>();
		protected IDictionary<object, DependencyObject> ItemsDictionary { get { return itemsDictionary; } }
		protected virtual bool ClearContainerForItemOverrideCore(DependencyObject element, object item) {
			if(item != null && element != item)
				ItemsDictionary.Remove(item);
			return false;
		}
		protected virtual bool PrepareContainerForItemOverrideCore(DependencyObject element, object item) {
			if(element != item)
				ItemsDictionary.Add(item, element);
			return false;
		}
		protected override void ClearContainerForItemOverride(DependencyObject element, object item) {
			if(!ClearContainerForItemOverrideCore(element, item))
				base.ClearContainerForItemOverride(element, item);
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			if(!PrepareContainerForItemOverrideCore(element, item))
				base.PrepareContainerForItemOverride(element, item);
		}
	}
	#endregion
	#region BaseContainerItemsControl
	public class BaseContainerItemsControl : SchedulerItemsControl {
	}
	#endregion
	public abstract class GenericContainerItemsControl<TItemContainer> : BaseContainerItemsControl where TItemContainer : XPFContentControl, new() {
		#region ValuePath
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		public static readonly DependencyProperty ValuePathProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<GenericContainerItemsControl<TItemContainer>, string>("ValuePath", String.Empty, (d, e) => d.OnValuePathChanged(e.OldValue, e.NewValue));
		public string ValuePath { get { return (string)GetValue(ValuePathProperty); } set { SetValue(ValuePathProperty, value); } }
		#endregion
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			TItemContainer container = element as TItemContainer;
			if(container == null) {
				base.PrepareContainerForItemOverride(element, item);
				return;
			}
			else {
				SetContentBinding(container, item);
				if(ItemTemplate != null)
					container.ContentTemplate = ItemTemplate;
				if(ItemTemplateSelector != null)
					container.ContentTemplateSelector = ItemTemplateSelector;
#if SL
						if(ItemContainerStyle != null)
							container.Style = ItemContainerStyle;
#endif
				ItemsDictionary.Add(item, container);
			}
		}
		protected virtual void SetContentBinding(TItemContainer container, object item) {
			string valuePath = ValuePath;
			if(String.IsNullOrEmpty(valuePath)) {
				container.Content = item;
				return;
			}
			Binding binding = new Binding(valuePath);
			binding.Source = item;
			binding.Mode = BindingMode.OneWay;
			binding.BindsDirectlyToSource = true;
			container.SetBinding(ContentControl.ContentProperty, binding);
		}
		protected override DependencyObject GetContainerForItemOverride() {
			return new TItemContainer();
		}
		protected virtual void OnValuePathChanged(string oldPath, string newPath) {
			foreach(KeyValuePair<object, DependencyObject> item in ItemsDictionary) { 
				TItemContainer container = item.Value as TItemContainer;
				if(container != null)
					SetContentBinding(container, item.Key);
			}
		}
	}
	public class NavigationButtonPairControl : SchedulerItemsControl, ISupportElementPosition {
		#region ShowBorder
		public bool ShowBorder {
			get { return (bool)GetValue(ShowBorderProperty); }
			set { SetValue(ShowBorderProperty, value); }
		}
		public static readonly DependencyProperty ShowBorderProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<NavigationButtonPairControl, bool>("ShowBorder", false, (d, e) => d.OnShowBorderChanged(e.OldValue, e.NewValue), null);
		void OnShowBorderChanged(bool oldValue, bool newValue) {
			UpdateMargin(newValue);
		}
		#endregion
		#region PairMargin
		public Thickness PairMargin {
			get { return (Thickness)GetValue(PairMarginProperty); }
			set { SetValue(PairMarginProperty, value); }
		}
		public static readonly DependencyProperty PairMarginProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<NavigationButtonPairControl, Thickness>("PairMargin", new Thickness(0), (d, e) => d.OnPairMarginChanged(e.OldValue, e.NewValue), null);
		void OnPairMarginChanged(Thickness oldValue, Thickness newValue) {
			UpdateMargin(ShowBorder);
		}
		#endregion
		#region BorderMargin
		public Thickness VisibleBorderMargin {
			get { return (Thickness)GetValue(VisibleBorderMarginProperty); }
			set { SetValue(VisibleBorderMarginProperty, value); }
		}
		public static readonly DependencyProperty VisibleBorderMarginProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<NavigationButtonPairControl, Thickness>("VisibleBorderMargin", new Thickness(0), (d, e) => d.OnVisibleBorderMarginChanged(e.OldValue, e.NewValue), null);
		void OnVisibleBorderMarginChanged(Thickness oldValue, Thickness newValue) {
			UpdateMargin(ShowBorder);
		}
		#endregion
		#region InvisibleBorderMargin
		public Thickness InvisibleBorderMargin {
			get { return (Thickness)GetValue(InvisibleBorderMarginProperty); }
			set { SetValue(InvisibleBorderMarginProperty, value); }
		}
		public static readonly DependencyProperty InvisibleBorderMarginProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<NavigationButtonPairControl, Thickness>("InvisibleBorderMargin", new Thickness(0), (d, e) => d.OnInvisibleBorderMarginChanged(e.OldValue, e.NewValue), null);
		void OnInvisibleBorderMarginChanged(Thickness oldValue, Thickness newValue) {
			UpdateMargin(ShowBorder);
		}
		#endregion
		public NavigationButtonPairControl() {
		}
		public void SubscribeEvent(PixelSnappedUniformGrid pixelSnappedUniformGrid) {
			pixelSnappedUniformGrid.ElementPositionRecalculated += new EventHandler(OnElementPositionRecalculated);
		}
		public void UnsubscribeEvent(PixelSnappedUniformGrid param1) {
			param1.ElementPositionRecalculated -= new EventHandler(OnElementPositionRecalculated);
		}
		void UpdateMargin(bool showBorder) {
			foreach(DependencyObject item in ItemsDictionary.Values) {
				FrameworkElement element = item as FrameworkElement;
				if(element == null)
					continue;
				AssignMargin(element);
			}
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			base.PrepareContainerForItemOverride(element, item);
			FrameworkElement frameworkElemnt = element as FrameworkElement;
			if(frameworkElemnt == null)
				return;
			AssignMargin(frameworkElemnt);
		}
		void AssignMargin(FrameworkElement frameworkElemnt) {
			Thickness actualMargin = PairMargin;
			Thickness borderMargin = (ShowBorder) ? VisibleBorderMargin : InvisibleBorderMargin;
			ElementPosition elementPosition = SchedulerItemsControl.GetElementPosition(frameworkElemnt) ?? ElementPosition.Standalone;
			if(elementPosition.IsLeft)
				actualMargin.Left += borderMargin.Left;
			if(elementPosition.IsRight)
				actualMargin.Right += borderMargin.Right;
			frameworkElemnt.Margin = actualMargin;
		}
		void OnElementPositionRecalculated(object sender, EventArgs e) {
			UpdateMargin(ShowBorder);
		}
		#region ISupportElementPosition Members
		public void OnRecalculatePostions(ElementPosition oldValue, ElementPosition newValue) {
			UpdateMargin(ShowBorder);
		}
		#endregion
	}
}
