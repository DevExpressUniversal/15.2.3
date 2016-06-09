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
using System.ComponentModel;
using System.Collections;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Controls;
using System.Drawing.Design;
namespace DevExpress.XtraGrid.Views.Base {
	public class ColumnViewAppearances : BaseViewAppearanceCollection {
		public ColumnViewAppearances(BaseView view) : base(view) { }
		AppearanceObject filterPanel, filterCloseButton, viewCaption;
		protected override void CreateAppearances() {
			this.filterCloseButton = CreateAppearance("FilterCloseButton"); 
			this.filterPanel = CreateAppearance("FilterPanel");
			this.viewCaption = CreateAppearance("ViewCaption");
		}
		void ResetFilterCloseButton() { FilterCloseButton.Reset(); }
		bool ShouldSerializeFilterCloseButton() { return FilterCloseButton.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewAppearancesFilterCloseButton"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject FilterCloseButton { get { return filterCloseButton; } }
		void ResetFilterPanel() { FilterPanel.Reset(); }
		bool ShouldSerializeFilterPanel() { return FilterPanel.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewAppearancesFilterPanel"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject FilterPanel { get { return filterPanel; } }
		void ResetViewCaption() { ViewCaption.Reset(); }
		bool ShouldSerializeViewCaption() { return ViewCaption.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewAppearancesViewCaption"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ViewCaption { get { return viewCaption; } }
	}
	public class ColumnViewPrintAppearances : BaseViewAppearanceCollection {
		AppearanceObject filterPanel;
		public ColumnViewPrintAppearances(BaseView view) : base(view) { }
		protected override void CreateAppearances() {
			this.filterPanel = CreateAppearance("FilterPanel"); 
		}
		void ResetFilterPanel() { FilterPanel.Reset(); }
		bool ShouldSerializeFilterPanel() { return FilterPanel.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewPrintAppearancesFilterPanel"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject FilterPanel { get { return filterPanel; } }
		protected override AppearanceObject CreateAppearanceInstance(AppearanceObject parent, string name) {
			return new AppearanceObjectPrint(this, parent, name);
		}
	}
}
