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
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Charts {
	[NonCategorized]
	public class IndicatorPresentation : ChartElementBase, IHitTestableElement, IFinishInvalidation {
		IndicatorItem indicatorItem;
		internal bool Visible { get { return indicatorItem != null && indicatorItem.Visible; } }
		public IndicatorItem IndicatorItem { 
			get { return indicatorItem; }
			set { indicatorItem = value;  }
		}
		public IndicatorPresentation() {
			DefaultStyleKey = typeof(IndicatorPresentation);
		}
		#region IHitTestableElement implementation
		object IHitTestableElement.Element { get { return indicatorItem.Indicator; } }
		object IHitTestableElement.AdditionalElement { get { return null; } }
		#endregion
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if (IndicatorItem != null)
				IndicatorItem.UpdateSelection();
		}
		protected override Size MeasureOverride(Size constraint) {
			base.MeasureOverride(constraint);
			return new Size(MathUtils.ConvertInfinityToDefault(constraint.Width, 0), MathUtils.ConvertInfinityToDefault(constraint.Height, 0));
		}
	}
}
