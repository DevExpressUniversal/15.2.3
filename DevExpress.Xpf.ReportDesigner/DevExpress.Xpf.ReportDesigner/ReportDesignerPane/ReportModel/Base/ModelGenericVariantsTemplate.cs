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
using System.Windows.Media;
using DevExpress.Xpf.Diagram;
using DevExpress.Xpf.Reports.UserDesigner.XRDiagram;
using DevExpress.XtraCharts;
using DevExpress.XtraReports.UI;
namespace DevExpress.Xpf.Reports.UserDesigner.ReportModel {
	public static class XRControlModel {
		public static XRControlModelBase<TXRObject> New<TXRObject>(XRControlModelFactory.ISource<TXRObject> source, ImageSource icon)
			where TXRObject : XRControl {
			return new XRControlModelBase<TXRObject>(source, icon);
		}
	}
	public static class XRControlModel<TDiagramItem>
		where TDiagramItem : DiagramItem, new() {
		public static XRControlModelBase<TXRObject, TDiagramItem> New<TXRObject>(XRControlModelFactory.ISource<TXRObject> source, ImageSource icon)
			where TXRObject : XRControl {
			return new XRControlModelBase<TXRObject, TDiagramItem>(source, icon);
		}
	}
	public class XRControlModelBase<TXRObject> : XRControlModelBase<TXRObject, XRDiagramItem>
		where TXRObject : XRControl {
		protected internal XRControlModelBase(XRControlModelFactory.ISource<TXRObject> source, ImageSource icon) : base(source, icon) { }
	}
}
namespace DevExpress.Xpf.Reports.UserDesigner.ReportModel {
	public static class XRContainerModel<TItemModel>
		where TItemModel : XRControlModelBase {
		public static XRContainerModelBase<TXRObject, TItemModel> New<TXRObject>(XRControlModelFactory.ISource<TXRObject> source, ImageSource icon)
			where TXRObject : XRControl {
			return new XRContainerModelBase<TXRObject, TItemModel>(source, icon);
		}
	}
	public static class XRContainerModel<TItemModel, TDiagramItem>
		where TItemModel : XRControlModelBase
		where TDiagramItem : DiagramContainerBase, new() {
		public static XRContainerModelBase<TXRObject, TItemModel, TDiagramItem> New<TXRObject>(XRControlModelFactory.ISource<TXRObject> source, ImageSource icon)
			where TXRObject : XRControl {
			return new XRContainerModelBase<TXRObject, TItemModel, TDiagramItem>(source, icon);
		}
	}
	public class XRContainerModelBase<TXRObject, TItemModel> : XRContainerModelBase<TXRObject, TItemModel, XRDiagramContainer>
		where TXRObject : XRControl
		where TItemModel : XRControlModelBase {
		protected internal XRContainerModelBase(XRControlModelFactory.ISource<TXRObject> source, ImageSource icon) : base(source, icon) { }
	}
}
namespace DevExpress.Xpf.Reports.UserDesigner.ReportModel {
	public static class XRTextModel {
		public static XRTextModelBase<TXRObject> New<TXRObject>(XRControlModelFactory.ISource<TXRObject> source, ImageSource icon)
			where TXRObject : XRControl {
			return new XRTextModelBase<TXRObject>(source, icon);
		}
	}
	public static class XRTextModel<TDiagramItem>
		where TDiagramItem : XRDiagramTextItem, new() {
		public static XRTextModelBase<TXRObject, TDiagramItem> New<TXRObject>(XRControlModelFactory.ISource<TXRObject> source, ImageSource icon)
			where TXRObject : XRControl {
			return new XRTextModelBase<TXRObject, TDiagramItem>(source, icon);
		}
	}
	public class XRTextModelBase<TXRObject> : XRTextModelBase<TXRObject, XRDiagramTextItem>
		where TXRObject : XRControl {
		protected internal XRTextModelBase(XRControlModelFactory.ISource<TXRObject> source, ImageSource icon) : base(source, icon) { }
	}
}
namespace DevExpress.Xpf.Reports.UserDesigner.ReportModel {
	public static class XRContainerTextModel<TItemModel>
		where TItemModel : XRControlModelBase {
		public static XRContainerTextModelBase<TXRObject, TItemModel> New<TXRObject>(XRControlModelFactory.ISource<TXRObject> source, ImageSource icon)
			where TXRObject : XRControl {
			return new XRContainerTextModelBase<TXRObject, TItemModel>(source, icon);
		}
	}
	public static class XRContainerTextModel<TItemModel, TDiagramItem>
		where TItemModel : XRControlModelBase
		where TDiagramItem : XRDiagramTextContainer, new() {
		public static XRContainerTextModelBase<TXRObject, TItemModel, TDiagramItem> New<TXRObject>(XRControlModelFactory.ISource<TXRObject> source, ImageSource icon)
			where TXRObject : XRControl {
			return new XRContainerTextModelBase<TXRObject, TItemModel, TDiagramItem>(source, icon);
		}
	}
	public class XRContainerTextModelBase<TXRObject, TItemModel> : XRContainerTextModelBase<TXRObject, TItemModel, XRDiagramTextContainer>
		where TXRObject : XRControl
		where TItemModel : XRControlModelBase {
		protected internal XRContainerTextModelBase(XRControlModelFactory.ISource<TXRObject> source, ImageSource icon) : base(source, icon) { }
	}
}
namespace DevExpress.Xpf.Reports.UserDesigner.ReportModel {
	public static class XRChartElementModel {
		public static XRChartElementModelBase<TXRObject> New<TXRObject>(XRChartElementModelFactory.ISource<TXRObject> source)
			where TXRObject : ChartElement {
			return new XRChartElementModelBase<TXRObject>(source);
		}
	}
	public static class XRChartElementModel<TDiagramItem>
		where TDiagramItem : XRDiagramLogicItem, new() {
		public static XRChartElementModelBase<TXRObject, TDiagramItem> New<TXRObject>(XRChartElementModelFactory.ISource<TXRObject> source)
			where TXRObject : ChartElement {
			return new XRChartElementModelBase<TXRObject, TDiagramItem>(source);
		}
	}
	public class XRChartElementModelBase<TXRObject> : XRChartElementModelBase<TXRObject, XRDiagramLogicItem>
		where TXRObject : ChartElement {
		protected internal XRChartElementModelBase(XRChartElementModelFactory.ISource<TXRObject> source) : base(source) { }
	}
}
namespace DevExpress.Xpf.Reports.UserDesigner.ReportModel {
	public static class XRComponentModel {
		public static XRComponentModelBase<TXRObject> New<TXRObject>(XRControlModelFactory.ISource<TXRObject> source, ImageSource icon)
			where TXRObject : IComponent {
			return new XRComponentModelBase<TXRObject>(source, icon);
		}
	}
	public static class XRComponentModel<TDiagramItem>
		where TDiagramItem : XRDiagramLogicItem, new() {
		public static XRComponentModelBase<TXRObject, TDiagramItem> New<TXRObject>(XRControlModelFactory.ISource<TXRObject> source, ImageSource icon)
			where TXRObject : IComponent {
			return new XRComponentModelBase<TXRObject, TDiagramItem>(source, icon);
		}
	}
	public class XRComponentModelBase<TXRObject> : XRComponentModelBase<TXRObject, XRDiagramLogicItem>
		where TXRObject : IComponent {
		protected internal XRComponentModelBase(XRControlModelFactory.ISource<TXRObject> source, ImageSource icon) : base(source, icon) { }
	}
}
namespace DevExpress.Xpf.Reports.UserDesigner.ReportModel {
	public static class XRCrossBandControlModel<TDiagramItem>
		where TDiagramItem : XRCrossBandControlDiagramItem, new() {
		public static XRCrossBandControlModelBase<TXRObject, TDiagramItem> New<TXRObject>(XRControlModelFactory.ISource<TXRObject> source, ImageSource icon)
			where TXRObject : XRCrossBandControl {
			return new XRCrossBandControlModelBase<TXRObject, TDiagramItem>(source, icon);
		}
	}
}
