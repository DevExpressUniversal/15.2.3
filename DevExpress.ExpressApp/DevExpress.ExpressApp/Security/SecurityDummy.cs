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
using System.Security;
namespace DevExpress.ExpressApp.Security {
	sealed class SecurityDummy : ISecurity {
		public Boolean IsGranted(IPermission permission) {
			return true;
		}
		public Type GetModuleType() {
			return null;
		}
		public IList<Type> GetBusinessClasses() {
			return Type.EmptyTypes;
		}
		public void Logon(IObjectSpace objectSpace) { }
		public void Logoff() { }
		public void ClearSecuredLogonParameters() { }
		public void ReloadPermissions() { }
		public Type UserType { get { return null; } }
		public Object User { get { return null; } }
		public String UserName { get { return ""; } }
		public Object UserId { get { return null; } }
		public Object LogonParameters { get { return null; } }
		public Boolean NeedLogonParameters { get { return false; } }
		public Boolean IsAuthenticated { get { return true; } }
		public Boolean IsLogoffEnabled { get { return false; } }
		public IObjectSpace LogonObjectSpace { get { return null; } }
	}
}
