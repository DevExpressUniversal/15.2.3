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
using System.Windows;
using System.Windows.Media.Animation;
using System.ComponentModel;
#if !SL
using DevExpress.Xpf.Editors.Themes;
#endif
namespace DevExpress.Xpf.Editors {
	public abstract partial class BaseProgressBarStyleSettings : BaseEditStyleSettings {
		public static readonly DependencyProperty AccelerateRatioProperty;
		static BaseProgressBarStyleSettings() {
			AccelerateRatioProperty = DependencyProperty.Register("AccelerateRatio", typeof(double), typeof(BaseProgressBarStyleSettings), new PropertyMetadata(1d, null));
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BaseProgressBarStyleSettingsAccelerateRatio")]
#endif
		public double AccelerateRatio {
			get { return (double)GetValue(AccelerateRatioProperty); }
			set { SetValue(AccelerateRatioProperty, value); }
		}
		protected abstract bool IsIndeterminate { get; }
		partial void ApplyPanelStyle(ProgressBarEdit editor);
		public override void ApplyToEdit(BaseEdit editor) {
			base.ApplyToEdit(editor);
			ProgressBarEdit progressBarEditor = editor as ProgressBarEdit;
			if(progressBarEditor == null)
				return;
			if(progressBarEditor.Panel == null)
				return;
			ApplyPanelStyle(progressBarEditor);
			progressBarEditor.IsIndeterminate = IsIndeterminate;
		}
	}
	public partial class ProgressBarStyleSettings : BaseProgressBarStyleSettings {
		protected override bool IsIndeterminate { get { return false; } }
	}
	public partial class ProgressBarMarqueeStyleSettings : BaseProgressBarStyleSettings {
		protected override bool IsIndeterminate { get { return true; } }
	}
}
