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
using System.Windows;
using System.Windows.Forms;
using DevExpress.Xpf.Core;
namespace DevExpress.XtraSpreadsheet.Services.Implementation {
	#region MessageBoxService
	public class MessageBoxService : IMessageBoxService {
		public DialogResult ShowMessage(string message, string title, MessageBoxIcon icon) {
			DXMessageBox.Show(message, title, MessageBoxButton.OK, (MessageBoxImage)icon);
			return DialogResult.OK;
		}
		public bool ShowOkCancelMessage(string message) {
			return DXMessageBox.Show(message, System.Windows.Forms.Application.ProductName, MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK;
		}
		public bool ShowYesNoMessage(string message) {
			return DXMessageBox.Show(message, System.Windows.Forms.Application.ProductName, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
		}
		public DialogResult ShowYesNoCancelMessage(string message) {
			DXMessageBox.Show(message, System.Windows.Forms.Application.ProductName, MessageBoxButton.YesNoCancel, MessageBoxImage.Information);
			return DialogResult.Yes;
		}
		public DialogResult ShowDataValidationDialog(string message, string title, DevExpress.Spreadsheet.DataValidationErrorStyle errorStyle) {
			if (errorStyle == DevExpress.Spreadsheet.DataValidationErrorStyle.Stop) {
				MessageBoxResult result = DXMessageBox.Show(message, title, MessageBoxButton.OKCancel, MessageBoxImage.Stop);
				return result == MessageBoxResult.OK ? DialogResult.No : (DialogResult)result;
			}
			if (errorStyle == DevExpress.Spreadsheet.DataValidationErrorStyle.Warning)
				return (DialogResult)DXMessageBox.Show(message, title, MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
			return (DialogResult)DXMessageBox.Show(message, title, MessageBoxButton.OKCancel, MessageBoxImage.Information);
		}
	}
	#endregion
}
