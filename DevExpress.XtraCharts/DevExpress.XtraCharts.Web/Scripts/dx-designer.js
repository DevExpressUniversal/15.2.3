var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        var ElementActions = (function () {
            function ElementActions(surfaceContext, selection) {
                var alignHandler = new AlignmentHandler(selection, surfaceContext);
                this.actions = [{
                    text: "Align to Grid",
                    imageClassName: "dxrd-image-actions-align_to_grid",
                    disabled: ko.observable(false),
                    visible: true,
                    clickAction: function () {
                        alignHandler.alignToGrid();
                    },
                }, {
                    text: "Size to Grid",
                    imageClassName: "dxrd-image-actions-size_to_grid",
                    disabled: ko.observable(false),
                    visible: true,
                    clickAction: function () {
                        alignHandler.sizeToGrid();
                    },
                }, {
                    text: "Center Horizontally",
                    imageClassName: "dxrd-image-actions-center_horizontally",
                    disabled: ko.observable(false),
                    visible: true,
                    clickAction: function () {
                        alignHandler.centerHorizontally();
                    },
                }, {
                    text: "Center Vertically",
                    imageClassName: "dxrd-image-actions-center_vertically",
                    disabled: ko.observable(false),
                    visible: true,
                    clickAction: function () {
                        alignHandler.centerVertically();
                    },
                }, {
                    text: "Bring to Front",
                    imageClassName: "dxrd-image-actions-bring_to_front",
                    disabled: ko.observable(false),
                    visible: true,
                    clickAction: function () {
                        alignHandler.bringToFront();
                    },
                }, {
                    text: "Send to Back",
                    imageClassName: "dxrd-image-actions-send_to_back",
                    disabled: ko.observable(false),
                    visible: true,
                    clickAction: function () {
                        alignHandler.sendToBack();
                    },
                }];
            }
            return ElementActions;
        })();
        Designer.ElementActions = ElementActions;
        var ElementsGroupActions = (function () {
            function ElementsGroupActions(surfaceContext, selection) {
                var alignHandler = new AlignmentHandler(selection, surfaceContext), spaceCommandHandler = new SpaceCommandHandler(selection, surfaceContext);
                this.actions = [{
                    text: "Align Lefts",
                    imageClassName: "dxrd-image-actions-align_lefts",
                    disabled: ko.observable(false),
                    visible: true,
                    clickAction: function () {
                        alignHandler.alignLeft();
                    },
                }, {
                    text: "Align Centers",
                    imageClassName: "dxrd-image-actions-align_centers",
                    disabled: ko.observable(false),
                    visible: true,
                    clickAction: function () {
                        alignHandler.alignVerticalCenters();
                    },
                }, {
                    text: "Align Rights",
                    imageClassName: "dxrd-image-actions-align_rights",
                    disabled: ko.observable(false),
                    visible: true,
                    clickAction: function () {
                        alignHandler.alignRight();
                    },
                }, {
                    text: "Align Tops",
                    imageClassName: "dxrd-image-actions-align_tops",
                    disabled: ko.observable(false),
                    visible: true,
                    clickAction: function () {
                        alignHandler.alignTop();
                    },
                }, {
                    text: "Align Middles",
                    imageClassName: "dxrd-image-actions-align_middles",
                    disabled: ko.observable(false),
                    visible: true,
                    clickAction: function () {
                        alignHandler.alignHorizontalCenters();
                    },
                }, {
                    text: "Align Bottoms",
                    imageClassName: "dxrd-image-actions-align_bottoms",
                    disabled: ko.observable(false),
                    visible: true,
                    clickAction: function () {
                        alignHandler.alignBottom();
                    },
                }, {
                    text: "Size to Control Width",
                    imageClassName: "dxrd-image-actions-make_same_width",
                    disabled: ko.observable(false),
                    visible: true,
                    clickAction: function () {
                        alignHandler.sizeToControlWidth();
                    },
                }, {
                    text: "Size to Control Height",
                    imageClassName: "dxrd-image-actions-make_same_height",
                    disabled: ko.observable(false),
                    visible: true,
                    clickAction: function () {
                        alignHandler.sizeToControlHeight();
                    },
                }, {
                    text: "Size to Control",
                    imageClassName: "dxrd-image-actions-make_same_sizes",
                    disabled: ko.observable(false),
                    visible: true,
                    clickAction: function () {
                        alignHandler.sizeToControl();
                    },
                }, {
                    text: "Make Horizontal Spacing Equal",
                    imageClassName: "dxrd-image-actions-make_horizontal_spacing_equal",
                    disabled: ko.observable(false),
                    visible: true,
                    clickAction: function () {
                        spaceCommandHandler.horizSpaceMakeEqual();
                    },
                }, {
                    text: "Increase Horizontal Spacing",
                    imageClassName: "dxrd-image-actions-increase_horizontal_spacing",
                    disabled: ko.observable(false),
                    visible: true,
                    clickAction: function () {
                        spaceCommandHandler.horizSpaceIncrease();
                    },
                }, {
                    text: "Decrease Horizontal Spacing",
                    imageClassName: "dxrd-image-actions-decrease_horizontal_spacing",
                    disabled: ko.observable(false),
                    visible: true,
                    clickAction: function () {
                        spaceCommandHandler.horizSpaceDecrease();
                    },
                }, {
                    text: "Remove Horizontal Spacing",
                    imageClassName: "dxrd-image-actions-remove_horizontal_spacing",
                    disabled: ko.observable(false),
                    visible: true,
                    clickAction: function () {
                        spaceCommandHandler.horizSpaceConcatenate();
                    },
                }, {
                    text: "Make Vertical Spacing Equal",
                    imageClassName: "dxrd-image-actions-make_vertical_spacing_equal",
                    disabled: ko.observable(false),
                    visible: true,
                    clickAction: function () {
                        spaceCommandHandler.vertSpaceMakeEqual();
                    },
                }, {
                    text: "Increase Vertical Spacing",
                    imageClassName: "dxrd-image-actions-increase_vertical_spacing",
                    disabled: ko.observable(false),
                    visible: true,
                    clickAction: function () {
                        spaceCommandHandler.vertSpaceIncrease();
                    },
                }, {
                    text: "Decrease Vertical Spacing",
                    imageClassName: "dxrd-image-actions-decrease_vertical_spacing",
                    disabled: ko.observable(false),
                    visible: true,
                    clickAction: function () {
                        spaceCommandHandler.vertSpaceDecrease();
                    },
                }, {
                    text: "Remove Vertical Spacing",
                    imageClassName: "dxrd-image-actions-remove_vertical_spacing",
                    disabled: ko.observable(false),
                    visible: true,
                    clickAction: function () {
                        spaceCommandHandler.vertSpaceConcatenate();
                    },
                }];
            }
            return ElementsGroupActions;
        })();
        Designer.ElementsGroupActions = ElementsGroupActions;
        var CopyPasteHandler = (function () {
            function CopyPasteHandler(selectionProvider) {
                var _this = this;
                this._copyInfo = ko.observable(null);
                this.hasPasteInfo = ko.computed(function () {
                    return _this._copyInfo() !== null;
                });
                this._selectionProvider = selectionProvider;
            }
            CopyPasteHandler.prototype.canCopy = function () {
                return this._selectionProvider.focused() !== null && !this._selectionProvider.focused().getControlModel().getMetaData().isCopyDeny;
            };
            CopyPasteHandler.prototype.canPaste = function () {
                var pasteTargetSurface = this._selectionProvider.focused();
                return pasteTargetSurface !== null && this.hasPasteInfo() && pasteTargetSurface.getControlModel().getMetaData().isContainer;
            };
            CopyPasteHandler.prototype.copy = function () {
                if (this.canCopy()) {
                    var serializer = new Designer.DesignerModelSerializer(), copyInfo = {
                        focused: this._selectionProvider.focused(),
                        objects: $.map(this._selectionProvider.selectedItems, function (item) {
                            return serializer.serialize(item.getControlModel());
                        })
                    };
                    this._copyInfo(copyInfo);
                }
            };
            CopyPasteHandler.prototype.cut = function () {
                var serializer = new Designer.DesignerModelSerializer(), cutInfo = {
                    focused: this._selectionProvider.focused(),
                    objects: $.map(this._selectionProvider.selectedItems, function (item) {
                        item.getControlModel().parentModel().removeChild(item.getControlModel());
                        return serializer.serialize(item.getControlModel());
                    })
                };
                this._copyInfo(cutInfo);
            };
            CopyPasteHandler.prototype.paste = function () {
                var _this = this;
                if (this.canPaste()) {
                    var pasteTargetSurface = this._selectionProvider.focused(), pasteTarget = pasteTargetSurface.getControlModel(), newSelection = [];
                    if (pasteTargetSurface === this._copyInfo().focused) {
                        pasteTargetSurface = pasteTargetSurface.parent;
                        pasteTarget = pasteTargetSurface.getControlModel();
                    }
                    if (!pasteTarget.getMetaData().isContainer) {
                        pasteTargetSurface = pasteTargetSurface.parent;
                        pasteTarget = pasteTargetSurface.getControlModel();
                    }
                    $.each(this._copyInfo().objects, function (_, value) {
                        value["@Name"] = undefined;
                        var newControl = pasteTarget.createChild(value);
                        var newControlSurface = Designer.findSurface(newControl);
                        newControlSurface.rect({ left: pasteTargetSurface.rect().width / 2, top: pasteTargetSurface.rect().height / 2 });
                        newSelection.push(newControlSurface);
                    });
                    this._selectionProvider.initialize();
                    newSelection.forEach(function (newControlSurface) {
                        _this._selectionProvider.selecting({ control: newControlSurface, cancel: false });
                    });
                }
            };
            CopyPasteHandler.prototype.deleteControls = function () {
                Designer.deleteSelection(this._selectionProvider);
            };
            return CopyPasteHandler;
        })();
        Designer.CopyPasteHandler = CopyPasteHandler;
        function getActionsTypes(selection) {
            if (selection.selectedItems.length > 1) {
                return [ElementActions, ElementsGroupActions];
            }
            else if (selection.focused()) {
                return selection.focused().getControlModel().getMetaData().elementActionsTypes;
            }
            return null;
        }
        Designer.getActionsTypes = getActionsTypes;
        var ActionsWrapper = (function () {
            function ActionsWrapper(surfaceContext, selection, undoEngine, actionsTypesProvider) {
                var _this = this;
                if (actionsTypesProvider === void 0) { actionsTypesProvider = getActionsTypes; }
                this.visible = ko.observable(false);
                this.actions = ko.observableArray();
                this.collapsed = ko.observable(false);
                ko.computed(function () {
                    var actions = [], elementActionsTypes = actionsTypesProvider(selection);
                    if (elementActionsTypes) {
                        elementActionsTypes.forEach(function (type) {
                            var elementActions = new type(surfaceContext(), selection);
                            actions.push.apply(actions, ko.unwrap(elementActions.actions));
                        }, []);
                        actions.forEach(function (action) {
                            var oldClickHandler = action.clickAction;
                            action.clickAction = function () {
                                undoEngine.start();
                                oldClickHandler.apply(_this);
                                undoEngine.end();
                            };
                        });
                    }
                    _this.actions(actions);
                    _this.visible(_this.actions().length !== 0);
                });
                this.toggleCollapsed = function () {
                    _this.collapsed(!_this.collapsed());
                };
            }
            return ActionsWrapper;
        })();
        Designer.ActionsWrapper = ActionsWrapper;
        var AlignmentHandler = (function () {
            function AlignmentHandler(selectionProvider, surfaceContext) {
                this._selectionProvider = selectionProvider;
                this._surfaceContext = surfaceContext;
            }
            AlignmentHandler.prototype._getFocusedItem = function () {
                return this._selectionProvider.focused();
            };
            AlignmentHandler.prototype._getFocusedParent = function () {
                return this._selectionProvider.focused().parent;
            };
            AlignmentHandler.prototype._visitAllSelectedItemsInSameContainerWithFocused = function (iterator) {
                var parent = this._selectionProvider.focused().parent, focused = this._selectionProvider.focused();
                this._selectionProvider.selectedItems.filter(function (item) {
                    return item !== focused && item.parent === parent;
                }).forEach(function (item) {
                    iterator(item);
                });
            };
            AlignmentHandler.prototype._centerByBand = function (isHoriz, margins) {
                var axisProperty = isHoriz ? "left" : "top", lengthProperty = isHoriz ? "width" : "height", focusedParent = this._getFocusedParent(), parentLengthProperty = focusedParent.rect()[lengthProperty] - margins.right(), minAxis = this._getFocusedItem().rect()[axisProperty], maxSide = this._getFocusedItem().rect()[axisProperty] + this._getFocusedItem().rect()[lengthProperty], newOffset;
                this._selectionProvider.selectedItems.filter(function (item) {
                    return focusedParent === item.parent;
                }).forEach(function (item) {
                    var axis = item.rect()[axisProperty];
                    if (axis < minAxis) {
                        minAxis = axis;
                    }
                });
                this._selectionProvider.selectedItems.filter(function (item) {
                    return focusedParent === item.parent;
                }).forEach(function (item) {
                    var side = item.rect()[axisProperty] + item.rect()[lengthProperty];
                    if (side > maxSide) {
                        maxSide = side;
                    }
                });
                newOffset = (parentLengthProperty - (maxSide - minAxis)) / 2 - minAxis;
                this._selectionProvider.selectedItems.filter(function (item) {
                    return focusedParent === item.parent;
                }).forEach(function (item) {
                    var newVal = {};
                    newVal[axisProperty] = item.rect()[axisProperty] + newOffset;
                    item.rect(newVal);
                });
            };
            AlignmentHandler.prototype._roundingValue = function (value, snapGridSize) {
                return Math.round(value / snapGridSize) * snapGridSize;
            };
            AlignmentHandler.prototype.alignLeft = function () {
                var left = this._getFocusedItem().rect().left;
                this._visitAllSelectedItemsInSameContainerWithFocused(function (item) {
                    item.rect({ left: left });
                });
            };
            AlignmentHandler.prototype.alignTop = function () {
                var top = this._getFocusedItem().rect().top;
                this._visitAllSelectedItemsInSameContainerWithFocused(function (item) {
                    item.rect({ top: top });
                });
            };
            AlignmentHandler.prototype.alignRight = function () {
                var right = this._getFocusedItem().rect().left + this._getFocusedItem().rect().width;
                this._visitAllSelectedItemsInSameContainerWithFocused(function (item) {
                    item.rect({ left: right - item.rect().width });
                });
            };
            AlignmentHandler.prototype.alignBottom = function () {
                var bottom = this._getFocusedItem().rect().top + this._getFocusedItem().rect().height;
                this._visitAllSelectedItemsInSameContainerWithFocused(function (item) {
                    item.rect({ top: bottom - item.rect().height });
                });
            };
            AlignmentHandler.prototype.alignVerticalCenters = function () {
                var verticalCenter = this._getFocusedItem().rect().left + this._getFocusedItem().rect().width / 2;
                this._visitAllSelectedItemsInSameContainerWithFocused(function (item) {
                    item.rect({ left: verticalCenter - item.rect().width / 2 });
                });
            };
            AlignmentHandler.prototype.alignHorizontalCenters = function () {
                var horizontalCenter = this._getFocusedItem().rect().top + this._getFocusedItem().rect().height / 2;
                this._visitAllSelectedItemsInSameContainerWithFocused(function (item) {
                    item.rect({ top: horizontalCenter - item.rect().height / 2 });
                });
            };
            AlignmentHandler.prototype.sizeToControlWidth = function () {
                var newWidth = this._getFocusedItem().rect().width;
                this._visitAllSelectedItemsInSameContainerWithFocused(function (item) {
                    item.rect({ width: newWidth });
                });
            };
            AlignmentHandler.prototype.sizeToControlHeight = function () {
                var newHeight = this._getFocusedItem().rect().height;
                this._visitAllSelectedItemsInSameContainerWithFocused(function (item) {
                    item.rect({ height: newHeight });
                });
            };
            AlignmentHandler.prototype.sizeToControl = function () {
                var newWidth = this._getFocusedItem().rect().width, newHeight = this._getFocusedItem().rect().height;
                this._visitAllSelectedItemsInSameContainerWithFocused(function (item) {
                    item.rect({ width: newWidth, height: newHeight });
                });
            };
            AlignmentHandler.prototype.centerHorizontally = function () {
                this._centerByBand(true, this._surfaceContext.margins);
            };
            AlignmentHandler.prototype.centerVertically = function () {
                this._centerByBand(false, new Designer.Margins(0, 0, 0, 0));
            };
            AlignmentHandler.prototype.alignToGrid = function () {
                var _this = this;
                var snapGridSize = this._surfaceContext.snapGridSize();
                this._selectionProvider.selectedItems.forEach(function (item) {
                    item.rect({
                        left: _this._roundingValue(item.rect().left, snapGridSize),
                        top: _this._roundingValue(item.rect().top, snapGridSize)
                    });
                });
            };
            AlignmentHandler.prototype.sizeToGrid = function () {
                var _this = this;
                var snapGridSize = this._surfaceContext.snapGridSize();
                this._selectionProvider.selectedItems.forEach(function (item) {
                    item.rect({
                        left: _this._roundingValue(item.rect().left, snapGridSize),
                        top: _this._roundingValue(item.rect().top, snapGridSize),
                        width: _this._roundingValue(item.rect().width, snapGridSize),
                        height: _this._roundingValue(item.rect().height, snapGridSize)
                    });
                });
            };
            AlignmentHandler.prototype._bringItemToFront = function (controlSurface) {
                var parentControls = controlSurface.getControlModel().parentModel()["controls"], itemIndex = parentControls().indexOf(controlSurface.getControlModel());
                parentControls.splice(itemIndex, 1);
                parentControls.push(controlSurface.getControlModel());
            };
            AlignmentHandler.prototype.bringToFront = function () {
                var _this = this;
                this._selectionProvider.selectedItems.forEach(function (item) {
                    if (!item.focused()) {
                        _this._bringItemToFront(item);
                    }
                });
                this._bringItemToFront(this._getFocusedItem());
            };
            AlignmentHandler.prototype.sendToBack = function () {
                var reverseSelectedItems = this._selectionProvider.selectedItems;
                reverseSelectedItems.reverse();
                reverseSelectedItems.forEach(function (item) {
                    var itemIndex = item.getControlModel().parentModel()["controls"].indexOf(item.getControlModel());
                    item.getControlModel().parentModel()["controls"].splice(itemIndex, 1);
                    item.getControlModel().parentModel()["controls"].splice(0, 0, item.getControlModel());
                });
            };
            return AlignmentHandler;
        })();
        Designer.AlignmentHandler = AlignmentHandler;
        var SpaceCommandHandler = (function () {
            function SpaceCommandHandler(selectionProvider, surfaceContext) {
                this._selectionProvider = selectionProvider;
                this._surfaceContext = surfaceContext;
            }
            SpaceCommandHandler.prototype._comparer = function (propertyName) {
                return function (a, b) {
                    return a.rect()[propertyName] - b.rect()[propertyName];
                };
            };
            SpaceCommandHandler.prototype._spaceIncrease = function (sign, isHoriz) {
                var sortedSelectedItems = this._selectionProvider.selectedItems, axisProperty = isHoriz ? "left" : "top", lengthProperty = isHoriz ? "width" : "height", margin = isHoriz ? this._surfaceContext.margins.left() : 0, snapGridSize = this._surfaceContext.snapGridSize(), focusedParent = this._selectionProvider.focused().getControlModel().parentModel(), focusedItem = this._selectionProvider.focused();
                sortedSelectedItems.sort(this._comparer(axisProperty));
                var focusedItemIndex = sortedSelectedItems.indexOf(this._selectionProvider.focused());
                this._selectionProvider.selectedItems.filter(function (item) {
                    return item !== focusedItem && item.getControlModel().parentModel() === focusedParent;
                }).forEach(function (item) {
                    var itemIndex = sortedSelectedItems.indexOf(item), spaceOffset = Math.abs(itemIndex - focusedItemIndex) * snapGridSize * sign, itemAxisProperty = item.rect()[axisProperty], itemLengthProperty = item.rect()[lengthProperty], parentLengthProperty = item.parent.rect()[lengthProperty] - margin, newValue;
                    if (itemIndex < focusedItemIndex) {
                        newValue = itemAxisProperty - spaceOffset;
                        if (newValue < 0) {
                            newValue = 0;
                        }
                    }
                    else {
                        newValue = itemAxisProperty + spaceOffset;
                        if ((newValue + itemLengthProperty) > parentLengthProperty) {
                            newValue = parentLengthProperty - itemLengthProperty;
                        }
                    }
                    var val = {};
                    val[axisProperty] = newValue;
                    item.rect(val);
                });
            };
            SpaceCommandHandler.prototype._spaceMakeEqual = function (isHoriz) {
                this._concatenateWithSpace(isHoriz, function (sortedSelectedItems, axisProperty, lengthProperty) {
                    var averageSpace = 0;
                    for (var i = 0; i < sortedSelectedItems.length - 1; i++) {
                        var currentValue = sortedSelectedItems[i + 1].rect()[axisProperty] - (sortedSelectedItems[i].rect()[axisProperty] + sortedSelectedItems[i].rect()[lengthProperty]);
                        averageSpace = (averageSpace * i + currentValue) / (i + 1);
                    }
                    return averageSpace;
                });
            };
            SpaceCommandHandler.prototype._concatenateWithSpace = function (isHoriz, getSpaceSize) {
                var sortedSelectedItems = this._selectionProvider.selectedItems, axisProperty = isHoriz ? "left" : "top", lengthProperty = isHoriz ? "width" : "height", spaceSize = 0, focusedParent = this._selectionProvider.focused().getControlModel().parentModel();
                sortedSelectedItems.sort(this._comparer(axisProperty));
                spaceSize = getSpaceSize(sortedSelectedItems, axisProperty, lengthProperty);
                this._selectionProvider.selectedItems.filter(function (item) {
                    return focusedParent === item.getControlModel().parentModel();
                }).forEach(function (item) {
                    var itemIndex = sortedSelectedItems.indexOf(item);
                    if (itemIndex > 0) {
                        var prevControl = sortedSelectedItems[itemIndex - 1], val = {};
                        val[axisProperty] = prevControl.rect()[axisProperty] + prevControl.rect()[lengthProperty] + spaceSize;
                        item.rect(val);
                    }
                });
            };
            SpaceCommandHandler.prototype.horizSpaceConcatenate = function () {
                this._concatenateWithSpace(true, function () {
                    return 0;
                });
            };
            SpaceCommandHandler.prototype.vertSpaceConcatenate = function () {
                this._concatenateWithSpace(false, function () {
                    return 0;
                });
            };
            SpaceCommandHandler.prototype.horizSpaceMakeEqual = function () {
                this._spaceMakeEqual(true);
            };
            SpaceCommandHandler.prototype.vertSpaceMakeEqual = function () {
                this._spaceMakeEqual(false);
            };
            SpaceCommandHandler.prototype.horizSpaceDecrease = function () {
                this._spaceIncrease(-1, true);
            };
            SpaceCommandHandler.prototype.horizSpaceIncrease = function () {
                this._spaceIncrease(1, true);
            };
            SpaceCommandHandler.prototype.vertSpaceDecrease = function () {
                this._spaceIncrease(-1, false);
            };
            SpaceCommandHandler.prototype.vertSpaceIncrease = function () {
                this._spaceIncrease(1, false);
            };
            return SpaceCommandHandler;
        })();
        Designer.SpaceCommandHandler = SpaceCommandHandler;
        var KeyboardHelper = (function () {
            function KeyboardHelper(selection, surfaceContext, copyPasteHandler, undoEngine) {
                var _this = this;
                this._selection = selection;
                this._undoEngine = undoEngine;
                this.shortcutMap = {
                    27: function (e) {
                        _this.processEsc();
                        return true;
                    },
                    37: function (e) {
                        _this.moveSelectedControls(true, true, -1);
                        return true;
                    },
                    38: function (e) {
                        _this.moveSelectedControls(true, false, -1);
                        return true;
                    },
                    39: function (e) {
                        _this.moveSelectedControls(false, true, 1);
                        return true;
                    },
                    40: function (e) {
                        _this.moveSelectedControls(false, false, 1);
                        return true;
                    },
                    46: function (e) {
                        if (!_this._selection.focused() || _this._selection.focused() && _this._selection.focused().getControlModel().getMetaData().isDeleteDeny) {
                            return true;
                        }
                        undoEngine && undoEngine.start();
                        copyPasteHandler.deleteControls();
                        undoEngine && undoEngine.end();
                        return true;
                    },
                    107: function (e) {
                        return _this._processKey(e, function () {
                            surfaceContext().zoom(surfaceContext().zoom() + 0.01);
                        });
                    },
                    109: function (e) {
                        return _this._processKey(e, function () {
                            surfaceContext().zoom(surfaceContext().zoom() - 0.01);
                        });
                    },
                    187: function (e) {
                        return _this._processKey(e, function () {
                            surfaceContext().zoom(1);
                        });
                    },
                };
            }
            KeyboardHelper.prototype._processKey = function (e, func) {
                if (e.ctrlKey) {
                    func(e);
                    return true;
                }
                return false;
            };
            KeyboardHelper.prototype.processShortcut = function (e) {
                var method = this.shortcutMap[e.keyCode];
                if (method) {
                    return method(e);
                }
                return false;
            };
            KeyboardHelper.prototype.processEsc = function () {
                var parent = this._selection.focused() && this._selection.focused().parent;
                parent && this._selection.focused(parent);
            };
            KeyboardHelper.prototype.moveSelectedControls = function (leftUp, isHoriz, sign) {
                var focusedControl = this._selection.focused();
                if (!focusedControl || focusedControl && focusedControl.getControlModel().getMetaData().isCopyDeny) {
                    return;
                }
                this._undoEngine && this._undoEngine.start();
                var distance = 1, axisProperty = isHoriz ? "left" : "top", lengthProperty = isHoriz ? "width" : "height", minAxis, maxSide, newAxis;
                if (focusedControl.rect) {
                    minAxis = focusedControl.rect()[axisProperty];
                    maxSide = focusedControl.rect()[axisProperty] + focusedControl.rect()[lengthProperty];
                }
                else {
                    return;
                }
                this._selection.selectedItems.forEach(function (item) {
                    var asix = item.rect()[axisProperty];
                    if (asix < minAxis) {
                        minAxis = asix;
                    }
                });
                this._selection.selectedItems.forEach(function (item) {
                    var side = item.rect()[axisProperty] + item.rect()[lengthProperty];
                    if (side > maxSide) {
                        maxSide = side;
                    }
                });
                if ((leftUp && minAxis <= 0) || (!focusedControl.parent.rect || (!leftUp && maxSide.toFixed(5) >= focusedControl.parent.rect()[lengthProperty]))) {
                    return;
                }
                else {
                    this._selection.selectedItems.filter(function (item) {
                        return !!item.rect;
                    }).forEach(function (item) {
                        var newVal = {}, itemAxisProperty = item.rect()[axisProperty], itemLengthProperty = item.rect()[lengthProperty], parentLengthProperty = item.parent.rect()[lengthProperty];
                        newAxis = itemAxisProperty + sign * distance;
                        if ((leftUp && newAxis >= 0) || (!leftUp && (newAxis + itemLengthProperty) <= parentLengthProperty)) {
                            newVal[axisProperty] = newAxis;
                        }
                        if (!leftUp && (newAxis + itemLengthProperty) > parentLengthProperty) {
                            newVal[axisProperty] = parentLengthProperty - itemLengthProperty;
                        }
                        if (leftUp && newAxis < 0 && itemAxisProperty > 0) {
                            newVal[axisProperty] = 0;
                        }
                        item.rect(newVal);
                    });
                }
                this._undoEngine && this._undoEngine.end();
            };
            return KeyboardHelper;
        })();
        Designer.KeyboardHelper = KeyboardHelper;
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var DevExpress;
(function (DevExpress) {
    var Data;
    (function (Data) {
        var CriteriaOperator = (function () {
            function CriteriaOperator() {
                var _this = this;
                this.operands = null;
                this.changeValue = function (type, propertyName, value, index) {
                    var result = new type(value);
                    if (index !== null) {
                        _this[propertyName][index] = result;
                    }
                    else {
                        _this[propertyName] = result;
                    }
                    return result;
                };
                this.changeProperty = function (model, propertyName, value, index) {
                    if (index !== null) {
                        _this[propertyName][index] = model;
                    }
                    else {
                        _this[propertyName] = model;
                    }
                };
            }
            CriteriaOperator.operators = function (enums) {
                var result = [].concat.apply([], enums.map(function (enumType) {
                    return getEnumNames(enumType).map(function (enumName) {
                        return { name: enumName, value: enumType[enumName], type: enumType };
                    });
                }));
                return result;
            };
            CriteriaOperator.parse = function (stringCriteria) {
                if (stringCriteria && stringCriteria !== "") {
                    return window["parser"].parse(stringCriteria);
                }
                return null;
            };
            CriteriaOperator.create = function (operatorType, property, rightPart) {
                var operator = null;
                switch (operatorType.type) {
                    case BinaryOperatorType:
                        operator = new BinaryOperator(property, rightPart.length && rightPart[0] || rightPart, operatorType.value);
                        break;
                    case GroupOperatorType:
                        operator = new GroupOperator(operatorType.value, rightPart ? rightPart.length >= 0 && rightPart || [rightPart] : []);
                        break;
                    case FunctionOperatorType:
                        if (operatorType.value === 5 /* IsNullOrEmpty */) {
                            operator = new FunctionOperator(operatorType.value, [property]);
                        }
                        else {
                            operator = new FunctionOperator(operatorType.value, [property, rightPart && rightPart.length && rightPart[0] || rightPart]);
                        }
                        break;
                    case BetweenOperator:
                        operator = new BetweenOperator(property, rightPart && rightPart.length && rightPart[0] || rightPart, new OperandValue(""));
                        break;
                    case InOperator:
                        operator = new InOperator(property, [rightPart.length && rightPart[0] || rightPart]);
                        break;
                    case UnaryOperatorType:
                        operator = new UnaryOperator(operatorType.value, property);
                        break;
                    case Aggregate:
                        if (property instanceof AggregateOperand) {
                            property.operatorType = operatorType.value;
                            if (property.operatorType === 1 /* Exists */ || property.operatorType === 0 /* Count */) {
                                property["aggregatedExpression"] = null;
                            }
                            if (operatorType.value === 1 /* Exists */) {
                                operator = property;
                            }
                            else {
                                operator = new BinaryOperator(property, rightPart.length && rightPart[0] || rightPart, 0 /* Equal */);
                            }
                        }
                        else {
                            var result = new AggregateOperand(property, null, operatorType.value, new GroupOperator(0 /* And */, []));
                            if (operatorType.value === 1 /* Exists */) {
                                operator = result;
                            }
                            else {
                                if (operatorType.value !== 0 /* Count */) {
                                    result.aggregatedExpression = new OperandProperty("");
                                }
                                operator = new BinaryOperator(result, rightPart.length && rightPart[0] || rightPart, 0 /* Equal */);
                            }
                        }
                        break;
                    default:
                        throw Error("Unsupported operator type");
                }
                if (operatorType.reverse) {
                    return new UnaryOperator(3 /* Not */, operator);
                }
                return operator;
            };
            CriteriaOperator.and = function (left, right) {
                return GroupOperator.combine(1 /* Or */, [left, right]);
            };
            CriteriaOperator.or = function (left, right) {
                return GroupOperator.combine(1 /* Or */, [left, right]);
            };
            CriteriaOperator.getNotValidRange = function (value, errorMessage) {
                var start = 0;
                var end = 0;
                var parts = errorMessage.split('\n');
                var errorText = parts[1];
                var errorLength = parts[2].length;
                if (errorText.indexOf('...') === 0) {
                    errorText = errorText.split("...")[1];
                }
                var start = value.indexOf(errorText);
                var end = start + errorLength;
                return { start: start, end: end };
            };
            Object.defineProperty(CriteriaOperator.prototype, "displayType", {
                get: function () {
                    return this.operatorType;
                },
                enumerable: true,
                configurable: true
            });
            Object.defineProperty(CriteriaOperator.prototype, "enumType", {
                get: function () {
                    return null;
                },
                enumerable: true,
                configurable: true
            });
            return CriteriaOperator;
        })();
        Data.CriteriaOperator = CriteriaOperator;
        function getEnumNames(enumType) {
            var result = [];
            for (var enumValue in enumType) {
                if (isNaN(enumValue)) {
                    result.push(enumValue);
                }
            }
            return result;
        }
        (function (GroupOperatorType) {
            GroupOperatorType[GroupOperatorType["And"] = 0] = "And";
            GroupOperatorType[GroupOperatorType["Or"] = 1] = "Or";
        })(Data.GroupOperatorType || (Data.GroupOperatorType = {}));
        var GroupOperatorType = Data.GroupOperatorType;
        var GroupOperator = (function (_super) {
            __extends(GroupOperator, _super);
            function GroupOperator(operation, operands) {
                var _this = this;
                _super.call(this);
                this.create = function (isGroup, property, specifics) {
                    var operator = new BinaryOperator(property, new OperandValue(""), 0 /* Equal */);
                    if (isGroup) {
                        operator = new GroupOperator(0 /* And */, []);
                    }
                    else if (specifics && specifics === "list") {
                        operator = new AggregateOperand(property, null, 1 /* Exists */, new GroupOperator(0 /* And */, []));
                    }
                    _this.operands.push(operator);
                    return _this.operands[_this.operands.indexOf(operator)];
                };
                this.change = function (operationType, item, property, rightPart) {
                    var position = _this.operands.indexOf(item);
                    if (position !== -1) {
                        _this.operands[position] = CriteriaOperator.create(operationType, property, rightPart);
                    }
                    else {
                        throw Error("dont have this element in operands collection");
                    }
                    item = null;
                    return _this.operands[position];
                };
                this.remove = function (operator) {
                    _this.operands.splice(_this.operands.indexOf(operator), 1);
                };
                this.operands = [];
                this.operatorType = operation;
                operands = operands || [new CriteriaOperator(), new CriteriaOperator()];
                operands.forEach(function (operand) { return _this.operands.push(operand); });
            }
            GroupOperator.combine = function (operation, operands) {
                var combinedOperands = [];
                (operands || []).forEach(function (operand) {
                    if (operand instanceof GroupOperator && operand.operatorType === operation) {
                        combinedOperands.push.apply(combinedOperands, operand.operands);
                    }
                    else {
                        combinedOperands.push(operand);
                    }
                });
                if (combinedOperands.length === 1) {
                    return combinedOperands[0];
                }
                return new GroupOperator(operation, combinedOperands);
            };
            Object.defineProperty(GroupOperator.prototype, "displayType", {
                get: function () {
                    return GroupOperatorType[this.operatorType];
                },
                enumerable: true,
                configurable: true
            });
            Object.defineProperty(GroupOperator.prototype, "enumType", {
                get: function () {
                    return GroupOperatorType;
                },
                enumerable: true,
                configurable: true
            });
            return GroupOperator;
        })(CriteriaOperator);
        Data.GroupOperator = GroupOperator;
        var OperandProperty = (function (_super) {
            __extends(OperandProperty, _super);
            function OperandProperty(propertyName) {
                _super.call(this);
                this.propertyName = propertyName;
            }
            Object.defineProperty(OperandProperty.prototype, "displayType", {
                get: function () {
                    return '[' + this.propertyName + ']';
                },
                enumerable: true,
                configurable: true
            });
            return OperandProperty;
        })(CriteriaOperator);
        Data.OperandProperty = OperandProperty;
        var OperandValue = (function (_super) {
            __extends(OperandValue, _super);
            function OperandValue(value) {
                _super.call(this);
                this.isNumber = false;
                var result = value;
                if (value && value["length"] && value[0] === "'" && value[value.length - 1] === "'") {
                    result = value.slice(1, value.length - 1);
                }
                else if (value && value["length"] && value[0] === "#" && value[value.length - 1] === "#") {
                    result = value.slice(1, value.length - 1);
                    result = Globalize.parseDate(result);
                    if (!result) {
                        result = Globalize.parseDate(value.slice(1, value.length - 1), ["yyyy-MM-dd", "MM/dd/yyyy HH:mm:ss"]);
                    }
                }
                else {
                    this.isNumber = $.isNumeric(value);
                }
                this.value = result;
            }
            Object.defineProperty(OperandValue.prototype, "displayType", {
                get: function () {
                    return this.value || "?";
                },
                enumerable: true,
                configurable: true
            });
            return OperandValue;
        })(CriteriaOperator);
        Data.OperandValue = OperandValue;
        var ConstantValue = (function (_super) {
            __extends(ConstantValue, _super);
            function ConstantValue(value) {
                _super.call(this, value);
            }
            return ConstantValue;
        })(OperandValue);
        Data.ConstantValue = ConstantValue;
        var OperandParameter = (function (_super) {
            __extends(OperandParameter, _super);
            function OperandParameter(parameterName, value) {
                _super.call(this, value);
                this.parameterName = parameterName;
            }
            Object.defineProperty(OperandParameter.prototype, "displayType", {
                get: function () {
                    return '?' + this.parameterName;
                },
                enumerable: true,
                configurable: true
            });
            return OperandParameter;
        })(OperandValue);
        Data.OperandParameter = OperandParameter;
        (function (Aggregate) {
            Aggregate[Aggregate["Count"] = 0] = "Count";
            Aggregate[Aggregate["Exists"] = 1] = "Exists";
            Aggregate[Aggregate["Min"] = 2] = "Min";
            Aggregate[Aggregate["Max"] = 3] = "Max";
            Aggregate[Aggregate["Avg"] = 4] = "Avg";
            Aggregate[Aggregate["Sum"] = 5] = "Sum";
            Aggregate[Aggregate["Single"] = 6] = "Single";
        })(Data.Aggregate || (Data.Aggregate = {}));
        var Aggregate = Data.Aggregate;
        var AggregateOperand = (function (_super) {
            __extends(AggregateOperand, _super);
            function AggregateOperand(property, aggregatedExpression, aggregateType, condition) {
                var _this = this;
                _super.call(this);
                this.change = function (operationType, item, property, rightPart) {
                    if (operationType.type === GroupOperatorType) {
                        _this.condition = CriteriaOperator.create(operationType, property, rightPart);
                        item = null;
                        return _this.condition;
                    }
                    else {
                        return null;
                    }
                };
                this.property = property;
                if (condition instanceof GroupOperator) {
                    this.condition = condition;
                }
                else {
                    if (condition instanceof UnaryOperator && condition.operatorType === 3 /* Not */) {
                        if (condition.operand instanceof GroupOperator) {
                            this.condition = new UnaryOperator(3 /* Not */, condition.operand);
                        }
                        else {
                            this.condition = new UnaryOperator(3 /* Not */, new GroupOperator(0 /* And */, condition.operand ? [condition.operand] : []));
                        }
                    }
                    else {
                        this.condition = new GroupOperator(0 /* And */, condition ? [condition] : []);
                    }
                }
                this.operatorType = aggregateType;
                this.aggregatedExpression = aggregatedExpression;
            }
            Object.defineProperty(AggregateOperand.prototype, "displayType", {
                get: function () {
                    return Aggregate[this.operatorType];
                },
                enumerable: true,
                configurable: true
            });
            Object.defineProperty(AggregateOperand.prototype, "enumType", {
                get: function () {
                    return Aggregate;
                },
                enumerable: true,
                configurable: true
            });
            return AggregateOperand;
        })(CriteriaOperator);
        Data.AggregateOperand = AggregateOperand;
        var JoinOperand = (function (_super) {
            __extends(JoinOperand, _super);
            function JoinOperand(joinTypeName, condition, type, aggregated) {
                _super.call(this);
                this.joinTypeName = joinTypeName;
                this.condition = condition;
                this.operatorType = type;
                this.aggregatedExpression = aggregated;
            }
            JoinOperand.joinOrAggregate = function (collectionProperty, condition, type, aggregated) {
                if (collectionProperty === null || collectionProperty.propertyName.length < 2 || collectionProperty.propertyName[0] != '<' || collectionProperty.propertyName[collectionProperty.propertyName.length - 1] != '>') {
                    return new AggregateOperand(collectionProperty, aggregated, type, condition);
                }
                else {
                    return new JoinOperand(collectionProperty.propertyName.substring(1, collectionProperty.propertyName.length - 2), condition, type, aggregated);
                }
            };
            return JoinOperand;
        })(CriteriaOperator);
        Data.JoinOperand = JoinOperand;
        var BetweenOperator = (function (_super) {
            __extends(BetweenOperator, _super);
            function BetweenOperator(property, begin, end) {
                _super.call(this);
                this.property = property;
                this.begin = begin;
                this.end = end;
            }
            Object.defineProperty(BetweenOperator.prototype, "displayType", {
                get: function () {
                    return "Between";
                },
                enumerable: true,
                configurable: true
            });
            Object.defineProperty(BetweenOperator.prototype, "operatorType", {
                get: function () {
                    return "Between";
                },
                enumerable: true,
                configurable: true
            });
            Object.defineProperty(BetweenOperator.prototype, "enumType", {
                get: function () {
                    return BetweenOperator;
                },
                enumerable: true,
                configurable: true
            });
            return BetweenOperator;
        })(CriteriaOperator);
        Data.BetweenOperator = BetweenOperator;
        var InOperator = (function (_super) {
            __extends(InOperator, _super);
            function InOperator(criteriaOperator, operands) {
                var _this = this;
                _super.call(this);
                this.operands = [];
                this.criteriaOperator = criteriaOperator;
                operands.forEach(function (operand) { return _this.operands.push(operand); });
            }
            Object.defineProperty(InOperator.prototype, "displayType", {
                get: function () {
                    return "In";
                },
                enumerable: true,
                configurable: true
            });
            Object.defineProperty(InOperator.prototype, "operatorType", {
                get: function () {
                    return "In";
                },
                enumerable: true,
                configurable: true
            });
            Object.defineProperty(InOperator.prototype, "enumType", {
                get: function () {
                    return InOperator;
                },
                enumerable: true,
                configurable: true
            });
            return InOperator;
        })(CriteriaOperator);
        Data.InOperator = InOperator;
        (function (BinaryOperatorType) {
            BinaryOperatorType[BinaryOperatorType["Equal"] = 0] = "Equal";
            BinaryOperatorType[BinaryOperatorType["NotEqual"] = 1] = "NotEqual";
            BinaryOperatorType[BinaryOperatorType["Greater"] = 2] = "Greater";
            BinaryOperatorType[BinaryOperatorType["Less"] = 3] = "Less";
            BinaryOperatorType[BinaryOperatorType["LessOrEqual"] = 4] = "LessOrEqual";
            BinaryOperatorType[BinaryOperatorType["GreaterOrEqual"] = 5] = "GreaterOrEqual";
            BinaryOperatorType[BinaryOperatorType["Like"] = 6] = "Like";
            BinaryOperatorType[BinaryOperatorType["BitwiseAnd"] = 7] = "BitwiseAnd";
            BinaryOperatorType[BinaryOperatorType["BitwiseOr"] = 8] = "BitwiseOr";
            BinaryOperatorType[BinaryOperatorType["BitwiseXor"] = 9] = "BitwiseXor";
            BinaryOperatorType[BinaryOperatorType["Divide"] = 10] = "Divide";
            BinaryOperatorType[BinaryOperatorType["Modulo"] = 11] = "Modulo";
            BinaryOperatorType[BinaryOperatorType["Multiply"] = 12] = "Multiply";
            BinaryOperatorType[BinaryOperatorType["Plus"] = 13] = "Plus";
            BinaryOperatorType[BinaryOperatorType["Minus"] = 14] = "Minus";
        })(Data.BinaryOperatorType || (Data.BinaryOperatorType = {}));
        var BinaryOperatorType = Data.BinaryOperatorType;
        var BinaryOperator = (function (_super) {
            __extends(BinaryOperator, _super);
            function BinaryOperator(left, right, operatorType) {
                _super.call(this);
                this.leftOperand = left || new CriteriaOperator();
                this.rightOperand = right || new CriteriaOperator();
                this.operatorType = operatorType;
            }
            Object.defineProperty(BinaryOperator.prototype, "displayType", {
                get: function () {
                    return Data.operatorTokens[BinaryOperatorType[this.operatorType]] || BinaryOperatorType[this.operatorType];
                },
                enumerable: true,
                configurable: true
            });
            Object.defineProperty(BinaryOperator.prototype, "enumType", {
                get: function () {
                    return BinaryOperatorType;
                },
                enumerable: true,
                configurable: true
            });
            return BinaryOperator;
        })(CriteriaOperator);
        Data.BinaryOperator = BinaryOperator;
        (function (UnaryOperatorType) {
            UnaryOperatorType[UnaryOperatorType["Minus"] = 0] = "Minus";
            UnaryOperatorType[UnaryOperatorType["Plus"] = 1] = "Plus";
            UnaryOperatorType[UnaryOperatorType["BitwiseNot"] = 2] = "BitwiseNot";
            UnaryOperatorType[UnaryOperatorType["Not"] = 3] = "Not";
            UnaryOperatorType[UnaryOperatorType["IsNull"] = 4] = "IsNull";
        })(Data.UnaryOperatorType || (Data.UnaryOperatorType = {}));
        var UnaryOperatorType = Data.UnaryOperatorType;
        var UnaryOperator = (function (_super) {
            __extends(UnaryOperator, _super);
            function UnaryOperator(operatorType, operand) {
                _super.call(this);
                this.operand = operand;
                this.operatorType = operatorType;
            }
            Object.defineProperty(UnaryOperator.prototype, "displayType", {
                get: function () {
                    return UnaryOperatorType[this.operatorType];
                },
                enumerable: true,
                configurable: true
            });
            Object.defineProperty(UnaryOperator.prototype, "enumType", {
                get: function () {
                    return UnaryOperatorType;
                },
                enumerable: true,
                configurable: true
            });
            return UnaryOperator;
        })(CriteriaOperator);
        Data.UnaryOperator = UnaryOperator;
        (function (FunctionOperatorType) {
            FunctionOperatorType[FunctionOperatorType["None"] = 0] = "None";
            FunctionOperatorType[FunctionOperatorType["Custom"] = 1] = "Custom";
            FunctionOperatorType[FunctionOperatorType["CustomNonDeterministic"] = 2] = "CustomNonDeterministic";
            FunctionOperatorType[FunctionOperatorType["Iif"] = 3] = "Iif";
            FunctionOperatorType[FunctionOperatorType["IsNull"] = 4] = "IsNull";
            FunctionOperatorType[FunctionOperatorType["IsNullOrEmpty"] = 5] = "IsNullOrEmpty";
            FunctionOperatorType[FunctionOperatorType["Trim"] = 6] = "Trim";
            FunctionOperatorType[FunctionOperatorType["Len"] = 7] = "Len";
            FunctionOperatorType[FunctionOperatorType["Substring"] = 8] = "Substring";
            FunctionOperatorType[FunctionOperatorType["Upper"] = 9] = "Upper";
            FunctionOperatorType[FunctionOperatorType["Lower"] = 10] = "Lower";
            FunctionOperatorType[FunctionOperatorType["Concat"] = 11] = "Concat";
            FunctionOperatorType[FunctionOperatorType["Ascii"] = 12] = "Ascii";
            FunctionOperatorType[FunctionOperatorType["Char"] = 13] = "Char";
            FunctionOperatorType[FunctionOperatorType["ToStr"] = 14] = "ToStr";
            FunctionOperatorType[FunctionOperatorType["Replace"] = 15] = "Replace";
            FunctionOperatorType[FunctionOperatorType["Reverse"] = 16] = "Reverse";
            FunctionOperatorType[FunctionOperatorType["Insert"] = 17] = "Insert";
            FunctionOperatorType[FunctionOperatorType["CharIndex"] = 18] = "CharIndex";
            FunctionOperatorType[FunctionOperatorType["Remove"] = 19] = "Remove";
            FunctionOperatorType[FunctionOperatorType["Abs"] = 20] = "Abs";
            FunctionOperatorType[FunctionOperatorType["Sqr"] = 21] = "Sqr";
            FunctionOperatorType[FunctionOperatorType["Cos"] = 22] = "Cos";
            FunctionOperatorType[FunctionOperatorType["Sin"] = 23] = "Sin";
            FunctionOperatorType[FunctionOperatorType["Atn"] = 24] = "Atn";
            FunctionOperatorType[FunctionOperatorType["Exp"] = 25] = "Exp";
            FunctionOperatorType[FunctionOperatorType["Log"] = 26] = "Log";
            FunctionOperatorType[FunctionOperatorType["Rnd"] = 27] = "Rnd";
            FunctionOperatorType[FunctionOperatorType["Tan"] = 28] = "Tan";
            FunctionOperatorType[FunctionOperatorType["Power"] = 29] = "Power";
            FunctionOperatorType[FunctionOperatorType["Sign"] = 30] = "Sign";
            FunctionOperatorType[FunctionOperatorType["Round"] = 31] = "Round";
            FunctionOperatorType[FunctionOperatorType["Ceiling"] = 32] = "Ceiling";
            FunctionOperatorType[FunctionOperatorType["Floor"] = 33] = "Floor";
            FunctionOperatorType[FunctionOperatorType["Max"] = 34] = "Max";
            FunctionOperatorType[FunctionOperatorType["Min"] = 35] = "Min";
            FunctionOperatorType[FunctionOperatorType["Acos"] = 36] = "Acos";
            FunctionOperatorType[FunctionOperatorType["Asin"] = 37] = "Asin";
            FunctionOperatorType[FunctionOperatorType["Atn2"] = 38] = "Atn2";
            FunctionOperatorType[FunctionOperatorType["BigMul"] = 39] = "BigMul";
            FunctionOperatorType[FunctionOperatorType["Cosh"] = 40] = "Cosh";
            FunctionOperatorType[FunctionOperatorType["Log10"] = 41] = "Log10";
            FunctionOperatorType[FunctionOperatorType["Sinh"] = 42] = "Sinh";
            FunctionOperatorType[FunctionOperatorType["Tanh"] = 43] = "Tanh";
            FunctionOperatorType[FunctionOperatorType["PadLeft"] = 44] = "PadLeft";
            FunctionOperatorType[FunctionOperatorType["PadRight"] = 45] = "PadRight";
            FunctionOperatorType[FunctionOperatorType["StartsWith"] = 46] = "StartsWith";
            FunctionOperatorType[FunctionOperatorType["EndsWith"] = 47] = "EndsWith";
            FunctionOperatorType[FunctionOperatorType["Contains"] = 48] = "Contains";
            FunctionOperatorType[FunctionOperatorType["ToInt"] = 49] = "ToInt";
            FunctionOperatorType[FunctionOperatorType["ToLong"] = 50] = "ToLong";
            FunctionOperatorType[FunctionOperatorType["ToFloat"] = 51] = "ToFloat";
            FunctionOperatorType[FunctionOperatorType["ToDouble"] = 52] = "ToDouble";
            FunctionOperatorType[FunctionOperatorType["ToDecimal"] = 53] = "ToDecimal";
            FunctionOperatorType[FunctionOperatorType["LocalDateTimeThisYear"] = 54] = "LocalDateTimeThisYear";
            FunctionOperatorType[FunctionOperatorType["LocalDateTimeThisMonth"] = 55] = "LocalDateTimeThisMonth";
            FunctionOperatorType[FunctionOperatorType["LocalDateTimeLastWeek"] = 56] = "LocalDateTimeLastWeek";
            FunctionOperatorType[FunctionOperatorType["LocalDateTimeThisWeek"] = 57] = "LocalDateTimeThisWeek";
            FunctionOperatorType[FunctionOperatorType["LocalDateTimeYesterday"] = 58] = "LocalDateTimeYesterday";
            FunctionOperatorType[FunctionOperatorType["LocalDateTimeToday"] = 59] = "LocalDateTimeToday";
            FunctionOperatorType[FunctionOperatorType["LocalDateTimeNow"] = 60] = "LocalDateTimeNow";
            FunctionOperatorType[FunctionOperatorType["LocalDateTimeTomorrow"] = 61] = "LocalDateTimeTomorrow";
            FunctionOperatorType[FunctionOperatorType["LocalDateTimeDayAfterTomorrow"] = 62] = "LocalDateTimeDayAfterTomorrow";
            FunctionOperatorType[FunctionOperatorType["LocalDateTimeNextWeek"] = 63] = "LocalDateTimeNextWeek";
            FunctionOperatorType[FunctionOperatorType["LocalDateTimeTwoWeeksAway"] = 64] = "LocalDateTimeTwoWeeksAway";
            FunctionOperatorType[FunctionOperatorType["LocalDateTimeNextMonth"] = 65] = "LocalDateTimeNextMonth";
            FunctionOperatorType[FunctionOperatorType["LocalDateTimeNextYear"] = 66] = "LocalDateTimeNextYear";
            FunctionOperatorType[FunctionOperatorType["IsOutlookIntervalBeyondThisYear"] = 67] = "IsOutlookIntervalBeyondThisYear";
            FunctionOperatorType[FunctionOperatorType["IsOutlookIntervalLaterThisYear"] = 68] = "IsOutlookIntervalLaterThisYear";
            FunctionOperatorType[FunctionOperatorType["IsOutlookIntervalLaterThisMonth"] = 69] = "IsOutlookIntervalLaterThisMonth";
            FunctionOperatorType[FunctionOperatorType["IsOutlookIntervalNextWeek"] = 70] = "IsOutlookIntervalNextWeek";
            FunctionOperatorType[FunctionOperatorType["IsOutlookIntervalLaterThisWeek"] = 71] = "IsOutlookIntervalLaterThisWeek";
            FunctionOperatorType[FunctionOperatorType["IsOutlookIntervalTomorrow"] = 72] = "IsOutlookIntervalTomorrow";
            FunctionOperatorType[FunctionOperatorType["IsOutlookIntervalToday"] = 73] = "IsOutlookIntervalToday";
            FunctionOperatorType[FunctionOperatorType["IsOutlookIntervalYesterday"] = 74] = "IsOutlookIntervalYesterday";
            FunctionOperatorType[FunctionOperatorType["IsOutlookIntervalEarlierThisWeek"] = 75] = "IsOutlookIntervalEarlierThisWeek";
            FunctionOperatorType[FunctionOperatorType["IsOutlookIntervalLastWeek"] = 76] = "IsOutlookIntervalLastWeek";
            FunctionOperatorType[FunctionOperatorType["IsOutlookIntervalEarlierThisMonth"] = 77] = "IsOutlookIntervalEarlierThisMonth";
            FunctionOperatorType[FunctionOperatorType["IsOutlookIntervalEarlierThisYear"] = 78] = "IsOutlookIntervalEarlierThisYear";
            FunctionOperatorType[FunctionOperatorType["IsOutlookIntervalPriorThisYear"] = 79] = "IsOutlookIntervalPriorThisYear";
            FunctionOperatorType[FunctionOperatorType["IsThisWeek"] = 80] = "IsThisWeek";
            FunctionOperatorType[FunctionOperatorType["IsThisMonth"] = 81] = "IsThisMonth";
            FunctionOperatorType[FunctionOperatorType["IsThisYear"] = 82] = "IsThisYear";
            FunctionOperatorType[FunctionOperatorType["DateDiffTick"] = 83] = "DateDiffTick";
            FunctionOperatorType[FunctionOperatorType["DateDiffSecond"] = 84] = "DateDiffSecond";
            FunctionOperatorType[FunctionOperatorType["DateDiffMilliSecond"] = 85] = "DateDiffMilliSecond";
            FunctionOperatorType[FunctionOperatorType["DateDiffMinute"] = 86] = "DateDiffMinute";
            FunctionOperatorType[FunctionOperatorType["DateDiffHour"] = 87] = "DateDiffHour";
            FunctionOperatorType[FunctionOperatorType["DateDiffDay"] = 88] = "DateDiffDay";
            FunctionOperatorType[FunctionOperatorType["DateDiffMonth"] = 89] = "DateDiffMonth";
            FunctionOperatorType[FunctionOperatorType["DateDiffYear"] = 90] = "DateDiffYear";
            FunctionOperatorType[FunctionOperatorType["GetDate"] = 91] = "GetDate";
            FunctionOperatorType[FunctionOperatorType["GetMilliSecond"] = 92] = "GetMilliSecond";
            FunctionOperatorType[FunctionOperatorType["GetSecond"] = 93] = "GetSecond";
            FunctionOperatorType[FunctionOperatorType["GetMinute"] = 94] = "GetMinute";
            FunctionOperatorType[FunctionOperatorType["GetHour"] = 95] = "GetHour";
            FunctionOperatorType[FunctionOperatorType["GetDay"] = 96] = "GetDay";
            FunctionOperatorType[FunctionOperatorType["GetMonth"] = 97] = "GetMonth";
            FunctionOperatorType[FunctionOperatorType["GetYear"] = 98] = "GetYear";
            FunctionOperatorType[FunctionOperatorType["GetDayOfWeek"] = 99] = "GetDayOfWeek";
            FunctionOperatorType[FunctionOperatorType["GetDayOfYear"] = 100] = "GetDayOfYear";
            FunctionOperatorType[FunctionOperatorType["GetTimeOfDay"] = 101] = "GetTimeOfDay";
            FunctionOperatorType[FunctionOperatorType["Now"] = 102] = "Now";
            FunctionOperatorType[FunctionOperatorType["UtcNow"] = 103] = "UtcNow";
            FunctionOperatorType[FunctionOperatorType["Today"] = 104] = "Today";
            FunctionOperatorType[FunctionOperatorType["AddTimeSpan"] = 105] = "AddTimeSpan";
            FunctionOperatorType[FunctionOperatorType["AddTicks"] = 106] = "AddTicks";
            FunctionOperatorType[FunctionOperatorType["AddMilliSeconds"] = 107] = "AddMilliSeconds";
            FunctionOperatorType[FunctionOperatorType["AddSeconds"] = 108] = "AddSeconds";
            FunctionOperatorType[FunctionOperatorType["AddMinutes"] = 109] = "AddMinutes";
            FunctionOperatorType[FunctionOperatorType["AddHours"] = 110] = "AddHours";
            FunctionOperatorType[FunctionOperatorType["AddDays"] = 111] = "AddDays";
            FunctionOperatorType[FunctionOperatorType["AddMonths"] = 112] = "AddMonths";
            FunctionOperatorType[FunctionOperatorType["AddYears"] = 113] = "AddYears";
            FunctionOperatorType[FunctionOperatorType["OrderDescToken"] = 114] = "OrderDescToken";
        })(Data.FunctionOperatorType || (Data.FunctionOperatorType = {}));
        var FunctionOperatorType = Data.FunctionOperatorType;
        var FunctionOperator = (function (_super) {
            __extends(FunctionOperator, _super);
            function FunctionOperator(operatorType, operands) {
                var _this = this;
                _super.call(this);
                this.toString = function (reverse) {
                    var result = (Data.operatorTokens[_this.displayType] || _this.displayType) + '(' + _this.operands.map(function (operand) {
                        return operand.toString();
                    }).join(", ") + ')';
                    return reverse ? "Not " + result : result;
                };
                this.operands = [];
                this.operatorType = operatorType;
                operands = operands || [new CriteriaOperator()];
                operands.forEach(function (operand) { return _this.operands.push(operand); });
            }
            Object.defineProperty(FunctionOperator.prototype, "displayType", {
                get: function () {
                    return FunctionOperatorType[this.operatorType] || this.operatorType.toString();
                },
                enumerable: true,
                configurable: true
            });
            Object.defineProperty(FunctionOperator.prototype, "enumType", {
                get: function () {
                    return FunctionOperatorType;
                },
                enumerable: true,
                configurable: true
            });
            return FunctionOperator;
        })(CriteriaOperator);
        Data.FunctionOperator = FunctionOperator;
        Data.operatorTokens = {
            "Plus": "+",
            "Minus": "-",
            "Equal": "=",
            "NotEqual": "<>",
            "Greater": ">",
            "Less": "<",
            "LessOrEqual": "<=",
            "GreaterOrEqual": ">="
        };
    })(Data = DevExpress.Data || (DevExpress.Data = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        ko.bindingHandlers["selectable"] = {
            update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
                var values = valueAccessor(), selection = ko.unwrap(values.selection), options = $.extend({ filter: '.dxd-selectable', distance: 1 }, ko.unwrap(values), {
                    selecting: function (event, ui) {
                        var _event = { control: ko.dataFor(ui.selecting), cancel: false, ctrlKey: event.ctrlKey };
                        selection.selecting(_event);
                        if (_event.cancel) {
                            $(ui.selecting).removeClass('ui-selecting');
                        }
                    },
                    start: function (event, ui) {
                        selection.clickHandler(null, event);
                        event.stopPropagation();
                    },
                    unselecting: function (event, ui) {
                        selection.unselecting(ko.dataFor(ui.unselecting));
                    }
                });
                $(element).selectable(options);
            }
        };
        ko.bindingHandlers["updateTop"] = {
            init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
                var value = valueAccessor();
                var updateTop = function () {
                    $(element).css('top', $(element).prev().position().top + $(element).prev().height() + "px");
                };
                var subscription = value.subscribe(function (newVal) {
                    updateTop();
                });
                setTimeout(updateTop, 500);
                ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                    subscription.dispose();
                });
            }
        };
        ko.bindingHandlers["focus"] = {
            init: function (element, valueAccessor) {
                var visible = valueAccessor().on || valueAccessor();
                var subscription = visible.subscribe(function (newVal) {
                    if (newVal) {
                        if (navigator.userAgent.toLowerCase().indexOf('firefox') === -1) {
                            $(element).find(":input").focus();
                        }
                    }
                });
                ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                    subscription.dispose();
                });
            }
        };
        ko.bindingHandlers["draggable"] = {
            init: function (element, valueAccessor) {
                var parent = $($(element).parents(".dx-designer")[0]);
                var values = valueAccessor(), initialScroll = { left: 0, top: 0 }, attachDelta = function (ui) {
                    var containment = values.containment || ".dxrd-ghost-container", $containment = parent.find(containment), $viewport = parent.find(".dxrd-ghost-container");
                    ui["delta"] = { left: $viewport.offset().left - $containment.offset().left, top: $viewport.offset().top - $containment.offset().top };
                    var $viewport = parent.find(".dxrd-viewport");
                    ui["scroll"] = { left: $viewport.scrollLeft() - initialScroll.left, top: $viewport.scrollTop() - initialScroll.top };
                }, options = $.extend({ snap: '.dxrd-drag-snap-line', snapTolerance: Designer.SnapLinesHelper.snapTolerance }, ko.unwrap(values), {
                    start: function (event, ui) {
                        var draggable = $(element).data("ui-draggable");
                        var $viewport = parent.find(".dxrd-viewport");
                        initialScroll.left = $viewport.scrollLeft();
                        initialScroll.top = $viewport.scrollTop();
                        values.startDrag && values.startDrag(ko.dataFor(event.toElement || event.currentTarget));
                    },
                    stop: function (event, ui) {
                        attachDelta(ui);
                        values.stopDrag(ui, ko.dataFor(event.target));
                    },
                    drag: function (event, ui) {
                        if (event.altKey === true) {
                            $(element).draggable("option", "snap", false);
                        }
                        else {
                            $(element).draggable("option", "snap", ".dxrd-drag-snap-line");
                        }
                        attachDelta(ui);
                        values.drag && values.drag(event, ui);
                    },
                    helper: function (event) {
                        $(element).draggable("option", "snap", ".dxrd-drag-snap-line");
                        values.helper && values.helper(ko.dataFor(event["toElement"] || event.currentTarget));
                        var $container = parent.find('.dxrd-drag-helper-source').clone().css({ 'display': 'block' });
                        $container.appendTo(parent.find(options.containment));
                        return $container;
                    }
                });
                options.containment = parent.find(options.containment);
                $(element).draggable(options);
            }
        };
        function num(v) {
            return parseInt(v, 10) || 0;
        }
        ko.bindingHandlers["resizable"] = {
            init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
                var values = valueAccessor(), $selectedNodes = null, options = $.extend({
                    handles: values.handles || "all",
                    ghost: false,
                    stop: function (event, ui) {
                        $selectedNodes.each(function (_, el) {
                            var control = ko.dataFor(el), $el = $(el), curleft = num($el.css("left")), curtop = num($el.css("top"));
                            if (curtop < 0) {
                                curtop = 0;
                            }
                            control.rect({ top: curtop, left: curleft, width: $el.width(), height: $el.height() });
                            $el.removeData("originalPosition");
                            $el.removeData("originalSize");
                        });
                        values.stopped();
                    },
                    start: function () {
                        values.starting();
                        $selectedNodes = $(".dxrd-viewport > .dxrd-focused, .dxrd-selected").filter(":visible");
                        $selectedNodes.each(function (_, el) {
                            var $el = $(el);
                            $el.data("originalPosition", { top: parseFloat($el.css("top")), left: parseFloat($el.css("left")) });
                            $el.data("originalSize", { width: $el.width(), height: $el.height() });
                        });
                    },
                    resize: function (event, ui) {
                        var dw = ui.size.width - ui.originalSize.width, dh = ui.size.height - ui.originalSize.height, dx = ui.position.left - ui.originalPosition.left, dy = ui.position.top - ui.originalPosition.top;
                        if (values.forceResize) {
                            values.forceResize({ size: new Designer.Size(ui.size.width, ui.size.height), delta: { dx: dx, dy: dy, dw: dw, dh: dh } });
                        }
                        $selectedNodes.each(function (key, el) {
                            if (el === event.target)
                                return;
                            var $el = $(el);
                            var originalPosition = $el.data("originalPosition"), originalSize = $el.data("originalSize");
                            $el.css({
                                left: originalPosition.left + dx,
                                top: originalPosition.top + dy,
                                width: originalSize.width + dw,
                                height: originalSize.height + dh
                            });
                        });
                    }
                }, ko.unwrap(values));
                $(element).resizable(options);
            },
            update: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
                var disabled = ko.unwrap(valueAccessor()).disabled || false;
                $(element).resizable("option", "disabled", disabled);
                $(element).resizable("option", "minHeight", ko.unwrap(valueAccessor()).minimumHeight) || 1;
                disabled ? $(element).children(".ui-resizable-handle").css("display", "none") : $(element).children(".ui-resizable-handle").css("display", "block");
            }
        };
        var trackCursorData = "dxd-track-cursor-data";
        var trackCursorClass = "dxd-track-cursor";
        var trackCursorSelector = "." + trackCursorClass;
        ko.bindingHandlers["trackCursor"] = {
            init: function (element, valueAccessor) {
                $(element).addClass(trackCursorClass);
                $(element).data(trackCursorData, valueAccessor);
                ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                    $(element).removeClass(trackCursorClass);
                    $(element).data(trackCursorData, null);
                });
            }
        };
        ko.bindingHandlers["dropTarget"] = {
            init: function (element, valueAccessor) {
                var selection = valueAccessor().dropTarget, subscriptionTarget = valueAccessor().subscriptionTarget || '.dx-designer', body = document.body, docElem = document.documentElement, handler = function (event) {
                    var elements = $(element).find(trackCursorSelector).filter(function (_, trackElement) {
                        var $trackElement = $(trackElement), value = $trackElement.data(trackCursorData)(), bounds = trackElement.getBoundingClientRect(), scrollTop = window.pageYOffset || docElem.scrollTop || body.scrollTop, scrollLeft = window.pageXOffset || docElem.scrollLeft || body.scrollLeft, clientTop = docElem.clientTop || body.clientTop || 0, clientLeft = docElem.clientLeft || body.clientLeft || 0, y = event.pageY - (bounds.top + scrollTop - clientTop), x = event.pageX - (bounds.left + scrollLeft - clientLeft), isOver = x >= 0 && x <= bounds.width && y <= bounds.height && y >= 0;
                        value($.extend(value(), { isOver: isOver, x: x, y: y }));
                        return isOver && !value().isNotDropTarget;
                    });
                    var max = -1, result = elements[0];
                    $.each(elements, function (_, trackElement) {
                        var zIndex = ($(trackElement).zIndex());
                        if (max <= zIndex) {
                            result = trackElement;
                            max = zIndex;
                        }
                    });
                    if (selection && result) {
                        ko.unwrap(selection).dropTarget = ko.dataFor(result);
                    }
                };
                $(element).closest(subscriptionTarget).bind("mousemove", handler);
                ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                    $(element).closest(subscriptionTarget).unbind("mousemove", handler);
                });
            }
        };
        ko.bindingHandlers["styleunit"] = {
            'update': function (element, valueAccessor) {
                var value = ko.utils.unwrapObservable(valueAccessor() || {});
                $.each(value, function (styleName, styleValue) {
                    styleValue = ko.utils.unwrapObservable(styleValue) || 0;
                    element.style[styleName] = styleValue + "px";
                });
            }
        };
        ko.bindingHandlers["templates"] = {
            init: function (element, valueAccessor) {
                var templateHtml = $(valueAccessor()).text(), $templateHtml = $(templateHtml);
                $(element).append($templateHtml);
                return { controlsDescendantBindings: true };
            }
        };
        ko.bindingHandlers["zoom"] = {
            update: function (element, valueAccessor) {
                var value = ko.unwrap(valueAccessor() || {});
                $.each(["-webkit-transform", "-moz-transform", "-ms-transform", "-o-transform", "transform"], function (_, styleName) {
                    element.style[styleName] = "scale(" + (value) + ")";
                });
                $(element).addClass("dxrd-transform-origin-left-top");
            }
        };
        ko.bindingHandlers["service"] = {
            init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
                var value = ko.unwrap(valueAccessor() || {}), findService = function (serviceName) {
                    var context = bindingContext.$parents.filter(function (item) {
                        return item[serviceName] !== undefined;
                    })[0];
                    if (context) {
                        return context[serviceName];
                    }
                    return null;
                }, service = findService(value.name);
                if (service) {
                    var entity = service(viewModel);
                    var childContext = bindingContext.createChildContext(entity.data);
                    ko.renderTemplate(entity.templateName, childContext, {}, element, 'replaceNode');
                }
            }
        };
        ko.bindingHandlers["dxrdAccordion"] = {
            init: function (element, valueAccessor) {
                var options = valueAccessor(), $element = $(element), $accordionContent = $element.find(".dxrd-accordion-content").first(), scrollUpdateCallback = function () {
                    var scrollView = $element.parents(".dx-scrollview").dxScrollView("instance");
                    scrollView && scrollView["update"]();
                };
                $element.find(".dxrd-accordion-header,.dxrd-accordion-button").first().off("dxclick").on("dxclick", function () {
                    var newCollapsed = options.alwaysShow && options.alwaysShow() ? false : !options.collapsed();
                    if (newCollapsed) {
                        $accordionContent.slideUp(options.timeout, function () {
                            scrollUpdateCallback();
                            options.collapsed(true);
                        });
                    }
                    else {
                        $accordionContent.slideDown(options.timeout, function () {
                            scrollUpdateCallback();
                            options.collapsed(false);
                        });
                    }
                });
                options.collapsed() ? $accordionContent.hide() : $accordionContent.show();
            }
        };
        ko.bindingHandlers['lazy'] = {
            init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
                var parsedBindings = valueAccessor();
                $.each(parsedBindings, function (innerBindingKey, innerBindingParameters) {
                    var innerBinding = ko.bindingHandlers[innerBindingKey];
                    setTimeout(function () {
                        var isInitialized = false;
                        ko.computed({
                            read: function () {
                                if (!isInitialized && innerBinding.init) {
                                    innerBinding.init(element, function () {
                                        return innerBindingParameters;
                                    }, allBindings, viewModel, bindingContext);
                                    isInitialized = true;
                                }
                                if (innerBinding.update) {
                                    innerBinding.update(element, function () {
                                        return innerBindingParameters;
                                    }, allBindings, viewModel, bindingContext);
                                }
                            },
                            disposeWhenNodeIsRemoved: element
                        });
                    }, 1);
                });
                return { controlsDescendantBindings: true };
            }
        };
        ko.bindingHandlers["dxLocalizedSelectBox"] = {
            init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
                var options = valueAccessor();
                var prevDisplayExpr = options.displayExpr;
                options.displayExpr = function (value) {
                    if ($.isFunction(prevDisplayExpr)) {
                        return Designer.getLocalization(prevDisplayExpr(value));
                    }
                    else {
                        return value ? Designer.getLocalization(value[prevDisplayExpr]) : value;
                    }
                };
                ko.bindingHandlers["dxSelectBox"].init(element, function () {
                    return options;
                }, allBindings, viewModel, bindingContext);
                return { controlsDescendantBindings: true };
            }
        };
        ko.virtualElements.allowedBindings["lazy"] = true;
        var KeyDownHandlersManager = (function () {
            function KeyDownHandlersManager(targetElement) {
                this._handlers = [];
                this._targetElement = targetElement;
            }
            Object.defineProperty(KeyDownHandlersManager.prototype, "_activeHandler", {
                get: function () {
                    return this._handlers.length > 0 ? this._handlers[this._handlers.length - 1] : null;
                },
                enumerable: true,
                configurable: true
            });
            KeyDownHandlersManager.prototype._removeHandler = function (handler) {
                var index = this._handlers.indexOf(handler);
                if (index < 0)
                    return;
                this._handlers.splice(index, 1);
                if (index === this._handlers.length) {
                    this._targetElement.off("keydown", handler);
                    if (this._activeHandler)
                        this._targetElement.on("keydown", this._activeHandler);
                }
            };
            KeyDownHandlersManager.prototype.bindHandler = function (element, handler) {
                var _this = this;
                if (this._activeHandler)
                    this._targetElement.off("keydown", this._activeHandler);
                this._handlers.push(handler);
                this._targetElement.on("keydown", handler);
                ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                    _this._removeHandler(handler);
                });
            };
            return KeyDownHandlersManager;
        })();
        ko.bindingHandlers["keyDownActions"] = (function () {
            var handlersManager = new KeyDownHandlersManager($(window));
            return {
                init: function (element, valueAccessor) {
                    var actionLists = valueAccessor(), handler = function (e) {
                        actionLists.processShortcut(actionLists.toolbarItems, e);
                    };
                    handlersManager.bindHandler(element, handler);
                }
            };
        })();
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        function copyObservables(from, to) {
            $.each(from || {}, function (name, value) {
                if (ko.isObservable(value)) {
                    to[name](value());
                }
                else {
                    copyObservables(value, to[name]);
                }
            });
        }
        Designer.copyObservables = copyObservables;
        function compareObjects(a, b) {
            var result = a && b && !(a instanceof Array) && !(b instanceof Array);
            result = result && (Object.getOwnPropertyNames(a).length === Object.getOwnPropertyNames(b).length);
            if (result) {
                $.each(a || {}, function (name, value) {
                    if (name.indexOf("_") !== 0 && (typeof value !== "function" || ko.isObservable(value))) {
                        if (ko.isObservable(value)) {
                            result = ko.unwrap(value) === ko.unwrap(b[name]);
                        }
                        else if (value instanceof Array) {
                            if ((b[name] instanceof Array) && value.length === b[name].length) {
                                $.each(value, function (index, item) {
                                    result = compareObjects(item, b[name][index]);
                                    return result;
                                });
                            }
                            else {
                                result = false;
                            }
                        }
                        else if (value instanceof Object) {
                            result = compareObjects(value, b[name]);
                        }
                        else {
                            result = value === b[name];
                        }
                        return result;
                    }
                });
            }
            return result;
        }
        Designer.compareObjects = compareObjects;
        var ElementViewModel = (function () {
            function ElementViewModel(model, parent, serializer) {
                var _this = this;
                this.actions = [];
                this.update = ko.observable(false);
                this.parentModel = ko.observable(parent);
                this.controlType = this.controlType || this.getControlFactory().getControlType(model);
                serializer = serializer || new Designer.DesignerModelSerializer();
                serializer.deserialize(this, model);
                this["displayName"] = ko.computed(function () {
                    var result = _this.name && _this.name();
                    if (!result) {
                        result = "unnamed " + _this.controlType;
                    }
                    return result;
                });
                this.resetValue = function (propertyName) {
                    if (_this[propertyName].resetValue) {
                        _this[propertyName].resetValue();
                    }
                    else {
                        var defaultValue = _this.getPropertyDefaultValue(propertyName);
                        if (ko.isObservable(_this[propertyName])) {
                            _this[propertyName](defaultValue);
                        }
                        else {
                            copyObservables(defaultValue, _this[propertyName]);
                        }
                    }
                };
                this.actions.push({ action: this.resetValue, title: "Reset", visible: this.isResettableProperty });
            }
            ElementViewModel.prototype.getPropertyDefaultValue = function (propertyName) {
                var info = this.getPropertyInfo(propertyName);
                return ko.unwrap(info && new Designer.DesignerModelSerializer().deserializeProperty(info, {}));
            };
            ElementViewModel.prototype.getPropertyInfo = function (propertyName) {
                return this.getInfo().filter(function (info) {
                    return info.propertyName === propertyName;
                })[0];
            };
            ElementViewModel.prototype.getInfo = function () {
                return this.getControlFactory().controlsMap[this.controlType].info;
            };
            ElementViewModel.prototype.createControl = function (model, serializer) {
                return this.getControlFactory().createControl(model, this, serializer);
            };
            ElementViewModel.prototype.getNearestParent = function (target) {
                return target.getMetaData().isContainer ? target : target.parentModel();
            };
            ElementViewModel.prototype.getControlInfo = function () {
                return this.getControlFactory().controlsMap[this.controlType || "Unknown"];
            };
            ElementViewModel.prototype.getMetaData = function () {
                var controlType = this.controlType ? this.controlType : "Unknown", data = this.getControlFactory().controlsMap[controlType];
                return {
                    isContainer: data.isContainer || false,
                    isCopyDeny: data.isCopyDeny || false,
                    isDeleteDeny: data.isDeleteDeny || false,
                    elementActionsTypes: data.elementActionsTypes
                };
            };
            ElementViewModel.prototype._hasModifiedValue = function (name) {
                return this["_" + name] && this["_" + name]() && this.isPropertyModified(name);
            };
            ElementViewModel.prototype.createChild = function (info) {
                var newControl = this.getControlFactory().createControl(info, this);
                this.addChild(newControl);
                return newControl;
            };
            ElementViewModel.prototype.removeChild = function (control) {
                if (this["controls"]) {
                    this["controls"].splice(this["controls"]().indexOf(control), 1);
                }
            };
            ElementViewModel.prototype.addChild = function (control) {
                if (this["controls"] && this["controls"]().indexOf(control) === -1) {
                    control.parentModel(this);
                    this["controls"].push(control);
                }
            };
            ElementViewModel.prototype.isPropertyDisabled = function (name) {
                return false;
            };
            ElementViewModel.prototype.isPropertyModified = function (name) {
                var needName = this["_" + name] ? "_" + name : name;
                if (this[needName].isPropertyModified) {
                    return this[needName].isPropertyModified();
                }
                else if (this[needName].isEmpty) {
                    return !this[needName].isEmpty();
                }
                else {
                    var defaultValue = this.getPropertyDefaultValue(name), propertyValue = ko.unwrap(this[needName]);
                    if (defaultValue instanceof Object) {
                        return !compareObjects(defaultValue, propertyValue);
                    }
                    else {
                        return defaultValue !== propertyValue;
                    }
                }
            };
            ElementViewModel.prototype.getControlFactory = function () {
                throw Error("Virtual method getControlFactory");
            };
            ElementViewModel.prototype.isResettableProperty = function (propertyName) {
                return ["name", "size", "location"].indexOf(propertyName) === -1;
            };
            Object.defineProperty(ElementViewModel.prototype, "root", {
                get: function () {
                    var root = this;
                    while (root && root.parentModel()) {
                        root = root.parentModel();
                    }
                    return root;
                },
                enumerable: true,
                configurable: true
            });
            return ElementViewModel;
        })();
        Designer.ElementViewModel = ElementViewModel;
        ;
        function findSurface(viewModel) {
            return viewModel["surface"];
        }
        Designer.findSurface = findSurface;
        function unitsToPixel(val, measureUnit, zoom) {
            if (zoom === void 0) { zoom = 1; }
            var result;
            if (measureUnit === "Test" || measureUnit === "Pixels") {
                result = 1 * val;
            }
            else if (measureUnit === "TenthsOfAMillimeter") {
                result = val * 3.78 / 10;
            }
            else {
                result = val * 96 / 100;
            }
            result = result * (zoom);
            return (Math.round(result * 100) / 100);
        }
        Designer.unitsToPixel = unitsToPixel;
        function pixelToUnits(val, measureUnit, zoom) {
            var result;
            if (measureUnit === "Test" || measureUnit === "Pixels") {
                result = 1 * val;
            }
            else if (measureUnit === "TenthsOfAMillimeter") {
                result = val / 3.78 * 10;
            }
            else {
                result = val / 96 * 100;
            }
            result = result / (zoom);
            return Math.round(result * 100) / 100;
        }
        Designer.pixelToUnits = pixelToUnits;
        var HoverInfo = (function () {
            function HoverInfo() {
                this._x = 0;
                this._y = 0;
                this.isOver = false;
            }
            Object.defineProperty(HoverInfo.prototype, "x", {
                get: function () {
                    return this._x;
                },
                set: function (newX) {
                    this._x = newX;
                },
                enumerable: true,
                configurable: true
            });
            Object.defineProperty(HoverInfo.prototype, "y", {
                get: function () {
                    return this._y;
                },
                set: function (newY) {
                    this._y = newY;
                },
                enumerable: true,
                configurable: true
            });
            return HoverInfo;
        })();
        Designer.HoverInfo = HoverInfo;
        function createObservableArrayMapCollection(elementModels, target, createItem) {
            elementModels.peek().forEach(function (item) {
                var surface = createItem(item);
                target.push(surface);
            });
            elementModels.subscribe(function (args) {
                var startIndex = target().length, deleteCount = 0, valuesToAdd = [];
                args.forEach(function (changeSet) {
                    if (changeSet.status === "deleted") {
                        deleteCount++;
                        if (changeSet.index < startIndex) {
                            startIndex = changeSet.index;
                        }
                    }
                });
                args.forEach(function (changeSet) {
                    if (changeSet.status === "added") {
                        if (changeSet.index < startIndex) {
                            startIndex = changeSet.index;
                        }
                        valuesToAdd.push(createItem(changeSet.value));
                    }
                });
                target.splice.apply(target, [startIndex, deleteCount].concat(valuesToAdd));
            }, null, "arrayChange");
        }
        Designer.createObservableArrayMapCollection = createObservableArrayMapCollection;
        var SurfaceElementArea = (function () {
            function SurfaceElementArea(control, context, unitProperties) {
                var _this = this;
                this._createSurface = function (item) {
                    return item["surface"] || new (item.getControlFactory()).controlsMap[item.controlType].surfaceType(item, _this._context);
                };
                this._control = control;
                this._context = context;
                control["surface"] = this;
                if (this._context) {
                    Designer.createUnitProperties(control, this, unitProperties, this._context.measureUnit, this._context.zoom);
                }
                this["_x"] = this["_x"] || ko.observable(0);
                this["_y"] = this["_y"] || ko.observable(0);
                this["_width"] = this["_width"] || ko.observable(0);
                this["_height"] = this["_height"] || ko.observable(0);
                var x = this["_x"], y = this["_y"], width = this["_width"], height = this["_height"];
                this["position"] = {
                    top: y,
                    left: x,
                    width: width,
                    height: height,
                    lineHeight: height
                };
                var _rect = ko.observable();
                ko.computed(function () {
                    if (!_this._control.update()) {
                        _rect({ top: y(), left: x(), right: x() + width(), bottom: y() + height(), width: width(), height: height() });
                    }
                });
                this.rect = ko.computed({
                    read: function () {
                        return _rect();
                    },
                    write: function (newRect) {
                        _this._control.update(true);
                        try {
                            if (newRect.left !== undefined) {
                                x(newRect.left);
                            }
                            if (newRect.top !== undefined) {
                                y(newRect.top);
                            }
                            if (newRect.right !== undefined && newRect.left === undefined) {
                                width(newRect.right - x());
                            }
                            if (newRect.bottom !== undefined && newRect.top === undefined) {
                                height(newRect.bottom - y());
                            }
                            if (newRect.right !== undefined && newRect.left !== undefined) {
                                width(newRect.right - newRect.left);
                            }
                            if (newRect.bottom !== undefined && newRect.top !== undefined) {
                                height(newRect.bottom - newRect.top);
                            }
                            if (newRect.width !== undefined) {
                                width(newRect.width);
                            }
                            if (newRect.height !== undefined) {
                                height(newRect.height);
                            }
                        }
                        finally {
                            _this._control.update(false);
                        }
                    }
                });
            }
            SurfaceElementArea.prototype.getRoot = function () {
                return this._context;
            };
            SurfaceElementArea.prototype.getControlModel = function () {
                return this._control;
            };
            return SurfaceElementArea;
        })();
        Designer.SurfaceElementArea = SurfaceElementArea;
        var SurfaceElementBase = (function (_super) {
            __extends(SurfaceElementBase, _super);
            function SurfaceElementBase(control, context, unitProperties) {
                var _this = this;
                _super.call(this, control, context, unitProperties);
                this._countSelectedChildren = ko.observable(0);
                this.focused = ko.observable(false);
                this.selected = ko.observable(false);
                this.underCursor = ko.observable(new HoverInfo());
                this.allowMultiselect = true;
                this.absolutePosition = new Designer.Point(0, 0);
                this.snapLines = ko.observableArray([]);
                this.getControlModel = function () {
                    return control;
                };
                this.cssCalculator = new Designer.CssCalculator(control);
                if (this._getChildrenHolderName() && control[this._getChildrenHolderName()]) {
                    var collection = ko.observableArray();
                    createObservableArrayMapCollection(control[this._getChildrenHolderName()], collection, this._createSurface);
                    this[this._getChildrenHolderName()] = collection;
                    this.isSelected = ko.computed(function () {
                        if (!(_this.focused() || _this.selected())) {
                            return collection().some(function (item) {
                                return item.isSelected();
                            });
                        }
                        return true;
                    });
                }
                else {
                    this.isSelected = ko.computed(function () {
                        return _this.focused() || _this.selected();
                    });
                }
                this.css = ko.computed(function () {
                    return $.extend({}, _this.cssCalculator.fontCss(), _this.cssCalculator.foreColorCss(), _this.cssCalculator.backGroundCss(), _this.cssCalculator.textAlignmentCss());
                });
                this.contentCss = ko.computed(function () {
                    return $.extend({}, _this.cssCalculator.fontCss(), _this.cssCalculator.foreColorCss(), _this.cssCalculator.textAlignmentCss(), _this.cssCalculator.angle(), _this.cssCalculator.paddingsCss());
                });
                ko.computed(function () {
                    _this.updateAbsolutePosition();
                });
                this.absoluteRect = ko.computed(function () {
                    var controlRect = _this.rect(), absolutePositionY = _this.absolutePosition.y();
                    return { top: absolutePositionY, left: controlRect.left, right: controlRect.right, bottom: absolutePositionY + controlRect.height, width: controlRect.width, height: controlRect.height };
                });
            }
            Object.defineProperty(SurfaceElementBase.prototype, "parent", {
                get: function () {
                    return this.getControlModel().parentModel() && this.getControlModel().parentModel().surface;
                },
                enumerable: true,
                configurable: true
            });
            SurfaceElementBase.prototype.checkParent = function (surfaceParent) {
                return this.parent === surfaceParent;
            };
            SurfaceElementBase.prototype._getChildrenHolderName = function () {
                return "controls";
            };
            SurfaceElementBase.prototype.getChildrenCollection = function () {
                return this._getChildrenHolderName() && this[this._getChildrenHolderName()] || ko.observableArray([]);
            };
            SurfaceElementBase.prototype.updateAbsolutePosition = function () {
                var _this = this;
                if (this.parent && this.parent.absolutePosition) {
                    var parentX = this.parent.absolutePosition.x(), parentY = this.parent.absolutePosition.y(), newX = parentX + this.rect().left, newY = parentY + this.rect().top;
                    this.absolutePosition.x(newX);
                    this.absolutePosition.y(newY);
                }
                else {
                    this.absolutePosition.x(0);
                    this.absolutePosition.y(0);
                }
                if (this.snapLines().length === 0 && this["isSnapTarget"]) {
                    var maxHeight = ko.computed(function () {
                        return _this._context["effectiveHeight"] && _this._context["effectiveHeight"]() || _this._context.pageHeight();
                    }), maxWidth = ko.computed(function () {
                        return _this._context.pageWidth() - _this._context.margins.left();
                    });
                    this.snapLines.push(new Designer.SnapLine(this.absolutePosition.x, this.absolutePosition.y, true, maxHeight, maxWidth));
                    this.snapLines.push(new Designer.SnapLine(ko.computed(function () {
                        return _this.absolutePosition.x() + _this.rect().width;
                    }), this.absolutePosition.y, true, maxHeight, maxWidth));
                    this.snapLines.push(new Designer.SnapLine(this.absolutePosition.x, this.absolutePosition.y, false, maxHeight, maxWidth));
                    this.snapLines.push(new Designer.SnapLine(this.absolutePosition.x, ko.computed(function () {
                        return _this.absolutePosition.y() + _this.rect().height;
                    }), false, maxHeight, maxWidth));
                }
            };
            return SurfaceElementBase;
        })(SurfaceElementArea);
        Designer.SurfaceElementBase = SurfaceElementBase;
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        Designer.localization_values = {
            'Page Header': 'dx_reportDesigner_DevExpress.XtraReports.UI.PageHeaderBand',
            'Max Nesting Level': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRTableOfContents.MaxNestingLevel',
            'Null Value Text': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControl.NullValueText',
            'Process Null Values': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRPictureBox.ProcessNullValues',
            'Actual Value': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRGauge.ActualValue',
            'Custom Draw a Series Point': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRChartScripts.OnCustomDrawSeriesPoint',
            'Row Count for Preview': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReport.PreviewRowCount',
            'Synchronize Bounds': 'dx_reportDesigner_DevExpress.XtraReports.UI.WinControlContainer.SyncBounds',
            'Fill Color': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRShape.FillColor',
            'Size Changed': 'dx_reportDesigner_DevExpress.XtraReports.UI.SubreportBaseScripts.OnSizeChanged',
            'Text': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRTableOfContentsTitle.Text',
            'Top': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRDockStyle.Top',
            'Format': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRPageInfo.Format',
            'Group Fields': 'dx_reportDesigner_DevExpress.XtraReports.UI.GroupHeaderBand.GroupFields',
            'Use Font': 'dx_reportDesigner_DevExpress.XtraReports.UI.StyleUsing.UseFont',
            'Custom Total Cell Style': 'dx_reportDesigner_DevExpress.XtraReports.UI.PivotGrid.XRPivotGridStyles.CustomTotalCellStyle',
            'Process Duplicates Target': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRLabel.ProcessDuplicatesTarget',
            'Lines': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRRichText.Lines',
            'Script Security Permissions': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReport.ScriptSecurityPermissions',
            'Sorting Summary Get Result': 'dx_reportDesigner_DevExpress.XtraReports.UI.GroupHeaderBandScripts.OnSortingSummaryGetResult',
            'Use Text Alignment': 'dx_reportDesigner_DevExpress.XtraReports.UI.StylePriority.UseTextAlignment',
            'Summary Get Result': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRLabelScripts.OnSummaryGetResult',
            'Keep Together': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControl.KeepTogether',
            'Field Value Style': 'dx_reportDesigner_DevExpress.XtraReports.UI.PivotGrid.XRPivotGridStyles.FieldValueStyle',
            'Padding': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRBarCode.PaddingInfo',
            'Data Adapter': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRPivotGrid.DataAdapter',
            'Data Source': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRSparkline.DataSource',
            'Palette Name': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRChart.PaletteName',
            'Report Print Options': 'dx_reportDesigner_DevExpress.XtraReports.UI.ReportPrintOptions',
            'Filter Separator': 'dx_reportDesigner_DevExpress.XtraReports.UI.PivotGrid.XRPivotGridAppearances.FilterSeparator',
            'Column Width': 'dx_reportDesigner_DevExpress.XtraReports.UI.MultiColumn.ColumnWidth',
            'Column Count': 'dx_reportDesigner_DevExpress.XtraReports.UI.MultiColumn.ColumnCount',
            'Show Text': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRBarCode.ShowText',
            'Line Width': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRShape.LineWidth',
            'Parameters': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReport.Parameters',
            'Location': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControl.Location',
            'Group Union': 'dx_reportDesigner_DevExpress.XtraReports.UI.GroupHeaderBand.GroupUnion',
            'Bottom Margin': 'dx_reportDesigner_DevExpress.XtraReports.UI.BottomMarginBand',
            'Merge': 'dx_reportDesigner_DevExpress.XtraReports.UI.ProcessDuplicatesMode.Merge',
            'Leave': 'dx_reportDesigner_DevExpress.XtraReports.UI.ProcessDuplicatesMode.Leave',
            'Process Duplicates Mode': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRBarCode.ProcessDuplicatesMode',
            'Empty Chart Text': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRChart.EmptyChartText',
            'Anchor Horizontally': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControl.AnchorHorizontal',
            'Repeat Every Page': 'dx_reportDesigner_DevExpress.XtraReports.UI.GroupBand.RepeatEveryPage',
            'Multiline': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRRichTextBoxBase.Multiline',
            'Visual Basic': 'dx_reportDesigner_DevExpress.XtraReports.ScriptLanguage.VisualBasic',
            'Average': 'dx_reportDesigner_DevExpress.XtraReports.UI.SortingSummaryFunction.Avg',
            'Min': 'dx_reportDesigner_DevExpress.XtraReports.UI.SortingSummaryFunction.Min',
            'Max': 'dx_reportDesigner_DevExpress.XtraReports.UI.SortingSummaryFunction.Max',
            'Variance': 'dx_reportDesigner_DevExpress.XtraReports.UI.SortingSummaryFunction.Var',
            'Sum': 'dx_reportDesigner_DevExpress.XtraReports.UI.SortingSummaryFunction.Sum',
            'Direction': 'dx_reportDesigner_DevExpress.XtraReports.UI.MultiColumn.Direction',
            'Group Header': 'dx_reportDesigner_DevExpress.XtraReports.UI.GroupHeaderBand',
            'Font': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRTableOfContentsLevelBase.Font',
            'Group': 'dx_reportDesigner_DevExpress.XtraReports.UI.SummaryRunning.Group',
            'Watermark': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReport.Watermark',
            'Even Style': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControl.XRControlStyles.EvenStyle',
            'Show Print Status Dialog': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReport.ShowPrintStatusDialog',
            'Data Member': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRSparkline.DataMember',
            'Function': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRGroupSortingSummary.Function',
            'Condition': 'dx_reportDesigner_DevExpress.XtraReports.UI.FormattingRule.Condition',
            'Scripts': 'dx_reportDesigner_DevExpress.XtraReports.UI.Band.Scripts',
            'OLAP Connection String': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRPivotGrid.OLAPConnectionString',
            'Bound Data Changed': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRChartScripts.OnBoundDataChanged',
            'Snap Line Margin': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControl.SnapLineMargin',
            'Level': 'dx_reportDesigner_DevExpress.XtraReports.UI.DetailReportBand.Level',
            'Export Options': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReport.ExportOptions',
            'Value Range': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRSparkline.ValueRange',
            'Average (Distinct)': 'dx_reportDesigner_DevExpress.XtraReports.UI.SummaryFunc.DAvg',
            'Summary (Distinct)': 'dx_reportDesigner_DevExpress.XtraReports.UI.SummaryFunc.DSum',
            'Variance (Distinct)': 'dx_reportDesigner_DevExpress.XtraReports.UI.SummaryFunc.DVar',
            'Population Variance': 'dx_reportDesigner_DevExpress.XtraReports.UI.SummaryFunc.VarP',
            'Evaluate Binding': 'dx_reportDesigner_DevExpress.XtraReports.UI.SubreportBaseScripts.OnEvaluateBinding',
            'Mouse Up in Preview': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControlEvents.OnPreviewMouseUp',
            'Maximum': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRGauge.Maximum',
            'Custom Total Cell': 'dx_reportDesigner_DevExpress.XtraReports.UI.PivotGrid.XRPivotGridAppearances.CustomTotalCell',
            'Detect URLs': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRRichTextBoxBase.DetectUrls',
            'Series Template': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRChart.SeriesTemplate',
            'Pie Series Point Exploded': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRChartScripts.OnPieSeriesPointExploded',
            'Custom': 'dx_reportDesigner_DevExpress.XtraReports.UI.SortingSummaryFunction.Custom',
            'Palette\'s Base Color Number': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRChart.PaletteBaseColorNumber',
            'Percentage': 'dx_reportDesigner_DevExpress.XtraReports.UI.SummaryFunc.Percentage',
            'Bookmark Duplicate Suppress': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReport.BookmarkDuplicateSuppress',
            'Custom Summary': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRPivotGridScripts.OnCustomSummary',
            'Panel': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRPanel',
            'Shape': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRShape',
            'Using Style': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControlStyle.StyleUsing',
            'Table': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRTable',
            'Start Point': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRCrossBandControl.StartPoint',
            'Chart': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRChart',
            'Gauge': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRGauge',
            'Label': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRLabel',
            'Use Foreground Color': 'dx_reportDesigner_DevExpress.XtraReports.UI.StylePriority.UseForeColor',
            'Sorting Summary': 'dx_reportDesigner_DevExpress.XtraReports.UI.GroupHeaderBand.SortingSummary',
            'Running Band': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRPageInfo.RunningBand',
            'Before the Band': 'dx_reportDesigner_DevExpress.XtraReports.UI.PageBreak.BeforeBand',
            'Double-Click in Preview': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControlEvents.OnPreviewDoubleClick',
            'Field Name': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRGroupSortingSummary.FieldName',
            'Request Parameters': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReport.RequestParameters',
            'Count (Distinct)': 'dx_reportDesigner_DevExpress.XtraReports.UI.SortingSummaryFunction.DCount',
            'Field Value Grand Total Style': 'dx_reportDesigner_DevExpress.XtraReports.UI.PivotGrid.XRPivotGridStyles.FieldValueGrandTotalStyle',
            'Table Row': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRTableRow',
            'Line Style': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRCrossBandLine.LineStyle',
            'Data Bindings': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControl.DataBindings',
            'Parent Changed': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControlEvents.OnParentChanged',
            'With Last Detail': 'dx_reportDesigner_DevExpress.XtraReports.UI.GroupFooterUnion.WithLastDetail',
            'Custom Unbound Field Data': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRPivotGridScripts.OnCustomUnboundFieldData',
            'Custom Group Interval': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRPivotGridScripts.OnCustomGroupInterval',
            'Page Information': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRPageInfo.PageInfo',
            'Checked': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRCheckBox.Checked',
            'Bookmark': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControl.Bookmark',
            'Bitmap': 'dx_reportDesigner_DevExpress.XtraReports.UI.WinControlImageType.Bitmap',
            'Height': 'dx_reportDesigner_DevExpress.XtraReports.UI.Band.Height',
            'Minimum': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRGauge.Minimum',
            'After the Band, Except for the Last Entry': 'dx_reportDesigner_DevExpress.XtraReports.UI.PageBreak.AfterBandExceptLastEntry',
            'Indent': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRTableOfContentsLevel.Indent',
            'Sorting Summary Reset': 'dx_reportDesigner_DevExpress.XtraReports.UI.GroupHeaderBandScripts.OnSortingSummaryReset',
            'Value': 'dx_reportDesigner_DevExpress.XtraReports.UI.ProcessDuplicatesTarget.Value',
            'Show Printing Warnings': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReport.ShowPrintingWarnings',
            'Multi-Column Options': 'dx_reportDesigner_DevExpress.XtraReports.UI.MultiColumn',
            'Draw the Watermark': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReport.DrawWatermark',
            'Median': 'dx_reportDesigner_DevExpress.XtraReports.UI.SortingSummaryFunction.Median',
            'Background Color': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControl.BackColor',
            'None': 'dx_reportDesigner_DevExpress.XtraReports.UI.MultiColumnMode.None',
            'Group Field': 'dx_reportDesigner_DevExpress.XtraReports.UI.GroupField',
            'Glyph Alignment': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRCheckBox.GlyphAlignment',
            'Annotations': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRChart.Annotations',
            'Sparkline': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRSparkline',
            'JScript .NET': 'dx_reportDesigner_DevExpress.XtraReports.ScriptLanguage.JScript',
            'Table Cell': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRTableCell',
            'Snap to Grid': 'dx_reportDesigner_DevExpress.XtraReports.UI.SnappingMode.SnapToGrid',
            'Style Priority': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControl.StylePriority',
            'Rich Text': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRRichTextBase.RtfText',
            'Cross-band Box': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRCrossBandBox',
            'Line Direction': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRLine.LineDirection',
            'Border Width': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControl.BorderWidth',
            'Row Span': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRTableCell.RowSpan',
            'Border Color': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControl.BorderColor',
            'Text Alignment': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControl.TextAlignment',
            'Group Sorting Summary': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRGroupSortingSummary',
            'Filter String': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReportBase.FilterString',
            'Custom Cell Value': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRPivotGridScripts.OnCustomCellValue',
            'Printable Component Container': 'dx_reportDesigner_DevExpress.XtraReports.UI.PrintableComponentContainer',
            'Locked in the End-User Designer': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControl.LockedInUserDesigner',
            'Page Footer': 'dx_reportDesigner_DevExpress.XtraReports.UI.PageFooterBand',
            'Border Dash Style': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControlStyle.BorderDashStyle',
            'Formatting': 'dx_reportDesigner_DevExpress.XtraReports.UI.FormattingRule.Formatting',
            'Standard Deviation': 'dx_reportDesigner_DevExpress.XtraReports.UI.SortingSummaryFunction.StdDev',
            'Format String': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRBinding.FormatString',
            'Back Color': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRTableOfContentsLevelBase.BackColor',
            'Suppress': 'dx_reportDesigner_DevExpress.XtraReports.UI.ValueSuppressType.Suppress',
            'Window Control Options': 'dx_reportDesigner_DevExpress.XtraReports.UI.WindowControlOptions',
            'Margins': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReport.Margins',
            'Using Settings of the Default Printer': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReport.DefaultPrinterSettingsUsing',
            'Borders': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControl.Borders',
            'Style Sheet': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReport.StyleSheet',
            'End Point': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRCrossBandControl.EndPoint',
            'Fill': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRDockStyle.Fill',
            'Annotation Repository': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRChart.AnnotationRepository',
            'Page Width': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReport.PageWidth',
            'Series Data Member': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRChart.SeriesDataMember',
            'Check Box': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRCheckBox',
            'Use Padding': 'dx_reportDesigner_DevExpress.XtraReports.UI.StylePriority.UsePadding',
            'View Theme': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRGauge.ViewTheme',
            'View Style': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRGauge.ViewStyle',
            'Page Color': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReport.PageColor',
            'With First Detail': 'dx_reportDesigner_DevExpress.XtraReports.UI.GroupUnion.WithFirstDetail',
            'Navigation URL': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControl.NavigateUrl',
            'Report Header': 'dx_reportDesigner_DevExpress.XtraReports.UI.ReportHeaderBand',
            'Custom Field Value Cells': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRPivotGridScripts.OnCustomFieldValueCells',
            'Styles': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRPivotGrid.Styles',
            'Standard Deviation (Distinct)': 'dx_reportDesigner_DevExpress.XtraReports.UI.SortingSummaryFunction.DStdDev',
            'Series Name Template': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRChart.SeriesNameTemplate',
            'Standard Population Deviation (Distinct)': 'dx_reportDesigner_DevExpress.XtraReports.UI.SummaryFunc.DStdDevP',
            'XML Data Path': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReportBase.XmlDataPath',
            'Options Chart Data Source': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRPivotGrid.OptionsChartDataSource',
            'Anchor Vertically': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControl.AnchorVertical',
            'Report Source Url': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRSubreport.ReportSourceUrl',
            'Data Source\'s Schema': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReport.DataSourceSchema',
            'Zip Code': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRZipCode',
            'Angle': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRLabel.Angle',
            'Process Duplicates': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRLabel.ProcessDuplicates',
            'Print Field Value': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRPivotGridScripts.OnPrintFieldValue',
            'Small Chart Text': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRChart.SmallChartText',
            'Display Name': 'dx_reportDesigner_DevExpress.XtraReports.UI.CalculatedField.DisplayName',
            'Custom Draw an Axis Label': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRChartScripts.OnCustomDrawAxisLabel',
            'Detail Count at Design Time': 'dx_reportDesigner_DevExpress.XtraReports.UI.ReportPrintOptions.DetailCountAtDesignTime',
            'Using Parent Style': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControl.ParentStyleUsing',
            'Drawing Method': 'dx_reportDesigner_DevExpress.XtraReports.UI.WinControlContainer.DrawMethod',
            'As Image': 'dx_reportDesigner_DevExpress.XtraReports.UI.WinControlPrintMode.AsImage',
            'Header Group Line Style': 'dx_reportDesigner_DevExpress.XtraReports.UI.PivotGrid.XRPivotGridStyles.HeaderGroupLineStyle',
            'Start Band': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRCrossBandControl.StartBand',
            'Label Scripts': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRLabelScripts',
            'Image': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRPictureBox.Image',
            'Suppress and Shrink': 'dx_reportDesigner_DevExpress.XtraReports.UI.ProcessDuplicatesMode.SuppressAndShrink',
            'Image Type': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRChart.ImageType',
            'Row Changed': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRSummaryEvents.OnRowChanged',
            'Report Source': 'dx_reportDesigner_DevExpress.XtraReports.UI.SubreportBase.ReportSource',
            'Default': 'dx_reportDesigner_DevExpress.XtraReports.UI.WinControlPrintMode.Default',
            'Paper Kind': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReport.PaperKind',
            'Paper Name': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReport.PaperName',
            'Text Horizontal Alignment': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRAppearanceObject.TextHorizontalAlignment',
            'Snapping Mode': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReport.SnappingMode',
            'Summary Row Changed': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRLabelScripts.OnSummaryRowChanged',
            'Image URL': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRPictureBox.ImageUrl',
            'Size': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControl.Size',
            'Name': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControl.Name',
            'Control Style': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControlStyle',
            'Foreground Color': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControlStyle.ForeColor',
            'Band Level Changed': 'dx_reportDesigner_DevExpress.XtraReports.UI.GroupBandScripts.OnBandLevelChanged',
            'Sort Fields': 'dx_reportDesigner_DevExpress.XtraReports.UI.DetailBand.SortFields',
            'Custom Field Sort': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRPivotGridScripts.OnCustomFieldSort',
            'Data Source Demanded': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReportScripts.OnDataSourceDemanded',
            'End Band': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRCrossBandControl.EndBand',
            'Sorting Summary Row Changed': 'dx_reportDesigner_DevExpress.XtraReports.UI.GroupHeaderBandScripts.OnSortingSummaryRowChanged',
            'Cell Style': 'dx_reportDesigner_DevExpress.XtraReports.UI.PivotGrid.XRPivotGridStyles.CellStyle',
            'Print at Bottom': 'dx_reportDesigner_DevExpress.XtraReports.UI.ReportFooterBand.PrintAtBottom',
            'Tag': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControl.Tag',
            'Show Print Margins Warning': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReport.ShowPrintMarginsWarning',
            'Get a Result': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRSummaryEvents.OnGetResult',
            'Detail Count': 'dx_reportDesigner_DevExpress.XtraReports.UI.ReportPrintOptions.DetailCount',
            'Sizing': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRPictureBox.Sizing',
            'Calculated Fields': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReport.CalculatedFields',
            'Whole Page': 'dx_reportDesigner_DevExpress.XtraReports.UI.GroupUnion.WholePage',
            'Parameters Submitted': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReportScripts.OnParametersRequestSubmit',
            'Use Column Width': 'dx_reportDesigner_DevExpress.XtraReports.UI.MultiColumnMode.UseColumnWidth',
            'Use Border Color': 'dx_reportDesigner_DevExpress.XtraReports.UI.StyleUsing.UseBorderColor',
            'Total Cell Style': 'dx_reportDesigner_DevExpress.XtraReports.UI.PivotGrid.XRPivotGridStyles.TotalCellStyle',
            'Print Progress': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReportScripts.OnPrintProgress',
            'Use Column Count': 'dx_reportDesigner_DevExpress.XtraReports.UI.MultiColumnMode.UseColumnCount',
            'Use Border Width': 'dx_reportDesigner_DevExpress.XtraReports.UI.StyleUsing.UseBorderWidth',
            'Axis Scale Changed': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRChartScripts.OnAxisScaleChanged',
            'View': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRSparkline.View',
            'Can Shrink': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControl.CanShrink',
            'Detail Count when Data Source is Empty': 'dx_reportDesigner_DevExpress.XtraReports.UI.ReportPrintOptions.DetailCountOnEmptyDataSource',
            'Formatting Rules': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControl.FormattingRules',
            'Before the Band, Except for the First Entry': 'dx_reportDesigner_DevExpress.XtraReports.UI.PageBreak.BeforeBandExceptFirstEntry',
            'Chart Scripts': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRChartScripts',
            'Draw the Grid': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReport.DrawGrid',
            'Band Scripts': 'dx_reportDesigner_DevExpress.XtraReports.UI.BandScripts',
            'Group Footer': 'dx_reportDesigner_DevExpress.XtraReports.UI.GroupFooterBand',
            'Background Image': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRChart.BackImage',
            'Trimming': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRAppearanceObject.Trimming',
            'Ignore Null Values': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRGroupSortingSummary.IgnoreNullValues',
            'Script References': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReport.ScriptReferences',
            'Ascending': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRColumnSortOrder.Ascending',
            'Xlsx Format String': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControl.XlsxFormatString',
            'Fields': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRPivotGrid.Fields',
            'Binding': 'dx_reportDesigner_DevExpress.XtraReports.Design.DataBinding.Binding',
            'First Down, then Across': 'dx_reportDesigner_DevExpress.XtraReports.UI.ColumnDirection.DownThenAcross',
            'Print Options': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRPivotGrid.OptionsPrint',
            'Before Print': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControlEvents.OnBeforePrint',
            'Sort Order': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRGroupSortingSummary.SortOrder',
            'Use WMPaint Recursively': 'dx_reportDesigner_DevExpress.XtraReports.UI.WinControlDrawMethod.UseWMPaintRecursive',
            'Page Info': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRPageInfo',
            'Field Value Display Text': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRPivotGridScripts.OnFieldValueDisplayText',
            'Clip Content': 'dx_reportDesigner_DevExpress.XtraReports.UI.PrintableComponentContainer.ClipContent',
            'Appearance Name': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRChart.AppearanceName',
            'Custom Draw a Series': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRChartScripts.OnCustomDrawSeries',
            'Custom Column Width': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRPivotGridScripts.OnCustomColumnWidth',
            'Mode': 'dx_reportDesigner_DevExpress.XtraReports.UI.MultiColumn.Mode',
            'Axis Whole Range Changed': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRChartScripts.OnAxisWholeRangeChanged',
            'Show Designer\'s Hints': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReport.ShowDesignerHints',
            'Axis Visual Range Changed': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRChartScripts.OnAxisVisualRangeChanged',
            'Fill Empty Space': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReportScripts.OnFillEmptySpace',
            'Get a Value': 'dx_reportDesigner_DevExpress.XtraReports.UI.CalculatedFieldScripts.OnGetValue',
            'Line Scripts': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRLineScripts',
            'Bands': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReportBase.Bands',
            'Symbology': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRBarCode.Symbology',
            'Grand Total Cell Style': 'dx_reportDesigner_DevExpress.XtraReports.UI.PivotGrid.XRPivotGridStyles.GrandTotalCellStyle',
            'Control Scripts': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControlScripts',
            'Cross-band Line': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRCrossBandLine',
            'Location Changed': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControlEvents.OnLocationChanged',
            'Designer Options': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReport.DesignerOptions',
            'Series': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRChart.Series',
            'Use Borders': 'dx_reportDesigner_DevExpress.XtraReports.UI.StyleUsing.UseBorders',
            'Width': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControl.Width',
            'Image Alignment': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRPictureBox.ImageAlignment',
            'Filter Separator Style': 'dx_reportDesigner_DevExpress.XtraReports.UI.PivotGrid.XRPivotGridStyles.FilterSeparatorStyle',
            'Diagram': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRChart.Diagram',
            'Script Language': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReport.ScriptLanguage',
            'Word Wrap': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRAppearanceObject.WordWrap',
            'Hundredths of an Inch': 'dx_reportDesigner_DevExpress.XtraReports.UI.ReportUnit.HundredthsOfAnInch',
            'Page Height': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReport.PageHeight',
            'Metafile': 'dx_reportDesigner_DevExpress.XtraReports.UI.WinControlImageType.Metafile',
            'Custom Paint': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRChartScripts.OnCustomPaint',
            'Count': 'dx_reportDesigner_DevExpress.XtraReports.UI.SortingSummaryFunction.Count',
            'Population Variance (Distinct)': 'dx_reportDesigner_DevExpress.XtraReports.UI.SortingSummaryFunction.DVarP',
            'Use Border Dash Style': 'dx_reportDesigner_DevExpress.XtraReports.UI.StylePriority.UseBorderDashStyle',
            'OLAP Data Provider': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRPivotGrid.OLAPDataProvider',
            'Orientation': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRBarCode.BarCodeOrientation',
            'Fill Style': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRChart.FillStyle',
            'Report Scripts': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReportScripts',
            'Report Footer': 'dx_reportDesigner_DevExpress.XtraReports.UI.ReportFooterBand',
            'Can Grow': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControl.CanGrow',
            'Running Summary': 'dx_reportDesigner_DevExpress.XtraReports.UI.SummaryFunc.RunningSum',
            'Property Name': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRBinding.PropertyName',
            'Summary': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRLabel.Summary',
            'After the Band': 'dx_reportDesigner_DevExpress.XtraReports.UI.PageBreak.AfterBand',
            'Prefilter': 'dx_reportDesigner_DevExpress.XtraReports.UI.PivotGrid.XRPrefilter',
            'Alignment': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRBarCode.Alignment',
            'Page': 'dx_reportDesigner_DevExpress.XtraReports.UI.SummaryRunning.Page',
            'Detail': 'dx_reportDesigner_DevExpress.XtraReports.UI.DetailBand',
            'Reset': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRSummaryEvents.OnReset',
            'Summary Reset': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRLabelScripts.OnSummaryReset',
            'Text Changed': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControlEvents.OnTextChanged',
            'Pixels': 'dx_reportDesigner_DevExpress.XtraReports.UI.ReportUnit.Pixels',
            'Report': 'dx_reportDesigner_DevExpress.XtraReports.UI.SummaryRunning.Report',
            'Bottom': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRDockStyle.Bottom',
            'Titles': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRChart.Titles',
            'Draw': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControlEvents.OnDraw',
            'First Across, then Down': 'dx_reportDesigner_DevExpress.XtraReports.UI.ColumnDirection.AcrossThenDown',
            'Pivot Grid': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRPivotGrid',
            'Header Group Line': 'dx_reportDesigner_DevExpress.XtraReports.UI.PivotGrid.XRPivotGridAppearances.HeaderGroupLine',
            'Descending': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRColumnSortOrder.Descending',
            'Table Of Contents Title': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRTableOfContentsTitle',
            'Use WMPrint Recursively': 'dx_reportDesigner_DevExpress.XtraReports.UI.WinControlDrawMethod.UseWMPrintRecursive',
            'Table Of Contents Level': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRTableOfContentsLevel',
            'Visible': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControl.Visible',
            'Standard Population Deviation': 'dx_reportDesigner_DevExpress.XtraReports.UI.SortingSummaryFunction.StdDevP',
            'Layout': 'dx_reportDesigner_DevExpress.XtraReports.UI.MultiColumn.Layout',
            'Snap Grid Size': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReport.SnapGridSize',
            'Parameters Request Before Show': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReportScripts.OnParametersRequestBeforeShow',
            'HTML Item Created': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControlEvents.OnHtmlItemCreated',
            'Use WMPrint Method': 'dx_reportDesigner_DevExpress.XtraReports.UI.WinControlDrawMethod.UseWMPrint',
            'Use WMPaint Method': 'dx_reportDesigner_DevExpress.XtraReports.UI.WinControlDrawMethod.UseWMPaint',
            'Print on Page': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControlEvents.OnPrintOnPage',
            'Windows Forms Control': 'dx_reportDesigner_DevExpress.XtraReports.UI.WinControlContainer',
            'Snap Line Padding': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControl.SnapLinePadding',
            'Formatting Rule Sheet': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReport.FormattingRuleSheet',
            'PivotGrid Scripts': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRPivotGridScripts',
            'Show Margin Lines in Preview': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReport.ShowPreviewMarginLines',
            'Custom Row Height': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRPivotGridScripts.OnCustomRowHeight',
            'Check State': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRCheckBox.CheckState',
            'Segment Width': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRZipCode.SegmentWidth',
            'Value Member': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRSparkline.ValueMember',
            'Field Header Style': 'dx_reportDesigner_DevExpress.XtraReports.UI.PivotGrid.XRPivotGridStyles.FieldHeaderStyle',
            'Total Cell': 'dx_reportDesigner_DevExpress.XtraReports.UI.PivotGrid.XRPivotGridAppearances.TotalCell',
            'Vertical Content Splitting': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReport.VerticalContentSplitting',
            'Sub-Report': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRSubreport',
            'Binary Data': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRBarCode.BinaryData',
            'Column Spacing': 'dx_reportDesigner_DevExpress.XtraReports.UI.MultiColumn.ColumnSpacing',
            'Field Value Total Style': 'dx_reportDesigner_DevExpress.XtraReports.UI.PivotGrid.XRPivotGridStyles.FieldValueTotalStyle',
            'Custom Draw Crosshair': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRChartScripts.OnCustomDrawCrosshair',
            'Module': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRBarCode.Module',
            'Prefilter Criteria Changed': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRPivotGridScripts.OnPrefilterCriteriaChanged',
            'Sub-Band': 'dx_reportDesigner_DevExpress.XtraReports.UI.SubBand',
            'Line': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRLine',
            'Detail Report': 'dx_reportDesigner_DevExpress.XtraReports.UI.DetailReportBand',
            'Text Trimming': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControl.TextTrimming',
            'Print Mode': 'dx_reportDesigner_DevExpress.XtraReports.UI.WinControlContainer.PrintMode',
            'Level Title': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRTableOfContents.LevelTitle',
            'Calculated Field Scripts': 'dx_reportDesigner_DevExpress.XtraReports.UI.CalculatedFieldScripts',
            'Stretch': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRShape.Stretch',
            'Pivot Grid Data Source Options': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRChart.PivotGridDataSourceOptions',
            'FieldValueGrandTotal': 'dx_reportDesigner_DevExpress.XtraReports.UI.PivotGrid.XRPivotGridAppearances.FieldValueGrandTotal',
            'Keep Together with Detail Reports': 'dx_reportDesigner_DevExpress.XtraReports.UI.DetailBand.KeepTogetherWithDetailReports',
            'Print Header': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRPivotGridScripts.OnPrintHeader',
            'Landscape': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReport.Landscape',
            'Group Header Band Scripts': 'dx_reportDesigner_DevExpress.XtraReports.UI.GroupHeaderBandScripts',
            'Data Field Options': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRPivotGrid.OptionsDataField',
            'Print Cell': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRPivotGridScripts.OnPrintCell',
            'Page Break': 'dx_reportDesigner_DevExpress.XtraReports.UI.Band.PageBreak',
            'Bar Code': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRBarCode',
            'Record Number': 'dx_reportDesigner_DevExpress.XtraReports.UI.SummaryFunc.RecordNumber',
            'Top Margin': 'dx_reportDesigner_DevExpress.XtraReports.UI.TopMarginBand',
            'Tenths of a Millimeter': 'dx_reportDesigner_DevExpress.XtraReports.UI.ReportUnit.TenthsOfAMillimeter',
            'C#': 'dx_reportDesigner_DevExpress.XtraReports.ScriptLanguage.CSharp',
            'Show Footer': 'dx_reportDesigner_DevExpress.XtraReports.Design.GroupSort.GroupSortReflectItem.ShowFooter',
            'Data Source\'s Row Changed': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReportScripts.OnDataSourceRowChanged',
            'Leader Symbol': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRTableOfContentsLevel.LeaderSymbol',
            'Enabled': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRGroupSortingSummary.Enabled',
            'Data Options': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRPivotGrid.OptionsData',
            'View Options': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRPivotGrid.OptionsView',
            'Appearance': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRPivotGrid.Appearance',
            'Auto-Module': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRBarCode.AutoModule',
            'Band\'s Height Changed': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReportScripts.OnBandHeightChanged',
            'Show Header': 'dx_reportDesigner_DevExpress.XtraReports.Design.GroupSort.GroupSortReflectItem.ShowHeader',
            'Fore Color': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRTableOfContentsLevelBase.ForeColor',
            'Field Header': 'dx_reportDesigner_DevExpress.XtraReports.UI.PivotGrid.XRPivotGridAppearances.FieldHeader',
            'Merge based on Tag': 'dx_reportDesigner_DevExpress.XtraReports.UI.ValueSuppressType.MergeByTag',
            'Expression': 'dx_reportDesigner_DevExpress.XtraReports.UI.CalculatedField.Expression',
            'Picture Box': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRPictureBox',
            'Automatic Layout': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRChart.AutoLayout',
            'Summary Calculated': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRLabelScripts.OnSummaryCalculated',
            'Navigation Target': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControl.Target',
            'Field Value': 'dx_reportDesigner_DevExpress.XtraReports.UI.PivotGrid.XRPivotGridAppearances.FieldValue',
            'Parent Bookmark': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControl.BookmarkParent',
            'Table Of Contents': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRTableOfContents',
            'Legend': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRChart.Legend',
            'Blank Detail Count': 'dx_reportDesigner_DevExpress.XtraReports.UI.ReportPrintOptions.BlankDetailCount',
            'Text Vertical Alignment': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRAppearanceObject.TextVerticalAlignment',
            'Custom Cell Display Text': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRPivotGridScripts.OnCustomCellDisplayText',
            'Auto Width': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRLabel.AutoWidth',
            'Roll Paper': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReport.RollPaper',
            'Print On': 'dx_reportDesigner_DevExpress.XtraReports.UI.PageBand.PrintOn',
            'Field Value Total': 'dx_reportDesigner_DevExpress.XtraReports.UI.PivotGrid.XRPivotGridAppearances.FieldValueTotal',
            'Click in Preview': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControlEvents.OnPreviewClick',
            'Parameters Changed': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReportScripts.OnParametersRequestValueChanged',
            'Calculated Field': 'dx_reportDesigner_DevExpress.XtraReports.UI.CalculatedField',
            'Horizontal Content Splitting': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReport.HorizontalContentSplitting',
            'Running': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRSummary.Running',
            'Lines Style': 'dx_reportDesigner_DevExpress.XtraReports.UI.PivotGrid.XRPivotGridStyles.LinesStyle',
            'Snap Lines': 'dx_reportDesigner_DevExpress.XtraReports.UI.SnappingMode.SnapLines',
            'Use Background Color': 'dx_reportDesigner_DevExpress.XtraReports.UI.StyleUsing.UseBackColor',
            'Formatting Rule': 'dx_reportDesigner_DevExpress.XtraReports.UI.FormattingRule',
            'Merge based on Value': 'dx_reportDesigner_DevExpress.XtraReports.UI.ValueSuppressType.MergeByValue',
            'Series Sorting': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRChart.SeriesSorting',
            'Mouse Move in Preview': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControlEvents.OnPreviewMouseMove',
            'Mouse Down in Preview': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControlEvents.OnPreviewMouseDown',
            'Printer Name': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReport.PrinterName',
            'Target Value': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRGauge.TargetValue',
            'View Type': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRGauge.ViewType',
            'Odd Style': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControl.XRControlStyles.OddStyle',
            'Level Default': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRTableOfContents.LevelDefault',
            'Print when Data Source is Empty': 'dx_reportDesigner_DevExpress.XtraReports.UI.ReportPrintOptions.PrintOnEmptyDataSource',
            'Start Page Number': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRPageInfo.StartPageNumber',
            'Style': 'dx_reportDesigner_DevExpress.XtraReports.UI.ConditionFormatting.Style',
            'Indicators Palette Name': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRChart.IndicatorsPaletteName',
            'After Print': 'dx_reportDesigner_DevExpress.XtraReports.UI.XRControlEvents.OnAfterPrint',
            'As Bricks': 'dx_reportDesigner_DevExpress.XtraReports.UI.WinControlPrintMode.AsBricks',
            'Cell': 'dx_reportDesigner_DevExpress.XtraReports.UI.PivotGrid.XRPivotGridAppearances.Cell',
            'Grand Total Cell': 'dx_reportDesigner_DevExpress.XtraReports.UI.PivotGrid.XRPivotGridAppearances.GrandTotalCell',
            'Show Export Warnings': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReport.ShowExportWarnings',
            'Style Sheet\'s Path': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReport.StyleSheetPath',
            'Measure Units': 'dx_reportDesigner_DevExpress.XtraReports.UI.XtraReport.ReportUnit',
            'Field Type': 'dx_reportDesigner_DevExpress.XtraReports.UI.CalculatedField.FieldType',
            'Multi Select': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridOptionsSelection.MultiSelect',
            'OLAP Filter Using Where Clause': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldOptions.OLAPFilterUsingWhereClause',
            'Show Column Grand Totals': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowColumnGrandTotals',
            'Show Unbound Expression Menu': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridField.Options.ShowUnboundExpressionMenu',
            'ForeColor': 'dx_reportDesigner_DevExpress.PivotGrid.Printing.PrintAppearanceObject.ForeColor',
            'Field': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridStyleFormatCondition.Field',
            'Custom Totals': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotTotalsVisibility.CustomTotals',
            'Summary Type': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldSortBySummaryInfo.SummaryType',
            'Totals Visibility': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldBase.TotalsVisibility',
            'OLAP Filter By UniqueName': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldOptions.OLAPFilterByUniqueName',
            'Empty': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridAppearances.Empty',
            'Paper Height': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.PivotGridPageSettings.PaperHeight',
            'TotalCell': 'dx_reportDesigner_DevExpress.PivotGrid.Printing.PrintAppearance.TotalCell',
            'Field Value Grand Total': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridAppearancesBase.FieldValueGrandTotal',
            'Column Header Area': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridAppearances.ColumnHeaderArea',
            'GrandTotalCell': 'dx_reportDesigner_DevExpress.PivotGrid.Printing.PrintAppearance.GrandTotalCell',
            'Read Only': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridField.Options.ReadOnly',
            'Filter Separator Bar Padding': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridOptionsView.FilterSeparatorBarPadding',
            'Top Value Type': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldBase.TopValueType',
            'Numeric': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGroupInterval.Numeric',
            'Visible Count': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridGroup.VisibleCount',
            'Grand Total Cell Format': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldBase.GrandTotalCellFormat',
            'Chart Data Vertical': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.PivotGridOptionsChartDataSourceBase.ChartDataVertical',
            'Apply To Total Cell': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridStyleFormatCondition.ApplyToTotalCell',
            'After Values': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotRowTotalsLocation.Far',
            'Show Value Hints': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridOptionsHint.ShowValueHints',
            'Traffic Lights': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotKPIGraphic.TrafficLights',
            'Summary Display Type': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldBase.SummaryDisplayType',
            'Status': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotKPIType.Status',
            'Show Vert Lines': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowVertLines',
            'Month-Year': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGroupInterval.DateMonthYear',
            'Cell Format': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridCustomTotalBase.CellFormat',
            'Apply To Grand Total Cell': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridStyleFormatCondition.ApplyToGrandTotalCell',
            'Filter Values': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridGroup.FilterValues',
            'Allow Cross-Group Variation': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridOptionsData.AllowCrossGroupVariation',
            'Day': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGroupInterval.DateDay',
            'OLAP None': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotSortMode.None',
            'Store Layout Options': 'dx_reportDesigner_DevExpress.XtraPivotGrid.OptionsLayoutPivotGrid.StoreLayoutOptions',
            'Hide Empty Variation Items': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldOptions.HideEmptyVariationItems',
            'Value Format': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldToolTips.ValueFormat',
            'Weight': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotKPIType.Weight',
            'Month': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGroupInterval.DateMonth',
            'KPI Graphic': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldBase.KPIGraphic',
            'Horizontal Scrolling': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridOptionsBehavior.HorizontalScrolling',
            'Drill Down Max Row Count': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridOptionsData.DrillDownMaxRowCount',
            'Allow Edit': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridField.Options.AllowEdit',
            'Rank In Column Smallest To Largest': 'dx_reportDesigner_DevExpress.Data.PivotGrid.PivotSummaryDisplayType.RankInColumnSmallestToLargest',
            'Percent Of Row Grand Total': 'dx_reportDesigner_DevExpress.Data.PivotGrid.PivotSummaryDisplayType.PercentOfRowGrandTotal',
            'Show Button Mode': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridField.Options.ShowButtonMode',
            'Enable Field Value Menu': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridOptionsMenu.EnableFieldValueMenu',
            'Export Empty Cells': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.PivotGridOptionsChartDataSourceBase.ExportEmptyCells',
            'Group Interval Numeric Range': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldBase.GroupIntervalNumericRange',
            'Percent Of Row': 'dx_reportDesigner_DevExpress.Data.PivotGrid.PivotSummaryDisplayType.PercentOfRow',
            'Variation for Entire Population': 'dx_reportDesigner_DevExpress.Data.PivotGrid.PivotSummaryType.Varp',
            'Allow Filter By Summary': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridOptionsCustomization.AllowFilterBySummary',
            'Show Grand Total': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldOptions.ShowGrandTotal',
            'PrefilterPanel': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridAppearances.PrefilterPanel',
            'Column Value Line Count': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridField.ColumnValueLineCount',
            'Data Area': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotArea.DataArea',
            'Show Row Totals': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowRowTotals',
            'OLAP Key': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotSortMode.Key',
            'Always': 'dx_reportDesigner_DevExpress.XtraPivotGrid.AllowHideFieldsType.Always',
            'Caption': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldBase.Caption',
            'Unbound Field Name': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldBase.UnboundFieldName',
            'All Areas': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridAllowedAreas.All',
            'Faces': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotKPIGraphic.Faces',
            'Show Data Headers': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowDataHeaders',
            'Print Horizontal Lines': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.PrintHorzLines',
            'Max Point Count By Series': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.PivotGridOptionsChartDataSourceBase.MaxPointCountBySeries',
            'Show Filter Separator Bar': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowFilterSeparatorBar',
            'Variation': 'dx_reportDesigner_DevExpress.Data.PivotGrid.PivotSummaryType.Var',
            'Show Grand Totals For Single Values': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowGrandTotalsForSingleValues',
            'Allow Sort': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldOptions.AllowSort',
            'Allow Drag': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldOptions.AllowDrag',
            'Never': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotOLAPFilterUsingWhereClause.Never',
            'BackColor': 'dx_reportDesigner_DevExpress.PivotGrid.Printing.PrintAppearanceObject.BackColor',
            'Criteria': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Prefilter.Criteria',
            'Header Filter Button Active': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridAppearances.HeaderFilterButtonActive',
            'Print Data Headers': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.PrintDataHeaders',
            'Draw Focus Rect': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.DrawFocusedCellRect',
            'Column Area': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotArea.ColumnArea',
            'Data Header Area': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridAppearances.DataHeaderArea',
            'Show Column Totals': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowColumnTotals',
            'Use Aggregate For Single Filter Value': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridOptionsOLAP.UseAggregateForSingleFilterValue',
            'Grand Total Text': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldBase.GrandTotalText',
            'Sort By Summary Info': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldBase.SortBySummaryInfo',
            'Show In Customization Form': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldOptions.ShowInCustomizationForm',
            'Custom Total Summary Type': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldSortBySummaryInfo.CustomTotalSummaryType',
            'Area Index': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldBase.AreaIndex',
            'Percent Of Grand Total': 'dx_reportDesigner_DevExpress.Data.PivotGrid.PivotSummaryDisplayType.PercentOfGrandTotal',
            'Page Settings': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.PageSettings',
            'Image Index': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridField.ImageIndex',
            'OLAP Unique Member Name': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldSortCondition.OLAPUniqueMemberName',
            'Allow Sort By Summary': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldOptions.AllowSortBySummary',
            'Status Arrow': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotKPIGraphic.StatusArrow',
            'Control': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridScrolling.Control',
            'Row Count': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridCells.RowCount',
            'Show Row Grand Totals': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowRowGrandTotals',
            'Allow Glyph Skinning': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridOptionsView.AllowGlyphSkinning',
            'Area': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridOptionsDataField.Area',
            'Unbound Expression': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldBase.UnboundExpression',
            'Year': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGroupInterval.DateYear',
            'Case Sensitive': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridOptionsData.CaseSensitive',
            'Date-Hour': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGroupInterval.DateHour',
            'Max Height': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridOptionsSelection.MaxHeight',
            'Preserve Collapsed Levels': 'dx_reportDesigner_DevExpress.XtraPivotGrid.CopyCollapsedValuesMode.PreserveCollapsedLevels',
            'Merge Row Field Values': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.MergeRowFieldValues',
            'Allow Drag In Customization Form': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridOptionsCustomization.AllowDragInCustomizationForm',
            'Show Filter Headers': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowFilterHeaders',
            'Total Cell Format': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldBase.TotalCellFormat',
            'Empty Value Text': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldBase.EmptyValueText',
            'Expanded In Fields Group': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldBase.ExpandedInFieldsGroup',
            'Loading Panel Delay': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridOptionsBehaviorBase.LoadingPanelDelay',
            'Row Totals Location': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.RowTotalsLocation',
            'Print Row Headers': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.PrintRowHeaders',
            'Group Filter Mode': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldOptions.GroupFilterMode',
            'Row Value Line Count': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridOptionsDataFieldEx.RowValueLineCount',
            'Row Area': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotArea.RowArea',
            'Date-Hour-Minute': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGroupInterval.DateHourMinute',
            'Show Cell Hints': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridOptionsHint.ShowCellHints',
            'Copy Field Values To Clipboard Behavior': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridOptionsBehavior.ClipboardCopyCollapsedValuesMode',
            'Top Value Show Others': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldBase.TopValueShowOthers',
            'Allow Filter': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldOptions.AllowFilter',
            'Rank In Row Largest To Smallest': 'dx_reportDesigner_DevExpress.Data.PivotGrid.PivotSummaryDisplayType.RankInRowLargestToSmallest',
            'Server Defined': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotKPIGraphic.ServerDefined',
            'Road Signs': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotKPIGraphic.RoadSigns',
            'Header Height Offset': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.HeaderHeightOffset',
            'Apply Best Fit On Field Dragging': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridOptionsBehavior.ApplyBestFitOnFieldDragging',
            'Allow Expand': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldOptions.AllowExpand',
            'Show Column Custom Totals': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.PivotGridOptionsChartDataSourceBase.ShowColumnCustomTotals',
            'Reversed Cylinder': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotKPIGraphic.ReversedCylinder',
            'Before Values': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotTotalsLocation.Near',
            'Total Value Format': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldBase.TotalValueFormat',
            'CustomTotalCell': 'dx_reportDesigner_DevExpress.PivotGrid.Printing.PrintAppearance.CustomTotalCell',
            'Criteria As String': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PrefilterBase.CriteriaString',
            'CellsArea': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridScrolling.CellsArea',
            'Selected Cell': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridAppearances.SelectedCell',
            'Top Value Count': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldBase.TopValueCount',
            'Day Of Year': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGroupInterval.DateDayOfYear',
            'Day Of Week': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGroupInterval.DateDayOfWeek',
            'Column Totals Location': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ColumnTotalsLocation',
            'Reversed Thermometer': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotKPIGraphic.ReversedThermometer',
            'Cell Editor': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridField.FieldEdit',
            'Use Print Appearance': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.UsePrintAppearance',
            'Copy To Clipboard With Field Values': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridOptionsBehavior.CopyToClipboardWithFieldValues',
            'Year Age': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGroupInterval.YearAge',
            'Merge Column Field Values': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.MergeColumnFieldValues',
            'Export As Numbers To Excel': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldBase.UseNativeFormat',
            'Use Summary Values': 'dx_reportDesigner_DevExpress.XtraPivotGrid.DataFieldUnboundExpressionMode.UseSummaryValues',
            'Quarter-Year': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGroupInterval.DateQuarterYear',
            'XMLA': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.OLAPDataProvider.Xmla',
            'Filter By Visible Fields Only': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridOptionsData.FilterByVisibleFieldsOnly',
            'Adomd': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.OLAPDataProvider.Adomd',
            'OleDb': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.OLAPDataProvider.OleDb',
            'Shapes': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotKPIGraphic.Shapes',
            'Rank In Row Smallest To Largest': 'dx_reportDesigner_DevExpress.Data.PivotGrid.PivotSummaryDisplayType.RankInRowSmallestToLargest',
            'Row Header Area': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridAppearances.RowHeaderArea',
            'Auto': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotOLAPFilterUsingWhereClause.Auto',
            'Cylinder': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotKPIGraphic.Cylinder',
            'Filter By UniqueName': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridOptionsOLAP.FilterByUniqueName',
            'Sort Mode': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldBase.SortMode',
            'Thermometer': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotKPIGraphic.Thermometer',
            'Standard Deviation for Entire Population': 'dx_reportDesigner_DevExpress.Data.PivotGrid.PivotSummaryType.StdDevp',
            'Allow Prefilter': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridOptionsCustomizationEx.AllowPrefilter',
            'Reversed Gauge': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotKPIGraphic.ReversedGauge',
            'Reset Options': 'dx_reportDesigner_DevExpress.XtraPivotGrid.OptionsLayoutPivotGrid.ResetOptions',
            'Header Filter Button Show Mode': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridOptionsView.HeaderFilterButtonShowMode',
            'Single Values Only': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotOLAPFilterUsingWhereClause.SingleValuesOnly',
            'Show Row Custom Totals': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.PivotGridOptionsChartDataSourceBase.ShowRowCustomTotals',
            'Allow Customization Form': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridOptionsCustomization.AllowCustomizationForm',
            'Value Text': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldToolTips.ValueText',
            'Empty Cell Text': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldBase.EmptyCellText',
            'Focused Cell': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridAppearances.FocusedCell',
            'Cell Selection': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridOptionsSelection.CellSelection',
            'OLAP ID': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotSortMode.ID',
            'Filter Area': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotArea.FilterArea',
            'Quarter': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGroupInterval.DateQuarter',
            'Percent Of Column Grand Total': 'dx_reportDesigner_DevExpress.Data.PivotGrid.PivotSummaryDisplayType.PercentOfColumnGrandTotal',
            'Show Row Headers': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowRowHeaders',
            'Week Of Year': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGroupInterval.DateWeekOfYear',
            'Month Age': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGroupInterval.MonthAge',
            'Percent': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotTopValueType.Percent',
            'Allow Run Time Summary Change': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldOptions.AllowRunTimeSummaryChange',
            'Show Custom Totals': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldOptions.ShowCustomTotals',
            'Duplicate Collapsed Values': 'dx_reportDesigner_DevExpress.XtraPivotGrid.CopyCollapsedValuesMode.DuplicateCollapsedValues',
            'Header Filter Button': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridAppearances.HeaderFilterButton',
            'Percent Variation': 'dx_reportDesigner_DevExpress.Data.PivotGrid.PivotSummaryDisplayType.PercentVariation',
            'Row Header Width': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridOptionsDataField.RowHeaderWidth',
            'Header Area': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridAppearances.HeaderArea',
            'Show In Prefilter': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldOptions.ShowInPrefilter',
            'Show Column Headers': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowColumnHeaders',
            'Print Unused Filter Fields': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.PrintUnusedFilterFields',
            'FieldValueTotal': 'dx_reportDesigner_DevExpress.PivotGrid.Printing.PrintAppearance.FieldValueTotal',
            'Remove Collapsed Levels': 'dx_reportDesigner_DevExpress.XtraPivotGrid.CopyCollapsedValuesMode.RemoveCollapsedLevels',
            'Week Age': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGroupInterval.WeekAge',
            'No Totals': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotTotalsVisibility.None',
            'Show Header Hints': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridOptionsHint.ShowHeaderHints',
            'Day Age': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGroupInterval.DayAge',
            'Rank In Column Largest To Smallest': 'dx_reportDesigner_DevExpress.Data.PivotGrid.PivotSummaryDisplayType.RankInColumnLargestToSmallest',
            'Tool Tips': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridField.ToolTips',
            'Min Width': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldBase.MinWidth',
            'Apply To Custom Total Cell': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridStyleFormatCondition.ApplyToCustomTotalCell',
            'Row Field Value Separator': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.RowFieldValueSeparator',
            'Apply To Cell': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridStyleFormatCondition.ApplyToCell',
            'Variance Arrow': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotKPIGraphic.VarianceArrow',
            'Options': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldBase.Options',
            'Grouping Interval': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldBase.GroupInterval',
            'Data Column Name': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldBase.FieldName',
            'Show Always': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotShowButtonModeEnum.ShowAlways',
            'Goal': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotKPIType.Goal',
            'Expand Button': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridAppearances.ExpandButton',
            'Show Column Grand Total Header': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowColumnGrandTotalHeader',
            'Show For The Focused Cell Only': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotShowButtonModeEnum.ShowForFocusedCell',
            'Display Folder': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldBase.DisplayFolder',
            'Minute': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGroupInterval.Minute',
            'Show Summary Type Name': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldOptions.ShowSummaryTypeName',
            'Second': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGroupInterval.Second',
            'Print Headers on Every Page': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.PrintHeadersOnEveryPage',
            'Data Field Unbound Expression Mode': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridOptionsData.DataFieldUnboundExpressionMode',
            'Show Horz Lines': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowHorzLines',
            'Max Series Count': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.PivotGridOptionsChartDataSourceBase.MaxSeriesCount',
            'Header Width Offset': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.HeaderWidthOffset',
            'Conditions': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldSortBySummaryInfo.Conditions',
            'Trend': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotKPIType.Trend',
            'Week Of Month': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGroupInterval.DateWeekOfMonth',
            'Show Values': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldOptions.ShowValues',
            'Display Text': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotSortMode.DisplayText',
            'Alphabetical': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGroupInterval.Alphabetical',
            'Row Tree Width': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridOptionsViewBase.RowTreeWidth',
            'Allowed Areas': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldBase.AllowedAreas',
            'Show Totals': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldOptions.ShowTotals',
            'Running Total': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldBase.RunningTotal',
            'Show Custom Totals For Single Values': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowCustomTotalsForSingleValues',
            'Selection': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridCells.Selection',
            'Add New Groups': 'dx_reportDesigner_DevExpress.XtraPivotGrid.OptionsLayoutPivotGrid.AddNewGroups',
            'Enable Header Menu': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridOptionsMenu.EnableHeaderMenu',
            'Field Naming': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridOptionsDataField.FieldNaming',
            'Show When Editor Is Active': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotShowButtonModeEnum.ShowOnlyInEditor',
            'Show Row Grand Total Header': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowRowGrandTotalHeader',
            'Group Fields in the Customization Window': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridOptionsView.GroupFieldsInCustomizationWindow',
            'FieldValue': 'dx_reportDesigner_DevExpress.PivotGrid.Printing.PrintAppearance.FieldValue',
            'Filter Header Area': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridAppearances.FilterHeaderArea',
            'Allow Hide Fields': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridOptionsCustomization.AllowHideFields',
            'Totals Location': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.TotalsLocation',
            'Print Vertical Lines': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.PrintVertLines',
            'Multi Selection': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridCells.MultiSelection',
            'OLAP Use NonEmpty': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldOptions.OLAPUseNonEmpty',
            'Header Text': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldToolTips.HeaderText',
            'Clipboard Copy Multiselection Mode': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridOptionsBehavior.ClipboardCopyMultiSelectionMode',
            'Show Totals For Single Values': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowTotalsForSingleValues',
            'Absolute Variation': 'dx_reportDesigner_DevExpress.Data.PivotGrid.PivotSummaryDisplayType.AbsoluteVariation',
            'Row Tree Offset': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridOptionsViewBase.RowTreeOffset',
            'Tree': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGroupFilterMode.Tree',
            'List': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGroupFilterMode.List',
            'Unbound Type': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridFieldBase.UnboundType',
            'Auto Expand Groups': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridOptionsData.AutoExpandGroups',
            'Standard Arrow': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotKPIGraphic.StandardArrow',
            'Tree Like': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotRowTotalsLocation.Tree',
            'Column Field Value Separator': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.ColumnFieldValueSeparator',
            'When Customization Form Visible': 'dx_reportDesigner_DevExpress.XtraPivotGrid.AllowHideFieldsType.WhenCustomizationFormVisible',
            'Selection Only': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridOptionsChartDataSource.SelectionOnly',
            'BorderColor': 'dx_reportDesigner_DevExpress.PivotGrid.Printing.PrintAppearanceObject.BorderColor',
            'Date': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGroupInterval.Date',
            'Hour': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGroupInterval.Hour',
            'Date-Hour-Minute-Second': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGroupInterval.DateHourMinuteSecond',
            'Reversed Status Arrow': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotKPIGraphic.ReversedStatusArrow',
            'Print Filter Headers': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.PrintFilterHeaders',
            'FieldHeader': 'dx_reportDesigner_DevExpress.PivotGrid.Printing.PrintAppearance.FieldHeader',
            'Print Column Headers': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.PrintColumnHeaders',
            'Absolute': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotTopValueType.Absolute',
            'Enable Appearance Focused Cell': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridOptionsSelection.EnableAppearanceFocusedCell',
            'Enable Header Area Menu': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridOptionsMenu.EnableHeaderAreaMenu',
            'Sort By Column Indicator Image': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridAppearances.SortByColumnIndicatorImage',
            'Use Async Mode': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridOptionsBehaviorBase.UseAsyncMode',
            'Paper Width': 'dx_reportDesigner_DevExpress.XtraPivotGrid.Data.PivotGridPageSettings.PaperWidth',
            'Default Members Behavior': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridOptionsOLAP.DefaultMembersBehavior',
            'Max Width': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridOptionsSelection.MaxWidth',
            'Percent Of Column': 'dx_reportDesigner_DevExpress.Data.PivotGrid.PivotSummaryDisplayType.PercentOfColumn',
            'Index': 'dx_reportDesigner_DevExpress.Data.PivotGrid.PivotSummaryDisplayType.Index',
            'Update Delay': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotGridOptionsChartDataSource.UpdateDelay',
            'Automatic Totals': 'dx_reportDesigner_DevExpress.XtraPivotGrid.PivotTotalsVisibility.AutomaticTotals',
            'Show Level 76.4%': 'dx_reportDesigner_DevExpress.XtraCharts.FibonacciIndicator.ShowLevel76_4',
            'Show Level 23.6%': 'dx_reportDesigner_DevExpress.XtraCharts.FibonacciIndicator.ShowLevel23_6',
            'From Center Horizontal': 'dx_reportDesigner_DevExpress.XtraCharts.RectangleGradientMode.FromCenterHorizontal',
            'Right Bottom': 'dx_reportDesigner_DevExpress.XtraCharts.DockCorner.RightBottom',
            'Reverse': 'dx_reportDesigner_DevExpress.XtraCharts.Axis.Reverse',
            'Close': 'dx_reportDesigner_DevExpress.XtraCharts.StockLevel.Close',
            'Line Tension Percent': 'dx_reportDesigner_DevExpress.XtraCharts.FullStackedSplineArea3DSeriesView.LineTensionPercent',
            'Equal Bar Width': 'dx_reportDesigner_DevExpress.XtraCharts.SideBySideStackedBarSeriesView.EqualBarWidth',
            'Right Outside': 'dx_reportDesigner_DevExpress.XtraCharts.LegendAlignmentHorizontal.RightOutside',
            'Diamond': 'dx_reportDesigner_DevExpress.XtraCharts.MarkerKind.Diamond',
            'Resolve Overlapping Options': 'dx_reportDesigner_DevExpress.XtraCharts.AxisLabel.ResolveOverlappingOptions',
            'Thousands': 'dx_reportDesigner_DevExpress.XtraCharts.NumericGridAlignment.Thousands',
            'Axis X': 'dx_reportDesigner_DevExpress.XtraCharts.SwiftPlotDiagram.AxisX',
            'Axis Y': 'dx_reportDesigner_DevExpress.XtraCharts.SwiftPlotDiagram.AxisY',
            'Left': 'dx_reportDesigner_DevExpress.XtraCharts.LegendAlignmentHorizontal.Left',
            'Start To Start': 'dx_reportDesigner_DevExpress.XtraCharts.TaskLinkType.StartToStart',
            'Shape Kind': 'dx_reportDesigner_DevExpress.XtraCharts.Annotation.ShapeKind',
            'Base Level Text Color': 'dx_reportDesigner_DevExpress.XtraCharts.FibonacciIndicatorLabel.BaseLevelTextColor',
            'Retrieve Column Custom Totals': 'dx_reportDesigner_DevExpress.XtraCharts.PivotGridDataSourceOptions.RetrieveColumnCustomTotals',
            'Data Filters': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesBase.DataFilters',
            'Scrolling Options': 'dx_reportDesigner_DevExpress.XtraCharts.Diagram3D.ScrollingOptions',
            'Custom Panel': 'dx_reportDesigner_DevExpress.XtraCharts.SimpleDiagram.CustomPanel',
            'Color': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesViewBase.Color',
            'YZX': 'dx_reportDesigner_DevExpress.XtraCharts.RotationOrder.YZX',
            'YXZ': 'dx_reportDesigner_DevExpress.XtraCharts.RotationOrder.YXZ',
            'ZXY': 'dx_reportDesigner_DevExpress.XtraCharts.RotationOrder.ZXY',
            'ZYX': 'dx_reportDesigner_DevExpress.XtraCharts.RotationOrder.ZYX',
            'XYZ': 'dx_reportDesigner_DevExpress.XtraCharts.RotationOrder.XYZ',
            'XZY': 'dx_reportDesigner_DevExpress.XtraCharts.RotationOrder.XZY',
            'Secondary Axes Y': 'dx_reportDesigner_DevExpress.XtraCharts.XYDiagram.SecondaryAxesY',
            'Secondary Axes X': 'dx_reportDesigner_DevExpress.XtraCharts.XYDiagram.SecondaryAxesX',
            'Axis': 'dx_reportDesigner_DevExpress.XtraCharts.AxisXCoordinate.Axis',
            'Marker Visibility': 'dx_reportDesigner_DevExpress.XtraCharts.RadarLineSeriesView.MarkerVisibility',
            'Default Pane': 'dx_reportDesigner_DevExpress.XtraCharts.XYDiagram2D.DefaultPane',
            'Plus': 'dx_reportDesigner_DevExpress.XtraCharts.MarkerKind.Plus',
            'Star': 'dx_reportDesigner_DevExpress.XtraCharts.MarkerKind.Star',
            'Not Equal': 'dx_reportDesigner_DevExpress.XtraCharts.DataFilterCondition.NotEqual',
            'Top Outside': 'dx_reportDesigner_DevExpress.XtraCharts.LegendAlignmentVertical.TopOutside',
            'Vertical': 'dx_reportDesigner_DevExpress.XtraCharts.LayoutDirection.Vertical',
            'Show Additional Levels': 'dx_reportDesigner_DevExpress.XtraCharts.FibonacciIndicator.ShowAdditionalLevels',
            'Outside': 'dx_reportDesigner_DevExpress.XtraCharts.BubbleLabelPosition.Outside',
            'Height to Width Ratio': 'dx_reportDesigner_DevExpress.XtraCharts.FunnelSeriesViewBase.HeightToWidthRatio',
            'Millisecond': 'dx_reportDesigner_DevExpress.XtraCharts.DateTimeMeasureUnit.Millisecond',
            'Shadow': 'dx_reportDesigner_DevExpress.XtraCharts.XYDiagramSeriesViewBase.Shadow',
            'Hatch Style': 'dx_reportDesigner_DevExpress.XtraCharts.HatchFillOptions.HatchStyle',
            'Color Each': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesView3DColorEachSupportBase.ColorEach',
            'Value Data Members': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesBase.ValueDataMembers',
            'Finish To Finish': 'dx_reportDesigner_DevExpress.XtraCharts.TaskLinkType.FinishToFinish',
            'Pyramid': 'dx_reportDesigner_DevExpress.XtraCharts.Bar3DModel.Pyramid',
            'Solid': 'dx_reportDesigner_DevExpress.XtraCharts.FillMode3D.Solid',
            'Top Left to Bottom Right': 'dx_reportDesigner_DevExpress.XtraCharts.RectangleGradientMode.TopLeftToBottomRight',
            'Y': 'dx_reportDesigner_DevExpress.XtraCharts.ChartAnchorPoint.Y',
            'X': 'dx_reportDesigner_DevExpress.XtraCharts.ChartAnchorPoint.X',
            'Tool Tip Image': 'dx_reportDesigner_DevExpress.XtraCharts.Series.ToolTipImage',
            'Bottom Inside': 'dx_reportDesigner_DevExpress.XtraCharts.BarSeriesLabelPosition.BottomInside',
            'Retrieve Row Custom Totals': 'dx_reportDesigner_DevExpress.XtraCharts.PivotGridDataSourceOptions.RetrieveRowCustomTotals',
            'Tangent': 'dx_reportDesigner_DevExpress.XtraCharts.PieSeriesLabelPosition.Tangent',
            'From Center Vertical': 'dx_reportDesigner_DevExpress.XtraCharts.RectangleGradientMode.FromCenterVertical',
            'Minimum Value Label': 'dx_reportDesigner_DevExpress.XtraCharts.RangeAreaLabelKind.MinValueLabel',
            'Child Color': 'dx_reportDesigner_DevExpress.XtraCharts.TaskLinkColorSource.ChildColor',
            'Border': 'dx_reportDesigner_DevExpress.XtraCharts.BarSeriesView.Border',
            'Chart Measure Unit': 'dx_reportDesigner_DevExpress.XtraCharts.ChartRangeControlClientSnapMode.ChartMeasureUnit',
            'Dimension': 'dx_reportDesigner_DevExpress.XtraCharts.SimpleDiagram3D.Dimension',
            'Antialiasing': 'dx_reportDesigner_DevExpress.XtraCharts.Legend.Antialiasing',
            'Minor Length': 'dx_reportDesigner_DevExpress.XtraCharts.TickmarksBase.MinorLength',
            'Show for Zero Values': 'dx_reportDesigner_DevExpress.XtraCharts.BarSeriesLabel.ShowForZeroValues',
            'Minor Color': 'dx_reportDesigner_DevExpress.XtraCharts.GridLines.MinorColor',
            'Value Numeric Options': 'dx_reportDesigner_DevExpress.XtraCharts.PointOptions.ValueNumericOptions',
            'Clockwise': 'dx_reportDesigner_DevExpress.XtraCharts.PieSweepDirection.Clockwise',
            'Line Visibility': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesLabelBase.LineVisibility',
            'Tens': 'dx_reportDesigner_DevExpress.XtraCharts.NumericMeasureUnit.Tens',
            'Ones': 'dx_reportDesigner_DevExpress.XtraCharts.NumericMeasureUnit.Ones',
            'Bar Distance': 'dx_reportDesigner_DevExpress.XtraCharts.SideBySideFullStackedBar3DSeriesView.BarDistance',
            'Minor Thickness': 'dx_reportDesigner_DevExpress.XtraCharts.TickmarksBase.MinorThickness',
            'Allow Stagger': 'dx_reportDesigner_DevExpress.XtraCharts.AxisLabelResolveOverlappingOptions.AllowStagger',
            'Max Horizontal Percentage': 'dx_reportDesigner_DevExpress.XtraCharts.Legend.MaxHorizontalPercentage',
            'Colorizer': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesBase.Colorizer',
            'Center': 'dx_reportDesigner_DevExpress.XtraCharts.FunnelSeriesLabelPosition.Center',
            'Marker Line Visible': 'dx_reportDesigner_DevExpress.XtraCharts.Line3DSeriesView.MarkerLineVisible',
            'AutoSize': 'dx_reportDesigner_DevExpress.XtraCharts.TextAnnotation.AutoSize',
            'Rotation Angle Y': 'dx_reportDesigner_DevExpress.XtraCharts.Diagram3D.RotationAngleY',
            'Rotation Angle X': 'dx_reportDesigner_DevExpress.XtraCharts.Diagram3D.RotationAngleX',
            'Rotation Angle Z': 'dx_reportDesigner_DevExpress.XtraCharts.Diagram3D.RotationAngleZ',
            'Max Allowed Point Count in Series': 'dx_reportDesigner_DevExpress.XtraCharts.PivotGridDataSourceOptions.MaxAllowedPointCountInSeries',
            'Numerical': 'dx_reportDesigner_DevExpress.XtraCharts.ScaleType.Numerical',
            'Use Mouse Wheel': 'dx_reportDesigner_DevExpress.XtraCharts.ZoomingOptions.UseMouseWheel',
            'MinValueAngle': 'dx_reportDesigner_DevExpress.XtraCharts.RangeAreaSeriesLabel.MinValueAngle',
            'Length': 'dx_reportDesigner_DevExpress.XtraCharts.TickmarksBase.Length',
            'Max Value Internal': 'dx_reportDesigner_DevExpress.XtraCharts.AxisRange.MaxValueInternal',
            'AxisValue': 'dx_reportDesigner_DevExpress.XtraCharts.StripLimit.AxisValue',
            'Value Level': 'dx_reportDesigner_DevExpress.XtraCharts.SingleLevelIndicator.ValueLevel',
            'Top to Bottom': 'dx_reportDesigner_DevExpress.XtraCharts.PolygonGradientMode.TopToBottom',
            'Envelope Line Style': 'dx_reportDesigner_DevExpress.XtraCharts.MovingAverage.EnvelopeLineStyle',
            'GridAlignment': 'dx_reportDesigner_DevExpress.XtraCharts.ChartRangeControlClientDateTimeGridOptions.GridAlignment',
            'Top N Options': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesBase.TopNOptions',
            'Inverted Step': 'dx_reportDesigner_DevExpress.XtraCharts.StepAreaSeriesView.InvertedStep',
            'Series Indent Fixed': 'dx_reportDesigner_DevExpress.XtraCharts.XYDiagram3D.SeriesIndentFixed',
            'Top Right to Bottom Left': 'dx_reportDesigner_DevExpress.XtraCharts.PolygonGradientMode.TopRightToBottomLeft',
            'Tail': 'dx_reportDesigner_DevExpress.XtraCharts.AnnotationConnectorStyle.Tail',
            'Use Weight': 'dx_reportDesigner_DevExpress.XtraCharts.PaneSizeMode.UseWeight',
            'DateTimeOptions': 'dx_reportDesigner_DevExpress.XtraCharts.AxisLabel.DateTimeOptions',
            'Show Others': 'dx_reportDesigner_DevExpress.XtraCharts.TopNOptions.ShowOthers',
            'Position': 'dx_reportDesigner_DevExpress.XtraCharts.AxisLabel3D.Position',
            'Dot': 'dx_reportDesigner_DevExpress.XtraCharts.DashStyle.Dot',
            'Rotation Options': 'dx_reportDesigner_DevExpress.XtraCharts.Diagram3D.RotationOptions',
            'Point View': 'dx_reportDesigner_DevExpress.XtraCharts.PointOptions.PointView',
            'Common Label Position': 'dx_reportDesigner_DevExpress.XtraCharts.CrosshairOptions.CommonLabelPosition',
            'Left Outside': 'dx_reportDesigner_DevExpress.XtraCharts.LegendAlignmentHorizontal.LeftOutside',
            'Auto Scale Breaks': 'dx_reportDesigner_DevExpress.XtraCharts.Axis.AutoScaleBreaks',
            'Shape Fillet': 'dx_reportDesigner_DevExpress.XtraCharts.Annotation.ShapeFillet',
            'Tickmarks': 'dx_reportDesigner_DevExpress.XtraCharts.RadarAxisY.Tickmarks',
            'Size As Percentage': 'dx_reportDesigner_DevExpress.XtraCharts.Pie3DSeriesView.SizeAsPercentage',
            'Currency': 'dx_reportDesigner_DevExpress.XtraCharts.NumericFormat.Currency',
            'Precision': 'dx_reportDesigner_DevExpress.XtraCharts.NumericOptions.Precision',
            'Size In Pixels': 'dx_reportDesigner_DevExpress.XtraCharts.XYDiagramPaneBase.SizeInPixels',
            'Min Value': 'dx_reportDesigner_DevExpress.XtraCharts.AxisRange.MinValue',
            'Axis Value': 'dx_reportDesigner_DevExpress.XtraCharts.CustomAxisLabel.AxisValue',
            'Parent Color': 'dx_reportDesigner_DevExpress.XtraCharts.TaskLinkColorSource.ParentColor',
            'Use Keyboard with Mouse': 'dx_reportDesigner_DevExpress.XtraCharts.ZoomingOptions.UseKeyboardWithMouse',
            'Week': 'dx_reportDesigner_DevExpress.XtraCharts.DateTimeGridAlignment.Week',
            'Millions': 'dx_reportDesigner_DevExpress.XtraCharts.NumericMeasureUnit.Millions',
            'Column Name': 'dx_reportDesigner_DevExpress.XtraCharts.DataFilter.ColumnName',
            'Scientific': 'dx_reportDesigner_DevExpress.XtraCharts.NumericFormat.Scientific',
            'Grid Alignment': 'dx_reportDesigner_DevExpress.XtraCharts.NumericScaleOptions.GridAlignment',
            'Exploded Points': 'dx_reportDesigner_DevExpress.XtraCharts.PieSeriesViewBase.ExplodedPoints',
            'Label Mode': 'dx_reportDesigner_DevExpress.XtraCharts.Annotation.LabelMode',
            'Resolve Overlapping Min Indent': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesLabelBase.ResolveOverlappingMinIndent',
            'Standard': 'dx_reportDesigner_DevExpress.XtraCharts.TimeSpanFormat.Standard',
            'Indicators': 'dx_reportDesigner_DevExpress.XtraCharts.XYDiagram2DSeriesViewBase.Indicators',
            'Runtime Rotation': 'dx_reportDesigner_DevExpress.XtraCharts.Annotation.RuntimeRotation',
            'Whole Range': 'dx_reportDesigner_DevExpress.XtraCharts.AxisBase.WholeRange',
            'Offest X': 'dx_reportDesigner_DevExpress.XtraCharts.CrosshairLabelPosition.OffsetX',
            'Offest Y': 'dx_reportDesigner_DevExpress.XtraCharts.CrosshairLabelPosition.OffsetY',
            'Moving Average And Envelope': 'dx_reportDesigner_DevExpress.XtraCharts.MovingAverageKind.MovingAverageAndEnvelope',
            'Max Line Count': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesLabelBase.MaxLineCount',
            'Grid Offset': 'dx_reportDesigner_DevExpress.XtraCharts.ScaleOptionsBase.GridOffset',
            'Thickness': 'dx_reportDesigner_DevExpress.XtraCharts.TickmarksBase.Thickness',
            'Minor Count': 'dx_reportDesigner_DevExpress.XtraCharts.AxisBase.MinorCount',
            'Text Color': 'dx_reportDesigner_DevExpress.XtraCharts.TextAnnotation.TextColor',
            'Max Limit': 'dx_reportDesigner_DevExpress.XtraCharts.Strip.MaxLimit',
            'Transparency': 'dx_reportDesigner_DevExpress.XtraCharts.XYDiagram3DSeriesViewBase.Transparency',
            'Stacked Group': 'dx_reportDesigner_DevExpress.XtraCharts.SideBySideStackedBar3DSeriesView.StackedGroup',
            'Marker Options': 'dx_reportDesigner_DevExpress.XtraCharts.AreaSeriesViewBase.MarkerOptions',
            'Kind': 'dx_reportDesigner_DevExpress.XtraCharts.FibonacciIndicator.Kind',
            'Exploded Distance Percentage': 'dx_reportDesigner_DevExpress.XtraCharts.PieSeriesViewBase.ExplodedDistancePercentage',
            'LegendItemPattern': 'dx_reportDesigner_DevExpress.XtraCharts.RangeColorizer.LegendItemPattern',
            'SweepDirection': 'dx_reportDesigner_DevExpress.XtraCharts.PieSeriesViewBase.SweepDirection',
            'Panes': 'dx_reportDesigner_DevExpress.XtraCharts.XYDiagram2D.Panes',
            'MinAllowedSizePercentage': 'dx_reportDesigner_DevExpress.XtraCharts.PieSeriesView.MinAllowedSizePercentage',
            'Total Seconds': 'dx_reportDesigner_DevExpress.XtraCharts.TimeSpanFormat.TotalSeconds',
            'Show Value Labels': 'dx_reportDesigner_DevExpress.XtraCharts.CrosshairOptions.ShowValueLabels',
            'Child Point': 'dx_reportDesigner_DevExpress.XtraCharts.Relation.ChildPoint',
            'Marker 2': 'dx_reportDesigner_DevExpress.XtraCharts.RangeAreaSeriesView.Marker2',
            'Marker 1': 'dx_reportDesigner_DevExpress.XtraCharts.RangeAreaSeriesView.Marker1',
            'Right': 'dx_reportDesigner_DevExpress.XtraCharts.RectangleIndents.Right',
            'Value Duration Format': 'dx_reportDesigner_DevExpress.XtraCharts.RangePointOptions.ValueDurationFormat',
            'Tool Tip Point Pattern': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesBase.ToolTipPointPattern',
            'WorkdaysOnly': 'dx_reportDesigner_DevExpress.XtraCharts.DateTimeScaleOptions.WorkdaysOnly',
            'Hole Radius Percent': 'dx_reportDesigner_DevExpress.XtraCharts.Doughnut3DSeriesView.HoleRadiusPercent',
            'Polygon': 'dx_reportDesigner_DevExpress.XtraCharts.RadarDiagramDrawingStyle.Polygon',
            'Value 2': 'dx_reportDesigner_DevExpress.XtraCharts.ValueLevel.Value_2',
            'Value 1': 'dx_reportDesigner_DevExpress.XtraCharts.ValueLevel.Value_1',
            'Color2': 'dx_reportDesigner_DevExpress.XtraCharts.PaletteEntry.Color2',
            'Left to Right': 'dx_reportDesigner_DevExpress.XtraCharts.RadarAxisXLabelTextDirection.LeftToRight',
            'Runtime Moving': 'dx_reportDesigner_DevExpress.XtraCharts.Annotation.RuntimeMoving',
            'Bar Width': 'dx_reportDesigner_DevExpress.XtraCharts.BarSeriesView.BarWidth',
            'Line Thickness': 'dx_reportDesigner_DevExpress.XtraCharts.FinancialSeriesViewBase.LineThickness',
            'Near': 'dx_reportDesigner_DevExpress.XtraCharts.ConstantLineTitleAlignment.Near',
            'Labels': 'dx_reportDesigner_DevExpress.XtraCharts.FibonacciIndicator.Label',
            'Left Top': 'dx_reportDesigner_DevExpress.XtraCharts.DockCorner.LeftTop',
            'Notched Arrow': 'dx_reportDesigner_DevExpress.XtraCharts.AnnotationConnectorStyle.NotchedArrow',
            'Border Visible': 'dx_reportDesigner_DevExpress.XtraCharts.RadarDiagram.BorderVisible',
            'ApproximateColors': 'dx_reportDesigner_DevExpress.XtraCharts.RangeColorizer.ApproximateColors',
            'Max Value Label': 'dx_reportDesigner_DevExpress.XtraCharts.RangeBarLabelKind.MaxValueLabel',
            'Marker Visible': 'dx_reportDesigner_DevExpress.XtraCharts.Legend.MarkerVisible',
            'From Center': 'dx_reportDesigner_DevExpress.XtraCharts.PolygonGradientMode.FromCenter',
            'Dock Corner': 'dx_reportDesigner_DevExpress.XtraCharts.FreePosition.DockCorner',
            'Show Argument Line': 'dx_reportDesigner_DevExpress.XtraCharts.CrosshairOptions.ShowArgumentLine',
            'Counterclockwise': 'dx_reportDesigner_DevExpress.XtraCharts.PieSweepDirection.Counterclockwise',
            'Arrow': 'dx_reportDesigner_DevExpress.XtraCharts.AnnotationConnectorStyle.Arrow',
            'Pane Distance': 'dx_reportDesigner_DevExpress.XtraCharts.XYDiagram2D.PaneDistance',
            'Show Below Line': 'dx_reportDesigner_DevExpress.XtraCharts.ConstantLineTitle.ShowBelowLine',
            'Edge2': 'dx_reportDesigner_DevExpress.XtraCharts.ScaleBreak.Edge2',
            'Edge1': 'dx_reportDesigner_DevExpress.XtraCharts.ScaleBreak.Edge1',
            'Auto Format': 'dx_reportDesigner_DevExpress.XtraCharts.DateTimeOptions.AutoFormat',
            'Auto Grid': 'dx_reportDesigner_DevExpress.XtraCharts.ScaleOptionsBase.AutoGrid',
            'Straight': 'dx_reportDesigner_DevExpress.XtraCharts.ScaleBreakStyle.Straight',
            'Crosshair Axis Label Options': 'dx_reportDesigner_DevExpress.XtraCharts.Axis2D.CrosshairAxisLabelOptions',
            'Labels Visibility': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesBase.LabelsVisibility',
            'Retrieve Column Totals': 'dx_reportDesigner_DevExpress.XtraCharts.PivotGridDataSourceOptions.RetrieveColumnTotals',
            'Bar Distance Fixed': 'dx_reportDesigner_DevExpress.XtraCharts.SideBySideRangeBarSeriesView.BarDistanceFixed',
            'Pane Layout Direction': 'dx_reportDesigner_DevExpress.XtraCharts.XYDiagram2D.PaneLayoutDirection',
            'Layout Direction': 'dx_reportDesigner_DevExpress.XtraCharts.SimpleDiagram3D.LayoutDirection',
            'Manual': 'dx_reportDesigner_DevExpress.XtraCharts.ChartRangeControlClientSnapMode.Manual',
            'Max Size': 'dx_reportDesigner_DevExpress.XtraCharts.BubbleSeriesView.MaxSize',
            'Gradient Mode': 'dx_reportDesigner_DevExpress.XtraCharts.PolygonGradientFillOptions.GradientMode',
            'Argument and Values': 'dx_reportDesigner_DevExpress.XtraCharts.PointView.ArgumentAndValues',
            'Custom Measure Unit': 'dx_reportDesigner_DevExpress.XtraCharts.NumericScaleOptions.CustomMeasureUnit',
            'Rotation Type': 'dx_reportDesigner_DevExpress.XtraCharts.Diagram3D.RotationType',
            'Circle': 'dx_reportDesigner_DevExpress.XtraCharts.MarkerKind.Circle',
            'Automatic: Integral': 'dx_reportDesigner_DevExpress.XtraCharts.DateTimeScaleMode.AutomaticIntegral',
            'Enable Antialiasing': 'dx_reportDesigner_DevExpress.XtraCharts.TextAnnotation.EnableAntialiasing',
            'Retrieve Row Totals': 'dx_reportDesigner_DevExpress.XtraCharts.PivotGridDataSourceOptions.RetrieveRowTotals',
            'Automatic: Average': 'dx_reportDesigner_DevExpress.XtraCharts.DateTimeScaleMode.AutomaticAverage',
            'NumericOptions': 'dx_reportDesigner_DevExpress.XtraCharts.AxisLabel.NumericOptions',
            'Use Mouse': 'dx_reportDesigner_DevExpress.XtraCharts.ScrollingOptions.UseMouse',
            'High': 'dx_reportDesigner_DevExpress.XtraCharts.StockLevel.High',
            'Open': 'dx_reportDesigner_DevExpress.XtraCharts.StockLevel.Open',
            'SnapMode': 'dx_reportDesigner_DevExpress.XtraCharts.ChartRangeControlClientGridOptions.SnapMode',
            'Use Keyboard': 'dx_reportDesigner_DevExpress.XtraCharts.ScrollingOptions.UseKeyboard',
            'Connector Style': 'dx_reportDesigner_DevExpress.XtraCharts.Annotation.ConnectorStyle',
            'Less Than Or Equal': 'dx_reportDesigner_DevExpress.XtraCharts.DataFilterCondition.LessThanOrEqual',
            'Total Minutes': 'dx_reportDesigner_DevExpress.XtraCharts.TimeSpanFormat.TotalMinutes',
            'Item': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesPoint.Item',
            'Model': 'dx_reportDesigner_DevExpress.XtraCharts.Bar3DSeriesView.Model',
            'Auto Side Margins': 'dx_reportDesigner_DevExpress.XtraCharts.Range.AutoSideMargins',
            'Fill Mode': 'dx_reportDesigner_DevExpress.XtraCharts.FillStyle3D.FillMode',
            'Dash Dot': 'dx_reportDesigner_DevExpress.XtraCharts.DashStyle.DashDot',
            'Value Line Style': 'dx_reportDesigner_DevExpress.XtraCharts.CrosshairOptions.ValueLineStyle',
            'Min Limit': 'dx_reportDesigner_DevExpress.XtraCharts.Strip.MinLimit',
            'Alignment Vertical': 'dx_reportDesigner_DevExpress.XtraCharts.Legend.AlignmentVertical',
            'Key': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesPointFilter.Key',
            'Value Line Color': 'dx_reportDesigner_DevExpress.XtraCharts.CrosshairOptions.ValueLineColor',
            'Show Open Close': 'dx_reportDesigner_DevExpress.XtraCharts.StockSeriesView.ShowOpenClose',
            'Exploded Points Filters': 'dx_reportDesigner_DevExpress.XtraCharts.PieSeriesViewBase.ExplodedPointsFilters',
            'Right Column': 'dx_reportDesigner_DevExpress.XtraCharts.FunnelSeriesLabelPosition.RightColumn',
            'Custom Labels': 'dx_reportDesigner_DevExpress.XtraCharts.Axis2D.CustomLabels',
            'Always Show Zero Level': 'dx_reportDesigner_DevExpress.XtraCharts.VisualRange.AlwaysShowZeroLevel',
            'Staggered': 'dx_reportDesigner_DevExpress.XtraCharts.AxisLabel.Staggered',
            'Runtime Resizing': 'dx_reportDesigner_DevExpress.XtraCharts.Annotation.RuntimeResizing',
            'Automatic Layout Settings Enabled': 'dx_reportDesigner_DevExpress.XtraCharts.PivotGridDataSourceOptions.AutoLayoutSettingsEnabled',
            'Horizontal': 'dx_reportDesigner_DevExpress.XtraCharts.TextOrientation.Horizontal',
            'Show Argument Labels': 'dx_reportDesigner_DevExpress.XtraCharts.CrosshairOptions.ShowArgumentLabels',
            'Bar Thickness': 'dx_reportDesigner_DevExpress.XtraCharts.ScrollBarOptions.BarThickness',
            'Tuesday': 'dx_reportDesigner_DevExpress.XtraCharts.Weekday.Tuesday',
            'Min Size': 'dx_reportDesigner_DevExpress.XtraCharts.BubbleSeriesView.MinSize',
            'Argument Scale Type': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesBase.ArgumentScaleType',
            'Back Image': 'dx_reportDesigner_DevExpress.XtraCharts.XYDiagramPaneBase.BackImage',
            'Total Days': 'dx_reportDesigner_DevExpress.XtraCharts.TimeSpanFormat.TotalDays',
            'Use Mouse Advanced': 'dx_reportDesigner_DevExpress.XtraCharts.RotationType.UseMouseAdvanced',
            'AggregateFunction': 'dx_reportDesigner_DevExpress.XtraCharts.ScaleOptionsBase.AggregateFunction',
            'Hide Overlapped': 'dx_reportDesigner_DevExpress.XtraCharts.AxisLabelResolveOverlappingMode.HideOverlapped',
            'Marker 2 Visibility': 'dx_reportDesigner_DevExpress.XtraCharts.RangeAreaSeriesView.Marker2Visibility',
            'Long Date': 'dx_reportDesigner_DevExpress.XtraCharts.DateTimeFormat.LongDate',
            'Long Time': 'dx_reportDesigner_DevExpress.XtraCharts.DateTimeFormat.LongTime',
            'SnapSpacing': 'dx_reportDesigner_DevExpress.XtraCharts.ChartRangeControlClientGridOptions.SnapSpacing',
            'Radial': 'dx_reportDesigner_DevExpress.XtraCharts.PieSeriesLabelPosition.Radial',
            'Threshold Value': 'dx_reportDesigner_DevExpress.XtraCharts.TopNMode.ThresholdValue',
            'Crosshair Label Visibility': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesBase.CrosshairLabelVisibility',
            'GridOffset': 'dx_reportDesigner_DevExpress.XtraCharts.ChartRangeControlClientGridOptions.GridOffset',
            'Size as Percentage': 'dx_reportDesigner_DevExpress.XtraCharts.PieSeriesViewBase.SizeAsPercentage',
            'Start Angle in Degrees': 'dx_reportDesigner_DevExpress.XtraCharts.RadarDiagram.StartAngleInDegrees',
            'Argument Data Member': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesBase.ArgumentDataMember',
            'Shape Position': 'dx_reportDesigner_DevExpress.XtraCharts.Annotation.ShapePosition',
            'Far': 'dx_reportDesigner_DevExpress.XtraCharts.ConstantLineTitleAlignment.Far',
            'Dock Target': 'dx_reportDesigner_DevExpress.XtraCharts.CrosshairFreePosition.DockTarget',
            'Hundreds': 'dx_reportDesigner_DevExpress.XtraCharts.NumericMeasureUnit.Hundreds',
            'Points': 'dx_reportDesigner_DevExpress.XtraCharts.Series.Points',
            'Min Value Internal': 'dx_reportDesigner_DevExpress.XtraCharts.Range.MinValueInternal',
            'Base Level Line Style': 'dx_reportDesigner_DevExpress.XtraCharts.FibonacciIndicator.BaseLevelLineStyle',
            'Percent Options': 'dx_reportDesigner_DevExpress.XtraCharts.SimplePointOptions.PercentOptions',
            'Vertical Indent': 'dx_reportDesigner_DevExpress.XtraCharts.Legend.VerticalIndent',
            'Scale Break Options': 'dx_reportDesigner_DevExpress.XtraCharts.Axis.ScaleBreakOptions',
            'Bar Color': 'dx_reportDesigner_DevExpress.XtraCharts.ScrollBarOptions.BarColor',
            'Strips': 'dx_reportDesigner_DevExpress.XtraCharts.Axis2D.Strips',
            'Alignment Horizontal': 'dx_reportDesigner_DevExpress.XtraCharts.Legend.AlignmentHorizontal',
            'Hatch': 'dx_reportDesigner_DevExpress.XtraCharts.FillMode.Hatch',
            'Dash Style': 'dx_reportDesigner_DevExpress.XtraCharts.LineStyle.DashStyle',
            'Color Data Member': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesBase.ColorDataMember',
            'One Label': 'dx_reportDesigner_DevExpress.XtraCharts.RangeBarLabelKind.OneLabel',
            'Runtime Zooming': 'dx_reportDesigner_DevExpress.XtraCharts.Diagram3D.RuntimeZooming',
            'Marker 1 Visibility': 'dx_reportDesigner_DevExpress.XtraCharts.RangeAreaSeriesView.Marker1Visibility',
            'Series Distance': 'dx_reportDesigner_DevExpress.XtraCharts.XYDiagram3D.SeriesDistance',
            'Keys': 'dx_reportDesigner_DevExpress.XtraCharts.KeyColorColorizer.Keys',
            'Others Argument': 'dx_reportDesigner_DevExpress.XtraCharts.TopNOptions.OthersArgument',
            'Retrieve Empty Cells': 'dx_reportDesigner_DevExpress.XtraCharts.PivotGridDataSourceOptions.RetrieveEmptyCells',
            'Text Visible': 'dx_reportDesigner_DevExpress.XtraCharts.Legend.TextVisible',
            'Inside': 'dx_reportDesigner_DevExpress.XtraCharts.PieSeriesLabelPosition.Inside',
            'Use Touch Device': 'dx_reportDesigner_DevExpress.XtraCharts.RotationOptions.UseTouchDevice',
            'Show Facet': 'dx_reportDesigner_DevExpress.XtraCharts.Bar3DSeriesView.ShowFacet',
            'Values': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesPoint.Values',
            'Constant Lines': 'dx_reportDesigner_DevExpress.XtraCharts.Axis2D.ConstantLines',
            'Bottom to Top': 'dx_reportDesigner_DevExpress.XtraCharts.RadarAxisXLabelTextDirection.BottomToTop',
            'Axis Label Text': 'dx_reportDesigner_DevExpress.XtraCharts.Strip.AxisLabelText',
            'Y Axis Scroll Bar Alignment': 'dx_reportDesigner_DevExpress.XtraCharts.ScrollBarOptions.YAxisScrollBarAlignment',
            'Legend Text': 'dx_reportDesigner_DevExpress.XtraCharts.Strip.LegendText',
            'Resolve Overlapping Mode': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesLabelBase.ResolveOverlappingMode',
            'Finish To Start': 'dx_reportDesigner_DevExpress.XtraCharts.TaskLinkType.FinishToStart',
            'Others': 'dx_reportDesigner_DevExpress.XtraCharts.PieExplodeMode.Others',
            'Min Value Marker': 'dx_reportDesigner_DevExpress.XtraCharts.RangeBarSeriesView.MinValueMarker',
            'Drawing Style': 'dx_reportDesigner_DevExpress.XtraCharts.RadarDiagram.DrawingStyle',
            'Perspective Angle': 'dx_reportDesigner_DevExpress.XtraCharts.Diagram3D.PerspectiveAngle',
            'Right Top': 'dx_reportDesigner_DevExpress.XtraCharts.DockCorner.RightTop',
            'SnapAlignment': 'dx_reportDesigner_DevExpress.XtraCharts.ChartRangeControlClientDateTimeGridOptions.SnapAlignment',
            'Area Width': 'dx_reportDesigner_DevExpress.XtraCharts.Area3DSeriesView.AreaWidth',
            'Fibonacci Retracement': 'dx_reportDesigner_DevExpress.XtraCharts.FibonacciIndicatorKind.FibonacciRetracement',
            'Z Order': 'dx_reportDesigner_DevExpress.XtraCharts.Annotation.ZOrder',
            'Min Indent': 'dx_reportDesigner_DevExpress.XtraCharts.AxisLabelResolveOverlappingOptions.MinIndent',
            'Square': 'dx_reportDesigner_DevExpress.XtraCharts.MarkerKind.Square',
            'Text Pattern': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesLabelBase.TextPattern',
            'Holidays': 'dx_reportDesigner_DevExpress.XtraCharts.WorkdaysOptions.Holidays',
            'To Center': 'dx_reportDesigner_DevExpress.XtraCharts.PolygonGradientMode.ToCenter',
            'Dash': 'dx_reportDesigner_DevExpress.XtraCharts.DashStyle.Dash',
            'Max Allowed Series Count': 'dx_reportDesigner_DevExpress.XtraCharts.PivotGridDataSourceOptions.MaxAllowedSeriesCount',
            'Use Mouse Standard': 'dx_reportDesigner_DevExpress.XtraCharts.RotationType.UseMouseStandard',
            'Border 1': 'dx_reportDesigner_DevExpress.XtraCharts.RangeAreaSeriesView.Border1',
            'Border 2': 'dx_reportDesigner_DevExpress.XtraCharts.RangeAreaSeriesView.Border2',
            'Show Level 100%': 'dx_reportDesigner_DevExpress.XtraCharts.FibonacciIndicator.ShowLevel100',
            'Both': 'dx_reportDesigner_DevExpress.XtraCharts.StockType.Both',
            'Minor Visible': 'dx_reportDesigner_DevExpress.XtraCharts.TickmarksBase.MinorVisible',
            'Ragged': 'dx_reportDesigner_DevExpress.XtraCharts.ScaleBreakStyle.Ragged',
            'Justify All Around Point': 'dx_reportDesigner_DevExpress.XtraCharts.ResolveOverlappingMode.JustifyAllAroundPoint',
            'Start To Finish': 'dx_reportDesigner_DevExpress.XtraCharts.TaskLinkType.StartToFinish',
            'Gradient': 'dx_reportDesigner_DevExpress.XtraCharts.FillMode3D.Gradient',
            'All': 'dx_reportDesigner_DevExpress.XtraCharts.PieExplodeMode.All',
            'X Axis Scroll Bar Visible': 'dx_reportDesigner_DevExpress.XtraCharts.ScrollBarOptions.XAxisScrollBarVisible',
            'End Text': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesNameTemplate.EndText',
            'Max Value': 'dx_reportDesigner_DevExpress.XtraCharts.Range.MaxValue',
            'Own Color': 'dx_reportDesigner_DevExpress.XtraCharts.TaskLinkColorSource.OwnColor',
            'Interlaced Fill Style': 'dx_reportDesigner_DevExpress.XtraCharts.RadarAxis.InterlacedFillStyle',
            'Argument': 'dx_reportDesigner_DevExpress.XtraCharts.FinancialIndicatorPoint.Argument',
            'Extrapolate to Infinity': 'dx_reportDesigner_DevExpress.XtraCharts.TrendLine.ExtrapolateToInfinity',
            'Pane': 'dx_reportDesigner_DevExpress.XtraCharts.XYDiagramSeriesViewBase.Pane',
            'Line Color': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesLabelBase.LineColor',
            'Child Border Color': 'dx_reportDesigner_DevExpress.XtraCharts.TaskLinkColorSource.ChildBorderColor',
            'Custom Grid Alignment': 'dx_reportDesigner_DevExpress.XtraCharts.NumericScaleOptions.CustomGridAlignment',
            'Retrieve Column Grand Totals': 'dx_reportDesigner_DevExpress.XtraCharts.PivotGridDataSourceOptions.RetrieveColumnGrandTotals',
            'Value 3': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesPointKey.Value_3',
            'Value 4': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesPointKey.Value_4',
            'Chart Grid': 'dx_reportDesigner_DevExpress.XtraCharts.ChartRangeControlClientGridMode.ChartGrid',
            'Rotation': 'dx_reportDesigner_DevExpress.XtraCharts.PieSeriesView.Rotation',
            'Zoom': 'dx_reportDesigner_DevExpress.XtraCharts.ChartImageSizeMode.Zoom',
            'Tile': 'dx_reportDesigner_DevExpress.XtraCharts.ChartImageSizeMode.Tile',
            'Rotated': 'dx_reportDesigner_DevExpress.XtraCharts.XYDiagram.Rotated',
            'Dash Dot Dot': 'dx_reportDesigner_DevExpress.XtraCharts.DashStyle.DashDotDot',
            'Pie Fill Style': 'dx_reportDesigner_DevExpress.XtraCharts.Pie3DSeriesView.PieFillStyle',
            'Saturday': 'dx_reportDesigner_DevExpress.XtraCharts.Weekday.Saturday',
            'Color 2': 'dx_reportDesigner_DevExpress.XtraCharts.FillOptionsColor2Base.Color2',
            'KeyProvider': 'dx_reportDesigner_DevExpress.XtraCharts.KeyColorColorizer.KeyProvider',
            'Top To Bottom': 'dx_reportDesigner_DevExpress.XtraCharts.TextOrientation.TopToBottom',
            'Two Labels': 'dx_reportDesigner_DevExpress.XtraCharts.RangeAreaLabelKind.TwoLabels',
            'Funnel Fill Style': 'dx_reportDesigner_DevExpress.XtraCharts.Funnel3DSeriesView.FunnelFillStyle',
            'Value and Weight': 'dx_reportDesigner_DevExpress.XtraCharts.BubbleLabelValueToDisplay.ValueAndWeight',
            'Group Header Pattern': 'dx_reportDesigner_DevExpress.XtraCharts.CrosshairOptions.GroupHeaderPattern',
            'GridMode': 'dx_reportDesigner_DevExpress.XtraCharts.ChartRangeControlClientGridOptions.GridMode',
            'Automatic': 'dx_reportDesigner_DevExpress.XtraCharts.ScaleMode.Automatic',
            'Argument Line Color': 'dx_reportDesigner_DevExpress.XtraCharts.CrosshairOptions.ArgumentLineColor',
            'Zooming Options': 'dx_reportDesigner_DevExpress.XtraCharts.Diagram3D.ZoomingOptions',
            'Argument Line Style': 'dx_reportDesigner_DevExpress.XtraCharts.CrosshairOptions.ArgumentLineStyle',
            'Date-time Measure Unit': 'dx_reportDesigner_DevExpress.XtraCharts.AxisBase.DateTimeMeasureUnit',
            'Billions': 'dx_reportDesigner_DevExpress.XtraCharts.NumericGridAlignment.Billions',
            'Parent Border Color': 'dx_reportDesigner_DevExpress.XtraCharts.TaskLinkColorSource.ParentBorderColor',
            'Inverted Triangle': 'dx_reportDesigner_DevExpress.XtraCharts.MarkerKind.InvertedTriangle',
            'Undefined': 'dx_reportDesigner_DevExpress.XtraCharts.PointView.Undefined',
            'Zoom Percent': 'dx_reportDesigner_DevExpress.XtraCharts.Diagram3D.ZoomPercent',
            'Show Axis Label': 'dx_reportDesigner_DevExpress.XtraCharts.Strip.ShowAxisLabel',
            'RangeStops': 'dx_reportDesigner_DevExpress.XtraCharts.RangeColorizer.RangeStops',
            'Total Milliseconds': 'dx_reportDesigner_DevExpress.XtraCharts.TimeSpanFormat.TotalMilliseconds',
            'Logarithmic': 'dx_reportDesigner_DevExpress.XtraCharts.AxisBase.Logarithmic',
            'Left Column': 'dx_reportDesigner_DevExpress.XtraCharts.FunnelSeriesLabelPosition.LeftColumn',
            'Indent from Marker': 'dx_reportDesigner_DevExpress.XtraCharts.BubbleSeriesLabel.IndentFromMarker',
            'Legend Point Options': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesBase.LegendPointOptions',
            'Date Time': 'dx_reportDesigner_DevExpress.XtraCharts.ScaleType.DateTime',
            'GridSpacing': 'dx_reportDesigner_DevExpress.XtraCharts.ChartRangeControlClientGridOptions.GridSpacing',
            'Crosshair Enabled': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesBase.CrosshairEnabled',
            'Bar Depth Auto': 'dx_reportDesigner_DevExpress.XtraCharts.Bar3DSeriesView.BarDepthAuto',
            'Line Visible': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesLabelBase.LineVisible',
            'Crosshair Highlight Points': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesBase.CrosshairHighlightPoints',
            'Runtime Anchoring': 'dx_reportDesigner_DevExpress.XtraCharts.Annotation.RuntimeAnchoring',
            'Y Axis Scroll Bar Visible': 'dx_reportDesigner_DevExpress.XtraCharts.ScrollBarOptions.YAxisScrollBarVisible',
            'Visibility': 'dx_reportDesigner_DevExpress.XtraCharts.BorderBase.Visibility',
            'Bottom Left to Top Right': 'dx_reportDesigner_DevExpress.XtraCharts.PolygonGradientMode.BottomLeftToTopRight',
            'Base Level Color': 'dx_reportDesigner_DevExpress.XtraCharts.FibonacciIndicator.BaseLevelColor',
            'Insert Zero Values': 'dx_reportDesigner_DevExpress.XtraCharts.ProcessMissingPointsMode.InsertZeroValues',
            'Range': 'dx_reportDesigner_DevExpress.XtraCharts.AxisBase.Range',
            'Color Source': 'dx_reportDesigner_DevExpress.XtraCharts.TaskLinkOptions.ColorSource',
            'Min Value Label': 'dx_reportDesigner_DevExpress.XtraCharts.RangeBarLabelKind.MinValueLabel',
            'Point Distance': 'dx_reportDesigner_DevExpress.XtraCharts.FunnelSeriesViewBase.PointDistance',
            'Tool Tip Series Pattern': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesBase.ToolTipSeriesPattern',
            'Show Value Line': 'dx_reportDesigner_DevExpress.XtraCharts.CrosshairOptions.ShowValueLine',
            'Value As Duration': 'dx_reportDesigner_DevExpress.XtraCharts.RangePointOptions.ValueAsDuration',
            'Envelope Percent': 'dx_reportDesigner_DevExpress.XtraCharts.MovingAverage.EnvelopePercent',
            'Show Level 0%': 'dx_reportDesigner_DevExpress.XtraCharts.FibonacciIndicator.ShowLevel0',
            'Crosshair Label Mode': 'dx_reportDesigner_DevExpress.XtraCharts.CrosshairOptions.CrosshairLabelMode',
            'Begin Text': 'dx_reportDesigner_DevExpress.XtraCharts.AxisLabel.BeginText',
            'Maximum Value Label': 'dx_reportDesigner_DevExpress.XtraCharts.RangeAreaLabelKind.MaxValueLabel',
            'Use Points': 'dx_reportDesigner_DevExpress.XtraCharts.PieExplodeMode.UsePoints',
            'Fibonacci Fans': 'dx_reportDesigner_DevExpress.XtraCharts.FibonacciIndicatorKind.FibonacciFans',
            'Fibonacci Arcs': 'dx_reportDesigner_DevExpress.XtraCharts.FibonacciIndicatorKind.FibonacciArcs',
            'Bubble Marker Options': 'dx_reportDesigner_DevExpress.XtraCharts.BubbleSeriesView.BubbleMarkerOptions',
            'Triangle': 'dx_reportDesigner_DevExpress.XtraCharts.MarkerKind.Triangle',
            'Retrieve Data by Columns': 'dx_reportDesigner_DevExpress.XtraCharts.PivotGridDataSourceOptions.RetrieveDataByColumns',
            'AxisY Coordinate': 'dx_reportDesigner_DevExpress.XtraCharts.PaneAnchorPoint.AxisYCoordinate',
            'Plane Depth Fixed': 'dx_reportDesigner_DevExpress.XtraCharts.XYDiagram3D.PlaneDepthFixed',
            'Equal Pie Size': 'dx_reportDesigner_DevExpress.XtraCharts.SimpleDiagram.EqualPieSize',
            'General': 'dx_reportDesigner_DevExpress.XtraCharts.DateTimeFormat.General',
            'Low': 'dx_reportDesigner_DevExpress.XtraCharts.StockLevel.Low',
            'Outer Indents': 'dx_reportDesigner_DevExpress.XtraCharts.FreePosition.OuterIndents',
            'Column Indent': 'dx_reportDesigner_DevExpress.XtraCharts.PieSeriesLabel.ColumnIndent',
            'Summary Function': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesBase.SummaryFunction',
            'Percentage Accuracy': 'dx_reportDesigner_DevExpress.XtraCharts.PercentOptions.PercentageAccuracy',
            'Horizontal Scroll Percent': 'dx_reportDesigner_DevExpress.XtraCharts.Diagram3D.HorizontalScrollPercent',
            'Link Options': 'dx_reportDesigner_DevExpress.XtraCharts.GanttSeriesView.LinkOptions',
            'Argument Date-time Options': 'dx_reportDesigner_DevExpress.XtraCharts.PointOptions.ArgumentDateTimeOptions',
            'Show In Legend': 'dx_reportDesigner_DevExpress.XtraCharts.Indicator.ShowInLegend',
            'Use Scroll Bars': 'dx_reportDesigner_DevExpress.XtraCharts.ScrollingOptions2D.UseScrollBars',
            'Short Time': 'dx_reportDesigner_DevExpress.XtraCharts.DateTimeFormat.ShortTime',
            'Short Date': 'dx_reportDesigner_DevExpress.XtraCharts.DateTimeFormat.ShortDate',
            'Zero': 'dx_reportDesigner_DevExpress.XtraCharts.AxisAlignment.Zero',
            'Rotation Order': 'dx_reportDesigner_DevExpress.XtraCharts.Diagram3D.RotationOrder',
            'Relations': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesPoint.Relations',
            'Date-time Grid Alignment': 'dx_reportDesigner_DevExpress.XtraCharts.AxisBase.DateTimeGridAlignment',
            'Total Hours': 'dx_reportDesigner_DevExpress.XtraCharts.TimeSpanFormat.TotalHours',
            'Box': 'dx_reportDesigner_DevExpress.XtraCharts.Bar3DModel.Box',
            'WorkdaysOptions': 'dx_reportDesigner_DevExpress.XtraCharts.DateTimeScaleOptions.WorkdaysOptions',
            'Show Crosshair Labels': 'dx_reportDesigner_DevExpress.XtraCharts.CrosshairOptions.ShowCrosshairLabels',
            'Top Level': 'dx_reportDesigner_DevExpress.XtraCharts.RadarAxisY.TopLevel',
            'Cone': 'dx_reportDesigner_DevExpress.XtraCharts.Bar3DModel.Cone',
            'Scale Breaks': 'dx_reportDesigner_DevExpress.XtraCharts.Axis.ScaleBreaks',
            'Text Direction': 'dx_reportDesigner_DevExpress.XtraCharts.RadarAxisXLabel.TextDirection',
            'Threshold Percent': 'dx_reportDesigner_DevExpress.XtraCharts.TopNOptions.ThresholdPercent',
            'SnapOffset': 'dx_reportDesigner_DevExpress.XtraCharts.ChartRangeControlClientGridOptions.SnapOffset',
            'Equally Spaced Items': 'dx_reportDesigner_DevExpress.XtraCharts.Legend.EquallySpacedItems',
            'Reduction Options': 'dx_reportDesigner_DevExpress.XtraCharts.FinancialSeriesViewBase.ReductionOptions',
            'Automatic Binding Settings Enabled': 'dx_reportDesigner_DevExpress.XtraCharts.PivotGridDataSourceOptions.AutoBindingSettingsEnabled',
            'Envelope Color': 'dx_reportDesigner_DevExpress.XtraCharts.MovingAverage.EnvelopeColor',
            'Depth': 'dx_reportDesigner_DevExpress.XtraCharts.Pie3DSeriesView.Depth',
            'Horizontal Indent': 'dx_reportDesigner_DevExpress.XtraCharts.Legend.HorizontalIndent',
            'MaxValueAngle': 'dx_reportDesigner_DevExpress.XtraCharts.RangeAreaSeriesLabel.MaxValueAngle',
            'Quarter and Year': 'dx_reportDesigner_DevExpress.XtraCharts.DateTimeFormat.QuarterAndYear',
            'Tool Tip Enabled': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesBase.ToolTipEnabled',
            'Numeric Options': 'dx_reportDesigner_DevExpress.XtraCharts.AxisBase.NumericOptions',
            'Qualitative': 'dx_reportDesigner_DevExpress.XtraCharts.ScaleType.Qualitative',
            'Bar Depth': 'dx_reportDesigner_DevExpress.XtraCharts.Bar3DSeriesView.BarDepth',
            'Synchronize With Whole Range': 'dx_reportDesigner_DevExpress.XtraCharts.VisualRange.SynchronizeWithWholeRange',
            'Two Columns': 'dx_reportDesigner_DevExpress.XtraCharts.PieSeriesLabelPosition.TwoColumns',
            'Side Margins Value': 'dx_reportDesigner_DevExpress.XtraCharts.Range.SideMarginsValue',
            'Workdays Only': 'dx_reportDesigner_DevExpress.XtraCharts.AxisBase.WorkdaysOnly',
            'Thursday': 'dx_reportDesigner_DevExpress.XtraCharts.Weekday.Thursday',
            'Parent Point': 'dx_reportDesigner_DevExpress.XtraCharts.Relation.ParentPoint',
            'Size Mode': 'dx_reportDesigner_DevExpress.XtraCharts.ImageAnnotation.SizeMode',
            'Fixed Point': 'dx_reportDesigner_DevExpress.XtraCharts.NumericFormat.FixedPoint',
            'Bottom Outside': 'dx_reportDesigner_DevExpress.XtraCharts.LegendAlignmentVertical.BottomOutside',
            'MeasureUnit': 'dx_reportDesigner_DevExpress.XtraCharts.DateTimeScaleOptions.MeasureUnit',
            'Exact Workdays': 'dx_reportDesigner_DevExpress.XtraCharts.WorkdaysOptions.ExactWorkdays',
            'Vertical Scroll Percent': 'dx_reportDesigner_DevExpress.XtraCharts.Diagram3D.VerticalScrollPercent',
            'Dock': 'dx_reportDesigner_DevExpress.XtraCharts.DockableTitle.Dock',
            'Scrolling Range': 'dx_reportDesigner_DevExpress.XtraCharts.AxisRange.ScrollingRange',
            'Numeric Scale Options': 'dx_reportDesigner_DevExpress.XtraCharts.AxisBase.NumericScaleOptions',
            'Month and Year': 'dx_reportDesigner_DevExpress.XtraCharts.DateTimeFormat.MonthAndYear',
            'Series Name': 'dx_reportDesigner_DevExpress.XtraCharts.PointView.SeriesName',
            'AnchorPoint': 'dx_reportDesigner_DevExpress.XtraCharts.Annotation.AnchorPoint',
            'Bottom Right to Top Left': 'dx_reportDesigner_DevExpress.XtraCharts.PolygonGradientMode.BottomRightToTopLeft',
            'ShowInLegend': 'dx_reportDesigner_DevExpress.XtraCharts.PaletteColorizerBase.ShowInLegend',
            'Date-time Scale Mode': 'dx_reportDesigner_DevExpress.XtraCharts.SecondaryAxisX.DateTimeScaleMode',
            'Line Marker Options': 'dx_reportDesigner_DevExpress.XtraCharts.LineSeriesView.LineMarkerOptions',
            'Marker Line Color': 'dx_reportDesigner_DevExpress.XtraCharts.Line3DSeriesView.MarkerLineColor',
            'Inner Indent': 'dx_reportDesigner_DevExpress.XtraCharts.NestedDoughnutSeriesView.InnerIndent',
            'Legend Text Pattern': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesBase.LegendTextPattern',
            'Side Margins Enabled': 'dx_reportDesigner_DevExpress.XtraCharts.AxisRange.SideMarginsEnabled',
            'Marker Line Style': 'dx_reportDesigner_DevExpress.XtraCharts.Line3DSeriesView.MarkerLineStyle',
            'Series Points Sorting Key': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesBase.SeriesPointsSortingKey',
            'Palette': 'dx_reportDesigner_DevExpress.XtraCharts.PaletteColorizerBase.Palette',
            'Skip': 'dx_reportDesigner_DevExpress.XtraCharts.ProcessMissingPointsMode.Skip',
            'Marker Size': 'dx_reportDesigner_DevExpress.XtraCharts.Legend.MarkerSize',
            'Tool Tip Hint Data Member': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesBase.ToolTipHintDataMember',
            'Value DateTime Options': 'dx_reportDesigner_DevExpress.XtraCharts.PointOptions.ValueDateTimeOptions',
            'Align to Center': 'dx_reportDesigner_DevExpress.XtraCharts.FunnelSeriesView.AlignToCenter',
            'Point Options': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesLabelBase.PointOptions',
            'Top Inside': 'dx_reportDesigner_DevExpress.XtraCharts.BarSeriesLabelPosition.TopInside',
            'Highlight Points': 'dx_reportDesigner_DevExpress.XtraCharts.CrosshairOptions.HighlightPoints',
            'Max Count': 'dx_reportDesigner_DevExpress.XtraCharts.AutoScaleBreaks.MaxCount',
            'Height to Width Ratio Auto': 'dx_reportDesigner_DevExpress.XtraCharts.FunnelSeriesView.HeightToWidthRatioAuto',
            'Closed': 'dx_reportDesigner_DevExpress.XtraCharts.RadarLineSeriesView.Closed',
            'Show Group Headers': 'dx_reportDesigner_DevExpress.XtraCharts.CrosshairOptions.ShowGroupHeaders',
            'ValueProvider': 'dx_reportDesigner_DevExpress.XtraCharts.RangeColorizer.ValueProvider',
            'Month and Day': 'dx_reportDesigner_DevExpress.XtraCharts.DateTimeFormat.MonthAndDay',
            'Crosshair Label Pattern': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesBase.CrosshairLabelPattern',
            'Continuous': 'dx_reportDesigner_DevExpress.XtraCharts.ScaleMode.Continuous',
            'Date Time Values': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesPoint.DateTimeValues',
            'Link Type': 'dx_reportDesigner_DevExpress.XtraCharts.TaskLink.LinkType',
            'Inner Indents': 'dx_reportDesigner_DevExpress.XtraCharts.FreePosition.InnerIndents',
            'Date Time Options': 'dx_reportDesigner_DevExpress.XtraCharts.AxisBase.DateTimeOptions',
            'Tool Tip Hint': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesPoint.ToolTipHint',
            'Right to Left': 'dx_reportDesigner_DevExpress.XtraCharts.PolygonGradientMode.RightToLeft',
            'Max Value Marker Visibility': 'dx_reportDesigner_DevExpress.XtraCharts.RangeBarSeriesView.MaxValueMarkerVisibility',
            'Friday': 'dx_reportDesigner_DevExpress.XtraCharts.Weekday.Friday',
            'Argument Numeric Options': 'dx_reportDesigner_DevExpress.XtraCharts.PointOptions.ArgumentNumericOptions',
            'Workdays': 'dx_reportDesigner_DevExpress.XtraCharts.WorkdaysOptions.Workdays',
            'Allow Rotate': 'dx_reportDesigner_DevExpress.XtraCharts.AxisLabelResolveOverlappingOptions.AllowRotate',
            'Number': 'dx_reportDesigner_DevExpress.XtraCharts.NumericFormat.Number',
            'Bottom To Top': 'dx_reportDesigner_DevExpress.XtraCharts.TextOrientation.BottomToTop',
            'Value2 Label': 'dx_reportDesigner_DevExpress.XtraCharts.RangeAreaLabelKind.Value2Label',
            'Process Missing Points': 'dx_reportDesigner_DevExpress.XtraCharts.ScaleOptionsBase.ProcessMissingPoints',
            'Arrow Height': 'dx_reportDesigner_DevExpress.XtraCharts.TaskLinkOptions.ArrowHeight',
            'Show Behind': 'dx_reportDesigner_DevExpress.XtraCharts.ConstantLine.ShowBehind',
            'Value Scale Type': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesBase.ValueScaleType',
            'Value1 Label': 'dx_reportDesigner_DevExpress.XtraCharts.RangeAreaLabelKind.Value1Label',
            'RangeControlNumericGridOptions': 'dx_reportDesigner_DevExpress.XtraCharts.XYDiagram2D.RangeControlNumericGridOptions',
            'Connector Length': 'dx_reportDesigner_DevExpress.XtraCharts.RelativePosition.ConnectorLength',
            'AxisX Coordinate': 'dx_reportDesigner_DevExpress.XtraCharts.PaneAnchorPoint.AxisXCoordinate',
            'Title': 'dx_reportDesigner_DevExpress.XtraCharts.Axis2D.Title',
            'Text Orientation': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesLabelBase.TextOrientation',
            'Text Offset': 'dx_reportDesigner_DevExpress.XtraCharts.Legend.TextOffset',
            'Interlaced': 'dx_reportDesigner_DevExpress.XtraCharts.AxisBase.Interlaced',
            'Perspective Enabled': 'dx_reportDesigner_DevExpress.XtraCharts.Diagram3D.PerspectiveEnabled',
            'Points Count': 'dx_reportDesigner_DevExpress.XtraCharts.SubsetBasedIndicator.PointsCount',
            'Pentagon': 'dx_reportDesigner_DevExpress.XtraCharts.MarkerKind.Pentagon',
            'Runtime Scrolling': 'dx_reportDesigner_DevExpress.XtraCharts.Diagram3D.RuntimeScrolling',
            'Hexagon': 'dx_reportDesigner_DevExpress.XtraCharts.MarkerKind.Hexagon',
            'Value to Display': 'dx_reportDesigner_DevExpress.XtraCharts.BubbleSeriesLabel.ValueToDisplay',
            'Minor Line Style': 'dx_reportDesigner_DevExpress.XtraCharts.GridLines.MinorLineStyle',
            'Point Marker Options': 'dx_reportDesigner_DevExpress.XtraCharts.PointSeriesView.PointMarkerOptions',
            'Runtime Exploding': 'dx_reportDesigner_DevExpress.XtraCharts.PieSeriesView.RuntimeExploding',
            'Min Value Marker Visibility': 'dx_reportDesigner_DevExpress.XtraCharts.RangeBarSeriesView.MinValueMarkerVisibility',
            'Envelope': 'dx_reportDesigner_DevExpress.XtraCharts.MovingAverageKind.Envelope',
            'Waved': 'dx_reportDesigner_DevExpress.XtraCharts.ScaleBreakStyle.Waved',
            'Insert Empty Points': 'dx_reportDesigner_DevExpress.XtraCharts.ProcessMissingPointsMode.InsertEmptyPoints',
            'Use Filters': 'dx_reportDesigner_DevExpress.XtraCharts.PieExplodeMode.UseFilters',
            'Arrow Width': 'dx_reportDesigner_DevExpress.XtraCharts.TaskLinkOptions.ArrowWidth',
            'Data Type': 'dx_reportDesigner_DevExpress.XtraCharts.DataFilter.DataType',
            'Logarithmic Base': 'dx_reportDesigner_DevExpress.XtraCharts.AxisBase.LogarithmicBase',
            'Explode Mode': 'dx_reportDesigner_DevExpress.XtraCharts.PieSeriesViewBase.ExplodeMode',
            'Line Length': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesLabelBase.LineLength',
            'Single Page Only': 'dx_reportDesigner_DevExpress.XtraCharts.PivotGridDataSourceOptions.SinglePageOnly',
            'Equal': 'dx_reportDesigner_DevExpress.XtraCharts.DataFilterCondition.Equal',
            'Use Angles': 'dx_reportDesigner_DevExpress.XtraCharts.RotationType.UseAngles',
            'Use Size In Pixels': 'dx_reportDesigner_DevExpress.XtraCharts.PaneSizeMode.UseSizeInPixels',
            'LabelFormatProvider': 'dx_reportDesigner_DevExpress.XtraCharts.ChartRangeControlClientGridOptions.LabelFormatProvider',
            'To Center Vertical': 'dx_reportDesigner_DevExpress.XtraCharts.RectangleGradientMode.ToCenterVertical',
            'Pattern': 'dx_reportDesigner_DevExpress.XtraCharts.CrosshairAxisLabelOptions.Pattern',
            'Monday': 'dx_reportDesigner_DevExpress.XtraCharts.Weekday.Monday',
            'RangeControlDateTimeGridOptions': 'dx_reportDesigner_DevExpress.XtraCharts.XYDiagram2D.RangeControlDateTimeGridOptions',
            'LabelFormat': 'dx_reportDesigner_DevExpress.XtraCharts.ChartRangeControlClientGridOptions.LabelFormat',
            'Allow Hide': 'dx_reportDesigner_DevExpress.XtraCharts.AxisLabelResolveOverlappingOptions.AllowHide',
            'Moving Average': 'dx_reportDesigner_DevExpress.XtraCharts.MovingAverageKind.MovingAverage',
            'PaletteName': 'dx_reportDesigner_DevExpress.XtraCharts.PaletteColorizerBase.PaletteName',
            'Less Than': 'dx_reportDesigner_DevExpress.XtraCharts.DataFilterCondition.LessThan',
            'Snap Mode': 'dx_reportDesigner_DevExpress.XtraCharts.CrosshairOptions.SnapMode',
            'Cross': 'dx_reportDesigner_DevExpress.XtraCharts.MarkerKind.Cross',
            'Justify Around Point': 'dx_reportDesigner_DevExpress.XtraCharts.ResolveOverlappingMode.JustifyAroundPoint',
            'Value As Percent': 'dx_reportDesigner_DevExpress.XtraCharts.PercentOptions.ValueAsPercent',
            'Offset X': 'dx_reportDesigner_DevExpress.XtraCharts.ToolTipPositionWithOffset.OffsetX',
            'Offset Y': 'dx_reportDesigner_DevExpress.XtraCharts.ToolTipPositionWithOffset.OffsetY',
            'Grid Spacing Auto': 'dx_reportDesigner_DevExpress.XtraCharts.AxisBase.GridSpacingAuto',
            'Series Point': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesPointAnchorPoint.SeriesPoint',
            'Measure Unit': 'dx_reportDesigner_DevExpress.XtraCharts.NumericScaleOptions.MeasureUnit',
            'Point 2': 'dx_reportDesigner_DevExpress.XtraCharts.FinancialIndicator.Point2',
            'Point 1': 'dx_reportDesigner_DevExpress.XtraCharts.FinancialIndicator.Point1',
            'Series Points Sorting': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesBase.SeriesPointsSorting',
            'Greater Than Or Equal': 'dx_reportDesigner_DevExpress.XtraCharts.DataFilterCondition.GreaterThanOrEqual',
            'Grid Spacing': 'dx_reportDesigner_DevExpress.XtraCharts.ScaleOptionsBase.GridSpacing',
            'Visibility In Panes': 'dx_reportDesigner_DevExpress.XtraCharts.Axis2D.VisibilityInPanes',
            'Financial': 'dx_reportDesigner_DevExpress.XtraCharts.AggregateFunction.Financial',
            'Rectangle': 'dx_reportDesigner_DevExpress.XtraCharts.ShapeKind.Rectangle',
            'Work Days Options': 'dx_reportDesigner_DevExpress.XtraCharts.AxisBase.WorkdaysOptions',
            'Ellipse': 'dx_reportDesigner_DevExpress.XtraCharts.ShapeKind.Ellipse',
            'Greater Than': 'dx_reportDesigner_DevExpress.XtraCharts.DataFilterCondition.GreaterThan',
            'Sunday': 'dx_reportDesigner_DevExpress.XtraCharts.Weekday.Sunday',
            'Rotation Direction': 'dx_reportDesigner_DevExpress.XtraCharts.RadarDiagram.RotationDirection',
            'Show in a Legend': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesBase.ShowInLegend',
            'Star Point Count': 'dx_reportDesigner_DevExpress.XtraCharts.BaseMarker.StarPointCount',
            'Is Empty': 'dx_reportDesigner_DevExpress.XtraCharts.SeriesPoint.IsEmpty',
            'Series Distance Fixed': 'dx_reportDesigner_DevExpress.XtraCharts.XYDiagram3D.SeriesDistanceFixed',
            'Level Line Length': 'dx_reportDesigner_DevExpress.XtraCharts.FinancialSeriesViewBase.LevelLineLength',
            'Interlaced Color': 'dx_reportDesigner_DevExpress.XtraCharts.AxisBase.InterlacedColor',
            'Rounded Rectangle': 'dx_reportDesigner_DevExpress.XtraCharts.ShapeKind.RoundedRectangle',
            'Max Vertical Percentage': 'dx_reportDesigner_DevExpress.XtraCharts.Legend.MaxVerticalPercentage',
            'Retrieve Row Grand Totals': 'dx_reportDesigner_DevExpress.XtraCharts.PivotGridDataSourceOptions.RetrieveRowGrandTotals',
            'Left Bottom': 'dx_reportDesigner_DevExpress.XtraCharts.DockCorner.LeftBottom',
            'Wednesday': 'dx_reportDesigner_DevExpress.XtraCharts.Weekday.Wednesday',
            'X Axis Scroll Bar Alignment': 'dx_reportDesigner_DevExpress.XtraCharts.ScrollBarOptions.XAxisScrollBarAlignment',
            'Grid Lines': 'dx_reportDesigner_DevExpress.XtraCharts.AxisBase.GridLines',
            'ScaleMode': 'dx_reportDesigner_DevExpress.XtraCharts.ScaleOptionsBase.ScaleMode',
            'Cross Axis': 'dx_reportDesigner_DevExpress.XtraCharts.TickmarksBase.CrossAxis',
            'Max Value Marker': 'dx_reportDesigner_DevExpress.XtraCharts.RangeBarSeriesView.MaxValueMarker',
            'To Center Horizontal': 'dx_reportDesigner_DevExpress.XtraCharts.RectangleGradientMode.ToCenterHorizontal',
            'Japanese Envelope Chou Number 4': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.JapaneseEnvelopeChouNumber4',
            'Japanese Envelope Chou Number 3': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.JapaneseEnvelopeChouNumber3',
            'Expanded Stacked': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataBarType.ExpandedStacked',
            'TimeSpan': 'dx_reportDesigner_DevExpress.XtraReports.UI.FieldType.TimeSpan',
            'Charset A': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.Code128Charset.CharsetA',
            'Charset B': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.Code128Charset.CharsetB',
            'Charset C': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.Code128Charset.CharsetC',
            'Prc Envelope Number 8 Rotated': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.PrcEnvelopeNumber8Rotated',
            'Forward Diagonal': 'dx_reportDesigner_DevExpress.XtraPrinting.Drawing.DirectionMode.ForwardDiagonal',
            'Center Image': 'dx_reportDesigner_DevExpress.XtraPrinting.ImageSizeMode.CenterImage',
            'FNC1 Functional Character': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataMatrixGS1Generator.FNC1Substitute',
            'BCC': 'dx_reportDesigner_DevExpress.XtraPrinting.RecipientFieldType.BCC',
            'Expanded': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataBarType.Expanded',
            'HTML': 'dx_reportDesigner_DevExpress.Snap.Extensions.Native.ActionLists.TextFormat.HTML',
            'Remove Old Columns': 'dx_reportDesigner_DevExpress.Utils.OptionsColumnLayout.RemoveOldColumns',
            'Executive': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.Executive',
            'Italy Envelope': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.ItalyEnvelope',
            'Legal Extra': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.LegalExtra',
            'Export Mode': 'dx_reportDesigner_DevExpress.XtraPrinting.HtmlExportOptionsBase.ExportMode',
            'Human-Readable Text': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.EAN128Generator.HumanReadableText',
            'Prc Envelope Number 1 Rotated': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.PrcEnvelopeNumber1Rotated',
            'Zoom Image': 'dx_reportDesigner_DevExpress.XtraPrinting.ImageSizeMode.ZoomImage',
            'Stop Page Building': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.StopPageBuilding',
            'JIS B4 Rotated ': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.B4JisRotated',
            'String': 'dx_reportDesigner_DevExpress.XtraTreeList.Data.UnboundColumnType.String',
            'Different Files': 'dx_reportDesigner_DevExpress.XtraPrinting.XlsxExportMode.DifferentFiles',
            'Application': 'dx_reportDesigner_DevExpress.XtraPrinting.PdfDocumentOptions.Application',
            'RTF Export Options': 'dx_reportDesigner_DevExpress.XtraPrinting.RtfExportOptions',
            'Standard 15x11': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.Standard15x11',
            'Standard 12x11': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.Standard12x11',
            'Standard 10x14': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.Standard10x14',
            'Standard 10x11': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.Standard10x11',
            'Standard 11x17': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.Standard11x17',
            'Zoom to Page Width': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.ZoomToPageWidth',
            'US Standard Fanfold': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.USStandardFanfold',
            'Dash-Dot-Dot': 'dx_reportDesigner_DevExpress.XtraPrinting.BorderDashStyle.DashDotDot',
            '8 x 32': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataMatrixSize.Matrix8x32',
            '8 x 18': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataMatrixSize.Matrix8x18',
            'Prc 32K Big Rotated': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.Prc32KBigRotated',
            'Use Margins': 'dx_reportDesigner_DevExpress.XtraPrinting.PrinterSettingsUsing.UseMargins',
            'PDF/A-2b': 'dx_reportDesigner_DevExpress.XtraPrinting.PdfExportOptions.PdfACompatible',
            'Letter Small': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.LetterSmall',
            'No': 'dx_reportDesigner_DevExpress.Utils.DefaultBoolean.False',
            'Prc 16K Rotated': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.Prc16KRotated',
            'Convert Images to Jpeg': 'dx_reportDesigner_DevExpress.XtraPrinting.PdfExportOptions.ConvertImagesToJpeg',
            'Letter Extra': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.LetterExtra',
            'Tip\'s Length': 'dx_reportDesigner_DevExpress.XtraPrinting.Shape.ShapeBracket.TipLength',
            'Trellis': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.Trellis',
            'PDF Export Options': 'dx_reportDesigner_DevExpress.XtraPrinting.PdfExportOptions',
            'Y to X Ratio': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.PDF417Generator.YToXRatio',
            'Wide Upward Diagonal': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.WideUpwardDiagonal',
            'Fillet': 'dx_reportDesigner_DevExpress.XtraPrinting.Shape.ShapeBrace.Fillet',
            'Character Set': 'dx_reportDesigner_DevExpress.XtraPrinting.HtmlExportOptionsBase.CharacterSet',
            'Middle Right': 'dx_reportDesigner_DevExpress.XtraPrinting.TextAlignment.MiddleRight',
            'Wave': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.Wave',
            'Use Native Format': 'dx_reportDesigner_DevExpress.XtraPrinting.XlsExportOptions.UseNativeFormat',
            'Scroll Page Down': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.ScrollPageDown',
            'Percent 90': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.Percent90',
            'Percent 80': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.Percent80',
            'Percent 50': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.Percent50',
            'Percent 40': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.Percent40',
            'Percent 70': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.Percent70',
            'Percent 75': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.Percent75',
            'Percent 60': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.Percent60',
            'Percent 10': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.Percent10',
            'Percent 05': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.Percent05',
            'Percent 30': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.Percent30',
            'Percent 25': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.Percent25',
            'Percent 20': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.Percent20',
            'Japanese Envelope Kaku Number 3': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.JapaneseEnvelopeKakuNumber3',
            'Japanese Envelope Kaku Number 2': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.JapaneseEnvelopeKakuNumber2',
            'Separator': 'dx_reportDesigner_DevExpress.XtraPrinting.TextExportOptionsBase.Separator',
            'Page Range': 'dx_reportDesigner_DevExpress.XtraPrinting.HtmlExportOptionsBase.PageRange',
            'Show Grid Lines': 'dx_reportDesigner_DevExpress.XtraPrinting.XlsExportOptions.ShowGridLines',
            'XLS Export Options': 'dx_reportDesigner_DevExpress.XtraPrinting.XlsExportOptions',
            'Close Preview': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.ClosePreview',
            'Image Export Options': 'dx_reportDesigner_DevExpress.XtraPrinting.ImageExportOptions',
            'B5 Envelope': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.B5Envelope',
            'Japanese Postcard Rotated': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.JapanesePostcardRotated',
            'Dotted Grid': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.DottedGrid',
            'Large Checker Board': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.LargeCheckerBoard',
            'Save Mode': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintPreviewOptions.SaveMode',
            'Double': 'dx_reportDesigner_DevExpress.XtraReports.UI.FieldType.Double',
            'Single File': 'dx_reportDesigner_DevExpress.XtraPrinting.XlsxExportMode.SingleFile',
            'Show Prev Page': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.ShowPrevPage',
            'Modulo 10': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.MSICheckSum.Modulo10',
            'A4 Small': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.A4Small',
            'Personal Envelope (6 3/4)': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.PersonalEnvelope',
            'Zoom In': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.ZoomIn',
            'Integer': 'dx_reportDesigner_DevExpress.Data.UnboundColumnType.Integer',
            'A4 Extra': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.A4Extra',
            'Stacked Omnidirectional': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataBarType.StackedOmnidirectional',
            'HTML Export Options': 'dx_reportDesigner_DevExpress.XtraPrinting.HtmlExportOptions',
            'Alpha Numeric': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeCompactionMode.AlphaNumeric',
            'A5 Extra': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.A5Extra',
            'Dark Downward Diagonal': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.DarkDownwardDiagonal',
            'Backward Diagonal': 'dx_reportDesigner_DevExpress.XtraPrinting.Drawing.DirectionMode.BackwardDiagonal',
            'Hand Tool': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.HandTool',
            'Binary': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.PDF417CompactionMode.Binary',
            'EnableCopying': 'dx_reportDesigner_DevExpress.XtraPrinting.PdfPermissionsOptions.EnableCoping',
            'Show Last Page': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.ShowLastPage',
            'Large Confetti': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.LargeConfetti',
            'Store All Options': 'dx_reportDesigner_DevExpress.Utils.OptionsColumnLayout.StoreAllOptions',
            'Author': 'dx_reportDesigner_DevExpress.XtraPrinting.PdfDocumentOptions.Author',
            'B4 Envelope': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.B4Envelope',
            'Compaction Mode': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeGenerator.CompactionMode',
            'Version': 'dx_reportDesigner_DevExpress.XtraPrinting.XpsDocumentOptions.Version',
            'Security Options': 'dx_reportDesigner_DevExpress.XtraPrinting.PdfExportOptions.PasswordSecurityOptions',
            'Using a Save File Dialog': 'dx_reportDesigner_DevExpress.XtraPrinting.SaveMode.UsingSaveFileDialog',
            'Version 40 (177x177)': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeVersion.Version40',
            'Version 26 (121x121)': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeVersion.Version26',
            'Version 27 (125x125)': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeVersion.Version27',
            'Version 24 (113x113)': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeVersion.Version24',
            'Version 25 (117x117)': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeVersion.Version25',
            'Version 22 (105x105)': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeVersion.Version22',
            'Version 23 (109x109)': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeVersion.Version23',
            'Version 20 (97x97)': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeVersion.Version20',
            'Version 21 (101x101)': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeVersion.Version21',
            'Version 28 (129x129)': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeVersion.Version28',
            'Version 29 (133x133)': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeVersion.Version29',
            'Version 15 (77x77)': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeVersion.Version15',
            'Version 14 (73x73)': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeVersion.Version14',
            'Version 17 (85x85)': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeVersion.Version17',
            'Version 16 (81x81)': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeVersion.Version16',
            'Version 11 (61x61)': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeVersion.Version11',
            'Version 10 (57x57)': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeVersion.Version10',
            'Version 13 (69x69)': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeVersion.Version13',
            'Version 12 (65x65)': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeVersion.Version12',
            'Version 19 (93x93)': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeVersion.Version19',
            'Version 18 (89x89)': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeVersion.Version18',
            'Version 37 (165x165)': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeVersion.Version37',
            'Version 36 (161x161)': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeVersion.Version36',
            'Version 35 (157x157)': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeVersion.Version35',
            'Version 34 (153x153)': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeVersion.Version34',
            'Version 33 (149x149)': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeVersion.Version33',
            'Version 32 (145x145)': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeVersion.Version32',
            'Version 31 (141x141)': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeVersion.Version31',
            'Version 30 (137x137)': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeVersion.Version30',
            'Version 39 (173x173)': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeVersion.Version39',
            'Version 38 (169x169)': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeVersion.Version38',
            'Bound': 'dx_reportDesigner_DevExpress.Data.UnboundColumnType.Bound',
            'Tail\'s Length': 'dx_reportDesigner_DevExpress.XtraPrinting.Shape.ShapeBrace.TailLength',
            'Strikeout': 'dx_reportDesigner_System.Drawing.Font.Strikeout',
            'Save': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.Save',
            'Copy': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.Copy',
            'File': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.File',
            'Find': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.Find',
            'User Name': 'dx_reportDesigner_DevExpress.XtraPrinting.PageInfo.UserName',
            'Standard 9x11': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.Standard9x11',
            'Using Printer Settings': 'dx_reportDesigner_DevExpress.XtraPrinting.PrinterSettingsUsing',
            'A5 Transverse': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.A5Transverse',
            'Top Justify': 'dx_reportDesigner_DevExpress.XtraPrinting.TextAlignment.TopJustify',
            'Top Center': 'dx_reportDesigner_System.Drawing.ContentAlignment.TopCenter',
            'Matrix Size': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataMatrixGenerator.MatrixSize',
            'Smart': 'dx_reportDesigner_DevExpress.XtraPrinting.VerticalContentSplitting.Smart',
            'Send File': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.SendFile',
            'A3 Extra': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.A3Extra',
            'Send as XLSX': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.SendXlsx',
            'Exact': 'dx_reportDesigner_DevExpress.XtraPrinting.VerticalContentSplitting.Exact',
            'Export Watermarks': 'dx_reportDesigner_DevExpress.XtraPrinting.HtmlExportOptionsBase.ExportWatermarks',
            'Dark Vertical': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.DarkVertical',
            'Slant': 'dx_reportDesigner_DevExpress.XtraReports.UI.LineDirection.Slant',
            'Top Left': 'dx_reportDesigner_System.Drawing.ContentAlignment.TopLeft',
            'Underline': 'dx_reportDesigner_System.Drawing.Font.Underline',
            'Light Vertical': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.LightVertical',
            'Image Tiling': 'dx_reportDesigner_DevExpress.XtraPrinting.Drawing.PageWatermark.ImageTiling',
            'Start and Stop Symbols': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.CodabarGenerator.StartStopPair',
            'Never Embedded Fonts': 'dx_reportDesigner_DevExpress.XtraPrinting.PdfExportOptions.NeverEmbeddedFonts',
            'Pointer': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.Pointer',
            'Dark Upward Diagonal': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.DarkUpwardDiagonal',
            'Prc Envelope Number 6 Rotated': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.PrcEnvelopeNumber6Rotated',
            'Zoom Out': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.ZoomOut',
            'Export as Image': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.ExportGraphic',
            'Prc Envelope Number 10': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.PrcEnvelopeNumber10',
            'Middle Center': 'dx_reportDesigner_DevExpress.XtraPrinting.TextAlignment.MiddleCenter',
            'A3 Transverse': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.A3Transverse',
            'JIS B5 Transverse': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.B5Transverse',
            'X12': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataMatrixCompactionMode.X12',
            'C40': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataMatrixCompactionMode.C40',
            'Body': 'dx_reportDesigner_DevExpress.XtraPrinting.EmailOptions.Body',
            'Subject': 'dx_reportDesigner_DevExpress.XtraPrinting.XpsDocumentOptions.Subject',
            'Invite Envelope': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.InviteEnvelope',
            'Number of Sides': 'dx_reportDesigner_DevExpress.XtraPrinting.Shape.ShapePolygon.NumberOfSides',
            'Paper Size': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.PaperSize',
            'High Resolution': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingPermissions.HighResolution',
            'Send as Image': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.SendGraphic',
            'OpenPassword': 'dx_reportDesigner_DevExpress.XtraPrinting.PdfPasswordSecurityOptions.OpenPassword',
            'Customize': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.Customize',
            'Middle Justify': 'dx_reportDesigner_DevExpress.XtraPrinting.TextAlignment.MiddleJustify',
            'Scroll Page Up': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.ScrollPageUp',
            'Prc Envelope Number 9 Rotated': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.PrcEnvelopeNumber9Rotated',
            'A6': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.A6',
            'A4': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.A4',
            'A5': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.A5',
            'A2': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.A2',
            'A3': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.A3',
            'B5': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.B5',
            'B4': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.B4',
            'Suppress 65536 Rows Warning': 'dx_reportDesigner_DevExpress.XtraPrinting.XlsExportOptions.Suppress65536RowsWarning',
            'Pixel Format': 'dx_reportDesigner_System.Drawing.Image.PixelFormat',
            'Int32': 'dx_reportDesigner_DevExpress.XtraReports.UI.FieldType.Int32',
            'Int16': 'dx_reportDesigner_DevExpress.XtraReports.UI.FieldType.Int16',
            'Monarch Envelope': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.MonarchEnvelope',
            'Float': 'dx_reportDesigner_DevExpress.XtraReports.UI.FieldType.Float',
            'Omnidirectional': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataBarType.Omnidirectional',
            'C3 Envelope': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.C3Envelope',
            'Large Grid': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.LargeGrid',
            '144 x 144': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataMatrixSize.Matrix144x144',
            'Dashed Horizontal': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.DashedHorizontal',
            'Auto-Size': 'dx_reportDesigner_DevExpress.XtraPrinting.ImageSizeMode.AutoSize',
            '120 x 120': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataMatrixSize.Matrix120x120',
            'Inserting Deleting Rotating': 'dx_reportDesigner_DevExpress.XtraPrinting.ChangingPermissions.InsertingDeletingRotating',
            'Skip Empty Rows': 'dx_reportDesigner_DevExpress.XtraPrinting.CsvExportOptions.SkipEmptyRows',
            'Dashed Vertical': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.DashedVertical',
            'Japanese Postcard': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.JapanesePostcard',
            'Boolean': 'dx_reportDesigner_DevExpress.XtraReports.UI.FieldType.Boolean',
            'Page Border Color': 'dx_reportDesigner_DevExpress.XtraPrinting.HtmlExportOptionsBase.PageBorderColor',
            'C6 Envelope': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.C6Envelope',
            'Page Count': 'dx_reportDesigner_DevExpress.XtraPrinting.PageInfo.Total',
            'Page Border Width': 'dx_reportDesigner_DevExpress.XtraPrinting.HtmlExportOptionsBase.PageBorderWidth',
            '104 x 104': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataMatrixSize.Matrix104x104',
            'Number 12 Envelope': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.Number12Envelope',
            'A3 Extra Transverse': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.A3ExtraTransverse',
            'A5 Rotated': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.A5Rotated',
            '132 x 132': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataMatrixSize.Matrix132x132',
            'Small Checker Board': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.SmallCheckerBoard',
            'Export as XLSX': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.ExportXlsx',
            'Export File': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.ExportFile',
            'Additional Recipients': 'dx_reportDesigner_DevExpress.XtraPrinting.EmailOptions.AdditionalRecipients',
            'MHT Export Options': 'dx_reportDesigner_DevExpress.XtraPrinting.MhtExportOptions',
            'Document Options': 'dx_reportDesigner_DevExpress.XtraPrinting.XpsExportOptions.DocumentOptions',
            'Legal': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.Legal',
            'Iso B4': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.IsoB4',
            'Folio': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.Folio',
            'JIS B6': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.B6Jis',
            'SuperB/SuperB/A3': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.BPlus',
            'SuperA/SuperA/A4': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.APlus',
            'Truncated': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataBarType.Truncated',
            'Dark Horizontal': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.DarkHorizontal',
            'Image Quality': 'dx_reportDesigner_DevExpress.XtraPrinting.PdfExportOptions.ImageQuality',
            'Show Next Page': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.ShowNextPage',
            'Prc Envelope Number 4 Rotated': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.PrcEnvelopeNumber4Rotated',
            'Middle Left': 'dx_reportDesigner_DevExpress.XtraPrinting.TextAlignment.MiddleLeft',
            'PDF Document Options': 'dx_reportDesigner_DevExpress.XtraPrinting.PdfDocumentOptions',
            'Sheet Name': 'dx_reportDesigner_DevExpress.XtraPrinting.XlsExportOptions.SheetName',
            'Version 4 (33x33)': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeVersion.Version4',
            'Version 5 (37x37)': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeVersion.Version5',
            'Version 6 (41x41)': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeVersion.Version6',
            'Version 7 (45x45)': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeVersion.Version7',
            'Version 1 (21x21)': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeVersion.Version1',
            'Version 2 (25x25)': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeVersion.Version2',
            'Version 3 (29x29)': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeVersion.Version3',
            'Version 8 (49x49)': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeVersion.Version8',
            'Version 9 (53x53)': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeVersion.Version9',
            'Page Layout Continuous': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.PageLayoutContinuous',
            'Using a Default Path': 'dx_reportDesigner_DevExpress.XtraPrinting.SaveMode.UsingDefaultPath',
            'Wide Narrow Ratio': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.Interleaved2of5Generator.WideNarrowRatio',
            'Keep Size': 'dx_reportDesigner_DevExpress.Snap.Core.Fields.UpdateMergeImageFieldMode.KeepSize',
            'CSV Export Options': 'dx_reportDesigner_DevExpress.XtraPrinting.CsvExportOptions',
            'Horizontal Brick': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.HorizontalBrick',
            'All Pages': 'dx_reportDesigner_DevExpress.XtraReports.UI.PrintOnPages.AllPages',
            'Outlined Diamond': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.OutlinedDiamond',
            'Ignore 256 Columns Limit': 'dx_reportDesigner_DevExpress.XtraPrinting.XlsExportOptions.Ignore256ColumnsLimit',
            'Skip Empty Columns': 'dx_reportDesigner_DevExpress.XtraPrinting.CsvExportOptions.SkipEmptyColumns',
            'Top Right': 'dx_reportDesigner_System.Drawing.ContentAlignment.TopRight',
            'Letter Plus': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.LetterPlus',
            'Description': 'dx_reportDesigner_DevExpress.XtraPrinting.XpsDocumentOptions.Description',
            'Recipient Name': 'dx_reportDesigner_DevExpress.XtraPrinting.EmailOptions.RecipientName',
            'Zoom to Two Pages': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.ZoomToTwoPages',
            'Horizontal Line Width': 'dx_reportDesigner_DevExpress.XtraPrinting.Shape.ShapeCross.HorizontalLineWidth',
            'PermissionsOptions': 'dx_reportDesigner_DevExpress.XtraPrinting.PdfPasswordSecurityOptions.PermissionsOptions',
            'Ledger': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.Ledger',
            'Letter': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.Letter',
            'Japanese Envelope Chou Number 4 Rotated': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.JapaneseEnvelopeChouNumber4Rotated',
            'Text Export Options': 'dx_reportDesigner_DevExpress.XtraPrinting.TextExportOptions',
            'Error Correction Level': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeGenerator.ErrorCorrectionLevel',
            'Vertical Line Width': 'dx_reportDesigner_DevExpress.XtraPrinting.Shape.ShapeCross.VerticalLineWidth',
            'Number 11 Envelope': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.Number11Envelope',
            'Statement': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.Statement',
            'Bottom Center': 'dx_reportDesigner_System.Drawing.ContentAlignment.BottomCenter',
            'Keywords': 'dx_reportDesigner_DevExpress.XtraPrinting.XpsDocumentOptions.Keywords',
            'Bottom Right': 'dx_reportDesigner_DevExpress.XtraPrinting.ImageAlignment.BottomRight',
            'Action After Export': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintPreviewOptions.ActionAfterExport',
            'Signature Options': 'dx_reportDesigner_DevExpress.XtraPrinting.PdfExportOptions.SignatureOptions',
            'ISO B5 Extra': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.B5Extra',
            'Horizontal Resolution': 'dx_reportDesigner_System.Drawing.Image.HorizontalResolution',
            'PrintingPermissions': 'dx_reportDesigner_DevExpress.XtraPrinting.PdfPermissionsOptions.PrintingPermissions',
            'Workbook Color Palette Compliance': 'dx_reportDesigner_DevExpress.XtraPrinting.XlsExportOptions.WorkbookColorPaletteCompliance',
            'A6 Rotated': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.A6Rotated',
            'GDI Vertical Font': 'dx_reportDesigner_System.Drawing.Font.GdiVerticalFont',
            'Light Horizontal': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.LightHorizontal',
            'Multiple Pages': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.MultiplePages',
            'Not with Report Header and Report Footer': 'dx_reportDesigner_DevExpress.XtraReports.UI.PrintOnPages.NotWithReportHeaderAndReportFooter',
            'Export as XLS': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.ExportXls',
            'Export as XPS': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.ExportXps',
            'Export as TXT': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.ExportTxt',
            'Export as PDF': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.ExportPdf',
            'Export as RTF': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.ExportRtf',
            'Export as MHT': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.ExportMht',
            'Export as HTML': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.ExportHtm',
            'Export as CSV': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.ExportCsv',
            'Ignore 65536 Rows Limit': 'dx_reportDesigner_DevExpress.XtraPrinting.XlsExportOptions.Ignore65536RowsLimit',
            'Weave': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.Weave',
            'XLSx Export Options': 'dx_reportDesigner_DevExpress.XtraPrinting.XlsxExportOptions',
            'Japanese Envelope Chou Number 3 Rotated': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.JapaneseEnvelopeChouNumber3Rotated',
            'Plaid': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.Plaid',
            'JIS B5 Rotated': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.B5JisRotated',
            'Store Appearance': 'dx_reportDesigner_DevExpress.Utils.OptionsColumnLayout.StoreAppearance',
            'Unit': 'dx_reportDesigner_System.Drawing.Font.Unit',
            'Divot': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.Divot',
            'Bold': 'dx_reportDesigner_System.Drawing.Font.Bold',
            'Quote Strings with Separators': 'dx_reportDesigner_DevExpress.XtraPrinting.TextExportOptionsBase.QuoteStringsWithSeparators',
            'Bottom Left': 'dx_reportDesigner_DevExpress.XtraPrinting.TextAlignment.BottomLeft',
            'D Sheet': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.DSheet',
            'Submit Parameters': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.SubmitParameters',
            'Prc Envelope Number 7 Rotated': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.PrcEnvelopeNumber7Rotated',
            'Raw Format': 'dx_reportDesigner_System.Drawing.Image.RawFormat',
            'Current Date and Time': 'dx_reportDesigner_DevExpress.XtraPrinting.PageInfo.DateTime',
            'Keep Scale': 'dx_reportDesigner_DevExpress.Snap.Core.Fields.UpdateMergeImageFieldMode.KeepScale',
            'C Sheet': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.CSheet',
            'Single File (Page-by-Page)': 'dx_reportDesigner_DevExpress.XtraPrinting.HtmlExportMode.SingleFilePageByPage',
            'Page Orientation': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.PageOrientation',
            'Rotate to the Right': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.BarCodeOrientation.RotateRight',
            'Small Confetti': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.SmallConfetti',
            'Tabloid Extra': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.TabloidExtra',
            'E Sheet': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.ESheet',
            'Page Setup': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.PageSetup',
            'A4 Plus': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.A4Plus',
            'Japanese Envelope You Number 4': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.JapaneseEnvelopeYouNumber4',
            'Low Resolution': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingPermissions.LowResolution',
            'Upside Down': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.BarCodeOrientation.UpsideDown',
            'Normal': 'dx_reportDesigner_DevExpress.XtraPrinting.ImageSizeMode.Normal',
            'RTF': 'dx_reportDesigner_DevExpress.Snap.Extensions.Native.ActionLists.TextFormat.RTF',
            'MHT': 'dx_reportDesigner_DevExpress.Snap.Extensions.Native.ActionLists.TextFormat.MHT',
            'DOC': 'dx_reportDesigner_DevExpress.Snap.Extensions.Native.ActionLists.TextFormat.DOC',
            'Encoding': 'dx_reportDesigner_DevExpress.XtraPrinting.TextExportOptionsBase.Encoding',
            'Wrap': 'dx_reportDesigner_DevExpress.Utils.WordWrap.Wrap',
            'Add New Columns': 'dx_reportDesigner_DevExpress.Utils.OptionsColumnLayout.AddNewColumns',
            'ASCII': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataMatrixCompactionMode.ASCII',
            'C65 Envelope': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.C65Envelope',
            'Stretch Image': 'dx_reportDesigner_DevExpress.XtraPrinting.ImageSizeMode.StretchImage',
            'Byte': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeCompactionMode.Byte',
            'Type': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataBarGenerator.Type',
            'Sphere': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.Sphere',
            'Reduce Palette For Exact Colors': 'dx_reportDesigner_DevExpress.XtraPrinting.WorkbookColorPaletteCompliance.ReducePaletteForExactColors',
            'Suppress 256 Columns Warning': 'dx_reportDesigner_DevExpress.XtraPrinting.XlsExportOptions.Suppress256ColumnsWarning',
            'Zoom Track Bar': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.ZoomTrackBar',
            'Default Resource File': 'dx_reportDesigner_DevExpress.XtraPrinting.PdfExportOptions.DefaultResourceFile',
            'Japanese Double Postcard Rotated': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.JapaneseDoublePostcardRotated',
            'Dashed Downward Diagonal': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.DashedDownwardDiagonal',
            'Prc Envelope Number 2 Rotated': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.PrcEnvelopeNumber2Rotated',
            'Magnifier': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.Magnifier',
            'PdfPermissionsOptions': 'dx_reportDesigner_DevExpress.XtraPrinting.PdfPasswordSecurityOptions.PdfPermissionsOptions',
            'Number 9 Envelope': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.Number9Envelope',
            'Remove Secondary Symbols': 'dx_reportDesigner_DevExpress.XtraPrinting.HtmlExportOptionsBase.RemoveSecondarySymbols',
            'Store Layout': 'dx_reportDesigner_DevExpress.Utils.OptionsColumnLayout.StoreLayout',
            'A4 Rotated': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.A4Rotated',
            'ChangingPermissions': 'dx_reportDesigner_DevExpress.XtraPrinting.PdfPermissionsOptions.ChangingPermissions',
            'C4 Envelope': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.C4Envelope',
            'Native Format Options': 'dx_reportDesigner_DevExpress.XtraPrinting.NativeFormatOptions',
            'Prc 32K Rotated': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.Prc32KRotated',
            'Number 10 Envelope': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.Number10Envelope',
            'PermissionsPassword': 'dx_reportDesigner_DevExpress.XtraPrinting.PdfPasswordSecurityOptions.PermissionsPassword',
            'Show Options Dialog Before Export': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintPreviewOptions.ShowOptionsBeforeExport',
            'Back Slant': 'dx_reportDesigner_DevExpress.XtraReports.UI.LineDirection.BackSlant',
            'Japanese Envelope Kaku Number 2 Rotated': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.JapaneseEnvelopeKakuNumber2Rotated',
            'Ask a User': 'dx_reportDesigner_DevExpress.XtraPrinting.ActionAfterExport.AskUser',
            'Light Upward Diagonal': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.LightUpwardDiagonal',
            'Limited': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataBarType.Limited',
            'A4 Transverse': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.A4Transverse',
            'Yes': 'dx_reportDesigner_DevExpress.Utils.DefaultBoolean.True',
            'Stacked': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataBarType.Stacked',
            'Recipient Address': 'dx_reportDesigner_DevExpress.XtraPrinting.EmailOptions.RecipientAddress',
            'Prc Envelope Number 5 Rotated': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.PrcEnvelopeNumber5Rotated',
            'Small Grid': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.SmallGrid',
            'Hyperlink': 'dx_reportDesigner_DevExpress.Snap.Extensions.Native.ActionLists.ContentType.Hyperlink',
            'Japanese Envelope You Number 4 Rotated': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.JapaneseEnvelopeYouNumber4Rotated',
            'Double Modulo 10': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.MSICheckSum.DoubleModulo10',
            'View Whole Page': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.ViewWholePage',
            'WordML': 'dx_reportDesigner_DevExpress.Snap.Extensions.Native.ActionLists.TextFormat.WordML',
            'Compression': 'dx_reportDesigner_DevExpress.XtraPrinting.XpsExportOptions.Compression',
            'Embed Images In HTML': 'dx_reportDesigner_DevExpress.XtraPrinting.HtmlExportOptions.EmbedImagesInHTML',
            'German Legal Fanfold': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.GermanLegalFanfold',
            'Image View Mode': 'dx_reportDesigner_DevExpress.XtraPrinting.Drawing.PageWatermark.ImageViewMode',
            'OpenDocument': 'dx_reportDesigner_DevExpress.Snap.Extensions.Native.ActionLists.TextFormat.OpenDocument',
            'EnableScreenReaders': 'dx_reportDesigner_DevExpress.XtraPrinting.PdfPermissionsOptions.EnableScreenReaders',
            'Print Direct': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.PrintDirect',
            'Prc 32K': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.Prc32K',
            'Prc 16K': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.Prc16K',
            'Adjust Colors To Default Palette': 'dx_reportDesigner_DevExpress.XtraPrinting.WorkbookColorPaletteCompliance.AdjustColorsToDefaultPalette',
            'Prc Envelope Number 10 Rotated': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.PrcEnvelopeNumber10Rotated',
            'Quarto': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.Quarto',
            'Decimal': 'dx_reportDesigner_DevExpress.Data.UnboundColumnType.Decimal',
            'DateTime': 'dx_reportDesigner_DevExpress.XtraTreeList.Data.UnboundColumnType.DateTime',
            'Diagonal Cross': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.DiagonalCross',
            'Diagonal Brick': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.DiagonalBrick',
            'Commenting Filling Signing': 'dx_reportDesigner_DevExpress.XtraPrinting.ChangingPermissions.CommentingFillingSigning',
            'Use Paper Kind': 'dx_reportDesigner_DevExpress.XtraPrinting.PrinterSettingsUsing.UsePaperKind',
            'Dotted Diamond': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.DottedDiamond',
            'Zig Zag': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.ZigZag',
            'Page Number': 'dx_reportDesigner_DevExpress.XtraPrinting.PageInfo.Number',
            'Zoom to Whole Page': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.ZoomToWholePage',
            'Count of Star Points': 'dx_reportDesigner_DevExpress.XtraPrinting.Shape.ShapeStar.StarPointCount',
            'Page Layout Facing': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.PageLayoutFacing',
            'Filling Signing': 'dx_reportDesigner_DevExpress.XtraPrinting.ChangingPermissions.FillingSigning',
            '80 x 80': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataMatrixSize.Matrix80x80',
            '88 x 88': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataMatrixSize.Matrix88x88',
            '96 x 96': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataMatrixSize.Matrix96x96',
            '64 x 64': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataMatrixSize.Matrix64x64',
            '72 x 72': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataMatrixSize.Matrix72x72',
            '48 x 48': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataMatrixSize.Matrix48x48',
            '40 x 40': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataMatrixSize.Matrix40x40',
            '44 x 44': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataMatrixSize.Matrix44x44',
            '52 x 52': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataMatrixSize.Matrix52x52',
            '26 x 26': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataMatrixSize.Matrix26x26',
            '24 x 24': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataMatrixSize.Matrix24x24',
            '22 x 22': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataMatrixSize.Matrix22x22',
            '20 x 20': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataMatrixSize.Matrix20x20',
            '36 x 36': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataMatrixSize.Matrix36x36',
            '32 x 32': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataMatrixSize.Matrix32x32',
            '16 x 16': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataMatrixSize.Matrix16x16',
            '16 x 48': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataMatrixSize.Matrix16x48',
            '16 x 36': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataMatrixSize.Matrix16x36',
            '18 x 18': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataMatrixSize.Matrix18x18',
            '12 x 26': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataMatrixSize.Matrix12x26',
            '12 x 12': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataMatrixSize.Matrix12x12',
            '12 x 36': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataMatrixSize.Matrix12x36',
            '14 x 14': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataMatrixSize.Matrix14x14',
            '10 x 10': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataMatrixSize.Matrix10x10',
            'Letter Rotated': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.LetterRotated',
            'Medium': 'dx_reportDesigner_DevExpress.XtraPrinting.PdfJpegImageQuality.Medium',
            'Truncate Symbol': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.PDF417Generator.TruncateSymbol',
            'Solid Diamond': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.SolidDiamond',
            'Lowest': 'dx_reportDesigner_DevExpress.XtraPrinting.PdfJpegImageQuality.Lowest',
            'Text Export Mode': 'dx_reportDesigner_DevExpress.XtraPrinting.TextExportOptionsBase.TextExportMode',
            'Physical Dimension': 'dx_reportDesigner_System.Drawing.Image.PhysicalDimension',
            'Recipient Address Prefix': 'dx_reportDesigner_DevExpress.XtraPrinting.EmailOptions.RecipientAddressPrefix',
            'Light Downward Diagonal': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.LightDownwardDiagonal',
            'Default Send Format': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintPreviewOptions.DefaultSendFormat',
            'Creator': 'dx_reportDesigner_DevExpress.XtraPrinting.XpsDocumentOptions.Creator',
            'DE': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.CodabarStartStopPair.DE',
            'AT': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.CodabarStartStopPair.AT',
            'BN': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.CodabarStartStopPair.BN',
            'Image Transparency': 'dx_reportDesigner_DevExpress.XtraPrinting.Drawing.PageWatermark.ImageTransparency',
            'Shingle': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.Shingle',
            'Wide Downward Diagonal': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.WideDownwardDiagonal',
            'Text Transparency': 'dx_reportDesigner_DevExpress.XtraPrinting.Drawing.PageWatermark.TextTransparency',
            'Show Print Dialog on Open': 'dx_reportDesigner_DevExpress.XtraPrinting.PdfExportOptions.ShowPrintDialogOnOpen',
            'Vertical Resolution': 'dx_reportDesigner_System.Drawing.Image.VerticalResolution',
            'Default Export Format': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintPreviewOptions.DefaultExportFormat',
            'Narrow Horizontal': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.NarrowHorizontal',
            'Concavity': 'dx_reportDesigner_DevExpress.XtraPrinting.Shape.ShapeStar.Concavity',
            'Edit Page H F': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.EditPageHF',
            'A3 Rotated': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.A3Rotated',
            'Clip': 'dx_reportDesigner_DevExpress.XtraPrinting.Drawing.ImageViewMode.Clip',
            'OpenXML': 'dx_reportDesigner_DevExpress.Snap.Extensions.Native.ActionLists.TextFormat.OpenXML',
            'Default Directory': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintPreviewOptions.DefaultDirectory',
            'Background': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.Background',
            'Document Map': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.DocumentMap',
            'C5 Envelope': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.C5Envelope',
            'Page Border\'s Color': 'dx_reportDesigner_DevExpress.XtraPrinting.ImageExportOptions.PageBorderColor',
            'Compressed': 'dx_reportDesigner_DevExpress.XtraPrinting.NativeFormatOptions.Compressed',
            'Dashed Upward Diagonal': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.DashedUpwardDiagonal',
            'Page Border\'s Width': 'dx_reportDesigner_DevExpress.XtraPrinting.ImageExportOptions.PageBorderWidth',
            'Zoom to Text Width': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.ZoomToTextWidth',
            'Print Preview Options': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintPreviewOptions',
            'Table Layout': 'dx_reportDesigner_DevExpress.XtraPrinting.HtmlExportOptionsBase.TableLayout',
            'Squeeze': 'dx_reportDesigner_DevExpress.XtraPrinting.ImageSizeMode.Squeeze',
            'CC': 'dx_reportDesigner_DevExpress.XtraPrinting.RecipientFieldType.CC',
            'TO': 'dx_reportDesigner_DevExpress.XtraPrinting.RecipientFieldType.TO',
            'Any Except Extracting Pages': 'dx_reportDesigner_DevExpress.XtraPrinting.ChangingPermissions.AnyExceptExtractingPages',
            'Send as CSV': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.SendCsv',
            'Send as MHT': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.SendMht',
            'Send as RTF': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.SendRtf',
            'Send as TXT': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.SendTxt',
            'Send as PDF': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.SendPdf',
            'Send as XPS': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.SendXps',
            'Send as XLS': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.SendXls',
            'Auto-Select Version': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeVersion.AutoVersion',
            'Prc Envelope Number 3': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.PrcEnvelopeNumber3',
            'Prc Envelope Number 2': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.PrcEnvelopeNumber2',
            'Prc Envelope Number 1': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.PrcEnvelopeNumber1',
            'Prc Envelope Number 7': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.PrcEnvelopeNumber7',
            'Prc Envelope Number 6': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.PrcEnvelopeNumber6',
            'Prc Envelope Number 5': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.PrcEnvelopeNumber5',
            'Prc Envelope Number 4': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.PrcEnvelopeNumber4',
            'Prc Envelope Number 9': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.PrcEnvelopeNumber9',
            'Prc Envelope Number 8': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.PrcEnvelopeNumber8',
            'XPS Document Options': 'dx_reportDesigner_DevExpress.XtraPrinting.XpsDocumentOptions',
            'L': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeErrorCorrectionLevel.L',
            'M': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeErrorCorrectionLevel.M',
            'H': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeErrorCorrectionLevel.H',
            'Q': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.QRCodeErrorCorrectionLevel.Q',
            'Level 4': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.ErrorCorrectionLevel.Level4',
            'Level 5': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.ErrorCorrectionLevel.Level5',
            'Level 6': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.ErrorCorrectionLevel.Level6',
            'Level 7': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.ErrorCorrectionLevel.Level7',
            'Level 0': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.ErrorCorrectionLevel.Level0',
            'Level 1': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.ErrorCorrectionLevel.Level1',
            'Level 2': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.ErrorCorrectionLevel.Level2',
            'Level 3': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.ErrorCorrectionLevel.Level3',
            'Level 8': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.ErrorCorrectionLevel.Level8',
            'Number 14 Envelope': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.Number14Envelope',
            'Not with Report Header': 'dx_reportDesigner_DevExpress.XtraReports.UI.PrintOnPages.NotWithReportHeader',
            'Letter Transverse': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.LetterTransverse',
            'Pdf Password Security Options': 'dx_reportDesigner_DevExpress.XtraPrinting.PdfPasswordSecurityOptions',
            'JIS B6 Rotated': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.B6JisRotated',
            'CStar': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.CodabarStartStopPair.CStar',
            'Japanese Double Postcard': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.JapaneseDoublePostcard',
            'Columns': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.PDF417Generator.Columns',
            'Segments In Row': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataBarGenerator.SegmentsInRow',
            'Prc Envelope Number 3 Rotated': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.PrcEnvelopeNumber3Rotated',
            'B6 Envelope': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.B6Envelope',
            'Fill Background': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.FillBackground',
            'Show Options before Saving': 'dx_reportDesigner_DevExpress.XtraPrinting.NativeFormatOptions.ShowOptionsBeforeSave',
            'German Standard Fanfold': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.GermanStandardFanfold',
            'Dash-Dot': 'dx_reportDesigner_DevExpress.XtraPrinting.BorderDashStyle.DashDot',
            'Italic': 'dx_reportDesigner_System.Drawing.Font.Italic',
            'Page Layout': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.PageLayout',
            'Calculate a Checksum': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.BarCodeGeneratorBase.CalcCheckSum',
            'Export Hyperlinks': 'dx_reportDesigner_DevExpress.XtraPrinting.XlsExportOptions.ExportHyperlinks',
            'Auto Charset': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.Code128Charset.CharsetAuto',
            'Rotate to the Left': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.BarCodeOrientation.RotateLeft',
            'Go To Page': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.GoToPage',
            'MSI Checksum': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.CodeMSIGenerator.MSICheckSum',
            'Page Number (Roman, Uppercase)': 'dx_reportDesigner_DevExpress.XtraPrinting.PageInfo.RomHiNumber',
            '"Current of Total" Page Numbers': 'dx_reportDesigner_DevExpress.XtraPrinting.PageInfo.NumberOfTotal',
            'Not with Report Footer': 'dx_reportDesigner_DevExpress.XtraReports.UI.PrintOnPages.NotWithReportFooter',
            'Bottom Justify': 'dx_reportDesigner_DevExpress.XtraPrinting.TextAlignment.BottomJustify',
            'Plain Text': 'dx_reportDesigner_DevExpress.Snap.Extensions.Native.ActionLists.TextFormat.PlainText',
            'Japanese Envelope Kaku Number 3 Rotated': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.JapaneseEnvelopeKakuNumber3Rotated',
            'Page Number (Roman, Lowercase)': 'dx_reportDesigner_DevExpress.XtraPrinting.PageInfo.RomLowNumber',
            'Print Selection': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.PrintSelection',
            'Tabloid': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.Tabloid',
            'Pdf Permissions Options': 'dx_reportDesigner_DevExpress.XtraPrinting.PdfPermissionsOptions',
            'Letter Extra Transverse': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.LetterExtraTransverse',
            'Format Type': 'dx_reportDesigner_DevExpress.Utils.FormatInfo.FormatType',
            'Rows': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.PDF417Generator.Rows',
            'E-mail Options': 'dx_reportDesigner_DevExpress.XtraPrinting.EmailOptions',
            'Note': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.Note',
            'Use Landscape': 'dx_reportDesigner_DevExpress.XtraPrinting.PrinterSettingsUsing.UseLandscape',
            'Category': 'dx_reportDesigner_DevExpress.XtraPrinting.XpsDocumentOptions.Category',
            'Resolution': 'dx_reportDesigner_DevExpress.XtraPrinting.ImageExportOptions.Resolution',
            'Highest': 'dx_reportDesigner_DevExpress.XtraPrinting.PdfJpegImageQuality.Highest',
            'DL Envelope': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.DLEnvelope',
            'Raw Data Mode': 'dx_reportDesigner_DevExpress.XtraPrinting.XlsExportOptions.RawDataMode',
            'Page Margins': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.PageMargins',
            'Object': 'dx_reportDesigner_DevExpress.XtraTreeList.Data.UnboundColumnType.Object',
            'No Wrap': 'dx_reportDesigner_DevExpress.Utils.WordWrap.NoWrap',
            '(Collection)': 'dx_reportDesigner_System.Collections.CollectionBase',
            'Scale': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.Scale',
            'Print': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.Print',
            'Narrow Vertical': 'dx_reportDesigner_System.Drawing.Drawing2D.HatchStyle.NarrowVertical',
            'Default File Name': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintPreviewOptions.DefaultFileName',
            'Show First Page': 'dx_reportDesigner_DevExpress.XtraPrinting.PrintingSystemCommand.ShowFirstPage',
            'Prc 32K Big': 'dx_reportDesigner_System.Drawing.Printing.PaperKind.Prc32KBig',
            'Edifact': 'dx_reportDesigner_DevExpress.XtraPrinting.BarCode.DataMatrixCompactionMode.Edifact',
            'GDI Character Set': 'dx_reportDesigner_System.Drawing.Font.GdiCharSet',
            'Look-Up Settings': 'dx_reportDesigner_DevExpress.XtraReports.Parameters.Parameter.LookUpSettings',
            'Parameter': 'dx_reportDesigner_DevExpress.XtraReports.Parameters.Parameter',
            'Static List': 'dx_reportDesigner_DevExpress.XtraReports.Parameters.StaticListLookUpSettings',
            'Address': 'dx_reportDesigner_DevExpress.XtraReports.Recipients.Recipient.Address',
            'Prefix': 'dx_reportDesigner_DevExpress.XtraReports.Recipients.Recipient.Prefix',
            'Contact Name': 'dx_reportDesigner_DevExpress.XtraReports.Recipients.Recipient.ContactName',
            'Dynamic List': 'dx_reportDesigner_DevExpress.XtraReports.Parameters.DynamicListLookUpSettings',
            'Look-Up Values': 'dx_reportDesigner_DevExpress.XtraReports.Parameters.StaticListLookUpSettings.LookUpValues',
            'Display Member': 'dx_reportDesigner_DevExpress.XtraReports.Parameters.DynamicListLookUpSettings.DisplayMember',
            'Highlight Negative Points': 'dx_reportDesigner_DevExpress.Sparkline.BarSparklineView.HighlightNegativePoints',
            'Highlight Min Point': 'dx_reportDesigner_DevExpress.Sparkline.SparklineViewBase.HighlightMinPoint',
            'Max Point Color': 'dx_reportDesigner_DevExpress.Sparkline.SparklineViewBase.MaxPointColor',
            'Show Markers': 'dx_reportDesigner_DevExpress.Sparkline.LineSparklineView.ShowMarkers',
            'Min Point Color': 'dx_reportDesigner_DevExpress.Sparkline.SparklineViewBase.MinPointColor',
            'Max Point Marker Size': 'dx_reportDesigner_DevExpress.Sparkline.LineSparklineView.MaxPointMarkerSize',
            'End Point Color': 'dx_reportDesigner_DevExpress.Sparkline.SparklineViewBase.EndPointColor',
            'Marker Color': 'dx_reportDesigner_DevExpress.Sparkline.LineSparklineView.MarkerColor',
            'Negative Point Color': 'dx_reportDesigner_DevExpress.Sparkline.SparklineViewBase.NegativePointColor',
            'Start Point Color': 'dx_reportDesigner_DevExpress.Sparkline.SparklineViewBase.StartPointColor',
            'Negative Point Marker Size': 'dx_reportDesigner_DevExpress.Sparkline.LineSparklineView.NegativePointMarkerSize',
            'Area Opacity': 'dx_reportDesigner_DevExpress.Sparkline.AreaSparklineView.AreaOpacity',
            'End Point Marker Size': 'dx_reportDesigner_DevExpress.Sparkline.LineSparklineView.EndPointMarkerSize',
            'Highlight Start Point': 'dx_reportDesigner_DevExpress.Sparkline.SparklineViewBase.HighlightStartPoint',
            'Min Point Marker Size': 'dx_reportDesigner_DevExpress.Sparkline.LineSparklineView.MinPointMarkerSize',
            'Highlight End Point': 'dx_reportDesigner_DevExpress.Sparkline.SparklineViewBase.HighlightEndPoint',
            'Start Point Marker Size': 'dx_reportDesigner_DevExpress.Sparkline.LineSparklineView.StartPointMarkerSize',
            'Highlight Max Point': 'dx_reportDesigner_DevExpress.Sparkline.SparklineViewBase.HighlightMaxPoint',
            'Circular': 'dx_reportDesigner_DevExpress.XtraGauges.Core.Customization.DashboardGaugeType.Circular',
            'Dashboard Gauge Type': 'dx_reportDesigner_DevExpress.XtraGauges.Core.Customization.DashboardGaugeType',
            'Half': 'dx_reportDesigner_DevExpress.XtraGauges.Core.Customization.DashboardGaugeStyle.Half',
            'Full': 'dx_reportDesigner_DevExpress.XtraGauges.Core.Customization.DashboardGaugeStyle.Full',
            'Three Fourths': 'dx_reportDesigner_DevExpress.XtraGauges.Core.Customization.DashboardGaugeStyle.ThreeFourth',
            'Dashboard Gauge Theme': 'dx_reportDesigner_DevExpress.XtraGauges.Core.Customization.DashboardGaugeTheme',
            'Dashboard Gauge Style': 'dx_reportDesigner_DevExpress.XtraGauges.Core.Customization.DashboardGaugeStyle',
            'Quarter Right': 'dx_reportDesigner_DevExpress.XtraGauges.Core.Customization.DashboardGaugeStyle.QuarterRight',
            'Flat Light': 'dx_reportDesigner_DevExpress.XtraGauges.Core.Customization.DashboardGaugeTheme.FlatLight',
            'Linear': 'dx_reportDesigner_DevExpress.XtraGauges.Core.Customization.DashboardGaugeType.Linear',
            'Flat Dark': 'dx_reportDesigner_DevExpress.XtraGauges.Core.Customization.DashboardGaugeTheme.FlatDark',
            'Quarter Left': 'dx_reportDesigner_DevExpress.XtraGauges.Core.Customization.DashboardGaugeStyle.QuarterLeft',
            'Align to Grid': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_AlignToGrid_STipTitle',
            'Save the current report.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_SaveFile_Description',
            'Make Same Width': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_SizeToControlWidth_STipTitle',
            'Data': 'dx_reportDesigner_ReportStringId.CatData',
            'Load File...': 'dx_reportDesigner_ReportStringId.Cmd_RtfLoad',
            'Behavior': 'dx_reportDesigner_ReportStringId.CatBehavior',
            'Change the font face.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_FontName_STipContent',
            'Justify': 'dx_reportDesigner_ReportStringId.UD_Capt_JustifyJustify',
            'Add the control from the clipboard': 'dx_reportDesigner_ReportStringId.UD_Hint_Paste',
            'Remove Vertical Spacing': 'dx_reportDesigner_ReportStringId.UD_TTip_VertSpaceConcatenate',
            'Close the report': 'dx_reportDesigner_ReportStringId.UD_Hint_Close',
            'Report has been changed. Do you want to save changes ?': 'dx_reportDesigner_ReportStringId.UD_Msg_ReportChanged',
            'Font Name': 'dx_reportDesigner_ReportStringId.UD_TTip_FormatFontName',
            'Font Size': 'dx_reportDesigner_ReportStringId.UD_TTip_FormatFontSize',
            'Paste': 'dx_reportDesigner_ReportStringId.UD_Capt_Paste',
            'Edit Bindings...': 'dx_reportDesigner_ReportStringId.Verb_EditBindings',
            'Save All': 'dx_reportDesigner_ReportStringId.UD_Capt_SaveAll',
            'Cut the selected controls from the report and put them on the Clipboard.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_Cut_STipContent',
            'Set the foreground color of the control': 'dx_reportDesigner_ReportStringId.UD_Hint_ForegroundColor',
            'Make the selected controls the same size': 'dx_reportDesigner_ReportStringId.UD_Hint_MakeSameSizeBoth',
            'Add New DataSource': 'dx_reportDesigner_ReportStringId.UD_Title_FieldList_AddNewDataSourceText',
            'Home': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_HtmlHome_Caption',
            'Copy the selected controls and put them on the Clipboard.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_Copy_STipContent',
            'Center Vertically': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_CenterVertically_STipTitle',
            '{0} {{ PaperKind: {1} }}': 'dx_reportDesigner_ReportStringId.RepTabCtl_ReportStatus',
            'Refresh': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_HtmlRefresh_STipTitle',
            'Multi-Column Mode': 'dx_reportDesigner_ReportStringId.STag_Name_ColumnMode',
            'Change the text background color.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_BackColor_STipContent',
            'Save the report with a new name': 'dx_reportDesigner_ReportStringId.UD_Hint_SaveFileAs',
            'Make Vertical Spacing Equal': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_VertSpaceMakeEqual_Caption',
            'Space for repeating columns.': 'dx_reportDesigner_ReportStringId.MultiColumnDesignMsg1',
            'Controls placed here will be printed incorrectly.': 'dx_reportDesigner_ReportStringId.MultiColumnDesignMsg2',
            'Save the current report with a new name.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_SaveFileAs_Description',
            'Navigation': 'dx_reportDesigner_ReportStringId.CatNavigation',
            'Can\'t create two instances of a class on a form': 'dx_reportDesigner_ReportStringId.Msg_CreateSomeInstance',
            'Printing Settings': 'dx_reportDesigner_ReportStringId.PivotGridForm_ItemSettings_Caption',
            'Center in Form': 'dx_reportDesigner_ReportStringId.UD_Group_CenterInForm',
            'No bookmarks were found in the report. To create a table of contents, specify a bookmark for at least one report element.': 'dx_reportDesigner_ReportStringId.Msg_NoBookmarksWereFoundInReportForXrToc',
            'Make Same Size': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_SizeToControl_Caption',
            'Save all modified reports.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_SaveAll_STipContent',
            'Select one or more of the Appearance objects to customize the printing appearances of the corresponding visual elements.': 'dx_reportDesigner_ReportStringId.PivotGridFrame_Appearances_DescriptionText',
            'Center Text': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_JustifyCenter_STipTitle',
            'An error occurred during deserialization - possible wrong report class name': 'dx_reportDesigner_ReportStringId.Msg_WrongReportClassName',
            'Convert To Labels': 'dx_reportDesigner_ReportStringId.Cmd_TableConvertToLabels',
            'Show or hide the Scripts Editor.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_Scripts_STipContent',
            'Invalid binding': 'dx_reportDesigner_ReportStringId.BindingMapperForm_InvalidBindingWarning',
            'Customize the current XRPivotGrid\'s layout and preview its data.': 'dx_reportDesigner_ReportStringId.PivotGridForm_ItemLayout_Description',
            'Size to Grid': 'dx_reportDesigner_ReportStringId.UD_Capt_MakeSameSizeSizeToGrid',
            'Right align the selected controls': 'dx_reportDesigner_ReportStringId.UD_Hint_AlignRights',
            'Delete': 'dx_reportDesigner_ReportStringId.Cmd_Delete',
            'Open...': 'dx_reportDesigner_ReportStringId.UD_Capt_OpenFile',
            'Error when trying to populate the datasource. The following exception was thrown:': 'dx_reportDesigner_ReportStringId.Msg_FillDataError',
            'Can\'t load the report. The file is possibly corrupted or report\'s assembly is missing.': 'dx_reportDesigner_ReportStringId.Msg_FileCorrupted',
            'Add a formatting rule': 'dx_reportDesigner_ReportStringId.FRSForm_TTip_AddRule',
            'Open a report.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_OpenFile_STipContent',
            'Main settings(Fields, Layout).': 'dx_reportDesigner_ReportStringId.PivotGridForm_GroupMain_Description',
            'Decrease the vertical spacing between the selected controls.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_VertSpaceDecrease_STipContent',
            'Done': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_StatusBar_HtmlDone',
            'Tile Vertical': 'dx_reportDesigner_ReportStringId.UD_Capt_MdiTileVertical',
            'Make the selected controls have the same width': 'dx_reportDesigner_ReportStringId.UD_Hint_MakeSameSizeWidth',
            'Data source:': 'dx_reportDesigner_ReportStringId.NewParameterEditorForm_DataSource',
            'Standard Controls': 'dx_reportDesigner_ReportStringId.UD_XtraReportsToolboxCategoryName',
            'Main Menu': 'dx_reportDesigner_ReportStringId.UD_Capt_MainMenuName',
            'Manage fields.': 'dx_reportDesigner_ReportStringId.PivotGridForm_ItemFields_Description',
            'Switch between tabbed and window MDI layout modes': 'dx_reportDesigner_ReportStringId.UD_Hint_TabbedInterface',
            'Increase Vertical Spacing': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_VertSpaceIncrease_STipTitle',
            'The padding should be greater than or equal to 0.': 'dx_reportDesigner_ReportStringId.Msg_IncorrectPadding',
            'Undo the last operation': 'dx_reportDesigner_ReportStringId.UD_Hint_Undo',
            'Redo the last operation': 'dx_reportDesigner_ReportStringId.UD_Hint_Redo',
            'Select or input the zoom factor': 'dx_reportDesigner_ReportStringId.UD_Hint_Zoom',
            'Close the designer': 'dx_reportDesigner_ReportStringId.UD_Hint_Exit',
            'Copy the control to the clipboard': 'dx_reportDesigner_ReportStringId.UD_Hint_Copy',
            'Make Horizontal Spacing Equal': 'dx_reportDesigner_ReportStringId.UD_TTip_HorizSpaceMakeEqual',
            'Side margins': 'dx_reportDesigner_ReportStringId.SR_Side_Margins',
            'Change the text foreground color.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_ForeColor_STipContent',
            'Toolbar': 'dx_reportDesigner_ReportStringId.UD_Capt_ToolbarName',
            'Report Explorer': 'dx_reportDesigner_ReportStringId.UD_Title_ReportExplorer',
            'Align Rights': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_AlignRight_Caption',
            'Align the positions of the selected controls to the grid.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_AlignToGrid_STipContent',
            'Align Lefts': 'dx_reportDesigner_ReportStringId.UD_TTip_AlignLeft',
            'Commands': 'dx_reportDesigner_ReportStringId.Cmd_Commands',
            'StyleSheet error': 'dx_reportDesigner_ReportStringId.SSForm_Msg_StyleSheetError',
            'New Report via Wizard...': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_NewReportWizard_Caption',
            'Clear': 'dx_reportDesigner_ReportStringId.Verb_RTFClear',
            'Back': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_HtmlBackward_STipTitle',
            'Align text to both the left and right sides, adding extra space between words as necessary.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_JustifyJustify_STipContent',
            'Forward': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_HtmlForward_Caption',
            'Bring to Front': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_BringToFront_Caption',
            'Underline the font': 'dx_reportDesigner_ReportStringId.UD_Hint_FontUnderline',
            'Make the vertical spacing between the selected controls equal.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_VertSpaceMakeEqual_STipContent',
            'HTML View': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_HtmlPageText',
            'Project Objects': 'dx_reportDesigner_ReportStringId.UD_Title_FieldList_ProjectObjectsText',
            'Invalid leader symbol.': 'dx_reportDesigner_ReportStringId.Msg_InvalidLeaderSymbolForXrTocLevel',
            'Align the positions of the selected controls to the grid': 'dx_reportDesigner_ReportStringId.UD_Hint_AlignToGrid',
            'Underline the selected text.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_FontUnderline_STipContent',
            'Undo': 'dx_reportDesigner_ReportStringId.UD_TTip_Undo',
            'Redo': 'dx_reportDesigner_ReportStringId.UD_TTip_Redo',
            'Column To Right': 'dx_reportDesigner_ReportStringId.Cmd_TableInsertColumnToRight',
            'Run Designer...': 'dx_reportDesigner_ReportStringId.Verb_RunDesigner',
            'Lefts': 'dx_reportDesigner_ReportStringId.UD_Capt_AlignLefts',
            'Vertical Spacing': 'dx_reportDesigner_ReportStringId.UD_Group_VerticalSpacing',
            'Preview': 'dx_reportDesigner_ReportStringId.RepTabCtl_Preview',
            'Formatting Toolbar': 'dx_reportDesigner_ReportStringId.UD_Capt_FormattingToolbarName',
            'Delete the control and copy it to the clipboard': 'dx_reportDesigner_ReportStringId.UD_Hint_Cut',
            'Column To Left': 'dx_reportDesigner_ReportStringId.Cmd_TableInsertColumnToLeft',
            'Validate': 'dx_reportDesigner_ReportStringId.ScriptEditor_Validate',
            'Data adapter:': 'dx_reportDesigner_ReportStringId.NewParameterEditorForm_DataAdapter',
            'Error': 'dx_reportDesigner_ReportStringId.Msg_ErrorTitle',
            'Column': 'dx_reportDesigner_ReportStringId.Cmd_TableDeleteColumn',
            'New': 'dx_reportDesigner_ReportStringId.UD_Capt_NewReport',
            'Click "Validate" to check scripts.': 'dx_reportDesigner_ReportStringId.ScriptEditor_ClickValidate',
            'Report StyleSheet files (*.repss)|*.repss|All files (*.*)|*.*': 'dx_reportDesigner_ReportStringId.SSForm_Msg_FileFilter',
            'Align the control\'s text to the left': 'dx_reportDesigner_ReportStringId.UD_Hint_JustifyLeft',
            'Change the zoom level of the document designer.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_Zoom_STipContent',
            'Incorrect argument\'s value': 'dx_reportDesigner_ReportStringId.Msg_IncorrectArgument',
            'Decrease Horizontal Spacing': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_HorizSpaceDecrease_Caption',
            'Show/Hide Windows': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_Windows_STipTitle',
            'Align the centers of the selected controls horizontally.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_AlignHorizontalCenters_STipContent',
            'Align To Grid': 'dx_reportDesigner_ReportStringId.Cmd_AlignToGrid',
            'Remove a formatting rule': 'dx_reportDesigner_ReportStringId.FRSForm_TTip_RemoveRule',
            'Show only invalid bindings': 'dx_reportDesigner_ReportStringId.BindingMapperForm_ShowOnlyInvalidBindings',
            'Vertical\r\npitch': 'dx_reportDesigner_ReportStringId.SR_Vertical_Pitch',
            'Row': 'dx_reportDesigner_ReportStringId.Cmd_TableDeleteRow',
            'Italicize the text.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_FontItalic_STipContent',
            'Format String...': 'dx_reportDesigner_ReportStringId.Verb_FormatString',
            'Send to Back': 'dx_reportDesigner_ReportStringId.UD_Capt_OrderSendToBack',
            'Report Source: {0}\r\n': 'dx_reportDesigner_ReportStringId.XRSubreport_ReportSourceInfo',
            'Select All': 'dx_reportDesigner_ReportStringId.UD_Capt_SelectAll',
            'You selected more than one formatting rule': 'dx_reportDesigner_ReportStringId.FRSForm_Msg_MoreThanOneRule',
            'Hide or show the {0} window': 'dx_reportDesigner_ReportStringId.UD_Hint_ViewDockPanels',
            'The following error occurred when the script in procedure {0} was executed:\r\n {1}': 'dx_reportDesigner_ReportStringId.Msg_ScriptExecutionError',
            'Make the font italic': 'dx_reportDesigner_ReportStringId.UD_Hint_FontItalic',
            'The group header or footer you want to delete is not empty. Do you want to delete this band along with its controls?': 'dx_reportDesigner_ReportStringId.Msg_GroupSortWarning',
            'The \'ReportSource\' property of a subreport control cannot be set to a descendant of the current report': 'dx_reportDesigner_ReportStringId.Msg_InvalidReportSource',
            'Zoom Factor: {0}%': 'dx_reportDesigner_ReportStringId.UD_Capt_ZoomFactor',
            'Exact:': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_ZoomExact_Caption',
            'There are following errors in script(s):\r\n{0}': 'dx_reportDesigner_ReportStringId.Msg_ScriptError',
            'Structure': 'dx_reportDesigner_ReportStringId.CatStructure',
            'Export warning: The following controls are overlapped and may be exported to HTML, RTF, XLS, XLSX, CSV and Text incorrectly - {0}.': 'dx_reportDesigner_ReportStringId.Msg_WarningControlsAreOverlapped',
            'Middles': 'dx_reportDesigner_ReportStringId.UD_Capt_AlignMiddles',
            'Data member:': 'dx_reportDesigner_ReportStringId.NewParameterEditorForm_DataMember',
            'Appearances': 'dx_reportDesigner_ReportStringId.PivotGridForm_ItemAppearances_Caption',
            'Processing...': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_StatusBar_HtmlProcessing',
            'Group and Sort': 'dx_reportDesigner_ReportStringId.UD_Title_GroupAndSort',
            'GroupFooter': 'dx_reportDesigner_ReportStringId.Cmd_GroupFooter',
            'New Report': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_NewReport_Caption',
            'Adjust the printing settings for the current XRPivotGrid.': 'dx_reportDesigner_ReportStringId.PivotGridForm_ItemSettings_Description',
            'Align the bottoms of the selected controls.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_AlignBottom_STipContent',
            'Align Left': 'dx_reportDesigner_ReportStringId.UD_TTip_FormatAlignLeft',
            'Align the tops of the selected controls': 'dx_reportDesigner_ReportStringId.UD_Hint_AlignTops',
            'Move the selected controls to the back': 'dx_reportDesigner_ReportStringId.UD_Hint_OrderSendToBack',
            'Edit Text': 'dx_reportDesigner_ReportStringId.Verb_EditText',
            'Bring To Front': 'dx_reportDesigner_ReportStringId.Cmd_BringToFront',
            'To add a new grouping or sorting level, first provide a data source for the report.': 'dx_reportDesigner_ReportStringId.Msg_GroupSortNoDataSource',
            'Create a new report using the Wizard': 'dx_reportDesigner_ReportStringId.UD_Hint_NewWizardReport',
            'Arrange all open documents from left to right': 'dx_reportDesigner_ReportStringId.UD_Hint_MdiTileVertical',
            ' style': 'dx_reportDesigner_ReportStringId.SSForm_Msg_StyleNamePreviewPostfix',
            'The specified expression is invalid.': 'dx_reportDesigner_ReportStringId.Msg_InvalidExpressionEx',
            'No styles are selected': 'dx_reportDesigner_ReportStringId.SSForm_Msg_NoStyleSelected',
            'Align Bottoms': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_AlignBottom_Caption',
            'Cascade': 'dx_reportDesigner_ReportStringId.UD_Capt_MdiCascade',
            'Launch the report wizard to create a new report.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_NewReportWizard_STipContent',
            'Create a new blank report.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_NewReport_STipContent',
            'Centers': 'dx_reportDesigner_ReportStringId.UD_Capt_AlignCenters',
            'Detail reports don\'t support multicolumn.': 'dx_reportDesigner_ReportStringId.Msg_DontSupportMulticolumn',
            'Justify the control\'s text': 'dx_reportDesigner_ReportStringId.UD_Hint_JustifyJustify',
            'Custom function \'{0}\' not found.': 'dx_reportDesigner_ReportStringId.Msg_NoCustomFunction',
            'Toolbox': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_ToolboxControlsPage',
            'Tops': 'dx_reportDesigner_ReportStringId.UD_Capt_AlignTops',
            'Right align the selected controls.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_AlignRight_STipContent',
            '(Not set)': 'dx_reportDesigner_ReportStringId.UD_PropertyGrid_NotSetText',
            'Elements': 'dx_reportDesigner_ReportStringId.CatElements',
            '"{0}" has been changed. Do you want to save changes ?': 'dx_reportDesigner_ReportStringId.UD_Msg_MdiReportChanged',
            'Increase the vertical spacing between the selected controls.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_VertSpaceIncrease_STipContent',
            'Save As...': 'dx_reportDesigner_ReportStringId.UD_Capt_SaveFileAs',
            'Increase': 'dx_reportDesigner_ReportStringId.UD_Capt_SpacingIncrease',
            'Printing': 'dx_reportDesigner_ReportStringId.CatPrinting',
            'Are you sure you want to apply these changes?': 'dx_reportDesigner_ReportStringId.Msg_ApplyChangesQuestion',
            'Open a report': 'dx_reportDesigner_ReportStringId.UD_Hint_OpenFile',
            'Save Report As': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_SaveFileAs_STipTitle',
            'Move Down': 'dx_reportDesigner_ReportStringId.Cmd_BandMoveDown',
            'The XRTableOfContents control can be placed only into Report Header and Report Footer bands.': 'dx_reportDesigner_ReportStringId.Msg_PlacingXrTocIntoIncorrectContainer',
            'All scripts are valid.': 'dx_reportDesigner_ReportStringId.ScriptEditor_ScriptsAreValid',
            'Make Same Height': 'dx_reportDesigner_ReportStringId.UD_TTip_SizeToControlHeight',
            'GroupHeader': 'dx_reportDesigner_ReportStringId.Cmd_GroupHeader',
            'Remove a style': 'dx_reportDesigner_ReportStringId.SSForm_TTip_RemoveStyle',
            'Increase the spacing between the selected controls': 'dx_reportDesigner_ReportStringId.UD_Hint_SpacingIncrease',
            'This operation will remove all calculated fields from all data tables. Do you wish to proceed?': 'dx_reportDesigner_ReportStringId.Msg_WarningRemoveCalculatedFields',
            'Cut': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_Cut_STipTitle',
            'Close the current report.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_Close_STipContent',
            'Undo the last operation.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_Undo_STipContent',
            'Switch to the {0} tab': 'dx_reportDesigner_ReportStringId.UD_Hint_ViewTabs',
            'Hide or show the {0}': 'dx_reportDesigner_ReportStringId.UD_Hint_ViewBars',
            '(New)': 'dx_reportDesigner_ReportStringId.ScriptEditor_NewString',
            'Center Horizontally': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_CenterHorizontally_STipTitle',
            'Paste the contents of the Clipboard.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_Paste_STipContent',
            'Loc': 'dx_reportDesigner_ReportStringId.DesignerStatus_Location',
            'Modify the XRPivotGrid\'s layout (sorting settings, field arrangement) and click the Apply button to apply the modifications to the current XRPivotGrid. You can also save the layout to an XML file (this can be loaded and applied to other views at design time and runtime).': 'dx_reportDesigner_ReportStringId.PivotGridFrame_Layouts_DescriptionText',
            'Load styles from a file': 'dx_reportDesigner_ReportStringId.SSForm_TTip_LoadStyles',
            'Add a Sort': 'dx_reportDesigner_ReportStringId.GroupSort_AddSort',
            'Zoom Toolbar': 'dx_reportDesigner_ReportStringId.UD_Capt_ZoomToolbarName',
            'Preview Row Count': 'dx_reportDesigner_ReportStringId.STag_Name_PreviewRowCount',
            'Arrange all open documents cascaded, so that they overlap each other': 'dx_reportDesigner_ReportStringId.UD_Hint_MdiCascade',
            'Move the selected controls to the back.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_SendToBack_STipContent',
            'Decrease Vertical Spacing': 'dx_reportDesigner_ReportStringId.UD_TTip_VertSpaceDecrease',
            'Clear formatting rules': 'dx_reportDesigner_ReportStringId.FRSForm_TTip_ClearRules',
            'Set the background color of the control': 'dx_reportDesigner_ReportStringId.UD_Hint_BackGroundColor',
            'Bind': 'dx_reportDesigner_ReportStringId.Verb_Bind',
            'Save...': 'dx_reportDesigner_ReportStringId.Verb_Save',
            'Delete...': 'dx_reportDesigner_ReportStringId.Verb_Delete',
            'Align Middles': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_AlignHorizontalCenters_Caption',
            'Send To Back': 'dx_reportDesigner_ReportStringId.Cmd_SendToBack',
            'Save/Export...': 'dx_reportDesigner_ReportStringId.Verb_Export',
            'Top\r\nmargin': 'dx_reportDesigner_ReportStringId.SR_Top_Margin',
            'Align the bottoms of the selected controls': 'dx_reportDesigner_ReportStringId.UD_Hint_AlignBottoms',
            'Properties': 'dx_reportDesigner_ReportStringId.Cmd_Properties',
            'Increase Horizontal Spacing': 'dx_reportDesigner_ReportStringId.UD_TTip_HorizSpaceIncrease',
            'Property': 'dx_reportDesigner_ReportStringId.BCForm_Lbl_Property',
            'Drag-and-drop this item to create a control bound to it;\r\n- or -\r\nDrag this item with the right mouse button or SHIFT\r\nto select a bound control from the popup menu;\r\n- or -\r\nUse the context menu to add a calculated field or parameter.': 'dx_reportDesigner_ReportStringId.UD_TTip_ItemDescription',
            'Horizontally': 'dx_reportDesigner_ReportStringId.UD_Capt_CenterInFormHorizontally',
            'Horizontally center the selected controls within a band': 'dx_reportDesigner_ReportStringId.UD_Hint_CenterInFormHorizontally',
            'Save File': 'dx_reportDesigner_ReportStringId.UD_TTip_FileSave',
            'Open File': 'dx_reportDesigner_ReportStringId.UD_TTip_FileOpen',
            'Report Designer': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_PageText',
            'Save all reports': 'dx_reportDesigner_ReportStringId.UD_Hint_SaveAll',
            'Value member:': 'dx_reportDesigner_ReportStringId.NewParameterEditorForm_ValueMember',
            'Bottoms': 'dx_reportDesigner_ReportStringId.UD_Capt_AlignBottoms',
            'Bring the selected controls to the front.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_BringToFront_STipContent',
            'Size the selected controls to the grid': 'dx_reportDesigner_ReportStringId.UD_Hint_MakeSameSizeSizeToGrid',
            'Zoom out the design surface': 'dx_reportDesigner_ReportStringId.UD_Hint_ZoomOut',
            'Open/Import...': 'dx_reportDesigner_ReportStringId.Verb_Import',
            'Insert...': 'dx_reportDesigner_ReportStringId.Verb_Insert',
            'Tabbed Interface': 'dx_reportDesigner_ReportStringId.UD_Capt_TabbedInterface',
            'Designer': 'dx_reportDesigner_ReportStringId.RepTabCtl_Designer',
            'Insert Band': 'dx_reportDesigner_ReportStringId.Cmd_InsertBand',
            'Can\'t load the report\'s layout. The file is possibly corrupted or contains incorrect information.': 'dx_reportDesigner_ReportStringId.Msg_FileContentCorrupted',
            'Align Right': 'dx_reportDesigner_ReportStringId.UD_TTip_FormatAlignRight',
            'Arrange all open documents from top to bottom': 'dx_reportDesigner_ReportStringId.UD_Hint_MdiTileHorizontal',
            'DetailReport': 'dx_reportDesigner_ReportStringId.Cmd_DetailReport',
            'Vertically center the selected controls within a band': 'dx_reportDesigner_ReportStringId.UD_Hint_CenterInFormVertically',
            'Align text to the right.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_JustifyRight_STipContent',
            'Align Centers': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_AlignVerticalCenters_STipTitle',
            'The report currently being edited is of a different type than the one you are trying to open.\r\nDo you want to open the selected report anyway?': 'dx_reportDesigner_ReportStringId.Msg_CreateReportInstance',
            'Left align the selected controls.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_AlignLeft_STipContent',
            'Remove Horizontal Spacing': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_HorizSpaceConcatenate_Caption',
            'Main': 'dx_reportDesigner_ReportStringId.PivotGridForm_GroupMain_Caption',
            'Text is too large.': 'dx_reportDesigner_ReportStringId.Msg_LargeText',
            'Align the centers of the selected controls vertically': 'dx_reportDesigner_ReportStringId.UD_Hint_AlignCenters',
            'Find the text on this page.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_HtmlFind_STipContent',
            'Number Across': 'dx_reportDesigner_ReportStringId.SR_Number_Across',
            'Show or hide the Tool Box, Report Explorer, Field List and Property Grid windows.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_Windows_STipContent',
            'Save styles to a file': 'dx_reportDesigner_ReportStringId.SSForm_TTip_SaveStyles',
            'Align Text Left': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_JustifyLeft_Caption',
            'TopMargin': 'dx_reportDesigner_ReportStringId.Cmd_TopMargin',
            'Zoom in to get a close-up view of the report.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_ZoomIn_STipContent',
            'CenterVertically': 'dx_reportDesigner_ReportStringId.UD_TTip_CenterVertically',
            'Align the centers of the selected controls vertically.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_AlignVerticalCenters_STipContent',
            'Horizontal pitch': 'dx_reportDesigner_ReportStringId.SR_Horizontal_Pitch',
            'Create a new blank report': 'dx_reportDesigner_ReportStringId.UD_Hint_NewReport',
            'Align Tops': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_AlignTop_Caption',
            'Printing option management for the current XRPivotGrid.': 'dx_reportDesigner_ReportStringId.PivotGridForm_GroupPrinting_Description',
            'Close the report designer.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_Exit_STipContent',
            '\r\n\r\nDataMember: {0}': 'dx_reportDesigner_ReportStringId.UD_TTip_DataMemberDescription',
            'Align Text Right': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_JustifyRight_STipTitle',
            'Remove': 'dx_reportDesigner_ReportStringId.UD_Capt_SpacingRemove',
            'Printing warning: Save the following reports to preview subreports with recent changes applied - {0}.': 'dx_reportDesigner_ReportStringId.Msg_WarningUnsavedReports',
            'Decrease the spacing between the selected controls': 'dx_reportDesigner_ReportStringId.UD_Hint_SpacingDecrease',
            'BottomMargin': 'dx_reportDesigner_ReportStringId.Cmd_BottomMargin',
            'Left align the selected controls': 'dx_reportDesigner_ReportStringId.UD_Hint_AlignLefts',
            'Use Ctrl with the left mouse button to rotate the shape': 'dx_reportDesigner_ReportStringId.Msg_ShapeRotationToolTip',
            'Redo the last operation.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_Redo_STipContent',
            'New via Wizard...': 'dx_reportDesigner_ReportStringId.UD_Capt_NewWizardReport',
            'Move back to the previous page.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_HtmlBackward_STipContent',
            'Delete unused styles': 'dx_reportDesigner_ReportStringId.SSForm_TTip_PurgeStyles',
            'Windows': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_Windows_Caption',
            'XtraReports': 'dx_reportDesigner_ReportStringId.Msg_Caption',
            'Add a Group': 'dx_reportDesigner_ReportStringId.GroupSort_AddGroup',
            'Window': 'dx_reportDesigner_ReportStringId.UD_Group_Window',
            'Horizontally center the selected controls within a band.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_CenterHorizontally_STipContent',
            'Edit': 'dx_reportDesigner_ReportStringId.UD_Group_Edit',
            'Move Up': 'dx_reportDesigner_ReportStringId.Cmd_BandMoveUp',
            'Row Above': 'dx_reportDesigner_ReportStringId.Cmd_TableInsertRowAbove',
            'Row Below': 'dx_reportDesigner_ReportStringId.Cmd_TableInsertRowBelow',
            'Horizontal Spacing': 'dx_reportDesigner_ReportStringId.UD_Group_HorizontalSpacing',
            'one band per page': 'dx_reportDesigner_ReportStringId.BandDsg_QuantityPerPage',
            'User Designer': 'dx_reportDesigner_ReportStringId.CatUserDesigner',
            'The condition must be Boolean!': 'dx_reportDesigner_ReportStringId.Msg_InvalidCondition',
            'Show fields selector': 'dx_reportDesigner_ReportStringId.PivotGridFrame_Layouts_SelectorCaption2',
            'Hide fields selector': 'dx_reportDesigner_ReportStringId.PivotGridFrame_Layouts_SelectorCaption1',
            ' selected styles...': 'dx_reportDesigner_ReportStringId.SSForm_Msg_SelectedStylesText',
            'PageHeader': 'dx_reportDesigner_ReportStringId.Cmd_PageHeader',
            'Decrease the horizontal spacing between the selected controls.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_HorizSpaceDecrease_STipContent',
            'ReportFooter': 'dx_reportDesigner_ReportStringId.Cmd_ReportFooter',
            'No more instances of XRTableOfContents can be added to the band.': 'dx_reportDesigner_ReportStringId.Msg_InvalidXrTocInstanceInBand',
            'There are cyclic bookmarks in the report.': 'dx_reportDesigner_ReportStringId.Msg_CyclicBookmarks',
            'Components': 'dx_reportDesigner_ReportStringId.UD_Title_ReportExplorer_Components',
            'Unbound': 'dx_reportDesigner_ReportStringId.Cmd_InsertUnboundDetailReport',
            'Clear styles': 'dx_reportDesigner_ReportStringId.SSForm_TTip_ClearStyles',
            'Field List': 'dx_reportDesigner_ReportStringId.UD_Title_FieldList',
            'Drag-and-drop this item to create a table with its items;\r\n- or -\r\nDrag this item with the right mouse button or SHIFT\r\nto create a \'header\' table with field names;\r\n- or -\r\nUse the context menu to add a calculated field or parameter.': 'dx_reportDesigner_ReportStringId.UD_TTip_TableDescription',
            'Add a style': 'dx_reportDesigner_ReportStringId.SSForm_TTip_AddStyle',
            'Show/Hide Scripts': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_Scripts_STipTitle',
            'Zoom in the design surface': 'dx_reportDesigner_ReportStringId.UD_Hint_ZoomIn',
            'Design': 'dx_reportDesigner_ReportStringId.CatDesign',
            'Report Source Url: {0}\r\n': 'dx_reportDesigner_ReportStringId.XRSubreport_ReportSourceUrlInfo',
            'Add Sub-Band': 'dx_reportDesigner_ReportStringId.Cmd_AddSubBand',
            'Adjust the print appearances of the current XRPivotGrid.': 'dx_reportDesigner_ReportStringId.PivotGridForm_ItemAppearances_Description',
            'Design in Report Wizard...': 'dx_reportDesigner_ReportStringId.Verb_ReportWizard',
            'Edit GroupFields...': 'dx_reportDesigner_ReportStringId.Verb_EditGroupFields',
            'Center text.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_JustifyCenter_STipContent',
            'Align the control\'s text to the center': 'dx_reportDesigner_ReportStringId.UD_Hint_JustifyCenter',
            'No more instances of XRTableOfContents can be added to the report.': 'dx_reportDesigner_ReportStringId.Msg_InvalidXrTocInstance',
            'Align the tops of the selected controls.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_AlignTop_STipContent',
            'Delete the control': 'dx_reportDesigner_ReportStringId.UD_Hint_Delete',
            'The Font name can\'t be empty.': 'dx_reportDesigner_ReportStringId.Msg_WarningFontNameCantBeEmpty',
            'PageFooter': 'dx_reportDesigner_ReportStringId.Cmd_PageFooter',
            'File not found.': 'dx_reportDesigner_ReportStringId.Msg_FileNotFound',
            'You selected more than one style': 'dx_reportDesigner_ReportStringId.SSForm_Msg_MoreThanOneStyle',
            'Change the font size.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_FontSize_STipContent',
            'You don\'t have sufficient permission to execute the scripts in this report.\r\n\r\nDetails:\r\n\r\n{0}': 'dx_reportDesigner_ReportStringId.Msg_ScriptingPermissionErrorMessage',
            'Data Binding': 'dx_reportDesigner_ReportStringId.STag_Name_DataBinding',
            'Align the control\'s text to the right': 'dx_reportDesigner_ReportStringId.UD_Hint_JustifyRight',
            'Add Field to Area': 'dx_reportDesigner_ReportStringId.Verb_AddFieldToArea',
            'Tasks': 'dx_reportDesigner_ReportStringId.STag_Capt_Tasks',
            'ReportHeader': 'dx_reportDesigner_ReportStringId.Cmd_ReportHeader',
            'Refresh this page.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_HtmlRefresh_STipContent',
            'Serialization Error': 'dx_reportDesigner_ReportStringId.Msg_SerializationErrorTitle',
            'Styles Editor': 'dx_reportDesigner_ReportStringId.SSForm_Caption',
            'Make the selected controls have the same height': 'dx_reportDesigner_ReportStringId.UD_Hint_MakeSameSizeHeight',
            'Entered code is not correct': 'dx_reportDesigner_ReportStringId.Msg_ScriptCodeIsNotCorrect',
            'Table of Contents Level Collection Editor': 'dx_reportDesigner_ReportStringId.XRTableOfContents_LevelCollectionEditor_Title',
            '{0} {1}': 'dx_reportDesigner_ReportStringId.STag_Capt_Format',
            'Load Report Template...': 'dx_reportDesigner_ReportStringId.Verb_LoadReportTemplate',
            'Save \'{0}\'': 'dx_reportDesigner_ReportStringId.Dlg_SaveFile_Title',
            'Layout Toolbar': 'dx_reportDesigner_ReportStringId.UD_Capt_LayoutToolbarName',
            'Move forward to the next page.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_HtmlForward_STipContent',
            'XRPivotGrid Fields': 'dx_reportDesigner_ReportStringId.PivotGridFrame_Fields_ColumnsText',
            'Rights': 'dx_reportDesigner_ReportStringId.UD_Capt_JustifyRight',
            'About': 'dx_reportDesigner_ReportStringId.Verb_About',
            'The specified expression contains invalid symbols (line {0}, character {1}).': 'dx_reportDesigner_ReportStringId.Msg_InvalidExpression',
            'to Grid': 'dx_reportDesigner_ReportStringId.UD_Capt_AlignToGrid',
            'View Code': 'dx_reportDesigner_ReportStringId.Cmd_ViewCode',
            'Edit and Reorder Bands...': 'dx_reportDesigner_ReportStringId.Verb_EditBands',
            'one band per report': 'dx_reportDesigner_ReportStringId.BandDsg_QuantityPerReport',
            'Importing a report layout. Please, wait...': 'dx_reportDesigner_ReportStringId.Msg_ReportImporting',
            'You must select fields for the report before you continue': 'dx_reportDesigner_ReportStringId.Wizard_PageChooseFields_Msg',
            'Number\r\nDown': 'dx_reportDesigner_ReportStringId.SR_Number_Down',
            'Display member:': 'dx_reportDesigner_ReportStringId.NewParameterEditorForm_DisplayMember',
            'Align the centers of the selected controls horizontally': 'dx_reportDesigner_ReportStringId.UD_Hint_AlignMiddles',
            'Save Report': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_SaveFile_STipTitle',
            'Vertically center the selected controls within a band.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_CenterVertically_STipContent',
            'Make the horizontal spacing between the selected controls equal.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_HorizSpaceMakeEqual_STipContent',
            'Make Same size': 'dx_reportDesigner_ReportStringId.UD_TTip_SizeToControl',
            'Open Report': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_OpenFile_STipTitle',
            'Vertically': 'dx_reportDesigner_ReportStringId.UD_Capt_CenterInFormVertically',
            'Make the selected text bold.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_FontBold_STipContent',
            'Save the report': 'dx_reportDesigner_ReportStringId.UD_Hint_SaveFile',
            'Place controls here to keep them together': 'dx_reportDesigner_ReportStringId.PanelDesignMsg',
            'Insert': 'dx_reportDesigner_ReportStringId.Cmd_TableInsert',
            'Insert Detail Report': 'dx_reportDesigner_ReportStringId.Cmd_InsertDetailReport',
            'Status Bar': 'dx_reportDesigner_ReportStringId.UD_Capt_StatusBarName',
            'Increase the horizontal spacing between the selected controls.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_HorizSpaceIncrease_STipContent',
            'Make the selected controls have the same width.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_SizeToControlWidth_STipContent',
            'Name: {0}\r\n': 'dx_reportDesigner_ReportStringId.XRSubreport_NameInfo',
            'Select and drag field to the PivotGrid fields panel to create PivotGrid field.': 'dx_reportDesigner_ReportStringId.PivotGridFrame_Fields_DescriptionText2',
            'You can add and delete XRPivotGrid fields and modify their settings.': 'dx_reportDesigner_ReportStringId.PivotGridFrame_Fields_DescriptionText1',
            'Exit': 'dx_reportDesigner_ReportStringId.UD_Capt_Exit',
            'Bring the selected controls to the front': 'dx_reportDesigner_ReportStringId.UD_Hint_OrderBringToFront',
            'Printing warning: The following controls are outside the right page margin, and this will cause extra pages to be printed - {0}.': 'dx_reportDesigner_ReportStringId.Msg_WarningControlsAreOutOfMargin',
            'Select all the controls in the document': 'dx_reportDesigner_ReportStringId.UD_Hint_SelectAll',
            'Remove the vertical spacing between the selected controls.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_VertSpaceConcatenate_STipContent',
            'Make the selected controls have the same height.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_SizeToControlHeight_STipContent',
            'Display the home page.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_HtmlHome_STipContent',
            'The error log is unrelated to the actual script, because the script has been changed after its last validation.\r\nTo see the actual script errors, click the Validate button again.': 'dx_reportDesigner_ReportStringId.ScriptEditor_ScriptHasBeenChanged',
            'Align': 'dx_reportDesigner_ReportStringId.UD_Group_Align',
            'Delete unused formatting rules': 'dx_reportDesigner_ReportStringId.FRSForm_TTip_PurgeRules',
            'Order': 'dx_reportDesigner_ReportStringId.UD_Group_Order',
            'Formatting Rule Sheet Editor': 'dx_reportDesigner_ReportStringId.FRSForm_Caption',
            'Zoom out to see more of the report at a reduced size.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_ZoomOut_STipContent',
            'Make Equal': 'dx_reportDesigner_ReportStringId.UD_Capt_SpacingMakeEqual',
            'Remove the horizontal spacing between the selected controls.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_HorizSpaceConcatenate_STipContent',
            'Scripts Errors': 'dx_reportDesigner_ReportStringId.UD_Title_ErrorList',
            'Size the selected controls to the grid.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_SizeToGrid_STipContent',
            'New Report via Wizard': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_NewReportWizard_STipTitle',
            'This method call is invalid for the object\'s current state': 'dx_reportDesigner_ReportStringId.Msg_InvalidMethodCall',
            'Null': 'dx_reportDesigner_ReportStringId.XRSubreport_NullReportSourceInfo',
            'Summary...': 'dx_reportDesigner_ReportStringId.Verb_SummaryWizard',
            'This operation will remove all parameters. Do you wish to proceed?': 'dx_reportDesigner_ReportStringId.Msg_WarningRemoveParameters',
            'Tile Horizontal': 'dx_reportDesigner_ReportStringId.UD_Capt_MdiTileHorizontal',
            'Make the font bold': 'dx_reportDesigner_ReportStringId.UD_Hint_FontBold',
            'Tab Buttons': 'dx_reportDesigner_ReportStringId.UD_Group_TabButtonsList',
            'Make the selected controls have the same size.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_SizeToControl_STipContent',
            'No formatting rules are selected': 'dx_reportDesigner_ReportStringId.FRSForm_Msg_NoRuleSelected',
            'Align text to the left.': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_JustifyLeft_STipContent',
            'Decrease': 'dx_reportDesigner_ReportStringId.UD_Capt_SpacingDecrease',
            'New Blank Report': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_NewReport_STipTitle',
            'Tool Box': 'dx_reportDesigner_ReportStringId.UD_Title_ToolBox',
            'Property Grid': 'dx_reportDesigner_ReportStringId.UD_Title_PropertyGrid',
            'Invalid file format': 'dx_reportDesigner_ReportStringId.SSForm_Msg_InvalidFileFormat',
            'Report Files (*{0})|*{1}|All Files (*.*)|*.*': 'dx_reportDesigner_ReportStringId.UD_SaveFileDialog_DialogFilter',
            'Save All Reports': 'dx_reportDesigner_ReportStringId.RibbonXRDesign_SaveAll_STipTitle',
            'Not enough memory to paint. Zoom level will be reset.': 'dx_reportDesigner_ReportStringId.Msg_NotEnoughMemoryToPaint',
            'Toolbars': 'dx_reportDesigner_ReportStringId.UD_Group_ToolbarsList',
            'Field Area for a New Field': 'dx_reportDesigner_ReportStringId.STag_Name_FieldArea',
            'Incorrect band type': 'dx_reportDesigner_ReportStringId.Msg_IncorrectBandType',
            'Remove the spacing between the selected controls': 'dx_reportDesigner_ReportStringId.UD_Hint_SpacingRemove',
            'Multi-Column Layout': 'dx_reportDesigner_ReportStringId.STag_Name_ColumnLayout',
            'Make the spacing between the selected controls equal': 'dx_reportDesigner_ReportStringId.UD_Hint_SpacingMakeEqual',
            'Fields Section Only': 'dx_reportDesigner_PivotGridStringId.CustomizationFormTopPanelOnly',
            'Grand Total': 'dx_reportDesigner_PivotGridStringId.GrandTotal',
            '[Bottom Panel Only 2 by 2 Layout]': 'dx_reportDesigner_PivotGridStringId.Alt_BottomPanelOnly2by2Layout',
            '[Expand]': 'dx_reportDesigner_PivotGridStringId.Alt_Expand',
            '[Top Panel Only Layout]': 'dx_reportDesigner_PivotGridStringId.Alt_TopPanelOnlyLayout',
            '[Column Area Headers]': 'dx_reportDesigner_PivotGridStringId.Alt_ColumnAreaHeaders',
            '(Show Blanks)': 'dx_reportDesigner_PivotGridStringId.FilterShowBlanks',
            'Hide': 'dx_reportDesigner_PivotGridStringId.PopupMenuHideField',
            'Fields Section and Areas Section Side-By-Side': 'dx_reportDesigner_PivotGridStringId.CustomizationFormStackedSideBySide',
            '(Ascending)': 'dx_reportDesigner_PivotGridStringId.Alt_SortedAscending',
            'Error retrieving drilldown dataset': 'dx_reportDesigner_PivotGridStringId.DrillDownException',
            'Good': 'dx_reportDesigner_PivotGridStringId.StatusGood',
            'Bad': 'dx_reportDesigner_PivotGridStringId.StatusBad',
            'Collapse All': 'dx_reportDesigner_PivotGridStringId.PopupMenuCollapseAll',
            'Collapse': 'dx_reportDesigner_PivotGridStringId.PopupMenuCollapse',
            'Row field:': 'dx_reportDesigner_PivotGridStringId.SummaryFilterRowField',
            'Drop Column Fields Here': 'dx_reportDesigner_PivotGridStringId.ColumnHeadersCustomization',
            'Headers': 'dx_reportDesigner_PivotGridStringId.PrintDesignerCategoryHeaders',
            'An error occurs in the Prefilter criteria. Please detect invalid property captions inside the criteria operands and correct or remove them.': 'dx_reportDesigner_PivotGridStringId.PrefilterInvalidCriteria',
            'Drop Data Items Here': 'dx_reportDesigner_PivotGridStringId.DataHeadersCustomization',
            'Add To': 'dx_reportDesigner_PivotGridStringId.CustomizationFormAddTo',
            'Move to Left': 'dx_reportDesigner_PivotGridStringId.PopupMenuMovetoLeft',
            'Move to Right': 'dx_reportDesigner_PivotGridStringId.PopupMenuMovetoRight',
            'Row Headers': 'dx_reportDesigner_PivotGridStringId.PrintDesignerRowHeaders',
            '(Descending)': 'dx_reportDesigner_PivotGridStringId.Alt_SortedDescending',
            '{0} StdDev': 'dx_reportDesigner_PivotGridStringId.TotalFormatStdDev',
            'Hidden Fields': 'dx_reportDesigner_PivotGridStringId.CustomizationFormHiddenFields',
            'Drag Items to the PivotGrid': 'dx_reportDesigner_PivotGridStringId.CustomizationFormText',
            'Drag fields between areas below:': 'dx_reportDesigner_PivotGridStringId.CustomizationFormHint',
            'Clear Rules from All Measures': 'dx_reportDesigner_PivotGridStringId.PopupMenuFormatRulesClearAllRules',
            'Expand All': 'dx_reportDesigner_PivotGridStringId.PopupMenuExpandAll',
            '[Filter Area Headers]': 'dx_reportDesigner_PivotGridStringId.Alt_FilterAreaHeaders',
            'Vertical Lines': 'dx_reportDesigner_PivotGridStringId.PrintDesignerVerticalLines',
            'Show Details command cannot be executed when multiple items are selected in a report filter field. Select a single item for each field in the report filter area before performing a drillthrough.': 'dx_reportDesigner_PivotGridStringId.OLAPDrillDownFilterException',
            'PivotGrid Prefilter': 'dx_reportDesigner_PivotGridStringId.PrefilterFormCaption',
            'Total': 'dx_reportDesigner_PivotGridStringId.Total',
            '[Filtered]': 'dx_reportDesigner_PivotGridStringId.Alt_FilterButtonActive',
            '[Hidden Field\'s Headers]': 'dx_reportDesigner_PivotGridStringId.Alt_FieldListHeaders',
            '[Stacked Side By Side Layout]': 'dx_reportDesigner_PivotGridStringId.Alt_StackedSideBySideLayout',
            '[Filter]': 'dx_reportDesigner_PivotGridStringId.Alt_FilterButton',
            '[Bottom Panel Only 1 by 4 Layout]': 'dx_reportDesigner_PivotGridStringId.Alt_BottomPanelOnly1by4Layout',
            'Max Visible Count': 'dx_reportDesigner_PivotGridStringId.SummaryFilterMaxVisibleCount',
            '{0} Count': 'dx_reportDesigner_PivotGridStringId.TotalFormatCount',
            '{0} Custom': 'dx_reportDesigner_PivotGridStringId.TotalFormatCustom',
            'Print Designer': 'dx_reportDesigner_PivotGridStringId.PrintDesigner',
            'Expression Editor...': 'dx_reportDesigner_PivotGridStringId.PopupMenuShowExpression',
            'Var': 'dx_reportDesigner_PivotGridStringId.SummaryVar',
            'Reload Data': 'dx_reportDesigner_PivotGridStringId.PopupMenuRefreshData',
            'Invert Filter': 'dx_reportDesigner_PivotGridStringId.FilterInvert',
            'Q{0}': 'dx_reportDesigner_PivotGridStringId.DateTimeQuarterFormat',
            'Cancel': 'dx_reportDesigner_PivotGridStringId.FilterCancel',
            'Hidden': 'dx_reportDesigner_PivotGridStringId.SummaryFilterLegendHidden',
            'Going Down': 'dx_reportDesigner_PivotGridStringId.TrendGoingDown',
            'Expand': 'dx_reportDesigner_PivotGridStringId.PopupMenuExpand',
            'PivotGrid Field List': 'dx_reportDesigner_PivotGridStringId.CustomizationFormCaption',
            'In order to use the PivotGrid OLAP functionality, you should have a MS OLAP OleDb provider installed on your system.\r\nYou can download it here:': 'dx_reportDesigner_PivotGridStringId.OLAPNoOleDbProvidersMessage',
            'Show values from': 'dx_reportDesigner_PivotGridStringId.SummaryFilterRangeFrom',
            '(Blank)': 'dx_reportDesigner_PivotGridStringId.FilterBlank',
            '[Data Area Headers]': 'dx_reportDesigner_PivotGridStringId.Alt_DataAreaHeaders',
            '[Resize]': 'dx_reportDesigner_PivotGridStringId.Alt_FilterWindowSizeGrip',
            'KPIs': 'dx_reportDesigner_PivotGridStringId.OLAPKPIsCaption',
            'StdDevp': 'dx_reportDesigner_PivotGridStringId.SummaryStdDevp',
            'Apply to specific level': 'dx_reportDesigner_PivotGridStringId.SummaryFilterApplyToSpecificLevel',
            'Edit Prefilter': 'dx_reportDesigner_PivotGridStringId.EditPrefilter',
            'Headers On Every Page': 'dx_reportDesigner_PivotGridStringId.PrintDesignerHeadersOnEveryPage',
            'Horizontal Lines': 'dx_reportDesigner_PivotGridStringId.PrintDesignerHorizontalLines',
            'Clear Sorting': 'dx_reportDesigner_PivotGridStringId.PopupMenuClearSorting',
            'Show Field List': 'dx_reportDesigner_PivotGridStringId.PopupMenuShowFieldList',
            'OK': 'dx_reportDesigner_PivotGridStringId.FilterOk',
            '[Layout Button]': 'dx_reportDesigner_PivotGridStringId.Alt_LayoutButton',
            '[Row Area Headers]': 'dx_reportDesigner_PivotGridStringId.Alt_RowAreaHeaders',
            '[Stacked Default Layout]': 'dx_reportDesigner_PivotGridStringId.Alt_StackedDefaultLayout',
            'Varp': 'dx_reportDesigner_PivotGridStringId.SummaryVarp',
            'No Change': 'dx_reportDesigner_PivotGridStringId.TrendNoChange',
            '{0} Varp': 'dx_reportDesigner_PivotGridStringId.TotalFormatVarp',
            'Format Rules': 'dx_reportDesigner_PivotGridStringId.PopupMenuFormatRules',
            'to': 'dx_reportDesigner_PivotGridStringId.SummaryFilterRangeTo',
            'Remove All Sorting': 'dx_reportDesigner_PivotGridStringId.PopupMenuRemoveAllSortByColumn',
            'Column Headers': 'dx_reportDesigner_PivotGridStringId.PrintDesignerColumnHeaders',
            'Field Values': 'dx_reportDesigner_PivotGridStringId.PrintDesignerCategoryFieldValues',
            'Apply only to specific level': 'dx_reportDesigner_PivotGridStringId.PopupMenuFormatRulesIntersectionOnly',
            'Drag a field here to customize layout': 'dx_reportDesigner_PivotGridStringId.CustomizationFormListBoxText',
            'StdDev': 'dx_reportDesigner_PivotGridStringId.SummaryStdDev',
            'This command cannot be used on multiple selections.': 'dx_reportDesigner_PivotGridStringId.CannotCopyMultipleSelections',
            'Clear Rules from This Intersection': 'dx_reportDesigner_PivotGridStringId.PopupMenuFormatRulesClearIntersectionRules',
            'Sort "{0}" by This Row': 'dx_reportDesigner_PivotGridStringId.PopupMenuSortFieldByRow',
            'Going Up': 'dx_reportDesigner_PivotGridStringId.TrendGoingUp',
            'Defer Layout Update': 'dx_reportDesigner_PivotGridStringId.CustomizationFormDeferLayoutUpdate',
            'Choose fields to add to report:': 'dx_reportDesigner_PivotGridStringId.CustomizationFormAvailableFields',
            'Search': 'dx_reportDesigner_PivotGridStringId.SearchBoxText',
            'Unused Filter Fields': 'dx_reportDesigner_PivotGridStringId.PrintDesignerUnusedFilterFields',
            'Sort A-Z': 'dx_reportDesigner_PivotGridStringId.PopupMenuSortAscending',
            'Sort Z-A': 'dx_reportDesigner_PivotGridStringId.PopupMenuSortDescending',
            'Drop Row Fields Here': 'dx_reportDesigner_PivotGridStringId.RowHeadersCustomization',
            'Measures': 'dx_reportDesigner_PivotGridStringId.OLAPMeasuresCaption',
            '{0} Average': 'dx_reportDesigner_PivotGridStringId.TotalFormatAverage',
            'Column field:': 'dx_reportDesigner_PivotGridStringId.SummaryFilterColumnField',
            'Drop Filter Fields Here': 'dx_reportDesigner_PivotGridStringId.FilterHeadersCustomization',
            'Sort "{0}" by This Column': 'dx_reportDesigner_PivotGridStringId.PopupMenuSortFieldByColumn',
            '(invalid property)': 'dx_reportDesigner_PivotGridStringId.PrefilterInvalidProperty',
            'Show Prefilter': 'dx_reportDesigner_PivotGridStringId.PopupMenuShowPrefilter',
            '{0} Total': 'dx_reportDesigner_PivotGridStringId.TotalFormat',
            '{0} StdDevp': 'dx_reportDesigner_PivotGridStringId.TotalFormatStdDevp',
            'Move to Beginning': 'dx_reportDesigner_PivotGridStringId.PopupMenuMovetoBeginning',
            'Hide Field List': 'dx_reportDesigner_PivotGridStringId.PopupMenuHideFieldList',
            'Filter Headers': 'dx_reportDesigner_PivotGridStringId.PrintDesignerFilterHeaders',
            'KPI Graphics': 'dx_reportDesigner_PivotGridStringId.PopupMenuKPIGraphic',
            'Fields Section and Areas Section Stacked': 'dx_reportDesigner_PivotGridStringId.CustomizationFormStackedDefault',
            'Customization Form Layout': 'dx_reportDesigner_PivotGridStringId.CustomizationFormLayoutButtonTooltip',
            '(Show All)': 'dx_reportDesigner_PivotGridStringId.FilterShowAll',
            'Hide Prefilter': 'dx_reportDesigner_PivotGridStringId.PopupMenuHidePrefilter',
            'Clear Rules from This Measure': 'dx_reportDesigner_PivotGridStringId.PopupMenuFormatRulesClearMeasureRules',
            'Update': 'dx_reportDesigner_PivotGridStringId.CustomizationFormUpdate',
            'Move to End': 'dx_reportDesigner_PivotGridStringId.PopupMenuMovetoEnd',
            '[Collapse]': 'dx_reportDesigner_PivotGridStringId.Alt_Collapse',
            'Data Headers': 'dx_reportDesigner_PivotGridStringId.PrintDesignerDataHeaders',
            'Areas Section Only (2 by 2)': 'dx_reportDesigner_PivotGridStringId.CustomizationFormBottomPanelOnly2by2',
            'Areas Section Only (1 by 4)': 'dx_reportDesigner_PivotGridStringId.CustomizationFormBottomPanelOnly1by4',
            'Neutral': 'dx_reportDesigner_PivotGridStringId.StatusNeutral',
            '{0} Var': 'dx_reportDesigner_PivotGridStringId.TotalFormatVar',
            '{0} Sum': 'dx_reportDesigner_PivotGridStringId.TotalFormatSum',
            '{0} Max': 'dx_reportDesigner_PivotGridStringId.TotalFormatMax',
            '{0} Min': 'dx_reportDesigner_PivotGridStringId.TotalFormatMin',
            'The palette is default and then can\'t be modified.': 'dx_reportDesigner_ChartStringId.MsgModifyDefaultPaletteError',
            'Export the current document in one of the available formats, and save it to the file on a disk.': 'dx_reportDesigner_ChartStringId.CmdExportPlaceHolderDescription',
            'Spline': 'dx_reportDesigner_ChartStringId.SvnSpline',
            'The distance between the panes should be greater than or equal to 0.': 'dx_reportDesigner_ChartStringId.MsgIncorrectPaneDistance',
            'The chart height must be set in pixels.': 'dx_reportDesigner_ChartStringId.MsgWebInvalidHeightUnit',
            'Compare values across categories and across series and display a pyramid chart on three axes.': 'dx_reportDesigner_ChartStringId.CmdCreatePyramidManhattanBarChartDescription',
            'Relative Position': 'dx_reportDesigner_ChartStringId.AnnotationRelativePosition',
            'Chart Control': 'dx_reportDesigner_ChartStringId.ChartControlDockTarget',
            'NarrowVertical': 'dx_reportDesigner_ChartStringId.WizHatchNarrowVertical',
            'The indent should be greater than or equal to 0.': 'dx_reportDesigner_ChartStringId.MsgIncorrectBarSeriesLabelIndent',
            'Show to what extent values have changed for different points in the same series.': 'dx_reportDesigner_ChartStringId.CmdCreateStepLineChartDescription',
            '"Can\'t set the series point, because it should belong to a series, and the series should be contained in the chart\'s collection.': 'dx_reportDesigner_ChartStringId.MsgIncorrectAnchorPointSeriesPoint',
            'SUM': 'dx_reportDesigner_ChartStringId.FunctionNameSum',
            'MAX': 'dx_reportDesigner_ChartStringId.FunctionNameMax',
            'MIN': 'dx_reportDesigner_ChartStringId.FunctionNameMin',
            'Chart Type': 'dx_reportDesigner_ChartStringId.WizChartTypePageName',
            'Populate the chart\'s datasource with data.': 'dx_reportDesigner_ChartStringId.VerbPopulateDescription',
            '100% Stacked Line in 3-D': 'dx_reportDesigner_ChartStringId.CmdCreateFullStackedLine3DChartMenuCaption',
            'AxisValue can\'t be set to null for the ConstantLine object.': 'dx_reportDesigner_ChartStringId.MsgIncorrectConstantLineAxisValue',
            'The tickmark length should be greater than 0.': 'dx_reportDesigner_ChartStringId.MsgIncorrectTickmarkLength',
            'Indicators changed': 'dx_reportDesigner_ChartStringId.TrnIndicatorsChanged',
            'Combine the advantages of both the 100% Stacked Column and Clustered Column chart types, so that you can stack different columns, and combine them into groups across the same axis value.': 'dx_reportDesigner_ChartStringId.CmdCreateSideBySideFullStackedBarChartDescription',
            'Bar': 'dx_reportDesigner_ChartStringId.SvnSideBySideBar',
            'Polar Line': 'dx_reportDesigner_ChartStringId.CmdCreatePolarLineChartMenuCaption',
            'Annotations...': 'dx_reportDesigner_ChartStringId.VerbAnnotations',
            'Pastel Kit': 'dx_reportDesigner_ChartStringId.AppPastelKit',
            'The pane\'s size in pixels should be greater than or equal to 0.': 'dx_reportDesigner_ChartStringId.MsgInvalidPaneSizeInPixels',
            'Chameleon': 'dx_reportDesigner_ChartStringId.AppChameleon',
            'Print Preview': 'dx_reportDesigner_ChartStringId.CmdPrintPreviewMenuCaption',
            'Image Annotation ': 'dx_reportDesigner_ChartStringId.ImageAnnotationPrefix',
            'Pie': 'dx_reportDesigner_ChartStringId.CmdCreatePieChartMenuCaption',
            'Use Ctrl with the center (wheel) mouse button\r\nto scroll the chart.': 'dx_reportDesigner_ChartStringId.Msg3DScrollingToolTip',
            'Use Ctrl with the left mouse button\r\nto resize the annotation.': 'dx_reportDesigner_ChartStringId.MsgAnnotationResizingToolTip',
            'Populate': 'dx_reportDesigner_ChartStringId.VerbPopulate',
            'The PivotGridDataSourceOptions.{0} property is available only if the chart\'s data source is a PivotGrid.': 'dx_reportDesigner_ChartStringId.MsgPivotGridDataSourceOptionsNotSupprotedProperty',
            'The doughnut hole percentage should be greater than or equal to 0 and less than or equal to 100.': 'dx_reportDesigner_ChartStringId.MsgIncorrectDoughnutHolePercent',
            'There are no visible series to represent in a chart.\r\nTry to add new series, or make sure that\r\nat least one of them is visible.': 'dx_reportDesigner_ChartStringId.MsgEmptyChart',
            'DottedDiamond': 'dx_reportDesigner_ChartStringId.WizHatchDottedDiamond',
            'Use Ctrl with the left mouse button\r\nto rotate the chart.': 'dx_reportDesigner_ChartStringId.Msg3DRotationToolTip',
            'Primary AxisX': 'dx_reportDesigner_ChartStringId.PrimaryAxisXName',
            'Customize the view-type-specific options of a series.\r\nNote that you may select a series by clicking it in the chart preview.': 'dx_reportDesigner_ChartStringId.WizSeriesViewPageDescription',
            'Primary AxisY': 'dx_reportDesigner_ChartStringId.PrimaryAxisYName',
            'Show trends for several series and compare their values for the same points arguments on a circular diagram on the basis of angles.': 'dx_reportDesigner_ChartStringId.CmdCreatePolarLineChartDescription',
            '100% Stacked Line': 'dx_reportDesigner_ChartStringId.CmdCreateFullStackedLineChartMenuCaption',
            'Side By Side Gantt': 'dx_reportDesigner_ChartStringId.SvnSideBySideGantt',
            'Bar 3D': 'dx_reportDesigner_ChartStringId.SvnSideBySideBar3D',
            '100% Stacked Column': 'dx_reportDesigner_ChartStringId.CmdCreateFullStackedBarChartMenuCaption',
            'The ZOrder should be greater than or equal to 0 and less than 100.': 'dx_reportDesigner_ChartStringId.MsgIncorrectAnnotationZOrder',
            'This page was already unregistered.': 'dx_reportDesigner_ChartStringId.MsgUnregisterPageError',
            'Nature Colors': 'dx_reportDesigner_ChartStringId.AppNatureColors',
            'FromCenterVertical': 'dx_reportDesigner_ChartStringId.WizGradientFromCenterVertical',
            'The data snapshot operation is complete. All series data now statically persist in the chart.\r\nThis also means that now the chart is in unbound mode.\r\n\r\nNOTE: You can undo this operation by pressing Ctrl+Z in the Visual Studio designer.': 'dx_reportDesigner_ChartStringId.MsgDataSnapshot',
            '3-D Cylinder': 'dx_reportDesigner_ChartStringId.CmdCreateCylinderManhattanBarChartMenuCaption',
            'Side By Side Bar 3D Stacked': 'dx_reportDesigner_ChartStringId.SvnSideBySideStackedBar3D',
            'The {0} property  can\'t be set to non-integer for the date-time scale.': 'dx_reportDesigner_ChartStringId.MsgIncorrectDateTimeRangeControlClientSpacing',
            'Strips changed': 'dx_reportDesigner_ChartStringId.TrnStripsChanged',
            'Use it when it\'s necessary to show stand-alone data points on the same chart plot.': 'dx_reportDesigner_ChartStringId.CmdCreatePointChartDescription',
            'SmallConfetti': 'dx_reportDesigner_ChartStringId.WizHatchSmallConfetti',
            'The area width should be greater than 0.': 'dx_reportDesigner_ChartStringId.MsgIncorrectAreaWidth',
            'Point': 'dx_reportDesigner_ChartStringId.CmdCreatePointChartMenuCaption',
            'Range Area in 3-D': 'dx_reportDesigner_ChartStringId.CmdCreateRangeArea3DChartMenuCaption',
            'TopToBottom': 'dx_reportDesigner_ChartStringId.WizSeriesLabelTextOrientationTopToBottom',
            'The chart doesn\'t contain an appearance with the {0} name.': 'dx_reportDesigner_ChartStringId.MsgIncorrectAppearanceName',
            'Behave similar to 100% Stacked Area Chart in 3D, but plot a fitted curve through each data point in a series.': 'dx_reportDesigner_ChartStringId.CmdCreateFullStackedSplineArea3DChartDescription',
            'WideUpwardDiagonal': 'dx_reportDesigner_ChartStringId.WizHatchWideUpwardDiagonal',
            'Compare the contribution of each value to a total across categories.': 'dx_reportDesigner_ChartStringId.CmdCreatePyramidStackedBar3DChartDescription',
            'Export to PDF': 'dx_reportDesigner_ChartStringId.CmdExportToPDFMenuCaption',
            'Auto-created Series': 'dx_reportDesigner_ChartStringId.AutocreatedSeriesName',
            'NarrowHorizontal': 'dx_reportDesigner_ChartStringId.WizHatchNarrowHorizontal',
            'Axis can\'t be set to null for the AxisYCoordinate object.': 'dx_reportDesigner_ChartStringId.MsgNullAxisYCoordinateAxis',
            'The {0} is abstract, and so an object of this type can\'t be instantiated and added as a wizard page.': 'dx_reportDesigner_ChartStringId.MsgWizardAbsractPageType',
            'Clear Data Source': 'dx_reportDesigner_ChartStringId.VerbClearDataSource',
            'Area 3D': 'dx_reportDesigner_ChartStringId.SvnArea3D',
            'Side By Side Bar 3D Stacked 100%': 'dx_reportDesigner_ChartStringId.SvnSideBySideFullStackedBar3D',
            'The chart doesn\'t contain a palette with the {0} name.': 'dx_reportDesigner_ChartStringId.MsgPaletteNotFound',
            '3-D Column': 'dx_reportDesigner_ChartStringId.CmdCreateManhattanBarChartMenuCaption',
            'Other': 'dx_reportDesigner_ChartStringId.RibbonOtherPageCaption',
            'Export to RTF': 'dx_reportDesigner_ChartStringId.CmdExportToRTFMenuCaption',
            'The legend horizontal indent should be greater than or equal to 0 and less than 1000.': 'dx_reportDesigner_ChartStringId.MsgIncorrectLegendHorizontalIndent',
            'Stacked Line': 'dx_reportDesigner_ChartStringId.CmdCreateStackedLineChartMenuCaption',
            'Open the Annotations Collection Editor.': 'dx_reportDesigner_ChartStringId.VerbAnnotationsDescription',
            'Microsoft Excel 2007 Work Book': 'dx_reportDesigner_ChartStringId.CmdExportToXLSXDescription',
            'The weight of the pane should be greater than 0.': 'dx_reportDesigner_ChartStringId.MsgIncorrectPaneWeight',
            'Secondary axis X deleted': 'dx_reportDesigner_ChartStringId.TrnSecondryAxisXDeleted',
            'Run the Chart Wizard, which allows the properties of the chart to be edited.': 'dx_reportDesigner_ChartStringId.VerbWizardDescription',
            'The maximum width of the label should be greater than or equal to 0.': 'dx_reportDesigner_ChartStringId.MsgIncorrectLabelMaxWidth',
            'Auto Size': 'dx_reportDesigner_ChartStringId.WizChartImageSizeModeAutoSize',
            'Display a wide area at the top, indicating the total points\' value, while other areas are proportionally smaller.\r\n\r\nUse it when it is necessary to represent stages in a sales process, show the amount of potential revenue for each stage, as well as identify potential problem areas in an organization\'s sales processes.\r\n    ': 'dx_reportDesigner_ChartStringId.CmdCreateFunnelChartDescription',
            'Manhattan Bar': 'dx_reportDesigner_ChartStringId.SvnManhattanBar',
            'It is impossible to set a custom range, if the DateTimeScaleMode is not Manual.': 'dx_reportDesigner_ChartStringId.MsgUnsupportedManualRangeForAutomaticDateTimeScaleMode',
            'Step Line in 3-D': 'dx_reportDesigner_ChartStringId.CmdCreateStepLine3DChartMenuCaption',
            'The length of the minor tickmark should be greater than 0.': 'dx_reportDesigner_ChartStringId.MsgIncorrectTickmarkMinorLength',
            'MaxValueInternal can\'t be set to NaN and Infinity values.': 'dx_reportDesigner_ChartStringId.MsgIncorrectAxisRangeMaxValueInternal',
            'Funnel': 'dx_reportDesigner_ChartStringId.SvnFunnel',
            'Compare values across categories and display clustered columns in 3-D format.': 'dx_reportDesigner_ChartStringId.CmdCreateBar3DChartDescription',
            'The line length should be greater than or equal to 0 and less than 1000.': 'dx_reportDesigner_ChartStringId.MsgIncorrectSeriesLabelLineLength',
            'You can\'t manually change the series point\'s value, because a chart is bound to data.': 'dx_reportDesigner_ChartStringId.MsgDenyChangeSeriesPointValue',
            'Scatter Radar Line': 'dx_reportDesigner_ChartStringId.SvnScatterRadarLine',
            '2-D Area': 'dx_reportDesigner_ChartStringId.CmdArea2DGroupPlaceHolderMenuCaption',
            'Compare values across categories and across series and display a cone chart on three axes.': 'dx_reportDesigner_ChartStringId.CmdCreateConeManhattanBarChartDescription',
            'Open Value': 'dx_reportDesigner_ChartStringId.OpenValuePatternDescription',
            'BottomRightToTopLeft': 'dx_reportDesigner_ChartStringId.WizGradientBottomRightToTopLeft',
            'Chart titles changed': 'dx_reportDesigner_ChartStringId.TrnChartTitlesChanged',
            'Red Orange': 'dx_reportDesigner_ChartStringId.PltRedOrange',
            'Clustered Pyramid': 'dx_reportDesigner_ChartStringId.CmdCreatePyramidBar3DChartMenuCaption',
            'Size in pixels should be greater than or equal to -1 and less than 50.': 'dx_reportDesigner_ChartStringId.MsgInvalidSizeInPixels',
            'The {0} SeriesView doesn\'t exist.': 'dx_reportDesigner_ChartStringId.MsgSeriesViewDoesNotExist',
            '(invisible)': 'dx_reportDesigner_ChartStringId.InvisibleSeriesView',
            'Line 3D': 'dx_reportDesigner_ChartStringId.SvnLine3D',
            'Scatter Polar Line': 'dx_reportDesigner_ChartStringId.CmdCreateScatterPolarLineChartMenuCaption',
            'The min limit of the strip should be less than the max limit.': 'dx_reportDesigner_ChartStringId.MsgIncorrectStripMinLimit',
            'the argument scale type': 'dx_reportDesigner_ChartStringId.MsgIncompatibleByArgumentScaleType',
            'Indicator deleted': 'dx_reportDesigner_ChartStringId.TrnIndicatorDeleted',
            'Nested Doughnut': 'dx_reportDesigner_ChartStringId.CmdCreateNestedDoughnutChartMenuCaption',
            'Tagged Image File Format': 'dx_reportDesigner_ChartStringId.CmdExportToTIFFDescription',
            'Dark Flat': 'dx_reportDesigner_ChartStringId.AppDarkFlat',
            'Blue Warm': 'dx_reportDesigner_ChartStringId.PltBlueWarm',
            'Editing isn\'t allowed!': 'dx_reportDesigner_ChartStringId.MsgPaletteEditingIsntAllowed',
            'Bubble': 'dx_reportDesigner_ChartStringId.SvnBubble',
            'Stacked Area in 3-D': 'dx_reportDesigner_ChartStringId.CmdCreateStackedArea3DChartMenuCaption',
            'Insert a point, funnel, financial, radar, polar, range, or gantt chart.': 'dx_reportDesigner_ChartStringId.CmdCreateOtherSeriesTypesChartPlaceHolderDescription',
            'Palettes changed': 'dx_reportDesigner_ChartStringId.TrnPalettesChanged',
            'Export to HTML': 'dx_reportDesigner_ChartStringId.CmdExportToHTMLMenuCaption',
            'The task link\'s minimum indent should be greater than or equal to 0.': 'dx_reportDesigner_ChartStringId.MsgIncorrectTaskLinkMinIndent',
            'The annotation height should be greater than 0.': 'dx_reportDesigner_ChartStringId.MsgIncorrectAnnotationHeight',
            'TODO.': 'dx_reportDesigner_ChartStringId.CmdCreateScatterRadarLineChartDescription',
            'TopNOptions can\'t be enabled for this series, because either its ValueScaleType is not Numerical or its data points have more than 1 value.': 'dx_reportDesigner_ChartStringId.MsgUnsupportedTopNOptions',
            'The GridAlignment property must be greater than or equal to the current measure unit.': 'dx_reportDesigner_ChartStringId.MsgIncorrectDateTimeGridAlignment',
            'Right-top': 'dx_reportDesigner_ChartStringId.WizDockCornerRightTop',
            'DashedUpwardDiagonal': 'dx_reportDesigner_ChartStringId.WizHatchDashedUpwardDiagonal',
            'The logarithmic base should be greater than 1.': 'dx_reportDesigner_ChartStringId.MsgInvalidLogarithmicBase',
            'The fixed series distance should be greater than or equal to 0.': 'dx_reportDesigner_ChartStringId.MsgIncorrectSeriesDistanceFixed',
            'JustifyAroundPoint': 'dx_reportDesigner_ChartStringId.WizResolveOverlappingModeJustifyAroundPoint',
            'LightVertical': 'dx_reportDesigner_ChartStringId.WizHatchLightVertical',
            'Show variation in stock prices over the course of a day. The Open and Close prices are represented by left and right lines on each point, and the Low and High prices are represented by the bottom and top values of the vertical line which is shown at each point.': 'dx_reportDesigner_ChartStringId.CmdCreateStockChartDescription',
            'Bitmap Picture': 'dx_reportDesigner_ChartStringId.CmdExportToBMPDescription',
            'numeric': 'dx_reportDesigner_ChartStringId.ScaleTypeNumerical',
            'Bar Stacked 100%': 'dx_reportDesigner_ChartStringId.SvnFullStackedBar',
            '3-D Area': 'dx_reportDesigner_ChartStringId.CmdArea3DGroupPlaceHolderMenuCaption',
            'Annotation deleted': 'dx_reportDesigner_ChartStringId.TrnAnnotationDeleted',
            'Foundry': 'dx_reportDesigner_ChartStringId.PltFoundry',
            'TopRightToBottomLeft': 'dx_reportDesigner_ChartStringId.WizGradientTopRightToBottomLeft',
            'Chart wizard settings applied': 'dx_reportDesigner_ChartStringId.TrnChartWizard',
            'Use Ctrl with the left mouse button\r\nto move the annotation.': 'dx_reportDesigner_ChartStringId.MsgAnnotationMovingToolTip',
            'DashedDownwardDiagonal': 'dx_reportDesigner_ChartStringId.WizHatchDashedDownwardDiagonal',
            '100% Stacked Cylinder': 'dx_reportDesigner_ChartStringId.CmdCreateCylinderFullStackedBar3DChartMenuCaption',
            'Click the ellipsis button...': 'dx_reportDesigner_ChartStringId.WizSpecifyDataFilters',
            'The series view\'s pane can\'t be set to null.': 'dx_reportDesigner_ChartStringId.MsgNullSeriesViewPane',
            'Gantt': 'dx_reportDesigner_ChartStringId.CmdGanttGroupPlaceHolderMenuCaption',
            'JustifyAllAroundPoints': 'dx_reportDesigner_ChartStringId.WizResolveOverlappingModeJustifyAllAroundPoints',
            'AxisValue can\'t be set to null for the AxisCoordinate object.': 'dx_reportDesigner_ChartStringId.MsgIncorrectAxisCoordinateAxisValue',
            'The specified pane either doesn\'t belong to a chart, or doesn\'t show the current axis whose visibility should be changed.': 'dx_reportDesigner_ChartStringId.MsgInvalidPane',
            'Series ': 'dx_reportDesigner_ChartStringId.SeriesPrefix',
            'The datasource doesn\'t contain a datamember with the "{0}" name.': 'dx_reportDesigner_ChartStringId.MsgIncorrectDataMember',
            'Terracotta Pie': 'dx_reportDesigner_ChartStringId.PltTerracottaPie',
            'Marquee': 'dx_reportDesigner_ChartStringId.PltMarquee',
            'Image Annotation': 'dx_reportDesigner_ChartStringId.ImageAnnotation',
            'DevExpress Scheduler holidays files (*.xml)|*.xml|Microsoft Office Outlook holidays files (*.hol)|*.hol|Text files (*.txt)|*.txt|All files (*.*)|*.*': 'dx_reportDesigner_ChartStringId.HolidaysImportFilter',
            'Combine the advantages of both the 100% Stacked Column and Clustered Column chart types in 3-D format, so that you can stack different columns, and combine them into groups across the same axis value.': 'dx_reportDesigner_ChartStringId.CmdCreateSideBySideFullStackedBar3DChartDescription',
            'Display data as filled areas on a diagram, with each data point displayed as a peak or hollow in the area. Use it when you need to show trends for several series on the same diagram, and also show the relationship of the parts to the whole.': 'dx_reportDesigner_ChartStringId.CmdCreateArea3DChartDescription',
            'Insert an area chart.\r\n\r\nArea charts emphasize differences between several sets of data over a period of time.\r\n    ': 'dx_reportDesigner_ChartStringId.CmdCreateAreaChartDescription',
            'Export to Image': 'dx_reportDesigner_ChartStringId.CmdExportToImagePlaceHolderMenuCaption',
            'Behave similar to Stacked Area Chart but plot a fitted curve through each data point in a series.': 'dx_reportDesigner_ChartStringId.CmdCreateStackedSplineAreaChartDescription',
            'Image Files(*.gif;*.jpg;*.jpeg;*.bmp;*.wmf;*.png)|*.gif;*.jpg;*.jpeg;*.bmp;*.wmf;*.png|All files(*.*)|*.*': 'dx_reportDesigner_ChartStringId.WizBackImageFileNameFilter',
            'Value must be equal or greater then 0.': 'dx_reportDesigner_ChartStringId.MsgValueMustBeGreateThenZero',
            'Compare values across categories and across series and display a cylinder chart on three axes.': 'dx_reportDesigner_ChartStringId.CmdCreateCylinderManhattanBarChartDescription',
            'The chart width must be set in pixels.': 'dx_reportDesigner_ChartStringId.MsgWebInvalidWidthUnit',
            'The current measure unit should be greater than 0.': 'dx_reportDesigner_ChartStringId.MsgIncorrectNumericMeasureUnit',
            'Display series as filled areas on a diagram, with two data points that define minimum and maximum limits.\r\n\r\nUse it when you need to accentuate the delta between start and end values.\r\n    ': 'dx_reportDesigner_ChartStringId.CmdCreateRangeAreaChartDescription',
            'Combine the advantages of both the Stacked Column and Clustered Column chart types in 3-D format, so that you can stack different columns, and combine them into groups across the same axis value.': 'dx_reportDesigner_ChartStringId.CmdCreateSideBySideStackedBar3DChartDescription',
            'Can\'t set the AxisXCoordinate\'s axis, because the specified axis isn\'t primary and isn\'t contained in the diagram\'s collection of secondary X-axes.': 'dx_reportDesigner_ChartStringId.MsgIncorrectAxisXCoordinateAxis',
            'Stacked Column in 3-D': 'dx_reportDesigner_ChartStringId.CmdCreateStackedBar3DChartMenuCaption',
            'LightUpwardDiagonal': 'dx_reportDesigner_ChartStringId.WizHatchLightUpwardDiagonal',
            'Free Position': 'dx_reportDesigner_ChartStringId.CrosshairLabelFreePosition',
            'Red Violet': 'dx_reportDesigner_ChartStringId.PltRedViolet',
            'Constant line deleted': 'dx_reportDesigner_ChartStringId.TrnConstantLineDeleted',
            'Behave similar to 100% Stacked Area, but plot a fitted curve through each data point in a series.': 'dx_reportDesigner_ChartStringId.CmdCreateFullStackedSplineAreaChartDescription',
            'The indent value should be greater than or equal to 0.': 'dx_reportDesigner_ChartStringId.MsgIncorrectIndentFromMarker',
            'The connector length should be greater than or equal to 0 and less than 1000.': 'dx_reportDesigner_ChartStringId.MsgIncorrectRelativePositionConnectorLength',
            'Load a chart from an XML file.': 'dx_reportDesigner_ChartStringId.VerbLoadLayoutDescription',
            'LightDownwardDiagonal': 'dx_reportDesigner_ChartStringId.WizHatchLightDownwardDiagonal',
            'Annotations changed': 'dx_reportDesigner_ChartStringId.TrnAnnotationsChanged',
            'See basic information on XtraCharts.': 'dx_reportDesigner_ChartStringId.VerbAboutDescription',
            'Behave similar to Area Chart but plot a fitted curve through each data point in a series.': 'dx_reportDesigner_ChartStringId.CmdCreateSplineAreaChartDescription',
            'Compare the contribution of each value to a total across categories by using horizontal rectangles.': 'dx_reportDesigner_ChartStringId.CmdCreateRotatedStackedBarChartDescription',
            'Red': 'dx_reportDesigner_ChartStringId.PltRed',
            'The{0} chart{1}.': 'dx_reportDesigner_ChartStringId.AlternateTextPlaceholder',
            'All Colors': 'dx_reportDesigner_ChartStringId.StyleAllColors',
            'Percent70': 'dx_reportDesigner_ChartStringId.WizHatchPercent70',
            'Percent75': 'dx_reportDesigner_ChartStringId.WizHatchPercent75',
            'Percent60': 'dx_reportDesigner_ChartStringId.WizHatchPercent60',
            'Percent50': 'dx_reportDesigner_ChartStringId.WizHatchPercent50',
            'Percent40': 'dx_reportDesigner_ChartStringId.WizHatchPercent40',
            'Percent30': 'dx_reportDesigner_ChartStringId.WizHatchPercent30',
            'Percent25': 'dx_reportDesigner_ChartStringId.WizHatchPercent25',
            'Percent20': 'dx_reportDesigner_ChartStringId.WizHatchPercent20',
            'Percent10': 'dx_reportDesigner_ChartStringId.WizHatchPercent10',
            'Percent05': 'dx_reportDesigner_ChartStringId.WizHatchPercent05',
            'Percent90': 'dx_reportDesigner_ChartStringId.WizHatchPercent90',
            'Percent80': 'dx_reportDesigner_ChartStringId.WizHatchPercent80',
            'Add titles to your chart, and customize their options.': 'dx_reportDesigner_ChartStringId.WizChartTitlesPageDescription',
            'Spline Area 3D': 'dx_reportDesigner_ChartStringId.SvnSpline3DArea',
            'Compare values across categories.': 'dx_reportDesigner_ChartStringId.CmdCreateConeBar3DChartDescription',
            'Palettes...': 'dx_reportDesigner_ChartStringId.VerbEditPalettes',
            'The GridAlignment property can\'t be modified in the automatic date-time scale mode.': 'dx_reportDesigner_ChartStringId.MsgIncorrectDateTimeGridAlignmentPropertyUsing',
            'Compare the percentage each value contributes to a total across categories and display 100% stacked columns in 3-D format.': 'dx_reportDesigner_ChartStringId.CmdCreateFullStackedBar3DChartDescription',
            'JPEG': 'dx_reportDesigner_ChartStringId.CmdExportToJPEGMenuCaption',
            'Rich Text Format': 'dx_reportDesigner_ChartStringId.CmdExportToRTFDescription',
            'Show points from two or more different series on the same circular diagram on the basis of angles.': 'dx_reportDesigner_ChartStringId.CmdCreatePolarPointChartDescription',
            'Side By Side Bar Stacked': 'dx_reportDesigner_ChartStringId.SvnSideBySideStackedBar',
            'Anchor Point can\'t be null.': 'dx_reportDesigner_ChartStringId.MsgIncorrectAnchorPoint',
            'Northern Lights': 'dx_reportDesigner_ChartStringId.PltNorthernLights',
            'The PolarAxisX doesn\'t support logarithmic mode.': 'dx_reportDesigner_ChartStringId.MsgPolarAxisXLogarithmic',
            'Strip ': 'dx_reportDesigner_ChartStringId.StripPrefix',
            'A pane\'s name can\'t be empty.': 'dx_reportDesigner_ChartStringId.MsgEmptyPaneName',
            'Select a placeholder to see the preview': 'dx_reportDesigner_ChartStringId.PatternEditorPreviewCaption',
            'MaxValue can\'t be set to null.': 'dx_reportDesigner_ChartStringId.MsgIncorrectAxisRangeMaxValue',
            'The marker size should be greater than 1.': 'dx_reportDesigner_ChartStringId.MsgIncorrectMarkerSize',
            'Reset legend point options': 'dx_reportDesigner_ChartStringId.VerbResetLegendPointOptions',
            'The exploded distance percentage value should be greater than 0.': 'dx_reportDesigner_ChartStringId.MsgIncorrectExplodedDistancePercentage',
            'AxisValue can\'t be set to null for the CustomAxisLabel object.': 'dx_reportDesigner_ChartStringId.MsgIncorrectCustomAxisLabelAxisValue',
            'This view can\'t represent negative\r\nvalues. All values must be either greater\r\nthan or equal to zero.': 'dx_reportDesigner_ChartStringId.PieIncorrectValuesText',
            'Relation\'s ChildPointID must be unique.': 'dx_reportDesigner_ChartStringId.MsgRelationChildPointIDNotUnique',
            'Select a printer, number of copies and other printing options before printing.': 'dx_reportDesigner_ChartStringId.CmdPrintDescription',
            'Color {0}': 'dx_reportDesigner_ChartStringId.StyleColorNumber',
            'The scroll bar thickness should be greater than or equal to 3 and less than or equal to 25.': 'dx_reportDesigner_ChartStringId.MsgIncorrectScrollBarThickness',
            'A secondary axis name can\'t be empty.': 'dx_reportDesigner_ChartStringId.MsgEmptySecondaryAxisName',
            'Combine the advantages of both the Stacked Cone and Clustered Cone chart types, so that you can stack different cones, and combine them into groups across the same axis value.': 'dx_reportDesigner_ChartStringId.CmdCreateConeSideBySideStackedBar3DChartDescription',
            'Combine the advantages of both the Stacked Pyramid and Clustered Pyramid chart types, so that you can stack different pyramids, and combine them into groups across the same axis value.': 'dx_reportDesigner_ChartStringId.CmdCreatePyramidSideBySideStackedBar3DChartDescription',
            'ImageUrl property can be used for the WebChartControl only. Please, use the Image property instead.': 'dx_reportDesigner_ChartStringId.MsgIncorrectUseImageUrlProperty',
            'The vertical scroll percent should be greater than or equal to -{0} and less than or equal to {0}.': 'dx_reportDesigner_ChartStringId.MsgIncorrectVerticalScrollPercent',
            'FromCenterHorizontal': 'dx_reportDesigner_ChartStringId.WizGradientFromCenterHorizontal',
            '(incompatible)': 'dx_reportDesigner_ChartStringId.IncompatibleSeriesView',
            'Since the current Pie series view displays the series created using a series template, the specified series point can\'t be removed from the collection of exploded points. You need to use another Explode Mode instead.': 'dx_reportDesigner_ChartStringId.MsgInvalidExplodedModeRemove',
            'Chart Tools': 'dx_reportDesigner_ChartStringId.RibbonPageCategoryCaption',
            'LegendPointOptions reset': 'dx_reportDesigner_ChartStringId.TrnLegendPointOptionsReset',
            'Run Chart Wizard...': 'dx_reportDesigner_ChartStringId.CmdRunWizardMenuCaption',
            'Automatic DateTimeScaleMode can\'t work together with zooming and scrolling.': 'dx_reportDesigner_ChartStringId.MsgUnsupportedDateTimeScaleModeWithScrollingZooming',
            'Double-click to edit...': 'dx_reportDesigner_ChartStringId.MsgPaletteDoubleClickToEdit',
            'The thickness of the minor tickmark should be greater than 0.': 'dx_reportDesigner_ChartStringId.MsgIncorrectTickmarkMinorThickness',
            'Load\r\nTemplate': 'dx_reportDesigner_ChartStringId.CmdLoadTemplateMenuCaption',
            'Black and White': 'dx_reportDesigner_ChartStringId.PltBlackAndWhite',
            'Add': 'dx_reportDesigner_ChartStringId.MenuItemAdd',
            'LargeCheckerBoard': 'dx_reportDesigner_ChartStringId.WizHatchLargeCheckerBoard',
            'Stacked Column': 'dx_reportDesigner_ChartStringId.CmdCreateStackedBarChartMenuCaption',
            'It\'s impossible to swap autocreated and fixed series.': 'dx_reportDesigner_ChartStringId.MsgCantSwapSeries',
            'Series deleted': 'dx_reportDesigner_ChartStringId.TrnSeriesDeleted',
            'Clustered Stacked Cone': 'dx_reportDesigner_ChartStringId.CmdCreateConeSideBySideStackedBar3DChartMenuCaption',
            'Insert a bar chart.\r\n\r\nBar charts are the best chart type for comparing multiple values.\r\n    ': 'dx_reportDesigner_ChartStringId.CmdCreateRotatedBarChartDescription',
            '3-D Line': 'dx_reportDesigner_ChartStringId.CmdCreateLine3DChartMenuCaption',
            'Opulent': 'dx_reportDesigner_ChartStringId.PltOpulent',
            'Create a template with the same setting as the current chart.': 'dx_reportDesigner_ChartStringId.CmdSaveAsTemplateDescription',
            'ExplodedPointsFilters changed': 'dx_reportDesigner_ChartStringId.TrnExplodedPointsFilters',
            'Summarize and display categories of data and compare amounts or values between different categories.': 'dx_reportDesigner_ChartStringId.CmdCreateRotatedBarChartPlaceHolderDescription',
            'The MeasureUnit property can\'t be modified in both the automatic and continuous numeric scale modes.': 'dx_reportDesigner_ChartStringId.MsgIncorrectNumericMeasureUnitPropertyUsing',
            'Spline Area Stacked 100%': 'dx_reportDesigner_ChartStringId.SvnSplineFullStackedArea',
            'Pane can\'t be set to null for the PaneAnchorPoint object.': 'dx_reportDesigner_ChartStringId.MsgNullPaneAnchorPointPane',
            'You can\'t manually change the series point\'s argument, because a chart is bound to data.': 'dx_reportDesigner_ChartStringId.MsgDenyChangeSeriesPointAgument',
            'Series point can\'t have a relation to itself.': 'dx_reportDesigner_ChartStringId.MsgSelfRelatedSeriesPoint',
            'The bar depth should be greater than 0.': 'dx_reportDesigner_ChartStringId.MsgIncorrectBarDepth',
            'Axis of values': 'dx_reportDesigner_ChartStringId.AxisYDefaultTitle',
            'Tool Tip Position can\'t be null.': 'dx_reportDesigner_ChartStringId.MsgIncorrectToolTipPosition',
            'Trek': 'dx_reportDesigner_ChartStringId.PltTrek',
            'Flow': 'dx_reportDesigner_ChartStringId.PltFlow',
            'Apex': 'dx_reportDesigner_ChartStringId.PltApex',
            'Blue': 'dx_reportDesigner_ChartStringId.PltBlue',
            'The bar width should be greater than 0.': 'dx_reportDesigner_ChartStringId.MsgIncorrectBarWidth',
            'Chart Titles': 'dx_reportDesigner_ChartStringId.WizChartTitlesPageName',
            'The {0} value level isn\'t supported by the {1}.': 'dx_reportDesigner_ChartStringId.MsgUnsupportedValueLevel',
            'Exact workdays changed': 'dx_reportDesigner_ChartStringId.TrnExactWorkdaysChanged',
            'File \'{0}\' isn\'t found.': 'dx_reportDesigner_ChartStringId.MsgFileNotFound',
            'The "SynchronizePointOptions" property can\'t be set at runtime.': 'dx_reportDesigner_ChartStringId.MsgSynchronizePointOptionsSettingRuntimeError',
            'The points count should be greater than 1.': 'dx_reportDesigner_ChartStringId.MsgIncorrectPointsCount',
            'The {0} property should be greater than 0.': 'dx_reportDesigner_ChartStringId.MsgIncorrectRangeControlClientSpacing',
            'Chart title deleted': 'dx_reportDesigner_ChartStringId.TrnChartTitleDeleted',
            'Regression Line': 'dx_reportDesigner_ChartStringId.IndRegressionLine',
            'Fibonacci Indicator': 'dx_reportDesigner_ChartStringId.IndFibonacciIndicator',
            'Point Labels': 'dx_reportDesigner_ChartStringId.WizSeriesLabelsPageName',
            'by {0} with "{1}"': 'dx_reportDesigner_ChartStringId.IncompatibleSeriesMessage',
            'Display the trend of the percentage each value contributes over time or categories.\r\n\r\nUse it to emphasize the trend in the proportion of each series.\r\n    ': 'dx_reportDesigner_ChartStringId.CmdCreateFullStackedAreaChartDescription',
            'Candle Stick': 'dx_reportDesigner_ChartStringId.CmdCreateCandleStickChartMenuCaption',
            '100% Stacked Pyramid': 'dx_reportDesigner_ChartStringId.CmdCreatePyramidFullStackedBar3DChartMenuCaption',
            'There is no panes to anchor to, because the chart\'s diagram type doesn\'t support panes.': 'dx_reportDesigner_ChartStringId.IncorrectDiagramTypeToolTipText',
            'A summary function with the name \'{0}\' is not registered.': 'dx_reportDesigner_ChartStringId.MsgSummaryFunctionIsNotRegistered',
            'Spline Area Stacked': 'dx_reportDesigner_ChartStringId.SvnSplineStackedArea',
            'Swift Plot': 'dx_reportDesigner_ChartStringId.SvnSwiftPlot',
            'Combine the advantages of both the Stacked Cylinder and Clustered Cylinder chart types, so that you can stack different cylinders, and combine them into groups across the same axis value.': 'dx_reportDesigner_ChartStringId.CmdCreateCylinderSideBySideStackedBar3DChartDescription',
            'Exploded points changed': 'dx_reportDesigner_ChartStringId.TrnExplodedPoints',
            '{0} data filter(s)': 'dx_reportDesigner_ChartStringId.WizDataFiltersEntered',
            'Clustered Column': 'dx_reportDesigner_ChartStringId.CmdCreateBarChartMenuCaption',
            'Yellow Orange': 'dx_reportDesigner_ChartStringId.PltYellowOrange',
            'The max limit of the strip should be greater than the min limit.': 'dx_reportDesigner_ChartStringId.MsgIncorrectStripMaxLimit',
            'Combine the advantages of both the 100% Stacked Cone and Clustered Cone chart types, so that you can stack different cones, and combine them into groups across the same axis value.': 'dx_reportDesigner_ChartStringId.CmdCreateConeSideBySideFullStackedBar3DChartDescription',
            'The {0} argument scale type is incompatible with the {1} series view.': 'dx_reportDesigner_ChartStringId.MsgIncompatibleArgumentScaleType',
            'It\'s impossible to set the sorting key\'s value to {0}.': 'dx_reportDesigner_ChartStringId.MsgInvalidSortingKey',
            'The primary axis can\'t be deleted. If you want to hide it, set its Visible property to false.': 'dx_reportDesigner_ChartStringId.IODeleteAxis',
            'The axis thickness should be greater than 0.': 'dx_reportDesigner_ChartStringId.MsgIncorrectAxisThickness',
            '100% Stacked Cone': 'dx_reportDesigner_ChartStringId.CmdCreateConeFullStackedBar3DChartMenuCaption',
            'Clustered 100% Stacked Pyramid': 'dx_reportDesigner_ChartStringId.CmdCreatePyramidSideBySideFullStackedBar3DChartMenuCaption',
            'The percent value should be greater than or equal to 0 and less than or equal to 100.': 'dx_reportDesigner_ChartStringId.MsgIncorrectPercentValue',
            'Illegal Operation': 'dx_reportDesigner_ChartStringId.IOCaption',
            'Bar 3D Stacked 100%': 'dx_reportDesigner_ChartStringId.SvnFullStackedBar3D',
            'Pane Anchor Point': 'dx_reportDesigner_ChartStringId.AnnotationPaneAnchorPoint',
            'Gray': 'dx_reportDesigner_ChartStringId.AppGray',
            'Dark': 'dx_reportDesigner_ChartStringId.AppDark',
            'The specified overlapping mode isn\'t supported by the current series view.': 'dx_reportDesigner_ChartStringId.MsgUnsupportedResolveOverlappingMode',
            'Pie 3D': 'dx_reportDesigner_ChartStringId.SvnPie3D',
            'Stock': 'dx_reportDesigner_ChartStringId.SvnStock',
            'Hit testing for 3D Chart Types isn\'t supported. So, this method is supported for 2D Chart Types only.': 'dx_reportDesigner_ChartStringId.MsgCalcHitInfoNotSupported',
            'Right-bottom': 'dx_reportDesigner_ChartStringId.WizDockCornerRightBottom',
            'DashedVertical': 'dx_reportDesigner_ChartStringId.WizHatchDashedVertical',
            'This group was already unregistered.': 'dx_reportDesigner_ChartStringId.MsgUnregisterGroupError',
            '100% Stacked Area in 3-D': 'dx_reportDesigner_ChartStringId.CmdCreateFullStackedArea3DChartMenuCaption',
            'Assign this pane to the\r\nSeries.View.Pane property,\r\nto show a series on this pane': 'dx_reportDesigner_ChartStringId.MsgEmptyPaneTextForHorizontalLayout',
            'The specified value to convert to the scale\'s internal representation isn\'t compatible with the current scale type.': 'dx_reportDesigner_ChartStringId.MsgInvalidScaleType',
            'Show the variation in the price of stock over the course of a day. The Open and Close prices are represented by a filled rectangle, and the Low and High prices are represented by the bottom and top values of the vertical line which is shown at each point.': 'dx_reportDesigner_ChartStringId.CmdCreateCandleStickChartDescription',
            'Joint Photographic Experts Group': 'dx_reportDesigner_ChartStringId.CmdExportToJPEGDescription',
            'DarkHorizontal': 'dx_reportDesigner_ChartStringId.WizHatchDarkHorizontal',
            'The percentage accuracy should be greater than 0.': 'dx_reportDesigner_ChartStringId.MsgIncorrectPercentageAccuracy',
            'Can\'t set the PaneAnchorPoint\'s pane, because the specified pane isn\'t default and isn\'t contained in the diagram\'s collection of panes.': 'dx_reportDesigner_ChartStringId.MsgIncorrectPaneAnchorPointPane',
            'Polar Point': 'dx_reportDesigner_ChartStringId.SvnPolarPoint',
            'The argument of the financial indicator\'s point can\'t be set to null.': 'dx_reportDesigner_ChartStringId.MsgNullFinancialIndicatorArgument',
            'Funnel 3D': 'dx_reportDesigner_ChartStringId.SvnFunnel3D',
            'Print and Export': 'dx_reportDesigner_ChartStringId.RibbonPrintExportGroupCaption',
            'Weighted Moving Average': 'dx_reportDesigner_ChartStringId.IndWeightedMovingAverage',
            'Portable Network Graphics': 'dx_reportDesigner_ChartStringId.CmdExportToPNGDescription',
            '(None)': 'dx_reportDesigner_ChartStringId.WizNoBackImage',
            'Range Area': 'dx_reportDesigner_ChartStringId.SvnRangeArea',
            'MinValueInternal can\'t be set to NaN and Infinity values.': 'dx_reportDesigner_ChartStringId.MsgIncorrectAxisRangeMinValueInternal',
            'Holidays changed': 'dx_reportDesigner_ChartStringId.TrnHolidaysChanged',
            'Constant Lines changed': 'dx_reportDesigner_ChartStringId.TrnConstantLinesChanged',
            'Compare the percentage that each value contributes to a total across categories by using vertical rectangles.\r\n\r\nUse it to emphasize the proportion of each data series.\r\n    ': 'dx_reportDesigner_ChartStringId.CmdCreateFullStackedBarChartDescription',
            'Combine the advantages of both the 100% Stacked Pyramid and Clustered Pyramid chart types, so that you can stack different pyramids, and combine them into groups across the same axis value.': 'dx_reportDesigner_ChartStringId.CmdCreatePyramidSideBySideFullStackedBar3DChartDescription',
            'Grayscale': 'dx_reportDesigner_ChartStringId.PltGrayscale',
            '\tThe minimum and maximum limits of the Strip can not be the same.': 'dx_reportDesigner_ChartStringId.MsgIncorrectStripConstructorParameters',
            'Panes changed': 'dx_reportDesigner_ChartStringId.TrnXYDiagramPanesChanged',
            'Series title deleted': 'dx_reportDesigner_ChartStringId.TrnSeriesTitleDeleted',
            'DashStyle.Empty can only be assigned to a constant line\'s LineStyle property.': 'dx_reportDesigner_ChartStringId.MsgIncorrectDashStyle',
            'Owner of the parent series point can\'t be null and must be of the Series type.': 'dx_reportDesigner_ChartStringId.MsgIncorrectParentSeriesPointOwner',
            'Clustered 100% Stacked Cone': 'dx_reportDesigner_ChartStringId.CmdCreateConeSideBySideFullStackedBar3DChartMenuCaption',
            'The palette base color number should be greater than or equal to 0 and less than or equal to the total number of palette colors.': 'dx_reportDesigner_ChartStringId.MsgIncorrectPaletteBaseColorNumber',
            'You should specify all of the summary function parameters.': 'dx_reportDesigner_ChartStringId.MsgSummaryFunctionParameterIsNotSpecified',
            'Compare the percentage each value contributes to a total across categories using horizontal rectangles.\r\n\r\nUse it when the values on the chart represent durations or when the category text is very long.\r\n    ': 'dx_reportDesigner_ChartStringId.CmdCreateRotatedFullStackedBarChartDescription',
            '3-D Spline': 'dx_reportDesigner_ChartStringId.CmdCreateSpline3DChartMenuCaption',
            'Cannot set the EqualBarWidth property unless the series is added to the chart\'s collection.': 'dx_reportDesigner_ChartStringId.MsgIncorrectEqualBarWidthPropertyUsing',
            'Compare the percentage each value contributes to a total across categories.': 'dx_reportDesigner_ChartStringId.CmdCreateConeFullStackedBar3DChartDescription',
            'The nested doughnut inner indent should be greater than or equal to 0.': 'dx_reportDesigner_ChartStringId.MsgIncorrectNestedDoughnutInnerIndent',
            'The arrow height should be greater than 0.': 'dx_reportDesigner_ChartStringId.MsgIncorrectArrowHeight',
            'The title indent should be greater than or equal to 0 and less than 1000.': 'dx_reportDesigner_ChartStringId.MsgIncorrectChartTitleIndent',
            'Radar Point': 'dx_reportDesigner_ChartStringId.SvnRadarPoint',
            'Clustered 100% Stacked Column in 3-D': 'dx_reportDesigner_ChartStringId.CmdCreateSideBySideFullStackedBar3DChartMenuCaption',
            '{0} series': 'dx_reportDesigner_ChartStringId.AlternateTextSeriesText',
            'Use Ctrl with the left mouse button\r\nto scroll the chart.': 'dx_reportDesigner_ChartStringId.Msg2DScrollingToolTip',
            'The legend vertical indent should be greater than or equal to 0 and less than 1000.': 'dx_reportDesigner_ChartStringId.MsgIncorrectLegendVerticalIndent',
            'Polar': 'dx_reportDesigner_ChartStringId.CmdPolarGroupPlaceHolderMenuCaption',
            'SeriesPointFilter': 'dx_reportDesigner_ChartStringId.DefaultSeriesPointFilterName',
            'You can\'t add any view type in this collection, because at least one view type must be available in the Wizard.': 'dx_reportDesigner_ChartStringId.MsgAddLastViewType',
            'Series Group': 'dx_reportDesigner_ChartStringId.StackedGroupPatternDescription',
            'Web Page': 'dx_reportDesigner_ChartStringId.CmdExportToHTMLDescription',
            'Clustered Cylinder': 'dx_reportDesigner_ChartStringId.CmdCreateCylinderBar3DChartMenuCaption',
            'The shape fillet should be greater than or equal to 0.': 'dx_reportDesigner_ChartStringId.MsgIncorrectShapeFillet',
            'The specified object isn\'t a ChartControl.': 'dx_reportDesigner_ChartStringId.MsgNotChartControl',
            '3-D Clustered Column': 'dx_reportDesigner_ChartStringId.CmdCreateBar3DChartMenuCaption',
            'Max count should be greater than 0.': 'dx_reportDesigner_ChartStringId.MsgInvalidMaxCount',
            'The grid alignment should be greater than or equal to the current measure unit.': 'dx_reportDesigner_ChartStringId.MsgIncorrectNumericGridAlignment',
            'Enable zooming (true)': 'dx_reportDesigner_ChartStringId.WizEnableZoomingTrue',
            'Custom Axis Labels changed': 'dx_reportDesigner_ChartStringId.TrnCustomAxisLabelChanged',
            'DateTimeScaleMode isn\'t supported for the GanttDiagram.': 'dx_reportDesigner_ChartStringId.MsgUnsupportedDateTimeScaleModeForGanttDiagram',
            'The Trees': 'dx_reportDesigner_ChartStringId.PltTheTrees',
            'Other Charts': 'dx_reportDesigner_ChartStringId.CmdCreateOtherSeriesTypesChartPlaceHolderMenuCaption',
            'The type of the "{0}" value data member isn\'t compatible with the {1} scale.': 'dx_reportDesigner_ChartStringId.MsgIncompatibleValueDataMember',
            'Stacked Line in 3-D': 'dx_reportDesigner_ChartStringId.CmdCreateStackedLine3DChartMenuCaption',
            'Export to XLS': 'dx_reportDesigner_ChartStringId.CmdExportToXLSMenuCaption',
            'Spline Area 3D Stacked 100%': 'dx_reportDesigner_ChartStringId.SvnSplineAreaFullStacked3D',
            'The dimension of the simple diagram should be greater than 0 and less than 100.': 'dx_reportDesigner_ChartStringId.MsgIncorrectSimpleDiagramDimension',
            'The top N threshold percent should be greater than 0 and less than or equal to 100.': 'dx_reportDesigner_ChartStringId.MsgIncorrectTopNThresholdPercent',
            'Stacked Cylinder': 'dx_reportDesigner_ChartStringId.CmdCreateCylinderStackedBar3DChartMenuCaption',
            'Pane deleted': 'dx_reportDesigner_ChartStringId.TrnPaneDeleted',
            'Insert a pie chart.\r\n\r\nPie charts display the contribution of each value to a total.\r\n\r\nUse it when values can be added together or when you have only one data series and all values are positive.\r\n    ': 'dx_reportDesigner_ChartStringId.CmdCreatePieChartDescription',
            'Cannot set the BarDistance property unless the series is added to the chart\'s collection.': 'dx_reportDesigner_ChartStringId.MsgIncorrectBarDistancePropertyUsing',
            'Assign this pane to the Series.View.Pane property,\r\nto show a series on this pane': 'dx_reportDesigner_ChartStringId.MsgEmptyPaneTextForVerticalLayout',
            'Low Value': 'dx_reportDesigner_ChartStringId.LowValuePatternDescription',
            'The top N threshold value should be greater than 0.': 'dx_reportDesigner_ChartStringId.MsgIncorrectTopNThresholdValue',
            'PNG': 'dx_reportDesigner_ChartStringId.CmdExportToPNGMenuCaption',
            'ZigZag': 'dx_reportDesigner_ChartStringId.WizHatchZigZag',
            'The specified summary function string is in an incorrect format.': 'dx_reportDesigner_ChartStringId.MsgIncorrectSummaryFunction',
            'Line Stacked 100%': 'dx_reportDesigner_ChartStringId.SvnFullStackedLine',
            'Area Stacked 100%': 'dx_reportDesigner_ChartStringId.SvnFullStackedArea',
            'Load a chart from template': 'dx_reportDesigner_ChartStringId.CmdLoadTemplateDescription',
            '{0}, {1}pt, {2}': 'dx_reportDesigner_ChartStringId.FontFormat',
            'The array of values must contain either numerical or date-time values.': 'dx_reportDesigner_ChartStringId.MsgIncorrectArrayOfValues',
            'Compare the contribution of each value to a total across categories and display stacked columns in 3-D format.': 'dx_reportDesigner_ChartStringId.CmdCreateStackedBar3DChartDescription',
            'Range Area 3D': 'dx_reportDesigner_ChartStringId.SvnRangeArea3D',
            'Show how much values have changed for different points of the same series.': 'dx_reportDesigner_ChartStringId.CmdCreateStepAreaChartDescription',
            'Compare the contribution of each value to a total across categories by using vertical rectangles.\r\n\r\nUse it to emphasize the total across series for one category.\r\n    ': 'dx_reportDesigner_ChartStringId.CmdCreateStackedBarChartDescription',
            '100% Stacked Spline Area in 3-D': 'dx_reportDesigner_ChartStringId.CmdCreateFullStackedSplineArea3DChartMenuCaption',
            '"Series point can\'t be set to null for the SeriesPointAncherPoint object.': 'dx_reportDesigner_ChartStringId.MsgNullAnchorPointSeriesPoint',
            'Spline Area': 'dx_reportDesigner_ChartStringId.SvnSplineArea',
            'The column indent should be greater than or equal to 0.': 'dx_reportDesigner_ChartStringId.MsgIncorrectPieSeriesLabelColumnIndent',
            'Secondary AxisX ': 'dx_reportDesigner_ChartStringId.SecondaryAxisXPrefix',
            'Secondary axis Y deleted': 'dx_reportDesigner_ChartStringId.TrnSecondryAxisYDeleted',
            'The start angle value should be greater than or equal to -360 and less than or equal to 360 degrees.': 'dx_reportDesigner_ChartStringId.MsgIncorrectStartAngle',
            ' showing {0}': 'dx_reportDesigner_ChartStringId.AlternateTextSeriesPlaceholder',
            'Copy all the data from the bound datasource to the chart, and then unbind the chart.': 'dx_reportDesigner_ChartStringId.VerbDataSnapshotDescription',
            'Doughnut': 'dx_reportDesigner_ChartStringId.CmdCreateDoughnutChartMenuCaption',
            'Scatter Line': 'dx_reportDesigner_ChartStringId.CmdCreateScatterLineChartMenuCaption',
            'Enable zooming (false)': 'dx_reportDesigner_ChartStringId.WizEnableZoomingFalse',
            'A value "{0}" isn\'t compatible with the current value scale type.': 'dx_reportDesigner_ChartStringId.MsgIncompatibleValue',
            'Value Duration': 'dx_reportDesigner_ChartStringId.ValueDurationPatternDescription',
            'Concourse': 'dx_reportDesigner_ChartStringId.PltConcourse',
            'Line 3D Stacked 100%': 'dx_reportDesigner_ChartStringId.SvnFullStackedLine3D',
            'The ScaleMode property cannot be used with AxisY.': 'dx_reportDesigner_ChartStringId.MsgAttemptToSetScaleModeForAxisY',
            'Behave similar to 3D Area Chart, but plot a fitted curve through each data point in a series.': 'dx_reportDesigner_ChartStringId.CmdCreateSplineArea3DChartDescription',
            'The range of a polar X-axis can\'t be changed.': 'dx_reportDesigner_ChartStringId.MsgPolarAxisXRangeChanged',
            'The {0} object isn\'t a data adapter.': 'dx_reportDesigner_ChartStringId.MsgIncorrectDataAdapter',
            'The specified view type is already present in the collection.': 'dx_reportDesigner_ChartStringId.MsgAddPresentViewType',
            'The specified {0} parameter type doesn\'t match the appropriate scale type, which is {1} for this axis.': 'dx_reportDesigner_ChartStringId.MsgDiagramToPointIncorrectValue',
            'Left-top': 'dx_reportDesigner_ChartStringId.WizDockCornerLeftTop',
            'An argument "{0}" isn\'t compatible with the current argument scale type.': 'dx_reportDesigner_ChartStringId.MsgIncompatibleArgument',
            'Bar 3D Stacked': 'dx_reportDesigner_ChartStringId.SvnStackedBar3D',
            'Display series as areas on a diagram, so that the value of each data point is aggregated with the underlying data points\' values.': 'dx_reportDesigner_ChartStringId.CmdCreateStackedArea3DChartDescription',
            'Clear the chart\'s datasource.': 'dx_reportDesigner_ChartStringId.VerbClearDataSourceDescription',
            'Step Area 3D': 'dx_reportDesigner_ChartStringId.SvnStepArea3D',
            'The border width should be greater than 0.': 'dx_reportDesigner_ChartStringId.MsgIncorrectBorderThickness',
            'Display all series stacked and is useful when it is necessary to compare how much each series adds to the total aggregate value for specific arguments (as percents).': 'dx_reportDesigner_ChartStringId.CmdCreateFullStackedLine3DChartDescription',
            'Area 3D Stacked 100%': 'dx_reportDesigner_ChartStringId.SvnFullStackedArea3D',
            'The pie depth should be greater than 0 and less than or equal to 100, since its value is measured in percents.': 'dx_reportDesigner_ChartStringId.MsgIncorrectPieDepth',
            'Polar Area': 'dx_reportDesigner_ChartStringId.SvnPolarArea',
            'AVERAGE': 'dx_reportDesigner_ChartStringId.FunctionNameAverage',
            'Compare values across categories by using vertical rectangles.\r\n\r\nUse it when the order of categories is not important or for displaying item counts such as a histogram.\r\n    ': 'dx_reportDesigner_ChartStringId.CmdCreateBarChartDescription',
            'The line tension percentage should be greater than or equal to 0 and less than or equal to 100.': 'dx_reportDesigner_ChartStringId.MsgIncorrectLineTensionPercent',
            'Step Area': 'dx_reportDesigner_ChartStringId.SvnStepArea',
            'Step Line': 'dx_reportDesigner_ChartStringId.SvnStepLine',
            'The specified series point doesn\'t belong to the current Pie series views\' collection of series points, and so it can\'t be added to the collection of exploded points.': 'dx_reportDesigner_ChartStringId.MsgInvalidExplodedSeriesPoint',
            'Combine the advantages of both the 100% Stacked Cylinder and Clustered Cylinder chart types, so that you can stack different cylinders, and combine them into groups across the same axis value.': 'dx_reportDesigner_ChartStringId.CmdCreateCylinderSideBySideFullStackedBar3DChartDescription',
            'Display the contribution of each value to a total while comparing series with one doughnut nested in another one.': 'dx_reportDesigner_ChartStringId.CmdCreateNestedDoughnutChartDescription',
            'Can\'t set the series view\'s pane, because the specified pane isn\'t default and isn\'t contained in the diagram\'s collection of panes.': 'dx_reportDesigner_ChartStringId.MsgIncorrectSeriesViewPane',
            'Compare the percentage values of different point arguments in the same series, and illustrate these values as easy to understand pie slices, but with a hole in its center.': 'dx_reportDesigner_ChartStringId.CmdCreateDoughnut3DChartDescription',
            '100% Stacked Spline Area': 'dx_reportDesigner_ChartStringId.CmdCreateFullStackedSplineAreaChartMenuCaption',
            'Customize the diagram\'s panes.\r\nNote that you may select a pane by clicking it in the chart preview.': 'dx_reportDesigner_ChartStringId.WizPanesPageDescription',
            'The palette with the {0} name already exists in the repository.': 'dx_reportDesigner_ChartStringId.MsgAddExistingPaletteError',
            'Data Snapshot': 'dx_reportDesigner_ChartStringId.VerbDataSnapshot',
            'Display series as filled area on a circular diagram on the basis of angles.': 'dx_reportDesigner_ChartStringId.CmdCreatePolarAreaChartDescription',
            'Step Area in 3-D': 'dx_reportDesigner_ChartStringId.CmdCreateStepArea3DChartMenuCaption',
            'The shadow size should be greater than 0.': 'dx_reportDesigner_ChartStringId.MsgIncorrectShadowSize',
            'Series point\'s ID must be unique.': 'dx_reportDesigner_ChartStringId.MsgSeriesPointIDNotUnique',
            '2-D Pie': 'dx_reportDesigner_ChartStringId.CmdPie2DGroupPlaceHolderMenuCaption',
            'The series distance should be greater than or equal to 0.': 'dx_reportDesigner_ChartStringId.MsgIncorrectSeriesDistance',
            'The legend marker size should be greater than 0 and less than 1000.': 'dx_reportDesigner_ChartStringId.MsgIncorrectLegendMarkerSize',
            'Contain numerous style presets of a chart control to specify its appearance depending on the selected palette.': 'dx_reportDesigner_ChartStringId.CmdChangeAppearancePlaceHolderDescription',
            'The annotation Width should be greater than 0.': 'dx_reportDesigner_ChartStringId.MsgIncorrectAnnotationWidth',
            'Display all points from different series in a stacked manner and is useful when it is necessary to compare how much each series adds to the total aggregate value for specific arguments.': 'dx_reportDesigner_ChartStringId.CmdCreateStackedLine3DChartDescription',
            'Text Annotation ': 'dx_reportDesigner_ChartStringId.TextAnnotationPrefix',
            'The default pane can\'t be deleted.': 'dx_reportDesigner_ChartStringId.IODeleteDefaultPane',
            'Green Yellow': 'dx_reportDesigner_ChartStringId.PltGreenYellow',
            'Radar Area': 'dx_reportDesigner_ChartStringId.CmdCreateRadarAreaChartMenuCaption',
            'DiagonalCross': 'dx_reportDesigner_ChartStringId.WizHatchDiagonalCross',
            'DiagonalBrick': 'dx_reportDesigner_ChartStringId.WizHatchDiagonalBrick',
            'The types of the MinValue and MaxValue don\'t match.': 'dx_reportDesigner_ChartStringId.MsgMinMaxDifferentTypes',
            'You can add only Indicator objects to this collection.': 'dx_reportDesigner_ChartStringId.MsgIncorrectIndicator',
            'TIFF': 'dx_reportDesigner_ChartStringId.CmdExportToTIFFMenuCaption',
            'Step Line 3D': 'dx_reportDesigner_ChartStringId.SvnStepLine3D',
            'The MeasureUnit property can\'t be modified for the axis Y.': 'dx_reportDesigner_ChartStringId.MsgMeasureUnitCanNotBeSetForAxisY',
            'Save a chart to an XML file.': 'dx_reportDesigner_ChartStringId.VerbSaveLayoutDescription',
            'Range Bar': 'dx_reportDesigner_ChartStringId.SvnOverlappedRangeBar',
            'Microsoft Excel 2000-2003 Work Book': 'dx_reportDesigner_ChartStringId.CmdExportToXLSDescription',
            'Annotation': 'dx_reportDesigner_ChartStringId.DefaultAnnotation',
            'Chart layout loaded': 'dx_reportDesigner_ChartStringId.TrnLoadLayout',
            'Incorrect transformation matrix.': 'dx_reportDesigner_ChartStringId.MsgIncorrectTransformationMatrix',
            'SmallCheckerBoard': 'dx_reportDesigner_ChartStringId.WizHatchSmallCheckerBoard',
            'Secondary AxisY ': 'dx_reportDesigner_ChartStringId.SecondaryAxisYPrefix',
            'The number of minor count should be greater than 0 and less than 100.': 'dx_reportDesigner_ChartStringId.MsgIncorrectMinorCount',
            'The ProcessMissingPoints property operates with the X-axis scale only.': 'dx_reportDesigner_ChartStringId.MsgProcessMissingPointsForValueAxis',
            'Slipstream': 'dx_reportDesigner_ChartStringId.PltSlipstream',
            'Parent and child points must belong to the same series.': 'dx_reportDesigner_ChartStringId.MsgIncorrectSeriesOfParentAndChildPoints',
            'FromCenter': 'dx_reportDesigner_ChartStringId.WizGradientFromCenter',
            'Triangular Moving Average': 'dx_reportDesigner_ChartStringId.IndTriangularMovingAverage',
            '3-D Cone': 'dx_reportDesigner_ChartStringId.CmdCreateConeManhattanBarChartMenuCaption',
            'Choose a palette to color series or their data points. Also choose the style, which specifies the chart\'s appearance depending on the current palette.': 'dx_reportDesigner_ChartStringId.WizAppearancePageDescription',
            'The Alignment can\'t be set to Alignment.Zero for the secondary axis.': 'dx_reportDesigner_ChartStringId.MsgInvalidZeroAxisAlignment',
            'SolidDiamond': 'dx_reportDesigner_ChartStringId.WizHatchSolidDiamond',
            'Stretch (a chart is stretched or shrunk to fit the page\r\non which it is printed)': 'dx_reportDesigner_ChartStringId.PrintSizeModeStretch',
            'Show activity columns from different series grouped by their arguments. Each column represents a range of data with two values for each argument value.': 'dx_reportDesigner_ChartStringId.CmdCreateSideBySideRangeBarChartDescription',
            'Display each row or column of data as a 3-D ribbon on three axes.': 'dx_reportDesigner_ChartStringId.CmdCreateLine3DChartDescription',
            'The \'{0}\' summary function is incompatible with the {1} scale.': 'dx_reportDesigner_ChartStringId.MsgIncompatibleSummaryFunction',
            'This property is intended for internal use only. You\'re not allowed to change its value.': 'dx_reportDesigner_ChartStringId.MsgInternalPropertyChangeError',
            'the view type': 'dx_reportDesigner_ChartStringId.MsgIncompatibleByViewType',
            'This control doesn\'t contain the specified chart.': 'dx_reportDesigner_ChartStringId.MsgNotBelongingChart',
            'The fixed series indent should be greater than or equal to 0.': 'dx_reportDesigner_ChartStringId.MsgIncorrectSeriesIndentFixed',
            '100% Stacked Column in 3-D': 'dx_reportDesigner_ChartStringId.CmdCreateFullStackedBar3DChartMenuCaption',
            'Change the palette for the current chart.': 'dx_reportDesigner_ChartStringId.CmdChangePalettePlaceHolderDescription',
            'Display trend overtime (dates, years) or ordered categories. Useful when there are many data points and the order is important.': 'dx_reportDesigner_ChartStringId.CmdCreateLineChartDescription',
            'All Holidays': 'dx_reportDesigner_ChartStringId.AllHolidays',
            'Label ': 'dx_reportDesigner_ChartStringId.CustomAxisLabelPrefix',
            'LeftToRight': 'dx_reportDesigner_ChartStringId.WizGradientLeftToRight',
            'The horizontal scroll percent should be greater than or equal to -{0} and less than or equal to {0}.': 'dx_reportDesigner_ChartStringId.MsgIncorrectHorizontalScrollPercent',
            'The Chart Wizard is invoked to help you adjust the main chart settings in one place.': 'dx_reportDesigner_ChartStringId.CmdRunWizardDescription',
            'Chart Wizard': 'dx_reportDesigner_ChartStringId.WizErrorMessageTitle',
            'The angle of the annotation should be greater than or equal to -360 and less than or equal to 360.': 'dx_reportDesigner_ChartStringId.MsgIncorrectTextAnnotationAngle',
            'The stock level line length value should be greater than or equal to 0.': 'dx_reportDesigner_ChartStringId.MsgIncorrectStockLevelLineLengthValue',
            '3-D Pie': 'dx_reportDesigner_ChartStringId.CmdPie3DGroupPlaceHolderMenuCaption',
            'Insert a line chart.\r\n\r\nLine charts are used to display trends overtime.\r\n    ': 'dx_reportDesigner_ChartStringId.CmdCreateLineChartPlaceHolderDescription',
            'Clustered Range Column': 'dx_reportDesigner_ChartStringId.CmdCreateSideBySideRangeBarChartMenuCaption',
            'Doughnut in 3-D': 'dx_reportDesigner_ChartStringId.CmdCreateDoughnut3DChartMenuCaption',
            'Invalid datasource type (no supported interfaces are implemented).': 'dx_reportDesigner_ChartStringId.MsgInvalidDataSource',
            'WideDownwardDiagonal': 'dx_reportDesigner_ChartStringId.WizHatchWideDownwardDiagonal',
            'Spline Area 3D Stacked': 'dx_reportDesigner_ChartStringId.SvnSplineAreaStacked3D',
            'Templates': 'dx_reportDesigner_ChartStringId.RibbonTemplatesGroupCaption',
            '{0} argument scale type cannot be specified, because the existing exploded point filters don\'t correspond to it.': 'dx_reportDesigner_ChartStringId.MsgIncorrectPieArgumentScaleType',
            'HideOverlapping': 'dx_reportDesigner_ChartStringId.WizResolveOverlappingModeHideOverlapping',
            'Secondary axes X changed': 'dx_reportDesigner_ChartStringId.TrnSecondaryAxesXChanged',
            'Close Value': 'dx_reportDesigner_ChartStringId.CloseValuePatternDescription',
            'The line thickness should be greater than 0.': 'dx_reportDesigner_ChartStringId.MsgIncorrectLineThickness',
            'Combine the advantages of both the Stacked Bar and Clustered Bar chart types, so that you can stack different bars, and combine them into groups across the same axis value.': 'dx_reportDesigner_ChartStringId.CmdCreateRotatedSideBySideStackedBarChartDescription',
            'LeftColumn': 'dx_reportDesigner_ChartStringId.WizPositionLeftColumn',
            'Crosshair Position can\'t be null.': 'dx_reportDesigner_ChartStringId.MsgIncorrectCrosshairPosition',
            'The dimension of the {0} summary function isn\'t compatible with the {1} series view ({2} but should be {3}).': 'dx_reportDesigner_ChartStringId.MsgIncompatibleSummaryFunctionDimension',
            'Relative': 'dx_reportDesigner_ChartStringId.WizShapePositionKindRelative',
            'Radar': 'dx_reportDesigner_ChartStringId.CmdRadarGroupPlaceHolderMenuCaption',
            'OutlinedDiamond': 'dx_reportDesigner_ChartStringId.WizHatchOutlinedDiamond',
            'Simple Moving Average': 'dx_reportDesigner_ChartStringId.IndSimpleMovingAverage',
            'Invalid placeholder': 'dx_reportDesigner_ChartStringId.InvalidPlaceholder',
            'Trend Line': 'dx_reportDesigner_ChartStringId.IndTrendLine',
            'Chart Anchor Point': 'dx_reportDesigner_ChartStringId.AnnotationChartAnchorPoint',
            'The legend text offset should be greater than or equal to 0 and less than 1000.': 'dx_reportDesigner_ChartStringId.MsgIncorrectLegendTextOffset',
            'The envelope percent should be greater than 0 and less than or equal to 100.': 'dx_reportDesigner_ChartStringId.MsgIncorrectEnvelopePercent',
            'Display the trend of the percentage each value contributes over time or ordered categories.': 'dx_reportDesigner_ChartStringId.CmdCreateFullStackedLineChartDescription',
            'The array of values is empty.': 'dx_reportDesigner_ChartStringId.MsgEmptyArrayOfValues',
            'Clustered 100% Stacked Column': 'dx_reportDesigner_ChartStringId.CmdCreateSideBySideFullStackedBarChartMenuCaption',
            'Customize the diagram\'s properties.': 'dx_reportDesigner_ChartStringId.WizDiagramPageDescription',
            '100% Stacked Bar': 'dx_reportDesigner_ChartStringId.CmdCreateRotatedFullStackedBarChartMenuCaption',
            'Since the current Pie series view displays the series created using a series template, the specified series point can\'t be added to the collection of exploded points. You need to use another Explode Mode instead.': 'dx_reportDesigner_ChartStringId.MsgInvalidExplodedModeAdd',
            'Technic': 'dx_reportDesigner_ChartStringId.PltTechnic',
            'The "PointOptions" property can\'t be set at runtime.': 'dx_reportDesigner_ChartStringId.MsgPointOptionsSettingRuntimeError',
            'Single File Web Page': 'dx_reportDesigner_ChartStringId.CmdExportToMHTDescription',
            'Constant Line ': 'dx_reportDesigner_ChartStringId.ConstantLinePrefix',
            'This chart diagram can not be displayed in a Range Control.': 'dx_reportDesigner_ChartStringId.InvalidRangeControlText',
            'Violet': 'dx_reportDesigner_ChartStringId.PltViolet',
            'The page of the {0} type is already registered. You can\'t add more than one page of a particular type.': 'dx_reportDesigner_ChartStringId.MsgWizardNonUniquePageType',
            'To enter data points manually, use the Points tab. Or, use other tabs, to specify data source settings for individual or auto-created series.': 'dx_reportDesigner_ChartStringId.WizDataPageDescription',
            'Stacked Bar': 'dx_reportDesigner_ChartStringId.CmdCreateRotatedStackedBarChartMenuCaption',
            'ToCenterHorizontal': 'dx_reportDesigner_ChartStringId.WizGradientToCenterHorizontal',
            'Stacked Pyramid': 'dx_reportDesigner_ChartStringId.CmdCreatePyramidStackedBar3DChartMenuCaption',
            'date-time': 'dx_reportDesigner_ChartStringId.ScaleTypeDateTime',
            'Can\'t set the series view\'s X-axis, because the specified axis isn\'t primary and isn\'t contained in the diagram\'s collection of secondary X-axes.': 'dx_reportDesigner_ChartStringId.MsgIncorrectSeriesViewAxisX',
            'Can\'t set the series view\'s Y-axis, because the specified axis isn\'t primary and isn\'t contained in the diagram\'s collection of secondary Y-axes.': 'dx_reportDesigner_ChartStringId.MsgIncorrectSeriesViewAxisY',
            'Summary function changed': 'dx_reportDesigner_ChartStringId.TrnSummaryFunctionChanged',
            'Clustered Stacked Column in 3-D': 'dx_reportDesigner_ChartStringId.CmdCreateSideBySideStackedBar3DChartMenuCaption',
            'BMP': 'dx_reportDesigner_ChartStringId.CmdExportToBMPMenuCaption',
            'The grid spacing of a polar X-axis can\'t be changed.': 'dx_reportDesigner_ChartStringId.MsgPolarAxisXGridSpacingChanged',
            '100% Stacked Area': 'dx_reportDesigner_ChartStringId.CmdCreateFullStackedAreaChartMenuCaption',
            'Use Ctrl with the left mouse button\r\nto rotate the annotation.': 'dx_reportDesigner_ChartStringId.MsgAnnotationRotationToolTip',
            'Run Wizard...': 'dx_reportDesigner_ChartStringId.VerbWizard',
            'BackwardDiagonal': 'dx_reportDesigner_ChartStringId.WizHatchBackwardDiagonal',
            'The perspective angle should be greater than or equal to 0 and less than 180.': 'dx_reportDesigner_ChartStringId.MsgIncorrectPerspectiveAngle',
            'The collection doesn\'t contain the specified item.': 'dx_reportDesigner_ChartStringId.MsgItemNotInCollection',
            'RightColumn': 'dx_reportDesigner_ChartStringId.WizPositionRightColumn',
            'Series Point Anchor Point': 'dx_reportDesigner_ChartStringId.AnnotationSeriesPointAnchorPoint',
            'Mouse Position': 'dx_reportDesigner_ChartStringId.ToolTipMousePosition',
            'Display the contribution of each value to a total.\r\n\r\nUse it when the values can be added together or when you have only one data series and all values are positive.\r\n    ': 'dx_reportDesigner_ChartStringId.CmdCreatePieChartPlaceHolderDescription',
            'LargeConfetti': 'dx_reportDesigner_ChartStringId.WizHatchLargeConfetti',
            'It\'s necessary to specify {0} value data members for the current series view.': 'dx_reportDesigner_ChartStringId.MsgIncorrectValueDataMemberCount',
            'The same element is repeated several times in the order array.': 'dx_reportDesigner_ChartStringId.MsgOrderRepeatedElementFound',
            'Shape Position can\'t be null.': 'dx_reportDesigner_ChartStringId.MsgIncorrectShapePosition',
            'Clustered Cone': 'dx_reportDesigner_ChartStringId.CmdCreateConeBar3DChartMenuCaption',
            'Bar Stacked': 'dx_reportDesigner_ChartStringId.SvnStackedBar',
            'TopLeftToBottomRight': 'dx_reportDesigner_ChartStringId.WizGradientTopLeftToBottomRight',
            'The BarSeriesLabelPosition.Top value isn\'t supported for this series view type.': 'dx_reportDesigner_ChartStringId.MsgIncorrectBarSeriesLabelPosition',
            'This property can\'t be customized at runtime.': 'dx_reportDesigner_ChartStringId.MsgDesignTimeOnlySetting',
            'Spline 3D': 'dx_reportDesigner_ChartStringId.SvnSpline3D',
            'Display horizontal bars along the time axis. Each bar represents a separate event with the start and end values, hence these charts are used to track different activities during the time frame.\r\n\r\nUse it when it\'s necessary to show activity bars from different series one above another, to compare their duration.\r\n    ': 'dx_reportDesigner_ChartStringId.CmdCreateSideBySideGanttChartDescription',
            'Series...': 'dx_reportDesigner_ChartStringId.VerbSeries',
            'The series view\'s X-axis can\'t be set to null.': 'dx_reportDesigner_ChartStringId.MsgNullSeriesViewAxisX',
            'The series view\'s Y-axis can\'t be set to null.': 'dx_reportDesigner_ChartStringId.MsgNullSeriesViewAxisY',
            'The summary function \'{0}\' accepts {1} parameters instead of {2}.': 'dx_reportDesigner_ChartStringId.MsgIncorrectSummaryFunctionParametersCount',
            'Workday': 'dx_reportDesigner_ChartStringId.Workday',
            'Clustered 100% Stacked Bar': 'dx_reportDesigner_ChartStringId.CmdCreateRotatedSideBySideFullStackedBarChartMenuCaption',
            'Clustered Stacked Column': 'dx_reportDesigner_ChartStringId.CmdCreateSideBySideStackedBarChartMenuCaption',
            'None (a chart is printed with the size identical to that\r\nshown on the form)': 'dx_reportDesigner_ChartStringId.PrintSizeModeNone',
            'Zoom (a chart is resized proportionally (without clipping),\r\nso that it best fits the page on which it is printed)': 'dx_reportDesigner_ChartStringId.PrintSizeModeZoom',
            'The precision of the percent value should be greater than 0.': 'dx_reportDesigner_ChartStringId.MsgIncorrectPercentPrecision',
            'The number of star points should be greater than 3 and less than 101.': 'dx_reportDesigner_ChartStringId.MsgIncorrectMarkerStarPointCount',
            'The value can\'t be equal to Double.NaN, Double.PositiveInfinity, or Double.NegativeInfinity.': 'dx_reportDesigner_ChartStringId.MsgIncorrectDoubleValue',
            'Radar Line': 'dx_reportDesigner_ChartStringId.SvnRadarLine',
            'Stacked Spline Area in 3-D': 'dx_reportDesigner_ChartStringId.CmdCreateStackedSplineArea3DChartMenuCaption',
            'The line width should be greater than 0.': 'dx_reportDesigner_ChartStringId.MsgIncorrectLineWidth',
            'The min value of the axis range should be less than its max value.': 'dx_reportDesigner_ChartStringId.MsgIncorrectAxisRange',
            'DataFilter': 'dx_reportDesigner_ChartStringId.DefaultDataFilterName',
            'The ProcessMissingPoints property can\'t be specified in the continuous scale mode.': 'dx_reportDesigner_ChartStringId.MsgProcessMissingPointsForContinuousScale',
            'Clustered Stacked Cylinder': 'dx_reportDesigner_ChartStringId.CmdCreateCylinderSideBySideStackedBar3DChartMenuCaption',
            'Display the trend of the contribution of each value over time or categories.\r\n\r\nUse it to emphasize the trend in the total across series for one category.\r\n    ': 'dx_reportDesigner_ChartStringId.CmdCreateStackedAreaChartDescription',
            'Stacked Cone': 'dx_reportDesigner_ChartStringId.CmdCreateConeStackedBar3DChartMenuCaption',
            'Scale breaks changed': 'dx_reportDesigner_ChartStringId.TrnScaleBreaksChanged',
            'Graphics Interchange Format': 'dx_reportDesigner_ChartStringId.CmdExportToGIFDescription',
            'BottomToTop': 'dx_reportDesigner_ChartStringId.WizSeriesLabelTextOrientationBottomToTop',
            'MinValue can\'t be set to null.': 'dx_reportDesigner_ChartStringId.MsgIncorrectAxisRangeMinValue',
            'Can\'t set the AxisYCoordinate\'s axis, because the specified axis isn\'t primary and isn\'t contained in the diagram\'s collection of secondary Y-axes.': 'dx_reportDesigner_ChartStringId.MsgIncorrectAxisYCoordinateAxis',
            'Axis visibility changed': 'dx_reportDesigner_ChartStringId.TrnAxisVisibilityChanged',
            'Wizard': 'dx_reportDesigner_ChartStringId.RibbonWizardGroupCaption',
            'The angle should be greater than or equal to -360 and less than or equal to 360.': 'dx_reportDesigner_ChartStringId.MsgIncorrectRelativePositionAngle',
            '': 'dx_reportDesigner_ChartStringId.CmdEmptyMenuCaption',
            'Series Binding': 'dx_reportDesigner_ChartStringId.WizSeriesDataBindingPageName',
            'Clustered Gantt': 'dx_reportDesigner_ChartStringId.CmdCreateSideBySideGanttChartMenuCaption',
            'Compare values across categories and across series on three axes.\r\n\r\nUse it when the categories and series are equally important.\r\n    ': 'dx_reportDesigner_ChartStringId.CmdCreateManhattanBarChartDescription',
            'Doughnut 3D': 'dx_reportDesigner_ChartStringId.SvnDoughnut3D',
            'Preview and make changes to pages before printing.': 'dx_reportDesigner_ChartStringId.CmdPrintPreviewDescription',
            'The unregistered element is found.': 'dx_reportDesigner_ChartStringId.MsgOrderUnregisteredElementFound',
            'The arrow width should be always odd and greater than 0.': 'dx_reportDesigner_ChartStringId.MsgIncorrectArrowWidth',
            'Track different activities during the time frame.': 'dx_reportDesigner_ChartStringId.CmdCreateGanttChartDescription',
            'Construction': 'dx_reportDesigner_ChartStringId.WizConstructionGroupName',
            'Yellow': 'dx_reportDesigner_ChartStringId.PltYellow',
            'SmallGrid': 'dx_reportDesigner_ChartStringId.WizHatchSmallGrid',
            'Revert the legend point options to their default values.': 'dx_reportDesigner_ChartStringId.VerbResetLegendPointOptionsDescription',
            'Scale Break ': 'dx_reportDesigner_ChartStringId.ScaleBreakPrefix',
            'Cannot set the BarDistanceFixed property unless the series is added to the chart\'s collection.': 'dx_reportDesigner_ChartStringId.MsgIncorrectBarDistanceFixedPropertyUsing',
            '2-D Column': 'dx_reportDesigner_ChartStringId.CmdColumn2DGroupPlaceHolderMenuCaption',
            'The MeasureUnit property can\'t be modified in both the automatic and continuous date-time scale modes.': 'dx_reportDesigner_ChartStringId.MsgIncorrectDateTimeMeasureUnitPropertyUsing',
            'Exploded': 'dx_reportDesigner_ChartStringId.ExplodedPointsDialogExplodedColumn',
            'The precision should be greater than or equal to 0.': 'dx_reportDesigner_ChartStringId.MsgIncorrectNumericPrecision',
            'Pane ': 'dx_reportDesigner_ChartStringId.XYDiagramPanePrefix',
            'In A Fog': 'dx_reportDesigner_ChartStringId.AppInAFog',
            'You can\'t manually change this series point collection, because a chart is bound to data.': 'dx_reportDesigner_ChartStringId.MsgDenyChangeSeriesPointCollection',
            'Blue Green': 'dx_reportDesigner_ChartStringId.PltBlueGreen',
            'Display data as areas on a diagram, so that the value of each data point is stacked with all the other corresponding data points\' values.\r\n\r\nUse it for comparing the percentage values of several series for the same point arguments.\r\n    ': 'dx_reportDesigner_ChartStringId.CmdCreateFullStackedArea3DChartDescription',
            'Pivot Grid Datasource': 'dx_reportDesigner_ChartStringId.WizPivotGridDataSourcePageName',
            'Text Annotation': 'dx_reportDesigner_ChartStringId.TextAnnotation',
            'There are no visible panes to show in a chart.\r\nTry to set the chart\'s Diagram.DefaultPane.Visible property to True,\r\nor show other panes from the Diagram.Panes collection.': 'dx_reportDesigner_ChartStringId.MsgNoPanes',
            'Pie in 3-D': 'dx_reportDesigner_ChartStringId.CmdCreatePie3DChartMenuCaption',
            'An argument can\'t be empty.': 'dx_reportDesigner_ChartStringId.MsgEmptyArgument',
            'Combine the advantages of both the 100% Stacked Bar and Clustered Bar chart types, so you can stack different bars, and combine them into groups across the same axis value.': 'dx_reportDesigner_ChartStringId.CmdCreateRotatedSideBySideFullStackedBarChartDescription',
            'Can\'t create an image for the specified size.': 'dx_reportDesigner_ChartStringId.MsgIncorrectImageBounds',
            'Side By Side Range Bar': 'dx_reportDesigner_ChartStringId.SvnSideBySideRangeBar',
            'LightHorizontal': 'dx_reportDesigner_ChartStringId.WizHatchLightHorizontal',
            'Holiday': 'dx_reportDesigner_ChartStringId.Holiday',
            'Orange Red': 'dx_reportDesigner_ChartStringId.PltOrangeRed',
            'Line Stacked': 'dx_reportDesigner_ChartStringId.SvnStackedLine',
            'Area Stacked': 'dx_reportDesigner_ChartStringId.SvnStackedArea',
            'Child series point\'s ID must be positive or equal to zero.': 'dx_reportDesigner_ChartStringId.MsgIncorrectChildSeriesPointID',
            'The ChartControl isn\'t found, or there are several charts on this control. To solve the problem, you should handle the WizardPage.InitializePage event and manually specify the chart.': 'dx_reportDesigner_ChartStringId.MsgInitializeChartNotFound',
            'Equity': 'dx_reportDesigner_ChartStringId.PltEquity',
            'HorizontalBrick': 'dx_reportDesigner_ChartStringId.WizHatchHorizontalBrick',
            'Light': 'dx_reportDesigner_ChartStringId.AppLight',
            'Left-bottom': 'dx_reportDesigner_ChartStringId.WizDockCornerLeftBottom',
            'Open the Series Collection Editor.': 'dx_reportDesigner_ChartStringId.VerbSeriesDescription',
            'Clustered Stacked Bar': 'dx_reportDesigner_ChartStringId.CmdCreateRotatedSideBySideStackedBarChartMenuCaption',
            '2-D Line': 'dx_reportDesigner_ChartStringId.CmdLine2DGroupPlaceHolderMenuCaption',
            'The length of the order array isn\'t equal to the total number of registered elements.': 'dx_reportDesigner_ChartStringId.MsgOrderArrayLengthMismatch',
            'Owner of the child series point can\'t be null and must be of the Series type.': 'dx_reportDesigner_ChartStringId.MsgIncorrectChildSeriesPointOwner',
            'Increase the chart\'s size,\r\nto view its layout.\r\n    ': 'dx_reportDesigner_ChartStringId.DefaultSmallChartText',
            'Office 2013': 'dx_reportDesigner_ChartStringId.PltOffice2013',
            'Origin': 'dx_reportDesigner_ChartStringId.PltOrigin',
            'Orange': 'dx_reportDesigner_ChartStringId.PltOrange',
            'Use Ctrl with the left mouse button\r\nto explode or collapse slices.': 'dx_reportDesigner_ChartStringId.Msg2DPieExplodingToolTip',
            'Office': 'dx_reportDesigner_ChartStringId.PltOffice',
            'ToCenter': 'dx_reportDesigner_ChartStringId.WizGradientToCenter',
            '3-D Funnel': 'dx_reportDesigner_ChartStringId.CmdCreateFunnel3DChartMenuCaption',
            'Open the Palettes Editor.': 'dx_reportDesigner_ChartStringId.VerbEditPalettesDescription',
            'The angle of the label should be greater than or equal to -360 and less than or equal to 360.': 'dx_reportDesigner_ChartStringId.MsgIncorrectLabelAngle',
            'LargeGrid': 'dx_reportDesigner_ChartStringId.WizHatchLargeGrid',
            'Impossible to export a chart to the specified image format.': 'dx_reportDesigner_ChartStringId.MsgIncorrectImageFormat',
            'Export to MHT': 'dx_reportDesigner_ChartStringId.CmdExportToMHTMenuCaption',
            'Plot a fitted curve through each data point in a series.': 'dx_reportDesigner_ChartStringId.CmdCreateSplineChartDescription',
            'Display the contribution of each value to a total like a pie chart, but it can contain multiple series.': 'dx_reportDesigner_ChartStringId.CmdCreateDoughnutChartDescription',
            'Failed to import holydays from the \'{0}\' file.': 'dx_reportDesigner_ChartStringId.MsgCantImportHolidays',
            'Aspect': 'dx_reportDesigner_ChartStringId.PltAspect',
            'The specified file isn\'t a correct image file. Please choose another one.': 'dx_reportDesigner_ChartStringId.WizInvalidBackgroundImage',
            'The minimum size should be greater than or equal to 0, and less than the maximum size.': 'dx_reportDesigner_ChartStringId.MsgIncorrectBubbleMinSize',
            'Export to XLSX': 'dx_reportDesigner_ChartStringId.CmdExportToXLSXMenuCaption',
            'The zoom percent should be greater than 0 and less than or equal to {0}.': 'dx_reportDesigner_ChartStringId.MsgIncorrectZoomPercent',
            'Stacked Area': 'dx_reportDesigner_ChartStringId.CmdCreateStackedAreaChartMenuCaption',
            'Free': 'dx_reportDesigner_ChartStringId.WizShapePositionKindFree',
            'The grid spacing should be greater than 0.': 'dx_reportDesigner_ChartStringId.MsgIncorrectGridSpacing',
            'Clustered Stacked Pyramid': 'dx_reportDesigner_ChartStringId.CmdCreatePyramidSideBySideStackedBar3DChartMenuCaption',
            'Stacked Spline Area': 'dx_reportDesigner_ChartStringId.CmdCreateStackedSplineAreaChartMenuCaption',
            'The edge1 value can\'t be null.': 'dx_reportDesigner_ChartStringId.MsgInvalidEdge1',
            'The edge2 value can\'t be null.': 'dx_reportDesigner_ChartStringId.MsgInvalidEdge2',
            'The top N values count should be greater than 0.': 'dx_reportDesigner_ChartStringId.MsgIncorrectTopNCount',
            'Spline Area in 3-D': 'dx_reportDesigner_ChartStringId.CmdCreateSplineArea3DChartMenuCaption',
            'Export': 'dx_reportDesigner_ChartStringId.CmdExportPlaceHolderMenuCaption',
            'Blue II': 'dx_reportDesigner_ChartStringId.PltBlueII',
            '3-D Pyramid': 'dx_reportDesigner_ChartStringId.CmdCreatePyramidManhattanBarChartMenuCaption',
            'The group with the {0} name is already registered.': 'dx_reportDesigner_ChartStringId.MsgWizardNonUniqueGroupName',
            'Display the trend of the contribution of each value over time or ordered categories.': 'dx_reportDesigner_ChartStringId.CmdCreateStackedLineChartDescription',
            'Wizard Page': 'dx_reportDesigner_ChartStringId.DefaultWizardPageLabel',
            'The reduction color value can\'t be empty.': 'dx_reportDesigner_ChartStringId.MsgIncorrectReductionColorValue',
            'Display the contribution of each value to a total.': 'dx_reportDesigner_ChartStringId.CmdCreatePie3DChartDescription',
            'The specified path cannot be resolved: {0}.': 'dx_reportDesigner_ChartStringId.MsgIncorrectPath',
            'Font can\'t be null': 'dx_reportDesigner_ChartStringId.MsgIncorrectFont',
            '2-D Bar': 'dx_reportDesigner_ChartStringId.CmdBar2DGroupPlaceHolderMenuCaption',
            'Use Ctrl with the left mouse button\r\nto move the anchor point.': 'dx_reportDesigner_ChartStringId.MsgAnchorPointMovingToolTip',
            'The "LegendPointOptions" property can\'t be set at runtime.': 'dx_reportDesigner_ChartStringId.MsgLegendPointOptionsSettingRuntimeError',
            'DottedGrid': 'dx_reportDesigner_ChartStringId.WizHatchDottedGrid',
            'Create series, and adjust their general properties.\r\nNote that the view type of the first visible series determines the diagram type and its set of specific options.': 'dx_reportDesigner_ChartStringId.WizSeriesPageDescription',
            'Incorrect value "{0}" for the property "{1}".': 'dx_reportDesigner_ChartStringId.MsgIncorrectPropertyValue',
            'Can\'t add a palette which has an empty name (\\"\\") to the palette repository. Please, specify a name for the palette.': 'dx_reportDesigner_ChartStringId.MsgInvalidPaletteName',
            'Series changed': 'dx_reportDesigner_ChartStringId.TrnSeriesChanged',
            'The type of the "{0}" argument data member isn\'t compatible with the {1} scale.': 'dx_reportDesigner_ChartStringId.MsgIncompatibleArgumentDataMember',
            'DashedHorizontal': 'dx_reportDesigner_ChartStringId.WizHatchDashedHorizontal',
            'Palette ': 'dx_reportDesigner_ChartStringId.PalettePrefix',
            'The AxisValue property cannot be set to null for the StripLimit object.': 'dx_reportDesigner_ChartStringId.MsgIncorrectStripLimitAxisValue',
            'Save As\r\nTemplate': 'dx_reportDesigner_ChartStringId.CmdSaveAsTemplateMenuCaption',
            'Chart Title': 'dx_reportDesigner_ChartStringId.DefaultChartTitle',
            'The "Label" property can\'t be set at runtime.': 'dx_reportDesigner_ChartStringId.MsgLabelSettingRuntimeError',
            'An incorrect value is specified. A dock target can only be a pane, or null (meaning the chart control itself).': 'dx_reportDesigner_ChartStringId.MsgIncorrectFreePositionDockTarget',
            'Axis of arguments': 'dx_reportDesigner_ChartStringId.AxisXDefaultTitle',
            'DarkDownwardDiagonal': 'dx_reportDesigner_ChartStringId.WizHatchDarkDownwardDiagonal',
            'The type of the "{0}" point isn\'t compatible with the {1} scale.': 'dx_reportDesigner_ChartStringId.MsgIncompatiblePointType',
            'DarkVertical': 'dx_reportDesigner_ChartStringId.WizHatchDarkVertical',
            'This series view doesn\'t support relations.': 'dx_reportDesigner_ChartStringId.MsgSeriesViewNotSupportRelations',
            'Create and customize annotations anchored to a chart, pane or series point.\r\nNote that you may select an annotation by clicking it in the chart preview.': 'dx_reportDesigner_ChartStringId.WizAnnotationsPageDescription',
            'Links': 'dx_reportDesigner_ChartStringId.ColumnLinks',
            'The tickmark thickness should be greater than 0.': 'dx_reportDesigner_ChartStringId.MsgIncorrectTickmarkThickness',
            'Display the trend of values over time or categories.': 'dx_reportDesigner_ChartStringId.CmdCreateAreaChartPlaceHolderDescription',
            'Show points from two or more different series on the same points arguments on a circular grid that has multiple axes along which data can be plotted.': 'dx_reportDesigner_ChartStringId.CmdCreateRadarPointChartDescription',
            'The nested doughnut weight should be greater than 0.': 'dx_reportDesigner_ChartStringId.MsgIncorrectNestedDoughnutWeight',
            'Enable scrolling (true)': 'dx_reportDesigner_ChartStringId.WizEnableScrollingTrue',
            'DarkUpwardDiagonal': 'dx_reportDesigner_ChartStringId.WizHatchDarkUpwardDiagonal',
            'COUNT': 'dx_reportDesigner_ChartStringId.FunctionNameCount',
            'Point Hint': 'dx_reportDesigner_ChartStringId.PointHintPatternDescription',
            'GIF': 'dx_reportDesigner_ChartStringId.CmdExportToGIFMenuCaption',
            'Exponential Moving Average': 'dx_reportDesigner_ChartStringId.IndExponentialMovingAverage',
            'The maximum size should be greater than the minimum size.': 'dx_reportDesigner_ChartStringId.MsgIncorrectBubbleMaxSize',
            'Side By Side Bar Stacked 100%': 'dx_reportDesigner_ChartStringId.SvnSideBySideFullStackedBar',
            '(none)': 'dx_reportDesigner_ChartStringId.WizDataFiltersDisabled',
            'Enable scrolling (false)': 'dx_reportDesigner_ChartStringId.WizEnableScrollingFalse',
            'The funnel hole percentage should be greater than or equal to 0 and less than or equal to 100.': 'dx_reportDesigner_ChartStringId.MsgIncorrectFunnelHolePercent',
            'Percent Value': 'dx_reportDesigner_ChartStringId.PercentValuePatternDescription',
            'Customize the chart\'s properties.': 'dx_reportDesigner_ChartStringId.WizChartPageDescription',
            'Range Column': 'dx_reportDesigner_ChartStringId.CmdCreateRangeBarChartMenuCaption',
            'Behave similar to Stacked Area in 3D chart, but plot a fitted curve through each data point in a series.': 'dx_reportDesigner_ChartStringId.CmdCreateStackedSplineArea3DChartDescription',
            'Line 3D Stacked': 'dx_reportDesigner_ChartStringId.SvnStackedLine3D',
            'The specified XML file can\'t be opened,\r\nbecause it is either not a supported file type,\r\nor because the file has been damaged.': 'dx_reportDesigner_ChartStringId.MsgChartLoadingException',
            'ToCenterVertical': 'dx_reportDesigner_ChartStringId.WizGradientToCenterVertical',
            'Presentation': 'dx_reportDesigner_ChartStringId.WizPresentationGroupName',
            'The GridAlignment property can\'t be modified in the automatic numeric scale mode.': 'dx_reportDesigner_ChartStringId.MsgIncorrectNumericGridAlignmentPropertyUsing',
            'DataFilters changed': 'dx_reportDesigner_ChartStringId.TrnDataFiltersChanged',
            'Violet II': 'dx_reportDesigner_ChartStringId.PltVioletII',
            'the value scale type': 'dx_reportDesigner_ChartStringId.MsgIncompatibleByValueScaleType',
            'The zero value is not acceptable for the workdays.  Use work days of a week.': 'dx_reportDesigner_ChartStringId.MsgUnsupportedWorkdaysForWorkdaysOptions',
            'qualitative': 'dx_reportDesigner_ChartStringId.ScaleTypeQualitative',
            'Clustered 100% Stacked Cylinder': 'dx_reportDesigner_ChartStringId.CmdCreateCylinderSideBySideFullStackedBar3DChartMenuCaption',
            'The {0} must be inherited from the {1} class.': 'dx_reportDesigner_ChartStringId.MsgWizardIncorrectBasePageType',
            'Display vertical columns along the Y-axis (the axis of values). Each column represents a range of data for each argument value.': 'dx_reportDesigner_ChartStringId.CmdCreateRangeBarChartDescription',
            'Display series as filled area on a circular grid that has multiple axes along which data can be plotted.': 'dx_reportDesigner_ChartStringId.CmdCreateRadarAreaChartDescription',
            'Show trends for several series and compare their values for the same points arguments on a circular grid that has multiple axes along which data can be plotted.': 'dx_reportDesigner_ChartStringId.CmdCreateRadarLineChartDescription',
            'Urban': 'dx_reportDesigner_ChartStringId.PltUrban',
            'Verve': 'dx_reportDesigner_ChartStringId.PltVerve',
            'The fixed plane depth should be greater than or equal to 1.': 'dx_reportDesigner_ChartStringId.MsgIncorrectPlaneDepthFixed',
            'Paper': 'dx_reportDesigner_ChartStringId.PltPaper',
            'Solstice': 'dx_reportDesigner_ChartStringId.PltSolstice',
            'Green': 'dx_reportDesigner_ChartStringId.PltGreen',
            'Civic': 'dx_reportDesigner_ChartStringId.PltCivic',
            'Oriel': 'dx_reportDesigner_ChartStringId.PltOriel',
            'The Mixed': 'dx_reportDesigner_ChartStringId.PltMixed',
            'Metro': 'dx_reportDesigner_ChartStringId.PltMetro',
            'The maximum line count should be greater than or equal to 0 and less than or equal to 20.': 'dx_reportDesigner_ChartStringId.MsgIncorrectMaxLineCount',
            'ForwardDiagonal': 'dx_reportDesigner_ChartStringId.WizHatchForwardDiagonal',
            'This property can\'t be used if the Direction property is set to {0}.': 'dx_reportDesigner_ChartStringId.MsgEquallySpacedItemsNotUsable',
            'Represent series points in the same order that they have in the collection.': 'dx_reportDesigner_ChartStringId.CmdCreateScatterLineChartDescription',
            'The SeriesPointRelations collection already contains this relation.': 'dx_reportDesigner_ChartStringId.MsgSeriesPointRelationAlreadyExists',
            'Area 3D Stacked': 'dx_reportDesigner_ChartStringId.SvnStackedArea3D',
            'This series is incompatible:\r\n  ': 'dx_reportDesigner_ChartStringId.IncompatibleSeriesHeader',
            'Axis can\'t be set to null for the AxisXCoordinate object.': 'dx_reportDesigner_ChartStringId.MsgNullAxisXCoordinateAxis',
            'Insert a column chart.\r\n\r\nColumn charts are used to compare values across categories.\r\n    ': 'dx_reportDesigner_ChartStringId.CmdCreateBarChartPlaceHolderDescription',
            'Child series point with ID equal to {0} doesn\'t exist.': 'dx_reportDesigner_ChartStringId.MsgChildSeriesPointNotExist',
            'The page can\'t be registered in the unregistered group': 'dx_reportDesigner_ChartStringId.MsgRegisterPageInUnregisterGroup',
            'Customize the point labels of a series.\r\nNote that you may select labels of a series by clicking them in the chart preview.': 'dx_reportDesigner_ChartStringId.WizSeriesLabelsPageDescription',
            'Choose a chart type you want to use. To filter chart types by their groups, use the values in the drop-down box.': 'dx_reportDesigner_ChartStringId.WizChartTypePageDescription',
            'The {0} value scale type is incompatible with the {1} series view.': 'dx_reportDesigner_ChartStringId.MsgIncompatibleValueScaleType',
            'There is no series in the chart\'s collection with at least one series point.': 'dx_reportDesigner_ChartStringId.IncorrectSeriesCollectionToolTipText',
            'Resemble a Scatter chart, but compare sets of three values instead of two. The third value determines the size of the bubble marker.': 'dx_reportDesigner_ChartStringId.CmdCreateBubbleChartDescription',
            'Load...': 'dx_reportDesigner_ChartStringId.VerbLoadLayout',
            'Secondary axes Y changed': 'dx_reportDesigner_ChartStringId.TrnSecondaryAxesYChanged',
            'The {0} ValueLevel is invalid for the current regression line.': 'dx_reportDesigner_ChartStringId.MsgIncorrectValueLevel',
            'This PolygonGradientMode isn\'t compatible with AreaSeriesView.': 'dx_reportDesigner_ChartStringId.MsgInvalidGradientMode',
            'Axes': 'dx_reportDesigner_ChartStringId.WizAxesPageName',
            'Combine the advantages of both the Stacked Column and Clustered Column chart types, so that you can stack different columns, and combine them into groups across the same axis value.': 'dx_reportDesigner_ChartStringId.CmdCreateSideBySideStackedBarChartDescription',
            'RightToLeft': 'dx_reportDesigner_ChartStringId.WizGradientRightToLeft',
            'Annotation ': 'dx_reportDesigner_ChartStringId.AnnotationPrefix',
            'Customize the legend\'s properties.': 'dx_reportDesigner_ChartStringId.WizLegendPageDescription',
            'The point distance value should be greater than 0.': 'dx_reportDesigner_ChartStringId.MsgIncorrectPointDistance',
            'Customize the X and Y axes of the diagram.\r\nNote that you may select an axis by clicking it in the chart preview.': 'dx_reportDesigner_ChartStringId.WizAxesPageDescription',
            'The {0} condition can\'t be applied to the "{1}" data.': 'dx_reportDesigner_ChartStringId.MsgInvalidFilterCondition',
            'BottomLeftToTopRight': 'dx_reportDesigner_ChartStringId.WizGradientBottomLeftToTopRight',
            'Series title changed': 'dx_reportDesigner_ChartStringId.TrnSeriesTitleChanged',
            'Adobe Portable Document Format': 'dx_reportDesigner_ChartStringId.CmdExportToPDFDescription',
            'High Value': 'dx_reportDesigner_ChartStringId.HighValuePatternDescription',
            'Series Views': 'dx_reportDesigner_ChartStringId.WizSeriesViewPageName',
            'WinLoss': 'dx_reportDesigner_SparklineStringId.viewWinLoss',
            'It\'s impossible to create an instance of a class: {0} because specified parameters are incorrect.': 'dx_reportDesigner_GaugesCoreStringId.MsgInvalidClassCreationParameters',
            'Path can\'t be created.': 'dx_reportDesigner_GaugesCoreStringId.MsgPathCreationError',
            'It\'s impossible to create an instance of a class {0} because specified text is incorrect: ': 'dx_reportDesigner_GaugesCoreStringId.MsgTextParsingError',
            'The gauge control can\'t be restored correctly, because the specified layout file contians the following invalid elements: {0}.': 'dx_reportDesigner_GaugesCoreStringId.MsgGaugeRestoreError',
            'We have all the information needed to process the report.': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Wizard_ReportComplete_Description',
            'What summary function would you like to calculate?': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Wizard_SummaryOptions_Description',
            'To be able to run the Document Viewer, the client web browser must support HTML5.': 'dx_reportDesigner_ASPxReportsStringId.WebDocumentViewer_PlatformNotSupported_Error',
            'Enter the text to find in the document.': 'dx_reportDesigner_ASPxReportsStringId.SearchDialog_EnterText',
            'The specified Report Service has not been found.': 'dx_reportDesigner_ASPxReportsStringId.DocumentViewer_RemoteSourceConnection_Error',
            'Report Wizard': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Wizard_Header',
            'Next': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Wizard_Next',
            'Ignore null values': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Wizard_SummaryOptions_IgnoreNullValues',
            'To log in to the Report Server, handle the RequestCredentials event.': 'dx_reportDesigner_ASPxReportsStringId.DocumentViewer_RemoteAuthenticatorCredentialHandled_Error',
            'Collapsed': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Accordion_Collapsed',
            'Align Left 1': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Wizard_ReportLayout_AlignLeft1',
            'Align Left 2': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Wizard_ReportLayout_AlignLeft2',
            'It is only possible to assign either the SettingsRemoteSource or ConfigurationRemoteSource property of ASPxDocumentViewer at a time.': 'dx_reportDesigner_ASPxReportsStringId.DocumentViewer_RemoteSourceSettingsAndConfiguration_Error',
            'Last Page': 'dx_reportDesigner_ASPxReportsStringId.ToolBarItemText_LastPage',
            'Insert Group Header Band': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_ReportActions_InsertGroupHeaderBand',
            'The value cannot be empty.': 'dx_reportDesigner_ASPxReportsStringId.ParametersPanel_DateTimeValueValidationError',
            'Finish': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Wizard_Finish',
            'To be able to run the Report Designer, the client web browser must support HTML5.': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_PlatformNotSupported_Error',
            'Size to Control Height': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_ElementsAction_SizeToControlHeight',
            'Display the specified page.': 'dx_reportDesigner_ASPxReportsStringId.DocumentViewer_RibbonCurrentPageToolTip',
            'of': 'dx_reportDesigner_ASPxReportsStringId.ToolBarItemText_OfLabel',
            'Delete Row': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_TableActions_DeleteRow',
            'Remove calculated field': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_FieldListActions_RemoveCalculatedField',
            'Insert Page Header Band': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_ReportActions_InsertPageHeaderBand',
            'Choose a Report Layout': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Wizard_ReportLayout_Title',
            'Choose a Table or View': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Wizard_ChooseDataMember_Title',
            'Guid should contain 32 digits delimited with 4 dashes (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx).': 'dx_reportDesigner_ASPxReportsStringId.ParametersPanel_GuidValidationError',
            'The report style specifies the appearance of your report.': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Wizard_ReportStyle_Description',
            'Insert Field in the Column Area': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_PivotActions_InsertFieldInTheColumnArea',
            'Formal': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Wizard_ReportStyle_Formal',
            'Specify the print settings and print the document.': 'dx_reportDesigner_ASPxReportsStringId.DocumentViewer_RibbonCommandToolTip_PrintReport',
            'Outline 2': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Wizard_ReportLayout_Outline2',
            'Outline 1': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Wizard_ReportLayout_Outline1',
            'Failed to log in with the specified user credentials.': 'dx_reportDesigner_ASPxReportsStringId.DocumentViewer_RemoteAuthenticatorLogin_Error',
            'Insert Cell': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_TableActions_InsertCell',
            'Create Groups': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Wizard_CreateGroups_Title',
            'Casual': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Wizard_ReportStyle_Casual',
            'Mht': 'dx_reportDesigner_ASPxReportsStringId.ExportName_mht',
            'Csv': 'dx_reportDesigner_ASPxReportsStringId.ExportName_csv',
            'Xls': 'dx_reportDesigner_ASPxReportsStringId.ExportName_xls',
            'Rtf': 'dx_reportDesigner_ASPxReportsStringId.ExportName_rtf',
            'Pdf': 'dx_reportDesigner_ASPxReportsStringId.ExportName_pdf',
            'Save To File': 'dx_reportDesigner_ASPxReportsStringId.DocumentViewer_RibbonCommandText_SaveToFile',
            'Insert Group Footer Band': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_ReportActions_InsertGroupFooterBand',
            'Match whole word only': 'dx_reportDesigner_ASPxReportsStringId.SearchDialog_WholeWord',
            'Insert Sub-Band': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_ReportActions_InsertSubBand',
            'Select the columns you want to display within your report.': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Wizard_ChooseColumns_Description',
            'Down': 'dx_reportDesigner_ASPxReportsStringId.SearchDialog_Down',
            'Match case': 'dx_reportDesigner_ASPxReportsStringId.SearchDialog_Case',
            'The DocumentViewerRemoteSourceSettings.CustomTokenStorage property is not assigned.': 'dx_reportDesigner_ASPxReportsStringId.DocumentViewer_RemoteSourceSettings_CustomTokenStorage_Error',
            'Print the report': 'dx_reportDesigner_ASPxReportsStringId.ToolBarItemText_PrintReport',
            'Parameters Panel': 'dx_reportDesigner_ASPxReportsStringId.DocumentViewer_RibbonCommandText_ParametersPanel',
            'Portrait': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Wizard_ReportLayout_Portrait',
            'There are no parameters available yet.': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_FieldList_Parameters',
            'Page Count:': 'dx_reportDesigner_ASPxReportsStringId.DocumentViewer_RibbonPageCountText',
            'Size to Control': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_ElementsAction_SizeToControl',
            'Current Page': 'dx_reportDesigner_ASPxReportsStringId.DocumentViewer_RibbonCurrentPageText',
            'Insert Field in the Data Area': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_PivotActions_InsertFieldInTheDataArea',
            'Create a New Style': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_StylesEditor_CreateNew',
            'Choose a Report Style': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Wizard_Report_Style',
            'Insert Field in the Filter Area': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_PivotActions_InsertFieldInTheFilterArea',
            'Insert Page Footer Band': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_ReportActions_InsertPageFooterBand',
            'Display the last document page.': 'dx_reportDesigner_ASPxReportsStringId.DocumentViewer_RibbonCommandToolTip_LastPage',
            'Finished searching the document.': 'dx_reportDesigner_ASPxReportsStringId.SearchDialog_Finished',
            'Find Next': 'dx_reportDesigner_ASPxReportsStringId.SearchDialog_FindNext',
            'Findnbsp;what': 'dx_reportDesigner_ASPxReportsStringId.SearchDialog_FindWhat',
            'Export a report and save it to the disk': 'dx_reportDesigner_ASPxReportsStringId.ToolBarItemText_SaveToDisk',
            'Export a report and show it in a new window': 'dx_reportDesigner_ASPxReportsStringId.ToolBarItemText_SaveToWindow',
            'Add parameter': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_FieldListActions_AddParameter',
            'Delete Column': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_TableActions_DeleteColumn',
            'Insert Top Margin Band': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_ReportActions_InsertTopMarginBand',
            'Find Text': 'dx_reportDesigner_ASPxReportsStringId.DocumentViewer_RibbonCommandText_FindText',
            'Next Page': 'dx_reportDesigner_ASPxReportsStringId.DocumentViewer_RibbonCommandText_NextPage',
            'First Page': 'dx_reportDesigner_ASPxReportsStringId.ToolBarItemText_FirstPage',
            'Display the search window': 'dx_reportDesigner_ASPxReportsStringId.ToolBarItemText_Search',
            'Preview Parameters': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Preview_ParametersTitle',
            'Display the first document page.': 'dx_reportDesigner_ASPxReportsStringId.DocumentViewer_RibbonCommandToolTip_FirstPage',
            'Stepped': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Wizard_ReportLayout_Stepped',
            'Delete Cell': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_TableActions_DeleteCell',
            'Choose a Data Source to use in your report.': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Wizard_ChooseDataSource_Description',
            'Specify the report\'s title': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Wizard_ReportComplete_SpecifyTitle',
            'It is only possible to assign either the Local Report or Remote Source of ASPxDocumentViewer at a time.': 'dx_reportDesigner_ASPxReportsStringId.DocumentViewer_LocalAndRemoteSource_Error',
            'Save the document to a file in a specified format.': 'dx_reportDesigner_ASPxReportsStringId.DocumentViewer_RibbonCommandToolTip_SaveToFile',
            'Previous Page': 'dx_reportDesigner_ASPxReportsStringId.DocumentViewer_RibbonCommandText_PreviousPage',
            'Display the next document page.': 'dx_reportDesigner_ASPxReportsStringId.DocumentViewer_RibbonCommandToolTip_NextPage',
            'Find text in the document.': 'dx_reportDesigner_ASPxReportsStringId.DocumentViewer_RibbonCommandToolTip_FindText',
            'Print Page': 'dx_reportDesigner_ASPxReportsStringId.DocumentViewer_RibbonCommandText_PrintPage',
            'Save To Window': 'dx_reportDesigner_ASPxReportsStringId.DocumentViewer_RibbonCommandText_SaveToWindow',
            'Adjust the field width so all fields fit onto a page': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Wizard_ReportLayout_AdjustFieldWidth',
            'The RequestCredentials event has not been subscribed to.': 'dx_reportDesigner_ASPxReportsStringId.DocumentViewer_RemoteRequestCredentials_Error',
            'To view the remote report, specify the ServerUri or EndpointConfigurationName property of the ASPxDocumentViewer.SettingsRemoteSource.': 'dx_reportDesigner_ASPxReportsStringId.DocumentViewer_RemoteSourceSettings_Error',
            'Insert Detail Report Band': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_ReportActions_InsertDetailReportBand',
            'To display a report, only one of the following actions can be performed at a time:\r\n- assigning the ASPxWebDocumentViewer.ReportSourceId property;\r\n- calling the ASPxWebDocumentViewer.OpenReport method;\r\n- calling the ASPxWebDocumentViewer.OpenReportXmlLayout method.': 'dx_reportDesigner_ASPxReportsStringId.WebDocumentViewer_OpenReport_Error',
            'Specify the print settings and print the current page.': 'dx_reportDesigner_ASPxReportsStringId.DocumentViewer_RibbonCommandToolTip_PrintPage',
            'Groups': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Groups',
            'Insert Column To the Left': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_TableActions_InsertColumnToLeft',
            'Columnar': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Wizard_ReportLayout_Columnar',
            'Cannot find a toolbar control with the specified name: \'{0}\'.': 'dx_reportDesigner_ASPxReportsStringId.DocumentViewer_ExternalRibbonNotFound_Error',
            'The report layout specifies the manner in which selected data fields are arranged on individual pages.': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Wizard_ReportLayout_Description',
            'Run Wizard': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_MenuButtons_RunWizard',
            'Compact': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Wizard_ReportStyle_Compact',
            'StringResources.DocumentViewer_RibbonReportGroupText': 'dx_reportDesigner_ASPxReportsStringId.DocumentViewer_RibbonReportGroupText',
            'The table or view you choose determines wich columns will be available in your report.': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Wizard_ChooseDataMember_Description',
            'Insert Bottom Margin Band': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_ReportActions_InsertBottomMarginBand',
            'To view a remote report, enable the PageByPage property of the SettingsReportViewer.': 'dx_reportDesigner_ASPxReportsStringId.DocumentViewer_RemotePageByPage_Error',
            'Access and modify the report parameter values.': 'dx_reportDesigner_ASPxReportsStringId.DocumentViewer_RibbonCommandToolTip_ParametersPanel',
            'Insert Detail Band': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_ReportActions_InsertDetailBand',
            'To create a new item, click Add.': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_GroupFields_Empty',
            'Insert Report Footer Band': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_ReportActions_InsertReportFooterBand',
            'Choose Columns to Display in Your Report': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Wizard_ChooseColumns_Title',
            'Insert Field in the Row Area': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_PivotActions_InsertFieldInTheRowArea',
            'The user credentials cannot be empty.': 'dx_reportDesigner_ASPxReportsStringId.DocumentViewer_RemoteAuthenticatorCredential_Error',
            'This command cannot be executed because a document has not yet been generated.': 'dx_reportDesigner_ASPxReportsStringId.DocumentViewer_NoRemoteDocumentInformation_Error',
            'Print the current page': 'dx_reportDesigner_ASPxReportsStringId.ToolBarItemText_PrintPage',
            'Display the previous document page.': 'dx_reportDesigner_ASPxReportsStringId.DocumentViewer_RibbonCommandToolTip_PreviousPage',
            'Add Data Items Here': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Pivot_AddDataItems',
            'Remove parameter': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_FieldListActions_RemoveParameter',
            'Navigate through the report\'s hierarchy of bookmarks.': 'dx_reportDesigner_ASPxReportsStringId.DocumentViewer_RibbonCommandToolTip_DocumentMap',
            'Tables': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Tables',
            'The report does not have any parameters yet. To create a new parameter, click Add Parameter.': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Parameters_CreateParameters',
            'Insert Column To the Right': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_TableActions_InsertColumnToRight',
            'Corporate': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Wizard_ReportStyle_Corporate',
            'Size to Control Width': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_ElementsAction_SizeToControlWidth',
            'Html': 'dx_reportDesigner_ASPxReportsStringId.ExportName_html',
            'Xlsx': 'dx_reportDesigner_ASPxReportsStringId.ExportName_xlsx',
            'StringResources.DocumentViewer_RibbonNavigationGroupText': 'dx_reportDesigner_ASPxReportsStringId.DocumentViewer_RibbonNavigationGroupText',
            'StringResources.DocumentViewer_RibbonExportGroupText': 'dx_reportDesigner_ASPxReportsStringId.DocumentViewer_RibbonExportGroupText',
            'Submit': 'dx_reportDesigner_ASPxReportsStringId.ParametersPanel_Submit',
            'Previous': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Wizard_Previous',
            'Selected fields': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Wizard_SelectedFields',
            'Save the document in a specified format and display the result in a new window.': 'dx_reportDesigner_ASPxReportsStringId.DocumentViewer_RibbonCommandToolTip_SaveToWindow',
            'Insert Row Below': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_TableActions_InsertRowBelow',
            'Insert Row Above': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_TableActions_InsertRowAbove',
            'Available fields': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Wizard_AvailableFields',
            'Add Filter Fields Here': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Pivot_AddFilterFields',
            'Add calculated field': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_FieldListActions_AddCalculatedField',
            'Data Source Wizard': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Wizard_DataSourceHeader',
            'Add Row Fields Here': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Pivot_AddRowFields',
            'Tabular': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Wizard_ReportLayout_Tabular',
            'Create multiple groups, each with a single field value, or define several fields in the same group.': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Wizard_CreateGroups_Description',
            'Up': 'dx_reportDesigner_ASPxReportsStringId.SearchDialog_Up',
            'Justified': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Wizard_ReportLayout_Justified',
            'Insert Report Header Band': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_ReportActions_InsertReportHeaderBand',
            'The Report is Complete': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Wizard_ReportComplete_Title',
            'Choose summary options': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Wizard_SummaryOptions_Title',
            'Print Report': 'dx_reportDesigner_ASPxReportsStringId.DocumentViewer_RibbonCommandText_PrintReport',
            'Add Column Fields Here': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Pivot_AddColumnFields',
            'Choose a Data Source': 'dx_reportDesigner_ASPxReportsStringId.ReportDesigner_Wizard_ChooseDataSource_Title',
        };
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        function getLocalization(text) {
            return window["Globalize"] && (Globalize.localize(Designer.localization_values[text]) || Globalize.localize(text)) || text;
        }
        Designer.getLocalization = getLocalization;
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        Designer.DEBUG = true;
        function checkModelReady(model) {
            return model.isModelReady ? model.isModelReady() : true;
        }
        Designer.checkModelReady = checkModelReady;
        function dataMemberWrapperCreator(dataMember, member) {
            return ko.computed({
                read: function () {
                    if (dataMember() && member()) {
                        return dataMember() + "." + member();
                    }
                    else if (dataMember()) {
                        return dataMember();
                    }
                    return member() ? member() : "";
                },
                write: function (val) {
                    if (dataMember() && val.indexOf(dataMember()) === 0) {
                        member(val.replace(dataMember() + ".", ""));
                    }
                    else {
                        member(val);
                    }
                }
            });
        }
        Designer.dataMemberWrapperCreator = dataMemberWrapperCreator;
        var FieldListProvider = (function () {
            function FieldListProvider(fieldListCallback, rootItems) {
                var _this = this;
                this.getItems = function (pathRequest) {
                    var result = $.Deferred();
                    if (rootItems && !pathRequest.fullPath) {
                        result.resolve($.map(rootItems(), function (item) {
                            var dataMemberInfo = { name: item.id || item.ref, displayName: item.name, isList: true, specifics: item.specifics || "ListSource", dragData: { noDragable: true } };
                            if (item.data && item.data.tableInfoCollection) {
                                dataMemberInfo.templateName = "dxrd-datasource-item";
                                dataMemberInfo["tableInfoItems"] = item.data.tableInfoCollection;
                            }
                            else if (item.data && item.data.objectType && item.data.objectType().indexOf("DevExpress.DataAccess.Sql.SqlDataSource") === 0) {
                                dataMemberInfo["canAddSqlQuery"] = true;
                            }
                            return dataMemberInfo;
                        }));
                    }
                    else {
                        _this._patchRequest(pathRequest, rootItems);
                        var canEditQuery = false;
                        if (rootItems && (pathRequest.fullPath === pathRequest.id || pathRequest.fullPath === pathRequest.ref)) {
                            var dsInfo = rootItems.peek().filter(function (dsItem) {
                                return dsItem.id === pathRequest.id || dsItem.ref === pathRequest.ref;
                            })[0];
                            canEditQuery = dsInfo && dsInfo.data && dsInfo.data.objectType && dsInfo.data.objectType().indexOf("DevExpress.DataAccess.Sql.SqlDataSource") === 0;
                        }
                        fieldListCallback(pathRequest).done(function (fields) {
                            result.resolve(fields.map(function (item) {
                                item["canEditQuery"] = canEditQuery;
                                return item;
                            }));
                        });
                    }
                    return result.promise();
                };
            }
            FieldListProvider.prototype._patchRequest = function (request, dataSources) {
                if (!dataSources) {
                    return;
                }
                for (var i = 0; i < dataSources().length; i++) {
                    if (dataSources()[i].id === request.id) {
                        request.ref = undefined;
                        return;
                    }
                    if (dataSources()[i].ref === request.ref) {
                        request.id = undefined;
                        return;
                    }
                }
            };
            return FieldListProvider;
        })();
        Designer.FieldListProvider = FieldListProvider;
        function NotifyAboutWarning(msg) {
            if (Designer.DEBUG) {
                throw new Error(msg);
            }
            else {
                console.warn(msg);
            }
        }
        Designer.NotifyAboutWarning = NotifyAboutWarning;
        function validateName(nameCandidate) {
            return nameCandidate && /^[A-Za-z][A-Za-z0-9_]+$/.test(nameCandidate);
        }
        Designer.validateName = validateName;
        Designer.nameValidationRules = [{ type: "custom", validationCallback: function (options) {
            return validateName(options.value);
        }, message: DevExpress.Designer.getLocalization('Name is required and should be a valid identifier.') }];
        function floatFromModel(val) {
            return ko.observable(val === undefined || val === null ? null : parseFloat(val));
        }
        Designer.floatFromModel = floatFromModel;
        Designer.papperKindMapper = {
            A2: { width: 1654, height: 2339 },
            A3: { width: 1169, height: 1654 },
            A3Extra: { width: 1268, height: 1752 },
            A3ExtraTransverse: { width: 1268, height: 1752 },
            A3Rotated: { width: 1654, height: 1169 },
            A3Transverse: { width: 1169, height: 1654 },
            A4: { width: 827, height: 1169 },
            A4Extra: { width: 929, height: 1268 },
            A4Plus: { width: 827, height: 1299 },
            A4Rotated: { width: 1169, height: 827 },
            A4Small: { width: 827, height: 1169 },
            A4Transverse: { width: 827, height: 1169 },
            A5: { width: 583, height: 827 },
            A5Extra: { width: 685, height: 925 },
            A5Rotated: { width: 827, height: 583 },
            A5Transverse: { width: 583, height: 827 },
            A6: { width: 413, height: 583 },
            A6Rotated: { width: 583, height: 413 },
            APlus: { width: 894, height: 1402 },
            B4: { width: 984, height: 1390 },
            B4Envelope: { width: 984, height: 1390 },
            B4JisRotated: { width: 1433, height: 1012 },
            B5: { width: 693, height: 984 },
            B5Envelope: { width: 693, height: 984 },
            B5Extra: { width: 791, height: 1087 },
            B5JisRotated: { width: 1012, height: 717 },
            B5Transverse: { width: 717, height: 1012 },
            B6Envelope: { width: 693, height: 492 },
            B6Jis: { width: 504, height: 717 },
            B6JisRotated: { width: 717, height: 504 },
            BPlus: { width: 1201, height: 1917 },
            C3Envelope: { width: 1276, height: 1803 },
            C4Envelope: { width: 902, height: 1276 },
            C5Envelope: { width: 638, height: 902 },
            C65Envelope: { width: 449, height: 902 },
            C6Envelope: { width: 449, height: 638 },
            CSheet: { width: 1700, height: 2200 },
            DLEnvelope: { width: 433, height: 866 },
            DSheet: { width: 2200, height: 3400 },
            ESheet: { width: 3400, height: 4400 },
            Executive: { width: 725, height: 1050 },
            Folio: { width: 850, height: 1300 },
            GermanLegalFanfold: { width: 850, height: 1300 },
            GermanStandardFanfold: { width: 850, height: 1200 },
            InviteEnvelope: { width: 866, height: 866 },
            IsoB4: { width: 984, height: 1390 },
            ItalyEnvelope: { width: 433, height: 906 },
            JapaneseDoublePostcard: { width: 787, height: 583 },
            JapaneseDoublePostcardRotated: { width: 583, height: 787 },
            JapanesePostcard: { width: 394, height: 583 },
            Ledger: { width: 1700, height: 1100 },
            Legal: { width: 850, height: 1400 },
            LegalExtra: { width: 927, height: 1500 },
            Letter: { width: 850, height: 1100 },
            LetterExtra: { width: 927, height: 1200 },
            LetterExtraTransverse: { width: 927, height: 1200 },
            LetterPlus: { width: 850, height: 1269 },
            LetterRotated: { width: 1100, height: 850 },
            LetterSmall: { width: 850, height: 1100 },
            LetterTransverse: { width: 827, height: 1100 },
            MonarchEnvelope: { width: 388, height: 750 },
            Note: { width: 850, height: 1100 },
            Number10Envelope: { width: 412, height: 950 },
            Number11Envelope: { width: 450, height: 1038 },
            Number12Envelope: { width: 475, height: 1100 },
            Number14Envelope: { width: 500, height: 1150 },
            Number9Envelope: { width: 388, height: 888 },
            PersonalEnvelope: { width: 362, height: 650 },
            Prc16K: { width: 575, height: 846 },
            Prc16KRotated: { width: 575, height: 846 },
            Prc32K: { width: 382, height: 594 },
            Prc32KBig: { width: 382, height: 594 },
            Prc32KBigRotated: { width: 382, height: 594 },
            Prc32KRotated: { width: 382, height: 594 },
            PrcEnvelopeNumber1: { width: 402, height: 650 },
            PrcEnvelopeNumber10: { width: 1276, height: 1803 },
            PrcEnvelopeNumber10Rotated: { width: 1803, height: 1276 },
            PrcEnvelopeNumber1Rotated: { width: 650, height: 402 },
            PrcEnvelopeNumber2: { width: 402, height: 693 },
            PrcEnvelopeNumber2Rotated: { width: 693, height: 402 },
            PrcEnvelopeNumber3: { width: 492, height: 693 },
            PrcEnvelopeNumber3Rotated: { width: 693, height: 492 },
            PrcEnvelopeNumber4: { width: 433, height: 819 },
            PrcEnvelopeNumber4Rotated: { width: 819, height: 433 },
            PrcEnvelopeNumber5: { width: 433, height: 866 },
            PrcEnvelopeNumber5Rotated: { width: 866, height: 433 },
            PrcEnvelopeNumber6: { width: 472, height: 906 },
            PrcEnvelopeNumber6Rotated: { width: 906, height: 472 },
            PrcEnvelopeNumber7: { width: 630, height: 906 },
            PrcEnvelopeNumber7Rotated: { width: 906, height: 630 },
            PrcEnvelopeNumber8: { width: 472, height: 1217 },
            PrcEnvelopeNumber8Rotated: { width: 1217, height: 472 },
            PrcEnvelopeNumber9: { width: 902, height: 1276 },
            PrcEnvelopeNumber9Rotated: { width: 1276, height: 902 },
            Quarto: { width: 846, height: 1083 },
            Standard10x11: { width: 1000, height: 1100 },
            Standard10x14: { width: 1000, height: 1400 },
            Standard11x17: { width: 1100, height: 1700 },
            Standard12x11: { width: 1200, height: 1100 },
            Standard15x11: { width: 1500, height: 1100 },
            Standard9x11: { width: 900, height: 1100 },
            Statement: { width: 550, height: 850 },
            Tabloid: { width: 1100, height: 1700 },
            TabloidExtra: { width: 1169, height: 1800 },
            USStandardFanfold: { width: 1488, height: 1100 },
        };
        function fromEnum(value) {
            var shotEnumValueKey = getShortTypeName(value);
            return ko.observable((this.values && this.values[shotEnumValueKey] !== undefined) ? shotEnumValueKey : value);
        }
        Designer.fromEnum = fromEnum;
        function getTypeNameFromFullName(controlType) {
            return controlType.split(',')[0].trim();
        }
        Designer.getTypeNameFromFullName = getTypeNameFromFullName;
        function getShortTypeName(controlType) {
            var fullTypeName = getTypeNameFromFullName(controlType), typeNameParts = fullTypeName.split('.');
            return typeNameParts[typeNameParts.length - 1];
        }
        Designer.getShortTypeName = getShortTypeName;
        function classExists(selector) {
            var lowerCaseSelector = selector.toLowerCase(), result = false;
            $.each(document.styleSheets || [], function (_, styleSheet) {
                var rules = styleSheet["rules"] ? styleSheet["rules"] : styleSheet["cssRules"];
                $.each(rules || [], function (_, rule) {
                    if (rule.selectorText && rule.selectorText.toLowerCase() === lowerCaseSelector) {
                        result = true;
                    }
                    return !result;
                });
                return !result;
            });
            return result;
        }
        Designer.classExists = classExists;
        function parseBool(val) {
            return ko.observable(val !== void 0 ? String(val).toLowerCase() === "true" : val);
        }
        Designer.parseBool = parseBool;
        function colorFromString(val) {
            var color = (val || "").split(",");
            var result = ko.observable(val);
            if (color.length > 1) {
                var alpha = (parseFloat(color[0]) / 255).toFixed(2);
                color.shift();
                color.push(alpha.toString());
                result = ko.observable("rgba(" + color.join(", ") + ")");
            }
            return result;
        }
        Designer.colorFromString = colorFromString;
        function saveAsInt(val) {
            return Math.round(val).toString();
        }
        Designer.saveAsInt = saveAsInt;
        function colorToString(val) {
            var color = (val || "").split(", ");
            var result = val;
            if (color.length > 1) {
                var alpha = Math.round(parseFloat(color[3]) * 255);
                color.pop();
                color[0] = color[0].split("(")[1];
                result = alpha.toString() + "," + color.join(",");
            }
            return result;
        }
        Designer.colorToString = colorToString;
        var Point = (function () {
            function Point(x, y) {
                this.x = ko.observable(x);
                this.y = ko.observable(y);
            }
            Point.prototype.getInfo = function () {
                return Designer.locationFake;
            };
            Point.fromString = function (value) {
                if (value === void 0) { value = "0, 0"; }
                var components = value.split(',');
                return new Point(parseFloat(components[0]), parseFloat(components[1]));
            };
            Point.prototype.toString = function () {
                return this.x() + ", " + this.y();
            };
            Point.unitProperties = ["x", "y"];
            return Point;
        })();
        Designer.Point = Point;
        var Size = (function () {
            function Size(width, height) {
                this.isPropertyDisabled = function (name) { return void 0; };
                this.width = ko.observable(width);
                this.height = ko.observable(height);
            }
            Size.prototype.getInfo = function () {
                return Designer.sizeFake;
            };
            Size.fromString = function (value) {
                if (value === void 0) { value = "0, 0"; }
                var components = value.split(',');
                return new Size(parseFloat(components[0]), parseFloat(components[1]));
            };
            Size.prototype.toString = function () {
                return this.width() + ", " + this.height();
            };
            Size.unitProperties = ["width", "height"];
            return Size;
        })();
        Designer.Size = Size;
        var Margins = (function () {
            function Margins(left, right, top, bottom) {
                this.bottom = ko.observable(bottom);
                this.left = ko.observable(left);
                this.right = ko.observable(right);
                this.top = ko.observable(top);
            }
            Margins.prototype.isEmpty = function () {
                return this.toString() === Margins.defaultVal;
            };
            Margins.fromString = function (value) {
                if (value === void 0) { value = Margins.defaultVal; }
                var components = value.split(',');
                return new Margins(parseInt(components[0]), parseInt(components[1]), parseInt(components[2]), parseInt(components[3]));
            };
            Margins.prototype.toString = function () {
                var result = Math.round(this.left()) + ", " + Math.round(this.right()) + ", " + Math.round(this.top()) + ", " + Math.round(this.bottom());
                return result;
            };
            Margins.defaultVal = "100, 100, 100, 100";
            Margins.unitProperties = ["left", "right"];
            return Margins;
        })();
        Designer.Margins = Margins;
        var Rectangle = (function () {
            function Rectangle(left, top, width, height) {
                if (left === void 0) { left = 0; }
                if (top === void 0) { top = 0; }
                if (width === void 0) { width = 0; }
                if (height === void 0) { height = 0; }
                this.left = ko.observable(0);
                this.top = ko.observable(0);
                this.width = ko.observable(0);
                this.height = ko.observable(0);
                this.left(left);
                this.top(top);
                this.width(width);
                this.height(height);
            }
            return Rectangle;
        })();
        Designer.Rectangle = Rectangle;
        function createUnitProperties(model, target, properties, measureUnit, zoom) {
            if (!properties)
                return;
            $.each(properties, function (propertyName, getModelValue) {
                var lastVal = 0;
                target[propertyName] = ko.computed({
                    read: function () {
                        var val = getModelValue(model)(), newVal = Designer.unitsToPixel(val, measureUnit.peek(), zoom());
                        if (Math.abs(newVal - lastVal) > 0.2) {
                            lastVal = newVal;
                            return lastVal;
                        }
                        return lastVal;
                    },
                    write: function (val) {
                        lastVal = val;
                        var result = Designer.pixelToUnits(val, measureUnit.peek(), zoom());
                        getModelValue(model)(result);
                    }
                });
            });
        }
        Designer.createUnitProperties = createUnitProperties;
        function propertiesVisitor(target, visitor, visited) {
            if (visited === void 0) { visited = []; }
            if (target && target !== undefined) {
                var properties = [];
                for (var propertyName in target) {
                    if (propertyName.indexOf("_") !== 0 && propertyName.indexOf("surface") === -1) {
                        var realPropertyName = propertyName;
                        if (ko.isComputed(target[propertyName]) && ko.isWriteableObservable(target["_" + propertyName])) {
                            realPropertyName = "_" + realPropertyName;
                        }
                        if (visited.indexOf(target[realPropertyName]) === -1 && !ko.isComputed(target[realPropertyName])) {
                            properties.push(target[realPropertyName]);
                        }
                    }
                }
                visitor(properties);
                visited.push.apply(visited, properties);
                $.each(properties, function (_, property) {
                    property = ko.unwrap(property);
                    if (typeof property === 'object') {
                        propertiesVisitor(property, visitor, visited);
                    }
                });
            }
        }
        Designer.propertiesVisitor = propertiesVisitor;
        function objectsVisitor(target, visitor, visited, skip) {
            if (visited === void 0) { visited = []; }
            if (skip === void 0) { skip = ["surface", "reportSource"]; }
            if (visited.indexOf(target) !== -1) {
                return;
            }
            if (target && target !== undefined) {
                var properties = [];
                for (var propertyName in target) {
                    if (visited.indexOf(target[propertyName]) === -1 && propertyName.indexOf("_") !== 0 && skip.indexOf(propertyName) === -1) {
                        properties.push(target[propertyName]);
                    }
                }
                visitor(target);
                visited.push(target);
                $.each(properties, function (_, property) {
                    property = ko.unwrap(property);
                    if (typeof property === 'object') {
                        objectsVisitor(property, visitor, visited, skip);
                    }
                });
            }
        }
        Designer.objectsVisitor = objectsVisitor;
        function collectionsVisitor(target, visitor, collectionsToProcess, visited) {
            if (collectionsToProcess === void 0) { collectionsToProcess = ["controls", "bands", "subBands", "crossBandControls", "rows", "cells", "fields"]; }
            if (visited === void 0) { visited = []; }
            if (visited.indexOf(target) !== -1) {
                return;
            }
            if (target && target !== undefined) {
                visited.push(target);
                for (var propertyName in target) {
                    if (collectionsToProcess.indexOf(propertyName) !== -1) {
                        visitor(target[propertyName]);
                        (target[propertyName]() || []).forEach(function (item) { return collectionsVisitor(item, visitor, collectionsToProcess, visited); });
                    }
                }
            }
        }
        Designer.collectionsVisitor = collectionsVisitor;
        function getImageClassName(controlType) {
            var controlType = getTypeNameFromFullName(controlType || "").split(".").join("_"), name;
            if (controlType.indexOf("XR") !== -1) {
                name = controlType.slice(2).toLowerCase();
            }
            else if (controlType === "DevExpress_XtraReports_UI_XtraReport") {
                name = "master_report";
            }
            return "dxrd-image-" + (name ? name : controlType.toLowerCase());
        }
        Designer.getImageClassName = getImageClassName;
        function getUniqueNameForNamedObjectsArray(objects, prefix) {
            if (prefix.indexOf("XR") === 0) {
                prefix = prefix[2].toLowerCase() + prefix.slice(3);
            }
            var indexBand = prefix.indexOf("Band");
            if (indexBand !== -1 && prefix !== "SubBand") {
                prefix = prefix.slice(0, indexBand) + prefix.slice(indexBand + 4);
            }
            return getUniqueName(objects.map(function (item) {
                return ko.unwrap(item.name);
            }), prefix);
        }
        Designer.getUniqueNameForNamedObjectsArray = getUniqueNameForNamedObjectsArray;
        function getUniqueName(names, prefix) {
            var i = 1, result = prefix + i;
            while (names.filter(function (item) {
                return item === result;
            }).length > 0) {
                i++;
                result = prefix + i;
            }
            ;
            return result;
        }
        Designer.getUniqueName = getUniqueName;
        var InlineTextEdit = (function () {
            function InlineTextEdit(selection) {
                var _this = this;
                this._showInline = ko.observable(false);
                this.text = ko.observable();
                var _text;
                this.singleControlSelected = ko.computed(function () {
                    if (selection.selectedItems) {
                        return selection.selectedItems.length === 1;
                    }
                    else
                        return selection.focused() ? true : false;
                });
                this.visible = ko.computed(function () {
                    var isControlFocused = !!(selection.focused() && selection.focused().getControlModel().text), isInlineShown = _this._showInline();
                    if (isControlFocused) {
                        _text = selection.focused().getControlModel().text;
                    }
                    else {
                        if (_text && _this._showInline()) {
                            _text(_this.text());
                            _this._showInline(false);
                        }
                    }
                    return isControlFocused && isInlineShown && _this.singleControlSelected();
                });
                this.show = function () {
                    if (_this._showInline()) {
                        return;
                    }
                    if (_this.singleControlSelected() && _text) {
                        _this.text(_text());
                        _this._showInline(true);
                    }
                    else {
                        _this._showInline(false);
                    }
                };
                this.keypressAction = function (args) {
                    if (args.jQueryEvent.keyCode === 27) {
                        _this._showInline(false);
                    }
                    if (args.jQueryEvent.keyCode === 13) {
                        _text(_this.text());
                        _this._showInline(false);
                    }
                };
            }
            return InlineTextEdit;
        })();
        Designer.InlineTextEdit = InlineTextEdit;
        ;
        var DesignControlsHelper = (function () {
            function DesignControlsHelper(target, handlers) {
                var _this = this;
                this._handlers = [];
                this._visitedCollections = [];
                this._setName = function (value) {
                    if (!value.name()) {
                        var controlType = value.controlType || "Unknown", initialText = value.getControlInfo && value.getControlInfo().defaultVal && value.getControlInfo().defaultVal["@Text"];
                        value.name(getUniqueNameForNamedObjectsArray(_this.allControls(), controlType));
                        if (value["text"] && !value["text"]() && (initialText === null || initialText === undefined)) {
                            value["text"](value.name());
                        }
                    }
                };
                this.added = function (value) {
                    _this._collectControls(value);
                };
                this.deleted = function (value) {
                    var index = _this.allControls.indexOf(value);
                    _this.allControls.splice(index, 1);
                    DevExpress.Designer.collectionsVisitor(value, function (collection) {
                        collection().forEach(function (item) {
                            _this.allControls.splice(_this.allControls.indexOf(item), 1);
                        });
                    });
                };
                this.allControls = ko.observableArray();
                var unwrappedTarget = target;
                if (ko.isSubscribable(target)) {
                    target.subscribe(function (newTarget) {
                        _this._visitedCollections = [];
                        _this.allControls([]);
                        _this._collectControls(newTarget);
                    });
                    unwrappedTarget = target.peek();
                }
                this.allControls.subscribe(function (args) {
                    args.forEach(function (value) {
                        _this._setName(value);
                    });
                });
                this._collectControls(unwrappedTarget);
                this._handlers.push.apply(this._handlers, handlers);
            }
            DesignControlsHelper.prototype._collectControls = function (target) {
                var _this = this;
                this.allControls.push(target);
                DevExpress.Designer.collectionsVisitor(target, function (collection) {
                    if (_this._visitedCollections.indexOf(collection) === -1) {
                        _this._visitedCollections.push(collection);
                        collection.subscribe(function (args) {
                            args.forEach(function (changeSet) {
                                _this[changeSet.status] && _this[changeSet.status](changeSet.value);
                                _this._handlers.forEach(function (handler) {
                                    handler[changeSet.status] && handler[changeSet.status](changeSet.value);
                                });
                            });
                        }, null, "arrayChange");
                    }
                    _this.allControls.push.apply(_this.allControls, collection());
                });
            };
            DesignControlsHelper.prototype.getControls = function (target) {
                var controls = ko.observableArray();
                DevExpress.Designer.collectionsVisitor(target, function (collection) {
                    controls.push.apply(controls, collection());
                });
                return controls;
            };
            return DesignControlsHelper;
        })();
        Designer.DesignControlsHelper = DesignControlsHelper;
        var ControlsFactory = (function () {
            function ControlsFactory() {
                this.controlsMap = {};
            }
            ControlsFactory.prototype.getControlType = function (model) {
                var controlType = getTypeNameFromFullName(model["@ControlType"] || "");
                return this.controlsMap[controlType] ? controlType : "Unknown";
            };
            ControlsFactory.prototype.createControl = function (model, parent, serializer) {
                var controlType = this.getControlType(model);
                return new (this.controlsMap[controlType] && this.controlsMap[controlType].type || Designer.ElementViewModel)(model, parent, serializer);
            };
            ControlsFactory.prototype.registerControl = function (typeName, metadata) {
                this.controlsMap[typeName] = metadata;
                this.controlsMap[typeName].info = $.extend(true, [], metadata.info);
            };
            ControlsFactory.prototype.getPropertyInfo = function (controlType, propertyDisplayName) {
                return this.controlsMap[controlType].info.filter(function (property) {
                    return property.displayName === propertyDisplayName;
                })[0];
            };
            return ControlsFactory;
        })();
        Designer.ControlsFactory = ControlsFactory;
        var SerializableModel = (function () {
            function SerializableModel(model, serializer, info) {
                serializer = serializer || new Designer.DesignerModelSerializer();
                serializer.deserialize(this, model, info);
            }
            return SerializableModel;
        })();
        Designer.SerializableModel = SerializableModel;
        function cutRefs(model) {
            objectsVisitor(model, function (target) {
                delete target["@Ref"];
            });
            return model;
        }
        Designer.cutRefs = cutRefs;
        var CssCalculator = (function () {
            function CssCalculator(control) {
                var _this = this;
                this.borderCss = function (zoom) {
                    var borderWidth = control["borderWidth"] && control["borderWidth"]() || "";
                    if (borderWidth && zoom) {
                        borderWidth = borderWidth * zoom;
                    }
                    var borderColor = control["borderColor"] && control["borderColor"]() || "";
                    var borders = control["borders"] && control["borders"]() || "";
                    var borderStyle = control["borderDashStyle"] && control["borderDashStyle"]() || "";
                    return _this.createBorders(borderStyle, borderWidth, borderColor, borders);
                };
                this.backGroundCss = function () {
                    return { backgroundColor: control["backColor"] && control["backColor"]() || "transparent" };
                };
                this.foreColorCss = function () {
                    var color = (control["foreColor"] && control["foreColor"]() || "transparent") === "transparent" ? "black" : control["foreColor"]();
                    return { color: color };
                };
                this.fontCss = function () {
                    return _this.createFont(control["font"] && control["font"]() || "");
                };
                this.paddingsCss = function () {
                    return _this.createPadding(control["padding"] && control["padding"]() || "");
                };
                this.textAlignmentCss = function () {
                    var align = control["textAlignment"] && control["textAlignment"]() || "";
                    return $.extend(_this.createVerticalAlignment(align), _this.createHorizontalAlignment(align));
                };
                this.stroke = function () {
                    var color = (control["foreColor"] && control["foreColor"]() || "transparent") === "transparent" ? "black" : control["foreColor"]();
                    return { 'stroke': color };
                };
                this.strokeWidth = function () {
                    var lineWidth = control["lineWidth"] && control["lineWidth"]() || "";
                    return { 'strokeWidth': lineWidth };
                };
                this.strokeWidthWithWidth = function () {
                    var lineWidth = control["width"] && control["width"]() || "";
                    return { 'strokeWidth': lineWidth };
                };
                this.strokeDashArray = function () {
                    var dashArray = _this.createStrokeDashArray(control["lineStyle"] && control["lineStyle"]() || "", control["lineWidth"] && control["lineWidth"]() || "");
                    return { 'strokeDasharray': dashArray };
                };
                this.strokeDashArrayWithWidth = function () {
                    var dashArray = _this.createStrokeDashArray(control["lineStyle"] && control["lineStyle"]() || "", control["width"] && control["width"]() || "");
                    return { 'strokeDasharray': dashArray };
                };
                this.crossBandBorder = function (position) {
                    return _this.createBorder(control["borderDashStyleCrossband"] && control["borderDashStyleCrossband"]() || "solid", control["borderWidth"] && control["borderWidth"]() || "", control["borderColor"] && control["borderColor"]() || "", control["borders"] && control["borders"]() || "", position);
                };
                this.angle = function () {
                    return _this.createAngle(control["angle"] && control["angle"]() || 0);
                };
                this.cellBorder = function (position, color) {
                    if (color === void 0) { color = "solid 1px Silver"; }
                    return _this.createControlBorder(control["borderDashStyle"] && control["borderDashStyle"]() || "solid", control["borderWidth"] && control["borderWidth"]() || "", control["borderColor"] && control["borderColor"]() || "", control["borders"] && control["borders"]() || "", position, color);
                };
                this.zipCodeFontCss = function (fontSize) {
                    return _this.createZipCodeFont(fontSize || control["size"]["height"]());
                };
                this.zipCodeAlignment = function () {
                    var align = "TopLeft";
                    return $.extend(_this.createVerticalAlignment(align), _this.createHorizontalAlignment(align));
                };
            }
            CssCalculator.prototype.createBorder = function (dashStyle, width, color, positions, position) {
                var line = {};
                positions = positions || "All";
                line = { stroke: "Silver", strokeWidth: 2 };
                var dash = this.createStrokeDashArray(dashStyle, width);
                if (positions.indexOf(position) !== -1 || positions.indexOf("All") !== -1) {
                    line["stroke"] = color;
                    line["strokeWidth"] = width;
                    line["strokeDasharray"] = dash;
                }
                return line;
            };
            CssCalculator.prototype.createControlBorder = function (borderStyle, width, color, positions, position, defaultColor) {
                if (defaultColor === void 0) { defaultColor = "solid 1px Silver"; }
                var border = {};
                positions = positions || "";
                border["border" + position] = defaultColor;
                if (borderStyle === "Dash") {
                    borderStyle = "dashed";
                }
                else if (borderStyle === "Dot") {
                    borderStyle = "dotted";
                }
                else if (borderStyle === "Double") {
                    borderStyle = "double";
                }
                else {
                    borderStyle = "solid";
                }
                if (positions.indexOf(position) !== -1 || positions.indexOf("All") !== -1) {
                    border["border" + position] = borderStyle + " " + width + "px " + color;
                }
                return border;
            };
            CssCalculator.prototype.createBorders = function (borderStyle, width, color, positions, defaultColor) {
                if (defaultColor === void 0) { defaultColor = "solid 1px Silver"; }
                var left = this.createControlBorder(borderStyle, width, color, positions, "Left", defaultColor);
                var right = this.createControlBorder(borderStyle, width, color, positions, "Right", defaultColor);
                var top = this.createControlBorder(borderStyle, width, color, positions, "Top", defaultColor);
                var bottom = this.createControlBorder(borderStyle, width, color, positions, "Bottom", defaultColor);
                var border = $.extend({}, left, right, top, bottom);
                return border;
            };
            CssCalculator.prototype.createZipCodeFont = function (height) {
                var fontStyles = {};
                fontStyles["fontFamily"] = "Impact";
                fontStyles["fontSize"] = height + "px";
                return fontStyles;
            };
            CssCalculator.prototype.createFont = function (fontString) {
                var fontStyles = {};
                fontString = fontString || "";
                var components = fontString.split(',');
                fontStyles["fontFamily"] = components[0];
                fontStyles["fontSize"] = components[1];
                if (components.length > 2) {
                    for (var i = 2; i < components.length; i++) {
                        if (components[i].indexOf("Bold") !== -1)
                            fontStyles["fontWeight"] = "Bold";
                        if (components[i].indexOf("Italic") !== -1)
                            fontStyles["fontStyle"] = "Italic";
                        if (components[i].indexOf("Underline") != -1)
                            fontStyles["textDecoration"] = "Underline";
                        if (components[i].indexOf("Strikeout") != -1)
                            fontStyles["textDecoration"] = (fontStyles["textDecoration"] ? fontStyles["textDecoration"] + " " : "") + "Line-through";
                    }
                }
                if (!fontStyles["fontWeight"]) {
                    fontStyles["fontWeight"] = "";
                }
                if (!fontStyles["fontStyle"]) {
                    fontStyles["fontStyle"] = "";
                }
                if (!fontStyles["textDecoration"]) {
                    fontStyles["textDecoration"] = "";
                }
                return fontStyles;
            };
            CssCalculator.prototype.createPadding = function (paddings) {
                var padding = {}, paddingModel = new Designer.Widgets.PaddingModel({ value: ko.observable(paddings) });
                padding["paddingLeft"] = paddingModel.left() + "px";
                padding["paddingTop"] = paddingModel.top() + "px";
                padding["paddingRight"] = paddingModel.right() + "px";
                padding["paddingBottom"] = paddingModel.bottom() + "px";
                return padding;
            };
            CssCalculator.prototype.createVerticalAlignment = function (alignment) {
                var result = {};
                if (alignment.indexOf("Top") !== -1) {
                    result["verticalAlign"] = "top";
                }
                if (alignment.indexOf("Middle") !== -1) {
                    result["verticalAlign"] = "middle";
                }
                if (alignment.indexOf("Bottom") !== -1) {
                    result["verticalAlign"] = "bottom";
                }
                return result;
            };
            CssCalculator.prototype.createHorizontalAlignment = function (alignment) {
                var result = {};
                if (alignment.indexOf("Left") !== -1) {
                    result["textAlign"] = "left";
                }
                if (alignment.indexOf("Right") !== -1) {
                    result["textAlign"] = "right";
                }
                if (alignment.indexOf("Center") !== -1) {
                    result["textAlign"] = "center";
                }
                if (alignment.indexOf("Justify") !== -1) {
                    result["textAlign"] = "justify";
                }
                return result;
            };
            CssCalculator.prototype.createStrokeDashArray = function (style, width) {
                if (style === "Solid") {
                    return "";
                }
                else if (style === "Dot") {
                    return [width, width * 2].join("px, ") + "px";
                }
                else if (style === "Dash") {
                    return [width * 4, width * 4].join("px, ") + "px";
                }
                else if (style === "DashDot") {
                    return [width * 4, width * 2, width, width * 2].join("px, ") + "px";
                }
                else if (style === "DashDotDot") {
                    return [width * 4, width * 2, width, width * 2, width, width * 2].join("px, ") + "px";
                }
                else {
                    return "";
                }
            };
            CssCalculator.prototype.createAngle = function (angle) {
                angle = -angle;
                return {
                    '-webkit-transform': "rotate(" + angle + "deg)",
                    '-moz-transform': "rotate(" + angle + "deg)",
                    '-o-transform': "rotate(" + angle + "deg)",
                    '-ms-transform': "rotate(" + angle + "deg)",
                    'transform': "rotate(" + angle + "deg)"
                };
            };
            return CssCalculator;
        })();
        Designer.CssCalculator = CssCalculator;
        var ObjectStructureProvider = (function () {
            function ObjectStructureProvider(target, displayName) {
                var _this = this;
                this.selectedPath = ko.observable("");
                this.selectedMember = ko.observable();
                this.getItems = function (pathRequest) {
                    var result = $.Deferred();
                    if (!pathRequest.fullPath) {
                        result.resolve([{ name: displayName, displayName: displayName, isList: true, specifics: displayName.toLowerCase(), dragData: { noDragable: true } }]);
                    }
                    else {
                        result.resolve(_this._getObjectPropertiesForPath(target, pathRequest.fullPath));
                    }
                    return result.promise();
                };
                this.selectedPath.subscribe(function (path) {
                    _this.selectedMember(_this._getMemberByPath(target, path));
                });
            }
            ObjectStructureProvider.prototype._getClassName = function (instance) {
                var funcNameRegex = /function (.{1,})\(/;
                var results = (funcNameRegex).exec((instance).constructor.toString());
                return (results && results.length > 1) ? results[1] : "";
            };
            ObjectStructureProvider.prototype._getMemberByPath = function (target, path) {
                var pathComponents = path.split("."), currentTarget = ko.unwrap(target);
                pathComponents.forEach(function (member) {
                    if (currentTarget[member]) {
                        currentTarget = ko.unwrap(currentTarget[member]);
                    }
                });
                return currentTarget;
            };
            ObjectStructureProvider.prototype._getObjectPropertiesForPath = function (target, path) {
                var _this = this;
                var currentTarget = this._getMemberByPath(target, path), result = [];
                if (currentTarget) {
                    var targetInfo = currentTarget.getInfo && currentTarget.getInfo();
                    if (currentTarget.push) {
                        $.each(currentTarget, function (index, arrayValue) {
                            var unwrapArrayValue = ko.unwrap(arrayValue), className = unwrapArrayValue.className && unwrapArrayValue.className();
                            result.push({
                                name: index.toString(),
                                displayName: ko.unwrap(unwrapArrayValue["displayName"] || unwrapArrayValue["name"]),
                                specifics: className || _this._getClassName(unwrapArrayValue),
                                isNeed: !!className
                            });
                        });
                    }
                    else {
                        $.each(currentTarget, function (propertyName, propertyValue) {
                            var propertyInfo = targetInfo && targetInfo.filter(function (propertyInfo) {
                                return propertyInfo.propertyName === propertyName;
                            })[0], unwrapPropertyValue = ko.unwrap(propertyValue);
                            if (typeof unwrapPropertyValue === 'object' && propertyInfo) {
                                result.push({
                                    name: propertyName,
                                    displayName: propertyInfo.displayName,
                                    specifics: propertyName
                                });
                            }
                        });
                    }
                }
                return result;
            };
            return ObjectStructureProvider;
        })();
        Designer.ObjectStructureProvider = ObjectStructureProvider;
        var ObjectStructureTreeListController = (function () {
            function ObjectStructureTreeListController(itemsProvider, propertyNames, listPropertyNames) {
                var _this = this;
                this.selectedItem = null;
                this.itemsFilter = function (item) {
                    return propertyNames ? propertyNames.indexOf(item.specifics) !== -1 || item["isNeed"] : true;
                };
                this.hasItems = function (item) {
                    return listPropertyNames ? listPropertyNames.indexOf(item.specifics) !== -1 : true;
                };
                this.getActions = function (item) {
                    return item.isSelected() && itemsProvider.selectedMember()["innerActions"] || [];
                };
                this.select = function (value) {
                    _this.selectedItem && _this.selectedItem.isSelected(false);
                    _this.selectedItem = value;
                    value.isSelected(true);
                };
            }
            ObjectStructureTreeListController.prototype.canSelect = function (value) {
                return true;
            };
            return ObjectStructureTreeListController;
        })();
        Designer.ObjectStructureTreeListController = ObjectStructureTreeListController;
        var TabPanel = (function () {
            function TabPanel(tabs) {
                var _this = this;
                this.tabs = [];
                this.collapsed = ko.observable(false);
                this.toggleCollapsedImageClassName = ko.computed(function () {
                    return _this.collapsed() ? "dxrd-image-propertygrid-expand" : "dxrd-image-propertygrid-collapse";
                });
                this.toggleCollapsedText = ko.computed(function () {
                    return _this.collapsed() ? "Open" : "Collapse";
                });
                this.tabs = tabs;
                ko.computed(function () {
                    var visibleTabs = tabs.filter(function (tab) {
                        return tab.visible();
                    });
                    if (visibleTabs.length !== 0) {
                        if (visibleTabs.filter(function (tab) {
                            return tab.active.peek();
                        }).length === 0) {
                            visibleTabs[0].active(true);
                        }
                    }
                    else {
                        _this.collapsed(true);
                    }
                });
                this.selectTab = function (e) {
                    var selectedTab = e.model;
                    _this.tabs.forEach(function (tab) {
                        tab.active(tab === selectedTab);
                    });
                    _this.collapsed(false);
                };
                var _width = ko.observable(340);
                this.width = ko.computed({
                    read: function () {
                        return _this.collapsed() ? 0 : _width();
                    },
                    write: function (newWidth) {
                        _width(newWidth);
                    }
                });
                this.headerWidth = ko.computed(function () {
                    return 50 + _this.width();
                });
            }
            return TabPanel;
        })();
        Designer.TabPanel = TabPanel;
        var TabInfo = (function () {
            function TabInfo(text, template, model, imageBaseName, computedVisible) {
                var _this = this;
                this.active = ko.observable(false);
                this.visible = ko.observable();
                imageBaseName = imageBaseName || text.toLowerCase();
                this.text = text;
                this.imageClassName = ko.computed(function () {
                    return "dxrd-image-" + imageBaseName + (_this.active() ? "-active" : "-inactive");
                });
                this.template = template;
                this.visible = ko.computed(function () {
                    return computedVisible !== undefined ? computedVisible() : true;
                });
                this.visible.subscribe(function (visibility) {
                    if (!visibility) {
                        _this.active(false);
                    }
                });
                this.model = model;
            }
            return TabInfo;
        })();
        Designer.TabInfo = TabInfo;
        function getFirstItemByPropertyValue(array, propertyName, propertyValue, fromIndex) {
            var fromIndex = fromIndex || 0;
            for (var i = fromIndex; i < array.length; i++) {
                var value = ko.isObservable(array[i][propertyName]) ? array[i][propertyName].peek() : array[i][propertyName];
                if (value === propertyValue) {
                    return array[i];
                }
            }
            return null;
        }
        Designer.getFirstItemByPropertyValue = getFirstItemByPropertyValue;
        var ControlsStore = (function (_super) {
            __extends(ControlsStore, _super);
            function ControlsStore(options) {
                if (options && options instanceof Array) {
                    this.visible = options.length > 1;
                    options.sort(function (a, b) {
                        if (a.name && a.name() && b.name && b.name()) {
                            var nameA = a.name().toLowerCase(), nameB = b.name().toLowerCase();
                            return (nameA < nameB) ? -1 : (nameA > nameB) ? 1 : 0;
                        }
                    });
                }
                _super.call(this, options);
            }
            ControlsStore.prototype.load = function (options) {
                if (options && options.filter && options.filter.length === 2) {
                    return $.Deferred().resolve([options.filter[1]]).promise();
                }
                return _super.prototype.load.call(this, options);
            };
            return ControlsStore;
        })(DevExpress.data.ArrayStore);
        Designer.ControlsStore = ControlsStore;
        function updateSurfaceContentSize(surfaceSize) {
            return function () {
                var rightAreaWidth = $(".dxrd-designer .dxrd-right-panel").outerWidth() + $(".dxrd-right-tabs").outerWidth();
                $(".dxrd-surface-wrapper").css("right", rightAreaWidth);
                var otherWidth = rightAreaWidth + $(".dxrd-toolbox-wrapper").outerWidth(), surfaceWidth = $(".dxrd-designer").width() - (otherWidth + 5);
                $(".dxrd-surface-wrapper").css("width", surfaceWidth);
                surfaceSize(surfaceWidth);
            };
        }
        Designer.updateSurfaceContentSize = updateSurfaceContentSize;
        function createPopularProperties(info, popularProperties) {
            var properties = [];
            popularProperties.forEach(function (name) {
                var property = info.filter(function (propertyInfo) {
                    return propertyInfo.propertyName === name;
                })[0];
                if (property) {
                    properties.push(property);
                }
            });
            return properties;
        }
        var PopupService = (function () {
            function PopupService() {
                this.data = ko.observable();
                this.title = ko.observable();
                this.visible = ko.observable(false);
                this.actions = ko.observableArray([]);
                this.target = ko.observable();
            }
            return PopupService;
        })();
        Designer.PopupService = PopupService;
        function generateDefaultParts(model) {
            return [
                { templateName: "dxrd-menubutton-template-base", model: model },
                { templateName: "dxrd-toolbar-template-base", model: model },
                { templateName: "dxrd-toolbox-template-base", model: model },
                { templateName: "dxrd-surface-template-base", model: model },
                { templateName: "dxrd-right-panel-template-base", model: model }
            ];
        }
        Designer.generateDefaultParts = generateDefaultParts;
        function deleteSelection(selection) {
            var focused = selection.focused();
            selection.selectedItems.forEach(function (item) {
                var itemModel = item.getControlModel(), parent = itemModel.parentModel();
                if (!item.getControlModel().getMetaData().isDeleteDeny && parent && item !== focused) {
                    parent.removeChild(itemModel);
                }
            });
            focused.getControlModel().parentModel().removeChild(focused.getControlModel());
            selection.focused(findNextSelection(focused));
        }
        Designer.deleteSelection = deleteSelection;
        function findNextSelection(removedElement) {
            var parentSurface = removedElement.parent;
            var targetSurface = parentSurface;
            if (parentSurface) {
                var childrenCollection = parentSurface.getChildrenCollection()();
                var indexInCollection = childrenCollection.indexOf(removedElement);
                if (indexInCollection === -1 && childrenCollection.length > 0) {
                    targetSurface = childrenCollection[childrenCollection.length - 1];
                }
                else if (childrenCollection.length > 1 && indexInCollection === childrenCollection.length - 1) {
                    targetSurface = childrenCollection[indexInCollection - 1];
                }
                else if (childrenCollection.length > 1 && indexInCollection === 0) {
                    targetSurface = childrenCollection[childrenCollection.length - 1];
                }
                else if (childrenCollection.length > 1) {
                    targetSurface = childrenCollection[childrenCollection.length - 1];
                }
                else if (indexInCollection === -1 && targetSurface.parent && targetSurface.parent.getChildrenCollection()().indexOf(targetSurface) === -1) {
                    targetSurface = findNextSelection(targetSurface);
                }
            }
            return targetSurface;
        }
        function createDesigner(model, surface, controlsFactory, groups, editors, parts) {
            if (groups === void 0) { groups = {}; }
            if (editors === void 0) { editors = []; }
            var undoEngine = new Designer.UndoEngine(model), selection = new Designer.SurfaceSelection(), snapHelper = new Designer.SnapLinesHelper(surface), controlsHelper = ko.computed(function () {
                return new DesignControlsHelper(model);
            }), dragHelperContent = new Designer.DragHelperContent(selection), toolboxItems = Designer.getToolboxItems(controlsFactory.controlsMap), appMenuVisible = ko.observable(false), inlineTextEdit = new InlineTextEdit(selection), editableObject = Designer.CombinedObject.getEditableObject(selection, undoEngine).extend({ throttle: 1 }), popularPG = new Designer.Widgets.ObjectProperties(ko.computed(function () {
                var popularProperties = { getInfo: function () {
                    return [];
                } }, editable = editableObject();
                if (editable) {
                    var controlInfo = controlsFactory.controlsMap[editable.controlType], propertiesInfo = createPopularProperties(controlInfo && controlInfo.info || [], controlInfo && controlInfo.popularProperties || []);
                    (propertiesInfo).forEach(function (item) {
                        popularProperties[item.propertyName] = editable[item.propertyName];
                    });
                    popularProperties.getInfo = function () {
                        return propertiesInfo;
                    };
                    popularProperties["actions"] = editable.actions;
                    popularProperties["isPropertyModified"] = function (name) {
                        return editable.isPropertyModified ? editable.isPropertyModified(name) : false;
                    };
                    popularProperties["getActionClassName"] = function (name) {
                        return editable["getActionClassName"] ? editable["getActionClassName"](name) : "";
                    };
                }
                return popularProperties;
            })), globalProperties = new Designer.Widgets.ControlProperties(editableObject, { groups: groups, editors: editors }), tabPanel = new TabPanel([new TabInfo("Properties", "dxrd-propertieswrapper", globalProperties, undefined)]);
            var designerModel = {
                parts: parts,
                model: model,
                isLoading: ko.observable(true),
                surface: surface,
                surfaceSize: ko.observable(0),
                controlsHelper: controlsHelper,
                selection: selection,
                undoEngine: undoEngine,
                toolboxItems: toolboxItems,
                editableObject: editableObject,
                popularProperties: popularPG,
                tabPanel: tabPanel,
                getControlsDS: function () {
                    return new ControlsStore(controlsHelper().allControls());
                },
                appMenuVisible: appMenuVisible,
                toggleAppMenu: function () {
                    appMenuVisible(!appMenuVisible());
                },
                actionLists: new Designer.ActionLists(surface, selection, undoEngine, function () {
                }),
                elementActions: new Designer.ActionsWrapper(surface, selection, undoEngine),
                inlineTextEdit: inlineTextEdit,
                resizeHandler: {
                    starting: function () {
                        selection.expectClick = true;
                        undoEngine.start();
                    },
                    stopped: function () {
                        undoEngine.end();
                        setTimeout(function () {
                            selection.expectClick = false;
                        }, 100);
                    }
                },
                snapHelper: snapHelper,
                dragHelperContent: dragHelperContent,
                dragHandler: new Designer.SelectionDragDropHandler(surface, selection, undoEngine, snapHelper, dragHelperContent),
                toolboxDragHandler: new Designer.ToolboxDragDropHandler(surface, selection, undoEngine, snapHelper, dragHelperContent, controlsFactory),
            };
            designerModel.parts = designerModel.parts || generateDefaultParts(designerModel);
            return designerModel;
        }
        Designer.createDesigner = createDesigner;
        function getEditorType(typeString) {
            if (typeString === "multiValueWithLookUp") {
                return DevExpress.Designer.Widgets.editorTemplates.multiValue;
            }
            if (typeString === "multiValue") {
                return DevExpress.Designer.Widgets.editorTemplates.multiValueEditable;
            }
            if (typeString === "Enum") {
                return DevExpress.Designer.Widgets.editorTemplates.combobox;
            }
            if (typeString === "System.String") {
                return DevExpress.Designer.Widgets.editorTemplates.text;
            }
            if (typeString === "System.Guid") {
                return DevExpress.Designer.Widgets.editorTemplates.text;
            }
            if (typeString === "System.SByte" || typeString === "System.Int32" || typeString === "System.Int16" || typeString === "System.Single" || typeString === "System.Double" || typeString === "System.Byte" || typeString === "System.UInt16" || typeString === "System.UInt32" || typeString === "System.Byte") {
                return DevExpress.Designer.Widgets.editorTemplates.numeric;
            }
            if (typeString === "System.Boolean") {
                return DevExpress.Designer.Widgets.editorTemplates.bool;
            }
            if (typeString === "System.DateTime") {
                return DevExpress.Designer.Widgets.editorTemplates.date;
            }
            if (typeString === "DevExpress.DataAccess.Expression") {
                return DevExpress.Designer.Widgets.editorTemplates.expressionEditor;
            }
            return DevExpress.Designer.Widgets.editorTemplates.text;
        }
        Designer.getEditorType = getEditorType;
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        var SnapLine = (function () {
            function SnapLine(x, y, isVertical, maxHeight, maxWidth) {
                var _this = this;
                if (y === void 0) { y = ko.observable(0); }
                if (isVertical === void 0) { isVertical = true; }
                if (maxHeight === void 0) { maxHeight = ko.observable(1001); }
                if (maxWidth === void 0) { maxWidth = ko.observable(1001); }
                this.originalX = ko.observable(0);
                this.originalY = ko.observable(0);
                this.active = ko.observable(false);
                this.position = new Designer.Rectangle();
                this.isVertical = true;
                this.maxHeight = ko.observable(0);
                this.maxWidth = ko.observable(0);
                this.isVertical = isVertical;
                ko.computed(function () {
                    _this.maxHeight(maxHeight());
                    _this.maxWidth(maxWidth());
                    _this.originalX(x());
                    _this.originalY(y());
                    if (isVertical) {
                        _this.position.left(x());
                        _this.position.height(maxHeight());
                    }
                    else {
                        _this.position.top(y());
                        _this.position.width(maxWidth());
                    }
                });
            }
            SnapLine.prototype.activate = function (position) {
                if (this.isVertical) {
                    var top = Math.min(this.originalY(), position.top), bottom = Math.max(this.originalY(), position.top);
                    this.position.height(bottom - top);
                    this.position.top(top);
                }
                else {
                    var left = Math.min(this.originalX(), position.left), right = Math.max(this.originalX(), position.left);
                    this.position.width(right - left);
                    this.position.left(left);
                }
                this.active(this.position.left() + this.position.width() < this.maxWidth() && this.position.height() + this.position.top() < this.maxHeight() && this.position.left() >= 0 && this.position.top() >= 0);
            };
            return SnapLine;
        })();
        Designer.SnapLine = SnapLine;
        var SnapLinesHelper = (function () {
            function SnapLinesHelper(surface) {
                if (surface === void 0) { surface = null; }
                this.snapLines = ko.observableArray([]);
                this._surfaceContext = surface;
            }
            SnapLinesHelper.prototype._getActiveSnapLines = function (position, tolerance) {
                if (tolerance === void 0) { tolerance = SnapLinesHelper.snapTolerance; }
                var result = [], horizontal = [], vertical = [];
                this.snapLines().forEach(function (item) {
                    if (item.isVertical) {
                        var currentDistance = Math.abs(item.position.left() - position.left);
                        if (currentDistance < tolerance) {
                            vertical.push({ snapLine: item, distance: currentDistance });
                        }
                    }
                    else {
                        var currentDistance = Math.abs(item.position.top() - position.top);
                        if (currentDistance < tolerance) {
                            horizontal.push({ snapLine: item, distance: currentDistance });
                        }
                    }
                });
                vertical.sort(function (a, b) {
                    return a.distance - b.distance;
                });
                horizontal.sort(function (a, b) {
                    return a.distance - b.distance;
                });
                if (vertical.length !== 0) {
                    result.push(vertical[0].snapLine);
                }
                if (horizontal.length !== 0) {
                    result.push(horizontal[0].snapLine);
                }
                return result;
            };
            SnapLinesHelper.prototype.updateSnapLines = function () {
                var controls = [];
                var newSnapLines = [];
                Designer.collectionsVisitor(this._surfaceContext(), function (targetProperty) {
                    controls.push.apply(controls, targetProperty());
                }, ["bands", "controls", "rows", "cells"]);
                controls.forEach(function (controlSurface) {
                    if (controlSurface.isSnapTarget) {
                        Array.prototype.push.apply(newSnapLines, controlSurface.snapLines());
                    }
                });
                this.snapLines(newSnapLines);
            };
            SnapLinesHelper.prototype.deactivateSnapLines = function () {
                this.snapLines().forEach(function (item) {
                    item.active(false);
                });
            };
            SnapLinesHelper.prototype.activateSnapLines = function (position) {
                var activeSnapLines = this._getActiveSnapLines(position);
                activeSnapLines.forEach(function (snapLine) {
                    snapLine.activate(position);
                });
            };
            SnapLinesHelper.snapTolerance = 10;
            return SnapLinesHelper;
        })();
        Designer.SnapLinesHelper = SnapLinesHelper;
        var DragHelperContent = (function (_super) {
            __extends(DragHelperContent, _super);
            function DragHelperContent(selectionProvider) {
                _super.call(this);
                this.controls = ko.observableArray([]);
                this._selectionProvider = selectionProvider;
            }
            DragHelperContent.prototype.update = function (surface) {
                var _this = this;
                this.controls([]);
                this.left(surface.absolutePosition.x());
                this.top(surface.absolutePosition.y());
                this.width(surface.rect().width);
                this.height(surface.rect().height);
                this._selectionProvider.selectedItems.forEach(function (controlSurface) {
                    if (controlSurface.parent === surface.parent) {
                        _this.controls.push({
                            left: ko.observable(controlSurface.absolutePosition.x() - _this.left()),
                            top: ko.observable(controlSurface.absolutePosition.y() - _this.top()),
                            width: ko.observable(controlSurface.rect().width),
                            height: ko.observable(controlSurface.rect().height)
                        });
                    }
                });
            };
            DragHelperContent.prototype.setContent = function (area) {
                this.controls([]);
                this.left(area.left());
                this.top(area.top());
                this.width(area.width());
                this.height(area.height());
                this.controls.push(area);
            };
            return DragHelperContent;
        })(Designer.Rectangle);
        Designer.DragHelperContent = DragHelperContent;
        var DragDropHandler = (function () {
            function DragDropHandler(surface, selection, undoEngine, snapHelper) {
                var _this = this;
                this._size = new Designer.Size(0, 0);
                this.surface = surface;
                this.selection = selection;
                this.snapHelper = snapHelper;
                this.stopDrag = function (ui, draggable) {
                    undoEngine.start();
                    _this.doStopDrag(ui, draggable);
                    undoEngine.end();
                    snapHelper.deactivateSnapLines();
                };
            }
            DragDropHandler.prototype._getAbsoluteSurfacePosition = function (ui) {
                return { left: ui.position.left - ui["delta"].left, top: ui.position.top - ui["delta"].top };
            };
            DragDropHandler.prototype.addControl = function (control, dropTargetSurface, size) {
                var targetWidth = (dropTargetSurface["width"] && dropTargetSurface["width"]()) || (dropTargetSurface["_width"] && dropTargetSurface["_width"]());
                var underCursor = dropTargetSurface.underCursor();
                if (underCursor.x < targetWidth) {
                    dropTargetSurface.getControlModel().addChild(control);
                    var controlSurface = Designer.findSurface(control);
                    var width = size.width(), height = size.height();
                    if (underCursor.x + width > targetWidth) {
                        controlSurface.rect({ left: targetWidth - width - 1, top: underCursor.y, width: width, height: height });
                    }
                    else {
                        controlSurface.rect({ left: underCursor.x, top: underCursor.y, width: width, height: height });
                    }
                    this.selection.initialize(controlSurface);
                }
            };
            DragDropHandler.prototype.recalculateSize = function (size) {
                var surface = ko.unwrap(this.surface);
                this._size.width(Designer.unitsToPixel(ko.unwrap(size.width) * surface.dpi() / 100, surface.measureUnit(), surface.zoom()));
                this._size.height(Designer.unitsToPixel(ko.unwrap(size.height) * surface.dpi() / 100, surface.measureUnit(), surface.zoom()));
            };
            DragDropHandler.prototype.helper = function (draggable) {
                this.snapHelper.updateSnapLines();
            };
            DragDropHandler.prototype.startDrag = function (draggable) {
            };
            DragDropHandler.prototype.drag = function (event, ui) {
                this.snapHelper.deactivateSnapLines();
                if (event.altKey !== true) {
                    var position = this._getAbsoluteSurfacePosition(ui);
                    this.snapHelper.activateSnapLines(position);
                    if (this._size.width() !== 0) {
                        this.snapHelper.activateSnapLines({ left: position.left + (this._size.width()), top: position.top });
                        this.snapHelper.activateSnapLines({ left: position.left, top: position.top + (this._size.height()) });
                    }
                }
            };
            DragDropHandler.prototype.doStopDrag = function (ui, draggable) {
            };
            return DragDropHandler;
        })();
        Designer.DragDropHandler = DragDropHandler;
        var SelectionDragDropHandler = (function (_super) {
            __extends(SelectionDragDropHandler, _super);
            function SelectionDragDropHandler(surface, selection, undoEngine, snapHelper, dragHelperContent) {
                var _this = this;
                _super.call(this, surface, selection, undoEngine, snapHelper);
                this.cursor = 'move';
                this.containment = '.dxrd-ghost-container';
                this["helper"] = function (draggable) {
                    _super.prototype.helper.call(_this, draggable);
                    dragHelperContent.update(draggable);
                };
            }
            SelectionDragDropHandler.prototype.adjustDropTarget = function (dropTargetSurface) {
                var selectedItemInTree = dropTargetSurface;
                while (selectedItemInTree != null) {
                    if (selectedItemInTree.selected && selectedItemInTree.selected()) {
                        dropTargetSurface = selectedItemInTree.parent;
                        break;
                    }
                    selectedItemInTree = selectedItemInTree.parent;
                }
                return dropTargetSurface;
            };
            SelectionDragDropHandler.prototype.startDrag = function (control) {
                this.selection.swapFocusedItem(control);
                var focusedSurface = this.selection.focused();
                var baseOffsetX = focusedSurface.rect().left + focusedSurface.underCursor().x;
                var baseOffsetY = focusedSurface.rect().top + focusedSurface.underCursor().y;
                this.selection.selectedItems.forEach(function (item) {
                    if (item.parent === focusedSurface.parent) {
                        item.underCursor().offsetX = item.rect().left - baseOffsetX;
                        item.underCursor().offsetY = item.rect().top - baseOffsetY;
                    }
                });
            };
            SelectionDragDropHandler.prototype.drag = function (event, ui) {
                ui.position.left += ui["scroll"].left;
                ui.position.top += ui["scroll"].top;
                _super.prototype.drag.call(this, event, ui);
            };
            SelectionDragDropHandler.prototype.doStopDrag = function (ui, _) {
                var _this = this;
                if (this.selection.dropTarget) {
                    var dropTarget = this.selection.dropTarget.getControlModel(), dropTargetSurface = dropTarget.getMetaData().isContainer ? this.selection.dropTarget : this.selection.dropTarget.parent;
                    if (!dropTargetSurface) {
                        console.log("%o", this.selection.dropTarget);
                        return;
                    }
                    var dropPointRelativeX = ui.position.left - dropTargetSurface["absolutePosition"].x();
                    var dropPointRelativeY = ui.position.top - dropTargetSurface["absolutePosition"].y();
                    var focusedSurface = this.selection.focused();
                    dropTargetSurface.underCursor().x = dropPointRelativeX - focusedSurface.underCursor().offsetX;
                    dropTargetSurface.underCursor().y = dropPointRelativeY - focusedSurface.underCursor().offsetY;
                    var itemsToDrop = [focusedSurface];
                    this.selection.selectedItems.forEach(function (item) {
                        if (focusedSurface !== item && item.parent === focusedSurface.parent) {
                            itemsToDrop.push(item);
                        }
                    });
                    itemsToDrop.forEach(function (item) {
                        var adjustedTarget = _this.adjustDropTarget(dropTargetSurface), parent = item.getControlModel().getNearestParent(adjustedTarget.getControlModel());
                        adjustedTarget = Designer.findSurface(parent);
                        var ajustLocation = function () {
                            var left = adjustedTarget.underCursor().x + item.underCursor().offsetX, top = adjustedTarget.underCursor().y + item.underCursor().offsetY;
                            item.rect({ left: left > 0 ? left : 0, top: top > 0 ? top : 0 });
                        };
                        if (item.parent !== adjustedTarget) {
                            item.parent.getControlModel().removeChild(item.getControlModel());
                            ajustLocation();
                            parent.addChild(item.getControlModel());
                        }
                        else {
                            ajustLocation();
                        }
                    });
                }
            };
            return SelectionDragDropHandler;
        })(DragDropHandler);
        Designer.SelectionDragDropHandler = SelectionDragDropHandler;
        var ToolboxDragDropHandler = (function (_super) {
            __extends(ToolboxDragDropHandler, _super);
            function ToolboxDragDropHandler(surface, selection, undoEngine, snapHelper, dragHelperContent, controlsFactory) {
                var _this = this;
                _super.call(this, surface, selection, undoEngine, snapHelper);
                this.cursor = 'arrow';
                this._controlsFactory = controlsFactory;
                this.containment = '.dxrd-designer';
                this["cursorAt"] = {
                    top: 0,
                    left: 0
                };
                this["helper"] = function (draggable) {
                    _super.prototype.helper.call(_this, draggable);
                    var toolboxItem = draggable;
                    var size = Designer.Size.fromString(toolboxItem.info["@SizeF"] || toolboxItem.info["size"] || "100,23");
                    _this.recalculateSize(size);
                    dragHelperContent.setContent(new Designer.Rectangle(0, 0, _this._size.width(), _this._size.height()));
                };
            }
            ToolboxDragDropHandler.prototype.doStopDrag = function (ui, draggable) {
                if (this.selection.dropTarget) {
                    var toolboxItem = draggable, control = this._controlsFactory.createControl($.extend({}, toolboxItem.info), null), parent = control.getNearestParent(this.selection.dropTarget.getControlModel()), dropTargetSurface = Designer.findSurface(parent);
                    var position = this._getAbsoluteSurfacePosition(ui);
                    dropTargetSurface.underCursor().x = position.left - (dropTargetSurface["absolutePosition"] && dropTargetSurface["absolutePosition"].x() || 0);
                    dropTargetSurface.underCursor().y = position.top - (dropTargetSurface["absolutePosition"] && dropTargetSurface["absolutePosition"].y() || 0);
                    if (this.surface().isFit && this.surface().isFit(dropTargetSurface) || dropTargetSurface.underCursor().isOver) {
                        this.addControl(control, dropTargetSurface, this._size);
                    }
                }
            };
            return ToolboxDragDropHandler;
        })(DragDropHandler);
        Designer.ToolboxDragDropHandler = ToolboxDragDropHandler;
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        var Widgets;
        (function (Widgets) {
            function compareEditorInfo(editor1, editor2) {
                return !!editor1 && !!editor2 && editor1.header === editor2.header && editor1.content === editor2.content && editor1.editorType === editor2.editorType;
            }
            var EditorAddOn = (function () {
                function EditorAddOn(editor, popupService) {
                    var _this = this;
                    this.showPopup = function (args) {
                        _this._popupService.title(_this._editor.displayName);
                        _this._updateActions(_this._editor._model());
                        _this._popupService.target(args.element);
                        _this._popupService.visible(true);
                    };
                    this.templateName = "dxrd-editor-addons";
                    this._editor = editor;
                    this._popupService = popupService;
                    this.visible = ko.computed(function () {
                        return editor._model() && editor._model().actions && editor._model().actions.length > 0;
                    });
                    this.editorMenuButtonCss = ko.computed(function () {
                        return editor._model() && editor._model()["getActionClassName"] && editor._model()["getActionClassName"](editor.name) || "";
                    });
                }
                EditorAddOn.prototype._updateActions = function (viewModel) {
                    var _this = this;
                    this._popupService.actions([]);
                    if (viewModel.actions) {
                        viewModel.actions.forEach(function (modelAction) {
                            if (modelAction.visible(_this._editor.name)) {
                                _this._popupService.actions.push({
                                    action: function () {
                                        modelAction.action(_this._editor.name);
                                        _this._popupService.visible(false);
                                    },
                                    title: modelAction.title,
                                    visible: function () {
                                        return true;
                                    }
                                });
                            }
                        });
                    }
                };
                return EditorAddOn;
            })();
            Widgets.EditorAddOn = EditorAddOn;
            var ObjectProperties = (function () {
                function ObjectProperties(target, editorsInfo, level) {
                    var _this = this;
                    if (level === void 0) { level = 0; }
                    this.level = 0;
                    this._editors = ko.observableArray([]);
                    this.level = level;
                    ko.computed(function () {
                        var viewModel = target();
                        var serializationInfo = editorsInfo && editorsInfo.editors || viewModel && viewModel["getInfo"] && viewModel["getInfo"]();
                        _this._createEditors(viewModel, serializationInfo);
                        _this.update(viewModel);
                    });
                }
                ObjectProperties.prototype.update = function (viewModel) {
                    if (viewModel) {
                        this._editors().forEach(function (editor) {
                            editor.update(viewModel);
                        });
                    }
                };
                ObjectProperties.prototype.createEditor = function (modelPropertyInfo) {
                    var editorType = modelPropertyInfo.editor && modelPropertyInfo.editor.editorType || Editor;
                    return new editorType(modelPropertyInfo, this.level);
                };
                ObjectProperties.prototype.createEditors = function (serializationInfo) {
                    var _this = this;
                    var self = this;
                    return (serializationInfo || []).filter(function (info) {
                        return !!info.editor && self._editors().filter(function (editor) { return editor.name === info.propertyName && compareEditorInfo(editor.info().editor, info.editor); }).length === 0;
                    }).map(function (info) {
                        return _this.createEditor(info);
                    });
                };
                ObjectProperties.prototype._createEditors = function (target, serializationInfo) {
                    var _this = this;
                    if (!serializationInfo)
                        return false;
                    this.createEditors(serializationInfo).forEach(function (editor) { return _this._editors.push(editor); });
                    var propertyNames = serializationInfo.map(function (info) {
                        return info.propertyName;
                    });
                    this._editors.sort(function (a, b) {
                        return propertyNames.indexOf(a.name) - propertyNames.indexOf(b.name);
                    });
                };
                ObjectProperties.prototype.getEditors = function () {
                    return this._editors();
                };
                return ObjectProperties;
            })();
            Widgets.ObjectProperties = ObjectProperties;
            var ControlProperties = (function (_super) {
                __extends(ControlProperties, _super);
                function ControlProperties(target, editorsInfo, level) {
                    var _this = this;
                    if (level === void 0) { level = 0; }
                    _super.call(this, target, editorsInfo, level);
                    this.focusedItem = ko.observable();
                    this.createEditorAddOn = function (editor) {
                        var editorAddOn = new EditorAddOn(editor, _this.popupService);
                        return {
                            templateName: editorAddOn.templateName,
                            data: editorAddOn
                        };
                    };
                    this.popupService = new Designer.PopupService();
                    this.createGroups(editorsInfo.groups);
                    this.update(target());
                    this.focusedImageClassName = ko.computed(function () {
                        return Designer.getImageClassName(target() && target().controlType);
                    });
                    this.focusedItem = target;
                    this.displayExpr = function (value) {
                        var displayName = value && (ko.unwrap(value.name) || ko.unwrap(value.displayName)), controlType = value && value.controlType;
                        return displayName + (controlType ? (' (' + DevExpress.Designer.getShortTypeName(controlType) + ')') : '');
                    };
                }
                ControlProperties.prototype.update = function (viewModel) {
                    _super.prototype.update.call(this, viewModel);
                    if (viewModel) {
                        (this.groups || []).forEach(function (group) {
                            group.update(viewModel);
                        });
                    }
                };
                ControlProperties.prototype.createGroups = function (groups) {
                    var _this = this;
                    this.groups = $.map(groups, function (groupInfo, displayName) {
                        return new Group(displayName, groupInfo, function (serializationInfo) {
                            return _this.createEditors(serializationInfo);
                        });
                    });
                };
                return ControlProperties;
            })(ObjectProperties);
            Widgets.ControlProperties = ControlProperties;
            var Editor = (function () {
                function Editor(modelPropertyInfo, level) {
                    var _this = this;
                    this._model = ko.observable();
                    this.isVisibleByContent = ko.observable(true);
                    this.isEditorSelected = ko.observable(false);
                    this.isPropertyModified = ko.computed(function () {
                        return _this._model() && _this._model().isPropertyModified && _this._model().isPropertyModified(_this.name);
                    });
                    this.collapsed = ko.observable(true);
                    this.info = ko.observable(modelPropertyInfo);
                    this.padding = 19 * level;
                    var defaultValue = ko.observable(null), propertyName = modelPropertyInfo.propertyName;
                    this["localizationId"] = modelPropertyInfo.localizationId;
                    this.validationRules = modelPropertyInfo.validationRules;
                    if (modelPropertyInfo.defaultVal !== undefined) {
                        defaultValue = ko.observable(modelPropertyInfo.defaultVal);
                    }
                    if (modelPropertyInfo.from) {
                        defaultValue = modelPropertyInfo.from(modelPropertyInfo.defaultVal);
                    }
                    if (modelPropertyInfo.array) {
                        defaultValue = ko.observableArray();
                    }
                    this.values = ko.computed(function () {
                        if (_this.info().values) {
                            return $.map(_this.info().values, function (displayValue, value) {
                                return { value: value, displayValue: displayValue };
                            });
                        }
                        if (_this.info().valuesArray) {
                            return $.map(_this.info().valuesArray, function (value) {
                                return { value: value.value, displayValue: value.displayValue };
                            });
                        }
                    });
                    this.level = level;
                    var editor = modelPropertyInfo.editor;
                    this._init(modelPropertyInfo.displayName, editor, defaultValue, propertyName);
                    this.disabled = ko.computed(function () {
                        var model = _this._model(), result = model && model.isPropertyDisabled && model.isPropertyDisabled(_this.name);
                        if (!result) {
                            var info = _this.info();
                            if (info && info.disabled !== void 0) {
                                result = ko.unwrap(info.disabled);
                            }
                        }
                        return result;
                    });
                    this.visible = ko.computed(function () {
                        var model = _this._model(), result = (model && model.isPropertyVisible) ? model.isPropertyVisible(_this.name) : _this.isVisibleByContent();
                        if (result) {
                            var info = _this.info();
                            if (info && info.visible !== void 0) {
                                result = ko.unwrap(info.visible);
                            }
                        }
                        return result;
                    });
                }
                Editor.prototype._init = function (displayName, editorTemplate, value, name) {
                    var _this = this;
                    this.displayName = displayName;
                    editorTemplate = editorTemplate || Widgets.editorTemplates.text;
                    this.templateName = editorTemplate.header;
                    this.contentTemplateName = editorTemplate.content;
                    this.defaultValue = editorTemplate === Widgets.editorTemplates.color ? "transparent" : null;
                    this.value = ko.computed({
                        read: function () {
                            var model = _this._model();
                            var modelValue = model && model[name] || value;
                            if (ko.isObservable(modelValue) && !modelValue["push"]) {
                                var hasValueInModel = modelValue() !== undefined && modelValue() !== null;
                                return hasValueInModel ? modelValue() : _this.defaultValue;
                            }
                            else {
                                return modelValue;
                            }
                        },
                        write: function (val) {
                            var model = _this._model();
                            if (!model) {
                                return;
                            }
                            var modelValue = model[name];
                            if (ko.isObservable(modelValue)) {
                                modelValue(val);
                            }
                            else {
                                model[name] = val;
                            }
                        }
                    });
                    this.name = name;
                    this.editorTemplate = editorTemplate && editorTemplate.custom || 'dxrd-property-editor';
                };
                Editor.prototype.findPropertyInfo = function (viewModel) {
                    var _this = this;
                    var modelInfo = viewModel["getInfo"] && viewModel["getInfo"]();
                    if (modelInfo) {
                        return modelInfo.filter(function (property) { return property.propertyName === _this.name; })[0];
                    }
                    return null;
                };
                Editor.prototype.updateInfo = function (propertyInfo) {
                    if (propertyInfo && compareEditorInfo(propertyInfo.editor, this.info().editor)) {
                        this.info(propertyInfo);
                        return true;
                    }
                    return !propertyInfo;
                };
                Editor.prototype.update = function (viewModel) {
                    var propertyInfo = this.findPropertyInfo(viewModel);
                    this.isVisibleByContent(viewModel[this.name] !== undefined && this.updateInfo(propertyInfo));
                    this._model(this.isVisibleByContent() ? viewModel : null);
                };
                Object.defineProperty(Editor.prototype, "isComplexEditor", {
                    get: function () {
                        return !!this.contentTemplateName;
                    },
                    enumerable: true,
                    configurable: true
                });
                return Editor;
            })();
            Widgets.Editor = Editor;
            var PropertyGridEditor = (function (_super) {
                __extends(PropertyGridEditor, _super);
                function PropertyGridEditor(info, level) {
                    var _this = this;
                    _super.call(this, info, level);
                    this.editorCreated = ko.observable(false);
                    this.collapsed.subscribe(function () {
                        if (!_this.editorCreated()) {
                            _this.viewmodel = new ObjectProperties(_this.value, { editors: info.info }, level + 1);
                            _this.editorCreated(true);
                        }
                    });
                    this.viewmodel = {};
                }
                return PropertyGridEditor;
            })(Editor);
            Widgets.PropertyGridEditor = PropertyGridEditor;
            var FontEditor = (function (_super) {
                __extends(FontEditor, _super);
                function FontEditor(info, level) {
                    _super.call(this, info, level);
                    var model = new Widgets.FontModel({ value: this.value });
                    var grid = new ObjectProperties(ko.observable(model), { editors: Designer.fontInfoFake }, level + 1);
                    this.viewmodel = grid;
                }
                return FontEditor;
            })(Editor);
            Widgets.FontEditor = FontEditor;
            var Group = (function () {
                function Group(displayName, serializationsInfo, createEditors, collapsed) {
                    var _this = this;
                    if (collapsed === void 0) { collapsed = true; }
                    this.editors = ko.observableArray();
                    this.editorsCreated = ko.observable(false);
                    this.displayName = displayName;
                    this._serializationsInfo = serializationsInfo;
                    this.collapsed = ko.observable(collapsed);
                    this.visible = ko.observable(false);
                    if (collapsed) {
                        var subscription = this.collapsed.subscribe(function (val) {
                            subscription.dispose();
                            _this.editors(createEditors(serializationsInfo));
                            if (_this._viewModel) {
                                _this.editors().forEach(function (editor) {
                                    editor.update(_this._viewModel);
                                });
                            }
                        });
                    }
                    else {
                        this.editors(createEditors(serializationsInfo));
                    }
                }
                Group.prototype.update = function (viewModel) {
                    var _this = this;
                    this._viewModel = viewModel;
                    if (viewModel) {
                        var isVisible = (viewModel.getInfo && viewModel.getInfo() || this._serializationsInfo).filter(function (modelInfo) {
                            return _this._serializationsInfo.filter(function (info) {
                                return info.propertyName === modelInfo.propertyName;
                            }).length > 0 && !!viewModel[modelInfo.propertyName] && ko.unwrap(modelInfo.visible) !== false;
                        }).length > 0;
                        this.visible(isVisible);
                        if (isVisible) {
                            this.editors().forEach(function (editor) {
                                editor.update(_this._viewModel);
                            });
                        }
                    }
                    else {
                        this.visible(false);
                    }
                };
                return Group;
            })();
            Widgets.Group = Group;
            var MultiValuesHelper = (function () {
                function MultiValuesHelper(value, items) {
                    var _this = this;
                    var values = value();
                    this._items = items.map(function (item) {
                        var selected = ko.observable(values.indexOf(item.value) !== -1);
                        return { selected: selected, value: item.value, displayValue: item.displayValue || item.value, toggleSelected: function () {
                            selected(!selected());
                        } };
                    });
                    this.selectedValuesString = ko.computed({
                        read: function () {
                            return _this._items.filter(function (item) {
                                return item.selected();
                            }).map(function (item) {
                                return item.displayValue;
                            }).join(", ");
                        },
                        write: function (newValue) {
                        }
                    });
                    this.isSelectedAll = ko.computed({
                        read: function () {
                            var selectedItemCount = _this._items.filter(function (item) {
                                return item.selected();
                            }).length;
                            if (selectedItemCount > 0 && selectedItemCount < _this._items.length) {
                                return undefined;
                            }
                            return selectedItemCount === _this._items.length;
                        },
                        write: function (newValue) {
                            _this._items.forEach(function (item) {
                                item.selected(newValue);
                            });
                        }
                    });
                    this.displayItems = [{ selected: this.isSelectedAll, value: null, displayValue: '(Select All)', toggleSelected: function () {
                        _this.isSelectedAll(!_this.isSelectedAll());
                    } }].concat(this._items);
                    this.updateValue = function () {
                        value(_this._items.filter(function (item) {
                            return item.selected();
                        }).map(function (item) {
                            return item.value;
                        }));
                    };
                }
                return MultiValuesHelper;
            })();
            Widgets.MultiValuesHelper = MultiValuesHelper;
        })(Widgets = Designer.Widgets || (Designer.Widgets = {}));
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        var Widgets;
        (function (Widgets) {
            Widgets.editorTemplates = {
                color: { header: "dxrd-color" },
                bool: { header: "dxrd-boolean" },
                numeric: { header: "dxrd-numeric" },
                date: { header: "dxrd-date" },
                borders: { header: "dxrd-borders" },
                font: { header: "dxrd-emptyHeader", content: "dxrd-objectEditorContent", editorType: Widgets.FontEditor },
                modificators: { custom: "dxrd-modificators" },
                combobox: { header: "dxrd-combobox" },
                comboboxEditable: { header: "dxrd-combobox-editable" },
                text: { header: "dxrd-text" },
                controls: { header: "dxrd-controls" },
                image: { header: "dxrd-image" },
                objecteditor: { header: "dxrd-emptyHeader", content: "dxrd-objectEditorContent", editorType: Widgets.PropertyGridEditor },
                objecteditorCustom: { custom: "dxrd-objectEditorContent", editorType: Widgets.PropertyGridEditor },
                treelist: { custom: "dxrd-treelistContent", editorType: Widgets.Editor },
                field: { header: "dxrd-field" },
                dataMember: { header: "dxrd-dataMember" },
                commonCollection: { custom: "dxrd-commonCollection" },
                filterEditor: { header: "dxrd-filterstring" },
                expressionEditor: { header: "dxrd-expressionstring" },
                multiValue: { header: "dxrd-multivalue" },
                multiValueEditable: { custom: "dxrd-multivalue-editable" }
            };
        })(Widgets = Designer.Widgets || (Designer.Widgets = {}));
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        Designer.sizeFake = [
            { propertyName: "height", displayName: "Height", editor: Designer.Widgets.editorTemplates.numeric },
            { propertyName: "width", displayName: "Width", editor: Designer.Widgets.editorTemplates.numeric }
        ];
        Designer.locationFake = [
            { propertyName: "x", displayName: "X", editor: Designer.Widgets.editorTemplates.numeric },
            { propertyName: "y", displayName: "Y", editor: Designer.Widgets.editorTemplates.numeric }
        ];
        Designer.fontInfoFake = [
            {
                propertyName: "family",
                displayName: "Font Name",
                editor: Designer.Widgets.editorTemplates.combobox,
                values: {
                    "Times New Roman": "Times New Roman",
                    "Arial": "Arial",
                    "Arial Black": "Arial Black",
                    "Comic Sans MS": "Comic Sans MS",
                    "Courier New": "Courier New",
                    "Georgia": "Georgia",
                    "Impact": "Impact",
                    "Lucida Console": "Lucida Console",
                    "Lucida Sans Unicode": "Lucida Sans Unicode",
                    "Tahoma": "Tahoma",
                    "Trebuchet MS": "Trebuchet MS",
                    "Verdana": "Verdana",
                    "MS Sans Serif": "MS Sans Serif",
                    "MS Serif": "MS Serif",
                    "Symbol": "Symbol",
                    "Webdings": "Webdings",
                    "Wingdings": "Wingdings"
                }
            },
            { propertyName: "size", displayName: "Size", editor: Designer.Widgets.editorTemplates.numeric },
            {
                propertyName: "unit",
                displayName: "Unit",
                editor: Designer.Widgets.editorTemplates.combobox,
                values: {
                    "pt": "Point",
                    "world": "World",
                    "px": "Pixel",
                    "in": "Inch",
                    "doc": "Document",
                    "mm": "Millimetr"
                }
            },
            { propertyName: "modificators", editor: Designer.Widgets.editorTemplates.modificators },
        ];
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        var SurfaceSelection = (function () {
            function SurfaceSelection() {
                var _this = this;
                this._focused = ko.observable(null);
                this._selectedControls = ko.observableArray();
                this.focused = ko.computed({
                    read: function () {
                        return _this._focused();
                    },
                    write: function (val) {
                        if (!!val) {
                            _this._firstSelected = val;
                        }
                        _this.updateSelection(_this._firstSelected);
                    }
                });
                this.dropTarget = null;
                this.expectClick = false;
            }
            SurfaceSelection.prototype._removeFromSelection = function (control) {
                control.focused(false);
                control.selected(false);
                if (this._selectedControls.indexOf(control) !== -1) {
                    this._selectedControls.splice(this._selectedControls.indexOf(control), 1);
                }
            };
            SurfaceSelection.prototype._setFocused = function (control) {
                if (this._focused()) {
                    this._removeFromSelection(this._focused());
                }
                this._focused(control);
                if (control) {
                    control.focused(true);
                    if (this._selectedControls.indexOf(control) === -1) {
                        this._selectedControls.push(control);
                    }
                    control.selected(true);
                }
            };
            SurfaceSelection.prototype._resetTabPanelFocus = function () {
                var isTabPanelFocused = document.activeElement && $(document.activeElement).closest(".dxrd-surface").length === 0;
                if (isTabPanelFocused) {
                    document.activeElement["blur"]();
                }
            };
            Object.defineProperty(SurfaceSelection.prototype, "selectedItems", {
                get: function () {
                    return this._selectedControls();
                },
                enumerable: true,
                configurable: true
            });
            SurfaceSelection.prototype.updateSelection = function (control) {
                this._selectedControls().forEach(function (selectedControl) {
                    selectedControl.focused(false);
                    selectedControl.selected(false);
                });
                this._selectedControls([]);
                this._setFocused(control);
            };
            SurfaceSelection.prototype.swapFocusedItem = function (control) {
                if (this._focused() !== control) {
                    this._focused().focused(false);
                    this._focused(control);
                    this._focused().focused(true);
                }
            };
            SurfaceSelection.prototype.initialize = function (control) {
                control = control || this.dropTarget;
                this._firstSelected = !!(control && control["focused"]) ? control : null;
                if (this._firstSelected) {
                    this.updateSelection(this._firstSelected);
                }
                else {
                    this.updateSelection(null);
                }
            };
            SurfaceSelection.prototype.clickHandler = function (control, event) {
                if (event === void 0) { event = { ctrlKey: false }; }
                if (this.expectClick) {
                    this.expectClick = false;
                    return;
                }
                control = control || this.dropTarget;
                if (!event.ctrlKey) {
                    if (this._selectedControls().length > 1 && this._selectedControls.indexOf(control) !== -1) {
                        this.swapFocusedItem(control);
                    }
                    else {
                        if (this._focused() !== control) {
                            this.initialize(control);
                        }
                    }
                }
                else {
                    this.selectionWithCtrl(control);
                }
                this._resetTabPanelFocus();
            };
            SurfaceSelection.prototype.selecting = function (event) {
                if (!this._focused()) {
                    this._setFocused(event.control);
                }
                else {
                    event.cancel = !event.control.checkParent(this._firstSelected);
                    if (!event.cancel) {
                        if (this._firstSelected && this._firstSelected.focused()) {
                            this._setFocused(event.control);
                        }
                        else if (this._selectedControls.indexOf(event.control) === -1) {
                            event.control.selected(true);
                            this._selectedControls.push(event.control);
                        }
                    }
                }
            };
            SurfaceSelection.prototype.unselecting = function (control) {
                if (this._focused() === control) {
                    this._setFocused(null);
                    if (this._selectedControls().length === 0) {
                        this._setFocused(this._firstSelected);
                    }
                    else {
                        this._setFocused(this._selectedControls()[0]);
                    }
                }
                else {
                    this._removeFromSelection(control);
                }
            };
            SurfaceSelection.prototype.selectionWithCtrl = function (control) {
                if (control && control.allowMultiselect) {
                    var selectedControls = this._selectedControls();
                    if (selectedControls.length === 0 || (selectedControls.length === 1 && (!selectedControls[0].allowMultiselect))) {
                        this.initialize(control);
                    }
                    else {
                        if (this._selectedControls.indexOf(control) === -1) {
                            control.selected(true);
                            this._selectedControls.push(control);
                        }
                        else {
                            if (this._selectedControls().length > 1) {
                                this.unselecting(control);
                            }
                        }
                    }
                }
            };
            return SurfaceSelection;
        })();
        Designer.SurfaceSelection = SurfaceSelection;
        var CombinedObject = (function () {
            function CombinedObject() {
            }
            CombinedObject.collectProperties = function (controls, allProperties) {
                controls.forEach(function (control) {
                    for (var propertyName in control) {
                        var property = control[propertyName];
                        if (ko.isObservable(property) && !property["push"]) {
                            allProperties[propertyName] = allProperties[propertyName] || { properties: [] };
                            allProperties[propertyName].properties.push(property);
                        }
                        else if ($.isPlainObject(property) && !allProperties[propertyName]) {
                            allProperties[propertyName] = allProperties[propertyName] || { object: {} };
                            if (controls.filter(function (item) {
                                return item[propertyName];
                            }).length === controls.length) {
                                CombinedObject.collectProperties(controls.map(function (item) {
                                    return item[propertyName];
                                }), allProperties[propertyName].object);
                            }
                        }
                    }
                });
            };
            CombinedObject.generateMergedObject = function (object, allProperties, controlsCount, undoEngine) {
                var isAdded = false;
                $.each(allProperties, function (propertyName, properties) {
                    if (properties.object) {
                        var subObject = {};
                        if (CombinedObject.generateMergedObject(subObject, properties.object, controlsCount, undoEngine)) {
                            isAdded = true;
                            object[propertyName] = subObject;
                        }
                    }
                    else if (properties.properties && properties.properties.length === controlsCount) {
                        isAdded = true;
                        var firstValue = properties.properties[0].peek();
                        object[propertyName] = ko.observable(properties.properties.every(function (property) {
                            return firstValue === property.peek();
                        }) ? firstValue : null);
                        object[propertyName].subscribe(function (val) {
                            undoEngine && undoEngine.start();
                            properties.properties.forEach(function (property) {
                                property(val);
                            });
                            undoEngine && undoEngine.end();
                        });
                    }
                });
                return isAdded;
            };
            CombinedObject.mergeControls = function (controls, undoEngine) {
                var allProperties = {}, result = {};
                CombinedObject.collectProperties(controls, allProperties);
                CombinedObject.generateMergedObject(result, allProperties, controls.length, undoEngine);
                $.extend(result, { controlType: "multiselect", displayName: ko.observable("") });
                return result;
            };
            CombinedObject.getEditableObject = function (selectionProvider, undoEngine) {
                var editableObject = ko.observable(null);
                return ko.computed({
                    read: function () {
                        if (selectionProvider.selectedItems.length > 1) {
                            return CombinedObject.mergeControls(selectionProvider.selectedItems.map(function (item) {
                                return item.getControlModel();
                            }), undoEngine);
                        }
                        else if (editableObject()) {
                            return editableObject();
                        }
                        else {
                            return selectionProvider.focused() && selectionProvider.focused().getControlModel();
                        }
                    },
                    write: function (val) {
                        if (val && val.surface) {
                            selectionProvider.focused(val.surface);
                        }
                        else {
                            editableObject(val);
                        }
                    }
                });
            };
            return CombinedObject;
        })();
        Designer.CombinedObject = CombinedObject;
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        function deserializeArray(model, creator) {
            var result = [];
            $.each(model || [], function (_, item) {
                var createdItem = creator(item);
                result.push(createdItem);
            });
            return ko.observableArray(result);
        }
        Designer.deserializeArray = deserializeArray;
        function toStringWithDelimiter(values, delimiter) {
            return (values || []).map(function (value) {
                var str = value !== undefined && value !== null ? value.toString() : "00";
                if (str.length === 1) {
                    str = "0" + str;
                }
                return str;
            }).join(delimiter);
        }
        function serializeDate(date) {
            var datePart = toStringWithDelimiter([date.getMonth() + 1, date.getDate(), date.getFullYear()], "/");
            var timePart = toStringWithDelimiter([date.getHours(), date.getMinutes(), date.getSeconds()], ":");
            return timePart === "00:00:00" ? datePart : datePart + " " + timePart;
        }
        Designer.serializeDate = serializeDate;
        var DesignerModelSerializer = (function () {
            function DesignerModelSerializer() {
                this._refTable = {};
                this._linkTable = {};
            }
            DesignerModelSerializer.prototype.linkObjects = function () {
                var _this = this;
                $.each(this._linkTable, function (index, properties) {
                    var val = _this._refTable[index];
                    if (val) {
                        $.each(properties, function (_, property) {
                            property(val);
                        });
                    }
                });
            };
            DesignerModelSerializer.prototype.deserializeProperty = function (modelPropertyInfo, model) {
                var _this = this;
                var modelValue = modelPropertyInfo.defaultVal, propertyName = modelPropertyInfo.propertyName, propName = modelPropertyInfo.modelName;
                if (!propName) {
                    return;
                }
                if (modelPropertyInfo.editor === Designer.Widgets.editorTemplates.bool) {
                    modelPropertyInfo.from = Designer.parseBool;
                }
                if (model[propName] !== undefined) {
                    modelValue = model[propName];
                }
                if (typeof modelPropertyInfo === "string") {
                    return ko.observable(modelValue);
                }
                else if (modelPropertyInfo.link) {
                    var value = ko.observable();
                    if (modelValue) {
                        var refVal = modelValue && modelValue.slice("#Ref-".length);
                        this._linkTable[refVal] = this._linkTable[refVal] || [];
                        this._linkTable[refVal].push(value);
                    }
                    return value;
                }
                else if (modelPropertyInfo.array) {
                    if (modelPropertyInfo.from) {
                        return modelPropertyInfo.from(modelValue, this);
                    }
                    else if (modelPropertyInfo.info) {
                        var result = [];
                        $.each(modelValue || [], function (_, item) {
                            var object = {};
                            _this.deserialize(object, item || {}, modelPropertyInfo.info);
                            result.push(object);
                        });
                        return ko.observableArray(result);
                    }
                }
                else if (modelPropertyInfo.from) {
                    return modelPropertyInfo.from(modelValue, this);
                }
                else if (modelPropertyInfo.info) {
                    var object = {};
                    this.deserialize(object, modelValue || {}, modelPropertyInfo.info);
                    return object;
                }
                else if (modelPropertyInfo.modelName) {
                    return ko.observable(modelValue);
                }
                else {
                    throw new Error("Invalid info '" + JSON.stringify(modelPropertyInfo) + "'");
                }
            };
            DesignerModelSerializer.prototype.deserialize = function (viewModel, model, serializationsInfo) {
                var _this = this;
                if (serializationsInfo === void 0) { serializationsInfo = null; }
                if (!model) {
                    return;
                }
                viewModel._model = $.extend({}, model);
                var serializationsInfo = viewModel.getInfo ? viewModel.getInfo() : serializationsInfo;
                var refValue = model["@Ref"];
                if (refValue) {
                    this._refTable[refValue] = viewModel;
                }
                serializationsInfo.forEach(function (modelPropertyInfo) {
                    var propertyName = modelPropertyInfo.propertyName, propName = modelPropertyInfo.modelName;
                    if (model[propName] !== undefined) {
                        delete viewModel._model[propName];
                    }
                    viewModel[propertyName] = _this.deserializeProperty(modelPropertyInfo, model);
                });
                this.linkObjects();
            };
            DesignerModelSerializer.prototype.serialize = function (viewModel, serializationsInfo, refs) {
                if (refs === void 0) { refs = null; }
                if (!serializationsInfo && !refs) {
                    return this._serialize(viewModel, null, null);
                }
                return this._serialize(viewModel, serializationsInfo, refs);
            };
            DesignerModelSerializer.prototype._serialize = function (viewModel, serializationsInfo, refs) {
                var _this = this;
                var result = $.extend({}, viewModel._model), isInitial = refs === null;
                refs = refs || { linkObjTable: [], objects: [] };
                serializationsInfo = viewModel.getInfo ? viewModel.getInfo() : serializationsInfo;
                delete result["@Ref"];
                if (viewModel["isEmpty"] && viewModel["isEmpty"]())
                    return;
                serializationsInfo.forEach(function (modelPropertyInfo) {
                    var propertyName = modelPropertyInfo.propertyName, value = ko.unwrap(viewModel["_" + propertyName] || viewModel[propertyName]), defaultVal = modelPropertyInfo.defaultVal;
                    var resultValue = {};
                    if (!modelPropertyInfo.modelName) {
                        return;
                    }
                    if ((value !== undefined && value !== null) && (($.isPlainObject(value) || !$.isEmptyObject(value)) || ($.isArray(value) && value.length > 0) || (!$.isArray(value) && !$.isPlainObject(value))) && (value !== defaultVal)) {
                        if (modelPropertyInfo.link) {
                            refs.linkObjTable.push({
                                obj: value,
                                setRef: function (index) {
                                    result[modelPropertyInfo.modelName] = "#Ref-" + index;
                                }
                            });
                        }
                        else if (modelPropertyInfo.array) {
                            resultValue = {};
                            var index = 1;
                            $.each(value, function (_, item) {
                                var info = modelPropertyInfo.info || null;
                                var item_ = _this._serialize(item, info, refs);
                                if (item_) {
                                    resultValue["Item" + index] = item_;
                                    item_["@Ref"] = (refs.objects.push(item) - 1).toString();
                                    index++;
                                }
                            });
                        }
                        else if (modelPropertyInfo.from) {
                            if (value["isEmpty"] && value["isEmpty"]()) {
                                resultValue = {};
                            }
                            else {
                                resultValue = modelPropertyInfo.toJsonObject ? modelPropertyInfo.toJsonObject(value, _this, refs) : value.toString();
                            }
                        }
                        else if (modelPropertyInfo.info || value["getInfo"]) {
                            resultValue = _this._serialize(value, modelPropertyInfo.info, refs);
                        }
                        else if (modelPropertyInfo.modelName) {
                            if (value instanceof Date) {
                                resultValue = serializeDate(value);
                            }
                            else {
                                resultValue = value;
                            }
                        }
                        else {
                            throw new Error("Invalid info '" + serializationsInfo.stringify() + "'");
                        }
                        if (($.isPlainObject(resultValue) && !$.isEmptyObject(resultValue)) || ($.isArray(resultValue) && resultValue["length"] > 0) || (!$.isArray(resultValue) && !$.isPlainObject(resultValue))) {
                            result[modelPropertyInfo.modelName] = resultValue;
                        }
                    }
                });
                if (isInitial) {
                    $.each(refs.linkObjTable, function (_, item) {
                        var refValue = refs.objects.indexOf(item.obj);
                        if (refValue !== -1) {
                            item.setRef(refValue.toString());
                        }
                    });
                }
                return result;
            };
            return DesignerModelSerializer;
        })();
        Designer.DesignerModelSerializer = DesignerModelSerializer;
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        var ActionLists = (function () {
            function ActionLists(surfaceContext, selection, undoEngine, customizeActions) {
                var _this = this;
                this.toolbarItems = [];
                this.menuItems = [];
                var copyPasteHandler = new Designer.CopyPasteHandler(selection), actions = [];
                this.keyboardHelper = new Designer.KeyboardHelper(selection, surfaceContext, copyPasteHandler, undoEngine);
                actions.push({
                    text: "Cut",
                    imageClassName: "dxrd-image-cut",
                    disabled: ko.computed(function () {
                        return !copyPasteHandler.canCopy();
                    }),
                    visible: true,
                    clickAction: function () {
                        undoEngine.start();
                        copyPasteHandler.cut();
                        undoEngine.end();
                    },
                    hotKey: "Ctrl+X"
                });
                actions.push({
                    text: "Copy",
                    imageClassName: "dxrd-image-copy",
                    disabled: ko.computed(function () {
                        return !copyPasteHandler.canCopy();
                    }),
                    visible: true,
                    clickAction: function () {
                        copyPasteHandler.copy();
                    },
                    hotKey: "Ctrl+C"
                });
                actions.push({
                    text: "Paste",
                    imageClassName: "dxrd-image-paste",
                    disabled: ko.computed(function () {
                        return !copyPasteHandler.canPaste();
                    }),
                    visible: true,
                    clickAction: function () {
                        undoEngine.start();
                        copyPasteHandler.paste();
                        undoEngine.end();
                    },
                    hotKey: "Ctrl+V"
                });
                actions.push({
                    text: "Delete",
                    imageClassName: "dxrd-image-delete",
                    disabled: ko.computed(function () {
                        if (selection.focused()) {
                            return selection.focused().getControlModel().getMetaData().isDeleteDeny;
                        }
                        else {
                            return false;
                        }
                    }),
                    visible: true,
                    clickAction: function () {
                        undoEngine.start();
                        copyPasteHandler.deleteControls();
                        undoEngine.end();
                    }
                });
                actions.push({
                    text: "Undo",
                    imageClassName: "dxrd-image-undo",
                    disabled: ko.computed(function () {
                        return !undoEngine.undoEnabled();
                    }),
                    visible: true,
                    clickAction: function () {
                        undoEngine.undo();
                    },
                    hotKey: "Ctrl+Z",
                    hasSeparator: true
                });
                actions.push({
                    text: "Redo",
                    imageClassName: "dxrd-image-redo",
                    disabled: ko.computed(function () {
                        return !undoEngine.redoEnabled();
                    }),
                    visible: true,
                    clickAction: function () {
                        undoEngine.redo();
                    },
                    hotKey: "Ctrl+Y"
                });
                actions.push({
                    text: "Zoom Out",
                    imageClassName: "dxrd-image-zoomout",
                    disabled: ko.observable(false),
                    visible: true,
                    clickAction: function () {
                        surfaceContext().zoom(surfaceContext().zoom() - 0.01);
                    },
                    hasSeparator: true
                });
                actions.push({
                    text: "Zoom 100%",
                    imageClassName: "dxrd-image-zoom",
                    disabled: ko.observable(false),
                    visible: true,
                    clickAction: function () {
                        surfaceContext().zoom(1);
                    },
                    templateName: "dxrd-zoom-select-template",
                    zoom: ko.computed({
                        read: function () {
                            return surfaceContext().zoom();
                        },
                        write: function (val) {
                            surfaceContext().zoom(val);
                        }
                    })
                });
                actions.push({
                    text: "Zoom In",
                    imageClassName: "dxrd-image-zoomin",
                    disabled: ko.observable(false),
                    visible: true,
                    clickAction: function () {
                        surfaceContext().zoom(surfaceContext().zoom() + 0.01);
                    }
                });
                if (customizeActions) {
                    customizeActions(actions);
                }
                actions.forEach(function (action) {
                    _this._registerAction(action["container"] === "menu" ? _this.menuItems : _this.toolbarItems, action);
                });
            }
            ActionLists.prototype._registerAction = function (container, action) {
                if (action["index"]) {
                    container.splice(action["index"], 0, action);
                }
                else {
                    container.push(action);
                }
            };
            ActionLists.prototype.processShortcut = function (actions, e) {
                var activeElement = $(document.activeElement);
                if (activeElement.is("textarea") || activeElement.is(":input") && (activeElement.attr("type") === "text" || activeElement.attr("type") === "number")) {
                    return;
                }
                if (!this.keyboardHelper.processShortcut(e)) {
                    $.each(actions, function (_, action) {
                        if (action.hotKey && (action.disabled && !action.disabled() || !action.disabled)) {
                            var keys = action.hotKey.split("+");
                            if (keys.indexOf("Ctrl") !== -1 && e.ctrlKey && keys[1].charCodeAt(0) === e.keyCode) {
                                action.clickAction();
                            }
                        }
                    });
                }
                else {
                    e.preventDefault();
                }
            };
            return ActionLists;
        })();
        Designer.ActionLists = ActionLists;
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        var ToolboxItem = (function () {
            function ToolboxItem(info) {
                this.info = info;
            }
            Object.defineProperty(ToolboxItem.prototype, "type", {
                get: function () {
                    return Designer.getTypeNameFromFullName(this.info["@ControlType"]);
                },
                enumerable: true,
                configurable: true
            });
            Object.defineProperty(ToolboxItem.prototype, "imageClassName", {
                get: function () {
                    return Designer.getImageClassName(this.type);
                },
                enumerable: true,
                configurable: true
            });
            Object.defineProperty(ToolboxItem.prototype, "index", {
                get: function () {
                    return this.info.index;
                },
                enumerable: true,
                configurable: true
            });
            return ToolboxItem;
        })();
        Designer.ToolboxItem = ToolboxItem;
        function getToolboxItems(controlsMap) {
            var toolboxItems = [];
            $.each(controlsMap, function (controlType, propertyValue) {
                if (!propertyValue.nonToolboxItem) {
                    var item = {
                        "@ControlType": controlType,
                        index: propertyValue.toolboxIndex || 0
                    };
                    if (propertyValue.size) {
                        item["size"] = propertyValue.size;
                    }
                    if (propertyValue.defaultVal) {
                        $.each(propertyValue.defaultVal, function (name, value) {
                            item[name] = value;
                        });
                    }
                    toolboxItems.push(new ToolboxItem(item));
                }
            });
            return toolboxItems.sort(function (item1, item2) {
                return item1.index - item2.index;
            });
        }
        Designer.getToolboxItems = getToolboxItems;
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        var UndoEngine = (function () {
            function UndoEngine(target) {
                var _this = this;
                this._groupObservers = [];
                this._groupPosition = -1;
                this._observers = [];
                this._subscriptions = [];
                this._visited = [];
                this._position = -1;
                this._inUndoRedo = false;
                this.redoEnabled = ko.observable(false);
                this.undoEnabled = ko.observable(false);
                this.isIngroup = -1;
                this._model = ko.unwrap(target);
                this.subscribe(this._model);
                if (ko.isSubscribable(target)) {
                    target.subscribe(function (newTargetValue) {
                        _this.reset();
                        _this._model = newTargetValue;
                        _this.subscribe(_this._model);
                    });
                }
            }
            Object.defineProperty(UndoEngine.prototype, "_modelReady", {
                get: function () {
                    return Designer.checkModelReady(this._model);
                },
                enumerable: true,
                configurable: true
            });
            UndoEngine.prototype.properyChanged = function (undoRecord) {
                if (this._inUndoRedo) {
                    return;
                }
                var currentPosition = this._position + 1;
                if (currentPosition < this._observers.length) {
                    this._observers = this._observers.splice(0, currentPosition);
                }
                this._observers.push(undoRecord);
                this._position = currentPosition;
                this.undoEnabled(true);
                this.redoEnabled(false);
            };
            UndoEngine.prototype.undoChangeSet = function (changeSet) {
                if (changeSet.propertyChanged) {
                    changeSet.observable(changeSet.propertyChanged.oldVal);
                }
                else {
                    var array = changeSet.observable;
                    if (changeSet.arrayChanges.status === "added") {
                        array.splice(changeSet.arrayChanges.index, 1);
                    }
                    else if (changeSet.arrayChanges.status === "deleted") {
                        array.splice(changeSet.arrayChanges.index, 0, changeSet.arrayChanges.value);
                    }
                    else
                        Designer.NotifyAboutWarning("Unsupported array modification status: " + changeSet.arrayChanges.status);
                }
            };
            UndoEngine.prototype.redoChangeSet = function (changeSet) {
                if (changeSet.propertyChanged) {
                    changeSet.observable(changeSet.propertyChanged.val);
                }
                else {
                    var array = changeSet.observable;
                    if (changeSet.arrayChanges.status === "added") {
                        array.splice(changeSet.arrayChanges.index, 0, changeSet.arrayChanges.value);
                    }
                    else if (changeSet.arrayChanges.status === "deleted") {
                        array.splice(changeSet.arrayChanges.index, 1);
                    }
                    else
                        Designer.NotifyAboutWarning("Unsupported array modification status: " + changeSet.arrayChanges.status);
                }
            };
            UndoEngine.prototype.subscribe = function (target) {
                var _this = this;
                Designer.propertiesVisitor(target, function (properties) {
                    properties.forEach(function (property) {
                        if (ko.isObservable(property)) {
                            var prevVal = property();
                            if (property["push"]) {
                                _this._subscriptions.push(property.subscribe(function (args) {
                                    if (_this._modelReady) {
                                        if (!_this._inUndoRedo) {
                                            if (args.length > 1) {
                                                Designer.NotifyAboutWarning("Multiple array changes isn't supported");
                                            }
                                            _this.properyChanged({ observable: property, arrayChanges: args[0] });
                                            _this.subscribe(args[0].value);
                                        }
                                    }
                                }, null, "arrayChange"));
                            }
                            else {
                                if (ko.isWriteableObservable(property)) {
                                    _this._subscriptions.push(property.subscribe(function (val) {
                                        if (_this._modelReady) {
                                            _this.properyChanged({
                                                observable: property,
                                                propertyChanged: { oldVal: prevVal, val: val }
                                            });
                                            prevVal = property();
                                        }
                                    }));
                                }
                            }
                        }
                    });
                }, this._visited);
            };
            UndoEngine.prototype.reset = function () {
                this._subscriptions.forEach(function (subscription) { return subscription.dispose(); });
                this._subscriptions = [];
                this._visited = [];
                this.clearHistory();
            };
            UndoEngine.prototype.clearHistory = function () {
                this._groupObservers = [];
                this._observers = [];
                this.redoEnabled(false);
                this.undoEnabled(false);
                this._inUndoRedo = false;
                this._groupPosition = -1;
                this._position = -1;
            };
            UndoEngine.prototype.undo = function () {
                var _this = this;
                try {
                    this._inUndoRedo = true;
                    if (this.undoEnabled()) {
                        var changeSet = this._observers[this._position];
                        if ($.isArray(changeSet)) {
                            $.each(changeSet.reverse(), function (_, item) {
                                _this.undoChangeSet(item);
                            });
                        }
                        else {
                            this.undoChangeSet(changeSet);
                        }
                        this._position = this._position - 1;
                        this.undoEnabled(this._observers.length !== 0 && this._position >= 0);
                        this.redoEnabled(true);
                    }
                }
                finally {
                    this._inUndoRedo = false;
                }
            };
            UndoEngine.prototype.redo = function () {
                var _this = this;
                try {
                    this._inUndoRedo = true;
                    if (this.redoEnabled()) {
                        var changeSet = this._observers[this._position + 1];
                        if ($.isArray(changeSet)) {
                            $.each(changeSet.reverse(), function (_, item) {
                                _this.redoChangeSet(item);
                            });
                        }
                        else {
                            this.redoChangeSet(changeSet);
                        }
                        this._position = this._position + 1;
                        this.undoEnabled(this._observers.length !== 0 && this._position >= 0);
                        this.redoEnabled(this._position + 1 < this._observers.length);
                    }
                }
                finally {
                    this._inUndoRedo = false;
                }
            };
            UndoEngine.prototype.start = function () {
                this.isIngroup++;
                if (this.isIngroup !== 0)
                    return;
                this._groupObservers = this._observers;
                this._observers = [];
                this._groupPosition = this._position;
                this._position = -1;
            };
            UndoEngine.prototype.end = function () {
                this.isIngroup--;
                if (this.isIngroup !== -1) {
                    return;
                }
                this._position = this._groupPosition + 1;
                this._groupObservers.splice(this._position, this._groupObservers.length - this._position, this._observers);
                this._observers = this._groupObservers;
            };
            return UndoEngine;
        })();
        Designer.UndoEngine = UndoEngine;
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        var Widgets;
        (function (Widgets) {
            var BordersModel = (function () {
                function BordersModel(object) {
                    var _this = this;
                    var isUpdated = false;
                    this.left = ko.observable(false);
                    this.right = ko.observable(false);
                    this.top = ko.observable(false);
                    this.setAll = function () {
                        isUpdated = true;
                        _this.left(true), _this.bottom(true), _this.right(true);
                        isUpdated = false;
                        _this.top(true);
                    };
                    this.setValue = function (name) {
                        _this[name](!_this[name]());
                    };
                    this.setNone = function () {
                        isUpdated = true;
                        _this.left(false), _this.bottom(false), _this.right(false);
                        isUpdated = false;
                        _this.top(false);
                    };
                    this.bottom = ko.observable(false);
                    ko.computed(function () {
                        if (isUpdated)
                            return;
                        isUpdated = true;
                        var val = object.value() || "None";
                        var components = val.split(',');
                        if (val.indexOf("All") !== -1) {
                            _this.left(true), _this.bottom(true), _this.right(true), _this.top(true);
                        }
                        else if (val.indexOf("None") !== -1) {
                            _this.left(false), _this.bottom(false), _this.right(false), _this.top(false);
                        }
                        else {
                            _this.left(val.indexOf("Left") !== -1);
                            _this.top(val.indexOf("Top") !== -1);
                            _this.right(val.indexOf("Right") !== -1);
                            _this.bottom(val.indexOf("Bottom") !== -1);
                        }
                        isUpdated = false;
                    });
                    ko.computed(function () {
                        var result = [];
                        if (_this.left() && _this.right() && _this.top() && _this.bottom()) {
                            result.push("All");
                        }
                        else if (!_this.left() && !_this.right() && !_this.top() && !_this.bottom()) {
                            result.push("None");
                        }
                        else {
                            _this.left() ? result.push("Left") : null;
                            _this.right() ? result.push("Right") : null;
                            _this.top() ? result.push("Top") : null;
                            _this.bottom() ? result.push("Bottom") : null;
                        }
                        if (!isUpdated) {
                            object.value(result.join(','));
                        }
                    });
                }
                return BordersModel;
            })();
            Widgets.BordersModel = BordersModel;
            ko.bindingHandlers['dxBorderEditor'] = {
                init: function (element, valueAccessor) {
                    $(element).children().remove();
                    var templateHtml = $('#dxrd-bordereditor').text(), $element = $(element).append(templateHtml);
                    ko.applyBindings({ value: new BordersModel(valueAccessor()) }, $element.children()[0]);
                    return { controlsDescendantBindings: true };
                }
            };
        })(Widgets = Designer.Widgets || (Designer.Widgets = {}));
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        var Widgets;
        (function (Widgets) {
            var CollectionItemWrapper = (function () {
                function CollectionItemWrapper(editor, array, index, displayNameField) {
                    var _this = this;
                    if (displayNameField === void 0) { displayNameField = ""; }
                    this.collapsed = ko.observable(true);
                    this.selected = ko.observable(false);
                    this.value = ko.computed({
                        read: function () {
                            return array.peek()[index()];
                        },
                        write: function (val) {
                            array.peek()[index()] = val;
                        }
                    });
                    this.editor = editor;
                    this.index = index;
                    this.name = ko.computed(function () {
                        return displayNameField && _this.value() && _this.value()[displayNameField] ? ko.unwrap(_this.value()[displayNameField]) : index();
                    });
                }
                return CollectionItemWrapper;
            })();
            Widgets.CollectionItemWrapper = CollectionItemWrapper;
            var CollectionEditorViewModel = (function () {
                function CollectionEditorViewModel(options) {
                    var _this = this;
                    this.selectedItem = ko.observable(null);
                    this.alwaysShow = ko.observable(false);
                    this.collapsed = ko.observable(options.collapsed !== false);
                    var addHandler = options.addHandler || options.info && options.info() && options.info()["addHandler"];
                    var hideButtons = options.hideButtons || options.info && options.info() && options.info()["hideButtons"];
                    this.displayPropertyName = options.info && options.info() && options.info()["displayPropertyName"] || options.displayName;
                    this.showButtons = ko.computed(function () {
                        return !ko.unwrap(hideButtons) && !_this.collapsed();
                    });
                    this.padding = options.level !== void 0 ? options.level * 19 : 0;
                    this.displayName = options.displayName;
                    this.options = options;
                    if (!options.displayName) {
                        this.collapsed(false);
                        this.alwaysShow(true);
                    }
                    this.values = ko.computed(function () {
                        return ko.unwrap(options.values());
                    });
                    this.add = function (model) {
                        options.undoEngine && options.undoEngine.start();
                        options.values().push(addHandler());
                        options.undoEngine && options.undoEngine.end();
                        model.jQueryEvent.stopPropagation();
                    };
                    this.up = function (model) {
                        _this._move(options.values(), -1);
                        model.jQueryEvent.stopPropagation();
                    };
                    this.down = function (model) {
                        _this._move(options.values(), 1);
                        model.jQueryEvent.stopPropagation();
                    };
                    this.remove = function (model) {
                        if (_this.selectedItem()) {
                            options.values().splice(_this.selectedItem().index(), 1);
                            _this.selectedItem(null);
                        }
                        model.jQueryEvent.stopPropagation();
                    };
                    this.select = function (event) {
                        _this.selectedItem(event.model);
                    };
                }
                CollectionEditorViewModel.prototype._move = function (array, offset) {
                    if (this.selectedItem()) {
                        var old_index = this.selectedItem().index(), new_index = old_index + offset;
                        if ((new_index >= array().length) || (new_index < 0)) {
                            return;
                        }
                        array.splice(new_index, 0, array.splice(old_index, 1)[0]);
                    }
                };
                return CollectionEditorViewModel;
            })();
            Widgets.CollectionEditorViewModel = CollectionEditorViewModel;
            ko.bindingHandlers['dxCollectionEditor'] = {
                init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
                    var values = valueAccessor(), gridViewModel = new CollectionEditorViewModel(values), templateHtml = $(values.editorTemplate || '#dxrd-collectioneditor').text(), $templateHtml = $(templateHtml), itemTemplateName = values.info && values.info() && values.info()["template"] || values.template;
                    if (itemTemplateName) {
                        var itemTemplateHtml = $(itemTemplateName).text();
                        $templateHtml.find(".dx-collection-item").append($(itemTemplateHtml));
                    }
                    else {
                        $templateHtml.find(".dx-collection-item").append($(element).children());
                    }
                    var $element = $(element).append($templateHtml);
                    var childContext = bindingContext.createChildContext(gridViewModel);
                    ko.applyBindings(childContext, $element.children()[0]);
                    return { controlsDescendantBindings: true };
                }
            };
        })(Widgets = Designer.Widgets || (Designer.Widgets = {}));
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        var Widgets;
        (function (Widgets) {
            var PathRequest = (function () {
                function PathRequest(fullPath) {
                    this.path = "";
                    this.fullPath = fullPath;
                    if (fullPath) {
                        if (fullPath.indexOf('.') !== -1) {
                            var pathComponents = fullPath.split('.');
                            this.id = this.ref = pathComponents[0];
                            pathComponents.splice(0, 1);
                            this.path = pathComponents.join('.');
                        }
                        else {
                            this.id = this.ref = fullPath;
                        }
                    }
                }
                return PathRequest;
            })();
            Widgets.PathRequest = PathRequest;
            var TreeListItemViewModel = (function () {
                function TreeListItemViewModel(options, path, hasItems) {
                    var _this = this;
                    if (path === void 0) { path = ""; }
                    if (hasItems === void 0) { hasItems = true; }
                    this.level = -1;
                    this.hasItems = true;
                    this.items = ko.observableArray();
                    this.collapsed = ko.observable(true);
                    this.data = null;
                    this.isSelected = ko.observable(false);
                    this._path = path;
                    this.hasItems = hasItems;
                    this._treeListController = options.treeListController;
                    this.getItems = function () {
                        return _this._loadItems(options);
                    };
                    this.toggleSelected = function () {
                        if (_this._treeListController.canSelect(_this)) {
                            options.selectedPath(_this.path);
                        }
                    };
                    this.toggleCollapsed = function () {
                        if (_this.hasItems) {
                            _this.collapsed(!_this.collapsed.peek());
                            if (!_this.collapsed.peek() && _this.items().length === 0) {
                                _this._loadItems(options);
                            }
                        }
                    };
                    this.nodeImageClass = this._getNodeImageClassName();
                }
                TreeListItemViewModel.prototype._getImageClassName = function (field) {
                    return ko.computed(function () {
                        return "dxrd-image-fieldlist-" + (field.specifics || "default").toLowerCase();
                    });
                };
                TreeListItemViewModel.prototype._getNodeImageClassName = function () {
                    var _this = this;
                    var imageClassName = ko.observable("dxrd-image-collapsed");
                    return ko.computed({
                        read: function () {
                            if (!_this.hasItems) {
                                return 'dxrd-image-leaf-node';
                            }
                            return _this.collapsed() ? 'dxrd-image-collapsed' : imageClassName();
                        },
                        write: function (newValue) {
                            imageClassName(newValue);
                        }
                    });
                };
                TreeListItemViewModel.prototype._loadItems = function (options) {
                    var _this = this;
                    var deferred = $.Deferred();
                    if (this._loader) {
                        this._loader.dispose();
                    }
                    this._loader = ko.computed(function () {
                        options.itemsProvider.getItems(new PathRequest(_this.path)).done(function (data) {
                            var _data = data;
                            if (_this._treeListController.itemsFilter) {
                                _data = data.filter(function (dataItem) {
                                    return _this._treeListController.itemsFilter(dataItem, _this.path);
                                });
                            }
                            _this.items.peek().forEach(function (item) { return item.dispose(); });
                            _this.items($.map(_data, function (item) {
                                var newItem = new TreeListItemViewModel(options, _this.path, options.treeListController.hasItems(item));
                                newItem.data = item;
                                newItem.level = _this.level + 1;
                                newItem.imageClassName = _this._getImageClassName(item);
                                return newItem;
                            }));
                            _this.nodeImageClass(_this.items.peek().length > 0 ? "dxrd-image-expanded" : "dxrd-image-leaf-node");
                            deferred.resolve(_this.items.peek());
                            var selectedPath = options.selectedPath.peek();
                            if (selectedPath) {
                                var item2Select = _this.items.peek().filter(function (item) {
                                    return selectedPath.indexOf(item.path) === 0;
                                })[0];
                                if (item2Select) {
                                    _this._selectItem(item2Select.name + selectedPath.substring(item2Select.path.length));
                                }
                            }
                        });
                    });
                    return deferred.promise();
                };
                TreeListItemViewModel.prototype._selectItem = function (itemPath) {
                    var _this = this;
                    if (!this.hasItems) {
                        return;
                    }
                    var selectItemDelegate = function () {
                        _this._find(itemPath);
                        if (_this.collapsed.peek()) {
                            _this.toggleCollapsed();
                        }
                    };
                    if (this.items.peek().length === 0) {
                        this.getItems().done(function (items) {
                            selectItemDelegate();
                        });
                    }
                    else {
                        selectItemDelegate();
                    }
                };
                TreeListItemViewModel.prototype._find = function (itemPath) {
                    var pathComponents = (itemPath || "").split(".");
                    var findResult = $.grep(this.items.peek(), function (childItem) {
                        return childItem.name === pathComponents[0];
                    });
                    var item = findResult.length > 0 && findResult[0] || null;
                    if (item) {
                        if (pathComponents.length > 1) {
                            pathComponents.splice(0, 1);
                            item._selectItem(pathComponents.join("."));
                        }
                        else {
                            this._treeListController.select(item);
                        }
                    }
                };
                Object.defineProperty(TreeListItemViewModel.prototype, "name", {
                    get: function () {
                        return this.data && this.data.name;
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(TreeListItemViewModel.prototype, "path", {
                    get: function () {
                        if (this.name) {
                            return this._path === "" ? ko.unwrap(this.name) : this._path + "." + ko.unwrap(this.name);
                        }
                        else {
                            return this._path;
                        }
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(TreeListItemViewModel.prototype, "text", {
                    get: function () {
                        return this.data && this.data.displayName;
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(TreeListItemViewModel.prototype, "templateName", {
                    get: function () {
                        return this.data && this.data.templateName;
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(TreeListItemViewModel.prototype, "actions", {
                    get: function () {
                        return this._treeListController.getActions ? this._treeListController.getActions(this) : [];
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(TreeListItemViewModel.prototype, "isDraggable", {
                    get: function () {
                        if (this.data && this.data["dragData"]) {
                            return !this.data["dragData"].noDragable;
                        }
                        if (this._treeListController.isDraggable) {
                            return this._treeListController.isDraggable(this);
                        }
                        return false;
                    },
                    enumerable: true,
                    configurable: true
                });
                TreeListItemViewModel.prototype.dispose = function () {
                    if (this._loader) {
                        this._loader.dispose();
                    }
                    this.items().forEach(function (item) { return item.dispose(); });
                };
                return TreeListItemViewModel;
            })();
            Widgets.TreeListItemViewModel = TreeListItemViewModel;
            var TreeListRootItemViewModel = (function (_super) {
                __extends(TreeListRootItemViewModel, _super);
                function TreeListRootItemViewModel(options, path, hasItems) {
                    var _this = this;
                    if (path === void 0) { path = ""; }
                    if (hasItems === void 0) { hasItems = true; }
                    _super.call(this, options, path, hasItems);
                    options.selectedPath.subscribe(function (newPath) {
                        _this._selectItem(_this.path !== "" ? newPath.substr(_this.path.length + 1) : newPath);
                    });
                    this._selectItem(this.path !== "" ? this.path + "." + options.selectedPath() : options.selectedPath());
                }
                return TreeListRootItemViewModel;
            })(TreeListItemViewModel);
            Widgets.TreeListRootItemViewModel = TreeListRootItemViewModel;
            var TreeListController = (function () {
                function TreeListController() {
                    this.selectedItem = null;
                }
                TreeListController.prototype.itemsFilter = function (item) {
                    return true;
                };
                TreeListController.prototype.hasItems = function (item) {
                    return item.specifics !== "none" && (item.specifics === "List" || item.specifics === "ListSource" || item.isList === true);
                };
                TreeListController.prototype.canSelect = function (value) {
                    return !value.hasItems;
                };
                TreeListController.prototype.select = function (value) {
                    if (this.canSelect(value)) {
                        this.selectedItem && this.selectedItem.isSelected(false);
                        this.selectedItem = value;
                        value.isSelected(true);
                    }
                };
                return TreeListController;
            })();
            Widgets.TreeListController = TreeListController;
            var DataMemberTreeListController = (function () {
                function DataMemberTreeListController() {
                    this.selectedItem = null;
                    this.suppressActions = true;
                }
                DataMemberTreeListController.prototype.itemsFilter = function (item) {
                    return item.specifics !== "parameters" && (item.specifics === "List" || item.specifics === "ListSource" || item.isList === true || item.specifics === "none");
                };
                DataMemberTreeListController.prototype.hasItems = function (item) {
                    return item.specifics !== "none";
                };
                DataMemberTreeListController.prototype.canSelect = function (value) {
                    return (value.hasItems && !!value.path && value.level > 0) || value.data.specifics === "none";
                };
                DataMemberTreeListController.prototype.select = function (value) {
                    if (this.canSelect(value)) {
                        this.selectedItem && this.selectedItem.isSelected(false);
                        this.selectedItem = value;
                        value.isSelected(true);
                    }
                };
                return DataMemberTreeListController;
            })();
            Widgets.DataMemberTreeListController = DataMemberTreeListController;
            ko.bindingHandlers['treelist'] = {
                init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
                    var values = valueAccessor(), updateTreeList = function (options) {
                        options.treeListController = options.treeListController ? options.treeListController : new TreeListController();
                        var treeListViewModel = new TreeListRootItemViewModel(options, options.path), templateHtml = $('#dxrd-treelist').text() || options.templateHtml, $element = $(element).html(templateHtml);
                        var childContext = bindingContext.createChildContext(treeListViewModel.items);
                        ko.applyBindings(childContext, $element.children()[0]);
                    };
                    updateTreeList($.extend({}, ko.unwrap(values)));
                    if (ko.isSubscribable(values)) {
                        values.subscribe(function (newValue) {
                            updateTreeList(newValue);
                        });
                    }
                    return { controlsDescendantBindings: true };
                }
            };
        })(Widgets = Designer.Widgets || (Designer.Widgets = {}));
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        var Widgets;
        (function (Widgets) {
            var FilterEditorHelper = (function () {
                function FilterEditorHelper(serializer) {
                    var _this = this;
                    this.canSelectLists = true;
                    this.canCreateParameters = false;
                    this.canChoiceParameters = true;
                    this.actions = {
                        remove: function (parent) {
                            return {
                                name: "remove",
                                func: function (value) {
                                    return function () {
                                        if (parent.model.remove) {
                                            parent.model.remove(value.model);
                                            parent.operands.remove(value);
                                        }
                                        else {
                                            parent.remove(parent.model);
                                        }
                                    };
                                }
                            };
                        },
                        create: function () {
                            return {
                                name: "create",
                                func: function (value) {
                                    return function (item) {
                                        var propertyName = "", specifics = "integer";
                                        if (_this.canSelectLists) {
                                            var field = value.fields()[0];
                                            propertyName = field && field.name || propertyName;
                                            specifics = field && field.specifics.toLowerCase() || specifics;
                                        }
                                        var newModel = value.model.create(item.value, new DevExpress.Data.OperandProperty(propertyName), specifics);
                                        value.operands.push(value.createChildSurface(newModel));
                                    };
                                }
                            };
                        },
                        change: function (parent) {
                            return {
                                name: "change",
                                func: function (value) {
                                    return function (type, property) {
                                        if (parent.model && parent.model.change && parent._change) {
                                            var prop = property || value.leftPart;
                                            var rightPart = value.rightPart;
                                            var rightPartModel = rightPart ? rightPart.model || rightPart.map(function (item) {
                                                return item.model;
                                            }) : new DevExpress.Data.OperandValue("");
                                            if (prop && prop.specifics === "date" && rightPartModel instanceof DevExpress.Data.OperandValue) {
                                                rightPartModel.value = new Date(new Date().setHours(0, 0, 0, 0));
                                            }
                                            type = type || CriteriaOperatorSurface.filterEditorOperators(prop.specifics)[0];
                                            var newModel = parent.model.change(type, value.model, prop && prop.model, rightPartModel);
                                            if (newModel === null) {
                                                parent.change(type, property);
                                            }
                                            else {
                                                parent._change(value, newModel);
                                            }
                                        }
                                        else {
                                            parent.change(type, property);
                                        }
                                    };
                                }
                            };
                        },
                        changeValueType: function (parent) {
                            return {
                                name: "changeValueType",
                                func: function (value) {
                                    return function (type) {
                                        var name = "";
                                        var position = null;
                                        $.each(parent, function (propertyName, property) {
                                            if (Array.isArray(ko.unwrap(property)) && ko.isObservable(property)) {
                                                var index = ko.unwrap(property).indexOf(value);
                                                if (index > -1) {
                                                    position = index;
                                                    name = propertyName;
                                                    return;
                                                }
                                            }
                                            else if (value === ko.unwrap(property) && ko.isObservable(property)) {
                                                name = propertyName;
                                                return;
                                            }
                                        });
                                        if (type.name === "Value") {
                                            var model = parent.model.changeValue(DevExpress.Data.OperandValue, name, "", position);
                                        }
                                        else if (type.name === "Property") {
                                            var propertyName = "";
                                            if (_this.canSelectLists) {
                                                var field = value.fields()[0];
                                                propertyName = field && field.name || propertyName;
                                            }
                                            var model = parent.model.changeValue(DevExpress.Data.OperandProperty, name, propertyName, position);
                                        }
                                        else if (type.name === "Parameter") {
                                            var model = parent.model.changeValue(DevExpress.Data.OperandParameter, name, parent.parameters().length > 0 ? parent.parameters()[0].name : "", position);
                                        }
                                        if (position !== null) {
                                            parent[name].splice(position, 1, parent.createChildSurface(model));
                                        }
                                        else {
                                            parent[name](parent.createChildSurface(model));
                                        }
                                    };
                                }
                            };
                        }
                    };
                    this.handlers = {
                        create: function (criteria, popupService) {
                            return {
                                data: new FilterEditorAddOn(criteria, popupService, "create", "createItems"),
                                templateName: "dx-filtereditor-create"
                            };
                        },
                        change: function (criteria, popupService) {
                            return {
                                data: new FilterEditorAddOn(criteria, popupService, "change", "items"),
                                templateName: "dx-filtereditor-change"
                            };
                        },
                        changeProperty: function (criteria, popupService) {
                            return {
                                data: new FilterEditorAddOn(criteria, popupService, "changeProperty", "items"),
                                templateName: "dx-filtereditor-changeProperty"
                            };
                        },
                        changeValueType: function (criteria, popupService) {
                            return {
                                data: new FilterEditorAddOn(criteria, popupService, "changeValueType", "changeTypeItems"),
                                templateName: "dx-filtereditor-changeValueType"
                            };
                        },
                        changeParameter: function (criteria, popupService) {
                            return {
                                data: new FilterEditorAddOn(criteria, popupService, "changeParameter", "items"),
                                templateName: "dx-filtereditor-changeParameter"
                            };
                        }
                    };
                    this.mapper = {
                        Aggregate: function (item, parent, path, fieldListProvider, helper, actions) {
                            return new AggregateOperandSurface(item, actions || [
                                (helper || parent.helper).actions.change(parent),
                                (helper || parent.helper).actions.remove(parent),
                            ], helper || parent.helper, fieldListProvider || parent.fieldListProvider, path || parent.path);
                        },
                        Property: function (item, parent, path, fieldListProvider, helper, actions) {
                            return new OperandPropertySurface(item, actions || [
                                (helper || parent.helper).actions.changeValueType(parent)
                            ], helper || parent.helper, fieldListProvider || parent.fieldListProvider, path || parent.path);
                        },
                        Parameter: function (item, parent, path, fieldListProvider, helper, actions) {
                            return new OperandParameterSurface(item, actions || [
                                (helper || parent.helper).actions.changeValueType(parent)
                            ], helper || parent.helper, fieldListProvider || parent.fieldListProvider, path || parent.path);
                        },
                        Value: function (item, parent, path, fieldListProvider, helper, actions) {
                            var value = new OperandValueSurface(item, actions || [
                                (helper || parent.helper).actions.changeValueType(parent)
                            ], helper || parent.helper, fieldListProvider || parent.fieldListProvider, path || parent.path);
                            value.valueType(parent.specifics);
                            return value;
                        },
                        Group: function (item, parent, path, fieldListProvider, helper, actions) {
                            return new GroupOperandSurface(item, actions || [
                                (helper || parent.helper).actions.create(parent),
                                (helper || parent.helper).actions.remove(parent),
                                (helper || parent.helper).actions.change(parent)
                            ], helper || parent.helper, fieldListProvider || parent.fieldListProvider, path || parent.path);
                        },
                        Between: function (item, parent, path, fieldListProvider, helper, actions) {
                            return new BetweenOperandSurface(item, actions || [
                                (helper || parent.helper).actions.change(parent),
                                (helper || parent.helper).actions.remove(parent),
                            ], helper || parent.helper, fieldListProvider || parent.fieldListProvider, path || parent.path);
                        },
                        Binary: function (item, parent, path, fieldListProvider, helper, actions) {
                            return new BinaryOperandSurface(item, actions || [
                                (helper || parent.helper).actions.change(parent),
                                (helper || parent.helper).actions.remove(parent),
                            ], helper || parent.helper, fieldListProvider || parent.fieldListProvider, path || parent.path);
                        },
                        Function: function (item, parent, path, fieldListProvider, helper, actions) {
                            return new FunctionOperandSurface(item, actions || [
                                (helper || parent.helper).actions.change(parent),
                                (helper || parent.helper).actions.remove(parent),
                            ], helper || parent.helper, fieldListProvider || parent.fieldListProvider, path || parent.path);
                        },
                        In: function (item, parent, path, fieldListProvider, helper, actions) {
                            return new InOperandSurface(item, actions || [
                                (helper || parent.helper).actions.change(parent),
                                (helper || parent.helper).actions.remove(parent),
                            ], helper || parent.helper, fieldListProvider || parent.fieldListProvider, path || parent.path);
                        },
                        Unary: function (item, parent, path, fieldListProvider, helper, actions) {
                            return new UnaryOperandSurface(item, actions || [
                                (helper || parent.helper).actions.change(parent),
                                (helper || parent.helper).actions.remove(parent),
                            ], helper || parent.helper, fieldListProvider || parent.fieldListProvider, path || parent.path);
                        },
                        Default: function (item, parent, path, fieldListProvider, helper, actions) {
                            return new CriteriaOperatorSurface(item, actions || [
                                (helper || parent.helper).actions.change(parent),
                                (helper || parent.helper).actions.remove(parent),
                            ], helper || parent.helper, fieldListProvider || parent.fieldListProvider, path || parent.path);
                        }
                    };
                    this.serializer = serializer || new FilterEditorSerializer();
                }
                return FilterEditorHelper;
            })();
            Widgets.FilterEditorHelper = FilterEditorHelper;
            var FilterStringOptions = (function () {
                function FilterStringOptions(filterString, dataMember, dataSource) {
                    var _this = this;
                    this.itemsProvider = null;
                    this.disabled = ko.observable(false);
                    this.resetValue = function () {
                        _this.value("");
                    };
                    this.value = filterString;
                    this.path = ko.computed(function () {
                        return dataMember && dataMember().fullPath() || "";
                    });
                    this.disabled = ko.computed(function () {
                        return dataSource && dataSource() === void 0 || false;
                    });
                    this.helper = new FilterEditorHelper();
                }
                return FilterStringOptions;
            })();
            Widgets.FilterStringOptions = FilterStringOptions;
            var FilterEditorAddOn = (function () {
                function FilterEditorAddOn(criteria, popupService, action, propertyName, templateName) {
                    var _this = this;
                    this.showPopup = function (args) {
                        if (_this._popupService["subscription"]) {
                            _this._popupService["subscription"].dispose();
                        }
                        _this._popupService.title("");
                        _this.target.isSelected(true);
                        _this._updateActions(_this.target);
                        _this._popupService.target(args.element);
                        _this._popupService.visible(true);
                    };
                    this.popupContentTemplate = "dx-filtereditor-popup-common";
                    this.target = criteria;
                    this._action = action;
                    this.propertyName = propertyName;
                    this._popupService = popupService;
                    this.popupContentTemplate = templateName || this.popupContentTemplate;
                }
                FilterEditorAddOn.prototype._updateActions = function (viewModel) {
                    var _this = this;
                    this._popupService.data(null);
                    if (viewModel) {
                        this._popupService["subscription"] = this._popupService.visible.subscribe(function (newVal) {
                            _this.target.isSelected(newVal);
                        });
                        this._popupService["viewModel"] = viewModel;
                        this._popupService.data({
                            data: ko.unwrap(viewModel[this.propertyName]),
                            template: this.popupContentTemplate,
                            click: function (data) {
                                viewModel[_this._action](data);
                                _this._popupService.visible(false);
                            },
                        });
                    }
                };
                return FilterEditorAddOn;
            })();
            Widgets.FilterEditorAddOn = FilterEditorAddOn;
            var FilterEditorSerializer = (function () {
                function FilterEditorSerializer(operatorTokens) {
                    if (operatorTokens === void 0) { operatorTokens = DevExpress.Data.operatorTokens; }
                    this.operatorTokens = operatorTokens;
                }
                FilterEditorSerializer.prototype.serializeGroupOperand = function (groupOperator, reverse) {
                    var _this = this;
                    var result = groupOperator.operands.map(function (operand) {
                        if (operand instanceof DevExpress.Data.GroupOperator) {
                            return "(" + _this.serialize(operand) + ")";
                        }
                        return _this.serialize(operand);
                    }).join(' ' + (this.operatorTokens[groupOperator.displayType] || groupOperator.displayType) + ' ');
                    return reverse ? "Not(" + result + ")" : result;
                };
                FilterEditorSerializer.prototype.serializeAggregateOperand = function (aggregateOperand, reverse) {
                    var operatorTypeSuffix = aggregateOperand.operatorType === 1 /* Exists */ ? "" : "." + DevExpress.Data.Aggregate[aggregateOperand.operatorType];
                    var condition = aggregateOperand.condition ? this.serialize(aggregateOperand.condition) : "";
                    var result = this.serialize(aggregateOperand.property) + '[' + condition + ']';
                    var aggregateSuffix = aggregateOperand.operatorType !== 1 /* Exists */ ? '(' + (aggregateOperand.aggregatedExpression && this.serialize(aggregateOperand.aggregatedExpression) || "") + ')' : '';
                    return result + operatorTypeSuffix + aggregateSuffix;
                };
                FilterEditorSerializer.prototype.serializeOperandProperty = function (operandProperty) {
                    return operandProperty.displayType;
                };
                FilterEditorSerializer.prototype.serializeOperandValue = function (operandValue) {
                    var result = operandValue.value;
                    if (result && operandValue.isNumber) {
                        return result;
                    }
                    else if (result && operandValue.value instanceof Date) {
                        return "#" + Designer.serializeDate(result) + "#";
                    }
                    return result ? "'" + result + "'" : "?";
                };
                FilterEditorSerializer.prototype.serializeOperandParameter = function (operandParameter) {
                    return operandParameter.displayType;
                };
                FilterEditorSerializer.prototype.serializeBetweenOperator = function (betweenOperator, reverse) {
                    var result = this.serialize(betweenOperator.property) + " " + betweenOperator.displayType + "(" + this.serialize(betweenOperator.begin) + ", " + this.serialize(betweenOperator.end) + ")";
                    return reverse ? "Not " + result : result;
                };
                FilterEditorSerializer.prototype.serializeInOperator = function (inOperator, reverse) {
                    var _this = this;
                    var result = this.serialize(inOperator.criteriaOperator) + " " + inOperator.displayType + "(" + inOperator.operands.map(function (operand) {
                        return _this.serialize(operand);
                    }).join(', ') + ")";
                    return reverse ? "Not " + result : result;
                };
                FilterEditorSerializer.prototype.serializeBinaryOperator = function (binaryOperator, reverse) {
                    var separator = reverse ? " Not " : " ";
                    return this.serialize(binaryOperator.leftOperand) + separator + (this.operatorTokens[binaryOperator.displayType] || binaryOperator.displayType) + ' ' + this.serialize(binaryOperator.rightOperand);
                };
                FilterEditorSerializer.prototype.serializeUnaryOperator = function (unaryOperator, reverse) {
                    if (unaryOperator.operatorType === 4 /* IsNull */) {
                        var separator = reverse ? " Not " : " ";
                        return this.serialize(unaryOperator.operand) + " Is" + separator + "Null";
                    }
                    else if (unaryOperator.operatorType === 3 /* Not */) {
                        return this.serialize(unaryOperator.operand, true);
                    }
                    var result = this.serialize(unaryOperator.operand) + " " + (this.operatorTokens[unaryOperator.displayType] || unaryOperator.displayType);
                    return reverse ? "Not " + result : result;
                };
                FilterEditorSerializer.prototype.serializeFunctionOperator = function (functionOperator, reverse) {
                    var _this = this;
                    var result = (this.operatorTokens[functionOperator.displayType] || functionOperator.displayType) + '(' + functionOperator.operands.map(function (operand) {
                        return _this.serialize(operand);
                    }).join(", ") + ')';
                    return reverse ? "Not " + result : result;
                };
                FilterEditorSerializer.prototype.serialize = function (criteriaOperator, reverse) {
                    if (reverse === void 0) { reverse = false; }
                    if (criteriaOperator instanceof DevExpress.Data.AggregateOperand) {
                        return this.serializeAggregateOperand(criteriaOperator, reverse);
                    }
                    if (criteriaOperator instanceof DevExpress.Data.BetweenOperator) {
                        return this.serializeBetweenOperator(criteriaOperator, reverse);
                    }
                    if (criteriaOperator instanceof DevExpress.Data.BinaryOperator) {
                        return this.serializeBinaryOperator(criteriaOperator, reverse);
                    }
                    if (criteriaOperator instanceof DevExpress.Data.ConstantValue) {
                        return this.serializeOperandValue(criteriaOperator);
                    }
                    if (criteriaOperator instanceof DevExpress.Data.FunctionOperator) {
                        return this.serializeFunctionOperator(criteriaOperator, reverse);
                    }
                    if (criteriaOperator instanceof DevExpress.Data.GroupOperator) {
                        return this.serializeGroupOperand(criteriaOperator, reverse);
                    }
                    if (criteriaOperator instanceof DevExpress.Data.InOperator) {
                        return this.serializeInOperator(criteriaOperator, reverse);
                    }
                    if (criteriaOperator instanceof DevExpress.Data.OperandParameter) {
                        return this.serializeOperandParameter(criteriaOperator);
                    }
                    if (criteriaOperator instanceof DevExpress.Data.OperandProperty) {
                        return this.serializeOperandProperty(criteriaOperator);
                    }
                    if (criteriaOperator instanceof DevExpress.Data.OperandValue) {
                        return this.serializeOperandValue(criteriaOperator);
                    }
                    if (criteriaOperator instanceof DevExpress.Data.UnaryOperator) {
                        return this.serializeUnaryOperator(criteriaOperator, reverse);
                    }
                    throw Error("Undefined type criteria operator");
                };
                FilterEditorSerializer.prototype.deserialize = function (stringCriteria) {
                    var operand = DevExpress.Data.CriteriaOperator.parse(stringCriteria);
                    if (operand instanceof DevExpress.Data.GroupOperator) {
                        return operand;
                    }
                    else if (operand instanceof DevExpress.Data.UnaryOperator && operand.operatorType === 3 /* Not */) {
                        var child = operand["operand"];
                        if (child instanceof DevExpress.Data.GroupOperator) {
                            return operand;
                        }
                        return new DevExpress.Data.UnaryOperator(3 /* Not */, new DevExpress.Data.GroupOperator(0 /* And */, child ? [child] : []));
                    }
                    return new DevExpress.Data.GroupOperator(0 /* And */, operand ? [operand] : []);
                };
                return FilterEditorSerializer;
            })();
            Widgets.FilterEditorSerializer = FilterEditorSerializer;
            var FilterEditor = (function () {
                function FilterEditor(options, fieldListProvider) {
                    var _this = this;
                    this.isValid = ko.observable(true);
                    this.operandSurface = ko.observable(null);
                    this.operand = null;
                    this.popupVisible = ko.observable(false);
                    this.buttonItems = [];
                    this.popupService = new Designer.PopupService();
                    this.options = options;
                    this.save = function () {
                        _this.options().value(options().helper.serializer.serialize(_this.operandSurface().model, false));
                        _this.popupVisible(false);
                    };
                    this.fieldListProvider = fieldListProvider;
                    this.isValid = ko.computed(function () {
                        try {
                            _this.operand = _this.options().helper.serializer.deserialize(_this.options().value());
                            return true;
                        }
                        catch (e) {
                            return false;
                        }
                    });
                    this.popupVisible.subscribe(function (newVal) {
                        _this.operand = _this.options().helper.serializer.deserialize(_this.options().value());
                        if (newVal) {
                            var surface = null;
                            if (_this.operand instanceof DevExpress.Data.UnaryOperator) {
                                surface = _this.options().helper.mapper.Unary(_this.operand, _this);
                                delete surface["remove"];
                                delete surface.operand()["remove"];
                            }
                            else {
                                surface = _this.options().helper.mapper.Group(_this.operand, _this);
                                delete surface["remove"];
                            }
                            _this.operandSurface(surface);
                        }
                        else {
                            _this.operandSurface(null);
                        }
                    });
                    this.createAddButton = function (criteria) {
                        return options().helper.handlers.create(criteria, _this.popupService);
                    };
                    this.createChangeType = function (criteria) {
                        return options().helper.handlers.change(criteria, _this.popupService);
                    };
                    this.createChangeProperty = function (criteria) {
                        return options().helper.handlers.changeProperty(criteria, _this.popupService);
                    };
                    this.createChangeParameter = function (criteria) {
                        return options().helper.handlers.changeParameter(criteria, _this.popupService);
                    };
                    this.createChangeValueType = function (criteria) {
                        return options().helper.handlers.changeValueType(criteria, _this.popupService);
                    };
                    this._createMainPopupButtons();
                }
                FilterEditor.prototype._createMainPopupButtons = function () {
                    var self = this;
                    this.buttonItems = [
                        { toolbar: 'bottom', location: 'after', widget: 'button', options: { text: 'Save', onClick: function () {
                            self.save();
                        } } },
                        { toolbar: 'bottom', location: 'after', widget: 'button', options: { text: 'Cancel', onClick: function () {
                            self.popupVisible(false);
                        } } }
                    ];
                };
                FilterEditor.prototype.change = function (type) {
                    var model = this.operandSurface().model;
                    var rightParts = model.operands || model.operand.operands;
                    var newModel = DevExpress.Data.CriteriaOperator.create(type, null, rightParts);
                    var surface = null;
                    if (newModel instanceof DevExpress.Data.UnaryOperator) {
                        surface = this.options().helper.mapper.Unary(newModel, this);
                        delete surface["remove"];
                        delete surface.operand()["remove"];
                    }
                    else {
                        surface = this.options().helper.mapper.Group(newModel, this);
                        delete surface["remove"];
                    }
                    this.operandSurface(surface);
                };
                Object.defineProperty(FilterEditor.prototype, "helper", {
                    get: function () {
                        return this.options().helper;
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(FilterEditor.prototype, "path", {
                    get: function () {
                        return this.options().path;
                    },
                    enumerable: true,
                    configurable: true
                });
                return FilterEditor;
            })();
            Widgets.FilterEditor = FilterEditor;
            var CriteriaOperatorSurface = (function () {
                function CriteriaOperatorSurface(operator, actions, helper, fieldListProvider, currentPath) {
                    var _this = this;
                    this.templateName = "dx-filtereditor-common";
                    this.isSelected = ko.observable(false);
                    this.operatorClass = "criteria-operator-item-operator";
                    this.createItems = null;
                    this.fields = ko.observable([]);
                    this.parameters = ko.observable([]);
                    this.model = operator;
                    this.createItems = [{ name: "Add group", value: true }, { name: "Add condition", value: false }];
                    this.fieldListProvider = fieldListProvider;
                    this.path = currentPath;
                    this.helper = helper;
                    ko.computed(function () {
                        _this.fieldListProvider && ko.unwrap(_this.fieldListProvider).getItems(new Widgets.PathRequest(_this.path())).done(function (data) {
                            _this.fields(data);
                        });
                    });
                    ko.computed(function () {
                        _this.fieldListProvider && ko.unwrap(_this.fieldListProvider).getItems(new Widgets.PathRequest("parameters")).done(function (data) {
                            _this.parameters(data);
                        });
                    });
                }
                CriteriaOperatorSurface.filterEditorOperators = function (specific) {
                    if (specific === void 0) { specific = "string"; }
                    var common = [{ name: "Equal", value: 0 /* Equal */, type: DevExpress.Data.BinaryOperatorType }, { name: "Does not equal", value: 1 /* NotEqual */, type: DevExpress.Data.BinaryOperatorType }, { name: "Is greater than", value: 2 /* Greater */, type: DevExpress.Data.BinaryOperatorType }, { name: "Is greater than or equal to", value: 5 /* GreaterOrEqual */, type: DevExpress.Data.BinaryOperatorType }, { name: "Is less than", value: 3 /* Less */, type: DevExpress.Data.BinaryOperatorType }, { name: "Is less than or equal to", value: 4 /* LessOrEqual */, type: DevExpress.Data.BinaryOperatorType }, { name: "Is between", value: "Between", type: DevExpress.Data.BetweenOperator }, { name: "Is not between", value: "Between", type: DevExpress.Data.BetweenOperator, reverse: true }];
                    switch (specific) {
                        case "string":
                            return [].concat(common, [
                                { name: "Contains", value: 48 /* Contains */, type: DevExpress.Data.FunctionOperatorType },
                                { name: "Does not contains", value: 48 /* Contains */, type: DevExpress.Data.FunctionOperatorType, reverse: true },
                                { name: "Begins with", value: 46 /* StartsWith */, type: DevExpress.Data.FunctionOperatorType },
                                { name: "Ends with", value: 47 /* EndsWith */, type: DevExpress.Data.FunctionOperatorType },
                                { name: "Is like", value: 6 /* Like */, type: DevExpress.Data.BinaryOperatorType },
                                { name: "Is not like", value: 6 /* Like */, type: DevExpress.Data.BinaryOperatorType, reverse: true },
                                { name: "Is any of", value: "In", type: DevExpress.Data.InOperator },
                                { name: "Is none of", value: "In", type: DevExpress.Data.InOperator, reverse: true },
                                { name: "Is blank", value: 5 /* IsNullOrEmpty */, type: DevExpress.Data.FunctionOperatorType },
                                { name: "Is not blank", value: 5 /* IsNullOrEmpty */, type: DevExpress.Data.FunctionOperatorType, reverse: true }
                            ]);
                        case "integer":
                        case "float":
                            return [].concat(common, [
                                { name: "Is null", value: 4 /* IsNull */, type: DevExpress.Data.UnaryOperatorType },
                                { name: "Is not null", value: 4 /* IsNull */, type: DevExpress.Data.UnaryOperatorType, reverse: true },
                                { name: "Is any of", value: "In", type: DevExpress.Data.InOperator },
                                { name: "Is none of", value: "In", type: DevExpress.Data.InOperator, reverse: true },
                            ]);
                        case "date":
                            return [].concat(common, [
                                { name: "Is null", value: 4 /* IsNull */, type: DevExpress.Data.UnaryOperatorType },
                                { name: "Is not null", value: 4 /* IsNull */, type: DevExpress.Data.UnaryOperatorType, reverse: true },
                                { name: "Is any of", value: "In", type: DevExpress.Data.InOperator },
                                { name: "Is none of", value: "In", type: DevExpress.Data.InOperator, reverse: true },
                                { name: "Is beyond this year", value: 67 /* IsOutlookIntervalBeyondThisYear */, type: DevExpress.Data.FunctionOperatorType },
                                { name: "Is later this year", value: 68 /* IsOutlookIntervalLaterThisYear */, type: DevExpress.Data.FunctionOperatorType },
                                { name: "Is later this month", value: 69 /* IsOutlookIntervalLaterThisMonth */, type: DevExpress.Data.FunctionOperatorType },
                                { name: "Is next week", value: 70 /* IsOutlookIntervalNextWeek */, type: DevExpress.Data.FunctionOperatorType },
                                { name: "Is later this week", value: 71 /* IsOutlookIntervalLaterThisWeek */, type: DevExpress.Data.FunctionOperatorType },
                                { name: "Is tomorrow", value: 72 /* IsOutlookIntervalTomorrow */, type: DevExpress.Data.FunctionOperatorType },
                                { name: "Is today", value: 73 /* IsOutlookIntervalToday */, type: DevExpress.Data.FunctionOperatorType },
                                { name: "Is yesterday", value: 74 /* IsOutlookIntervalYesterday */, type: DevExpress.Data.FunctionOperatorType },
                                { name: "Is earlier this week", value: 75 /* IsOutlookIntervalEarlierThisWeek */, type: DevExpress.Data.FunctionOperatorType },
                                { name: "Is last week", value: 76 /* IsOutlookIntervalLastWeek */, type: DevExpress.Data.FunctionOperatorType },
                                { name: "Is earlier this month", value: 77 /* IsOutlookIntervalEarlierThisMonth */, type: DevExpress.Data.FunctionOperatorType },
                                { name: "Is earlier this month", value: 78 /* IsOutlookIntervalEarlierThisYear */, type: DevExpress.Data.FunctionOperatorType },
                                { name: "Is prior this year", value: 79 /* IsOutlookIntervalPriorThisYear */, type: DevExpress.Data.FunctionOperatorType },
                            ]);
                        case "list":
                            return [
                                { name: "Exists", value: 1 /* Exists */, type: DevExpress.Data.Aggregate },
                                { name: "Count", value: 0 /* Count */, type: DevExpress.Data.Aggregate },
                                { name: "Max", value: 3 /* Max */, type: DevExpress.Data.Aggregate },
                                { name: "Min", value: 2 /* Min */, type: DevExpress.Data.Aggregate },
                                { name: "Sum", value: 5 /* Sum */, type: DevExpress.Data.Aggregate },
                                { name: "Avg", value: 4 /* Avg */, type: DevExpress.Data.Aggregate }
                            ];
                        case "group":
                            return [
                                { name: "And", value: 0 /* And */, type: DevExpress.Data.GroupOperatorType },
                                { name: "Or", value: 1 /* Or */, type: DevExpress.Data.GroupOperatorType },
                                { name: "Not And", value: 0 /* And */, reverse: true, type: DevExpress.Data.GroupOperatorType },
                                { name: "Not Or", value: 1 /* Or */, reverse: true, type: DevExpress.Data.GroupOperatorType },
                            ];
                    }
                    return [].concat(common);
                };
                CriteriaOperatorSurface.prototype._createLeftPartProperty = function (value) {
                    if (value instanceof DevExpress.Data.OperandProperty) {
                        return this.createChildSurface(value, this.path, this.fieldListProvider, this.helper, [
                            this.helper.actions.change(this)
                        ]);
                    }
                    else {
                        return this.createChildSurface(value);
                    }
                };
                CriteriaOperatorSurface.prototype._createActions = function (actions, args) {
                    var _this = this;
                    actions.forEach(function (action) {
                        _this[action.name] = action.func(_this, args);
                    });
                };
                CriteriaOperatorSurface.prototype.createChildSurface = function (item, path, fieldListProvider, helper, actions) {
                    if (item instanceof DevExpress.Data.AggregateOperand) {
                        return this.helper.mapper.Aggregate(item, this, path, fieldListProvider, helper, actions);
                    }
                    else if (item instanceof DevExpress.Data.OperandProperty) {
                        return this.helper.mapper.Property(item, this, path, fieldListProvider, helper, actions);
                    }
                    else if (item instanceof DevExpress.Data.OperandParameter) {
                        return this.helper.mapper.Parameter(item, this, path, fieldListProvider, helper, actions);
                    }
                    else if (item instanceof DevExpress.Data.OperandValue) {
                        return this.helper.mapper.Value(item, this, path, fieldListProvider, helper, actions);
                    }
                    if (item instanceof DevExpress.Data.GroupOperator) {
                        return this.helper.mapper.Group(item, this, path, fieldListProvider, helper, actions);
                    }
                    else if (item instanceof DevExpress.Data.BetweenOperator) {
                        return this.helper.mapper.Between(item, this, path, fieldListProvider, helper, actions);
                    }
                    else if (item instanceof DevExpress.Data.BinaryOperator) {
                        return this.helper.mapper.Binary(item, this, path, fieldListProvider, helper, actions);
                    }
                    else if (item instanceof DevExpress.Data.FunctionOperator) {
                        return this.helper.mapper.Function(item, this, path, fieldListProvider, helper, actions);
                    }
                    else if (item instanceof DevExpress.Data.InOperator) {
                        return this.helper.mapper.In(item, this, path, fieldListProvider, helper, actions);
                    }
                    else if (item instanceof DevExpress.Data.UnaryOperator) {
                        return this.helper.mapper.Unary(item, this, path, fieldListProvider, helper, actions);
                    }
                    return this.helper.mapper.Default(item, this, path, fieldListProvider, helper, actions);
                };
                Object.defineProperty(CriteriaOperatorSurface.prototype, "specifics", {
                    get: function () {
                        if (this.leftPart) {
                            var specifics = (this.leftPart["aggregatedExpression"] && this.leftPart["aggregatedExpression"]() && this.leftPart["aggregatedExpression"]().specifics || this.leftPart.specifics).toLowerCase();
                            if (specifics.indexOf("calc") === 0) {
                                specifics = specifics.split("calc")[1];
                            }
                            return specifics === "list" ? "integer" : specifics;
                        }
                        return "integer";
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(CriteriaOperatorSurface.prototype, "items", {
                    get: function () {
                        return CriteriaOperatorSurface.filterEditorOperators(this.specifics);
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(CriteriaOperatorSurface.prototype, "displayType", {
                    get: function () {
                        var _this = this;
                        var item = this.items.filter(function (item) {
                            return _this.model.operatorType === item.value && _this.reverse === item.reverse && _this.model.enumType === item.type;
                        })[0];
                        return item && item.name || "";
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(CriteriaOperatorSurface.prototype, "leftPart", {
                    get: function () {
                        return null;
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(CriteriaOperatorSurface.prototype, "rightPart", {
                    get: function () {
                        return null;
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(CriteriaOperatorSurface.prototype, "css", {
                    get: function () {
                        return this.operatorClass + (this.isSelected() ? " selected" : "");
                    },
                    enumerable: true,
                    configurable: true
                });
                return CriteriaOperatorSurface;
            })();
            Widgets.CriteriaOperatorSurface = CriteriaOperatorSurface;
            var BinaryOperandSurface = (function (_super) {
                __extends(BinaryOperandSurface, _super);
                function BinaryOperandSurface(operator, actions, helper, fieldListProvider, path) {
                    _super.call(this, operator, actions, helper, fieldListProvider, path);
                    this.contentTemplateName = "dx-filtereditor-binary";
                    this.leftOperand = ko.observable(null);
                    this.rightOperand = ko.observable(null);
                    this.leftOperand(this._createLeftPartProperty(operator.leftOperand));
                    this.rightOperand(this.createChildSurface(operator.rightOperand));
                    this._createActions(actions, this.leftOperand().propertyName);
                }
                Object.defineProperty(BinaryOperandSurface.prototype, "leftPart", {
                    get: function () {
                        return this.leftOperand && this.leftOperand();
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(BinaryOperandSurface.prototype, "rightPart", {
                    get: function () {
                        return this.rightOperand();
                    },
                    enumerable: true,
                    configurable: true
                });
                return BinaryOperandSurface;
            })(CriteriaOperatorSurface);
            Widgets.BinaryOperandSurface = BinaryOperandSurface;
            var FunctionOperandSurface = (function (_super) {
                __extends(FunctionOperandSurface, _super);
                function FunctionOperandSurface(operator, actions, helper, fieldListProvider, path) {
                    _super.call(this, operator, actions, helper, fieldListProvider, path);
                    this.contentTemplateName = "dx-filtereditor-function";
                    this.operands = ko.observableArray([]);
                    this.operands.push(this._createLeftPartProperty(operator.operands[0]));
                    for (var i = 1; i < operator.operands.length; i++) {
                        this.operands.push(this.createChildSurface(operator.operands[i]));
                    }
                    this._createActions(actions, this.operands()[0].propertyName);
                }
                Object.defineProperty(FunctionOperandSurface.prototype, "leftPart", {
                    get: function () {
                        return this.operands && this.operands()[0];
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(FunctionOperandSurface.prototype, "rightPart", {
                    get: function () {
                        return this.operands && this.operands()[1];
                    },
                    enumerable: true,
                    configurable: true
                });
                return FunctionOperandSurface;
            })(CriteriaOperatorSurface);
            Widgets.FunctionOperandSurface = FunctionOperandSurface;
            var InOperandSurface = (function (_super) {
                __extends(InOperandSurface, _super);
                function InOperandSurface(operator, actions, helper, fieldListProvider, path) {
                    var _this = this;
                    _super.call(this, operator, actions, helper, fieldListProvider, path);
                    this.contentTemplateName = "dx-filtereditor-in";
                    this.operands = ko.observableArray([]);
                    this.criteriaOperator = ko.observable(null);
                    this.criteriaOperator(this._createLeftPartProperty(operator.criteriaOperator));
                    this.operands((operator.operands || []).map(function (operand) {
                        return _this.createChildSurface(operand);
                    }));
                    this.addValue = function () {
                        var value = new DevExpress.Data.OperandValue(null);
                        _this.model.operands.push(value);
                        _this.operands.push(_this.createChildSurface(value));
                    };
                    this._createActions(actions, this.criteriaOperator().propertyName);
                }
                Object.defineProperty(InOperandSurface.prototype, "leftPart", {
                    get: function () {
                        return this.criteriaOperator && this.criteriaOperator();
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(InOperandSurface.prototype, "rightPart", {
                    get: function () {
                        return this.operands();
                    },
                    enumerable: true,
                    configurable: true
                });
                return InOperandSurface;
            })(CriteriaOperatorSurface);
            Widgets.InOperandSurface = InOperandSurface;
            var BetweenOperandSurface = (function (_super) {
                __extends(BetweenOperandSurface, _super);
                function BetweenOperandSurface(operator, actions, helper, fieldListProvider, path) {
                    _super.call(this, operator, actions, helper, fieldListProvider, path);
                    this.property = ko.observable(null);
                    this.end = ko.observable(null);
                    this.begin = ko.observable(null);
                    this.contentTemplateName = "dx-filtereditor-between";
                    this.property(this._createLeftPartProperty(operator.property));
                    this._createActions(actions, this.property().propertyName);
                    this.begin(this.createChildSurface(operator.begin));
                    this.end(this.createChildSurface(operator.end));
                }
                Object.defineProperty(BetweenOperandSurface.prototype, "leftPart", {
                    get: function () {
                        return this.property && this.property();
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(BetweenOperandSurface.prototype, "rightPart", {
                    get: function () {
                        return [this.begin(), this.end()];
                    },
                    enumerable: true,
                    configurable: true
                });
                return BetweenOperandSurface;
            })(CriteriaOperatorSurface);
            Widgets.BetweenOperandSurface = BetweenOperandSurface;
            var OperandSurfaceBase = (function (_super) {
                __extends(OperandSurfaceBase, _super);
                function OperandSurfaceBase(operator, actions, helper, fieldListProvider, path, mapper) {
                    _super.call(this, operator, actions, helper, fieldListProvider, path);
                }
                Object.defineProperty(OperandSurfaceBase.prototype, "changeTypeItems", {
                    get: function () {
                        if (this.helper.canChoiceParameters && (this.parameters && this.parameters().length > 0 || this.helper.canCreateParameters)) {
                            return [{ name: "Property" }, { name: "Value" }, { name: "Parameter" }];
                        }
                        return [{ name: "Property" }, { name: "Value" }];
                    },
                    enumerable: true,
                    configurable: true
                });
                return OperandSurfaceBase;
            })(CriteriaOperatorSurface);
            Widgets.OperandSurfaceBase = OperandSurfaceBase;
            var OperandValueSurface = (function (_super) {
                __extends(OperandValueSurface, _super);
                function OperandValueSurface(operator, actions, helper, fieldListProvider, path, mapper) {
                    var _this = this;
                    _super.call(this, operator, actions, helper, fieldListProvider, path);
                    this._value = ko.observable(null);
                    this.valueType = ko.observable("integer");
                    this.isEditable = ko.observable(false);
                    this.templateName = "dx-filtereditor-value";
                    this._createActions(actions);
                    this._value(operator.value);
                    this._value.subscribe(function (newVal) {
                        _this.model.value = newVal;
                    });
                    this.value = ko.computed(function () {
                        var value = _this._value();
                        if (value instanceof Date) {
                            value = Designer.serializeDate(value);
                        }
                        return value || OperandValueSurface.defaultDisplay;
                    });
                }
                Object.defineProperty(OperandValueSurface.prototype, "displayType", {
                    get: function () {
                        return null;
                    },
                    enumerable: true,
                    configurable: true
                });
                OperandValueSurface.defaultDisplay = "Enter a value";
                return OperandValueSurface;
            })(OperandSurfaceBase);
            Widgets.OperandValueSurface = OperandValueSurface;
            var GroupOperandSurface = (function (_super) {
                __extends(GroupOperandSurface, _super);
                function GroupOperandSurface(operator, actions, helper, fieldListProvider, path) {
                    var _this = this;
                    _super.call(this, operator, actions, helper, fieldListProvider, path);
                    this.templateName = "dx-filtereditor-group";
                    this.operatorClass = "criteria-operator-item-group";
                    this.operands = ko.observableArray([]);
                    this.operands((operator.operands || []).map(function (operand) {
                        return _this.createChildSurface(operand);
                    }));
                    this._createActions(actions);
                    this._change = function (value, newModel) {
                        var position = _this.operands().indexOf(value);
                        _this.operands.splice(position, 1, _this.createChildSurface(newModel));
                    };
                }
                Object.defineProperty(GroupOperandSurface.prototype, "specifics", {
                    get: function () {
                        return "group";
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(GroupOperandSurface.prototype, "rightPart", {
                    get: function () {
                        return this.operands();
                    },
                    enumerable: true,
                    configurable: true
                });
                return GroupOperandSurface;
            })(CriteriaOperatorSurface);
            Widgets.GroupOperandSurface = GroupOperandSurface;
            var AggregateOperandSurface = (function (_super) {
                __extends(AggregateOperandSurface, _super);
                function AggregateOperandSurface(operator, actions, helper, fieldListProvider, path) {
                    var _this = this;
                    _super.call(this, operator, actions, helper, fieldListProvider, path);
                    this.contentTemplateName = "dx-filtereditor-aggregate";
                    this.property = ko.observable(null);
                    this.aggregatedExpression = ko.observable(null);
                    this.condition = ko.observable(null);
                    this.property(this.createChildSurface(operator.property, this.path, this.fieldListProvider, this.helper, [helper.actions.change(this)]));
                    var childPath = ko.computed(function () {
                        return _this.path() + "." + _this.property().propertyName();
                    });
                    if (operator.aggregatedExpression) {
                        operator.aggregatedExpression.propertyName = operator.aggregatedExpression.propertyName || this.fields()[0].name;
                        this.aggregatedExpression(this.createChildSurface(operator.aggregatedExpression, childPath, this.fieldListProvider, helper, []));
                        this.templateName = "dx-filtereditor-aggregate-common";
                    }
                    if (operator.operatorType === 0 /* Count */) {
                        this.templateName = "dx-filtereditor-aggregate-common";
                    }
                    var surface = this.createChildSurface(operator.condition, childPath);
                    if (surface["operand"]) {
                        delete surface["operand"]().remove;
                    }
                    delete surface["remove"];
                    this.condition(surface);
                    this._createActions(actions, this.property().propertyName);
                    this._change = function (value, newModel) {
                        var surface = _this.createChildSurface(newModel, childPath);
                        if (surface["operand"]) {
                            delete surface["operand"]().remove;
                        }
                        delete surface["remove"];
                        _this.condition(surface);
                    };
                }
                Object.defineProperty(AggregateOperandSurface.prototype, "specifics", {
                    get: function () {
                        return this.property && this.property().specifics.toLowerCase();
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(AggregateOperandSurface.prototype, "leftPart", {
                    get: function () {
                        return this.property && this.property();
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(AggregateOperandSurface.prototype, "rightPart", {
                    get: function () {
                        return this.aggregatedExpression();
                    },
                    enumerable: true,
                    configurable: true
                });
                return AggregateOperandSurface;
            })(CriteriaOperatorSurface);
            Widgets.AggregateOperandSurface = AggregateOperandSurface;
            var OperandParameterSurface = (function (_super) {
                __extends(OperandParameterSurface, _super);
                function OperandParameterSurface(operator, actions, helper, fieldListProvider, path) {
                    var _this = this;
                    _super.call(this, operator, actions, helper, fieldListProvider, path);
                    this.changeParameter = function (item) {
                        _this.model.parameterName = item.name;
                        _this.parameterName(item.name);
                    };
                    this.operatorClass = "criteria-operator-item-parameter";
                    this.valueType = ko.observable("");
                    this.parameterName = ko.observable("");
                    this.templateName = "dx-filtereditor-parameter";
                    this._createActions(actions);
                    this.parameterName(operator.parameterName);
                }
                Object.defineProperty(OperandParameterSurface.prototype, "items", {
                    get: function () {
                        return this.parameters;
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(OperandParameterSurface.prototype, "displayType", {
                    get: function () {
                        return null;
                    },
                    enumerable: true,
                    configurable: true
                });
                return OperandParameterSurface;
            })(OperandSurfaceBase);
            Widgets.OperandParameterSurface = OperandParameterSurface;
            var UnaryOperandSurface = (function (_super) {
                __extends(UnaryOperandSurface, _super);
                function UnaryOperandSurface(operator, actions, helper, fieldListProvider, path) {
                    _super.call(this, operator, actions, helper, fieldListProvider, path);
                    this.contentTemplateName = "dx-filtereditor-unary";
                    this.operand = ko.observable(null);
                    var operand = this.createChildSurface(operator.operand);
                    if (operator.operatorType === 3 /* Not */) {
                        this.templateName = "dx-filtereditor-not";
                        operand.reverse = true;
                    }
                    else {
                        operand = this._createLeftPartProperty(operator.operand);
                    }
                    this.operand(operand);
                    this._createActions(actions, this.operand().propertyName);
                }
                Object.defineProperty(UnaryOperandSurface.prototype, "leftPart", {
                    get: function () {
                        var operand = this.operand();
                        return operand && operand.reverse ? operand.leftPart : operand;
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(UnaryOperandSurface.prototype, "rightPart", {
                    get: function () {
                        var operand = this.operand();
                        return operand && operand.reverse ? operand.rightPart : null;
                    },
                    enumerable: true,
                    configurable: true
                });
                return UnaryOperandSurface;
            })(CriteriaOperatorSurface);
            Widgets.UnaryOperandSurface = UnaryOperandSurface;
            var OperandPropertySurface = (function (_super) {
                __extends(OperandPropertySurface, _super);
                function OperandPropertySurface(operator, actions, helper, fieldListProvider, path) {
                    var _this = this;
                    _super.call(this, operator, actions, helper, fieldListProvider, path);
                    this.propertyName = ko.observable("");
                    this.field = ko.computed({
                        read: function () {
                            return _this.fields().filter(function (item) {
                                return item.name === _this.propertyName();
                            })[0] || null;
                        },
                        write: function (newVal) {
                            var isChange = _this.specifics !== newVal.specifics;
                            _this.propertyName(newVal.name);
                            _this.model.propertyName = newVal.name;
                            if (isChange) {
                                _this["change"] && _this["change"](null, _this);
                            }
                        }
                    });
                    this.valueType = ko.observable("");
                    this.changeProperty = function (item) {
                        _this.field(item);
                    };
                    this.templateName = "dx-filtereditor-property";
                    this.operatorClass = "criteria-operator-item-field";
                    this._createActions(actions);
                    var self = this;
                    this.propertyName(operator.propertyName);
                }
                Object.defineProperty(OperandPropertySurface.prototype, "specifics", {
                    get: function () {
                        return this.field && this.field() && this.field().specifics.toLowerCase() || "integer";
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(OperandPropertySurface.prototype, "items", {
                    get: function () {
                        return this.fields;
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(OperandPropertySurface.prototype, "displayType", {
                    get: function () {
                        return null;
                    },
                    enumerable: true,
                    configurable: true
                });
                return OperandPropertySurface;
            })(OperandSurfaceBase);
            Widgets.OperandPropertySurface = OperandPropertySurface;
            ko.bindingHandlers['dxFilterEditor'] = {
                init: function (element, valueAccessor) {
                    $(element).children().remove();
                    $(element).addClass("dx-filtereditor");
                    var templateHtml = $('#dxrd-filtereditor').text(), $element = $(element).append(templateHtml), values = valueAccessor();
                    var itemsProvider = ko.observable(ko.unwrap(values.fieldListProvider));
                    ko.computed(function () {
                        if (values.options().itemsProvider) {
                            itemsProvider(ko.unwrap(values.options().itemsProvider));
                        }
                        else {
                            itemsProvider(ko.unwrap(values.fieldListProvider));
                        }
                    });
                    ko.applyBindings(new FilterEditor(values.options, itemsProvider), $element.children()[0]);
                    return { controlsDescendantBindings: true };
                }
            };
        })(Widgets = Designer.Widgets || (Designer.Widgets = {}));
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        var Widgets;
        (function (Widgets) {
            var operatorNames = [
                { text: "+", image: "addition", description: "Adds the value of one numeric expression to another or concatenates two strings." },
                { text: "-", image: "subtraction", description: "Finds the difference between two numbers." },
                { text: "*", image: "multiplication", description: "Multiplies the value of two expressions." },
                { text: "/", image: "division", description: "Divides the first operand by the second." },
                { text: "%", image: "modulus", hasSeparator: true, description: "Returns the remainder (modulus) obtained by dividing one numeric expression into another." },
                { text: "()", image: "parenthesis", hasSeparator: true },
                { text: "|", description: "Compares each bit of its first operand to the corresponding bit of its second operand. If either bit is 1, the corresponding result bit is set to 1. Otherwise, the corresponding result bit is set to 0." },
                { text: "&", description: "Performs a bitwise logical AND operation between two integer values." },
                { text: "^", description: "Performs a logical exclusion on two Boolean expressions, or a bitwise exclusion on two numeric expressions." },
                { text: "==", image: "equal", description: "Returns true if both operands have the same value; otherwise, it returns false." },
                { text: "!=", image: "not_equal", description: "Returns true if the operands do not have the same value; otherwise, it returns false." },
                { text: "<", image: "less", description: "Less than operator. Used to compare expressions." },
                { text: "<=", image: "less_or_equal", description: "Less than or equal to operator. Used to compare expressions." },
                { text: ">=", image: "greater_or_equal", description: "Greater than or equal to operator. Used to compare expressions." },
                { text: ">", hasSeparator: true, image: "greater", description: "Greater than operator. Used to compare expressions." },
                { text: "In", description: "In (,,,)    Tests for the existence of a property in an object." },
                { text: "Like", description: "Compares a string against a pattern. If the value of the string matches the pattern, then the result is true. If the string does not match the pattern, the result is false. If both string and pattern are empty strings, the result is true." },
                { text: "Between", description: "Between (,)    Specifies a range to test. Returns true if a value is greater than or equal to the first operand and less than or equal to the second operand." },
                { text: "And", image: "and", description: "Performs a logical conjunction on two expressions." },
                { text: "Or", image: "or", description: "Performs a logical disjunction on two Boolean expressions." },
                { text: "Not", image: "not", description: "Performs logical negation on an expression." }
            ];
            Widgets.functionDisplay = {
                None: null,
                Custom: null,
                CustomNonDeterministic: null,
                Iif: [{ paramCount: 3, text: "Iif(, , )" }],
                IsNull: [{ paramCount: 1, text: "IsNull()" }],
                IsNullOrEmpty: [{ paramCount: 1, text: "IsNullOrEmpty()" }],
                Trim: [{ paramCount: 1, text: "Trim()" }],
                Len: [{ paramCount: 1, text: "Len()" }],
                Substring: [
                    { paramCount: 3, text: "Substring('', , )" },
                    { paramCount: 2, text: "Substring('', )" }
                ],
                Upper: [{ paramCount: 1, text: "Upper()" }],
                Lower: [{ paramCount: 1, text: "Lower()" }],
                Concat: [{ paramCount: Infinity, text: "Concat(, )" }],
                Ascii: [{ paramCount: 1, text: "Ascii('')" }],
                Char: [{ paramCount: 1, text: "Char()" }],
                ToStr: [{ paramCount: 1, text: "ToStr()" }],
                Replace: [{ paramCount: 3, text: "Replace('','', '')" }],
                Reverse: [{ paramCount: 1, text: "Reverse('')" }],
                Insert: [{ paramCount: 3, text: "Insert('', , '')" }],
                CharIndex: [
                    { paramCount: 2, text: "CharIndex('','')" },
                    { paramCount: 3, text: "CharIndex('','', )" }
                ],
                Remove: [{ paramCount: 3, text: "Remove('', , )" }],
                Abs: [{ paramCount: 1, text: "Abs()" }],
                Sqr: [{ paramCount: 1, text: "Sqr()" }],
                Cos: [{ paramCount: 1, text: "Cos()" }],
                Sin: [{ paramCount: 1, text: "Sin()" }],
                Atn: [{ paramCount: 1, text: "Atn()" }],
                Exp: [{ paramCount: 1, text: "Exp()" }],
                Log: [
                    { paramCount: 1, text: "Log()" },
                    { paramCount: 2, text: "Log(, )" },
                ],
                Rnd: [{ paramCount: 0, text: "Rnd()" }],
                Tan: [{ paramCount: 1, text: "Tan()" }],
                Power: [{ paramCount: 2, text: "Power(, )" }],
                Sign: [{ paramCount: 1, text: "Sign()" }],
                Round: [
                    { paramCount: 1, text: "Round()" },
                    { paramCount: 2, text: "Round(, )" },
                ],
                Ceiling: [{ paramCount: 1, text: "Ceiling()" }],
                Floor: [{ paramCount: 1, text: "Floor()" }],
                Max: [{ paramCount: 2, text: "Max(, )" }],
                Min: [{ paramCount: 2, text: "Min(, )" }],
                Acos: [{ paramCount: 1, text: "Acos()" }],
                Asin: [{ paramCount: 1, text: "Asin()" }],
                Atn2: [{ paramCount: 2, text: "Atn2(, )" }],
                BigMul: [{ paramCount: 2, text: "BigMul(, )" }],
                Cosh: [{ paramCount: 1, text: "Cosh()" }],
                Log10: [{ paramCount: 1, text: "Log10()" }],
                Sinh: [{ paramCount: 1, text: "Sinh()" }],
                Tanh: [{ paramCount: 1, text: "Tanh()" }],
                PadLeft: [
                    { paramCount: 2, text: "PadLeft(, )" },
                    { paramCount: 3, text: "PadLeft(, , '')" }
                ],
                PadRight: [
                    { paramCount: 2, text: "PadRight(, )" },
                    { paramCount: 3, text: "PadRight(, , '')" }
                ],
                StartsWith: [{ paramCount: 2, text: "StartsWith('', '')" }],
                EndsWith: [{ paramCount: 2, text: "EndsWith('', '')" }],
                Contains: [{ paramCount: 0, text: "Contains('', '')" }],
                ToInt: [{ paramCount: 1, text: "ToInt()" }],
                ToLong: [{ paramCount: 1, text: "ToLong()" }],
                ToFloat: [{ paramCount: 1, text: "ToFloat()" }],
                ToDouble: [{ paramCount: 1, text: "ToDouble()" }],
                ToDecimal: [{ paramCount: 1, text: "ToDecimal()" }],
                LocalDateTimeThisYear: [{ paramCount: 0, text: "LocalDateTimeThisYear()" }],
                LocalDateTimeThisMonth: [{ paramCount: 0, text: "LocalDateTimeThisMonth()" }],
                LocalDateTimeLastWeek: [{ paramCount: 0, text: "LocalDateTimeLastWeek()" }],
                LocalDateTimeThisWeek: [{ paramCount: 0, text: "LocalDateTimeThisWeek()" }],
                LocalDateTimeYesterday: [{ paramCount: 0, text: "LocalDateTimeYesterday()" }],
                LocalDateTimeToday: [{ paramCount: 0, text: "LocalDateTimeToday()" }],
                LocalDateTimeNow: [{ paramCount: 0, text: "LocalDateTimeNow()" }],
                LocalDateTimeTomorrow: [{ paramCount: 0, text: "LocalDateTimeTomorrow()" }],
                LocalDateTimeDayAfterTomorrow: [{ paramCount: 0, text: "LocalDateTimeDayAfterTomorrow()" }],
                LocalDateTimeNextWeek: [{ paramCount: 0, text: "LocalDateTimeNextWeek()" }],
                LocalDateTimeTwoWeeksAway: [{ paramCount: 0, text: "LocalDateTimeTwoWeeksAway()" }],
                LocalDateTimeNextMonth: [{ paramCount: 0, text: "LocalDateTimeNextMonth()" }],
                LocalDateTimeNextYear: [{ paramCount: 0, text: "LocalDateTimeNextYear()" }],
                IsOutlookIntervalBeyondThisYear: null,
                IsOutlookIntervalLaterThisYear: null,
                IsOutlookIntervalLaterThisMonth: null,
                IsOutlookIntervalNextWeek: null,
                IsOutlookIntervalLaterThisWeek: null,
                IsOutlookIntervalTomorrow: null,
                IsOutlookIntervalToday: null,
                IsOutlookIntervalYesterday: null,
                IsOutlookIntervalEarlierThisWeek: null,
                IsOutlookIntervalLastWeek: null,
                IsOutlookIntervalEarlierThisMonth: null,
                IsOutlookIntervalEarlierThisYear: null,
                IsOutlookIntervalPriorThisYear: null,
                IsThisWeek: [{ paramCount: 1, text: "IsThisWeek()" }],
                IsThisMonth: [{ paramCount: 1, text: "IsThisMonth()" }],
                IsThisYear: [{ paramCount: 1, text: "IsThisYear()" }],
                DateDiffTick: [{ paramCount: 2, text: "DateDiffTick(, )" }],
                DateDiffSecond: [{ paramCount: 2, text: "DateDiffSecond(, )" }],
                DateDiffMilliSecond: [{ paramCount: 2, text: "DateDiffMilliSecond(, )" }],
                DateDiffMinute: [{ paramCount: 2, text: "DateDiffMinute(, )" }],
                DateDiffHour: [{ paramCount: 2, text: "DateDiffHour(, )" }],
                DateDiffDay: [{ paramCount: 2, text: "DateDiffDay(, )" }],
                DateDiffMonth: [{ paramCount: 2, text: "DateDiffMonth(, )" }],
                DateDiffYear: [{ paramCount: 2, text: "DateDiffYear(, )" }],
                GetDate: [{ paramCount: 1, text: "GetDate()" }],
                GetMilliSecond: [{ paramCount: 1, text: "GetMilliSecond()" }],
                GetSecond: [{ paramCount: 1, text: "GetSecond()" }],
                GetMinute: [{ paramCount: 1, text: "GetMinute()" }],
                GetHour: [{ paramCount: 1, text: "GetHour()" }],
                GetDay: [{ paramCount: 1, text: "GetDay()" }],
                GetMonth: [{ paramCount: 1, text: "GetMonth()" }],
                GetYear: [{ paramCount: 1, text: "GetYear()" }],
                GetDayOfWeek: [{ paramCount: 1, text: "GetDayOfWeek()" }],
                GetDayOfYear: [{ paramCount: 1, text: "GetDayOfYear()" }],
                GetTimeOfDay: [{ paramCount: 1, text: "GetTimeOfDay()" }],
                Now: [{ paramCount: 0, text: "Now()" }],
                UtcNow: [{ paramCount: 0, text: "UtcNow()" }],
                Today: [{ paramCount: 0, text: "Today()" }],
                AddTimeSpan: [{ paramCount: 2, text: "AddTimeSpan(, )" }],
                AddTicks: [{ paramCount: 2, text: "AddTicks(, )" }],
                AddMilliSeconds: [{ paramCount: 2, text: "AddMilliSeconds(, )" }],
                AddSeconds: [{ paramCount: 2, text: "AddSeconds(, )" }],
                AddMinutes: [{ paramCount: 2, text: "AddMinutes(, )" }],
                AddHours: [{ paramCount: 2, text: "AddHours(, )" }],
                AddDays: [{ paramCount: 2, text: "AddDays(, )" }],
                AddMonths: [{ paramCount: 2, text: "AddMonths(, )" }],
                AddYears: [{ paramCount: 2, text: "AddYears(, )" }],
                OrderDescToken: null
            };
            var Tools = (function () {
                function Tools(value, parametersOptions, fieldListOptions) {
                    var _this = this;
                    this.popularItems = [];
                    this.toolBox = [];
                    this.description = ko.observable();
                    this.value = value;
                    this._defaultClick = function (item) {
                        if (_this.value()[_this.value().length - 1] === " ") {
                            _this.value(_this.value() + (item.text || item) + " ");
                        }
                        else {
                            _this.value(_this.value() + " " + (item.text || item) + " ");
                        }
                    };
                    this.popularItems = this._generatePopularItems(operatorNames.filter(function (item) {
                        return !!item.image;
                    }));
                    this.toolBox = [
                        this._generateList("Functions", $.map(Widgets.functionDisplay, function (value) {
                            if (value) {
                                var result = [];
                                value.forEach(function (item) {
                                    result.push(item);
                                });
                                return result;
                            }
                        })),
                        this._generateList("Operators", operatorNames.filter(function (item) {
                            return !!item.description;
                        }))
                    ];
                    this.toolBox.push(this._generateList("Fields", { fields: fieldListOptions, parameters: parametersOptions }, "dxrd-expressioneditor-fields", "37%"));
                }
                Tools.prototype._generateList = function (title, content, templateName, width, click) {
                    var _this = this;
                    if (templateName === void 0) { templateName = null; }
                    return {
                        templateName: templateName,
                        width: width || "30%",
                        title: title,
                        content: content,
                        click: click || this._defaultClick,
                        selection: function (item) {
                            _this.description(item.description || item.text);
                        }
                    };
                };
                Tools.prototype._generatePopularItems = function (values, click) {
                    var _this = this;
                    return values.map(function (item) {
                        return {
                            templateName: item.templateName || null,
                            text: item.text || item,
                            imgClassName: "dxrd-image-expressioneditor-" + item.image,
                            hasSeparator: item.hasSeparator,
                            description: item.description,
                            click: click || _this._defaultClick
                        };
                    });
                };
                return Tools;
            })();
            Widgets.Tools = Tools;
            var ExpressionEditorTreeListController = (function (_super) {
                __extends(ExpressionEditorTreeListController, _super);
                function ExpressionEditorTreeListController(value, fullPath, calculatedFieldName, selectedPath) {
                    _super.call(this);
                    this.selectedPath = null;
                    this.value = value;
                    this.path = fullPath;
                    this.fiedlName = calculatedFieldName;
                    this.selectedPath = selectedPath;
                }
                ExpressionEditorTreeListController.prototype.itemsFilter = function (item) {
                    return item.specifics !== "none" && item.name !== ko.unwrap(this.fiedlName);
                };
                ExpressionEditorTreeListController.prototype.select = function (value) {
                    this.value(this.value() + '[' + (this.path() !== "" ? value.path.substring(this.path().length + 1) : value.path) + ']');
                    this.selectedPath("");
                };
                ExpressionEditorTreeListController.prototype.canSelect = function (value) {
                    return true;
                };
                return ExpressionEditorTreeListController;
            })(Widgets.TreeListController);
            Widgets.ExpressionEditorTreeListController = ExpressionEditorTreeListController;
            var ExpressionEditor = (function () {
                function ExpressionEditor(options, fieldListProvider) {
                    var _this = this;
                    this.popupVisible = ko.observable(false);
                    this.value = ko.observable("");
                    this.textAreaValue = ko.observable("");
                    this.isValid = ko.observable(true);
                    this.buttonItems = [];
                    this.value = options().value;
                    this.popupVisible.subscribe(function (newVal) {
                        _this.textAreaValue(_this.value());
                    });
                    this.fieldListProvider = ko.unwrap(fieldListProvider);
                    var self = this;
                    this.save = function (sender) {
                        try {
                            DevExpress.Data.CriteriaOperator.parse(_this.textAreaValue());
                            options().value(_this.textAreaValue());
                            _this.popupVisible(false);
                        }
                        catch (exception) {
                            var result = DevExpress.Data.CriteriaOperator.getNotValidRange(_this.textAreaValue(), exception.message);
                            sender.element.parents(".dx-expressioneditor").find(":input")[0].setSelectionRange(result.start, result.end);
                            _this.isValid(false);
                        }
                    };
                    var parametersOptions = ko.observable(null);
                    ko.computed(function () {
                        if (options()) {
                            var selectedPath = ko.observable("");
                            parametersOptions({
                                itemsProvider: _this.fieldListProvider,
                                selectedPath: selectedPath,
                                path: "parameters",
                                treeListController: new ExpressionEditorTreeListController(_this.textAreaValue, ko.observable(""), "", selectedPath)
                            });
                        }
                    });
                    if (options().path) {
                        var treeListOptions = ko.observable(null);
                        this.tools = new Tools(this.textAreaValue, parametersOptions, treeListOptions);
                        ko.computed(function () {
                            if (options()) {
                                var selectedPath = ko.observable("");
                                treeListOptions({
                                    itemsProvider: _this.fieldListProvider,
                                    path: options().path(),
                                    selectedPath: selectedPath,
                                    treeListController: new ExpressionEditorTreeListController(_this.textAreaValue, options().path, options().fieldName, selectedPath)
                                });
                            }
                        });
                    }
                    else {
                        this.tools = new Tools(this.textAreaValue, parametersOptions);
                    }
                    this._createMainPopupButtons();
                }
                ExpressionEditor.prototype._createMainPopupButtons = function () {
                    var self = this;
                    this.buttonItems = [
                        { toolbar: 'bottom', location: 'after', widget: 'button', options: { text: 'Save', onClick: function (sender) {
                            self.save(sender);
                        } } },
                        { toolbar: 'bottom', location: 'after', widget: 'button', options: { text: 'Cancel', onClick: function () {
                            self.popupVisible(false);
                        } } },
                        { toolbar: 'bottom', location: 'before', template: function () {
                            return $('#dx-expressioneditor-description');
                        }, text: self.tools.description }
                    ];
                };
                return ExpressionEditor;
            })();
            Widgets.ExpressionEditor = ExpressionEditor;
            ko.bindingHandlers['dxExpressionEditor'] = {
                init: function (element, valueAccessor) {
                    $(element).children().remove();
                    $(element).addClass("dx-filtereditor");
                    var templateHtml = $('#dxrd-expressioneditor').text(), $element = $(element).append(templateHtml), values = valueAccessor();
                    ko.applyBindings(new ExpressionEditor(values.options, values.fieldListProvider), $element.children()[0]);
                    return { controlsDescendantBindings: true };
                }
            };
        })(Widgets = Designer.Widgets || (Designer.Widgets = {}));
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        var dxFieldListPicker = (function (_super) {
            __extends(dxFieldListPicker, _super);
            function dxFieldListPicker(element, options) {
                _super.call(this, element, options);
            }
            dxFieldListPicker.prototype._renderValue = function () {
                var dataBinding = this.option("dataBinding"), value = dataBinding && dataBinding.dataMember() || this.option("value"), displayExpr = this.option("displayExpr") && this.option("displayExpr")(value);
                this.option("text", displayExpr !== null && displayExpr !== void 0 ? displayExpr : value);
                _super.prototype._renderValue.call(this);
            };
            dxFieldListPicker.prototype._showDropDown = function () {
                if (this._popup) {
                    this._popup.option("width", this.element().width());
                    var popupContent = this._popup.content() && this._popup.content()[0];
                    popupContent.style.maxHeight = 'none';
                }
            };
            dxFieldListPicker.prototype._renderContentImpl = function () {
            };
            dxFieldListPicker.prototype._optionChanged = function (obj, value) {
                var _this = this;
                var name = obj.name || obj, newValue = value || obj.value;
                switch (name) {
                    case "value":
                        this._input().val(newValue);
                        var dataBinding = this.option("dataBinding");
                        if (dataBinding) {
                            dataBinding.updateBinding(newValue, this.option("dataSources"));
                        }
                        setTimeout($.proxy(function () {
                            _this.option("opened", false);
                        }, this), 50);
                    default:
                        _super.prototype._optionChanged.apply(this, arguments);
                        if (name === "opened" && newValue) {
                            this._showDropDown();
                        }
                }
            };
            dxFieldListPicker.prototype._closeOutsideDropDownHandler = function (e, ignoreContainerClicks) {
                _super.prototype._closeOutsideDropDownHandler.call(this, e, true);
            };
            dxFieldListPicker.prototype._hideOnBlur = function () {
                return false;
            };
            dxFieldListPicker.prototype._popupConfig = function () {
                return $.extend(_super.prototype._popupConfig.call(this), {
                    container: '.dx-viewport',
                    contentTemplate: this._options._templates.template,
                    closeOnOutsideClick: true
                });
            };
            return dxFieldListPicker;
        })(DevExpress.ui.dxDropDownEditor);
        Designer.dxFieldListPicker = dxFieldListPicker;
        DevExpress.registerComponent("dxFieldListPicker", dxFieldListPicker);
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        var Widgets;
        (function (Widgets) {
            var editor_prefix = "dx-fileimage", EDITOR_CLASS = editor_prefix + " dx-dropdowneditor", EDITOR_INPUT_WRAPPER_CLASS = editor_prefix + "-input-wrapper", EDITOR_BUTTON_CLASS = editor_prefix + "-button dx-widget dx-button-normal dx-dropdowneditor-button dxrd-ellipsis-button", EDITOR_BUTTON_ICON = editor_prefix + "-icon" + " dxrd-ellipsis-image dx-dropdowneditor-icon";
            var dxFileImagePicker = (function (_super) {
                __extends(dxFileImagePicker, _super);
                function dxFileImagePicker(element, options) {
                    _super.call(this, element, options);
                }
                dxFileImagePicker.prototype._handleFiles = function (filesHolder) {
                    var _this = this;
                    var files = filesHolder.files;
                    for (var i = 0; i < files.length; i++) {
                        var file = files[i];
                        var imageType = /image.*/;
                        if (!file.type.match(imageType)) {
                            continue;
                        }
                        var fr = new FileReader();
                        fr.onload = function (args) {
                            var encodedImage = fr.result.replace(/^data:[^,]+,/, '');
                            _this.option("value", encodedImage);
                        };
                        fr.readAsDataURL(file);
                    }
                };
                dxFileImagePicker.prototype._render = function () {
                    _super.prototype._render.call(this);
                    var _this = this;
                    this.element().addClass(EDITOR_CLASS);
                    this._renderButton();
                    this._filesinput = $("<input type='file' accept='image/x' style='display:none' />").on("change", function (e) {
                        _this._handleFiles(_this._filesinput.get(0));
                    }).appendTo(this.element());
                };
                dxFileImagePicker.prototype._renderInput = function (inputContainer) {
                    this._inputContainer = inputContainer || $("<div />");
                    this._inputContainer.addClass(EDITOR_INPUT_WRAPPER_CLASS);
                    this.element().append(this["_inputContainer"]);
                    _super.prototype._renderInput.call(this, inputContainer);
                };
                dxFileImagePicker.prototype._renderButton = function () {
                    this._button = $("<div />").addClass(EDITOR_BUTTON_CLASS);
                    this._attachButtonEvents();
                    this._buttonIcon = $("<div />").addClass(EDITOR_BUTTON_ICON).height("100%").appendTo(this._button);
                    var buttonsContainer = _super.prototype._buttonsContainer.call(this);
                    this._button.appendTo(buttonsContainer);
                };
                dxFileImagePicker.prototype._updateButtonSize = function () {
                    this._buttonIcon.height(this.element().height());
                };
                dxFileImagePicker.prototype._attachButtonEvents = function () {
                    var _this = this;
                    this._button.off("click");
                    if (!this.option("disabled")) {
                        this._button.on("click", function (e) {
                            if (!_this.option("value")) {
                                _this._filesinput.val("");
                            }
                            _this._filesinput.click();
                        });
                    }
                };
                dxFileImagePicker.prototype._renderValue = function () {
                    this.option("text", this.option("value") ? "image" : "(none)");
                    _super.prototype._renderValue.call(this);
                };
                return dxFileImagePicker;
            })(DevExpress.ui.dxTextBox);
            Widgets.dxFileImagePicker = dxFileImagePicker;
            DevExpress.registerComponent("dxFileImagePicker", dxFileImagePicker);
        })(Widgets = Designer.Widgets || (Designer.Widgets = {}));
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        var Widgets;
        (function (Widgets) {
            var editor_prefix = "dx-filterstringeditor", EDITOR_CLASS = editor_prefix + " dx-dropdowneditor", EDITOR_BUTTON_CLASS = editor_prefix + "-button dx-widget dx-button-normal dx-dropdowneditor-button dxrd-ellipsis-button", EDITOR_BUTTON_ICON = editor_prefix + "-icon dxrd-ellipsis-image dx-dropdowneditor-icon";
            var dxPopupWithAutoHeight = (function (_super) {
                __extends(dxPopupWithAutoHeight, _super);
                function dxPopupWithAutoHeight(element, options) {
                    _super.call(this, element, options);
                }
                dxPopupWithAutoHeight.prototype._setContentHeight = function () {
                    this["_$popupContent"].css({
                        height: "100%"
                    });
                };
                return dxPopupWithAutoHeight;
            })(DevExpress.ui.dxPopup);
            Widgets.dxPopupWithAutoHeight = dxPopupWithAutoHeight;
            var dxFilterStringEditor = (function (_super) {
                __extends(dxFilterStringEditor, _super);
                function dxFilterStringEditor(element, options) {
                    _super.call(this, element, options);
                }
                dxFilterStringEditor.prototype._init = function () {
                    _super.prototype._init.call(this);
                    this.element().addClass(EDITOR_CLASS);
                };
                dxFilterStringEditor.prototype._render = function () {
                    _super.prototype._render.call(this);
                    this._renderButton();
                };
                dxFilterStringEditor.prototype._renderButton = function () {
                    this._button = $("<div />").addClass(EDITOR_BUTTON_CLASS);
                    this._attachButtonEvents();
                    this._buttonIcon = $("<div />").addClass(EDITOR_BUTTON_ICON).appendTo(this._button);
                    var buttonsContainer = _super.prototype._buttonsContainer.call(this);
                    this._button.appendTo(buttonsContainer);
                };
                dxFilterStringEditor.prototype._attachButtonEvents = function () {
                    var _this = this;
                    this._button.off("click");
                    if (!this.option("disabled")) {
                        this._button.on("click", function (e) {
                            if (_this.option("buttonAction")) {
                                _this.option("buttonAction")();
                                e.stopPropagation();
                            }
                        });
                    }
                };
                dxFilterStringEditor.prototype._optionChanged = function (obj, value) {
                    var name = obj.name || obj;
                    switch (name) {
                        case "disabled":
                            this._attachButtonEvents();
                            break;
                    }
                    _super.prototype._optionChanged.apply(this, arguments);
                };
                return dxFilterStringEditor;
            })(DevExpress.ui.dxTextEditor);
            Widgets.dxFilterStringEditor = dxFilterStringEditor;
            DevExpress.registerComponent("dxFilterStringEditor", dxFilterStringEditor);
            DevExpress.registerComponent("dxPopupWithAutoHeight", dxPopupWithAutoHeight);
        })(Widgets = Designer.Widgets || (Designer.Widgets = {}));
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        var Widgets;
        (function (Widgets) {
            var fonts = [
                "Times New Roman",
                "Arial",
                "Arial Black",
                "Comic Sans MS",
                "Courier New",
                "Georgia",
                "Impact",
                "Lucida Console",
                "Lucida Sans Unicode",
                "Tahoma",
                "Trebuchet MS",
                "Verdana",
                "MS Sans Serif",
                "MS Serif"
            ];
            var units = [
                { value: "pt", display: "Point" },
                { value: "world", display: "World" },
                { value: "px", display: "Pixel" },
                { value: "in", display: "Inch" },
                { value: "doc", display: "Document" },
                { value: "mm", display: "Millimeter" }
            ];
            var FontModel = (function () {
                function FontModel(object) {
                    var _this = this;
                    this.modificators = {
                        bold: null,
                        italic: null,
                        underline: null,
                        strikeout: null
                    };
                    this["fonts"] = fonts;
                    this["units"] = units;
                    this.family = ko.observable("Times New Roman");
                    this.unit = ko.observable("pt");
                    this.size = ko.observable(9);
                    this.modificators.bold = ko.observable(false);
                    this.modificators.italic = ko.observable(false);
                    this.modificators.strikeout = ko.observable(false);
                    this.modificators.underline = ko.observable(false);
                    var isUpdated = false;
                    ko.computed(function () {
                        if (isUpdated)
                            return;
                        isUpdated = true;
                        try {
                            if (object.value()) {
                                var components = object.value().split(',');
                                var family = components[0];
                                var size;
                                var unit;
                                var bold = false;
                                var italic = false;
                                var underline = false;
                                var strikeout = false;
                                units.forEach(function (element) {
                                    if (components[1].indexOf(element.value) != -1) {
                                        size = parseFloat(components[1].split(element.value)[0]);
                                        unit = element.value;
                                    }
                                });
                                if (components.length > 2) {
                                    for (var i = 2; i < components.length; i++) {
                                        if (components[i].indexOf("Bold") != -1)
                                            bold = true;
                                        if (components[i].indexOf("Italic") != -1)
                                            italic = true;
                                        if (components[i].indexOf("Underline") != -1)
                                            underline = true;
                                        if (components[i].indexOf("Strikeout") != -1)
                                            strikeout = true;
                                    }
                                }
                                _this.family(family);
                                _this.unit(unit);
                                _this.size(size);
                                _this.modificators.bold(bold);
                                _this.modificators.italic(italic);
                                _this.modificators.underline(underline);
                                _this.modificators.strikeout(strikeout);
                            }
                        }
                        finally {
                            isUpdated = false;
                        }
                    });
                    ko.computed(function () {
                        var result = _this.family() + ", " + _this.size() + _this.unit();
                        if (_this.modificators.bold() || _this.modificators.italic() || _this.modificators.strikeout() || _this.modificators.underline()) {
                            result += ", style=";
                            var styles = [];
                            if (_this.modificators.bold())
                                styles.push("Bold");
                            if (_this.modificators.italic())
                                styles.push("Italic");
                            if (_this.modificators.underline())
                                styles.push("Underline");
                            if (_this.modificators.strikeout())
                                styles.push("Strikeout");
                            result += styles.join(', ');
                        }
                        if (!isUpdated) {
                            object.value(result);
                        }
                    });
                }
                return FontModel;
            })();
            Widgets.FontModel = FontModel;
        })(Widgets = Designer.Widgets || (Designer.Widgets = {}));
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        var Widgets;
        (function (Widgets) {
            var PaddingModel = (function () {
                function PaddingModel(object) {
                    var _this = this;
                    var isUpdated = false;
                    this.left = ko.observable(0);
                    this.right = ko.observable(0);
                    this.top = ko.observable(0);
                    this.bottom = ko.observable(0);
                    this.dpi = 100;
                    ko.computed(function () {
                        if (isUpdated)
                            return;
                        isUpdated = true;
                        if (object.value()) {
                            var val = object.value();
                            var components = val.split(',');
                            _this.left(parseFloat(components[0]) || 0);
                            _this.right(parseFloat(components[1]) || 0);
                            _this.top(parseFloat(components[2]) || 0);
                            _this.bottom(parseFloat(components[3]) || 0);
                            _this.dpi = parseFloat(components[4]) || 100;
                        }
                        isUpdated = false;
                    });
                    ko.computed(function () {
                        if (_this.left() || _this.right() || _this.top() || _this.bottom()) {
                            var result = _this.left() + "," + _this.right() + "," + _this.top() + "," + _this.bottom() + "," + 100;
                            if (!isUpdated) {
                                object.value(result);
                            }
                        }
                    });
                }
                return PaddingModel;
            })();
            Widgets.PaddingModel = PaddingModel;
        })(Widgets = Designer.Widgets || (Designer.Widgets = {}));
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
//# sourceMappingURL=dx-designer.js.map
/* parser generated by jison 0.4.15 */
/*
  Returns a Parser object of the following structure:

  Parser: {
    yy: {}
  }

  Parser.prototype: {
    yy: {},
    trace: function(),
    symbols_: {associative list: name ==> number},
    terminals_: {associative list: number ==> name},
    productions_: [...],
    performAction: function anonymous(yytext, yyleng, yylineno, yy, yystate, $$, _$),
    table: [...],
    defaultActions: {...},
    parseError: function(str, hash),
    parse: function(input),

    lexer: {
        EOF: 1,
        parseError: function(str, hash),
        setInput: function(input),
        input: function(),
        unput: function(str),
        more: function(),
        less: function(n),
        pastInput: function(),
        upcomingInput: function(),
        showPosition: function(),
        test_match: function(regex_match_array, rule_index),
        next: function(),
        lex: function(),
        begin: function(condition),
        popState: function(),
        _currentRules: function(),
        topState: function(),
        pushState: function(condition),

        options: {
            ranges: boolean           (optional: true ==> token location info will include a .range[] member)
            flex: boolean             (optional: true ==> flex-like lexing behaviour where the rules are tested exhaustively to find the longest match)
            backtrack_lexer: boolean  (optional: true ==> lexer regexes are tested in order and for each matching regex the action code is invoked; the lexer terminates the scan when a token is returned by the action code)
        },

        performAction: function(yy, yy_, $avoiding_name_collisions, YY_START),
        rules: [...],
        conditions: {associative list: name ==> set},
    }
  }


  token location info (@$, _$, etc.): {
    first_line: n,
    last_line: n,
    first_column: n,
    last_column: n,
    range: [start_number, end_number]       (where the numbers are indexes into the input string, regular zero-based)
  }


  the parseError function receives a 'hash' object with these members for lexer and parser errors: {
    text:        (matched text)
    token:       (the produced terminal token, if any)
    line:        (yylineno)
  }
  while parser (grammar) errors will also provide these members, i.e. parser errors deliver a superset of attributes: {
    loc:         (yylloc)
    expected:    (string describing the set of expected tokens)
    recoverable: (boolean: TRUE when the parser has a error recovery rule available for this particular error)
  }
*/
var parser = (function(){
var o=function(k,v,o,l){for(o=o||{},l=k.length;l--;o[k[l]]=v);return o},$V0=[1,15],$V1=[1,10],$V2=[1,33],$V3=[1,27],$V4=[1,18],$V5=[1,19],$V6=[1,28],$V7=[1,29],$V8=[1,13],$V9=[1,30],$Va=[1,31],$Vb=[1,32],$Vc=[1,22],$Vd=[1,23],$Ve=[1,3],$Vf=[1,4],$Vg=[1,5],$Vh=[1,9],$Vi=[1,11],$Vj=[1,12],$Vk=[1,14],$Vl=[1,37],$Vm=[1,46],$Vn=[1,45],$Vo=[1,42],$Vp=[1,35],$Vq=[1,36],$Vr=[1,38],$Vs=[1,39],$Vt=[1,40],$Vu=[1,41],$Vv=[1,43],$Vw=[1,44],$Vx=[1,47],$Vy=[1,48],$Vz=[1,49],$VA=[1,50],$VB=[1,51],$VC=[1,52],$VD=[1,53],$VE=[1,54],$VF=[1,55],$VG=[5,11,17,19,20,22,28,35,46,47,48,49,50,51,53,54,55,56,57,58,59,60,61,62,64],$VH=[1,64],$VI=[2,14],$VJ=[1,67],$VK=[1,69],$VL=[1,72],$VM=[16,28],$VN=[1,76],$VO=[15,16,28],$VP=[5,11,19,20,22,28,35,50,51,53,54,55,56,57,58,59,60,61,62,64],$VQ=[5,11,28,35,59,60],$VR=[16,17,20],$VS=[5,11,17,19,20,22,28,35,48,50,51,53,54,55,56,57,58,59,60,61,62,64],$VT=[5,11,28,35,53,54,57,58,59,60,61],$VU=[5,11,19,20,28,35,53,54,55,56,57,58,59,60,61],$VV=[11,35];
var parser = {trace: function trace() { },
yy: {},
symbols_: {"error":2,"expressions":3,"exp":4,"EOF":5,"criteriaList":6,"\\0":7,"queryCollection":8,"expOrSort":9,";":10,",":11,"SORT_ASC":12,"SORT_DESC":13,"type":14,"COL":15,".":16,"+":17,"upcast":18,"OP_LT":19,"OP_GT":20,"column":21,"^":22,"param":23,"?":24,"property":25,"field":26,"[":27,"]":28,"aggregate":29,"aggregateSuffix":30,"topLevelAggregate":31,"AGG_COUNT":32,"AGG_EXISTS":33,"(":34,")":35,"AGG_AVG":36,"AGG_SUM":37,"AGG_SINGLE":38,"MinStart":39,"MaxStart":40,"AGG_MIN":41,"AGG_MAX":42,"CONST":43,"NUM":44,"NULL":45,"*":46,"/":47,"-":48,"%":49,"|":50,"&":51,"~":52,"OP_EQ":53,"OP_NE":54,"OP_GE":55,"OP_LE":56,"OP_LIKE":57,"NOT":58,"AND":59,"OR":60,"IS":61,"OP_IN":62,"argumentslist":63,"OP_BETWEEN":64,"FUNCTION":65,"commadelimitedlist":66,"$accept":0,"$end":1},
terminals_: {2:"error",5:"EOF",7:"\\0",10:";",11:",",12:"SORT_ASC",13:"SORT_DESC",15:"COL",16:".",17:"+",19:"OP_LT",20:"OP_GT",22:"^",24:"?",27:"[",28:"]",32:"AGG_COUNT",33:"AGG_EXISTS",34:"(",35:")",36:"AGG_AVG",37:"AGG_SUM",38:"AGG_SINGLE",41:"AGG_MIN",42:"AGG_MAX",43:"CONST",44:"NUM",45:"NULL",46:"*",47:"/",48:"-",49:"%",50:"|",51:"&",52:"~",53:"OP_EQ",54:"OP_NE",55:"OP_GE",56:"OP_LE",57:"OP_LIKE",58:"NOT",59:"AND",60:"OR",61:"IS",62:"OP_IN",64:"OP_BETWEEN",65:"FUNCTION"},
productions_: [0,[3,2],[6,1],[6,2],[8,1],[8,3],[8,3],[9,1],[9,2],[9,2],[14,1],[14,3],[14,3],[18,4],[21,1],[21,2],[21,1],[21,1],[23,2],[23,1],[25,1],[25,3],[26,3],[29,3],[29,6],[29,5],[29,4],[29,3],[29,1],[31,1],[30,1],[30,1],[30,3],[30,3],[30,4],[30,4],[30,3],[30,4],[30,2],[30,2],[39,3],[40,3],[4,1],[4,1],[4,1],[4,1],[4,1],[4,1],[4,3],[4,3],[4,3],[4,3],[4,3],[4,3],[4,3],[4,3],[4,2],[4,2],[4,2],[4,3],[4,3],[4,3],[4,3],[4,3],[4,3],[4,3],[4,4],[4,2],[4,3],[4,3],[4,3],[4,3],[4,4],[4,3],[4,7],[4,2],[4,2],[4,2],[4,4],[4,4],[63,3],[63,2],[66,1],[66,3]],
performAction: function anonymous(yytext, yyleng, yylineno, yy, yystate /* action[1] */, $$ /* vstack */, _$ /* lstack */) {
/* this == yyval */

var $0 = $$.length - 1;
switch (yystate) {
case 1:
 return $$[$0-1]; 
break;
case 2:
 result = new DevExpress.Data.CriteriaOperator(); 
break;
case 3:
 result = $$[$0-1]; 
break;
case 4:
 this.$ = [ $$[$0] ]; 
break;
case 5: case 6:
 this.$ = $$[$0-2]; this.$.push($$[$0]); 
break;
case 7: case 10: case 16: case 20: case 45: case 46: case 47:
 this.$ = $$[$0]; 
break;
case 8: case 38: case 39: case 70: case 80:
 this.$ = $$[$0-1]; 
break;
case 9:
 this.$ = new DevExpress.Data.FunctionOperator(DevExpress.Data.FunctionOperatorType.OrderDescToken, $$[$0-1]); 
break;
case 11: case 21:
 this.$ = new DevExpress.Data.OperandProperty($$[$0-2].propertyName + '.' + $$[$0].propertyName); 
break;
case 12:
 this.$ = new DevExpress.Data.OperandProperty($$[$0-2].propertyName + '+' + $$[$0].propertyName); 
break;
case 13:
 this.$ = new DevExpress.Data.OperandProperty('<' + $$[$0-2].propertyName + '>' + $$[$0].propertyName); 
break;
case 14:
 this.$ = new DevExpress.Data.OperandProperty($$[$0]); 
break;
case 15:
 this.$ = new DevExpress.Data.OperandProperty($$[$0-1].propertyName + ' ' + $$[$0]); 
break;
case 17:
 this.$ = new DevExpress.Data.OperandProperty("^"); 
break;
case 18:
 this.$ = new DevExpress.Data.OperandParameter($$[$0]); 
break;
case 19:
 this.$ = new DevExpress.Data.OperandValue(undefined); 
break;
case 22:
 this.$ = new DevExpress.Data.OperandProperty($$[$0-1].propertyName); 
break;
case 23:

		var agg = $$[$0];
		this.$ = DevExpress.Data.JoinOperand.joinOrAggregate($$[$0-2], null, agg.operatorType, agg.aggregatedExpression);
	
break;
case 24:

		var agg = $$[$0];
		this.$ = DevExpress.Data.JoinOperand.joinOrAggregate($$[$0-5], $$[$0-3], agg.operatorType, agg.aggregatedExpression);
	
break;
case 25:

		var agg = $$[$0];
		this.$ = DevExpress.Data.JoinOperand.joinOrAggregate($$[$0-4], null, agg.operatorType, agg.aggregatedExpression);
	
break;
case 26:
 this.$ = DevExpress.Data.JoinOperand.joinOrAggregate($$[$0-3], $$[$0-1], DevExpress.Data.Aggregate.Exists, null); 
break;
case 27:
 this.$ = DevExpress.Data.JoinOperand.joinOrAggregate($$[$0-2], null, DevExpress.Data.Aggregate.Exists, null); 
break;
case 30: case 32:
 this.$ = new DevExpress.Data.AggregateOperand(null, null, DevExpress.Data.Aggregate.Count, null); 
break;
case 31: case 33:
 this.$ = new DevExpress.Data.AggregateOperand(null, null, DevExpress.Data.Aggregate.Exists, null); 
break;
case 34:
 this.$ = new DevExpress.Data.AggregateOperand(null, $$[$0-1], DevExpress.Data.Aggregate.Avg, null); 
break;
case 35:
 this.$ = new DevExpress.Data.AggregateOperand(null, $$[$0-1], DevExpress.Data.Aggregate.Sum, null); 
break;
case 36:
 this.$ = new DevExpress.Data.AggregateOperand(null, new DevExpress.Data.OperandProperty("This"), DevExpress.Data.Aggregate.Single, null); 
break;
case 37:
 this.$ = new DevExpress.Data.AggregateOperand(null, $$[$0-1], DevExpress.Data.Aggregate.Single, null); 
break;
case 40:
 this.$ = new DevExpress.Data.AggregateOperand(null, $$[$0], DevExpress.Data.Aggregate.Min, null); 
break;
case 41:
 this.$ = new DevExpress.Data.AggregateOperand(null, $$[$0], DevExpress.Data.Aggregate.Max, null); 
break;
case 42:
 this.$ = new DevExpress.Data.ConstantValue($$[$0]); 
break;
case 43:
 this.$ = new DevExpress.Data.ConstantValue(parseFloat($$[$0])); 
break;
case 44:
 this.$ = new DevExpress.Data.ConstantValue(null); 
break;
case 48:
 this.$ = new DevExpress.Data.BinaryOperator($$[$0-2], $$[$0], DevExpress.Data.BinaryOperatorType.Multiply); 
break;
case 49:
 this.$ = new DevExpress.Data.BinaryOperator($$[$0-2], $$[$0], DevExpress.Data.BinaryOperatorType.Divide); 
break;
case 50:
 this.$ = new DevExpress.Data.BinaryOperator($$[$0-2], $$[$0], DevExpress.Data.BinaryOperatorType.Plus); 
break;
case 51:
 this.$ = new DevExpress.Data.BinaryOperator($$[$0-2], $$[$0], DevExpress.Data.BinaryOperatorType.Minus); 
break;
case 52:
 this.$ = new DevExpress.Data.BinaryOperator($$[$0-2], $$[$0], DevExpress.Data.BinaryOperatorType.Modulo); 
break;
case 53:
 this.$ = new DevExpress.Data.BinaryOperator($$[$0-2], $$[$0], DevExpress.Data.BinaryOperatorType.BitwiseOr); 
break;
case 54:
 this.$ = new DevExpress.Data.BinaryOperator($$[$0-2], $$[$0], DevExpress.Data.BinaryOperatorType.BitwiseAnd); 
break;
case 55:
 this.$ = new DevExpress.Data.BinaryOperator($$[$0-2], $$[$0], DevExpress.Data.BinaryOperatorType.BitwiseXor); 
break;
case 56:

								this.$ = new DevExpress.Data.UnaryOperator(DevExpress.Data.UnaryOperatorType.Minus, $$[$0]);
							
break;
case 57:
 this.$ = new DevExpress.Data.UnaryOperator(DevExpress.Data.UnaryOperatorType.Plus, $$[$0]); 
break;
case 58:
 this.$ = new DevExpress.Data.UnaryOperator(DevExpress.Data.UnaryOperatorType.BitwiseNot, $$[$0]); 
break;
case 59:
 this.$ = new DevExpress.Data.BinaryOperator($$[$0-2], $$[$0], DevExpress.Data.BinaryOperatorType.Equal); 
break;
case 60:
 this.$ = new DevExpress.Data.BinaryOperator($$[$0-2], $$[$0], DevExpress.Data.BinaryOperatorType.NotEqual); 
break;
case 61:
 this.$ = new DevExpress.Data.BinaryOperator($$[$0-2], $$[$0], DevExpress.Data.BinaryOperatorType.Greater); 
break;
case 62:
 this.$ = new DevExpress.Data.BinaryOperator($$[$0-2], $$[$0], DevExpress.Data.BinaryOperatorType.Less); 
break;
case 63:
 this.$ = new DevExpress.Data.BinaryOperator($$[$0-2], $$[$0], DevExpress.Data.BinaryOperatorType.GreaterOrEqual); 
break;
case 64:
 this.$ = new DevExpress.Data.BinaryOperator($$[$0-2], $$[$0], DevExpress.Data.BinaryOperatorType.LessOrEqual); 
break;
case 65:
 this.$ = new DevExpress.Data.BinaryOperator($$[$0-2], $$[$0], DevExpress.Data.BinaryOperatorType.Like); 
break;
case 66:
 this.$ = new DevExpress.Data.UnaryOperator(DevExpress.Data.UnaryOperatorType.Not, new DevExpress.Data.BinaryOperator($$[$0-3], $$[$0], DevExpress.Data.BinaryOperatorType.Like)); 
break;
case 67:
 this.$ = new DevExpress.Data.UnaryOperator(DevExpress.Data.UnaryOperatorType.Not, $$[$0]); 
break;
case 68:
 this.$ = DevExpress.Data.GroupOperator.combine(DevExpress.Data.GroupOperatorType.And, [$$[$0-2], $$[$0]]); 
break;
case 69:
 this.$ = DevExpress.Data.GroupOperator.combine(DevExpress.Data.GroupOperatorType.Or, [$$[$0-2], $$[$0]]); 
break;
case 71:
 this.$ = new DevExpress.Data.UnaryOperator(DevExpress.Data.UnaryOperatorType.IsNull, $$[$0-2]); 
break;
case 72:
 this.$ = new DevExpress.Data.UnaryOperator(DevExpress.Data.UnaryOperatorType.Not, new DevExpress.Data.UnaryOperator(DevExpress.Data.UnaryOperatorType.IsNull, $$[$0-3])); 
break;
case 73:
 this.$ = new DevExpress.Data.InOperator($$[$0-2], $$[$0]); 
break;
case 74:
 this.$ = new DevExpress.Data.BetweenOperator($$[$0-6], $$[$0-3], $$[$0-1]); 
break;
case 75: case 76:
  this.$ = new DevExpress.Data.FunctionOperator(DevExpress.Data.FunctionOperatorType[$$[$0-1]] || $$[$0-1], $$[$0]); 
break;
case 77:
 this.$ = null; 
break;
case 78:
 this.$ = new DevExpress.Data.FunctionOperator(DevExpress.Data.FunctionOperatorType.Min, [$$[$0-3].aggregatedExpression, $$[$0-1]]); 
break;
case 79:
 this.$ = new DevExpress.Data.FunctionOperator(DevExpress.Data.FunctionOperatorType.Max, [$$[$0-3].aggregatedExpression, $$[$0-1]]); 
break;
case 81:
 this.$ = []; 
break;
case 82:

							var lst = [];
							lst.push($$[$0]);
							this.$ = lst;
						
break;
case 83:

							var lst = $$[$0-2];
							lst.push($$[$0]);
							this.$ = lst;
						
break;
}
},
table: [{3:1,4:2,15:$V0,17:$V1,18:26,19:$V2,21:24,22:$V3,23:6,24:$V4,25:20,26:7,27:$V5,29:8,30:25,31:21,32:$V6,33:$V7,34:$V8,36:$V9,37:$Va,38:$Vb,39:16,40:17,41:$Vc,42:$Vd,43:$Ve,44:$Vf,45:$Vg,48:$Vh,52:$Vi,58:$Vj,65:$Vk},{1:[3]},{5:[1,34],17:$Vl,19:$Vm,20:$Vn,22:$Vo,46:$Vp,47:$Vq,48:$Vr,49:$Vs,50:$Vt,51:$Vu,53:$Vv,54:$Vw,55:$Vx,56:$Vy,57:$Vz,58:$VA,59:$VB,60:$VC,61:$VD,62:$VE,64:$VF},o($VG,[2,42]),o($VG,[2,43]),o($VG,[2,44]),o($VG,[2,45]),o($VG,[2,46],{27:[1,56]}),o($VG,[2,47]),{4:57,15:$V0,17:$V1,18:26,19:$V2,21:24,22:$V3,23:6,24:$V4,25:20,26:7,27:$V5,29:8,30:25,31:21,32:$V6,33:$V7,34:$V8,36:$V9,37:$Va,38:$Vb,39:16,40:17,41:$Vc,42:$Vd,43:$Ve,44:$Vf,45:$Vg,48:$Vh,52:$Vi,58:$Vj,65:$Vk},{4:58,15:$V0,17:$V1,18:26,19:$V2,21:24,22:$V3,23:6,24:$V4,25:20,26:7,27:$V5,29:8,30:25,31:21,32:$V6,33:$V7,34:$V8,36:$V9,37:$Va,38:$Vb,39:16,40:17,41:$Vc,42:$Vd,43:$Ve,44:$Vf,45:$Vg,48:$Vh,52:$Vi,58:$Vj,65:$Vk},{4:59,15:$V0,17:$V1,18:26,19:$V2,21:24,22:$V3,23:6,24:$V4,25:20,26:7,27:$V5,29:8,30:25,31:21,32:$V6,33:$V7,34:$V8,36:$V9,37:$Va,38:$Vb,39:16,40:17,41:$Vc,42:$Vd,43:$Ve,44:$Vf,45:$Vg,48:$Vh,52:$Vi,58:$Vj,65:$Vk},{4:60,15:$V0,17:$V1,18:26,19:$V2,21:24,22:$V3,23:6,24:$V4,25:20,26:7,27:$V5,29:8,30:25,31:21,32:$V6,33:$V7,34:$V8,36:$V9,37:$Va,38:$Vb,39:16,40:17,41:$Vc,42:$Vd,43:$Ve,44:$Vf,45:$Vg,48:$Vh,52:$Vi,58:$Vj,65:$Vk},{4:61,15:$V0,17:$V1,18:26,19:$V2,21:24,22:$V3,23:6,24:$V4,25:20,26:7,27:$V5,29:8,30:25,31:21,32:$V6,33:$V7,34:$V8,35:[1,62],36:$V9,37:$Va,38:$Vb,39:16,40:17,41:$Vc,42:$Vd,43:$Ve,44:$Vf,45:$Vg,48:$Vh,52:$Vi,58:$Vj,65:$Vk},{34:$VH,63:63},o([15,16],$VI,{63:65,34:$VH}),{11:[1,66],35:$VJ},{11:[1,68],35:$VK},o($VG,[2,19],{15:[1,70]}),{15:$VL,18:26,19:$V2,21:24,22:$V3,25:71},{16:[1,73]},o($VG,[2,28]),{34:[1,74]},{34:[1,75]},o($VM,[2,20],{15:$VN}),o($VG,[2,29]),o($VO,[2,16]),o($VO,[2,17]),o($VG,[2,30],{34:[1,77]}),o($VG,[2,31],{34:[1,78]}),{34:[1,79]},{34:[1,80]},{34:[1,81]},{14:82,15:[1,83]},{1:[2,1]},{4:84,15:$V0,17:$V1,18:26,19:$V2,21:24,22:$V3,23:6,24:$V4,25:20,26:7,27:$V5,29:8,30:25,31:21,32:$V6,33:$V7,34:$V8,36:$V9,37:$Va,38:$Vb,39:16,40:17,41:$Vc,42:$Vd,43:$Ve,44:$Vf,45:$Vg,48:$Vh,52:$Vi,58:$Vj,65:$Vk},{4:85,15:$V0,17:$V1,18:26,19:$V2,21:24,22:$V3,23:6,24:$V4,25:20,26:7,27:$V5,29:8,30:25,31:21,32:$V6,33:$V7,34:$V8,36:$V9,37:$Va,38:$Vb,39:16,40:17,41:$Vc,42:$Vd,43:$Ve,44:$Vf,45:$Vg,48:$Vh,52:$Vi,58:$Vj,65:$Vk},{4:86,15:$V0,17:$V1,18:26,19:$V2,21:24,22:$V3,23:6,24:$V4,25:20,26:7,27:$V5,29:8,30:25,31:21,32:$V6,33:$V7,34:$V8,36:$V9,37:$Va,38:$Vb,39:16,40:17,41:$Vc,42:$Vd,43:$Ve,44:$Vf,45:$Vg,48:$Vh,52:$Vi,58:$Vj,65:$Vk},{4:87,15:$V0,17:$V1,18:26,19:$V2,21:24,22:$V3,23:6,24:$V4,25:20,26:7,27:$V5,29:8,30:25,31:21,32:$V6,33:$V7,34:$V8,36:$V9,37:$Va,38:$Vb,39:16,40:17,41:$Vc,42:$Vd,43:$Ve,44:$Vf,45:$Vg,48:$Vh,52:$Vi,58:$Vj,65:$Vk},{4:88,15:$V0,17:$V1,18:26,19:$V2,21:24,22:$V3,23:6,24:$V4,25:20,26:7,27:$V5,29:8,30:25,31:21,32:$V6,33:$V7,34:$V8,36:$V9,37:$Va,38:$Vb,39:16,40:17,41:$Vc,42:$Vd,43:$Ve,44:$Vf,45:$Vg,48:$Vh,52:$Vi,58:$Vj,65:$Vk},{4:89,15:$V0,17:$V1,18:26,19:$V2,21:24,22:$V3,23:6,24:$V4,25:20,26:7,27:$V5,29:8,30:25,31:21,32:$V6,33:$V7,34:$V8,36:$V9,37:$Va,38:$Vb,39:16,40:17,41:$Vc,42:$Vd,43:$Ve,44:$Vf,45:$Vg,48:$Vh,52:$Vi,58:$Vj,65:$Vk},{4:90,15:$V0,17:$V1,18:26,19:$V2,21:24,22:$V3,23:6,24:$V4,25:20,26:7,27:$V5,29:8,30:25,31:21,32:$V6,33:$V7,34:$V8,36:$V9,37:$Va,38:$Vb,39:16,40:17,41:$Vc,42:$Vd,43:$Ve,44:$Vf,45:$Vg,48:$Vh,52:$Vi,58:$Vj,65:$Vk},{4:91,15:$V0,17:$V1,18:26,19:$V2,21:24,22:$V3,23:6,24:$V4,25:20,26:7,27:$V5,29:8,30:25,31:21,32:$V6,33:$V7,34:$V8,36:$V9,37:$Va,38:$Vb,39:16,40:17,41:$Vc,42:$Vd,43:$Ve,44:$Vf,45:$Vg,48:$Vh,52:$Vi,58:$Vj,65:$Vk},{4:92,15:$V0,17:$V1,18:26,19:$V2,21:24,22:$V3,23:6,24:$V4,25:20,26:7,27:$V5,29:8,30:25,31:21,32:$V6,33:$V7,34:$V8,36:$V9,37:$Va,38:$Vb,39:16,40:17,41:$Vc,42:$Vd,43:$Ve,44:$Vf,45:$Vg,48:$Vh,52:$Vi,58:$Vj,65:$Vk},{4:93,15:$V0,17:$V1,18:26,19:$V2,21:24,22:$V3,23:6,24:$V4,25:20,26:7,27:$V5,29:8,30:25,31:21,32:$V6,33:$V7,34:$V8,36:$V9,37:$Va,38:$Vb,39:16,40:17,41:$Vc,42:$Vd,43:$Ve,44:$Vf,45:$Vg,48:$Vh,52:$Vi,58:$Vj,65:$Vk},{4:94,15:$V0,17:$V1,18:26,19:$V2,21:24,22:$V3,23:6,24:$V4,25:20,26:7,27:$V5,29:8,30:25,31:21,32:$V6,33:$V7,34:$V8,36:$V9,37:$Va,38:$Vb,39:16,40:17,41:$Vc,42:$Vd,43:$Ve,44:$Vf,45:$Vg,48:$Vh,52:$Vi,58:$Vj,65:$Vk},{4:95,15:$V0,17:$V1,18:26,19:$V2,21:24,22:$V3,23:6,24:$V4,25:20,26:7,27:$V5,29:8,30:25,31:21,32:$V6,33:$V7,34:$V8,36:$V9,37:$Va,38:$Vb,39:16,40:17,41:$Vc,42:$Vd,43:$Ve,44:$Vf,45:$Vg,48:$Vh,52:$Vi,58:$Vj,65:$Vk},{4:96,15:$V0,17:$V1,18:26,19:$V2,21:24,22:$V3,23:6,24:$V4,25:20,26:7,27:$V5,29:8,30:25,31:21,32:$V6,33:$V7,34:$V8,36:$V9,37:$Va,38:$Vb,39:16,40:17,41:$Vc,42:$Vd,43:$Ve,44:$Vf,45:$Vg,48:$Vh,52:$Vi,58:$Vj,65:$Vk},{4:97,15:$V0,17:$V1,18:26,19:$V2,21:24,22:$V3,23:6,24:$V4,25:20,26:7,27:$V5,29:8,30:25,31:21,32:$V6,33:$V7,34:$V8,36:$V9,37:$Va,38:$Vb,39:16,40:17,41:$Vc,42:$Vd,43:$Ve,44:$Vf,45:$Vg,48:$Vh,52:$Vi,58:$Vj,65:$Vk},{4:98,15:$V0,17:$V1,18:26,19:$V2,21:24,22:$V3,23:6,24:$V4,25:20,26:7,27:$V5,29:8,30:25,31:21,32:$V6,33:$V7,34:$V8,36:$V9,37:$Va,38:$Vb,39:16,40:17,41:$Vc,42:$Vd,43:$Ve,44:$Vf,45:$Vg,48:$Vh,52:$Vi,58:$Vj,65:$Vk},{57:[1,99]},{4:100,15:$V0,17:$V1,18:26,19:$V2,21:24,22:$V3,23:6,24:$V4,25:20,26:7,27:$V5,29:8,30:25,31:21,32:$V6,33:$V7,34:$V8,36:$V9,37:$Va,38:$Vb,39:16,40:17,41:$Vc,42:$Vd,43:$Ve,44:$Vf,45:$Vg,48:$Vh,52:$Vi,58:$Vj,65:$Vk},{4:101,15:$V0,17:$V1,18:26,19:$V2,21:24,22:$V3,23:6,24:$V4,25:20,26:7,27:$V5,29:8,30:25,31:21,32:$V6,33:$V7,34:$V8,36:$V9,37:$Va,38:$Vb,39:16,40:17,41:$Vc,42:$Vd,43:$Ve,44:$Vf,45:$Vg,48:$Vh,52:$Vi,58:$Vj,65:$Vk},{45:[1,102],58:[1,103]},{34:$VH,63:104},{34:[1,105]},{4:106,15:$V0,17:$V1,18:26,19:$V2,21:24,22:$V3,23:6,24:$V4,25:20,26:7,27:$V5,28:[1,107],29:8,30:25,31:21,32:$V6,33:$V7,34:$V8,36:$V9,37:$Va,38:$Vb,39:16,40:17,41:$Vc,42:$Vd,43:$Ve,44:$Vf,45:$Vg,48:$Vh,52:$Vi,58:$Vj,65:$Vk},o($VG,[2,56]),o($VG,[2,57]),o($VP,[2,58],{17:$Vl,46:$Vp,47:$Vq,48:$Vr,49:$Vs}),o($VQ,[2,67],{17:$Vl,19:$Vm,20:$Vn,22:$Vo,46:$Vp,47:$Vq,48:$Vr,49:$Vs,50:$Vt,51:$Vu,53:$Vv,54:$Vw,55:$Vx,56:$Vy,57:$Vz,58:$VA,61:$VD,62:$VE,64:$VF}),{17:$Vl,19:$Vm,20:$Vn,22:$Vo,35:[1,108],46:$Vp,47:$Vq,48:$Vr,49:$Vs,50:$Vt,51:$Vu,53:$Vv,54:$Vw,55:$Vx,56:$Vy,57:$Vz,58:$VA,59:$VB,60:$VC,61:$VD,62:$VE,64:$VF},o($VG,[2,77]),o($VG,[2,75]),{4:111,15:$V0,17:$V1,18:26,19:$V2,21:24,22:$V3,23:6,24:$V4,25:20,26:7,27:$V5,29:8,30:25,31:21,32:$V6,33:$V7,34:$V8,35:[1,110],36:$V9,37:$Va,38:$Vb,39:16,40:17,41:$Vc,42:$Vd,43:$Ve,44:$Vf,45:$Vg,48:$Vh,52:$Vi,58:$Vj,65:$Vk,66:109},o($VG,[2,76]),{4:112,15:$V0,17:$V1,18:26,19:$V2,21:24,22:$V3,23:6,24:$V4,25:20,26:7,27:$V5,29:8,30:25,31:21,32:$V6,33:$V7,34:$V8,36:$V9,37:$Va,38:$Vb,39:16,40:17,41:$Vc,42:$Vd,43:$Ve,44:$Vf,45:$Vg,48:$Vh,52:$Vi,58:$Vj,65:$Vk},o($VG,[2,38]),{4:113,15:$V0,17:$V1,18:26,19:$V2,21:24,22:$V3,23:6,24:$V4,25:20,26:7,27:$V5,29:8,30:25,31:21,32:$V6,33:$V7,34:$V8,36:$V9,37:$Va,38:$Vb,39:16,40:17,41:$Vc,42:$Vd,43:$Ve,44:$Vf,45:$Vg,48:$Vh,52:$Vi,58:$Vj,65:$Vk},o($VG,[2,39]),o($VG,[2,18]),{16:[1,115],28:[1,114]},o($VO,$VI),{15:$VL,18:26,19:$V2,21:117,22:$V3,30:116,32:$V6,33:$V7,36:$V9,37:$Va,38:$Vb,39:118,40:119,41:$Vc,42:$Vd},{4:120,15:$V0,17:$V1,18:26,19:$V2,21:24,22:$V3,23:6,24:$V4,25:20,26:7,27:$V5,29:8,30:25,31:21,32:$V6,33:$V7,34:$V8,36:$V9,37:$Va,38:$Vb,39:16,40:17,41:$Vc,42:$Vd,43:$Ve,44:$Vf,45:$Vg,48:$Vh,52:$Vi,58:$Vj,65:$Vk},{4:121,15:$V0,17:$V1,18:26,19:$V2,21:24,22:$V3,23:6,24:$V4,25:20,26:7,27:$V5,29:8,30:25,31:21,32:$V6,33:$V7,34:$V8,36:$V9,37:$Va,38:$Vb,39:16,40:17,41:$Vc,42:$Vd,43:$Ve,44:$Vf,45:$Vg,48:$Vh,52:$Vi,58:$Vj,65:$Vk},o($VO,[2,15]),{35:[1,122]},{35:[1,123]},{4:124,15:$V0,17:$V1,18:26,19:$V2,21:24,22:$V3,23:6,24:$V4,25:20,26:7,27:$V5,29:8,30:25,31:21,32:$V6,33:$V7,34:$V8,36:$V9,37:$Va,38:$Vb,39:16,40:17,41:$Vc,42:$Vd,43:$Ve,44:$Vf,45:$Vg,48:$Vh,52:$Vi,58:$Vj,65:$Vk},{4:125,15:$V0,17:$V1,18:26,19:$V2,21:24,22:$V3,23:6,24:$V4,25:20,26:7,27:$V5,29:8,30:25,31:21,32:$V6,33:$V7,34:$V8,36:$V9,37:$Va,38:$Vb,39:16,40:17,41:$Vc,42:$Vd,43:$Ve,44:$Vf,45:$Vg,48:$Vh,52:$Vi,58:$Vj,65:$Vk},{4:127,15:$V0,17:$V1,18:26,19:$V2,21:24,22:$V3,23:6,24:$V4,25:20,26:7,27:$V5,29:8,30:25,31:21,32:$V6,33:$V7,34:$V8,35:[1,126],36:$V9,37:$Va,38:$Vb,39:16,40:17,41:$Vc,42:$Vd,43:$Ve,44:$Vf,45:$Vg,48:$Vh,52:$Vi,58:$Vj,65:$Vk},{16:[1,129],17:[1,130],20:[1,128]},o($VR,[2,10]),o($VG,[2,48]),o($VG,[2,49]),o($VS,[2,50],{46:$Vp,47:$Vq,49:$Vs}),o($VS,[2,51],{46:$Vp,47:$Vq,49:$Vs}),o($VG,[2,52]),o([5,11,19,20,28,35,50,53,54,55,56,57,58,59,60,61,62,64],[2,53],{17:$Vl,22:$Vo,46:$Vp,47:$Vq,48:$Vr,49:$Vs,51:$Vu}),o($VP,[2,54],{17:$Vl,46:$Vp,47:$Vq,48:$Vr,49:$Vs}),o([5,11,19,20,22,28,35,50,53,54,55,56,57,58,59,60,61,62,64],[2,55],{17:$Vl,46:$Vp,47:$Vq,48:$Vr,49:$Vs,51:$Vu}),o($VT,[2,59],{17:$Vl,19:$Vm,20:$Vn,22:$Vo,46:$Vp,47:$Vq,48:$Vr,49:$Vs,50:$Vt,51:$Vu,55:$Vx,56:$Vy,62:$VE,64:$VF}),o($VT,[2,60],{17:$Vl,19:$Vm,20:$Vn,22:$Vo,46:$Vp,47:$Vq,48:$Vr,49:$Vs,50:$Vt,51:$Vu,55:$Vx,56:$Vy,62:$VE,64:$VF}),o($VU,[2,61],{17:$Vl,22:$Vo,46:$Vp,47:$Vq,48:$Vr,49:$Vs,50:$Vt,51:$Vu,62:$VE,64:$VF}),o($VU,[2,62],{17:$Vl,22:$Vo,46:$Vp,47:$Vq,48:$Vr,49:$Vs,50:$Vt,51:$Vu,62:$VE,64:$VF}),o($VU,[2,63],{17:$Vl,22:$Vo,46:$Vp,47:$Vq,48:$Vr,49:$Vs,50:$Vt,51:$Vu,62:$VE,64:$VF}),o($VU,[2,64],{17:$Vl,22:$Vo,46:$Vp,47:$Vq,48:$Vr,49:$Vs,50:$Vt,51:$Vu,62:$VE,64:$VF}),o($VT,[2,65],{17:$Vl,19:$Vm,20:$Vn,22:$Vo,46:$Vp,47:$Vq,48:$Vr,49:$Vs,50:$Vt,51:$Vu,55:$Vx,56:$Vy,62:$VE,64:$VF}),{4:131,15:$V0,17:$V1,18:26,19:$V2,21:24,22:$V3,23:6,24:$V4,25:20,26:7,27:$V5,29:8,30:25,31:21,32:$V6,33:$V7,34:$V8,36:$V9,37:$Va,38:$Vb,39:16,40:17,41:$Vc,42:$Vd,43:$Ve,44:$Vf,45:$Vg,48:$Vh,52:$Vi,58:$Vj,65:$Vk},o($VQ,[2,68],{17:$Vl,19:$Vm,20:$Vn,22:$Vo,46:$Vp,47:$Vq,48:$Vr,49:$Vs,50:$Vt,51:$Vu,53:$Vv,54:$Vw,55:$Vx,56:$Vy,57:$Vz,58:$VA,61:$VD,62:$VE,64:$VF}),o([5,11,28,35,60],[2,69],{17:$Vl,19:$Vm,20:$Vn,22:$Vo,46:$Vp,47:$Vq,48:$Vr,49:$Vs,50:$Vt,51:$Vu,53:$Vv,54:$Vw,55:$Vx,56:$Vy,57:$Vz,58:$VA,59:$VB,61:$VD,62:$VE,64:$VF}),o($VG,[2,71]),{45:[1,132]},o($VG,[2,73]),{4:133,15:$V0,17:$V1,18:26,19:$V2,21:24,22:$V3,23:6,24:$V4,25:20,26:7,27:$V5,29:8,30:25,31:21,32:$V6,33:$V7,34:$V8,36:$V9,37:$Va,38:$Vb,39:16,40:17,41:$Vc,42:$Vd,43:$Ve,44:$Vf,45:$Vg,48:$Vh,52:$Vi,58:$Vj,65:$Vk},{17:$Vl,19:$Vm,20:$Vn,22:$Vo,28:[1,134],46:$Vp,47:$Vq,48:$Vr,49:$Vs,50:$Vt,51:$Vu,53:$Vv,54:$Vw,55:$Vx,56:$Vy,57:$Vz,58:$VA,59:$VB,60:$VC,61:$VD,62:$VE,64:$VF},o($VG,[2,27],{16:[1,135]}),o($VG,[2,70]),{11:[1,137],35:[1,136]},o($VG,[2,81]),o($VV,[2,82],{17:$Vl,19:$Vm,20:$Vn,22:$Vo,46:$Vp,47:$Vq,48:$Vr,49:$Vs,50:$Vt,51:$Vu,53:$Vv,54:$Vw,55:$Vx,56:$Vy,57:$Vz,58:$VA,59:$VB,60:$VC,61:$VD,62:$VE,64:$VF}),{17:$Vl,19:$Vm,20:$Vn,22:$Vo,35:[1,138],46:$Vp,47:$Vq,48:$Vr,49:$Vs,50:$Vt,51:$Vu,53:$Vv,54:$Vw,55:$Vx,56:$Vy,57:$Vz,58:$VA,59:$VB,60:$VC,61:$VD,62:$VE,64:$VF},{17:$Vl,19:$Vm,20:$Vn,22:$Vo,35:[1,139],46:$Vp,47:$Vq,48:$Vr,49:$Vs,50:$Vt,51:$Vu,53:$Vv,54:$Vw,55:$Vx,56:$Vy,57:$Vz,58:$VA,59:$VB,60:$VC,61:$VD,62:$VE,64:$VF},o([5,11,17,19,20,22,27,28,35,46,47,48,49,50,51,53,54,55,56,57,58,59,60,61,62,64],[2,22]),{15:$VL,18:26,19:$V2,21:117,22:$V3},o($VG,[2,23]),o($VM,[2,21],{15:$VN}),{35:$VJ},{35:$VK},o($VV,[2,40],{17:$Vl,19:$Vm,20:$Vn,22:$Vo,46:$Vp,47:$Vq,48:$Vr,49:$Vs,50:$Vt,51:$Vu,53:$Vv,54:$Vw,55:$Vx,56:$Vy,57:$Vz,58:$VA,59:$VB,60:$VC,61:$VD,62:$VE,64:$VF}),o($VV,[2,41],{17:$Vl,19:$Vm,20:$Vn,22:$Vo,46:$Vp,47:$Vq,48:$Vr,49:$Vs,50:$Vt,51:$Vu,53:$Vv,54:$Vw,55:$Vx,56:$Vy,57:$Vz,58:$VA,59:$VB,60:$VC,61:$VD,62:$VE,64:$VF}),o($VG,[2,32]),o($VG,[2,33]),{17:$Vl,19:$Vm,20:$Vn,22:$Vo,35:[1,140],46:$Vp,47:$Vq,48:$Vr,49:$Vs,50:$Vt,51:$Vu,53:$Vv,54:$Vw,55:$Vx,56:$Vy,57:$Vz,58:$VA,59:$VB,60:$VC,61:$VD,62:$VE,64:$VF},{17:$Vl,19:$Vm,20:$Vn,22:$Vo,35:[1,141],46:$Vp,47:$Vq,48:$Vr,49:$Vs,50:$Vt,51:$Vu,53:$Vv,54:$Vw,55:$Vx,56:$Vy,57:$Vz,58:$VA,59:$VB,60:$VC,61:$VD,62:$VE,64:$VF},o($VG,[2,36]),{17:$Vl,19:$Vm,20:$Vn,22:$Vo,35:[1,142],46:$Vp,47:$Vq,48:$Vr,49:$Vs,50:$Vt,51:$Vu,53:$Vv,54:$Vw,55:$Vx,56:$Vy,57:$Vz,58:$VA,59:$VB,60:$VC,61:$VD,62:$VE,64:$VF},{15:[1,143]},{15:[1,144]},{15:[1,145]},o($VT,[2,66],{17:$Vl,19:$Vm,20:$Vn,22:$Vo,46:$Vp,47:$Vq,48:$Vr,49:$Vs,50:$Vt,51:$Vu,55:$Vx,56:$Vy,62:$VE,64:$VF}),o($VG,[2,72]),{11:[1,146],17:$Vl,19:$Vm,20:$Vn,22:$Vo,46:$Vp,47:$Vq,48:$Vr,49:$Vs,50:$Vt,51:$Vu,53:$Vv,54:$Vw,55:$Vx,56:$Vy,57:$Vz,58:$VA,59:$VB,60:$VC,61:$VD,62:$VE,64:$VF},o($VG,[2,26],{16:[1,147]}),{30:148,32:$V6,33:$V7,36:$V9,37:$Va,38:$Vb,39:118,40:119,41:$Vc,42:$Vd},o($VG,[2,80]),{4:149,15:$V0,17:$V1,18:26,19:$V2,21:24,22:$V3,23:6,24:$V4,25:20,26:7,27:$V5,29:8,30:25,31:21,32:$V6,33:$V7,34:$V8,36:$V9,37:$Va,38:$Vb,39:16,40:17,41:$Vc,42:$Vd,43:$Ve,44:$Vf,45:$Vg,48:$Vh,52:$Vi,58:$Vj,65:$Vk},o($VG,[2,78]),o($VG,[2,79]),o($VG,[2,34]),o($VG,[2,35]),o($VG,[2,37]),o($VO,[2,13]),o($VR,[2,11]),o($VR,[2,12]),{4:150,15:$V0,17:$V1,18:26,19:$V2,21:24,22:$V3,23:6,24:$V4,25:20,26:7,27:$V5,29:8,30:25,31:21,32:$V6,33:$V7,34:$V8,36:$V9,37:$Va,38:$Vb,39:16,40:17,41:$Vc,42:$Vd,43:$Ve,44:$Vf,45:$Vg,48:$Vh,52:$Vi,58:$Vj,65:$Vk},{30:151,32:$V6,33:$V7,36:$V9,37:$Va,38:$Vb,39:118,40:119,41:$Vc,42:$Vd},o($VG,[2,25]),o($VV,[2,83],{17:$Vl,19:$Vm,20:$Vn,22:$Vo,46:$Vp,47:$Vq,48:$Vr,49:$Vs,50:$Vt,51:$Vu,53:$Vv,54:$Vw,55:$Vx,56:$Vy,57:$Vz,58:$VA,59:$VB,60:$VC,61:$VD,62:$VE,64:$VF}),{17:$Vl,19:$Vm,20:$Vn,22:$Vo,35:[1,152],46:$Vp,47:$Vq,48:$Vr,49:$Vs,50:$Vt,51:$Vu,53:$Vv,54:$Vw,55:$Vx,56:$Vy,57:$Vz,58:$VA,59:$VB,60:$VC,61:$VD,62:$VE,64:$VF},o($VG,[2,24]),o($VG,[2,74])],
defaultActions: {34:[2,1]},
parseError: function parseError(str, hash) {
    if (hash.recoverable) {
        this.trace(str);
    } else {
        throw new Error(str);
    }
},
parse: function parse(input) {
    var self = this, stack = [0], tstack = [], vstack = [null], lstack = [], table = this.table, yytext = '', yylineno = 0, yyleng = 0, recovering = 0, TERROR = 2, EOF = 1;
    var args = lstack.slice.call(arguments, 1);
    var lexer = Object.create(this.lexer);
    var sharedState = { yy: {} };
    for (var k in this.yy) {
        if (Object.prototype.hasOwnProperty.call(this.yy, k)) {
            sharedState.yy[k] = this.yy[k];
        }
    }
    lexer.setInput(input, sharedState.yy);
    sharedState.yy.lexer = lexer;
    sharedState.yy.parser = this;
    if (typeof lexer.yylloc == 'undefined') {
        lexer.yylloc = {};
    }
    var yyloc = lexer.yylloc;
    lstack.push(yyloc);
    var ranges = lexer.options && lexer.options.ranges;
    if (typeof sharedState.yy.parseError === 'function') {
        this.parseError = sharedState.yy.parseError;
    } else {
        this.parseError = Object.getPrototypeOf(this).parseError;
    }
    function popStack(n) {
        stack.length = stack.length - 2 * n;
        vstack.length = vstack.length - n;
        lstack.length = lstack.length - n;
    }
    _token_stack:
        function lex() {
            var token;
            token = lexer.lex() || EOF;
            if (typeof token !== 'number') {
                token = self.symbols_[token] || token;
            }
            return token;
        }
    var symbol, preErrorSymbol, state, action, a, r, yyval = {}, p, len, newState, expected;
    while (true) {
        state = stack[stack.length - 1];
        if (this.defaultActions[state]) {
            action = this.defaultActions[state];
        } else {
            if (symbol === null || typeof symbol == 'undefined') {
                symbol = lex();
            }
            action = table[state] && table[state][symbol];
        }
                    if (typeof action === 'undefined' || !action.length || !action[0]) {
                var errStr = '';
                expected = [];
                for (p in table[state]) {
                    if (this.terminals_[p] && p > TERROR) {
                        expected.push('\'' + this.terminals_[p] + '\'');
                    }
                }
                if (lexer.showPosition) {
                    errStr = 'Parse error on line ' + (yylineno + 1) + ':\n' + lexer.showPosition() + '\nExpecting ' + expected.join(', ') + ', got \'' + (this.terminals_[symbol] || symbol) + '\'';
                } else {
                    errStr = 'Parse error on line ' + (yylineno + 1) + ': Unexpected ' + (symbol == EOF ? 'end of input' : '\'' + (this.terminals_[symbol] || symbol) + '\'');
                }
                this.parseError(errStr, {
                    text: lexer.match,
                    token: this.terminals_[symbol] || symbol,
                    line: lexer.yylineno,
                    loc: yyloc,
                    expected: expected
                });
            }
        if (action[0] instanceof Array && action.length > 1) {
            throw new Error('Parse Error: multiple actions possible at state: ' + state + ', token: ' + symbol);
        }
        switch (action[0]) {
        case 1:
            stack.push(symbol);
            vstack.push(lexer.yytext);
            lstack.push(lexer.yylloc);
            stack.push(action[1]);
            symbol = null;
            if (!preErrorSymbol) {
                yyleng = lexer.yyleng;
                yytext = lexer.yytext;
                yylineno = lexer.yylineno;
                yyloc = lexer.yylloc;
                if (recovering > 0) {
                    recovering--;
                }
            } else {
                symbol = preErrorSymbol;
                preErrorSymbol = null;
            }
            break;
        case 2:
            len = this.productions_[action[1]][1];
            yyval.$ = vstack[vstack.length - len];
            yyval._$ = {
                first_line: lstack[lstack.length - (len || 1)].first_line,
                last_line: lstack[lstack.length - 1].last_line,
                first_column: lstack[lstack.length - (len || 1)].first_column,
                last_column: lstack[lstack.length - 1].last_column
            };
            if (ranges) {
                yyval._$.range = [
                    lstack[lstack.length - (len || 1)].range[0],
                    lstack[lstack.length - 1].range[1]
                ];
            }
            r = this.performAction.apply(yyval, [
                yytext,
                yyleng,
                yylineno,
                sharedState.yy,
                action[1],
                vstack,
                lstack
            ].concat(args));
            if (typeof r !== 'undefined') {
                return r;
            }
            if (len) {
                stack = stack.slice(0, -1 * len * 2);
                vstack = vstack.slice(0, -1 * len);
                lstack = lstack.slice(0, -1 * len);
            }
            stack.push(this.productions_[action[1]][0]);
            vstack.push(yyval.$);
            lstack.push(yyval._$);
            newState = table[stack[stack.length - 2]][stack[stack.length - 1]];
            stack.push(newState);
            break;
        case 3:
            return true;
        }
    }
    return true;
}};
/* generated by jison-lex 0.3.4 */
var lexer = (function(){
var lexer = ({

EOF:1,

parseError:function parseError(str, hash) {
        if (this.yy.parser) {
            this.yy.parser.parseError(str, hash);
        } else {
            throw new Error(str);
        }
    },

// resets the lexer, sets new input
setInput:function (input, yy) {
        this.yy = yy || this.yy || {};
        this._input = input;
        this._more = this._backtrack = this.done = false;
        this.yylineno = this.yyleng = 0;
        this.yytext = this.matched = this.match = '';
        this.conditionStack = ['INITIAL'];
        this.yylloc = {
            first_line: 1,
            first_column: 0,
            last_line: 1,
            last_column: 0
        };
        if (this.options.ranges) {
            this.yylloc.range = [0,0];
        }
        this.offset = 0;
        return this;
    },

// consumes and returns one char from the input
input:function () {
        var ch = this._input[0];
        this.yytext += ch;
        this.yyleng++;
        this.offset++;
        this.match += ch;
        this.matched += ch;
        var lines = ch.match(/(?:\r\n?|\n).*/g);
        if (lines) {
            this.yylineno++;
            this.yylloc.last_line++;
        } else {
            this.yylloc.last_column++;
        }
        if (this.options.ranges) {
            this.yylloc.range[1]++;
        }

        this._input = this._input.slice(1);
        return ch;
    },

// unshifts one char (or a string) into the input
unput:function (ch) {
        var len = ch.length;
        var lines = ch.split(/(?:\r\n?|\n)/g);

        this._input = ch + this._input;
        this.yytext = this.yytext.substr(0, this.yytext.length - len);
        //this.yyleng -= len;
        this.offset -= len;
        var oldLines = this.match.split(/(?:\r\n?|\n)/g);
        this.match = this.match.substr(0, this.match.length - 1);
        this.matched = this.matched.substr(0, this.matched.length - 1);

        if (lines.length - 1) {
            this.yylineno -= lines.length - 1;
        }
        var r = this.yylloc.range;

        this.yylloc = {
            first_line: this.yylloc.first_line,
            last_line: this.yylineno + 1,
            first_column: this.yylloc.first_column,
            last_column: lines ?
                (lines.length === oldLines.length ? this.yylloc.first_column : 0)
                 + oldLines[oldLines.length - lines.length].length - lines[0].length :
              this.yylloc.first_column - len
        };

        if (this.options.ranges) {
            this.yylloc.range = [r[0], r[0] + this.yyleng - len];
        }
        this.yyleng = this.yytext.length;
        return this;
    },

// When called from action, caches matched text and appends it on next action
more:function () {
        this._more = true;
        return this;
    },

// When called from action, signals the lexer that this rule fails to match the input, so the next matching rule (regex) should be tested instead.
reject:function () {
        if (this.options.backtrack_lexer) {
            this._backtrack = true;
        } else {
            return this.parseError('Lexical error on line ' + (this.yylineno + 1) + '. You can only invoke reject() in the lexer when the lexer is of the backtracking persuasion (options.backtrack_lexer = true).\n' + this.showPosition(), {
                text: "",
                token: null,
                line: this.yylineno
            });

        }
        return this;
    },

// retain first n characters of the match
less:function (n) {
        this.unput(this.match.slice(n));
    },

// displays already matched input, i.e. for error messages
pastInput:function () {
        var past = this.matched.substr(0, this.matched.length - this.match.length);
        return (past.length > 20 ? '...':'') + past.substr(-20).replace(/\n/g, "");
    },

// displays upcoming input, i.e. for error messages
upcomingInput:function () {
        var next = this.match;
        if (next.length < 20) {
            next += this._input.substr(0, 20-next.length);
        }
        return (next.substr(0,20) + (next.length > 20 ? '...' : '')).replace(/\n/g, "");
    },

// displays the character position where the lexing error occurred, i.e. for error messages
showPosition:function () {
        var pre = this.pastInput();
        var c = new Array(pre.length + 1).join("-");
        return pre + this.upcomingInput() + "\n" + c + "^";
    },

// test the lexed token: return FALSE when not a match, otherwise return token
test_match:function (match, indexed_rule) {
        var token,
            lines,
            backup;

        if (this.options.backtrack_lexer) {
            // save context
            backup = {
                yylineno: this.yylineno,
                yylloc: {
                    first_line: this.yylloc.first_line,
                    last_line: this.last_line,
                    first_column: this.yylloc.first_column,
                    last_column: this.yylloc.last_column
                },
                yytext: this.yytext,
                match: this.match,
                matches: this.matches,
                matched: this.matched,
                yyleng: this.yyleng,
                offset: this.offset,
                _more: this._more,
                _input: this._input,
                yy: this.yy,
                conditionStack: this.conditionStack.slice(0),
                done: this.done
            };
            if (this.options.ranges) {
                backup.yylloc.range = this.yylloc.range.slice(0);
            }
        }

        lines = match[0].match(/(?:\r\n?|\n).*/g);
        if (lines) {
            this.yylineno += lines.length;
        }
        this.yylloc = {
            first_line: this.yylloc.last_line,
            last_line: this.yylineno + 1,
            first_column: this.yylloc.last_column,
            last_column: lines ?
                         lines[lines.length - 1].length - lines[lines.length - 1].match(/\r?\n?/)[0].length :
                         this.yylloc.last_column + match[0].length
        };
        this.yytext += match[0];
        this.match += match[0];
        this.matches = match;
        this.yyleng = this.yytext.length;
        if (this.options.ranges) {
            this.yylloc.range = [this.offset, this.offset += this.yyleng];
        }
        this._more = false;
        this._backtrack = false;
        this._input = this._input.slice(match[0].length);
        this.matched += match[0];
        token = this.performAction.call(this, this.yy, this, indexed_rule, this.conditionStack[this.conditionStack.length - 1]);
        if (this.done && this._input) {
            this.done = false;
        }
        if (token) {
            return token;
        } else if (this._backtrack) {
            // recover context
            for (var k in backup) {
                this[k] = backup[k];
            }
            return false; // rule action called reject() implying the next rule should be tested instead.
        }
        return false;
    },

// return next match in input
next:function () {
        if (this.done) {
            return this.EOF;
        }
        if (!this._input) {
            this.done = true;
        }

        var token,
            match,
            tempMatch,
            index;
        if (!this._more) {
            this.yytext = '';
            this.match = '';
        }
        var rules = this._currentRules();
        for (var i = 0; i < rules.length; i++) {
            tempMatch = this._input.match(this.rules[rules[i]]);
            if (tempMatch && (!match || tempMatch[0].length > match[0].length)) {
                match = tempMatch;
                index = i;
                if (this.options.backtrack_lexer) {
                    token = this.test_match(tempMatch, rules[i]);
                    if (token !== false) {
                        return token;
                    } else if (this._backtrack) {
                        match = false;
                        continue; // rule action called reject() implying a rule MISmatch.
                    } else {
                        // else: this is a lexer rule which consumes input without producing a token (e.g. whitespace)
                        return false;
                    }
                } else if (!this.options.flex) {
                    break;
                }
            }
        }
        if (match) {
            token = this.test_match(match, rules[index]);
            if (token !== false) {
                return token;
            }
            // else: this is a lexer rule which consumes input without producing a token (e.g. whitespace)
            return false;
        }
        if (this._input === "") {
            return this.EOF;
        } else {
            return this.parseError('Lexical error on line ' + (this.yylineno + 1) + '. Unrecognized text.\n' + this.showPosition(), {
                text: "",
                token: null,
                line: this.yylineno
            });
        }
    },

// return next match that has a token
lex:function lex() {
        var r = this.next();
        if (r) {
            return r;
        } else {
            return this.lex();
        }
    },

// activates a new lexer condition state (pushes the new lexer condition state onto the condition stack)
begin:function begin(condition) {
        this.conditionStack.push(condition);
    },

// pop the previously active lexer condition state off the condition stack
popState:function popState() {
        var n = this.conditionStack.length - 1;
        if (n > 0) {
            return this.conditionStack.pop();
        } else {
            return this.conditionStack[0];
        }
    },

// produce the lexer rule set which is active for the currently active lexer condition state
_currentRules:function _currentRules() {
        if (this.conditionStack.length && this.conditionStack[this.conditionStack.length - 1]) {
            return this.conditions[this.conditionStack[this.conditionStack.length - 1]].rules;
        } else {
            return this.conditions["INITIAL"].rules;
        }
    },

// return the currently active lexer condition state; when an index argument is provided it produces the N-th previous condition state, if available
topState:function topState(n) {
        n = this.conditionStack.length - 1 - Math.abs(n || 0);
        if (n >= 0) {
            return this.conditionStack[n];
        } else {
            return "INITIAL";
        }
    },

// alias for begin(condition)
pushState:function pushState(condition) {
        this.begin(condition);
    },

// return the number of states currently on the stack
stateStackSize:function stateStackSize() {
        return this.conditionStack.length;
    },
options: {},
performAction: function anonymous(yy,yy_,$avoiding_name_collisions,YY_START) {
var YYSTATE=YY_START;
switch($avoiding_name_collisions) {
case 0:/* skip whitespace */
break;
case 1:return 64
break;
case 2:return 62
break;
case 3:return 58
break;
case 4:return 61
break;
case 5:return 45
break;
case 6:return 46
break;
case 7:return 47
break;
case 8:return 48
break;
case 9:return 17
break;
case 10:return 22
break;
case 11:return 54
break;
case 12:return '!'
break;
case 13:return 49
break;
case 14:return 34
break;
case 15:return 35
break;
case 16:return 27
break;
case 17:return 28
break;
case 18:return 54
break;
case 19:return 55
break;
case 20:return 56
break;
case 21:return 20
break;
case 22:return 19
break;
case 23:return 36
break;
case 24:return 42
break;
case 25:return 41
break;
case 26:return 38
break;
case 27:return 32
break;
case 28:return 33
break;
case 29:return 37
break;
case 30:return 53
break;
case 31:return 53
break;
case 32:return 57
break;
case 33:return 59
break;
case 34:return 60
break;
case 35:return 5
break;
case 36:return 44
break;
case 37:return 16
break;
case 38:return 11
break;
case 39:return 24
break;
case 40:return 15	
break;
case 41:return 43	
break;
case 42:return 43
break;
case 43:return 'INVALID'
break;
}
},
rules: [/^(?:\s+)/,/^(?:Between\b)/,/^(?:In\b)/,/^(?:Not\b)/,/^(?:Is\b)/,/^(?:Null\b)/,/^(?:\*)/,/^(?:\/)/,/^(?:-)/,/^(?:\+)/,/^(?:\^)/,/^(?:!=)/,/^(?:!)/,/^(?:%)/,/^(?:\()/,/^(?:\))/,/^(?:\[)/,/^(?:\])/,/^(?:<>)/,/^(?:>=)/,/^(?:<=)/,/^(?:>)/,/^(?:<)/,/^(?:Avg\b)/,/^(?:Max\b)/,/^(?:Min\b)/,/^(?:Single\b)/,/^(?:Count\b)/,/^(?:Exists\b)/,/^(?:Sum\b)/,/^(?:==)/,/^(?:=)/,/^(?:Like\b)/,/^(?:And\b)/,/^(?:Or\b)/,/^(?:$)/,/^(?:(?:\d*\.)?\d+)/,/^(?:\.)/,/^(?:,)/,/^(?:\?)/,/^(?:^[a-zA-Z_][a-zA-Z0-9_]*)/,/^(?:'[^']*')/,/^(?:#[^#]*#)/,/^(?:.)/],
conditions: {"INITIAL":{"rules":[0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43],"inclusive":true}}
});
return lexer;
})();
parser.lexer = lexer;
function Parser () {
  this.yy = {};
}
Parser.prototype = parser;parser.Parser = Parser;
return new Parser;
})();


if (typeof require !== 'undefined' && typeof exports !== 'undefined') {
exports.parser = parser;
exports.Parser = parser.Parser;
exports.parse = function () { return parser.parse.apply(parser, arguments); };
exports.main = function commonjsMain(args) {
    if (!args[1]) {
        console.log('Usage: '+args[0]+' FILE');
        process.exit(1);
    }
    var source = require('fs').readFileSync(require('path').normalize(args[1]), "utf8");
    return exports.parser.parse(source);
};
if (typeof module !== 'undefined' && require.main === module) {
  exports.main(process.argv.slice(1));
}
}