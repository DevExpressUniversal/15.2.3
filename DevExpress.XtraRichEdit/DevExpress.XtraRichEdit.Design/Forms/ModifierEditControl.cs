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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Utils;
namespace DevExpress.XtraRichEdit.Design.Forms {
	[DXToolboxItem(false)]
	public partial class ModifierEditControl : UserControl {
		readonly IWindowsFormsEditorService edSvc;
		Keys editValue;
		public ModifierEditControl(IWindowsFormsEditorService edSvc, object editValue) {
			Guard.ArgumentNotNull(edSvc, "edSvc");
			this.edSvc = edSvc;
			this.editValue = (Keys)editValue;
			InitializeComponent();
			SubscribeToControlsEvents();
			UpdateControl();
		}
		public object EditValue { get { return Modifiers; } }
		protected internal Keys Modifiers { get { return editValue; } }
		protected void SetKey(Keys key, bool value) {
			if (value)
				editValue |= key;
			else
				editValue &= ~key;
		}
		void btnOK_Click(object sender, EventArgs e) {
			edSvc.CloseDropDown();
		}
		void btnReset_Click(object sender, EventArgs e) {
			editValue = Keys.None;
			UpdateControl();
		}
		void chkCtrl_CheckedChanged(object sender, EventArgs e) {
			SetKey(Keys.Control, chkCtrl.Checked);
		}
		void chkShift_CheckedChanged(object sender, EventArgs e) {
			SetKey(Keys.Shift, chkShift.Checked);
		}
		void chkAlt_CheckedChanged(object sender, EventArgs e) {
			SetKey(Keys.Alt, chkAlt.Checked);
		}
		protected virtual void SubscribeToControlsEvents() {
			this.chkCtrl.CheckedChanged += new EventHandler(chkCtrl_CheckedChanged);
			this.chkShift.CheckedChanged += new EventHandler(chkShift_CheckedChanged);
			this.chkAlt.CheckedChanged += new EventHandler(chkAlt_CheckedChanged);
		}
		protected virtual void UnsubscribeToControlsEvents() {
			this.chkCtrl.CheckedChanged -= new EventHandler(chkCtrl_CheckedChanged);
			this.chkShift.CheckedChanged -= new EventHandler(chkShift_CheckedChanged);
			this.chkAlt.CheckedChanged -= new EventHandler(chkAlt_CheckedChanged);
		}
		void UpdateControl() {
			UnsubscribeToControlsEvents();
			try {
				this.chkCtrl.Checked = (Modifiers & Keys.Control) == Keys.Control;
				this.chkShift.Checked = (Modifiers & Keys.Shift) == Keys.Shift;
				this.chkAlt.Checked = (Modifiers & Keys.Alt) == Keys.Alt;
			}
			finally {
				SubscribeToControlsEvents();
			}
		}
	}
}
