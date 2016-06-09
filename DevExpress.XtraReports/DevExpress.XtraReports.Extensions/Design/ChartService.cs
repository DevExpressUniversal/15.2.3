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
using System.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.XtraPrinting.Design;
using DevExpress.XtraCharts.Native;
using System.Windows.Forms;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using System.ComponentModel.Design;
namespace DevExpress.XtraReports.Design {
	class ChartService : IChartService {
		IServiceProvider provider;
		public ChartService(IServiceProvider provider) {
			this.provider = provider;
		}
		public void Invalidate(XRControl control) {
			IBandViewInfoService viewInfoService = GetBandViewInfoService(provider);
			if(viewInfoService != null)
				viewInfoService.InvalidateControlView(control);
		}
		public Point PointToClient(XRControl control, Point p) {
			IBandViewInfoService viewInfoService = GetBandViewInfoService(provider);
			if(viewInfoService != null) {
				p = viewInfoService.View.PointToClient(p);
				Rectangle rect = Rectangle.Round(viewInfoService.GetControlViewClientBounds(control, control.Band));
				p.X -= rect.Left;
				p.Y -= rect.Top;
			}
			ZoomService zoomService = GetZoomService(provider);
			if(zoomService != null) {
				p.X = zoomService.UnscaleValue(p.X);
				p.Y = zoomService.UnscaleValue(p.Y);
			}
			return p;
		}
		public Point PointToCanvas(XRControl control, Point p) {
			ZoomService zoomService = GetZoomService(provider);
			if(zoomService != null) {
				p.X = zoomService.ScaleValue(p.X);
				p.Y = zoomService.ScaleValue(p.Y);
			}
			IBandViewInfoService viewInfoService = GetBandViewInfoService(provider);
			if(viewInfoService != null) {
				Rectangle rect = Rectangle.Round(viewInfoService.GetControlScreenBounds(control));
				p.X += rect.Left;
				p.Y += rect.Top;
			}
			return p;
		}
		static IBandViewInfoService GetBandViewInfoService(IServiceProvider servProvider) {
			return servProvider == null ? null : servProvider.GetService(typeof(IBandViewInfoService)) as IBandViewInfoService;
		}
		static ZoomService GetZoomService(IServiceProvider servProvider) {
			return servProvider == null ? null : servProvider.GetService(typeof(ZoomService)) as ZoomService;
		}
		public object GetLookAndFeel(IServiceProvider servProvider) {
			return LookAndFeelProviderHelper.GetLookAndFeel(servProvider);
		}
		public void ShowErrorMessage(XRChart chart, string message, string title) {
			DevExpress.XtraPrinting.Tracer.TraceError(NativeSR.TraceSource, message);
			NotificationService.ShowMessage<XtraReport>(LookAndFeelProviderHelper.GetLookAndFeel(provider), provider.GetOwnerWindow(), message, title, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}
	}
}
