#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.ExpressApp.Win.Templates.Controls {
	public class ToolbarCustomizedEventArgs : EventArgs {
		private CollectionChangeAction action;
		private BarItemLink link;
		public ToolbarCustomizedEventArgs(BarItemLink link, CollectionChangeAction action) {
			this.action = action;
			this.link = link;
		}
		public BarItemLink Link {
			get { return link; }
		}
		public CollectionChangeAction Action {
			get { return action; }
		}
	}
	[ToolboxItem(false)]
	public class XafRibbonControl : RibbonControl {
		public XafRibbonControl() {
			ShowApplicationButton = DevExpress.Utils.DefaultBoolean.True;
			AllowCustomization = false;
			AutoHideEmptyItems = true;
		}
		protected override void OnAddToToolbar(BarItemLink link) {
			base.OnAddToToolbar(link);
			OnToolbarCustomized(link, CollectionChangeAction.Add);
		}
		protected override void OnRemoveFromToolbar(BarItemLink link) {
			OnToolbarCustomized(link, CollectionChangeAction.Remove);
			base.OnRemoveFromToolbar(link);
		}
		protected virtual void OnToolbarCustomized(BarItemLink link, CollectionChangeAction action) {
			if(ToolbarCustomized != null) {
				ToolbarCustomized(this, new ToolbarCustomizedEventArgs(link, action));
			}
		}
		public event EventHandler<ToolbarCustomizedEventArgs> ToolbarCustomized;
	}
}
