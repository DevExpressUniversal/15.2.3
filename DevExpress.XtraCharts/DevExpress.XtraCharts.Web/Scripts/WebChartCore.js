(function() {

var HyperlinkSource = ASPx.CreateClass(null, {
    constructor: function(hyperlink) {
        this.hyperlink = hyperlink;
    }
});
// ChartHitRectangle
function ChartHitRectangle(left, top, right, bottom) {
    this.left = left;
    this.top = top;
    this.right = right;
    this.bottom = bottom;
}
ChartHitRectangle.prototype = {
    IsVisible: function(x, y) {
        return (x >= this.left) && (x < this.right) && (y >= this.top) && (y < this.bottom);
    }
}
ChartHitRectangle.prototype.constructor = ChartHitRectangle;
// ~ChartHitRectangle

// ChartHitEmpty
function ChartHitEmpty() {
}
ChartHitEmpty.prototype = {
    IsVisible: function(x, y) {
        return false;
    }    
}
ChartHitEmpty.prototype.constructor = ChartHitEmpty;
// ~ChartHitEmpty

// ChartHitExpression
function ChartHitExpression(leftOperand, rightOperand) {
    if(ChartHitExpression.AsProrotype)
        return delete ChartHitExpression.AsProrotype;
    this.leftOperand = leftOperand;
    this.rightOperand = rightOperand;
}
ChartHitExpression.prototype = {
    IsVisible: function(x, y) {
        throw "ChartHitExpression abstract error";
    }
}
ChartHitExpression.prototype.constructor = ChartHitExpression;
// ~ChartHitExpression

// ChartHitUnion
function ChartHitUnion(leftOperand, rightOperand) {
    ChartHitExpression.call(this, leftOperand, rightOperand);
}
ChartHitExpression.AsPrototype = true;
ChartHitUnion.prototype = new ChartHitExpression();
ChartHitUnion.prototype.constructor = ChartHitUnion;
ChartHitUnion.prototype.IsVisible = function(x, y) {
    return this.leftOperand.IsVisible(x, y) || this.rightOperand.IsVisible(x, y);
}
// ~ChartHitUnion

// ChartHitIntersection
function ChartHitIntersection(leftOperand, rightOperand) {
    ChartHitExpression.call(this, leftOperand, rightOperand);
}
ChartHitExpression.AsPrototype = true;
ChartHitIntersection.prototype = new ChartHitExpression();
ChartHitIntersection.prototype.constructor = ChartHitIntersection;
ChartHitIntersection.prototype.IsVisible = function(x, y) {
    return this.leftOperand.IsVisible(x, y) && this.rightOperand.IsVisible(x, y);
}
// ~ChartHitIntersection

// ChartHitExclusion
function ChartHitExclusion(leftOperand, rightOperand) {
    ChartHitExpression.call(this, leftOperand, rightOperand);
}
ChartHitExpression.AsPrototype = true;
ChartHitExclusion.prototype = new ChartHitExpression();
ChartHitExclusion.prototype.constructor = ChartHitExclusion;
ChartHitExclusion.prototype.IsVisible = function(x, y) {
    return this.leftOperand.IsVisible(x, y) && !this.rightOperand.IsVisible(x, y);
}
// ~ChartHitExclusion

// ChartHitXor
function ChartHitXor(leftOperand, rightOperand) {
    ChartHitExpression.call(this, leftOperand, rightOperand);
}
ChartHitExpression.AsPrototype = true;
ChartHitXor.prototype = new ChartHitExpression();
ChartHitXor.prototype.constructor = ChartHitXor;
ChartHitXor.prototype.IsVisible = function(x, y) {
    return this.leftOperand.IsVisible(x, y) ^ this.rightOperand.IsVisible(x, y);
}
// ~ChartHitXor

// ChartHitPath
function ChartHitPath(lineSegments, alternate) {
    this.lineSegments = lineSegments;
    this.alternate = alternate;
}
ChartHitPath.prototype = {
    IsVisible: function(x, y) {
        if(this.lineSegments.length == 0)
            return false;
        var isVisible = this.IsVisibleInternal(x - 0.01, y);
        if(!isVisible)
            isVisible = this.IsVisibleInternal(x + 0.01, y);
        return isVisible;
    },
    IsVisibleInternal: function(x, y) {    
        var filteredLineSegments = this.FilterLineSegments(x, y);
        if(filteredLineSegments.length == 0)
            return false;
            
        var xIntersections = new ChartXIntersections(filteredLineSegments, x);
        var indicator = this.CalcIndicator(filteredLineSegments, xIntersections, y);
        return this.alternate ? (indicator % 2) != 0 : indicator != 0;
    },
    FilterLineSegments: function(x, y) {
        var filteredLineSegments = [];
        for(var i = 0; i < this.lineSegments.length; i++) {
            var lineSegment = this.lineSegments[i];
            if(this.ShouldFilterLineSegmentByX(lineSegment, x))
                continue;
            if(this.ShouldFilterLineSegmentByY(lineSegment, y))
                continue;
            filteredLineSegments.push(lineSegment);
        }
        return filteredLineSegments;
    },
    ShouldFilterLineSegmentByX: function(lineSegment, x) {
        return ((lineSegment.startX < x) && (lineSegment.finishX < x)) || 
            ((lineSegment.startX > x) && (lineSegment.finishX > x));
    },
    ShouldFilterLineSegmentByY: function(lineSegment, y) {
        return (lineSegment.startY < y) && (lineSegment.finishY < y);
    },
    CalcIndicator: function(lineSegments, xIntersections, y) {
        var indicator = 0;
        for(var i = 0; i < xIntersections.intersections.length; i++) {
            if(xIntersections.intersections[i] < y)
                continue;
            if(lineSegments[i].LeftToRight())
                indicator++;
            else if(lineSegments[i].RightToLeft())
                indicator--;
            else
                throw ChartLineSegment.err;
        }
        return indicator;
    }
}
ChartHitPath.prototype.constructor = ChartHitPath;
// ~ChartHitPath

// ChartLineSegment
function ChartLineSegment(startX, startY, finishX, finishY) {
    this.startX = startX;
    this.startY = startY;
    this.finishX = finishX;
    this.finishY = finishY;
}
ChartLineSegment.err = "Invalid line segment";
ChartLineSegment.prototype = {
    LeftToRight : function() {
        return this.startX < this.finishX;
    },
    RightToLeft : function() {
        return this.startX > this.finishX;
    }
}
ChartLineSegment.prototype.constructor = ChartLineSegment;
// ~ChartLineSegment

// ChartXIntersections
function ChartXIntersections(lineSegments, x) {
    this.intersections = new Array(lineSegments.length);
    for(var i = 0; i < lineSegments.length; i++)
        this.intersections[i] = this.CalcIntersection(lineSegments[i], x);
}
ChartXIntersections.prototype = {
    CalcIntersection: function(lineSegment, x) {
        var delta = lineSegment.finishX - lineSegment.startX;
        if(delta == 0)
            throw ChartLineSegment.err;
            
        var k = (lineSegment.finishY - lineSegment.startY) / delta;
        var b = (lineSegment.startY * lineSegment.finishX - lineSegment.finishY * lineSegment.startX) / delta;
        return k * x + b;
    }
}
ChartXIntersections.prototype.constructor = ChartXIntersections;
// ~ChartXIntersections

// ChartHitTestController
function ChartHitTestController(hitInfo) {
    this.objects = [];
    this.additionalObjects = [];
    
    var hitInfoLoader = new ChartHitInfoLoader(hitInfo);
    this.regions = hitInfoLoader.regions;
}
ChartHitTestController.prototype = {
    HitTest: function(x, y) {
        var hitRegions = this.CreateHitRegions(x, y);
        return this.CreateHitObjects(hitRegions, x, y);
    },
    CreateHitRegions: function(x, y) {
        var hitRegions = [];
        for(var i = 0; i < this.regions.length; i++)
            if(this.regions[i].IsVisible(x, y))
                hitRegions.push(this.regions[i]);
        return hitRegions;
    },
    CreateHitObjects: function(hitRegions, x, y) {
        var hitObjects = [];
        for(var i = 0; i < hitRegions.length; i++) {
            var hitRegion = hitRegions[i];
            hitObjects.push(this.CreateHitObject(hitRegion, x, y));
        }
        return hitObjects;
    },
    CreateHitObject: function(hitRegion, x, y) {
        var hitObject = this.objects[hitRegion.id];
        var additionalHitObject = 
            hitRegion.additionalId != -1 ? 
            this.additionalObjects[hitRegion.additionalId] : 
            null;
        if (ASPx.IsExists(hitRegion.toolTipPoint) && additionalHitObject != null)
            additionalHitObject.toolTipPoint = hitRegion.toolTipPoint;
        if (ASPx.IsExists(hitRegion.legendItemsHitRegions))
            additionalHitObject = this.CreateLegendItemHitObject(hitRegion.legendItemsHitRegions, x, y);
        if (ASPx.IsExists(hitRegion.hyperlink) && hitObject != null)
            additionalHitObject = new HyperlinkSource(hitRegion.hyperlink);
        return new ASPxClientHitObject(hitObject, additionalHitObject);
    },
    CreateLegendItemHitObject: function (legendItemsHitRegions, x, y) {
        for (i = 0; i < legendItemsHitRegions.length; i++) {
            if (legendItemsHitRegions[i].IsVisible(x, y))
                return new ASPxClientLegendCheckBox(legendItemsHitRegions[i].id);
        }
        return null;
    }
}
ChartHitTestController.prototype.constructor = ChartHitTestController;
// ~ChartHitTestController

// ChartHitRegion
function ChartHitRegion(primitive, id, additionalId, toolTipPoint, legendItemsHitRegions, hyperlink) {
    this.primitive = primitive;
    this.id = id;
    this.additionalId = additionalId;
    this.toolTipPoint = toolTipPoint;
    this.legendItemsHitRegions = legendItemsHitRegions;
    this.hyperlink = hyperlink;
}
ChartHitRegion.prototype = {
    IsVisible: function(x, y) {
        return this.primitive.IsVisible(x, y); 
    }
}
ChartHitRegion.prototype.constructor = ChartHitRegion;
// ~ChartHitRegion

// ChartHitInfoLoader
function ChartHitInfoLoader(hitInfo) {
    this.regions = [];
    this.LoadHitInfo(hitInfo);
}
ChartHitInfoLoader.errPrefix = "ChartHitTestController loading error: ";
ChartHitInfoLoader.errRegionType = ChartHitTestController.errPrefix + "Invalid hit region type";
ChartHitInfoLoader.errRegionFormat = ChartHitTestController.errPrefix + "Invalid hit region format";
ChartHitInfoLoader.prototype = {
    LoadHitInfo: function(hitInfo) {
        for(var i = 0; i < hitInfo.length; i++)
            this.LoadHitRegion(hitInfo[i]);
    },
    LoadHitRegion: function(interimHitRegion) {
        var id = interimHitRegion.hi;                
        var additionalId = ASPx.IsExists(interimHitRegion.hia) ? interimHitRegion.hia: -1;
        var primitive = this.LoadPrimitive(interimHitRegion.r);
        var toolTipPoint = interimHitRegion.ttp;
        var hyperlink = interimHitRegion.l;
        if (ASPx.IsExists(interimHitRegion.legChbRegns))
            var legendItemCheckboxRegions = this.LoadLegendItemsRegionsArray(interimHitRegion.legChbRegns);
        this.regions.unshift(new ChartHitRegion(primitive, id, additionalId, toolTipPoint, legendItemCheckboxRegions, hyperlink));
    },
    LoadLegendItemsRegionsArray: function (legendItemsInterimRegionsArray) {
        var result = [];
        for (i = 0 ; i < legendItemsInterimRegionsArray.length; i++) {
            var interimLegendItemObject = legendItemsInterimRegionsArray[i];
            var legendItemId = interimLegendItemObject.legItmId;
            var primitive = this.LoadPrimitive(interimLegendItemObject.r);
            var legendItemHitRegion = new ChartHitRegion(primitive, legendItemId, -1, null, null, null);
            result.push(legendItemHitRegion);
        }
        return result;
    },
    LoadPrimitive: function(interimPrimitive) {
        if(interimPrimitive.t == "R")
            return this.LoadRectangle(interimPrimitive);
        else if(interimPrimitive.t == "O")
            return new ChartHitEmpty();
        else if(interimPrimitive.t == "U")
            return this.LoadUnion(interimPrimitive);
        else if(interimPrimitive.t == "I")
            return this.LoadIntersection(interimPrimitive);
        else if(interimPrimitive.t == "E")
            return this.LoadExclusion(interimPrimitive);
        else if(interimPrimitive.t == "X")
            return this.LoadXor(interimPrimitive);
        else if(interimPrimitive.t == "P")
            return this.LoadPath(interimPrimitive);            
        throw ChartHitInfoLoader.errRegionType;
    },
    LoadRectangle: function(interimRectangle) {
        if(interimRectangle.r.length != 4)
            throw ChartHitInfoLoader.errRegionFormat;
        return new ChartHitRectangle(interimRectangle.r[0], interimRectangle.r[1], interimRectangle.r[2], interimRectangle.r[3]);
    },
    LoadUnion: function(interimExpression) {
        var leftOperand = this.LoadPrimitive(interimExpression.l);
        var rightOperand = this.LoadPrimitive(interimExpression.r);
        return new ChartHitUnion(leftOperand, rightOperand);
    },
    LoadIntersection: function(interimExpression) {
        var leftOperand = this.LoadPrimitive(interimExpression.l);
        var rightOperand = this.LoadPrimitive(interimExpression.r);
        return new ChartHitIntersection(leftOperand, rightOperand);
    },
    LoadExclusion: function(interimExpression) {
        var leftOperand = this.LoadPrimitive(interimExpression.l);
        var rightOperand = this.LoadPrimitive(interimExpression.r);
        return new ChartHitExclusion(leftOperand, rightOperand);
    },
    LoadXor: function(interimExpression) {
        var leftOperand = this.LoadPrimitive(interimExpression.l);
        var rightOperand = this.LoadPrimitive(interimExpression.r);
        return new ChartHitXor(leftOperand, rightOperand);
    },
    LoadPath: function(interimPath) {       
        var lineSegments = [];
        var points = [];
        var bezierPoints = [];
        var indexInStartArray = 0;
        var indexInBezierArray = 0;
        for(var pointIndex = 0; pointIndex < interimPath.p.length; pointIndex++) {
            if(this.IsStartPoint(interimPath, pointIndex, indexInStartArray)) {
                indexInStartArray++;
                this.UpdateLineSegments(lineSegments, points);
                points.length = 0;
            }
            else if(this.IsBezierPoint(interimPath, pointIndex, indexInBezierArray)) {
                indexInBezierArray++;
                if(bezierPoints.length == 0)
                    bezierPoints.push(interimPath.p[pointIndex - 1]);                    
                bezierPoints.push(interimPath.p[pointIndex]);
                if(bezierPoints.length == 4) {
                    this.CalcBezierApproximation(
                        bezierPoints[0][0], bezierPoints[0][1], 
                        bezierPoints[1][0], bezierPoints[1][1], 
                        bezierPoints[2][0], bezierPoints[2][1],
                        bezierPoints[3][0], bezierPoints[3][1], points);
                    bezierPoints.length = 0;
                }
                continue;
            }
            points.push(interimPath.p[pointIndex]);                
        }
        this.UpdateLineSegments(lineSegments, points);
        return new ChartHitPath(lineSegments, interimPath.a);
    },
    UpdateLineSegments: function(lineSegments, points) {
        if(points.length < 2)
            return;
                               
        for(var i = 0; i < points.length - 1; i++)
            lineSegments.push(new ChartLineSegment(
                points[i][0], 
                points[i][1], 
                points[i + 1][0], 
                points[i + 1][1]));
        lineSegments.push(new ChartLineSegment(
            points[points.length - 1][0], 
            points[points.length - 1][1], 
            points[0][0], 
            points[0][1]));        
    },
    CreatePoint: function(x, y) {
        return [x, y];
    },
    IsStartPoint: function(interimPath, pointIndex, indexInStartArray) {
        return (indexInStartArray < interimPath.s.length) && (interimPath.s[indexInStartArray] == pointIndex);
    },
    IsBezierPoint: function(interimPath, pointIndex, indexInBezierArray) {
        return (indexInBezierArray < interimPath.b.length) && (interimPath.b[indexInBezierArray] == pointIndex);
    },
    CalcBezierApproximation: function(x1, y1, x2, y2, x3, y3, x4, y4, points) {
        var dx1 = x2 - x1;
        var dy1 = y2 - y1;
        var dx2 = x3 - x2;
        var dy2 = y3 - y2;
        var dx3 = x4 - x3;
        var dy3 = y4 - y3;
                    
        var length = Math.sqrt(dx1 * dx1 + dy1 * dy1) + Math.sqrt(dx2 * dx2 + dy2 * dy2) + Math.sqrt(dx3 * dx3 + dy3 * dy3);
        var stepCount = Math.round(length * 0.25);
                   
        var step = 1 / (stepCount + 1);
        var step2 = step * step;
        var step3 = step * step * step;
                    
        var step13 = step * 3;
        var step23 = step2 * 3;
        var step26 = step2 * 6;
        var step36 = step3 * 6;
                    
        var tempX1 = x1 - x2 * 2 + x3;
        var tempY1 = y1 - y2 * 2 + y3;
        var tempX2 = (x2 - x3) * 3 - x1 + x4;
        var tempY2 = (y2 - y3) * 3 - y1 + y4;
                    
        var dx = (x2 - x1) * step13 + tempX1 * step23 + tempX2 * step3;
        var dy = (y2 - y1) * step13 + tempY1 * step23 + tempY2 * step3;
        var ddx = tempX1 * step26 + tempX2 * step36;
        var ddy = tempY1 * step26 + tempY2 * step36;
        var dddx = tempX2 * step36;
        var dddy = tempY2 * step36;
                
        var x = x1;
        var y = y1;
        for(var i = 0; i < stepCount; i++) {
            x += dx;
            y += dy;
                 
            points.push(this.CreatePoint(x, y));
                    
            dx += ddx;
            dy += ddy;
            ddx += dddx;
            ddy += dddy;
        }
        points.push(this.CreatePoint(x4, y4));
    }
}
ChartHitInfoLoader.prototype.constructor = ChartHitInfoLoader;
// ~ChartHitInfoLoader

ASPx.ChartHitRectangle = ChartHitRectangle;
ASPx.ChartHitEmpty = ChartHitEmpty;
ASPx.ChartHitExpression = ChartHitExpression;
ASPx.ChartHitUnion = ChartHitUnion;
ASPx.ChartHitIntersection = ChartHitIntersection;
ASPx.ChartHitExclusion = ChartHitExclusion;
ASPx.ChartHitXor = ChartHitXor;
ASPx.ChartHitPath = ChartHitPath;
ASPx.ChartLineSegment = ChartLineSegment;
ASPx.ChartXIntersections = ChartXIntersections;
ASPx.ChartHitTestController = ChartHitTestController;
ASPx.ChartHitRegion = ChartHitRegion;
ASPx.ChartHitInfoLoader = ChartHitInfoLoader;
ASPx.HyperlinkSource = HyperlinkSource;
})();