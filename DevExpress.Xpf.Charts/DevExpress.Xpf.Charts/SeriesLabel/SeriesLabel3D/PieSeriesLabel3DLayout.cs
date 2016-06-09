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

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using DevExpress.Charts.Native;
using System;
namespace DevExpress.Xpf.Charts.Native {
	public class Pie3DLabelLayout : SeriesLabel3DLayout, IPieLabelLayout {
		readonly Point3D maxTop, maxBottom, midTop, midBottom;
		readonly Vector3D direction;
		GRect2D labelBounds;
		ContentPresenter contentPresenter;
		double angle;
		public PieLabelPosition Position { get { return PieSeries.GetLabelPosition(Label); } }
		public Point3D MaxTop { get { return maxTop; } }
		public Point3D MidTop { get { return midTop; } }
		public Point3D MaxBottom { get { return maxBottom; } }
		public Point3D MidBottom { get { return midBottom; } }
		public double LabelIndent { get { return Label.Indent; } }
		public ContentPresenter ContentPresenter { get { return contentPresenter; } set { contentPresenter = value; } }
		public Color LabelConectorColor { get; set; }
		#region IPieLabelLayout Members
		public double Angle {
			get {
				return angle;  
			} 
		}
		public bool ResolveOverlapping {
			get { return ((PieSeries3D)Label.Series).LabelsResolveOverlapping; }
		}
		public GRect2D LabelBounds {
			get {
				return labelBounds;
			}
			set {
				labelBounds = value;
			}
		}
		public bool Visible {
			get {
				return Label.Series != null ? Label.Series.LabelsVisibility : false;
			}
			set {
				if (Label.Series != null)
					Label.Series.LabelsVisibility = value;
			}
		}
		#endregion
		public Pie3DLabelLayout(SeriesLabel label, Point3D maxTop, Point3D maxBottom, Point3D midTop, Point3D midBottom, Vector3D direction)
			: base(label) {
			this.maxTop = maxTop;
			this.maxBottom = maxBottom;
			this.midTop = midTop;
			this.midBottom = midBottom;
			this.direction = direction;
		}
		public override LabelModelContainer CreateModel(Diagram3DDomain domain, Series3DPointGeometry pointGeometry, Color pointColor, Model3DGroup modelHolder) {
			return Position == PieLabelPosition.Outside ? base.CreateModel(domain, pointGeometry, pointColor, modelHolder) : null;
		}
		public Point3D GetDockPoint(SimpleDiagram3DDomain domain) {
			return TransformPoint(domain.IsTopVisible ? MidTop : MidBottom);
		}
		public Point GetLabelPoint2D(SimpleDiagram3DDomain domain) {
			return domain.Project(GetLabelPoint3D(domain));
		}
		public Point CalculateLabelConnectPoint(SimpleDiagram3DDomain domain) {
			double x = GetDirection2D(domain).X < 0 ? LabelBounds.Right : LabelBounds.Left;
			return new Point(x, LabelBounds.Top + ContentPresenter.DesiredSize.Height / 2.0);
		}
		public void CompleteLayout(SimpleDiagram3DDomain domain) {
			Vector direction2D = GetDirection2D(domain);
			angle = Math.Atan2(direction2D.Y, direction2D.X);
		}
		public Vector GetDirection2D(SimpleDiagram3DDomain domain) {
			Point3D dockPoint3D = GetDockPoint(domain);
			Point3D endPoint = new Point3D(dockPoint3D.X + direction.X, dockPoint3D.Y + direction.Y, dockPoint3D.Z + direction.Z);
			Point endPoint2D = domain.Project(endPoint);
			Point dockPoint2D = domain.Project(dockPoint3D);
			Vector result = new Vector(endPoint2D.X - dockPoint2D.X, endPoint2D.Y - dockPoint2D.Y);
			result.Normalize();
			return result;
		}
		protected override Point3D GetLabelPoint3D(Diagram3DDomain domain) {
			return TransformPoint(((SimpleDiagram3DDomain)domain).IsTopVisible ? maxTop : maxBottom);
		}
		protected override Vector3D GetLabelDirection3D(Diagram3DDomain domain) {
			return TransformVector(direction);
		}
	}
}
