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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using DevExpress.Utils.Design;
namespace DevExpress.Web.Design {
	public partial class IconEditorForm : XtraForm, ITopFormOptionsProvider {
		static IconEditorForm() {
			SkinManager.EnableFormSkins();
		}
		public IconEditorForm() {
			InitializeComponent();
			LoadImageGallery();
		}
		public object EditValue { get { return ImageGalleryControl.EditValue; } }
		IconEditorControl ImageGalleryControl { get { return dxImageGalleryControl; } }
		public void InitServices(IServiceProvider serviceProvider) {
			ImageGalleryControl.InitServices(serviceProvider, null, this);
		}
		void LoadImageGallery() {
			DXImageGalleryStorage.Default.LoadAsync();
		}
		#region ITopFormOptionsProvider
		DXImageGalleryResourceType ITopFormOptionsProvider.ResourceType { get { return DXImageGalleryResourceType.Form; } }
		bool ITopFormOptionsProvider.AllowMultiSelect { get { return false; } }
		Size? ITopFormOptionsProvider.DesiredImageSize { get { return null; } }
		bool ITopFormOptionsProvider.IsAsync { get { return false; } }
		#endregion
		#region Disposing
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		#endregion
	}
}
