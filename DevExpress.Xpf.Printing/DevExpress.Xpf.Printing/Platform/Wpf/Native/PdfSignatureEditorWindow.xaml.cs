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

using DevExpress.Xpf.Core;
using DevExpress.XtraPrinting.Native;
using System;
using System.Collections.Generic;
namespace DevExpress.Xpf.Printing.Native {
	public partial class PdfSignatureEditorWindow : DXDialog, IPdfSignatureEditorView {
		public event EventHandler Submit;
		public event EventHandler SelectedCertificateItemChanged;
		public PdfSignatureEditorWindow() {
			InitializeComponent();
			listBoxEdit.SelectedIndexChanged += (o, e) => {
				if(SelectedCertificateItemChanged != null) {
					SelectedCertificateItemChanged(this, EventArgs.Empty);
				}
			};
		}
		public string Reason {
			get {
				return teReason.Text;
			}
			set {
				teReason.Text = value;
			}
		}
		public string Location {
			get {
				return teLocation.Text;
			}
			set {
				teLocation.Text = value;
			}
		}
		public string ContactInfo {
			get {
				return teContactInformation.Text;
			}
			set {
				teContactInformation.Text = value;
			}
		}
		public ICertificateItem SelectedCertificateItem {
			get {
				return listBoxEdit.EditValue as ICertificateItem;
			}
			set {
				listBoxEdit.EditValue = value;
			}
		}
		public void FillCertificateItems(IEnumerable<ICertificateItem> items) {
			listBoxEdit.ItemsSource = items;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			OkButton.Click += (o, e) => {
				if(Submit != null)
					Submit(this, EventArgs.Empty);
			};
		}
		public void EnableTextEditors(bool enable) {
			teReason.IsEnabled = teLocation.IsEnabled = teContactInformation.IsEnabled = enable;
		}
	}
}
