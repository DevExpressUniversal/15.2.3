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
using System.Collections;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows;
namespace DevExpress.Xpf.Carousel.GraphFunction {
	public class GraphNodeCollection : CollectionBase {
		Graph owner;
		public GraphNodeCollection(Graph owner) { this.owner = owner; }
		public int Add(GraphNode node) { return List.Add(node); }
		public void Insert(int index, GraphNode node) { List.Insert(index, node); }
		public void Remove(GraphNode node) { List.Remove(node); }
		public  GraphNode this[int index] { 
			get { return List[index] as GraphNode; }
			set { List[index] = value; }
		} 
		public int IndexOf(GraphNode node) { return List.IndexOf(node); }
		public Graph Owner { get { return owner; } }
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			((GraphNode)value).SetOwner(Owner);
		}
		protected override void OnRemoveComplete(int index, object value) {
			base.OnRemoveComplete(index, value);
			((GraphNode)value).SetOwner(null);
		}
	}
	public class GraphCollection : CollectionBase {
		IGraphEditor graphControl = null;
		public GraphCollection() : this(null) { }
		public GraphCollection(IGraphEditor graphControl) {
			this.graphControl = graphControl;
		}
		public IGraphEditor GraphControl {
			get { return graphControl; }
			set {
				if(GraphControl == value) return;
				graphControl = value;
				OnPropertiesChanged();
			}
		}
		protected internal virtual void OnPropertiesChanged() {
			if(GraphControl != null)
				GraphControl.UpdateView();
		}
		public int Add(Graph graph) { return List.Add(graph); }
		public void Insert(int index, Graph graph) { List.Insert(index, graph); }
		public void Remove(Graph graph) { List.Remove(graph); }
		public Graph this[int index] { get { return List[index] as Graph; } set { List[index] = value; } }
		public int IndexOf(Graph graph) { return List.IndexOf(graph); }
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			((Graph)value).SetColl(this);
		}
		protected override void OnRemoveComplete(int index, object value) {
			base.OnRemoveComplete(index, value);
			((Graph)value).SetColl(null);
		}
	}
	public class GraphDrawInfo {
		Pen normalPen;
		Pen selectedPen;
		Pen hotTrackPen;
		Pen pressedPen;
		Pen splineWeightPen;
		Pen selectedLinePen;
		Pen selectedHotTrackPen;
		Pen selectedPressedPen;
		Graph graph;
		public GraphDrawInfo(Graph graph) {
			this.graph = graph;
		}
		public Graph Graph { get { return graph; } }
		public Pen NormalLinePen {
			get {
				if(normalPen == null) {
					normalPen = new Pen(new SolidColorBrush(Graph.Color), Graph.Width);
				}
				normalPen.DashStyle = Graph.Style;
				return normalPen;
			}
		}
		public Pen HotTrackLinePen {
			get {
				if(hotTrackPen == null) {
					hotTrackPen = new Pen(new SolidColorBrush(Graph.OwnerColl.GraphControl.HotTrackColor), Graph.Width);
				}
				hotTrackPen.DashStyle = Graph.Style;
				return hotTrackPen;
			}
		}
		public Pen PressedLinePen {
			get {
				if(pressedPen == null) {
					pressedPen = new Pen(new SolidColorBrush(Graph.OwnerColl.GraphControl.PressedColor), Graph.Width);
				}
				pressedPen.DashStyle = Graph.Style;
				return pressedPen;
			}
		}
		public Pen SelectedPointPen {
			get {
				if(selectedPen == null) {
					selectedPen = new Pen(new SolidColorBrush(Graph.OwnerColl.GraphControl.HotTrackColor), Graph.Width * 2);
				}
				selectedPen.DashStyle = Graph.Style;
				return selectedPen;
			}
		}
		public Pen SelectedLinePen {
			get {
				if(selectedLinePen == null) {
					selectedLinePen = new Pen(new SolidColorBrush(Graph.Color), Graph.Width * 2);
				}
				selectedLinePen.DashStyle = Graph.Style;
				return selectedLinePen;
			}
		}
		public Pen SelectedHotTrackPen {
			get {
				if(selectedHotTrackPen == null) {
					selectedHotTrackPen = new Pen(new SolidColorBrush(Graph.OwnerColl.GraphControl.HotTrackColor), Graph.Width * 2);
				}
				selectedHotTrackPen.DashStyle = Graph.Style;
				return selectedHotTrackPen;
			}
		}
		public Pen SelectedPressedPen {
			get {
				if(selectedPressedPen == null) {
					selectedPressedPen = new Pen(new SolidColorBrush(Graph.OwnerColl.GraphControl.PressedColor), Graph.Width * 2);
				}
				selectedPressedPen.DashStyle = Graph.Style;
				return selectedPressedPen;
			}
		}
		public Pen SplineWeightPen {
			get {
				if(splineWeightPen == null) {
					splineWeightPen = new Pen(new SolidColorBrush(Graph.OwnerColl.GraphControl.SplineWeightColor), 1);
				}
				return splineWeightPen;
			}
		}
		public Pen SelectedSplineWeightPen {
			get {
				if(splineWeightPen == null) {
					splineWeightPen = new Pen(new SolidColorBrush(Graph.OwnerColl.GraphControl.SplineWeightColor), Graph.Width * 2);
				}
				return splineWeightPen;
			}
		}
		public Pen NormalPen {
			get {
				NormalLinePen.DashStyle = DashStyles.Solid;
				return NormalLinePen;
			}
		}
		public Pen HotTrackPen {
			get {
				HotTrackLinePen.DashStyle = DashStyles.Solid;
				return HotTrackLinePen;
			}
		}
		public Pen PressedPen {
			get {
				PressedLinePen.DashStyle = DashStyles.Solid;
				return PressedLinePen;
			}
		}
	}
	public class GraphPainter {
		GraphDrawInfo info;
		Graph graph;
		public GraphPainter(Graph graph) {
			this.graph = graph;
		}
		public Graph Graph { get { return graph; } }
		public GraphDrawInfo DrawInfo {
			get {
				if(info == null)
					info = new GraphDrawInfo(Graph);
				return info;
			}
			set {
				info = value;
			}
		}
		protected virtual bool Contains(Point scrPoint1, Point scrPoint2) {
			return !((scrPoint1.Y < 0.0f && scrPoint2.Y < 0.0f) || (scrPoint1.Y > GraphControl.ClientRect.Height && scrPoint2.Y > GraphControl.ClientRect.Height));
		}
		bool IsNodeLineSelected(GraphNode node) {
			return node.SelectedInfo != null && node.SelectedInfo.HitTest == GraphNodeHitTest.Line;
		}
		protected virtual Pen GetLinePen(GraphNode node) {
			if(node.PressedInfo.HitTest == GraphNodeHitTest.Line) {
				if(IsNodeLineSelected(node)) return DrawInfo.SelectedHotTrackPen;
				return DrawInfo.PressedLinePen;
			}
			if(node.HotInfo.HitTest == GraphNodeHitTest.Line) {
				if(IsNodeLineSelected(node)) return DrawInfo.SelectedPressedPen;
				return DrawInfo.HotTrackLinePen;
			}
			if(node.SelectedInfo.HitTest != GraphNodeHitTest.None) return DrawInfo.SelectedLinePen;
			return DrawInfo.NormalLinePen;
		}
		bool IsNodeP1Selected(GraphNode node) {
			return node.SelectedInfo != null && node.SelectedInfo.HitTest == GraphNodeHitTest.Point1;
		}
		bool IsNodeP2Selected(GraphNode node) {
			return node.SelectedInfo != null && node.SelectedInfo.HitTest == GraphNodeHitTest.Point2;
		}
		bool IsNodeP3Selected(GraphNode node) {
			return node.SelectedInfo != null && node.SelectedInfo.HitTest == GraphNodeHitTest.Point3;
		}
		bool IsNodeP4Selected(GraphNode node) {
			return node.SelectedInfo != null && node.SelectedInfo.HitTest == GraphNodeHitTest.Point4;
		}
		protected virtual Pen GetPoint1Pen(GraphNode node) {
			if(node.PressedInfo.HitTest == GraphNodeHitTest.Point1) return DrawInfo.PressedPen;
			if(node.HotInfo.HitTest == GraphNodeHitTest.Point1) return DrawInfo.HotTrackPen;
			if(IsNodeP1Selected(node)) return DrawInfo.SelectedPointPen;
			return DrawInfo.NormalPen;
		}
		protected virtual Pen GetPoint2Pen(GraphNode node) {
			if(node.PressedInfo.HitTest == GraphNodeHitTest.Point2) return DrawInfo.PressedPen;
			if(node.HotInfo.HitTest == GraphNodeHitTest.Point2) return DrawInfo.HotTrackPen;
			if(IsNodeP2Selected(node)) return DrawInfo.SelectedPointPen;
			return DrawInfo.NormalPen;
		}
		protected virtual Pen GetPoint3Pen(GraphNode node) {
			if(node.PressedInfo.HitTest == GraphNodeHitTest.Point3) return DrawInfo.PressedPen;
			if(node.HotInfo.HitTest == GraphNodeHitTest.Point3) return DrawInfo.HotTrackPen;
			if(IsNodeP3Selected(node)) return DrawInfo.SelectedPointPen;
			return DrawInfo.NormalPen;
		}
		protected virtual Pen GetPoint4Pen(GraphNode node) {
			if(node.PressedInfo.HitTest == GraphNodeHitTest.Point4) return DrawInfo.PressedPen;
			if(node.HotInfo.HitTest == GraphNodeHitTest.Point4) return DrawInfo.HotTrackPen;
			if(IsNodeP4Selected(node)) return DrawInfo.SelectedPointPen;
			return DrawInfo.NormalPen;
		}
		protected virtual Pen GetSplineWeightPen3(GraphNode node) {
			if(node.PressedInfo.HitTest == GraphNodeHitTest.Point3) return DrawInfo.PressedPen;
			if(node.HotInfo.HitTest == GraphNodeHitTest.Point3) return DrawInfo.HotTrackPen;
			if(IsNodeP3Selected(node)) return DrawInfo.SelectedSplineWeightPen;
			return DrawInfo.SplineWeightPen;
		}
		protected virtual Pen GetSplineWeightPen4(GraphNode node) {
			if(node.PressedInfo.HitTest == GraphNodeHitTest.Point4) return DrawInfo.PressedPen;
			if(node.HotInfo.HitTest == GraphNodeHitTest.Point4) return DrawInfo.HotTrackPen;
			if(IsNodeP4Selected(node)) return DrawInfo.SelectedSplineWeightPen;
			return DrawInfo.SplineWeightPen;
		}
		public IGraphEditor GraphControl { get { return Graph.GraphControl; } }
		protected virtual void DrawPoint(DrawingContext c, Point pt, Pen pen) {
			c.DrawRectangle(null, pen, new Rect(Math.Floor(pt.X - GraphControl.NodePointSize / 2) + 0.5, Math.Floor(pt.Y - GraphControl.NodePointSize / 2) + 0.5, GraphControl.NodePointSize, GraphControl.NodePointSize));
		}
		protected virtual void DrawNodePoints(DrawingContext c, GraphNode node) {
			Point pt1 = GraphControl.Point2ViewPort(node.P1), pt2 = GraphControl.Point2ViewPort(node.P2);
			UpdateScrPoints(node, ref pt1, ref pt2);
			if(node.IsFirstNode || node.IsFirstNodeAfterBroken)
				DrawPoint(c, pt1, GetPoint1Pen(node));
			DrawPoint(c, pt2, GetPoint2Pen(node));
		}
		protected virtual void DrawConstNode(DrawingContext c, GraphNode node) {
			Point scrPoint1 = GraphControl.Point2ViewPort(node.P1);
			Point scrPoint2 = GraphControl.Point2ViewPort(new Point(node.P2.X, node.P1.Y));
			if(!Contains(scrPoint1, scrPoint2)) return;
			c.DrawLine(GetLinePen(node), scrPoint1, scrPoint2);
		}
		protected virtual void UpdateScrPoints(GraphNode node, ref Point p1, ref Point p2) {
			if(node.Type == GraphNodeType.Const) p2 = GraphControl.Point2ViewPort(new Point(node.P2.X, node.P1.Y));
		}
		protected virtual void DrawBoolNode(DrawingContext c, GraphNode node) {
			Point scrPoint1 = GraphControl.Point2ViewPort(node.P1);
			Point scrPoint2 = GraphControl.Point2ViewPort(node.P2);
			UpdateScrPoints(node, ref scrPoint1, ref scrPoint2);
			c.DrawLine(GetLinePen(node), scrPoint1, scrPoint2);
		}
		protected virtual void DrawLineNode(DrawingContext c, GraphNode node) {
			Point scrPoint1 = GraphControl.Point2ViewPort(node.P1);
			Point scrPoint2 = GraphControl.Point2ViewPort(node.P2);
			if(!Contains(scrPoint1, scrPoint2)) return;
			c.DrawLine(GetLinePen(node), scrPoint1, scrPoint2);
		}
		protected virtual void DrawSplineNode(DrawingContext c, GraphNode node) {
			Point scrPoint1 = GraphControl.Point2ViewPort(node.P1);
			Point scrPoint2 = GraphControl.Point2ViewPort(node.P2);
			if(!Contains(scrPoint1, scrPoint2)) return;
			Point pt3 = GraphControl.Point2ViewPort(node.P3), pt4 = GraphControl.Point2ViewPort(node.P4);
			if(Graph.ShowAuxElements) {
				DrawPoint(c, pt3, GetPoint3Pen(node));
				DrawPoint(c, pt4, GetPoint4Pen(node));
			}
			Pen pen = GetLinePen(node);
			Point delta = new Point(GraphControl.GetXLen(5), GraphControl.GetYLen(5));
			Point prevPoint = scrPoint1, newPoint = new Point(0,0), scrPoint = new Point(0,0);
			for(newPoint.X = node.P1.X; newPoint.X <= node.P2.X; newPoint.X += delta.X) {
				newPoint.Y = node.Calculate(newPoint.X);
				scrPoint = GraphControl.Point2ViewPort(newPoint);
				c.DrawLine(pen, prevPoint, scrPoint);
				prevPoint = scrPoint;
			}
			newPoint.Y = node.Calculate(node.P2.X);
			scrPoint = GraphControl.Point2ViewPort(newPoint);
			c.DrawLine(pen, prevPoint, scrPoint);
			if(Graph.ShowAuxElements) {
				c.DrawLine(GetSplineWeightPen3(node), scrPoint1, pt3);
				c.DrawLine(GetSplineWeightPen4(node), scrPoint2, pt4);
			}
		}
		protected virtual void DrawNode(DrawingContext c, GraphNode node) {
			double scrPoint1 = GraphControl.X2ViewPort(node.P1.X);
			double scrPoint2 = GraphControl.X2ViewPort(node.P2.X);
			if(scrPoint2 < 0.0f || scrPoint1 > GraphControl.ClientRect.Width) return;
			if(Graph.ShowAuxElements)
				DrawNodePoints(c, node);
			switch(node.Type) {
				case GraphNodeType.Const:
					DrawConstNode(c, node);
					break;
				case GraphNodeType.Line:
					DrawLineNode(c, node);
					break;
				case GraphNodeType.Spline:
					DrawSplineNode(c, node);
					break;
			}
		}
		public virtual void Draw(DrawingContext c) {
			if(!Graph.Visible) return;
			foreach(GraphNode node in Graph.Nodes) {
				DrawNode(c, node);
			}
		}
	}
	public class Graph {
		GraphCollection ownerColl;
		GraphNodeCollection nodes;
		GraphPainter painter;
		Color color;
		bool visible;
		DashStyle style;
		float width;
		bool showAuxElements;
		public Graph() {
			this.color = Colors.Black;
			this.visible = true;
			this.width = 1.0f;
			this.style = DashStyles.Solid;
			this.showAuxElements = true;
		}
		protected virtual GraphNodeCollection CreateNodes() {
			return new GraphNodeCollection(this);
		}
		protected virtual GraphPainter CreatePainter() {
			return new GraphPainter(this);
		}
		public GraphNodeCollection Nodes { 
			get {
				if(nodes == null) 
					nodes = CreateNodes();
				return nodes; 
			}
		}
		public GraphPainter Painter {
			get {
				if(painter == null) 
					painter = CreatePainter();
				return painter;
			}
		}
		public Color Color { 
			get { return color; }
			set {
				if(Color == value) return;
				color = value;
				OnVisualPropertiesChanged();
			}
		}
		public bool Visible {
			get { return visible; }
			set {
				if(Visible == value) return;
				visible = value;
				OnPropertiesChanged();
			}
		}
		public bool ShowAuxElements {
			get { return showAuxElements; }
			set {
				if(ShowAuxElements == value) return;
				showAuxElements = value;
				OnPropertiesChanged();
			}
		}
		public float Width {
			get { return width; }
			set {
				if(Width == value) return;
				width = value;
				OnPropertiesChanged();
			}
		}
		public DashStyle Style {
			get { return style; }
			set {
				if(Style == value) return;
				style = value;
				OnPropertiesChanged();
			}
		}
		public IGraphEditor GraphControl { get { return OwnerColl == null? null: OwnerColl.GraphControl; } }
		public GraphCollection OwnerColl { get { return ownerColl; } }
		internal void SetColl(GraphCollection coll) { this.ownerColl = coll; }
		protected double GetP1MinX(int nodeIndex) {
			return nodeIndex > 0 ? Nodes[nodeIndex - 1].P1.X + 0.00001 : double.MinValue;
		}
		protected double GetP1MaxX(int nodeIndex) {
			return Nodes[nodeIndex].P2.X - 0.000001;
		}
		protected double GetP2MinX(int nodeIndex) {
			return Nodes[nodeIndex].P1.X + 0.000001;
		}
		protected double GetP2MaxX(int nodeIndex) {
			return nodeIndex < Nodes.Count - 1 ? Nodes[nodeIndex + 1].P2.X - 0.000001 : double.MaxValue;
		}
		GraphNode selectedNode;
		public GraphNode SelectedNode {
			get { return selectedNode; }
			set {
				if(SelectedNode == value) return;
				selectedNode = value;
				OnSelectedNodeChanged();
				OnPropertiesChanged();
			}
		}
		protected virtual void OnSelectedNodeChanged() {
			if(GraphControl == null || GraphControl.SelectedGraph != this) return;
			GraphControl.OnSelectedNodeChanged();
		}
		public void RemoveSelNode() {
			if(SelectedNode == null) return;
			int index = Nodes.IndexOf(SelectedNode);
			Nodes.Remove(SelectedNode);
			selectedNode = null;
			if(index >= Nodes.Count) {
				if(Nodes.Count > 0)
					SelectedNode = Nodes[Nodes.Count - 1] as GraphNode;
				SelectedNode.SelectedInfo.HitTest = GraphNodeHitTest.Line;
				OnPropertiesChanged();
				return;
			}
			if(index < Nodes.Count) {
				SelectedNode = Nodes[index] as GraphNode;
				SelectedNode.SelectedInfo.HitTest = GraphNodeHitTest.Line;
			}
			if(Nodes.Count < 2 || index == 0) {
				OnPropertiesChanged();
				return;
			}
			if(Nodes[index - 1].SplitNext)
				Nodes[index].P1 = Nodes[index - 1].P2;
			else
				Nodes[index].P1 = new Point(Nodes[index - 1].P2.X, Nodes[index].P1.Y);
			OnPropertiesChanged();
		}
		public void SelectedNodeSplitNext(bool value) {
			if(SelectedNode == null) return;
			SelectedNode.SplitNext = value;
			if(value == false) return;
			int index = Nodes.IndexOf(SelectedNode);
			if(index == Nodes.Count - 1) return;
			GraphNode nn = Nodes[index + 1] as GraphNode;
			MoveSelPoint2(new Point(0, nn.P1.Y - SelectedNode.P2.Y));
		}
		public void MoveSelPoint1(Point delta) {
			if(SelectedNode == null) return;
			int index = Nodes.IndexOf(SelectedNode);
			SelectedNode.MovePoint1(delta, GetP1MinX(index), GetP1MaxX(index));
			if(index == 0) return;
			GraphNode pn = Nodes[index - 1] as GraphNode;
			if(pn.SplitNext)
				pn.MovePoint2(new Point(SelectedNode.P1.X - pn.P2.X, SelectedNode.P1.Y - pn.P2.Y));
			else
				pn.MovePoint2(new Point(SelectedNode.P1.X - pn.P2.X, 0));
		}
		public void MoveSelPoint2(Point delta) {
			if(SelectedNode == null) return;
			int index = Nodes.IndexOf(SelectedNode);
			SelectedNode.MovePoint2(delta, GetP2MinX(index), GetP2MaxX(index));
			if(index == Nodes.Count - 1) return;
			GraphNode nn = Nodes[index + 1] as GraphNode;
			if(SelectedNode.SplitNext)
				nn.MovePoint1(new Point(SelectedNode.P2.X - nn.P1.X, SelectedNode.P2.Y - nn.P1.Y));
			else
				nn.MovePoint1(new Point(SelectedNode.P2.X - nn.P1.X, 0));
		}
		public void MoveSelPoint3(Point delta) {
			if(SelectedNode == null) return;
			int index = Nodes.IndexOf(SelectedNode);
			SelectedNode.MovePoint3(delta, double.MinValue, double.MaxValue);
		}
		public void MoveSelPoint4(Point delta) {
			if(SelectedNode == null) return;
			int index = Nodes.IndexOf(SelectedNode);
			SelectedNode.MovePoint4(delta, double.MinValue, double.MaxValue);
		}
		public void SelectNode(Point pt) {
			int index = FindNodeByPoint(pt);
			if(index == -1)
				SelectedNode = null;
			else SelectedNode = Nodes[index] as GraphNode;
		}
		protected virtual int FindNodeByPoint(Point pt) {
			for(int i = 0; i < Nodes.Count; i++) {
				if(Nodes[i].P1.X <= pt.X && pt.X <= Nodes[i].P2.X) {
					double y = Nodes[i].Calculate(pt.X);
					if(Math.Abs(y - pt.Y) < 5) return i;
					return -1;
				}
			}
			return -1;
		}
		protected virtual int FindNodeIndexByX(double x) {
			if(Nodes.Count == 0) return 0;
			if(Nodes[0].P1.X > x) return -1;
			for(int i = 0; i < Nodes.Count; i++) {
				if(Nodes[i].P1.X <= x && x <= Nodes[i].P2.X) return i;
			}
			return Nodes.Count;
		}
		public void InsertNode(Point pt, GraphNodeType nodeType) {
			int index = FindNodeIndexByX(pt.X);
			GraphNode node = new GraphNode();
			node.Type = nodeType;
			if(index >= 0 && index < Nodes.Count) {
				node.P1 = Nodes[index].P1;
				node.P2 = pt;
				Nodes[index].P1 = pt;
			}
			else if(index == Nodes.Count) {
				node.P2 = pt;
				if(Nodes.Count > 0)
					node.P1 = Nodes[Nodes.Count - 1].P2;
				else
					node.P1 = new Point(0, 0);
			}
			else if(index < 0) {
				node.P1 = pt;
				node.P2 = Nodes[0].P1;
			}
			Nodes.Insert(index, node);
			InitializeSplineWeight(index);
		}
		protected virtual void InitializeSplineWeight(int index) {
			if(Nodes[index].Type == GraphNodeType.Spline) {
				Point delta = new Point(0,0);
				if(index == 0 || !Nodes[index - 1].SplitNext) {
					delta = new Point((Nodes[index].P2.X - Nodes[index].P1.X) / 3, (Nodes[index].P2.Y - Nodes[index].P1.Y) / 3);
					Nodes[index].P3 = new Point(Nodes[index].P1.X + delta.X, Nodes[index].P1.X + delta.Y);
				}
				else {
					delta = new Point(Nodes[index - 1].P2.X - Nodes[index - 1].P4.X, Nodes[index - 1].P2.Y - Nodes[index - 1].P4.Y);
					Nodes[index].P3 = new Point(Nodes[index].P1.X + delta.X, Nodes[index].P1.Y + delta.Y);
				}
				if(index == Nodes.Count - 1) {
					delta = new Point((Nodes[index].P2.X - Nodes[index].P1.X) / 3, (Nodes[index].P2.Y - Nodes[index].P1.Y) / 3);
					Nodes[index].P4 = new Point(Nodes[index].P2.X - delta.X, Nodes[index].P2.Y - delta.Y);
				}
				else {
					delta = new Point(Nodes[index + 1].P2.X - Nodes[index + 1].P4.X, Nodes[index + 1].P2.Y - Nodes[index + 1].P4.Y);
					Nodes[index].P4 = new Point(Nodes[index].P2.X + delta.X, Nodes[index].P2.Y + delta.Y); ;
				}
			}
		}
		protected virtual void ClearNodes(int startIndex) {
			for(int i = startIndex; i < Nodes.Count; i++) {
				Nodes[i].SelectedInfo = null;
			}
		}
		public void ProcessHitTest(MouseButton button, MouseButtonState state, Point pt) {
			foreach(GraphNode node in Nodes) {
				node.ProcessHitTest(button, state, pt);
				if(button == MouseButton.Left && node.SelectedInfo.HitTest != GraphNodeHitTest.None) {
					SelectedNode = node;
					ClearNodes(Nodes.IndexOf(node) + 1);
					return;
				}
			}
			if(button == MouseButton.Left) {
				SelectedNode = null;
				ClearNodes(0);
			}
		}
		protected virtual void OnVisualPropertiesChanged() {
			Painter.DrawInfo = null;
			OnPropertiesChanged();
		}
		protected internal virtual void OnPropertiesChanged() {
			if(GraphControl == null) return;
			GraphControl.UpdateView();
		}
		public int FindNodeIndex(double t) {
			if (Nodes.Count == 0) return -1;
			if (t < Nodes[0].P1.X) return 0;
			if (t > Nodes[Nodes.Count - 1].P2.X) return Nodes.Count - 1;
			for (int i = 0; i < Nodes.Count; i++) {
				if (Nodes[i].P1.X <= t && Nodes[i].P2.X >= t) return i;
			}
			return -1;
		}
		public double GetValue(double t) {
			int nodeIndex = FindNodeIndex(t);
			if (nodeIndex == -1) return 0.0;
			if (t < Nodes[0].P1.X) return Nodes[0].P1.Y;
			if (t > Nodes[Nodes.Count - 1].P2.X) return Nodes[Nodes.Count - 1].P2.Y;
			return Nodes[nodeIndex].Calculate(t);
		}
	}
}
