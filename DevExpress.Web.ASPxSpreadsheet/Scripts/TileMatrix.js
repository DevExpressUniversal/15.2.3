(function() {
    ASPxClientSpreadsheet.TileMatrix = ASPx.CreateClass(null, {
        constructor: function() {
            this.clearCache();
        },

        clearCache: function() {
            this.gridMatrix = {};
            this.colHeaders = {};
            this.rowHeaders = {};
        },

        getHeaderInfo: function(index, isCol) {
            var hash = isCol ? this.colHeaders : this.rowHeaders;
            return hash[index];
        },
        getTileSizes: function(index, isCol) {
            var headerInfo = this.getHeaderInfo(index, isCol);
            if(!headerInfo) return null;
            return headerInfo[isCol ? "widths" : "heights"];
        },
        getTileTotalSize: function(index, isCol) {
            var headerInfo = this.getHeaderInfo(index, isCol);
            if(!headerInfo) return null;
            return headerInfo[isCol ? "totalWidth" : "totalHeight"];
        },
        getTileIncrementalRanges: function(index, isCol) {
            var headerInfo = this.getHeaderInfo(index, isCol);
            if(!headerInfo) return null;
            return headerInfo[isCol ? "incrementalWidths" : "incrementalHeights"];
        },
        getHeaderInfoArray: function(isCol) {
            var result = [];
            var hash = isCol ? this.colHeaders : this.rowHeaders;
            for(var index in hash)
                result.push(hash[index]);
            return result;
        },
        getModelIndecesBoundaryPositions: function(isCol) {            
            var hash = isCol ? this.colHeaders : this.rowHeaders,
                boundaryIndices = [];
            for(var index in hash) 
                if(hash.hasOwnProperty(index) && hash[index]) {
                    var headerInfo = hash[index];
                    if(headerInfo.modelIndices && headerInfo.modelIndices.length > 0) {
                        boundaryIndices.push(headerInfo.modelIndices[0]);
                        boundaryIndices.push(headerInfo.modelIndices[headerInfo.modelIndices.length - 1]);
                    }
                }
            if(boundaryIndices.length > 0)
                boundaryIndices.shift();
            if(boundaryIndices.length > 0)
                boundaryIndices.pop();

            return boundaryIndices;
        },
        findHeaderInfo: function(isCol, delegate) {
            var hash = isCol ? this.colHeaders : this.rowHeaders;
            for(var index in hash) {
                var found = delegate(hash[index]);
                if(found) break;
            }
        },
        insertHeaderInfo: function(info, isCol) {
            if(!info) return;
            var hash = isCol ? this.colHeaders : this.rowHeaders;
            hash[info.index] = info;
        },
        removeHeaderInfo: function(index, isCol) {
            var hash = isCol ? this.colHeaders : this.rowHeaders;
            delete hash[index];
        },
        getGridTileInfo: function(rowIndex, colIndex) {
            var row = this.gridMatrix[rowIndex];
            return row && row[colIndex];
        },
        getGridTileInfoArray: function() {
            var result = [];
            for(var rowIndex in this.gridMatrix) {
                var row = this.gridMatrix[rowIndex];
                for(var colIndex in row)
                    result.push(row[colIndex]);
            }
            return result;
        },
        insertGridTileInfo: function(info) {
            if(!info) return;
            var row = this.gridMatrix[info.rowIndex] = this.gridMatrix[info.rowIndex] || {};
            row[info.colIndex] = info;
        },
        removeGridTileInfo: function(rowIndex, colIndex) {
            var row = this.gridMatrix[rowIndex];
            if(!row) return;
            delete row[colIndex];
            if(this.getObjectSize(row) > 0) return;
            delete this.gridMatrix[rowIndex];
        },
        containsRange: function(range) {
            for(var r = range.top; r <= range.bottom; r++) {
                for(var c = range.left; c <= range.right; c++)
                    if(!this.getGridTileInfo(r, c))
                        return false;
            }
            return true;
        },
        getObjectSize: function(obj) {
            if(Object.keys)
                return Object.keys(obj).length;
            var count = 0;
            for(var key in obj) {
                if(obj.hasOwnProperty(key))
                    count++;
            }
            return count;
        }
    });
})();