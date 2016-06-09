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
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Gauges.Native;
namespace DevExpress.Xpf.Gauges {
	public class StringToCharConverter : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if (sourceType == typeof(string))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			string stringValue = value as string;
			if (stringValue != null && stringValue.Length == 1)
				return stringValue[0];
			throw new FormatException();
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if (destinationType == typeof(string))
				return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (value is Char)
				return value.ToString();
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public class SymbolStateToVisibilityConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (targetType == typeof(Visibility)) {
				SymbolState state = value as SymbolState;
				bool visible = false;
				int index;
				if (state != null && Int32.TryParse(parameter.ToString(), out index)) {
					if (index < state.Segments.Length)
						visible = state.Segments[index];
					return visible ? Visibility.Visible : Visibility.Collapsed;
				}
			}
			return Visibility.Collapsed;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is Visibility)
				if (targetType == typeof(bool)) {
					return (Visibility)value == Visibility.Visible ? true : false;
				}
			return null;
		}
	}
	public class PresentationToFillConverter : IValueConverter {
		enum SegmentType {
			Active,
			Inactive
		}
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			IDefaultSymbolPresentation presentation = value as IDefaultSymbolPresentation;
			if (targetType == typeof(Brush) && parameter != null && presentation != null) {
				SegmentType type;
				if (Enum.TryParse<SegmentType>(parameter.ToString(), true, out type))
					return type == SegmentType.Active ? presentation.ActualFillActive : presentation.ActualFillInactive;
			}
			return null;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	[NonCategorized]
	public class SymbolState {
		const int defaultSegmentCount = 17;
		readonly bool[] segments;
		readonly string symbol;
		internal string Symbol { get { return symbol; } }
		public bool[] Segments {
			get { return segments; }
		}
		internal SymbolState(params bool[] values) : this(string.Empty, values) {
		}
		internal SymbolState(string symbol, params bool[] values) : this(symbol, defaultSegmentCount, values) {			
		}
		internal SymbolState(string symbol, int segmentCount, params bool[] values) {
			this.symbol = symbol;
			segments = new bool[segmentCount];
			if (values != null) {
				int minLength = Math.Min(segmentCount, values.Length);
				for (int i = 0; i < minLength; i++)
					segments[i] = values[i];
			}
		}
		internal SymbolState Unite(SymbolState unitedSymbolState) {
			bool[] resulatSegments = new bool[Math.Min(segments.Length, unitedSymbolState.segments.Length)];
			for (int i = 0; i < resulatSegments.Length; i++)
				resulatSegments[i] = Segments[i] || unitedSymbolState.Segments[i];
			return new SymbolState(symbol + unitedSymbolState.symbol, resulatSegments.Length, resulatSegments);
		}
	}
	[NonCategorized]
	public class SymbolInfo : ElementInfoBase, IWeakEventListener {
		readonly int symbolIndex;
		string displayText = String.Empty;
		Thickness margin;
		Transform renderTransform = null;
		SymbolState symbolState = null;
		internal int SymbolIndex { get { return symbolIndex; } }
		protected internal override bool InfluenceOnGaugeSize { get { return true; } }
		public string DisplayText {
			get { return displayText; }
			set {
				if (displayText != value) {
					displayText = value;
					NotifyPropertyChanged("DisplayText");
				}
			}
		}
		public Thickness Margin {
			get { return margin; }
			set {
				if (margin != value) {
					margin = value;
					NotifyPropertyChanged("Margin");
				}
			}
		}
		public Transform RenderTransform {
			get { return renderTransform; }
			set {
				if (renderTransform != value) {
					renderTransform = value;
					NotifyPropertyChanged("RenderTransform");
				}
			}
		}
		public SymbolState SymbolState {
			get { return symbolState; }
			set {
				if (symbolState != value) {
					symbolState = value;
					NotifyPropertyChanged("SymbolState");
					UpdateSegments();
				}
			}
		}
		public SymbolInfo(ILayoutCalculator layoutCalculator, PresentationControl presentationControl, PresentationBase presentation, int symbolIndex)
			: base(layoutCalculator, 0, presentationControl, presentation) {
				this.symbolIndex = symbolIndex;
		}
		#region IWeakEventListener implementation
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			bool success = false;
			if (managerType == typeof(PropertyChangedWeakEventManager)) {
				if (sender is PresentationBase) {
					PresentationChanged();
					success = true;
				}
			}
			return success;
		}
		#endregion
		protected virtual void UpdateSegments() {
		}
		protected override void PresentationChanging(PresentationBase oldValue, PresentationBase newValue) {
			base.PresentationChanging(oldValue, newValue);
			CommonUtils.SubscribePropertyChangedWeakEvent(oldValue, newValue, this);
		}
	}
}
