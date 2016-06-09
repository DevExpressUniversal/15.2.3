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
namespace DevExpress.ExpressApp.DC {
	[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, Inherited = true)]
	public class DomainComponentAttribute : Attribute {
	}
	public class DomainLogicAttribute : Attribute {
		private Type interType;
		public DomainLogicAttribute(Type interfaceType) {
			interType = interfaceType;
		}
		public Type InterfaceType {
			get { return interType; }
		}
	}
	[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Property)]
	public class PersistentDcAttribute : Attribute {
	}
	[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Property)]
	public class NonPersistentDcAttribute : Attribute {
	}
	public class FieldSizeAttribute : Attribute {
		public const Int32 Unlimited = -1;
		private Int32 size;
		public FieldSizeAttribute(Int32 size) {
			this.size = size;
		}
		public Int32 Size {
			get { return size; }
		}
	}
	public class AggregatedAttribute : Attribute {
	}
	public class IgnoreForAssociationAttribute : Attribute {
	}
	[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, Inherited = true)]
	public class XafDefaultPropertyAttribute : Attribute {
		private String name;
		public XafDefaultPropertyAttribute(String name) {
			this.name = name;
		}
		public String Name {
			get { return name; }
		}
	}
	[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public sealed class XafDisplayNameAttribute : Attribute {
		private string displayName;
		public XafDisplayNameAttribute(string displayName) {
			this.displayName = displayName;
		}
		public string DisplayName { get { return displayName; } }
	}
	[AttributeUsage(AttributeTargets.Property, Inherited = true)]
	public class CalculatedAttribute : Attribute {
		private String expression;
		public CalculatedAttribute(String expression) {
			this.expression = expression;
		}
		public String Expression {
			get { return expression; }
		}
	}
	[AttributeUsage(AttributeTargets.Property, Inherited = true)]
	public class BackReferencePropertyAttribute : Attribute {
		string propertyName;
		public string PropertyName {
			get { return propertyName; }
		}
		public BackReferencePropertyAttribute(string propertyName) {
			this.propertyName = propertyName;
		}
	}
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = true)]
	public class IncludeItemAttribute : Attribute {
		bool isOverrideMethod;
		public bool IsOverrideMethod { get { return isOverrideMethod; } }
		public IncludeItemAttribute() {
			isOverrideMethod = false;
		}
		public IncludeItemAttribute(bool isOverrideMethod) {
			this.isOverrideMethod = isOverrideMethod;
		}
	}
	[AttributeUsage(AttributeTargets.Method)]
	public class CreateInstanceAttribute : Attribute {
		private Type instanceType;
		public CreateInstanceAttribute() { }
		public CreateInstanceAttribute(Type instanceType) {
			this.instanceType = instanceType;
		}
		public Type InstanceType {
			get { return instanceType; }
		}
	}
}
