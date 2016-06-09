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
	using DevExpress.Web;
	public class LoadingPanelExtension: ExtensionBase {
		public LoadingPanelExtension(LoadingPanelSettings settings)
			: base(settings) {
		}
		public LoadingPanelExtension(LoadingPanelSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected internal new MVCxLoadingPanel Control {
			get { return (MVCxLoadingPanel)base.Control; }
		}
		protected internal new LoadingPanelSettings Settings {
			get { return (LoadingPanelSettings)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.ClientSideEvents.Assign(Settings.ClientSideEvents);
			Control.ContainerElementID = Settings.ContainerElementID;
			Control.Images.CopyFrom(Settings.Images);
			Control.ImagePosition = Settings.ImagePosition;
			Control.HorizontalOffset = Settings.HorizontalOffset;
			Control.Modal = Settings.Modal;
			Control.ShowImage = Settings.ShowImage;
			Control.Styles.CopyFrom(Settings.Styles);
			Control.Text = Settings.Text;
			Control.VerticalOffset = Settings.VerticalOffset;
			Control.RightToLeft = Settings.RightToLeft;
			Control.CustomJSProperties += Settings.CustomJSProperties;
			Control.ContainerElementResolve += Settings.ContainerElementResolve;
		}
		protected override void AssignRenderProperties() {
			base.AssignRenderProperties();
			Control.Template = ContentControlTemplate<TemplateContainerBase>.Create(
				Settings.TemplateContent, Settings.TemplateContentMethod, typeof(TemplateContainerBase));
		}
		protected override DevExpress.Web.ASPxWebControl CreateControl() {
			return new MVCxLoadingPanel();
		}
	}
}
