/// <reference path="_references.js"/>

(function () {
var ASPxClientTreeView = ASPx.CreateClass(ASPxClientControl, {
    //Constants
    DisabledCssClassName: "dxtvDisabled",
    ElbowCssClassName: "dxtv-elb",
    ElbowWithoutLineCssClassName: "dxtv-elbNoLn",
    ButtonCssClassName: "dxtv-btn",
    NodeCssClassName: "dxtv-nd",
    LineCssClassName: "dxtv-ln",
    NodeCheckboxCssClassName: "dxtv-ndChk",
    NodeImageCssClassName: "dxtv-ndImg",
    NodeTextSpanCssClassName: "dxtv-ndTxt",
    NodeTemplateCssClassName: "dxtv-ndTmpl",
    RtlCssClassName: "dxtvRtl",
    IndexPathSeparator: "_",
    NodeIDPrefix: "_N",
    NodeImageIDPostfix: "I",
    NodeLoadingPanelIDPostfix: "NLP",
    SampleExpandButtonIDPostfix: "_SEB",
    SampleCollapseButtonIDPostfix: "_SCB",
    SampleNodeLoadingPanelIDPostfix: "_SNLP",
    ControlContentDivIDPostfix: "_CD",
    NodeCheckboxIDPostfix: "_CHK",
    ExpandNodeCommand: "E",
    ExpandAllNodesCommand: "EA",
    CheckNodeRecursiveCommand: "CHKNR",
    RaiseNodeClickEventCommand: "NCLK",
    RaiseExpandedChangingEventCommand: "ECHANGING",
    RaiseCheckedChangedEventCommand: "CCHNGD",
    PostRequestArgsSeparator: "|",
    NodeClickServerEventName: "NodeClick",
    ExpandedChangingServerEventName: "ExpandedChanging",
    CheckedChangedServerEventName: "CheckedChanged",
    WidthMeasurementIncrement: 1000,
    IE6WidthMeasurementDivWidth: 99999,
    AnimationDuration: 300,
    MinAnimationDuration: 200,
    HoverCorrectionDelay: 50,

    //Ctor
    constructor: function (name) {
        this.constructor.prototype.constructor.call(this, name);

        //Server-provided settings
        this.enableAnimation = true;
        this.nodesInfo = null;
        this.nodesUrls = null;
        this.contentBoundsMode = true;
        this.checkNodesRecursive = false;
        this.allowSelectNode = false;
        this.virtualMode = false;
        this.imageProperties = null;
        this.icbFocusedStyle = [];
        this.requireWidthRecalculationOnHover = false;
        this.nodeLoadingPanelWidth = 0;
        this.nodeLoadingPanelHeight = 0;

        //Elements events handlers
        var instance = this;
        this.expandCollapseHandler = function (e) { instance.HandleExpandButtonClick(e); };
        this.nodeClickHandler = function (e) { instance.HandleNodeClick(e); };
        this.nodeDblClickHandler = function (e) { instance.HandleNodeDblClick(e); };

        //Fields
        this.rootNode = new ASPxClientTreeViewNode(this);
        this.selectedNodeContentElementID = null;
        this.requireRaiseExpandedChangedList = [];
        this.initialControlWidth = 0;

        //Events
        this.NodeClick = new ASPxClientEvent();
        this.ExpandedChanged = new ASPxClientEvent();
        this.ExpandedChanging = new ASPxClientEvent();
        this.CheckedChanged = new ASPxClientEvent();
    },

    //Utils
    ReplaceElementWithSampleElement: function (srcElement, sampleElement, copySrcInnerHtml) {
        var newElement = sampleElement.cloneNode(true);
        newElement.id = srcElement.id;
        ASPx.SetElementDisplay(newElement, true);
        if(copySrcInnerHtml && srcElement.innerHTML)
            ASPx.SetInnerHtml(newElement, srcElement.innerHTML);
        srcElement.parentNode.replaceChild(newElement, srcElement);
        return newElement;
    },

    ToggleExpandButtonClickHandler: function (attach, button) {
        var method = attach ? ASPx.Evt.AttachEventToElement : ASPx.Evt.DetachEventFromElement;
        method(button, "click", this.expandCollapseHandler);
    },

    AddElementCssClass: function (element, className) {
        this.RemoveElementCssClass(element, className);
        element.className = element.className + " " + className;
    },

    RemoveElementCssClass: function (element, className) {
        var newElementClassName = element.className.replace(className, "");
        element.className = ASPx.Str.Trim(newElementClassName);
    },

    RemoveEmptyTextNodes: function (element) {
        var nonEmptyTextPattern = /\S/;
        var textNodeType = 3;
        var elementNodeType = 1;
        for(var i = 0; i < element.childNodes.length; i++) {
            if(element.childNodes[i].nodeType == textNodeType &&
                !nonEmptyTextPattern.test(element.childNodes[i].nodeValue)) {
                element.removeChild(element.childNodes[i]);
                i--;
            } else if(element.childNodes[i].nodeType == elementNodeType)
                this.RemoveEmptyTextNodes(element.childNodes[i]);
        }
    },

    GetNodeIDByContentElementID: function (contentElementID) {
        return ASPx.Str.Trim(contentElementID.replace(this.name + "_", ""));
    },

    GetNodeIndexPathByContentElementID: function (contentElementID) {
        return ASPx.Str.Trim(contentElementID.replace(this.name + this.NodeIDPrefix, ""));
    },

    CreatePostRequestArgs: function (command, arg1, arg2) {
        var args = command + this.PostRequestArgsSeparator + arg1;
        if(arg2 || arg2 === "")
            args += this.PostRequestArgsSeparator + arg2;
        return args;
    },

    MergeNodesData: function (srcData, destData) {
        for(var key in srcData)
            destData[key] = srcData[key];
    },

    GetNodeListItem: function (clientNode) {
        var contentElement = ASPx.GetElementById(clientNode.contentElementID);
        return ASPx.GetParentByTagName(contentElement, "LI");
    },

    SerializeBooleanValue: function (value) {
        return value ? "T" : "";
    },

    GetContentElementByNodeID: function (nodeID) {
        return ASPx.GetElementById(this.name + "_" + nodeID);
    },

    GetListItems: function (list) {
        var listNodes = ASPx.GetChildElementNodes(list);
        var listItems = [];
        if(!listNodes)
            return null;
        for(var i = 0; i < listNodes.length; i++) {
            if(listNodes[i].tagName == "LI")
                listItems.push(listNodes[i]);
        }
        return listItems;
    },

    GetClickedContentElementByEventArgs: function (e) {
        var clickedElement = ASPx.Evt.GetEventSource(e);
        var contentElement = ASPx.GetParentByClassName(clickedElement, this.NodeCssClassName);
        if(ASPx.ElementContainsCssClass(clickedElement, this.NodeCheckboxCssClassName)) {
            //B158058
            if(ASPx.Browser.Firefox) {
                ASPx.Evt.PreventEventAndBubble(e);
                var instance = this;
                var internalCheckBox = this.internalCheckBoxCollection.Get(contentElement.id);
                window.setTimeout(function () {
                    ASPx.CheckableElementHelper.Instance.InvokeClick(internalCheckBox, e);
                });
            }
            return null;
        }
        if(!contentElement || ASPx.ElementContainsCssClass(contentElement, this.DisabledCssClassName))
            return null;
        return contentElement;
    },

    IsRightToLeft: function () {
        return ASPx.IsElementRightToLeft(this.GetMainElement());
    },

    //Initialization
    InlineInitialize: function () {
        ASPxClientControl.prototype.InlineInitialize.call(this);

        this.SetInitialControlWidth();
        if(this.imageProperties)
            this.CreateInternalCheckBoxCollection();
        this.InitializeControlElements();
    },
    Initialize: function () {
        ASPxClientControl.prototype.Initialize.call(this);
        this.InitLoadingPanelSize();
        this.AssignEllipsisToolTips();
    },

    AdjustControlCore: function () {
        this.CorrectControlWidth();
    },
    CreateInternalCheckBoxCollection: function () {
        this.internalCheckBoxCollection = new ASPx.CheckBoxInternalCollection(this.imageProperties, true);
    },
    AddInternalCheckBoxToCollection: function (icbInputElement, contentElement) {
        var instance = this;
        var internalCheckBox = this.internalCheckBoxCollection.Add(contentElement.id, icbInputElement);
        internalCheckBox.SetEnabled(!this.IsNodeDisabled(contentElement));
        internalCheckBox.readOnly = this.readOnly;
        internalCheckBox.CreateFocusDecoration(this.icbFocusedStyle);
        internalCheckBox.CheckedChanged.AddHandler(
            function (s, e) {
                instance.UpdateCheckedInNodesState(s.inputElement, true);
                instance.OnNodeCheckboxClick(s.inputElement);
            }
        );
    },

    InitLoadingPanelSize: function () {
        var panel = this.GetSampleNodeLoadingPanel();
        if(panel) {
            this.nodeLoadingPanelWidth = panel.offsetWidth;
            this.nodeLoadingPanelHeight = panel.offsetHeight;
            ASPx.SetElementDisplay(panel, false);
            panel.style.visibility = "";
            panel.style.position = "";
        }
    },
    InitializeControlElements: function () {
        var mainElement = this.GetMainElement();
        //B159764
        if(ASPx.ElementContainsCssClass(mainElement, this.DisabledCssClassName))
            return;
        var rootList = ASPx.GetNodeByTagName(mainElement, "UL", 0);
        this.InitializeNodeContainerElements(rootList, "", null);
        this.InitializeNodeSelection();
    },

    InitializeNodeSelection: function () {
        var contentElement = this.GetContentElementByNodeID(this.GetSelectedNodeIDFromState());
        if(contentElement)
            this.ApplySelectionToNode(contentElement);
        else { //T103674
            var firstNodeId = this.NodeIDPrefix.replace("_", "") + this.GetNodeIndexPath(0, 0);
            ASPx.GetStateController().DeselectElementBySrcElement(this.GetContentElementByNodeID(firstNodeId));
        }
    },

    GetNodeIndexPath: function (listItemIndex, parentIndexPath) {
        if(!parentIndexPath)
            return listItemIndex.toString();
        return parentIndexPath + this.IndexPathSeparator + listItemIndex.toString();
    },

    GetElementID: function (IDPrefix, indexPath) {
        return this.name + IDPrefix + indexPath;
    },

    ApplyServerProvidedClientNodeInfo: function (clientNode, indexPath) {
        if(!this.nodesInfo[indexPath])
            return;
        clientNode.clientEnabled = !!this.nodesInfo[indexPath][0];
        clientNode.clientVisible = !!this.nodesInfo[indexPath][1];
        clientNode.name = this.nodesInfo[indexPath][2];
        clientNode.navigateUrl = this.nodesInfo[indexPath][3];
        clientNode.target = this.nodesInfo[indexPath][4];
    },

    CreateClientNode: function (index, parentClientNode, indexPath) {
        if(!this.nodesInfo)
            return null;
        var clientNode = new ASPxClientTreeViewNode();
        clientNode.treeView = this;
        clientNode.parent = parentClientNode;
        clientNode.index = index;
        this.ApplyServerProvidedClientNodeInfo(clientNode, indexPath, null);
        if(parentClientNode)
            parentClientNode.nodes.push(clientNode);
        else
            this.rootNode.nodes.push(clientNode);
        return clientNode;
    },

    GetServerProvidedNodeIndex: function (listItem) {
        var nodeContentElement = ASPx.GetNodesByPartialClassName(listItem, this.NodeCssClassName)[0];
        if(!nodeContentElement || !nodeContentElement.id)
            return null;
        var nodeIndexPath = this.GetNodeIndexPathByContentElementID(nodeContentElement.id);
        var nodeIndices = nodeIndexPath.split(this.IndexPathSeparator);
        return parseInt(nodeIndices[nodeIndices.length - 1]);
    },

    InitializeNodeContainerElements: function (nodeContainer, parentIndexPath, parentClientNode) {
        var listItems = this.GetListItems(nodeContainer);
        if(!listItems)
            return;
        for(var i = 0, nodeIndex = 0; i < listItems.length; i++, nodeIndex++) {
            var serverProvidedNodeIndex = this.GetServerProvidedNodeIndex(listItems[i]);
            if(serverProvidedNodeIndex)
                nodeIndex = serverProvidedNodeIndex;
            var nodeIndexPath = this.GetNodeIndexPath(nodeIndex, parentIndexPath);
            var clientNode = this.CreateClientNode(i, parentClientNode, nodeIndexPath);
            if(clientNode)
                clientNode.last = i == listItems.length - 1;
            this.InitializeNodeContent(listItems[i], nodeIndexPath, clientNode);
            this.InitializeExpandButton(listItems[i]);
            if(clientNode) {
                if(!clientNode.clientVisible)
                    this.SetNodeClientVisible(clientNode, false);
                if(!clientNode.clientEnabled)
                    this.SetNodeClientEnabled(clientNode, false);
            }
            var subnodesList = ASPx.GetNodeByTagName(listItems[i], "UL", 0);
            if(subnodesList)
                this.InitializeNodeContainerElements(subnodesList, nodeIndexPath, clientNode);
        }
    },

    InitializeExpandButton: function (listItem) {
        var button = ASPx.GetNodesByPartialClassName(listItem, this.ButtonCssClassName)[0];
        if(button && !ASPx.ElementContainsCssClass(button, this.DisabledCssClassName))
            this.ToggleExpandButtonClickHandler(true, button);
    },

    InitializeNodeContent: function (listItem, nodeIndexPath, clientNode) {
        var nodeContentElement = ASPx.GetNodesByPartialClassName(listItem, this.NodeCssClassName)[0];
        if(!nodeContentElement)
            return;
        var contentElementID = this.GetElementID(this.NodeIDPrefix, nodeIndexPath);
        if(clientNode) {
            clientNode.enabled = !this.IsNodeDisabled(nodeContentElement);
            clientNode.contentElementID = contentElementID;
            var textSpan = ASPx.GetNodesByPartialClassName(nodeContentElement, this.NodeTextSpanCssClassName)[0];
            clientNode.text = textSpan ? ASPx.GetInnerText(textSpan) : "";
        }
        nodeContentElement.id = contentElementID;
        if(clientNode && clientNode.last && ASPx.Browser.IE) {
            //Q482883
            var listItem = ASPx.GetParentByTagName(nodeContentElement, "LI");
            var clearElement = ASPx.GetNodeByTagName(listItem, "B", 0);
            if(clearElement && clearElement.style.display == "inline")
                clearElement.style.display = "";
        }
        ASPx.AssignAccessabilityEventsToLink(nodeContentElement);

        ASPx.Evt.AttachEventToElement(nodeContentElement, "click", this.nodeClickHandler);
        ASPx.Evt.AttachEventToElement(nodeContentElement, "dblclick", this.nodeDblClickHandler);
        if(this.contentBoundsMode) {
            var instance = this;
            ASPx.Evt.AttachEventToElement(nodeContentElement, "mousemove", function (evt) {
                instance.CorrectControlWidthOnHover(evt.target);
            });
        }
        this.InitializeNodeImage(nodeContentElement, clientNode);

        var icbMainElement = ASPx.GetNodesByPartialClassName(nodeContentElement, this.NodeCheckboxCssClassName)[0];
        if(icbMainElement) {
            var icbInputElement = ASPx.GetNodeByTagName(icbMainElement, "input", 0);
            icbInputElement.id = this.GetElementID(this.NodeCheckboxIDPostfix, nodeIndexPath);
            if(this.imageProperties)
                this.AddInternalCheckBoxToCollection(icbInputElement, nodeContentElement);
        }
    },
    IsNodeDisabled: function (nodeContentElement) {
        return ASPx.ElementContainsCssClass(nodeContentElement, this.DisabledCssClassName);
    },

    InitializeNodeImage: function (nodeContentElement, clientNode) {
        var nodeImage = ASPx.GetNodesByPartialClassName(nodeContentElement, this.NodeImageCssClassName)[0];
        if(!nodeImage)
            return;
        nodeImage.id = nodeContentElement.id + this.NodeImageIDPostfix;
        if(!ASPx.Browser.IE) {
            var nodeImageLoaded = nodeImage.naturalWidth !== 0 && nodeImage.naturalHeight !== 0 && nodeImage.complete;
            if(!nodeImageLoaded) {
                var instance = this;
                ASPx.Evt.AttachEventToElement(nodeImage, "load", function (evt) {
                    instance.CorrectControlWidth();
                });
                if(ASPx.Browser.WebKitFamily)
                    ASPx.Evt.AttachEventToElement(nodeImage, "error", function (evt) {
                        instance.CorrectControlWidth();
                    });
            }
        }
        if(clientNode)
            clientNode.imageUrl = nodeImage.src;
    },

    //Width correction
    GetControlContentDiv: function () {
        return ASPx.GetElementById(this.name + this.ControlContentDivIDPostfix);
    },

    SetInitialControlWidth: function () {
        var mainElement = this.GetMainElement();
        if(mainElement.style.width && !ASPx.IsPercentageSize(mainElement.style.width))
            this.initialControlWidth = mainElement.offsetWidth;
    },

    //Q382036
    CorrectControlWidthOnHover: function (nodeElement) {
        //Q413109
        if(!this.requireWidthRecalculationOnHover)
            return;

        if(ASPx.Browser.IE && ASPx.Browser.Version < 9) {
            var instance = this;
            window.setTimeout(function () {
                instance.CorrectControlWidth();
            }, 0);
            return;
        }

        if(nodeElement.lastCorrectionOnHover && (new Date().getTime() - nodeElement.lastCorrectionOnHover <= this.HoverCorrectionDelay))
            return;

        this.CorrectControlWidth();

        nodeElement.lastCorrectionOnHover = new Date().getTime();
    },

    CorrectControlWidth: function () {
        var mainElement = this.GetMainElement();
        var controlContentDiv = this.GetControlContentDiv();
        var rootNodesList = ASPx.GetNodeByTagName(controlContentDiv, "UL", 0);
        if(!rootNodesList)
            return;

        mainElement.style.overflow = "hidden";
        ASPx.SetElementFloat(rootNodesList, "left");
        var prevListWidth = 0;
        do {
            prevListWidth = rootNodesList.offsetWidth;
            ASPx.SetOffsetWidth(controlContentDiv, controlContentDiv.offsetWidth + this.WidthMeasurementIncrement);
            //B186031
            if(ASPx.Browser.Opera) {
                //HACK: this forces UL DOM measures update in Opera (argghh!!)
                rootNodesList.style.width = "100%";
                var dummy = rootNodesList.offsetWidth;
                rootNodesList.style.width = "";
            }
        } while(prevListWidth != rootNodesList.offsetWidth);

        if(rootNodesList.offsetWidth > this.initialControlWidth) {
            var needRoundingCorrection = ASPx.Browser.HardwareAcceleration || // HACK: IE9 uses fractional number in size, but returns rounded-off offsetWidth + T145573
                                         ASPx.Browser.MacOSMobilePlatform;               // HACK: T173786
            var roundingCorrection = needRoundingCorrection ? 1 : 0; 
            var isPercentageWidth = ASPx.IsPercentageSize(mainElement.style.width);
            mainElement.style[isPercentageWidth ? "minWidth" : "width"] = rootNodesList.offsetWidth + roundingCorrection + "px";
        }
        ASPx.SetElementFloat(rootNodesList, "");
        mainElement.style.overflow = "";
        controlContentDiv.style.width = "";
    },

    //Nodes representative state
    GetPreviousSiblingNodeListItem: function (clientNode) {
        var previousNodeIndex = clientNode.index - 1;
        if(previousNodeIndex < 0)
            return null;
        var previousClientNode = clientNode.parent ? clientNode.parent.GetNode(previousNodeIndex) :
            this.rootNode.nodes[previousNodeIndex];
        return this.GetNodeListItem(previousClientNode);
    },

    GetListItemElbowSpan: function (listItem) {
        return ASPx.GetNodesByPartialClassName(listItem, this.ElbowCssClassName)[0] ||
                ASPx.GetNodesByPartialClassName(listItem, this.ElbowWithoutLineCssClassName)[0];
    },

    SetNodeClientVisible: function (clientNode, clientVisible) {
        if(this.IsRootNode(clientNode))
            return;
        var listItem = this.GetNodeListItem(clientNode);
        ASPx.SetElementDisplay(listItem, clientVisible);
        this.CorrectControlWidth();
        //NOTE: replace previous node elbow ifthis is a last node
        if(!clientNode.last)
            return;
        var previousNodeListItem = this.GetPreviousSiblingNodeListItem(clientNode);
        if(!previousNodeListItem)
            return;
        var previousNodeElbowSpan = this.GetListItemElbowSpan(previousNodeListItem);
        if(ASPx.ElementContainsCssClass(previousNodeElbowSpan, this.ElbowWithoutLineCssClassName))
            return;
        this.InitializeExpandButton(previousNodeListItem);
        if(clientVisible)
            this.AddElementCssClass(previousNodeListItem, this.LineCssClassName);
        else
            this.RemoveElementCssClass(previousNodeListItem, this.LineCssClassName);
    },

    SetNodeClientEnabled: function (clientNode, clientEnabled) {
        if(this.IsRootNode(clientNode))
            return;
        var contentElement = ASPx.GetElementById(clientNode.contentElementID);
        ASPx.GetStateController().SetElementEnabled(contentElement, clientEnabled);
        if(this.internalCheckBoxCollection) {
            var internalCheckBox = this.internalCheckBoxCollection.Get(contentElement.id);
            if(internalCheckBox)
                internalCheckBox.SetEnabled(clientEnabled);
        }
        if(clientNode.navigateUrl) {
            var nodeLink = this.GetNodeLinkElement(clientNode);
            if(!clientEnabled && nodeLink.href) {
                ASPx.Attr.SetAttribute(nodeLink, "savedhref", nodeLink.href);
                ASPx.Attr.RemoveAttribute(nodeLink, "href");
            }
            else if(clientEnabled && ASPx.Attr.GetAttribute(nodeLink, "savedhref")) {
                ASPx.Attr.SetAttribute(nodeLink, "href", ASPx.Attr.GetAttribute(nodeLink, "savedhref"));
                ASPx.Attr.RemoveAttribute(nodeLink, "savedhref");
            }
        }
        var listItem = ASPx.GetParentByTagName(contentElement, "LI");
        var button = ASPx.GetNodesByPartialClassName(listItem, this.ButtonCssClassName)[0];
        if(button) {
            this.ToggleExpandButtonClickHandler(clientEnabled, button);
            button.style.cursor = clientEnabled ? "pointer" : "default";
        }
    },

    //Node data
    SetNodeText: function (clientNode, text) {
        if(this.IsRootNode(clientNode))
            return;
        var contentElement = ASPx.GetElementById(clientNode.contentElementID);
        var textSpan = ASPx.GetNodesByPartialClassName(contentElement, this.NodeTextSpanCssClassName)[0];
        if(textSpan) {
            ASPx.SetInnerHtml(textSpan, text);
            this.CorrectControlWidth();
        }
    },

    SetNodeImageUrl: function (clientNode, url) {
        if(this.IsRootNode(clientNode))
            return;
        var contentElement = ASPx.GetElementById(clientNode.contentElementID);
        var nodeImage = ASPx.GetNodesByPartialClassName(contentElement, this.NodeImageCssClassName)[0];
        if(nodeImage)
            nodeImage.src = url;
    },

    GetNodeLinkElement: function (clientNode) {
        var contentElement = ASPx.GetElementById(clientNode.contentElementID);
        return this.GetNodeLinkElementCore(contentElement);
    },

    GetNodeLinkElementCore: function(contentElement) {
        if(ASPx.ElementContainsCssClass(contentElement, this.NodeTemplateCssClassName))
            return null;
        return contentElement.tagName == "A" ? contentElement : ASPx.GetNodeByTagName(contentElement, "A", 0);
    },

    GetNodeNavigateUrl: function (clientNode) {
        if(this.contentBoundsMode || this.IsRootNode(clientNode))
            return clientNode.navigateUrl || "";
        var nodeLink = this.GetNodeLinkElement(clientNode);
        return nodeLink ? (nodeLink.href || ASPx.Attr.GetAttribute(nodeLink, "savedhref")) : "";
    },

    SetNodeNavigateUrl: function (clientNode, url) {
        if(this.IsRootNode(clientNode))
            return;
        var nodeLink = this.GetNodeLinkElement(clientNode);
        if(nodeLink) {
            if(ASPx.Attr.IsExistsAttribute(nodeLink, "savedhref"))
                ASPx.Attr.SetAttribute(nodeLink, "savedhref", url);
            else if(ASPx.Attr.IsExistsAttribute(nodeLink, "href"))
                nodeLink.href = url;
            clientNode.navigateUrl = url;
        }
    },

    //Node check
    UpdateCheckedInNodesState: function (checkbox, needToSaveToCookies) {
        var contentElement = ASPx.GetParentByClassName(checkbox, this.NodeCssClassName);
        var internalCheckBox = this.internalCheckBoxCollection.Get(contentElement.id);
        var value = internalCheckBox.GetCurrentInputKey();
        var nodeID = this.GetNodeIDByContentElementID(contentElement.id);
        this.nodesState[2][nodeID] = value;
        if(needToSaveToCookies)
            this.UpdateNodesStateCookies();
    },
    UpdateRecursiveCheckedNodesStateOnCallback: function (checkState, nodeIDs) {
        this.HideLoadingPanel();
        for(var i = 0; i < nodeIDs.length; i++) {
            if(this.nodesState[2][nodeIDs[i]] != checkState)
                this.nodesState[2][nodeIDs[i]] = checkState;
        }
        this.UpdateNodesStateCookies();
    },

    CreateRecursiveNodeCheckRequest: function (contentElement) {
        var requestArgs = this.CreatePostRequestArgs(this.CheckNodeRecursiveCommand,
                this.GetNodeIDByContentElementID(contentElement.id));
        if(this.autoPostBack) {
            if(this.isInitialized)
                this.SendPostBack(requestArgs);
            return;
        }
        if(this.callBack) {
            var mainElement = this.GetMainElement();
            this.CreateLoadingDiv(mainElement);
            this.CreateLoadingPanelWithAbsolutePosition(mainElement);
            this.CreateCallback(requestArgs);
        }
    },

    OnNodeCheckboxClick: function (checkbox) {
        var contentElement = ASPx.GetParentByClassName(checkbox, this.NodeCssClassName);
        var internalCheckBox = this.internalCheckBoxCollection.Get(contentElement.id);
        if(this.checkNodesRecursive) {
            var checkBoxState = internalCheckBox.GetCurrentInputKey();
            this.UpdateCheckedStateRecursive(checkbox, checkBoxState);
        }
        var processOnServer = this.RaiseCheckedChanged(contentElement);
        if(this.checkNodesRecursive && !this.virtualMode && (this.autoPostBack || this.callBack))
            this.CreateRecursiveNodeCheckRequest(contentElement);
        else if(processOnServer) {
            var requestArgs = this.CreatePostRequestArgs(this.RaiseCheckedChangedEventCommand,
                this.GetNodeIDByContentElementID(contentElement.id));
            if(this.isInitialized)
                this.SendPostBack(requestArgs);
        }
    },

    UpdateCheckedStateRecursive: function (checkbox, state) {
        var listItem = ASPx.GetParentByTagName(checkbox, "LI");
        this.UpdateDescendantsCheckedState(listItem, state);
        this.UpdateAncestorsCheckedState(listItem);
        this.UpdateNodesStateCookies();
    },

    UpdateDescendantsCheckedState: function (listItem, state) {
        var subnodesList = ASPx.GetNodeByTagName(listItem, "UL", 0);
        if(!subnodesList) {
            listItem.requireUpdateCheckedState = true;
            listItem.checkedState = state;
            return;
        }
        var descendantListItems = this.GetListItems(subnodesList);
        for(var i = 0; i < descendantListItems.length; i++) {
            var contentElement = ASPx.GetNodesByPartialClassName(descendantListItems[i], this.NodeCssClassName)[0];
            var internalCheckBox = this.internalCheckBoxCollection.Get(contentElement.id);
            if(internalCheckBox && internalCheckBox.GetValue() != state) {
                internalCheckBox.SetValue(state);
                this.UpdateCheckedInNodesState(internalCheckBox.inputElement);
            }
            this.UpdateDescendantsCheckedState(descendantListItems[i], state);
        }
    },

    UpdateAncestorsCheckedState: function (listItem) {
        var parentSubnodesList = ASPx.GetParentByTagName(listItem, "UL");
        var parentListItem = ASPx.GetParentByTagName(parentSubnodesList, "LI");
        //NOTE: ensure that we are still inside the control
        if(!parentListItem || !ASPx.GetParentById(parentListItem, this.name))
            return;
        var parentChecked = true;
        var parentUnchecked = true;
        var siblingListItems = this.GetListItems(parentSubnodesList);
        for(var i = 0; i < siblingListItems.length; i++) {
            var contentElement = ASPx.GetNodesByPartialClassName(siblingListItems[i], this.NodeCssClassName)[0];
            var internalCheckBox = this.internalCheckBoxCollection.Get(contentElement.id);
            if(internalCheckBox) {
                var currentCheckBoxState = internalCheckBox.GetCurrentInputKey();
                if(currentCheckBoxState != ASPx.CheckBoxInputKey.Checked)
                    parentChecked = false;
                if(currentCheckBoxState != ASPx.CheckBoxInputKey.Unchecked)
                    parentUnchecked = false;
            }
        }
        var parentCheckboxContentElement = ASPx.GetNodesByPartialClassName(parentListItem, this.NodeCssClassName)[0];
        if(parentCheckboxContentElement) {
            var parentCheckBoxState = parentChecked ? ASPx.CheckBoxInputKey.Checked : (parentUnchecked ? ASPx.CheckBoxInputKey.Unchecked : ASPx.CheckBoxInputKey.Indeterminate);
            var parentInternalCheckBox = this.internalCheckBoxCollection.Get(parentCheckboxContentElement.id);
            if(parentInternalCheckBox && parentInternalCheckBox.GetValue() != parentCheckBoxState) {
                parentInternalCheckBox.SetValue(parentCheckBoxState);
                this.UpdateCheckedInNodesState(parentInternalCheckBox.inputElement);
            }
        }
        this.UpdateAncestorsCheckedState(parentListItem);
    },

    SetNodeState: function (clientNode, state) {
        if(this.IsRootNode(clientNode))
            return;
        var contentElement = ASPx.GetElementById(clientNode.contentElementID);
        var checkbox = ASPx.GetNodesByPartialClassName(contentElement, this.NodeCheckboxCssClassName)[0];
        var stateKey = ASPx.CheckBoxInputKey[state];
        var internalCheckBox = this.internalCheckBoxCollection.Get(contentElement.id);
        if(internalCheckBox && internalCheckBox.GetValue() != stateKey) {
            internalCheckBox.SetValue(stateKey);
            this.UpdateCheckedInNodesState(internalCheckBox.inputElement, true);
        }
        if(this.checkNodesRecursive && checkbox)
            this.UpdateCheckedStateRecursive(checkbox, stateKey);
    },

    GetNodeState: function (clientNode) {
        if(!this.internalCheckBoxCollection)
            return ASPx.CheckBoxCheckState.Unchecked;
        var contentElement = ASPx.GetElementById(clientNode.contentElementID);
        var internalCheckBox = this.internalCheckBoxCollection.Get(contentElement.id);
        return internalCheckBox ? internalCheckBox.GetCurrentCheckState() : ASPx.CheckBoxCheckState.Unchecked;
    },

    //Node click and selection
    GetSelectedNodeIDFromState: function () {
        return this.nodesState ? this.nodesState[1] : "";
    },

    SetSelectedNodeIDToState: function (nodeID) {
        if(!this.nodesState || !ASPx.IsExists(this.nodesState[1]))
            return;
        this.nodesState[1] = nodeID;
        this.UpdateNodesStateCookies();
    },

    NavigateToNodeLink: function (contentElementID) {
        var navigateUrl = null;
        var target = null;
        if(this.rootNode.nodes.length > 0) {
            var clientNode = this.rootNode.GetNodeByContentElementID(contentElementID);
            navigateUrl = clientNode.navigateUrl;
            target = clientNode.target;
        } else {
            var nodeIndexPath = this.GetNodeIndexPathByContentElementID(contentElementID);
            if(!(this.nodesUrls && this.nodesUrls[nodeIndexPath]))
                return;
            navigateUrl = this.nodesUrls[nodeIndexPath][0];
            target = this.nodesUrls[nodeIndexPath][1];
        }
        if(navigateUrl)
            ASPx.Url.Navigate(navigateUrl, target || "");
    },

    ApplySelectionToNode: function (contentElement) {
        var previouslySelectedContentElement = ASPx.GetElementById(this.selectedNodeContentElementID);
        if(previouslySelectedContentElement)
            ASPx.GetStateController().DeselectElementBySrcElement(previouslySelectedContentElement);
        if(contentElement) {
            ASPx.GetStateController().SelectElementBySrcElement(contentElement);
            this.CorrectControlWidth();
            this.selectedNodeContentElementID = contentElement.id;
        } else
            this.selectedNodeContentElementID = null;
    },

    HandleNodeClick: function (e) {
        var contentElement = this.GetClickedContentElementByEventArgs(e);
        if(!contentElement)
            return;
        if(this.allowSelectNode && this.selectedNodeContentElementID != contentElement.id) {
            this.SetSelectedNodeIDToState(this.GetNodeIDByContentElementID(contentElement.id));
            ASPx.ClearHoverState();
            this.ApplySelectionToNode(contentElement);
            ASPx.UpdateHoverState(e);
        }
        var processEventOnServer = this.RaiseNodeClick(contentElement, e);
        if(contentElement.tagName != "A")
            this.NavigateToNodeLink(contentElement.id);
        if(processEventOnServer && !this.GetNodeLinkElementCore(contentElement)) {
            var postbackArgs = this.CreatePostRequestArgs(this.RaiseNodeClickEventCommand,
                this.GetNodeIDByContentElementID(contentElement.id));
            if(this.isInitialized)
                this.SendPostBack(postbackArgs);
        }
    },

    //Callbacks
    OnCallback: function (resultObj) {
        if(!resultObj)
            return;
        switch (resultObj[0]) {
            case this.ExpandNodeCommand:
                if(resultObj[1])
                    this.ProcessNodeExpandingOnCallback(resultObj[1], resultObj[2], resultObj[3], resultObj[4]);
                else
                    this.ProcessAllNodesExpandingOnCallback(resultObj[2], resultObj[3], resultObj[4]);
                break;
            case this.CheckNodeRecursiveCommand:
                this.UpdateRecursiveCheckedNodesStateOnCallback(resultObj[1], resultObj[2]);
                break;
        }
        if(resultObj[5])
            this.UpdateNodesStateCookies();
    },
    OnCallbackFinalized: function() {
        this.InitializeNodeSelection();
    },

    //Node loading panel
    GetSampleNodeLoadingPanel: function () {
        return ASPx.GetElementById(this.name + this.SampleNodeLoadingPanelIDPostfix);
    },

    GetNodeLoadingPanelID: function (nodeID) {
        return this.name + this.NodeLoadingPanelIDPostfix + nodeID;
    },

    ShowNodeLoadingPanel: function (button, nodeID) {
        var sampleLoadingPanel = this.GetSampleNodeLoadingPanel();
        if(!sampleLoadingPanel || !button) {
            var mainElement = this.GetMainElement();
            this.CreateLoadingDiv(mainElement);
            this.CreateLoadingPanelWithAbsolutePosition(mainElement);
            return;
        }
        var elbowSpan = ASPx.GetParentByTagName(button, "SPAN");

        var panel = this.CloneNodeLoadingPanel(sampleLoadingPanel, nodeID, elbowSpan);
        ASPx.SetElementDisplay(panel, true);
        this.SetNodeLoadingPanelPosition(panel, button, elbowSpan);
        ASPx.SetElementDisplay(button, false);
    },

    HideNodeLoadingPanel: function (nodeID) {
        var panel = ASPx.GetElementById(this.GetNodeLoadingPanelID(nodeID));
        if(panel)
            ASPx.RemoveElement(panel);
    },

    SetNodeLoadingPanelPosition: function (panel, button, elbowSpan) {
        elbowSpan.style.position = "relative";
        //NOTE: now elbowSpan is the offsetParent of the button
        var buttonStyle = ASPx.GetCurrentStyle(button);
        var marginLeft = parseInt(buttonStyle.marginLeft);
        var marginTop = parseInt(buttonStyle.marginTop);
        var leftOffset = marginLeft - Math.round((this.nodeLoadingPanelWidth - button.offsetWidth) / 2);
        var topOffset = marginTop - Math.round((this.nodeLoadingPanelHeight - button.offsetHeight) / 2);
        elbowSpan.style.position = "";
        if(this.IsRightToLeft())
            panel.style.marginRight = elbowSpan.offsetWidth - this.nodeLoadingPanelWidth - leftOffset + "px";
        else
            panel.style.marginLeft = leftOffset + "px";
        panel.style.marginTop = topOffset + "px";
    },

    CloneNodeLoadingPanel: function (sampleLoadingPanel, nodeID, elbowSpan) {
        var clonedPanel = sampleLoadingPanel.cloneNode(true);
        clonedPanel.id = this.GetNodeLoadingPanelID(nodeID);
        clonedPanel.dir = "ltr";
        elbowSpan.appendChild(clonedPanel);
        return clonedPanel;
    },

    //Expand-Collapse
    GetSampleExpandButton: function () {
        return ASPx.GetElementById(this.name + this.SampleExpandButtonIDPostfix);
    },

    GetSampleCollapseButton: function () {
        return ASPx.GetElementById(this.name + this.SampleCollapseButtonIDPostfix);
    },

    SetExpandedToState: function (nodeID, expanded) {
        this.nodesState[0][nodeID] = this.SerializeBooleanValue(expanded);
        this.UpdateNodesStateCookies();
    },

    GetExpandedDataFromState: function () {
        return this.nodesState[0];
    },

    HandleNodeDblClick: function (e) {
        var contentElement = this.GetClickedContentElementByEventArgs(e);
        if(!contentElement)
            return;
        var listItem = ASPx.GetParentByTagName(contentElement, "LI");
        var button = ASPx.GetNodesByPartialClassName(listItem, this.ButtonCssClassName)[0];
        var expandedState = this.GetExpandedDataFromState();
        var nodeID = this.GetNodeIDByContentElementID(contentElement.id);
        if(!button && !ASPx.IsExists(expandedState[nodeID]))
            return;
        this.ProcessUserInitiatedExpandCollapse(button, listItem);
    },

    InsertSubnodesListMarkup: function (listItem, subnodesListMarkup) {
        //NOTE: we are using fake div to obtain html element from retrieved markup.
        //This approach helps to avoid events and input values reset, which happens
        //ifwe are using ASPx.SetInnerHtml forlistItem.
        var fakeDiv = document.createElement("DIV");
        ASPx.SetInnerHtml(fakeDiv, subnodesListMarkup);
        while(fakeDiv.childNodes.length != 0)
            listItem.appendChild(fakeDiv.childNodes[0]);
    },

    ProcessAllNodesExpandingOnCallback: function (subnodesListMarkup, nodesData, nodeNames) {
        this.HideLoadingPanel();
        for(var nodeIndexPath in nodesData)
            this.nodesState[0][this.NodeIDPrefix.replace("_", "") + nodeIndexPath] = 'T';
        this.UpdateNodesStateCookies();
        this.MergeRetrievedOnCallbackNodeInfo(nodesData, nodeNames);
        var contentDiv = this.GetControlContentDiv();
        contentDiv.innerHTML = subnodesListMarkup;
        var subnodesList = ASPx.GetNodeByTagName(contentDiv, "UL", 0);
        this.rootNode.nodes = [];
        this.InitializeNodeContainerElements(subnodesList, "", null);
        this.AdjustControl();
    },

    MergeRetrievedOnCallbackNodeInfo: function (nodesData, nodeNames) {
        if(nodesData) {
            var destData = this.nodesInfo || this.nodesUrls;
            this.MergeNodesData(nodesData, destData);
        }
        if(nodeNames) {
            this.MergeNodesData(nodeNames, this.nodesState[3]);
            this.UpdateNodesStateCookies();
        }
    },

    ProcessNodeExpandingOnCallback: function (nodeID, subnodesListMarkup, nodesData, nodeNames) {
        this.MergeRetrievedOnCallbackNodeInfo(nodesData, nodeNames);
        var contentElement = this.GetContentElementByNodeID(nodeID);
        contentElement.performingExpandNodeRequest = false;
        this.HideNodeLoadingPanel(this.GetNodeIDByContentElementID(contentElement.id));
        var listItem = ASPx.GetParentByTagName(contentElement, "LI");
        this.InsertSubnodesListMarkup(listItem, subnodesListMarkup);
        var subnodesList = ASPx.GetNodeByTagName(listItem, "UL", 0);
        var nodeIndexPath = this.GetNodeIndexPathByContentElementID(contentElement.id);
        var clientNode = null;
        if(this.rootNode.nodes.length > 0)
            clientNode = this.rootNode.GetNodeByContentElementID(contentElement.id);
        this.InitializeNodeContainerElements(subnodesList, nodeIndexPath, clientNode);
        if(listItem.requireUpdateCheckedState)
            this.UpdateDescendantsCheckedState(listItem, listItem.checkedState);
        if(this.checkNodesRecursive) {
            var listItems = this.GetListItems(subnodesList);
            if(listItems)
                this.UpdateAncestorsCheckedState(listItems[0]);
        }
        var button = ASPx.GetNodesByPartialClassName(listItem, this.ButtonCssClassName)[0];
        this.PerformNodeExpand(subnodesList, button);
    },

    HandleExpandButtonClick: function (e) {
        var button = ASPx.Evt.GetEventSource(e);
        var listItem = ASPx.GetParentByTagName(button, "LI");
        this.ProcessUserInitiatedExpandCollapse(button, listItem);
    },

    ProcessUserInitiatedExpandCollapse: function (button, listItem) {
        var contentElement = ASPx.GetNodesByPartialClassName(listItem, this.NodeCssClassName)[0];
        if(contentElement.performingExpandNodeRequest)
            return;
        var expandedChangingResults = this.RaiseExpandedChanging(listItem);
        if(expandedChangingResults.cancel)
            return;
        this.ToggleNodeExpandCollapse(listItem, button, true, expandedChangingResults.processOnServer);
    },

    ToggleNodeExpandCollapse: function (listItem, button, requireRaiseExpandedChanged, processOnServer) {
        var subnodesList = ASPx.GetNodeByTagName(listItem, "UL", 0);
        var expanding = !subnodesList || subnodesList.style.display == "none";
        var contentElement = ASPx.GetNodesByPartialClassName(listItem, this.NodeCssClassName)[0];
        if(processOnServer) {
            var requestArgs = this.CreatePostRequestArgs(this.RaiseExpandedChangingEventCommand,
                this.GetNodeIDByContentElementID(contentElement.id), this.SerializeBooleanValue(expanding));
            if(this.isInitialized)
                this.SendPostBack(requestArgs);
            return;
        }
        this.SetExpandedToState(this.GetNodeIDByContentElementID(contentElement.id), expanding);
        if(requireRaiseExpandedChanged)
            this.requireRaiseExpandedChangedList.push(contentElement.id);
        if(subnodesList) {
            if(expanding)
                this.PerformNodeExpand(subnodesList, button);
            else
                this.PerformNodeCollapse(subnodesList, button);
            return;
        }
        contentElement.performingExpandNodeRequest = true;
        this.SendExpandNodePostRequest(contentElement.id, button);
    },

    SendExpandNodePostRequest: function (contentElementID, button) {
        var requestArgs = this.CreatePostRequestArgs(this.ExpandNodeCommand,
            this.GetNodeIDByContentElementID(contentElementID));
        if(this.autoPostBack || !this.callBack) {
            if(this.isInitialized)
                this.SendPostBack(requestArgs);
            return;
        }
        this.ShowNodeLoadingPanel(button, this.GetNodeIDByContentElementID(contentElementID));
        this.CreateCallback(requestArgs);
    },

    StartExpandCollapseAnimation: function (subnodesList, expanding) {
        var height = subnodesList.offsetHeight;
        if(expanding)
            ASPx.SetOffsetHeight(subnodesList, 0);
        ASPx.AnimationHelper.createAnimationTransition(subnodesList, {
            animationEngine: "js", //TODO bug with css animation
            property: "height", unit: "px",
            duration: height > 120 ? this.AnimationDuration : this.MinAnimationDuration,
            onComplete: function (el) {
                this.OnCompleteAnimation(el, expanding);
            }.aspxBind(this)
        }).Start(expanding ? 0 : height, expanding ? height : 0);
    },
    OnCompleteAnimation: function (element, expanding) {
        element.style.height = "";
        if(!expanding) {
            ASPx.SetElementDisplay(element, false);
            this.CorrectControlWidth();
        }
        this.RaiseExpandedChanged(element);
    },

    ReplaceExpandButtonWithSampleButton: function (button, sampleButton) {
        var newButton = this.ReplaceElementWithSampleElement(button, sampleButton, false);
        var listItem = ASPx.GetParentByTagName(newButton, "LI");
        var contentElement = ASPx.GetNodesByPartialClassName(listItem, this.NodeCssClassName)[0];
        if(!ASPx.ElementContainsCssClass(contentElement, this.DisabledCssClassName))
            this.ToggleExpandButtonClickHandler(true, newButton);
        else
            newButton.style.cursor = "default";
    },

    AccomplishExpandCollapse: function (expandButton, sampleButton, subnodesList) {
        if(expandButton)
            this.ReplaceExpandButtonWithSampleButton(expandButton, sampleButton);
        if(!this.enableAnimation)
            this.RaiseExpandedChanged(subnodesList);
    },

    PerformNodeExpand: function (subnodesList, expandButton) {
        ASPx.SetElementDisplay(subnodesList, true);
        this.CorrectControlWidth();
        if(this.enableAnimation)
            this.StartExpandCollapseAnimation(subnodesList, true);
        this.AccomplishExpandCollapse(expandButton, this.GetSampleCollapseButton(), subnodesList);
    },

    PerformNodeCollapse: function (subnodesList, expandButton) {
        if(this.enableAnimation)
            this.StartExpandCollapseAnimation(subnodesList, false);
        else {
            ASPx.SetElementDisplay(subnodesList, false);
            this.CorrectControlWidth();
        }
        this.AccomplishExpandCollapse(expandButton, this.GetSampleExpandButton(), subnodesList);
    },

    GetNodeExpanded: function (clientNode) {
        if(this.IsRootNode(clientNode))
            return true;
        var expandedState = this.GetExpandedDataFromState();
        var nodeID = this.GetNodeIDByContentElementID(clientNode.contentElementID);
        if(ASPx.IsExists(expandedState[nodeID]))
            return !!expandedState[nodeID];
        return false;
    },

    SetNodeExpanded: function (clientNode) {
        if(this.IsRootNode(clientNode))
            return;
        var contentElement = ASPx.GetElementById(clientNode.contentElementID);
        if(contentElement.performingExpandNodeRequest)
            return;
        var listItem = this.GetNodeListItem(clientNode);
        var button = ASPx.GetNodesByPartialClassName(listItem, this.ButtonCssClassName)[0];
        var nodeID = this.GetNodeIDByContentElementID(clientNode.contentElementID);
        var expandedState = this.GetExpandedDataFromState();
        if(!button && !ASPx.IsExists(expandedState[nodeID]))
            return;
        this.ToggleNodeExpandCollapse(listItem, button, false, false);
    },


    UpdateStateObject: function(){
        this.UpdateStateObjectWithObject({ nodesState: this.nodesState });
    },
    UpdateNodesStateCookies: function() {
        if(this.cookieName) {
            ASPx.Cookie.DelCookie(this.cookieName);
            ASPx.Cookie.SetCookie(this.cookieName, ASPx.Json.ToJson(this.nodesState));
        }
    },
    AreChildNodesLoaded: function (node) {
        var listItem = this.GetNodeListItem(node);
        var button = ASPx.GetNodesByPartialClassName(listItem, this.ButtonCssClassName)[0];
        return !button || !!ASPx.GetNodeByTagName(listItem, "UL", 0);
    },

    //Events
    RaiseNodeClick: function (contentElement, htmlEvent) {
        var processOnServer = this.autoPostBack || this.IsServerEventAssigned(this.NodeClickServerEventName);
        if(!this.NodeClick.IsEmpty()) {
            var clientNode = this.rootNode.GetNodeByContentElementID(contentElement.id);
            var args = new ASPxClientTreeViewNodeClickEventArgs(processOnServer, clientNode,
                contentElement, htmlEvent);
            this.NodeClick.FireEvent(this, args);
            processOnServer = args.processOnServer;
        }
        return processOnServer;
    },

    RaiseExpandedChanged: function (subnodesList) {
        if(this.ExpandedChanged.IsEmpty())
            return;
        var listItem = ASPx.GetParentByTagName(subnodesList, "LI");
        var contentElement = ASPx.GetNodesByPartialClassName(listItem, this.NodeCssClassName)[0];
        var nodeIndex = ASPx.Data.ArrayIndexOf(this.requireRaiseExpandedChangedList, contentElement.id);
        if(nodeIndex < 0)
            return;
        this.requireRaiseExpandedChangedList.splice(nodeIndex, 1);
        var clientNode = this.rootNode.GetNodeByContentElementID(contentElement.id);
        var args = new ASPxClientTreeViewNodeEventArgs(clientNode);
        this.ExpandedChanged.FireEvent(this, args);
    },

    RaiseExpandedChanging: function (listItem) {
        var processOnServer = this.autoPostBack || this.IsServerEventAssigned(this.ExpandedChangingServerEventName);
        var contentElement = ASPx.GetNodesByPartialClassName(listItem, this.NodeCssClassName)[0];
        var clientNode = this.rootNode.GetNodeByContentElementID(contentElement.id);
        var args = new ASPxClientTreeViewNodeCancelEventArgs(processOnServer, clientNode);
        if(!this.ExpandedChanging.IsEmpty())
            this.ExpandedChanging.FireEvent(this, args);
        return args;
    },

    RaiseCheckedChanged: function (contentElement) {
        var processOnServer = this.autoPostBack || this.IsServerEventAssigned(this.CheckedChangedServerEventName);
        if(!this.CheckedChanged.IsEmpty()) {
            var clientNode = this.rootNode.GetNodeByContentElementID(contentElement.id);
            var args = new ASPxClientTreeViewNodeProcessingModeEventArgs(processOnServer, clientNode);
            this.CheckedChanged.FireEvent(this, args);
            processOnServer = args.processOnServer;
        }
        return processOnServer;
    },

    //API
    GetNode: function (index) {
        return this.rootNode.GetNode(index);
    },
    GetNodeByName: function (name) {
        return this.rootNode.GetNodeByName(name);
    },
    GetNodeByText: function (text) {
        return this.rootNode.GetNodeByText(text);
    },
    GetNodeCount: function () {
        return this.rootNode.GetNodeCount();
    },
    GetSelectedNode: function () {
        return this.rootNode.GetNodeByContentElementID(this.selectedNodeContentElementID);
    },
    SetSelectedNode: function (node) {
        if(node && (node.treeView != this || this.IsRootNode(node)) || !this.allowSelectNode)
            return;
        var contentElement = node ? ASPx.GetElementById(node.contentElementID) : null;
        var selectedNodeID = contentElement ? this.GetNodeIDByContentElementID(contentElement.id) : "";
        this.SetSelectedNodeIDToState(selectedNodeID);
        this.ApplySelectionToNode(contentElement);
    },
    GetRootNode: function () {
        return this.rootNode;
    },

    IsRootNode: function (clientNode) {
        return clientNode == this.rootNode;
    },

    ExpandCollapseNodesRecursive: function (clientNodes, expand) {
        for(var i = 0; i < clientNodes.length; i++) {
            clientNodes[i].SetExpanded(expand);
            if(clientNodes[i].nodes.length > 0)
                this.ExpandCollapseNodesRecursive(clientNodes[i].nodes, expand);
        }
    },
    CollapseAll: function () {
        if(this.rootNode.nodes.length == 0)
            return;
        this.ExpandCollapseNodesRecursive(this.rootNode.nodes, false);
    },
    ExpandAll: function () {
        if(this.rootNode.nodes.length == 0)
            return;
        var postRequestArgs = this.CreatePostRequestArgs(this.ExpandAllNodesCommand, "");
        if(this.autoPostBack) {
            if(this.isInitialized)
                this.SendPostBack(postRequestArgs);
            return;
        }
        if(this.callBack) {
            var mainElement = this.GetMainElement();
            this.CreateLoadingDiv(mainElement);
            this.CreateLoadingPanelWithAbsolutePosition(mainElement);
            this.CreateCallback(postRequestArgs);
            return;
        }
        this.ExpandCollapseNodesRecursive(this.rootNode.nodes, true);
    }
});
ASPxClientTreeView.Cast = ASPxClientControl.Cast;
var ASPxClientTreeViewNode = ASPx.CreateClass(null, {
    constructor: function (treeView, parent, index, name) {
        this.treeView = treeView;
        this.parent = parent;
        this.index = index;
        this.name = name;

        //Fields
        this.enabled = true;
        this.clientEnabled = true;
        this.clientVisible = true;
        this.navigateUrl = null;
        this.target = null;
        this.text = null;
        this.imageUrl = null;
        this.nodes = [];
        this.contentElementID = null;
        this.last = false;
    },
    GetNodeCount: function () {
        return this.nodes.length;
    },
    GetNode: function (index) {
        return (index >= 0 && index < this.nodes.length) ? this.nodes[index] : null;
    },

    GetNodeBySelector: function (selector) {
        for(var i = 0; i < this.nodes.length; i++) {
            if(selector(this.nodes[i]))
                return this.nodes[i];
            var foundNode = this.nodes[i].GetNodeBySelector(selector);
            if(foundNode)
                return foundNode;
        }
        return null;
    },

    GetNodeByContentElementID: function (contentElementID) {
        return this.GetNodeBySelector(function (node) { return node.contentElementID == contentElementID; });
    },

    SetCheckState: function (value) {
        this.treeView.SetNodeState(this, value);
    },
    GetNodeByName: function (name) {
        return this.GetNodeBySelector(function (node) { return node.name == name; });
    },
    GetNodeByText: function (text) {
        return this.GetNodeBySelector(function (node) { return node.GetText() == text });
    },
    GetExpanded: function () {
        return this.treeView.GetNodeExpanded(this);
    },
    SetExpanded: function (value) {
        if(this.GetExpanded() == value)
            return;
        this.treeView.SetNodeExpanded(this);
    },
    GetChecked: function () {
        return this.GetCheckState() == ASPx.CheckBoxCheckState.Checked;
    },
    SetChecked: function (value) {
        this.SetCheckState(value ? ASPx.CheckBoxCheckState.Checked : ASPx.CheckBoxCheckState.Unchecked);
    },
    GetCheckState: function () {
        return this.treeView.GetNodeState(this);
    },
    GetEnabled: function () {
        return this.enabled && this.clientEnabled;
    },
    SetEnabled: function (value) {
        this.clientEnabled = value;
        this.treeView.SetNodeClientEnabled(this, this.clientEnabled);
    },
    GetImageUrl: function () {
        return this.imageUrl || "";
    },
    SetImageUrl: function (value) {
        this.imageUrl = value;
        this.treeView.SetNodeImageUrl(this, value);
    },
    GetNavigateUrl: function () {
        return this.treeView.GetNodeNavigateUrl(this);
    },
    SetNavigateUrl: function (value) {
        this.treeView.SetNodeNavigateUrl(this, value);
    },
    GetText: function () {
        var nbspChar = String.fromCharCode(160)
        return this.text ? this.text.replace(new RegExp(nbspChar, "g"), " ") : "";
    },
    SetText: function (value) {
        this.text = value;
        this.treeView.SetNodeText(this, value);
    },
    GetVisible: function () {
        return this.clientVisible;
    },
    SetVisible: function (value) {
        this.clientVisible = value;
        this.treeView.SetNodeClientVisible(this, this.clientVisible);
    },
    GetHtmlElement: function () {
        return ASPx.GetElementById(this.contentElementID);
    }
});
var ASPxClientTreeViewNodeProcessingModeEventArgs = ASPx.CreateClass(ASPxClientProcessingModeEventArgs, {
    constructor: function (processOnServer, node) {
        this.constructor.prototype.constructor.call(this, processOnServer);
        this.node = node;
    }
});
var ASPxClientTreeViewNodeClickEventArgs = ASPx.CreateClass(ASPxClientTreeViewNodeProcessingModeEventArgs, {
    constructor: function (processOnServer, node, htmlElement, htmlEvent) {
        this.constructor.prototype.constructor.call(this, processOnServer, node);
        this.htmlElement = htmlElement;
        this.htmlEvent = htmlEvent;
    }
});
var ASPxClientTreeViewNodeEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function (node) {
        this.node = node;
    }
});
var ASPxClientTreeViewNodeCancelEventArgs = ASPx.CreateClass(ASPxClientProcessingModeCancelEventArgs, {
    constructor: function (processOnServer, node) {
        this.constructor.prototype.constructor.call(this, processOnServer);
        this.node = node;
    }
});

window.ASPxClientTreeView = ASPxClientTreeView;
window.ASPxClientTreeViewNode = ASPxClientTreeViewNode;
window.ASPxClientTreeViewNodeProcessingModeEventArgs = ASPxClientTreeViewNodeProcessingModeEventArgs;
window.ASPxClientTreeViewNodeClickEventArgs = ASPxClientTreeViewNodeClickEventArgs;
window.ASPxClientTreeViewNodeEventArgs = ASPxClientTreeViewNodeEventArgs;
window.ASPxClientTreeViewNodeCancelEventArgs = ASPxClientTreeViewNodeCancelEventArgs;
})();