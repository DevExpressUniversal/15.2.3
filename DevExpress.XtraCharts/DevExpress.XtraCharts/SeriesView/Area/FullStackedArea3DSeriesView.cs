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

using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using System;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(FullStackedArea3DSeriesViewTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class FullStackedArea3DSeriesView : StackedArea3DSeriesView {
		protected internal override string StringId { get { return ChartLocalizer.GetString(ChartStringId.SvnFullStackedArea3D); } }
		protected internal override bool SideMarginsEnabled { get { return false; } }
		protected override Type PointInterfaceType { get { return typeof(IFullStackedPoint); } }
		protected internal override string DefaultLabelTextPattern {
			get { return "{" + PatternUtils.PercentValuePlaceholder + ":G4}"; }
		}
		public FullStackedArea3DSeriesView() : base() {
		}
		protected override ChartElement CreateObjectForClone() {
			return new FullStackedAreaSeriesView();
		}
		protected override double GetAnnotationValue(MinMaxValues values, IAxisRangeData axisRangeY) {
			return MinMaxValues.Intersection(values, axisRangeY).CalculateCenter();
		}
		protected override SeriesContainer CreateContainer() {
			return new FullStackedInteractionContainer(this, true);
		}
		protected internal override PointOptions CreatePointOptions() {
			return new FullStackedAreaPointOptions();
		}
		protected internal override SeriesLabelBase CreateSeriesLabel() {
			return new FullStackedArea3DSeriesLabel();
		}
	}
}
