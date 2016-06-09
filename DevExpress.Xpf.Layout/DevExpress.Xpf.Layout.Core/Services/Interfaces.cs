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
using DevExpress.Xpf.Layout.Core.Platform;
namespace DevExpress.Xpf.Layout.Core {
	public interface IUIService : IDisposable {
		bool ProcessMouse(IView view, MouseEventType eventType, MouseEventArgs ea);
		bool ProcessKey(IView view, KeyEventType eventype, System.Windows.Input.Key key);
	}
	public interface IUIServiceListener {
		object Key { get; }
		IUIServiceProvider ServiceProvider { get; set; }
	}
	public interface IUIServiceProvider {
		void RegisterUIServiceListener(IUIServiceListener listener);
		ServiceListener GetUIServiceListener<ServiceListener>(object key) where ServiceListener : class, IUIServiceListener;
		ServiceListener GetUIServiceListener<ServiceListener>() where ServiceListener : class, IUIServiceListener;
	}
	public interface IDragService : IUIService {
		ILayoutElement DragItem { get; set; }
		IView DragSource { get; set; }
		Point DragOrigin { get; set; }
		bool SuspendBehindDragging { get; set; }
		OperationType OperationType { get; }
		void SetState(OperationType type);
		void Reset();
	}
	public interface IDragServiceListener : IUIServiceListener {
		OperationType OperationType { get; }
		void OnEnter();
		void OnLeave();
		bool CanDrag(Point point, ILayoutElement element);
		bool CanDrop(Point point, ILayoutElement element);
		void OnInitialize(Point point, ILayoutElement element);
		void OnBegin(Point point, ILayoutElement element);
		void OnDragging(Point point, ILayoutElement element);
		void OnDrop(Point point, ILayoutElement element);
		void OnCancel();
		void OnComplete();
	}
	public interface IDragServiceStateFactory {
		IDragServiceState Create(IDragService service, OperationType operationType);
	}
	public interface IDragServiceState {
		OperationType OperationType { get; }
		void ProcessMouseDown(IView view, Point point);
		void ProcessMouseUp(IView view, Point point);
		void ProcessMouseMove(IView view, Point point);
		void ProcessKeyDown(IView view, System.Windows.Input.Key key);
		void ProcessKeyUp(IView view, System.Windows.Input.Key key);
		void ProcessCancel(IView view);
		void ProcessComplete(IView view);
	}
	public interface ISelectionService : IUIService {
		bool GetSelectedState(ILayoutElement element);
		void Select(IView view, ILayoutElement element, SelectionMode mode);
		void ClearSelection(IView view);
		bool ExtendSelectionToParent(IView view);
	}
	public interface ISelectionServiceListener : IUIServiceListener {
		SelectionMode CheckMode(ILayoutElement item);
		bool OnSelectionChanging(ILayoutElement element, bool selected);
		void OnSelectionChanged(ILayoutElement element, bool selected);
		ILayoutElement[] OnRequestSelectionRange(ILayoutElement first, ILayoutElement last);
	}
	public interface IUIInteractionService : IUIService {
		void Activate(IView view);
		void Deactivate(IView view);
		void SetActiveItem(IView view, ILayoutElement element);
	}
	public interface IUIInteractionServiceListener : IUIServiceListener {
		void OnActivate();
		void OnDeactivate();
		bool OnActiveItemChanging(ILayoutElement element);
		bool OnActiveItemChanged(ILayoutElement element);
		bool OnClickPreviewAction(LayoutElementHitInfo clickInfo);
		bool OnClickAction(LayoutElementHitInfo clickInfo);
		bool OnDoubleClickAction(LayoutElementHitInfo clickInfo);
		bool OnMenuAction(LayoutElementHitInfo clickInfo);
		bool OnMiddleButtonClickAction(LayoutElementHitInfo clickInfo);
	}
	public interface IActionService : IUIService {
		void Hide(IView view, bool immediately);
		void Expand(IView view);
		void Collapse(IView view);
		void ShowSelection(IView view);
		void HideSelection(IView view);
	}
	public interface IActionServiceListener : IUIServiceListener {
		void OnHide(bool immediately);
		void OnExpand();
		void OnCollapse();
		void OnShowSelection();
		void OnHideSelection();
	}
	public interface IContextActionService : IUIService {
		bool DoContextAction(IView view, ILayoutElement element, ContextAction action);
	}
	public interface IContextActionServiceListener : IUIServiceListener {
		bool OnContextAction(ILayoutElement element, ContextAction action);
	}
}
