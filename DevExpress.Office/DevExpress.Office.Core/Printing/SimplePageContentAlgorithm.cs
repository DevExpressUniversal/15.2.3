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
using System.Text;
using DevExpress.XtraPrinting.Native;
using System.Collections;
using System.Drawing;
using DevExpress.XtraPrinting;
namespace DevExpress.Office.Layout.Export {
	public class SimplePageContentAlgorithm : IPageContentAlgorithm {
		DocumentBand documentBand;
		IList addedBricks;
		RectangleF bounds = RectangleF.Empty;
		public bool Process(PSPage page, DocumentBand documentBand, RectangleF bounds, PointF offset) {
			if(this.documentBand == documentBand) {
				addedBricks = null;
				return false;
			}
			this.documentBand = documentBand;
			addedBricks = ArrayListExtentions.CreateInstance<Brick>(documentBand.Bricks);
			this.bounds = bounds;
			page.ClearContent();
			page.AddContent(documentBand.Bricks);
			page.NoClip = true;
			page.LockContent();
			return true;
		}
		public RectangleF Bounds {
			get { return bounds; }
		}
		public IList AddedPageBricks {
			get {
				if(addedBricks != null)
					return addedBricks;
				return new object[0]; 
			}
		}
	}
}
