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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
namespace DevExpress.Xpf.Carousel.GraphFunction {
	public enum GraphNodeType { Line, Const, Spline }
	public enum GraphNodeHitTest { None, Line, Point1, Point2, Point3, Point4 }
	public class GraphNodeHitInfo {
		Point hitPoint;
		GraphNodeHitTest hitTest;
		public GraphNodeHitInfo() {
			this.hitPoint = GraphNode.InvalidPoint;
			this.hitTest = GraphNodeHitTest.None;
		}
		public Point HitPoint {
			get { return hitPoint; }
			set { hitPoint = value; }
		}
		public GraphNodeHitTest HitTest {
			get { return hitTest; }
			set { hitTest = value; }
		}
		public static bool HitTestEquals(GraphNodeHitInfo info1, GraphNodeHitInfo info2) {
			GraphNodeHitTest test1 = info1 == null ? GraphNodeHitTest.None : info1.HitTest;
			GraphNodeHitTest test2 = info2 == null ? GraphNodeHitTest.None : info2.HitTest;
			return test1 == test2;
		}
	}
	public class GraphNode {
		public static Point InvalidPoint = new Point(double.NaN, double.NaN);
		Graph owner;
		GraphNodeType type;
		Point p1;
		Point p2;
		Point p3;
		Point p4;
		double a;
		double b;
		double c;
		bool splitNext;
		GraphNodeHitInfo hotInfo;
		GraphNodeHitInfo pressedInfo;
		GraphNodeHitInfo selectedInfo;
		public GraphNode() { 
			this.type = GraphNodeType.Line;
			this.splitNext = true;
			this.p1 = InvalidPoint;
			this.p2 = InvalidPoint;
			this.p3 = InvalidPoint;
			this.p4 = InvalidPoint;
		}
		public GraphNodeType Type {
			get { return type; }
			set {
				if(Type == value) return;
				type = value;
				OnNodeTypeChaged();
			}
		}
		public Point P1 { 
			get { return p1; }
			set {
				if(P1 == value) return;
				p1 = value;
				OnLayoutChanged();
			}
		}
		public Point P2 {
			get { return p2; }
			set {
				if(P2 == value) return;
				p2 = value;
				OnLayoutChanged();
			}
		}
		public Point P3 {
			get { return p3; }
			set {
				if(P3 == value) return;
				p3 = value;
				OnLayoutChanged();
			}
		}
		public Point P4 {
			get { return p4; }
			set {
				if(P4 == value) return;
				p4 = value;
				OnLayoutChanged();
			}
		}
		public Point R1 { get { return new Point(P3.X - P1.X, P3.Y - P1.Y); } }
		public Point R2 { get { return new Point(P4.X - P2.X, P4.Y - P2.Y); } }
		public double K {
			get {
				if(Type == GraphNodeType.Line)
					return (P2.Y - P1.Y) / (P2.X - P1.X);
				return 1.0;
			}
		}
		public bool SplitNext {
			get { return splitNext; }
			set {
				if(SplitNext == value) return;
				splitNext = value;
				OnLayoutChanged();
			}
		}
		public int Index { get { return Owner.Nodes.IndexOf(this); } }
		internal bool IsFirstNode { get { return Index == 0; } }
		internal bool IsFirstNodeAfterBroken { get { return Index == 0 || !Owner.Nodes[Index - 1].SplitNext || Owner.Nodes[Index-1].Type == GraphNodeType.Const; } }
		public GraphNodeHitInfo HotInfo { 
			get {
				if(hotInfo == null)
					hotInfo = new GraphNodeHitInfo();
				return hotInfo;
			}
			set {
				GraphNodeHitInfo oldInfo = HotInfo;
				if(HotInfo == value) return;
				hotInfo = value;
				if(!GraphNodeHitInfo.HitTestEquals(oldInfo, HotInfo))
					OnPropertiesChanged();
			}
		}
		public GraphNodeHitInfo PressedInfo {
			get {
				if(pressedInfo == null)
					pressedInfo = new GraphNodeHitInfo();
				return pressedInfo;
			}
			set {
				GraphNodeHitInfo oldInfo = PressedInfo;
				if(PressedInfo == value) return;
				pressedInfo = value;
				if(!GraphNodeHitInfo.HitTestEquals(oldInfo, PressedInfo))
					OnPropertiesChanged();
			}
		}
		public GraphNodeHitInfo SelectedInfo {
			get {
				if(selectedInfo == null)
					selectedInfo = new GraphNodeHitInfo();
				return selectedInfo;
			}
			set {
				GraphNodeHitInfo oldInfo = SelectedInfo;
				if(SelectedInfo == value) return;
				selectedInfo = value;
				if(!GraphNodeHitInfo.HitTestEquals(oldInfo, SelectedInfo))
					OnPropertiesChanged();
			}
		}
		public Graph Owner { get { return owner; } }
		internal void SetOwner(Graph graph) {
			this.owner = graph;
			OnPropertiesChanged();
		}
		protected double A { get { return a; } }
		protected double B { get { return b; } }
		protected double C { get { return c; } }
		protected double D { get { return P1.Y; } }
		protected virtual void CalcSplineParams() {
			this.a = (-p1.Y + 3.0 * (p3.Y - p4.Y) + p2.Y);
			this.b = (-6.0 * p3.Y + 3.0 * (p1.Y + p4.Y));
			this.c = 3.0 * (p3.Y - p1.Y);
		}
		protected virtual void OnLayoutChanged() { 
			if(Type == GraphNodeType.Spline)
				CalcSplineParams();
			OnPropertiesChanged();
		}
		protected virtual void OnPropertiesChanged() {
			if(Owner != null)
				Owner.OnPropertiesChanged();
		}
		public double Calculate(Point pt) {
			return Calculate(pt.X);
		}
		public double Calculate(double t) {
			if(Type == GraphNodeType.Const) return CalcConst(t);
			if(Type == GraphNodeType.Line) return CalcLine(t);
			return CalcSpline(t);
		}
		protected double CalcConst(double t) { 
			return P1.Y; 
		}
		protected double CalcLine(double t) { 
			return P1.Y + (t - P1.X) * K; 
		}
		protected double CalcSpline(double t) {
			double locTime = P2.X - P1.X;
			t = locTime == 0.0? 1.0: (t - P1.X) / locTime;
			double t2 = t * t;
			double t3 = t2 * t;
			return A * t3 + B * t2 + C * t + D;
		}
		protected virtual void OnNodeTypeChaged() {
			double dx = (P2.X - P1.X) / 3.0;
			double dy = (P2.Y - P1.Y) / 3.0;
			if(Type == GraphNodeType.Spline && (double.IsNaN(P3.X) || double.IsNaN(P4.X) || double.IsNaN(P3.Y) || double.IsNaN(P4.Y))) {
				P3 = new Point(P1.X + dx, P1.Y + dy);
				P4 = new Point(P2.X - dx, P2.Y - dy);
			}
			OnPropertiesChanged();
		}
		public double ConstrainDeltaX(double x, double deltaX, double minX, double maxX) {
			if(x + deltaX < minX) deltaX = minX - x;
			else if(x + deltaX > maxX) deltaX = maxX - x;
			return deltaX;
		}
		public void MovePoint1(Point delta) { MovePoint1(delta, double.MinValue, double.MaxValue); }
		public void MovePoint2(Point delta) { MovePoint2(delta, double.MinValue, double.MaxValue); }
		public void MovePoint3(Point delta) { MovePoint3(delta, double.MinValue, double.MaxValue); }
		public void MovePoint4(Point delta) { MovePoint4(delta, double.MinValue, double.MaxValue); }
		public void MovePoint1(Point delta, double minX, double maxX) {
			delta.X = ConstrainDeltaX(P1.X, delta.X, minX, maxX);
			P1 = new Point(P1.X + delta.X, P1.Y + delta.Y);
			if(Type == GraphNodeType.Spline)
				MovePoint3(delta);
		}
		public void MovePoint2(Point delta, double minX, double maxX) {
			delta.X = ConstrainDeltaX(P2.X, delta.X, minX, maxX);
			P2 = new Point(P2.X + delta.X, P2.Y + delta.Y);
			if(Type == GraphNodeType.Spline)
				MovePoint4(delta);
		}
		public void MovePoint3(Point delta, double minX, double maxX) {
			delta.X = ConstrainDeltaX(P3.X, delta.X, minX, maxX);
			P3 = new Point(P3.X + delta.X, P3.Y + delta.Y);
		}
		public void MovePoint4(Point delta, double minX, double maxX) {
			delta.X = ConstrainDeltaX(P4.X, delta.X, minX, maxX);
			P4 = new Point(P4.X + delta.X, P4.Y + delta.Y);
		}
		public IGraphEditor GraphControl { get { return Owner.GraphControl; } }
		protected double NodePointSize { get { return GraphControl.NodePointSize; } }
		protected Rect GetPointRectangle(Point pt) {
			return new Rect(pt.X - GraphControl.GetXLen(NodePointSize) * 0.5, pt.Y - GraphControl.GetYLen(NodePointSize) * 0.5, GraphControl.GetXLen(NodePointSize), GraphControl.GetYLen(NodePointSize));
		}
		protected virtual GraphNodeHitInfo ProcessHitTestCore(Point pt) {
			GraphNodeHitInfo hi = new GraphNodeHitInfo();
			hi.HitPoint = pt;
			Rect rect1 = GetPointRectangle(P1);
			Rect rect2 = Type == GraphNodeType.Const? GetPointRectangle(new Point(P2.X, P1.Y)): GetPointRectangle(P2);
			Rect rect3 = GetPointRectangle(P3);
			Rect rect4 = GetPointRectangle(P4);
			if(Type == GraphNodeType.Spline && rect3.Contains(pt)) {
				hi.HitTest = GraphNodeHitTest.Point3;
				return hi;
			}
			if(Type == GraphNodeType.Spline && rect4.Contains(pt)) {
				hi.HitTest = GraphNodeHitTest.Point4;
				return hi;
			}
			if(rect1.Contains(pt)) {
				hi.HitTest = GraphNodeHitTest.Point1;
				return hi;
			}
			if(rect2.Contains(pt)) {
				hi.HitTest = GraphNodeHitTest.Point2;
				return hi;
			}
			if(P1.X <= pt.X && pt.X <= P2.X) {
				if(Math.Abs(pt.Y - Calculate(pt.X)) < 5.0 / Owner.GraphControl.ScaleY) hi.HitTest = GraphNodeHitTest.Line;
			}
			return hi;
		}
		public virtual void ProcessHitTest(MouseButton button, MouseButtonState state, Point pt) {
			if(button == MouseButton.Left) {
				PressedInfo = ProcessHitTestCore(pt);
				SelectedInfo = PressedInfo;
				HotInfo = new GraphNodeHitInfo();
			}
			else {
				HotInfo = ProcessHitTestCore(pt);
				PressedInfo = new GraphNodeHitInfo();
			}
		}
	}
}
