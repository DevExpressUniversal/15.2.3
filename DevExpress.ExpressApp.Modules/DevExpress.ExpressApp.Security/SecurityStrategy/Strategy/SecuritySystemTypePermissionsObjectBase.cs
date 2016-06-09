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
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
namespace DevExpress.ExpressApp.Security.Strategy {
	[Persistent("SecuritySystemTypePermissionsObject")] 
	[System.ComponentModel.DisplayName("Base Type Operation Permissions")]
	public class SecuritySystemTypePermissionsObjectBase : XPCustomObject, ITypePermissionOperations, ICheckedListBoxItemsProvider {
#if MediumTrust
		private Guid oid = Guid.Empty;
		[Browsable(false), Key(true), NonCloneable]
		public Guid Oid {
			get { return oid; }
			set { oid = value; }
		}
#else
		[Persistent("Oid"), Key(true), Browsable(false), MemberDesignTimeVisibility(false)]
		private Guid oid = Guid.Empty;
		[PersistentAlias("oid"), Browsable(false)]
		public Guid Oid {
			get { return oid; }
		}
#endif
		protected virtual IEnumerable<IOperationPermission> GetPermissionsCore() {
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
			else {
				Tracing.Tracer.LogWarning("Cannot create OperationPermission objects: TargetType is null. {0} class, {1} Oid", GetType(), Oid);
			}
			return result;
		}
		protected virtual void OnItemsChanged() {
			if(ItemsChanged != null) {
				ItemsChanged(this, EventArgs.Empty);
			}
		}
		public SecuritySystemTypePermissionsObjectBase(Session session)
			: base(session) {
		}
		public IEnumerable<IOperationPermission> GetPermissions() {
			return GetPermissionsCore();
		}
		[ValueConverter(typeof(TypeToStringConverter))]
		[Size(SizeAttribute.Unlimited)]
		[VisibleInDetailView(false), VisibleInListView(false)]
		[RuleRequiredField("SecuritySystemTypePermissionsObjectBase_TargetType_RuleRequiredField", DefaultContexts.Save)]
		[TypeConverter(typeof(SecurityStrategyTargetTypeConverter))]
		[ImmediatePostData]
		public Type TargetType {
			get { return GetPropertyValue<Type>("TargetType"); }
			set {
				SetPropertyValue<Type>("TargetType", value);
				OnItemsChanged();
				OnPropertyChangedPermissions(SecurityOperations.Read, AllowRead);
				OnPropertyChangedPermissions(SecurityOperations.Write, AllowWrite);
			}
		}
		public string Object {
			get {
				if(TargetType != null) {
					string classCaption = CaptionHelper.GetClassCaption(TargetType.FullName);
					return string.IsNullOrEmpty(classCaption) ? TargetType.Name : classCaption;
				}
				return string.Empty;
			}
		}
		[System.ComponentModel.DisplayName("Read")]
		public bool AllowRead {
			get { return GetPropertyValue<bool>("AllowRead"); }
			set {
				SetPropertyValue<bool>("AllowRead", value);
				OnPropertyChangedPermissions(SecurityOperations.Read, value);
			}
		}
		protected virtual void OnPropertyChangedPermissions(string operation, bool value) {
		}
		[System.ComponentModel.DisplayName("Write")]
		public bool AllowWrite {
			get { return GetPropertyValue<bool>("AllowWrite"); }
			set {
				SetPropertyValue<bool>("AllowWrite", value);
				OnPropertyChangedPermissions(SecurityOperations.Write, value);
			}
		}
		[System.ComponentModel.DisplayName("Create")]
		public bool AllowCreate {
			get { return GetPropertyValue<bool>("AllowCreate"); }
			set {
				SetPropertyValue<bool>("AllowCreate", value);
				OnPropertyChangedPermissions(SecurityOperations.Create, value);
			}
		}
		[System.ComponentModel.DisplayName("Delete")]
		public bool AllowDelete {
			get { return GetPropertyValue<bool>("AllowDelete"); }
			set {
				SetPropertyValue<bool>("AllowDelete", value);
				OnPropertyChangedPermissions(SecurityOperations.Delete, value);
			}
		}
		[System.ComponentModel.DisplayName("Navigate")]
		public bool AllowNavigate {
			get { return GetPropertyValue<bool>("AllowNavigate"); }
			set {
				SetPropertyValue<bool>("AllowNavigate", value);
				OnPropertyChangedPermissions(SecurityOperations.Navigate, value);
			}
		}
		[DevExpress.Xpo.Aggregated]
		[Association]
		public XPCollection<SecuritySystemMemberPermissionsObject> MemberPermissions {
			get { return GetCollection<SecuritySystemMemberPermissionsObject>("MemberPermissions"); }
		}
		[DevExpress.Xpo.Aggregated]
		[Association]
		public XPCollection<SecuritySystemObjectPermissionsObject> ObjectPermissions {
			get { return GetCollection<SecuritySystemObjectPermissionsObject>("ObjectPermissions"); }
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
		public event EventHandler ItemsChanged;
	}
}
