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
using System.Linq;
using System.Windows.Media;
using DevExpress.Xpf.Diagram;
using DevExpress.Xpf.Reports.UserDesigner.XRDiagram;
using DevExpress.XtraCharts;
using DevExpress.XtraReports.UI;
namespace DevExpress.Xpf.Reports.UserDesigner.ReportModel {
	public partial class XRControlModelBase<TXRObject, TDiagramItem> : XRControlModelBase
		where TXRObject : XRControl
		where TDiagramItem : DiagramItem, new() {
		protected internal XRControlModelBase(XRControlModelFactory.ISource<TXRObject> source, ImageSource icon) : base(source, icon) { }
		public new TXRObject XRObject { get { return (TXRObject)base.XRObject; } }
		public new TDiagramItem DiagramItem { get { return (TDiagramItem)base.DiagramItem; } }
		protected override DiagramItem CreateDiagramItem() {
			return new TDiagramItem();
		}
	}
	public partial class XRChartElementModelBase<TXRObject, TDiagramItem> : XRChartElementModelBase
		where TXRObject : ChartElement
		where TDiagramItem : XRDiagramLogicItem, new() {
		protected internal XRChartElementModelBase(XRChartElementModelFactory.ISource<TXRObject> source) : base(source) { }
		public new TXRObject XRObject { get { return (TXRObject)base.XRObject; } }
		public new TDiagramItem DiagramItem { get { return (TDiagramItem)base.DiagramItem; } }
		protected override DiagramItem CreateDiagramItem() {
			return new TDiagramItem();
		}
	}
	public partial class XRCrossBandControlModelBase<TXRObject, TDiagramItem> : XRCrossBandControlModelBase
		where TXRObject : XRCrossBandControl
		where TDiagramItem : XRCrossBandControlDiagramItem, new() {
		protected internal XRCrossBandControlModelBase(XRControlModelFactory.ISource<TXRObject> source, ImageSource icon) : base(source, icon) { }
		public new TXRObject XRObject { get { return (TXRObject)base.XRObject; } }
		public new TDiagramItem DiagramItem { get { return (TDiagramItem)base.DiagramItem; } }
		protected override DiagramItem CreateDiagramItem() {
			return new TDiagramItem();
		}
	}
	public partial class XRComponentModelBase<TXRObject, TDiagramItem> : XRComponentModelBase
		where TXRObject : IComponent
		where TDiagramItem : XRDiagramLogicItem, new() {
		protected internal XRComponentModelBase(XRControlModelFactory.ISource<TXRObject> source, ImageSource icon) : base((XRControlModelFactory.ISource<IComponent>)source, icon) { }
		public new TXRObject XRObject { get { return (TXRObject)base.XRObject; } }
		public new TDiagramItem DiagramItem { get { return (TDiagramItem)base.DiagramItem; } }
		protected override DiagramItem CreateDiagramItem() {
			return new TDiagramItem();
		}
	}
}
