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
#if !Win
using DevExpress.Xpf.PropertyGrid.Internal;
using DevExpress.Mvvm.Native;
#endif
#if SL
using DevExpress.Data.Browsing;
#else
using System.ComponentModel;
#endif
namespace DevExpress.XtraVerticalGrid.Data {
	public class GetDescriptorContextCommand {
		bool isMultiSource;
		PGridDataModeHelperContextCache contextCache;
		object dataSource;
		IServiceProvider serviceProvider;
		Attribute[] browsableAttributes;
		int initializeLockerCount;
		public void Initialize(bool isMultiSource, PGridDataModeHelperContextCache contextCache, object dataSource, IServiceProvider serviceProvider, Attribute[] browsableAttributes) {
			this.isMultiSource = isMultiSource;
			this.contextCache = contextCache;
			this.dataSource = dataSource;
			this.serviceProvider = serviceProvider;
			this.browsableAttributes = browsableAttributes;
			initializeLockerCount++;
		}
		protected PGridDataModeHelperContextCache ContextCache { get { return contextCache; } }
		protected bool IsMultiSource { get { return isMultiSource; } }
		protected object DataSource { get { return dataSource; } }
		protected IServiceProvider ServiceProvider { get { return serviceProvider; } }
		protected Attribute[] BrowsableAttributes { get { return browsableAttributes; } }
		public bool OnlyBrowsable { get; set; }
		public DescriptorContext Execute(string propertyName) {
			if(this.contextCache == null)
				return CreateCachedDescriptorContext(propertyName, null, null, null);
			return RequestDescriptorContext(propertyName);
		}
		public DescriptorContext Execute(DescriptorContext parentContext, PropertyDescriptor descriptor) {
			string fieldName = FieldNameHelper.GetFieldName(parentContext.GetFieldNameForChild(), descriptor.Name);
			DescriptorContext context = ContextCache[IsMultiSource, fieldName];
			if (context != null)
				return context;
			object components = GetValues(parentContext);
			if (components == null)
				return null;
			if (IsMultiSource) {
				descriptor = GetNextPropertyDescriptor(descriptor.Name, components, parentContext);
			}
			return CreateCachedDescriptorContext(fieldName, components, descriptor, parentContext);
		}
		public void Release() {
			this.initializeLockerCount--;
			if (this.initializeLockerCount == 0)
				ReleaseInternal();
		}
		public void ReleaseInternal() {
			this.isMultiSource = false;
			this.contextCache = null;
			this.dataSource = null;
			this.serviceProvider = null;
			this.browsableAttributes = null;
		}
		DescriptorContext RequestDescriptorContext(string propertyName) {
			DescriptorContext context = ContextCache[IsMultiSource, propertyName];
			if(context != null) return context;
			if(PropertyHelper.IsRoot(propertyName))
				return CreateCachedDescriptorContext(PropertyHelper.RootPropertyName, DataSource, null, null);
			DescriptorContext parentContext = RequestDescriptorContext(FieldNameHelper.GetParentFieldName(propertyName));
			object components = GetValues(parentContext);
			if(components == null)
				return CreateCachedDescriptorContext(propertyName, null, null, null);
			PropertyDescriptor propertyDescriptor = GetNextPropertyDescriptor(FieldNameHelper.GetPropertyName(propertyName), components, parentContext);
			return CreateCachedDescriptorContext(propertyName, components, propertyDescriptor, parentContext);
		}
		object GetValues(DescriptorContext context) {
#if !Win
			return context.GetValues();
#else
			if(context.PropertyDescriptor == null)
				return context.Instance;
			object value = (IsMultiSource) ? ((MultiObjectPropertyDescriptor)context.PropertyDescriptor).GetValues((object[])context.Instance) :
				PropertyHelper.GetValue(context.PropertyDescriptor, context.Instance);
			return value;
#endif
		}
		PropertyDescriptor GetNextPropertyDescriptor(string propertyName, object value, DescriptorContext parentContext) {
			if(IsMultiSource)
				return MultiObjectPropertyDescriptor.GetMultiProp(parentContext, (object[])value, propertyName, BrowsableAttributes, OnlyBrowsable);
			else {
				return parentContext.GetProperty(value, BrowsableAttributes, propertyName, OnlyBrowsable);
			}
		}
		DescriptorContext CreateCachedDescriptorContext(string fieldName, object obj, PropertyDescriptor pd, DescriptorContext parentContext) {
#if !Win
			DescriptorContextFactory factory = ServiceProvider.With(x => (DescriptorContextFactory)x.GetService(typeof(DescriptorContextFactory)));
			DescriptorContext context = factory.Create(obj, pd, fieldName, parentContext, IsMultiSource, ServiceProvider);
			if (context == null)
				return null;
			factory.AddContext(context, IsMultiSource, ContextCache, parentContext);
			context.ContextAdded();
#else
			DescriptorContext context = new DescriptorContext(obj, pd, ServiceProvider);
			context.FieldName = fieldName;
			ContextCache[IsMultiSource, fieldName] = context;
#endif
			return context;
		}
	}
}
