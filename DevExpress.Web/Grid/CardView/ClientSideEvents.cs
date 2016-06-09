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

using System.ComponentModel;
namespace DevExpress.Web {
	public class CardViewClientSideEvents : GridClientSideEvents {
		public CardViewClientSideEvents()
			: base() {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewClientSideEventsColumnSorting"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(false), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public new string ColumnSorting { get { return base.ColumnSorting; } set { base.ColumnSorting = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewClientSideEventsCustomButtonClick"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(false), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public new string CustomButtonClick { get { return base.CustomButtonClick; } set { base.CustomButtonClick = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewClientSideEventsSelectionChanged"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(false), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public new string SelectionChanged { get { return base.SelectionChanged; } set { base.SelectionChanged = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewClientSideEventsCustomizationWindowCloseUp"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(false), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public new string CustomizationWindowCloseUp { get { return base.CustomizationWindowCloseUp; } set { base.CustomizationWindowCloseUp = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewClientSideEventsBatchEditStartEditing"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(false), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public new string BatchEditStartEditing { get { return base.BatchEditStartEditing; } set { base.BatchEditStartEditing = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewClientSideEventsBatchEditEndEditing"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(false), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public new string BatchEditEndEditing { get { return base.BatchEditEndEditing; } set { base.BatchEditEndEditing = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewClientSideEventsBatchEditConfirmShowing"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(false), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public new string BatchEditConfirmShowing { get { return base.BatchEditConfirmShowing; } set { base.BatchEditConfirmShowing = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewClientSideEventsBatchEditTemplateCellFocused"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(false), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public new string BatchEditTemplateCellFocused { get { return base.BatchEditTemplateCellFocused; } set { base.BatchEditTemplateCellFocused = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewClientSideEventsCardClick"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(false), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string CardClick { get { return GetEventHandler("CardClick"); } set { SetEventHandler("CardClick", value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewClientSideEventsCardDblClick"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(false), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string CardDblClick { get { return GetEventHandler("CardDblClick"); } set { SetEventHandler("CardDblClick", value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewClientSideEventsFocusedCardChanged"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(false), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string FocusedCardChanged { get { return GetEventHandler("FocusedCardChanged"); } set { SetEventHandler("FocusedCardChanged", value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewClientSideEventsBatchEditCardValidating"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(false), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string BatchEditCardValidating { get { return GetEventHandler("BatchEditCardValidating"); } set { SetEventHandler("BatchEditCardValidating", value); } }
		protected override void AddEventNames(System.Collections.Generic.List<string> names) {
			base.AddEventNames(names);
			names.AddRange(new string[] {
				"CardClick",
				"CardDblClick",
				"FocusedCardChanged",
				"BatchEditCardValidating"
			});
		}
	}
}
