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
namespace DevExpress.ExpressApp.Security {
	public class CreateCustomLogonParametersEventArgs : EventArgs {
		private object logonParameters;
		private bool askLogonParametersViaUI = false;
		public object LogonParameters {
			get { return logonParameters; }
			set { logonParameters = value; }
		}
		public bool AskLogonParametersViaUI {
			get { return askLogonParametersViaUI; }
			set { askLogonParametersViaUI = value; }
		}
	}
	[ToolboxItemFilter("Xaf", ToolboxItemFilterType.Require)]
	[ToolboxBitmap(typeof(DevExpress.ExpressApp.Security.AuthenticationBase), "Resources.Toolbox_Authentication_AuthenticationBase.ico")]
	public abstract class AuthenticationBase : Component{
		private object logonParameters;
		private bool logonParametersQuerried = false;
		private ISecurityStrategyBase security;
		protected internal ISecurityStrategyBase Security {
			get { return security; }
			set { security = value; }
		}
		public virtual void SetLogonParameters(object logonParameters) { } 
		public virtual object Authenticate(IObjectSpace objectSpace) {
			return null;
		}
		public virtual void Logoff() { } 
		public virtual void ClearSecuredLogonParameters() { }
		public virtual bool IsSecurityMember(object theObject, string memberName) {
			return IsSecurityMember(theObject.GetType(), memberName);
		}
		public virtual bool IsSecurityMember(Type type, string memberName) {
			return false;
		}
		[Category("Behavior")]
		[Browsable(false)]
		public virtual object LogonParameters { 
			get {
				if(!logonParametersQuerried) {
					CreateCustomLogonParametersEventArgs argsCustom = new CreateCustomLogonParametersEventArgs();
					if (CreateCustomLogonParameters != null) {
						CreateCustomLogonParameters(this, argsCustom);
					}
					logonParameters = argsCustom.LogonParameters;
					logonParametersQuerried = true;
				}
				return logonParameters;
			}
		}
		[Category("Behavior")]
		public virtual bool AskLogonParametersViaUI { get { return false; } }
		[Category("Design")]
		public virtual string Name { get { return GetType().Name;  } }
		[Category("Behavior")]
		public abstract bool IsLogoffEnabled { get; }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Type UserType {
			get { return null; }
			set {
			}
		}
		public virtual IList<Type> GetBusinessClasses() {
			return new Type[0];
		}
		public event EventHandler<CreateCustomLogonParametersEventArgs> CreateCustomLogonParameters;
	}
	[ToolboxItem(false)]
	public class AuthenticationDummy : AuthenticationBase {
		public override object Authenticate(IObjectSpace objectSpace) {
			return null;
		}
		public override bool IsLogoffEnabled {
			get { return false; }
		}
	}
}
