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

using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Localization;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
namespace DevExpress.DashboardWin.Native {
	public partial class DataSourceWizardErrorForm : XtraForm {
		public string ErrorText { get { return textEdit1.Text; } set { textEdit1.Text = value; } }
		public string CaptionText { get { return labelControl1.Text; } set { labelControl1.Text = value; } }
		public DataSourceWizardErrorForm() {
			InitializeComponent();
			pictureEdit1.Image = DataAccess.UI.Native.ImageHelper.GetImage("warning");
		}
	}
	public interface IErrorHandlerForm : IErrorHandler {
		void Initialize(IWin32Window owner, UserLookAndFeel lookAndFeel);
	}
	public class ErrorHandler : IErrorHandlerForm {
		IWin32Window ownerWindow;
		UserLookAndFeel lookAndFeel;
		public void Initialize(IWin32Window ownerWindow, UserLookAndFeel lookAndFeel) {
			this.ownerWindow = ownerWindow;
			this.lookAndFeel = lookAndFeel;
		}
		public void ShowErrorForm(string verboseErrorString) {
			ShowErrorFormInternal(string.Empty, DataAccessUILocalizer.GetString(DataAccessUIStringId.ErrorFormDatasourceInitializationText), verboseErrorString);
		}
		public void ShowErrorForm(string formText, string formCaption, string verboseErrorString) {
			ShowErrorFormInternal(formText, formCaption, verboseErrorString);
		}
		public void ShowErrorMessageBox(string errorText) {
			XtraMessageBox.Show(lookAndFeel, ownerWindow, errorText,
				DashboardWinLocalizer.GetString(DashboardWinStringId.MessageBoxErrorTitle), MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}
		void ShowErrorFormInternal(string formText, string formCaption, string verboseErrorString) {
			using(DataSourceWizardErrorForm form = new DataSourceWizardErrorForm()) {
				if(!string.IsNullOrEmpty(formText))
					form.Text = formText;
				form.LookAndFeel.ParentLookAndFeel = lookAndFeel;
				form.CaptionText = formCaption;
				form.ErrorText = verboseErrorString;
				form.ShowDialog(ownerWindow);
			}
		}
		public void ShowDataSourceLoaderResultMessageBox(DataSourceLoadingResultType result) {
			string text = null;
			switch(result) {
				case DataSourceLoadingResultType.Aborted:
					text = DataAccessUILocalizer.GetString(DataAccessUIStringId.LoadingDataSourceAborted);
					break;
				case DataSourceLoadingResultType.OutOfMemory:
					text = DataAccessUILocalizer.GetString(DataAccessUIStringId.LoadingDatasourceOutOfMemoryError);
					break;
			}
			if(!string.IsNullOrEmpty(text))
				ShowErrorMessageBox(text);
		}
		public void ShowDataSourceLoadingErrors(List<DataLoaderError> errors) {
			StringBuilder sb = new StringBuilder();
			foreach(DataLoaderError error in errors) {
				switch(error.Type) {
					case DataLoaderErrorType.Custom:
						sb.AppendFormat(DataAccessUILocalizer.GetString(DataAccessUIStringId.DataSourceName), error.DataSourceName);
						sb.AppendLine();
						sb.AppendFormat(DataAccessUILocalizer.GetString(DataAccessUIStringId.LoadingDataCustomError), error.Message);
						break;
					case DataLoaderErrorType.Query:
						sb.AppendFormat(DataAccessUILocalizer.GetString(DataAccessUIStringId.DataSourceName), error.DataSourceName);
						sb.AppendLine();
						sb.AppendFormat(DataAccessUILocalizer.GetString(DataAccessUIStringId.LoadingDataSqlError), error.Message);
						break;
					case DataLoaderErrorType.Connection:
						sb.AppendFormat(DataAccessUILocalizer.GetString(DataAccessUIStringId.LoadingDataSourceOpeningConnectionError),
							error.DataSourceName, error.Message);
						break;
					case DataLoaderErrorType.PreviewCustom:
						sb.AppendFormat(DataAccessUILocalizer.GetString(DataAccessUIStringId.LoadingDataCustomError), error.Message);
						break;
					case DataLoaderErrorType.PreviewQuery:
						sb.AppendFormat(DataAccessUILocalizer.GetString(DataAccessUIStringId.LoadingDataSqlError), error.Message);
						break;
				}
			}
			ShowErrorForm(sb.ToString());
		}
	}
}
