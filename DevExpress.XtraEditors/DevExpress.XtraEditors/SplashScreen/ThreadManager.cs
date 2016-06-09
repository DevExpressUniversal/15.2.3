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
using System.Threading;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Helpers;
using DevExpress.XtraSplashForm;
using DevExpress.XtraSplashScreen.Utils;
using System.Collections.Generic;
using DevExpress.XtraWaitForm;
using System.Reflection;
namespace DevExpress.XtraSplashScreen {
	#region Delegates
	delegate void ScreenAction();
	delegate void ScreenAction<T>(T obj);
	delegate void ScreenAction<T1, T2>(T1 arg1, T2 arg2);
	#endregion
	public abstract class ThreadManagerBase : IDisposable {
		static ThreadManagerBase() {
			AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
		}
		public ThreadManagerBase(SplashScreenManager manager) {
			this.Manager = manager;
			this.InPendingMode = false;
			this.PostponedCommandManager = new PostponedCommandManager();
		}
		~ThreadManagerBase() { Dispose(false); }
		public static ThreadManagerBase Create(SplashScreenManager manager) {
			if(manager.Mode == Mode.SplashScreen)
				return new SplashScreenThreadManager(manager);
			if(manager.Mode == Mode.Layer)
				return new SplashScreenLayerThreadManager(manager);
			return new WaitFormThreadManager(manager);
		}
		SplashScreenManager managerCore = null;
		public SplashScreenManager Manager {
			get { return this.managerCore; }
			set { this.managerCore = value; }
		}
		Form formInstance = null;
		public Form Form {
			get { return this.formInstance; }
			set { this.formInstance = value; }
		}
		Thread threadCore = null;
		public Thread Thread {
			get { return this.threadCore; }
			set { this.threadCore = value; }
		}
		AutoResetEvent syncEventCore = new AutoResetEvent(false);
		public AutoResetEvent SyncEvent {
			get { return this.syncEventCore; }
			set { this.syncEventCore = value; }
		}
		protected PostponedCommandManager PostponedCommandManager { get; private set; }
		public bool InPendingMode {
			get;
			set;
		}
		public SplashFormBase SplashBase { get { return Form as SplashFormBase; } }
		public void Start() {
			Thread = new Thread(ThreadEntryPoint);
			InitThread();
			SetCursor();
			Thread.Start(SkinName);
			SyncEvent.WaitOne();
		}
		protected virtual bool CanSetWaitCursor {
			get { return ShouldUsePendingMode; }
		}
		Cursor previousCursor = null;
		protected virtual void SetCursor() {
			if(!CanSetWaitCursor)
				return;
			previousCursor = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
		}
		protected virtual void RestoreCursor() {
			if(!CanSetWaitCursor)
				return;
			Cursor.Current = previousCursor;
		}
		protected virtual void InitThread() {
			Thread currentThread = System.Threading.Thread.CurrentThread;
			Thread.IsBackground = true;
			Thread.CurrentCulture = currentThread.CurrentCulture;
			Thread.CurrentUICulture = currentThread.CurrentUICulture;
			if(SplashScreenManager.ApartmentState.HasValue) Thread.SetApartmentState(SplashScreenManager.ApartmentState.Value);
		}
		public void Destroy(int closingDelay, Form parent, bool throwExceptionIfAlreadyClosed) {
			DoDestroy(closingDelay, parent, throwExceptionIfAlreadyClosed);
		}
		bool destroyCalled = false;
		protected virtual void DoDestroy(int closingDelay, Form parent, bool throwExceptionIfAlreadyClosed) {
			destroyCalled = true;
			if(InPendingMode) {
				return;
			}
			if(Form == null && ShouldUsePendingMode && !InPendingMode) {
				SyncEvent.WaitOne();
			}
			if(Form == null) {
				if(!throwExceptionIfAlreadyClosed) return;
				throw new InvalidOperationException(string.Format("{0} wasn't created", FormName));
			}
			if(!Form.IsHandleCreated) return;
			Form.BeginInvoke((ScreenAction)delegate {
				if(closingDelay > 0) SplashBase.DelayedClose(closingDelay, parent);
				else Form.Close();
			});
		}
		public void Destroy(int closingDelay, Form parent) {
			Destroy(closingDelay, parent, true);
		}
		void ThreadEntryPoint(object skinName) {
			Thread thread = Thread;
			ThreadRegistry.AddThread(thread);
			InnerThreadContext threadContext = new InnerThreadContext();
			try {
				ThreadEntryPointCore((string)skinName);
			}
			finally {
				ThreadRegistry.RemoveThread(thread);
				threadContext.Dispose();
			}
		}
		protected void ThreadEntryPointCore(string skinName) {
			WindowsFormsSynchronizationContext.AutoInstall = false;
			SubscribeEvents();
			if(ShouldUsePendingMode) {
				EnterToPendingMode();
				if(destroyCalled) {
					Dispose();
					return;
				}
			}
			InitLookAndFeel(skinName);
			Form = CreateForm();
			ForceCreateHandle(Form);
			if(SplashBase != null) {
				SplashBase.Properties = Manager.Properties.Clone();
			}
			Form.Shown += Form_Shown;
			InitForm();
			if(Manager != null) Manager.OnBeforeShow();
			try {
				DoRun();
			}
			catch(ThreadAbortException) {
			}
			finally {
				if(Form != null) {
					Form.Dispose();
				}
				Form = null;
				ReleaseLookAndFeel();
			}
		}
		protected virtual void DoRun() {
			OnFormDisplaying();
			try {
				if(SplashBase != null) SplashBase.ShowDialog();
				else Form.ShowDialog();
			}
			finally {
				OnFormClosed();
			}
		}
		protected virtual void ForceCreateHandle(Form form) {
			IntPtr handle = form.Handle;
		}
		protected bool ShouldUsePendingMode {
			get {
				if(Manager == null || Manager.Properties == null)
					return false;
				return Manager.Properties.PendingTime > 0;
			}
		}
		protected virtual void EnterToPendingMode() {
			InPendingMode = true;
			try {
				SyncEvent.Set();
				Thread.Sleep(Manager.Properties.PendingTime);
			}
			finally {
				InPendingMode = false;
				RestoreCursor();
			}
		}
		protected virtual string SkinName {
			get {
				if(!string.IsNullOrEmpty(SplashScreenManager.SkinName))
					return SplashScreenManager.SkinName;
				Form parentForm = Manager.Properties.ParentForm;
				ISupportLookAndFeel laf = parentForm as ISupportLookAndFeel;
				if(laf != null && parentForm.IsHandleCreated)
					return laf.LookAndFeel.ActiveSkinName;
				return UserLookAndFeel.Default.SkinName;
			}
		}
		protected virtual Form CreateForm() {
			SplashFormBase form = null;
			if(Manager.CreateFormFunction == null)
				form = (SplashFormBase)Activator.CreateInstance(Manager.SplashFormType);
			else 
				form = Manager.CreateFormFunction();
			if(ShouldUsePendingMode)
				form.AssignPostponedManager(PostponedCommandManager);
			return form;
		}
		protected virtual void InitLookAndFeel(string skinName) {
			ReflectionHelper.InvokeStaticMethod(AssemblyInfo.SRAssemblyBonusSkins, "DevExpress.UserSkins.BonusSkins", "Register");
			SkinHelper.RegisterUserSkins(skinName);
			UserLookAndFeel.Default.SetSkinStyle((string)skinName);
		}
		protected virtual void ReleaseLookAndFeel() {
			UserLookAndFeel.Default.Dispose();
		}
		void Form_Shown(object sender, EventArgs e) {
			Form.Shown -= Form_Shown;
			SyncEvent.Set();
		}
		protected internal void Join() {
			Thread thread = Thread;
			if(thread != null) thread.Join();
		}
		protected virtual void SubscribeEvents() {
			AppDomain domain = AppDomain.CurrentDomain;
			domain.DomainUnload += OnDomainUnload;
		}
		protected virtual void UnsubscribeEvents() {
			AppDomain domain = AppDomain.CurrentDomain;
			domain.DomainUnload -= OnDomainUnload;
		}
		void OnDomainUnload(object sender, EventArgs e) {
			DoDisposeForm();
		}
		static void OnProcessExit(object sender, EventArgs e) {
			ThreadRegistry.Join(true);
		}
		#region IDisposable
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		protected internal void DoDisposeForm() {
			if(this.fDisposed) return;
			SplashFormBase form = (SplashFormBase)Form;
			if(form != null && form.IsHandleCreated) {
				try { form.Invoke(new MethodInvoker(form.Dispose)); }
				catch { }
			}
		}
		bool fDisposed = false;
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				UnsubscribeEvents();
				UnsubscribeFormClosed();
				if(Form != null) {
					Form.Shown -= Form_Shown;
				}
				DoDisposeForm();
				if(PostponedCommandManager != null) {
					PostponedCommandManager.Dispose();
				}
				PostponedCommandManager = null;
				if(SyncEvent != null)
					SyncEvent.Close();
				SyncEvent = null;
				Thread = null;
				Manager = null;
			}
			this.fDisposed = true;
		}
		protected virtual void InitForm() {
			SetFormLocation();
			SubscribeFormClosed();
		}
		protected void SubscribeFormClosed() {
			Form form = Form;
			if(form != null) form.FormClosed += OnFormClosed;
		}
		protected void UnsubscribeFormClosed() {
			Form form = Form;
			if(form != null) form.FormClosed -= OnFormClosed;
		}
		protected virtual void OnFormClosed(object sender, FormClosedEventArgs e) {
			if(Manager != null) {
				Manager.OnClosed();
				if(Manager.IsInnerManager) {
					Manager.Dispose();
					Manager = null;
				}
			}
		}
		protected virtual void OnFormDisplaying() {
			if(Manager != null) Manager.IsSplashFormVisible = true;
		}
		protected virtual void OnFormClosed() {
		}
		protected void SetFormLocation() {
			Form.StartPosition = FormStartPosition.Manual;
			Form.Location = ShouldUseUserLocationSettings ? GetFormUserLocation() : GetFormAutoLocation();
		}
		protected virtual Point GetFormUserLocation() {
			return Manager.SplashFormLocation;
		}
		protected virtual Point GetFormAutoLocation() {
			Point res = Point.Empty;
			Form parentForm = Manager.Properties.ParentForm;
			if(parentForm != null) {
				if(parentForm.IsHandleCreated && parentForm.WindowState != FormWindowState.Minimized)
					res = LayoutHelper.GetLocationRelParentCore(Form, parentForm);
				else res = LayoutHelper.GetLocationRelCursorScreen(Form);
			}
			else res = LayoutHelper.GetLocationRelAppWindow(Form);
			return res;
		}
		protected internal bool ShouldUseUserLocationSettings {
			get { return LayoutHelper.ShouldUseUserLocation(Manager); }
		}
		#region postponed commands
		protected internal virtual void SendCommand(Enum cmd, object arg) {
			PostponedCommandManager.AddSendCommand(cmd, arg);
		}
		protected internal virtual void SetWaitFormCaption(string caption) {
			PostponedCommandManager.AddSetWaitFormCaption(caption);
		}
		protected internal virtual void SetWaitFormDescription(string description) {
			PostponedCommandManager.AddWaitFormDescription(description);
		}
		#endregion
		public abstract string FormName { get; }
		public abstract void OnOwnerFormHandleCreated();
	}
	class InnerThreadContext : IDisposable {
		object threadContext;
		public InnerThreadContext() {
			this.threadContext = null;
		}
		public object ThreadContext {
			get {
				if(this.threadContext == null) {
					this.threadContext = LoadThreadContext();
				}
				return this.threadContext;
			}
		}
		static readonly Type ThreadContextType = typeof(Application).GetNestedType("ThreadContext", BindingFlags.NonPublic);
		protected object LoadThreadContext() {
			MethodInfo fromCurrentMethod = ThreadContextType.GetMethod("FromCurrent", BindingFlags.Static | BindingFlags.NonPublic);
			return fromCurrentMethod.Invoke(null, null);
		}
		public void InvokeDispose(bool postQuit) {
			MethodInfo disposeMethod = GetMethod("Dispose", typeof(bool));
			if(disposeMethod != null) disposeMethod.Invoke(ThreadContext, new object[] { postQuit });
		}
		protected MethodInfo GetMethod(string methodName, params Type[] types) {
			return ThreadContext.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic, null, types, null);
		}
		public void Dispose() {
			Dispose(true);
		}
		bool disposed = false;
		protected virtual void Dispose(bool disposing) {
			if(this.disposed) return;
			InvokeDispose(false);
			this.disposed = true;
		}
	}
	class SplashScreenThreadManager : ThreadManagerBase {
		public SplashScreenThreadManager(SplashScreenManager manager) : base(manager) { }
		protected override void InitForm() {
			base.InitForm();
			SplashScreen scn = Form as SplashScreen;
			if(SplashBase != null && SplashBase.Properties.Image != null && scn != null) {
				scn.ShowMode = ShowMode.Image;
				scn.SplashImage = SplashBase.Properties.Image;
			}
		}
		protected override void OnFormClosed(object sender, FormClosedEventArgs e) {
			DoActivateParentForm();
			base.OnFormClosed(sender, e);
		}
		protected virtual void DoActivateParentForm() {
			if(!SplashScreenManager.ActivateParentOnSplashFormClosing)
				return;
			Form parent = Manager.Properties.ParentForm;
			if(parent != null && parent.IsHandleCreated)
				ActivateParentFormCore(parent);
		}
		void ActivateParentFormCore(Form parentForm) {
			try {
				parentForm.Invoke((ScreenAction)parentForm.Activate);
			}
			catch { } 
		}
		public override void OnOwnerFormHandleCreated() {
			Form form = Form;
			if(form == null) return;
			try {
				form.BeginInvoke(new MethodInvoker(form.Activate));
			}
			catch { }
		}
		public override string FormName { get { return "Splash Screen"; } }
	}
	class SplashScreenLayerThreadManager : ThreadManagerBase {
		public SplashScreenLayerThreadManager(SplashScreenManager manager) : base(manager) { }
		protected override Form CreateForm() {
			return (SplashFormBase)Activator.CreateInstance(Manager.SplashFormType, Manager.Properties.Image, ShouldUseUserLocationSettings);
		}
		public override void OnOwnerFormHandleCreated() {
		}
		public override string FormName { get { return "Image"; } }
	}
	class WaitFormThreadManager : ThreadManagerBase {
		public WaitFormThreadManager(SplashScreenManager manager)
			: base(manager) {
		}
		public override void OnOwnerFormHandleCreated() {
		}
		public override string FormName { get { return "Wait Form"; } }
	}
	public class ThreadRegistry {
		static ThreadRegistry instance = new ThreadRegistry();
		List<Thread> list;
		protected ThreadRegistry() {
			this.list = new List<Thread>();
		}
		static object lockObject = new object();
		public static void AddThread(Thread thread) {
			lock(lockObject) {
				instance.list.Add(thread);
			}
		}
		public static void RemoveThread(Thread thread) {
			if(IsWaiting) return;
			lock(lockObject) {
				instance.list.Remove(thread);
			}
		}
		bool isWaiting = false;
		public static void Join(bool reset) {
			instance.isWaiting = true;
			try {
				lock(lockObject) {
					if(instance.list.Count == 0) return;
					foreach(Thread thread in instance.list) {
						thread.Join();
					}
					if(reset) instance.list.Clear();
				}
			}
			finally {
				instance.isWaiting = false;
			}
		}
		public static bool IsWaiting { get { return instance.isWaiting; } }
	}
	public class PostponedCommandManager : IDisposable {
		Queue<PostponedCommandBase> queue;
		public PostponedCommandManager() {
			this.queue = new Queue<PostponedCommandBase>();
		}
		public void AddSendCommand(Enum cmd, object arg) {
			Queue.Enqueue(new CommonPostponedCommand(cmd, arg));
		}
		public void AddSetWaitFormCaption(string caption) {
			Queue.Enqueue(new SetCaptionPostponedCommand(caption));
		}
		public void AddWaitFormDescription(string description) {
			Queue.Enqueue(new SetDescriptionPostponedCommand(description));
		}
		public static void Execute(PostponedCommandManager manager, SplashFormBase form) {
			foreach(PostponedCommandBase command in manager.Queue) {
				IPostponedCommandExecutable runner = command as IPostponedCommandExecutable;
				if(runner != null) runner.Execute(form);
			}
		}
		public void Dispose() {
			Clear();
		}
		public void Clear() {
			Queue.Clear();
		}
		protected Queue<PostponedCommandBase> Queue { get { return queue; } }
	}
	public interface IPostponedCommandExecutable {
		void Execute(SplashFormBase form);
	}
	public abstract class PostponedCommandBase : IPostponedCommandExecutable {
		void IPostponedCommandExecutable.Execute(SplashFormBase form) {
			ExecuteCore(form);
		}
		protected abstract void ExecuteCore(SplashFormBase form);
	}
	public class CommonPostponedCommand : PostponedCommandBase {
		Enum cmd;
		object arg;
		public CommonPostponedCommand(Enum cmd, object arg) {
			this.cmd = cmd;
			this.arg = arg;
		}
		protected override void ExecuteCore(SplashFormBase form) {
			form.ProcessCommand(Cmd, Arg);
		}
		public Enum Cmd { get { return cmd; } }
		public object Arg { get { return arg; } }
	}
	public class SetCaptionPostponedCommand : PostponedCommandBase {
		string caption;
		public SetCaptionPostponedCommand(string caption) {
			this.caption = caption;
		}
		protected override void ExecuteCore(SplashFormBase form) {
			WaitForm wf = form as WaitForm;
			if(wf != null) wf.SetCaption(Caption);
		}
		public string Caption { get { return caption; } }
	}
	public class SetDescriptionPostponedCommand : PostponedCommandBase {
		string description;
		public SetDescriptionPostponedCommand(string description) {
			this.description = description;
		}
		protected override void ExecuteCore(SplashFormBase form) {
			WaitForm wf = form as WaitForm;
			if(wf != null) wf.SetDescription(Description);
		}
		public string Description { get { return description; } }
	}
	class LayoutHelper {
		public static bool ShouldUseUserLocation(SplashScreenManager mgr) {
			return mgr.SplashFormStartPosition == SplashFormStartPosition.Manual;
		}
		public static Point GetLocationRelAppWindow(Form form) {
			IntPtr handle = ProcessHelper.GetMainWindowHandle();
			if(handle == IntPtr.Zero)
				return GetLocationRelCursorScreen(form);
			return GetLocationRelForm(form, handle);
		}
		public static Point GetLocationRelForm(Form form, IntPtr handle) {
			NativeMethods.RECT rect = new NativeMethods.RECT();
			if(NativeMethods.GetWindowRect(handle, ref rect)) {
				Point loc = new Point(rect.Left, rect.Top);
				Size size = new Size(rect.Right - rect.Left, rect.Bottom - rect.Top);
				return GetLocationRelParentCore(form.Size, loc, size);
			}
			return GetLocationRelCursorScreen(form);
		}
		public static Point GetLocationRelCursorScreen(Form form) {
			Screen screen = Screen.FromPoint(Control.MousePosition);
			return LayoutHelper.GetLocationRelParentCore(form.Size, screen.Bounds.Location, screen.Bounds.Size);
		}
		public static Point GetLocationRelParentCore(Form form, Form parentForm) {
			Size parentSize = parentForm.Size;
			Point parentLoc = parentForm.Location;
			if(parentForm.IsMdiChild && parentForm.MdiParent != null) {
				parentSize = parentForm.MdiParent.Size;
				parentLoc = parentForm.MdiParent.Location;
			}
			return GetLocationRelParentCore(form.Size, parentLoc, parentSize);
		}
		public static Point GetLocationRelParentCore(Size size, Point parentLoc, Size parentSize) {
			return new Point(parentLoc.X + parentSize.Width / 2 - size.Width / 2, parentLoc.Y + parentSize.Height / 2 - size.Height / 2);
		}
	}
}
