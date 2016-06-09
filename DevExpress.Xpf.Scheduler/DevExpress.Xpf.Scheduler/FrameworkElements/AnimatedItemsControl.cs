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
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Collections;
using System.Windows.Media.Animation;
using DevExpress.Utils;
using System.Collections.Specialized;
using System.Diagnostics;
using DXContentPresenter = DevExpress.Xpf.Core.DXContentPresenter;
#if SL
using System.ComponentModel;
#endif
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class SchedulerObservableCollection<T> : ObservableCollection<T> {
#if SL
		public void Move(int oldIndex, int newIndex) {
			T item = base[oldIndex];
			base.RemoveItem(oldIndex);
			base.InsertItem(newIndex, item);
			this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
		}
#endif
	}
	public class NotificationItemsSource : IEnumerable, INotifyCollectionChanged {
		SchedulerObservableCollection<object> items;
		public NotificationItemsSource(IEnumerable source) {
			this.items = new SchedulerObservableCollection<object>();
			INotifyCollectionChanged notifyItemsCollection = source as INotifyCollectionChanged;
			if (notifyItemsCollection != null)
				notifyItemsCollection.CollectionChanged += new NotifyCollectionChangedEventHandler(notifyItemsCollection_CollectionChanged);
			foreach (object obj in source)
				Add(obj);
		}
		void notifyItemsCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			SetNewContent(sender as IEnumerable);
		}
		public void Add(object obj) {
			items.Add(obj);
		}
		#region INotifyCollectionChanged Members
		public event NotifyCollectionChangedEventHandler CollectionChanged {
			add { items.CollectionChanged += value; }
			remove { items.CollectionChanged -= value; }
		}
		#endregion
		public virtual void SetNewContent(IEnumerable enumerable) {
			IEnumerator enumerator = enumerable.GetEnumerator();
			enumerator.Reset();
#if DEBUG || DEBUGTEST
			List<object> newItems = new List<object>();
#endif
			int newItemsCount = 0;
			for (int index = 0; enumerator.MoveNext(); index++) {
				object current = enumerator.Current;
#if DEBUG || DEBUGTEST
				newItems.Add(current);
#endif
				newItemsCount++;
				int oldIndex = OldIndexOf(current, index);
				if (oldIndex >= 0) {
					if (!Object.ReferenceEquals(current, items[oldIndex])) {
					}
					if (oldIndex == index) {
						continue;
					}
					else {
						Move(oldIndex, index);
					}
				}
				else {
					Insert(index, current);
				}
			}
			while (items.Count > newItemsCount) {
				RemoveAt(items.Count - 1);
			}
#if DEBUG || DEBUGTEST
			int count = items.Count;
			Debug.Assert(count == newItems.Count);
			for (int i = 0; i < count; i++)
				Debug.Assert(Object.Equals(items[i], newItems[i]));
#endif
		}
		protected virtual int GetOldIndex(BitArray foundItems, object current) {
			int oldIndex = OldIndexOf(current, 0);
			while (oldIndex >= 0 && foundItems[oldIndex]) {
				oldIndex = OldIndexOf(current, oldIndex + 1);
			}
			return oldIndex;
		}
		protected virtual int OldIndexOf(object current, int startIndex) {
			if (startIndex == 0)
				return items.IndexOf(current);
			for (int i = startIndex; i < items.Count; i++)
				if (Object.Equals(current, items[i]))
					return i;
			return -1;
		}
		#region IEnumerable Members
		public IEnumerator GetEnumerator() {
			return items.GetEnumerator();
		}
		#endregion
		protected virtual void Move(int oldIndex, int index) {
			items.Move(oldIndex, index);
		}
		protected virtual void Insert(int index, object obj) {
			items.Insert(index, obj);
		}
		protected virtual void RemoveAt(int index) {
			items.RemoveAt(index);
		}
	}
#if !SL
	public class AnimatedItemsControl : ItemsControl {
		static AnimatedItemsControl() {
			ItemsSourceProperty.OverrideMetadata(typeof(AnimatedItemsControl), new FrameworkPropertyMetadata(null, null, CoerceItemsSource));
		}
		static object CoerceItemsSource(DependencyObject d, object baseValue) {
			return ((AnimatedItemsControl)d).CoerceItemsSource(baseValue);
		}
		public AnimatedItemsControl() {
			DefaultStyleKey = typeof(AnimatedItemsControl);
		}
		public static string trace = String.Empty;
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
		}
		protected virtual object CoerceItemsSource(object baseValue) {
			IEnumerable enumerable = baseValue as IEnumerable;
			if (enumerable == null)
				return baseValue;
			NotificationItemsSource itemsSource = ItemsSource as NotificationItemsSource;
			if (itemsSource == null)
				return new NotificationItemsSource(enumerable);
			else {
				itemsSource.SetNewContent(enumerable);
				return itemsSource;
			}
		}
		protected override DependencyObject GetContainerForItemOverride() {
			return new DXContentPresenter();
		}
	}
#endif
	public interface IAnimationPanel {
	}
	public abstract class VisualChildChangeAction {
		readonly UIElement element;
		readonly int index;
		protected VisualChildChangeAction(UIElement element, int index) {
			Guard.ArgumentNotNull(element, "element");
			Guard.ArgumentNonNegative(index, "index");
			this.element = element;
			this.index = index;
		}
		public UIElement Element { get { return element; } }
		public int Index { get { return index; } }
	}
	public class VisualChildMovedChangeAction : VisualChildChangeAction {
		int oldIndex;
		public VisualChildMovedChangeAction(UIElement element, int index, int oldIndex)
			: base(element, index) {
			Guard.ArgumentNonNegative(oldIndex, "oldIndex");
			this.oldIndex = oldIndex;
		}
		public int OldIndex {
			get {
				return oldIndex;
			}
		}
	}
	public class VisualChildEmptyChangeAction : VisualChildChangeAction {
		public VisualChildEmptyChangeAction(UIElement element, int index)
			: base(element, index) {
		}
	}
	public class VisualChildAddedChangeAction : VisualChildChangeAction {
		public VisualChildAddedChangeAction(UIElement element, int index)
			: base(element, index) {
		}
	}
	public class VisualChildRemovedChangeAction : VisualChildChangeAction {
		public VisualChildRemovedChangeAction(UIElement element, int index)
			: base(element, index) {
		}
	}
#if !SL
	public abstract class AnimationAction {
		readonly VisualChildChangeAction changeAction;
		protected AnimationAction(VisualChildChangeAction changeAction) {
			Guard.ArgumentNotNull(changeAction, "changeAction");
			this.changeAction = changeAction;
		}
		public VisualChildChangeAction ChangeAction { get { return changeAction; } }
		public abstract UIElementArrangeResult Calculate(AnimationPanelDispatcher animationPanelDispatcher, PanelArrangeResult lastArrangeResult, PanelArrangeResult newArrangeResult, double animationState);
		public abstract UIElementMeasureResult CalculateMeasure(AnimationPanelDispatcher animationPanelDispatcher, PanelMeasureResult lastMeasureResult, PanelMeasureResult newMeasureResult, double animationState);
	}
	public class EmptyAnimationAction : AnimationAction {
		public EmptyAnimationAction(VisualChildEmptyChangeAction changeAction)
			: base(changeAction) {
		}
		public override UIElementArrangeResult Calculate(AnimationPanelDispatcher animationPanelDispatcher, PanelArrangeResult lastArrangeResult, PanelArrangeResult newArrangeResult, double animationState) {
			int index = ChangeAction.Index;
			Rect rect = animationPanelDispatcher.AnimateRect(lastArrangeResult.ChildrenArrangeResult[index].ArrangeRect, newArrangeResult.ChildrenArrangeResult[index].ArrangeRect, animationState);
			return new UIElementArrangeResult(rect, ChangeAction.Element);
		}
		public override UIElementMeasureResult CalculateMeasure(AnimationPanelDispatcher animationPanelDispatcher, PanelMeasureResult lastMeasureResult, PanelMeasureResult newMeasureResult, double animationState) {
			int index = ChangeAction.Index;
			Size size = animationPanelDispatcher.AnimateSize(lastMeasureResult.ChildrenMeasureResult[index].MeasureSize, newMeasureResult.ChildrenMeasureResult[index].MeasureSize, animationState);
			return new UIElementMeasureResult(size, ChangeAction.Element);
		}
	}
	public class MoveAnimationAction : AnimationAction {
		public MoveAnimationAction(VisualChildMovedChangeAction changeAction)
			: base(changeAction) {
		}
		public override UIElementArrangeResult Calculate(AnimationPanelDispatcher animationPanelDispatcher, PanelArrangeResult lastArrangeResult, PanelArrangeResult newArrangeResult, double animationState) {
			VisualChildMovedChangeAction changeAction = (VisualChildMovedChangeAction)ChangeAction;
			int newIndex = changeAction.Index;
			int oldIndex = changeAction.OldIndex;
			Rect rect = animationPanelDispatcher.AnimateRect(lastArrangeResult.ChildrenArrangeResult[oldIndex].ArrangeRect, newArrangeResult.ChildrenArrangeResult[newIndex].ArrangeRect, animationState);
			return new UIElementArrangeResult(rect, changeAction.Element);
		}
		public override UIElementMeasureResult CalculateMeasure(AnimationPanelDispatcher animationPanelDispatcher, PanelMeasureResult lastMeasureResult, PanelMeasureResult newMeasureResult, double animationState) {
			VisualChildMovedChangeAction changeAction = (VisualChildMovedChangeAction)ChangeAction;
			int newIndex = changeAction.Index;
			int oldIndex = changeAction.OldIndex;
			Size size = animationPanelDispatcher.AnimateSize(lastMeasureResult.ChildrenMeasureResult[oldIndex].MeasureSize, newMeasureResult.ChildrenMeasureResult[newIndex].MeasureSize, animationState);
			return new UIElementMeasureResult(size, changeAction.Element);
		}
	}
	public class RemoveAnimationAction : AnimationAction {
		public RemoveAnimationAction(VisualChildRemovedChangeAction changeAction)
			: base(changeAction) {
		}
		public override UIElementArrangeResult Calculate(AnimationPanelDispatcher animationPanelDispatcher, PanelArrangeResult lastArrangeResult, PanelArrangeResult newArrangeResult, double animationState) {
			VisualChildRemovedChangeAction changeAction = (VisualChildRemovedChangeAction)ChangeAction;
			int oldIndex = changeAction.Index;
			changeAction.Element.Opacity = 1.0 - animationState;
			return new UIElementArrangeResult(lastArrangeResult.ChildrenArrangeResult[oldIndex].ArrangeRect, changeAction.Element);
		}
		public override UIElementMeasureResult CalculateMeasure(AnimationPanelDispatcher animationPanelDispatcher, PanelMeasureResult lastMeasureResult, PanelMeasureResult newMeasureResult, double animationState) {
			VisualChildRemovedChangeAction changeAction = (VisualChildRemovedChangeAction)ChangeAction;
			int oldIndex = changeAction.Index;
			return new UIElementMeasureResult(lastMeasureResult.ChildrenMeasureResult[oldIndex].MeasureSize, changeAction.Element);
		}
		private Rect CalculateRemoveFirstAnimation(AnimationPanelDispatcher animationPanelDispatcher, UIElementArrangeResult newResult, UIElementArrangeResult oldResult, double animationState) {
			Point from = oldResult.ArrangeRect.Location;
			Point to = new Point(0, -oldResult.ArrangeRect.Size.Height);
			return new Rect(animationPanelDispatcher.AnimatePoint(from, to, animationState), newResult.ArrangeRect.Size);
		}
		private Rect CalculateRemoveLastAnimation(AnimationPanelDispatcher animationPanelDispatcher, UIElementArrangeResult newResult, UIElementArrangeResult oldResult, double animationState) {
			Point from = oldResult.ArrangeRect.Location;
			Point to = new Point(0, oldResult.ArrangeRect.Location.Y + newResult.ArrangeRect.Height);
			return new Rect(animationPanelDispatcher.AnimatePoint(from, to, animationState), newResult.ArrangeRect.Size);
		}
	}
	public class AddAnimationAction : AnimationAction {
		public AddAnimationAction(VisualChildAddedChangeAction changeAction)
			: base(changeAction) {
		}
		public override UIElementArrangeResult Calculate(AnimationPanelDispatcher animationPanelDispatcher, PanelArrangeResult lastArrangeResult, PanelArrangeResult newArrangeResult, double animationState) {
			VisualChildAddedChangeAction changeAction = (VisualChildAddedChangeAction)ChangeAction;
			int newIndex = changeAction.Index;
			return new UIElementArrangeResult(newArrangeResult.ChildrenArrangeResult[newIndex].ArrangeRect, changeAction.Element);
		}
		public override UIElementMeasureResult CalculateMeasure(AnimationPanelDispatcher animationPanelDispatcher, PanelMeasureResult lastMeasureResult, PanelMeasureResult newMeasureResult, double animationState) {
			VisualChildAddedChangeAction changeAction = (VisualChildAddedChangeAction)ChangeAction;
			int newIndex = changeAction.Index;
			return new UIElementMeasureResult(newMeasureResult.ChildrenMeasureResult[newIndex].MeasureSize, changeAction.Element);
		}
		private Rect CalculateAddFirstAnimation(AnimationPanelDispatcher animationPanelDispatcher, UIElementArrangeResult newResult, UIElementArrangeResult oldResult, double animationState) {
			Point from = new Point(0, -newResult.ArrangeRect.Size.Height);
			Point to = newResult.ArrangeRect.Location;
			return new Rect(animationPanelDispatcher.AnimatePoint(from, to, animationState), newResult.ArrangeRect.Size);
		}
		private Rect CalculateAddLastAnimation(AnimationPanelDispatcher animationPanelDispatcher, UIElementArrangeResult newResult, UIElementArrangeResult oldResult, double animationState) {
			Point to = newResult.ArrangeRect.Location;
			Point from = new Point(0, newResult.ArrangeRect.Location.Y + newResult.ArrangeRect.Height);
			return new Rect(animationPanelDispatcher.AnimatePoint(from, to, animationState), newResult.ArrangeRect.Size);
		}
	}
	public class AnimationPanelDispatcher {
		PanelMeasureResult lastMeasureResult;
		PanelArrangeResult lastArrangeResult;
		List<UIElement> lastArrangedChildren;
		List<AnimationAction> animationActions;
		List<UIElement> removedItems;
		DoubleAnimation animation;
		AnimationPanel2 panel;
		public AnimationPanelDispatcher(AnimationPanel2 panel) {
			this.lastArrangeResult = null;
			this.lastArrangedChildren = new List<UIElement>();
			this.animationActions = new List<AnimationAction>();
			this.removedItems = new List<UIElement>();
			this.panel = panel;
			animation = new DoubleAnimation();
			animation.From = 0;
			animation.To = 1;
			animation.FillBehavior = FillBehavior.Stop;
			animation.Duration = new Duration(TimeSpan.FromMilliseconds(500));
		}
		public virtual List<UIElement> LastArrangedChildren { get { return lastArrangedChildren; } }
		public virtual void ApplyArrangeResult(PanelArrangeResult arrangeResult) {
			lastArrangeResult = arrangeResult;
			lastArrangedChildren.Clear();
			int count = arrangeResult.ChildrenArrangeResult.Count;
			for (int i = 0; i < count; i++)
				lastArrangedChildren.Add(arrangeResult.ChildrenArrangeResult[i].Element);
		}
		public virtual void ApplyMeasureResult(PanelMeasureResult measureResult) {
			Guard.ArgumentNotNull(measureResult, "measureResult");
			this.lastMeasureResult = measureResult;
		}
		public virtual PanelMeasureResult CalculateAnimationMeasure(PanelMeasureResult newMeasureResult, double animationState) {
			animationState = -2.0 * animationState * animationState * animationState + 3.0 * animationState * animationState;
			int count = animationActions.Count;
			List<UIElementMeasureResult> result = new List<UIElementMeasureResult>();
			for (int i = 0; i < count; i++) {
				result.Add(animationActions[i].CalculateMeasure(this, lastMeasureResult, newMeasureResult, animationState));
			}
			return new PanelMeasureResult(result, AnimateSize(lastMeasureResult.MeasureSize, newMeasureResult.MeasureSize, animationState));
		}
		public virtual PanelArrangeResult CalculateAnimationArrange(PanelArrangeResult newArrangeResult, double animationState) {
			animationState = -2.0 * animationState * animationState * animationState + 3.0 * animationState * animationState;
			int count = animationActions.Count;
			List<UIElementArrangeResult> result = new List<UIElementArrangeResult>();
			for (int i = 0; i < count; i++) {
				result.Add(animationActions[i].Calculate(this, lastArrangeResult, newArrangeResult, animationState));
			}
			Size arrangeSize = CalculateAnimationArrangeSize(newArrangeResult, animationState);
			return new PanelArrangeResult(result, arrangeSize);
		}
		protected virtual Size CalculateAnimationArrangeSize(PanelArrangeResult newArrageResult, double animationState) {
			return AnimateSize(lastArrangeResult.ArrangeSize, newArrageResult.ArrangeSize, animationState);
		}
		public virtual Rect AnimateRect(Rect from, Rect to, double state) {
			return new Rect(AnimatePoint(from.Location, to.Location, state), AnimateSize(from.Size, to.Size, state));
		}
		public virtual Size AnimateSize(Size from, Size to, double state) {
			return new Size(AnimateValue(from.Width, to.Width, state), AnimateValue(from.Height, to.Height, state));
		}
		public virtual Point AnimatePoint(Point from, Point to, double state) {
			return new Point(AnimateValue(from.X, to.X, state), AnimateValue(from.Y, to.Y, state));
		}
		public virtual double AnimateValue(double from, double to, double state) {
			return from * (1.0 - state) + to * state;
		}
		public virtual void ApplyVisualChildrenChanges(List<VisualChildChangeAction> changeActions) {
			animationActions.Clear();
			int count = changeActions.Count;
			for (int i = 0; i < count; i++) {
				VisualChildChangeAction changeAction = changeActions[i];
				VisualChildMovedChangeAction childMovedAction = changeAction as VisualChildMovedChangeAction;
				if (childMovedAction != null) {
					animationActions.Add(new MoveAnimationAction(childMovedAction));
					continue;
				}
				VisualChildRemovedChangeAction childRemovedAction = changeAction as VisualChildRemovedChangeAction;
				if (childRemovedAction != null) {
					removedItems.Add(childRemovedAction.Element);
					animationActions.Add(new RemoveAnimationAction(childRemovedAction));
					continue;
				}
				VisualChildAddedChangeAction childAddedAction = changeAction as VisualChildAddedChangeAction;
				if (childAddedAction != null) {
					animationActions.Add(new AddAnimationAction(childAddedAction));
					continue;
				}
				VisualChildEmptyChangeAction childEmptyAction = changeAction as VisualChildEmptyChangeAction;
				if (childEmptyAction != null) {
					animationActions.Add(new EmptyAnimationAction(childEmptyAction));
					continue;
				}
			}
		}
		bool animationStarted;
		bool actually;
		public virtual bool IsAnimationActive {
			get { return animationStarted; }
		}
		public void StartAnimation(EventHandler animationComplete) {
			animation.Completed += animationComplete;
			animationStarted = true;
		}
		public void StartAnimationActually() {
			if (!actually) {
				actually = true;
				panel.BeginAnimation(AnimationPanel2.AnimationStateProperty, animation);
			}
		}
		public void StopAnimation() {
			this.removedItems.Clear();
			animationStarted = false;
			actually = false;
		}
		public List<UIElement> GetRemovedItems() {
			return removedItems;
		}
	}
	public abstract class AnimationPanel2 : Panel {
		readonly AnimationPanelDispatcher animationDispatcher;
		bool visualChildrenChanged;
		#region AnimationState
		public double AnimationState {
			get { return (double)GetValue(AnimationStateProperty); }
			set { SetValue(AnimationStateProperty, value); }
		}
		public static readonly DependencyProperty AnimationStateProperty = CreateAnimationStateProperty();
		static DependencyProperty CreateAnimationStateProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AnimationPanel2, double>("AnimationState", 0, FrameworkPropertyMetadataOptions.AffectsMeasure);
		}
		#endregion
		protected AnimationPanel2() {
			this.animationDispatcher = CreateAnimationDispatcher();
		}
		protected virtual AnimationPanelDispatcher AnimationDispatcher { get { return animationDispatcher; } }
		protected virtual bool VisualChildrenChanged { get { return visualChildrenChanged; } }
		protected virtual AnimationPanelDispatcher CreateAnimationDispatcher() {
			return new AnimationPanelDispatcher(this);
		}
		protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved) {
			if (this.IsLoaded)
				this.visualChildrenChanged = true;
			base.OnVisualChildrenChanged(visualAdded, visualRemoved);
		}
		protected abstract PanelArrangeResult ArrangeItems(List<UIElement> items, Size finalSize);
		protected abstract PanelMeasureResult MeasureItems(List<UIElement> items, Size availableSize);
		protected override int VisualChildrenCount {
			get {
				return base.VisualChildrenCount + animationDispatcher.GetRemovedItems().Count;
			}
		}
		protected override Visual GetVisualChild(int index) {
			if (index < Children.Count)
				return Children[index];
			else
				return animationDispatcher.GetRemovedItems()[index - Children.Count];
		}
		protected override Size MeasureOverride(Size availableSize) {
			if (visualChildrenChanged && !animationDispatcher.IsAnimationActive) {
				ProcessVisualChildrenChanges();
				StartAnimation();
				lastFinalSize = Size.Empty;
			}
			PanelMeasureResult measureResult;
			if (!animationDispatcher.IsAnimationActive) {
				measureResult = MeasureItems(GetChildrenAsList(), availableSize);
				animationDispatcher.ApplyMeasureResult(measureResult);
			}
			else {
				PanelMeasureResult newMeasureResult = MeasureItems(GetChildrenAsList(), availableSize);
				measureResult = animationDispatcher.CalculateAnimationMeasure(newMeasureResult, AnimationState);
				ApplyMeasureResult(measureResult);
			}
			return measureResult.MeasureSize;
		}
		private void StartAnimation() {
			List<UIElement> removedItems = animationDispatcher.GetRemovedItems();
			int count = removedItems.Count;
			for (int i = 0; i < count; i++)
				AddVisualChild(removedItems[i]);
			animationDispatcher.StartAnimation(OnAnimationComplete);
		}
		protected virtual void OnAnimationComplete(object sender, EventArgs e) {
			List<UIElement> removedItems = animationDispatcher.GetRemovedItems();
			int count = removedItems.Count;
			for (int i = 0; i < count; i++)
				RemoveVisualChild(removedItems[i]);
			animationDispatcher.StopAnimation();
			visualChildrenChanged = false;
		}
		PanelArrangeResult newArrangeResult;
		Size lastFinalSize;
		protected override Size ArrangeOverride(Size finalSize) {
			PanelArrangeResult arrangeResult;
			if (!animationDispatcher.IsAnimationActive) {
				arrangeResult = ArrangeItems(GetChildrenAsList(), finalSize);
				animationDispatcher.ApplyArrangeResult(arrangeResult);
			}
			else {
				if (lastFinalSize != finalSize) {
					newArrangeResult = ArrangeItems(GetChildrenAsList(), finalSize);
					lastFinalSize = finalSize;
				}
				arrangeResult = animationDispatcher.CalculateAnimationArrange(newArrangeResult, AnimationState);
				ApplyArrangeResult(arrangeResult);
				animationDispatcher.StartAnimationActually();
			}
			return arrangeResult.ArrangeSize;
		}
		protected virtual List<UIElement> GetChildrenAsList() {
			int count = Children.Count;
			List<UIElement> result = new List<UIElement>();
			for (int i = 0; i < count; i++)
				result.Add(Children[i]);
			return result;
		}
		protected virtual void ApplyMeasureResult(PanelMeasureResult arrangeResult) {
			List<UIElementMeasureResult> elementsResult = arrangeResult.ChildrenMeasureResult;
			int count = elementsResult.Count;
			for (int i = 0; i < count; i++) {
				UIElementMeasureResult elementMeasureResult = elementsResult[i];
				elementMeasureResult.Element.Measure(elementMeasureResult.MeasureSize);
			}
		}
		protected virtual void ApplyArrangeResult(PanelArrangeResult arrangeResult) {
			List<UIElementArrangeResult> elementsResult = arrangeResult.ChildrenArrangeResult;
			int count = elementsResult.Count;
			for (int i = 0; i < count; i++) {
				UIElementArrangeResult elementArrangeResult = elementsResult[i];
				elementArrangeResult.Element.Arrange(elementArrangeResult.ArrangeRect);
			}
		}
		protected virtual void ProcessVisualChildrenChanges() {
			List<VisualChildChangeAction> changeActions = new List<VisualChildChangeAction>();
			ProcessNewChildren(changeActions);
			ProcessOldChildren(changeActions);
			animationDispatcher.ApplyVisualChildrenChanges(changeActions);
		}
		private void ProcessNewChildren(List<VisualChildChangeAction> changeActions) {
			int count = Children.Count;
			List<UIElement> lastArrangedChild = AnimationDispatcher.LastArrangedChildren;
			for (int i = 0; i < count; i++) {
				UIElement child = Children[i];
				int oldIndex = lastArrangedChild.IndexOf(child);
				if (oldIndex >= 0) {
					if (oldIndex != i)
						changeActions.Add(new VisualChildMovedChangeAction(child, i, oldIndex));
					else
						changeActions.Add(new VisualChildEmptyChangeAction(child, i));
				}
				else
					changeActions.Add(new VisualChildAddedChangeAction(child, i));
			}
		}
		private void ProcessOldChildren(List<VisualChildChangeAction> changeActions) {
			List<UIElement> lastArrangedChild = AnimationDispatcher.LastArrangedChildren;
			int count = lastArrangedChild.Count;
			for (int i = 0; i < count; i++) {
				UIElement child = lastArrangedChild[i];
				if (!Children.Contains(child)) {
					changeActions.Add(new VisualChildRemovedChangeAction(child, i));
				}
			}
		}
	}
#endif
	public class UIElementArrangeResult {
		Rect arrangeRect;
		UIElement element;
		public UIElementArrangeResult(Rect arrangeRect, UIElement element) {
			Guard.ArgumentNotNull(element, "element");
			this.arrangeRect = arrangeRect;
			this.element = element;
		}
		public Rect ArrangeRect { get { return arrangeRect; } }
		public UIElement Element { get { return element; } }
	}
	public class UIElementMeasureResult {
		Size measureSize;
		UIElement element;
		public UIElementMeasureResult(Size measureSize, UIElement element) {
			Guard.ArgumentNotNull(element, "element");
			this.measureSize = measureSize;
			this.element = element;
		}
		public Size MeasureSize { get { return measureSize; } }
		public UIElement Element { get { return element; } }
	}
	public class PanelArrangeResult {
		List<UIElementArrangeResult> childrenArrangeResult;
		Size arrangeSize;
		public PanelArrangeResult(List<UIElementArrangeResult> childrenArrangeResult, Size arrangeSize) {
			Guard.ArgumentNotNull(childrenArrangeResult, "childrenArrangeResult");
			this.childrenArrangeResult = childrenArrangeResult;
			this.arrangeSize = arrangeSize;
		}
		public Size ArrangeSize { get { return arrangeSize; } }
		public List<UIElementArrangeResult> ChildrenArrangeResult { get { return childrenArrangeResult; } }
	}
	public class PanelMeasureResult {
		List<UIElementMeasureResult> childrenMeasureResult;
		Size measureSize;
		public PanelMeasureResult(List<UIElementMeasureResult> childrenMeasureResult, Size measureSize) {
			Guard.ArgumentNotNull(childrenMeasureResult, "childrenMeasureResult");
			this.childrenMeasureResult = childrenMeasureResult;
			this.measureSize = measureSize;
		}
		public Size MeasureSize { get { return measureSize; } }
		public List<UIElementMeasureResult> ChildrenMeasureResult { get { return childrenMeasureResult; } }
	}
#if !SL
	public class AnimationStackPanel : AnimationPanel2 {
		protected override PanelMeasureResult MeasureItems(List<UIElement> items, Size availableSize) {
			int count = items.Count;
			double width = double.IsPositiveInfinity(availableSize.Width) ? 0 : availableSize.Width;
			double height = 0;
			List<UIElementMeasureResult> result = new List<UIElementMeasureResult>();
			for (int i = 0; i < count; i++) {
				UIElement child = items[i];
				child.Measure(availableSize);
				result.Add(new UIElementMeasureResult(availableSize, child));
				width = Math.Max(child.DesiredSize.Width, width);
				height += child.DesiredSize.Height;
			}
			Size size = new Size(width, height);
			return new PanelMeasureResult(result, size);
		}
		protected override PanelArrangeResult ArrangeItems(List<UIElement> items, Size finalSize) {
			int count = items.Count;
			double height = 0;
			List<UIElementArrangeResult> result = new List<UIElementArrangeResult>();
			for (int i = 0; i < count; i++) {
				UIElement child = items[i];
				Rect rect = new Rect(0, height, finalSize.Width, child.DesiredSize.Height);
				child.Arrange(rect);
				height += child.RenderSize.Height;
				result.Add(new UIElementArrangeResult(rect, child));
			}
			return new PanelArrangeResult(result, finalSize);
		}
	}
#endif
}
