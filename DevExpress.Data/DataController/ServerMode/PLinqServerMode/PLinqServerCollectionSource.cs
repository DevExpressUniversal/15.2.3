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
using System.Linq;
using System.Text;
using System.ComponentModel;
using DevExpress.Data.Helpers;
namespace DevExpress.Data.PLinq.Helpers {
	using System.ComponentModel.Design;
	using System.Globalization;
#if !SL
	using System.Windows.Forms.Design;
	using System.Drawing;
	public class PLinqServerModeSourceObjectTypeConverter : TypeListConverter {
		SortedList<string, Type> typesCache;
		public const string None = "(none)";
		public PLinqServerModeSourceObjectTypeConverter() : base(new Type[0]) { }
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			SortedList<string, Type> list = new SortedList<string, Type>();
			list.Add(None, null);
			try {
				ITypeDiscoveryService typeDiscovery = (ITypeDiscoveryService)context.GetService(typeof(ITypeDiscoveryService));
				if(typeDiscovery != null) {
					foreach(Type t in typeDiscovery.GetTypes(typeof(object), true)) {
						if(t.Namespace == null || t.Namespace.StartsWith("DevExpress."))
							continue;
						if(t.ContainsGenericParameters)
							continue;
						if(t.IsValueType)
							continue;
						if(t.IsAbstract && t.IsSealed)
							continue;
						list.Add(t.FullName, t);
					}
				}
			} catch(Exception e) {
				IUIService s = (IUIService)context.GetService(typeof(IUIService));
				if(s != null)
					s.ShowError(e);
			}
			typesCache = list;
			return new StandardValuesCollection(list.Values.ToArray());
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object val) {
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
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object val, Type destType) {
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
}
namespace DevExpress.Data.PLinq {
	using DevExpress.Data.PLinq.Helpers;
	using System.Threading;
#if SL
	using DevExpress.Xpf.ComponentModel;
	using DevExpress.Data.Browsing;
#else
	[System.Drawing.ToolboxBitmap(typeof(ResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "PLinqServerModeSource.bmp")]
#endif
	[DXToolboxItem(true)]
	[DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabData)]
	public class PLinqServerModeSource : Component, IListSource, ISupportInitialize, IPLinqServerModeFrontEndOwner, IDXCloneable {
#if !SL
		bool IListSource.ContainsListCollection {
			get { return false; }
		}
#endif
		object IDXCloneable.DXClone() {
			return DXClone();
		}
		protected virtual object DXClone() {
			PLinqServerModeSource clone = DXCloneCreate();
			clone._DefaultSorting = this._DefaultSorting;
			clone._ElementType = this._ElementType;
			clone._DegreeOfParallelism = this._DegreeOfParallelism;
			clone.InconsistencyDetected = this.InconsistencyDetected;
			clone.ExceptionThrown = this.ExceptionThrown;
			clone._Source = this._Source;
			return clone;
		}
		protected virtual PLinqServerModeSource DXCloneCreate() {
			return new PLinqServerModeSource();
		}
		IList IListSource.GetList() {
			return List;
		}
		PLinqServerModeFrontEnd _List;
		PLinqServerModeFrontEnd List {
			get {
				if(_List == null) {
					_List = CreateList();
					_List.InconsistencyDetected += new EventHandler<ServerModeInconsistencyDetectedEventArgs>(_List_InconsistencyDetected);
					_List.ExceptionThrown += new EventHandler<ServerModeExceptionThrownEventArgs>(_List_ExceptionThrown);
				}
				return _List;
			}
		}
		public void Reload() {
			List.Refresh();
		}
		protected virtual PLinqServerModeFrontEnd CreateList() {
			return new PLinqServerModeFrontEnd(this);
		}
		void ForceCatchUp() {
			if(!IsInitialized())
				return;
			List.CatchUp();
		}
		Type _ElementType;
		[
#if !SL
	DevExpressDataLocalizedDescription("PLinqServerModeSourceElementType"),
#endif
		RefreshProperties(RefreshProperties.All), DefaultValue(null)]
#if !SL //TODO SL
		[TypeConverter(typeof(PLinqServerModeSourceObjectTypeConverter))]
#endif
		public Type ElementType {
			get { return _ElementType; }
			set {
				if (Source != null) {
					value = PLinqServerModeCore.ExtractGenericEnumerableType(Source);
				}
				if (ElementType == value)
					return;
				_ElementType = value;
				ForceCatchUp();
			}
		}
		string _DefaultSorting = string.Empty;
		[DefaultValue("")]
		public string DefaultSorting {
			get { return _DefaultSorting; }
			set {
				if(value == null) value = string.Empty;
				if(DefaultSorting == value)
					return;
				_DefaultSorting = value;
				ForceCatchUp();
			}
		}
		int? _DegreeOfParallelism;
		[DefaultValue(null)]
		public int? DegreeOfParallelism {
			get { return _DegreeOfParallelism; }
			set
			{
				if(DegreeOfParallelism == value)
					return;
				_DegreeOfParallelism = value;
				ForceCatchUp();
			}
		}
		IEnumerable _Source;
		[
		DefaultValue(null), RefreshProperties(RefreshProperties.All)]
		public IEnumerable Source {
			get { return _Source; }
			set {
				if (Source == value)
					return;
				_Source = value;
				if(Source != null)
					ElementType = PLinqServerModeCore.ExtractGenericEnumerableType(Source);
				ForceCatchUp();
			}
		}
		int _initCount = 0;
		bool IsInitialized() {
			return _initCount == 0;
		}
		void ISupportInitialize.BeginInit() {
			++_initCount;
			ForceCatchUp();
		}
		void ISupportInitialize.EndInit() {
			--_initCount;
			ForceCatchUp();
		}
		class PostState {
			public bool ShouldFailWithException;
		}
		void _List_ExceptionThrown(object sender, ServerModeExceptionThrownEventArgs e) {
			OnExceptionThrown(e);
		}
		void _List_InconsistencyDetected(object sender, ServerModeInconsistencyDetectedEventArgs e) {
			OnInconsistencyDetected(e);
			if(e.Handled)
				return;
			SynchronizationContext context = SynchronizationContext.Current;
			if(IsGoodContext(context)) {
				PostState state = new PostState();
				state.ShouldFailWithException = true;
				context.Post(DoPostpronedReload, state);
				state.ShouldFailWithException = false;
			} else {
				FailUnderAspOrAnotherNonPostEnvironment();
			}
		}
		void DoPostpronedReload(object state) {
			PostState postedState = (PostState)state;
			if(postedState.ShouldFailWithException)
				FailUnderAspOrAnotherNonPostEnvironment();
			else
				Reload();
		}
		void FailUnderAspOrAnotherNonPostEnvironment() {
		}
		protected virtual bool IsGoodContext(SynchronizationContext context) {
			if(context == null)
				return false;
			return true;
		}
		public event EventHandler<ServerModeExceptionThrownEventArgs> ExceptionThrown;
		public event EventHandler<ServerModeInconsistencyDetectedEventArgs> InconsistencyDetected;
		protected virtual void OnExceptionThrown(ServerModeExceptionThrownEventArgs e) {
			if(ExceptionThrown != null)
				ExceptionThrown(this, e);
		}
		protected virtual void OnInconsistencyDetected(ServerModeInconsistencyDetectedEventArgs e) {
			if(InconsistencyDetected != null)
				InconsistencyDetected(this, e);
		}
		Type IPLinqServerModeFrontEndOwner.ElementType {
			get { return this.ElementType; }
		}
		IEnumerable IPLinqServerModeFrontEndOwner.Source {
			get { return this.Source; }
		}
		bool? _IsDesignMode;
		bool IPLinqServerModeFrontEndOwner.IsReadyForTakeOff() {
			if(Source == null)
				return false;
			if(!this.IsInitialized())
				return false;
			if(IsDesignModeHelper.GetIsDesignModeBypassable(this, ref _IsDesignMode))
				return false;
			return true;
		}
	}
}
