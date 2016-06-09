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
using System.IO;
using System.Text;
using DevExpress.Office.Utils;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Model;
using System.Runtime.InteropServices;
using DevExpress.Office.PInvoke;
#if !DXPORTABLE
using PlatformIndependentDataObject = System.Windows.Forms.DataObject;
using System.Windows.Forms;
#else
using PlatformIndependentDataObject = DevExpress.Compatibility.System.Windows.Forms.DataObject;
using DevExpress.Compatibility.System.Windows.Forms;
#endif
namespace DevExpress.XtraSpreadsheet {
	public class SpreadsheetClipboardWithDelayedRendering : IClipboard {
		const int WM_RENDERFORMAT = 773;
		const int WM_RENDERALLFORMATS = 774;
		const int WM_DESTROYCLIPBOARD = 0x0307;
		string FormatUnicodeText = OfficeDataFormats.UnicodeText;
		const int CF_UnicodeText = 13;
		System.IntPtr clipboardOwnerHandler;
		TranslationTable<int> formatCodeTable = new TranslationTable<int>();
		ICopiedRangeDataForClipboard dataProvider;
		CopiedRangeProvider copiedRange;
		EventHandler onChangedOutside;
		bool skipNextWM_DESTROYCLIPBOARD = false;
		bool isOwner = false;
		public SpreadsheetClipboardWithDelayedRendering(System.IntPtr clipboardOwnerHandler, ICopiedRangeDataForClipboard dataProvider) {
			this.clipboardOwnerHandler = clipboardOwnerHandler;
		}
		public event EventHandler ChangedOutside { add { onChangedOutside += value; } remove { onChangedOutside -= value; } }
		protected internal virtual void RaiseChangedOutside() {
			if (onChangedOutside != null)
				onChangedOutside(this, EventArgs.Empty);
		}
		public void Clear() {
			Win32.OpenClipboard(this.clipboardOwnerHandler);
			Win32.EmptyClipboard();
			Win32.CloseClipboard();
		}
		public void LeaveCopiedDataInClipboard() {
			if (!isOwner)
				return;
			try {
				skipNextWM_DESTROYCLIPBOARD = true;
				OnRenderAllFormats(true);
			}
			finally {
				skipNextWM_DESTROYCLIPBOARD = false;
			}
		}
		public void SetData(ICopiedRangeDataForClipboard dataProvider, CopiedRangeProvider copiedRange) {
			this.dataProvider = dataProvider;
			this.copiedRange = copiedRange;
			formatCodeTable = CreateFormatCodeTable(dataProvider);
			Win32.OpenClipboard(this.clipboardOwnerHandler);
			skipNextWM_DESTROYCLIPBOARD = true;
			Win32.EmptyClipboard();
			try {
				foreach (string dataFormat in dataProvider.DataFormatsForCopy) {
					int formatCode = formatCodeTable.GetEnumValue(dataFormat, -1, true);
					Win32.SetClipboardData(formatCode, IntPtr.Zero);
				}
			}
			finally {
				Win32.CloseClipboard();
				skipNextWM_DESTROYCLIPBOARD = false;
				isOwner = true;
			}
		}
		void OnRenderFormat(int clipboardFormat) {
			if (!isOwner)
				return;
			string format = formatCodeTable.GetStringValue(clipboardFormat, -1);
			object data = dataProvider.GetData(format, this.copiedRange);
			byte[] bytes = null;
			MemoryStream dataAsStream = data as MemoryStream;
			ChunkedMemoryStream dataAsChunkedStream = data as ChunkedMemoryStream;
			string asString = data as String;
			if (dataAsStream != null)
				bytes = dataAsStream.ToArray();
			else if (dataAsChunkedStream != null)
				bytes = dataAsChunkedStream.ToArray();
			else if (asString != null)
				bytes = Encoding.Unicode.GetBytes(asString);
			if (bytes != null)
				RenderFormat(clipboardFormat, bytes);
		}
		TranslationTable<int> CreateFormatCodeTable(ICopiedRangeDataForClipboard dataProvider) {
			TranslationTable<int> result = new TranslationTable<int>();
			foreach (string dataFormat in dataProvider.DataFormatsForCopy) {
				if (String.Equals(dataFormat, FormatUnicodeText, StringComparison.OrdinalIgnoreCase))
					result.Add(CF_UnicodeText, FormatUnicodeText);
				else
					RegisterFormatCode(dataFormat, result);
			}
			return result;
		}
		[System.Security.SecuritySafeCritical]
		public bool IsClipboardMessange(ref Message msg) {
			return msg.Msg == WM_RENDERFORMAT 
				|| msg.Msg == WM_RENDERALLFORMATS 
				|| msg.Msg == WM_DESTROYCLIPBOARD;
		}
		public bool HandleWndProc(int msg, int wParam) {
			if (msg == WM_RENDERFORMAT) {
				OnRenderFormat(wParam);
			}
			else if (msg == WM_RENDERALLFORMATS) {
				OnRenderAllFormats();
			}
			else if (msg == WM_DESTROYCLIPBOARD) {
				OnClipboardChangedOutside();
			}
			else
				return false;
			return true;
		}
		private void OnClipboardChangedOutside() {
			if (skipNextWM_DESTROYCLIPBOARD) {
				skipNextWM_DESTROYCLIPBOARD = false;
				return;
			}
			isOwner = false;
			RaiseChangedOutside();
		}
		void OnRenderAllFormats(bool emptyClipboard) {
			if (emptyClipboard) {
				Win32.OpenClipboard(this.clipboardOwnerHandler);
				Win32.EmptyClipboard();
				Win32.CloseClipboard();
			}
			Win32.OpenClipboard(this.clipboardOwnerHandler);
			try {
				foreach (string dataFormat in dataProvider.DataFormatsAfterCloseDocument) {
					int code = this.formatCodeTable.GetEnumValue(dataFormat, -1, true);
					OnRenderFormat(code);
				}
			}
			finally {
				Win32.CloseClipboard();
				if(emptyClipboard)
					isOwner = false;
			}
		}
		void OnRenderAllFormats() {
			OnRenderAllFormats(false);
		}
		void RegisterFormatCode(string formatString, TranslationTable<int> result) {
			int code = Office.PInvoke.Win32.RegisterClipboardFormat(formatString);
			result.Add(code, formatString);
		}
		[System.Security.SecuritySafeCritical]
		void RenderFormat(int formatCode, byte[] bytes) {
			int numberOfBytes = bytes.Length; 
			IntPtr hClipboardMemoryData = IntPtr.Zero;
			IntPtr pointer = IntPtr.Zero;
			try {
				hClipboardMemoryData = Marshal.AllocHGlobal(numberOfBytes);
				pointer = Win32.GlobalLock(hClipboardMemoryData);
				Marshal.Copy(bytes, 0, pointer, numberOfBytes);
				Win32.GlobalUnlock(hClipboardMemoryData);
				Win32.SetClipboardData(formatCode, hClipboardMemoryData);
			}
			catch {
				if (pointer != IntPtr.Zero)
					Win32.GlobalUnlock(hClipboardMemoryData); 
				if (hClipboardMemoryData != IntPtr.Zero)
					Marshal.FreeHGlobal(hClipboardMemoryData); 
			}
		}
	}
}
