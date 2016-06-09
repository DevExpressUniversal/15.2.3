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
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Forms;
using DevExpress.Office.PInvoke;
using DevExpress.XtraSpreadsheet.Localization;
namespace DevExpress.XtraSpreadsheet.Forms {
	#region ProtectedRangeSecurityInformation
	public class ProtectedRangeSecurityInformation : SimpleSecurityInformation {
		#region Fields
		const string defaultSecurityDescriptor = "O:WDG:WDD:";
		string securityDescriptor = defaultSecurityDescriptor;
		#endregion
		public ProtectedRangeSecurityInformation() {
			Win32.SI_ACCESS editRangeWithoutPassword = new Win32.SI_ACCESS();
			editRangeWithoutPassword.szName = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PermissionEditRangeWithoutPassword);
			editRangeWithoutPassword.dwFlags = (int)Win32.SI_ACCESS_FLAG.SI_ACCESS_GENERAL;
			const int ACTRL_DS_CREATE_CHILD = 0x00000001;
			editRangeWithoutPassword.mask = ACTRL_DS_CREATE_CHILD;
			this.AccessList = new Win32.SI_ACCESS[] { editRangeWithoutPassword };
		}
		#region Properties
		public string SecurityDescriptor {
			get { return securityDescriptor; }
			set {
				if (String.IsNullOrEmpty(value))
					value = defaultSecurityDescriptor;
				if (SecurityDescriptor == value)
					return;
				this.securityDescriptor = value;
			}
		}
		#endregion
		public override void GetSecurity(int requestInformation, IntPtr ppSecurityDescriptor, bool fDefault) {
			Win32.ConvertStringSecurityDescriptorToSecurityDescriptor(SecurityDescriptor, ppSecurityDescriptor);
		}
		public override void SetSecurity(int requestInformation, IntPtr securityDescriptor) {
			this.SecurityDescriptor = ConvertToStringSecurityDescriptor(securityDescriptor);
		}
		[SecuritySafeCritical]
		string ConvertToStringSecurityDescriptor(IntPtr securityDescriptor) {
			string result = string.Empty;
			Win32.SetSecurityDescriptorControl(securityDescriptor, Win32.SECURITY_DESCRIPTOR_CONTROL.SE_DACL_AUTO_INHERIT_REQ, 0);
			IntPtr worldSid;
			Win32.ConvertStringSidToSid("S-1-1-0", out worldSid);
			Win32.SetSecurityDescriptorOwner(securityDescriptor, worldSid, 0);
			Win32.SetSecurityDescriptorGroup(securityDescriptor, worldSid, 0);
			Win32.SECURITY_INFORMATION information;
			information = Win32.SECURITY_INFORMATION.GROUP_SECURITY_INFORMATION | Win32.SECURITY_INFORMATION.OWNER_SECURITY_INFORMATION | Win32.SECURITY_INFORMATION.DACL_SECURITY_INFORMATION | Win32.SECURITY_INFORMATION.SACL_SECURITY_INFORMATION;
			result = Win32.ConvertSecurityDescriptorToStringSecurityDescriptor(securityDescriptor, information);
			Marshal.FreeHGlobal(worldSid);
			return result;
		}
	}
	#endregion
}
