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
namespace DevExpress.XtraBars.Navigation {
	partial class AccordionSearchControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components != null)
					components.Dispose();
				if(AccordionControl != null) {
					AccordionControl.RightToLeftChanged -= OnRightToLeftChanged;
				}
				if(LookAndFeel != null) {
					LookAndFeel.StyleChanged -= LookAndFeel_StyleChanged;
				}
				if(this.teSearch != null) {
					this.teSearch.KeyDown -= teSearch_KeyDown;
				}
				GotFocus -= AccordionSearchControl_GotFocus;
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.teSearch = new DevExpress.XtraEditors.SearchControl();
			this.lbSearchIcon = new AccordionLabelControl();
			((System.ComponentModel.ISupportInitialize)(this.teSearch.Properties)).BeginInit();
			this.SuspendLayout();
			this.teSearch.Location = new System.Drawing.Point(102, 43);
			this.teSearch.Name = "teSearch";
			this.teSearch.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.teSearch.Size = new System.Drawing.Size(108, 18);
			this.teSearch.TabIndex = 0;
			this.teSearch.EditValueChanged += teSearch_EditValueChanged;
			this.teSearch.Properties.Buttons.Add(new DevExpress.XtraEditors.Repository.ClearButton());
			this.teSearch.Properties.NullValuePromptShowForEmptyValue = true;
			this.lbSearchIcon.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			this.lbSearchIcon.Location = new System.Drawing.Point(45, 40);
			this.lbSearchIcon.Name = "lbSearchIcon";
			this.lbSearchIcon.Size = new System.Drawing.Size(0, 13);
			this.lbSearchIcon.TabIndex = 1;
			this.lbSearchIcon.Click += new System.EventHandler(this.peSearchIconClick);
			this.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.Appearance.Options.UseBackColor = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.lbSearchIcon);
			this.Controls.Add(this.teSearch);
			this.Name = "AccordionSearchControl";
			this.Padding = new System.Windows.Forms.Padding(20);
			this.Size = new System.Drawing.Size(252, 108);
			((System.ComponentModel.ISupportInitialize)(this.teSearch.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.SearchControl teSearch;
		private AccordionLabelControl lbSearchIcon;
	}
}
