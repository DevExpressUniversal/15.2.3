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
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public class AreaSeries3D : XYSeries3D, IGeometryHolder {
		public static readonly DependencyProperty AreaWidthProperty;
		public static readonly DependencyProperty MaterialProperty;
		static AreaSeries3D() {
			Type ownerType = typeof(AreaSeries3D);
			AreaWidthProperty = DependencyProperty.Register("AreaWidth", typeof(double), ownerType,
				new FrameworkPropertyMetadata(0.6, ChartElementHelper.UpdateWithClearDiagramCache),
				new ValidateValueCallback(AreaWidthValidation));
			MaterialProperty = DependencyProperty.Register("Material", typeof(Material), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, MaterialPropertyChanged));
		}
		static bool AreaWidthValidation(object areaWidth) {
			return (double)areaWidth > 0;
		}
		static void MaterialPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			AreaSeries3D series = (AreaSeries3D)d;
			if (series != null) {
				Material material = e.NewValue as Material;
				series.actualMaterial = material == null ? CreateDefaultMaterial() : material;
			}
			ChartElementHelper.UpdateWithClearDiagramCache(d, e);
		}
		static Material CreateDefaultMaterial() {
			MaterialGroup group = new MaterialGroup();
			group.Children.Add(new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(128, 128, 128))));
			group.Children.Add(new SpecularMaterial(new SolidColorBrush(Color.FromRgb(229, 229, 229)), 56.0));
			return group;
		}
		Material actualMaterial;
#if !SL
	[DevExpressXpfChartsLocalizedDescription("AreaSeries3DActualMaterial")]
#endif
		public Material ActualMaterial { get { return actualMaterial; } }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("AreaSeries3DAreaWidth"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public double AreaWidth {
			get { return (double)GetValue(AreaWidthProperty); }
			set { SetValue(AreaWidthProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("AreaSeries3DMaterial"),
#endif
		Category(Categories.Presentation)
		]
		public Material Material {
			get { return (Material)GetValue(MaterialProperty); }
			set { SetValue(MaterialProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool ColorEach {
			get { return base.ColorEach; }
			set { base.ColorEach = value; }
		}
		protected override bool ShouldSortPoints { get { return true; } }
		protected internal override Type SupportedDiagramType { get { return typeof(XYDiagram3D); } }
		protected internal override double SeriesDepth { get { return AreaWidth; } }
		protected internal override bool ActualColorEach { get { return false; } }
		protected internal override bool IsPredefinedModelUses { get { return Material == null; } }
		public AreaSeries3D() {
			actualMaterial = CreateDefaultMaterial();
			DefaultStyleKey = typeof(AreaSeries3D);
		}
		#region IGeometryHolder
		GeometryStripCreator IGeometryHolder.CreateStripCreator() {
			return CreateStripCreator();
		}
		#endregion
		protected virtual GeometryStripCreator CreateStripCreator() {
			return new AreaGeometryStripCreator();
		}
		protected override Series CreateObjectForClone() {
			return new AreaSeries3D();
		}
		protected internal override SeriesData CreateSeriesData() {
			SeriesData = new AreaSeries3DData(this);
			return SeriesData;
		}
		protected override void AssignForBinding(Series series) {
			base.AssignForBinding(series);
			AreaSeries3D areaSeries3D = series as AreaSeries3D;
			if (areaSeries3D != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, areaSeries3D, AreaWidthProperty);
				if (CopyPropertyValueHelper.IsValueSet(areaSeries3D, MaterialProperty))
					if (CopyPropertyValueHelper.VerifyValues(this, areaSeries3D, MaterialProperty))
						Material = areaSeries3D.Material.CloneCurrentValue();
			}
		}
	}
}
