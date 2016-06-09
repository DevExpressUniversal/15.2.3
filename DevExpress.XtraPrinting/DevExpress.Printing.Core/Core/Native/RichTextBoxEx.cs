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

using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
namespace DevExpress.XtraPrinting.Native {
	[
	ToolboxItem(false),
	]
	public class RichTextBoxEx : System.Windows.Forms.RichTextBox {
		const int EM_GETPARAFORMAT = 1085;
		const int EM_SETPARAFORMAT = 1095;
		const int EM_SETTYPOGRAPHYOPTIONS = 1226;
		const int TO_ADVANCEDTYPOGRAPHY = 1;
		const int PFM_ALIGNMENT = 8;
		const int SCF_SELECTION = 1;
		const int EM_SETCHARFORMAT = 0x444;
		const int EM_GETCHARFORMAT = 0x43a;
		const int CFM_COLOR = 0x4000000;
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			SetAdditionalOptions();
		}
		[System.Security.SecuritySafeCritical]
		void SetAdditionalOptions() {
			Win32.SendMessage(Handle, EM_SETTYPOGRAPHYOPTIONS, TO_ADVANCEDTYPOGRAPHY, new IntPtr(TO_ADVANCEDTYPOGRAPHY));
		}
		public HorizontalAlignmentEx SelectionAlignmentEx {
			[System.Security.SecuritySafeCritical]
			get {
				Win32.PARAFORMAT2 fmt = new Win32.PARAFORMAT2();
				fmt.cbSize = Marshal.SizeOf(fmt);
				IntPtr lParam = Marshal.AllocCoTaskMem(Marshal.SizeOf(fmt));
				Win32.SendMessage(Handle, EM_GETPARAFORMAT, SCF_SELECTION, lParam);
				fmt = (Win32.PARAFORMAT2)Marshal.PtrToStructure(lParam, typeof(Win32.PARAFORMAT2));
				return (fmt.dwMask & PFM_ALIGNMENT) == 0 ? HorizontalAlignmentEx.Left : (HorizontalAlignmentEx)fmt.wAlignment;
			}
			[System.Security.SecuritySafeCritical]
			set {
				Win32.PARAFORMAT2 fmt = new Win32.PARAFORMAT2();
				fmt.cbSize = Marshal.SizeOf(fmt);
				fmt.dwMask = PFM_ALIGNMENT;
				fmt.wAlignment = (short)value;
				IntPtr lParam = Marshal.AllocCoTaskMem(Marshal.SizeOf(fmt));
				Marshal.StructureToPtr(fmt, lParam, false);
				Win32.SendMessage(Handle, EM_SETPARAFORMAT, SCF_SELECTION, lParam);
			}
		}
	}
}
