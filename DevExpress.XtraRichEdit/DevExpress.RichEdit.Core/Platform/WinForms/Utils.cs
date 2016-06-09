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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Text;
using DevExpress.Office.Layout;
using DevExpress.Office.PInvoke;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.Data.Helpers;
namespace DevExpress.XtraRichEdit.Utils {
	#region SafeNativeMethods
	[System.Security.SuppressUnmanagedCodeSecurity]
	internal static class SafeNativeMethods {
		[System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2145:TransparentMethodsShouldNotUseSuppressUnmanagedCodeSecurity")]
		public static extern Int32 WaitForSingleObject(IntPtr Handle, Int32 Wait);
	}
	#endregion
	#region SyncUtils
	public static class SyncUtils {
		[SecuritySafeCritical]
		public static bool BlockedWaitOne(WaitHandle wait) {
			return SafeNativeMethods.WaitForSingleObject(wait.SafeWaitHandle.DangerousGetHandle(), -1) == 0;
		}
#if DEBUGTEST && !SL
		[SecuritySafeCritical]
		public static bool BlockedWaitOne(WaitHandle wait, int timeout) {
			return SafeNativeMethods.WaitForSingleObject(wait.SafeWaitHandle.DangerousGetHandle(), timeout) == 0;
		}
#endif
	}
	#endregion
}
