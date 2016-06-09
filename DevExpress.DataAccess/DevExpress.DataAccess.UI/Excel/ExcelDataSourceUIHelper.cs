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
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using DevExpress.DataAccess.Excel;
using DevExpress.DataAccess.Native.Excel;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.UI.Native;
using DevExpress.DataAccess.UI.Wizard;
using DevExpress.DataAccess.UI.Wizard.Clients;
using DevExpress.DataAccess.UI.Wizard.Services;
using DevExpress.DataAccess.Wizard;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Native;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraEditors;
namespace DevExpress.DataAccess.UI.Excel {
	public static class ExcelDataSourceUIHelper {
		#region inner classes
		class DataSourceEditorRunner : ExcelDataSourceWizardRunner<ExcelDataSourceModel> {
			public DataSourceEditorRunner(IWizardRunnerContext context) : base(context) { }
			#region Overrides of ExcelDataSourceWizardRunner<ExcelDataSourceModel>
			protected override string WizardTitle {
				get { return DataSourceEditorCaption; }
			}
			#endregion
		}
		#endregion
		#region EditDataSource(...)
		public static bool EditDataSource(this ExcelDataSource dataSource) {
			return EditDataSource(dataSource, null);
		}
		public static bool EditDataSource(this ExcelDataSource dataSource, UserLookAndFeel lookAndFeel) {
			return EditDataSource(dataSource, lookAndFeel, null);
		}
		public static bool EditDataSource(this ExcelDataSource dataSource, UserLookAndFeel lookAndFeel, IWin32Window owner) {
			return EditDataSource(dataSource, lookAndFeel, owner, new ExcelSchemaProvider());
		}
		public static bool EditDataSource(this ExcelDataSource dataSource, UserLookAndFeel lookAndFeel, IWin32Window owner, IExcelSchemaProvider excelSchemaProvider) {
			return EditDataSource(dataSource, new DefaultWizardRunnerContext(lookAndFeel, owner), excelSchemaProvider);
		}
		public static bool EditDataSource(this ExcelDataSource dataSource, IWizardRunnerContext context, IExcelSchemaProvider excelSchemaProvider) {
			Guard.ArgumentNotNull(dataSource, "dataSource");
			Guard.ArgumentNotNull(excelSchemaProvider, "excelSchemaProvider");
			Guard.ArgumentNotNull(context, "context");
			var model = CreateModel(dataSource);
			var runner = new DataSourceEditorRunner(context);
			var client = new ExcelDataSourceWizardClientUI(excelSchemaProvider);
			if(!runner.Run(client, model))
				return false;
			model = runner.WizardModel;
			dataSource.SourceOptions = model.SourceOptions;
			dataSource.FileName = model.FileName;
			dataSource.Schema.Clear();
			dataSource.Schema.AddRange(model.Schema);
			ExcelSourceOptions excelOptions = model.SourceOptions as ExcelSourceOptions;
			if(excelOptions != null && !string.IsNullOrEmpty(excelOptions.Password) && !model.ShouldSavePassword) {
				excelOptions.PasswordInternal = excelOptions.Password;
				excelOptions.Password = null;
			}
			return true;
		}
		#endregion
		#region UpdateSchema(...)
		public static bool UpdateSchema(this ExcelDataSource dataSource, IWin32Window owner, UserLookAndFeel lookAndFeel, IExcelSchemaProvider schemaProvider) {
			Guard.ArgumentNotNull(dataSource, "dataSource");
			schemaProvider = schemaProvider ?? new ExcelSchemaProvider();
			if(string.IsNullOrEmpty(dataSource.FileName) && dataSource.Stream == null || dataSource.SourceOptions == null) {
				XtraMessageBox.Show(lookAndFeel, owner, "FileName and SourceOptions should be assigned", "ExcelDataSource", MessageBoxButtons.OK, MessageBoxIcon.Exclamation); 
				return false;
			}
			var beforeFillEventArgs = new BeforeFillEventArgs {FileName = dataSource.FileName, SourceOptions = dataSource.SourceOptions.Clone()};
			var excelOptionsCustomizationService = (IExcelOptionsCustomizationService)dataSource.GetService(typeof(IExcelOptionsCustomizationService));
			if(excelOptionsCustomizationService != null) {
				excelOptionsCustomizationService.Customize(beforeFillEventArgs);
			}
			Func<CancellationToken, FieldInfo[]> getSchema = (token) => schemaProvider.GetSchema(beforeFillEventArgs.FileName, null, ExcelDocumentFormat.Csv, beforeFillEventArgs.SourceOptions, token);
			var waitFormActivator = new WaitFormActivatorDesignTime(owner, typeof(WaitFormWithCancel), lookAndFeel.ActiveSkinName);
			var fieldInfos = ExcelSchemaLoaderAsync.GetSchema(getSchema, waitFormActivator, new ExceptionHandler(lookAndFeel, owner, "Cannon get schema")); 
			if(fieldInfos == null) {
				return false;
			}
			dataSource.Schema.Clear();
			dataSource.Schema.AddRange(fieldInfos);
			XtraMessageBox.Show(lookAndFeel, owner, "Schema has been updated successfully", "ExcelDataSource", MessageBoxButtons.OK, MessageBoxIcon.Information); 
			return true;
		}
		#endregion
		#region private members
		static ExcelDataSourceModel CreateModel(ExcelDataSource dataSource) {
			return new ExcelDataSourceModel {
				SourceOptions = dataSource.SourceOptions != null ? dataSource.SourceOptions.Clone() : null,
				FileName = dataSource.FileName,
				Schema = dataSource.Schema == null ? new FieldInfo[0] : dataSource.Schema.Select(fi => new FieldInfo { Name = fi.Name, Type = fi.Type, Selected = fi.Selected }).ToArray()
			};
		}
		static string DataSourceEditorCaption {
			get { return DataAccessUILocalizer.GetString(DataAccessUIStringId.ExcelDataSourceEditorTitle); }
		}
		#endregion
	}
}
