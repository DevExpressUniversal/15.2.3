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
using System.Windows.Media;
namespace DevExpress.Xpf.Charts.Native {
	public class Axis3DMapping : AxisMappingEx {
		readonly double orthSize;
		readonly TransformGroup transform;
		public double OrthSize { get { return orthSize; } }
		public Transform Transform { get { return transform; } }
		protected override double ClampSize { get { return Lenght; } }
		public Axis3DMapping(Axis3D axis, Rect bounds) : base(axis, axis.IsVertical ? bounds.Height : bounds.Width) {
			orthSize = axis.IsValuesAxis ? bounds.Width - 1 : bounds.Height - 1;
			transform = new TransformGroup();
			if (axis.IsValuesAxis)
				transform.Children.Add(new RotateTransform() { Angle = -90 });
			else
				transform.Children.Add(new ScaleTransform() { ScaleX = 1, ScaleY = -1 });
			transform.Children.Add(new TranslateTransform() { X = bounds.Left, Y = bounds.Bottom });
		}
	}
}
