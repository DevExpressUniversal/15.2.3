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
using DevExpress.Utils;
namespace DevExpress.XtraCharts.Designer.Native {
	[
	ModelBinding(typeof(XYDiagramPaneBaseModel)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					 "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public partial class PaneOptionsControl : ElementsOptionsControlBase {
		new XYDiagramPaneBaseModel Model { get { return (XYDiagramPaneBaseModel)base.Model; } }
		public PaneOptionsControl() {
			InitializeComponent();
			AddDependence<System.Boolean>("BorderVisible", "BorderColor", visible => visible);
			AddDependence<System.Boolean>("ScrollBarOptions.XAxisScrollBarVisible", "ScrollBarOptions.XAxisScrollBarAlignment", (enable) => { return Model.ActualEnableAxisXScrolling && Model.ScrollBarOptions.XAxisScrollBarVisible; });
			AddDependence<System.Boolean>("ScrollBarOptions.YAxisScrollBarVisible", "ScrollBarOptions.YAxisScrollBarAlignment", (enable) => { return Model.ActualEnableAxisYScrolling && Model.ScrollBarOptions.YAxisScrollBarVisible; });
			AddDependence<DefaultBoolean>("EnableAxisXScrolling", "ScrollBarOptions.XAxisScrollBarVisible", (enable) => { return Model.ActualEnableAxisXScrolling; });
			AddDependence<DefaultBoolean>("EnableAxisYScrolling", "ScrollBarOptions.YAxisScrollBarVisible", (enable) => { return Model.ActualEnableAxisYScrolling; });
			AddDependence<DefaultBoolean>("EnableAxisXScrolling", "ScrollBarOptions.XAxisScrollBarAlignment", (enable) => { return Model.ActualEnableAxisXScrolling && Model.ScrollBarOptions.XAxisScrollBarVisible; });
			AddDependence<DefaultBoolean>("EnableAxisYScrolling", "ScrollBarOptions.YAxisScrollBarAlignment", (enable) => { return Model.ActualEnableAxisYScrolling && Model.ScrollBarOptions.YAxisScrollBarVisible; });
		}
	}
}
