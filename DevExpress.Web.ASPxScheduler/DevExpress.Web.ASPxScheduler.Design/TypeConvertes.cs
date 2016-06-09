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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Web.UI.Design;
using DevExpress.Web;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.Web.ASPxScheduler.Design {
	public class ASPxSchedulerIDConverter : ComponentIDConverter {
		protected override bool IsValidComponentType(IComponent component) {
			return component is ASPxScheduler;
		}
	}
	public class SchedulerDesignerProvider<T> where T : IPersistentObject {
		IServiceProvider serviceProvider;
		public SchedulerDesignerProvider(IServiceProvider serviceProvider) {
			this.serviceProvider = serviceProvider;
		}
		public ASPxSchedulerDesigner GetSchedulerDesigner(IPersistentObjectStorageProvider<T> storageProvider) {
			IPersistentObjectStorage<T> objectStorage = storageProvider.ObjectStorage;
			if (objectStorage == null)
				return null;
			return GetSchedulerDesigner(objectStorage);
		}
		public ASPxSchedulerDesigner GetSchedulerDesigner(IPersistentObjectStorage<T> objectStorage) {
			ASPxScheduler scheduler = GetScheduler(objectStorage);
			if (scheduler == null)
				return null;
			IDesignerHost host = (IDesignerHost)serviceProvider.GetService(typeof(IDesignerHost));
			if (host == null) {
				ISite site = scheduler.Site;
				if (site != null) {
					host = (IDesignerHost)site.GetService(typeof(IDesignerHost));
					if (host == null)
						return null;
				}
			}
			return host.GetDesigner(scheduler) as ASPxSchedulerDesigner;
		}
		public ASPxScheduler GetScheduler(IPersistentObjectStorage<T> objectStorage) {
			ASPxSchedulerStorage storage = objectStorage.Storage as ASPxSchedulerStorage;
			if (storage == null)
				return null;
			return storage.Control;
		}
	}
	public abstract class ASPxSchedulerDataSourceViewSchemaConverter<T> : DataSourceViewSchemaConverter where T : IPersistentObject {
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context, Type typeFilter) {
			if (context != null) {
				ITypeDescriptorContext customContext = SubstituteContext(context);
				return base.GetStandardValues(customContext, typeFilter);
			}
			else
				return base.GetStandardValues(context, typeFilter);
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			if (context != null) {
				ITypeDescriptorContext customContext = SubstituteContext(context);
				return base.GetStandardValuesSupported(customContext);
			}
			else
				return base.GetStandardValuesSupported(context);
		}
		protected internal virtual ASPxSchedulerDesigner GetSchedulerDesigner(ITypeDescriptorContext context) {
			IPersistentObjectStorageProvider<T> objectStorageProvider = context.Instance as IPersistentObjectStorageProvider<T>;
			if (objectStorageProvider == null)
				return null;
			SchedulerDesignerProvider<T> provider = new SchedulerDesignerProvider<T>(context);
			return provider.GetSchedulerDesigner(objectStorageProvider);
		}
		protected internal abstract ITypeDescriptorContext SubstituteContext(ITypeDescriptorContext originalContext);
	}
	public class TypeDescriptorContextWrapper : ITypeDescriptorContext {
		#region Fields
		ITypeDescriptorContext originalContext;
		object newInstance;
		#endregion
		public TypeDescriptorContextWrapper(ITypeDescriptorContext originalContext, object newInstance) {
			if (originalContext == null)
				Exceptions.ThrowArgumentNullException("originalContext");
			if (newInstance == null)
				Exceptions.ThrowArgumentNullException("newInstance");
			this.originalContext = originalContext;
			this.newInstance = newInstance;
		}
		#region ITypeDescriptorContext Members
		public IContainer Container { get { return originalContext.Container; } }
		public PropertyDescriptor PropertyDescriptor { get { return originalContext.PropertyDescriptor; } }
		public object Instance { get { return newInstance; } }
		public void OnComponentChanged() {
			originalContext.OnComponentChanged();
		}
		public bool OnComponentChanging() {
			return originalContext.OnComponentChanging();
		}
		#endregion
		#region IServiceProvider Members
		public object GetService(Type serviceType) {
			return originalContext.GetService(serviceType);
		}
		#endregion
	}
	public class ASPxSchedulerAppointmentDataSourceViewSchemaConverter : ASPxSchedulerDataSourceViewSchemaConverter<Appointment> {
		protected internal override ITypeDescriptorContext SubstituteContext(ITypeDescriptorContext originalContext) {
			ASPxSchedulerDesigner designer = GetSchedulerDesigner(originalContext);
			if (designer == null)
				return originalContext;
			return new TypeDescriptorContextWrapper(originalContext, designer.AppointmentDataSourceViewSchemaAccessor);
		}
	}
	public class ASPxSchedulerResourceDataSourceViewSchemaConverter : ASPxSchedulerDataSourceViewSchemaConverter<Resource> {
		protected internal override ITypeDescriptorContext SubstituteContext(ITypeDescriptorContext originalContext) {
			ASPxSchedulerDesigner designer = GetSchedulerDesigner(originalContext);
			if (designer == null)
				return originalContext;
			return new TypeDescriptorContextWrapper(originalContext, designer.ResourceDataSourceViewSchemaAccessor);
		}
	}
}
