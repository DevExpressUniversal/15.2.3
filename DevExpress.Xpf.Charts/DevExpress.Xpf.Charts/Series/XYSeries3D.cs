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
using System.Windows;
using System.Windows.Media.Media3D;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
using System.Windows.Media;
using System;
namespace DevExpress.Xpf.Charts {
	public abstract class XYSeries3D : XYSeries, IXYSeriesView {
		#region IXYSeriesView
		IAxisData IXYSeriesView.AxisXData {
			get {
				IXYDiagram xyDiagram = Diagram as IXYDiagram;
				return xyDiagram == null ? null : (IAxisData)xyDiagram.AxisX;
			}
		}
		IAxisData IXYSeriesView.AxisYData {
			get {
				IXYDiagram xyDiagram = Diagram as IXYDiagram;
				return xyDiagram == null ? null : (IAxisData)xyDiagram.AxisY;
			}
		}
		ToolTipPointDataToStringConverter IXYSeriesView.CrosshairConverter { get { return null; } }
		string IXYSeriesView.CrosshairLabelPattern { get { return string.Empty; } }
		IPane IXYSeriesView.Pane { get { return ActualPane; } }
		bool IXYSeriesView.SideMarginsEnabled { get { return true; } }
		int IXYSeriesView.PixelsPerArgument { get { return 40; } }
		bool IXYSeriesView.CrosshairEnabled { get { return false; } }
		IEnumerable<double> IXYSeriesView.GetCrosshairValues(RefinedPoint refinedPoint) {
			return null;
		}
		List<ISeparatePaneIndicator> IXYSeriesView.GetSeparatePaneIndicators() {
			return new List<ISeparatePaneIndicator>();
		}
		List<IAffectsAxisRange> IXYSeriesView.GetIndicatorsAffectRange() {
			return new List<IAffectsAxisRange>();
		}
		#endregion
		protected override Type PointInterfaceType {
			get {
				return typeof(IXYPoint);
			}
		}
		protected override bool Is3DView { get { return true; } }
		protected internal virtual bool IsPredefinedModelUses { get { return true; } }
		protected internal override VisualSelectionType SupportedSelectionType { get { return VisualSelectionType.Brightness; } }
		protected override void FillPredefinedPointAnimationKinds(List<AnimationKind> animationKinds) {
		}
		protected override void FillPredefinedSeriesAnimationKinds(List<AnimationKind> animationKinds) {
		}
		protected internal override void ChangeSeriesPointSelection(SeriesPoint seriesPoint, bool isSelected) {
			XYDiagram3D diagram3D = Diagram as XYDiagram3D;
			if (diagram3D != null) {
				Cache.UpdateSelectionForPointCache(seriesPoint, isSelected, false);
				SeriesPointCache pointCache = Cache.GetSeriesPointCache(seriesPoint);
				if (pointCache != null) {
					Color selectedColor = IsPredefinedModelUses ? GetSeriesPointColor(pointCache.RefinedPoint) : VisualSelectionHelper.CustomModelSelectionColor;
					diagram3D.ChangeItemBrush(seriesPoint, selectedColor);
				}
			}
		}
		protected override void OnIsSelectedChanged(bool isSelected) {
			base.OnIsSelectedChanged(isSelected);
			XYDiagram3D diagram3D = Diagram as XYDiagram3D;
			if (diagram3D != null) {
				Color selectedColor = IsPredefinedModelUses ? Cache.DrawOptions.ActualColor : VisualSelectionHelper.CustomModelSelectionColor;
				diagram3D.ChangeItemBrush(this, selectedColor);
			}
		}
		protected override PatternDataProvider GetDataProvider(PatternConstants patternConstant) {
			return GetXYDataProvider(patternConstant);
		}
		internal Point CalculateToolTipPoint(SeriesPoint point) {
			Point location = new Point();
			Diagram3D diagram3D = Diagram as Diagram3D;
			SeriesPointCache cache = Cache.GetSeriesPointCache(point);
			if (cache != null && diagram3D != null && diagram3D.ChartControl != null) {
				VisualContainer container = diagram3D.VisualContainers[0];
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
