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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using DevExpress.Diagram.Core;
using DevExpress.XtraDiagram.Extensions;
namespace DevExpress.XtraDiagram.Base {
	public struct DiagramElementPoint {
		Point logicalPoint;
		Point displayPoint;
		public DiagramElementPoint(Point logicalPoint, Point displayPoint) {
			this.logicalPoint = logicalPoint;
			this.displayPoint = displayPoint;
		}
		public Point LogicalPoint { get { return logicalPoint; } }
		public Point DisplayPoint { get { return displayPoint; } }
		public void Offset(int dx, int dy) {
			OffsetDisplay(dx, dy);
			OffsetLogical(dx, dy);
		}
		public void OffsetLogical(int dx, int dy) {
			this.logicalPoint.Offset(dx, dy);
		}
		public void OffsetDisplay(int dx, int dy) {
			this.displayPoint.Offset(dx, dy);
		}
		public bool IsEmpty {
			get { return LogicalPoint.IsEmpty && DisplayPoint.IsEmpty; }
		}
		public DiagramElementBounds CreateRect(Size gripSize) {
			Rectangle logicalRect = gripSize.CreateRect(LogicalPoint);
			Rectangle displayRect = gripSize.CreateRect(DisplayPoint);
			return new DiagramElementBounds(logicalRect, displayRect);
		}
		public static DiagramElementPoint EmptyPoint = new DiagramElementPoint();
	}
	public struct DiagramElementBounds {
		Rectangle logicalBounds;
		Rectangle displayBounds;
		public DiagramElementBounds(Rectangle bounds) : this(bounds, bounds) { }
		public DiagramElementBounds(Rectangle logicalBounds, Rectangle displayBounds) {
			this.logicalBounds = logicalBounds;
			this.displayBounds = displayBounds;
		}
		public Rectangle LogicalBounds { get { return logicalBounds; } }
		public Rectangle DisplayBounds { get { return displayBounds; } }
		public bool IsEmpty {
			get { return LogicalBounds.IsEmpty && DisplayBounds.IsEmpty; }
		}
		public static DiagramElementBounds EmptyBounds = new DiagramElementBounds();
		public void OffsetDisplay(int dx, int dy) {
			this.displayBounds.Offset(dx, dy);
		}
		public void OffsetLogical(int dx, int dy) {
			this.logicalBounds.Offset(dx, dy);
		}
		public void SetDisplayRect(Rectangle rect) {
			SetDisplayRect(rect.X, rect.Y, rect.Width, rect.Height);
		}
		public void SetDisplayRect(int x, int y, int width, int height) {
			this.displayBounds.X = x;
			this.displayBounds.Y = y;
			this.displayBounds.Width = width;
			this.displayBounds.Height = height;
		}
		public void SetLogicalRect(Rectangle rect) {
			SetLogicalRect(rect.X, rect.Y, rect.Width, rect.Height);
		}
		public void SetLogicalRect(int x, int y, int width, int height) {
			this.logicalBounds.X = x;
			this.logicalBounds.Y = y;
			this.logicalBounds.Width = width;
			this.logicalBounds.Height = height;
		}
		public DiagramElementPoint GetTopLeftPt() {
			return new DiagramElementPoint(LogicalBounds.GetTopLeftPt(), DisplayBounds.GetTopLeftPt());
		}
		public DiagramElementPoint GetTopPt() {
			return new DiagramElementPoint(LogicalBounds.GetTopPt(), DisplayBounds.GetTopPt());
		}
		public DiagramElementPoint GetTopRightPt() {
			return new DiagramElementPoint(LogicalBounds.GetTopRightPt(), DisplayBounds.GetTopRightPt());
		}
		public DiagramElementPoint GetLeftPt() {
			return new DiagramElementPoint(LogicalBounds.GetLeftPt(), DisplayBounds.GetLeftPt());
		}
		public DiagramElementPoint GetRightPt() {
			return new DiagramElementPoint(LogicalBounds.GetRightPt(), DisplayBounds.GetRightPt());
		}
		public DiagramElementPoint GetBottomLeftPt() {
			return new DiagramElementPoint(LogicalBounds.GetBottomLeftPt(), DisplayBounds.GetBottomLeftPt());
		}
		public DiagramElementPoint GetBottomPt() {
			return new DiagramElementPoint(LogicalBounds.GetBottomPt(), DisplayBounds.GetBottomPt());
		}
		public DiagramElementPoint GetBottomRightPt() {
			return new DiagramElementPoint(LogicalBounds.GetBottomRightPt(), DisplayBounds.GetBottomRightPt());
		}
	}
	public enum SizeGripKind {
		None, TopLeft, Top, TopRight, Left, Right, BottomLeft, Bottom, BottomRight
	}
	public class Pair<TFirst, TSecond> {
		readonly TFirst first;
		readonly TSecond second;
		public Pair(TFirst first, TSecond second) {
			this.first = first;
			this.second = second;
		}
		public TFirst First { get { return first; } }
		public TSecond Second { get { return second; } }
	}
	public class PointPair : Pair<Point, Point> {
		public PointPair(Point first, Point second)
			: base(first, second) {
		}
		public int Horizontal {
			get { return Math.Abs(End.X - Start.X); }
		}
		public int Vertical {
			get { return Math.Abs(End.Y - Start.Y); }
		}
		public Point Start { get { return First; } }
		public Point End { get { return Second; } }
		public PointPair SetHorz(int lenghtValue) {
			int horzVal = Horizontal - lenghtValue;
			return new PointPair(new Point(Start.X + horzVal / 2, Start.Y), new Point(End.X - horzVal / 2, End.Y));
		}
		public PointPair SetVert(int lenghtValue) {
			int vertVal = Vertical - lenghtValue;
			return new PointPair(new Point(Start.X, Start.Y + vertVal / 2), new Point(End.X, End.Y - vertVal / 2));
		}
	}
	public struct DiagramLineDrawArgs {
		int x1;
		int y1;
		int x2;
		int y2;
		public DiagramLineDrawArgs(int x1, int y1, int x2, int y2) {
			this.x1 = x1;
			this.y1 = y1;
			this.x2 = x2;
			this.y2 = y2;
		}
		public int X1 { get { return x1; } }
		public int Y1 { get { return y1; } }
		public int X2 { get { return x2; } }
		public int Y2 { get { return y2; } }
	}
	public class DiagramItemSelection {
		readonly DiagramElementBounds bounds;
		readonly Dictionary<SizeGripKind, DiagramElementBounds> rects;
		readonly DiagramElementBounds rotationGripBounds;
		public DiagramItemSelection()
			: this(DiagramElementBounds.EmptyBounds, DiagramElementBounds.EmptyBounds) {
		}
		public DiagramItemSelection(DiagramElementBounds elementBounds, DiagramElementBounds rotationGripBounds) {
			this.bounds = elementBounds;
			this.rotationGripBounds = rotationGripBounds;
			this.rects = new Dictionary<SizeGripKind, DiagramElementBounds>();
		}
		public DiagramElementBounds this[SizeGripKind gripKind] {
			get { return this.rects[gripKind]; }
			set { this.rects[gripKind] = value; }
		}
		public bool InSizeGrip(Point pt) {
			foreach(var pair in rects) {
				if(pair.Value.LogicalBounds.Contains(pt)) return true;
			}
			return false;
		}
		public bool InRotationGrip(Point pt) {
			return RotationGripBounds.DisplayBounds.Contains(pt);
		}
		public SizeGripKind GetGripKind(Point pt) {
			foreach(var pair in rects) {
				if(pair.Value.LogicalBounds.Contains(pt)) return pair.Key;
			}
			return SizeGripKind.None;
		}
		public IEnumerable<Rectangle> DisplayRects {
			get { return GetDisplayRects(); }
		}
		protected IEnumerable<Rectangle> GetDisplayRects() {
			foreach(var pair in this.rects)
				yield return pair.Value.DisplayBounds;
		}
		public DiagramElementBounds RotationGripBounds { get { return rotationGripBounds; } }
		public DiagramElementBounds Bounds { get { return bounds; } }
	}
	public class ConfigurableArea {
		ConfigurationParameter[] parameters;
		public ConfigurableArea(IEnumerable<ConfigurationParameter> parameters)
			: this(parameters.ToArray()) {
		}
		public ConfigurableArea(ConfigurationParameter[] parameters) {
			this.parameters = parameters;
		}
		public bool InParameter(Point pt) {
			return Array.Exists(Parameters, parameter => parameter.Bounds.LogicalBounds.Contains(pt));
		}
		public ConfigurationParameter GetParameter(Point pt) {
			return Array.Find(Parameters, parameter => parameter.Bounds.LogicalBounds.Contains(pt));
		}
		public ConfigurationParameter[] Parameters { get { return parameters; } }
	}
	public struct ConfigurationParameter {
		readonly DiagramElementBounds bounds;
		readonly ParameterViewInfo parameter;
		public ConfigurationParameter(DiagramElementBounds bounds, ParameterViewInfo parameter) {
			this.bounds = bounds;
			this.parameter = parameter;
		}
		public DiagramElementBounds Bounds { get { return bounds; } }
		public ParameterViewInfo Parameter { get { return parameter; } }
	}
	public struct ConnectorSelection {
		readonly DiagramElementBounds beginPoint;
		readonly DiagramElementBounds endPoint;
		readonly DiagramElementBounds[] intermediatePoints;
		public ConnectorSelection(DiagramElementBounds beginPoint, DiagramElementBounds endPoint, DiagramElementBounds[] intermediatePoints) {
			this.beginPoint = beginPoint;
			this.endPoint = endPoint;
			this.intermediatePoints = intermediatePoints;
		}
		public void ForEachIntermediatePoint(Action<DiagramElementBounds> action) {
			Array.ForEach(IntermediatePoints, action);
		}
		public bool InBeginPoint(Point pt) {
			return BeginPoint.LogicalBounds.Contains(pt);
		}
		public bool InEndPoint(Point pt) {
			return EndPoint.LogicalBounds.Contains(pt);
		}
		public bool InIntermediatePoint(Point pt) {
			return Array.Exists(IntermediatePoints, rect => rect.LogicalBounds.Contains(pt));
		}
		public bool Contains(Point pt) {
			return InBeginPoint(pt) || InEndPoint(pt) || InIntermediatePoint(pt);
		}
		public int GetPointIndex(Point point) {
			return Array.FindIndex(IntermediatePoints, rect => rect.LogicalBounds.Contains(point));
		}
		public DiagramElementBounds BeginPoint { get { return beginPoint; } }
		public DiagramElementBounds EndPoint { get { return endPoint; } }
		public DiagramElementBounds[] IntermediatePoints { get { return intermediatePoints; } }
	}
	public struct RotationGrip {
		public static readonly RotationGrip Empty = new RotationGrip(Size.Empty, 0);
		readonly Size gripSize;
		readonly int vertOffset;
		public RotationGrip(Size gripSize, int vertOffset) {
			this.gripSize = gripSize;
			this.vertOffset = vertOffset;
		}
		public Size GripSize { get { return gripSize; } }
		public int VertOffset { get { return vertOffset; } }
	}
	public interface IXtraPathView {
		ShapeGeometry Shape { get; }
		string Text { get; }
		Rectangle TextBounds { get; }
		double Angle { get; }
		int ItemId { get; }
		Size Size { get; }
	}
	internal class DiagramSimplePathView : IXtraPathView {
		readonly ShapeGeometry shape;
		public DiagramSimplePathView(ShapeGeometry shape) {
			this.shape = shape;
		}
		public virtual double Angle { get { return 0; } }
		public ShapeGeometry Shape { get { return shape; } }
		public virtual string Text { get { return string.Empty; } }
		public virtual Rectangle TextBounds { get { return Rectangle.Empty; } }
		public int ItemId { get { return Ids.InvalidId; } }
		public Size Size { get { return Size.Empty; } }
	}
	public struct TextPathOptions {
		readonly Font font;
		readonly StringAlignment horzAlign;
		readonly StringAlignment vertAlign;
		public TextPathOptions(Font font, StringAlignment horzAlign, StringAlignment vertAlign) {
			this.font = font;
			this.horzAlign = horzAlign;
			this.vertAlign = vertAlign;
		}
		public override bool Equals(object obj) {
			TextPathOptions other = (TextPathOptions)obj;
			return IsEquals(other);
		}
		bool IsEquals(TextPathOptions other) {
			return HorzAlign == other.HorzAlign && VertAlign == other.VertAlign && IsFontEquals(Font, other.Font);
		}
		bool IsFontEquals(Font x, Font y) {
			return x.FontFamily.Name == y.FontFamily.Name && x.Style == y.Style && (Math.Abs(x.Size - y.Size) < 0.1);
		}
		public override int GetHashCode() {
			return Font.GetHashCode() ^ HorzAlign.GetHashCode() ^ VertAlign.GetHashCode();
		}
		public Font Font { get { return font; } }
		public StringAlignment HorzAlign { get { return horzAlign; } }
		public StringAlignment VertAlign { get { return vertAlign; } }
	}
	public class ShapedSelection {
		readonly DiagramElementBounds[] points;
		public ShapedSelection(DiagramElementBounds[] points) {
			this.points = points;
		}
		public ShapedSelection(IEnumerable<DiagramElementBounds> points)
			: this(points.ToArray()) {
		}
		public void ForEachPoint(Action<DiagramElementBounds> action) {
			Array.ForEach(Points, action);
		}
		public DiagramElementBounds[] Points { get { return points; } }
	}
	public struct ConnectorSelectionColors {
		Color border;
		Color sizeGrip;
		Color gripCorner;
		public ConnectorSelectionColors(Color border, Color sizeGrip, Color gripCorner) {
			this.border = border;
			this.sizeGrip = sizeGrip;
			this.gripCorner = gripCorner;
		}
		public Color Border { get { return border; } }
		public Color SizeGrip { get { return sizeGrip; } }
		public Color GripCorner { get { return gripCorner; } }
	}
	public struct ConnectorSelectionFreeBeginPointColors {
		Color border;
		Color corner;
		public ConnectorSelectionFreeBeginPointColors(Color border, Color corner) {
			this.border = border;
			this.corner = corner;
		}
		public Color Border { get { return border; } }
		public Color Corner { get { return corner; } }
	}
	public struct ConnectorSelectionFreeEndPointColors {
		Color border;
		Color back;
		Color corner;
		public ConnectorSelectionFreeEndPointColors(Color border, Color back, Color corner) {
			this.border = border;
			this.back = back;
			this.corner = corner;
		}
		public Color Border { get { return border; } }
		public Color Back { get { return back; } }
		public Color Corner { get { return corner; } }
	}
	public struct ConnectorSelectionConnectedPointColors {
		Color back;
		Color border;
		public ConnectorSelectionConnectedPointColors(Color back, Color border) {
			this.back = back;
			this.border = border;
		}
		public Color Back { get { return back; } }
		public Color Border { get { return border; } }
	}
	public struct ConnectorSelectionIntermediatePointColors {
		Color back;
		public ConnectorSelectionIntermediatePointColors(Color back) {
			this.back = back;
		}
		public Color Back { get { return back; } }
	}
	public struct DiagramIdleTask {
		bool updateUIOnIdle;
		bool updateProperties;
		bool updateUICommands;
		bool checkPaintCache;
		public DiagramIdleTask(bool updateUIOnIdle = false, bool updateProperties = false, bool updateUICommands = false, bool checkPaintCache = false) {
			this.updateUIOnIdle = updateUIOnIdle;
			this.updateProperties = updateProperties;
			this.updateUICommands = updateUICommands;
			this.checkPaintCache = checkPaintCache;
		}
		public void Set(bool? updateUIOnIdle = null, bool? updateProperties = null, bool? updateUICommands = null, bool? checkPaintCache = null) {
			if(updateUIOnIdle.HasValue) {
				this.updateUIOnIdle = updateUIOnIdle.Value;
			}
			if(updateProperties.HasValue) {
				this.updateProperties = updateProperties.Value;
			}
			if(updateUICommands.HasValue) {
				this.updateUICommands = updateUICommands.Value;
			}
			if(checkPaintCache.HasValue) {
				this.checkPaintCache = checkPaintCache.Value;
			}
		}
		public void Merge(DiagramIdleTask task) {
			if(task.UpdateUIOnIdle)
				this.updateUIOnIdle = true;
			if(task.UpdateProperties)
				this.updateProperties = true;
			if(task.UpdateUICommands)
				this.updateUICommands = true;
			if(task.CheckPaintCache)
				this.checkPaintCache = true;
		}
		public override bool Equals(object obj) {
			DiagramIdleTask other = (DiagramIdleTask)obj;
			return UpdateUIOnIdle == other.UpdateUIOnIdle && UpdateProperties == other.UpdateProperties && UpdateUICommands == other.UpdateUICommands && CheckPaintCache == other.CheckPaintCache;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public static DiagramIdleTask EmptyTask = new DiagramIdleTask();
		public static DiagramIdleTask UpdatePropertyGridTask = new DiagramIdleTask(true, true);
		public static DiagramIdleTask CheckPaintCacheTask = new DiagramIdleTask(true, false, false, true);
		public static DiagramIdleTask UpdateAllTask = new DiagramIdleTask(true, true, true, true);
		public bool UpdateUIOnIdle { get { return updateUIOnIdle; } }
		public bool UpdateProperties { get { return updateProperties; } }
		public bool UpdateUICommands { get { return updateUICommands; } }
		public bool CheckPaintCache { get { return checkPaintCache; } }
	}
}
