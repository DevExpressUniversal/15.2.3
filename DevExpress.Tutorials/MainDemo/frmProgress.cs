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
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
namespace DevExpress.DXperience.Demos {
	internal partial class frmProgress : XtraForm {
		Form parent;
		const int requiredCount = 200;
		const int requiredDataCount = 20;
		int requiredUpdateCount = 0;
		public frmProgress() : this(null) { }
		public frmProgress(Form parent) {
			this.parent = parent;
			InitializeComponent();
			labelControl1.Text = DevExpress.Tutorials.Properties.Resources.LookAndFeelChanging;
		}
		void Locate() {
			if(parent != null) {
				this.ClientSize = new Size(476, 66);
				this.Location = new Point(parent.Bounds.X + (parent.Width - this.Width) / 2,
					parent.Bounds.Y + (parent.Height - this.Height) / 2);
			}
		}
		public void ShowProgress(int count) {
			Locate();
			progressBarControl1.Position = 0;
			progressBarControl1.Properties.Maximum = count;
			requiredUpdateCount = count / 55;
			if(requiredUpdateCount == 0) requiredUpdateCount = 1;
			if(count > requiredCount) this.Show();
			FormInvalidate();
		}
		public void ShowProgress(int recordCount, string caption) {
			Locate();
			progressBarControl1.Position = 0;
			progressBarControl1.Properties.Maximum = 100;
			requiredUpdateCount = 1;
			labelControl1.Text = caption;
			if(recordCount > requiredDataCount) this.Show();
			FormInvalidate();
		}
		void FormInvalidate() {
			labelControl1.Refresh();
			this.Refresh();
		}
		public void Progress(int index) {
			if(this.Visible && index % requiredUpdateCount == 0) {
				progressBarControl1.Position = index;
				progressBarControl1.Refresh();
			}
		}
		public void HideProgress() {
			progressBarControl1.Position = progressBarControl1.Properties.Maximum;
			progressBarControl1.Refresh();
			this.Hide();
		}
	}
}
