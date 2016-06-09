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

using DevExpress.Web;
using DevExpress.XtraReports.Web.DocumentViewer.Ribbon.Native;
namespace DevExpress.XtraReports.Web.DocumentViewer.Ribbon {
	public class DocumentViewerRibbonDropDownButtonItemBase : RibbonDropDownButtonItem {
		string commandId = null;
		protected string CommandID {
			get { return commandId ?? (commandId = RibbonCommandService.GetCommandID(ImageResourceName)); }
		}
		protected virtual string ImageResourceName {
			get { return string.Empty; }
		}
		protected virtual string DefaultText { get { return string.Empty; } }
		protected virtual string DefaultToolTip { get { return string.Empty; } }
		protected virtual RibbonItemSize DefaultItemSize { get { return RibbonItemSize.Large; } }
		protected virtual RibbonDropDownButtonCollection DefaultCommands { get { return null; } }
		protected override RibbonDropDownButtonCollection GetItems() {
			return DefaultCommands != null && base.Items.Count == 0 ? DefaultCommands : base.GetItems();
		}
		protected override ItemImagePropertiesBase GetSmallImage() {
			if(!base.GetSmallImage().IsEmpty)
				return base.GetSmallImage();
			var properties = DocumentViewerRibbonHelper.GetRibbonItemImageProperty(this, ImageResourceName, string.Empty);
			properties.CopyFrom(SmallImage);
			return properties;
		}
		protected override ItemImagePropertiesBase GetLargeImage() {
			if(!base.GetLargeImage().IsEmpty)
				return base.GetLargeImage();
			var properties = DocumentViewerRibbonHelper.GetRibbonItemImageProperty(this, ImageResourceName, RibbonCommandService.LargeSuffix);
			properties.CopyFrom(LargeImage);
			return properties;
		}
		protected override string GetName() {
			return CommandID;
		}
		protected override RibbonItemSize GetSize() {
			return DefaultItemSize;
		}
		protected override string GetText() {
			return string.IsNullOrEmpty(base.Text) ? DefaultText : base.GetText();
		}
		protected override string GetToolTip() {
			return string.IsNullOrEmpty(base.ToolTip) ? DefaultToolTip : base.GetToolTip();
		}
	}
}
