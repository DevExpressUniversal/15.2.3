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
using DevExpress.Office;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.API.Native.Implementation;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.ComponentModel.Design;
namespace DevExpress.Spreadsheet {
	public interface ShapeCollection : ISimpleCollection<Shape> {
#if !SL && !DXPORTABLE
		[Obsolete("Use the Worksheet.Pictures.AddPicture(Image image, Cell topLeftCell) method instead.", false)]
		Picture AddPicture(Image image, Cell topLeftCell);
		[Obsolete("Use the Worksheet.Pictures.AddPicture(string filename, Cell topLeftCell) method instead.", false)]
		Picture AddPicture(string filename, Cell topLeftCell);
		[Obsolete("Use the Worksheet.Pictures.AddPicture(Image image, Range targetRange) method instead.", false)]
		Picture AddPicture(Image image, Range targetRange);
		[Obsolete("Use the Worksheet.Pictures.AddPicture(Image image, Range targetRange, bool lockAspectRatio) method instead.", false)]
		Picture AddPicture(Image image, Range targetRange, bool lockAspectRatio);
		[Obsolete("Use the Worksheet.Pictures.AddPicture(string filename, Range targetRange) method instead.", false)]
		Picture AddPicture(string filename, Range targetRange);
		[Obsolete("Use the Worksheet.Pictures.AddPicture(string filename, Range targetRange, bool lockAspectRatio) method instead.", false)]
		Picture AddPicture(string filename, Range targetRange, bool lockAspectRatio);
		[Obsolete("Use the Worksheet.Pictures.AddPicture(string filename, float x, float y) method instead.", false)]
		Picture AddPicture(string filename, float x, float y);
		[Obsolete("Use the Worksheet.Pictures.AddPicture(string filename, float x, float y, float width, float height) method instead.", false)]
		Picture AddPicture(string filename, float x, float y, float width, float height);
		[Obsolete("Use the Worksheet.Pictures.AddPicture(string filename, float x, float y, float width, float height, bool lockAspectRatio) method instead.", false)]
		Picture AddPicture(string filename, float x, float y, float width, float height, bool lockAspectRatio);
		[Obsolete("Use the Worksheet.Pictures.AddPicture(Image image, float x, float y) method instead.", false)]
		Picture AddPicture(Image image, float x, float y);
		[Obsolete("Use the Worksheet.Pictures.AddPicture(Image image, float x, float y, float width, float height) method instead.", false)]
		Picture AddPicture(Image image, float x, float y, float width, float height);
		[Obsolete("Use the Worksheet.Pictures.AddPicture(Image image, float x, float y, float width, float height, bool lockAspectRatio) method instead.", false)]
		Picture AddPicture(Image image, float x, float y, float width, float height, bool lockAspectRatio);
#endif
		[Obsolete("Use the Worksheet.Pictures.AddPicture(SpreadsheetImageSource imageSource, Cell topLeftCell) method instead.", false)]
		Picture AddPicture(SpreadsheetImageSource imageSource, Cell topLeftCell);
		[Obsolete("Use the Worksheet.Pictures.AddPicture(SpreadsheetImageSource imageSource, Range targetRange) method instead.", false)]
		Picture AddPicture(SpreadsheetImageSource imageSource, Range targetRange);
		[Obsolete("Use the Worksheet.Pictures.AddPicture(SpreadsheetImageSource imageSource, Range targetRange, bool lockAspectRatio) method instead.", false)]
		Picture AddPicture(SpreadsheetImageSource imageSource, Range targetRange, bool lockAspectRatio);
		[Obsolete("Use the Worksheet.Pictures.AddPicture(SpreadsheetImageSource imageSource, float x, float y) method instead.", false)]
		Picture AddPicture(SpreadsheetImageSource imageSource, float x, float y);
		[Obsolete("Use the Worksheet.Pictures.AddPicture(SpreadsheetImageSource imageSource, float x, float y, float width, float height) method instead.", false)]
		Picture AddPicture(SpreadsheetImageSource imageSource, float x, float y, float width, float height);
		[Obsolete("Use the Worksheet.Pictures.AddPicture(SpreadsheetImageSource imageSource, float x, float y, float width, float height, bool lockAspectRatio) method instead.", false)]
		Picture AddPicture(SpreadsheetImageSource imageSource, float x, float y, float width, float height, bool lockAspectRatio);
		bool Contains(string shapeName);
		IList<Shape> GetShapesByName(string shapeName);
		Shape GetShapeById(int id);
		void Clear();
		void NormalizeZOrder();
		void RemoveAt(int index);
	}
#region SpreadsheetImageSource (abstract class)
	[ComVisible(true)]
	public abstract class SpreadsheetImageSource {
#if !SL
		[ComVisible(false)]
		public static SpreadsheetImageSource FromFile(string fileName) {
			return new FileDocumentImageSource(fileName);
		}
		[ComVisible(false)]
		public static SpreadsheetImageSource FromImage(Image image) {
			return new ImageDocumentImageSource((Image) image.Clone());
		}
#endif
		[ComVisible(false)]
		public static SpreadsheetImageSource FromStream(Stream stream) {
			return new StreamDocumentImageSource(stream);
		}
		[ComVisible(false)]
		public static SpreadsheetImageSource FromUri(string uri, IServiceContainer serviceContainer) {
			return new UriDocumentImageSource(uri);
		}
		protected internal abstract OfficeImage CreateImage(DevExpress.XtraSpreadsheet.Model.DocumentModel documentModel);
	}
#endregion
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Utils;
	using ModelCellPosition = DevExpress.XtraSpreadsheet.Model.CellPosition;
	using DevExpress.Spreadsheet;
	using System.Collections;
	partial class NativeShapeCollection : ShapeCollection {
		readonly NativeWorksheet worksheet;
		readonly List<NativeDrawingObject> innerList = new List<NativeDrawingObject>();
		public NativeShapeCollection(NativeWorksheet worksheet) {
			this.worksheet = worksheet;
		}
		public Shape this[int index] {
			get {
				return innerList[index];
			}
		}
		public bool Contains(string shapeName) {
			return GetShapeByName(shapeName) != null;
		}
		public IList<Shape> GetShapesByName(string shapeName) {
			List<Shape> result = new List<Shape>();
			for (int i = 0; i < Count; i++) {
				if ((innerList[i].Name == shapeName) && (innerList[i].IsValid))
					result.Add(this[i]);
			}
			return result;
		}
		public Shape GetShapeById(int id) {
			for (int i = 0; i < Count; i++) {
				if ((innerList[i].Id == id) && (innerList[i].IsValid))
					return this[i];
			}
			return null;
		}
		public int Count { get { return innerList.Count; } }
		public List<NativeDrawingObject> InnerList { get { return innerList; } }
		Shape GetShapeByName(string name) {
			for (int i = 0; i < Count; i++) {
				if ((innerList[i].Name == name) && (innerList[i].IsValid))
					return this[i];
			}
			return null;
		}
#if !SL
		public Picture AddPicture(Image image, Cell topLeftCell) {
			return AddPicture(SpreadsheetImageSource.FromImage(image), topLeftCell);
		}
		public Picture AddPicture(string fileName, Cell topLeftCell) {
			return AddPictureTopLeftAnchor(SpreadsheetImageSource.FromFile(fileName), topLeftCell);
		}
		public Picture AddPicture(Image image, Range targetRange) {
			return AddPicture(image, targetRange, false);
		}
		public Picture AddPicture(Image image, Range targetRange, bool lockAspectRatio) {
			return AddPicture(SpreadsheetImageSource.FromImage(image), targetRange, lockAspectRatio);
		}
		public Picture AddPicture(string filename, Range targetRange) {
			return AddPicture(filename, targetRange, false);
		}
		public Picture AddPicture(string filename, Range targetRange, bool lockAspectRatio) {
			return AddPicture(SpreadsheetImageSource.FromFile(filename), targetRange, lockAspectRatio);
		}
		public Picture AddPicture(string filename, float x, float y, float width, float height, bool lockAspectRatio) {
			return AddPicture(SpreadsheetImageSource.FromFile(filename), x, y, width, height, lockAspectRatio);
		}
		public Picture AddPicture(Image image, float x, float y) {
			return AddPicture(SpreadsheetImageSource.FromImage(image), x, y);
		}
		public Picture AddPicture(Image image, float x, float y, float width, float height) {
			return AddPicture(SpreadsheetImageSource.FromImage(image), x, y, width, height, false);
		}
		public Picture AddPicture(Image image, float x, float y, float width, float height, bool lockAspectRatio) {
			return AddPicture(SpreadsheetImageSource.FromImage(image), x, y, width, height, lockAspectRatio);
		}
		public Picture AddPicture(string filename, float x, float y) {
			return AddPicture(SpreadsheetImageSource.FromFile(filename), x, y);
		}
		public Picture AddPicture(string filename, float x, float y, float width, float height) {
			return AddPicture(SpreadsheetImageSource.FromFile(filename), x, y, width, height, false);
		}
#endif
		public Picture AddPicture(SpreadsheetImageSource imageSource, Cell topLeftCell) {
			return AddPictureTopLeftAnchor(imageSource, topLeftCell);
		}
		public Picture AddPicture(SpreadsheetImageSource imageSource, float x, float y) {
			Guard.ArgumentNotNull(imageSource, "imageSource");
			Guard.ArgumentNonNegative(x, "CoordinateX");
			Guard.ArgumentNonNegative(y, "CoordinateY");
			OfficeImage image = imageSource.CreateImage(worksheet.ModelWorkbook);
			float topOffset = UnitsToModelUnitsF(y);
			float leftOffset = UnitsToModelUnitsF(x);
			worksheet.ModelWorksheet.InsertPicture(image, leftOffset, topOffset);
			return (Picture)this[this.Count - 1];
		}
		public Picture AddPicture(SpreadsheetImageSource imageSource, float x, float y, float width, float height) {
			return AddPicture(imageSource, x, y, width, height, false);
		}
		public Picture AddPicture(SpreadsheetImageSource imageSource, float x, float y, float width, float height, bool lockAspectRatio) {
			Guard.ArgumentNotNull(imageSource, "imageSource");
			Guard.ArgumentNonNegative(x, "CoordinateX");
			Guard.ArgumentNonNegative(y, "CoordinateY");
			Guard.ArgumentPositive(width, "width");
			Guard.ArgumentPositive(height, "height");
			OfficeImage image = imageSource.CreateImage(worksheet.ModelWorkbook);
			float topOffset = UnitsToModelUnitsF(y);
			float leftOffset = UnitsToModelUnitsF(x);
			float modelWidth = UnitsToModelUnitsF(width);
			float modelHeight = UnitsToModelUnitsF(height);
			worksheet.ModelWorksheet.InsertPicture(image, leftOffset, topOffset, modelWidth, modelHeight, lockAspectRatio);
			return (Picture)this[this.Count - 1];
		}
		public Picture AddPicture(SpreadsheetImageSource imageSource, Range targetRange) {
			return AddPicture(imageSource, targetRange, false);
		}
		public Picture AddPicture(SpreadsheetImageSource imageSource, Range targetRange, bool lockAspectRatio) {
			Guard.ArgumentNotNull(imageSource, "imageSource");
			Guard.ArgumentNotNull(targetRange, "targetRange");
			if (!targetRange.Worksheet.Equals(worksheet))
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorUseRangeFromAnotherWorksheet);
			OfficeImage image = imageSource.CreateImage(worksheet.ModelWorkbook);
			int sheetId = worksheet.ModelWorksheet.SheetId;
			Model.CellKey topLeft = new Model.CellKey(sheetId, targetRange.LeftColumnIndex, targetRange.TopRowIndex);
			Model.CellKey bottomRight = new Model.CellKey(sheetId, targetRange.RightColumnIndex + 1, targetRange.BottomRowIndex + 1);
			worksheet.ModelWorksheet.InsertPicture(image, topLeft, bottomRight, lockAspectRatio);
			return (Picture)this[this.Count - 1];
		}
		Picture AddPictureTopLeftAnchor(SpreadsheetImageSource imageSource, Cell topLeftCell) {
			Guard.ArgumentNotNull(imageSource, "imageSource");
			Guard.ArgumentNotNull(topLeftCell, "topLeftCell");
			if (!topLeftCell.Worksheet.Equals(worksheet))
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorUseRangeFromAnotherWorksheet);
			OfficeImage image = imageSource.CreateImage(worksheet.ModelWorkbook);
			Model.CellKey topLeftCellKey = ((NativeCell)topLeftCell).ReadOnlyModelCell.Key;
			worksheet.ModelWorksheet.InsertPicture(image, topLeftCellKey);
			return (Picture)this[this.Count - 1];
		}
		protected float UnitsToModelUnitsF(float value) {
			return worksheet.NativeWorkbook.UnitsToModelUnitsF(value);
		}
		protected float ModelUnitsToUnitsF(float value) {
			return worksheet.NativeWorkbook.ModelUnitsToUnitsF(value);
		}
		public IEnumerator<Shape> GetEnumerator() {
			return new EnumeratorConverter<NativeDrawingObject, Shape>(innerList.GetEnumerator(), ConvertNativeDrawingObjectToShape);
		}
		Shape ConvertNativeDrawingObjectToShape(NativeDrawingObject item) {
			return item;
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return innerList.GetEnumerator();
		}
		#region ICollection members
		object ICollection.SyncRoot {
			get {
				ICollection collection = innerList;
				return collection.SyncRoot;
			}
		}
		bool ICollection.IsSynchronized {
			get {
				ICollection collection = innerList;
				return collection.IsSynchronized;
			}
		}
		void ICollection.CopyTo(Array array, int index) {
			Array.Copy(innerList.ToArray(), 0, array, index, innerList.Count);
		}
		#endregion
		internal void AddCore(NativeDrawingObject item) {
			innerList.Add(item);
		}
		internal void InsertCore(int index, NativeDrawingObject item) {
			innerList.Insert(index, item);
		}
		internal int IndexOf(NativeDrawingObject item) {
			return innerList.IndexOf(item);
		}
		internal void RemoveCore(NativeDrawingObject item) {
			innerList.Remove(item);
		}
		public void Clear() {
			for (int i = 0; i < this.Count; i++) {
				innerList[i].IsValid = false;
			}
			worksheet.ClearNativeDrawingsCore();
			worksheet.ModelWorksheet.ClearDrawingObjectsCollection();
		}
		public void NormalizeZOrder() { worksheet.ModelWorksheet.NormalizeZOrder(); }
		public void ClearCore() {
			innerList.Clear();
		}
		public void RemoveAt(int index) {
			NativeDrawingObject item = innerList[index];
			if (item.IsValid) {
				item.IsValid = false;
				if (item is Picture) {
					worksheet.ModelWorksheet.RemoveDrawing(((NativePicture)item).ModelPicture);
				}
			}
			worksheet.RemoveNativeDrawing(item);
		}
		public NativeDrawingObject FindPictureByTopLeftCell(Cell cell) {
			foreach (NativeDrawingObject item in innerList) {
				if (Object.ReferenceEquals(item.TopLeftCell, cell)) {
					return item;
				}
			}
			return null;
		}
	}
	#region StreamDocumentImageSource
	public class StreamDocumentImageSource : SpreadsheetImageSource {
		readonly Stream stream;
		public StreamDocumentImageSource(Stream stream) {
			Guard.ArgumentNotNull(stream, "stream");
			this.stream = stream;
		}
		public Stream Stream { get { return stream; } }
		protected internal override OfficeImage CreateImage(Model.DocumentModel documentModel) {
			return documentModel.CreateImage(stream);
		}
	}
	#endregion
	#region UriDocumentImageSource
	public class UriDocumentImageSource : SpreadsheetImageSource {
		readonly string uri;
		public UriDocumentImageSource(string uri) {
			Guard.ArgumentNotNull(uri, "uri");
			this.uri = uri;
		}
		public string Uri { get { return uri; } }
		protected internal override OfficeImage CreateImage(Model.DocumentModel documentModel) {
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
	public class ImageDocumentImageSource : SpreadsheetImageSource {
		readonly Image image;
		public ImageDocumentImageSource(Image image) {
			Guard.ArgumentNotNull(image, "image");
			this.image = image;
		}
		protected internal override OfficeImage CreateImage(Model.DocumentModel documentModel) {
			return documentModel.CreateImage(image);
		}
	}
	#endregion
#endif
}
