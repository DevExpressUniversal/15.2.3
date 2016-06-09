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

using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public class PdfAnnotationAppearance {
		internal static PdfAnnotationAppearance Parse(PdfReaderDictionary dictionary, string key) {
			object value;
			if (!dictionary.TryGetValue(key, out value))
				return null;
			PdfObjectCollection objects = dictionary.Objects;
			value = objects.TryResolve(value);
			return value == null ? null : new PdfAnnotationAppearance(objects, value);
		}
		readonly PdfForm defaultForm;
		readonly Dictionary<string, PdfForm> forms;
		public PdfForm DefaultForm { get { return defaultForm; } }
		public IDictionary<string, PdfForm> Forms { get { return forms; } }
		PdfAnnotationAppearance(PdfObjectCollection objects, object value) {
			PdfReaderStream stream = value as PdfReaderStream;
			if (stream == null) {
				PdfReaderDictionary dictionary = value as PdfReaderDictionary;
				if (dictionary == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				forms = new Dictionary<string, PdfForm>();
				foreach (KeyValuePair<string, object> pair in dictionary) {
					PdfObjectReference reference = pair.Value as PdfObjectReference;
					if (reference == null)
						PdfDocumentReader.ThrowIncorrectDataException();
					PdfForm form = objects.GetForm(reference.Number);
					if (form == null)
						PdfDocumentReader.ThrowIncorrectDataException();
					string name = pair.Key;
					forms.Add(name, form);
					if (name == "On" || (defaultForm == null && name != PdfButtonFormField.OffStateName))
						defaultForm = form;
				}
			}
			else {
				defaultForm = objects.GetForm(stream.Dictionary.Number);
				if (defaultForm == null)
					PdfDocumentReader.ThrowIncorrectDataException();
			}
		}
		internal PdfAnnotationAppearance(PdfDocumentCatalog documentCatalog, PdfRectangle bBox) {
			defaultForm = new PdfForm(documentCatalog, bBox);
		}
		internal void UpdateDefaultFormTransformation(PdfRectangle bBox, PdfTransformationMatrix matrix) {
			defaultForm.BBox = bBox;
			defaultForm.Matrix = matrix;
		}
		internal List<string> GetNames(string defaultName) {
			return forms == null ? new List<string>() { defaultName } : new List<string>(forms.Keys);
		}
		internal object ToWritableObject(PdfObjectCollection collection) {
			return forms == null ? (object)collection.AddObject(defaultForm) : (object)PdfElementsDictionaryWriter.Write(forms, value => collection.AddObject((PdfObject)value));
		}
	}
}
