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
using DevExpress.XtraBars;
using System.ComponentModel;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Localization;
namespace DevExpress.XtraScheduler.UI {
	#region ViewNavigator
	[Obsolete(ObsoleteText.SRViewNavigator, true)]
	public partial class ViewNavigator : SchedulerCommandBarComponent {
		public ViewNavigator()
			: base() {
		}
		public ViewNavigator(IContainer container)
			: base(container) {
			container.Add(this);
		}
		protected override Type SupportedBarType { get { return typeof(ViewNavigatorBar); } }
		protected override Type SupportedBarItemType { get { return typeof(ViewNavigatorItemBase); } }
		protected override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new ViewNavigatorItemBuilder();
		}
		protected override CommandBasedBar CreateBarInstance() {
			return new ViewNavigatorBar();
		}
	}
	#endregion
	#region ViewNavigatorBar
	public class ViewNavigatorBar : CommandBasedBar {
		public ViewNavigatorBar()
			: base() {
		}
		public ViewNavigatorBar(BarManager manager)
			: base(manager) {
		}
		public ViewNavigatorBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText {
			get { return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Caption_ViewNavigator); }
		}
	}
	#endregion
}
