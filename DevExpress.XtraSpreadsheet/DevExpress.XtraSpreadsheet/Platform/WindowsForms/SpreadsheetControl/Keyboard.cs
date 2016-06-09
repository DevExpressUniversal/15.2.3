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
using System.Windows.Forms;
using System.Security.Permissions;
using System.ComponentModel;
using DevExpress.Office.PInvoke;
namespace DevExpress.XtraSpreadsheet {
	public partial class SpreadsheetControl {
		bool acceptsTab = true;
		bool acceptsReturn = true;
		bool acceptsEscape = true;
		#region Fields
		#region AcceptsTab
		[
#if !SL
	DevExpressXtraSpreadsheetLocalizedDescription("SpreadsheetControlAcceptsTab"),
#endif
		DefaultValue(true)]
		public bool AcceptsTab {
			get { return acceptsTab; }
			set {
				if (acceptsTab == value)
					return;
				acceptsTab = value;
			}
		}
		#endregion
		#region AcceptsReturn
		[
#if !SL
	DevExpressXtraSpreadsheetLocalizedDescription("SpreadsheetControlAcceptsReturn"),
#endif
		DefaultValue(true)]
		public bool AcceptsReturn {
			get { return acceptsReturn; }
			set {
				if (acceptsReturn == value)
					return;
				acceptsReturn = value;
			}
		}
		#endregion
		#region AcceptsEscape
		[
#if !SL
	DevExpressXtraSpreadsheetLocalizedDescription("SpreadsheetControlAcceptsEscape"),
#endif
		DefaultValue(true)]
		public bool AcceptsEscape {
			get { return acceptsEscape; }
			set {
				if (acceptsEscape == value)
					return;
				acceptsEscape = value;
			}
		}
		#endregion
		#endregion
		#region Keyboard handling
		bool suppressKeyPressHandling;
		[System.Security.SecuritySafeCritical]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
		protected override bool ProcessDialogChar(char charCode) {
			if (Control.ModifierKeys != Keys.Alt)
				return false;
			else
				return base.ProcessDialogChar(charCode);
		}
		protected override bool IsInputKey(Keys keyData) {
			if ((keyData == Keys.Tab || (keyData == (Keys.Tab | Keys.Shift))) && !AcceptsTab)
				return false;
			if (keyData == Keys.Return && !AcceptsReturn)
				return false;
			if (keyData == Keys.Escape && !AcceptsEscape)
				return false;
			if (InnerControl == null)
				return false;
			return InnerControl.IsEnabled;
		}
		protected internal bool IsAltGrPressed() {
			const int VK_LCONTROL = 0xA2;
			const int VK_RMENU = 0xA5;
			bool leftCtrlPressed = Win32.GetAsyncKeyState((Keys)VK_LCONTROL) != 0;
			bool rightMenuPressed = Win32.GetAsyncKeyState((Keys)VK_RMENU) != 0;
			return leftCtrlPressed && rightMenuPressed;
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			if (IsDisposed)
				return;
			base.OnKeyDown(e);
			if (e.Handled)
				return;
			if (e.KeyCode == Keys.Tab && !AcceptsTab)
				return;
			if (e.KeyCode == Keys.Return && !AcceptsReturn)
				return;
			if (e.KeyCode == Keys.Escape && !AcceptsEscape)
				return;
			if (InnerControl != null && !IsAltGrPressed()) {
				InnerControl.OnKeyDown(e);
				suppressKeyPressHandling = e.Handled;
			}
			if (e.KeyCode == Keys.ProcessKey) {
				InnerControl.OpenInplaceEditor(String.Empty);
				e.Handled = true;
			}
		}
		protected override void OnKeyUp(KeyEventArgs e) {
			if (IsDisposed)
				return;
			suppressKeyPressHandling = false;
			if (InnerControl != null && !IsAltGrPressed())
				InnerControl.OnKeyUp(e);
			base.OnKeyUp(e);
		}
		protected override void OnKeyPress(KeyPressEventArgs e) {
			if (IsDisposed)
				return;
			if (!suppressKeyPressHandling) {
				base.OnKeyPress(e);
				if (e.Handled)
					return;
				if (InnerControl != null)
					InnerControl.OnKeyPress(e);
				e.Handled = true; 
			}
			else
				base.OnKeyPress(e);
		}
		#endregion
	}
}
