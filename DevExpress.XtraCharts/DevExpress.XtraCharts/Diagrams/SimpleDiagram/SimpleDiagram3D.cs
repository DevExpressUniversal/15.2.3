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
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Collections.Generic;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(SimpleDiagram3DTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class SimpleDiagram3D : Diagram3D, ISimpleDiagram {
		const LayoutDirection DefaultLayoutDirection = LayoutDirection.Horizontal;
		const int DefaultDimension = 3;
		LayoutDirection layoutDirection = DefaultLayoutDirection;
		int dimension = DefaultDimension;
		protected override int DefaultRotationAngleX { get { return -60; } }
		protected override int DefaultRotationAngleY { get { return 0; } }
		protected override int DefaultRotationAngleZ { get { return 0; } }
		protected override int DefaultPerspectiveAngle { get { return 20; } }
		protected internal override bool DependsOnBounds { get { return Chart.AutoLayout; } }
		SimpleDiagramLayoutDirection ISimpleDiagram.LayoutDirection { get { return (SimpleDiagramLayoutDirection)layoutDirection; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SimpleDiagram3DLayoutDirection"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SimpleDiagram3D.LayoutDirection"),
		Category("Behavior"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public LayoutDirection LayoutDirection {
			get { return layoutDirection; }
			set {
				if (value != layoutDirection) {
					SendNotification(new ElementWillChangeNotification(this));
					layoutDirection = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SimpleDiagram3DDimension"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SimpleDiagram3D.Dimension"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public int Dimension {
			get { return dimension; }
			set {
				if (value != dimension) {
					if (value < SimpleDiagramAutoLayoutHelper.MinDimension || value > SimpleDiagramAutoLayoutHelper.MaxDimension)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectSimpleDiagramDimension));
					SendNotification(new ElementWillChangeNotification(this));
					dimension = value;
					RaiseControlChanged();
				}
			}
		}
		public SimpleDiagram3D() : base() { }
		#region ShouldSerialize & Reset
		bool ShouldSerializeLayoutDirection() {
			return layoutDirection != DefaultLayoutDirection;
		}
		void ResetLayoutDirection() {
			LayoutDirection = DefaultLayoutDirection;
		}
		bool ShouldSerializeDimension() {
			return dimension != DefaultDimension;
		}
		void ResetDimension() {
			Dimension = DefaultDimension;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeLayoutDirection() || ShouldSerializeDimension();
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "LayoutDirection":
					return ShouldSerializeLayoutDirection();
				case "Dimension":
					return ShouldSerializeDimension();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		List<IRefinedSeries> GetSeries() {
			List<IRefinedSeries> seriesList = new List<IRefinedSeries>();
			foreach (IRefinedSeries refinedSeries in ViewController.RefinedSeriesForLegend) {
				SeriesBase series = (SeriesBase)refinedSeries.Series;
				if (refinedSeries.Series.ShouldBeDrawnOnDiagram && refinedSeries.Points.Count > 0)
					seriesList.Add(refinedSeries);
			}
			return seriesList;
		}
		protected override ChartElement CreateObjectForClone() {
			return new SimpleDiagram3D();
		}
		protected internal override void UpdateAutomaticLayout(Rectangle bounds) {
			List<IRefinedSeries> series = GetSeries();
			int actualDimension = SimpleDiagramAutoLayoutHelper.CalculateDimension(bounds.Width, bounds.Height, series.Count);
			CustomizeSimpleDiagramLayoutEventArgs e = new CustomizeSimpleDiagramLayoutEventArgs(actualDimension, LayoutDirection.Horizontal);
			Chart.ContainerAdapter.OnCustomizeSimpleDiagramLayout(e);
			dimension = e.Dimension;
			layoutDirection = e.LayoutDirection;
		}
		protected internal override bool Contains(object obj) {
			return false;
		}
		protected internal override DiagramViewData CalculateViewData(TextMeasurer textMeasurer, Rectangle diagramBounds, IList<RefinedSeriesData> seriesDataList, bool performRangeCorrection) {
			UpdateLastDiagramBounds(diagramBounds);
			if (!diagramBounds.AreWidthAndHeightPositive())
				return null;
			List<SeriesLayout> seriesLayoutList = new List<SeriesLayout>();
			List<SeriesLabelLayoutList> labelLayoutLists = new List<SeriesLabelLayoutList>();
			int seriesCount = seriesDataList.Count;
			IList<GRect2D> domainBounds;
			if (seriesCount > 0)
				domainBounds = SimpleDiagramLayout.Calculate(this, GraphicUtils.ConvertRect(diagramBounds), seriesCount);
			else {
				domainBounds = new List<GRect2D>();
				domainBounds.Add(GraphicUtils.ConvertRect(diagramBounds));
			}
			List<AnnotationLayout> annotationsAnchorPointsLayout = new List<AnnotationLayout>();
			for (int i = 0; i < seriesCount; i++) {
				SimpleDiagram3DSeriesLayout seriesLayout = new SimpleDiagram3DSeriesLayout(seriesDataList[i], textMeasurer, domainBounds[i]);
				if (seriesLayout.Domain != null && seriesLayout.Domain.Bounds.AreWidthAndHeightPositive()) {
					seriesLayoutList.Add(seriesLayout);
					labelLayoutLists.Add(seriesLayout.LabelLayoutList);
					annotationsAnchorPointsLayout.AddRange(seriesLayout.AnnotationsAnchorPointsLayout);
				}
			}
			return new SimpleDiagram3DViewData(this, diagramBounds, seriesLayoutList, labelLayoutLists, annotationsAnchorPointsLayout);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			SimpleDiagram3D diagram = obj as SimpleDiagram3D;
			if (diagram != null) {
				layoutDirection = diagram.layoutDirection;
				dimension = diagram.dimension;
			}
		}
	}
}
