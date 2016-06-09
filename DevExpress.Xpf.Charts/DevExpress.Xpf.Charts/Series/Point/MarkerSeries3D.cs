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
	public enum Marker3DLabelPosition {
		Center = 0,
		Top = 1
	}
	public abstract class MarkerSeries3D : XYSeries3D  {
		public static readonly DependencyProperty ModelProperty;
		public static readonly DependencyProperty LabelPositionProperty;
		public static readonly DependencyProperty TransformProperty;
		static MarkerSeries3D() {
			Type ownerType = typeof(MarkerSeries3D);
			ModelProperty = DependencyProperty.RegisterAttached("Model", typeof(Marker3DModel), ownerType, new PropertyMetadata(MarkerModelPropertyChanged));
			LabelPositionProperty = DependencyProperty.RegisterAttached("LabelPosition", typeof(Marker3DLabelPosition), 
				ownerType, new PropertyMetadata(Marker3DLabelPosition.Center, ChartElementHelper.UpdateWithClearDiagramCache)); 
			TransformProperty = DependencyProperty.RegisterAttached("Transform", typeof(Transform3D), ownerType, new PropertyMetadata(TransformPropertyChanged));
		}
		protected static void MarkerModelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MarkerSeries3D series = d as MarkerSeries3D;
			if (series != null) {
				Marker3DModel model = e.NewValue as Marker3DModel;
				if (model == null) {
					series.actualModel = new SphereMarker3DModel();
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
		Category(Categories.Layout),
		XtraSerializableProperty
		]
		public static Marker3DLabelPosition GetLabelPosition(SeriesLabel label) {
			return (Marker3DLabelPosition)label.GetValue(LabelPositionProperty);
		}
		public static void SetLabelPosition(SeriesLabel label, Marker3DLabelPosition position) {
			label.SetValue(LabelPositionProperty, position);
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
		Marker3DModel actualModel;
		protected abstract double Size { get; }
		protected internal override Type SupportedDiagramType { get { return typeof(XYDiagram3D); } }
		protected internal override bool IsPredefinedModelUses { get { return ActualModel is PredefinedMarker3DModel; } }		
#if !SL
	[DevExpressXpfChartsLocalizedDescription("MarkerSeries3DActualModel")]
#endif
		public Marker3DModel ActualModel { get { return actualModel; } }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("MarkerSeries3DModel"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public Marker3DModel Model {
			get { return (Marker3DModel)GetValue(ModelProperty); }
			set { SetValue(ModelProperty, value); }
		}
		public MarkerSeries3D() {
			actualModel = new SphereMarker3DModel();
			actualModel.SetOwner(this);
		}
		protected override void AssignForBinding(Series series) {
			base.AssignForBinding(series);
			MarkerSeries3D markerSeries3D = series as MarkerSeries3D;
			if (markerSeries3D != null) {
				if (CopyPropertyValueHelper.IsValueSet(markerSeries3D, ModelProperty))
					if (CopyPropertyValueHelper.VerifyValues(this, markerSeries3D, ModelProperty))
						Model = markerSeries3D.Model.CloneModel();
				if (Label != null && markerSeries3D.Label != null)
					MarkerSeries3D.SetLabelPosition(Label, MarkerSeries3D.GetLabelPosition(markerSeries3D.Label));
			}
		}
	}
}
