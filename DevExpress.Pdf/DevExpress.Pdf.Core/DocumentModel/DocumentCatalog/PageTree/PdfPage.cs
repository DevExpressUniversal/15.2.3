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
using DevExpress.Pdf.Localization;
namespace DevExpress.Pdf {
	public class PdfPage : PdfPageTreeObject {
		static PdfStream ConvertStream(PdfReaderStream stream) {
			PdfDictionary dictionary = new PdfDictionary();
			object filterValue;
			if (stream.Dictionary.TryGetValue(PdfReaderStream.FilterDictionaryKey, out filterValue))
				dictionary.Add(PdfReaderStream.FilterDictionaryKey, filterValue);
			if (stream.Dictionary.TryGetValue(PdfReaderStream.DecodeParametersDictionaryKey, out filterValue))
				dictionary.Add(PdfReaderStream.DecodeParametersDictionaryKey, filterValue);
			PdfStream result = new PdfStream(dictionary, stream.DecryptedData);
			return result;
		}
		internal const string PageTreeNodeType = "Page";
		const string lastModifiedDictionaryKey = "LastModified";
		const string bleedBoxDictionaryKey = "BleedBox";
		const string trimBoxDictionaryKey = "TrimBox";
		const string artBoxDictionaryKey = "ArtBox";
		const string contentsDictionaryKey = "Contents";
		const string groupDictionaryKey = "Group";
		const string thumbDictionaryKey = "Thumb";
		const string articleBeadsDictionaryKey = "B";
		const string displayDurationDictionaryKey = "Dur";
		const string transDictionaryKey = "Trans";
		const string annotsDictionaryKey = "Annots";
		const string additionalActionsDicrtionaryKey = "AA";
		const string structParentsDictionaryKey = "StructParents";
		const string idDictionaryKey = "ID";
		const string preferredZoomDictionaryKey = "PZ";
		const string tabsDictionaryKey = "Tabs";
		const string userUnitDictionaryKey = "UserUnit";
		const byte space = (byte)' ';
		readonly double userUnit = 1;
		readonly List<PdfAnnotation> annotations = new List<PdfAnnotation>();
		readonly PdfPageActions actions;
		PdfRectangle bleedBox;
		PdfRectangle trimBox;
		PdfRectangle artBox;
		DateTimeOffset? lastModified;
		PdfTransparencyGroup transparencyGroup;
		PdfImage thumbnailImage;
		IList<int> articleBeadsObjectNumbers;
		PdfBead[] articleBeads;
		double displayDuration = -1;
		PdfPagePresentation trans;
		PdfMetadata metadata;
		int? structParents;
		Dictionary<string, PdfPieceInfoEntry> pieceInfo;
		byte[] id;
		double? preferredZoomFactor;
		PdfAnnotationTabOrder annotationTabOrder;
		IList<PdfReaderStream> contents = new List<PdfReaderStream>();
		bool hasFunctionalLimits;
		WeakReference commands;
		PdfReaderDictionary dictionary;
		bool ensuredAnnotations = false;
		bool ensured = false;
		public double UserUnit { get { return userUnit; } }
		public IList<PdfAnnotation> Annotations { 
			get {
				EnsureAnnotations();
				return annotations; 
			} 
		}
		public PdfPageActions Actions { get { return actions; } }
		public PdfRectangle BleedBox {
			get { return bleedBox ?? CropBox; }
			set {
				CheckBox(value, PdfCoreStringId.MsgIncorrectPageBleedBox);
				bleedBox = value;
			}
		}
		public PdfRectangle TrimBox {
			get { return trimBox ?? CropBox; }
			set {
				CheckBox(value, PdfCoreStringId.MsgIncorrectPageTrimBox);
				trimBox = value;
			}
		}
		public PdfRectangle ArtBox {
			get { return artBox ?? CropBox; }
			set {
				CheckBox(value, PdfCoreStringId.MsgIncorrectPageArtBox);
				artBox = value;
			}
		}
		public DateTimeOffset? LastModified {
			get {
				EnsurePage();
				return lastModified;
			}
			set {
				EnsurePage();
				lastModified = value; 
			}
		}
		public PdfCommandList Commands {
			get {
				if (commands != null) {
					object target = commands.Target;
					if (commands.IsAlive)
						return (PdfCommandList)target;
				}
				EnsurePage();
				PdfCommandList cmds;
				if (contents == null)
					cmds = new PdfCommandList();
				else {
					List<byte> data = new List<byte>();
					foreach (PdfReaderStream stream in contents) {
						data.AddRange(stream.GetData(true));
						data.Add(space);
					}
					cmds = PdfContentStreamParser.GetContent(Resources, data.ToArray());
				}
				commands = new WeakReference(cmds);
				return cmds;
			}
		}
		public PdfTransparencyGroup TransparencyGroup {
			get {
				EnsurePage();
				return transparencyGroup;
			}
		}
		public PdfImage ThumbnailImage {
			get {
				EnsurePage();
				return thumbnailImage;
			}
		}
		public IList<PdfBead> ArticleBeads {
			get {
				EnsurePage();
				if (articleBeads == null && articleBeadsObjectNumbers != null) {
					int count = articleBeadsObjectNumbers.Count;
					articleBeads = new PdfBead[count];
					PdfObjectCollection objects = DocumentCatalog.Objects;
					for (int i = 0; i < count; i++) {
						PdfBead bead = objects.GetResolvedObject<PdfBead>(articleBeadsObjectNumbers[i], true);
						if (bead == null)
							articleBeads[i] = null;
						else {
							if (bead.Page != this)
								PdfDocumentReader.ThrowIncorrectDataException();
							articleBeads[i] = bead;
						}
					}
					articleBeadsObjectNumbers = null;
				}
				return articleBeads;
			}
		}
		public double DisplayDuration {
			get {
				EnsurePage();
				return displayDuration;
			}
		}
		public PdfPagePresentation Trans {
			get {
				EnsurePage();
				return trans;
			}
		}
		public PdfMetadata Metadata {
			get {
				EnsurePage();
				return metadata;
			}
		}
		public int? StructParents {
			get {
				EnsurePage();
				return structParents;
			}
		}
		public byte[] ID {
			get {
				EnsurePage();
				return id;
			}
		}
		public double? PreferredZoomFactor {
			get {
				EnsurePage();
				return preferredZoomFactor;
			}
		}
		public PdfAnnotationTabOrder AnnotationTabOrder {
			get {
				EnsurePage();
				return annotationTabOrder;
			}
		}
		public IDictionary<string, PdfPieceInfoEntry> PieceInfo {
			get {
				EnsurePage();
				return pieceInfo;
			}
		}
		internal bool HasFunctionalLimits {
			get {
				EnsurePage();
				return hasFunctionalLimits;
			}
			set { hasFunctionalLimits = value; }
		}
		internal PdfPage(PdfDocumentCatalog documentCatalog, PdfRectangle mediaBox, PdfRectangle cropBox, int rotate) : base(documentCatalog, null, mediaBox, cropBox, rotate) {
		}
		internal PdfPage(PdfPageTreeNode parent, PdfReaderDictionary dictionary) : base(parent, dictionary) {
			this.dictionary = dictionary;
			if (dictionary.GetName(PdfDictionary.DictionaryTypeKey) != PageTreeNodeType || Resources == null || MediaBox == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			userUnit = dictionary.GetNumber(userUnitDictionaryKey) ?? 1.0;
			bleedBox = dictionary.GetRectangle(bleedBoxDictionaryKey);
			trimBox = dictionary.GetRectangle(trimBoxDictionaryKey);
			artBox = dictionary.GetRectangle(artBoxDictionaryKey);
			PdfObjectCollection objects = dictionary.Objects;
			PdfReaderDictionary aa = dictionary.GetDictionary(additionalActionsDicrtionaryKey);
			if (aa != null)
				actions = new PdfPageActions(aa);
			IList<object> articleBeadsArray = dictionary.GetArray(articleBeadsDictionaryKey);
			if (articleBeadsArray != null) {
				articleBeadsObjectNumbers = new List<int>();
				foreach (object bead in articleBeadsArray)
					if (bead == null)
						articleBeadsObjectNumbers.Add(-1);
					else {
						PdfObjectReference beadReference = bead as PdfObjectReference;
						if (beadReference == null)
							PdfDocumentReader.ThrowIncorrectDataException();
						else
							articleBeadsObjectNumbers.Add(beadReference.Number);
					}
			}
			UpdateBoxes();
		}
		internal void EnsureAnnotations() {
			if (!ensuredAnnotations && dictionary != null) {
				ensuredAnnotations = true;
				IList<object> annots = dictionary.GetArray(annotsDictionaryKey);
				if (annots != null)
					foreach (object annot in annots)
						annotations.Add(dictionary.Objects.GetAnnotation(this, annot));
				if (ensured)
					dictionary = null;
			}
		}
		internal void EnsurePage() {
			if (!ensured && dictionary != null) {
				ensured = true;
				PdfObjectCollection objects = dictionary.Objects;
				lastModified = dictionary.GetDate(lastModifiedDictionaryKey);
				object value;
				if (dictionary.TryGetValue(contentsDictionaryKey, out value)) {
					value = objects.TryResolve(value);
					PdfReaderStream stream = value as PdfReaderStream;
					if (stream == null) {
						IList<object> array = value as IList<object>;
						if (array == null)
							PdfDocumentReader.ThrowIncorrectDataException();
						foreach (object obj in array) {
							stream = objects.TryResolve(obj) as PdfReaderStream;
							if (stream == null)
								PdfDocumentReader.ThrowIncorrectDataException();
							contents.Add(stream);
						}
					}
					else
						contents.Add(stream);
				}
				PdfReaderDictionary groupDictionary = dictionary.GetDictionary(groupDictionaryKey);
				if (groupDictionary != null)
					transparencyGroup = new PdfTransparencyGroup(groupDictionary);
				if (dictionary.TryGetValue(thumbDictionaryKey, out value)) {
					PdfXObject xObject = objects.GetXObject(value, null, PdfImage.Type);
					if (xObject != null) {
						thumbnailImage = xObject as PdfImage;
						if (thumbnailImage == null)
							PdfDocumentReader.ThrowIncorrectDataException();
					}
				}
				displayDuration = dictionary.GetNumber(displayDurationDictionaryKey) ?? -1;
				PdfReaderDictionary presentationDict = dictionary.GetDictionary(transDictionaryKey);
				if (presentationDict != null)
					trans = new PdfPagePresentation(presentationDict);
				metadata = dictionary.GetMetadata();
				pieceInfo = PdfPieceInfoEntry.Parse(dictionary);
				structParents = dictionary.GetInteger(structParentsDictionaryKey);
				id = dictionary.GetBytes(idDictionaryKey);
				preferredZoomFactor = dictionary.GetNumber(preferredZoomDictionaryKey);
				annotationTabOrder = PdfEnumToStringConverter.Parse<PdfAnnotationTabOrder>(dictionary.GetName(tabsDictionaryKey));
				if(ensuredAnnotations)
					dictionary = null;
			}
		}
		internal PdfPoint FromUserSpace(PdfPoint point, double factorX, double factorY, int angle) {
			PdfRectangle cropBox = CropBox;
			switch (NormalizeRotate(angle + Rotate)) {
				case 90:
					return new PdfPoint(point.Y / factorY, point.X / factorX);
				case 180:
					return new PdfPoint(cropBox.Width - point.X / factorX, point.Y / factorY);
				case 270:
					return new PdfPoint(cropBox.Width - point.Y / factorY, cropBox.Height - point.X / factorX);
				default:
					return new PdfPoint(point.X / factorX, cropBox.Height - point.Y / factorY);
			}
		}
		internal PdfPoint ToUserSpace(PdfPoint point, double factorX, double factorY, int angle) {
			PdfRectangle cropBox = CropBox;
			switch (NormalizeRotate(angle + Rotate)) {
				case 90:
					return new PdfPoint(point.Y * factorX, point.X * factorY);
				case 180:
					return new PdfPoint((cropBox.Width - point.X) * factorX, point.Y * factorY);
				case 270:
					return new PdfPoint((cropBox.Height - point.Y) * factorX, (cropBox.Width - point.X) * factorY);
				default:
					return new PdfPoint(point.X * factorX, (cropBox.Height - point.Y) * factorY);
			}
		}
		internal PdfPoint GetTopLeftCorner(int rotationAngle) {
			PdfRectangle cropBox = CropBox;
			switch (NormalizeRotate(rotationAngle + Rotate)) {
				case 90:
					return new PdfPoint(cropBox.Left, cropBox.Bottom);
				case 180:
					return cropBox.BottomRight;
				case 270:
					return new PdfPoint(cropBox.Right, cropBox.Top);
				default:
					return cropBox.TopLeft;
			}
		}
		internal PdfPoint GetSize(int rotationAngle) {
			PdfRectangle cropBox = CropBox;
			double width;
			double height;
			int rotate = PdfPageTreeNode.NormalizeRotate(Rotate + rotationAngle);
			if (rotate == 90 || rotate == 270) {
				width = cropBox.Height;
				height = cropBox.Width;
			}
			else {
				width = cropBox.Width;
				height = cropBox.Height;
			}
			return new PdfPoint(Math.Max(1.0, width * userUnit), Math.Max(1.0, height * userUnit));
		}
		internal void ReplaceCommands(PdfCommandList commands) {
			EnsurePage();
			contents = new PdfReaderStream[] { commands.ToReaderStream(Resources) };
			this.commands = new WeakReference(commands);
		}
		protected override void UpdateBoxes() {
			base.UpdateBoxes();
			PdfRectangle mediaBox = MediaBox;
			PdfRectangle actualBleedBox = BleedBox;
			if (!mediaBox.Contains(actualBleedBox))
				BleedBox = mediaBox.Trim(actualBleedBox);
			PdfRectangle actualTrimBox = TrimBox;
			if (!mediaBox.Contains(actualTrimBox))
				TrimBox = mediaBox.Trim(actualTrimBox);
			PdfRectangle actualArtBox = ArtBox;
			if (!mediaBox.Contains(actualArtBox))
				ArtBox = mediaBox.Trim(actualArtBox);
		}
		protected override PdfWriterDictionary CreateDictionary(PdfObjectCollection objects) {
			EnsurePage();
			EnsureAnnotations();
			PdfWriterDictionary dictionary = base.CreateDictionary(objects);
			dictionary.Add(PdfDictionary.DictionaryTypeKey, new PdfName(PageTreeNodeType));
			dictionary.AddNullable(lastModifiedDictionaryKey, lastModified);
			PdfRectangle cropBox = CropBox;
			dictionary.Add(bleedBoxDictionaryKey, BleedBox, cropBox);
			dictionary.Add(trimBoxDictionaryKey, TrimBox, cropBox);
			dictionary.Add(artBoxDictionaryKey, ArtBox, cropBox);
			int contentsCount = contents.Count;
			if (contentsCount > 1)
				dictionary.Add(contentsDictionaryKey, new PdfWritableConvertableArray(contents, stream => objects.AddStream(ConvertStream((PdfReaderStream)stream))));
			else if (contentsCount == 1)
				dictionary.Add(contentsDictionaryKey, ConvertStream(contents[0]));
			dictionary.Add(groupDictionaryKey, transparencyGroup);
			dictionary.Add(thumbDictionaryKey, thumbnailImage);
			dictionary.Add(displayDurationDictionaryKey, displayDuration, -1);
			if (trans != null)
				dictionary.Add(transDictionaryKey, trans.Write());
			dictionary.AddList(annotsDictionaryKey, annotations);
			PdfPieceInfoEntry.WritePieceInfo(dictionary, pieceInfo);
			if (actions != null) {
				PdfWriterDictionary aa = new PdfWriterDictionary(objects);
				aa.Add(PdfPageActions.OpenedKey, Actions.Opened);
				aa.Add(PdfPageActions.ClosedKey, Actions.Closed);
				dictionary.Add(additionalActionsDicrtionaryKey, aa);
			}
			dictionary.AddNullable(structParentsDictionaryKey, structParents);
			dictionary.Add(idDictionaryKey, id, null);
			dictionary.AddNullable(preferredZoomDictionaryKey, preferredZoomFactor);
			if (annotationTabOrder != PdfAnnotationTabOrder.RowOrder)
				dictionary.AddEnumName(tabsDictionaryKey, annotationTabOrder);
			dictionary.Add(userUnitDictionaryKey, userUnit, 1);
			dictionary.AddList(articleBeadsDictionaryKey, ArticleBeads);
			return dictionary;
		}
	}
}
