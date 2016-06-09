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

using DevExpress.Skins;
using DevExpress.XtraBars.Ribbon.Drawing;
using DevExpress.XtraBars.Ribbon.Handler;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.XtraBars.Ribbon {
	public class RecentSeparatorItem : RecentItemBase {
		public RecentSeparatorItem() : base() { }
		public RecentSeparatorItem(RecentPanelBase panel)
			: base(panel) {
		}
		protected override RecentItemViewInfoBase CreateItemViewInfo() {
			return new RecentSeparatorItemViewInfo(this);
		}
		protected override RecentItemPainterBase CreateItemPainter() {
			return new RecentSeparatorItemPainter();
		}
		protected override RecentItemHandlerBase CreateItemHandler() {
			return new RecentSeparatorItemHandler(this);
		}
	}
}
namespace DevExpress.XtraBars.Ribbon.ViewInfo {
	public class RecentSeparatorItemViewInfo : RecentItemViewInfoBase {
		public RecentSeparatorItemViewInfo(RecentSeparatorItem item) : base(item) { }
		public override SkinElementInfo GetItemInfo() {
			SkinElement elem = CommonSkins.GetSkin(Item.RecentControl.LookAndFeel.ActiveLookAndFeel)["LabelLine"];
			if(elem == null)
				elem = CommonSkins.GetSkin(DevExpress.XtraEditors.Controls.DefaultSkinProvider.Default)["LabelLine"];
			return new SkinElementInfo(elem, Bounds);
		}
	}
}
namespace DevExpress.XtraBars.Ribbon.Drawing {
	public class RecentSeparatorItemPainter : RecentItemPainterBase {
	}
}
namespace DevExpress.XtraBars.Ribbon.Handler {
	public class RecentSeparatorItemHandler : RecentItemHandlerBase {
		public RecentSeparatorItemHandler(RecentSeparatorItem item) : base(item) { }
	}
}
