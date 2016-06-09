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
using DevExpress.Utils;
using DevExpress.Web;
namespace DevExpress.Web.Mvc {
	public class RatingControlSettings : SettingsBase {
		public RatingControlSettings() {
			ItemCount = 5;
			ItemWidth = 19;
			ItemHeight = 19;
			FillPrecision = RatingControlItemFillPrecision.Half;
			Titles = string.Empty;
			ToolTip = string.Empty;
		}
		public int ItemCount { get; set; }
		public bool ReadOnly { get; set; }
		public int ItemWidth { get; set; }
		public int ItemHeight { get; set; }
		public RatingControlItemFillPrecision FillPrecision { get; set; }
		public string Titles { get; set; }
		public string ImageMapUrl { get; set; }
		public RatingControlClientSideEvents ClientSideEvents { get { return (RatingControlClientSideEvents)ClientSideEventsInternal; } }
		public bool ClientVisible { get { return ClientVisibleInternal; } set { ClientVisibleInternal = value; } }
		public DefaultBoolean RightToLeft { get { return RightToLeftInternal; } set { RightToLeftInternal = value; } }
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new RatingControlClientSideEvents();
		}
		protected override ImagesBase CreateImages() {
			return new ImagesBase(null);
		}
		protected override StylesBase CreateStyles() {
			return new StylesBase(null);
		}
	}
}
