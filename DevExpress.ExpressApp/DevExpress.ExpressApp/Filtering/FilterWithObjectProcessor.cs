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
using DevExpress.ExpressApp.DC;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Filtering {
	public enum FilterWithObjectsProcessorMode { ObjectToString, StringToObject, ObjectToObject }
	public class FilterWithObjectsProcessor : CriteriaProcessorBase {
		private const String ObjectTypeString = "@ObjectType:";
		private const String ObjectIdString = "@ObjectID:";
		protected IObjectSpace objectSpace;
		private FilterWithObjectsProcessorMode mode;
		private ITypeInfo objectTypeInfo;
		private bool isAsyncServerMode;
		protected override void Process(OperandValue theOperand) {
			base.Process(theOperand);
			if(mode == FilterWithObjectsProcessorMode.ObjectToString) {
				if (theOperand.Value != null) {
					ITypeInfo valueTypeInfo = XafTypesInfo.Instance.FindTypeInfo(theOperand.Value.GetType());
					if (valueTypeInfo != null && valueTypeInfo.IsPersistent) {
						theOperand.Value = ObjectTypeString + valueTypeInfo.FullName + ObjectIdString + objectSpace.GetKeyValueAsString(theOperand.Value);
					}
				}
			}
			else if(mode == FilterWithObjectsProcessorMode.StringToObject) {
				if(theOperand.Value is String) {
					object value;
					if(CanConvertStringToObject((String)theOperand.Value, out value)) {
						ITypeInfo typeInfo = value != null ? XafTypesInfo.Instance.FindTypeInfo(value.GetType()) : null;
						theOperand.Value = isAsyncServerMode && typeInfo != null && typeInfo.DeclaredDefaultMember != null ? ReflectionHelper.GetMemberValue(value, typeInfo.DeclaredDefaultMember.BindingName) : value;
					}
				}
			}
			else if(mode == FilterWithObjectsProcessorMode.ObjectToObject) {
				theOperand.Value = (objectSpace.GetObject(theOperand.Value) == null) ? theOperand.Value : objectSpace.GetObject(theOperand.Value);
			}
		}
		protected override void Process(AggregateOperand theOperand) {
			base.Process(theOperand);
			if(!CriteriaOperator.ReferenceEquals(theOperand.Condition, null)) {
				theOperand.Condition.Accept(this);
			}
		}
		protected override void Process(OperandProperty theOperand) {
			base.Process(theOperand);
			IMemberInfo displayableMemberDescriptor = objectTypeInfo != null ? ReflectionHelper.FindDisplayableMemberDescriptor(objectTypeInfo, theOperand.PropertyName) : null;
			if(isAsyncServerMode && displayableMemberDescriptor != null) {
				theOperand.PropertyName = displayableMemberDescriptor.BindingName;
			}
		}
		public bool CanConvertStringToObject(string strValue, out object result) {
			result = null;
			if(string.IsNullOrEmpty(strValue)) return false;
			strValue = strValue.Trim();
			if(string.IsNullOrEmpty(strValue)) return false;
			if(strValue[0] == '\'') strValue = strValue.Remove(0, 1);
			if(!string.IsNullOrEmpty(strValue) && strValue[strValue.Length - 1] == '\'') strValue = strValue.Remove(strValue.Length - 1, 1);
			Int32 index1 = strValue.IndexOf(ObjectTypeString);
			Int32 index2 = strValue.IndexOf(ObjectIdString);
			if((index1 == 0) && (index2 > 0)) {
				String objectTypeName = strValue.Substring(ObjectTypeString.Length, index2 - ObjectTypeString.Length);
				String objectId = strValue.Substring(index2 + ObjectIdString.Length);
				Type objectType = ReflectionHelper.GetType(objectTypeName);
				result = objectSpace.GetObjectByKey(objectType, objectSpace.GetObjectKey(objectType, objectId));
				return true;
			} else {
				int openBracketIndex = strValue.IndexOf('(');
				if(openBracketIndex > 0 && strValue[strValue.Length - 1] == ')' && openBracketIndex < strValue.Length - 2) {
					string objectTypeName = strValue.Substring(0, openBracketIndex);
					string objectId = strValue.Substring(openBracketIndex + 1, strValue.Length - openBracketIndex - 2);
					Type objectType = ReflectionHelper.FindType(objectTypeName);
					if(objectType != null) {
						result = objectSpace.GetObjectByKey(objectType, objectSpace.GetObjectKey(objectType, objectId));
						return true;
					}
				}
			}
			return false;
		}
		public FilterWithObjectsProcessor(IObjectSpace objectSpace) {
			this.objectSpace = objectSpace;
		}
		public FilterWithObjectsProcessor(IObjectSpace objectSpace, ITypeInfo objectTypeInfo, bool isAsyncServerMode)
			: this(objectSpace) {
			this.objectTypeInfo = objectTypeInfo;
			this.isAsyncServerMode = isAsyncServerMode;
		}
		public void Process(CriteriaOperator criteria, FilterWithObjectsProcessorMode mode) {
			this.mode = mode;
			Process(criteria);
		}
		public string GetStringForObject(object obj) {
			OperandValue opValue = new OperandValue(obj);
			this.mode = FilterWithObjectsProcessorMode.ObjectToString;
			Process(opValue);
			return opValue.Value != null ? opValue.Value.ToString() : string.Empty;
		}
		public static bool IsObjectString(string value) {
			return value.StartsWith(ObjectTypeString) && value.Contains(ObjectIdString);
		}
	}
}
