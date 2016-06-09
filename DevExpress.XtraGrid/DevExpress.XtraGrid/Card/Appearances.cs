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
using DevExpress.XtraGrid.Views.Base;
using System.Drawing.Design;
namespace DevExpress.XtraGrid.Views.Card {
	public class CardViewAppearances : ColumnViewAppearances {
		public CardViewAppearances(BaseView view) : base(view) { }
		AppearanceObject emptySpace, cardCaption, fieldCaption, fieldValue, focusedCardCaption, separatorLine, 
			cardButton, card, cardExpandButton, selectedCardCaption, hideSelectionCardCaption;
		protected override void CreateAppearances() {
			base.CreateAppearances();
			this.emptySpace = CreateAppearance("EmptySpace");
			this.cardCaption = CreateAppearance("CardCaption");
			this.fieldCaption = CreateAppearance("FieldCaption");
			this.fieldValue = CreateAppearance("FieldValue");
			this.focusedCardCaption = CreateAppearance("FocusedCardCaption");
			this.selectedCardCaption = CreateAppearance("SelectedCardCaption");
			this.hideSelectionCardCaption = CreateAppearance("HideSelectionCardCaption");
			this.separatorLine = CreateAppearance("SeparatorLine");
			this.cardButton = CreateAppearance("CardButton");
			this.cardExpandButton = CreateAppearance("CardExpandButton");
			this.card = CreateAppearance("Card");
		}
		void ResetEmptySpace() { EmptySpace.Reset(); }
		bool ShouldSerializeEmptySpace() { return EmptySpace.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardViewAppearancesEmptySpace"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject EmptySpace { get { return emptySpace; } }
		void ResetCardCaption() { CardCaption.Reset(); }
		bool ShouldSerializeCardCaption() { return CardCaption.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardViewAppearancesCardCaption"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject CardCaption { get { return cardCaption; } }
		void ResetFieldCaption() { FieldCaption.Reset(); }
		bool ShouldSerializeFieldCaption() { return FieldCaption.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardViewAppearancesFieldCaption"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject FieldCaption { get { return fieldCaption; } }
		void ResetFieldValue() { FieldValue.Reset(); }
		bool ShouldSerializeFieldValue() { return FieldValue.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardViewAppearancesFieldValue"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject FieldValue { get { return fieldValue; } }
		void ResetFocusedCardCaption() { FocusedCardCaption.Reset(); }
		bool ShouldSerializeFocusedCardCaption() { return FocusedCardCaption.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardViewAppearancesFocusedCardCaption"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject FocusedCardCaption { get { return focusedCardCaption; } }
		void ResetHideSelectionCardCaption() { HideSelectionCardCaption.Reset(); }
		bool ShouldSerializeHideSelectionCardCaption() { return HideSelectionCardCaption.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardViewAppearancesHideSelectionCardCaption"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject HideSelectionCardCaption { get { return hideSelectionCardCaption; } }
		void ResetSelectedCardCaption() { SelectedCardCaption.Reset(); }
		bool ShouldSerializeSelectedCardCaption() { return SelectedCardCaption.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardViewAppearancesSelectedCardCaption"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject SelectedCardCaption { get { return selectedCardCaption; } }
		void ResetSeparatorLine() { SeparatorLine.Reset(); }
		bool ShouldSerializeSeparatorLine() { return SeparatorLine.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardViewAppearancesSeparatorLine"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject SeparatorLine { get { return separatorLine; } }
		void ResetCard() { Card.Reset(); }
		bool ShouldSerializeCard() { return Card.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardViewAppearancesCard"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Card { get { return card; } }
		void ResetCardButton() { CardButton.Reset(); }
		bool ShouldSerializeCardButton() { return CardButton.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardViewAppearancesCardButton"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject CardButton { get { return cardButton; } }
		void ResetCardExpandButton() { CardExpandButton.Reset(); }
		bool ShouldSerializeCardExpandButton() { return CardExpandButton.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardViewAppearancesCardExpandButton"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject CardExpandButton { get { return cardExpandButton; } }
	}
	public class CardViewPrintAppearances : ColumnViewPrintAppearances {
		public CardViewPrintAppearances(BaseView view) : base(view) { }
		AppearanceObject cardCaption, fieldCaption, fieldValue, card;
		protected override void CreateAppearances() {
			base.CreateAppearances();
			this.cardCaption = CreateAppearance("CardCaption");
			this.fieldCaption = CreateAppearance("FieldCaption");
			this.fieldValue = CreateAppearance("FieldValue");
			this.card = CreateAppearance("Card");
		}
		void ResetCardCaption() { CardCaption.Reset(); }
		bool ShouldSerializeCardCaption() { return CardCaption.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardViewPrintAppearancesCardCaption"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject CardCaption { get { return cardCaption; } }
		void ResetFieldCaption() { FieldCaption.Reset(); }
		bool ShouldSerializeFieldCaption() { return FieldCaption.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardViewPrintAppearancesFieldCaption"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject FieldCaption { get { return fieldCaption; } }
		void ResetFieldValue() { FieldValue.Reset(); }
		bool ShouldSerializeFieldValue() { return FieldValue.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardViewPrintAppearancesFieldValue"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject FieldValue { get { return fieldValue; } }
		void ResetCard() { Card.Reset(); }
		bool ShouldSerializeCard() { return Card.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardViewPrintAppearancesCard"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Card { get { return card; } }
	}
}
