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
using System.Globalization;
using System.Collections.Generic;
using DevExpress.Pdf.Localization;
namespace DevExpress.Pdf.Native {
	public class PdfDocumentCatalog : PdfObject, IPdfBookmarkParent {
		const string dictionaryType = "Catalog";
		const string pagesDictionaryKey = "Pages";
		const string pageLabelsDictionaryKey = "PageLabels";
		const string destinationDictionaryKey = "Dests";
		const string viewerPreferencesDictionaryKey = "ViewerPreferences";
		const string pageLayoutDictionaryKey = "PageLayout";
		const string pageModeDictionaryKey = "PageMode";
		const string outlinesDictionaryKey = "Outlines";
		const string threadsDictionaryKey = "Threads";
		const string openActionDictionaryKey = "OpenAction";
		const string additionalActionsDictionaryKey = "AA";
		const string acroFormDictionaryKey = "AcroForm";
		const string structTreeRootDictionaryKey = "StructTreeRoot";
		const string markInfoDictionaryKey = "MarkInfo";
		const string outputIntentsDictionaryKey = "OutputIntents";
		const string ocPropertiesDictionaryKey = "OCProperties";
		const string needsRenderingDictionaryKey = "NeedsRendering";
		const string legalContentAttestationDictionaryKey = "Legal";
		internal const string NamesDictionaryKey = "Names";
		internal const string PermissionsDictionaryKey = "Perms";
		static readonly Dictionary<double, string> fileVersions = new Dictionary<double, string>() { { 1.0, "1.0" }, { 1.1, "1.1" }, { 1.2, "1.2" }, { 1.3, "1.3" }, 
																									 { 1.4, "1.4" }, { 1.5, "1.5" }, { 1.6, "1.6" }, { 1.7, "1.7" } };
		readonly string version;
		readonly Dictionary<string, PdfDeveloperExtension> developerExtensions;
		readonly PdfPageList pages;
		readonly PdfDeferredSortedDictionary<int, PdfPageLabel> pageLabels;
		readonly PdfViewerPreferences viewerPreferences;
		readonly PdfPageLayout pageLayout = PdfPageLayout.SinglePage;
		readonly PdfPageMode pageMode = PdfPageMode.UseNone;
		readonly IList<PdfArticleThread> threads;
		readonly PdfDocumentActions actions;
		readonly PdfMarkInfo markInfo;
		readonly CultureInfo languageCulture;
		readonly IList<PdfOutputIntent> outputIntents;
		readonly Dictionary<string, PdfPieceInfoEntry> pieceInfo;
		readonly PdfOptionalContentProperties optionalContentProperties;
		readonly bool needsRendering;
		readonly PdfCreationOptions creationOptions;
		PdfFileAttachmentList fileAttachments;
		PdfLogicalStructure logicalStructure;
		PdfOutlines outlines;
		PdfInteractiveForm acroForm;
		PdfDestination openDestination;
		PdfAction openAction;
		PdfObjectCollection objects;
		int lastObjectNumber;
		int writerProgress;
		int progressCount;
		PdfMetadata metadata;
		PdfNames names;
		IDictionary<string, PdfDestination> destinations;
		PdfBookmarkList bookmarks;
		bool bookmarksChanged;
		PdfReaderDictionary dictionary;
		bool ensured;
		bool ensuredLogicalStructure;
		bool ensuredOutlines;
		public string Version { get { return version; } }
		public IDictionary<string, PdfDeveloperExtension> DeveloperExtensions { get { return developerExtensions; } }
		public PdfPageList Pages { get { return pages; } }
		public IDictionary<int, PdfPageLabel> PageLabels { get { return pageLabels; } }
		public PdfFileAttachmentList FileAttachments { 
			get {
				if (fileAttachments == null)
					fileAttachments = new PdfFileAttachmentList(this);
				return fileAttachments; 
			} 
		}
		public PdfNames Names {
			get {
				Ensure();
				return names;
			}
		}
		public IDictionary<string, PdfDestination> Destinations {
			get {
				Ensure();
				return destinations;
			}
		}
		public PdfViewerPreferences ViewerPreferences { get { return viewerPreferences; } }
		public PdfPageLayout PageLayout { get { return pageLayout; } }
		public PdfPageMode PageMode { get { return pageMode; } }
		public PdfOutlines Outlines {
			get {
				if (bookmarksChanged) {
					outlines = PdfBookmarkList.CreateOutlines(bookmarks);
					bookmarksChanged = false;
				}
				EnsureOutlines();
				return outlines;
			}
		}
		public IList<PdfBookmark> Bookmarks {
			get {
				if (bookmarks == null) {
					EnsureOutlines();
					bookmarks = new PdfBookmarkList(this, outlines);
				}
				return bookmarks;
			}
			set { 
				bookmarks = new PdfBookmarkList(this, value);
				bookmarksChanged = true;
			}
		}
		public IList<PdfArticleThread> Threads { get { return threads; } }
		public PdfDocumentActions Actions { get { return actions; } }
		public PdfMetadata Metadata { get { return metadata; } internal set { metadata = value; } }
		public PdfLogicalStructure LogicalStructure {
			get {
				if (!ensuredLogicalStructure && dictionary != null) {
					object value;
					if (dictionary.TryGetValue(structTreeRootDictionaryKey, out value)) {
						PdfReaderDictionary structTreeRootDictionary = dictionary.Objects.TryResolve(value) as PdfReaderDictionary;
						if (structTreeRootDictionary != null) 
							logicalStructure = new PdfLogicalStructure(structTreeRootDictionary);
					}
					ensuredLogicalStructure = true;
					FlushDictionary();
				}
				return logicalStructure;
			}
		}
		public PdfMarkInfo MarkInfo { get { return markInfo; } }
		public CultureInfo LanguageCulture { get { return languageCulture; } }
		public IList<PdfOutputIntent> OutputIntents { get { return outputIntents; } }
		public Dictionary<string, PdfPieceInfoEntry> PieceInfo { get { return pieceInfo; } }
		public PdfOptionalContentProperties OptionalContentProperties { get { return optionalContentProperties; } }
		public bool NeedsRendering { get { return needsRendering; } }
		public PdfInteractiveForm AcroForm { get { return acroForm; } }
		public PdfDestination OpenDestination {
			get { return openDestination; }
			set { openDestination = value; }
		}
		public PdfAction OpenAction {
			get { return openAction; }
			set { openAction = value; }
		}
		public PdfObjectCollection Objects {
			get { return objects; }
			set { objects = value; }
		}
		public int LastObjectNumber {
			get { return lastObjectNumber; }
			set { lastObjectNumber = Math.Max(lastObjectNumber, value); }
		}
		public PdfCreationOptions CreationOptions { get { return creationOptions; } }
		PdfDocumentCatalog IPdfBookmarkParent.DocumentCatalog { get { return this; } }
		public event PdfProgressChangedEventHandler ProgressChanged;
		public PdfDocumentCatalog(PdfObjectCollection objects, PdfCreationOptions creationOptions) {
			this.creationOptions = creationOptions ?? new PdfCreationOptions();
			this.objects = objects;
			objects.DocumentCatalog = this;
			pages = new PdfPageList(this);
			languageCulture = CultureInfo.InvariantCulture;
			names = new PdfNames(null);
			destinations = names.PageDestinations;
			outputIntents = new PdfOutputIntent[] { new PdfOutputIntent() };
		}
		public PdfDocumentCatalog(PdfReaderDictionary dictionary) : base(dictionary.Number) {
			this.creationOptions = new PdfCreationOptions();
			this.dictionary = dictionary;
			objects = dictionary.Objects;
			objects.DocumentCatalog = this;
			progressCount = objects.Count;
			object versionValue;
			if (dictionary.TryGetValue("Version", out versionValue)) {
				versionValue = objects.TryResolve(versionValue);
				PdfName name = versionValue as PdfName;
				if (name != null)
					version = name.Name;
				else if (versionValue is double) {
					double doubleVersion = (double)versionValue;
					if (!fileVersions.TryGetValue(doubleVersion, out version))
						PdfDocumentReader.ThrowIncorrectDataException();
				}
				else
					PdfDocumentReader.ThrowIncorrectDataException();
			}
			string type = dictionary.GetName(PdfDictionary.DictionaryTypeKey);
			PdfReaderDictionary pagesDictionary = dictionary.GetDictionary(pagesDictionaryKey);
			if ((type != null && type != dictionaryType) || pagesDictionary == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			developerExtensions = PdfDeveloperExtension.Parse(dictionary.GetDictionary("Extensions"));
			PdfPageTreeNode pageTreeNode = new PdfPageTreeNode(null, pagesDictionary);
			pages = new PdfPageList(pageTreeNode, this);
			pageLabels = PdfNumberTreeNode<PdfPageLabel>.Parse(dictionary.GetDictionary(pageLabelsDictionaryKey), (o, v) => new PdfPageLabel(o, v), true);
			PdfReaderDictionary viewerPreferencesDictionary = dictionary.GetDictionary(viewerPreferencesDictionaryKey);
			if (viewerPreferencesDictionary != null)
				viewerPreferences = new PdfViewerPreferences(viewerPreferencesDictionary);
			pageLayout = PdfEnumToStringConverter.Parse<PdfPageLayout>(dictionary.GetName(pageLayoutDictionaryKey));
			pageMode = PdfEnumToStringConverter.Parse<PdfPageMode>(dictionary.GetName(pageModeDictionaryKey));
			threads = PdfArticleThread.Parse(objects, dictionary.GetArray(threadsDictionaryKey));
			object value;
			if (dictionary.TryGetValue(openActionDictionaryKey, out value)) {
				IList<object> destinationArray = objects.TryResolve(value) as IList<object>;
				if (destinationArray == null) {
					openAction = dictionary.GetAction(openActionDictionaryKey);
					if (openAction == null)
						PdfDocumentReader.ThrowIncorrectDataException();
				}
				else {
					openDestination = objects.GetDestination(value);
					openDestination.ResolveInternalPage();
				}
			}
			PdfReaderDictionary aa = dictionary.GetDictionary(additionalActionsDictionaryKey);
			if (aa != null)
				actions = new PdfDocumentActions(aa);
			PdfReaderDictionary acroFormDictionary = dictionary.GetDictionary(acroFormDictionaryKey);
			if (acroFormDictionary != null)
				acroForm = new PdfInteractiveForm(acroFormDictionary);
			metadata = dictionary.GetMetadata();
			if (dictionary.TryGetValue(markInfoDictionaryKey, out value)) {
				value = objects.TryResolve(value);
				PdfReaderDictionary markInfoDictionary = value as PdfReaderDictionary;
				if (markInfoDictionary == null) {
					if (!(value is bool))
						PdfDocumentReader.ThrowIncorrectDataException();
					markInfo = new PdfMarkInfo((bool)value);
				}
				else
					markInfo = new PdfMarkInfo(markInfoDictionary);
			}
			languageCulture = dictionary.GetLanguageCulture();
			outputIntents = dictionary.GetArray<PdfOutputIntent>(outputIntentsDictionaryKey, d => {
				PdfReaderDictionary outputIntentDictionary = objects.TryResolve(d) as PdfReaderDictionary;
				if (outputIntentDictionary == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				return new PdfOutputIntent(outputIntentDictionary);
			});
			pieceInfo = PdfPieceInfoEntry.Parse(dictionary);
			PdfReaderDictionary oc = dictionary.GetDictionary(ocPropertiesDictionaryKey);
			if (oc != null)
				optionalContentProperties = new PdfOptionalContentProperties(oc);
			needsRendering = dictionary.GetBoolean("NeedsRendering") ?? false;
		}
		public PdfPage FindPage(object value) {
			if (value == null)
				return null;
			PdfObjectReference reference = value as PdfObjectReference;
			if (reference != null)
				return objects.GetPage(reference.Number);
			if (!(value is int))
				PdfDocumentReader.ThrowIncorrectDataException();
			int pageIndex = (int)value;
			return (pageIndex < 0 || pageIndex >= pages.Count) ? null : pages[pageIndex];
		}
		public void Append(PdfDocumentCatalog documentCatalog) {
			pages.AppendDocument(documentCatalog);
		}
		public PdfPage AddPage(PdfRectangle mediaBox, PdfRectangle cropBox, int rotate) {
			return pages.AddNewPage(new PdfPage(this, mediaBox, cropBox, rotate));
		}
		public PdfPage AddPage(int pageNumber, PdfRectangle mediaBox, PdfRectangle cropBox, int rotate) {
			int pageIndex = pageNumber - 1;
			int pageCount = pages.Count;
			if (pageIndex < 0 || pageIndex > pageCount)
				throw new ArgumentOutOfRangeException("position", PdfCoreLocalizer.GetString(PdfCoreStringId.MsgIncorrectInsertingPageNumber));
			return pageIndex == pageCount ? AddPage(mediaBox, cropBox, rotate) : pages.InsertNewPage(pageIndex, new PdfPage(this, mediaBox, cropBox, rotate));
		}
		public void DeletePage(int pageNumber) {
			pages.DeletePage(pageNumber);
		}
		public void ResetProgress() {
			writerProgress = 0;
		}
		internal void AppendInteractiveFormResources(PdfResources resources) {
			if (acroForm == null)
				acroForm = new PdfInteractiveForm(new PdfReaderDictionary(objects, PdfObject.DirectObjectNumber, 0));
			acroForm.Resources.AppendResources(resources);
		}
		internal void AddInteractiveFormField(PdfInteractiveFormField formField) {
			if (formField != null) {
				if (acroForm == null)
					acroForm = new PdfInteractiveForm(new PdfReaderDictionary(objects, PdfObject.DirectObjectNumber, 0));
				acroForm.AddInteractiveFormField(formField);
			}
		}
		internal PdfInteractiveFormField ResolveUnparsedInteractiveFormFields(PdfObjectReference reference) {
			PdfInteractiveFormField result = null;
			PdfReaderDictionary dictionary = objects.TryResolve(reference) as PdfReaderDictionary;
			if (dictionary != null) {
				PdfObjectReference parentReference = dictionary.GetObjectReference(PdfInteractiveFormField.ParentDictionaryKey);
				if (parentReference == null)
					result = objects.GetInteractiveFormField(acroForm, null, reference);
				else {
					PdfInteractiveFormField parent = ResolveUnparsedInteractiveFormFields(parentReference);
					result = objects.GetInteractiveFormField(acroForm, parent, reference);
				}
			}
			return result;
		}
		internal PdfWidgetAnnotation FindWidget(int widgetObjectNumber) {
			foreach (PdfPage page in pages)
				foreach (PdfAnnotation annotation in page.Annotations) {
					PdfWidgetAnnotation widget = annotation as PdfWidgetAnnotation;
					if (widget != null && widget.ObjectNumber == widgetObjectNumber)
						return widget;
				}
			return null;
		}
		void FlushDictionary() {
			if (ensured && ensuredOutlines && ensuredLogicalStructure)
				dictionary = null;
		}
		void EnsureOutlines() {
			if (!ensuredOutlines && dictionary != null) {
				PdfReaderDictionary outlinesDictionary = dictionary.GetDictionary(outlinesDictionaryKey);
				if (outlinesDictionary != null)
					outlines = new PdfOutlines(outlinesDictionary);
				ensuredOutlines = true;
				FlushDictionary();
			}
		}
		void Ensure() {
			if (!ensured && dictionary != null) {
				PdfReaderDictionary destinationsDictionary = dictionary.GetDictionary(destinationDictionaryKey);
				names = new PdfNames(dictionary.GetDictionary(NamesDictionaryKey));
				if (destinationsDictionary != null && destinationsDictionary.Count > 0) 
					foreach(KeyValuePair<string, PdfDestination> destination in  PdfDestination.Parse(destinationsDictionary))
						names.PageDestinations[destination.Key] = destination.Value;
				destinations = names.PageDestinations;
				ensured = true;
				FlushDictionary();
			}
		}
		void RaiseProgressChanged() {
			int percentage = (int)(100.0 * writerProgress / progressCount);
			if (ProgressChanged != null)
				ProgressChanged(this, new PdfProgressChangedEventArgs(Math.Min(100, percentage)));
		}
		protected internal override object ToWritableObject(PdfObjectCollection objects) {
			Ensure();
			objects.ElementWriting += (s, e) => {
				writerProgress = objects.WrittenObjectsCount;
				RaiseProgressChanged();
			};
			PdfWriterDictionary dictionary = new PdfWriterDictionary(objects);
			dictionary.Add(PdfDictionary.DictionaryTypeKey, new PdfName(dictionaryType));
			dictionary.Add(pagesDictionaryKey, pages.GetPageNode(objects, true));
			dictionary.AddIfPresent(pageLabelsDictionaryKey, PdfNumberTreeNode<PdfPageLabel>.Write(objects, pageLabels, null));
			if (viewerPreferences != null)
				dictionary.Add(viewerPreferencesDictionaryKey, viewerPreferences.Write());
			dictionary.AddEnumName(pageLayoutDictionaryKey, pageLayout);
			dictionary.AddEnumName(pageModeDictionaryKey, pageMode);
			dictionary.Add(outlinesDictionaryKey, Outlines);
			dictionary.AddList<PdfArticleThread>(threadsDictionaryKey, threads);
			dictionary.Add(openActionDictionaryKey, openDestination);
			dictionary.Add(openActionDictionaryKey, openAction);
			dictionary.Add(additionalActionsDictionaryKey, actions);
			dictionary.Add(acroFormDictionaryKey, acroForm);
			dictionary.Add(PdfMetadata.Name, metadata);
			dictionary.Add(structTreeRootDictionaryKey, LogicalStructure);
			if (markInfo != null)
				dictionary.Add(markInfoDictionaryKey, markInfo.Write());
			dictionary.AddLanguage(languageCulture);
			if (outputIntents != null && outputIntents.Count > 0)
				dictionary.AddList(outputIntentsDictionaryKey, outputIntents, i => ((PdfOutputIntent)i).Write(objects));
			PdfPieceInfoEntry.WritePieceInfo(dictionary, pieceInfo);
			if (optionalContentProperties != null)
				dictionary.Add(ocPropertiesDictionaryKey, optionalContentProperties.Write(objects));
			dictionary.Add(needsRenderingDictionaryKey, needsRendering, false);
			if (names != null)
				dictionary.Add(NamesDictionaryKey, names.Write(objects));
			return dictionary;
		}
		void IPdfBookmarkParent.Invalidate() {
			bookmarksChanged = true;
		}
	}
}
