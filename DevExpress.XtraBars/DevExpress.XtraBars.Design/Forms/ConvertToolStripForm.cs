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

using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraBars.Design {
	public partial class ConvertToolStripForm : XtraForm {
		public ConvertToolStripForm() {
			InitializeComponent();
		}
		public bool ConvertToolStrip { get; set; }
		public bool ConvertMenuStrip { get; set; }
		public bool ConvertContextMenuStrip { get; set; }
		public bool ConvertStatusStrip { get; set; }
		public bool DeleteItems { get; set; }
		bool needUpdateCheckAll = true;
		bool need = true;
		private void checkedListBoxControl1_ItemCheck(object sender, DevExpress.XtraEditors.Controls.ItemCheckEventArgs e) {
			if(e.Index == 0) {
				OnCheckAllChanged(e.State);
				return;
			}
			if(checkedListBoxControl1.Items[3].CheckState == CheckState.Checked) ConvertToolStrip = true;
			if(checkedListBoxControl1.Items[3].CheckState == CheckState.Unchecked) ConvertToolStrip = false;
			if(checkedListBoxControl1.Items[2].CheckState == CheckState.Checked) ConvertMenuStrip = true;
			if(checkedListBoxControl1.Items[2].CheckState == CheckState.Unchecked) ConvertMenuStrip = false;
			if(checkedListBoxControl1.Items[1].CheckState == CheckState.Checked) ConvertContextMenuStrip = true;
			if(checkedListBoxControl1.Items[1].CheckState == CheckState.Unchecked) ConvertContextMenuStrip = false;
			if(checkedListBoxControl1.Items[4].CheckState == CheckState.Checked) ConvertStatusStrip = true;
			if(checkedListBoxControl1.Items[4].CheckState == CheckState.Unchecked) ConvertStatusStrip = false;
			if(needUpdateCheckAll)
				UpdateCheckAll();
		}
		private void OnCheckAllChanged(CheckState checkState) {
			needUpdateCheckAll = false;
			if(need) {
				if(checkState == CheckState.Checked) {
					foreach(CheckedListBoxItem item in checkedListBoxControl1.Items) {
						item.CheckState = CheckState.Checked;
					}
				}
				if(checkState == CheckState.Unchecked) {
					foreach(CheckedListBoxItem item in checkedListBoxControl1.Items) {
						item.CheckState = CheckState.Unchecked;
					}
				}
			}
			needUpdateCheckAll = true;
		}
		private void UpdateCheckAll() {
			needUpdateCheckAll = false;
			need = false;
			if(ConvertToolStrip && ConvertMenuStrip && ConvertContextMenuStrip && ConvertStatusStrip) {
				checkedListBoxControl1.Items[0].CheckState = CheckState.Checked;
			}
			else if(ConvertToolStrip || ConvertMenuStrip || ConvertContextMenuStrip || ConvertStatusStrip) {
				checkedListBoxControl1.Items[0].CheckState = CheckState.Indeterminate;
			}
			else checkedListBoxControl1.Items[0].CheckState = CheckState.Unchecked;
			needUpdateCheckAll = true;
			need = true;
		}
		private void checkEdit1_CheckedChanged(object sender, EventArgs e) {
			DeleteItems = checkEdit1.Checked;
		}
	}
}
