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
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
namespace DevExpress.XtraScheduler.UI {
	#region RibbonViewNavigator
	[Obsolete(ObsoleteText.SRRibbonViewNavigator, true)]
	public partial class RibbonViewNavigator : SchedulerRibbonCommandBarComponent {
		protected override Type SupportedRibbonPageGroupType { get { return typeof(ViewNavigatorRibbonPageGroup); } }
		protected override Type SupportedRibbonPageType { get { return typeof(ViewNavigatorRibbonPage); } }
		protected override Type SupportedBarItemType { get { return typeof(ViewNavigatorItemBase); } }
		protected override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new ViewNavigatorRibbonPage();
		}
		protected override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new ViewNavigatorRibbonPageGroup();
		}
		protected override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new ViewNavigatorItemBuilder();
		}
	}
	#endregion
	#region ViewNavigatorRibbonPage
	public class ViewNavigatorRibbonPage : CommandBasedRibbonPage {
		public ViewNavigatorRibbonPage()
			: base() {
		}
		public ViewNavigatorRibbonPage(string text)
			: base(text) {
		}
		public override string DefaultText { get { return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Caption_ViewNavigator); } }
	}
	#endregion
	#region ViewNavigatorRibbonPageGroup
	public class ViewNavigatorRibbonPageGroup : CommandBasedRibbonPageGroup {
		public ViewNavigatorRibbonPageGroup()
			: base() {
		}
		public ViewNavigatorRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText {
			get { return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Caption_ViewNavigator); }
		}
	}
	#endregion
}
