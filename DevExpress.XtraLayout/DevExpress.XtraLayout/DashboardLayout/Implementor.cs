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
using System.Runtime.InteropServices;
using DevExpress.Utils.Controls;
using System.Diagnostics;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Skins;
using Padding = DevExpress.XtraLayout.Utils.Padding;
using DevExpress.XtraLayout.Handlers;
using DevExpress.XtraLayout.HitInfo;
namespace DevExpress.XtraDashboardLayout {
	public class DashboardLayoutControlImplementor : LayoutControlImplementor {
		public DashboardLayoutControlImplementor(ILayoutControlOwner owner)
			: base(owner) {
		}
		protected override LayoutControlGroup CreateRoot() {
			return (DashboardLayoutControlGroupBase)CreateLayoutGroup(null);
		}
		public override LayoutGroupHandlerWithTabHelper CreateRootHandler(LayoutGroup group) {
			return new DashboardLayoutGroupHandler(group); 
		}
		protected override EnabledStateController CreateEnabledStateController() {
			return new DashboardEnabledStateController(as_I);
		}
		public override LayoutGroup CreateLayoutGroup(LayoutGroup parent) {
			return new DashboardLayoutControlGroupBase(parent as DashboardLayoutControlGroupBase);
		}
		public override BaseLayoutItem CreateLayoutItem(LayoutGroup parent) {
			return new DashboardLayoutControlItemBase(parent as DashboardLayoutControlGroupBase);
		}
		public override TabbedGroup CreateTabbedGroup(LayoutGroup parent) {
			throw new Exception("Not allowed");
		}
		public override EmptySpaceItem CreateEmptySpaceItem(LayoutGroup parent) {
			throw new Exception("Not allowed");
		}
		public override SplitterItem CreateSplitterItem(LayoutGroup parent) {
			throw new Exception("Not allowed");
		}
		protected override IFixedLayoutControlItem CreateFixedLayoutItem(string typeName) {
			throw new Exception("Not allowed");
		}
		public override void SetIsModified(bool newVal) {
			base.SetIsModified(newVal);
			(RootGroup as DashboardLayoutControlGroupBase).RebuildCrosshairCollection();
		}
		public override ToolTipControlInfo GetItemToolTipInfo(Point pt, BaseLayoutItemHitInfo hi) {
			DashboardHitInfo dhi = hi as DashboardHitInfo;
			if(dhi != null) {
				DashboardLayoutControl layoutControl = (DashboardLayoutControl)owner;
				layoutControl.RaiseGetCaptionImageToolTip(layoutControl.PointToScreen(pt), dhi.AdditionalHitType, dhi.Item);
			}
			return base.GetItemToolTipInfo(pt, hi);
		}
		protected override void UpdateDefaultValues(LayoutControlDefaultsPropertyBag defaults) {
			base.UpdateDefaultValues(defaults);
			defaults.GroupSpacing = Padding.Empty;
			Skin skin = DashboardSkins.GetSkin(LookAndFeel);
			if(skin != null) {
				Padding padding = GetSkinPadding(skin, DashboardSkins.SkinDashboardItemSpacing);
				if(padding != Padding.Empty) {
					defaults.LayoutItemSpacing = padding;
					defaults.GroupSpacing = padding;
				}
			}
		}
	}
	public class DashboardEnabledStateController : EnabledStateController {
		public DashboardEnabledStateController(ILayoutControl owner) : base(owner) { }
		protected override bool CalcEnabledState(BaseLayoutItem item, bool state) {
			return true;
		}
	}
	public class DashboardLayoutGroupHandler : LayoutGroupHandlerWithTabHelper {
		public DashboardLayoutGroupHandler(BaseLayoutItem item) : base(item) { }
		protected override bool PerformSelection() {
			return false;
		}
	}
}
