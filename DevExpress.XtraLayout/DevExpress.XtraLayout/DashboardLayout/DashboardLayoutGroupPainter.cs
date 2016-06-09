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
using System.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout.ViewInfo;
using DevExpress.XtraLayout.Painting;
using DevExpress.XtraLayout.Utils;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.XtraLayout;
using System.Collections;
namespace DevExpress.XtraDashboardLayout {
	public class DashboardGroupPainter : DashboardLayoutControlItemPainter, IPanelControlOwner {
		public override void DrawObject(ObjectInfoArgs e) {
			base.DrawObject(e);
			BaseViewInfo vi = e as BaseViewInfo;
			if(vi != null) {
				DashboardLayoutControlGroupBase group = vi.Owner as DashboardLayoutControlGroupBase;
				if(group != null)
					group.RaisePaintEvent(new System.Windows.Forms.PaintEventArgs(e.Graphics, e.Bounds));
			}
		}
		public override void DrawControlsArea(BaseLayoutItemViewInfo e) {
			if(e!=null && (e.Owner as DashboardLayoutControlGroupBase).GroupBordersVisible)ObjectPainter.DrawObject(e.Cache, GetBorderPainter(e), e.BorderInfo);
			if(e != null) {
				LayoutGroup group = e.Owner as LayoutGroup;
				if(group.Expanded) {
					if(group.Items.Count > 0) {
						foreach(BaseLayoutItem item in new ArrayList(group.Items))
							ObjectPainter.DrawObject(e.Cache, item.PaintStyle.GetPainter(item), item.ViewInfo);
					}
				}
			}
		}
	}
}
