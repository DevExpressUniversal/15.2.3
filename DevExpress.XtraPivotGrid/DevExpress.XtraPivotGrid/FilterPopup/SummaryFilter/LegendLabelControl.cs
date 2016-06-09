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

using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
namespace DevExpress.XtraPivotGrid.FilterPopup.SummaryFilter {
	class LegendLabelControl : LabelControl {
		const int ImageSize = 8;
		Color legendColor = Color.Black;
		public LegendLabelControl() {
			ImageAlignToText = ImageAlignToText.LeftCenter;
		}
		public Color LegendColor {
			get { return legendColor; }
			set {
				if(legendColor != value || Appearance.Image == null) {
					legendColor = value;
					UpdateImage();
				}
			}
		}
		void UpdateImage() {
			Bitmap image = new Bitmap(ImageSize, Height);
			using(Graphics graphics = Graphics.FromImage(image))
			using(Brush brushBackground = new SolidBrush(BackColor))
			using(Brush brushForeground = new SolidBrush(LegendColor)) {
				int textBottom = GetTextBaselineY() - 1;
				graphics.FillRectangle(brushBackground, graphics.ClipBounds);
				graphics.FillRectangle(brushForeground, new Rectangle(0, textBottom - ImageSize, ImageSize, ImageSize));
			}
			Appearance.Image = image;
		}
	}
}
