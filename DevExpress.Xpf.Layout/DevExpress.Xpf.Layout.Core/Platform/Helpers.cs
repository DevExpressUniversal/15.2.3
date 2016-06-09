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

using System.Runtime.InteropServices;
using System.Windows.Interop;
namespace DevExpress.Xpf.Layout.Core.Platform {
	static class SystemInformation {
		public static int DoubleClickTime {
			get { return BrowserInteropHelper.IsBrowserHosted ? 500 : WinAPI.DoubleClickTime; }
		}
		public static int DoubleClickWidth {
			get { return BrowserInteropHelper.IsBrowserHosted ? 4 : WinAPI.DoubleClickWidth; }
		}
		public static int DoubleClickHeight {
			get { return BrowserInteropHelper.IsBrowserHosted ? 4 : WinAPI.DoubleClickHeight; }
		}
	}
	static class WinAPI {
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		static extern int GetDoubleClickTime();
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		static extern int GetSystemMetrics(int nIndex);
		[System.Security.SecuritySafeCritical]
		static int WA_GetDoubleClickTime() {
			return GetDoubleClickTime();
		}
		[System.Security.SecuritySafeCritical]
		static int WA_GetSystemMetrics(int nIndex) {
			return GetSystemMetrics(0x24);
		}
		public static int DoubleClickTime {
			get { return WA_GetDoubleClickTime(); }
		}
		public static int DoubleClickWidth {
			get { return WA_GetSystemMetrics(0x24); }
		}
		public static int DoubleClickHeight {
			get { return WA_GetSystemMetrics(0x25); }
		}
	}
}
