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
using System.Windows.Controls;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.DemoBase {
	partial class SliderControl : Control {
		public static readonly DependencyProperty HeaderProperty;
		public static readonly DependencyProperty UOMProperty;
		public static readonly DependencyProperty EditValueProperty;
		public static readonly DependencyProperty SmallStepProperty;
		public static readonly DependencyProperty LargeStepProperty;
		public static readonly DependencyProperty MinimumProperty;
		public static readonly DependencyProperty MaximumProperty;
		public static readonly DependencyProperty ValueMinWidthProperty;
		public SliderControl() {
			InitializeComponent();
		}
		static SliderControl() {
			Type ownerType = typeof(SliderControl);
			HeaderProperty = DependencyPropertyManager.Register("Header", typeof(string), ownerType, new PropertyMetadata(string.Empty));
			UOMProperty = DependencyPropertyManager.Register("UOM", typeof(string), ownerType, new PropertyMetadata(string.Empty));
			EditValueProperty = DependencyPropertyManager.Register("EditValue", typeof(double), ownerType, new PropertyMetadata());
			SmallStepProperty = DependencyPropertyManager.Register("SmallStep", typeof(double), ownerType, new PropertyMetadata());
			LargeStepProperty = DependencyPropertyManager.Register("LargeStep", typeof(double), ownerType, new PropertyMetadata());
			MinimumProperty = DependencyPropertyManager.Register("Minimum", typeof(double), ownerType, new PropertyMetadata());
			MaximumProperty = DependencyPropertyManager.Register("Maximum", typeof(double), ownerType, new PropertyMetadata());
			ValueMinWidthProperty = DependencyPropertyManager.Register("ValueMinWidth", typeof(double), ownerType, new PropertyMetadata(27.0));
		}
		public string Header {
			get { return (string)GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}
		public string UOM {
			get { return (string)GetValue(UOMProperty); }
			set { SetValue(UOMProperty, value); }
		}
		public double EditValue {
			get { return (double)GetValue(EditValueProperty); }
			set { SetValue(EditValueProperty, value); }
		}
		public double SmallStep {
			get { return (double)GetValue(SmallStepProperty); }
			set { SetValue(SmallStepProperty, value); }
		}
		public double LargeStep {
			get { return (double)GetValue(LargeStepProperty); }
			set { SetValue(LargeStepProperty, value); }
		}
		public double Minimum {
			get { return (double)GetValue(MinimumProperty); }
			set { SetValue(MinimumProperty, value); }
		}
		public double Maximum {
			get { return (double)GetValue(MaximumProperty); }
			set { SetValue(MaximumProperty, value); }
		}
		public double ValueMinWidth {
			get { return (double)GetValue(ValueMinWidthProperty); }
			set { SetValue(ValueMinWidthProperty, value); }
		}
		EventHandler valueChangedHandler;
		public event EventHandler ValueChanged {
			add { valueChangedHandler += value; }
			remove { valueChangedHandler -= value; }
		}
		protected void RaiseValueChanged(EventHandler handler) {
			EventArgs e = new EventArgs();
			if(handler != null)
				handler(this, e);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			TrackBarEdit trackBarEdit = (TrackBarEdit)LayoutHelper.FindElementByName(this, "trackBarEdit");
			if(trackBarEdit != null)
				trackBarEdit.EditValueChanged += new EditValueChangedEventHandler(trackBarEdit_EditValueChanged);
		}
		void trackBarEdit_EditValueChanged(object sender, EditValueChangedEventArgs e) {
			RaiseValueChanged(valueChangedHandler);
		}
	}
}
