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
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
namespace DevExpress.Xpf.Charts.Native {
	public class AreaSeries3DLabelLayout : XYSeriesLabel3DLayout {
		readonly Point3D frontPoint;
		readonly Point3D backPoint;
		public AreaSeries3DLabelLayout(SeriesLabel label, Point3D point, Vector3D direction, double zOffset) : base(label, point, direction) {
			this.frontPoint = new Point3D(point.X, point.Y, point.Z + zOffset);
			this.backPoint = new Point3D(point.X, point.Y, point.Z - zOffset);
		}
		public Point3D GetFrontPoint() {
			return TransformPoint(frontPoint);
		}
		public Point3D GetBackPoint() {
			return TransformPoint(backPoint);
		}
		bool IsFrontPoint(Point3D cameraPosition) {
			Point3D frontPoint = GetFrontPoint();
			Point3D backPoint = GetBackPoint();
			double distanceToFrontPoint = MathUtils.CalcDistance(frontPoint, cameraPosition);
			double distanceToBackPoint = MathUtils.CalcDistance(backPoint, cameraPosition);
			return distanceToFrontPoint < distanceToBackPoint;
		}
		protected override Point3D GetLabelPoint3D(Diagram3DDomain domain) {
			if (IsFrontPoint(domain.RelativeCameraPosition))
				return GetFrontPoint();
			else
				return GetBackPoint();
		}
		protected override Vector3D GetLabelDirection3D(Diagram3DDomain domain) {
			if (IsFrontPoint(((Diagram3DDomain)domain).RelativeCameraPosition))
				return new Vector3D(0, 0, 1);
			else
				return new Vector3D(0, 0, -1);
		}
		public override LabelModelContainer CreateModel(Diagram3DDomain domain, Series3DPointGeometry pointGeometry, Color pointColor, Model3DGroup modelHolder) {
			Point3D point = GetLabelPoint3D(domain);
			Vector3D direction = GetLabelDirection3D(domain);
			ScaleTransform3D transform = domain.CreateScaleTransform(point);
			ContentPresenter presenter = pointGeometry.LabelContentPresenter;
			double offset = Math.Sqrt((presenter.DesiredSize.Width * transform.ScaleX) * (presenter.DesiredSize.Width * transform.ScaleX) + 
				(presenter.DesiredSize.Height * transform.ScaleY) * (presenter.DesiredSize.Height * transform.ScaleY)) / 2;
			point.Offset(direction.X * offset, direction.Y * offset, direction.Z * offset);
			return Label3DHelper.CreateLabelModelContainer(domain, presenter, pointGeometry.LabelModel, point, new Vector3D(), Label.RenderMode);
		}
	}
}
