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

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using System.Xml;
using System.Text;
using System.IO;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors.ToolboxIcons;
namespace DevExpress.XtraEditors.Repository {
	public class RepositoryItemMRUEdit : RepositoryItemComboBox {
		int maxItemCount;
		bool allowRemoveMRUItems;
		public RepositoryItemMRUEdit() {
			this.allowRemoveMRUItems = true;
			this.fValidateOnEnterKey = true;
			this.maxItemCount = 0;
		}
		protected override bool DefaultImmeditatePopup { get { return true; } }
		public override void Assign(RepositoryItem item) {
			RepositoryItemMRUEdit source = item as RepositoryItemMRUEdit;
			BeginUpdate(); 
			try {
				base.Assign(item);
				if(source == null) return;
				this.allowRemoveMRUItems = source.AllowRemoveMRUItems;
				this.maxItemCount = source.MaxItemCount;
				this.fValidateOnEnterKey = source.ValidateOnEnterKey;
			}
			finally {
				EndUpdate();
			}
			Events.AddHandler(addingMRUItem, source.Events[addingMRUItem]);
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(ObsoleteText.SRObsoletePropertiesText)]
		public new RepositoryItemMRUEdit Properties { get { return this; } }
		[Browsable(false)]
		public override string EditorTypeName { get { return "MRUEdit"; } }
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemMRUEditValidateOnEnterKey"),
#endif
 DefaultValue(true)]
		public override bool ValidateOnEnterKey { get { return base.ValidateOnEnterKey; } set { base.ValidateOnEnterKey = value; } }
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemMRUEditImmediatePopup"),
#endif
 DefaultValue(true), SmartTagProperty("Immediate Popup", "", 0)]
		public override bool ImmediatePopup { get { return base.ImmediatePopup; } set { base.ImmediatePopup = value; } }
		[DXCategory(CategoryName.Data), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemMRUEditItems"),
#endif
 Localizable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor))]
		public new MRUEditItemCollection Items { get { return base.Items as MRUEditItemCollection; } }
		protected internal override bool UpdateEditValueFromPopup { get { return TextEditStyle == TextEditStyles.Standard; } }
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemMRUEditMaxItemCount"),
#endif
 DefaultValue(0), SmartTagProperty("Max Item Count", "")]
		public virtual int MaxItemCount {
			get { return maxItemCount; }
			set {
				if(value < 0) value = 0;
				if(MaxItemCount == value) return;
				maxItemCount = value;
				Items.CheckItemCount();
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemMRUEditAllowRemoveMRUItems"),
#endif
 DefaultValue(true)]
		public virtual bool AllowRemoveMRUItems {
			get { return allowRemoveMRUItems; }
			set {
				if(AllowRemoveMRUItems == value) return;
				allowRemoveMRUItems = value;
				OnPropertiesChanged();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool AutoComplete {
			get { return base.AutoComplete; }
			set { base.AutoComplete = value; }
		}
		protected override void OnItems_CollectionChanged(object sender, CollectionChangeEventArgs e) {
			if(OwnerEdit != null && OwnerEdit.InplaceType != InplaceType.Standalone && e.Action != CollectionChangeAction.Refresh) {
				BeginUpdate();
				try {
					RepositoryItemMRUEdit mru = OwnerEdit.Tag as RepositoryItemMRUEdit;
					if(mru != null && mru != this) {
						mru.BeginUpdate();
						try {
							mru.Items.Assign(Items);
						}
						finally {
							mru.CancelUpdate();
						}
					}
				}
				finally {
					CancelUpdate();
				}
			}
			else
				OnPropertiesChanged();
		}
		protected override ComboBoxItemCollection CreateItemCollection() {
			return new MRUEditItemCollection(this);
		}
		static object addingMRUItem = new object();
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemMRUEditAddingMRUItem"),
#endif
 DXCategory(CategoryName.Events)]
		public event AddingMRUItemEventHandler AddingMRUItem {
			add { this.Events.AddHandler(addingMRUItem, value); }
			remove { this.Events.RemoveHandler(addingMRUItem, value); }
		}
		protected internal virtual void RaiseAddingMRUItem(AddingMRUItemEventArgs e) {
			AddingMRUItemEventHandler handler = (AddingMRUItemEventHandler)this.Events[addingMRUItem];
			if(handler != null) handler(GetEventSender(), e);
		}
		static object removingMRUItem = new object();
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemMRUEditAddingMRUItem"),
#endif
 DXCategory(CategoryName.Events)]
		public event RemovingMRUItemEventHandler RemovingMRUItem {
			add { this.Events.AddHandler(removingMRUItem, value); }
			remove { this.Events.RemoveHandler(removingMRUItem, value); }
		}
		protected internal virtual void RaiseRemovingMRUItem(RemovingMRUItemEventArgs e) {
			RemovingMRUItemEventHandler handler = (RemovingMRUItemEventHandler)this.Events[removingMRUItem];
			if(handler != null) handler(GetEventSender(), e);
		}
		protected internal override bool AllowValidateOnEnterKey { get { return true; } }
		string GetItemName(int i) {
			return string.Format("Item{0}", i);
		}
		public void SaveItemsToXml(string fileName) {
			SaveItemsToXml(fileName, Encoding.Default);
		}
		public void SaveItemsToXml(string fileName, Encoding encoding) {
			using(XmlTextWriter writer = new XmlTextWriter(fileName, encoding)) 
				WriteData(writer);
		}
		public void SaveItemsToStream(Stream stream) {
			using(XmlTextWriter writer = new XmlTextWriter(stream, Encoding.Default))
				WriteData(writer);
		}
		void WriteData(XmlTextWriter writer) {
			writer.WriteStartDocument();
			writer.WriteWhitespace("\n");
			writer.WriteStartElement("Items");
			writer.WriteWhitespace("\n");
			for(int i = 0; i < Items.Count; i++) {
				writer.WriteElementString(GetItemName(i), string.Format("{0}", Items[i]));
				writer.WriteWhitespace("\n");
			}
			writer.WriteEndElement();
			writer.WriteEndDocument();
			writer.Close();
		}
		void ReadData(XmlTextReader reader) {
			BeginUpdate();
			Items.Clear();
			try {
				while(reader.Read()) {
					reader.MoveToContent();
					if(reader.NodeType == System.Xml.XmlNodeType.Text) {
						Items.Add(reader.Value, false);
					}
				}
			} finally {
				EndUpdate();
			}
		}
		public void LoadItemsFromXml(string fileName) {
			using(XmlTextReader reader = new XmlTextReader(fileName))
				ReadData(reader);
		}
		public void LoadItemsFromStream(Stream stream) {
			MRUMemoryStream st = stream as MRUMemoryStream;
			if(st != null)
				st.Seek(0, SeekOrigin.Begin);
			using(XmlTextReader reader = new XmlTextReader(stream)) {
				ReadData(reader);
			}
		}
		[Browsable(false)]
		public override SimpleContextItemCollectionOptions ContextButtonOptions {
			get { return base.ContextButtonOptions; }
		}
		[Browsable(false)]
		public override ContextItemCollection ContextButtons {
			get { return base.ContextButtons; }
		}
	}
}
namespace DevExpress.XtraEditors {
	[Description("Stores recently used items (MRU) within its drop-down window."),
	 ToolboxTabName(AssemblyInfo.DXTabCommon),
	ToolboxBitmap(typeof(ToolboxIconsRootNS), "MRUEdit")
	]
	public class MRUEdit : ComboBoxEdit {
		public MRUEdit() {
		}
		[Browsable(false)]
		public override string EditorTypeName { get { return "MRUEdit"; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("MRUEditProperties"),
#endif
 DXCategory(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SmartTagSearchNestedProperties]
		public new RepositoryItemMRUEdit Properties { get { return base.Properties as RepositoryItemMRUEdit; } }
		protected internal new PopupMRUForm PopupForm { get { return base.PopupForm as PopupMRUForm; } }
		protected override PopupBaseForm CreatePopupForm() {
			return new PopupMRUForm(this);
		}
		protected override void OnEditorKeyDown(KeyEventArgs e) {
			if(!e.Handled && IsPopupOpen && e.KeyData == Keys.Enter) {
				PopupForm.ProcessKeyDown(e);
				if(e.Handled) return;
			}
			base.OnEditorKeyDown(e);
		}
		protected internal override void OnPopupSelectedIndexChanged() {
			if(IsMaskBoxAvailable) {
				EditValue = PopupForm.ResultValue;
			}
			base.OnPopupSelectedIndexChanged();
		}
		protected override void UpdatePopupEditValueIndex(int prevIndex) {
			if(Properties.TextEditStyle != TextEditStyles.Standard) base.UpdatePopupEditValueIndex(prevIndex);
			else {
				if(SelectedIndex != prevIndex) {
					Properties.RaiseSelectedIndexChanged(EventArgs.Empty);
					Properties.RaiseSelectedValueChanged(EventArgs.Empty);
				}
			}
		}
		protected override void FindUpdateEditValue(int itemIndex, bool jopened) {
			if(Properties.TextEditStyle == TextEditStyles.Standard) {
				FindUpdateEditValueAutoSearchText();
				return;
			}
			if(itemIndex > -1) EditValue = Properties.Items[itemIndex];
		}
		protected override void OnValidating(CancelEventArgs e) {
			base.OnValidating(e);
			if(e.Cancel || EditValue == null || EditValue == DBNull.Value || EditValue.Equals(string.Empty)) return;
			Properties.Items.Add(EditValue);
		}
		protected override void FindUpdatePopupSelectedItem(int itemIndex) {
			if(Properties.TextEditStyle != TextEditStyles.Standard) {
				base.FindUpdatePopupSelectedItem(itemIndex);
				return;
			}
			if(PopupForm != null) PopupForm.SetMruFilter(GetAutoSearchTextFilter());
		}
		protected override void ProcessAutoSearchNavKey(KeyEventArgs e) { }
		protected override void DoImmediatePopup(int itemIndex, char key) {
			if(itemIndex != -1 && !IsPopupOpen) {
				ShowPopup();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("MRUEditAddingMRUItem"),
#endif
 DXCategory(CategoryName.Events)]
		public event AddingMRUItemEventHandler AddingMRUItem {
			add { Properties.AddingMRUItem += value; }
			remove { Properties.AddingMRUItem -= value; }
		}
		protected override void SilentRemoveCore(object item) {
			RemovingMRUItemEventArgs e = new RemovingMRUItemEventArgs(item);
			Properties.RaiseRemovingMRUItem(e);
			if(e.Cancel)
				return;
			base.SilentRemoveCore(item);
			return;
		}
		protected internal override void OnActionItemClick(ListItemActionInfo action) {
			if(!SilentRemove(action.Item)) return;
			if(IsPopupOpen) RefreshPopup();
			if(!CanShowPopup) ClosePopup(PopupCloseMode.Cancel);
		}
		protected internal override bool HasItemActions { get { return Properties != null && !Properties.ReadOnly && Properties.AllowRemoveMRUItems; } }
		protected internal override void CreateItemActions(BaseListBoxViewInfo.ItemInfo itemInfo) {
			itemInfo.ActionInfo = new ListItemActionCollection();
			itemInfo.ActionInfo.Add(new ListItemDeleteActionInfo(itemInfo));
		}
	}
}
namespace DevExpress.XtraEditors.Popup {
	[ToolboxItem(false)]
	public class PopupMRUForm : PopupListBoxForm {
		public PopupMRUForm(ComboBoxEdit ownerEdit) : base(ownerEdit) { }
		int lockChanges = 0;
		protected internal override void OnBeforeShowPopup() {
			base.OnBeforeShowPopup();
			this.lockChanges++;
			try {
				SetMruFilter(OwnerEdit.GetAutoSearchTextFilter());
			}
			finally {
				this.lockChanges--;
			}
		}
		public virtual void SetMruFilter(string text) {
			if(OwnerEdit == null || Properties.TextEditStyle != TextEditStyles.Standard) return;
			SelectedItemIndex = -1;
			ListBox.SetFilter(OwnerEdit.AutoSearchText);
		}
		protected override void SetupListBoxOnShow() {
			ListBox.HotTrackItems = false;
		}
		protected internal override void OnVisibleRowCountChanged() {
			if(this.lockChanges != 0) return;
			OwnerEdit.RefreshPopup();
		}
	}
}
namespace DevExpress.XtraEditors.ViewInfo {
	public class MRUEditViewInfo : ComboBoxViewInfo {
		public MRUEditViewInfo(RepositoryItem item) : base(item) {
		}
	}
}
namespace DevExpress.XtraEditors.Controls {
	public class MRUMemoryStream : MemoryStream {
		public void CloseStream() {
			base.Close();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void Close() {
		}
	}
	public enum MRUItemAddReason { AddNew, Internal }
	[ListBindable(false)]
	public class MRUEditItemCollection : ComboBoxItemCollection {
		internal MRUItemAddReason addReason = MRUItemAddReason.AddNew;
		public MRUEditItemCollection(RepositoryItemComboBox properties) : base(properties) {}
		public override void Assign(ComboBoxItemCollection collection) {
			BeginUpdate();
			addReason = MRUItemAddReason.Internal;
			try {
				Clear();
				int count = Math.Min(collection.Count, Properties.MaxItemCount);
				if(Properties.MaxItemCount == 0) count = collection.Count;
				for(int n = 0; n < count; n++) {
					base.Add(CloneItem(collection[n]));
				}
			}
			finally {
				addReason = MRUItemAddReason.AddNew;
				EndUpdate();
			}
		}
		public override void AddRange(object[] items) {
			BeginUpdate();
			try {
				foreach(object item in items) Add(item, false);
			} finally {
				EndUpdate();
			}
		}
		public int Add(object item, bool insertAtTop) {
			if(insertAtTop) {
				item = ExtractItem(item);
				if(!CanAddItem(item)) return -1;
				if(CanRemoveLastItem) List.RemoveAt(Count - 1);
				List.Insert(0, item);
				return 0;
			} else 
				if(CanAddItem(item))
					return base.Add(item);
			return -1;
		}
		public override int Add(object item) {
			return Add(item, true);
		}
		protected new RepositoryItemMRUEdit Properties { get { return base.Properties as RepositoryItemMRUEdit; } }
		protected override bool CanAddItem(object item) {
			if(IndexOf(item) != -1) return false;
			if((Properties.MaxItemCount == 0) || (Count < Properties.MaxItemCount || CanRemoveLastItem)) {
				AddingMRUItemEventArgs ee = new AddingMRUItemEventArgs(item, addReason);
				Properties.RaiseAddingMRUItem(ee);
				return !ee.Cancel;
			}
			return false;
		}
		protected virtual bool CanRemoveLastItem {
			get { return (Properties.MaxItemCount > 0 && Count >= Properties.MaxItemCount); }
		}
		protected internal virtual void CheckItemCount() {
			if(Properties.MaxItemCount == 0) return;
			while(Count > Properties.MaxItemCount)
				RemoveAt(Count - 1);
		}
	}
	public class MRUItemEventArgs : CancelEventArgs { 
		object item;
		public MRUItemEventArgs(object item) {
			this.item = item;
		}
		public object Item { get { return item; } }
	}
	public class AddingMRUItemEventArgs : MRUItemEventArgs {
		MRUItemAddReason reason;
		public AddingMRUItemEventArgs(object item) : this(item, MRUItemAddReason.AddNew) { }
		public AddingMRUItemEventArgs(object item, MRUItemAddReason reason) : base(item) {
			this.reason = reason;
		}
		public MRUItemAddReason AddReason { get { return reason; } }
	}
	public delegate void AddingMRUItemEventHandler(object sender, AddingMRUItemEventArgs e);
	public class RemovingMRUItemEventArgs : MRUItemEventArgs {
		public RemovingMRUItemEventArgs(object item) : base(item) { }
	}
	public delegate void RemovingMRUItemEventHandler(object sender, RemovingMRUItemEventArgs e);
}
