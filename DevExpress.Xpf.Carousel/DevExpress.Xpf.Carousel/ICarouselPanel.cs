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
namespace DevExpress.Xpf.Carousel {
	public interface ICarouselPanel {
		bool ActivateItemOnClick { get; set; }
		System.Windows.UIElement ActiveItem { get; set; }
		int AttractorPointIndex { get; set; }
		bool AnimationDisabled { get; set; }
		double AnimationTime { get; set; }
		int FirstVisibleItemIndex { get; set; }
		bool IsInvertedDirection { get; set; }
		bool IsRepeat { get; set; }
		System.Windows.Media.PathGeometry ItemMovingPath { get; set; }
		void Move(int offset);
		void MoveFirstPage();
		void MoveLastPage();
		void MoveNext();
		void MoveNextPage();
		void MovePrev();
		void MovePrevPage();
		FunctionBase OffsetDistributionFunction { get; set; }
		FunctionBase OffsetAnimationAddFunction { get; set; }
		ParameterCollection ParameterSet { get; set; }
		System.Windows.Thickness PathPadding { get; set; }
		bool PathVisible { get; set; }
		double SmoothingTimeFraction { get; set; }
		int VisibleItemCount { get; set; }
	}
}
