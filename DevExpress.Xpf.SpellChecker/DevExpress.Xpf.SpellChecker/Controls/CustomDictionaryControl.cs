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

using DevExpress.Xpf.Core;
using DevExpress.XtraSpellChecker;
using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.SpellChecker.Native;
using System.Text;
using System;
using System.Windows.Controls;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.SpellChecker.Forms {
	#region CustomDictionaryOkCommand
	public class CustomDictionaryOkCommand : UICommand<CustomDictionaryControl> {
		public CustomDictionaryOkCommand(CustomDictionaryControl control)
			: base(control) {
		}
		protected override bool CanExecuteCore(object parameter) {
			return true;
		}
		protected override void ExecuteCore(object parameter) {
			Control.DialogResult = true;
			Control.SpellChecker.FormsManager.HideEditDictionaryForm();
		}
	}
	#endregion
	#region CustomDictionaryCancelCommand
	public class CustomDictionaryCancelCommand : UICommand<CustomDictionaryControl> {
		public CustomDictionaryCancelCommand(CustomDictionaryControl control)
			: base(control) {
		}
		protected override bool CanExecuteCore(object parameter) {
			return true;
		}
		protected override void ExecuteCore(object parameter) {
			Control.DialogResult = false;
			Control.SpellChecker.FormsManager.HideEditDictionaryForm();
		}
	}
	#endregion
	#region SizeLimitControl
	[DXToolboxBrowsable(false)]
	public class SizeLimitControl : Decorator {
		#region InitialWidth
		public static readonly DependencyProperty InitialWidthProperty = CreateInitialWidthProperty();
		static DependencyProperty CreateInitialWidthProperty() {
			return DependencyProperty.Register("InitialWidth", typeof(double), typeof(SizeLimitControl), new PropertyMetadata(100.0));
		}
		public double InitialWidth {
			get { return (double)GetValue(InitialWidthProperty); }
			set { SetValue(InitialWidthProperty, value); }
		}
		#endregion
		#region InitialHeight
		public static readonly DependencyProperty InitialHeightProperty = CreateInitialHeightProperty();
		static DependencyProperty CreateInitialHeightProperty() {
			return DependencyProperty.Register("InitialHeight", typeof(double), typeof(SizeLimitControl), new PropertyMetadata(100.0));
		}
		public double InitialHeight {
			get { return (double)GetValue(InitialHeightProperty); }
			set { SetValue(InitialHeightProperty, value); }
		}
		#endregion
		protected override Size MeasureOverride(Size constraint) {
			double width = Double.IsInfinity(constraint.Width) ? InitialWidth : constraint.Width;
			double height = Double.IsInfinity(constraint.Height) ? InitialHeight : constraint.Height;
			return base.MeasureOverride(new Size(width, height));
		}
	} 
	#endregion
	#region CustomDictionaryControl
	[DXToolboxBrowsable(false)]
	public class CustomDictionaryControl : SpellCheckerControlBase {
		SpellCheckerCustomDictionary dictionary;
		bool isModified;
		bool dialogResult;
		public CustomDictionaryControl() : base() { }
		public CustomDictionaryControl(SpellChecker spellChecker, SpellCheckerCustomDictionary dictionary)
			: base(spellChecker) {
			DefaultStyleKey = typeof(CustomDictionaryControl);
			this.dictionary = dictionary;
			Initialize();
		}
		#region Properties
		public SpellCheckerCustomDictionary Dictionary { 
			get { return dictionary; }
			set {
				dictionary = value;
				LoadWords();
			}
		}
		public virtual bool IsModified { get { return isModified; } set { isModified = value; } }
		internal bool DialogResult { get { return dialogResult; } set { dialogResult = value; } }
		#region OkCommand
		public static readonly DependencyProperty OkCommandProperty = CreateOkCommandProperty();
		static DependencyProperty CreateOkCommandProperty() {
			return DependencyProperty.Register("OkCommand", typeof(ICommand), typeof(CustomDictionaryControl), new PropertyMetadata(null));
		}
		public ICommand OkCommand {
			get { return (ICommand)GetValue(OkCommandProperty); }
			set { SetValue(OkCommandProperty, value); }
		}
		#endregion
		#region CancelCommand
		public static readonly DependencyProperty CancelCommandProperty = CreateCancelCommandProperty();
		static DependencyProperty CreateCancelCommandProperty() {
			return DependencyProperty.Register("CancelCommand", typeof(ICommand), typeof(CustomDictionaryControl), new PropertyMetadata(null));
		}
		public ICommand CancelCommand {
			get { return (ICommand)GetValue(CancelCommandProperty); }
			set { SetValue(CancelCommandProperty, value); }
		}
		#endregion
		#region Text
		public static readonly DependencyProperty TextProperty = CreateTextProperty();
		static DependencyProperty CreateTextProperty() {
			return DependencyProperty.Register("Text", typeof(string), typeof(CustomDictionaryControl), new PropertyMetadata(String.Empty, OnTextPropertyChanged));
		}
		static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((CustomDictionaryControl)d).IsModified = true;
		}
		public string Text {
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		#endregion
		#endregion
		protected override void Initialize() {
			OkCommand = new CustomDictionaryOkCommand(this);
			CancelCommand = new CustomDictionaryCancelCommand(this);
			LoadWords();
		}
		protected virtual void LoadWords() {
			StringBuilder result = new StringBuilder();
			if (Dictionary.WordCount > 0) {
				for (int i = 0; i < Dictionary.WordCount - 1; i++)
					result.Append(Dictionary[i] + "\r\n");
				result.Append(Dictionary[Dictionary.WordCount - 1]);
			}
			Text = result.ToString();
			IsModified = false;
		}
		public virtual bool NeedSave() {
			return IsModified && DialogResult;
		}
		protected virtual void OnLoad() {
			LoadWords();
		}
	} 
	#endregion
}
