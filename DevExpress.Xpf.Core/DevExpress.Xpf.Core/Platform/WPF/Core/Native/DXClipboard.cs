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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System;
using System.Runtime.InteropServices;
using System.Threading;
namespace DevExpress.Xpf.Core.Native {
	public partial class DXClipboard {
		static IClipboardHelper CreateClipboardHelper() {
			if(BrowserInteropHelper.IsBrowserHosted && DevExpress.Data.Helpers.SecurityHelper.IsPartialTrust)
				return new XBAPClipboardHelper();
			else
				return new ClipboardHelper();
		}
	}
	public class ClipboardHelper : IClipboardHelper {
		public bool ContainsText() {
			return Clipboard.ContainsText();
		}
		public string GetText() {
			return Clipboard.GetText();
		}
		public void SetDataFromClipboardDataProvider(IClipboardDataProvider сlipboardDataProvider) {
			DataObject data = new DataObject();
			data.SetData(typeof(string), сlipboardDataProvider.GetTextFromClipboard());
			object selData = сlipboardDataProvider.GetObjectFromClipboard();
			if(selData != null) data.SetData(selData);
			WorkaroundClipboardBugs(() => Clipboard.SetDataObject(data));
		}
		public void SetText(string text) {
			WorkaroundClipboardBugs(() => Clipboard.SetText(text));
		}
		static void WorkaroundClipboardBugs(Action copyAction) {
			int attempts = 0;
			while(attempts < 5) {
				try {
					copyAction();
					return;
				} catch(COMException) {
					Thread.Sleep(10);
					attempts++;
				}
			} 
		}
	}
	public class XBAPClipboardHelper : IClipboardHelper {
		TextBox textBox;
		TextBox TextBox {
			get {
				if(textBox == null)
					textBox = new TextBox();
				return textBox;
			}
		}
		public bool ContainsText() {
			return !string.IsNullOrEmpty(GetText());
		}
		public string GetText() {
			TextBox.Text = string.Empty;
			ApplicationCommands.Paste.Execute(null, TextBox);
			return TextBox.Text;
		}
		public void SetDataFromClipboardDataProvider(IClipboardDataProvider сlipboardDataProvider) {
		}
		public void SetText(string text) {
		}
	}
}
