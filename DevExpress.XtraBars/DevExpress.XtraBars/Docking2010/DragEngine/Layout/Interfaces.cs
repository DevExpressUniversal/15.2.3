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
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.XtraBars.Docking2010.Base;
namespace DevExpress.XtraBars.Docking2010.DragEngine {
	public interface IUIElement {
		IUIElement Scope { get; }
		UIChildren Children { get; }
	}
	public class UIChildren {
		List<IUIElement> elements;
		public UIChildren() {
			elements = new List<IUIElement>();
		}
		public void Add(IUIElement element) {
			if(!elements.Contains(element))
				elements.Add(element);
		}
		public bool Remove(IUIElement element) {
			return elements.Remove(element);
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
	}
	public interface ILayoutElement : DevExpress.Utils.Base.IBaseObject, ISupportHierarchy<ILayoutElement> {
		ILayoutContainer Container { get; }
		Point Location { get; }
		Size Size { get; }
		Rectangle Bounds { get; }
		bool IsReady { get; }
		bool EnsureBounds();
		void Invalidate();
		bool HitTestingEnabled { get; }
		bool HitTest(Point pt);
		LayoutElementHitInfo CalcHitInfo(Point pt);
		State GetState(object hitResult);
		void SetState(object hitResult, State state);
		void ResetState();
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
	public interface ILayoutElementHost : DevExpress.Utils.Base.IBaseObject {
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
	public interface IUIViewAdapter : ILayoutElementHostAdapter {
		UIViewCollection Views { get; }
		IUIView GetView(object rootKey);
		IUIView GetView(ILayoutElement element);
		IUIView GetView(Point screenPoint);
		IUIView GetBehindView(IUIView sourceView, Point screenPoint);
		IDragService DragService { get; }
		IUIInteractionService UIInteractionService { get; }
		bool IsInEvent { get; }
		void ProcessMouseEvent(IUIView view, MouseEventType eventype, MouseEventArgs ea);
		void ProcessKey(IUIView view, KeyEventType eventype, Keys key);
		bool ProcessFlickEvent(IUIView view, Point point, DevExpress.Utils.Gesture.FlickGestureArgs args);
		void ProcessGesture(IUIView view, GestureID gid, DevExpress.Utils.Gesture.GestureArgs args, object[] parameters);
	}
	public interface ILayoutElementBehavior {
		bool AllowDragging { get; }
		bool CanDrag(OperationType operation);
	}
	public interface IUIView : ILayoutElementHost, IUIServiceProvider {
		IUIViewAdapter Adapter { get; }
		void OnKeyDown(Keys key);
		void OnKeyUp(Keys key);
		void OnMouseDown(MouseEventArgs ea);
		void OnMouseUp(MouseEventArgs ea);
		void OnMouseMove(MouseEventArgs ea);
		void OnMouseWheel(MouseEventArgs ea);
		void OnMouseLeave();
		bool OnFlick(Point point, DevExpress.Utils.Gesture.FlickGestureArgs args);
		IntPtr ZHandle { get; }
		void InvalidateZHandle();
	}
}
