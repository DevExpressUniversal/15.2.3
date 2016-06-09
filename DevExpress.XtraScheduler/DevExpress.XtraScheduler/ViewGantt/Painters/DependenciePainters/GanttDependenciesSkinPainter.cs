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

using DevExpress.LookAndFeel;
using DevExpress.Skins;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Drawing {
	public class GanttDependenciesSkinPainter : GanttDependenciesPainter {
		UserLookAndFeel lookAndFeel;
		Image arrowImage;
		Image cornerImage;
		public GanttDependenciesSkinPainter(UserLookAndFeel lookAndFeel) {
			this.lookAndFeel = lookAndFeel;
			this.arrowImage = CalculateArrowImage();
			this.cornerImage = CalculateCornerImage();
		}
		protected internal override Image GetArrowImage() {
			return arrowImage;
		}
		protected internal override int GetArrowWidth() {
			return arrowImage.Width;
		}
		protected internal override Color GetBaseColor() {
			SkinElement element = SkinPainterHelper.GetSkinElement(lookAndFeel, SchedulerSkins.SkinDependency);
			if (element == null || element.Properties == null)
				return base.GetBaseColor();
			Color color = element.Properties.GetColor(SchedulerSkins.PropBaseColor);
			if (color == Color.Empty)
				return base.GetBaseColor();
			return color;
		}
		protected internal override Image GetCornerImage() {
			return cornerImage;
		}
		protected internal override int GetConnectorIndent() {
			int cornerWidth = cornerImage == null ? 0 : cornerImage.Width;
			return GetArrowWidth() + cornerWidth + GanttDependenciesPainter.ViewInfoMinLenght;
		}
		protected internal override Color GetDependencyColor() {
			SkinElement element = SkinPainterHelper.GetSkinElement(lookAndFeel, SchedulerSkins.SkinDependency);
			if (element == null)
				return Color.Empty;
			Color color = element.Color.GetForeColor();
			if (color == Color.Empty)
				return Color.Empty;
			int colorOpacity = element.Properties.GetInteger(SchedulerSkins.PropOpacity);
			return CreateColor(colorOpacity, color);
		}
		protected internal override Color GetSelectedDependencyColor() {
			SkinElement element = SkinPainterHelper.GetSkinElement(lookAndFeel, SchedulerSkins.SkinDependency);
			if (element == null)
				return Color.Empty;
			SkinProperties properties = element.Properties;
			Color selectedColor = properties.GetColor(SchedulerSkins.PropSelectedColor);
			if (selectedColor == Color.Empty)
				return Color.Empty;
			int selectedColorOpacity = properties.GetInteger(SchedulerSkins.PropSelectedOpacity);
			return CreateColor(selectedColorOpacity, selectedColor);
		}
		Image CalculateArrowImage() {
			SkinElement element = SkinPainterHelper.GetSkinElement(lookAndFeel, SchedulerSkins.SkinDependency);
			if (element == null || element.Image == null)
				return base.GetArrowImage();
			return element.Image.Image;
		}
		Image CalculateCornerImage() {
			SkinElement element = SkinPainterHelper.GetSkinElement(lookAndFeel, SchedulerSkins.SkinDependencyCorner);
			if (element == null || element.Image == null)
				return base.GetCornerImage();
			return element.Image.Image;
		}
		Color CreateColor(int opacity, Color color) {
			return Color.FromArgb(opacity, color.R, color.G, color.B);
		}
	}
}
