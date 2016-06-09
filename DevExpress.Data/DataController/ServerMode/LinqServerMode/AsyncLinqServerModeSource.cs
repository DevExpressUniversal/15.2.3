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
using DevExpress.Data.Linq.Helpers;
using DevExpress.Data.Helpers;
using DevExpress.Data.Async;
using DevExpress.Data.Async.Helpers;
using System.Threading;
#if SL
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using DevExpress.Data.Browsing;
using DevExpress.Xpf.ComponentModel;
#else
using System.Windows.Forms;
using System.Drawing;
using DevExpress.Utils;
#endif
namespace DevExpress.Data.Linq {
	public class GetQueryableEventArgs: EventArgs {
		public IQueryable QueryableSource { get; set; }
		public string KeyExpression { get; set; }
		public bool AreSourceRowsThreadSafe { get; set; }
		public object Tag { get; set; }
		public GetQueryableEventArgs() {
			KeyExpression = string.Empty;
		}
	}
	[DXToolboxItem(true)]
	[DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabData)]
	[DefaultEvent("GetQueryable")]
#if !SL
	[ToolboxBitmap(typeof(ResFinder), ControlConstants.BitmapPath + "LinqInstantFeedbackSource.bmp")]
#endif
	public class LinqInstantFeedbackSource: Component, IListSource, IDXCloneable {
		public LinqInstantFeedbackSource() { }
		public LinqInstantFeedbackSource(EventHandler<GetQueryableEventArgs> getQueryable) {
			this.GetQueryable += getQueryable;
		}
		public LinqInstantFeedbackSource(EventHandler<GetQueryableEventArgs> getQueryable, EventHandler<GetQueryableEventArgs> freeQueryable)
			: this(getQueryable) {
			this.DismissQueryable += freeQueryable;
		}
		static EventHandler<T> ToEventHandler<T>(Action<T> action) where T: EventArgs {
			if(action == null)
				return null;
			else
				return delegate(object sender, T e) { action(e); };
		}
		public LinqInstantFeedbackSource(Action<GetQueryableEventArgs> getQueryable)
			: this(ToEventHandler(getQueryable)) {
		}
		public LinqInstantFeedbackSource(Action<GetQueryableEventArgs> getQueryable, Action<GetQueryableEventArgs> freeQueryable)
			: this(ToEventHandler(getQueryable)
			, ToEventHandler(freeQueryable)) {
		}
		Type _ElementType;
		[RefreshProperties(RefreshProperties.All), DefaultValue(null)]
#if !SL //TODO SL
		[TypeConverter(typeof(LinqServerModeSourceObjectTypeConverter))]
#endif
		public Type DesignTimeElementType {
			get { return _ElementType; }
			set {
				if(_ElementType == value)
					return;
				TestCanChangeProperties();
				_ElementType = value;
				FillKeyExpression();
				ForceCatchUp();
			}
		}
		void FillKeyExpression() {
			if(DesignTimeElementType == null)
				return;
			if(KeyExpression != null && KeyExpression.IndexOfAny(new char[] { ',', ';' }) >= 0)
				return;
			try {
				if(DesignTimeElementType.GetProperty(KeyExpression) != null)
					return;
			} catch { }
			KeyExpression = LinqServerModeCore.GuessKeyExpression(DesignTimeElementType);
		}
		string _KeyExpression = string.Empty;
		[DefaultValue("")]
		public string KeyExpression {
			get { return _KeyExpression; }
			set {
				if(KeyExpression == value)
					return;
				TestCanChangeProperties();
				_KeyExpression = value;
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
		bool _AreSourceRowsThreadSafe;
		[DefaultValue(false)]
		public bool AreSourceRowsThreadSafe {
			get { return _AreSourceRowsThreadSafe; }
			set {
				if(AreSourceRowsThreadSafe == value)
					return;
				TestCanChangeProperties();
				_AreSourceRowsThreadSafe = value;
				ForceCatchUp();
			}
		}
		void TestCanChangeProperties() {
			if(_AsyncListServer != null)
				throw new InvalidOperationException("It's impossible to change InstantFeedback source properties when it is already used by grid.");
		}
		void ForceCatchUp() {
			if(_DTWrapper != null) {
				_DTWrapper.ElementType = DesignTimeElementType;
				_DTWrapper.AreThreadSafe = AreSourceRowsThreadSafe;
			}
		}
		public event EventHandler<GetQueryableEventArgs> GetQueryable;
		public event EventHandler<GetQueryableEventArgs> DismissQueryable;
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
			wrapper.AreThreadSafe = AreSourceRowsThreadSafe;
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
			GetQueryableEventArgs args = new GetQueryableEventArgs();
			e.Tag = args;
			if(!string.IsNullOrEmpty(this.KeyExpression))
				args.KeyExpression = this.KeyExpression;
			args.AreSourceRowsThreadSafe = this.AreSourceRowsThreadSafe;
			if(this.GetQueryable != null)
				this.GetQueryable(this, args);
			LinqServerModeSource src = new LinqServerModeSource();
			e.ListServerSource = src;
			if(args.QueryableSource == null) {
				src.KeyExpression = "Message";
				src.QueryableSource = new GetQueryableNotHandledMessenger[] { new GetQueryableNotHandledMessenger() }.AsQueryable();
			} else {
				src.KeyExpression = args.KeyExpression;
				src.QueryableSource = args.QueryableSource;
				src.DefaultSorting = this.DefaultSorting;
			}
		}
		void listServerFree(object sender, ListServerGetOrFreeEventArgs e) {
			GetQueryableEventArgs args = ((GetQueryableEventArgs)e.Tag);
			if(DismissQueryable != null)
				DismissQueryable(this, args);
		}
		void getTypeInfo(object sender, GetTypeInfoEventArgs e) {
			GetQueryableEventArgs getQueryableArgs = (GetQueryableEventArgs)e.Tag;
			PropertyDescriptorCollection sourceDescriptors = ListBindingHelper.GetListItemProperties(e.ListServerSource);
			if(getQueryableArgs.QueryableSource == null) {
				e.TypeInfo = new TypeInfoNoQueryable(this.DesignTimeElementType);
			} else if(getQueryableArgs.AreSourceRowsThreadSafe) {
				e.TypeInfo = new TypeInfoThreadSafe(sourceDescriptors);
			} else {
				e.TypeInfo = new TypeInfoProxied(sourceDescriptors, this.DesignTimeElementType);
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
		object IDXCloneable.DXClone() {
			return DXClone();
		}
		protected virtual object DXClone() {
			LinqInstantFeedbackSource clone = DXCloneCreate();
			clone._AreSourceRowsThreadSafe = this._AreSourceRowsThreadSafe;
			clone._DefaultSorting = this._DefaultSorting;
			clone._ElementType = this._ElementType;
			clone._KeyExpression = this._KeyExpression;
			clone.IsDisposed = this.IsDisposed;
			clone.GetQueryable = this.GetQueryable;
			clone.DismissQueryable = this.DismissQueryable;
			return clone;
		}
		protected virtual LinqInstantFeedbackSource DXCloneCreate() {
			return new LinqInstantFeedbackSource();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public object ExtractOriginalRow(object uiThreadRow) {
			return ReadonlyThreadSafeProxyForObjectFromAnotherThread.ExtractOriginalRow(uiThreadRow);
		}
	}
}
namespace DevExpress.Data.Linq.Helpers {
	public class AsyncListDesignTimeWrapper: IBindingList, ITypedList {
		Type _ElementType;
		bool _AreThreadSafe;
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
		public bool AreThreadSafe {
			get { return _AreThreadSafe; }
			set {
				if(AreThreadSafe == value)
					return;
				_AreThreadSafe = value;
				if(_Descriptors != null) {
					_Descriptors = null;
					ListChanged(this, new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, -1));
				}
			}
		}
		PropertyDescriptorCollection GetDescriptors() {
			PropertyDescriptorCollection full = TypeDescriptor.GetProperties(ElementType);
			if(AreThreadSafe)
				return full;
			return new PropertyDescriptorCollection(full.Cast<PropertyDescriptor>().Where(pd => !TypeInfoProxied.IsNotThreadSafe(pd.PropertyType)).ToArray());
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
	public abstract class TypeInfoBase {
		public abstract PropertyDescriptorCollection UIDescriptors { get; }
		public abstract object GetWorkerThreadRowInfo(object workerRow);
		public abstract object GetUIThreadRow(object rowInfo);
	}
	public class TypeInfoThreadSafe: TypeInfoBase {
		readonly PropertyDescriptorCollection PropertyDescriptors;
		public TypeInfoThreadSafe(PropertyDescriptorCollection propertyDescriptors) {
			this.PropertyDescriptors = propertyDescriptors;
		}
		public override PropertyDescriptorCollection UIDescriptors { get { return PropertyDescriptors; } }
		public override object GetWorkerThreadRowInfo(object workerRow) { return workerRow; }
		public override object GetUIThreadRow(object rowInfo) { return rowInfo; }
	}
	public class TypeInfoProxied: TypeInfoBase {
		readonly PropertyDescriptorCollection uiDescriptors;
		readonly PropertyDescriptor[] workerDescriptors;
		public TypeInfoProxied(PropertyDescriptorCollection workerThreadDescriptors, Type designTimeType) {
			PropertyDescriptorCollection prototypes = designTimeType == null ? workerThreadDescriptors : TypeDescriptor.GetProperties(designTimeType);
			List<PropertyDescriptor> ui = new List<PropertyDescriptor>(prototypes.Count);
			List<PropertyDescriptor> wr = new List<PropertyDescriptor>(prototypes.Count);
			foreach(PropertyDescriptor proto in prototypes) {
				if(IsNotThreadSafe(proto.PropertyType))
					continue;
				ui.Add(new ReadonlyThreadSafeProxyForObjectFromAnotherThreadPropertyDescriptor(proto, ui.Count));
				wr.Add(workerThreadDescriptors.Find(proto.Name, false));
			}
			uiDescriptors = new PropertyDescriptorCollection(ui.ToArray(), true);
			workerDescriptors = wr.ToArray();
		}
		public static bool IsNotThreadSafe(Type type) {
			if(type == null)
				return true;
			if(typeof(IListSource).IsAssignableFrom(type))
				return true;
			if(type.IsArray)
				return false;
			if(typeof(System.Collections.IList).IsAssignableFrom(type))
				return true;
			return false;
		}
		public override PropertyDescriptorCollection UIDescriptors {
			get { return uiDescriptors; }
		}
		public override object GetWorkerThreadRowInfo(object workerRow) {
			object[] rv = new object[workerDescriptors.Length];
			for(int i = 0; i < workerDescriptors.Length; ++i) {
				PropertyDescriptor pd = workerDescriptors[i];
				if(pd != null)
					rv[i] = pd.GetValue(workerRow);
			}
			return new ReadonlyThreadSafeProxyForObjectFromAnotherThread(workerRow, rv);
		}
		public override object GetUIThreadRow(object rowInfo) {
			return rowInfo;
		}
	}
	class TypeInfoNoQueryable: TypeInfoBase {
		readonly PropertyDescriptorCollection uiDescriptors;
		public TypeInfoNoQueryable(Type designTimeType) {
			Type type = designTimeType ?? typeof(GetQueryableNotHandledMessenger);
			PropertyDescriptorCollection prototypes = TypeDescriptor.GetProperties(type);
			List<PropertyDescriptor> ui = new List<PropertyDescriptor>(prototypes.Count);
			foreach(PropertyDescriptor proto in prototypes) {
				ui.Add(new NoQueryablePropertyDescriptor(proto.Name, proto.PropertyType));
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
		class NoQueryablePropertyDescriptor: PropertyDescriptor {
			readonly Type Type;
			public NoQueryablePropertyDescriptor(string name, Type type)
				: base(name, new Attribute[0]) {
				this.Type = type;
			}
			public override bool CanResetValue(object component) {
				return false;
			}
			public override Type ComponentType {
				get { return typeof(GetQueryableNotHandledMessenger); }
			}
			public override object GetValue(object component) {
				if(this.PropertyType == typeof(string))
					return GetQueryableNotHandledMessenger.MessageText;
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
	class GetQueryableNotHandledMessenger {
		public static string MessageText = "Please handle the " + typeof(LinqInstantFeedbackSource).Name + ".GetQueryable event and provide a valid QueryableSource and Key";
		public string Message { get { return MessageText; } }
	}
}
