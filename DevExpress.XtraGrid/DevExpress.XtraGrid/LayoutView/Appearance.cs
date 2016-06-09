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
using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Base;
namespace DevExpress.XtraGrid.Views.Layout {
	public class LayoutViewAppearances : ColumnViewAppearances {
		AppearanceObject viewBackground, cardCaption, card, headerPanel;
		AppearanceObject fieldCaption, fieldValue, fieldEditingValue;
		AppearanceObject focusedCardCaption, selectedCardCaption, hideSelectionCardCaption;
		AppearanceObject separatorLine;
		AppearanceObject selectionFrame;
		public LayoutViewAppearances(BaseView view) : base(view) { }
		protected override void CreateAppearances() {
			base.CreateAppearances();
			this.viewBackground = CreateAppearance("ViewBackground");
			this.cardCaption = CreateAppearance("CardCaption");
			this.fieldCaption = CreateAppearance("FieldCaption");
			this.fieldValue = CreateAppearance("FieldValue");
			this.fieldEditingValue = CreateAppearance("FieldEditingValue");
			this.focusedCardCaption = CreateAppearance("FocusedCardCaption");
			this.selectedCardCaption = CreateAppearance("SelectedCardCaption");
			this.hideSelectionCardCaption = CreateAppearance("HideSelectionCardCaption");
			this.separatorLine = CreateAppearance("SeparatorLine");
			this.card = CreateAppearance("Card");
			this.selectionFrame = CreateAppearance("SelectionFrame");
			this.headerPanel = CreateAppearance("HeaderPanel");
		}
		void ResetSelectionFrame() { SelectionFrame.Reset(); }
		bool ShouldSerializeSelectionFrame() { return SelectionFrame.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewAppearancesSelectionFrame"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject SelectionFrame { get { return selectionFrame; } }
		void ResetViewBackground() { ViewBackground.Reset(); }
		bool ShouldSerializeViewBackground() { return ViewBackground.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewAppearancesViewBackground"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ViewBackground { get { return viewBackground; } }
		void ResetCard() { Card.Reset(); }
		bool ShouldSerializeCard() { return Card.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewAppearancesCard"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Card { get { return card; } }
		void ResetCardCaption() { CardCaption.Reset(); }
		bool ShouldSerializeCardCaption() { return CardCaption.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewAppearancesCardCaption"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject CardCaption { get { return cardCaption; } }
		void ResetHeaderPanel() { HeaderPanel.Reset(); }
		bool ShouldSerializeHeaderPanel() { return HeaderPanel.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewAppearancesHeaderPanel"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject HeaderPanel { get { return headerPanel; } }
		void ResetFieldCaption() { FieldCaption.Reset(); }
		bool ShouldSerializeFieldCaption() { return FieldCaption.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewAppearancesFieldCaption"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject FieldCaption { get { return fieldCaption; } }
		void ResetFieldValue() { FieldValue.Reset(); }
		bool ShouldSerializeFieldValue() { return FieldValue.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewAppearancesFieldValue"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject FieldValue { get { return fieldValue; } }
		void ResetFieldEditingValue() { FieldEditingValue.Reset(); }
		bool ShouldSerializeFieldEditingValue() { return FieldEditingValue.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewAppearancesFieldEditingValue"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject FieldEditingValue { get { return fieldEditingValue; } }
		void ResetSelectedCardCaption() { SelectedCardCaption.Reset(); }
		bool ShouldSerializeSelectedCardCaption() { return SelectedCardCaption.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewAppearancesSelectedCardCaption"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject SelectedCardCaption { get { return selectedCardCaption; } }
		void ResetFocusedCardCaption() { FocusedCardCaption.Reset(); }
		bool ShouldSerializeFocusedCardCaption() { return FocusedCardCaption.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewAppearancesFocusedCardCaption"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject FocusedCardCaption { get { return focusedCardCaption; } }
		void ResetHideSelectionCardCaption() { HideSelectionCardCaption.Reset(); }
		bool ShouldSerializeHideSelectionCardCaption() { return HideSelectionCardCaption.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewAppearancesHideSelectionCardCaption"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject HideSelectionCardCaption { get { return hideSelectionCardCaption; } }
		void ResetSeparatorLine() { SeparatorLine.Reset(); }
		bool ShouldSerializeSeparatorLine() { return SeparatorLine.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewAppearancesSeparatorLine"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject SeparatorLine { get { return separatorLine; } }
	}
	public class LayoutViewPrintAppearances : ColumnViewPrintAppearances {
		public LayoutViewPrintAppearances(BaseView view) : base(view) { }
		AppearanceObject card,cardCaption, fieldCaption, fieldValue;
		protected override void CreateAppearances() { 
			base.CreateAppearances();
			this.card = CreateAppearance("Card");
			this.cardCaption = CreateAppearance("CardCaption");
			this.fieldCaption = CreateAppearance("FieldCaption");
			this.fieldValue = CreateAppearance("FieldValue");
		}
		void ResetCard() { Card.Reset(); }
		bool ShouldSerializeCard() { return Card.ShouldSerialize(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Card { get { return card; } }
		void ResetCardCaption() { CardCaption.Reset(); }
		bool ShouldSerializeCardCaption() { return CardCaption.ShouldSerialize(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject CardCaption { get { return cardCaption; } }
		void ResetFieldCaption() { FieldCaption.Reset(); }
		bool ShouldSerializeFieldCaption() { return FieldCaption.ShouldSerialize(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject FieldCaption { get { return fieldCaption; } }
		void ResetFieldValue() { FieldValue.Reset(); }
		bool ShouldSerializeFieldValue() { return FieldValue.ShouldSerialize(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject FieldValue { get { return fieldValue; } }
	}
}
