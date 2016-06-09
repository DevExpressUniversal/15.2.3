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
using System.ComponentModel;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.XtraLayout.Utils;
namespace DevExpress.DataAccess.UI.Wizard.Views {
	[ToolboxItem(false)]
	public partial class SaveConnectionPageView : WizardViewBase, ISaveConnectionPageView {
		public SaveConnectionPageView() {
			InitializeComponent();
			LocalizeComponent();
		}
		#region IWizardPageView Members
		public override string HeaderDescription {
			get { return DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageSaveConnection); }
		}
		#endregion
		#region ISaveConnectionPageView Members
		public bool ShouldSaveConnectionString {
			get { return this.checkSaveConnection.Checked; }
			set {
				if(this.checkSaveConnection.Checked == value)
					return;
				this.checkSaveConnection.Checked = value; 
			}
		}
		public string ConnectionName {
			get { return this.textConnectionName.Text; }
			set { this.textConnectionName.Text = value; }
		}
		public bool ShouldSaveCredentials {
			get { return (bool)this.radioGroupSaveCredentials.EditValue; }
		}
		public void SetConnectionUsesServerAuth(bool value) {
			layoutGroupCredentials.Visibility = value ? LayoutVisibility.Always : LayoutVisibility.Never;
		}
		public void SetCanSaveToStorage(bool value) {
			this.checkSaveConnection.Checked = value;
			layoutGroupConnection.Visibility = value ? LayoutVisibility.Always : LayoutVisibility.Never;
		}
		#endregion
		void LocalizeComponent() {
			radioGroupSaveCredentials.Properties.Items[0].Description = DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageSaveConnection_SaveCredentials);
			radioGroupSaveCredentials.Properties.Items[1].Description = DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageSaveConnection_SkipSaveCredentials);
			labelSaveCredentials.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageSaveConnection_SaveCredentialsQuestion);
			labelSaveConnection.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageSaveConnection_SaveConnectionString);
			checkSaveConnection.Properties.Caption = DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageSaveConnection_ConfirmSaving);
		}
		private void checkSaveConnection_CheckStateChanged(object sender, EventArgs e) {
			this.textConnectionName.Enabled = this.checkSaveConnection.Checked;
		}
	}
}
