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
using DevExpress.Persistent.Base;
using System.Runtime.Serialization;
using System.Text;
namespace DevExpress.ExpressApp.Security {
	public abstract class PermissionBase : IPermission {
		public abstract IPermission Copy();
		public void Demand() {
			SecuritySystem.Demand(this);
		}
		public virtual IPermission Intersect(IPermission target) {
			return target;
		}
		public virtual bool IsSubsetOf(IPermission target) {
			if((target == null) || (GetType() != target.GetType())) {
				return false;
			}
			else {
				return true;
			}
		}
		public virtual IPermission Union(IPermission target) {
			return target;
		}
		public virtual void FromXml(SecurityElement e) { }
		public virtual SecurityElement ToXml() {
			SecurityElement result = new SecurityElement("IPermission");
			result.AddAttribute("class", GetType().FullName);
			result.AddAttribute("assembly", ReflectionHelper.GetAssemblyName(GetType().Module.Assembly));
			result.AddAttribute("version", ReflectionHelper.GetAssemblyVersion(GetType().Module.Assembly).ToString());
			return result;
		}
		public static string ToXmlString(IPermission permission) {
			return permission.ToXml().ToString();
		}
		public static IPermission FromXmlString(string xmlString) {
			SecurityElement element = SecurityElement.FromString(xmlString);
			string className = element.Attribute("class");
			Type type = ReflectionHelper.FindType(className);
			if(type == null) {
				return null;
			}
			IPermission result = (IPermission)ReflectionHelper.CreateObject(type);
			result.FromXml(element);
			return result;
		}
		public static PermissionBase operator +(PermissionBase left, PermissionBase right) {
			return (PermissionBase)left.Union(right);
		}
		public PermissionBase() { }
		public virtual string GetHashString() {
			return ToXml().ToString();
		}
	}
}
