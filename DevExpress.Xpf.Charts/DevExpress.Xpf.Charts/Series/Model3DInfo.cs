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
using System.Windows.Media.Media3D;
namespace DevExpress.Xpf.Charts.Native {
	public abstract class Model3DInfo {
		Model3D model;
		Transform3D originalTrasform;
		SeriesPoint point;
		SeriesLabel3DLayout labelLayout;
		protected SeriesPoint Point { get { return point; } }
		public Model3D Model { get { return model; } }
		public SeriesLabel3DLayout LabelLayout { get { return labelLayout; } }
		public Model3DInfo(SeriesPoint point, Model3D model, SeriesLabel3DLayout labelLayout) {
			this.model = model;
			originalTrasform = model == null ? null : model.Transform;
			this.point = point;
			this.labelLayout = labelLayout;
		}
		public void UpdateTransform(Transform3D commonTransform) {
			if (model == null)
				return;
			Transform3D transform = GetAdditionalTransform();
			if (transform == null && commonTransform == null)
				return;
			Transform3DGroup group = new Transform3DGroup();
			if (originalTrasform != null)
				group.Children.Add(originalTrasform);
			if (transform != null)
				group.Children.Add(transform);
			if (commonTransform != null)
				group.Children.Add(commonTransform);
			model.Transform = group;
			if (labelLayout != null)
				labelLayout.SetTransform(group);
		}
		protected abstract Transform3D GetAdditionalTransform();
	}
	public class XYSeriesModel3DInfo : Model3DInfo {
		Vector3D offset;
		Size3D size;
		public XYSeriesModel3DInfo(SeriesPoint point, Model3D model, XYSeriesLabel3DLayout labelLayout) : base(point, model, labelLayout) {
			if (model == null || model.Bounds.IsEmpty) {
				offset = new Vector3D();
				size = new Size3D();
			}
			else {
				offset = new Vector3D(model.Bounds.Location.X, model.Bounds.Location.Y, model.Bounds.Location.Z);
				size = new Size3D(model.Bounds.Size.X / 2.0, model.Bounds.Size.Y / 2.0, model.Bounds.Size.Z / 2.0);
			}
		}
		void ModifyTransform(Transform3D transform) {
			Transform3DGroup group = transform as Transform3DGroup;
			if (group == null) {
				RotateTransform3D rotate = transform as RotateTransform3D;
				if (rotate == null) {
					TranslateTransform3D translate = transform as TranslateTransform3D;
					if (translate == null) {
						ScaleTransform3D scale = transform as ScaleTransform3D;
						if (scale != null) {
							scale.CenterX *= size.X;
							scale.CenterY *= size.Y;
							scale.CenterZ *= size.Z;
						}
					}
					else {
						translate.OffsetX *= size.X;
						translate.OffsetY *= size.Y;
						translate.OffsetZ *= size.Z;
					}
				}
				else {
					rotate.CenterX *= size.X;
					rotate.CenterY *= size.Y;
					rotate.CenterZ *= size.Z;
				}
			}
			else
				foreach (Transform3D child in group.Children)
					ModifyTransform(child);
		}
		protected override Transform3D GetAdditionalTransform() {
			Transform3D transform = Point.GetValue(SeriesPointModel3D.TransformProperty) as Transform3D;
			if (transform == null)
				return null;
			transform = transform.CloneCurrentValue();
			ModifyTransform(transform);
			Vector3D fullOffset = new Vector3D(offset.X + size.X, offset.Y + size.Y, offset.Z + size.Z);
			Transform3DGroup group = new Transform3DGroup();
			group.Children.Add(new TranslateTransform3D(-fullOffset));
			group.Children.Add(transform);
			group.Children.Add(new TranslateTransform3D(fullOffset));
			return group;
		}
	}
	public class PieModel3DInfo : Model3DInfo {
		double angleRadian;
		double radius;
		SeriesPoint point;
		public PieModel3DInfo(SeriesPoint point, Model3D model, double angle, double radius, Pie3DLabelLayout labelLayout) : base(point, model, labelLayout) {
			this.angleRadian = MathUtils.Degree2Radian(angle);
			this.radius = radius;
			this.point = point;
		}
		protected override Transform3D GetAdditionalTransform() {
			double offset = radius * (double)point.GetValue(PieSeries.ExplodedDistanceProperty);
			return new TranslateTransform3D(offset * Math.Cos(angleRadian), 0, -offset * Math.Sin(angleRadian));
		}
	}
}
