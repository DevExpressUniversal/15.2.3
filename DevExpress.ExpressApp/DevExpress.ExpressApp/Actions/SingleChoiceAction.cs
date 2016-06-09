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
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Actions {
	public class SingleChoiceActionExecuteEventArgs : SimpleActionExecuteEventArgs {
		private ChoiceActionItem selectedItem;
		public SingleChoiceActionExecuteEventArgs(ActionBase action, ISelectionContext context, ChoiceActionItem selectedItem)
			: base(action, context){
			this.selectedItem = selectedItem;
		}
		public ChoiceActionItem SelectedChoiceActionItem {
			get { return selectedItem; }
		}
	}
	public delegate void SingleChoiceActionExecuteEventHandler(Object sender, SingleChoiceActionExecuteEventArgs e);
	public enum SingleChoiceActionItemType { ItemIsMode, ItemIsOperation }
	[DXToolboxItem(true)]
	[ToolboxBitmap(typeof(XafApplication), "Resources.Actions.Action_SingleChoiceAction.bmp")]
	[DevExpress.Utils.ToolboxTabName(XafAssemblyInfo.DXTabXafActions)]
	public class SingleChoiceAction : ChoiceActionBase {
		private bool itemTypeChanged = false;
		private ChoiceActionItem selectedItem;
		private SingleChoiceActionItemType itemType = SingleChoiceActionItemType.ItemIsMode;
		private void OnSelectedItemChanged() {
			if(SelectedItemChanged != null && (LockCount == 0))
				SelectedItemChanged(this, EventArgs.Empty);
		}
		private void OnItemTypeChanged() {
			if(LockCount > 0) {
				itemTypeChanged = true;
			}
			else {
				if(ItemTypeChanged != null) {
					ItemTypeChanged(this, EventArgs.Empty);
				}
			}
		}
		protected override void OnDeactivated() {
			SelectedItem = null;
			base.OnDeactivated();
		}
		protected override void LogActionInfo() {
			base.LogActionInfo();
			Tracing.Tracer.LogValue("SelectedItem", selectedItem);
		}
		protected override void ReleaseLockedEvents() {
			base.ReleaseLockedEvents();
			OnSelectedItemChanged();
			if(itemTypeChanged) {
				OnItemTypeChanged();
				itemTypeChanged = false;
			}
		}
		public override string GetTotalToolTip() {
			SingleChoiceActionHelper helper = new SingleChoiceActionHelper(this);
			return helper.GetActionToolTip();
		}
		protected internal override void RaiseExecute(ActionBaseEventArgs eventArgs) {
			Execute(this, (SingleChoiceActionExecuteEventArgs)eventArgs);
		}
		public SingleChoiceAction() : this(null) { }
		public SingleChoiceAction(IContainer container) : base(container) { }
		public SingleChoiceAction(Controller owner, string id, PredefinedCategory category)
			: this(owner, id, category.ToString()) { }
		public SingleChoiceAction(Controller owner, string id, string category) : base(owner, id, category) { }
		public void DoExecute(ChoiceActionItem selectedItem) {
			SelectedItem = selectedItem;
			ExecuteCore(Execute, new SingleChoiceActionExecuteEventArgs(this, SelectionContext, selectedItem));
		}
		public ChoiceActionItem FindItemByCaptionPath(string captionPath) {
			if(string.IsNullOrEmpty(captionPath)) return null;
			string[] pathParts = SplitPath(captionPath);
			ChoiceActionItemCollection currentItemsCollection = Items;
			ChoiceActionItem foundItem = null;
			for(int i = 0; i < pathParts.Length; i++) {
				if(currentItemsCollection.Count == 0) {
					break;
				}
				foundItem = null;
				foreach(ChoiceActionItem item in currentItemsCollection) {
					if(item.Caption == pathParts[i]) {
						foundItem = item;
						break;
					}
				}
				if(foundItem == null) {
					break;
				}
				currentItemsCollection = foundItem.Items;
			}
			return foundItem;
		}
		public ChoiceActionItem FindItemByIdPath(string idPath) {
			if(string.IsNullOrEmpty(idPath)) return null;
			string[] pathParts = SplitPath(idPath);
			ChoiceActionItemCollection currentItemsCollection = Items;
			ChoiceActionItem foundItem = null;
			for(int i = 0; i < pathParts.Length; i++) {
				if(currentItemsCollection.Count == 0) {
					break;
				}
				foundItem = null;
				foreach(ChoiceActionItem item in currentItemsCollection) {
					if(item.Id == pathParts[i]) {
						foundItem = item;
						break;
					}
				}
				if(foundItem == null) {
					break;
				}
				currentItemsCollection = foundItem.Items;
			}
			return foundItem;
		}
		private string[] SplitPath(string path) {
			const char escapeCharacter = '\\';
			const char separator = '/';
			List<string> result = new List<string>();
			int pathPartStartIndex = 0;
			for(int i = 0; i < path.Length; i++) {
				if(path[i] == escapeCharacter) {
					i++;
					if(i == path.Length || (path[i] != escapeCharacter && path[i] != separator)) {
						throw new ArgumentException(string.Format("The {0} path has excessive escape character.", path), "path");
					}
				}
				else if(path[i] == separator) {
					if(pathPartStartIndex != i) {
						result.Add(GetDecodedPathPart(path.Substring(pathPartStartIndex, i - pathPartStartIndex)));
					}
					pathPartStartIndex = i + 1;
				}
			}
			if(pathPartStartIndex < path.Length) {
				result.Add(GetDecodedPathPart(path.Substring(pathPartStartIndex)));
			}
			return result.ToArray();
		}
		private string GetDecodedPathPart(string pathPart) {
			return pathPart.Replace(@"\\", @"\").Replace(@"\/", @"/");
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ChoiceActionItem SelectedItem {
			get { return selectedItem; }
			set {
				if(selectedItem != value) {
					if(value != null) {
						if(!value.Enabled) {
							throw new InvalidOperationException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.UnableToAssignDisabledActionItem,
								this.Id, value.Id, GetReasonsForFalse(value.Enabled, true)));
						}
						if(!value.Active) {
							throw new InvalidOperationException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.UnableToAssignInvisibleActionItem,
								this.Id, value.Id, GetReasonsForFalse(value.Active, true)));
						}
					}
					selectedItem = value;
					OnSelectedItemChanged();
				}
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int SelectedIndex {
			get { return Items.IndexOf(selectedItem); }
			set { SelectedItem = Items[value]; }
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("SingleChoiceActionItemType"),
#endif
 Category("Items"), DefaultValue(SingleChoiceActionItemType.ItemIsMode)]
		public SingleChoiceActionItemType ItemType {
			get { return itemType; }
			set {
				if(itemType != value) {
					itemType = value;
					OnItemTypeChanged();
				}
			}
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("SingleChoiceActionExecute"),
#endif
 Category("Action")]
		public event SingleChoiceActionExecuteEventHandler Execute;
		[Browsable(false)]
		public event EventHandler SelectedItemChanged;
		[Browsable(false)]
		public event EventHandler ItemTypeChanged;
		protected override void OnItemsChanged(ChoiceActionItem item, ChoiceActionItemChangesType changedType) {
			if(changedType == ChoiceActionItemChangesType.Remove || changedType == ChoiceActionItemChangesType.ItemsRemove) {
				if(SelectedItem == item) {
					SelectedItem = null;
				}
			}
			base.OnItemsChanged(item, changedType);
		}
	}
	public class SingleChoiceActionHelper {
		private SingleChoiceAction action;
		public SingleChoiceActionHelper(SingleChoiceAction action) {
			this.action = action;
		}
		public ChoiceActionItem FindDefaultItem() {
			ChoiceActionItem defaultItem = Action.Items.FirstActiveItem;
			if(Action.DefaultItemMode == DefaultItemMode.LastExecutedItem) {
				if(Action.SelectedItem != null && Action.SelectedItem.Active) {
					defaultItem = Action.SelectedItem;
				}
			}
			return defaultItem;
		}
		public string GetActionCaption(string defaultCaption) {
			ChoiceActionItem defaultItem = FindDefaultItem();
			string actionCaption = defaultCaption;
			if(defaultItem != null) {
				string formatCaption = Action.Model.CaptionFormat;
				if(!String.IsNullOrEmpty(formatCaption)) {
					actionCaption = String.Format(formatCaption, Action.Caption, defaultItem.Caption);
				}
			}
			return actionCaption;
		}
		public ImageInfo GetActionImageInfo(ImageInfo defaultImageInfo) {
			return GetActionImageInfo(defaultImageInfo, false);
		}
		public ImageInfo GetActionImageInfo(ImageInfo defaultImageInfo, bool isLarge) {
			ChoiceActionItem defaultItem = FindDefaultItem();
			ImageInfo result = defaultImageInfo;
			if(defaultItem != null) {
				if(Action.ImageMode == ImageMode.UseItemImage) {
					if(!isLarge) {
						result = ImageLoader.Instance.GetImageInfo(defaultItem.ImageName);
					}
					else {
						result = ImageLoader.Instance.GetLargeImageInfo(defaultItem.ImageName);
					}
				}
			}
			return result;
		}
		public string GetActionImageName() {
			if(Action.ImageMode == ImageMode.UseItemImage) {
				ChoiceActionItem defaultItem = FindDefaultItem();
				if(defaultItem != null) {
					return defaultItem.ImageName;
				}
			}
			return Action.ImageName;
		}
		public string GetActionToolTip() {
			ChoiceActionItem defaultItem = FindDefaultItem();
			bool needShowDefaultItemCaption = defaultItem != null && !Action.ShowItemsOnClick && Action.ItemType != SingleChoiceActionItemType.ItemIsMode;
			return Action.GetTotalToolTip(needShowDefaultItemCaption ? defaultItem.Caption : string.Empty);
		}
		public SingleChoiceAction Action {
			get { return action; }
		}
	}
}
