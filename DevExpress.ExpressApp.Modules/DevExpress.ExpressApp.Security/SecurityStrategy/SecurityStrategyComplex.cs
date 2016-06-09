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
using System.Drawing;
using System.ComponentModel.Design;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Design;
using DevExpress.ExpressApp.Security.Strategy;
namespace DevExpress.ExpressApp.Security {
	public class CustomInitializeNewUserEventArgs : HandledEventArgs {
		public CustomInitializeNewUserEventArgs(IObjectSpace objectSpace, object user) {
			this.ObjectSpace = objectSpace;
			this.User = user;
		}
		public IObjectSpace ObjectSpace { get; private set; }
		public object User { get; private set; }
	}
	public interface ICanInitialize {
		void Initialize(IObjectSpace objectSpace, SecurityStrategyComplex security);
	}
	[DXToolboxItem(true)] 
	[DevExpress.Utils.ToolboxTabName(DevExpress.ExpressApp.XafAssemblyInfo.DXTabXafSecurity)]
	[Designer("DevExpress.ExpressApp.Security.Design.SecurityStrategyComplexDesigner, DevExpress.ExpressApp.Security.Design" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(IDesigner))]
	[ToolboxBitmap(typeof(DevExpress.ExpressApp.Security.SecurityStrategyComplex), "Resources.Toolbox_SecurityStrategyComplex.ico")]
	public class SecurityStrategyComplex : SecurityStrategy, ISecurityComplex, IRoleTypeProvider {
		private Type roleType;
		private string newUserRoleName = AdministratorRoleName;
		public override IList<Type> GetBusinessClasses() {
			List<Type> result = new List<Type>(base.GetBusinessClasses());
			if(RoleType != null && !result.Contains(RoleType)) {
				result.Add(RoleType);
			}
			return result;
		}
		protected override void InitializeNewUserCore(IObjectSpace objectSpace, object user) {
			Guard.ArgumentNotNull(user, "user");
			Guard.ArgumentNotNull(objectSpace, "objectSpace");
			CustomInitializeNewUserEventArgs args = new CustomInitializeNewUserEventArgs(objectSpace, user);
			if(CustomInitializeNewUser != null) {
				CustomInitializeNewUser(this, args);
			}
			if(!args.Handled) {
				if(user is ICanInitialize) {
					((ICanInitialize)user).Initialize(objectSpace, this);
				}
			}
		}
		static SecurityStrategyComplex() {
		}
		public SecurityStrategyComplex() {
		}
		public SecurityStrategyComplex(Type userType, Type roleType, AuthenticationBase authentication)
			: base(userType, authentication) {
			this.roleType = roleType;
		}
		[TypeConverter(typeof(BusinessClassTypeConverter<ISecuritySystemRole>))]
		public Type RoleType {
			get { return roleType; }
			set {
				roleType = value;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string NewUserRoleName { get {return newUserRoleName;} set {newUserRoleName = value;} }
		public event EventHandler<CustomInitializeNewUserEventArgs> CustomInitializeNewUser;
	}
}
