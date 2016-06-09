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
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
namespace DevExpress.Persistent.BaseImpl.EF {
	public class TypePermissionObject : ITypePermissionOperations, ICheckedListBoxItemsProvider {
		private Type targetType;
		private bool allowRead;
		private bool allowWrite;
		private bool allowCreate;
		private bool allowDelete;
		private bool allowNavigate;
		[Browsable(false)]
		public bool IsCalledSetAssociationPermissions = false;
		public TypePermissionObject() {
			MemberPermissions = new List<SecuritySystemMemberPermissionsObject>();
			ObjectPermissions = new List<SecuritySystemObjectPermissionsObject>();
		}
		public IEnumerable<IOperationPermission> GetPermissions() {
			List<IOperationPermission> result = new List<IOperationPermission>();
			if(TargetType != null) {
				if(AllowRead) {
					result.Add(new TypeOperationPermission(TargetType, SecurityOperations.Read));
				}
				if(AllowWrite) {
					result.Add(new TypeOperationPermission(TargetType, SecurityOperations.Write));
				}
				if(AllowCreate) {
					result.Add(new TypeOperationPermission(TargetType, SecurityOperations.Create));
				}
				if(AllowDelete) {
					result.Add(new TypeOperationPermission(TargetType, SecurityOperations.Delete));
				}
				if(AllowNavigate) {
					result.Add(new TypeOperationPermission(TargetType, SecurityOperations.Navigate));
				}
				foreach(SecuritySystemMemberPermissionsObject memberPermissionObject in MemberPermissions) {
					result.AddRange(memberPermissionObject.GetPermissions());
				}
				foreach(SecuritySystemObjectPermissionsObject objectPermissionObject in ObjectPermissions) {
					result.AddRange(objectPermissionObject.GetPermissions());
				}
			}
			return result;
		}
		[Browsable(false)]
		public Int32 ID { get; protected set; }
		[DisplayName("Read")]
		public Boolean AllowRead {
			get {
				return allowRead;
			}
			set {
				allowRead = value;
				if(!IsCalledSetAssociationPermissions) {
					if(Role != null && Role.AutoAssociationPermissions) {
						IsCalledSetAssociationPermissions = true;
						Role.SetAssociationTypePermissions(this, "Read");
					}
				}
			}
		}
		[DisplayName("Write")]
		public Boolean AllowWrite {
			get {
				return allowWrite;
			}
			set {
				allowWrite = value;
				if(!IsCalledSetAssociationPermissions) {
					if(Role != null && Role.AutoAssociationPermissions) {
						IsCalledSetAssociationPermissions = true;
						Role.SetAssociationTypePermissions(this, "Write");
					}
				}
			} 
		}
		[DisplayName("Create")]
		public Boolean AllowCreate {
			get {
				return allowCreate;
			}
			set {
				allowCreate = value;
			}
		}
		[DisplayName("Delete")]
		public Boolean AllowDelete {
			get {
				return allowDelete;
			}
			set {
				allowDelete = value;
			}
		}
		[DisplayName("Navigate")]
		public Boolean AllowNavigate {
			get {
				return allowNavigate;
			}
			set {
				allowNavigate = value;
			}
		}
		[Browsable(false)]
		public String TargetTypeFullName { get; protected set; }
		public virtual Role Role { get; set; }
		[Aggregated]
		public virtual IList<SecuritySystemMemberPermissionsObject> MemberPermissions { get; set; }
		[Aggregated]
		public virtual IList<SecuritySystemObjectPermissionsObject> ObjectPermissions { get; set; }
		[NotMapped]
		[ImmediatePostData]
		[RuleRequiredField]
		public Type TargetType {
			get {
				if((targetType == null) && !String.IsNullOrWhiteSpace(TargetTypeFullName)) {
					targetType = ReflectionHelper.FindType(TargetTypeFullName);
				}
				return targetType;
			}
			set {
				targetType = value;
				if(targetType != null) {
					TargetTypeFullName = targetType.FullName;
				}
				else {
					TargetTypeFullName = "";
				}
				OnItemsChanged();
			}
		}
		[NotMapped]
		public string Object {
			get {
				if(TargetType != null) {
					string classCaption = CaptionHelper.GetClassCaption(TargetType.FullName);
					return string.IsNullOrEmpty(classCaption) ? TargetType.Name : classCaption;
				}
				return string.Empty;
			}
		}
		Dictionary<Object, String> ICheckedListBoxItemsProvider.GetCheckedListBoxItems(string targetMemberName) {
			Dictionary<Object, String> result = new Dictionary<Object, String>();
			if(targetMemberName == "Members" && TargetType != null) {
				ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(TargetType);
				foreach(IMemberInfo memberInfo in typeInfo.Members) {
					if(memberInfo.IsVisible || (memberInfo.FindAttribute<SecurityBrowsableAttribute>() != null)) {
						string caption = CaptionHelper.GetMemberCaption(memberInfo);
						if(result.ContainsKey(memberInfo.Name)) {
							throw new LightDictionary<string, string>.DuplicatedKeyException(memberInfo.Name, result[memberInfo.Name], caption);
						}
						result.Add(memberInfo.Name, caption);
					}
				}
			}
			return result;
		}
		protected virtual void OnItemsChanged() {
			if(ItemsChanged != null) {
				ItemsChanged(this, EventArgs.Empty);
			}
		}
		public event EventHandler ItemsChanged;
	}
}
