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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors.Designer.Utils;
using DevExpress.Utils.Extensions.Helpers;
using DevExpress.LookAndFeel;
namespace DevExpress.XtraPrinting.Design.RemoteDocumentSourceDesign.Views {
	[ToolboxItem(false)]
	public partial class ChooseDocumentSourceTypeView : UserControl, IChooseDocumentSourceTypeView {
		string IPageView.HeaderText {
			get { return "RemoteDocument source type"; }
		}
		string IPageView.DescriptionText {
			get { return "Select a report document source type"; }
		}
		public event EventHandler DocumentSourceTypeChanged;
		public RemoteDocumentSourceType DocumentSourceType {
			get {
				var checkedItem = galleryControl1.Gallery.GetCheckedItems().FirstOrDefault();
				if(checkedItem==null)
					throw new InvalidOperationException();
				return (RemoteDocumentSourceType)checkedItem.Value;
			}
			set {
				galleryControl1.Gallery.GetAllItems().First(x => (int)x.Value == (int)value).Checked = true;
			}
		}
		public ChooseDocumentSourceTypeView() {
			InitializeComponent();
			this.ParentChanged += OnParentChanged;
		}
		void OnParentChanged(object sender, EventArgs e) {
			var parentForm = this.FindForm();
			if(parentForm is ISupportLookAndFeel)
				galleryControl1.LookAndFeel.SetSkinStyle(((ISupportLookAndFeel)parentForm).LookAndFeel.SkinName);
		}
		void galleryControl1_Gallery_ItemCheckedChanged(object sender, XtraBars.Ribbon.GalleryItemEventArgs e) {
			DocumentSourceTypeChanged.SafeRaise(sender);
		}
	}
}
