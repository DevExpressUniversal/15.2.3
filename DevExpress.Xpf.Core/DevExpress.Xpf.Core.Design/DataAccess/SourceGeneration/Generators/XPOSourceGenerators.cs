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

namespace DevExpress.Design.DataAccess {
	using System;
	using DevExpress.Xpf.Core.Design.CoreUtils;
	using Microsoft.Windows.Design.Model;
	abstract class XPObjectSourceGenerator : BaseXamlDataSourceGenerator {
		protected override void InitializeDataSourceItem(IModelItem xpoSourceItem, IDataSourceGeneratorContext context) {
			SetProperty(xpoSourceItem, DataSourcePropertyCodeName.ObjectType, GetElementType(context));
			InitializeDisplayableProperties(xpoSourceItem, context);
		}
		protected virtual void InitializeDisplayableProperties(IModelItem xpoSourceItem, IDataSourceGeneratorContext context) {
			var displayableProperties = string.Join(";", context.SettingsModel.Fields);
			if(!string.IsNullOrEmpty(displayableProperties))
				SetProperty(xpoSourceItem, "DisplayableProperties", displayableProperties);
		}
		protected static IModelItem CreateModeltem(IModelItem xpoSourceItem, DevExpress.Xpf.Core.Design.DXTypeInfo typeInfo) {
			var modelItem = ModelFactory.CreateItem(((ModelItem)xpoSourceItem.Value).Context, typeInfo.ResolveType());
			return xpoSourceItem.EditingContext.ServiceProvider.GetService<IModelService>().CreateModelItem(modelItem) as IModelItem;
		}
	}
	abstract class XPObjectSourceWithSessionGenerator : XPObjectSourceGenerator {
		protected override void InitializeDataSourceItem(IModelItem xpoSourceItem, IDataSourceGeneratorContext context) {
			SetProperty(xpoSourceItem, "Session", Activator.CreateInstance(XPODataSourceHelper.GetXPOSessionType()));
			base.InitializeDataSourceItem(xpoSourceItem, context);
		}
	}
	class XPCollectionSourceGenerator : XPObjectSourceWithSessionGenerator {
		protected override Type GetDataSourceType() {
			return RuntimeTypes.XPCollectionDataSource.ResolveType();
		}
	}
	class XPViewSourceGenerator : XPObjectSourceWithSessionGenerator {
		protected override Type GetDataSourceType() {
			return RuntimeTypes.XPViewDataSource.ResolveType();
		}
		protected override void InitializeDataSourceItem(IModelItem xpoSourceItem, IDataSourceGeneratorContext context) {
			base.InitializeDataSourceItem(xpoSourceItem, context);
			var settingsModel = context.SettingsModel as IXPViewSourceSettingsModel;
			var properties = System.ComponentModel.TypeDescriptor.GetProperties(GetElementType(context));
			foreach(ComponentModel.PropertySortDescription sortDescription in settingsModel.SortDescriptions) {
				var property = CreateSortProperty(xpoSourceItem, sortDescription, properties);
				xpoSourceItem.Properties["Sorting"].Collection.Add(property);
			}
		}
		protected override void InitializeDisplayableProperties(IModelItem xpoSourceItem, IDataSourceGeneratorContext context) {
			var properties = System.ComponentModel.TypeDescriptor.GetProperties(GetElementType(context));
			foreach(string field in context.SettingsModel.Fields) {
				var property = CreateViewProperty(xpoSourceItem, properties[field]);
				xpoSourceItem.Properties["Properties"].Collection.Add(property);
			}
		}
		static IModelItem CreateViewProperty(IModelItem xpoSourceItem, System.ComponentModel.PropertyDescriptor pd) {
			return SetNameAndCriteria(pd, CreateModeltem(xpoSourceItem, RuntimeTypes.XPViewDataSourceProperty));
		}
		static IModelItem CreateSortProperty(IModelItem xpoSourceItem, ComponentModel.PropertySortDescription sd, System.ComponentModel.PropertyDescriptorCollection properties) {
			var property = CreateModeltem(xpoSourceItem, RuntimeTypes.XPViewDataSourceSortProperty);
			SetNameAndCriteria(properties[sd.PropertyName], property, "PropertyName");
			var directionValue = System.Enum.ToObject(typeof(DevExpress.Xpo.DB.SortingDirection), (int)sd.Direction);
			property.Properties["Direction"].SetValue(directionValue);
			return property;
		}
		static IModelItem SetNameAndCriteria(System.ComponentModel.PropertyDescriptor pd, IModelItem property, string nameProperty = "Name") {
			var attributes = new DevExpress.Data.Utils.AnnotationAttributes(pd);
			string propName = attributes.HasDisplayAttribute ? attributes.Name : pd.DisplayName;
			property.Properties[nameProperty].SetValue(propName);
			if(pd.Name != propName)
				property.Properties["Property"].SetValue(Data.Filtering.CriteriaOperator.Parse("[" + pd.Name + "]"));
			return property;
		}
	}
}
