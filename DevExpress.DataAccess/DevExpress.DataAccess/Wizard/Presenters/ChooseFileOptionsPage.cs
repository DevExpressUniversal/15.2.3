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
using System.IO;
using System.Linq;
using System.Threading;
using DevExpress.Data.WizardFramework;
using DevExpress.DataAccess.Excel;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Excel;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Native;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.SpreadsheetSource.Csv;
namespace DevExpress.DataAccess.Wizard.Presenters {
	public class ChooseFileOptionsPage<TModel> : WizardPageBase<IChooseFileOptionsPageView, TModel> where TModel : IExcelDataSourceModel {
		ExcelDocumentFormat documentFormat;
		readonly IWizardRunnerContext context;
		readonly IExcelSchemaProvider schemaProvider;
		CsvSourceOptions optionsForDetect = new CsvSourceOptions {DetectNewlineType = true, DetectValueSeparator = true, DetectEncoding = true};
		public ChooseFileOptionsPage(IChooseFileOptionsPageView view, IWizardRunnerContext context, IExcelSchemaProvider excelSchemaProvider)
			: base(view) {
			this.context = context;
			schemaProvider = excelSchemaProvider;
		}
		protected virtual IWaitFormActivator WaitFormActivator { get { return context.WaitFormActivator; } }
		protected virtual IExceptionHandler ExceptionHandler { get { return context.CreateExceptionHandler(ExceptionHandlerKind.Default); } }
		#region Overrides of WizardPageBase<IChooseExcelFileAndOptionsPageView,TModel>
		public override bool MoveNextEnabled {
			get { return true; }
		}
		public override Type GetNextPageType() {
			return documentFormat == ExcelDocumentFormat.Csv
				? typeof(ConfigureExcelFileColumnsPage<TModel>)
				: typeof(ChooseExcelFileDataRangePage<TModel>);
		}
		public override void Begin() {
			documentFormat = ExcelDataLoaderHelper.DetectFormat(Model.FileName);
			if(documentFormat == ExcelDocumentFormat.Csv) {
				DetectOptions((CsvSourceOptions)Model.SourceOptions);
			}
			View.DocumentFormat = documentFormat;
			View.Initialize(Model.SourceOptions.Clone());
			View.DetectEncoding += View_DetectEncoding;
			View.DetectNewlineType += View_DetectNewlineType;
			View.DetectValueSeparator += View_DetectValueSeparator;
		}
		void View_DetectValueSeparator(object sender, EventArgs e) {
			DetectOptions(optionsForDetect);
			View.SetValueSeparator(optionsForDetect.ValueSeparator);
		}
		void View_DetectNewlineType(object sender, EventArgs e) {
			DetectOptions(optionsForDetect);
			View.SetNewlineType(optionsForDetect.NewlineType);
		}
		void View_DetectEncoding(object sender, EventArgs e) {
			DetectOptions(optionsForDetect);
			View.SetEncoding(optionsForDetect.Encoding);
		}
		public override bool Validate(out string errorMessage) {
			errorMessage = null;
			Func<CancellationToken, FieldInfo[]> getSchema = (token) => schemaProvider.GetSchema(Model.FileName, null, ExcelDocumentFormat.Xls, View.SourceOptions, token);
			if(documentFormat == ExcelDocumentFormat.Csv) {
				if(Model.Schema == null) {
					Model.Schema = ExcelSchemaLoaderAsync.GetSchema(getSchema, WaitFormActivator, ExceptionHandler);
					return Model.Schema != null;
				}
				if(!Model.SourceOptions.Equals(View.SourceOptions)) {
					var oldSchema = Model.Schema;
					Model.Schema = ExcelSchemaLoaderAsync.GetSchema(getSchema, WaitFormActivator, ExceptionHandler);
					if(Model.Schema == null) {
						return false;
					}
					foreach(FieldInfo fieldInfo in Model.Schema) {
						var match = oldSchema.FirstOrDefault(fi => fi.Name == fieldInfo.Name);
						if(match != null)
							fieldInfo.Selected = match.Selected;
					}
				}
			}
			return true;
		}
		public override void Commit() {
			View.DetectEncoding -= View_DetectEncoding;
			View.DetectNewlineType -= View_DetectNewlineType;
			View.DetectValueSeparator -= View_DetectValueSeparator;
			Model.SourceOptions = View.SourceOptions;
		}
		#endregion
		void DetectOptions(CsvSourceOptions options) {
			if(!options.DetectEncoding && !options.DetectNewlineType && !options.DetectValueSeparator)
				return;
			using(Stream stream = new FileStream(Model.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
				CsvSpreadsheetSourceOptions spreadsheetSourceOptions = new CsvSpreadsheetSourceOptions();
				spreadsheetSourceOptions.AutoDetect(stream);
				if(options.DetectEncoding)
					options.Encoding = spreadsheetSourceOptions.Encoding;
				if(options.DetectNewlineType)
					options.NewlineType = (CsvNewlineType)spreadsheetSourceOptions.NewlineType;
				if(options.DetectValueSeparator)
					options.ValueSeparator = spreadsheetSourceOptions.ValueSeparator;
			}
		}
	}
}
