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

using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.CodeDom;
using System.Security;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Security.Permissions;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils.Serializing;
namespace DevExpress.XtraReports {
	[TypeConverter(typeof(ScriptSecurityPermission.TypeConverter))]
	public class ScriptSecurityPermission {
		#region inner classes
		internal class TypeConverter : System.ComponentModel.TypeConverter {
			#region static
			static object[] GetConstructorParams(object value) {
				object[] result = new object[2];
				ScriptSecurityPermission scriptSecurityPermission = (ScriptSecurityPermission)value;
				result[0] = scriptSecurityPermission.Name;
				result[1] = scriptSecurityPermission.Deny;
				return result;
			}
			#endregion
			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
				if (destinationType == typeof(InstanceDescriptor))
					return true;
				return base.CanConvertTo(context, destinationType);
			}
			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
				if (destinationType == typeof(InstanceDescriptor)) {
					ConstructorInfo ci = typeof(ScriptSecurityPermission).GetConstructor(new Type[] {typeof(string), typeof(bool)});
					return new InstanceDescriptor(ci, GetConstructorParams(value), true);
				}
				return base.ConvertTo(context, culture, value, destinationType);
			}
		}
		class AttributeNameTypeConverter : TypeConverter {
			public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
				return true;
			}
			public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
				return true;
			}
			public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
				ArrayList array = new ArrayList();
				Assembly securityAssembly = Assembly.GetAssembly(typeof(CodeAccessPermission));
				Type[] types = securityAssembly.GetTypes();
				foreach (Type type in types)
					if (!type.IsAbstract && typeof(CodeAccessPermission).IsAssignableFrom(type))
						array.Add(type.FullName);
				return new StandardValuesCollection(array);
			}
		}
		#endregion
		string name;
		bool deny;
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("ScriptSecurityPermissionName"),
#endif
		TypeConverter(typeof(AttributeNameTypeConverter))
		]
		public string Name { get { return name; } set { name = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("ScriptSecurityPermissionDeny"),
#endif
 DefaultValue(true)]
		public bool Deny { get { return deny; } set { deny = value; } }
		[
		BrowsableAttribute(false),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public IPermission Permission { get; set; }
		public ScriptSecurityPermission(string name, bool deny) {
			this.name = name;
			this.deny = deny;
		}
		public ScriptSecurityPermission(string name) : this(name, true) {
		}
		public ScriptSecurityPermission() {
		}
		public override string ToString() {
			return String.IsNullOrEmpty(name) ? GetType().Name : name;
		}
		internal bool TryGetPermission(out IPermission permission) {
			if(Permission != null) {
				permission = Permission;
				return true;
			}
			permission = null;
			if(!String.IsNullOrEmpty(Name)) {
				Type type = Type.GetType(Name);
				if(type != null)
					permission = (IPermission)Activator.CreateInstance(type, new object[] { PermissionState.Unrestricted });
			}
			return permission != null;
		}
	}
	[
	ListBindable(BindableSupport.No),
	TypeConverter(typeof(DevExpress.Utils.Design.CollectionTypeConverter)),
	]
	public class ScriptSecurityPermissionCollection : CollectionBase {
#if !SL
	[DevExpressXtraReportsLocalizedDescription("ScriptSecurityPermissionCollectionItem")]
#endif
		public ScriptSecurityPermission this[int index] {
			get { return (ScriptSecurityPermission)InnerList[index]; }
		}
		public int Add(ScriptSecurityPermission permission) {
			return InnerList.Add(permission);
		}
		public void AddRange(ScriptSecurityPermission[] permissions) {
			InnerList.AddRange(permissions);
		}
	}
}
