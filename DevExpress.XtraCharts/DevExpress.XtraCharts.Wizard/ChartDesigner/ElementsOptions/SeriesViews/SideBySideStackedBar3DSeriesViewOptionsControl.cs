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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Design.Serialization;
namespace DevExpress.XtraCharts.Designer.Native {
	[
	ModelBinding(typeof(SideBySideStackedBar3DViewModel)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					 "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public partial class SideBySideStackedBar3DSeriesViewOptionsControl : ElementsOptionsControlBase {
		public SideBySideStackedBar3DSeriesViewOptionsControl() {
			InitializeComponent();
			AddDependence<bool>("ColorEach", "Color", colorEach => !colorEach);
			AddDependence<Bar3DModel>("Model", "ShowFacet", model => model == Bar3DModel.Box || model == Bar3DModel.Cylinder);
			AddDependence<FillMode>("FillStyle.FillMode", "FillStyle.HatchFillOptionsModel.HatchStyle", EditorActivity.Visible, fillMode => fillMode == FillMode.Hatch);
			AddDependence<FillMode>("FillStyle.FillMode", "FillStyle.HatchFillOptionsModel.Color2", EditorActivity.Visible, fillMode => fillMode == FillMode.Hatch);
			AddDependence<FillMode>("FillStyle.FillMode", "FillStyle.RectangleGradientFillOptionsModel.GradientMode", EditorActivity.Visible, fillMode => fillMode == FillMode.Gradient);
			AddDependence<FillMode>("FillStyle.FillMode", "FillStyle.RectangleGradientFillOptionsModel.Color2", EditorActivity.Visible, fillMode => fillMode == FillMode.Gradient);
			AddDependence<FillMode>("FillStyle.FillMode", "FillStyle.PolygonGradientFillOptionsModel.GradientMode", EditorActivity.Visible, fillMode => false);
			AddDependence<FillMode>("FillStyle.FillMode", "FillStyle.PolygonGradientFillOptionsModel.Color2", EditorActivity.Visible, fillMode => false);
		}
	}
}
