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
using System.Text;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.NodeWrappers;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Actions {
	public enum ActiveItemsBehavior { Independent, RequireActiveItems }
	public class ChoiceActionItem : IComplexChoiceAction {
		public const string HasActiveItemsKey = "hasActiveItems";
		public const string DisplayMember = "Caption";
		private ChoiceActionItemCollection items;
		private IModelBaseChoiceActionItem model = new ModelChoiceActionItemInternal(string.Empty);
		private bool isOwnInfo;
		private object data;
		private Dictionary<string, object> dataItems = new Dictionary<string, object>();
		private bool beginGroup;
		private BoolList enabledList = new BoolList();
		private BoolList activeList = new BoolList();
		private IComplexChoiceAction owner;
		private ActiveItemsBehavior activeItemsBehavior = ActiveItemsBehavior.Independent;
		private void ItemChangedCore(ChoiceActionItem item, ChoiceActionItemChangesType changedPropertyType) {
			UpdateState(item, changedPropertyType);
			if(owner != null) {
				owner.ItemChangedCore(item, changedPropertyType);
			}
		}
		private void InitializeAddedItem(ChoiceActionItem chidItem) {
			SetDefaultItemCaption(chidItem);
			UpdateActive();
		}
		private void UpdateActive() {
			bool hasActiveItems = activeItemsBehavior == ActiveItemsBehavior.Independent; 
			foreach(ChoiceActionItem item in Items) {
				if(item.Active) {
					hasActiveItems = true;
					break;
				}
			}
			Active[HasActiveItemsKey] = hasActiveItems;
		}
		private void enabledList_ValueChanged(object sender, BoolValueChangedEventArgs e) {
			ItemChanged(this, ChoiceActionItemChangesType.Enabled);
		}
		private void activeList_ResultValueChanged(object sender, BoolValueChangedEventArgs e) {
			ItemChanged(this, ChoiceActionItemChangesType.Active);
		}
		private void UpdateState(ChoiceActionItem item, ChoiceActionItemChangesType changedPropertyType) {
			if(changedPropertyType == ChoiceActionItemChangesType.Add || changedPropertyType == ChoiceActionItemChangesType.ItemsAdd) {
				InitializeAddedItem(item);
			}
			if(changedPropertyType == ChoiceActionItemChangesType.Remove) {
				UpdateActive();
			}
			if(changedPropertyType == ChoiceActionItemChangesType.Active) {
				UpdateActive();
			}
		}
		private void SetDefaultItemCaption(ChoiceActionItem chidItem) {
			if(string.IsNullOrEmpty(chidItem.Caption)) {
				chidItem.Caption = string.Format("{0} {1}", ChoiceActionBase.DefaultItemCaption, items.IndexOf(chidItem) + 1);
			}
		}
		protected internal void NotifyAboutCurrentAspectChanged() {
			ItemChanged(this, ChoiceActionItemChangesType.ToolTip);
			ItemChanged(this, ChoiceActionItemChangesType.Caption);
			foreach(ChoiceActionItem item in items) {
				item.NotifyAboutCurrentAspectChanged();
			}
		}
		protected virtual void ItemChanged(ChoiceActionItem item, ChoiceActionItemChangesType changedPropertyType) {
			ItemChangedCore(item, changedPropertyType);
		}
		protected internal ChoiceActionItem Clone() {
			ChoiceActionItem result = new ChoiceActionItem();
			result.model = model;
			result.data = data;
			result.beginGroup = beginGroup;
			result.enabledList = enabledList;
			result.activeList = activeList;
			result.activeItemsBehavior = activeItemsBehavior;
			foreach(ChoiceActionItem childItem in items) {
				result.Items.Add(childItem.Clone());
			}
			return result;
		}
		protected internal bool IsOwnInfo {
			get { return isOwnInfo; }
		}
		public ChoiceActionItem() {
			isOwnInfo = true;
			enabledList.ResultValueChanged += new EventHandler<BoolValueChangedEventArgs>(enabledList_ValueChanged);
			activeList.ResultValueChanged += new EventHandler<BoolValueChangedEventArgs>(activeList_ResultValueChanged);
			items = new ChoiceActionItemCollection(this);
		}
		public ChoiceActionItem(string caption, object data)
			: this(null, caption, data) {
		}
		public ChoiceActionItem(string id, string caption, object data)
			: this() {
			model.SetValue<string>(DevExpress.ExpressApp.Model.Core.ModelValueNames.Id, id);
			this.Caption = caption;
			this.data = data;
		}
		public ChoiceActionItem(IModelBaseChoiceActionItem info)
			: this() {
			isOwnInfo = false;
			this.model = info;
		}
		public ChoiceActionItem(IModelBaseChoiceActionItem info, object data)
			: this(info) {
			this.data = data;
		}
		public override string ToString() {
			StringBuilder result = new StringBuilder();
			if(string.IsNullOrEmpty(Caption)) {
				result.Append(GetType().Name);
			}
			else {
				result.Append(Caption);
			}
			if(Items.Count > 0) {
				result.Append("[");
				foreach(ChoiceActionItem child in Items) {
					result.Append(child.ToString());
					result.Append(",");
				}
				result.Remove(result.Length - 1, 1);
				result.Append("]");
			}
			return result.ToString();
		}
		protected internal void AssignInfo(IModelChoiceActionItem info) {
			if(info != null && this.model != info) {
				this.model = new ModelChoiceActionItemInternal(info);
				if(info.ChoiceActionItems != null) {
					foreach(ChoiceActionItem item in Items) {
						IModelChoiceActionItem itemInfo = info.ChoiceActionItems[item.Id];
						if(itemInfo != null) {
							item.AssignInfo(itemInfo);
						}
					}
				}
			}
		}
		public string GetItemPath() {
			string path = Caption;
			ChoiceActionItem current = this;
			while(current != null) {
				if(current.ParentItem != null) {
					path = current.ParentItem.Caption + '.' + path;
				}
				current = current.ParentItem;
			}
			return path;
		}
		public string GetCaptionPath() {
			string path = GetEncodedPathPart(Caption);
			ChoiceActionItem current = this;
			while(current != null) {
				if(current.ParentItem != null) {
					path = AddParentPartToPath(current.ParentItem.Caption, path);
				}
				current = current.ParentItem;
			}
			return path;
		}
		public string GetIdPath() {
			string path = GetEncodedPathPart(Id);
			ChoiceActionItem current = this;
			while(current != null) {
				if(current.ParentItem != null) {
					path = AddParentPartToPath(current.ParentItem.Id, path);
				}
				current = current.ParentItem;
			}
			return path;
		}
		private string AddParentPartToPath(string parentPart, string path) {
			return GetEncodedPathPart(parentPart) + '/' + path;
		}
		private string GetEncodedPathPart(string pathPart) {
			return pathPart.Replace(@"\", @"\\").Replace(@"/", @"\/");
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ChoiceActionItemImageName"),
#endif
 Category("Item"), DefaultValue("")]
		public string ImageName {
			get { return model.ImageName; }
			set {
				if(model.ImageName != value) {
					model.ImageName = value;
					ItemChanged(this, ChoiceActionItemChangesType.Image);
				}
			}
		}
		[Browsable(false)]
		public BoolList Enabled {
			get { return enabledList; }
		}
		[Browsable(false)]
		public BoolList Active {
			get { return activeList; }
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ChoiceActionItemCaption"),
#endif
 Category("Item"), DefaultValue("")]
		public string Caption {
			get { return model.Caption; }
			set {
				if(model.Caption != value) {
					model.Caption = value;
					if(string.IsNullOrEmpty(model.Id)) {
						model.Id = value;
					}
					ItemChanged(this, ChoiceActionItemChangesType.Caption);
				}
			}
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ChoiceActionItemToolTip"),
#endif
 Category("Item"), DefaultValue(""), EditorBrowsable( EditorBrowsableState.Never), Browsable(false)]
		public string ToolTip {
			get { return model.ToolTip; }
			set {
				if(model.ToolTip != value) {
					model.ToolTip = value;
					ItemChanged(this, ChoiceActionItemChangesType.ToolTip);
				}
			}
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ChoiceActionItemShortcut"),
#endif
 DefaultValue(""), Category("Behavior")]
		public string Shortcut {
			get { return model.Shortcut; }
			set {
				if(model.Shortcut != value) {
					model.Shortcut = value;
				}
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string Id {
			get { return model.Id; }
			set {
				if(model.Id != value) {
					model.Id = value;
				}
			}
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ChoiceActionItemBeginGroup"),
#endif
 Category("Item"), DefaultValue(false)]
		public bool BeginGroup {
			get { return beginGroup; }
			set {
				if(beginGroup != value) {
					beginGroup = value;
					ItemChanged(this, ChoiceActionItemChangesType.BeginGroup);
				}
			}
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ChoiceActionItemData"),
#endif
 TypeConverter(typeof(StringConverter)), Bindable(true), Localizable(false), Category("Item"), DefaultValue(null)]
		public object Data {
			get { return data; }
			set {
				if(data != value) {
					data = value;
					ItemChanged(this, ChoiceActionItemChangesType.Data);
				}
			}
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ChoiceActionItemItems"),
#endif
 Category("Child Items"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ChoiceActionItemCollection Items {
			get { return items; }
		}
		public bool IsHierarchical() {
			foreach(ChoiceActionItem item in Items) {
				if(item.Items.Count != 0) {
					return true;
				}
			}
			return false;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ChoiceActionItem ParentItem {
			get {
				if(owner != null && owner is ChoiceActionItem) {
					return (ChoiceActionItem)owner;
				}
				return null;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Dictionary<string, object> DataItems {
			get { return dataItems; }
		}
		public IModelBaseChoiceActionItem Model { get { return model; } }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ChoiceActionItemActiveItemsBehavior"),
#endif
 DefaultValue(ActiveItemsBehavior.Independent)]
		public ActiveItemsBehavior ActiveItemsBehavior {
			get { return activeItemsBehavior; }
			set {
				activeItemsBehavior = value;
				ItemChanged(this, ChoiceActionItemChangesType.Active);
			}
		}
		internal IComplexChoiceAction Owner {
			get { return owner; }
			set { owner = value; }
		}
		void IComplexChoiceAction.ItemChangedCore(ChoiceActionItem item, ChoiceActionItemChangesType changedPropertyType) {
			ItemChangedCore(item, changedPropertyType);
		}
		void IComplexChoiceAction.ItemsChangedCore(ChoiceActionItemChangesType changedPropertyType) {
			if(items.Count > 0) {
				foreach(ChoiceActionItem item in items) {
					UpdateState(item, changedPropertyType);
				}
			}
			ItemChanged(this, changedPropertyType);
		}
	}
	[Flags]
	public enum ChoiceActionItemChangesType {
		Enabled = 1,
		Active = 2,
		Caption = 4,
		Image = 8,
		BeginGroup = 16,
		Data = 32,
		Items = 64,
		ItemsAdd = 128,
		ItemsRemove = 256,
		Add = 512,
		Remove = 1024,
		ToolTip = 2048
	}
}
