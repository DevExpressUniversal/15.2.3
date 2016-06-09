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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.ExpressApp.EasyTest.Utils;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Templates.ActionControls;
using DevExpress.ExpressApp.Utils;
using DevExpress.XtraBars;
namespace DevExpress.ExpressApp.Win.Templates.Bars.ActionControls {
	[DXToolboxItem(false)]
	[DesignTimeVisible(false)]
	[DebuggerDisplay("{ActionId,nq}")]
	public abstract class BarItemActionControl<TBarItem> : Component, IActionControl, ISupportInitialize, ITestableControl where TBarItem : BarItem {
		private string actionId;
		private TBarItem barItem;
		private string confirmationMessage;
		private int initializing;
		private bool isInitialized;
		private void BarItem_Disposed(object sender, EventArgs e) {
			BarItem.Disposed -= BarItem_Disposed;
			if(NativeControlDisposed != null) {
				NativeControlDisposed(this, EventArgs.Empty);
			}
		}
		protected void CheckCanSet(string propertyName) {
			if(!IsInitializing && !DesignMode) {
				string message = string.Format("Cannot assign the '{0}' value because property initialization must be done between 'BeginInit' and 'EndInit' calls.", propertyName);
				throw new InvalidOperationException(message);
			}
		}
		protected bool IsConfirmed() {
			bool result = true;
			if(!string.IsNullOrEmpty(confirmationMessage)) {
				result = WinApplication.Messaging.GetUserChoice(confirmationMessage, BarItem.Caption, MessageBoxButtons.YesNo) == DialogResult.Yes;
			}
			return result;
		}
		protected virtual void OnEndInit() {
			if(ActionId == null) {
				throw new InvalidOperationException("Cannot initialize the Action Control because its 'ActionId' property is null.");
			}
			if(BarItem == null) {
				string message = string.Format("Cannot initialize the '{0}' Action Control because its 'BarItem' property is null.", ActionId);
				throw new InvalidOperationException(message);
			}
		}
		protected virtual void UpdateCaption() {
			BarItem.Caption = Caption;
		}
		protected virtual void UpdateImage() {
			BarItem.Glyph = GetImage(ImageName);
			BarItem.LargeGlyph = GetLargeImage(ImageName);
		}
		protected virtual void UpdatePaintStyle() {
			switch(PaintStyle) {
				case ActionItemPaintStyle.Default:
					BarItem.PaintStyle = BarItemPaintStyle.Standard;
					break;
				case ActionItemPaintStyle.Caption:
					BarItem.PaintStyle = BarItemPaintStyle.Caption;
					break;
				case ActionItemPaintStyle.CaptionAndImage:
					BarItem.PaintStyle = BarItemPaintStyle.CaptionGlyph;
					break;
				case ActionItemPaintStyle.Image:
					BarItem.PaintStyle = BarItemPaintStyle.Standard;
					break;
			}
		}
		protected virtual Image GetImage(string imageName) {
			ImageInfo imageInfo = ImageLoader.Instance.GetImageInfo(imageName);
			return !imageInfo.IsEmpty ? imageInfo.Image : null;
		}
		protected virtual Image GetLargeImage(string imageName) {
			ImageInfo imageInfo = ImageLoader.Instance.GetLargeImageInfo(imageName);
			return !imageInfo.IsEmpty ? imageInfo.Image : null;
		}
		protected BarItemActionControl() { }
		protected BarItemActionControl(string actionId, TBarItem barItem) {
			BeginInit();
			ActionId = actionId;
			BarItem = barItem;
			EndInit();
		}
		public void BeginInit() {
			if(IsInitialized) {
				string message = String.Format("The 'BeginInit' method call is invalid for the '{0}' Action Control because it is already initialized.", ActionId);
				throw new InvalidOperationException(message);
			}
			initializing++;
		}
		public void EndInit() {
			initializing--;
			if(!IsInitializing) {
				OnEndInit();
				isInitialized = true;
			}
		}
		public virtual void SetVisible(bool visible) {
			BarItem.Visibility = visible ? BarItemVisibility.Always : BarItemVisibility.Never;
		}
		public virtual void SetEnabled(bool enabled) {
			BarItem.Enabled = enabled;
		}
		public void SetCaption(string caption) {
			Caption = caption;
			UpdateCaption();
		}
		public virtual void SetToolTip(string toolTip) {
			BarItem.Hint = toolTip;
		}
		public void SetImage(string imageName) {
			ImageName = imageName;
			UpdateImage();
		}
		public virtual void SetConfirmationMessage(string confirmationMessage) {
			this.confirmationMessage = confirmationMessage;
		}
		public virtual void SetShortcut(string shortcutString) {
			BarItem.ItemShortcut = ShortcutHelper.ParseBarShortcut(shortcutString);
		}
		public void SetPaintStyle(ActionItemPaintStyle paintStyle) {
			PaintStyle = paintStyle;
			UpdatePaintStyle();
		}
		protected bool IsInitializing {
			get { return initializing != 0; }
		}
		protected bool IsInitialized {
			get { return isInitialized; }
		}
		protected string Caption { get; set; }
		protected string ImageName { get; set; }
		protected ActionItemPaintStyle PaintStyle { get; set; }
		[Description("Specifies the Action Control's identifier.")]
		public string ActionId {
			get { return actionId; }
			set {
				if(ActionId != value) {
					CheckCanSet("ActionId");
					actionId = value;
				}
			}
		}
		[Browsable(false)]
		public TBarItem BarItem {
			get { return barItem; }
			set {
				if(BarItem != value) {
					if(BarItem != null) {
						BarItem.Disposed -= BarItem_Disposed;
					}
					CheckCanSet("BarItem");
					barItem = value;
					if(BarItem != null) {
						BarItem.Disposed += BarItem_Disposed;
					}
				}
			}
		}
		[Description("Indicates the Action Control's underlying control.")]
		public object NativeControl {
			get { return BarItem; }
		}
		public event EventHandler NativeControlDisposed;
		void ITestableControl.SetTestName(string testName) { 
			string name = testName;
			if(ActionId == DevExpress.ExpressApp.Win.SystemModule.ExitController.ExitActionId) {
				name = ActionId;
			}
			BarItem.Name = name;
		}
	}
}
