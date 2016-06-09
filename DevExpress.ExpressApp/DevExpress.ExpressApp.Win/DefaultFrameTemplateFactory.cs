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
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.ExpressApp.Win.Templates.Bars;
using DevExpress.ExpressApp.Win.Templates.Ribbon;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.ExpressApp.Win {
	public class DefaultFrameTemplateFactory : FrameTemplateFactoryBase {
		protected override IFrameTemplate CreateNestedFrameTemplate() {
			return new NestedFrameTemplate();
		}
		protected override IFrameTemplate CreatePopupWindowTemplate() {
			return new PopupForm();
		}
		protected override IFrameTemplate CreateLookupControlTemplate() {
			return new LookupControlTemplate();
		}
		protected override IFrameTemplate CreateLookupWindowTemplate() {
			return new LookupForm();
		}
		protected override IFrameTemplate CreateApplicationWindowTemplate() {
			return new MainForm();
		}
		protected override IFrameTemplate CreateViewTemplate() {
			return new DetailViewForm();
		}
	}
	public class DefaultFrameTemplateFactoryV2 : FrameTemplateFactoryBase {
		protected override IFrameTemplate CreateNestedFrameTemplate() {
			return new NestedFrameTemplateV2();
		}
		protected override IFrameTemplate CreatePopupWindowTemplate() {
			return new PopupForm();
		}
		protected override IFrameTemplate CreateLookupControlTemplate() {
			return new LookupControlTemplate();
		}
		protected override IFrameTemplate CreateLookupWindowTemplate() {
			return new LookupForm();
		}
		protected override IFrameTemplate CreateApplicationWindowTemplate() {
			if(FormStyle == RibbonFormStyle.Standard) {
				return new MainFormV2();
			}
			else {
				return new MainRibbonFormV2();
			}
		}
		protected override IFrameTemplate CreateViewTemplate() {
			if(FormStyle == RibbonFormStyle.Standard) {
				return new DetailFormV2();
			}
			else {
				return new DetailRibbonFormV2();
			}
		}
		public RibbonFormStyle FormStyle { get; set; }
	}
}
