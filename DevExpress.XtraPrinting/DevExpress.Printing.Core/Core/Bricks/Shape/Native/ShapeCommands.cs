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
using System.Drawing;
namespace DevExpress.XtraPrinting.Shape.Native {
	#region ShapeCommands
	public abstract class ShapeCommandBase {
		public abstract void Accept(IShapeCommandVisitor visitor);
	}
	public abstract class ShapePointsCommand : ShapeCommandBase {
		PointF[] points;
		public PointF[] Points {
			get { return points; }
		}
		protected ShapePointsCommand(int pointCount) {
			points = new PointF[pointCount];
		}
	}
	public class ShapeLineCommand : ShapePointsCommand {
		const int startPointIndex = 0;
		const int endPointIndex = 1;
		public PointF StartPoint {
			get { return Points[startPointIndex]; }
			set { Points[startPointIndex] = value; }
		}
		public PointF EndPoint {
			get { return Points[endPointIndex]; }
			set { Points[endPointIndex] = value; }
		}
		public float Length {
			get {
				return (float)Math.Sqrt(
					(StartPoint.X - EndPoint.X) * (StartPoint.X - EndPoint.X) +
					(StartPoint.Y - EndPoint.Y) * (StartPoint.Y - EndPoint.Y)
					);
			}
		}
		public ShapeLineCommand(PointF startPoint, PointF endPoint)
			: this(startPoint, endPoint, 2) {
		}
		protected ShapeLineCommand(PointF startPoint, PointF endPoint, int pointCount)
			: base(pointCount) {
			StartPoint = startPoint;
			EndPoint = endPoint;
		}
		public override void Accept(IShapeCommandVisitor visitor) {
			visitor.VisitShapeLineCommand(this);
		}
	}
	public class ShapeBezierCommand : ShapeLineCommand {
		const int startControlPointIndex = 2;
		const int endControlPointIndex = 3;
		public PointF StartControlPoint {
			get { return Points[startControlPointIndex]; }
			set { Points[startControlPointIndex] = value; }
		}
		public PointF EndControlPoint {
			get { return Points[endControlPointIndex]; }
			set { Points[endControlPointIndex] = value; }
		}
		public ShapeBezierCommand(PointF startPoint, PointF startControlPoint, PointF endControlPoint, PointF endPoint)
			: base(startPoint, endPoint, 4) {
			StartControlPoint = startControlPoint;
			EndControlPoint = endControlPoint;
		}
		public override void Accept(IShapeCommandVisitor visitor) {
			visitor.VisitShapeBezierCommand(this);
		}
	}
	public abstract class ShapePathCommand : ShapeCommandBase {
		ShapePointsCommandCollection commands;
		public ShapePointsCommandCollection Commands {
			get { return commands; }
		}
		public bool IsClosed {
			get {
				if(commands.Count == 0)
					return false;
				ShapeLineCommand startCommand = (ShapeLineCommand)commands[0];
				ShapeLineCommand endCommand = (ShapeLineCommand)commands[commands.Count - 1];
				return startCommand.StartPoint == endCommand.EndPoint;
			}
		}
		protected ShapePathCommand(ShapePointsCommandCollection commands) {
			this.commands = commands;
		}
	}
	public class ShapeDrawPathCommand : ShapePathCommand {
		public ShapeDrawPathCommand(ShapePointsCommandCollection commands)
			: base(commands) {
		}
		public override void Accept(IShapeCommandVisitor visitor) {
			visitor.VisitShapeDrawPathCommand(this);
		}
	}
	public class ShapeFillPathCommand : ShapePathCommand {
		public ShapeFillPathCommand(ShapePointsCommandCollection commands)
			: base(commands) {
		}
		public override void Accept(IShapeCommandVisitor visitor) {
			visitor.VisitShapeFillPathCommand(this);
		}
	}
	#endregion
}
