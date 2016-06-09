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
using System.Linq;
using System.Text;
using DevExpress.Utils;
using DevExpress.Web.Internal;
using System.Collections.Specialized;
using DevExpress.Web.Localization;
using DevExpress.Web.Design;
#if ASP
using System.Web.UI;
using System.Web.UI.WebControls;
#else
using DevExpress.vNext;
using DevExpress.vNext.Internal;
#endif
namespace DevExpress.Web {
	public enum AutoBoolean { Auto, True, False }
	public enum ImageSizeMode { ActualSizeOrFit = 0, FitProportional = 1, FillAndCrop = 2 }
	public enum AnimationType { Slide = 0, Fade = 1, None = 2, Auto = 3 }
	public enum ElementVisibilityMode { None = 0, Faded = 1, OnMouseOver = 2, Always = 3 }
	public enum LayoutItemCaptionLocation { Left, Top, Right, Bottom, NotSet, [Obsolete("Use the NotSet value instead.")]NoSet }
	public enum FormLayoutHorizontalAlign { Right, Left, Center, NotSet, [Obsolete("Use the NotSet value instead.")]NoSet }
	public enum FormLayoutVerticalAlign { Top, Bottom, Middle, NotSet, [Obsolete("Use the NotSet value instead.")]NoSet }
	public enum EditorCaptionPosition { Left, Top, Right, Bottom, NotSet }
	public enum EditorCaptionHorizontalAlign { Right, Left, Center, NotSet }
	public enum EditorCaptionVerticalAlign { Top, Bottom, Middle, NotSet }
	public enum ErrorTextPosition { Top, Right, Bottom, Left }
	public enum CheckState { Checked, Unchecked, Indeterminate }
	public enum HelpTextPosition { Auto, Top, Bottom, Left, Right }
	public enum HelpTextHorizontalAlign { Auto, Left, Right, Center }
	public enum HelpTextVerticalAlign { Auto, Top, Bottom, Middle }
	public enum HelpTextDisplayMode { Popup, Inline }
	public enum GroupBoxDecoration { Default, Box, HeadingLine, None }
	public enum ScalePosition { None, Both, RightOrBottom, LeftOrTop }
	public enum ValueToolTipPosition { None, RightOrBottom, LeftOrTop }
	public enum ScaleLabelHighlightMode { None, AlongBarHighlight, HandlePosition }
	public enum Direction { Reversed, Normal }
	public enum ImagePosition { Left, Top, Right, Bottom }
	public enum ItemLinkMode { TextOnly, TextAndImage, ContentBounds }
	public enum CloseAction { None, CloseButton, OuterMouseClick, MouseOut }
	public enum PopupAction { None, LeftMouseClick, RightMouseClick, MouseOver }
	public enum PopupMenuCloseAction { OuterMouseClick, MouseOut };
	public enum WindowCloseAction { None, CloseButton, OuterMouseClick, MouseOut, Default }
	public enum WindowPopupAction { None, LeftMouseClick, RightMouseClick, MouseOver, Default }
	public enum PopupAlignCorrection { Auto, Disabled }
	public enum PopupHorizontalAlign { NotSet, OutsideLeft, LeftSides, Center, RightSides, OutsideRight, WindowCenter }
	public enum PopupVerticalAlign { NotSet, Above, TopSides, Middle, BottomSides, Below, WindowCenter }
	public enum ResizingMode { Postponed, Live }
	public enum ColumnResizeMode { Disabled, Control, NextColumn }
	public enum ControlRenderMode { Classic = 0, Lightweight = 1 }
	public enum ScrollBarMode { Hidden, Visible, Auto }
	public enum MenuIconSetType { NotSet, Colored, ColoredLight, GrayScaled, GrayScaledWithWhiteHottrack }
	public class StateManager : IStateManager, IPropertiesDirtyTracker {
		private StateBag fStateBag;
		private bool fTrackingViewState;
		protected StateBag ViewState {
			get {
				if (IsReadOnlyViewState) {
					fStateBag = new StateBag();
					if (IsTrackingViewState)
						((IStateManager)fStateBag).TrackViewState();
				}
				return fStateBag;
			}
		}
		protected StateBag ReadOnlyViewState {
			get {
				return fStateBag;
			}
		}
		protected bool IsReadOnlyViewState { get { return fStateBag == emptyViewState; } }
		static StateBag emptyViewState = new StateBag();
		public StateManager() {
			fStateBag = emptyViewState;
		}
		public override string ToString() {
			return CommonUtils.GetObjectText(this);
		}
		protected object GetObjectProperty(string key, object defaultValue) {
			return ViewStateUtils.GetObjectProperty(ReadOnlyViewState, key, defaultValue);
		}
		protected void SetObjectProperty(string key, object defaultValue, object value) {
			if (!IsReadOnlyViewState || value != defaultValue)
				ViewStateUtils.SetObjectProperty(ViewState, key, value);
		}
		protected object GetEnumProperty(string key, object defaultValue) {
			return ViewStateUtils.GetEnumProperty(ReadOnlyViewState, key, defaultValue);
		}
		protected void SetEnumProperty(string key, object defaultValue, object value) {
			if (!IsReadOnlyViewState || !Object.Equals(value, defaultValue))
				ViewStateUtils.SetEnumProperty(ViewState, key, defaultValue, value);
		}
		protected bool GetBoolProperty(string key, bool defaultValue) {
			return ViewStateUtils.GetBoolProperty(ReadOnlyViewState, key, defaultValue);
		}
		protected void SetBoolProperty(string key, bool defaultValue, bool value) {
			if (!IsReadOnlyViewState || value != defaultValue)
				ViewStateUtils.SetBoolProperty(ViewState, key, defaultValue, value);
		}
		protected DefaultBoolean GetDefaultBooleanProperty(string key, DefaultBoolean defaultValue) {
			return ViewStateUtils.GetDefaultBooleanProperty(ReadOnlyViewState, key, defaultValue);
		}
		protected void SetDefaultBooleanProperty(string key, DefaultBoolean defaultValue, DefaultBoolean value) {
			if (!IsReadOnlyViewState || value != defaultValue)
				ViewStateUtils.SetDefaultBooleanProperty(ViewState, key, defaultValue, value);
		}
		protected string GetStringProperty(string key, string defaultValue) {
			return ViewStateUtils.GetStringProperty(ReadOnlyViewState, key, defaultValue);
		}
		protected void SetStringProperty(string key, string defaultValue, string value) {
			if (!IsReadOnlyViewState || value != defaultValue)
				ViewStateUtils.SetStringProperty(ViewState, key, defaultValue, value);
		}
		protected char GetCharProperty(string key, char defaultValue) {
			return ViewStateUtils.GetCharProperty(ReadOnlyViewState, key, defaultValue);
		}
		protected void SetCharProperty(string key, char defaultValue, char value) {
			if(!IsReadOnlyViewState || value != defaultValue)
				ViewStateUtils.SetCharProperty(ViewState, key, defaultValue, value);
		}
		protected int GetIntProperty(string key, int defaultValue) {
			return ViewStateUtils.GetIntProperty(ReadOnlyViewState, key, defaultValue);
		}
		protected void SetIntProperty(string key, int defaultValue, int value) {
			if (!IsReadOnlyViewState || value != defaultValue)
				ViewStateUtils.SetIntProperty(ViewState, key, defaultValue, value);
		}
		protected long GetLongProperty(string key, long defaultValue) {
			return ViewStateUtils.GetLongProperty(ReadOnlyViewState, key, defaultValue);
		}
		protected void SetLongProperty(string key, long defaultValue, long value) {
			if(!IsReadOnlyViewState || value != defaultValue)
				ViewStateUtils.SetLongProperty(ViewState, key, defaultValue, value);
		}
		protected Decimal GetDecimalProperty(string key, Decimal defaultValue) {
			return ViewStateUtils.GetDecimalProperty(ReadOnlyViewState, key, defaultValue);
		}
		protected void SetDecimalProperty(string key, Decimal defaultValue, Decimal value) {
			if (!IsReadOnlyViewState || value != defaultValue)
				ViewStateUtils.SetDecimalProperty(ViewState, key, defaultValue, value);
		}
		protected Unit GetUnitProperty(string key, Unit defaultValue) {
			return ViewStateUtils.GetUnitProperty(ReadOnlyViewState, key, defaultValue);
		}
		protected void SetUnitProperty(string key, Unit defaultValue, Unit value) {
			if (!IsReadOnlyViewState || value != defaultValue)
				ViewStateUtils.SetUnitProperty(ViewState, key, defaultValue, value);
		}
		protected System.Drawing.Color GetColorProperty(string key, System.Drawing.Color defaultValue) {
			return ViewStateUtils.GetColorProperty(ReadOnlyViewState, key, defaultValue);
		}
		protected void SetColorProperty(string key, System.Drawing.Color defaultValue, System.Drawing.Color value) {
			if (!IsReadOnlyViewState || value != defaultValue)
				ViewStateUtils.SetColorProperty(ViewState, key, defaultValue, value);
		}
		static IStateManager[] emptyObjects = new IStateManager[0];
		protected virtual IStateManager[] GetStateManagedObjects() {
			return emptyObjects;
		}
		static GetStateManagerObject[] emptyDelegates = new GetStateManagerObject[0];
		protected virtual GetStateManagerObject[] GetStateManagedObjectsDelegates() {
			return emptyDelegates;
		}
		protected void TrackViewState(IStateManager stateManaged) {
			if(stateManaged != null && IsTrackingViewState)
				stateManaged.TrackViewState();
		}
		protected T GetObject<T>(ref T field, bool create) where T : IStateManager, new() {
			if(field == null && create)
				TrackViewState(field = new T());
			return field;
		}
		protected T CreateObject<T>(ref T field) where T : IStateManager, new() {
			return GetObject(ref field, true);
		}
		protected AppearanceStyle GetObject(ref AppearanceStyle field, bool create) {
			if(field == null && create)
				TrackViewState(field = new AppearanceStyle());
			return field;
		}
		protected AppearanceStyle CreateObject(ref AppearanceStyle field) {
			return GetObject(ref field, true);
		}
		protected virtual void LoadViewState(object savedState) {
			if(savedState == null)
				return;
			GetStateManagerObject[] delegates = GetStateManagedObjectsDelegates();
			IStateManager[] objects = GetStateManagedObjects();
			if(delegates == null || delegates.Length == 0) {
				ViewStateUtils.LoadViewState(savedState, (IStateManager)ViewState, objects);
				return;
			}
			if(objects == null || objects.Length == 0) {
				ViewStateUtils.LoadViewState(this, savedState, (IStateManager)ViewState, delegates);
				return;
			}
			object[] res = (object[])savedState;
			if(res[0] != null)
				ViewStateUtils.LoadViewState(res[0], (IStateManager)ViewState, objects);
			if(res[1] != null)
				ViewStateUtils.LoadObjectsViewState(this, (object[])res[1], delegates);
		}
		protected virtual object SaveViewState() {
			GetStateManagerObject[] delegates = GetStateManagedObjectsDelegates();
			IStateManager[] objects = GetStateManagedObjects();
			if (delegates == null || delegates.Length == 0)
				return ViewStateUtils.SaveViewState((IStateManager)ReadOnlyViewState, objects);
			if (objects == null || objects.Length == 0)
				return ViewStateUtils.SaveViewState(this, (IStateManager)ReadOnlyViewState, delegates);
			object[] res = new object[2];
			res[0] = ViewStateUtils.SaveViewState((IStateManager)ReadOnlyViewState, objects);
			res[1] = ViewStateUtils.SaveViewState(this, (object)null, delegates);
			return res;
		}
		protected virtual void TrackViewState() {
			fTrackingViewState = true;
			ViewStateUtils.TrackObjectsViewState(this, GetStateManagedObjectsDelegates());
			ViewStateUtils.TrackViewState((IStateManager)ReadOnlyViewState, GetStateManagedObjects());
		}
		protected bool IsTrackingViewState {
			get { return fTrackingViewState; }
		}
		protected virtual void SetPropertiesDirty() {
			ViewStateUtils.SetPropertiesDirty(this, ReadOnlyViewState, GetStateManagedObjectsDelegates());
			ViewStateUtils.SetPropertiesDirty(null, GetStateManagedObjects());
		}
		bool IStateManager.IsTrackingViewState {
			get { return IsTrackingViewState; }
		}
		void IStateManager.LoadViewState(object savedState) {
			LoadViewState(savedState);
		}
		object IStateManager.SaveViewState() {
			return SaveViewState();
		}
		void IStateManager.TrackViewState() {
			TrackViewState();
		}
		void IPropertiesDirtyTracker.SetPropertiesDirty() {
			SetPropertiesDirty();
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class CollectionItem : StateManager, IWebControlObject, IPropertiesOwner, IExpressionsAccessor, IDesignTimeCollectionItem {
		protected Collection fCollection = null;
		ExpressionBindingCollection expressionBindings;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual int Index {
			get { return GetIndex(); }
			set { SetIndex(value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Collection Collection {
			get { return fCollection; }
		}
		protected bool ClientVisibleInternal {
			get { return GetBoolProperty("ClientVisibleInternal", true); }
			set { SetBoolProperty("ClientVisibleInternal", true, value); }
		}
		protected bool ClientEnabledInternal {
			get { return GetBoolProperty("ClientEnabledInternal", true); }
			set { SetBoolProperty("ClientEnabledInternal", true, value); }
		}
		protected internal bool RuntimeCreated {
			get { return GetBoolProperty("RuntimeCreated", false); }
			set { SetBoolProperty("RuntimeCreated", false, value); }
		}
		public CollectionItem()
			: base() {
		}
		public virtual void Assign(CollectionItem source) {
			IExpressionsAccessor expAccessor = (IExpressionsAccessor)source;
			if(expAccessor.HasExpressions)
				this.expressionBindings = expAccessor.Expressions;
		}
		protected internal virtual void SetDirty() {
			ReadOnlyViewState.SetDirty(true);
		}
		public override string ToString() {
			return GetType().Name;
		}
		protected int GetIndex() {
			if (Collection != null)
				return Collection.IndexOf(this);
			else
				return -1;
		}
		protected void SetIndex(int value) {
			if (Collection != null) {
				Collection.Move(GetIndex(), value);
			}
		}
		protected internal void SetCollection(Collection collection) {
			fCollection = collection;
			if (fCollection != null && (fCollection as IStateManager).IsTrackingViewState)
				(this as IPropertiesDirtyTracker).SetPropertiesDirty();
		}
		protected internal bool GetVisible() {
			return GetBoolProperty("Visible", true);
		}
		protected internal void SetVisible(bool value) {
			if (GetVisible() != value) {
				SetBoolProperty("Visible", true, value);
				if (Collection != null)
					Collection.Changed();
			}
		}
		protected internal int GetVisibleIndex() {
			if (Collection != null)
				return Collection.GetItemVisibleIndex(this);
			else
				return -1;
		}
		protected internal void SetVisibleIndex(int value) {
			if (Collection != null) {
				int visibleIndex = Collection.GetItemVisibleIndex(this);
				if(visibleIndex != value)
					Collection.SetItemVisibleIndex(this, value);
			}
		}
		protected virtual bool IsLoading() {
			if (Collection != null && Collection.Owner != null)
				return Collection.Owner.IsLoading();
			return false;
		}
		protected virtual bool IsRendering() {
			if(Collection != null && Collection.Owner != null)
				return Collection.Owner.IsRendering();
			return false;
		}
		protected virtual bool IsDesignMode() {
			if (Collection != null && Collection.Owner != null)
				return Collection.Owner.IsDesignMode();
			return false;
		}
		protected virtual void LayoutChanged() {
			if (Collection != null && Collection.Owner != null)
				Collection.Owner.LayoutChanged();
		}
		protected virtual void TemplatesChanged() {
			if (Collection != null && Collection.Owner != null)
				Collection.Owner.TemplatesChanged();
		}
		bool IWebControlObject.IsLoading() {
			return IsLoading();
		}
		bool IWebControlObject.IsRendering() {
			return IsRendering();
		}
		bool IWebControlObject.IsDesignMode() {
			return IsDesignMode();
		}
		void IWebControlObject.LayoutChanged() {
			LayoutChanged();
		}
		void IWebControlObject.TemplatesChanged() {
			TemplatesChanged();
		}
		void IPropertiesOwner.Changed(PropertiesBase properties) {
			LayoutChanged();
		}
		ExpressionBindingCollection IExpressionsAccessor.Expressions {
			get {
				if(expressionBindings == null)
					expressionBindings = new ExpressionBindingCollection();
				return expressionBindings;
			}
		}
		bool IExpressionsAccessor.HasExpressions { get { return this.expressionBindings != null && this.expressionBindings.Count > 0; } }
		protected virtual string GetDesignTimeFieldName() {
			return string.Empty;
		}
		protected virtual void SetDesignTimeFieldName(string name) { 
		}
		protected virtual string GetDesignTimeCaption() {
			var result = ToString();
			if(string.IsNullOrEmpty(result))
				result = GetType().Name;
			return result;
		}
		protected virtual bool GetDesignTimeVisible() {
			return GetVisible();
		}
		protected virtual void SetDesignTimeVisible(bool visible) {
			SetVisible(visible);
		}
		protected virtual int GetDesignTimeVisibleIndex() {
			return GetVisibleIndex();
		}
		protected virtual void SetDesignTimeVisibleIndex(int index) {
			SetVisibleIndex(index);
		}
		protected virtual PropertiesBase GetDesignTimeItemEditProperties() {
			return null;
		}
		protected virtual IDesignTimeCollectionItem GetDesignTimeItemParent() { 
			return Collection != null ? Collection.Owner as IDesignTimeCollectionItem : null;
		}
		protected virtual IList GetDesignTimeItems() {
			return null;
		}
		protected virtual string[] GetDesignTimeHiddenPropertyNames() { 
			return new string[] { };
		}
		string IDesignTimeCollectionItem.FieldName { get { return GetDesignTimeFieldName(); } set { SetDesignTimeFieldName(value); } }
		string IDesignTimeCollectionItem.Caption { get { return GetDesignTimeCaption(); } }
		bool IDesignTimeCollectionItem.ReadOnly { get { return false; } }
		bool IDesignTimeCollectionItem.Visible {
			get { return GetDesignTimeVisible(); }
			set { SetDesignTimeVisible(value); }
		}
		int IDesignTimeCollectionItem.VisibleIndex { get { return GetDesignTimeVisibleIndex(); } set { SetDesignTimeVisibleIndex(value); } }
		PropertiesBase IDesignTimeCollectionItem.EditorProperties { get { return GetDesignTimeItemEditProperties(); } }
		IDesignTimeCollectionItem IDesignTimeCollectionItem.Parent { get { return GetDesignTimeItemParent(); } }
		IList IDesignTimeCollectionItem.Items { get { return GetDesignTimeItems(); } }
		string[] IDesignTimeCollectionItem.GetHiddenPropertyNames() {
			return GetDesignTimeHiddenPropertyNames();
		}
		void IDesignTimeCollectionItem.Assign(IDesignTimeCollectionItem item) {
			Assign(item as CollectionItem);
		}
	}
	public abstract class StateManagedCollectionBase : IList, ICollection, IEnumerable, IStateManager {
		ArrayList collectionItems = new ArrayList();
		bool tracking;
		bool saveAll;
		StateBag stateBag;
		protected StateManagedCollectionBase() {
			this.stateBag = new StateBag();
		}
#if !SL
	[DevExpressWebLocalizedDescription("StateManagedCollectionBaseCount")]
#endif
public int Count { get { return CollectionItems.Count; } }
#if !SL
	[DevExpressWebLocalizedDescription("StateManagedCollectionBaseIsEmpty")]
#endif
public bool IsEmpty { get { return Count == 0; } }
		protected internal StateBag InternalViewState { get { return stateBag; }  }
		protected internal bool IsSavedAll {
			get { return ViewStateUtils.GetBoolProperty(InternalViewState, "IsSavedAll", false); }
		}
		protected internal ArrayList CollectionItems { get { return this.collectionItems; } }
		protected internal bool SaveAll { set { this.saveAll = value; } get { return this.saveAll; } }
		protected internal bool Tracking { set { this.tracking = value; } get { return this.tracking; } }
		public void Clear() {
			OnClear();
			CollectionItems.Clear();
			OnClearComplete();
			if (Tracking)
				ForceSaveAll();
		}
		public void CopyTo(Array array, int index) {
			CollectionItems.CopyTo(array, index);
		}
		public IEnumerator GetEnumerator() {
			return CollectionItems.GetEnumerator();
		}
		public void SetDirty() {
			ForceSaveAll();
		}
		protected virtual object CreateKnownType(int index) {
			throw new InvalidOperationException("StateManagedCollection_NoKnownTypes");
		}
		protected abstract void SetDirtyObject(object o);
		protected virtual Type[] GetKnownTypes() {
			return null;
		}
		protected virtual void OnClear() {
		}
		protected virtual void OnClearComplete() {
		}
		protected virtual void OnInsert(int index, object value) {
		}
		protected virtual void OnInsertComplete(int index, object value) {
		}
		protected virtual void OnRemove(int index, object value) {
		}
		protected virtual void OnRemoveComplete(int index, object value) {
		}
		protected virtual void OnValidate(object value) {
			if (value == null)
				throw new ArgumentNullException("value");
		}
		protected void SetIsSavedAll(bool value) {
			if (value)
				ViewStateUtils.SetBoolProperty(InternalViewState, "IsSavedAll", false, value);
			else
				InternalViewState.Remove("IsSavedAll");
		}
		protected virtual void ForceSaveAll() {
			SaveAll = true;
		}
		int ICollection.Count { get { return this.Count; } }
		bool ICollection.IsSynchronized { get { return false; } }
		object ICollection.SyncRoot { get { return null; } }
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		bool IList.IsFixedSize { get { return false; } }
		bool IList.IsReadOnly { get { return CollectionItems.IsReadOnly; } }
		object IList.this[int index] {
			get { return CollectionItems[index]; }
			set {
				if ((index < 0) || (index >= this.Count))
					throw new ArgumentOutOfRangeException("index", "StateManagedCollection_InvalidIndex"); 
				((IList)this).RemoveAt(index);
				((IList)this).Insert(index, value);
			}
		}
		int IList.Add(object value) {
			OnValidate(value);
			InsertInternal(-1, value);
			return CollectionItems.Count - 1;
		}
		void IList.Clear() {
			Clear();
		}
		bool IList.Contains(object value) {
			if (value == null) return false;
			OnValidate(value);
			return CollectionItems.Contains(value);
		}
		int IList.IndexOf(object value) {
			if (value == null) return -1;
			OnValidate(value);
			return CollectionItems.IndexOf(value);
		}
		void IList.Insert(int index, object value) {
			if (value == null)
				throw new ArgumentNullException("value");
			if ((index < 0) || (index > this.Count))
				throw new ArgumentOutOfRangeException("index", ""); 
			OnValidate(value);
			InsertInternal(index, value);
			if (Tracking)
				ForceSaveAll();
		}
		void IList.Remove(object value) {
			if (value != null) {
				OnValidate(value);
				((IList)this).RemoveAt(((IList)this).IndexOf(value));
			}
		}
		void IList.RemoveAt(int index) {
			object item = CollectionItems[index];
			OnRemove(index, item);
			CollectionItems.RemoveAt(index);
			try {
				OnRemoveComplete(index, item);
			} catch {
				CollectionItems.Insert(index, item);
				throw;
			}
			if (Tracking)
				ForceSaveAll();
		}
		protected virtual void LoadViewState(object savedState) {
			if (savedState != null) {
				Pair savedStatePair = (Pair)savedState;
				((IStateManager)InternalViewState).LoadViewState(savedStatePair.First);
				LoadItemsFromViewState(savedStatePair);
			} else
				SetIsSavedAll(false);
		}
		protected virtual void LoadItemsFromViewState(Pair savedStatePair) {
			if (IsSavedAll) {
				Clear();
				if (savedStatePair.Second != null)
					LoadAllItemsFromViewState(savedStatePair.Second);
			}
			else
				if (savedStatePair.Second != null)
					LoadChangedItemsFromViewState(savedStatePair.Second);
		}
		protected virtual void TrackViewState() {
			Tracking = true;
			foreach (IStateManager manager in CollectionItems)
				manager.TrackViewState();
			((IStateManager)InternalViewState).TrackViewState();
		}
		protected virtual object SaveViewState() {
			Pair savedStatePair = new Pair();
			SetIsSavedAll(SaveAll);
			savedStatePair.First = ((IStateManager)InternalViewState).SaveViewState();
			savedStatePair.Second = SaveAll ? SaveAllItemsToViewState() : SaveChangedItemsToViewState();
			return (savedStatePair.First == null && savedStatePair.Second == null) ? null :savedStatePair;
		}
		bool IStateManager.IsTrackingViewState { get { return Tracking; } }
		void IStateManager.LoadViewState(object savedState) {
			LoadViewState(savedState);
		}
		object IStateManager.SaveViewState() {
			return SaveViewState();
		}
		void IStateManager.TrackViewState() {
			TrackViewState();
		}
		private void LoadAllItemsFromViewState(object savedState) {
			Pair savedPair = (Pair)savedState;
			if (savedPair.Second is Pair) {
				Pair second = (Pair)savedPair.Second;
				object[] stateArray = (object[])savedPair.First;
				int[] typeNameIndexArray = (int[])second.First;
				ArrayList typeNameArray = (ArrayList)second.Second;
				Clear();
				for (int i = 0; i < stateArray.Length; i++) {
					object itemInstance;
					if (typeNameIndexArray == null)
						itemInstance = CreateKnownType(0);
					else {
						int index = typeNameIndexArray[i];
						if (index < this.GetKnownTypeCount())
							itemInstance = this.CreateKnownType(index);
						else {
							string typeName = (string)typeNameArray[index - this.GetKnownTypeCount()];
							itemInstance = Activator.CreateInstance(Type.GetType(typeName));
						}
					}
					((IStateManager)itemInstance).TrackViewState();
					((IStateManager)itemInstance).LoadViewState(stateArray[i]);
					((IList)this).Add(itemInstance);
				}
			} else {
				object[] stateArray = (object[])savedPair.First;
				int[] typeNameIndexArray = (int[])savedPair.Second;
				Clear();
				for (int j = 0; j < stateArray.Length; j++) {
					int index = typeNameIndexArray != null ? typeNameIndexArray[j] : 0;
					object itemInstance = this.CreateKnownType(index);
					((IStateManager)itemInstance).TrackViewState();
					((IStateManager)itemInstance).LoadViewState(stateArray[j]);
					((IList)this).Add(itemInstance);
				}
			}
		}
		private void LoadChangedItemsFromViewState(object savedState) {
			Triplet savedStateTriplet = (Triplet)savedState;
			if (savedStateTriplet.Third is Pair) {
				Pair third = (Pair)savedStateTriplet.Third;
				ArrayList itemIndexArray = (ArrayList)savedStateTriplet.First;
				ArrayList stateArray = (ArrayList)savedStateTriplet.Second;
				ArrayList typeNameIndexArray = (ArrayList)third.First;
				ArrayList typeNameArray = (ArrayList)third.Second;
				for (int i = 0; i < itemIndexArray.Count; i++) {
					int itemIndex = (int)itemIndexArray[i];
					if (itemIndex < Count)
						((IStateManager)((IList)this)[itemIndex]).LoadViewState(stateArray[i]);
					else {
						object newItemInstance;
						if (typeNameIndexArray == null)
							newItemInstance = this.CreateKnownType(0);
						else {
							int index = (int)typeNameIndexArray[i];
							if (index < this.GetKnownTypeCount())
								newItemInstance = this.CreateKnownType(index);
							else {
								string typeName = (string)typeNameArray[index - this.GetKnownTypeCount()];
								newItemInstance = Activator.CreateInstance(Type.GetType(typeName));
							}
						}
						((IStateManager)newItemInstance).TrackViewState();
						((IStateManager)newItemInstance).LoadViewState(stateArray[i]);
						((IList)this).Add(newItemInstance);
					}
				}
			} else {
				ArrayList itemIndexArray = (ArrayList)savedStateTriplet.First;
				ArrayList stateArray = (ArrayList)savedStateTriplet.Second;
				ArrayList newItemIndexArray = (ArrayList)savedStateTriplet.Third;
				for (int j = 0; j < itemIndexArray.Count; j++) {
					int itemIndex = (int)itemIndexArray[j];
					if (itemIndex < this.Count)
						((IStateManager)((IList)this)[itemIndex]).LoadViewState(stateArray[j]);
					else {
						int newItemIndex = newItemIndexArray != null ? (int)newItemIndexArray[j] : 0;
						object newItemInstance = this.CreateKnownType(newItemIndex);
						((IStateManager)newItemInstance).TrackViewState();
						((IStateManager)newItemInstance).LoadViewState(stateArray[j]);
						((IList)this).Add(newItemInstance);
					}
				}
			}
		}
		private object SaveAllItemsToViewState() {
			object savedStateObject = null;
			bool needSaveAllItemsToViewState = false;
			int[] typeNameIndexArray = new int[CollectionItems.Count];
			object[] savedItemStateArray = new object[CollectionItems.Count];
			ArrayList typeNameArray = null;
			IDictionary typeNameIndexDictionary = null;
			int knownTypeCount = this.GetKnownTypeCount();
			for (int i = 0; i < CollectionItems.Count; i++) {
				object curItem = CollectionItems[i];
				SetDirtyObject(curItem);
				savedItemStateArray[i] = ((IStateManager)curItem).SaveViewState();
				if (savedItemStateArray[i] != null)
					needSaveAllItemsToViewState = true;
				Type type = curItem.GetType();
				int index = knownTypeCount != 0 ? ((IList)GetKnownTypes()).IndexOf(type) : -1;
				if (index != -1) {
					typeNameIndexArray[i] = index;
				} else {
					if (typeNameArray == null) {
						typeNameArray = new ArrayList();
						typeNameIndexDictionary = new HybridDictionary();
					}
					object typeNameIndex = typeNameIndexDictionary[type];
					if (typeNameIndex == null) {
						typeNameArray.Add(type.AssemblyQualifiedName);
						typeNameIndex = (typeNameArray.Count + knownTypeCount) - 1;
						typeNameIndexDictionary[type] = typeNameIndex;
					}
					typeNameIndexArray[i] = (int)typeNameIndex;
				}
			}
			if (needSaveAllItemsToViewState) {
				if (typeNameArray != null)
					savedStateObject = new Pair(savedItemStateArray, new Pair(typeNameIndexArray, typeNameArray));
				else {
					if (knownTypeCount == 1)
						typeNameIndexArray = null;
					savedStateObject = new Pair(savedItemStateArray, typeNameIndexArray);
				}
			}
			return savedStateObject;
		}
		private object SaveChangedItemsToViewState() {
			object savedStateObject = null;
			bool needSaveAllItemsToViewState = false;
			int count = CollectionItems.Count;
			ArrayList itemIndexArray = new ArrayList();
			ArrayList itemSavedStateArray = new ArrayList();
			ArrayList typeNameIndexArray = new ArrayList();
			ArrayList typeNameArray = null;
			IDictionary typeNameIndexDictionary = null;
			int knownTypeCount = this.GetKnownTypeCount();
			for (int i = 0; i < count; i++) {
				object savedItemState = ((IStateManager)CollectionItems[i]).SaveViewState();
				if (savedItemState != null) {
					needSaveAllItemsToViewState = true;
					itemIndexArray.Add(i);
					itemSavedStateArray.Add(savedItemState);
					Type type = CollectionItems[i].GetType();
					int typeNameIndex = knownTypeCount != 0 ? ((IList)GetKnownTypes()).IndexOf(type) : -1;
					if (typeNameIndex != -1)
						typeNameIndexArray.Add(typeNameIndex);
					else {
						if (typeNameArray == null) {
							typeNameArray = new ArrayList();
							typeNameIndexDictionary = new HybridDictionary();
						}
						object typeNameIndexObject = typeNameIndexDictionary[type];
						if (typeNameIndexObject == null) {
							typeNameArray.Add(type.AssemblyQualifiedName);
							typeNameIndexObject = (typeNameArray.Count + knownTypeCount) - 1;
							typeNameIndexDictionary[type] = typeNameIndexObject;
						}
						typeNameIndexArray.Add(typeNameIndexObject);
					}
				}
			}
			if (needSaveAllItemsToViewState) {
				if (typeNameArray != null) {
					savedStateObject = new Triplet(itemIndexArray, itemSavedStateArray,
						new Pair(typeNameIndexArray, typeNameArray));
				} else {
					if (knownTypeCount == 1)
						typeNameIndexArray = null;
					return savedStateObject = new Triplet(itemIndexArray, itemSavedStateArray, typeNameIndexArray);
				}
			}
			return savedStateObject;
		}
		private void InsertInternal(int index, object obj) {
			if (obj == null)
				throw new ArgumentNullException("obj");
			if (((IStateManager)this).IsTrackingViewState) {
				((IStateManager)obj).TrackViewState();
				this.SetDirtyObject(obj);
			}
			this.OnInsert(index, obj);
			int newIndex;
			if (index == -1)
				newIndex = CollectionItems.Add(obj);
			else {
				newIndex = index;
				CollectionItems.Insert(index, obj);
			}
			try {
				OnInsertComplete(index, obj);
			} catch {
				CollectionItems.RemoveAt(newIndex);
				throw;
			}
		}
		private int GetKnownTypeCount() {
			Type[] knownTypes = GetKnownTypes();
			return knownTypes == null ? 0 : knownTypes.Length;
		}
	}
	[Editor("DevExpress.Web.Design.CollectionEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor)),
	TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter))]
	public class Collection : StateManagedCollectionBase, IStateManagerTracker, IPropertiesDirtyTracker, IAssignableCollection {
		private Type[] fKnownTypes = null;
		private IWebControlObject fOwner;
		private int fLockCount = 0;
		private bool fVisibleItemsListCreated = false;
		private ArrayList fVisibleItems = null;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IWebControlObject Owner {
			get { return GetOwnerCore(); }
		}
		protected virtual IWebControlObject GetOwnerCore() { return fOwner; }
		protected bool IsLoading {
			get { return (Owner != null) ? Owner.IsLoading() : false; }
		}
		protected bool IsDesignMode {
			get { return (Owner != null) ? Owner.IsDesignMode() : false; }
		}
		protected bool Locked {
			get { return fLockCount > 0; }
		}
		public Collection()
			: base() {
		}
		public Collection(IWebControlObject owner)
			: base() {
			fOwner = owner;
		}
		protected override object CreateKnownType(int index) {
			Type[] knownTypes = GetKnownTypes();
			return Activator.CreateInstance(knownTypes[index]);
		}
		protected override Type[] GetKnownTypes() {
			if(fKnownTypes == null)
				fKnownTypes = new Type[1] { GetKnownType() };
			return fKnownTypes;
		}
		protected override void SetDirtyObject(object o) {
			((CollectionItem)o).SetDirty();
		}
		protected internal CollectionItem CreateKnownTypeItem(int index) {
			return CreateKnownType(index) as CollectionItem;
		}
		protected internal void BeginUpdate() {
			fLockCount++;
		}
		protected internal void CancelUpdate() {
			fLockCount--;
		}
		protected internal void EndUpdate() {
			CancelUpdate();
			ForceSaveAll();
			Changed();
		}
		protected CollectionItem CloneItem(CollectionItem item) {
			CollectionItem result = (CollectionItem)Activator.CreateInstance(item.GetType());
			result.Assign(item);
			return result;
		}
		public virtual void Assign(IAssignableCollection source) {
			Collection sourceCollection = source as Collection;
			if(sourceCollection == null)
				return;
			BeginUpdate();
			try {
				Clear();
				if(sourceCollection != null) {
					for(int i = 0; i < sourceCollection.Count; i++)
						Add(CloneItem(sourceCollection.GetItem(i)));
				}
			} finally {
				EndUpdate();
			}
		}
		public void Move(int oldIndex, int newIndex) {
			if((oldIndex >= 0) && (oldIndex != newIndex)) {
				BeginUpdate();
				try {
					CollectionItem item = GetItem(oldIndex);
					Remove(item);
					if(newIndex > Count)
						newIndex = Count;
					Insert(newIndex, item);
				} finally {
					EndUpdate();
				}
			}
		}
		public void RemoveAt(int index) {
			(this as IList).RemoveAt(index);
		}
		public override string ToString() {
			return "(Collection)";
		}
		protected override void OnInsertComplete(int index, object value) {
			CollectionItem collectionItem = value as CollectionItem;
			collectionItem.SetCollection(this);
			if(!IsDesignMode && !IsLoading)
				collectionItem.RuntimeCreated = true;
			Changed();
		}
		protected override void OnRemove(int index, object value) {
			(value as CollectionItem).SetCollection(null);
		}
		protected override void OnRemoveComplete(int index, object value) {
			Changed();
		}
		protected override void OnClear() {
			for(int i = 0; i < Count; i++) {
				GetItem(i).SetCollection(null);
			}
		}
		protected override void OnClearComplete() {
			Changed();
		}
		protected override void OnValidate(object obj) {
			base.OnValidate(obj);
			bool found = false;
			Type[] knownTypes = GetKnownTypes();
			for(int i = 0; i < knownTypes.Length; i++) {
				if((obj.GetType() == knownTypes[i]) || obj.GetType().IsSubclassOf(knownTypes[i])) {
					found = true;
					break;
				}
			}
			if(!found)
				throw new ArgumentException(StringResources.InvalidCollectionItemType);
		}
		protected internal CollectionItem GetItem(int index) {
			return (CollectionItem)(this as IList)[index];
		}
		protected virtual Type GetKnownType() {
			return typeof(CollectionItem);
		}
		protected internal CollectionItem Add(CollectionItem item) {
			BeforeAddItem(item);
			(this as IList).Add(item);
			return item;
		}
		protected internal int IndexOf(CollectionItem item) {
			return (this as IList).IndexOf(item);
		}
		protected internal void Insert(int index, CollectionItem item) {
			BeforeAddItem(item);
			(this as IList).Insert(index, item);
		}
		protected internal void Remove(CollectionItem item) {
			(this as IList).Remove(item);
		}
		protected virtual void OnBeforeAdd(CollectionItem item) {
		}
		protected void BeforeAddItem(CollectionItem item) {
			OnBeforeAdd(item);
			if(item == null)
				throw new ArgumentNullException(StringResources.InvalidNullValue);
			if(item.Collection != null)
				item.Collection.Remove(item);
		}
		protected override void ForceSaveAll() {
			if(!Locked) {
				base.ForceSaveAll();
				((IPropertiesDirtyTracker)this).SetPropertiesDirty();
			}
		}
		protected virtual void OnChanged() {
		}
		protected internal void Changed() {
			if(!Locked) {
				ResetVisibleItemsList();
				OnChanged();
			}
		}
		protected void CheckVisibleItemsList() {
			if(!fVisibleItemsListCreated) {
				RefreshVisibleItemsList();
				fVisibleItemsListCreated = true;
			}
		}
		protected internal void ResetVisibleItemsList() {
			fVisibleItemsListCreated = false;
		}
		protected internal CollectionItem GetVisibleItem(int index) {
			return GetVisibleItemsInternal()[index] as CollectionItem;
		}
		protected internal int GetVisibleItemCount() {
			return GetVisibleItemsInternal().Count;
		}
		protected ArrayList GetVisibleItemsInternal() {
			CheckVisibleItemsList();
			return fVisibleItems != null
				? fVisibleItems
				: CollectionItems;
		}
		protected internal int GetItemVisibleIndex(CollectionItem item) {
			CheckVisibleItemsList();
			if(fVisibleItems != null) {
				return fVisibleItems.IndexOf(item);
			} else {
				return item.Index;
			}
		}
		protected internal void SetItemVisibleIndex(CollectionItem item, int visibleIndex) {
			if(Locked) return;
			int oldVisibleIndex = item.GetVisibleIndex();
			if(oldVisibleIndex != visibleIndex) {
				BeginUpdate();
				try {
					if(visibleIndex <= -1) {
						item.SetVisible(false);
					} else {
						if(!item.GetVisible()) {
							item.SetVisible(true);
							ResetVisibleItemsList(); 
						}
						if(visibleIndex >= GetVisibleItemCount())
							Move(item.Index, Count);
						else
							Move(item.Index, GetVisibleItem(visibleIndex).Index);
					}
				} finally {
					EndUpdate();
				}
			}
		}
		protected internal void RefreshVisibleItemsList() {
			if(IsAllItemsVisible()) {
				if(fVisibleItems != null)
					fVisibleItems = null;
			} else {
				if(fVisibleItems == null)
					fVisibleItems = new ArrayList();
				else
					fVisibleItems.Clear();
				for(int i = 0; i < Count; i++) {
					CollectionItem item = GetItem(i);
					if(item.GetVisible())
						fVisibleItems.Add(item);
				}
			}
		}
		protected internal bool IsAllItemsVisible() {
			for(int i = 0; i < Count; i++) {
				if(!GetItem(i).GetVisible())
					return false;
			}
			return true;
		}
		void IStateManagerTracker.ViewStateLoaded() {
			Changed(); 
		}
		void IPropertiesDirtyTracker.SetPropertiesDirty() {
			for(int i = 0; i < Count; i++) {
				(GetItem(i) as IPropertiesDirtyTracker).SetPropertiesDirty();
			}
		}
	}
	public class Collection<T> : Collection, IList<T> where T : CollectionItem {
		public Collection()
			: base() {
		}
		public Collection(IWebControlObject owner)
			: base(owner) {
		}
		#region IList<T> Members
		public T this[int index] {
			get { return (T)(this as IList)[index]; }
			set { (this as IList)[index] = value; }
		}
		public int IndexOf(T item) {
			return base.IndexOf(item);
		}
		public void Insert(int index, T item) {
			base.Insert(index, item);
		}
		#endregion
		#region ICollection<T> Members
		bool ICollection<T>.IsReadOnly {
			get { return (this as IList).IsReadOnly; }
		}
		public virtual void Add(T item) {
			base.Add(item);
		}
		public bool Contains(T item) {
			return (this as IList).Contains(item);
		}
		public bool Remove(T item) {
			if(CollectionItems.Contains(item)) {
				base.Remove(item);
				return true;
			}
			return false;
		}
		public void CopyTo(T[] array, int index) {
			base.CopyTo(array, index);
		}
		#endregion
		#region IEnumerable<T> Members
		IEnumerator<T> IEnumerable<T>.GetEnumerator() {
			foreach(object item in CollectionItems)
				yield return (T)item;
		}
		#endregion
		private new CollectionItem Add(CollectionItem item) {
			return base.Add(item);
		}
		protected T AddInternal(T item) { 
			Add(item);
			return item;
		}
		public void Add(params T[] items) {
			AddRange(items);
		}
		protected override Type GetKnownType() {
			return typeof(T);
		}
		public new T GetVisibleItem(int index) {
			return base.GetVisibleItem(index) as T;
		}
		public new int GetVisibleItemCount() {
			return base.GetVisibleItemCount();
		}
		public IEnumerable GetVisibleItems() {
			return GetVisibleItemsInternal();
		}
		public void AddRange(IEnumerable<T> items) {
			foreach(T item in new List<T>(items)) 
				Add(item);
		}
		public int IndexOf(Predicate<T> match) {
			for(int i = 0; i < Count; i++)
				if(match(this[i]))
					return i;
			return -1;
		}
		public T Find(Predicate<T> match) {
			return FindByIndex(IndexOf(match));
		}
		protected T FindByIndex(int index) {
			return index == -1 ? null : this[index];
		}
		public IEnumerable<T> FindAll(Predicate<T> match) {
			foreach(T item in this)
				if(match(item))
					yield return item;
		}
		public void RemoveAll(Predicate<T> match) {
			foreach(T item in new List<T>(FindAll(match)))
				Remove(item);
		}
		public Collection<T> ForEach(Action<T> action) {
			foreach(T item in this)
				action(item);
			return this;
		}
		public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter) {
			List<TOutput> result = new List<TOutput>();
			foreach(T item in this)
				result.Add(converter(item));
			return result;
		}
	}
	public class HierarchicalCollection<T> : Collection<T>, IHierarchicalEnumerable where T : CollectionItem, IHierarchyData {
		public HierarchicalCollection()
			: base() {
		}
		public HierarchicalCollection(IWebControlObject owner)
			: base(owner) {
		}
		public T FindRecursive(Predicate<T> match) {
			return Find(match) ?? FindRecursiveInternal(match);
		}
		T FindRecursiveInternal(Predicate<T> match) {
			foreach(T item in this) {
				HierarchicalCollection<T> collection = (HierarchicalCollection<T>)item.GetChildren();
				T possibleResult = collection.FindRecursive(match);
				if(possibleResult != null)
					return possibleResult;
			}
			return null;
		}
		public IEnumerable<T> FindAllRecursive(Predicate<T> match) {
			foreach(T resultItem in FindAll(match))
				yield return resultItem;
			foreach(T item in this) {
				HierarchicalCollection<T> collection = (HierarchicalCollection<T>)item.GetChildren();
				foreach(T resultItem in collection.FindAllRecursive(match))
					yield return resultItem;
			}
		}
		public void RemoveAllRecursive(Predicate<T> match) {
			RemoveAll(match);
			foreach(T item in this) {
				HierarchicalCollection<T> collection = (HierarchicalCollection<T>)item.GetChildren();
				collection.RemoveAllRecursive(match);
			}
		}
		public IHierarchyData GetHierarchyData(object enumeratedItem) {
			return (T)enumeratedItem;
		}
	}
	public class ContentControlCollectionItem : CollectionItem {
		private Control ownerControl = new Control();
		private ContentControlCollection contentControlCollection = null;
		private CollectionItemControl contentControl = null;
		public ContentControlCollectionItem()
			: base() {
		}
		[Browsable(false), AutoFormatDisable,
		NotifyParentProperty(true), 
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ControlCollection Controls {
			get {
				if(IsDeserialization())			  
					return ControlsOwner.Controls;  
				else
					return ContentControl.Controls;
			}
		}
		[Browsable(false), AutoFormatDisable,
		EditorBrowsableAttribute(EditorBrowsableState.Never),
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ContentControlCollection ContentCollection {
			get {
				if(contentControlCollection == null)
					contentControlCollection = CreateContentControlCollection(this.ownerControl);
				return contentControlCollection;
			}
		}
		protected CollectionItemControl ControlsOwner {
			get {
				if(contentControl == null)
					contentControl = new CollectionItemControl(this);
				return contentControl;
			}
		}
		protected internal ContentControl ContentControl {
			get {
				if(ContentCollection.Count == 0)
					ContentCollection.Add(CreateContentControl());
				return ContentCollection[0];
			}
		}
		protected virtual ContentControlCollection CreateContentControlCollection(Control ownerControl) {
			return new ContentControlCollection(ownerControl);
		}
		protected virtual ContentControl CreateContentControl() {
			return new ContentControl();
		}
		protected virtual bool IsDeserialization() {
			return (Collection == null);
		}
	}
	public interface IPropertiesOwner {
		void Changed(PropertiesBase properties);
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class PropertiesBase : StateManager {
		private IPropertiesOwner fOwner = null;
		private int fLockCount = 0;
		public PropertiesBase() {
		}
		public PropertiesBase(IPropertiesOwner owner) {
			fOwner = owner;
		}
		protected IPropertiesOwner Owner {
			get { return fOwner; }
		}
		public virtual void Assign(PropertiesBase source) {
		}
		protected void BeginUpdate() {
			fLockCount++;
		}
		protected void EndUpdate() {
			fLockCount--;
			if (fLockCount == 0)
				Changed();
		}
		protected virtual void Changed() {
			if (fOwner != null && fLockCount == 0)
				fOwner.Changed(this);
		}
	}
	public class HtmlAttribute {
		private string name;
		private string value;
		public static readonly HtmlAttribute Empty;
		static HtmlAttribute() {
			Empty = new HtmlAttribute("", "");
		}
		public HtmlAttribute()
			: this("", "") {
		}
		public HtmlAttribute(string name, string value) {
			this.name = name;
			this.value = value;
		}
		public virtual bool IsEmpty {
			get { return String.IsNullOrEmpty(this.name) || String.IsNullOrEmpty(this.value); }
		}
		public string Name {
			get { return name; }
			set { name = value; }
		}
		public string Value {
			get { return value; }
			set { this.value = value; }
		}
	}
	public class ExpandoAttribute : HtmlAttribute {
		private string htmlElementId;
		private const string CreationScriptFormat =
			"document.getElementById(\"{0}\").setAttribute(\"{1}\", \"{2}\");\n";
		public ExpandoAttribute(string name, string value, string htmlElementId)
			: base(name, value) {
			this.htmlElementId = htmlElementId;
		}
		public override bool IsEmpty {
			get {
				return base.IsEmpty || String.IsNullOrEmpty(this.htmlElementId);
			}
		}
		public string HtmlElementId {
			get { return this.htmlElementId; }
		}
		public string GetCreationScript() {
			if (IsEmpty)
				return "";
			else
				return String.Format(CreationScriptFormat, this.htmlElementId, this.Name, this.Value);
		}
	}
	public class ExpandoAttributes {
		private List<ExpandoAttribute> expandoAttributes = null;
		public void AddAttribute(string name, string value, string htmlElementId) {
			if (this.expandoAttributes == null)
				this.expandoAttributes = new List<ExpandoAttribute>();
			int index = IndexOf(name, htmlElementId);
			if (index != -1)
				this.expandoAttributes[index].Value = value;
			else
				expandoAttributes.Add(new ExpandoAttribute(name, value, htmlElementId));
		}
		internal void Clear() {
			if (this.expandoAttributes != null)
				this.expandoAttributes.Clear();
		}
		internal string GetCreationScript() {
			if (this.expandoAttributes == null)
				return "";
			StringBuilder sb = new StringBuilder();
			foreach (ExpandoAttribute attr in this.expandoAttributes)
				sb.Append(attr.GetCreationScript());
			return sb.ToString();
		}
		protected int IndexOf(string name, string htmlElementId) {
			int index = -1;
			if (this.expandoAttributes == null)
				return index;
			for (int i = 0; i < this.expandoAttributes.Count; i++) {
				if ((this.expandoAttributes[i].Name == name) &&
					(this.expandoAttributes[i].HtmlElementId == htmlElementId)) {
					return i;
				}
			}
			return index;
		}
	}
	public class SettingsLoadingPanel : PropertiesBase {
		public const int DefaultDelay = 300;
		public SettingsLoadingPanel(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SettingsLoadingPanelDelay"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, DefaultValue(DefaultDelay)]
		public int Delay {
			get { return GetIntProperty("Delay", DefaultDelay); }
			set {
				CommonUtils.CheckNegativeValue(value, "Delay");
				SetIntProperty("Delay", DefaultDelay, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SettingsLoadingPanelEnabled"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, DefaultValue(true)]
		public virtual bool Enabled {
			get { return GetBoolProperty("Enabled", true); }
			set {
				SetBoolProperty("Enabled", true, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SettingsLoadingPanelText"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, DefaultValue(StringResources.LoadingPanelText), Localizable(true)]
		public virtual string Text {
			get { return GetStringProperty("Text", ASPxperienceLocalizer.GetString(ASPxperienceStringId.Loading)); }
			set {
				SetStringProperty("Text", ASPxperienceLocalizer.GetString(ASPxperienceStringId.Loading), value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SettingsLoadingPanelImagePosition"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, DefaultValue(ImagePosition.Left)]
		public virtual ImagePosition ImagePosition {
			get { return (ImagePosition)GetEnumProperty("ImagePosition", ImagePosition.Left); }
			set {
				SetEnumProperty("ImagePosition", ImagePosition.Left, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SettingsLoadingPanelShowImage"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, DefaultValue(true)]
		public virtual bool ShowImage {
			get { return GetBoolProperty("ShowImage", true); }
			set {
				SetBoolProperty("ShowImage", true, value);
				Changed();
			}
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			SettingsLoadingPanel src = source as SettingsLoadingPanel;
			if(src != null) {
				Delay = src.Delay;
				Enabled = src.Enabled;
				ImagePosition = src.ImagePosition;
				ShowImage = src.ShowImage;
				Text = src.Text;
			}
		}
	}
}
namespace DevExpress.Web.Internal {
	public abstract class CustomCollection<T> : IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable, IStateManager {
		private List<T> items;
		public CustomCollection() { }
		public virtual T this[int index] {
			get { return Items[index]; }
			set {
				if(IsNullOrEmpty(value)) {
					Items.RemoveAt(index);
				} else {
					bool duplicateTokenRemoved = false;
					bool isEqualValue = value.Equals(Items[index]);
					if(isEqualValue) return;
					if(Contains(value)) {
						Items.Remove(value);
						duplicateTokenRemoved = true;
					}
					if(duplicateTokenRemoved && index == Items.Count)
						Items.Insert(index, value);
					else
						Items[index] = value;
					Changed();
				}
			}
		}
		public void Add(T item) {
			AddInternal(item);
		}
		public bool Contains(T item) {
			return Items.Contains(item);
		}
		public int IndexOf(T item) {
			return Items.IndexOf(item);
		}
		public void Insert(int index, T item) {
			bool isEqualValue = index < Items.Count && item.Equals(Items[index]);
			if(isEqualValue) return;
			if(Contains(item))
				Items.Remove(item);
			Items.Insert(index, item);
			Changed();
		}
		public bool Remove(T item) {
			return RemoveInternal(item);
		}
		public virtual void AddRange(IEnumerable<T> items) {
			List<T> list = items.ToList();
			list.RemoveAll(item => IsNullOrEmpty(item));
			if(list.Count > 0) {
				Items.AddRange(list.Distinct());
				SetItemsSilent(Items.Distinct().ToList());
				Changed();
			}
		}
		public void Clear() {
			Items.Clear();
			Changed();
		}
		public void Assign(CustomCollection<T> source) {
			Clear();
			AddRange(source.Items);
		}
		protected internal virtual void AddInternal(T item) {
			if(!Contains(item) && !IsNullOrEmpty(item)) {
				Items.Add(item);
				Changed();
			}
		}
		protected internal bool RemoveInternal(T item) {
			bool result = Items.Remove(item);
			Changed();
			return result;
		}
		public void RemoveAt(int index) {
			Items.RemoveAt(index);
			Changed();
		}
		public int Count {
			get { return Items.Count; }
		}
		public void CopyTo(T[] array, int arrayIndex) {
			Items.CopyTo(array, arrayIndex);
		}
		public bool IsReadOnly { get { return false; } }
		public IEnumerator<T> GetEnumerator() {
			return Items.GetEnumerator();
		}
		protected List<T> Items {
			get {
				if(items == null)
					items = new List<T>();
				return items;
			}
		}
		protected void SetItemsSilent(List<T> items) {
			this.items = items;
		}
		protected abstract void Changed();
		protected abstract bool IsNullOrEmpty(T value);
		#region ICollection Members
		void ICollection.CopyTo(Array array, int arrayIndex) { Items.CopyTo(array.Cast<T>().ToArray(), arrayIndex); }
		int ICollection.Count { get { return this.Count; } }
		bool ICollection.IsSynchronized { get { return false; } }
		object ICollection.SyncRoot { get { return null; } }
		#endregion
		#region IEnumerator Members
		IEnumerator IEnumerable.GetEnumerator() {
			return Items.GetEnumerator();
		}
		#endregion
		#region IList Members
		object IList.this[int index] {
			get { return this[index]; }
			set { if(value is T) this[index] = (T)value; }
		}
		int IList.Add(object item) { if(item is T) { Add((T)item); return Items.Count - 1; }; return -1; }
		bool IList.Contains(object item) { return (item is T) ? Contains((T)item) : false; }
		int IList.IndexOf(object item) { return (item is T) ? IndexOf((T)item) : -1; }
		void IList.Insert(int index, object item) { if(item is T) Insert(index, (T)item); }
		void IList.Remove(object item) { if(item is T) Remove((T)item); }
		bool IList.IsFixedSize { get { return false; } }
		#endregion
		#region IStateManager Members
		bool IStateManager.IsTrackingViewState {
			get { return Tracking; }
		}
		void IStateManager.LoadViewState(object state) {
			this.items = state as List<T>;
		}
		object IStateManager.SaveViewState() {
			List<T> items = Items;
			return items;
		}
		bool tracking;
		protected bool Tracking { set { tracking = value; } get { return tracking; } }
		void IStateManager.TrackViewState() {
			Tracking = true;
		}
		#endregion
	}
}
