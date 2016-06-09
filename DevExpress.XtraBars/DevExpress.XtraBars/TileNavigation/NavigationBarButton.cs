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

using DevExpress.Utils;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ButtonPanel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraBars.Navigation {
	public class NavigationBarButtonsPanel : BaseButtonsPanel {
		public NavigationBarButtonsPanel(IButtonsPanelOwner owner)
			: base(owner) {
			Orientation = (((OfficeNavigationBar)owner).ControlCore as ITileControlProperties).Orientation;
			ContentAlignment = System.Drawing.ContentAlignment.MiddleCenter;
		}
		public Size ButtonSize { get { return GetPanelSize(); } }
		Size GetPanelSize() {
			if(Owner is OfficeNavigationBar) {
				OfficeNavigationBar bar = Owner as OfficeNavigationBar;
				if(bar.Orientation == System.Windows.Forms.Orientation.Horizontal)
					return new Size(0, bar.ContentBounds.Height);
				else
					return new Size(bar.ContentBounds.Width, 0);
			}
			return new Size(0, 0);
		}
		protected override IButtonsPanelViewInfo CreateViewInfo() {
			return new NavigationBarButtonsPanelViewInfo(this);
		}
		protected override void RaiseButtonClick(IBaseButton button) {
			if(button is IButton) RaiseButtonClick(button as IButton);
		}
		protected void RaiseButtonClick(IButton button) {
			if(ButtonClick != null)
				ButtonClick(this, new ButtonEventArgs(button));
		}
		public event ButtonEventHandler ButtonClick;
	}
	public class NavigationBarButton : DevExpress.XtraEditors.ButtonPanel.BaseButton, IButton {
		protected override AppearanceObject CreateAppearance() {
			return new AppearanceObject();
		}
	}
	class NavigationBarCustomizationButton : DevExpress.XtraEditors.ButtonPanel.BaseButton, IButton, IDefaultButton {
		public NavigationBarCustomizationButton(OfficeNavigationBar owner)
			: base() {
			this.ownerCore = owner;
			var image =  ResourceImageHelper.CreateImageFromResources("DevExpress.XtraBars.TileNavigation.Images.NavigationBarCustomizationButtonImage.png", typeof(NavigationBarCustomizationButton).Assembly);
			Image = image;
			Caption = string.Empty;
		}
		OfficeNavigationBar ownerCore;
		protected internal OfficeNavigationBar Owner { get { return ownerCore; } }
		protected override AppearanceObject CreateAppearance() {
			return new AppearanceObject();
		}
	}
}
