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
using System.Drawing;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Native.Lines;
namespace DevExpress.XtraPrinting.Native.Lines {
	public class LinesForm : XtraForm {
		private SimpleButton btnCancel;
		private SimpleButton btnOK;
		private PanelControl pnlButtons;
		LinesContainer linesContainer;
		System.ComponentModel.IContainer components = null;
		public LinesContainer LinesContainer {
			get { return linesContainer; }
		}
		public LinesForm() {
			InitializeComponent();
			EditorContextMenuLookAndFeelHelper.InitBarManager(ref this.components, this);
		}
		public LinesForm(BaseLine[] lines, UserLookAndFeel lookAndFeel, string caption) : this() {
			LookAndFeel.ParentLookAndFeel = lookAndFeel;
			Text = caption;
			linesContainer = new LinesContainer();
			Controls.Add(linesContainer);
			SuspendLayout();
			linesContainer.FillWithLines(lines, lookAndFeel, 100, 5, 5);
			ClientSize = new Size(linesContainer.Width, linesContainer.Height + pnlButtons.Height);
			ResumeLayout(false);
		}
		#region InitializeComponent
		void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LinesForm));
			this.pnlButtons = new DevExpress.XtraEditors.PanelControl();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOK = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.pnlButtons)).BeginInit();
			this.pnlButtons.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.pnlButtons, "pnlButtons");
			this.pnlButtons.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlButtons.CausesValidation = false;
			this.pnlButtons.Controls.Add(this.btnCancel);
			this.pnlButtons.Controls.Add(this.btnOK);
			this.pnlButtons.Name = "pnlButtons";
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.CausesValidation = false;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Name = "btnOK";
			this.AcceptButton = this.btnOK;
			this.CancelButton = this.btnCancel;
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.pnlButtons);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LinesForm";
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.pnlButtons)).EndInit();
			this.pnlButtons.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components != null)
					components.Dispose();
			}
			base.Dispose(disposing);
		}
#if DEBUGTEST
		public SimpleButton Test_BtnCancel { get { return btnCancel; } }
		public SimpleButton Test_BtnOK { get { return btnOK; } }
#endif
	}
}
