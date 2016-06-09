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
using DevExpress.Data.WizardFramework;
using DevExpress.DataAccess.Excel;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Native.Excel;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.SpreadsheetSource;
namespace DevExpress.DataAccess.Wizard.Presenters {
	public class ChooseFilePage<TModel> : WizardPageBase<IChooseFilePageView, TModel> where TModel : IExcelDataSourceModel {
		readonly IWizardRunnerContext context;
		ExcelSourceOptions actualExcelOptions;
		FileInfo fileInfo;
		public ChooseFilePage(IChooseFilePageView view, IWizardRunnerContext context)
			: base(view) {
			this.context = context;
		}
		#region Overrides of WizardPageBase<IChooseFilePageView,TModel>
		public override bool MoveNextEnabled {
			get { return !string.IsNullOrEmpty(View.FileName); }
		}
		public override void Begin() {
			View.FileName = Model.FileName;
			View.Changed += View_Changed;
		}
		public override bool Validate(out string errorMessage) {
			errorMessage = null;
			if(!File.Exists(View.FileName)) {
				ExceptionHandler.HandleException(
					new FileNotFoundException(
						string.Format(
							DataAccessLocalizer.GetString(DataAccessStringId.ExcelDataSource_FileNotFoundMessage),
							View.FileName),
						View.FileName));
				return false;
			}
			Exception exception;
			do {
				exception = null;
				try {
					TryCreateSource(View.FileName, fileInfo.Password);
					return true;
				}
				catch(EncryptedFileException e) {
					if(e.Error == EncryptedFileError.EncryptionTypeNotSupported) {
						ExceptionHandler.HandleException(e.Wrap());
						return false;
					}
					exception = e.Wrap();
				}
				catch(IOException e) {
					exception = e;
				}
				catch(Exception e) {
					ExceptionHandler.HandleException(e);
					return false;
				}
				if(exception != null && !View.ShowPasswordForm(exception.Message, View.FileName, out this.fileInfo)) {
					return false;
				}
				View.FileName = this.fileInfo.FileName;
			}
			while(exception != null);
			return true;
		}
		public override void Commit() {
			View.Changed -= View_Changed;
			if(Model.FileName != View.FileName) {
				Model.FileName = View.FileName;
				Model.Schema = null;
			}
			ExcelDocumentFormat docFormat;
			try {
				docFormat = DocumentFormat;
			}
			catch(ArgumentException) {
				return;
			}
			catch(IOException) {
				return;
			}
			if(docFormat != ExcelDocumentFormat.Csv) {
				ActualExcelOptions.Password = fileInfo.Password;
				Model.SourceOptions = ActualExcelOptions;
				Model.ShouldSavePassword = fileInfo.SavePassword;
			} else {
				Model.SourceOptions = (Model.SourceOptions as CsvSourceOptions) ?? new CsvSourceOptions {
					DetectNewlineType = true,
					DetectEncoding = true,
					DetectValueSeparator = true
				};
			}
		}
		public override Type GetNextPageType() {
			return typeof(ChooseFileOptionsPage<TModel>);
		}
		#endregion
		protected virtual IExceptionHandler ExceptionHandler { get { return context.CreateExceptionHandler(ExceptionHandlerKind.Default); } }
		ExcelSourceOptions ActualExcelOptions {
			get { return (Model.SourceOptions as ExcelSourceOptions) ?? actualExcelOptions ?? (actualExcelOptions = new ExcelSourceOptions()); }
		}
		ExcelDocumentFormat DocumentFormat {
			get { return ExcelDataLoaderHelper.DetectFormat(View.FileName); }
		}
		void TryCreateSource(string fileName, string password) {
			using(ExcelDataLoaderHelper.CreateSource(
				null,
				DocumentFormat, fileName, DocumentFormat == ExcelDocumentFormat.Csv
					? (ExcelSourceOptionsBase)new CsvSourceOptions()
					: new ExcelSourceOptions { Password = password })) { }
		}
		void View_Changed(object sender, EventArgs e) {
			RaiseChanged();
		}
	}
	public struct FileInfo {
		public string FileName { get; set; }
		public string Password { get; set; }
		public bool SavePassword { get; set; }
	}
}
