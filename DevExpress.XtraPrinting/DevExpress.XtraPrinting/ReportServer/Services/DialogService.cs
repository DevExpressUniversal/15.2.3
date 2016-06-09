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

using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraPrinting;
namespace DevExpress.ReportServer.Printing.Services {
	public class DialogService : IDialogService {
		readonly ISupportLookAndFeel supportLookAndFeel;
		readonly IWin32Window parent;
		protected ISupportLookAndFeel SupportLookAndFeel { get { return supportLookAndFeel; } }
		protected IWin32Window Parent { get { return parent; } }
		public DialogService(ISupportLookAndFeel supportLookAndFeel, IWin32Window parent) {
			this.supportLookAndFeel = supportLookAndFeel;
			this.parent = parent;
		}
		public void ShowErrorMessage(Exception error) {
			ShowErrorMessage(error, parent);
		}
		public void ShowErrorMessage(Exception error, IWin32Window owner) {
			if(error != null)
				NotificationService.ShowException<PrintingSystemBase>(supportLookAndFeel.LookAndFeel, owner, error);
		}
		public void ShowErrorMessage(string message) {
			ShowErrorMessage(message, parent);
		}
		public void ShowErrorMessage(string message, IWin32Window owner) {
			NotificationService.ShowMessage<PrintingSystemBase>(supportLookAndFeel.LookAndFeel, owner, message, PreviewStringId.Msg_ErrorTitle.GetString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
}
