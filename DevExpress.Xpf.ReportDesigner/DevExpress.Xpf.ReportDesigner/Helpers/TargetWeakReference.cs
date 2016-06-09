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
namespace DevExpress.Xpf.Reports.UserDesigner.Native {
	public class TargetWeakReference {
		WeakReference reference;
		int hashCode;
		public TargetWeakReference(object target) : this(target, false) { }
		public TargetWeakReference(object target, bool trackResurrection) {
			hashCode = base.GetHashCode();
			reference = new WeakReference(target, trackResurrection);
		}
		public bool IsAlive { get { return reference.IsAlive; } }
		public object Target { get { return reference.Target; } }
		public bool TrackResurrection { get { return reference.TrackResurrection; } }
		public override string ToString() { return reference.ToString(); }
		#region Equality
		public override int GetHashCode() {
			object target = reference.Target;
			return target == null ? hashCode : (hashCode = target.GetHashCode());
		}
		public static bool operator ==(TargetWeakReference ref1, TargetWeakReference ref2) {
			bool ref1IsNull = (object)ref1 == null;
			bool ref2IsNull = (object)ref2 == null;
			if(ref1IsNull && ref2IsNull) return true;
			if(ref1IsNull || ref2IsNull) return false;
			object target1 = ref1.Target;
			object target2 = ref2.Target;
			return target1 == null && target2 == null ? ReferenceEquals(ref1, ref2) : Equals(target1, target2);
		}
		public override bool Equals(object obj) { return this == obj as TargetWeakReference; }
		public static bool operator !=(TargetWeakReference ref1, TargetWeakReference ref2) { return !(ref1 == ref2); }
		#endregion
	}
}
