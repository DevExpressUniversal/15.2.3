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
using DevExpress.Xpf.PropertyGrid;
using DevExpress.Mvvm.Native;
using System.ComponentModel;
using System.Collections;
using DevExpress.XtraVerticalGrid.Data;
using System.Linq;
namespace DevExpress.Xpf.PropertyGrid.Internal {
	public class ImmutableDescriptorContext : PropertyDescriptorContext {
		public ImmutableDescriptorContext(object instance, PropertyDescriptor descriptor, IServiceProvider serviceProvider, bool isMultiSource)
			: base(instance, descriptor, serviceProvider, isMultiSource) {
		}
		protected override bool GetShouldSerializeValue() {
			if (ParentContext == null)
				return true;
			return ParentContext.ShouldSerializeValue;
		}
		protected override bool IsPropertyReadOnly { get { return ShouldRenderReadOnly; } }
		public override bool ShouldRenderReadOnly {
			get { return ParentContext == null ? true : ParentContext.ShouldRenderReadOnly; }
		}
		internal protected override RowHandle SetValueInternal(object value) {
			if (IsMultiSource) {
				((MultiObjectPropertyDescriptor)ParentContext.PropertyDescriptor).SetValues((object[])ParentContext.MultiInstance, CreateInstances(value));
				return ParentContext.RowHandle;
			}
			else {
				return ParentContext.SetValueInternal(CreateInstance(value, Name, ParentContext, ParentContext.Value, DataController.BrowsableAttributes));
			}
		}
		object[] CreateInstances(object settingValue) {
			return ((object[])MultiInstance).Select(parentValue => {
				return CreateInstance(settingValue, Name, ParentContext, parentValue, DataController.BrowsableAttributes);
			}).ToArray();
		}
		public override RowHandle ResetValue() {
			return ParentContext.ResetValue();
		}
		public override bool CanResetValue() {
			return ParentContext.CanResetValue();
		}
		public static object CreateInstance(object newValue, string name, DescriptorContext parentContext, object parentValue, Attribute[] browsableAttributes) {
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(parentValue, browsableAttributes);
			Hashtable currentValues = GetSameLevelValues(properties, parentValue);
			currentValues[name] = newValue;
			if (parentContext.Converter.GetCreateInstanceSupported(parentContext))
				return parentContext.Converter.CreateInstance(parentContext, currentValues);
			else {
				return Activator.CreateInstance(parentContext.PropertyType, GetValues(currentValues, properties).ToArray());
			}
		}
		static IEnumerable<object> GetValues(Hashtable values, PropertyDescriptorCollection properties) {
			return properties.Cast<PropertyDescriptor>().Select(p => values[p.Name]);
		}
		static Hashtable GetSameLevelValues(PropertyDescriptorCollection properties, object value) {
			Hashtable values = new Hashtable();
			properties.Cast<PropertyDescriptor>().ForEach(descriptor => values[descriptor.Name] = PropertyHelper.GetValue(descriptor, value));
			return values;
		}
	}
}
