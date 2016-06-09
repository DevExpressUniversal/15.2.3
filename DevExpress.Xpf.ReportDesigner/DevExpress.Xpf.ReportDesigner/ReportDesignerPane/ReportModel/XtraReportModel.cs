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
using System.Linq;
using System.Windows;
using System.Windows.Media;
using DevExpress.Mvvm.Native;
using DevExpress.Utils;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Reports.UserDesigner.Layout.Native;
using DevExpress.Xpf.Reports.UserDesigner.Native;
using DevExpress.Xpf.Reports.UserDesigner.Toolbox;
using DevExpress.Xpf.Reports.UserDesigner.XRDiagram;
using DevExpress.XtraReports.UI;
namespace DevExpress.Xpf.Reports.UserDesigner.ReportModel {
	public class XtraReportModel : XtraReportModelBase {
		public XtraReportModel(IReportModelOwner owner, XtraReport report, ImageSource icon = null)
			: base(owner, report, icon ?? XRControlModelBase.GetDefaultIcon("XtraReport")) {
			PrinterName = CreateXRPropertyModel(() => XRObject.PrinterName, () => XRObject.PrinterName, printer => Tracker.Set(XRObject, x => x.PrinterName, printer));
		}
		protected override void InitializeXRObjectIfNeeded(object xrObject, ModelFactoryData factoryData) {
			base.InitializeXRObjectIfNeeded(xrObject, factoryData);
			var report = (XtraReport)xrObject;
			if(report.Bands[BandKind.TopMargin] == null)
				report.Bands.Add(XtraReport.CreateBand(BandKind.TopMargin));
			if(report.Bands[BandKind.Detail] == null) {
				var detail = XtraReport.CreateBand(BandKind.Detail);
				detail.HeightF = BoundsConverter.ToFloat(BoundsConverter.ToDouble(250.0f, XtraPrinting.GraphicsDpi.Pixel), report.Dpi);
				report.Bands.Add(detail);
			}
			if(report.Bands[BandKind.BottomMargin] == null)
				report.Bands.Add(XtraReport.CreateBand(BandKind.BottomMargin));
		}
		public XRPropertyModel<string> PrinterName { get; private set; }
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int DetailCountAtDesignTime {
			get { return XRObject.ReportPrintOptions.DetailCountAtDesignTime; }
			set { XRObject.ReportPrintOptions.DetailCountAtDesignTime = value; }
		}
		protected override IEnumerable<PropertyDescriptor> GetEditableProperties() {
			return base.GetEditableProperties()
				.InjectPropertyModel(this, x => x.PrinterName)
				.InjectProperty(this, x => x.DetailCountAtDesignTime);
		}
		protected override BaseXRCommands CreateXRCommands() { return new XRCommands(this); }
		protected override IEnumerable<ToolViewModel> CreateToolboxToolsCore(XRDiagramControl diagram) {
			var tools = new XRControlToolBase[] {
				new XRControlTool<XRLabel>(diagram),
				new XRControlTool<XRCheckBox>(diagram),
				new XRControlTool<XRRichText>(diagram),
				new XRControlTool<XRPictureBox>(diagram),
				new XRControlTool<XRPanel>(diagram, XRDefaults<XRPanel>.Size),
				new XRControlTool<XRTable>(diagram, x => new Size(x.PageSize.Width - x.RightPadding, XRControlModelBase.DefaultSize.Height)),
				new XRControlTool<XRLine>(diagram),
				new XRControlTool<XRShape>(diagram),
				new XRControlTool<XRBarCode>(diagram),
				new XRControlTool<XRZipCode>(diagram),
				new XRControlTool<XRChart>(diagram, new Size(200.0, 150.0)),
				new XRControlTool<XRGauge>(diagram, XRDefaults<XRGauge>.Size),
				new XRControlTool<XRSparkline>(diagram, XRDefaults<XRSparkline>.Size),
				new XRControlTool<XRPivotGrid>(diagram, XRDefaults<XRPivotGrid>.Size),
				new XRControlTool<XRSubreport>(diagram, XRDefaults<XRSubreport>.Size),
				new XRControlTool<XRTableOfContents>(diagram, XRDefaults<XRTableOfContents>.GetBandWidthSize),
				new XRControlTool<XRPageInfo>(diagram, XRDefaults<XRPageInfo>.Size),
				new XRControlTool<XRPageBreak>(diagram, XRDefaults<XRPageBreak>.GetBandWidthSize),
				new XRControlTool<XRCrossBandLine>(diagram, XRDefaults<XRCrossBandLine>.Size),
				new XRControlTool<XRCrossBandBox>(diagram, XRDefaults<XRCrossBandBox>.Size)
			};
			Func<XRControlToolBase, ImageSource> getIcon = x => ImageSourceHelper.GetImageSource(AssemblyHelper.GetResourceUri(typeof(XtraReportModel).Assembly, string.Format("Images/Toolbox32x32/{0}.png", x.XRObjectType.Name)));
			return tools.Select(x => new ToolViewModel(x, getIcon(x)));
		}
	}
}
