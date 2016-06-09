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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Input;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
namespace DevExpress.Utils.TouchHelpers {
	[Guid("41C81592-514C-48BD-A22E-E6AF638521A6")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IInputPanelConfiguration {
		int EnableFocusTracking();
	}
	[ComImport, Guid("2853ADD3-F096-4C63-A78F-7FA3EA837FB7")]
	class InputPanelConfiguration {
	}
	public static class TouchKeyboardSupport {
		static bool enableTouchKeyboard = false;
		public static bool EnableTouchKeyboard {
			get { return enableTouchKeyboard; }
			set {
				if(value && !IsWindows8)
					value = false;
				if(enableTouchKeyboard == value) return;
				enableTouchKeyboard = value;
				if(enableTouchKeyboard)
					enableTouchKeyboard = DisableTabletSupport();
			}
		}
		static bool? isWindows8;
		public static bool IsWindows8 {
			get {
				if(isWindows8 == null) 
					isWindows8 = DetectWindows8();
				return isWindows8.Value;
			}
		}
		static bool DetectWindows8() {
			try {
				var win8version = new Version(6, 2, 9200, 0);
				if(Environment.OSVersion.Platform == PlatformID.Win32NT &&
					Environment.OSVersion.Version >= win8version) {
					return true;
				}
			}
			catch { }
			return false;
		}
		public static void CheckEnableTouchSupport(Form form) {
			CheckEnableTouchSupportCore(form);
		}
		[System.Security.SecuritySafeCritical]
		public static void EnableFocusTracking() {
			InputPanelConfiguration cp = new InputPanelConfiguration();
			IInputPanelConfiguration icp = cp as IInputPanelConfiguration;
			if(icp != null)
				icp.EnableFocusTracking();
		}
		[System.Security.SecuritySafeCritical]
		static void CheckEnableTouchSupportCore(Form form) {
			if(!EnableTouchKeyboard) return;
			if(form.IsHandleCreated) {
				System.Windows.Automation.AutomationElement asForm =
				  System.Windows.Automation.AutomationElement.FromHandle(form.Handle);
				EnableFocusTracking();
			}
			else {
				EventHandler handleCreated = null;
				EventHandler disposed = null;
				FormClosedEventHandler closed = null;
				closed = (s, e) => {
					Form f = (Form)s;
					f.FormClosed -= closed;
					f.Disposed -= disposed;
					f.HandleCreated -= handleCreated;
				};
				disposed = (s, e) => {
					Form f = (Form)s;
					f.FormClosed -= closed;
					f.Disposed -= disposed;
					f.HandleCreated -= handleCreated;
				};
				handleCreated = (s, e) => { 
					((Form)s).HandleCreated -= handleCreated;
					CheckEnableTouchSupport((Form)s);
				};
			}
		}
		public static bool DisableTabletSupport() {
			return DisableTabletSupportCore();
		}
		[System.Security.SecuritySafeCritical]
		static bool DisableTabletSupportCore() {
			TabletDeviceCollection devices = System.Windows.Input.Tablet.TabletDevices;
			if(devices.Count > 0) {
				Type inputManagerType = typeof(System.Windows.Input.InputManager);
				object stylusLogic = inputManagerType.InvokeMember("StylusLogic",
							BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
							null, InputManager.Current, null);
				if(stylusLogic != null) {
					Type stylusLogicType = stylusLogic.GetType();
					int maxAttemptsCount = devices.Count * 2;
					while(devices.Count > 0 && maxAttemptsCount > 0) {
						stylusLogicType.InvokeMember("OnTabletRemoved",
								BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic,
								null, stylusLogic, new object[] { (uint)0 });
						maxAttemptsCount--;
					}
				}
			}
			return devices.Count == 0;
		}
	}
}
namespace DevExpress.Utils.Internal {
	#region TouchUIAdapter
	public static class TouchUIAdapter {
		public static float GetFactor(UserLookAndFeel lookAndFeel) {
			return 1.4f + lookAndFeel.GetTouchScaleFactor() / 10.0f;
		}
		public static SizeF GetSize(UserLookAndFeel lookAndFeel) {
			float factor = GetFactor(lookAndFeel);
			return new SizeF(factor, factor);
		}
	}
	#endregion
	#region FormTouchUIAdapter
	public static class FormTouchUIAdapter {
		#region ShowDialog
		public static DialogResult ShowDialog(Form form, IWin32Window owner) {
			return ShowCore(form, owner, true);
		}
		public static DialogResult ShowDialog(Form form) {
			return ShowCore(form, null, true);
		}
		#endregion
		#region Show
		public static DialogResult Show(Form form, IWin32Window owner) {
			return ShowCore(form, owner, false);
		}
		public static DialogResult Show(Form form) {
			return ShowCore(form, null, false);
		}
		#endregion
		static DialogResult ShowCore(Form form, IWin32Window owner, bool isModal) {
			DialogResult result = System.Windows.Forms.DialogResult.None;
			try {
				form.Load += OnFormLoad;
				if (isModal)
					result = ShowDialogCore(form, owner);
				else
					ShowFormCore(form, owner);
			}
			finally {
				form.Load -= OnFormLoad;
			}
			return result;
		}
		static DialogResult ShowDialogCore(Form form, IWin32Window owner) {
			return owner != null ? form.ShowDialog(owner) : form.ShowDialog();
		}
		static void ShowFormCore(Form form, IWin32Window owner) {
			if (owner != null)
				form.Show(owner);
			else
				form.Show();
		}
		static void OnFormLoad(object sender, EventArgs e) {
			XtraForm form = sender as XtraForm;
			if (form == null)
				return;
			UserLookAndFeel activeLookAndFeel = UserLookAndFeel.Default.ActiveLookAndFeel;
			if (activeLookAndFeel.GetTouchUI())
				form.Scale(TouchUIAdapter.GetSize(activeLookAndFeel));
		}
	}
	#endregion
}
