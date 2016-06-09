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
using System.Text;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
namespace DevExpress.DashboardWin.Native {
	public class DashboardLayoutControlCaption {
		readonly ItemCaptionViewControl caption;
		bool elementFontBold = true;
		bool drillDownTextBold;
		Color elementFontColor;
		Color drillDownTextColor;
		public ItemCaptionViewControl Caption { get { return caption; } }
		public DashboardLayoutControlCaption(IItemCaption itemCaption) {
			caption = new ItemCaptionViewControl(itemCaption);
		}
		public void UpdateLookAndFeel(UserLookAndFeel lookAndFeel) {
			Skin skin = DashboardSkins.GetSkin(lookAndFeel);
			SkinElement sel = null;
			if(skin != null)
				sel = skin[DashboardSkins.SkinDashboardItemCaptionTop];
			if(sel == null) {
				skin = CommonSkins.GetSkin(lookAndFeel);
				sel = skin[CommonSkins.SkinGroupPanelCaptionTop];
			}
			if(sel != null) {
				elementFontBold = sel.Color.FontBold;
				elementFontColor = sel.Color.GetForeColor();
				drillDownTextBold = sel.Properties.GetBoolean("FontBold2", !elementFontBold);
				drillDownTextColor = sel.Properties.GetColor("ForeColor2", elementFontColor);
				caption.Update();
			}
		}
		public ClientArea CalcCaptionArea(ClientArea viewerArea, int locationY) {
			return new ClientArea {
				Left = viewerArea.Left,
				Top = locationY,
				Width = viewerArea.Width,
				Height = viewerArea.Top - locationY
			};
		}
		public string Update(ItemCaptionContentInfo itemCaptionContentInfo, StringBuilder sb) {
			string elementCaption = itemCaptionContentInfo.CaptionText;
			string drillDownText = itemCaptionContentInfo.FilterValuesText;
			DashboardWinHelper.AppendColoredText(sb, elementFontBold, elementFontColor, elementCaption);
			if(!string.IsNullOrEmpty(drillDownText))
				DashboardWinHelper.AppendColoredText(sb, drillDownTextBold, drillDownTextColor, drillDownText);
			return elementCaption + drillDownText;
		}
		public void UpdateCaption(string caption, IList<FormattableValue> drillDownValues) {
			this.caption.Update(caption, drillDownValues);
		}
	}
}
