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
using System.Security;
using System.Security.Permissions;
using System.Collections.Generic;
#if !DXWINDOW
using DevExpress.XtraPrinting.Native;
#endif
#if DXWINDOW
namespace DevExpress.Internal.DXWindow.Data {
#else
namespace DevExpress.Data.Helpers {
#endif
	public static class SecurityHelper {
		static bool forcePartialTrustMode;
		public static bool IsPartialTrust {
			get {
				return forcePartialTrustMode || !IsPermissionGranted(new ReflectionPermission(ReflectionPermissionFlag.MemberAccess));
			}
		}
		public static bool ForcePartialTrustMode { get { return forcePartialTrustMode; } set { forcePartialTrustMode = value; } }
		public static bool IsWeakRefAvailable {
			get { return IsPermissionGranted(new WeakReferencePermission()); }
		}
#if !DXWINDOW
		public static bool IsUnmanagedCodeGrantedAndHasZeroHwnd {
			get { return SecurityHelper.IsPermissionGranted(new SecurityPermission(SecurityPermissionFlag.UnmanagedCode)) && GraphicsHelper.CanCreateFromZeroHwnd; }
		}
		public static bool IsUnmanagedCodeGrantedAndCanUseGetHdc {
			get { return SecurityHelper.IsPermissionGranted(new SecurityPermission(SecurityPermissionFlag.UnmanagedCode)) && GraphicsHelper.CanUseGetHdc; }
		}
#endif
		[ThreadStatic]
		static PermissionCheckerSet permissionCheckerSet;  
		public static bool IsPermissionGranted(IPermission permission) {
			if(permissionCheckerSet == null)
				permissionCheckerSet = new PermissionCheckerSet();
			return permissionCheckerSet.IsGranted(permission);
		}
		public static void ForceRecheckPermissions() {
			permissionCheckerSet = null;
		}
	}
	class WeakReferencePermission : IPermission {
		#region IPermission Members
		public IPermission Copy() {
			throw new NotImplementedException();
		}
		public void Demand() {
			new WeakReference(new object());
		}
		public IPermission Intersect(IPermission target) {
			throw new NotImplementedException();
		}
		public bool IsSubsetOf(IPermission target) {
			return target is WeakReferencePermission;
		}
		public IPermission Union(IPermission target) {
			throw new NotImplementedException();
		}
		#endregion
		#region ISecurityEncodable Members
		public void FromXml(SecurityElement e) {
			throw new NotImplementedException();
		}
		public SecurityElement ToXml() {
			throw new NotImplementedException();
		}
		#endregion
	}
	public class PermissionChecker {
		bool? isPermissionGranted;
		IPermission permission;
		public IPermission Permission {
			get { return permission; }
		}
		public bool IsPermissionGranted {
			get {
				if(!isPermissionGranted.HasValue)
					isPermissionGranted = IsPermissionGrantedCore(permission);
				return isPermissionGranted.Value;
			}
		}
		public PermissionChecker(IPermission permission) {
			this.permission = permission;
		}
		bool IsPermissionGrantedCore(IPermission permission) {
			try {
				permission.Demand();
				return true;
			} catch(SecurityException) {
				return false;
			}
		}
	}
	class PermissionCheckerSet {
		List<PermissionChecker> checkers = new List<PermissionChecker>();
		public bool IsGranted(IPermission permission) {
			PermissionChecker checker = GetChecker(permission);
			if(checker == null) {
				checker = new PermissionChecker(permission);
				checkers.Add(checker);
			}
			return checker.IsPermissionGranted;
		}
		PermissionChecker GetChecker(IPermission permission) {
			foreach(PermissionChecker checker in checkers) {
				if(checker.Permission.GetType() == permission.GetType() && permission.IsSubsetOf(checker.Permission))
					return checker;
			}
			return null;
		}
	}
}
