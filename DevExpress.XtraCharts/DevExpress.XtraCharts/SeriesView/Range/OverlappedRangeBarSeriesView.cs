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
using System.Globalization;
using System.Drawing;
using System.Reflection;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Design;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(OverlappedRangeBarSeriesViewTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class OverlappedRangeBarSeriesView : RangeBarSeriesView {
		protected internal override string StringId { get { return ChartLocalizer.GetString(ChartStringId.SvnOverlappedRangeBar); } }
		protected override Type PointInterfaceType { get { return typeof(IRangePoint); } }
		public OverlappedRangeBarSeriesView()
			: base() {
		}
		protected override ChartElement CreateObjectForClone() {
			return new SideBySideRangeBarSeriesView();
		}
		protected override double GetRefinedPointMax(RefinedPoint point) {
			IRangePoint rangePoint = (IRangePoint)point;
			return Math.Max(rangePoint.Value1, rangePoint.Value2);
		}
		protected override double GetRefinedPointMin(RefinedPoint point) {
			IRangePoint rangePoint = (IRangePoint)point;
			return Math.Min(rangePoint.Value1, rangePoint.Value2);
		}
		protected override double GetRefinedPointAbsMin(RefinedPoint point) {
			IRangePoint rangePoint = (IRangePoint)point;
			return Math.Min(Math.Abs(rangePoint.Value1), Math.Abs(rangePoint.Value2));
		}
	}
}
