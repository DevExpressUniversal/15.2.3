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
namespace DevExpress.Pdf.Native {
	public class PdfResources : PdfObject {
		protected class PdfResourceNamesDictionary : Dictionary<string, int> {
		}
		const string xObjectsDictionaryName = "XObject";
		const string fontsDictionaryName = "Font";
		const string colorSpacesDictionaryName = "ColorSpace";
		const string graphicsStateParametersDictionaryName = "ExtGState";
		const string patternsDictionaryName = "Pattern";
		const string shadingsDictionaryName = "Shading";
		const string procSetName = "ProcSet";
		const string propertiesDictionaryName = "Properties";
		const string fontNamePrefix = "F";
		const string xObjectNamePrefix = "O";
		const string graphicsStateParametersPrefix = "P";
		const string colorSpaceNamePrefix = "CS";
		const string shadingNamePrefix = "S";
		const string patternNamePrefix = "Ptrn";
		const string propertiesNamePrefix = "Prop";
		readonly PdfDocumentCatalog documentCatalog;
		readonly PdfResourceNamesDictionary graphicsStateParameters = new PdfResourceNamesDictionary();
		readonly PdfResourceNamesDictionary colorSpaces = new PdfResourceNamesDictionary();
		readonly PdfResourceNamesDictionary patterns = new PdfResourceNamesDictionary();
		readonly PdfResourceNamesDictionary xObjects = new PdfResourceNamesDictionary();
		readonly PdfResourceNamesDictionary fonts = new PdfResourceNamesDictionary();
		readonly PdfResourceNamesDictionary shadings = new PdfResourceNamesDictionary();
		readonly PdfResourceNamesDictionary properties = new PdfResourceNamesDictionary();
		readonly PdfResources parentResources;
		readonly bool shouldBeWritten;
		PdfReaderDictionary resources;
		Dictionary<string, Dictionary<string, string>> renamedResources = null;
		protected PdfResources ParentResources { get { return parentResources; } }
		internal PdfObjectCollection Objects { get { return documentCatalog.Objects; } }
		public PdfResources(PdfDocumentCatalog documentCatalog, PdfResources parentResources, PdfReaderDictionary resources, bool shouldBeWritten)
			: base(resources == null ? PdfObject.DirectObjectNumber : resources.Number) {
			this.parentResources = parentResources;
			this.resources = resources;
			this.documentCatalog = documentCatalog;
			this.shouldBeWritten = shouldBeWritten;
		}
		public void AddFont(PdfFont font) {
			if (!fonts.ContainsValue(font.ObjectNumber))
				fonts.Add(CreateResourceName(fonts, fontNamePrefix, GetFont), documentCatalog.Objects.AddResolvedObject(font, true));
		}
		public void AddPattern(PdfPattern pattern) {
			if (!patterns.ContainsValue(pattern.ObjectNumber))
				patterns.Add(CreateResourceName(patterns, patternNamePrefix, GetPattern), documentCatalog.Objects.AddResolvedObject(pattern));
		}
		public string AddXObject(int number) {
			foreach (KeyValuePair<string, int> res in xObjects)
				if (res.Value == number) {
					return res.Key;
				}
			string name = CreateResourceName(xObjects, xObjectNamePrefix, GetXObject);
			xObjects.Add(name, number);
			return name;
		}
		public void AddXObject(PdfXObject xObject) {
			if (!xObjects.ContainsValue(xObject.ObjectNumber))
				xObjects.Add(CreateResourceName(xObjects, xObjectNamePrefix, GetXObject), documentCatalog.Objects.AddResolvedObject(xObject));
		}
		public void AddGraphicsStateParameters(PdfGraphicsStateParameters parameters) {
			if (!graphicsStateParameters.ContainsValue(parameters.ObjectNumber))
				graphicsStateParameters.Add(CreateResourceName(graphicsStateParameters, graphicsStateParametersPrefix, GetGraphicsStateParameters), documentCatalog.Objects.AddResolvedObject(parameters));
		}
		public PdfGraphicsStateParameters GetGraphicsStateParameters(string graphicsStateParametersName) {
			return GetResourceWithParent<PdfGraphicsStateParameters>(graphicsStateParametersName, rp => rp.graphicsStateParameters, graphicsStateParametersDictionaryName, (o, d) => {
				PdfReaderDictionary dictionary = d as PdfReaderDictionary;
				if (dictionary == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				return new PdfGraphicsStateParameters(dictionary);
			});
		}
		public PdfColorSpace GetColorSpace(string colorSpaceName) {
			return GetResourceWithParent<PdfColorSpace>(colorSpaceName, rp => rp.colorSpaces, colorSpacesDictionaryName, (o, d) => PdfColorSpace.Parse(o, d)) ?? PdfColorSpace.CreateColorSpace(colorSpaceName);
		}
		public PdfShading GetShading(string shadingName) {
			return GetResourceWithParent<PdfShading>(shadingName, rp => rp.shadings, shadingsDictionaryName, (o, d) => o.GetShading(d));
		}
		public PdfPattern GetPattern(string patternName) {
			return GetResourceWithParent<PdfPattern>(patternName, rp => rp.patterns, patternsDictionaryName, (o, d) => PdfPattern.Parse(d));
		}
		public PdfXObject GetXObject(string xObjectName) {
			return GetResourceWithParent<PdfXObject>(xObjectName, rp => rp.xObjects, xObjectsDictionaryName, (o, d) => o.GetXObject(d, this, null));
		}
		public PdfFont GetFont(string fontName) {
			return GetResourceWithParent<PdfFont>(fontName, rp => rp.fonts, fontsDictionaryName, (o, d) => {
				if (d == null || (d is PdfFreeObject))
					return null;
				PdfReaderDictionary dictionary = d as PdfReaderDictionary;
				if (dictionary == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				return PdfFont.CreateFont(dictionary);
			});
		}
		public PdfProperties GetProperties(string propertiesName) {
			return GetResourceWithParent<PdfProperties>(propertiesName, rp => rp.properties, propertiesDictionaryName, (o, d) => {
				PdfReaderDictionary dictionary = d as PdfReaderDictionary;
				if (dictionary == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				return PdfProperties.Parse(dictionary);
			});
		}
		public PdfName FindShadingName(PdfShading value) {
			return FindResourceName(value, shadings, shadingsDictionaryName);
		}
		public PdfName FindPatternName(PdfPattern value) {
			return FindResourceName(value, patterns, patternsDictionaryName);
		}
		public PdfName FindColorSpaceName(PdfColorSpace value) {
			return FindResourceName(value, colorSpaces, colorSpacesDictionaryName);
		}
		public PdfName FindGraphicsStateParametersName(PdfGraphicsStateParameters value) {
			return FindResourceName(value, graphicsStateParameters, graphicsStateParametersDictionaryName);
		}
		public PdfName FindXObjectName(PdfXObject value) {
			return FindResourceName(value, xObjects, xObjectsDictionaryName);
		}
		public PdfName FindFontName(PdfFont value) {
			return FindResourceName(value, fonts, fontsDictionaryName);
		}
		public PdfName FindPropertiesName(PdfProperties value) {
			return FindResourceName(value, properties, propertiesDictionaryName);
		}
		internal void AppendResources(PdfResources appendedResources) {
			Dictionary<string, Dictionary<string, string>> renamedResources = new Dictionary<string, Dictionary<string, string>>();
			ICollection<string> resourceNames = appendedResources.GetResourceNames(graphicsStateParametersDictionaryName, appendedResources.graphicsStateParameters);
			renamedResources.Add(graphicsStateParametersDictionaryName, AppendResources(appendedResources, resourceNames, appendedResources.GetGraphicsStateParameters, GetGraphicsStateParameters, graphicsStateParameters, graphicsStateParametersPrefix));
			resourceNames = appendedResources.GetResourceNames(colorSpacesDictionaryName, appendedResources.colorSpaces);
			renamedResources.Add(colorSpacesDictionaryName, AppendResources(appendedResources, resourceNames, appendedResources.GetColorSpace, GetColorSpace, colorSpaces, colorSpaceNamePrefix));
			resourceNames = appendedResources.GetResourceNames(shadingsDictionaryName, appendedResources.shadings);
			renamedResources.Add(shadingsDictionaryName, AppendResources(appendedResources, resourceNames, appendedResources.GetShading, GetShading, shadings, shadingNamePrefix));
			resourceNames = appendedResources.GetResourceNames(patternsDictionaryName, appendedResources.patterns);
			renamedResources.Add(patternsDictionaryName, AppendResources(appendedResources, resourceNames, appendedResources.GetPattern, GetPattern, patterns, patternNamePrefix));
			resourceNames = appendedResources.GetResourceNames(xObjectsDictionaryName, appendedResources.xObjects);
			renamedResources.Add(xObjectsDictionaryName, AppendResources(appendedResources, resourceNames, appendedResources.GetXObject, GetXObject, xObjects, xObjectNamePrefix));
			resourceNames = appendedResources.GetResourceNames(fontsDictionaryName, appendedResources.fonts);
			renamedResources.Add(fontsDictionaryName, AppendResources(appendedResources, resourceNames, appendedResources.GetFont, GetFont, fonts, fontNamePrefix));
			resourceNames = appendedResources.GetResourceNames(propertiesDictionaryName, appendedResources.properties);
			renamedResources.Add(propertiesDictionaryName, AppendResources(appendedResources, resourceNames, appendedResources.GetProperties, GetProperties, properties, propertiesNamePrefix));
			appendedResources.RenameResourceWhileCloning(renamedResources);
		}
		internal void FreeRenamedResources() {
			renamedResources = null;
		}
		protected virtual void FillResourcesWriterDictionary<T>(PdfWriterDictionary dictionary, Func<PdfResources, PdfResourceNamesDictionary> getResourcesNames, Func<string, T> getResource) where T : PdfObject {
			PdfObjectCollection writeObjects = dictionary.Objects;
			IDictionary<string, int> resourcesNames = getResourcesNames(this);
			if (resourcesNames != null)
				foreach (KeyValuePair<string, int> kvp in resourcesNames) {
					string key = kvp.Key;
					if (!dictionary.ContainsKey(key))
						dictionary.Add(key, writeObjects.AddObject(kvp.Value, () => getResource(key)));
				}
		}
		protected internal override object ToWritableObject(PdfObjectCollection objects) {
			ResolveResourcesDictionary();
			PdfWriterDictionary result = new PdfWriterDictionary(objects);
			result.Add(graphicsStateParametersDictionaryName, CreateResourceWriterDictionary(objects, r => r.graphicsStateParameters, GetGraphicsStateParameters), null);
			result.Add(patternsDictionaryName, CreateResourceWriterDictionary(objects, r => r.patterns, GetPattern), null);
			result.Add(xObjectsDictionaryName, CreateResourceWriterDictionary(objects, r => r.xObjects, GetXObject), null);
			result.Add(fontsDictionaryName, CreateResourceWriterDictionary(objects, r => r.fonts, GetFont), null);
			result.Add(shadingsDictionaryName, CreateResourceWriterDictionary(objects, r => r.shadings, GetShading), null);
			result.Add(propertiesDictionaryName, CreateResourceWriterDictionary(objects, r => r.properties, GetProperties), null);
			result.Add(colorSpacesDictionaryName, CreateResourceWriterDictionary(objects, r => r.colorSpaces, GetColorSpace), null);
			if (!shouldBeWritten && result.Count == 0)
				return null;
			result.Add(procSetName, new PdfWritableArray(new object[] { new PdfName("PDF"), new PdfName("Text"), new PdfName("ImageB"), new PdfName("ImageC"), new PdfName("ImageI") }));
			return result;
		}
		PdfWriterDictionary CreateResourceWriterDictionary<T>(PdfObjectCollection objects, Func<PdfResources, PdfResourceNamesDictionary> getResourcesNames, Func<string, T> getResource) where T : PdfObject {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(objects);
			FillResourcesWriterDictionary(dictionary, getResourcesNames, getResource);
			return dictionary.Count > 0 ? dictionary : null;
		}
		void ResolveResourcesDictionary() {
			ResolveResource<PdfXObject>(xObjectsDictionaryName, xObjects, GetXObject);
			ResolveResource<PdfFont>(fontsDictionaryName, fonts, GetFont);
			ResolveResource<PdfColorSpace>(colorSpacesDictionaryName, colorSpaces, GetColorSpace);
			ResolveResource<PdfGraphicsStateParameters>(graphicsStateParametersDictionaryName, graphicsStateParameters, GetGraphicsStateParameters);
			ResolveResource<PdfPattern>(patternsDictionaryName, patterns, GetPattern);
			ResolveResource<PdfShading>(shadingsDictionaryName, shadings, GetShading);
			ResolveResource<PdfProperties>(propertiesDictionaryName, properties, GetProperties);
			resources = null;
			if (parentResources != null)
				parentResources.ResolveResourcesDictionary();
		}
		void ResolveResource<T>(string resourceKey, Dictionary<string, int> resourceDictionary, Func<string, T> create) where T : PdfObject {
			object value;
			if (resources != null && resources.TryGetValue(resourceKey, out value)) {
				PdfObjectCollection objects = documentCatalog.Objects;
				PdfReaderDictionary dictionary = objects.TryResolve(value) as PdfReaderDictionary;
				if (dictionary != null)
					foreach (KeyValuePair<string, object> kvp in dictionary) {
						string key = kvp.Key;
						if (!resourceDictionary.ContainsKey(key)) {
							object resourceValue = kvp.Value;
							PdfObjectReference reference = resourceValue as PdfObjectReference;
							if (reference == null)
								create(key);
							else
								resourceDictionary.Add(key, reference.Number);
						}
					}
			}
		}
		void RenameResourceWhileCloning(Dictionary<string, Dictionary<string, string>> renamedResources) {
			this.renamedResources = renamedResources;
		}
		Dictionary<string, string> AppendResources<T>(PdfResources appendedResources, ICollection<string> names, Func<string, T> getAppendedResource, Func<string, T> getCurrentResource, Dictionary<string, int> currentDictionary, string resourcePrefix) where T : PdfObject {
			Dictionary<string, string> result = new Dictionary<string, string>();
			if (names.Count > 0) {
				PdfObjectCollection objects = documentCatalog.Objects;
				foreach (string name in names) {
					T resource = getAppendedResource(name);
					if (resource != null) {
						if (getCurrentResource(name) != null) {
							string newName = CreateResourceName(currentDictionary, resourcePrefix, getCurrentResource);
							result.Add(name, newName);
							currentDictionary.Add(newName, objects.CloneObject(resource, appendedResources.documentCatalog.Objects.Id));
						}
						else
							currentDictionary.Add(name, objects.CloneObject(resource, appendedResources.documentCatalog.Objects.Id));
					}
				}
			}
			return result.Count > 0 ? result : null;
		}
		ICollection<string> GetResourceNames(string resourceKey, Dictionary<string, int> resolvedResources) {
			HashSet<string> names = new HashSet<string>(resolvedResources.Keys);
			object value;
			if (resources != null && resources.TryGetValue(resourceKey, out value)) {
				PdfReaderDictionary dictionary = resources.Objects.TryResolve(value) as PdfReaderDictionary;
				if (dictionary != null)
					foreach (string name in dictionary.Keys)
						if (!names.Contains(name))
							names.Add(name);
			}
			return names;
		}
		T GetResourceWithParent<T>(string resourceName, Func<PdfResources, Dictionary<string, int>> getResources, string resourceKey, Func<PdfObjectCollection, object, T> create) where T : PdfObject {
			T result = GetResource<T>(resourceName, getResources, resourceKey, create);
			if (result == null && parentResources != null) {
				result = parentResources.GetResourceWithParent<T>(resourceName, getResources, resourceKey, create);
				if (result != null)
					getResources(this)[resourceName] = result.ObjectNumber;
			}
			return result;
		}
		T GetResource<T>(string resourceName, Func<PdfResources, Dictionary<string, int>> getResources, string resourceKey, Func<PdfObjectCollection, object, T> create) where T : PdfObject {
			PdfObjectCollection objects = documentCatalog.Objects;
			Dictionary<string, int> resourcesDictionary = getResources(this);
			int objectNumber;
			if (!resourcesDictionary.TryGetValue(resourceName, out objectNumber)) {
				if (resources == null)
					return null;
				object value;
				if (!resources.TryGetValue(resourceKey, out value))
					return null;
				PdfReaderDictionary dictionary = objects.TryResolve(value) as PdfReaderDictionary;
				if (dictionary == null || !dictionary.TryGetValue(resourceName, out value))
					return null;
				PdfObjectReference reference = value as PdfObjectReference;
				if (reference == null) {
					objectNumber = objects.AddResolvedObject(create(objects, value));
				}
				else
					objectNumber = reference.Number;
				resourcesDictionary[resourceName] = objectNumber;
			}
			return objects.ResolveObject<T>(objectNumber, () => create(objects, objects.GetObjectData(objectNumber)));
		}
		PdfName FindResourceName<T>(T value, IDictionary<string, int> resources, string key) where T : PdfObject {
			if (value != null)
				foreach (KeyValuePair<string, int> res in resources)
					if (res.Value == value.ObjectNumber) {
						string name = null;
						Dictionary<string, string> dictionary = null;
						if (renamedResources != null && renamedResources.TryGetValue(key, out dictionary) && dictionary != null)
							dictionary.TryGetValue(res.Key, out name);
						return new PdfName(String.IsNullOrEmpty(name) ? res.Key : name);
					}
			return parentResources != null ? parentResources.FindResourceName<T>(value, resources, key) : null;
		}
		string CreateResourceName<T>(Dictionary<string, int> dictionary, string prefix, Func<string, T> getResource) where T : PdfObject {
			string name = prefix + '0';
			for (int i = 0; dictionary.ContainsKey(name) || getResource(name) != null; name = prefix + ++i) {
			}
			return name;
		}
	}
}
