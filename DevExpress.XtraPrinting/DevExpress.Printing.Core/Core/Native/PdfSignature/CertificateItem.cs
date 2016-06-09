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
using DevExpress.XtraPrinting.Localization;
namespace DevExpress.XtraPrinting.Native {
	public interface ICertificateItem {
		string Subject { get; }
		string Description { get; }
	}
	public class CertificateItem : ICertificateItem {
		static string IssuerText { get { return PreviewLocalizer.GetString(PreviewStringId.ExportOption_PdfSignature_Issuer); } }
		static string ValidRangeText { get { return PreviewLocalizer.GetString(PreviewStringId.ExportOption_PdfSignature_ValidRange); } }
		public string Subject { get; private set; }
		public string Issuer { get; private set; }
		public DateTime? From { get; private set; }
		public DateTime? To { get; private set; }
		public X509Certificate2 Certificate { get; private set; }
		public CertificateItem(X509Certificate2 cert) {
			this.Subject = GetName(cert.SubjectName);
			this.Issuer = GetName(cert.IssuerName);
			this.From = cert.NotBefore;
			this.To = cert.NotAfter;
			this.Certificate = cert;
		}
		static string GetName(X500DistinguishedName distinguishedName) {
			string[] elements = distinguishedName.Decode(X500DistinguishedNameFlags.UseUTF8Encoding | X500DistinguishedNameFlags.DoNotUseQuotes | X500DistinguishedNameFlags.DoNotUsePlusSign | X500DistinguishedNameFlags.UseNewLines).Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			foreach(string element in elements) {
				if(element.StartsWith("CN="))
					return element.Substring(3);
			}
			return "";
		}
		public string Description {
			get {
				StringBuilder description = new StringBuilder();
				description.Append(IssuerText);
				description.AppendLine(Issuer);
				description.AppendFormat(ValidRangeText, From, To);
				return description.ToString();
			}
		}					  
	}
	public sealed class NoneCertificateItem : ICertificateItem {
		static string NoneText { get { return PreviewLocalizer.GetString(PreviewStringId.ExportOption_PdfSignature_EmptyCertificate); } }
		public static NoneCertificateItem Instance = new NoneCertificateItem();
		NoneCertificateItem() { }
		public string Subject {
			get { return NoneText; }
		}
		public string Description {
			get {
				return null;
			}
		}
	}
}
