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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using DevExpress.DataAccess.EntityFramework;
namespace DevExpress.DataAccess.UI.Native.EntityFramework {
	class EFDataConnectionTypeConverter : ExpandableObjectConverter {
		class SourcePropertyDescriptor : SimplePropertyDescriptor {
			public SourcePropertyDescriptor()
				: base(typeof(EFDataConnection), "Source", typeof(string), new Attribute[] { new ReadOnlyAttribute(true) }) {
			}
			public override object GetValue(object component) {
				return ((EFDataConnection)component).ConnectionParameters.Source.ToString();
			}
			public override void SetValue(object component, object value) {
				throw new NotSupportedException();
			}
		}
		class ConnectionStringPropertyDescriptor : SimplePropertyDescriptor {
			public ConnectionStringPropertyDescriptor()
				: base(typeof(EFDataConnection), "ConnectionString", typeof(string), new Attribute[] { new ReadOnlyAttribute(true) } ) {
			}
			public override object GetValue(object component) {
				return ((EFDataConnection)component).ConnectionParameters.ConnectionString;
			}
			public override void SetValue(object component, object value) {
				throw new NotSupportedException();
			}
		}
		class ConnectionStringNamePropertyDescriptor : SimplePropertyDescriptor {
			public ConnectionStringNamePropertyDescriptor()
				: base(typeof(EFDataConnection), "ConnectionStringName", typeof(string), new Attribute[] { new ReadOnlyAttribute(true) }) {
			}
			public override object GetValue(object component) {
				return ((EFDataConnection)component).ConnectionParameters.ConnectionStringName;
			}
			public override void SetValue(object component, object value) {
				throw new NotSupportedException();
			}
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if(destinationType == typeof(InstanceDescriptor))
				return true;
			if(destinationType == typeof(string))
				return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(InstanceDescriptor) && value != null) {
				EFDataConnection connection = (EFDataConnection)value;
				ConstructorInfo ci = typeof(EFDataConnection).GetConstructor(new[] { typeof(string), typeof(EFConnectionParameters) });
				return new InstanceDescriptor(ci, new object[] { connection.Name, connection.ConnectionParameters });
			}
			if(destinationType == typeof(string) && value != null) {
				EFDataConnection connection = (EFDataConnection)value;
				if(!string.IsNullOrWhiteSpace(connection.Name))
					return connection.Name;
				if(!string.IsNullOrWhiteSpace(connection.ConnectionParameters.ConnectionStringName))
					return connection.ConnectionParameters.ConnectionStringName;
				return connection.ConnectionParameters.ConnectionString;
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			EFDataConnection dataConnection = value as EFDataConnection;
			if(dataConnection != null) {
				List<PropertyDescriptor> result = new List<PropertyDescriptor>();
				if(dataConnection.ConnectionParameters != null) {
					result.Add(new SourcePropertyDescriptor());
					result.Add(new ConnectionStringPropertyDescriptor());
					result.Add(new ConnectionStringNamePropertyDescriptor());
				}
				return new PropertyDescriptorCollection(result.ToArray());
			}
			return base.GetProperties(context, value, attributes);
		}
	}
}
