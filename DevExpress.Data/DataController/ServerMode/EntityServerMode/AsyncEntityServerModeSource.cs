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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Data.Helpers;
using DevExpress.Data.Linq.Helpers;
using DevExpress.Data.Async.Helpers;
using System.Threading;
using System.Windows.Forms;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.Data.Linq {
	[DXToolboxItem(true)]
	[ToolboxBitmap(typeof(ResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "EntityInstantFeedbackSource.bmp")]
	[DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabData)]	
	[DefaultEvent("GetQueryable")]
#if !SL
#endif
	public class EntityInstantFeedbackSource : Component, IListSource, IDXCloneable {
		public EntityInstantFeedbackSource() { }
		public EntityInstantFeedbackSource(EventHandler<GetQueryableEventArgs> getQueryable) {
			this.GetQueryable += getQueryable;
		}
		public EntityInstantFeedbackSource(EventHandler<GetQueryableEventArgs> getQueryable, EventHandler<GetQueryableEventArgs> freeQueryable)
			: this(getQueryable) {
			this.DismissQueryable += freeQueryable;
		}
		static EventHandler<T> ToEventHandler<T>(Action<T> action) where T: EventArgs {
			if(action == null)
				return null;
			else
				return delegate(object sender, T e) { action(e); };
		}
		public EntityInstantFeedbackSource(Action<GetQueryableEventArgs> getQueryable)
			: this(ToEventHandler(getQueryable)) {
		}
		public EntityInstantFeedbackSource(Action<GetQueryableEventArgs> getQueryable, Action<GetQueryableEventArgs> freeQueryable)
			: this(ToEventHandler(getQueryable), ToEventHandler(freeQueryable)) {
		}
		Type _ElementType;
		[RefreshProperties(RefreshProperties.All), DefaultValue(null)]
#if !SL //TODO SL
		[TypeConverter(typeof(LinqServerModeSourceObjectTypeConverter))]
#endif
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
		void FillKeyExpression() {
			if (DesignTimeElementType == null)
				return;
			if (KeyExpression != null && KeyExpression.IndexOfAny(new char[] { ',', ';' }) >= 0)
				return;
			try {
				if (DesignTimeElementType.GetProperty(KeyExpression) != null)
					return;
			} catch { }
			KeyExpression = EntityServerModeCore.GuessKeyExpression(DesignTimeElementType);
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
		void TestCanChangeProperties() {
			if (_AsyncListServer != null)
				throw new InvalidOperationException("Already in use!");
		}
		void ForceCatchUp() {
			if (_DTWrapper != null)
				_DTWrapper.ElementType = DesignTimeElementType;
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
			if (!string.IsNullOrEmpty(this.KeyExpression))
				args.KeyExpression = this.KeyExpression;
			args.AreSourceRowsThreadSafe = this.AreSourceRowsThreadSafe;
			if (this.GetQueryable != null)
				this.GetQueryable(this, args);
			EntityServerModeSource src = new EntityServerModeSource();
			e.ListServerSource = src;
			if (args.QueryableSource == null) {
				src.KeyExpression = "Message";
				src.QueryableSource = new GetQueryableNotHandledEntityMessenger[] { new GetQueryableNotHandledEntityMessenger() }.AsQueryable();
			} else {
				src.KeyExpression = args.KeyExpression;
				src.QueryableSource = args.QueryableSource;
				src.DefaultSorting = this.DefaultSorting;
			}
		}
		void listServerFree(object sender, ListServerGetOrFreeEventArgs e) {
			GetQueryableEventArgs args = ((GetQueryableEventArgs)e.Tag);
			if (DismissQueryable != null)
				DismissQueryable(this, args);
		}
		void getTypeInfo(object sender, GetTypeInfoEventArgs e) {
			GetQueryableEventArgs getQueryableArgs = (GetQueryableEventArgs)e.Tag;
			PropertyDescriptorCollection sourceDescriptors = ListBindingHelper.GetListItemProperties(e.ListServerSource);
			if (getQueryableArgs.QueryableSource == null) {
				e.TypeInfo = new TypeInfoNoQueryableEntity(this.DesignTimeElementType);
			} else if (getQueryableArgs.AreSourceRowsThreadSafe) {
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
		object IDXCloneable.DXClone() {
			return DXClone();
		}
		protected virtual object DXClone() {
			EntityInstantFeedbackSource clone = DXCloneCreate();
			clone._AreSourceRowsThreadSafe = this._AreSourceRowsThreadSafe;
			clone._DefaultSorting = this._DefaultSorting;
			clone._ElementType = this._ElementType;
			clone._KeyExpression = this._KeyExpression;
			clone.IsDisposed = this.IsDisposed;
			clone.GetQueryable = this.GetQueryable;
			clone.DismissQueryable = this.DismissQueryable;
			return clone;
		}
		protected virtual EntityInstantFeedbackSource DXCloneCreate() {
			return new EntityInstantFeedbackSource();
		}
	}
}
namespace DevExpress.Data.Linq.Helpers {
	class TypeInfoNoQueryableEntity : TypeInfoBase {
		readonly PropertyDescriptorCollection uiDescriptors;
		public TypeInfoNoQueryableEntity(Type designTimeType) {
			Type type = designTimeType ?? typeof(GetQueryableNotHandledEntityMessenger);
			PropertyDescriptorCollection prototypes = TypeDescriptor.GetProperties(type);
			List<PropertyDescriptor> ui = new List<PropertyDescriptor>(prototypes.Count);
			foreach (PropertyDescriptor proto in prototypes) {
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
		class NoQueryablePropertyDescriptor : PropertyDescriptor {
			readonly Type Type;
			public NoQueryablePropertyDescriptor(string name, Type type)
				: base(name, new Attribute[0]) {
				this.Type = type;
			}
			public override bool CanResetValue(object component) {
				return false;
			}
			public override Type ComponentType {
				get { return typeof(GetQueryableNotHandledEntityMessenger); }
			}
			public override object GetValue(object component) {
				if (this.PropertyType == typeof(string))
					return GetQueryableNotHandledEntityMessenger.MessageText;
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
	class GetQueryableNotHandledEntityMessenger {
		public static string MessageText = "Please handle the " + typeof(EntityInstantFeedbackSource).Name + ".GetQueryable event and provide a valid QueryableSource and Key";
		public string Message { get { return MessageText; } }
	}
}
