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
using DevExpress.Utils;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
namespace DevExpress.DashboardWin.Native {
	public class DragAreaAppearances : BaseAppearanceCollection {
		static void ApplySkinToAppearance(UserLookAndFeel lookAndFeel, string skinElementName, AppearanceDefault appearance) {
			Skin skin = DashboardSkins.GetSkin(lookAndFeel);
			if (skin != null) {
				SkinElement skinElement = skin[skinElementName];
				if (skinElement != null)
					skinElement.Apply(appearance);
			} 
		}
		Color chartPaneRemoveButtonColor;
		AppearanceObject areaAppearance;
		AppearanceObject groupAppearance;
		AppearanceObject groupOptionsButtonAppearance;
		AppearanceObject itemAppearance;
		AppearanceObject itemOptionsButtonAppearance;
		Color DragItemForeColor { get; set; }
		Color DragItemForeColorHot { get; set; }
		Color DragItemForeColorPressed { get; set; }
		Color DragGroupOptionsButtonForeColor { get; set; }
		Color DragGroupOptionsButtonForeColorHot { get; set; }
		Color DragGroupOptionsButtonForeColorPressed { get; set; }
		public Color ChartPaneRemoveButtonColor { get { return chartPaneRemoveButtonColor; } }
		public AppearanceObject AreaAppearance { get { return areaAppearance; } }
		public AppearanceObject GroupAppearance { get { return groupAppearance; } }
		public AppearanceObject DragAreaButtonAppearance { get { return groupOptionsButtonAppearance; } }
		public AppearanceObject ItemAppearance { get { return itemAppearance; } }
		public AppearanceObject ItemOptionsButtonAppearance { get { return itemOptionsButtonAppearance; } }
		public DragAreaAppearances(UserLookAndFeel lookAndFeel) {
			Update(lookAndFeel);
		}
		protected override void CreateAppearances() {
			base.CreateAppearances();
			areaAppearance = CreateAppearance(DashboardSkins.SkinDragArea);
			groupAppearance = CreateAppearance(DashboardSkins.SkinDragGroup);
			groupOptionsButtonAppearance = CreateAppearance(DashboardSkins.SkinDragGroupOptionsButton);
			itemAppearance = CreateAppearance(DashboardSkins.SkinDragItem);
			itemOptionsButtonAppearance = CreateAppearance(DashboardSkins.SkinDragItemOptionsButton);
		}
		public void Update(UserLookAndFeel lookAndFeel) {
			chartPaneRemoveButtonColor = SystemColors.ControlText;
			AppearanceDefault areaAppearance = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control);
			AppearanceDefault groupAppearance = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, SystemColors.ControlDarkDark);
			AppearanceDefault groupOptionsButtonAppearance = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, SystemColors.ControlDarkDark);
			AppearanceDefault itemAppearance = new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlLight, SystemColors.ControlDarkDark, SystemColors.ControlLightLight);
			AppearanceDefault itemOptionsButtonAppearance = new AppearanceDefault(SystemColors.ControlText, 
				SystemColors.ControlLight, SystemColors.ControlDarkDark, SystemColors.ControlLightLight);
			if (lookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) {
				ApplySkinToAppearance(lookAndFeel, DashboardSkins.SkinDragArea, areaAppearance);
				ApplySkinToAppearance(lookAndFeel, DashboardSkins.SkinDragGroup, groupAppearance);
				ApplySkinToAppearance(lookAndFeel, DashboardSkins.SkinDragGroupOptionsButton, groupOptionsButtonAppearance);
				ApplySkinToAppearance(lookAndFeel, DashboardSkins.SkinDragItem, itemAppearance);
				ApplySkinToAppearance(lookAndFeel, DashboardSkins.SkinDragItemOptionsButton, itemOptionsButtonAppearance);
			}
			GetAppearance(DashboardSkins.SkinDragArea).Assign(areaAppearance);
			GetAppearance(DashboardSkins.SkinDragGroup).Assign(groupAppearance);
			GetAppearance(DashboardSkins.SkinDragGroupOptionsButton).Assign(groupOptionsButtonAppearance);
			GetAppearance(DashboardSkins.SkinDragItem).Assign(itemAppearance);
			GetAppearance(DashboardSkins.SkinDragItemOptionsButton).Assign(itemOptionsButtonAppearance);
			Skin skin = DashboardSkins.GetSkin(lookAndFeel);
			if (skin != null) {
				chartPaneRemoveButtonColor = skin.Colors.GetColor("ChartPaneRemoveButton", chartPaneRemoveButtonColor);
				SkinElement skinElement = skin[DashboardSkins.SkinDragItem];
				if (skinElement != null) {
					DragItemForeColor = itemAppearance.ForeColor;
					DragItemForeColorHot = skinElement.Properties.GetColor("ColorForeColorHot");
					DragItemForeColorPressed = skinElement.Properties.GetColor("ColorForeColorPressed");
				}
				else {
					DragItemForeColor = itemAppearance.ForeColor;
					DragItemForeColorHot = itemAppearance.ForeColor;
					DragItemForeColorPressed = itemAppearance.ForeColor;
				}
				skinElement = skin[DashboardSkins.SkinDragGroupOptionsButton];
				if (skinElement != null) {
					DragGroupOptionsButtonForeColor = groupOptionsButtonAppearance.ForeColor;
					DragGroupOptionsButtonForeColorHot = skinElement.Properties.GetColor("ColorForeColorHot");
					DragGroupOptionsButtonForeColorPressed = skinElement.Properties.GetColor("ColorForeColorPressed");
				}
				else {
					DragGroupOptionsButtonForeColor = groupOptionsButtonAppearance.ForeColor;
					DragGroupOptionsButtonForeColorHot = groupOptionsButtonAppearance.ForeColor;
					DragGroupOptionsButtonForeColorPressed = groupOptionsButtonAppearance.ForeColor;
				}
			} 
		}
		 public AppearanceObject GetItemAppearance(DragItemState state) {
			switch (state) {
				case DragItemState.Hot:
					itemAppearance.ForeColor = DragItemForeColorHot;
					break;
				case DragItemState.Selected:
					itemAppearance.ForeColor = DragItemForeColorPressed;
					break;
				default:
					itemAppearance.ForeColor = DragItemForeColor;
					break;
			}
			return itemAppearance;
		}
		 public  Image GetColoredDragAreaButtonGlyph(Image glyph, DragAreaButtonState optionsButtonState) {
			switch (optionsButtonState) {
				case DragAreaButtonState.Normal:
					return ImageHelper.ColorBitmap(glyph, DragGroupOptionsButtonForeColor);
				case DragAreaButtonState.Hot:
					return ImageHelper.ColorBitmap(glyph, DragGroupOptionsButtonForeColorHot);
				case DragAreaButtonState.Selected:
					return ImageHelper.ColorBitmap(glyph, DragGroupOptionsButtonForeColorPressed);
				case DragAreaButtonState.Disabled:
					return ImageHelper.ColorBitmap(glyph, DragGroupOptionsButtonForeColor, 0.35f);
			}				 
			return glyph;
		 }
	}
}
