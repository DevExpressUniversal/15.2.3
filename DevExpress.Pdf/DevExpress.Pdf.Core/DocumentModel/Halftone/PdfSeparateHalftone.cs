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
	public class PdfSeparateHalftone : PdfHalftone {
		internal const int Number = 5;
		const string grayDictionaryKey = "Gray";
		const string redDictionaryKey = "Red";
		const string greenDictionaryKey = "Green";
		const string blueDictionaryKey = "Blue";
		const string cyanDictionaryKey = "Cyan";
		const string magentaDictionaryKey = "Magenta";
		const string yellowDictionaryKey = "Yellow";
		const string blackDictionaryKey = "Black";
		const string defaultDictionaryKey = "Default";
		static PdfHalftone ResolveHalftone(PdfReaderDictionary dictionary, string key) {
			object value;
			if (!dictionary.TryGetValue(key, out value))
				PdfDocumentReader.ThrowIncorrectDataException();
			return dictionary.Objects.GetHalftone(value);
		}
		readonly IDictionary<string, PdfHalftone> components;
		readonly PdfHalftone defaultHalftone;
		public IDictionary<string, PdfHalftone> Components { get { return components; } }
		public PdfHalftone Default { get { return defaultHalftone; } }
		internal PdfSeparateHalftone(PdfReaderDictionary dictionary) : base(dictionary) {
			PdfObjectCollection objects = dictionary.Objects;
			components = new Dictionary<string, PdfHalftone>();
			foreach (KeyValuePair<string, object> pair in dictionary) {
				string key = pair.Key;
				if (key != PdfDictionary.DictionaryTypeKey && key != HalftoneTypeDictionaryKey && key != HalftoneNameDictionaryKey && key != defaultDictionaryKey)
					components[key] = PdfHalftone.Parse(objects.TryResolve(pair.Value));
			}
			defaultHalftone = ResolveHalftone(dictionary, defaultDictionaryKey);
		}
		protected override PdfWriterDictionary CreateDictionary(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = base.CreateDictionary(objects);
			dictionary.Add(HalftoneTypeDictionaryKey, Number);
			foreach (KeyValuePair<string, PdfHalftone> pair in components)
				dictionary.Add(pair.Key, pair.Value.CreateWriteableObject(objects));
			dictionary.Add(defaultDictionaryKey, defaultHalftone.CreateWriteableObject(objects));
			return dictionary;
		}
		protected internal override bool IsSame(PdfHalftone halftone) {
			PdfSeparateHalftone separateHalftone = halftone as PdfSeparateHalftone;
			if (separateHalftone == null || !base.IsSame(halftone) || !defaultHalftone.IsSame(separateHalftone.defaultHalftone))
				return false;
			IDictionary<string, PdfHalftone> separateHalftoneComponents = separateHalftone.components;
			if (components.Count != separateHalftoneComponents.Count)
				return false;
			foreach (KeyValuePair<string, PdfHalftone> pair in components) {
				PdfHalftone halftoneToCompare;
				if (!separateHalftoneComponents.TryGetValue(pair.Key, out halftoneToCompare) || !pair.Value.IsSame(halftoneToCompare))
					return false;
			}
			return true;
		}
	}
}
