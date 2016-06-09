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
using System.Windows;
using System.Windows.Media;
using DevExpress.Charts.Native;
using DevExpress.Charts.NotificationCenter;
namespace DevExpress.Xpf.Charts {
	public interface ISupportTransparency {
		double Transparency { get; set; }
	}
	public interface ISupportStackedGroup {
		object StackedGroup { get; set; }
	}
	public interface ISupportMarker2D {
		Marker2DModel MarkerModel { get; set; }
		int MarkerSize { get; set; }
		bool MarkerVisible { get; set; }
	}
	public interface ILegendVisible {
		DataTemplate LegendMarkerTemplate { get; }
		bool CheckedInLegend { get; }
		bool CheckableInLegend { get; }
	}
	public interface IDockTarget {
		Rect GetBounds();
	}
}
namespace DevExpress.Xpf.Charts.Native {
	public interface IAxisLabelFormatter : IAxisLabelFormatterCore { }
	public interface IDataLabelFormatter {
		string GetDataLabelText(ISeriesPoint point);
	}
	public interface IUnwindAnimation {
		Rect CreateAnimatedClipBounds(Rect clipBounds, double progress);
	}
	public interface IOwnedElement {
		IChartElement Owner { get; set; }
	}
	public interface INotificationOwner {
		ViewController Controller { get; }
	}
	public interface IChartElement : IOwnedElement, INotificationOwner {
		void AddChild(object child);
		void RemoveChild(object child);
		bool Changed(ChartUpdate updateInfo);		
	}
	public interface ISupportFlowDirection {
		Transform CreateDirectionTransform();
	}
	public interface ISupportNegativeFlipping {
		bool ShouldFlip { get; }
	}
	public interface ISeriesItem {
		DrawOptions DrawOptions { get; set; }
		string LegendText { get; set; }
		bool ShouldUpdate { get; }
		bool HasPoints { get; }
		IEnumerable<ISeriesPointData> PointData { get; }
		void Update(IRefinedSeries refinedSeries);
	}
	public interface ISeriesPointData {
		Color Color { get; set; }
		string LegendText { get; set; }
		string[] LabelsTexts { get; set; }
		double Opacity { get; set; }
		LegendItem LegendItem { get; set; }
		RefinedPoint RefinedPoint { get; }
		ISeriesPoint SeriesPoint { get; }
		int IndexInSeries { get; set; }
	}
	public interface IAnimatableElement {
		ChartAnimationMode AnimationMode { get; }
		AnimationState AnimationState { get; }
		AnimationAutoStartMode AnimationAutoStartMode { get; }
		void ProgressChanged(AnimationProgress source);
	}
	public interface IHitTestableElement {
		object Element { get; }
		object AdditionalElement { get; }
	}
	public interface IFinishInvalidation {
	}
	public interface ISupportSeriesBorder {
		SeriesBorder Border { get; }
		SeriesBorder ActualBorder { get; }
	}
	public interface ILineSeries {
		int LineThickness { get; }
		LineStyle LineStyle { get; }
	}
	public interface IMapping {
		Point GetDiagramPoint(double argument, double value);
		Point GetRoundedDiagramPoint(double argument, double value);
		bool IsLabelVisibleForResolveOverlapping(Point initialAnchorPoint);
		bool Rotated { get; }
		bool NavigationEnabled { get; }
		Rect Viewport { get; }
	}
	public interface IWizardDataProvider {
		Type PointInterfaceType { get; }
	}
	public interface IInteractiveElement {
		bool IsHighlighted { get; set; }
		bool IsSelected { get; set; }
		object Content { get; }
	}
}
