#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Collections.Generic;
using System.Collections.Specialized;
namespace DevExpress.ExpressApp.DC.ClassGeneration {
	internal class AttributeTypesRegistrationInfo {
		private static AttributeTypesRegistrationInfo _instance;
		private readonly Dictionary<Type, string[]> attributesParameters;
		private readonly Dictionary<Type, string[]> namedParameters;
		private AttributeTypesRegistrationInfo() {
			attributesParameters = new Dictionary<Type, string[]>();
			namedParameters = new Dictionary<Type, string[]>();
			RegisterAttributeParameters(typeof(DevExpress.Xpo.AssociationAttribute), new string[] { "Name", "AssemblyName", "ElementTypeName"});
			RegisterAttributeParameters(typeof(DevExpress.Xpo.ManyToManyAliasAttribute), new string[] { "OneToManyCollectionName", "ReferenceInTheIntermediateTableName" });
			RegisterAttributeParameters(typeof(DevExpress.Xpo.PersistentAliasAttribute), new string[] { "AliasExpression" });
			RegisterAttributeParameters(typeof(DevExpress.Xpo.PersistentAttribute), new string[] { "MapTo" });
			RegisterAttributeParameters(typeof(DevExpress.Xpo.DelayedAttribute), new string[] { "FieldName", "GroupName", "UpdateModifiedOnly" });
			RegisterAttributeParameters(typeof(DevExpress.Xpo.SizeAttribute), new string[] { "Size" });
			RegisterAttributeParameters(typeof(DevExpress.Xpo.NonPersistentAttribute), new string[] { });
			RegisterAttributeParameters(typeof(DevExpress.Xpo.AggregatedAttribute), new string[] { });
			RegisterAttributeParameters(typeof(DevExpress.Xpo.ValueConverterAttribute), new string[] { "ConverterType" });
			RegisterAttributeParameters(typeof(DevExpress.Xpo.ExplicitLoadingAttribute), new string[] { "Depth" });
			RegisterAttributeParameters(typeof(DevExpress.Xpo.DbTypeAttribute), new string[] { "DbColumnTypeName" });
			RegisterAttributeParameters(typeof(DevExpress.Xpo.IndexedAttribute), new string[] { "AdditionalFields" }, new string[] { "Unique" });
			RegisterAttributeParameters(typeof(DevExpress.Xpo.NullValueAttribute), new string[] { "Value" });
			RegisterAttributeParameters(typeof(DevExpress.Xpo.MapInheritanceAttribute), new string[] { "MapType" });
			RegisterAttributeParameters(typeof(DevExpress.Xpo.OptimisticLockingAttribute), new string[] { "FieldName" });
			RegisterAttributeParameters(typeof(DevExpress.Xpo.DeferredDeletionAttribute), new string[] { "Enabled" });
			RegisterAttributeParameters(typeof(DevExpress.Xpo.CustomAttribute), new string[] { "Name", "Value" });
		}
		public static AttributeTypesRegistrationInfo Instance {
			get {
				if(_instance == null) {
					_instance = new AttributeTypesRegistrationInfo();
				}
				return _instance;
			}
		}
		internal void RegisterAttributeParameters(Type attributeType, string[] constructorParameterNames) {
			RegisterAttributeParameters(attributeType, constructorParameterNames, new string[] { });
		}
		internal void RegisterAttributeParameters(Type attributeType, string[] constructorParameterNames, string[] namedParameterNames) {
			attributesParameters[attributeType] = constructorParameterNames;
			namedParameters[attributeType] = namedParameterNames;
		}
		internal bool IsAttrubuteRegistered(Type attributeType) {
			return attributesParameters.ContainsKey(attributeType);
		}
		internal string[] GetAttributeParametersByType(Type attributeType, bool isNamed) {
			string[] result = null;
			if(isNamed) {
				namedParameters.TryGetValue(attributeType, out result);
			}
			else {
				attributesParameters.TryGetValue(attributeType, out result);
			}
			return result;
		}
	}
	internal class AttributesCode : CodeProvider {
		private IList<Attribute> attributes;
		public static void Append(CodeBuilder builder, IList<Attribute> attributes) {
			AttributesCode attributesCodeGenerator = new AttributesCode(attributes);
			attributesCodeGenerator.GetCode(builder);
		}
		internal AttributesCode(IList<Attribute> attributes) {
			this.attributes = attributes;
		}
		private static string ObjectToString(object obj) {
			if(obj == null) {
				return "null";
			}
			if(obj is string) {
				return string.Format(@"@""{0}""", obj.ToString().Replace("\"", "\"\""));
			}
			if(obj is Type) {
				return string.Format(@"typeof({0})", CodeBuilder.TypeToString((Type)obj));
			}
			if(obj is bool) {
				return (bool)obj ? "true" : "false";
			}
			if(obj is char) {
				return string.Format("'{0}'", obj);
			}
			if(obj is IEnumerable && obj.GetType().IsGenericType && obj.GetType().GetGenericArguments().Length == 1) {
				Type genericArgumentType = obj.GetType().GetGenericArguments()[0];
				List<string> parameters = new List<string>();
				foreach(object item in (IEnumerable)obj) {
					parameters.Add(ObjectToString(item));
				}
				return string.Format("new {0}[] {{ {1} }}", CodeBuilder.TypeToString(genericArgumentType), string.Join(", ", parameters.ToArray()));
			}
			if(obj is StringCollection) {
				List<string> parameters = new List<string>();
				foreach(string item in (StringCollection)obj) {
					parameters.Add(ObjectToString(item));
				}
				return string.Format("new string[] {{ {0} }}", string.Join(", ", parameters.ToArray()));
			}
			if(obj.GetType().IsEnum) {
				return string.Format("{0}.{1}", CodeBuilder.TypeToString(obj.GetType()), obj.ToString());
			}
			return obj.ToString().Replace(',', '.');
		}
		protected override void GetCodeCore(CodeBuilder builder) {
			foreach(Attribute attribute in attributes) {
				Type attributeType = attribute.GetType();
				if(AttributeTypesRegistrationInfo.Instance.IsAttrubuteRegistered(attributeType)) {
					List<string> attributeParameters = new List<string>();
					string[] attributeParameterPropertyNames = AttributeTypesRegistrationInfo.Instance.GetAttributeParametersByType(attributeType, false);
					foreach(string propertyName in attributeParameterPropertyNames) {
						object parameterValue = attributeType.GetProperty(propertyName).GetValue(attribute, new object[] { });
						attributeParameters.Add(ObjectToString(parameterValue));
					}
					string[] namedParameterPropertyNames = AttributeTypesRegistrationInfo.Instance.GetAttributeParametersByType(attributeType, true);
					foreach(string propertyName in namedParameterPropertyNames) {
						object parameterValue = attributeType.GetProperty(propertyName).GetValue(attribute, new object[] { });
						attributeParameters.Add(string.Format("{0} = {1}", propertyName, ObjectToString(parameterValue)));
					}
					if(attributeParameters.Count == 0) {
						builder.AppendLine(string.Format("[{0}]", CodeBuilder.TypeToString(attribute.GetType())));
					}
					else {
						string attributeTypeString = CodeBuilder.TypeToString(attribute.GetType());
						string attributeParametersString = string.Join(", ", attributeParameters.ToArray());
						builder.AppendLine(string.Format("[{0}({1})]", attributeTypeString, attributeParametersString));
					}
				}
			}
		}
	}
}
