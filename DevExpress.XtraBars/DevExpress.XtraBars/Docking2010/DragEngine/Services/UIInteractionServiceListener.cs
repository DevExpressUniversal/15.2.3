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
namespace DevExpress.XtraBars.Docking2010.DragEngine {
	public class UIInteractionServiceListener : IUIInteractionServiceListener {
		IUIServiceProvider serviceProviderCore;
		public IUIServiceProvider ServiceProvider {
			get { return serviceProviderCore; }
			set { serviceProviderCore = value; }
		}
		public object Key { get { return typeof(IUIInteractionServiceListener); } }
		public virtual bool OnActiveItemChanging(ILayoutElement element) { return true; }
		public virtual bool OnActiveItemChanged(ILayoutElement element) { return true; }
		public virtual void OnActivate() { }
		public virtual void OnDeactivate() { }
		public virtual bool OnClickPreviewAction(LayoutElementHitInfo clickInfo) { return false; }
		public virtual bool OnClickAction(LayoutElementHitInfo clickInfo) { return false; }
		public virtual bool OnDoubleClickAction(LayoutElementHitInfo clickInfo) {
			return ToggleStateOnDoubleClick(clickInfo.Element);
		}
		public virtual bool OnMenuAction(LayoutElementHitInfo clickInfo) { return false; }
		public virtual bool OnMouseDown(MouseButtons buttons, LayoutElementHitInfo hitInfo) { return false; }
		public virtual void OnMouseMove(MouseButtons buttons, LayoutElementHitInfo hitInfo) { }
		public virtual void OnMouseLeave(LayoutElementHitInfo hitInfo) { }
		public virtual void OnMouseWheel(MouseEventArgs ea, LayoutElementHitInfo hitInfo) { }
		public virtual bool OnMouseUp(MouseButtons buttons, LayoutElementHitInfo hitInfo) { return false; }
		public virtual bool OnFlick(Point point, DevExpress.Utils.Gesture.FlickGestureArgs args, LayoutElementHitInfo hitInfo) { return false; }
		public virtual bool OnGesture(GestureID gid, DevExpress.Utils.Gesture.GestureArgs args, object[] parameters, LayoutElementHitInfo hitInfo) { return false; }
		protected bool ToggleStateOnDoubleClick(ILayoutElement element) {
			if(element == null) return false;
			bool isFloatingElement = IsFloatingElement(element);
			if((isFloatingElement && CanMaximizeOrRestore(element)) || IsMDIDocument(element)) {
				return IsMaximized(element) ? RestoreElementOnDoubleClick(element) : MaximizeElementOnDoubleClick(element);
			}
			else {
				if(IsControlItemElement(element))
					return DoControlItemDoubleClick(element);
			}
			BaseUIView baseView = ServiceProvider as BaseUIView;
			if(baseView != null) {
				baseView.doubleClickFloatingCore++;
				try {
					bool fSuspend = isFloatingElement ? baseView.CanSuspendDocking(element) :
						baseView.CanSuspendFloating(element);
					if(fSuspend) return false;
				}
				finally { baseView.doubleClickFloatingCore--; }
			}
			return isFloatingElement ? DockElementOnDoubleClick(element) : FloatElementOnDoubleClick(element);
		}
		protected virtual bool MaximizeElementOnDoubleClick(ILayoutElement element) { return false; }
		protected virtual bool RestoreElementOnDoubleClick(ILayoutElement element) { return false; }
		protected virtual bool DockElementOnDoubleClick(ILayoutElement element) {
			return false;
		}
		protected virtual bool DoControlItemDoubleClick(ILayoutElement element) {
			return false;
		}
		protected virtual bool FloatElementOnDoubleClick(ILayoutElement element) {
			Rectangle itemScreenRect = ElementHelper.GetScreenRect((ILayoutElementHost)ServiceProvider, element);
			Rectangle itemContainerScreenRect = (element.Container == null) ? Rectangle.Empty :
				ElementHelper.GetScreenRect((ILayoutElementHost)ServiceProvider, element.Container);
			if(!CanFloatOnDoubleClick(element)) return false;
			IUIView floatingView = GetFloatingView(element);
			if(floatingView != null) {
				floatingView.EnsureLayoutRoot();
				InitFloatingView(floatingView, itemScreenRect, itemContainerScreenRect);
			}
			return floatingView != null;
		}
		protected bool CanFloatOnDoubleClick(ILayoutElement element) {
			var info = Dragging.InfoHelper.GetBaseDocumentInfo(element);
			if(info != null) {
				var document = info.BaseDocument;
				if(document == null) return true;
				return document.CanFloatOnDoubleClick();
			}
			return true;
		}
		protected virtual bool CanMaximizeOrRestore(ILayoutElement element) { return false; }
		protected virtual bool IsMaximized(ILayoutElement element) { return false; }
		protected virtual bool IsFloatingElement(ILayoutElement element) { return false; }
		protected virtual bool IsControlItemElement(ILayoutElement element) { return false; }
		protected virtual bool IsMDIDocument(ILayoutElement element) { return false; }
		protected virtual IUIView GetFloatingView(ILayoutElement element) { return null; }
		protected virtual void InitFloatingView(IUIView floatingView, Rectangle itemScreenRect, Rectangle itemContainerScreenRect) { }
		protected DevExpress.Utils.DXMouseEventArgs GetArgs(Point pt) {
			return GetArgs(Control.MouseButtons, pt);
		}
		protected DevExpress.Utils.DXMouseEventArgs GetArgs(MouseButtons buttons, Point pt) {
			return GetArgs(buttons, pt, false);
		}
		protected static DevExpress.Utils.DXMouseEventArgs GetArgs(MouseButtons buttons, Point pt, bool handled) {
			return new DevExpress.Utils.DXMouseEventArgs(buttons, 0, pt.X, pt.Y, 0, handled);
		}
	}
}
