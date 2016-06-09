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
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.XtraPrinting.Native;
using System.Drawing;
using DevExpress.XtraPrinting;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Control;
using DevExpress.LookAndFeel;
namespace DevExpress.XtraReports.Native.Templates {
	[ToolboxItem(false)]
	public class FramedPictureBox : UserControl {
		public FramedPictureBox() {
			InitializeComponent();
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true); 
		}
		private void InitializeComponent() {
			this.SuspendLayout();
			this.Name = "FramedPictureBox";
			this.Size = new System.Drawing.Size(492, 407);
			this.ResumeLayout(false);
		}
		Image image;
		[Bindable(true)]
		public Image EditValue {
			set {
				image = value;
				Refresh();
			}
			get {
				return image;
			} 
		}
		Rectangle GetImageRect(SkinPaddingEdges spe) {
			if(EditValue == null)
				return Rectangle.Empty;
			RectangleF imageRect = new RectangleF(PointF.Empty, EditValue.Size);
			var clientRect = spe.Deflate(ClientRectangle);
			if (imageRect.Width > clientRect.Width || imageRect.Height > clientRect.Height) {
				imageRect.Size = MathMethods.ZoomInto(clientRect.Size, imageRect.Size);
			}
			imageRect = RectF.Align(imageRect, clientRect, BrickAlignment.Center, BrickAlignment.Center);
			return Rectangle.Round(imageRect);
		}
		UserLookAndFeel GetLookAndFeel() {
			XtraForm form = FindForm() as XtraForm;
			return form != null ? form.LookAndFeel : null;
		}
		SkinPaddingEdges GetEdges(UserLookAndFeel lookAndFeel) {
			return lookAndFeel != null ? DevExpress.DocumentView.SkinPaintHelper.GetSkinEdges(lookAndFeel, PrintingSkins.SkinBorderPage) :
				new SkinPaddingEdges(0);
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			if(EditValue == null)
				return;
			using(GraphicsCache cache = new GraphicsCache(e.Graphics)) {
				e.Graphics.FillRectangle(cache.GetSolidBrush(BackColor), ClientRectangle);
				UserLookAndFeel lookAndFeel = GetLookAndFeel();
				SkinPaddingEdges spe = GetEdges(lookAndFeel);
				Rectangle rect = GetImageRect(spe);
				DevExpress.DocumentView.SkinPaintHelper.DrawSkinElement(lookAndFeel, cache, PrintingSkins.SkinBorderPage, spe.Inflate(rect), 1);
				e.Graphics.DrawImage(EditValue, rect);
			}
		}
	}
}
