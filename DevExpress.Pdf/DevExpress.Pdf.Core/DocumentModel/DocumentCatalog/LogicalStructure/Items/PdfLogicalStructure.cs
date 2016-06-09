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
	public class PdfLogicalStructureElementList : List<PdfLogicalStructureElement> {}
	public class PdfLogicalStructure : PdfLogicalStructureEntry {
		const string dictionaryType = "StructTreeRoot";
		const string idTreeDictionaryKey = "IDTree";
		const string parentTreeDictionaryKey = "ParentTree";
		const string roleMapDictionaryKey = "RoleMap";
		const string attributeClassMapDictionaryKey = "ClassMap";
		static object WriteParents(PdfObjectCollection collection, List<PdfLogicalStructureElement> parents) {
			return parents.Count == 1 ? (object)collection.AddObject(parents[0]) : (object)new PdfWritableObjectArray(parents, collection);
		}
		readonly IDictionary<string, string> roleMap;
		readonly IDictionary<string, PdfLogicalStructureElementAttribute[]> attributeClassMap;
		PdfDeferredSortedDictionary<string, PdfLogicalStructureItem> elements;
		PdfDeferredSortedDictionary<int, PdfLogicalStructureElementList> parents;
		PdfReaderDictionary idTreeDictionary;
		PdfReaderDictionary parentTreeDictionary;
		internal IDictionary<string, PdfLogicalStructureItem> Elements {
			get {
				Resolve();
				return elements;
			}
		}
		public IDictionary<int, PdfLogicalStructureElementList> Parents {
			get {
				Resolve();
				return parents;
			}
		}
		public IDictionary<string, string> RoleMap { get { return roleMap; } }
		public IDictionary<string, PdfLogicalStructureElementAttribute[]> AttributeClassMap { get { return attributeClassMap; } }
		internal PdfLogicalStructure(PdfReaderDictionary dictionary) : base(null, dictionary) {
			string type = dictionary.GetName(PdfDictionary.DictionaryTypeKey);
			if (type != null && type != dictionaryType)
				PdfDocumentReader.ThrowIncorrectDataException();
			idTreeDictionary = dictionary.GetDictionary(idTreeDictionaryKey);
			parentTreeDictionary = dictionary.GetDictionary(parentTreeDictionaryKey);
			PdfReaderDictionary roleMapDictionary = dictionary.GetDictionary(roleMapDictionaryKey);
			if (roleMapDictionary != null) {
				roleMap = new Dictionary<string, string>();
				foreach (KeyValuePair<string, object> pair in roleMapDictionary) {
					PdfName roleName = pair.Value as PdfName;
					if (roleName == null)
						PdfDocumentReader.ThrowIncorrectDataException();
					roleMap.Add(pair.Key, roleName.Name);
				}
			}
			PdfReaderDictionary attributeClassMapDictionary = dictionary.GetDictionary(attributeClassMapDictionaryKey);
			if (attributeClassMapDictionary != null) {
				PdfObjectCollection collection = dictionary.Objects;
				attributeClassMap = new Dictionary<string, PdfLogicalStructureElementAttribute[]>();
				foreach (KeyValuePair<string, object> pair in attributeClassMapDictionary) {
					string key = pair.Key;
					object value = pair.Value;
					if (key != PdfReaderDictionary.DictionaryTypeKey || !(value is PdfName))
						attributeClassMap.Add(key, PdfLogicalStructureElementAttribute.Parse(collection, value));
				}
			}
		}
		PdfLogicalStructureElementList GetParents(PdfObjectCollection collection, object value) {
			PdfLogicalStructureElementList parents = new PdfLogicalStructureElementList();
			object obj = collection.TryResolve(value);
			PdfReaderDictionary dictionary = obj as PdfReaderDictionary;
			if (dictionary == null) {
				IList<object> valueList = obj as IList<object>;
				if (valueList == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				foreach (object listObject in valueList)
					if (listObject != null) {
						PdfObjectReference reference = listObject as PdfObjectReference;
						if (reference == null)
							PdfDocumentReader.ThrowIncorrectDataException();
						PdfLogicalStructureElement element = collection.GetResolvedObject<PdfLogicalStructureElement>(reference.Number, true);
						if (element != null)
							parents.Add(element);
					}
			}
			else {
				PdfLogicalStructureElement element = collection.GetResolvedObject<PdfLogicalStructureElement>(dictionary.Number, true);
				if (element != null)
					parents.Add(element);
			}
			return parents;
		}
		protected internal override void Resolve() {
			base.Resolve();
			if (idTreeDictionary != null) {
				elements = PdfNameTreeNode<PdfLogicalStructureItem>.Parse(idTreeDictionary, (collection, value) => collection.GetLogicalStructureItem(this, this, value));
				if (elements != null)
					elements.ResolveAll();
				idTreeDictionary = null;
			}
			if (parentTreeDictionary != null) {
				parents = PdfNumberTreeNode<PdfLogicalStructureElementList>.Parse(parentTreeDictionary, (collection, value) => GetParents(collection, value), false);
				if (parents != null)
					parents.ResolveAll();
				parentTreeDictionary = null;
			}
		}
		protected override PdfWriterDictionary CreateDictionary(PdfObjectCollection objects) {
			Resolve();
			PdfWriterDictionary dictionary = new PdfWriterDictionary(objects);
			dictionary.Add(PdfDictionary.DictionaryTypeKey, new PdfName(dictionaryType));
			WriteKids(dictionary, objects);
			dictionary.AddIfPresent(idTreeDictionaryKey, PdfNameTreeNode<PdfLogicalStructureItem>.Write(objects, elements));
			dictionary.AddIfPresent(parentTreeDictionaryKey, PdfNumberTreeNode<PdfLogicalStructureElementList>.Write(objects, parents, (collection, value) => WriteParents(collection, value)));
			if (roleMap != null) {
				PdfWriterDictionary roleMapDictionary = new PdfWriterDictionary(null);
				foreach (KeyValuePair<string, string> pair in roleMap)
					roleMapDictionary.AddName(pair.Key, pair.Value);
				dictionary.Add(roleMapDictionaryKey, roleMapDictionary);
			}
			if (attributeClassMap != null) {
				PdfWriterDictionary attributeClassMapDictionary = new PdfWriterDictionary(objects);
				foreach (KeyValuePair<string, PdfLogicalStructureElementAttribute[]> pair in attributeClassMap) {
					PdfLogicalStructureElementAttribute[] attributes = pair.Value;
					if (attributes == null)
						attributeClassMapDictionary.Add(pair.Key, (object)null);
					else if (attributes.Length == 1)
						attributeClassMapDictionary.Add(pair.Key, attributes[0]);
					else
						attributeClassMapDictionary.Add(pair.Key, new PdfWritableObjectArray(attributes, objects));
				}
				dictionary.Add(attributeClassMapDictionaryKey, attributeClassMapDictionary);
			}
			return dictionary;
		}
	}
}
