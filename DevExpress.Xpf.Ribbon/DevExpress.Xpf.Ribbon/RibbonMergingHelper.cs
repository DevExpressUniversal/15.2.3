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
using System.Linq;
using System.Text;
using System.Windows;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Bars;
namespace DevExpress.Xpf.Ribbon {
	public static class RibbonMergingHelper {
		#region Dependency Properties
		public static readonly DependencyProperty MergeWithProperty =
			DependencyProperty.RegisterAttached("MergeWith", typeof(RibbonControl), typeof(RibbonMergingHelper), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits,
				OnMergeWithChanged));
		public static readonly DependencyProperty MergeStatusBarWithProperty =
			DependencyProperty.RegisterAttached("MergeStatusBarWith", typeof(RibbonStatusBarControl), typeof(RibbonMergingHelper), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits,
				OnMergeStatusBarWithChanged));
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty MergingHelperInternalBaseProperty =
			DependencyProperty.RegisterAttached("MergingHelperInternalBase", typeof(MergingHelperInternalBase), typeof(RibbonMergingHelper), new PropertyMetadata(null));
		#endregion
		public static RibbonControl GetMergeWith(FrameworkElement d) { return (RibbonControl)d.GetValue(MergeWithProperty); }
		public static void SetMergeWith(FrameworkElement d, RibbonControl parentRibbon) { d.SetValue(MergeWithProperty, parentRibbon); }
		public static RibbonStatusBarControl GetMergeStatusBarWith(FrameworkElement d) { return (RibbonStatusBarControl)d.GetValue(MergeStatusBarWithProperty); }
		public static void SetMergeStatusBarWith(FrameworkElement d, RibbonStatusBarControl parentStatusBar) { d.SetValue(MergeStatusBarWithProperty, parentStatusBar); }
		static MergingHelperInternalBase GetMergingHelperInternalBase(FrameworkElement d) { return (MergingHelperInternalBase)d.GetValue(MergingHelperInternalBaseProperty); }
		static void SetMergingHelperInternalBase(FrameworkElement d, MergingHelperInternalBase h) { d.SetValue(MergingHelperInternalBaseProperty, h); }
		static MergingHelperInternal<TControl> GetMergingHelperInternal<TControl>(TControl control, Func<MergingHelperInternal<TControl>> createFunc) where TControl : FrameworkElement {
			var data = (MergingHelperInternal<TControl>)GetMergingHelperInternalBase(control);
			if(data == null) {
				data = createFunc();
				SetMergingHelperInternalBase(control, data);
			}
			return data;
		}
		static void OnMergeWithChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			RibbonControl ribbon = d as RibbonControl;
			if(ribbon == null) return;
			GetMergingHelperInternal(ribbon, () => new RibbonMergingHelperInternal(ribbon)).OnMergeWithChanged(ribbon, e);
		}
		static void OnMergeStatusBarWithChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			RibbonStatusBarControl statusBar = d as RibbonStatusBarControl;
			if(statusBar == null) return;
			GetMergingHelperInternal(statusBar, () => new StatusBarMergingHelperInternal(statusBar)).OnMergeWithChanged(statusBar, e);
		}
		class RibbonMergingHelperInternal : MergingHelperInternal<RibbonControl> {
			public RibbonMergingHelperInternal(RibbonControl ribbon) : base(ribbon) { }
			protected override void Merge(RibbonControl parent, RibbonControl child) {
				parent.Merge(child);
			}
			protected override void Unmerge(RibbonControl child) {
				child.MergedParent.Do(p => p.UnMerge(child));
			}
			protected override RibbonControl GetParent(RibbonControl child) {
				return RibbonMergingHelper.GetMergeWith(child);
			}
			protected override RibbonControl GetMergedParent(RibbonControl child) {
				return child.MergedParent;
			}
			protected override MDIMergeStyle GetMergeStyle(RibbonControl child) {
				return child.MDIMergeStyle;
			}
		}
		class StatusBarMergingHelperInternal : MergingHelperInternal<RibbonStatusBarControl> {
			public StatusBarMergingHelperInternal(RibbonStatusBarControl statusBar) : base(statusBar) { }
			protected override void Merge(RibbonStatusBarControl parent, RibbonStatusBarControl child) {
				parent.Merge(child);
			}
			protected override void Unmerge(RibbonStatusBarControl child) {
				child.MergedParent.Do(p => p.UnMerge(child));
			}
			protected override RibbonStatusBarControl GetParent(RibbonStatusBarControl child) {
				return RibbonMergingHelper.GetMergeStatusBarWith(child);
			}
			protected override RibbonStatusBarControl GetMergedParent(RibbonStatusBarControl child) {
				return child.MergedParent;
			}
			protected override MDIMergeStyle GetMergeStyle(RibbonStatusBarControl child) {
				return child.MDIMergeStyle;
			}
		}
		abstract class MergingHelperInternalBase { }
		abstract class MergingHelperInternal<T> : MergingHelperInternalBase where T : FrameworkElement {
			T control;
			public MergingHelperInternal(T control) {
				this.control = control;
			}
			abstract protected void Merge(T parent, T child);
			abstract protected void Unmerge(T child);
			abstract protected T GetParent(T child);
			abstract protected T GetMergedParent(T child);
			abstract protected MDIMergeStyle GetMergeStyle(T child);
			public void OnMergeWithChanged(T control, DependencyPropertyChangedEventArgs e) {
				if(DesignerProperties.GetIsInDesignMode(control)) return;
				if(GetMergeStyle(control) == MDIMergeStyle.Never) return;
				control.Visibility = Visibility.Collapsed;
				T oldValue = (T)e.OldValue;
				T newValue = (T)e.NewValue;
				if(oldValue != null) {
					control.Loaded -= OnRibbonLoaded;
					control.Unloaded -= OnRibbonUnloaded;
					OnRibbonUnloadedCore(control, oldValue);
					UnsubscribeRibbonVisibility();
				}
				if(newValue != null) {
					control.Loaded += OnRibbonLoaded;
					control.Unloaded += OnRibbonUnloaded;
					if(control.IsLoaded)
						OnRibbonLoaded(control, null);
					SubscribeRibbonVisibilityIfNeeded();
				}
			}
			bool IsRibbonVisible() {
				if(control.Parent is FrameworkElement)
					return ((FrameworkElement)control.Parent).IsVisible;
				return true;
			}
			void UnsubscribeRibbonVisibility() {
				if(control.Parent is FrameworkElement) {
					((FrameworkElement)control.Parent).IsVisibleChanged -= OnRibbonIsVisibleChanged;
				}
			}
			void SubscribeRibbonVisibilityIfNeeded() {
				if(GetMergeStyle(control) == MDIMergeStyle.Default || GetMergeStyle(control) == MDIMergeStyle.WhenChildActivated) {
					if(control.Parent is FrameworkElement) {
						UnsubscribeRibbonVisibility();
						((FrameworkElement)control.Parent).IsVisibleChanged += OnRibbonIsVisibleChanged;
					}
				}
			}
			void OnRibbonIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
				if(!(bool)e.NewValue) 
					Unmerge(control);
				else
					Merge(GetParent(control), control);
			}
			void OnRibbonLoaded(object sender, RoutedEventArgs e) {
				T ribbon = (T)sender;
				T parent = GetParent(control);
				parent.Loaded += OnParentLoaded;
				parent.Unloaded += OnParentUnloaded;
				if(parent.IsLoaded)
					OnParentLoaded(parent, null);
				SubscribeRibbonVisibilityIfNeeded();
			}
			void OnRibbonUnloaded(object sender, RoutedEventArgs e) {
				T ribbon = (T)sender;
				OnRibbonUnloadedCore(ribbon, GetParent(control));
			}
			void OnRibbonUnloadedCore(T ribbon, T parent) {
				parent.Loaded -= OnParentLoaded;
				parent.Unloaded -= OnParentUnloaded;
				OnParentUnloaded(parent, null);
			}
			void OnParentLoaded(object sender, RoutedEventArgs e) {
				T parent = (T)sender;
				Unmerge(control);
				if(GetMergeStyle(control) == MDIMergeStyle.Default || GetMergeStyle(control) == MDIMergeStyle.WhenChildActivated) {
					if(IsRibbonVisible())
						Merge(parent, control);
				} else Merge(parent, control);
			}
			void OnParentUnloaded(object sender, RoutedEventArgs e) {
				T parent = (T)sender;
				if(GetMergedParent(control) == parent) {
					Unmerge(control);
				}
			}
		}
	}
}
