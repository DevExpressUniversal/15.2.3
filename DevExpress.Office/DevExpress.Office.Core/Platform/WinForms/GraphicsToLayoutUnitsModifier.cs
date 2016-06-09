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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Permissions;
using DevExpress.Utils;
using DevExpress.Utils.Text;
using DevExpress.Office.Layout;
using DevExpress.Data.Helpers;
namespace DevExpress.Office.Utils {
	#region GraphicsToLayoutUnitsModifier
	public class GraphicsToLayoutUnitsModifier : IDisposable {
		readonly Graphics graphics;
		readonly DocumentLayoutUnitConverter unitConverter;
		GraphicsUnit oldUnit;
		float oldScale;
		HdcDpiModifier hdcDpiModifier;
		Matrix oldMatrix;
		public GraphicsToLayoutUnitsModifier(Graphics graphics, DocumentLayoutUnitConverter unitConverter) {
			Guard.ArgumentNotNull(graphics, "graphics");
			Guard.ArgumentNotNull(unitConverter, "unitConverter");
			this.graphics = graphics;
			this.unitConverter = unitConverter;
			Apply();
		}
		public void Dispose() {
			Restore();
		}
		protected void Apply() {
			this.oldUnit = graphics.PageUnit;
			this.oldScale = graphics.PageScale;
			this.oldMatrix = graphics.Transform;
			graphics.PageUnit = (GraphicsUnit)unitConverter.GraphicsPageUnit;
			graphics.PageScale = unitConverter.GraphicsPageScale;
			graphics.ResetTransform();
			if (SecurityHelper.IsUnmanagedCodeGrantedAndCanUseGetHdc)
				this.hdcDpiModifier = new HdcDpiModifier(graphics, new Size(4096, 4096), (int)Math.Round(unitConverter.Dpi));
		}
		protected void Restore() {
			try {
				if (this.hdcDpiModifier != null && SecurityHelper.IsUnmanagedCodeGrantedAndCanUseGetHdc)
					this.hdcDpiModifier.Dispose();
				graphics.PageUnit = oldUnit;
				graphics.PageScale = oldScale;
				graphics.ResetTransform();
				graphics.Transform = oldMatrix;
			}
			catch {
			}
		}
	}
	#endregion
}
