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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
namespace DevExpress.ExpressApp.Security.Strategy {
	[System.ComponentModel.DisplayName("Member Operation Permissions")]
	[ImageName("BO_Security_Permission_Member")]
	[DefaultListViewOptions(true, NewItemRowPosition.Top)]
	public class SecuritySystemMemberPermissionsObject : XPCustomObject, ICheckedListBoxItemsProvider {
		protected virtual void OnItemsChanged() {
			if(ItemsChanged != null) {
				ItemsChanged(this, EventArgs.Empty);
			}
		}
		private void OnPropertyChangedPermissions(string operation, bool value) {
			SecuritySystemTypePermissionObject securitySystemTypePermissionObject = Owner as SecuritySystemTypePermissionObject;
			if(securitySystemTypePermissionObject != null) {
				SecuritySystemRole securitySystemRole = securitySystemTypePermissionObject.Owner as SecuritySystemRole;
				if(securitySystemRole != null && SecuritySystemRoleBase.AutoAssociationPermissions) {
					securitySystemRole.TypeAssociationPermissionsOwnerHelper.FindAndSetAssociationMemberPermission(this, operation, value);
				}
			}
		}
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
		public SecuritySystemMemberPermissionsObject(Session session) : base(session) { }
		public IList<IOperationPermission> GetPermissions() {
			IList<IOperationPermission> result = new List<IOperationPermission>();
			if(Owner == null) {
				Tracing.Tracer.LogWarning("Cannot create OperationPermission objects: Owner property returns null. {0} class, {1} Oid", GetType(), Oid);
				return result;
			}
			else if(Owner.TargetType == null) {
				Tracing.Tracer.LogWarning("Cannot create OperationPermission objects: Owner.TargetType property returns null. {0} class, {1} Oid", GetType(), Oid);
				return result;
			}
			else if(string.IsNullOrEmpty(Members)) {
				Tracing.Tracer.LogWarning("Cannot create OperationPermission objects: Members property returns null or empty string. {0} class, {1} Oid", GetType(), Oid);
				return result;
			}
			else {
				if(AllowRead) {
					if(string.IsNullOrEmpty(Criteria)) {
						result.Add(new MemberOperationPermission(Owner.TargetType, Members, SecurityOperations.Read));
					}
					else {
						result.Add(new MemberCriteriaOperationPermission(Owner.TargetType, Members, Criteria, SecurityOperations.Read));
					}
				}
				if(AllowWrite) {
					if(string.IsNullOrEmpty(Criteria)) {
						result.Add(new MemberOperationPermission(Owner.TargetType, Members, SecurityOperations.Write));
					}
					else {
						result.Add(new MemberCriteriaOperationPermission(Owner.TargetType, Members, Criteria, SecurityOperations.Write));
					}
				}
			}
			return result;
		}
		[Size(SizeAttribute.Unlimited)]
		[VisibleInListView(true)]
		[EditorAlias(EditorAliases.CheckedListBoxEditor)]
		public string Members {
			get { return GetPropertyValue<string>("Members"); }
			set { SetPropertyValue<string>("Members", value); }
		}
		[NonPersistent]
		[Browsable(false)]
		public bool IsMemberExists {
			get {
				if(string.IsNullOrEmpty(Members)) {
					return false;
				}
				ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(Owner.TargetType);
				string[] membersArray = Members.Split(';');
				if(membersArray.Length == 0) {
					return false;
				}
				foreach(string member in membersArray) {
					if(typeInfo.FindMember(member.Trim()) == null) {
						return false;
					}
				}
				return true;
			}
		}
		[VisibleInListView(false), VisibleInDetailView(false)]
		public bool AllowRead {
			get { return GetPropertyValue<bool>("AllowRead"); }
			set {
				SetPropertyValue<bool>("AllowRead", value);
				OnPropertyChangedPermissions(SecurityOperations.Read, value);
			}
		}
		[VisibleInListView(false), VisibleInDetailView(false)]
		public bool AllowWrite {
			get { return GetPropertyValue<bool>("AllowWrite"); }
			set {
				SetPropertyValue<bool>("AllowWrite", value);
				OnPropertyChangedPermissions(SecurityOperations.Write, value);
			}
		}
		[CriteriaOptions("Owner.TargetType")]
		[EditorAlias(EditorAliases.PopupCriteriaPropertyEditor)]
		[Size(SizeAttribute.Unlimited)]
		[DevExpress.ExpressApp.Model.ModelDefault("RowCount", "0")]
		[VisibleInListView(true), VisibleInDetailView(true)]
		public string Criteria {
			get { return GetPropertyValue<string>("Criteria"); }
			set { SetPropertyValue<string>("Criteria", value); }
		}
		[Association]
		[VisibleInListView(false), VisibleInDetailView(false)]
		public SecuritySystemTypePermissionsObjectBase Owner {
			get { return GetPropertyValue<SecuritySystemTypePermissionsObjectBase>("Owner"); }
			set { SetPropertyValue<SecuritySystemTypePermissionsObjectBase>("Owner", value); }
		}
		[NonPersistent]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[DevExpress.Xpo.DisplayName("Read")]
		public bool? EffectiveRead {
			get { return AllowRead ? true : (Owner != null && Owner.AllowRead) ? (bool?)null : false; }
			set { AllowRead = value.HasValue ? value.Value : false; }
		}
		[NonPersistent]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[DevExpress.Xpo.DisplayName("Write")]
		public bool? EffectiveWrite {
			get { return AllowWrite ? true : (Owner != null && Owner.AllowWrite) ? (bool?)null : false; }
			set { AllowWrite = value.HasValue ? value.Value : false; }
		}
		public string InheritedFrom {
			get {
				string result = "";
				if(Owner != null) {
					if(Owner.AllowRead) {
						result = string.Concat(result, string.Format(CaptionHelper.GetLocalizedText("Messages", "Read") + CaptionHelper.GetLocalizedText("Messages", "IsInheritedFrom"), CaptionHelper.GetClassCaption(Owner.TargetType.FullName)));
					}
					if(Owner.AllowWrite) {
						result = string.Concat(result, string.Format(CaptionHelper.GetLocalizedText("Messages", "Write") + CaptionHelper.GetLocalizedText("Messages", "IsInheritedFrom"), CaptionHelper.GetClassCaption(Owner.TargetType.FullName)));
					}
				}
				return result;
			}
		}
		Dictionary<Object, String> ICheckedListBoxItemsProvider.GetCheckedListBoxItems(string targetMemberName) {
			if(Owner == null || !(Owner is ICheckedListBoxItemsProvider)) {
				return new Dictionary<Object, String>();
			}
			return ((ICheckedListBoxItemsProvider)Owner).GetCheckedListBoxItems(targetMemberName);
		}
		public event EventHandler ItemsChanged;
	}
}
