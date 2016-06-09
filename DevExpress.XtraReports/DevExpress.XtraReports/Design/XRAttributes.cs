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
using System.ComponentModel;
using System.Globalization;
using System.ComponentModel.Design;
using DevExpress.XtraReports.Localization;
namespace DevExpress.XtraReports
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public sealed class ReportAssociatedComponentAttribute : Attribute {
		public override bool Equals(object obj) {
			return obj is ReportAssociatedComponentAttribute;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public sealed class RootClassAttribute : Attribute {
		public override bool Equals(object obj) {
			return obj is RootClassAttribute;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	[AttributeUsage(AttributeTargets.Class, Inherited = true)]
	public sealed class BandKindAttribute : Attribute {
		DevExpress.XtraReports.UI.BandKind bandKind;
		public DevExpress.XtraReports.UI.BandKind BandKind {
			get { return bandKind; }
		}
		public BandKindAttribute(DevExpress.XtraReports.UI.BandKind bandKind) {
			this.bandKind = bandKind;
		}
		public override bool Equals(object obj) {
			BandKindAttribute other = obj as BandKindAttribute;
			return (other != null) && other.bandKind == this.bandKind;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	[AttributeUsage(AttributeTargets.All)]
	public sealed class BandTypeAttribute : Attribute {
		Type bandType;
		public Type BandType {
			get { return bandType; }
		}
		public BandTypeAttribute(Type bandType) {
			if(!typeof(DevExpress.XtraReports.UI.Band).IsAssignableFrom(bandType))
				throw new ArgumentException("bandType");
			this.bandType = bandType;
		}
		public override bool Equals(object obj) {
			BandTypeAttribute other = obj as BandTypeAttribute;
			return (other != null) && other.bandType == this.bandType;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
}
namespace DevExpress.XtraReports.UserDesigner {
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class XRToolboxSubcategoryAttribute : Attribute {
		public int Subcategory {
			get;
			private set;
		}
		public int Position {
			get;
			private set;
		}
		public XRToolboxSubcategoryAttribute(int subcategory) : this(subcategory, 0) {
		}
		public XRToolboxSubcategoryAttribute(int subcategory, int position) {
			Subcategory = subcategory;
			Position = position;
		}
	}
	[Flags]
	public enum DesignDockPanelType { 
		FieldList = 1, 
		PropertyGrid = 2, 
		ReportExplorer = 4, 
		ToolBox = 8, 
		GroupAndSort = 16, 
		ErrorList = 32,
		All = FieldList | PropertyGrid | ReportExplorer | ToolBox | GroupAndSort | ErrorList
	}
}
