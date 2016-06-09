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
using System.Collections.Generic;
namespace DevExpress.Xpf.Charts {
	public sealed class SimpleDiagram3D : Diagram3D, ISimpleDiagram, IHitTestableElement {
		public static readonly DependencyProperty DimensionProperty;
		public static readonly DependencyProperty LayoutDirectionProperty;
		static SimpleDiagram3D() {
			Type ownerType = typeof(SimpleDiagram3D);
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
			DimensionProperty = DependencyProperty.Register("Dimension", typeof(int), ownerType,
				new FrameworkPropertyMetadata(3, ChartElementHelper.Update), new ValidateValueCallback(DimensionValidation));
			LayoutDirectionProperty = DependencyProperty.Register("LayoutDirection", typeof(LayoutDirection), ownerType,
				new FrameworkPropertyMetadata(LayoutDirection.Horizontal, ChartElementHelper.Update));
			PerspectiveAngleProperty.OverrideMetadata(ownerType,
				new FrameworkPropertyMetadata(20.0, FrameworkPropertyMetadataOptions.AffectsRender, ChartElementHelper.Update));
		}
		static bool DimensionValidation(object dimension) {
			return (int)dimension > 0 && (int)dimension < 100;
		}
		protected internal override CompatibleViewType CompatibleViewType {
			get { return CompatibleViewType.SimpleView; }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SimpleDiagram3DDimension"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty]
		public int Dimension {
			get { return (int)GetValue(DimensionProperty); }
			set { SetValue(DimensionProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SimpleDiagram3DLayoutDirection"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty]
		public LayoutDirection LayoutDirection {
			get { return (LayoutDirection)GetValue(LayoutDirectionProperty); }
			set { SetValue(LayoutDirectionProperty, value); }
		}
		public SimpleDiagram3D() {
			EndInit();
		}
		#region ISimpleDiagram Implementation
		SimpleDiagramLayoutDirection ISimpleDiagram.LayoutDirection {
			get { return (SimpleDiagramLayoutDirection)LayoutDirection; }
		}
		#endregion
		#region IHitTestableElement implementation
		Object IHitTestableElement.Element { get { return this; } }
		Object IHitTestableElement.AdditionalElement { get { return null; } }
		#endregion
		protected override Transform3D CreateDefaultContentTransform() {
			return new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), -60));
		}
		protected override DiagramLightingCollection CreateDefaultLighting() {
			Color ambientColor = new Color();
			ambientColor.R = ambientColor.G = ambientColor.B = 130;
			DiagramLightingCollection collection = new DiagramLightingCollection();
			collection.Add(new AmbientLight(ambientColor));
			collection.Add(new DirectionalLight(Colors.White, new Vector3D(1, -1.5, -2)));
			return collection;
		}
		protected internal override Diagram3DDomain CreateDomain(VisualContainer visualContainer) {
			if (visualContainer.Bounds.Height > 0 && visualContainer.Bounds.Width > 0) {
				int containerIndex = VisualContainers.IndexOf(visualContainer);
				if (containerIndex >= 0 && containerIndex < Series.Count) {
					PieSeries3D series = Series[containerIndex] as PieSeries3D;
					if (series == null) {
						ChartDebug.Fail("PieSeries3D is expected");
						return null;
					}
					if (series.GetActualVisible())
						return new SimpleDiagram3DDomain(this, visualContainer.Bounds, series);
				}
			}
			return null;
		}
		protected internal override List<VisibilityLayoutRegion> GetElementsForAutoLayout(Size size) {
			List<VisibilityLayoutRegion> models = new List<VisibilityLayoutRegion>();
			if (Series.Count == 0)
				return models;
			List<GRect2D> elementsBounds = SimpleDiagramLayout.Calculate(this, new GRect2D(0, 0, (int)size.Width, (int)size.Height), Series.Count);
			elementsBounds = CorrectElementsBounds(elementsBounds);
			for (int i = 0; i < elementsBounds.Count; i++) {
				Series series = Series[i];
				Size seriesSize = elementsBounds[i].ToRect().Size;
				models.Add(new VisibilityLayoutRegion(new GRealSize2D(seriesSize.Width, seriesSize.Height), GetElementsForAutoLayout(series)));
			}
			return models;
		}
		List<GRect2D> CorrectElementsBounds(List<GRect2D> elementsBounds) {
			elementsBounds.Sort(new RectComparer());
			GRect2D previous = elementsBounds[0];
			for (int i = 1; i < elementsBounds.Count; i++) {
				if (InFrame(previous, elementsBounds[i]))
					elementsBounds[i] = previous;
				else
					previous = elementsBounds[i];
			}
			return elementsBounds;
		}
		bool InFrame(GRect2D pattern, GRect2D rect) {
			return rect.Height < pattern.Height * 1.05 && rect.Height > pattern.Height * 0.95 &&
				rect.Width < pattern.Width * 1.05 && rect.Width > pattern.Width * 0.95;
		}
		List<ISupportVisibilityControlElement> GetElementsForAutoLayout(Series series) {
			List<ISupportVisibilityControlElement> elements = new List<ISupportVisibilityControlElement>();
			TitleCollection titles = null;
			FunnelSeries2D funnelSeries2D = series as FunnelSeries2D;
			if (funnelSeries2D != null)
				titles = funnelSeries2D.Titles;
			PieSeries pieSeries = series as PieSeries;
			if (pieSeries != null)
				titles = pieSeries.Titles;
			if (titles != null)
				foreach (Title title in titles)
					if (!title.Visible.HasValue)
						elements.Add(title);
			return elements;
		}
		protected override Size MeasureOverride(Size constraint) {
			if (ChartControl == null || ChartControl.ActualAutoLayout) {
				this.Dimension = SimpleDiagramAutoLayoutHelper.CalculateDimension((int)constraint.Width, (int)constraint.Height, this.Series.Count);
				this.LayoutDirection = Charts.LayoutDirection.Horizontal;
			}
			return base.MeasureOverride(constraint);
		}
	}
}
