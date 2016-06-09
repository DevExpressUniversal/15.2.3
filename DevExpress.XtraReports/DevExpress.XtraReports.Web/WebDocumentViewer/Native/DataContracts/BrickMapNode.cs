#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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

using System.Collections.Generic;
using System.Runtime.Serialization;
namespace DevExpress.XtraReports.Web.WebDocumentViewer.Native.DataContracts {
	[DataContract]
	public class BrickMapNode {
		[DataMember(Name = "top")]
		public int Top { get; set; }
		[DataMember(Name = "left")]
		public int Left { get; set; }
		[DataMember(Name = "height")]
		public int Height { get; set; }
		[DataMember(Name = "width")]
		public int Width { get; set; }
		[DataMember(Name = "content")]
		public Dictionary<string, object> Content { get; set; }
		[DataMember(Name = "indexes")]
		public string Indexes { get; set; }
		[DataMember(Name = "genlIndex")]
		public int GeneralIndex { get; set; }
		[DataMember(Name = "col")]
		public int Col { get; set; }
		[DataMember(Name = "row")]
		public int Row { get; set; }
		[DataMember(Name = "navigation")]
		public BrickMapNodeNavigation Navigation { get; set; }
		[DataMember(Name = "bricks")]
		public BrickMapNode[] Bricks { get; set; }
	}
}
