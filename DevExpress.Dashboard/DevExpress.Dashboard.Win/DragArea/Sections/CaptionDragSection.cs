#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using System.Drawing;
using DevExpress.Utils.Drawing;
namespace DevExpress.DashboardWin.Native {
	public abstract class CaptionDragSection : DragSection {
		string caption;
		Rectangle captionBounds;
		protected override Rectangle ContentBounds { get { return captionBounds; } }
		protected CaptionDragSection(DragArea area, string caption, string itemName)
			: this(area, caption, itemName, null) {
		}
		protected CaptionDragSection(DragArea area, string caption, string itemName, string itemNamePlural)
			: base(area, itemName, itemNamePlural) {
			SetCaption(caption);
		}
		protected void SetCaption(string caption) {
			this.caption = caption;
		}
		protected virtual Color? GetCaptionColor(DragAreaDrawingContext drawingContext) {
			return drawingContext.Painters.AreaPainter.SectionHeaderColor;
		}
		protected override void ArrangeContent(DragAreaDrawingContext drawingContext, GraphicsCache cache, Point location) {
			captionBounds = new Rectangle(location, DragAreaDrawingContext.MeasureString(caption, drawingContext.Appearances.AreaAppearance, cache));
		}
		protected override void PaintContent(DragAreaDrawingContext drawingContext, GraphicsCache cache) {
			DragAreaDrawingContext.DrawString(caption, drawingContext.Appearances.AreaAppearance, cache, captionBounds, GetCaptionColor(drawingContext));
		}
	}
}
