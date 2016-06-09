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
	public class MatrixView5x8 : MatrixView {
		public static readonly DependencyProperty PresentationProperty = DependencyPropertyManager.Register("Presentation",
			typeof(Matrix5x8Presentation), typeof(MatrixView5x8), new PropertyMetadata(null, PresentationPropertyChanged));
		[Category(Categories.Presentation)]
		public Matrix5x8Presentation Presentation {
			get { return (Matrix5x8Presentation)GetValue(PresentationProperty); }
			set { SetValue(PresentationProperty, value); }
		}
		protected override GaugeDependencyObject CreateObject() {
			return new MatrixView5x8();
		}
		protected internal override SymbolViewInternal CreateInternalView() {
			return new MatrixView5x8Internal();
		}
	}
}
namespace DevExpress.Xpf.Gauges.Native {
	public class MatrixView5x8Internal : MatrixViewInternal {
		Matrix5x8Model Model { get { return Gauge != null ? Gauge.ActualModel.Matrix5x8Model : null; } }
		Matrix5x8Presentation Presentation { get { return ((MatrixView5x8)View).Presentation; } }
		protected override SymbolsModelBase ModelBase { get { return Model; } }
		protected override SymbolPresentation ActualPresentation {
			get {
				if (Presentation != null)
					return Presentation;
				if (Model != null && Model.Presentation != null)
					return Model.Presentation;
				return new DefaultMatrix5x8Presentation();
			}
		}
		internal override double DefaultHeightToWidthRatio { get { return 1.6; } }
		protected internal override int SymbolWidth { get { return 5; } }
		protected internal override int SymbolHeight { get { return 8; } }
		protected internal override SymbolState GetEmptySymbolState() {
			return new SymbolState(string.Empty, 40, false);
		}
	}
}
