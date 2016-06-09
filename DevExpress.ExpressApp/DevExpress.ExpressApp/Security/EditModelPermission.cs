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
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Security;
using System.Runtime.Serialization;
namespace DevExpress.ExpressApp.Security {
	public enum ModelAccessModifier { Deny, Allow }
	[DomainComponent]
	public class EditModelPermission : PermissionBase {
		private static bool alwaysGranted;
		private ModelAccessModifier modifier;
		public EditModelPermission() : base() { }
		public EditModelPermission(ModelAccessModifier modifier)
			: this() {
			this.modifier = modifier;
		}
		public override IPermission Copy() {
			return new EditModelPermission(modifier);
		}
		public override IPermission Intersect(IPermission target) {
			IPermission result = null;
			EditModelPermission editModelPermission = target as EditModelPermission;
			if(editModelPermission == null) {
				throw new ArgumentException(
					string.Format("Incorrect permission is passed: '{0}' instead of '{1}'",
					target.GetType(), GetType()));
			}
			if(alwaysGranted) {
				result = new EditModelPermission(ModelAccessModifier.Allow);
			}
			else if(editModelPermission.modifier == modifier) {
				result = new EditModelPermission(modifier);
			}
			return result;
		}
		public override IPermission Union(IPermission target) {
			IPermission result = Intersect(target);
			if(result == null) {
				result = new EditModelPermission(ModelAccessModifier.Deny);
			}
			return result;
		}
		public override void FromXml(SecurityElement e) {
			modifier = (ModelAccessModifier)Enum.Parse(typeof(ModelAccessModifier), e.Attributes["modifier"].ToString());
		}
		public override string ToString() {
			EnumDescriptor enumDescriptor = new EnumDescriptor(typeof(ModelAccessModifier));
			return CaptionHelper.GetClassCaption(GetType().FullName) + " (" + enumDescriptor.GetCaption(Modifier) + ")";
		}
		public override SecurityElement ToXml() {
			SecurityElement result = base.ToXml();
			result.AddAttribute("modifier", modifier.ToString());
			return result;
		}
		public override bool IsSubsetOf(IPermission target) {
			return Intersect(target) != null;
		}
		public ModelAccessModifier Modifier {
			get { return modifier; }
			set { modifier = value; }
		}
		public static bool AlwaysGranted {
			get { return alwaysGranted; }
			set { alwaysGranted = value; }
		}
	}
}
