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
using System.Threading;
using DevExpress.Data.Async.Helpers;
using DevExpress.Data.Helpers;
using DevExpress.Data.Linq.Helpers;
using DevExpress.Data.Filtering;
using System.Windows.Forms;
using DevExpress.Data.WcfLinq.Helpers;
namespace DevExpress.Data.WcfLinq {
	public interface IOdata { }
	public class GetSourceEventArgs : EventArgs {
		ServerModeCoreExtender extender;
		public IQueryable Query { get; set; }		
		public string KeyExpression { get; set; }
		public bool AreSourceRowsThreadSafe { get; set; }
		public object Tag { get; set; }
		public ServerModeCoreExtender Extender {
			get { return extender; }
		}
		public GetSourceEventArgs(ServerModeCoreExtender extender) {
			this.extender = extender;
		}
	}
	[System.Drawing.ToolboxBitmap(typeof(ResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "WcfInstantFeedbackSource.bmp")]
	[DXToolboxItem(true)]
	[DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabData)]
	[DefaultEvent("GetSource")]
	public class WcfInstantFeedbackSource : Component, IListSource, IDXCloneable, IOdata {
		AsyncListServer2DatacontrollerProxy _AsyncListServer;
		AsyncListDesignTimeWrapper _DTWrapper;
		IList _List;
		public event EventHandler<GetSourceEventArgs> GetSource;
		public event EventHandler<GetSourceEventArgs> DismissSource;
		CriteriaOperator _FixedFilter;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public CriteriaOperator FixedFilterCriteria {
			get { return _FixedFilter; }
			set {
				if (ReferenceEquals(FixedFilterCriteria, value))
					return;
				TestCanChangeProperties();
				_FixedFilter = value;
				ForceCatchUp();
			}
		}
		[Browsable(false)]
		[DefaultValue("")]
		public string FixedFilterString {
			get {
				return CriteriaOperator.ToString(FixedFilterCriteria);
			}
			set {
				FixedFilterCriteria = CriteriaOperator.Parse(value);
			}
		}
		string _DefaultSorting = string.Empty;
		[DefaultValue("")]
		public string DefaultSorting {
			get { return _DefaultSorting; }
			set {
				if (DefaultSorting == value)
					return;
				TestCanChangeProperties();
				_DefaultSorting = value;
				ForceCatchUp();
			}
		}
		string _KeyExpression = string.Empty;
		[DefaultValue("")]
		public string KeyExpression {
			get { return _KeyExpression; }
			set {
				if (KeyExpression == value)
					return;
				TestCanChangeProperties();
				_KeyExpression = value;
				ForceCatchUp();
			}
		}
		bool _AreSourceRowsThreadSafe;
		[DefaultValue(false)]
		public bool AreSourceRowsThreadSafe {
			get { return _AreSourceRowsThreadSafe; }
			set {
				if (AreSourceRowsThreadSafe == value)
					return;
				TestCanChangeProperties();
				_AreSourceRowsThreadSafe = value;
				ForceCatchUp();
			}
		}
		public WcfInstantFeedbackSource() { }
		public WcfInstantFeedbackSource(EventHandler<GetSourceEventArgs> getSource) {
			this.GetSource += getSource;
		}
		public WcfInstantFeedbackSource(EventHandler<GetSourceEventArgs> getSource, EventHandler<GetSourceEventArgs> freeSource)
			: this(getSource) {
			this.DismissSource += freeSource;
		}
		static EventHandler<T> ToEventHandler<T>(Action<T> action) where T: EventArgs {
			if(action == null)
				return null;
			else
				return delegate(object sender, T e) { action(e); };
		}
		public WcfInstantFeedbackSource(Action<GetSourceEventArgs> getSource)
			: this(ToEventHandler(getSource)) {
		}
		public WcfInstantFeedbackSource(Action<GetSourceEventArgs> getSource, Action<GetSourceEventArgs> freeSource)
			: this(ToEventHandler(getSource)
			, ToEventHandler(freeSource)) {
		}
		Type _ElementType;
		[RefreshProperties(RefreshProperties.All), DefaultValue(null)]
		[TypeConverter(typeof(LinqServerModeSourceObjectTypeConverter))]
		public Type DesignTimeElementType {
			get { return _ElementType; }
			set {
				if (_ElementType == value)
					return;
				TestCanChangeProperties();
				_ElementType = value;
				FillKeyExpression();
				ForceCatchUp();
			}
		}
		void TestCanChangeProperties() {
			if (_AsyncListServer != null)
				throw new InvalidOperationException("Already in use!");
		}
		void FillKeyExpression() {
			if (DesignTimeElementType == null)
				return;
			try {
				if (DesignTimeElementType.GetProperty(KeyExpression) != null)
					return;
			} catch { }
			KeyExpression = WcfServerModeCore.GuessKeyExpression(DesignTimeElementType);
		}
		void ForceCatchUp() {
			if (_DTWrapper != null)
				_DTWrapper.ElementType = DesignTimeElementType;
		}
		#region IListSource Members
		bool IListSource.ContainsListCollection {
			get { return false; }
		}
		bool? _isDesignMode;
		IList IListSource.GetList() {
			bool designMode = IsDesignModeHelper.GetIsDesignModeBypassable(this, ref _isDesignMode);
			if (_List == null) {
				if (IsDisposed)
					throw new ObjectDisposedException(this.ToString());
				if (designMode) {
					_List = _DTWrapper = CreateDesignTimeWrapper();
				} else {
					_List = _AsyncListServer = CreateRunTimeProxy();
				}
			}
			return _List;
		}
		#endregion
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
			GetSourceEventArgs args = new GetSourceEventArgs(new ServerModeCoreExtender());
			e.Tag = args;
			if (!string.IsNullOrEmpty(this.KeyExpression))
				args.KeyExpression = this.KeyExpression;
			args.AreSourceRowsThreadSafe = this.AreSourceRowsThreadSafe;
			if (this.GetSource != null)
				this.GetSource(this, args);
			if (args.Query == null) {
				e.ListServerSource = new DummyListServer();
			} else {
				WcfServerModeSource src = new WcfServerModeSource(args.Extender);
				e.ListServerSource = src;
				src.KeyExpression = args.KeyExpression;
				src.Query = args.Query;				
				src.DefaultSorting = this.DefaultSorting;
				src.FixedFilterCriteria = this.FixedFilterCriteria;
			}
		}
		void listServerFree(object sender, ListServerGetOrFreeEventArgs e) {
			GetSourceEventArgs args = ((GetSourceEventArgs)e.Tag);
			if (DismissSource != null)
				DismissSource(this, args);
		}
		void getTypeInfo(object sender, GetTypeInfoEventArgs e) {
			GetSourceEventArgs getQueryArgs = (GetSourceEventArgs)e.Tag;
			PropertyDescriptorCollection sourceDescriptors = ListBindingHelper.GetListItemProperties(e.ListServerSource);
			if (getQueryArgs.Query == null) {
				e.TypeInfo = new TypeInfoNoSource(this.DesignTimeElementType);
			} else if (getQueryArgs.AreSourceRowsThreadSafe) {
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
			if (_AsyncListServer != null) {
				_AsyncListServer.Dispose();
				_AsyncListServer = null;
			}
			base.Dispose(disposing);
		}
		public void Refresh() {
			if (_AsyncListServer == null)
				return;
			_AsyncListServer.Refresh();
		}
		#region IDXCloneable Members
		object IDXCloneable.DXClone() {
			return DXClone();
		}
		protected virtual object DXClone() {
			WcfInstantFeedbackSource clone = DXCloneCreate();
			clone._AreSourceRowsThreadSafe = this._AreSourceRowsThreadSafe;
			clone._DefaultSorting = this._DefaultSorting;
			clone._ElementType = this._ElementType;
			clone._KeyExpression = this._KeyExpression;
			clone.IsDisposed = this.IsDisposed;
			clone.GetSource = this.GetSource;
			clone.DismissSource = this.DismissSource;
			return clone;
		}
		protected virtual WcfInstantFeedbackSource DXCloneCreate() {
			return new WcfInstantFeedbackSource();
		}
		#endregion
	}
}
namespace DevExpress.Data.WcfLinq.Helpers {
	class TypeInfoNoSource : TypeInfoBase {
		readonly PropertyDescriptorCollection uiDescriptors;
		public TypeInfoNoSource(Type designTimeType) {
			Type type = designTimeType ?? typeof(GetSourceNotHandledMessenger);
			PropertyDescriptorCollection prototypes = TypeDescriptor.GetProperties(type);
			List<PropertyDescriptor> ui = new List<PropertyDescriptor>(prototypes.Count);
			foreach (PropertyDescriptor proto in prototypes) {
				ui.Add(new NoSourcePropertyDescriptor(proto.Name, proto.PropertyType));
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
		class NoSourcePropertyDescriptor : PropertyDescriptor {
			readonly Type Type;
			public NoSourcePropertyDescriptor(string name, Type type)
				: base(name, new Attribute[0]) {
				this.Type = type;
			}
			public override bool CanResetValue(object component) {
				return false;
			}
			public override Type ComponentType {
				get { return typeof(GetSourceNotHandledMessenger); }
			}
			public override object GetValue(object component) {
				if (this.PropertyType == typeof(string))
					return GetSourceNotHandledMessenger.MessageText;
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
	class GetSourceNotHandledMessenger {
		public static string MessageText = "Please handle the " + typeof(WcfInstantFeedbackSource).Name + ".GetSource event and provide a valid Query and Key";
		public string Message { get { return MessageText; } }
	}
	class DummyListServer : List<GetSourceNotHandledMessenger>, IListServer {
		public DummyListServer()
			: base() {
				Add(new GetSourceNotHandledMessenger());
		}
		public void Apply(Filtering.CriteriaOperator filterCriteria, ICollection<ServerModeOrderDescriptor> sortInfo, int groupCount, ICollection<ServerModeSummaryDescriptor> groupSummaryInfo, ICollection<ServerModeSummaryDescriptor> totalSummaryInfo) {  }
		public event EventHandler<ServerModeExceptionThrownEventArgs> ExceptionThrown {
			add { }
			remove { }
		}
		public int FindIncremental(Filtering.CriteriaOperator expression, string value, int startIndex, bool searchUp, bool ignoreStartRow, bool allowLoop) {
			return 0;
		}
		public IList GetAllFilteredAndSortedRows() {
			return this;
		}
		public virtual bool PrefetchRows(ListSourceGroupInfo[] groupsToPrefetch, CancellationToken cancellationToken) {
			return this.Count >= 0;
		}
		public List<ListSourceGroupInfo> GetGroupInfo(ListSourceGroupInfo parentGroup) {
			return new List<ListSourceGroupInfo>(0);
		}
		public int GetRowIndexByKey(object key) {
			return 0;
		}
		public object GetRowKey(int index) {
			return this.Count == 0 ? null : this[0];
		}
		public List<object> GetTotalSummary() {
			return new List<object>();
		}
		public object[] GetUniqueColumnValues(Filtering.CriteriaOperator expression, int maxCount, bool includeFilteredOut) {
			return this.Count == 0 ? new object[0] : new object[] { this[0].Message };
		}
		public event EventHandler<ServerModeInconsistencyDetectedEventArgs> InconsistencyDetected {
			add { }
			remove { }
		}
		public int LocateByValue(Filtering.CriteriaOperator expression, object value, int startIndex, bool searchUp) {
			return 0;
		}
		public void Refresh() {
		}
	}
}
