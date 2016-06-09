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
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	public class VmlDrawing {
		#region Fields
		readonly VmlShapeCollection shapes;
		VmlShapeLayout shapeLayout;
		VmlShapeTypeCollection shapeTypes;
		#endregion
		public VmlDrawing() {
			shapes = new VmlShapeCollection();
			shapeLayout = new VmlShapeLayout();
			shapeTypes = new VmlShapeTypeCollection();
		}
		#region Properties
		public VmlShapeLayout ShapeLayout { get { return shapeLayout; } set { shapeLayout = value; } }
		public VmlShapeTypeCollection ShapeTypes { get { return shapeTypes; } set { shapeTypes = value; } }
		public VmlShapeCollection Shapes { get { return shapes; } }
		#endregion
	}
	public class VmlShapeTypeCollection : SimpleCollection<VmlShapeType> {
		public VmlShapeTypeCollection()
			: base() {
				Add(new VmlShapeType());
		}
		#region Properties
		public VmlShapeType DefaultItem { get { return this[0]; } }
		#endregion
		public VmlShapeType GetByID(string id) {
			foreach (VmlShapeType type in this)
				if (string.Compare(type.Id, id) == 0)
					return type;
			return null;
		}
	}
}
