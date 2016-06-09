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
using DevExpress.Utils;
namespace DevExpress.XtraCharts.Designer.Native {
	[
	ModelBinding(typeof(PointSeriesLabelModel)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					 "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public partial class PointSeriesLabelOptionsControl : ElementsOptionsControlBase {
		new PointSeriesLabelModel Model { get { return (PointSeriesLabelModel)base.Model; } }
		public PointSeriesLabelOptionsControl() {
			InitializeComponent();
			AddDependence<DefaultBoolean>("Border.Visibility", "Border.Color", visibility => visibility != DefaultBoolean.False && Model.LabelsVisibility == DefaultBoolean.True);
			AddDependence<DefaultBoolean>("Border.Visibility", "Border.Thickness", visibility => visibility != DefaultBoolean.False && Model.LabelsVisibility == DefaultBoolean.True);
			AddDependence<DefaultBoolean>("LabelsVisibility", "Position", visibility => visibility == DefaultBoolean.True);
			AddDependence<DefaultBoolean>("LabelsVisibility", "TextColor", visibility => visibility == DefaultBoolean.True);
			AddDependence<DefaultBoolean>("LabelsVisibility", "BackColor", visibility => visibility == DefaultBoolean.True);
			AddDependence<DefaultBoolean>("LabelsVisibility", "Border.Visibility", visibility => visibility == DefaultBoolean.True);
			AddDependence<DefaultBoolean>("LabelsVisibility", "Border.Color", visibility => visibility == DefaultBoolean.True && Model.Border.Visibility != DefaultBoolean.False);
			AddDependence<DefaultBoolean>("LabelsVisibility", "Border.Thickness", visibility => visibility == DefaultBoolean.True && Model.Border.Visibility != DefaultBoolean.False);
			AddDependence<DefaultBoolean>("LabelsVisibility", "FillStyle.FillMode", visibility => visibility == DefaultBoolean.True);
			AddDependence<DefaultBoolean>("LabelsVisibility", "LineVisibility", visibility => visibility == DefaultBoolean.True);
			AddDependence<DefaultBoolean>("LabelsVisibility", "TextPattern", visibility => visibility == DefaultBoolean.True);
			AddDependence<DefaultBoolean>("LabelsVisibility", "TextOrientation", visibility => visibility == DefaultBoolean.True);
			AddDependence<DefaultBoolean>("LabelsVisibility", "TextAlignment", visibility => visibility == DefaultBoolean.True);
			AddDependence<DefaultBoolean>("LabelsVisibility", "LineLength", visibility => visibility == DefaultBoolean.True);
			AddDependence<FillMode>("FillStyle.FillMode", "FillStyle.HatchFillOptionsModel.HatchStyle", EditorActivity.Visible, fillMode => fillMode == FillMode.Hatch);
			AddDependence<FillMode>("FillStyle.FillMode", "FillStyle.HatchFillOptionsModel.Color2", EditorActivity.Visible, fillMode => fillMode == FillMode.Hatch);
			AddDependence<FillMode>("FillStyle.FillMode", "FillStyle.RectangleGradientFillOptionsModel.GradientMode", EditorActivity.Visible, fillMode => fillMode == FillMode.Gradient);
			AddDependence<FillMode>("FillStyle.FillMode", "FillStyle.RectangleGradientFillOptionsModel.Color2", EditorActivity.Visible, fillMode => fillMode == FillMode.Gradient);
			AddDependence<FillMode>("FillStyle.FillMode", "FillStyle.PolygonGradientFillOptionsModel.GradientMode", EditorActivity.Visible, fillMode => false);
			AddDependence<FillMode>("FillStyle.FillMode", "FillStyle.PolygonGradientFillOptionsModel.Color2", EditorActivity.Visible, fillMode => false);
		}
	}
}
