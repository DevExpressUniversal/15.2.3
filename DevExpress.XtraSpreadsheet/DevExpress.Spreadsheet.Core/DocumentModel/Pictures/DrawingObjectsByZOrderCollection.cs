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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using DevExpress.Office.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region ZOrderRange
	public struct ZOrderRange {
		public int Start { get; set; }
		public int End { get; set; }
		public int Value { get; set; }
		public ZOrderRange(int start, int end, int value)
			: this() {
			Start = start;
			End = end;
			Value = value;
		}
	}
	#endregion
	#region DrawingObjectsByZOrderCollection
	public class DrawingObjectsByZOrderCollections : IEnumerable<IDrawingObject> {
		#region Fields
		readonly IDrawingObjectsContainer drawingObjectsContainer;
		readonly List<IDrawingObject> drawingObjects;
		readonly List<int> zOrders;
		public int Count { get { return drawingObjects.Count; } }
		#endregion
		public DrawingObjectsByZOrderCollections(IDrawingObjectsContainer drawingObjectsContainer) {
			this.drawingObjectsContainer = drawingObjectsContainer;
			drawingObjects = new List<IDrawingObject>();
			zOrders = new List<int>();
		}
		#region Properties
		public IDrawingObject this[int index] { get { return drawingObjects[index]; } }
		public IDrawingObject First { get { return drawingObjects.Count > 0 ? drawingObjects[0] : null; } }
		public IDrawingObject Last { get { return drawingObjects.Count > 0 ? drawingObjects[Count - 1] : null; } }
		#endregion
		public void Clear() {
			this.drawingObjects.Clear();
			this.zOrders.Clear();
		}
		public int AddDrawingObject(int index, IDrawingObject drawingObject) {
			int zOrder = zOrders.Count == 0 ? 1 : zOrders[zOrders.Count - 1] + 1;
			return AddDrawingObject(index, drawingObject, zOrder);
		}
		public int AddDrawingObject(int index, IDrawingObject drawingObject, int zOrder) {
			if(index < 0 || index >= drawingObjectsContainer.DrawingObjects.Count || drawingObjectsContainer.DrawingObjects[index] != drawingObject)
				Exceptions.ThrowInternalException();
			drawingObjects.Insert(index, drawingObject);
			zOrders.Insert(index, zOrder);
			return zOrder;
		}
		public int GetZOrder(IDrawingObject drawingObject) { return zOrders[drawingObjects.IndexOf(drawingObject)]; }
		public void RemoveDrawingObject(IDrawingObject drawingObject) {
			int index = drawingObjects.IndexOf(drawingObject);
			drawingObjects.RemoveAt(index);
			zOrders.RemoveAt(index);
		}
		void SetZOrderCore(IDrawingObject drawingObject, int newZOrder) {
			if (newZOrder < 1)
				Exceptions.ThrowArgumentException("newZOrder", newZOrder);
			int index = zOrders.BinarySearch(newZOrder);
			if (index < 0)
				index = ~index;
			drawingObjects.Insert(index, drawingObject);
			zOrders.Insert(index, newZOrder);
		}
		public void SetZOrder(IDrawingObject drawingObject, int newZOrder) {
			ZOrderRange zOrderRange = PrepareZOrderRange(drawingObject, newZOrder);
			SetZOrder(drawingObject, newZOrder, zOrderRange);
		}
		public void SetZOrder(IDrawingObject drawingObject, int newZOrder, ZOrderRange zOrderRange) {
			if (drawingObjects.IndexOf(drawingObject) < 0)
				Exceptions.ThrowInternalException();
			IncreaseZOrders(zOrderRange);
			RemoveDrawingObject(drawingObject);
			SetZOrderCore(drawingObject, newZOrder);
		}
		public void ForEach(Action<IDrawingObject> action) { drawingObjects.ForEach(action); }
		public void Normalize() {
			for (int i = 0; i < zOrders.Count; i++) {
				zOrders[i] = i + 1;
			}
		}
		public ZOrderRange PrepareZOrderRange(IDrawingObject drawingObject, int newZOrder) {
			ZOrderRange result = new ZOrderRange(0, -1, -1);
			int index = zOrders.BinarySearch(newZOrder);
			if (index >= 0) {
				int end = index;
				while (end < zOrders.Count - 1 && zOrders[end] == zOrders[end + 1] - 1 && drawingObjects[end + 1] != drawingObject)
					end++;
				result = new ZOrderRange(index, end, -1);
			}
			return result;
		}
		public List<ZOrderRange> PrepareZOrderRangesForNormalize() {
			List<ZOrderRange> result = new List<ZOrderRange>();
			int start = 0;
			while (start < zOrders.Count) {
				int end = start;
				while (end < zOrders.Count - 1 && zOrders[end] == zOrders[end + 1] - 1)
					end++;
				ZOrderRange zOrderRange = new ZOrderRange(start, end, zOrders[start]);
				result.Add(zOrderRange);
				start = end + 1;
			}
			return result;
		}
		void IncreaseZOrders(ZOrderRange zOrderRange) {
			for (int i = zOrderRange.Start; i <= zOrderRange.End; i++)
				zOrders[i]++;
		}
		void DecreaseZOrders(ZOrderRange zOrderRange) {
			for (int i = zOrderRange.Start; i <= zOrderRange.End; i++)
				zOrders[i]--;
		}
		public void UndoZOrder(IDrawingObject drawingObject, int oldZOrder, int newZOrder, ZOrderRange zOrderRange) {
			if (drawingObjects.IndexOf(drawingObject) < 0)
				Exceptions.ThrowInternalException();
			if (newZOrder < oldZOrder) {
				RemoveDrawingObject(drawingObject);
				DecreaseZOrders(zOrderRange);
			}
			else {
				DecreaseZOrders(zOrderRange);
				RemoveDrawingObject(drawingObject);
			}
			SetZOrderCore(drawingObject, oldZOrder);
		}
		public void UndoNormalize(List<ZOrderRange> zOrderRanges) {
			foreach (ZOrderRange zOrderRange in zOrderRanges) {
				for (int i = zOrderRange.Start; i <= zOrderRange.End; i++) {
					zOrders[i] = zOrderRange.Value + (i - zOrderRange.Start);
				}
			}
		}
		#region GetDrawingObjects
		protected internal List<IDrawingObject> GetDrawingObjects(CellRange range) {
			List<IDrawingObject> result = new List<IDrawingObject>();
			foreach(IDrawingObject currentDrawingObject in this) {
				bool drawingObjectInRange = DrawingObjectInRange(range, currentDrawingObject);
				if(drawingObjectInRange)
					result.Add(currentDrawingObject);
			}
			return result;
		}
		protected internal List<IDrawingObject> GetDrawingObjects(CellRange range, DrawingObjectType drawingObjectType) {
			List<IDrawingObject> result = new List<IDrawingObject>();
			foreach (IDrawingObject currentDrawingObject in this) {
				if (currentDrawingObject.DrawingType != drawingObjectType)
					continue;
				bool drawingObjectInRange = DrawingObjectInRange(range, currentDrawingObject);
				if (drawingObjectInRange)
					result.Add(currentDrawingObject);
			}
			return result;
		}
		static bool DrawingObjectInRange(CellRange range, IDrawingObject currentDrawingObject) {
			int topRow = Math.Max(currentDrawingObject.From.Row, range.TopLeft.Row);
			int bottomRow = Math.Min(currentDrawingObject.To.Row, range.BottomRight.Row);
			int leftColumn = Math.Max(currentDrawingObject.From.Col, range.TopLeft.Column);
			int rightColumn = Math.Min(currentDrawingObject.To.Col, range.BottomRight.Column);
			bool drawingObjectInRange = bottomRow >= topRow && rightColumn >= leftColumn;
			return drawingObjectInRange;
		}
		protected internal List<IDrawingObject> GetDrawingObjects() {
			return new List<IDrawingObject>(drawingObjects);
		}
		#endregion
		#region GetPictures
		protected List<Picture> GetPictures(CellRange range) {
			List<Picture> result = new List<Picture>();
			foreach (IDrawingObject currentDrawingObject in this) {
				Picture currentPicture = currentDrawingObject as Picture;
				if (currentPicture == null)
					continue;
				int topRow = Math.Max(currentDrawingObject.From.Row, range.TopLeft.Row);
				int bottomRow = Math.Min(currentDrawingObject.To.Row, range.BottomRight.Row);
				int leftColumn = Math.Max(currentDrawingObject.From.Col, range.TopLeft.Column);
				int rightColumn = Math.Min(currentDrawingObject.To.Col, range.BottomRight.Column);
				if (bottomRow >= topRow && rightColumn >= leftColumn)
					result.Add(currentPicture);
			}
			return result;
		}
		#endregion
		#region Implementation of IEnumerable
		public IEnumerator<IDrawingObject> GetEnumerator() {
			return new DrawingObjectcsByZOrderEnumerator(this);
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		#endregion
		#region Layout
		public void BringForward(IDrawingObject drawingObject) {
			if (drawingObject == drawingObjects.Last())
				return;
			IDrawingObject other = drawingObjects[drawingObjects.IndexOf(drawingObject) + 1];
			other.ZOrder = drawingObject.ZOrder;
		}
		public void SendBackward(IDrawingObject drawingObject) {
			if (drawingObject == drawingObjects[0])
				return;
			IDrawingObject other = drawingObjects[drawingObjects.IndexOf(drawingObject) - 1];
			drawingObject.ZOrder = other.ZOrder;
		}
		public void BringToFront(IDrawingObject drawingObject) {
			if(drawingObject != drawingObjects.Last())
				drawingObject.ZOrder = drawingObjects.Last().ZOrder + 1;
		}
		public void SendToBack(IDrawingObject drawingObject) {
			if (drawingObject != drawingObjects.First())
				drawingObject.ZOrder = 1;
		}
		#endregion
	}
	#endregion
	#region DrawingObjectcsByZOrderEnumerator
	class DrawingObjectcsByZOrderEnumerator : IEnumerator<IDrawingObject> {
		#region Fields
		readonly DrawingObjectsByZOrderCollections drawingObjectsByZOrderCollections;
		int position;
		#endregion
		public DrawingObjectcsByZOrderEnumerator(DrawingObjectsByZOrderCollections drawingObjectsByZOrderCollections) {
			this.drawingObjectsByZOrderCollections = drawingObjectsByZOrderCollections;
			position = -1;
		}
		#region Implementation of IDisposable
		public void Dispose() {
		}
		#endregion
		#region Implementation of IEnumerator
		public bool MoveNext() {
			position++;
			return (position < drawingObjectsByZOrderCollections.Count);
		}
		public void Reset() {
			position = -1;
		}
		public IDrawingObject Current {
			get {
				try {
					return drawingObjectsByZOrderCollections[position];
				}
				catch (IndexOutOfRangeException) {
					throw new IndexOutOfRangeException();
				}
			}
		}
		object IEnumerator.Current {
			get { return Current; }
		}
		#endregion
	}
	#endregion
	public interface IDrawingObjectByZOrderSorter {
		DrawingObjectsByZOrderCollections DrawingObjectsByZOrderCollections { get; }
	}
}
