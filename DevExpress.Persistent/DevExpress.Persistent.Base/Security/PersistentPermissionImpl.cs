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
using System.Security;
namespace DevExpress.Persistent.Base.Security {
	public class PersistentPermissionImpl {
		private IPermission permission;
		private IPermission GetPermissionFromXml(string permissionXml) {
			try {
				if(!String.IsNullOrEmpty(permissionXml)) {
					SecurityElement securityElement = SecurityElement.FromString(permissionXml);
					string typeName = securityElement.Attribute("class");
					string assemblyName = securityElement.Attribute("assembly");
					IPermission result = (IPermission)ReflectionHelper.CreateObject(typeName);
					result.FromXml(securityElement);
					return result;
				}
			}
			catch(Exception e) {
				Tracing.Tracer.LogError(e);
			}
			return null;
		}
		public IPermission Permission {
			get { return permission; }
			set { permission = value; }
		}
		public override string ToString() {
			if(permission != null) {
				return permission.ToString();
			}
			else {
				return "Permission is null";
			}
		}
		public string SerializedPermission {
			get {
				if(permission != null) {
					return permission.ToXml().ToString();
				}
				else {
					return "";
				}
			}
			set {
				try {
					permission = GetPermissionFromXml(value);
				}
				catch(Exception e) {
					System.Diagnostics.Trace.WriteLine(e.ToString());
				}
			}
		}
	}
}
