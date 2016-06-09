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
	[PdfDefaultField(PdfMarkupAnnotationReplyType.Reply)]
	public enum PdfMarkupAnnotationReplyType { 
		[PdfFieldName("R")]
		Reply, 
		Group 
	}
	public abstract class PdfMarkupAnnotation : PdfAnnotation {
		const string titleDictionaryKey = "T";
		const string popupDictionaryKey = "Popup";
		const string opacityDictionaryKey = "CA";
		const string richTextDictionaryKey = "RC";
		const string creationDateDictionaryKey = "CreationDate";
		const string inReplyToDictionaryKey = "IRT";
		const string subjectDictionaryKey = "Subj";
		const string replyTypeDictionaryKey = "RT";
		const string intentDictionaryKey = "IT";
		const double defaultOpacity = 1.0;
		readonly string title;
		readonly double opacity;
		readonly string richTextData;
		readonly DateTimeOffset? creationDate;
		readonly string subject;
		readonly string intent;
		readonly PdfMarkupAnnotationReplyType replyType;
		readonly int popupAnnotationNumber;
		readonly int inReplyToAnnotationNumber;
		PdfPopupAnnotation popup;
		PdfAnnotation inReplyTo;
		public string Title { get { return title; } }
		public double Opacity { get { return opacity; } }
		public string RichTextData { get { return richTextData; } }
		public DateTimeOffset? CreationDate { get { return creationDate; } }
		public string Subject { get { return subject; } }
		public string Intent { get { return intent; } }
		public PdfMarkupAnnotationReplyType ReplyType { get { return replyType; } }
		public PdfPopupAnnotation Popup { 
			get {
				if (popup == null && popupAnnotationNumber != 0) {
					popup = Find(popupAnnotationNumber) as PdfPopupAnnotation;
					if (popup == null)
						PdfDocumentReader.ThrowIncorrectDataException();
				}
				return popup; 
			}
		}
		public PdfAnnotation InReplyTo { 
			get {
				if (inReplyTo == null && inReplyToAnnotationNumber != 0) {
					inReplyTo = Find(inReplyToAnnotationNumber);
					if (inReplyTo == null)
						PdfDocumentReader.ThrowIncorrectDataException();
				}
				return inReplyTo; 
			}
		}
		protected PdfMarkupAnnotation(PdfPage page, PdfReaderDictionary dictionary) : base(page, dictionary) {
			title = dictionary.GetString(titleDictionaryKey);
			object value;
			if (dictionary.TryGetValue(popupDictionaryKey, out value) && value != null) {
				PdfObjectReference popupReference = value as PdfObjectReference;
				if (popupReference == null) {
					PdfReaderDictionary popupDictionary = value as PdfReaderDictionary;
					if (popupDictionary == null)
						PdfDocumentReader.ThrowIncorrectDataException();
					popup = new PdfPopupAnnotation(page, popupDictionary);
					if (popup == null)
						PdfDocumentReader.ThrowIncorrectDataException();
					page.Annotations.Add(popup);
				}
				else
					popupAnnotationNumber = popupReference.Number;
			}
			opacity = dictionary.GetNumber(opacityDictionaryKey) ?? defaultOpacity;
			if (opacity < 0.0 || opacity > 1.0)
				PdfDocumentReader.ThrowIncorrectDataException();
			richTextData = dictionary.GetStringAdvanced(richTextDictionaryKey);
			creationDate = dictionary.GetDate(creationDateDictionaryKey);
			PdfObjectReference reference = dictionary.GetObjectReference(inReplyToDictionaryKey);
			if (reference != null) {
				inReplyToAnnotationNumber = reference.Number;
				replyType = dictionary.GetEnumName<PdfMarkupAnnotationReplyType>(replyTypeDictionaryKey);
			}
			subject = dictionary.GetString(subjectDictionaryKey);
			intent = dictionary.GetName(intentDictionaryKey);
		}
		PdfAnnotation Find(int number) {
			foreach (PdfAnnotation annotation in Page.Annotations)
				if (annotation.ObjectNumber == number)
					return annotation;
			return null;
		}
		protected override PdfWriterDictionary CreateDictionary(PdfObjectCollection collection) {
			PdfWriterDictionary dictionary = base.CreateDictionary(collection);
			dictionary.AddIfPresent(titleDictionaryKey, title);
			dictionary.Add(popupDictionaryKey, Popup);
			dictionary.Add(opacityDictionaryKey, opacity, defaultOpacity);
			dictionary.AddNotNullOrEmptyString(richTextDictionaryKey, richTextData);
			dictionary.AddIfPresent(creationDateDictionaryKey, creationDate);
			if (InReplyTo != null) {
				dictionary.Add(inReplyToDictionaryKey, inReplyTo);
				dictionary.AddEnumName<PdfMarkupAnnotationReplyType>(replyTypeDictionaryKey, replyType);
			}
			dictionary.AddNotNullOrEmptyString(subjectDictionaryKey, subject);
			dictionary.AddName(intentDictionaryKey, intent);
			return dictionary;
		}
	}
}
