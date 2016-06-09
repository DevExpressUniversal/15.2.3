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
using System.Windows;
using System.Windows.Media;
namespace DevExpress.Xpf.Gauges.Native {
	public interface IOwnedElement {
		object Owner { get; set; }
	}
	public interface IModelSupported {
		void UpdateModel();	   
	}
	public interface IGaugeLayoutElement {
		ElementLayout Layout { get; }
		Point Offset { get; }
		Point RenderTransformOrigin { get; }
		bool InfluenceOnGaugeSize { get; }
	}
	public interface IScaleLayoutElement {
		ScaleElementLayout Layout { get; }
		Point RenderTransformOrigin { get; }
	}
	public interface IElementInfo {
		void Invalidate();
	}
	public interface ILayoutCalculator {
		ElementLayout CreateLayout(Size constraint);
		void CompleteLayout(ElementInfoBase elementInfo);
	}
	public interface INamedElement {
		string Name { get; }
	}
	public interface IAnimatableElement {
		bool InProgress { get; }
		void ProgressChanged();
	}
	public interface IHitTestableElement {
		Object Element { get; }
		Object Parent { get; }
		bool IsHitTestVisible { get; }
	}
	public interface IDefaultSymbolPresentation {
		Brush ActualFillActive { get; }
		Brush ActualFillInactive { get; }
	}
	public interface ILogicalParent {
		void AddChild(object child);
		void RemoveChild(object child);
	}
}
