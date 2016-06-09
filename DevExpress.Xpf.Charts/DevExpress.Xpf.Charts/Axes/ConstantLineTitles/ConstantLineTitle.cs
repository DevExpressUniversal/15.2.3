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
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public enum ConstantLineTitleAlignment {
		Near,
		Far
	}
	public class ConstantLineTitle : AxisElementTitleBase {
		public static readonly DependencyProperty AlignmentProperty = DependencyPropertyManager.Register("Alignment", 
			typeof(ConstantLineTitleAlignment), typeof(ConstantLineTitle), new PropertyMetadata(ConstantLineTitleAlignment.Near, ChartElementHelper.Update));
		public static readonly DependencyProperty ShowBelowLineProperty = DependencyPropertyManager.Register("ShowBelowLine", typeof(bool), typeof(ConstantLineTitle), new PropertyMetadata(false, ChartElementHelper.Update));
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("ConstantLineTitleAlignment"),
#endif
		Category(Categories.Layout),
		XtraSerializableProperty
		]
		public ConstantLineTitleAlignment Alignment {
			get { return (ConstantLineTitleAlignment)GetValue(AlignmentProperty); }
			set { SetValue(AlignmentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("ConstantLineTitleShowBelowLine"),
#endif
		Category(Categories.Layout),
		XtraSerializableProperty
		]
		public bool ShowBelowLine {
			get { return (bool)GetValue(ShowBelowLineProperty); }
			set { SetValue(ShowBelowLineProperty, value); }
		}
		internal ConstantLine ConstantLine { get { return ((IOwnedElement)this).Owner as ConstantLine; } }
		protected internal override Axis Axis {
			get {
				ConstantLine constantLine = ConstantLine;
				return constantLine == null ? null : constantLine.Axis;
			}
		}
		protected internal override object HitTestableElement { get { return ConstantLine; } }
		protected internal override bool ShouldRotate { 
			get { 
				Axis axis = Axis;
				return axis != null && !axis.IsVertical;
			} 
		}
		protected internal override double RotateAngle { get { return -90; } }
		public ConstantLineTitle() {
			DefaultStyleKey = typeof(ConstantLineTitle);
		}
	}
}
