#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.API.Native.Implementation;
using DevExpress.Office.Utils;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.ComponentModel.Design;
namespace DevExpress.XtraRichEdit.API.Native {
	#region DocumentImage
	[ComVisible(true)]
	public interface DocumentImage {
		DocumentRange Range { get; }
		float ScaleX { get; set; }
		float ScaleY { get; set; }
		SizeF OriginalSize { get; }
		SizeF Size { get; set; }
		string Uri { get; set; }
		OfficeImage Image { get; }
	}
	#endregion
	#region ReadOnlyDocumentImageCollection
	[ComVisible(true)]
	public interface ReadOnlyDocumentImageCollection : ISimpleCollection<DocumentImage> {
		ReadOnlyDocumentImageCollection Get(DocumentRange range);
	}
	#endregion
	#region DocumentImageCollection
	[ComVisible(true)]
	public interface DocumentImageCollection : ReadOnlyDocumentImageCollection {
		DocumentImage Insert(DocumentPosition pos, DocumentImageSource imageSource);
		DocumentImage Append(DocumentImageSource imageSource);
#if !SL
		DocumentImage Insert(DocumentPosition pos, Image image);
		DocumentImage Append(Image image);
#endif
	}
	#endregion
	#region DocumentImageSource (abstract class)
	[ComVisible(true)]
	public abstract class DocumentImageSource {
#if !SL
		[ComVisible(false)]
		public static DocumentImageSource FromFile(string fileName) {
			return new FileDocumentImageSource(fileName);
		}
		[ComVisible(false)]
		public static DocumentImageSource FromImage(Image image) {
			return new ImageDocumentImageSource(image);
		}
#endif
		[ComVisible(false)]
		public static DocumentImageSource FromStream(Stream stream) {
			return new StreamDocumentImageSource(stream);
		}
		[ComVisible(false)]
		public static DocumentImageSource FromUri(string uri, IServiceContainer serviceContainer) {
			return new UriDocumentImageSource(uri);
		}
		protected internal abstract OfficeImage CreateImage(DevExpress.XtraRichEdit.Model.DocumentModel documentModel);
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.API.Native.Implementation {
	using ModelPosition = DevExpress.XtraRichEdit.Model.DocumentModelPosition;
	using ModelRunIndex = DevExpress.XtraRichEdit.Model.RunIndex;
	using ModelInlinePictureRun = DevExpress.XtraRichEdit.Model.InlinePictureRun;
	using ModelFloatingObjectAnchorRun = DevExpress.XtraRichEdit.Model.FloatingObjectAnchorRun;
	using ModelPictureFloatingObjectContent = DevExpress.XtraRichEdit.Model.PictureFloatingObjectContent;
	using ModelPictureContainerRun = DevExpress.XtraRichEdit.Model.IPictureContainerRun;
	using ModelRectangularScalableObject = DevExpress.XtraRichEdit.Model.IRectangularScalableObject;
	using DevExpress.XtraRichEdit.API.Internal;
	using DevExpress.XtraRichEdit.Services;
	using System.Collections.Generic;
	using System.Collections;
	using ModelPieceTable = DevExpress.XtraRichEdit.Model.PieceTable;
	using ModelDocumentModel = DevExpress.XtraRichEdit.Model.DocumentModel;
	using ModelLogPosition = DevExpress.XtraRichEdit.Model.DocumentLogPosition;
	using ModelRunInfo = DevExpress.XtraRichEdit.Model.RunInfo;
	using DevExpress.XtraRichEdit.Model;
	using Office.Utils.Internal;
	using Compatibility.System.Drawing;
	#region NativeDocumentImage
	public class NativeDocumentImage : DocumentImage {
		readonly NativeSubDocument document;
		readonly NativeDocumentRange range;
		public static NativeDocumentImage TryCreate(NativeSubDocument document, DevExpress.XtraRichEdit.Model.TextRunBase run, ModelRunIndex runIndex) {
			if (run is ModelInlinePictureRun)
				return new NativeDocumentImage(document, runIndex);
			ModelFloatingObjectAnchorRun anchorRun = run as ModelFloatingObjectAnchorRun;
			if (anchorRun != null) {
				ModelPictureFloatingObjectContent content = anchorRun.Content as ModelPictureFloatingObjectContent;
				if (content != null)
					return new NativeDocumentImage(document, runIndex);
			}
			return null;
		}
		public static bool CanCreateImage(NativeSubDocument document, DevExpress.XtraRichEdit.Model.TextRunBase run) {
			if (run is ModelInlinePictureRun)
				return true;
			ModelFloatingObjectAnchorRun anchorRun = run as ModelFloatingObjectAnchorRun;
			if (anchorRun != null) {
				ModelPictureFloatingObjectContent content = anchorRun.Content as ModelPictureFloatingObjectContent;
				if (content != null)
					return true;
			}
			return false;
		}
		public static NativeDocumentImage CreateUnsafe(NativeSubDocument document, ModelRunIndex runIndex) {
			return new NativeDocumentImage(document, runIndex);
		}
		NativeDocumentImage(NativeSubDocument document, ModelRunIndex runIndex) {
			Guard.ArgumentNotNull(document, "document");
			this.document = document;
			ModelPosition start = ModelPosition.FromRunStart(document.PieceTable, runIndex);
			ModelPosition end = start.Clone();
			end.LogPosition = start.LogPosition + 1;
			this.range = new NativeDocumentRange(document, start, end);
		}
		ModelPictureContainerRun InlinePictureRun { get { return (ModelPictureContainerRun)document.PieceTable.Runs[range.Start.Position.RunIndex]; } }
		ModelRectangularScalableObject RectangleScalableObject { get { return (ModelRectangularScalableObject)document.PieceTable.Runs[range.Start.Position.RunIndex]; } }
		#region DocumentImage Members
		public DocumentRange Range { get { return range; } }
		public float ScaleX {
			get { return RectangleScalableObject.ScaleX / 100.0f; }
			set { RectangleScalableObject.ScaleX = value * 100.0f; }
		}
		public float ScaleY {
			get { return RectangleScalableObject.ScaleY / 100.0f; }
			set { RectangleScalableObject.ScaleY = value * 100.0f; }
		}
		public SizeF OriginalSize {
			get {
				Size originalSize = RectangleScalableObject.OriginalSize;
				return new SizeF(document.ModelUnitsToUnits(originalSize.Width), document.ModelUnitsToUnits(originalSize.Height));
			}
		}
		public SizeF Size {
			get {
				SizeF actualSize = InlinePictureRun.ActualSizeF;
				return new SizeF(document.ModelUnitsToUnitsF(actualSize.Width), document.ModelUnitsToUnitsF(actualSize.Height));
			}
			set {
				int width = Math.Max(1, document.UnitsToModelUnits(value.Width));
				int height = Math.Max(1, document.UnitsToModelUnits(value.Height));
				InlinePictureRun.ActualSize = new Size(width, height);
			}
		}
		public OfficeImage Image {
			get {
				OfficeImage result = InlinePictureRun.PictureContent.Image;
#if !SL // B201030 - RichEditControl freezes when trying to immediately access image created using DocumentImageSource.FromUri
				InternalOfficeImageHelper.EnsureLoadComplete(result);
#endif
				return result;
			}
		}
		public string Uri { get { return Image.Uri; } set { Image.Uri = value; } }
		#endregion
	}
	#endregion
	#region NativeDocumentImageCollection
	public class NativeDocumentImageCollection : DocumentImageCollection {
		readonly List<DocumentImage> innerList = new List<DocumentImage>();
		readonly NativeSubDocument document;
		public NativeDocumentImageCollection(NativeSubDocument document) {
			Guard.ArgumentNotNull(document, "document");
			this.document = document;
		}
		ModelPieceTable PieceTable { get { return document.PieceTable; } }
		ModelDocumentModel DocumentModel { get { return PieceTable.DocumentModel; } }
		public List<DocumentImage> InnerList { get { return innerList; } }
		#region ISimpleCollection<DocumentImage> Members
		DocumentImage ISimpleCollection<DocumentImage>.this[int index] { 
			get {
				int picterRunCount = 0;
				DocumentImage result = null;
				ProcessImages((run, runIndex) => {
					picterRunCount++;
					if (picterRunCount - 1 == index) {
						result = NativeDocumentImage.TryCreate(document, run, new RunIndex(runIndex));
						return false;
					}
					return true;
				});
				return result;
			} 
		}
		#endregion
		void ProcessImages(Func<TextRunBase, int, bool> action) {
			TextRunCollection runs = PieceTable.Runs;
			int count = runs.Count;
			for (int i = 0; i < count; i++) {
				TextRunBase run = runs[new RunIndex(i)];
				if(NativeDocumentImage.CanCreateImage(document, run)) {
					if (!action(run, i))
						break;
				}
			}
		}
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return ((IEnumerable<DocumentImage>)this).GetEnumerator();
		}
		#endregion
		#region IEnumerable<DocumentImage> Members
		IEnumerator<DocumentImage> IEnumerable<DocumentImage>.GetEnumerator() {
			TextRunCollection runs = PieceTable.Runs;
			int count = runs.Count;
			for (int i = 0; i < count; i++) {
				InlinePictureRun picterRun = runs[new RunIndex(i)] as InlinePictureRun;
				if (picterRun != null)
					yield return NativeDocumentImage.CreateUnsafe(document, new RunIndex(i));
			}
		}
		#endregion
		#region ICollection Members
		void ICollection.CopyTo(Array array, int index) {
			List<DocumentImage> images = new List<DocumentImage>();
			ProcessImages((run, runIndex) => {
				images.Add(NativeDocumentImage.CreateUnsafe(document, new RunIndex(runIndex)));
				return true;
			});
			Array.Copy(images.ToArray(), 0, array, index, images.Count);
		}
		int ICollection.Count { 
			get {
				int result = 0;
				ProcessImages((run, runIndex) => {
					result++;
					return true;
				});
				return result;
			}
		}
		bool ICollection.IsSynchronized {
			get {
				return false;
			}
		}
		object ICollection.SyncRoot {
			get {
				return this;
			}
		}
		#endregion
		public DocumentImage Insert(DocumentPosition pos, DocumentImageSource imageSource) {
			document.CheckValid();
			document.CheckDocumentPosition(pos);
			OfficeImage image = imageSource.CreateImage(DocumentModel);
			if (image == null)
				return null;
			ModelRunInfo runInfo = new ModelRunInfo(PieceTable);
			ModelLogPosition logPosition = document.NormalizeLogPosition(pos.LogPosition);
			PieceTable.InsertInlinePicture(logPosition, image);
			PieceTable.CalculateRunInfoStart(logPosition, runInfo);
			return NativeDocumentImage.CreateUnsafe(document, runInfo.Start.RunIndex);
		}
		public DocumentImage Append(DocumentImageSource imageSource) {
			return Insert(document.EndPosition, imageSource);
		}
#if !SL
		public DocumentImage Insert(DocumentPosition pos, Image image) {
			return Insert(pos, DocumentImageSource.FromImage(image));
		}
		public DocumentImage Append(Image image) {
			return Insert(document.EndPosition, image);
		}
#endif
		public ReadOnlyDocumentImageCollection Get(DocumentRange range) {
			document.CheckValid();
			document.CheckDocumentRange(range);
			NativeDocumentRange nativeRange = (NativeDocumentRange)range;
			ModelRunIndex firstRunIndex = nativeRange.NormalizedStart.Position.RunIndex;
			ModelRunIndex lastRunIndex = nativeRange.NormalizedEnd.Position.RunIndex;
			if (nativeRange.NormalizedEnd.Position.RunOffset == 0)
				lastRunIndex--;
			NativeReadOnlyDocumentImageCollection result = new NativeReadOnlyDocumentImageCollection();
			for (ModelRunIndex i = firstRunIndex; i <= lastRunIndex; i++) {
				NativeDocumentImage image = NativeDocumentImage.TryCreate(document, PieceTable.Runs[i], i);
				if (image != null)
					result.Add(image);
			}
			return result;
		}
	}
	#endregion
	public class NativeReadOnlyDocumentImageCollection : NativeReadOnlyCollection<DocumentImage, NativeReadOnlyDocumentImageCollection, ReadOnlyDocumentImageCollection>, ReadOnlyDocumentImageCollection {
		protected override NativeReadOnlyDocumentImageCollection CreateCollection() {
			return new NativeReadOnlyDocumentImageCollection();
		}
		protected override bool Contains(DocumentRange range, DocumentImage item) {
			DocumentPosition start = item.Range.Start;
			return range.Contains(start);
		}
	}
	#region StreamDocumentImageSource
	public class StreamDocumentImageSource : DocumentImageSource {
		readonly Stream stream;
		public StreamDocumentImageSource(Stream stream) {
			Guard.ArgumentNotNull(stream, "stream");
			this.stream = stream;
		}
		public Stream Stream { get { return stream; } }
		protected internal override OfficeImage CreateImage(DevExpress.XtraRichEdit.Model.DocumentModel documentModel) {
			return documentModel.CreateImage(stream);
		}
	}
	#endregion
	#region UriDocumentImageSource
	public class UriDocumentImageSource : DocumentImageSource {
		readonly string uri;
		public UriDocumentImageSource(string uri) {
			Guard.ArgumentNotNull(uri, "uri");
			this.uri = uri;
		}
		public string Uri { get { return uri; } }
		protected internal override OfficeImage CreateImage(DevExpress.XtraRichEdit.Model.DocumentModel documentModel) {
			return new UriBasedOfficeImage(uri, 0, 0, documentModel, true);
		}
	}
	#endregion
#if !SL
	#region FileDocumentImageSource
	public class FileDocumentImageSource : StreamDocumentImageSource {
		public FileDocumentImageSource(string fileName)
			: base(new FileStream(fileName, FileMode.Open, FileAccess.Read)) {
		}
	}
	#endregion
	#region ImageDocumentImageSource
	public class ImageDocumentImageSource : DocumentImageSource {
		readonly Image image;
		public ImageDocumentImageSource(Image image) {
			Guard.ArgumentNotNull(image, "image");
			this.image = image;
		}
		protected internal override OfficeImage CreateImage(DevExpress.XtraRichEdit.Model.DocumentModel documentModel) {
			return documentModel.CreateImage(image);
		}
	}
	#endregion
#endif
}
