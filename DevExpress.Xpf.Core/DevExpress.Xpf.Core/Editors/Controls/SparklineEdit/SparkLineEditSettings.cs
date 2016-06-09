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

#if !SL
#else
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.WPFCompatibility;
using DevExpress.Xpf.Editors.WPFCompatibility.Extensions;
using DevExpress.Data.Mask;
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Editors.Automation;
#endif
#if SL
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using System.Windows.Media.Effects;
#endif
using System;
using System.Windows;
using DevExpress.Xpf.Utils;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Editors.Internal;
using System.Windows.Data;
namespace DevExpress.Xpf.Editors.Settings {
	public class SparklineEditSettings : BaseEditSettings {
		public static readonly DependencyProperty PointValueMemberProperty;
		public static readonly DependencyProperty PointArgumentMemberProperty;
		public static readonly DependencyProperty PointArgumentSortOrderProperty;
		public static readonly DependencyProperty FilterCriteriaProperty;
		public static readonly DependencyProperty PointArgumentRangeProperty;
		public static readonly DependencyProperty PointValueRangeProperty;
		static SparklineEditSettings() {
			Type ownerType = typeof(SparklineEditSettings);
			PointValueMemberProperty = DependencyPropertyManager.Register("PointValueMember", typeof(string), ownerType, new FrameworkPropertyMetadata(string.Empty));
			PointArgumentMemberProperty = DependencyPropertyManager.Register("PointArgumentMember", typeof(string), ownerType, new FrameworkPropertyMetadata(string.Empty));
			PointArgumentSortOrderProperty = DependencyProperty.Register("PointArgumentSortOrder", typeof(SparklineSortOrder), ownerType, new FrameworkPropertyMetadata(null));
			FilterCriteriaProperty = DependencyProperty.Register("FilterCriteria", typeof(CriteriaOperator), ownerType, new FrameworkPropertyMetadata(null));
			PointArgumentRangeProperty = DependencyProperty.Register("PointArgumentRange", typeof(Range), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, (o, args) => ((SparklineEditSettings)o).OnPointArgumentRangeChanged((Range)args.NewValue)));
			PointValueRangeProperty = DependencyProperty.Register("PointValueRange", typeof(Range), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, (o, args) => ((SparklineEditSettings)o).OnPointValueRangeChanged((Range)args.NewValue)));
		}
		public string PointArgumentMember {
			get { return (string)GetValue(PointArgumentMemberProperty); }
			set { SetValue(PointArgumentMemberProperty, value); }
		}
		public string PointValueMember {
			get { return (string)GetValue(PointValueMemberProperty); }
			set { SetValue(PointValueMemberProperty, value); }
		}
		public SparklineSortOrder PointArgumentSortOrder {
			get { return (SparklineSortOrder)GetValue(PointArgumentSortOrderProperty); }
			set { SetValue(PointArgumentSortOrderProperty, value); }
		}
		public CriteriaOperator FilterCriteria {
			get { return (CriteriaOperator)GetValue(FilterCriteriaProperty); }
			set { SetValue(FilterCriteriaProperty, value); }
		}
		public Range PointArgumentRange {
			get { return (Range)GetValue(PointArgumentRangeProperty); }
			set { SetValue(PointArgumentRangeProperty, value); }
		}
		public Range PointValueRange {
			get { return (Range)GetValue(PointValueRangeProperty); }
			set { SetValue(PointValueRangeProperty, value); }
		}
		void OnPointValueRangeChanged(Range range) {
			AddLogicalChild(range);
		}
		void OnPointArgumentRangeChanged(Range range) {
			AddLogicalChild(range);
		}
		void bindToArgumentRange(SparklineEdit editor) {
			editor.PointArgumentRange = new Range();
			Binding bindingLimit1 = new Binding("Limit1") { Source = this.PointArgumentRange };
			bindingLimit1.Mode = BindingMode.OneWay;
			BindingOperations.SetBinding(editor.PointArgumentRange, Range.Limit1Property, bindingLimit1);
			Binding bindingLimit2 = new Binding("Limit2") { Source = this.PointArgumentRange };
			bindingLimit2.Mode = BindingMode.OneWay;
			BindingOperations.SetBinding(editor.PointArgumentRange, Range.Limit2Property, bindingLimit2);
			Binding bindingAuto = new Binding("Auto") { Source = this.PointArgumentRange };
			bindingAuto.Mode = BindingMode.OneWay;
			BindingOperations.SetBinding(editor.PointArgumentRange, Range.AutoProperty, bindingAuto);
		}
		void bindToValueRange(SparklineEdit editor) {
			editor.PointValueRange = new Range();
			Binding bindingLimit1 = new Binding("Limit1") { Source = this.PointValueRange };
			bindingLimit1.Mode = BindingMode.OneWay;
			BindingOperations.SetBinding(editor.PointValueRange, Range.Limit1Property, bindingLimit1);
			Binding bindingLimit2 = new Binding("Limit2") { Source = this.PointValueRange };
			bindingLimit2.Mode = BindingMode.OneWay;
			BindingOperations.SetBinding(editor.PointValueRange, Range.Limit2Property, bindingLimit2);
			Binding bindingAuto = new Binding("Auto") { Source = this.PointValueRange };
			bindingAuto.Mode = BindingMode.OneWay;
			BindingOperations.SetBinding(editor.PointValueRange, Range.AutoProperty, bindingAuto);
		}
		protected override void AssignToEditCore(IBaseEdit edit) {
			base.AssignToEditCore(edit);
			SparklineEdit editor = edit as SparklineEdit;
			if (editor == null)
				return;
			SetValueFromSettings(PointValueMemberProperty, () => editor.PointValueMember = PointValueMember);
			SetValueFromSettings(PointArgumentMemberProperty, () => editor.PointArgumentMember = PointArgumentMember);
			SetValueFromSettings(PointArgumentSortOrderProperty, () => editor.PointArgumentSortOrder = PointArgumentSortOrder);
			SetValueFromSettings(FilterCriteriaProperty, () => editor.FilterCriteria = FilterCriteria);
			bindToArgumentRange(editor);
			bindToValueRange(editor);
		}
	}
}
