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
namespace DevExpress.XtraCharts.Native {
	public class Graphics3DCache {
		PrimitivesContainer conatiner;
		Node graphicsTree;
		PolygonGroups polygonGroups;
		public PrimitivesContainer Container { 
			get { return conatiner; } 
			set { conatiner = value; } 
		}
		public Node GraphicsTree { 
			get { return graphicsTree; }
			set { graphicsTree = value; } 
		}
		public PolygonGroups PolygonGroups { 
			get { return polygonGroups; } 
			set { polygonGroups = value; } 
		}
		public Graphics3DCache() {
		}
		public void Reset() {
			conatiner = null;
			graphicsTree = null;
			polygonGroups = null;
		}
		public GraphicsCommand CreateGraphicsCommand(XYDiagram3DCoordsCalculator calculator) {
			if (graphicsTree == null)
				return polygonGroups == null ? null : polygonGroups.CreateGraphicsCommand();
			GraphicsCommand maskDepthCommand = new MaskDepthBufferGraphicsCommand();
			DiagramPoint eye = calculator.UnProject(new DiagramPoint(calculator.Bounds.Left + (calculator.Bounds.Width / 2.0),
																	 calculator.Bounds.Bottom - (calculator.Bounds.Height / 2.0),
																	 -Int32.MaxValue));
			maskDepthCommand.AddChildCommand(graphicsTree.CreateGraphicsCommand(eye));
			return maskDepthCommand;
		}
	}
}
