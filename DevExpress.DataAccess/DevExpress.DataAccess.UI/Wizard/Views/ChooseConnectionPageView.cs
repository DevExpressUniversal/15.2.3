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
using System.Linq;
using System.Windows.Forms;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.Wizard;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.XtraLayout.Utils;
namespace DevExpress.DataAccess.UI.Wizard.Views {
	[ToolboxItem(false)]
	public partial class ChooseConnectionPageView : WizardViewBase, IChooseConnectionPageView {
		readonly SqlWizardOptions options;
		public ChooseConnectionPageView(IEnumerable<SqlDataConnection> connections, SqlWizardOptions options) {
			this.options = options;
			InitializeComponent();
			LocalizeComponent();
			this.layoutControlContent.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			radioGroupCreateNewConnection.EditValue = false;
			SetConnections(connections);
			if(DisableNewConnections)
				layoutItemCreateConnection.Visibility = LayoutVisibility.Never;
		}
		protected bool DisableNewConnections { get { return this.options.HasFlag(SqlWizardOptions.DisableNewConnections); } }
		ChooseConnectionPageView() : this(Enumerable.Empty<SqlDataConnection>(), SqlWizardOptions.None) { }
		#region IWizardPageView Members
		public override string HeaderDescription {
			get {
				return DisableNewConnections
					? DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseConnectionNoChoice)
					: DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseConnection);
			}
		}
		#endregion
		#region IChooseConnectionPageView Members
		public void SetSelectedConnection(INamedItem connection) {
			if(connection == null)
				return;
			foreach(object item in this.listBoxChooseConnection.Items)
				if((string) item == connection.Name) {
					this.listBoxChooseConnection.SelectedItem = item;
					return;
				}
			radioGroupCreateNewConnection.EditValue = true;
		}
		public bool ShouldCreateNewConnection {
			get { return (bool)radioGroupCreateNewConnection.EditValue || listBoxChooseConnection.ItemCount == 0; }
		}
		public string ExistingConnectionName {
			get { return (string) this.listBoxChooseConnection.SelectedItem; }
		}
		public event EventHandler Changed;
		#endregion
		void SetConnections(IEnumerable<INamedItem> connections) {
			this.listBoxChooseConnection.Items.Clear();
			foreach(var item in connections) {
				this.listBoxChooseConnection.Items.Add(item.Name);
			}
		}
		void chooseConnectionListBox_SelectedIndexChanged(object sender, EventArgs e) {
			RaiseChanged();
		}
		protected void RaiseChanged() {
			if(Changed != null)
				Changed(this, EventArgs.Empty);
		}
		void LocalizeComponent() {
			radioGroupCreateNewConnection.Properties.Items[0].Description = DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseConnection_SpecifyCustomConnection);
			radioGroupCreateNewConnection.Properties.Items[1].Description = DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseConnection_ChooseExistingConnection);
		}
		private void chooseConnectionListBox_MouseDoubleClick(object sender, MouseEventArgs e) {
			if(this.listBoxChooseConnection.IndexFromPoint(e.Location) != -1)
				this.MoveForward();
		}
		private void rgCreateNewConnection_EditValueChanged(object sender, EventArgs e) {
			this.listBoxChooseConnection.Enabled = !(bool)radioGroupCreateNewConnection.EditValue;
			RaiseChanged();
		}
	}
}
