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
using System.Drawing;
using System.ComponentModel.Design;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Design.Adapters;
namespace DevExpress.XtraReports.Design.MouseTargets {
	class PivotGridMouseTarget : ControlMouseTarget {
		new XRPivotGridDesigner Designer {
			get { return (XRPivotGridDesigner)base.Designer; }
		}
		public PivotGridMouseTarget(XRControl xrControl, IServiceProvider servProvider)
			: base(xrControl, servProvider) {
		}
		public override void HandleMouseMove(object sender, BandMouseEventArgs e) {
			base.HandleMouseMove(sender, e);
			InvalidateViewInfo();
		}
		public override void HandleMouseLeave(object sender, EventArgs e) {
			base.HandleMouseLeave(sender, e);
			InvalidateViewInfo();
		}
		void InvalidateViewInfo() {
			PointF pt = Designer.ScreenPointToClient(System.Windows.Forms.Cursor.Position);
			foreach(ComponentViewInfo item in Designer.DrawInfos) {
				if(item.RectanglePx.Contains(pt) && !item.Hot) {
					this.BandViewSvc.InvalidateControlView(this.XRControl);
					return;
				}
				if(!item.RectanglePx.Contains(pt) && item.Hot) {
					this.BandViewSvc.InvalidateControlView(this.XRControl);
					return;
				}
			}
		}
		public override void HandleMouseDown(object sender, BandMouseEventArgs e) {
			PointF pt = BandPointToClient(e.Location, e.ViewInfo);
			foreach(ComponentViewInfo item in Designer.DrawInfos) {
				if(item.RectanglePx.Contains(pt)) {
					Designer.SelectComponents(new object[] { item.Component }, SelectionTypes.Replace);
					return;
				}
			}
			base.HandleMouseDown(sender, e);
		}
		PointF BandPointToClient(Point pt, BandViewInfo viewInfo) {
			IBoundsAdapter adapter = BoundsAdapterService.GetAdapter(XRControl, servProvider);
			RectangleF bounds = adapter.GetClientBandBounds(viewInfo);
			return new PointF(pt.X - bounds.X, pt.Y - bounds.Y);
		}
	}
}
