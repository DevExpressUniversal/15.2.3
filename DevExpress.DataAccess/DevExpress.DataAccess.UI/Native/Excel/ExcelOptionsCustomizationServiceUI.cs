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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.DataAccess.Excel;
using DevExpress.DataAccess.Native.Excel;
using DevExpress.DataAccess.UI.Wizard.Views;
using DevExpress.LookAndFeel;
using DevExpress.SpreadsheetSource;
using DevExpress.Utils;
namespace DevExpress.DataAccess.UI.Native.Excel {
	public class ExcelOptionsCustomizationServiceUI : IExcelOptionsCustomizationService {
		readonly IWin32Window owner;
		readonly UserLookAndFeel lookAndFeel;
		readonly TaskScheduler ui;
		public ExcelOptionsCustomizationServiceUI(IWin32Window owner, TaskScheduler ui, UserLookAndFeel lookAndFeel) {
			this.ui = ui;
			this.lookAndFeel = lookAndFeel;
			this.owner = owner;
		}
		#region Implementation of IExcelParametersService
		public virtual void Customize(BeforeFillEventArgs eventArgs) {
			Guard.ArgumentNotNull(eventArgs, "eventArgs");
			var excelSourceOptions = eventArgs.SourceOptions as ExcelSourceOptions;
			bool isExcelOptions = excelSourceOptions != null;
			Exception exception;
			do {
				exception = null;
				try {
					if(isExcelOptions) {
						TryCreateExcelSource(eventArgs.FileName, excelSourceOptions.Password);
					}
					else {
						TryCreateCsvSource(eventArgs.FileName);
					}
				}
				catch(EncryptedFileException ex) {
					if(ex.Error == EncryptedFileError.EncryptionTypeNotSupported) {
						throw;
					}
					exception = ex.Wrap();
				}
				catch(IOException ex) {
					exception = ex;
				}
				if(exception != null) {
					using(PasswordRequestForm form = new PasswordRequestForm(
						isExcelOptions,
						canSavePassword: false) {
							FileName = eventArgs.FileName,
							JustificationText = exception.Message
						}) {
						form.LookAndFeel.ParentLookAndFeel = lookAndFeel;
						if(form.ShowDialog(owner) != DialogResult.OK)
							throw exception;
						eventArgs.FileName = form.FileName;
						if(isExcelOptions) {
							excelSourceOptions.Password = form.Password;
						}
					}
				}
			}
			while(exception != null);
		}
		void TryCreateExcelSource(string fileName, string password) {
			ExcelDataLoaderHelper.CreateSource(null, ExcelDocumentFormat.Xls, fileName, new ExcelSourceOptions {Password = password});
		}
		void TryCreateCsvSource(string fileName) {
			ExcelDataLoaderHelper.CreateSource(null, ExcelDocumentFormat.Csv, fileName, new CsvSourceOptions());
		}
		#endregion
	}
}
