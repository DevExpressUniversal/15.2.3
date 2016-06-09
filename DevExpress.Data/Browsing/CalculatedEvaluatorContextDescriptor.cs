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

using DevExpress.Data.Filtering.Helpers;
using DevExpress.XtraReports.UI;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Parameters;
using DevExpress.Utils;
using DevExpress.Data.Native;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.Data.Browsing {
	public class XREvaluatorContextDescriptor : EvaluatorContextDescriptor {
		IEnumerable<IParameter> parameters;
		DataContext dataContext;
		object dataSource;
		string dataMember;
		Dictionary<string, PropertyDescriptor> propertyCollection = new Dictionary<string, PropertyDescriptor>();
		Dictionary<string, EvaluatorContextDescriptor> contextDescriptorCollection = new Dictionary<string, EvaluatorContextDescriptor>();
		public override bool IsTopLevelCollectionSource {
			get { return true; }
		}
		public XREvaluatorContextDescriptor(IEnumerable<IParameter> parameters, DataContext dataContext, object dataSource, string dataMember) {
			this.parameters = parameters;
			this.dataContext = dataContext;
			this.dataSource = dataSource;
			this.dataMember = dataMember;
		}
		public override object GetPropertyValue(object source, EvaluatorProperty propertyPath) {
			object result = GetPropertyValue(source, propertyPath.PropertyPath);
			if(propertyPath.SubProperty == null)
				return result;
			if(result == null && parameters != null) {
				IParameter parameter = parameters.GetByName(propertyPath.SubProperty.PropertyPath);
				if(parameter != null)
					result = parameter.Value;
			}
			if(result == null) {
				EvaluatorContext nestedContext = this.GetNestedContext(source, propertyPath.PropertyPathTokenized[0]);
				if(nestedContext != null)
					result = nestedContext.GetPropertyValue(propertyPath.SubProperty);
			}
			if(result == null)
				result = GetValueByName(propertyPath.SubProperty.PropertyPath);
			return result;
		}
		object GetPropertyValue(object source, string propertyName) {
			PropertyDescriptor propertyDescriptor = null;
			if(string.IsNullOrEmpty(propertyName))
				return dataContext.GetDataBrowser(dataSource, dataMember, false).DataSource;
			if(!propertyCollection.TryGetValue(propertyName, out propertyDescriptor)) {
				PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(source);
				propertyDescriptor = properties.Find(propertyName, false);
				if(propertyDescriptor == null && dataContext != null) {
					DataBrowser dataBrowser = dataContext.GetDataBrowser(dataSource, dataMember, false);
					if(dataBrowser != null)
						propertyDescriptor = dataBrowser.FindItemProperty(propertyName, true);
				}
				propertyCollection.Add(propertyName, propertyDescriptor);
			}
			return propertyDescriptor != null ? propertyDescriptor.GetValue(source) : null;
		}
		object GetValueByName(string name) {
			DataBrowser browser = dataContext.GetDataBrowser(dataSource, dataMember, false);
			try {
				if(name == "RowCount")
					return browser == null ? 0 : browser.Count;
				if(name == "CurrentRowIndex")
					return browser == null ? -1 : browser.Position;
			} catch {
			}
			return null;
		}
		public override EvaluatorContext GetNestedContext(object source, string propertyName) {
			object nestedSource = GetPropertyValue(source, propertyName);
			if(nestedSource == null)
				return null;
			nestedSource = GetFirstItemForList(nestedSource);
			return new EvaluatorContext(GetContextDescriptor(propertyName), nestedSource);
		}
		public override IEnumerable GetCollectionContexts(object source, string collectionName) {
			object collectionSrc = GetPropertyValue(source, collectionName);
			if(collectionSrc == null)
				return null;
			IList list = GetList(collectionSrc);
			if(list == null)
				return null;
			return new CollectionContexts(GetContextDescriptor(collectionName), list);
		}
		EvaluatorContextDescriptor GetContextDescriptor(string propertyName) {
			EvaluatorContextDescriptor descriptor = null;
			propertyName = propertyName ?? string.Empty;
			if(!this.contextDescriptorCollection.TryGetValue(propertyName, out descriptor)) {
				descriptor = new XREvaluatorContextDescriptor(parameters, dataContext, dataSource, BindingHelper.JoinStrings(".", dataMember, propertyName));
				contextDescriptorCollection.Add(propertyName, descriptor);
			}
			return descriptor;
		}
		static object GetFirstItemForList(object nestedSource) {
			IList list = GetList(nestedSource);
			if(list != null && list.Count > 0)
				return list[0];
			return nestedSource;
		}
		static IList GetList(object nestedSource) {
			IList list = nestedSource as IList;
			if(list == null) {
				IListSource listSource = nestedSource as IListSource;
				if(listSource != null)
					list = listSource.GetList();
			}
			return list;
		}
	}
	public class CalculatedEvaluatorContextDescriptor : XREvaluatorContextDescriptor {
		FieldType propertyType;
		public CalculatedEvaluatorContextDescriptor(IEnumerable<IParameter> parameters, ICalculatedField calculatedField, DataContext dataContext)
			: base(parameters, dataContext, calculatedField.DataSource, calculatedField.DataMember) {
			Guard.ArgumentNotNull(calculatedField, "calculatedField");
			this.propertyType = calculatedField.FieldType;
		}
		public override object GetPropertyValue(object source, EvaluatorProperty propertyPath) {
			object result = base.GetPropertyValue(source, propertyPath);
			return result is int ? GetCastedResult((int)result) : result;
		}
		object GetCastedResult(int result) {
			if(propertyType == FieldType.Double)
				return (double)result;
			else if(propertyType == FieldType.Float)
				return (float)result;
			return result;
		}
	}
}
