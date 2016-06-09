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
using DevExpress.Utils;
using DevExpress.Xpf.Scheduler.Internal;
using DevExpress.Xpf.Core.Native;
#if SL
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using DevExpress.Xpf.Scheduler.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Scheduler.Drawing {
	#region ScrollSynchronizer
	public class ScrollSynchronizer : DependencyObject {
		public static readonly DependencyProperty ScrollGroupProperty;
		static ScrollSynchronizer() {
			ScrollGroupProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterAttachedPropertyCore<ScrollSynchronizer, object>("ScrollGroup", null, FrameworkPropertyMetadataOptions.None, OnScrollGroupChanged);
		}
		#region ScrollGroup
		public static void SetScrollGroup(DependencyObject obj, object scrollGroup) {
			obj.SetValue(ScrollGroupProperty, scrollGroup);
		}
		public static object GetScrollGroup(DependencyObject obj) {
			return obj.GetValue(ScrollGroupProperty);
		}
		static void OnScrollGroupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ScrollViewer scrollViewer = d as ScrollViewer;
			if (scrollViewer != null) {
				if (e.OldValue != null) {
					if (scrollViewers.ContainsKey(scrollViewer)) {
						ScrollViewerSubscriber.Unsubscribe(scrollViewer, ScrollViewer_ScrollChanged);
						scrollViewers.Remove(scrollViewer);
					}
				}
				object newValue = e.NewValue;
				if (newValue != null) {
					if (horizontalScrollOffsets.ContainsKey(newValue))
						scrollViewer.ScrollToHorizontalOffset(horizontalScrollOffsets[newValue]);
					else
						horizontalScrollOffsets.Add(newValue, scrollViewer.HorizontalOffset);
					if (verticalScrollOffsets.ContainsKey(newValue))
						scrollViewer.ScrollToVerticalOffset(verticalScrollOffsets[newValue]);
					else
						verticalScrollOffsets.Add(newValue, scrollViewer.VerticalOffset);
					scrollViewers.Add(scrollViewer, newValue);
					ScrollViewerSubscriber.Subscribe(scrollViewer, ScrollViewer_ScrollChanged);
				}
			}
		}
		#endregion
		static WeakKeyDictionary<ScrollViewer, object> scrollViewers = new WeakKeyDictionary<ScrollViewer, object>();
		static Dictionary<object, double> horizontalScrollOffsets = new Dictionary<object, double>();
		static Dictionary<object, double> verticalScrollOffsets = new Dictionary<object, double>();
		static void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e) {
			ScrollViewer changedScrollViewer = sender as ScrollViewer;
			if (changedScrollViewer == null)
				return;
			if (e.VerticalChange != 0 || e.HorizontalChange != 0)
				Scroll(changedScrollViewer);
		}
		static void Scroll(ScrollViewer changedScrollViewer) {
			object group = scrollViewers[changedScrollViewer];
			verticalScrollOffsets[group] = changedScrollViewer.VerticalOffset;
			horizontalScrollOffsets[group] = changedScrollViewer.HorizontalOffset;
			foreach (WeakReference key in scrollViewers.Keys) {
				ScrollViewer scrollViewer = key.Target as ScrollViewer;
				if (scrollViewer != null) {
					if (scrollViewers[scrollViewer] == group && scrollViewer != changedScrollViewer) {
						if (scrollViewer.VerticalOffset != changedScrollViewer.VerticalOffset)
							scrollViewer.ScrollToVerticalOffset(changedScrollViewer.VerticalOffset);
						if (scrollViewer.HorizontalOffset != changedScrollViewer.HorizontalOffset)
							scrollViewer.ScrollToHorizontalOffset(changedScrollViewer.HorizontalOffset);
					}
				}
			}
		}
	}
	#endregion
	public class GroupKey : DependencyObject {
		#region GroupName
		public string GroupName {
			get { return (string)GetValue(GroupNameProperty); }
			set { SetValue(GroupNameProperty, value); }
		}
		public static readonly DependencyProperty GroupNameProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<GroupKey, string>("GroupName", String.Empty);
		#endregion
	}
	public class ScrollViewerSynchronizer : Decorator {
		class Offset {
			public Offset(double horizontalOffset, double verticalOffset) {
				HorizontalOffset = horizontalOffset;
				VerticalOffset = verticalOffset;
			}
			public Offset() {
			}
			public double HorizontalOffset { get; set; }
			public double VerticalOffset { get; set; }
			public bool IsEqual(double horizontal, double vertical) {
				return DoubleUtil.AreClose(horizontal, HorizontalOffset) && DoubleUtil.AreClose(vertical, VerticalOffset);
			}
			public bool IsEqual(Offset offset) {
				return IsEqual(offset.HorizontalOffset, offset.VerticalOffset);
			}
			public void Assign(double horizontalOffset, double verticalOffset) {
				HorizontalOffset = horizontalOffset;
				VerticalOffset = verticalOffset;
			}
			public void Assign(Offset offset) {
				Assign(offset.HorizontalOffset, offset.VerticalOffset);
			}
			public override string ToString() {
				return String.Format("({0}, {1})", HorizontalOffset, VerticalOffset);
			}
		}
		#region IsSynchronize
		public static readonly DependencyProperty IsSynchronizeProperty = CreateIsSynchronizeProperty();
		static DependencyProperty CreateIsSynchronizeProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterAttachedPropertyCore<ScrollViewerSynchronizer, bool>("IsSynchronize", false, FrameworkPropertyMetadataOptions.None, OnIsSynchronizePropertyChanged);
		}
		static void OnIsSynchronizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ScrollViewer scrollViewer = d as ScrollViewer;
			if (scrollViewer == null)
				return;
			bool oldValue = (bool)e.OldValue;
			bool newValue = (bool)e.NewValue;
			scrollViewer.Dispatcher.BeginInvoke(new Action(() => {
				ScrollViewerSynchronizer synchronizer = LayoutHelper.FindParentObject<ScrollViewerSynchronizer>(scrollViewer);
				if (synchronizer == null)
					return;
				synchronizer.OnIsSynchronizePropertyChanged(scrollViewer, oldValue, newValue);
			}));
		}
		public static void SetIsSynchronize(DependencyObject obj, bool value) {
			obj.SetValue(IsSynchronizeProperty, value);
		}
		public static bool GetIsSynchronize(DependencyObject obj) {
			return (bool)obj.GetValue(IsSynchronizeProperty);
		}
		#endregion
		#region Fields
		List<WeakReference> scrollViewers = new List<WeakReference>();
		bool inMeasure;
		bool isSynchronized = true;
		Offset offset = new Offset();
		#endregion
		public ScrollViewerSynchronizer() {
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
		}
		void ForEachAliveScrollViewer(Action<ScrollViewer> action) {
			for (int i = scrollViewers.Count - 1; i >= 0; i--) {
				ScrollViewer scrollViewer = scrollViewers[i].Target as ScrollViewer;
				if (scrollViewer != null)
					action(scrollViewer);
				else
					scrollViewers.RemoveAt(i);
			}
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			ForEachAliveScrollViewer(SubscribeToScrollChanged);
		}
		void OnUnloaded(object sender, RoutedEventArgs e) {
			ForEachAliveScrollViewer(UnsubscribeToScrollChanged);
		}
		void SubscribeToScrollChanged(ScrollViewer scrollViewer) {
			ScrollViewerSubscriber.Unsubscribe(scrollViewer, OnScrollChanged);
			ScrollViewerSubscriber.Subscribe(scrollViewer, OnScrollChanged);
		}
		void UnsubscribeToScrollChanged(ScrollViewer scrollViewer) {
			ScrollViewerSubscriber.Unsubscribe(scrollViewer, OnScrollChanged);
		}
		double ScrollToVerticalOffset(ScrollViewer scrollViewer, double offset) {
			if (DoubleUtil.GreaterThanOrClose(offset, 0) && offset > scrollViewer.VerticalOffset) {
				scrollViewer.ScrollToVerticalOffset(offset);
				return offset;
			}
			else
				return scrollViewer.VerticalOffset;
		}
		double ScrollToHorizontalOffset(ScrollViewer scrollViewer, double offset) {
			if (DoubleUtil.GreaterThanOrClose(offset, 0) && offset > scrollViewer.HorizontalOffset) {
				scrollViewer.ScrollToHorizontalOffset(offset);
				return offset;
			}
			else
				return scrollViewer.HorizontalOffset;
		}
		void OnIsSynchronizePropertyChanged(ScrollViewer scrollViewer, bool oldValue, bool newValue) {
			if (!oldValue)
				RemoveScrollViewer(scrollViewer);
			if (newValue) {
				offset.HorizontalOffset = ScrollToHorizontalOffset(scrollViewer, offset.HorizontalOffset);
				offset.VerticalOffset = ScrollToVerticalOffset(scrollViewer, offset.VerticalOffset);
				Synchronize();
				AddScrollViewer(scrollViewer);
			}
		}
		public virtual void AddScrollViewer(ScrollViewer scrollViewer) {
			int index = IndexOf(scrollViewer);
			if (index < 0) {
				scrollViewers.Add(new WeakReference(scrollViewer));
				SubscribeToScrollChanged(scrollViewer);
			}
		}
		public virtual void RemoveScrollViewer(ScrollViewer scrollViewer) {
			int index = IndexOf(scrollViewer);
			if (index >= 0) {
				scrollViewers.RemoveAt(index);
				UnsubscribeToScrollChanged(scrollViewer);
			}
		}
		protected internal virtual int IndexOf(ScrollViewer scrollViewer) {
			int count = scrollViewers.Count;
			for (int index = 0; index < count; index++) {
				ScrollViewer target = scrollViewers[index].Target as ScrollViewer;
				if (Object.ReferenceEquals(target, scrollViewer))
					return index;
			}
			return -1;
		}
		void OnScrollChanged(object sender, ScrollChangedEventArgs e) {
			ScrollViewer scrollViewer = sender as ScrollViewer;
			if (scrollViewer == null)
				return;
			Offset newOffset = new Offset(scrollViewer.HorizontalOffset, scrollViewer.VerticalOffset);
			if (offset.IsEqual(newOffset))
				return;
			offset.Assign(newOffset);
			if (!this.inMeasure)
				InvalidateMeasure();
			this.isSynchronized = false;
		}
		void Synchronize() {
			ForEachAliveScrollViewer(Synchronize);
		}
		void Synchronize(ScrollViewer scrollViewer) {
			if (!DoubleUtil.AreClose(scrollViewer.VerticalOffset, offset.VerticalOffset))
				scrollViewer.ScrollToVerticalOffset(offset.VerticalOffset);
			if (!DoubleUtil.AreClose(scrollViewer.HorizontalOffset, offset.HorizontalOffset))
				scrollViewer.ScrollToHorizontalOffset(offset.HorizontalOffset);
		}
		protected override Size MeasureOverride(Size availableSize) {
			this.inMeasure = true;
			Size result = base.MeasureOverride(availableSize);
			if (!this.isSynchronized) {
				Synchronize();
				this.isSynchronized = true;
			}
			this.inMeasure = false;
			return result;
		}
	}
}
