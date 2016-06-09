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
using System.ComponentModel;
using DevExpress.Utils;
using DevExpress.XtraReports.Parameters;
namespace DevExpress.DocumentServices.ServiceModel {
	class ClientParameter : IClientParameter {
		internal event EventHandler ValueChanged;
		readonly Parameter parameter;
		readonly string path;
		public string Description {
			get { return parameter.Description; }
			set { parameter.Description = value; }
		}
		public string Name {
			get { return parameter.Name; }
		}
		public Type Type {
			get { return parameter.Type; }
		}
		public object Value {
			get { return parameter.Value; }
			set {
				if(value == null && Type.IsValueType)
					throw new ArgumentException("Cannot set null value for a value type parameter.");
				if(value != null && !MultiValue && !Type.IsInstanceOfType(value))
					throw new ArgumentException("Cannot set value of unrelated type.");
				if(value != null && MultiValue && !(value is IEnumerable))
					throw new ArgumentException("Value must be an enumerable.");
				parameter.Value = value;
				if(ValueChanged != null)
					ValueChanged(this, EventArgs.Empty);
			}
		}
		public bool MultiValue {
			get { return parameter.MultiValue; }
		}
		public bool Visible {
			get { return parameter.Visible; }
			set { parameter.Visible = value; }
		}
		internal Parameter OriginalParameter {
			get { return parameter; }
		}
		internal string Path {
			get { return path; }
		}
		[DefaultValue(false)]
		internal bool IsFilteredLookUpSettings {get; set;}
		internal ClientParameter(Parameter parameter, string path) {
			Guard.ArgumentNotNull(parameter, "parameter");
			this.parameter = parameter;
			this.path = path;
		}
		internal ClientParameter(Parameter parameter)
			: this(parameter, string.Empty) {
		}
	}
}
