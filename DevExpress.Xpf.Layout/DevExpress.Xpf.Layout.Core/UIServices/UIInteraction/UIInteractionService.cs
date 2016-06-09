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
namespace DevExpress.Xpf.Layout.Core.UIInteraction {
	class UIInteractionService : UIService, IUIInteractionService {
		protected override void OnDispose() {
			UnSubscribe(ActiveItem);
			activeItemCore = null;
			lastClickView = null;
			prevHotInfo = null;
			prevPressedInfo = null;
			base.OnDispose();
		}
		public void SetActiveItem(IView view, ILayoutElement element) {
			if(view == null || element == null) return;
			DoChangeActiveItem(view, element);
		}
		public void Activate(IView view) {
			if(view == null) return;
			IViewAdapter adapter = view.Adapter;
			if(adapter != null) {
				IView[] views = adapter.Views.ToArray();
				DoActivate(view);
				for(int i = 0; i < views.Length; i++) {
					IView v = views[i];
					v.InvalidateZOrder();
					if(v != view) {
						if(v.Type == HostType.AutoHide)
							adapter.ActionService.Hide(v, false);
						DoDeactivate(v);
					}
				}
			}
		}
		public void Deactivate(IView view) {
			if(view == null) return;
			DoDeactivate(view);
		}
		protected override bool ProcessMouseOverride(IView view, Platform.MouseEventType eventType, Platform.MouseEventArgs ea) {
			IViewAdapter adapter = view.Adapter;
			if(adapter != null) {
				if(eventType == Platform.MouseEventType.MouseLeave)
					return ProcessMouseLeave();
				LayoutElementHitInfo hitInfo = adapter.CalcHitInfo(view, ea.Point);
				if(eventType == Platform.MouseEventType.MouseDown) {
					if(!hitInfo.InControlBox) {
						Activate(Sender);
						SetActiveItem(Sender, hitInfo.Element);
						hitInfo = adapter.CalcHitInfo(view, ea.Point);
					}
				}
				ILayoutElement actionTarget = hitInfo.Element;
				if(CanProcessUIInteraction(actionTarget)) {
					bool result = false;
					switch(eventType) {
						case Platform.MouseEventType.MouseDown: result = ProcessMouseDown(hitInfo, ea); break;
						case Platform.MouseEventType.MouseUp: result = ProcessMouseUp(hitInfo, ea); break;
						case Platform.MouseEventType.MouseMove: result = ProcessMouseMove(hitInfo, ea); break;
					}
					if(result) {
						adapter.DragService.Reset();
					}
					return result;
				}
			}
			return false;
		}
		protected bool CanProcessUIInteraction(ILayoutElement element) {
			return (element != null) && !element.IsDisposing;
		}
		protected bool ProcessMouseDown(LayoutElementHitInfo downHitInfo, Platform.MouseEventArgs ea) {
			CheckResetPrevPressedElement(downHitInfo);
			if(ea.Buttons == Platform.MouseButtons.Left) {
				SetElementPressedInfo(downHitInfo.Element,
						downHitInfo.IsPressed ? downHitInfo : LayoutElementHitInfo.Empty
					);
			}
			if(ea.Buttons == Platform.MouseButtons.Middle) {
				DoMiddleButtonClickAction(downHitInfo);
			}
			return false;
		}
		protected bool ProcessMouseMove(LayoutElementHitInfo moveInfo, Platform.MouseEventArgs ea) {
			CheckResetPrevHotElement(moveInfo);
			SetElementHotTrackedInfo(moveInfo.Element,
					moveInfo.IsHot ? moveInfo : LayoutElementHitInfo.Empty
				);
			return false;
		}
		protected bool ProcessMouseUp(LayoutElementHitInfo upHitInfo, Platform.MouseEventArgs ea) {
			CheckResetPrevPressedElement(upHitInfo);
			bool result = false;
			bool wasDisposing = false;
			if(ea.Buttons == Platform.MouseButtons.None && (upHitInfo.Element != null)) {
				if(ea.ChangedButtons == Platform.MouseButtons.Left) {
					if(upHitInfo.InClickPreviewBounds)
						DoClickPreviewAction(upHitInfo);
					if(upHitInfo.InClickBounds || upHitInfo.InDoubleClickBounds) {
						result = CheckClick(upHitInfo);
					}
				}
				if(ea.ChangedButtons == Platform.MouseButtons.Right) {
					if(upHitInfo.InMenuBounds) {
						result = DoMenuAction(upHitInfo);
						if(result)
							ea.Handled = true;
					}
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
			}
			return result;
		}
		protected bool ProcessMouseLeave() {
			CheckResetPrevHotElement(LayoutElementHitInfo.Empty);
			return false;
		}
		const long tickLength = 0x2710L;
		bool fWaitForSecondClick;
		WeakReference lastClickView = new WeakReference(null);
		System.Windows.Point lastClickPoint;
		long lastClickTime;
		bool CheckClick(LayoutElementHitInfo hi) {
			if(fWaitForSecondClick) {
				if(hi.InDoubleClickBounds && IsDoubleClick(hi)) {
					DoDoubleClickAction(hi);
					fWaitForSecondClick = false;
					return true;
				}
				else SaveFirstClickInfo(hi);
			}
			else SaveFirstClickInfo(hi);
			bool isClick = (hi.Element != null) && (hi.Element.GetState(hi.HitResult) & State.Pressed) != 0;
			if(isClick && hi.InClickBounds) {
				if(hi.Element != null)
					hi.Element.ResetState();
				if(DoClickAction(hi) || !hi.InDoubleClickBounds) {
					fWaitForSecondClick = false;
					return true;
				}
			}
			return false;
		}
		bool IsDoubleClick(LayoutElementHitInfo hi) {
			return (Sender == lastClickView.Target)
				&& CheckLocation(hi.HitPoint)
				&& CheckTime(System.DateTime.Now.Ticks);
		}
		bool CheckLocation(System.Windows.Point point) {
			bool conditionX = Math.Abs(point.X - lastClickPoint.X) <= Platform.SystemInformation.DoubleClickWidth;
			bool conditionY = Math.Abs(point.Y - lastClickPoint.Y) <= Platform.SystemInformation.DoubleClickHeight;
			return conditionX && conditionY;
		}
		bool CheckTime(long time) {
			return (Math.Abs(lastClickTime - time) / tickLength) <= Platform.SystemInformation.DoubleClickTime;
		}
		void SaveFirstClickInfo(LayoutElementHitInfo hi) {
			lastClickPoint = hi.HitPoint;
			lastClickView = new WeakReference(Sender);
			lastClickTime = System.DateTime.Now.Ticks;
			fWaitForSecondClick = true;
		}
		protected void DoActivate(IView view) {
			IUIInteractionServiceListener listener = view.GetUIServiceListener<IUIInteractionServiceListener>();
			if(listener != null)
				listener.OnActivate();
		}
		protected void DoDeactivate(IView view) {
			IUIInteractionServiceListener listener = view.GetUIServiceListener<IUIInteractionServiceListener>();
			if(listener != null)
				listener.OnDeactivate();
		}
		ILayoutElement activeItemCore;
		public ILayoutElement ActiveItem { 
			get { return activeItemCore; }
		}
		protected void DoChangeActiveItem(IView view, ILayoutElement element) {
			IUIInteractionServiceListener listener = view.GetUIServiceListener<IUIInteractionServiceListener>();
			if(listener != null) {
				if(ActiveItem == element && ActiveItem.IsActive) return;
				if(listener.OnActiveItemChanging(element)) {
					UnSubscribe(ActiveItem);
					if(listener.OnActiveItemChanged(element)) {
						if(element != null && element.IsDisposing)
							element = null;
						activeItemCore = element;
						Subscribe(ActiveItem);
					}
					else
						activeItemCore = null;
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
			ILayoutElement item = sender as ILayoutElement;
			if(item == null) item = ActiveItem;
			UnSubscribe(item);
			if(prevHotInfo != null && prevHotInfo.Element == item)
				prevHotInfo = null;
			if(prevPressedInfo != null && prevPressedInfo.Element == item)
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
		protected bool DoMiddleButtonClickAction(LayoutElementHitInfo clickInfo) {
			IUIInteractionServiceListener listener = Sender.GetUIServiceListener<IUIInteractionServiceListener>();
			if(listener != null)
				return listener.OnMiddleButtonClickAction(clickInfo);
			return false;
		}
		LayoutElementHitInfo prevHotInfo, prevPressedInfo;
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
			if(prevPressedInfo != null && ActiveItem == null) {
				ILayoutElement pressedItem = prevPressedInfo.Element;
				if(pressedItem != null)
					pressedItem.Disposed += OnActiveItemDisposed;
			}
		}
		void SetElementHotTrackedInfo(ILayoutElement element, LayoutElementHitInfo hitInfo) {
			if(element != null) element.SetState(hitInfo.HitResult, State.Hot);
		}
		void SetElementPressedInfo(ILayoutElement element, LayoutElementHitInfo hitInfo) {
			if(element != null) element.SetState(hitInfo.HitResult, State.Pressed);
		}
	}
}
