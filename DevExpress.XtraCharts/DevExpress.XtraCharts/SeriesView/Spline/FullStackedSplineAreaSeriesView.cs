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
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(SplineFullStackedAreaSeriesViewTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class FullStackedSplineAreaSeriesView : FullStackedAreaSeriesView, ISplineSeriesView, IStackedSplineView {
		const int DefaultLineTensionPercent = 80;
		int lineTensionPercent = DefaultLineTensionPercent;
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("FullStackedSplineAreaSeriesViewLineTensionPercent"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.FullStackedSplineAreaSeriesView.LineTensionPercent"),
		XtraSerializableProperty
		]
		public int LineTensionPercent {
			get { return lineTensionPercent; }
			set {
				if (value > 100 || value < 0)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectLineTensionPercent));
				if (lineTensionPercent != value) {
					SendNotification(new ElementWillChangeNotification(this));
					lineTensionPercent = value;
					RaiseControlChanged();
				}
			}
		}
		double LineTension { get { return (double)lineTensionPercent / 100; } }
		protected override CustomBorder ActualBorder { get { return null; } }
		protected internal override string StringId { get { return ChartLocalizer.GetString(ChartStringId.SvnSplineFullStackedArea); } }
		protected internal override string DefaultPointToolTipPattern {
			get {
				return PatternParser.FullStackedToolTipPattern(GetDefaultArgumentFormat(), GetDefaultFormat(Series.ValueScaleType));				
			}
		}
		#region ISplineSeriesView
		bool ISplineSeriesView.ShouldCorrectRanges { get { return false; } }
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if(propertyName == "LineTensionPercent")
				return ShouldSerializeLineTensionPercent();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeLineTensionPercent() {
			return lineTensionPercent != DefaultLineTensionPercent;
		}
		void ResetLineTensionPercent() {
			LineTensionPercent = DefaultLineTensionPercent;
		}
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				ShouldSerializeLineTensionPercent();
		}
		#endregion
		protected override ChartElement CreateObjectForClone() {
			return new FullStackedSplineAreaSeriesView();
		}
		protected override IGeometryStrip CreateStripInternal() {
			return new BezierRangeStrip(LineTension);
		}
		protected override SeriesContainer CreateContainer() {
			return new FullStackedInteractionContainer(this, true);
		}
		protected override GeometryStripCreator CreateStripCreator() {
			return new SplineStackedAreaGeometryStripCreator(LineTension);
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
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			ISplineSeriesView view = obj as ISplineSeriesView;
			if (view == null)
				return;
			lineTensionPercent = view.LineTensionPercent;
		}	 
	}
}
