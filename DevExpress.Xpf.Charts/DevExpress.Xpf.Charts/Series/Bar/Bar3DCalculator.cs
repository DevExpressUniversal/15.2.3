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
using System.Windows.Media;
using System.Windows.Media.Media3D;
namespace DevExpress.Xpf.Charts.Native {
	public class Bar3DSectionData {
		public static Bar3DSectionData CreateInstance(Bar3DSection section, bool loadFromResources) {
			if(section == null)
				return null;
			Model3D model = section.GetModel(loadFromResources);
			if (model == null)
				return null;
			Rect3D bounds = Graphics3DUtils.CalcBounds(Graphics3DUtils.GetRefPoints(model.Bounds),
				!section.ActualAlignByX, !section.ActualAlignByY, !section.ActualAlignByZ);
			if (bounds.IsEmpty)
				return null;
			return new Bar3DSectionData(model, section.ActualFixedHeight, section.ActualUseViewColor, bounds);
		}
		Model3D model;
		bool fixedHeight;
		bool useViewColor;
		Rect3D bounds;
		public Model3D Model { get { return model; } }
		public bool FixedHeight { get { return fixedHeight; } }
		public bool UseViewColor { get { return useViewColor; } }
		public Rect3D Bounds { get { return bounds; } }
		Bar3DSectionData(Model3D model, bool fixedHeight, bool useViewColor, Rect3D bounds) {
			this.model = model;
			this.fixedHeight = fixedHeight;
			this.useViewColor = useViewColor;
			this.bounds = bounds;
		}
		void AppendTransform(Matrix3D matrix) {
			Transform3D transform = new MatrixTransform3D(matrix);
			bounds = transform.TransformBounds(bounds);
			if (model.Transform == null)
				model.Transform = transform;
			else {
				Matrix3D value = model.Transform.Value;
				value.Append(matrix);
				model.Transform = new MatrixTransform3D(value);
			}
		}
		public void SetBrush(SolidColorBrush brush) {
			if (UseViewColor)
				Graphics3DUtils.HandleModel(model, delegate(GeometryModel3D geometryModel) { Graphics3DUtils.SetMaterialBrush(geometryModel.Material, brush, true); });
		}
		public void Scale(Vector3D vector) {
			ScaleAt(vector, MathUtils.CalcCenter(bounds));
		}
		public void Offset(Point3D point) {
			Matrix3D matrix = new Matrix3D();
			Point3D center = MathUtils.CalcCenter(bounds);
			matrix.OffsetX = point.X - center.X;
			matrix.OffsetY = point.Y - center.Y;
			matrix.OffsetZ = point.Z - center.Z;
			AppendTransform(matrix);
		}
		public void ScaleAt(Vector3D vector, Point3D center) {
			Matrix3D matrix = new Matrix3D();
			matrix.ScaleAt(vector, center);
			AppendTransform(matrix);
		}
		public Bar3DSectionData CloneCurrentValue() {
			return new Bar3DSectionData(model.CloneCurrentValue(), fixedHeight, useViewColor, bounds);
		}
	}
	public static class Bar3DCalculator {
		static Vector3D CalcScaleVectorByXZ(double sizeX, double sizeZ, double innerSizeX, double innerSizeZ) {
			if (innerSizeX == 0 && innerSizeZ == 0)
				return new Vector3D(1, 1, 1);
			double ratio;
			if (innerSizeX == 0)
				ratio = sizeZ / innerSizeZ;
			else if (innerSizeZ == 0)
				ratio = sizeX / innerSizeX;
			else {
				double xRatio = sizeX / innerSizeX;
				double zRatio = sizeZ / innerSizeZ;
				ratio = xRatio < zRatio ? xRatio : zRatio;
			}
			return new Vector3D(ratio, ratio, ratio);
		}
		static Size3D CalcModelSize(IList<Bar3DSectionData> sectionsData) {
			double sizeX = 0;
			double sizeY = 0;
			double sizeZ = 0;
			foreach (Bar3DSectionData sectionData in sectionsData) {
				sizeX = Math.Max(sectionData.Bounds.SizeX, sizeX);
				sizeY = Math.Max(sectionData.Bounds.SizeY, sizeY);
				sizeZ = Math.Max(sectionData.Bounds.SizeZ, sizeZ);
			}
			return new Size3D(sizeX, sizeY, sizeZ);
		}
		static void InscribeByXZ(IList<Bar3DSectionData> sectionsData, Rect3D bounds) {
			Size3D modelSize = CalcModelSize(sectionsData);
			Vector3D vector = CalcScaleVectorByXZ(bounds.SizeX, bounds.SizeZ, modelSize.X, modelSize.Z);
			foreach (Bar3DSectionData sectionData in sectionsData)
				sectionData.Scale(vector);
		}
		static Vector3D CalcScaleVectorByY(double height, double innerHeight) {
			if (innerHeight == 0)
				return new Vector3D(1, 1, 1);
			double ratio = height / innerHeight;
			return new Vector3D(1, ratio, 1);
		}
		static List<Bar3DSectionData> InscribeFixedSectionsByY(IList<Bar3DSectionData> sectionsData, double sizeY, double fixedHeight, bool scaleFixed) {
			List<Bar3DSectionData> actualSectionsData = new List<Bar3DSectionData>();
			if (scaleFixed) {
				Vector3D vector = CalcScaleVectorByY(sizeY, fixedHeight);
				foreach (Bar3DSectionData sectionData in sectionsData) {
					if (sectionData.FixedHeight) {
						sectionData.Scale(vector);
						actualSectionsData.Add(sectionData);
					}
				}
			}
			else {
				double height = 0;
				for (int i = sectionsData.Count - 1; i >= 0; i--) {
					if (sectionsData[i].FixedHeight) {
						if (height < sizeY) {
							actualSectionsData.Insert(0, sectionsData[i]);
							height += sectionsData[i].Bounds.SizeY;
						}
						else
							break;
					}
				}
			}
			return actualSectionsData;
		}
		static List<Bar3DSectionData> InscribeSizableSectionsByY(IList<Bar3DSectionData> sectionsData, Rect3D bounds, double fixedHeight, double sizableHeight) {
			List<Bar3DSectionData> actualSectionsData = new List<Bar3DSectionData>();
			Vector3D vector = CalcScaleVectorByY(bounds.SizeY - fixedHeight, sizableHeight);
			foreach (Bar3DSectionData sectionData in sectionsData) {
				if (!sectionData.FixedHeight)
					sectionData.Scale(vector);
				actualSectionsData.Add(sectionData);
			}
			return actualSectionsData;
		}
		static List<Bar3DSectionData> InscribeByY(IList<Bar3DSectionData> sectionsData, Rect3D bounds, bool scaleFixed) {
			double fixedHeight = 0;
			double sizableHeight = 0;
			foreach (Bar3DSectionData sectionData in sectionsData) {
				if (sectionData.FixedHeight)
					fixedHeight += sectionData.Bounds.SizeY;
				else
					sizableHeight += sectionData.Bounds.SizeY;
			}
			return bounds.SizeY <= fixedHeight ?
				InscribeFixedSectionsByY(sectionsData, bounds.SizeY, fixedHeight, scaleFixed) :
				InscribeSizableSectionsByY(sectionsData, bounds, fixedHeight, sizableHeight);
		}
		static void Build(IList<Bar3DSectionData> sectionsData, Rect3D bounds) {
			Point3D center = MathUtils.CalcCenter(bounds);
			double sizeY = bounds.SizeY < 1 ? 1 : bounds.SizeY;
			double offsetY = bounds.Location.Y + sizeY;
			for (int i = sectionsData.Count - 1; i >= 0; i--) {
				sectionsData[i].Offset(new Point3D(center.X, offsetY - sectionsData[i].Bounds.SizeY / 2, center.Z));
				offsetY -= sectionsData[i].Bounds.SizeY;
			}
		}
		static void InvertByY(IList<Bar3DSectionData> sectionsData, Rect3D bounds) {
			Vector3D vector = new Vector3D(1, -1, 1);
			Point3D center = MathUtils.CalcCenter(bounds);
			foreach (Bar3DSectionData sectionData in sectionsData)
				sectionData.ScaleAt(vector, center);
		}
		public static Model3DGroup CalculateBar(IList<Bar3DSectionData> sectionsData, Rect3D bounds, bool invertByY, SolidColorBrush brush) {
			Model3DGroup group = new Model3DGroup();
			if (bounds.SizeY == 0 || sectionsData == null || sectionsData.Count == 0)
				return group;
			List<Bar3DSectionData> sectionsCopy = new List<Bar3DSectionData>(sectionsData.Count);
			foreach (Bar3DSectionData sectionData in sectionsData)
				sectionsCopy.Add(sectionData.CloneCurrentValue());
			InscribeByXZ(sectionsCopy, bounds);
			List<Bar3DSectionData> actualSectionsData = InscribeByY(sectionsCopy, bounds, true);
			if (actualSectionsData == null || actualSectionsData.Count == 0)
				return group;
			Build(actualSectionsData, bounds);
			if (invertByY)
				InvertByY(sectionsCopy, bounds);
			foreach (Bar3DSectionData sectionData in actualSectionsData) {
				sectionData.SetBrush(brush);
				group.Children.Add(sectionData.Model);
			}
			return group;
		}
	}
}
