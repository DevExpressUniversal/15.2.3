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
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Layout;
namespace DevExpress.XtraRichEdit.Painters {
	#region DraftViewPainter (abstract class)
	public abstract class DraftViewPainter : RichEditViewPainter {
		protected DraftViewPainter(DraftView view)
			: base(view) {
		}
		protected internal override void DrawEmptyPages(GraphicsCache cache) {
		}
		protected internal override void DrawEmptyPage(GraphicsCache cache, PageViewInfo page) {
		}
		protected internal override void DrawEmptyComment(GraphicsCache cache, CommentViewInfo comment) {
		}
		protected internal override void DrawEmptyExtensionComment(GraphicsCache cache, CommentViewInfo comment) {
		}
		protected override bool CommentsVisible(int count) {
			return false;
		}
		protected internal override Rectangle GetPageBounds(PageViewInfo page) {
			Rectangle result = base.GetPageBounds(page);
			result.Width = Int32.MaxValue / 4;
			return result;
		}
	}
	#endregion
	#region DraftViewFlatPainter
	public class DraftViewFlatPainter : DraftViewPainter {
		public DraftViewFlatPainter(DraftView view)
			: base(view) {
		}
	}
	#endregion
	#region DraftViewUltraFlatPainter
	public class DraftViewUltraFlatPainter : DraftViewPainter {
		public DraftViewUltraFlatPainter(DraftView view)
			: base(view) {
		}
	}
	#endregion
	#region DraftViewStyle3DPainter
	public class DraftViewStyle3DPainter : DraftViewPainter {
		public DraftViewStyle3DPainter(DraftView view)
			: base(view) {
		}
	}
	#endregion
	#region DraftViewOffice2003Painter
	public class DraftViewOffice2003Painter : DraftViewPainter {
		public DraftViewOffice2003Painter(DraftView view)
			: base(view) {
		}
	}
	#endregion
	#region DraftViewWindowsXPPainter
	public class DraftViewWindowsXPPainter : DraftViewPainter {
		public DraftViewWindowsXPPainter(DraftView view)
			: base(view) {
		}
	}
	#endregion
	#region DraftViewSkinPainter
	public class DraftViewSkinPainter : DraftViewPainter {
		public DraftViewSkinPainter(DraftView view)
			: base(view) {
		}
		protected internal override void DrawEmptyPage(GraphicsCache cache, PageViewInfo page) {
#if (DEBUG)
			int pageIndex = View.FormattingController.PageController.Pages.IndexOf(page.Page);
			RichEditControl control = (RichEditControl)View.Control;
			cache.Graphics.DrawString(String.Format("DBG: PageIndex={0}", pageIndex), control.Font, Brushes.Red, page.Bounds, StringFormat.GenericTypographic);
#endif
		}
		protected internal override void DrawEmptyComment(GraphicsCache cache, CommentViewInfo comment) {
		}
		protected internal override void DrawEmptyExtensionComment(GraphicsCache cache, CommentViewInfo comment) {
		}
	}
	#endregion
	#region DraftViewBackgroundPainter (abstract class)
	public abstract class DraftViewBackgroundPainter : RichEditViewBackgroundPainter {
		protected DraftViewBackgroundPainter(DraftView view)
			: base(view) {
		}
		public override void Draw(GraphicsCache cache, Rectangle bounds) {
			cache.FillRectangle(GetActualPageBackColor(), bounds);
		}
		protected internal override Color GetActualPageBackColor() {
			DocumentProperties documentProperties = View.DocumentModel.DocumentProperties;
			Color pageBackColor = documentProperties.PageBackColor;
			if (documentProperties.DisplayBackgroundShape && !DXColor.IsTransparentOrEmpty(pageBackColor))
				return pageBackColor;
			return View.ActualBackColor;
		}
	}
	#endregion
	#region DraftViewFlatBackgroundPainter
	public class DraftViewFlatBackgroundPainter : DraftViewBackgroundPainter {
		public DraftViewFlatBackgroundPainter(DraftView view)
			: base(view) {
		}
	}
	#endregion
	#region DraftViewUltraFlatBackgroundPainter
	public class DraftViewUltraFlatBackgroundPainter : DraftViewBackgroundPainter {
		public DraftViewUltraFlatBackgroundPainter(DraftView view)
			: base(view) {
		}
	}
	#endregion
	#region DraftViewStyle3DBackgroundPainter
	public class DraftViewStyle3DBackgroundPainter : DraftViewBackgroundPainter {
		public DraftViewStyle3DBackgroundPainter(DraftView view)
			: base(view) {
		}
	}
	#endregion
	#region DraftViewOffice2003BackgroundPainter
	public class DraftViewOffice2003BackgroundPainter : DraftViewBackgroundPainter {
		public DraftViewOffice2003BackgroundPainter(DraftView view)
			: base(view) {
		}
	}
	#endregion
	#region DraftViewWindowsXPBackgroundPainter
	public class DraftViewWindowsXPBackgroundPainter : DraftViewBackgroundPainter {
		public DraftViewWindowsXPBackgroundPainter(DraftView view)
			: base(view) {
		}
	}
	#endregion
	#region DraftViewSkinBackgroundPainter
	public class DraftViewSkinBackgroundPainter : DraftViewBackgroundPainter {
		public DraftViewSkinBackgroundPainter(DraftView view)
			: base(view) {
		}
	}
	#endregion
}
