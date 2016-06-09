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
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Gauges.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Gauges {
	[NonCategorized]
	public class ScaleElementsInfo : ElementInfoBase  {
		readonly Scale scale;
		ObservableCollection<object> elements;
		protected internal override Object HitTestableObject { get { return null; } }
		protected internal override Object HitTestableParent { get { return scale; } }
		protected internal override bool IsHitTestVisible { get { return scale.IsHitTestVisible; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public ObservableCollection<object> Elements {
			get { return elements; }
			private set {
				if (elements != value) {
					elements = value;
					NotifyPropertyChanged("Elements");
				}
			}
		}
		internal ScaleElementsInfo(Scale scale, int zIndex) : base(scale, zIndex, new ScaleElementsPresentationControl(), null) {
			this.scale = scale;
			Elements = new ObservableCollection<object>();
		}
		protected internal override void Invalidate() {
			base.Invalidate();
			ScaleElementsPresentationControl presentation = PresentationControl as ScaleElementsPresentationControl;
			if (presentation != null && presentation.Panel != null)
				presentation.Panel.InvalidateMeasure();
		}
	}
	public class ScaleElementsPresentationControl : PresentationControl {
		internal Panel Panel { get { return CommonUtils.GetChildPanel(GetTemplateChild("PART_Elements") as ItemsControl); } }
		public ScaleElementsPresentationControl() {
			DefaultStyleKey = typeof(ScaleElementsPresentationControl);
		}
	}
}
