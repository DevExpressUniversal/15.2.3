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
namespace DevExpress.Xpf.Editors.Internal {
	public abstract class SparklinePropertyDescriptorBase : PropertyDescriptor {
		public static readonly object UnsetValue = new object();
		public static bool IsUnsetValue(object value) {
			return UnsetValue == value;
		}
		public static SparklinePropertyDescriptorBase CreatePropertyDescriptor(Type componentType, string path, string internalPath = null) {
			return new SparklinePropertyDescriptor(path, internalPath);
		}
		protected string Path { get; private set; }
		protected string InternalPath { get; private set; }
		public override string DisplayName { get { return Path; } }
		public override System.Type ComponentType { get { return typeof(object); } }
		public override string Name { get { return Path; } }
		public override bool IsReadOnly { get { return false; } }
		public override System.Type PropertyType { get { return typeof(object); } }
		protected SparklinePropertyDescriptorBase(string path, string internalPath)
			: base(path, null) {
			Path = path;
			InternalPath = internalPath;
		}
		protected abstract object GetValueImpl(object component);
		public override bool CanResetValue(object component) {
			return false;
		}
		public override object GetValue(object component) {
			return GetValueImpl(component);
		}
		public override void ResetValue(object component) {
		}
		public override void SetValue(object component, object value) {
		}
		public override bool ShouldSerializeValue(object component) {
			return false;
		}
		public bool IsRelevant(string internalPath) {
			if (internalPath == null || InternalPath == null)
				return false;
			return internalPath == InternalPath;
		}
		public virtual void Reset() {
		}
	}
}
