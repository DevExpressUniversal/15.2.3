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
using System.ComponentModel;
namespace DevExpress.ExpressApp.Editors {
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
	public sealed class PropertyEditorAttribute : Attribute {
		private Type propertyType;
		internal Boolean isDefaultAlias;
		internal String alias;
		bool isDefaultEditor = true;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public PropertyEditorAttribute(Type propertyType, String alias, Boolean isDefaultEditor) {
			this.propertyType = propertyType;
			this.alias = alias;
			this.isDefaultEditor = isDefaultEditor;
			this.isDefaultAlias = true;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public PropertyEditorAttribute(Type propertyType, String alias)
			: this(propertyType, alias, true) {
		}
		public PropertyEditorAttribute(Type propertyType, bool defaultEditor)
			: this(propertyType, propertyType.Name, true) {
			isDefaultEditor = defaultEditor;
		}
		[Obsolete("Use 'PropertyEditorAttribute(Type propertyType, bool defaultEditor)' instead.", true)]
		public PropertyEditorAttribute(Type propertyType) : this(propertyType, true) { throw new NotImplementedException("Use 'PropertyEditorAttribute(Type propertyType, bool defaultEditor)' instead."); }
		[Obsolete("Use 'PropertyEditorAttribute(Type propertyType, bool defaultEditor)' instead.", true)]
		public PropertyEditorAttribute() : this(typeof(Object)) { throw new NotImplementedException("Use 'PropertyEditorAttribute(Type propertyType, bool defaultEditor)' instead."); }
#if !SL
	[DevExpressExpressAppLocalizedDescription("PropertyEditorAttributePropertyType")]
#endif
		public Type PropertyType {
			get { return propertyType; }
		}
		public bool IsDefaultEditor {
			get { return isDefaultEditor; }
		}
	}
}
