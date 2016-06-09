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
	public class PdfRichMediaSettings : PdfObject {
		const string richMediaSettingsDictionaryName = "RichMediaSettings";
		const string activationKey = "Activation";
		const string deactivationKey = "Deactivation";
		readonly PdfRichMediaActivation activation;
		readonly PdfRichMediaDeactivation deactivation;
		public PdfRichMediaActivation Activation { get { return activation; } }
		public PdfRichMediaDeactivation Deactivation { get { return deactivation; } }
		internal PdfRichMediaSettings(PdfRichMediaAnnotation annotation, PdfReaderDictionary dictionary) : base (dictionary.Number) {
			if (dictionary != null) {
				string type = dictionary.GetName(PdfDictionary.DictionaryTypeKey);
				if (type != null && type != "RichMediaSettings")
					PdfDocumentReader.ThrowIncorrectDataException();
				PdfReaderDictionary activationDictionary = dictionary.GetDictionary(activationKey);
				if (activationDictionary != null)
					activation = new PdfRichMediaActivation(annotation, activationDictionary);
				PdfReaderDictionary deactivationDictionary = dictionary.GetDictionary(deactivationKey);
				if (deactivationDictionary != null)
					deactivation = new PdfRichMediaDeactivation(deactivationDictionary);
			}
		}
		protected internal override object ToWritableObject(PdfObjectCollection collection) {
			PdfWriterDictionary result = new PdfWriterDictionary(collection);
			result.Add(PdfDictionary.DictionaryTypeKey, new PdfName(richMediaSettingsDictionaryName));
			result.Add(activationKey, activation);
			result.Add(deactivationKey, deactivation);
			return result;
		}
	}
}
