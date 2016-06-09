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
	[Flags]
	public enum PdfSignatureFlags { None = 0, SignaturesExist = 1, AppendOnly = 2 };
	public class PdfInteractiveForm : PdfObject {
		const string fieldsKey = "Fields";
		const string resourceKey = "DR";
		const string needAppearancesKey = "NeedAppearances";
		const string signatureFlagsKey = "SigFlags";
		const string xfaKey = "XFA";
		const string calculationOrderKey = "CO";
		readonly List<PdfInteractiveFormField> fields = new List<PdfInteractiveFormField>();
		readonly bool needAppearances;
		readonly PdfCommandList defaultAppearanceCommands;
		readonly PdfTextJustification defaultTextJustification;
		readonly PdfXFAForm xfaForm;
		readonly IList<PdfInteractiveFormField> calculationOrder;
		PdfSignatureFlags signatureFlags;
		PdfResources resources;
		bool modified;
		public IList<PdfInteractiveFormField> Fields { get { return fields; } }
		public bool NeedAppearances { get { return needAppearances; } }
		public IEnumerable<PdfCommand> DefaultAppearanceCommands { get { return defaultAppearanceCommands; } }
		public PdfTextJustification DefaultTextJustification { get { return defaultTextJustification; } }
		public PdfXFAForm XFAForm { get { return xfaForm; } }
		public PdfSignatureFlags SignatureFlags {
			get { return signatureFlags; }
			internal set { signatureFlags = value; }
		}
		internal IList<PdfInteractiveFormField> CalculationOrder { get { return calculationOrder; } }
		internal PdfResources Resources { get { return resources; } }
		internal bool Modified {
			get { return modified; }
			set { modified = value; }
		}
		internal event PdfInteractiveFormFieldValueChangingEventHandler FormFieldValueChanging;
		internal event PdfInteractiveFormFieldValueChangedEventHandler FormFieldValueChanged;
		internal PdfInteractiveForm(PdfReaderDictionary dictionary) : base(dictionary.Number) {
			PdfObjectCollection objects = dictionary.Objects;
			PdfDocumentCatalog documentCatalog = objects.DocumentCatalog;
			resources = dictionary.GetResources(resourceKey, null, false);
			IList<object> fieldsArray = dictionary.GetArray(fieldsKey);
			if (fieldsArray != null)
				foreach (object value in fieldsArray)
					fields.Add(objects.GetInteractiveFormField(this, null, value));
			calculationOrder = dictionary.GetArray<PdfInteractiveFormField>(calculationOrderKey, o => objects.GetInteractiveFormField(this, null, o));
			needAppearances = dictionary.GetBoolean(needAppearancesKey) ?? false;
			signatureFlags = (PdfSignatureFlags)(dictionary.GetInteger(signatureFlagsKey) ?? 0);
			defaultAppearanceCommands = dictionary.GetAppearance(resources);
			defaultTextJustification = dictionary.GetTextJustification();
			object xfa;
			if (dictionary.TryGetValue(xfaKey, out xfa)) {
				xfa = objects.TryResolve(xfa);
				IList<object> xfaArray = xfa as IList<object>;
				if (xfaArray == null) {
					PdfReaderStream xfaStream = xfa as PdfReaderStream;
					if (xfaStream == null)
						PdfDocumentReader.ThrowIncorrectDataException();
					xfaForm = new PdfXFAForm(xfaStream.GetData(true));
				}
				else
					xfaForm = new PdfXFAForm(objects, xfaArray);
			}
		}
		internal void AddInteractiveFormField(PdfInteractiveFormField formField) {
			if (formField.Parent != null)
				AddInteractiveFormField(formField.Parent);
			else if (!fields.Contains(formField))
				fields.Add(formField);
		}
		internal bool RaiseFormFieldValueChanging(PdfInteractiveFormFieldValueChangingEventArgs args) {
			if (FormFieldValueChanging != null) {
				FormFieldValueChanging(this, args);
				return !args.Cancel;
			}
			return true;
		}
		internal void RaiseFormFieldValueChanged(string fieldName, object oldValue, object newValue) {
			if (FormFieldValueChanged != null)
				FormFieldValueChanged(this, new PdfInteractiveFormFieldValueChangedEventArgs(fieldName, oldValue, newValue));
		}
		protected internal override object ToWritableObject(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(objects);
			dictionary.Add(resourceKey, resources);
			dictionary.Add(needAppearancesKey, needAppearances, false);
			dictionary.Add(signatureFlagsKey, (int)signatureFlags, 0);
			dictionary.AddList(fieldsKey, fields);
			if (defaultAppearanceCommands != null)
				dictionary.Add(PdfDictionary.DictionaryAppearanceKey, defaultAppearanceCommands.ToByteArray(resources));
			dictionary.Add(PdfDictionary.DictionaryJustificationKey, PdfEnumToValueConverter.Convert(defaultTextJustification));
			if (xfaForm != null)
				dictionary.Add(xfaKey, xfaForm.Write());
			if (calculationOrder != null)
				dictionary.Add(calculationOrderKey, new PdfWritableObjectArray(calculationOrder, objects));
			return dictionary;
		}
	}
}
