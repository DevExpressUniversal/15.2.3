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
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using DevExpress.Xpf.Core;
using System.Windows.Media;
using System.Windows.Data;
using DevExpress.XtraScheduler;
using DependencyPropertyHelper = DevExpress.Xpf.Core.Native.DependencyPropertyHelper;
using DevExpress.Xpf.Core.Native;
using System.Windows.Input;
using DevExpress.Utils;
using System.Collections.ObjectModel;
using DevExpress.XtraScheduler.Native;
using DevExpress.Xpf.Scheduler.Internal;
using System.ComponentModel;
using DevExpress.Xpf.Scheduler.Native;
using DevExpress.XtraScheduler.Internal.Diagnostics;
#if SL
using Visual = System.Windows.UIElement;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Scheduler.WPFCompatibility;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
#endif
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class SharedGroupSizeContainer : Decorator {
		#region SharedGroupStates
		public static SharedGroupStateCollection GetSharedGroupStates(DependencyObject d) {
			return (SharedGroupStateCollection)d.GetValue(SharedGroupStatesProperty);
		}
#if SL
		public static SharedGroupStateCollection GetSharedGroupStatesRouted(DependencyObject d) {
			SharedGroupStateCollection result = null;
			DependencyObject current = d;
			while(current != null && !(current is SchedulerControl)) {
				result = current.GetValue(SharedGroupStatesProperty) as SharedGroupStateCollection;
				if(result != null)
					break;
				current = VisualTreeHelper.GetParent(current);
			}			
			return result;
		}
#endif
		public static void SetSharedGroupStates(DependencyObject d, SharedGroupStateCollection value) {
			SchedulerLogger.Trace(XpfLoggerTraceLevel.SharedSizePanel, "->Attach SharedGroupStates property to {0}-({1})", VisualElementHelper.GetElementName(d), VisualElementHelper.GetTypeName(d));
			d.SetValue(SharedGroupStatesProperty, value);
		}
		public static readonly DependencyProperty SharedGroupStatesProperty = CreateSharedGroupStatesProperty();
		static DependencyProperty CreateSharedGroupStatesProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterAttachedPropertyCore<SharedGroupSizeContainer, SharedGroupStateCollection>("SharedGroupStates", null,
				FrameworkPropertyMetadataOptions.Inherits, SharedGroupStatesPropertyChanged);
		}
		static void SharedGroupStatesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SchedulerLogger.Trace(XpfLoggerTraceLevel.SharedSizePanel, "->Attach SharedGroupStates property to {0}-({1})", VisualElementHelper.GetElementName(d), VisualElementHelper.GetTypeName(d));
			SharedSizePanel panel = d as SharedSizePanel;
			if (panel != null)
				panel.OnSharedGroupStatesChanged((SharedGroupStateCollection)e.OldValue, (SharedGroupStateCollection)e.NewValue);
		}
		#endregion
		#region MeasuredSize
		public static readonly DependencyProperty MeasuredSizeProperty = CreateMeasuredSizeProperty();
		static DependencyProperty CreateMeasuredSizeProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SharedGroupSizeContainer, Size>("MeasuredSize", Size.Empty);
		}
		Size lastMeasuredSize = Size.Empty;
		public Size MeasuredSize {
			get { return (Size)GetValue(MeasuredSizeProperty); }
			set {
				if (value == lastMeasuredSize)
					return;
				lastMeasuredSize = value;
				SetValue(MeasuredSizeProperty, value);
			}
		}
		#endregion
		public SharedGroupSizeContainer() {
			SharedGroupStateCollection collection = new SharedGroupStateCollection(this);
			SetSharedGroupStates(this, collection);
		}
		void OnSharedGroupStateChanged(object sender, EventArgs e) {			
			InvalidateMeasure();
		}
		bool isInsideMeasure;
		bool remeasureRequared;
		protected internal virtual void OnInvalidateInnerSharedSizeGroup() {
			if (!isInsideMeasure)
				InvalidateMeasure();
			else
				remeasureRequared = true;
		}
		protected override Size MeasureOverride(Size constraint) {
			try {
				Size result;
				isInsideMeasure = true;
				do {
					remeasureRequared = false;
					result = base.MeasureOverride(constraint);
				} while (remeasureRequared);
				MeasuredSize = result;
				return result;
			}
			finally {
				isInsideMeasure = false;
			}
		}
		protected override Size ArrangeOverride(Size arrangeSize) {
			GetSharedGroupStates(this).InvalidateArrange();
			Size result = base.ArrangeOverride(arrangeSize);
			return result;
		}
	}
	public class SharedSizePanel : SchedulerPanelBase {
		#region SharedSizeGroup
		public string SharedSizeGroup {
			get { return (string)GetValue(SharedSizeGroupProperty); }
			set { SetValue(SharedSizeGroupProperty, value); }
		}
		public static readonly DependencyProperty SharedSizeGroupProperty = CreateSharedSizeGroupProperty();
		static DependencyProperty CreateSharedSizeGroupProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SharedSizePanel, string>("SharedSizeGroup", String.Empty,
				FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure, (d, e) => d.OnSharedGroupKeyChanged(e.OldValue, e.NewValue, d.Orientation, d.Orientation), null);
		}		
		void OnSharedGroupKeyChanged(string oldName, string newName, Orientation oldOrientation, Orientation newOrientation) {
			SchedulerLogger.TraceOpenGroup(XpfLoggerTraceLevel.SharedSizePanel, "->SharedSizePanel.SharedGroupKeyChanged: {0}-({1}), old=({2},{3}), new=({4},{5})", VisualElementHelper.GetElementName(this), VisualElementHelper.GetTypeName(this), oldName, oldOrientation, newName, newOrientation);
			if (oldName != newName || oldOrientation != newOrientation) {
				SharedGroupStateCollection collection = SharedGroupStateCollection.GetCollection(this);
				if(collection == null)
					return;
				SharedGroupState oldState = collection.GetGroup(oldName, oldOrientation);
				if (oldState != null) {
					oldState.RemoveSharedSizePanel(this);
					groupState = null;
				}
				SharedGroupState newState = collection.GetGroup(newName, newOrientation);
				if (newState != null) {
					newState.AddSharedSizePanel(this);
					groupState = newState;
				}
			}
			SchedulerLogger.TraceCloseGroup(XpfLoggerTraceLevel.SharedSizePanel);
		}
		#endregion
		#region IsBaseSizePanel
		public bool IsBaseSizePanel {
			get { return (bool)GetValue(IsBaseSizePanelProperty); }
			set { SetValue(IsBaseSizePanelProperty, value); }
		}
		public static readonly DependencyProperty IsBaseSizePanelProperty = CreateIsBaseSizePanelProperty();
		static DependencyProperty CreateIsBaseSizePanelProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SharedSizePanel, bool>("IsBaseSizePanel", true, null);
		}
		#endregion
		#region Orientation
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		public static readonly DependencyProperty OrientationProperty = CreateOrientationProperty();
		static DependencyProperty CreateOrientationProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SharedSizePanel, Orientation>("Orientation", Orientation.Vertical,
				FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure, (d, e) => d.OnSharedGroupKeyChanged(d.SharedSizeGroup, d.SharedSizeGroup, e.OldValue, e.NewValue), null);
		}
		#endregion
		#region Span
		public static int GetSpan(DependencyObject d) {
			return (int)d.GetValue(SpanProperty);
		}
		public static void SetSpan(DependencyObject d, int value) {
			d.SetValue(SpanProperty, value);
		}
		public static readonly DependencyProperty SpanProperty = CreateSpanProperty();
		static DependencyProperty CreateSpanProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterAttachedProperty<SharedSizePanel, int>("Span", 0, FrameworkPropertyMetadataOptions.None, null);
		}
		#endregion
		#region NotificationElement
		public FrameworkElement NotificationElement {
			get { return (FrameworkElement)GetValue(NotificationElementProperty); }
			set { SetValue(NotificationElementProperty, value); }
		}
		public static readonly DependencyProperty NotificationElementProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SharedSizePanel, FrameworkElement>("NotificationElement", null);
		#endregion
		SharedGroupState groupState;
		public SharedSizePanel() {
			LayoutUpdated += OnLayoutUpdated;
#if SL
			Unloaded += SharedSizePanel_Unloaded;
			Loaded += SharedSizePanel_Loaded;
#endif
		}
#if SL
		void SharedSizePanel_Unloaded(object sender, RoutedEventArgs e) {
			SharedGroupState sharedGroupState = GetSharedGroupState();
			if (sharedGroupState != null)
				sharedGroupState.RemoveSharedSizePanel(this);
		}
		void SharedSizePanel_Loaded(object sender, RoutedEventArgs e) {
			SharedGroupState sharedGroupState = GetSharedGroupState();
			if (sharedGroupState != null)
				sharedGroupState.AddSharedSizePanel(this);
			InvalidateMeasure();
		}
#endif
		internal void OnSharedGroupStatesChanged(SharedGroupStateCollection oldValue, SharedGroupStateCollection newValue) {
			SchedulerLogger.TraceOpenGroup(XpfLoggerTraceLevel.SharedSizePanel, "->SharedSizePanel.OnSharedGroupStatesChanged: {0}-({1}), old={2}, new={3}", VisualElementHelper.GetElementName(this), VisualElementHelper.GetTypeName(this), VisualElementHelper.GetStringValue(oldValue), VisualElementHelper.GetStringValue(newValue));
			if(oldValue != null) {
				SharedGroupState oldSharedGroupState = GetSharedGroupState(oldValue);
				oldSharedGroupState.RemoveSharedSizePanel(this);
				groupState = null;
			}
			if(newValue != null) {
				SharedGroupState newSharedGroupState = GetSharedGroupState(newValue);
				newSharedGroupState.AddSharedSizePanel(this);
				groupState = newSharedGroupState;
			}
			SchedulerLogger.TraceCloseGroup(XpfLoggerTraceLevel.SharedSizePanel);
		}
		#region OnMeasure
		EventHandler onMeasure;
		public event EventHandler OnMeasure { add { onMeasure += value; } remove { onMeasure -= value; } }
		protected virtual void RaiseOnMeasure() {
			if(onMeasure != null)
				onMeasure(this, EventArgs.Empty);
		}
		#endregion
		#region OnArrange
		EventHandler onArrange;
		public event EventHandler OnArrange { add { onArrange += value; } remove { onArrange -= value; } }
		protected virtual void RaiseOnArrange() {
			if (onArrange != null)
				onArrange(this, EventArgs.Empty);
		}
		#endregion
		void OnLayoutUpdated(object sender, EventArgs e) {
			SharedGroupState sharedGroupState = GetSharedGroupState();
			if (sharedGroupState != null)
				sharedGroupState.LayoutUpdated();
		}
		protected override Size MeasureOverrideCore(Size availableSize) {
			SchedulerLogger.TraceOpenGroup(XpfLoggerTraceLevel.SharedSizePanel, "->SharedSizePanel.MeasureOverrideCore [begin]: {0}-({1}), SharedSizeGroup={2}, IsBaseSizePanel={3}, availableSize={4}", Name, VisualElementHelper.GetTypeName(this), SharedSizeGroup, IsBaseSizePanel, availableSize);
			RaiseOnMeasure();
			SharedGroupState sharedGroupState = GetSharedGroupState();
			sharedGroupState.OnPanelMeasure(this);
			int count = Children.Count;
			double totalPrimarySize = 0;
			double maxSecondarySize = 0;
			SizeHelperBase sizeHelper = SizeHelperBase.GetDefineSizeHelper(Orientation);
			int currentCellIndex = 0;
			for (int i = 0; i < count; i++) {
				UIElement child = Children[i];
				int span;
				SchedulerLogger.TraceOpenGroup(XpfLoggerTraceLevel.SharedSizePanel, "| process child[{0}]: {1}-({2})", i, VisualElementHelper.GetElementName(child), VisualElementHelper.GetTypeName(child));
				if (IsBaseSizePanel) {
					Size measureSize = sizeHelper.CreateSize(Double.PositiveInfinity, sizeHelper.GetSecondarySize(availableSize));
					child.Measure(measureSize);
					span = GetChildSpan(child);
					Size desiredSize = child.DesiredSize;
					sharedGroupState.ApplyDesiredSize(this, currentCellIndex, sizeHelper.GetDefineSize(desiredSize), span);
					currentCellIndex += span;
					SchedulerLogger.Trace(XpfLoggerTraceLevel.SharedSizePanel, "MeasureSize={0}, DesiredSize={1}", measureSize, desiredSize);
				}
				else {
					ApplyTemplateToChild(child);
					span = GetChildSpan(child);
					Size measureSize = sizeHelper.CreateSize(sharedGroupState.GetIntermediateSize(currentCellIndex, span), sizeHelper.GetSecondarySize(availableSize));
					if (child.DesiredSize != measureSize)
						SchedulerLayoutHelper.InvalidateParentLayoutCache(this);
					child.Measure(measureSize);
					SchedulerLogger.Trace(XpfLoggerTraceLevel.SharedSizePanel, "DesiredSize={1}, measureSize={2}", i, child.DesiredSize, measureSize);
					currentCellIndex += span;
				}
				if (NotificationElement != null)
					NotificationElement.InvalidateMeasure();
				totalPrimarySize += sharedGroupState.GetIntermediateSize(i, span);
				maxSecondarySize = Math.Max(maxSecondarySize, sizeHelper.GetSecondarySize(child.DesiredSize));
				SchedulerLogger.TraceCloseGroup(XpfLoggerTraceLevel.SharedSizePanel);
			}
			Size result = sizeHelper.CreateSize(totalPrimarySize, maxSecondarySize);
			SchedulerLogger.TraceCloseGroup(XpfLoggerTraceLevel.SharedSizePanel, "<-SharedSizePanel.MeasureOverrideCore [end]: Name={0}, SharedSizeGroup={1}, measuredSize={2}", Name, SharedSizeGroup, result);
			return result;
		}
		void ApplyTemplateToChild(Visual child) {
#if SL
			Control control = child as Control;
			if (control != null)
				control.ApplyTemplate();
#else
			FrameworkElement fe = child as FrameworkElement;
			SchedulerLogger.Trace(XpfLoggerTraceLevel.SharedSizePanel, "->SharedSizePanel.ApplyTemplateToChild: Name={0}-({1}), SharedSizeGroup={2}", (fe == null) ? child.GetType().Name : fe.Name, child.GetType().Name, SharedSizeGroup);
			if (fe != null)
				fe.ApplyTemplate();
#endif
		}
		protected virtual int GetChildSpan(UIElement child) {
			int result = GetSpan(child);
			if (result == 0) {
				int childrenCount = VisualTreeHelper.GetChildrenCount(child);
				if(childrenCount > 0)
					result = GetSpan(VisualTreeHelper.GetChild(child, 0));
			}
			return result != 0 ? result : 1;
		}
		protected virtual SharedGroupState GetSharedGroupState() {
			if (groupState == null)
				groupState = GetSharedGroupState(SharedGroupStateCollection.GetCollection(this));
			return groupState;
		}
		protected virtual SharedGroupState GetSharedGroupState(SharedGroupStateCollection collection) {
			return collection != null ? collection.GetGroup(SharedSizeGroup, Orientation) : null;
		}
		protected override Size ArrangeOverrideCore(Size finalSize) {
			SchedulerLogger.TraceOpenGroup(XpfLoggerTraceLevel.SharedSizePanel, "->SharedSizePanel.ArrangeOverrideCore [begin]: Name={0}, SharedSizeGroup={1}, finalSize={2}", Name, SharedSizeGroup, finalSize);
			RaiseOnArrange();
			SharedGroupState sharedGroupState = GetSharedGroupState();
			int count = Children.Count;
			SizeHelperBase sizeHelper = SizeHelperBase.GetDefineSizeHelper(Orientation);
			double totalPrimarySize = 0;
			double secondarySize = sizeHelper.GetSecondarySize(finalSize);
			int currentCellIndex = 0;
			for (int i = 0; i < count; i++) {
				UIElement child = Children[i];
				int span = GetChildSpan(child);
				double childFinalPrimarySize = sharedGroupState.GetFinalSize(currentCellIndex, span);
				Point location = sizeHelper.CreatePoint(totalPrimarySize, 0);
				Size childFinalSize = sizeHelper.CreateSize(childFinalPrimarySize, secondarySize);
				child.Arrange(new Rect(location, childFinalSize));
				totalPrimarySize += childFinalPrimarySize;
				currentCellIndex += span;
			}
			Size resultSize = sizeHelper.CreateSize(totalPrimarySize, secondarySize);
			SchedulerLogger.TraceCloseGroup(XpfLoggerTraceLevel.SharedSizePanel, "<-SharedSizePanel.ArrangeOverrideCore [end]: resultSize={0}", resultSize);
			return resultSize;
		}
		protected override void RecalculatePositions() {
			ElementPositionPropertyHelper.RecalculatePositionsForPanel(this, Orientation);
		}
	}
	public class SharedGroupState {
		protected class SpanDesiredSize {
			int startIndex;
			int span;
			double desiredSize;
			public SpanDesiredSize(int startIndex, double desiredSize, int span) {
				this.startIndex = startIndex;
				this.span = span;
				this.desiredSize = desiredSize;
			}
			public int StartIndex { get { return startIndex; } }
			public int Span { get { return span; } }
			public double DesiredSize { get { return desiredSize; } set { desiredSize = value; } }
		}
		protected class SpanDesiredSizeComparer : IComparable<SpanDesiredSize> {
			SpanDesiredSize size;
			public SpanDesiredSizeComparer(SpanDesiredSize size) {
				this.size = size;
			}
			#region IComparable<SpanDesiredSize> Members
			public int CompareTo(SpanDesiredSize other) {
				if (size.StartIndex == other.StartIndex) {
					if (size.Span == other.Span)
						return 0;
					else
						return other.Span - size.Span;
				}
				else
					return other.StartIndex - size.StartIndex;
			}
			#endregion
		}
		List<double> desiredPrimarySizes;
		List<SpanDesiredSize> spanSizes;
		List<double> finalSizes;
		List<SharedSizePanel> sharedSizePanelCollection;
		SharedGroupStateCollection collection;
		bool isFirstMeasure = true;
		public SharedGroupState(SharedGroupStateCollection collection) {
			this.sharedSizePanelCollection = new List<SharedSizePanel>();
			InvalidateMeasure();
			this.collection = collection;
		}
		protected virtual bool IsArrangeValid { get { return finalSizes != null; } }
		List<SharedSizePanel> SharedSizePanelCollection { get { return sharedSizePanelCollection; } }
		public virtual void ApplyDesiredSize(SharedSizePanel panel, int childIndex, double desiredSize, int span) {
			InvalidateArrange();
			EnsureListCapacity(desiredPrimarySizes, childIndex);
			if (span <= 1) {
				if (desiredPrimarySizes[childIndex] < desiredSize) {
					desiredPrimarySizes[childIndex] = desiredSize;
					InvalidatePanelsMeasure(panel);
				}
			} else
				AddSpanSize(panel, childIndex, desiredSize, span);
		}
		public void AddSharedSizePanel(SharedSizePanel panel) {
			SharedSizePanelCollection.Add(panel);
		}
		public void RemoveSharedSizePanel(SharedSizePanel panel) {
			SchedulerLogger.TraceOpenGroup(XpfLoggerTraceLevel.SharedSizePanel, "->SharedGroupState.RemoveSharedSizePanel: {0}-({1})", VisualElementHelper.GetElementName(panel), VisualElementHelper.GetTypeName(panel));
			if (panel.IsBaseSizePanel)
				InvalidatePanelsMeasure(panel);
			SharedSizePanelCollection.Remove(panel);
			SchedulerLogger.TraceCloseGroup(XpfLoggerTraceLevel.SharedSizePanel);
		}
		protected virtual void AddSpanSize(SharedSizePanel panel, int childIndex, double desiredSize, int span) {
			InvalidateArrange();
			SpanDesiredSize newSize = new SpanDesiredSize(childIndex, desiredSize, span);
			int insertIndex = DevExpress.Utils.Algorithms.BinarySearch(spanSizes, new SpanDesiredSizeComparer(newSize));
			if (insertIndex >= 0) {
				if (spanSizes[insertIndex].DesiredSize < desiredSize) {
					spanSizes[childIndex].DesiredSize = desiredSize;
					InvalidatePanelsMeasure(panel);
				}
			} else {
				AddSpanSizeCore(insertIndex, newSize);
				InvalidatePanelsMeasure(panel);
			}
		}
		public void InvalidatePanelsMeasure(SharedSizePanel skipPanel) {
			SchedulerLogger.TraceOpenGroup(XpfLoggerTraceLevel.SharedSizePanel, "->SharedGroupState.InvalidatePanelsMeasure [begin]: PanelAskingInvalidation={0}-({1}), Group={2}, Orientation={3}", skipPanel.Name, skipPanel.GetType().Name, skipPanel.SharedSizeGroup, skipPanel.Orientation);
			collection.Owner.OnInvalidateInnerSharedSizeGroup();
			foreach (SharedSizePanel panel in SharedSizePanelCollection) {
				if (panel != skipPanel) {
					SchedulerLogger.TraceOpenGroup(XpfLoggerTraceLevel.SharedSizePanel, "| process panel {0}-({1})", VisualElementHelper.GetElementName(panel), VisualElementHelper.GetTypeName(panel));
					panel.InvalidateMeasure();
					DependencyObject current = VisualTreeHelper.GetParent(panel);
#if DEBUGTEST
					SchedulerLogger.Trace(XpfLoggerTraceLevel.SharedSizePanel, "->InvalidateMeasureForPanel:");
#endif
					while (current != null && current != collection.Owner) {
						UIElement element = current as UIElement;
						if (element != null) {
#if !SL
#if DEBUGTEST
							SchedulerLogger.Trace(XpfLoggerTraceLevel.SharedSizePanel, "->InvalidateMeasure parent {0}-({1}), IsMeasureValid={2}", VisualElementHelper.GetTypeName(element), current.GetType().Name, element.IsMeasureValid);
#endif
							if (!element.IsMeasureValid) 
								break;
#endif
							element.InvalidateMeasure();
						}
						current = VisualTreeHelper.GetParent(current);
					}
					SchedulerLogger.TraceCloseGroup(XpfLoggerTraceLevel.SharedSizePanel);
				}
			}
			SchedulerLogger.TraceCloseGroup(XpfLoggerTraceLevel.SharedSizePanel, "<-SharedGroupState.InvalidatePanelsMeasure [end]");
		}
		private void AddSpanSizeCore(int insertIndex, SpanDesiredSize newSize) {
			insertIndex = ~insertIndex;
			spanSizes.Insert(insertIndex, newSize);
		}
		protected virtual void EnsureListCapacity<T>(List<T> list, int lastIndex) {
			for (int i = list.Count; i <= lastIndex; i++)
				list.Add(default(T));
		}
		public virtual double GetFinalSize(int index, int span) {
			EnsureArrange();
			int endIndex = index + span - 1;
			EnsureListCapacity(finalSizes, endIndex);
			return GetTotalSize(index, endIndex);
		}
		public virtual double GetIntermediateSize(int index, int span) {
			return GetFinalSize(index, span);
		}
		protected virtual void EnsureArrange() {
			if (IsArrangeValid)
				return;
			finalSizes = new List<double>(desiredPrimarySizes);
			int count = spanSizes.Count;
			for (int i = 0; i < count; i++) {
				SpanDesiredSize spanSize = spanSizes[i];
				int startIndex = spanSize.StartIndex;
				int endIndex = startIndex + spanSize.Span - 1;
				EnsureListCapacity(finalSizes, endIndex);
				double totalSize = GetTotalSize(startIndex, endIndex);
				if (totalSize >= spanSize.DesiredSize)
					continue;
				AdjustFinalSizes(startIndex, endIndex, totalSize, spanSize.DesiredSize);
			}
		}
		protected virtual double GetTotalSize(int startIndex, int endIndex) {
			double result = 0.0;
			for (int i = startIndex; i <= endIndex; i++)
				result += finalSizes[i];
			return result;
		}
		protected virtual void AdjustFinalSizes(int startIndex, int endIndex, double totalSize, double desiredTotalSize) {
			if (totalSize != 0.0)
				AdjustFinalSizesCore(startIndex, endIndex, totalSize, desiredTotalSize);
			else {
				double size = desiredTotalSize / (endIndex - startIndex + 1.0);
				AssignFinalSizesCore(startIndex, endIndex, size);
			}
		}
		private void AssignFinalSizesCore(int startIndex, int endIndex, double size) {
			for (int i = startIndex; i <= endIndex; i++)
				finalSizes[i] = size;
		}
		private void AdjustFinalSizesCore(int startIndex, int endIndex, double totalSize, double desiredTotalSize) {
			double delta = desiredTotalSize - totalSize;
			double weight = delta / totalSize + 1.0;
			for (int i = startIndex; i <= endIndex; i++)
				finalSizes[i] *= weight;
		}
		public virtual void InvalidateMeasure() {
			desiredPrimarySizes = new List<double>();
			spanSizes = new List<SpanDesiredSize>();
			InvalidateArrange();
		}
		public virtual void InvalidateArrange() {
			finalSizes = null;
		}
		public void OnPanelMeasure(SharedSizePanel panel) {
			if (isFirstMeasure) {
				SchedulerLogger.TraceOpenGroup(XpfLoggerTraceLevel.SharedSizePanel, "//InvalidatePanelsOnFirstMeasure");
				isFirstMeasure = false;
				InvalidateMeasure();
				InvalidatePanelsMeasure(panel);
				SchedulerLogger.TraceCloseGroup(XpfLoggerTraceLevel.SharedSizePanel);
			}			
		}
		internal void LayoutUpdated() {
			isFirstMeasure = true;
		}
	}
	public class SharedGroupStateCollection {
		public static SharedGroupState GetGroup(DependencyObject dp, string name, Orientation orientation) {
			SharedGroupStateCollection collection = GetCollection(dp);
			if(collection == null)
				return null;
			return collection.GetGroup(name, orientation);
		}
		public static SharedGroupStateCollection GetCollection(DependencyObject dp) {
#if SL
			return SharedGroupSizeContainer.GetSharedGroupStatesRouted(dp);
#else
			return SharedGroupSizeContainer.GetSharedGroupStates(dp);				
#endif
		}
		#region SharedGroupStateKey
		protected class SharedGroupStateKey {
			string name;
			Orientation orientation;
			public SharedGroupStateKey(string name, Orientation orientation) {
				this.name = name;
				this.orientation = orientation;
			}
			public string Name { get { return name; } }
			public Orientation Orientation { get { return orientation; } }
			public override int GetHashCode() {
				return Name.GetHashCode() ^ Orientation.GetHashCode();
			}
			public override bool Equals(object obj) {				
				SharedGroupStateKey key = obj as SharedGroupStateKey;
				if (key == null)
					return false;
				return key.Name == Name && key.Orientation == Orientation;
			}
		}
		#endregion
		SharedGroupSizeContainer owner;
		Dictionary<SharedGroupStateKey, SharedGroupState> groups;		
		public SharedGroupStateCollection(SharedGroupSizeContainer owner) {
			Guard.ArgumentNotNull(owner, "owner");
			this.owner = owner;
			this.groups = new Dictionary<SharedGroupStateKey, SharedGroupState>();
		}
		protected internal virtual SharedGroupSizeContainer Owner { get { return owner; } }
		public SharedGroupState GetGroup(string name, Orientation orientation) {
			SharedGroupState result;
			SharedGroupStateKey key = new SharedGroupStateKey(name, orientation);
			if (groups.TryGetValue(key, out result))
				return result;
			result = new SharedGroupState(this);
			groups.Add(key, result);
			return result;
		}
		public virtual void InvalidateMeasure() {			
			foreach (SharedGroupState state in groups.Values)
				state.InvalidateMeasure();
		}
		public virtual void InvalidateArrange() {
			foreach (SharedGroupState state in groups.Values)
				state.InvalidateArrange();
		}
#if DEBUGTEST 
		internal string GetStringValue() {
			return String.Format("Count={0}", this.groups.Count);
		}
#endif
	}
	#region DayViewMoreButtonControl
	public class DayViewMoreButtonControl : Control {
		readonly VisualDayViewMoreButton upMoreButton;
		readonly VisualDayViewMoreButton downMoreButton;
		public DayViewMoreButtonControl() {
			DefaultStyleKey = typeof(DayViewMoreButtonControl);
			this.upMoreButton = new VisualDayViewMoreButton();
			this.downMoreButton = new VisualDayViewMoreButton();
			Loaded += new RoutedEventHandler(OnLoaded);
			Unloaded += new RoutedEventHandler(OnUnloaded);
		}
		#region Properties
		#region VisibleAppointmentInfos
		public ObservableCollection<DayViewAppointmentInfo> VisibleAppointmentInfos {
			get { return (ObservableCollection<DayViewAppointmentInfo>)GetValue(VisibleAppointmentInfosProperty); }
			set { SetValue(VisibleAppointmentInfosProperty, value); }
		}
		public static readonly DependencyProperty VisibleAppointmentInfosProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DayViewMoreButtonControl, ObservableCollection<DayViewAppointmentInfo>>("VisibleAppointmentInfos", null, (d, e) => d.OnVisibleAppointmentInfosChanged(e.OldValue, e.NewValue), null);
		void OnVisibleAppointmentInfosChanged(ObservableCollection<DayViewAppointmentInfo> oldValue, ObservableCollection<DayViewAppointmentInfo> newValue) {
			TryRecalculateMoreButtons();
			if (oldValue != null) {
				oldValue.CollectionChanged -= OnVisibleAppointmentInfosCollectionChanged;
			}
			if (newValue != null) {
				newValue.CollectionChanged += OnVisibleAppointmentInfosCollectionChanged;
			}
		}
		void OnVisibleAppointmentInfosCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			TryRecalculateMoreButtons();
		}
		#endregion
		#region ScrollViewer
		public static readonly DependencyProperty ScrollViewerProperty = CreateScrollViewerProperty();
		static DependencyProperty CreateScrollViewerProperty() {
			return DependencyPropertyHelper.RegisterProperty<DayViewMoreButtonControl, ScrollViewer>("ScrollViewer", null, OnScrollViewerChanged);
		}
		static void OnScrollViewerChanged(DayViewMoreButtonControl control, DependencyPropertyChangedEventArgs<ScrollViewer> e) {
			control.OnScrollViewerChanged(e.OldValue, e.NewValue);
			control.TryRecalculateMoreButtons();
		}
		public ScrollViewer ScrollViewer {
			get { return (ScrollViewer)GetValue(ScrollViewerProperty); }
			set { SetValue(ScrollViewerProperty, value); }
		}
		#endregion
		public VisualDayViewMoreButton UpMoreButton { get { return upMoreButton; } }
		public VisualDayViewMoreButton DownMoreButton { get { return downMoreButton; } }
		#region View
		public static readonly DependencyProperty ViewProperty = CreateViewProperty();
		static DependencyProperty CreateViewProperty() {
			return DependencyPropertyHelper.RegisterProperty<DayViewMoreButtonControl, SchedulerViewBase>("View", null);
		}
		public SchedulerViewBase View {
			get { return (SchedulerViewBase)GetValue(ViewProperty); }
			set { SetValue(ViewProperty, value); }
		}
		#endregion
		#endregion
		void OnUnloaded(object sender, RoutedEventArgs e) {
			UnsubscribeScrollViewer(ScrollViewer);
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			UnsubscribeScrollViewer(ScrollViewer);
			SubscribeScrollViewer(ScrollViewer);
		}
		protected internal virtual void TryRecalculateMoreButtons() {
			if (ScrollViewer != null && VisibleAppointmentInfos != null)
				CalculateMoreButtons();
		}
		protected internal virtual void OnScrollViewerChanged(ScrollViewer oldValue, ScrollViewer newValue) {
			UnsubscribeScrollViewer(oldValue);
			SubscribeScrollViewer(newValue);			
		}
		protected void SubscribeScrollViewer(ScrollViewer scrollViewer) {
			if (scrollViewer != null)
				ScrollViewerSubscriber.Subscribe(scrollViewer, OnScrollChanged);
		}
		protected void UnsubscribeScrollViewer(ScrollViewer scrollViewer) {
			if (scrollViewer != null)
				ScrollViewerSubscriber.Unsubscribe(scrollViewer, OnScrollChanged);
		}
		protected internal virtual void OnScrollChanged(object sender, ScrollChangedEventArgs e) {
			TryRecalculateMoreButtons();
		}
		protected internal virtual void CalculateMoreButtons() {
			Rect viewportBounds = GetViewportBounds();
			DownMoreButton.ScrollOffset = CalculateDownScrollOffset(viewportBounds);
			DownMoreButton.Visibility = DownMoreButton.ScrollOffset > double.MinValue ? Visibility.Visible : Visibility.Collapsed;
			UpMoreButton.ScrollOffset = CalculateUpScrollOffset(viewportBounds);
			UpMoreButton.Visibility = UpMoreButton.ScrollOffset < double.MaxValue ? Visibility.Visible : Visibility.Collapsed;
		}
		Rect GetViewportBounds() {
			return new Rect(new Point(ScrollViewer.HorizontalOffset, ScrollViewer.VerticalOffset), new Size(ScrollViewer.ViewportWidth, ScrollViewer.ViewportHeight));
		}
		protected internal virtual double CalculateDownScrollOffset(Rect viewportBounds) {
			double offset = double.MinValue;
			int count = VisibleAppointmentInfos.Count;
			for (int i = 0; i < count; i++)
				offset = CalculateDownScrollOffsetCore(viewportBounds, VisibleAppointmentInfos[i], offset);
			return offset;
		}
		double CalculateDownScrollOffsetCore(Rect viewportBounds, DayViewAppointmentInfo info, double offset) {
			if (info.Bounds.Bottom < viewportBounds.Top)
				return Math.Max(info.Bounds.Top, offset);
			else
				return offset;
		}
		protected internal virtual double CalculateUpScrollOffset(Rect viewportBounds) {
			double offset = double.MaxValue;
			int count = VisibleAppointmentInfos.Count;
			for (int i = 0; i < count; i++)
				offset = CalculateUpScrollOffsetCore(viewportBounds, VisibleAppointmentInfos[i], offset);
			return offset;
		}
		double CalculateUpScrollOffsetCore(Rect viewportBounds, DayViewAppointmentInfo info, double offset) {
			if (info.Bounds.Top > viewportBounds.Bottom)
				return Math.Min(viewportBounds.Top + (info.Bounds.Bottom - viewportBounds.Bottom), offset);
			else
				return offset;
		}
	}
	#endregion
	#region DayViewAppointmentInfo
	public class DayViewAppointmentInfo {
		public DayViewAppointmentInfo(Rect bounds, Appointment appointment) {
			Bounds = bounds;
			Appointment = appointment;
		}
		public Rect Bounds { get; set; }
		public Appointment Appointment { get; set; }
		public override bool Equals(object obj) {
			if (Object.ReferenceEquals(this, obj))
				return true;
			DayViewAppointmentInfo info = obj as DayViewAppointmentInfo;
			if (info == null)
				return false;
			return this.Bounds.Equals(info.Bounds) && Object.ReferenceEquals(this.Appointment, info.Appointment);
		}
		public override int GetHashCode() {
			return (Bounds.GetHashCode() << 28) | Appointment.GetHashCode();
		}
	}
	#endregion
#if !SL
	#region DayViewScrollViewer
	public class DayViewScrollViewer : SchedulerScrollViewer {
		public static readonly DependencyProperty BottomOffsetProperty;
		static DayViewScrollViewer() {
			BottomOffsetProperty = DependencyPropertyHelper.RegisterProperty<DayViewScrollViewer, double>("BottomOffset", 0.0);
			EventManager.RegisterClassHandler(typeof(DayViewScrollViewer), FrameworkElement.RequestBringIntoViewEvent, new RequestBringIntoViewEventHandler(OnRequestBringIntoView));
		}
		public DayViewScrollViewer(){
			DefaultStyleKey = typeof(DayViewScrollViewer);
		}
		#region SelectionLayoutChanged
		public void Subscribe(SelectionPresenter selectionPresenter) {
			selectionPresenter.SelectionLayoutChanged += new LastSelectedCellLayoutChangedEventHandler(OnSelectionLayoutChanged);
		}
		public void Unsubscribe(SelectionPresenter selectionPresenter) {
			selectionPresenter.SelectionLayoutChanged -= new LastSelectedCellLayoutChangedEventHandler(OnSelectionLayoutChanged);
		}
		protected virtual void OnSelectionLayoutChanged(object sender, LastSelectedCellLayoutChangedEventArgs e) {
			Rect rect = e.GetCellBounds(this);
			if (rect == Rect.Empty)
				return;
			if (rect.Top < 0)
				ScrollToVerticalOffset(VerticalOffset + rect.Top);
			else if (rect.Bottom > ViewportHeight)
				ScrollToVerticalOffset(VerticalOffset + (rect.Bottom - ViewportHeight));
		}
		#endregion
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("DayViewScrollViewerViewportBounds")]
#endif
		public Rect ViewportBounds { get { return new Rect(new Point(HorizontalOffset, VerticalOffset), new Size(ViewportWidth, ViewportHeight)); } }
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("DayViewScrollViewerBottomOffset")]
#endif
		public double BottomOffset { get { return (double)GetValue(BottomOffsetProperty); } set { SetValue(BottomOffsetProperty, value); } }
		protected override void OnScrollChanged(ScrollChangedEventArgs e) {
			BottomOffset = ScrollableHeight - VerticalOffset;
			base.OnScrollChanged(e);
		}
		static void OnRequestBringIntoView(object sender, RequestBringIntoViewEventArgs e) {
			e.Handled = true;
		}
	}
	#endregion
#endif
	public class DayViewAppointmentsScrollCalculator {
		readonly ScrollViewer scrollViewer;
		public DayViewAppointmentsScrollCalculator(ScrollViewer scrollViewer) {
			this.scrollViewer = scrollViewer;
		}
		public double CalculateVerticalScrollOffsetMakeAppointmentVisible(double appointmentTop, double appointmentBottom) {
			double offsetValue = 0.0;
			double miniOffset = Math.Min(Math.Max(0, appointmentBottom - appointmentTop), scrollViewer.ViewportHeight);
			double viewportBottom = scrollViewer.VerticalOffset + scrollViewer.ViewportHeight;
			if (appointmentBottom <= scrollViewer.VerticalOffset) {
				offsetValue = appointmentBottom - scrollViewer.VerticalOffset;
				offsetValue -= miniOffset;
			}
			else if (appointmentTop >= viewportBottom) {
				offsetValue = appointmentTop - viewportBottom;
				offsetValue += miniOffset;
			}
			else if (appointmentBottom >= viewportBottom) {
				double bottomAppointmentOffset = appointmentBottom - viewportBottom;
				offsetValue = Math.Min(bottomAppointmentOffset, scrollViewer.ViewportHeight);
				if (scrollViewer.VerticalOffset + offsetValue > appointmentTop)
					offsetValue -= (viewportBottom - appointmentTop); 
			}
			else if (appointmentTop < scrollViewer.VerticalOffset) { 
				offsetValue = appointmentTop - scrollViewer.VerticalOffset;
			}
			return scrollViewer.VerticalOffset + offsetValue;
		}
		public Rect GetVisibleBounds(VisualAppointmentInfo appointmentInfo, Visual visual) {
			GeneralTransform panelToVisualTransform = appointmentInfo.Panel.Visual.TransformToVisual(visual);
			Rect appointmentBounds = panelToVisualTransform.TransformBounds(appointmentInfo.Bounds);
			Rect viewportBounds = new Rect(new Point(0, 0), new Size(scrollViewer.ViewportWidth, scrollViewer.ViewportHeight));
			GeneralTransform scrollViewerToVisualTransform = scrollViewer.TransformToVisual(visual);
			Rect result = appointmentBounds;
			result.Intersect(scrollViewerToVisualTransform.TransformBounds(viewportBounds));
			return result;
		}
		protected internal Rect GetViewportBounds() {
			Rect result = new Rect();
			result.X = scrollViewer.HorizontalOffset;
			result.Y = scrollViewer.VerticalOffset;
			result.Width = scrollViewer.ViewportWidth;
			result.Height = scrollViewer.ViewportHeight;
			return result;
		}
	}
	#region DayViewAppointmentInfoCollection
	public class DayViewAppointmentInfoCollection : ObservableCollection<DayViewAppointmentInfo> {
	}
	#endregion
	#region DayViewAppointmentInfoContainer
	public class DayViewAppointmentInfoContainer : Decorator, IAppointmentsInfoChangedListener<DayViewAppointmentInfo> {
		readonly DayViewAppointmentInfoCollection appointmentInfos = new DayViewAppointmentInfoCollection();
		public DayViewAppointmentInfoCollection AppointmentInfos { get { return appointmentInfos; } }
		#region IAppointmentsInfoChangedListener<DayViewAppointmentInfo> Members
		void IAppointmentsInfoChangedListener<DayViewAppointmentInfo>.OnChanged(List<DayViewAppointmentInfo> oldInfos, List<DayViewAppointmentInfo> newInfos) {
			OnChangedCore(oldInfos, newInfos);
		}
		#endregion
		protected virtual void OnChangedCore(List<DayViewAppointmentInfo> oldInfos, List<DayViewAppointmentInfo> newInfos) {
			if (!HasChanged(oldInfos, newInfos))
				return;
			if (oldInfos != null && oldInfos.Count > 0)
				RemoveOldInfos(oldInfos);
			if (newInfos != null && newInfos.Count > 0)
				AddNewInfos(newInfos);
		}
		protected virtual void AddNewInfos(List<DayViewAppointmentInfo> infos) {
			foreach (DayViewAppointmentInfo info in infos) {
				AppointmentInfos.Add(info);
			}
		}
		protected virtual void RemoveOldInfos(List<DayViewAppointmentInfo> infos) {
			foreach (DayViewAppointmentInfo info in infos) {
				AppointmentInfos.Remove(info);
			}
		}
		protected virtual bool HasChanged(List<DayViewAppointmentInfo> oldInfos, List<DayViewAppointmentInfo> newInfos) {
			if (Object.ReferenceEquals(oldInfos, newInfos))
				return false;
			if (oldInfos == null || newInfos == null)
				return true;
			if (oldInfos.Count == newInfos.Count) {
				int count = oldInfos.Count;
				for (int i = 0; i < count; i++) {
					if (!oldInfos[i].Equals(newInfos[i]))
						return true;
				}
				return false;
			}
			else
				return true;
		}
	}
	#endregion
	public class DayViewItemsControl : ItemsControl {
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			base.PrepareContainerForItemOverride(element, item); 
		}
	}
	public class PixelSnappedSharedSizePanel : SharedSizePanel {
		protected override Size ArrangeOverrideCore(Size finalSize) {
			SchedulerLogger.TraceOpenGroup(XpfLoggerTraceLevel.PixelSnappedSharedSizePanel, "->PixelSnappedSharedSizePanel.ArrangeOverrideCore: [{0}], finalSize = {1}", Name, finalSize);
			RaiseOnArrange();
			SharedGroupStateCollection collection = SharedGroupStateCollection.GetCollection(this);
			SharedGroupState sharedGroupState = GetSharedGroupState(collection);
			int count = Children.Count;
			SizeHelperBase sizeHelper = SizeHelperBase.GetDefineSizeHelper(Orientation);
			Rect[] splitSizeRects = PixelSnappedUniformGridLayoutHelper.SplitSizeDpiAware(finalSize, count, new Thickness(0), Orientation);
			double totalPrimarySize = 0;
			double secondarySize = sizeHelper.GetSecondarySize(finalSize);
			bool isGroupDirty = false;
			int currentCellIndex = 0;
			for (int i = 0; i < count; i++) {
				UIElement child = Children[i];
				SchedulerLogger.TraceOpenGroup(XpfLoggerTraceLevel.PixelSnappedSharedSizePanel, "| process child[{0}]: {1}-({2})", i, VisualElementHelper.GetElementName(child), VisualElementHelper.GetTypeName(child));
				int span = GetChildSpan(child);
				double childFinalPrimarySize = sizeHelper.GetDefineSize(splitSizeRects[i].Size());
				double lastPrimarySize = sharedGroupState.GetFinalSize(currentCellIndex, span);
				bool areClose = DoubleUtil.AreCloseApproximately(lastPrimarySize, childFinalPrimarySize, 1.0);
				SchedulerLogger.Trace(XpfLoggerTraceLevel.PixelSnappedSharedSizePanel, "childFinalPrimarySize={0}, lastPrimarySize={1}, areClose={2}", childFinalPrimarySize, lastPrimarySize, areClose);
				if (!areClose) {
					isGroupDirty = true;
				}
				else 
					childFinalPrimarySize = lastPrimarySize;
				sharedGroupState.ApplyDesiredSize(this, currentCellIndex, childFinalPrimarySize, span);
				Point location = sizeHelper.CreatePoint(totalPrimarySize, 0);
				Size childFinalSize = sizeHelper.CreateSize(childFinalPrimarySize, secondarySize);
				child.Arrange(new Rect(location, childFinalSize));
				SchedulerLogger.Trace(XpfLoggerTraceLevel.PixelSnappedSharedSizePanel, "child.RenderSize = {0}, childFinalSize = {1}", child.RenderSize, childFinalSize);
				totalPrimarySize += childFinalPrimarySize;
				currentCellIndex += span;
				SchedulerLogger.TraceCloseGroup(XpfLoggerTraceLevel.PixelSnappedSharedSizePanel);
			}
			if (isGroupDirty)
				sharedGroupState.InvalidatePanelsMeasure(this);
			SchedulerLogger.TraceCloseGroup(XpfLoggerTraceLevel.PixelSnappedSharedSizePanel);
			return sizeHelper.CreateSize(totalPrimarySize, secondarySize);
		}
	}
	#region DayViewAppointmentInfoCreatedEventHandler
	public delegate void DayViewAppointmentInfoCreatedEventHandler(object sender, DayViewAppointmentInfoCreatedEventArgs e);
	#endregion
	#region DayViewAppointmentInfoCreatedEventArgs
	public class DayViewAppointmentInfoCreatedEventArgs : EventArgs {
		List<DayViewAppointmentInfo> newValue;
		List<DayViewAppointmentInfo> oldValue;
		FrameworkElement handlerElement;
		public List<DayViewAppointmentInfo> NewValue { get { return newValue; } set { newValue = value; } }
		public List<DayViewAppointmentInfo> OldValue { get { return oldValue; } set { oldValue = value; } }
		public FrameworkElement HandlerElement { get { return handlerElement; } set { handlerElement = value; } }
	}
	#endregion
}
