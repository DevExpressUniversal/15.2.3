#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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

using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Templates.ActionContainers {
	public class TabPageChoiceActionItem : ChoiceActionItemWrapper {
		private TabPage tabPage;
		public override void SetImageName(string imageName) {
			if(ShowImage) {
				ASPxImageHelper.SetImageProperties(tabPage.TabImage, imageName);
			}
			else {
				ASPxImageHelper.ClearImageProperties(tabPage.TabImage);
			}
		}
		public override void SetCaption(string caption) {
			tabPage.Text = caption;
		}
		public override void SetData(object data) { }
		public override void SetShortcut(string shortcutString) { }
		public override void SetEnabled(bool enabled) {
			tabPage.Enabled = enabled;
		}
		public override void SetVisible(bool visible) {
			tabPage.Visible = visible;
		}
		public override void SetToolTip(string toolTip) {
			tabPage.ToolTip = toolTip;
		}
		public TabPageChoiceActionItem(ChoiceActionItem actionItem, ChoiceActionBase action, bool showImages)
			: base(actionItem, action) {
			tabPage = new TabPage();
			ShowImage = showImages;
			SyncronizeWithItem();
		}
		public TabPage TabPage {
			get { return tabPage; }
		}
	}
}
