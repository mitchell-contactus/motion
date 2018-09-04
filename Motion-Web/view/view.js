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
    url: '/api/tickets/view',
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
  card += '<h5>' + comment.UserName + '</h5>';
  card += comment.CreatedDate;
  card += '</div><div class="card-body">';
  card += comment.Entry;
  card += '</div></div>';
  $('#comments').append(card);
}

function buildEvent(event) {
  var line = '<li>';
  line += event.EventType;
  line += ' at ' + event.Timestamp;
  line += ' by ' + event.Name;
  line += '</li>';
  $('#history > ul').append(line);
}

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
        url: '/api/tickets/edit',
        type: 'POST',
        data: data,
        async: false
      });
  }

}