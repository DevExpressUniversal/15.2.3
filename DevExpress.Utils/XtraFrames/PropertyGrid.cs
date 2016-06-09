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
using System.Collections;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Collections.Generic;
namespace DevExpress.Utils.Frames {
	[ToolboxItem(false)]
	public class PropertyGridEx : PropertyGrid {
		ContextMenu pgMenu;
		MenuItem menuReset;
		MenuItem menuDescription;
		public PropertyGridEx() {
			this.pgMenu = new ContextMenu();
			this.menuReset = this.pgMenu.MenuItems.Add("Reset", new System.EventHandler(this.menuReset_Click));
			this.pgMenu.MenuItems.Add("-");
			this.menuDescription = this.pgMenu.MenuItems.Add("Description", new System.EventHandler(this.menuDescription_Click));
			this.pgMenu.Popup += new System.EventHandler(this.PopupMenu);
			ContextMenu = this.pgMenu;
			Control gridView = GetPropertyGridView();
			if(gridView != null) {
				gridView.MouseDoubleClick += gridView_MouseDoubleClick;
				gridView.ControlAdded += gridView_ControlAdded;
				gridView.ControlRemoved += gridView_ControlRemoved;
			}
		}
		void gridView_ControlRemoved(object sender, ControlEventArgs e) {
			if(e.Control != null)
				e.Control.MouseDoubleClick -= editor_MouseDoubleClick;
		}
		void gridView_ControlAdded(object sender, ControlEventArgs e) {
			if(e.Control != null && e.Control.GetType().Name.Contains("GridViewEdit"))
				e.Control.MouseDoubleClick += editor_MouseDoubleClick;
		}
		void editor_MouseDoubleClick(object sender, MouseEventArgs e) {
			if(SelectedGridItem == null)
				return;
			this.lastProperty = SelectedGridItem.PropertyDescriptor;
			this.lastValue = SelectedGridItem.Value;
			OnPropertyChangedCore(true);
			this.lastProperty = null;
		}
		Control GetGridViewEdit() {
			Control gridView = GetPropertyGridView();
			if(gridView == null)
				return null;
			foreach(Control editor in gridView.Controls) {
				if(editor.GetType().Name.Contains("GridViewEdit"))
					return editor;
			}
			return null;
		}
		void gridView_MouseDoubleClick(object sender, MouseEventArgs e) {
			OnPropertyChangedCore(true);
		}
		[Browsable(false)]
		public ContextMenu PropertyGridMenu { get { return pgMenu; } }
		ISite site;
		public override ISite Site {
			get {
				return site;
			}
			set {
				site = value;
				base.Site = value;
			}
		}
		protected override void Dispose( bool disposing ) {
			if(disposing) {
				RemovePropertyChangedHandler();
				base.Site = null;
			}
			base.Dispose(disposing);
		}
		protected virtual void ShowContextMenu() {
			if(Site == null) return;
			IMenuCommandService menu = (IMenuCommandService)Site.GetService(typeof(IMenuCommandService));
		}
		protected override void WndProc(ref Message msg) {
			base.WndProc(ref msg);
		}
		public void ShowEvents(bool show) {
			ShowEventsButton(show);
		}
		[Browsable(false)]
		public bool DrawFlat { 
			get { return DrawFlatToolbar; }
			set { DrawFlatToolbar = value; }
		}
		public void ExpandProperty(string fullPropertyName) {
			if(GridView == null) return;
			GridEntriesHelper entriesHelper = new GridEntriesHelper(GetAllGridEntries(), true);
			entriesHelper.ExpandPropertyName(fullPropertyName);
		}
		public bool SelectItem(string fullPropertyName) {
			if(GridView == null) return false;
			SelectCorrectTab(fullPropertyName);
			ExpandCategory(fullPropertyName);
			GridEntriesHelper entriesHelper = new GridEntriesHelper(GetAllGridEntries(), true);
			GridItem entry = entriesHelper.GetGridItemByFullPropertyName(fullPropertyName);
			if(entry != null)
				SelectGridItem(entry);
			return entry != null;
		}
		public void SelectPropertiesTab() {
			SelectCorrectTab(false);
		}
		public void SelectEventsTab() {
			SelectCorrectTab(true);
		}
		public int GetScrollOffset() {
			if(GridView == null) return 0 ;
			MethodInfo mi = GridView.GetType().GetMethod("GetScrollOffset", BindingFlags.Instance | BindingFlags.Public);
			if(mi != null) return (int)mi.Invoke(GridView, null);
			return 0;
		}
		public void SetScrollOffset(int offset) {
			if(GridView == null) return;
			MethodInfo mi = GridView.GetType().GetMethod("SetScrollOffset", BindingFlags.Instance | BindingFlags.Public);
			if(mi != null) mi.Invoke(GridView, new object[] { offset});
		}
		#region If event handler added - generate the event about it
		static readonly object eventHandlerAdded = new object();
		public event EventHandler EventHandlerAdded {
			add { Events.AddHandler(eventHandlerAdded, value); }
			remove { Events.RemoveHandler(eventHandlerAdded, value); }
		}
		object lastValue = null;
		object lastObject = null;
		PropertyDescriptor lastProperty = null;
		Control GetPropertyGridView() {
			foreach(Control control in Controls) {
				if(control.GetType().Name.Contains("PropertyGridView"))
					return control;
			}
			return null;
		}
		protected override void OnSelectedGridItemChanged(SelectedGridItemChangedEventArgs e) {
			base.OnSelectedGridItemChanged(e);
			if(e.NewSelection == null || e.NewSelection.PropertyDescriptor == null) return;
			if(!e.NewSelection.PropertyDescriptor.PropertyType.IsSubclassOf(typeof(Delegate))) return;
			RemovePropertyChangedHandler();
			if(SelectedObject == null || SelectedObjects == null || SelectedObjects.Length > 1) return;
			lastObject = GetLastObject(SelectedObject);
			if(lastObject == null)
				return;
			lastProperty = e.NewSelection.PropertyDescriptor;
			if(lastProperty == null)
				return;
			try {
				lastValue = e.NewSelection.Value;
				lastProperty.AddValueChanged(lastObject, OnPropertyChanged);
			}
			catch { }
		}
		protected virtual object GetLastObject(object selectedObject) {
			var objectWrapper = selectedObject as DevExpress.Utils.Design.IDXObjectWrapper;
			return objectWrapper != null ? objectWrapper.SourceObject : SelectedObject;
		}
		void OnPropertyChanged(object sender, EventArgs e) {
			OnPropertyChangedCore(false);
		}
		void OnPropertyChangedCore(bool isDoubleClick) {
			if(lastProperty == null) return;
			if(!lastProperty.PropertyType.IsSubclassOf(typeof(Delegate))) return;
			if(lastValue != null && !isDoubleClick) return;
			RemovePropertyChangedHandler();
			OnEventHandlerAdded(EventArgs.Empty);
		}
		void RemovePropertyChangedHandler() {
			try {
				if(lastObject != null && lastProperty != null)
					lastProperty.RemoveValueChanged(lastObject, OnPropertyChanged);
				lastObject = null;
				lastValue = null;
				lastProperty = null;
			}
			catch { }
		}
		protected virtual void OnEventHandlerAdded(EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[eventHandlerAdded];
			if(handler != null) handler(this, e);
		}
		#endregion
		protected object GridView { 
			get {
				FieldInfo fieldInfo = typeof(PropertyGrid).GetField("gridView", BindingFlags.NonPublic | BindingFlags.Instance);
				if(fieldInfo == null) return null;
				return fieldInfo.GetValue(this);
			}
		}
		public GridItemCollection GetAllGridEntries() {
			if(GridView == null) return null;
			MethodInfo methodInfo = GridView.GetType().GetMethod("GetAllGridEntries", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[0], null);
			if(methodInfo != null) 
				return methodInfo.Invoke(GridView, null) as GridItemCollection;
			return null;
		}
		protected ToolStripButton[] ViewTabButtons {
			get {
				if(GridView == null) return null;
				FieldInfo fieldInfo = typeof(PropertyGrid).GetField("viewTabButtons", BindingFlags.NonPublic | BindingFlags.Instance);
				if(fieldInfo == null) return null;
				return fieldInfo.GetValue(this) as ToolStripButton[];
				}
		}
		protected void SelectGridItem(GridItem entry) {
			if(entry == null) return;
			MethodInfo methodInfo = GridView.GetType().GetMethod("SelectGridEntry", BindingFlags.NonPublic | BindingFlags.Instance);
			if(methodInfo != null) 
				methodInfo.Invoke(GridView, new object[] {entry, true});
		}
		protected void ExpandCategory(string fullPropertyName) {
			if(SelectedObject == null) return;
			if(PropertySort != PropertySort.Categorized 
				&& PropertySort != PropertySort.CategorizedAlphabetical) return;
			string category = GetCategoryName(fullPropertyName);
			if(category != "") {
				ExpandCategoryItem(category);
			}
		}
		string GetCategoryName(string fullPropertyName) {
			fullPropertyName = GetFirstProperty(fullPropertyName);
			PropertyDescriptorCollection collection = TypeDescriptor.GetProperties(SelectedObject);
			for(int i = 0; i < collection.Count; i ++)
				if(collection[i].Name == fullPropertyName)
					return collection[i].Category;
			return "";
		}
		void SelectCorrectTab(string fullName) {
			SelectCorrectTab(GetIsEvent(fullName));
		}
		void SelectCorrectTab(bool isEvent) {
			ToolStripButton[] buttons = ViewTabButtons;
			if(buttons == null || buttons.Length < 2) return;
			ToolStripButton button = buttons[isEvent ? 1 : 0];
			if(button != null)
				button.PerformClick();
		}
		bool GetIsEvent(string fullName) {
			if(SelectedObject == null) return false;
			EventDescriptorCollection collection = TypeDescriptor.GetEvents(SelectedObject);
			for(int i = 0; i < collection.Count; i ++)
				if(collection[i].Name == fullName) 
					return true;
			return false;
		}
		string GetFirstProperty(string fullPropertyName) {
			int indexOf = fullPropertyName.IndexOf('.');
			if(indexOf > 0) 
				fullPropertyName = fullPropertyName.Substring(0, indexOf);
			return fullPropertyName;
		}
		void ExpandCategoryItem(string category) {
			GridItemCollection items = GetAllGridEntries();			
			for(int i = 0; i < items.Count; i ++) {
				if(items[i].PropertyDescriptor == null && items[i].Label == category) {
					items[i].Expanded = true;
					break;
				}
			}
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			if(e.Button == MouseButtons.Right) 
				pgMenu.Show(this, new Point(e.X, e.Y));
		}
		void menuReset_Click(object sender, System.EventArgs e) {
			ResetSelectedProperty();
			base.OnPropertyValueChanged(new PropertyValueChangedEventArgs(SelectedGridItem, ""));
		}
		void menuDescription_Click(object sender, System.EventArgs e) {
			HelpVisible = !HelpVisible;
		}
		void PopupMenu(object sender, System.EventArgs e) {
			GridItem gItem = SelectedGridItem;
			if(gItem != null && gItem.GridItemType == GridItemType.Property && gItem.PropertyDescriptor != null) {
				ShowMenuItems(true);
				try {
					menuReset.Enabled = gItem.PropertyDescriptor.CanResetValue(SelectedObject);
				}
				catch {
				}
				menuDescription.Checked = HelpVisible;
			} else
				ShowMenuItems(false);
		}
		void ShowMenuItems(bool makeVisible) {
			foreach(MenuItem item in pgMenu.MenuItems) {
				item.Visible = makeVisible;
			}
		}
	}
	internal class GridEntriesHelper {
		List<GridItem> entries = new List<GridItem>();
		public GridEntriesHelper(GridItemCollection entries, bool excludeCategories) {
			if(entries != null) {
				foreach(GridItem item in entries) {
					if(excludeCategories && item.PropertyDescriptor == null) continue;
					this.entries.Add(item);
				}
			}
		}
		public List<GridItem> Entries { get { return entries; } }
		public int Count { get { return Entries.Count; } }
		public GridItem this[int index ] { get { return Entries[index]; } }
		public GridItem this[string label] {
			get {
				for(int n = 0; n < entries.Count; n++) {
					if(entries[n].Label == label) return entries[n];
				}
				return null;
			}
		}
		public GridItem ExpandPropertyName(string propertyName) {
			if(Entries == null) return null;
			string nestedPropertyName = "";
			int indexOf = propertyName.IndexOf('.');
			if(indexOf > 0) {
				nestedPropertyName = propertyName.Substring(indexOf + 1, propertyName.Length - indexOf - 1);
				propertyName = propertyName.Substring(0, indexOf);
			} 
			GridItem entry = this[propertyName];
			if(entry != null) {
				entry.Expanded = true;
				if(nestedPropertyName != "") {
					GridEntriesHelper entriesHelper = new GridEntriesHelper(entry.GridItems, true);
					entry = entriesHelper.ExpandPropertyName(nestedPropertyName);
				} 
			}
			return entry;
		}
		public GridItem GetGridItemByFullPropertyName(string propertyName) {
			if(Entries == null) return null;
			GridEntriesHelper helper = this;
			if(propertyName.IndexOf('.') > -1) {
				int index = propertyName.Length;
				while(propertyName[--index] != '.');
				string expandedProperyName = propertyName.Substring(0, index);
				propertyName = propertyName.Remove(0, index + 1);
				GridItem entry = ExpandPropertyName(expandedProperyName);
				if(entry != null)
					helper = new GridEntriesHelper(entry.GridItems, true);
			}
			return helper[propertyName];
		}
	}
}
