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
namespace DevExpress.PivotGrid.OLAP {
	static class OLAPSchemaGuid {
		public static readonly Guid Catalogs = new Guid("C8B52211-5CF3-11CE-ADE5-00AA0044773D");
		public static readonly Guid Cubes = new Guid("C8B522D8-5CF3-11CE-ADE5-00AA0044773D");
		public static readonly Guid Dimensions = new Guid("C8B522D9-5CF3-11CE-ADE5-00AA0044773D");
		public static readonly Guid Hierarchies = new Guid("C8B522DA-5CF3-11CE-ADE5-00AA0044773D");
		public static readonly Guid Measures = new Guid("C8B522DC-5CF3-11CE-ADE5-00AA0044773D");
		public static readonly Guid MeasureGroups = new Guid("E1625EBF-FA96-42fd-BEA6-DB90ADAFD96B");
		public static readonly Guid KPIs = new Guid("2AE44109-ED3D-4842-B16F-B694D1CB0E3F");
		public static readonly Guid Members = new Guid("C8B522DE-5CF3-11CE-ADE5-00AA0044773D");
		public static readonly Guid Levels = new Guid("C8B522DB-5CF3-11CE-ADE5-00AA0044773D");
		public static readonly Guid Properties = new Guid("C8B522DD-5CF3-11CE-ADE5-00AA0044773D");
	}
}
