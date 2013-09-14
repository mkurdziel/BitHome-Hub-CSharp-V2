
var dashboardId = "";
var m_dashboards = {};
var m_activeDashboard = null;
var m_suppressUpdates = false;


function updateDashboardValues() {
    if (m_activeDashboard != null) {
        var dashboardId = m_activeDashboard;
        $.ajax({
            type: 'GET',
            url: '/api/dashboards/'+dashboardId+'/values',
            data: { format: 'json' },
            success: function(data) {
            // successful request; do something with the data
            if (!m_suppressUpdates) {
            	for (var i = 0, len = data.length; i < len; ++i) {
                 updateDashboardItemValue(dashboardId, data[i]);
             } 
         }
     }
  });
  }
}

function updateDashboardItemValue(dashboardId, webDashboardValue) {
    if (webDashboardValue.FeedId != null) {
        $('#feed_ctrl_' + dashboardId + "_" + webDashboardValue.FeedId + "_" + webDashboardValue.DataStreamId).slider('option', 'value', parseInt(webDashboardValue.Value));
    } else if (webDashboardValue.ActionId != null) {
    	var ctrls = $('[id$=_'+ webDashboardValue.ParameterId+']');
    	
    	for (var i = 0, len = ctrls.length; i < len; ++i) {
	    	if ($(ctrls[i]).is('input:hidden') == false && $(ctrls[i]).hasClass('bithome-parameter')) {
	        	$(ctrls[i]).slider('option', 'value', parseInt(webDashboardValue.Value));
	        }
        }
    }
}

function createDraggables() {
    $(".bithome-control").draggable(
    {
       stop: function (event, ui) {
           updatePosition(ui);
       },
       grid: [20, 20],
       handle: ".bithome-control-drag",
       containment: "parent"
   }
   );
}

function updatePosition(control) {
    var itemId = control.helper.attr('dashboard-item-id');
    var top = control.position.top;
    var left = control.position.left;

    $.ajax({

        type: 'POST',
        url: '/api/dashboards/' + dashboardId + "/items/" + itemId + "/position",
        data: {
            format: 'json',
            dashboardId: dashboardId,
            itemId: itemId,
            positionX: left,
            positionY: top
        }
    });
}

function updateDashboards() {

    $.ajax({
        type: 'GET',
        url: '/api/dashboards',
        data: { format: 'json' },
        beforeSend: function () {
        },
        success: function (data) {

        // Save this locally
        m_dashboards = data;

        clearDashboardData();

        createDashboardTabs(m_dashboards);

        if (m_activeDashboard != null) {
            if ($(m_activeDashboard) != undefined) {
                $('#dashboard_link_' + m_activeDashboard).tab('show');
            }
        }
        //createDraggables();
    },
    error: function () {
        // failed request; give feedback to user
    }
});
}

function clearDashboardData() {
    $('#dashboard-tabs').empty();
    $('#dashboard-tab-content').empty();
}

function createDashboard() {
    var nodeId = undefined;
    if ($('#create-dashboard-type-node').is(':checked')) {
        nodeId = $('#create-dashboard-nodes').val();
    }

    $.ajax({
        type: 'POST',
        url: '/api/dashboards',
        data: {
            name: $('#create-dashboard-name').val(),
            nodeId: nodeId
        },
        beforeSend: function () {
        },
        success: function (data) {
            updateDashboards();

            $('#create-dashboard-modal').modal('hide');
        },
        error: function () {
        // failed request; give feedback to user
    }
});
}

function addActions() {
    var dashboardId = $('#addaction-dashboard-id').val();
    var actionIds = [];

    $('#addaction-actions').find('li.ui-selected').each(function (index, value) {
        var actionId = $(value).attr('actionid');

        actionIds.push(actionId);
    });

    $.ajax({

        type: 'POST',
        url: '/api/dashboards/' + dashboardId + '/items',
        data: {
            actionids: JSON.stringify(actionIds)
        },
        beforeSend: function () {
        },
        success: function (data) {
            updateDashboards();

            $('#addaction-dashboard-modal').modal('hide');
        },
        error: function () {
        // failed request; give feedback to user
    }
});
}


function addFeeds() {

    var dashboardId = $('#addfeed-dashboard-id').val();
    var feedIds = [];

    $('#addfeed-feeds').find('li.ui-selected').each(function (index, value) {
        var feedId = $(value).attr('feedid');

        feedIds.push(feedId);
    });

    $.ajax({

        type: 'POST',
        url: '/api/dashboards/' + dashboardId + '/items',
        data: {
            feedids: JSON.stringify(feedIds)
        },
        beforeSend: function () {
        },
        success: function (data) {
            updateDashboards();

            $('#addfeed-dashboard-modal').modal('hide');
        },
        error: function () {
        // failed request; give feedback to user
    }
});
}

function showDeleteDashboardItem(dashboard, dashboardItem) {
    $('#delete-dashboarditem-title').text('Delete Dashboard Item ' + dashboardItem.Action.Name);
    $('#delete-dashboarditem-id').val(dashboardItem.Id);
    $('#delete-dashboarditem-dashboardid').val(dashboard.Id);
    $('#delete-dashboarditem-modal').modal('show');
}

function showPropertiesDashboardItemValue(dashboardId, dashboardItemId, dashboardItemValueId) {

  $.ajax({
        type: 'GET',
        url: '/api/dashboards/' + dashboardId + '/items/' + dashboardItemId + '/values/' + dashboardItemValueId,
        data: { format: 'json' },
        success: function (data) {
			$('#properties-dashboarditemvalue-title').text(data.Name + ' Settings');
		    $('#properties-dashboarditemvalue-name').val(data.Name);
		    $('#properties-dashboarditemvalue-name').attr('placeholder', 'Name');
		    $('#properties-dashboarditemvalue-id').val(data.Id);
		    $('#properties-dashboarditemvalue-dashboarditemid').val(dashboardItemId);
		    $('#properties-dashboarditemvalue-dashboardid').val(dashboardId);
		    
		    if (data.Constant == null) {
		    	$('#properties-dashboarditemvalue-constant').attr('disabled', 'disabled');
		    	$('#properties-dashboarditemvalue-constant').val('');
		    	$('#properties-dashboarditemvalue-source-input').prop("checked", true);
		    } else {
		        $('#properties-dashboarditemvalue-constant').removeAttr('disabled');
		    	$('#properties-dashboarditemvalue-constant').val(data.Constant);
		    	$('#properties-dashboarditemvalue-source-constant').prop("checked", true);
		    }
		    
		    $('#properties-dashboarditemvalue-modal').modal('show');
        },
	    async: false
	});
}

function showPropertiesDashboardItem(dashboard, dashboardItem) {
    var name = dashboardItem.Name ? dashboardItem.Name : dashboardItem.Action.Name;
    $('#properties-dashboarditem-title').text(dashboardItem.Action.Name + ' Settings');
    $('#properties-dashboarditem-name').val(name);
    $('#properties-dashboarditem-name').attr('placeholder', dashboardItem.Action.Name);
    $('#properties-dashboarditem-showexecutebutton').prop('checked', dashboardItem.ShowExecuteButton);
    $('#properties-dashboarditem-id').val(dashboardItem.Id);
    $('#properties-dashboarditem-dashboardid').val(dashboard.Id);
    
    $('#properties-dashboarditem-values').empty();
    jQuery.each(dashboardItem.Values, function (index, dashboardItemValue) {
    	var listItem = $("<li/>", { text : dashboardItemValue.Name });
    	var editLink = $("<a/>", { html : "&nbsp;<span class='glyphicon glyphicon-cog'></span>" });
    	editLink.on("click", function (dashboard, dashboardItem, dashboardItemValue) {
		    return function () {
		        showPropertiesDashboardItemValue(dashboard.Id, dashboardItem.Id, dashboardItemValue.Id);
		    }
		}(dashboard, dashboardItem, dashboardItemValue));
    	
    	listItem.append(editLink);
    	
    	$('#properties-dashboarditem-values').append( listItem );
    });
    
    $('#properties-dashboarditem-modal').modal('show');
}

function showDeleteDashboard(dashboard) {
    $('#delete-dashboard-title').text('Delete Dashboard ' + dashboard.Name);
    $('#delete-dashboard-id').val(dashboard.Id);
    $('#delete-dashboard-modal').modal('show');
}

function showPropertiesDashboard(dashboard) {
    $('#properties-dashboard-title').text('Dashboard ' + dashboard.Name + ' Properties');
    $('#properties-dashboard-name').val(dashboard.Name);
    $('#properties-dashboard-id').val(dashboard.Id);
    $('#properties-dashboard-modal').modal('show');
}

function showAddAction(dashboard) {
    updateActions();
    $('#addaction-dashboard-title').text('Add Action To ' + dashboard.Name);
    $('#addaction-dashboard-id').val(dashboard.Id);
    $('#addaction-dashboard-modal').modal('show');
}

function showAddFeed(dashboard) {
    updateFeeds();
    $('#addfeed-dashboard-title').text('Add Feed To ' + dashboard.Name);
    $('#addfeed-dashboard-id').val(dashboard.Id);
    $('#addfeed-dashboard-modal').modal('show');
}

function showCreateDashboard() {

    $('#create-dashboard-nodes').empty();
    $('#create-dashboard-nodes').attr('disabled', 'disabled');
    $('#create-dashboard-type-blank').prop("checked", true)

    $.ajax({
        type: 'GET',
        url: '/api/nodes',
        data: { format: 'json' },
        beforeSend: function () {
            $("#nodes-table tr").remove();
        },
        success: function (data) {
        // successful request; do something with the data
        for (var i = 0, len = data.length; i < len; ++i) {
            var node = data[i];

            $('#create-dashboard-nodes').append('<option value="' + node.Id + '">' + node.Identifier + '</option>');
        }
    },
    async: false
});

    $('#create-dashboard-modal').modal('show');
}

function updateActions() {
    $.ajax({
        type: 'GET',
        url: '/api/actions/node',
        data: { format: 'json' },
        beforeSend: function () {
            $("#addaction-actions li").remove();
        },
        success: function (data) {
        // successful request; do something with the data
        for (var i = 0, nodelen = data.length; i < nodelen; ++i) {
            var webNode = data[i];

            for (var a = 0, actionlen = webNode.Actions.length; a < actionlen; ++a) {
                var action = webNode.Actions[a];

                $('#addaction-actions').append(
                 $('<li/>', {
                     'class': 'ui-widget-content',
                     'actionid': action.Id,
                     'text': webNode.Node.Identifier + ' - ' + action.Name
                 })
                 );
            }
        }

        $("#addaction-actions").selectable();
    },
    error: function () {
        // failed request; give feedback to user
    },
    async: false
});
}


function updateFeeds() {
    $.ajax({
        type: 'GET',
        url: '/api/feeds',
        data: { format: 'json' },
        beforeSend: function () {
            $("#addfeed-feeds li").remove();
        },
        success: function (data) {
        // successful request; do something with the data
        for (var i = 0, nodelen = data.length; i < nodelen; ++i) {
            var feed = data[i];

            $('#addfeed-feeds').append(
                $('<li/>', {
                    'class': 'ui-widget-content',
                    'feedid': feed.Id,
                    'text': feed.Name
                })
                );
        }

        $("#addfeed-feeds").selectable();
    },
    error: function () {
        // failed request; give feedback to user
    },
    async: false
});
}

function getDashboard (dashboardId) {
	var retVal = null;
	$.ajax({
       type: 'GET',
       url: '/api/dashboards/' + dashboardId,
       data: { format: 'json' },
       success: function (data) {
          retVal = data;
      },
      async: false
  });
	return retVal;
}

function deleteDashboard() {
    var dashboardId = $('#delete-dashboard-id').val();

    $.ajax({

        type: 'POST',
        url: '/api/dashboards/' + dashboardId + '/delete',
        data: {},
        beforeSend: function () {
        },
        success: function (data) {

            updateDashboards()

            $('#delete-dashboard-modal').modal('hide');
        },
        error: function () {
        // failed request; give feedback to user
    }
});
}

function deleteDashboardItem() {

    var dashboardId = $('#delete-dashboarditem-dashboardid').val();
    var dashboardItemId = $('#delete-dashboarditem-id').val();

    $.ajax({

        type: 'POST',
        url: '/api/dashboards/' + dashboardId + '/items/' + dashboardItemId + '/delete',
        data: {},
        beforeSend: function () {
        },
        success: function (data) {

            updateDashboards();

            $('#delete-dashboarditem-modal').modal('hide');
        },
        error: function () {
        // failed request; give feedback to user
    }
});
}

function propertiesDashboard() {
    var dashboardId = $('#properties-dashboard-id').val();
    var name = $('#properties-dashboard-name').val();

    $.ajax({
        type: 'PUT',
        url: '/api/dashboards/' + dashboardId,
        data: { name: name },
        beforeSend: function () {
        },
        success: function (data) {

            updateDashboards();

            $('#properties-dashboard-modal').modal('hide');
        },
        error: function () {
        // failed request; give feedback to user
    	}
	});
}

function propertiesDashboardItem() {
    var dashboardId = $('#properties-dashboarditem-dashboardid').val();
    var dashboardItemId = $('#properties-dashboarditem-id').val();

    var name = $('#properties-dashboarditem-name').val();
    var showexecutebutton = $('#properties-dashboarditem-showexecutebutton').prop('checked');

    $.ajax({
        type: 'PUT',
        url: '/api/dashboards/' + dashboardId + '/items/' + dashboardItemId,
        data: { 
        	name: name,
        	showexecutebutton: showexecutebutton
        },
        beforeSend: function () { },
        success: function (data) {

            updateDashboards();

            $('#properties-dashboarditem-modal').modal('hide');
        },
        error: function () {
        // failed request; give feedback to user
    	}
	});
}

function propertiesDashboardItemValue() {
    var dashboardId = $('#properties-dashboarditemvalue-dashboardid').val();
    var dashboardItemId = $('#properties-dashboarditemvalue-dashboarditemid').val();
    var dashboardItemValueId = $('#properties-dashboarditemvalue-id').val();

    var name = $('#properties-dashboarditemvalue-name').val();
    var constant = null;

	if ( $('#properties-dashboarditemvalue-source-constant').prop("checked")) {
		constant = $('#properties-dashboarditemvalue-constant').val();
	}

    $.ajax({
        type: 'PUT',
        url: '/api/dashboards/' + dashboardId + '/items/' + dashboardItemId + '/values/' + dashboardItemValueId,
        data: { 
        	name: name,
        	constant: constant
        },
        beforeSend: function () { },
        success: function (data) {

            updateDashboards();

            $('#properties-dashboarditemvalue-modal').modal('hide');
        },
        error: function () {
        // failed request; give feedback to user
    	}
	});
}

function createDashboardTabs(dashboards) {
    for (var i = 0; i < dashboards.length; ++i) {
        var dashboard = dashboards[i];

    // Create the link across the top
    var link = $('<a/>', {
        'id': 'dashboard_link_' + dashboard.Id,
        'dashboard-id': dashboard.Id,
        'href': '#dashboard_' + dashboard.Id,
        'text': dashboard.Name,
        'data-toggle': 'tab'
    });

    link.click(function(n) {
        return (function() { 
	        $(this).tab('show');
	        m_activeDashboard = n;
    	 });
    }(dashboard.Id));
    
    $('#dashboard-tabs').append(
      $('<li/>').append(link)
    );

    // Create the tab content item
    var tab = $('<div/>', {
        'class': 'tab-pane dashboard-pane',
        'id': 'dashboard_' + dashboard.Id
    });

    createDashboardTabContent(dashboard, tab, true);

    $('#dashboard-tab-content').append(tab);
}

// Add the create tab

var link = $('<a/>', {
    'href': 'javascript:void(0);',
    'text': '+ New Dashboard'
});

link.click(function (e) {
    showCreateDashboard();
});

$('#dashboard-tabs').append(
	$('<li/>').append(link)
    );
}

function createDashboardTabContent(dashboard, tab, showSettings) {
    var title = $('<h3/>', { 'text': dashboard.Name });
    
    if (showSettings) {
	    var settingsButtonGroup = $('<div/>', { 'class': 'btn-group' });
	    var settingsToggle = $('<button/>', { 'class': 'btn btn-xs dropdown-toggle', 'data-toggle': 'dropdown', 'href': '#' });
	    settingsToggle.append($('<i/>', { 'class': 'icon-cog' }));
	    settingsToggle.append($('<span/>', { 'class': 'caret' }));

	    var settingsDrop = $('<ul/>', { 'class': 'dropdown-menu' });

	    var deleteLink = $('<a/>', { 'href': '#', 'text': 'Delete Dashboard' });
	    deleteLink.click(function (e) {
	        showDeleteDashboard(dashboard);
	    });
	    settingsDrop.append($('<li/>').append(deleteLink));

	    var propertiesLink = $('<a/>', { 'href': '#', 'text': 'Dashboard Properties' });
	    propertiesLink.click(function (e) {
	        showPropertiesDashboard(dashboard);
	    });
	    settingsDrop.append($('<li/>').append(propertiesLink));

	    var addLink = $('<a/>', { 'href': '#', 'text': 'Add Action' });
	    addLink.click(function (e) {
	        showAddAction(dashboard);
	    });
	    settingsDrop.append($('<li/>').append(addLink));

	    var addFeedLink = $('<a/>', { 'href': '#', 'text': 'Add Feed' });
	    addFeedLink.click(function (e) {
	        showAddFeed(dashboard);
	    });
	    settingsDrop.append($('<li/>').append(addFeedLink));

	    var fullscreenLink = $('<a/>', { 'href': 'javascript:void(0);', 'text': 'Fullscreen' });
	    fullscreenLink.click(function (e) {
	        window.location.href = '/dashboards/' + dashboard.Id;
	    });
	    settingsDrop.append($('<li/>').append(fullscreenLink));


	    settingsButtonGroup.append(settingsToggle);
	    settingsButtonGroup.append(settingsDrop);

	    title.append(settingsButtonGroup);
    }

    tab.append(title);

    var dashboardSpace = $('<div/>', {
        'id': 'dashboard_space_' + dashboard.Id,
        'class': 'dashboard'
    });

    tab.append(dashboardSpace);

    createDashboardItems(dashboard, dashboardSpace, showSettings);
}

function createDashboardItems(dashboard, dashboardSpace, showSettings) {
    dashboardId = dashboard.Id;

    $.ajax({

        type: 'GET',
        url: '/api/dashboards/' + dashboardId + '/items',
        data: { format: 'json' },
        beforeSend: function () {
        },
        success: function (data) {


        // Loop through each action
        jQuery.each(data, function (index, webDashboardItem) {
            createItem(dashboard, dashboardSpace, webDashboardItem, showSettings);

        });
    },
    error: function () {
        // failed request; give feedback to user
    }
});
}

function createItem(dashboard, dashboardSpace, webDashboardItem, showSettings) {
    if (webDashboardItem.Action != null) {
        createItemFromAction(dashboard, dashboardSpace, webDashboardItem, showSettings)
    }

    if (webDashboardItem.Feed != null) {
        createItemFromFeed(dashboard, dashboardSpace, webDashboardItem, showSettings)
    }

}

function createItemFromFeed(dashboard, dashboardSpace, webDashboardItem, showSettings) {
    var feed = webDashboardItem.Feed;
    var control = $("<div/>", {
        'dashboard-item-id': webDashboardItem.Id,
        'id': 'control_' + feed.Id,
        'class': 'draggable ui-widget-content bithome-control'
    });

    control.css('top', webDashboardItem.PositionY + "px");
    control.css('left', webDashboardItem.PositionX + "px");
    var name = webDashboardItem.Name ? webDashboardItem.Name : feed.Name;

    var titleBar = $('<div/>', { 'html': '<span>' + name + '<span/>', 'class': 'bithome-control-title bithome-control-drag' });
    var settingsButton = $('<span/>', {
        'class': 'dropdown',
        'html': '<a class="dropdown-toggle" data-toggle="dropdown" href="#"><i class="icon-cog"></i></a>'
    });

    var settingsMenu = $('<ul/>', {
        'class': 'dropdown-menu',
        'role': 'menu'
    });

    var deleteLink = $('<a/>', { 'href': '#', 'text': 'Delete Item' });
    deleteLink.click(function (e) {
        showDeleteDashboardItem(dashboard, webDashboardItem);
    });
    settingsMenu.append($('<li/>').append(deleteLink));

    settingsButton.append(settingsMenu);
    titleBar.append(settingsButton);
    control.append(titleBar);


// Create the parameter controls
jQuery.each(webDashboardItem.DataStreams, function (index, datastream) {
    var paramWrap = $("<div/>", {
        'class': 'slider-wrapper'
    });

    var paramCtrl = $("<div/>", {
        'id': 'feed_ctrl_' + dashboard.Id + "_" + feed.Id + '_' + datastream.Id,
        'datastream-id': datastream.Id,
        'feed-id': feed.Id,
        'class': 'slider-vertical bithome-parameter'
    });

    paramCtrl.slider({
        orientation: "vertical",
        range: "min",
        min: datastream.MinValue,
        max: datastream.MaxValue,
        value: 0,
        start: function( event, ui ) {
        	_suppressUpdates = true;
        },
        stop: function( event, ui ) {
        	_suppressUpdates = false;
        },
        slide: $.throttle(function (feedId) {
            return function () {
               executeFeed(feedId);
           };
       }(feed.Id), 100, false, true)
    }).sliderAccess({
      touchonly : false
  });
    paramWrap.append(paramCtrl);
    control.append(paramWrap);
});

//slide: function (actionId) {
//return function() {
//   $.delay(1).throttle( 100, execute(actionId));
//};

var executeButton = $("<a/>", {
    'class': 'btn btn-primary btn-medium',
    'text': 'Execute'
});
control.append($("<br/>"));
control.append(executeButton);
executeButton.on("click", function (feedId) {
    return function () {
        executeFeed(feedId);
    }
}(feed.Id));

dashboardSpace.append(control);
}

function createItemFromAction(dashboard, dashboardSpace, webDashboardItem, showSettings) {
    var action = webDashboardItem.Action;
    var control = $("<div/>", {
        'dashboard-item-id': webDashboardItem.Id,
        'action-id': action.Id,
        'id': 'control_' + webDashboardItem.Id,
        'class': 'draggable ui-widget-content bithome-control'
    });

    control.css('top', webDashboardItem.PositionY + "px");
    control.css('left', webDashboardItem.PositionX + "px");
    var name = webDashboardItem.Name ? webDashboardItem.Name : action.Name;

    var titleBar = $('<div/>', { 'html': '<span>' + name + '<span/>', 'class': 'bithome-control-title bithome-control-drag' });
    if (showSettings) {
	    var settingsButton = $('<span/>', {
	        'class': 'dropdown',
	        'html': '<button class="dropdown-toggle btn btn-default btn-xs" data-toggle="dropdown" href="javascript:void(0);"><i class="glyphicon glyphicon-cog"></i></a>'
	    });

	    var settingsMenu = $('<ul/>', {
	        'class': 'dropdown-menu',
	        'role': 'menu'
	    });

	    var deleteLink = $('<a/>', { 'href': '#', 'text': 'Delete Item' });
	    deleteLink.click(function (e) {
	        showDeleteDashboardItem(dashboard, webDashboardItem);
	    });
	    settingsMenu.append($('<li/>').append(deleteLink));

    	var propertiesLink = $('<a/>', { 'href': '#', 'text': 'Item Properties' });
	    propertiesLink.click(function (e) {
	        showPropertiesDashboardItem(dashboard, webDashboardItem);
	    });
	    settingsMenu.append($('<li/>').append(propertiesLink));


	    settingsButton.append(settingsMenu);
	    titleBar.append(settingsButton);
    }
    control.append(titleBar);


// Create the parameter controls
jQuery.each(webDashboardItem.Parameters, function (index, param) {
	// Make sure this is not a constant
	if (webDashboardItem.Values[param.Id] != null) {
		if (webDashboardItem.Values[param.Id].Constant == null) {
		    var paramWrap = $("<div/>", {
		        'id': 'param_wrap_' + dashboard.Id + '_' + webDashboardItem.Id + '_' + param.Id,
		        'class': 'slider-wrapper'
		    });

		    var paramCtrl = $("<div/>", {
		        'id': 'param_ctrl_' + dashboard.Id + '_' + webDashboardItem.Id + '_' + param.Id,
		        'parameter-id': param.Id,
		        'class': 'slider-vertical bithome-parameter'
		    });
		    
		    paramWrap.append(paramCtrl);
		    control.append(paramWrap);

		    paramCtrl.slider({
		        orientation: "vertical",
		        range: "min",
		        min: param.MinimumValue,
		        max: param.MaximumValue,
		        value: 0,
		        start: function( event, ui ) {
		        	m_suppressUpdates = true;
		        },
		        stop: function( event, ui ) {
		        	m_suppressUpdates = false;
		        },
		        slide: $.throttle(function (webDashboardItemId) {
		            return function () {
		                execute(webDashboardItemId);
		            };
		        }(webDashboardItem.Id), 150, false, true)
		    });
	    } else {
	    	control.append(
	    		$('<input>', { 'id' : 'param_ctrl_' + dashboard.Id + '_' + webDashboardItem.Id + '_' + param.Id, 
	    		'parameter-id' : param.Id,
	    		'class' : 'bithome-parameter-hidden',
	    		'type' : 'hidden',
	    		'value' : webDashboardItem.Values[param.Id].Constant })
	    	);
	    }
    }
});

//slide: function (actionId) {
//return function() {
//   $.delay(1).throttle( 100, execute(actionId));
//};

	if (webDashboardItem.ShowExecuteButton) {
		var executeButton = $("<a/>", {
		    'class': 'btn btn-primary btn-medium',
		    'text': 'Execute'
		});
		control.append($("<br/>"));
		control.append(executeButton);
		executeButton.on("click", function (webDashboardItemId) {
		    return function () {
		        execute(webDashboardItemId);
		    }
		}(webDashboardItem.Id));
	}

dashboardSpace.append(control);
}

function execute(dashboardItemId) {
    var control = $('#control_' + dashboardItemId);
    var actionId = control.attr('action-id');
    var parameters = {};

    jQuery.each(control.find('.bithome-parameter'), function (index, param) {
        var paramId = $(param).attr('parameter-id');
        var value = $(param).slider("option", "value");
        parameters[paramId] = value;
    });
    
    jQuery.each(control.find('.bithome-parameter-hidden'), function (index, param) {
        var paramId = $(param).attr('parameter-id');
        var value = $(param).val();
        parameters[paramId] = value;
    });

    $.ajax({

        type: 'POST',
        url: '/api/actions/' + actionId + "/execute",
        data: {
            format: 'json',
            parameters: JSON.stringify(parameters)
        },
        beforeSend: function () {
        // this is where we append a loading image
    },
    success: function (data) {
    }
});
}

function executeFeed(feedId) {
    var control = $('#control_' + feedId);
    var parameters = {};

    jQuery.each(control.find('.bithome-parameter'), function (index, param) {
        var datastreamId = $(param).attr('datastream-id');
        var value = $(param).slider("option", "value");
        parameters[datastreamId] = value;
    });

    $.ajax({

        type: 'PUT',
        url: '/api/feeds/' + feedId,
        data: {
            format: 'json',
            datastreams: JSON.stringify(parameters)
        },
        beforeSend: function () {
        // this is where we append a loading image
    },
    success: function (data) {
    }
});
}