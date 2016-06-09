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
namespace DevExpress.XtraSpreadsheet.Services {
	public interface IRangeSecurityService {
		bool CheckAccess(string securityDescriptor);
		string CreateSecurityDescriptor(IEnumerable<DevExpress.Spreadsheet.EditRangePermission> permissions);
	}
}
#if !SL && !DOTNET
namespace DevExpress.XtraSpreadsheet.Services.Implementation {
	using System.Security.AccessControl;
	using System.Security.Principal;
	public class RangeSecurityService : IRangeSecurityService {
		public bool CheckAccess(string securityDescriptor) {
			try {
				if (string.IsNullOrEmpty(securityDescriptor))
					return false;
				RangeSecurity security = new RangeSecurity();
				security.SetSecurityDescriptorSddlForm(securityDescriptor);
				return security.CheckAccess();
			}
			catch {
				return false;
			}
		}
		public string CreateSecurityDescriptor(IEnumerable<DevExpress.Spreadsheet.EditRangePermission> permissions) {
			try {
				RangeSecurity security = new RangeSecurity();
				foreach (DevExpress.Spreadsheet.EditRangePermission permission in permissions) {
					try {
						NTAccount account = new NTAccount(permission.DomainName, permission.UserName);
						SecurityIdentifier sid = account.Translate(typeof(SecurityIdentifier)) as SecurityIdentifier;
						if (sid != null)
							security.AddAccessRule(sid, 0x1, permission.Deny ? AccessControlType.Deny : AccessControlType.Allow);
					}
					catch {
					}
				}
				SecurityIdentifier everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
				security.SetOwner(everyone);
				security.SetGroup(everyone);
				return security.GetSecurityDescriptorSddlForm(AccessControlSections.All);
			}
			catch {
				return String.Empty;
			}
		}
	}
	public enum RangeRights {
		EditWithoutPassword = 0x001,
	}
	public class RangeAccessRule : AccessRule {
		public RangeAccessRule(IdentityReference identity, int accessMask, AccessControlType accessType)
			: base(identity, accessMask, false, InheritanceFlags.None, PropagationFlags.None, accessType) {
		}
		public RangeRights AccessRights { get { return (RangeRights)AccessMask; } }
	}
	public class RangeSecurity : CommonObjectSecurity {
		public RangeSecurity()
			: base(false) {
		}
		public override Type AccessRuleType { get { return typeof(RangeAccessRule); } }
		public override Type AuditRuleType { get { return null; } }
		public override Type AccessRightType { get { return typeof(RangeRights); } }
		public override AccessRule AccessRuleFactory(IdentityReference identityReference, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags,
			PropagationFlags propagationFlags, AccessControlType type) {
			return new RangeAccessRule(identityReference, accessMask, type);
		}
		public void AddAccessRule(IdentityReference identityReference, int accessMask, AccessControlType type) {
			base.AddAccessRule(new RangeAccessRule(identityReference, accessMask, type));
		}
		public void RemoveAccessRule(RangeAccessRule rule) {
			base.RemoveAccessRule(rule);
		}
		public override AuditRule AuditRuleFactory(IdentityReference identityReference, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags flags) {
			return null;
		}
		public bool CheckAccess() {
			try {
				WindowsIdentity currentIdentity = WindowsIdentity.GetCurrent();
				WindowsPrincipal principal = new WindowsPrincipal(currentIdentity);
				AuthorizationRuleCollection collection = GetAccessRules(true, false, typeof(NTAccount));
				foreach (AuthorizationRule rule in collection) {
					if (!rule.IdentityReference.IsValidTargetType(typeof(SecurityIdentifier)))
						continue;
					SecurityIdentifier sid = rule.IdentityReference.Translate(typeof(SecurityIdentifier)) as SecurityIdentifier;
					if (sid == null || !principal.IsInRole(sid))
						continue;
					RangeAccessRule rangeRule = rule as RangeAccessRule;
					if (rangeRule != null) {
						RangeRights rights = rangeRule.AccessRights;
						if (rights == RangeRights.EditWithoutPassword)
							return rangeRule.AccessControlType == AccessControlType.Allow;
					}
				}
				return false;
			}
			catch {
				return false;
			}
		}
	}
}
#endif
