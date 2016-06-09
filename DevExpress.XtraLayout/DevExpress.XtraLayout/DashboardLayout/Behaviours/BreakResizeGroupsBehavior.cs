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
using System.Data;
using System.Drawing;
using System.Collections;
using DevExpress.XtraLayout;
using DevExpress.Utils;
using DevExpress.Data;
using DevExpress.XtraEditors;
using System.Reflection;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraLayout.Helpers;
using System.Windows.Forms;
using System.Collections.Generic;
using DevExpress.XtraLayout.Handlers;
using DevExpress.Utils.Controls;
using DevExpress.XtraLayout.HitInfo;
using DevExpress.Utils.Drawing;
using DevExpress.XtraLayout.Customization;
namespace DevExpress.XtraDashboardLayout {
	public class BreakResizeGroupBehavior : BaseBehaviour {
		public BreakResizeGroupBehavior(AdornerWindowHandler handler) : base(handler) { }
		public override void Invalidate() {
			base.Invalidate();
			CrosshairHelper.GetGroupCrosshairs(owner.Owner.Root);
			Dictionary<Crosshair, Crosshair> options= new Dictionary<Crosshair,Crosshair>();
			foreach(DashboardLayoutControlItemBase bli in owner.Owner.Root.Items) {
			}
		}
	}
	public class BreakResizeGroupBehaviorGlyph : Glyph {
		public BreakResizeGroupBehaviorGlyph(DashboardLayoutControlItemBase targetItem)
			: base(targetItem.Owner) {
			this.targetItem = targetItem;
			Bounds = BreakResizeGroupBehaviorGlyph.CalcBounds(targetItem);
		}
		DashboardLayoutControlItemBase targetItem;
		public DashboardLayoutControlItemBase TargetItem { get { return targetItem; } set { targetItem = value; } }
		const int imageWidth = 31;
		const int imageHeight = 30;
		public static Rectangle CalcBounds(BaseLayoutItem item) {
			Point leftTopImageLocation = RectangleHelper.RightBottomCorner(item.ViewInfo.BoundsRelativeToControl);
			leftTopImageLocation.X -= imageWidth / 2;
			leftTopImageLocation.Y -= imageHeight / 2;
			return new Rectangle(leftTopImageLocation, new Size(imageWidth, imageHeight));
		}
		public override void Paint(Graphics g) {
			base.Paint(g);
		}
	}
}
