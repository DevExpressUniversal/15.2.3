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
namespace DevExpress.XtraBars.Docking2010.DragEngine {
	class UIInteractionService : UIService, IUIInteractionService {
		protected override void ResetCore() {
			UnSubscribe(ActiveItem);
			activeItemCore = null;
			lastClickView = null;
			prevHotInfo = null;
			prevPressedInfo = null;
			SuspendTabMouseActivation = false;
		}
		public void CancelUIInteractionOperation() {
			fWaitForSecondClick = false;
			lastClickPoint = InvalidPoint;
			lastClickView = null;
		}
		public void SetActiveItem(IUIView view, ILayoutElement element) {
			if(view == null || element == null) return;
			if(DoValidate(view))
				DoChangeActiveItem(view, element);
		}
		public bool ValidationCancelled { get; set; }
		public bool DoValidate(IUIView view) {
			if(view == null || !(view is BaseUIView)) return true;
			ValidationCancelled = !((BaseUIView)view).DoValidate();
			return !ValidationCancelled;
		}
		public bool SuspendTabMouseActivation { get; set; }
		public bool CheckSuspendTabMouseActivation(IUIView view, ILayoutElement element) {
			if(view == null || !(view is BaseUIView)) return true;
			SuspendTabMouseActivation = ((BaseUIView)view).CanSuspendTabMouseActivation(element);
			return !SuspendTabMouseActivation;
		}
		public void Activate(IUIView view) {
			if(view == null) return;
			IUIViewAdapter adapter = view.Adapter;
			if(adapter != null) {
				IUIView[] views = adapter.Views.ToArray();
				DoActivate(view);
				for(int i = 0; i < views.Length; i++) {
					IUIView v = views[i];
					v.InvalidateZHandle();
					if(v != view) {
						DoDeactivate(v);
					}
				}
			}
		}
		public void Deactivate(IUIView view) {
			if(view == null) return;
			DoDeactivate(view);
		}
		protected override bool ProcessKeyOverride(IUIView view, KeyEventType eventype, Keys key) {
			return false;
		}
		bool inDragging = false;
		protected override bool ProcessMouseOverride(IUIView view, MouseEventType eventType, MouseEventArgs ea) {
			ValidationCancelled = false;
			IUIViewAdapter adapter = view.Adapter;
			if(adapter != null) {
				inDragging = adapter.DragService.DragItem != null;
				if(eventType == MouseEventType.MouseLeave)
					return ProcessMouseLeave();
				LayoutElementHitInfo hitInfo = adapter.CalcHitInfo(view, ea.Location);
				bool boundsChangedOnActivating = false;
				if(eventType == MouseEventType.MouseDown) {
					SuspendTabMouseActivation = false;
					if(!hitInfo.InControlBox && (ea.Button & MouseButtons.Middle) == 0) {
						Rectangle oldBounds = view.LayoutRoot.Bounds;
						Activate(Sender);
						if(CheckSuspendTabMouseActivation(Sender, hitInfo.Element)) {
							SetActiveItem(Sender, hitInfo.Element);
							hitInfo = adapter.CalcHitInfo(view, ea.Location);
							if(oldBounds != view.LayoutRoot.Bounds)
								boundsChangedOnActivating = true;
						}
					}
				}
				ILayoutElement actionTarget = hitInfo.Element;
				if(CanProcessUIInteraction(actionTarget)) {
					bool result = false;
					switch(eventType) {
						case MouseEventType.MouseDown: result = ProcessMouseDown(hitInfo, ea); break;
						case MouseEventType.MouseUp: result = ProcessMouseUp(hitInfo, ea); break;
						case MouseEventType.MouseMove: result = ProcessMouseMove(hitInfo, ea); break;
						case MouseEventType.MouseWheel: result = ProcessMouseWheel(hitInfo, ea); break;
					}
					result |= boundsChangedOnActivating;
					if(result) {
						adapter.DragService.CancelDragOperation();
					}
					return result;
				}
				else {
					if(eventType == MouseEventType.MouseUp)
						CheckResetPrevHandlerDownInfo(hitInfo, ea);
				}
			}
			return false;
		}
		protected override bool ProcessFlickOverride(IUIView view, Point point, DevExpress.Utils.Gesture.FlickGestureArgs args) {
			IUIViewAdapter adapter = view.Adapter;
			if(adapter != null) {
				LayoutElementHitInfo hitInfo = adapter.CalcHitInfo(view, point);
				if(CanProcessUIInteraction(hitInfo.Element)) 
					return ProcessFlick(point, hitInfo, args);
			}
			return false;
		}
		protected override bool ProcessGestureOverride(IUIView view, GestureID gid, DevExpress.Utils.Gesture.GestureArgs args, object[] parameters) {
			IUIViewAdapter adapter = view.Adapter;
			if(adapter != null) {
				Point point;
				switch(gid) {
					case GestureID.QueryAllowGesture:
						point = (Point)parameters[0];
						break;
					case GestureID.Begin:
						point = args.Start.Point;
						break;
					default:
						point = args.Current.Point;
						break;
				}
				LayoutElementHitInfo hitInfo = adapter.CalcHitInfo(view, point);
				if(gid == GestureID.Begin)
					beginHandlerGestureInfo = hitInfo.InHandlerBounds ? hitInfo : null;
				if(args.IsEnd && beginHandlerGestureInfo != null) {
					CheckResetBeginHandlerGestureInfo(hitInfo, gid, args, parameters);
				}
				if(CanProcessUIInteraction(hitInfo.Element))
					return ProcessGesture(hitInfo, gid, args, parameters);
			}
			return false;
		}
		protected bool CanProcessUIInteraction(ILayoutElement element) {
			return (element != null) && !element.IsDisposing;
		}
		protected bool ProcessMouseDown(LayoutElementHitInfo downHitInfo, MouseEventArgs ea) {
			CheckResetPrevPressedElement(downHitInfo);
			if(ea.Button == MouseButtons.Left) {
				SetElementPressedInfo(downHitInfo.Element,
						downHitInfo.IsPressed ? downHitInfo : LayoutElementHitInfo.Empty
					);
				prevHandlerDownInfo = downHitInfo.InHandlerBounds ? downHitInfo : null;
			}
			if(downHitInfo.InHandlerBounds)
				return DoMouseDown(ea.Button, downHitInfo);
			return false;
		}
		protected bool ProcessMouseMove(LayoutElementHitInfo moveInfo, MouseEventArgs ea) {
			CheckResetPrevHandlerMoveInfo(moveInfo, ea);
			CheckResetPrevHotElement(moveInfo);
			if(!inDragging) {
				SetElementHotTrackedInfo(moveInfo.Element,
					moveInfo.IsHot ? moveInfo : LayoutElementHitInfo.Empty
				);
			}
			return false;
		}
		protected bool ProcessMouseUp(LayoutElementHitInfo upHitInfo, MouseEventArgs ea) {
			CheckResetPrevHandlerDownInfo(upHitInfo, ea);
			CheckResetPrevPressedElement(upHitInfo);
			bool result = false;
			bool wasDisposing = false;
			if(ea.Button != MouseButtons.None && (upHitInfo.Element != null)) {
				if(ea.Button == MouseButtons.Left) {
					if(upHitInfo.InClickPreviewBounds)
						DoClickPreviewAction(upHitInfo);
					if(upHitInfo.InClickBounds || upHitInfo.InDoubleClickBounds) {
						result = CheckClick(upHitInfo);
					}
				}
				if(ea.Button == MouseButtons.Right) {
					if(upHitInfo.InMenuBounds)
						result = DoMenuAction(upHitInfo);
				}
				wasDisposing = upHitInfo.Element.IsDisposing;
				if(result) {
					prevHotInfo = prevPressedInfo = null;
				}
			}
			else fWaitForSecondClick = false;
			if(!wasDisposing) {
				SetElementPressedInfo(upHitInfo.Element, LayoutElementHitInfo.Empty);
				if(result && upHitInfo.InControlBox)
					SetActiveItem(Sender, upHitInfo.Element);
				else {
					if(upHitInfo.InHandlerBounds)
						DoMouseUp(ea.Button, upHitInfo);
				}
			}
			return result;
		}
		protected bool ProcessMouseWheel(LayoutElementHitInfo hitInfo, MouseEventArgs ea) {
			CheckResetPrevPressedElement(hitInfo);
			if(hitInfo.InBounds)
				DoMouseWheel(ea, hitInfo);
			return false;
		}
		protected bool ProcessMouseLeave() {
			CheckResetPrevHandlerMoveInfo(LayoutElementHitInfo.Empty, null);
			CheckResetPrevHotElement(LayoutElementHitInfo.Empty);
			return false;
		}
		protected bool ProcessFlick(Point point, LayoutElementHitInfo hitInfo, DevExpress.Utils.Gesture.FlickGestureArgs args) {
			CheckResetPrevPressedElement(hitInfo);
			return hitInfo.InBounds && DoFlick(point, hitInfo, args);
		}
		protected bool ProcessGesture(LayoutElementHitInfo hitInfo, GestureID gid, DevExpress.Utils.Gesture.GestureArgs args, object[] parameters) {
			CheckResetPrevPressedElement(hitInfo);
			return DoGesture(gid, args, parameters, hitInfo);
		}
		protected override Keys[] GetKeys() {
			return new Keys[] { };
		}
		const long tickLength = 0x2710L;
		bool fWaitForSecondClick;
		IUIView lastClickView;
		Point lastClickPoint;
		long lastClickTime;
		bool CheckClick(LayoutElementHitInfo hi) {
			if(fWaitForSecondClick) {
				if(hi.InDoubleClickBounds && IsDoubleClick(hi)) {
					fWaitForSecondClick = false;
					return DoDoubleClickAction(hi);
				}
				else SaveFirstClickInfo(hi);
			}
			else SaveFirstClickInfo(hi);
			bool isClick = (hi.Element != null) && (hi.Element.GetState(hi.HitResult) & State.Pressed) != 0;
			if(isClick && hi.InClickBounds && DoClickAction(hi)) {
				if(hi.Element != null)
					hi.Element.ResetState();
				fWaitForSecondClick = false;
				return true;
			}
			return false;
		}
		bool IsDoubleClick(LayoutElementHitInfo hi) {
			return (Sender == lastClickView)
				&& CheckLocation(hi.HitPoint)
				&& CheckTime(System.DateTime.Now.Ticks);
		}
		bool CheckLocation(Point point) {
			Size doubleClickSize = SystemInformation.DoubleClickSize;
			bool conditionX = Math.Abs(point.X - lastClickPoint.X) <= doubleClickSize.Width;
			bool conditionY = Math.Abs(point.Y - lastClickPoint.Y) <= doubleClickSize.Height;
			return conditionX && conditionY;
		}
		bool CheckTime(long time) {
			return (Math.Abs(lastClickTime - time) / tickLength) <= SystemInformation.DoubleClickTime;
		}
		void SaveFirstClickInfo(LayoutElementHitInfo hi) {
			lastClickPoint = hi.HitPoint;
			lastClickView = Sender;
			lastClickTime = System.DateTime.Now.Ticks;
			fWaitForSecondClick = true;
		}
		protected void DoActivate(IUIView view) {
			IUIInteractionServiceListener listener = view.GetUIServiceListener<IUIInteractionServiceListener>();
			if(listener != null)
				listener.OnActivate();
		}
		protected void DoDeactivate(IUIView view) {
			IUIInteractionServiceListener listener = view.GetUIServiceListener<IUIInteractionServiceListener>();
			if(listener != null)
				listener.OnDeactivate();
		}
		ILayoutElement activeItemCore;
		public ILayoutElement ActiveItem {
			get { return activeItemCore; }
		}
		protected void DoChangeActiveItem(IUIView view, ILayoutElement element) {
			IUIInteractionServiceListener listener = view.GetUIServiceListener<IUIInteractionServiceListener>();
			if(listener != null) {
				if(ActiveItem == element) return;
				if(listener.OnActiveItemChanging(element)) {
					UnSubscribe(ActiveItem);
					if(listener.OnActiveItemChanged(element)) {
						if(element != null && element.IsDisposing)
							element = null;
						activeItemCore = element;
						Subscribe(ActiveItem);
					}
				}
			}
		}
		void Subscribe(ILayoutElement item) {
			if(item == null) return;
			item.Disposed += OnActiveItemDisposed;
		}
		void UnSubscribe(ILayoutElement item) {
			if(item == null) return;
			item.Disposed -= OnActiveItemDisposed;
		}
		void OnActiveItemDisposed(object sender, EventArgs e) {
			UnSubscribe(ActiveItem);
			if(prevHotInfo != null && prevHotInfo.Element == ActiveItem)
				prevHotInfo = null;
			if(prevPressedInfo != null && prevPressedInfo.Element == ActiveItem)
				prevPressedInfo = null;
			activeItemCore = null;
		}
		protected bool DoClickPreviewAction(LayoutElementHitInfo clickInfo) {
			IUIInteractionServiceListener listener = Sender.GetUIServiceListener<IUIInteractionServiceListener>();
			if(listener != null)
				return listener.OnClickPreviewAction(clickInfo);
			return false;
		}
		protected bool DoClickAction(LayoutElementHitInfo clickInfo) {
			IUIInteractionServiceListener listener = Sender.GetUIServiceListener<IUIInteractionServiceListener>();
			if(listener != null)
				return listener.OnClickAction(clickInfo);
			return false;
		}
		protected bool DoDoubleClickAction(LayoutElementHitInfo clickInfo) {
			IUIInteractionServiceListener listener = Sender.GetUIServiceListener<IUIInteractionServiceListener>();
			if(listener != null)
				return listener.OnDoubleClickAction(clickInfo);
			return false;
		}
		protected bool DoMenuAction(LayoutElementHitInfo clickInfo) {
			IUIInteractionServiceListener listener = Sender.GetUIServiceListener<IUIInteractionServiceListener>();
			if(listener != null)
				return listener.OnMenuAction(clickInfo);
			return false;
		}
		protected bool DoMouseDown(MouseButtons buttons, LayoutElementHitInfo hitInfo) {
#if DEBUGTEST
			IUIInteractionServiceListener listener = Sender.GetUIServiceListener<IUIInteractionServiceListener>(hitInfo.Element.GetType());
			if(listener == null)
				listener = Sender.GetUIServiceListener<IUIInteractionServiceListener>();
#else
			IUIInteractionServiceListener listener = Sender.GetUIServiceListener<IUIInteractionServiceListener>();
#endif
			if(listener != null)
				return listener.OnMouseDown(buttons, hitInfo);
			return false;
		}
		protected void DoMouseMove(MouseButtons buttons, LayoutElementHitInfo hitInfo) {
#if DEBUGTEST
			IUIInteractionServiceListener listener = Sender.GetUIServiceListener<IUIInteractionServiceListener>(hitInfo.Element.GetType());
			if(listener == null)
				listener = Sender.GetUIServiceListener<IUIInteractionServiceListener>();
#else
			IUIInteractionServiceListener listener = Sender.GetUIServiceListener<IUIInteractionServiceListener>();
#endif
			if(listener != null)
				listener.OnMouseMove(buttons, hitInfo);
		}
		protected void DoMouseLeave(LayoutElementHitInfo hitInfo) {
#if DEBUGTEST
			IUIInteractionServiceListener listener = Sender.GetUIServiceListener<IUIInteractionServiceListener>(hitInfo.Element.GetType());
			if(listener == null)
				listener = Sender.GetUIServiceListener<IUIInteractionServiceListener>();
#else
			IUIInteractionServiceListener listener = Sender.GetUIServiceListener<IUIInteractionServiceListener>();
#endif
			if(listener != null)
				listener.OnMouseLeave(hitInfo);
		}
		protected bool DoMouseUp(MouseButtons buttons, LayoutElementHitInfo hitInfo) {
#if DEBUGTEST
			IUIInteractionServiceListener listener = Sender.GetUIServiceListener<IUIInteractionServiceListener>(hitInfo.Element.GetType());
			if(listener == null)
				listener = Sender.GetUIServiceListener<IUIInteractionServiceListener>();
#else
			IUIInteractionServiceListener listener = Sender.GetUIServiceListener<IUIInteractionServiceListener>();
#endif
			if(listener != null)
				return listener.OnMouseUp(buttons, hitInfo);
			return false;
		}
		protected void DoMouseWheel(MouseEventArgs ea, LayoutElementHitInfo hitInfo) {
			IUIInteractionServiceListener listener = Sender.GetUIServiceListener<IUIInteractionServiceListener>();
			if(listener != null)
				listener.OnMouseWheel(ea, hitInfo);
		}
		protected bool DoFlick(Point point, LayoutElementHitInfo hitInfo, DevExpress.Utils.Gesture.FlickGestureArgs args) {
			IUIInteractionServiceListener listener = Sender.GetUIServiceListener<IUIInteractionServiceListener>();
			if(listener != null)
				return listener.OnFlick(point, args, hitInfo);
			return false;
		}
		protected bool DoGesture(GestureID gid, DevExpress.Utils.Gesture.GestureArgs args, object[] parameters, LayoutElementHitInfo hitInfo) {
			IUIInteractionServiceListener listener = Sender.GetUIServiceListener<IUIInteractionServiceListener>();
			if(listener != null)
				return listener.OnGesture(gid, args, parameters, hitInfo);
			return false;
		}
		LayoutElementHitInfo prevHotInfo, prevPressedInfo, prevHandlerMoveInfo, prevHandlerDownInfo, beginHandlerGestureInfo;
		void CheckResetPrevHandlerMoveInfo(LayoutElementHitInfo hitInfo, MouseEventArgs ea) {
			if(prevHandlerMoveInfo != null && prevHandlerMoveInfo.Element != hitInfo.Element)
				DoMouseLeave(prevHandlerMoveInfo.Patch(hitInfo.HitPoint));
			prevHandlerMoveInfo = hitInfo.InHandlerBounds ? hitInfo : null;
			if(hitInfo.InHandlerBounds)
				DoMouseMove(ea.Button, hitInfo);
		}
		void CheckResetPrevHandlerDownInfo(LayoutElementHitInfo hitInfo, MouseEventArgs ea) {
			if(prevHandlerDownInfo != null && prevHandlerDownInfo.Element != hitInfo.Element)
				DoMouseUp(ea.Button, prevHandlerDownInfo.Patch(hitInfo.HitPoint));
			prevHandlerDownInfo = null;
		}
		void CheckResetPrevHotElement(LayoutElementHitInfo hitInfo) {
			if(prevHotInfo != null && prevHotInfo.Element != hitInfo.Element) {
				SetElementHotTrackedInfo(prevHotInfo.Element, LayoutElementHitInfo.Empty);
			}
			prevHotInfo = hitInfo.IsHot ? hitInfo : null;
		}
		void CheckResetPrevPressedElement(LayoutElementHitInfo hitInfo) {
			if(prevPressedInfo != null && prevPressedInfo.Element != hitInfo.Element) {
				SetElementPressedInfo(prevPressedInfo.Element, LayoutElementHitInfo.Empty);
			}
			prevPressedInfo = hitInfo.IsPressed ? hitInfo : null;
		}
		void CheckResetBeginHandlerGestureInfo(LayoutElementHitInfo hitInfo, GestureID gid, DevExpress.Utils.Gesture.GestureArgs args, object[] parameters) {
			if(beginHandlerGestureInfo != null && beginHandlerGestureInfo.Element != hitInfo.Element)
				DoGesture(gid, args, parameters, beginHandlerGestureInfo);
			beginHandlerGestureInfo = null;
		}
		void SetElementHotTrackedInfo(ILayoutElement element, LayoutElementHitInfo hitInfo) {
			if(element != null) element.SetState(hitInfo.HitResult, State.Hot);
		}
		void SetElementPressedInfo(ILayoutElement element, LayoutElementHitInfo hitInfo) {
			if(element != null) element.SetState(hitInfo.HitResult, State.Pressed);
		}
	}
}
