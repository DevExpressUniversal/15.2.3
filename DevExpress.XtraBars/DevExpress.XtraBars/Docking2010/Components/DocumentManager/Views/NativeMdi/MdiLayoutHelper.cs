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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Mdi;
using DevExpress.Utils.Serializing;
namespace DevExpress.XtraBars.Docking2010.Views.NativeMdi {
	public static class MdiLayoutHelper {
		public static int CascadeLength = 25;
		public static Rectangle[] TileLayout(int elementCount, int columnCount, Rectangle bounds) {
			Rectangle[] result = new Rectangle[elementCount];
			int remainedWidth = bounds.Width;
			int remainedHeight = bounds.Height;
			Point offset = bounds.Location;
			Size size = Size.Empty;
			int patchedDocumentCount = 0;
			for(int i = columnCount; i > 0; i--) {
				size.Width = remainedWidth / i;
				remainedWidth -= size.Width;
				int rowCount = (elementCount - patchedDocumentCount) / i;
				remainedHeight = bounds.Height;
				for(int j = patchedDocumentCount; j < rowCount + patchedDocumentCount; j++) {
					size.Height = remainedHeight / (rowCount + patchedDocumentCount - j);
					remainedHeight -= size.Height;
					result[j] = new Rectangle(offset, size);
					offset.Y += size.Height;
				}
				patchedDocumentCount += rowCount;
				offset.X += size.Width;
				offset.Y = bounds.Y;
			}
			return result;
		}
		public static Rectangle[] CascadeLayout(int elementCount, Rectangle bounds) {
			Rectangle[] result = new Rectangle[elementCount];
			int cascadeCount = 0;
			if(bounds.Height < bounds.Width)
				cascadeCount = (int)((((double)bounds.Height) * 0.01) + 0.5);
			else
				cascadeCount = (int)((((double)bounds.Width) * 0.01) + 0.5);
			int height = bounds.Height - cascadeCount * CascadeLength;
			int width = bounds.Width - cascadeCount * CascadeLength;
			Size size = new Size(width, height);
			Point offset = bounds.Location;
			for(int i = 0; i < elementCount; i++) {
				result[i] = new Rectangle(offset, size);
				offset.Offset(CascadeLength, CascadeLength);
				if(((i + 1) % (cascadeCount + 1)) == 0)
					offset = bounds.Location;
			}
			return result;
		}
	}
}
