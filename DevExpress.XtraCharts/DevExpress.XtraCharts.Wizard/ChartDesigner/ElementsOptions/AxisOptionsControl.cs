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

using DevExpress.Charts.Native;
using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.XtraCharts.Designer.Native {
	[
	ModelBinding(typeof(AxisXModel)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					 "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public partial class AxisOptionsControl : ElementsOptionsControlBase {
		public AxisOptionsControl() {
			InitializeComponent();
			AddDependence<bool>("VisualRange.Auto", "VisualRange.MinValue", (x) => { return !x; });
			AddDependence<bool>("VisualRange.Auto", "VisualRange.MaxValue", (x) => { return !x; });
			AddDependence<bool>("WholeRange.Auto", "WholeRange.MinValue", (x) => { return !x; });
			AddDependence<bool>("WholeRange.Auto", "WholeRange.MaxValue", (x) => { return !x; });
			AddDependence<bool>("Logarithmic", "LogarithmicBase", (x) => { return x; });
			AddDependence<ActualScaleType>("ScaleType", "Logarithmic", EditorActivity.Visible, (x) => { return x == ActualScaleType.Numerical; });
			AddDependence<ActualScaleType>("ScaleType", "LogarithmicBase", EditorActivity.Visible, (x) => { return x == ActualScaleType.Numerical; }); 
			AddDependence<DefaultBoolean>("Title.Visibility", "Title.Text", (x) => { return x == DefaultBoolean.True || x == DefaultBoolean.Default; });
		}
	}
}
