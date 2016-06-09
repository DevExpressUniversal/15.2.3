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
namespace DevExpress.Pdf {
	public class PdfResetFormAction : PdfAction {
		internal const string Name = "ResetForm";
		const string fieldsDictionaryKey = "Fields";
		const string flagsDictionaryKey = "Flags";
		const int excludeFieldsValue = 1;
		const int defaultFlag = 0;
		static PdfInteractiveFormField FindFieldByName(string fullName, IList<PdfInteractiveFormField> list) {
			if (list != null)
				foreach (PdfInteractiveFormField field in list) {
					if (field.FullName == fullName)
						return field;
					if (field.Kids != null) {
						PdfInteractiveFormField foundField = FindFieldByName(fullName, field.Kids);
						if (foundField != null)
							return foundField;
					}
				}
			return null;
		}
		readonly int flags;
		List<PdfInteractiveFormField> fields;
		IList<object> formFieldObjects;
		public bool ExcludeFields { get { return (flags & excludeFieldsValue) == excludeFieldsValue; } }
		public IEnumerable<PdfInteractiveFormField> Fields {
			get {
				FillFormFields();
				return fields;
			}
		}
		protected override string ActionType { get { return Name; } }
		internal PdfResetFormAction(PdfReaderDictionary dictionary) : base(dictionary) {
			formFieldObjects = dictionary.GetArray(fieldsDictionaryKey);
			if (formFieldObjects != null)
				fields = new List<PdfInteractiveFormField>(formFieldObjects.Count);
			flags = dictionary.GetInteger(flagsDictionaryKey) ?? defaultFlag;
			if (flags < 0)
				PdfDocumentReader.ThrowIncorrectDataException();
		}
		protected internal override void Execute(IPdfInteractiveOperationController interactiveOperationController, IList<PdfPage> pages) {
			FillFormFields();
			if (fields == null)
				interactiveOperationController.ResetForm();
			else
				if (ExcludeFields)
					interactiveOperationController.ResetFormExcludingFields(fields);
				else
					interactiveOperationController.ResetFields(fields);
		}
		void FillFormFields() {
			if (formFieldObjects != null) {
				PdfDocumentCatalog documentCatalog = DocumentCatalog;
				foreach (object value in formFieldObjects) {
					PdfInteractiveFormField formField = null;
					PdfObjectReference reference = value as PdfObjectReference;
					PdfObjectCollection objects = documentCatalog.Objects;
					if (reference != null)
						formField = objects.GetResolvedInteractiveFormField(reference);
					else {
						string fullName = PdfDocumentReader.ConvertToString(objects.TryResolve(value) as byte[]);
						if (!String.IsNullOrEmpty(fullName) && documentCatalog.AcroForm != null)
							formField = FindFieldByName(fullName, documentCatalog.AcroForm.Fields);
					}
					if (formField != null)
						fields.Add(formField);
				}
				if (fields.Count == 0)
					fields = null;
				formFieldObjects = null;
			}
		}
		protected override PdfWriterDictionary CreateDictionary(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = base.CreateDictionary(objects);
			FillFormFields();
			dictionary.AddList(fieldsDictionaryKey, fields);
			dictionary.Add(flagsDictionaryKey, flags, defaultFlag);
			return dictionary;
		}
	}
}
