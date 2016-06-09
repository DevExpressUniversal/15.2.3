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
using System.Reflection;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Helpers;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
namespace DevExpress.XtraPdfViewer.Native {
	[DXToolboxItem(false)]
	public class PdfSearchParametersButton : DropDownButton {
		class PdfSearchParametersButtonPainter : SkinDropDownButtonPainter {
			readonly ISkinProvider skinProvider;
			public PdfSearchParametersButtonPainter(ISkinProvider skinProvider) : base(skinProvider) {
				this.skinProvider = skinProvider;
			}
			protected override void DrawButton(ObjectInfoArgs e) {
				base.DrawButton(e);
				Color color = DefaultAppearance.ForeColor;
				Skin skin = CommonSkins.GetSkin(skinProvider);
				if (skin != null) {
					SkinElement element = skin[CommonSkins.SkinDropDownButton1];
					if (element != null)
						color = element.Color.GetForeColor();
				}
				DrawImage(e, color);
			}
		}
		class PdfSearchParametersButtonViewInfo : DropDownButtonViewInfo {
			static Image image = Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("DevExpress.XtraPdfViewer.Images.SearchSettingsButton.png"));
			public Image Image { get { return image; } }
			public PdfSearchParametersButtonViewInfo(DropDownButton owner) : base(owner) {
			}
			protected override EditorButtonPainter GetButtonPainter() {
				return (EditorButtonPainter)new PdfSearchParametersButtonPainter(OwnerControl.LookAndFeel);
			}
		}
		static void DrawImage(ObjectInfoArgs objectInfo, Color color) {
			DropDownButtonObjectInfoArgs dropDownButtonInfo = objectInfo as DropDownButtonObjectInfoArgs;
			if (dropDownButtonInfo != null) {
				PdfSearchParametersButtonViewInfo viewInfo = dropDownButtonInfo.ViewInfo as PdfSearchParametersButtonViewInfo;
				if (viewInfo != null) {
					Image image = viewInfo.Image;
					if (image != null)
						using (Image coloredImage = ColoredImageHelper.GetColoredImage(image, color)) {
							Rectangle destRect = objectInfo.Bounds;
							int buttonWidth = destRect.Width - 12;
							Size imageSize = image.Size;
							int margin = 3;
							destRect.Height -= margin * 2;
							destRect.Width = destRect.Height;
							destRect.Offset((buttonWidth - destRect.Width) / 2, margin);
							objectInfo.Cache.Graphics.DrawImage(coloredImage, destRect, new Rectangle(Point.Empty, imageSize), GraphicsUnit.Pixel);
						}
				}
			}
		}
		public PdfSearchParametersButton() {
		}
		protected override BaseStyleControlViewInfo CreateViewInfo() {
			return new PdfSearchParametersButtonViewInfo(this);
		}
	}
}
