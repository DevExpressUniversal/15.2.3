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
using DevExpress.ExpressApp.Actions;
namespace DevExpress.ExpressApp.Templates {
	public abstract class ChoiceActionItemWrapper : IDisposableExt {
		private static Array choiceActionItemChangesTypeValues = Enum.GetValues(typeof(ChoiceActionItemChangesType));
		private ChoiceActionItem choiceActionItem;
		private ChoiceActionBase action;
		private Boolean isDisposed;
		private bool showImage = true;
		private void action_ItemsChanged(object sender, ItemsChangedEventArgs e) {
			if(isDisposed) {
				return;
			}
			ChoiceActionItemChangesType changedType;
			if(e.ChangedItemsInfo.TryGetValue(choiceActionItem, out changedType)) {
				foreach(ChoiceActionItemChangesType value in choiceActionItemChangesTypeValues) {
					if((changedType & value) == value) {
						switch(value) {
							case ChoiceActionItemChangesType.Caption:
								SetCaption(choiceActionItem.Caption);
								break;
							case ChoiceActionItemChangesType.Image:
								SetImageName(choiceActionItem.ImageName);
								break;
							case ChoiceActionItemChangesType.Data:
								SetData(choiceActionItem.Data);
								break;
							case ChoiceActionItemChangesType.Enabled:
								SetEnabled(choiceActionItem.Enabled);
								break;
							case ChoiceActionItemChangesType.Active:
								SetVisible(choiceActionItem.Active);
								break;
							case ChoiceActionItemChangesType.ToolTip:
								SetToolTip(choiceActionItem.ToolTip);
								break;
						}
					}
				}
			}
		}
		protected void SyncronizeWithItem() {
			if(ActionItem != null) {
				SyncronizeWithItemCore();
			}
		}
		protected virtual void SyncronizeWithItemCore() {
			if(ShowImage) {
				SetImageName(ActionItem.ImageName);
			}
			SetCaption(ActionItem.Caption);
			SetData(ActionItem.Data);
			SetShortcut(ActionItem.Model.Shortcut);
			SetEnabled(ActionItem.Enabled);
			SetVisible(ActionItem.Active);
			SetToolTip(ActionItem.ToolTip);
		}
		public abstract void SetImageName(string imageName);
		public abstract void SetCaption(string caption);
		public abstract void SetData(object data);
		public abstract void SetShortcut(string shortcutString);
		public abstract void SetEnabled(bool enabled);
		public abstract void SetVisible(bool visible);
		public abstract void SetToolTip(string toolTip);
		public ChoiceActionItemWrapper(ChoiceActionItem choiceActionItem, ChoiceActionBase action) {
			if(choiceActionItem == null && action == null) return;
			this.choiceActionItem = choiceActionItem;
			this.action = action;
			this.action.ItemsChanged += new EventHandler<ItemsChangedEventArgs>(action_ItemsChanged);
		}
		public bool ShowImage {
			get { return showImage; }
			set {
				showImage = value;
			}
		}
		public ChoiceActionItem ActionItem { get { return choiceActionItem; } }
		public virtual void Dispose() {
			isDisposed = true;
			if(this.choiceActionItem != null) {
				this.choiceActionItem = null;
			}
			if(action != null) {
				this.action.ItemsChanged -= new EventHandler<ItemsChangedEventArgs>(action_ItemsChanged);
				action = null;
			}
		}
		public bool IsDisposed {
			get { return isDisposed; }
		}
	}
}
