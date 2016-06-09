#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Text;
using System.Data;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Drawing;
namespace DevExpress.ExpressApp.Design {
	public class ConnectionTray : ListViewTray<XafApplication> {
		private void Application_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
			if(e.PropertyName == "Connection") {
				RefreshItems();
			}
		}
		protected override bool AllowAddItem(System.Drawing.Design.ToolboxItem item) {
			if(base.AllowAddItem(item)) {
				Type itemType = designer.GetToolboxItemType(item);
				if(typeof(IDbConnection).IsAssignableFrom(itemType)) {
					return true;
				}
			}
			return false;
		}
		protected override bool CanProcessToolboxItem(ToolboxItem item) {
			Type itemType = designer.GetToolboxItemType(item);
			if(!(typeof(IDbConnection).IsAssignableFrom(itemType))) {
				return false;
			}
			PropertyDescriptor ownerProperty = TypeDescriptor.GetProperties(DataSource)["Connection"];
			IDbConnection oldConnection = (IDbConnection)ownerProperty.GetValue(DataSource);
			if(oldConnection != null) {
				if(MessageBox.Show("Application already contains connection.\nDo you want to replace it?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) {
					return false;
				}
			}
			return true;
		}
		protected override void ComponentsCreated(IComponent[] createdComponents) {
			base.ComponentsCreated(createdComponents);
			if(createdComponents.Length > 0 && createdComponents[0] is IDbConnection) {
				IDbConnection oldConnection = DataSource.Connection;
				DataSource.Connection = (IDbConnection)createdComponents[0];
				designer.ComponentChangeService.OnComponentChanged(DataSource, TypeDescriptor.GetProperties(DataSource)["Connection"], oldConnection, DataSource.Connection);
				if(oldConnection != null && oldConnection is IComponent) {
					designer.DesignerHost.DestroyComponent((IComponent)oldConnection);
				}
			}
			RefreshItems();
		}
		protected override void OnDataSourceChanged(XafApplication oldDataSource, XafApplication newDataSource) {
			if(oldDataSource != null) {
				oldDataSource.PropertyChanged -= new PropertyChangedEventHandler(Application_PropertyChanged);
			}
			if(newDataSource != null) {
				newDataSource.PropertyChanged += new PropertyChangedEventHandler(Application_PropertyChanged);
			}
		}
		protected override void RefreshItemsCore() {
			this.Items.Clear();
			LargeImageList.Images.Clear();
			this.LargeImageList.Images.Add(typeof(System.Data.SqlClient.SqlConnection).FullName, DesignImagesLoader.GetImage("Designer_Connection_Sql.ico"));
			this.LargeImageList.Images.Add(typeof(System.Data.OleDb.OleDbConnection).FullName, DesignImagesLoader.GetImage("Designer_Connection_OleDb.ico"));
			this.LargeImageList.Images.Add(typeof(System.Data.Odbc.OdbcConnection).FullName, DesignImagesLoader.GetImage("Designer_Connection_Odbc.ico"));
			this.LargeImageList.Images.Add("System.Data.OracleClient.OracleConnection", DesignImagesLoader.GetImage("Designer_Connection_Oracle.ico"));
			IDbConnection connection = DataSource.Connection;
			if(connection != null) {
				AddListViewItem(connection, connection.GetType().Name);
			}
		}
		public ConnectionTray()
			: base() {
			ToolTipMessage = "To add a Connection, drag it from the Toolbox, and use the Properties window to set its properties.";
			canShowPlaceholder = true;
			canShowToolTip = true;
			SetTooltip();
		}
	}
}
