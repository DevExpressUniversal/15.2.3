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
using System.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraSpreadsheet.Forms.Design {
	[DXToolboxItem(false)]
	public partial class EmailAddressControl : XtraUserControl {
		public EmailAddressControl() {
			InitializeComponent();
		}
		#region Properties
		public string EmailText { get { return teEmail.Text; } set { teEmail.Text = value; } }
		public string SubjectText { get { return teSubject.Text; } set { teSubject.Text = value; } }
		public XtraEditors.TextEdit EmailTextEdit { get { return teEmail; } }
		public XtraEditors.TextEdit SubjectTextEdit { get { return teSubject; } }
		public int EmailSelectionStart { get { return teEmail.SelectionStart; } set { teEmail.SelectionStart = value; } }
		public int SubjectSelectionStart { get { return teSubject.SelectionStart; } set { teSubject.SelectionStart = value; } }
		#endregion
		#region Events
		public event ChangingEventHandler EmailEditValueChanging { add { teEmail.EditValueChanging += value; } remove { teEmail.EditValueChanging -= value; } }
		public event ChangingEventHandler SubjectEditValueChanging { add { teSubject.EditValueChanging += value; } remove { teSubject.EditValueChanging -= value; } }
		#endregion
	}
}
