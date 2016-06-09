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
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core.Native;
using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.Xpf.Data;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.Filtering;
namespace DevExpress.Xpf.Grid {
	public abstract class DetailDescriptorBase : DXFrameworkContentElement {
		#region static
		static readonly DataTemplate DefaultHeaderContentTemplate = XamlHelper.GetTemplate(@"<TextBlock Text=""{Binding}""/>"); 
		public static readonly DependencyProperty ShowHeaderProperty;
		public static readonly DependencyProperty HeaderContentTemplateProperty;
		public static readonly DependencyProperty ContentTemplateProperty;
		static DetailDescriptorBase() {
			Type ownerType = typeof(DetailDescriptorBase);
			ShowHeaderProperty = DependencyPropertyManager.Register("ShowHeader", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, (d, e) => ((DetailDescriptorBase)d).InvalidateTree()));
			HeaderContentTemplateProperty = DependencyPropertyManager.Register("HeaderContentTemplate", typeof(DataTemplate), ownerType, new FrameworkPropertyMetadata(DefaultHeaderContentTemplate));
			ContentTemplateProperty = DependencyPropertyManager.Register("ContentTemplate", typeof(DataTemplate), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((DetailDescriptorBase)d).InvalidateTree()));
		}
		#endregion
		IDetailDescriptorOwner owner;
		internal IDetailDescriptorOwner Owner { 
			get { return owner ?? NullDetailDescriptorOwner.Instance; } 
			set { 
				owner = value;
				OnMasterControlChanged();
			} 
		}
		protected virtual void OnMasterControlChanged() {
			UpdateMasterControl();
		}
		internal virtual void UpdateMasterControl() {
		}
		public DetailDescriptorBase() {
			DetailViewIndents = new ObservableCollection<DetailIndent>();
		}
		public virtual IEnumerable<DetailDescriptorContainer> DataControlDetailDescriptors { get { yield return new DetailDescriptorContainer(null); } }
		public bool ShowHeader {
			get { return (bool)GetValue(ShowHeaderProperty); }
			set { SetValue(ShowHeaderProperty, value); }
		}
		public ObservableCollection<DetailIndent> DetailViewIndents { get; protected set; }
		public DataTemplate HeaderContentTemplate {
			get { return (DataTemplate)GetValue(HeaderContentTemplateProperty); }
			set { SetValue(HeaderContentTemplateProperty, value); }
		}
		public DataTemplate ContentTemplate {
			get { return (DataTemplate)GetValue(ContentTemplateProperty); }
			set { SetValue(ContentTemplateProperty, value); }
		}
		DetailSynchronizationQueues synchronizationQueues;
		internal DetailSynchronizationQueues SynchronizationQueues {
			get {
				if(synchronizationQueues == null)
					synchronizationQueues = new DetailSynchronizationQueues();
				return synchronizationQueues;
			}
		}
		internal void InvalidateTree() {
			Owner.InvalidateTree();
		}
		internal abstract DetailInfoWithContent CreateRowDetailInfo(RowDetailContainer container);
		internal virtual void SynchronizeDetailTree() {
			SynchronizationQueues.SynchronizeUnsynchronizedNodes();
		}
		internal abstract void UpdateDetailDataControls(Action<DataControlBase> updateOpenDetailMethod, Action<DataControlBase> updateClosedDetailMethod = null);
		internal abstract void UpdateOriginationDataControls(Action<DataControlBase> updateMethod);
		internal abstract void OnDetach();
		internal virtual void UpdateDetailViewIndents(ObservableCollection<DetailIndent> ownerIndents, Thickness margin) {
			SetDetailViewIndentsCollectionSize((ownerIndents == null ? 0 : ownerIndents.Count) + 1);
			int level = 1;
			for(int i = 0; i < DetailViewIndents.Count - 1; i++) {
				DetailViewIndents[i].Update(ownerIndents[i].Width, ownerIndents[i].WidthRight, ownerIndents[i].Level);
				level = ownerIndents[i].Level + 1;
			}
			DetailViewIndents[DetailViewIndents.Count - 1].Update(margin.Left, margin.Right, level);
		}
		void SetDetailViewIndentsCollectionSize(int count) {
			if(DetailViewIndents.Count > count) {
				for(int i = DetailViewIndents.Count - 1; i > count - 1; i--) {
					DetailViewIndents.RemoveAt(i);
				}
			}
			if(DetailViewIndents.Count < count) {
				for(int i = 0; i < count - DetailViewIndents.Count; i++) {
					DetailViewIndents.Add(new DetailIndent());
				}
			}
		}
		internal abstract DataControlBase GetChildDataControl(DataControlBase dataControl, int rowHandle, object detailRow);
	}
	[ContentProperty(DataControlPropertyName)]
	public class DataControlDetailDescriptor : DetailDescriptorBase, IDataControlOwner {
		internal const string DataControlPropertyName = "DataControl";
		#region static
		public static readonly DependencyProperty DataControlProperty;
		public static readonly DependencyProperty ItemsSourceValueConverterProperty;
		static readonly DependencyPropertyKey HeaderContentPropertyKey;
		public static readonly DependencyProperty HeaderContentProperty;
		public static readonly DependencyProperty ItemsSourcePathProperty;
		public static readonly DependencyProperty ParentPathProperty;
		static DataControlDetailDescriptor() {
			Type ownerType = typeof(DataControlDetailDescriptor);
			DataControlProperty = DependencyPropertyManager.Register(DataControlPropertyName, typeof(DataControlBase), ownerType,
				new PropertyMetadata(null, (d, e) => ((DataControlDetailDescriptor)d).OnDataControlChanged((DataControlBase)e.OldValue)));
			HeaderContentPropertyKey = DependencyPropertyManager.RegisterReadOnly("HeaderContent", typeof(object), ownerType, new PropertyMetadata(null));
			HeaderContentProperty = HeaderContentPropertyKey.DependencyProperty;
			ItemsSourceValueConverterProperty = DependencyPropertyManager.Register("ItemsSourceValueConverter", typeof(IValueConverter), ownerType,
				new PropertyMetadata(null));
			ItemsSourcePathProperty = DependencyPropertyManager.Register("ItemsSourcePath", typeof(string), ownerType, new PropertyMetadata(String.Empty));
			ParentPathProperty = DependencyPropertyManager.Register("ParentPath", typeof(string), ownerType, new PropertyMetadata(String.Empty));
		}
		#endregion
		void OnDataControlChanged(DataControlBase oldValue) {
			if(oldValue != null) {
				this.RemoveLogicalChild(oldValue);
				oldValue.DataControlOwner = null;
			}
			if(DataControl != null) { 
				this.AddLogicalChild(DataControl);
				DataControl.DataControlOwner = this;
				DataControl.UpdateOwnerDetailDescriptor();
			}
			ValidateMasterDetailConsistency();
			UpdateMasterControl();
		}
		readonly DetailDescriptorContainer detailDescriptorContainer;
		public override IEnumerable<DetailDescriptorContainer> DataControlDetailDescriptors { get { yield return detailDescriptorContainer; } } 
#if !SL // TODO SL
		protected override IEnumerator LogicalChildren {
			get { return DataControlBase.GetSingleObjectEnumerator(DataControl); }
		}
#endif
		public object HeaderContent { get { return GetValue(HeaderContentProperty); } }
		public DataControlBase DataControl {
			get { return (DataControlBase)GetValue(DataControlProperty); }
			set { SetValue(DataControlProperty, value); }
		}
		public BindingBase ItemsSourceBinding { get; set; }
		public string ItemsSourcePath {
			get { return (string)GetValue(ItemsSourcePathProperty); }
			set { SetValue(ItemsSourcePathProperty, value); }
		}
		public IValueConverter ItemsSourceValueConverter {
			get { return (IValueConverter)GetValue(ItemsSourceValueConverterProperty); }
			set { SetValue(ItemsSourceValueConverterProperty, value); }
		}
		public string ParentPath {
			get { return (string)GetValue(ParentPathProperty); }
			set { SetValue(ParentPathProperty, value); }
		}
		CustomGetParentEventEventHandler customGetParentEvent;
		public event CustomGetParentEventEventHandler CustomGetParent {
			add { customGetParentEvent += value; }
			remove { customGetParentEvent -= value; }
		}
		PropertyChangeListener headerContentListener;
		public DataControlDetailDescriptor() {
			detailDescriptorContainer = new DetailDescriptorContainer(this);
			headerContentListener = PropertyChangeListener.Create(
				new Binding(DataControlProperty.GetName() + ".View." + DataViewBase.DetailHeaderContentProperty.GetName()) { Source = this },
				obj => this.SetValue(HeaderContentPropertyKey, obj));
		}
		internal override DetailInfoWithContent CreateRowDetailInfo(RowDetailContainer container) {
			return new DataControlDetailInfo(this, container);
		}
		internal override void SynchronizeDetailTree() {
			base.SynchronizeDetailTree();
			if(DataControl != null) {
				DataControl.MasterDetailProvider.SynchronizeDetailTree();
			}
		}
		internal override void UpdateDetailDataControls(Action<DataControlBase> updateOpenDetailMethod, Action<DataControlBase> updateClosedDetailMethod = null) {
			if(DataControl != null) {
				DataControlOriginationElementHelper.EnumerateDependentElemetsSkipOriginationControl(DataControl, dataControl => dataControl, updateOpenDetailMethod, updateClosedDetailMethod);
				DataControl.MasterDetailProvider.UpdateDetailDataControls(updateOpenDetailMethod, updateClosedDetailMethod);
			}
		}
		internal override void UpdateOriginationDataControls(Action<DataControlBase> updateMethod) {
			if(DataControl != null) {
				updateMethod(DataControl);
				DataControl.MasterDetailProvider.UpdateOriginationDataControls(updateMethod);
			}
		}
		internal override void UpdateMasterControl() {
			if(DataControl != null && DataControl.DataView != null) {
				DataControl.DataView.UpdateMasterDetailViewProperties();
			}
		}
		internal override void UpdateDetailViewIndents(ObservableCollection<DetailIndent> ownerIndents, Thickness margin) {
			base.UpdateDetailViewIndents(ownerIndents, margin);
			if(DataControl != null)
				DataControl.UpdateChildrenDetailViewIndents(DetailViewIndents);
		}
		internal BindingBase GetItemsSourceBinding() {
			if(!string.IsNullOrEmpty(ItemsSourcePath) || ItemsSourceValueConverter!=null)
				return new Binding(ItemsSourcePath ?? String.Empty) { Converter = ItemsSourceValueConverter};
			return ItemsSourceBinding;
		}
		void ValidateMasterDetailConsistency() {
			if(DataControl == null)
				return;
			DataControl.ThrowNotSupportedInDetailException();
			DataControl.ThrowNotSupportedInMasterDetailException();
			if(DataControl.DataView == null)
				return;
			DataControl.DataView.ThrowNotSupportedInMasterDetailException();
			DataControl.DataView.ThrowNotSupportedInDetailException();
		}
		internal override void OnDetach() {
			SynchronizationQueues.Clear();
		}
		bool isColumnsPopulated = false;
		internal void PopulateColumnsIfNeeded(DataProviderBase dataProvider) {
			if(isColumnsPopulated) return;
			if(DataControl.ShouldPopulateColumns()) {
				DataControl.PopulateColumnsIfNeeded(dataProvider);
			}
			DataControl.syncPropertyLocker.DoLockedAction(() => {
				foreach (ColumnBase column in DataControl.ColumnsCore)
					column.UpdateColumnTypeProperties(dataProvider);
			});
			isColumnsPopulated = true;
		}
		#region IDataControlOwner Members
		void IDataControlOwner.EnumerateOwnerDataControls(Action<DataControlBase> action) {
			Owner.EnumerateOwnerDataControls(action);
		}
		bool IDataControlOwner.CanSortColumn(ColumnBase column) {
			return column.GetActualAllowSorting();
		}
		bool IDataControlOwner.CanGroupColumn(ColumnBase column) {
			return column.GetActualAllowGroupingCore();
		}
		void IDataControlOwner.ValidateMasterDetailConsistency() {
			ValidateMasterDetailConsistency();
		}
		DataControlBase IDataControlOwner.FindDetailDataControlByRow(object detailRow) {
			return FindDetailDataControlByRow(detailRow);
		}
		object IDataControlOwner.GetParentRow(object detailRow) {
			return GetParentRow(detailRow);
		}
		#endregion
		DataControlBase FindDetailDataControlByRow(object detailRow) {
			Stack<object> parentRows = new Stack<object>();
			object currentRow = detailRow;
			DataControl.EnumerateThisAndOwnerDataControls(dataControl => {
				parentRows.Push(currentRow);
				if(dataControl == DataControl.GetRootDataControl())
					return;
				currentRow = dataControl.DataControlOwner.GetParentRow(currentRow);
			});
			DataControlBase currentDataControl = DataControl.GetRootDataControl();
			int parentRowHandle = DataControlBase.InvalidRowHandle;
			foreach(object row in parentRows) {
				if(parentRowHandle != DataControlBase.InvalidRowHandle)
					currentDataControl = currentDataControl.DetailDescriptorCore.GetChildDataControl(currentDataControl, parentRowHandle, row);
				parentRowHandle = currentDataControl.DataProviderBase.FindRowByRowValue(row);
				if(parentRowHandle == DataControlBase.InvalidRowHandle)
					return null;
			}
			return currentDataControl;
		}
		object GetParentRow(object detailRow) {
			CustomGetParentEventArgs eventArgs = new CustomGetParentEventArgs(detailRow);
			if(customGetParentEvent != null)
				customGetParentEvent(DataControl, eventArgs);
			if(eventArgs.Handled)
				return eventArgs.Parent;
			if(string.IsNullOrEmpty(ParentPath) || detailRow == null)
				return null;
			Type rowType = detailRow.GetType();
			var descriptor = new CriteriaCompiledContextDescriptorDescripted(TypeDescriptor.GetProperties(rowType));
			CriteriaOperator filterCriteria = CriteriaOperator.Parse(ParentPath);
			var fit = CriteriaCompiler.ToUntypedDelegate(filterCriteria, descriptor);
			return fit(detailRow);
		}
		internal override DataControlBase GetChildDataControl(DataControlBase parent, int parentRowHandle, object detailRow) {
			parent.MasterDetailProvider.SetMasterRowExpanded(parentRowHandle, true, this);
			DataControlBase detailDataControl = parent.MasterDetailProvider.FindDetailDataControl(parentRowHandle, this);
			if(detailDataControl.DataProviderBase.FindRowByRowValue(detailRow) != DataControlBase.InvalidRowHandle)
				return detailDataControl;
			return null;
		}
	}
	public class ContentDetailDescriptor : DetailDescriptorBase {
		#region static
		public static readonly DependencyProperty HeaderContentProperty;
		static ContentDetailDescriptor() {
			Type ownerType = typeof(ContentDetailDescriptor);
			HeaderContentProperty = DependencyPropertyManager.Register("HeaderContent", typeof(object), ownerType, new FrameworkPropertyMetadata(null));
		}
		#endregion
		[TypeConverter(typeof(ObjectConverter))]
		public object HeaderContent { 
			get { return GetValue(HeaderContentProperty); }
			set { SetValue(HeaderContentProperty, value); }
		}
		internal override DetailInfoWithContent CreateRowDetailInfo(RowDetailContainer container) {
			return new ContentDetailInfo(this, container);
		}
		internal override void UpdateDetailDataControls(Action<DataControlBase> updateOpenDetailMethod, Action<DataControlBase> updateClosedDetailMethod = null) { }
		internal override void UpdateOriginationDataControls(Action<DataControlBase> updateMethod) { }
		internal override void OnDetach() { }
		internal override DataControlBase GetChildDataControl(DataControlBase dataControl, int rowHandle, object detailRow) {
			return null;
		}
	}
	public abstract class MultiDetailDescriptor : ContentDetailDescriptor, IDetailDescriptorOwner {
		readonly ObservableCollectionCore<DetailDescriptorContainer> dataControlDescriptors = new ObservableCollectionCore<DetailDescriptorContainer>();
		public DetailDescriptorCollection DetailDescriptors { get; private set; }
		void UpdateNestedDetailDescriptorsCache() {
			dataControlDescriptors.BeginUpdate();
			dataControlDescriptors.Clear();
			foreach(DetailDescriptorBase descriptor in DetailDescriptors)
				foreach(DetailDescriptorContainer container in descriptor.DataControlDetailDescriptors)
					dataControlDescriptors.Add(container);
			dataControlDescriptors.EndUpdate();
			MultiDetailDescriptor owner = Owner as MultiDetailDescriptor;
			if(owner == null) return;
			owner.UpdateNestedDetailDescriptorsCache();
		}
		public override IEnumerable<DetailDescriptorContainer> DataControlDetailDescriptors { get { return dataControlDescriptors; } }
		protected MultiDetailDescriptor() {
			DetailDescriptors = new DetailDescriptorCollection(this);
		}
		internal override void SynchronizeDetailTree() {
			base.SynchronizeDetailTree();
			UpdateChildDetailDescriptors(detailDescriptor => detailDescriptor.SynchronizeDetailTree());
		}
		internal override void UpdateDetailDataControls(Action<DataControlBase> updateOpenDetailMethod, Action<DataControlBase> updateClosedDetailMethod = null) {
			UpdateChildDetailDescriptors(detailDescriptor => detailDescriptor.UpdateDetailDataControls(updateOpenDetailMethod, updateClosedDetailMethod));
		}
		internal override void UpdateOriginationDataControls(Action<DataControlBase> updateMethod) {
			UpdateChildDetailDescriptors(detailDescriptor => detailDescriptor.UpdateOriginationDataControls(updateMethod));
		}
		internal void OnDescriptorAdded(DetailDescriptorBase descriptor) {
			AddLogicalChild(descriptor);
			descriptor.Owner = this;
			UpdateNestedDetailDescriptorsCache();
		}
		internal void OnDescriptorRemoved(DetailDescriptorBase descriptor) {
			descriptor.Owner = null;
			RemoveLogicalChild(descriptor);
			UpdateNestedDetailDescriptorsCache();
		}
#if !SL
		protected override IEnumerator LogicalChildren { get { return DetailDescriptors.GetEnumerator(); } }
#endif
		internal override void UpdateDetailViewIndents(ObservableCollection<DetailIndent> ownerIndents, Thickness margin) {
			base.UpdateDetailViewIndents(ownerIndents, margin);
			UpdateChildDetailDescriptors(detailDescriptor => detailDescriptor.UpdateDetailViewIndents(ownerIndents, margin));
		}
		internal override void UpdateMasterControl() {
			UpdateChildDetailDescriptors(detailDescriptor => detailDescriptor.UpdateMasterControl());
		}
		internal override void OnDetach() {
			UpdateChildDetailDescriptors(detailDescriptor => detailDescriptor.OnDetach());
		}
		void UpdateChildDetailDescriptors(Action<DetailDescriptorBase> updateAction) {
			foreach(DetailDescriptorBase detailDescriptor in DetailDescriptors) {
				updateAction(detailDescriptor);
			}
		}
		internal override DataControlBase GetChildDataControl(DataControlBase parent, int parentRowHandle, object detailRow) {
			DataControlBase detailDataControl = null;
			UpdateChildDetailDescriptors(detailDescriptor => {
				detailDataControl = detailDescriptor.GetChildDataControl(parent, parentRowHandle, detailRow);
				if(detailDataControl != null)
					return;
			});
			return detailDataControl;
		}
		#region IDetailDescriptorOwner Members
		void IDetailDescriptorOwner.InvalidateTree() {
			InvalidateTree();
		}
		void IDetailDescriptorOwner.EnumerateOwnerDataControls(Action<DataControlBase> action) {
			Owner.EnumerateOwnerDataControls(action);
		}
		bool IDetailDescriptorOwner.CanAssignTo(DataControlBase dataControl) {
			throw new InvalidOperationException("Specified detail descriptor is already the child of another detail descriptor.");
		}
		#endregion
	}
	public class TabViewDetailDescriptor : MultiDetailDescriptor {
		internal override DetailInfoWithContent CreateRowDetailInfo(RowDetailContainer container) {
			return new TabsDetailInfo(this, container);
		}
	}
	public class DetailDescriptorCollection : ObservableCollection<DetailDescriptorBase> {
		readonly MultiDetailDescriptor owner;
		public DetailDescriptorCollection(MultiDetailDescriptor owner) {
			this.owner = owner;
		}
		protected override void InsertItem(int index, DetailDescriptorBase item) {
			base.InsertItem(index, item);
			owner.OnDescriptorAdded(item);
		}
		protected override void RemoveItem(int index) {
			owner.OnDescriptorRemoved(this[index]);
			base.RemoveItem(index);
		}
		protected override void SetItem(int index, DetailDescriptorBase item) {
			owner.OnDescriptorRemoved(this[index]);
			base.SetItem(index, item);
			owner.OnDescriptorAdded(item);
		}
		protected override void ClearItems() {
			foreach(DetailDescriptorBase descriptor in this) {
				owner.OnDescriptorRemoved(descriptor);
			}
			base.ClearItems();
		}
	}
	public class DetailDescriptorContainer : INotifyPropertyChanged {
		public DetailDescriptorBase Content { get; private set; }
		public DetailDescriptorContainer(DetailDescriptorBase content) {
			this.Content = content;
		}
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged { add {  } remove {  } }
	}
	public class DetailDescriptorCollectionConverter : IValueConverter {
		object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return value is DetailDescriptorCollection ? SimpleBridgeReadonlyObservableCollection<DetailDescriptorContainer, DetailDescriptorBase>.Create((DetailDescriptorCollection)value, detailDescriptor => new DetailDescriptorContainer(detailDescriptor)) : null;
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	[Serializable]
	public class NotSupportedInMasterDetailException : NotSupportedException {
		const string HelpLinkString = "http://go.devexpress.com/XPF-MasterDetail-Limitations.aspx";
		const string LearnMoreMessage = " For a complete list of limitations, please see " + HelpLinkString;
		internal const string
			HitTestInfoCanBeCalculatedOnlyOnTheMasterViewLevel = "Hit testing is supported only by a master view. Hit information cannot be calculated for detail views.",
			ServerAndInstantFeedbackModeNotSupported = "Server and Instant Feedback UI modes are not supported in master-detail mode." + LearnMoreMessage,
			ICollectionViewNotSupported = "ICollectionView is not supported in master-detail mode." + LearnMoreMessage,
			AutoFilterRowNotSupported = "The Auto-Filter Row feature is not supported by detail grids." + LearnMoreMessage,
			NewItemRowNotSupported = "The New Item Row feature is not supported by detail grids." + LearnMoreMessage,
			MultiSelectionNotSupported = "Multiple cell selection and selection via the checkbox column is not supported in master-detail mode." + LearnMoreMessage,
			OnlyTableViewSupported = "CardView and TreeListView are not supported in master-detail mode." + LearnMoreMessage,
			OnlyGridControlSupported = "TreeListControl cannot be used to represent detail data." + LearnMoreMessage,
			BandsNotSupported = "Banded layout is not supported in Master-Detail mode." + LearnMoreMessage;
		public NotSupportedInMasterDetailException(string message)
			: base(message) {
#if !SL
			HelpLink = HelpLinkString;
#endif
		}
#if !SL
		public override string HelpLink { get; set; }
#endif
	}
}
namespace DevExpress.Xpf.Grid.Native {
	public interface IDetailDescriptorOwner {
		void InvalidateTree();
		void EnumerateOwnerDataControls(Action<DataControlBase> action);
		bool CanAssignTo(DataControlBase dataControl);
	}
	public class NullDetailDescriptorOwner : IDetailDescriptorOwner {
		public static readonly IDetailDescriptorOwner Instance = new NullDetailDescriptorOwner();
		NullDetailDescriptorOwner() { }
		void IDetailDescriptorOwner.InvalidateTree() { }
		void IDetailDescriptorOwner.EnumerateOwnerDataControls(Action<DataControlBase> action) { }
		bool IDetailDescriptorOwner.CanAssignTo(DataControlBase dataControl) { return true; }
	}
	public interface IDataControlOwner {
		void EnumerateOwnerDataControls(Action<DataControlBase> action);
		void ValidateMasterDetailConsistency();
		bool CanSortColumn(ColumnBase column);
		bool CanGroupColumn(ColumnBase column);
		DataControlBase FindDetailDataControlByRow(object detailRow);
		object GetParentRow(object detailRow);
	}
	public class NullDataControlOwner : IDataControlOwner {
		public static readonly IDataControlOwner Instance = new NullDataControlOwner();
		NullDataControlOwner() { }
		void IDataControlOwner.EnumerateOwnerDataControls(Action<DataControlBase> action) { }
		void IDataControlOwner.ValidateMasterDetailConsistency() { }
		bool IDataControlOwner.CanSortColumn(ColumnBase column) {
			if(column.OwnerControl == null)
				return false;
			return column.ActualAllowSorting && column.OwnerControl.DataProviderBase.CanSortCollectionView();
		}
		bool IDataControlOwner.CanGroupColumn(ColumnBase column) {
			if(column.OwnerControl == null)
				return false;
			return column.ActualAllowGroupingCore && column.OwnerControl.DataProviderBase.CanGroupCollectionView();
		}
		DataControlBase IDataControlOwner.FindDetailDataControlByRow(object detailRow) {
			return null;
		}
		object IDataControlOwner.GetParentRow(object detailRow) {
			return null;
		}
	}
}
