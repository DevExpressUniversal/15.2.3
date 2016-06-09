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

using DevExpress.Xpf.Bars;
using System;
using System.ComponentModel;
namespace DevExpress.Xpf.Ribbon {
	public class RibbonPropertyChangedEventArgs : EventArgs {
		public object OldValue { get; protected set; }
		public object NewValue { get; protected set; }
		public RibbonPropertyChangedEventArgs(object oldValue, object newValue) {
			OldValue = oldValue;
			NewValue = newValue;
		}
	}
	public class ToolbarCustomizationMenuEventArgs : EventArgs {
		public QuickAccessToolbarCustomizationMenu Menu { get; protected set; }
		public ToolbarCustomizationMenuEventArgs(QuickAccessToolbarCustomizationMenu customizationMenu) {
			Menu = customizationMenu;
		}
	}
	public class ToolbarCustomizationMenuShowingEventArgs : ToolbarCustomizationMenuEventArgs {
		public bool Cancel { get; set; }
		public ToolbarCustomizationMenuShowingEventArgs(QuickAccessToolbarCustomizationMenu customizationMenu) : base(customizationMenu) { }
	}
	public class ToolbarCustomizationMenuClosedEventArgs : ToolbarCustomizationMenuEventArgs {
		public ToolbarCustomizationMenuClosedEventArgs(QuickAccessToolbarCustomizationMenu customizationMenu) : base(customizationMenu) { }
	}
	public class RibbonPopupMenuShowingEventArgs : EventArgs {
		public BarItem BarItem { get; protected set; }
		public BarItemLinkBase Link { get; protected set; }
		public bool IsLinkInToolbar { get; protected set; }
		public RibbonPopupMenu Menu { get; protected set; }
		public bool Cancel { get; set; }
		public RibbonPopupMenuShowingEventArgs(RibbonPopupMenu popupMenu, BarItem item, BarItemLinkBase itemLink, bool isLinkInToolbar) {
			Menu = popupMenu;
			BarItem = item;
			Link = itemLink;
			IsLinkInToolbar = isLinkInToolbar;
		}
	}
	public class RibbonPopupMenuClosedEventArgs : EventArgs {
		public RibbonPopupMenu Menu { get; protected set; }
		public RibbonPopupMenuClosedEventArgs(RibbonPopupMenu popupMenu) {
			Menu = popupMenu;
		}
	}
	public class RibbonPageInsertedEventArgs : EventArgs {
		public RibbonPage RibbonPage { get; protected set; }
		public int Index { get; protected set; }
		public RibbonPageInsertedEventArgs(RibbonPage page, int pageIndex) {
			RibbonPage = page;
			Index = pageIndex;
		}
	}
	public class RibbonPageRemovedEventArgs : RibbonPageInsertedEventArgs {
		public RibbonPageRemovedEventArgs(RibbonPage page, int pageIndex) : base(page, pageIndex) { }
	}
	public class RibbonPageGroupInsertedEventArgs : EventArgs {
		public RibbonPageGroup RibbonPageGroup { get; protected set; }
		public int Index { get; protected set; }
		public RibbonPageGroupInsertedEventArgs(RibbonPageGroup group, int groupIndex) {
			RibbonPageGroup = group;
			Index = groupIndex;
		}
	}
	public class RibbonSaveRestoreLayoutExceptionEventArgs : EventArgs {
		public Exception Exception { get; private set; }
		public RibbonSaveRestoreLayoutExceptionEventArgs(Exception ex) {
			this.Exception = ex;
		}
	}
	public class RibbonPageGroupRemovedEventArgs : RibbonPageGroupInsertedEventArgs {
		public RibbonPageGroupRemovedEventArgs(RibbonPageGroup group, int groupIndex) : base(group, groupIndex) { }
	}
	public delegate void RibbonPropertyChangedEventHandler(object sender, RibbonPropertyChangedEventArgs e);
	public delegate void ToolbarCustomizationMenuShowingEventHandler(object sender, ToolbarCustomizationMenuShowingEventArgs e);
	public delegate void ToolbarModeChangedEventHandler(object sender, PropertyChangedEventArgs e);
	public delegate void ToolbarCustomizationMenuClosedEventHandler(object sender, ToolbarCustomizationMenuClosedEventArgs e);
	public delegate void RibbonPopupMenuClosedEventHandler(object sender, RibbonPopupMenuClosedEventArgs e);
	public delegate void RibbonPopupMenuShowingEventHandler(object sender, RibbonPopupMenuShowingEventArgs e);
	public delegate void RibbonPageInsertedEventHandler(object sender, RibbonPageInsertedEventArgs e);
	public delegate void RibbonPageRemovedEventHandler(object sender, RibbonPageRemovedEventArgs e);
	public delegate void RibbonPageGroupInsertedEventHandler(object sender, RibbonPageGroupInsertedEventArgs e);
	public delegate void RibbonPageGroupRemovedEventHandler(object sender, RibbonPageGroupRemovedEventArgs e);
	public delegate void RibbonSaveRestoreLayoutExceptionEventHandler(object sender, RibbonSaveRestoreLayoutExceptionEventArgs e);
}
