#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
namespace DevExpress.ExpressApp.DC.ClassGeneration {
	sealed class PropertyMetadata : MetadataWithAttributes {
		private string name;
		private Type propertyType;
		private bool isReadOnly;
		private bool isLogicRequired;
		private InterfaceMetadata owner;
		private AssociationMetadata associationInfo;
		private bool isPersistent;
		internal string Name {
			get { return name; }
			set { name = value; }
		}
		internal Type PropertyType {
			get { return propertyType; }
			set { propertyType = value; }
		}
		internal bool IsReadOnly {
			get { return isReadOnly; }
			set { isReadOnly = value; }
		}
		internal bool IsLogicRequired {
			get { return isLogicRequired; }
			set { isLogicRequired = value; }
		}
		internal InterfaceMetadata Owner {
			get { return owner; }
			set { owner = value; }
		}
		internal AssociationMetadata AssociationInfo {
			get { return associationInfo; }
			set { associationInfo = value; }
		}
		internal bool IsPersistent {
			get { return isPersistent; }
			set { isPersistent = value; }
		}
		public override string ToString() {
			return string.Format("PropertyMetadata: {0}", Name);
		}
	}
	abstract class MetadataWithAttributes {
		private readonly IList<Attribute> attributes;
		internal MetadataWithAttributes() {
			attributes = new List<Attribute>();
		}
		internal IList<Attribute> Attributes {
			get { return attributes; }
		}
		internal AttributeType FindAttribute<AttributeType>() where AttributeType : Attribute {
			foreach(Attribute attribute in attributes) {
				if(attribute is AttributeType) {
					return (AttributeType)attribute;
				}
			}
			return null;
		}
		internal AttributeType[] FindAttributes<AttributeType>() where AttributeType : Attribute {
			List<AttributeType> result = new List<AttributeType>();
			foreach(Attribute attribute in attributes) {
				if(attribute is AttributeType) {
					result.Add((AttributeType)attribute);
				}
			}
			return result.ToArray();
		}
	}
}
