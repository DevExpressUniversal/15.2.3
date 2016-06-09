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
	public class DockPanelExtension : PopupControlExtensionBase {
		public DockPanelExtension(DockPanelSettings settings)
			: base(settings) {
		}
		public DockPanelExtension(DockPanelSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected internal new MVCxDockPanel Control {
			get { return (MVCxDockPanel)base.Control; }
		}
		protected internal new DockPanelSettings Settings {
			get { return (DockPanelSettings)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			Control.AllowedDockState = Settings.AllowedDockState;
			Control.CallbackRouteValues = Settings.CallbackRouteValues;
			Control.ClientSideEvents.Assign(Settings.ClientSideEvents);
			Control.ForbiddenZones.Assign(Settings.ForbiddenZones);
			Control.Images.CopyFrom(Settings.Images);
			Control.LoadContentViaCallback = Settings.LoadContentViaCallback;
			Control.OwnerZoneUID = Settings.OwnerZoneUID;
			Control.PanelUID = Settings.PanelUID;
			Control.Styles.CopyFrom(Settings.Styles);
			Control.VisibleIndex = Settings.VisibleIndex;
			base.AssignInitialProperties();
		}
		protected override Control GetCallbackResultControl() {
			return Control.GetCallbackResultControl();
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			Control.SetupRelationWithZone();
			Control.EnsurePostDataLoaded();
			Control.EnsureClientStateLoaded();
		}
		protected override DevExpress.Web.ASPxWebControl CreateControl() {
			return new MVCxDockPanel();
		}
	}
}
