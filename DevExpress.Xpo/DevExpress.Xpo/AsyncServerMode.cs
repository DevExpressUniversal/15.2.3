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
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.Data.Async.Helpers;
using DevExpress.Xpo.Metadata;
using System.Threading;
using System.ComponentModel;
using System.Linq;
#if SL
using DevExpress.Data.Browsing;
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using DevExpress.Xpf.ComponentModel;
#endif
namespace DevExpress.Xpo {
	using DevExpress.Xpo.Helpers;
	using System.Threading;
	using DevExpress.Data.Helpers;
	using System.Drawing;
	[DXToolboxItem(true)]
	[DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabOrmComponents)]
	[DefaultEvent("ResolveSession")]
	[ToolboxBitmap(typeof(ToolboxIcons.ToolboxIconsRootNS), "XPInstantFeedbackSource")]
	public class XPInstantFeedbackSource: Component, IListSource, IXPClassInfoProvider, IDXCloneable {
		public XPInstantFeedbackSource() { }
		public XPInstantFeedbackSource(IContainer container)
			: this() {
			container.Add(this);
		}
		void Init(Type objectType, string assemblyName, string typeName, string displayableProperties, CriteriaOperator fixedCriteria, EventHandler<ResolveSessionEventArgs> resolveSession, EventHandler<ResolveSessionEventArgs> dismissSession) {
			this.ObjectType = objectType;
			this._AssemblyName = assemblyName;
			this._TypeName = typeName;
			this._DisplayableProperties = displayableProperties;
			this._FixedFilter = fixedCriteria;
			if(resolveSession != null)
				ResolveSession += resolveSession;
			if(dismissSession != null)
				DismissSession += dismissSession;
		}
		public XPInstantFeedbackSource(Type objectType, string displayableProperties, CriteriaOperator fixedCriteria, EventHandler<ResolveSessionEventArgs> resolveSession, EventHandler<ResolveSessionEventArgs> dismissSession) {
			Init(objectType, null, null, displayableProperties, fixedCriteria, resolveSession, dismissSession);
		}
		public XPInstantFeedbackSource(string assemblyName, string typeName, string displayableProperties, CriteriaOperator fixedCriteria, EventHandler<ResolveSessionEventArgs> resolveSession, EventHandler<ResolveSessionEventArgs> dismissSession) {
			Init(null, assemblyName, typeName, displayableProperties, fixedCriteria, resolveSession, dismissSession);
		}
		static string FillDefaultDisplayablePropertiesIfNotProvided(XPClassInfo ci, string providedDiplayableProperties) {
			if(string.IsNullOrEmpty(providedDiplayableProperties))
				return GetDefaultDisplayableProperties(ci);
			else
				return providedDiplayableProperties;
		}
		public XPInstantFeedbackSource(XPClassInfo classInfo, string displayableProperties, CriteriaOperator fixedCriteria, EventHandler<ResolveSessionEventArgs> resolveSession, EventHandler<ResolveSessionEventArgs> dismissSession) {
			Init(classInfo.ClassType, classInfo.AssemblyName, classInfo.FullName, FillDefaultDisplayablePropertiesIfNotProvided(classInfo, displayableProperties), fixedCriteria, resolveSession, dismissSession);
		}
		static EventHandler<T> ToEventHandler<T>(Action<T> action) where T : EventArgs {
			if(action == null)
				return null;
			else
				return delegate(object sender, T e) {
					action(e);
				};
		}
		public XPInstantFeedbackSource(Type objectType, string displayableProperties, CriteriaOperator fixedCriteria, Action<ResolveSessionEventArgs> resolveSession, Action<ResolveSessionEventArgs> dismissSession)
			: this(objectType, displayableProperties, fixedCriteria
			, ToEventHandler(resolveSession), ToEventHandler(dismissSession)) {
		}
		public XPInstantFeedbackSource(string assemblyName, string typeName, string displayableProperties, CriteriaOperator fixedCriteria, Action<ResolveSessionEventArgs> resolveSession, Action<ResolveSessionEventArgs> dismissSession)
			: this(assemblyName, typeName, displayableProperties, fixedCriteria
			, ToEventHandler(resolveSession), ToEventHandler(dismissSession)) {
		}
		public XPInstantFeedbackSource(XPClassInfo classInfo, string displayableProperties, CriteriaOperator fixedCriteria, Action<ResolveSessionEventArgs> resolveSession, Action<ResolveSessionEventArgs> dismissSession)
			: this(classInfo, displayableProperties, fixedCriteria
			, ToEventHandler(resolveSession), ToEventHandler(dismissSession)) {
		}
		public XPInstantFeedbackSource(Type objectType, string displayableProperties, CriteriaOperator fixedCriteria)
			: this(objectType, displayableProperties, fixedCriteria, (EventHandler<ResolveSessionEventArgs>)null, null) {
		}
		public XPInstantFeedbackSource(string assemblyName, string typeName, string displayableProperties, CriteriaOperator fixedCriteria)
			: this(assemblyName, typeName, displayableProperties, fixedCriteria, (EventHandler<ResolveSessionEventArgs>)null, null) {
		}
		public XPInstantFeedbackSource(XPClassInfo classInfo, string displayableProperties, CriteriaOperator fixedCriteria)
			: this(classInfo, displayableProperties, fixedCriteria, (EventHandler<ResolveSessionEventArgs>)null, null) {
		}
		public XPInstantFeedbackSource(Type objectType)
			: this(objectType, null, null) {
		}
		public XPInstantFeedbackSource(string assemblyName, string typeName)
			: this(assemblyName, typeName, null, null) {
		}
		public XPInstantFeedbackSource(XPClassInfo classInfo)
			: this(classInfo, null, null) {
		}
		Type _ElementType;
		string _AssemblyName, _TypeName;
		CriteriaOperator _FixedFilter;
		string _DisplayableProperties;
		string _DefaultSorting;
		public event EventHandler<ResolveSessionEventArgs> ResolveSession;
		public event EventHandler<ResolveSessionEventArgs> DismissSession;
		[
#if !SL
	DevExpressXpoLocalizedDescription("XPInstantFeedbackSourceObjectType"),
#endif
 RefreshProperties(RefreshProperties.All)]
#if !SL
		[TypeConverter(typeof(XPInstantFeedbackSourceObjectTypeConverter))]
#endif
		[DefaultValue(null)]
		public Type ObjectType {
			get { return _ElementType; }
			set {
				if(ObjectType == value)
					return;
				TestCanChangeProperties();
				bool resetDisplayables = (_ElementType != null);
				_ElementType = value;
				if(resetDisplayables)
					_DisplayableProperties = null;
				ForceCatchUp();
			}
		}
		bool ShouldSerializeDisplayableProperties() {
			return DisplayableProperties != GetDefaultDisplayableProperties();
		}
		[
#if !SL
	DevExpressXpoLocalizedDescription("XPInstantFeedbackSourceDisplayableProperties"),
#endif
 RefreshProperties(RefreshProperties.All)]
#if !SL
		[Editor("DevExpress.Xpo.Design.DisplayablePropertiesEditor, " + AssemblyInfo.SRAssemblyXpoDesignFull, "System.Drawing.Design.UITypeEditor, System.Drawing")]
#endif
		public string DisplayableProperties {
			get {
				if(string.IsNullOrEmpty(_DisplayableProperties))
					_DisplayableProperties = GetDefaultDisplayableProperties();
				return _DisplayableProperties;
			}
			set {
				if(DisplayableProperties == value)
					return;
				TestCanChangeProperties();
				_DisplayableProperties = value;
				ForceCatchUp();
			}
		}
		bool ShouldSerializeDefaultSorting() {
			return !string.IsNullOrEmpty(DefaultSorting);
		}
#if !SL
	[DevExpressXpoLocalizedDescription("XPInstantFeedbackSourceDefaultSorting")]
#endif
#if !SL
		[Editor("DevExpress.Xpo.Design.DefaultSortingCollectionEditor, " + AssemblyInfo.SRAssemblyXpoDesignFull, "System.Drawing.Design.UITypeEditor, System.Drawing")]
#endif
		public string DefaultSorting {
			get {
				return _DefaultSorting;
			}
			set {
				if(DefaultSorting == value)
					return;
				TestCanChangeProperties();
				_DefaultSorting = value;
				ForceCatchUp();
			}
		}
		bool? _isDesignMode;
		protected bool IsDesignMode {
			get {
				return DevExpress.Data.Helpers.IsDesignModeHelper.GetIsDesignModeBypassable(this, ref _isDesignMode);
			}
		}
		XPDictionary GetDesignDictionary() {
#if !SL
			if(this.IsDesignMode)
				return XPInstantListDesignTimeWrapper.GetDesignDictionary(this.Site);
			else
#endif
				return XpoDefault.GetDictionary();
		}
		string GetDefaultDisplayableProperties() {
			return GetDefaultDisplayableProperties(GetDesignClassInfo());
		}
		static string GetDefaultDisplayableProperties(XPClassInfo ci) {
			if(ci == null)
				return null;
			return string.Join(";", XPInstantListDesignTimeWrapper.GetDefaultDisplayableProperties(ci));
		}
#if !SL
#if !SL
	[DevExpressXpoLocalizedDescription("XPInstantFeedbackSourceFixedFilterCriteria")]
#endif
		[Editor("DevExpress.Xpo.Design.XPInstantFeedbackSourceCriteriaEditor, " + AssemblyInfo.SRAssemblyXpoDesignFull, "System.Drawing.Design.UITypeEditor, System.Drawing")]
		[TypeConverter("DevExpress.Xpo.Design.CriteriaEditor, " + AssemblyInfo.SRAssemblyXpoDesignFull)]
#endif
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public CriteriaOperator FixedFilterCriteria {
			get { return _FixedFilter; }
			set {
				if(ReferenceEquals(FixedFilterCriteria, value))
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
		AsyncListServer2DatacontrollerProxy _AsyncListServer;
#if !SL
		XPInstantListDesignTimeWrapper _DTWrapper;
#endif
		System.Collections.IList _List;
#if !SL
		bool IListSource.ContainsListCollection {
			get { return false; }
		}
#endif
		System.Collections.IList IListSource.GetList() {
			if(_List == null) {
				if(IsDisposed)
					throw new ObjectDisposedException(this.ToString());
#if !SL
				if(this.IsDesignMode) {
					_List = _DTWrapper = CreateDesignTimeWrapper();
				} else {
#endif
					_List = _AsyncListServer = GetRunTimeProxy();
#if !SL
				}
#endif
			}
			return _List;
		}
#if !SL
		XPInstantListDesignTimeWrapper CreateDesignTimeWrapper() {
			XPInstantListDesignTimeWrapper wrapper = new XPInstantListDesignTimeWrapper(this.Site, this.ObjectType, this.DisplayableProperties);
			return wrapper;
		}
#endif
		AsyncListServer2DatacontrollerProxy GetRunTimeProxy() {
			XPAsyncListServerCore core = CreateRuntimeCore();
			core.ListServerGet += listServerGet;
			core.ListServerFree += listServerFree;
			core.GetTypeInfo += getTypeInfo;
			core.GetPropertyDescriptors += getPropertyDescriptors;
			core.GetWorkerThreadRowInfo += getWorkerRowInfo;
			core.GetUIThreadRow += getUIRow;
			AsyncListServer2DatacontrollerProxy rv = CreateRuntimeProxy(core);
			return rv;
		}
		protected virtual AsyncListServer2DatacontrollerProxy CreateRuntimeProxy(XPAsyncListServerCore core) {
			return new AsyncListServer2DatacontrollerProxy(core);
		}
		protected virtual XPAsyncListServerCore CreateRuntimeCore() {
			return new XPAsyncListServerCore(SynchronizationContext.Current);
		}
		void listServerGet(object sender, ListServerGetOrFreeEventArgs e) {
			ResolveSessionEventArgs args = new ResolveSessionEventArgs();
			listServerGetTag tag = new listServerGetTag();
			tag.Args = args;
			e.Tag = tag;
			if(this.ResolveSession != null)
				this.ResolveSession(this, args);
			Session session = null;
			if(args.Session != null)
				session = args.Session.Session;
			if(session == null) {
				session = tag.OurSession = new Session();
			}
			XPClassInfo ci = null;
			session.Dictionary.QueryClassInfo(ObjectType);
			if(!string.IsNullOrEmpty(_TypeName))
				ci = session.Dictionary.QueryClassInfo(_AssemblyName, _TypeName);
			if(ci == null)
				ci = session.Dictionary.QueryClassInfo(ObjectType);
			if(ci == null)
				ci = session.GetClassInfo<PersistentBase>();
			e.ListServerSource = tag.Src = new XPServerCollectionSource(session, ci, FixedFilterCriteria);
			tag.Src.DisplayableProperties = DisplayableProperties;
			tag.Src.DefaultSorting = DefaultSorting;
		}
		void listServerFree(object sender, ListServerGetOrFreeEventArgs e) {
			listServerGetTag tag = (listServerGetTag)e.Tag;
			if(tag.OurSession != null)
				tag.OurSession.Dispose();
			if(DismissSession != null)
				DismissSession(this, tag.Args);
		}
		void getTypeInfo(object sender, GetTypeInfoEventArgs e) {
			PropertyDescriptorCollection sourceDescriptors = 
#if SL
				ListBindingHelper.GetListItemProperties(e.ListServerSource);
#else
				System.Windows.Forms.ListBindingHelper.GetListItemProperties(e.ListServerSource);
#endif
			List<PropertyDescriptor> workers = new List<PropertyDescriptor>();
			List<PropertyDescriptor> uis = new List<PropertyDescriptor>();
			foreach(string propName in DisplayableProperties.Split(';').Where(s => !string.IsNullOrEmpty(s)).Distinct()) {
				PropertyDescriptor wpd = sourceDescriptors.Find(propName, false);
				PropertyDescriptor uipd = XPInstantListDesignTimeWrapper.GetMessagingDescriptorIfUnsafe(propName, wpd);
				if(uipd == null)
					uipd = new ReadonlyThreadSafeProxyForObjectFromAnotherThreadPropertyDescriptor(wpd, workers.Count);
				uis.Add(uipd);
				if(wpd != null)
					workers.Add(wpd);
			}
			e.TypeInfo = new PropertyDescriptorCollection[] { new PropertyDescriptorCollection(uis.ToArray(), true), new PropertyDescriptorCollection(workers.ToArray(), true) };
		}
		void getPropertyDescriptors(object sender, GetPropertyDescriptorsEventArgs e) {
			e.PropertyDescriptors = ((PropertyDescriptorCollection[])e.TypeInfo)[0];
		}
		void getWorkerRowInfo(object sender, GetWorkerThreadRowInfoEventArgs e) {
			object row = e.WorkerThreadRow;
			List<object> rv = new List<object>();
			PropertyDescriptorCollection getters = ((PropertyDescriptorCollection[])e.TypeInfo)[1];
			foreach(PropertyDescriptor pd in getters)
				rv.Add(pd.GetValue(row));
			e.RowInfo = new ReadonlyThreadSafeProxyForObjectFromAnotherThread(row, rv.ToArray());
		}
		void getUIRow(object sender, GetUIThreadRowEventArgs e) {
			e.UIThreadRow = e.RowInfo;
		}
		void TestCanChangeProperties() {
			if(_AsyncListServer != null)
				throw new InvalidOperationException(Res.GetString(Res.Async_CanChangeProperties));
		}
		void ForceCatchUp() {
#if !SL
			if(_DTWrapper != null) {
				_DTWrapper.ElementType = ObjectType;
				_DTWrapper.DisplayableProperties = DisplayableProperties;
			}
#endif
		}
		bool IsDisposed;
		protected override void Dispose(bool disposing) {
			IsDisposed = true;
			_List = null;
#if !SL
			_DTWrapper = null;
#endif
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
			XPInstantFeedbackSource clone = DXCloneCreate();
			clone._AssemblyName = this._AssemblyName;
			clone._DefaultSorting = this._DefaultSorting;
			clone._DisplayableProperties = this._DisplayableProperties;
			clone._ElementType = this._ElementType;
			clone._FixedFilter = this._FixedFilter;
			clone._TypeName = this._TypeName;
			clone.IsDisposed = this.IsDisposed;
			clone.ResolveSession = this.ResolveSession;
			clone.DismissSession = this.DismissSession;
			return clone;
		}
		protected virtual XPInstantFeedbackSource DXCloneCreate() {
			return new XPInstantFeedbackSource();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public object ExtractOriginalRow(object uiThreadRow) {
			return ReadonlyThreadSafeProxyForObjectFromAnotherThread.ExtractOriginalRow(uiThreadRow);
		}
		XPClassInfo GetDesignClassInfo() {
			XPDictionary dictionary = GetDesignDictionary();
			dictionary.QueryClassInfo(ObjectType);
			if(!string.IsNullOrEmpty(_TypeName)) {
				var ci = dictionary.QueryClassInfo(_AssemblyName, _TypeName);
				if(ci != null)
					return ci;
			}
			return dictionary.QueryClassInfo(ObjectType);
		}
		XPClassInfo IXPClassInfoProvider.ClassInfo {
			get { return GetDesignClassInfo(); }
		}
		XPDictionary Metadata.Helpers.IXPDictionaryProvider.Dictionary {
			get { return GetDesignDictionary(); }
		}
	}
}
namespace DevExpress.Xpo.Helpers {
	public class XPInstantListDesignTimeWrapper 
#if !SL
		: IBindingList, ITypedList 
#endif
	{
#if !SL
		readonly DefaultSession DesignSession;
		public XPInstantListDesignTimeWrapper(ISite site, Type type, string dispProps) {
			this.DesignSession = new DefaultSession(site);
			this.Site = site;
			this._ElementType = type;
			this._DisplayableProperties = dispProps;
		}
		readonly ISite Site;
		Type _ElementType;
		string _DisplayableProperties;
		PropertyDescriptorCollection _Descriptors;
		public Type ElementType {
			get { return _ElementType; }
			set {
				if(ElementType == value)
					return;
				_ElementType = value;
				InvalidateDescriptors();
			}
		}
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
		void InvalidateDescriptors() {
			if(_Descriptors != null) {
				_Descriptors = null;
				ListChanged(this, new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, -1));
			}
		}
#endif
		public static XPInstantPropertyDescriptorJustMessage GetMessagingDescriptorIfUnsafe(string name, PropertyDescriptor prototype) {
			if(prototype == null)
				return new XPInstantPropertyDescriptorJustMessage(name, string.Format("'{0}' member does not exist", name));
			if(ReflectionDictionary.DefaultCanGetClassInfoByType(prototype.PropertyType) || (typeof(System.Collections.IEnumerable).IsAssignableFrom(prototype.PropertyType) && !DevExpress.Xpo.DB.DBColumn.IsStorableType(prototype.PropertyType)))
				return new XPInstantPropertyDescriptorJustMessage(name, string.Format("'{0}' member is not safe", name));
			return null;
		}
#if !SL
		PropertyDescriptorCollection GetDescriptors() {
			return GetDescriptorsCore() ?? PropertyDescriptorCollection.Empty;
		}
		PropertyDescriptorCollection GetDescriptorsCore() {
			if(ElementType == null)
				return null;
			if(string.IsNullOrEmpty(DisplayableProperties))
				return null;
			XPClassInfo ci = DesignSession.Dictionary.QueryClassInfo(ElementType);
			if(ci == null)
				return null;
			PropertyDescriptorCollection props = DesignSession.GetProperties(ci);
			List<PropertyDescriptor> rv = new List<PropertyDescriptor>();
			foreach(string name in DisplayableProperties.Split(';').Where(s => !string.IsNullOrEmpty(s)).Distinct()) {
				PropertyDescriptor prototype = props.Find(name, false);
				if(prototype == null)
					continue;
				PropertyDescriptor pd = GetMessagingDescriptorIfUnsafe(name, prototype);
				if(pd == null)
					pd = prototype;
				rv.Add(pd);
			}
			return new PropertyDescriptorCollection(rv.ToArray(), true);
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
			return string.Empty.GetEnumerator();
		}
		#endregion
#endif
		public static IEnumerable<string> GetDefaultDisplayableProperties(XPClassInfo classInfo) {
			return GetDefaultDisplayableProperties(classInfo, 2);
		}
		public static IEnumerable<string> GetDefaultDisplayableProperties(XPClassInfo classInfo, int depthOfReferences) {
			return GetDefProps(depthOfReferences, classInfo);
		}
		static IEnumerable<string> GetDefProps(int depthLeft, XPClassInfo ci) {
			if(depthLeft < 0)
				yield break;
			foreach(XPMemberInfo mi in ci.Members) {
				if(!mi.IsVisibleInDesignTime)
					continue;
				if(!mi.IsPublic)
					continue;
				if(!(mi.IsPersistent || mi.IsAliased))
					continue;
				if(mi.IsDelayed)
					continue;
				if(mi.ReferenceType == null) {
					yield return mi.Name;
				} else {
					string prefix = mi.Name + ".";
					foreach(string prop in GetDefProps(depthLeft - 1, mi.ReferenceType))
						yield return prefix + prop;
				}
			}
		}
#if !SL
		public static XPDictionary GetDesignDictionary(ISite site) {
			return new DesignTimeReflection(site);
		}
#endif
	}
	public class XPInstantPropertyDescriptorJustMessage : PropertyDescriptor {
		readonly string Message;
		public XPInstantPropertyDescriptorJustMessage(string name, string message)
			: base(name, new Attribute[0]) {
			this.Message = message;
		}
		public override bool CanResetValue(object component) {
			return false;
		}
		public override Type ComponentType {
			get { return typeof(object[]); }
		}
		public override object GetValue(object component) {
			return Message;
		}
		public override bool IsReadOnly {
			get { return true; }
		}
		public override Type PropertyType {
			get { return typeof(string); }
		}
		public override void ResetValue(object component) {
			throw new NotSupportedException();
		}
		public override void SetValue(object component, object value) {
			throw new NotSupportedException();
		}
		public override bool ShouldSerializeValue(object component) {
			return true;
		}
	}
#if !SL
	public class XPInstantFeedbackSourceObjectTypeConverter : TypeListConverter {
		SortedList<string, Type> typesCache;
		public const string None = "(none)";
		public XPInstantFeedbackSourceObjectTypeConverter() : base(new Type[0]) { }
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			SortedList<string, Type> list = new SortedList<string, Type>();
			list.Add(None, null);
			try {
				XPDictionary dictionary = XPInstantListDesignTimeWrapper.GetDesignDictionary(((XPInstantFeedbackSource)context.Instance).Site);
				foreach(XPClassInfo ci in dictionary.Classes) {
					if(!ci.IsPersistent)
						continue;
					if(ci.ClassType == null)
						continue;
					if(ci.FullName.StartsWith("DevExpress.Xpo."))
						continue;
					list.Add(ci.FullName, ci.ClassType);
				}
			} catch(Exception e) {
				System.Windows.Forms.Design.IUIService s = (System.Windows.Forms.Design.IUIService)context.GetService(typeof(System.Windows.Forms.Design.IUIService));
				if(s != null)
					s.ShowError(e);
			}
			typesCache = list;
			return new StandardValuesCollection(new List<Type>(list.Values).ToArray());
		}
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object val) {
			string str = val as string;
			if(str != null) {
				if(str == None)
					return null;
				Type t;
				if(typesCache.TryGetValue(str, out t))
					return t;
				t = Type.GetType(str);
				if(t != null)
					return t;
			}
			return base.ConvertFrom(context, culture, val);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if(destinationType == typeof(string)) {
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object val, Type destType) {
			if(destType == typeof(string)) {
				if(val == null)
					return None;
				if(val is Type)
					return ((Type)val).FullName;
			}
			return base.ConvertTo(context, culture, val, destType);
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return false;
		}
	}
#endif
	public class XPAsyncListServerCore : AsyncListServerCore {
		public XPAsyncListServerCore(SynchronizationContext context) : base(context) { }
		protected override CommandQueue CreateCommandQueue(SynchronizationContext context, SendOrPostCallback somethingInTheOutputQueueCallback, EventHandler<ListServerGetOrFreeEventArgs> listServerGet, EventHandler<ListServerGetOrFreeEventArgs> listServerFree, EventHandler<GetTypeInfoEventArgs> getTypeInfo, EventHandler<GetWorkerThreadRowInfoEventArgs> getWorkerThreadRowInfo) {
			return new XPCommandQueue(context, somethingInTheOutputQueueCallback, listServerGet, listServerFree, getTypeInfo, getWorkerThreadRowInfo);
		}
	}
	public class XPCommandQueue : CommandQueue {
		public XPCommandQueue(SynchronizationContext context, SendOrPostCallback somethingInTheOutputQueueCallback, EventHandler<ListServerGetOrFreeEventArgs> listServerGet, EventHandler<ListServerGetOrFreeEventArgs> listServerFree, EventHandler<GetTypeInfoEventArgs> getTypeInfo, EventHandler<GetWorkerThreadRowInfoEventArgs> getWorkerThreadRowInfo)
			: base(context, somethingInTheOutputQueueCallback, listServerGet, listServerFree, getTypeInfo, getWorkerThreadRowInfo) {
		}
		protected override void Visit(DevExpress.Data.Async.CommandRefresh result) {
			((ISessionProvider)this.ListServer).Session.DropIdentityMap();
			base.Visit(result);
		}
	}
	public class listServerGetTag {
		public ResolveSessionEventArgs Args;
		public Session OurSession;
		public XPServerCollectionSource Src;
	}
}
