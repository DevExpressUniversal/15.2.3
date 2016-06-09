#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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
namespace DevExpress.Mvvm.DataAnnotations {
	public abstract class OrderAttribute : Attribute {
		int? order;
		public int Order {
			get {
				if(order == null)
					throw new InvalidOperationException();
				return order.Value;
			}
			set { 
				order = value; 
			}
		}
		public int? GetOrder() {
			return order;
		}
	}
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
	public class ToolBarItemAttribute : OrderAttribute {
		public string Page { get; set; }
		public string PageGroup { get; set; }
	}
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
	public class ContextMenuItemAttribute : OrderAttribute {
		public string Group { get; set; }
	}
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
	public class CommandParameterAttribute : Attribute {
		public string CommandParameter { get; private set; }
		public CommandParameterAttribute(string commandParameter) {
			this.CommandParameter = commandParameter;
		}
	}
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class HiddenAttribute : Attribute {
	}
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public abstract class InstanceInitializerAttributeBase : Attribute {
		readonly Func<object> createInstanceCallback;
		protected InstanceInitializerAttributeBase(Type type) 
			: this(type, type.Name, null) {
		}
		protected InstanceInitializerAttributeBase(Type type, string name, Func<object> createInstanceCallback) {
			if(Object.ReferenceEquals(type, null))
				throw new ArgumentNullException("type");
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");
			this.Type = type;
			this.Name = name;
			this.createInstanceCallback = createInstanceCallback;
		}
		public string Name { get; private set; }
		public Type Type { get; private set; }
		public virtual object CreateInstance() {
			return createInstanceCallback != null ? createInstanceCallback() : Activator.CreateInstance(Type);
		}
#if !SILVERLIGHT && !NETFX_CORE
		public override object TypeId {
			get {
				return this;
			}
		}
#endif
	}
	public class InstanceInitializerAttribute : InstanceInitializerAttributeBase {
		public InstanceInitializerAttribute(Type type)
			: base(type) {
		}
		public InstanceInitializerAttribute(Type type, string name)
			: base(type, name, null) {
		}
		internal InstanceInitializerAttribute(Type type, string name, Func<object> createInstanceCallback)
			: base(type, name, createInstanceCallback) {
		}
	}
	public class NewItemInstanceInitializerAttribute : InstanceInitializerAttributeBase {
		public NewItemInstanceInitializerAttribute(Type type)
			: base(type) {
		}
		public NewItemInstanceInitializerAttribute(Type type, string name)
			: base(type, name, null) {
		}
		internal NewItemInstanceInitializerAttribute(Type type, string name, Func<object> createInstanceCallback)
			: base(type, name, createInstanceCallback) {
		}
	}
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class ScaffoldDetailCollectionAttribute : Attribute {
		public const bool DefaultScaffold = true;
		public ScaffoldDetailCollectionAttribute()
			: this(DefaultScaffold) {
		}
		public ScaffoldDetailCollectionAttribute(bool scaffold) {
			this.Scaffold = scaffold;
		}
		public bool Scaffold { get; private set; }
	}
}
