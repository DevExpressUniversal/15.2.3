#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using DevExpress.DashboardWin.Bars;
using DevExpress.DashboardWin.Localization;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraBars.Commands.Ribbon;
namespace DevExpress.DashboardWin.Bars {
	public class ViewRibbonPage : ControlCommandBasedRibbonPage {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonGroupView); } }
	}
	public class ViewBar : DashboardCommandBar {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.BarViewName); } }
	}
	public class SkinsRibbonPageGroup : DashboardRibbonPageGroup {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonGroupSkinsCaption); } }
	}
	public class DashboardSkinsBarItem : RibbonGalleryBarItem, ICommandBarItem {
		public DashboardSkinsBarItem() { 
			AllowDrawArrow = true;
		}
		#region ICommandBarItem Members
		void ICommandBarItem.InvokeCommand() {
		}
		bool ICommandBarItem.IsEqual(ICommandBarItem item) {
			if (item == null)
				return false;
			return this.GetType() == item.GetType();
		}
		void ICommandBarItem.SetupStatusBarLink(BarItemLink link) {
		}
		void ICommandBarItem.UpdateCaption() {
		}
		void ICommandBarItem.UpdateChecked() {
		}
		void ICommandBarItem.UpdateGroupIndex() {
		}
		void ICommandBarItem.UpdateImages() {
		}
		void ICommandBarItem.UpdateVisibility() {
		}
		#endregion
	}
}
namespace DevExpress.DashboardWin.Native {
	public class SkinsBarItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {			
			items.Add(new DashboardSkinsBarItem());
		}
	}
	public class SkinsBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(ViewRibbonPage); } }
		public override Type SupportedBarType { get { return typeof(ViewBar); } }
		public override int DockColumn { get { return DashboardBarDockColumns.View; } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(SkinsRibbonPageGroup); } }
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SkinsBarItemBuilder();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new SkinsRibbonPageGroup();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new ViewRibbonPage();
		}
		public override Bar CreateBar() {
			return new ViewBar();
		}
	}
}
