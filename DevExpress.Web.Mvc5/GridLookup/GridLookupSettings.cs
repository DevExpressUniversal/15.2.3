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

using DevExpress.Web;
using System;
namespace DevExpress.Web.Mvc {
	public class GridLookupSettings : EditorSettings {
		public GridLookupSettings() {
			GridViewProperties = new MVCxGridViewProperties(GridViewSettings);
		}
		public AnimationType AnimationType { get; set; }
		public MVCxGridViewProperties GridViewProperties { get; private set; }
		public GridViewClientSideEvents GridViewClientSideEvents { get { return GridViewSettings.ClientSideEvents; } }
		public string KeyFieldName { get { return GridViewSettings.KeyFieldName; } set { GridViewSettings.KeyFieldName = value; } }
		public MVCxGridViewCommandColumn CommandColumn { get { return GridViewSettings.CommandColumn; } }
		public MVCxGridViewColumnCollection Columns { get { return GridViewSettings.Columns; } }
		public GridViewImages GridViewImages { get { return GridViewSettings.Images; } }
		public GridViewEditorImages GridViewImagesEditors { get { return GridViewSettings.ImagesEditors; } }
		public FilterControlImages GridViewImagesFilterControl { get { return GridViewSettings.ImagesFilterControl; } }
		public GridViewStyles GridViewStyles { get { return GridViewSettings.Styles; } }
		public GridViewPagerStyles GridViewStylesPager { get { return GridViewSettings.StylesPager; } }
		public GridViewEditorStyles GridViewStylesEditors { get { return GridViewSettings.StylesEditors; } }
		public FilterControlStyles GridViewStylesFilterControl { get { return GridViewSettings.StylesFilterControl; } }
		public GridViewPopupControlStyles GridViewStylesPopup { get { return GridViewSettings.StylesPopup; } }
		public new MVCxGridLookupProperties Properties { get { return (MVCxGridLookupProperties)base.Properties; } }
		public bool ShowModelErrors { get; set; }
		public EventHandler DataBinding { get; set; }
		public EventHandler DataBound { get; set; }
		GridViewSettings gridViewSettings;
		protected internal GridViewSettings GridViewSettings {
			get {
				if(gridViewSettings == null)
					gridViewSettings = new GridViewSettings();
				return gridViewSettings;
			}
		}
		protected override EditPropertiesBase CreateProperties() {
			return new MVCxGridLookupProperties();
		}
	}
}
