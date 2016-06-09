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

using DevExpress.Utils.Design;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraBars {
	internal partial class Win8OnlyWindow : XtraForm {
		protected internal Win8OnlyWindow() { }
		public Win8OnlyWindow(Type type, int milliseconds) {
			DevExpress.Skins.SkinManager.EnableFormSkins();
			InitializeComponent();
			this.typeName = type == null ? "DefaultType" : type.Name;
			labelControl1.Text = string.Format(strWin8Only, typeName);
			timer = new Timer() ;
			timer.Interval = milliseconds;
			timer.Tick += simpleButton1_Click;
			timer.Start();
		}
		const string strWin8Only = "The {0} component is only supported on Windows 8.";
		string typeName;
		Timer timer;
		public void ShowMessage() {
			if(DontShow){
				TimerUsubscribe();
				Close();
			} 
			else this.Show();
		}
		void TimerUsubscribe() {
			timer.Tick -= simpleButton1_Click;
			timer.Stop();
			timer.Dispose();
		}
		private void simpleButton1_Click(object sender, EventArgs e) {
			DontShow = ceDontShow.Checked;
			TimerUsubscribe();
			this.Close();
		}
		public const string RegistryPath = "Software\\Developer Express\\{0}\\";
		public const string RegistryEntry = "DontShowWin8OnlyWindow";
		string GetRegistryPath {
			get { return String.Format(RegistryPath, typeName); }
		}
		public bool DontShow {
			get {
				PropertyStore store = new PropertyStore(GetRegistryPath);
				if(store == null)
					return false;
				store.Restore();
				return store.RestoreBoolProperty(RegistryEntry, false);
			}
			set {
				PropertyStore store = new PropertyStore(GetRegistryPath);
				if(store == null)
					return;
				store.AddProperty(RegistryEntry, value);
				store.Store();
			}
		}
	}
	internal static class Win8ComponentHelper {
		public static bool IsOSSupported { get { return CheckWindowsVersion(); } }
		static bool CheckWindowsVersion() {
			OperatingSystem os = Environment.OSVersion;
			return os.Version.Major == 6 && os.Version.Minor > 1;
		}
		public static void ShowOSNotSupportedMessage(Type tagretType) { ShowOSNotSupportedMessage(tagretType, 15000); }
		public static void ShowOSNotSupportedMessage(Type tagretType, int timeout) {
			if(tagretType == null) return;
			Win8OnlyWindow window = new Win8OnlyWindow(tagretType, timeout);
			window.ShowMessage();
		}
	}
}
