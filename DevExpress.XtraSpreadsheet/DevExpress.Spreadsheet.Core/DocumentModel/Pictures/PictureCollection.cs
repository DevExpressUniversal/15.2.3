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
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Utils;
using System.Collections.Generic;
using DevExpress.Office;
namespace DevExpress.XtraSpreadsheet.Model {
	#region DrawingObjectsCollection
	public class DrawingObjectsCollection : SimpleCollection<IDrawingObject>, IDisposable {
		#region Fields
		readonly IDrawingObjectByZOrderSorter drawingObjectByZOrderSorter;
		#endregion
		public DrawingObjectsCollection(IDrawingObjectByZOrderSorter drawingObjectByZOrderSorter) {
			this.drawingObjectByZOrderSorter = drawingObjectByZOrderSorter;
		}
		protected internal int GetPictureCount() {
			int result = 0;
			foreach (IDrawingObject drawing in this) {
				if (drawing is Picture)
					result++;
			}
			return result;
		}
		protected internal int GetChartCount() {
			int result = 0;
			foreach (IDrawingObject drawing in this) {
				if (drawing is Chart)
					result++;
			}
			return result;
		}
		protected internal int GetShapesCount() {
			int result = 0;
			foreach(IDrawingObject drawing in this) {
				if(drawing is ModelShape)
					result++;
			}
			return result;
		}
		#region GetDrawingObjects
		protected internal List<IDrawingObject> GetDrawings() {
			return new List<IDrawingObject>(this.InnerList);
		}
		protected internal List<IDrawingObject> GetDrawings(CellRange range) {
			List<IDrawingObject> result = new List<IDrawingObject>();
			foreach (IDrawingObject drawing in this) {
				int topRow = Math.Max(drawing.From.Row, range.TopLeft.Row);
				int bottomRow = Math.Min(drawing.To.Row, range.BottomRight.Row);
				int leftColumn = Math.Max(drawing.From.Col, range.TopLeft.Column);
				int rightColumn = Math.Min(drawing.To.Col, range.BottomRight.Column);
				if (bottomRow >= topRow && rightColumn >= leftColumn)
					result.Add(drawing);
			}
			return result;
		}
		protected internal List<IDrawingObject> GetIncludesDrawings(CellRange range) {
			List<IDrawingObject> result = new List<IDrawingObject>();
			foreach (IDrawingObject drawing in this)
				if (range.ContainsCell(drawing.From.Col, drawing.From.Row) && range.ContainsCell(drawing.To.Col, drawing.To.Row))
					result.Add(drawing);
			return result;
		}
		#endregion
		protected internal List<IDrawingObject> GetDrawingsHostedInCell(ICell cell) {
			List<IDrawingObject> result = new List<IDrawingObject>();
			foreach (IDrawingObject drawing in this) {
				if (drawing.From.Col == cell.ColumnIndex && drawing.From.Row == cell.RowIndex)
					result.Add(drawing);
			}
			return result;
		}
		protected internal List<IDrawingObject> GetIntersectedWithPrintArea(CellRangeBase range) {
			List<IDrawingObject> result = new List<IDrawingObject>();
			foreach(IDrawingObject drawing in this) {
				if(range.Intersects(new CellRange(drawing.Worksheet, new CellPosition(Math.Max(0, drawing.From.Col - 1), Math.Max(0, drawing.From.Row - 1)), new CellPosition(drawing.To.Col, drawing.To.Row))))
					result.Add(drawing);
			}
			return result;
		}
		protected internal int GetMaxDrawingId() {
			int result = 1;
			for (int i = 0; i < this.Count; i++)
				result = Math.Max(this[i].DrawingObject.Properties.Id, result);
			return result;
		}
		#region GetPictures
		protected internal List<Picture> GetPictures(CellRange range) {
			List<Picture> result = new List<Picture>();
			foreach (IDrawingObject drawing in this) {
				int topRow = Math.Max(drawing.From.Row, range.TopLeft.Row);
				int bottomRow = Math.Min(drawing.To.Row, range.BottomRight.Row);
				int leftColumn = Math.Max(drawing.From.Col, range.TopLeft.Column);
				int rightColumn = Math.Min(drawing.To.Col, range.BottomRight.Column);
				if (bottomRow >= topRow && rightColumn >= leftColumn) {
					Picture picture = drawing as Picture;
					if(picture != null)
						result.Add(picture);
				}
			}
			return result;
		}
		protected internal List<Picture> GetPictures() {
			List<Picture> result = new List<Picture>();
			foreach (IDrawingObject drawing in this) {
				Picture picture = drawing as Picture;
				if (picture != null)
					result.Add(picture);
			}
			return result;
		}
		#endregion
		protected internal List<Picture> GetPicturesHostedInCell(ICell cell) {
			List<Picture> result = new List<Picture>();
			foreach (IDrawingObject drawing in this) {
				if (drawing.From.Col == cell.ColumnIndex && drawing.From.Row == cell.RowIndex) {
					Picture picture = drawing as Picture;
					if(picture != null)
						result.Add(picture);
				}
			}
			return result;
		}
		public override int Add(IDrawingObject item) {
			int index = base.Add(item);
			item.SetIndexInCollection(index);
			drawingObjectByZOrderSorter.DrawingObjectsByZOrderCollections.AddDrawingObject(index, item);
			RaiseDrawingAdded(item);
			return index;
		}
		public override void RemoveAt(int index) {
			IDrawingObject removed = InnerList[index];
			RaiseDrawingRemoved(removed);
			for (int i = index + 1; i < Count; i++) {
				IDrawingObject currentDrawing = InnerList[i];
				currentDrawing.SetIndexInCollection(i - 1);
			}
			base.RemoveAt(index);
			drawingObjectByZOrderSorter.DrawingObjectsByZOrderCollections.RemoveDrawingObject(removed);
		}
		public override void Clear() {
			RaiseCollectionCleared();
			base.Clear();
		}
		public override void Insert(int index, IDrawingObject item) {
			base.Insert(index, item);
			for (int i = index + 1; i < Count; i++) {
				IDrawingObject currentDrawing = InnerList[i];
				currentDrawing.SetIndexInCollection(i);
			}
			drawingObjectByZOrderSorter.DrawingObjectsByZOrderCollections.AddDrawingObject(index, item);
			RaiseDrawingInserted(index, item);
		}
		public void Insert(int index, IDrawingObject item, int zOrder) {
			base.Insert(index, item);
			for (int i = index + 1; i < Count; i++) {
				IDrawingObject currentDrawing = InnerList[i];
				currentDrawing.SetIndexInCollection(i);
			}
			drawingObjectByZOrderSorter.DrawingObjectsByZOrderCollections.AddDrawingObject(index, item, zOrder);
			RaiseDrawingInserted(index, item);
		}
		public IEnumerable<Chart> GetChartsEnumerator() {
			foreach (IDrawingObject drawingObject in InnerList) {
				Chart chart = drawingObject as Chart;
				if (chart != null)
					yield return chart;
			}
		}
		#region DrawingAdded event
		DrawingObjectsCollectionChangedEventHandler onDrawingAdded;
		internal event DrawingObjectsCollectionChangedEventHandler DrawingAdded { add { onDrawingAdded += value; } remove { onDrawingAdded -= value; } }
		protected internal virtual void RaiseDrawingAdded(IDrawingObject newDrawing) {
			if (onDrawingAdded != null) {
				DrawingObjectsCollectionChangedEventArgs args = new DrawingObjectsCollectionChangedEventArgs(newDrawing);
				onDrawingAdded(this, args);
			}
		}
		#endregion
		#region DrawingInserted event
		DrawingInsertedEventHandler onDrawingInserted;
		internal event DrawingInsertedEventHandler DrawingInserted { add { onDrawingInserted += value; } remove { onDrawingInserted -= value; } }
		protected internal virtual void RaiseDrawingInserted(int index, IDrawingObject newDrawing) {
			if (onDrawingInserted != null) {
				DrawingInsertedEventArgs args = new DrawingInsertedEventArgs(index, newDrawing);
				onDrawingInserted(this, args);
			}
		}
		#endregion
		#region DrawingRemoved event
		DrawingObjectsCollectionChangedEventHandler onDrawingRemoved;
		internal event DrawingObjectsCollectionChangedEventHandler DrawingRemoved { add { onDrawingRemoved += value; } remove { onDrawingRemoved -= value; } }
		protected internal virtual void RaiseDrawingRemoved(IDrawingObject drawing) {
			if (onDrawingRemoved != null) {
				DrawingObjectsCollectionChangedEventArgs args = new DrawingObjectsCollectionChangedEventArgs(drawing);
				onDrawingRemoved(this, args);
			}
		}
		#endregion
		#region Clear event
		EventHandler onCollectionClear;
		internal event EventHandler CollectionCleared { add { onCollectionClear += value; } remove { onCollectionClear -= value; } }
		protected internal virtual void RaiseCollectionCleared() {
			if (onCollectionClear != null) {
				EventArgs args = new EventArgs();
				onCollectionClear(this, args);
			}
		}
		#endregion
		#region Notification
		public void OnRangeRemoving(RemoveRangeNotificationContext notificationContext) {
			foreach(IDrawingObject drawing in this)
				drawing.OnRangeRemoving(notificationContext);
			if (notificationContext.Mode == RemoveCellMode.Default)
				return;
			CellRange removableRange = notificationContext.Range;
			List<IDrawingObject> deleted = GetIncludesDrawings(removableRange);
			if(deleted.Count > 0) {
				Worksheet sheet = removableRange.Worksheet as Worksheet;
				foreach(IDrawingObject current in deleted)
					if(current != null && current.AnchorType == AnchorType.TwoCell)
						sheet.RemoveDrawing(current);
			}
			foreach(IDrawingObject drawing in this)
				drawing.AnchorData.OnRangeRemoving(notificationContext);
		}
		public void OnRangeInserting(InsertRangeNotificationContext context) {
			foreach (IDrawingObject drawing in this)
				drawing.OnRangeInserting(context);
			foreach(IDrawingObject drawing in this)
				drawing.AnchorData.OnRangeInserting(context);
		}
		#endregion
		#region IDisposable Members
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				foreach (IDrawingObject drawing in this)
					drawing.Dispose();
			} 
		}
		#endregion
	}
	#endregion
	public interface IDrawingObjectsContainer {
		DrawingObjectsCollection DrawingObjects { get; }
	}
}
