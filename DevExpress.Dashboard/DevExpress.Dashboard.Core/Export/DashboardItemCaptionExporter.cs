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

using System;
using System.Collections.Generic;
using System.Drawing;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.UI;
namespace DevExpress.DashboardExport {
	public class ExportItemCaption : IItemCaption {
		DetailBand band;
		Rectangle captionBounds;
		DashboardFontInfo fontInfo;
		Font Font { get { return FontHelper.GetFont(ExportHelper.CalcCaptionFont(captionBounds.Height), fontInfo); } }
		public ExportItemCaption(DetailBand band, Rectangle captionBounds, DashboardFontInfo fontInfo) {
			this.band = band;
			this.captionBounds = captionBounds;
			this.fontInfo = fontInfo;
		}
		void IItemCaption.Update(ItemCaptionContentInfo itemCaptionContentInfo) {
			string captionText = itemCaptionContentInfo.CaptionText;
			Font captionTextFont = Font;
			string drillDownValuesText = itemCaptionContentInfo.FilterValuesText;
			Font drillDownValuesTextFont = Font;
			SizeF captionTextSize = string.IsNullOrEmpty(captionText) ? Size.Empty : Measurement.MeasureString(captionText, captionTextFont, GraphicsUnit.Pixel);
			SizeF drillDownValuesTextSize = string.IsNullOrEmpty(drillDownValuesText) ? Size.Empty : Measurement.MeasureString(drillDownValuesText, drillDownValuesTextFont, GraphicsUnit.Pixel);
			TextWithParameters textWithParameters = new TextWithParameters {
				Text = itemCaptionContentInfo.CaptionText,
				TextFont = captionTextFont,
				TextColor = Color.Black,
				TextPaddingInfo = new PaddingInfo(5, 0, 0, 0),
				Parameters = itemCaptionContentInfo.FilterValuesText,
				ParametersFont = captionTextFont,
				ParametersColor = Color.Black,
				ParametersPaddingInfo = new PaddingInfo(5, 0, 0, 0),
				Alignment = TextAlignment.MiddleLeft
			};
			foreach(KeyValuePair<LabelType, XRTrimmingTextLabel> pair in textWithParameters.GetLabels()) {
				band.Controls.Add(pair.Value);
				if(pair.Key == LabelType.Text)
					pair.Value.BoundsF = new RectangleF {
						X = captionBounds.Left,
						Y = captionBounds.Top,
						Width = Math.Min(captionTextSize.Width, captionBounds.Width),
						Height = captionBounds.Height
					};
				else
					pair.Value.BoundsF = new RectangleF {
						X = captionBounds.Left + captionTextSize.Width,
						Y = captionBounds.Top,
						Width = Math.Min(drillDownValuesTextSize.Width, captionBounds.Width - captionTextSize.Width),
						Height = captionBounds.Height
					};
			}
		}
	}
}
