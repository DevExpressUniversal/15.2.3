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
using System.Windows;
using DevExpress.Xpf.Gauges.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Gauges {
	public class SevenSegmentsView : SegmentsView {
		public static readonly DependencyProperty PresentationProperty = DependencyPropertyManager.Register("Presentation",
			typeof(SevenSegmentsPresentation), typeof(SevenSegmentsView), new PropertyMetadata(null, PresentationPropertyChanged));
		[Category(Categories.Presentation)]
		public SevenSegmentsPresentation Presentation {
			get { return (SevenSegmentsPresentation)GetValue(PresentationProperty); }
			set { SetValue(PresentationProperty, value); }
		}
		public SevenSegmentsView() {
			InitializeMapping();
		}
		void InitializeMapping() {
			StatesMaskConverter converter = new StatesMaskConverter();
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = '0', SegmentsStates = (StatesMask)converter.ConvertFromString("1 1 1 1 1 1 0"), SymbolType = SymbolType.Main });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = '1', SegmentsStates = (StatesMask)converter.ConvertFromString("0 1 1 0 0 0 0"), SymbolType = SymbolType.Main });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = '2', SegmentsStates = (StatesMask)converter.ConvertFromString("1 1 0 1 1 0 1"), SymbolType = SymbolType.Main });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = '3', SegmentsStates = (StatesMask)converter.ConvertFromString("1 1 1 1 0 0 1"), SymbolType = SymbolType.Main });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = '4', SegmentsStates = (StatesMask)converter.ConvertFromString("0 1 1 0 0 1 1"), SymbolType = SymbolType.Main });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = '5', SegmentsStates = (StatesMask)converter.ConvertFromString("1 0 1 1 0 1 1"), SymbolType = SymbolType.Main });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = '6', SegmentsStates = (StatesMask)converter.ConvertFromString("1 0 1 1 1 1 1"), SymbolType = SymbolType.Main });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = '7', SegmentsStates = (StatesMask)converter.ConvertFromString("1 1 1 0 0 0 0"), SymbolType = SymbolType.Main });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = '8', SegmentsStates = (StatesMask)converter.ConvertFromString("1 1 1 1 1 1 1"), SymbolType = SymbolType.Main });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = '9', SegmentsStates = (StatesMask)converter.ConvertFromString("1 1 1 1 0 1 1"), SymbolType = SymbolType.Main });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = '-', SegmentsStates = (StatesMask)converter.ConvertFromString("0 0 0 0 0 0 1"), SymbolType = SymbolType.Main });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = '.', SegmentsStates = (StatesMask)converter.ConvertFromString("0 0 0 0 0 0 0 1"), SymbolType = SymbolType.Additional });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = ',', SegmentsStates = (StatesMask)converter.ConvertFromString("0 0 0 0 0 0 0 1"), SymbolType = SymbolType.Additional });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = ':', SegmentsStates = (StatesMask)converter.ConvertFromString("0 0 0 0 0 0 0 0 1"), SymbolType = SymbolType.Main });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = '\'', SegmentsStates = (StatesMask)converter.ConvertFromString("0 0 0 0 0 0 0 0 0 1"), SymbolType = SymbolType.Additional });
		}
		protected override GaugeDependencyObject CreateObject() {
			return new SevenSegmentsView();
		}
		protected internal override SymbolViewInternal CreateInternalView() {
			return new SevenSegmentsViewInternal();
		}
	}
}
namespace DevExpress.Xpf.Gauges.Native {
	public class SevenSegmentsViewInternal : SegmentsViewInternal {
		SevenSegmentsModel Model { get { return Gauge != null ? Gauge.ActualModel.SevenSegmentsModel : null; } }
		SevenSegmentsPresentation Presentation { get { return ((SevenSegmentsView)View).Presentation; } }
		internal override double DefaultHeightToWidthRatio { get { return 122.0 / 74.0; } }
		protected override SymbolsModelBase ModelBase { get { return Model; } }
		protected override SymbolPresentation ActualPresentation {
			get {
				if (Presentation != null)
					return Presentation;
				if (Model != null && Model.Presentation != null)
					return Model.Presentation;
				return new DefaultSevenSegmentsPresentation();
			}
		}
	}
}
