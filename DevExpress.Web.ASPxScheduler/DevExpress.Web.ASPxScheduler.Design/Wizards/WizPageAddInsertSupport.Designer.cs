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

namespace DevExpress.Web.ASPxScheduler.Design.Wizards {
	partial class WizPageAddInsertSupport {
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizPageAddInsertSupport));
			this.chkAutoRetrieveId = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.headerPicture)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkAutoRetrieveId.Properties)).BeginInit();
			this.SuspendLayout();
			this.titleLabel.Text = "Setup Appointment Storage";
			this.subtitleLabel.Size = new System.Drawing.Size(389, 53);
			this.subtitleLabel.Text = resources.GetString("subtitleLabel.Text");
			this.headerPanel.Size = new System.Drawing.Size(497, 81);
			this.headerSeparator.Location = new System.Drawing.Point(0, 81);
			this.chkAutoRetrieveId.EditValue = true;
			this.chkAutoRetrieveId.Location = new System.Drawing.Point(21, 89);
			this.chkAutoRetrieveId.Name = "chkAutoRetrieveId";
			this.chkAutoRetrieveId.Properties.Caption = "Retrieve and update ID automatically";
			this.chkAutoRetrieveId.Size = new System.Drawing.Size(213, 19);
			this.chkAutoRetrieveId.TabIndex = 5;
			this.Controls.Add(this.chkAutoRetrieveId);
			this.Name = "WizPageAddInsertSupport";
			this.Controls.SetChildIndex(this.chkAutoRetrieveId, 0);
			this.Controls.SetChildIndex(this.headerPanel, 0);
			this.Controls.SetChildIndex(this.headerSeparator, 0);
			this.Controls.SetChildIndex(this.titleLabel, 0);
			this.Controls.SetChildIndex(this.subtitleLabel, 0);
			this.Controls.SetChildIndex(this.headerPicture, 0);
			((System.ComponentModel.ISupportInitialize)(this.headerPicture)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkAutoRetrieveId.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private XtraEditors.CheckEdit chkAutoRetrieveId;
	}
}
