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
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using System.Drawing.Drawing2D;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Office.Layout;
using DevExpress.DocumentView;
namespace DevExpress.XtraRichEdit.Painters {
	#region SimpleViewPainter (abstract class)
	public abstract class SimpleViewPainter : RichEditViewPainter {
		protected SimpleViewPainter(SimpleView view)
			: base(view) {
		}
		protected internal override Rectangle GetPageBounds(PageViewInfo page) {
			Rectangle bounds = page.Page.Bounds;
			bounds.Width = Int32.MaxValue / 2;
			return bounds;
		}
		protected internal override RectangleF GetNewClientBounds(RectangleF clipBounds, RectangleF oldClipBounds) {
			return oldClipBounds;
		}
	}
	#endregion
	#region SimpleViewFlatPainter
	public class SimpleViewFlatPainter : SimpleViewPainter {
		public SimpleViewFlatPainter(SimpleView view)
			: base(view) {
		}
	}
	#endregion
	#region SimpleViewUltraFlatPainter
	public class SimpleViewUltraFlatPainter : SimpleViewPainter {
		public SimpleViewUltraFlatPainter(SimpleView view)
			: base(view) {
		}
	}
	#endregion
	#region SimpleViewStyle3DPainter
	public class SimpleViewStyle3DPainter : SimpleViewPainter {
		public SimpleViewStyle3DPainter(SimpleView view)
			: base(view) {
		}
	}
	#endregion
	#region SimpleViewOffice2003Painter
	public class SimpleViewOffice2003Painter : SimpleViewPainter {
		public SimpleViewOffice2003Painter(SimpleView view)
			: base(view) {
		}
	}
	#endregion
	#region SimpleViewWindowsXPPainter
	public class SimpleViewWindowsXPPainter : SimpleViewPainter {
		public SimpleViewWindowsXPPainter(SimpleView view)
			: base(view) {
		}
	}
	#endregion
	#region SimpleViewSkinPainter
	public class SimpleViewSkinPainter : SimpleViewPainter {
		readonly RichEditViewSkinPainterHelper helper;
		SkinElement moreButtonElement;
		public SimpleViewSkinPainter(SimpleView view)
			: base(view) {
			this.helper = new RichEditViewSkinPainterHelper(view);
		}
		protected internal override void DrawEmptyPage(GraphicsCache cache, PageViewInfo page) {
		}
		protected internal override void DrawEmptyComment(GraphicsCache cache, CommentViewInfo comment) {
			Rectangle rect = comment.Bounds;
			const long maxBitmapArea = 1920 * 1200;
			long area = (long)rect.Width * (long)rect.Height;
			if (area > 0 && area <= maxBitmapArea) {
				helper.DrawEmptyComment(cache, comment);
			}
			else {
				DrawCommentImage(cache, comment, rect);
			}
		}
		protected internal virtual void DrawCommentImage(GraphicsCache cache, CommentViewInfo comment, Rectangle commentBounds) {
			Color fillColor = DocumentModel.CommentColorer.GetColor(comment.Comment);
			SkinElementInfo element = helper.CreateCommentSkinElementInfo(commentBounds, RichEditSkins.SkinCommentBorder, fillColor);
			if (element.Element != null) {
				helper.DrawCommentImageCore(cache, commentBounds, element); 
			}
			else {
				base.DrawEmptyComment(cache, comment);
			} 
		}
		protected internal override void DrawEmptyExtensionComment(GraphicsCache cache, CommentViewInfo comment) {
			if (moreButtonElement == null) {
				moreButtonElement = helper.GetSkinElement(LookAndFeel, RichEditSkins.SkinCommentMoreButton);
				if (moreButtonElement == null || moreButtonElement.Image == null) {
					base.DrawEmptyExtensionComment(cache, comment);
					return;
				}
			}
			helper.DrawEmptyExtensionComment(cache, comment, moreButtonElement);
		}
		protected internal override void ResetCache() {
			helper.ResetCache();
		}
	}
	#endregion
	#region SimpleViewBackgroundPainter (abstract class)
	public abstract class SimpleViewBackgroundPainter : RichEditViewBackgroundPainter {
		protected SimpleViewBackgroundPainter(SimpleView view)
			: base(view) {
		}
		public new SimpleView View { get { return (SimpleView)base.View; } }
		public override void Draw(GraphicsCache cache, Rectangle bounds) {
			cache.FillRectangle(GetActualPageBackColor(), bounds);
		}
		protected internal override Color GetActualPageBackColor() {
			Color pageBackColor = View.DocumentModel.DocumentProperties.PageBackColor;			
			return DXColor.IsTransparentOrEmpty(pageBackColor) ? View.ActualBackColor : pageBackColor;
		}
	}
	#endregion
	#region SimpleViewFlatBackgroundPainter
	public class SimpleViewFlatBackgroundPainter : SimpleViewBackgroundPainter {
		public SimpleViewFlatBackgroundPainter(SimpleView view)
			: base(view) {
		}
	}
	#endregion
	#region SimpleViewUltraFlatBackgroundPainter
	public class SimpleViewUltraFlatBackgroundPainter : SimpleViewBackgroundPainter {
		public SimpleViewUltraFlatBackgroundPainter(SimpleView view)
			: base(view) {
		}
	}
	#endregion
	#region SimpleViewStyle3DBackgroundPainter
	public class SimpleViewStyle3DBackgroundPainter : SimpleViewBackgroundPainter {
		public SimpleViewStyle3DBackgroundPainter(SimpleView view)
			: base(view) {
		}
	}
	#endregion
	#region SimpleViewOffice2003BackgroundPainter
	public class SimpleViewOffice2003BackgroundPainter : SimpleViewBackgroundPainter {
		public SimpleViewOffice2003BackgroundPainter(SimpleView view)
			: base(view) {
		}
	}
	#endregion
	#region SimpleViewWindowsXPBackgroundPainter
	public class SimpleViewWindowsXPBackgroundPainter : SimpleViewBackgroundPainter {
		public SimpleViewWindowsXPBackgroundPainter(SimpleView view)
			: base(view) {
		}
	}
	#endregion
	#region SimpleViewSkinBackgroundPainter
	public class SimpleViewSkinBackgroundPainter : SimpleViewBackgroundPainter {
		public SimpleViewSkinBackgroundPainter(SimpleView view)
			: base(view) {
		}
	}
	#endregion
}
