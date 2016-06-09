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
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.Skins;
using DevExpress.UserSkins;
using DevExpress.Utils.Design;
namespace DevExpress.DashboardWin.Design {
	public class VSLookAndFeelService : LookAndFeelService {
		static VSLookAndFeelService() {
			BonusSkins.Register();
			SkinManager.EnableFormSkins();
		}
		public static void SetControlLookAndFeel(IServiceProvider provider, Control control, bool allowColorsChanging) {
			ISupportLookAndFeel supportLookAndFeel = control as ISupportLookAndFeel;
			UserLookAndFeel userLookAndFeel = GetLookAndFeel(provider);
			if(supportLookAndFeel != null)
				supportLookAndFeel.LookAndFeel.ParentLookAndFeel = userLookAndFeel;
			if(allowColorsChanging) {
				Skin skin = CommonSkins.GetSkin(userLookAndFeel);
				control.BackColor = skin.Colors.GetColor(CommonColors.Control);
				control.ForeColor = skin.Colors.GetColor(CommonColors.ControlText);
			}
		}
		public static UserLookAndFeel GetLookAndFeel(IServiceProvider provider) {
			ILookAndFeelService lookAndFeelService = provider.GetService(typeof(ILookAndFeelService)) as ILookAndFeelService;
			return (lookAndFeelService != null) ? lookAndFeelService.LookAndFeel : null;
		}
		public VSLookAndFeelService(IServiceProvider servProvider)
			: base() {
			LookAndFeel.UseDefaultLookAndFeel = false;
			LookAndFeel.SkinName = VSThemeHelper.GetAppropriateSkinName(servProvider);
		}
	}
}
