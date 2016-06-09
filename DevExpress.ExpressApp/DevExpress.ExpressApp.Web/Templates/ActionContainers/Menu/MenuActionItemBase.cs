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
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.Web;
#if DebugTest
using DevExpress.ExpressApp.Tests;
#endif
using DevExpress.ExpressApp.Actions;
namespace DevExpress.ExpressApp.Web.Templates.ActionContainers.Menu {
	public abstract class MenuActionItemBase : WebActionBaseItem, ISupportActionProcessing
#if DebugTest
, IActionBaseItemUnitTestable
#endif
 {
		private XafMenuItem menuItem;
		private bool useLargeImage;
		public abstract void SetClientClickHandler(XafCallbackManager callbackManager, string controlID);
		protected virtual XafMenuItem CreateMenuItem() {
			return new XafMenuItem(this, this as IClientClickScriptProvider);
		}
		protected bool IsPostBackRequired {
			get { return Action.Model.GetValue<bool>("IsPostBackRequired"); }
		}
		protected override void SetEnabled(bool enabled) {
			MenuItem.ClientEnabled = enabled;
		}
		protected override void SetVisible(bool visible) {
			MenuItem.ClientVisible = visible;
			MenuItem.Visible = visible;
		}
		protected override void SetCaption(string caption) {
			MenuItem.Text = caption;
		}
		protected override void SetToolTip(string toolTip) {
			MenuItem.ToolTip = toolTip;
		}
		protected override ExpressApp.Utils.ImageInfo GetImageInfo(ExpressApp.Templates.ActionItemPaintStyle paintStyle) {
			if(!UseLargeImage) {
				return base.GetImageInfo(paintStyle);
			}
			else {
				return DevExpress.ExpressApp.Utils.ImageLoader.Instance.GetLargeImageInfo(Action.ImageName);
			}
		}
		protected override ExpressApp.Templates.ActionItemPaintStyle GetDefaultPaintStyle() {
			if(WebApplicationStyleManager.IsNewStyle) {
				return ExpressApp.Templates.ActionItemPaintStyle.Image;
			}
			else {
				return base.GetDefaultPaintStyle();
			}
		}
		protected override void SetConfirmationMessage(string message) { }
		protected override void SetImage(DevExpress.ExpressApp.Utils.ImageInfo imageInfo) {
			if(!imageInfo.IsUrlEmpty) {
				ASPxImageHelper.SetImageProperties(MenuItem.Image, imageInfo, imageInfo.ImageName);
				if(imageInfo.Width < 32 || imageInfo.Height < 29) {
					MenuItem.ItemStyle.CssClass = "smallImage";
				}
			}
			else {
				ASPxImageHelper.ClearImageProperties(MenuItem.Image);
			}
		}
		public XafMenuItem MenuItem {
			get {
				if(menuItem == null) {
					menuItem = CreateMenuItem();
				}
				return menuItem;
			}
		}
		public bool UseLargeImage {
			get {
				return useLargeImage;
			}
			set {
				if(useLargeImage != value) {
					useLargeImage = value;
					SynchronizeWithActionPaintStyleDependentProperties();
				}
			}
		}
		public MenuActionItemBase(ActionBase action)
			: base(action) {
			MenuItem.Name = action.Id;
			MenuItem.AdaptivePriority = action.Model.GetValue<int>("AdaptivePriority");
			MenuItem.AdaptiveText = action.Model.Caption;
			SynchronizeWithAction();
		}
		#region EasyTest
		public override string TestCaption {
			get {
				return Action.Caption;
			}
		}
		public override string ClientId {
			get { return JSUpdateScriptHelper.GetMenuId(MenuItem.Menu); }
		}
		public override IJScriptTestControl TestControl {
			get { return new JSASPxMenuActionsHolderItem(); }
		}
		#endregion
		#region IActionBaseItemUnitTestable Members
#if DebugTest
		bool IActionBaseItemUnitTestable.ControlVisible {
			get { return MenuItem.ClientVisible; }
		}
		bool IActionBaseItemUnitTestable.ControlEnabled {
			get { return MenuItem.ClientEnabled; }
		}
		string IActionBaseItemUnitTestable.ControlCaption {
			get { return MenuItem.Text; }
		}
		string IActionBaseItemUnitTestable.ControlToolTip {
			get { return MenuItem.ToolTip; }
		}
		bool IActionBaseItemUnitTestable.SupportPaintStyle {
			get { return true; }
		}
		bool IActionBaseItemUnitTestable.ImageVisible {
			get { return !MenuItem.Image.IsEmpty; }
		}
		bool IActionBaseItemUnitTestable.CaptionVisible {
			get { return !string.IsNullOrEmpty(MenuItem.Text); }
		}
#endif
		#endregion
		#region ISupportActionProcessing Members
		public abstract void ProcessAction();
		#endregion
	}
}
