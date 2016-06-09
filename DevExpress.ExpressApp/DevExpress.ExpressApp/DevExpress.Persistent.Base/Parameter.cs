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
namespace DevExpress.Persistent.Base {
	public class ParameterList : Dictionary<string, IParameter> {
		public void Add(IParameter credential) {
			base.Add(credential.Name, credential);
		}
	}
	public interface IParameter {
		object CurrentValue { get; set; }
		string Caption { get; set; }
		string Name { get; }
		bool Visible { get;	}
		Type Type {	get; }
		bool IsMasked { get; set; }
		bool IsRequired { get; set; }
		bool IsReadOnly { get; }
	}
	public abstract class ParameterBase : IParameter {
		private Type type;
		private bool isMasked;
		private bool isRequired;
		private string name;
		private string caption;
		private bool visible = true;
		public ParameterBase(string name, Type valueType) {
			if(valueType == null) {
				throw new ArgumentNullException("valueType");
			}
			this.name = name;
			this.type = valueType;
			this.visible = false;
		}
		public virtual IParameter Clone() {
			return (IParameter)this.MemberwiseClone();
		}
		public Type Type {
			get { return type; }
		}
		public string Name { get { return name; } }
		public bool IsMasked { get { return isMasked; }	set { isMasked = value; } }
		public bool IsRequired { get { return isRequired; } set { isRequired = value; } }
		public string Caption {
			get { return string.IsNullOrEmpty(caption) ? Name : caption; }
			set { caption = value; }
		}
		public bool Visible { get { return visible; } set { visible = value; } }
		public virtual bool IsReadOnly { get { return true; } }
		protected abstract object GetCurrentValue();
		protected virtual void SetCurrentValue(object value) { }
		#region
		object IParameter.CurrentValue { get { return GetCurrentValue(); } set { SetCurrentValue(value); } }
		#endregion
	}
	public abstract class ReadOnlyParameter : ParameterBase {
		public ReadOnlyParameter(string name, Type valueType)
			: base(name, valueType) {
			this.Visible = false;
		}
		public abstract object CurrentValue { get; }
		protected override object GetCurrentValue() { return CurrentValue; }
	}
}
