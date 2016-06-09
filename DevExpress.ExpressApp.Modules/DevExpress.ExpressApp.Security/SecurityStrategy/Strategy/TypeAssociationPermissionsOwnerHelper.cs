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
using System.Linq;
using System.Text;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.Xpo;
namespace DevExpress.ExpressApp.Security.Strategy {
	public class TypeAssociationPermissionsOwnerHelper {
		private bool isBusy;
		private void SetAssociationMemberPermissions(SecuritySystemTypePermissionsObjectOwner typePermissionObjectOwner, Type targetType, string memberName, string allOperation, bool value) {
			if(typePermissionObjectOwner.TypePermissions.Count(p => p.TargetType == targetType) > 1) {
				return;
			}
			SecuritySystemTypePermissionObject typePermissionObject = typePermissionObjectOwner.EnsureTypePermissionObject(targetType);
			SecuritySystemMemberPermissionsObject memberPermissionsObject = GetOrCreateMemberPermissionObject(typePermissionObjectOwner, typePermissionObject, memberName);
			if(memberPermissionsObject != null) {
				string[] operations = allOperation.Split(ServerPermissionRequestProcessor.Delimiters, StringSplitOptions.RemoveEmptyEntries);
				foreach(var operation in operations) {
					switch(operation) {
						case "Read":
							if(memberPermissionsObject.AllowRead != value) {
								memberPermissionsObject.AllowRead = value;
							}
							break;
						case "Write":
							if(memberPermissionsObject.AllowWrite != value) {
								memberPermissionsObject.AllowWrite = value;
							}
							break;
					}
				}
			}
		}
		private SecuritySystemMemberPermissionsObject GetOrCreateMemberPermissionObject(SecuritySystemTypePermissionsObjectOwner typePermissionObjectOwner, SecuritySystemTypePermissionObject typePermissionObject, string memberName) {
			bool isParametersCompability = !typePermissionObject.MemberPermissions.Any(p => p.Members.Contains(memberName) &&
				(p.Members.Any(t => t == ';') || !String.IsNullOrEmpty(p.Criteria)));
			int countTargetMemberPermissions = typePermissionObject.MemberPermissions.Count(p => p.Members.Contains(memberName));
			if(!isParametersCompability || countTargetMemberPermissions > 1) {
				return null;
			}
			foreach(SecuritySystemMemberPermissionsObject memberPermission in typePermissionObject.MemberPermissions) {
				if(memberPermission.Members == memberName) {
					return memberPermission;
				}
			}
			SecuritySystemMemberPermissionsObject memberPermissionsObject = new SecuritySystemMemberPermissionsObject(typePermissionObjectOwner.Session);
			memberPermissionsObject.Members = memberName;
			typePermissionObject.MemberPermissions.Add(memberPermissionsObject);
			return memberPermissionsObject;
		}
		private void SetAggregatedAssociationTypePermissions(SecuritySystemTypePermissionsObjectOwner typePermissionObjectOwner, Type targetType, string allOperation, bool value) {
			if(typePermissionObjectOwner.TypePermissions.Count(p => p.TargetType == targetType) > 1) {
				return;
			}
			SecuritySystemTypePermissionObject typePermissionObject = typePermissionObjectOwner.EnsureTypePermissionObject(targetType);
			string[] operations = allOperation.Split(ServerPermissionRequestProcessor.Delimiters, StringSplitOptions.RemoveEmptyEntries);
			foreach(var operation in operations) {
				switch(operation) {
					case "Read":
						if(typePermissionObject.AllowRead != value) {
							typePermissionObject.AllowRead = value;
						}
						break;
					case "Write":
						if(typePermissionObject.AllowWrite != value ||
						   typePermissionObject.AllowDelete != value ||
						   typePermissionObject.AllowCreate != value) {
							typePermissionObject.AllowWrite = value;
							typePermissionObject.AllowDelete = value;
							typePermissionObject.AllowCreate = value;
						}
						break;
				}
			}
		}
		public void FindAndSetAssociationMemberPermission(SecuritySystemMemberPermissionsObject memberPermissionsBase, string operation, bool value) {
			if(!isBusy) {
				try {
					isBusy = true;
					SecuritySystemTypePermissionObject typePermissionObject = memberPermissionsBase.Owner as SecuritySystemTypePermissionObject;
					if(typePermissionObject != null) {
						SecuritySystemTypePermissionsObjectOwner typePermissionsObjectOwner = typePermissionObject.Owner as SecuritySystemTypePermissionsObjectOwner;
						if(!typePermissionsObjectOwner.Session.IsObjectsLoading) {
							if(typePermissionsObjectOwner.TypePermissions.Count(p => p.TargetType == typePermissionObject.TargetType) > 1) {
								return;
							}
							if(typePermissionsObjectOwner != null) {
								string[] membersName = memberPermissionsBase.Members.Split(ServerPermissionRequestProcessor.Delimiters, StringSplitOptions.RemoveEmptyEntries);
								ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(typePermissionObject.TargetType);
								foreach(IMemberInfo memberInfo in typeInfo.Members) {
									if(memberInfo.IsAssociation) {
										if(memberInfo.IsAggregated) {
											if(membersName.Contains(memberInfo.Name) && memberInfo.MemberType.IsGenericType && memberInfo.MemberType.GetGenericTypeDefinition().IsEquivalentTo(typeof(XPCollection<>))) {
												if(typePermissionObject.MemberPermissions.Count(p => p.Members.Contains(memberInfo.Name)) > 1) {
													continue;
												}
												SetAggregatedAssociationTypePermissions(typePermissionsObjectOwner, memberInfo.AssociatedMemberInfo.Owner.Type, operation, value);
											}
											continue;
										}
										if(membersName.Contains(memberInfo.Name)) {
											if(!String.IsNullOrEmpty(memberPermissionsBase.Criteria) || typePermissionObject.MemberPermissions.Count(p => p.Members.Contains(memberInfo.Name)) > 1) {
												continue;
											}
											SetAssociationMemberPermissions(typePermissionsObjectOwner, memberInfo.AssociatedMemberInfo.Owner.Type, memberInfo.AssociatedMemberInfo.Name, operation, value);
										}
									}
								}
							}
						}
					}
				}
				finally {
					isBusy = false;
				}
			}
		}
		private bool CheckAnyTypePermissions(SecuritySystemTypePermissionsObjectOwner typePermissionsObjectOwner, Type type, string operation) {
			IEnumerable<SecuritySystemTypePermissionObject> SecuritySystemTypePermissionObjects = typePermissionsObjectOwner.TypePermissions.Where(p => p.TargetType == type);
			foreach(SecuritySystemTypePermissionObject securitySystemTypePermissionObject in SecuritySystemTypePermissionObjects) {
				switch(operation) {
					case "Read":
						if(securitySystemTypePermissionObject.AllowRead) {
							return true;
						}
						break;
					case "Write":
						if(securitySystemTypePermissionObject.AllowWrite) {
							return true;
						}
						break;
				}
			}
			return false;
		}
		public void FindAndSetAssociationTypePermission(SecuritySystemTypePermissionObject typePermissions, string operation, bool value) {
			if(!isBusy) {
				try {
					isBusy = true;
					SecuritySystemTypePermissionsObjectOwner typePermissionsObjectOwner = typePermissions.Owner as SecuritySystemTypePermissionsObjectOwner;
					if(!typePermissions.Session.IsObjectsLoading) {
					if(typePermissionsObjectOwner.TypePermissions.Count(p => p.TargetType == typePermissions.TargetType) > 1) {
						return;
					}
					if(typePermissionsObjectOwner != null) {
						IEnumerable<IMemberInfo> membersInfo = GetAssociationShareType(typePermissions.TargetType);
						foreach(IMemberInfo memberInfo in membersInfo) {						  
								if(memberInfo.IsAggregated) {
									if(memberInfo.MemberType.IsGenericType && memberInfo.MemberType.GetGenericTypeDefinition().IsEquivalentTo(typeof(XPCollection<>))) {
										SetAggregatedAssociationTypePermissions(typePermissionsObjectOwner, memberInfo.AssociatedMemberInfo.Owner.Type, operation, value);
									}
								}
								else {
									SetAssociationMemberPermissions(typePermissionsObjectOwner, memberInfo.AssociatedMemberInfo.Owner.Type, memberInfo.AssociatedMemberInfo.Name, operation, value);
								}
							}
						}
					}
				}
				finally {
					isBusy = false;
				}
			}
		}
		private bool CheckPermissionsToAssociationObject(SecuritySystemTypePermissionsObjectOwner typePermissionsObjectOwner, Type type) {
			if(typePermissionsObjectOwner.TypePermissions.Where(p => p.TargetType == type).Count() > 0) {
				return true;
			}
			return false;
		}
		public void FindAndDeleteAssociationTypePermission(SecuritySystemRoleBase securitySystemRoleBase, SecuritySystemTypePermissionObject typePermissions) {
			if(!isBusy) {
				try {
					isBusy = true;
					SecuritySystemTypePermissionsObjectOwner typePermissionsObjectOwner = typePermissions.Owner as SecuritySystemTypePermissionsObjectOwner;
					if(typePermissionsObjectOwner == null) {
						typePermissionsObjectOwner = securitySystemRoleBase;
					}
					if(typePermissionsObjectOwner != null) {
						IEnumerable<IMemberInfo> membersInfo = GetAssociationShareType(typePermissions.TargetType);
						foreach(IMemberInfo memberInfo in membersInfo) {
							if(!typePermissions.Session.IsObjectsLoading) {
								SecuritySystemTypePermissionObject securitySystemTypePermissionObject = typePermissionsObjectOwner.TypePermissions.FirstOrDefault(p => p.TargetType == memberInfo.AssociatedMemberInfo.Owner.Type);
								if(securitySystemTypePermissionObject != null) {
									if(memberInfo.IsAggregated && memberInfo.MemberType.IsGenericType && memberInfo.MemberType.GetGenericTypeDefinition().IsEquivalentTo(typeof(XPCollection<>))) {
										securitySystemTypePermissionObject.AllowCreate = false;
										securitySystemTypePermissionObject.AllowRead = false;
										securitySystemTypePermissionObject.AllowWrite = false;
										securitySystemTypePermissionObject.AllowDelete = false;
										securitySystemTypePermissionObject.AllowNavigate = false;
									}
									else {
										SecuritySystemMemberPermissionsObject securitySystemMemberPermissionsObject = securitySystemTypePermissionObject.MemberPermissions.FirstOrDefault(p => p.Members == memberInfo.AssociatedMemberInfo.Name);
										if(securitySystemMemberPermissionsObject != null) {
											securitySystemMemberPermissionsObject.AllowRead = false;
											securitySystemMemberPermissionsObject.AllowWrite = false;
										}
									}
								}
							}
						}
					}
				}
				finally {
					isBusy = false;
				}
			}
		}
		private IEnumerable<IMemberInfo> GetAssociationShareType(Type type) {
			List<IMemberInfo> memberInfoList = new List<IMemberInfo>();
			ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(type);
			if(typeInfo != null) {
				foreach(IMemberInfo member in typeInfo.Members) {
					if(member.IsAssociation) {
						memberInfoList.Add(member);
					}
				}
			}
			return memberInfoList;
		}
	}
}
