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
using System.Windows.Forms;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.Wizard.Views;
namespace DevExpress.DataAccess.UI.Wizard.Views {
	[ToolboxItem(false)]
	public partial class ChooseEFConnectionStringPageView : WizardViewBase, IChooseEFConnectionStringPageView {
		public ChooseEFConnectionStringPageView() {
			InitializeComponent();
			LocalizeComponent();
		}
		#region IWizardPageView Members
		public override string HeaderDescription {
			get { return DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseEFConnectionString); }
		}
		#endregion
		#region IChooseEFConnectionStringPageView Members
		public event EventHandler Changed;
		public bool ShouldCreateNewConnection {
			get { return checkCreateNewConnection.Checked || listBoxChooseConnection.ItemCount == 0; }
		}
		public string ExistingConnectionName {
			get { return (string)this.listBoxChooseConnection.SelectedItem; }
		}
		public void Initialize() {
			layoutControlContent.LookAndFeel.ParentLookAndFeel = LookAndFeel;
		}
		public void SetConnections(IEnumerable<INamedItem> connections) {
			listBoxChooseConnection.Items.Clear();
			foreach(var item in connections) {
				listBoxChooseConnection.Items.Add(item.Name);
			}
		}
		public void SetSelectedConnection(INamedItem connection) {
			if(connection == null) {
				checkCreateNewConnection.Checked = true;
				return;
			}
			foreach(object item in this.listBoxChooseConnection.Items)
				if((string)item == connection.Name) {
					listBoxChooseConnection.SelectedItem = item;
					return;
				}
		}
		#endregion
		protected void RaiseChanged() {
			if(Changed != null)
				Changed(this, EventArgs.Empty);
		}
		void LocalizeComponent() {
			checkCreateNewConnection.Properties.Caption = DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseEFConnectionString_CustomConnection);
			checkChooseConnection.Properties.Caption = DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseEFConnectionString_ChooseConnection);
		}
		void chooseConnectionListBox_SelectedIndexChanged(object sender, EventArgs e) {
			RaiseChanged();
		}
		void ceCreateNewConnection_CheckedChanged(object sender, EventArgs e) {
			listBoxChooseConnection.Enabled = checkChooseConnection.Checked;
			RaiseChanged();
		}
		void chooseConnectionListBox_MouseDoubleClick(object sender, MouseEventArgs e) {
			if(this.listBoxChooseConnection.IndexFromPoint(e.Location) != -1)
				this.MoveForward();
		}		
	}
}
