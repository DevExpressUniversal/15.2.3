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
namespace DevExpress.Charts.Native {
	public enum SimpleDiagramLayoutDirection {
		Horizontal,
		Vertical
	}
	public interface ISimpleDiagram : IDiagram {
		SimpleDiagramLayoutDirection LayoutDirection { get; }
		int Dimension { get; }
	}
	public class SimpleDiagramLayout {
		delegate GRect2D TransformRectangle(GRect2D rect);
		static GRect2D PassRectangle(GRect2D rect) {
			return rect;
		}
		static GRect2D RotateRectangle(GRect2D rect) {
			return new GRect2D(rect.Top, rect.Left, rect.Height, rect.Width);
		}
		public static List<GRect2D> Calculate(ISimpleDiagram diagram, GRect2D bounds, int domainCount) {
			TransformRectangle transformRectangle = diagram.LayoutDirection == SimpleDiagramLayoutDirection.Vertical ? 
				(TransformRectangle)RotateRectangle : (TransformRectangle)PassRectangle;
			return new SimpleDiagramLayout(bounds, diagram.Dimension, domainCount, transformRectangle).domainBounds;
		}
		readonly GRect2D bounds;
		readonly TransformRectangle transformRectangle;
		readonly List<GRect2D> domainBounds;
		SimpleDiagramLayout(GRect2D bounds, int dimension, int domainCount, TransformRectangle transformRectangle)  {
			this.bounds = transformRectangle(bounds);
			this.transformRectangle = transformRectangle;
			domainBounds = new List<GRect2D>(domainCount);
			Calculate(Math.Min(dimension, domainCount), domainCount);
		}
		void FillRow(int count, int position, int height) {
			int width = bounds.Width / count;
			int widthCorrection = bounds.Width - width * count;
			for (int i = 0, x = bounds.Left; i < count; i++) {
				int correctedWidth = width;
				if (widthCorrection > 0) {
					correctedWidth++;
					widthCorrection--;
				}
				domainBounds.Add(transformRectangle(new GRect2D(x, position, correctedWidth, height)));
				x += correctedWidth;
			}				
		}
		void Calculate(int dimension, int domainCount) {
			int completedRowCount = domainCount / dimension;				
			int lastRowDomainCount = domainCount % dimension;
			int rowCount = completedRowCount;
			if (lastRowDomainCount > 0)
				rowCount++;
			int height = bounds.Height / rowCount;
			int heightCorrection = bounds.Height - height * rowCount;
			int y = bounds.Top;
			for (int i = 0; i < completedRowCount; i++) {
				int correctedHeight = height;
				if (heightCorrection > 0) {
					correctedHeight++;
					heightCorrection--;
				}
				FillRow(dimension, y, correctedHeight);
				y += correctedHeight;
			}
			if (lastRowDomainCount > 0)
				FillRow(lastRowDomainCount, y, bounds.Height - y + bounds.Top);
		}
	}
}
