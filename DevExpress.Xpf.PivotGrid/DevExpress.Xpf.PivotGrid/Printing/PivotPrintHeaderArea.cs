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
using DevExpress.Xpf.PivotGrid.Internal;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
#else
using DependencyPropertyManager = System.Windows.DependencyProperty;
using DevExpress.Xpf.Utils;
#endif
namespace DevExpress.Xpf.PivotGrid.Printing {
	public class PivotPrintHeaderArea : FieldHeaders {
		public const int TopPadding = 4;
		public const int LeftPadding = 2;
		#region static
		public static readonly DependencyProperty HasBottomBorderProperty;
		public static readonly DependencyProperty HasTopBorderProperty;
		static PivotPrintHeaderArea() {
			Type ownerType = typeof(PivotPrintHeaderArea);
			HasBottomBorderProperty = DependencyPropertyManager.Register("HasBottomBorder", typeof(bool), ownerType, new PropertyMetadata(true, (d, e) => ((PivotPrintHeaderArea)d).OnHasBottomBorderChanged()));
			HasTopBorderProperty = DependencyPropertyManager.Register("HasTopBorder", typeof(bool), ownerType, new PropertyMetadata(true, (d, e) => ((PivotPrintHeaderArea)d).OnHasTopBorderChanged()));
		}
		#endregion
		public PivotPrintHeaderArea() { }
		protected override void SetDefaultStyleKey() {
			this.SetDefaultStyleKey(typeof(PivotPrintHeaderArea));
		}
		public bool HasBottomBorder {
			get { return (bool)GetValue(HasBottomBorderProperty); }
			set { SetValue(HasBottomBorderProperty, value); }
		}
		public bool HasTopBorder {
			get { return (bool)GetValue(HasTopBorderProperty); }
			set { SetValue(HasTopBorderProperty, value); }
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			base.EnsureItems();
		}
		protected override FieldHeader CreateFieldHeader() {
			return new PrintFieldHeader() {
				BorderThickness = new Thickness(0, HasTopBorder ? 1 : 0, 0, HasBottomBorder ? 1 : 0),
				Padding = new Thickness(TopPadding, HasTopBorder ? LeftPadding : LeftPadding + 1,
					TopPadding, Area != FieldListArea.RowArea || HasBottomBorder ? LeftPadding : LeftPadding + 1)
			};
		}
		void OnHasBottomBorderChanged() {
			EnsureHeadersBorder();
		}
		void OnHasTopBorderChanged() {
			EnsureHeadersBorder();
		}
		void EnsureHeadersBorder() {
			if(Area != FieldListArea.ColumnArea || Panel == null)
				return;
			foreach(UIElement element in Panel.Children) {
				PrintFieldHeader header = element as PrintFieldHeader;
				if(header != null)
					SetHeaderBorder(header);
				PrintGroupHeader groupHeader = element as PrintGroupHeader;
				if(groupHeader != null)
					SetGroupHeaderBorder(groupHeader);
			}
		}
		void SetGroupHeaderBorder(PrintGroupHeader header) {
			SetHeaderBorder(header);
			header.EnsureHeadersBorder();
		}
		void SetHeaderBorder(FieldHeader header) {
			Thickness thickness = header.BorderThickness;
			thickness.Bottom = HasBottomBorder ? 1 : 0;
			thickness.Top = HasTopBorder ? 1 : 0;
			header.BorderThickness = thickness;
		}
		protected override GroupHeader CreateGroupHeader() {
			return new PrintGroupHeader() {
				BorderThickness = new Thickness(0, HasTopBorder ? 1 : 0, 0, HasBottomBorder ? 1 : 0),
				Padding = new Thickness(TopPadding, HasTopBorder ? LeftPadding : LeftPadding + 1,
					TopPadding, Area != FieldListArea.RowArea || HasBottomBorder ? LeftPadding : LeftPadding + 1)
			};
		}
		protected override void SetBestFitDecorator() { }
		protected override IHeadersFieldsSource CreateFieldsSource() {
			return new RealFieldsSource(this, false);
		}
		protected override void SubscribeEvents() { }
		protected override void UnsubscribeEvents() { }
		protected override bool AllowBind(System.Collections.Generic.IList<PivotGridField> fields) {
			return Panel != null && fields != null;
		}
	}
}
