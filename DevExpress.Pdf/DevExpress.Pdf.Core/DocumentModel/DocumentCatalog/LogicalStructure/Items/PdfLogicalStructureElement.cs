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

using System.Globalization;
using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public class PdfLogicalStructureElement : PdfLogicalStructureEntry {
		const string dictionaryType = "StructElem";
		const string structureTypeDictionaryKey = "S";
		const string parentDictionaryKey = "P";
		const string idDictionaryKey = "ID";
		const string pageDictionaryKey = "Pg";
		const string attributesDictionaryKey = "A";
		const string attributeClassesDictionaryKey = "C";
		const string revisionNumberDictionaryKey = "R";
		const string titleDictionaryKey = "T";
		const string alternateDescriptionDictionaryKey = "Alt";
		const string abbreviationDictionaryKey = "E";
		const string actualTextDictionaryKey = "ActualText";
		readonly PdfLogicalStructureEntry parent;
		readonly string structureType;
		readonly int? structureTypeObject;
		readonly byte[] id;
		readonly PdfPage page;
		readonly IList<PdfLogicalStructureElementAttribute> attributes = null;
		readonly IList<string> attributeClasses;
		readonly int revisionNumber;
		readonly string title;
		readonly CultureInfo languageCulture;
		readonly string alternateDescription;
		readonly string abbreviation;
		readonly string actualText;
		public PdfLogicalStructureEntry Parent { get { return parent; } }
		public string StructureType { get { return structureType; } }
		public byte[] ID { get { return id; } }
		public PdfPage Page { get { return page; } }
		public IEnumerable<PdfLogicalStructureElementAttribute> Attributes { get { return attributes; } }
		public IList<string> AttributeClasses { get { return attributeClasses; } }
		public int RevisionNumber { get { return revisionNumber; } }
		public string Title { get { return title; } }
		public CultureInfo LanguageCulture { get { return languageCulture; } }
		public string AlternateDescription { get { return alternateDescription; } }
		public string Abbreviation { get { return abbreviation; } }
		public string ActualText { get { return actualText; } }
		protected internal override PdfPage ContainingPage { get { return page; } }
		internal PdfLogicalStructureElement(PdfLogicalStructure logicalStructure, PdfLogicalStructureEntry parent, PdfReaderDictionary dictionary) : base(logicalStructure, dictionary) {
			this.parent = parent;
			object value;
			if (!dictionary.TryGetValue(structureTypeDictionaryKey, out value))
				PdfDocumentReader.ThrowIncorrectDataException();
			value = dictionary.Objects.TryResolve(value);
			PdfName structureTypeName = value as PdfName;
			if (structureTypeName != null)
				structureType = structureTypeName.Name;
			else 
				structureTypeObject = value as int?;
			string name = dictionary.GetName(PdfDictionary.DictionaryTypeKey);
			if ((name != null && name != dictionaryType) || (structureType == null && !structureTypeObject.HasValue))
				PdfDocumentReader.ThrowIncorrectDataException();
			id = dictionary.GetBytes(idDictionaryKey);
			PdfObjectCollection objects = dictionary.Objects;
			PdfObjectReference pageReference = dictionary.GetObjectReference(pageDictionaryKey);
			if (pageReference != null)
				page = logicalStructure.DocumentCatalog.FindPage(pageReference);
			if (dictionary.TryGetValue(attributesDictionaryKey, out value))
				attributes = PdfLogicalStructureElementAttribute.Parse(objects, value);
			if (dictionary.TryGetValue(attributeClassesDictionaryKey, out value)) {
				value = objects.TryResolve(value);
				PdfName attributeClass = value as PdfName;
				if (attributeClass == null) {
					IList<object> list = value as IList<object>;
					if (list == null)
						PdfDocumentReader.ThrowIncorrectDataException();
					attributeClasses = new List<string>(list.Count);
					foreach (object obj in list) {
						attributeClass = obj as PdfName;
						if (attributeClass == null)
							PdfDocumentReader.ThrowIncorrectDataException();
						attributeClasses.Add(attributeClass.Name);
					}
				}
				else
					attributeClasses = new List<string> { attributeClass.Name };
			}
			revisionNumber = dictionary.GetInteger(revisionNumberDictionaryKey) ?? 0;
			title = dictionary.GetString(titleDictionaryKey);
			languageCulture = dictionary.GetLanguageCulture();
			alternateDescription = dictionary.GetString(alternateDescriptionDictionaryKey);
			abbreviation = dictionary.GetString(abbreviationDictionaryKey);
			actualText = dictionary.GetString(actualTextDictionaryKey);
		}
		protected override PdfWriterDictionary CreateDictionary(PdfObjectCollection collection) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(collection);
			if (structureTypeObject.HasValue)
				dictionary.Add(structureTypeDictionaryKey, structureTypeObject.Value);
			else
				dictionary.AddName(structureTypeDictionaryKey, structureType);
			dictionary.Add(parentDictionaryKey, parent);
			dictionary.AddIfPresent(idDictionaryKey, id);
			dictionary.Add(pageDictionaryKey, page);
			WriteKids(dictionary, collection);
			if (attributes != null)
				dictionary.Add(attributesDictionaryKey, new PdfWritableObjectArray(attributes, collection));
			if (attributeClasses != null)
				if (attributeClasses.Count == 1)
					dictionary.AddName(attributeClassesDictionaryKey, attributeClasses[0]);
				else
					dictionary.Add(attributeClassesDictionaryKey, new PdfWritableConvertableArray<string>(attributeClasses, o => new PdfName(o)));
			dictionary.Add(revisionNumberDictionaryKey, revisionNumber, 0);
			dictionary.AddIfPresent(titleDictionaryKey, title);
			dictionary.AddLanguage(languageCulture);
			dictionary.AddIfPresent(alternateDescriptionDictionaryKey, alternateDescription);
			dictionary.AddNotNullOrEmptyString(abbreviationDictionaryKey, abbreviation);
			dictionary.AddNotNullOrEmptyString(actualTextDictionaryKey, actualText);
			return dictionary;
		}
	}
}
