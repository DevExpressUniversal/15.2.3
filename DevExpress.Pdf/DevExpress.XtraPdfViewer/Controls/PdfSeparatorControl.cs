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

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
namespace DevExpress.XtraPdfViewer.Native {
	[DXToolboxItem(false)]
	public class PdfSeparatorControl : BaseStyleControl {
		class PdfSeparatorControlViewInfo : BaseStyleControlViewInfo {
			public SkinElementInfo LineInfo { get { return new SkinElementInfo(CommonSkins.GetSkin(LookAndFeel.ActiveLookAndFeel)[CommonSkins.SkinLabelLine], Bounds); } }
			public override BorderPainter BorderPainter { get { return new EmptyBorderPainter(); } }
			public override AppearanceDefault DefaultAppearance { get { return new AppearanceDefault(Color.Transparent); } }
			public PdfSeparatorControlViewInfo(BaseStyleControl control) : base(control) { 
			}
			public override Size CalcBestFit(Graphics graphics) {
				if (LookAndFeel.ActiveStyle != ActiveLookAndFeelStyle.Skin)
					return new Size(0, 2);
				SkinElementInfo info = LineInfo;
				if (info != null) {
					SkinElement skinElement = info.Element;
					if (skinElement != null && skinElement.HasImage)
						return new Size(0, skinElement.Image.Image.Height);
				}
				return new Size(0, 2);
			}   
			protected override void CalcClientRect(Rectangle bounds) {
				fClientRect = bounds;
			}
		}
		class PdfSeparatorControlPainter : BaseControlPainter {
			public override void Draw(ControlGraphicsInfoArgs args) {
				base.Draw(args);
				GraphicsCache cache = args.Cache;
				if (args != null) {
					PdfSeparatorControlViewInfo viewInfo = args.ViewInfo as PdfSeparatorControlViewInfo;
					if (viewInfo != null) {
						UserLookAndFeel lookAndFeel = viewInfo.LookAndFeel;
						if (lookAndFeel != null && lookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) {
							ObjectPainter.DrawObject(cache, SkinElementPainter.Default, viewInfo.LineInfo);
							return;
						}
					}
				}
				Graphics graphics = args.Graphics;
				if (graphics != null) {
					int right = args.Bounds.Right;
					graphics.DrawLine(cache.GetPen(SystemColors.ControlDark), new Point(0, 0), new Point(right, 0));
					graphics.DrawLine(cache.GetPen(SystemColors.ControlLightLight), new Point(0, 1), new Point(right, 1));
				}
			}   
		}
		public PdfSeparatorControl() {
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			AutoSize = true;
		}
		public override Size GetPreferredSize(Size proposedSize) {
			return new Size(Width, CalcBestSize().Height);
		}
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
			base.SetBoundsCore(x, y, width, GetPreferredSize(new Size(width, height)).Height, specified);
		}
		protected override BaseStyleControlViewInfo CreateViewInfo() {
			return new PdfSeparatorControlViewInfo(this);
		}
		protected override BaseControlPainter CreatePainter() {
			return new PdfSeparatorControlPainter();
		}
	}
}
