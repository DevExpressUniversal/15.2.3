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
using System.Windows.Forms;
using DevExpress.Utils.Gesture;
namespace DevExpress.XtraBars.Docking2010.DragEngine {
	abstract class ViewEventSubscriber<T> : IDisposable {
		protected BaseUIView View;
		public T Source;
		protected ViewEventSubscriber(T source, BaseUIView view) {
			Source = source;
			View = view;
			if(Source != null)
				Subscribe(Source);
		}
		protected abstract void Subscribe(T element);
		protected abstract void UnSubscribe(T element);
		public void Dispose() {
			if(Source != null) {
				UnSubscribe(Source);
				Source = default(T);
			}
			View = null;
			GC.SuppressFinalize(this);
		}
	}
	class MouseEventSubscriber<T> : ViewEventSubscriber<T> , IMessageFilter
		where T : Control{
		public MouseEventSubscriber(T source, BaseUIView view)
			: base(source, view) {
		}
		T currentElement;
		protected override void Subscribe(T element) {
			element.MouseDown += RootUIElementMouseDown;
			element.MouseMove += RootUIElementMouseMove;
			element.MouseUp += RootUIElementMouseUp;
			element.MouseLeave += RootUIElementMouseLeave;
			if(!DevExpress.Utils.Design.DesignTimeTools.IsDesignMode)
				Application.AddMessageFilter(this);
			currentElement = element;
			IFlickGestureProvider flickGestureProvider = element as IFlickGestureProvider;
			if(flickGestureProvider != null)
				flickGestureProvider.Flick += RootUIElementFlick;
			IGestureProvider gestureProvider = element as IGestureProvider;
			if(gestureProvider != null) 
				gestureProvider.Gesture += RootUIElementGesture;
		}
		protected override void UnSubscribe(T element) {
			element.MouseDown -= RootUIElementMouseDown;
			element.MouseMove -= RootUIElementMouseMove;
			element.MouseUp -= RootUIElementMouseUp;
			element.MouseLeave -= RootUIElementMouseLeave;
			currentElement = null;
			Application.RemoveMessageFilter(this);
			IFlickGestureProvider flickGestureProvider = element as IFlickGestureProvider;
			if(flickGestureProvider != null)
				flickGestureProvider.Flick -= RootUIElementFlick;
			IGestureProvider gestureProvider = element as IGestureProvider;
			if(gestureProvider != null) 
				gestureProvider.Gesture -= RootUIElementGesture;
		}
		void RootUIElementMouseUp(object sender, MouseEventArgs e) {
			View.OnMouseUp(e);
		}
		void RootUIElementMouseMove(object sender, MouseEventArgs e) {
			View.OnMouseMove(e);
		}
		void RootUIElementMouseDown(object sender, MouseEventArgs e) {
			View.OnMouseDown(e);
		}
		void RootUIElementMouseLeave(object sender, EventArgs e) {
			View.OnMouseLeave();
		}
		bool RootUIElementFlick(Point point, FlickGestureArgs args) {
			return View.OnFlick(point, args);
		}
		void RootUIElementGesture(GestureID gid, GestureArgs args, object[] parameters) {
			View.OnGesture(gid, args, parameters);
		}
		protected virtual MouseEventArgs Preprocess(MouseEventArgs e) { return e; }
		#region IMessageFilter Members
		public bool PreFilterMessage(ref Message m) {
			if(m.Msg == DevExpress.Utils.Drawing.Helpers.MSG.WM_MOUSEWHEEL && currentElement != null)
				View.OnMouseWheel(WinAPIHelper.GetMouseArgs(currentElement.Handle, m));
			return false;
		}
		#endregion
	}
	class KeyboardEventSubscriber : ViewEventSubscriber<Control> {
		public KeyboardEventSubscriber(Control rootUIElement, BaseUIView view)
			: base(rootUIElement, view) {
		}
		protected override void Subscribe(Control element) {
			element.KeyDown += RootUIElementKeyDown;
			element.KeyUp += RootUIElementKeyUp;
		}
		protected override void UnSubscribe(Control element) {
			element.KeyDown -= RootUIElementKeyDown;
			element.KeyUp -= RootUIElementKeyUp;
		}
		void RootUIElementKeyDown(object sender, KeyEventArgs e) {
			View.OnKeyDown(e.KeyCode);
			CheckTabKey(e);
		}
		void RootUIElementKeyUp(object sender, KeyEventArgs e) {
			View.OnKeyUp(e.KeyCode);
			CheckTabKey(e);
		}
		void CheckTabKey(KeyEventArgs e) {
			if(View != null && View.Adapter.DragService.OperationType != OperationType.Regular)
				if(e.KeyCode == Keys.Tab) e.Handled = true;
		}
	}
}
