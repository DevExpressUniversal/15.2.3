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
using System.Windows;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.RichEdit;
namespace DevExpress.Xpf.RichEdit.Menu {
	#region XpfRichEditMenuBuilderUIFactory
	public class XpfRichEditMenuBuilderUIFactory : IMenuBuilderUIFactory<RichEditCommand, RichEditCommandId> {
		public IDXMenuCheckItemCommandAdapter<RichEditCommandId> CreateMenuCheckItemAdapter(RichEditCommand command) {
			return new RichEditMenuCheckItemCommandSLAdapter(command);
		}
		public virtual IDXMenuItemCommandAdapter<RichEditCommandId> CreateMenuItemAdapter(RichEditCommand command) {
			return new RichEditMenuItemCommandSLAdapter(command);
		}
		public virtual IDXPopupMenu<RichEditCommandId> CreatePopupMenu() {
			return new RichEditMenuBuilderInfo();
		}
		public IDXPopupMenu<RichEditCommandId> CreateSubMenu() {
			return new RichEditMenuBuilderInfo();
		}
	}
	#endregion
	public class XpfRichEditHoverMenuBuilderUIFactory : XpfRichEditMenuBuilderUIFactory {
		readonly BarManager barManager;
		public XpfRichEditHoverMenuBuilderUIFactory(BarManager barManager) {
			Guard.ArgumentNotNull(barManager, "barManager");
			this.barManager = barManager;
		}
		public BarManager BarManager { get { return barManager; } }
		public override IDXPopupMenu<RichEditCommandId> CreatePopupMenu() {
			return new RichEditHoverMenu();
		}
		public override IDXMenuItemCommandAdapter<RichEditCommandId> CreateMenuItemAdapter(RichEditCommand command) {
			return new RichEditToolMenuItemCommandSLAdapter(command, BarManager);
		}
	}
}
