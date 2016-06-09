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

using System.Runtime.Serialization;
namespace DevExpress.XtraReports.Web.ReportDesigner.Native.DataContracts {
	[DataContract]
	public class XRShapeLayout {
		[DataMember(Name = "shapeType")]
		public string ShapeType { get; set; }
		[DataMember(Name = "width")]
		public float Width { get; set; }
		[DataMember(Name = "height")]
		public float Height { get; set; }
		[DataMember(Name = "fillet")]
		public int Fillet { get; set; }
		[DataMember(Name = "numberOfSides")]
		public int NumberOfSides { get; set; }
		[DataMember(Name = "angle")]
		public int Angle { get; set; }
		[DataMember(Name = "arrowHeight")]
		public int ArrowHeight { get; set; }
		[DataMember(Name = "arrowWidth")]
		public int ArrowWidth { get; set; }
		[DataMember(Name = "concavity")]
		public int Concavity { get; set; }
		[DataMember(Name = "starPointCount")]
		public int StarPointCount { get; set; }
		[DataMember(Name = "horizontalLineWidth")]
		public int HorizontalLineWidth { get; set; }
		[DataMember(Name = "verticalLineWidth")]
		public int VerticalLineWidth { get; set; }
		[DataMember(Name = "tipLength")]
		public int TipLength { get; set; }
		[DataMember(Name = "tailLength")]
		public int TailLength { get; set; }
		[DataMember(Name = "fillColor")]
		public string FillColor { get; set; }
		[DataMember(Name = "lineWidth")]
		public float LineWidth { get; set; }
		[DataMember(Name = "foreColor")]
		public string ForeColor { get; set; }
		[DataMember(Name = "stretch")]
		public bool Stretch { get; set; }
		[DataMember(Name = "dpi")]
		public float Dpi { get; set; }
	}
}
