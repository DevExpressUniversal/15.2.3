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
using System.Linq;
using System.Text;
using System.Security.Cryptography.X509Certificates;
namespace DevExpress.XtraPrinting.Native {
	public class PdfSignatureEditorPresenter {
		readonly IPdfSignatureEditorView view;
		readonly PdfSignatureOptions options;
		public PdfSignatureEditorPresenter(PdfSignatureOptions options, IPdfSignatureEditorView view) {
			this.view = view;
			this.options = options;
			InitializeCertificateItems();
			view.Reason = options.Reason;
			view.ContactInfo = options.ContactInfo;
			view.Location = options.Location;
			view.SelectedCertificateItemChanged += (o, e) => {
				InvalidateTextEditorsisEnabled();
			};
			InvalidateTextEditorsisEnabled();
			view.Submit += (o, e) => OnSubmit();
		}
		void InitializeCertificateItems() {
			List<ICertificateItem> certificateItems = new List<ICertificateItem>(
				SelectCertificates().ConvertAll<ICertificateItem>(x => new CertificateItem((X509Certificate2)x))
			);
			certificateItems.Insert(0, NoneCertificateItem.Instance);
			view.FillCertificateItems(certificateItems);
			view.SelectedCertificateItem = options.Certificate != null ? FindCertificateItem(certificateItems, options.Certificate) : NoneCertificateItem.Instance;
		}
		static ICertificateItem FindCertificateItem(List<ICertificateItem> certificateItems, X509Certificate2 certificate) {
			foreach(var certificateItem in certificateItems) {
				if(certificateItem is CertificateItem && object.Equals(certificate, ((CertificateItem)certificateItem).Certificate))
					return certificateItem;
			}
			return NoneCertificateItem.Instance;
		}
		static X509Certificate2Collection SelectCertificates() {
			X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
			store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
			X509Certificate2Collection collection = store.Certificates;
			collection = collection.Find(X509FindType.FindByTimeValid, DateTime.Now, true);
			collection = collection.Find(X509FindType.FindByKeyUsage, X509KeyUsageFlags.DigitalSignature, true);
			return collection;
		}
		void InvalidateTextEditorsisEnabled() {
			view.EnableTextEditors(view.SelectedCertificateItem != NoneCertificateItem.Instance);
		}
		void OnSubmit() {
			if(view.SelectedCertificateItem == NoneCertificateItem.Instance) {
				options.Certificate = null;
				return;
			}
			CertificateItem selectedCertificateItem = (CertificateItem)view.SelectedCertificateItem;
			options.Certificate = selectedCertificateItem.Certificate;
			options.Reason = view.Reason;
			options.Location = view.Location;
			options.ContactInfo = view.ContactInfo;
		}
	}
}
