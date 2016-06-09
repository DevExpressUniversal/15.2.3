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

namespace DevExpress.XtraRichEdit.Design {
	partial class TabStopPositionEdit {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.edtCurrentPosition = new DevExpress.XtraEditors.TextEdit();
			this.lbPositions = new DevExpress.XtraEditors.ListBoxControl();
			((System.ComponentModel.ISupportInitialize)(this.edtCurrentPosition.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lbPositions)).BeginInit();
			this.SuspendLayout();
			this.edtCurrentPosition.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.edtCurrentPosition.CausesValidation = false;
			this.edtCurrentPosition.Location = new System.Drawing.Point(0, 0);
			this.edtCurrentPosition.Name = "edtCurrentPosition";
			this.edtCurrentPosition.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
			this.edtCurrentPosition.Size = new System.Drawing.Size(145, 20);
			this.edtCurrentPosition.TabIndex = 0;
			this.lbPositions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lbPositions.Location = new System.Drawing.Point(9, 20);
			this.lbPositions.Name = "lbPositions";
			this.lbPositions.Size = new System.Drawing.Size(136, 102);
			this.lbPositions.TabIndex = 1;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.lbPositions);
			this.Controls.Add(this.edtCurrentPosition);
			this.Name = "TabStopPositionEdit";
			this.Size = new System.Drawing.Size(145, 128);
			((System.ComponentModel.ISupportInitialize)(this.edtCurrentPosition.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lbPositions)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraEditors.TextEdit edtCurrentPosition;
		private DevExpress.XtraEditors.ListBoxControl lbPositions;
	}
}
