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
using DevExpress.Mvvm.UI.Native;
using System.Windows;
using DevExpress.Diagram.Core;
using DevExpress.Xpf.Core;
using DevExpress.XtraReports.UI;
using DevExpress.Mvvm;
using System.Drawing.Printing;
using System.Windows.Input;
using DevExpress.XtraPrinting;
using DevExpress.Xpf.Reports.UserDesigner.Layout.Native;
using DevExpress.XtraPrinting.Native;
using System.Collections.Generic;
using DevExpress.Xpf.DataAccess.DataSourceWizard;
using System;
namespace DevExpress.Xpf.Reports.UserDesigner.Editors {
	public class PageSetupEditor : Control {
		public static readonly DependencyProperty DpiProperty;
		public static readonly DependencyProperty EditValueProperty;
		public static readonly DependencyProperty IsLandscapeProperty;
		public static readonly DependencyProperty PaperKindProperty;
		public static readonly DependencyProperty LeftMarginProperty;
		public static readonly DependencyProperty RightMarginProperty;
		public static readonly DependencyProperty TopMarginProperty;
		public static readonly DependencyProperty BottomMarginProperty;
		static readonly DependencyPropertyKey MaxLeftMarginValuePropertyKey;
		public static readonly DependencyProperty MaxLeftMarginValueProperty;
		static readonly DependencyPropertyKey MaxRightMarginValuePropertyKey;
		public static readonly DependencyProperty MaxRightMarginValueProperty;
		static readonly DependencyPropertyKey MaxTopMarginValuePropertyKey;
		public static readonly DependencyProperty MaxTopMarginValueProperty;
		static readonly DependencyPropertyKey MaxBottomMarginValuePropertyKey;
		public static readonly DependencyProperty MaxBottomMarginValueProperty;
		static PageSetupEditor() {
			DependencyPropertyRegistrator<PageSetupEditor>.New()
				.Register(d => d.Dpi, out DpiProperty, 0f)
				.Register(d => d.EditValue, out EditValueProperty, null, d => d.OnEditValueChanged(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
				.Register(d => d.IsLandscape, out IsLandscapeProperty, false, d => d.OnMarginsChanged())
				.Register(d => d.LeftMargin, out LeftMarginProperty, 0, d => d.OnMarginsChanged())
				.Register(d => d.RightMargin, out RightMarginProperty, 0, d => d.OnMarginsChanged())
				.Register(d => d.TopMargin, out TopMarginProperty, 0, d => d.OnMarginsChanged())
				.Register(d => d.BottomMargin, out BottomMarginProperty, 0, d => d.OnMarginsChanged())
				.Register(d => d.PaperKind, out PaperKindProperty, PaperKind.Letter, d => d.OnMarginsChanged())
				.RegisterReadOnly(d => d.MaxLeftMarginValue, out MaxLeftMarginValuePropertyKey, out MaxLeftMarginValueProperty, 0f)
				.RegisterReadOnly(d => d.MaxRightMarginValue, out MaxRightMarginValuePropertyKey, out MaxRightMarginValueProperty, 0f)
				.RegisterReadOnly(d => d.MaxTopMarginValue, out MaxTopMarginValuePropertyKey, out MaxTopMarginValueProperty, 0f)
				.RegisterReadOnly(d => d.MaxBottomMarginValue, out MaxBottomMarginValuePropertyKey, out MaxBottomMarginValueProperty, 0f)
				.OverrideDefaultStyleKey();
		}
		readonly Lazy<IEnumerable<BooleanViewModel>> orientations = BooleanViewModel.CreateList("Portrait", "Landscape", false);
		public IEnumerable<BooleanViewModel> Orientations { get { return orientations.Value; } } 
		public SelectionModel<IDiagramItem> EditValue {
			get { return (SelectionModel<IDiagramItem>)GetValue(EditValueProperty); }
			set { SetValue(EditValueProperty, value); }
		}
		public PaperKind PaperKind {
			get { return (PaperKind)GetValue(PaperKindProperty); }
			set { SetValue(PaperKindProperty, value); }
		}
		public bool IsLandscape {
			get { return (bool)GetValue(IsLandscapeProperty); }
			set { SetValue(IsLandscapeProperty, value); }
		}
		public float LeftMargin {
			get { return (float)GetValue(LeftMarginProperty); }
			set { SetValue(LeftMarginProperty, value); }
		}
		public float RightMargin {
			get { return (float)GetValue(RightMarginProperty); }
			set { SetValue(RightMarginProperty, value); }
		}
		public float TopMargin {
			get { return (float)GetValue(TopMarginProperty); }
			set { SetValue(TopMarginProperty, value); }
		}
		public float BottomMargin {
			get { return (float)GetValue(BottomMarginProperty); }
			set { SetValue(BottomMarginProperty, value); }
		}
		public float Dpi {
			get { return (float)GetValue(DpiProperty); }
			set { SetValue(DpiProperty, value); }
		}
		public float MaxLeftMarginValue {
			get { return (float)GetValue(MaxLeftMarginValueProperty); }
			private set { SetValue(MaxLeftMarginValuePropertyKey, value); }
		}
		public float MaxRightMarginValue {
			get { return (float)GetValue(MaxRightMarginValueProperty); }
			private set { SetValue(MaxRightMarginValuePropertyKey, value); }
		}
		public float MaxTopMarginValue {
			get { return (float)GetValue(MaxTopMarginValueProperty); }
			private set { SetValue(MaxTopMarginValuePropertyKey, value); }
		}
		public float MaxBottomMarginValue {
			get { return (float)GetValue(MaxBottomMarginValueProperty); }
			private set { SetValue(MaxBottomMarginValuePropertyKey, value); }
		}
		DelegateCommand saveCommand;
		public ICommand SaveCommand {
			get {
				if(saveCommand == null)
					saveCommand = new DelegateCommand(Save, false);
				return saveCommand;
			}
		}
		void OnEditValueChanged() {
			if(EditValue == null) return;
			IsLandscape = EditValue.GetPropertyValueEx((XtraReport x) => x.Landscape);
			PaperKind = EditValue.GetPropertyValueEx((XtraReport x) => x.PaperKind);
			var reportMargins = EditValue.GetPropertyValueEx((XtraReport x) => x.Margins);
			LeftMargin = reportMargins.Left / Dpi;
			RightMargin = reportMargins.Right / Dpi;
			TopMargin = reportMargins.Top / Dpi;
			BottomMargin = reportMargins.Bottom / Dpi;
		}
		void Save() {
			if(EditValue == null) return;
			EditValue.SetPropertyValueEx((XtraReport x) => x.Landscape, IsLandscape);
			EditValue.SetPropertyValueEx((XtraReport x) => x.PaperKind, PaperKind);
			var margins = new Margins((int)(Dpi * LeftMargin), (int)(Dpi * RightMargin), (int)(Dpi * TopMargin), (int)(Dpi * BottomMargin));
			EditValue.SetPropertyValueEx((XtraReport x) => x.Margins, margins);
		}
		System.Drawing.Size PageSizeInInches { get { return PageSizeInfo.GetPageSize(PaperKind, System.Drawing.GraphicsUnit.Inch); } }
		void OnMarginsChanged() {
			MaxLeftMarginValue = PageSizeInInches.Width - RightMargin;
			MaxRightMarginValue = PageSizeInInches.Width - LeftMargin;
			MaxTopMarginValue = PageSizeInInches.Height - BottomMargin;
			MaxBottomMarginValue = PageSizeInInches.Height - TopMargin;
		}
	}
}
