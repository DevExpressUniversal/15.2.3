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
using System.Text;
using DevExpress.Web.Design;
using System.Windows.Forms;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;
using DevExpress.Web.Captcha;
using DevExpress.Web.Internal;
using System.Drawing;
using DevExpress.Utils.About;
namespace DevExpress.Web.Design {
	public class ASPxCaptchaDesigner : ASPxWebControlDesigner {
		ASPxCaptcha captcha;
		public override void ShowAbout() {
			CaptchaAboutDialogHelper.ShowAbout(Component.Site);
		}
		public override void RunDesigner() {
			ShowDialog(CreateEditorForm("ClientSideEvents", this.captcha.ClientSideEvents));
		}
		public override void Initialize(IComponent component) {
			this.captcha = (ASPxCaptcha)component;
			base.Initialize(component);
			EnsureReferences("DevExpress.Web" + AssemblyInfo.VSuffix);
		}
	}
	public class CaptchaAboutDialogHelper : AboutDialogHelperBase {
		public static void ShowAbout(IServiceProvider provider) {
			ShowAboutForm(provider, typeof(ASPxCaptcha), new ProductKind[] { ProductKind.FreeOffer, ProductKind.DXperienceASP });
		}
		public static void ShowTrialAbout(IServiceProvider provider) {
			if (ShouldShowTrialAbout(typeof(ASPxCaptcha)))
				ShowAbout(provider);
		}
	}
	public class FontFamilyEditor : DropDownUITypeEditorBase {
		private readonly string[] PredefinedFontFamilies = new string[] { 
			"Times New Roman",
			"Courier New",
			"Georgia",
			"Verdana"
		};
		protected override void ApplySelectedValue(ListBox valueList, ITypeDescriptorContext context) {
			CaptchaImageProperties imageProperties = (CaptchaImageProperties)context.Instance;
			imageProperties.FontFamily = valueList.SelectedItem.ToString();
		}
		protected override void FillValueList(ListBox valueList, ITypeDescriptorContext context) {
			for (int i = 0; i < PredefinedFontFamilies.Length; i++)
				valueList.Items.Add(PredefinedFontFamilies[i]);
		}
		protected override PropertyDescriptor GetChangedPropertyDescriptor(object component) {
			return null;
		}
		protected override object GetComponent(ITypeDescriptorContext context) {
			CaptchaImageProperties imageProperties = (CaptchaImageProperties)context.Instance;
			return imageProperties.Captcha;
		}
		protected override void SetInitiallySelectedValue(ListBox valueList, ITypeDescriptorContext context) {
			valueList.SelectedItem = CaptchaImageProperties.DefaultFontFamily;
		}
	}
}
