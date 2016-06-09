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
using System.Text;
using System.Web.UI;
using DevExpress.ExpressApp.Actions;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Templates.ActionContainers.Menu {
	public abstract class ClickableMenuActionItem : MenuActionItemBase, IClientClickScriptProvider {
		private bool processOnServer = true;
		public string ClientClickScript { get; set; }
		public bool ProcessOnServer {
			get { return processOnServer; }
			set { processOnServer = value; }
		}
		public ClickableMenuActionItem(ActionBase action)
			: base(action) {
		}
		protected override XafMenuItem CreateMenuItem() {
			return new XafMenuItem(this, this);
		}
		public override void SetClientClickHandler(XafCallbackManager callbackManager, string controlID) {
		}
		#region IClientClickScriptProvider Members
		public virtual string GetScript(XafCallbackManager callbackManager, string controlID, string indexPath) {
			return GetScript((IXafCallbackManager)callbackManager, controlID, indexPath);
		}
		internal string GetScript(IXafCallbackManager callbackManager, string controlID, string indexPath) {
			string result = ClientClickScript;
			if(!string.IsNullOrEmpty(result)) {
				result += "\n";
			}
			return result + (processOnServer ? callbackManager.GetScript(controlID, string.Format("'{0}'", indexPath), Action.GetFormattedConfirmationMessage(), IsPostBackRequired) : "");
		}
		#endregion
	}
}
