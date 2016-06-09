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

#if customization
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Utils;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Windows.Input;
namespace DevExpress.Xpf.Carousel {
	public abstract class ScribblePanelState {
		protected ScribblePanel owner;
		public ScribblePanelState(ScribblePanel owner) { this.owner = owner; }
		public abstract ScribblePanelState ProcessMessage(RoutedEventArgs e);
	}
	public class ScribblePanelNormalState : ScribblePanelState {
		public ScribblePanelNormalState(ScribblePanel owner) : base(owner) { }
		public override ScribblePanelState ProcessMessage(RoutedEventArgs e) {
			MouseEventArgs mea = e as MouseEventArgs;
			if (e.RoutedEvent == Panel.MouseDownEvent) return new ScribblePanelSetStartPointState(owner, mea.GetPosition(owner));
			else return this;
		}
	}
	public class ScribblePanelSetStartPointState : ScribblePanelState {
		protected Point startPoint;
		public ScribblePanelSetStartPointState(ScribblePanel owner, Point p) : base(owner) { startPoint = p; }
		protected double CalcDistance(Point p1, Point p2) { return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y)); }
		protected double allowedPointsDistance = 10;
		public override ScribblePanelState ProcessMessage(RoutedEventArgs e) {
			MouseEventArgs mea = e as MouseEventArgs;
			if (e.RoutedEvent == Panel.MouseMoveEvent) {
				Point currentPos = mea.GetPosition(owner);
				if (CalcDistance(startPoint, currentPos) > allowedPointsDistance) return new ScribblePanelDrawingState(owner, currentPos);
				else
					return this;
			} else return new ScribblePanelNormalState(owner);
		}
	}
	public class ScribblePanelDrawingState : ScribblePanelSetStartPointState {
		List<Point> points = new List<Point>();
		public ScribblePanelDrawingState(ScribblePanel owner, Point startPoint)
			: base(owner, startPoint) {
			points.Add(startPoint);
		}
		Point endPoint;
		protected Point LastAddedPoint { get { return points[points.Count - 1]; } }
		protected bool CanAddPoint(Point p) {
			return CalcDistance(LastAddedPoint, p) >= allowedPointsDistance;
		}
		protected void UpdateOwnerPath() {
			PolyBezierSegment segment = new PolyBezierSegment(points, true);
			PathFigure pf = new PathFigure(startPoint, new PathSegment[] { segment }, false);
			Path path = new Path();
			path.Data = new PathGeometry(new PathFigure[] { pf });
			path.Stroke = Brushes.Red;
			path.StrokeThickness = 5;
			owner.ScribblePath = path;
		}
		public override ScribblePanelState ProcessMessage(RoutedEventArgs e) {
			MouseEventArgs mea = e as MouseEventArgs;
			if (e.RoutedEvent == Panel.MouseMoveEvent) {
				endPoint = mea.GetPosition(owner);
				if (CanAddPoint(endPoint)) {
					points.Add(endPoint);
					UpdateOwnerPath();
				}
				return this;
			} else {
				UpdateOwnerPath();
				return new ScribblePanelNormalState(owner);
			}
		}
	}
	public class ScribblePanel : Panel {
		static ScribblePanel() { 
			ScribblePathProperty = DependencyProperty.Register("ScribblePath", typeof(Path), typeof(ScribblePanel), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
		}
		public static readonly DependencyProperty ScribblePathProperty;
		ScribblePanelState state;
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			state = new ScribblePanelNormalState(this);
			AddHandler(MouseDownEvent, new RoutedEventHandler(ScribblePanelProcessEvent));
			AddHandler(MouseMoveEvent, new RoutedEventHandler(ScribblePanelProcessEvent));
			AddHandler(MouseUpEvent, new RoutedEventHandler(ScribblePanelProcessEvent));
			AddHandler(MouseEnterEvent, new RoutedEventHandler(ScribblePanelProcessEvent));
			AddHandler(MouseLeaveEvent, new RoutedEventHandler(ScribblePanelProcessEvent));
		}
		void ScribblePanelProcessEvent(object sender, RoutedEventArgs e) {
			state = state.ProcessMessage(e);
		}
		public Path ScribblePath {
			get { return (Path)GetValue(ScribblePathProperty); }
			set { SetValue(ScribblePathProperty, value); }
		}
		protected override void OnRender(DrawingContext dc) {
			base.OnRender(dc);
			if (ScribblePath != null) dc.DrawGeometry(Brushes.Transparent, new Pen(Brushes.Red, 3), ScribblePath.Data);
		}
	}
}
#endif
