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
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public enum PdfFormSubmissionType {
		[PdfFieldValue(0)]
		None,
		[PdfFieldValue(1)]
		HttpGet,
		[PdfFieldValue(2)]
		HttpPost
	};
	public class PdfSourceInformation : PdfObject {
		const string urlKey = "AU";
		const string timeStampKey = "TS";
		const string expirationStampKey = "E";
		const string submissionTypeKey = "S";
		readonly string url;
		readonly DateTimeOffset? timeStamp;
		readonly DateTimeOffset? expirationStamp;
		readonly PdfFormSubmissionType formSubmissionType;
		public string Url { get { return url; } }
		public DateTimeOffset? TimeStamp { get { return timeStamp; } }
		public DateTimeOffset? ExpirationStamp { get { return expirationStamp; } }
		public PdfFormSubmissionType FormSubmissionType { get { return formSubmissionType; } }
		internal PdfSourceInformation(PdfReaderDictionary dictionary) : base(dictionary.Number) {
			url = dictionary.GetString(urlKey);
			if (String.IsNullOrEmpty(url))
				PdfDocumentReader.ThrowIncorrectDataException();
			timeStamp = dictionary.GetDate(timeStampKey);
			expirationStamp = dictionary.GetDate(expirationStampKey);
			formSubmissionType = PdfEnumToValueConverter.Parse<PdfFormSubmissionType>(dictionary.GetInteger(submissionTypeKey), PdfFormSubmissionType.None);
		}
		protected internal override object ToWritableObject(PdfObjectCollection collection) {
			PdfWriterDictionary result = new PdfWriterDictionary(collection);
			result.Add(urlKey, url, null);
			result.Add(timeStampKey, timeStamp, null);
			result.Add(expirationStampKey, expirationStamp, null);
			if (formSubmissionType != PdfFormSubmissionType.None)
				result.Add(submissionTypeKey, PdfEnumToValueConverter.Convert(formSubmissionType));
			return result;
		}
	}
}
