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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.DemoBase.Helpers;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Helpers;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using DevExpress.XtraTests;
namespace DevExpress.Xpf.DemoBase.DemoTesting {
	public abstract class BaseTestingFixture : IErrorLog {
		Thread uiThread = null;
		public class UIAutomationActions {
			public static void ToggleButton(Func<ToggleButton> getButtonDelegate) {
				ToggleButton(getButtonDelegate());
			}
			public static void ToggleButton(ToggleButton button) {
				ToggleButtonAutomationPeer peer = FrameworkElementAutomationPeer.CreatePeerForElement(button) as ToggleButtonAutomationPeer;
				IToggleProvider invoker = peer.GetPattern(PatternInterface.Toggle) as IToggleProvider;
				invoker.Toggle();
			}
			public static void ClickButton(Func<ButtonBase> getButtonDelegate) {
				ClickButton(getButtonDelegate());
			}
			public static void ClickButton(ButtonBase button) {
				ButtonBaseAutomationPeer peer = FrameworkElementAutomationPeer.CreatePeerForElement(button) as ButtonBaseAutomationPeer;
				IInvokeProvider invoker = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
				invoker.Invoke();
			}
		}
		public class EditorsActions {
			public static void ToggleCheckEdit(Func<CheckEdit> getEditDelegate) {
				ToggleCheckEdit(getEditDelegate());
			}
			public static void ToggleCheckEdit(CheckEdit edit) {
				BaseEditHelper.ToggleCheckEdit(edit);
			}
			public static void PrepareToInput(BaseEdit edit) {
				edit.Focus();
			}
			public static void PrepareToInput(TextEdit edit, int selectionStart, int selectionLength) {
				PrepareToInput(edit);
				edit.SelectionStart = selectionStart;
				edit.SelectionLength = selectionLength;
			}
			public static void SendString(TextEdit edit, string text) {
				PrepareToInput(edit);
				WindowTester.SendString(text);
			}
			public static void SendKey(TextEdit edit, char key) {
				PrepareToInput(edit);
				WindowTester.SendKey(key);
			}
			public static void SetSelectedIndex(ComboBoxEdit edit, int selectedIndex) {
				PrepareToInput(edit);
				edit.SelectedIndex = selectedIndex;
			}
			public static void SetEditValue(BaseEdit edit, object editValue) {
				PrepareToInput(edit);
				edit.EditValue = editValue;
			}
		}
		public class MouseActions {
			public static void ClickElement(FrameworkElement element) {
				WindowTester.SendLMouseClick(element, element.ActualWidth / 2, element.ActualHeight / 2);
			}
			public static void LeftMouseDown(FrameworkElement element, double x, double y) {
				WindowTester.SendLMouseDown(element, x, y);
			}
			public static void LeftMouseUp(FrameworkElement element, double x, double y) {
				WindowTester.SendLMouseUp(element, x, y);
			}
			public static void MouseMove(FrameworkElement element, double x, double y) {
				WindowTester.SendMouseMove(element, x, y);
			}
		}
		public class HelperActions {
			public static T FindElementByType<T>(FrameworkElement parent) where T : FrameworkElement {
				return (T)LayoutHelper.FindElement(parent, (element) => element is T);
			}
			public static T FindElementByName<T>(FrameworkElement parent, string name) where T : FrameworkElement {
				return (T)LayoutHelper.FindElement(parent, (element) => (element is T) && element.Name == name);
			}
		}
		static WindowTester windowTester;
		internal static WindowTester WindowTester {
			get {
				if(BrowserInteropHelper.IsBrowserHosted)
					Assert.Instance.Fail("WindowTester can't be used in XBAP");
				if(windowTester == null)
					windowTester = new WindowTester(DevExpress.Xpf.DemoBase.Helpers.HostSizeBinder.RetrieveRootElement() as Window);
				return windowTester;
			}
		}
		protected Theme defaultTheme;
		StringBuilder errorLog = new StringBuilder();
		protected internal Assert Assert { get { return Assert.Instance; } }
		protected internal AssertLog AssertLog { get; private set; }
		protected internal FrameworkElement RootElementInstance { get { return DemoTestingHelper.RootElementInstance; } }
		protected BaseTestingFixture() {
			AssertLog = new AssertLog(this);
		}
		protected void BusyWait(Func<bool> condition, int msTimeout = 10000) {
			bool isUiThread = this.uiThread == Thread.CurrentThread;
			IAmAlive("BusyWait" + (isUiThread ? " in ui thread" : ""));
			Stopwatch sw = Stopwatch.StartNew();
			while(!condition()) {
				if(sw.Elapsed.Milliseconds > msTimeout) {
					IAmAlive("BusyWait timed out after " + msTimeout + "ms");
					throw new Exception("BusyWait time out");
				}
				Thread.Sleep(10);
				if(isUiThread) {
					UpdateLayoutAndDoEvents();
				}
			}
		}
		protected void IAmAlive(string message) {
			DemoTestingHelper.IAmAlive(string.Format("Module: {0}; Message: {1}\n", DemoBaseTesting.CurrentModule, message));
		}
		protected void DispatchAsync(Action action) {
			RootElementInstance.Dispatcher.BeginInvoke(action);
		}
		void TesterThread() {
			BusyWait(() => DemoBaseTesting.IsReady);
			IAmAlive("Getting default theme");
			defaultTheme = ThemeManager.ApplicationThemeName == null ? null : Theme.FindTheme(ThemeManager.ApplicationThemeName);
			IAmAlive("Waiting for modules");
			BusyWait(() => DemoBaseTesting.Modules != null);
			IAmAlive("Running checks");
			CreateActions();
			IAmAlive("Tester thread has completed");
		}
		public void Start() {
			this.uiThread = Thread.CurrentThread;
			DemoTestingHelper.IsLogging = true;
			DemoTestingHelper.Logs = string.Empty;
			if(DevExpress.DemoData.Helpers.EnvironmentHelper.IsClickOnce)
				Thread.Sleep(15000); 
			Task.Factory
				.StartNew(TesterThread)
				.ContinueWith(t => FinishTesting(t.Exception), TaskScheduler.FromCurrentSynchronizationContext())
				.ContinueWith(t => {
					if(t.Exception != null) {
						MessageBox.Show(t.Exception.ToString());
						throw t.Exception;
					}
				}, TaskScheduler.FromCurrentSynchronizationContext());
		}
		public void AddErrorToLog(string errorMessage) {
			errorLog.AppendLine(errorMessage);
		}
		protected virtual void CreateActions() { }
		void FinishTesting(Exception unhandledException) {
			IAmAlive("Before finishing testing actions");
			BeforeFinishTesting();
			IAmAlive("Finishing testing");
			string error = null;
			if(unhandledException != null) {
				DemoTestingHelper.Service.OnError(string.Format("unhandled exception:\n{0}\n{1}\n{2}", unhandledException.Message, unhandledException.StackTrace, unhandledException.InnerException));
			} else {
				error = errorLog.ToString();
				if(!string.IsNullOrEmpty(error)) {
					DemoTestingHelper.Service.OnError("there are errors collected while testing:\n" + error);
				}
			}
			DemoTestingHelper.TestComplete();
			if(System.Windows.Interop.BrowserInteropHelper.IsBrowserHosted) return;
			bool debugFlag = System.IO.File.Exists(@"c:\temp\debug.flag");
			if(!debugFlag) {
				foreach(Window window in Application.Current.Windows) {
					window.Close();
				}
			}
			if(unhandledException != null)
				Debugger.Break();
			if(!string.IsNullOrEmpty(error)) {
				if(debugFlag) {
					throw new Exception(error.ToString());
				} else {
					Debugger.Break();
				}
			}
		}
		protected internal void UpdateLayoutAndDoEvents() {
			DispatcherHelper.UpdateLayoutAndDoEvents(RootElementInstance);
		}
		protected virtual void BeforeFinishTesting() {
		}
		public IDemoBaseTesting DemoBaseTesting { get; internal set; }
	}
	public abstract class BaseDemoTestingFixture : BaseTestingFixture {
		Dictionary<Type, List<WeakReference>> refTable = new Dictionary<Type, List<WeakReference>>();
		protected void AddSimpleAction(Action action) { 
			Dispatch(action);
		}
		protected void AddConditionAction(Func<bool> condition, Action action) { 
			DispatchBusyWait(condition);
		}
		class RoutedEventHandlerPair {
			public UIElement Owner { get; set; }
			public RoutedEventHandler Handler { get; set; }
		};
		protected void AddEventAction(RoutedEvent re, Func<UIElement> getElementCallback, Action action, Action preeventAction) { 
			Func<RoutedEventHandler, RoutedEventHandlerPair> subscribe = h => {
				RoutedEventHandler handler = (s, e) => h(s, e);
				UIElement owner = getElementCallback();
				owner.AddHandler(re, handler);
				return new RoutedEventHandlerPair() { Owner = owner, Handler = handler };
			};
			Action<RoutedEventHandlerPair> unsubscribe = p => {
				RoutedEventHandlerPair pair = (RoutedEventHandlerPair)p;
				pair.Owner.RemoveHandler(re, pair.Handler);
			};
			bool invoked = false;
			RoutedEventHandlerPair pair2 = null;
			pair2 = subscribe((s, e) => {
				unsubscribe(pair2);
				if(action != null) {
					action();
				}
				invoked = true;
			});
			BusyWait(() => invoked);
		}
		protected void DispatchBusyWait(Func<bool> condition) {
			bool? res = null;
			while(res == null || !(bool)res) {
				DispatchAsync(() => res = condition());
				BusyWait(() => res != null);
				Thread.Sleep(150);
			}
		}
		protected void Dispatch(Action action) {
			bool invoked = false;
			DispatchAsync(() => {
				action();
				invoked = true;
			});
			BusyWait(() => invoked);
		}
		protected T DispatchExpr<T>(Func<T> func) {
			T res = default(T);
			Dispatch(() => res = func());
			return res;
		}
		protected BaseDemoTestingFixture() { }
		protected void AddLoadModuleActions(Type moduleType) {
			var module = DemoBaseTesting.Modules.FirstOrDefault(m => m.ModuleType == moduleType);
			CreateSetCurrentDemoActions(module, true);
		}
		protected virtual void CreateSetCurrentDemoActions(ModuleDescription module, bool checkMemoryLeaks) {
			bool b = false;
			object mutex = new object();
			object handler = DemoBaseTesting.SubscribeToModuleAppear((s, e) => { lock (mutex) { b = true; } });
			DispatchAsync(() => DemoBaseTesting.LoadModule(module, true));
			IAmAlive("Waiting for module " + module + " to load");
			BusyWait(() => { lock (mutex) { return b; } });
			DemoBaseTesting.UnsubscribeFromModuleAppear(handler);
			IAmAlive("Clearing the focus");
			DispatchAsync(() => ResetFocus());
			if(checkMemoryLeaks) {
				IAmAlive("Collecting references for memory leak detection");
				Dispatch(AddCollectReferencesAction);
			} else {
				IAmAlive("Memory leak detection is disabled for this module");
			}
			AssertNoModuleException();
		}
		private void AssertNoModuleException() {
			Assert.IsNull(DemoBaseTesting.DemoModuleException, string.Format("Unhandled exception in demo module:\n{0}", ExceptionHelper.GetMessage(DemoBaseTesting.DemoModuleException)));
		}
		protected void ResetFocus() {
			DemoBaseTesting.ResetFocusElement.Focus();
			System.Windows.Input.FocusManager.SetFocusedElement(Application.Current.MainWindow, DemoBaseTesting.ResetFocusElement);
		}
		protected override void BeforeFinishTesting() {
			base.BeforeFinishTesting();
			DoCheckMemoryLeaks();
		}
		protected virtual bool CheckMemoryLeaks(Type moduleType) { return false; }
		void AddCollectReferencesAction() {
			Type currentModuleType = DemoBaseTesting.CurrentModule.ModuleType;
			if(!CheckMemoryLeaks(currentModuleType)) return;
			List<WeakReference> refList = new List<WeakReference>();
			refTable[currentModuleType] = refList;
			MemoryLeaksHelper.CollectReferences(DemoBaseTesting.CurrentDemoModule, refList);
		}
		void DoCheckMemoryLeaks() {
			System.Threading.Thread.Sleep(3000);
			DevExpress.Xpf.Core.DispatcherHelper.DoEvents(DispatcherPriority.SystemIdle);
			List<LeakingModuleInfo> leakingModules = new List<LeakingModuleInfo>();
			MemoryLeaksHelper.GarbageCollect();
			foreach(KeyValuePair<Type, List<WeakReference>> pair in refTable) {
				foreach(WeakReference reference in pair.Value) {
					if(reference.Target != null) {
						leakingModules.Add(new LeakingModuleInfo(pair.Key, pair.Value));
						break;
					}
				}
			}
			foreach(LeakingModuleInfo info in leakingModules) {
				AssertLog.Fail(info.ModuleType.FullName + " module: memory leaks\r\nLeaks details:\r\n" + info.GetLeakDetails(30));
			}
		}
	}
	public abstract class DemoModulesAccessor<T> where T : DemoModule {
		readonly BaseDemoTestingFixture fixture;
		protected T DemoModule { get { return (T)fixture.DemoBaseTesting.CurrentDemoModule; } }
		protected DemoModulesAccessor(BaseDemoTestingFixture fixture) {
			this.fixture = fixture;
		}
	}
	class LeakingModuleInfo {
		readonly Type moduleType;
		readonly List<WeakReference> leaks;
		public LeakingModuleInfo(Type moduleType, List<WeakReference> objects) {
			this.moduleType = moduleType;
			this.leaks = new List<WeakReference>();
			int count = objects.Count;
			for(int i = 0; i < count; i++) {
				WeakReference reference = objects[i];
				if(reference.Target != null)
					leaks.Add(reference);
			}
		}
		public Type ModuleType { get { return moduleType; } }
		public List<WeakReference> Leaks { get { return leaks; } }
		public string GetLeakDetails(int maxLeakCount) {
			Dictionary<Type, MemoryLeakInfo> leakDetails = new Dictionary<Type, MemoryLeakInfo>();
			int count = leaks.Count;
			for(int i = 0; i < count; i++) {
				object target = leaks[i].Target;
				if(target != null) {
					MemoryLeakInfo info;
					if(!leakDetails.TryGetValue(target.GetType(), out info)) {
						info = new MemoryLeakInfo(target.GetType());
						leakDetails.Add(target.GetType(), info);
					}
					info.InstanceCount++;
				}
			}
			List<MemoryLeakInfo> result = new List<MemoryLeakInfo>();
			foreach(Type type in leakDetails.Keys)
				result.Add(leakDetails[type]);
			result.Sort(new MemoryLeakInfoInstanceCountComparer());
			StringBuilder sb = new StringBuilder();
			count = Math.Min(result.Count, maxLeakCount);
			for(int i = 0; i < count; i++)
				sb.AppendLine(String.Format("Leaked {1} instance(s) of type {0}", result[i].Type.FullName, result[i].InstanceCount));
			return sb.ToString();
		}
	}
	public class MemoryLeakInfoInstanceCountComparer : IComparer<MemoryLeakInfo> {
		public int Compare(MemoryLeakInfo x, MemoryLeakInfo y) {
			return Comparer<int>.Default.Compare(x.InstanceCount, y.InstanceCount);
		}
	}
	public class MemoryLeakInfo {
		readonly Type type;
		int instanceCount;
		public MemoryLeakInfo(Type type) {
			this.type = type;
		}
		public Type Type { get { return type; } }
		public int InstanceCount { get { return instanceCount; } set { instanceCount = value; } }
	}
}
