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
using System.Windows.Shapes;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public abstract class Pie3DModel : ModelBase {
		public static IEnumerable<Pie3DKind> GetPredefinedKinds() {
			return Pie3DKind.List;
		}
		Polyline polyline;
		protected abstract bool IsInternalModel { get; }
		protected abstract string ActualSource { get; }
		protected abstract bool ActualLoadFromResources { get; }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("Pie3DModelModelName")]
#endif
		public abstract string ModelName { get; }
		internal Size GetSize() {
			Polyline polyline = GetPolyline();
			return polyline == null ? Size.Empty : polyline.RenderedGeometry.Bounds.Size;
		}
		internal Polyline GetPolyline() {
			if (polyline == null)
				polyline = ShapeExtractor.ExtractPolyline(LoadObject(ActualSource, ActualLoadFromResources, IsInternalModel));
			return polyline;
		}
		protected abstract Pie3DModel CreateObjectForClone();
		protected virtual void Assign(Pie3DModel model) { }
		protected internal Pie3DModel CloneModel() {
			Pie3DModel model = CreateObjectForClone();
			model.Assign(this);
			return model;
		}
		protected internal override void ClearCache() {
			polyline = null;
		}
	}
	public abstract class PredefinedPie3DModel : Pie3DModel {
		protected override bool IsInternalModel { get { return true; } }
		protected override bool ActualLoadFromResources { get { return true; } }
	}
	public class CirclePie3DModel : PredefinedPie3DModel {
		protected override string ActualSource { get { return @"pie3D/circle.xaml"; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("CirclePie3DModelModelName")]
#endif
		public override string ModelName { get { return "Circle"; } }
		protected override Pie3DModel CreateObjectForClone() {
			return new CirclePie3DModel();
		}
	}
	public class RectanglePie3DModel : PredefinedPie3DModel {
		protected override string ActualSource { get { return @"pie3D/rectangle.xaml"; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("RectanglePie3DModelModelName")]
#endif
		public override string ModelName { get { return "Rectangle"; } }
		protected override Pie3DModel CreateObjectForClone() {
			return new RectanglePie3DModel();
		}
	}
	public class PentagonPie3DModel : PredefinedPie3DModel {
		protected override string ActualSource { get { return @"pie3D/pentagon.xaml"; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("PentagonPie3DModelModelName")]
#endif
		public override string ModelName { get { return "Pentagon"; } }
		protected override Pie3DModel CreateObjectForClone() {
			return new PentagonPie3DModel();
		}
	}
	public class HexagonPie3DModel : PredefinedPie3DModel {
		protected override string ActualSource { get { return @"pie3D/hexagon.xaml"; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("HexagonPie3DModelModelName")]
#endif
		public override string ModelName { get { return "Hexagon"; } }
		protected override Pie3DModel CreateObjectForClone() {
			return new HexagonPie3DModel();
		}
	}
	public class RoundedRectanglePie3DModel : PredefinedPie3DModel {
		protected override string ActualSource { get { return @"pie3D/rounded_rectangle.xaml"; } }
		public override string ModelName { get { return "Rounded Rectangle"; } }
		protected override Pie3DModel CreateObjectForClone() {
			return new RoundedRectanglePie3DModel();
		}
	}
	public class SemiCirclePie3DModel : PredefinedPie3DModel {
		protected override string ActualSource { get { return @"pie3D/semi_circle.xaml"; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("SemiCirclePie3DModelModelName")]
#endif
		public override string ModelName { get { return "Semi Circle"; } }
		protected override Pie3DModel CreateObjectForClone() {
			return new SemiCirclePie3DModel();
		}
	}
	public class SemiRectanglePie3DModel : PredefinedPie3DModel {
		protected override string ActualSource { get { return @"pie3D/semi_rectangle.xaml"; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("SemiRectanglePie3DModelModelName")]
#endif
		public override string ModelName { get { return "Semi Rectangle"; } }
		protected override Pie3DModel CreateObjectForClone() {
			return new SemiRectanglePie3DModel();
		}
	}
	public class SemiPentagonPie3DModel : PredefinedPie3DModel {
		protected override string ActualSource { get { return @"pie3D/semi_pentagon.xaml"; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("SemiPentagonPie3DModelModelName")]
#endif
		public override string ModelName { get { return "Semi Pentagon"; } }
		protected override Pie3DModel CreateObjectForClone() {
			return new SemiPentagonPie3DModel();
		}
	}
	public class SemiHexagonPie3DModel : PredefinedPie3DModel {
		protected override string ActualSource { get { return @"pie3D/semi_hexagon.xaml"; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("SemiHexagonPie3DModelModelName")]
#endif
		public override string ModelName { get { return "Semi Hexagon"; } }
		protected override Pie3DModel CreateObjectForClone() {
			return new SemiHexagonPie3DModel();
		}
	}
	public class SemiRoundedRectanglePie3DModel : PredefinedPie3DModel {
		protected override string ActualSource { get { return @"pie3D/semi_rounded_rectangle.xaml"; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("SemiRoundedRectanglePie3DModelModelName")]
#endif
		public override string ModelName { get { return "Semi Rounded Rectangle"; } }
		protected override Pie3DModel CreateObjectForClone() {
			return new SemiRoundedRectanglePie3DModel();
		}
	}
	public class CustomPie3DModel : Pie3DModel {
		public static readonly DependencyProperty SourceProperty;
		public static readonly DependencyProperty LoadFromResourcesProperty;
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("CustomPie3DModelSource"),
#endif
		Category(Categories.Common)
		]
		public string Source {
			get { return (string)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("CustomPie3DModelLoadFromResources"),
#endif
		Category(Categories.Common)
		]
		public bool LoadFromResources {
			get { return (bool)GetValue(LoadFromResourcesProperty); }
			set { SetValue(LoadFromResourcesProperty, value); }
		}
		static CustomPie3DModel() {
			Type ownerType = typeof(CustomPie3DModel);
			SourceProperty = DependencyProperty.Register("Source", typeof(string), ownerType,
				new FrameworkPropertyMetadata(PropertyChanged));
			LoadFromResourcesProperty = DependencyProperty.Register("LoadFromResources", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, PropertyChanged));
		}
		protected override bool IsInternalModel { get { return false; } }
		protected override string ActualSource { get { return Source; } }
		protected override bool ActualLoadFromResources { get { return LoadFromResources; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("CustomPie3DModelModelName")]
#endif
		public override string ModelName { get { return "Custom"; } }
		protected override Pie3DModel CreateObjectForClone() {
			return new CustomPie3DModel();
		}
		protected override void Assign(Pie3DModel model) {
			base.Assign(model);
			CustomPie3DModel customPie3DModel = model as CustomPie3DModel;
			if (customPie3DModel != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, customPie3DModel, LoadFromResourcesProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, customPie3DModel, SourceProperty);
			}
		}
	}
}
