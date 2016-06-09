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
using System.Linq;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
namespace DevExpress.ExpressApp.Utils {
	public class XafDataViewExpressionDescriptor : PropertyDescriptor {
		private Int32 valueIndex;
		private Type memberType;
		public XafDataViewExpressionDescriptor(String expressionName, Type memberType, Int32 valueIndex)
			: base(expressionName, new Attribute[] { }) {
			this.memberType = memberType;
			this.valueIndex = valueIndex;
		}
		public override Boolean CanResetValue(Object component) {
			return false;
		}
		public override Type ComponentType {
			get { return typeof(XafDataViewRecord); }
		}
		public override Object GetValue(Object component) {
			if(valueIndex >= 0) {
				return ((XafDataViewRecord)component)[valueIndex];
			}
			else {
				return null;
			}
		}
		public override Boolean IsReadOnly {
			get { return true; }
		}
		public override Type PropertyType {
			get { return (memberType != null) ? memberType : typeof(Object); }
		}
		public override void ResetValue(Object component) {
		}
		public override void SetValue(Object component, Object value) {
		}
		public override Boolean ShouldSerializeValue(Object component) {
			return false;
		}
	}
	public class XafDataViewExpressionDescriptorCollection : PropertyDescriptorCollection {
		private ITypeInfo typeInfo;
		public XafDataViewExpressionDescriptorCollection(ITypeInfo typeInfo)
			: base(new PropertyDescriptor[] { }) {
			this.typeInfo = typeInfo;
		}
		public override PropertyDescriptor Find(String name, Boolean ignoreCase) {
			PropertyDescriptor result = base.Find(name, ignoreCase);
			if(result == null) {
				IMemberInfo memberInfo = typeInfo.FindMember(name);
				if(memberInfo != null) {
					result = new XafDataViewExpressionDescriptor(name, memberInfo.MemberType, Int32.MinValue);
					Add(result);
				}
			}
			return result;
		}
	}
}
