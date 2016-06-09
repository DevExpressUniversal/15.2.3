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
using System.Drawing.Drawing2D;
using System.Reflection;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Painters;
using DevExpress.XtraRichEdit.Utils;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
namespace DevExpress.XtraRichEdit.Ruler {
	#region HorizontalRulerPainter (abstract class)
	public abstract class RulerPainterBase : RichEditViewPainter {
		#region Fields
		readonly RulerControlBase ruler;
		#endregion
		protected RulerPainterBase(RulerControlBase ruler)
			: base(ruler.RichEditControl.ActiveView) {
			this.ruler = ruler;
		}
		#region Properties
		protected virtual Color ForeColor { get { return SystemColors.WindowText; } }
		protected internal virtual int VerticalTextPaddingBottom { get { return ruler.PixelsToLayoutUnitsV(2); } } 
		protected internal virtual int VerticalTextPaddingTop { get { return ruler.PixelsToLayoutUnitsV(2); } } 
		protected internal virtual int PaddingTop { get { return ruler.PixelsToLayoutUnitsV(5); } }
		protected internal virtual int PaddingBottom { get { return ruler.PixelsToLayoutUnitsV(5); } }
		#endregion
		protected internal virtual Bitmap GetReportsSkinElementImage(string elementName, int imageIndex) {
			SkinElement element = ReportsSkins.GetSkin(LookAndFeel)[elementName];
			ImageCollection images = element.Image.GetImages();
			return (Bitmap)images.Images[imageIndex];
		}
		protected internal virtual SkinElementInfo GetReportsSkinElement(string elementName, Rectangle bounds) {
			SkinElement element = ReportsSkins.GetSkin(LookAndFeel)[elementName];
			return new SkinElementInfo(element, bounds);
		}
		protected internal virtual SkinElementInfo GetRichEditSkinElement(string elementName, Rectangle bounds) {
			SkinElement element = RichEditSkins.GetSkin(LookAndFeel)[elementName];
			return new SkinElementInfo(element, bounds);
		}
		protected internal virtual void DrawReportsSkinElement(GraphicsCache cache, string elementName, Rectangle bounds) {
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, GetReportsSkinElement(elementName, bounds));
		}
		protected internal virtual void DrawRichEditSkinElement(GraphicsCache cache, string elementName, Rectangle bounds) {
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, GetRichEditSkinElement(elementName, bounds));
		}
		public abstract int CalculateTotalRulerSize(int textSize);
	}
	#endregion
}
