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
using DevExpress.Xpf.Core.Native;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.PropertyGrid.Internal;
using DevExpress.Xpf.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Editors;
namespace DevExpress.Xpf.PropertyGrid {
	public enum NavigationDirection {
		Left = 0x1,
		Up = 0x2,
		Right = 0x4,
		Down = 0x8,
		Next = 0x10,
		Previous = 0x20,
		PageUp = 0x40,
		PageDown = 0x80,
		First = 0x100,
		Last = 0x200,
		Enter = 0x400,
		PreviousContainer = 0x800,
		NextContainer = 0x1000,
		None = 0x0
	}
	enum NavigationDirectionEx {
		Previous = NavigationDirection.Previous | NavigationDirection.Left | NavigationDirection.Up,
		Next = NavigationDirection.Next | NavigationDirection.Right | NavigationDirection.Down,
	}
	public enum NavigationMode {
		None,
		Local,
		Auto
	}
	public interface INavigationSupport {
		bool ProcessNavigation(NavigationDirection direction);
		IEnumerable<FrameworkElement> GetChildren();
		FrameworkElement GetParent();
		bool GetSkipNavigation();
		bool GetUseLinearNavigation();
	}
	public class NavigationManager {
		public static readonly DependencyProperty NavigationModeProperty;
		public static readonly DependencyProperty NavigationIndexProperty;
		bool openEditorOnSelection = false;
		static NavigationManager() {
			NavigationModeProperty = DependencyPropertyManager.RegisterAttached("NavigationMode", typeof(NavigationMode), typeof(NavigationManager), new FrameworkPropertyMetadata(NavigationMode.None));
			NavigationIndexProperty = DependencyPropertyManager.RegisterAttached("NavigationIndex", typeof(int), typeof(NavigationManager), new FrameworkPropertyMetadata(0));
		}
		public bool OpenEditorOnSelection {
			get { return openEditorOnSelection; }
			protected internal set { openEditorOnSelection = value; }
		}		
		public NavigationManager() { }
		public static NavigationMode GetNavigationMode(DependencyObject obj) {
			if (obj == null)
				return NavigationMode.None;
			return (NavigationMode)obj.GetValue(NavigationModeProperty);
		}
		public static void SetNavigationMode(DependencyObject obj, NavigationMode value) {
			obj.SetValue(NavigationModeProperty, value);
		}
		public static int GetNavigationIndex(DependencyObject obj) {
			if (obj == null)
				return Int32.MaxValue;
			return (int)obj.GetValue(NavigationIndexProperty);
		}
		public static void SetNavigationIndex(DependencyObject obj, int value) {
			obj.SetValue(NavigationIndexProperty, value);
		}
		Dictionary<NavigationDirection, Func<FrameworkElement, bool>> callbacks;
		public PropertyGridControl Grid { get; set; }
		public PropertyGridView View { get; set; }
		public SelectionStrategy SelectionStrategy { get { return Grid.SelectionStrategy; } }
		public RowDataGenerator RowDataGenerator { get { return Grid.RowDataGenerator; } }
		public DataViewBase DataView { get { return Grid.DataView; } }		
		public CellEditorOwner CellEditorOwner { get { return View.CellEditorOwner; } }
		public NavigationManager(PropertyGridControl owner) {
			this.Grid = owner;
			this.View = owner.View;
			RegisterCallbacks();
		}
		void RegisterCallbacks() {
			callbacks = new Dictionary<NavigationDirection, Func<FrameworkElement, bool>>();
			callbacks[NavigationDirection.Left] = NavigateLeft;
			callbacks[NavigationDirection.Up] = NavigateUp;
			callbacks[NavigationDirection.Right] = NavigateRight;
			callbacks[NavigationDirection.Down] = NavigateDown;
			callbacks[NavigationDirection.Next] = new Func<FrameworkElement, bool>(fe => NavigateNext(fe, false));
			callbacks[NavigationDirection.Previous] = new Func<FrameworkElement, bool>(fe => NavigatePrevious(fe));
			callbacks[NavigationDirection.PageUp] = OnPageUp;
			callbacks[NavigationDirection.PageDown] = OnPageDown;
			callbacks[NavigationDirection.First] = SelectFirst;
			callbacks[NavigationDirection.Last] = SelectLast;
			callbacks[NavigationDirection.Enter] = new Func<FrameworkElement, bool>(fe => false);
			callbacks[NavigationDirection.NextContainer] = SelectNextContainer;
			callbacks[NavigationDirection.PreviousContainer] = SelectPreviousContainer;
		}		
		bool Navigate(NavigationDirection direction, FrameworkElement element) {
			return callbacks[direction](element);
		}
		public static NavigationDirection GetDirection(KeyEventArgs e) {
			var modifiers = DevExpress.Xpf.Editors.Helpers.ModifierKeysHelper.GetKeyboardModifiers(e);
			if (e.Key == Key.Enter && !e.Handled)
				return NavigationDirection.Enter;
			if (e.Key == Key.Home)
				return NavigationDirection.First;
			if (e.Key == Key.End)
				return NavigationDirection.Last;
			if (e.Key == Key.Left)
				return NavigationDirection.Left;
			if (e.Key == Key.Right)
				return NavigationDirection.Right;
			if (e.Key == Key.Up)
				return NavigationDirection.Up;
			if (e.Key == Key.Down)
				return NavigationDirection.Down;
			if (e.Key == Key.Tab) {
				var ctrlPressed = DevExpress.Xpf.Editors.Helpers.ModifierKeysHelper.IsCtrlPressed(modifiers);
				if (DevExpress.Xpf.Editors.Helpers.ModifierKeysHelper.IsShiftPressed(modifiers))
					return ctrlPressed ? NavigationDirection.PreviousContainer : NavigationDirection.Previous;
				else
					return ctrlPressed ? NavigationDirection.NextContainer : NavigationDirection.Next;
			}
			if (Enum.Equals(e.Key, Key.PageUp))
				return NavigationDirection.PageUp;
			if (Enum.Equals(e.Key, Key.PageDown))
				return NavigationDirection.PageDown;
			return NavigationDirection.None;
		}
		public void ProcessNavigation(KeyEventArgs e) {
			if (!Grid.IsKeyboardFocusWithin)
				return;
			var direction = GetDirection(e);
			if (direction == NavigationDirection.None)
				return;
			if (!View.CheckCanNavigate()) {
				e.Handled = true;
				return;
			}
			FrameworkElement selectedObject = GetSelectedObject();
			if (selectedObject == null || PropertyGridHelper.GetPropertyGrid(selectedObject) != Grid)
				return;
			var currentNavigationMode = GetNavigationMode(selectedObject);
			if (currentNavigationMode == NavigationMode.Local)
				return;
			if ((selectedObject as INavigationSupport).If(x => x.ProcessNavigation(direction)).ReturnSuccess()) {
				e.Handled = true;
				return;
			}
			if (GetUseLinearNavigation(selectedObject) || (direction & (NavigationDirection.Next | NavigationDirection.Previous)) != 0) {
				if (((int)direction & (int)NavigationDirectionEx.Next) != 0) {
					e.Handled = NavigateNext(selectedObject);
					return;
				}
				if (((int)direction & (int)NavigationDirectionEx.Previous) != 0) {
					e.Handled = NavigatePrevious(selectedObject);
					return;
				}
			}
			else {
				switch (direction) {
					case NavigationDirection.Left:
					case NavigationDirection.Up:
					case NavigationDirection.Right:
					case NavigationDirection.Down:
						bidirectionalNavigationLocker.Reset();
						e.Handled = Navigate(direction, selectedObject);
						return;
				}
			}
			switch (direction) {
				case NavigationDirection.PageUp:
				case NavigationDirection.PageDown:
				case NavigationDirection.First:
				case NavigationDirection.Last:
				case NavigationDirection.NextContainer:
				case NavigationDirection.PreviousContainer:
					e.Handled = Navigate(direction, selectedObject);
					break;
			}
		}
		protected virtual bool SelectPreviousContainer(FrameworkElement arg) {
			MoveFocusHelper.MoveFocus(View.PropertyGrid, true);
			return true;
		}
		protected virtual bool SelectNextContainer(FrameworkElement arg) {
			MoveFocusHelper.MoveFocus(View.PropertyGrid, false);
			return true;
		}
		#region bidirectional
		private bool NavigateDown(FrameworkElement selectedObject) {
			return NavigateBidirectional(selectedObject, NavigationDirection.Down);
		}
		private bool NavigateRight(FrameworkElement selectedObject) {
			return NavigateBidirectional(selectedObject, NavigationDirection.Right);
		}
		private bool NavigateUp(FrameworkElement selectedObject) {
			return NavigateBidirectional(selectedObject, NavigationDirection.Up);
		}
		private bool NavigateLeft(FrameworkElement selectedObject) {
			return NavigateBidirectional(selectedObject, NavigationDirection.Left);
		}
		readonly Locker bidirectionalNavigationLocker = new Locker();
		private bool NavigateBidirectional(FrameworkElement selectedObject, NavigationDirection direction, bool useChildren = false) {
			var parent = useChildren ? selectedObject : GetParentObject(selectedObject);
			if (parent == null)
				return false;
			if (GetUseLinearNavigation(parent)) {
				var directionEx = (NavigationDirectionEx)direction;
				if ((directionEx & NavigationDirectionEx.Next) != 0) {
					var skipChildren = bidirectionalNavigationLocker.IsLocked;
					bidirectionalNavigationLocker.Unlock();
					var result = NavigateNext(selectedObject, skipChildren, direction);
					return result;
				}
				if ((directionEx & NavigationDirectionEx.Previous) != 0)
					return NavigatePrevious(selectedObject, direction);
			}
			if (!useChildren && NavigateBidirectional(selectedObject, direction, true))
				return true;
			var siblings = GetChildren(parent).Where(x => x != selectedObject).ToList();
			if (useChildren && siblings.Count == 0)
				return false;
			var candidate = GetElementToSelect(parent, selectedObject, direction);
			if (candidate == parent) {
				if (!useChildren)
					bidirectionalNavigationLocker.Lock();
				else
					candidate = null;
			}
			if (candidate != null) {
				FrameworkElement newCandidate = candidate;
				if (!(candidate as INavigationSupport).If(x => !x.GetSkipNavigation()).ReturnSuccess()) {
					do {
						candidate = newCandidate;
						newCandidate = GetElementToSelect(newCandidate, selectedObject, direction);
					} while (newCandidate != candidate);
				}
				candidate = newCandidate;
			}
			return Select(candidate, direction);
		}
		FrameworkElement GetElementToSelect(FrameworkElement parent, FrameworkElement selectedObject, NavigationDirection direction) {
			var siblings = GetChildren(parent).Where(x => x != selectedObject).ToList();
			var left = new List<FrameworkElement>();
			var up = new List<FrameworkElement>();
			var right = new List<FrameworkElement>();
			var down = new List<FrameworkElement>();
			var siblingPositions = siblings.Select(x => x.TransformToVisual(selectedObject).Transform(new Point(0, 0))).ToList();
			for (int i = 0; i < siblings.Count; i++) {
				var point = siblingPositions[i];
				var sibling = siblings[i];
				if (point.X < 0)
					left.Add(sibling);
				if (point.Y < 0)
					up.Add(sibling);
				if (point.X > 0)
					right.Add(sibling);
				if (point.Y > 0)
					down.Add(sibling);
			}
			var horizontalOrderFunc = new Func<FrameworkElement, double>(x => siblingPositions[siblings.IndexOf(x)].X);
			var verticalOrderFunc = new Func<FrameworkElement, double>(x => siblingPositions[siblings.IndexOf(x)].Y);
			left = left.OrderByDescending(horizontalOrderFunc).ThenBy(x => Math.Abs(verticalOrderFunc(x))).ToList();
			right = right.OrderBy(horizontalOrderFunc).ThenBy(x => Math.Abs(verticalOrderFunc(x))).ToList();
			up = up.OrderByDescending(verticalOrderFunc).ThenBy(x => Math.Abs(horizontalOrderFunc(x))).ToList();
			down = down.OrderBy(verticalOrderFunc).ThenBy(x => Math.Abs(horizontalOrderFunc(x))).ToList();
			List<FrameworkElement> list = null;
			switch (direction) {
				case NavigationDirection.Left:
					list = left;
					break;
				case NavigationDirection.Up:
					list = up;
					break;
				case NavigationDirection.Right:
					list = right;
					break;
				case NavigationDirection.Down:
					list = down;
					break;
			}
			return list.FirstOrDefault() ?? parent;
		}
		#endregion
		#region simple
		bool NavigatePrevious(FrameworkElement selectedObject, NavigationDirection direction = NavigationDirection.Previous) {
			var selectedParent = GetParentObject(selectedObject);
			if (selectedParent == null)
				return true;
			var siblings = GetChildren(selectedParent).OrderBy(x => GetNavigationIndex(x)).ToList();
			var index = siblings.IndexOf(selectedObject);
			if (index != 0)
				return SelectSelfOrLastChild(siblings[index - 1]);
			else
				return Select(selectedParent, direction);
		}
		bool SelectSelfOrLastChild(FrameworkElement element) {
			var children = GetChildren(element).ToList();
			if (children.Count == 0)
				return Select(element, NavigationDirection.Previous);
			else
				return SelectSelfOrLastChild(children[children.Count - 1]);
		}
		private bool NavigateNext(FrameworkElement selectedObject, bool skipChildren = false, NavigationDirection direction = NavigationDirection.Next) {
			var innerChildren = skipChildren ? null : GetChildren(selectedObject).OrderBy(x => GetNavigationIndex(x)).ToList().If(x => x.Count > 0);
			if (innerChildren != null) {
				var value = Select(innerChildren[0], NavigationDirection.Next);
				if (value)
					return value;
			}
			var selectedParent = GetParentObject(selectedObject);
			if (selectedParent == null)
				return true;
			var siblings = GetChildren(selectedParent).OrderBy(x => GetNavigationIndex(x)).ToList();
			var index = siblings.IndexOf(selectedObject);
			if (index == siblings.Count - 1)
				return NavigateNext(selectedParent, true);
			else
				return Select(siblings[index + 1], direction);
		}
		#endregion
		#region common
		protected internal bool OnPageDown(FrameworkElement selectedObject) {
			OnPage(true);
			return true;
		}
		protected internal bool OnPageUp(FrameworkElement selectedObject) {
			OnPage(false);
			return true;
		}
		private bool SelectLast(FrameworkElement selectedObject) {
			var editor = selectedObject.With(GetParentObject).With(GetChildren).With(x => x.LastOrDefault());
			return editor.Return(x => Select(x, NavigationDirection.Last), () => false);
		}
		private bool SelectFirst(FrameworkElement selectedObject) {
			var editor = selectedObject.With(GetParentObject).With(GetChildren).With(x => x.FirstOrDefault());
			return editor.Return(x => Select(x, NavigationDirection.First), () => false);
		}
		public void SelectFirstElement() {
			NavigateNext(View);
		}
		public void SelectLastElement() {
			SelectLast(View);
		}
		#endregion
		FrameworkElement GetSelectedObject() {
			FrameworkElement focusedElement = null;
			var selectedRowControl = Grid.SelectedPropertyPath.With(x => RowDataGenerator.RowDataFromHandle(DataView.GetHandleByFieldName(x))).With(x => View.GetRowControl(x)) ??
				(View.SelectedItem as RowDataBase).With(x => View.GetRowControl(x));
			if(selectedRowControl == null || selectedRowControl.IsKeyboardFocusWithin && !selectedRowControl.IsKeyboardFocused || !View.IsKeyboardFocusWithin) {
				focusedElement = Keyboard.FocusedElement as FrameworkElement;
				FrameworkElement tempFocusedElement = null;
				if (GetNavigationMode(focusedElement) != NavigationMode.None)
					tempFocusedElement = focusedElement;
				focusedElement = (focusedElement.VisualParents().FirstOrDefault(x => GetNavigationMode(x) != NavigationMode.None && !GetSkipNavigation(x)) as FrameworkElement) ?? tempFocusedElement;
			}
			if (focusedElement == null)
				focusedElement = selectedRowControl;
			return focusedElement;
		}
		FrameworkElement GetParentObject(FrameworkElement selectedObject) {
			return (selectedObject as INavigationSupport).With(x => x.GetParent()) ?? selectedObject.VisualParents().FirstOrDefault(x => GetNavigationMode(x) != NavigationMode.None || x is INavigationSupport) as FrameworkElement;
		}
		public static IEnumerable<FrameworkElement> GetChildren(FrameworkElement parent) {
			if (parent is INavigationSupport) {
				var children = ((INavigationSupport)parent).GetChildren();
				if (children != null)
					return children.Where(x=>x!=null);
			}
			List<FrameworkElement> list = new List<FrameworkElement>();
			var enumerator = new ConditionalVisualTreeEnumerator(parent, x => NavigationManager.GetNavigationMode(x) == NavigationMode.None);
			while (enumerator.MoveNext())
				(enumerator.Current as FrameworkElement).If(x => NavigationManager.GetNavigationMode(x) != NavigationMode.None && x != parent).Do(x => list.Add(x));
			return list.Where(x => x != null);
		}
		bool Select(FrameworkElement element, NavigationDirection direction) {
			if (element == null)
				return false;
			if(element is PropertyGridControl && VisualTreeHelper.GetParent(element).With(x=>PropertyGridHelper.GetPropertyGrid(x)) == null) {				
				return true;
			}				
			if(element is PropertyGridView && direction == NavigationDirection.Next && ((element as PropertyGridView).SelectedItem != null) && !GetSkipNavigation(element)) {
				if (CommitCurrentCellEditorAndCheckCanNavigate(element)) {
					element.Focus();
				}
				return true;
			}
			var grid = PropertyGridHelper.GetPropertyGrid(element) ?? Grid;
			var view = grid.View;			
			var manager = grid.NavigationManager;
			var strategy = grid.SelectionStrategy;
			var owner = view.CellEditorOwner;
			bool hasActiveEditor = owner.ActiveEditor != null || manager.OpenEditorOnSelection;
			if (!view.CheckCanNavigate())
				return false;
			if (grid != Grid && CellEditorOwner.ActiveEditor != null)
				grid.View.ImmediateActionsManager.EnqueueAction(() => grid.View.Focus());
			if (GetSkipNavigation(element)) {
				manager.OpenEditorOnSelection |= !owner.EditorWasClosed;
				if (direction == NavigationDirection.First)
					direction = NavigationDirection.Next;
				if (direction == NavigationDirection.Last)
					direction = NavigationDirection.Previous;
				return manager.Navigate(direction, element);
			}
			if (element is RowControlBase) {
				manager.OpenEditorOnSelection |= !owner.EditorWasClosed && owner.ActiveEditor != null;
				if (CommitCurrentCellEditorAndCheckCanNavigate(element)) {
					if (!view.IsKeyboardFocusWithin)
						view.Focus();
					var target = (element as RowControlBase);
					if (!((INavigationSupport)target).GetSkipNavigation()) {						
						owner.EditorWasClosed = true;
					}
					strategy.SelectViaHandle(target.RowData.Handle);
				}
				return true;
			}
			if (element is CellEditorPresenter) {
				if (CommitCurrentCellEditorAndCheckCanNavigate(element)) {
					strategy.SelectViaHandle(PropertyGridHelper.GetRowData(element).Handle);
					(element as CellEditorPresenter).IsSelected = true;
					if (hasActiveEditor)
						(element as CellEditorPresenter).ShowEditor();
				}
				return true;
			}
			if (CommitCurrentCellEditorAndCheckCanNavigate(element)) {
				var data = PropertyGridHelper.GetRowData(element);
				var dataStrategy = data.With(x => x.RowDataGenerator).With(x => x.View).With(x => x.SelectionStrategy);
				if (dataStrategy == strategy)
					data.With(x => x.Handle).Do(x => strategy.SelectViaHandle(x));
				manager.OpenEditorOnSelection = hasActiveEditor;
				owner.EditorWasClosed = true;
				FocusElementOrFirstAvailableChild(element, direction);
			}
			return true;
		}
		public static void FocusElementOrFirstAvailableChild(FrameworkElement element, NavigationDirection direction) {
			var forwardDirection = (((NavigationDirectionEx)direction) & NavigationDirectionEx.Previous) == 0;
			var selector = forwardDirection ? new Func<IEnumerable<DependencyObject>, Func<DependencyObject, bool>, DependencyObject>(Enumerable.FirstOrDefault) : Enumerable.Last;
			selector(element.VisualChildren(true), x => x is FrameworkElement && (x as FrameworkElement).Focusable).Do(x => KeyboardHelper.Focus((FrameworkElement)x));
		}
		public static bool CheckCanSelect(FrameworkElement element) {
			if (element == null)
				return false;
			return NavigationManager.GetNavigationMode(element) != NavigationMode.None && !(element as INavigationSupport).If(x => x.GetSkipNavigation()).ReturnSuccess();
		}
		bool CommitCurrentCellEditorAndCheckCanNavigate(FrameworkElement element) {
			if (element == null)
				return true;
			var cellEditorError = false;
			foreach (var view in element.VisualParents().OfType<PropertyGridView>()) {
				view.CellEditorOwner.CurrentCellEditor.Do(x => x.CommitEditor());
				cellEditorError |= view.HasErrorInActiveEditor;
				if (cellEditorError)
					return false;
			}
			return !cellEditorError;
		}
		bool GetUseLinearNavigation(DependencyObject obj) {
			if (obj is INavigationSupport)
				return ((INavigationSupport)obj).GetUseLinearNavigation();
			return false;
		}
		bool GetSkipNavigation(DependencyObject obj) {
			if (obj is INavigationSupport)
				return (obj as INavigationSupport).GetSkipNavigation();
			return false;
		}
		void OnPage(bool down) {
			int coeff = down ? 1 : -1;
			var rdb = View.SelectedItem as RowDataBase;
			if (rdb == null)
				return;
			var datalist = new List<RowDataBase>();
			int selectedIndex = datalist.IndexOf(rdb);
			if (!datalist.IsValidIndex(selectedIndex + coeff * 1))
				return;
			var controlsList = datalist.Select(x => View.GetRowControl(View.ItemContainerGenerator, x)).ToList();
			var headersList = new List<FrameworkElement>();
			var rectsList = headersList.Select(x => x == null ? new Rect(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity) : new Rect(x.TransformToAncestor(View.ScrollViewer).Transform(new Point()), x.RenderSize)).ToList();
			var svRect = new Rect(new Point(), new Size(View.ScrollViewer.ViewportWidth, View.ScrollViewer.ViewportHeight));
			if (svRect.Contains(rectsList[selectedIndex + coeff * 1])) {
				int i = 0;
				while (svRect.Contains(rectsList[selectedIndex + coeff * (i++)]) && rectsList.IsValidIndex(selectedIndex + coeff * i))
					;
				i -= 1;
				i = i * coeff;
				if (rectsList.IsValidIndex(selectedIndex + i)) {
					Grid.SelectionStrategy.SelectViaHandle(datalist[selectedIndex + i].Handle);
					return;
				}
			}
			if (down)
				View.ScrollViewer.PageDown();
			else
				View.ScrollViewer.PageUp();
			View.ScrollViewer.InvalidateVisual();
			View.ScrollViewer.UpdateLayout();
			rectsList = headersList.Select(x => (x == null || !x.IsVisible) ? new Rect(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity) : new Rect(x.TransformToAncestor(View.ScrollViewer).Transform(new Point()), x.RenderSize)).ToList();
			for (int i = down ? rectsList.Count - 1 : 0; down && i >= 0 || !down && i < rectsList.Count; i -= coeff) {
				if (svRect.Contains(rectsList[i])) {
					Grid.SelectionStrategy.SelectViaHandle(datalist[i].Handle);
					return;
				}
			}
		}
	}
}
