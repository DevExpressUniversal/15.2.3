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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Animation;
namespace DevExpress.Xpf.Editors {
	public class ProgressBarEditStrategy : RangeBaseEditStrategyBase {
		new ProgressBarEdit Editor { get { return base.Editor as ProgressBarEdit; } }
		public ProgressBarEditStrategy(BaseEdit editor)
			: base(editor) {
		}
		public virtual void IsPercentChanged(bool value) {
			UpdateDisplayText();
		}
		public override string CoerceDisplayText(string displayText) {
			UpdateDisplayValue();
			string text = IsInSupportInitialize ? string.Empty : GetDisplayText();
			return base.CoerceDisplayText(text);
		}
		protected internal override bool ShouldProcessNullInput(System.Windows.Input.KeyEventArgs e) {
			return false;
		}
		protected override void UpdateDisplayValue() {
			double value = ObjectToDoubleConverter.TryConvert(ValueContainer.EditValue);
			Info.DisplayValue = Editor.IsPercent ? (value - Editor.Minimum) / GetRange() : value;
		}
		protected override object GetValueForDisplayText() {
			return Info.DisplayValue;
		}
#if SL
		public virtual void ContentDisplayModeChanged(ContentDisplayMode value) {
			UpdateContentTemplate(value);
		}
		public virtual void UpdateContentTemplate(ContentDisplayMode value) {
			if (value == ContentDisplayMode.Value)
				Editor.ContentTemplate = Editor.ContentDisplayModeContentTemplate;
			else
				Editor.ContentTemplate = Editor.ContentDisplayModeNoneTemplate;
		}
#else
		public virtual void ContentDisplayModeChanged(ContentDisplayMode value) {
		}
#endif
	}
}
