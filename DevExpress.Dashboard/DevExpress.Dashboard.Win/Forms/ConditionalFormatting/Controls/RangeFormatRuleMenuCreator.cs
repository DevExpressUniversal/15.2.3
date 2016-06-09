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
using System.Drawing.Drawing2D;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Localization;
using DevExpress.LookAndFeel;
namespace DevExpress.DashboardWin.Native {
	abstract class RangeSetFormatRuleMenuCreatorBase : GroupedMenuCreator<FormatConditionRangeSetTypeGroups, FormatConditionRangeSetPredefinedType> {
		public RangeSetFormatRuleMenuCreatorBase(UserLookAndFeel lookAndFeel) : base(lookAndFeel) {
		}
		protected override string GetDescription() {
			return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeSetDescription);
		}
		protected override string GetGroupCaption(FormatConditionRangeSetTypeGroups group) {
			return group.Localize();
		}
		protected override string GetTypeCaption(FormatConditionRangeSetPredefinedType type) {
			return type.Localize();
		}
		protected override string GetTypeDescription(FormatConditionRangeSetPredefinedType type) {
			return type.Descript();
		}
	}
	class RangeIconSetFormatRuleMenuCreator : RangeSetFormatRuleMenuCreatorBase {
		public RangeIconSetFormatRuleMenuCreator(UserLookAndFeel lookAndFeel)
			: base(lookAndFeel) {
		}
		protected override IList<FormatConditionRangeSetPredefinedType> GetTypes(FormatConditionRangeSetTypeGroups group) {
			return group.ToRangeIconTypes();
		}
		protected override int GetItemsCount(FormatConditionRangeSetPredefinedType type) {
			return 8 - type.ToIconTypes().Count;
		}
		protected override Image GetIconImage(FormatConditionRangeSetPredefinedType type) {
			const int iconIndent = 1;
			IList<FormatConditionIconType> iconTypes = type.ToIconTypes();
			if(iconTypes == null || iconTypes.Count == 0)
				return null;
			Image firstImage = iconTypes[0].ToImage(Scheme);
			if(firstImage == null)
				throw new ArgumentException("Range icon type is undefined");
			Size iconSize = firstImage.Size;
			Bitmap iconsImage = new Bitmap((iconSize.Width + iconIndent) * iconTypes.Count - iconIndent, iconSize.Height);
			using(Graphics g = Graphics.FromImage(iconsImage)) {
				for(int i = 0; i < iconTypes.Count; i++)
					g.DrawImage(iconTypes[i].ToImage(Scheme), i * (iconSize.Width + iconIndent), 0);
			}
			return iconsImage;
		}
	}
	class RangeColorSetFormatRuleMenuCreator : RangeSetFormatRuleMenuCreatorBase {
		public RangeColorSetFormatRuleMenuCreator(UserLookAndFeel lookAndFeel)
			: base(lookAndFeel) {
		}
		protected override IList<FormatConditionRangeSetPredefinedType> GetTypes(FormatConditionRangeSetTypeGroups group) {
			return group.ToRangeColorTypes();
		}
		protected override int GetItemsCount(FormatConditionRangeSetPredefinedType type) {
			return 8 - type.ToColorTypes().Count;
		}
		protected override Image GetIconImage(FormatConditionRangeSetPredefinedType type) {
			const int iconSize = 16;
			const int iconIndent = 2;
			IList<FormatConditionAppearanceType> colorTypes = type.ToColorTypes();
			if(colorTypes == null || colorTypes.Count == 0)
				return null;
			Bitmap iconsImage = new Bitmap((iconSize + iconIndent) * colorTypes.Count - iconIndent, iconSize);
			using(Graphics g = Graphics.FromImage(iconsImage)) {
				for(int i = 0; i < colorTypes.Count; i++) {
					AppearanceSettings settings = (AppearanceSettings)colorTypes[i].ToAppearanceSettings(Scheme);
					Rectangle bounds = new Rectangle(i * (iconSize + iconIndent), 0, iconSize, iconSize);
					using(Brush brush = new SolidBrush(settings.BackColor.Value)) {
						g.FillRectangle(brush, bounds);
						DrawBorder(g, bounds);
					}
				}
			}
			return iconsImage;
		}
	}
	class RangeGradientFormatRuleMenuCreator : GroupedMenuCreator<FormatConditionRangeGradientTypeGroups, FormatConditionRangeGradientPredefinedType> {
		public RangeGradientFormatRuleMenuCreator(UserLookAndFeel lookAndFeel)
			: base(lookAndFeel) {
		}
		protected override string GetDescription() {
			return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeGradientDescription);
		}
		protected override string GetGroupCaption(FormatConditionRangeGradientTypeGroups group) {
			return group.Localize();
		}
		protected override string GetTypeCaption(FormatConditionRangeGradientPredefinedType type) {
			return type.Localize();
		}
		protected override string GetTypeDescription(FormatConditionRangeGradientPredefinedType type) {
			return type.Descript();
		}
		protected override IList<FormatConditionRangeGradientPredefinedType> GetTypes(FormatConditionRangeGradientTypeGroups group) {
			return group.ToRangeGradientTypes();
		}
		protected override int GetItemsCount(FormatConditionRangeGradientPredefinedType type) {
			return 6 - type.ToColors(Scheme).Count;
		}
		protected override Image GetIconImage(FormatConditionRangeGradientPredefinedType type) {
			const int iconSize = 20;
			IList<Color> colors = type.ToColors(Scheme);
			int count = colors == null ? 0 : colors.Count;
			if(count == 0)
				return null;
			Bitmap iconsImage = new Bitmap(iconSize * count, iconSize);
			using(Graphics g = Graphics.FromImage(iconsImage)) {
				Rectangle bounds = new Rectangle(0, 0, iconsImage.Width, iconsImage.Height);
				ColorBlend colorBlend = new ColorBlend(count);
				for(int i = 0; i < count; i++) {
					colorBlend.Positions[i] = (float)i / (count - 1);
					colorBlend.Colors[i] = colors[i];
				}
				using(LinearGradientBrush brush = new LinearGradientBrush(bounds, Color.Empty, Color.Empty, 0, false)) {
					brush.InterpolationColors = colorBlend;
					g.FillRectangle(brush, bounds);
					DrawBorder(g, bounds);
				}
			}
			return iconsImage;
		}
	}
}
