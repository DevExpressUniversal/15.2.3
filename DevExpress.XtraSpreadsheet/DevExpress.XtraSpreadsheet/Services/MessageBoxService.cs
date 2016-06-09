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
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Spreadsheet;
using DevExpress.Utils;
using DevExpress.XtraEditors;
namespace DevExpress.XtraSpreadsheet.Services.Implementation {
	#region MessageBoxService
	public class MessageBoxService : IMessageBoxService {
		readonly Control control;
		readonly UserLookAndFeel lookAndFeel;
		public MessageBoxService(Control control, UserLookAndFeel lookAndFeel) {
			Guard.ArgumentNotNull(control, "control");
			Guard.ArgumentNotNull(lookAndFeel, "lookAndFeel");
			this.control = control;
			this.lookAndFeel = lookAndFeel;
		}
		public DialogResult ShowMessage(string message, string title, MessageBoxIcon icon) {
			return XtraMessageBox.Show(lookAndFeel, control, message, title, MessageBoxButtons.OK, icon);
		}
		public DialogResult ShowDataValidationDialog(string message, string title, DataValidationErrorStyle errorStyle) {
			MessageBoxButtons buttons;
			MessageBoxIcon icon;
			if (errorStyle == DataValidationErrorStyle.Stop) {
				buttons = MessageBoxButtons.RetryCancel;
				icon = MessageBoxIcon.Error;
			}
			else if (errorStyle == DataValidationErrorStyle.Warning) {
				buttons = MessageBoxButtons.YesNoCancel;
				icon = MessageBoxIcon.Warning;
			}
			else {
				buttons = MessageBoxButtons.OKCancel;
				icon = MessageBoxIcon.Information;
			}
			return XtraMessageBox.Show(lookAndFeel, control, message, title, buttons, icon);
		}
		public bool ShowOkCancelMessage(string message) {
			return DialogResult.OK == XtraMessageBox.Show(lookAndFeel, control, message, Application.ProductName, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
		}
		public bool ShowYesNoMessage(string message) {
			return DialogResult.Yes == XtraMessageBox.Show(lookAndFeel, control, message, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
		}
		public DialogResult ShowYesNoCancelMessage(string message) {
			return XtraMessageBox.Show(lookAndFeel, control, message, Application.ProductName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
		}
	}
	#endregion
}
