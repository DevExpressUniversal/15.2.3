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
using DevExpress.Xpf.Core;
using System.Windows;
using System.ComponentModel;
using DevExpress.Xpf.PivotGrid.Internal;
using System.Collections.Generic;
using System.Collections;
using DevExpress.Utils.Serializing;
using CoreXtraPivotGrid = DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Xpf.Core.Native;
using DevExpress.XtraPivotGrid;
#if SL
using DXFrameworkContentElement = System.Windows.FrameworkElement;
using ApplicationException = System.Exception;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DevExpress.Xpf.Core.WPFCompatibility;
#else
using DependencyPropertyManager = System.Windows.DependencyProperty;
#endif
namespace DevExpress.Xpf.PivotGrid {
	public class PivotGridGroup : DXFrameworkContentElement, IEnumerable, IEnumerable<PivotGridField>
#if SL
		, IDependencyPropertyChangeListener
#endif
		{
		public const string GroupChangedString = "Group was changed";
		#region static stuff
		public static readonly DependencyProperty CaptionProperty;
		public static readonly DependencyProperty DisplayTextProperty;
		static readonly DependencyPropertyKey DisplayTextPropertyKey;
		public static readonly DependencyProperty AreaProperty;
		static readonly DependencyPropertyKey AreaPropertyKey;
		public static readonly DependencyProperty AreaIndexProperty;
		static readonly DependencyPropertyKey AreaIndexPropertyKey;
		public static readonly DependencyProperty CountProperty;
		static readonly DependencyPropertyKey CountPropertyKey;
		public static readonly DependencyProperty VisibleCountProperty;
		static readonly DependencyPropertyKey VisibleCountPropertyKey;
		public static readonly DependencyProperty VisibleProperty;
		static readonly DependencyPropertyKey VisiblePropertyKey;
		public
#if SL
		new 
#endif
		static readonly DependencyProperty WidthProperty;
		static readonly DependencyPropertyKey WidthPropertyKey;
		public static readonly DependencyProperty HierarchyProperty;
		public static readonly DependencyProperty ShowNewValuesProperty;
		public static readonly RoutedEvent ChangedEvent;
		static PivotGridGroup() {
			Type ownerType = typeof(PivotGridGroup);
			CaptionProperty = DependencyPropertyManager.Register("Caption", typeof(string),
				ownerType, new PropertyMetadata(string.Empty, (d,e) => ((PivotGridGroup)d).OnCaptionChanged((string)e.NewValue)));
			DisplayTextPropertyKey = DependencyPropertyManager.RegisterReadOnly("DisplayText", typeof(string),
				ownerType, new PropertyMetadata(string.Empty));
			DisplayTextProperty = DisplayTextPropertyKey.DependencyProperty;
			AreaPropertyKey = DependencyPropertyManager.RegisterReadOnly("Area", typeof(FieldArea),
				ownerType, new PropertyMetadata());
			AreaProperty = AreaPropertyKey.DependencyProperty;
			AreaIndexPropertyKey = DependencyPropertyManager.RegisterReadOnly("AreaIndex", typeof(int),
				ownerType, new PropertyMetadata());
			AreaIndexProperty = AreaIndexPropertyKey.DependencyProperty;
			CountPropertyKey = DependencyPropertyManager.RegisterReadOnly("Count", typeof(int),
				ownerType, new PropertyMetadata());
			CountProperty = CountPropertyKey.DependencyProperty;
			VisibleCountPropertyKey = DependencyPropertyManager.RegisterReadOnly("VisibleCount", typeof(int),
				ownerType, new PropertyMetadata());
			VisibleCountProperty = VisibleCountPropertyKey.DependencyProperty;
			VisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("Visible", typeof(bool),
				ownerType, new PropertyMetadata());
			VisibleProperty = VisiblePropertyKey.DependencyProperty;
			WidthPropertyKey = DependencyPropertyManager.RegisterReadOnly("Width", typeof(double),
				ownerType, new PropertyMetadata());
			WidthProperty = WidthPropertyKey.DependencyProperty;
			HierarchyProperty = DependencyPropertyManager.Register("Hierarchy",typeof(string), ownerType,new PropertyMetadata((d,e) => ((PivotGridGroup)d).OnHierarchyChanged((string)e.NewValue)));
			ShowNewValuesProperty = DependencyPropertyManager.Register("ShowNewValues", typeof(bool), ownerType, new PropertyMetadata((d1, e1) => OnPropertyChanged((d, e) => ((PivotGridGroup)d).SyncGroupShowNewValues(true, true), d1, e1)));
			ChangedEvent = EventManager.RegisterRoutedEvent("Changed", RoutingStrategy.Direct,
				typeof(GroupChangedEventHandler), ownerType);
		}
		public static string GetDisplayText(DependencyObject d) {
			if(d == null)
				throw new ArgumentNullException("d");
			return (string)d.GetValue(DisplayTextProperty);
		}
		static void OnPropertyChanged(PropertyChangedCallback baseCallback, DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PivotGridGroup g = (PivotGridGroup)d;
			PivotGridControl pivot = g.Data != null ? g.Data.PivotGrid : null;
			PivotGridWpfData.DataRelatedDPChanged(pivot, baseCallback, d, e);
		}
		#endregion
		PivotGridInternalGroup internalGroup;
		FieldsList groupFields;
		public PivotGridGroup() : this(new PivotGridInternalGroup()) { }
		protected internal PivotGridGroup(PivotGridInternalGroup internalGroup)
			: this(internalGroup, false) {
		}
		protected internal PivotGridGroup(PivotGridInternalGroup internalGroup, bool syncFields) {
			this.internalGroup = internalGroup;
			this.synchronizer = new PropertiesSynchronizer();
			InternalGroup.Wrapper = this;
			SubscribeEvents();
			if(syncFields)
				SyncFieldsCollection(true, true);
			SyncGroupProperties(true, true, null);
		}
		protected internal PivotGridInternalGroup InternalGroup {
			get { return internalGroup; }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridGroupCaption"),
#endif
		 XtraSerializableProperty(),
		 XtraSerializablePropertyId(PivotSerializationOptions.LayoutID)]
		public string Caption {
			get { return (string)GetValue(CaptionProperty); }
			set { SetValue(CaptionProperty, value); }
		}
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("PivotGridGroupDisplayText")]
#endif
		public string DisplayText {
			get { return (string)GetValue(DisplayTextProperty); }
			protected set { this.SetValue(DisplayTextPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("PivotGridGroupWidth")]
#endif
		public
#if SL
		new 
#endif
		double Width {
			get { return (double)GetValue(WidthProperty); }
			protected set { this.SetValue(WidthPropertyKey, value); }
		}
		public PivotGridField this[int index] {
			get {
				if(InternalGroup == null) return null;
				return ((PivotGridInternalField)InternalGroup[index]).GetWrapper();
			}
		}
		[
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.NameCollection, false, true, true, 0),
		XtraSerializablePropertyId(PivotSerializationOptions.LayoutID),
		]
		public IList GroupFields {
			get {
				if(groupFields == null)
					groupFields = new FieldsList(this);
				return groupFields; 
			}
		}
		[
		Browsable(false), 
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridGroupFilterValues"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Content,1),
		XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID)
		]
		public GroupFilterValues FilterValues {
			get { return InternalGroup.FilterValues; }
		}
		public event GroupChangedEventHandler Changed {
			add { this.AddHandler(ChangedEvent, value); }
			remove { this.RemoveHandler(ChangedEvent, value); }
		}
		[Browsable(false)]
		public FieldArea Area {
			get { return (FieldArea)GetValue(AreaProperty); }
			private set { this.SetValue(AreaPropertyKey, value); }
		}
		[Browsable(false)]
		public int AreaIndex {
			get { return (int)GetValue(AreaIndexProperty); }
			private set { this.SetValue(AreaIndexPropertyKey, value); }
		}
		[Browsable(false)]
		public bool Visible {
			get { return (bool)GetValue(VisibleProperty); }
			private set { this.SetValue(VisiblePropertyKey, value); }
		}
		[Browsable(false)]
		public int Count {
			get { return (int)GetValue(CountProperty); }
			private set { this.SetValue(CountPropertyKey, value); }
		}
		[Browsable(false)]
		public int VisibleCount {
			get { return (int)GetValue(VisibleCountProperty); }
			private set { this.SetValue(VisibleCountPropertyKey, value); }
		}
		[Browsable(false)]
		public int Index {
			get { return InternalGroup != null ? InternalGroup.Index : -1; }
		}
		[Browsable(false),
		 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		 EditorBrowsable(EditorBrowsableState.Never),
		 XtraSerializableProperty(),
		 XtraSerializablePropertyId(PivotSerializationOptions.LayoutID)]
		public string Hierarchy {
			get { return (string)GetValue(HierarchyProperty); }
			set { SetValue(HierarchyProperty, value); }
		}
		public bool ShowNewValues {
			get { return (bool)GetValue(ShowNewValuesProperty); }
			set { SetValue(ShowNewValuesProperty, value); }
		}
		[Browsable(false)]
		public PivotGridField FirstField {
			get {
				if(Count == 0) return null;
				return this[0];
			}
		}
		public void Add(PivotGridField field) {
			if(Contains(field)) return;
			field.Group = this;
			InternalGroup.Add(field.GetInternalField());
		}
		public void AddRange(params PivotGridField[] fields) {
			foreach(PivotGridField field in fields) {
				Add(field);
			}
		}
		public bool CanAdd(PivotGridField field) {
			return InternalGroup.CanAdd(field.GetInternalField());
		}
		public bool CanChangeArea(PivotGridField field) {
			return InternalGroup.CanChangeArea(field.GetInternalField());
		}
		public bool CanChangeAreaTo(FieldArea newArea, int newAreaIndex) {
			return InternalGroup.CanChangeAreaTo(newArea.ToPivotArea(), newAreaIndex);
		}
		public void Clear() {
			InternalGroup.Clear();
		}
		public int IndexOf(PivotGridField field) {
			return InternalGroup.IndexOf(field.GetInternalField());
		}
		public bool Contains(PivotGridField field) {
			return InternalGroup.Contains(field.GetInternalField());
		}
		public void Remove(PivotGridField field) {
			if(!Contains(field)) return;
			InternalGroup.Remove(field.GetInternalField());
			field.Group = null;
		}
		public void RemoveAt(int index) {
			Remove(this[index]);
		}
		public List<PivotGridField> GetVisibleFields() {
			List<PivotGridField> res = new List<PivotGridField>();
			foreach(DevExpress.XtraPivotGrid.PivotGridFieldBase field in InternalGroup.GetVisibleFields()) {
				res.Add(field.GetWrapper());
			}
			return res;
		}
		public List<object> GetUniqueValues(object[] parentValues) {
			return InternalGroup.GetUniqueValues(parentValues);
		}
		public bool IsFieldVisible(PivotGridField field) {
			return InternalGroup.IsFieldVisible(field.GetInternalField());
		}
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("PivotGridGroupIsFilterAllowed")]
#endif
		public bool IsFilterAllowed { get { return InternalGroup.IsFilterAllowed; } }
		public void SetShowNewValuesAsync(bool showNewValues, AsyncCompletedHandler asyncCompleted) {
			if(showNewValues == ShowNewValues)
				return;
			Data.InvertGroupFilterAsync(FilterValues, FilterValues.FilterType == FieldFilterType.Excluded ? CoreXtraPivotGrid.PivotFilterType.Included : CoreXtraPivotGrid.PivotFilterType.Excluded,
				true, 
				(d) => {
					ShowNewValues = showNewValues;
					if(asyncCompleted != null)
						asyncCompleted.Invoke(new AsyncOperationResult(d));
				});
		}
		protected void SubscribeEvents() {
			InternalGroup.Changed += OnChanged;
		}
		protected void UnsubscribeEvents() {
			InternalGroup.Changed -= OnChanged;
		}
		void OnChanged(object sender, EventArgs e) {
			OnChanged(GroupChangeType.InternalChange);
		}
		protected virtual void OnChanged(GroupChangeType change) {
			SyncGroupProperties(true, true, change);
		}
		protected PivotGridWpfData Data { get { return internalGroup.Data; } }
		#region SyncProperties
		PropertiesSynchronizer synchronizer;
		void SyncGroupProperties(bool read, bool write, GroupChangeType? changeType) {
			synchronizer.SyncProperty(Data, read, write, null, () => {
				Caption = InternalGroup.Caption;
				DisplayText = InternalGroup.ToString();
				Area = InternalGroup.Area.ToFieldArea();
				AreaIndex = InternalGroup.AreaIndex;
				Count = InternalGroup.Count;
				VisibleCount = InternalGroup.VisibleCount;
				Visible = InternalGroup.Visible;
				Hierarchy = InternalGroup.Hierarchy;
				ShowNewValues = InternalGroup.ShowNewValues;
				if(Count != VisibleCount)
					UpdateFieldsVisibility();
				UpdateWidth();
				UpdateFieldsOrder();
				if(changeType != null)
					this.RaiseEvent(new GroupChangedEventArgs(ChangedEvent, changeType.Value));
			});
		}
		internal void SyncGroupShowNewValues(bool read, bool write) {
			synchronizer.SyncProperty(Data, read, write, () => internalGroup.ShowNewValues = ShowNewValues, () => ShowNewValues = InternalGroup.ShowNewValues);
		}
		protected virtual void UpdateWidth() {
			double newWidth = 0;
			for(int i = 0; i < VisibleCount; i++) {
				newWidth += this[i].Width;
			}
			Width = newWidth;
		}
		protected virtual void UpdateFieldsVisibility() {
			for(int i = VisibleCount; i < Count; i++) {
				this[i].SyncFieldVisible(true, false);
			}
		}
		protected internal void UpdateFieldsOrder() {
			UnsubscribeEvents();
			List<PivotGridField> fieldlist = new List<PivotGridField>(this);
			fieldlist.Sort(delegate(PivotGridField x, PivotGridField y) {
				return x.GroupIndex.CompareTo(y.GroupIndex);
			});
			bool isChanged = false;
			for(int i = 0; i < fieldlist.Count; i++) {
				if(this[i] == fieldlist[i] || this[i].GroupIndex == fieldlist[i].GroupIndex) continue;
				ChangeFieldIndex(fieldlist[i], i);
				isChanged = true;
			}
			SubscribeEvents();
			if(isChanged)
				OnChanged(GroupChangeType.InternalChange);
		}
		protected void ChangeFieldIndex(PivotGridField field, int newIndex) {
			InternalGroup.ChangeFieldIndex(field.InternalField, newIndex);
		}
		void SyncFieldsCollection(bool read, bool write) {
			synchronizer.SyncProperty(Data, read, write, null, () => {
				foreach(CoreXtraPivotGrid.PivotGridFieldBase baseField in internalGroup.Fields) {
					PivotGridField field = baseField.GetWrapper();
					if(field == null) throw new ArgumentException("SyncFieldsCollection: field == null");
					field.Group = this;
				}
			});
		}
		#endregion
		internal void OnFieldExpandedChanged(PivotGridField field) {
			OnChanged(GroupChangeType.FieldExpandCollapse);
		}
		internal void OnFieldSizeChanged(PivotGridField field) {
			UpdateWidth();
		}
		protected void OnHierarchyChanged(string newValue) {
			if(internalGroup.Hierarchy != newValue)
				internalGroup.Hierarchy = newValue;
		}
		protected void OnCaptionChanged(string newValue) {
			if(InternalGroup.Caption != newValue) {
				InternalGroup.Caption = newValue;
				OnChanged(GroupChangeType.InternalChange);
			}
		}
		#region IEnumerable
		System.Collections.IEnumerator IEnumerable.GetEnumerator() {
			return new GroupEnumerator(this);
		}
		#endregion
		#region IEnumerable<PivotGridField> Members
		IEnumerator<PivotGridField> IEnumerable<PivotGridField>.GetEnumerator() {
			return new GroupEnumerator(this);
		}
		#endregion
		class GroupEnumerator : IEnumerator, IEnumerator<PivotGridField> {
			PivotGridGroup group;
			int position;
			bool isGroupChanged;
			public GroupEnumerator(PivotGridGroup group) {
				this.position = -1;
				this.group = group;
				this.group.Changed += OnGroupChanged;
			}
			void OnGroupChanged(object sender, GroupChangedEventArgs e) {
				if(e.ChangeType == GroupChangeType.InternalChange)
					isGroupChanged = true;
			}
			#region IEnumerator Members
			object IEnumerator.Current {
				get { return group[position]; }
			}
			bool IEnumerator.MoveNext() {
				position++;
				if(isGroupChanged)
					throw new InvalidOperationException(GroupChangedString);
				return position < group.Count;
			}
			void IEnumerator.Reset() {
				position = 0;
			}
			#endregion            
			#region IEnumerator<PivotGridField> Members
			PivotGridField IEnumerator<PivotGridField>.Current {
				get { return group[position]; }
			}
			#endregion
			#region IDisposable Members
			void IDisposable.Dispose() { }
			#endregion
		}
		class FieldsList : IList {
			PivotGridGroup group;
			public FieldsList(PivotGridGroup group) {
				this.group = group;
			}
			#region IList Members
			public int Add(object value) {
				group.Add((PivotGridField)value);
				return group.Count - 1;
			}
			public void Clear() {
				group.Clear();
			}
			public bool Contains(object value) {
				return group.Contains((PivotGridField)value);
			}
			public int IndexOf(object value) {
				return group.IndexOf((PivotGridField)value);
			}
			public void Insert(int index, object value) {
				throw new NotImplementedException();
			}
			public bool IsFixedSize { get { return false; } }
			public bool IsReadOnly { get { return false; } }
			public void Remove(object value) {
				group.Remove((PivotGridField)value);
			}
			public void RemoveAt(int index) {
				group.RemoveAt(index);
			}
			public object this[int index] {
				get { return group[index]; }
				set { throw new NotImplementedException(); }
			}
			#endregion
			#region ICollection Members
			public void CopyTo(Array array, int index) {
				throw new NotImplementedException();
			}
			public int Count {
				get { return group.Count; }
			}
			public bool IsSynchronized {
				get { return false; }
			}
			public object SyncRoot {
				get { throw new NotImplementedException(); }
			}
			#endregion
			#region IEnumerable Members
			public IEnumerator GetEnumerator() {
				return ((IEnumerable)group).GetEnumerator();
			}
			#endregion
		}
#if !SL
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
			PropertyChange(e);
		}
#else
		#region IDependencyPropertyChangeListener Members
		void IDependencyPropertyChangeListener.OnPropertyAssigning(DependencyProperty dp, object value) { }
		void IDependencyPropertyChangeListener.OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			PropertyChange(e);
		}
		#endregion
#endif
		void PropertyChange(DependencyPropertyChangedEventArgs e) {
			PivotGridControl PivotGrid = Parent as PivotGridControl;
			if(PivotGrid != null)
				PivotGrid.RaisePropertyChanged(e, this);
		}
		public override string ToString() {
			return DisplayText;
		}
	}	
	public delegate void GroupChangedEventHandler(object sender, GroupChangedEventArgs e);
	public class GroupChangedEventArgs : RoutedEventArgs {
		GroupChangeType changeType;
		public GroupChangedEventArgs(RoutedEvent routedEvent, GroupChangeType changeType) 
			: base(routedEvent)  {
			this.changeType = changeType;
		}
		public GroupChangeType ChangeType { get { return changeType; } }
	}
	public enum GroupChangeType {
		InternalChange,
		FieldExpandCollapse
	}
	public class PivotGridInternalGroup : DevExpress.XtraPivotGrid.PivotGridGroup {
		public event EventHandler Changed;
		public PivotGridGroup Wrapper { get; set; } 
		public override void OnChanged() {
			base.OnChanged();
			RaiseChanged();
		}
		protected void RaiseChanged() {
			if(Changed != null)
				Changed(this, EventArgs.Empty);
		}
		public new GroupFilterValues FilterValues { get { return (GroupFilterValues)base.FilterValues; } }
		protected override CoreXtraPivotGrid.PivotGroupFilterValues CreateFilterValues() {
			return new GroupFilterValues(this);
		}
		protected internal virtual new PivotGridWpfData Data { get { return base.Data as PivotGridWpfData; } }
		internal bool IsFieldValid(PivotGridField pivotGridField) {
			return base.IsFieldValid(pivotGridField.GetInternalField());
		}
	}
	public class PivotGridInternalGroupCollection : CoreXtraPivotGrid.PivotGridGroupCollection {
		public PivotGridInternalGroupCollection(PivotGridWpfData data)
			: base(data) {
		}
		protected override CoreXtraPivotGrid.PivotGridGroup CreateGroup() {
			return new PivotGridInternalGroup();
		}
	}
	public class PivotGridGroupCollection : PivotChildCollection<PivotGridGroup> {
		CoreXtraPivotGrid.PivotGridGroupCollection groups;
		PivotGridWpfData data;
		public PivotGridGroupCollection(PivotGridWpfData data, CoreXtraPivotGrid.PivotGridGroupCollection groups)
			: base(data.PivotGrid, true) {
			this.data = data;
			this.groups = groups;
			this.synchronizer = new PropertiesSynchronizer();
			Data.FieldSizeChanged += OnFieldSizeChanged;
		}
		protected PivotGridWpfData Data { get { return data; } }			 
		protected DevExpress.XtraPivotGrid.PivotGridGroupCollection Groups {
			get { return groups; }
		}
		public PivotGridGroup Add() {
			PivotGridGroup g = new PivotGridGroup();
			Add(g);
			return g;
		}
		public PivotGridGroup Add(params PivotGridField[] fields) {
			PivotGridGroup g = new PivotGridGroup();
			Add(g);
			g.AddRange(fields);
			return g;
		}
		protected override void OnItemAdded(int index, PivotGridGroup group) {
			base.OnItemAdded(index, group);
			SyncGroupInserted(false, true, index, group);
		}
		protected override void OnItemRemoved(int index, PivotGridGroup group) {
			base.OnItemRemoved(index, group);
			SyncGroupRemoved(false, true, group);
			OnGroupRemoved(group);
		}
		protected override void OnItemMoved(int oldIndex, int newIndex, PivotGridGroup group) {
			base.OnItemMoved(oldIndex, newIndex, group);
			SyncGroupMoved(false, true, oldIndex, newIndex, group);
		}
		protected override void OnItemReplaced(int index, PivotGridGroup oldGroup, PivotGridGroup newGroup) {
			base.OnItemReplaced(index, oldGroup, newGroup);
			SyncGroupReplaced(false, true, index, newGroup);
		}
		protected override void OnItemsClearing(IList<PivotGridGroup> oldItems) {
			Data.BeginUpdate();
			base.OnItemsClearing(oldItems);
			foreach(PivotGridGroup item in oldItems)
				OnGroupRemoved(item);
			Data.EndUpdate();
		}
		protected virtual void OnGroupRemoved(PivotGridGroup item) {
			for(int i = item.Count - 1; i >= 0; i--) {
				item[i].Group = null;					 
			}
		}
		protected void OnFieldSizeChanged(object sender, FieldSizeChangedEventArgs e) {
			if(e.IsWidthChanged && e.Field.Group != null) {
				PivotGridField field = e.Field.GetWrapper();
				field.Group.OnFieldSizeChanged(field);
			}
		}
		int lockUpdateCount;
		void BeginUpdate() {
			this.lockUpdateCount++;
		}
		void EndUpdate() {
			--this.lockUpdateCount;
		}
		bool IsLockUpdate { get { return this.lockUpdateCount > 0; } }
		#region SyncProperties
		PropertiesSynchronizer synchronizer;
		internal void SyncGroupCollection(bool read, bool write) {
			synchronizer.SyncProperty(Data, read, write, null, () => {
				List<PivotGridGroup> syncGroups = new List<PivotGridGroup>(this);
				BeginUpdate();
				foreach(PivotGridInternalGroup internalGroup in groups) {
					int index = -1;
					for(int i = 0; i < syncGroups.Count; i++)
						if(syncGroups[i].InternalGroup == internalGroup) {
							index = i;
							break;
						}
					if(index == -1) 
						this.Add(CreateGroup(internalGroup, true));
					else
						syncGroups.RemoveAt(index);
				}
				foreach(PivotGridGroup group in syncGroups) {
					foreach(PivotGridField field in Data.Fields) {
						if(field.Group == group)
							field.Group = null;
					}
					this.Remove(group);
				}
				EndUpdate();
			});
		}
		void SyncGroupInserted(bool read, bool write, int index, PivotGridGroup group) {
			if(IsLockUpdate) return;
			synchronizer.SyncProperty(Data, read, write, () => {
				Groups.Insert(index, group.InternalGroup);
			}, null);
		}
		void SyncGroupRemoved(bool read, bool write, PivotGridGroup group) {
			if(IsLockUpdate) return;
			synchronizer.SyncProperty(Data, read, write, () => {
				Groups.Remove(group.InternalGroup);
			}, null);
		}
		void SyncGroupMoved(bool read, bool write, int oldIndex, int newIndex, PivotGridGroup group) {
			if(IsLockUpdate) return;
			synchronizer.SyncProperty(Data, read, write, () => {
				Data.BeginUpdate();
				Groups.RemoveAt(oldIndex);
				Groups.Insert(newIndex, group.InternalGroup);
				Data.EndUpdate();
			}, null);
		}
		void SyncGroupReplaced(bool read, bool write, int index, PivotGridGroup newGroup) {
			if(IsLockUpdate) return;
			synchronizer.SyncProperty(Data, read, write, () => {
				Data.BeginUpdate();
				Groups.RemoveAt(index);
				Groups.Insert(index, newGroup.InternalGroup);
				Data.EndUpdate();
			}, null);
		}
		#endregion
		PivotGridGroup CreateGroup(PivotGridInternalGroup internalGroup, bool syncFields) {
			PivotGridGroup group = new PivotGridGroup(internalGroup, syncFields);
			Data.PivotGrid.AddChild(group);
			return group;
		}
	}
	public class GroupFilterValues : CoreXtraPivotGrid.PivotGroupFilterValues {
		public GroupFilterValues(PivotGridInternalGroup group)
			: base(group) {
		}
		protected new PivotGridInternalGroup Group { get { return (PivotGridInternalGroup)base.Group; } }
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("GroupFilterValuesFilterType")]
#endif
		[XtraSerializableProperty(1), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID)]
		public new FieldFilterType FilterType {
			get { return base.FilterType.ToFieldFilterType(); }
			set { base.FilterType = value.ToPivotFilterType(); }
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new bool SetValuesAsync(PivotGroupFilterValuesCollection values, PivotFilterType filterType, DevExpress.XtraPivotGrid.AsyncCompletedHandler handler) {
			return base.SetValuesAsync(values, filterType, handler);
		}
		public bool SetValuesAsync(PivotGroupFilterValuesCollection values, FieldFilterType filterType, AsyncCompletedHandler handler) {
			return SetValuesAsync(values, filterType.ToPivotFilterType(), handler.ToCoreAsyncCompleted());
		}
		protected override void OnChanged(bool isAsync, CoreXtraPivotGrid.AsyncCompletedHandler handler) {
			base.OnChanged(isAsync, handler);
			if(IsLockUpdate || Group == null || Group.Wrapper == null) return;
				Group.Wrapper.SyncGroupShowNewValues(true, false);
		}
	}
}
