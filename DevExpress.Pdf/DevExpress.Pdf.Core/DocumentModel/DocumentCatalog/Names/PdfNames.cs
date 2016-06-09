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
using System;
namespace DevExpress.Pdf {
	public class PdfNames {
		const string pageDestinationKey = "Dests";
		const string annotationAppearanceKey = "AP";
		const string javaScriptKey = "JavaScript";
		const string pageNamesKey = "Pages";
		const string idsKey = "IDS";
		const string urlsKey = "URLS";
		const string embeddedKey = "EmbeddedFiles";
		internal static string NewKey<Q>(IDictionary<string, Q> source) where Q : class {
			string newName = Guid.NewGuid().ToString();
			while (source.ContainsKey(newName))
				newName = Guid.NewGuid().ToString();
			return newName;
		}
		static PdfJavaScriptAction CreateJavaScriptAction(PdfObjectCollection collection, object value) {
			PdfObjectReference reference = value as PdfObjectReference;
			if (reference != null)
				return collection.ResolveObject<PdfJavaScriptAction>(reference.Number, () => CreateJavaScriptAction(collection, collection.GetObjectData(reference.Number)));
			PdfReaderDictionary dictionary = value as PdfReaderDictionary;
			if (dictionary == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			return new PdfJavaScriptAction(dictionary);
		}
		readonly PdfDeferredSortedDictionary<string, PdfDestination> pageDestinations;
		readonly PdfDeferredSortedDictionary<string, PdfAnnotationAppearances> annotationAppearances;
		readonly PdfDeferredSortedDictionary<string, PdfJavaScriptAction> javaScriptActions;
		readonly PdfDeferredSortedDictionary<string, PdfPage> pageNames;
		readonly PdfDeferredSortedDictionary<string, PdfSpiderSet> webCaptureContentSetsIds;
		readonly PdfDeferredSortedDictionary<string, PdfSpiderSet> webCaptureContentSetsUrls;
		readonly PdfDeferredSortedDictionary<string, PdfFileSpecification> embeddedFiles;
		List<PdfDestination> unresolvedInternalDestinations;
		public IDictionary<string, PdfDestination> PageDestinations { get { return pageDestinations; } }
		public IDictionary<string, PdfAnnotationAppearances> AnnotationAppearances { get { return annotationAppearances; } }
		public IDictionary<string, PdfJavaScriptAction> JavaScriptActions { get { return javaScriptActions; } }
		public IDictionary<string, PdfPage> PageNames { get { return pageNames; } }
		public IDictionary<string, PdfSpiderSet> WebCaptureContentSetsIds { get { return webCaptureContentSetsIds; } }
		public IDictionary<string, PdfSpiderSet> WebCaptureContentSetsUrls { get { return webCaptureContentSetsUrls; } }
		public IDictionary<string, PdfFileSpecification> EmbeddedFiles { get { return embeddedFiles; } }
		internal PdfNames(PdfReaderDictionary dictionary) {
			if (dictionary == null) {
				pageDestinations = new PdfDeferredSortedDictionary<string, PdfDestination>();
				embeddedFiles = new PdfDeferredSortedDictionary<string, PdfFileSpecification>();
			}
			else {
				pageDestinations = PdfNameTreeNode<PdfDestination>.Parse(dictionary.GetDictionary(pageDestinationKey), (o, v) => o.GetDestination(v));
				annotationAppearances = PdfNameTreeNode<PdfAnnotationAppearances>.Parse(dictionary.GetDictionary(annotationAppearanceKey), (o, v) => o.GetAnnotationAppearances(v, null));
				javaScriptActions = PdfNameTreeNode<PdfJavaScriptAction>.Parse(dictionary.GetDictionary(javaScriptKey), (o, v) => CreateJavaScriptAction(o, v));
				pageNames = PdfNameTreeNode<PdfPage>.Parse(dictionary.GetDictionary(pageNamesKey), (o, v) => o.DocumentCatalog.FindPage(v));
				webCaptureContentSetsIds = PdfNameTreeNode<PdfSpiderSet>.Parse(dictionary.GetDictionary(idsKey), (o, v) => PdfSpiderSet.Create(o, v));
				webCaptureContentSetsUrls = PdfNameTreeNode<PdfSpiderSet>.Parse(dictionary.GetDictionary(urlsKey), (o, v) => PdfSpiderSet.Create(o, v));
				embeddedFiles = PdfNameTreeNode<PdfFileSpecification>.Parse(dictionary.GetDictionary(embeddedKey), (o, v) => o.GetFileSpecification(v)) ?? new PdfDeferredSortedDictionary<string, PdfFileSpecification>();
			}
		}
		internal PdfWriterDictionary Write(PdfObjectCollection collection) {
			if (unresolvedInternalDestinations != null) {
				foreach (PdfDestination destionation in unresolvedInternalDestinations)
					destionation.ResolveInternalPage();
				unresolvedInternalDestinations = null;
			}
			PdfWriterDictionary result = new PdfWriterDictionary(collection);
			result.AddIfPresent(pageDestinationKey, PdfNameTreeNode<PdfDestination>.Write(collection, pageDestinations));
			result.AddIfPresent(annotationAppearanceKey, PdfNameTreeNode<PdfAnnotationAppearances>.Write(collection, annotationAppearances));
			result.AddIfPresent(javaScriptKey, PdfNameTreeNode<PdfJavaScriptAction>.Write(collection, javaScriptActions));
			result.AddIfPresent(pageNamesKey, PdfNameTreeNode<PdfPage>.Write(collection, pageNames));
			result.AddIfPresent(idsKey, PdfNameTreeNode<PdfSpiderSet>.Write(collection, webCaptureContentSetsIds));
			result.AddIfPresent(urlsKey, PdfNameTreeNode<PdfSpiderSet>.Write(collection, webCaptureContentSetsUrls));
			result.AddIfPresent(embeddedKey, PdfNameTreeNode<PdfFileSpecification>.Write(collection, embeddedFiles));
			return result;
		}
		internal string AddDestination(PdfDestination destination) {
			if (unresolvedInternalDestinations == null)
				unresolvedInternalDestinations = new List<PdfDestination>();
			string destinationName = NewKey<PdfDestination>(pageDestinations);
			pageDestinations.Add(destinationName, destination);
			unresolvedInternalDestinations.Add(destination);
			return destinationName;
		}
		internal string ReserveDestinationName() {
			string destinationName = NewKey<PdfDestination>(pageDestinations);
			pageDestinations.Add(destinationName, null);
			return destinationName;
		}
	}
}
