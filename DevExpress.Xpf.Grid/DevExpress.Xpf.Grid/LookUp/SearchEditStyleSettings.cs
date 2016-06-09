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
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Editors;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Grid.LookUp {
	public class SearchLookUpEditStyleSettings : LookUpEditStyleSettings {
		public static readonly DependencyProperty IsTextEditableProperty;
		static SearchLookUpEditStyleSettings() {
			Type ownerType = typeof(SearchLookUpEditStyleSettings);
			IsTextEditableProperty = DependencyPropertyManager.Register("IsTextEditable", typeof(bool?), ownerType, new PropertyMetadata(null, (d, e) => ((SearchLookUpEditStyleSettings)d).IsTextEditableChanged((bool?)e.NewValue)));
		}
		public bool? IsTextEditable {
			get { return (bool?)GetValue(IsTextEditableProperty); }
			set { SetValue(IsTextEditableProperty, value); }
		}
		public override void ApplyToEdit(BaseEdit editor) {
			base.ApplyToEdit(editor);
			LookUpEdit lookUp = (LookUpEdit)editor;
			lookUp.IsTextEditable = IsTextEditable != null && IsTextEditable.Value;
			if (!lookUp.IsPropertyAssigned(LookUpEditBase.ImmediatePopupProperty))
				lookUp.ImmediatePopup = true;
		}
		protected override EditorPlacement GetFindButtonPlacement(LookUpEditBase editor) {
			return EditorPlacement.Popup;
		}
		protected override EditorPlacement GetNullValueButtonPlacement(LookUpEditBase editor) {
			return EditorPlacement.None;
		}
		protected override EditorPlacement GetAddNewButtonPlacement(LookUpEditBase editor) {
			return EditorPlacement.None;
		}
		protected virtual void IsTextEditableChanged(bool? newValue) {
		}
		protected override bool ShouldFocusPopup { get { return true; } }
		protected internal override bool ShowSearchPanel {
			get { return true; }
		}
		protected override FilterByColumnsMode FilterByColumnsMode {
			get { return FilterByColumnsMode.Default; }
		}
		protected override FilterCondition FilterCondition {
			get { return FilterCondition.Contains; }
		}
		public override bool GetIsTextEditable(ButtonEdit editor) {
			return false;
		}
	}
	public class SearchTokenLookUpEditStyleSettings : SearchLookUpEditStyleSettings, ITokenStyleSettings {
		public static readonly DependencyProperty ShowTokenButtonsProperty;
		public static readonly DependencyProperty TokenBorderTemplateProperty;
		public static readonly DependencyProperty EnableTokenWrappingProperty;
		public static readonly DependencyProperty NewTokenPositionProperty;
		public static readonly DependencyProperty TokenTextTrimmingProperty;
		public static readonly DependencyProperty TokenMaxWidthProperty;
		public static readonly DependencyProperty AddTokenOnPopupSelectionProperty;
		public static readonly DependencyProperty AllowEditTokensProperty;
		static SearchTokenLookUpEditStyleSettings() {
			Type ownerType = typeof(SearchTokenLookUpEditStyleSettings);
			EnableTokenWrappingProperty = DependencyProperty.Register("EnableTokenWrapping", typeof(bool?), ownerType);
			TokenBorderTemplateProperty = DependencyProperty.Register("TokenBorderTemplate", typeof(ControlTemplate), ownerType);
			ShowTokenButtonsProperty = DependencyProperty.Register("ShowTokenButtons", typeof(bool?), ownerType, new FrameworkPropertyMetadata(true));
			NewTokenPositionProperty = DependencyProperty.Register("NewTokenPosition", typeof(NewTokenPosition?), ownerType, new FrameworkPropertyMetadata(null));
			TokenTextTrimmingProperty = DependencyProperty.Register("TokenTextTrimming", typeof(TextTrimming?), ownerType, new FrameworkPropertyMetadata(null));
			TokenMaxWidthProperty = DependencyProperty.Register("TokenMaxWidth", typeof(double?), ownerType, new FrameworkPropertyMetadata(null));
			AddTokenOnPopupSelectionProperty = DependencyProperty.Register("AddTokenOnPopupSelection", typeof(bool?), ownerType, new FrameworkPropertyMetadata(null));
			AllowEditTokensProperty = DependencyProperty.Register("AllowEditTokens", typeof(bool?), ownerType, new FrameworkPropertyMetadata(null));
		}
		public bool? AllowEditTokens {
			get { return (bool?)GetValue(AllowEditTokensProperty); }
			set { SetValue(AllowEditTokensProperty, value); }
		}
		[Obsolete]
		public bool? AddTokenOnPopupSelection {
			get { return (bool?)GetValue(AddTokenOnPopupSelectionProperty); }
			set { SetValue(AddTokenOnPopupSelectionProperty, value); }
		}
		public double? TokenMaxWidth {
			get { return (double?)GetValue(TokenMaxWidthProperty); }
			set { SetValue(TokenMaxWidthProperty, value); }
		}
		public TextTrimming? TokenTextTrimming {
			get { return (TextTrimming?)GetValue(TokenTextTrimmingProperty); }
			set { SetValue(TokenTextTrimmingProperty, value); }
		}
		public NewTokenPosition? NewTokenPosition {
			get { return (NewTokenPosition?)GetValue(NewTokenPositionProperty); }
			set { SetValue(NewTokenPositionProperty, value); }
		}
		public bool? EnableTokenWrapping {
			get { return (bool?)GetValue(EnableTokenWrappingProperty); }
			set { SetValue(EnableTokenWrappingProperty, value); }
		}
		public ControlTemplate TokenBorderTemplate {
			get { return (ControlTemplate)GetValue(TokenBorderTemplateProperty); }
			set { SetValue(TokenBorderTemplateProperty, value); }
		}
		public bool? ShowTokenButtons {
			get { return (bool?)GetValue(ShowTokenButtonsProperty); }
			set { SetValue(ShowTokenButtonsProperty, value); }
		}
		public override bool IsTokenStyleSettings() {
			return true;
		}
		protected override bool GetActualAllowDefaultButton(ButtonEdit editor) {
			LookUpEditBasePropertyProvider btn = (LookUpEditBasePropertyProvider)ActualPropertyProvider.GetProperties(editor);
			return !btn.EnableTokenWrapping;
		}
	}
}
