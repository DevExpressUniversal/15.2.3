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
using System.ComponentModel.Design;
namespace DevExpress.XtraReports {
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
	public sealed class XRDesignerAttribute : Attribute {
		private string designerTypeName;
		private Type designerBaseType;
		private string typeId;
		public XRDesignerAttribute(string designerTypeName) {
			this.designerTypeName = designerTypeName;
			this.designerBaseType = typeof(IDesigner);
		}
		public XRDesignerAttribute(string designerTypeName, Type designerBaseType) {
			this.designerTypeName = designerTypeName;
			this.designerBaseType = designerBaseType;
		}
		public string DesignerBaseTypeName {
			get { return designerBaseType.FullName; }
		}
		public string DesignerTypeName {
			get { return designerTypeName; }
		}
		public override object TypeId {
			get {
				if (typeId == null) {
					string baseType = designerBaseType.Name;
					int comma = baseType.IndexOf(',');
					if (comma != -1) {
						baseType = baseType.Substring(0, comma);
					}
					typeId = GetType().FullName + baseType;
				}
				return typeId;
			}
		}
		public override bool Equals(object obj) {
			if (obj == this) return true;
			XRDesignerAttribute other = obj as XRDesignerAttribute;
			return (other != null) && other.designerBaseType.Name == designerBaseType.Name && other.designerTypeName == designerTypeName;
		}
		public override int GetHashCode() {
			return designerTypeName.GetHashCode() ^ designerBaseType.Name.GetHashCode();
		}
	}
}
