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
using System.Collections;
using DevExpress.Utils;
namespace DevExpress.DocumentServices.ServiceModel {
	class DefaultValueParameter : IClientParameter {
		static string ExtractName(string path) {
			int lastDotIndex = path.LastIndexOf('.');
			if(lastDotIndex == -1)
				return path;
			lastDotIndex++;
			return path.Substring(lastDotIndex);
		}
		string description;
		bool isDescriptionChanged;
		object value;
		bool isValueChanged;
		bool visible;
		bool isVisibleChanged;
		public string Description {
			get { return description; }
			set {
				description = value;
				isDescriptionChanged = true;
			}
		}
		public string Name { get; private set; }
		public Type Type {
			get { return Value != null ? Value.GetType() : null; }
		}
		public object Value {
			get { return value; }
			set {
				this.value = value;
				isValueChanged = true;
			}
		}
		public bool MultiValue {
			get { return value is IEnumerable; }
		}
		public bool Visible {
			get { return visible; }
			set {
				visible = value;
				isVisibleChanged = true;
			}
		}
		internal string Path { get; private set; }
		public void CopyTo(IClientParameter parameter) {
			if(isDescriptionChanged)
				parameter.Description = Description;
			if(isValueChanged)
				parameter.Value = Value;
			if(isVisibleChanged)
				parameter.Visible = Visible;
		}
		public DefaultValueParameter(string path) {
			Guard.ArgumentNotNull(path, "path");
			Path = path;
			Name = ExtractName(path);
		}
		public void CopyFrom(IClientParameter parameter) {
			description = parameter.Description;
			isDescriptionChanged = false;
			isValueChanged = false;
			value = parameter.Value;
			isVisibleChanged = false;
			visible = parameter.Visible;
		}
	}
}
