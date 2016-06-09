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

using System.Web.Mvc;
using System.Web.UI;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web.Mvc.Internal;
	public class CallbackPanelExtension: CollapsiblePanelExtension {
		public CallbackPanelExtension(CallbackPanelSettings settings)
			: base(settings) {
		}
		public CallbackPanelExtension(CallbackPanelSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected internal new MVCxCallbackPanel Control {
			get { return (MVCxCallbackPanel)base.Control; }
		}
		protected internal new CallbackPanelSettings Settings {
			get { return (CallbackPanelSettings)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			Control.Styles.CopyFrom(Settings.Styles);
			base.AssignInitialProperties();
			Control.CallbackRouteValues = Settings.CallbackRouteValues;
			Control.ClientSideEvents.Assign(Settings.ClientSideEvents);
			Control.EnableCallbackAnimation = Settings.EnableCallbackAnimation;
			Control.HideContentOnCallback = Settings.HideContentOnCallback;
			Control.Images.CopyFrom(Settings.Images);
			Control.SettingsLoadingPanel.Assign(Settings.SettingsLoadingPanel);
			Control.BeforeGetCallbackResult += Settings.BeforeGetCallbackResult;
			Control.Callback += Settings.CallbackInternal;
		}
		protected override Control GetCallbackResultControl() {
			return Control.GetCallbackResultControl();
		}
		protected override DevExpress.Web.ASPxWebControl CreateControl() {
			return new MVCxCallbackPanel();
		}
	}
}
