#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.Utils;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardCommon {
	public abstract class DataItemDefinition {
		const string xmlDataMember = "DataMember";
		public string DataMember { get; private set; }
		protected DataItemDefinition(string dataMember) {
			DataMember = dataMember;
		}
		public override bool Equals(object obj) {
			DataItemDefinition definition = obj as DataItemDefinition;
			return definition != null && definition.DataMember == DataMember;
		}
		public override int GetHashCode() {
			return DataMember != null ? DataMember.GetHashCode() : 0;
		}
		public override string ToString() {
			return DataMember;
		}
		protected internal virtual void SaveToXml(XElement element) {
			if (!string.IsNullOrEmpty(DataMember))
				element.Add(new XAttribute(xmlDataMember, DataMember));
		}
		protected internal virtual void LoadFromXml(XElement element) {
			DataMember = element.GetAttributeValue(xmlDataMember);
		}
	}
}
