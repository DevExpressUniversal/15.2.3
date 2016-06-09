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
using DevExpress.Web;
namespace DevExpress.Web.Mvc {
	public class DockManagerSettings: SettingsBase {
		public DockManagerSettings() {
			FreezeLayout = false;
			SaveStateToCookies = false;
			SaveStateToCookiesID = String.Empty;
		}
		public object CallbackRouteValues { get; set; }
		public DockManagerClientSideEvents ClientSideEvents { get { return (DockManagerClientSideEvents)base.ClientSideEventsInternal; } }
		public new AppearanceStyleBase ControlStyle { get { return (AppearanceStyleBase)base.ControlStyle; } }
		public bool FreezeLayout { get; set; }
		public ImagesBase Images { get { return ImagesInternal; } }
		public bool SaveStateToCookies { get; set; }
		public string SaveStateToCookiesID { get; set; }
		public StylesBase Styles { get { return StylesInternal; } }
		public ASPxClientLayoutHandler ClientLayout { get { return ClientLayoutInternal; } set { ClientLayoutInternal = value; } }
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new DockManagerClientSideEvents();
		}
		protected override AppearanceStyleBase CreateControlStyle() {
			return new AppearanceStyleBase();
		}
		protected override ImagesBase CreateImages() {
			return new ImagesBase(null);
		}
		protected override StylesBase CreateStyles() {
			return new StylesBase(null);
		}
	}
}
