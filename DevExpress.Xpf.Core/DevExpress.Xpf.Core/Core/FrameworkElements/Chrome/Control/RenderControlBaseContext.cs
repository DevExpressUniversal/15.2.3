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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Mvvm.Native;
using System.Linq;
namespace DevExpress.Xpf.Core.Native {
	public abstract class RenderControlBaseContext : FrameworkRenderElementContext {
		HorizontalAlignment hca;
		VerticalAlignment vca;
		public HorizontalAlignment HorizontalContentAlignment {
			get { return hca; }
			set { SetProperty(ref hca, value, FREInvalidateOptions.UpdateLayout, HorizontalContentAlignmentChanged); }
		}
		public VerticalAlignment VerticalContentAlignment {
			get { return vca; }
			set { SetProperty(ref vca, value, FREInvalidateOptions.UpdateLayout, VerticalContentAlignmentChanged); }
		}
		public override bool AttachToRoot { get { return Visibility != null ? Visibility.Value == System.Windows.Visibility.Visible : Factory.Visibility == System.Windows.Visibility.Visible; } }
		public virtual FrameworkElement Control { get; internal set; }
		public Transform GeneralTransform { get; protected set; }
		protected RenderControlBaseContext(RenderControlBase factory)
			: base(factory) {
		}
		protected bool CanSetValue(string propertyName) {
			return IsContextProperty(propertyName) || Control != null;
		}
		protected virtual bool IsContextProperty(string propertyName) { return string.Equals("Opacity", propertyName); }
		protected override void SetValueOverride(string propertyName, object value) {
			if (!CanSetValue(propertyName))
				return;
			if(object.Equals(GetValue(propertyName),value))
				return;
			if(IsContextProperty(propertyName))
				base.SetValueOverride(propertyName, value);
			else
				RenderTriggerHelper.SetConvertedValue(Control, propertyName, value);
		}
		protected override void ResetValueOverride(string propertyName) {
			if (!CanSetValue(propertyName))
				return;
			if(IsContextProperty(propertyName))
				base.ResetValueOverride(propertyName);
			else
				RenderTriggerHelper.SetConvertedValue(Control, propertyName, null);
		}
		protected override object GetValueOverride(string propertyName) {
			if (!CanSetValue(propertyName))
				return null;
			if(IsContextProperty(propertyName))
				return base.GetValueOverride(propertyName);
			else
				return RenderTriggerHelper.GetValue(Control, propertyName);
		}
		protected override void UpdateOpacity() {
			Control.Opacity = ActualOpacity * RenderTreeHelper.RenderAncestors(this).Select(x => x.ActualOpacity).Aggregate((a, b) => a * b);
		}
		Matrix currentMatrixTransform = Matrix.Identity;
		public override bool ShouldUseTransform() {
			base.ShouldUseTransform();
			return true;
		}
		public override void UpdateRenderTransform() {
			base.UpdateRenderTransform();
			UpdateGeneralTransform();
		}
		protected virtual void UpdateGeneralTransform() {
			double x = VisualOffset.X, y = VisualOffset.Y;
			foreach (var anc in RenderTreeHelper.RenderAncestors(this)) {
				x += anc.VisualOffset.X;
				y += anc.VisualOffset.Y;
			}
			var translateTransform = Matrix.Identity;
			translateTransform.Translate(x, y);
			if (currentMatrixTransform.Equals(translateTransform)) {
				GeneralTransform = Transform.Identity;
				return;
			}
			var mt = new MatrixTransform(translateTransform);
			mt.Freeze();
			GeneralTransform = mt;
		}
		protected virtual void HorizontalContentAlignmentChanged() {
			var control = Control as Control;
			if (control != null)
				control.HorizontalContentAlignment = HorizontalContentAlignment;
		}
		protected virtual void VerticalContentAlignmentChanged() {
			var control = Control as Control;
			if (control != null)
				control.VerticalContentAlignment = VerticalContentAlignment;
		}
		protected override void OnForegroundChanged(object oldValue, object newValue) {
			base.OnForegroundChanged(oldValue, newValue);
			UpdateControlForeground();
		}
		protected internal virtual void UpdateControlForeground() {
			if (Control != null)
				Control.SetCurrentValue(System.Windows.Documents.TextElement.ForegroundProperty, Foreground);
		}
	}
}
