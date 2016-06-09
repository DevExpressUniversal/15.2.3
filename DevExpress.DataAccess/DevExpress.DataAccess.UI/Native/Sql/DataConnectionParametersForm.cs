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
using System.Windows.Forms;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.UI.Native.Sql.DataConnectionControls;
namespace DevExpress.DataAccess.UI.Native.Sql {
	public partial class DataConnectionParametersForm : OkCancelForm {
		readonly ConnectionParametersControl chooser = new ConnectionParametersControl();
		string errorMessage;
		IWin32Window ownerForm;
		public string ErrorMessage { get { return this.errorMessage; } set { this.errorMessage = value; } }
		protected override bool Sizable { get { return false; } }
		public IWin32Window OwnerForm { get { return this.ownerForm; } set { this.ownerForm = value; } }
		public DataConnectionParametersForm() {		 
			InitializeComponent();   
			LocalizeComponent();
			this.chooser.Dock = DockStyle.Fill;			
			this.parametersPanel.Controls.Add(this.chooser);			
		}
		public DataConnectionParametersBase ShowParametersForm(string connectionName, DataConnectionParametersBase connectionParameters) {
			this.detailsText.Text = string.Format(DataAccessUILocalizer.GetString(DataAccessUIStringId.ConnectionErrorFormDetailPattern), connectionName, this.errorMessage);
			this.chooser.SetParameters(connectionParameters);
			return ShowDialog(this.ownerForm) == DialogResult.Cancel ? null : this.chooser.GetParameters();
		}
		void LocalizeComponent() {
			detailsHeaderText.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.DataConnectionParametersDialog_Header_UnableConnect);
			Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.DataConnectionParametersDialog);
		}
		void DataConnectionParametersProviderForm_Load(object sender, EventArgs e) {
			detailsText.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			detailsText.BackColor = BackColor;
			chooser.LookAndFeel.ParentLookAndFeel = LookAndFeel;
		}
	}
}
