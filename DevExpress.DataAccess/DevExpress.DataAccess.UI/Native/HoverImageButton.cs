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

using System.Drawing;
using DevExpress.XtraEditors;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;
namespace DevExpress.DataAccess.UI.Native {
	[DXToolboxItem(false)]
	public class HoverImageButton : SimpleButton {
		public Image[] Images {
			get { return new[] { hoverImage, normalImage }; }
			set {
				hoverImage = value[0];
				normalImage = value[1];
				Invalidate();
			}
		}
		Image hoverImage, normalImage;
		GraphicsPath graphicsPath;
		public HoverImageButton(Image[] images)
			: this(images[0], images[1]) {
		}
		public HoverImageButton(Image hoverImage, Image normalImage) {
			this.hoverImage = hoverImage;
			this.normalImage = normalImage;
			Rectangle rectangle = new Rectangle(0, 0, hoverImage.Size.Width + 2, hoverImage.Size.Height + 2);
			Size = rectangle.Size;
			graphicsPath = new GraphicsPath();
			graphicsPath.AddEllipse(rectangle);
			Region = new Region(graphicsPath);
			AllowFocus = false;
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (disposing) {
				if (graphicsPath != null) {
					graphicsPath.Dispose();
					graphicsPath = null;
				}
			}
		}
		protected override void OnPaint(PaintEventArgs e) {
			e.Graphics.DrawImage(Capture ? hoverImage : normalImage, 1, 1);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			Capture = RectangleToScreen(new Rectangle(0, 0, Width, Height)).Contains(PointToScreen(e.Location));
			base.OnMouseMove(e);
		}
	}
}
