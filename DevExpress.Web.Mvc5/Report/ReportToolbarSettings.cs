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

using System.Drawing;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.XtraReports.Web;
namespace DevExpress.Web.Mvc {
	public class ReportToolbarSettings : SettingsBase {
		internal ReportToolbarMenuStyle MenuStyle { get; private set; }
		public string ReportViewerName { get; set; }
		public Unit ItemSpacing { get { return MenuStyle.ItemSpacing; } set { MenuStyle.ItemSpacing = value; } }
		public Paddings Paddings { get { return MenuStyle.Paddings; } }
		public BackgroundImage SeparatorBackgroundImage { get { return MenuStyle.SeparatorBackgroundImage; } }
		public Color SeparatorColor { get { return MenuStyle.SeparatorColor; } set { MenuStyle.SeparatorColor = value; } }
		public Unit SeparatorHeight { get { return MenuStyle.SeparatorHeight; } set { MenuStyle.SeparatorHeight = value; } }
		public Unit SeparatorWidth { get { return MenuStyle.SeparatorWidth; } set { MenuStyle.SeparatorWidth = value; } }
		public Paddings SeparatorPaddings { get { return MenuStyle.SeparatorPaddings; } }
		public ReportToolbarImages Images { get { return (ReportToolbarImages)ImagesInternal; } }
		public bool ShowDefaultButtons { get; set; }
		public ReportToolbarItemCollection Items { get; private set; }
		public ReportToolbarClientSideEvents ClientSideEvents { get { return (ReportToolbarClientSideEvents)ClientSideEventsInternal; } }
		public ReportToolbarStyles Styles { get { return (ReportToolbarStyles)StylesInternal; } }
		public bool AccessibilityCompliant { get { return AccessibilityCompliantInternal; } set { AccessibilityCompliantInternal = value; } }
		public ReportToolbarEditorCaptionSettings CaptionSettings { get; private set; }
		public ReportToolbarSettings() {
			MenuStyle = new ReportToolbarMenuStyle();
			ShowDefaultButtons = true;
			Items = new ReportToolbarItemCollection();
			CaptionSettings = new ReportToolbarEditorCaptionSettings(null);
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new ReportToolbarClientSideEvents();
		}
		protected override ImagesBase CreateImages() {
			return new ReportToolbarImages(null);
		}
		protected override StylesBase CreateStyles() {
			return new ReportToolbarStyles(null);
		}
	}
}
