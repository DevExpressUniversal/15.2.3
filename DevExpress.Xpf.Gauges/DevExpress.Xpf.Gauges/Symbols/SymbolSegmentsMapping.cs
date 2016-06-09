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
using System.Globalization;
using System.Windows;
using DevExpress.Xpf.Gauges.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Gauges {
	public enum SymbolType {
		Main,
		Additional
	}
	public class StatesMaskConverter : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if (sourceType == typeof(string))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			string stringValue = value as string;
			if (stringValue != null) {
				string[] stringStates = stringValue.Split(new string[] { " ", ",", ", " }, StringSplitOptions.RemoveEmptyEntries);
				if (stringStates.Length > 0) {
					bool[] boolStates = new bool[stringStates.Length];
					for (int i = 0; i < stringStates.Length; i++) {
						int intState;
						if (Int32.TryParse(stringStates[i], out intState))
							boolStates[i] = intState != 0;
					}
					return new StatesMask(boolStates);
				}
			}
			return null;
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if (destinationType == typeof(string))
				return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (value is StatesMask) {
				StatesMask stateCollection = (StatesMask)value;
				if (destinationType == typeof(string)) {
					string resultString = "";
					foreach (bool state in stateCollection.States) {
						resultString += resultString != "" ? " " : "";
						resultString += state ? "1" : "0";
					}
					return resultString;
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	[TypeConverter(typeof(StatesMaskConverter))]
	public struct StatesMask {
		bool[] states;
		public bool[] States {
			get { return states; }
		}
		public StatesMask(params bool[] initialStates) {
			if (initialStates != null)
				states = initialStates;
			else
				states = new bool[0];
		}
	}
	public class SymbolSegmentsMapping : GaugeDependencyObject {
		public static readonly DependencyProperty SymbolProperty = DependencyPropertyManager.Register("Symbol",
			typeof(char), typeof(SymbolSegmentsMapping), new PropertyMetadata(NotifyPropertyChanged));
		public static readonly DependencyProperty SegmentsStatesProperty = DependencyPropertyManager.Register("SegmentsStates",
			typeof(StatesMask), typeof(SymbolSegmentsMapping), new PropertyMetadata(NotifyPropertyChanged));
		public static readonly DependencyProperty SymbolTypeProperty = DependencyPropertyManager.Register("SymbolType",
			typeof(SymbolType), typeof(SymbolSegmentsMapping), new PropertyMetadata(SymbolType.Main, NotifyPropertyChanged));
		[
		Category(Categories.Data),
		TypeConverter(typeof(StringToCharConverter))
		]
		public char Symbol {
			get { return (char)GetValue(SymbolProperty); }
			set { SetValue(SymbolProperty, value); }
		}
		[Category(Categories.Data)]
		public StatesMask SegmentsStates {
			get { return (StatesMask)GetValue(SegmentsStatesProperty); }
			set { SetValue(SegmentsStatesProperty, value); }
		}
		[Category(Categories.Data)]
		public SymbolType SymbolType {
			get { return (SymbolType)GetValue(SymbolTypeProperty); }
			set { SetValue(SymbolTypeProperty, value); }
		}
		protected override GaugeDependencyObject CreateObject() {
			return new SymbolSegmentsMapping();
		}
	}
}
