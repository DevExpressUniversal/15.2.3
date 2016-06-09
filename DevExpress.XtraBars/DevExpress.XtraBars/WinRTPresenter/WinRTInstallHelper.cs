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
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using Microsoft.Win32;
namespace DevExpress.XtraBars.WinRTLiveTiles {
	partial class WinRTAppInstallHelper : XtraForm {
		public WinRTAppInstallHelper(bool isDesignMode) {
			InitializeComponent();
			StartPosition = FormStartPosition.CenterScreen;
			TopMost = true;
			SetLinkColor();
			if(isDesignMode) {
				InitClosingTimer();
				this.Deactivate += WinRTInstallHelper_Deactivate;
			}
		}
		void SetLinkColor() {
			if(LookAndFeel.ActiveSkinName == "Metropolis Dark")
				labelControl5.Appearance.ForeColor = Color.DarkOrange;
		}
		void WinRTInstallHelper_Deactivate(object sender, EventArgs e) {
			this.Activated += WinRTInstallHelper_Activated;
			this.Deactivate -= WinRTInstallHelper_Deactivate;
		}
		void WinRTInstallHelper_Activated(object sender, EventArgs e) {
			timer.Stop();
			UpdateCloseButton(true);
			this.Activated += WinRTInstallHelper_Activated;
		}
		Timer timer;
		void InitClosingTimer() {
			timer = new Timer();
			timer.Interval = 1000;
			timer.Tick += timer_Tick;
			timer.Start();
		}
		int closeCounter = 10;
		void timer_Tick(object sender, EventArgs e) {
			if(closeCounter == 0) Close();
			else {
				closeCounter--;
				UpdateCloseButton(false);
			}
		}
		void UpdateCloseButton(bool hideCounter) {
			if(hideCounter) simpleButton1.Text = "Close";
				else simpleButton1.Text = String.Format("Close ({0})", closeCounter);
		}
		private void simpleButton1_Click(object sender, EventArgs e) {
			Close();
		}
		private void labelControl5_Click(object sender, EventArgs e) {
			try {
				Process.Start(@"http://apps.microsoft.com/windows/en-us/app/devexpress-live-tile-manager/acdad900-0021-4cbd-90cd-4ae5a03e91f5");
			}
			finally {
				Close();
			}
		}
	}
	public static class WinRTAppInstallHelperWindow {
		static WinRTAppInstallHelper window = null;
		public static void Show(bool isDesignMode) {
			if(window != null) return;
			window = new WinRTAppInstallHelper(isDesignMode);
			window.Show();
			window.FormClosed += window_FormClosed;
		}
		static void window_FormClosed(object sender, FormClosedEventArgs e) {
			window.FormClosed -= window_FormClosed;
			window = null;
		}
	}
}
