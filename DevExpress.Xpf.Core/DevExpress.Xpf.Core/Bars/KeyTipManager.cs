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

using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows;
using System.Collections;
using System;
using DevExpress.Xpf.Core;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Data;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Bars {
	public enum RibbonKeyTipHorizontalPlacement {
		KeyTipLeftAtTargetLeft = 0,
		KeyTipCenterAtTargetLeft,
		KeyTipRightAtTargetLeft,
		KeyTipLeftAtTargetCenter,
		KeyTipCenterAtTargetCenter,
		KeyTipRightAtTargetCenter,
		KeyTipLeftAtTargetRight,
		KeyTipCenterAtTargetRight,
		KeyTipRightAtTargetRight
	}
	public enum RibbonKeyTipVerticalPlacement {
		AutoRow = 0,
		TopRow,
		CenterRow,
		BottomRow,
		CaptionGroupRow,
		KeyTipBottomAtTargetBottom,
		KeyTipCenterAtTargetBottom,
		KeyTipTopAtTargetBottom,
		KeyTipBottomAtTargetCenter,
		KeyTipCenterAtTargetCenter,
		KeyTipTopAtTargetCenter,
		KeyTipBottomAtTargetTop,
		KeyTipCenterAtTargetTop,
		KeyTipTopAtTargetTop
	}
	public class RibbonKeyTip : DependencyObject {
		public static readonly DependencyProperty IsEnabledProperty =
			DependencyProperty.Register("IsEnabled", typeof(bool), typeof(RibbonKeyTip), new PropertyMetadata(false));
		public bool IsEnabled {
			get { return (bool)GetValue(IsEnabledProperty); }
			set { SetValue(IsEnabledProperty, value); }
		}
		public string Caption { get; set; }
		public string KeyTip { get; set; }
		public bool IsVisible { get; set; }
		public DependencyObject Owner { get { return owner; }
			set {
				if (owner == value) return;
				var oldValue = owner;
				owner = value;
				OnOwnerChanged(oldValue);
			}
		}
		public DependencyObject TargetElement { get; set; }
		public RibbonKeyTipHorizontalPlacement HorizontalPlacement { get; set; }
		public RibbonKeyTipVerticalPlacement VerticalPlacement { get; set; }
		public double HorizontalOffset { get; set; }
		public double VerticalOffset { get; set; }
		public bool IsActualVisible { get; set; }
		public string ActualKeyTip { get; set; }
		public object Tag { get; set; }
		void OnOwnerChanged(DependencyObject oldValue) {
			if(Owner == null)
				ClearValue(IsEnabledProperty);
			else
				BindingOperations.SetBinding(this, IsEnabledProperty, new Binding() { Source = Owner, Path = new PropertyPath("IsEnabled") });
		}
		DependencyObject owner;
	}
	public class RibbonKeyTipList : List<RibbonKeyTip> { };
	public enum ComplexLayoutState { Updating, Updated }
	public class ComplexLayoutStateChangedEventArgs : EventArgs {
		public ComplexLayoutState State { get; protected set; }
		public ComplexLayoutStateChangedEventArgs(ComplexLayoutState layoutState) {
			State = layoutState;
		}
	}
	public delegate void ComplexLayoutStateChangedEventHandler(object sender, ComplexLayoutStateChangedEventArgs e);
	public interface IComplexLayout {
		ComplexLayoutState ComplexLayoutState { get; }
		event ComplexLayoutStateChangedEventHandler ComplexLayoutStateChanged;
	}
	public class KeyTipGenerator {
		static RibbonKeyTipList keyTips;
		static int numericKeyTip;
		static bool HasCollision(string keyTipText1, string keyTipText2) {
			return keyTipText1.StartsWith(keyTipText2) || keyTipText2.StartsWith(keyTipText1);
		}
		static RibbonKeyTipList GetCollisionKeyTips(string keyTipText) {
			RibbonKeyTipList collisionKeyTips = new RibbonKeyTipList();
			foreach(RibbonKeyTip keyTip in keyTips) {
				if(HasValue(keyTip.ActualKeyTip) && HasCollision(keyTip.ActualKeyTip, keyTipText)) {
					collisionKeyTips.Add(keyTip);
				}
			}
			if(collisionKeyTips.Count == 0)
				return null;
			return collisionKeyTips;
		}
		static public void Generate(RibbonKeyTipList levelKeyTips) {
			keyTips = levelKeyTips;
			try {
				numericKeyTip = 0;
				ClearActualKeyTips();
				GenerateKeyTipsByUserKeyTips();
				GenerateKeyTipsByCaption();
				GenerateNumericKeyTips();
			} finally {
				keyTips = null;
			}
		}
		static protected void ClearActualKeyTips() {
			if(keyTips == null) return;
			foreach(RibbonKeyTip keyTip in keyTips) {
				keyTip.ActualKeyTip = string.Empty;
			}
		}
		static string GenerateNumericKeyTipText(int numericKeyTip) {
			if(numericKeyTip < 9) return (numericKeyTip + 1).ToString();
			if(numericKeyTip < 99) return "0" + (numericKeyTip + 1).ToString();
			if(numericKeyTip < 999) return "00" + (numericKeyTip + 1).ToString();
			return string.Empty;
		}
		static void GenerateNumericKeyTip(RibbonKeyTip keyTip) {
			while(GetCollisionKeyTips(GenerateNumericKeyTipText(numericKeyTip)) != null) numericKeyTip++;
			keyTip.ActualKeyTip = GenerateNumericKeyTipText(numericKeyTip);
			numericKeyTip++;
			return;
		}
		static void GenerateNumericKeyTips() {
			foreach(RibbonKeyTip keyTip in keyTips) {
				if(!HasValue(keyTip.ActualKeyTip)) GenerateNumericKeyTip(keyTip);
			}
		}
		static bool ShouldIgnoreChar(char s) {
			return s == '&' || s == ' ' || char.IsDigit(s);
		}
		static bool GenerateKeyTipByChars(RibbonKeyTip keyTip, char firstChar, char secondChar, char thirdChar) {
			string text = firstChar.ToString();
			if(secondChar != (char)0) text += secondChar.ToString();
			if(thirdChar != (char)0) text += thirdChar.ToString();
			text = text.ToUpper();
			if(GetCollisionKeyTips(text) != null) return false;
			keyTip.ActualKeyTip = text;
			return true;
		}
		static bool GenerateKeyTipByCaption(RibbonKeyTip keyTip) {
			for(int i = 0; i < keyTip.Caption.Length; i++) {
				if(ShouldIgnoreChar(keyTip.Caption[i])) continue;
				if(GenerateKeyTipByChars(keyTip, keyTip.Caption[i], (char)0, (char)0)) return true;
			}
			for(int i = 0; i < keyTip.Caption.Length - 1; i++) {
				if(ShouldIgnoreChar(keyTip.Caption[i])) continue;
				for(int j = i+1; j < keyTip.Caption.Length; j++) {
					if(ShouldIgnoreChar(keyTip.Caption[j])) continue;
					if(GenerateKeyTipByChars(keyTip, keyTip.Caption[i], keyTip.Caption[j], (char)0)) return true;
				}
			}
			for(int i = 0; i < keyTip.Caption.Length - 2; i++) {
				if(ShouldIgnoreChar(keyTip.Caption[i])) continue;
				for(int j = i+1; j < keyTip.Caption.Length - 1; j++) {
					if(ShouldIgnoreChar(keyTip.Caption[j])) continue;
					for(int k = j + 1; k < keyTip.Caption.Length; k++) {
						if(ShouldIgnoreChar(keyTip.Caption[k])) continue;
						if(GenerateKeyTipByChars(keyTip, keyTip.Caption[i], keyTip.Caption[j], keyTip.Caption[k])) return true;
					}
				}
			}
			return false;
		}
		static void GenerateKeyTipsByCaption() {
			foreach(RibbonKeyTip keyTip in keyTips) {
				if(!HasValue(keyTip.ActualKeyTip) && HasValue(keyTip.Caption)) GenerateKeyTipByCaption(keyTip);
			}
		}
		static bool HasValue(string str) {
			return str != null && str.Length != 0;
		}
		static void GenerateKeyTipsByUserKeyTips() {
			foreach(RibbonKeyTip keyTip in keyTips) {
				if(!HasValue(keyTip.KeyTip)) continue;
				RibbonKeyTipList collisionKeyTips = GetCollisionKeyTips(keyTip.KeyTip.ToUpper());
				if(collisionKeyTips != null) {
					string exceptionText = "KeyTip`s collision: \"" + keyTip.KeyTip + "\"";
					foreach(RibbonKeyTip collistionKeyTip in collisionKeyTips) {
						exceptionText += ",\"" + collistionKeyTip.ActualKeyTip + "\"";
					}
					throw new ArgumentException(exceptionText);
				}
				keyTip.ActualKeyTip = keyTip.KeyTip.ToUpper();
			}
		}
	}
}
