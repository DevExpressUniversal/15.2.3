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
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.Office {
	#region OpenXmlRelation
	public class OpenXmlRelation {
		#region Fields
		string id;
		string target;
		string type;
		string targetMode;
		#endregion
		public OpenXmlRelation() {
		}
		public OpenXmlRelation(string id, string target, string type)
			: this(id, target, type, null) {
		}
		public OpenXmlRelation(string id, string target, string type, string targetMode) {
			this.id = id;
			this.target = target;
			this.type = type;
			this.targetMode = targetMode;
		}
		#region Properties
		public string Id { get { return id; } set { id = value; } }
		public string Target { get { return target; } set { target = value; } }
		public string Type { get { return type; } set { type = value; } }
		public string TargetMode { get { return targetMode; } set { targetMode = value; } }
		#endregion
	}
	#endregion
	#region OpenXmlRelationCollection
	public class OpenXmlRelationCollection : List<OpenXmlRelation> {
		public OpenXmlRelation LookupRelationById(string id) {
			if (String.IsNullOrEmpty(id))
				return null;
			int count = Count;
			for (int i = 0; i < count; i++)
				if (this[i].Id == id)
					return this[i];
			return null;
		}
		public OpenXmlRelation LookupRelationByType(string type) {
			if (String.IsNullOrEmpty(type))
				return null;
			int count = Count;
			for (int i = 0; i < count; i++)
				if (this[i].Type == type)
					return this[i];
			return null;
		}
		public string GenerateId() {
			return String.Format("rId{0}", Count + 1);
		}
		internal OpenXmlRelation LookupRelationByTargetAndType(string target, string type) {
			if(String.IsNullOrEmpty(target) || String.IsNullOrEmpty(type))
				return null;
			int count = Count;
			for(int i = 0; i < count; i++)
				if(this[i].Target == target && this[i].Type == type)
					return this[i];
			return null;
		}
	}
	#endregion
}
