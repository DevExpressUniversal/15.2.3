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
using DevExpress.Utils.Drawing.Helpers;
namespace DevExpress.Utils.Animation {
	interface ITransitionAdornerBootStraper : IAsyncAdorner {
		event EventHandler Disposed;
		void Invalidate();
	}
	class TransitionAsyncAdornerWindow : BaseAsyncAdornerWindow {
		public TransitionAsyncAdornerWindow(BaseAsyncAdornerElementInfo elementCore)
			: base(elementCore) { }
		public void InvalidateAsync() {
			lock(syncLock) {
				TransitionInfoArgs infoArgs = (Element.InfoArgs as TransitionInfoArgs);
				infoArgs.UpdateAsync();
				Invalidate();
			}
		}
		protected override void NCHitTest(ref System.Windows.Forms.Message m) {
			m.Result = new IntPtr(DevExpress.Utils.Drawing.Helpers.NativeMethods.HT.HTNOWHERE);
		}
	}
	class TransitionAdornerBootStrapper : BaseAsyncAdornerBootStrapper, ITransitionAdornerBootStraper {
		event EventHandler disposedCore;
		TransitionAdornerBootStrapper(IntPtr hWnd, BaseAsyncAdornerElementInfo info) : base(hWnd, info) { }
		public static ITransitionAdornerBootStraper Show(IntPtr hWnd, BaseAsyncAdornerElementInfo info) {
			return new TransitionAdornerBootStrapper(hWnd, info);
		}
		protected override BaseAsyncAdornerWindow CreateAsyncAdornerWindow(BaseAsyncAdornerElementInfo info) {
			return new TransitionAsyncAdornerWindow(info);
		}
		void ITransitionAdornerBootStraper.Invalidate() {
			if(AdornerWindow == null) return;
			AdornerWindow.InvalidateAsync();
		}
		#region fix T286043
		#endregion
		protected override System.Drawing.Rectangle GetBounds(IntPtr hWnd) {
			if(Owner != null) return GetRealBounds(Owner, Owner.Bounds);
			return base.GetBounds(hWnd);
		}
		System.Drawing.Rectangle GetRealBounds(System.Windows.Forms.Control control, System.Drawing.Rectangle bounds) {
			if(control == null) return bounds;
			System.Windows.Forms.Control parent = control.Parent;
			if(parent == null) return bounds;
			System.Drawing.Rectangle newBounds = System.Drawing.Rectangle.Intersect(bounds, parent.ClientRectangle);
			newBounds.Offset(parent.Location);
			return GetRealBounds(parent, newBounds);
		}
		protected new TransitionAsyncAdornerWindow AdornerWindow { get { return base.AdornerWindow as TransitionAsyncAdornerWindow; } }
		protected override void OnImpound() {
			base.OnImpound();
			if(IsDisposing) return;
			((IDisposable)this).Dispose();
		}
		protected override void OnDispose() {
			if(AdornerWindow != null) {
				DevExpress.Utils.Internal.LayeredWindowMessanger.PostClose(AdornerWindow.Handle);
				syncObj.WaitOne();
			}
			base.OnDispose();
			if(raiseDisposedContext != null) {
				raiseDisposedContext.Post(RaiseDisposed, this);
				raiseDisposedContext = null;
			}
		}
		void RaiseDisposed(object owner) {
			if(disposedCore != null)
				disposedCore(owner, EventArgs.Empty);
		}
		event EventHandler ITransitionAdornerBootStraper.Disposed {
			add { disposedCore += value; }
			remove { disposedCore -= value; }
		}
	}
}
