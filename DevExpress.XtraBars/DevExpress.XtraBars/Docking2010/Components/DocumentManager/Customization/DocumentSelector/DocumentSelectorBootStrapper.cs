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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Drawing.Helpers;
namespace DevExpress.XtraBars.Docking2010.Customization {
	public interface IDocumentSelector : IBaseAdorner {
		bool Show(bool showLast);		
		void Next();
		void Prev();
		void MoveLeft();
		void MoveRight();
		void MoveUp();
		void MoveDown();
		bool? Click(Point point);
		void Hover(Point screenPoint);
		bool IsOwnControl(Control control);
		bool ShowMenu();
		bool SetNextDocument(bool forward);
	}
	class DocumentSelectorBootStrapper : BaseAdornerBootStrapper, DevExpress.Utils.Win.Hook.IHookController {
		delegate bool KeyDownAction();
		IDictionary<Keys, KeyDownAction> keyDownActions;
		IDocumentSelector Selector { get { return Adorner as IDocumentSelector; } }
		public DocumentSelectorBootStrapper(IDocumentSelector selector) : base(selector) {
			keyDownActions = new Dictionary<Keys, KeyDownAction>();
			keyDownActions.Add(Keys.Control | Keys.Tab, OnCtrlTab);
			keyDownActions.Add(Keys.Control | Keys.Shift | Keys.Tab, OnCtrlShiftTab);
			keyDownActions.Add(Keys.Control | Keys.Left, OnCtrlLeft);
			keyDownActions.Add(Keys.Control | Keys.Right, OnCtrlRight);
			keyDownActions.Add(Keys.Control | Keys.Up, OnCtrlUp);
			keyDownActions.Add(Keys.Control | Keys.Down, OnCtrlDown);
			keyDownActions.Add(Keys.Control | Keys.Shift | Keys.Left, OnCtrlLeft);
			keyDownActions.Add(Keys.Control | Keys.Shift | Keys.Right, OnCtrlRight);
			keyDownActions.Add(Keys.Control | Keys.Shift | Keys.Up, OnCtrlUp);
			keyDownActions.Add(Keys.Control | Keys.Shift | Keys.Down, OnCtrlDown);
			keyDownActions.Add(Keys.Control | Keys.Alt | Keys.Down, OnCtrlAltDown);
			keyDownActions.Add(Keys.Control | Keys.PageDown, OnCtrlTab);
			keyDownActions.Add(Keys.Control | Keys.PageUp, OnCtrlShiftTab);
			Hook();
		}
		protected override void DisposeCore(bool isDispose) {
			if(isDispose)
				UnHook();
			base.DisposeCore(isDispose);
		}				
		Keys[] keyDownList = new Keys[] { 
			Keys.Control| Keys.Tab,
			Keys.Control| Keys.Shift | Keys.Tab,
			Keys.Control| Keys.Left,
			Keys.Control| Keys.Right,
			Keys.Control| Keys.Up,
			Keys.Control| Keys.Down,
		};
		int[] mouseCancel = new int[] { 0x0207, 0x020B, 0x00A4, 0x00A7, 0x00AB };
		int[] mouseIgnore = new int[] { 0x0202, 0x0204, 0x0205, 0x020A };
		protected override bool PreFilterMessage(ref Message m) {			
			if(Array.IndexOf(mouseCancel, m.Msg) != -1) {
				if(IsShown) Cancel();
			}
			if(Array.IndexOf(mouseIgnore, m.Msg) != -1) {
				if(IsShown) return true;
			}
			if(m.Msg == MSG.WM_MOUSEMOVE) {
				if(IsShown) {
					Control ctrl = WinAPIHelper.FindControl(m.HWnd);
					Point screenPoint = ctrl.PointToScreen(WinAPIHelper.GetPoint(m.LParam));
					Hover(screenPoint);
					return true;
				}
			}
			if(m.Msg == MSG.WM_LBUTTONDOWN) {
				if(IsShown) {
					Control ctrl = WinAPIHelper.FindControl(m.HWnd);
					Point screenPoint = ctrl.PointToScreen(WinAPIHelper.GetPoint(m.LParam));
					return OnClick(ref m, screenPoint);
				}
			}
			if(m.Msg == MSG.WM_ACTIVATEAPP) {
				if(IsShown) Cancel();
			}
			if(m.Msg == MSG.WM_KEYDOWN) {
				Keys key = (Keys)WinAPIHelper.GetInt(m.WParam);
				if(key == Keys.Escape)
					if(IsShown) Cancel();
				KeyDownAction action;
				if(keyDownActions.TryGetValue(key | Control.ModifierKeys, out action)) {
					Control ctrl = WinAPIHelper.FindControl(m.HWnd);
					if(Selector != null && Selector.IsOwnControl(ctrl))
						return action();
				}
				if(key == Keys.ShiftKey && isShown)
					return true;
				else {
					if(IsShown)
						Cancel();
				}
			}
			if(m.Msg == MSG.WM_KEYUP) {
				Keys key = (Keys)WinAPIHelper.GetInt(m.WParam);
				if(key == Keys.ControlKey || key == Keys.Menu) 
					return OnCancel();
			}
			return base.PreFilterMessage(ref m);
		}
		bool OnClick(ref Message m, Point screenPoint) {
			bool? clickResult = Click(screenPoint);
			if(clickResult.HasValue) {
				if(clickResult.Value)
					Hide();
				m.Result = IntPtr.Zero;
				return true;
			}
			else Cancel();
			return false;
		}
		void Hover(Point screenPoint) {
			if(Selector != null)
				Selector.Hover(screenPoint);
		}
		bool OnCancel() {
			if(IsShown) Hide();
			return IsShown;
		}		
		bool OnCtrlTab() {
			if(Selector.SetNextDocument(true)) {
				return true;
			}
			if(IsShown) Next();
			else Show();
			return IsShown;
		}
		bool OnCtrlShiftTab() {
			if(Selector.SetNextDocument(false)) {
				return true;
			}
			if(IsShown) Prev();
			else ShowLast();
			return IsShown;
		}
		bool OnCtrlLeft() {
			if(IsShown) MoveLeft();
			return IsShown;
		}
		bool OnCtrlRight() {
			if(IsShown) MoveRight();
			return IsShown;
		}
		bool OnCtrlUp() {
			if(IsShown) MoveUp();
			return IsShown;
		}
		bool OnCtrlDown() {
			if(IsShown) MoveDown();
			return IsShown;
		}
		bool OnCtrlAltDown() {
			return Selector.ShowMenu();
		}
		protected override bool ShowCore() {
			return Selector.Show(false);
		}	   
		void ShowLast() {
			if(Selector != null) {
				bool shown = Selector.Show(true);
				if(isShown != shown) {
					isShown = shown;
					if(shown) Selector.RaiseShown();
				}
			}
		}
		bool? Click(Point point) {
			if(Selector != null)
				return Selector.Click(point);
			return null;
		}  
		void Next() {
			if(Selector != null)
				Selector.Next();
		}
		void Prev() {
			if(Selector != null)
				Selector.Prev();
		}
		void MoveLeft() {
			if(Selector != null)
				Selector.MoveLeft();
		}
		void MoveRight() {
			if(Selector != null)
				Selector.MoveRight();
		}
		void MoveUp() {
			if(Selector != null)
				Selector.MoveUp();
		}
		void MoveDown() {
			if(Selector != null)
				Selector.MoveDown();
		}
		#region HOOKS
		bool isAttachedToHookManagerCore;
		protected bool IsAttachedToHookManager {
			get { return isAttachedToHookManagerCore; }
		}
		protected void Hook() {
			if(!IsAttachedToHookManager) {
				DevExpress.Utils.Win.Hook.HookManager.DefaultManager.AddController(this);
				isAttachedToHookManagerCore = true;
			}
		}
		protected void UnHook() {
			DevExpress.Utils.Win.Hook.HookManager.DefaultManager.RemoveController(this);
			isAttachedToHookManagerCore = false;
		}
		bool DevExpress.Utils.Win.Hook.IHookController.InternalPostFilterMessage(int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam) {
			return false;
		}
		bool DevExpress.Utils.Win.Hook.IHookController.InternalPreFilterMessage(int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam) {
			if(Msg == DevExpress.Utils.Drawing.Helpers.MSG.WM_ACTIVATEAPP && WParam == IntPtr.Zero && isShown)
				Cancel();
			return false;
		}
		IntPtr DevExpress.Utils.Win.Hook.IHookController.OwnerHandle {
			get { return OwnerHandleCore; }
		}
		protected virtual IntPtr OwnerHandleCore { get { return IntPtr.Zero; } }
		#endregion HOOKS
	}
}
