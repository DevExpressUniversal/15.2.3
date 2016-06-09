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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Text;
using DevExpress.XtraEditors.ButtonPanel;
namespace DevExpress.XtraBars.Docking2010 {
	public class WindowsUISeparatorInfo : WindowsUIBaseSeparatorInfo {
		public WindowsUISeparatorInfo(IBaseButton separator)
			: base(separator) {
		}
		public override void Calc(System.Drawing.Graphics g, BaseButtonPainter painter, System.Drawing.Point offset, System.Drawing.Rectangle maxRect, bool isHorizontal) {
			Calc(g, painter, offset, maxRect, isHorizontal, true);
		}
		public override void UpdatePaintAppearance(BaseButtonPainter painter) {
			AppearanceHelper.Combine(PaintAppearance,
				 new AppearanceObject[] { Button.Properties.Appearance }, painter.DefaultAppearance);
		}
	}
	public class WindowsUIButtonInfo : WindowsUIBaseButtonInfo {
		public WindowsUIButtonInfo(IBaseButton button)
			: base(button) {
		}
		public override bool Disabled {
			get { return base.Disabled || !ButtonPanelOwner.Enabled; }
			set { base.Disabled = value; }
		}
		protected internal new IWindowsUIButtonPanelOwner ButtonPanelOwner {
			get {
				return base.ButtonPanelOwner as IWindowsUIButtonPanelOwner;
			}
		}
		public override void UpdatePaintAppearance(BaseButtonPainter painter) {
			AppearanceHelper.Combine(PaintAppearance,
				 new AppearanceObject[] { Button.Properties.Appearance, GetStateAppearance() }, painter.DefaultAppearance);
		}
		public override void Calc(System.Drawing.Graphics g, BaseButtonPainter painter, System.Drawing.Point offset, System.Drawing.Rectangle maxRect, bool isHorizontal) {
			AppearanceList = null;
			Calc(g, painter, offset, maxRect, isHorizontal, true);
		}
		protected override Image GetImageByUriCore(DxImageUri imageUri) {
			if(imageUri.HasLargeImage)
				return imageUri.GetLargeImage();
			return base.GetImageByUriCore(imageUri);
		}
		protected override bool IsSideImageLocation(DevExpress.XtraEditors.ButtonPanel.ImageLocation location) {
			return location == DevExpress.XtraEditors.ButtonPanel.ImageLocation.AfterText || location == DevExpress.XtraEditors.ButtonPanel.ImageLocation.BeforeText;
		}
		protected override void CalcDefault(ref Rectangle first, ref Rectangle second, Point offset, int interval, Rectangle content) {
			CalcHorizontal(ref first, ref second, offset, interval, content);
		}
		protected override Size GetImageSize(bool hasImage) {
			Size collectionImageSize = ImageCollection.GetImageListSize(ButtonPanelOwner.ButtonBackgroundImages);
			Size baseImageSize = base.GetImageSize(hasImage);
			if(!collectionImageSize.IsEmpty)
				return collectionImageSize;
			return base.GetImageSize(hasImage);
		}
	}
	public class CustomHeaderButtonInfo : WindowsUIButtonInfo {
		public CustomHeaderButtonInfo(IBaseButton button)
			: base(button) {
		}
		protected override void CalcDefault(ref Rectangle first, ref Rectangle second, Point offset, int interval, Rectangle content) {
			CalcVertical(ref first, ref second, offset, interval, content);
		}
		protected override bool IsSideImageLocation(DevExpress.XtraEditors.ButtonPanel.ImageLocation location) {
			return location == DevExpress.XtraEditors.ButtonPanel.ImageLocation.AfterText ||
				location == DevExpress.XtraEditors.ButtonPanel.ImageLocation.BeforeText ||
				location == DevExpress.XtraEditors.ButtonPanel.ImageLocation.Default;
		}
	}
}
