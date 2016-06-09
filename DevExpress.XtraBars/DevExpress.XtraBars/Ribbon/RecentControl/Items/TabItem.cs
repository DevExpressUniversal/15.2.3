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
using DevExpress.Utils;
using DevExpress.XtraBars.Ribbon.Handler;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraBars.Ribbon {
	public class RecentTabItem : RecentTextGlyphItemBase {
		RecentPanelBase panel;
		public RecentTabItem()
			: base() {
			CreateTabPanel();
		}
		protected override BaseAppearanceCollection CreateAppearanceCollection() {
			return new BaseRecentItemAppearanceCollection();
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RecentPanelBase TabPanel { get { return panel; } }
		void CreateTabPanel() {
			this.panel = new RecentStackPanel();
			this.panel.SetOwnerControl(RecentControl);
			this.panel.UpdateCaption(Glyph, Caption);
		}
		protected override void OnOwnerPanelChanged() {
			base.OnOwnerPanelChanged();
			UpdateTabPanel();
		}
		protected override void OnItemCaptionChanged() {
			base.OnItemCaptionChanged();
			this.panel.UpdateCaption(Glyph, Caption);
		}
		protected override void OnItemGlyphChanged() {
			base.OnItemGlyphChanged();
			this.panel.UpdateCaption(Glyph, Caption);
		}
		void UpdateTabPanel() {
			if(RecentControl != null) {
				TabPanel.SetOwnerControl(RecentControl);
			}
		}
		protected override RecentItemViewInfoBase CreateItemViewInfo() {
			return new RecentTabItemViewInfo(this);
		}
		protected override RecentItemHandlerBase CreateItemHandler() {
			return new RecentTabItemHandler(this);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				TabPanel.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
namespace DevExpress.XtraBars.Ribbon.ViewInfo {
	public class RecentTabItemViewInfo : RecentTextGlyphItemViewInfoBase {
		public RecentTabItemViewInfo(RecentTabItem item) : base(item) { }
		public RecentTabItem TabItem { get { return Item as RecentTabItem; } }
		public override bool IsSelected {
			get {
				return base.IsSelected || (Item.RecentControl !=null && Item.RecentControl.SelectedTab == TabItem && !Item.RecentControl.IsDesignMode);
			}
		}
		protected override AppearanceDefaultInfo[] GetAppearanceDefaultInfo() {
			AppearanceDefault app = new AppearanceDefault();
			ApplyBaseForeColor(app);
			app.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular);
			app.FontStyleDelta = System.Drawing.FontStyle.Regular;
			return new AppearanceDefaultInfo[]{
				new AppearanceDefaultInfo("ItemNormal", app),
				new AppearanceDefaultInfo("ItemHovered", app),
				new AppearanceDefaultInfo("ItemPressed", app)
			};
		}
		protected override BaseRecentItemAppearanceCollection ControlAppearances {
			get { return (Item.RecentControl.Appearances as RecentAppearanceCollection).TabItem; }
		}
		protected override BaseRecentItemAppearanceCollection ItemAppearanceCollection {
			get { return TabItem.Appearances as BaseRecentItemAppearanceCollection; }
		}
		protected override Padding ControlItemPadding { get { return Item.RecentControl.PaddingTabItem; } }
		protected override Padding GetSkinItemPadding() {
			return new Padding(5);
		}
		protected internal override void SetAppearanceDirty() {
			base.SetAppearanceDirty();
			TabItem.TabPanel.ViewInfo.SetAppearanceDirty();
		}
	}
}
namespace DevExpress.XtraBars.Ribbon.Handler {
	public class RecentTabItemHandler : RecentItemHandlerBase {
		public RecentTabItemHandler(RecentTabItem item) : base(item) { }
	}
}
