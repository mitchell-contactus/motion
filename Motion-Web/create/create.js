function loadForms(session) {
	$.ajax({
		url: '/apiv2/forms/list',
	    type: 'POST',
	    data: {
	      session: session.session
	    },
	    success: function(data) {
	    	$.each(data, function(i, row) {
		        var option = $('<option />', {
		        	value: row['ID'],
		        	html: row['Name']
		        });
		        $('#form').append(option);
		    });
		    loadCustomFields(session, $('#form').val());
	    }
	});
}

function loadCustomFields(session, formId) {
	$.ajax({
		url: '/apiv2/forms/getFields',
	    type: 'POST',
	    data: {
	      session: session.session,
	      formId: formId
	    },
	    success: function(data) {
	    	$('#custom-forms').empty();
	    	$.each(data, function(i, field) {
	    		$('#custom-forms').append(buildField(field));
		    });
	    }
	});
}

function buildField(field) {
	var h = '<div class="form-group">';
	h += '<label for="' + field.Name + '">' + field.Name + '</label>';

	switch (field.Type) {
		case 'multiselect':
			h += '<select class="custom-select" id="' + field.Name + '" multiple>';
			$.each(field.Options, function(i, option) {
				h += '<option value="' + option + '">' + option + '</option>';
			});
			h += '</select>';
			break;
		case 'select':
			h += '<select class="custom-select" id="' + field.Name + '">';
			$.each(field.Options, function(i, option) {
				h += '<option value="' + option + '">' + option + '</option>';
			});
			h += '</select>';
			break;
		default:
			h += '<input type="' + field.Type + '" id="' + field.Name + '" />';
			break;
	}
	h += '</div>';
	return h;
}

function submit(session) {
	var postData = {
		session : session.session,
		form_id : $('#form').val(),
		subject : $('#subject').val(),
		GUID : $('#GUID').val(),
		source : "Web"
	};

	var validated = true;
	$.ajax({
		url: '/apiv2/forms/getFields',
	    type: 'POST',
	    async: false,
	    data: {
	      session: postData.session,
	      formId: postData.form_id
	    },
	    success: function(data) {
	    	$.each(data, function(i, field) {
	    		var value = $('#' + field.Name).val();

	    		if (field.Required && (value == null || value == "null" || value == "")) {
	    			validated = false;
	    		}

	    		if (field.Type == 'multiselect') {
	    			postData[field.Name] = value.join(',');
	    		} else {
	    			postData[field.Name] = value;
	    		}
	    		
		    });
	    }
	});

	if (!validated) {
		alert("Bad validation");
		//TODO: throw an error or something
		return;
	}

	$.ajax({
		url: '/apiv2/tickets/create',
	    type: 'POST',
	    data: postData,
	    success: function(ticketId) {
	    	$.ajax({
	    		url: '/apiv2/tickets/comment',
	    		type: 'POST',
	    		data: {
				    session: session.session,
				    ticketId: ticketId,
				    comment: $('#summernote').summernote('code'),
				    type: 1
				},
				success : function () {
					document.location = "../view?ticketId=" + ticketId;
				},
				error : function () {
					document.location = "../view?ticketId=" + ticketId;
				}
	    	});
	    }
	});
}

// https://stackoverflow.com/questions/105034/create-guid-uuid-in-javascript
function generateUUID() { // Public Domain/MIT
    var d = new Date().getTime();
    if (typeof performance !== 'undefined' && typeof performance.now === 'function'){
        d += performance.now(); //use high-precision timer if available
    }
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = (d + Math.random() * 16) % 16 | 0;
        d = Math.floor(d / 16);
        return (c === 'x' ? r : (r & 0x3 | 0x8)).toString(16);
    });
}