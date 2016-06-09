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
using System.ComponentModel;
namespace DevExpress.Web.Mvc {
	using DevExpress.Utils;
	using DevExpress.Web;
	public class ValidationSummarySettings : SettingsBase {
		LinkStyle linkStyle;
		public ValidationSummarySettings() {
			this.linkStyle = new LinkStyle();
			ShowErrorAsLink = true;
			RightToLeft = DefaultBoolean.Default;
		}
		public ValidationSummaryClientSideEvents ClientSideEvents {
			get { return (ValidationSummaryClientSideEvents)ClientSideEventsInternal; }
		}
		public string HeaderText { get; set; }
		public LinkStyle LinkStyle { get { return linkStyle; } }
		public ValidationSummaryRenderMode RenderMode { get; set; }
		public DefaultBoolean RightToLeft { get; set; }
		public bool ShowErrorAsLink { get; set; }
		public ValidationSummaryStyles Styles {
			get { return (ValidationSummaryStyles)StylesInternal; }
		}
		public string ValidationGroup { get; set; }
		[Obsolete("This property is now obsolete. Use the editor's Properties.ValidationSettings.Display property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShowErrorsInEditors { get { return ShowErrorsInEditorsInternal; } set { ShowErrorsInEditorsInternal = value; } }
		protected internal bool ShowErrorsInEditorsInternal { get; set; }
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new ValidationSummaryClientSideEvents();
		}
		protected override ImagesBase CreateImages() {
			return null;
		}
		protected override StylesBase CreateStyles() {
			return new ValidationSummaryStyles(null);
		}
	}
}
