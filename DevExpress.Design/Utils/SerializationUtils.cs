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
using System.Data;
using System.IO;
using System.Resources;
using System.Reflection;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Windows.Forms;
using Microsoft.CSharp;
#if DXWhidbey
using System.ComponentModel.Design.Serialization;
#endif
namespace DevExpress.Serialization.CodeDom {
#if DXWhidbey    
	public class DXComponentCodeDomSerializer : CodeDomSerializer {
		static CodeDomSerializer defaultSerializer = null;
		static MethodInfo deserializeInstance = null;
		static MethodInfo serializeSupportInitialize = null;
		static DXComponentCodeDomSerializer() {
			Type type = typeof(CodeDomSerializer).Assembly.GetType("System.ComponentModel.Design.Serialization.ComponentCodeDomSerializer");
			if(type != null) {
				PropertyInfo pi = null;
				pi = type.GetProperty("Default", BindingFlags.Static | BindingFlags.NonPublic);
				if(pi == null) type.GetProperty("Default", BindingFlags.Static | BindingFlags.Public);
				if(pi != null) defaultSerializer = pi.GetValue(null, null) as CodeDomSerializer;
			}
			if(defaultSerializer == null)
				defaultSerializer = new CodeDomSerializer();
			else {
				deserializeInstance = type.GetMethod("DeserializeInstance", BindingFlags.NonPublic | BindingFlags.Instance);
				serializeSupportInitialize = type.GetMethod("SerializeSupportInitialize", BindingFlags.NonPublic | BindingFlags.Instance);
			}
		}
		public override object Serialize(IDesignerSerializationManager manager, object value) {
			IComponent comp = value as Component;
			if(comp != null && comp.Site == null)
				return DXSerialize(manager, value);
			return defaultSerializer.Serialize(manager, value);
		}
		object DXSerialize(IDesignerSerializationManager manager, object value) {
			object res = base.Serialize(manager, value);
			CodeStatementCollection collection = res as CodeStatementCollection;
			if(collection == null || serializeSupportInitialize == null) return res;
			CodeExpression expression = base.GetExpression(manager, value);
			if(value is ISupportInitialize) {
				serializeSupportInitialize.Invoke(defaultSerializer,
					new object[] { manager, collection, expression, value, "BeginInit" });
				serializeSupportInitialize.Invoke(defaultSerializer,
					new object[] { manager, collection, expression, value, "EndInit" });
			}
			return res;
		}
		protected override object DeserializeInstance(IDesignerSerializationManager manager, Type type, object[] parameters, string name, bool addToContainer) {
			if(deserializeInstance != null) return deserializeInstance.Invoke(defaultSerializer, new object[] { manager, type, parameters, name, addToContainer });
			return base.DeserializeInstance(manager, type, parameters, name, addToContainer);
		}
	}
#endif
}
