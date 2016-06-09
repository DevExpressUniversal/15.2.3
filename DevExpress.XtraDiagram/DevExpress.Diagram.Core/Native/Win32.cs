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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Forms;
namespace DevExpress.Diagram.Core.Native {
	[SecuritySafeCritical]
	public static class Win32 {
		public static IWin32Window GetOwnerWindow() {
			IntPtr hwnd = Win32.GetActiveWindow();
			return hwnd != IntPtr.Zero ? new Win32Window(hwnd) : null;
		}
		public static IntPtr GetActiveWindow() { return Win32Core.GetActiveWindow(); }
		public static IntPtr CreateCursor(Stream cursorStream) {
			IntPtr handle = IntPtr.Zero;
			string tempFileName = Path.GetTempFileName();
			try {
				using(BinaryReader reader = new BinaryReader(cursorStream)) {
					using(FileStream stream = new FileStream(tempFileName, FileMode.Open, FileAccess.Write, FileShare.None)) {
						byte[] buffer = reader.ReadBytes(0x1000);
						int length = buffer.Length;
						while(length >= 0x1000) {
							stream.Write(buffer, 0, 0x1000);
							length = reader.Read(buffer, 0, 0x1000);
						}
						stream.Write(buffer, 0, length);
					}
				}
				handle = Win32Core.LoadImage(IntPtr.Zero, tempFileName, Win32Core.IMAGE_CURSOR, 0, 0, Win32Core.LR_LOADFROMFILE);
			}
			finally {
				File.Delete(tempFileName);
			}
			return handle;
		}
		class Win32Core {
			[DllImport("User32.dll")]
			internal static extern IntPtr GetActiveWindow();
			internal const int IMAGE_CURSOR = 2;
			internal const int LR_LOADFROMFILE = 0x10;
			[DllImport("user32.dll")]
			internal static extern IntPtr LoadImage(IntPtr hinst, string stName, int nType, int cxDesired, int cyDesired, int nFlags);
		}
	}
	class Win32Window : IWin32Window {
		IntPtr handle = IntPtr.Zero;
		IntPtr IWin32Window.Handle {
			get { return handle; }
		}
		public Win32Window(IntPtr handle) {
			this.handle = handle;
		}
	}
}
