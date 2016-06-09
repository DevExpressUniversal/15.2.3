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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Security.Cryptography.X509Certificates;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraPrinting.Native.WinControls {
	public partial class PdfSignatureEditorForm : XtraForm, IPdfSignatureEditorView {
		public event EventHandler Submit;
		public event EventHandler SelectedCertificateItemChanged;
		public string Reason {
			get {
				return tbReason.Text;
			}
			set {
				tbReason.Text = value;
			}
		}
		public string ContactInfo {
			get {
				return tbContactInfo.Text;
			}
			set {
				tbContactInfo.Text = value;
			}
		}
		public new string Location {
			get {
				return tbLocation.Text;
			}
			set {
				tbLocation.Text = value;
			}
		}
		public ICertificateItem SelectedCertificateItem {
			get {
				return certificateSelector.SelectedItem;
			}
			set {
				certificateSelector.SelectedItem = value;
			}
		}
		public PdfSignatureEditorForm() {
			InitializeComponent();
			certificateSelector.SelectionChanged += (o, e) => {
				if(SelectedCertificateItemChanged != null)
					SelectedCertificateItemChanged(this, EventArgs.Empty);
			};
		}
		public void FillCertificateItems(IEnumerable<ICertificateItem> items) {
			certificateSelector.FillItems(items);
		}
		public void EnableTextEditors(bool enable) {
			tbReason.Enabled = tbLocation.Enabled = tbContactInfo.Enabled = enable;
		}
		void btnOK_Click(object sender, EventArgs e) {
			if(Submit != null) {
				Submit(this, EventArgs.Empty);
			}
			Close();
		}
		void btnCancel_Click(object sender, EventArgs e) {
			Close();
		}
	}
}
