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
	public class PdfRichMediaAnnotation : PdfAnnotation {
		internal const string Type = "RichMedia";
		const string richMediaContentKey = "RichMediaContent";
		const string richMediaContentDictionaryName = "RichMediaContent";
		const string richMediaSettingsKey = "RichMediaSettings";
		const string assetsKey = "Assets";
		const string configurationsKey = "Configurations";
		readonly PdfDeferredSortedDictionary<string, PdfFileSpecification> assets;
		readonly List<PdfRichMediaConfiguration> configurations = new List<PdfRichMediaConfiguration>();
		readonly PdfRichMediaSettings settings;
		public IDictionary<string, PdfFileSpecification> Assets { get { return assets; } }
		public IList<PdfRichMediaConfiguration> Configurations { get { return configurations; } }
		public PdfRichMediaSettings Settings { get { return settings; } }
		protected override string AnnotationType { get { return Type; } }
		internal PdfRichMediaAnnotation(PdfPage page, PdfReaderDictionary dictionary) : base(page, dictionary) {
			PdfReaderDictionary richMediaContentDictionary = dictionary.GetDictionary(richMediaContentKey);
			if (richMediaContentDictionary == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			string type = richMediaContentDictionary.GetName(PdfDictionary.DictionaryTypeKey);
			if (type != null && type != richMediaContentDictionaryName)
				PdfDocumentReader.ThrowIncorrectDataException();
			assets = PdfNameTreeNode<PdfFileSpecification>.Parse(richMediaContentDictionary.GetDictionary(assetsKey), (o, v) => o.GetFileSpecification(v));
			IList<object> configurationsArray = richMediaContentDictionary.GetArray(configurationsKey);
			if (configurationsArray != null) {
				PdfObjectCollection collection = dictionary.Objects;
				foreach (object value in configurationsArray) {
					PdfObjectReference reference = value as PdfObjectReference;
					if (reference == null)
						PdfDocumentReader.ThrowIncorrectDataException();
					PdfReaderDictionary configurationDictionary = collection.GetDictionary(reference.Number);
					if (configurationDictionary == null)
						PdfDocumentReader.ThrowIncorrectDataException();
					configurations.Add(new PdfRichMediaConfiguration(assets, configurationDictionary));
				}
			}
			settings = new PdfRichMediaSettings(this, dictionary.GetDictionary(richMediaSettingsKey));
		}
		protected override PdfWriterDictionary CreateDictionary(PdfObjectCollection collection) {
			PdfWriterDictionary result = base.CreateDictionary(collection);
			result.Add(richMediaContentKey, WriteRichMediaContent(collection));
			if (settings.Activation != null || settings.Deactivation != null)
				result.Add(richMediaSettingsKey, settings);
			return result;
		}
		PdfWriterDictionary WriteRichMediaContent(PdfObjectCollection collection) {
			PdfWriterDictionary result = new PdfWriterDictionary(collection);
			result.AddName(PdfDictionary.DictionaryTypeKey, richMediaContentDictionaryName);
			result.AddIfPresent(assetsKey, PdfNameTreeNode<PdfFileSpecification>.Write(collection, assets));
			result.AddList(configurationsKey, configurations);
			return result;
		}
	}
}
