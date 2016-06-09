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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.Localization;
namespace DevExpress.XtraPivotGrid.Customization {
	public class CustomizationFormLayoutButton : SimpleButton {
		#region statics
		static PivotGridStringId[] layoutIds = new PivotGridStringId[] { 
			PivotGridStringId.CustomizationFormStackedDefault, 
			PivotGridStringId.CustomizationFormStackedSideBySide, 
			PivotGridStringId.CustomizationFormTopPanelOnly, 
			PivotGridStringId.CustomizationFormBottomPanelOnly2by2, 
			PivotGridStringId.CustomizationFormBottomPanelOnly1by4 };
		static ImageCollection menuImages = null;
		protected static ImageCollection MenuImages {
			get {
				if(menuImages == null)
					menuImages = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources(
						"DevExpress.XtraPivotGrid.Images.customization2007menu.png",
						typeof(CustomizationFormLayoutButton).Assembly, new Size(32, 32));
				return menuImages;
			}
		}
		#endregion
		CustomizationForm customizationForm;
		DXPopupMenu menu;
		public CustomizationFormLayoutButton(CustomizationForm owner)
			: base() {
			this.customizationForm = owner;
			Image = CustomizationForm.Icons.Images[0];
			ImageLocation = ImageLocation.MiddleCenter;
			ToolTip = PivotGridLocalizer.Active.GetLocalizedString(PivotGridStringId.CustomizationFormLayoutButtonTooltip);
			AllowFocus = false;
			IsDefault = false;
		}
		protected CustomizationForm CustomizationForm {
			get { return customizationForm; }
		}
		protected internal DXPopupMenu Menu {
			get {
				if(menu == null)
					menu = CreateMenu();
				return menu;
			}
		}
		protected PivotGridViewInfoData Data {
			get { return CustomizationForm.Data; }
		}
		protected string GetMenuItemText(CustomizationFormLayout layout) {
			int layoutIndex = (int)layout;
			if(layoutIndex < 0 || layoutIndex >= layoutIds.Length)
				throw new ArgumentOutOfRangeException("layout");
			return PivotGridLocalizer.Active.GetLocalizedString(layoutIds[layoutIndex]);
		}
		protected virtual DXPopupMenu CreateMenu() {
			DXPopupMenu res = new DXPopupMenu();
			for(int i = 0; i < CustomizationForm.SupportedLayouts.Count; i++) {
				CustomizationFormLayout layout = CustomizationForm.SupportedLayouts[i];
				if(!CustomizationFormEnumExtensions.IsLayoutAllowed(CustomizationForm.Data.OptionsCustomization.CustomizationFormAllowedLayouts, layout))
					continue;
				DXMenuItem item = new DXMenuItem(GetMenuItemText(layout));
				item.Tag = layout;
				item.Click += new EventHandler(OnMenuItemClick);
				item.Image = MenuImages.Images[i];
				res.Items.Add(item);
			}
			return res;
		}
		void OnMenuItemClick(object sender, EventArgs e) {
			DXMenuItem item = (DXMenuItem)sender;
			CustomizationFormLayout layout = (CustomizationFormLayout)item.Tag;
			CustomizationForm.FormLayout = layout;
		}
		protected override void OnClick(EventArgs e) {
			base.OnClick(e);
			Point p = new Point(0, Height);
			MenuManagerHelper.ShowMenu(Menu, Data.ActiveLookAndFeel, Data.MenuManager, this, p);
		}
	}
}
