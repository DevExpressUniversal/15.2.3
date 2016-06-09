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
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Templates {
	public enum ActionItemPaintStyle { Default, Caption, CaptionAndImage, Image }
	public static class ActionItemPaintStyleMerger {
		public static ActionItemPaintStyle Merge(ActionItemPaintStyle minorPaintStyle, ActionItemPaintStyle majorPaintStyle) {
			if(majorPaintStyle == ActionItemPaintStyle.Default) {
				return minorPaintStyle;
			}
			return majorPaintStyle;
		}
	}
	public abstract class ActionBaseItem : IDisposableExt {
		private Boolean isDisposed;
		private ISelectionContext currentContext;
		private ActionBase action;
		private string actionId;
		private void OnSelectionContextChanged() {
			GuardNotDisposed();
			if(currentContext != null) {
				currentContext.SelectionTypeChanged -= new EventHandler(currentContext_SelectionTypeChanged);
			}
			currentContext = action.SelectionContext;
			if(currentContext != null) {
				currentContext.SelectionTypeChanged += new EventHandler(currentContext_SelectionTypeChanged);
			}
		}
		private void currentContext_SelectionTypeChanged(object sender, EventArgs e) {
			if(isDisposed) {
				return; 
			}
			SetVisible(IsVisible);
		}
		private string GetCaption(ActionItemPaintStyle paintStyle) {
			if(paintStyle == ActionItemPaintStyle.Caption || paintStyle == ActionItemPaintStyle.CaptionAndImage ||
				!ImageExist(Action.ImageName)) {
				GuardNotDisposed();
				return Action.Caption;
			}
			else {
				return String.Empty;
			}
		}
		protected virtual ImageInfo GetImageInfo(ActionItemPaintStyle paintStyle) {
			if(paintStyle == ActionItemPaintStyle.Image || paintStyle == ActionItemPaintStyle.CaptionAndImage) {
				return GetImageInfo();
			}
			else {
				return ImageInfo.Empty;
			}
		}
		protected virtual ImageInfo GetImageInfo() {
			GuardNotDisposed();
			return ImageLoader.Instance.GetImageInfo(Action.ImageName);
		}
		protected virtual bool ImageExist(string imageName) {
			return !string.IsNullOrEmpty(imageName);
		}
		protected void GuardNotDisposed() {
			Guard.NotDisposed(this, new string[] { "actionId", actionId });
		}
		protected virtual ActionItemPaintStyle GetPaintStyle() {
			GuardNotDisposed();
			return ActionItemPaintStyleMerger.Merge(GetDefaultPaintStyle(), action.PaintStyle);
		}
		protected void SynchronizeWithActionPaintStyleDependentProperties() {
			ActionItemPaintStyle paintStyle = GetPaintStyle();
			SetCaption(GetCaption(paintStyle));
			SetImage(GetImageInfo(paintStyle));
		}
		protected virtual ActionItemPaintStyle GetDefaultPaintStyle() {
			return ActionItemPaintStyle.CaptionAndImage;
		}
		protected virtual void SynchronizeWithActionCore() {
			GuardNotDisposed();
			SetToolTip(action.GetTotalToolTip());
			SetEnabled(action.Enabled);
			SetVisible(IsVisible);
			SynchronizeWithActionPaintStyleDependentProperties();
			SetConfirmationMessage(action.GetFormattedConfirmationMessage());
			SetShortcut(action.Model.Shortcut);
			SetPaintStyle(GetPaintStyle());
		}
		protected void SynchronizeWithAction() {
			if(Action == null) {  
				return;
			}
			SynchronizeWithActionCore();
		}
		protected abstract void SetEnabled(bool enabled);
		protected abstract void SetVisible(bool visible);
		protected abstract void SetCaption(string caption);
		protected abstract void SetToolTip(string toolTip);
		protected abstract void SetImage(ImageInfo imageInfo);
		protected abstract void SetConfirmationMessage(string message);
		protected abstract void SetShortcut(string shortcutString);
		protected virtual void SetPaintStyle(ActionItemPaintStyle paintStyle) {
			SynchronizeWithActionPaintStyleDependentProperties();
		}
		public ActionBaseItem(ActionBase action) {
			Guard.ArgumentNotNull(action, "action");
			this.action = action;
			this.actionId = action.Id;
			action.Changed += new EventHandler<ActionChangedEventArgs>(Action_Changed);
			OnSelectionContextChanged();
		}
		private void Action_Changed(object sender, ActionChangedEventArgs e) {
			if(isDisposed) {
				return; 
			}
			ActionChangedType eChangedPropertType = e.ChangedPropertyType;
			switch(e.ChangedPropertyType) {
				case ActionChangedType.Caption: {
						SetCaption(GetCaption(GetPaintStyle()));
						break;
					}
				case ActionChangedType.Enabled: {
						SetEnabled(action.Enabled);
						break;
					}
				case ActionChangedType.Active: {
						SetVisible(IsVisible);
						break;
					}
				case ActionChangedType.ToolTip: {
						SetToolTip(action.GetTotalToolTip());
						break;
					}
				case ActionChangedType.Image: {
						SetImage(GetImageInfo(GetPaintStyle()));
						break;
					}
				case ActionChangedType.ConfirmationMessage: {
						SetConfirmationMessage(action.ConfirmationMessage);
						break;
					}
				case ActionChangedType.SelectionContext: {
						OnSelectionContextChanged();
						break;
					}
				case ActionChangedType.PaintStyle: {
						SetPaintStyle(GetPaintStyle());
						break;
					}
				case ActionChangedType.Shortcut: {
						SetShortcut(action.Shortcut);
						break;
					}
			}
		}
		public virtual void Dispose() {
			isDisposed = true;
			if(currentContext != null) {
				currentContext.SelectionTypeChanged -= new EventHandler(currentContext_SelectionTypeChanged);
				currentContext = null;
			}
			if(action != null) {
				action.Changed -= new EventHandler<ActionChangedEventArgs>(Action_Changed);
				action = null;
			}
		}
		public ActionBase Action {
			get { return action; }
		}
		public bool IsVisible {
			get {
				if(action == null) return false; 
				bool result = action.Active;
				if(action.SelectionDependencyType == SelectionDependencyType.RequireSingleObject) {
					result = result && (action.SelectionContext != null) && (action.SelectionContext.SelectionType != SelectionType.None);
				}
				return result;
			}
		}
		#region IDisposableExt Members
		public Boolean IsDisposed {
			get { return isDisposed; }
		}
		#endregion
		#region Obsolete 15.2
		[Obsolete("Use 'SetImage(ImageInfo imageInfo)' metod instead."), System.ComponentModel.Browsable(false), System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		protected virtual void SetLargeImage(ImageInfo imageInfo) { }
		#endregion
	}
}
