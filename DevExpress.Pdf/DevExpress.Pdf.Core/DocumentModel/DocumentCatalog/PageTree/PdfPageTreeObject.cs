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
using DevExpress.Pdf.Native;
using DevExpress.Pdf.Localization;
namespace DevExpress.Pdf {
	public abstract class PdfPageTreeObject : PdfObject {
		const string parentDictionaryKey = "Parent";
		const string resourcesDictionaryKey = "Resources";
		const string mediaBoxDictionaryKey = "MediaBox";
		const string cropBoxDictionaryKey = "CropBox";
		const string rotateDictionaryKey = "Rotate";
		internal static int NormalizeRotate(int rotate) {
			int step = -360;
			if (rotate < 0)
				step = 360;
			while ((rotate < 0 && step > 0) || (rotate > 270 && step < 0))
				rotate += step;
			return rotate;
		}
		internal static bool CheckRotate(int rotate) {
			return rotate == 0 || rotate == 90 || rotate == 180 || rotate == 270;
		}
		readonly PdfDocumentCatalog documentCatalog;
		readonly PdfPageResources resources;
		PdfPageTreeNode parent;
		PdfRectangle mediaBox;
		PdfRectangle cropBox;
		int rotate;
		internal PdfDocumentCatalog DocumentCatalog { get { return documentCatalog; } }
		internal PdfResources Resources { get { return resources; } }
		internal PdfPageTreeNode Parent {
			get { return parent; }
			set { parent = value; }
		}
		public PdfRectangle MediaBox {
			get { return mediaBox; }
			set {
				if (value == null)
					throw new ArgumentNullException();
				mediaBox = value;
				UpdateBoxes();
			}
		}
		public PdfRectangle CropBox {
			get { return cropBox ?? mediaBox; }
			set {
				CheckBox(value, PdfCoreStringId.MsgIncorrectPageCropBox);
				cropBox = value;
			}
		}
		public int Rotate {
			get { return rotate; }
			set {
				value = NormalizeRotate(value);
				if (!CheckRotate(value))
					throw new ArgumentOutOfRangeException("Rotate", PdfCoreLocalizer.GetString(PdfCoreStringId.MsgIncorrectPageRotate));
				rotate = value;
			}
		}
		protected PdfPageTreeObject(PdfDocumentCatalog documentCatalog, PdfPageTreeNode parent, PdfRectangle mediaBox, PdfRectangle cropBox, int rotate) {
			this.documentCatalog = documentCatalog;
			this.parent = parent;
			this.mediaBox = mediaBox;
			this.cropBox = cropBox;
			Rotate = rotate;
			CheckBox(cropBox, PdfCoreStringId.MsgIncorrectPageCropBox);
			resources = new PdfPageResources(documentCatalog, parent == null ? null : parent.resources, null);
		}
		protected PdfPageTreeObject(PdfPageTreeNode parent, PdfReaderDictionary dictionary) : base(dictionary.Number) {
			PdfObjectCollection objects = dictionary.Objects;
			this.documentCatalog = objects.DocumentCatalog;
			this.parent = parent;
			PdfPageResources parentResources = parent == null ? null : parent.resources;
			object resourcesValue;
			if (!dictionary.TryGetValue(resourcesDictionaryKey, out resourcesValue))
				resourcesValue = null;
			resources = objects.GetPageResources(resourcesValue, parentResources);
			mediaBox = dictionary.GetRectangle(mediaBoxDictionaryKey);
			cropBox = dictionary.GetRectangle(cropBoxDictionaryKey);
			PdfObjectReference parentReference = dictionary.GetObjectReference(parentDictionaryKey);
			bool noParent = parent == null;
			if (noParent) {
				if (parentReference != null)
					PdfDocumentReader.ThrowIncorrectDataException();
			}
			else {
				if (mediaBox == null)
					mediaBox = parent.mediaBox;
				if (cropBox == null)
					cropBox = parent.cropBox;
			}
			if (mediaBox != null)
				mediaBox = PdfRectangle.InflateToNonEmpty(mediaBox);
			int rotate = NormalizeRotate(dictionary.GetInteger(rotateDictionaryKey) ?? (noParent ? 0 : parent.rotate));
			if (!CheckRotate(rotate))
				PdfDocumentReader.ThrowIncorrectDataException();
			this.rotate = rotate;
			UpdateBoxes();
		}
		protected void CheckBox(PdfRectangle box, PdfCoreStringId messageId) {
			if (box != null && !mediaBox.Contains(box))
				throw new ArgumentOutOfRangeException(PdfCoreLocalizer.GetString(messageId));
		}
		protected virtual void UpdateBoxes() {
			PdfRectangle actualCropBox = CropBox;
			if (mediaBox != null && actualCropBox != null && !mediaBox.Contains(actualCropBox))
				cropBox = mediaBox.Trim(actualCropBox);
		}
		protected virtual PdfWriterDictionary CreateDictionary(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(objects);
			if (parent == null) {
				dictionary.Add(mediaBoxDictionaryKey, mediaBox);
				dictionary.Add(cropBoxDictionaryKey, cropBox, mediaBox);
				dictionary.Add(rotateDictionaryKey, rotate, 0);
			}
			else {
				PdfPageTreeNode node = objects.DocumentCatalog.Pages.GetPageNode(objects, false);
				dictionary.Add(parentDictionaryKey, new PdfObjectReference(node.ObjectNumber));
				dictionary.Add(mediaBoxDictionaryKey, mediaBox, node.mediaBox);
				dictionary.Add(cropBoxDictionaryKey, CropBox, node.CropBox);
				dictionary.Add(rotateDictionaryKey, rotate, node.Rotate);
			}
			dictionary.Add(resourcesDictionaryKey, resources);
			return dictionary;
		}
		protected internal override object ToWritableObject(PdfObjectCollection objects) {
			return CreateDictionary(objects);
		}
	}
}
