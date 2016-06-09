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
using System.Reflection;
using System.Windows.Forms;
using DevExpress.DataAccess;
using DevExpress.DataAccess.EntityFramework;
using DevExpress.DataAccess.Excel;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.EntityFramework;
using DevExpress.DataAccess.Native.Excel;
using DevExpress.DataAccess.ObjectBinding;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.UI.EntityFramework;
using DevExpress.DataAccess.UI.ObjectBinding;
using DevExpress.DataAccess.UI.Sql;
using DevExpress.DataAccess.UI.Wizard;
using DevExpress.DataAccess.UI.Excel;
using DevExpress.DataAccess.Wizard;
using DevExpress.DataAccess.Wizard.Native;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Entity.ProjectModel;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraSpreadsheet.Forms {
	public partial class ManageDataSourcesForm : XtraForm {
		#region
		readonly ManageDataSourcesViewModel viewModel;
		#endregion
		#region Properties
		public ManageDataSourcesViewModel ViewModel { get { return viewModel; } }
		#endregion
		public ManageDataSourcesForm() {
			InitializeComponent();
		}
		public ManageDataSourcesForm(ManageDataSourcesViewModel viewModel) {
			this.viewModel = viewModel;
			InitializeComponent();
			SetupBindings();
		}
		void SetupBindings() {
			grdDataSources.BeginUpdate();
			grdDataSources.DataSource = ViewModel.DataSources;
			btnRemove.DataBindings.Add("Enabled", ViewModel, "CanRemove");
			gridView1.Columns[0].ColumnEdit = this.repositoryItemButtonEdit1;
			gridView1.Columns[0].Caption = "Data Sources";
			grdDataSources.EndUpdate();
		}
		private void btnAdd_Click(object sender, EventArgs e) {
			grdDataSources.BeginUpdate();
			ViewModel.PerformAdd();
			grdDataSources.EndUpdate();
		}
		private void btnRemove_Click(object sender, EventArgs e) {
			int index = gridView1.FocusedRowHandle;
			if(index == -1)
				return;
			grdDataSources.BeginUpdate();
			ViewModel.PerformRemove(index);
			grdDataSources.EndUpdate();
		}
		void repositoryItemButtonEdit1_ButtonClick(object sender, ButtonPressedEventArgs e) {
			int index = gridView1.FocusedRowHandle;
			IDataComponent dataComponent = ViewModel.GetDataComponent(index);
			UserLookAndFeel lookAndFeel = ((SpreadsheetControl)ViewModel.SpreadsheetControl).LookAndFeel;
			IParameterService parameterService = ViewModel.DocumentModel.MailMergeParameters.GetParameterService();
			ConnectionStorageService connectionStorage = new ConnectionStorageService();
			ISolutionTypesProvider solutionTypesProvider = EntityServiceHelper.GetRuntimeSolutionProvider(Assembly.GetEntryAssembly());
			SqlDataSource sqlDataSource = dataComponent as SqlDataSource;
			if(sqlDataSource != null)
				sqlDataSource.ConfigureConnection(new ConfigureConnectionContext{ LookAndFeel = lookAndFeel, Owner = this });
			EFDataSource efDataSource = dataComponent as EFDataSource;
			if(efDataSource != null) {
				RuntimeConnectionStringsProvider connectionStrings = new RuntimeConnectionStringsProvider();
				DefaultWizardRunnerContext wizardRunnerContext = new DefaultWizardRunnerContext(lookAndFeel, ViewModel.SpreadsheetControl);
				efDataSource.EditConnection(wizardRunnerContext, solutionTypesProvider, connectionStrings, connectionStorage, parameterService);
			}
			ObjectDataSource objectDataSource = dataComponent as ObjectDataSource;
			if(objectDataSource != null) {
				DefaultWizardRunnerContext wizardRunnerContext = new DefaultWizardRunnerContext(lookAndFeel, ViewModel.SpreadsheetControl);
				objectDataSource.EditDataSource(solutionTypesProvider, wizardRunnerContext, parameterService, OperationMode.DataOnly);
			}
			ExcelDataSource excelDataSource = dataComponent as ExcelDataSource;
			if(excelDataSource != null) {
				IWizardRunnerContext wizardRunnerContext = new DefaultWizardRunnerContext(lookAndFeel, ViewModel.SpreadsheetControl);
				IExcelSchemaProvider excelSchemaProvider = new ExcelSchemaProvider();
				excelDataSource.EditDataSource(wizardRunnerContext, excelSchemaProvider);
			}
			ViewModel.DocumentModel.MailMergeParameters.UpdateFromParameterService(parameterService);
			grdDataSources.BeginUpdate();
			ViewModel.PerformEditDataSource(dataComponent, index);
			grdDataSources.EndUpdate();
		}
		private void btnOK_Click(object sender, EventArgs e) {
			ViewModel.ApplyChanges();
			DialogResult = DialogResult.OK;
		}
		private void btnCancel_Click(object sender, EventArgs e) {
		}
	}
}
