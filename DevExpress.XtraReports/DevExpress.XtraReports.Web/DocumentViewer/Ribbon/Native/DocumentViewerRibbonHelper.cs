#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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
using System.Collections.Generic;
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.XtraReports.Web.Localization;
namespace DevExpress.XtraReports.Web.DocumentViewer.Ribbon.Native {
	public static class DocumentViewerRibbonHelper {
		public static ASPxRibbon GetRibbonControl(object owner) {
			RibbonItemBase item = owner as RibbonItemBase;
			if(item != null)
				return GetRibbonControl(item.Ribbon);
			else {
				RibbonGroup group = owner as RibbonGroup;
				if(group != null)
					return group.Ribbon;
			}
			return owner as ASPxRibbon;
		}
		public static ASPxRibbon GetRibbonControl(Page page, string ribbonControlID) {
			if(!String.IsNullOrEmpty(ribbonControlID) && page != null) {
				Control control = RibbonHelper.LookupRibbonControl(page, ribbonControlID.Trim(' '));
				if(control != null)
					return control as ASPxRibbon;
				else
					throw new InvalidOperationException(string.Format(ASPxReportsLocalizer.GetString(ASPxReportsStringId.DocumentViewer_ExternalRibbonNotFound_Error), ribbonControlID));
			}
			return null;
		}
		public static ItemImagePropertiesBase GetRibbonItemImageProperty(object owner, string imageName, string size) {
			ASPxRibbon ribbon = GetRibbonControl(owner);
			ItemImageProperties properties = new ItemImageProperties();
			properties.CopyFrom(RibbonHelper.GetRibbonImageProperties(ribbon, DocumentViewerRibbonImages.RibbonDVSpriteName,
					delegate(ISkinOwner skinOwner) { return new DocumentViewerRibbonImages(skinOwner, ribbon.Images.IconSet); }, imageName, size));
			return properties;
		}
	}
}
