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
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Collections;
using DevExpress.Data.Helpers;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.Data {
	public interface IRealTimeListChangeProcessor {
		List<RealTimeProxyForObject> Cache { get; set; }
		ListChangedEventHandler ListChanged { get; }
		RealTimePropertyDescriptorCollection PropertyDescriptorsCollection { get; set; }
		void NotifyLastProcessedCommandCreationTime(DateTime sent);
		bool IsCatchUp { get; set; }
	}
	[DXToolboxItem(true)]
	[ToolboxBitmap(typeof(ResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "RealTimeSource.bmp")]
	[Designer("DevExpress.Xpo.Design.RealtimeSourceDesigner, " + AssemblyInfo.SRAssemblyXpoDesign)]
	[DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabData)]
	public class RealTimeSource : Component, IListSource, IDXCloneable {
		private bool ignoreItemEvents;
		object source;
		object dataSource;
		bool _IsDisposed;
		bool isSuspended;
		object suspendDataSource;
		RealTimeSourceCore _RealTimeCore;
		internal RealTimeSourceCore RealTimeCore {
			get {
				if(_RealTimeCore == null)
					_RealTimeCore = CreateRuntimeCore();
				return _RealTimeCore;
			}
			set { _RealTimeCore = value; }
		}
		bool useWeakEventHandler = true;
#if !SL
		RealTimeSourceDesignTimeWrapper _DTWrapper;
		internal RealTimeSourceDesignTimeWrapper DTWrapper {
			get {
				if(_DTWrapper == null)
					_DTWrapper = CreateDesignTimeWrapper();
				return _DTWrapper;
			}
			set { _DTWrapper = value; }
		}
#endif
		[AttributeProvider(typeof(IListSource)), DefaultValue(null), Category("Data"), RefreshProperties(RefreshProperties.All)]
#if !SL
	[DevExpressDataLocalizedDescription("RealTimeSourceDataSource")]
#endif
		public object DataSource {
			get {
				if(this.source == null)
					return this.dataSource;
				if(this.source == this._RealTimeCore)
					return this._RealTimeCore.DataSource;
				else
					return this._DTWrapper.DataSource;
			}
			set {
				if(_IsDisposed)
					return;
				IList listSource;
				if(value != null) {
					if(value is IList)
						listSource = (IList)value;
					else
						if(value is IListSource) {
							if(((IListSource)value).ContainsListCollection)
								return;
							else
								listSource = ((IListSource)value).GetList();
						} else
							throw new InvalidOperationException("DataSource");
				} else
					listSource = null;
				DisplayableProperties = GetDefaultDisplayableProperties(listSource);
				if(this.source == null) {
					if(this.dataSource == listSource)
						return;
					else {
						this.dataSource = listSource;
					}
				} else {
					if(this.source == this._DTWrapper)
						if(this._DTWrapper.DataSource == listSource)
							return;
						else
							this._DTWrapper.DataSource = listSource;
					if(this.source == this._RealTimeCore) {
						if(this._RealTimeCore.DataSource == listSource)
							return;
						this._RealTimeCore.DataSource = listSource;
					}
				}
				_DisplayableProperties = string.Empty;
				ForceCatchUp();
				this.isSuspended = false;
			}
		}
		string _DisplayableProperties;
		[Editor("DevExpress.Design.RealTimeSourcePropertiesEditor, " + AssemblyInfo.SRAssemblyDesign, "System.Drawing.Design.UITypeEditor, System.Drawing")]
		[RefreshProperties(RefreshProperties.All)]
#if !SL
	[DevExpressDataLocalizedDescription("RealTimeSourceDisplayableProperties")]
#endif
		public string DisplayableProperties {
			get {
				if(string.IsNullOrEmpty(_DisplayableProperties))
					_DisplayableProperties = GetDefaultDisplayableProperties(DataSource);
				return _DisplayableProperties;
			}
			set {
				if(DisplayableProperties == value)
					return;
				_DisplayableProperties = value;
				ForceCatchUp();
			}
		}
		[DefaultValue(false)]
#if !SL
	[DevExpressDataLocalizedDescription("RealTimeSourceIgnoreItemEvents")]
#endif
		public bool IgnoreItemEvents {
			get { return ignoreItemEvents; }
			set { ignoreItemEvents = value; }
		}
		public static int SendQueueTimeout = 2000;
		[Browsable(false)]
		public bool UseWeakEventHandler {
			get { return useWeakEventHandler; }
			set {
				this.useWeakEventHandler = value;
				if(_RealTimeCore != null)
					_RealTimeCore.UseWeakEventHandler = value;
			}
		}
		public RealTimeSource() {
			this.isSuspended = false;
			this.IgnoreItemEvents = false;
		}
		bool IListSource.ContainsListCollection { get { return false; } }
		public IList GetList() {
			if(source == null) {
				if(_IsDisposed)
					throw new ObjectDisposedException(this.ToString());
#if !SL
				if(this.DesignMode)
					source = DTWrapper;
				else
#endif
					source = RealTimeCore;
			}
			return source as IList;
		}
#if !SL
		RealTimeSourceDesignTimeWrapper CreateDesignTimeWrapper() {
			RealTimeSourceDesignTimeWrapper wrapper = new RealTimeSourceDesignTimeWrapper(DataSource, DisplayableProperties);
			return wrapper;
		}
#endif
		RealTimeSourceCore CreateRuntimeCore() {
			RealTimeSourceCore core = new RealTimeSourceCore(DataSource, null, this._DisplayableProperties, IgnoreItemEvents, UseWeakEventHandler);
			return core;
		}
		string GetDefaultDisplayableProperties(object dataSource) {
			if(dataSource == null)
				return null;
			IEnumerable<string> properies = RealTimeSource.GetDisplayableProperties(dataSource);
			if(ReferenceEquals(properies, null))
				return string.Empty;
			else
				return string.Join(";", properies.ToArray());
		}
		static public IEnumerable<string> GetDisplayableProperties(object source) {
			return GetDisplayableProperties(source, 0);
		}
		static IEnumerable<string> GetDisplayableProperties(object source, int depthOfReferences) {
			PropertyDescriptorCollection pdc = (source is ITypedList) ? ((ITypedList)source).GetItemProperties(null) : ListBindingHelper.GetListItemProperties(source);
			if(pdc == null)
				return null;
			else
				return GetDefProps(depthOfReferences, pdc);
		}
		static IEnumerable<string> GetDefProps(int depthLeft, PropertyDescriptorCollection pdc) {
			if(depthLeft < 0)
				yield break;
			foreach(PropertyDescriptor pd in pdc) {
				if(!pd.IsBrowsable)
					continue;
				yield return pd.Name;
				string prefix = pd.Name + ".";
				foreach(string prop in GetDefProps(depthLeft - 1, pd.GetChildProperties()))
					yield return prefix + prop;
			}
		}
		void ForceCatchUp() {
#if !SL
			if(_DTWrapper != null) {
				_DTWrapper.DisplayableProperties = DisplayableProperties;
			}
#endif
			if(_RealTimeCore != null) {
				_RealTimeCore.DisplayableProperties = DisplayableProperties;
			}
		}
		object IDXCloneable.DXClone() {
			return DXClone();
		}
		protected virtual object DXClone() {
			RealTimeSource clone = DXCloneCreate();
			clone._DisplayableProperties = this._DisplayableProperties;
			clone.source = this.source;
			return clone;
		}
		protected virtual RealTimeSource DXCloneCreate() {
			return new RealTimeSource();
		}
		protected override void Dispose(bool disposing) {
			_IsDisposed = true;
			source = null;
			this.dataSource = null;
#if !SL
			_DTWrapper = null;
#endif
			if(_RealTimeCore != null) {
				_RealTimeCore.Dispose();
				_RealTimeCore = null;
			}
			base.Dispose(disposing);
		}
		public TimeSpan GetQueueDelay() {
			if(_RealTimeCore != null)
				return _RealTimeCore.GetQueueDelay();
			return TimeSpan.Zero;
		}
		public void Suspend() {
			if(this.isSuspended)
				return;
			if(_RealTimeCore == null)
				return;
			this.suspendDataSource = _RealTimeCore.DataSource;
			DataSource = null;
			this.isSuspended = true;
		}
		public void Resume() {
			if(!this.isSuspended)
				return;
			if(_RealTimeCore == null)
				return;
			this.isSuspended = false;
			DataSource = this.suspendDataSource;
		}
		public void CatchUp() {
			if(_RealTimeCore == null)
				return;
			if(this.isSuspended)
				return;
			_RealTimeCore.CatchUp();
		}
	}
#if !SL
	internal class RealTimeSourceDesignTimeWrapper : IBindingList, ITypedList {
		public event ListChangedEventHandler ListChanged;
		object dataSource;
		string _DisplayableProperties;
		PropertyDescriptorCollection _Descriptors;
		[RefreshProperties(RefreshProperties.All)]
		public string DisplayableProperties {
			get {
				return _DisplayableProperties;
			}
			set {
				if(DisplayableProperties == value)
					return;
				_DisplayableProperties = value;
				InvalidateDescriptors();
			}
		}
		public object DataSource {
			get { return this.dataSource; }
			set {
				if(this.dataSource == value)
					return;
				this.dataSource = value;
				InvalidateDescriptors();
			}
		}
		void InvalidateDescriptors() {
			if(_Descriptors != null) {
				_Descriptors = null;
				if(ListChanged != null)
					ListChanged(this, new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, -1));
			}
		}
		public RealTimeSourceDesignTimeWrapper(object source, string dispProps) {
			this.dataSource = source;
			this._DisplayableProperties = dispProps;
		}
		internal static RealTimePropertyDescriptor GetMessagingDescriptorIfUnsafe(string name, PropertyDescriptor prototype) {
			if(prototype == null)
				return new RealTimePropertyDescriptor(name, string.Format("'{0}' member does not exist", name));
			if(typeof(IEnumerable).IsAssignableFrom(prototype.PropertyType) && !DevExpress.Xpo.DB.DBColumn.IsStorableType(prototype.PropertyType))
				return new RealTimePropertyDescriptor(name, string.Format("'{0}' member is not safe", name));
			return null;
		}
		PropertyDescriptorCollection GetDescriptors() {
			return GetDescriptorsCore() ?? PropertyDescriptorCollection.Empty;
		}
		PropertyDescriptorCollection GetDescriptorsCore() {
			if(this.dataSource == null)
				return null;
			if(string.IsNullOrEmpty(DisplayableProperties))
				return null;
			if(!(this.dataSource is ITypedList))
				return null;
			PropertyDescriptorCollection props = ((ITypedList)this.dataSource).GetItemProperties(null);
			List<PropertyDescriptor> rv = new List<PropertyDescriptor>();
			foreach(string name in DisplayableProperties.Split(';')) {
				PropertyDescriptor prototype = props.Find(name, false);
				if(prototype == null)
					continue;
				PropertyDescriptor pd = GetMessagingDescriptorIfUnsafe(name, prototype);
				if(pd == null)
					pd = new RealTimePropertyDescriptor(name, prototype, -1, true);
				rv.Add(pd);
			}
			return new PropertyDescriptorCollection(rv.ToArray(), true);
		}
		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors) {
			if(_Descriptors == null) {
				_Descriptors = GetDescriptors();
			}
			return _Descriptors;
		}
		string ITypedList.GetListName(PropertyDescriptor[] listAccessors) {
			return string.Empty;
		}
		#region IBindingList Members
		void IBindingList.AddIndex(PropertyDescriptor property) {
		}
		object IBindingList.AddNew() {
			throw new NotSupportedException();
		}
		bool IBindingList.AllowEdit {
			get { return false; }
		}
		bool IBindingList.AllowNew {
			get { return false; }
		}
		bool IBindingList.AllowRemove {
			get { return false; }
		}
		void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction) {
		}
		int IBindingList.Find(PropertyDescriptor property, object key) {
			return -1;
		}
		bool IBindingList.IsSorted {
			get { return false; }
		}
		void IBindingList.RemoveIndex(PropertyDescriptor property) {
		}
		void IBindingList.RemoveSort() {
		}
		ListSortDirection IBindingList.SortDirection {
			get {
				throw new NotSupportedException();
			}
		}
		PropertyDescriptor IBindingList.SortProperty {
			get { throw new NotSupportedException(); }
		}
		bool IBindingList.SupportsChangeNotification {
			get { return true; }
		}
		bool IBindingList.SupportsSearching {
			get { return false; }
		}
		bool IBindingList.SupportsSorting {
			get { return false; }
		}
		#endregion
		#region IList Members
		int IList.Add(object value) {
			throw new NotSupportedException();
		}
		void IList.Clear() {
			throw new NotSupportedException();
		}
		bool IList.Contains(object value) {
			return false;
		}
		int IList.IndexOf(object value) {
			return -1;
		}
		void IList.Insert(int index, object value) {
			throw new NotSupportedException();
		}
		bool IList.IsFixedSize {
			get { return true; }
		}
		bool IList.IsReadOnly {
			get { return true; }
		}
		void IList.Remove(object value) {
			throw new NotSupportedException();
		}
		void IList.RemoveAt(int index) {
			throw new NotSupportedException();
		}
		object IList.this[int index] {
			get {
				return null;
			}
			set {
				throw new NotSupportedException();
			}
		}
		#endregion
		#region ICollection Members
		void ICollection.CopyTo(Array array, int index) {
		}
		int ICollection.Count {
			get { return 0; }
		}
		bool ICollection.IsSynchronized {
			get { return false; }
		}
		object ICollection.SyncRoot {
			get { return this; }
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return string.Empty.GetEnumerator();
		}
		#endregion
	}
#endif
	internal class RealTimeSourceCore : IRealTimeListChangeProcessor
#if !SL
, IBindingList, ITypedList, IDisposable
#endif
 {
		bool isDisposed = false;
		public event ListChangedEventHandler ListChanged;
		readonly SynchronizationContext synchronizationContext;
		RealTimeQueue _Worker;
		List<RealTimeProxyForObject> cache;
		string listName;
		RealTimePropertyDescriptorCollection propertyCollection;
		IBindingList dataSourceAdapter;
		string displayableProperties;
		DateTime? lastProcessedCommandCreationTime;
		readonly object syncObject = new object();
		object SyncObject { get { return this.syncObject; } }
		public string DisplayableProperties {
			get { lock(SyncObject) return this.displayableProperties; }
			set {
				lock(SyncObject) {
					this.displayableProperties = value;
					if(Worker != null)
						Worker.DisplayableProperties = value;
				}
			}
		}
		readonly ItemPropertyNotificationMode notificationMode;
		bool waitReset;
		bool isCatchUp;
		public object DataSource {
			get {
				lock(SyncObject)
					if(this.dataSourceAdapter is BindingListAdapterBase)
						return ((BindingListAdapterBase)this.dataSourceAdapter).OriginalDataSource;
					else
						return this.dataSourceAdapter;
			}
			set {
				lock(SyncObject) {
					if(this.dataSourceAdapter == value)
						return;
					InvalidateDataSource();
					if(value == null)
						return;
					if(!(value is IList))
						throw new ArgumentException("DataSource");
					lock(((IList)value).SyncRoot) {
						if(value is IBindingList)
							this.dataSourceAdapter = (IBindingList)value;
						else
							if(value is IList) {
								BindingListAdapterBase adapter = BindingListAdapterBase.CreateFromList(value as IList, this.notificationMode);
								adapter.OriginalDataSource = value;
								this.dataSourceAdapter = adapter;
							}
						DataSourceChanged();
					}
				}
			}
		}
		public bool UseWeakEventHandler { get; set; }
		RealTimeQueue Worker {
			get {
				if(isDisposed)
					throw new ObjectDisposedException(this.GetType().FullName);
				return _Worker;
			}
		}
		#region IRealTimeListChangeProcessor Member
		void IRealTimeListChangeProcessor.NotifyLastProcessedCommandCreationTime(DateTime sent) { lastProcessedCommandCreationTime = sent; }
		List<RealTimeProxyForObject> IRealTimeListChangeProcessor.Cache { get { return this.cache; } set { this.cache = value; } }
		ListChangedEventHandler IRealTimeListChangeProcessor.ListChanged { get { return ListChanged; } }
		RealTimePropertyDescriptorCollection IRealTimeListChangeProcessor.PropertyDescriptorsCollection { get { return this.propertyCollection; } set { this.propertyCollection = value; } }
		bool IRealTimeListChangeProcessor.IsCatchUp { get { return isCatchUp; } set { isCatchUp = value; } }
		#endregion IRealTimeListChangeProcessor Member
		public RealTimeSourceCore(object source, SynchronizationContext context, string displayableProperties, bool ignoreItemEvents, bool useWeakEventHandler) {
			this.synchronizationContext = context ?? SynchronizationContext.Current;
			this.displayableProperties = displayableProperties;
			this.notificationMode = !ignoreItemEvents ? ItemPropertyNotificationMode.PropertyChanged : ItemPropertyNotificationMode.None;
			this.UseWeakEventHandler = useWeakEventHandler;
			this.cache = new List<RealTimeProxyForObject>();
			this.propertyCollection = null;
			this.waitReset = true;
			DataSource = source;
		}
		void InvalidateDataSource() {
			if(isDisposed)
				return;
			if(_Worker != null) {
				_Worker.Dispose();
				_Worker = null;
			}
			this.waitReset = true;
			this.cache = null;
			this.propertyCollection = null;
			if(this.dataSourceAdapter is BindingListAdapterBase)
				((IDisposable)this.dataSourceAdapter).Dispose();
			this.dataSourceAdapter = null;
		}
		void DataSourceChanged() {
			this.propertyCollection = RealTimePropertyDescriptorCollection.CreatePropertyDescriptorCollection(this.dataSourceAdapter, this.displayableProperties);
			this.listName = (this.dataSourceAdapter is ITypedList) ? ((ITypedList)this.dataSourceAdapter).GetListName(null) : string.Empty;
			_Worker = new RealTimeQueue(synchronizationContext, this.dataSourceAdapter, this.propertyCollection, SourceListChanged, UseWeakEventHandler, DisplayableProperties);
		}
		#region IBindingList Members
		public void AddIndex(PropertyDescriptor property) { throw new NotImplementedException(); }
		public object AddNew() { throw new NotImplementedException(); }
		public bool AllowEdit { get { return false; } }
		public bool AllowNew { get { return false; } }
		public bool AllowRemove { get { return false; } }
		public void ApplySort(PropertyDescriptor property, ListSortDirection direction) { throw new NotImplementedException(); }
		public int Find(PropertyDescriptor property, object key) {
			lock(SyncObject) {
				if(property == null)
					return this.cache.IndexOf(key as RealTimeProxyForObject);
				else
					return this.cache.IndexOf(this.cache.FirstOrDefault(i => { object value; return i.Content.TryGetValue((RealTimePropertyDescriptor)property, out value) ? (value == key) : false; }
					));
			}
		}
		public bool IsSorted { get { return false; } }
		public void RemoveIndex(PropertyDescriptor property) { throw new NotImplementedException(); }
		public void RemoveSort() { throw new NotImplementedException(); }
		public ListSortDirection SortDirection { get { return ListSortDirection.Ascending; } }
		public PropertyDescriptor SortProperty { get { return null; } }
		public bool SupportsChangeNotification { get { return true; } }
		public bool SupportsSearching { get { return true; } }
		public bool SupportsSorting { get { return false; } }
		public int Add(object value) { throw new NotImplementedException(); }
		public void Clear() { throw new NotImplementedException(); }
		public bool Contains(object value) {
			lock(SyncObject)
				return this.cache.Contains(value as RealTimeProxyForObject);
		}
		public int IndexOf(object value) {
			lock(SyncObject)
				return this.cache.IndexOf(value as RealTimeProxyForObject);
		}
		public void Insert(int index, object value) { throw new NotImplementedException(); }
		public bool IsFixedSize { get { return false; } }
		public bool IsReadOnly { get { return true; } }
		public void Remove(object value) { throw new NotImplementedException(); }
		public void RemoveAt(int index) { throw new NotImplementedException(); }
		public object this[int index] {
			get { lock(SyncObject) return this.cache[index]; }
			set { lock(SyncObject) this.cache[index] = value as RealTimeProxyForObject; }
		}
		public void CopyTo(Array array, int index) {
			lock(SyncObject)
				this.cache.ToArray().CopyTo(array, index);
		}
		public int Count {
			get {
				lock(SyncObject) {
					if(this.cache == null)
						return 0;
					else
						return this.cache.Count;
				}
			}
		}
		public bool IsSynchronized { get { return true; } }
		public object SyncRoot { get { return this.SyncObject; } }
		public IEnumerator GetEnumerator() {
			if(isDisposed)
				throw new ObjectDisposedException(this.ToString());
			lock(SyncObject) {
				if(this.cache == null)
					return Enumerable.Empty<RealTimeProxyForObject>().GetEnumerator();
				else
					return this.cache.GetEnumerator();
			}
		}
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
			lock(SyncObject) {
				PropertyDescriptorCollection pdc = null;
				if(listAccessors == null)
					pdc = propertyCollection;
				else
					pdc = ListBindingHelper.GetListItemProperties(listAccessors[0].PropertyType);
				return pdc;
			}
		}
		public string GetListName(PropertyDescriptor[] listAccessors) {
			return listName;
		}
		#endregion IBindingList Members
		void SourceListChanged(object sender, RealTimeEventBase command) {
			if(_Worker == null || (this.waitReset && !(command is RealTimeResetEvent) && !(command is RealTimePropertyDescriptorsChangedEvent)))
				return;
			lock(this.SyncObject) {
				this.waitReset = false;
				if(GetQueueDelay().TotalMilliseconds > RealTimeSource.SendQueueTimeout) {
					isCatchUp = true;
				}
				command.PostProcess(this);
				if(Worker.IsQueueEmpty) {
					isCatchUp = false;
					if(ListChanged != null)
						ListChanged(this, new ListChangedEventArgs(ListChangedType.Reset, -1));
				}
			}
		}
		internal TimeSpan GetQueueDelay() {
			lock(SyncObject) {
				if(Worker == null)
					return TimeSpan.Zero;
				if(Worker.IsQueueEmpty) {
					this.lastProcessedCommandCreationTime = null;
				}
				if(lastProcessedCommandCreationTime.HasValue)
					return DateTime.Now - this.lastProcessedCommandCreationTime.Value;
				else
					return TimeSpan.Zero;
			}
		}
		internal void CatchUp() {
			isCatchUp = true;
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				InvalidateDataSource();
				this.isDisposed = true;
			}
		}
		~RealTimeSourceCore() { Dispose(false); }
	}
	public class RealTimePropertyDescriptor : PropertyDescriptor {
		readonly int index;
		readonly Type propertyType;
		readonly string message;
		readonly bool visible;
		readonly Func<object, object> getFunc;
		public bool Visible { get { return this.visible; } }
		public int Index { get { return this.index; } }
		public override bool CanResetValue(object component) { return false; }
		public override Type ComponentType { get { return typeof(RealTimeProxyForObject); } }
		public override bool IsReadOnly { get { return true; } }
		public override Type PropertyType { get { return this.propertyType; } }
		public override void ResetValue(object component) { throw new NotSupportedException(); }
		public override bool ShouldSerializeValue(object component) { return false; }
		public RealTimePropertyDescriptor(string name, PropertyDescriptor prototype, int index, bool visible)
			: base(name, new Attribute[0]) {
			this.propertyType = prototype.PropertyType;
			this.index = index;
			this.visible = visible;
			ParameterExpression pe = Expression.Parameter(typeof(object), "x");
			var getExpr = DevExpress.Data.Filtering.Helpers.CriteriaCompiledContextDescriptorDescripted.MakeAccessFromDescriptor(pe, prototype);
			if(getExpr.Type != typeof(object))
				getExpr = Expression.Convert(getExpr, typeof(object));
			this.getFunc = Expression.Lambda<Func<object, object>>(getExpr, pe).Compile();
		}
		public RealTimePropertyDescriptor(string name, string message)
			: base(name, new Attribute[0]) {
			this.propertyType = typeof(string);
			this.index = -1;
			this.message = message;
			this.visible = true;
		}
		public override object GetValue(object component) {
			if(index < 0)
				return this.message;
			RealTimeProxyForObject obj = component as RealTimeProxyForObject;
			if(obj == null)
				return null;
			object value = null;
			obj.Content.TryGetValue(this, out value);
			return value;
		}
		public override void SetValue(object component, object value) {
			RealTimeProxyForObject obj = component as RealTimeProxyForObject;
			obj.Content[this] = value;
		}
		public object GetSourceValue(object component) {
			return getFunc(component);
		}
	}
	public class RealTimePropertyDescriptorCollection : PropertyDescriptorCollection {
		readonly static object syncObject = new object();
		public static RealTimePropertyDescriptorCollection CreatePropertyDescriptorCollection(IList list, string displayableProperties) {
			return new RealTimePropertyDescriptorCollection(list, displayableProperties);
		}
		public static RealTimePropertyDescriptorCollection CreatePropertyDescriptorCollection(PropertyDescriptorCollection properties) {
			return new RealTimePropertyDescriptorCollection(properties, string.Empty);
		}
		static PropertyDescriptorCollection GetSourcePropertyDescriptorCollection(IList source) {
			var originalPDc = source is ITypedList ? ((ITypedList)source).GetItemProperties(null) : ListBindingHelper.GetListItemProperties(source);
			return DevExpress.Data.Access.DataListDescriptor.GetFastProperties(originalPDc);
		}
		readonly Dictionary<PropertyDescriptor, RealTimePropertyDescriptor> descriptorDict;
		readonly PropertyDescriptorCollection sourcePDC;
		public PropertyDescriptorCollection SourcePropertyDescriptorCollection { get { return this.sourcePDC; } }
		RealTimePropertyDescriptorCollection(IList list, string displayableProperties)
			: this(GetSourcePropertyDescriptorCollection(list), displayableProperties) { }
		RealTimePropertyDescriptorCollection(PropertyDescriptorCollection properties, string displayableProperties)
			: base(null) {
			lock(syncObject) {
				this.sourcePDC = properties;
				this.descriptorDict = new Dictionary<PropertyDescriptor, RealTimePropertyDescriptor>();
				if(string.IsNullOrEmpty(displayableProperties) && properties == null)
					return;
				string[] displayProperties;
				if(string.IsNullOrEmpty(displayableProperties))
					displayProperties = new string[0];
				else
					displayProperties = displayableProperties.Split(';');
				for(int i = 0; i < properties.Count; i++) {
					AddProperty(properties[i], i, (displayProperties.Count() == 0 || displayProperties.Contains(properties[i].Name)));
				}
			}
		}
		RealTimePropertyDescriptor AddProperty(PropertyDescriptor property, int index, bool visible) {
			if(!visible) return null;
			if(index < 0)
				index = this.descriptorDict.Count;
			RealTimePropertyDescriptor rpd = new RealTimePropertyDescriptor(property.Name, property, index, visible);
			this.descriptorDict.Add(property, rpd);
			Add(rpd);
			return rpd;
		}
		public RealTimePropertyDescriptor this[PropertyDescriptor pd] {
			get {
				RealTimePropertyDescriptor rtpd;
				lock(syncObject) {
					if(this.descriptorDict.TryGetValue(pd, out rtpd))
						return rtpd;
				}
				return null;
			}
		}
		public PropertyDescriptor GetPropertyDescriptor(RealTimePropertyDescriptor rpd) {
			lock(syncObject) {
				return this.descriptorDict.FirstOrDefault(i => i.Value == rpd).Key;
			}
		}
		public PropertyDescriptor GetPropertyDescriptorByName(string name) {
			lock(syncObject) {
				return GetPropertyDescriptor(((RealTimePropertyDescriptor)this[name]));
			}
		}
		public override IEnumerator GetEnumerator() {
			lock(syncObject)
				return base.GetEnumerator();
		}
		public override PropertyDescriptor this[int index] {
			get {
				lock(syncObject)
					return base[index];
			}
		}
		public override PropertyDescriptor this[string name] {
			get {
				lock(syncObject)
					return base[name];
			}
		}
		public override PropertyDescriptor Find(string name, bool ignoreCase) {
			lock(syncObject)
				return base.Find(name, ignoreCase);
		}
		public bool TryGetRealtimePropertyDescriptor(PropertyDescriptor pd, out RealTimePropertyDescriptor rpd) {
			lock(syncObject)
				return descriptorDict.TryGetValue(pd, out rpd);
		}
		public Dictionary<RealTimePropertyDescriptor, object> GetSourceValue(object component) {
			Dictionary<RealTimePropertyDescriptor, object> content = new Dictionary<RealTimePropertyDescriptor, object>(this.Count);
			foreach(KeyValuePair<PropertyDescriptor, RealTimePropertyDescriptor> item in descriptorDict) {
				content[item.Value] = item.Value.GetSourceValue(component);
			}
			return content;
		}
	}
	public class RealTimeProxyForObject {
		public RealTimePropertyDescriptor GetChangedPropertyDescriptor() {
			if(Content.Count != 1)
				return null;
			return Content.Keys.First();
		}
		public Dictionary<RealTimePropertyDescriptor, object> Content { get; private set; }
		public RealTimeProxyForObject(object source, RealTimePropertyDescriptorCollection pdc)
			: this(source, pdc, null) {
		}
		public RealTimeProxyForObject(object source, RealTimePropertyDescriptorCollection pdc, PropertyDescriptor pdSource) {
			this.Content = FillValue(source, pdc, pdSource);
		}
		static Dictionary<RealTimePropertyDescriptor, object> FillValue(object source, RealTimePropertyDescriptorCollection pdc, PropertyDescriptor pdSource) {
			if(pdSource != null) {
				var rv = new Dictionary<RealTimePropertyDescriptor, object>(pdc.Count);
				RealTimePropertyDescriptor rpd;
				if(!pdc.TryGetRealtimePropertyDescriptor(pdSource, out rpd))
					throw new InvalidOperationException("RealTimePropertyDescriptorCollection");
				rv[rpd] = rpd.GetSourceValue(source);
				return rv;
			} else {
				return pdc.GetSourceValue(source);
			}
		}
		public void Assign(RealTimeProxyForObject source) {
			if(source == null)
				return;
			foreach(var propertyValue in source.Content) {
				if(propertyValue.Key == null)
					continue;
				Content[propertyValue.Key] = propertyValue.Value;
			}
		}
		public override bool Equals(object obj) {
			RealTimeProxyForObject other = obj as RealTimeProxyForObject;
			if(other == null)
				return false;
			object value;
			foreach(var item in Content) {
				if(!other.Content.TryGetValue(item.Key, out value))
					return false;
				if(!object.Equals(item.Value, value))
					return false;
			}
			return true;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	public static class RTListChangedEventArgs {
		[ThreadStatic]
		static ListChangedEventArgs eventArg;
		readonly static Action<ListChangedEventArgs, int> oldIndexSet;
		readonly static Action<ListChangedEventArgs, int> newIndexSet;
		readonly static Action<ListChangedEventArgs, PropertyDescriptor> propertyDescriptorSet;
		readonly static Action<ListChangedEventArgs, ListChangedType> listChangedTypeSet;
		readonly static MethodInfo assignInfo = typeof(RTListChangedEventArgs).GetMethod("Assign", BindingFlags.NonPublic | BindingFlags.Static);
		static RTListChangedEventArgs() {
			try {
				oldIndexSet = GetSetter<int>("oldIndex");
				newIndexSet = GetSetter<int>("newIndex");
				propertyDescriptorSet = GetSetter<PropertyDescriptor>("propDesc");
				listChangedTypeSet = GetSetter<ListChangedType>("listChangedType");
			} catch { }
		}
		static Action<ListChangedEventArgs, T> GetSetter<T>(string fieldName) {
			FieldInfo fi = typeof(ListChangedEventArgs).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
			ParameterExpression p = Expression.Parameter(typeof(ListChangedEventArgs), "param1");
			ParameterExpression p2 = Expression.Parameter(typeof(T), "param2");
			MemberExpression b = Expression.MakeMemberAccess(p, fi);
			return (Action<ListChangedEventArgs, T>)Expression.Lambda(Expression.Call(null, assignInfo.MakeGenericMethod(b.Type), b, p2), p, p2).Compile();
		}
		public static ListChangedEventArgs Create(ListChangedType listChangedType, PropertyDescriptor propDesc) {
			return Create(listChangedType, -1, -1, null);
		}
		public static ListChangedEventArgs Create(ListChangedType listChangedType, int newIndex) {
			return Create(listChangedType, newIndex, -1, null);
		}
		public static ListChangedEventArgs Create(ListChangedType listChangedType, int newIndex, PropertyDescriptor propDesc) {
			return Create(listChangedType, newIndex, -1, propDesc);
		}
		public static ListChangedEventArgs Create(ListChangedType listChangedType, int newIndex, int oldIndex) {
			return Create(listChangedType, newIndex, oldIndex, null);
		}
		public static ListChangedEventArgs Create(ListChangedType listChangedType, int newIndex, int oldIndex, PropertyDescriptor propDesc) {
			if(propDesc != null && oldIndex >= 0)
				throw new InvalidOperationException("RealTimeListChangedEventArgs");
			if(oldIndexSet == null || newIndexSet == null || propertyDescriptorSet == null || listChangedTypeSet == null) {
				eventArg = GetListChangedEventArgsInternal(listChangedType, newIndex, oldIndex, propDesc);
			} else {
				if(eventArg == null) {
					eventArg = GetListChangedEventArgsInternal(listChangedType, newIndex, oldIndex, propDesc);
				}
				oldIndexSet(eventArg, oldIndex);
				newIndexSet(eventArg, newIndex);
				propertyDescriptorSet(eventArg, propDesc);
				listChangedTypeSet(eventArg, listChangedType);
			}
			return eventArg;
		}
		static ListChangedEventArgs GetListChangedEventArgsInternal(ListChangedType listChangedType, int newIndex, int oldIndex, PropertyDescriptor propDesc) {
			if(propDesc != null)
				return new ListChangedEventArgs(listChangedType, newIndex, propDesc);
			else
				return new ListChangedEventArgs(listChangedType, newIndex, oldIndex);
		}
		static void Assign<T>(ref T left, T right) {
			left = right;
		}
	}
}
