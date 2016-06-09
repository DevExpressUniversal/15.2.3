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

namespace DevExpress.XtraCharts.Designer.Native {
	partial class PaletteGalleryControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.paletteEdit = new DevExpress.XtraCharts.Design.PalettePopupContainerEdit();
			this.paletteEditContainer = new DevExpress.XtraEditors.PopupContainerControl();
			this.paletteEditControl = new DevExpress.XtraCharts.Designer.Native.DesignerPaletteEditControl();
			((System.ComponentModel.ISupportInitialize)(this.paletteEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.paletteEditContainer)).BeginInit();
			this.paletteEditContainer.SuspendLayout();
			this.SuspendLayout();
			this.paletteEdit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.paletteEdit.Location = new System.Drawing.Point(0, 0);
			this.paletteEdit.Margin = new System.Windows.Forms.Padding(0);
			this.paletteEdit.Name = "paletteEdit";
			this.paletteEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.paletteEdit.Properties.CloseOnLostFocus = false;
			this.paletteEdit.Properties.CloseOnOuterMouseClick = false;
			this.paletteEdit.Properties.PopupControl = this.paletteEditContainer;
			this.paletteEdit.Properties.UsePopupControlMinSize = true;
			this.paletteEdit.Size = new System.Drawing.Size(400, 20);
			this.paletteEdit.TabIndex = 2;
			this.paletteEdit.QueryPopUp += new System.ComponentModel.CancelEventHandler(this.paletteEdit_QueryPopUp);
			this.paletteEdit.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.paletteEdit_Closed);
			this.paletteEdit.EditValueChanged += new System.EventHandler(this.paletteEdit_EditValueChanged);
			this.paletteEditContainer.Controls.Add(this.paletteEditControl);
			this.paletteEditContainer.Location = new System.Drawing.Point(50, 0);
			this.paletteEditContainer.MinimumSize = new System.Drawing.Size(200, 50);
			this.paletteEditContainer.Name = "paletteEditContainer";
			this.paletteEditContainer.Size = new System.Drawing.Size(200, 263);
			this.paletteEditContainer.TabIndex = 6;
			this.paletteEditControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.paletteEditControl.Location = new System.Drawing.Point(0, 0);
			this.paletteEditControl.Name = "paletteEditControl";
			this.paletteEditControl.Size = new System.Drawing.Size(200, 263);
			this.paletteEditControl.TabIndex = 5;
			this.paletteEditControl.OnPaletteChanged += new System.EventHandler(this.paletteEditControl_OnPaletteChanged);
			this.paletteEditControl.OnNeedClose += new System.EventHandler(this.paletteEditControl_OnNeedClose);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this.paletteEditContainer);
			this.Controls.Add(this.paletteEdit);
			this.Margin = new System.Windows.Forms.Padding(0);
			this.Name = "PaletteGalleryControl";
			this.Size = new System.Drawing.Size(400, 266);
			((System.ComponentModel.ISupportInitialize)(this.paletteEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.paletteEditContainer)).EndInit();
			this.paletteEditContainer.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private Design.PalettePopupContainerEdit paletteEdit;
		private DesignerPaletteEditControl paletteEditControl;
		private XtraEditors.PopupContainerControl paletteEditContainer;
	}
}
