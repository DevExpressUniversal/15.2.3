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
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using DevExpress.Charts.Native;
namespace DevExpress.Xpf.Charts.Native {
	public class Marker3DLabelLayout : XYSeriesLabel3DLayout {
		readonly Point3D boundsStartPoint;
		readonly Point3D boundsEndPoint;
		readonly Point3D connectorPoint;
		public Marker3DLabelLayout(SeriesLabel label, Point3D point, Point3D connectorPoint, Vector3D direction, Model3D model) : base(label, point, direction) {
			boundsStartPoint = model.Bounds.Location;
			boundsEndPoint = new Point3D(model.Bounds.Location.X + model.Bounds.SizeX,
				model.Bounds.Location.Y + model.Bounds.SizeY, model.Bounds.Location.Z + model.Bounds.SizeZ);
			this.connectorPoint = connectorPoint;
		}
		public double GetOffset(Transform3D transform) {
			Point3D startPoint = transform.Transform(boundsStartPoint);
			Point3D endPoint = transform.Transform(boundsEndPoint);
			return MathUtils.CalcDistance(startPoint, endPoint) / 2;
		}
		protected override Point3D GetConnectorStartPoint(Diagram3DDomain domain) {
			return TransformPoint(connectorPoint);
		}
		protected override Vector3D GetLabelDirection3D(Diagram3DDomain domain) {
			switch (PointSeries3D.GetLabelPosition(Label)) {
				case Marker3DLabelPosition.Top:
					return base.GetLabelDirection3D(domain);
				case Marker3DLabelPosition.Center:
					Point3D point = GetLabelPoint3D(domain);
					Point3D cameraPosition = domain.RelativeCameraPosition;
					Vector3D direction = new Vector3D(cameraPosition.X - point.X, cameraPosition.Y - point.Y, cameraPosition.Z - point.Z);
					MathUtils.Normalize(ref direction);
					return direction;
				default:
					ChartDebug.Fail("Unknown Marker3DLabelPosition value.");
					goto case Marker3DLabelPosition.Top;
			}
		}
		public override LabelModelContainer CreateModel(Diagram3DDomain domain, Series3DPointGeometry pointGeometry, Color pointColor, Model3DGroup modelHolder) {
			if (PointSeries3D.GetLabelPosition(Label) == Marker3DLabelPosition.Top)
				return base.CreateModel(domain, pointGeometry, pointColor, modelHolder);
			Point3D point = GetLabelPoint3D(domain);
			Vector3D direction = GetLabelDirection3D(domain);
			double offsetDistance = GetOffset(domain.ModelTransform);
			point.Offset(direction.X * offsetDistance, direction.Y * offsetDistance, direction.Z * offsetDistance);
			return Label3DHelper.CreateLabelModelContainer(domain, pointGeometry.LabelContentPresenter, pointGeometry.LabelModel, point, new Vector3D(), Label.RenderMode);
		}
	}
}
