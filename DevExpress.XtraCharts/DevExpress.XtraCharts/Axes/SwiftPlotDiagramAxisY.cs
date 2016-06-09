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

using System.ComponentModel.Design.Serialization;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using System.ComponentModel;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts {
	public abstract class SwiftPlotDiagramAxisYBase : SwiftPlotDiagramAxis {
		protected override int DefaultMinorCount { get { return Constants.AxisYDefaultMinorCount; } }
		protected override int GridSpacingFactor { get { return Constants.AxisYGridSpacingFactor; } }
		protected internal override bool IsValuesAxis { get { return true; } }
		protected internal override bool IsVertical { get { return true; } }
		protected SwiftPlotDiagramAxisYBase(string name, SwiftPlotDiagram diagram) : base(name, diagram) { }
		protected override Tickmarks CreateTickmarks() {
			return new TickmarksY(this);
		}
		protected override AxisTitle CreateAxisTitle() {
			return new AxisTitleY(this);
		}
		protected override AxisRange CreateAxisRange(RangeDataBase wholeAxisRange, RangeDataBase visibleAxisRange) {				
			return new AxisYRange(this);
		}
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class SwiftPlotDiagramAxisY : SwiftPlotDiagramAxisYBase {
		static readonly string defaultName = ChartLocalizer.GetString(ChartStringId.PrimaryAxisYName);
		protected override AxisAlignment DefaultAlignment { get { return AxisAlignment.Near; } }
		protected override ChartElementVisibilityPriority Priority { get { return ChartElementVisibilityPriority.AxisY; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),	
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new string Name { 
			get { return base.Name; } 
			set { base.Name = value; }
		}
		internal SwiftPlotDiagramAxisY(SwiftPlotDiagram diagram) : base(defaultName, diagram) {
		}
		protected override ChartElement CreateObjectForClone() {
			return new SwiftPlotDiagramAxisY(null);
		}
		protected override GridLines CreateGridLines() {
			return new GridLinesY(this);
		}
	}
}
