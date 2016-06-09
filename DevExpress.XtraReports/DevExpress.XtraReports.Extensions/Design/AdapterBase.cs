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
using System.Text;
using DevExpress.XtraReports.UI;
using System.ComponentModel.Design;
namespace DevExpress.XtraReports.Design {
	public class AdapterBase {
		protected IServiceProvider servProvider;
		IBandViewInfoService bandViewSvc;
		ZoomService zoomService;
		IDesignerHost host;
		ReportDesigner reportDesigner;
		protected XtraReport RootReport {
			get {
				return (XtraReport)Host.RootComponent;
			}
		}
		protected IDesignerHost Host {
			get {
				if(host == null)
					host = (IDesignerHost)GetService(typeof(IDesignerHost));
				return host;
			}
		}
		protected ZoomService ZoomService {
			get {
				if(zoomService == null)
					zoomService = ZoomService.GetInstance(servProvider);
				return zoomService;
			}
		}
		protected ReportDesigner ReportDesigner {
			get {
				if(reportDesigner == null)
					reportDesigner = (ReportDesigner)Host.GetDesigner(Host.RootComponent);
				return reportDesigner;
			}
		}
		protected IBandViewInfoService BandViewSvc {
			get {
				if(bandViewSvc == null)
					bandViewSvc = (IBandViewInfoService)GetService(typeof(IBandViewInfoService));
				return bandViewSvc;
			}
		}
		public AdapterBase(IServiceProvider servProvider) {
			this.servProvider = servProvider;
		}
		protected object GetService(Type serviceType) {
			return servProvider.GetService(serviceType);
		}
	}
	public class ControlAdapterBase : AdapterBase {
		XRControlDesignerBase designer;
		XRControl xrControl;
		protected XRControl XRControl {
			get {
				return xrControl;
			}
		}
		protected XRControlDesignerBase Designer {
			get {
				if(designer == null)
					designer = (XRControlDesignerBase)Host.GetDesigner(xrControl);
				return designer;
			}
		}
		public ControlAdapterBase(XRControl xrControl, IServiceProvider servProvider)
			: base(servProvider) {
			this.xrControl = xrControl;
		}
	}
}
