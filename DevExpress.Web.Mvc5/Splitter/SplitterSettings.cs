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
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Utils;
namespace DevExpress.Web.Mvc {
	public class SplitterSettings: SettingsBase {
		MVCxSplitterPaneCollection panes;
		public SplitterSettings() {
			this.panes = new MVCxSplitterPaneCollection();
			AllowResize = true;
			Orientation = Orientation.Horizontal;
			PaneMinSize = 40;
			ResizingMode = ResizingMode.Postponed;
			ShowSeparatorImage = true;
			SeparatorVisible = true;
		}
		public bool AllowResize { get; set; }
		public SplitterClientSideEvents ClientSideEvents { get { return (SplitterClientSideEvents)ClientSideEventsInternal; } }
		public bool ClientVisible { get { return ClientVisibleInternal; } set { ClientVisibleInternal = value; } }
		public new SplitterStyle ControlStyle { get { return (SplitterStyle)base.ControlStyle; } }
		public SplitterImages Images { get { return (SplitterImages)ImagesInternal; } }
		public Orientation Orientation { get; set; }
		public bool FullscreenMode { get; set; }
		public Unit PaneMinSize { get; set; }
		public MVCxSplitterPaneCollection Panes { get { return panes; } }
		public ResizingMode ResizingMode { get; set; }
		public DefaultBoolean RightToLeft { get { return RightToLeftInternal; } set { RightToLeftInternal = value; } }
		public bool SaveStateToCookies { get; set; }
		public string SaveStateToCookiesID { get; set; }
		public Unit SeparatorSize { get; set; }
		public bool SeparatorVisible { get; set; }
		public bool ShowSeparatorImage { get; set; }
		public bool ShowCollapseForwardButton { get; set; }
		public bool ShowCollapseBackwardButton { get; set; }
		public SplitterStyles Styles { get { return (SplitterStyles)StylesInternal; } }
		public ASPxClientLayoutHandler ClientLayout { get { return ClientLayoutInternal; } set { ClientLayoutInternal = value; } }
		public CustomJSPropertiesEventHandler CustomJSProperties { get; set; }
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new SplitterClientSideEvents();
		}
		protected override AppearanceStyleBase CreateControlStyle() {
			return new SplitterStyle();
		}
		protected override ImagesBase CreateImages() {
			return new SplitterImages(null);
		}
		protected override StylesBase CreateStyles() {
			return new SplitterStyles(null);
		}
	}
}
