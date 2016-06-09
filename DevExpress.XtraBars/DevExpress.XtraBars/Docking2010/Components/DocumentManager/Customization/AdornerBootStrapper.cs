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
namespace DevExpress.XtraBars.Docking2010.Customization {
	public interface IBaseAdorner {
		bool Show();
		void Hide();
		void Cancel();
		void RaiseShown();
		void RaiseHidden();
		AdornerHitTest HitTest(Point point);
	}
	internal abstract class BaseAdornerBootStrapper : IDisposable, IMessageFilter {
		IBaseAdorner adornerCore;
		bool isDisposing;
		internal bool isShown;
		int initializationThreadId;
		public BaseAdornerBootStrapper(IBaseAdorner baseAdorner) {
			this.adornerCore = baseAdorner;
			this.initializationThreadId = GetInitializationThreadId();
			Application.AddMessageFilter(this);
			Application.ApplicationExit += Application_ApplicationExit;
		}
		protected virtual int GetInitializationThreadId() {
			return GetCurrentThreadId();
		}
		static int GetCurrentThreadId() {
			return DevExpress.Utils.Win.Hook.HookManager.GetCurrentThreadId();
		}
		void Application_ApplicationExit(object sender, EventArgs e) {
			if(0 == AsyncAdornerWindow.lockApplicationExit) {
				if(GetCurrentThreadId() == initializationThreadId)
					((IDisposable)this).Dispose();
			}
		}
		public virtual void Cancel() {
			if(adornerCore != null) {
				Adorner.Cancel();
				Adorner.RaiseHidden();
				CancelCore();
			}
			isShown = false;
		}
		protected virtual void CancelCore() { }
		void IDisposable.Dispose() {
			if(!isDisposing) {
				isDisposing = true;
				Application.ApplicationExit -= Application_ApplicationExit;
				Application.RemoveMessageFilter(this);
				if(IsShown)
					Cancel();
				DisposeCore(IsDisposing);
				this.adornerCore = null;
			}
			GC.SuppressFinalize(this);
		}
		protected virtual void DisposeCore(bool isDispose) { }
		public virtual void Hide() {
			if(adornerCore != null) {
				Adorner.Hide();
				Adorner.RaiseHidden();
				HideCore();
			}
			isShown = false;
		}
		protected virtual void HideCore() { }
		public virtual void Show() {
			if(adornerCore != null) {
				bool shown = ShowCore();
				if(IsShown != shown) {
					isShown = shown;
					if(shown)
						Adorner.RaiseShown();
				}
			}
		}
		protected abstract bool ShowCore();
		protected virtual bool PreFilterMessage(ref Message m) { 
			return false; 
		}
		protected virtual bool CanFilterMessage { 
			get { return !IsDisposing; } 
		}
		bool IMessageFilter.PreFilterMessage(ref Message m) {
			if(!CanFilterMessage) return false;
			return PreFilterMessage(ref m);
		}
		public bool IsDisposing { 
			get { return isDisposing; } 
		}
		public bool IsShown { 
			get { return isShown; } 
		}
		protected IBaseAdorner Adorner { 
			get { return adornerCore; } 
		}
	}
}
