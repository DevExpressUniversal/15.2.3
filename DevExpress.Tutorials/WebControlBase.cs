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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
namespace DevExpress.DXperience.Demos {
	public class WebHelper {
		public static string demoPath = System.IO.Directory.GetCurrentDirectory();
		public const int ICC_USEREX_CLASSES = 0x00000200;
		[StructLayout(LayoutKind.Sequential, Pack=1)]
			public class INITCOMMONCONTROLSEX {
			public int  dwSize = 8; 
			public int  dwICC;
		}
		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int Msg, uint wParam, uint lParam);
		[DllImport("comctl32.dll")]
		public static extern bool InitCommonControlsEx(INITCOMMONCONTROLSEX icc);
		[DllImport("kernel32.dll")]
		public static extern IntPtr LoadLibrary(string libname);
		[DllImport("kernel32.dll")]
		public static extern bool FreeLibrary(IntPtr hModule);
		[DllImport("uxtheme.dll")]
		public static extern void SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);
		public static void SetConnectionString(System.Data.OleDb.OleDbConnection oleDbConnection, string path) {
			oleDbConnection.ConnectionString = String.Format(@"Provider=Microsoft.Jet.OLEDB.4.0;User ID=Admin;Data Source={0};Mode=Share Deny None;Extended Properties="""";Jet OLEDB:System database="""";Jet OLEDB:Registry Path="""";Jet OLEDB:Database Password="""";Jet OLEDB:Engine Type=5;Jet OLEDB:Database Locking Mode=1;Jet OLEDB:Global Partial Bulk Ops=2;Jet OLEDB:Global Bulk Transactions=1;Jet OLEDB:New Database Password="""";Jet OLEDB:Create System Database=False;Jet OLEDB:Encrypt Database=False;Jet OLEDB:Don't Copy Locale on Compact=False;Jet OLEDB:Compact Without Replica Repair=False;Jet OLEDB:SFP=False", path);
		}
	}
	[ToolboxItem(false)]
	public class WebControlBase : TutorialControlBase {
		[ToolboxItem(false)]
			private class WebBrowser : AxHost {
			object ocx;
			public WebBrowser() : base("8856f961-340a-11d0-a96b-00c04fd705a2") {
			}
			protected override void AttachInterfaces() {
				try {
					ocx = this.GetOcx();
				} catch {}
			}
			public void Navigate(string url) {
				if(ocx != null) {
					object nullObject = null;
					ocx.GetType().InvokeMember("Navigate2", System.Reflection.BindingFlags.InvokeMethod, 
						null, ocx, new object[] {url, nullObject, nullObject, nullObject, nullObject});
				}
			}
		}
		private System.ComponentModel.IContainer components = null;
		private WebBrowser webBrowser;
		private IntPtr m_hMod;
		private IntPtr m_hMod2;
		public WebControlBase() {
			SetWebBrowserXPTheme();
			webBrowser = new WebBrowser();
			webBrowser.Dock = DockStyle.Fill;
			Controls.Add(webBrowser);
			webBrowser.ContainingControl = this;
		}
		private void SetWebBrowserXPTheme() {
			if ((m_hMod == IntPtr.Zero) || (m_hMod2 == IntPtr.Zero)) {
				WebHelper.INITCOMMONCONTROLSEX iccex = new WebHelper.INITCOMMONCONTROLSEX();
				iccex.dwICC = WebHelper.ICC_USEREX_CLASSES;
				WebHelper.InitCommonControlsEx(iccex);
				m_hMod = WebHelper.LoadLibrary("shell32.dll");
				m_hMod2 = WebHelper.LoadLibrary("explorer.exe");
			}
		}
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				WebHelper.FreeLibrary(m_hMod);
				WebHelper.FreeLibrary(m_hMod2);
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		protected void Navigate(string url) {
			webBrowser.Navigate(url);
		}
	}
}
