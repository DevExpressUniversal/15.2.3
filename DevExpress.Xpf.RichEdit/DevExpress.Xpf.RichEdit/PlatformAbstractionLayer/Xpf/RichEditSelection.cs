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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.XtraRichEdit.Drawing;
#if SL
using TransformMatrix = DevExpress.Xpf.Core.Native.Matrix;
#else
using TransformMatrix = System.Drawing.Drawing2D.Matrix;
#endif
namespace DevExpress.Xpf.RichEdit.Controls.Internal {
	public enum SelectionType {
		None,
		Flow,
		Image,
		Sizer,
	}
	#region IXpfRichEditSelection
	public interface IXpfRichEditSelection {
		SelectionType Type { get; }
		void AddRect(Rect rect, TransformMatrix transform);
		void AddEllipse(Rect rect, TransformMatrix transform);
		void AddFloatingObjectRect(Rect rect, TransformMatrix transform);
		void Recalculate();
	}
	#endregion
	#region XpfRichEditSelection
	public class XpfRichEditSelection : ContentControl, IXpfRichEditSelection {
		Stack<IXpfRichEditSelection> selections = new Stack<IXpfRichEditSelection>();
		Grid grid;
		public XpfRichEditSelection() {
			Focusable = false;
			this.grid = new Grid();
			Content = grid;
			OnSelectionTypeChanged(SelectionType.Flow);
		}
		public SelectionType Type {
			get {
				if (selections.Count <= 0)
					return SelectionType.None;
				return Selection.Type;
			}
			set {
				if (Type == value)
					return;
				OnSelectionTypeChanged(value);
			}
		}
		public IXpfRichEditSelection Selection {
			get {
				return selections.Count > 0 ? selections.Peek() : null;
			}
		}
		protected virtual void OnSelectionTypeChanged(SelectionType selectionType) {
			switch (selectionType) {
				case SelectionType.Flow:
					AppendFlowSelection();
					break;
				case SelectionType.Image:
					AppendImageSelection();
					break;
			}
		}
		void AppendFlowSelection() {
			XpfRichEditFlowSelection selection = new XpfRichEditFlowSelection();
			selections.Push(selection);
			grid.Children.Add(selection);
		}
		void AppendImageSelection() {
			XpfRichEditImageSelection selection = new XpfRichEditImageSelection();
			selections.Push(selection);
			grid.Children.Add(selection);
		}
		public void Reset() {
			selections.Clear();
			grid.Children.Clear();
		}
		#region IXpfRichEditSelection Members
		public virtual void AddRect(Rect rect, TransformMatrix transform) {
			Selection.AddRect(rect, transform);
		}
		public void AddEllipse(Rect rect, TransformMatrix transform) {
			Selection.AddEllipse(rect, transform);
		}
		public void Recalculate() {
			Selection.Recalculate();
		}
		public void AddFloatingObjectRect(Rect rect, TransformMatrix transform) {
			Selection.AddFloatingObjectRect(rect, transform);
		}
		#endregion
}
	#endregion
	#region XpfRichEditFlowSelection
	public class XpfRichEditFlowSelection : Control, IXpfRichEditSelection {
		readonly List<Rect> bounds;
		public XpfRichEditFlowSelection() {
			bounds = new List<Rect>();
			DefaultStyleKey = typeof(XpfRichEditFlowSelection);
		}
		public SelectionType Type { get { return SelectionType.Flow; } }
		public Panel Root { get; private set; }
		public PathGeometry Geometry { get; private set; }
		public FrameworkElement SingleItemSelection { get; private set; }
		public void ClearBounds() {
			bounds.Clear();
		}
		public void AddEllipse(Rect rect, TransformMatrix transform) {
		}
		public void AddRect(Rect rect, TransformMatrix transform) {
			bounds.Add(rect);
		}
		public void AddFloatingObjectRect(Rect rect, TransformMatrix transform) {
		}
		void AddPath(List<PathSegment> segments) {
			if(segments.Count == 0)
				return;
			PathFigure path = new PathFigure();
			path.IsClosed = true;
			path.IsFilled = true;
			path.StartPoint = (segments[0] as LineSegment).Point;
			segments.RemoveAt(0);
			PathSegmentCollection pathSegments = new PathSegmentCollection();
			for (int i = 0; i < segments.Count; i++)
				pathSegments.Add(segments[i]);
			path.Segments = pathSegments;
			Geometry.Figures.Add(path);
		}
		public void Recalculate() {
			ApplyTemplate();
			if (Geometry == null)
				return;
			Geometry.Figures.Clear();
			RectangleCalculator calculator = new RectangleCalculator();
			foreach(Rect rect in bounds) {
				calculator.Fill(rect);
			}
			foreach (List<PathSegment> shape in calculator.Result)
				AddPath(shape);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			CreateRoot();
			CreateGeometry();
			Recalculate();
		}
		private void CreateGeometry() {
			Geometry = GetTemplateChild("Geometry") as PathGeometry;
		}
		private void CreateRoot() {
			Root = VisualTreeHelper.GetChild(this, 0) as Panel;
		}
	}
	#endregion
	#region XpfRichEditImageSelection
	public class XpfRichEditImageSelection : Control, IXpfRichEditSelection {
		[ThreadStatic()]
		static Brush selectionFillBrush;
		[ThreadStatic()]
		static Brush rotationZoneFillBrush;
		[ThreadStatic()]
		static Brush selectionBorderBrush;
		abstract class ShapeInfo {
			Rect rect;
			TransformMatrix transform;
			bool isFrame;
			public Rect Rect { get { return rect; } set { rect = value; } }
			public TransformMatrix Transform { get { return transform; } set { transform = value; } }
			public bool IsFrame { get { return isFrame; } set { isFrame = value; } }
			public abstract System.Windows.Shapes.Shape CreateShape();
		}
		class RectInfo : ShapeInfo {
			public override System.Windows.Shapes.Shape CreateShape() {
				return new System.Windows.Shapes.Rectangle();
			}
		}
		class EllipseInfo : ShapeInfo {
			public override System.Windows.Shapes.Shape CreateShape() {
				return new System.Windows.Shapes.Ellipse();
			}
		}
		readonly List<ShapeInfo> shapes;
		public static readonly DependencyProperty HotSizeProperty = DependencyProperty.Register("HotSize", typeof(double), typeof(XpfRichEditImageSelection), new PropertyMetadata(15.0));
		public double HotSize { get { return (double)GetValue(HotSizeProperty); } set { SetValue(HotSizeProperty, value); } }
		public Panel Root { get; private set; }
		public SelectionType Type { get { return SelectionType.Image; } }
		public XpfRichEditImageSelection() {
			DefaultStyleKey = typeof(XpfRichEditImageSelection);
			shapes = new List<ShapeInfo>();
		}
		static Brush SelectionFillBrush {
			get {
				if(selectionFillBrush == null)
					selectionFillBrush = CreateSelectionFillBrush();
				return selectionFillBrush;
			}
		}
		static Brush RotationZoneFillBrush {
			get {
				if (rotationZoneFillBrush == null)
					rotationZoneFillBrush = CreateRotationZoneFillBrush();
				return rotationZoneFillBrush;			   
			}
		}
		static Brush SelectionBorderBrush {
			get {
				if(selectionBorderBrush == null)
					selectionBorderBrush = new SolidColorBrush(Color.FromArgb(128, 105, 147, 211));
				return selectionBorderBrush;
			}
		}
		static Brush CreateSelectionFillBrush() {
			return CreateSelectionFillBrush(Color.FromArgb(255, 202, 234, 237), Colors.White); 
		}
		static Brush CreateRotationZoneFillBrush() {
			return CreateSelectionFillBrush(Color.FromArgb(255, 136, 228, 58), Colors.White); 
		}
		static Brush CreateSelectionFillBrush(Color color1, Color color2) {
			LinearGradientBrush brush = new LinearGradientBrush();
			GradientStopCollection stops = brush.GradientStops;
			GradientStop stop = new GradientStop();
			stop.Color = color2;
			stop.Offset = 0;
			stops.Add(stop);
			stop = new GradientStop();
			stop.Color = color1;
			stop.Offset = 0.5;
			stops.Add(stop);
			stop = new GradientStop();
			stop.Color = color2;
			stop.Offset = 1;
			stops.Add(stop);
			brush.GradientStops = stops;
			brush.StartPoint = new Point(0.5, 0);
			brush.EndPoint = new Point(0.5, 1);
			brush.Freeze();
			return brush;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			CreateRoot();
			Recalculate();
		}
		private void CreateRoot() {
			Root = VisualTreeHelper.GetChild(this, 0) as Panel;
		}
		#region IXpfRichEditSelection Members
		public void AddEllipse(Rect rect, TransformMatrix transform) {
			ShapeInfo info = new EllipseInfo();
			info.Rect = rect;
			info.IsFrame = true;
			info.Transform = transform;
			shapes.Add(info);
		}
		public void AddRect(Rect rect, TransformMatrix transform) {
			ShapeInfo info = new RectInfo();
			info.Rect = rect;
			info.Transform = transform;
			info.IsFrame = true;
			shapes.Add(info);
		}
		public void AddFloatingObjectRect(Rect rect, TransformMatrix transform) {
			ShapeInfo info = new RectInfo();
			info.Rect = rect;
			info.Transform = transform;
			info.IsFrame = false;
			shapes.Add(info);
		}
		public void Recalculate() {
			ApplyTemplate();
			if (Root == null)
				return;
			int ellipseCount = 0;
			System.Windows.Shapes.Ellipse firstEllipseShape = null;
			for (int i = 0; i < shapes.Count; i++) {
				ShapeInfo info = shapes[i];
				System.Windows.Shapes.Shape shape = info.CreateShape();
				System.Windows.Shapes.Ellipse ellipseShape = shape as System.Windows.Shapes.Ellipse;
				if (ellipseShape != null) {
					ellipseCount++;
					if (firstEllipseShape == null)
						firstEllipseShape = ellipseShape;
				}
				if (info.IsFrame)
					shape.Fill = SelectionFillBrush;
				shape.Stroke = SelectionBorderBrush;
				shape.HorizontalAlignment = HorizontalAlignment.Left;
				shape.VerticalAlignment = VerticalAlignment.Top;
				shape.StrokeThickness = 1;
				Rect rect = info.Rect;
				TransformMatrix transform = info.Transform;
				if (transform != null) {
					transform = transform.Clone();
					transform.Translate((float)rect.Left, (float)rect.Top);
					shape.RenderTransform = XpfPainter.CreateMatrixTransform(transform);
				}
				else
					shape.RenderTransform = new TranslateTransform() { X = rect.Left, Y = rect.Top };
				shape.Width = rect.Width;
				shape.Height = rect.Height;
				Root.Children.Add(shape);
			}
			if ((ellipseCount % 2) != 0 && firstEllipseShape != null)
				firstEllipseShape.Fill = RotationZoneFillBrush;
		}
		#endregion
	}
	#endregion
	#region XpfRichEditSizerSelection
	public class XpfRichEditSizerSelection : Control, IXpfRichEditSelection {
		public XpfRichEditSizerSelection() {
			DefaultStyleKey = typeof(XpfRichEditSizerSelection);
		}
		#region IXpfRichEditSelection Members
		public SelectionType Type { get { return SelectionType.Sizer; } }
		public void AddEllipse(Rect rect, TransformMatrix transform) {
		}
		public void AddRect(Rect rect, TransformMatrix transform) {
			if (transform != null) {
				transform = transform.Clone();
				transform.Translate((float)rect.Left, (float)rect.Top);
				RenderTransform = XpfPainter.CreateMatrixTransform(transform);
			}
			else
				RenderTransform = new TranslateTransform() { X = rect.Left, Y = rect.Top };
			Width = rect.Width;
			Height = rect.Height;
		}
		public void Recalculate() {
		}
		public void AddFloatingObjectRect(Rect rect, TransformMatrix transform) {
		}
		#endregion
	}
	#endregion
	public class XpfRichEditCommentSelection : XpfRichEditSelection {
		public XpfRichEditCommentSelection() : base() { }
	}
	class RectangleCalculator {
		static readonly int[] Z = new[] { int.MinValue, int.MaxValue };
		List<int> verGrid;
		List<int> horGrid;
		List<List<bool>> matrix;
		List<List<PathSegment>> result;
		enum Corner { 
			LeftTop = 0,
			LeftBot,
			RightBot,
			RightTop
		}
		static Corner Clockwise(Corner a) { return (a == Corner.LeftTop) ? Corner.RightTop : (Corner)(a - 1); }
		static Corner Counterclockwise(Corner a) { return (a == Corner.RightTop) ? Corner.LeftTop : (Corner)(a + 1); }
		static int DeltaColIdx(Corner dir) { 
			switch(dir) {
				case Corner.RightTop:
					return -1;
				case Corner.LeftBot:
					return 1;
				default:
					return 0;
			}
		}
		static int DeltaRowIdx(Corner dir) { 
			switch(dir) {
				case Corner.RightBot:
					return -1;
				case Corner.LeftTop:
					return 1;
				default:
					return 0;
			}
		}
		public RectangleCalculator() { Reset(); }
		public void Reset() {
			verGrid = new List<int>(Z);
			horGrid = new List<int>(Z);
			matrix = new List<List<bool>>(new[] { new List<bool>(new[] { false }) });
			result = null;
		}
		public void Fill(Rect rect) {
			int left = GetColIdx((int)rect.Left);
			int right = GetColIdx((int)rect.Right);
			int top = GetRowIdx((int)rect.Top);
			int bot = GetRowIdx((int)rect.Bottom);
			for(int i = top; i < bot; i++) {
				List<bool> row = matrix[i];
				for(int j = left; j < right; j++)
					row[j] = true;
			}
		}
		public List<List<PathSegment>> Result { 
			get {
				if(object.ReferenceEquals(result, null))
					Calculate();
				return result;
			} 
		}
		void Calculate() {
			result = new List<List<PathSegment>>();
			for(int rowIdx = 0; rowIdx < matrix.Count; rowIdx++) {
				List<bool> rowData = matrix[rowIdx];
				for(int colIdx = 0; colIdx < rowData.Count; colIdx++) {
					if(!rowData[colIdx])
						continue;
					bool[][] domainMatrix = ExtractDomainOfConnectivity(rowIdx, colIdx);
					List<PathSegment> shape = WalkFrontier(domainMatrix, rowIdx, colIdx);
					result.Add(shape);
					FillExterior(domainMatrix);
					Inverse(domainMatrix);
					for(int holeRowIdx = rowIdx; holeRowIdx < domainMatrix.Length; holeRowIdx++) {
						bool[] holeRowData = domainMatrix[holeRowIdx];
						for(int holeColIdx = 0; holeColIdx < holeRowData.Length; holeColIdx++) {
							if(!holeRowData[holeColIdx])
								continue;
							List<PathSegment> hole = WalkFrontier(domainMatrix, holeRowIdx, holeColIdx);							
							result.Add(ReversePath(hole));
							FillAreaWith(domainMatrix, holeRowIdx, holeColIdx, false);
						}
					}
				}
			}
			verGrid = null;
			horGrid = null;
			matrix = null;
		}
		List<PathSegment> WalkFrontier(bool[][] domain, int startCellRowIdx, int startCellColIdx) {
			List<PathSegment> shape = new List<PathSegment>();
			Corner corner = Corner.LeftTop;
			int rowIdx = startCellRowIdx;
			int colIdx = startCellColIdx;
			AddCornerToResult(shape, rowIdx, colIdx, corner);
			bool open = true;
			while(open) {
				Corner nextCorner = Clockwise(corner);
				int nextRowIdx = rowIdx + DeltaRowIdx(nextCorner);
				int nextColIdx = colIdx + DeltaColIdx(nextCorner);
				int angle = -1;
				while(!domain[nextRowIdx][nextColIdx]) {
					nextCorner = Counterclockwise(nextCorner);
					angle++;
					if(angle > 0) {
						AddCornerToResult(shape, rowIdx, colIdx, nextCorner);
						if(rowIdx == startCellRowIdx && colIdx == startCellColIdx && nextCorner == Corner.LeftTop) {
							open = false;
							break;
						}
					}
					nextRowIdx = rowIdx + DeltaRowIdx(nextCorner);
					nextColIdx = colIdx + DeltaColIdx(nextCorner);
				}
				corner = nextCorner;
				rowIdx = nextRowIdx;
				colIdx = nextColIdx;
				if(angle == -1)
					AddCornerToResult(shape, rowIdx, colIdx, corner);
			}
			return shape;
		}
		static List<PathSegment> ReversePath(List<PathSegment> path) {
			if(object.ReferenceEquals(path, null))
				return null;
			List<PathSegment> result = new List<PathSegment>(path.Count);
			foreach (PathSegment segment in path) {
				result.Insert(0, segment);
			}
			return result;
		}
		bool[][] ExtractDomainOfConnectivity(int rowIdx, int colIdx) {
			int rowNum =  matrix.Count;
			int colNum = matrix[0].Count;
			bool[][] result = new bool[matrix.Count][];
			for(int i = 0; i < rowNum; i++)
				result[i] = new bool[colNum];
			ExtractDomainOfConnectivity(result, rowIdx, colIdx);
			return result;
		}
		void ExtractDomainOfConnectivity(bool[][] result, int rowIdx, int colIdx) {
			result[rowIdx][colIdx] = true;
			matrix[rowIdx][colIdx] = false;
			if(matrix[rowIdx + 1][colIdx])
				ExtractDomainOfConnectivity(result, rowIdx + 1, colIdx);
			if(matrix[rowIdx][colIdx + 1])
				ExtractDomainOfConnectivity(result, rowIdx, colIdx + 1);
			if(matrix[rowIdx - 1][colIdx])
				ExtractDomainOfConnectivity(result, rowIdx - 1, colIdx);
			if(matrix[rowIdx][colIdx - 1])
				ExtractDomainOfConnectivity(result, rowIdx, colIdx - 1);
		}
		void FillExterior(bool[][] domain) {
			FillAreaWith(domain, 0, 0, true);
		}
		void FillAreaWith(bool[][] domain, int i, int j, bool val) {
			if (domain[i][j] == val)
				return;
			Queue<System.Drawing.Point> queue = new Queue<System.Drawing.Point>();
			queue.Enqueue(new System.Drawing.Point(i, j));
			while(queue.Count > 0) {
				System.Drawing.Point p = queue.Dequeue();
				int left = p.X;
				int right = p.X;
				int y = p.Y;
				while (left > 0 && domain[left - 1][y] != val)
					left--;
				while (right < domain.Length - 1 && domain[right + 1][y] != val)
					right++;
				for (int x = left; x <= right; x++) {
					domain[x][y] = val;
					if (y > 0 && domain[x][y - 1] != val)
						queue.Enqueue(new System.Drawing.Point(x, y - 1));
					if (y < domain[x].Length - 1 && domain[x][y + 1] != val)
						queue.Enqueue(new System.Drawing.Point(x, y + 1));
				}
			}
		}
		void Inverse(bool[][] matrix) {
			foreach(bool[] row in matrix)
				for(int i = 0; i < row.Length; i++)
					row[i] = !row[i];
		}
		void AddCornerToResult(List<PathSegment> shape, int rowIdx, int colIdx, Corner corner) { 
			switch(corner) {
				case Corner.LeftTop:
					AddLeftTopToResult(shape, rowIdx, colIdx);
					break;
				case Corner.LeftBot:
					AddLeftBotToResult(shape, rowIdx, colIdx);
					break;
				case Corner.RightBot:
					AddRightBotToResult(shape, rowIdx, colIdx);
					break;
				case Corner.RightTop:
					AddRightTopToResult(shape, rowIdx, colIdx);
					break;
			}
		}
		void AddLeftTopToResult(List<PathSegment> shape, int rowIdx, int colIdx) { AddPointToResult(shape, rowIdx, colIdx); }
		void AddLeftBotToResult(List<PathSegment> shape, int rowIdx, int colIdx) { AddPointToResult(shape, rowIdx + 1, colIdx); }
		void AddRightBotToResult(List<PathSegment> shape, int rowIdx, int colIdx) { AddPointToResult(shape, rowIdx + 1, colIdx + 1); }
		void AddRightTopToResult(List<PathSegment> shape, int rowIdx, int colIdx) { AddPointToResult(shape, rowIdx, colIdx + 1); }
		void AddPointToResult(List<PathSegment> shape, int rowIdx, int colIdx) {
			shape.Add(new LineSegment() { Point = new Point(horGrid[colIdx], verGrid[rowIdx]) });
		}
		int GetRowIdx(int y) { return GetIdx(y, verGrid, SplitRow); }
		int GetColIdx(int x) { return GetIdx(x, horGrid, SplitColumn); }
		int GetIdx(int coordVal, List<int> grid, Action<int> split) {
			int i = 1;
			while(i < grid.Count) {
				if(grid[i] < coordVal) {
					i++;
					continue;
				}
				if(grid[i] == coordVal)
					return i;
				grid.Insert(i, coordVal);
				split(i - 1);
				return i;
			}
			return -1;
		}
		void SplitRow(int rowIdx) {
			matrix.Insert(rowIdx, new List<bool>(matrix[rowIdx]));
		}
		void SplitColumn(int colIdx) {
			foreach(List<bool> row in matrix)
				row.Insert(colIdx, row[colIdx]);
		}
	}
}
