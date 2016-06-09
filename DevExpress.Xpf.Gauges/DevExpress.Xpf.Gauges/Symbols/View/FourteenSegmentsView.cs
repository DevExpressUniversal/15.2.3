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
	public class FourteenSegmentsView : SegmentsView {
		public static readonly DependencyProperty PresentationProperty = DependencyPropertyManager.Register("Presentation",
			typeof(FourteenSegmentsPresentation), typeof(FourteenSegmentsView), new PropertyMetadata(null, PresentationPropertyChanged)); 
		[Category(Categories.Presentation)]
		public FourteenSegmentsPresentation Presentation {
			get { return (FourteenSegmentsPresentation)GetValue(PresentationProperty); }
			set { SetValue(PresentationProperty, value); }
		}
		public FourteenSegmentsView() {
			InitializeMapping();
		}
		void InitializeMapping() {
			StatesMaskConverter converter = new StatesMaskConverter();						
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = '0', SegmentsStates = (StatesMask)converter.ConvertFromString("1 1 1 1 1 1 0 0 0 0 1 0 1 0") });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = '1', SegmentsStates = (StatesMask)converter.ConvertFromString("0 1 1 0 0 0 0 0 0 0 0 0 0 0") });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = '2', SegmentsStates = (StatesMask)converter.ConvertFromString("1 1 0 1 1 0 0 1 0 1 0 0 0 0") });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = '3', SegmentsStates = (StatesMask)converter.ConvertFromString("1 1 1 1 0 0 0 1 0 1 0 0 0 0") });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = '4', SegmentsStates = (StatesMask)converter.ConvertFromString("0 1 1 0 0 1 0 1 0 1 0 0 0 0") });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = '5', SegmentsStates = (StatesMask)converter.ConvertFromString("1 0 1 1 0 1 0 1 0 1 0 0 0 0") });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = '6', SegmentsStates = (StatesMask)converter.ConvertFromString("1 0 1 1 1 1 0 1 0 1 0 0 0 0") });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = '7', SegmentsStates = (StatesMask)converter.ConvertFromString("1 1 1 0 0 0 0 0 0 0 0 0 0 0") });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = '8', SegmentsStates = (StatesMask)converter.ConvertFromString("1 1 1 1 1 1 0 1 0 1 0 0 0 0") });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = '9', SegmentsStates = (StatesMask)converter.ConvertFromString("1 1 1 1 0 1 0 1 0 1 0 0 0 0") });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = '-', SegmentsStates = (StatesMask)converter.ConvertFromString("0 0 0 0 0 0 0 1 0 1 0 0 0 0") });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = '+', SegmentsStates = (StatesMask)converter.ConvertFromString("0 0 0 0 0 0 1 1 1 1 0 0 0 0") });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = '/', SegmentsStates = (StatesMask)converter.ConvertFromString("0 0 0 0 0 0 0 0 0 0 1 0 1 0") });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = '*', SegmentsStates = (StatesMask)converter.ConvertFromString("0 0 0 0 0 0 1 1 1 1 1 1 1 1") });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = 'A', SegmentsStates = (StatesMask)converter.ConvertFromString("1 1 1 0 1 1 0 1 0 1 0 0 0 0") });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = 'B', SegmentsStates = (StatesMask)converter.ConvertFromString("1 1 1 1 0 0 1 1 1 0 0 0 0 0") });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = 'C', SegmentsStates = (StatesMask)converter.ConvertFromString("1 0 0 1 1 1 0 0 0 0 0 0 0 0") });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = 'D', SegmentsStates = (StatesMask)converter.ConvertFromString("1 1 1 1 0 0 1 0 1 0 0 0 0 0") });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = 'E', SegmentsStates = (StatesMask)converter.ConvertFromString("1 0 0 1 1 1 0 1 0 1 0 0 0 0") });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = 'F', SegmentsStates = (StatesMask)converter.ConvertFromString("1 0 0 0 1 1 0 1 0 1 0 0 0 0") });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = 'G', SegmentsStates = (StatesMask)converter.ConvertFromString("1 0 1 1 1 1 0 1 0 0 0 0 0 0") });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = 'H', SegmentsStates = (StatesMask)converter.ConvertFromString("0 1 1 0 1 1 0 1 0 1 0 0 0 0") });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = 'I', SegmentsStates = (StatesMask)converter.ConvertFromString("1 0 0 1 0 0 1 0 1 0 0 0 0 0") });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = 'J', SegmentsStates = (StatesMask)converter.ConvertFromString("0 1 1 1 1 0 0 0 0 0 0 0 0 0") });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = 'K', SegmentsStates = (StatesMask)converter.ConvertFromString("0 0 0 0 1 1 0 0 0 1 1 1 0 0") });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = 'L', SegmentsStates = (StatesMask)converter.ConvertFromString("0 0 0 1 1 1 0 0 0 0 0 0 0 0") });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = 'M', SegmentsStates = (StatesMask)converter.ConvertFromString("0 1 1 0 1 1 0 0 0 0 1 0 0 1") });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = 'N', SegmentsStates = (StatesMask)converter.ConvertFromString("0 1 1 0 1 1 0 0 0 0 0 1 0 1") });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = 'O', SegmentsStates = (StatesMask)converter.ConvertFromString("1 1 1 1 1 1 0 0 0 0 0 0 0 0") });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = 'P', SegmentsStates = (StatesMask)converter.ConvertFromString("1 1 0 0 1 1 0 1 0 1 0 0 0 0") });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = 'Q', SegmentsStates = (StatesMask)converter.ConvertFromString("1 1 1 1 1 1 0 0 0 0 0 1 0 0") });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = 'R', SegmentsStates = (StatesMask)converter.ConvertFromString("1 1 0 0 1 1 0 1 0 1 0 1 0 0") });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = 'S', SegmentsStates = (StatesMask)converter.ConvertFromString("1 0 1 1 0 1 0 1 0 1 0 0 0 0") });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = 'T', SegmentsStates = (StatesMask)converter.ConvertFromString("1 0 0 0 0 0 1 0 1 0 0 0 0 0") });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = 'U', SegmentsStates = (StatesMask)converter.ConvertFromString("0 1 1 1 1 1 0 0 0 0 0 0 0 0") });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = 'V', SegmentsStates = (StatesMask)converter.ConvertFromString("0 0 0 0 1 1 0 0 0 0 1 0 1 0") });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = 'W', SegmentsStates = (StatesMask)converter.ConvertFromString("0 1 1 0 1 1 0 0 0 0 0 1 1 0") });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = 'X', SegmentsStates = (StatesMask)converter.ConvertFromString("0 0 0 0 0 0 0 0 0 0 1 1 1 1") });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = 'Y', SegmentsStates = (StatesMask)converter.ConvertFromString("0 1 0 0 0 1 0 1 1 1 0 0 0 0") });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = 'Z', SegmentsStates = (StatesMask)converter.ConvertFromString("1 0 0 1 0 0 0 0 0 0 1 0 1 0") });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = '.', SegmentsStates = (StatesMask)converter.ConvertFromString("0 0 0 0 0 0 0 0 0 0 0 0 0 0 1"), SymbolType = SymbolType.Additional });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = ',', SegmentsStates = (StatesMask)converter.ConvertFromString("0 0 0 0 0 0 0 0 0 0 0 0 0 0 1"), SymbolType = SymbolType.Additional });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = ':', SegmentsStates = (StatesMask)converter.ConvertFromString("0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1"), SymbolType = SymbolType.Main });
			SymbolMapping.Add(new SymbolSegmentsMapping() { Symbol = '\'', SegmentsStates = (StatesMask)converter.ConvertFromString("0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1"), SymbolType = SymbolType.Additional });
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FourteenSegmentsView();
		}
		protected internal override SymbolViewInternal CreateInternalView() {
			return new FourteenSegmentsViewInternal();
		}
	}
}
namespace DevExpress.Xpf.Gauges.Native {
	public class FourteenSegmentsViewInternal : SegmentsViewInternal {
		FourteenSegmentsModel Model { get { return Gauge != null ? Gauge.ActualModel.FourteenSegmentsModel : null; } }
		FourteenSegmentsPresentation Presentation { get { return ((FourteenSegmentsView)View).Presentation; } }
		internal override double DefaultHeightToWidthRatio { get { return 122.0 / 86.0; } }
		protected override SymbolsModelBase ModelBase { get { return Model; } }
		protected override SymbolPresentation ActualPresentation {
			get {
				if (Presentation != null)
					return Presentation;
				if (Model != null && Model.Presentation != null)
					return Model.Presentation;
				return new DefaultFourteenSegmentsPresentation();
			}
		}
	}
}
