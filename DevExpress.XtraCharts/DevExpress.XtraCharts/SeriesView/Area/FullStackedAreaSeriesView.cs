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
using System.ComponentModel.Design.Serialization;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(FullStackedAreaSeriesViewTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class FullStackedAreaSeriesView : StackedAreaSeriesView {
		protected override CustomBorder ActualBorder { get { return null; } }
		protected internal override string StringId { get { return ChartLocalizer.GetString(ChartStringId.SvnFullStackedArea); } }
		protected internal override bool SideMarginsEnabled { get { return false; } }
		protected override Type PointInterfaceType { get { return typeof(IFullStackedPoint); } }
		protected internal override string DefaultPointToolTipPattern {
			get {
				return PatternParser.FullStackedToolTipPattern(GetDefaultArgumentFormat(), GetDefaultFormat(Series.ValueScaleType));				
			}
		}
		protected internal override string DefaultLabelTextPattern {
			get { return "{" + PatternUtils.PercentValuePlaceholder + ":G4}"; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new CustomBorder Border { get { return null; } }
		protected override void CalculateAnnotationAnchorPointLayout(Annotation annotation, XYDiagramAnchorPointLayoutList anchorPointLayoutList, RefinedPointData pointData) {
			AnnotationHelper.CalculateAchorPointLayoutForCenterAreaPoint(annotation, this, anchorPointLayoutList, pointData);
		}		
		protected override ChartElement CreateObjectForClone() {
			return new FullStackedAreaSeriesView();
		}
		protected override SeriesContainer CreateContainer() {
			return new FullStackedInteractionContainer(this, true);
		}
		protected internal override DiagramPoint? CalculateRelativeToolTipPosition(XYDiagramMappingBase mapping, RefinedPointData pointData) {
			return AnnotationHelper.CalculateAchorPointForCenterAreaPointWithoutScrolling(mapping.Container, pointData.RefinedPoint.Argument, GetSeriesPointValues(pointData.RefinedPoint));
		}
		protected internal override ToolTipPointDataToStringConverter CreateToolTipValueToStringConverter() {
			return new ToolTipFullStackedValueToStringConverter(Series);
		}
		protected internal override string[] GetAvailablePointPatternPlaceholders() {
			return ToolTipPatternUtils.PercentViewPointPatterns;
		}
		protected override DrawOptions CreateSeriesDrawOptionsInternal() {
			return new FullStackedAreaDrawOptions(this);
		}
		protected internal override PointOptions CreatePointOptions() {
			return new FullStackedAreaPointOptions();
		}
		protected internal override SeriesLabelBase CreateSeriesLabel() {
			return new FullStackedAreaSeriesLabel();
		}
	}
}
