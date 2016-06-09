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
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Media3D;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public abstract class Marker3DModel : ModelBase {
		public static IEnumerable<Marker3DKind> GetPredefinedKinds() {
			return Marker3DKind.List;
		}
		Model3D model;
		protected abstract bool IsInternalModel { get; }
		protected internal abstract string ActualSource { get; }
		protected abstract bool ActualLoadFromResources { get; }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("Marker3DModelModelName")]
#endif
		public abstract string ModelName { get; }
		internal Model3D GetModel() {
			if (model == null)
				model = Model3DExtractor.Extract(LoadObject(ActualSource, ActualLoadFromResources, IsInternalModel));
			return model;
		}
		protected abstract Marker3DModel CreateObjectForClone();
		protected virtual void Assign(Marker3DModel model) { }
		protected internal Marker3DModel CloneModel() {
			Marker3DModel model = CreateObjectForClone();
			model.Assign(this);
			return model;
		}
		protected internal override void ClearCache() {
			model = null;
		}
	}
	public abstract class PredefinedMarker3DModel : Marker3DModel {
		protected override bool IsInternalModel { get { return true; } }
		protected override bool ActualLoadFromResources { get { return true; } }
	}
	public class CapsuleMarker3DModel : PredefinedMarker3DModel {
		protected internal override string ActualSource { get { return "Marker3D/capsule.xaml"; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("CapsuleMarker3DModelModelName")]
#endif
		public override string ModelName { get { return "Capsule"; } }
		protected override Marker3DModel CreateObjectForClone() {
			return new CapsuleMarker3DModel();
		}
	}
	public class ConeMarker3DModel : PredefinedMarker3DModel {
		protected internal override string ActualSource { get { return "Marker3D/cone.xaml"; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("ConeMarker3DModelModelName")]
#endif
		public override string ModelName { get { return "Cone"; } }
		protected override Marker3DModel CreateObjectForClone() {
			return new ConeMarker3DModel();
		}
	}
	public class CubeMarker3DModel : PredefinedMarker3DModel {
		protected internal override string ActualSource { get { return "Marker3D/cube.xaml"; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("CubeMarker3DModelModelName")]
#endif
		public override string ModelName { get { return "Cube"; } }
		protected override Marker3DModel CreateObjectForClone() {
			return new CubeMarker3DModel();
		}
	}
	public class CylinderMarker3DModel : PredefinedMarker3DModel {
		protected internal override string ActualSource { get { return "Marker3D/cylinder.xaml"; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("CylinderMarker3DModelModelName")]
#endif
		public override string ModelName { get { return "Cylinder"; } }
		protected override Marker3DModel CreateObjectForClone() {
			return new CylinderMarker3DModel();
		}
	}
	public class HexagonMarker3DModel : PredefinedMarker3DModel {
		protected internal override string ActualSource { get { return "Marker3D/hexagon.xaml"; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("HexagonMarker3DModelModelName")]
#endif
		public override string ModelName { get { return "Hexagon"; } }
		protected override Marker3DModel CreateObjectForClone() {
			return new HexagonMarker3DModel();
		}
	}
	public class PyramidMarker3DModel : PredefinedMarker3DModel {
		protected internal override string ActualSource { get { return "Marker3D/pyramid.xaml"; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("PyramidMarker3DModelModelName")]
#endif
		public override string ModelName { get { return "Pyramid"; } }
		protected override Marker3DModel CreateObjectForClone() {
			return new PyramidMarker3DModel();
		}
	}
	public class RoundedCubeMarker3DModel : PredefinedMarker3DModel {
		protected internal override string ActualSource { get { return "Marker3D/rounded_cube.xaml"; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("RoundedCubeMarker3DModelModelName")]
#endif
		public override string ModelName { get { return "Rounded Cube"; } }
		protected override Marker3DModel CreateObjectForClone() {
			return new RoundedCubeMarker3DModel();
		}
	}
	public class SphereMarker3DModel : PredefinedMarker3DModel {
		protected internal override string ActualSource { get { return "Marker3D/sphere.xaml"; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("SphereMarker3DModelModelName")]
#endif
		public override string ModelName { get { return "Sphere"; } }
		protected override Marker3DModel CreateObjectForClone() {
			return new SphereMarker3DModel();
		}
	}
	public class StarMarker3DModel : PredefinedMarker3DModel {
		protected internal override string ActualSource { get { return "Marker3D/star.xaml"; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("StarMarker3DModelModelName")]
#endif
		public override string ModelName { get { return "Star"; } }
		protected override Marker3DModel CreateObjectForClone() {
			return new StarMarker3DModel();
		}
	}
	public class CustomMarker3DModel : Marker3DModel {
		public static readonly DependencyProperty LoadFromResourcesProperty;
		public static readonly DependencyProperty SourceProperty;
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("CustomMarker3DModelLoadFromResources"),
#endif
		Category(Categories.Common)
		]
		public bool LoadFromResources {
			get { return (bool)GetValue(LoadFromResourcesProperty); }
			set { SetValue(LoadFromResourcesProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("CustomMarker3DModelSource"),
#endif
		Category(Categories.Common)
		]
		public string Source {
			get { return (string)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}
		static CustomMarker3DModel() {
			Type ownerType = typeof(CustomMarker3DModel);
			LoadFromResourcesProperty = DependencyProperty.Register("LoadFromResources", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, PropertyChanged));
			SourceProperty = DependencyProperty.Register("Source", typeof(string), ownerType,
				new FrameworkPropertyMetadata(PropertyChanged));
		}
		protected override bool IsInternalModel { get { return false; } }
		protected override bool ActualLoadFromResources { get { return LoadFromResources; } }
		protected internal override string ActualSource { get { return Source; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("CustomMarker3DModelModelName")]
#endif
		public override string ModelName { get { return "Custom"; } }
		protected override Marker3DModel CreateObjectForClone() {
			return new CustomMarker3DModel();
		}
		protected override void Assign(Marker3DModel model) {
			base.Assign(model);
			CustomMarker3DModel customMarker3DModel = model as CustomMarker3DModel;
			if (customMarker3DModel != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, customMarker3DModel, LoadFromResourcesProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, customMarker3DModel, SourceProperty);
			}
		}
	}
}
