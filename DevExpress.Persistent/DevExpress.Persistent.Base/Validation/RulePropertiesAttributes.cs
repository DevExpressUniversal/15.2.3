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
using System.Reflection;
namespace DevExpress.Persistent.Validation {
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class RulePropertiesMemberOfAttribute : Attribute {
		private string declaringTypeMemberName;
		public RulePropertiesMemberOfAttribute(string declaringTypeMemberName) {
			this.declaringTypeMemberName = declaringTypeMemberName;
		}
		public string DeclaringTypeMemberName {
			get { return declaringTypeMemberName; }
			set { declaringTypeMemberName = value; }
		}
	}
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class RulePropertiesIndexedAttribute : Attribute {
	}
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class RulePropertiesIndexAttribute : Attribute {
		private int index;
		public RulePropertiesIndexAttribute(int index) {
			this.index = index;
		}
		public int Index {
			get { return index; }
		}
		public static int RetrieveIndex(PropertyInfo propertyInfo) {
			object[] attributes = propertyInfo.GetCustomAttributes(typeof(RulePropertiesIndexAttribute), false);
			if(attributes.Length > 0) {
				return ((RulePropertiesIndexAttribute)attributes[0]).index;
			}
			return -1;
		}
	}
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class RulePropertiesRequiredAttribute : Attribute {
		private bool isRequired;
		public RulePropertiesRequiredAttribute() {
			isRequired = true;
		}
		public RulePropertiesRequiredAttribute(bool isRequired) {
			this.isRequired = isRequired;
		}
		public bool IsRequired {
			get { return isRequired; }
			set { isRequired = value; }
		}
	}
}
