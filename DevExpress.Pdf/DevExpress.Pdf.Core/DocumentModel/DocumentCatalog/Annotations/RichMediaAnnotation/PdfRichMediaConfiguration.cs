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
	public class PdfRichMediaConfiguration : PdfObject {
		const string configurationDictionaryName = "RichMediaConfiguration";
		const string nameKey = "Name";
		const string instancesKey = "Instances";
		readonly int number;
		readonly string name;
		readonly List<PdfRichMediaInstance> instances = new List<PdfRichMediaInstance>();
		readonly PdfRichMediaContentType contentType;
		internal int Number { get { return number; } }
		public string Name { get { return name; } }
		public IList<PdfRichMediaInstance> Instances { get { return instances; } }
		public PdfRichMediaContentType ContentType { get { return contentType; } }
		internal PdfRichMediaConfiguration(IDictionary<string, PdfFileSpecification> assets, PdfReaderDictionary dictionary) : base (dictionary.Number) {
			string type = dictionary.GetName(PdfDictionary.DictionaryTypeKey);
			if (type != null && type != configurationDictionaryName)
				PdfDocumentReader.ThrowIncorrectDataException();
			number = dictionary.Number;
			name = dictionary.GetString(nameKey);
			IList<object> instancesArray = dictionary.GetArray(instancesKey);
			if (instancesArray != null) {
				PdfObjectCollection collection = dictionary.Objects;
				foreach (object value in instancesArray) {
					PdfObjectReference reference = value as PdfObjectReference;
					if (reference == null)
						PdfDocumentReader.ThrowIncorrectDataException();
					PdfReaderDictionary instanceDictionary = collection.GetDictionary(reference.Number);
					if (instanceDictionary == null)
						PdfDocumentReader.ThrowIncorrectDataException();
					instances.Add(new PdfRichMediaInstance(assets, instanceDictionary));
				}
			}
			PdfRichMediaContentType? contentTypeValue = dictionary.GetRichMediaContentType();
			if (contentTypeValue.HasValue)
				contentType = contentTypeValue.Value;
			else if (instances.Count > 0)
				contentType = instances[0].ContentType;
		}
		protected internal override object ToWritableObject(PdfObjectCollection collection) {
			PdfWriterDictionary result = new PdfWriterDictionary(collection);
			result.Add(PdfDictionary.DictionaryTypeKey, new PdfName(configurationDictionaryName));
			result.AddEnumName(PdfDictionary.DictionarySubtypeKey, contentType);
			result.Add(nameKey, name, null);
			result.AddList(instancesKey, instances);
			return result;
		}
	}
}
