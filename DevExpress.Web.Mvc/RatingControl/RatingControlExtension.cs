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
using DevExpress.Web;
using DevExpress.Web.Mvc.Internal;
using DevExpress.Web.Mvc.UI;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web;
	public class RatingControlExtension : ExtensionBase {
		public RatingControlExtension(RatingControlSettings settings)
			: base(settings) {
		}
		public RatingControlExtension(RatingControlSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected internal new ASPxRatingControl Control {
			get { return (ASPxRatingControl)base.Control; }
		}
		protected internal new RatingControlSettings Settings {
			get { return (RatingControlSettings)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.ClientSideEvents.Assign(Settings.ClientSideEvents);
			Control.ClientVisible = Settings.ClientVisible;
			Control.ImageMapUrl = Settings.ImageMapUrl;
			Control.ItemCount = Settings.ItemCount;
			Control.ItemHeight = Settings.ItemHeight;
			Control.ItemWidth = Settings.ItemWidth;
			Control.FillPrecision = Settings.FillPrecision;
			Control.Titles = Settings.Titles;
			Control.ReadOnly = Settings.ReadOnly;
			Control.RightToLeft = Settings.RightToLeft;	
		}
		public RatingControlExtension Bind(decimal value) {
			Control.Value = value;
			return this;
		}
		public static decimal GetValue(string name) {
			RatingControlExtension extension = (RatingControlExtension)ExtensionManager.GetExtension(ExtensionType.RatingControl, name, ExtensionCacheMode.Request);
			extension.LoadPostData();
			return extension.Control.Value;
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxRatingControl();
		}
	}
}
