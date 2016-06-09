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
using System.Web.Mvc;
using System.Web.UI;
using DevExpress.Web;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web;
	using DevExpress.Web.Mvc.Internal;
	public class SplitterExtension : ExtensionBase {
		public SplitterExtension(SplitterSettings settings)
			: base(settings) {
		}
		public SplitterExtension(SplitterSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected internal new MVCxSplitter Control {
			get { return (MVCxSplitter)base.Control; }
		}
		protected internal new SplitterSettings Settings {
			get { return (SplitterSettings)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.AllowResize = Settings.AllowResize;
			Control.ClientSideEvents.Assign(Settings.ClientSideEvents);
			Control.ClientVisible = Settings.ClientVisible;
			Control.Images.CopyFrom(Settings.Images);
			Control.Orientation = Settings.Orientation;
			Control.FullscreenMode = Settings.FullscreenMode;
			Control.PaneMinSize = Settings.PaneMinSize;
			Control.Panes.Assign(Settings.Panes);
			Control.ResizingMode = Settings.ResizingMode;
			Control.SaveStateToCookies = Settings.SaveStateToCookies;
			Control.SaveStateToCookiesID = Settings.SaveStateToCookiesID;
			Control.SeparatorSize = Settings.SeparatorSize;
			Control.SeparatorVisible = Settings.SeparatorVisible;
			Control.ShowSeparatorImage = Settings.ShowSeparatorImage;
			Control.ShowCollapseForwardButton = Settings.ShowCollapseForwardButton;
			Control.ShowCollapseBackwardButton = Settings.ShowCollapseBackwardButton;
			Control.Styles.CopyFrom(Settings.Styles);
			Control.RightToLeft = Settings.RightToLeft;
			Control.ClientLayout += Settings.ClientLayout;
			Control.CustomJSProperties += Settings.CustomJSProperties;
		}
		protected override void AssignRenderProperties() {
			base.AssignRenderProperties();
			for(int i = 0; i < Control.Panes.Count; i++) {
				MVCxSplitterPane sourcePane = (i < Settings.Panes.Count) ? Settings.Panes[i] : null;
				AssignPaneContent(sourcePane, Control.Panes[i]);
			}
		}
		protected void AssignPaneContent(MVCxSplitterPane sourcePane, SplitterPane destinationPane) {
			if(sourcePane == null) return;
			destinationPane.Controls.Add(DevExpress.Web.Mvc.Internal.ContentControl.Create(sourcePane.Content, sourcePane.ContentMethod));
			for(int i = 0; i < destinationPane.Panes.Count; i++) {
				MVCxSplitterPane sourceSubPane = (sourcePane != null && i < sourcePane.Panes.Count) ? sourcePane.Panes[i] : null;
				AssignPaneContent(sourceSubPane, destinationPane.Panes[i]);
			}
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			Control.EnsureClientStateLoaded();
		}
		protected override DevExpress.Web.ASPxWebControl CreateControl() {
			return new MVCxSplitter();
		}
	}
}
