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
using DevExpress.Xpf.Layout.Core.Platform;
using System.Collections.Specialized;
namespace DevExpress.Xpf.Layout.Core {
#if SILVERLIGHT
	public interface IObserver<in T> {
		void OnCompleted();
		void OnError(Exception error);
		void OnNext(T value);
	}
	public interface IObservable<out T> {
		IDisposable Subscribe(IObserver<T> observer);
	}
	public enum Dock {
		Left = 0,
		Top = 1,
		Right = 2,
		Bottom = 3,
	}
#endif
	public enum ContextAction {
		Float
	}
	public enum SizingAction {
		None = 0,
		North = 3,
		South = 6,
		East = 2,
		West = 1,
		NorthEast = 5,
		NorthWest = 4,
		SouthEast = 8,
		SouthWest = 7
	}
	public enum DropType {
		None,
		Center,
		Top,
		Bottom,
		Left,
		Right
	}
	public enum ResizeType {
		None,
		Top,
		Bottom,
		Left,
		Right
	}
	public enum Alignment {
		None,
		Fill,
		TopCenter,
		TopLeft,
		TopRight,
		MiddleCenter,
		MiddleLeft,
		MiddleRight,
		BottomCenter,
		BottomLeft,
		BottomRight
	}
	public enum DockType {
		None,
		Left,
		Right,
		Top,
		Bottom,
		Fill
	}
	public enum MoveType {
		None,
		Left,
		Right,
		Top,
		Bottom,
		InsideGroup
	}
	public enum InsertType {
		Before,
		After
	}
	public enum OperationType {
		Regular,
		Resizing,
		Reordering,
		Floating,
		ClientDragging,
		FloatingMoving,
		FloatingResizing,
		NonClientDragging
	}
	public enum ViewAction {
		Hiding, 
		Hide, 
		ShowSelection, 
		HideSelection
	}
	[Flags]
	public enum State : byte {
		Normal = 0x00,
		Hot = 0x01,
		Pressed = 0x02,
	}
	public enum SelectionMode {
		SingleItem,
		MultipleItems,
		ItemRange
	}
	public enum LayoutItemType {
		Panel,
		Group,
		TabPanelGroup,
		FloatGroup,
		Document,
		DocumentPanelGroup,
		AutoHideContainer,
		AutoHideGroup,
		AutoHidePanel,
		ControlItem,
		Splitter,
		CustomizationControl,
		LayoutTreeView,
		HiddenItemsList,
		HiddenItem,
		TreeItem,
		FixedItem,
		TabItem,
		LayoutSplitter,
		EmptySpaceItem,
		Separator,
		Label
	}
	public interface IBaseObject : IDisposable {
		bool IsDisposing { get; }
		event EventHandler Disposed;
	}
	public interface ISupportHierarchy<T> : ISupportVisitor<T>
		where T : class {
		T Parent { get; }
		T[] Nodes { get; }
	}
	public interface IReadOnlyCollection<T> : IEnumerable<T>, ISupportVisitor<T>
		where T : class {
		int Count { get; }
		bool Contains(T element);
	}
	public interface IChangeableCollection<T> : IReadOnlyCollection<T>, ISupportNotification<T>
		where T : class {
		void Add(T element);
		void AddRange(T[] elements);
		bool Remove(T element);
		void CopyTo(T[] array, int index);
		void Clear();
	}
	public enum CollectionChangedType {
		ElementAdded,
		ElementRemoved,
		ElementUpdated,
		ElementDisposed
	}
	public interface ISupportNotification<T>
		where T : class {
		event CollectionChangedHandler<T> CollectionChanged;
	}
	public interface ISupportVisitor<T>
		where T : class {
		void Accept(IVisitor<T> visitor);
		void Accept(VisitDelegate<T> visit);
	}
	public interface IVisitor<T>
		where T : class {
		void Visit(T element);
	}
	public interface IUIElement {
		IUIElement Scope { get; }
		UIChildren Children { get; }
	}
	public class UIChildren : INotifyCollectionChanged {
		List<IUIElement> elements;
		public UIChildren() {
			elements = new List<IUIElement>();
		}
		public void Add(IUIElement element) {
			if(!elements.Contains(element)) {
				elements.Add(element);
				RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, element));
			}
		}
		public bool Remove(IUIElement element) {
			bool removed = elements.Remove(element);
			if(removed)
				RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, element));
			return removed;
		}
		public IUIElement[] GetElements() {
			return elements.ToArray();
		}
		public T GetElement<T>() where T : class, IUIElement {
			foreach(IUIElement element in elements)
				if(element is T) return element as T;
			return null;
		}
		public int Count { get { return elements.Count; } }
		public void MakeLast(IUIElement element) {
			if(elements.Contains(element)) {
				elements.Remove(element);
				elements.Add(element);
			}
		}
		void RaiseCollectionChanged(NotifyCollectionChangedEventArgs ea) {
			var handler = CollectionChanged;
			if(handler != null) handler(this, ea);
		}
		#region INotifyCollectionChanged
		public event NotifyCollectionChangedEventHandler CollectionChanged;
		#endregion
	}
	public class CollectionChangedEventArgs<T> : EventArgs
		where T : class {
		T elementCore;
		CollectionChangedType changedTypeCore;
		public CollectionChangedEventArgs(T element, CollectionChangedType changedType)
			: base() {
			this.elementCore = element;
			this.changedTypeCore = changedType;
		}
		public T Element {
			get { return elementCore; }
		}
		public CollectionChangedType ChangedType {
			get { return changedTypeCore; }
		}
	}
	public delegate void VisitDelegate<T>(T element) where T : class;
	public delegate void CollectionChangedHandler<T>(CollectionChangedEventArgs<T> ea) where T : class;
	public interface ISelectionKey {
		object Item { get; }
		object ElementKey { get; }
		object ViewKey { get; }
	}
	public interface ILayoutElement : IBaseObject, ISupportHierarchy<ILayoutElement> {
		ILayoutContainer Container { get; }
		Size Size { get; }
		Point Location { get; }
		bool IsActive { get; }
		bool IsReady { get; }
		bool EnsureBounds();
		void Invalidate();
		State GetState(object hitResult);
		void SetState(object hitResult, State state);
		void ResetState();
		bool HitTestingEnabled { get; }
		bool HitTest(Point pt);
		LayoutElementHitInfo CalcHitInfo(Point pt);
		IEnumerator<ILayoutElement> GetEnumerator();
	}
	public interface ILayoutContainer : ILayoutElement {
		LayoutElementCollection Items { get; }
	}
	public interface ILayoutElementFactory {
		ILayoutElement CreateLayoutHierarchy(object rootKey);
		ILayoutElement GetElement(object key);
	}
	public enum HostType {
		Layout, Floating, AutoHide
	}
	public interface ILayoutElementHost : IBaseObject {
		HostType Type { get; }
		ILayoutElement LayoutRoot { get; }
		object RootKey { get; }
		ILayoutElement GetElement(object key);
		void EnsureLayoutRoot();
		void Invalidate();
		bool IsActiveAndCanProcessEvent { get; }
		Point ClientToScreen(Point clientPoint);
		Point ScreenToClient(Point screenPoint);
		void ReleaseCapture();
		void SetCapture();
		ILayoutElementFactory GetLayoutElementFactory();
		ILayoutElementBehavior GetElementBehavior(ILayoutElement element);
		ILayoutElement GetDragItem(ILayoutElement element);
	}
	public interface ILayoutElementHostAdapter : IDisposable {
		bool HitTest(ILayoutElementHost host, Point pt);
		LayoutElementHitInfo CalcHitInfo(ILayoutElementHost host, Point pt);
	}
	public interface IViewAdapter : ILayoutElementHostAdapter {
		ViewCollection Views { get; }
		IView GetView(object rootKey); 
		IView GetView(ILayoutElement element);
		IView GetView(Point screenPoint);
		IView GetBehindView(IView sourceView, Point screenPoint);
		Point GetBehindViewPoint(IView source, IView behindView, Point point);
		object NotificationSource { get; }
		IDragService DragService { get; }
		ISelectionService SelectionService { get; }
		IUIInteractionService UIInteractionService { get; }
		IActionService ActionService { get; }
		IContextActionService ContextActionService { get; }
		bool IsInEvent { get; }
		void ProcessMouseEvent(IView view, MouseEventType eventype, MouseEventArgs ea);
		void ProcessKey(IView view, KeyEventType eventype, System.Windows.Input.Key key);
		void ProcessAction(ViewAction action);
		void ProcessAction(IView view, ViewAction action);
	}
	public interface ILayoutElementBehavior {
		bool AllowDragging { get; }
		bool CanDrag(OperationType operation);
		bool CanSelect();
	}
	public interface IView : ILayoutElementHost, IUIServiceProvider {
		IViewAdapter Adapter { get; }
		void OnKeyDown(System.Windows.Input.Key key);
		void OnKeyUp(System.Windows.Input.Key key);
		void OnMouseDown(MouseEventArgs ea);
		void OnMouseUp(MouseEventArgs ea);
		void OnMouseMove(MouseEventArgs ea);
		void OnMouseLeave();
		bool CanHandleMouseDown();
		int ZOrder { get; }
		void InvalidateZOrder();
	}
}
