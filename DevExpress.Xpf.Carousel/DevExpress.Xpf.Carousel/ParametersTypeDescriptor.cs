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
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Utils;
namespace DevExpress.Xpf.Carousel {
	public class ParametersTypeDescriptor : CustomTypeDescriptorBase, INotifyPropertyChanged {
		double position;
		ItemTransfer transfer;
		public ParametersTypeDescriptor(CarouselPanel owner) {
			this.Owner = owner;
		}
		internal double Position {
			get { return position; }
			set {
				if(position == value)
					return;
				position = value;
			}
		}
		FunctionBase DistortionCompensationFunction { get; set; }
		internal CarouselPanel Owner { get; private set; }
		public double ActualPosition {
			get {
				double result = 0;
				if(Owner.AnimationProgress < Owner.SmoothingTimeFraction && DistortionCompensationFunction != null)
					result = DistortionCompensationFunction.GetValue(Position);
				else
					result = Owner.GetDistortedPosition(Position, Transfer);
				return Owner.OffsetDistributionWrapper.GetValue(result);
			}
		}
		internal ItemTransfer Transfer {
			get { return transfer; }
			set {
				double y1 = 0;
				double x1 = Position;
				bool isActualTransfer = false;
				if(Transfer != null)
					isActualTransfer = Transfer.IsActual;
				if(isActualTransfer)
					y1 = Owner.GetDistortedPosition(Position, Transfer); ;
				DistortionCompensationFunction = null;
				transfer = value;
				double startPositionOfNewTransform = Owner.GetDistortedPosition(Transfer.GetPosition(0), Transfer);
				if(isActualTransfer && y1 != startPositionOfNewTransform) {
					double y2 = Transfer.GetPosition(Owner.SmoothingTimeFraction);
					double d2 = Owner.GetDistortedPosition(y2, Transfer, Owner.SmoothingTimeFraction);
					double k = (y1 - d2) / (x1 - y2);
					double b = y1 - k * x1;
					DistortionCompensationFunction = new LinearFunction(new Point(x1, y1), new Point(y2, d2));
				}
			}
		}
		internal void NotifyParametersChanged(List<String> skiplist) {
			foreach(PropertyDescriptor descriptor in TypeDescriptor.GetProperties(this)) {
				if(skiplist != null && skiplist.Contains(descriptor.Name)) continue;
				RaisePropertyChanged(descriptor.Name);
			}
		}
		void RaisePropertyChanged(string name) {
			if(PropertyChangedEventStorage != null)
				PropertyChangedEventStorage(this, new PropertyChangedEventArgs(name)); 
		}
		#region ICustomTypeDescriptor Members
		public override PropertyDescriptorCollection GetProperties(Attribute[] attributes) {
			List<PropertyDescriptor> descriptors = new List<PropertyDescriptor>();
			foreach(Parameter param in Owner.ParameterSetWrapper)
				descriptors.Add(new ParameterDescriptor(param));
			descriptors.Add(OffsetParameterDescriptor.OffsetXDescriptor);
			descriptors.Add(OffsetParameterDescriptor.OffsetYDescriptor);
			return new PropertyDescriptorCollection(descriptors.ToArray());
		}
		#endregion
		#region INotifyPropertyChanged Members
		PropertyChangedEventHandler PropertyChangedEventStorage = null;
		public event PropertyChangedEventHandler PropertyChanged {
			add {
				PropertyChangedEventStorage += value;
			}
			remove {
				PropertyChangedEventStorage -= value;
			}
		}
		#endregion
	}
}
