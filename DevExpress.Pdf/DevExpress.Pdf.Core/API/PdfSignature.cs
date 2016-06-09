#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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
using System.Security.Cryptography.X509Certificates;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public class PdfSignature : PdfObject {
		const string typeDictionary = "Sig";
		const string filterDictionaryKey = "Filter";
		const string subFilterDictionaryKey = "SubFilter";
		const string contentsDictionaryKey = "Contents";
		const string certDictionaryKey = "Cert";
		const string byteRangeDictionaryKey = "ByteRange";
		const string nameDictionaryKey = "Name";
		const string signingTimeDictionaryKey = "M";
		const string locationDictionaryKey = "Location";
		const string reasonDictionaryKey = "Reason";
		const string contactInfoDictionaryKey = "ContactInfo";
		const string adobeFilter = "Adobe.PPKMS";
		const string adobeSubFilter = "adbe.pkcs7.sha1";
		readonly string filter = adobeFilter;
		readonly string subFilter = adobeSubFilter;
		readonly X509Certificate2 certificate;
		string name;
		string location;
		string reason;
		string contactInfo;
		public string Filter { get { return filter; } }
		public string SubFilter { get { return subFilter; } }
		public string Name {
			get { return name; }
			set { name = value; }
		}
		public string Location {
			get { return location; }
			set { location = value; }
		}
		public string Reason {
			get { return reason; }
			set { reason = value; }
		}
		public string ContactInfo {
			get { return contactInfo; }
			set { contactInfo = value; }
		}
		public PdfSignature(X509Certificate2 certificate) {
			if (certificate == null)
				throw new ArgumentNullException("certificate");
			this.certificate = certificate;
		}
		protected internal override object ToWritableObject(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(objects);
			dictionary.AddName("Type", "Sig");
			dictionary.AddName("Filter", "Adobe.PPKMS");
			dictionary.AddName("SubFilter", "adbe.pkcs7.sha1");
			if (!string.IsNullOrEmpty(Reason))
				dictionary.AddASCIIString("Reason", Reason);
			if (!string.IsNullOrEmpty(Location))
				dictionary.AddASCIIString("Location", Location);
			if (!string.IsNullOrEmpty(ContactInfo))
				dictionary.AddASCIIString("ContactInfo", ContactInfo);
			dictionary.Add("M", DateTimeOffset.Now);
			PdfSignatureContents contents = new PdfSignatureContents(certificate);
			dictionary.Add("Contents", contents);
			dictionary.Add("ByteRange", new PdfSignatureByteRange(contents));
			return dictionary;
		}
	}
}
