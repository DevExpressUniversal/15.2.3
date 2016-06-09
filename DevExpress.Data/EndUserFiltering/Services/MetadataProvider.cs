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

namespace DevExpress.Utils.Filtering.Internal {
	using System;
	using System.ComponentModel.DataAnnotations;
	using DevExpress.Data.Utils;
	public interface IMetadataProvider {
		string GetCaption(string name);
		string GetDescription(string name);
		string GetLayout(string name);
		int GetOrder(string name);
		string GetShortName(string name);
		bool GetIsVisible(string name);
		string GetDataFormatString(string name);
		DataType? GetDataType(string name);
		Type GetEnumDataType(string name);
		Type GetType(string name);
		Type GetAttributesTypeDefinition(string name);
		IMetricAttributes GetAttributes(string name);
	}
	class MetadataProvider : IMetadataProvider {
		IMetadataStorage metadataStorage;
		public MetadataProvider(IMetadataStorage metadataStorage) {
			this.metadataStorage = metadataStorage;
		}
		public string GetCaption(string name) {
			return GetValueFromAnnotationAttributes(name, AnnotationAttributes.GetName) ?? GetName(name);
		}
		public string GetShortName(string name) {
			return GetValueFromAnnotationAttributes(name, AnnotationAttributes.GetShortName);
		}
		public string GetDescription(string name) {
			return GetValueFromAnnotationAttributes(name, AnnotationAttributes.GetDescription);
		}
		public string GetLayout(string name) {
			return GetValueFromAnnotationAttributes(name, AnnotationAttributes.GetGroupName);
		}
		public bool GetIsVisible(string name) {
			return GetValueFromAnnotationAttributes(name, AnnotationAttributes.GetAutoGenerateColumnOrFilter);
		}
		public int GetOrder(string name) {
			int index = 0;
			return metadataStorage.TryGetValue(name, out index) ? index : GetColumnIndexFromMetadata(name);
		}
		public DataType? GetDataType(string name) {
			return GetValueFromAnnotationAttributes(name, AnnotationAttributes.GetDataType);
		}
		public string GetDataFormatString(string name) {
			return GetValueFromAnnotationAttributes(name, AnnotationAttributes.GetDataFormatString);
		}
		public Type GetType(string name) {
			return GetValueFromFilterAttributes(name, a => a.PropertyType);
		}
		public Type GetEnumDataType(string name) {
			return GetValueFromFilterAttributes(name, a => a.EnumDataType);
		}
		public Type GetAttributesTypeDefinition(string name) {
			return GetValueFromFilterAttributes(name, a => a.GetMetricAttributesTypeDefinition() ?? GetMetricAttributesTypeDefinition(name, a.PropertyType, a.EnumDataType));
		}
		public IMetricAttributes GetAttributes(string name) {
			return GetValueFromFilterAttributes(name, a => a.GetMetricAttributes() ?? CreateMetricAttributes(name, a.PropertyType, a.EnumDataType));
		}
		Type GetMetricAttributesTypeDefinition(string name, Type propertyType, Type enumDataType) {
			return MetricAttributes.GetMetricAttributesTypeDefinition((IMetricAttributesCache)metadataStorage, name, propertyType, enumDataType);
		}
		IMetricAttributes CreateMetricAttributes(string name, Type propertyType, Type enumDataType) {
			return MetricAttributes.CreateLazyMetricAttributes((IMetricAttributesCache)metadataStorage, name, propertyType, enumDataType);
		}
		int GetColumnIndexFromMetadata(string name) {
			return GetValueFromAnnotationAttributes(name, a => AnnotationAttributes.GetColumnIndex(a));
		}
		T GetValueFromAnnotationAttributes<T>(string name, Func<AnnotationAttributes, T> getValue) {
			AnnotationAttributes a;
			return metadataStorage.TryGetValue(name, out a) ? getValue(a) : default(T);
		}
		T GetValueFromFilterAttributes<T>(string name, Func<FilterAttributes, T> getValue) {
			FilterAttributes va;
			return metadataStorage.TryGetValue(name, out va) ? getValue(va) : default(T);
		}
		string GetName(string path) {
			int pos = path.LastIndexOf('.');
			if(pos < 0) return path;
			return path.Substring(pos, path.Length - pos);
		}
	}
}
