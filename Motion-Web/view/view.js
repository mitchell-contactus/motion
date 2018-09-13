function loadTicket(ticketId, session) {
  $.each($('select[data]'), function(i, tag) {
    $(tag).empty();
    var nullV = $(tag).attr('nullValue');
    if (nullV != null) {
      var option = $('<option />', {
        value: "null",
        html: nullV
      });
      $(tag).append(option);
    }

    $.ajax({
      url: $(tag).attr('data'),
      type: 'POST',
      async: false,
      data: {
        session: session.session,
        ticketId: ticketId
      },
      success: function(data) {
        $.each(data, function(i, row) {
          var option = $('<option />', {
            value: row[$(tag).attr('dataValue')],
            html: row[$(tag).attr('dataText')]
          });
          $(tag).append(option);
        });
      }
    });
  });

  $.ajax({
    url: '/apiv2/tickets/view',
    type: 'POST',
    data: {
      session: session.session,
      ticketId: ticketId
    },
    success: function(ticket) {
      $('#messageCount').text(ticket.Comments.length);
      $('entry#priority').text(ticket.Priority == null ? "--" : ticket.Priority);

      //TODO
      $('#contact').text(ticket.ContactName == null ? "No Contact Selected" : ticket.ContactName);
      $('#due').text(ticket.DueDate == null ? "Not Assigned" : ticket.DueDate);
      //Participants

      $.each($('static[data]'), function(i, tag) {
        $(tag).text(ticket[$(tag).attr('data')]);
      });

      $.each($('entry[data][type="text"]'), function(i, tag) {
        $(tag).text(ticket[$(tag).attr('data')]);
      });

      $.each($('entry[data][type="select"]'), function(i, tag) {
        $(tag).text($('select#' + tag.id + ' option[value="' + ticket[$(tag).attr('data')] +'"]').html());
        $(tag).attr('value', ticket[$(tag).attr('data')]);
        if (ticket[$(tag).attr('data')] == null) {
          $(tag).attr('value', "null");
        }
      });

      $('#comments').empty();
      $.each(ticket.Comments, function(i, comment) {
        buildComment(comment);
      });
      $('#history > ul').empty();
      $.each(ticket.History, function(i, event) {
        buildEvent(event);
      });

      $('.edit-button').prop('disabled', false);
    }
  });
}

function buildComment(comment) {
  var card = '<div class="card mb-2">';
  card += '<div class="card-header">';
  card += '<h5>' + comment.UserName + ' <small>' + buildCommentType(comment.Type) + '</small></h5>';
  card += comment.CreatedDate;
  card += '</div><div class="card-body">';
  card += comment.Entry;
  card += '</div></div>';
  $('#comments').append(card);
}

function buildCommentType(type) {
  switch(type) {
    case 0:
      return 'Commented internally';
    case 1:
      return 'Commented publicly';
    default:
      return 'Unknown comment type';
  }
}

function buildEvent(event) {
  var line = '<li>';
  line += buildEventType(event.EventType);
  line += ' at ' + event.Timestamp;
  line += ' by ' + event.Name;
  line += '</li>';
  $('#history > ul').append(line);
}

function buildEventType(type) {
  switch (type) {
    case 1:
      return '<span class="badge badge-primary">Created</span>';
    case 2:
      return '<span class="badge badge-secondary">Viewed</span>';
    case 3:
      return '<span class="badge badge-primary">Commented Internally</span>';
    case 4:
      return '<span class="badge badge-primary">Closed</span>';
    case 5:
      return '<span class="badge badge-info">Changed Queue</span>';
    case 6:
      return '<span class="badge badge-info">Edited</span>';
    case 7:
      return '<span class="badge badge-secondary">Preview</span>';
    case 8:
      return '<span class="badge badge-danger">Security Prevented Change</span>';
    case 9:
      return '<span class="badge badge-info">Assigned</span>';
    case 10:
      return '<span class="badge badge-primary">Commented Externally</span>';
    case 11:
      return '<span class="badge badge-warning">Reopened</span>';
    case 12:
      return '<span class="badge badge-info">Status Change</span>';
    case 13:
      return '<span class="badge badge-info">Added Participant</span>';
    case 14:
      return '<span class="badge badge-info">Removed Participant</span>';
    case 15:
      return '<span class="badge badge-primary">Created Subtask</span>';
    default:
      return '<span>' + type + '</span>';
  }
}

// enum TICKET_EVENT
// {
//     ERROR_CREATING_TICKET = -1,
//     CREATED = 1,
//     VIEWED = 2,
//     COMMENTED_INTERNAL = 3,
//     CLOSED = 4,
//     CHANGED_QUEUE = 5,
//     EDITED = 6,
//     PREVIEW = 7,
//     SECURITY_PREVENTED = 8,
//     ASSIGNED = 9,
//     COMMENTED_EXTERNAL = 10,
//     REOPENED = 11,
//     STATUS_CHANGE = 12, // General status hange other than Open, reopened, or closed
//     ADDED_PARTICIPANT = 13,
//     REMOVED_PARTICIPANT = 14,
//     CREATED_SUBTASK = 15
// };

function makeEditable() {
  $.each($('entry[type="select"'), function(i, entry) {
    $(entry).prop('hidden', true);
    $('select#' + entry.id + ' option[value="' + $(entry).attr("value") +'"]').prop('selected', true);
    $('select#' + entry.id).prop('hidden', false);
  });

  $.each($('entry[type="text"'), function(i, entry) {
    $(entry).prop('hidden', true);
    $('input[data="' + $(entry).attr('data') + '"]').prop('hidden', false);
    $('input[data="' + $(entry).attr('data') + '"]').val($(entry).html());
  });
}

function discardEdits() {
  $.each($('entry[type="select"'), function(i, entry) {
    $('select#' + entry.id).prop('hidden', true);
    $(entry).prop('hidden', false);
  });

  $.each($('entry[type="text"'), function(i, entry) {
    $(entry).prop('hidden', false);
    $('input[data="' + $(entry).attr('data') + '"]').prop('hidden', true);
  });
}

function saveEdits(ticketId, session) {
  $('.edit-button').prop('disabled', true);

  var data = {
    session: session.session,
    ticketId: ticketId
  };
  var hasEdits = false;

  $.each($('entry[data][type="text"]'), function(i, tag) {
    var oldV = $(tag).html();
    var newV = $('input[data="' + $(tag).attr('data') + '"]').val();

    if (oldV != newV) {
      data[$(tag).attr('data')] = newV;
      console.log("new value for " + $(tag).attr('data') + " = " + newV);
      hasEdits = true;
    }
  });

  $.each($('entry[data][type="select"'), function(i, tag) {
    var oldV = $(tag).attr('value');
    var newV = $('select#' + tag.id).val();

    if (oldV != newV) {
      data[$(tag).attr('data')] = newV;
      console.log("new value for " + $(tag).attr('data') + " = " + newV);
      hasEdits = true;
    }
  });

  // TODO
  var oldV = $('entry#priority').html();
  var newV = $('select#priority').val();
  if (oldV != newV && !(oldV == '--' && newV == 'null') && newV != 'null') {
    data['Priority'] = newV;
    console.log("new value for Priority = " + newV);
    hasEdits = true;
  }

  if (hasEdits) {
    $.ajax({
        url: '/apiv2/tickets/edit',
        type: 'POST',
        data: data,
        async: false
      });
  }

}

function comment(ticketId, session, type) {
  var data = {
    session: session.session,
    ticketId: ticketId,
    comment: $('#commentBody').val(),
    type: type
  };

  var assigned = $('#submit-comment-assigned').val();
  if (assigned != 'null') {
    data['subtask_assigned'] = assigned;
  }

  $.ajax({
        url: '/apiv2/tickets/comment',
        type: 'POST',
        data: data,
        async: false
      });

  $('#new-comment-box').collapse('hide');
  $('#commentBody').val('');
}