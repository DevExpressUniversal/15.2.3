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
using System.Collections.Generic;
using DevExpress.Pdf.Native;
using DevExpress.Utils;
namespace DevExpress.Pdf {
	public class PdfSignatureFormField : PdfInteractiveFormField {
		internal const string Type = "Sig";
		const string signatureDictionaryKey = "V";
		static string GenerateFieldName(PdfDocumentCatalog catalog) {
			string fieldName = Guid.NewGuid().ToString();
			PdfInteractiveForm acroForm = catalog.AcroForm;
			if (acroForm != null && acroForm.Fields != null) {
				while (true) {
					bool collision = false;
					foreach (PdfInteractiveFormField field in acroForm.Fields) {
						if (field.Name != null && field.Equals(fieldName)) {
							collision = true;
							break;
						}
					}
					if (!collision)
						return fieldName;
					fieldName = Guid.NewGuid().ToString();
				}
			}
			return fieldName;
		}
		internal static void CreateSignatureFormField(PdfDocumentCatalog catalog, PdfSignature signature) {
			PdfObjectCollection objects = catalog.Objects;
			IList<PdfPage> pages = catalog.Pages;
			if (pages != null && pages.Count > 0) {
				PdfReaderDictionary dictionary = new PdfReaderDictionary(objects, objects.LastObjectNumber + 1, 0);
				objects.AddItem(new PdfObjectContainer(objects.LastObjectNumber + 1, 0, dictionary), false);
				dictionary.Add(PdfDictionary.DictionaryTypeKey, new PdfName(PdfAnnotation.DictionaryType));
				dictionary.Add(PdfDictionary.DictionarySubtypeKey, new PdfName(PdfWidgetAnnotation.Type));
				dictionary.Add("FT", new PdfName(PdfSignatureFormField.Type));
				dictionary.Add("F", (int)(PdfAnnotationFlags.Print | PdfAnnotationFlags.Locked));
				dictionary.Add("T", DXEncoding.GetEncoding("Windows-1251").GetBytes(GenerateFieldName(catalog)));
				PdfPage firstPage = pages[0];
				dictionary.Add("P", new PdfObjectReference(firstPage.ObjectNumber));
				dictionary.Add("Rect", new object[] { 0, 0, 0, 0 });
				dictionary.Add("V", signature);
				PdfObjectReference dictionaryReference = new PdfObjectReference(dictionary.Number);
				PdfWidgetAnnotation widget = objects.GetAnnotation(firstPage, dictionaryReference) as PdfWidgetAnnotation;
				if (widget == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				firstPage.Annotations.Add(widget);
				PdfSignatureFormField field = new PdfSignatureFormField(catalog.AcroForm, dictionary, signature, dictionaryReference);
				catalog.AddInteractiveFormField(field);
				catalog.AcroForm.SignatureFlags = PdfSignatureFlags.AppendOnly | PdfSignatureFlags.SignaturesExist;
			}
		}
		readonly PdfSignature signature;
		protected override string FieldType { get { return Type; } }
		protected internal override bool ShouldHighlight { get { return false; } }
		PdfSignatureFormField(PdfInteractiveForm form, PdfReaderDictionary dictionary, PdfSignature signature, PdfObjectReference reference) : base(form, null, dictionary, reference) {
			this.signature = signature;
		}
		internal PdfSignatureFormField(PdfInteractiveForm form, PdfInteractiveFormField parent, PdfReaderDictionary dictionary, PdfObjectReference valueReference) : base(form, parent, dictionary, valueReference) {
		}
		protected internal override void FillDictionary(PdfWriterDictionary dictionary) {
			base.FillDictionary(dictionary);
			dictionary.Add("V", signature);
		}
	}
}
