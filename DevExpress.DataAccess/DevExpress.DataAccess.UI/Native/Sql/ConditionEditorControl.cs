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
using DevExpress.Data.Filtering;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
namespace DevExpress.DataAccess.UI.Native.Sql {
	public partial class ConditionEditorControl : XtraUserControl {
		protected static void DisposeImageNotNull(ref Image image) {
			if (image != null) {
				image.Dispose();
				image = null;
			}
		}
		protected const int topMargin = 5;
		protected const int leftMargin = 5;
		protected const int horizontalSpacing = 5;
		protected const int verticalSpacing = 0;
		protected readonly Color defaultOperatorColor = Color.Green;
		protected Image addNormal, addHover, removeNormal, removeHover;
		protected Dictionary<BinaryOperatorType, Image[]> conditionOperationsImages = new Dictionary<BinaryOperatorType, Image[]>();
		ImageCollection nodeImages;
		protected ImageCollection NodeImages {
			get {
				if(this.nodeImages == null)
					this.nodeImages = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraEditors.FilterEditor.Images.NodeImages.png", typeof(FilterControl).Assembly, new Size(13, 13), Color.Magenta);
				return this.nodeImages;
			}
		}
		protected ConditionEditorControl() {
			InitializeComponent();
			this.barAndDockingController1.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			LookAndFeel.StyleChanged += LookAndFeel_StyleChanged;
			this.addHover = NodeImages.Images[5];
			this.addNormal = ImageHelper.CreateBlendBitmap(this.addHover);
			this.removeHover = NodeImages.Images[1];
			this.removeNormal = ImageHelper.CreateBlendBitmap(this.removeHover);			
			UpdateSkinColors();
		}
		protected internal virtual void AlignItems() {
		}
		protected virtual void CreateBitmaps(Color operatorColor, Color foreColor) {
		}
		protected virtual void CreateObjectNames() {
		}
		protected virtual void UpdateSkinColors() {
			Color backColor = CommonSkins.GetSkin(LookAndFeel).Colors.GetColor("Window");
			Color foreColor = CommonSkins.GetSkin(LookAndFeel).Colors.GetColor("MenuText");
			this.scrollableControl.BackColor = backColor;
			this.panelControls.BackColor = backColor;
			BackColor = new SkinElementInfo(CommonSkins.GetSkin(LookAndFeel)[CommonSkins.SkinTextBorder]).Element.Border.Top;
			Color operatorColor = EditorsSkins.GetSkin(LookAndFeel).Colors.GetColor(EditorsSkins.SkinFilterControlOperatorTextColor);
			if(operatorColor.IsEmpty)
				operatorColor = this.defaultOperatorColor;
			CreateBitmaps(operatorColor, foreColor);
		}
		protected virtual void CreateItems() {
		}
		protected PopupMenu CreatePopupMenu() {
			PopupMenu menu = new PopupMenu(this.barManager);
			this.components.Add(menu);
			return menu;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				DisposeImageNotNull(ref this.addHover);
				DisposeImageNotNull(ref this.addNormal);
				DisposeImageNotNull(ref this.removeHover);
				DisposeImageNotNull(ref this.removeNormal);
				foreach(Image[] images in this.conditionOperationsImages.Values) {
					DisposeImageNotNull(ref images[0]);
					DisposeImageNotNull(ref images[1]);
				}
				if(this.components != null)
					this.components.Dispose();
			}
			base.Dispose(disposing);
		}		
		protected void LookAndFeel_StyleChanged(object sender, EventArgs e) {
			UpdateSkinColors();
		}
	}
	public class IncompleteConditionException : Exception {
		public IncompleteConditionException(string message) : base(message) {
		}
	}
}
