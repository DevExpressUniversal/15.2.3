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

using DevExpress.Data.Helpers;
using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
namespace DevExpress.Office.PInvoke {
	static partial class PInvokeSafeNativeMethods {
		[DllImport("secur32.dll", CharSet = CharSet.Auto)]
		[SuppressUnmanagedCodeSecurity]
		internal static extern int GetUserNameEx(Win32.ExtendedNameFormat nameFormat, StringBuilder userName, ref int userNameSize);
	}
	public static partial class Win32 {
		#region ExtendedNameFormat
		public enum ExtendedNameFormat {
			Unknown = 0,
			FullyQualifiedDN = 1,
			SamCompatible = 2,
			Display = 3,
			UniqueId = 6,
			Canonical = 7,
			UserPrincipal = 8,
			CanonicalEx = 9,
			ServicePrincipal = 10,
			DnsDomain = 12
		}
		#endregion
		static bool getUserNameFailed;
		[SecuritySafeCritical]
		public static string GetUserName(ExtendedNameFormat nameFormat) {
			if (getUserNameFailed)
				return String.Empty;
			try {
				if (!SecurityHelper.IsPermissionGranted(new SecurityPermission(SecurityPermissionFlag.UnmanagedCode)))
					return String.Empty;
				StringBuilder userName = new StringBuilder(1000);
				int bufferSize = userName.Capacity;
				int result = PInvokeSafeNativeMethods.GetUserNameEx(nameFormat, userName, ref bufferSize);
				return result != 0 ? userName.ToString() : String.Empty;
			}
			catch {
				getUserNameFailed = true;
				return String.Empty;
			}
		}
	}
}
