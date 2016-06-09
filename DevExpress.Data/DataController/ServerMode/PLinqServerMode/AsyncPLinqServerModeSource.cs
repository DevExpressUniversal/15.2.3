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
using System.Text;
using System.ComponentModel;
using DevExpress.Data.Helpers;
using DevExpress.Data.Async;
using DevExpress.Data.Async.Helpers;
using System.Threading;
using DevExpress.Compatibility.System.ComponentModel;
#if SL
using System.Collections;
using DevExpress.Data.Browsing;
using DevExpress.Data.PLinq.Helpers;
using DevExpress.Xpf.ComponentModel;
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
#else
using System.Windows.Forms;
using System.Collections;
using DevExpress.Data.PLinq;
using DevExpress.Data.PLinq.Helpers;
using DevExpress.Data.Linq;
#endif
namespace DevExpress.Data.PLinq {
	public class GetEnumerableEventArgs : EventArgs {
		public IEnumerable Source { get; set; }
		public object Tag { get; set; }
	}
#if !SL
	[System.Drawing.ToolboxBitmap(typeof(ResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "PLinqInstantFeedbackSource.bmp")]
#endif
	[DXToolboxItem(true)]
	[DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabData)]
	[DefaultEvent("GetEnumerable")]
	public class PLinqInstantFeedbackSource : Component, IListSource, IDXCloneable {
		public PLinqInstantFeedbackSource() { }
		public PLinqInstantFeedbackSource(EventHandler<GetEnumerableEventArgs> getEnumerable) {
			this.GetEnumerable += getEnumerable;
		}
		public PLinqInstantFeedbackSource(EventHandler<GetEnumerableEventArgs> getEnumerable, EventHandler<GetEnumerableEventArgs> freeEnumerable)
			: this(getEnumerable) {
			this.DismissEnumerable += freeEnumerable;
		}
		static EventHandler<T> ToEventHandler<T>(Action<T> action) where T: EventArgs {
			if(action == null)
				return null;
			else
				return delegate(object sender, T e) { action(e); };
		}
		public PLinqInstantFeedbackSource(Action<GetEnumerableEventArgs> getEnumerable)
			: this(ToEventHandler(getEnumerable)) {
		}
		public PLinqInstantFeedbackSource(Action<GetEnumerableEventArgs> getEnumerable, Action<GetEnumerableEventArgs> freeEnumerable)
			: this(ToEventHandler(getEnumerable)
			, ToEventHandler(freeEnumerable)) {
		}
		Type _ElementType;
		[RefreshProperties(RefreshProperties.All), DefaultValue(null)]
#if !SL //TODO SL
		[TypeConverter(typeof(PLinqServerModeSourceObjectTypeConverter))]
#endif
		public Type DesignTimeElementType {
			get { return _ElementType; }
			set {
				if(_ElementType == value)
					return;
				TestCanChangeProperties();
				_ElementType = value;
				ForceCatchUp();
			}
		}
		string _DefaultSorting = string.Empty;
		[DefaultValue("")]
		public string DefaultSorting {
			get { return _DefaultSorting; }
			set {
				if(DefaultSorting == value)
					return;
				TestCanChangeProperties();
				_DefaultSorting = value;
				ForceCatchUp();
			}
		}
		int? _DegreeOfParallelism;
		[DefaultValue(null)]
		public int? DegreeOfParallelism {
			get { return _DegreeOfParallelism; }
			set {
				if(DegreeOfParallelism == value)
					return;
				TestCanChangeProperties();
				_DegreeOfParallelism = value;
				ForceCatchUp();
			}
		}
		void TestCanChangeProperties() {
			if(_AsyncListServer != null)
				throw new InvalidOperationException("Already in use!");
		}
		void ForceCatchUp() {
			if(_DTWrapper != null)
				_DTWrapper.ElementType = DesignTimeElementType;
		}
		public event EventHandler<GetEnumerableEventArgs> GetEnumerable;
		public event EventHandler<GetEnumerableEventArgs> DismissEnumerable;
		AsyncListServer2DatacontrollerProxy _AsyncListServer;
		AsyncListDesignTimeWrapper _DTWrapper;
		System.Collections.IList _List;
#if !SL
		bool IListSource.ContainsListCollection {
			get { return false; }
		}
#endif
		bool? _isDesignMode;
		System.Collections.IList IListSource.GetList() {
			bool designMode = IsDesignModeHelper.GetIsDesignModeBypassable(this, ref _isDesignMode);
			if(_List == null) {
				if(IsDisposed)
					throw new ObjectDisposedException(this.ToString());
				if(designMode) {
					_List = _DTWrapper = CreateDesignTimeWrapper();
				} else {
					_List = _AsyncListServer = CreateRunTimeProxy();
				}
			}
			return _List;
		}
		AsyncListDesignTimeWrapper CreateDesignTimeWrapper() {
			var wrapper = new AsyncListDesignTimeWrapper();
			wrapper.ElementType = this.DesignTimeElementType;
			return wrapper;
		}
		AsyncListServer2DatacontrollerProxy CreateRunTimeProxy() {
			AsyncListServerCore core = new AsyncListServerCore(SynchronizationContext.Current);
			core.ListServerGet += listServerGet;
			core.ListServerFree += listServerFree;
			core.GetTypeInfo += getTypeInfo;
			core.GetPropertyDescriptors += getPropertyDescriptors;
			core.GetWorkerThreadRowInfo += getWorkerRowInfo;
			core.GetUIThreadRow += getUIRow;
			AsyncListServer2DatacontrollerProxy rv = new AsyncListServer2DatacontrollerProxy(core);
			return rv;
		}
		void listServerGet(object sender, ListServerGetOrFreeEventArgs e) {
			GetEnumerableEventArgs args = new GetEnumerableEventArgs();
			e.Tag = args;
			if(this.GetEnumerable != null)
				this.GetEnumerable(this, args);
			PLinqServerModeSource src = new PLinqServerModeSource();
			e.ListServerSource = src;
			if(args.Source == null) {
				src.Source = new GetEnumerableNotHandledMessenger[] { new GetEnumerableNotHandledMessenger() };
			} else {
				src.Source = args.Source;
				src.DefaultSorting = this.DefaultSorting;
				src.DegreeOfParallelism = this.DegreeOfParallelism;
			}
		}
		void listServerFree(object sender, ListServerGetOrFreeEventArgs e) {
			GetEnumerableEventArgs args = ((GetEnumerableEventArgs)e.Tag);
			if(DismissEnumerable != null)
				DismissEnumerable(this, args);
		}
		void getTypeInfo(object sender, GetTypeInfoEventArgs e) {
			GetEnumerableEventArgs getEnumerableArgs = (GetEnumerableEventArgs)e.Tag;
			PropertyDescriptorCollection sourceDescriptors = ListBindingHelper.GetListItemProperties(e.ListServerSource);
			if(getEnumerableArgs.Source == null) {
				e.TypeInfo = new TypeInfoNoQueryable(this.DesignTimeElementType);
			} else {
				e.TypeInfo = new TypeInfoThreadSafe(sourceDescriptors);
			}
		}
		void getPropertyDescriptors(object sender, GetPropertyDescriptorsEventArgs e) {
			e.PropertyDescriptors = ((TypeInfoBase)e.TypeInfo).UIDescriptors;
		}
		void getWorkerRowInfo(object sender, GetWorkerThreadRowInfoEventArgs e) {
			e.RowInfo = ((TypeInfoBase)e.TypeInfo).GetWorkerThreadRowInfo(e.WorkerThreadRow);
		}
		void getUIRow(object sender, GetUIThreadRowEventArgs e) {
			e.UIThreadRow = ((TypeInfoBase)e.TypeInfo).GetUIThreadRow(e.RowInfo);
		}
		object IDXCloneable.DXClone() {
			return DXClone();
		}
		protected virtual object DXClone() {
			PLinqInstantFeedbackSource clone = DXCloneCreate();
			clone._DegreeOfParallelism = this._DegreeOfParallelism;
			clone._DefaultSorting = this._DefaultSorting;
			clone._ElementType = this._ElementType;
			clone.IsDisposed = this.IsDisposed;
			clone.GetEnumerable = this.GetEnumerable;
			clone.DismissEnumerable = this.DismissEnumerable;
			return clone;
		}
		protected virtual PLinqInstantFeedbackSource DXCloneCreate() {
			return new PLinqInstantFeedbackSource();
		}
		bool IsDisposed;
		protected override void Dispose(bool disposing) {
			IsDisposed = true;
			_List = null;
			_DTWrapper = null;
			if(_AsyncListServer != null) {
				_AsyncListServer.Dispose();
				_AsyncListServer = null;
			}
			base.Dispose(disposing);
		}
		public void Refresh() {
			if(_AsyncListServer == null)
				return;
			_AsyncListServer.Refresh();
		}
	}
}
namespace DevExpress.Data.PLinq.Helpers {
	public class AsyncListDesignTimeWrapper : IBindingList, ITypedList {
		Type _ElementType;
		PropertyDescriptorCollection _Descriptors;
		public Type ElementType {
			get { return _ElementType; }
			set {
				if(ElementType == value)
					return;
				_ElementType = value;
				if(_Descriptors != null) {
					_Descriptors = null;
					ListChanged(this, new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, -1));
				}
			}
		}
		PropertyDescriptorCollection GetDescriptors() {
			return TypeDescriptor.GetProperties(ElementType);
		}
		public event ListChangedEventHandler ListChanged;
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
		int System.Collections.IList.Add(object value) {
			throw new NotSupportedException();
		}
		void System.Collections.IList.Clear() {
			throw new NotSupportedException();
		}
		bool System.Collections.IList.Contains(object value) {
			return false;
		}
		int System.Collections.IList.IndexOf(object value) {
			return -1;
		}
		void System.Collections.IList.Insert(int index, object value) {
			throw new NotSupportedException();
		}
		bool System.Collections.IList.IsFixedSize {
			get { return true; }
		}
		bool System.Collections.IList.IsReadOnly {
			get { return true; }
		}
		void System.Collections.IList.Remove(object value) {
			throw new NotSupportedException();
		}
		void System.Collections.IList.RemoveAt(int index) {
			throw new NotSupportedException();
		}
		object System.Collections.IList.this[int index] {
			get {
				return null;
			}
			set {
				throw new NotSupportedException();
			}
		}
		#endregion
		#region ICollection Members
		void System.Collections.ICollection.CopyTo(Array array, int index) {
		}
		int System.Collections.ICollection.Count {
			get { return 0; }
		}
		bool System.Collections.ICollection.IsSynchronized {
			get { return false; }
		}
		object System.Collections.ICollection.SyncRoot {
			get { return this; }
		}
		#endregion
		#region IEnumerable Members
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return new object[0].GetEnumerator();
		}
		#endregion
	}
	abstract class TypeInfoBase {
		public abstract PropertyDescriptorCollection UIDescriptors { get; }
		public abstract object GetWorkerThreadRowInfo(object workerRow);
		public abstract object GetUIThreadRow(object rowInfo);
	}
	class TypeInfoThreadSafe : TypeInfoBase {
		readonly PropertyDescriptorCollection PropertyDescriptors;
		public TypeInfoThreadSafe(PropertyDescriptorCollection propertyDescriptors) {
			this.PropertyDescriptors = propertyDescriptors;
		}
		public override PropertyDescriptorCollection UIDescriptors { get { return PropertyDescriptors; } }
		public override object GetWorkerThreadRowInfo(object workerRow) { return workerRow; }
		public override object GetUIThreadRow(object rowInfo) { return rowInfo; }
	}
	class TypeInfoProxied : TypeInfoBase {
		readonly PropertyDescriptorCollection uiDescriptors;
		readonly PropertyDescriptor[] workerDescriptors;
		public TypeInfoProxied(PropertyDescriptorCollection workerThreadDescriptors, Type designTimeType) {
			PropertyDescriptorCollection prototypes = designTimeType == null ? workerThreadDescriptors : TypeDescriptor.GetProperties(designTimeType);
			List<PropertyDescriptor> ui = new List<PropertyDescriptor>(prototypes.Count);
			List<PropertyDescriptor> wr = new List<PropertyDescriptor>(prototypes.Count);
			foreach(PropertyDescriptor proto in prototypes) {
				ui.Add(new ProxyPropertyDescriptor(proto.Name, proto.PropertyType, ui.Count));
				wr.Add(workerThreadDescriptors.Find(proto.Name, false));
			}
			uiDescriptors = new PropertyDescriptorCollection(ui.ToArray(), true);
			workerDescriptors = wr.ToArray();
		}
		public override PropertyDescriptorCollection UIDescriptors {
			get { return uiDescriptors; }
		}
		public override object GetWorkerThreadRowInfo(object workerRow) {
			object[] rv = new object[workerDescriptors.Length];
			if (workerRow == null) return rv;
			for(int i = 0; i < workerDescriptors.Length; ++i) {
				PropertyDescriptor pd = workerDescriptors[i];
				if(pd != null)
					rv[i] = pd.GetValue(workerRow);
			}
			return rv;
		}
		public override object GetUIThreadRow(object rowInfo) {
			return rowInfo;
		}
		class ProxyPropertyDescriptor : PropertyDescriptor {
			readonly Type Type;
			readonly int Index;
			public ProxyPropertyDescriptor(string name, Type type, int index)
				: base(name, new Attribute[0]) {
				this.Type = type;
				this.Index = index;
			}
			public override bool CanResetValue(object component) {
				return false;
			}
			public override Type ComponentType {
				get { return typeof(object[]); }
			}
			public override object GetValue(object component) {
				if(component == null)
					return null;
				else
					return ((object[])component)[Index];
			}
			public override bool IsReadOnly {
				get { return true; }
			}
			public override Type PropertyType {
				get { return Type; }
			}
			public override void ResetValue(object component) {
			}
			public override void SetValue(object component, object value) {
				throw new NotSupportedException();
			}
			public override bool ShouldSerializeValue(object component) {
				return false;
			}
		}
	}
	class TypeInfoNoQueryable : TypeInfoBase {
		readonly PropertyDescriptorCollection uiDescriptors;
		public TypeInfoNoQueryable(Type designTimeType) {
			Type type = designTimeType ?? typeof(GetEnumerableNotHandledMessenger);
			PropertyDescriptorCollection prototypes = TypeDescriptor.GetProperties(type);
			List<PropertyDescriptor> ui = new List<PropertyDescriptor>(prototypes.Count);
			foreach(PropertyDescriptor proto in prototypes) {
				ui.Add(new NoEnumerablePropertyDescriptor(proto.Name, proto.PropertyType));
			}
			uiDescriptors = new PropertyDescriptorCollection(ui.ToArray(), true);
		}
		public override PropertyDescriptorCollection UIDescriptors {
			get { return uiDescriptors; }
		}
		public override object GetWorkerThreadRowInfo(object workerRow) {
			return workerRow;
		}
		public override object GetUIThreadRow(object rowInfo) {
			return rowInfo;
		}
		class NoEnumerablePropertyDescriptor : PropertyDescriptor {
			readonly Type Type;
			public NoEnumerablePropertyDescriptor(string name, Type type)
				: base(name, new Attribute[0]) {
				this.Type = type;
			}
			public override bool CanResetValue(object component) {
				return false;
			}
			public override Type ComponentType {
				get { return typeof(GetEnumerableNotHandledMessenger); }
			}
			public override object GetValue(object component) {
				if(this.PropertyType == typeof(string))
					return GetEnumerableNotHandledMessenger.MessageText;
				else
					return null;
			}
			public override bool IsReadOnly {
				get { return true; }
			}
			public override Type PropertyType {
				get { return Type; }
			}
			public override void ResetValue(object component) {
			}
			public override void SetValue(object component, object value) {
				throw new NotSupportedException();
			}
			public override bool ShouldSerializeValue(object component) {
				return false;
			}
		}
	}
	class GetEnumerableNotHandledMessenger {
		public static string MessageText = "Please handle the " + typeof(PLinqInstantFeedbackSource).Name + ".GetEnumerable event and provide a valid Source and Key";
		public string Message { get { return MessageText; } }
	}
}
