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
using System.Text;
using System.IO;
using System.Collections.Generic;
using DevExpress.Pdf.Localization;
namespace DevExpress.Pdf.Native {
	public class PdfObjectCollection {
		const string fieldNamePrefix = "Field";
		internal const string TrailerSizeKey = "Size";
		internal const string TrailerInfoKey = "Info";
		internal const string TrailerRootKey = "Root";
		readonly Guid id = Guid.NewGuid();
		readonly IDictionary<int, PdfDocumentItem> collection = new Dictionary<int, PdfDocumentItem>();
		readonly Dictionary<int, WeakReference> resolvedObjects = new Dictionary<int, WeakReference>();
		readonly Dictionary<int, PdfInteractiveFormField> resolvedInteractiveFormField = new Dictionary<int, PdfInteractiveFormField>();
		readonly Dictionary<int, PdfAnnotationAppearances> resolvedAppearances = new Dictionary<int, PdfAnnotationAppearances>();
		readonly Dictionary<int, PdfPageResources> resolvedPageResources = new Dictionary<int, PdfPageResources>();
		readonly List<PdfDocumentItem> savedReferences = new List<PdfDocumentItem>();
		readonly Dictionary<int, Dictionary<Guid, int>> writtenObjectsNumber = new Dictionary<int, Dictionary<Guid, int>>();
		readonly Dictionary<string, string> renamedDestinations = new Dictionary<string, string>();
		readonly Dictionary<string, string> renamedFormField = new Dictionary<string, string>();
		HashSet<string> savedDestinationNames = null;
		Dictionary<PdfObject, int> deferredSavedObjects = new Dictionary<PdfObject, int>();
		Dictionary<PdfObject, int> writtenObjects = new Dictionary<PdfObject, int>();
		Guid foreignCollectionId;
		Func<PdfObjectContainer, PdfObjectSlot> writeIndirectObject;
		PdfDocumentStream documentStream;
		PdfDocumentCatalog documentCatalog;
		PdfEncryptionInfo encryptionInfo;
		int lastObjectNumber;
		bool forceUpdateNumbers = false;
		bool? isStreamDetached = null;
		public event EventHandler ElementWriting;
		public int WrittenObjectsCount { get { return writtenObjectsNumber.Count; } }
		public int Count { get { return collection.Count; } }
		public PdfEncryptionInfo EncryptionInfo {
			get { return encryptionInfo; }
			set {
				encryptionInfo = value;
				if (documentStream != null)
					documentStream.EncryptionInfo = value;
			}
		}
		public bool IsStreamDetached { 
			get { return isStreamDetached.GetValueOrDefault(documentStream == null); } 
			internal set { isStreamDetached = value; } 
		}
		public PdfDocumentCatalog DocumentCatalog {
			get { return documentCatalog; }
			set {
				int lastNumber = LastObjectNumber;
				documentCatalog = value;
				documentCatalog.LastObjectNumber = lastNumber;
				foreach (KeyValuePair<PdfObject, int> kvp in documentCatalog.Objects.deferredSavedObjects)
					deferredSavedObjects[kvp.Key] = kvp.Value;
			}
		}
		internal int LastObjectNumber {
			get {
				return documentCatalog == null ? lastObjectNumber : documentCatalog.LastObjectNumber;
			}
			set {
				if (documentCatalog == null)
					lastObjectNumber = Math.Max(lastObjectNumber, value);
				else
					documentCatalog.LastObjectNumber = value;
			}
		}
		internal IEnumerator<PdfObjectContainer> EnumeratorOfContainers {
			get {
				List<int> keys = new List<int>(collection.Keys);
				foreach (int key in keys) {
					PdfDocumentItem item = collection[key];
					if (item is PdfObjectContainer)
						yield return (PdfObjectContainer)item;
				}
			}
		}
		internal Guid Id { get { return id; } }
		public PdfObjectCollection(PdfDocumentStream stream) {
			this.documentStream = stream;
		}
		public PdfObjectCollection(PdfDocumentStream stream, Func<PdfObjectContainer, PdfObjectSlot> writeIndirectObject)
			: this(stream) {
			this.writeIndirectObject = writeIndirectObject;
		}
		public void PrepareToWrite(PdfDocumentCatalog catalog) {
			if (catalog != null) {
				PdfObjectCollection objects = catalog.Objects;
				LastObjectNumber = objects.LastObjectNumber;
				DocumentCatalog = catalog;
			}
		}
		public int CloneObject(PdfObject value, Guid foreignCollectionId) {
			PdfObjectReference reference = AddCloneObject(value.ObjectNumber, () => value, foreignCollectionId);
			return reference == null ? PdfObject.DirectObjectNumber : reference.Number;
		}
		public void AddSavedDestinationName(string name) {
			if (savedDestinationNames != null && !String.IsNullOrEmpty(name) && !savedDestinationNames.Contains(name))
				savedDestinationNames.Add(name);
		}
		public IList<PdfPage> ClonePages(IList<PdfPage> pages, bool cloneNonPageContentElements) {
			PdfCompatibility compatibility = documentCatalog.CreationOptions.Compatibility;
			if (compatibility != PdfCompatibility.Pdf)
				throw new NotSupportedException(String.Format(PdfCoreLocalizer.GetString(PdfCoreStringId.MsgIncompatibleOperationWithCurrentDocumentFormat), compatibility));
			List<PdfPage> result = new List<PdfPage>();
			if (pages != null && pages.Count > 0) {
				PdfDocumentCatalog foreignCatalog = pages[0].DocumentCatalog;
				Guid foreignCollectionId = foreignCatalog.Objects.id;
				try {
					PrepareToClone(foreignCatalog);
					PdfPageTreeNode node = documentCatalog.Pages.GetPageNode(this, false);
					foreach (PdfPage foreignPage in pages) {
						PdfObjectReference reference = AddCloneObject(foreignPage.ObjectNumber, () => foreignPage, foreignCollectionId);
						PdfPage page = ResolveObject<PdfPage>(reference.Number, () => new PdfPage(node, GetDictionary(reference.Number)));
						foreach (PdfAnnotation annotation in page.Annotations) {
							PdfWidgetAnnotation widget = annotation as PdfWidgetAnnotation;
							if (widget != null)
								documentCatalog.AddInteractiveFormField(widget.ResolveInteractiveFormField());
						}
						result.Add(page);
					}
					if (cloneNonPageContentElements) {
						CloneOutlines(foreignCollectionId, documentCatalog.Bookmarks, foreignCatalog.Bookmarks);
						PdfDeferredSortedDictionary<string, PdfFileSpecification> embeddedFiles = (PdfDeferredSortedDictionary<string, PdfFileSpecification>)documentCatalog.Names.EmbeddedFiles;
						foreach (PdfFileSpecification fileSpecification in foreignCatalog.Names.EmbeddedFiles.Values) {
							PdfObjectReference reference = AddCloneObject(fileSpecification.ObjectNumber, () => fileSpecification, foreignCollectionId);
							embeddedFiles.AddDeferred(PdfNames.NewKey(embeddedFiles), reference, GetFileSpecification);
						}
					}
					documentCatalog.FileAttachments.InvalidateSearchedFileAnnotations();
					PdfDeferredSortedDictionary<string, PdfDestination> destinations = (PdfDeferredSortedDictionary<string, PdfDestination>)documentCatalog.Destinations;
					PdfDeferredSortedDictionary<string, PdfDestination> foreignDestinations = (PdfDeferredSortedDictionary<string, PdfDestination>)foreignCatalog.Destinations;
					foreach (string name in savedDestinationNames) {
						PdfDestination destination;
						if (foreignDestinations.TryGetValue(name, out destination) && destination != null) {
							PdfDestination duplicate = (PdfDestination)((IPdfDeferredSavedObject)destination).CreateDuplicate();
							destinations.AddDeferred(GetDestinationName(name), AddDeferredSavedObject(duplicate, foreignCollectionId), o => duplicate);
						}
					}
				}
				finally {
					ClearAfterClonning(foreignCatalog);
				}
			}
			return result;
		}
		public void UpdateStream(Stream stream) {
			documentStream = new PdfDocumentStream(new BufferedStream(stream));
			documentStream.EncryptionInfo = encryptionInfo;
		}
		public object GetContainerValue(int number) {
			PdfObjectContainer container = GetObject(number) as PdfObjectContainer;
			if (container == null)
				return null;
			return container.Value;
		}
		public void RemoveCorruptedObjects() {
			foreach (int key in new List<int>(collection.Keys))
				if (!(collection[key] is PdfObjectStreamElement))
					collection.Remove(key);
		}
		public void AddItem(PdfDocumentItem obj, bool force) {
			int number = obj.ObjectNumber;
			if (force || !collection.ContainsKey(number)) {
				collection[number] = obj;
				LastObjectNumber = number;
			}
		}
		public void AddFreeObject(int number, int generation) {
			if (number == 0 || !collection.ContainsKey(number)) {
				collection[number] = new PdfFreeObject(number, generation);
				LastObjectNumber = number;
			}
		}
		public PdfEncryptionInfo EnsureEncryptionInfo(object value, byte[][] id, PdfGetPasswordAction getPasswordAction) {
			if (encryptionInfo == null) {
				object resolvedObject = TryResolve(value);
				if (resolvedObject == null)
					return null;
				PdfReaderDictionary dictionary = resolvedObject as PdfReaderDictionary;
				if (dictionary == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				encryptionInfo = new PdfEncryptionInfo(dictionary, id, getPasswordAction);
				if (documentStream != null)
					documentStream.EncryptionInfo = encryptionInfo;
			}
			return encryptionInfo;
		}
		public object TryResolvePrivateDataObject(object value) {
			return TryResolve(value, ReadPrivateDataIndirectObject);
		}
		public object TryResolve(object value) {
			return TryResolve(value, ReadIndirectObject);
		}
		public object TryResolve(object value, Func<long, PdfIndirectObject> readObject) {
			PdfObjectReference reference = value as PdfObjectReference;
			if (reference == null)
				return value;
			int number = reference.Number;
			object result = GetObjectData(number, readObject);
			PdfReaderDictionary dictionary = result as PdfReaderDictionary;
			if (dictionary == null)
				return result;
			if (dictionary.Number != number)
				dictionary.Number = number;
			return dictionary;
		}
		public object GetObjectData(int objectNumber) {
			return GetObjectData(GetObject(objectNumber), objectNumber, ReadIndirectObject);
		}
		public object GetObjectData(int objectNumber, Func<long, PdfIndirectObject> readObject) {
			return GetObjectData(ResolveObject(objectNumber, readObject), objectNumber, readObject);
		}
		public object GetObjectData(object obj, int objectNumber) {
			return GetObjectData(objectNumber, ReadIndirectObject);
		}
		public object GetObjectData(object obj, int objectNumber, Func<long, PdfIndirectObject> readObject) {
			PdfObjectContainer container = obj as PdfObjectContainer;
			if (container != null)
				return container.Value;
			PdfIndirectObject indirectObject = obj as PdfIndirectObject;
			if (indirectObject != null) {
				int generation = indirectObject.ObjectGeneration;
				object value = PdfDocumentParser.ParseObject(this, objectNumber, generation, indirectObject.Data, indirectObject.ApplyEncryption);
				return value;
			}
			PdfObjectStreamElement streamElement = obj as PdfObjectStreamElement;
			if (streamElement == null)
				if (obj == null || obj is PdfFreeObject || obj is PdfObjectSlot)
					return null;
				else
					PdfDocumentReader.ThrowIncorrectDataException();
			int streamObjectNumber = streamElement.ObjectStreamNumber;
			PdfObjectStream objectStream = ResolveObject(streamObjectNumber, readObject) as PdfObjectStream;
			if (objectStream == null) {
				PdfReaderStream stream = GetObjectData(streamObjectNumber, readObject) as PdfReaderStream;
				if (stream == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				objectStream = new PdfObjectStream(stream);
				ReplaceCollectionItem(objectStream);
			}
			return objectStream.Objects[streamElement.ElementIndex];
		}
		public PdfReaderDictionary GetDictionary(int objectNumber) {
			object obj = GetObject(objectNumber);
			if (obj == null || obj is PdfFreeObject)
				return null;
			PdfReaderDictionary dictionary = GetObjectData(obj, objectNumber) as PdfReaderDictionary;
			if (dictionary == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			if (dictionary.Number != objectNumber) {
				dictionary.Number = objectNumber;
				dictionary.Generation = 0;
			}
			return dictionary;
		}
		public PdfReaderStream GetStream(int objectNumber) {
			object obj = GetObject(objectNumber);
			return (obj == null || (obj is PdfFreeObject)) ? null : (GetObjectData(obj, objectNumber) as PdfReaderStream);
		}
		public void AddResolvedPage(int objectNumber, PdfPage page) {
			resolvedObjects[objectNumber] = new WeakReference(page);
		}
		public int AddResolvedObject(PdfObject value) {
			return AddResolvedObject(value, false);
		}
		public int AddResolvedObject(PdfObject value, bool force) {
			if (value == null)
				return 0;
			if (!savedReferences.Contains(value)) {
				int number = ++LastObjectNumber;
				value.ObjectNumber = number;
				if (force || writeIndirectObject == null) {
					resolvedObjects.Add(number, new WeakReference(value));
					savedReferences.Add(value);
				}
				else
					AddObject(value);
				return number;
			}
			return value.ObjectNumber;
		}
		public PdfInteractiveFormField GetResolvedInteractiveFormField(PdfObjectReference reference) {
			if (reference == null)
				return null;
			PdfInteractiveFormField result;
			resolvedInteractiveFormField.TryGetValue(reference.Number, out result);
			return result;
		}
		public PdfPage GetPage(int objectNumber) {
			PdfPage page = GetResolvedObject<PdfPage>(objectNumber, true);
			if (page == null)
				page = documentCatalog.Pages.FindPage(objectNumber);
			return page;
		}
		public T GetResolvedObject<T>(int objectNumber, bool mustBe) where T : PdfObject {
			T result = null;
			WeakReference reference;
			if (resolvedObjects.TryGetValue(objectNumber, out reference)) {
				result = reference.Target as T;
				if (!reference.IsAlive && result == null && mustBe)
					PdfDocumentReader.ThrowIncorrectDataException();
			}
			return result;
		}
		public PdfAnnotationAppearances GetAnnotationAppearances(object value, PdfRectangle parentBBox) {
			if (value == null)
				return null;
			PdfReaderDictionary dictionary = value as PdfReaderDictionary;
			if (dictionary != null)
				return new PdfAnnotationAppearances(dictionary, parentBBox);
			PdfReaderStream stream = value as PdfReaderStream;
			if (stream != null)
				return new PdfAnnotationAppearances(PdfForm.Create(stream, null));
			PdfObjectReference reference = value as PdfObjectReference;
			if (reference != null) {
				PdfAnnotationAppearances result;
				if (!resolvedAppearances.TryGetValue(reference.Number, out result)) {
					object resolvedValue = TryResolve(value);
					if (resolvedValue is PdfReaderStream)
						result = new PdfAnnotationAppearances(GetForm(reference.Number));
					else
						result = GetAnnotationAppearances(resolvedValue, parentBBox);
					resolvedAppearances.Add(reference.Number, result);
				}
				return result;
			}
			PdfDocumentReader.ThrowIncorrectDataException();
			return null;
		}
		public PdfInteractiveFormField GetInteractiveFormField(PdfInteractiveForm form, PdfInteractiveFormField parent, object value) {
			if (value == null)
				return null;
			PdfObjectReference reference = value as PdfObjectReference;
			if (reference == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			int objectNumber = reference.Number;
			PdfInteractiveFormField result;
			if (resolvedInteractiveFormField.TryGetValue(objectNumber, out result))
				return result;
			value = TryResolve(value);
			if (value == null)
				return null;
			PdfReaderDictionary dictionary = value as PdfReaderDictionary;
			if (dictionary == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			result = PdfInteractiveFormField.Parse(form, parent, dictionary, reference);
			resolvedInteractiveFormField.Add(objectNumber, result);
			return result;
		}
		public PdfAction GetAction(object value) {
			return GetItem<PdfAction>(value, dictionary => PdfAction.Parse(dictionary));
		}
		public PdfAdditionalActions GetAdditionalActions(object value, PdfAnnotation parent) {
			return GetItem<PdfAdditionalActions>(value, dictionary => new PdfAdditionalActions(parent, dictionary));
		}
		public PdfAnnotation GetAnnotation(PdfPage page, object value) {
			return GetItem<PdfAnnotation>(value, d => PdfAnnotation.Parse(page, d));
		}
		public PdfArticleThread GetArticleThread(PdfObjectReference reference) {
			return ResolveObject<PdfArticleThread>(reference.Number, dictionary => new PdfArticleThread(dictionary));
		}
		public PdfBead GetBead(int objectNumber, PdfArticleThread thread) {
			return ResolveObject<PdfBead>(objectNumber, dictionary => new PdfBead(thread, dictionary));
		}
		public PdfDestination GetDestination(object value) {
			PdfReaderDictionary dictionary = TryResolve(value) as PdfReaderDictionary;
			if (dictionary != null) {
				string type = dictionary.GetName(PdfDictionary.DictionaryTypeKey);
				string actionType = dictionary.GetName(PdfAction.ActionTypeDictionaryKey);
				if (actionType != null && (type == null || type == PdfAction.DictionaryType || type == "A")) {
					PdfJumpAction action = GetAction(value) as PdfJumpAction;
					if (action == null)
						PdfDocumentReader.ThrowIncorrectDataException();
					return action.Destination;
				}
			}
			Func<object, PdfDestination> create = o => PdfDestination.Parse(documentCatalog, o);
			return GetItem<PdfDestination>(value, create);
		}
		public PdfFileSpecification GetFileSpecification(object value) {
			Func<object, PdfFileSpecification> fn = d => PdfFileSpecification.Parse(null, d);
			return GetItem<PdfFileSpecification>(value, fn);
		}
		public PdfFont GetFont(int objectNumber) {
			return ResolveObject<PdfFont>(objectNumber, () => {
				object value = GetObjectData(objectNumber);
				PdfFont result = null;
				if (value != null) {
					PdfReaderDictionary dictionary = GetObjectData(objectNumber) as PdfReaderDictionary;
					if (dictionary == null)
						PdfDocumentReader.ThrowIncorrectDataException();
					result = PdfFont.CreateFont(dictionary);
				}
				return result;
			});
		}
		public PdfForm GetForm(int objectNumber) {
			return ResolveObject<PdfForm>(objectNumber, stream => new PdfForm(stream, null));
		}
		public PdfGraphicsStateParameters GetGraphicsStateParameters(int objectNumber) {
			return ResolveObject<PdfGraphicsStateParameters>(objectNumber, dictionary => new PdfGraphicsStateParameters(dictionary));
		}
		public PdfHalftone GetHalftone(object value) {
			Func<object, PdfHalftone> fn = o => PdfHalftone.Parse(o);
			return GetItem<PdfHalftone>(value, fn);
		}
		public PdfLogicalStructureElementAttribute GetLogicalStructureElementAttributes(int objectNumber) {
			return ResolveObject<PdfLogicalStructureElementAttribute>(objectNumber, dictionary => PdfLogicalStructureElementAttribute.Parse(dictionary));
		}
		public PdfLogicalStructureItem GetLogicalStructureItem(PdfLogicalStructure logicalStructure, PdfLogicalStructureEntry parent, object value) {
			Func<object, PdfLogicalStructureItem> fn = d => PdfLogicalStructureItem.Parse(logicalStructure, parent, TryResolve(d));
			return GetItem<PdfLogicalStructureItem>(value, fn);
		}
		public PdfOptionalContent GetOptionalContent(object value) {
			return GetItem<PdfOptionalContent>(value, dict => PdfOptionalContent.Create(dict));
		}
		public PdfOptionalContentGroup GetOptionalContentGroup(object value) {
			PdfOptionalContentGroup group = GetOptionalContent(value) as PdfOptionalContentGroup;
			if (group == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			return group;
		}
		public PdfShading GetShading(object value) {
			return GetItem<PdfShading>(value, v => {
				PdfReaderStream stream = null;
				PdfReaderDictionary dictionary = v as PdfReaderDictionary;
				if (dictionary == null) {
					stream = v as PdfReaderStream;
					if (stream == null)
						PdfDocumentReader.ThrowIncorrectDataException();
					dictionary = stream.Dictionary;
				}
				return PdfShading.Parse(stream, dictionary);
			});
		}
		public PdfXObject GetXObject(object value, PdfResources parentResources, string defaultSubtype) {
			return GetItem<PdfXObject>(value, stream => PdfXObject.Parse(stream, parentResources, defaultSubtype));
		}
		public PdfPageResources GetPageResources(object value, PdfPageResources parentResources) {
			PdfObjectReference reference = value as PdfObjectReference;
			PdfPageResources result;
			int number = PdfObject.DirectObjectNumber;
			if (reference != null) {
				number = reference.Number;
				if (resolvedPageResources.TryGetValue(number, out result))
					return result;
			}
			result = new PdfPageResources(documentCatalog, parentResources, TryResolve(value) as PdfReaderDictionary);
			if (number > 0) {
				if (resolvedObjects.ContainsKey(number))
					result.ObjectNumber = PdfObject.DirectObjectNumber;
				resolvedPageResources.Add(number, result);
			}
			return result;
		}
		public PdfResources GetResources(object value, PdfResources parentResources, bool shouldBeWritten) {
			if (value == null)
				return new PdfResources(documentCatalog, parentResources, null, shouldBeWritten);
			bool resetNumber = false;
			PdfResources result = GetItem<PdfResources>(value, dictionary =>{
				PdfResources resources = new PdfResources(documentCatalog, parentResources, dictionary, shouldBeWritten);
				int number = resources.ObjectNumber;
				if (number > 0 && resolvedPageResources.ContainsKey(number))
					resetNumber = true;
				return resources;
			});
			if (resetNumber)
				result.ObjectNumber = PdfObject.DirectObjectNumber;
			return result;
		}
		public T ResolveObject<T>(int objectNumber, Func<T> create) where T : PdfObject {
			T result;
			WeakReference reference;
			if (resolvedObjects.TryGetValue(objectNumber, out reference)) {
				object value = reference.Target;
				if (reference.IsAlive) {
					result = value as T;
					if (result == null)
						PdfDocumentReader.ThrowIncorrectDataException();
					return result;
				}
				else resolvedObjects.Remove(objectNumber);
			}
			result = create();
			if (result != null) {
				result.ObjectNumber = objectNumber;
				resolvedObjects.Add(objectNumber, new WeakReference(result));
			}
			return result;
		}
		public PdfObjectReference AddStream(PdfDictionary dictionary, byte[] data) {
			RaiseElementWrited();
			return AddStream(new PdfCompressedStream(dictionary, data));
		}
		public PdfObjectReference AddStream(byte[] data) {
			return AddStream(new PdfDictionary(), data);
		}
		public PdfObjectReference AddStream(string data) {
			return AddStream(Encoding.UTF8.GetBytes(data));
		}
		public PdfObjectReference AddStream(PdfStream stream) {
			return AddObject(stream);
		}
		public PdfObjectReference AddDictionary(PdfDictionary dictionary) {
			return AddObject(dictionary);
		}
		public PdfObjectReference AddObject(int number, Func<PdfObject> getObject) {
			return AddObject(number, getObject, false);
		}
		public PdfObjectReference AddObject(PdfObject value) {
			return value == null ? null : AddObject(value.ObjectNumber, () => value);
		}
		public void ResolveAllSlots() {
			List<int> keys = new List<int>(collection.Keys);
			foreach (int number in keys) {
				PdfDocumentItem item = collection[number];
				PdfObjectSlot slot = item as PdfObjectSlot;
				if (slot != null) {
					item = ResolveSlot(slot, ReadPrivateDataIndirectObject);
					if (item == null || item.ObjectNumber != number)
						PdfDocumentReader.ThrowIncorrectDataException();
				}
				if (item != null)
					ReplaceCollectionItem(item);
			}
			documentStream = null;
		}
		public string GetDestinationName(string currentName) {
			string newName = currentName;
			if (currentName != null && renamedDestinations.Count > 0)
				if (!renamedDestinations.TryGetValue(currentName, out newName))
					newName = currentName;
			return newName;
		}
		public string GetFormFieldName(string currentName) {
			string newName = currentName;
			if (currentName != null && renamedFormField.Count > 0)
				if (!renamedFormField.TryGetValue(currentName, out newName))
					newName = currentName;
			return newName;
		}
		public void FinalizeWritingAndClearWriteParameters() {
			foreach (KeyValuePair<PdfObject, int> kvp in deferredSavedObjects) {
				IPdfDeferredSavedObject deferredObject = kvp.Key as IPdfDeferredSavedObject;
				foreignCollectionId = deferredObject.CollectionId;
				AddObject(kvp.Key.ObjectNumber, () => kvp.Key, true);
			}
			foreignCollectionId = Guid.Empty;
			deferredSavedObjects.Clear();
			writeIndirectObject = null;
			writtenObjects.Clear();
			writtenObjectsNumber.Clear();
		}
		public PdfObjectReference GetSavedObjectReference(int number, Guid idCollection) {
			return GetSavedObjectReference(number, idCollection, (d) => {
				PdfObjectCollection objects = documentCatalog.Objects;
				if (objects != this)
					return objects.GetSavedObjectReference(number, idCollection);
				return null;
			});
		}
		PdfObjectReference GetSavedObjectReference(int number, Guid idCollection, Func<Dictionary<Guid, int>, PdfObjectReference> create) {
			Dictionary<Guid, int> dictionary = null;
			if (number > 0) {
				int writtenNumber = PdfObject.DirectObjectNumber;
				if (writtenObjectsNumber.TryGetValue(number, out dictionary) && dictionary.TryGetValue(idCollection, out writtenNumber))
					return new PdfObjectReference(writtenNumber);
			}
			return create == null ? null : create(dictionary);
		}
		PdfObjectReference AddDeferredSavedObject<T>(T value, Guid collectionId) where T : PdfObject, IPdfDeferredSavedObject {
			int number;
			if (!deferredSavedObjects.TryGetValue(value, out number)) {
				number = ++LastObjectNumber;
				deferredSavedObjects.Add(value, number);
			}
			return new PdfObjectReference(number);
		}
		PdfObjectReference AddObject(int number, Func<PdfObject> getObject, bool writeDeferredObjects) {
			PdfObject value = null;
			if (number <= 0) {
				value = getObject();
				if (value == null)
					return null;
				number = value.ObjectNumber;
			}
			Guid valueCollectionID = foreignCollectionId == Guid.Empty ? this.id : foreignCollectionId;
			return GetSavedObjectReference(number, valueCollectionID, dictionary => {
				value = value ?? getObject();
				if (value == null)
					return null;
				int writtenNumber;
				if (!writtenObjects.TryGetValue(value, out writtenNumber)) {
					bool deferredSavedObjectsContainsValue = value is IPdfDeferredSavedObject && deferredSavedObjects.TryGetValue(value, out writtenNumber);
					if (writeDeferredObjects || !deferredSavedObjectsContainsValue) {
						if (!writeDeferredObjects && (number < 1 || value.ObjectNumber != number)) {
							writtenNumber = ++LastObjectNumber;
							AddToWrittenObjects(value, writtenNumber);
						}
						else {
							if (forceUpdateNumbers)
								writtenNumber = ++LastObjectNumber;
							else
								writtenNumber = deferredSavedObjectsContainsValue ? writtenNumber : number;
							if (dictionary == null) {
								dictionary = new Dictionary<Guid, int>();
								writtenObjectsNumber[number] = dictionary;
							}
							dictionary[valueCollectionID] = writtenNumber;
						}
						object result = value.ToWritableObject(this);
						if (writeDeferredObjects || result != null) {
							PdfObjectContainer container = new PdfObjectContainer(writtenNumber, 0, result);
							if (writeIndirectObject == null)
								AddItem(container, true);
							else
								writeIndirectObject(container);
							RaiseElementWrited();
						}
						else
							return null;
					}
				}
				return new PdfObjectReference(writtenNumber);
			});
		}
		void RaiseElementWrited() {
			if (ElementWriting != null)
				ElementWriting(this, EventArgs.Empty);
		}
		PdfIndirectObject ReadIndirectObject(long offset) {
			return documentStream.ReadIndirectObject(offset);
		}
		PdfIndirectObject ReadPrivateDataIndirectObject(long offset) {
			return documentStream.ReadPrivateDataIndirectObject(offset);
		}
		PdfDocumentItem GetObject(int number) {
			return ResolveObject(number, ReadIndirectObject);
		}
		PdfDocumentItem ResolveObject(int number, Func<long, PdfIndirectObject> readObject) {
			PdfDocumentItem item;
			if (collection.TryGetValue(number, out item))
				return ResolveSlot(item as PdfObjectSlot, readObject) ?? item;
			return null;
		}
		PdfDocumentItem ResolveSlot(PdfObjectSlot slot, Func<long, PdfIndirectObject> readObject) {
			if (slot != null)
				try {
					long offset = slot.Offset;
					if (offset == 0) {
						PdfFreeObject item = new PdfFreeObject(slot.ObjectNumber, slot.ObjectGeneration);
						ReplaceCollectionItem(item);
						return item;
					}
					else {
						PdfIndirectObject o = readObject(offset);
						o.ApplyEncryption = slot.ApplyEncryption;
						return o;
					}
				}
				catch {
				}
			return null;
		}
		void ReplaceCollectionItem(PdfDocumentItem obj) {
			int number = obj.ObjectNumber;
			PdfDocumentItem pdfObject;
			if (!collection.TryGetValue(number, out pdfObject))
				PdfDocumentReader.ThrowIncorrectDataException();
			collection[number] = obj;
		}
		T ResolveObject<T>(int objectNumber, Func<PdfReaderStream, T> create) where T : PdfObject {
			return ResolveObject<T>(objectNumber, () => {
				PdfReaderStream stream = GetStream(objectNumber);
				return stream == null ? null : create(stream);
			});
		}
		T ResolveObject<T>(int objectNumber, Func<PdfReaderDictionary, T> create) where T : PdfObject {
			return ResolveObject<T>(objectNumber, () => {
				PdfReaderDictionary dictionary = GetDictionary(objectNumber);
				return dictionary == null ? null : create(dictionary);
			});
		}
		T GetItem<T>(object value, Func<object, T> create) where T : PdfObject {
			if (value == null)
				return null;
			PdfObjectReference reference = value as PdfObjectReference;
			if (reference != null)
				return ResolveObject<T>(reference.Number, () => GetItem<T>(TryResolve(value), create));
			return create(value);
		}
		T GetItem<T>(object value, Func<PdfReaderDictionary, T> create) where T : PdfObject {
			Func<object, T> fn = o => create((PdfReaderDictionary)o);
			return GetItem<T>(value, fn);
		}
		T GetItem<T>(object value, Func<PdfReaderStream, T> create) where T : PdfObject {
			Func<object, T> fn = o => create((PdfReaderStream)o);
			return GetItem<T>(value, fn);
		}
		PdfObjectReference AddObject(object value) {
			int number = ++LastObjectNumber;
			if (writeIndirectObject != null)
				writeIndirectObject(new PdfObjectContainer(number, 0, value));
			else
				AddItem(new PdfObjectContainer(number, 0, value), true);
			RaiseElementWrited();
			return new PdfObjectReference(number);
		}
		void AddToWrittenObjects(PdfObject value, int number) {
			writtenObjects.Add(value, number);
			PdfInteractiveFormField formField = value as PdfInteractiveFormField;
			if (formField != null && formField.Widget != null)
				writtenObjects.Add(formField.Widget, number);
			else {
				PdfWidgetAnnotation widget = value as PdfWidgetAnnotation;
				if (widget != null && widget.InteractiveFormField != null)
					writtenObjects.Add(widget.InteractiveFormField, number);
				else {
					PdfAnnotationAppearances appearances = value as PdfAnnotationAppearances;
					if (appearances != null && appearances.Form != null)
						writtenObjects.Add(appearances.Form, number);
				}
			}
		}
		PdfObjectReference AddCloneObject(int objectNumber, Func<PdfObject> getObject, Guid foreignCollectionId) {
			this.foreignCollectionId = foreignCollectionId;
			PdfDocumentStream documentStream = this.documentStream;
			Func<PdfObjectContainer, PdfObjectSlot> writeIndirectObject = this.writeIndirectObject;
			this.forceUpdateNumbers = true;
			try {
				if (writeIndirectObject != null) {
					this.writeIndirectObject = container => {
						PdfObjectSlot slot = writeIndirectObject(container);
						slot.ApplyEncryption = EncryptionInfo != null;
						AddItem(slot, false);
						int number = slot.ObjectNumber;
						Dictionary<Guid, int> dictionary;
						if (!writtenObjectsNumber.TryGetValue(number, out dictionary)) {
							dictionary = new Dictionary<Guid, int>();
							writtenObjectsNumber.Add(number, dictionary);
						}
						dictionary.Add(this.id, number);
						return slot;
					};
				}
				else {
					MemoryStream stream = new MemoryStream();
					this.documentStream = new PdfDocumentStream(stream);
					PdfObjectWriter writer = new PdfObjectWriter(this.documentStream);
					this.writeIndirectObject = container => {
						this.documentStream.Reset();
						PdfObjectSlot slot = writer.WriteIndirectObject(container);
						slot.ApplyEncryption = false;
						AddItem(ResolveSlot(slot, ReadPrivateDataIndirectObject), false);
						return slot;
					};
				}
				return AddObject(objectNumber, getObject);
			}
			finally {
				this.foreignCollectionId = Guid.Empty;
				this.forceUpdateNumbers = false;
				this.documentStream = documentStream;
				this.writeIndirectObject = writeIndirectObject;
			}
		}
		void CloneOutlines(Guid foreignCollectionId, IList<PdfBookmark> bookmarks, IList<PdfBookmark> foreignBookmarks) {
			foreach (PdfBookmark bookmark in foreignBookmarks) {
				PdfDestinationObject destinationObject = bookmark.DestinationObject;
				if (destinationObject != null) {
					string name = destinationObject.DestinationName;
					if (!string.IsNullOrEmpty(name)) {
						AddSavedDestinationName(name);
						destinationObject = new PdfDestinationObject(GetDestinationName(name));
					}
					else {
						PdfDestination destination = bookmark.Destination;
						destinationObject = destination == null ? null : new PdfDestinationObject(GetDestination(AddCloneObject(destination.ObjectNumber, () => destination, foreignCollectionId)));
					}
				}
				PdfAction action = bookmark.Action;
				if (action != null)
					action = GetAction(AddCloneObject(action.ObjectNumber, () => action, foreignCollectionId));
				PdfBookmark newBookmark = new PdfBookmark(bookmark, destinationObject, action) { Parent = documentCatalog };
				bookmarks.Add(newBookmark);
				CloneOutlines(foreignCollectionId, newBookmark.Children, bookmark.Children);
			}
		}
		void PrepareToClone(PdfDocumentCatalog foreignCatalog) {
			savedDestinationNames = new HashSet<string>();
			IDictionary<string, PdfDestination> destinations = documentCatalog.Destinations;
			PdfObjectCollection objects = documentCatalog.Objects;
			foreach (string name in foreignCatalog.Destinations.Keys)
				if (destinations.ContainsKey(name))
					renamedDestinations.Add(name, PdfNames.NewKey(destinations));
			PdfInteractiveForm foreingAcroForm = foreignCatalog.AcroForm;
			if (foreingAcroForm != null) {
				documentCatalog.AppendInteractiveFormResources(foreignCatalog.AcroForm.Resources);
				if (documentCatalog.AcroForm != null && documentCatalog.AcroForm.Fields.Count > 0) {
					List<string> names = new List<string>();
					foreach (PdfInteractiveFormField field in documentCatalog.AcroForm.Fields)
						names.Add(field.Name);
					foreach (PdfInteractiveFormField formField in foreingAcroForm.Fields) {
						if (names.Contains(formField.Name)) {
							string name = fieldNamePrefix + "0";
							for (int i = 0; names.Contains(name); name = fieldNamePrefix + ++i) { }
							renamedFormField.Add(formField.Name, name);
							names.Add(name);
						}
					}
				}
			}
		}
		void ClearAfterClonning(PdfDocumentCatalog foreignCatalog) {
			renamedDestinations.Clear();
			renamedFormField.Clear();
			PdfInteractiveForm foreingAcroForm = foreignCatalog.AcroForm;
			if (foreingAcroForm != null)
				foreingAcroForm.Resources.FreeRenamedResources();
			savedDestinationNames = null;
		}
	}
}
