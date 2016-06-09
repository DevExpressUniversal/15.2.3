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

using DevExpress.Mvvm.Native;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
namespace DevExpress.Xpf.Core.Native {
	public class RenderConditionCollection : ObservableCollection<RenderPropertyBase> { }	
	[Browsable(false)]
	public abstract class RenderPropertyBase {
		public abstract RenderPropertyContextBase CreateContext();
	}
	[Browsable(false)]
	public class RenderPropertyChangedListener : RenderPropertyBase {
		string property;
		public string Property { get { return property ?? DependencyProperty.With(x => x.Name); } set { property = value; } }
		[IgnoreDependencyPropertiesConsistencyChecker]
		private DependencyProperty dependencyProperty;
		public DependencyProperty DependencyProperty {
			get { return dependencyProperty; }
			set { dependencyProperty = value; }
		}
		public RenderValueSource ValueSource { get; set; }
		public string TargetName { get; set; }
		public string SourceName { get; set; }
		public RenderPropertyChangedListener() {
			ValueSource = RenderValueSource.DataContext;
		}
		public override RenderPropertyContextBase CreateContext() {
			return new RenderPropertyChangedListenerContext(this);
		}
	}
	public enum RenderValueSource { DataContext, ElementName, TemplatedParent }
	[Browsable(true)]
	public class RenderCondition : RenderPropertyChangedListener {
		public object Value { get; set; }		
		public bool FallbackIsValid { get; set; }
		public RenderCondition() {
			FallbackIsValid = false;
		}
		public override RenderPropertyContextBase CreateContext() {
			return new RenderConditionContext(this);
		}		
	}
	public enum RenderConditionGroupOperator { And, Or }
	[Browsable(true)]
	[ContentProperty("Conditions")]
	public class RenderConditionGroup : RenderPropertyBase {
		readonly RenderConditionCollection conditions;
		public RenderConditionCollection Conditions { get { return conditions; } }
		public RenderConditionGroupOperator Operator { get; set; }
		public RenderConditionGroup() {
			Operator = RenderConditionGroupOperator.And;
			conditions = new RenderConditionCollection();
		}
		public override RenderPropertyContextBase CreateContext() {
			return new RenderConditionGroupContext(this);
		}
	}
}
