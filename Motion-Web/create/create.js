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