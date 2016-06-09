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
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Media3D;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public class PieSeries3D : PieSeries {
		public static readonly DependencyProperty ModelProperty;
		public static readonly DependencyProperty DepthTransformProperty;
		static PieSeries3D() {
			Type ownerType = typeof(PieSeries3D);
			ModelProperty = DependencyProperty.Register("Model", typeof(Pie3DModel),
				ownerType, new FrameworkPropertyMetadata(ModelPropertyChanged));
			DepthTransformProperty = DependencyProperty.Register("DepthTransform", typeof(double), ownerType,
				new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender,
					ChartElementHelper.UpdateWithClearDiagramCache),
				new ValidateValueCallback(DepthTransformValidation));
		}
		static bool DepthTransformValidation(object depthTransform) {
			return (double)depthTransform > 0;
		}
		static void ModelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PieSeries3D series = d as PieSeries3D;
			if (series != null) {
				Pie3DModel model = e.NewValue as Pie3DModel;
				if (model == null) {
					series.actualModel = new RectanglePie3DModel();
					e = new DependencyPropertyChangedEventArgs(e.Property, e.OldValue, series.actualModel);
				}
				else
					series.actualModel = model;
			}
			ChartElementHelper.ChangeOwnerAndUpdateWithClearDiagramCache(d, e);
		}
		Pie3DModel actualModel;
		protected internal override Type SupportedDiagramType { get { return typeof(SimpleDiagram3D); } }
		protected internal override VisualSelectionType SupportedSelectionType { get { return VisualSelectionType.Brightness; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("PieSeries3DActualModel")]
#endif
		public Pie3DModel ActualModel { get { return actualModel; } }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("PieSeries3DModel"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public Pie3DModel Model {
			get { return (Pie3DModel)GetValue(ModelProperty); }
			set { SetValue(ModelProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("PieSeries3DDepthTransform"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public double DepthTransform {
			get { return (double)GetValue(DepthTransformProperty); }
			set { SetValue(DepthTransformProperty, value); }
		}
		public PieSeries3D() {
			actualModel = new RectanglePie3DModel();
			actualModel.SetOwner(this);
			DefaultStyleKey = typeof(PieSeries3D);
		}
		protected override Series CreateObjectForClone() {
			return new PieSeries3D();
		}
		protected internal override SeriesData CreateSeriesData() {
			SeriesData = new PieSeries3DData(this);
			return SeriesData;
		}
		protected override void AssignForBinding(Series series) {
			base.AssignForBinding(series);
			PieSeries3D pieSeries3D = series as PieSeries3D;
			if (pieSeries3D != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, pieSeries3D, DepthTransformProperty);
				if (CopyPropertyValueHelper.IsValueSet(pieSeries3D, ModelProperty))
					if (CopyPropertyValueHelper.VerifyValues(this, pieSeries3D, ModelProperty))
						Model = pieSeries3D.Model.CloneModel();
			}
		}
		protected override void FillPredefinedPointAnimationKinds(List<AnimationKind> animationKinds) {
		}
		protected override void FillPredefinedSeriesAnimationKinds(List<AnimationKind> animationKinds) {
		}
		protected override System.Windows.Media.Color GetSeriesPointColor(DevExpress.Charts.Native.RefinedPoint refinedPoint, bool isSelected) {
			if (isSelected && !(ActualModel is PredefinedPie3DModel))
				return VisualSelectionHelper.CustomModelSelectionColor;
			else
				return base.GetSeriesPointColor(refinedPoint, isSelected);
		}
		protected internal override void ChangeSeriesPointSelection(SeriesPoint seriesPoint, bool isSelected) {
			base.ChangeSeriesPointSelection(seriesPoint, isSelected);
			SimpleDiagram3D diagram3D = Diagram as SimpleDiagram3D;
			if (diagram3D != null) {
				Cache.UpdateSelectionForPointCache(seriesPoint, isSelected, true);
				diagram3D.InvalidateDiagram();
			}
		}
		protected override bool Is3DView { get { return true; } }
		internal Point CalculateToolTipPoint(SeriesPoint point) {
			Point location = new Point();
			Diagram3D diagram3D = Diagram as Diagram3D;
			SeriesPointCache cache = Cache.GetSeriesPointCache(point);
			if (cache != null && diagram3D != null && diagram3D.ChartControl != null) {
				IRefinedSeries refinedSeries = diagram3D.ViewController.GetRefinedSeries(this);
				VisualContainer container = diagram3D.VisualContainers[diagram3D.ViewController.ActiveRefinedSeries.IndexOf(refinedSeries)];
				Diagram3DDomain domain = diagram3D.CreateDomain(container);
				Point3D toolTipPoint = SeriesData.CalculateToolTipPoint(cache, domain);
				location = domain.Project(toolTipPoint);
				Rect diagramRect = LayoutHelper.GetRelativeElementRect(container, diagram3D.ChartControl);
				location.X += diagramRect.Left;
				location.Y += diagramRect.Top;
			}
			return location;
		}
	}
}
