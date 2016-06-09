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
using System.Windows;
using System.Windows.Data;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Validation;
using DevExpress.Xpf.Editors.Helpers;
namespace DevExpress.Xpf.Editors.EditStrategy {
	public enum SyncValueUpdateMode { Default, Update }
	public delegate object PropertyCoercionHandler(object baseValue);
	public delegate void PropertyUpdaterHandler(object baseValue);
	public class PropertyCoercionHelper {
		Locker setSyncValueLocker = new Locker();
		BaseEdit Editor { get; set; }
		EditStrategyBase EditStrategy { get { return Editor.EditStrategy; } }
		Dictionary<object, PropertyCoercionHandler> GetBaseValueHandlers { get; set; }
		Dictionary<object, PropertyCoercionHandler> GetPropertyValueHandlers { get; set; }
		Dictionary<object, PropertyUpdaterHandler> UpdaterHandlers { get; set; }
		readonly NullableContainer syncValue = new NullableContainer();
		[IgnoreDependencyPropertiesConsistencyChecker]
		DependencyProperty syncProperty;
		public bool HasSyncValue { get { return syncValue.HasValue; } }
		public object SyncValue { get { return HasSyncValue ? syncValue.Value : Editor.EditValue; } }
		public SyncValueUpdateMode SyncValueUpdateMode { get; private set; }
		public DependencyProperty SyncProperty {
			get { return syncProperty; }
			private set { syncProperty = value; }
		}
		public void SetSyncValue(DependencyProperty dp, object value, SyncValueUpdateMode syncValueUpdateMode) {
			if (syncValueUpdateMode == SyncValueUpdateMode.Default) {
				SyncProperty = dp;
				syncValue.SetValue(value);
			}
			else if (syncValueUpdateMode == SyncValueUpdateMode.Update && setSyncValueLocker.IsLocked) {
				SyncProperty = dp;
				syncValue.SetValue(value);
				SyncValueUpdateMode = syncValueUpdateMode;
			}
		}
		public PropertyCoercionHelper(BaseEdit editor) {
			Editor = editor;
			GetBaseValueHandlers = new Dictionary<object, PropertyCoercionHandler>();
			GetPropertyValueHandlers = new Dictionary<object, PropertyCoercionHandler>();
			UpdaterHandlers = new Dictionary<object, PropertyUpdaterHandler>();
		}
		public void Register(DependencyProperty dp, PropertyCoercionHandler getBaseValueHandler, PropertyCoercionHandler getPropertyValueHandler) {
			GetBaseValueHandlers[dp] = getBaseValueHandler;
			GetPropertyValueHandlers[dp] = getPropertyValueHandler;
		}
		public void Register(DependencyProperty dp, PropertyCoercionHandler getBaseValueHandler, PropertyCoercionHandler getPropertyValueHandler, PropertyUpdaterHandler updater) {
			UpdaterHandlers[dp] = updater;
			GetBaseValueHandlers[dp] = getBaseValueHandler;
			GetPropertyValueHandlers[dp] = getPropertyValueHandler;
		}
		public void Register(DependencyPropertyKey dpk, PropertyCoercionHandler getBaseValueHandler, PropertyCoercionHandler getPropertyValueHandler) {
			GetBaseValueHandlers[dpk] = getBaseValueHandler;
			GetPropertyValueHandlers[dpk] = getPropertyValueHandler;
		}
		public void Register(DependencyPropertyKey dpk, PropertyCoercionHandler getBaseValueHandler, PropertyCoercionHandler getPropertyValueHandler, PropertyUpdaterHandler updater) {
			GetBaseValueHandlers[dpk] = getBaseValueHandler;
			GetPropertyValueHandlers[dpk] = getPropertyValueHandler;
			UpdaterHandlers[dpk] = updater;
		}
		void SetValue(object property, object value) {
			PropertyUpdaterHandler handler;
			if (UpdaterHandlers.TryGetValue(property, out handler))
				handler(value);
			else {
				object previousValue = GetPreviousValue(property);
				if (object.Equals(value, previousValue))
					return;
				if (property is DependencyProperty)
					BaseEditHelper.SetCurrentValue(Editor, (DependencyProperty)property, value);
				else if (property is DependencyPropertyKey)
					Editor.SetValue((DependencyPropertyKey)property, value);
			}
		}
		bool ShouldUpdateProperty(object dp) {
			DependencyProperty property = dp as DependencyProperty;
			if (property == null)
				return false;
			return Editor.AllowUpdateTwoWayBoundPropertiesOnSynchronization || !IsTwoWayBound(property);
		}
		bool IsTwoWayBound(DependencyProperty dp) {
			BindingExpression bindingExpression = Editor.GetBindingExpression(dp);
			if (bindingExpression == null)
				return false;
			Binding binding = bindingExpression.ParentBinding;
			if (binding == null)
				return false;
#if !SL
			return binding.Mode == BindingMode.TwoWay || binding.Mode != BindingMode.OneWayToSource;
#else
			return binding.Mode == BindingMode.TwoWay;
#endif
		}
		object GetPreviousValue(object property) {
			DependencyProperty result;
			if (property is DependencyProperty)
				result = (DependencyProperty)property;
			else if (property is DependencyPropertyKey)
				result = ((DependencyPropertyKey)property).DependencyProperty;
			else
				throw new ArgumentException("property");
			return Editor.GetValue(result);
		}
		public void Update(DependencyProperty dp, object baseValue) {
			object cachedSyncValue = dp != null ? GetBaseValueHandlers[dp](baseValue) : baseValue;
			foreach (object property in GetPropertyValueHandlers.Keys) {
				if (dp == property)
					continue;
				SetValue(property, GetPropertyValueHandlers[property](cachedSyncValue));
			}
		}
		public void Update() {
			object cachedSyncValue = SyncProperty != null ? GetBaseValueHandlers[SyncProperty](SyncValue) : SyncValue;
			if (Editor.PropertyProvider.SuppressFeatures) {
				UpdateProperty(SyncProperty, cachedSyncValue);
				return;
			}
			foreach (object property in GetPropertyValueHandlers.Keys)
				UpdateProperty(property, cachedSyncValue);
		}
		public void UpdateProperty(object property, object cachedSyncValue) {
			if (ShouldUpdateProperty(property))
				SetValue(property, GetPropertyValueHandlers[property](cachedSyncValue));
		}
		public void Update(DependencyProperty dp, object oldValue, object newValue) {
			if (dp == null)
				throw new ArgumentException("dp");
			object oldBaseValue = GetBaseValueHandlers[dp](oldValue);
			object newBaseValue = GetBaseValueHandlers[dp](newValue);
			SetSyncValue(dp, newValue, SyncValueUpdateMode.Default);
			if (oldBaseValue == newBaseValue)
				return;
			if (Editor.PropertyProvider.SuppressFeatures)
				return;
			if (EditStrategy.RaiseValueChangingEvents(oldBaseValue, newBaseValue)) {
				Update(dp, newValue);
				RaiseValueChangedEvents(oldBaseValue, newBaseValue);
			}
			else {
				SetSyncValue(null, oldBaseValue, SyncValueUpdateMode.Default);
				Update(null, oldBaseValue);
			}
		}
		void RaiseValueChangedEvents(object oldBaseValue, object newBaseValue) {
			try {
				setSyncValueLocker.Lock();
				EditStrategy.RaiseValueChangedEvents(oldBaseValue, newBaseValue);
			}
			finally {
				setSyncValueLocker.Unlock();
			}
			if (SyncValueUpdateMode == SyncValueUpdateMode.Update)
				Update(SyncProperty, SyncValue);
			SyncValueUpdateMode = SyncValueUpdateMode.Default;
		}
		public void ResetSyncValue() {
			SetSyncValue(null, null, SyncValueUpdateMode.Default);
		}
	}
}
