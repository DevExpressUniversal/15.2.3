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
namespace DevExpress.Xpf.Charts.Native {
	public static class Model3DExtractor {
		#region inner classes
		abstract class GeometryContainer {
			public static GeometryContainer CreateInstance(object obj) {
				if (obj is Model3D)
					return Model3DContainer.CreateInstance((Model3D)obj);
				else if (obj is Visual3D)
					return Visual3DContainer.CreateInstance((Visual3D)obj);
				else if (obj is Visual)
					return VisualContainer.CreateInstance((Visual)obj);
				else
					return null;
			}
			protected abstract Matrix3D Matrix { get; }
			protected abstract GeometryModel3D Model { get; }
			protected GeometryContainer() {
			}
			void AddOwnModel(Model3DGroup modelHolder, Matrix3D matrix) {
				if (Model == null)
					return;
				GeometryModel3D newModel = Model;
				newModel.Transform = new MatrixTransform3D(matrix);
				modelHolder.Children.Add(newModel);
			}
			void AddChildrenModels(Model3DGroup modelHolder, Matrix3D matrix) {
				List<GeometryContainer> children = GetChildren();
				if (children == null || children.Count == 0)
					return;
				foreach (GeometryContainer child in children)
					child.AddModels(modelHolder, matrix);
			}
			void AddModels(Model3DGroup modelHolder, Matrix3D parentMatrix) {
				Matrix3D matrix = Matrix * parentMatrix;
				AddOwnModel(modelHolder, matrix);
				AddChildrenModels(modelHolder, matrix);
			}
			protected abstract List<GeometryContainer> GetChildren();
			public Model3DGroup GetModel() {
				Model3DGroup modelHolder = new Model3DGroup();
				AddModels(modelHolder, Matrix3D.Identity);
				return modelHolder;
			}
		}
		abstract class Model3DContainer : GeometryContainer {
			public static Model3DContainer CreateInstance(Model3D model) {
				if (model is GeometryModel3D)
					return new GeometryModel3DContainer((GeometryModel3D)model);
				else if (model is Model3DGroup)
					return new Model3DGroupContainer((Model3DGroup)model);
				else
					return null;
			}
		}
		class GeometryModel3DContainer : Model3DContainer {
			GeometryModel3D model;
			protected override Matrix3D Matrix { get { return model.Transform == null ? Matrix3D.Identity : model.Transform.Value; } }
			protected override GeometryModel3D Model { get { return model; } }
			public GeometryModel3DContainer(GeometryModel3D model) {
				this.model = model;
			}
			protected override List<GeometryContainer> GetChildren() {
				return null;
			}
		}
		class Model3DGroupContainer : Model3DContainer {
			Model3DGroup group;
			protected override Matrix3D Matrix { get { return group.Transform == null ? Matrix3D.Identity : group.Transform.Value; } }
			protected override GeometryModel3D Model { get { return null; } }
			public Model3DGroupContainer(Model3DGroup group) {
				this.group = group;
			}
			protected override List<GeometryContainer> GetChildren() {
				List<GeometryContainer> children = new List<GeometryContainer>();
				foreach (Model3D model in group.Children) {
					GeometryContainer container = Model3DContainer.CreateInstance(model);
					if (container != null)
						children.Add(container);
				}
				return children;
			}
		}
		abstract class Visual3DContainer : GeometryContainer {
			public static ModelVisual3DContainer CreateInstance(Visual3D visual) {
				if (visual is ModelVisual3D)
					return new ModelVisual3DContainer((ModelVisual3D)visual);
				else
					return null;
			}
		}
		class ModelVisual3DContainer : Visual3DContainer {
			ModelVisual3D visual;
			protected override Matrix3D Matrix { get { return visual.Transform == null ? Matrix3D.Identity : visual.Transform.Value; } }
			protected override GeometryModel3D Model { get { return null; } }
			public ModelVisual3DContainer(ModelVisual3D visual) {
				this.visual = visual;
			}
			protected override List<GeometryContainer> GetChildren() {
				List<GeometryContainer> children = new List<GeometryContainer>();
				GeometryContainer container = Model3DContainer.CreateInstance(visual.Content);
				if (container != null)
					children.Add(container);
				foreach (Visual3D child in visual.Children) {
					container = Visual3DContainer.CreateInstance(child);
					if (container != null)
						children.Add(container);
				}
				return children;
			}
		}
		class VisualContainer : GeometryContainer {
			public static VisualContainer CreateInstance(Visual visual) {
				if (visual is Viewport3D)
					return new Viewport3DContainer((Viewport3D)visual);
				else if (visual is Page)
					return new PageContainer((Page)visual);
				else
					return new VisualContainer(visual);
			}
			Visual visual;
			protected override Matrix3D Matrix { get { return Matrix3D.Identity; } }
			protected override GeometryModel3D Model { get { return null; } }
			public VisualContainer(Visual visual) {
				this.visual = visual;
			}
			protected override List<GeometryContainer> GetChildren() {
				List<GeometryContainer> children = new List<GeometryContainer>();
				for (int i = 0; i < VisualTreeHelper.GetChildrenCount(visual); i++) {
					Visual child = (Visual)VisualTreeHelper.GetChild(visual, i);
					GeometryContainer container = VisualContainer.CreateInstance(child);
					if (container != null)
						children.Add(container);
				}
				return children;
			}
		}
		class Viewport3DContainer : VisualContainer {
			Viewport3D viewport;
			protected override Matrix3D Matrix { get { return Matrix3D.Identity; } }
			protected override GeometryModel3D Model { get { return null; } }
			public Viewport3DContainer(Viewport3D viewport)
				: base(viewport) {
				this.viewport = viewport;
			}
			protected override List<GeometryContainer> GetChildren() {
				List<GeometryContainer> children = new List<GeometryContainer>();
				foreach (Visual3D visual in viewport.Children) {
					GeometryContainer container = Visual3DContainer.CreateInstance(visual);
					if (container != null)
						children.Add(container);
				}
				return children;
			}
		}
		class PageContainer : VisualContainer {
			Page page;
			protected override Matrix3D Matrix { get { return Matrix3D.Identity; } }
			protected override GeometryModel3D Model { get { return null; } }
			public PageContainer(Page page)
				: base(page) {
				this.page = page;
			}
			protected override List<GeometryContainer> GetChildren() {
				List<GeometryContainer> children = new List<GeometryContainer>();
				GeometryContainer child = GeometryContainer.CreateInstance(page.Content);
				if (child != null)
					children.Add(child);
				return children;
			}
		}
		#endregion
		public static Model3DGroup Extract(object obj) {
			GeometryContainer container = GeometryContainer.CreateInstance(obj);
			if (container == null)
				return null;
			Model3DGroup model = container.GetModel();
			if (model == null || model.Children.Count == 0)
				return null;
			return model;
		}
	}
}
