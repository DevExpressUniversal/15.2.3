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
using System.Security;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
namespace DevExpress.Printing.Native.PrintEditor {
	public class PrinterPreferences {
		[DllImport("kernel32.dll")]
		static extern IntPtr GlobalLock(IntPtr hMem);
		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool GlobalUnlock(IntPtr hMem);
		[DllImport("kernel32.dll")]
		static extern IntPtr GlobalFree(IntPtr hMem);
		[DllImport("winspool.Drv", EntryPoint = "DocumentPropertiesW", SetLastError = true, ExactSpelling = true,
			CallingConvention = CallingConvention.StdCall)]
		static extern int DocumentProperties(IntPtr hwnd, IntPtr hPrinter,
		[MarshalAs(UnmanagedType.LPWStr)] string pDeviceName,
		IntPtr pDevModeOutput, IntPtr pDevModeInput, int fMode);
		private const int DM_IN_BUFFER = 8;
		private const int DM_OUT_BUFFER = 2;
		private const int DM_IN_PROMPT = 4;
		private const int IDOK = 1;
		[SecuritySafeCritical]
		public void ShowPrinterProperties(PrinterSettings settings, IntPtr hwnd) {
			if(!settings.IsValid)
				throw new InvalidPrinterException(settings);
			IntPtr hDevMode = settings.GetHdevmode(settings.DefaultPageSettings);
			IntPtr pDevMode = GlobalLock(hDevMode);
			int sizeNeeded = DocumentProperties(hwnd, IntPtr.Zero, settings.PrinterName, IntPtr.Zero, pDevMode, 0);
			IntPtr devModeData = Marshal.AllocHGlobal(sizeNeeded);
			int result = DocumentProperties(hwnd, IntPtr.Zero, settings.PrinterName, devModeData, pDevMode, DM_IN_BUFFER | DM_OUT_BUFFER | DM_IN_PROMPT);
			GlobalUnlock(hDevMode);
			if(result == IDOK) {
				settings.SetHdevmode(devModeData);
			}
			GlobalFree(hDevMode);
			Marshal.FreeHGlobal(devModeData);
		}
	}
}
