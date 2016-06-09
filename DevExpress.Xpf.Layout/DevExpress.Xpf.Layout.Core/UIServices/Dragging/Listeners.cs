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

using System.Windows;
namespace DevExpress.Xpf.Layout.Core.Dragging {
	public abstract class BaseDragListener : IDragServiceListener {
		public IUIServiceProvider ServiceProvider { get; set; }
		public object Key { get { return OperationType; } }
		public abstract OperationType OperationType { get; }
		public virtual void OnEnter() { }
		public virtual void OnLeave() { }
		public virtual bool CanDrag(Point point, ILayoutElement element) {
			return (element != null);
		}
		public virtual bool CanDrop(Point point, ILayoutElement element) { return false; }
		public virtual void OnInitialize(Point point, ILayoutElement element) { }
		public virtual void OnBegin(Point point, ILayoutElement element) { }
		public virtual void OnDragging(Point point, ILayoutElement element) { }
		public virtual void OnDrop(Point point, ILayoutElement element) { }
		public virtual void OnCancel() { }
		public virtual void OnComplete() { }
		protected static bool IsRoot(ILayoutElement element) {
			return (element != null) && (element.Parent == null);
		}
	}
	public class RegularListener : BaseDragListener {
		public sealed override OperationType OperationType {
			get { return OperationType.Regular; }
		}
	}
	public class FloatingListener : BaseDragListener {
		public sealed override OperationType OperationType {
			get { return OperationType.Floating; }
		}
		public override void OnEnter() {
			IView view = ServiceProvider as IView;
			InitFloating(view.Adapter.DragService.DragItem);
		}
		public override void OnBegin(Point point, ILayoutElement element) {
			IView view = (IView)ServiceProvider;
			if(view.Type == HostType.AutoHide)
				view.Adapter.ActionService.Hide(view, true);
			InitFloating(element);
		}
		void InitFloating(ILayoutElement element) {
			Rect itemScreenRect = ElementHelper.GetScreenRect((ILayoutElementHost)ServiceProvider, element);
			Rect itemContainerScreenRect = (element.Container == null) ? Rect.Empty :
				ElementHelper.GetScreenRect((ILayoutElementHost)ServiceProvider, element.Container);
			IView floatingView = GetFloatingView(element);
			if(floatingView != null) {
				floatingView.EnsureLayoutRoot();
				InitFloatingView(floatingView, itemScreenRect, itemContainerScreenRect);
				floatingView.Adapter.DragService.DragItem = floatingView.LayoutRoot;
			}
		}
		protected virtual IView GetFloatingView(ILayoutElement element) {
			return null;
		}
		protected virtual void InitFloatingView(IView floatingView, Rect itemScreenRect, Rect itemContainerScreenRect) { }
	}
	public class ResizingListener : BaseDragListener {
		public sealed override OperationType OperationType {
			get { return OperationType.Resizing; }
		}
	}
	public class NonClientDraggingListener : BaseDragListener {
		public sealed override OperationType OperationType {
			get { return OperationType.NonClientDragging; }
		}
	}
	public class ClientDraggingListener : BaseDragListener {
		public sealed override OperationType OperationType {
			get { return OperationType.ClientDragging; }
		}
		public override bool CanDrag(Point point, ILayoutElement element) {
			return (element != null) && (element.Parent != null);
		}
	}
	public class ReorderingListener : BaseDragListener {
		public sealed override OperationType OperationType {
			get { return OperationType.Reordering; }
		}
		public override bool CanDrag(Point point, ILayoutElement element) {
			return (element != null) && (element.Parent != null);
		}
	}
	public class FloatingMovingListener : BaseDragListener {
		public sealed override OperationType OperationType {
			get { return OperationType.FloatingMoving; }
		}
		public override bool CanDrag(Point point, ILayoutElement element) {
			return (element != null) &&
					IsRoot(element) ||
					(IsRoot(element.Container) && element.Container.Items.Count == 1);
		}
	}
	public class FloatingResizingListener : BaseDragListener {
		public sealed override OperationType OperationType {
			get { return OperationType.FloatingResizing; }
		}
		public override bool CanDrag(Point point, ILayoutElement element) {
			return (element != null) &&
					IsRoot(element) ||
					(IsRoot(element.Container) && element.Container.Items.Count == 1);
		}
	}
}
