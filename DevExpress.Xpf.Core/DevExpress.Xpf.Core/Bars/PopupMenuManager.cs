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
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.Core.Native;
using DevExpress.Utils;
using DevExpress.Mvvm.Native;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Core.HitTest;
using System.Windows.Input;
using System.Diagnostics;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Bars {
	public static class PopupMenuManager {
		public static bool GetIgnorePopupItemClickBehaviour(DependencyObject obj) { return (bool)obj.GetValue(IgnorePopupItemClickBehaviourProperty); }
		public static void SetIgnorePopupItemClickBehaviour(DependencyObject obj, bool value) { obj.SetValue(IgnorePopupItemClickBehaviourProperty, value); }
		public static readonly DependencyProperty IgnorePopupItemClickBehaviourProperty = DependencyPropertyManager.RegisterAttached("IgnorePopupItemClickBehaviour", typeof(bool), typeof(PopupMenuManager), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));
		[ThreadStatic]
		static Stack<BarPopupBase> openedPopups;
		[ThreadStatic]
		static DispatcherTimer popupTimer;
#if DEBUGTEST
		public static int? ForcedMenuShowDelay { get; set; }
#endif
		static Stack<BarPopupBase> OpenedPopups { get { return openedPopups ?? (openedPopups = InitializeOpenedPopups()); } }
		static DispatcherTimer PopupTimer { get { return popupTimer ?? (popupTimer = InitializePopupTimer()); } }
		static DispatcherTimer InitializePopupTimer() {
			var timer = new DispatcherTimer() {
#if DEBUGTEST
				Interval = TimeSpan.FromMilliseconds((double)ForcedMenuShowDelay.Return(x => x.Value, () => SystemParameters.MenuShowDelay))
#else
				Interval = TimeSpan.FromMilliseconds((double)SystemParameters.MenuShowDelay)
#endif
			};
			timer.Tick += OnShowPopupTimerTick;
			return timer;			
		}
		static Stack<BarPopupBase> InitializeOpenedPopups() {
			var stack = new Stack<BarPopupBase>();
			stack.Push(null);
			return stack;
		}		
		public static BarPopupBase TopPopup { get { return OpenedPopups.Peek(); } }
		public static bool IsAnyPopupOpened { get { return TopPopup != null; } }
		#region timer
		enum PopupTimerDataAction { Show, Close, CloseChildren }
		class PopupTimerData {
			public bool Ignore { get; set; }
			public BarPopupBase Popup { get; set; }
			public Action Action { get; set; }
			public PopupTimerDataAction Show { get; set; }
		}
		static void StartPopupTimer(BarPopupBase popup, Action popupAction, PopupTimerDataAction show) {
			var pData = PopupTimer.Tag as PopupTimerData;
			if (pData == null || pData.Show != show || pData.Popup != popup) {
				StopShowPopupTimer();
			}
			PopupTimer.Tag = new PopupTimerData() { Popup = popup, Action = popupAction, Show = show };
			if (!PopupTimer.IsEnabled) {
				PopupTimer.Start();				
			}				
		}
		static void OnShowPopupTimerTick(object sender, EventArgs e) {
			var data = PopupTimer.Tag as PopupTimerData;
			StopShowPopupTimer();
			if(data == null || data.Ignore) return;			
			switch(data.Show) {
				case PopupTimerDataAction.Show:
					ShowPopup(data.Popup, data.Action);
					break;
				case PopupTimerDataAction.Close:
					ClosePopup(data.Popup, data.Action);
					break;
				case PopupTimerDataAction.CloseChildren:
					CloseChildren(data.Popup);
					break;
			}			
		}
		static void StopShowPopupTimer(bool continueTimer = false) {
			if (continueTimer) {
				(PopupTimer.Tag as PopupTimerData).Do(x => x.Ignore = true);
				return;
			}
			PopupTimer.Tag = null;
			if (PopupTimer.IsEnabled)
				PopupTimer.Stop();
		}
		#endregion
		#region PUBLIC
		public static void ShowPopup(BarPopupBase popup, bool showImmediately = true, Action showPopupAction = null) {
			if(showImmediately)
				ShowPopup(popup, showPopupAction);
			else
				StartPopupTimer(popup, showPopupAction, PopupTimerDataAction.Show);
		}
		public static bool CloseAllPopups() {
			return CloseAllPopups(null, null);
		}
		static DependencyObject GetClosestPopupChildByMousePosition(MouseEventArgs args) {
			if (args == null) return null;			
			return openedPopups.FirstOrDefault(x => ContainsPoint(x, args)).With(x => x.PopupContent as DependencyObject);
		}
		static bool ContainsPoint(BarPopupBase popup, MouseEventArgs args) {
			var visualContent = popup.With(x => x.Child);
			if (visualContent == null)
				return false;
			var position = args.GetPosition(visualContent);
			var renderRect = new Rect(new Point(), visualContent.RenderSize);
			return renderRect.Contains(position);
		}
		public static bool CloseAllPopups(object sender, MouseEventArgs args) {
			LogBase.Add(null, sender, "-popupmenumanager.closeallpopups_obj_rargs");
			var top = TopPopup;
			var source = GetClosestPopupChildByMousePosition(args) ?? (sender as DependencyObject);
			LogBase.Add(null, source, "source");
			CloseAllPopups(source, IsPreviewEventArgs(args));
			if (top!=null && TopPopup == null)
				NavigationTree.ExitMenuMode();
			LogBase.Add(null, sender, "popupmenumanager.closeallpopups_obj_rargs-");
			return top != TopPopup;
		}
		static bool IsPreviewEventArgs(RoutedEventArgs args) {
			return args.If(x => x.RoutedEvent.RoutingStrategy == RoutingStrategy.Tunnel).ReturnSuccess();
		}
		public static void ClosePopupBranch(BarPopupBase popup) {
			ClosePopupBranch(popup, null, false);
		}		
		public static void ClosePopup(BarPopupBase popup, bool closeImmediately = true, Action closePopupAction = null) {
			if(closeImmediately)
				ClosePopup(popup, closePopupAction);
			else
				StartPopupTimer(popup, closePopupAction, PopupTimerDataAction.Close);
		}
		public static void CancelPopupOpening(BarPopupBase popup) {
			var data = PopupTimer.Tag as PopupTimerData;
			if(data.If(x => x.Show != PopupTimerDataAction.Close).With(x => x.Popup) != popup)
				return;
			StopShowPopupTimer(true);
		}
		public static void CancelPopupClosing(BarPopupBase popup) {
			var data = PopupTimer.Tag as PopupTimerData;
			var dataPopup = data.If(x => x.Show != PopupTimerDataAction.Show).With(x => x.Popup);
			if(dataPopup==null || !(PopupAncestors(popup, false).Contains(dataPopup) && popup.IsOpen)) 
				return;
			StopShowPopupTimer(true);
		}
		public static void CloseChildren(BarPopupBase popup, bool closeImmediately = true) {
			if(!closeImmediately) {
				StartPopupTimer(popup, null, PopupTimerDataAction.CloseChildren);
				return;
			}
			CloseChildren(popup);
		}
		static bool CloseChildren(BarPopupBase popup) {
			if (!OpenedPopups.Any(x => x != null && GetParentPopup(x) == popup))
				return true;
			bool closeResult = true;
			while (TopPopup != popup && TopPopup != null && closeResult) {
				closeResult = ClosePopup(TopPopup, null);
			}
			return closeResult;
		}
		public static bool IsInPopup(object obj, BarPopupBase popup) {
			return IsInPopup(obj, x => x == popup);
		}
		public static bool IsInPopup(object obj, Func<BarPopupBase, bool> predicate) {
			var dObj = obj as DependencyObject;
			if (dObj == null)
				return false;
			var popup = BarManagerHelper.GetPopup(dObj);
			if (popup == null)
				return false;
			return PopupAncestors(popup, true).FirstOrDefault(predicate).ReturnSuccess();
		}		
		static bool IsInMenuBarControl(BarPopupBase popup) {
			return popup != null && popup.Parent is MenuBarControl;
		}
		public static BarPopupBase GetParentPopup(BarPopupBase popup) {
			var result = GetParentPopupImpl(popup);
			if (IsInMenuBarControl(result))
				result = GetParentPopup(result);
			return result;
		}
		static BarPopupBase GetParentPopupImpl(BarPopupBase popup) {
			Guard.ArgumentNotNull(popup, "popup");
			var usePt = popup.If(x => x.Placement == PlacementMode.MousePoint).ReturnSuccess();
			return BarManagerHelper.GetPopup(popup)
				?? popup.Owner.With(x => BarManagerHelper.GetPopup(x))
				?? popup.OwnerLinkControl.With(x => BarManagerHelper.GetPopup(x))
				?? (popup.Placement != PlacementMode.MousePoint ? null : GetPopupUnderMouse().If(x => ZIndex(x) < ZIndex(popup) || ZIndex(popup) == -1));
		}
		static int ZIndex(BarPopupBase popup) {
			if (popup == null || !OpenedPopups.Contains(popup)) return -1;
			return OpenedPopups.ToList().Do(x=>x.Reverse()).IndexOf(popup);
		}
		static BarPopupBase GetPopupUnderMouse() {			
			return OpenedPopups.Where(x => x != null)
				.LastOrDefault(x => {
					var pc = x.PopupContent as UIElement;
					if (pc == null) return false;
					bool hasHit = false;
					HitTestHelper.HitTest(pc, pT => { hasHit = true; return HitTestFilterBehavior.Stop; }, hr => HitTestResultBehavior.Stop, new PointHitTestParameters(Mouse.GetPosition(pc)));
					return hasHit;
				});
		}
		public static IEnumerable<BarPopupBase> PopupAncestors(BarPopupBase popup, bool includeSelf) {
			if(!includeSelf)
				popup = GetParentPopup(popup);
			while(popup != null) {
				yield return popup;
				popup = GetParentPopup(popup);
			}
		}
		public static BarPopupBase GetAncestor(BarPopupBase popup, bool includeSelf, Func<BarPopupBase, bool> predicate) {
			return PopupAncestors(popup, includeSelf).FirstOrDefault(predicate);
		}
		#endregion
		static bool CheckShouldCloseIfItemClicked(DependencyObject originalSource) {
			if (originalSource is BarPopupBase)
				return true;
			var gi = LayoutHelper.FindLayoutOrVisualParentObject<GalleryItemControl>(originalSource).If(x=>IsInSamePopup(x, originalSource));
			if (gi != null)
				return true;
			var lc = LayoutHelper.FindLayoutOrVisualParentObject<BarItemLinkControlBase>(originalSource);
			lc = lc.If(x => IsInSamePopup(x, originalSource));
			var blc = lc as BarButtonItemLinkControl;
			if (blc != null ? !blc.If(x => x.CloseSubMenuOnClick).ReturnSuccess() : lc != null)
				return false;
			return (lc as IPopupOwner).If(x => !x.IsOnBar && x.ActAsDropdown).Return(x => x == null, () => true);
		}
		static bool IsInSamePopup(DependencyObject first, DependencyObject second) {
			if (first == null || second == null)
				return false;
			return BarManagerHelper.GetPopup(first) == BarManagerHelper.GetPopup(second);
		}
		static void ClosePopupBranch(BarPopupBase popup, DependencyObject originalSource, bool isPreview) {
			try {
				LogBase.Add(null, null, "-popupmenumanager.closepopupbranch");
			if (popup == null) {
					BarPopupBase staysOpenPopup = OpenedPopups.FirstOrDefault((x) => x == null ? false : x.StaysOpenOnOuterClick);
					if (staysOpenPopup != null && !(popup == null && originalSource == null))
					CloseChildren(staysOpenPopup);
				else
					ClosePopup(null, true);
				return;
			}
				if (!CheckShouldCloseIfItemClicked(originalSource)) return;
			BarPopupBase topMenu = GetAncestor(popup, false, menu => menu.ActualItemClickBehaviour != PopupItemClickBehaviour.Undefined);
				var behavior = popup.ActualItemClickBehaviour;
				if (isPreview)
					behavior &= PopupItemClickBehaviour.CloseChildren;
			if (originalSource != null && GetIgnorePopupItemClickBehaviour(originalSource))
				behavior = PopupItemClickBehaviour.None;
			switch (behavior) {
				case PopupItemClickBehaviour.None:
					return;
				case PopupItemClickBehaviour.CloseAllPopups:
					ClosePopup(null, true);
					break;
				case PopupItemClickBehaviour.CloseCurrentBranch:
					ClosePopup(GetAncestor(popup, true, x=>x.IsBranchHeader), true);
					break;
				case PopupItemClickBehaviour.ClosePopup:
					ClosePopup(popup, true);
					break;
				case PopupItemClickBehaviour.CloseChildren:
						LogBase.Add(null, null, "-switch.CloseChildren");
						LogBase.Add(null, popup, "popup");
					CloseChildren(popup, true);
						LogBase.Add(null, null, "switch.CloseChildren-");
					break;
				case PopupItemClickBehaviour.Undefined:
					if (topMenu != null) {
						switch (topMenu.ActualItemClickBehaviour) {
							case PopupItemClickBehaviour.None:
								return;
							case PopupItemClickBehaviour.CloseAllPopups:
								ClosePopup(null, true);
								return;
							case PopupItemClickBehaviour.CloseCurrentBranch:
							case PopupItemClickBehaviour.ClosePopup:
								ClosePopup(topMenu);
								return;
							case PopupItemClickBehaviour.CloseChildren:
								CloseChildren(popup, true);
								return;
						}
					}				
					break;
			}			
			} finally {
				LogBase.Add(null, null, "popupmenumanager.closepopupbranch-");
		}
		}
		static void CloseAllPopups(DependencyObject originalSource, bool isPreview) {
			LogBase.Add(null, null, "-popupmenumanager.closeallpopups_do_bool_bool");
			LogBase.Add(null, originalSource, "originalSource");
			LogBase.Add(null, isPreview, "originalSource");
			ClosePopupBranch(originalSource.With(x => BarManagerHelper.GetPopup(x)), originalSource, isPreview);
			LogBase.Add(null, null, "popupmenumanager.closeallpopups_do_bool_bool-");
		}
		static void ShowPopup(BarPopupBase popup, Action showPopupAction) {
			StopShowPopupTimer();
			if(showPopupAction != null) {
				showPopupAction();
				return;
			}
			if(TopPopup == popup)
				return;
			var parentPopup = popup.With(x => GetParentPopup(x));
			if(parentPopup.If(x => !x.IsOpen).ReturnSuccess())
				return;
			while(popup != TopPopup && parentPopup != TopPopup && TopPopup!=null) {
				ClosePopup(TopPopup, true);
			}
			if(popup == null)
				return;
			if(!popup.IsOpen) {
				popup.SetCurrentValue(BarPopupBase.IsOpenProperty, true);
			}
			if (!OpenedPopups.Contains(popup)) {
				OpenedPopups.Push(popup);
				IDisposable dmep = NavigationTree.DisableMouseEventsProcessing();
				popup.Dispatcher.BeginInvoke(new Action(() => { dmep.Do(x => x.Dispose()); }));
			}				
		}
		static bool ClosePopup(BarPopupBase popup, Action closePopupAction) {
			if(closePopupAction != null) {
				closePopupAction();
				return true;
			}
			if (CloseChildren(popup)) {
				return ClosePopupCore(popup);
			}
			return false;
		}
		static bool ClosePopupCore(BarPopupBase popup) {
			if (popup == null || TopPopup != popup || popup.PopupLocker.IsLocked)
				return false;
			using (NavigationTree.Lock()) {
				using (popup.PopupLocker.Lock()) {
					CancelPopupOpening(popup);
					popup.ClosePopup();
					if (!popup.IsOpen)
						OpenedPopups.Pop();
					return !popup.IsOpen;
				}
			}
		}
	}
}
