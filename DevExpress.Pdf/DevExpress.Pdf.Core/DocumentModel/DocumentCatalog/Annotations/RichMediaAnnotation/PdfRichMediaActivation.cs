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

using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	[PdfDefaultField(PdfRichMediaActivationCondition.Explicitly)]
	public enum PdfRichMediaActivationCondition {
		[PdfFieldName("XA")]
		Explicitly,
		[PdfFieldName("PO")]
		PageBecomeCurrent,
		[PdfFieldName("PV")]
		PageBecomeVisible
	}
	public class PdfRichMediaActivation : PdfObject {
		const string richMediaActivationDictionaryName = "RichMediaActivation";
		const string conditionKey = "Condition";
		const string configurationKey = "Configuration";
		readonly PdfRichMediaActivationCondition condition;
		readonly PdfRichMediaConfiguration configuration;
		public PdfRichMediaActivationCondition Condition { get { return condition; } }
		public PdfRichMediaConfiguration Configuration { get { return configuration; } }
		internal PdfRichMediaActivation(PdfRichMediaAnnotation annotation, PdfReaderDictionary dictionary) : base(dictionary.Number) {
			string type = dictionary.GetName(PdfDictionary.DictionaryTypeKey);
			if (type != null && type != richMediaActivationDictionaryName)
				PdfDocumentReader.ThrowIncorrectDataException();
			condition = PdfEnumToStringConverter.Parse<PdfRichMediaActivationCondition>(dictionary.GetName(conditionKey));
			PdfObjectReference reference = dictionary.GetObjectReference(configurationKey);
			if (reference != null) {
				int number = reference.Number;
				foreach (PdfRichMediaConfiguration item in annotation.Configurations)
					if (item.Number == number) {
						configuration = item;
						break;
					}
				if (configuration == null)
					PdfDocumentReader.ThrowIncorrectDataException();
			}
		}
		protected internal override object ToWritableObject(PdfObjectCollection collection) {
			PdfWriterDictionary result = new PdfWriterDictionary(collection);
			result.Add(PdfDictionary.DictionaryTypeKey, new PdfName(richMediaActivationDictionaryName));
			result.AddEnumName(conditionKey, condition);
			result.Add(configurationKey, configuration);
			return result;
		}
	}
}
