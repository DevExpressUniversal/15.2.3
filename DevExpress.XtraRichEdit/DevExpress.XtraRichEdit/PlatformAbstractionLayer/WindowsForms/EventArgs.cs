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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraRichEdit.Menu;
using DevExpress.XtraRichEdit.Layout.Export;
namespace DevExpress.XtraRichEdit {
	#region PopupMenuShowingEventHandler
	public delegate void PopupMenuShowingEventHandler(object sender, PopupMenuShowingEventArgs e);
	#endregion
	#region PopupMenuShowingEventArgs
	public class PopupMenuShowingEventArgs : EventArgs {
		RichEditPopupMenu menu;
		public PopupMenuShowingEventArgs(RichEditPopupMenu menu) {
			this.menu = menu;
		}
		public RichEditPopupMenu Menu { get { return menu; } set { menu = value; } }
	}
	#endregion
	#region PreparePopupMenuEventHandler
	[Obsolete("You should use the 'PopupMenuShowingEventHandler' instead", true), EditorBrowsable(EditorBrowsableState.Never)]
	public delegate void PreparePopupMenuEventHandler(object sender, PreparePopupMenuEventArgs e);
	#endregion
	#region PreparePopupMenuEventArgs
	[Obsolete("You should use the 'PopupMenuShowingEventArgs' instead", true), EditorBrowsable(EditorBrowsableState.Never)]
	public class PreparePopupMenuEventArgs : PopupMenuShowingEventArgs {
		public PreparePopupMenuEventArgs(RichEditPopupMenu menu)
			: base(menu) {
		}
	}
	#endregion
	#region RichEditViewCustomDrawEventHandler
	public delegate void RichEditViewCustomDrawEventHandler(object sender, RichEditViewCustomDrawEventArgs e);
	#endregion
	#region RichEditViewCustomDrawEventArgs
	public class RichEditViewCustomDrawEventArgs : EventArgs {
		readonly GraphicsCache cache;
		public RichEditViewCustomDrawEventArgs(GraphicsCache cache) {
			Guard.ArgumentNotNull(cache, "cache");
			this.cache = cache;
		}
		public GraphicsCache Cache { get { return cache; } }
	}
	#endregion
	#region RichEditCustomMarkDrawEventHandler
	public delegate void RichEditCustomMarkDrawEventHandler(object sender, RichEditCustomMarkDrawEventArgs e);
	#endregion
	#region RichEditCustomMarkDrawEventArgs
	public class RichEditCustomMarkDrawEventArgs : EventArgs {
		readonly Graphics graphics;
		readonly CustomMarkVisualInfoCollection visualInfoCollection;
		public RichEditCustomMarkDrawEventArgs(Graphics graphics, CustomMarkVisualInfoCollection visualInfoCollection) {
			Guard.ArgumentNotNull(graphics, "graphics");
			Guard.ArgumentNotNull(visualInfoCollection, "visualInfoCollection");
			this.graphics = graphics;
			this.visualInfoCollection = visualInfoCollection;
		}
		public Graphics Graphics { get { return graphics; } }
		public CustomMarkVisualInfoCollection VisualInfoCollection { get { return visualInfoCollection; } }
	}
	#endregion
}
