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
using System.Collections;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Text;
using DevExpress.Utils.Controls;
using DevExpress.DashboardCommon.Viewer;
using System;
namespace DevExpress.DashboardWin.Native {
	public static class CardPainter {
		public static void Draw(AppearanceObject appearance, GraphicsCache cache, IDrawPropertiesContainer drawProperties, CardPresenter presenter, Rectangle bounds, CardModel card) {
			foreach(DrawProperties draw in drawProperties.GetDrawProperties) {
				TextDrawProperties text = draw as TextDrawProperties;
				if(text != null) 
					appearance.DrawString(cache, text.Text, text.Bounds, text.Font, presenter.GetBrush(text.Color), text.Format);
				ImageDrawProperties image = draw as ImageDrawProperties;
				if(image != null)
					cache.Graphics.DrawImage(image.Image, image.Bounds.Location);
			}
		}
	}
	public class CardStringMeasurer : ICardStringMeasurer {
		readonly GraphicsCache cache;
		public CardStringMeasurer(GraphicsCache cache) {
			this.cache = cache;
		}
		public float GetHeight(Font font) {
			return font.GetHeight(cache.Graphics);
		}
		public int GetStringHeight(string text, Font font, int width, StringFormat format) {
			return TextUtils.GetStringHeight(cache.Graphics, text, font, width, format);
		}
		public bool IsStringFit(string text, Font font, int width, StringFormat format) {
			return TextUtils.GetStringSize(cache.Graphics, text, font).Width <= width;
		}
	}
}
