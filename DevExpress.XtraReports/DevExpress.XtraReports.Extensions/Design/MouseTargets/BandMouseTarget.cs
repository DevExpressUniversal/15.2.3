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
using System.Drawing.Design;
using System.Windows.Forms;
using System.Collections;
using System.Drawing.Printing;
using DevExpress.Utils;
namespace DevExpress.XtraReports.Design.MouseTargets {
	class BandMouseTarget : MouseTargetBase {
		public BandMouseTarget(XRControl xrControl, IServiceProvider servProvider)
			: base(xrControl, servProvider) {
		}
		public override bool ContainsPoint(Point pt, BandViewInfo viewInfo) {
			if(viewInfo == null)
				return false;
			Rectangle rect = Rectangle.Union(viewInfo.BandBounds, viewInfo.BottomSideHitTestBounds);
			rect = Rectangle.Union(rect, viewInfo.RightSideHitTestBounds);
			return rect.Contains(pt);
		}
		public override void HandleMouseDown(object sender, BandMouseEventArgs e) {
			if(XRControlDesignerBase.CursorEquals(Cursors.HSplit) && e.Button.IsLeft()) {
				BandViewSvc.StartBandHeightChanging(Array.IndexOf(BandViewSvc.ViewInfos, e.ViewInfo));
			} else if(XRControlDesignerBase.CursorEquals(Cursors.VSplit) && e.Button.IsLeft()) {
				BandViewSvc.StartWidthChanging(e.ViewInfo, Control.MousePosition);
			} else
				base.HandleMouseDown(sender, e);
		}
		protected override Cursor GetCursor(Point pt) {
			ToolboxItem tbxItem = GetSelectedToolboxItem();
			if(tbxItem == null) {
				SplitService splitSvc = GetService(typeof(SplitService)) as SplitService;
				if(splitSvc.IsRunning)
					return splitSvc.Vertical ? Cursors.VSplit : Cursors.HSplit;
				CapturePaintService captSvc = GetService(typeof(CapturePaintService)) as CapturePaintService;
				BandViewInfo viewInfo = BandViewSvc.GetViewInfoByBand((Band)XRControl);
				if(captSvc.IsRunning || viewInfo == null)
					return base.GetCursor(pt);
				if(!Designer.Locked && viewInfo.BottomSideHitTestBounds.Contains(pt) && viewInfo.Expanded)
					return Cursors.HSplit;
				if(viewInfo.LeftMarginHitTestBounds.Contains(pt) || viewInfo.RightMarginHitTestBounds.Contains(pt))
					return Cursors.VSplit;
				if(this.RootReport.PaperKind.Equals(PaperKind.Custom) && viewInfo.RightSideHitTestBounds.Contains(pt))
					return Cursors.VSplit;
			}
			return base.GetCursor(pt);
		}
		protected override void PrepareSelectedComponentsList(ArrayList components, XtraReportBase report, RectangleF bounds) {
			foreach(BandViewInfo bandViewInfo in BandViewSvc.ViewInfos) {
				if(bandViewInfo.Band is XtraReportBase || !bandViewInfo.Expanded)
					continue;
				ArrayList printableControls = bandViewInfo.Band.GetPrintableControls();
				foreach(XRControl printableControl in printableControls) {
					RectangleF r = BandViewSvc.GetControlScreenBounds(printableControl, bandViewInfo);
					if(!components.Contains(printableControl.RealControl) && !r.Contains(bounds) && r.IntersectsWith(bounds))
						components.Add(printableControl.RealControl);
				}
			}
		}
	}
}
