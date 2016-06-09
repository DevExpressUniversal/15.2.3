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
using System.Windows.Forms;
using System.Drawing;
namespace DevExpress.XtraBars.Docking2010.DragEngine {
	public interface IUIService {
		bool ProcessMouse(IUIView view, MouseEventType eventType, MouseEventArgs ea);
		bool ProcessKey(IUIView view, KeyEventType eventype, Keys key);
		bool ProcessFlickEvent(IUIView view, Point point, DevExpress.Utils.Gesture.FlickGestureArgs args);
		bool ProcessGesture(IUIView view, GestureID gid, DevExpress.Utils.Gesture.GestureArgs args, object[] parameters);
		void Reset();
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
	public interface IUIInteractionService : IUIService {
		void Activate(IUIView view);
		void Deactivate(IUIView view);
		void SetActiveItem(IUIView view, ILayoutElement element);
		void CancelUIInteractionOperation();
		bool ValidationCancelled { get; }
		bool SuspendTabMouseActivation { get; }
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
		bool OnMouseDown(MouseButtons buttons, LayoutElementHitInfo hitInfo);
		void OnMouseMove(MouseButtons buttons, LayoutElementHitInfo hitInfo);
		bool OnMouseUp(MouseButtons buttons, LayoutElementHitInfo hitInfo);
		void OnMouseLeave(LayoutElementHitInfo hitInfo);
		void OnMouseWheel(MouseEventArgs ea, LayoutElementHitInfo hitInfo);
		bool OnFlick(Point point, DevExpress.Utils.Gesture.FlickGestureArgs args, LayoutElementHitInfo hitInfo);
		bool OnGesture(GestureID gid, DevExpress.Utils.Gesture.GestureArgs args, object[] parameters, LayoutElementHitInfo hitInfo);
	}
	public interface IDragService : IUIService {
		ILayoutElement DragItem { get; set; }
		IUIView DragSource { get; set; }
		Point DragOrigin { get; set; }
		bool SuspendBehindDragging { get;}
		bool NonClientDragging { get; set; }
		OperationType OperationType { get; }
		void SetState(OperationType type);
		void CancelDragOperation();
		void ReparentDragOperation();
	}
	public interface IDragServiceStateFactory {
		IDragServiceState Create(IDragService service, OperationType operationType);
	}
	public interface IDragServiceState {
		OperationType OperationType { get; }
		void ProcessMouseDown(IUIView view, Point point);
		void ProcessMouseUp(IUIView view, Point point);
		void ProcessMouseMove(IUIView view, Point point);
		void ProcessKeyDown(IUIView view, Keys key);
		void ProcessKeyUp(IUIView view, Keys key);
		void ProcessCancel(IUIView view);
	}
	public delegate void DragOperationContextAction();
	public interface IDragOperationContext : IDisposable {
		void RegisterAction(DragOperationContextAction action);
		void Reset();
		void Flush();
	}
	public interface IDragServiceListener : IUIServiceListener {
		OperationType OperationType { get; }
		void OnEnter(Point point, ILayoutElement element);
		void OnLeave();
		bool CanDrag(Point point, ILayoutElement element);
		bool CanDrop(Point point, ILayoutElement element);
		void OnBegin(Point point, ILayoutElement element);
		void OnDragging(Point point, ILayoutElement element);
		void OnDrop(Point point, ILayoutElement element);
		void OnCancel();
	}
}
