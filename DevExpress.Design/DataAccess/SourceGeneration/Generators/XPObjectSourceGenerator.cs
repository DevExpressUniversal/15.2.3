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
	public static class XPODataSourceHelper {
		public static System.Type GetXPOType(string xpoTypeName) {
			return XPODataTypeInfo.GetXPOType(xpoTypeName);
		}
		public static System.Type GetXPOSessionType() {
			return GetXPOType("UnitOfWork");
		}
		public static System.Type GetXPOViewPropertyType() {
			return GetXPOType("ViewProperty");
		}
	}
	abstract class XPObjectSourceGenerator : DataSourceGeneratorBase {
		protected override IModelItem CreateDataSourceItem(IDataSourceGeneratorContext context) {
			return context.CreateDataSource(GetDataSourceObjectType());
		}
		protected override void InitializeDataSourceItem(IModelItem xpoSourceItem, IDataSourceGeneratorContext context) {
			SetProperty(xpoSourceItem, "ObjectType", GetElementType(context));
			InitializeDisplayableProperties(xpoSourceItem, context);
		}
		protected sealed override void SetDataSource(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			context.ClearDataMember();
			context.SetDataSource(dataSourceItem);
		}
		protected virtual System.Type GetDataSourceObjectType() {
			return XPODataTypeInfo.GetXPOType(GetDataSourceObjectTypeName());
		}
		protected abstract string GetDataSourceObjectTypeName();
		protected virtual void InitializeDisplayableProperties(IModelItem xpoSourceItem, IDataSourceGeneratorContext context) {
			var displayableProperties = string.Join(";", context.SettingsModel.Fields);
			if(!string.IsNullOrEmpty(displayableProperties))
				SetProperty(xpoSourceItem, "DisplayableProperties", displayableProperties);
		}
	}
	abstract class XPObjectSourceWithSessionGenerator : XPObjectSourceGenerator {
		IModelItem sessionItem;
		protected sealed override IModelItem CreateDataSourceItem(IDataSourceGeneratorContext context) {
			sessionItem = context.CreateDataSource(XPODataSourceHelper.GetXPOSessionType());
			return base.CreateDataSourceItem(context);
		}
		protected override void InitializeDataSourceItem(IModelItem xpoSourceItem, IDataSourceGeneratorContext context) {
			SetProperty(xpoSourceItem, "Session", sessionItem.Value);
			base.InitializeDataSourceItem(xpoSourceItem, context);
		}
	}
	class XPCollectionSourceGenerator : XPObjectSourceWithSessionGenerator {
		protected override string GetDataSourceObjectTypeName() {
			return "XPCollection";
		}
	}
	class XPViewSourceGenerator : XPObjectSourceWithSessionGenerator {
		protected override string GetDataSourceObjectTypeName() {
			return "XPView";
		}
		protected override void InitializeDataSourceItem(IModelItem xpoSourceItem, IDataSourceGeneratorContext context) {
			base.InitializeDataSourceItem(xpoSourceItem, context);
			var settingsModel = context.SettingsModel as IXPViewSourceSettingsModel;
			var properties = System.ComponentModel.TypeDescriptor.GetProperties(GetElementType(context));
			foreach(ComponentModel.PropertySortDescription sortDescription in settingsModel.SortDescriptions) {
				var property = CreateSortProperty(context, sortDescription, properties);
				xpoSourceItem.Properties["Sorting"].Collection.Add(property);
			}
		}
		protected override void InitializeDisplayableProperties(IModelItem xpoSourceItem, IDataSourceGeneratorContext context) {
			base.InitializeDisplayableProperties(xpoSourceItem, context);
			var properties = System.ComponentModel.TypeDescriptor.GetProperties(GetElementType(context));
			foreach(string field in context.SettingsModel.Fields) {
				var property = CreateViewProperty(context, properties[field]);
				xpoSourceItem.Properties["Properties"].Collection.Add(property);
			}
		}
		static IModelItem CreateViewProperty(IDataSourceGeneratorContext context, System.ComponentModel.PropertyDescriptor pd) {
			return SetNameAndCriteria(pd, context.CreateDataSource(XPODataSourceHelper.GetXPOViewPropertyType()));
		}
		static IModelItem CreateSortProperty(IDataSourceGeneratorContext context, ComponentModel.PropertySortDescription sd, System.ComponentModel.PropertyDescriptorCollection properties) {
			var property = context.CreateDataSource(typeof(DevExpress.Xpo.SortProperty));
			SetNameAndCriteria(properties[sd.PropertyName], property, "PropertyName");
			var directionValue = System.Enum.ToObject(typeof(DevExpress.Xpo.DB.SortingDirection), (int)sd.Direction);
			property.Properties["Direction"].SetValue(directionValue);
			return property;
		}
		static IModelItem SetNameAndCriteria(System.ComponentModel.PropertyDescriptor pd, IModelItem property, string nameProperty = "Name") {
			var attributes = new DevExpress.Data.Utils.AnnotationAttributes(pd);
			property.Properties[nameProperty].SetValue(attributes.HasDisplayAttribute ? attributes.Name : pd.DisplayName);
			property.Properties["Property"].SetValue(Data.Filtering.CriteriaOperator.Parse("[" + pd.Name + "]"));
			return property;
		}
	}
}
