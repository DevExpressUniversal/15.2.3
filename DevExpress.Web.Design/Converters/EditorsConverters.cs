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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.Design.WebControls;
using DevExpress.Utils.About;
using DevExpress.Utils.Design;
using DevExpress.Web.Design;
namespace DevExpress.Web.Design {
	public class TypeDescriptorContextWrapper : ITypeDescriptorContext {
		private ITypeDescriptorContext typeDescriptorContext;
		private object instance;
		public TypeDescriptorContextWrapper(ITypeDescriptorContext typeDescriptorContext, object instance) {
			this.typeDescriptorContext = typeDescriptorContext;
			this.instance = instance;
		}
		object ITypeDescriptorContext.Instance {
			get { return instance; }
		}
		IContainer ITypeDescriptorContext.Container { get { return typeDescriptorContext.Container; } }
		void ITypeDescriptorContext.OnComponentChanged() { typeDescriptorContext.OnComponentChanged(); }
		bool ITypeDescriptorContext.OnComponentChanging() { return typeDescriptorContext.OnComponentChanging(); }
		PropertyDescriptor ITypeDescriptorContext.PropertyDescriptor { get { return typeDescriptorContext.PropertyDescriptor; } }
		object IServiceProvider.GetService(Type serviceType) { return typeDescriptorContext.GetService(serviceType); }
	}
	public class EditPropertiesDataSourceIDConverter : DataSourceIDConverter {
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
			string textValue = value as string;
			if(textValue != null && textValue == SystemDesignSRHelper.GetDataControlNoDataSource())
				return string.Empty;
			return base.ConvertFrom(context, culture, value);
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			string textValue = value as string;
			if(string.IsNullOrEmpty(textValue))
				return SystemDesignSRHelper.GetDataControlNoDataSource();
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			ArrayList list = new ArrayList(base.GetStandardValues(context));
			if(context != null) {
				IDesignerHost service = (IDesignerHost)context.GetService(typeof(IDesignerHost));
				if(service != null)
					list = new ArrayList(base.GetStandardValues(new TypeDescriptorContextWrapper(context, service.RootComponent)));
			}
			if(list.Count > 0)
				list.RemoveAt(list.Count - 1);
			return new StandardValuesCollection(list);
		}
	}
	public sealed class EditPropertiesDataSourceViewSchemaConverter : DataSourceViewSchemaConverterBase {
		public override object GetDataSourceOwner(ITypeDescriptorContext context) {
			return context.Instance;
		}
	}
	public sealed class ListBoxColumnDataSourceViewSchemaConverter : DataSourceViewSchemaConverterBase {		
		public override object GetDataSourceOwner(ITypeDescriptorContext context) {
			return ListBoxDesignerHelper.GetDataSourceOwner(context);
		}
	}
	public abstract class DataSourceViewSchemaConverterBase : DataSourceViewSchemaConverter {
		#region Nested Types
		private class DataSourceViewSchemaAccessor : IDataSourceViewSchemaAccessor {
			private object schema;
			public DataSourceViewSchemaAccessor(object schema) {
				this.schema = schema;
			}
			object IDataSourceViewSchemaAccessor.DataSourceViewSchema {
				get { return schema; }
				set { }
			}
		}
		#endregion
		public abstract object GetDataSourceOwner(ITypeDescriptorContext context);
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			if(context == null) 
				return base.GetStandardValues(context);
			object dataSourceOwner = GetDataSourceOwner(context);
			string dataSourceID = DesignUtils.GetDataSourceID(dataSourceOwner);
			if(string.IsNullOrEmpty(dataSourceID))
				return base.GetStandardValues(context);
			var result = GetStandardValuesFromDataSourceOnPage(context, dataSourceID);
			if(result == null || result.Count == 0)
				result = GetStandardValuesFromDataSourceInTemplate(context, dataSourceOwner as ASPxDataWebControl, dataSourceID);
			return result;
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return context != null;
		}
		StandardValuesCollection GetStandardValuesFromDataSourceOnPage(ITypeDescriptorContext context, string dataSourceID) {
			if(string.IsNullOrEmpty(dataSourceID))
				return base.GetStandardValues(context);
			return GetViewStandardValues(context, DesignUtils.GetDesignerDataSourceView(context, dataSourceID));
		}
		StandardValuesCollection GetStandardValuesFromDataSourceInTemplate(ITypeDescriptorContext context, ASPxDataWebControl dataSource, string dataSourceID) {
			if(dataSource == null)
				return base.GetStandardValues(context);
			var containerControl = dataSource.NamingContainer as Control;
			if(containerControl == null)
				return null;
			foreach(Control control in containerControl.Controls)
				if(control is IDataSource && control.ID == dataSourceID)
					return GetViewStandardValues(context, DesignUtils.GetDesignerDataSourceView(context, control));
			return base.GetStandardValues(context);
		}
		StandardValuesCollection GetViewStandardValues(ITypeDescriptorContext context, DesignerDataSourceView view) {
			var accessor = GetDataSourceViewSchemaAccessor(view);
			if(accessor == null)
				return base.GetStandardValues(context);
			return base.GetStandardValues(new TypeDescriptorContextWrapper(context, accessor));
		}
		DataSourceViewSchemaAccessor GetDataSourceViewSchemaAccessor(DesignerDataSourceView view) { 
			if(view == null)
				return null;
			if(view.Schema == null && view.DataSourceDesigner != null)
				view.DataSourceDesigner.RefreshSchema(true);
			return new DataSourceViewSchemaAccessor(view.Schema);
		}
	}
}
