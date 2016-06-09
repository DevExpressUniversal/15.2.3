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
using System.Runtime.Serialization;
using System.Text;
using DevExpress.Persistent.Base;
using DevExpress.Utils;
namespace DevExpress.ExpressApp.Security {
	public static class SecurityOperations {
		public const string Read = "Read";
		public const string Write = "Write";
		public const string Create = "Create";
		public const string Delete = "Delete";
		public const string Navigate = "Navigate";
		public const string FullObjectAccess = Read + ";" + Write + ";" + Delete + ";" + Navigate;
		public const string FullAccess = FullObjectAccess + ";" + Create;
		public const string ReadOnlyAccess = Read + ";" + Navigate;
		public const string CRUDAccess = Create + ";" + Read + ";" + Write + ";" + Delete;
		public const string ReadWriteAccess = Read + ";" + Write;
		public const char Delimiter = ';';
	}
	public interface IObjectTypePermissionRequst {
		Type ObjectType { get; }
	}
	public class PermissionRequest : IPermissionRequest {
		public PermissionRequest(IObjectSpace objectSpace, Type objectType, string operation) : this(objectSpace, objectType, operation, null) { }
		public PermissionRequest(IObjectSpace objectSpace, Type objectType, string operation, object targetObject) : this(objectSpace, objectType, operation, null, null) { }
		public PermissionRequest(IObjectSpace objectSpace, Type objectType, string operation, object targetObject, string memberName) {
			Guard.ArgumentNotNull(objectType, "objectType");
			Guard.ArgumentIsNotNullOrEmpty(operation, "operation");
			ObjectSpace = objectSpace;
			ObjectType = objectType;
			Operation = operation;
			TargetObject = targetObject;
			MemberName = memberName;
		}
		public IObjectSpace ObjectSpace { get; set; }
		public Type ObjectType { get; set; }
		public string Operation { get; set; }
		public object TargetObject { get; set; }
		public string MemberName { get; set; }
		#region IPermissionRequest Members
		public object GetHashObject() {
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(ObjectSpace == null ? "null" : ObjectSpace.GetHashCode().ToString());
			stringBuilder.Append("-");
			stringBuilder.Append(ObjectType == null ? "null" : ObjectType.FullName);
			stringBuilder.Append("-");
			stringBuilder.Append(Operation == null ? "null" : Operation);
			stringBuilder.Append("-");
			stringBuilder.Append(TargetObject == null ? "null" : TargetObject.GetHashCode().ToString());
			stringBuilder.Append("-");
			stringBuilder.Append(MemberName == null ? "null" : MemberName);
			return stringBuilder.ToString();
		}
		#endregion
	}
	[Serializable]
	public abstract class OperationPermissionRequestBase : IPermissionRequest{
		public static bool UseStringHashCodeObject = true; 
		private string operation;
		protected OperationPermissionRequestBase() { }
		public OperationPermissionRequestBase(string operation) {
			this.operation = operation;
		}
		public string Operation {
			get { return operation; }
			private set { operation = value; }
		}
		public abstract object GetHashObject();
	}
	[Obsolete("Use 'PermissionRequest' instead.")]
	[Serializable]
	public class ClientPermissionRequest : OperationPermissionRequestBase, ISerializable, IObjectTypePermissionRequst {
		private object hashCodeObject;
		public ClientPermissionRequest(Type objectType, string memberName, string targetObjectHandle, string operation)
			: base(operation) {
			Guard.ArgumentNotNull(objectType, "objectType");
			MemberName = memberName;
			TargetObjectHandle = targetObjectHandle;
			ObjectType = objectType;
		}
		public ClientPermissionRequest(SerializationInfo information, StreamingContext context)
			: base(information.GetString("Operation")) {
			MemberName = information.GetString("MemberName");
			string typeName = information.GetString("ObjectType");
			Guard.ArgumentIsNotNullOrEmpty(typeName, "information.GetString(\"ObjectType\")");
			ObjectType = ReflectionHelper.GetType(typeName);
			TargetObjectHandle = information.GetString("TargetObjectHandle");
		}
		[System.Security.SecurityCritical]
		public void GetObjectData(SerializationInfo info, StreamingContext context) {
			info.AddValue("Operation", Operation);
			info.AddValue("MemberName", MemberName);
			info.AddValue("ObjectType", ObjectType.FullName);
			info.AddValue("TargetObjectHandle", TargetObjectHandle);
		}
		public override object GetHashObject() {
			if(hashCodeObject == null) {
				StringBuilder key = new StringBuilder();
				key.Append(GetType().Name);
				key.Append(" - ");
				key.Append(ObjectType.FullName);
				key.Append(" - ");
				key.Append(Operation);
				key.Append(" - ");
				key.Append(TargetObjectHandle);
				key.Append(" - ");
				key.Append(MemberName);
				if(UseStringHashCodeObject) {
					hashCodeObject = key.ToString();
				}
				else {
					hashCodeObject = key.ToString().GetHashCode();
				}
			}
			return hashCodeObject;
		}
		public string TargetObjectHandle { get; private set; }
		public Type ObjectType { get; private set; }
		public string MemberName { get; private set; }
	}
	[Serializable]
	public class ModelOperationPermissionRequest : IPermissionRequest {
		public ModelOperationPermissionRequest() { }
		public object GetHashObject() {
			if(OperationPermissionRequestBase.UseStringHashCodeObject) {
				return typeof(ModelOperationPermissionRequest).FullName;
			}
			else {
				return typeof(ModelOperationPermissionRequest).GetHashCode();
			}
		}
	}
}
