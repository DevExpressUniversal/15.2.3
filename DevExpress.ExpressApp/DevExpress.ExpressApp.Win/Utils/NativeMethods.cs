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
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Forms;
namespace DevExpress.ExpressApp.Win.Utils {
	public static class NativeMethods {
		private const int SW_RESTORE = 9;
		private static bool extractIconFromExecutable = true;
		private static bool alreadyExtracted = false;
		private static Icon exeIconLarge;
		private static Icon exeIconSmall;
		[DllImport("SHELL32.DLL", EntryPoint = "ExtractIconEx", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
		private static extern Int32 ExtractIconEx(string lpszFile, Int32 nIconIndex, ref IntPtr phiconLarge, ref IntPtr phiconSmall, Int32 nIcons);
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern bool DestroyIcon(IntPtr handle);
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern bool ShowWindow(IntPtr hWnd, Int32 nCmdShow);
		[SecuritySafeCritical]
		private static Icon GetIcon(Image image) {
			using(Bitmap bitmap = new Bitmap(image)) {
				IntPtr handle = bitmap.GetHicon();
				Icon result = GetIcon(handle);
				DestroyIcon(handle);
				return result;
			}
		}
		private static Icon GetIcon(IntPtr iconHandle) { 
			Icon icon = Icon.FromHandle(iconHandle);
			Icon result = (Icon)icon.Clone();
			icon.Dispose();
			return result;
		}
		[SecuritySafeCritical]
		private static void ExtractExeIcons() {
			if(extractIconFromExecutable && !alreadyExtracted) {
				alreadyExtracted = true;
				IntPtr smallIcon = IntPtr.Zero;
				IntPtr largeIcon = IntPtr.Zero;
				int resultIconCount = ExtractIconEx(Application.ExecutablePath, 0, ref largeIcon, ref smallIcon, 1);
				if(resultIconCount > 0) {
					if(largeIcon != IntPtr.Zero) {
						exeIconLarge = GetIcon(largeIcon);
						DestroyIcon(largeIcon);
					}
					if(smallIcon != IntPtr.Zero) {
						exeIconSmall = GetIcon(smallIcon);
						DestroyIcon(smallIcon);
					}
				}
			}
		}
		public static void SetFormIcon(Form form, Image smallImage, Image largeImage) {
			Icon smallIcon = smallImage != null ? GetIcon(smallImage) : null;
			Icon largeIcon = largeImage != null ? GetIcon(largeImage) : null;
			SetFormIcon(form, smallIcon, largeIcon);
		}
		public static void SetFormIcon(Form form, Icon smallIcon, Icon largeIcon) {
			if(largeIcon != null) {
				form.Icon = largeIcon;
			}
			if(smallIcon != null) {
				FieldInfo fi = typeof(Form).GetField("smallIcon", BindingFlags.SetField | BindingFlags.Instance | BindingFlags.NonPublic);
				if(fi != null) {
					fi.SetValue(form, smallIcon);
				}
				MethodInfo mi = typeof(Form).GetMethod("UpdateWindowIcon", BindingFlags.Instance | BindingFlags.NonPublic);
				if(mi != null) {
					mi.Invoke(form, new object[] { true });
				}
			}
		}
		public static void SetExecutingApplicationIcon(Form form) {
			SetFormIcon(form, ExeIconSmall, ExeIconLarge);
		}
		public static bool IsCtrlShiftPressed() {
			Keys keys = Control.ModifierKeys;
			return
				(((keys & Keys.Shift) == Keys.Shift) || ((keys & Keys.ShiftKey) == Keys.ShiftKey))
				&&
				(((keys & Keys.Control) == Keys.Control) || ((keys & Keys.ControlKey) == Keys.ControlKey));
		}
		public static Icon ExeIconLarge {
			get {
				ExtractExeIcons();
				return exeIconLarge != null ? (Icon)exeIconLarge.Clone() : null;
			}
		}
		public static Icon ExeIconSmall {
			get {
				ExtractExeIcons();
				return exeIconSmall != null ? (Icon)exeIconSmall.Clone() : null;
			}
		}
		public static bool ExtractIconFromExecutable {
			get { return extractIconFromExecutable; }
			set { extractIconFromExecutable = value; }
		}
		[SecuritySafeCritical]
		public static bool RestoreForm(Form form) {
			return ShowWindow(form.Handle, SW_RESTORE);
		}
	}
}
