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
using System.Threading;
using System.Drawing;
using DevExpress.Utils.Drawing.Helpers;
using System.Windows.Forms;
namespace DevExpress.Utils.Animation {
	public class BaseAsyncAdornerBootStrapper : IAsyncAdorner {
		IntPtr ownerWindow;
		Thread thread;
		protected SynchronizationContext raiseDisposedContext;
		protected AutoResetEvent syncObj;
		BaseAsyncAdornerWindow adornerWindow;
		bool isDisposing;
		System.Collections.Generic.List<SubscriptionTarget> subscriptionTargetCollection;
		public BaseAsyncAdornerBootStrapper(IntPtr hWnd, BaseAsyncAdornerElementInfo info) {
			ownerWindow = hWnd;
			Initialize(info);
			Start(info);
		}
		void SubscribeCore(Control control) {
			if(control == null || subscriptionTargetCollection == null) return;
			SubscriptionTarget target = new SubscriptionTarget(control);
			target.LocationChanged += AdornerWindowLocationChanged;
			target.SizeChanged += AdornerWindowSizeChanged;
			target.Move += AdornerWindowLocationChanged;
			subscriptionTargetCollection.Add(target);
		}
		protected void Subscribe() {
			subscriptionTargetCollection = new System.Collections.Generic.List<SubscriptionTarget>();
			SubscribeCore(Owner);
			Form ownerForm = Owner as Form;
			if(ownerForm != null && ownerForm.IsMdiChild)
				SubscribeCore(ownerForm.MdiParent);
		}
		protected void Unsubscribe() {
			if(subscriptionTargetCollection == null) return;
			foreach(SubscriptionTarget target in subscriptionTargetCollection) {
				target.LocationChanged -= AdornerWindowLocationChanged;
				target.SizeChanged -= AdornerWindowSizeChanged;
				target.Move -= AdornerWindowLocationChanged;
				target.Dispose();
			}
			subscriptionTargetCollection.Clear();
			subscriptionTargetCollection = null;
		}
		void AdornerWindowSizeChanged(object sender, EventArgs e) { Dispose(); }
		internal void AdornerWindowLocationChanged(object sender, EventArgs e) {
			adornerWindow.Show(Translate(ownerWindow, IntPtr.Zero, System.Drawing.Point.Empty));
		}
		protected virtual void Start(BaseAsyncAdornerElementInfo info) {
			StartCore(info);
			syncObj.WaitOne();
		}
		protected void StartCore(BaseAsyncAdornerElementInfo info) {
			OnBoot();
			syncObj = new AutoResetEvent(false);
			raiseDisposedContext = System.Threading.SynchronizationContext.Current;
			Thread currentThread = Thread.CurrentThread;
			thread = new Thread(AsyncOperationThreadEntryPoint);
			thread.CurrentCulture = currentThread.CurrentCulture;
			thread.CurrentUICulture = currentThread.CurrentUICulture;
			thread.SetApartmentState(ApartmentState.STA);
			thread.IsBackground = true;
			thread.Start(info);
		}
		protected virtual Rectangle GetBounds(IntPtr hWnd) {
			NativeMethods.RECT r = new NativeMethods.RECT();
			NativeMethods.GetWindowRect(hWnd, ref r);
			return r.ToRectangle();
		}
		Point Translate(IntPtr source, IntPtr target, Point p) {
			NativeMethods.POINT pt = new NativeMethods.POINT(p.X, p.Y);
			NativeMethods.MapWindowPoints(source, target, ref pt, 1);
			return new Point(pt.X, pt.Y);
		}
		static object createWindowSyncObject;
		protected static object CreateWindowSyncObject {
			get {
				if(createWindowSyncObject == null) {
					Type t = typeof(System.Windows.Forms.NativeWindow);
					System.Reflection.FieldInfo fi = t.GetField("createWindowSyncObject", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
					if(fi != null)
						createWindowSyncObject = fi.GetValue(null);
				}
				return createWindowSyncObject;
			}
		}
		[STAThread]
		void AsyncOperationThreadEntryPoint(object info) {
			bool isLock = false;
			BaseAsyncAdornerElementInfo elementInfo = info as BaseAsyncAdornerElementInfo;
			try {
				Monitor.TryEnter(CreateWindowSyncObject, ref isLock);
				if(elementInfo == null)
					isLock = false;
			}
			finally {
				if(isLock) {
					Monitor.Exit(CreateWindowSyncObject);
					AsyncOperationThreadEntryPointCore(elementInfo);
				}
				else
					OnImpound();
			}
		}
		protected virtual void AsyncOperationThreadEntryPointCore(BaseAsyncAdornerElementInfo elementInfo) {
			InitLookAndFeel(elementInfo.InfoArgs);
			InitAdornerWindow(elementInfo);
			syncObj.Set();
			Application.Run();
			OnImpound();
		}
		protected void InitAdornerWindow(BaseAsyncAdornerElementInfo elementInfo) {
			adornerWindow = CreateAsyncAdornerWindow(elementInfo);
			adornerWindow.Create(ownerWindow);
			adornerWindow.Size = GetBounds(ownerWindow).Size;
			adornerWindow.Show(Translate(ownerWindow, IntPtr.Zero, System.Drawing.Point.Empty));
			adornerWindow.Update();
			Subscribe();
		}
		void IAsyncAdorner.Cancel() { Dispose(); }
		void IDisposable.Dispose() { Dispose(); }
		void Dispose() {
			if(!isDisposing) {
				isDisposing = true;
				OnDispose();
			}
			GC.SuppressFinalize(this);
		}
		protected virtual BaseAsyncAdornerWindow CreateAsyncAdornerWindow(BaseAsyncAdornerElementInfo info) {
			return new BaseAsyncAdornerWindow(info);
		}
		protected virtual void InitLookAndFeel(IAsyncAdornerElementInfoArgs infoArgs) {
			if(infoArgs == null || infoArgs.SkinInfo == null) return;
			DevExpress.Skins.SkinManager.SetDefaultSkinManager(infoArgs.SkinInfo.SkinManager);
			LookAndFeel.UserLookAndFeel.SetDefaultLookAndFeel(infoArgs.SkinInfo.LookAndFeel);
		}
		protected virtual void Initialize(BaseAsyncAdornerElementInfo info) { }
		protected BaseAsyncAdornerWindow AdornerWindow { get { return adornerWindow; } }
		protected bool IsDisposing { get { return isDisposing; } }
		protected virtual void OnBoot() { }
		protected virtual void OnImpound() {
			Unsubscribe();
			Ref.Dispose(ref adornerWindow);
			if(syncObj != null)
				syncObj.Set();
			DevExpress.Skins.SkinManager.SetDefaultSkinManager(null);
			LookAndFeel.UserLookAndFeel.SetDefaultLookAndFeel(null);
		}
		protected virtual void OnDispose() {
			Unsubscribe();
			Ref.Dispose(ref syncObj);
			Ref.Dispose(ref adornerWindow);
			thread = null;
		}
		protected BaseAsyncAdornerElementInfo Element { get { return AdornerWindow.Element; } }
		protected Control Owner { get { return Control.FromHandle(ownerWindow); } }
		class SubscriptionTarget : IDisposable {
			Control boundsChangedSubscriptionTarget;
			Form moveSubscriptionTarget;
			public SubscriptionTarget(Control control) {
				boundsChangedSubscriptionTarget = control;
				moveSubscriptionTarget = (control != null) ? (control as Form ?? control.FindForm()) : null;
				if(moveSubscriptionTarget != null && moveSubscriptionTarget.IsMdiChild)
					moveSubscriptionTarget = moveSubscriptionTarget.MdiParent;
			}
			public event EventHandler LocationChanged {
				add {
					if(boundsChangedSubscriptionTarget == null) return;
					boundsChangedSubscriptionTarget.LocationChanged += value;
				}
				remove {
					if(boundsChangedSubscriptionTarget == null) return;
					boundsChangedSubscriptionTarget.LocationChanged -= value;
				}
			}
			public event EventHandler SizeChanged {
				add {
					if(boundsChangedSubscriptionTarget == null) return;
					boundsChangedSubscriptionTarget.SizeChanged += value;
				}
				remove {
					if(boundsChangedSubscriptionTarget == null) return;
					boundsChangedSubscriptionTarget.SizeChanged -= value;
				}
			}
			public event EventHandler Move {
				add {
					if(moveSubscriptionTarget == null) return;
					moveSubscriptionTarget.Move += value;
				}
				remove {
					if(moveSubscriptionTarget == null) return;
					moveSubscriptionTarget.Move -= value;
				}
			}
			public void Dispose() {
				boundsChangedSubscriptionTarget = null;
				moveSubscriptionTarget = null;
			}
		}
	}
}
