#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using DevExpress.EasyTest.Framework;
namespace DevExpress.ExpressApp.EasyTest.WebAdapter.Utils {
	public static class FileDownloadDialogHelper {
		private static bool isProcessFileDownloadDialog = false;
		public static IntPtr FindFileDownloadDialog() {
			return Win32Helper.FindWindowEx(new IntPtr(0), 0, "#32770", "File Download");
		}
		public static bool ProcessFileDownloadDialog(WebCommandAdapter adapter) {
			try {
				EasyTestTracer.Tracer.InProcedure("ProcessFileDownloadDialog");
				if(!isProcessFileDownloadDialog) {
					isProcessFileDownloadDialog = true;
					try {
						if(!ProcessIE8FileDownloadDialogAction(adapter, "Save")) {
							EasyTestTracer.Tracer.LogText("ProcessIE8FileDownloadDialogAction false");
							return ProcessIE9NotificationBarSaveAs(adapter);
						}
						return true;
					}
					finally {
						isProcessFileDownloadDialog = false;
					}
				}
				return true;
			}
			finally {
				EasyTestTracer.Tracer.OutProcedure("ProcessFileDownloadDialog");
			}
		}
		public static bool ProcessIE8FileDownloadDialogAction(WebCommandAdapter adapter, string action) {
			try {
				EasyTestTracer.Tracer.InProcedure("ProcessIE8FileDownloadDialogAction");
				IntPtr fileDownloadHWnd = FindFileDownloadDialog();
				if(fileDownloadHWnd != IntPtr.Zero) {
					UnmanagedHandleDialogControl testControl = new UnmanagedHandleDialogControl(fileDownloadHWnd, adapter);
					testControl.ControlType = TestControlType.Action;
					testControl.Name = "Save";
					testControl.Act(null);
					return true;
				}
				return false;
			}
			finally {
				EasyTestTracer.Tracer.OutProcedure("ProcessIE8FileDownloadDialogAction");
			}
		}
		public static bool ProcessIE9NotificationBarSaveAs(WebCommandAdapter adapter) {
			return ProcessIE9NotificationBarSaveAs(adapter, 0);
		}
		public static bool ProcessIE9NotificationBarSaveAs(WebCommandAdapter adapter, int retryCount) {
			try {
				EasyTestTracer.Tracer.InProcedure("ProcessIE9NotificationBarSaveAs");
				EasyTestTracer.Tracer.LogText("SaveDialogOpened state: " + SaveDialogOpened.ToString());
				if(!SaveDialogOpened) {
					IntPtr frameNotificationBarHwnd = FindIE9FrameNotificationBar();
					EasyTestTracer.Tracer.LogText("ProcessIE9NotificationBarSaveAs frameNotificationBarHwnd == " + frameNotificationBarHwnd.ToString());
					if(frameNotificationBarHwnd != IntPtr.Zero) {
						System.Windows.Automation.AutomationElementCollection amc = System.Windows.Automation.AutomationElement.FromHandle(frameNotificationBarHwnd).FindAll(System.Windows.Automation.TreeScope.Children, System.Windows.Automation.Condition.TrueCondition);
						foreach(System.Windows.Automation.AutomationElement _element in amc) {
							EasyTestTracer.Tracer.LogText("AutomationElement item name: " + _element.Current.Name);
							if(_element.Current.Name == "Notification" || _element.Current.Name == "Notification bar") {
								EasyTestTracer.Tracer.LogText("Notification bar found");
								foreach(System.Windows.Automation.AutomationElement barItem in _element.FindAll(System.Windows.Automation.TreeScope.Children, System.Windows.Automation.Condition.TrueCondition)) {
									EasyTestTracer.Tracer.LogText("AutomationElement barItem name: " + barItem.Current.Name);
									if(barItem.Current.Name == "Save") {
										System.Windows.Automation.AutomationElementCollection bmc = barItem.FindAll(System.Windows.Automation.TreeScope.Children, System.Windows.Automation.Automation.ControlViewCondition);
										System.Windows.Automation.InvokePattern click1 = (System.Windows.Automation.InvokePattern)bmc[0].GetCurrentPattern(System.Windows.Automation.AutomationPattern.LookupById(10000));
										click1.Invoke();
										EasyTestTracer.Tracer.LogText("Save found. Sleep for 1000 ms.");
										Thread.Sleep(1000);
										foreach(IEasyTestWebBrowser browser in adapter.WebAdapter.WebBrowsers) {
											System.Windows.Automation.TreeWalker trw = new System.Windows.Automation.TreeWalker(System.Windows.Automation.Condition.TrueCondition);
											System.Windows.Automation.AutomationElement mainWindow = trw.GetParent(System.Windows.Automation.AutomationElement.FromHandle(new IntPtr(browser.BrowserWindowHandle)));
											System.Windows.Automation.AutomationElementCollection main = mainWindow.FindAll(System.Windows.Automation.TreeScope.Children
											, System.Windows.Automation.Condition.TrueCondition);
											foreach(System.Windows.Automation.AutomationElement el in main) {
												EasyTestTracer.Tracer.LogText("AutomationElement all items list LocalizedControlType name: " + el.Current.LocalizedControlType);
											}
											foreach(System.Windows.Automation.AutomationElement el in main) {
												EasyTestTracer.Tracer.LogText("AutomationElement LocalizedControlType name: " + el.Current.LocalizedControlType);
												if(el.Current.LocalizedControlType == "menu") {
													EasyTestTracer.Tracer.LogText("Save menu found");
													EasyTestTracer.Tracer.LogText("iteration: " + retryCount);
													System.Windows.Automation.InvokePattern clickMenu = (System.Windows.Automation.InvokePattern)
																el.FindAll(System.Windows.Automation.TreeScope.Children, System.Windows.Automation.Condition.TrueCondition)[1].GetCurrentPattern(System.Windows.Automation.AutomationPattern.LookupById(10000));
													clickMenu.Invoke();
													EasyTestTracer.Tracer.LogText("Menu item invoked. Sleep for 100 ms.");
													Thread.Sleep(100);
													SaveDialogOpened = true;
													return true;
												}
											}
										}
									}
								}
								EasyTestTracer.Tracer.LogText("Find AutomationElement break!!!");
								break;
							}
						}
					}
				}
				if(retryCount <= 3) {
					retryCount++;
					EasyTestTracer.Tracer.LogText("retry ProcessIE9NotificationBarSaveAs. Iteration: " + retryCount);
					return ProcessIE9NotificationBarSaveAs(adapter, retryCount);
				}
				return false;
			}
			finally {
				EasyTestTracer.Tracer.OutProcedure("ProcessIE9NotificationBarSaveAs");
			}
		}
		[ThreadStaticAttribute]
		private static bool _saveDialogOpened = false;
		public static bool SaveDialogOpened {
			get {
				return _saveDialogOpened;
			}
			set {
				EasyTestTracer.Tracer.LogText(string.Format("Set SaveDialogOpened. Old value: {0}, New value: {1}", _saveDialogOpened, value));
				_saveDialogOpened = value;
			}
		}
		public static void ProcessFileDownloadDialogClose(WebCommandAdapter adapter) {
			if(!ProcessIE8FileDownloadDialogAction(adapter, "Close")) {
				ProcessIE9NotificationBarClose();
			}
		}
		public static void ProcessIE9NotificationBarClose() {
			IntPtr frameNotificationBarHwnd = FindIE9FrameNotificationBar();
			if(frameNotificationBarHwnd != IntPtr.Zero) {
				System.Windows.Automation.AutomationElementCollection amc = System.Windows.Automation.AutomationElement.FromHandle(frameNotificationBarHwnd).FindAll(System.Windows.Automation.TreeScope.Children, System.Windows.Automation.Condition.TrueCondition);
				foreach(System.Windows.Automation.AutomationElement _element in amc) {
					if(_element.Current.Name == "Notification") {
						foreach(System.Windows.Automation.AutomationElement barItem in _element.FindAll(System.Windows.Automation.TreeScope.Children, System.Windows.Automation.Condition.TrueCondition)) {
							if(barItem.Current.Name == "Close") {
								Thread.Sleep(100);
								System.Windows.Automation.AutomationPattern[] pats = barItem.GetSupportedPatterns();
								foreach(System.Windows.Automation.AutomationPattern pat in pats) {
									if(pat.Id == 10000) {
										System.Windows.Automation.InvokePattern click = (System.Windows.Automation.InvokePattern)barItem.GetCurrentPattern(pat);
										click.Invoke();
										break;
									}
								}
								break;
							}
						}
						break;
					}
				}
			}
		}
		private static IntPtr FindIE9FrameNotificationBar() {
			EasyTestTracer.Tracer.InProcedure("FindIE9FrameNotificationBar");
			IntPtr ieFrameHWnd = Win32Helper.FindWindowEx(new IntPtr(0), 0, "IEFrame", null);
			if(ieFrameHWnd != IntPtr.Zero) {
				EasyTestTracer.Tracer.LogText("ieFrameHWnd: " + ieFrameHWnd.ToString());
				IntPtr frameNotificationBarHWnd = Win32Helper.FindWindowEx(ieFrameHWnd, 0, "Frame Notification Bar", null);
				EasyTestTracer.Tracer.OutProcedure("FindIE9FrameNotificationBar");
				return frameNotificationBarHWnd;
			}
			EasyTestTracer.Tracer.OutProcedure("FindIE9FrameNotificationBar");
			return IntPtr.Zero;
		}
	}
	public class Win32Helper {
		private const uint WM_LBUTTONDOWN = 0x0201;
		private const uint WM_LBUTTONUP = 0x0202;
		private const uint MOUSEEVENTF_MOVE = 0x0001,
			MOUSEEVENTF_LEFTDOWN = 0x0002,
			MOUSEEVENTF_LEFTUP = 0x0004,
			MOUSEEVENTF_RIGHTDOWN = 0x0008,
			MOUSEEVENTF_RIGHTUP = 0x0010,
			MOUSEEVENTF_MIDDLEDOWN = 0x0020,
			MOUSEEVENTF_MIDDLEUP = 0x0040,
			MOUSEEVENTF_WHEEL = 0x0800,
			MOUSEEVENTF_ABSOLUTE = 0x8000;
		private const int defaultMouseMoveDelayPerPixels = 50;
		private IntPtr hWnd = IntPtr.Zero;
		private int parentProcessId = -1;
		private int processId = -1;
		public const uint WM_COMMAND = 0x0111;
		public const ushort BN_CLICKED = 0x0;
		public const uint WM_SETTEXT = 0x000C;
		public const uint WM_GETTEXT = 0x000D;
		public const uint WM_GETTEXTLENGTH = 0x000E;
		public const ushort ButtonOpenID = 1;
		public const uint WM_CLEAR = 0x0303;
		public const uint EM_SETSEL = 0xB1;
		public const uint WM_SETFOCUS = 0x0007;
		public const uint WM_ACTIVATEAPP = 0x001C;
		public const int SW_RESTORE = 9;
		public static IntPtr GetParentIEFrameWindow(IntPtr hWnd) {
			int currentWindowHandle = hWnd.ToInt32();
			do {
				currentWindowHandle = Win32Helper.GetParent(currentWindowHandle);
			} while(currentWindowHandle != 0 && GetClassName(new IntPtr(currentWindowHandle)) != "IEFrame");
			return new IntPtr(currentWindowHandle);
		}
		public static int GetRootWindow(int hWnd) {
			int currentWindowHandle = hWnd;
			int result = currentWindowHandle;
			do {
				currentWindowHandle = Win32Helper.GetParent(currentWindowHandle);
				if(currentWindowHandle != 0) {
					result = currentWindowHandle;
				}
			} while(currentWindowHandle != 0);
			return result;
		}
		public IntPtr GetChildWndForProcess(IntPtr parent, string wndClassName, int parentProcessId, int processId) {
			this.parentProcessId = parentProcessId;
			this.processId = processId;
			GCHandle gch = GCHandle.Alloc(wndClassName);
			try {
				EnumChildWindows(parent, FindWndByClassName, (IntPtr)gch);
			}
			finally {
				if(gch.IsAllocated) {
					gch.Free();
				}
			}
			return this.hWnd;
		}
		public IntPtr GetChildWndByClassName(IntPtr parent, string wndClassName) {
			return GetChildWndForProcess(parent, wndClassName, -1, -1);
		}
		public static void MouseClick(IntPtr hWnd, Point point, MouseButtons mouseButtons) {
			try {
				EasyTestTracer.Tracer.InProcedure("MouseClick");
				POINT globalPoint = new POINT(point.X, point.Y);
				MapWindowPoints(hWnd, IntPtr.Zero, ref globalPoint, 1);
				point = new Point(globalPoint.X, globalPoint.Y);
				MoveMousePointTo(point);
				System.Threading.Thread.Sleep(300);
				mouse_event(GetMouseFlagsByMouseButtons(mouseButtons, true), Convert.ToInt32(point.X), Convert.ToInt32(point.Y), 0, IntPtr.Zero);
				System.Threading.Thread.Sleep(300);
				Application.DoEvents();
				mouse_event(GetMouseFlagsByMouseButtons(mouseButtons, false), Convert.ToInt32(point.X), Convert.ToInt32(point.Y), 0, IntPtr.Zero);
				System.Threading.Thread.Sleep(300);
				Application.DoEvents();
			}
			finally {
				EasyTestTracer.Tracer.OutProcedure("MouseClick");
			}
		}
		public static void ButtonClick(IntPtr buttonHandle) {
			ButtonClick(buttonHandle, 10, 10);
		}
		public static void ButtonClick(IntPtr buttonHandle, int left, int top) {
			Win32Helper.SetForegroundWindow(buttonHandle);
			System.Threading.Thread.Sleep(300);
			Win32Helper.POINT globalPoint = new Win32Helper.POINT(left, top);
			Win32Helper.MapWindowPoints(buttonHandle, IntPtr.Zero, ref globalPoint, 1);
			Win32Helper.MoveMousePointTo(new System.Drawing.Point(globalPoint.X, globalPoint.Y));
			System.Threading.Thread.Sleep(400);
			Win32Helper.SendMessage(buttonHandle.ToInt32(), 245, 0, 0);
			System.Windows.Forms.Application.DoEvents();
			System.Threading.Thread.Sleep(1000);
		}
		public static void MoveMousePointTo(Point pt) {
			int timeoutMilliseconds = 5000;
			DateTime endTime = DateTime.Now.AddMilliseconds(timeoutMilliseconds);
			int countX = 1, countY = 1;
			while(!Cursor.Position.Equals(pt)) {
				Point cpt = Cursor.Position;
				int dX = cpt.X - pt.X, dY = cpt.Y - pt.Y;
				double dXY = dY != 0 ? Math.Abs((double)dX / (double)dY) : 1;
				double dYX = dX != 0 ? Math.Abs((double)dY / (double)dX) : 1;
				if((cpt.X == pt.X) || (dXY * (countX++) <= 1))
					dX = 0;
				else {
					dX = cpt.X < pt.X ? 1 : -1;
					countX = 1;
				}
				if((cpt.Y == pt.Y) || (dYX * (countY++) <= 1))
					dY = 0;
				else {
					dY = cpt.Y < pt.Y ? 1 : -1;
					countY = 1;
				}
				cpt.X += dX;
				cpt.Y += dY;
				Cursor.Position = cpt;
				int x = Convert.ToInt32((65536.0 * cpt.X / Screen.PrimaryScreen.Bounds.Width) + (65536.0 / Screen.PrimaryScreen.Bounds.Width / 2));
				int y = Convert.ToInt32((65536.0 * cpt.Y / Screen.PrimaryScreen.Bounds.Height) + (65536.0 / Screen.PrimaryScreen.Bounds.Height / 2));
				mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, x, y, 0, IntPtr.Zero);
				Application.DoEvents();
				if(DateTime.Now > endTime) {
					EasyTestTracer.Tracer.LogText("MoveMousePointTo: Timeout expired:" + timeoutMilliseconds);
					break;
				}
			}
		}
		private static uint GetMouseFlagsByMouseButtons(MouseButtons mouseButtons, bool down) {
			uint flags = MOUSEEVENTF_ABSOLUTE;
			switch(mouseButtons) {
				case MouseButtons.Left:
					flags = down ? MOUSEEVENTF_LEFTDOWN : MOUSEEVENTF_LEFTUP;
					break;
				case MouseButtons.Middle:
					flags = down ? MOUSEEVENTF_RIGHTDOWN : MOUSEEVENTF_RIGHTUP;
					break;
				case MouseButtons.Right:
					flags = down ? MOUSEEVENTF_MIDDLEDOWN : MOUSEEVENTF_MIDDLEUP;
					break;
			}
			return flags;
		}
		private bool FindWndByClassName(IntPtr hWnd, IntPtr arg) {
			GCHandle gch = (GCHandle)arg;
			string className = (string)gch.Target;
			if(className.Equals(GetClassName(hWnd))) {
				if(parentProcessId == -1 && processId == -1) {
					this.hWnd = hWnd;
					return false;
				}
				else {
					int currentProcessId = 0;
					GetWindowThreadProcessId(hWnd, out currentProcessId);
					int currentParentProcessId = ParentProcessUtilities.GetParentProcessId(currentProcessId);
					if(currentParentProcessId == parentProcessId || currentProcessId == processId) {
						this.hWnd = hWnd;
						return false;
					}
					return true;
				}
			}
			return true;
		}
		private static string GetClassName(IntPtr hWnd) {
			StringBuilder className = new StringBuilder(256);
			GetClassName(hWnd, className, className.Capacity);
			return className.ToString();
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct RECT {
			public int Left;		
			public int Top;		 
			public int Right;	   
			public int Bottom;	  
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct POINT {
			public int X;
			public int Y;
			public POINT(int x, int y) {
				X = x;
				Y = y;
			}
		}
		public delegate bool EnumWindowProc(IntPtr hWnd, IntPtr parameter);
		[DllImport("user32.dll")]
		extern public static void mouse_event(uint dwFlags, int dx, int dy, uint dwData, IntPtr dwExtraInfo);
		[DllImport("USER32.DLL", EntryPoint = "FindWindowExW", CharSet = CharSet.Unicode)]
		public static extern IntPtr FindWindowEx(IntPtr hwndParent, uint hwndChildAfter, string ClassName, string WindowName);
		[DllImport("user32.dll")]
		public static extern Int64 SendMessage(int hWnd, uint Msg, uint wParam, uint lParam);
		[DllImport("user32.dll")]
		public static extern IntPtr SetForegroundWindow(IntPtr hWnd);
		[DllImport("user32.dll")]
		public static extern int GetWindowText(int hWnd, System.Text.StringBuilder text, int count);
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetWindowRect(int hWnd, out RECT lpRect);
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern int MapWindowPoints(IntPtr hWndFrom, IntPtr hWndTo, ref POINT pt, int cPoints);
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
		[DllImport("user32")]
		private static extern bool EnumChildWindows(IntPtr window, EnumWindowProc callback, IntPtr i);
		[DllImport("user32")]
		public static extern int GetLastActivePopup(IntPtr parentWindow);
		[DllImport("user32.dll", SetLastError = true)]
		public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);
		[DllImport("USER32.DLL", EntryPoint = "SendMessage")]
		public static extern uint SendMessage(IntPtr hWnd, uint Msg, uint wParam, string lParam);
		[DllImport("USER32.DLL", EntryPoint = "SendMessage")]
		public static extern int SendMessage(IntPtr hWnd, uint Msg, uint wParam, IntPtr lParam);
		[DllImport("USER32.DLL", EntryPoint = "SendMessage")]
		public static extern int SendMessage(IntPtr hWnd, uint Msg, uint wParam, int lParam);
		[DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
		public static extern bool SendMessage(IntPtr hWnd, uint Msg, int wParam, StringBuilder lParam);
		[DllImport("USER32.DLL", EntryPoint = "GetParent")]
		public static extern int GetParent(int hWnd);
		[DllImport("user32.dll")]
		public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);
		[DllImport("user32.dll")]
		public static extern int SetActiveWindow(IntPtr hWnd);
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr GetConsoleWindow();
		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
	}
	public struct ParentProcessUtilities {
		internal IntPtr Reserved1;
		internal IntPtr PebBaseAddress;
		internal IntPtr Reserved2_0;
		internal IntPtr Reserved2_1;
		internal IntPtr UniqueProcessId;
		internal IntPtr InheritedFromUniqueProcessId;
		[DllImport("ntdll.dll")]
		private static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass, ref ParentProcessUtilities processInformation, int processInformationLength, out int returnLength);
		public static int GetParentProcessId(int id) {
			Process process = Process.GetProcessById(id);
			IntPtr processHandle;
			try {
				processHandle = process.Handle;
			}
			catch(Win32Exception) {
				return -1;
			}
			return GetParentProcessId(process.Handle);
		}
		public static int GetParentProcessId(IntPtr processHandle) {
			ParentProcessUtilities pbi = new ParentProcessUtilities();
			int returnLength;
			int status = NtQueryInformationProcess(processHandle, 0, ref pbi, Marshal.SizeOf(pbi), out returnLength);
			if(status != 0) {
				return 0;
			}
			try {
				return pbi.InheritedFromUniqueProcessId.ToInt32();
			}
			catch(ArgumentException) {
				return 0;
			}
		}
	}
}
