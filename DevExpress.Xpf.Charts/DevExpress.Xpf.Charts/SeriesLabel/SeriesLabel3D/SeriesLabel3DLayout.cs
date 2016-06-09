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

using System.Windows.Media;
using System.Windows.Media.Media3D;
namespace DevExpress.Xpf.Charts.Native {
	public abstract class SeriesLabel3DLayout {
		readonly SeriesLabel label;
		Transform3D transform;
		public SeriesLabel Label { get { return label; } }
		public SeriesLabel3DLayout(SeriesLabel label) {
			this.label = label;
		}
		Point3D CalcConnectorEndPoint(Matrix3D scaleMatrix, Point3D startPoint, Vector3D direction) {
			double length = label.Indent == 0 ? 0 : scaleMatrix.Transform(new Point3D(0, label.Indent, 0)).Y;
			direction.Normalize();
			Vector3D actualDirection = Vector3D.Multiply(direction, length);
			Transform3D transform = new TranslateTransform3D((Vector3D)startPoint);
			return transform.Transform((Point3D)actualDirection);
		}
		public void SetTransform(Transform3D transform) {
			this.transform = transform;
		}
		protected Point3D TransformPoint(Point3D point) {
			return transform == null ? point : transform.Transform(point);
		}
		protected Vector3D TransformVector(Vector3D vector) {
			return transform == null ? vector : transform.Transform(vector);
		}
		protected abstract Point3D GetLabelPoint3D(Diagram3DDomain domain);
		protected abstract Vector3D GetLabelDirection3D(Diagram3DDomain domain);
		protected virtual Point3D GetConnectorStartPoint(Diagram3DDomain domain) {
			return GetLabelPoint3D(domain);
		}
		public virtual LabelModelContainer CreateModel(Diagram3DDomain domain, Series3DPointGeometry pointGeometry, Color pointColor, Model3DGroup modelHolder) {
			Point3D point = GetLabelPoint3D(domain);
			Vector3D direction = GetLabelDirection3D(domain);
			Matrix3D scaleMatrix = domain.CreateScaleTransform(point).Value;
			Point3D connectionPoint = CalcConnectorEndPoint(scaleMatrix, point, direction);
			if (label.ConnectorVisible) {
				Model3D connectorModel = Graphics3DUtils.CreateLineModel(GetConnectorStartPoint(domain), connectionPoint, 
					scaleMatrix.Transform(new Point3D(label.ConnectorThickness, 0, 0)).X, new DiffuseMaterial(new SolidColorBrush(pointColor)));
				if (connectorModel != null)
					modelHolder.Children.Add(connectorModel);
			}
			return Label3DHelper.CreateLabelModelContainer(domain, pointGeometry.LabelContentPresenter, pointGeometry.LabelModel, connectionPoint, direction, label.RenderMode);
		}
	}
}
