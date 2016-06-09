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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using DevExpress.Utils.Design;
using DevExpress.Utils.Navigation;
using DevExpress.Utils.Serializing;
using DevExpress.XtraEditors;
using DevExpress.Utils.VisualEffects;
using DevExpress.Utils;
namespace DevExpress.XtraBars.Navigation {
	[ToolboxItem(false), DesignTimeVisible(false)]
	public class NavigationBarItem : Component, DevExpress.Utils.MVVM.ISupportCommandBinding, ISupportAdornerElement {
		static readonly object selected = new object();
		NavigationBarItemCore tile;
		public NavigationBarItem() {
			tile = new NavigationBarItemCore(this);
			ShowPeekFormOnItemHover = DevExpress.Utils.DefaultBoolean.Default;
		}
		[DefaultValue(true), Category(CategoryName.Appearance)]
		public bool Visible {
			get { return Tile.Visible; }
			set { Tile.Visible = value; }
		}
		internal NavigationBarItemCore Tile {
			get { return tile; }
		}
		string name = String.Empty;
		[DefaultValue(""), Browsable(false), XtraSerializableProperty]
		public string Name {
			get {
				return Site == null ? name : Site.Name;
			}
			set {
				if(Site != null) {
					Site.Name = value;
					name = Site.Name;
				}
				else { name = value; }
			}
		}
		[DefaultValue(null), Category(CategoryName.Data), SmartTagProperty("Tag", ""),
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(UITypeEditor)),
		TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))]
		public object Tag {
			get { return Tile.Tag; }
			set { Tile.Tag = value; }
		}
		[DefaultValue(null), Category(CategoryName.Appearance),
		SmartTagProperty("Text", "Appearance", 0)]
		public string Text {
			get { return Tile.Text; }
			set { Tile.Text = value; }
		}
		[DefaultValue(null), Category(CategoryName.Appearance),
		SmartTagProperty("Image", "Appearance", 10),
		Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		protected internal Image Image {
			get { return Tile.Image; }
			set { Tile.Image = value; }
		}
		string custText = String.Empty;
		[DefaultValue(""), Category(CategoryName.Properties)]
		public string CustomizationText {
			get { return custText; }
			set { custText = value; }
		}
		SuperToolTip superTip;
		[Editor("DevExpress.XtraEditors.Design.ToolTipContainerUITypeEditor, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(UITypeEditor)), 
		SmartTagProperty("Super Tip", "Appearance", 7), Category("Appearance")]
		public virtual SuperToolTip SuperTip {
			get { return superTip; }
			set { superTip = value; }
		}
		protected virtual bool ShouldSerializeSuperTip() { return SuperTip != null && !SuperTip.IsEmpty; }
		public virtual void ResetSuperTip() { SuperTip = null; }
		#region Commands
		public virtual IDisposable BindCommand(object command, Func<object> queryCommandParameter = null) {
			return DevExpress.Utils.MVVM.CommandHelper.Bind(this,
				(item, execute) => item.Selected += (s, e) => execute(),
				(item, canExecute) => SetSelected(item.Tile, !canExecute() ? this : null),
				command, queryCommandParameter);
		}
		public virtual IDisposable BindCommand(System.Linq.Expressions.Expression<Action> commandSelector, object source, Func<object> queryCommandParameter = null) {
			return DevExpress.Utils.MVVM.CommandHelper.Bind(this,
				(item, execute) => item.Selected += (s, e) => execute(),
				(item, canExecute) => SetSelected(item.Tile, !canExecute() ? this : null),
				commandSelector, source, queryCommandParameter);
		}
		public virtual IDisposable BindCommand<T>(System.Linq.Expressions.Expression<Action<T>> commandSelector, object source, Func<T> queryCommandParameter = null) {
			return DevExpress.Utils.MVVM.CommandHelper.Bind(this,
				(item, execute) => item.Selected += (s, e) => execute(),
				(item, canExecute) => SetSelected(item.Tile, !canExecute() ? this : null),
				commandSelector, source, () => (queryCommandParameter != null) ? queryCommandParameter() : default(T));
		}
		static void SetSelected(NavigationBarItemCore tile, NavigationBarItem item) {
			if(tile == null || item == null) return;
			NavigationBarCore navBar = tile.Control as NavigationBarCore;
			if(navBar != null && navBar.Owner != null)
				navBar.Owner.SelectedItem = item;
			else {
				if(item.Collection != null && item.Collection.Owner != null)
					item.Collection.Owner.SelectedItem = item;
			}
		}
		#endregion Commands
		protected internal void ShowPeekForm(Control content, Rectangle itemRect) {
			if(Collection == null || Collection.Owner == null) return;
			Collection.Owner.ShowPeekForm(content, itemRect, this);
		}
		[Browsable(false)]
		public NavigationBarItemCollection Collection { get; internal set; }
		[DefaultValue(DevExpress.Utils.DefaultBoolean.Default), Category(CategoryName.Behavior)]
		public DevExpress.Utils.DefaultBoolean ShowPeekFormOnItemHover { get; set; }
		protected virtual internal bool CanShowPeekFormOnItemHover() {
			return ShowPeekFormOnItemHover != DevExpress.Utils.DefaultBoolean.False;
		}
		public override string ToString() {
			return DevExpress.Utils.Controls.OptionsHelper.GetObjectText(this);
		}
		public void Select() {
			SetSelected(Tile, this);
			RaiseSelected();
		}
		protected virtual void RaiseSelected() {
			var handler = Events[selected] as NavigationBarItemClickEventHandler;
			if(handler != null)
				handler(this, new NavigationBarItemEventArgs(this));
		}
		event NavigationBarItemClickEventHandler Selected {
			add { Events.AddHandler(selected, value); }
			remove { Events.RemoveHandler(selected, value); }
		}
		#region ISupportAdornerElement
		Rectangle ISupportAdornerElement.Bounds {
			get { return (Tile as ISupportAdornerElement).Bounds; }
		}
		event UpdateActionEventHandler ISupportAdornerElement.Changed {
			add { }
			remove { }
		}
		bool ISupportAdornerElement.IsVisible {
			get {
				bool visible = (Tile as ISupportAdornerElement).IsVisible; 
				var controlInfo = ((Tile as ISupportAdornerElement).Owner as OfficeNavigationBarViewInfo);
				if(controlInfo == null) return visible;
				return visible && !controlInfo.HiddenItems.Contains(Tile); 
			}
		}
		ISupportAdornerUIManager ISupportAdornerElement.Owner {
			get { return (Tile as ISupportAdornerElement).Owner; }
		}
		#endregion ISupportAdornerElement
	}
	class ClientNavigationBarItem : NavigationBarItem {
		INavigationItem itemCore;
		public ClientNavigationBarItem(INavigationItem item) {
			this.itemCore = item;
			this.Visible = item.Visible;
			Tile.Name = item.Name;
			Tile.Text = item.Text;
			Tile.Image = item.Image;
		}
	}
	class NavigationBarItemCore : TileItem {
		public NavigationBarItemCore(NavigationBarItem ownerItem) {
			this.ownerItemcore = ownerItem;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(hoverPeekTimer != null) {
					hoverPeekTimer.Tick -= hoverPeekTimer_Tick;
					hoverPeekTimer.Dispose();
					hoverPeekTimer = null;
				}
			}
			base.Dispose(disposing);
		}
		NavigationBarItem ownerItemcore;
		internal NavigationBarItem OwnerItem {
			get { return ownerItemcore; }
		}
		protected override TileItemViewInfo CreateViewInfo() {
			return new NavigationBarItemViewInfo(this);
		}
		Timer hoverPeekTimer;
		protected internal Timer HoverPeekTimer {
			get {
				if(hoverPeekTimer == null) {
					hoverPeekTimer = new Timer();
					hoverPeekTimer.Tick += hoverPeekTimer_Tick;
				}
				return hoverPeekTimer;
			}
		}
		void hoverPeekTimer_Tick(object sender, EventArgs e) {
			HoverPeekTimer.Stop();
			ShowPeekForm(Control as NavigationBarCore);
		}
		protected internal void ShowPeekForm(NavigationBarCore owner) {
			if(owner == null || !OwnerItem.CanShowPeekFormOnItemHover()) return;
			ShowPeekForm(owner.RaiseQueryControl(OwnerItem));
		}
		protected virtual void ShowPeekForm(Control control) {
			if(OwnerItem == null || control == null) return;
			Rectangle rect = Control.Control.RectangleToScreen(ItemInfo.Bounds);
			OwnerItem.ShowPeekForm(control, rect);
		}
	}
	[ListBindable(false), TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter)),
	RefreshProperties(RefreshProperties.All)]
	public class NavigationBarItemCollection : CollectionBase, IEnumerable<NavigationBarItem> {
		public NavigationBarItem this[int index] { get { return (NavigationBarItem)List[index]; } set { List[index] = value; } }
		public NavigationBarItem this[string name] {
			get {
				foreach(NavigationBarItem item in List) {
					if(string.Equals(name, item.Name))
						return item;
				}
				return null;
			}
			set {
				for(int i = 0; i < List.Count; i++) {
					if(string.Equals(name, (List[i] as NavigationBarItem).Name)) List[i] = value;
				}
			}
		}
		public NavigationBarItemCollection(OfficeNavigationBar owner) {
			this.ownerCore = owner;
		}
		OfficeNavigationBar ownerCore;
		public OfficeNavigationBar Owner {
			get { return ownerCore; }
		}
		public int IndexOf(NavigationBarItem item) { return List.IndexOf(item); }
		public virtual int Add(NavigationBarItem item) {
			if(List.Contains(item)) return List.IndexOf(item);
			return List.Add(item);
		}
		public virtual void AddRange(NavigationBarItem[] items) {
			this.BeginUpdate();
			try {
				foreach(NavigationBarItem item in items) {
					Add(item);
				}
			}
			finally { this.EndUpdate(); }
		}
		public void Insert(int index, NavigationBarItem item) {
			if(List.Contains(item))
				return;
			List.Insert(index, item);
		}
		public bool Contains(NavigationBarItem barItem) {
			return List.Contains(barItem);
		}
		public void Remove(NavigationBarItem item) {
			if(!List.Contains(item)) return;
			List.Remove(item);
		}
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			SetCollection(value, this);
			AddToContainer(value);
			OnCollectionChanged();
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			OnCollectionChanged();
		}
		protected override void OnRemoveComplete(int index, object value) {
			base.OnRemoveComplete(index, value);
			SetCollection(value, null);
			RemoveFromContainer(value);
			OnCollectionChanged();
		}
		protected override void OnSetComplete(int index, object oldValue, object newValue) {
			base.OnSetComplete(index, oldValue, newValue);
			SetCollection(oldValue, null);
			SetCollection(newValue, this);
			RemoveFromContainer(oldValue);
			AddToContainer(newValue);
			OnCollectionChanged();
		}
		void AddToContainer(object component) {
			if(Owner != null) Owner.AddItemToContainer(component);
		}
		void RemoveFromContainer(object component) {
			if(Owner != null) Owner.RemoveItemFromContainer(component);
		}
		int lockUpdate = 0;
		public void BeginUpdate() {
			lockUpdate++;
		}
		public void EndUpdate() {
			if(--lockUpdate == 0)
				OnCollectionChanged();
		}
		protected virtual void OnCollectionChanged() {
			if(lockUpdate != 0) return;
			RaiseCollectionChanged();
		}
		void SetCollection(object item, NavigationBarItemCollection collection) {
			(item as NavigationBarItem).Collection = collection;
		}
		void RaiseCollectionChanged() {
			EventHandler handler = CollectionChanged;
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		public event EventHandler CollectionChanged;
		IEnumerator<NavigationBarItem> IEnumerable<NavigationBarItem>.GetEnumerator() {
			foreach(NavigationBarItem item in List)
				yield return item;
		}
		public override string ToString() {
			if(Count == 0) return "None";
			if(Count == 1)
				return string.Concat("{", ((NavigationBarItem)List[0]).Text, "}");
			return string.Format("Count {0}", Count);
		}
	}
}
