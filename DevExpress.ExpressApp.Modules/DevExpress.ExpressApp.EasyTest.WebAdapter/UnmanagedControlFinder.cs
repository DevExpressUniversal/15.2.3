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
using System.Collections.Generic;
using System.Text;
using System.Windows.Automation;
using DevExpress.EasyTest.Framework;
using DevExpress.ExpressApp.EasyTest.WebAdapter.TestControls;
using DevExpress.ExpressApp.EasyTest.WebAdapter.Utils;
using System.Threading;
namespace DevExpress.ExpressApp.EasyTest.WebAdapter {
	public class UnmanagedControlFinder {
		private WebCommandAdapter adapter;
		public UnmanagedControlFinder(WebCommandAdapter adapter) {
			this.adapter = adapter;
		}
		public ITestControl FindControl(ITestControls testControls, string controlType, string name) {
			return FindControlCore(testControls, controlType, name, 0);
		}
		public ITestControl FindControlCore(ITestControls testControls, string controlType, string name, int iteration) {
			EasyTestTracer.Tracer.InProcedure("UnmanagedControlFinder.FindControl");
			EasyTestTracer.Tracer.LogText("controlType: " + controlType);
			EasyTestTracer.Tracer.LogText("name: " + name);
			ITestControl result = null;
			EasyTestTracer.Tracer.LogText("Iterating adapter.WebAdapter.WebBrowsers");
			foreach(IEasyTestWebBrowser browser in adapter.WebAdapter.WebBrowsers) {
				EasyTestTracer.Tracer.LogText("browser.Browser.HWND: " + browser.Browser.HWND);
				EasyTestTracer.Tracer.LogText("browser.Browser.Visible: " + browser.Browser.Visible);
				EasyTestTracer.Tracer.LogText("browser.BrowserWindowHandle: " + browser.BrowserWindowHandle);
				EasyTestTracer.Tracer.LogText("browser.DialogWindowHandle: " + browser.DialogWindowHandle);
				EasyTestTracer.Tracer.LogText("FileDownloadDialogHelper.SaveDialogOpened: " + FileDownloadDialogHelper.SaveDialogOpened);
				if(FileDownloadDialogHelper.SaveDialogOpened) {
					EasyTestTracer.Tracer.InProcedure("TestBBB");
					System.Windows.Automation.TreeWalker trw = new System.Windows.Automation.TreeWalker(System.Windows.Automation.Condition.TrueCondition);
					EasyTestTracer.Tracer.LogText("getting mainwindow ");
					System.Windows.Automation.AutomationElement mainWindow = trw.GetParent(System.Windows.Automation.AutomationElement.FromHandle(new IntPtr(browser.BrowserWindowHandle)));
					EasyTestTracer.Tracer.LogText("mainwindow: " + mainWindow);
					EasyTestTracer.Tracer.LogText("and saveAsDialog on it ");
					AutomationElement saveAsDialog = mainWindow.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, "Save As"));
					if(saveAsDialog != null) {
						EasyTestTracer.Tracer.LogText("saveAsDialog found: " + saveAsDialog);
						EasyTestTracer.Tracer.LogText("wrapping it with UnmanagedHandleDialogControl ");
						UnmanagedHandleDialogControl handleDialog = new UnmanagedHandleDialogControl(saveAsDialog);
						handleDialog.Name = name;
						handleDialog.ControlType = controlType;
						result = handleDialog;
					}
					else {
						EasyTestTracer.Tracer.LogText("saveAsDialog not found!!!!!!!!!!!" );
						if(iteration == 0) {
							EasyTestTracer.Tracer.LogText("wait 1 sec and try again;");
							Thread.Sleep(1000);
							return FindControlCore(testControls, controlType, name, 1);
						}
					}
					EasyTestTracer.Tracer.OutProcedure("TestBBB");
				}
				else {
					EasyTestTracer.Tracer.InProcedure("TestAAA");
					IntPtr dialogHandle = browser.DialogWindowHandle;
					EasyTestTracer.Tracer.LogText("browser.DialogWindowHandle != IntPtr.Zero: " + (dialogHandle != IntPtr.Zero));
					if(dialogHandle != IntPtr.Zero) {
						EasyTestTracer.Tracer.LogText("wrapping dialog handle with UnmanagedHandleDialogControl ");
						UnmanagedHandleDialogControl handleDialog = new UnmanagedHandleDialogControl(dialogHandle, adapter);
						handleDialog.Name = name;
						handleDialog.ControlType = controlType;
						result = handleDialog;
					}
					EasyTestTracer.Tracer.OutProcedure("TestAAA");
				}
			}
			EasyTestTracer.Tracer.OutProcedure("UnmanagedControlFinder.FindControl");
			if(result == null) {
				FileDownloadDialogHelper.SaveDialogOpened = false;
			}
			return result;
		}
		public static IntPtr FindUnmanagedControl(IntPtr parentHandle, string className, List<string> captions) {
			IntPtr result = IntPtr.Zero;
			if(captions == null) {
				result = Win32Helper.FindWindowEx(parentHandle, 0, className, null);
			}
			else {
				foreach(string buttonCaption in captions) {
					result = Win32Helper.FindWindowEx(parentHandle, 0, className, buttonCaption);
					if(result != IntPtr.Zero) {
						break;
					}
					result = Win32Helper.FindWindowEx(parentHandle, 0, className, "&" + buttonCaption);
					if(result != IntPtr.Zero) {
						break;
					}
				}
			}
			if(result == IntPtr.Zero) {
				IntPtr childHandle = IntPtr.Zero;
				do {
					childHandle = Win32Helper.FindWindowEx(parentHandle, (uint)childHandle.ToInt32(), null, null);
					if(childHandle != IntPtr.Zero) {
						result = FindUnmanagedControl(childHandle, className, captions);
						if(result != IntPtr.Zero) {
							break;
						}
					}
				} while(childHandle != IntPtr.Zero);
			}
			return result;
		}
		public static IntPtr FindUnmanagedButton(IntPtr dialogHandle, List<string> buttonCaptions) {
			return FindUnmanagedControl(dialogHandle, "Button", buttonCaptions);
		}
	}
	public class UnmanagedHandleDialogControl : TestControlBase, IControlAct, ITestWindow, IControlText {
		private IntPtr dialogHandle;
		private const int maxCaptionLength = 256;
		WebCommandAdapter adapter = null;
		AutomationElement saveAsDialog;
		public UnmanagedHandleDialogControl(IntPtr dialogHandle, WebCommandAdapter adapter) {
			EasyTestTracer.Tracer.LogText("UnmanagedHandleDialogControl created ");
			EasyTestTracer.Tracer.LogText("dialogHandle: ", dialogHandle);
			EasyTestTracer.Tracer.LogText("adapter: ", adapter);
			this.dialogHandle = dialogHandle;
			this.adapter = adapter;
		}
		public UnmanagedHandleDialogControl(AutomationElement saveAsDialog) {
			EasyTestTracer.Tracer.LogText("UnmanagedHandleDialogControl created ");
			EasyTestTracer.Tracer.LogText("saveAsDialog: ", saveAsDialog);
			this.saveAsDialog = saveAsDialog;
		}
		public IntPtr FindControlInOpenSaveDialog(IntPtr dialogHandle, string testTagType, string name) {
			if(testTagType == TestControlType.Field && name == "File name:") {
				IntPtr result = UnmanagedControlFinder.FindUnmanagedControl(dialogHandle, "Edit", null);
				if(result != IntPtr.Zero)
					return result;
			}
			return IntPtr.Zero;
		}
		#region IControlAct Members
		public void Act(string value) {
			try {
				EasyTestTracer.Tracer.InProcedure("UnmanagedHandleDialogControl.Act");
				EasyTestTracer.Tracer.LogText("UnmanagedHandleDialogControl act called");
				EasyTestTracer.Tracer.LogText("ControlType: " + ControlType);
				EasyTestTracer.Tracer.LogText("this.Name: " + this.Name);
				if(ControlType == TestControlType.Action) {
					if(saveAsDialog != null) {
						EasyTestTracer.Tracer.LogText("saveAsDialog: " + saveAsDialog);
						EasyTestTracer.Tracer.LogText("searching the save button in saveAsDialog");
						var saveButton = saveAsDialog.FindFirst(TreeScope.Descendants, new AndCondition(new PropertyCondition(AutomationElement.NameProperty, "Save"),
								new PropertyCondition(AutomationElement.ControlTypeProperty, System.Windows.Automation.ControlType.Button)));
						EasyTestTracer.Tracer.LogText("save button found: " + (saveButton == null));
						EasyTestTracer.Tracer.LogText("getting its pattern");
						InvokePattern pattern = (InvokePattern)saveButton.GetCurrentPattern(InvokePattern.Pattern);
						EasyTestTracer.Tracer.LogText("invoking pattren");
						pattern.Invoke();
						EasyTestTracer.Tracer.LogText("invoking pattren done");
						EasyTestTracer.Tracer.LogText("setting SaveDialogOpened to false");
						FileDownloadDialogHelper.SaveDialogOpened = false;
						System.Threading.Thread.Sleep(300);
						EasyTestTracer.Tracer.LogText("call FileDownloadDialogHelper.ProcessIE9NotificationBarClose();");
						FileDownloadDialogHelper.ProcessIE9NotificationBarClose();
						return;
					}
					else {
						EasyTestTracer.Tracer.LogText("saveAsDialog is null ");
						List<string> targetActions = new List<string>();
						List<string> okActions = new List<string>(new string[] { "Yes", "OK" });
						List<string> cancelActions = new List<string>(new string[] { "No", "Cancel" });
						if(okActions.Contains(this.Name)) {
							targetActions = okActions;
						}
						else if(cancelActions.Contains(this.Name)) {
							targetActions = cancelActions;
						}
						else {
							targetActions.Add(this.Name);
						}
						EasyTestTracer.Tracer.LogText("searching buttons: ");
						foreach(string action in targetActions) {
							EasyTestTracer.Tracer.LogText(action);
						}
						IntPtr buttonHandle = UnmanagedControlFinder.FindUnmanagedButton(dialogHandle, targetActions);
						if(buttonHandle != IntPtr.Zero) {
							EasyTestTracer.Tracer.LogText("button found: " + buttonHandle);
							EasyTestTracer.Tracer.LogText("clicking it");
							Win32Helper.ButtonClick(buttonHandle);
							adapter.WaitForBrowserResponse(false);
							FileDownloadDialogHelper.ProcessIE9NotificationBarClose();
							return;
						}
					}
				}
				throw new AdapterOperationException("The '" + this.Name + "' item is not found");
			}
			finally {
				EasyTestTracer.Tracer.OutProcedure("UnmanagedHandleDialogControl.Act");
			}
		}
		#endregion
		#region ITestWindow Members
		public string Caption {
			get {
				StringBuilder stringBuilder = new StringBuilder(maxCaptionLength);
				if(Win32Helper.GetWindowText(dialogHandle.ToInt32(), stringBuilder, maxCaptionLength) > 0) {
					return stringBuilder.ToString();
				}
				return string.Empty;
			}
		}
		public IntPtr GetActiveWindowHandle() {
			return dialogHandle;
		}
		public void SetWindowSize(int width, int height) {
		}
		#endregion
		#region ITestWindow Members
		public void Close() {
			throw new NotImplementedException();
		}
		#endregion
		#region IControlText Members
		public string Text {
			get {
				throw new NotImplementedException();
			}
			set {
				EasyTestTracer.Tracer.InProcedure("UnmanagedHandleDialogControl.GetText");
				EasyTestTracer.Tracer.LogText("ControlType: " + ControlType);
				EasyTestTracer.Tracer.LogText("Name: " + Name);
				if(ControlType == TestControlType.Field) {
					if(saveAsDialog != null) {
						EasyTestTracer.Tracer.LogText("saveAsDialog: " + saveAsDialog);
						EasyTestTracer.Tracer.LogText("lookign for the File Name text box  in it");
						AutomationElement textBox = saveAsDialog.FindFirst(TreeScope.Descendants, new AndCondition(
					   new PropertyCondition(AutomationElement.NameProperty, "File name:"),
					   new PropertyCondition(AutomationElement.ControlTypeProperty, System.Windows.Automation.ControlType.Edit)));
						IntPtr controlHandle = new IntPtr(textBox.Current.NativeWindowHandle);
						EasyTestTracer.Tracer.LogText("setting focus to text box");
						Win32Helper.SendMessage(controlHandle, Win32Helper.WM_SETFOCUS, 0, 0);
						EasyTestTracer.Tracer.LogText("setting selection to text box");
						Win32Helper.SendMessage(controlHandle, Win32Helper.EM_SETSEL, 0, -1);
						EasyTestTracer.Tracer.LogText("setting clearing text box");
						Win32Helper.SendMessage(controlHandle, Win32Helper.WM_CLEAR, 0, IntPtr.Zero);
						EasyTestTracer.Tracer.LogText("setting text to text box: " + value);
						Win32Helper.SendMessage(controlHandle, Win32Helper.WM_SETTEXT, 0, value);
					}
					else {
						EasyTestTracer.Tracer.LogText("saveAsDialog is null");
						EasyTestTracer.Tracer.LogText("lookign for the File Name text box ");
						IntPtr controlHandle = FindControlInOpenSaveDialog(dialogHandle, ControlType, Name);
						if(controlHandle == IntPtr.Zero) {
							EasyTestTracer.Tracer.LogText("controlHandle is null");
							throw new AdapterOperationException(string.Format("Cannot find the '{0}' field in the 'UnmanagedHandleDialog' control", Name));
						}
						EasyTestTracer.Tracer.LogText("controlHandle found");
						EasyTestTracer.Tracer.LogText("setting focus to text box");
						Win32Helper.SendMessage(controlHandle, Win32Helper.WM_SETFOCUS, 0, 0);
						EasyTestTracer.Tracer.LogText("setting selection to text box");
						Win32Helper.SendMessage(controlHandle, Win32Helper.EM_SETSEL, 0, -1);
						EasyTestTracer.Tracer.LogText("setting clearing text box");
						Win32Helper.SendMessage(controlHandle, Win32Helper.WM_CLEAR, 0, IntPtr.Zero);
						EasyTestTracer.Tracer.LogText("setting text to text box: " + value);
						Win32Helper.SendMessage(controlHandle, Win32Helper.WM_SETTEXT, 0, value);
					}
				}
			}
		}
		#endregion
	}
}
