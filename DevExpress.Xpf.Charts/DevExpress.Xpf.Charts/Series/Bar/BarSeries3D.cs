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
	public class BarSeries3D : XYSeries3D, IBarSeriesView {
		public static readonly DependencyProperty ModelProperty;
		public static readonly DependencyProperty BarWidthProperty;
		public static readonly DependencyProperty TransformProperty;
		static BarSeries3D() {
			Type ownerType = typeof(BarSeries3D);
			ModelProperty = DependencyProperty.Register("Model", typeof(Bar3DModel), ownerType, new PropertyMetadata(ModelPropertyChanged));
			BarWidthProperty = DependencyProperty.Register("BarWidth", typeof(double), ownerType,
				new PropertyMetadata(0.6, BarWidthPropertyChanged), new ValidateValueCallback(BarWidthValidation));
			TransformProperty = DependencyProperty.RegisterAttached("Transform", typeof(Transform3D), ownerType, new PropertyMetadata(TransformPropertyChanged));
		}
		static bool BarWidthValidation(object barWidth) {
			return (double)barWidth > 0;
		}
		static void BarWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
		   IChartElement series = ((IChartElement)d).Owner as IChartElement;
		   ChartElementHelper.Update(d, new SeriesGroupsInteractionUpdateInfo(series), ChartElementChange.ClearDiagramCache);		   
		}
		static void ModelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			BarSeries3D series = d as BarSeries3D;
			if (series != null) {
				Bar3DModel model = e.NewValue as Bar3DModel;
				if (model == null) {
					series.actualModel = new BoxBar3DModel();
					e = new DependencyPropertyChangedEventArgs(e.Property, e.OldValue, series.actualModel);
				}
				else
					series.actualModel = model;
			}
			ChartElementHelper.ChangeOwnerAndUpdateWithClearDiagramCache(d, e);
		}
		static void TransformPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SeriesPoint.SynchronizeProperties(d, e, TransformProperty, SeriesPointModel3D.TransformProperty);
			ChartElementHelper.UpdateWithClearDiagramCache(d, e);
		}
		[
		Category(Categories.Layout)
		]
		public static Transform3D GetTransform(SeriesPoint point) {
			return (Transform3D)point.GetValue(TransformProperty);
		}
		public static void SetTransform(SeriesPoint point, Transform3D transform) {
			point.SetValue(TransformProperty, transform);
		}
		Bar3DModel actualModel;
		protected internal override Type SupportedDiagramType { get { return typeof(XYDiagram3D); } }
		protected internal override bool IsPredefinedModelUses { get { return ActualModel is PredefinedBar3DModel; } }
		protected internal override double SeriesDepth { get { return BarWidth; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("BarSeries3DActualModel")]
#endif
		public Bar3DModel ActualModel { get { return actualModel; } }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("BarSeries3DModel"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public Bar3DModel Model {
			get { return (Bar3DModel)GetValue(ModelProperty); }
			set { SetValue(ModelProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("BarSeries3DBarWidth"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public double BarWidth {
			get { return (double)GetValue(BarWidthProperty); }
			set { SetValue(BarWidthProperty, value); }
		}
		public BarSeries3D() {
			actualModel = new BoxBar3DModel();
			actualModel.SetOwner(this);
			DefaultStyleKey = typeof(BarSeries3D);
		}
		protected override Series CreateObjectForClone() {
			return new BarSeries3D();
		}
		protected override void AssignForBinding(Series series) {
			base.AssignForBinding(series);
			BarSeries3D barSeries3D = series as BarSeries3D;
			if (barSeries3D != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, barSeries3D, BarWidthProperty);
				if (CopyPropertyValueHelper.IsValueSet(barSeries3D, ModelProperty))
					if (CopyPropertyValueHelper.VerifyValues(this, barSeries3D, ModelProperty))
						Model = barSeries3D.Model.CloneModel();
			}
		}
		protected internal override SeriesData CreateSeriesData() {
			SeriesData = new BarSeries3DData(this);
			return SeriesData;
		}
	}
}
