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
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Office;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.Spreadsheet {
	public interface PictureCollection : ISimpleCollection<Picture> {
#if !SL
		Picture AddPicture(Image image, Cell topLeftCell);
		Picture AddPicture(string filename, Cell topLeftCell);
		Picture AddPicture(Image image, Range targetRange);
		Picture AddPicture(Image image, Range targetRange, bool lockAspectRatio);
		Picture AddPicture(string filename, Range targetRange);
		Picture AddPicture(string filename, Range targetRange, bool lockAspectRatio);
		Picture AddPicture(string filename, float x, float y);
		Picture AddPicture(string filename, float x, float y, float width, float height);
		Picture AddPicture(string filename, float x, float y, float width, float height, bool lockAspectRatio);
		Picture AddPicture(Image image, float x, float y);
		Picture AddPicture(Image image, float x, float y, float width, float height);
		Picture AddPicture(Image image, float x, float y, float width, float height, bool lockAspectRatio);
#endif
		Picture AddPicture(SpreadsheetImageSource imageSource, Cell topLeftCell);
		Picture AddPicture(SpreadsheetImageSource imageSource, Range targetRange);
		Picture AddPicture(SpreadsheetImageSource imageSource, Range targetRange, bool lockAspectRatio);
		Picture AddPicture(SpreadsheetImageSource imageSource, float x, float y);
		Picture AddPicture(SpreadsheetImageSource imageSource, float x, float y, float width, float height);
		Picture AddPicture(SpreadsheetImageSource imageSource, float x, float y, float width, float height, bool lockAspectRatio);
		void RemoveAt(int index);
		void Clear();
		bool Contains(Picture picture);
		int IndexOf(Picture picture);
		Picture GetPictureById(int id);
		IList<Picture> GetPicturesByName(string pictureName);
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using System.Collections;
	using DevExpress.Office.Utils;
	using DevExpress.Utils;
	using DevExpress.XtraSpreadsheet.Localization;
	using DevExpress.XtraSpreadsheet.Utils;
	using DevExpress.Spreadsheet;
	partial class NativePictureCollection : PictureCollection {
		readonly NativeWorksheet worksheet;
		readonly List<NativePicture> innerList = new List<NativePicture>();
		public NativePictureCollection(NativeWorksheet worksheet) {
			this.worksheet = worksheet;
		}
		public Picture this[int index] {
			get {
				return innerList[index];
			}
		}
		public Picture GetPictureById(int id) {
			for (int i = 0; i < Count; i++) {
				if ((innerList[i].Id == id) && (innerList[i].IsValid))
					return this[i];
			}
			return null;
		}
		public IList<Picture> GetPicturesByName(string pictureName) {
			List<Picture> result = new List<Picture>();
			for (int i = 0; i < Count; i++) {
				if ((innerList[i].Name == pictureName) && (innerList[i].IsValid))
					result.Add(this[i]);
			}
			return result;
		}
		public int Count { get { return innerList.Count; } }
		public List<NativePicture> InnerList { get { return innerList; } }
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
			return this[this.Count - 1];
		}
		Picture AddPictureTopLeftAnchor(SpreadsheetImageSource imageSource, Cell topLeftCell) {
			Guard.ArgumentNotNull(imageSource, "imageSource");
			Guard.ArgumentNotNull(topLeftCell, "topLeftCell");
			if (!topLeftCell.Worksheet.Equals(worksheet))
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorUseRangeFromAnotherWorksheet);
			OfficeImage image = imageSource.CreateImage(worksheet.ModelWorkbook);
			Model.CellKey topLeftCellKey = ((NativeCell)topLeftCell).ReadOnlyModelCell.Key;
			worksheet.ModelWorksheet.InsertPicture(image, topLeftCellKey);
			return this[this.Count - 1];
		}
		#region IEnumerable members
		public IEnumerator<Picture> GetEnumerator() {
			return new EnumeratorConverter<NativePicture, Picture>(innerList.GetEnumerator(), ConvertNativePictureToPicture);
		}
		Picture ConvertNativePictureToPicture(NativePicture item) {
			return item;
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return innerList.GetEnumerator();
		}
		#endregion
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
		internal void AddCore(NativePicture item) {
			innerList.Add(item);
		}
		internal void InsertCore(int index, NativePicture item) {
			innerList.Insert(index, item);
		}
		internal int IndexOf(NativePicture item) {
			return innerList.IndexOf(item);
		}
		internal void RemoveCore(NativePicture item) {
			innerList.Remove(item);
		}
		public void Clear() {
			int count = this.Count;
			for (int i = count - 1; i >= 0; i--) {
				NativePicture item = innerList[i];
				if (item.IsValid) {
					item.IsValid = false;
					worksheet.ModelWorksheet.RemoveDrawing(item.ModelPicture);
				}
				worksheet.RemoveNativeDrawing(item);
			}
		}
		public void ClearCore() {
			innerList.Clear();
		}
		public void RemoveAt(int index) {
			NativePicture item = innerList[index];
			if (item.IsValid) {
				item.IsValid = false;
				worksheet.ModelWorksheet.RemoveDrawing(item.ModelPicture);
			}
			worksheet.RemoveNativeDrawing(item);
		}
		public bool Contains(Picture picture) {
			return InnerList.Contains((NativePicture)picture);
		}
		public int IndexOf(Picture picture) {
			return InnerList.IndexOf((NativePicture)picture);
		}
		protected float UnitsToModelUnitsF(float value) {
			return worksheet.NativeWorkbook.UnitsToModelUnitsF(value);
		}
		protected float ModelUnitsToUnitsF(float value) {
			return worksheet.NativeWorkbook.ModelUnitsToUnitsF(value);
		}
	}
}
