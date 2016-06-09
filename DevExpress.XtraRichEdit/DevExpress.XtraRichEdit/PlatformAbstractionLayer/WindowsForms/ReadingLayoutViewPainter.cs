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
using System.Drawing;
using DevExpress.Utils.Drawing;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.XtraRichEdit.Layout;
namespace DevExpress.XtraRichEdit.Painters {
	#region ReadingLayoutViewPainter (abstract class)
	public abstract class ReadingLayoutViewPainter : RichEditViewPainter {
		protected ReadingLayoutViewPainter(ReadingLayoutView view)
			: base(view) {
		}
		protected internal override void DrawEmptyPage(GraphicsCache cache, PageViewInfo page) {
			cache.FillRectangle(View.ActualBackColor, page.ClientBounds);
#if (DEBUG)
			int pageIndex = View.FormattingController.PageController.Pages.IndexOf(page.Page);
			RichEditControl control = (RichEditControl)View.Control;
			cache.Graphics.DrawString(String.Format("DBG: PageIndex={0}", pageIndex), control.Font, Brushes.Red, page.Bounds, StringFormat.GenericTypographic);
#endif
		}
		protected internal override void DrawEmptyComment(GraphicsCache cache, Layout.CommentViewInfo comment) {
		}
		protected internal override void DrawEmptyExtensionComment(GraphicsCache cache, CommentViewInfo comment) { 
		}
	}
	#endregion
	#region ReadingLayoutViewFlatPainter
	public class ReadingLayoutViewFlatPainter : ReadingLayoutViewPainter {
		public ReadingLayoutViewFlatPainter(ReadingLayoutView view)
			: base(view) {
		}
	}
	#endregion
	#region ReadingLayoutViewUltraFlatPainter
	public class ReadingLayoutViewUltraFlatPainter : ReadingLayoutViewPainter {
		public ReadingLayoutViewUltraFlatPainter(ReadingLayoutView view)
			: base(view) {
		}
	}
	#endregion
	#region ReadingLayoutViewStyle3DPainter
	public class ReadingLayoutViewStyle3DPainter : ReadingLayoutViewPainter {
		public ReadingLayoutViewStyle3DPainter(ReadingLayoutView view)
			: base(view) {
		}
	}
	#endregion
	#region ReadingLayoutViewOffice2003Painter
	public class ReadingLayoutViewOffice2003Painter : ReadingLayoutViewPainter {
		public ReadingLayoutViewOffice2003Painter(ReadingLayoutView view)
			: base(view) {
		}
	}
	#endregion
	#region ReadingLayoutViewWindowsXPPainter
	public class ReadingLayoutViewWindowsXPPainter : ReadingLayoutViewPainter {
		public ReadingLayoutViewWindowsXPPainter(ReadingLayoutView view)
			: base(view) {
		}
	}
	#endregion
	#region ReadingLayoutViewSkinPainter
	public class ReadingLayoutViewSkinPainter : ReadingLayoutViewPainter {
		public ReadingLayoutViewSkinPainter(ReadingLayoutView view)
			: base(view) {
		}
		protected internal override void DrawEmptyPage(GraphicsCache cache, PageViewInfo page) {
			cache.FillRectangle(View.ActualBackColor, page.ClientBounds);
#if (DEBUG)
			int pageIndex = View.FormattingController.PageController.Pages.IndexOf(page.Page);
			RichEditControl control = (RichEditControl)View.Control;
			cache.Graphics.DrawString(String.Format("DBG: PageIndex={0}", pageIndex), control.Font, Brushes.Red, page.Bounds, StringFormat.GenericTypographic);
#endif
		}
		protected internal override void DrawEmptyComment(GraphicsCache cache, Layout.CommentViewInfo comment) {
		}
		protected internal override void DrawEmptyExtensionComment(GraphicsCache cache, CommentViewInfo comment) {
		}
	}
	#endregion
	#region ReadingLayoutViewBackgroundPainter (abstract class)
	public abstract class ReadingLayoutViewBackgroundPainter : RichEditViewBackgroundPainter {
		protected  ReadingLayoutViewBackgroundPainter(ReadingLayoutView view)
			: base(view) {
		}
	}
	#endregion
	#region ReadingLayoutViewFlatBackgroundPainter
	public class ReadingLayoutViewFlatBackgroundPainter : ReadingLayoutViewBackgroundPainter {
		public ReadingLayoutViewFlatBackgroundPainter(ReadingLayoutView view)
			: base(view) {
		}
	}
	#endregion
	#region ReadingLayoutViewUltraFlatBackgroundPainter
	public class ReadingLayoutViewUltraFlatBackgroundPainter : ReadingLayoutViewBackgroundPainter {
		public ReadingLayoutViewUltraFlatBackgroundPainter(ReadingLayoutView view)
			: base(view) {
		}
	}
	#endregion
	#region ReadingLayoutViewStyle3DBackgroundPainter
	public class ReadingLayoutViewStyle3DBackgroundPainter : ReadingLayoutViewBackgroundPainter {
		public ReadingLayoutViewStyle3DBackgroundPainter(ReadingLayoutView view)
			: base(view) {
		}
	}
	#endregion
	#region ReadingLayoutViewOffice2003BackgroundPainter
	public class ReadingLayoutViewOffice2003BackgroundPainter : ReadingLayoutViewBackgroundPainter {
		public ReadingLayoutViewOffice2003BackgroundPainter(ReadingLayoutView view)
			: base(view) {
		}
	}
	#endregion
	#region ReadingLayoutViewWindowsXPBackgroundPainter
	public class ReadingLayoutViewWindowsXPBackgroundPainter : ReadingLayoutViewBackgroundPainter {
		public ReadingLayoutViewWindowsXPBackgroundPainter(ReadingLayoutView view)
			: base(view) {
		}
	}
	#endregion
	#region ReadingLayoutViewSkinBackgroundPainter
	public class ReadingLayoutViewSkinBackgroundPainter : ReadingLayoutViewBackgroundPainter {
		public ReadingLayoutViewSkinBackgroundPainter(ReadingLayoutView view)
			: base(view) {
		}
	}
	#endregion
}
