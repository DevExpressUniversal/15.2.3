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
#if SL
using DevExpress.Xpf.ComponentModel;
using DevExpress.Data.Browsing;
#endif
namespace DevExpress.Data.Helpers {
	public static class IsDesignModeHelper {
		static bool _BypassDesignModeAlterationDetection = false;
		[Obsolete("It is not recommended to use this option in your code. Refer to the www.devexpress.com/issue=T121952 KB Article for more details.")]
		public static bool BypassDesignModeAlterationDetection {
			get { return _BypassDesignModeAlterationDetection; }
			set { _BypassDesignModeAlterationDetection = value; }
		}
		public static bool GetCurrentBypassDesignModeAlterationDetection() {
			return _BypassDesignModeAlterationDetection;
		}
		static bool GetCurrentDesignMode(Component component) {
			var site = component.Site;
			return site != null && site.DesignMode;
		}
		public static void Validate(Component component, ref bool? isDesignTime) {
			if(!isDesignTime.HasValue)
				return;
			var current = GetCurrentDesignMode(component);
			if(isDesignTime.Value != current) {
				throw new InvalidOperationException(string.Format("The DesignMode property changed for component '{0}' from {1} to {2} during or after initialization. Make sure that the component is not placed onto a user control or a form that is visually inherited or placed onto another control in design mode. Refer to the www.devexpress.com/issue=T121952 KB Article for more details.", component, isDesignTime, current));
			}
		}
		public static bool GetIsDesignMode(Component component, ref bool? isDesignTime) {
			Validate(component, ref isDesignTime);
			if(!isDesignTime.HasValue) {
				isDesignTime = GetCurrentDesignMode(component);
			}
			return isDesignTime.Value;
		}
		public static bool GetIsDesignModeBypassable(Component component, ref bool? isDesignTime) {
			if(GetCurrentBypassDesignModeAlterationDetection())
				return GetCurrentDesignMode(component);
			else
				return GetIsDesignMode(component, ref isDesignTime);
		}
	}
}
namespace DevExpress.Data.Linq.Helpers {
	using System.ComponentModel.Design;
	using System.Globalization;
#if !SL
	using System.Windows.Forms.Design;
	public class LinqServerModeSourceObjectTypeConverter : TypeListConverter {
		SortedList<string, Type> typesCache;
		public const string None = "(none)";
		public LinqServerModeSourceObjectTypeConverter() : base(new Type[0]) { }
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
						if(list.ContainsKey(t.FullName))
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
namespace DevExpress.Data.Linq {
	using DevExpress.Data.Linq.Helpers;
	using System.Drawing;
	using System.Threading;
#if SL
	using DevExpress.Xpf.ComponentModel;
	using DevExpress.Data.Browsing;
#else
	[ToolboxBitmap(typeof(ResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "LinqServerModeSource.bmp")]
#endif
	[DXToolboxItem(true)]
	[DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabData)]
	public class LinqServerModeSource : Component, IListSource, ISupportInitialize, ILinqServerModeFrontEndOwner, IDXCloneable {
#if !SL
		bool IListSource.ContainsListCollection {
			get { return false; }
		}
#endif
		object IDXCloneable.DXClone() {
			return DXClone();
		}
		protected virtual object DXClone() {
			LinqServerModeSource clone = DXCloneCreate();
			clone._DefaultSorting = this._DefaultSorting;
			clone._ElementType = this._ElementType;
			clone._KeyExpression = this._KeyExpression;
			clone.InconsistencyDetected = this.InconsistencyDetected;
			clone.ExceptionThrown = this.ExceptionThrown;
			clone._QueryableSource = this._QueryableSource;
			return clone;
		}
		protected virtual LinqServerModeSource DXCloneCreate() {
			return new LinqServerModeSource();
		}
		IList IListSource.GetList() {
			return List;
		}
		LinqServerModeFrontEnd _List;
		LinqServerModeFrontEnd List {
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
		protected virtual LinqServerModeFrontEnd CreateList() {
			return new LinqServerModeFrontEnd(this);
		}
		void ForceCatchUp() {
			if(!IsInitialized())
				return;
			List.CatchUp();
		}
		Type _ElementType;
		[
#if !SL
	DevExpressDataLocalizedDescription("LinqServerModeSourceElementType"),
#endif
		RefreshProperties(RefreshProperties.All), DefaultValue(null)]
#if !SL //TODO SL
		[TypeConverter(typeof(LinqServerModeSourceObjectTypeConverter))]
#endif
		public Type ElementType {
			get { return _ElementType; }
			set {
				if (QueryableSource != null) {
					value = QueryableSource.ElementType;
				}
				if (ElementType == value)
					return;
				_ElementType = value;
				FillKeyExpression();
				ForceCatchUp();
			}
		}
		void FillKeyExpression() {
			if(ElementType == null)
				return;
			if(KeyExpression != null && KeyExpression.IndexOfAny(new char[] { ',', ';' }) >= 0)
				return;
			try {
				if(ElementType.GetProperty(KeyExpression) != null)
					return;
			} catch { }
			string guessed = LinqServerModeCore.GuessKeyExpression(ElementType);
			if(!string.IsNullOrEmpty(guessed))
				KeyExpression = guessed;
		}
		string _KeyExpression = string.Empty;
		[
#if !SL
	DevExpressDataLocalizedDescription("LinqServerModeSourceKeyExpression"),
#endif
		DefaultValue("")]
		public string KeyExpression {
			get { return _KeyExpression; }
			set {
				if (KeyExpression == value)
					return;
				_KeyExpression = value;
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
		IQueryable _QueryableSource;
		[
#if !SL
	DevExpressDataLocalizedDescription("LinqServerModeSourceQueryableSource"),
#endif
		DefaultValue(null), RefreshProperties(RefreshProperties.All)]
		public IQueryable QueryableSource {
			get { return _QueryableSource; }
			set {
				if (QueryableSource == value)
					return;
				_QueryableSource = value;
				if (QueryableSource != null)
					ElementType = QueryableSource.ElementType;
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
		void _List_ExceptionThrown(object sender, ServerModeExceptionThrownEventArgs e) {
			OnExceptionThrown(new LinqServerModeExceptionThrownEventArgs(e.Exception));
		}
		void _List_InconsistencyDetected(object sender, ServerModeInconsistencyDetectedEventArgs e) {
			LinqServerModeInconsistencyDetectedEventArgs ee = new LinqServerModeInconsistencyDetectedEventArgs();
			ee.Handled = e.Handled;
			OnInconsistencyDetected(ee);
			e.Handled = ee.Handled;
			if(e.Handled)
				return;
			e.Handled = true;
			InconsistentHelper.PostpronedInconsistent(() => Reload(), null);
		}
		public event LinqServerModeExceptionThrownEventHandler ExceptionThrown;
		public event LinqServerModeInconsistencyDetectedEventHandler InconsistencyDetected;
		protected virtual void OnExceptionThrown(LinqServerModeExceptionThrownEventArgs e) {
			if(ExceptionThrown != null)
				ExceptionThrown(this, e);
		}
		protected virtual void OnInconsistencyDetected(LinqServerModeInconsistencyDetectedEventArgs e) {
			if(InconsistencyDetected != null)
				InconsistencyDetected(this, e);
		}
		Type ILinqServerModeFrontEndOwner.ElementType {
			get { return this.ElementType; }
		}
		IQueryable ILinqServerModeFrontEndOwner.QueryableSource {
			get { return this.QueryableSource; }
		}
		bool? _isDesignMode;
		bool ILinqServerModeFrontEndOwner.IsReadyForTakeOff() {
			if(KeyExpression == null || KeyExpression.Length == 0)
				return false;
			if(QueryableSource == null)
				return false;
			if(!this.IsInitialized())
				return false;
			if(IsDesignModeHelper.GetIsDesignModeBypassable(this, ref _isDesignMode))
				return false;
			return true;
		}
		string ILinqServerModeFrontEndOwner.KeyExpression {
			get { return this.KeyExpression; }
		}
	}
	public class LinqServerModeExceptionThrownEventArgs : EventArgs {
		Exception _Exception;
		public Exception Exception {
			get { return _Exception; }
		}
		public LinqServerModeExceptionThrownEventArgs(Exception exception) {
			this._Exception = exception;
		}
	}
	public class LinqServerModeInconsistencyDetectedEventArgs : EventArgs {
		bool _handled = false;
		public bool Handled {
			get { return _handled; }
			set { _handled = value; }
		}
	}
	public delegate void LinqServerModeExceptionThrownEventHandler(object sender, LinqServerModeExceptionThrownEventArgs e);
	public delegate void LinqServerModeInconsistencyDetectedEventHandler(object sender, LinqServerModeInconsistencyDetectedEventArgs e);
}
