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
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Native {
	public interface IDataSerializer {
		bool CanSerialize(object data, object extensionProvider);
		string Serialize(object data, object extensionProvider);
		bool CanDeserialize(string value, string typeName, object extensionProvider);
		object Deserialize(string value, string typeName, object extensionProvider);
	}
	public class SerializationService : InstanceProvider<IDataSerializer> {
		public const string Guid = "DataSerializationExtension";
		static readonly object padlock = new object();
		static SerializationService instance = new SerializationService();
		public static void RegisterSerializer(string contextName, IDataSerializer serializer) {
			lock(padlock) {
				instance.Register(contextName, serializer);
			}
		}
		public static bool SerializeObject(object value, out string result, IExtensionsProvider extensionProvider) {
			lock(padlock) {
				return instance.Serialize(value, out result, extensionProvider);
			}
		}
		public static bool DeserializeObject(string value, Type destType, out object result, IExtensionsProvider extensionProvider) {
			return DeserializeObject(value, destType.FullName, out result, extensionProvider);
		}
		public static bool DeserializeObject(string value, string typeName, out object result, IExtensionsProvider rootComponent) {
			lock(padlock) {
				return instance.Deserialize(value, typeName, out result, rootComponent);
			}
		}
		void Register(string contextName, IDataSerializer serializer) {
			SetInstance(contextName, serializer);
		}
		IDataSerializer GetSerializer(IExtensionsProvider extensionsProvider) {
			return extensionsProvider != null && extensionsProvider.Extensions.ContainsKey(SerializationService.Guid) ? GetInstance(extensionsProvider.Extensions[SerializationService.Guid]) : null; 
		}
		bool Serialize(object value, out string result, IExtensionsProvider extensionProvider) {
			IDataSerializer serializer = GetSerializer(extensionProvider);
			if(serializer != null && serializer.CanSerialize(value, extensionProvider)) {
				result = serializer.Serialize(value, extensionProvider);
				return true;
			}
			result = null;
			return false;
		}
		bool Deserialize(string value, string typeName, out object result, IExtensionsProvider extensionProvider) {
			IDataSerializer serializer = GetSerializer(extensionProvider);
			if(serializer != null && serializer.CanDeserialize(value, typeName, extensionProvider)) {
				result = serializer.Deserialize(value, typeName, extensionProvider);
				return true;
			} 
			result = null;
			return false;
		}
	}
}
