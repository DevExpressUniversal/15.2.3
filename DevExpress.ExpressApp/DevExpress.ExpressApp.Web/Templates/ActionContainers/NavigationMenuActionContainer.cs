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

using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.Web;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Actions;
using System.Collections.ObjectModel;
using DevExpress.ExpressApp.Utils;
using System.ComponentModel;
using DevExpress.ExpressApp.Web.Templates.ActionContainers.Menu;
namespace DevExpress.ExpressApp.Web.Templates.ActionContainers {
	public class MenuChoiceActionItem : ChoiceActionItemWrapper, IClientClickScriptProvider, ISupportActionProcessing {
		private XafMenuItem menuItem;
		private SingleChoiceAction action;
		public override void SetImageName(string imageName) {
			ImageInfo imageInfo = ImageLoader.Instance.GetImageInfo(imageName);
			ASPxImageHelper.SetImageProperties(menuItem.Image, imageInfo, imageName);
			if(!imageInfo.IsUrlEmpty) {
				if(imageInfo.Width < 32 || imageInfo.Height < 29) {
					menuItem.ItemStyle.CssClass = "smallImage";
				}
			}
		}
		public override void SetCaption(string caption) {
			menuItem.Text = caption;
		}
		public override void SetData(object data) { }
		public override void SetShortcut(string shortcutString) { }
		public override void SetEnabled(bool enabled) {
			menuItem.ClientEnabled = enabled;
			menuItem.Enabled = true;
		}
		public override void SetVisible(bool visible) {
			menuItem.ClientVisible = visible;
		}
		public override void SetToolTip(string toolTip) {
			menuItem.ToolTip = toolTip;
		}
		public MenuChoiceActionItem(ChoiceActionItem actionItem, ChoiceActionBase action) : this(actionItem, action, true) { }
		public MenuChoiceActionItem(ChoiceActionItem actionItem, ChoiceActionBase action, bool showImage)
			: base(actionItem, action) {
			ShowImage = showImage;
			menuItem = new XafMenuItem(this, this);
			menuItem.Name = actionItem.Id;
			SyncronizeWithItem();
			menuItem.BeginGroup = actionItem.BeginGroup;
			this.action = (SingleChoiceAction)action;
		}
		public XafMenuItem MenuItem {
			get { return menuItem; }
		}
		#region IClientClickScriptProvider Members
		public string GetScript(XafCallbackManager callbackManager, string controlID, string indexPath) {
			return GetScript((IXafCallbackManager)callbackManager, controlID, indexPath);
		}
		internal string GetScript(IXafCallbackManager callbackManager, string controlID, string indexPath) {
			if(!action.ShowItemsOnClick || menuItem.Items.Count == 0) {
				return callbackManager.GetScript(controlID, string.Format("'{0}'", indexPath), action.GetFormattedConfirmationMessage(), action.Model.GetValue<bool>("IsPostBackRequired"));
			}
			return "";
		}
		#endregion
		#region ISupportActionProcessing Members
		public void ProcessAction() {
			action.DoExecute(this.ActionItem);
		}
		public ActionBase Action {
			get { return action; }
		}
		#endregion
	}
}
