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
using System;
using System.ComponentModel;
using System.Windows;
namespace DevExpress.Xpf.Editors {
	public abstract class BarSparklineControlBase : SparklineControl {
		#region Dependency Properties
		public static readonly DependencyProperty BarDistanceProperty = DependencyProperty.Register("BarDistance", typeof(Int32), typeof(BarSparklineControlBase),
				new FrameworkPropertyMetadata(defaultBarDistance, (d, e) => ((BarSparklineControlBase)d).OnBarDistanceChanged((Int32)e.NewValue)));
		#endregion
		const int defaultBarDistance = 2;
		int barDistance = defaultBarDistance;
		internal int ActualBarDistance { get { return barDistance; } }
		public int BarDistance {
			get { return (int)GetValue(BarDistanceProperty); }
			set { SetValue(BarDistanceProperty, value); }
		}
		void OnBarDistanceChanged(int barDistance) {
			this.barDistance = barDistance;
			PropertyChanged();
		}
		public override void Assign(SparklineControl view) {
			base.Assign(view);
			BarSparklineControlBase barView = view as BarSparklineControlBase;
			if(barView != null) {
				barDistance = barView.barDistance;
			}
		}
	}
}
