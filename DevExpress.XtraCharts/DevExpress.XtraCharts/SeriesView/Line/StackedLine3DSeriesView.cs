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
	TypeConverter(typeof(StackedLine3DSeriesViewTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class StackedLine3DSeriesView : Line3DSeriesView, IStackedView {
		protected override bool NeedSeriesInteraction { get { return true; } }
		protected internal override bool DateTimeValuesSupported { get { return false; } }
		protected internal override string StringId { get { return ChartLocalizer.GetString(ChartStringId.SvnStackedLine3D); } }
		protected override Type PointInterfaceType { get { return typeof(IStackedPoint); } }
		public StackedLine3DSeriesView() : base() {
		}
		protected internal override MinMaxValues GetSeriesPointValues(RefinedPoint refinedPoint) {
			return new MinMaxValues(((IStackedPoint)refinedPoint).MinValue, ((IStackedPoint)refinedPoint).MaxValue);
		}
		protected internal override SeriesLabelBase CreateSeriesLabel() {
			return new StackedLine3DSeriesLabel();
		}
		protected override ChartElement CreateObjectForClone() {
			return new StackedLine3DSeriesView();
		}
		protected override SeriesContainer CreateContainer() {
			return new StackedInteractionContainer(this, true);
		}
		protected override GeometryStripCreator CreateStripCreator() {
			return new StackedLineGeometryStripCreator();
		}
	}
}
