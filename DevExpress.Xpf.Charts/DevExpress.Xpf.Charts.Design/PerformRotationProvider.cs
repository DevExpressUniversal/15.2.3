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

using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using DevExpress.Xpf.Charts.Native;
using System.Windows.Controls;
using DevExpress.Xpf.Design;
namespace DevExpress.Xpf.Charts.Design {
	public class PerformRotationProvider : DXAdornerProviderBase {
		Diagram3D Diagram { get { return platformObject as Diagram3D; } }
		Point lastPosition;
		bool AllowRotate {
			get { return Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl); }
		}
		Transform3D Transform {
			get { return Diagram == null ? null : Diagram.ActualContentTransform; }
			set {
				ModelProperty rotationMatrix = AdornedElement.Properties[Diagram3D.ContentTransformProperty.Name];
				rotationMatrix.SetValue(value);
			}
		}
		public PerformRotationProvider() : base() {
			hookPanel.MouseMove += new MouseEventHandler(hookPanel_MouseMove);
			hookPanel.MouseEnter += new MouseEventHandler(hookPanel_MouseEnter);
		}
		protected override Control CreateHookPanel() {
			return new HookMousePanel();
		}
		void hookPanel_MouseEnter(object sender, MouseEventArgs e) {
			lastPosition = e.GetPosition(hookPanel);
		}
		void hookPanel_MouseMove(object sender, MouseEventArgs e) {
			Point currentPosition = e.GetPosition(hookPanel);
			if (AllowRotate) {
				double factor = Math.PI * 0.25;
				double xAngle = (currentPosition.X - lastPosition.X) * factor;
				double yAngle = (currentPosition.Y - lastPosition.Y) * factor;
				PerformRotate(xAngle, yAngle);
			}
			lastPosition = currentPosition;
		}
		public void PerformRotate(double angleX, double angleY) {
			Transform3D transform = Transform;
			if (transform != null)
				using(ModelEditingScope batchedChange = AdornedElement.BeginEdit()) {
					Transform = new MatrixTransform3D(Diagram3DDomain.PerformRotation(transform.Value, angleX, angleY));
					batchedChange.Complete();
				}
		}
	}
}
