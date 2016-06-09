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
using System.ComponentModel;
using System.Windows.Controls;
using DevExpress.Data.Access;
using DevExpress.Xpf.Core.Internal;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Utils;
using System.Dynamic;
#if !SL
using System.Windows.Interop;
using System.Windows.Forms;
#else
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using DevExpress.Data.Browsing;
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
#endif
namespace DevExpress.Xpf.Editors.Native {
	public enum LookUpPropertyDescriptorType { Value, Display }
	public abstract class LookUpPropertyDescriptorBase : PropertyDescriptor {
		public static readonly object UnsetValue = new object();
		public static bool IsUnsetValue(object value) {
			return UnsetValue == value;
		}
		public static LookUpPropertyDescriptorBase CreatePropertyDescriptor(Type componentType, LookUpPropertyDescriptorType descriptorType, string path, string internalPath = null) {
			System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(path), "path");
			if (typeof(ICustomItem).IsAssignableFrom(componentType))
				return new LookUpCustomItemPropertyDescriptor(descriptorType, path);
			if (typeof(ListBoxItem).IsAssignableFrom(componentType))
				return new LookUpListBoxItemPropertyDescriptor(descriptorType, path);
			return string.IsNullOrEmpty(internalPath)
				? (LookUpPropertyDescriptorBase)new LookUpGetItemPropertyDescriptor(descriptorType, path, internalPath)
				: new LookUpGetPropertyPropertyDescriptor(descriptorType, path, internalPath);
		}
		protected string Path { get; private set; }
		protected string InternalPath { get; private set; }
		protected LookUpPropertyDescriptorType DescriptorType { get; private set; }
		protected LookUpPropertyDescriptorBase(LookUpPropertyDescriptorType descriptorType, string path, string internalPath)
			: base(path, null) {
			Path = path;
			InternalPath = internalPath;
			DescriptorType = descriptorType;
		}
		public override bool CanResetValue(object component) {
			return false;
		}
		public override string DisplayName { get { return Path; } }
		public override System.Type ComponentType { get { return typeof(object); } }
		public override object GetValue(object component) {
			return GetValueImpl(component);
		}
		public override string Name { get { return Path; } }
		protected abstract object GetValueImpl(object component);
		protected abstract void SetValueImpl(object component, object value);
		public override bool IsReadOnly { get { return false; } }
		public override System.Type PropertyType { get { return typeof(object); } }
		public override void ResetValue(object component) {
		}
		public override void SetValue(object component, object value) {
			SetValueImpl(component, value);
		}
		public override bool ShouldSerializeValue(object component) {
			return false;
		}
		public virtual bool IsRelevant(string internalPath) {
			if (internalPath == null || InternalPath == null)
				return false;
			return internalPath == InternalPath;
		}
		public virtual void Reset() {
		}
	}
	public class LookUpPropertyDescriptor : LookUpPropertyDescriptorBase {
		static readonly object Wrapped = new object();
		static object GetWrapped(object component) {
			return component ?? Wrapped;
		}
		readonly Dictionary<Type, LookUpPropertyDescriptorBase> descriptorsCache = new Dictionary<Type, LookUpPropertyDescriptorBase>();
		public LookUpPropertyDescriptor(LookUpPropertyDescriptorType descriptorType, string path, string internalPath)
			: base(descriptorType, path, internalPath) {
		}
		protected override object GetValueImpl(object component) {
			return GetDescriptor(component).GetValue(component);
		}
		PropertyDescriptor GetDescriptor(object component) {
			Type componentType = GetWrapped(component).GetType();
			LookUpPropertyDescriptorBase descriptor;
			descriptorsCache.TryGetValue(componentType, out descriptor);
			if (descriptor == null || !descriptor.IsRelevant(InternalPath)) {
				descriptor = CreatePropertyDescriptor(componentType, DescriptorType, Path, InternalPath);
				descriptorsCache[componentType] = descriptor;
			}
			return descriptor;
		}
		protected override void SetValueImpl(object component, object value) {
			GetDescriptor(component).SetValue(component, value);
		}
		public override void Reset() {
			if (descriptorsCache != null)
				descriptorsCache.Clear();
		}
	}
	public class LookUpGetPropertyPropertyDescriptor : LookUpPropertyDescriptorBase {
		static bool IsComplexColumn(string member) {
			return !string.IsNullOrEmpty(member) && member.Contains(".");
		}
		bool ShouldCreateFastPropertyDescriptor {
			get {
#if SL
				return !DesignerProperties.IsInDesignTool;
#else
				return !BrowserInteropHelper.IsBrowserHosted;
#endif
			}
		}
		PropertyDescriptor BaseDescriptor { get; set; }
		PropertyDescriptor CreateBaseDescriptor(object component) {
			System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(InternalPath), "InternalPath");
			if (IsComplexColumn(InternalPath))
				return new ComplexPropertyDescriptorReflection(component, InternalPath);
			PropertyDescriptor descriptor = CreatePropertyAccessDescriptor(component);
			return descriptor ?? new LookUpGetItemPropertyDescriptor(DescriptorType, Path, InternalPath);
		}
		PropertyDescriptor CreatePropertyAccessDescriptor(object component) {
			if (component is DynamicObject)
				return new DynamicObjectPropertyDescriptor(InternalPath);
			if (component is IDictionary<string, object>)
				return new ExpandoPropertyDescriptor(null, InternalPath, null);
			PropertyDescriptorCollection properties = ListBindingHelper.GetListItemProperties(component);
			PropertyDescriptor descriptor = properties[InternalPath];
			if (descriptor == null) {
				properties = TypeDescriptor.GetProperties(component);
				descriptor = properties[InternalPath];
			}
			if (descriptor != null)
				return ShouldCreateFastPropertyDescriptor ? CreateFastPropertyDescriptor(descriptor) : descriptor;
			return null;
		}
		protected virtual PropertyDescriptor CreateFastPropertyDescriptor(PropertyDescriptor descriptor) {
			return DataListDescriptor.GetFastProperty(descriptor);
		}
		public LookUpGetPropertyPropertyDescriptor(LookUpPropertyDescriptorType descriptorType, string path, string internalPath)
			: base(descriptorType, path, internalPath) {
		}
		protected override object GetValueImpl(object component) {
			if (BaseDescriptor == null)
				BaseDescriptor = CreateBaseDescriptor(component);
			return BaseDescriptor.GetValue(component);
		}
		public override bool IsRelevant(string internalPath) {
			if (BaseDescriptor != null && BaseDescriptor is LookUpPropertyDescriptorBase)
				return ((LookUpPropertyDescriptorBase)BaseDescriptor).IsRelevant(InternalPath);
			return base.IsRelevant(internalPath);
		}
		protected override void SetValueImpl(object component, object value) {
			if (BaseDescriptor == null)
				BaseDescriptor = CreateBaseDescriptor(component);
			BaseDescriptor.SetValue(component, value);
		}
	}
	public class LookUpGetItemPropertyDescriptor : LookUpPropertyDescriptorBase {
		static readonly ReflectionHelper Helper = new ReflectionHelper();
		public LookUpGetItemPropertyDescriptor(LookUpPropertyDescriptorType descriptorType, string path, string internalPath)
			: base(descriptorType, path, internalPath) {
		}
		protected override object GetValueImpl(object component) {
			return !string.IsNullOrEmpty(InternalPath) ? UnsetValue : component;
		}
		protected override void SetValueImpl(object component, object value) {
			Helper.SetPropertyValue(component, Path, value);
		}
		public override bool IsRelevant(string internalPath) {
			return true;
		}
	}
	public class LookUpListBoxItemPropertyDescriptor : LookUpPropertyDescriptorBase {
		public LookUpListBoxItemPropertyDescriptor(LookUpPropertyDescriptorType descriptorType, string path)
			: base(descriptorType, path, null) {
		}
		protected override object GetValueImpl(object component) {
			var item = (ListBoxItem)component;
			return item.Content;
		}
		protected override void SetValueImpl(object component, object value) {
			var item = (ListBoxItem)component;
			item.Content = value;
		}
		public override bool IsRelevant(string internalPath) {
			return true;
		}
	}
	public class LookUpCustomItemPropertyDescriptor : LookUpPropertyDescriptorBase {
		public LookUpCustomItemPropertyDescriptor(LookUpPropertyDescriptorType descriptorType, string path)
			: base(descriptorType, path, null) {
		}
		protected override object GetValueImpl(object component) {
			var item = (ICustomItem)component;
			if (DescriptorType == LookUpPropertyDescriptorType.Display)
				return item.DisplayValue;
			if (DescriptorType == LookUpPropertyDescriptorType.Value)
				return item.EditValue;
			throw new ArgumentException("Path");
		}
		protected override void SetValueImpl(object component, object value) {
			var item = (ICustomItem)component;
			if (DescriptorType == LookUpPropertyDescriptorType.Display)
				item.DisplayValue = value;
			if (DescriptorType == LookUpPropertyDescriptorType.Value)
				item.EditValue = value;
			throw new ArgumentException("Path");
		}
		public override bool IsRelevant(string internalPath) {
			return true;
		}
	}
	public class DynamicObjectMemberBinder : GetMemberBinder {
		public DynamicObjectMemberBinder(string name) : base(name, true) { }
		public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion) {
			return null;
		}
	}
	public class DynamicObjectPropertyDescriptor : PropertyDescriptor {
		GetMemberBinder GetMemberBinder { get; set; }
		public DynamicObjectPropertyDescriptor(string path)
			: base(path, null) {
			GetMemberBinder = new DynamicObjectMemberBinder(path);
		}
		public override object GetValue(object component) {
			DynamicObject obj = component as DynamicObject;
			object value;
			if (obj.TryGetMember(GetMemberBinder, out value))
				return value;
			return null;
		}
		public override System.Type ComponentType { get { return typeof(object); } }
		public override bool IsReadOnly { get { return false; } }
		public override System.Type PropertyType { get { return typeof(object); } }
		public override void ResetValue(object component) {
		}
		public override void SetValue(object component, object value) {
		}
		public override bool ShouldSerializeValue(object component) {
			return false;
		}
		public override bool CanResetValue(object component) {
			return false;
		}
	}
	public class GetStringFromLookUpValuePropertyDescriptor : LookUpPropertyDescriptorBase {
		public GetStringFromLookUpValuePropertyDescriptor(PropertyDescriptor propertyDescriptor)
			: base(LookUpPropertyDescriptorType.Display, propertyDescriptor.Name, propertyDescriptor.Name) {
			Guard.ArgumentNotNull(propertyDescriptor, "PropertyDescriptor");
			PropertyDescriptor = propertyDescriptor;
		}
		public PropertyDescriptor PropertyDescriptor { get; private set; }
		protected override object GetValueImpl(object component) {
			object value = PropertyDescriptor.GetValue(component);
			if (IsUnsetValue(value))
				value = null;
			return value == null ? null : value.ToString();
		}
		protected override void SetValueImpl(object component, object value) {
		}
	}
	public class EditorsDataControllerWrappedDescriptor : LookUpPropertyDescriptorBase {
		LookUpPropertyDescriptorBase BaseDescriptor { get; set; }
		public EditorsDataControllerWrappedDescriptor(LookUpPropertyDescriptorType descriptorType, string path, string internalPath) : base(descriptorType, path, internalPath) {
			BaseDescriptor = new LookUpPropertyDescriptor(descriptorType, path, internalPath);
		}
		protected override object GetValueImpl(object component) {
			var result = BaseDescriptor.GetValue(component);
			return IsUnsetValue(result) ? null : result;
		}
		protected override void SetValueImpl(object component, object value) {
			BaseDescriptor.SetValue(component, value);
		}
	}
}
