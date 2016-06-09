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
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Editors {
	public class ObjectEditorHelperBase {
		private IMemberInfo displayMember;
		private ITypeInfo lookupObjectTypeInfo;
		protected String lookupPropertyName;
		private IMemberInfo GetDisplayMember(ITypeInfo lookupObjectTypeInfo, String lookupPropertyName) {
			IMemberInfo result = null;
			if(!String.IsNullOrEmpty(lookupPropertyName)) {
				IMemberInfo displayableMemberDescriptor = ReflectionHelper.FindDisplayableMemberDescriptor(lookupObjectTypeInfo, lookupPropertyName);
				if(displayableMemberDescriptor != null) {
					result = displayableMemberDescriptor;
				}
				else {
					result = lookupObjectTypeInfo.FindMember(lookupPropertyName);
				}
			}
			if((result == null) && (lookupObjectTypeInfo != null)) {
				result = lookupObjectTypeInfo.DeclaredDefaultMember;
			}
			return result;
		}
		protected virtual string Escape(string text) {
			return text;
		}
		public ObjectEditorHelperBase(ITypeInfo lookupObjectTypeInfo, IModelMemberViewItem viewItemModel) {
			this.lookupObjectTypeInfo = lookupObjectTypeInfo;
			lookupPropertyName = (viewItemModel != null) ? viewItemModel.LookupProperty : "";
			displayMember = GetDisplayMember(lookupObjectTypeInfo, lookupPropertyName);
		}
		public ObjectEditorHelperBase(ITypeInfo lookupObjectTypeInfo, IMemberInfo displayMember) {
			this.lookupObjectTypeInfo = lookupObjectTypeInfo;
			this.displayMember = displayMember;
		}
		public string GetFullDisplayMemberName(string propertyName) {
			return propertyName + (DisplayMember != null ? "." + DisplayMember.Name : "");
		}
		public Object GetDisplayValue(Object editValue, IObjectSpace objectSpace) {
			Object result = editValue;
			if(editValue != null) {
				if((objectSpace != null) && (editValue is XafDataViewRecord)) {
					if((DisplayMember != null) && ((XafDataViewRecord)editValue).ContainsMember(DisplayMember.Name)) {
						result = ((XafDataViewRecord)editValue)[DisplayMember.Name];
					}
					else if((lookupObjectTypeInfo.KeyMembers.Count > 0) && ((XafDataViewRecord)editValue).ContainsMember(lookupObjectTypeInfo.KeyMembers[0].Name)) {
						result = ((XafDataViewRecord)editValue)[lookupObjectTypeInfo.KeyMembers[0].Name];
					}
				}
				else {
					if((DisplayMember != null) && LookupObjectType.IsAssignableFrom(editValue.GetType())) {
						result = DisplayMember.GetValue(editValue);
					}
				}
			}
			return result;
		}
		public String GetDisplayText(Object editValue, String nullText, String format, IObjectSpace objectSpace) {
			if(editValue != null) {
				Object result = GetDisplayValue(editValue, objectSpace);
				if(result != null) {
					if(result.GetType().IsEnum) {
						EnumDescriptor enumDescriptor = new EnumDescriptor(result.GetType());
						result = enumDescriptor.GetCaption(result);
					}
					if(!String.IsNullOrEmpty(format)) {
						result = String.Format(format, result);
					}
					return result.ToString();
				}
				return "";
			}
			return nullText;
		}
		public String GetEscapedDisplayText(Object editValue, String nullText, String displayFormat, IObjectSpace objectSpace) {
			return Escape(GetDisplayText(editValue, nullText, displayFormat, objectSpace));
		}
		public Object GetDisplayValue(Object editValue) {
			return GetDisplayValue(editValue, null);
		}
		public String GetDisplayText(Object editValue, String nullText, String format) {
			return GetDisplayText(editValue, nullText, format, null);
		}
		public String GetEscapedDisplayText(Object editValue, String nullText, String displayFormat) {
			return GetEscapedDisplayText(editValue, nullText, displayFormat, null);
		}
		public Type LookupObjectType {
			get { return LookupObjectTypeInfo.Type; }
		}
		public ITypeInfo LookupObjectTypeInfo {
			get { return lookupObjectTypeInfo; }
			protected internal set {
				lookupObjectTypeInfo = value;
				displayMember = GetDisplayMember(lookupObjectTypeInfo, lookupPropertyName);
			}
		}
		public IMemberInfo DisplayMember {
			get { return displayMember; }
		}
	}
}
