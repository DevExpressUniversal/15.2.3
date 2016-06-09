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

using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Editors.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
namespace DevExpress.Xpf.Editors {
	public class BarSparklineControl : BarSparklineControlBase, ISupportNegativePointsControl {
		#region Dependency Properties
		public static readonly DependencyProperty HighlightNegativePointsProperty = DependencyProperty.Register("HighlightNegativePoints", typeof(Boolean), typeof(BarSparklineControl),
				new FrameworkPropertyMetadata(defaultHighlightNegativePoints, (d, e) => ((BarSparklineControl)d).OnHighlightNegativePointsChanged((Boolean)e.NewValue)));
		#endregion
		const bool defaultHighlightNegativePoints = false;
		bool highlightNegativePoints;
		protected internal override bool ActualShowNegativePoint { get { return ActualHighlightNegativePoints; } }
		internal bool ActualHighlightNegativePoints { get { return highlightNegativePoints; } }
		public bool HighlightNegativePoints {
			get { return (bool)GetValue(HighlightNegativePointsProperty); }
			set { SetValue(HighlightNegativePointsProperty, value); }
		}
		public override SparklineViewType Type { get { return SparklineViewType.Bar; } }
		void OnHighlightNegativePointsChanged(bool highlightNegativePoints) {
			this.highlightNegativePoints = highlightNegativePoints;
			PropertyChanged();
		}
		protected override string GetViewName() {
			return EditorLocalizer.GetString(EditorStringId.SparklineViewBar);
		}
		protected internal override BaseSparklinePainter CreatePainter() {
			return new BarSparklinePainter();
		}
		public override void Assign(SparklineControl view) {
			base.Assign(view);
			ISupportNegativePointsControl negativePointsView = view as ISupportNegativePointsControl;
			if(negativePointsView != null) {
				highlightNegativePoints = negativePointsView.HighlightNegativePoints;
			}
		}
	}
}
