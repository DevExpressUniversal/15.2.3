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
using System.Drawing;
using System.Linq;
using DevExpress.Utils.Drawing.Animation;
namespace DevExpress.XtraBars.Docking2010.Views.Widget {
	public class TableGroupInfo : BaseElementInfo {
		IDictionary<RowDefinition, RowDefinitionInfo> rowDefinitionInfosCore;
		IDictionary<ColumnDefinition, ColumnDefinitionInfo> columnDefinitionInfosCore;
		IDictionary<Document, IDocumentInfo> documentInfosCore;
		IDictionary<Point, List<Point>> dropList;
		IDictionary<Point, List<Point>> dropListTemp;
		TableGroup groupCore;
		public TableGroupInfo(WidgetView view, TableGroup group)
			: base(view) {
			groupCore = group;
			rowDefinitionInfosCore = new Dictionary<RowDefinition, RowDefinitionInfo>();
			columnDefinitionInfosCore = new Dictionary<ColumnDefinition, ColumnDefinitionInfo>();
			documentInfosCore = new Dictionary<Document, IDocumentInfo>();
			dropList = new Dictionary<Point, List<Point>>();
			dropListTemp = new Dictionary<Point, List<Point>>();
		}
		public TableGroup Group {
			get { return groupCore; }
		}
		protected IDictionary<Document, IDocumentInfo> DocumentInfos {
			get { return documentInfosCore; }
		}
		public IDictionary<ColumnDefinition, ColumnDefinitionInfo> ColumnDefinitionInfos {
			get { return columnDefinitionInfosCore; }
		}
		public IDictionary<RowDefinition, RowDefinitionInfo> RowDefinitionInfos {
			get { return rowDefinitionInfosCore; }
		}
		protected WidgetView WidgetView {
			get { return Owner as WidgetView; }
		}
		protected internal IDictionary<Point, List<Point>> DropList {
			get { return dropList; }
		}
		protected override void CalcCore(Graphics g, Rectangle bounds) {
			if(ColumnDefinitionInfos.Count == 0 || RowDefinitionInfos.Count == 0) {
				foreach(var documentInfo in DocumentInfos.Values) {
					documentInfo.Bounds = new Rectangle(-10000, -10000, 200, 200);
				}
				return;
			}
			CalcTableElements(g, bounds);
			CalcDocumentInfos();
		}
		protected internal Point GetHitTest(Point point) {
			Point result = Point.Empty;
			for(int i = 0; i < WidgetView.Rows.Count; i++) {
				RowDefinitionInfo info = RowDefinitionInfos[WidgetView.Rows[i]];
				int top = info.Bounds.Top - (int)Math.Round(WidgetView.DocumentSpacing / 2.0);
				int bottom = info.Bounds.Bottom + (int)Math.Round(WidgetView.DocumentSpacing / 2.0);
				if(i == 0 && point.Y <= info.Bounds.Top) {
					result.Y = i;
					break;
				}
				if(i == WidgetView.Rows.Count - 1 && point.Y >= info.Bounds.Bottom) {
					result.Y = i;
					break;
				}
				if(point.Y >= top && point.Y <= bottom) {
					result.Y = i;
					break;
				}
			}
			for(int i = 0; i < WidgetView.Columns.Count; i++) {
				ColumnDefinitionInfo info = ColumnDefinitionInfos[WidgetView.Columns[i]];
				int left = info.Bounds.Left - (int)Math.Round(WidgetView.DocumentSpacing / 2.0);
				int right = info.Bounds.Right + (int)Math.Round(WidgetView.DocumentSpacing / 2.0);
				if(i == 0 && point.X <= info.Bounds.Left) {
					result.X = i;
					break;
				}
				if(i == WidgetView.Columns.Count - 1 && point.X >= info.Bounds.Right) {
					result.X = i;
					break;
				}
				if(point.X >= left && point.X <= right) {
					result.X = i;
					break;
				}
			}
			return result;
		}
		protected virtual void CalcTableElements(Graphics g, Rectangle bounds) {
			int boundsWidth = bounds.Width - (WidgetView.Columns.Count + 1) * WidgetView.DocumentSpacing;
			int boundsHeight = bounds.Height - (WidgetView.Rows.Count + 1) * WidgetView.DocumentSpacing;
			StackGroupLengthHelper.CalcActualGroupLength(boundsHeight, Group.Rows);
			StackGroupLengthHelper.CalcActualGroupLength(boundsWidth, Group.Columns);
			int offsetX = bounds.X + WidgetView.DocumentSpacing;
			int offsetY = bounds.Y + WidgetView.DocumentSpacing;
			foreach(var columnDefinition in Group.Columns) {
				var columnDefinitionInfo = ColumnDefinitionInfos[columnDefinition] as IBaseElementInfo;
				columnDefinitionInfo.Calc(g, new Rectangle(offsetX, bounds.Y + WidgetView.DocumentSpacing, columnDefinition.ActualLength, bounds.Height));
				offsetX += columnDefinition.ActualLength + WidgetView.DocumentSpacing;
			}
			foreach(var rowDefinition in Group.Rows) {
				var rowDefinitionInfo = RowDefinitionInfos[rowDefinition] as IBaseElementInfo;
				rowDefinitionInfo.Calc(g, new Rectangle(bounds.X + WidgetView.DocumentSpacing, offsetY, bounds.Width, rowDefinition.ActualLength));
				offsetY += rowDefinition.ActualLength + WidgetView.DocumentSpacing;
			}
		}
		protected virtual void CalcDocumentInfos() {
			foreach(var documentInfo in DocumentInfos.Values) {
				if(documentInfo.ColumnIndex < 0 || documentInfo.RowIndex < 0 || documentInfo.RowIndex >= Group.Rows.Count || documentInfo.ColumnIndex >= Group.Columns.Count) {
					documentInfo.Bounds = new Rectangle(-10000, -10000, 200, 200);
					continue;
				}
				var rowInfo = RowDefinitionInfos[Group.Rows[documentInfo.RowIndex]];
				var columnInfo = ColumnDefinitionInfos[Group.Columns[documentInfo.ColumnIndex]];
				Point location = new Point(columnInfo.Bounds.X, rowInfo.Bounds.Y);
				Size size = new Size(columnInfo.Bounds.Width, rowInfo.Bounds.Height);
				if(documentInfo.ColumnSpan > 1) {
					for(int i = documentInfo.ColumnIndex + 1; i < documentInfo.ColumnIndex + documentInfo.ColumnSpan && i < ColumnDefinitionInfos.Count; i++) {
						size.Width += ColumnDefinitionInfos[Group.Columns[i]].Bounds.Width;
					}
					size.Width += (documentInfo.ColumnSpan - 1) * WidgetView.DocumentSpacing;
				}
				if(documentInfo.RowSpan > 1) {
					for(int i = documentInfo.RowIndex + 1; i < documentInfo.RowIndex + documentInfo.RowSpan && i < RowDefinitionInfos.Count; i++) {
						size.Height += RowDefinitionInfos[Group.Rows[i]].Bounds.Height;
					}
					size.Height += (documentInfo.RowSpan - 1) * WidgetView.DocumentSpacing;
				}
				documentInfo.Bounds = new Rectangle(location, size);
			}
		}
		public override System.Type GetUIElementKey() {
			return typeof(TableGroupInfo);
		}
		protected internal void Register(RowDefinition rowDefinition) {
			if(rowDefinitionInfosCore.Keys.Contains(rowDefinition)) return;
			rowDefinitionInfosCore.Add(rowDefinition, new RowDefinitionInfo(WidgetView, rowDefinition));
		}
		protected internal void Register(ColumnDefinition columnDefinition) {
			if(columnDefinitionInfosCore.Keys.Contains(columnDefinition)) return;
			columnDefinitionInfosCore.Add(columnDefinition, new ColumnDefinitionInfo(WidgetView, columnDefinition));
		}
		protected internal void Unregister(RowDefinition rowDefinition) {
			if(!rowDefinitionInfosCore.Keys.Contains(rowDefinition)) return;
			rowDefinitionInfosCore.Remove(rowDefinition);
		}
		protected internal void Unregister(ColumnDefinition columnDefinition) {
			if(!columnDefinitionInfosCore.Keys.Contains(columnDefinition)) return;
			columnDefinitionInfosCore.Remove(columnDefinition);
		}
		protected internal void Register(Document document) {
			if(DocumentInfos.Keys.Contains(document)) return;
			DocumentInfo info = document.Info as DocumentInfo ?? new DocumentInfo(WidgetView, document);
			document.SetInfo(info);
			DocumentInfos.Add(document, info);
		}
		protected internal void Unregister(Document document) {
			if(!DocumentInfos.Keys.Contains(document)) return;
			DocumentInfos.Remove(document);
		}
		EmptyDocument emptyDocument = new EmptyDocument();
		protected internal void OnBeginDragging(Document document) {
			FillDropList(document);
			FilterDropList(document);
		}
		protected internal void OnDragging(Point point, BaseDocument dragItem) {
			int newEmptyDocumentRowIndex = 0;
			int newEmptyDocumentColumnIndex = 0;
			GetNewEmptyDocumentIndex(point, ref newEmptyDocumentRowIndex, ref newEmptyDocumentColumnIndex);
			var document = Group.GetDocument(newEmptyDocumentColumnIndex, newEmptyDocumentRowIndex);
			if((newEmptyDocumentRowIndex != emptyDocument.RowIndex || newEmptyDocumentColumnIndex != emptyDocument.ColumnIndex) &&
				(emptyDocument.RowIndex > -1 && emptyDocument.ColumnIndex > -1)) {
				ResetDragging();
			}
			emptyDocument.RowIndex = newEmptyDocumentRowIndex;
			emptyDocument.RowSpan = (dragItem as Document).RowSpan;
			emptyDocument.ColumnIndex = newEmptyDocumentColumnIndex;
			emptyDocument.ColumnSpan = (dragItem as Document).ColumnSpan;
			InsertEmptyDocument();
		}
		protected internal void OnEndDragging(BaseDocument document) {
			SwapDocuments(document);
		}
		protected internal void ResetDragging() {
			RemoveEmptyDocument();
		}
		protected virtual void SwapDocuments(BaseDocument document) {
			using(Owner.LockPainting()) {
				var widgetDocument = document as Document;
				Point emptyDocumentLocation = new Point(emptyDocument.ColumnIndex, emptyDocument.RowIndex);
				var swapDocument = Group.GetDocument(emptyDocument.ColumnIndex, emptyDocument.RowIndex);
				if(swapDocument is EmptyDocument)
					swapDocument = null;
				List<Document> documentsToSwap = GetDocumentToSwap(ref emptyDocumentLocation, widgetDocument);
				if(documentsToSwap != null) {
					foreach(var doc in documentsToSwap) {
						doc.ColumnIndex += widgetDocument.ColumnIndex - emptyDocumentLocation.X;
						doc.RowIndex += widgetDocument.RowIndex - emptyDocumentLocation.Y;
					}
					if(documentsToSwap.Contains(swapDocument)) {
						widgetDocument.ColumnIndex = emptyDocumentLocation.X;
						widgetDocument.RowIndex = emptyDocumentLocation.Y;
					}
				}
				if(swapDocument == null) {
					widgetDocument.ColumnIndex = emptyDocumentLocation.X;
					widgetDocument.RowIndex = emptyDocumentLocation.Y;
				}
				AddAnimation(documentsToSwap != null ? documentsToSwap.ToArray() : new Document[] { swapDocument }, widgetDocument);
				Group.Items.Remove(emptyDocument);
				Group.Add(widgetDocument);
			}
			dropList.Clear();
		}
		List<Document> GetDocumentToSwap(ref Point emptyDocumentLocation, Document document) {
			List<Document> documentsToSwap = new List<Document>();
			var pointsToDrop = GetPointsToDrop(ref emptyDocumentLocation);
			if(pointsToDrop != null) {
				GetDocumentToSwapCore(pointsToDrop, documentsToSwap);
				return documentsToSwap;
			}
			emptyDocumentLocation.X = document.ColumnIndex;
			emptyDocumentLocation.Y = document.RowIndex;
			return null;
		}
		void GetDocumentToSwapCore(List<Point> points, List<Document> documentsToSwap) {
			points.ForEach(new System.Action<Point>(p =>
			{
				var swDocument = Group.GetDocument(p.X, p.Y);
				if(swDocument != null && !documentsToSwap.Contains(swDocument))
					documentsToSwap.Add(swDocument);
			}));
		}
		List<Point> GetPointsToDrop(ref Point emptyDocumentLocation) {
			var emptyDoc = Group.GetDocumentWithSpan(emptyDocumentLocation.X, emptyDocumentLocation.Y);
			if(emptyDoc != null)
				foreach(var item in DropList.Keys) {
					var documnent = Group.GetDocumentWithSpan(item.X, item.Y);
					if(documnent == emptyDoc) {
						emptyDocumentLocation = item;
						return dropList[item];
					}
				}
			if(Enumerable.Contains(dropList.Keys, emptyDocumentLocation)) {
				return dropList[emptyDocumentLocation];
			}
			foreach(KeyValuePair<Point, List<Point>> item in dropList) {
				if(item.Value.Contains(emptyDocumentLocation)) {
					emptyDocumentLocation = item.Key;
					return item.Value;
				}
			}
			return null;
		}
		void AddAnimation(IEnumerable<Document> swapDocuments, Document document) {
			if(swapDocuments != null) {
				foreach(var item in swapDocuments) {
					if(item == null || item == emptyDocument) continue;
					AddAnimation(item);
				}
			}
			AddAnimation(document);
		}
		void AddAnimation(Document document) {
			WidgetContainer container = document.ContainerControl as WidgetContainer;
			container.BringToFront();
			container.AddBoundsAnimation(container.Bounds, GetTargetBounds(document), false);
		}
		internal Rectangle GetTargetBounds(Document document) {
			Point dropPoint = new Point(document.ColumnIndex, document.RowIndex);
			if(document is EmptyDocument) {
				var value = GetPointsToDrop(ref dropPoint);
				if(value == null)
					return Rectangle.Empty;
			}
			return GetTargetBoundsCore(dropPoint, document);
		}
		internal void CheckItemBounds(DocumentContainer container, ref Rectangle currentRect) {
			Size minSize = container.Info.GetNCMinSize();
			emptyDocument.Sizing = true;
			InsertEmptyDocument();
			if(currentRect.Width <= minSize.Width || currentRect.Height <= minSize.Height) {
				if(currentRect.X != emptyDocument.Info.Bounds.X && currentRect.Width == minSize.Width) {
					currentRect.X = emptyDocument.Info.Bounds.Right - minSize.Width;
					currentRect.Width = minSize.Width;
				}
				if(currentRect.Y != emptyDocument.Info.Bounds.Y && currentRect.Height == minSize.Height) {
					currentRect.Y = emptyDocument.Info.Bounds.Bottom - minSize.Height;
					currentRect.Height = minSize.Height;
				}
			}
			Point topLeft = new Point(currentRect.X, currentRect.Y);
			Point bottomRight = new Point(currentRect.Right, currentRect.Bottom);
			Document checkDocument = CalcItemPosition(topLeft, bottomRight);
			if(!Group.ValidateDocumentPosition(checkDocument, (Document)container.Document)) {
				emptyDocument.AssignProperties(checkDocument);
				emptyDocument.Info.Bounds = GetTargetBoundsCore(new Point(emptyDocument.ColumnIndex, emptyDocument.RowIndex), emptyDocument);
			}
			if(currentRect.Width > emptyDocument.Info.Bounds.Width) {
				currentRect.X = emptyDocument.Info.Bounds.X;
				currentRect.Width = emptyDocument.Info.Bounds.Width;
			}
			if(currentRect.Height > emptyDocument.Info.Bounds.Height) {
				currentRect.Y = emptyDocument.Info.Bounds.Y;
				currentRect.Height = emptyDocument.Info.Bounds.Height;
			}
		}
		internal Document CalcItemPosition(Point topLeft, Point bottomRight) {
			if(WidgetView.DocumentSpacing == 0) {
				topLeft.Offset(1, 1);
				bottomRight.Offset(-1, -1);
			}
			Point topLeftPointHit = GetHitTest(topLeft);
			Point bottomRightPointHit = GetHitTest(bottomRight);
			Document document = new Document();
			document.RowIndex = topLeftPointHit.Y;
			document.RowSpan = bottomRightPointHit.Y - topLeftPointHit.Y + 1;
			document.ColumnIndex = topLeftPointHit.X;
			document.ColumnSpan = bottomRightPointHit.X - topLeftPointHit.X + 1;
			return document;
		}
		internal Rectangle GetTargetBoundsCore(Point dropPoint, Document document) {
			if(dropPoint.X < 0 || dropPoint.Y < 0) return Rectangle.Empty;
			if(ColumnDefinitionInfos.Count != Group.Columns.Count || RowDefinitionInfos.Count != Group.Rows.Count) return Rectangle.Empty;
			var columnInfo = ColumnDefinitionInfos[Group.Columns[dropPoint.X]];
			var rowInfo = RowDefinitionInfos[Group.Rows[dropPoint.Y]];
			Size size = new Size(columnInfo.Bounds.Width, rowInfo.Bounds.Height);
			Point location = new Point(columnInfo.Bounds.X, rowInfo.Bounds.Y);
			if(document.ColumnSpan > 1) {
				for(int i = dropPoint.X + 1; i < dropPoint.X + document.ColumnSpan && i < ColumnDefinitionInfos.Count; i++) {
					size.Width += ColumnDefinitionInfos[Group.Columns[i]].Bounds.Width;
				}
				size.Width += (document.ColumnSpan - 1) * WidgetView.DocumentSpacing;
			}
			if(document.RowSpan > 1) {
				for(int i = dropPoint.Y + 1; i < dropPoint.Y + document.RowSpan && i < RowDefinitionInfos.Count; i++) {
					size.Height += RowDefinitionInfos[Group.Rows[i]].Bounds.Height;
				}
				size.Height += (document.RowSpan - 1) * WidgetView.DocumentSpacing;
			}
			return new Rectangle(location, size);
		}
		protected internal void InsertEmptyDocument() {
			if(!DocumentInfos.ContainsKey(emptyDocument)) {
				Group.Add(emptyDocument);
			}
		}
		protected internal void RemoveEmptyDocument() {
			emptyDocument.Sizing = false;
			if(DocumentInfos.ContainsKey(emptyDocument)) {
				Group.Items.Remove(emptyDocument);
			}
		}
		protected void GetNewEmptyDocumentIndex(Point point, ref int rowIndex, ref int columnIndex) {
			foreach(var columnDefinition in ColumnDefinitionInfos.Keys) {
				var columnDefinitionInfo = ColumnDefinitionInfos[columnDefinition] as IBaseElementInfo;
				var columnBounds = columnDefinitionInfo.Bounds;
				columnBounds.Y -= WidgetView.DocumentSpacing;
				columnBounds.Height += WidgetView.DocumentSpacing;
				columnBounds.Inflate(WidgetView.DocumentSpacing / 2, WidgetView.DocumentSpacing / 2);
				if(columnBounds.X <= point.X && columnBounds.Right >= point.X &&
					   columnBounds.Bottom >= point.Y && columnBounds.Y <= point.Y)
					columnIndex = Group.Columns.IndexOf(columnDefinition);
				if(columnIndex == 0 && point.X > columnBounds.Right)
					columnIndex = ColumnDefinitionInfos.Count - 1;
			}
			foreach(var rowDefinition in RowDefinitionInfos.Keys) {
				var rowDefinitionInfo = RowDefinitionInfos[rowDefinition] as IBaseElementInfo;
				var rowBounds = rowDefinitionInfo.Bounds;
				rowBounds.X -= WidgetView.DocumentSpacing;
				rowBounds.Width += WidgetView.DocumentSpacing;
				rowBounds.Inflate(WidgetView.DocumentSpacing / 2, WidgetView.DocumentSpacing / 2);
				if(rowBounds.X <= point.X && rowBounds.Right >= point.X &&
					   rowBounds.Bottom >= point.Y && rowBounds.Y <= point.Y)
					rowIndex = Group.Rows.IndexOf(rowDefinition);
				if(rowIndex == 0 && point.Y > rowBounds.Bottom)
					rowIndex = ColumnDefinitionInfos.Count - 1;
			}
		}
		protected virtual void FilterDropList(Document document) {
			foreach(var item in dropListTemp) {
				bool containsDocument = false;
				bool intersectDocument = false;
				foreach(var point in item.Value) {
					containsDocument |= Group.GetDocumentWithSpan(point.X, point.Y) != null;
					intersectDocument |= document.RowIndex <= point.Y && document.RowIndex + document.RowSpan > point.Y &&
						document.ColumnIndex <= point.X && document.ColumnIndex + document.ColumnSpan > point.X;
				}
				if(containsDocument && intersectDocument)
					continue;
				dropList.Add(item);
			}
			dropListTemp.Clear();
		}
		protected virtual void FillDropList(Document document) {
			for(int m = 0; m < RowDefinitionInfos.Count; m++) {
				for(int l = 0; l < ColumnDefinitionInfos.Count; l++) {
					List<Point> documentRange = new List<Point>();
					int rowSum = 0;
					int columnSum = 0;
					int maxRowSum = 0;
					int maxColumnSum = 0;
					Document nextDocument = null;
					Document prevDocument = null;
					nextDocument = Group.GetDocumentWithSpan(l, m);
					if(nextDocument != null && (nextDocument.RowIndex != m || nextDocument.ColumnIndex != l))
						continue;
					for(int j = m; j < m + document.RowSpan && j < RowDefinitionInfos.Count; j++) {
						for(int i = l; i < l + document.ColumnSpan && i < ColumnDefinitionInfos.Count; i++) {
							nextDocument = Group.GetDocumentWithSpan(i, j);
							Point point = new Point(i, j);
							if(!documentRange.Contains(point))
								documentRange.Add(point);
							if(prevDocument == nextDocument && prevDocument != null) continue;
							if(nextDocument != null && (nextDocument.RowIndex != j || nextDocument.ColumnIndex != i)) {
								if(nextDocument.RowIndex < m || nextDocument.ColumnIndex < l) {
									maxColumnSum = int.MaxValue;
									break;
								}
								continue;
							}
							columnSum += nextDocument == null ? 1 : nextDocument.ColumnSpan;
							prevDocument = nextDocument;
						}
						if(columnSum > maxColumnSum)
							maxColumnSum = columnSum;
						columnSum = 0;
					}
					prevDocument = null;
					for(int i = l; i < l + document.ColumnSpan && i < ColumnDefinitionInfos.Count; i++) {
						for(int j = m; j < m + document.RowSpan && j < RowDefinitionInfos.Count; j++) {
							nextDocument = Group.GetDocumentWithSpan(i, j);
							Point point = new Point(i, j);
							if(!documentRange.Contains(point))
								documentRange.Add(point);
							if(prevDocument == nextDocument && prevDocument != null) continue;
							if(nextDocument != null && (nextDocument.RowIndex != j || nextDocument.ColumnIndex != i)) {
								if(nextDocument.RowIndex < m || nextDocument.ColumnIndex < l) {
									maxRowSum = int.MaxValue;
									break;
								}
								continue;
							}
							rowSum += nextDocument == null ? 1 : nextDocument.RowSpan;
							prevDocument = nextDocument;
						}
						if(rowSum > maxRowSum)
							maxRowSum = rowSum;
						rowSum = 0;
					}
					if(maxRowSum == document.RowSpan && maxColumnSum == document.ColumnSpan) {
						dropListTemp.Add(new Point(l, m), documentRange);
					}
					rowSum = columnSum = maxColumnSum = maxRowSum = 0;
				}
			}
		}
		protected internal virtual void AddAnimation() {
			DevExpress.Utils.Drawing.Animation.ISupportXtraAnimation animationObject = Owner.Manager.GetOwnerControl() as DevExpress.Utils.Drawing.Animation.ISupportXtraAnimation;
			if(animationObject == null) return;
			var existAnimation = DevExpress.Utils.Drawing.Animation.XtraAnimator.Current.Get(animationObject, Group);
			var newAnimation = new WidgetsMovingAnimationInfo(Group, animationObject, Group, WidgetView.DocumentAnimationProperties);
			if(existAnimation != null) {
				newAnimation.BeginTick = existAnimation.CurrentTick;
				XtraAnimator.RemoveObject(animationObject, Group);
				existAnimation.Dispose();
			}
			DevExpress.Utils.Drawing.Animation.XtraAnimator.Current.AddAnimation(newAnimation);
		}
		protected internal void OnEndSizing(Document document) {
			document.AssignProperties(emptyDocument);
			RemoveEmptyDocument();
		}
	}
}
