function loadTicketsFromForm(session, formId) {
  $('#listBody').empty();
  var data = { session: session.session };
  if (formId != null) {
    data["formIds"] = formId;
  }
  $.ajax({
    url: '/apiv2/tickets/list',
    type: 'POST',
    data: data,
    success: function(data) {
      $.each(data, function(i, ticket) {
        $('#listBody').append(buildRow(ticket));
      });
      $('#listBody tr').click(function(event) {
        document.location = '../view/?ticketId=' + event.target.parentElement.id;
      });
    }
  });
}

function loadTicketsFromView(session, viewId) {
  $('#listBody').empty();
  var data = { session: session.session };
  if (viewId != null) {
    data["viewId"] = viewId;
  }
  $.ajax({
    url: '/apiv2/tickets/list',
    type: 'POST',
    data: data,
    success: function(data) {
      $.each(data, function(i, ticket) {
        $('#listBody').append(buildRow(ticket));
      });
      $('#listBody tr').click(function(event) {
        document.location = '../view/?ticketId=' + event.target.parentElement.id;
      });
    }
  });
}

function buildRow(ticket) {
  var row = '<tr id="' + ticket.ID + '">';
  row += '<td>' + ticket.ID + '</td>';
  row += '<td>' + ((ticket.Priority == null) ? '' : ticket.Priority) + '</td>';
  row += '<td>' + ticket.FormName + '</td>';
  row += '<td>' + ticket.Status + '</td>';
  row += '<td>' + ticket.CreatedDate + '</td>';
  row += '<td>' + ticket.Subject + '</td>';
  row += '<td>' + ticket.OpenedByName + '</td>';
  row += '<td>' + ((ticket.AssignedName == null) ? '' : ticket.AssignedName) + '</td>';
  row += '<td>' + ((ticket.DueDate == null) ? '' : ticket.DueDate) + '</td>';
  row += '<td>' + ticket.UpdatedDate + '</td>';
  row += '</tr>';
  return row;
}

function loadForms(session, defaultForm) {
  $('#formSelect').empty();
  $('#formSelect').append('<option value="null" selected>Queue</option>');

  $.ajax({
    url: '/apiv2/forms/list',
    type: 'POST',
    data: {
      session: session.session
    },
    success: function(data) {
      $.each(data, function(i, form) {
        $('#formSelect').append(buildOption(form));
      });
      if (defaultForm != null) {
        $('#formSelect').val(defaultForm);
      } else {
        $('#formSelect').val('null');
      }
    }
  });
}

function loadViews(session, defaultView) {
  $('#formSelect').empty();
  $('#formSelect').append('<option value="null" selected>View</option>');

  $.ajax({
    url: '/apiv2/views/list',
    type: 'POST',
    data: {
      session: session.session
    },
    success: function(data) {
      $.each(data, function(i, view) {
        $('#formSelect').append(buildOption(view));
      });
      if (defaultView != null) {
        $('#formSelect').val(defaultView);
      } else {
        $('#formSelect').val('null');
      }
    }
  });
}

function buildOption(data) {
  return '<option value="' + data.ID + '">' + data.Name + '</option>';
}