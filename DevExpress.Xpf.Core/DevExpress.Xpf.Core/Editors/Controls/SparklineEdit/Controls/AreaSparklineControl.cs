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

using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Editors.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
namespace DevExpress.Xpf.Editors {
	public class AreaSparklineControl : LineSparklineControl {
		#region Dependency Properties
		public static readonly DependencyProperty AreaOpacityProperty = DependencyProperty.Register("AreaOpacity", typeof(Double), typeof(AreaSparklineControl),
				new FrameworkPropertyMetadata(defaultAreaOpacity, (d, e) => ((AreaSparklineControl)d).OnAreaOpacityChanged((Double)e.NewValue)));
		#endregion
		const double defaultAreaOpacity = 0.5;
		double areaOpacity = defaultAreaOpacity;
		internal double ActualAreaOpacity { get { return areaOpacity; } }
		public double AreaOpacity {
			get { return (double)GetValue(AreaOpacityProperty); }
			set { SetValue(AreaOpacityProperty, value); }
		}
		public override SparklineViewType Type {
			get { return SparklineViewType.Area; }
		}
		void OnAreaOpacityChanged(double areaOpacity) {
			this.areaOpacity = areaOpacity;
			PropertyChanged();
		}
		protected override string GetViewName() {
			return EditorLocalizer.GetString(EditorStringId.SparklineViewArea);
		}
		protected internal override BaseSparklinePainter CreatePainter() {
			return new AreaSparklinePainter();
		}
		public override void Assign(SparklineControl view) {
			base.Assign(view);
			AreaSparklineControl areaView = view as AreaSparklineControl;
			if(areaView != null) {
				areaOpacity = areaView.areaOpacity;
			}
		}
	}
}
