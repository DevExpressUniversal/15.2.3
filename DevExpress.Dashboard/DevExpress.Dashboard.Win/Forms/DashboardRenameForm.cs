#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Linq;
using System.Windows.Forms;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Localization;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess;
using DevExpress.DataAccess.Wizard.Services;
namespace DevExpress.DashboardWin.Native {
	public partial class DashboardRenameForm : DashboardForm {
		readonly DashboardDesigner designer;
		readonly IRenameFormModel model;
		public DashboardRenameForm() {
			InitializeComponent();
		}
		public DashboardRenameForm(DashboardDesigner designer, IRenameFormModel model)
			: this() {
			this.designer = designer;
			this.model = model;
			LookAndFeel.ParentLookAndFeel = designer.LookAndFeel;
			edtDataSourceName.Text = model.Name;
			edtDataSourceName.Focus();
			Text = model.Caption;
		}
		void ButtonOKClick(object sender, EventArgs e) {
			if(model.Name == edtDataSourceName.Text || model.TryRename(edtDataSourceName.Text)) {
				DialogResult = DialogResult.OK;
			}
		}
	}
	public interface IRenameFormModel {
		string Caption { get; }
		string Name { get; }
		bool TryRename(string newName);
	}
	public class DataSourceRenameFormModel : IRenameFormModel {
		readonly IDashboardDataSource dataSource;
		readonly DashboardDesigner designer;
		public string Name { get { return dataSource.Name; } }
		public string Caption { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RenameDataSourceFormCaption); } }
		public DataSourceRenameFormModel(IDashboardDataSource dataSource, DashboardDesigner designer) {
			this.dataSource = dataSource;
			this.designer = designer;
		}
		public bool TryRename(string newName) {
			IErrorInfo errorInfo = designer.Dashboard.DataSourceCaptionGenerator.ValidateName(newName);
			if(errorInfo == null) {
				RenameDataSourceHistoryItem historyItem = new RenameDataSourceHistoryItem(dataSource, newName);
				designer.History.RedoAndAdd(historyItem);
				return true;
			} else
				designer.ErrorHandler.HandleError(errorInfo);
			return false;
		}
	}
	public class QueryRenameFormModel : IRenameFormModel {
		readonly SqlQuery query;
		readonly DashboardDesigner designer;
		public string Name { get { return query.Name; } }
		public string Caption { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RenameQueryFormCaption); } }
		public QueryRenameFormModel(SqlQuery query, DashboardDesigner designer) {
			this.query = query;
			this.designer = designer;
		}
		public bool TryRename(string newName) {
			try {
				query.Name = newName;
			} catch(InvalidNameException exception) {
				DashboardWinHelper.ShowWarningMessage(designer.LookAndFeel, designer, exception.Message);
				return false;
			}
			return true;
		}
	}
}
