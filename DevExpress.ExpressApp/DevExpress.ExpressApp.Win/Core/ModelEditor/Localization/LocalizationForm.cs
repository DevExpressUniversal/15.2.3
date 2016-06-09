#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.ExpressApp.DC;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	[ToolboxItem(false)]
	public class LocalizationForm : DetailViewForm {
		public const string Title = "eXpressApp Framework Localization";
		private GridListEditor gridListEditor;
		public LocalizationForm() {
			gridListEditor = new GridListEditorForLocalizationForm(null);
			gridListEditor.CreateControls();
			Text = Title;
			MainMenuBar.Visible = false;
			XtraGrid.GridControl gridControl = gridListEditor.Grid;
			gridControl.Dock = DockStyle.Fill;
			((Control)ViewSiteControl).Controls.Add(gridControl);
			Image LocalizationImage = ImageLoader.Instance.GetImageInfo("BO_Localization_32x32").Image;
			if (LocalizationImage != null) {
				this.Icon = Icon.FromHandle(new Bitmap(LocalizationImage).GetHicon());
			}
		}
		public XtraGrid.GridControl GridControl {
			get { return gridListEditor.Grid; }
		}
		public GridListEditor GridListEditor {
			get { return gridListEditor; }
		}
	}
	public class GridListEditorForLocalizationForm : GridListEditor {
		public GridListEditorForLocalizationForm(DevExpress.ExpressApp.Model.IModelListView model)
			: base(model) {
		}
		public void SetObjectTypeInfo(ITypeInfo typeInfo) {
			ObjectTypeInfo = typeInfo;
		}
	}
}
