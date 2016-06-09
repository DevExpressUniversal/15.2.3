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
	partial class AppearanceGalleryControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.popupAppearanceControl = new DevExpress.XtraEditors.PopupContainerEdit();
			this.styleEditContainer = new DevExpress.XtraEditors.PopupContainerControl();
			this.stylesContainer = new DevExpress.XtraCharts.Design.StylesContainerControl();
			((System.ComponentModel.ISupportInitialize)(this.popupAppearanceControl.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.styleEditContainer)).BeginInit();
			this.styleEditContainer.SuspendLayout();
			this.SuspendLayout();
			this.popupAppearanceControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.popupAppearanceControl.Location = new System.Drawing.Point(0, 0);
			this.popupAppearanceControl.Margin = new System.Windows.Forms.Padding(0);
			this.popupAppearanceControl.Name = "popupAppearanceControl";
			this.popupAppearanceControl.Properties.AutoHeight = false;
			this.popupAppearanceControl.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.popupAppearanceControl.Properties.PopupControl = this.styleEditContainer;
			this.popupAppearanceControl.Properties.ShowPopupCloseButton = false;
			this.popupAppearanceControl.Size = new System.Drawing.Size(502, 20);
			this.popupAppearanceControl.TabIndex = 4;
			this.popupAppearanceControl.QueryCloseUp += new System.ComponentModel.CancelEventHandler(this.popupContainerEdit1_QueryCloseUp);
			this.popupAppearanceControl.QueryPopUp += new System.ComponentModel.CancelEventHandler(this.popupContainerEdit1_QueryPopUp);
			this.styleEditContainer.Controls.Add(this.stylesContainer);
			this.styleEditContainer.Location = new System.Drawing.Point(3, 3);
			this.styleEditContainer.Name = "styleEditContainer";
			this.styleEditContainer.Size = new System.Drawing.Size(457, 178);
			this.styleEditContainer.TabIndex = 6;
			this.stylesContainer.Appearance.BackColor = System.Drawing.Color.White;
			this.stylesContainer.Appearance.Options.UseBackColor = true;
			this.stylesContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.stylesContainer.Location = new System.Drawing.Point(0, 0);
			this.stylesContainer.Name = "stylesContainer";
			this.stylesContainer.Size = new System.Drawing.Size(457, 178);
			this.stylesContainer.TabIndex = 0;
			this.stylesContainer.OnEditValueChanged += new System.EventHandler(this.stylesContainer_OnEditValueChanged);
			this.stylesContainer.OnNeedClose += new System.EventHandler(this.stylesContainer_OnNeedClose);
			this.AutoSize = true;
			this.Controls.Add(this.styleEditContainer);
			this.Controls.Add(this.popupAppearanceControl);
			this.Margin = new System.Windows.Forms.Padding(0);
			this.Name = "AppearanceGalleryControl";
			this.Size = new System.Drawing.Size(502, 184);
			((System.ComponentModel.ISupportInitialize)(this.popupAppearanceControl.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.styleEditContainer)).EndInit();
			this.styleEditContainer.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private XtraEditors.PopupContainerEdit popupAppearanceControl;
		private XtraEditors.PopupContainerControl styleEditContainer;
		private Design.StylesContainerControl stylesContainer;
	}
}
