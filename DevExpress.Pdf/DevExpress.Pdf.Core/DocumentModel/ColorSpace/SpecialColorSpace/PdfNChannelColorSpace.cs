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
	public class PdfNChannelColorSpace : PdfDeviceNColorSpace {
		internal new const string TypeName = "NChannel";
		const string colorantsDictionaryKey = "Colorants";
		const string processDictionaryKey = "Process";
		const string colorSpaceDictionaryKey = "ColorSpace";
		const string componentsDictionaryKey = "Components";
		readonly PdfSeparationColorSpace[] colorants;
		readonly string[] processComponentsNames;
		readonly PdfColorSpace processColorSpace;
		public PdfSeparationColorSpace[] Colorants { get { return colorants; } }
		public string[] ProcessComponentsNames { get { return processComponentsNames; } }
		public PdfColorSpace ProcessColorSpace { get { return processColorSpace; } }
		internal PdfNChannelColorSpace(PdfObjectCollection collection, IList<object> array, PdfReaderDictionary dictionary) : base(collection, array) {
			List<string> actualNames = new List<string>();
			PdfDictionary colorantsDictionary = dictionary.GetDictionary(colorantsDictionaryKey);
			if (colorantsDictionary != null) {
				int colorantsCount = colorantsDictionary.Count;
				if (colorantsCount == 0)
					PdfDocumentReader.ThrowIncorrectDataException();
				colorants = new PdfSeparationColorSpace[colorantsCount];
				int index = 0;
				foreach (KeyValuePair<string, object> pair in colorantsDictionary) {
					string colorantName = pair.Key;
					PdfSeparationColorSpace separationColorSpace = PdfColorSpace.Parse(collection, pair.Value) as PdfSeparationColorSpace;
					if (separationColorSpace == null || separationColorSpace.Name != colorantName || separationColorSpace.ComponentsCount != 1) 
						PdfDocumentReader.ThrowIncorrectDataException();
					colorants[index++] = separationColorSpace;
					actualNames.Add(colorantName);
				}
			}
			PdfReaderDictionary processDictionary = dictionary.GetDictionary(processDictionaryKey);
			if (processDictionary != null) {
				IList<object> processComponentsArray = processDictionary.GetArray(componentsDictionaryKey);
				object value = null;
				if (processComponentsArray == null || !processDictionary.TryGetValue(colorSpaceDictionaryKey, out value))
					PdfDocumentReader.ThrowIncorrectDataException();
				processColorSpace = PdfColorSpace.Parse(collection, value);
				int processComponentsCount = processComponentsArray.Count;
				if (processComponentsCount != processColorSpace.ComponentsCount)
					PdfDocumentReader.ThrowIncorrectDataException();
				processComponentsNames = new string[processComponentsCount];
				for (int i = 0; i < processComponentsCount; i++) {
					PdfName componentName = processComponentsArray[i] as PdfName;
					if (componentName == null)
						PdfDocumentReader.ThrowIncorrectDataException();
					processComponentsNames[i] = componentName.Name;
				}
				actualNames.AddRange(processComponentsNames);
			}
			if (colorants == null && processColorSpace == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			foreach (string name in Names)
				if (!actualNames.Contains(name))
					PdfDocumentReader.ThrowIncorrectDataException();
		}
		protected override bool CheckArraySize(int actualSize) {
			return actualSize == 5;
		}
		protected override IList<object> CreateListToWrite(PdfObjectCollection collection) {
			List<object> list = new List<object>(base.CreateListToWrite(collection));
			PdfWriterDictionary dictionary = new PdfWriterDictionary(collection);
			dictionary.Add(PdfDictionary.DictionarySubtypeKey, new PdfName(TypeName));
			if (colorants != null) {
				PdfDictionary colorantsDictionary = new PdfDictionary();
				foreach (PdfSeparationColorSpace colorant in colorants)
					colorantsDictionary.Add(colorant.Name, colorant.Write(collection));
				dictionary.Add(colorantsDictionaryKey, collection.AddDictionary(colorantsDictionary));
			}
			if (processColorSpace != null) {
				PdfDictionary processDictionary = new PdfDictionary();
				processDictionary.Add(colorSpaceDictionaryKey, processColorSpace.Write(collection));
				List<object> components = new List<object>(processComponentsNames.Length);
				foreach (string name in processComponentsNames)
					components.Add(new PdfName(name));
				processDictionary.Add(componentsDictionaryKey, components);
				dictionary.Add(processDictionaryKey, collection.AddDictionary(processDictionary));
			}
			list.Add(collection.AddDictionary(dictionary));
			return list;
		}
	}
}
