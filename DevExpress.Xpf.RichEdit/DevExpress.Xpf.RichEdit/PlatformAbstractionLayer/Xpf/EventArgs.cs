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
using DevExpress.XtraRichEdit.Menu;
using DevExpress.Xpf.Bars;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.Xpf.RichEdit;
namespace DevExpress.Xpf.RichEdit {
	#region PopupMenuShowingEventHandler
	public delegate void PopupMenuShowingEventHandler(object sender, PopupMenuShowingEventArgs e);
	#endregion
	#region PopupMenuShowingEventArgs
	public class PopupMenuShowingEventArgs : EventArgs {
		PopupMenu menu;
		public PopupMenuShowingEventArgs(PopupMenu menu) {
			Guard.ArgumentNotNull(menu, "menu");
			this.menu = menu;
		}
		public PopupMenu Menu { get { return menu; } set { menu = value; } }
	}
	#endregion
	#region HoverMenuShowingEventHandler
	public delegate void HoverMenuShowingEventHandler(object sender, HoverMenuShowingEventArgs e);
	#endregion
	#region HoverMenuShowingEventArgs
	public class HoverMenuShowingEventArgs : EventArgs {
		RichEditHoverMenu menu;
		public HoverMenuShowingEventArgs(RichEditHoverMenu menu) {
			Guard.ArgumentNotNull(menu, "menu");
			this.menu = menu;
		}
		public RichEditHoverMenu Menu { get { return menu; } set { menu = value; } }
	}
	#endregion
	#region CustomMarkDrawEventHandler
	public delegate void CustomMarkDrawEventHandler(object sender, CustomMarkDrawEventArgs e);
	#endregion
	#region CustomMarkDrawEventArgs
	public class CustomMarkDrawEventArgs : EventArgs {
		readonly CustomMarkVisualInfoCollection visualInfoCollection;
		public CustomMarkDrawEventArgs(CustomMarkVisualInfoCollection visualInfoCollection) {
			this.visualInfoCollection = visualInfoCollection;
		}
		public CustomMarkVisualInfoCollection VisualInfoCollection { get { return visualInfoCollection; } }
	}
	#endregion
}
namespace DevExpress.XtraRichEdit {
	#region PreparePopupMenuEventHandler
	[Obsolete("You should use the 'PopupMenuShowingEventHandler' instead", true), EditorBrowsable(EditorBrowsableState.Never)]
	public delegate void PreparePopupMenuEventHandler(object sender, PreparePopupMenuEventArgs e);
	#endregion
	#region PreparePopupMenuEventArgs
	[Obsolete("You should use the 'PopupMenuShowingEventArgs' instead", true), EditorBrowsable(EditorBrowsableState.Never)]
	public class PreparePopupMenuEventArgs : PopupMenuShowingEventArgs {
		public PreparePopupMenuEventArgs(PopupMenu menu)
			: base(menu) {
		}
	}
	#endregion
	#region PrepareHoverMenuEventHandler
	[Obsolete("You should use the 'HoverMenuShowingEventHandler' instead", false), EditorBrowsable(EditorBrowsableState.Never)]
	public delegate void PrepareHoverMenuEventHandler(object sender, PrepareHoverMenuEventArgs e);
	#endregion
	#region PrepareHoverMenuEventArgs
	[Obsolete("You should use the 'HoverMenuShowingEventArgs' instead", false), EditorBrowsable(EditorBrowsableState.Never)]
	public class PrepareHoverMenuEventArgs : HoverMenuShowingEventArgs {
		public PrepareHoverMenuEventArgs(RichEditHoverMenu menu)
			: base(menu) {
		}
	}
	#endregion
}
