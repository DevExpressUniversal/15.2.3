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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Layout.Designer;
namespace DevExpress.XtraGrid.Views.Layout.Customization {
	public class LayoutViewCustomizationForm : XtraForm {
		LayoutView ownerView =null;
		LayoutViewCustomizer customizer = null;
		public LayoutViewCustomizationForm(LayoutView view) {
			this.ownerView = view;
			this.AllowFormSkin = true;
			FormBorderStyle = FormBorderStyle.SizableToolWindow;
			ShowInTaskbar = false;
			StartPosition = FormStartPosition.CenterScreen;
			RightToLeft = view.IsRightToLeft ? RightToLeft.Yes : RightToLeft.No;
			Customizer.Parent = this;
			Customizer.Dock = DockStyle.Fill;
			Customizer.InitFrame(View, null, null);
			this.Text = Customizer.Localizer.StringByID(CustomizationStringID.CustomizationFormCaption);
			MinimumSize = view.ScaleSize(new Size(500, 400));
			int iCardWidth = Math.Max(view.CardMinSize.Width, view.TemplateCard.Width);
			int iCardHeignt = Math.Max(view.CardMinSize.Height, view.TemplateCard.Height);
			Size = view.ScaleSize(new Size(300 + 200 + Math.Min(iCardWidth, 400), 250 + Math.Min(iCardHeignt, 400)));
		}
		protected LayoutView View { get { return ownerView; } }
		protected internal LayoutViewCustomizer Customizer {
			get {
				if(customizer==null) customizer = CreateCustomizer();
				return customizer;
			}
		}
		protected virtual LayoutViewCustomizer CreateCustomizer() {
			return new LayoutViewCustomizer();
		}
		protected override void OnFormClosing(FormClosingEventArgs e) {
			Customizer.ProcessClosing();
			base.OnFormClosing(e);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(customizer!=null) {
					customizer.Dispose();
					customizer = null;
				}
				ownerView =null;
			}
			base.Dispose(disposing);
		}
	}
}
