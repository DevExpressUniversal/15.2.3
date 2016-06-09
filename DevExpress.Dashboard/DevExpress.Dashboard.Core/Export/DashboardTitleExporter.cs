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

using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
namespace DevExpress.DashboardExport {
	public class ExportDashboardTitle {
		readonly DetailBand band;
		readonly Rectangle titleRect;
		readonly Font font;
		readonly TextAlignment textAlignment;
		public ExportDashboardTitle(DetailBand band, Font font, TextAlignment textAlignment, Rectangle titleRect) {
			this.band = band;
			this.font = font;
			this.textAlignment = textAlignment;
			this.titleRect = titleRect;
		}		
		public void Initialize(DashboardTitleViewModel titleViewModel) {
			Guard.ArgumentNotNull(titleViewModel, "titleViewModel");
			DashboardImageViewModel imageViewModel = null;
			DashboardTitleAlignment alignment = DashboardTitleAlignment.Left;
			if(titleViewModel.LayoutModel != null) {
				imageViewModel = titleViewModel.LayoutModel.ImageViewModel;
				alignment = titleViewModel.LayoutModel.Alignment;
			}
			DashboardTitleImageInfo imageInfo = DashboardTitleImageInfo.Create(imageViewModel);
			int imageWidth = 0;
			int imageHeight = 0;
			if(imageInfo != null) {
				imageWidth = imageInfo.ImageSize.Width;
				imageHeight = imageInfo.ImageSize.Height;
			}
			SizeF titleTextSize = SizeF.Empty;
			if(!string.IsNullOrEmpty(titleViewModel.Text))
				titleTextSize = Measurement.MeasureString(titleViewModel.Text, font, GraphicsUnit.Pixel);
			int left = alignment == DashboardTitleAlignment.Center ?
				Math.Max(0, Convert.ToInt32(titleRect.Width - imageWidth - Convert.ToInt32(titleTextSize.Width) - 1) / 2) :
				DashboardTitleViewModel.LeftPadding;
			XRPanel panel = new XRPanel();
			band.Controls.Add(panel);
			panel.BoundsF = titleRect;
			if(imageInfo != null) {
				XRPictureBox pictureBox = new XRPictureBox() {
					Image = imageInfo.Image,
					Sizing = XtraPrinting.ImageSizeMode.ZoomImage,
					Borders = BorderSide.None
				};
				panel.Controls.Add(pictureBox);
				pictureBox.BoundsF = new RectangleF(left, titleRect.Height - imageHeight, imageWidth, imageHeight);
			}
			TextWithParameters textWithParameters = new TextWithParameters {
				Text = titleViewModel.Text,
				TextFont = font,
				TextColor = Color.Black,
				TextPaddingInfo = new PaddingInfo(0, 0, 0, 0),
				Alignment = textAlignment
			};
			RectangleF labelBounds = new RectangleF(imageWidth + 1 + left, 0, Math.Min(titleRect.Width - imageWidth - 1, titleTextSize.Width), titleRect.Height);
			foreach(KeyValuePair<LabelType, XRTrimmingTextLabel> pair in textWithParameters.GetLabels()) {
				panel.Controls.Add(pair.Value);
				pair.Value.BoundsF = labelBounds;
			}
		}
	}
}
